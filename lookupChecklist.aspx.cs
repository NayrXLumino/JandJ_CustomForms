/* Revision no. W2.1.00001 
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   04/17/2013
 *  Update Notes    :   
 *      -   endrosed date and effectivity date swap
 */
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
using System.Collections.Generic;
using System.Text;

public partial class pgeLookupChecklist : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    DataTable dtErrors = new DataTable();
    enum WFProcess { Endorse, Approval, Disapprove, Return };

    private bool hasCCLine
    {
        get { return this.ViewState["hasCCLine"] == null ? false : Boolean.Parse(this.ViewState["hasCCLine"].ToString()); }
        set { this.ViewState["hasCCLine"]=value;}
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Response.Cache.SetAllowResponseInBrowserHistory(true);
        hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                Session["errorLogged"] = string.Empty;
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
                hfPageIndex.Value = "0";
                hfRowCount.Value = "0";
                fillCostcenterDropdown();
                bindGrid();
                UpdatePagerLocation();
                Session["transaction"] = "CHECKLIST";
            }
            LoadComplete += new EventHandler(pgeLookupChecklist_LoadComplete);
        }
    }

    void pgeLookupChecklist_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "checklistScripts";
        string jsurl = "Javascript/_collapseDivChecklist.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
        btnEndorseChecker2.Attributes.Add("OnClick", "javascript:setReadOnly('btnEndorseChecker2')");
        btnEndorseApprover.Attributes.Add("OnClick", "javascript:setReadOnly('btnEndorseApprover')");
        btnApprove.Attributes.Add("OnClick", "javascript:setReadOnly('btnApprove')");
        btnDisapprove.Attributes.Add("OnClick", "javascript:setReadOnly('btnDisapprove')");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        btnLoad.Attributes.Add("OnClick", "javascript:return loadTransaction();");
        try
        {
            CheckBox cbxTemp = (CheckBox)dgvResult.HeaderRow.FindControl("chkBoxAll");
            cbxTemp.Attributes.Add("onClick", "javascript:SelectAll();");
        }
        catch 
        { 
            //no implementation
        }
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }
    #region Methods
    private void InsertInfoForNotification(EmailNotificationBL EMBL, string controlNumbers)
    {
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                EMBL.InsertInfoForNotification(controlNumbers
                                                , Session["userLogged"].ToString()
                                                , dal);
                
                dal.CommitTransactionSnapshot();
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                dal.RollBackTransactionSnapshot();
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }

    private void fillCostcenterDropdown()
    {
        DataSet ds = new DataSet();
        #region SQL
        string sql = @" SELECT DISTINCT LEFT({1}_CostCenter, {4}) [CODE]
                             , (dbo.getCostCenterFullNameV2(LEFT({1}_CostCenter, {4}))) [DESC]
                          FROM {0}
                          LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                            ON empApprovalRoute.Arm_EmployeeId = {1}_EmployeeId
                           AND empApprovalRoute.Arm_TransactionId = '{2}'
                         INNER JOIN T_ApprovalRouteMaster AS routeMaster
                            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                         WHERE (({1}_Status = '3' AND routeMaster.Arm_Checker1 = '{3}')
                            OR ({1}_Status = '5' AND routeMaster.Arm_Checker2 = '{3}')
                            OR ({1}_Status = '7' AND routeMaster.Arm_Approver = '{3}'))                        
                           AND Convert(varchar,GETDATE(),101) BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, GETDATE())";

        string tableName = string.Empty;
        string prefix = string.Empty;
        string transactionId = string.Empty;
        string trimValue = rblCCOption.SelectedValue.Equals("S") ? "6" : "4";

        switch (Request.QueryString["type"].ToString().ToUpper().Trim())
        { 
            case "OT":
                tableName = "T_EmployeeOvertime";
                prefix = "Eot";
                transactionId = "OVERTIME";
                break;
            case "LV":
                tableName = "T_EmployeeLeaveAvailment";
                prefix = "Elt";
                transactionId = "LEAVE";
                break;
            case "TR":
                tableName = "T_TimeRecMod";
                prefix = "Trm";
                transactionId = "TIMEMOD";
                break;
            case "FT":
                tableName = "T_FlexTime";
                prefix = "Flx";
                transactionId = "FLEXTIME";
                break;
            case "JS":
                tableName = "T_JobSplitHeader";
                prefix = "Jsh";
                transactionId = "JOBMOD";
                break;
            case "MV":
                tableName = "T_Movement";
                prefix = "Mve";
                transactionId = "MOVEMENT";
                break;
            case "TX":
                tableName = "T_PersonnelInfoMovement";
                prefix = "Pit";
                transactionId = "TAXMVMNT";
                break;
            case "BF":
                tableName = "T_BeneficiaryUpdate";
                prefix = "But";
                transactionId = "BNEFICIARY ";
                break;
            case "AD":
                tableName = "T_AddressMovement";
                prefix = "Amt";
                transactionId = "ADDRESS";
                break;
            case "GP":
                tableName = "E_EmployeeGatePass";
                prefix = "Egp";
                transactionId = "GATEPASS";
                break;
            default:
                break;
        }
        

        #endregion
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, tableName
                                                         , prefix
                                                         , transactionId
                                                         , (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString()
                                                         , trimValue), CommandType.Text);
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
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        { 
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ddlCostCenter.Items.Add(new ListItem( ds.Tables[0].Rows[i]["DESC"].ToString()
                                                    , ds.Tables[0].Rows[i]["CODE"].ToString()));
            }
        }
    }


    protected string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;
                    
                        WITH TempTable AS (
                      SELECT Row_Number() OVER (Order by [Control No]) [Row], *
                        FROM ( ");
        sql.Append(getQuery());
        sql.Append(@")   AS temp)");
        sql.Append(@"SELECT * FROM TempTable
                      WHERE Row between @startIndex AND @startIndex + @numRow - 1");
        sql.Append(" SELECT COUNT(*) FROM (");
        sql.Append(getQuery());
        sql.Append(") AS ROWS");//Just find a common column to count


        return sql.ToString();
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
        if (!CommonMethods.isEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[1].Rows)
                hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));

            #region Remove Column
            ds.Tables[0].Columns.Remove("Row");
            //Database parameter driven
            if (rblCCOption.SelectedValue.ToString().Equals("S"))
            {
                ds.Tables[0].Columns.Remove("Department");
            }
            else
            {
                ds.Tables[0].Columns.Remove("Section");
            }
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
            switch (Request.QueryString["type"].ToString().ToUpper())
            {
                case "OT":
                    #region OT
                    //DASH Specific
                    ds.Tables[0].Columns.Remove("DASH Work Code");
                    ds.Tables[0].Columns.Remove("Client Job Name");
                    ds.Tables[0].Columns.Remove("DASH Job Code");
                    ds.Tables[0].Columns.Remove("Client Job No");
                    ds.Tables[0].Columns.Remove("DASH Class Code");
                    //Depending if Used
                    if (CommonMethods.getFillerStatus("Eot_Filler01") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                    if (CommonMethods.getFillerStatus("Eot_Filler02") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                    if (CommonMethods.getFillerStatus("Eot_Filler03") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));
                    #endregion
                    break;
                case "LV":
                    #region LV
                    if (CommonMethods.getFillerStatus("Elt_Filler01") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler01"));
                    if (CommonMethods.getFillerStatus("Elt_Filler02") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler02"));
                    if (CommonMethods.getFillerStatus("Elt_Filler03") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler03"));
                    #endregion
                    break;
                case "SW":
                    #region SW
                    if (CommonMethods.getFillerStatus("Swt_Filler01") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler01"));
                    if (CommonMethods.getFillerStatus("Swt_Filler02") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler02"));
                    if (CommonMethods.getFillerStatus("Swt_Filler03") == "C")
                        ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler03"));
                    #endregion
                    break;
            }

            //dgvResult.DataSource = ds;
            //dgvResult.DataBind();
        }
            
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    private string getQuery()
    {
        string query = string.Empty;
        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                query = getOvertimeData();
                break;
            case "LV":
                query = getLeaveData();
                break;
            case "TR":
                query = getTimeModificationData();
                break;
            case "FT":
                query = getFlexTimeData();
                break;
            case "JS":
                query = getJobModificationData();
                break;
            case "MV":
                query = getMovementData();
                break;
            case "TX":
                query = getTaxCivilData();
                break;
            case "BF":
                query = getBeneficiaryData();
                break;
            case "AD":
                query = getAddressData();
                break;
            case "SW":
                query = getStraightWorkData();
                break;
            case "GP":
                query = getGatePassData();
                break;
            default:
                break;
        }
        query = query.Replace("@UserLogged", string.Format("'{0}'", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString()));
        query = query.Replace("@filterCostCenter", string.Format("'{0}'", ddlCostCenter.SelectedValue.ToString()));
        return query;
    }

    private void CheckAll()
    {
        int ctrZ = 0;
        if (dgvResult.Rows.Count > 0)
        {
            CheckBox CBA = (CheckBox)dgvResult.HeaderRow.Cells[0].FindControl("chkBoxAll");
            if (CBA.Checked)
            {
                for (ctrZ = 0; ctrZ < dgvResult.Rows.Count; ctrZ++)
                {
                    CheckBox CB = (CheckBox)dgvResult.Rows[ctrZ].Cells[0].FindControl("chkBox");
                    CB.Checked = true;
                }
            }
            else
            {
                for (ctrZ = 0; ctrZ < dgvResult.Rows.Count; ctrZ++)
                {
                    CheckBox CB = (CheckBox)dgvResult.Rows[ctrZ].Cells[0].FindControl("chkBox");
                    CB.Checked = false;
                }
            }
        }
    }

    private void buttonControl()
    {
        int check = 0;
        int E1 = 0;
        int E2 = 0;
        int Approver = 0;

        for (check = 0; check < dgvResult.Rows.Count; check++)
        {
            CheckBox CB = (CheckBox)dgvResult.Rows[check].Cells[0].FindControl("chkBox");
            bool xx = CB.Checked;

            if (xx)
            {
                if (dgvResult.Rows[check].Cells[3].Text.ToUpper() == "ENDORSED TO CHECKER 1")
                {
                    E1++;
                }

                if (dgvResult.Rows[check].Cells[3].Text.ToUpper() == "ENDORSED TO CHECKER 2")
                {
                    E2++;
                }

                if (dgvResult.Rows[check].Cells[3].Text.ToUpper() == "ENDORSED TO APPROVER")
                {
                    Approver++;
                }

            }
        }

        btnEndorseChecker2.Enabled = (E1 > 0);
        btnEndorseApprover.Enabled = (E2 > 0);
        btnApprove.Enabled = (Approver > 0);
        if (btnEndorseChecker2.Enabled || btnEndorseApprover.Enabled || btnApprove.Enabled)
        {
            btnDisapprove.Enabled = true;
        }
    }


    private void assignButtonLoad()
    {
        //string controlNo = string.Empty;
        //controlNo = dgvResult.SelectedRow.Cells[1].Text;
        //string url = string.Empty;
        //switch (controlNo.Substring(0, 1).ToUpper())
        //{ 
        //    case "V"://OVERTIME
        //        url = "window.opener.location.href='Transactions/Overtime/pgeOvertimeIndividual.aspx?cn={0}';";
        //        break;
        //    case "L"://LEAVE
        //        url = "window.opener.location.href='Transactions/Leave/pgeLeaveIndividual.aspx?cn={0}';";
        //        break;
        //    case "T"://TIME MODIFICATION
        //        url = "window.opener.location.href='Transactions/TimeModification/pgeTimeModification.aspx?cn={0}';";
        //        break;
        //    case "I"://TAX CODE / CIVIL STATUS
        //        url = "window.opener.location.href='Transactions/Personnel/pgeTaxCodeCivilStatus.aspx?cn={0}';";
        //        break;
        //    case "E"://BENEFICIARY
        //        url = "window.opener.location.href='Transactions/Personnel/pgeBeneficiaryUpdate.aspx?cn={0}';";
        //        break;
        //    case "P"://ADDRESS MOVEMENT
        //        if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("PRESENT"))
        //        {
        //            url = "window.opener.location.href='Transactions/Personnel/pgeAddressPresent.aspx?cn={0}';";
        //        }
        //        else if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("PERMANENT"))
        //        {
        //            url = "window.opener.location.href='Transactions/Personnel/pgeAddressPermanent.aspx?cn={0}';";
        //        }
        //        else if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("EMERGENCY CONTACT"))
        //        {
        //            url = "window.opener.location.href='Transactions/Personnel/pgeAddressEmergency.aspx?cn={0}';";
        //        }
        //        break;
        //    case "M": // MOVEMENT
        //        if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("GROUP"))
        //        {
        //            url = "window.opener.location.href='Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx?cn={0}';";
        //        }
        //        else if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("SHIFT"))
        //        {
        //            url = "window.opener.location.href='Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx?cn={0}';";
        //        }
        //        else if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("COSTCENTER"))
        //        {
        //            url = "window.opener.location.href='Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx?cn={0}';";
        //        }
        //        else if (dgvResult.SelectedRow.Cells[8].Text.ToUpper().Equals("RESTDAY"))
        //        {
        //            url = "window.opener.location.href='Transactions/WorkInformation/pgeRestdayIndividualUpdate.aspx?cn={0}';";
        //        }
        //        break;
        //    // Jobsplit whose control no starts with J are not currently loadable in this lookup
        //    //case "J":
        //    //url = "window.opener.location.href='Transactions/ManhourAllocation/pgeWorkRecord.aspx?cn={0}';";
        //    //break;
        //    case "S":
        //        url = "window.opener.location.href='Transactions/ManhourAllocation/pgeJobSplitMod.aspx?cn={0}';";
        //        break;
        //    default:
        //        break;
        //}
        //Session["transaction"] = "CHECKLIST";
        //btnLoad.OnClientClick = string.Format(url, controlNo);
    }

    private bool isCutOff(string transaction, string controlNo, DALHelper dal)
    {
        bool isCutOff = false;
        string sql = string.Empty;
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@ControlNo", controlNo);
        #region SQL
        switch (transaction)
        {
            case "OVERTIME":
                sql = @"SELECT CASE WHEN ( (  Eot_OvertimeDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_EmployeeOvertime
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'OVERTIME'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Eot_ControlNo = @ControlNo";
                break;
            case "LEAVE":
                sql = @"SELECT CASE WHEN ( (  Elt_LeaveDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_EmployeeLeaveAvailment
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'LEAVE'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Elt_ControlNo = @ControlNo";
                break;
            case "TIME MODIFICATION":
                sql = @"SELECT CASE WHEN ( (  Trm_ModDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_TimeRecMod
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Trm_ControlNo = @ControlNo";
                break;
            case "FLEXTIME":
                sql = @"SELECT CASE WHEN ( (  Flx_FlexDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_FlexTime
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Flx_ControlNo = @ControlNo";
                break;
            case "JOBSPLIT MODIFICATION":
                sql = @"SELECT CASE WHEN ( (  Jsh_JobSplitDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_JobSplitHeader
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Jsh_ControlNo = @ControlNo";
                break;
            case "WORK INFO MOVEMENT":
                sql = @"SELECT CASE WHEN ( (  Mve_EffectivityDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_Movement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Mve_ControlNo = @ControlNo";
                break;
            case "TAX CODE / CIVIL STATUS":
                sql = @"SELECT CASE WHEN ( (  Pit_EffectivityDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_PersonnelInfoMovement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Pit_ControlNo = @ControlNo";
                break;
            case "BENEFICIARY UPDATE":
                sql = @"SELECT CASE WHEN ( (  But_EffectivityDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_BeneficiaryUpdate
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE But_ControlNo = @ControlNo";
                break;
            case "ADDRESS MOVEMENT":
                sql = @"SELECT CASE WHEN ( (  Amt_EffectivityDate <= Ppm_EndCycle)
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_AddressMovement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Amt_ControlNo = @ControlNo";
                break;
			case "STRAIGHT WORK":
	        sql = @"SELECT CASE WHEN ( (   Swt_FromDate >= Ppm_StartCycle AND Swt_ToDate <= Ppm_EndCycle)
	                                   AND Pcm_ProcessFlag = 1)
	                            THEN 'TRUE'
	                            ELSE 'FALSE'
	                        END
	                  FROM T_EmployeeStraightWork
	                 INNER JOIN T_PayPeriodMaster
	                    ON Ppm_CycleIndicator = 'C'
	                 INNER JOIN T_ProcessControlMaster
	                    ON Pcm_SystemId = 'TIMEKEEP'
	                   AND Pcm_ProcessId = 'CUT-OFF'
	                 WHERE Swt_ControlNo = @ControlNo";
	        break;
            default:
                break;
        }

        #region CHIYODA SPECIFIC
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            switch (transaction)
            {
                case "OVERTIME":
                    sql = @"SELECT CASE WHEN ( Eot_OvertimeDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_EmployeeOvertime
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'OVERTIME'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Eot_ControlNo = @ControlNo";
                    break;
                case "LEAVE":
                    sql = @"SELECT CASE WHEN ( Elt_LeaveDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_EmployeeLeaveAvailment
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'LEAVE'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Elt_ControlNo = @ControlNo";
                    break;
                case "TIME MODIFICATION":
                    sql = @"SELECT CASE WHEN ( Trm_ModDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_TimeRecMod
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Trm_ControlNo = @ControlNo";
                    break;
                case "FLEXTIME":
                    sql = @"SELECT CASE WHEN ( Flx_FlexDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_FlexTime
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Flx_ControlNo = @ControlNo";
                    break;
                case "JOBSPLIT MODIFICATION":
                    sql = @"SELECT CASE WHEN ( Jsh_JobSplitDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_JobSplitHeader
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Jsh_ControlNo = @ControlNo";
                    break;
                case "WORK INFO MOVEMENT":
                    sql = @"SELECT CASE WHEN ( Mve_EffectivityDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_Movement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'TIMEKEEP'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Mve_ControlNo = @ControlNo";
                    break;
                case "TAX CODE / CIVIL STATUS":
                    sql = @"SELECT CASE WHEN ( Pit_EffectivityDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_PersonnelInfoMovement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Pit_ControlNo = @ControlNo";
                    break;
                case "BENEFICIARY UPDATE":
                    sql = @"SELECT CASE WHEN ( But_EffectivityDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_BeneficiaryUpdate
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE But_ControlNo = @ControlNo";
                    break;
                case "ADDRESS MOVEMENT":
                    sql = @"SELECT CASE WHEN ( Amt_EffectivityDate < Ppm_StartCycle
                                           AND Pcm_ProcessFlag = 1)
                                    THEN 'TRUE'
                                    ELSE 'FALSE'
                                END
                          FROM T_AddressMovement
                         INNER JOIN T_PayPeriodMaster
                            ON Ppm_CycleIndicator = 'C'
                         INNER JOIN T_ProcessControlMaster
                            ON Pcm_SystemId = 'PAYROLL'
                           AND Pcm_ProcessId = 'CUT-OFF'
                         WHERE Amt_ControlNo = @ControlNo";
                    break;
                default:
                    break;
            }
        }
        #endregion
        #endregion

        if ( !controlNo.Equals(string.Empty) 
          //&& controlNo.Trim().Length.Equals(12) 
          && !sql.Equals(string.Empty))
        {
            try
            {
                isCutOff = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text, param));
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                isCutOff = true;
            }

            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                if (transaction.Equals("JOBSPLIT MODIFICATION"))
                {
                    isCutOff = false;
                }
            }
        }
        else
        {
            isCutOff = true;
            CommonMethods.ErrorsToTextFile(new Exception("Invalid control number for checking in checklist isCutoff(). " + controlNo.ToString()), Session["userLogged"].ToString());
        }
        

        return isCutOff;
    }
    #region Methods for fetching data returns DataSet
    private string getOvertimeData()
    {
        #region SQL
        string sqlCounters = string.Empty;

        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Eot_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql =string.Format(@"  SELECT Eot_ControlNo [Control No]
                              , Eot_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Eot_OvertimeDate,101) [OT Date]
                              , CONVERT(varchar(10),Eot_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Eot_AppliedDate,114),5)[Applied Date/Time]
                              , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                              , LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) [Start Time]
                              , LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) [End Time]
                              , Eot_OvertimeHour [Hours]
                              , Eot_Reason [Reason]
                              , dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 4)) [Department]
                              , dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) [Section]
                              {0}
                              , Slm_DashWorkCode [DASH Work Code]
                              , Slm_ClientJobName [Client Job Name]
                              , Eot_JobCode [DASH Job Code]
                              , Eot_ClientJobNo [Client Job No]
                              , Slm_DashClassCode [DASH Class Code]
                              , CONVERT(varchar(10),Eot_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Eot_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Eot_Status
	                                 WHEN '3' THEN dbo.GetControlEmployeeName(Eot_EmployeeId)
			                         WHEN '5' THEN dbo.GetControlEmployeeNameV2(Eot_CheckedBy)
			                         WHEN '7' THEN dbo.GetControlEmployeeNameV2(Eot_Checked2By)
			                         ELSE ''
                                 END [Last Updated By]
                              , CASE Eot_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Eot_EndorsedDateToChecker,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Eot_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Eot_CheckedDate,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Eot_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Eot_Checked2Date,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Eot_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                              , AD2.Adt_AccountDesc [@Eot_Filler1Desc]
                              , AD3.Adt_AccountDesc [@Eot_Filler2Desc]
                              , AD4.Adt_AccountDesc [@Eot_Filler3Desc]
                              , Eot_BatchNo [Batch No]
                          FROM T_EmployeeOvertime
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Eot_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Eot_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Eot_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Eot_ApprovedBy
                           LEFT JOIN T_SalesMaster 
                             ON Slm_DashJobCode = Eot_JobCode 
                            AND Slm_ClientJobNo = Eot_ClientJobNo
                           LEFT JOIN T_AccountDetail AD2
                             ON AD2.Adt_AccountCode = Eot_Filler1
                            AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler01')
                           LEFT JOIN T_AccountDetail AD3
                             ON AD3.Adt_AccountCode = Eot_Filler2
                            AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler02')
                           LEFT JOIN T_AccountDetail AD4
                             ON AD4.Adt_AccountCode = Eot_Filler3
                            AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler03')

                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Eot_Status
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
                            AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                           LEFT JOIN T_ParameterMasterExt
                             ON Pmx_ParameterValue = Eot_OvertimeType
                            AND Pmx_ParameterID = 'OTTYPE'
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Eot_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Eot_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Eot_Status = '7' AND routeMaster.Arm_Approver = @UserLogged)) 
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", Eot_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Eot_ControlNo LIKE '{0}%' )
                                      OR ( Eot_EmployeeId LIKE '{0}%' )
                                      OR ( ADT1.Adt_AccountDesc LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_Lastname LIKE '{0}%' )
                                      OR ( Emt_Firstname LIKE '{0}%' )
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 4)) LIKE '%{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Eot_OvertimeDate,101) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Eot_AppliedDate,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Eot_AppliedDate,114),5) LIKE '%{0}%' )
                                      OR ( ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') LIKE '{0}%' )
                                      OR ( LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) LIKE '{0}%' )
                                      OR ( LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) LIKE '{0}%' )
                                      OR ( Eot_OvertimeHour LIKE '{0}%' )
                                      OR ( Eot_Reason LIKE '{0}%' )
                                      OR ( Slm_DashWorkCode LIKE '{0}%' )
                                      OR ( Slm_ClientJobName LIKE '{0}%' )
                                      OR ( Eot_JobCode LIKE '{0}%' )
                                      OR ( Eot_ClientJobNo LIKE '{0}%' )
                                      OR ( Slm_DashClassCode LIKE '{0}%' )
                                      OR ( CONVERT(varchar(10),Eot_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Eot_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                      OR ( CASE Eot_Status
	                                         WHEN '3' THEN dbo.GetControlEmployeeName(Eot_EmployeeId)
			                                 WHEN '5' THEN dbo.GetControlEmployeeNameV2(Eot_CheckedBy)
			                                 WHEN '7' THEN dbo.GetControlEmployeeNameV2(Eot_Checked2By)
			                                 ELSE ''
                                         END  LIKE '{0}%' )
                                      OR ( CASE Eot_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Eot_EndorsedDateToChecker,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Eot_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Eot_CheckedDate,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Eot_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Eot_Checked2Date,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Eot_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '%{0}%' )
                                      OR ( Eot_Filler1 LIKE '{0}%' )
                                      OR ( Eot_Filler2 LIKE '{0}%' )
                                      OR ( Eot_Filler3 LIKE '{0}%' )
                                      OR ( Eot_BatchNo LIKE '{0}%' )
                                        {1}
                                    )";
       
        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Eot_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Eot_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Eot_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
 
        sql = sql.Replace("@Eot_Filler1Desc", CommonMethods.getFillerName("Eot_Filler01"));
        sql = sql.Replace("@Eot_Filler2Desc", CommonMethods.getFillerName("Eot_Filler02"));
        sql = sql.Replace("@Eot_Filler3Desc", CommonMethods.getFillerName("Eot_Filler03"));
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }

    private string getLeaveData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim()!="")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Elt_ControlNo in ({0})", ControNumbers);
            }
        }

        string sql =string.Format(@"  SELECT  Elt_ControlNo [Control No]
                              , Elt_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Elt_LeaveType [Leave Code]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Elt_LeaveDate,101) [Leave Date]
                              , CONVERT(varchar(10),Elt_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Elt_AppliedDate,114),5)[Applied Date/Time]
                              , Ltm_LeaveDesc [Leave Type]
                              , LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) [Start Time]
                              , LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) [End Time]
                              , Elt_LeaveHour [Hours]
                              , AD2.Adt_AccountDesc [@Elt_Filler1Desc]
                              , AD3.Adt_AccountDesc [@Elt_Filler2Desc]
                              , AD4.Adt_AccountDesc [@Elt_Filler3Desc]
                              , Elt_DayUnit [Day Unit]
                              , Elt_Reason [Reason]
                              , dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 4)) [Department]
                              , dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) [Section]
                                {0}
                              , ISNULL(ADT2.Adt_AccountDesc, '- not applicable -') [Category]
                              
                              , CONVERT(varchar(10),Elt_InformDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Elt_InformDate,114),5) [Inform Date]
                              , CONVERT(varchar(10),Elt_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Elt_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Elt_Status
		                             WHEN '3' THEN dbo.GetControlEmployeeName(Elt_EmployeeId)
		                             WHEN '5' THEN dbo.GetControlEmployeeNameV2(Elt_CheckedBy)
		                             WHEN '7' THEN dbo.GetControlEmployeeNameV2(Elt_Checked2By)
		                             ELSE ''
	                             END [Last Updated By]
                              , CASE Elt_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Elt_EndorsedDateToChecker,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Elt_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Elt_CheckedDate,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Elt_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Elt_Checked2Date,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Elt_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                           FROM T_EmployeeLeaveAvailment
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Elt_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Elt_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Elt_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Elt_ApprovedBy
                            LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Elt_Status
                           LEFT JOIN T_LeaveTypeMaster
                             ON Ltm_LeaveType = Elt_LeaveType
                           LEFT JOIN T_AccountDetail ADT2
                             ON ADT2.Adt_AccountType = 'LVECATEGRY'
                            AND ADT2.Adt_AccountCode = Elt_LeaveCategory
                           LEFT JOIN T_AccountDetail AD2
                             ON AD2.Adt_AccountCode = Elt_Filler1
                            AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler01')
                           LEFT JOIN T_AccountDetail AD3
                             ON AD3.Adt_AccountCode = Elt_Filler2
                            AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler02')
                           LEFT JOIN T_AccountDetail AD4
                             ON AD4.Adt_AccountCode = Elt_Filler3
                            AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler03')

                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
                            AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Elt_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Elt_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Elt_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", Elt_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Elt_ControlNo LIKE '{0}%' )
                                      OR ( Elt_EmployeeId LIKE '{0}%' )
                                      OR ( ADT1.Adt_AccountDesc LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_Lastname LIKE '{0}%' )
                                      OR ( Emt_Firstname LIKE '{0}%' )
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 4)) LIKE '%{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Elt_LeaveDate,101) LIKE '%{0}%' )
                                      OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                      OR ( ADT2.Adt_AccountDesc LIKE '{0}%' )
                                      OR ( CONVERT(varchar(10),Elt_AppliedDate,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Elt_AppliedDate,114),5) LIKE '%{0}%' )
                                      OR ( LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) LIKE '{0}%' )
                                      OR ( LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) LIKE '{0}%' )
                                      OR ( Elt_LeaveHour LIKE '{0}%' )
                                      OR ( Elt_DayUnit LIKE '{0}%' )
                                      OR ( Elt_Reason LIKE '{0}%' )
                                      OR ( CONVERT(varchar(10),Elt_InformDate,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Elt_InformDate,114),5) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Elt_EndorsedDateToChecker,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Elt_EndorsedDateToChecker,114),5) LIKE '%{0}%' )
                                      OR ( CASE Elt_Status
		                                        WHEN '3' THEN dbo.GetControlEmployeeName(Elt_EmployeeId)
		                                        WHEN '5' THEN dbo.GetControlEmployeeNameV2(Elt_CheckedBy)
		                                        WHEN '7' THEN dbo.GetControlEmployeeNameV2(Elt_Checked2By)
		                                        ELSE ''
	                                        END LIKE '{0}%' )
                                      OR ( CASE Elt_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Elt_EndorsedDateToChecker,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Elt_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Elt_CheckedDate,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Elt_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Elt_Checked2Date,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Elt_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '%{0}%' )
                                       OR ( Elt_Leavecode LIKE '{0}%' )
                                       OR ( Elt_Filler1 LIKE '{0}%' )
                                       OR ( Elt_Filler2 LIKE '{0}%' )
                                       OR ( Elt_Filler3 LIKE '{0}%' )
                                        {1}
                                     )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter,holder, !hasCCLine ? "" : string.Format(@" OR ( Elt_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Elt_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Elt_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";

        sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
        sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
        sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }

    private string getTimeModificationData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Trm_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql =string.Format(@"  SELECT Trm_ControlNo [Control No]
                              , Trm_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Trm_ModDate,101) [Modification Date]
                              , CONVERT(varchar(10),Trm_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Trm_AppliedDate,114),5)[Applied Date/Time]
                              , ADT2.Adt_AccountDesc [Type]
                              , CASE WHEN (Trm_ActualTimeIn1 = '')
                                     THEN ''
                                     ELSE LEFT(Trm_ActualTimeIn1,2) + ':' + RIGHT(Trm_ActualTimeIn1,2)
                                 END [Time IN1]
                              , CASE WHEN (Trm_ActualTimeOut1 = '')
                                     THEN ''
                                     ELSE LEFT(Trm_ActualTimeOut1,2) + ':' + RIGHT(Trm_ActualTimeOut1,2)
                                 END [Time OUT1]
                              , CASE WHEN (Trm_ActualTimeIn2 = '')
                                     THEN ''
                                     ELSE LEFT(Trm_ActualTimeIn2,2) + ':' + RIGHT(Trm_ActualTimeIn2,2)
                                 END [Time IN2]
                              , CASE WHEN (Trm_ActualTimeOut2 = '')
                                     THEN ''
                                     ELSE LEFT(Trm_ActualTimeOut2,2) + ':' + RIGHT(Trm_ActualTimeOut2,2)
                                 END [Time OUT2]
                              , Trm_Reason [Reason]
                              , CASE WHEN ISNULL(E1.Ell_ShiftCode, '') <> ''
		                             THEN '['+E1.Ell_ShiftCode+'] '
			                            + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
			                            + '-'
			                            + LEFT(S1.Scm_ShiftBreakStart,2) + ':' + RIGHT(S1.Scm_ShiftBreakStart,2)
			                            + '  ' 
			                            + LEFT(S1.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S1.Scm_ShiftBreakEnd,2)
			                            + '-'
			                            + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2) 
		                             ELSE '['+E2.Ell_ShiftCode+'] '
			                            + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
			                            + '-'
			                            + LEFT(S2.Scm_ShiftBreakStart,2) + ':' + RIGHT(S2.Scm_ShiftBreakStart,2)
			                            + '  ' 
			                            + LEFT(S2.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S2.Scm_ShiftBreakEnd,2)
			                            + '-'
			                            + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2) 
	                             END [Shift for the Day]
                              , dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 4)) [Department]
                              , dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 6)) [Section]
                              {0}
                              , CONVERT(varchar(10),Trm_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Trm_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                             , CASE Trm_Status
			                         WHEN '3' THEN dbo.GetControlEmployeeName(Trm_EmployeeId)
			                         WHEN '5' THEN dbo.GetControlEmployeeNameV2(Trm_CheckedBy)
			                         WHEN '7' THEN dbo.GetControlEmployeeNameV2(Trm_Checked2By)
			                         ELSE ''
		                         END [Last Updated By]
                              , CASE Trm_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Trm_EndorsedDateToChecker,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Trm_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Trm_CheckedDate,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Trm_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Trm_Checked2Date,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Trm_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                           FROM T_TimeRecMod
                           LEFT JOIN T_EmployeeLogLedger E1
                             ON E1.Ell_ProcessDate = Trm_ModDate
                            AND E1.Ell_EmployeeId = Trm_EmployeeId
                           LEFT JOIN T_EmployeeLogLedgerHist E2
                             ON E2.Ell_ProcessDate = Trm_ModDate
                            AND E2.Ell_EmployeeId = Trm_EmployeeId
                           LEFT JOIN T_ShiftCodeMaster S1
                             ON S1.Scm_ShiftCode = E1.Ell_ShiftCode
                           LEFT JOIN T_ShiftCodeMaster S2
                             ON S2.Scm_ShiftCode = E2.Ell_ShiftCode
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Trm_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Trm_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Trm_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Trm_ApprovedBy
                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Trm_Status
                           LEFT JOIN T_AccountDetail ADT2
                             ON ADT2.Adt_AccountType = 'TMERECTYPE'
                            AND ADT2.Adt_AccountCode = Trm_Type
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
                            AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Trm_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Trm_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Trm_Status = '7' AND routeMaster.Arm_Approver = @UserLogged) )
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", Trm_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Trm_ControlNo LIKE '{0}%')
                                      OR ( Trm_EmployeeId LIKE '{0}%')
                                      OR ( ADT1.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_Lastname LIKE '{0}%')
                                      OR ( Emt_Firstname LIKE '{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 4)) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Trm_ModDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Trm_AppliedDate,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Trm_AppliedDate,114),5) LIKE '&{0}%')
                                      OR ( ADT2.Adt_AccountDesc LIKE '{0}%')
                                      OR ( CASE WHEN (Trm_ActualTimeIn1 = '')
                                                THEN ''
                                                ELSE LEFT(Trm_ActualTimeIn1,2) + ':' + RIGHT(Trm_ActualTimeIn1,2)
                                            END LIKE '{0}%')
                                      OR ( CASE WHEN (Trm_ActualTimeOut1 = '')
                                                THEN ''
                                                ELSE LEFT(Trm_ActualTimeOut1,2) + ':' + RIGHT(Trm_ActualTimeOut1,2)
                                            END LIKE '{0}%')
                                      OR ( CASE WHEN (Trm_ActualTimeIn2 = '')
                                                THEN ''
                                                ELSE LEFT(Trm_ActualTimeIn2,2) + ':' + RIGHT(Trm_ActualTimeIn2,2)
                                            END LIKE '{0}%')
                                      OR ( CASE WHEN (Trm_ActualTimeOut2 = '')
                                                THEN ''
                                                ELSE LEFT(Trm_ActualTimeOut2,2) + ':' + RIGHT(Trm_ActualTimeOut2,2)
                                            END LIKE '{0}%')
                                      OR ( Trm_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Trm_EndorsedDateToChecker,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Trm_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE Trm_Status
			                                    WHEN '3' THEN dbo.GetControlEmployeeName(Trm_EmployeeId)
			                                    WHEN '5' THEN dbo.GetControlEmployeeNameV2(Trm_CheckedBy)
			                                    WHEN '7' THEN dbo.GetControlEmployeeNameV2(Trm_Checked2By)
			                                    ELSE ''
		                                    END LIKE '{0}%')
                                      OR ( CASE Trm_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Trm_EndorsedDateToChecker,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Trm_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Trm_CheckedDate,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Trm_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Trm_Checked2Date,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Trm_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '{0}%') 
                                        {1} )
                                        ";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Trm_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Trm_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Trm_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }

    private string getFlexTimeData()
    {
        #region SQL
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = @"  SELECT Flx_ControlNo [Control No]
	                          , Flx_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
	                          , Emt_NickName [ID Code]
	                          , Emt_NickName [Nickname]
	                          , Emt_Lastname [Lastname]
	                          , Emt_Firstname [Firstname]
	                          , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Flx_FlexDate,101) [Flex Date]
                              , CONVERT(varchar(10),Flx_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Flx_AppliedDate,114),5)[Applied Date/Time]
                              , ADT2.Adt_AccountDesc [Type]
                              , Flx_Occurrence [Occurence]
                              , LEFT(Flx_TimeIn,2) + ':' + RIGHT(Flx_TimeIn,2) [Time IN]
                              , Flx_Late [Late(in minutes)]
                              , Flx_Reason [Reason]
                              , '[' + Scm_ShiftCode + '] ' 
                                + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) + '-'
                                + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) + '   '
                                + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) + '-'
                                + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [Shift]
	                          , dbo.getCostCenterFullNameV2(LEFT(Flx_Costcenter, 4)) [Department]
	                          , dbo.getCostCenterFullNameV2(LEFT(Flx_Costcenter, 6)) [Section]
                              
                              , CONVERT(varchar(10),Flx_EndorsedDateToChecker,101) 
		                        + ' ' 
		                        + LEFT(CONVERT(varchar(20),Flx_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Flx_Status
                                     WHEN '3' THEN dbo.GetControlEmployeeName(Flx_EmployeeId)
                                     WHEN '5' THEN dbo.GetControlEmployeeNameV2(Flx_CheckedBy)
                                     WHEN '7' THEN dbo.GetControlEmployeeNameV2(Flx_Checked2By)
                                     ELSE ''
                                 END [Last Updated By]
                              , CASE Flx_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Flx_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Flx_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Flx_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Flx_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Flx_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Flx_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                           FROM T_FlexTime
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Flx_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Flx_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Flx_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Flx_ApprovedBy
                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Flx_Status
                           LEFT JOIN T_AccountDetail ADT2
                             ON ADT2.Adt_AccountType = 'TMEMONITOR'
                            AND ADT2.Adt_AccountCode = Flx_Type
                           LEFT JOIN T_ShiftCodeMaster
                             ON Scm_ShiftCode = Flx_ShiftCode
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'FLEXTIME'
                            AND Flx_FlexDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Trm_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Trm_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Trm_Status = '7' AND routeMaster.Arm_Approver = @UserLogged) )";

        string searchFilter = @"AND (    ( Flx_ControlNo LIKE '{0}%')
	                                  OR ( Flx_EmployeeId LIKE '{0}%')
                                      OR ( ADT1.Adt_AccountDesc LIKE '{0}%')
	                                  OR ( Emt_NickName LIKE '{0}%')
	                                  OR ( Emt_NickName LIKE '{0}%')
	                                  OR ( Emt_Lastname LIKE '{0}%')
	                                  OR ( Emt_Firstname LIKE '{0}%')
	                                  OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
	                                  OR ( dbo.getCostCenterFullNameV2(LEFT(Flx_Costcenter, 4)) LIKE '{0}%')
	                                  OR ( dbo.getCostCenterFullNameV2(LEFT(Flx_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Flx_FlexDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Flx_AppliedDate,101) 
                                           + ' ' 
                                           + LEFT(CONVERT(varchar(20),Flx_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( ADT2.Adt_AccountDesc LIKE '{0}%')
                                      OR ( Flx_Occurrence LIKE '{0}%')
                                      OR ( LEFT(Flx_TimeIn,2) + ':' + RIGHT(Flx_TimeIn,2) LIKE '{0}%')
                                      OR ( '[' + Scm_ShiftCode + '] ' 
                                           + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) + '-'
                                           + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) + '   '
                                           + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) + '-'
                                           + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) LIKE '%{0}%')
                                      OR ( Flx_Late LIKE '{0}%')
                                      OR ( Flx_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Flx_EndorsedDateToChecker,101) 
		                                   + ' ' 
		                                   + LEFT(CONVERT(varchar(20),Flx_EndorsedDateToChecker,114),5) LIKE '${0}%')
                                      OR ( CASE Flx_Status
                                                WHEN '3' THEN dbo.GetControlEmployeeName(Flx_EmployeeId)
                                                WHEN '5' THEN dbo.GetControlEmployeeNameV2(Flx_CheckedBy)
                                                WHEN '7' THEN dbo.GetControlEmployeeNameV2(Flx_Checked2By)
                                                ELSE ''
                                         END  LIKE '{0}%')
                                      OR ( CASE Flx_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Flx_EndorsedDateToChecker,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Flx_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Flx_CheckedDate,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Flx_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Flx_Checked2Date,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Flx_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '%{0}%') )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder);
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Flx_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Trm_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        #endregion

        return sql;
    }

    private string getJobModificationData()
    {
        #region SQL
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = @"  SELECT Jsh_ControlNo [Control No]
                              , Jsh_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , dbo.getCostCenterFullNameV2(LEFT(Jsh_Costcenter, 4)) [Department]
	                          , dbo.getCostCenterFullNameV2(LEFT(Jsh_Costcenter, 6)) [Section]
                              , CONVERT(varchar(10),Jsh_JobSplitDate,101) [Jobsplit Date]
                              , CONVERT(varchar(10),Jsh_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Jsh_AppliedDate,114),5)[Applied Date/Time]
                              , Jsh_Entry [Entry]
                              , CONVERT(varchar(10),Jsh_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Jsh_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Jsh_Status
                                     WHEN '3' THEN dbo.GetControlEmployeeName(Jsh_EmployeeId)
                                     WHEN '5' THEN dbo.GetControlEmployeeNameV2(Jsh_CheckedBy)
                                     WHEN '7' THEN dbo.GetControlEmployeeNameV2(Jsh_Checked2By)
                                     ELSE ''
                                 END [Last Updated By]
                              , CASE Jsh_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Jsh_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Jsh_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Jsh_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Jsh_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Jsh_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Jsh_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                              , Jsh_RefControlNo [Reference]
                           FROM T_JobSplitHeader
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Jsh_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Jsh_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Jsh_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Jsh_ApprovedBy
                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Jsh_Status
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Jsh_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'JOBMOD'
                            AND Jsh_JobSplitDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Jsh_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Jsh_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Jsh_Status = '7' AND routeMaster.Arm_Approver = @UserLogged) )";

        string searchFilter = @"AND (    ( Jsh_ControlNo LIKE '{0}%')
                                      OR ( Jsh_EmployeeId LIKE '{0}%')
                                      OR ( ADT1.Adt_AccountDesc LIKE '{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_Lastname LIKE '{0}%')
                                      OR ( Emt_Firstname LIKE '{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Jsh_Costcenter, 4)) LIKE '{0}%')
	                                  OR ( dbo.getCostCenterFullNameV2(LEFT(Jsh_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Jsh_JobSplitDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Jsh_AppliedDate,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Jsh_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( Jsh_Entry LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Jsh_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Jsh_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE Jsh_Status
                                             WHEN '3' THEN dbo.GetControlEmployeeName(Jsh_EmployeeId)
                                             WHEN '5' THEN dbo.GetControlEmployeeNameV2(Jsh_CheckedBy)
                                             WHEN '7' THEN dbo.GetControlEmployeeNameV2(Jsh_Checked2By)
                                             ELSE ''
                                         END LIKE '{0}%')
                                      OR ( CASE Jsh_Status
                                             WHEN '3' THEN CONVERT(varchar(10),Jsh_EndorsedDateToChecker,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Jsh_EndorsedDateToChecker,114),5)
                                             WHEN '5' THEN CONVERT(varchar(10),Jsh_CheckedDate,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Jsh_CheckedDate,114),5)
                                             WHEN '7' THEN CONVERT(varchar(10),Jsh_Checked2Date,101) 
                                                         + ' ' 
                                                         + LEFT(CONVERT(varchar(20),Jsh_Checked2Date,114),5)
                                             ELSE ''
                                         END LIKE '%{0}%')
                                      OR ( Jsh_RefControlNo LIKE '{0}%') )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder);
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Jsh_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Jsh_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        #endregion

        return sql;
    }

    private string getMovementData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Mve_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = string.Format(@"   SELECT Mve_ControlNo [Control No]
                               , Mve_EmployeeId [ID No]
                               , ADT1.Adt_AccountDesc [Status]
                               , Emt_NickName [ID Code]
                               , Emt_NickName [Nickname]
                               , Emt_Lastname [Lastname]
                               , Emt_Firstname [Firstname]
                               , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                               , CONVERT(varchar(10),Mve_EffectivityDate,101) [Effectivity Date]
                               , ADT2.Adt_Accountdesc [Move Type]
                               , CONVERT(varchar(10),Mve_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Mve_AppliedDate,114),5)[Applied Date/Time]
                               
                               , CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               , CASE WHEN (Mve_Type = 'S')
                                      THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
                                      WHEN (Mve_Type = 'G')
                                      THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
                                      WHEN (Mve_Type = 'C')
                                      THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
                                      ELSE Mve_From
                                  END [From]
                               , CASE WHEN (Mve_Type = 'S')
                                      THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
                                      WHEN (Mve_Type = 'G')
                                      THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
                                      WHEN (Mve_Type = 'C')
                                      THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
                                      ELSE Mve_To
                                  END [To]
                               , Mve_Reason [Reason]
                               , dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) [Section]
                               {0}
                               , CASE Mve_Status
                                      WHEN '3' THEN dbo.GetControlEmployeeName(Mve_EmployeeId)
                                      WHEN '5' THEN dbo.GetControlEmployeeNameV2(Mve_CheckedBy)
                                      WHEN '7' THEN dbo.GetControlEmployeeNameV2(Mve_Checked2By)
                                      ELSE ''
                                  END [Last Updated By]
                              , CASE Mve_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Mve_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Mve_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Mve_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Mve_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                               , Mve_BatchNo [Batch No]
                            FROM T_Movement
                           INNER JOIN T_EmployeeMaster
                              ON Emt_EmployeeId = Mve_EmployeeId
                            LEFT JOIN T_UserMaster UMT1
                              ON UMT1.Umt_UserCode = Mve_CheckedBy
                            LEFT JOIN T_UserMaster UMT2
                              ON UMT2.Umt_UserCode = Mve_Checked2By
                            LEFT JOIN T_UserMaster UMT3
                              ON UMT3.Umt_UserCode = Mve_ApprovedBy
                            LEFT JOIN T_AccountDetail ADT1
                              ON ADT1.Adt_AccountType = 'WFSTATUS'
                             AND ADT1.Adt_AccountCode = Mve_Status
                            LEFT JOIN T_AccountDetail ADT2
                              ON ADT2.Adt_AccountType = 'MOVETYPE'
                             AND ADT2.Adt_AccountCode = Mve_Type
                            ---- JOIN FOR FROM - TO Description
	                        LEFT JOIN T_ShiftCodeMaster S1
	                          ON S1.Scm_ShiftCode = Mve_From
	                         AND Mve_Type = 'S'
	                        LEFT JOIN T_ShiftCodeMaster S2
	                          ON S2.Scm_ShiftCode = Mve_To
	                         AND Mve_Type = 'S'
	                        LEFT JOIN T_AccountDetail A1
	                          ON A1.Adt_AccountCode = LEFT(Mve_From, 3)
	                         AND A1.Adt_AccountType = 'WORKTYPE'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN T_AccountDetail A2
	                          ON A2.Adt_AccountCode = LEFT(Mve_To, 3)
	                         AND A2.Adt_AccountType = 'WORKTYPE'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN T_AccountDetail A3
	                          ON A3.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
	                         AND A3.Adt_AccountType = 'WORKGROUP'
	                         AND Mve_Type = 'G'
	                        LEFT JOIN T_AccountDetail A4
	                          ON A4.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
	                         AND A4.Adt_AccountType = 'WORKGROUP'
	                         AND Mve_Type = 'G'
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'MOVEMENT'
                            AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Mve_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Mve_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Mve_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", Mve_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Mve_ControlNo LIKE '{0}%')
                                      OR ( Mve_EmployeeId LIKE '{0}%')
                                      OR ( ADT1.Adt_AccountDesc LIKE '{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_NickName LIKE '{0}%')
                                      OR ( Emt_Lastname LIKE '{0}%')
                                      OR ( Emt_Firstname LIKE '{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 4)) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Mve_EffectivityDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Mve_AppliedDate,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Mve_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( ADT2.Adt_Accountdesc LIKE '{0}%')
                                      OR ( CASE WHEN (Mve_Type = 'S')
                                              THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
                                              WHEN (Mve_Type = 'G')
                                              THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
                                              WHEN (Mve_Type = 'C')
                                              THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
                                              ELSE Mve_From
                                          END LIKE '{0}%')
                                      OR ( CASE WHEN (Mve_Type = 'S')
                                              THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
                                              WHEN (Mve_Type = 'G')
                                              THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
                                              WHEN (Mve_Type = 'C')
                                              THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
                                              ELSE Mve_To
                                          END LIKE '{0}%')
                                      OR ( Mve_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE Mve_Status
                                              WHEN '3' THEN dbo.GetControlEmployeeName(Mve_EmployeeId)
                                              WHEN '5' THEN dbo.GetControlEmployeeNameV2(Mve_CheckedBy)
                                              WHEN '7' THEN dbo.GetControlEmployeeNameV2(Mve_Checked2By)
                                              ELSE ''
                                          END LIKE '{0}%')
                                      OR ( CASE Mve_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Mve_CheckedDate,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Mve_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Mve_Checked2Date,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Mve_Checked2Date,114),5)
                                                ELSE ''
                                           END LIKE '{0}%')
                                      OR (Mve_BatchNo LIKE '%{0}%')
                                    {1}
                                    )
                                    ";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Mve_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Mve_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Mve_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }

    private string getTaxCivilData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Pit_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = string.Format(@"   SELECT Pit_ControlNo [Control No]
                               , Pit_EmployeeId [ID No]
                               , AD1.Adt_AccountDesc [Status]
                               , Emt_NickName [ID Code]
                               , Emt_NickName [Nickname]
                               , Emt_Lastname [Lastname]
                               , Emt_Firstname [Firstname]
                               , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                               , Convert(varchar(10), Pit_EffectivityDate, 101) [Effectivity Date]
                               , Pit_MoveType [Type]
                               , Pit_From [From Tax Code]
                               , ADFTAX.Adt_AccountDesc [From Tax Desc]
                               , Pit_To [To Tax Code]
                               , ADTTAX.Adt_AccountDesc [To Tax Desc]
                               , Pit_Filler1 [From Civil Code]
                               , ADFCIVIL.Adt_AccountDesc [From Civil Desc]
                               , Pit_Filler2 [To Civil Code]
                               , ADTCIVIL.Adt_AccountDesc [To Civil Desc]
                               , Pit_Reason [Reason]
                               , dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) [Section]
                                {0}
                               , CONVERT(varchar(10),Pit_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Pit_AppliedDate,114),5)[Applied Date/Time]
                               , CONVERT(varchar(10),Pit_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Pit_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               , CASE Pit_Status
                                      WHEN '3' THEN dbo.GetControlEmployeeName(Pit_EmployeeId)
                                      WHEN '5' THEN dbo.GetControlEmployeeNameV2(Pit_CheckedBy)
                                      WHEN '7' THEN dbo.GetControlEmployeeNameV2(Pit_Checked2By)
                                      ELSE ''
                                  END [Last Updated By]
                              , CASE Pit_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Pit_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Pit_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Pit_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Pit_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Pit_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Pit_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                            FROM T_PersonnelInfoMovement
                            INNER JOIN T_EmployeeMaster
                              ON Emt_EmployeeID =  Pit_EmployeeId
                            LEFT JOIN T_UserMaster C1 
                              ON C1.Umt_UserCode = Pit_CheckedBy
                            LEFT JOIN T_UserMaster C2 
                              ON C2.Umt_UserCode = Pit_Checked2By
                            LEFT JOIN T_UserMaster AP 
                              ON AP.Umt_UserCode = Pit_ApprovedBy
                            LEFT JOIN T_AccountDetail AD1 
                              ON AD1.Adt_AccountCode = Pit_Status 
                             AND AD1.Adt_AccountType =  'WFSTATUS'
                            LEFT JOIN T_AccountDetail ADFTAX 
                              ON ADFTAX.Adt_AccountCode = Pit_From 
                             AND ADFTAX.Adt_AccountType =  'TAXCODE'
                            LEFT JOIN T_AccountDetail ADTTAX 
                              ON ADTTAX.Adt_AccountCode = Pit_To 
                             AND ADTTAX.Adt_AccountType =  'TAXCODE'
                            LEFT JOIN T_AccountDetail ADFCIVIL 
                              ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
                             AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
                            LEFT JOIN T_AccountDetail ADTCIVIL 
                              ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
                             AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'
                            LEFT JOIN T_TransactionRemarks 
                              ON Trm_ControlNo = Pit_ControlNo
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Arm_EmployeeId = Pit_EmployeeId
                             AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                             AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                              ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                           WHERE ( (Pit_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Pit_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Pit_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))
                            AND Pit_MoveType = 'P1' 
                            @ADDITIONALCONDITION", !hasCCLine ? "" : ", Pit_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Pit_ControlNo LIKE '{0}%' )
                                      OR ( Pit_EmployeeId LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '%{0}%' )
                                      OR ( Emt_Lastname LIKE '%{0}%' )
                                      OR ( Emt_Firstname LIKE '%{0}%')
                                      OR ( Convert(varchar(10), Pit_EffectivityDate, 101) LIKE '%{0}%')
                                      OR ( Pit_From LIKE '%{0}%')
                                      OR ( ADFTAX.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Pit_To LIKE '%{0}%') 
                                      OR ( ADTTAX.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Pit_Filler1 LIKE '%{0}%')
                                      OR ( ADFCIVIL.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Pit_Filler2 LIKE '%{0}%')
                                      OR ( ADTCIVIL.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 4)) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Pit_EffectivityDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Pit_AppliedDate,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Pit_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( AD1.Adt_Accountdesc LIKE '{0}%')
                                      OR ( Pit_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Pit_EndorsedDateToChecker,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Pit_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE Pit_Status
                                              WHEN '3' THEN dbo.GetControlEmployeeName(Pit_EmployeeId)
                                              WHEN '5' THEN dbo.GetControlEmployeeNameV2(Pit_CheckedBy)
                                              WHEN '7' THEN dbo.GetControlEmployeeNameV2(Pit_Checked2By)
                                              ELSE ''
                                          END LIKE '{0}%')
                                      OR ( CASE Pit_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Pit_EndorsedDateToChecker,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Pit_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Pit_CheckedDate,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Pit_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Pit_Checked2Date,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Pit_Checked2Date,114),5)
                                                ELSE ''
                                           END LIKE '{0}%')
                                   {1} )
                                        ";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Pit_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Pit_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Pit_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }

    private string getBeneficiaryData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND But_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = string.Format(@"   SELECT But_ControlNo [Control No]
                               , But_EmployeeId [ID No]
                               , AD1.Adt_AccountDesc [Status]
                               , Emt_NickName [ID Code]
                               , Emt_NickName [Nickname]
                               , Emt_Lastname [Lastname]
                               , Emt_Firstname [Firstname]
                               , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                               , dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
                               , Convert(varchar(10), But_EffectivityDate, 101) [Effectivity Date]
                               , CONVERT(varchar(10),But_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5)[Applied Date/Time]
                               , CASE WHEN (But_Type = 'N') 
			                          THEN 'NEW ENTRY'
			                          ELSE 'UPDATE EXISTING'
		                          END [Type]
                               , But_SeqNo [Seq No]
                               , But_Lastname [Beneficiary Lastname]
	                           , But_Firstname [Beneficiary Firstname]
	                           , But_Middlename [Beneficiary Middlename]
                               , But_Gender [Gender]
                               , But_CivilStatus [Civil Status]
                               , But_Company [Company]
                               , But_Occupation [Occupation]
	                           , Convert(varchar(10), But_Birthdate, 101) [Birthdate]
	                           , But_Relationship [Relationship Code]
	                           , AD2.Adt_AccountDesc [Relationship Desc]
	                           , But_Hierarchy [Hierarchy Code]
	                           , AD3.Adt_AccountDesc [Hierarchy Desc]
	                           , But_HMODependent [HMO Dependent]
	                           , But_InsuranceDependent [Insurance Dependent]
	                           , But_BIRDependent [BIR Dependent]
	                           , But_AccidentDependent [Accident Dependent]
	                           , Convert(varchar(10), But_DeceasedDate, 101) [Deceased Date]
	                           , Convert(varchar(10), But_CancelDate, 101) [Cancelled Date]
                               , But_Reason [Reason]
                               , CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                 + ' '
                                 + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               , CASE But_Status
                                      WHEN '3' THEN dbo.GetControlEmployeeName(But_EmployeeId)
                                      WHEN '5' THEN dbo.GetControlEmployeeNameV2(But_CheckedBy)
                                      WHEN '7' THEN dbo.GetControlEmployeeNameV2(But_Checked2By)
                                      ELSE ''
                                  END [Last Updated By]
                              , CASE But_Status
                                     WHEN '3' THEN CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),But_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),But_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),But_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),But_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                            FROM T_BeneficiaryUpdate
                            INNER JOIN T_EmployeeMaster
                              ON Emt_EmployeeID =  But_EmployeeId
                            LEFT JOIN T_UserMaster C1 
                              ON C1.Umt_UserCode = But_CheckedBy
                            LEFT JOIN T_UserMaster C2 
                              ON C2.Umt_UserCode = But_Checked2By
                            LEFT JOIN T_UserMaster AP 
                              ON AP.Umt_UserCode = But_ApprovedBy
                            LEFT JOIN T_AccountDetail AD1 
                              ON AD1.Adt_AccountCode = But_Status 
                             AND AD1.Adt_AccountType =  'WFSTATUS'
                            LEFT JOIN T_AccountDetail AD2
                              ON AD2.Adt_AccountCode = But_Relationship
                             AND AD2.Adt_AccountType = 'RELATION'
                            LEFT JOIN T_AccountDetail AD3
                              ON AD3.Adt_AccountCode = But_Hierarchy
                             AND AD3.Adt_AccountType = 'HIERARCHDP'
                            LEFT JOIN T_TransactionRemarks 
                              ON Trm_ControlNo = But_ControlNo
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
                             AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
                             AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                              ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                           WHERE ( (But_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (But_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (But_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", But_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( But_ControlNo LIKE '{0}%' )
                                      OR ( But_EmployeeId LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '%{0}%' )
                                      OR ( Emt_Lastname LIKE '%{0}%' )
                                      OR ( Emt_Firstname LIKE '%{0}%')
                                      OR ( Convert(varchar(10), But_EffectivityDate, 101) LIKE '%{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),But_EffectivityDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),But_AppliedDate,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( But_Lastname LIKE '{0}%' )
                                      OR ( But_Firstname LIKE '%{0}%')
                                      OR ( But_Middlename LIKE '{0}%')
                                      OR ( Convert(varchar(10), But_Birthdate, 101) LIKE '%{0}%')
                                      OR ( But_Relationship LIKE '{0}%')
                                      OR ( AD2.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( But_Hierarchy LIKE '{0}%')
                                      OR ( AD3.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Convert(varchar(10), But_DeceasedDate, 101) LIKE '%{0}%')
                                      OR ( Convert(varchar(10), But_CancelDate, 101) LIKE '%{0}%')
                                      OR ( AD1.Adt_Accountdesc LIKE '{0}%')
                                      OR ( But_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE But_Status
                                              WHEN '3' THEN dbo.GetControlEmployeeName(But_EmployeeId)
                                              WHEN '5' THEN dbo.GetControlEmployeeNameV2(But_CheckedBy)
                                              WHEN '7' THEN dbo.GetControlEmployeeNameV2(But_Checked2By)
                                              ELSE ''
                                          END LIKE '{0}%')
                                      OR ( CASE But_Status
                                                WHEN '3' THEN CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),But_CheckedDate,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),But_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),But_Checked2Date,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),But_Checked2Date,114),5)
                                                ELSE ''
                                           END LIKE '{0}%')
                                    {1} )
                                    ";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( But_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(But_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(But_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion

        return sql;
    }
    
    private string getAddressData()
    {
        #region SQL
        string sqlCounters = string.Empty;
        string additionalCondition = string.Empty;
        if (Request.QueryString["condition"] != null && Request.QueryString["condition"].ToString().Trim() != "")
        {
            additionalCondition = Request.QueryString["condition"].ToString().ToUpper();
            string[] extractCondition = additionalCondition.Split('|');
            string ControNumbers = string.Empty;
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null && ControlNumber.Trim() != "")
                    ControNumbers += "'" + ControlNumber + "',";
            }
            if (ControNumbers != null && ControNumbers.Trim() != "")
            {
                ControNumbers = ControNumbers.Remove(ControNumbers.ToString().LastIndexOf(","), ",".Length);
                sqlCounters = string.Format(@" AND Amt_ControlNo in ({0})", ControNumbers);
            }
        }
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql =string.Format(@"   SELECT Amt_ControlNo [Control No]
                               , Amt_EmployeeId [ID No]
                               , AD1.Adt_AccountDesc [Status]
                               , Emt_NickName [ID Code]
                               , Emt_NickName [Nickname]
                               , Emt_Lastname [Lastname]
                               , Emt_Firstname [Firstname]
                               , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                               , Convert(varchar(10), Amt_EffectivityDate, 101) [Effectivity Date]
                               , CASE Amt_Type 
                                      WHEN 'A1' THEN 'Present'
                                      WHEN 'A2' THEN 'Permanent'
                                      WHEN 'A3' THEN 'Emergency Contact'
                                      ELSE ''
                                  END [Type]
                               , Amt_Address1 [Number/Street]
                               , ADADDRESS2.Adt_AccountDesc [Barangay/Municipality]
                               , ADADDRESS3.Adt_AccountDesc [City/Province/District]
                               , Amt_TelephoneNo [Telephone No]
                               , case Amt_Type
		                            when 'A1' then Amt_CellularNo 
		                            else '- not applicable -'
	                            end [Cellular No]
                                , case Amt_Type
		                            when 'A1' then Amt_EmailAddress 
		                            else '- not applicable -'
	                            end [Email Address]
                                , case Amt_Type 
					                when 'A1' then Rte_RouteCode
					                else '- not applicable -'
				                end [Route Code]
			                    , case Amt_Type	
					                when 'A1' then Rte_RouteName
					                else '- not applicable -'
				                end [Route Name]
			                    , case Amt_Type	
									when 'A1' then CAST(Rte_Amount AS VARCHAR)
									else '- not applicable -'
								end [Amount]
                                , CASE Amt_Type 
                                        WHEN 'A3' THEN Amt_ContactPerson
                                        ELSE '- not applicable -'
                                    END [Contact Person]
                                  , CASE Amt_Type 
                                        WHEN 'A3' THEN ADRelation.Adt_AccountDesc
                                        ELSE '- not applicable -'
                                    END [Contact Relation]
                               , Amt_Reason [Reason]
                               , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 4)) [Department]
                               , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) [Section]
                               , CONVERT(varchar(10),Amt_EndorsedDateToChecker,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Amt_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                               , CONVERT(varchar(10),Amt_AppliedDate,101) 
                                 + ' ' 
                                 + LEFT(CONVERT(varchar(20),Amt_AppliedDate,114),5)[Applied Date/Time]
                               , CASE Amt_Status
                                      WHEN '3' THEN dbo.GetControlEmployeeName(Amt_EmployeeId)
                                      WHEN '5' THEN dbo.GetControlEmployeeNameV2(Amt_CheckedBy)
                                      WHEN '7' THEN dbo.GetControlEmployeeNameV2(Amt_Checked2By)
                                      ELSE ''
                                  END [Last Updated By]
                              , CASE Amt_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Amt_EndorsedDateToChecker,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Amt_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Amt_CheckedDate,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Amt_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Amt_Checked2Date,101) 
                                                 + ' ' 
                                                 + LEFT(CONVERT(varchar(20),Amt_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                            FROM T_AddressMovement
                            INNER JOIN T_EmployeeMaster
                              ON Emt_EmployeeID =  Amt_EmployeeId
                            LEFT JOIN T_UserMaster C1 
                              ON C1.Umt_UserCode = Amt_CheckedBy
                            LEFT JOIN T_UserMaster C2 
                              ON C2.Umt_UserCode = Amt_Checked2By
                            LEFT JOIN T_UserMaster AP 
                              ON AP.Umt_UserCode = Amt_ApprovedBy
                            LEFT JOIN T_AccountDetail AD1 
                              ON AD1.Adt_AccountCode = Amt_Status 
                             AND AD1.Adt_AccountType =  'WFSTATUS'
                            LEFT JOIN T_AccountDetail ADADDRESS2 
                              ON ADADDRESS2.Adt_AccountCode = Amt_Address2
                             AND ADADDRESS2.Adt_AccountType =  'BARANGAY'
                            LEFT JOIN T_AccountDetail ADADDRESS3
                              ON ADADDRESS3.Adt_AccountCode = Amt_Address3 
                             AND ADADDRESS3.Adt_AccountType =  'ZIPCODE'
                            LEFT JOIN T_TransactionRemarks 
                              ON Trm_ControlNo = Amt_ControlNo
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                              ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
                             AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
                             AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                              ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                            LEFT JOIN T_AccountDetail ADRelation
                                ON ADRelation.Adt_AccountCode = Amt_ContactRelation
                                AND ADRelation.Adt_AccountType = 'RELATION'
                            LEFT JOIN T_RouteMaster ON Rte_RouteCode = Amt_Filler1 
				                    AND Rte_EffectivityDate = (SELECT MAX(Rte_EffectivityDate) 
									    FROM T_RouteMaster
									        WHERE Rte_EffectivityDate <= Amt_EffectivityDate
									        AND Rte_RouteCode = Amt_Filler1)
                           WHERE ( (Amt_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Amt_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Amt_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))
                                @ADDITIONALCONDITION", !hasCCLine ? "" : ", Amt_CostCenterLine [CC Line]");

        string searchFilter = @"AND (    ( Amt_ControlNo LIKE '{0}%' )
                                      OR ( Amt_EmployeeId LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '%{0}%' )
                                      OR ( Emt_Lastname LIKE '%{0}%' )
                                      OR ( Emt_Firstname LIKE '%{0}%')
                                      OR ( Convert(varchar(10), Amt_EffectivityDate, 101) LIKE '%{0}%')
                                      OR ( Amt_Address2 LIKE '%{0}%')
                                      OR ( ADADDRESS2.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( Amt_Address3 LIKE '%{0}%') 
                                      OR ( ADADDRESS3.Adt_AccountDesc LIKE '%{0}%')
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 4)) LIKE '{0}%')
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Amt_EffectivityDate,101) LIKE '%{0}%')
                                      OR ( CONVERT(varchar(10),Amt_AppliedDate,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Amt_AppliedDate,114),5) LIKE '%{0}%')
                                      OR ( AD1.Adt_Accountdesc LIKE '{0}%')
                                      OR ( Amt_Reason LIKE '{0}%')
                                      OR ( CONVERT(varchar(10),Amt_EndorsedDateToChecker,101) 
                                         + ' ' 
                                         + LEFT(CONVERT(varchar(20),Amt_EndorsedDateToChecker,114),5) LIKE '%{0}%')
                                      OR ( CASE Amt_Status
                                              WHEN '3' THEN dbo.GetControlEmployeeName(Amt_EmployeeId)
                                              WHEN '5' THEN dbo.GetControlEmployeeNameV2(Amt_CheckedBy)
                                              WHEN '7' THEN dbo.GetControlEmployeeNameV2(Amt_Checked2By)
                                              ELSE ''
                                          END LIKE '{0}%')
                                      OR ( CASE Amt_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Amt_EndorsedDateToChecker,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Amt_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Amt_CheckedDate,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Amt_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Amt_Checked2Date,101) 
                                                            + ' ' 
                                                            + LEFT(CONVERT(varchar(20),Amt_Checked2Date,114),5)
                                                ELSE ''
                                           END LIKE '{0}%')
                                    {1} )
                                    ";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Amt_CostCenterLine LIKE '{0}%' )", holder));
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Amt_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Amt_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";
        sql = sql.Replace("@ADDITIONALCONDITION", sqlCounters);
        #endregion
        
        return sql;
    }

    private string getStraightWorkData()
    {
        #region SQL
        //ParameterInfo[] param = new ParameterInfo[2];
        //param[0] = new ParameterInfo("@UserLogged", (Request.QueryString["usercode"] != null && Request.QueryString["usercode"].ToString().Trim() != "") ? Request.QueryString["usercode"].ToString().Trim() : Session["userLogged"].ToString());
        //param[1] = new ParameterInfo("@filterCostCenter", ddlCostCenter.SelectedValue.ToString());
        string sql = @"  SELECT Swt_ControlNo [Control No]
                              , Swt_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Swt_FromDate,101) [From Date]
                              , CONVERT(varchar(10),Swt_ToDate,101) [To Date]
                              , CONVERT(varchar(10),Swt_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Swt_AppliedDate,114),5)[Applied Date/Time]
                              , '['+Swt_ShiftCode+'] '
			                              + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
			                              + '-'
			                              + LEFT(S1.Scm_ShiftBreakStart,2) + ':' + RIGHT(S1.Scm_ShiftBreakStart,2)
			                              + '  ' 
			                              + LEFT(S1.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S1.Scm_ShiftBreakEnd,2)
			                              + '-'
			                              + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2) 
		                            [Shift]
                              , Swt_UnpaidBreak [Unpaid Break]
                              , Swt_Reason [Reason]
                              , dbo.getCostCenterFullNameV2(LEFT(Swt_Costcenter, 4)) [Department]
                              , dbo.getCostCenterFullNameV2(LEFT(Swt_Costcenter, 6)) [Section]
                              
                              , CONVERT(varchar(10),Swt_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Swt_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Swt_Status
	                                 WHEN '3' THEN dbo.GetControlEmployeeName(Swt_EmployeeId)
			                         WHEN '5' THEN dbo.GetControlEmployeeNameV2(Swt_CheckedBy)
			                         WHEN '7' THEN dbo.GetControlEmployeeNameV2(Swt_Checked2By)
			                         ELSE ''
                                 END [Last Updated By]
                              , CASE Swt_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Swt_EndorsedDateToChecker,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Swt_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Swt_CheckedDate,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Swt_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Swt_Checked2Date,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Swt_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                              , AD2.Adt_AccountDesc [@Swt_Filler1Desc]
                              , AD3.Adt_AccountDesc [@Swt_Filler2Desc]
                              , AD4.Adt_AccountDesc [@Swt_Filler3Desc]
                              , Swt_BatchNo [Batch No]
                           FROM T_EmployeeStraightWork
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Swt_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Swt_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Swt_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Swt_ApprovedBy
                           LEFT JOIN T_ShiftCodeMaster S1
                             ON S1.Scm_ShiftCode = Swt_ShiftCode
                           LEFT JOIN T_AccountDetail AD2
                             ON AD2.Adt_AccountCode = Swt_Filler1
                            AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler01')
                           LEFT JOIN T_AccountDetail AD3
                             ON AD3.Adt_AccountCode = Swt_Filler2
                            AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler02')
                           LEFT JOIN T_AccountDetail AD4
                             ON AD4.Adt_AccountCode = Swt_Filler3
                            AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler03')
                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Swt_Status
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Swt_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'STRAIGHTWK'
                            AND Swt_FromDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Swt_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Swt_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Swt_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))";

        string searchFilter = @"AND (    ( Swt_ControlNo LIKE '{0}%' )
                                      OR ( Swt_EmployeeId LIKE '{0}%' )
                                      OR ( ADT1.Adt_AccountDesc LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_Lastname LIKE '{0}%' )
                                      OR ( Emt_Firstname LIKE '{0}%' )
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Swt_Costcenter, 4)) LIKE '%{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Swt_Costcenter, 6)) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Swt_FromDate,101) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Swt_ToDate,101) LIKE '%{0}%' )
                                      OR ( Swt_UnpaidBreak LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Swt_AppliedDate,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Swt_AppliedDate,114),5) LIKE '%{0}%' )
                                      OR ( Swt_Reason LIKE '{0}%' )
                                      OR ( CONVERT(varchar(10),Swt_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Swt_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                      OR ( CASE Swt_Status
	                                         WHEN '3' THEN dbo.GetControlEmployeeName(Swt_EmployeeId)
			                                 WHEN '5' THEN dbo.GetControlEmployeeNameV2(Swt_CheckedBy)
			                                 WHEN '7' THEN dbo.GetControlEmployeeNameV2(Swt_Checked2By)
			                                 ELSE ''
                                         END  LIKE '{0}%' )
                                      OR ( CASE Swt_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Swt_EndorsedDateToChecker,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Swt_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Swt_CheckedDate,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Swt_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Swt_Checked2Date,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Swt_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '%{0}%' )
                                      OR ( Swt_Filler1 LIKE '{0}%' )
                                      OR ( Swt_Filler2 LIKE '{0}%' )
                                      OR ( Swt_Filler3 LIKE '{0}%' )
                                      OR ( Swt_BatchNo LIKE '{0}%' )
                                    )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder);
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Swt_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Swt_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";

        sql = sql.Replace("@Swt_Filler1Desc", CommonMethods.getFillerName("Swt_Filler01"));
        sql = sql.Replace("@Swt_Filler2Desc", CommonMethods.getFillerName("Swt_Filler02"));
        sql = sql.Replace("@Swt_Filler3Desc", CommonMethods.getFillerName("Swt_Filler03"));
        #endregion

        return sql;
    }

    private string getGatePassData()
    {
        #region SQL

        string sql = @" SELECT Egp_ControlNo [Control No]
                              , Egp_EmployeeId [ID No]
                              , ADT1.Adt_AccountDesc [Status]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                              , CONVERT(varchar(10),Egp_GatePassDate,101) [Gate Pass Date]
                              , CONVERT(varchar(10),Egp_AppliedDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Egp_AppliedDate,114),5)[Applied Date/Time]
                              , Egp_ApplicationTypeRemarks [Application Type Remarks]
                              , dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 4)) [Department]
                              , dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 6)) [Section]
                              
                              , CONVERT(varchar(10),Egp_EndorsedDateToChecker,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Egp_EndorsedDateToChecker,114),5)[Endorsed Date/Time]
                              , CASE Egp_Status
	                                 WHEN '3' THEN dbo.GetControlEmployeeName(Egp_EmployeeId)
			                         WHEN '5' THEN dbo.GetControlEmployeeNameV2(Egp_CheckedBy)
			                         WHEN '7' THEN dbo.GetControlEmployeeNameV2(Egp_Checked2By)
			                         ELSE ''
                                 END [Last Updated By]
                              , CASE Egp_Status
                                     WHEN '3' THEN CONVERT(varchar(10),Egp_EndorsedDateToChecker,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Egp_EndorsedDateToChecker,114),5)
                                     WHEN '5' THEN CONVERT(varchar(10),Egp_CheckedDate,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Egp_CheckedDate,114),5)
                                     WHEN '7' THEN CONVERT(varchar(10),Egp_Checked2Date,101) 
						                         + ' ' 
						                         + LEFT(CONVERT(varchar(20),Egp_Checked2Date,114),5)
                                     ELSE ''
                                 END [Date Last Updated]
                           FROM E_EmployeeGatePass
                          INNER JOIN T_EmployeeMaster
                             ON Emt_EmployeeId = Egp_EmployeeId
                           LEFT JOIN T_UserMaster UMT1
                             ON UMT1.Umt_UserCode = Egp_CheckedBy
                           LEFT JOIN T_UserMaster UMT2
                             ON UMT2.Umt_UserCode = Egp_Checked2By
                           LEFT JOIN T_UserMaster UMT3
                             ON UMT3.Umt_UserCode = Egp_ApprovedBy
                           LEFT JOIN T_AccountDetail ADT1
                             ON ADT1.Adt_AccountType = 'WFSTATUS'
                            AND ADT1.Adt_AccountCode = Egp_Status
                           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                             ON empApprovalRoute.Arm_EmployeeId = Egp_EmployeeId
                            AND empApprovalRoute.Arm_TransactionId = 'GATEPASS'
                            AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                          INNER JOIN T_ApprovalRouteMaster AS routeMaster
                             ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                          WHERE (  (Egp_Status = '3' AND routeMaster.Arm_Checker1 = @UserLogged)
                                OR (Egp_Status = '5' AND routeMaster.Arm_Checker2 = @UserLogged)
                                OR (Egp_Status = '7' AND routeMaster.Arm_Approver = @UserLogged))";

        string searchFilter = @"AND (    ( Egp_ControlNo LIKE '{0}%' )
                                      OR ( Egp_EmployeeId LIKE '{0}%' )
                                      OR ( ADT1.Adt_AccountDesc LIKE '%{0}%' )
                                      OR ( Emt_NickName LIKE '{0}%' )
                                      OR ( Emt_Lastname LIKE '{0}%' )
                                      OR ( Emt_Firstname LIKE '{0}%' )
                                      OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 4)) LIKE '%{0}%' )
                                      OR ( dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 6)) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Egp_GatePassDate,101) LIKE '%{0}%' )
                                      OR ( CONVERT(varchar(10),Egp_AppliedDate,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Egp_AppliedDate,114),5) LIKE '%{0}%' )
                                      OR ( Egp_ApplicationTypeRemarks LIKE '{0}%' )
                                      OR ( CONVERT(varchar(10),Egp_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + LEFT(CONVERT(varchar(20),Egp_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                      OR ( CASE Egp_Status
	                                         WHEN '3' THEN dbo.GetControlEmployeeName(Egp_EmployeeId)
			                                 WHEN '5' THEN dbo.GetControlEmployeeNameV2(Egp_CheckedBy)
			                                 WHEN '7' THEN dbo.GetControlEmployeeNameV2(Egp_Checked2By)
			                                 ELSE ''
                                         END  LIKE '{0}%' )
                                      OR ( CASE Egp_Status
                                                WHEN '3' THEN CONVERT(varchar(10),Egp_EndorsedDateToChecker,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Egp_EndorsedDateToChecker,114),5)
                                                WHEN '5' THEN CONVERT(varchar(10),Egp_CheckedDate,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Egp_CheckedDate,114),5)
                                                WHEN '7' THEN CONVERT(varchar(10),Egp_Checked2Date,101) 
						                                    + ' ' 
						                                    + LEFT(CONVERT(varchar(20),Egp_Checked2Date,114),5)
                                                ELSE ''
                                            END LIKE '%{0}%' )
                                    )";

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            sql += string.Format(searchFilter, holder);
        }

        sql += rblCCOption.SelectedValue.Equals("D") ? " AND ( LEFT(Egp_CostCenter,4) = @filterCostCenter OR 'ALL' = @filterCostCenter)" : " AND ( LEFT(Egp_CostCenter,6) = @filterCostCenter OR 'ALL' = @filterCostCenter)";

        #endregion

        return sql;
    }
    #endregion

    #endregion

    #region Events
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        this.bindGrid();
        UpdatePagerLocation();
        this.txtSearch.Focus();
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        CheckAll();
        buttonControl();
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string VIEWLOGREFERENCE = Resources.Resource.VIEWLOGREFERENCE;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.dgvResult, "Select$" + e.Row.RowIndex);
            e.Row.Attributes.Add("onclick", "javascript:return ChecklistSelection('" + e.Row.RowIndex + "')"); ;
            if (Convert.ToBoolean(VIEWLOGREFERENCE))
            {
                if (Request.QueryString["type"].ToString().Equals("TR"))
                {
                    e.Row.Attributes["ondblclick"] = "javascript:return lookupTKProximityLogs2('" + e.Row.Cells[2].Text + "','" + e.Row.Cells[7].Text + "');";
                }
            }
        }
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    }

    protected void dgvError_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    }

    protected void rblOtion_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblOtion.SelectedValue.ToString().Equals("A"))
        {
            pnlForChecking.Visible = true;
            pnlForDisapprove.Visible = false;
        }
        else
        {
            pnlForChecking.Visible = false;
            pnlForDisapprove.Visible = true;
            btnDisapprove.Text = "DISAPPROVE";
            if (rblOtion.SelectedValue.ToString().Equals("R"))
            {
                btnDisapprove.Text = "RETURN";
            }
        }

        bool isChecked = false;
        foreach (GridViewRow row in dgvResult.Rows)
        {
            isChecked = ((CheckBox)row.FindControl("chkBox")).Checked;
            if (isChecked)
                break;
        }

        if (isChecked)
        {
            btnApprove.Enabled = true;
            btnDisapprove.Enabled = true;
        }
        else
        {
            btnApprove.Enabled = false;
            btnDisapprove.Enabled = false;
        }

    }

    protected void rblCCOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        fillCostcenterDropdown();
        bindGrid();
        UpdatePagerLocation();
    }

    protected void ddlCostCenter_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.bindGrid();
        UpdatePagerLocation();
        this.txtSearch.Focus();
    }

    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnLoad.Enabled = true;
        buttonControl();
        //assignButtonLoad();
    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        Response.Write("<script type='text/javascript'>window.close();</script>");
    }

    protected void btnEndorseChecker2_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Endorse);
    }

    protected void btnEndorseApprover_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Endorse);
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Approval);
    }

    private void ProcessTransaction(WFProcess wfProcess)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            int countAffected = 0;
            int countCutOff = 0;
            dtErrors.Columns.Add("Control No"); 
            dtErrors.Columns.Add("Exception");

            #region Get Transaction Type Details
            string strStoredProcName = "";
            string transactionSystemID = "";
            string strRouteTransactionID = "";
            string strWFTransactionType = Request.QueryString["type"].ToString().ToUpper();
            switch (strWFTransactionType)
            {
                case "OT":
                    strStoredProcName = "ApproveOvertime";
                    transactionSystemID = "OVERTIME";
                    strRouteTransactionID = "OVERTIME";
                    break;
                case "LV":
                    strStoredProcName = "ApproveLeave";
                    transactionSystemID = "LEAVE";
                    strRouteTransactionID = "LEAVE";
                    break;
                case "TR":
                    strStoredProcName = "ApproveTimeRecMod";
                    transactionSystemID = "TIMEKEEP";
                    strRouteTransactionID = "TIMEMOD";
                    break;
                //case "FT":
                //    strStoredProcName = "ApproveFlexTime";
                //    transactionSystemID = "TIMEKEEP";
                //    strRouteTransactionID = "FLEXTIME";
                //    break;
                //case "JS":
                //    strStoredProcName = "ApproveJobSplit";
                //    transactionSystemID = "TIMEKEEP";
                //    strRouteTransactionID = "JOBMOD";
                //    break;
                case "MV":
                    strStoredProcName = "ApproveMovement";
                    transactionSystemID = "TIMEKEEP";
                    strRouteTransactionID = "MOVEMENT";
                    break;
                case "TX":
                    strStoredProcName = "ApproveTaxCodeCivilStatusUpdate";
                    transactionSystemID = "PAYROLL";
                    strRouteTransactionID = "TAXMVMNT";
                    break;
                case "BF":
                    strStoredProcName = "ApproveBeneficiaryUpdate";
                    transactionSystemID = "PAYROLL";
                    strRouteTransactionID = "BNEFICIARY";
                    break;
                case "AD":
                    strStoredProcName = "ApproveAddressMovement";
                    transactionSystemID = "PAYROLL";
                    strRouteTransactionID = "ADDRESS";
                    break;
                case "GP":
                    transactionSystemID = "PERSONNEL";
                    strRouteTransactionID = "GATEPASS";
                    break;
                //case "SW":
                //    strStoredProcName = "ApproveStraightWork";
                //    transactionSystemID = "TIMEKEEP";
                //    strRouteTransactionID = "STRAIGHTWK";
                //    break;
            }
            #endregion

            if (wfProcess == WFProcess.Disapprove && rblOtion.SelectedValue == "R")
            {
                wfProcess = WFProcess.Return;
            }

            if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
            {
                string controlNumbers = string.Empty;                
                Dictionary<string,string> transactionTypeDictionary = new Dictionary<string,string>();

                if ((wfProcess == WFProcess.Disapprove && string.IsNullOrEmpty(txtRemarks.Text))
                    || (wfProcess == WFProcess.Return && string.IsNullOrEmpty(txtRemarks.Text)))
                {
                    MessageBox.Show("Remarks are required.");
                    return;
                }

                string strStatus = "";
                string strForEndorseToChecker2 = "";
                string strForEndorseToApprover = "";
                string strForApproval = "";
                string strForDisapproval = "";
                string strForReturn = "";

                CheckBox CB;
                for (int i = 0; i < dgvResult.Rows.Count; i++)
                {
                    CB = (CheckBox)dgvResult.Rows[i].Cells[0].FindControl("chkBox");
                    if (CB.Checked)
                    {
                        if (wfProcess == WFProcess.Endorse)
                        {
                            if (dgvResult.Rows[i].Cells[3].Text.ToUpper().Trim().Equals("ENDORSED TO CHECKER 1"))
                            {
                                strStatus = CommonMethods.getStatusRoute(dgvResult.Rows[i].Cells[2].Text.ToUpper().Trim(), strRouteTransactionID, btnEndorseChecker2.Text.Trim().ToUpper());
                                if (strStatus == "5")
                                    strForEndorseToChecker2 += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                                else if (strStatus == "7")
                                    strForEndorseToApprover += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                                else if (strStatus == "9")
                                    strForApproval += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                            }
                            if (dgvResult.Rows[i].Cells[3].Text.ToUpper().Trim().Equals("ENDORSED TO CHECKER 2"))
                            {
                                strForEndorseToApprover += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                            }
                        }
                        else if (wfProcess == WFProcess.Approval)
                        {
                            if (dgvResult.Rows[i].Cells[3].Text.ToUpper().Trim().Equals("ENDORSED TO APPROVER"))
                            {
                                strForApproval += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                            }
                        }
                        else if (wfProcess == WFProcess.Disapprove)
                        {
                            strForDisapproval += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                        }
                        else if (wfProcess == WFProcess.Return)
                        {
                            strForReturn += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                        }
                    }
                }

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        string strEndorseToChecker2Query = "";
                        string strEndorseToApproverQuery = "";
                        string strForApprovalQuery = "";
                        string strForDisapprovalQuery = "";
                        string strForReturnQuery = "";

                        if (strForEndorseToChecker2 != "")
                        {
                            strForEndorseToChecker2 = strForEndorseToChecker2.Substring(0, strForEndorseToChecker2.Length - 1);
                            strEndorseToChecker2Query = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' "
                                                                        , strForEndorseToChecker2
                                                                        , Session["userLogged"].ToString()
                                                                        , strWFTransactionType
                                                                        , "5"
                                                                        , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                        }
                        if (strForEndorseToApprover != "")
                        {
                            strForEndorseToApprover = strForEndorseToApprover.Substring(0, strForEndorseToApprover.Length - 1);
                            strEndorseToApproverQuery = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' "
                                                                        , strForEndorseToApprover
                                                                        , Session["userLogged"].ToString()
                                                                        , strWFTransactionType
                                                                        , "7"
                                                                        , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                        }
                        if (strForApproval != "")
                        {
                            strForApproval = strForApproval.Substring(0, strForApproval.Length - 1);
                            if (!strWFTransactionType.ToUpper().Equals("GP"))
                            {
                                strForApprovalQuery = string.Format("EXEC {0} '{1}', '{2}', '{3}' "
                                                                            , strStoredProcName
                                                                            , strForApproval
                                                                            , Session["userLogged"].ToString()
                                                                            , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                            }
                            else
                            {
                                strForApprovalQuery = string.Format(@" DECLARE @ControlNo AS VARCHAR(12)
                                                                                DECLARE @EmployeeId AS VARCHAR(15)
                                                                                DECLARE @RecordDate AS DATETIME
                                                                                DECLARE @RecordStatus AS CHAR(1)
                                                                                DECLARE @Ludatetime AS DATETIME
                                                                                DECLARE @APPROVED_STATUS AS CHAR(1) = '9'
                                                                                DECLARE @TranCounter INT;  
                                                                                DECLARE @ApprovedBy AS VARCHAR(15)
                                                                                DECLARE @CreateEmailNotification BIT
                                                                                SET @ApprovedBy = '{1}'
                                                                                SET @CreateEmailNotification = '{2}'
                                                                                SET @TranCounter = @@TRANCOUNT; 

                                                                                CREATE TABLE #Result (
	                                                                                [ControlNo] VARCHAR(12) NOT NULL,
	                                                                                [Result] INT NULL,
	                                                                                [Message] NVARCHAR(4000) NULL
                                                                                ) 
                                                                                DECLARE database_cursor CURSOR FOR 
                                                                                SELECT Data
                                                                                FROM dbo.Split('{0}',',') --DELIMITER IS COMMA

                                                                                OPEN database_cursor
                                                                                FETCH NEXT FROM database_cursor INTO @ControlNo

                                                                                WHILE @@FETCH_STATUS = 0 
                                                                                BEGIN 
	                                                                                BEGIN TRY
		                                                                                IF @TranCounter > 0
		                                                                                BEGIN
			                                                                                IF @ControlNo <> ''
				                                                                                SAVE TRANSACTION @ControlNo; --REUSED @ControlNo VARIABLE AS SAVEPOINT NAME
			                                                                                ELSE
				                                                                                SAVE TRANSACTION TMPSVPT;
		                                                                                END
		                                                                                ELSE 
			                                                                                BEGIN TRANSACTION;

		                                                                                IF NOT EXISTS (SELECT Egp_Status 
						                                                                                FROM E_EmployeeGatePass
						                                                                                WHERE Egp_ControlNo = @ControlNo)
			                                                                                THROW 51000,'Transaction does not exist',1;

		                                                                                IF EXISTS (SELECT Egp_Status
					                                                                                FROM E_EmployeeGatePass
					                                                                                WHERE Egp_ControlNo = @ControlNo
						                                                                                AND Egp_Status <> @APPROVED_STATUS)
		                                                                                BEGIN
			                                                                                SELECT @EmployeeId = Egp_EmployeeId
				                                                                                , @RecordDate = Egp_GatePassDate 
				                                                                                , @RecordStatus = Egp_Status
			                                                                                FROM E_EmployeeGatePass
			                                                                                WHERE Egp_ControlNo = @ControlNo
				                                                                                AND Egp_Status <> @APPROVED_STATUS

			                                                                                IF @RecordStatus IN ('4','6','8')
				                                                                                THROW 56000,'Transaction already disapproved',1;

			                                                                                IF @RecordStatus IN ('0','2')
				                                                                                THROW 56000,'Transaction already cancelled',1;

			                                                                                IF @RecordStatus IN ('1')
				                                                                                THROW 56000,'Transaction cannot be approved',1;

			                                                                                SET @Ludatetime = GETDATE()

		
			                                                                                BEGIN
				                                                                                UPDATE E_EmployeeGatePass
					                                                                                SET Egp_ApprovedBy = @ApprovedBy
					                                                                                , Egp_ApprovedDate = @Ludatetime
					                                                                                , Egp_Status = @APPROVED_STATUS
				                                                                                WHERE Egp_ControlNo = @ControlNo
					                                                                                AND Egp_Status <> @APPROVED_STATUS
								
				                                                                                --CREATE EMAIL NOTIFICATION
				                                                                                IF @CreateEmailNotification = 1
				                                                                                BEGIN
					                                                                                UPDATE T_EmailNotification
					                                                                                SET Ent_Status = 'X'
					                                                                                WHERE Ent_ControlNo = @ControlNo
						                                                                                AND Ent_Status = 'A'

					                                                                                INSERT INTO T_EmailNotification
						                                                                                (Ent_ControlNo
						                                                                                ,Ent_SeqNo
						                                                                                ,Ent_TransactionType
						                                                                                ,Ent_Action
						                                                                                ,Ent_Status
						                                                                                ,Usr_Login
						                                                                                ,Ludatetime)
					                                                                                VALUES
						                                                                                (@ControlNo
						                                                                                ,ISNULL((SELECT RIGHT('000' + CONVERT(VARCHAR, MAX(CONVERT(INT, Ent_SeqNo)) + 1), 3) FROM T_EmailNotification WHERE Ent_ControlNo = @ControlNo), '001')
						                                                                                ,'GATEPASS'
						                                                                                ,'APPROVE'
						                                                                                ,'A'
						                                                                                ,@ApprovedBy
						                                                                                ,@Ludatetime)
				                                                                                END
			                                                                                END
		                                                                                END
		                                                                                ELSE
			                                                                                THROW 52000,'Transaction already approved',1;
		
		                                                                                IF @TranCounter = 0
			                                                                                COMMIT TRANSACTION

		                                                                                INSERT INTO #Result VALUES (@ControlNo, 1, 'Successful')
	                                                                                END TRY
	                                                                                BEGIN CATCH
		                                                                                IF @TranCounter = 0
			                                                                                ROLLBACK TRANSACTION
		                                                                                ELSE IF XACT_STATE() <> -1  
		                                                                                BEGIN
			                                                                                IF @ControlNo <> ''
				                                                                                ROLLBACK TRANSACTION @ControlNo; --ROLLBACK @ControlNo SAVEPOINT ONLY
			                                                                                ELSE
				                                                                                ROLLBACK TRANSACTION TMPSVPT;
		                                                                                END

		                                                                                INSERT INTO #Result VALUES (@ControlNo, ERROR_NUMBER(), ERROR_MESSAGE())
		
	                                                                                END CATCH

	                                                                                FETCH NEXT FROM database_cursor INTO @ControlNo
                                                                                END

                                                                                CLOSE database_cursor 
                                                                                DEALLOCATE database_cursor

                                                                                SELECT * FROM #Result", strForApproval
                                                                                              , Session["userLogged"].ToString()
                                                                                              , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION).Equals("TRUE"));
                            }
                        }
                        if (strForDisapproval != "")
                        {
                            strForDisapproval = strForDisapproval.Substring(0, strForDisapproval.Length - 1);
                            strForDisapprovalQuery = string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}' "
                                                                        , strForDisapproval
                                                                        , Session["userLogged"].ToString()
                                                                        , txtRemarks.Text.Trim().ToUpper()
                                                                        , strWFTransactionType
                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                        }
                        if (strForReturn != "")
                        {
                            strForReturn = strForReturn.Substring(0, strForReturn.Length - 1);
                            strForReturnQuery = string.Format("EXEC ReturnWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}' "
                                                                        , strForReturn
                                                                        , Session["userLogged"].ToString()
                                                                        , txtRemarks.Text.Trim().ToUpper()
                                                                        , strWFTransactionType
                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                        }

                        DataSet dsResult = dal.ExecuteDataSet(string.Format(@"BEGIN TRY
                                                                            BEGIN TRANSACTION
                                                                            {0}
                                                                            {1}
                                                                            {2}
                                                                            {3}
                                                                            {4}
                                                                            COMMIT TRANSACTION
                                                                            END TRY
                                                                            BEGIN CATCH
                                                                            ROLLBACK TRANSACTION
                                                                            THROW;
                                                                            END CATCH
                                                                            ", strEndorseToChecker2Query, strEndorseToApproverQuery, strForApprovalQuery, strForDisapprovalQuery, strForReturnQuery));
                        dtErrors.Rows.Clear();
                        for (int i = 0; i < dsResult.Tables.Count; i++)
                        {
                            ConsolidateResults(dsResult.Tables[i], ref countCutOff, ref countAffected);
                        }
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

                string cutOffMsg = string.Empty;
                if (countCutOff > 0)
                {
                    cutOffMsg = cutOffMsg + countCutOff + " row(s) affected on CUT-OFF.\n";
                    cutOffMsg = cutOffMsg + CommonMethods.GetErrorMessageForCutOff(transactionSystemID);
                }
                string affectedMsg = string.Empty;
                if (countAffected > 0)
                {
                    affectedMsg = countAffected + " row(s) affected.";
                    if (countCutOff > 0)
                    {
                        affectedMsg += "\nTransaction was approved because it belongs to the future quincena. Transaction does not affect the current payroll calculation.";
                    }
                    affectedMsg += "\n\n";
                }
                if (affectedMsg != string.Empty || cutOffMsg != string.Empty)
                {
                    MessageBox.Show(affectedMsg + cutOffMsg);
                    bindGrid();
                    UpdatePagerLocation();
                    btnLoad.Enabled = false;
                    btnEndorseChecker2.Enabled = false;
                    btnEndorseApprover.Enabled = false;
                    btnApprove.Enabled = false;
                    btnDisapprove.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
            if (dtErrors.Rows.Count > 0)
            {
                ibtnError.Visible = true;
                lblError.Visible = true;
                lblError.Text = "Show Errors: " + dtErrors.Rows.Count.ToString();
                dgvError.DataSource = dtErrors;
                dgvError.DataBind();
            }
            else
            {
                ibtnError.Visible = false;
                lblError.Visible = false;
                lblError.Text = string.Empty;
                dgvError.DataSource = new DataTable("Dummy");
                dgvError.DataBind();
                //MenuLog
                //SystemMenuLogBL.InsertEditLog("WFCHKAP", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "");

            }
        }
        else 
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }

    private void ConsolidateResults(DataTable dtResult, ref int countCutOff, ref int countAffected)
    {
        if (dtResult.Rows.Count > 0)
        {
            DataRow[] drArrRows;
            drArrRows = dtResult.Select("Result = 53000");
            countCutOff += drArrRows.Length;
            drArrRows = dtResult.Select("Result = 1");
            countAffected += drArrRows.Length;
            drArrRows = dtResult.Select("Result <> 1 and Result <> 53000");
            foreach (DataRow row in drArrRows)
            {
                dtErrors.Rows.Add(dtErrors.NewRow());
                dtErrors.Rows[dtErrors.Rows.Count - 1]["Control No"] = row["ControlNo"].ToString();
                dtErrors.Rows[dtErrors.Rows.Count - 1]["Exception"] = row["Message"].ToString();
            }
        }
    }

    protected void btnDisapprove_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Disapprove);
    }

    protected void btnGrow_Click(object sender, EventArgs e)
    {
        int max = Convert.ToInt32(Resources.Resource.MAXFONT);
        int s = Convert.ToInt32(dgvResult.Font.Size.ToString().Replace("pt",""));
        if ((s + 1) <= max)
        {
            dgvResult.Font.Size = FontUnit.Point(s + 1);

            btnGrow.Enabled = !((s + 1) == max);
        }
        btnShrink.Enabled = true;
    }

    protected void btnShrink_Click(object sender, EventArgs e)
    {
        int min = Convert.ToInt32(Resources.Resource.MINFONT);
        int s = Convert.ToInt32(dgvResult.Font.Size.ToString().Replace("pt", ""));
        if ((s - 1) >= min)
        {
            dgvResult.Font.Size = FontUnit.Point(s - 1);
            
            btnShrink.Enabled = !((s - 1) == min);
        }
        btnGrow.Enabled = true;
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
    #endregion
}
