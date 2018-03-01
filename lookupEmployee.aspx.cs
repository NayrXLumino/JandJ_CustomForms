using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class lookupEmployee : System.Web.UI.Page
{
    #region Class Variable
    static int pageIndex = 0;
    static int rowCount = 0;
    static int numRows = Convert.ToInt32(Resources.Resource.LOOKUPPAGEITEMS);
    CommonMethods methods = new CommonMethods();
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        if (!Page.IsPostBack)
        {
            LoadComplete += new EventHandler(lookupEmployee_LoadComplete);
            lblPage.Text = "0 of 0";
            fillCostCenterAccessDropDown();

            if (Convert.ToBoolean(Resources.Resource.BINDEMPLOYEELOOKUP))
            {
                txtSearch_TextChanged(null, null);
            }

            txtSearch.Focus();
        }
    }

    void lookupEmployee_LoadComplete(object sender, EventArgs e)
    {
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
    }

    #region Event
    protected void ddlCostCenter_SelectedIndexChanged(object sender, EventArgs e)
    {
        pageIndex = 0;
        rowCount = 0;
        txtSearch_TextChanged(null, null);
        hfEmployeeId.Value = string.Empty;
        hfEmployeeName.Value = string.Empty;
        hfEmployeeNickname.Value = string.Empty;
        UpdatePagerLocation();
        txtSearch.Focus();
    }
    
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        bool isUnifiedEmployees = false;

        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        try
        {
            string isUnified = Request.QueryString["uni"].ToString();
            if (isUnified == "1")
            {
                isUnifiedEmployees = true;
            }
        }
        catch
        {
            isUnifiedEmployees = false;
        }

        if (isUnifiedEmployees)
        {
            #region Unified Employees
            ForUnifiedEmployeeLookup(sender);
            #endregion
        }
        else
        {
            #region Default Lookup
            if (sender != null)
            {
                pageIndex = 0;
                rowCount = 0;
            }
            DataSet ds = new DataSet();
            string sql = @"declare @startIndex int;
                           SET @startIndex = (@pageIndex * @numRow) + 1;

                          WITH TempTable AS (
                        SELECT Row_Number() OVER (Order by [ID No]) [Row], *
                          FROM ( SELECT Emt_EmployeeId [ID No]
                                      , Emt_Lastname + ', ' + Emt_Firstname [Name]
                                      , Emt_NickName [Nickname]
                                      , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Costcenter]
                                      , ISNULL(Dcm_Departmentdesc, '') [Department]
                                   FROM T_EmployeeMaster
                                   LEFT JOIN T_DepartmentCodeMaster
	                                 ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                                  THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                                  ELSE ''
							                                  END

                                     {2}
                                  WHERE LEFT(Emt_JobStatus,1) = 'A'
                                  {0}
                              ) AS temp)
                            {1}
                           FROM TempTable
                          WHERE Row BETWEEN @startIndex and @startIndex + @numRow - 1

                         SELECT COUNT(Emt_EmployeeId) [Rows]
                           FROM T_EMployeeMaster

                                     {2}
                          WHERE LEFT(Emt_JobStatus,1) = 'A'
                            {0} ";

            string sqlSelect = @" SELECT [ID No] 
                                   , [Name]
                                   , [Nickname]
                                   , [Costcenter]";
            switch (Resources.Resource._3RDINFO.ToString().ToUpper())
            {
                case "COSTCENTER":
                    sqlSelect = @" SELECT [ID No] 
                                    , [Name]
                                    , [Costcenter]
                                    , [Nickname]";
                    break;
                case "NICKNAME":
                    sqlSelect = @" SELECT [ID No] 
                                    , [Name]
                                    , [Nickname]
                                    , [Costcenter]";
                    break;
                case "DEPARTMENT":
                    sqlSelect = @" SELECT [ID No] 
                                    , [Name]
	                                , [Department]
                                    , [Costcenter]";
                    break;
                default:
                    break;
            }

            sql = sql.Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString());
            sql = string.Format(sql, filterQuery(), sqlSelect, (!hasCCLine ? "" : @"LEFT JOIN (SELECT * FROM dbo.GetAllLatestCostCenterLines(GETDATE(), DATEADD(month, 5, GETDATE()))) EmployeeCostCenterLineMovement ON Ecm_EmployeeID = Emt_EmployeeID"));
            string transactionType = Request.QueryString["type"].ToString();
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString(), SqlDbType.VarChar, 15);
            param[1] = new ParameterInfo("@TransactionType", transactionType, SqlDbType.VarChar, 10);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (!CommonMethods.isEmpty(ds))
            {
                DataTable dt = ds.Tables[0].Copy();
                rowCount = 0;
                #region Remove columns
                //if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                //{
                //    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                //    {
                //        dt.Columns.Remove("Nickname");
                //    }
                //    else
                //    {
                //        dt.Columns.Remove("ID Code");
                //    }
                //}
                //else
                //{
                //    dt.Columns.Remove("ID Code");
                //    dt.Columns.Remove("Nickname");
                //}

                //if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                //{
                //    dt.Columns.Remove("Lastname");
                //    dt.Columns.Remove("Firstname");
                //}
                #endregion
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    rowCount += Convert.ToInt32(dr[0]);
                }
                dgvResult.DataSource = dt;
                dgvResult.DataBind();
            }
            else
            {
                dgvResult.DataSource = ds;
                dgvResult.DataBind();
            }
            hfEmployeeId.Value = string.Empty;
            hfEmployeeName.Value = string.Empty;
            hfEmployeeNickname.Value = string.Empty;
            UpdatePagerLocation();
            txtSearch.Focus();
            #endregion
        }
    }

    protected void PageButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            pageIndex--;
        else if (((Button)sender).ID == "btnNext")
            pageIndex++;

        this.txtSearch_TextChanged(null, null);
        UpdatePagerLocation();
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 1);
    }

    protected void dgvResult_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.dgvResult, "Select$" + e.Row.RowIndex);
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValue('" + e.Row.RowIndex + "')"); ;
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')"); ;
        }
    }

    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfEmployeeId.Value = dgvResult.SelectedRow.Cells[0].Text;
        if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
        {
            hfEmployeeNickname.Value = dgvResult.SelectedRow.Cells[1].Text;

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                hfEmployeeName.Value = dgvResult.SelectedRow.Cells[3].Text;
            }
            else
            {
                hfEmployeeName.Value = dgvResult.SelectedRow.Cells[2].Text 
                                     + ", " 
                                     + dgvResult.SelectedRow.Cells[3].Text;
            }
        }
        else
        {
            hfEmployeeNickname.Value = string.Empty;
            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                hfEmployeeName.Value = dgvResult.SelectedRow.Cells[2].Text;
            }
            else
            {
                hfEmployeeName.Value = dgvResult.SelectedRow.Cells[1].Text
                                       +", "
                                       + dgvResult.SelectedRow.Cells[2].Text;
            }
        }

        
    }
    #endregion

    #region Methods
    //Perth Added 11/29/2012
    private string LoopQueryThroughProfile(string Query)
    {
        string ret = string.Empty;

        using (DALHelper dal = new DALHelper(false))
        {
            try
            {
                dal.OpenDB();
                DataTable dt = dal.ExecuteDataSet(@"
SELECT 
	Prf_Database
FROM T_PRofiles
WHERE Prf_Status = 'A'
AND Prf_ProfileType IN ('P', 'S', 'T')
").Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0)
                        ret += " UNION ";
                    ret += Query.Replace("@PROFILE", dt.Rows[i]["Prf_Database"].ToString().Trim());
                }
            }
            catch (Exception er)
            {
                CommonMethods.ErrorsToTextFile(er, "LoopQueryThroughProfile()");
                ret = Query;
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return ret;
    }

    private void ForUnifiedEmployeeLookup(object sender)
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        if (sender != null)
        {
            pageIndex = 0;
            rowCount = 0;
        }
        DataSet ds = new DataSet();
        string sqlEmployees = string.Format(@"
SELECT DISTINCT Emt_EmployeeId [ID No]
    , Emt_Lastname + ', ' + Emt_Firstname [Name]
    , Emt_NickName [Nickname]
    , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Costcenter]
    , ISNULL(Dcm_Departmentdesc, '') [Department]
FROM T_EmployeeMaster
LEFT JOIN T_DepartmentCodeMaster
	ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								ELSE ''
							END
{2}
WHERE LEFT(Emt_JobStatus,1) = 'A'
{0}

UNION

SELECT DISTINCT Emt_EmployeeId [ID No]
    , Emt_Lastname + ', ' + Emt_Firstname [Name]
    , Emt_NickName [Nickname]
    , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Costcenter]
    , ISNULL(Dcm_Departmentdesc, '') [Department]
FROM {1}..T_EmployeeMaster
LEFT JOIN T_DepartmentCodeMaster
	ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								ELSE ''
							END
{2}
WHERE LEFT(Emt_JobStatus,1) = 'A'
{0}
"
            , filterQuery(), Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString()), (!hasCCLine ? "" : @"---apsungahid added for line code access filter 20141211
                                                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                                                      FROM E_CostCenterLineMaster 
												                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
										                                                                                       ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                                                           AND GETDATE() BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))"));

        string sqlEmployeeCount = string.Format(@"
SELECT DISTINCT Emt_EmployeeId 
FROM T_EMployeeMaster
{2}
WHERE LEFT(Emt_JobStatus,1) = 'A'
{0}

UNION

SELECT DISTINCT Emt_EmployeeId 
FROM {1}..T_EMployeeMaster
{2}

WHERE LEFT(Emt_JobStatus,1) = 'A'
{0}
"
            , filterQuery(), Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString()), (!hasCCLine ? "" : @"---apsungahid added for line code access filter 20141211
                                                                                                                               LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
								                                                                                                          FROM E_CostCenterLineMaster 
								                                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
					                                                                                                           ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                                                           AND GETDATE() BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE())) "));

        string sql = @"declare @startIndex int;
                           SET @startIndex = (@pageIndex * @numRow) + 1;

                          WITH TempTable AS (
                        SELECT Row_Number() OVER (Order by [ID No]) [Row], *
                          FROM ( 
                                  {0}
                              ) AS temp)
                            {1}
                           FROM TempTable
                          WHERE Row BETWEEN @startIndex and @startIndex + @numRow - 1

                         SELECT COUNT(Emt_EmployeeId) [Rows]
                           FROM (
                            {2} ) AS COUNTTABLE";

        string sqlSelect = @" SELECT [ID No] 
                                   , [Name]
                                   , [Nickname]
                                   , [Costcenter]";
        switch (Resources.Resource._3RDINFO.ToString().ToUpper())
        {
            case "COSTCENTER":
                sqlSelect = @" SELECT DISTINCT [ID No] 
                                    , [Name]
                                    , [Costcenter]
                                    , [Nickname]";
                break;
            case "NICKNAME":
                sqlSelect = @" SELECT DISTINCT [ID No] 
                                    , [Name]
                                    , [Nickname]
                                    , [Costcenter]";
                break;
            case "DEPARTMENT":
                sqlSelect = @" SELECT DISTINCT [ID No] 
                                    , [Name]
	                                , [Department]
                                    , [Costcenter]";
                break;
            default:
                break;
        }

        sql = sql.Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString());
        sql = string.Format(sql, sqlEmployees, sqlSelect, sqlEmployeeCount);
        string transactionType = Request.QueryString["type"].ToString();
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString(), SqlDbType.VarChar, 15);
        param[1] = new ParameterInfo("@TransactionType", transactionType, SqlDbType.VarChar, 10);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (!CommonMethods.isEmpty(ds))
        {
            DataTable dt = ds.Tables[0].Copy();
            rowCount = 0;
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                rowCount += Convert.ToInt32(dr[0]);
            }
            dgvResult.DataSource = dt;
            dgvResult.DataBind();
        }
        else
        {
            dgvResult.DataSource = ds;
            dgvResult.DataBind();
        }
        hfEmployeeId.Value = string.Empty;
        hfEmployeeName.Value = string.Empty;
        hfEmployeeNickname.Value = string.Empty;
        UpdatePagerLocation();
        txtSearch.Focus();
    }

    private void fillCostCenterAccessDropDown()
    {
        string sql = @" SELECT Uca_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Uca_CostCenterCode) [Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_UserCode = @UserCode
                           AND Uca_SytemId = @TransactionType
                           AND Uca_CostCenterCode <> 'ALL'

                        SELECT Cct_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Cct_CostCenterCode) [Desc]
                          FROM T_Costcenter
                         WHERE Cct_Status = 'A' ";
        string transactionType = Request.QueryString["type"].ToString();
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[1] = new ParameterInfo("@TransactionType", transactionType);

        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
        ddlCostCenter.Items.Clear();
        ddlCostCenter.Items.Add(new ListItem("ALL", "ALL"));

        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ddlCostCenter.Items.Add(new ListItem(ds.Tables[0].Rows[i]["Desc"].ToString()
                                                    , ds.Tables[0].Rows[i]["Code"].ToString()));
            }
        }
        else
        {
            if (CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), Request.QueryString["type"].ToString()))
            {

                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    ddlCostCenter.Items.Add(new ListItem(ds.Tables[1].Rows[i]["Desc"].ToString()
                                                        , ds.Tables[1].Rows[i]["Code"].ToString()));
                }    
            }
        }
    }
    private string filterQuery()
    {
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        
        string filter = string.Empty;

        string searchFilter = @"AND (    ( Emt_EmployeeId LIKE '{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_Lastname LIKE '{0}%' )
                                      OR ( Emt_Firstname LIKE '{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(Emt_CostCenterCode) LIKE '%{0}%' )
                                    )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            filter += string.Format(searchFilter, holder);
        }

