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

public partial class Tools_Password_lookupUserMaster : System.Web.UI.Page
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
            fillCostCenterAccessDropDown();
            txtSearch_TextChanged(null, null);
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
        txtSearch_TextChanged(null, null);
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
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
                          FROM ( SELECT Umt_UserCode [ID No] 
                                      , Umt_UserNickname [ID Code]
                                      , Umt_UserNickname [Nickname]
                                      , Umt_UserLname [Lastname]
                                      , Umt_UserFname [Firstname]
                                      , ISNULL(dbo.getCostCenterFullNameV2(Emt_CostCenterCode),'') [Costcenter]
                                   FROM T_UserMaster
                                   LEFT JOIN T_EmployeeMaster
                                     ON Emt_EmployeeId = Umt_UserCode
                                  WHERE ( Umt_Status = 'A'
                                         {0} )
                                     OR Umt_UserCode = @UserCode
                              ) AS temp)
                         SELECT [ID No] 
                              , [ID Code]
                              , [Nickname]
                              , [Lastname]
                              , [Firstname]
                              , [Costcenter]
                           FROM TempTable
                          WHERE Row BETWEEN @startIndex and @startIndex + @numRow - 1

                         SELECT COUNT(Umt_UserCode) [Rows]
                           FROM T_UserMaster
                           LEFT JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Umt_UserCode
                          WHERE ( Umt_Status = 'A'
                                 {0} )
                             OR Umt_UserCode = @UserCode ";

        sql = sql.Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString());
        sql = string.Format(sql, filterQuery());
        string transactionType = Request.QueryString["type"].ToString();
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[1] = new ParameterInfo("@TransactionType", transactionType);

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
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    dt.Columns.Remove("Nickname");
                }
                else
                {
                    dt.Columns.Remove("ID Code");
                }
            }
            else
            {
                dt.Columns.Remove("ID Code");
                dt.Columns.Remove("Nickname");
            }

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                dt.Columns.Remove("Lastname");
                dt.Columns.Remove("Firstname");
            }
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
            dgvResult.DataSource = new DataSet("dummy");
            dgvResult.DataBind();
        }
        hfEmployeeId.Value = string.Empty;
        hfEmployeeName.Value = string.Empty;
        hfEmployeeNickname.Value = string.Empty;
        UpdatePagerLocation();
        txtSearch.Focus();
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
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }

    protected void dgvResult_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
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
                                       + ", "
                                       + dgvResult.SelectedRow.Cells[2].Text;
            }
        }


    }
    #endregion

    #region Methods
    private void fillCostCenterAccessDropDown()
    {
        string sql = @" SELECT Uca_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Uca_CostCenterCode) [Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_UserCode = @UserCode
                           AND Uca_SytemId = @TransactionType
                           AND Uca_CostCenterCode <> 'ALL' 

                        SELECT DISTINCT Emt_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Emt_CostCenterCode) [Desc]
                          FROM T_EmployeeMaster
                         WHERE LEFT(Emt_JobStatus,1) = 'A'
                           AND ISNULL(Emt_CostCenterCode,'') <> ''";
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
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ddlCostCenter.Items.Add(new ListItem(ds.Tables[0].Rows[i]["Desc"].ToString()
                                                        , ds.Tables[0].Rows[i]["Code"].ToString()));
                }
            }
            else
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
        string filter = string.Empty;
        string searchFilter = @" AND ( ( Umt_UserCode LIKE '{0}%' )
                                    OR ( Umt_UserNickname LIKE '{0}%' )
                                    OR ( Umt_UserLname LIKE '{0}%' )
                                    OR ( Umt_UserFname LIKE '{0}%' )
                                    OR ( ISNULL(dbo.getCostCenterFullNameV2(Emt_CostCenterCode),'') LIKE '%{0}%' ) )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            filter += string.Format(searchFilter, holder);
        }

        if (ddlCostCenter.SelectedValue.Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), Request.QueryString["type"].ToString()))
            {
                filter += @"AND Emt_CostcenterCode IN (SELECT Uca_CostcenterCode
                                                         FROM T_UserCostcenterAccess
                                                        WHERE Uca_UserCode = @UserCode
                                                          AND Uca_SytemID = @TransactionType)";
            }
        }
        else
        {
            filter += string.Format(@"AND Emt_CostcenterCode = '{0}'", ddlCostCenter.SelectedValue.ToString());
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
