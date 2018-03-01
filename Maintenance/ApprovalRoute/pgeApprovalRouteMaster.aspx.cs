using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Payroll.DAL;
using CommonLibrary;

public partial class Maintenance_ApprovalRoute_pgeApprovalRouteMaster : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    GeneralBL GNBL = new GeneralBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFRTEMASTER"))
                {
                    Response.Redirect(@"../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    txtSearch_TextChanged(null, null);
                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                    Page.PreRender += new EventHandler(Page_PreRender);
                }
            }
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeApprovalRouteMaster_LoadComplete);
        }
    }

    

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    #region Events
    void Maintenance_ApprovalRoute_pgeApprovalRouteMaster_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtRouteId.Attributes.Add("readOnly", "true");
        txtChecker1Id.Attributes.Add("readOnly", "true");
        txtChecker2Id.Attributes.Add("readOnly", "true");
        txtApproverId.Attributes.Add("readOnly", "true");
        txtCostCenterCode.Attributes.Add("readOnly", "true");
        btnChecker1.OnClientClick = string.Format("return lookupARCheckerApprover('txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupARCheckerApprover('txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupARCheckerApprover('txtApprover')");

        btnCostcenter.OnClientClick = string.Format("return lookupARCostCenter('txtCostCenter')");
        cbxAllCostcenter.Attributes.Add("onclick", "allCostcenter()");

    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
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

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValueToControlNo('" + e.Row.RowIndex + "')");
            e.Row.Attributes.Add("ondblclick", "javascript:return lookupARRouteUsage('" + e.Row.RowIndex + "')"); ;
        }
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                string err = string.Empty;
                switch (btnX.Text.ToUpper())
                { 
                    case "NEW":
                        VIEWER.ActiveViewIndex = 1;
                        btnX.Text = "CREATE";
                        btnY.Text = "CANCEL";
                        //txtRouteId.Text = "AUTO-GENERATED";
                        txtChecker1Id.Text = string.Empty;
                        txtChecker1Name.Text = string.Empty;
                        txtChecker2Id.Text = string.Empty;
                        txtChecker2Name.Text = string.Empty;
                        txtApproverId.Text = string.Empty;
                        txtApproverName.Text = string.Empty;
                        txtRouteId.Text = string.Empty;
                        ddlStatus.SelectedIndex = 0;
                        break;
                    case "CREATE":
                        err = checkEntry();
                        if (err.Equals(string.Empty))
                        {
                            try
                            {
                                GNBL.CreateApprovalRoute(PopulateDr(), cbxForJobMod.Checked);
                                MessageBox.Show(string.Format("Successfully created {0} approval route.", NewRouteID));
                                btnX.Text = "NEW";
                                btnY.Text = "EDIT";
                                btnInUse.Visible = false;
                                VIEWER.ActiveViewIndex = 0;
                                txtSearch_TextChanged(null, null);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show(err);
                        }
                        break;
                    case "SAVE":
                        err = checkEntry();
                        if (err.Equals(string.Empty))
                        {
                            GNBL.UpdateApprovalRoute(PopulateDr());
                            MessageBox.Show("Successfully updated approval route.");
                            btnX.Text = "NEW";
                            btnY.Text = "EDIT";
                            btnInUse.Visible = false;
                            VIEWER.ActiveViewIndex = 0;
                            txtSearch_TextChanged(null, null);
                        }
                        else
                        {
                            MessageBox.Show(err);
                        }
                        break;
                    default:
                        break;
                }
            }  
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }         
    }

    protected void btnY_Click(object sender, EventArgs e)
    {
        switch (btnY.Text.ToUpper())
        {
            case "EDIT":
                if (!txtRouteId.Text.Equals(string.Empty))
                {
                    VIEWER.ActiveViewIndex = 1;
                    setDetailInformation();
                    btnX.Text = "SAVE";
                    btnY.Text = "CANCEL";
                }
                else
                {
                    MessageBox.Show("Select route from list.");
                }
                break;
            case "CANCEL":
                VIEWER.ActiveViewIndex = 0;
                txtRouteId.Text = string.Empty;
                btnX.Text = "NEW";
                btnY.Text = "EDIT";
                btnInUse.Visible = false;
                break;
            default:
                break;
        }
        Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
    }

    protected void ddlStatusList_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFRTEMASTER");
        btnX.Enabled = userGrant.canAdd();
        btnY.Enabled = userGrant.canEdit();

        if (!Convert.ToBoolean(Resources.Resource.ENABLEAPPROVALCOSTCENTER))
        {
            txtCostCenterCode.Text = "ALL";
            txtCostCenterCode.ReadOnly = true;
            btnCostcenter.Enabled = false;
            txtCostCenterDesc.Text = "AVAILABLE TO ALL COSTCENTERS";
            txtCostCenterDesc.ReadOnly = true;
            cbxAllCostcenter.Checked = true;
            cbxAllCostcenter.Enabled = false;
        }
        
        if (!Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            cbxForJobMod.Visible = false;
        }
        if (Convert.ToBoolean(Resources.Resource.LEARSPECIFIC))
        {
            cbxForJobMod.Checked = true;
        }
    }

    private string checkEntry()
    {
        string errMsg = string.Empty;
        if (txtChecker1Id.Text.Equals(string.Empty))
        { 
            errMsg += "\nChecker 1 required.";
        }
        if (txtChecker2Id.Text.Equals(string.Empty))
        {
            errMsg += "\nChecker 2 required.";
        }
        if (txtApproverId.Text.Equals(string.Empty))
        {
            errMsg += "\nApprover required.";
        }
        if (errMsg.Equals(string.Empty))
        {
            if (btnX.Text.Equals("CREATE"))
            {
                string routeId = GNBL.GetRouteID(txtChecker1Id.Text, txtChecker2Id.Text, txtApproverId.Text);
                if (!routeId.Equals(string.Empty))
                {
                    errMsg += "\nApproval Route already exists. Refer to: " + routeId;
                }
                if (!CheckAppRouteExists())
                {
                    errMsg += "\nAPRROUTE does not exists in Transaction Control Master table.";
                }
            }
            else if (btnX.Text.Equals("SAVE"))
            {
                if (ddlStatus.SelectedValue.Equals("C"))
                {
                    int countUse = GNBL.countRouteUsage(txtRouteId.Text);
                    if (countUse > 0)
                    {
                        errMsg += "\nCannot deactivate approval route.\nThere are " + countUse.ToString() + " using the route.";
                        btnInUse.Visible = true;
                    }
                    else
                    {
                        btnInUse.Visible = false;
                    }
                }
            }
        }
        return errMsg;
    }

    private bool CheckAppRouteExists()
    {
        string query = string.Format(@"SELECT * FROM T_TransactionControlMaster
                                            WHERE Tcm_TransactionCode = 'APRROUTE'");
        DataSet ds;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }

        return (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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

    private string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;

                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Route ID] DESC) [Row]
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
                                            WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(Arm_RouteId) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"SELECT Arm_RouteId [Route ID]
                         , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
                         , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
                         , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
                         , CASE WHEN Arm_CostcenterCode <> 'ALL'
                                THEN dbo.getCostCenterFullNameV2(Arm_CostcenterCode)
                                ELSE 'AVAILABLE TO ALL COSTCENTERS'
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
        filter = string.Format(@"   FROM T_ApprovalRouteMaster
                                   WHERE Arm_Status = '{0}' ", ddlStatusList.SelectedValue);
        
        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "GENERAL"))
        {
            filter += string.Format(@" AND  (  Arm_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                         FROM T_UserCostCenterAccess
                                                                        WHERE Uca_UserCode = '{0}'
                                                                          AND Uca_SytemId = 'GENERAL'
                                                                          AND Uca_Status = 'A')
                                                OR Arm_CostcenterCode = 'ALL')", Session["userLogged"].ToString());
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Arm_RouteId LIKE '{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker1) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Checker2) LIKE '%{0}%' )
                                          OR ( dbo.GetControlEmployeeNameV2(Arm_Approver) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(Arm_CostcenterCode) LIKE '%{0}%' )
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

    private void setDetailInformation()
    {
        DataSet ds = new DataSet();
        #region Query
        string sql = @" SELECT Arm_RouteId [Route ID]
                             , Arm_Checker1 [Checker 1 ID]
                             , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1 Name]
                             , Arm_Checker2 [Checker 2 ID]
                             , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2 Name]
                             , Arm_Approver [Approver ID]
                             , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver Name]
                             , Arm_CostcenterCode [Cost Center Code]
                             , CASE WHEN Arm_CostcenterCode <> 'ALL'
                                    THEN dbo.getCostCenterFullNameV2(Arm_CostcenterCode)
                                    ELSE 'AVAILABLE TO ALL COSTCENTERS'
                                END [Cost Center Desc]
                             , Arm_Status [Status]
                          FROM T_ApprovalRouteMaster
                         WHERE Arm_RouteId = @RouteId
                        ";
        #endregion
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@RouteId", txtRouteId.Text);
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
            txtChecker1Id.Text = ds.Tables[0].Rows[0]["Checker 1 ID"].ToString();
            txtChecker1Name.Text = ds.Tables[0].Rows[0]["Checker 1 Name"].ToString();
            txtChecker2Id.Text = ds.Tables[0].Rows[0]["Checker 2 ID"].ToString();
            txtChecker2Name.Text = ds.Tables[0].Rows[0]["Checker 2 Name"].ToString();
            txtApproverId.Text = ds.Tables[0].Rows[0]["Approver ID"].ToString();
            txtApproverName.Text = ds.Tables[0].Rows[0]["Approver Name"].ToString();
            txtCostCenterCode.Text = ds.Tables[0].Rows[0]["Cost Center Code"].ToString();
            txtCostCenterDesc.Text = ds.Tables[0].Rows[0]["Cost Center Desc"].ToString();

            if (ds.Tables[0].Rows[0]["Status"].ToString().Equals("A"))
            {
                ddlStatus.SelectedIndex = 0;
            }
            else
            {
                ddlStatus.SelectedIndex = 1;
            }
        }
    }

    string NewRouteID = string.Empty;
    private DataRow PopulateDr()
    {
        DataRow dr = DbRecord.Generate("T_ApprovalRouteMaster");

        dr["Arm_RouteId"] = txtRouteId.Text;
        
            if (txtRouteId.Text.ToString().Trim() == "")
                dr["Arm_RouteId"] = NewRouteID = GNBL.GetRouteControlNumber();
        
        dr["Arm_Checker1"] = txtChecker1Id.Text;
        dr["Arm_Checker2"] = txtChecker2Id.Text;
        dr["Arm_Approver"] = txtApproverId.Text;
        dr["Arm_CostcenterCode"] = txtCostCenterCode.Text;
        dr["Arm_Status"] = ddlStatus.SelectedValue.ToString();
        dr["Usr_login"] = Session["userLogged"].ToString();
        return dr;
    }
    #endregion
    
}
