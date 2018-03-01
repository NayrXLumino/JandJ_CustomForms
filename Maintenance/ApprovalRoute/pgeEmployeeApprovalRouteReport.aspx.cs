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

public partial class Maintenance_ApprovalRoute_pgeEmployeeApprovalRouteReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFEMPROUTE"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
            PreRender += new EventHandler(Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport_PreRender);
        }
        LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport_LoadComplete);
    }

    #region Events
    void Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        btnEmployee.OnClientClick = string.Format("return lookupARRepEmployee()");
        btnRouteId.OnClientClick = string.Format("return lookupARRepRouteID()");
        btnTransaction.OnClientClick = string.Format("return lookupARRepTransaction()");
        btnChecker1.OnClientClick = string.Format("return lookupARRepCheckerApprover('Checker1')");
        btnChecker2.OnClientClick = string.Format("return lookupARRepCheckerApprover('Checker2')");
        btnApprover.OnClientClick = string.Format("return lookupARRepCheckerApprover('Approver')");
        btnCostcenter.OnClientClick = string.Format("return lookupARRepCostcenter()");

    }

    void Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport_PreRender(object sender, EventArgs e)
    {

    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
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
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];

                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "EMPLOYEE ROUTE REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Employee Route Report");
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
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];

                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "EMPLOYEE ROUTE REPORT", initializeExcelHeader());
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
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFEMPROUTE");
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
    }

    private string SQLBuilder(string replaceString)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Cost Center],[Employee Name]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@"                              ) AS temp)
                                           SELECT [Employee ID]
                                                , [Employee Name]
                                                , [Transaction]
                                                , [Cost Center]
                                                , [Start Date]
                                                , [End Date]
                                                , [Route ID]
                                                , [Checker 1 ID]
                                                , [Checker 1 Name]
                                                , [Checker 2 ID]
                                                , [Checker 2 Name]
                                                , [Approver ID]
                                                , [Approver Name]
                                                --, [Status]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            ");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(T_EmployeeApprovalRoute.Arm_RouteId) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        return sql.ToString().Replace("!#!", replaceString);
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"SELECT Arm_EmployeeId [Employee ID]
                         , ISNULL(dbo.GetControlEmployeeName(Arm_EmployeeId), 'NOT REGISTERED IN EMPLOYEE MASTER') [Employee Name]
                         , Tcm_TransactionDesc [Transaction]
                         , T_EmployeeApprovalRoute.Arm_RouteID [Route ID]
                         , Convert(varchar(10), T_EmployeeApprovalRoute.Arm_StartDate, 101) [Start Date]
                         , Convert(varchar(10), T_EmployeeApprovalRoute.Arm_EndDate, 101) [End Date]
                         , dbo.GetCostcenterFullNameV2(Emt_CostcenterCode) [Cost Center]
                         , Arm_Checker1 [Checker 1 ID]
                         , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1 Name]
                         , Arm_Checker2 [Checker 2 ID]
                         , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2 Name]
                         , Arm_Approver [Approver ID]
                         , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver Name]
                         , CASE T_EmployeeApprovalRoute.Arm_Status
	                       WHEN 'A' THEN 'ACTIVE'
	                       WHEN 'C' THEN 'INACTIVE' 
	                        END [Status]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        filter = @"   FROM T_EmployeeApprovalRoute
                      INNER JOIN T_EmployeeMaster
                        ON Emt_EmployeeId = Arm_EmployeeId
                      LEFT JOIN T_ApprovalRouteMaster
                        ON T_ApprovalRouteMaster.Arm_RouteID = T_EmployeeApprovalRoute.Arm_RouteID
                     INNER JOIN T_TransactionControlMaster
                        ON Tcm_TransactionCode = Arm_TransactionID
                       AND Tcm_Status = 'A' ";

        #region textBox Filters
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_EmployeeId {0}
                                           OR Emt_Firstname {0}
                                           OR Emt_Lastname {0}
                                           OR Emt_Middlename {0}
                                           OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
        }
        if (!txtRouteId.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( T_EmployeeApprovalRoute.Arm_RouteId {0})", sqlINFormat(txtRouteId.Text));
        }
        if (!txtTransaction.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_TransactionId {0}
                                           OR Tcm_TransactionDesc {0} )", sqlINFormat(txtTransaction.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_CostcenterCode {0}
                                           OR dbo.getCostCenterFullNameV2(Emt_CostcenterCode) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                     , Session["userLogged"].ToString());
        }
        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "GENERAL"))
        {
            filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                        FROM T_UserCostCenterAccess
                                                                    WHERE Uca_UserCode = '{0}'
                                                                        AND Uca_SytemId = 'GENERAL')
                                                    OR Arm_EmployeeId = '{0}'))", Session["userLogged"].ToString());
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_Checker1 {0}
                                           OR dbo.GetControlEmployeeNameV2(Arm_Checker1) {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_Checker2 {0}
                                           OR dbo.GetControlEmployeeNameV2(Arm_Checker2) {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_Approver {0}
                                           OR dbo.GetControlEmployeeNameV2(Arm_Approver) {0})", sqlINFormat(txtApprover.Text));
        }
        #endregion
        if (!ddlStatus.SelectedValue.Equals("ALL"))
        {
            filter += string.Format(@" AND T_EmployeeApprovalRoute.Arm_Status = '{0}'", ddlStatus.SelectedValue);
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( T_EmployeeApprovalRoute.Arm_RouteId LIKE '{0}%' )
                                          OR ( Tcm_TransactionDesc LIKE '%{0}%' )
                                          OR ( Arm_EmployeeId LIKE '%{0}%' )
                                          OR ( ISNULL(dbo.GetControlEmployeeName(Arm_EmployeeId), 'NOT REGISTERED IN EMPLOYEE MASTER') LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker1) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker2) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Approver) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(Arm_CostCenterCode) LIKE '%{0}%' )
                                          OR ( CASE T_EmployeeApprovalRoute.Arm_Status
                                               WHEN 'A' THEN 'ACTIVE' 
                                               WHEN 'C' THEN 'INACTIVE' 
                                                END LIKE '{0}%' )
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
        if (!txtRouteId.Text.Trim().Equals(string.Empty))
        {
            criteria += lblRouteId.Text + ":" + txtRouteId.Text.Trim() + "-";
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            criteria += lblCostcenter.Text + ":" + txtCostcenter.Text.Trim() + "-";
        }
        if (!ddlStatus.SelectedValue.Trim().Equals(string.Empty))
        {
            criteria += lblStatus.Text + ":" + ddlStatus.SelectedValue.Trim() + "-";
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker1.Text + ":" + txtChecker1.Text.Trim() + "-";
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker2.Text + ":" + txtChecker2.Text.Trim() + "-";
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            criteria += lblApprover.Text + ":" + txtApprover.Text.Trim() + "-";
        }
        return criteria.Trim();
    }
    #endregion

}
