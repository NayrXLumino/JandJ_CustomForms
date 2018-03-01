using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class Transactions_Leave_pgeLeaveCreditsReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLVECREDREP"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    PreRender += new EventHandler(Transactions_Leave_pgeLeaveCreditsReport_PreRender);
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveCreditsReport_LoadComplete);
        }
    }

    #region Events
    void Transactions_Leave_pgeLeaveCreditsReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "leaveScripts";
        string jsurl = "_leave.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupRLVEmployee()");
        btnCostcenter.OnClientClick = string.Format("return lookupRLVCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRLVCostcenterLine()");
        btnLeaveYear.OnClientClick = string.Format("return lookupRLVLeaveYear()");
        btnLeaveType.OnClientClick = string.Format("return lookupRLVType()");
        
    }

    void Transactions_Leave_pgeLeaveCreditsReport_PreRender(object sender, EventArgs e)
    {

    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFLVECREDREP", txtEmployee.Text, true, Session["userLogged"].ToString());
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        bindGrid();
        UpdatePagerLocation();
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                                
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "LEAVE CREDITS REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Leave Credits Report");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "LEAVE REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        btnGenerate.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFLVECREDREP");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);

            txtEmployee.Text = Session["userLogged"].ToString();
            tbrEmployee.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
        }
        else
        {
            btnExport.Enabled = false;
            btnPrint.Enabled = false;
        }

        if (MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF"))
        {
            tbrCostcenterLine.Visible = true;
        }
        else
        {
            tbrCostcenterLine.Visible = false;
        }

      
    }

    private string SQLBuilder(string stringReplace)
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        StringBuilder sql = new StringBuilder();
        sql.Append(string.Format(@"
                            declare @LHRSINDAY as decimal(6,3);
                            declare @LVHRENTRY as char(1);
                            SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                                FROM T_ParameterMaster 
                                                WHERE Pmt_ParameterId = 'LHRSINDAY');
                            SET @LVHRENTRY = '{0}';
                            declare @startIndex int;
                            SET @startIndex = (@pageIndex * @numRow) + 1;
                           WITH TempTable AS ( SELECT Row_Number()
                                                 OVER ( ORDER BY [Lastname] ASC, [Leave Year] ASC, [Leave Type] ASC) [Row]
                                                    , *
                                                 FROM ( ", (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS) ? '1' : '0')));
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeLeave", "T_EmployeeLeaveHist"));
        sql.Append(string.Format(@"                  ) AS temp)
                                               SELECT [ID No]
                                                    , [Nickname]
                                                    , [ID Code]
                                                    , [Lastname]
                                                    , [Firstname]
                                                    , [MI]
                                                    , [Leave Year]
                                                    , [Leave Type]
                                                    , [Entitled]
                                                    , [Used]
                                                    , [Pending]
                                                    , [Balance]
                                                    , [Costcenter]
                                                    {0}
                                                 FROM TempTable
                                                !#! WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                                ", !hasCCLine ? "" : ", [CC Line]"));
        sql.Append(@" SELECT SUM(cnt)
                            FROM ( SELECT COUNT(Elm_EmployeeId) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"           UNION 
                                   SELECT COUNT(Elm_EmployeeId)");
        sql.Append(getFilters().Replace("T_EmployeeLeave", "T_EmployeeLeaveHist"));
        sql.Append(@"        ) as Rows");

        return sql.ToString().Replace("!#!", stringReplace);
    }

    private string getColumns()
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string columns = string.Empty;
        columns = string.Format(@" SELECT Elm_EmployeeId [ID No]
						                , Emt_NickName [Nickname]
						                , Emt_NickName [ID Code]
						                , Emt_LastName [Lastname]
						                , Emt_FirstName [Firstname]
						                , LEFT(Emt_MiddleName, 1) [MI]
						                , Elm_LeaveYear [Leave Year]
						                , Ltm_LeaveDesc [Leave Type]
                                        , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                                        THEN Elm_Entitled
                                                                        ELSE Elm_Entitled / @LHRSINDAY 
                                                                    END )[Entitled]
                                        , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                                        THEN Elm_Used
                                                                        ELSE Elm_Used / @LHRSINDAY 
                                                                    END )[Used]
                                        , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                                        THEN Elm_Reserved
                                                                        ELSE Elm_Reserved / @LHRSINDAY 
                                                                    END )[Pending]
                                        , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                                        THEN Elm_Entitled - Elm_Used - Elm_Reserved 
                                                                        ELSE (Elm_Entitled / @LHRSINDAY ) - (Elm_Used / @LHRSINDAY) - (Elm_Reserved / @LHRSINDAY)
                                                                    END )[Balance]
						                , dbo.getCostCenterFullNameV2( Emt_CostCenterCode ) [Costcenter]
                                              {0}", !hasCCLine ? "" : ", Ecm_LineCode [CC Line]");
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@"   FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        INNER JOIN T_EmployeeMaster
						   ON Emt_EmployeeID = Elm_EmployeeId
                         LEFT JOIN E_EmployeeCostCenterLineMovement
                           ON Ecm_EmployeeID = Elm_EmployeeId
                          AND GETDATE() BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                          AND Ecm_Status = 'A'
                            {0}
                         WHERE 1 = 1 ", !hasCCLine ? @"---apsungahid added for line code access filter 20141121
                                                            LEFT JOIN T_UserCostCenterAccess [UCA1]
                                                          ON UCA1.Uca_CostcenterCode = Emt_CostCenterCode
                                                         AND UCA1.Uca_CostcenterCode <> 'ALL'
                                                         AND UCA1.Uca_Status = 'A'

                                                        LEFT JOIN T_UserCostCenterAccess [UCA2]
                                                          ON UCA2.Uca_CostcenterCode = 'ALL'
                                                         AND UCA2.Uca_Status = 'A'

                                                        LEFT JOIN E_UserCostcenterLineAccess [UCL1]
                                                          ON UCL1.Ucl_CostCenterCode = IIF(UCA2.Uca_CostCenterCode IS NOT NULL, UCL1.Ucl_CostCenterCode, UCA1.Uca_CostCenterCode)
                                                         AND UCL1.Ucl_SystemID = 'LEAVE'
                                                         AND UCL1.Ucl_Status = 'A'
                                                         AND UCL1.Ucl_LineCode <> 'ALL'

                                                        LEFT JOIN E_UserCostcenterLineAccess [UCL2]
                                                          ON UCL2.Ucl_CostCenterCode = IIF(UCA2.Uca_CostCenterCode IS NOT NULL, UCL2.Ucl_CostCenterCode, UCA1.Uca_CostCenterCode)
                                                         AND UCL2.Ucl_SystemID = 'LEAVE'
                                                         AND UCL2.Ucl_Status = 'A'
                                                         AND UCL2.Ucl_LineCode = 'ALL'

                                                        LEFT JOIN E_CostCenterLineMaster [CLM1]
                                                          ON CLM1.Clm_CostCenterCode = IIF(UCL2.Ucl_CostCenterCode IS NOT NULL, CLM1.Clm_CostCenterCode, UCA1.Uca_CostCenterCode)
                                                         AND CLM1.Clm_Status = 'A' 

                                                        LEFT JOIN E_CostCenterLineMaster [CLM2]
                                                          ON CLM2.Clm_CostCenterCode = Emt_CostCenterCode
                                                         AND CLM2.Clm_Status = 'A'" :  "");
        #region textBox Filters
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_EmployeeId {0} )", sqlINFormat(txtEmployee.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_Costcentercode {0} ) ", sqlINFormat(txtCostcenter.Text)
                                                                                                 , Session["userLogged"].ToString());
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
        }

        if (hasCCLine)//flag costcenter line to retain old code
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
            {
                filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = 'LEAVE')
                                                                        OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }
            if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Ecm_LineCode {0} ) ", sqlINFormat(txtCostcenterLine.Text));
            }
        }
        else
        {
            filter += string.Format(@" AND ( ( ( UCA1.Uca_UserCode = '{0}'
                                                 AND UCA1.Uca_SytemId = 'LEAVE') 
	                                            OR ( UCA2.Uca_UserCode = '{0}'
                                                 AND UCA2.Uca_SytemId = 'LEAVE') )
                                            AND ( 'ALL' = IIF(CLM2.Clm_CostCenterCode IS NULL, 'ALL', IIF(UCL2.Ucl_CostcenterCode IS NOT NULL, 'ALL', ''))
	                                            OR Ecm_LineCode = IIF(UCL2.Ucl_CostcenterCode IS NOT NULL, Ecm_LineCode , UCL1.Ucl_LineCode) )
                                             OR Emt_EmployeeId = '{0}' )", Session["userLogged"].ToString());
        }

        if (!txtLeaveYear.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Elm_LeaveYear {0})", sqlINFormat(txtLeaveYear.Text));
        }
        if (!txtLeaveType.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Elm_LeaveType {0}
                                               OR Ltm_LeaveDesc {0})", sqlINFormat(txtLeaveType.Text));
        }
        #endregion
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            searchFilter = @"AND ( ( Elm_LeaveYear LIKE '{0}%' )
                                    OR ( Elm_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                    OR ( CONVERT(VARCHAR(20), (CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                        THEN Elm_Entitled
                                                        ELSE Elm_Entitled / @LHRSINDAY 
                                                    END ))) LIKE '{0}%' )
                                    OR ( CONVERT(VARCHAR(20), (CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                        THEN Elm_Used
                                                        ELSE Elm_Used / @LHRSINDAY 
                                                    END ))) LIKE '{0}%' )
                                    OR ( CONVERT(VARCHAR(20), (CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                        THEN Elm_Reserved
                                                        ELSE Elm_Reserved / @LHRSINDAY 
                                                    END ))) LIKE '{0}%' )
                                    OR ( CONVERT(VARCHAR(20), (CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                        THEN Elm_Entitled - Elm_Used - Elm_Reserved 
                                                        ELSE (Elm_Entitled / @LHRSINDAY ) - (Elm_Used / @LHRSINDAY) - (Elm_Reserved / @LHRSINDAY)
                                                    END ))) LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Emt_Costcentercode) LIKE '%{0}%' )
                                    @LINECODE)";
            if (hasCCLine)
            {
                searchFilter = searchFilter.Replace("@LINECODE", " OR  ( Ecm_LineCode LIKE '{0}%' ) ");
            }
            else
            {
                searchFilter = searchFilter.Replace("@LINECODE", "");
            }
        }

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            filter += string.Format(searchFilter, holder);
        }

        return filter;
    }

    private string sqlINFormat(string text)
    {
        string[] temp = text.Split(',');
        string value = "IN ( ";
        for (int i = 0; i < temp.Length; i++)
        {
            value += "'" + temp[i].Trim() + "'";
            if (i != temp.Length - 1)
                value += ",";
        }
        value += ")";
        return value;
    }
    
    private void UpdatePagerLocation()
    {
        int pageIndex = Convert.ToInt32(hfPageIndex.Value);
        int numRows = Convert.ToInt32(hfNumRows.Value);
        int rowCount = Convert.ToInt32(hfRowCount.Value);
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRowNo.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
            #region Remove Columns
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    ds.Tables[0].Columns.Remove("Nickname");
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                }
            }
            else
            {
                ds.Tables[0].Columns.Remove("ID Code");
                ds.Tables[0].Columns.Remove("Nickname");
            }

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                ds.Tables[0].Columns.Remove("Lastname");
                ds.Tables[0].Columns.Remove("Firstname");
                ds.Tables[0].Columns.Remove("MI");
            }

            #endregion
        }
        hfRowCount.Value = "0";
        foreach (DataRow dr in ds.Tables[1].Rows)
            hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    private string initializeExcelHeader()
    {
        string criteria = string.Empty;
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            criteria += lblEmployee.Text + ":" + txtEmployee.Text.Trim() + "-";
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            criteria += lblCostcenter.Text + ":" + txtCostcenter.Text.Trim() + "-";
        }
        if (!txtLeaveYear.Text.Trim().Equals(string.Empty))
        {
            criteria += lblPayPeriod.Text + ":" + txtLeaveYear.Text.Trim() + "-";
        }
        if (!txtLeaveType.Text.Trim().Equals(string.Empty))
        {
            criteria += lblLeaveType.Text + ":" + txtLeaveType.Text.Trim() + "-";
        }
        
        return criteria.Trim();
    }
    #endregion
}