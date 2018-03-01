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

public partial class Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFROUTEREP"))
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
        btnRouteId.OnClientClick = string.Format("return lookupARRepRouteID()");
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

                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "APPROVAL ROUTE REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Approval Route Report");
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

                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "APPROVAL ROUTE REPORT", initializeExcelHeader());
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
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFROUTEREP");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);
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
                                             OVER ( ORDER BY [Route ID]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@"                              ) AS temp)
                                           SELECT [Route ID]
                                                , [Checker 1]
                                                , [Checker 2]
                                                , [Approver]
                                                , [Cost Center Assignment]
                                                , [Status]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            ");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(Arm_RouteId) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        return sql.ToString().Replace("!#!", replaceString);
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"SELECT Arm_RouteId [Route ID]
	                     , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
	                     , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
	                     , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
	                     , CASE WHEN Arm_CostCenterCode != 'ALL'
							THEN dbo.getCostCenterFullNameV2(Arm_CostCenterCode) 
							ELSE Arm_CostCenterCode
						    END [Cost Center Assignment]
	                     , CASE Arm_Status
	                       WHEN 'A' THEN 'ACTIVE'
	                       WHEN 'C' THEN 'INACTIVE'
	                        END [Status]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        filter = @"   FROM T_ApprovalRouteMaster
                     WHERE 1 = 1 ";

        #region textBox Filters
        if (!txtRouteId.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_RouteId {0})", sqlINFormat(txtRouteId.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Arm_CostcenterCode {0}
                                           OR dbo.getCostCenterFullNameV2(Arm_CostcenterCode) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                     , Session["userLogged"].ToString());
        }
        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "GENERAL"))
        {
            filter += string.Format(@" AND  (  ( Arm_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                        FROM T_UserCostCenterAccess
                                                                    WHERE Uca_UserCode = '{0}'
                                                                        AND Uca_SytemId = 'GENERAL')))", Session["userLogged"].ToString());
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
            filter += string.Format(@" AND Arm_Status = '{0}'", ddlStatus.SelectedValue);
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Arm_RouteId LIKE '{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker1) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker2) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Approver) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(Arm_CostCenterCode) LIKE '%{0}%' )
                                          OR ( CASE Arm_Status
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