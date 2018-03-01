using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary;
using Payroll.DAL;
using System.Data;

public partial class Maintenance_ApprovalRoute_pgePopEmployeeApprovalRoute : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    GeneralBL GNBL = new GeneralBL();
    MenuGrant MGBL = new MenuGrant();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFROUTEENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());               
                InitializeControls();
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
            }
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgePopEmployeeApprovalRoute_LoadComplete);
        }
    }
    void Maintenance_ApprovalRoute_pgePopEmployeeApprovalRoute_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        
        btnOvertime.OnClientClick = "return lookupNewARRouteAssignment('txtOvertime')";
        //btnOTNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("OVERTIME"), Encrypt.encryptText(hfEmployeeID.Value.ToString()));
        txtOvertime.Attributes.Add("readOnly", "true");
        txtOvertimeAP.Attributes.Add("readOnly", "true");
        txtOvertimeC1.Attributes.Add("readOnly", "true");
        txtOvertimeC2.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpOTStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        SetMinDate();
        
    }
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }
    private void InitializeControls()
    {
        hfEmployeeID.Value = Request.QueryString["employeeId"].ToString();
        hfState.Value = Request.QueryString["state"].ToString();
        Session["_employeeID"] = hfEmployeeID.Value;
        ClearControls();

        if (Request.QueryString["transaction"].ToString().Equals("TAXMVMNT"))
            lblOvertime.Text = "TAX/CIVIL STATUS";
        else if (Request.QueryString["transaction"].ToString().Equals("BNEFICIARY"))
            lblOvertime.Text = "BENEFICIARY";
        else if (Request.QueryString["transaction"].ToString().Equals("MOVEMENT"))
            lblOvertime.Text = "WORK INFO";
        else if (Request.QueryString["transaction"].ToString().Equals("STRAIGHTWK"))
            lblOvertime.Text = "STRAIGHT WORK";
        else if (Request.QueryString["transaction"].ToString().Equals("TIMEMOD"))
            lblOvertime.Text = "TIME RECORD";
        else if (Request.QueryString["transaction"].ToString().Equals("JOBSPLIT"))
            lblOvertime.Text = "MAN HOUR";
        else if (Request.QueryString["transaction"].ToString().Equals("GATEPASS"))
            lblOvertime.Text = "GATE PASS";
        else
            lblOvertime.Text = Request.QueryString["transaction"].ToString();

        if (hfState.Value.ToString().Equals("EDIT"))
        {
            txtOvertime.Text = Request.QueryString["routeID"].ToString();
            dtpOTStartDate.DateString = Request.QueryString["startDate"].ToString();
            dtpOTStartDate.Enabled = false;
            dtpOTEndDate.Enabled = false;
            btnY.Enabled = false;

            DataTable dtResult = getCheckerApprover();
            if (dtResult.Rows.Count > 0)
            {
                txtOvertimeC1.Text = dtResult.Rows[0]["Checker 1"].ToString();
                txtOvertimeC2.Text = dtResult.Rows[0]["Checker 2"].ToString();
                txtOvertimeAP.Text = dtResult.Rows[0]["Approver"].ToString();
                dtpOTEndDate.DateString = dtResult.Rows[0]["End Date"].ToString();
                chkApprove.Checked = Convert.ToBoolean(dtResult.Rows[0]["Approve"].ToString());
                chkDisapprove.Checked = Convert.ToBoolean(dtResult.Rows[0]["Disapprove"].ToString());
                chkEndorse.Checked = Convert.ToBoolean(dtResult.Rows[0]["Endorse"].ToString());
                chkReturn.Checked = Convert.ToBoolean(dtResult.Rows[0]["Return"].ToString());
            }
        }

    }

    private void ClearControls()
    {
        txtOvertime.Text = string.Empty;
        txtOvertimeC1.Text = string.Empty;
        txtOvertimeC2.Text = string.Empty;
        txtOvertimeAP.Text = string.Empty;
        dtpOTEndDate.Reset();
        dtpOTStartDate.Reset();
        chkApprove.Checked = false;
        chkDisapprove.Checked = false;
        chkEndorse.Checked = false;
        chkReturn.Checked = false;
    }
    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            string transactionUpdated = string.Empty;
            string noChangesMande = string.Empty;
            string startDate = string.Empty;
            string endDate = string.Empty;

            string errMsg = checkEntry();
            if (errMsg.Equals(string.Empty))
            {
                startDate = dtpOTStartDate.DateString == string.Empty ? string.Empty : dtpOTStartDate.DateString;
                endDate = dtpOTEndDate.DateString == string.Empty ? string.Empty : dtpOTEndDate.DateString;

                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();

                        switch(Request.QueryString["transaction"].ToString())
                        {                            
                            case "OVERTIME":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "OVERTIME", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Overtime";
                                    }
                                    catch //PK error meaning transation exists
                                    {
                                        GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                    }
                                }
                                else
                                {
                                    if(GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Overtime";
                                    else
                                        noChangesMande += "\n  Overtime";
                                }
                                
                                break;
                            case "LEAVE":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "LEAVE", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "LEAVE", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Leave";
                                    }
                                    catch
                                    {
                                        //GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "LEAVE", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    }
                                }
                                else
                                {
                                    if(GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "LEAVE", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Leave";
                                    else
                                        noChangesMande += "\n  Leave";
                                }
                               
                                break;
                            case "TIMEMOD":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "TIMEMOD", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TIMEMOD", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Time Record";
                                    }
                                    catch
                                    {
                                        //
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TIMEMOD", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Time Record";
                                    else
                                        noChangesMande += "\n  Time Record";
                                }
                                break;

                            case "JOBSPLIT":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "JOBMOD", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "JOBMOD", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Man Hour";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "JOBMOD", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Man Hour";
                                    else
                                        noChangesMande += "\n  Man Hour";
                                }
                                break;
                            case "STRAIGHTWORK":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "STRAIGHTWK", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "STRAIGHTWK", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Straight Work";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "STRAIGHTWK", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Straight Work";
                                    else
                                        noChangesMande += "\n  Straight Work";
                                }
                                break;

                            case "FLEXTIME":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "FLEXTIME", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "FLEXTIME", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Flex Time";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "FLEXTIME", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Flex Time";
                                    else
                                        noChangesMande += "\n  Flex Time";
                                }
                                break;
                            case "BNEFICIARY":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "BNEFICIARY", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "BNEFICIARY", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Beneficiary";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "BNEFICIARY", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Beneficiary";
                                    else
                                        noChangesMande += "\n  Beneficiary";
                                }
                                break;
                            case "ADDRESS":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "ADDRESS", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "ADDRESS", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Address";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "ADDRESS", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Address";
                                    else
                                        noChangesMande += "\n  Address";
                                }
                                break;
                            case "MOVEMENT":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "MOVEMENT", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "MOVEMENT", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Work Information";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "MOVEMENT", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Work Information";
                                    else
                                        noChangesMande += "\n  Work Information";
                                }
                                break;
                            case "TAXMVMNT":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "TAXMVMNT", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TAXMVMNT", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Tax/Civil Status";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TAXMVMNT", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Tax/Civil Status";
                                    else
                                        noChangesMande += "\n  Tax/Civil Status";
                                }
                                break;
                            case "MANPOWER":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "MANPOWER", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "MANPOWER", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Man Power";
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    if(GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "MANPOWER", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                    transactionUpdated += "\n  Man Power";
                                    else
                                        noChangesMande += "\n  Man Power";
                                }
                                break;
                            case "TRAINING":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "TRAINING", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TRAINING", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Training";
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "TRAINING", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Training";
                                    else
                                        noChangesMande += "\n  Training";
                                }
                                break;
                            case "GATEPASS":
                                if (!hfState.Value.ToString().Equals("EDIT"))
                                {
                                    try
                                    {
                                        GNBL.UpdateLatestRoute(hfEmployeeID.Value.ToString(), "GATEPASS", startDate, dal);
                                        GNBL.InsertEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "GATEPASS", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal);
                                        transactionUpdated += "\n  Gate Pass";
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    if (GNBL.UpdateEmployeeRoute(hfEmployeeID.Value.ToString(), txtOvertime.Text, "GATEPASS", Session["userLogged"].ToString(), startDate, endDate, chkEndorse.Checked, chkReturn.Checked, chkApprove.Checked, chkDisapprove.Checked, dal) > 0)
                                        transactionUpdated += "\n  Gate Pass";
                                    else
                                        noChangesMande += "\n  Gate Pass";
                                }
                                break;
                        }

                        if (!transactionUpdated.Equals(string.Empty))
                        {
                            MessageBox.Show(string.Format("Successfully {0} route.", hfState.Value.ToString().Equals("EDIT") ? "updated" : "added new"));
                        }
                        else
                            if(!noChangesMande.Equals(string.Empty))
                            MessageBox.Show(string.Format("No changes made."));
                        //hfPrevEntry.Value = changeSnapShot();
                        dal.CommitTransactionSnapshot();
                    }
                    catch (Exception ex)
                    {
                        dal.RollBackTransactionSnapshot();
                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            else
            {
                MessageBox.Show(errMsg);
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
            Session["_employeeID"] = hfEmployeeID.Value;
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Save

    protected void btnY_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            ClearControls();
            Session["_employeeID"] = hfEmployeeID.Value;
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Cancel

    private string checkEntry()
    {
        string err = string.Empty;

        if (dtpOTStartDate.DateString != string.Empty
            && dtpOTEndDate.DateString != string.Empty)
        {
            if (dtpOTStartDate.Date > dtpOTEndDate.Date)
                err += "Start Date is greater than End Date.\n";
        }   

        return err;
    }
    private DataTable getCheckerApprover()
    {
        string query = string.Format(@"SELECT T_EmployeeApprovalRoute.Arm_TransactionID [Transaction Code]
	                             , T_EmployeeApprovalRoute.Arm_RouteID [Route ID]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
                                 , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
                                 , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_StartDate,101) [Start Date]
                                 , ISNULL(T_EmployeeApprovalRoute.Arm_NotifyEndorse, 0) [Endorse]
                                 , ISNULL(T_EmployeeApprovalRoute.Arm_NotifyReturn, 0) [Return]
                                 , ISNULL(T_EmployeeApprovalRoute.Arm_NotifyDisapprove, 0) [Disapprove]
                                 , ISNULL(T_EmployeeApprovalRoute.Arm_NotifyApprove, 0) [Approve]    
                                 , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_EndDate,101) [End Date]
                                FROM T_EmployeeApprovalRoute
                                INNER JOIN T_ApprovalRouteMaster
                                    ON T_ApprovalRouteMaster.Arm_RouteID = T_EmployeeApprovalRoute.Arm_RouteID
                                WHERE Arm_EmployeeId = '{0}'
                                    AND Arm_TransactionID = '{1}'
                                    AND T_EmployeeApprovalRoute.Arm_RouteID = '{2}'
                                    AND Arm_StartDate = '{3}'", hfEmployeeID.Value.ToString()
                                                        , Request.QueryString["transaction"].ToString()
                                                        , txtOvertime.Text
                                                        , Request.QueryString["startDate"].ToString());
        DataTable dtResult = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }

        return dtResult;
    }
    private void SetMinDate()
    {
        if (hfState.Value.ToString() == "NEW")
        {
            string query = @"SELECT
	                            Ppm_StartCycle
                            FROM T_PayPeriodMaster
                            WHERE Ppm_CycleIndicator = 'C'";
            string query2 = string.Format(@"SELECT COALESCE(Arm_EndDate, Arm_StartDate)
                                FROM T_EmployeeApprovalRoute
                                WHERE Arm_EmployeeID = '{0}'
                                AND Arm_TransactionID = '{1}'
                                AND Arm_StartDate = (SELECT MAX(Arm_StartDate) FROM T_EmployeeApprovalRoute
                                WHERE Arm_EmployeeID = '{0}'
                                AND Arm_TransactionID = '{1}')", hfEmployeeID.Value.ToString()
                                                               , Request.QueryString["transaction"].ToString());
            string query3 = string.Format(@"SELECT Emt_HireDate FROM T_EmployeeMaster
                                WHERE Emt_EmployeeID = '{0}'", hfEmployeeID.Value.ToString());

            DataTable dt;
            DataTable dt2;
            DataTable dt3;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dt2 = dal.ExecuteDataSet(query2).Tables[0];
                dt3 = dal.ExecuteDataSet(query3).Tables[0];
                dal.CloseDB();
            }

            if (dt2 != null && dt2.Rows.Count > 0 && dt.Rows[0][0].ToString() != "")
            {
                DataRow drResult = dt2.Rows[0];
                dtpOTStartDate.MinDate = Convert.ToDateTime(drResult[0]).AddDays(1);
            }
            else if (dt != null && dt.Rows.Count > 0)
            {

                DataRow drResult;
                if (dt3.Rows.Count > 0 && dt3 != null && Convert.ToDateTime(dt3.Rows[0]["Emt_HireDate"].ToString()) < Convert.ToDateTime(dt.Rows[0]["Ppm_StartCycle"].ToString()))
                    drResult = dt3.Rows[0];
                else
                    drResult = dt.Rows[0];

                dtpOTStartDate.MinDate = Convert.ToDateTime(drResult[0]);
            }
        }
    }
}