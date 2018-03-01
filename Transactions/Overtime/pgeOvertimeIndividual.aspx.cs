/* File revision no. W2.1.00003
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/11/2013
 *  Update Notes    :   
 *      -   Updated End time computation, added additional javascript on starttime and hours textbox
 */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;
//using SystemMenuLogBL;

public partial class Transactions_Overtime_pgeOvertimeIndividual : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    OvertimeBL OTBL = new OvertimeBL();
    MenuGrant MGBL = new MenuGrant();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFOTENTRY"))
        {
            Response.Redirect(@"../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeEmployee();
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
                try
                {
                    string cntrlNumber = Request.QueryString["cn"].ToString();
                    loadTransactionDetail();
                    hfSaved.Value = "1";
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
                if (!hfPrevOTDate.Value.Equals(dtpOTDate.Date.ToShortDateString()))
                {
                    dtpOTDate_Change(dtpOTDate, new EventArgs());
                    hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
                }
            }
            LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeIndividual_LoadComplete);
            if (Session["transaction"].ToString().Equals("CHECKLIST"))
            {
                try
                {
                    loadTransactionDetail();
                    MessageBox.Show("Transaction loaded from checklist");
                }
                catch(Exception ex)
                { 
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
                Session["transaction"] = string.Empty;
            }
            else if (Session["transaction"].ToString().Equals("PENDING"))
            {
                //try
                //{
                //    loadTransactionDetail();
                //    //MessageBox.Show("Transaction loaded from pending list");
                //    hfSaved.Value = "1";
                //}
                //catch (Exception ex)
                //{
                //    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                //}
                Session["transaction"] = string.Empty;
            } 
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Overtime_pgeOvertimeIndividual_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "overtimeScripts";
        string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtOTEndTime.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        txtJobCode.Attributes.Add("readOnly", "true");
        txtClientJobNo.Attributes.Add("readOnly", "true");
        txtClientJobName.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpOTDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpOTDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");
        txtOTStartTime.Attributes.Add("OnKeyUp", "javascript:formatStartTime()");
        //txtOTStartTime.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        txtOTStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtOTStartTime.Attributes.Add("OnChange", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnKeyUp", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnChange", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        btnJobCode.OnClientClick = string.Format("return lookupOTJobCode()");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtRemarks.Attributes.Add("OnKeyUp", "javascript:isMaxLengthAfterKeyPress('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        txtReason.Attributes.Add("OnKeyUp", "javascript:isMaxLengthAfterKeyPress('txtReason',199);");
       
        //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
        //{
        //    txtOTHours.Attributes.Add("OnKeyUp", "javascript:computeEndTime()");
        //    //txtOTHours.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        //    txtOTHours.Attributes.Add("OnChange", "javascript:computeEndTime()");
        //    txtOTHours.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        //    txtOTEndTime.Attributes.Add("readOnly", "true");
        //}
        //else
        //{
        //    txtOTEndTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        //    txtOTEndTime.Attributes.Add("OnChange", "javascript:computeOTHours();");
        //    txtOTEndTime.Attributes.Add("OnKeyUp", "javascript:formatEndTime();");
        //    txtOTHours.Attributes.Add("readOnly", "true");
        //}
    }

    protected void dtpOTDate_Change(object sender, EventArgs e)
    {
        initializeShift(false);
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        clearValues();
        initializeShift(false);
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        bool flagSuccessful = false;
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (hfPrevEntry.Value.Equals(changeSnapShot()))
                {
                    if ( Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                      || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "OVERTIME", btnX.Text.Trim().ToUpper());
                            //EmailNotificationBL EMBL = new EmailNotificationBL();
                            //EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
                            if (!status.Equals(string.Empty))
                            {
                                //DataRow drDetails = PopulateDR(status, txtControlNo.Text);
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransactionSnapshot();

                                    string strEndorseApproveQuery = "";
                                    string strSuccessPrompt = "";
                                    switch (btnX.Text.ToUpper())
                                    {
                                        case "ENDORSE TO CHECKER 1":
                                        case "ENDORSE TO CHECKER 2":
                                        case "ENDORSE TO APPROVER":
                                            strEndorseApproveQuery = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', 'OT', '{2}', '{3}', '{4}' "
                                                                                        , txtControlNo.Text
                                                                                        , Session["userLogged"].ToString()
                                                                                        , status
                                                                                        , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                            strSuccessPrompt = "Successfully endorsed transaction.";
                                            break;
                                        case "APPROVE":
                                            strEndorseApproveQuery = string.Format("EXEC ApproveOvertime '{0}', '{1}', '{2}' "
                                                                                        , txtControlNo.Text
                                                                                        , Session["userLogged"].ToString()
                                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                            strSuccessPrompt = "Successfully approved transaction.";
                                            break;
                                        default:
                                            break;
                                    }

                                    DataTable dtResult = dal.ExecuteDataSet(strEndorseApproveQuery).Tables[0];
                                    DataRow[] drArrRows = dtResult.Select("Result = 1");
                                    if (drArrRows.Length > 0)
                                    {
                                        //MenuLog
                                        SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "", false);
                                        MessageBox.Show(strSuccessPrompt);
                                        flagSuccessful = true;
                                    }
                                    else
                                    {
                                        drArrRows = dtResult.Select("Result <> 1");
                                        if (drArrRows.Length > 0)
                                            MessageBox.Show(drArrRows[0]["Message"].ToString());
                                    }

                                    //switch (btnX.Text.ToUpper())
                                    //{
                                    //    case "ENDORSE TO CHECKER 1":
                                    //        OTBL.UpdateOTRecord(btnX.Text.Trim().ToUpper()
                                    //                           , drDetails
                                    //                           , dal);
                                    //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    //        MessageBox.Show("Successfully endorsed transaction.");
                                    //        break;
                                    //    case "ENDORSE TO CHECKER 2":
                                    //        OTBL.UpdateOTRecord(drDetails, dal);
                                    //        OTBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                    //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    //        MessageBox.Show("Successfully endorsed transaction.");
                                    //        break;
                                    //    case "ENDORSE TO APPROVER":
                                    //        OTBL.UpdateOTRecord(drDetails, dal);
                                    //        OTBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                    //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    //        MessageBox.Show("Successfully endorsed transaction.");
                                    //        break;
                                    //    case "APPROVE":
                                    //        if (!CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
                                    //        {
                                    //            OTBL.UpdateOTRecord(drDetails, dal);
                                    //            OTBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                    //            OTBL.EmployeeLogLedgerUpdate(txtControlNo.Text, Session["userLogged"].ToString(), dal);
                                    //            EMBL.ActionProperty = EmailNotificationBL.Action.APPROVE;
                                    //            MessageBox.Show("Successfully approved transaction.");
                                    //        }
                                    //        else
                                    //        {
                                    //            flagSuccessful = false;
                                    //            MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
                                    //        }
                                    //        break;
                                    //    default:
                                    //        break;
                                    //}
                                    //if (flagSuccessful)
                                    //{
                                    //    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    //    {
                                    //        EMBL.InsertInfoForNotification(txtControlNo.Text
                                    //                                      , Session["userLogged"].ToString()
                                    //                                      , dal);
                                    //    }
                                    //    //MenuLog
                                    //    SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(),"",false);
                                       
                                    //    restoreDefaultControls();
                                    //}
                                    dal.CommitTransactionSnapshot();
                                }
                                catch (Exception ex)
                                {
                                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                                    //drDetails = null;
                                    dal.RollBackTransactionSnapshot();
                                }
                                finally
                                {
                                    dal.CloseDB();
                                }

                                if (flagSuccessful)
                                    restoreDefaultControls();
                            }
                            else
                            {
                                MessageBox.Show("No route defined for user.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
                    }
                }
                else
                {
                    MessageBox.Show("Some changes have been made. Save transaction to update.");
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Endorse to Checker 1 / Endorse to Checker 2 / Endorse to Approver / Approve

    protected void btnY_Click(object sender, EventArgs e)
    {
        bool flagSuccessful = false;
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
                {
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransactionSnapshot();
                            DataTable dtResult;
                            DataRow[] drArrRows;
                            switch (btnY.Text.ToUpper())
                            {
                                case "CLEAR":
                                    restoreDefaultControls();
                                    break;
                                case "CANCEL":
                                    dtResult = dal.ExecuteDataSet(string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', 'OT', '{3}' "
                                                                                            , txtControlNo.Text
                                                                                            , Session["userLogged"].ToString()
                                                                                            , ""
                                                                                            , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                    drArrRows = dtResult.Select("Result = 1");
                                    if (drArrRows.Length > 0)
                                    {
                                        //MenuLog
                                        SystemMenuLogBL.InsertDeleteLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                        flagSuccessful = true;
                                        MessageBox.Show("Transaction cancelled");
                                    }
                                    else
                                    {
                                        drArrRows = dtResult.Select("Result <> 1");
                                        if (drArrRows.Length > 0)
                                            MessageBox.Show(drArrRows[0]["Message"].ToString());
                                    }

                                    //OTBL.UpdateOTRecord(PopulateDR("2", txtControlNo.Text), dal);
                                    ////MenuLog
                                    //SystemMenuLogBL.InsertDeleteLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                    // restoreDefaultControls();
                                    //MessageBox.Show("Transaction cancelled");
                                    break;
                                case "DISAPPROVE":
                                    if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                    {
                                        dtResult = dal.ExecuteDataSet(string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', 'OT', '{3}' "
                                                                                                , txtControlNo.Text
                                                                                                , Session["userLogged"].ToString()
                                                                                                , txtRemarks.Text.Trim().ToUpper()
                                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                        drArrRows = dtResult.Select("Result = 1");
                                        if (drArrRows.Length > 0)
                                        {
                                            //MenuLog
                                            SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                            flagSuccessful = true;
                                            MessageBox.Show("Successfully disapproved transaction.");
                                        }
                                        else
                                        {
                                            drArrRows = dtResult.Select("Result <> 1");
                                            if (drArrRows.Length > 0)
                                                MessageBox.Show(drArrRows[0]["Message"].ToString());
                                        }

                                        //string status = string.Empty;
                                        //switch (txtStatus.Text.ToUpper())
                                        //{
                                        //    case "ENDORSED TO CHECKER 1":
                                        //        status = "4";
                                        //        break;
                                        //    case "ENDORSED TO CHECKER 2":
                                        //        status = "6";
                                        //        break;
                                        //    case "ENDORSED TO APPROVER":
                                        //        status = "8";
                                        //        break;
                                        //    default:
                                        //        break;
                                        //}
                                        //OTBL.UpdateOTRecord(PopulateDR(status, txtControlNo.Text), dal);
                                        //OTBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        //if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        //{
                                        //    EmailNotificationBL EMBL = new EmailNotificationBL();
                                        //    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
                                        //    EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                        //    EMBL.InsertInfoForNotification(txtControlNo.Text
                                        //                                  , Session["userLogged"].ToString()
                                        //                                  , dal);
                                        //}
                                        ////MenuLog
                                        //SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                        //restoreDefaultControls();
                                        //MessageBox.Show("Successfully disapproved transaction.");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Enter remarks for disapproval.");
                                    }
                                    break;
                                default:
                                    break;
                            }
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

                        if (flagSuccessful)
                            restoreDefaultControls();
                    }
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Clear / Cancel / Disapprove

    protected void btnZ_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
                {
                        //string errMsg1 = checkEntry1();
                        //if (errMsg1.Equals(string.Empty))
                        //{
                            //string err = checkRequiredFields();
                    
                            //if (checkRequiredFields() == string.Empty)
                            //{
                            using (DALHelper dal = new DALHelper())
                            {
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransactionSnapshot();

                                    DataTable dtResult;
                                    DataRow[] drArrRows;
                                    switch (btnZ.Text.ToUpper())
                                    {
                                        case "SAVE":
                                            //Get Filler Codes
                                            string filler1 = string.Empty;
                                            string filler2 = string.Empty;
                                            string filler3 = string.Empty;
                                            if (txtFiller1.Text != string.Empty)
                                            {
                                                DataSet dsFiller1 = getFillerCode(txtFiller1.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller1.Text + "'", "Eot_Filler01"));
                                                if (dsFiller1.Tables[0].Rows.Count > 0)
                                                    filler1 = dsFiller1.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                            }
                                            if (txtFiller2.Text != string.Empty)
                                            {
                                                DataSet dsFiller2 = getFillerCode(txtFiller2.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller2.Text + "'", "Eot_Filler02"));
                                                if (dsFiller2.Tables[0].Rows.Count > 0)
                                                    filler2 = dsFiller2.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                            }
                                            if (txtFiller3.Text != string.Empty)
                                            {
                                                DataSet dsFiller3 = getFillerCode(txtFiller3.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller3.Text + "'", "Eot_Filler03"));
                                                if (dsFiller3.Tables[0].Rows.Count > 0)
                                                    filler3 = dsFiller3.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                            }

                                            dtResult = dal.ExecuteDataSet(string.Format("EXEC CreateOvertimeRecord '{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}' "
                                                                                                , txtControlNo.Text
                                                                                                , txtEmployeeId.Text.ToString()
                                                                                                , dtpOTDate.Date.ToString("MM/dd/yyyy")
                                                                                                , ddlOTType.SelectedValue.ToString()
                                                                                                , txtOTStartTime.Text.ToString().Replace(":", "")
                                                                                                , txtOTEndTime.Text.ToString().Replace(":", "")
                                                                                                , txtOTHours.Text.ToString()
                                                                                                , txtReason.Text.ToString().ToUpper()
                                                                                                , "1" //Status=NEW
                                                                                                , Session["userLogged"].ToString() 
                                                                                                , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                                                , "WFOTENTRY"
                                                                                                , false //No Email Notification here
                                                                                                , txtJobCode.Text.ToString().ToUpper()
                                                                                                , txtClientJobName.Text.ToString().ToUpper()
                                                                                                , filler1.ToUpper()
                                                                                                , filler2.ToUpper()
                                                                                                , filler3.ToUpper())).Tables[0];
                                            drArrRows = dtResult.Select("Result = 1");
                                            if (drArrRows.Length > 0)
                                            {
                                                SystemMenuLogBL.InsertAddLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                                txtControlNo.Text = drArrRows[0]["ControlNo"].ToString();
                                                txtStatus.Text = "NEW";
                                                enableButtons();
                                                if (hfSaved.Value.Equals("0"))
                                                {
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("New transaction saved.");
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Transaction updated.");
                                                }
                                                hfPrevEntry.Value = changeSnapShot();
                                            }
                                            else
                                            {
                                                SystemMenuLogBL.InsertAddLog("WFOTENTRY", false, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                                drArrRows = dtResult.Select("Result <> 1");
                                                string strErrorMsg = "Please fix the ff. error(s):\n\n";
                                                for (int x = 0; x < drArrRows.Length; x++)
                                                {
                                                    strErrorMsg += string.Format("{0}. {1}\n", x + 1, drArrRows[x]["Message"]);
                                                }
                                                MessageBox.Show(strErrorMsg);
                                            }
                                            /*
                                            string errMsg2 = checkEntry2(dal);
                                            if (errMsg2.Equals(string.Empty))
                                            {
                                                if (hfSaved.Value.Equals("0"))
                                                {
                                                    string controlNo = CommonMethods.GetControlNumber("OVERTIME");
                                                    if (controlNo.Trim().Equals(string.Empty))
                                                    {
                                                        MessageBox.Show("Transaction control for Overtime was not created.\nSave again or relogin.");
                                                    }
                                                    else
                                                    {
                                                        DataRow dr = PopulateDR("1", controlNo);
                                                        OTBL.CreateOTRecord(dr, dal);
                                                        txtControlNo.Text = controlNo;
                                                        txtStatus.Text = "NEW";
                                                        //MenuLog
                                                        SystemMenuLogBL.InsertAddLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                                        enableButtons();
                                                        hfSaved.Value = "1";
                                                        MessageBox.Show("New transaction saved.");
                                                    }
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", txtControlNo.Text);
                                                    OTBL.UpdateOTRecordSave(dr, dal);
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("Transaction updated.");
                                                }
                                                hfPrevEntry.Value = changeSnapShot();
                                            }
                                            else
                                            {
                                                MessageBox.Show(errMsg2);
                                            }
                                             */
                                            break;
                                        case "RETURN TO EMPLOYEE":
                                            if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                            {
                                                dtResult = dal.ExecuteDataSet(string.Format("EXEC ReturnWFTransaction '{0}', '{1}', '{2}', 'OT', '{3}' "
                                                                                                , txtControlNo.Text
                                                                                                , Session["userLogged"].ToString()
                                                                                                , txtRemarks.Text.Trim().ToUpper()
                                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                                drArrRows = dtResult.Select("Result = 1");
                                                if (drArrRows.Length > 0)
                                                {
                                                    SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                                    restoreDefaultControls();
                                                    MessageBox.Show("Successfully returned transaction.");
                                                }
                                                else
                                                {
                                                    drArrRows = dtResult.Select("Result <> 1");
                                                    if (drArrRows.Length > 0)
                                                        MessageBox.Show(drArrRows[0]["Message"].ToString());
                                                }

                                                //OTBL.UpdateOTRecord(PopulateDR("1", txtControlNo.Text), dal);
                                                //OTBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                                //if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                                //{
                                                //    EmailNotificationBL EMBL = new EmailNotificationBL();
                                                //    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
                                                //    EMBL.ActionProperty = EmailNotificationBL.Action.RETURN;
                                                //    EMBL.InsertInfoForNotification(txtControlNo.Text
                                                //                                  , Session["userLogged"].ToString()
                                                //                                  , dal);
                                                //}
                                                ////MenuLog
                                                //SystemMenuLogBL.InsertEditLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                                ////SystemMenuLogBL.InsertAddLog("WFOTENTRY", true, txtEmployeeId.Text.ToString().ToUpper());
                                                //restoreDefaultControls();
                                                //MessageBox.Show("Successfully returned transaction.");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Enter remarks for action.");
                                            }
                                            break;
                                        default:
                                            break;
                                    }
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
                        //}
                        //else
                        //{
                        //    MessageBox.Show(err);
                        //}
                     //}
                    //else
                    //{
                    //    MessageBox.Show(errMsg1);
                    //}
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Save / Return To Employee
    #endregion

    #region Methods
    private void initializeEmployee()
    {
        DataSet ds = new DataSet();
        string sql = @"  SELECT Emt_EmployeeId [ID No]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname] 
	                          , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	                          , ISNULL(Dcm_Departmentdesc, '') [Department]
                           FROM T_EmployeeMaster
	                       LEFT JOIN T_DepartmentCodeMaster
	                         ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                          THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                          ELSE ''
							                          END
                          WHERE Emt_EmployeeId = @EmployeeId";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString());
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
        lblNickName.Text = Resources.Resource._3RDINFO;
        if (!CommonMethods.isEmpty(ds))
        {
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            //txtNickname.Text = ds.Tables[0].Rows[0]["Nickname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "OVERTIME", "WFOTENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpOTDate.Date = DateTime.Now;
            //Default Parameter
            //dtpOTDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
            //Andre added for use MINPASTPRD paramter
            dtpOTDate.MinDate = CommonMethods.getMinimumDateOfFiling();
            //END
            dtpOTDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
        //hfOTFRACTION.Value = CommonMethods.getParamterValue("OTFRACTION").ToString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        initializeShift(false);
        OTBL.initializeOTTypes(ddlOTType, false);
        enableControls();
        enableButtons();
        showOptionalFields();
    }

    private void enableControls()
    {
        System.Web.UI.HtmlControls.HtmlImage cal = new System.Web.UI.HtmlControls.HtmlImage();
        cal = (System.Web.UI.HtmlControls.HtmlImage)dtpOTDate.Controls[2];
        switch (txtStatus.Text.ToUpper())
        { 
            case "":
                //dtpOTDate.Enabled = true;
                cal.Disabled = false;

                ddlOTType.Enabled = true;
                txtOTStartTime.ReadOnly = false;
                txtOTHours.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
                //{
                //    txtOTHours.ReadOnly = false;
                //    txtOTEndTime.ReadOnly = true;

                //    txtOTHours.BackColor = System.Drawing.Color.White;
                //    txtOTEndTime.BackColor = System.Drawing.Color.Gainsboro;
                //}
                //else
                //{
                //    txtOTHours.ReadOnly = true;
                //    txtOTEndTime.ReadOnly = false;

                //    txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                //    txtOTEndTime.BackColor = System.Drawing.Color.White;
                //}

                txtOTStartTime.BackColor = System.Drawing.Color.White;
                txtOTHours.BackColor = System.Drawing.Color.White;
               
                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                //dtpOTDate.Enabled = true;
                cal.Disabled = false;

                ddlOTType.Enabled = true;
                txtOTStartTime.ReadOnly = false;
                txtOTHours.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
                //{
                //    txtOTHours.ReadOnly = false;
                //    txtOTEndTime.ReadOnly = true;

                //    txtOTHours.BackColor = System.Drawing.Color.White;
                //    txtOTEndTime.BackColor = System.Drawing.Color.Gainsboro;
                //}
                //else
                //{
                //    txtOTHours.ReadOnly = true;
                //    txtOTEndTime.ReadOnly = false;

                //    txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                //    txtOTEndTime.BackColor = System.Drawing.Color.White;
                //}

                txtOTStartTime.BackColor = System.Drawing.Color.White;
                txtOTHours.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                //dtpOTDate.Enabled = false;
                cal.Disabled = true;

                ddlOTType.Enabled = false;
                txtOTStartTime.ReadOnly = true;
                txtOTHours.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
                //{
                //    txtOTHours.ReadOnly = false;
                //    txtOTEndTime.ReadOnly = true;

                //    txtOTHours.BackColor = System.Drawing.Color.White;
                //    txtOTEndTime.BackColor = System.Drawing.Color.Gainsboro;
                //}
                //else
                //{
                //    txtOTHours.ReadOnly = true;
                //    txtOTEndTime.ReadOnly = false;

                //    txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                //    txtOTEndTime.BackColor = System.Drawing.Color.White;
                //}

                txtOTStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                //dtpOTDate.Enabled = false;
                cal.Disabled = true;

                ddlOTType.Enabled = false;
                txtOTStartTime.ReadOnly = true;
                txtOTHours.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
                //{
                //    txtOTHours.ReadOnly = false;
                //    txtOTEndTime.ReadOnly = true;

                //    txtOTHours.BackColor = System.Drawing.Color.White;
                //    txtOTEndTime.BackColor = System.Drawing.Color.Gainsboro;
                //}
                //else
                //{
                //    txtOTHours.ReadOnly = true;
                //    txtOTEndTime.ReadOnly = false;

                //    txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                //    txtOTEndTime.BackColor = System.Drawing.Color.White;
                //}

                txtOTStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                //dtpOTDate.Enabled = false;
                cal.Disabled = true;

                ddlOTType.Enabled = false;
                txtOTStartTime.ReadOnly = true;
                txtOTHours.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
                //{
                //    txtOTHours.ReadOnly = false;
                //    txtOTEndTime.ReadOnly = true;

                //    txtOTHours.BackColor = System.Drawing.Color.White;
                //    txtOTEndTime.BackColor = System.Drawing.Color.Gainsboro;
                //}
                //else
                //{
                //    txtOTHours.ReadOnly = true;
                //    txtOTEndTime.ReadOnly = false;

                //    txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                //    txtOTEndTime.BackColor = System.Drawing.Color.White;
                //}

                txtOTStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtOTHours.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            default:
                break;
        }
    }

    private void enableButtons()
    {
        switch (txtStatus.Text.ToUpper())
        {
            case "":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = false;

                btnZ.Text = "SAVE";
                btnY.Text = "CLEAR";
                btnX.Text = "ENDORSE TO CHECKER 1";
                break;
            case "NEW":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = true;

                btnZ.Text = "SAVE";
                btnY.Text = "CANCEL";
                btnX.Text = "ENDORSE TO CHECKER 1";
                break;
            case "ENDORSED TO CHECKER 1":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = true;

                btnZ.Text = "RETURN TO EMPLOYEE";
                btnY.Text = "DISAPPROVE";
                btnX.Text = "ENDORSE TO CHECKER 2";
                break;
            case "ENDORSED TO CHECKER 2":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = true;

                btnZ.Text = "RETURN TO EMPLOYEE";
                btnY.Text = "DISAPPROVE";
                btnX.Text = "ENDORSE TO APPROVER";
                break;
            case "ENDORSED TO APPROVER":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = true;

                btnZ.Text = "RETURN TO EMPLOYEE";
                btnY.Text = "DISAPPROVE";
                btnX.Text = "APPROVE";
                break;
            default:
                break;
        }
    }

    private void showOptionalFields()
    {
        try
        {
            tbrJobCode.Visible = methods.GetProcessControlFlag("OVERTIME", "WFOTJOB");
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            tbrJobCode.Visible = false;
        }
        DataSet ds = new DataSet();
        string sql = @"DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                      FROM T_ProcessControlMaster
                                                     WHERE Pcm_SystemID = 'OVERTIME'
                                                       AND Pcm_ProcessID = 'AUTOROUTE'
                                                       AND Pcm_Status = 'A' )
                       SELECT IIF(Cfl_ColName = 'Eot_Filler01' AND @AUTOROUTE = 1, '', Cfl_ColName) Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
                          AND Cfl_Status = 'A'
                          AND 1 = IIF(Cfl_ColName <> 'Eot_Filler01', 1, IIF(@AUTOROUTE = 1, 0, 1))";
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
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
            for(int i = 0; i <ds.Tables[0].Rows.Count; i++)
            {
                try
                {
                    switch (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper())
                    {
                        case "EOT_FILLER01":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller1.Visible = true;
                                lblFiller1.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller1.Attributes.Add("OnClick", "javascript:return lookupGenericFiller(" + "'txtFiller1','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller1.Visible = false;
                            }
                            break;
                        case "EOT_FILLER02":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller2.Visible = true;
                                lblFiller2.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller2.Attributes.Add("OnClick", "javascript:return lookupGenericFiller(" + "'txtFiller2','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller2.Visible = false;
                            }
                            break;
                        case "EOT_FILLER03":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller3.Visible = true;
                                lblFiller3.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller3.Attributes.Add("OnClick", "javascript:return lookupGenericFiller(" + "'txtFiller3','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller3.Visible = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
        }
    }

    private void initializeShift(bool isLoaded)
    {
        DataSet ds = new DataSet();

		ds = CommonMethods.getEmployeeShift(txtEmployeeId.Text, dtpOTDate.Date.ToString("MM/dd/yyyy"));
		if(Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
		{
	        if (dtpOTDate.Date > DateTime.Now)
	        {
                ds = CommonMethods.getDefaultShiftCHIYODA(txtEmployeeId.Text, dtpOTDate.Date.ToString("MM/dd/yyyy"));
	        }
		}

        if (!CommonMethods.isEmpty(ds))
        {
            txtOTShift.Text = "[" + ds.Tables[0].Rows[0]["Code"].ToString() + "] "
                            + ds.Tables[0].Rows[0]["TimeIn"].ToString().Insert(2, ":") + "-"
                            + ds.Tables[0].Rows[0]["BreakStart"].ToString().Insert(2, ":") + "  "
                            + ds.Tables[0].Rows[0]["BreakEnd"].ToString().Insert(2, ":") + "-"
                            + ds.Tables[0].Rows[0]["TimeOut"].ToString().Insert(2, ":");
            txtDayCode.Text = ds.Tables[0].Rows[0]["DayCode"].ToString();
            hfShiftType.Value = ds.Tables[0].Rows[0]["Type"].ToString();
            hfShiftHours.Value = ds.Tables[0].Rows[0]["Hours"].ToString();
            hfShiftPaid.Value = ds.Tables[0].Rows[0]["PaidBreak"].ToString();
            hfI1.Value = ds.Tables[0].Rows[0]["TimeIn"].ToString();
            hfO1.Value = ds.Tables[0].Rows[0]["BreakStart"].ToString();
            hfI2.Value = ds.Tables[0].Rows[0]["BreakEnd"].ToString();
            hfO2.Value = ds.Tables[0].Rows[0]["TimeOut"].ToString();

            hfShiftCode.Value = ds.Tables[0].Rows[0]["Code"].ToString();
            // Clear OT Hours and OT End Time text boxes.. Additional by RANDY
            txtOTHours.Text = "";
            txtOTEndTime.Text = "";

            if (!isLoaded)
            {
                if (ds.Tables[0].Rows[0]["DayCode"].ToString().Contains("REG"))
                {
                    decimal otStartPad = MethodsLibrary.Methods.GetParameterValue("OTSTARTPAD");
                    int o2MIN = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
                    txtOTStartTime.Text = (Convert.ToInt32(o2MIN + otStartPad) / 60).ToString().PadLeft(2, '0') 
                                        + ":"
                                        + (Convert.ToInt32(o2MIN + otStartPad) % 60).ToString().PadLeft(2, '0');
                    //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
                }
                else
                {
                   // if (!Convert.ToBoolean(Resources.Resource.FEPSPECIFIC))
                    //{
                        if (Convert.ToBoolean(Resources.Resource.LOADOTDEFAULTSTARTTIME))
                        {
                            txtOTStartTime.Text = hfI1.Value.Insert(2, ":");
                        }
                        else
                        {
                            if (!ds.Tables[0].Rows[0]["TimeIn1"].ToString().Equals("0000"))
                            {
                                txtOTStartTime.Text = CommonMethods.roundupTime(ds.Tables[0].Rows[0]["TimeIn1"].ToString(), Convert.ToInt32(CommonMethods.getParamterValue("OTFRACTION")));
                            }
                            else if (!ds.Tables[0].Rows[0]["TimeIn2"].ToString().Equals("0000"))
                            {
                                txtOTStartTime.Text = CommonMethods.roundupTime(ds.Tables[0].Rows[0]["TimeIn2"].ToString(), Convert.ToInt32(CommonMethods.getParamterValue("OTFRACTION")));
                            }
                            else
                            {
                                txtOTStartTime.Text = string.Empty;
                            }

                        }
                    //}
                    //else
                    //{
                    //    txtOTStartTime.Text = string.Empty;
                    //}
                }
            }
        }
    }

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        #region Start Time
        if (txtOTStartTime.Text.Length < 5)
        {
            err += "\nStart Time invalid format.(hh:mm)";
        }
        else
        {
            try
            {
                int x = Convert.ToInt32(txtOTStartTime.Text.Substring(0, 2));
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtOTStartTime.Text.Substring(3, 2));
                decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) / 60);
                temp = Math.Round(temp, 2);

                if(Convert.ToInt32(txtOTStartTime.Text.Substring(3, 2)) > 59)
                {
                    err += "\nStart Time minutes is invalid";
                }
                else if(temp > Convert.ToDecimal(71.98))
                {
                    err += "\nStart Time maximum time exceeded.(up to 71:59)";
                }
                else if (!txtOTStartTime.Text.Contains(":"))
                {
                    err += "\nStart Time invalid format.(hh:mm)";
                }
            }
            catch
            {
                err += "\nStart Time invalid format.(hh:mm)";
            }
        }
        #endregion
        //if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
        //{
            if (err.Equals(string.Empty))
            {
                // Randy added (31-JUL-2012): validation of the OT hours
                computeEndTime();
            }
            //else rollback update
            //{
            //    err += checkRequiredFields();
            //}
        //}
        //else
        //{

        //    if (err.Equals(string.Empty))
        //    {
        //        computeOTHours();
        //    }
        //}

        #region End Time
        //if (txtOTEndTime.Text != string.Empty)
        //{
            if (txtOTEndTime.Text.Length < 5)
            {
                err += "\nEnd Time invalid format.(hh:mm)";
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(txtOTEndTime.Text.Substring(0, 2));
                    DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtOTEndTime.Text.Substring(3, 2));
                    decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtOTEndTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTEndTime.Text.Substring(3, 2)) / 60);
                    temp = Math.Round(temp, 2);

                    if (Convert.ToInt32(txtOTEndTime.Text.Substring(3, 2)) > 59)
                    {
                        err += "\nEnd Time minutes is invalid";
                    }
                    else if (temp > Convert.ToDecimal(72.00))
                    {
                        err += "\nEnd Time maximum time exceeded.(up to 72:00)";
                    }
                    else if (!txtOTEndTime.Text.Contains(":"))
                    {
                        err += "\nEnd Time invalid format.(hh:mm)";
                    }
                }
                catch
                {
                    err += "\nEnd Time invalid format.(hh:mm)";
                }
            }
        //}
        #endregion
        #region Start Time and End Time and Overtime hours Perth Adde 06/23/2012
        if (this.txtOTStartTime.Text.Trim() != string.Empty
            && this.txtOTEndTime.Text.Trim() != string.Empty
            && this.txtOTStartTime.Text.Trim() == this.txtOTEndTime.Text.Trim())
        {
            err += "\nStart Time must not be equal to End Time";
        }
        //if (txtOTHours.Text != string.Empty)
        //{
            if (Convert.ToDecimal(txtOTHours.Text) <= 0)
            {
                err += "\nOvertime hours must be grater than  Zero";
            }
        //}
        #endregion
        #region Overtime Hours
        //if (txtOTHours.Text != string.Empty)
        //{
            if (Convert.ToDecimal(txtOTHours.Text) > Convert.ToDecimal(72.00))
            {
                err += "\nOvertime hours excceed maximum hours.(72.00)";
            }
        //}
        #endregion
        #region Start and End time validation after checking all fomat is correct.
        if (err.Equals(string.Empty))
        {
            int I1 = (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(2, 2));
            int O1 = (Convert.ToInt32(hfO1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO1.Value.Substring(2, 2));
            int I2 = (Convert.ToInt32(hfI2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI2.Value.Substring(2, 2));
            int O2 = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
            int PaidBreak = Convert.ToInt32(hfShiftPaid.Value);//Already in minutes format
            int OTStart = (Convert.ToInt32(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtOTStartTime.Text.Substring(3, 2));
            int OTEnd = (Convert.ToInt32(txtOTEndTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtOTEndTime.Text.Substring(3, 2));
            if (hfShiftType.Value.Equals("G"))
            {
                //if (OTEnd < OTStart)
                //    OTEnd += 1440;
                if (O1 < I1)
                    O1 += 1440;
                if (I2 < I1)
                    I2 += 1440;
                if (O2 < I1)
                    O2 += 1440;
                if (ddlOTType.SelectedValue.Equals("P") && txtDayCode.Text.Contains("REG"))
                {
                    if (OTStart < I1)
                        OTStart += 1440;
                    if (OTEnd < I1)
                        OTEnd += 1440;
                    else
                        if (OTStart > OTEnd)
                            OTStart -= 1440;
                }
            }
            if (ddlOTType.SelectedValue.Equals("P"))//Validate Post Overtime
            {
                //Andre added for CHIYODA: 20110218 filing of OVERTIME in break
                if (OTStart >= O1 && OTStart <= I2 && OTEnd >= O1 && OTEnd <= I2)
                { 
                    //This is allowed. placed code here to by pass next trapping in ELSE IF
                }//End
                else if (OTStart < O2 && txtDayCode.Text.ToUpper().Contains("REG"))
                {
                    err += "\nStart Time cannot be within shift in POST Overtime on regular days.";
                }
            }
            else if (ddlOTType.SelectedValue.Equals("A"))//Overtime Type is Advance
            {
                //for HOGP: just removed trapping for ADVANCE filing for NON-REGULAR days
                if (OTEnd > I1 && txtDayCode.Text.ToUpper().Contains("REG"))
                {
                    err += "\nEnd Time cannot be greater than shift Time In for ADVANCE Overtime";
                }
                else if (OTStart >= I1 && txtDayCode.Text.ToUpper().Contains("REG"))
                {
                    err += "\nStart Time cannot be greater or equal than shift Time In for ADVANCE Overtime";
                }

                //Andre commented: 20101013 not applicable for HOGP
                //Andre enabled: 20110218
                if (err.Equals(string.Empty))
                {
                    if (!txtDayCode.Text.Contains("REG"))
                    {
                        err += "\nCannot file ADVANCE Overtime on non-regular days.";
                    }
                    else if (OTEnd > I1 && txtDayCode.Text.ToUpper().Contains("REG"))
                    {
                        err += "\nEnd TIme cannot be greater than shift Time In for ADVANCE Overtime";
                    }
                    else if (OTStart >= I1 && txtDayCode.Text.ToUpper().Contains("REG"))
                    {
                        err += "\nStart Time cannot be greater or equal than shift Time In for ADVANCE Overtime";
                    }
                }
            }
            else //Overtime Type is Mid
            {
                if (OTStart < O1)
                {
                    err += "\nStart Time cannot be lesser than shift break start for MID Overtime";
                } 
                if (OTStart >= I2)
                {
                    err += "\nStart Time cannot be greater or equal than shift break end for MID Overtime";
                }
                if (OTEnd <= O1)
                {
                    err += "\nEnd Time cannot be lesser or equal than shift break start for MID Overtime";
                }
                if (OTEnd > I2)
                {
                    err += "\nEnd Time cannot be greater than shift break end for MID Overtime";
                }
            }

            if(OTStart > OTEnd && err.Equals(string.Empty))
            {
                err += "Start time cannot be greater then endtime.";
            }
        }
        #endregion
        #region Reason
        if (txtReason.Text.Length > 200)
        {
            err += "\nReason exceeds maximum characters allowed.(" + txtReason.Text.Length.ToString() + "/200)";
        } 
        #endregion
        #region Remarks
        if (txtRemarks.Text.Length > 200)
        {
            err += "\nRemarks exceeds maximum characters allowed.(" + txtRemarks.Text.Length.ToString() + "/200)";
        }
        #endregion
        return err;
    }

    private string checkEntry2(DALHelper dal)//Validation from DB parameters/data
    {
        DataSet ds = new DataSet();
        decimal otHours = 0;
        string err = string.Empty;
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "OVERTIME", "WFOTENTRY");
        bool canFileOT = userGrant.canGenerate();
        #region SQL
        #region Overtime Conflicts
        string sqlConfilcts = @"declare @start datetime
                                declare @end datetime

                                SET @start = dbo.getDatetimeFormatV2(@startTime,@overtimeDate,@overtimeType,@shiftType,@shiftStart)
                                SET @end = dbo.getDatetimeFormatV2(@endTime,@overtimeDate,@overtimeType,@shiftType,@shiftEnd)

                                SELECT Eot_ControlNo [Control No]
                                     , Convert(varchar(10),Eot_OvertimeDate,101) [OT Date]
                                     , Eot_StartTime [Start Time]
                                     , Eot_EndTime [End Time]
                                     , ISNULL(SCM1.Scm_ShiftTimeIn, SCM2.Scm_ShiftTimeIn) [Time In]
                                  FROM T_EmployeeOvertime

                                  LEFT JOIN T_EmployeeLogLedger ELL1
                                    ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL1.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM1
                                    ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode

                                  LEFT JOIN T_EmployeeLogLedgerHist ELL2
                                    ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL2.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM2
                                    ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode

                                 WHERE Eot_EmployeeId = @EmployeeId
                                   AND Eot_ControlNo <> @CurrentControlNo
                                   AND Eot_Status IN ('1','3','5','7', '9', 'A')
                                   AND Eot_OvertimeType IN @OTTypes
                                   AND (   ( @start >= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                              , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                              , @TYPECHECK 
                                                                              , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                              , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
                                         AND @start < dbo.getDatetimeFormatV2( Eot_EndTime
                                                                             , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                             , @TYPECHECK 
                                                                             , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                             , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
                                        OR ( @end > dbo.getDatetimeFormatV2( Eot_StartTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , @TYPECHECK 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
                                         AND @end <= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , @TYPECHECK 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
	                                    OR (     @start <= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                  , @TYPECHECK 
                                                                                  , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                  , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
	                                         AND  @end >= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                                 , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                 , @TYPECHECK 
                                                                                 , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                 , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
                                       )
                                 UNION   

                                 SELECT Eot_ControlNo [Control No]
                                     , Convert(varchar(10),Eot_OvertimeDate,101) [OT Date]
                                     , Eot_StartTime [Start Time]
                                     , Eot_EndTime [End Time]
                                     , ISNULL(SCM1.Scm_ShiftTimeIn, SCM2.Scm_ShiftTimeIn) [Time In]
                                  FROM T_EmployeeOvertimeHist

                                  LEFT JOIN T_EmployeeLogLedger ELL1
                                    ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL1.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM1
                                    ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode

                                  LEFT JOIN T_EmployeeLogLedgerHist ELL2
                                    ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL2.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM2
                                    ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode

                                 WHERE Eot_EmployeeId = @EmployeeId
                                   AND Eot_ControlNo <> @CurrentControlNo
                                   AND Eot_Status IN ('1','3','5','7', '9', 'A')
                                   AND Eot_OvertimeType IN @OTTypes
                                   AND (   ( @start >= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                              , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                              , @TYPECHECK 
                                                                              , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                              , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
                                         AND @start < dbo.getDatetimeFormatV2( Eot_EndTime
                                                                             , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                             , @TYPECHECK 
                                                                             , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                             , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
                                        OR ( @end > dbo.getDatetimeFormatV2( Eot_StartTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , @TYPECHECK 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
                                         AND @end <= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , @TYPECHECK 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
	                                    OR (     @start <= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                  , @TYPECHECK 
                                                                                  , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                  , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn))
	                                         AND  @end >= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                                 , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                 , @TYPECHECK
                                                                                 , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                 , ISNULL(SCM2.Scm_ShiftTimeOut, SCM1.Scm_ShiftTimeOut))
	                                       )
                                       )
                                 ORDER BY [OT Date]";
        #endregion
        #region Duplicate Entry
        string sqlDuplicates = @"
SELECT 
	Convert(varchar(20), Eot_OvertimeDate, 101) [Eot_OvertimeDate]
	, Eot_ControlNo
FROM T_EmployeeOvertime
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_StartTime = @startTime
	AND Eot_EndTime = @endTime
	AND Eot_Status IN ('1','3','5','7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo

UNION ALL

SELECT 
	Convert(varchar(20), Eot_OvertimeDate, 101) [Eot_OvertimeDate]
	, Eot_ControlNo
FROM T_EmployeeOvertimeHist
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_StartTime = @startTime
	AND Eot_EndTime = @endTime
	AND Eot_Status IN ('1','3','5','7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo
        ";
        #endregion
        #region LogLedger Exist
        string sqlLogLedgerExist = string.Format(@"
SELECT Ell_EmployeeID
FROM T_EmployeeLogLedger
WHERE Ell_EmployeeID = @EmployeeId
AND Ell_ProcessDate = @overtimeDate
UNION
SELECT Ell_EmployeeID
FROM T_EmployeeLogLedgerHist
WHERE Ell_EmployeeID = @EmployeeId
AND Ell_ProcessDate = @overtimeDate"
            );
        #endregion
        #region Parameters
        string sqlParameters = @"   SELECT Pmt_ParameterId
                                         , Pmt_NumericValue
                                      FROM T_ParameterMaster
                                     WHERE Pmt_ParameterId IN ('MAXOTHR','MAXOTHRRG','MINOTHR','OTFRACTION','STRTOTFRAC', 'OTSTARTPAD')";
        #endregion
        #region Overtime Hours Revised by Perth 04/25/2013 retrieve the ot hours using only date
//        string sqlOvertimeHours = @"declare @tempTable as TABLE
//                                    (
//	                                    total decimal(8,2)
//                                    )
//
//                                    INSERT INTO @tempTable
//                                    SELECT ISNULL( SUM(CASE WHEN (Convert( varchar(10)
//                                                                         , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                  , Eot_OvertimeType 
//                                                                                                  , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                         THEN SCM2.Scm_ScheduleType
//                                                                                                         ELSE SCM1.Scm_ScheduleType
//                                                                                                     END 
//                                                                                                  , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                         THEN SCM2.Scm_ShiftTimeIn
//                                                                                                         ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                     END )
//                                                                                                  , 101) < @overtimeDate)
//                                                            THEN (datediff( MINUTE
//                                                                         , dbo.getDatetimeFormatV2( '0000'
//                                                                                                , Convert(varchar(10),@overtimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , '0000' )
//                                                                         , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                   END )) / 60.00)
//                                                            ELSE
//                                                            CASE WHEN (Convert(varchar(10),dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                , Eot_OvertimeType 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                                   END 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                   END ),101) > @overtimeDate)
//                                                                 THEN Eot_OvertimeHour - (datediff( MINUTE
//                                                                                                 , dbo.getDatetimeFormatV2( '0000'
//                                                                                                                        , Convert(varchar(10),dateadd(day,1,@overtimeDate),101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , '0000' )
//                                                                                                 , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                        , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                               ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                           END) ) /60.00)
//                                                                 ELSE Eot_OvertimeHour
//                                                             END
//                                                        END), 0) [Total]
//                                      FROM T_EmployeeOvertime
//
//                                      LEFT JOIN T_EmployeeLogLedger ELL1
//                                        ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL1.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM1
//                                        ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode
//                                      LEFT JOIN T_EmployeeLogLedgerHist ELL2
//                                        ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL2.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM2
//                                        ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode
//
//                                     WHERE (Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate
//                                        OR Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate)
//                                       AND Eot_EmployeeId = @EmployeeId
//                                       AND Eot_Status IN ('1','3','5','7','9','A')
//                                       AND Eot_ControlNo <> @CurrentControlNo
//									   AND ISNULL(ELL2.Ell_DayCode, ELL1.Ell_DayCode) LIKE 'REG%' 
//
//                                    UNION
//
//                                    SELECT ISNULL( SUM(CASE WHEN (Convert( varchar(10)
//                                                                         , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                  , Eot_OvertimeType 
//                                                                                                  , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                         THEN SCM2.Scm_ScheduleType
//                                                                                                         ELSE SCM1.Scm_ScheduleType
//                                                                                                     END 
//                                                                                                  , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                         THEN SCM2.Scm_ShiftTimeIn
//                                                                                                         ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                     END )
//                                                                                                  , 101) < @overtimeDate)
//                                                            THEN (datediff( MINUTE
//                                                                         , dbo.getDatetimeFormatV2( '0000'
//                                                                                                , Convert(varchar(10),@overtimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , '0000')
//                                                                         , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                   END )) / 60.00)
//                                                            ELSE
//                                                            CASE WHEN (Convert(varchar(10),dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                , Eot_OvertimeType 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                                   END 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                   END ),101) > @overtimeDate)
//                                                                 THEN Eot_OvertimeHour - (datediff( MINUTE
//                                                                                                 , dbo.getDatetimeFormatV2( '0000'
//                                                                                                                        , Convert(varchar(10),dateadd(day,1,@overtimeDate),101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , '0000' )
//                                                                                                 , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                        , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                               ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                           END) ) /60.00)
//                                                                 ELSE Eot_OvertimeHour
//                                                             END
//                                                        END), 0) [Total]
//                                      FROM T_EmployeeOvertimeHist
//
//                                      LEFT JOIN T_EmployeeLogLedger ELL1
//                                        ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL1.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM1
//                                        ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode
//                                      LEFT JOIN T_EmployeeLogLedgerHist ELL2
//                                        ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL2.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM2
//                                        ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode
//
//                                     WHERE (Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate
//                                        OR Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate)
//                                       AND Eot_EmployeeId = @EmployeeId
//                                       AND Eot_Status IN ('1','3','5','7','9','A')
//                                       AND Eot_ControlNo <> @CurrentControlNo
//									   AND ISNULL(ELL2.Ell_DayCode, ELL1.Ell_DayCode) LIKE 'REG%' 
//									   
//                                    SELECT SUM(Total)
//                                    FROM @tempTable";
        string sqlOvertimeHours = @"
SELECT
	COALESCE(sum(Eot_OvertimeHour), 0.00) [Total Hours]
FROM (
SELECT 
	Eot_OvertimeHour
FROM T_EmployeeOvertime
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo
UNION ALL
SELECT 
	Eot_OvertimeHour
FROM T_EmployeeOvertimeHist
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo
	) AS TABLETEMP
        ";
        string sqlOvertimeHourRecords = @"
SELECT 
	'[' + Eot_ControlNo 
		+ '] ' + LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime, 2)
		+ ' - ' + LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2)
		+ ' ' + CONVERT(VARCHAR(20), Eot_OvertimeHour)
        + CASE WHEN Eot_OvertimeHour <= 1
			THEN ' HOUR'
			ELSE ' HOURS'
			END [Details]
	, Eot_StartTime
FROM T_EmployeeOvertime
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo
UNION ALL
SELECT 
	'[' + Eot_ControlNo 
		+ '] ' + LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime, 2)
		+ ' - ' + LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2)
		+ ' ' + CONVERT(VARCHAR(20), Eot_OvertimeHour) 
        + CASE WHEN Eot_OvertimeHour <= 1
			THEN ' HOUR'
			ELSE ' HOURS'
			END [Details]
	, Eot_StartTime
FROM T_EmployeeOvertimeHist
WHERE Eot_EmployeeId = @EmployeeId
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
	AND Eot_ControlNo <> @CurrentControlNo
ORDER BY Eot_StartTime
";
        #endregion
        #region Company
        string sqlCompany = @"
            SELECT 
	            Ccd_CompanyCode
            FROM T_CompanyMaster
            ";
        #endregion
        ParameterInfo[] param = new ParameterInfo[9];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@CurrentControlNo", txtControlNo.Text);
        param[2] = new ParameterInfo("@overtimeDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        param[3] = new ParameterInfo("@startTime", txtOTStartTime.Text.Replace(":",""));
        param[4] = new ParameterInfo("@endTime", txtOTEndTime.Text.Replace(":", ""));
        param[5] = new ParameterInfo("@overtimeType", ddlOTType.SelectedValue);
        param[6] = new ParameterInfo("@shiftType", hfShiftType.Value.Trim());
        param[7] = new ParameterInfo("@shiftStart", hfI1.Value.Trim());
        param[8] = new ParameterInfo("@shiftEnd", hfO2.Value.Trim());

        //Perth Added 10/2/2012
        //Perth Commented 04/25/2013 not needed
        //decimal dActualOtHours = GetActualOtHours();

        //Perth Added 4/12/2013
        DateTime dtTransactStart = GetDateValue(dtpOTDate.Date.ToString("MM/dd/yyyy"), txtOTStartTime.Text.Trim(), hfI1.Value);
        DateTime dtTransactEnd = GetDateValue(dtpOTDate.Date.ToString("MM/dd/yyyy"), txtOTEndTime.Text.Trim(), hfI1.Value);

        #endregion
        if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
          || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
        {
            #region HOGP Specific For checking of Shift Start End not same to Filed OT Start End
            DataSet dsCompany = dal.ExecuteDataSet(sqlCompany, CommandType.Text);
            string CompanyCode = string.Empty;

            if (dsCompany != null
                && dsCompany.Tables != null
                && dsCompany.Tables.Count > 0
                && dsCompany.Tables[0].Rows.Count > 0)
            {
                CompanyCode = dsCompany.Tables[0].Rows[0][0].ToString().Trim().ToUpper();
            }

            if (CompanyCode == "HOGP")
            {
                sqlConfilcts = sqlConfilcts.Replace("@TYPECHECK", @"CASE WHEN ISNULL(ELL1.Ell_DayCode, ELL2.Ell_DayCode) NOT LIKE 'REG%' THEN 'A' ELSE Eot_OvertimeType END");
            }
            else
            {
                sqlConfilcts = sqlConfilcts.Replace("@TYPECHECK", @"Eot_OvertimeType");
            }
            #endregion

            if (CommonMethods.getMinuteValue(txtOTStartTime.Text.Replace(":", "")) >= CommonMethods.getMinuteValue(hfO1.Value.Replace(":", ""))
              && CommonMethods.getMinuteValue(txtOTStartTime.Text.Replace(":", "")) <= CommonMethods.getMinuteValue(hfI2.Value.Replace(":", ""))
              && CommonMethods.getMinuteValue(txtOTEndTime.Text.Replace(":", "")) >= CommonMethods.getMinuteValue(hfO1.Value.Replace(":", ""))
              && CommonMethods.getMinuteValue(txtOTEndTime.Text.Replace(":", "")) <= CommonMethods.getMinuteValue(hfI2.Value.Replace(":", ""))
              && ddlOTType.SelectedValue.Equals("M"))
            {
                ds = dal.ExecuteDataSet(sqlConfilcts.Replace("@OTTypes", "('M')"), CommandType.Text, param);
            }
            else 
            {
                ds = dal.ExecuteDataSet(sqlConfilcts.Replace("@OTTypes", "('A', 'P')"), CommandType.Text, param);
            }

            if (!CommonMethods.isEmpty(ds))
            {
                string errHeader = string.Empty;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DateTime dtTempStart = GetDateValue(
                                                ds.Tables[0].Rows[i]["OT Date"].ToString()
                                                , ds.Tables[0].Rows[i]["Start Time"].ToString().Insert(2, ":")
                                                , (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Time In"].ToString().Trim()) ? ds.Tables[0].Rows[i]["Time In"].ToString().Trim() : hfI1.Value.Trim()));
                    DateTime dtTempEnd = GetDateValue(
                                                ds.Tables[0].Rows[i]["OT Date"].ToString()
                                                , ds.Tables[0].Rows[i]["End Time"].ToString().Insert(2, ":")
                                                , (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Time In"].ToString().Trim()) ?ds.Tables[0].Rows[i]["Time In"].ToString().Trim() : hfI1.Value.Trim()));
                    if (CheckIfOverlap(dtTransactStart, dtTransactEnd, dtTempStart, dtTempEnd))
                    {
                        errHeader += @"\n    "
                             + ds.Tables[0].Rows[i]["Control No"].ToString()
                             + "  "
                             + ds.Tables[0].Rows[i]["OT Date"].ToString()
                             + "   "
                             + ds.Tables[0].Rows[i]["Start Time"].ToString().Insert(2, ":")
                             + " - "
                             + ds.Tables[0].Rows[i]["End Time"].ToString().Insert(2, ":");
                    }
                }
                if (errHeader.Trim() != string.Empty)
                {
                    err += "Overtime conflicts:" + errHeader;                    
                }
            }

            if (err.Equals(string.Empty))
            {
                ds = dal.ExecuteDataSet(sqlLogLedgerExist, CommandType.Text, param);
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    err += "Record does not exist in ledger.";
                }
            }

            if (err.Equals(string.Empty))
            {
                ds = new DataSet();
                ds = dal.ExecuteDataSet(sqlDuplicates, CommandType.Text, param);
                if (!CommonMethods.isEmpty(ds))
                {
                    err += "Duplicate Entry : ";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        err += @"\n    "
                             + ds.Tables[0].Rows[i]["Eot_OvertimeDate"].ToString()
                             + " - "
                             + ds.Tables[0].Rows[i]["Eot_ControlNo"].ToString();
                    }
                }
            }
            if (err.Equals(string.Empty))//..continue with other trappings
            {
                ds = new DataSet();
                ds = dal.ExecuteDataSet(sqlParameters, CommandType.Text);
                otHours = Convert.ToDecimal(dal.ExecuteScalar(sqlOvertimeHours, CommandType.Text,param));               
                if (!CommonMethods.isEmpty(ds))
                {
                    DataSet dsTempDetails = null;
                    string strDetail = string.Empty;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        switch (ds.Tables[0].Rows[i]["Pmt_ParameterId"].ToString().ToUpper())
                        { 
                            case "MAXOTHR":
                                if (otHours + Convert.ToDecimal(txtOTHours.Text) > Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) && !txtDayCode.Text.Equals("REG"))
                                {
                                    dsTempDetails = null;
                                    strDetail = string.Empty;
                                    dsTempDetails = dal.ExecuteDataSet(sqlOvertimeHourRecords, CommandType.Text, param);
                                    if (!CommonMethods.isEmpty(dsTempDetails))
                                    {
                                        strDetail += string.Format("\nYour current total overtime hour(s) for {0} : {1}\nDetails : ", dtpOTDate.Date.ToString("MM/dd/yyyy"), otHours);
                                        for (int x = 0; x < dsTempDetails.Tables[0].Rows.Count; x++)
                                        {
                                            strDetail += "\n - " + dsTempDetails.Tables[0].Rows[x][0].ToString().Trim();
                                        }
                                        strDetail += string.Format("\nYour remaining hours to apply : {0}", Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) - otHours);
                                    }
                                    err += "\nMaximum overtime hours for non-regular days is " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + "." + strDetail;
                                }
                                break;
                            case "MAXOTHRRG":
                                if (otHours + Convert.ToDecimal(txtOTHours.Text) > Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) && txtDayCode.Text.Equals("REG"))
                                {
                                    dsTempDetails = null;
                                    strDetail = string.Empty;
                                    dsTempDetails = dal.ExecuteDataSet(sqlOvertimeHourRecords, CommandType.Text, param);
                                    if (!CommonMethods.isEmpty(dsTempDetails))
                                    {
                                        strDetail += string.Format("\nYour current total overtime hour(s) for {0} : {1}\nDetails : ", dtpOTDate.Date.ToString("MM/dd/yyyy"), otHours);
                                        for (int x = 0; x < dsTempDetails.Tables[0].Rows.Count; x++)
                                        {
                                            strDetail += "\n - " + dsTempDetails.Tables[0].Rows[x][0].ToString().Trim();
                                        }
                                        strDetail += string.Format("\nYour remaining hours to apply : {0}", Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) - otHours);
                                    }
                                    err += "\nMaximum overtime hours for regular days is " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + "." + strDetail;
                                }
                                break;
                            case "MINOTHR"://By default if no classification is setup for the employee then it would use default MINOTHR not from extension table
                                if ((Convert.ToDecimal(txtOTHours.Text) + otHours) < Convert.ToDecimal(getMINOTHR(txtEmployeeId.Text)))
                                {
                                    //err += "\nMinimum overtime hours is " + Convert.ToString(Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) * 60) + ".";
                                    err += "\nMinimum overtime hours is " + Convert.ToString(Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"])) + ".";
                                }
                                break;
                            case "STRTOTFRAC":
                                if (Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) % Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 0)
                                {
                                    err += "\n Invalid entry in minutes. Must be divisible by " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + ".";
                                }
                                else if (Convert.ToDecimal(txtOTEndTime.Text.Substring(3, 2)) % Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 0)
                                {
                                    err += "\n Invalid entry in minutes. Must be divisible by " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + ".";
                                }
                                break;
                            case "OTFRACTION":
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 1
                                    && (Convert.ToDecimal(txtOTHours.Text) * 60) % Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 0)
                                {
                                    err += "\n Invalid entry in overtime fraction. Must be divisible by " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + ".";
                                }
                                break;
                            case "OTSTARTPAD": //Andre added 20130702. Determin if start time of transacton is valid. this is only applicable to POST OT REGULAR DAYS
                                int o2MIN = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
                                if (hfShiftType.Value.Equals("G")
                                 && o2MIN < (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(2, 2))
                                 && (Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) > 1440)
                                {
                                    o2MIN += 1440;
                                }
                                if (txtDayCode.Text.StartsWith("REG")
                                  && ddlOTType.SelectedValue.Equals("P")
                                  && (Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2))
                                   < o2MIN + Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]))
                                {
                                    err += "\n Invalid entry in overtime start time. Valid start time is "
                                        + (Convert.ToInt32(o2MIN + Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"])) / 60).ToString() 
                                        + ":"
                                        + (Convert.ToInt32(o2MIN + Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"])) % 60).ToString()
                                        + " onwards.";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (err.Equals(string.Empty))
            {
                if (CommonMethods.isMismatchCostcenterAMS(dal, txtEmployeeId.Text, dtpOTDate.Date.ToString("MM/dd/yyyy")))
                {
                    err += "Cannot proceed with transaction, there is a mismatch in the cost center setup between AMS system.";
                }
            }

            if (err.Equals(string.Empty))//..continue with other trappings
            {
                if (!OTBL.CheckHasRoute(dal, txtEmployeeId.Text, dtpOTDate.Date.ToString("MM/dd/yyyy")))
                {
                    err += "No shuttle route setup.";
                }
            }

            if (err.Equals(string.Empty))
            {
                if (MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                {
                    err += CommonMethods.GetErrorMessageForCYCCUTOFF();
                }
            }
            if (err.Equals(string.Empty))
            {
                string timeCutOff = string.Empty;
                DataSet dsParameter = new DataSet();
                DataSet dsShift = CommonMethods.getShiftInformation(hfShiftCode.Value);
                string scheduleType = string.Empty;
                string paramValue = string.Empty;
                decimal timeIn = 0;
                decimal timeOut = 0;
                int timeExt = 0;

                if (dsShift.Tables[0].Rows.Count > 0)
                {
                    scheduleType = dsShift.Tables[0].Rows[0]["Scm_ScheduleType"].ToString();
                    timeIn = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString());
                    timeOut = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString());
                }

                string query = string.Format(@"SELECT Pmx_ParameterValue
                                        FROM T_ParameterMasterExt
                                        WHERE Pmx_ParameterId = 'OTFLB4'
                                        AND Pmx_Classification = '{0}'

                                        SELECT Pmt_NumericValue
                                        FROM T_ParameterMaster
                                        WHERE Pmt_ParameterId = 'OTFILEPAD'", scheduleType);

                dsParameter = dal.ExecuteDataSet(query);

                if (dsParameter.Tables[0].Rows.Count > 0 )
                {
                    paramValue = dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString();
                    if(dsParameter.Tables[1].Rows.Count > 0)
                    timeExt = Convert.ToInt32(dsParameter.Tables[1].Rows[0]["Pmt_NumericValue"]);

                    if (paramValue.ToUpper().Equals("STARTOFSHIFT"))
                    {
                        if(timeExt > 0)
                            timeCutOff = Convert.ToString(Convert.ToDecimal(timeIn.ToString().PadLeft(4, '0').Substring(0, 2)) + timeExt) + ":" + timeIn.ToString().PadLeft(4,'0').Substring(2, 2); 
                        else
                            timeCutOff = timeIn.ToString().Substring(0, 2) + ":" + timeIn.ToString().Substring(2, 2);
                    }
                    else if (paramValue.ToUpper().Equals("ENDOFSHIFT"))
                    {
                        if (timeIn < timeOut)
                        {
                            if (timeExt > 0)
                                timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + timeExt).PadLeft(2, '0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                            else
                                timeCutOff = timeOut.ToString().Substring(0, 2) + ":" + timeOut.ToString().Substring(2, 2);
                        }
                        else if (timeOut < timeIn)
                        {
                            if (timeExt > 0)
                                timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + 24 + timeExt).PadLeft(2,'0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                            else
                                timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + 24).PadLeft(2,'0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                        }
                    }
                    else
                    {
                        if (paramValue.Length == 4)
                        {
                            timeCutOff = dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString().Substring(0, 2) + ":" + dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString().Substring(2, 2);
                        }
                    }
                    if (!canFileOT)
                    {
                        if (timeCutOff != string.Empty)
                        {
                            DateTime dateTimeCutOff;
                            if (Convert.ToDouble(timeCutOff.Replace(":", "")) > 2359 && Convert.ToDouble(timeCutOff.Replace(":", "")) < 4759)
                            {
                                dateTimeCutOff = Convert.ToDateTime(string.Format("{0} {1}", dtpOTDate.Date.AddDays(1).ToString("MM/dd/yyyy"), Convert.ToString(Convert.ToDecimal(timeCutOff.Substring(0, 2)) - 24).PadLeft(2, '0') + ":" + timeCutOff.Substring(3, 2).PadLeft(2, '0')));
                                if (DateTime.Now > dateTimeCutOff)
                                {
                                    err += string.Format("Overtime Filing Cut-off: {0}", dateTimeCutOff.ToString("MM/dd/yyyy hh:mm tt"));
                                }
                            }
                            else if (Convert.ToDouble(timeCutOff.Replace(":", "")) <= 2359)
                            {
                                dateTimeCutOff = Convert.ToDateTime(string.Format("{0} {1}", dtpOTDate.Date.ToString("MM/dd/yyyy"), timeCutOff));

                                if (DateTime.Now > dateTimeCutOff)
                                {
                                    err += string.Format("Overtime Filing Cut-off: {0}", dateTimeCutOff.ToString("MM/dd/yyyy hh:mm tt"));
                                }
                            }
                        }
                    }
                }

            }
        }
        else
        {
            err += CommonMethods.GetErrorMessageForCutOff("OVERTIME");
        }
        return err;
    }

    //Perth Added 04/12/2013
    public DateTime GetDateValue(string strDatetime, string strTime, string strShiftStart)
    {
        DateTime dtStart;
        try
        {
            decimal dCurrentStart = Convert.ToDecimal(strTime.Replace(":", "").Trim().Substring(0, 2)) * 60
                                    + Convert.ToDecimal(strTime.Replace(":", "").Trim().Substring(2, 2));
            decimal dShiftStart = Convert.ToDecimal(strShiftStart.Replace(":", "").Trim().Substring(0, 2)) * 60
                                    + Convert.ToDecimal(strShiftStart.Replace(":", "").Trim().Substring(2, 2));
            if (dCurrentStart < dShiftStart)
            {
                dCurrentStart += 1440;
                string str1 = string.Format("{0:00.00}", dCurrentStart / 60).Substring(0, 2);
                string str2 = string.Format("{0:00}", dCurrentStart % 60).Substring(0, 2);
                strTime = string.Format("{0}:{1}", str1, str2);
            }
            int iDateAdd = 0;
            if (Convert.ToInt32(strTime.Trim().Substring(0, 2)) >= 24)
                iDateAdd = Convert.ToInt32(strTime.Trim().Substring(0, 2)) / 24;
            dtStart = Convert.ToDateTime(Convert.ToDateTime(strDatetime).AddDays(iDateAdd).ToString("MM/dd/yyyy")
                                            + string.Format(" {0:00}{1}"
                                                    , Convert.ToInt32(strTime.Trim().Substring(0, 2)) % 24
                                                    , strTime.Trim().Substring(2, 3)));
        }
        catch(Exception er)
        {
            throw er;
        }
        return dtStart;
    }

    public bool CheckIfOverlap(DateTime dtToCompareStart, DateTime dtToCompareEnd, DateTime dtTransactionStart, DateTime dtTransactionEnd)
    {
        bool ret = false;
        if (dtTransactionStart > dtTransactionEnd)
        {
            dtTransactionEnd = dtTransactionEnd.AddDays(1);
        }
        if ((dtToCompareStart >= dtTransactionStart && dtToCompareStart <= dtTransactionEnd)
            || (dtToCompareEnd >= dtTransactionStart && dtToCompareEnd <= dtTransactionEnd)
            || (dtToCompareStart <= dtTransactionStart && dtToCompareEnd >= dtTransactionEnd))
        {
            ret = true;
        }
        return ret;
    }

    //private void computeOTHours()
    //{
    //    decimal _start = 0;
    //    decimal _end = 0;

    //    string _startTime = txtOTStartTime.Text;
    //    string _endTime = txtOTEndTime.Text;
    //    _startTime = _startTime.Replace(":", "");
    //    _endTime = _endTime.Replace(":", "");


    //    _start = Convert.ToDecimal(_startTime.Substring(0, 2)) * 60 + Convert.ToDecimal(_startTime.Substring(2, 2));
    //    _end = Convert.ToDecimal(_endTime.Substring(0, 2)) * 60 + Convert.ToDecimal(_endTime.Substring(2, 2));

    //    decimal finalOTHours = 0;
    //    string shiftTimeIn = hfI1.Value;
    //    string shiftBreakStart = hfO1.Value;
    //    string shiftBreakEnd = hfI2.Value;
    //    string shiftTimeOut = hfO2.Value;
    //    string shiftPaidBreak = hfShiftPaid.Value;
    //    string shiftScheduleType = hfShiftType.Value;

    //    decimal intTimeIn = (int.Parse(shiftTimeIn.Substring(0, 2)) * 60) + int.Parse(shiftTimeIn.Substring(2, 2));
    //    decimal intBreakStart = (int.Parse(shiftBreakStart.Substring(0, 2)) * 60) + int.Parse(shiftBreakStart.Substring(2, 2));
    //    decimal intBreakEnd = (int.Parse(shiftBreakEnd.Substring(0, 2)) * 60) + int.Parse(shiftBreakEnd.Substring(2, 2));
    //    decimal intTimeOut = (int.Parse(shiftTimeOut.Substring(0, 2)) * 60) + int.Parse(shiftTimeOut.Substring(2, 2));
    //    decimal intPaidBreak = int.Parse(shiftPaidBreak);

    //    if (_start != 0 && _end != 0)
    //    {
    //        if (shiftScheduleType == "G")
    //        {
    //            if (_end < _start)
    //            {
    //                _end += 1440;
    //            }
    //            if (intBreakStart < intTimeIn)
    //            {
    //                intBreakStart += 1440;
    //            }
    //            if (intBreakEnd < intTimeIn)
    //            {
    //                intBreakEnd += 1440;
    //            }
    //            if (intTimeOut < intTimeIn)
    //            {
    //                intTimeOut += 1440;
    //            }
    //        }
    //        finalOTHours = _end - _start;
    //        if (_start <= intBreakStart && _end >= intBreakEnd)
    //        {
    //            finalOTHours = finalOTHours - (intBreakEnd - intBreakStart);
    //        }
    //        else if (_start <= intBreakStart && (_end > intTimeOut && _end <= intBreakEnd))
    //        {
    //            finalOTHours = finalOTHours - (_end - intBreakStart);
    //        }
    //        else if ((_start >= intBreakStart && _start < intBreakEnd) && _end >= intBreakEnd)
    //        {
    //            finalOTHours = finalOTHours - (intBreakEnd - _start);
    //        }

    //        if (_start >= intBreakStart && _start <= intBreakEnd)
    //        {
    //            intPaidBreak = intPaidBreak - (intBreakEnd - _start);
    //        }
    //        else if (_end >= intBreakStart && _end <= intBreakEnd)
    //        {
    //            intPaidBreak = intPaidBreak - (_end - intBreakStart);
    //        }
    //        else if (_start > intBreakEnd || _end <= intBreakStart)
    //        {
    //            intPaidBreak = 0;
    //        }

    //        if (intPaidBreak < 0)
    //        {
    //            intPaidBreak = 0;
    //        }
    //        if (intPaidBreak != 0 && Convert.ToDecimal(_startTime) == Convert.ToDecimal(shiftBreakEnd))
    //        {
    //            finalOTHours = (finalOTHours + intPaidBreak) / 60;
    //        }
    //        else if (intPaidBreak != 0 && Convert.ToDecimal(_end) == Convert.ToDecimal(shiftBreakStart))
    //        {
    //            finalOTHours = finalOTHours / 60;
    //        }
    //        else
    //        {
    //            finalOTHours = (finalOTHours + intPaidBreak) / 60;
    //        }
    //        if (_end == 0)
    //        {
    //            txtOTHours.Text = "";
    //        }
    //        else
    //        {
    //            if (finalOTHours != 0)
    //            {
    //                txtOTHours.Text = Math.Round(finalOTHours, 2).ToString();
    //            }
    //            else
    //            {
    //                txtOTHours.Text = "";
    //            }
    //        }
    //    }
    //    else
    //    {
    //        txtOTHours.Text = "";
    //    }

    //}
    // Randy added (31-JUL-2012): validation of the OT hoursy
    private void computeEndTime()
    {
        string _start = txtOTStartTime.Text;
        string _hours = txtOTHours.Text;
        _start = _start.Replace(":", "");

        string finalEndTimeHours = "X";
        string finalEndTimeMins = "X";

        string shiftTimeIn = hfI1.Value;
        string shiftBreakStart = hfO1.Value;
        string shiftBreakEnd = hfI2.Value;
        string shiftTimeOut = hfO2.Value;
        string shiftPaidBreak = hfShiftPaid.Value;
        string shiftScheduleType = hfShiftType.Value;

        int intTimeIn = (int.Parse(shiftTimeIn.Substring(0, 2)) * 60) + int.Parse(shiftTimeIn.Substring(2, 2));
        int intBreakStart = (int.Parse(shiftBreakStart.Substring(0, 2)) * 60) + int.Parse(shiftBreakStart.Substring(2, 2));
        int intBreakEnd = (int.Parse(shiftBreakEnd.Substring(0, 2)) * 60) + int.Parse(shiftBreakEnd.Substring(2, 2));
        int intTimeOut = (int.Parse(shiftTimeOut.Substring(0, 2)) * 60) + int.Parse(shiftTimeOut.Substring(2, 2));
        int intPaidBreak = int.Parse(shiftPaidBreak);


        if (_start != "" && _hours != "")
        {
            if (shiftScheduleType == "G")
            {
                if (intBreakStart < intTimeIn) 
                {
                    intBreakStart += 1440;
                }
                if (intBreakEnd < intTimeIn)
                {
                    intBreakEnd += 1440;
                }
                if (intTimeOut < intTimeIn)
                {
                    intTimeOut += 1440;
                }
            }

            // OT time in/out
            int actualTimeIn = (int.Parse(_start.Substring(0, 2)) * 60) + int.Parse(_start.Substring(2, 2));
            int actualTimeOut = actualTimeIn + (int)(float.Parse(_hours) * 60);
            int excessFromPaid = 0;
            int intFinalTime = actualTimeOut;

            if (actualTimeIn <= intBreakStart && actualTimeOut >= intBreakEnd)
            {
                intFinalTime = intFinalTime + (intBreakEnd - intBreakStart - intPaidBreak);
            }
            if (actualTimeOut > intBreakStart && actualTimeOut <= intBreakEnd)
            {
                excessFromPaid = (actualTimeOut - intBreakStart - intPaidBreak);
                if (excessFromPaid > 0)
                {
                    intFinalTime = intBreakEnd + excessFromPaid;
                }
            }

            //added for allowing OVERTIME for breaktime. Just comment if not used
            if (actualTimeIn >= intBreakStart && actualTimeIn <= intBreakEnd && actualTimeOut >= intBreakStart && actualTimeOut <= intBreakEnd && intPaidBreak == 0)
            {
                intFinalTime = intFinalTime - (intBreakEnd - intBreakStart);
            }
            //end allow overtimein break

            int round = intFinalTime / 1440;
            if (shiftScheduleType == "G" && intFinalTime > 1440 && (((int.Parse(shiftTimeOut.Substring(0, 2)) * 60) + int.Parse(shiftTimeOut.Substring(2, 2))) > 1440))
            {
                intFinalTime = (intFinalTime - 1440) + (round * 1440);
            }
            //Start format
            finalEndTimeHours = Math.Floor((decimal)intFinalTime / 60).ToString();
            finalEndTimeMins = (intFinalTime - (Math.Floor((decimal)intFinalTime / 60) * 60)).ToString();

            if (int.Parse(finalEndTimeHours) < 10)
            {
                finalEndTimeHours = String.Format("0{0}", finalEndTimeHours);
            }
            if (int.Parse(finalEndTimeMins) < 10)
            {
                finalEndTimeMins = String.Format("0{0}", finalEndTimeMins);
            }

            if (_hours == "" || float.Parse(_hours) == 0)
            {
                txtOTEndTime.Text = "";
            }
            else
            {
                if (finalEndTimeHours != "X" && finalEndTimeMins != "X")
                {
                    txtOTEndTime.Text = String.Format("{0}:{1}", finalEndTimeHours, finalEndTimeMins);
                }
                else
                {
                    txtOTEndTime.Text = "";
                }
            }
        }
        else
        {
            txtOTEndTime.Text = "";
        }
    }

    //Perth Added 10/2/2012
    private decimal GetActualOtHours()
    {
        decimal ret = 0;

        decimal dStart = Convert.ToDecimal(Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) / 60);
        decimal dEnd = Convert.ToDecimal(Convert.ToDecimal(this.txtOTEndTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTEndTime.Text.Substring(3, 2)) / 60);

        if (dEnd > 24)
            dEnd = 24;

        ret = dEnd - dStart;
    
        return ret;
    }

    private string getMINOTHR(string employeeId)//function not in use
    {
        string value = string.Empty;
		string sql = string.Empty;
		if(Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
             sql = @"declare @defaultValue as decimal(8,2)
                         SET @defaultValue = (SELECT Pmt_NumericValue
                                                FROM T_ParameterMaster
                                               WHERE Pmt_ParameterId = 'MINOTHR')
                            
                      SELECT ISNULL(Convert(varchar(10),Pmx_ParameterValue),@defaultValue)
                        FROM T_EmployeeMaster
                        LEFT JOIN T_ParameterMasterExt
                          ON Pmx_Classification = Emt_JobLevel
                         AND Pmx_ParameterId = 'MINOTHR'
                       WHERE Emt_EmployeeID = @EmployeeId";
    	}
		else
		{
			sql = @"SELECT Pmt_NumericValue
	                  FROM T_ParameterMaster
	                 WHERE Pmt_ParameterId = 'MINOTHR'";
		}
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", employeeId);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                value = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text, param));
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

        return value;

    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_EmployeeOvertime");
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        if (txtFiller1.Text != string.Empty)
        {
            DataSet dsFiller1 = getFillerCode(txtFiller1.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller1.Text + "'", "Eot_Filler01"));
            if (dsFiller1.Tables[0].Rows.Count > 0)
                filler1 = dsFiller1.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        if (txtFiller2.Text != string.Empty)
        {
            DataSet dsFiller2 = getFillerCode(txtFiller2.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller2.Text + "'", "Eot_Filler02"));
            if (dsFiller2.Tables[0].Rows.Count > 0)
                filler2 = dsFiller2.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        if (txtFiller3.Text != string.Empty)
        {
            DataSet dsFiller3 = getFillerCode(txtFiller3.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller3.Text + "'", "Eot_Filler03"));
            if (dsFiller3.Tables[0].Rows.Count > 0)
                filler3 = dsFiller3.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        //Andre: removed condition. ALWAYS retreive current. 20130702
        //bool isCutoff = methods.GetProcessControlFlag("OVERTIME", "CUT-OFF");
        //if (isCutoff)
        //{
        //    dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        //}
        //else
        //{
            dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        //}

        //Perth Added 06/28/2012 For CurrentPayPeriod Not Affected by Cut off, but by OT Date
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            dr["Eot_CurrentPayPeriod"] = methods.GetCurrentPayPeriodCPH(dtpOTDate.Date.ToString("MM/dd/yyyy"));

            if (dr["Eot_CurrentPayPeriod"].ToString().Trim() == string.Empty)
            {
                //Andre: removed condition. ALWAYS retreive current. 20130702
                //if (isCutoff)
                //{
                //    dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
                //}
                //else
                //{
                    dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
                //} 
            }
        }
        //End

        dr["Eot_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Eot_OvertimeDate"] = dtpOTDate.Date.ToString("MM/dd/yyyy");
        dr["Eot_Seqno"] = OTBL.GetOTSequence(dtpOTDate.Date.ToString("MM/dd/yyyy"), txtEmployeeId.Text.ToString());
        dr["Eot_OvertimeType"] = ddlOTType.SelectedValue.ToString().ToUpper();
        dr["Eot_StartTime"] = txtOTStartTime.Text.ToString().Replace(":", "");
        dr["Eot_EndTime"] = txtOTEndTime.Text.ToString().Replace(":", "");//this.getOTEndTime(txtOTStartTime.Text, txtOTHours.Text).ToUpper();
        dr["Eot_OvertimeHour"] = txtOTHours.Text.ToString().ToUpper();
        dr["Eot_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["Eot_JobCode"] = txtJobCode.Text.ToString().ToUpper();
        dr["Eot_ClientJobNo"] = txtClientJobName.Text.ToString().ToUpper();
        dr["Eot_CheckedBy"] = Session["userLogged"].ToString();
        dr["Eot_Checked2By"] = Session["userLogged"].ToString();
        dr["Eot_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Eot_Status"] = Status.ToUpper();
        dr["Eot_ControlNo"] = ControlNum; 
        dr["Eot_OvertimeFlag"] = OTBL.ComputeOTFlag(dtpOTDate.Date.ToString("MM/dd/yyyy")).ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Eot_Filler1"] = filler1.ToUpper();
        dr["Eot_Filler2"] = filler2.ToUpper();
        dr["Eot_Filler3"] = filler3.ToUpper();

        return dr;
    }

    private DataRow PopultateDRForRemarks(string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_TransactionRemarks");

        dr["Trm_ControlNo"] = ControlNum;
        dr["Trm_Remarks"] = txtRemarks.Text.ToString().ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        return dr;
    }

    private string getOTEndTime(string start, string hours)
    {
        DataSet dsEnd = new DataSet();
        string finalEndTimeHours = string.Empty;
        string finalEndTimeMins = string.Empty;
        string[] array = new string[2];
        array[0] = dtpOTDate.Date.ToString("MM/dd/yyyy");
        array[1] = txtEmployeeId.Text;
        dsEnd.Clear();

        #region Get TIMES
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            try
            {
                string sqlGetShift = @" if(select convert(char(10),Ppm_EndCycle,101) from dbo.T_PayPeriodMaster where Ppm_CycleIndicator='C')>'{0}'
                                         begin
                                         select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,Scm_PaidBreak,Scm_ScheduleType,
                                                    --Leave hours in 1st half  
                                                      convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                      ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                    --Leave hours in 2nd half
                                                      convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                         (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                           from T_ShiftCodeMaster where  Scm_ShiftCode=
                                             (select Ell_ShiftCode from T_EmployeeLogLedger  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'
                                              union
                                              select Ell_ShiftCode from T_EmployeeLogLedgerHist  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}')                                                  
                                         end
                                       else
                                         begin 
                                         select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,Scm_PaidBreak,Scm_ScheduleType,
                                                    --Leave hours in 1st half  
                                                      convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                      ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                    --Leave hours in 2nd half
                                                      convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                         (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                           from T_ShiftCodeMaster where  Scm_ShiftCode=(select emt_shiftcode from t_employeemaster where emt_employeeid='{1}')
                                         end";

                string sqlDayCode = @"   SELECT Ell_Daycode 
                                           FROM T_EmployeeLogledger
                                          WHERE Ell_EmployeeId  = '{1}'
                                            AND Ell_ProcessDate ='{0}'
                                          UNION
                                         SELECT Ell_Daycode 
                                           FROM T_EmployeeLogledgerHist
                                          WHERE Ell_EmployeeId  = '{1}'
                                            AND Ell_ProcessDate ='{0}'";

                dsEnd = dal.ExecuteDataSet(string.Format(sqlGetShift, array), CommandType.Text);
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
        #endregion

        //Start computation of endtime
        try
        {
            string shiftTimeIn = dsEnd.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString();
            string shiftBreakStart = dsEnd.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString();
            string shiftBreakEnd = dsEnd.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString();
            string shiftTimeOut = dsEnd.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString();
            string shiftPaidBreak = dsEnd.Tables[0].Rows[0]["Scm_PaidBreak"].ToString();
            string shiftSchedType = dsEnd.Tables[0].Rows[0]["Scm_ScheduleType"].ToString();

            int intTimeIn = (Convert.ToInt32(shiftTimeIn.Substring(0, 2)) * 60) + Convert.ToInt32(shiftTimeIn.Substring(2, 2));
            int intBreakStart = (Convert.ToInt32(shiftBreakStart.Substring(0, 2)) * 60) + Convert.ToInt32(shiftBreakStart.Substring(2, 2));
            int intBreakEnd = (Convert.ToInt32(shiftBreakEnd.Substring(0, 2)) * 60) + Convert.ToInt32(shiftBreakEnd.Substring(2, 2));
            int intTimeOut = (Convert.ToInt32(shiftTimeOut.Substring(0, 2)) * 60) + Convert.ToInt32(shiftTimeOut.Substring(2, 2));
            int intPaidBreak = Convert.ToInt32(shiftPaidBreak);


            if (shiftSchedType.Equals("G"))
            {
                #region Add 1440 minutes(24 Hours) for Graveyard shift
                if (intBreakStart < intTimeIn)
                {
                    intBreakStart += 1440;
                }
                if (intBreakEnd < intTimeIn)
                {
                    intBreakEnd += 1440;
                }
                if (intTimeOut < intTimeIn)
                {
                    intTimeOut += 1440;
                }
                #endregion
            }

            int actualTimeIn = (Convert.ToInt32(start.Replace(":", "").Substring(0, 2)) * 60) + Convert.ToInt32(start.Replace(":", "").Substring(2, 2));
            int actualTimeOut = actualTimeIn + Convert.ToInt32((Convert.ToDecimal(hours) * 60));
            int excessFromPaid = 0;
            int intFinalTime = actualTimeOut;

            if (actualTimeIn <= intBreakStart && actualTimeOut >= intBreakEnd)
            {
                intFinalTime = intFinalTime + (intBreakEnd - intBreakStart - intPaidBreak);
            }
            if (actualTimeOut > intBreakStart && actualTimeOut <= intBreakEnd)
            {

                excessFromPaid = (actualTimeOut - intBreakStart - intPaidBreak);
                if (excessFromPaid > 0)
                {
                    intFinalTime = intBreakEnd + excessFromPaid;
                }
            }
            if (shiftSchedType.Equals("G") && intFinalTime > 1440)
            {
                intFinalTime -= 1440;
            }

            //Start to format for saving. Format sample '0700' not '07:00'
            finalEndTimeHours = Convert.ToString(intFinalTime / 60);
            finalEndTimeMins = Convert.ToString(intFinalTime - ((intFinalTime / 60) * 60));

            if (finalEndTimeHours.Length < 2)
            {
                finalEndTimeHours = "0" + finalEndTimeHours;
            }
            if (finalEndTimeMins.Length < 2)
            {
                finalEndTimeMins = "0" + finalEndTimeMins;
            }
            //End formatting
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
        }
        return finalEndTimeHours + finalEndTimeMins;
    }

    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = dtpOTDate.Date.ToString()
                 + ddlOTType.SelectedValue
                 + txtOTStartTime.Text
                 + txtOTHours.Text
                 + txtOTEndTime.Text
                 + txtReason.Text
                 + txtFiller1.Text
                 + txtFiller2.Text
                 + txtFiller3.Text
                 + txtJobCode.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        clearValues();
        initializeEmployee();
        initializeControls();
    }

    private void clearValues()
    {
        txtControlNo.Text = string.Empty;
        txtOTStartTime.Text = string.Empty;
        txtOTHours.Text = string.Empty;
        txtOTEndTime.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtJobCode.Text = string.Empty;
        txtClientJobNo.Text = string.Empty;
        txtClientJobName.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevOTDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        ddlOTType.SelectedIndex = 0;
        hfSaved.Value = "0";
    }

    private void loadTransactionDetail()
    {
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        DataRow dr = OTBL.getOvertimeInfo(Request.QueryString["cn"].Trim());
        txtEmployeeId.Text = dr["ID No"].ToString();
        txtEmployeeName.Text = dr["Lastname"].ToString() + ", " + dr["Firstname"].ToString() + " " + (dr["Middlename"].ToString().Trim() == string.Empty ? "" : dr["Middlename"].ToString().Substring(0, 1) + ".");
        txtNickname.Text = dr[Resources.Resource._3RDINFO].ToString();
        //if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
        //{
        //    txtNickname.Text = dr["Nickname"].ToString();
        //}
        //else
        //{
        //    txtNickname.Text = string.Empty;
        //}
        txtControlNo.Text = dr["Control No"].ToString();
        dtpOTDate.Date = Convert.ToDateTime(dr["OT Date"].ToString());
        txtDayCode.Text = dr["Day Code"].ToString();
        txtOTShift.Text = "[" + dr["Shift Code"].ToString() + "] "
                        + dr["Shift Time In"].ToString().Insert(2, ":") + "-"
                        + dr["Break Start"].ToString().Insert(2, ":") + "  "
                        + dr["Break End"].ToString().Insert(2, ":") + "-"
                        + dr["Shift Time Out"].ToString().Insert(2, ":");
        //ddlOTType.Items.Add(new ListItem(dr["Type"].ToString()));
        for (int i = 0; i < ddlOTType.Items.Count; i++)
        {
            //if (ddlOTType.Items[i].Value.Length >= 2)
            //{
            if (i > 0 && (ddlOTType.Items[i].Value.Substring(0, 1).ToUpper().Equals(dr["Type"].ToString().Substring(0, 1).ToUpper())))
                {
                    ddlOTType.SelectedIndex = i;
                    break;

                }
            //}
        }
       
        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        //DASH Specific
        txtJobCode.Text = dr["Job Code"].ToString();
        txtClientJobNo.Text = dr["Client Job No"].ToString();
        txtClientJobName.Text = dr["Client Job Name"].ToString();

        if (dr[CommonMethods.getFillerName("Eot_Filler01")].ToString() != string.Empty)
        {
            DataSet dsFiller1 = getFillerCode(dr[CommonMethods.getFillerName("Eot_Filler01")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Eot_Filler01")].ToString() + "'", "Eot_Filler01"));
            if (dsFiller1.Tables[1].Rows.Count > 0)
                filler1 = dsFiller1.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller1.Visible = true;
        }
        if (dr[CommonMethods.getFillerName("Eot_Filler02")].ToString() != string.Empty)
        {
            DataSet dsFiller2 = getFillerCode(dr[CommonMethods.getFillerName("Eot_Filler02")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Eot_Filler02")].ToString() + "'", "Eot_Filler02"));
            if (dsFiller2.Tables[1].Rows.Count > 0)
                filler2 = dsFiller2.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller2.Visible = true;
        }
        if (dr[CommonMethods.getFillerName("Eot_Filler03")].ToString() != string.Empty)
        {
            DataSet dsFiller3 = getFillerCode(dr[CommonMethods.getFillerName("Eot_Filler03")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Eot_Filler03")].ToString() + "'", "Eot_Filler03"));
            if (dsFiller3.Tables[1].Rows.Count > 0)
                filler3= dsFiller3.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller3.Visible = true;
        } 
        txtFiller1.Text = filler1.ToUpper();
        txtFiller2.Text = dr[CommonMethods.getFillerName("Eot_Filler02")].ToString();
        txtFiller3.Text = dr[CommonMethods.getFillerName("Eot_Filler03")].ToString();

        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpOTDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        dtpOTDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
        initializeShift(true);
        enableControls();
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "OVERTIME", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeOvertimeIndividual.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        showOptionalFields();
        txtRemarks.Focus();
        txtOTStartTime.Text = dr["Start"].ToString();
        txtOTEndTime.Text = dr["End"].ToString();
        txtOTHours.Text = dr["Hours"].ToString();
        hfPrevEntry.Value = changeSnapShot();
    }
    private DataSet getFillerCode(string filler, string fillerType)
    {
        string query = string.Format(@"SELECT Adt_AccountCode
                            FROM T_AccountDetail
                            WHERE Adt_AccountDesc = '{0}'
                            AND Adt_Status = 'A'
                            AND Adt_AccountType = '{1}'

                            SELECT Adt_AccountDesc
                            FROM T_AccountDetail
                            WHERE Adt_AccountCode = '{0}'
                            AND Adt_AccountType = '{1}'
                            AND Adt_Status = 'A'
                            ", filler, fillerType);
        DataSet dsResult;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dsResult = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }
        return dsResult;
    }
    private string getColumnFillerLookUp(string colText, string filler)
    {
        DataSet ds = new DataSet();
        string result = string.Empty;
        string sql = string.Format(@"SELECT Cfl_Lookup
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
                          AND Cfl_Status = 'A'
                        and Cfl_Lookup = (select TOP 1 Adt_AccountType
												from T_AccountDetail
                                                inner join T_ColumnFiller
												on Cfl_Lookup = Adt_AccountType
												where {0}
                                                AND Adt_AccountType = (SELECT Cfl_Lookup
																				FROM T_ColumnFiller
																			WHERE Cfl_ColName = '{1}'
																				AND Cfl_Status = 'A')
                                                AND Adt_Status = 'A')", colText, filler);
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
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
        if (ds.Tables[0].Rows.Count > 0)
            result = ds.Tables[0].Rows[0]["Cfl_Lookup"].ToString();
        return result;
    }
    //private string checkRequiredFields()
    //{

    //    if (Convert.ToDecimal(hfOTFRACTION.Value) != 1)
    //    {
    //        if (txtOTHours.Text != string.Empty && txtOTEndTime.Text != string.Empty)
    //            computeEndTime();
    //    }
    //    else
    //    {
    //        if (txtOTHours.Text != string.Empty && txtOTEndTime.Text != string.Empty)
    //            computeOTHours();
    //    }
    //    string err = string.Empty;
    //    if (txtOTHours.Text == string.Empty)
    //    {
    //        err += "\nOvertime Hours is Empty.";
    //    }
    //    if (txtOTEndTime.Text == string.Empty)
    //    {
    //        err += "\nEnd Time is Empty.";
    //    }
    //    if(txtReason.Text==string.Empty)
    //    {
    //        err += "\nReason is Empty.";
    //    }
    //    if (txtOTStartTime.Text == string.Empty)
    //    {
    //        err += "\nStart Time is Empty.";
    //    }

    //    return err;
    //}
    #endregion
}