//////        if (ddlCostCenter.SelectedValue.Equals("ALL"))
//////        {
//////            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), Request.QueryString["type"].ToString()))
//////            {
//////                filter += @"AND Emt_CostcenterCode IN (SELECT Uca_CostcenterCode
//////                                                      FROM T_UserCostcenterAccess
//////                                                     WHERE Uca_UserCode = @UserCode
//////                                                       AND Uca_SytemID = @TransactionType)";
//////            }
//////        }
//////        else
//////        {
//////            filter += string.Format(@"AND Emt_CostcenterCode = '{0}'", ddlCostCenter.SelectedValue.ToString());
//////        }

        if (ddlCostCenter.SelectedValue.Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), Request.QueryString["type"].ToString()))
            {
                filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = @TransactionType)
                                                                        OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }

            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND (Emt_CostCenterCode IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', @TransactionType) WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									OR Emt_CostCenterCode + ISNULL(Ecm_LineCode,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', @TransactionType))
                                    OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        else
        {
            filter += string.Format(@"AND Emt_CostCenterCode = '{0}'", ddlCostCenter.SelectedItem.Value);

            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND (Emt_CostCenterCode IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', @TransactionType) WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									OR Emt_CostCenterCode + ISNULL(Ecm_LineCode,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', @TransactionType))
                                    OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }

        return filter;

    }
    
    private void UpdatePagerLocation()
    {
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblPage.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }
    #endregion

    
}
