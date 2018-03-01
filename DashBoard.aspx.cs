using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using System.Data;
using DevExpress.Web.ASPxGridView;
using Payroll.DAL;
using DevExpress.Web.ASPxTreeList;

public partial class DashBoard : System.Web.UI.Page
{
    CheckListBL CKBL = new CheckListBL();
    CommonMethods methods = new CommonMethods();
    DataTable dtErrors = new DataTable();
    System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
    enum WFProcess { EndorseApproval, Disapprove, Return };

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["dbConn"] != null)
        {
            sqlData.ConnectionString = CommonLibrary.Encrypt.decryptText(Session["dbConn"].ToString());
            //sqlData.ConnectionString = "Data Source=APSY500\SQL2012;Initial Catalog=PAYROLLGENIE_EPSON;Persist Security Info=True;User ID=SA;Password=systemadmin";
            SqlDetails.ConnectionString = sqlData.ConnectionString;
        }
    }
    private string CheckLevel
    {
        set
        {
            this.ViewState["CheckLevel"] = value;
        }
        get
        {
            return this.ViewState["CheckLevel"] == null
                    ? string.Empty
                    : this.ViewState["CheckLevel"].ToString().Trim();
        }
    }
    private string queryDetails
    {
        set
        {
            this.ViewState["queryDetails"] = value;
        }
        get
        {
            return this.ViewState["queryDetails"] == null
                    ? string.Empty
                    : this.ViewState["queryDetails"].ToString().Trim();
        }
    }
    private string TransactionType
    {
        set
        {
            this.ViewState["TransactionType"] = value;
        }
        get
        {
            return this.ViewState["TransactionType"] == null
                    ? string.Empty
                    : this.ViewState["TransactionType"].ToString().Trim();
        }
    }
    private string TransactionID
    {
        set
        {
            this.ViewState["TransactionID"] = value;
        }
        get
        {
            return this.ViewState["TransactionID"] == null
                    ? string.Empty
                    : this.ViewState["TransactionID"].ToString().Trim();
        }
    }
    private string Transaction
    {
        set
        {
            this.ViewState["Transaction"] = value;
        }
        get
        {
            return this.ViewState["Transaction"] == null
                    ? string.Empty
                    : this.ViewState["Transaction"].ToString().Trim();
        }
    }
    private string GetQuery
    {
        set
        {
            this.ViewState["GetQuery"] = value;
        }
        get
        {
            return this.ViewState["GetQuery"] == null
                    ? string.Empty
                    : this.ViewState["GetQuery"].ToString().Trim();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
         if (!CommonMethods.isAlive())
        {
            Response.Redirect("index.aspx?pr=dc");
        }
        // if (!Page.IsPostBack)
         //{
             InitialzieCounters();
             agvData.Settings.ShowGroupedColumns = false;
             agvData.Settings.ShowGroupPanel = false;
             showChecklist();
             if (this.GetQuery.Trim() != string.Empty)
             {
                 this.sqlData.SelectCommand = this.GetQuery;

             }
             if (this.queryDetails.Trim() != string.Empty)
             {
                 this.SqlDetails.SelectCommand = this.queryDetails;

                 for (int i = 0; i < agvDetails.Columns.Count; i++)
                 {
                     agvDetails.Columns[i].CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False;
                 }
             }
             if (!IsPostBack)
             {
                 ASPxSplitter1.Panes[1].Collapsed = true;
             }
             FT.Visible = false;
             JS.Visible = false;
             SW.Visible = false;
             MFT.Visible = false;
             MJS.Visible = false;
             MSW.Visible = false;
             LoadComplete += new EventHandler(Dashboard_LoadComplete);
        // }
    }
    private void showChecklist()
    {
        OT.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFOTENTRY");
        LV.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFLVEENTRY");
        TR.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFTIMERECENTRY");
        FT.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFFLXENTRY");
        JS.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFJOBSPLTMOD");
        MV.Visible = (CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFSHIFTUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFGROUPUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFCCUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFRESTDAY"));
        BF.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFBENEUPDATE");
        TX.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFTAXCODE");
        AD.Visible = (CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDPRES")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDPERMA")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDEMER"));
        GP.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFGATEPASS");
        approverOverride.Visible = pnlChecklist.Visible = (OT.Visible || LV.Visible || TR.Visible || FT.Visible || JS.Visible || MV.Visible || TX.Visible || BF.Visible || AD.Visible || GP.Visible);
        if (approverOverride.Visible)
        {
            approverOverride.Visible = IsApproverOverride(Session["userLogged"].ToString());
        }
        MOT.Visible = MOT.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME);
        MLV.Visible = MLV.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE);
        MTR.Visible = MTR.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION);
        MFT.Visible = MFT.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWFLEXTIME);
        MJS.Visible = MJS.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWJOBSPLIT);
        MMV.Visible = MMV.Visible = MMV.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION);
        MBF.Visible = MBF.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWBENEFICIARY);
        MTX.Visible = MTX.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTAXCIVIL);
        MAD.Visible = MAD.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWADDRESS);
        MSW.Visible = MSW.Visible = MSW.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWSTRAIGHTWORK);
        MGP.Visible = MGP.Visible = MGP.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWGATEPASS);
    }

    private bool IsApproverOverride(string userCode)
    {
        bool retVal = false;
        DataSet ds = new DataSet();
        string query = string.Format(@"
SELECT *
FROM T_ApprovalRouteOverrideMaster
WHERE Aro_Usercode = '{0}'", userCode);
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
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
            retVal = true;
        }

        return retVal;
    }


    private DataSet GetApproverOverride(string userCode, string userOverride)
    {
        DataSet ds = new DataSet();
        string query = string.Format(@"
SELECT *
FROM T_ApprovalRouteOverrideMaster
WHERE Aro_Usercode = '{0}' AND Aro_UserOverride = '{1}'", userCode, userOverride);
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
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
        if (CommonMethods.isEmpty(ds))
        {
            ds = null;
        }

        return ds;
    }

    protected void Dashboard_LoadComplete(object sender, EventArgs e)
    {
        this.btnOTW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnLVW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTRW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnFTW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnJSW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnMVW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTXW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnBFW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnADW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnSWW.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnGPW.Attributes.Add("OnClick", "javascript:return ColapsePane();");//

        this.btnOTC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnLVC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTRC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnFTC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnJSC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnMVC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTXC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnBFC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnADC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnSWC.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnGPC.Attributes.Add("OnClick", "javascript:return ColapsePane();");//

        this.btnOTN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnLVN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTRN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnFTN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnJSN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnMVN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnTXN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnBFN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnADN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnSWN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        this.btnGPN.Attributes.Add("OnClick", "javascript:return ColapsePane();");//

        //this.btnMOTN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMLVN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMTRN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMFTN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMJSN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMMVN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMTXN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMBFN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMADN.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMSWN.Attributes.Add("OnClick", "javascript:return ColapsePane();");//

        //this.btnMOTA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMLVA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMTRA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMFTA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMJSA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMMVA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMTXA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMBFA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMADA.Attributes.Add("OnClick", "javascript:return ColapsePane();");
        //this.btnMSWA.Attributes.Add("OnClick", "javascript:return ColapsePane();");//

        this.btnShowDetails.Attributes.Add("OnClick", string.Format("javascript:return lookupCheckList('{0}');", (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()));
        this.btnShowWaitListDetails.Attributes.Add("OnClick", string.Format("javascript:return lookupWaitList('{0}');", (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()));
        this.btnShowNextLevelDetails.Attributes.Add("OnClick", string.Format("javascript:return lookupNextLevel('{0}');", (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()));
        this.btnApproverOverride.Attributes.Add("OnClick", string.Format("javascript:return lookupApproverOverride('{0}');", Session["userLogged"].ToString()));
        this.txtApproverOverride.Attributes.Add("readonly", "true");
        this.txtApproverOverrideDesc.Attributes.Add("readonly", "true");
        hfUserCode.Value = (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString();
        this.btnEndorseApprove.Attributes.Add("OnClick", "javascript:setReadOnly('ctl00_ContentPlaceHolder1_ASPxSplitter1_btnEndorseApprove')");
        
    }
    protected string GetCaptionText(GridViewGroupRowTemplateContainer container)
    {
        string captionText = !string.IsNullOrEmpty(container.Column.Caption) ? container.Column.Caption : container.Column.FieldName;
        return string.Format("{0} : {1} {2}", captionText, container.GroupText, container.SummaryText);
    }
    
    private int groupCount
    {
        set
        {
            this.ViewState["groupCount"] = value;
        }
        get
        {
            return this.ViewState["groupCount"] == null
                    ? 0
                    : Int32.Parse(this.ViewState["groupCount"].ToString().Trim());
        }
    }
    
    #region Counters
    private void InitialzieCounters()
    {
        InitializeWaitListCounters();
        InitializeCheckListCounters();
        InitializeNextLevelCounters();
        InitializeNewPendingCounters();
        InitializeApprovedCounters();
        InitializeDisapprovedCounters();
    }

    #region ForApprovalWaitlistNext Counters
    private void InitializeWaitListCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
        sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
								  AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Eot_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Eot_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Eot_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
								  AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Elt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Elt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Elt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
								  AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Trm_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Trm_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Trm_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
								  AND Flx_FlexDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Flx_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Flx_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Flx_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 INNER JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
								  AND Jsh_JobSplitDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ((Jsh_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR  (Jsh_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR  (Jsh_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' ))
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
								  AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Mve_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Mve_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Mve_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
								  AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Pit_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Pit_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Pit_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
								  AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (But_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (But_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (But_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
								  AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Amt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Amt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Amt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                                --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                                 begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Swt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Swt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'STRAIGHTWK'
								  AND Swt_FromDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Swt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Swt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Swt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )
                                end

                                --Table 10: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                                 begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Egp_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Egp_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'GATEPASS'
								  AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Egp_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Egp_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Egp_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )
                                end
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()), CommandType.Text);
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
            DataSet dsApproverOverride = null;
            bool isApproverOverride = false;
            if (txtApproverOverride.Text.Trim() != string.Empty)
            {
                dsApproverOverride = GetApproverOverride(Session["userLogged"].ToString(), txtApproverOverride.Text);
                isApproverOverride = true;
            }

            btnOTW.Text = GetButtonValue(ds, 0, "OVERTIME", isApproverOverride, dsApproverOverride);
            btnLVW.Text = GetButtonValue(ds, 1, "LEAVE", isApproverOverride, dsApproverOverride);
            btnTRW.Text = GetButtonValue(ds, 2, "TIMEMOD", isApproverOverride, dsApproverOverride);
            btnFTW.Text = GetButtonValue(ds, 3, "FLEXTIME", isApproverOverride, dsApproverOverride);
            btnJSW.Text = GetButtonValue(ds, 4, "JOBMOD", isApproverOverride, dsApproverOverride);
            btnMVW.Text = GetButtonValue(ds, 5, "MOVEMENT", isApproverOverride, dsApproverOverride);
            btnTXW.Text = GetButtonValue(ds, 6, "TAXMVMNT", isApproverOverride, dsApproverOverride);
            btnBFW.Text = GetButtonValue(ds, 7, "BNEFICIARY", isApproverOverride, dsApproverOverride);
            btnADW.Text = GetButtonValue(ds, 8, "ADDRESS", isApproverOverride, dsApproverOverride);
            btnSWW.Text = GetButtonValue(ds, 9, "STRAIGHTWK", isApproverOverride, dsApproverOverride);
            btnGPW.Text = GetButtonValue(ds, 10, "GATEPASS", isApproverOverride, dsApproverOverride);
            
            SetVisibleOnZeroTextButton(btnOTW);
            SetVisibleOnZeroTextButton(btnLVW);
            SetVisibleOnZeroTextButton(btnTRW);
            SetVisibleOnZeroTextButton(btnFTW);
            SetVisibleOnZeroTextButton(btnJSW);
            SetVisibleOnZeroTextButton(btnMVW);
            SetVisibleOnZeroTextButton(btnTXW);
            SetVisibleOnZeroTextButton(btnBFW);
            SetVisibleOnZeroTextButton(btnADW);
            SetVisibleOnZeroTextButton(btnSWW);
            SetVisibleOnZeroTextButton(btnGPW);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnOTW);
            SetVisibleOnZeroTextButton(btnLVW);
            SetVisibleOnZeroTextButton(btnTRW);
            SetVisibleOnZeroTextButton(btnFTW);
            SetVisibleOnZeroTextButton(btnJSW);
            SetVisibleOnZeroTextButton(btnMVW);
            SetVisibleOnZeroTextButton(btnTXW);
            SetVisibleOnZeroTextButton(btnBFW);
            SetVisibleOnZeroTextButton(btnADW);
            SetVisibleOnZeroTextButton(btnSWW);
            SetVisibleOnZeroTextButton(btnGPW);
        }
    }

    private void InitializeCheckListCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
        sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
								  AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Eot_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Eot_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Eot_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
								  AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Elt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Elt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Elt_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
								  AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Trm_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Trm_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Trm_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
								  AND Flx_FlexDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Flx_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Flx_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Flx_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 INNER JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
								  AND Jsh_JobSplitDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (  ( Jsh_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                      OR ( Jsh_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                      OR ( Jsh_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                      )
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
								  AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Mve_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Mve_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Mve_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
								  AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Pit_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Pit_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Pit_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
								  AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( But_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( But_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( But_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
								  AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Amt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Amt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Amt_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                                --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                               begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Swt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Swt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'STRAIGHTWK'
								  AND Swt_FromDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Swt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Swt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Swt_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                end

                                --Table 11: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                               begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Egp_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Egp_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'GATEPASS'
								  AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Egp_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Egp_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Egp_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                end
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()), CommandType.Text);
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
            DataSet dsApproverOverride = null;
            bool isApproverOverride = false;
            if (txtApproverOverride.Text.Trim() != string.Empty)
            {
                dsApproverOverride = GetApproverOverride(Session["userLogged"].ToString(), txtApproverOverride.Text);
                isApproverOverride = true;
            }

            btnOTC.Text = GetButtonValue(ds, 0, "OVERTIME", isApproverOverride, dsApproverOverride);
            btnLVC.Text = GetButtonValue(ds, 1, "LEAVE", isApproverOverride, dsApproverOverride);
            btnTRC.Text = GetButtonValue(ds, 2, "TIMEMOD", isApproverOverride, dsApproverOverride);
            btnFTC.Text = GetButtonValue(ds, 3, "FLEXTIME", isApproverOverride, dsApproverOverride);
            btnJSC.Text = GetButtonValue(ds, 4, "JOBMOD", isApproverOverride, dsApproverOverride);
            btnMVC.Text = GetButtonValue(ds, 5, "MOVEMENT", isApproverOverride, dsApproverOverride);
            btnTXC.Text = GetButtonValue(ds, 6, "TAXMVMNT", isApproverOverride, dsApproverOverride);
            btnBFC.Text = GetButtonValue(ds, 7, "BNEFICIARY", isApproverOverride, dsApproverOverride);
            btnADC.Text = GetButtonValue(ds, 8, "ADDRESS", isApproverOverride, dsApproverOverride);
            btnSWC.Text = GetButtonValue(ds, 9, "STRAIGHTWK", isApproverOverride, dsApproverOverride);
            btnGPC.Text = GetButtonValue(ds, 10, "GATEPASS", isApproverOverride, dsApproverOverride);
            
            SetVisibleOnZeroTextButton(btnOTC);
            SetVisibleOnZeroTextButton(btnLVC);
            SetVisibleOnZeroTextButton(btnTRC);
            SetVisibleOnZeroTextButton(btnFTC);
            SetVisibleOnZeroTextButton(btnJSC);
            SetVisibleOnZeroTextButton(btnMVC);
            SetVisibleOnZeroTextButton(btnTXC);
            SetVisibleOnZeroTextButton(btnBFC);
            SetVisibleOnZeroTextButton(btnADC);
            SetVisibleOnZeroTextButton(btnSWC);
            SetVisibleOnZeroTextButton(btnGPC);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnOTC);
            SetVisibleOnZeroTextButton(btnLVC);
            SetVisibleOnZeroTextButton(btnTRC);
            SetVisibleOnZeroTextButton(btnFTC);
            SetVisibleOnZeroTextButton(btnJSC);
            SetVisibleOnZeroTextButton(btnMVC);
            SetVisibleOnZeroTextButton(btnTXC);
            SetVisibleOnZeroTextButton(btnBFC);
            SetVisibleOnZeroTextButton(btnADC);
            SetVisibleOnZeroTextButton(btnSWC);
            SetVisibleOnZeroTextButton(btnGPC);
        }
    }

    private void InitializeNextLevelCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
         sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
								  AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Eot_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Eot_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
								  AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Elt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2) 
                                   OR ( Elt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
								  AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Trm_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Trm_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
								  AND Flx_FlexDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Flx_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Flx_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 INNER JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
								  AND Jsh_JobSplitDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (( Jsh_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR  ( Jsh_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver ))
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
								  AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Mve_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Mve_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
								  AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Pit_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Pit_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
								  AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( But_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( But_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
								  AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Amt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Amt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                                 begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Swt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Swt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'STRAIGHTWK'
								  AND Swt_FromDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Swt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Swt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )
                                end

                               --Table 11: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                                 begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                 INNER JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Egp_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Egp_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'GATEPASS'
								  AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                 INNER JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Egp_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Egp_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )
                                end
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString()), CommandType.Text);
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
            DataSet dsApproverOverride = null;
            bool isApproverOverride = false;
            if (txtApproverOverride.Text.Trim() != string.Empty)
            {
                dsApproverOverride = GetApproverOverride(Session["userLogged"].ToString(), txtApproverOverride.Text);
                isApproverOverride = true;
            }

            btnOTN.Text = GetButtonValue(ds, 0, "OVERTIME", isApproverOverride, dsApproverOverride);
            btnLVN.Text = GetButtonValue(ds, 1, "LEAVE", isApproverOverride, dsApproverOverride);
            btnTRN.Text = GetButtonValue(ds, 2, "TIMEMOD", isApproverOverride, dsApproverOverride);
            btnFTN.Text = GetButtonValue(ds, 3, "FLEXTIME", isApproverOverride, dsApproverOverride);
            btnJSN.Text = GetButtonValue(ds, 4, "JOBMOD", isApproverOverride, dsApproverOverride);
            btnMVN.Text = GetButtonValue(ds, 5, "MOVEMENT", isApproverOverride, dsApproverOverride);
            btnTXN.Text = GetButtonValue(ds, 6, "TAXMVMNT", isApproverOverride, dsApproverOverride);
            btnBFN.Text = GetButtonValue(ds, 7, "BNEFICIARY", isApproverOverride, dsApproverOverride);
            btnADN.Text = GetButtonValue(ds, 8, "ADDRESS", isApproverOverride, dsApproverOverride);
            btnSWN.Text = GetButtonValue(ds, 9, "STRAIGHTWK", isApproverOverride, dsApproverOverride);
            btnGPN.Text = GetButtonValue(ds, 10, "GATEPASS", isApproverOverride, dsApproverOverride);

            SetVisibleOnZeroTextButton(btnOTN);
            SetVisibleOnZeroTextButton(btnLVN);
            SetVisibleOnZeroTextButton(btnTRN);
            SetVisibleOnZeroTextButton(btnFTN);
            SetVisibleOnZeroTextButton(btnJSN);
            SetVisibleOnZeroTextButton(btnMVN);
            SetVisibleOnZeroTextButton(btnTXN);
            SetVisibleOnZeroTextButton(btnBFN);
            SetVisibleOnZeroTextButton(btnADN);
            SetVisibleOnZeroTextButton(btnSWN);
            SetVisibleOnZeroTextButton(btnGPN);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnOTN);
            SetVisibleOnZeroTextButton(btnLVN);
            SetVisibleOnZeroTextButton(btnTRN);
            SetVisibleOnZeroTextButton(btnFTN);
            SetVisibleOnZeroTextButton(btnJSN);
            SetVisibleOnZeroTextButton(btnMVN);
            SetVisibleOnZeroTextButton(btnTXN);
            SetVisibleOnZeroTextButton(btnBFN);
            SetVisibleOnZeroTextButton(btnADN);
            SetVisibleOnZeroTextButton(btnSWN);
            SetVisibleOnZeroTextButton(btnGPN);
        }
    }
    public string GetButtonValue(DataSet ds, int tableNum, string TransactionID, bool isApproverOverride,DataSet dsApproverOverride)
    {
        string num = "0";
        try
        {
            if (!isApproverOverride ||
                (dsApproverOverride.Tables[0].Select(string.Format("Aro_TransactionID = '{0}'", TransactionID)).Length > 0))
                num = ds.Tables[tableNum].Rows[0][0].ToString();
        }
        catch { num = "0"; }

        return num;
    }
    #endregion

    #region Own Transactions
    private void InitializeNewPendingCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
        sqlCounters = @"--Table 1: Overtime
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertime
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1})
                                         UNION
                                        SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertimeHist
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1}) ) AS TEMP

                               --Table 2: LEAVE
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailment
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}'
                                         UNION 
                                        SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailmentHist
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}' ) AS TEMP

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                WHERE Trm_EmployeeId = '{0}'
                                  AND Trm_Status IN ({1})

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                WHERE Flx_EmployeeId = '{0}'
                                  AND Flx_Status IN ({1})

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                WHERE Jsh_EmployeeId = '{0}'
                                  AND Jsh_Status IN ({1})
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                WHERE Mve_EmployeeId = '{0}'
                                  AND Mve_Status IN ({1})
 
                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                WHERE Pit_EmployeeId = '{0}'
                                  AND Pit_MoveType = 'P1'
                                  AND Pit_Status IN ({1}) 
 
                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                WHERE But_EmployeeId = '{0}'
                                  AND But_Status IN ({1}) 

                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                WHERE Amt_EmployeeId = '{0}'
                                  AND Amt_Status IN ({1}) 
                    
                                --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                                 begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                WHERE Swt_EmployeeId = '{0}'
                                  AND Swt_Status IN ({1}) 
                                end

                                --Table 11: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                                 begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                WHERE Egp_EmployeeId = '{0}'
                                  AND Egp_Status IN ({1}) 
                                end
                                ";
        #endregion
        string statusNP = "'1','3','5','7','N'";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString(), statusNP), CommandType.Text);
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
            try { btnMOTN.Text = ds.Tables[0].Rows[0][0].ToString(); }
            catch { btnMOTN.Text = "0"; }

            try { btnMLVN.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { btnMLVN.Text = "0"; }

            try { btnMTRN.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { btnMTRN.Text = "0"; }

            try { btnMFTN.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { btnMFTN.Text = "0"; }

            try { btnMJSN.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { btnMJSN.Text = "0"; }

            try { btnMMVN.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { btnMMVN.Text = "0"; }

            try { btnMTXN.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { btnMTXN.Text = "0"; }

            try { btnMBFN.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { btnMBFN.Text = "0"; }

            try { btnMADN.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { btnMADN.Text = "0"; }

            try { btnMSWN.Text = ds.Tables[9].Rows[0][0].ToString(); }
            catch { btnMSWN.Text = "0"; }

            try { btnMGPN.Text = ds.Tables[10].Rows[0][0].ToString(); }
            catch { btnMGPN.Text = "0"; }

            SetVisibleOnZeroTextButton(btnMOTN);
            SetVisibleOnZeroTextButton(btnMLVN);
            SetVisibleOnZeroTextButton(btnMTRN);
            SetVisibleOnZeroTextButton(btnMFTN);
            SetVisibleOnZeroTextButton(btnMJSN);
            SetVisibleOnZeroTextButton(btnMMVN);
            SetVisibleOnZeroTextButton(btnMTXN);
            SetVisibleOnZeroTextButton(btnMBFN);
            SetVisibleOnZeroTextButton(btnMADN);
            SetVisibleOnZeroTextButton(btnMSWN);
            SetVisibleOnZeroTextButton(btnMGPN);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnMOTN);
            SetVisibleOnZeroTextButton(btnMLVN);
            SetVisibleOnZeroTextButton(btnMTRN);
            SetVisibleOnZeroTextButton(btnMFTN);
            SetVisibleOnZeroTextButton(btnMJSN);
            SetVisibleOnZeroTextButton(btnMMVN);
            SetVisibleOnZeroTextButton(btnMTXN);
            SetVisibleOnZeroTextButton(btnMBFN);
            SetVisibleOnZeroTextButton(btnMADN);
            SetVisibleOnZeroTextButton(btnMSWN);
            SetVisibleOnZeroTextButton(btnMGPN);
        }
    }

    private void InitializeApprovedCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
        sqlCounters = @"--Table 1: Overtime
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertime
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1})
                                         UNION
                                        SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertimeHist
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1}) ) AS TEMP

                               --Table 2: LEAVE
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailment
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}'
                                         UNION 
                                        SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailmentHist
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}' ) AS TEMP

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                WHERE Trm_EmployeeId = '{0}'
                                  AND Trm_Status IN ({1})

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                WHERE Flx_EmployeeId = '{0}'
                                  AND Flx_Status IN ({1})

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                WHERE Jsh_EmployeeId = '{0}'
                                  AND Jsh_Status IN ({1})
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                WHERE Mve_EmployeeId = '{0}'
                                  AND Mve_Status IN ({1})
 
                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                WHERE Pit_EmployeeId = '{0}'
                                  AND Pit_MoveType = 'P1'
                                  AND Pit_Status IN ({1}) 
 
                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                WHERE But_EmployeeId = '{0}'
                                  AND But_Status IN ({1}) 

                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                WHERE Amt_EmployeeId = '{0}'
                                  AND Amt_Status IN ({1}) 
                    
                                --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                                 begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                WHERE Swt_EmployeeId = '{0}'
                                  AND Swt_Status IN ({1}) 
                                end

                                --Table 11: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                                 begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                WHERE Egp_EmployeeId = '{0}'
                                  AND Egp_Status IN ({1}) 
                                end
                                ";
        #endregion
        string statusAD = "'9'";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString(), statusAD), CommandType.Text);
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
            try { btnMOTA.Text = ds.Tables[0].Rows[0][0].ToString(); }
            catch { btnMOTA.Text = "0"; }

            try { btnMLVA.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { btnMLVA.Text = "0"; }

            try { btnMTRA.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { btnMTRA.Text = "0"; }

            try { btnMFTA.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { btnMFTA.Text = "0"; }

            try { btnMJSA.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { btnMJSA.Text = "0"; }

            try { btnMMVA.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { btnMMVA.Text = "0"; }

            try { btnMTXA.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { btnMTXA.Text = "0"; }

            try { btnMBFA.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { btnMBFA.Text = "0"; }

            try { btnMADA.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { btnMADA.Text = "0"; }

            try { btnMSWA.Text = ds.Tables[9].Rows[0][0].ToString(); }
            catch { btnMSWA.Text = "0"; }

            try { btnMGPA.Text = ds.Tables[10].Rows[0][0].ToString(); }
            catch { btnMGPA.Text = "0"; }

            SetVisibleOnZeroTextButton(btnMOTA);
            SetVisibleOnZeroTextButton(btnMLVA);
            SetVisibleOnZeroTextButton(btnMTRA);
            SetVisibleOnZeroTextButton(btnMFTA);
            SetVisibleOnZeroTextButton(btnMJSA);
            SetVisibleOnZeroTextButton(btnMMVA);
            SetVisibleOnZeroTextButton(btnMTXA);
            SetVisibleOnZeroTextButton(btnMBFA);
            SetVisibleOnZeroTextButton(btnMADA);
            SetVisibleOnZeroTextButton(btnMSWA);
            SetVisibleOnZeroTextButton(btnMGPA);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnMOTA);
            SetVisibleOnZeroTextButton(btnMLVA);
            SetVisibleOnZeroTextButton(btnMTRA);
            SetVisibleOnZeroTextButton(btnMFTA);
            SetVisibleOnZeroTextButton(btnMJSA);
            SetVisibleOnZeroTextButton(btnMMVA);
            SetVisibleOnZeroTextButton(btnMTXA);
            SetVisibleOnZeroTextButton(btnMBFA);
            SetVisibleOnZeroTextButton(btnMADA);
            SetVisibleOnZeroTextButton(btnMSWA);
            SetVisibleOnZeroTextButton(btnMGPA);
        }
    }

    private void InitializeDisapprovedCounters()
    {
        string sqlCounters = string.Empty;
        #region SQL Counters
        sqlCounters = @"--Table 1: Overtime
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertime
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1})
                                         UNION
                                        SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertimeHist
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1}) ) AS TEMP

                               --Table 2: LEAVE
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailment
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}'
                                         UNION 
                                        SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailmentHist
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}' ) AS TEMP

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                WHERE Trm_EmployeeId = '{0}'
                                  AND Trm_Status IN ({1})

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                WHERE Flx_EmployeeId = '{0}'
                                  AND Flx_Status IN ({1})

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                WHERE Jsh_EmployeeId = '{0}'
                                  AND Jsh_Status IN ({1})
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                WHERE Mve_EmployeeId = '{0}'
                                  AND Mve_Status IN ({1})
 
                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                WHERE Pit_EmployeeId = '{0}'
                                  AND Pit_MoveType = 'P1'
                                  AND Pit_Status IN ({1}) 
 
                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                WHERE But_EmployeeId = '{0}'
                                  AND But_Status IN ({1}) 

                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                WHERE Amt_EmployeeId = '{0}'
                                  AND Amt_Status IN ({1}) 
                    
                                --Table 10: STRAIGHT WORK
                                IF object_id('T_EmployeeStraightWork') is not null
                                 begin
                               SELECT ISNULL(COUNT(Swt_Status),0)
                                 FROM T_EmployeeStraightWork
                                WHERE Swt_EmployeeId = '{0}'
                                  AND Swt_Status IN ({1}) 
                                end

                                --Table 11: GATE PASS
                                IF object_id('E_EmployeeGatePass') is not null
                                 begin
                               SELECT ISNULL(COUNT(Egp_Status),0)
                                 FROM E_EmployeeGatePass
                                WHERE Egp_EmployeeId = '{0}'
                                  AND Egp_Status IN ({1}) 
                                end
                                ";
        #endregion
        string statusAD = "'0','2','4','6','8'";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString(), statusAD), CommandType.Text);
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
            try { btnMOTD.Text = ds.Tables[0].Rows[0][0].ToString(); }
            catch { btnMOTD.Text = "0"; }

            try { btnMLVD.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { btnMLVD.Text = "0"; }

            try { btnMTRD.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { btnMTRD.Text = "0"; }

            try { btnMFTD.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { btnMFTD.Text = "0"; }

            try { btnMJSD.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { btnMJSD.Text = "0"; }

            try { btnMMVD.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { btnMMVD.Text = "0"; }

            try { btnMTXD.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { btnMTXD.Text = "0"; }

            try { btnMBFD.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { btnMBFD.Text = "0"; }

            try { btnMADD.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { btnMADD.Text = "0"; }

            try { btnMSWD.Text = ds.Tables[9].Rows[0][0].ToString(); }
            catch { btnMSWD.Text = "0"; }

            try { btnMGPD.Text = ds.Tables[10].Rows[0][0].ToString(); }
            catch { btnMGPD.Text = "0"; }

            SetVisibleOnZeroTextButton(btnMOTD);
            SetVisibleOnZeroTextButton(btnMLVD);
            SetVisibleOnZeroTextButton(btnMTRD);
            SetVisibleOnZeroTextButton(btnMFTD);
            SetVisibleOnZeroTextButton(btnMJSD);
            SetVisibleOnZeroTextButton(btnMMVD);
            SetVisibleOnZeroTextButton(btnMTXD);
            SetVisibleOnZeroTextButton(btnMBFD);
            SetVisibleOnZeroTextButton(btnMADD);
            SetVisibleOnZeroTextButton(btnMSWD);
            SetVisibleOnZeroTextButton(btnMGPD);
        }
        else
        {
            SetVisibleOnZeroTextButton(btnMOTD);
            SetVisibleOnZeroTextButton(btnMLVD);
            SetVisibleOnZeroTextButton(btnMTRD);
            SetVisibleOnZeroTextButton(btnMFTD);
            SetVisibleOnZeroTextButton(btnMJSD);
            SetVisibleOnZeroTextButton(btnMMVD);
            SetVisibleOnZeroTextButton(btnMTXD);
            SetVisibleOnZeroTextButton(btnMBFD);
            SetVisibleOnZeroTextButton(btnMADD);
            SetVisibleOnZeroTextButton(btnMSWD);
            SetVisibleOnZeroTextButton(btnMGPD);
        }
    } 
    #endregion
    #endregion

    private void SetVisibleOnZeroTextButton(ASPxButton btn)
    {
        try
        {
            if (Convert.ToInt32(btn.Text.Trim()) > 0)
            {
                btn.Visible = true;
            }
            else
            {
                btn.Visible = false;
            }
        }
        catch
        {
            btn.Visible = false;
        }
    }

    protected void WaitList_Click(object sender, EventArgs e)
    {
        this.SetQueryFromButtons(((ASPxButton)sender).ID.ToString().Trim(), "1");
    }

    protected void Refresh_Click(object sender, EventArgs e)
    {
      
    }
    
    protected void CheckList_Click(object sender, EventArgs e)
    {
        this.SetQueryFromButtons(((ASPxButton)sender).ID.ToString().Trim(), "2");
    }

    protected void NextList_Click(object sender, EventArgs e)
    {
        this.SetQueryFromButtons(((ASPxButton)sender).ID.ToString().Trim(), "3");
    }

    protected void NewAndPending_Click(object sender, EventArgs e)
    {
        this.SetQueryFromButtons(((ASPxButton)sender).ID.ToString().Trim(), "4");
    }

    protected void ApprovedDisapproved_Click(object sender, EventArgs e)
    {
        this.SetQueryFromButtons(((ASPxButton)sender).ID.ToString().Trim(), "5");
    }

    protected void EmailTransaction_Click(object sender, EventArgs e)
    { 

    }

    protected void rbtnChoices_SelectedIndexChanged(object sender, EventArgs e)
    { 

    }

    private int countCutOff
    {
        set
        {
            this.ViewState["countCutOff"] = value;
        }
        get
        {
            return this.ViewState["countCutOff"] == null
                    ? 0
                    : Int32.Parse(this.ViewState["countCutOff"].ToString().Trim());
        }
    }
    private int countAffected
    {
        set
        {
            this.ViewState["countAffected"] = value;
        }
        get
        {
            return this.ViewState["countAffected"] == null
                    ? 0
                    : Int32.Parse(this.ViewState["countAffected"].ToString().Trim());
        }
    }
    private string transactionSystemID
    {
        set
        {
            this.ViewState["transactionSystemID"] = value;
        }
        get
        {
            return this.ViewState["transactionSystemID"] == null
                    ? string.Empty
                    : this.ViewState["transactionSystemID"].ToString().Trim();
        }
    }

    protected void EndorseApprove_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.EndorseApproval);
    }

    private void ProcessTransaction(WFProcess wfProcess)
    {
        if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            countCutOff = 0;
            countAffected = 0;
            dtErrors.Columns.Add("Control No");
            dtErrors.Columns.Add("Exception");
            List<object> value = agvData.GetSelectedFieldValues("Key");
            string query = string.Empty;
            DataSet ds = null;
            int CountSelectedRows = 0;
            string strControlNumbers = "";

            if ((wfProcess == WFProcess.Disapprove && string.IsNullOrEmpty(txtDisAprvRemarks.Text))
                || (wfProcess == WFProcess.Return && string.IsNullOrEmpty(txtRemarksReturn.Text)))
            {
                MessageBox.Show("Remarks are required.");
            }
            else if (value.Count > 0)
            {
                foreach (string key in value)
                {
                    if (key != null)
                    {
                        CountSelectedRows++;
                        strControlNumbers += key + "|";
                    }
                }
                if (CountSelectedRows < 1)
                {
                    MessageBox.Show("Please select items to proceed.");
                }
                else
                {
                    query = CKBL.getControlNoQueryBasedOnTransactionType(hfTransactionType.Value.ToString(), "", "2", false, Session["userLogged"].ToString(), strControlNumbers);
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            ds = dal.ExecuteDataSet(query);
                        }
                        catch { }
                        finally { dal.CloseDB(); }
                    }

                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        string controNumber = "";
                        string CurrentStatus = "";
                        string NextStatus = "";
                        string strForEndorseToChecker2 = "";
                        string strForEndorseToApprover = "";
                        string strForApproval = "";
                        string strForDisapproval = "";
                        string strForReturn = "";
                        for (int trav = 0; trav < ds.Tables[0].Rows.Count; trav++)
                        {
                            controNumber = ds.Tables[0].Rows[trav]["Control No"].ToString();
                            CurrentStatus = ds.Tables[0].Rows[trav]["Status"].ToString();
                            NextStatus = ds.Tables[0].Rows[trav]["Next Status"].ToString();
                            if (wfProcess == WFProcess.EndorseApproval)
                            {
                                if (NextStatus != null && NextStatus.Trim() != "" && NextStatus.Trim() != "10")
                                {
                                    if (NextStatus == "5")
                                        strForEndorseToChecker2 += controNumber + ",";
                                    else if (NextStatus == "7")
                                        strForEndorseToApprover += controNumber + ",";
                                    else if (NextStatus == "9")
                                        strForApproval += controNumber + ",";
                                }
                            }
                            else if (wfProcess == WFProcess.Disapprove)
                            {
                                if (CurrentStatus != null && CurrentStatus.Trim() != "" && CurrentStatus.Trim() != "10")
                                {
                                    strForDisapproval += controNumber + ",";
                                }
                            }
                            else if (wfProcess == WFProcess.Return)
                            {
                                if (CurrentStatus != null && CurrentStatus.Trim() != "" && CurrentStatus.Trim() != "10")
                                {
                                    strForReturn += controNumber + ",";
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
                                                                                , hfTransactionType.Value.ToString()
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
                                                                                , hfTransactionType.Value.ToString()
                                                                                , "7"
                                                                                , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                }
                                if (strForApproval != "")
                                {
                                    #region Get Stored Procedure Name
                                    string strStoredProcName = "";
                                    switch (hfTransactionType.Value.ToString().ToUpper())
                                    {
                                        case "OT":
                                            strStoredProcName = "ApproveOvertime";
                                            transactionSystemID = "OVERTIME";
                                            break;
                                        case "LV":
                                            strStoredProcName = "ApproveLeave";
                                            transactionSystemID = "LEAVE";
                                            break;
                                        case "TR":
                                            strStoredProcName = "ApproveTimeRecMod";
                                            transactionSystemID = "TIMEKEEP";
                                            break;
                                        //case "FT":
                                        //    strStoredProcName = "ApproveFlexTime";
                                        //    transactionSystemID = "TIMEKEEP";
                                        //    break;
                                        //case "JS":
                                        //    strStoredProcName = "ApproveJobSplit";
                                        //    transactionSystemID = "TIMEKEEP";
                                        //    break;
                                        case "MV":
                                            strStoredProcName = "ApproveMovement";
                                            transactionSystemID = "TIMEKEEP";
                                            break;
                                        case "TX":
                                            strStoredProcName = "ApproveTaxCodeCivilStatusUpdate";
                                            transactionSystemID = "PAYROLL";
                                            break;
                                        case "BF":
                                            strStoredProcName = "ApproveBeneficiaryUpdate";
                                            transactionSystemID = "PAYROLL";
                                            break;
                                        case "AD":
                                            strStoredProcName = "ApproveAddressMovement";
                                            transactionSystemID = "PAYROLL";
                                            break;
                                       
                                        //case "SW":
                                        //    strStoredProcName = "ApproveStraightWork";
                                        //    transactionSystemID = "TIMEKEEP";
                                        //    break;
                                    }
                                    #endregion

                                    if (!hfTransactionType.Value.ToString().ToUpper().Equals("GP"))
                                    {
                                        strForApproval = strForApproval.Substring(0, strForApproval.Length - 1);
                                        strForApprovalQuery = string.Format("EXEC {0} '{1}', '{2}', '{3}' "
                                                                                    , strStoredProcName
                                                                                    , strForApproval
                                                                                    , Session["userLogged"].ToString()
                                                                                    , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                    }
                                    else
                                    {
                                        strForApproval = strForApproval.Substring(0, strForApproval.Length - 1);
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
                                                                                , txtDisAprvRemarks.Text.Trim().ToUpper()
                                                                                , hfTransactionType.Value.ToString()
                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                }
                                if (strForReturn != "")
                                {
                                    strForReturn = strForReturn.Substring(0, strForReturn.Length - 1);
                                    strForReturnQuery = string.Format("EXEC ReturnWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}' "
                                                                                , strForReturn
                                                                                , Session["userLogged"].ToString()
                                                                                , txtRemarksReturn.Text.Trim().ToUpper()
                                                                                , hfTransactionType.Value.ToString()
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
                                    ConsolidateResults(dsResult.Tables[i]);
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
                            cutOffMsg = "\n\n";
                            cutOffMsg = cutOffMsg + countCutOff + " row(s) affected on CUT-OFF.\n";
                            cutOffMsg = cutOffMsg + CommonMethods.GetErrorMessageForCutOff(transactionSystemID);
                        }
                        agvData.DataBind();

                        if (wfProcess == WFProcess.EndorseApproval)
                            InitialzieCounters();
                        else if (wfProcess == WFProcess.Disapprove || wfProcess == WFProcess.Return)
                            InitializeCheckListCounters();

                        if (dtErrors.Rows.Count > 0)
                        {
                            apcResultsGrid.ShowOnPageLoad = true;
                            lblProcessResults.Text = "Show Errors: " + dtErrors.Rows.Count.ToString();
                            agvError.DataSource = dtErrors;
                            agvError.DataBind();
                        }
                        else
                        {
                            apcResultsGrid.ShowOnPageLoad = false;
                            if (wfProcess == WFProcess.Disapprove)
                            {
                                acpProceed.ShowOnPageLoad = false;
                                txtDisAprvRemarks.Text = "";
                            }
                            else if (wfProcess == WFProcess.Return)
                            {
                                apcReturn.ShowOnPageLoad = false;
                                txtRemarksReturn.Text = "";
                            }
                        }
                        MessageBox.Show(countAffected + " row(s) affected." + cutOffMsg);
                    }

                }
            }
            else
            {
                MessageBox.Show("Please select items to proceed.");
            }
        }
        else
        {
            MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
        }
        
    }

    private void ConsolidateResults(DataTable dtResult)
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
          && controlNo.Trim().Length.Equals(12) 
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

    protected void Disapprove_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Disapprove);
    }

    protected void Return_Click(object sender, EventArgs e)
    {
        ProcessTransaction(WFProcess.Return);
    }

    private void SetQueryFromButtons(string btnID, string checkLevel)
    {
        string query = string.Empty;
        this.lblDoubleClick.Visible = false;
        CheckLevel = checkLevel;
        switch (checkLevel)
        {
            case "1":
                this.TransactionType = "Wait List";
                break;
            case "2":
                this.TransactionType = "Check List";
                break;
            case "3":
                this.TransactionType = "Next Level / For Approval List";
                break;
            case "4":
                this.TransactionType = "New and Pending List";
                this.lblDoubleClick.Visible = true;
                break;
            case "5":
                this.TransactionType = "Approved and Disapproved List";
                break;
        }
        string userID = (txtApproverOverride.Text.Trim() != string.Empty) ? txtApproverOverride.Text.Trim() : Session["userLogged"].ToString();
        if (Convert.ToBoolean(Resources.Resource.ALLOWAPPROVEDIRECT))
        {
            switch (btnID)
            {
                case "btnOTW": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; hfTransactionType.Value = "OT"; break;
                case "btnLVW": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; hfTransactionType.Value = "LV"; break;
                case "btnTRW": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; hfTransactionType.Value = "TR"; break;
                case "btnFTW": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; hfTransactionType.Value = "FT"; break;
                case "btnJSW": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; hfTransactionType.Value = "JS"; break;
                case "btnMVW": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; hfTransactionType.Value = "MV"; break;
                case "btnTXW": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; hfTransactionType.Value = "TX"; break;
                case "btnBFW": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; hfTransactionType.Value = "BF"; break;
                case "btnADW": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; hfTransactionType.Value = "AD"; break;
                case "btnSWW": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; hfTransactionType.Value = "SW"; break;
                case "btnGPW": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; hfTransactionType.Value = "GP"; break;

                case "btnOTC": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; hfTransactionType.Value = "OT"; break;
                case "btnLVC": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; hfTransactionType.Value = "LV"; break;
                case "btnTRC": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; hfTransactionType.Value = "TR"; break;
                case "btnFTC": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; hfTransactionType.Value = "FT"; break;
                case "btnJSC": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; hfTransactionType.Value = "JS"; break;
                case "btnMVC": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; hfTransactionType.Value = "MV"; break;
                case "btnTXC": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; hfTransactionType.Value = "TX"; break;
                case "btnBFC": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; hfTransactionType.Value = "BF"; break;
                case "btnADC": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; hfTransactionType.Value = "AD"; break;
                case "btnSWC": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; hfTransactionType.Value = "SW"; break;
                case "btnGPC": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; hfTransactionType.Value = "GP"; break;

                case "btnOTN": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; hfTransactionType.Value = "OT"; break;
                case "btnLVN": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; hfTransactionType.Value = "LV"; break;
                case "btnTRN": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; hfTransactionType.Value = "TR"; break;
                case "btnFTN": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; hfTransactionType.Value = "FT"; break;
                case "btnJSN": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; hfTransactionType.Value = "JS"; break;
                case "btnMVN": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; hfTransactionType.Value = "MV"; break;
                case "btnTXN": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; hfTransactionType.Value = "TX"; break;
                case "btnBFN": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; hfTransactionType.Value = "BF"; break;
                case "btnADN": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; hfTransactionType.Value = "AD"; break;
                case "btnSWN": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; hfTransactionType.Value = "SW"; break;
                case "btnGPN": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; hfTransactionType.Value = "GP"; break;

                case "btnMOTN": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, OT.Visible, userID); TransactionID = "Overtime"; Transaction = "OVERTIME";hfTransactionType.Value = "OT"; break;
                case "btnMLVN": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, LV.Visible, userID); TransactionID = "Leave"; Transaction = "LEAVE";hfTransactionType.Value = "LV" ;break;
                case "btnMTRN": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, TR.Visible, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; hfTransactionType.Value = "TR";break;
                case "btnMFTN": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, FT.Visible, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; hfTransactionType.Value = "FT";break;
                case "btnMJSN": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, JS.Visible, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; hfTransactionType.Value = "JS";break;
                case "btnMMVN": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, MV.Visible, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; hfTransactionType.Value = "MV";break;
                case "btnMTXN": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, TX.Visible, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; hfTransactionType.Value = "TX";break;
                case "btnMBFN": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, BF.Visible, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY";hfTransactionType.Value = "BF"; break;
                case "btnMADN": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, AD.Visible, userID); TransactionID = "Address"; Transaction = "ADDRESS"; hfTransactionType.Value = "AD";break;
                case "btnMSWN": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, SW.Visible, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; hfTransactionType.Value = "SW"; break;
                case "btnMGPN": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, SW.Visible, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; hfTransactionType.Value = "GP"; break;

                case "btnMOTA": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, OT.Visible, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; hfTransactionType.Value = "OT"; break;
                case "btnMLVA": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, LV.Visible, userID); TransactionID = "Leave"; Transaction = "LEAVE";hfTransactionType.Value = "LV"; break;
                case "btnMTRA": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, TR.Visible, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION";hfTransactionType.Value = "TR"; break;
                case "btnMFTA": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, FT.Visible, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; hfTransactionType.Value = "FT";break;
                case "btnMJSA": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, JS.Visible, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; hfTransactionType.Value = "JS";break;
                case "btnMMVA": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, MV.Visible, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION";hfTransactionType.Value = "MV"; break;
                case "btnMTXA": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, TX.Visible, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; hfTransactionType.Value = "TX";break;
                case "btnMBFA": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, BF.Visible, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY";hfTransactionType.Value = "BF" ;break;
                case "btnMADA": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, AD.Visible, userID); TransactionID = "Address"; Transaction = "ADDRESS"; hfTransactionType.Value = "AD";break;
                case "btnMSWA": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, AD.Visible, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; hfTransactionType.Value = "SW"; break;
                case "btnMGPA": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, AD.Visible, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; hfTransactionType.Value = "GP"; break;
            }
            this.GetQuery = query;
            BindToGrid();
            SetupGrid();
        }
        else
        {
            switch (btnID)
            {
                case "btnOTW": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; break;
                case "btnLVW": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; break;
                case "btnTRW": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; break;
                case "btnFTW": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; break;
                case "btnJSW": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; break;
                case "btnMVW": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; break;
                case "btnTXW": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; break;
                case "btnBFW": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; break;
                case "btnADW": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; break;
                case "btnSWW": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; break;
                case "btnGPW": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; break;

                case "btnOTC": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; break;
                case "btnLVC": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; break;
                case "btnTRC": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; break;
                case "btnFTC": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; break;
                case "btnJSC": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; break;
                case "btnMVC": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; break;
                case "btnTXC": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; break;
                case "btnBFC": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; break;
                case "btnADC": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; break;
                case "btnSWC": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; break;
                case "btnGPC": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; break;

                case "btnOTN": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, false, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; break;
                case "btnLVN": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, false, userID); TransactionID = "Leave"; Transaction = "LEAVE"; break;
                case "btnTRN": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, false, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; break;
                case "btnFTN": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, false, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; break;
                case "btnJSN": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, false, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; break;
                case "btnMVN": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, false, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; break;
                case "btnTXN": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, false, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; break;
                case "btnBFN": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, false, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; break;
                case "btnADN": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, false, userID); TransactionID = "Address"; Transaction = "ADDRESS"; break;
                case "btnSWN": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, false, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; break;
                case "btnGPN": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, false, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; break;

                case "btnMOTN": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, OT.Visible, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; break;
                case "btnMLVN": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, LV.Visible, userID); TransactionID = "Leave"; Transaction = "LEAVE"; break;
                case "btnMTRN": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, TR.Visible, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; break;
                case "btnMFTN": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, FT.Visible, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; break;
                case "btnMJSN": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, JS.Visible, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; break;
                case "btnMMVN": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, MV.Visible, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; break;
                case "btnMTXN": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, TX.Visible, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; break;
                case "btnMBFN": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, BF.Visible, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; break;
                case "btnMADN": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, AD.Visible, userID); TransactionID = "Address"; Transaction = "ADDRESS"; break;
                case "btnMSWN": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, SW.Visible, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; break;
                case "btnMGPN": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, SW.Visible, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; break;

                case "btnMOTA": query = CKBL.getQueryBasedOnTransactionType("OT", "", checkLevel, OT.Visible, userID); TransactionID = "Overtime"; Transaction = "OVERTIME"; break;
                case "btnMLVA": query = CKBL.getQueryBasedOnTransactionType("LV", "", checkLevel, LV.Visible, userID); TransactionID = "Leave"; Transaction = "LEAVE"; break;
                case "btnMTRA": query = CKBL.getQueryBasedOnTransactionType("TR", "", checkLevel, TR.Visible, userID); TransactionID = "Time Modification"; Transaction = "TIMEMODIFICATION"; break;
                case "btnMFTA": query = CKBL.getQueryBasedOnTransactionType("FT", "", checkLevel, FT.Visible, userID); TransactionID = "Flex Time"; Transaction = "FLEXTIME"; break;
                case "btnMJSA": query = CKBL.getQueryBasedOnTransactionType("JS", "", checkLevel, JS.Visible, userID); TransactionID = "Job Split"; Transaction = "JOBSPLIT"; break;
                case "btnMMVA": query = CKBL.getQueryBasedOnTransactionType("MV", "", checkLevel, MV.Visible, userID); TransactionID = "Work Information"; Transaction = "WORKINFORMATION"; break;
                case "btnMTXA": query = CKBL.getQueryBasedOnTransactionType("TX", "", checkLevel, TX.Visible, userID); TransactionID = "Tax / Civil"; Transaction = "TAXCIVIL"; break;
                case "btnMBFA": query = CKBL.getQueryBasedOnTransactionType("BF", "", checkLevel, BF.Visible, userID); TransactionID = "Beneficiary"; Transaction = "BENEFICIARY"; break;
                case "btnMADA": query = CKBL.getQueryBasedOnTransactionType("AD", "", checkLevel, AD.Visible, userID); TransactionID = "Address"; Transaction = "ADDRESS"; break;
                case "btnMSWA": query = CKBL.getQueryBasedOnTransactionType("SW", "", checkLevel, AD.Visible, userID); TransactionID = "Straightwork"; Transaction = "STRAIGHTWORK"; break;
                case "btnMGPA": query = CKBL.getQueryBasedOnTransactionType("GP", "", checkLevel, AD.Visible, userID); TransactionID = "Gate Pass"; Transaction = "GATEPASS"; break;
            }
            this.queryDetails = query;
            //bindtosecondgrid
        }
       
    }

    private void BindToGrid()
    {
        agvData.UnGroup(agvData.Columns["Section"]);
        agvData.UnGroup(agvData.Columns["Line"]);
        this.lblInformation.Text = this.TransactionType.Trim() == string.Empty
                                  ? ""
                                  : "Loaded from " + this.TransactionType.Trim();
        if (this.TransactionType.Trim() == "Check List")
        {
            this.agvData.Selection.UnselectAll();
            this.lblDoubleClick.Visible = false;
            if (txtApproverOverride.Text.Trim() == string.Empty)
            {
                this.agvData.ClientSideEvents.RowDblClick = "SetRowToLoad";
                this.lblDoubleClick.Visible = true;
            }
        }
        else if (this.TransactionType.Trim() == "Wait List")
        {
            this.agvData.ClientSideEvents.RowDblClick = null;        
        }
        if (this.TransactionType.Trim() == "New and Pending List")
        {
            this.lblDoubleClick.Visible = false;
            this.agvData.ClientSideEvents.RowDblClick = null;
            if (txtApproverOverride.Text.Trim() == string.Empty)
            {
                this.agvData.ClientSideEvents.RowDblClick = "SetRowToLoad";
                this.lblDoubleClick.Visible = true;
            }
        }
        this.lblTransactionType.Text = "[" + this.TransactionID.Trim() + " Transactions]";

        //CommonMethods.BindGrid(this.agvData, this.sqlData, this.GetQuery, "Control No");

        agvData.SettingsText.EmptyDataRow = "No Data to Display";
        agvData.DataSource = null;
        sqlData.SelectCommand = GetQuery;
        sqlData.DataBind();
        agvData.DataBind();
        agvData.ExpandAll();
        Session["countViewStateRowCount"]=agvData.VisibleRowCount;
        agvData.GroupBy(agvData.Columns["Section"]);
        agvData.GroupBy(agvData.Columns["Line"]);
        agvData.ExpandAll();   
        for (int i = 0; i < agvData.Columns.Count; i++)
        {
            agvData.Columns[i].CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False;
        }
        //this.btnApprove.ClientEnabled = false;
        //this.btnEndorseChecker2.ClientEnabled = false;
        //this.btnEndorseApprove.ClientEnabled = false;
        this.btnEndorseApprove.ClientEnabled = true;
        this.btnDisApproveTran.ClientEnabled = true;
        this.btnRet.ClientEnabled = true;
        groupCount = 1;
        //SetupGrid();
    }
    private void SetupGrid()
    {
        string qstring = this.TransactionID;
        this.agvData.Visible = true;
        this.agvData.Columns[0].Visible = false;
        for (int i = 0; i < agvData.Columns.Count; i++)
        {
            if (agvData.Columns[i] is DevExpress.Web.ASPxGridView.GridViewCommandColumn)
            {
                agvData.Columns[i].Visible = (this.TransactionType.Trim() == "Check List" || this.TransactionType.Trim() == "Wait List" || this.TransactionType.Trim() == "Next Level / For Approval List");
            }
        }
        this.btnEndorseApprove.Visible = true;
        this.btnDisApproveTran.Visible = true;
        this.btnShowDetails.Visible = true;
        this.btnShowWaitListDetails.Visible = true;
        this.btnRet.Visible = true;
        if (this.TransactionType == "Check List")
        {
            this.pnlChoices.Visible = true;
            this.pnlEndorseApprove.Visible = this.rbtnChoices.SelectedValue.ToString() == "A" ? true : false;
            this.pnlDisapprove.Visible = this.rbtnChoices.SelectedValue.ToString() == "A" ? false : true;

            this.btnShowWaitListDetails.Visible = false;
            this.btnShowNextLevelDetails.Visible = false;
            agvData.Columns[0].Visible = true;
            if (txtApproverOverride.Text.Trim() != string.Empty)
                btnShowDetails.Visible = false;
        }
        else if(this.TransactionType == "Wait List")
        {
            this.pnlChoices.Visible = false;
            this.pnlEndorseApprove.Visible = true;
            this.pnlDisapprove.Visible = false;

            this.btnEndorseApprove.Visible = false;
            this.btnDisApproveTran.Visible = false;
            this.btnShowDetails.Visible = false;
            if (txtApproverOverride.Text.Trim() != string.Empty)
                btnShowWaitListDetails.Visible = false;
            else
                this.btnShowWaitListDetails.Visible = true;
            this.btnShowNextLevelDetails.Visible = false;
            this.btnRet.Visible = false;

        }
        else if (this.TransactionType == "Next Level / For Approval List")
        {
            this.pnlChoices.Visible = false;
            this.pnlEndorseApprove.Visible = true;
            this.pnlDisapprove.Visible = false;

            this.btnEndorseApprove.Visible = false;
            this.btnDisApproveTran.Visible = false;
            this.btnShowDetails.Visible = false;
            this.btnShowWaitListDetails.Visible = false;
            if (txtApproverOverride.Text.Trim() != string.Empty)
                btnShowNextLevelDetails.Visible = false;
            else
                this.btnShowNextLevelDetails.Visible = true;
            this.btnRet.Visible = false;

        }
        else
        {
            this.pnlChoices.Visible = false;
            this.pnlEndorseApprove.Visible = false;
            this.pnlDisapprove.Visible = false;
        }
        

        string strColname = string.Empty;
        switch (qstring.ToUpper())
        {
            case "OVERTIME":
                #region Overtime
                this.agvData.Columns["Transaction Date"].Caption = "OT Date";
                this.agvData.Columns["Hours"].Caption = "OT Hours";
                this.agvData.Columns["Type"].Caption = "OT Type";

                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            case "LEAVE":
                #region Leave
                this.agvData.Columns["Transaction Date"].Caption = "Leave Date";
                this.agvData.Columns["Hours"].Caption = "Leave Hours";
                this.agvData.Columns["Type"].Caption = "Leave Type";
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            case "TIME MODIFICATION":
                #region Time Mod
                this.agvData.Columns["Transaction Date"].Caption = "Modification Date";
                this.agvData.Columns["Type"].Caption = "Mod Type";
                this.agvData.Columns["Start Time"].Visible = false;
                this.agvData.Columns["End Time"].Visible = false;
                this.agvData.Columns["Hours"].Visible = false;
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            case "FLEX TIME":
                #region Flex
                this.agvData.Columns["Transaction Date"].Caption = "Flex Date";
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            case "JOB SPLIT":
                #region Jobsplit
                this.agvData.Columns["Transaction Date"].Caption = "Jobsplit Date";
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            case "STRAIGHTWORK":
                #region Straightwork
                agvData.Visible = true;
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;

            case "GATE PASS":
                #region Time Mod
                this.agvData.Columns["Transaction Date"].Caption = "Gate Pass Date";
                this.agvData.Columns["Type"].Caption = "Application Type";
                this.agvData.Columns["Start Time"].Visible = false;
                this.agvData.Columns["End Time"].Visible = false;
                this.agvData.Columns["Hours"].Visible = false;
                this.agvData.Columns["Col 06"].Visible = false;
                #endregion
                break;
            default:
                #region Others
                this.agvData.Columns["Transaction Date"].Caption = "Effectivity Date";
                switch (qstring.ToUpper())
                {
                    case "WORK INFORMATION":
                        #region Movement
                            this.agvData.Columns["Start Time"].Caption = "Move From";
                            this.agvData.Columns["End Time"].Caption = "Move To";
                            this.agvData.Columns["Hours"].Visible = false;
                            this.agvData.Columns["Col 06"].Visible = false;
                        #endregion
                        break;
                    case "TAX / CIVIL":
                        #region Tax Info
                        this.agvData.Columns["Start Time"].Caption = "Civil From";
                        this.agvData.Columns["End Time"].Caption = "Civil To";
                        this.agvData.Columns["Hours"].Caption = "Tax From";
                        this.agvData.Columns["Col 06"].Caption = "Tax To";
                        #endregion
                        break;
                    case "BENEFICIARY":
                        #region Beneficiary
                            this.agvData.Columns["Start Time"].Visible = false;
                            this.agvData.Columns["End Time"].Visible = false;
                            this.agvData.Columns["Hours"].Visible = false;
                            this.agvData.Columns["Col 06"].Visible = false;
                        #endregion
                        break;
                    case "ADDRESS":
                        #region Address
                            this.agvData.Columns["Start Time"].Visible = false;
                            this.agvData.Columns["End Time"].Visible = false;
                            this.agvData.Columns["Hours"].Visible = false;
                            this.agvData.Columns["Col 06"].Visible = false;
                        #endregion
                        break;
                    default:
                        this.agvData.Visible = false;
                        break;
                }
                //this.agvData.Columns["Filler1Desc"].Visible = false;
                //this.agvData.Columns["Filler2Desc"].Visible = false;
                //this.agvData.Columns["Filler3Desc"].Visible = false;
                //this.agvData.Columns["Batch No"].Visible = false;
                #endregion
                break;
        }
    }
    protected void cbAll_Init(object sender, EventArgs e)
    {
        ASPxCheckBox chk = sender as ASPxCheckBox;
        ASPxGridView grid = (chk.NamingContainer as GridViewHeaderTemplateContainer).Grid;

        string s = string.Empty;
        if (Session["countViewStateRowCount"] != null)
            s = Session["countViewStateRowCount"].ToString();
        hfRowCount.Value = s;// countViewStateRowCount.ToString();
        int rowCount = Int32.Parse(hfRowCount.Value.ToString() == null || hfRowCount.Value.ToString().Trim() == "" ? "0" : hfRowCount.Value.ToString());
        chk.Checked = (grid.Selection.Count == rowCount && grid.Selection.Count != 0);
        //Session["countViewStateRowCount"] = 0;
        //hfRowCount.Value = agvData.Selection.Count.ToString();
    }
    protected void grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e)
    {
        try
        {
            ASPxGridView grid = sender as ASPxGridView;
            Int32 start = grid.VisibleStartIndex;
            Int32 end = grid.VisibleStartIndex + grid.SettingsPager.PageSize;
            Int32 selectNumbers = 0;

            Int32 E1 = 0;
            Int32 E2 = 0;
            Int32 AP = 0;
            end = (end > grid.VisibleRowCount ? grid.VisibleRowCount : end);
            for (Int32 i = start; i < end; i++)
            {
                if (grid.Selection.IsRowSelected(i))
                {
                    //DataRow dr = grid.GetDataRow(i);
                    //if (dr["STATUS"].ToString().Trim() == "ENDORSED TO CHECKER 1")
                    //    E1++;
                    //else if (dr["STATUS"].ToString().Trim() == "ENDORSED TO CHECKER 2")
                    //    E2++;
                    //else if (dr["STATUS"].ToString().Trim() == "ENDORSED TO APPROVER")
                    //    AP++;
                    selectNumbers++;
                }
            }
            e.Properties["cpSelectedRowsOnPage"] = selectNumbers;
            e.Properties["cpVisibleRowCount"] = hfRowCount.Value;
            e.Properties["cpE1"] = E1;
            e.Properties["cpE2"] = E2;
            e.Properties["cpAP"] = grid.VisibleRowCount;
            string s ="";
            foreach (GridViewDataColumn gvdc in grid.GetGroupedColumns()){
                s += gvdc.FieldName + "|";
            }
            s =s.Remove(s.Length - 1, 1);
            e.Properties["cpGrouedColumns"] = s;
        }
        catch
        {

        }
    }
    protected void agvData_DataBinding(object sender, EventArgs e)
    {
       
                
    }
    protected void agvData_CustomColumnGroup(object sender, CustomColumnSortEventArgs e)
    {
        //agvData.ExpandAll();
    }
    protected void hfToLoad_ValueChanged(object sender, EventArgs e)
    {
        //string value=agvData.GetRowValues(Int32.Parse(hfToLoad.Value.ToString()), "Key").ToString();
    }
    protected void agvResults_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {

    }
    protected void agvDetails_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        agvResults.Visible = false;
        string query = CKBL.getOvertimeQueryDetails(CheckLevel, false, false, hfToLoad.Value.ToString());
        queryDetails=query = string.Format(query, Session["userLogged"].ToString());
        SqlDetails.SelectCommand = query;
        SqlDetails.DataBind();
        agvDetails.DataBind();
        for (int i = 0; i < agvDetails.Columns.Count; i++)
        {
            agvDetails.Columns[i].CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False;
        }
        //agvDetails.SettingsBehavior.
    }
    ////testing https://www.devexpress.com/Support/Center/Question/Details/Q253997
    //https://community.devexpress.com/forums/t/100178.aspx
    
    protected void Grid_CustomCallback2(object sender, ASPxGridViewCustomCallbackEventArgs e) {
        //hfRowCount.Value = "0";
        //if (!Boolean.Parse(hfTriggeredChildNode.Value.ToString()))
        {
            agvData.FocusedRowIndex = -1;
            int countData = 0;
            string[] parameters = e.Parameters.Split(';');

            int index = int.Parse(parameters[0]);
            string fieldname = parameters[1];
            bool isGroupRowSelected = bool.Parse(parameters[2]);

            System.Collections.ObjectModel.ReadOnlyCollection<GridViewDataColumn> groupedCols = agvData.GetGroupedColumns();


            if (groupedCols[groupedCols.Count - 1].FieldName == fieldname)
            {
                // Checked groupcolumn is the lowest level groupcolumn;
                // we can apply original recursive checking here
                if (!agvData.IsRowExpanded(index))
                    agvData.ExpandRow(index, true); // expand grouped column for consistent behaviour
                int childRowCount = agvData.GetChildRowCount(index);
                for (int i = 0; i < childRowCount; i++)
                {
                    DataRow row = agvData.GetChildDataRow(index, i);
                    object test = row["Key"];
                    agvData.Selection.SetSelectionByKey(row["Key"], isGroupRowSelected);
                    //countData++;
                }
                //hfRowCount.Value = countData.ToString();
            }
            else if (groupedCols[0].FieldName == fieldname)
            {
                // checked row is not the lowest level groupcolumn:
                // we will find the Datarows that are to be checked recursively by iterating all rows
                // and compare the fieldvalues of the fields described by the checked groupcolumn
                // and all its parent groupcolumns. Rows that match these criteria are to the checked.
                // CAVEAT: only expanded rows can be iterated, so we will have to expand clicked row recursivly before iterating the grid

                int gidx = -1;
                foreach (GridViewDataColumn gcol in groupedCols)
                {
                    if (gcol.FieldName == fieldname)
                    {
                        gidx = groupedCols.IndexOf(gcol);
                        break;
                    }
                }

                DataRow CheckedDataRow = agvData.GetDataRow(index);
                //Build dictionary with checked groucolumn and its parent groupcolumn fieldname and values
                Dictionary<string, object> DictParentFieldnamesValues = new Dictionary<string, object>();
                string parentfieldname;
                object parentkeyvalue;
                for (int i = gidx; i >= 0; i--)
                {
                    // find parent groupcols and parentkeyvalue
                    GridViewDataColumn pcol = groupedCols[i];
                    parentfieldname = pcol.FieldName;
                    parentkeyvalue = CheckedDataRow[parentfieldname];
                    DictParentFieldnamesValues.Add(parentfieldname, parentkeyvalue);
                }


                bool isChildDatarowOfClickedGroup;
                if (!agvData.IsRowExpanded(index))
                    agvData.ExpandRow(index, true);
                for (int i = 0; i <= agvData.VisibleRowCount - 1; i++)
                {
                    DataRow row = agvData.GetDataRow(i);

                    isChildDatarowOfClickedGroup = true;
                    // check whether row does belong to checked group all the parent groups of the clicked group
                    foreach (KeyValuePair<string, object> kvp in DictParentFieldnamesValues)
                    {
                        parentfieldname = kvp.Key;
                        parentkeyvalue = kvp.Value;
                        if (row[parentfieldname].Equals(parentkeyvalue) == false)
                        {
                            isChildDatarowOfClickedGroup = false;
                            break;
                            //Iterated row does not belong to at least one parentgroup of the clicked groupbox; do not change selection state for this row
                        }
                    }

                    if (isChildDatarowOfClickedGroup == true)
                    {
                        //row meets all the criteria for belonging to the clicked group and all parents of the clicked group:
                        // change selection state
                        agvData.Selection.SetSelectionByKey(row["Key"], isGroupRowSelected);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= agvData.VisibleRowCount - 1; i++)
                {
                    DataRow row = agvData.GetDataRow(i);
                    agvData.Selection.SetSelectionByKey(row["Key"], isGroupRowSelected);
                }
            }
        }
   }

    protected void Grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        string[] parameters = e.Parameters.Split(';');
        int index = int.Parse(parameters[0]);
        bool isGroupRowSelected = bool.Parse(parameters[1]);
        int childRowCount = agvData.GetChildRowCount(index);
        for (int i = 0; i < childRowCount; i++)
        {
            DataRow row = agvData.GetChildDataRow(index, i);
            agvData.Selection.SetSelectionByKey(row["Key"], isGroupRowSelected);

        }
        agvData.ExpandAll();

    }
    private int countViewStateRowCount
    {
        set
        {
            this.ViewState["countViewStateRowCount"] = value;
        }
        get
        {
            return this.ViewState["countViewStateRowCount"] == null
                    ? 0
                    : Int32.Parse(this.ViewState["countViewStateRowCount"].ToString().Trim());
        }
    }
    protected void Grid_HtmlRowPrepared2(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType == GridViewRowType.Group)
        {
            //       foreach (TableCell cell in e.Row.Cells)
            //       {
            //           foreach (Control control in cell.Controls)
            //           {
            //               GridViewGroupRowTemplateContainer container = control as GridViewGroupRowTemplateContainer;
            //               if (container == null)
            //                   continue;
            //               ASPxCheckBox checkBox = (ASPxCheckBox)container.FindControl("checkBox");
            //               checkBox.Visible = (this.TransactionType.Trim() == "Check List");
            //               //ProviderConnectionPoint the CallbackArgumentsParser function also with the fieldname of the grouped column
            //               checkBox.ClientSideEvents.CheckedChanged = string.Format(@"function(s, e){{ agvData.PerformCallback('{0};{1};' + s.GetChecked()); }}",

            //container.VisibleIndex, container.Column.FieldName);

            //               //checkBox.Checked = GetChecked(container);
            //               checkBox.Checked = GetChecked(e.VisibleIndex);
            //           }
            //       }
            ASPxCheckBox checkBox = (ASPxCheckBox)agvData.FindGroupRowTemplateControl(e.VisibleIndex, "checkBox");
            if (checkBox != null)
                checkBox.Checked = GetChecked(e.VisibleIndex);
        }
        
    }
    protected void checkBox_Init(object sender, EventArgs e)
    {
        ASPxCheckBox checkBox = sender as ASPxCheckBox;
        GridViewGroupRowTemplateContainer container = checkBox.NamingContainer as GridViewGroupRowTemplateContainer;
        checkBox.ClientSideEvents.CheckedChanged = string.Format("function(s, e){{ agvData.PerformCallback('{0};{1};' + s.GetChecked()); }}", container.VisibleIndex, container.Column.FieldName);
    }
    protected void Grid_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType == GridViewRowType.Group)
        {
            ASPxCheckBox checkBox = agvData.FindGroupRowTemplateControl(e.VisibleIndex, "checkBox") as ASPxCheckBox;

            if (checkBox != null)
            {
                checkBox.ClientSideEvents.CheckedChanged = string.Format("function(s, e){{ agvData.PerformCallback('{0};' + s.GetChecked()); }}", e.VisibleIndex);
                checkBox.Checked = GetChecked(e.VisibleIndex);
            }

        }

    }

    protected bool GetChecked(int visibleIndex)
    {
        int childRowCount = agvData.GetChildRowCount(visibleIndex);
        for (int i = 0; i < childRowCount; i++)
        {
            bool isRowSelected = agvData.Selection.IsRowSelectedByKey(agvData.GetChildDataRow(visibleIndex, i)["Key"]);

            if (!isRowSelected)
                return false;
        }
        return true;
    }
    protected bool GetChecked(GridViewGroupRowTemplateContainer container)
    {
        string keyvalue = container.KeyValue.ToString();
        //container.Column["Key"]
        //string =container.Column["Key"].;
        //for (int i = 0; i < agvData.GetChildRowCount(visibleIndex); i++)
        //{
        //    bool isRowSelected = agvData.Selection.IsRowSelectedByKey(agvData.GetChildDataRow(visibleIndex, i)["Key"]);

        //    if (!isRowSelected)
        //        return false;
        //}
        //foreach (GridViewGroupRowTemplateContainer contain in agvData)
        //{ 
        //}

        return false;
    }

    //protected void treeList_CustomDataCallback(object sender, DevExpress.Web.ASPxTreeList.TreeListCustomDataCallbackEventArgs e)
    //{
    //    //e.Result = treeList.SelectionCount.ToString();
    //}
    //protected void treeList_DataBound(object sender, EventArgs e)
    //{
    //    SetNodeSelectionSettings();
    //}
    
    //void SetNodeSelectionSettings()
    //{
    //    TreeListNodeIterator iterator = treeList.CreateNodeIterator();
    //    TreeListNode node;
    //    while (true)
    //    {
    //        node = iterator.GetNext();
    //        if (node == null) break;
    //        //switch (cmbMode.SelectedIndex)
    //        //{
    //        //    case 1:
    //        //        node.AllowSelect = !node.HasChildren;
    //        //        break;
    //        //    case 2:
    //        //        node.AllowSelect = node.HasChildren;
    //        //        break;
    //        //    case 3:
    //        //        node.AllowSelect = node.Level > 2;
    //        //        break;
    //        //}
    //    }
    //}
    protected void btnApproverOverrideClear_Click(object sender, EventArgs e)
    {
        txtApproverOverride.Text = txtApproverOverrideDesc.Text = string.Empty;
        InitialzieCounters();
    }
}