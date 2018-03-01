/* Revision no. W2.1.00002 
 * 
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/27/2013
 *  Update Notes    :   
 *      -   Leave Combination Checking for same Paid or Unpaid Type
 */
/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/14/2013
 *  Update Notes    :   
 *      -   Updated Saving, Approval and Disapproval part to check if Leave Credits are deductable
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
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Created by: Andre Antonio Sungahid
/// Important notes:
///     Leave Dropdown Code/Value
///         Leave Code       2 char
///         With Category    1 char
///         With Credit      1 char
///         DayUnit         21 char including commas(,)
///                 Total   25 characters
///                 
/// IF CLINICSYSFLAG is True CLinic System Is Used
///     CLINICSYSCNCK = Connection String
///     CLINICSYSUPLDLOC = Uploads Certificates
/// </summary>
public partial class Transactions_Leave_pgeLeaveIndividual : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    LeaveBL LVBL = new LeaveBL();
    MenuGrant MGBL = new MenuGrant();

    protected void Page_Load(object sender, EventArgs e)
    {

        //bool trial =!CommonMethods.isAffectedByCutoff("LEAVE", "06/16/2014");
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLVEENTRY"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    LeaveBL.CorrectLeaveLedger();
                    initializeEmployee();
                    initializeControls();
                    initializeLeaveParameters();
                    
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
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
                if (!hfPrevLVDate.Value.Equals(dtpLVDate.Date.ToShortDateString()))
                {
                    dtpLVDate_Change(dtpLVDate, new EventArgs());
                    hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
                }
                if (hfFromNotice.Value.Equals("TRUE"))
                {
                    initializeLeaveParameters();
                    loadTransactionDetail(hfControlNo.Value);
                    ddlType_SelectedIndexChanged(null, null);
                    hfSaved.Value = "0";
                    hfFromNotice.Value = "FALSE";
                    MessageBox.Show("Transaction loaded from Leave Notice(s)");
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveIndividual_LoadComplete);
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
                Session["transaction"] = string.Empty;
            } 
        }
    }
    
    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Leave_pgeLeaveIndividual_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "leaveScripts";
        string jsurl = "_leave.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtControlNo.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpLVDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpLVDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");

        btnLeaveNotice.Attributes.Add("OnClick", "javascript:return lookupLVLeaveNoticeOnEntry();");
        txtLVStartTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVStartTime')");
        txtLVStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtLVEndTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVEndTime')");
        txtLVEndTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        ddlType.Attributes.Add("OnChange", "javascript:ddlType_ClientChanged();");
        rblDayUnit.Attributes.Add("OnClick", "javascript:rblDayUnit_ClientChanged();");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        clearValues();
        initializeShift();
        initializeDefaultValues();
        initializeLeaveParameters();
    }

    protected void dtpLVDate_Change(object sender, EventArgs e)
    {
        initializeShift();
        initializeLeaveParameters();
        this.txtLVEndTime.Text = string.Empty;
        this.txtLVStartTime.Text = string.Empty;
        this.rblDayUnit.SelectedIndex = -1;
    }

    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    { 
        //Special trapping to enable filler for sickeness. Control Lookup is dependent on T_CoulumnFillerValues.
        //This would just hide or unhide extra control for Filler 1
        #region Sql
        
        string sql = @"
                       SELECT Adt_AccountCode [Code]
                          , Adt_AccountDesc [Description]
                       FROM T_AccountDetail
                      WHERE Adt_AccountType = 'LVECATEGRY'
                        AND Adt_Status = 'A'
    ";
        #endregion

        tbrFiller1.Visible = false;
        if (this.ddlType.SelectedIndex != -1)
        {
            if (this.ddlType.SelectedValue != null
                && this.ddlType.SelectedValue.ToString().Trim() != string.Empty
                && this.ddlType.SelectedValue.ToString().Trim().Substring(2, 1) == "1")
            {
                #region Initialize Dropdown
                DataSet dsCategory = null;
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dsCategory = dal.ExecuteDataSet(sql, CommandType.Text);
                    }
                    catch
                    { }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                if (!CommonMethods.isEmpty(dsCategory))
                {
                    this.ddlCategory.Enabled = true;
                    ddlCategory.Items.Clear();
                    ddlCategory.Items.Add(new ListItem("", ""));
                    for (int i = 0; i < dsCategory.Tables[0].Rows.Count; i++)
                    {
                        ddlCategory.Items.Add(new ListItem(dsCategory.Tables[0].Rows[i]["Description"].ToString()
                                                      , dsCategory.Tables[0].Rows[i]["Code"].ToString()));
                    }
                }
                else
                {
                    this.ddlCategory.Enabled = false;
                    ddlCategory.Items.Clear();
                }
                #endregion
            }
            else
            {
                #region Clear Dropdown
                ddlCategory.Items.Clear(); 
                this.ddlCategory.Items.Add(new ListItem("- not applicable -", "- not applicable -"));
                this.ddlCategory.Enabled = false;
                this.ddlCategory.SelectedValue = "- not applicable -";
                #endregion
            }
        }

        illnessShow(ddlType);
        //hfIndexType.Value = ddlType.SelectedIndex.ToString();
        return;
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        illnessShow(ddlCategory);
    }
    public void illnessShow( DropDownList ddl)
    {
        if (!ddl.SelectedValue.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL)
                && !Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
            {
                if (ddl.SelectedValue.Substring(0, 2).Equals("SL"))
                {
                    //showOptionalFields();
                    tbrFiller1.Visible = true;
                }
                else
                {
                    tbrFiller1.Visible = false;
                }
            }
            else
            {
                tbrFiller1.Visible = false;
            }
        }
        else
        {
            tbrFiller1.Visible = false;
        }
    }
    protected void btnX_Click(object sender, EventArgs e)
    {
        bool flagSuccessful = false;
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                string isLeaveError = string.Empty;
                if (hfPrevEntry.Value.Equals(changeSnapShot()))
                {
                    if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                      || !CommonMethods.isAffectedByCutoff("LEAVE", dtpLVDate.Date.ToString("MM/dd/yyyy")))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "LEAVE", btnX.Text.Trim().ToUpper());
                            if (!status.Equals(string.Empty))
                            {
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
                                            strEndorseApproveQuery = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', 'LV', '{2}', '{3}', '{4}' "
                                                                                        , txtControlNo.Text
                                                                                        , Session["userLogged"].ToString()
                                                                                        , status
                                                                                        , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                                            strSuccessPrompt = "Successfully endorsed transaction.";
                                            break;
                                        case "APPROVE":
                                            strEndorseApproveQuery = string.Format("EXEC ApproveLeave '{0}', '{1}', '{2}' "
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
                                        SystemMenuLogBL.InsertEditLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "", false);
                                        MessageBox.Show(strSuccessPrompt);
                                        flagSuccessful = true;
                                    }
                                    else
                                    {
                                        drArrRows = dtResult.Select("Result <> 1");
                                        if (drArrRows.Length > 0)
                                            MessageBox.Show(drArrRows[0]["Message"].ToString());
                                    }

                                    dal.CommitTransactionSnapshot();
                                }
                                catch (Exception ex)
                                {
                                    isLeaveError = ex.Message;
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
                            else
                            {
                                MessageBox.Show("No route defined for user");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                    }
                }
                else
                {
                    MessageBox.Show("Some changes have been made. Save transaction to update.");
                }
                if (isLeaveError != string.Empty)
                {
                    MessageBox.Show(isLeaveError);
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
                string isLeaveError = string.Empty;
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("LEAVE", dtpLVDate.Date.ToString("MM/dd/yyyy")))
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
                                    dtResult = dal.ExecuteDataSet(string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', 'LV', '{3}' "
                                                                                            , txtControlNo.Text
                                                                                            , Session["userLogged"].ToString()
                                                                                            , ""
                                                                                            , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                    drArrRows = dtResult.Select("Result = 1");
                                    if (drArrRows.Length > 0)
                                    {
                                        //MenuLog
                                        SystemMenuLogBL.InsertDeleteLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                        flagSuccessful = true;
                                        MessageBox.Show("Transaction cancelled");
                                    }
                                    else
                                    {
                                        drArrRows = dtResult.Select("Result <> 1");
                                        if (drArrRows.Length > 0)
                                            MessageBox.Show(drArrRows[0]["Message"].ToString());
                                    }

                                    break;
                                case "DISAPPROVE":
                                    if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                    {
                                        dtResult = dal.ExecuteDataSet(string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', 'LV', '{3}' "
                                                                                                , txtControlNo.Text
                                                                                                , Session["userLogged"].ToString()
                                                                                                , txtRemarks.Text.Trim().ToUpper()
                                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                        drArrRows = dtResult.Select("Result = 1");
                                        if (drArrRows.Length > 0)
                                        {
                                            //MenuLog
                                            SystemMenuLogBL.InsertEditLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                            flagSuccessful = true;
                                            MessageBox.Show("Successfully disapproved transaction.");
                                        }
                                        else
                                        {
                                            drArrRows = dtResult.Select("Result <> 1");
                                            if (drArrRows.Length > 0)
                                                MessageBox.Show(drArrRows[0]["Message"].ToString());
                                        }
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
                            isLeaveError = ex.Message;   
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
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                }
                if (isLeaveError.Trim() != string.Empty)
                {
                    MessageBox.Show(isLeaveError);
                }
                refreshLeaveLedger();
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
                string isLeaveError = string.Empty;
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("LEAVE", dtpLVDate.Date.ToString("MM/dd/yyyy")))
                {
                    //string errMsg1 = checkEntry1();
                    //if (errMsg1.Equals(string.Empty))
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
                                            DataSet dsFiller1 = getFillerCode(txtFiller1.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller1.Text + "'", "Elt_Filler01"));
                                            if (dsFiller1.Tables[0].Rows.Count > 0)
                                                filler1 = dsFiller1.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                        }
                                        if (txtFiller2.Text != string.Empty)
                                        {
                                            DataSet dsFiller2 = getFillerCode(txtFiller2.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller2.Text + "'", "Elt_Filler02"));
                                            if (dsFiller2.Tables[0].Rows.Count > 0)
                                                filler2 = dsFiller2.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                        }
                                        if (txtFiller3.Text != string.Empty)
                                        {
                                            DataSet dsFiller3 = getFillerCode(txtFiller3.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller3.Text + "'", "Elt_Filler03"));
                                            if (dsFiller3.Tables[0].Rows.Count > 0)
                                                filler3 = dsFiller3.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                                        }

                                        dtResult = dal.ExecuteDataSet(string.Format("EXEC CreateLeaveRecord '{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}','{18}','{19}' "
                                                                                            , txtControlNo.Text
                                                                                            , txtEmployeeId.Text.ToString()
                                                                                            , dtpLVDate.Date.ToString("MM/dd/yyyy")
                                                                                            , ddlType.SelectedValue.ToUpper().Substring(0, 2)
                                                                                            , txtLVStartTime.Text.ToString().Replace(":", "")
                                                                                            , txtLVEndTime.Text.ToString().Replace(":", "")
                                                                                            , -1
                                                                                            , rblDayUnit.SelectedValue
                                                                                            , txtReason.Text.ToString().ToUpper()
                                                                                            , (ddlCategory.SelectedValue.ToUpper().IndexOf("NOT APP") != -1) ? "" : ddlCategory.SelectedValue
                                                                                            , txtDateFiled.Text
                                                                                            , "1" //Status=NEW
                                                                                            , Session["userLogged"].ToString()
                                                                                            , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                                            , "WFLVEENTRY"
                                                                                            , false //No Email Notification here
                                                                                            , Convert.ToBoolean(hfNoticeFlag.Value)
                                                                                            , ddlType.SelectedValue.Substring(0, 2).Equals("SL") || ddlCategory.SelectedValue.Equals("SL") ? filler1.ToUpper() : string.Empty
                                                                                            , filler2.ToUpper()
                                                                                            , filler3.ToUpper())).Tables[0];
                                        drArrRows = dtResult.Select("Result = 1");
                                        if (drArrRows.Length > 0)
                                        {
                                            SystemMenuLogBL.InsertAddLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
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
                                            #region To disable leave notice lookup after save of transaction
                                            //if (LVBL.hasLeaveNotice(txtEmployeeId.Text))//if other employees are allowed to endorse notice for other employees
                                            if (Session["userLogged"].ToString().Equals(txtEmployeeId.Text) && LVBL.hasLeaveNotice(Session["userLogged"].ToString()))
                                            {
                                                btnLeaveNotice.Visible = true;
                                            }
                                            else
                                            {
                                                btnLeaveNotice.Visible = false;
                                            }
                                            #endregion
                                            if (!hfControlNo.Value.Equals(string.Empty))
                                            {
                                                LVBL.UpdateLVNoticeRecordStatus(hfControlNo.Value, "9", Session["userLogged"].ToString(), dal);
                                                hfControlNo.Value = string.Empty;
                                            }
                                            capturePrevValues();
                                            //hfPrevEntry.Value = changeSnapShot();
                                        }
                                        else
                                        {
                                            SystemMenuLogBL.InsertAddLog("WFLVEENTRY", false, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                            drArrRows = dtResult.Select("Result <> 1");
                                            string strErrorMsg = "Please fix the ff. error(s):\n\n";
                                            for (int x = 0; x < drArrRows.Length; x++)
                                            {
                                                strErrorMsg += string.Format("{0}. {1}\n", x + 1, drArrRows[x]["Message"]);
                                            }
                                            MessageBox.Show(strErrorMsg);
                                        }
                                        break;
                                    case "RETURN TO EMPLOYEE":
                                        if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                        {
                                            dtResult = dal.ExecuteDataSet(string.Format("EXEC ReturnWFTransaction '{0}', '{1}', '{2}', 'LV', '{3}' "
                                                                                                , txtControlNo.Text
                                                                                                , Session["userLogged"].ToString()
                                                                                                , txtRemarks.Text.Trim().ToUpper()
                                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                            drArrRows = dtResult.Select("Result = 1");
                                            if (drArrRows.Length > 0)
                                            {
                                                SystemMenuLogBL.InsertEditLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                                restoreDefaultControls();
                                                MessageBox.Show("Successfully returned transaction.");
                                            }
                                            else
                                            {
                                                drArrRows = dtResult.Select("Result <> 1");
                                                if (drArrRows.Length > 0)
                                                    MessageBox.Show(drArrRows[0]["Message"].ToString());
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Enter remarks for action.");
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                //MenuLog
                                //SystemMenuLogBL.InsertAddLog("WFLVEENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
                                dal.CommitTransactionSnapshot();
                            }
                            catch (Exception ex)
                            {
                                isLeaveError = ex.Message;
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
                    //    MessageBox.Show(errMsg1);
                    //}
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                }
                if (isLeaveError.Trim() != string.Empty)
                {
                    MessageBox.Show(isLeaveError);
                }
                refreshLeaveLedger();
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
                              , isnull(('[' + Scm_ShiftCode + '] '
								+ LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2)
								+ '-' + LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2)
								+ ' ' + LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2)
								+ '-' + LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2)), '--NO DEFAULT SHIFT--') [Default Shift]
                           FROM T_EmployeeMaster
	                       LEFT JOIN T_DepartmentCodeMaster
	                         ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                          THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                          ELSE ''
							                          END
							LEFT JOIN T_ShiftCodeMaster
								ON Emt_Shiftcode = Scm_ShiftCode
                          WHERE Emt_EmployeeId = @EmployeeId

	                    SELECT 
		                    '[' + Ppm_PayPeriod + '] (' 
		                    + CONVERT(VARCHAR(20), Ppm_StartCycle, 101)
		                    + ' - ' + CONVERT(VARCHAR(20), Ppm_EndCycle, 101) + ')'
	                    FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'
                        ";
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
            this.txtDefaultShift.Text = ds.Tables[0].Rows[0]["Default Shift"].ToString().Trim();
            try
            {
                this.txtCurrentCycle.Text = ds.Tables[1].Rows[0][0].ToString().Trim();
            }
            catch
            {
                this.txtCurrentCycle.Text = "NO CURRENT CYCLE FOUND";
            }
        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
            this.txtDefaultShift.Text = string.Empty;
            this.txtCurrentCycle.Text = "NO CURRENT CYCLE FOUND";
        }
    }

    private void initializeDefaultValues()
    {
        DataSet ds = new DataSet();
        string sql = @"  SELECT Emt_EmployeeId [ID No]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname] 
	                          , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	                          , ISNULL(Dcm_Departmentdesc, '') [Department]
                              , isnull(('[' + Scm_ShiftCode + '] '
								+ LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2)
								+ '-' + LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2)
								+ ' ' + LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2)
								+ '-' + LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2)), '--NO DEFAULT SHIFT--') [Default Shift]
                           FROM T_EmployeeMaster
	                       LEFT JOIN T_DepartmentCodeMaster
	                         ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                          THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                          ELSE ''
							                          END
							LEFT JOIN T_ShiftCodeMaster
								ON Emt_Shiftcode = Scm_ShiftCode
                          WHERE Emt_EmployeeId = @EmployeeId

	                    SELECT 
		                    '[' + Ppm_PayPeriod + '] (' 
		                    + CONVERT(VARCHAR(20), Ppm_StartCycle, 101)
		                    + ' - ' + CONVERT(VARCHAR(20), Ppm_EndCycle, 101) + ')'
	                    FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'
                        ";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text.Trim());
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
            this.txtDefaultShift.Text = ds.Tables[0].Rows[0]["Default Shift"].ToString().Trim();
            try
            {
                this.txtCurrentCycle.Text = ds.Tables[1].Rows[0][0].ToString().Trim();
            }
            catch
            {
                this.txtCurrentCycle.Text = "NO CURRENT CYCLE FOUND";
            }
        }
        else
        {
            this.txtDefaultShift.Text = string.Empty;
            this.txtCurrentCycle.Text = "NO CURRENT CYCLE FOUND";
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "LEAVE", "WFLVEENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpLVDate.Date = DateTime.Now;
        dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        
        hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        hfLVHRENTRY.Value = (!methods.GetProcessControlFlag("LEAVE", "DAYSEL")).ToString();
        hfLHRSINDAY.Value = CommonMethods.getParamterValue("LHRSINDAY").ToString();
        lblUnit.Text = (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS)) ? "UNIT: IN HOURS" : "UNIT: IN DAYS";
        txtDateFiled.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm");
        initializeShift();
        enableControls();
        enableButtons();
        showOptionalFields();
        hfPrevEntry.Value = changeSnapShot();

        //if (LVBL.hasLeaveNotice(txtEmployeeId.Text))//if other employees are allowed to endorse notice for other employees
        if (Session["userLogged"].ToString().Equals(txtEmployeeId.Text) && LVBL.hasLeaveNotice(Session["userLogged"].ToString()))
        {
            btnLeaveNotice.Visible = true;
        }
        else
        {
            btnLeaveNotice.Visible = false;
        }

        hfCHIYODA.Value = Resources.Resource.CHIYODASPECIFIC.ToString().ToUpper();
        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
        {
            this.divCertificates.InnerHtml = "";
            this.tblClinicCertificates.Visible = true;
            this.hfClinicRec.Value = string.Empty;
        }
        else
        {
            this.divCertificates.InnerHtml = "";
            this.tblClinicCertificates.Visible = false;
        }
    }

    private void initializeShift()
    {
        DataSet ds = new DataSet();
	    if(Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
		{
			ds = CommonMethods.getDefaultShiftCHIYODA(txtEmployeeId.Text, dtpLVDate.Date.ToString("MM/dd/yyyy"));
		}
		else
		{
        	ds = CommonMethods.getEmployeeShift(txtEmployeeId.Text, dtpLVDate.Date.ToString("MM/dd/yyyy"));
		}
        if (!CommonMethods.isEmpty(ds))
        {
            txtLVShift.Text = "[" + ds.Tables[0].Rows[0]["Code"].ToString() + "] "
                            + ds.Tables[0].Rows[0]["TimeIn"].ToString().Insert(2, ":") + "-"
                            + ds.Tables[0].Rows[0]["BreakStart"].ToString().Insert(2, ":") + "  "
                            + ds.Tables[0].Rows[0]["BreakEnd"].ToString().Insert(2, ":") + "-"
                            + ds.Tables[0].Rows[0]["TimeOut"].ToString().Insert(2, ":");
            txtDayCode.Text = ds.Tables[0].Rows[0]["DayCode"].ToString();
            hfShiftCode.Value = ds.Tables[0].Rows[0]["Code"].ToString();
            hfShiftType.Value = ds.Tables[0].Rows[0]["Type"].ToString();
            hfShiftHours.Value = ds.Tables[0].Rows[0]["Hours"].ToString();
            hfShiftPaid.Value = ds.Tables[0].Rows[0]["PaidBreak"].ToString();
            hfI1.Value = ds.Tables[0].Rows[0]["TimeIn"].ToString();
            hfO1.Value = ds.Tables[0].Rows[0]["BreakStart"].ToString();
            hfI2.Value = ds.Tables[0].Rows[0]["BreakEnd"].ToString();
            hfO2.Value = ds.Tables[0].Rows[0]["TimeOut"].ToString();
        }
    }

    private void enableControls()
    {
        System.Web.UI.HtmlControls.HtmlImage cal = new System.Web.UI.HtmlControls.HtmlImage();
        cal = (System.Web.UI.HtmlControls.HtmlImage)dtpLVDate.Controls[2];

        switch (txtStatus.Text.ToUpper())
        {
            case "":
                if (!hfNoticeFlag.Value.Equals("TRUE"))
                {
                    //dtpLVDate.Enabled = true;
                    cal.Disabled = false;

                    ddlType.Enabled = true;
                    ddlCategory.Enabled = true;
                    txtLVStartTime.ReadOnly = false;
                    txtLVEndTime.ReadOnly = false;
                    rblDayUnit.Enabled = true;
                    txtReason.ReadOnly = false;
                    txtRemarks.ReadOnly = true;
                    btnFiller1.Enabled = true;
                    btnFiller2.Enabled = true;
                    btnFiller3.Enabled = true;

                    txtLVStartTime.BackColor = System.Drawing.Color.White;
                    txtLVEndTime.BackColor = System.Drawing.Color.White;
                    txtReason.BackColor = System.Drawing.Color.White;
                    txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                }
                else
                {
                    //dtpLVDate.Enabled = false;
                    cal.Disabled = true;

                    ddlType.Enabled = false;
                    ddlCategory.Enabled = false;
                    txtLVStartTime.ReadOnly = true;
                    txtLVEndTime.ReadOnly = true;
                    rblDayUnit.Enabled = false;
                    txtReason.ReadOnly = false;
                    txtRemarks.ReadOnly = true;
                    btnFiller1.Enabled = false;
                    btnFiller2.Enabled = false;
                    btnFiller3.Enabled = false;

                    txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                    txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                    txtReason.BackColor = System.Drawing.Color.White;
                    txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                }
                break;
            case "NEW":
                if (!hfNoticeFlag.Value.Equals("TRUE"))
                {
                    //dtpLVDate.Enabled = true;
                    cal.Disabled = false;

                    ddlType.Enabled = true;
                    ddlCategory.Enabled = true;
                    txtLVStartTime.ReadOnly = false;
                    txtLVEndTime.ReadOnly = false;
                    rblDayUnit.Enabled = true;
                    txtReason.ReadOnly = false;
                    txtRemarks.ReadOnly = true;
                    btnFiller1.Enabled = true;
                    btnFiller2.Enabled = true;
                    btnFiller3.Enabled = true;

                    txtLVStartTime.BackColor = System.Drawing.Color.White;
                    txtLVEndTime.BackColor = System.Drawing.Color.White;
                    txtReason.BackColor = System.Drawing.Color.White;
                    txtRemarks.BackColor = System.Drawing.Color.Gainsboro;

                    //For Clinic
                    EnableControls(false);
                }
                else
                {
                    //dtpLVDate.Enabled = false;
                    cal.Disabled = true;

                    ddlType.Enabled = false;
                    ddlCategory.Enabled = false;
                    txtLVStartTime.ReadOnly = true;
                    txtLVEndTime.ReadOnly = true;
                    rblDayUnit.Enabled = false;
                    txtReason.ReadOnly = false;
                    txtRemarks.ReadOnly = true;
                    btnFiller1.Enabled = false;
                    btnFiller2.Enabled = false;
                    btnFiller3.Enabled = false;

                    txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                    txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                    txtReason.BackColor = System.Drawing.Color.White;
                    txtRemarks.BackColor = System.Drawing.Color.Gainsboro;

                    //For Clinic
                    EnableControls(false);
                }
                break;
            case "ENDORSED TO CHECKER 1":
                //dtpLVDate.Enabled = false;
                cal.Disabled = true;

                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                //dtpLVDate.Enabled = false;
                cal.Disabled = true;

                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                //dtpLVDate.Enabled = false;
                cal.Disabled = true;

                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            default:
                break;
        }
    }

    private void enableButtonsCheckApprover()
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
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Elt_Filler%'
                          AND Cfl_Status = 'A'";
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
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                try
                {
                    switch (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper())
                    {
                        case "ELT_FILLER01":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A")
                              )
                            {
                                if( ddlType.SelectedValue.Equals("SL"))
                                    tbrFiller1.Visible = true;
                                else
                                    tbrFiller1.Visible = false;
                                lblFiller1.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller1.Attributes.Add("OnClick", "javascript:return lookupGenericFiller(" + "'txtFiller1','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller1.Visible = false;
                            }
                            break;
                        case "ELT_FILLER02":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString(                                                                                                                                                  ).Equals(string.Empty)
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
                        case "ELT_FILLER03":
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
            //Added additional code for viewing only on SL.
            //ddlType_SelectedIndexChanged(null, null);
            //..End..End..Added additional code for viewing only on SL.
        }
    }

    private void initializeLeaveParameters()
    {
        DateTime LVDate;
        if(hfControlNo.Value!= string.Empty)
        {
        DataRow dr = LVBL.getLeaveInfoFromNotice(hfControlNo.Value);
            LVDate = Convert.ToDateTime(dr["Leave Date"].ToString());
        }
        else
        {
            LVDate = dtpLVDate.Date;
        }
        DataSet ds = new DataSet();
        #region SQL
        string sql = @"
            --Leave ledger query
                      declare @LHRSINDAY as decimal(6,3)
                      declare @LVHRENTRY as char(1)
                          SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                              FROM T_ParameterMaster 
                                             WHERE Pmt_ParameterId = 'LHRSINDAY')
                          SET @LVHRENTRY = '{0}'
                    
                       SELECT Ltm_LeaveDesc [Description]
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
                            , Elm_LeaveYear [Year]
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year

            --Leave type query
                       SELECT Elm_LeaveType [Type]
                            , Ltm_LeaveDesc [Description]
                            , Ltm_WithCategory [Category]
                            , REPLICATE(' ', 2 - LEN(Ltm_LeaveType)) + RTRIM(Ltm_LeaveType)
                            + Convert(varchar(1),Ltm_WithCategory)
                            + Convert(varchar(1),Ltm_WithCredit)
                            + REPLICATE(' ', 21- LEN(ISNULL(Ltm_DayUnit,''))) + RTRIM(ISNULL(Ltm_DayUnit,'')) [Code]
                            , Ltm_PaidLeave
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                            AND Ltm_CombinedLeave = 0
                            AND Ltm_Status = 'A'
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year

                        UNION

                       SELECT Ltm_LeaveType [Type]
                            , Ltm_LeaveDesc [Description]
                            , Ltm_WithCategory [Category]
                            , REPLICATE(' ', 2 - LEN(Ltm_LeaveType)) + RTRIM(Ltm_LeaveType)
                            + Convert(varchar(1),Ltm_WithCategory)
                            + Convert(varchar(1),Ltm_WithCredit)
                            + REPLICATE(' ', 21- LEN(ISNULL(Ltm_DayUnit,''))) + RTRIM(ISNULL(Ltm_DayUnit,'')) [Code]
                            , Ltm_PaidLeave
                         FROM T_LeaveTypeMaster
                         INNER JOIN T_Employeemaster
                           ON Emt_EmployeeId = @EmployeeId
                        LEFT JOIN T_ParameterMasterExt ON Pmx_Classification = Ltm_LeaveType
	                        AND Pmx_Status = 'A' 
	                        AND Pmx_ParameterID = 'LVFEMALE'
                        WHERE Ltm_Status = 'A'
                          AND Ltm_CombinedLeave = 0
	                      AND Ltm_WithCredit = 0
                          AND Ltm_Leavetype = Case WHEN Pmx_Classification IS NULL THEN Ltm_Leavetype
		                    ELSE CASE WHEN Emt_Gender = 'F' THEN  Pmx_Classification ELSE '' END END
                        ORDER BY Ltm_PaidLeave DESC, Ltm_WithCategory ASC

            --Leave category query
                        SELECT '- not applicable -' [Code]
                                , '- not applicable -' [Description]
                        UNION
                       SELECT Adt_AccountCode [Code]
                          , Adt_AccountDesc [Description]
                       FROM T_AccountDetail
                      WHERE Adt_AccountType = 'LVECATEGRY'
                        AND Adt_Status = 'A'";

        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            sql = @"
                      --Leave ledger query
                      declare @LHRSINDAY as decimal(6,3)
                      declare @LVHRENTRY as char(1)
                          SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                              FROM T_ParameterMaster 
                                             WHERE Pmt_ParameterId = 'LHRSINDAY')
                          SET @LVHRENTRY = '{0}'
                    
                       SELECT Ltm_LeaveDesc [Description]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Entitled
                                                         ELSE Elm_Entitled / @LHRSINDAY 
                                                     END )[Entitled]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN (Elm_Used + Elm_Reimbursed)
                                                         ELSE (Elm_Used + Elm_Reimbursed) / @LHRSINDAY 
                                                     END )[Used + Reimbursed]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Reserved
                                                         ELSE Elm_Reserved / @LHRSINDAY 
                                                     END )[Pending]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Entitled - (Elm_Used + Elm_Reimbursed) - Elm_Reserved 
                                                         ELSE (Elm_Entitled / @LHRSINDAY ) - ((Elm_Used + Elm_Reimbursed) / @LHRSINDAY) - (Elm_Reserved / @LHRSINDAY)
                                                     END )[Balance]
                             , Elm_LeaveYear [Year]
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year

            --Leave type query
                       SELECT Elm_LeaveType [Type]
                            , Ltm_LeaveDesc [Description]
                            , Ltm_WithCategory [Category]
                            , REPLICATE(' ', 2 - LEN(Ltm_LeaveType)) + RTRIM(Ltm_LeaveType)
                            + Convert(varchar(1),Ltm_WithCategory)
                            + Convert(varchar(1),Ltm_WithCredit)
                            + REPLICATE(' ', 21- LEN(ISNULL(Ltm_DayUnit,''))) + RTRIM(ISNULL(Ltm_DayUnit,'')) [Code]
                            , Ltm_PaidLeave
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year
                          AND Ltm_CombinedLeave = 0

                        UNION

                       SELECT Ltm_LeaveType [Type]
                            , Ltm_LeaveDesc [Description]
                            , Ltm_WithCategory [Category]
                            , REPLICATE(' ', 2 - LEN(Ltm_LeaveType)) + RTRIM(Ltm_LeaveType)
                            + Convert(varchar(1),Ltm_WithCategory)
                            + Convert(varchar(1),Ltm_WithCredit)
                            + REPLICATE(' ', 21- LEN(ISNULL(Ltm_DayUnit,''))) + RTRIM(ISNULL(Ltm_DayUnit,'')) [Code]
                            , Ltm_PaidLeave
                         FROM T_LeaveTypeMaster
                         LEFT JOIN T_Employeemaster
                           ON Emt_EmployeeId = @EmployeeId
                        WHERE Ltm_Status = 'A'
                          AND Ltm_CombinedLeave = 0
                          AND   ( Ltm_PaidLeave = 1
                              AND Ltm_WithCredit = 0)
                           OR   ( Ltm_PaidLeave = 0
                              AND ( Ltm_LeaveType <> CASE WHEN (Emt_Gender = 'F')
                                                          THEN ''
                                                          ELSE 'ML'
                                                      END
                                AND Ltm_WithCredit <> 1)
                              AND Ltm_Status = 'A')
                        ORDER BY Ltm_PaidLeave DESC, Ltm_WithCategory ASC

            --Leave category query
                        SELECT '- not applicable -' [Code]
                                , '- not applicable -' [Description]
                        UNION
                       SELECT Adt_AccountCode [Code]
                          , Adt_AccountDesc [Description]
                       FROM T_AccountDetail
                      WHERE Adt_AccountType = 'LVECATEGRY'
                        AND Adt_Status = 'A'";
        }
        #endregion
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text, SqlDbType.VarChar, 15);
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(LVDate,ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty), SqlDbType.VarChar, 4);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS) ? '1' :'0')), CommandType.Text, param);
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

        if (ds.Tables.Count > 0)
        {
            //Initialize employee leave ledger
            if (ds.Tables[0].Rows.Count > 0)
            {
                dgvLedger.DataSource = ds.Tables[0];
                dgvLedger.DataBind();
                lblCheckLedger.Text = string.Empty;
            }
            else
            {
                dgvLedger.DataSource = new DataTable("Dummy");
                dgvLedger.DataBind();
                lblCheckLedger.Text = " - No PAID Leave credits";
            }
            //Initialize leave type dropdown
            ddlType.Items.Clear();
            ddlType.Items.Add(new ListItem("Select a type", ""));
            if (ds.Tables[1].Rows.Count > 0)
            { 
                for(int i = 0 ;i < ds.Tables[1].Rows.Count; i++)
                {
                    ddlType.Items.Add(new ListItem( ds.Tables[1].Rows[i]["Description"].ToString()
                                                  , ds.Tables[1].Rows[i]["Code"].ToString()));
                }
            }
            //Initialize category dropdown
            if (false)
            {
                ddlCategory.Items.Clear();
                if (ds.Tables[2].Rows.Count > 0)
                {
                    ddlCategory.Items.Add(new ListItem("", ""));
                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                    {
                        ddlCategory.Items.Add(new ListItem(ds.Tables[2].Rows[i]["Description"].ToString()
                                                      , ds.Tables[2].Rows[i]["Code"].ToString()));
                    }
                }
            }
            //Initialize Radio Button Day Unit
            rblDayUnit.SelectedIndex = -1;

        }
        else
        {
            MessageBox.Show("Error in fetching leave data.");
        }
    }

    private void refreshLeaveLedger()
    {
        DataSet ds = new DataSet();
        string sql = @"--Leave ledger query
                      declare @LHRSINDAY as decimal(8,2)
                      declare @LVHRENTRY as char(1)
                          SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                              FROM T_ParameterMaster 
                                             WHERE Pmt_ParameterId = 'LHRSINDAY')
                          --SET @LVHRENTRY = (SELECT Convert(char(1),Pcm_ProcessFlag)
                          --                    FROM T_ProcessControlMaster 
                          --                   WHERE Pcm_SystemID = 'LEAVE'
                          --                     AND Pcm_ProcessID = 'LVHRENTRY')
                          SET @LVHRENTRY = '{0}'
                    
                       SELECT Ltm_LeaveDesc [Description]
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
                             , Elm_LeaveYear [Year]
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year";

        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            sql = @"
                      declare @LHRSINDAY as decimal(8,2)
                      declare @LVHRENTRY as char(1)
                          SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                              FROM T_ParameterMaster 
                                             WHERE Pmt_ParameterId = 'LHRSINDAY')
                          --SET @LVHRENTRY = (SELECT Convert(char(1),Pcm_ProcessFlag)
                          --                    FROM T_ProcessControlMaster 
                          --                   WHERE Pcm_SystemID = 'LEAVE'
                          --                     AND Pcm_ProcessID = 'LVHRENTRY')
                          SET @LVHRENTRY = '{0}'
                    
                       SELECT Ltm_LeaveDesc [Description]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Entitled
                                                         ELSE Elm_Entitled / @LHRSINDAY 
                                                     END )[Entitled]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN (Elm_Used + Elm_Reimbursed)
                                                         ELSE (Elm_Used + Elm_Reimbursed) / @LHRSINDAY 
                                                     END )[Used + Reimbursed]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Reserved
                                                         ELSE Elm_Reserved / @LHRSINDAY 
                                                     END )[Pending]
                            , CONVERT(decimal(6,3), CASE WHEN (@LVHRENTRY <> '0')
                                                         THEN Elm_Entitled - Elm_Used - Elm_Reserved 
                                                         ELSE (Elm_Entitled / @LHRSINDAY ) - ((Elm_Used + Elm_Reimbursed) / @LHRSINDAY) - (Elm_Reserved / @LHRSINDAY)
                                                     END )[Balance]
                             , Elm_LeaveYear [Year]
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          --AND Elm_LeaveYear = @Year ";
        }

        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDate.Date,ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS) ? '1' : '0')), CommandType.Text, param);
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
            dgvLedger.DataSource = ds.Tables[0];
            dgvLedger.DataBind();
            lblCheckLedger.Text = "";
            LeaveBL.CorrectLeaveLedger();
        }
        else
        {
            dgvLedger.DataSource = new DataTable("Dummy");
            dgvLedger.DataBind();
            lblCheckLedger.Text = " - No PAID Leave credits";
        }

    }
 
    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        #region Start Time
        if (txtLVStartTime.Text.Length < 5)
        {
            err += "\nStart Time invalid format.(hh:mm)";
        }
        else
        {
            try
            {
                int x = Convert.ToInt32(txtLVStartTime.Text.Substring(0, 2));
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtLVStartTime.Text.Substring(3, 2));
                decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtLVStartTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtLVStartTime.Text.Substring(3, 2)) / 60);
                temp = Math.Round(temp, 2);

                if (Convert.ToInt32(txtLVStartTime.Text.Substring(3, 2)) > 59)
                {
                    err += "\nStart Time minutes is invalid";
                }
                else if (temp > Convert.ToDecimal(71.98))
                {
                    err += "\nStart Time maximum time exceeded.(up to 71:59)";
                }
                else if (!txtLVStartTime.Text.Contains(":"))
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
        #region End Time
        if (txtLVEndTime.Text.Length < 5)
        {
            err += "\nEnd Time invalid format.(hh:mm)";
        }
        else
        {
            try
            {
                int x = Convert.ToInt32(txtLVEndTime.Text.Substring(0, 2));
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtLVEndTime.Text.Substring(3, 2));
                decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtLVEndTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtLVEndTime.Text.Substring(3, 2)) / 60);
                temp = Math.Round(temp, 2);

                if (Convert.ToInt32(txtLVEndTime.Text.Substring(3, 2)) > 59)
                {
                    err += "\nEnd Time minutes is invalid";
                }
                else if (temp > Convert.ToDecimal(72.00))
                {
                    err += "\nEnd Time maximum time exceeded.(up to 72:00)";
                }
                else if (!txtLVEndTime.Text.Contains(":"))
                {
                    err += "\nEnd Time invalid format.(hh:mm)";
                }
            }
            catch
            {
                err += "\nEnd Time invalid format.(hh:mm)";
            }
        }
        #endregion
        #region Leave Type
        if (ddlType.SelectedValue.Equals(string.Empty))
        {
            err += "\nSelect a leave type.";
        }
        #endregion
        #region Check day code of filing
        if (err.Equals(string.Empty))
        {
            if (!(txtDayCode.Text.Contains("REG") || txtDayCode.Text.Contains("SPL") || txtDayCode.Text.Contains("CMPY") || txtDayCode.Text.Contains("PSD")))//For SPL to be followed up on checkEntry2()
            {
                if (!ddlType.SelectedValue.ToString().Substring(0, 2).Equals("OB"))
                {
                    err += "Could not file leave for this day: " + txtDayCode.Text;
                }
            }
        }
        #endregion
        #region Start and End time validation after checking all fomat is correct.
        if (err.Equals(string.Empty))
        {
            int I1 = (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(2, 2));
            int O1 = (Convert.ToInt32(hfO1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO1.Value.Substring(2, 2));
            int I2 = (Convert.ToInt32(hfI2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI2.Value.Substring(2, 2));
            int O2 = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
            int PaidBreak = Convert.ToInt32(hfShiftPaid.Value);//Already in minutes format
            int LVStart = (Convert.ToInt32(txtLVStartTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtLVStartTime.Text.Substring(3, 2));
            int LVEnd = (Convert.ToInt32(txtLVEndTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtLVEndTime.Text.Substring(3, 2));
            if (hfShiftType.Value.Equals("G"))
            {
                if (O1 < I1)
                    O1 += 1440;
                if (I2 < I1)
                    I2 += 1440;
                if (O2 < I1)
                    O2 += 1440;
                if (LVStart < I1)
                    LVStart += 1440;
            }
            if (LVEnd < LVStart)
                LVEnd += 1440;

            if (getLeaveTypePaidLeave(ddlType.SelectedValue.Substring(0, 2)).Rows.Count==0)
            {
                    //if (!ddlType.SelectedValue.Substring(0, 2).Equals("OB"))//for THI OB can be outside of shift...
                    //{
                        if (LVStart < I1)
                        {
                            err += "\nStart time is less than shift time in.";
                        }
                        else if (LVStart > O2)
                        {
                            err += "\nStart time is greater than shift time out.";
                        }
                        else if (LVEnd > O2)
                        {
                            err += "\nEnd time is greater than shift time out.";
                        }
                        else if (LVEnd < I1)
                        {
                            err += "\nEnd time is less than shift time in.";
                        }
                    //}
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

    private string checkEntry2(DALHelper dal)
    {
        string err = string.Empty;
        DataSet ds = new DataSet();
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "LEAVE", "WFLVEENTRY");
        bool canFileLV = userGrant.canGenerate();
        #region SQL
        #region Leave Confilct
        string sqlConflicts = @"SELECT Elt_ControlNo [Control No]
	                                 , Convert(varchar(10),Elt_LeaveDate,101) [Leave Date]
	                                 , Elt_StartTime [Start]
	                                 , Elt_EndTime [End]
	                                 , Adt_AccountDesc [Status]
                                  FROM T_EmployeeLeaveAvailment
                                  LEFT JOIN T_AccountDetail
                                    ON Adt_AccountType = 'WFSTATUS'
                                   AND Adt_AccountCode = Elt_Status
                                 WHERE Elt_EmployeeId = @EmployeeId
                                   AND Elt_Status IN ('1','3','5','7','9','A')
                                   AND Elt_ControlNo <> @ControlNo
                                   AND Elt_ControlNo NOT IN ( SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailment
							                                   WHERE Elt_EmployeeId = @EmployeeId
                                                                 AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                 AND Elt_Status = '0'
							                                   UNION 
							                                  SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailmentHist
							                                   WHERE Elt_EmployeeId = @EmployeeId
                                                                 AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                 AND Elt_Status = '0')
                                   AND (   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') >= dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') < dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) > dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) <= dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') <= dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) >= dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) ) )
                                    
                                UNION 

                                SELECT Elt_ControlNo [Control No]
	                                 , Convert(varchar(10),Elt_LeaveDate,101) [Leave Date]
	                                 , Elt_StartTime [Start]
	                                 , Elt_EndTime [End]
	                                 , Adt_AccountDesc [Status]
                                  FROM T_EmployeeLeaveAvailmentHist
                                  LEFT JOIN T_AccountDetail
                                    ON Adt_AccountType = 'WFSTATUS'
                                   AND Adt_AccountCode = Elt_Status
                                 WHERE Elt_EmployeeId = @EmployeeId
                                   AND Elt_Status IN ('1','3','5','7','9','A')
                                   AND Elt_ControlNo <> @ControlNo
                                   AND Elt_ControlNo NOT IN (   SELECT Elt_RefControlNo
								                                  FROM T_EmployeeLeaveAvailment
							                                     WHERE Elt_EmployeeId = @EmployeeId
                                                                   AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                   AND Elt_Status = '0'
							                                     UNION 
							                                    SELECT Elt_RefControlNo
								                                  FROM T_EmployeeLeaveAvailmentHist
							                                     WHERE Elt_EmployeeId = @EmployeeId
                                                                   AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                   AND Elt_Status = '0')
                                   AND (   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') >= dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') < dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) > dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) <= dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') <= dbo.getDatetimeFormatV3(Elt_StartTime, Convert(varchar(10),Elt_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) >= dbo.getDatetimeFormatV3(Elt_EndTime, Convert(varchar(10),Elt_LeaveDate,101), 1, Elt_StartTime) ) ) 

                                 UNION 

                                SELECT Eln_ControlNo [Control No]
                                     , Convert(varchar(10),Eln_LeaveDate,101) [Leave Date]
                                     , Eln_StartTime [Start]
                                     , Eln_EndTime [End]
                                     , Adt_AccountDesc + '-FROM NOTICE' [Status]
                                  FROM T_EmployeeLeaveNotice
                                  LEFT JOIN T_AccountDetail
                                    ON Adt_AccountType = 'WFSTATUS'
                                   AND Adt_AccountCode = Eln_Status
                                 WHERE Eln_EmployeeId = @EmployeeId
                                   AND Eln_Status IN ('1')
                                   AND Eln_ControlNo <> @NoticeControlNo
                                   AND Eln_ControlNo NOT IN (   SELECT Elt_RefControlNo
								                                  FROM T_EmployeeLeaveAvailment
							                                     WHERE Elt_EmployeeId = @EmployeeId
                                                                   AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                   AND Elt_Status = '0'
							                                     UNION 
							                                    SELECT Elt_RefControlNo
								                                  FROM T_EmployeeLeaveAvailmentHist
							                                     WHERE Elt_EmployeeId = @EmployeeId
                                                                   AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@leaveDate)) AND dateadd(day, 1, Convert(datetime,@leaveDate))
								                                   AND Elt_Status = '0')
                                   AND (   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') >= dbo.getDatetimeFormatV3(Eln_StartTime, Convert(varchar(10),Eln_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') < dbo.getDatetimeFormatV3(Eln_EndTime, Convert(varchar(10),Eln_LeaveDate,101), 1, Eln_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) > dbo.getDatetimeFormatV3(Eln_StartTime, Convert(varchar(10),Eln_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) <= dbo.getDatetimeFormatV3(Eln_EndTime, Convert(varchar(10),Eln_LeaveDate,101), 1, Eln_StartTime) )
                                      OR   ( dbo.getDatetimeFormatV3(@startTime,@leaveDate,0,'0000') <= dbo.getDatetimeFormatV3(Eln_StartTime, Convert(varchar(10),Eln_LeaveDate,101), 0, '0000')
                                         AND dbo.getDatetimeFormatV3(@endTime,@leaveDate,1,@startTime) >= dbo.getDatetimeFormatV3(Eln_EndTime, Convert(varchar(10),Eln_LeaveDate,101), 1, Eln_StartTime) ) ) 
";
        #endregion
        #region Leave Parameters
        string sqlParameters = @"   SELECT Pmt_ParameterId
                                         , Pmt_NumericValue
                                      FROM T_ParameterMaster
                                     WHERE Pmt_ParameterId IN ('LVFRACTION','MINLVHR')";
        #endregion
        #region Duplicate Entry
        string sqlDulicate = @" SELECT Elt_ControlNo [Control No]
	                                 , Convert(varchar(10),Elt_LeaveDate,101) [Leave Date]
                                     , Elt_LeaveType [Type]
	                                 , Elt_StartTime [Start]
	                                 , Elt_EndTime [End]
                                  FROM T_EmployeeLeaveAvailment
                                 WHERE Elt_EmployeeId = @EmployeeId
                                   AND Elt_Status IN ('1','3','5','7','9','A')
                                   AND Elt_ControlNo <> @ControlNo
                                   AND Elt_LeaveType = @LeaveType
                                   AND Elt_LeaveDate = @leaveDate
                                   AND Elt_StartTime = @startTime
                                   AND Elt_EndTime=@endTime
                                   AND Elt_ControlNo NOT IN (  SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailment
							                                   WHERE Elt_EmployeeId = @EmployeeId
								                                 AND Elt_Status = '0'
							                                   UNION 
							                                  SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailmentHist
							                                   WHERE Elt_EmployeeId = @EmployeeId
								                                 AND Elt_Status = '0')
                                    
                                UNION 

                                SELECT Elt_ControlNo [Control No]
	                                 , Convert(varchar(10),Elt_LeaveDate,101) [Leave Date]
                                     , Elt_LeaveType [Type]
	                                 , Elt_StartTime [Start]
	                                 , Elt_EndTime [End]
                                  FROM T_EmployeeLeaveAvailmentHist
                                 WHERE Elt_EmployeeId = @EmployeeId
                                   AND Elt_Status IN ('1','3','5','7','9','A')
                                   AND Elt_ControlNo <> @ControlNo
                                   AND Elt_LeaveType = @LeaveType
                                   AND Elt_LeaveDate = @leaveDate
                                    AND Elt_StartTime = @startTime
                                   AND Elt_EndTime=@endTime
                                   AND Elt_ControlNo NOT IN (  SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailment
							                                   WHERE Elt_EmployeeId = @EmployeeId
								                                 AND Elt_Status = '0'
							                                   UNION 
							                                  SELECT Elt_RefControlNo
								                                FROM T_EmployeeLeaveAvailmentHist
							                                   WHERE Elt_EmployeeId = @EmployeeId
								                                 AND Elt_Status = '0') ";
        #endregion
        #region Combination Leave Check
        string sqlCombinedLeave = @"
declare @1stLeave as char(2)
declare @2ndLeave as char(2)
declare @isExist as varchar(7)
declare @isPaid AS BIT
SET @2ndLeave = @LeaveType
SET @isPaid = ( SELECT Ltm_PaidLeave FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @2ndLeave )
SET @1stLeave = ( SELECT TOP 1 Elt_LeaveType
					FROM (    
					SELECT Elt_LeaveType [Elt_LeaveType]
                    FROM T_EmployeeLeaveAvailment
                    LEFT JOIN T_LeaveTypeMaster	
						ON Ltm_LeaveType = Elt_LeaveType
                    WHERE Elt_Status IN ('1','3','5','7','9', 'A')
                     AND Elt_EmployeeId = @EmployeeId
                     AND Elt_LeaveDate = @leaveDate
                     AND Elt_ControlNo <> @ControlNo
                     AND Ltm_PaidLeave = @isPaid
                     AND Elt_ControlNo NOT IN ( SELECT Elt_RefControlNo
                                                  FROM T_EmployeeLeaveAvailment
                                                 WHERE Elt_EmployeeId = @EmployeeId
                                                   AND Elt_Status = '0'
                                                 UNION 
                                                SELECT Elt_RefControlNo
                                                  FROM T_EmployeeLeaveAvailmentHist
                                                 WHERE Elt_EmployeeId = @EmployeeId
                                                   AND Elt_Status = '0') 
                                                 
                   UNION
                  SELECT Elt_LeaveType [Elt_LeaveType]
                    FROM T_EmployeeLeaveAvailmentHist
                    LEFT JOIN T_LeaveTypeMaster	
						ON Ltm_LeaveType = Elt_LeaveType
                   WHERE Elt_Status IN ('1','3','5','7','9', 'A')
                     AND Elt_EmployeeId = @EmployeeId
                     AND Elt_LeaveDate = @leaveDate 
                     AND Elt_ControlNo <> @ControlNo
                     AND Ltm_PaidLeave = @isPaid
                     AND Elt_ControlNo NOT IN ( SELECT Elt_RefControlNo
                                                  FROM T_EmployeeLeaveAvailment
                                                 WHERE Elt_EmployeeId = @EmployeeId
                                                   AND Elt_Status = '0'
                                                 UNION 
                                                SELECT Elt_RefControlNo
                                                  FROM T_EmployeeLeaveAvailmentHist
                                                 WHERE Elt_EmployeeId = @EmployeeId
                                                   AND Elt_Status = '0')  
                   UNION
                  SELECT Eln_LeaveType [Elt_LeaveType]
                    FROM T_EmployeeLeaveNotice
                    LEFT JOIN T_LeaveTypeMaster	
						ON Ltm_LeaveType = Eln_LeaveType
                   WHERE Eln_Status IN ('1')
                     AND Eln_EmployeeId = @EmployeeId
                     AND Eln_LeaveDate = @leaveDate
                     AND Ltm_PaidLeave = @isPaid
                     AND Eln_ControlNo <> @NoticeControlNo ) AS TEMPTABLE)


SET @isExist = (SELECT Ltm_Status
                  FROM T_LeaveTypeMaster
                 WHERE ( LEFT(Ltm_LeaveDesc,7) = ISNULL(@1stLeave,'') + ' + ' + @2ndLeave
                      OR LEFT(Ltm_LeaveDesc,7) = @2ndLeave + ' + ' +  ISNULL(@1stLeave,'') )
                   AND Ltm_CombinedLeave = 1)

SELECT CASE WHEN (ISNULL(@1stLeave,'') = '') OR (@1stLeave = @2ndLeave)
            THEN 'A'
            ELSE CASE WHEN (ISNULL(@isExist, 'C') = 'C')
                      THEN @1stLeave + ' + ' + @2ndLeave
                      ELSE 'A'
                  END
        END  ";
        #endregion
        #region Get Job Status Perth Added 04/02/2012
        string sqlJobStatus = string.Format(@"
                SELECT
	                ISNULL(EMT_JOBSTATUS, '') [EMT_JOBSTATUS]
	                ,Ccd_CompanyCode
                FROM T_EMPLOYEEMASTER
                LEFT JOIN T_AccountDetail
                    ON Adt_AccountCode = Emt_JobStatus
                    AND Adt_AccountType = 'JOBSTATUS'
                LEFT JOIN T_CompanyMaster
                    ON 1 = 1
                WHERE EMT_EMPLOYEEID = '{0}'   
            ", this.txtEmployeeId.Text.Trim());
        #endregion
        #region Sql Leave Type for Clinic
        string sqlLeaveType = @"
    SELECT 
		CASE WHEN Ltm_UsedInClinic IS NOT NULL
			THEN 
				CASE WHEN Ltm_UsedInClinic = 1
					THEN 'TRUE'
					ELSE 'FALSE'
				END
			ELSE
				'FALSE'
		END [Ltm_UsedInClinic]
	FROM T_LeaveTypeMaster
	WHERE Ltm_LeaveType = @LeaveType
		AND Ltm_Status = 'A'
        ";
        #endregion
        #endregion

        ParameterInfo[] param = new ParameterInfo[7];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@leaveDate", dtpLVDate.Date.ToString("MM/dd/yyyy"));
        param[2] = new ParameterInfo("@startTime", txtLVStartTime.Text.Replace(":", ""));
        param[3] = new ParameterInfo("@endTime", txtLVEndTime.Text.Replace(":", ""));
        param[4] = new ParameterInfo("@LeaveType", ddlType.SelectedValue.ToUpper().Substring(0, 2));
        param[5] = new ParameterInfo("@ControlNo", txtControlNo.Text.ToUpper());
        param[6] = new ParameterInfo("@NoticeControlNo", hfControlNo.Value);

        #region For Clinic System
        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
        {
            if (this.hfClinicRec.Value != null
                && this.hfClinicRec.Value.ToString().Trim() != string.Empty)
            {
                #region Check if record was modified
                string clinErr = string.Empty;
                string tempChanges = this.dtpLVDate.Date.ToString("MM/dd/yyyy")
                                    + "|" + this.ddlType.SelectedValue.ToString()
                                    + "|" + this.txtRemarks.Text.Trim()
                                    + "|" + this.txtLVStartTime.Text.Trim()
                                    + "|" + this.txtLVEndTime.Text.Trim();
                string[] arrClinicRecords = this.hfClinicRec.Value.ToString().Trim().Split(new char[] { '|' });
                string[] arrChanges = tempChanges.Split(new char[] { '|' });
                if (!(arrClinicRecords.Length > arrChanges.Length))
                {
                    using (DALHelper dalClinic = GetDALHelperClinic())
                    {
                        try
                        {
                            dalClinic.OpenDB();
                            if (arrChanges[0].ToString().Trim() != arrClinicRecords[0].ToString().Trim()) //Leave Date
                            {
                                clinErr += "Changing of Saved Leave date is not allowed";
                            }

                            if (clinErr.Trim() == string.Empty)
                            {
                                if (arrChanges[1].ToString().Trim() != arrClinicRecords[1].ToString().Trim()) //Leave type
                                {
                                    DataSet dsClinicLeaves = dal.ExecuteDataSet(@"
SELECT 
	TOP 1 Ltm_LeaveDesc
FROM T_LeaveTypeMaster
WHERE Ltm_UsedInClinic = 1
	AND Ltm_LeaveType = @LeaveType
                                ", CommandType.Text, param);
                                    if (CommonMethods.isEmpty(dsClinicLeaves))
                                    {
                                        clinErr += string.Format("Leave Type {0} is not allowed", this.ddlType.Text.Trim());
                                    }
                                }

                            }
                            if (clinErr.Trim() == string.Empty)
                            {
                                if (arrChanges[2].ToString().Trim() != arrClinicRecords[2].ToString().Trim()) //Remarks
                                {
                                    clinErr += "Changing of Remarks is not allowed";
                                }
                            }
                            if (clinErr.Trim() == string.Empty)
                            {
                                if (arrChanges[3].ToString().Trim() != arrClinicRecords[3].ToString().Trim()) //Start Time
                                {
                                    clinErr += "Changing of Start Time is not allowed";
                                }
                            }
                            if (clinErr.Trim() == string.Empty)
                            {
                                if (arrChanges[4].ToString().Trim() != arrClinicRecords[4].ToString().Trim()) //End Time
                                {
                                    clinErr += "Changing of End Time is not allowed";
                                }
                            }
                        }
                        catch
                        {

                        }
                        finally
                        {
                            dalClinic.CloseDB();
                        }
                    }
                }
                if (clinErr.Trim() != string.Empty)
                {
                    err = clinErr;
                    return err;
                }
                #endregion
            }
            else if (this.txtStatus.Text.Trim() == string.Empty)
            {
                #region Check if Leave type is used for CLinic
                DataSet dsLeave = dal.ExecuteDataSet(string.Format(@"
SELECT
	CASE WHEN Ltm_WithCategory = 1
		THEN 'WITH CAT'
		ELSE Ltm_LeaveType
		END
FROM T_LeaveTypeMaster
WHERE Ltm_UsedInClinic = 1
	AND Ltm_LeaveType = '{0}'
	AND Ltm_LeaveType NOT IN ('UN','{1}')
                ", ddlType.SelectedValue.ToString().Substring(0, 2), Resources.Resource.UNPAIDLVECODE));
                if (!CommonMethods.isEmpty(dsLeave))
                {
                    if (dsLeave.Tables[0].Rows[0][0].ToString().Trim() == "WITH CAT")
                    {
                        if (ddlCategory.SelectedValue.ToString().Trim() == "SL")
                        {
                            err += "Leave type " + ddlType.Items[ddlType.SelectedIndex].Text.ToString()+ " with SICK LEAVE Category can only be filed by CLINIC";
                        }
                    }
                    else
                    {
                        err += "Leave type " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " can only be filed by CLINIC";
                    }
                    return err;
                }
                #endregion
            }
        }
        #endregion
        
        #region Check if Unpaid Leave is Used
        if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
        {
            if (ddlType.SelectedValue != null
                && ddlType.SelectedValue.Trim().IndexOf(Resources.Resource.UNPAIDLVECODE) != -1)
            {
                if (ddlCategory.Items.Count > 0)
                {
                    string category = this.ddlCategory.SelectedValue.ToString().Trim();
                    DataSet dsIsLeaveCategory = dal.ExecuteDataSet(string.Format(@"
SELECT 
	Ltm_WithCredit
FROM T_LeaveTypeMaster
WHERE Ltm_LeaveType = '{0}'
                    ", category));
                    if (!CommonMethods.isEmpty(dsIsLeaveCategory))
                    {
                        if (Convert.ToBoolean(dsIsLeaveCategory.Tables[0].Rows[0][0].ToString().Trim()))
                        {
                            string leaveYear = dtpLVDate.Date.Year.ToString();
                            if (dtpLVDate.Date.Year > Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR")))
                            {
                                if (dtpLVDate.Date < Convert.ToDateTime(Resources.Resource.LEAVEREFRESH + "/" + (Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR") + 1).ToString())))
                                {
                                    leaveYear = (dtpLVDate.Date.Year - 1).ToString();
                                }
                                else
                                {
                                    leaveYear = dtpLVDate.Date.ToString("yyyy");
                                }
                            }
                            else
                            {
                                leaveYear = dtpLVDate.Date.ToString("yyyy");
                            }

                            decimal deduct = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                          , txtLVEndTime.Text.Replace(":", "")
                                                          , rblDayUnit.SelectedValue.ToUpper()
                                                          , hfShiftCode.Value
                                                          , ddlType.SelectedValue.ToString().Substring(0, 2)
                                                          , true);
                            decimal credits = LVBL.getCredits(txtEmployeeId.Text
                                                             , leaveYear
                                                             , category + "00"
                                                             , dal);
                            
                            if (deduct <= credits)
                            {
                                err += "Cannot use unpaid leave, Leave Credits for " + this.ddlCategory.Text.Trim() + " is still available";
                            }
                        }
                    }
                }
            }
        }
        #endregion

        if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
          || !CommonMethods.isAffectedByCutoff("LEAVE", dtpLVDate.Date.ToString("MM/dd/yyyy")))
        {
            if (MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
            {
                #region If Cycle Cut-off
                err += CommonMethods.GetErrorMessageForCYCCUTOFF();
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                #region Leave overlapping check
                ds = dal.ExecuteDataSet(sqlConflicts, CommandType.Text, param);
                if (!CommonMethods.isEmpty(ds))
                {
                    err += "Leave conflicts:";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        err += @"\n    "
                             + ds.Tables[0].Rows[i]["Control No"].ToString()
                             + "  "
                             + ds.Tables[0].Rows[i]["Leave Date"].ToString()
                             + "   "
                             + ds.Tables[0].Rows[i]["Start"].ToString().Insert(2, ":")
                             + " - "
                             + ds.Tables[0].Rows[i]["End"].ToString().Insert(2, ":");
                    }
                }
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                #region Leave parameters checking
                ds = new DataSet();
                ds = dal.ExecuteDataSet(sqlParameters, CommandType.Text);
                //Perth Added 04/02/2012
                DataSet dsJobStatus = dal.ExecuteDataSet(sqlJobStatus, CommandType.Text);
                //End
                decimal temp = LVBL.calculateLeaveHoursHours( txtLVStartTime.Text.Replace(":", "")
                                                            , txtLVEndTime.Text.Replace(":", "")
                                                            , methods.GetProcessControlFlag("LEAVE", "DAYSEL") ? rblDayUnit.SelectedValue.ToUpper() : string.Empty
                                                            , hfShiftCode.Value
                                                            , ddlType.SelectedValue.Substring(0, 2)
                                                            , false);
                if (!CommonMethods.isEmpty(ds))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        switch (ds.Tables[0].Rows[i]["Pmt_ParameterId"].ToString().ToUpper())
                        {
                            case "LVFRACTION":
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 1
                                        && (temp * 60) % Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) != 0
                                        && !ddlType.SelectedValue.Substring(0, 2).Equals("UN")
                                        && !ddlType.SelectedValue.Substring(0, 2).Equals("OB"))
                                { 
                                    err += "\n Invalid entry in leave fraction. Must be divisible by " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString() + ".";
                                }
                                break;
                            case "MINLVHR":
                                //Perth Modified 04/02/2012 Added checking of job status halves min leave for HOGP managers
                                if (dsJobStatus != null
                                    && dsJobStatus.Tables[0].Rows.Count > 0
                                    && dsJobStatus.Tables[0].Rows[0]["Ccd_CompanyCode"].ToString().Trim().ToUpper() == "HOGP"
                                    && dsJobStatus.Tables[0].Rows[0]["EMT_JOBSTATUS"].ToString().Trim() == "AM")
                                {
                                    if (temp < (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) /2)
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("OB")
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("UN"))
                                    {
                                        err += "\nMinimum leave hour(s) is " + (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString()) / 2);
                                    }
                                }
                                else if(this.rblDayUnit.SelectedValue != null)
                                {
                                    if (rblDayUnit.SelectedValue.ToString().Trim() != "HA"
                                        && rblDayUnit.SelectedValue.ToString().Trim() != "HP"
                                        && temp < Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"])
                                        && !ddlType.SelectedValue.Substring(0, 2).Equals("OB")
                                        && !ddlType.SelectedValue.Substring(0, 2).Equals("UN"))
                                    {
                                        err += "\nMinimum leave hour(s) is " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString();
                                    }
                                }
                                else
                                {
                                    if (temp < Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"])
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("OB")
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("UN"))
                                    {
                                        err += "\nMinimum leave hour(s) is " + ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString();
                                    }
                                }
                                //End
                                break;
                            default:
                                break;
                        }
                    }
                } 
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                #region Duplicate entry for leave type on the same date
                ds = new DataSet();
                ds = dal.ExecuteDataSet(sqlDulicate, CommandType.Text, param);
                if (!CommonMethods.isEmpty(ds))
                {
                    err += "Cannot file the same leave type on the same day:";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        err += @"\n    "
                             + ds.Tables[0].Rows[i]["Control No"].ToString()
                             + "  "
                             + ds.Tables[0].Rows[i]["Leave Date"].ToString()
                             + "  ["
                             + ds.Tables[0].Rows[i]["Type"].ToString().Insert(2, ":")
                             + "]  "
                             + ds.Tables[0].Rows[i]["Start"].ToString().Insert(2, ":")
                             + " - "
                             + ds.Tables[0].Rows[i]["End"].ToString().Insert(2, ":");
                    }
                } 
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                #region Leave Combination check
                string combinedLeave = string.Empty;
                combinedLeave = dal.ExecuteScalar(sqlCombinedLeave, CommandType.Text, param).ToString();
                if (!combinedLeave.Equals("A"))
                {
                    err += "\n " + combinedLeave + " is not allowed on the same day.";
                }
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                #region Checking of leave credits if suffice for leave with credits
                #region Get Records From Previous
                ds = null;
                if (this.txtControlNo.Text.Trim() != string.Empty)
                {
                    ds = dal.ExecuteDataSet(@"
SELECT
	Elt_EmployeeID
    , CONVERT(VARCHAR(20), Elt_LeaveDate, 101) [Elt_LeaveDate]
	, Elt_LeaveType
	, Elt_StartTime
	, Elt_EndTime
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveAvailment
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Elt_LeaveType = Ltm_LeaveType
WHERE Elt_ControlNo = @ControlNo
	AND Ltm_WithCredit = 1
                    ", CommandType.Text, param);
                }
                bool isChanged = false;
                bool isNewState = true;
                #endregion
                if (!CommonMethods.isEmpty(ds))
                {
                    if (Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()).ToString("MM/dd/yyyy") != dtpLVDate.Date.ToString("MM/dd/yyyy")
                        ||ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim() != this.ddlType.SelectedValue.ToString().Substring(0, 2)
                        || ds.Tables[0].Rows[0]["Elt_StartTime"].ToString().Trim() != this.txtLVStartTime.Text.Trim().Replace(":", "")
                        || ds.Tables[0].Rows[0]["Elt_EndTime"].ToString().Trim() != this.txtLVEndTime.Text.Trim().Replace(":", ""))
                    {
                        isChanged = true;
                    }
                    isNewState = false;
                }

                if (isChanged)
                {
                    #region If Changed
                    if (ddlType.SelectedValue.ToUpper().Substring(0, 2)
                        != ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim())
                    {
                        if (LVBL.isCreditsDeductable(
                                        "CANCEL"
                                        , string.Format("{0}", LVBL.GetYear(Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()), ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()))
                                        , ds.Tables[0].Rows[0]["Elt_EmployeeID"].ToString().Trim()
                                        , ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()
                                        , ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()
                                        , dal))
                        {
                            if (!LVBL.isCreditsDeductable(
                                        "NEW"
                                        , string.Format("{0}", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty))
                                        , this.txtEmployeeId.Text.Trim()
                                        , this.ddlType.SelectedValue.ToString().Substring(0, 2)
                                        , this.txtLVStartTime.Text.Trim().Replace(":", "")
                                        , this.txtLVEndTime.Text.Trim().Replace(":", "")
                                        , this.rblDayUnit.SelectedValue.ToUpper()
                                        , this.hfShiftCode.Value
                                        , dal))
                            {
                                err += "Credits for " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " cannot suffice";
                            }
                        }
                        else
                        {
                            err = "Cannot cancel existing leave record. Cancelling will result to negative leave credits";
                        }
                    }
                    else
                    {
                        if (LVBL.isCreditsDeductable(
                                        "CANCEL"
                                        , string.Format("{0}", LVBL.GetYear(Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()), ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()))
                                        , ds.Tables[0].Rows[0]["Elt_EmployeeID"].ToString().Trim()
                                        , ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()
                                        , ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()
                                        , dal))
                        {
                            decimal dHour = LVBL.calculateLeaveHoursHours(
                                                    this.txtLVStartTime.Text.Trim().Replace(":", "")
                                                    , this.txtLVEndTime.Text.Trim().Replace(":", "")
                                                    , this.rblDayUnit.SelectedValue.ToUpper()
                                                    , this.hfShiftCode.Value
                                                    , this.ddlType.SelectedValue.ToString().Substring(0, 2)
                                                    , true);
                            decimal dToDeduct = 0;
                            bool isCreditsDeduct = true;
                            if (Convert.ToDecimal(ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()) > dHour)
                            {
                                dToDeduct = Convert.ToDecimal(ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()) - dHour;
                                isCreditsDeduct = LVBL.isCreditsDeductable(
                                        "CANCEL"
                                        , string.Format("{0}", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty))
                                        , this.txtEmployeeId.Text.Trim()
                                        , this.ddlType.SelectedValue.ToString().Substring(0, 2)
                                        , string.Format("{0:0.00}", dToDeduct)
                                        , dal);
                            }
                            else if (Convert.ToDecimal(ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()) < dHour)
                            {
                                dToDeduct = dHour - Convert.ToDecimal(ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim());
                                isCreditsDeduct = LVBL.isCreditsDeductable(
                                        "NEW"
                                        , string.Format("{0}", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty))
                                        , this.txtEmployeeId.Text.Trim()
                                        , this.ddlType.SelectedValue.ToString().Substring(0, 2)
                                        , string.Format("{0:0.00}", dToDeduct)
                                        , dal);
                            }
                            if (!isCreditsDeduct)
                            {
                                err += "Credits for " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " cannot suffice";
                            }
                        }
                        else
                        {
                            err = "Cannot cancel existing leave record. Cancelling will result to negative leave credits";
                        }
                    }
                    #endregion
                }
                else
                {
                    #region new save
                    if (isNewState)
                    {
                        #region New Save 
                        if (!LVBL.isCreditsDeductable(
                                                "NEW"
                                                , string.Format("{0}", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty))
                                                , this.txtEmployeeId.Text.Trim()
                                                , this.ddlType.SelectedValue.ToString().Substring(0, 2)
                                                , this.txtLVStartTime.Text.Trim().Replace(":", "")
                                                , this.txtLVEndTime.Text.Trim().Replace(":", "")
                                                , this.rblDayUnit.SelectedValue.ToUpper()
                                                , this.hfShiftCode.Value
                                                , dal))
                        {
                            err += "Credits for " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " cannot suffice";
                        }
                        DataTable dt = CheckCreditsNotNegative(this.txtEmployeeId.Text.Trim(), string.Empty, string.Empty);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            err += "No leave credits for " + this.ddlType.SelectedItem.ToString() + " for the year " + dtpLVDate.Date.Year;
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region PREVIOUS
                #region Information
                /************************************************************************************************************\
                 * First process of this is to check if there are changes in the entries, is so then we proceed to check if *
                 * the date of leave is within the year noted as  LEAVEYEAR in  T_ParameterMaster. LEAVEYEAR maybe not have *
                 * been updated as the year change in real time so we check leave date against server date year to proceed. *
                 * If LEAVEYEAR is equal to leave date year then check  against server and if the same  check and if not do *
                 * nothing credits maybe for next year's leaves.                                                            *
                 * If leave date  year is greater than LEAVEYEAR we need to check if we by pass checking. This serves as  a *
                 * feature we can turn of and on. ON for system not to deduct and OFF to still deduct even if leave date is *
                 * for next year. Control flag for this is LVCRBYPASS in LEAVE system.                                      *
                 * This scenarios must be the same with actual credits deduction.                                           *
                \************************************************************************************************************/
                #endregion
                //bool checkCredits = false;
                //if (!changeSnapShot().Equals(hfPrevEntry.Value))
                //{
                //    checkCredits = true;
                //}

                //if (checkCredits)
                //{
                //    string leaveYear = string.Format("{0}", LVBL.GetYear(dtpLVDate.Date)); //dtpLVDate.Date.Year.ToString();
                //    #region Check Credits
                //    decimal deduct = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                //                                            , txtLVEndTime.Text.Replace(":", "")
                //                                            , rblDayUnit.SelectedValue.ToUpper()
                //                                            , hfShiftCode.Value
                //                                            , ddlType.SelectedValue.ToString().Substring(0, 2)
                //                                            , true);
                //    decimal credits = LVBL.getCredits(txtEmployeeId.Text
                //                                        , leaveYear
                //                                        , ddlType.SelectedValue.ToUpper().Substring(0, 2)
                //                                        , dal);
                //    if (hfPrevType.Value.Equals(ddlType.SelectedValue.ToUpper()))
                //    {
                //        credits += LVBL.calculateLeaveHoursHours(hfPrevStart.Value.Replace(":", "")
                //                                                , hfPrevEnd.Value.Replace(":", "")
                //                                                , hfPrevDayUnit.Value
                //                                                , hfPrevShiftCode.Value
                //                                                , hfPrevType.Value
                //                                                , true);
                //    }

                //    if (deduct > credits)
                //    {
                //        err += "Credits for " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " cannot suffice";
                //    }
                //    #endregion
                //}                
                #endregion
            }
            if (err.Equals(string.Empty) && (txtDayCode.Text.Contains("SPL") || txtDayCode.Text.Contains("CMPY")))
            {
                #region Special Holidays Filing
                if (!LVBL.isPaidWithNoCreditsLeave(ddlType.SelectedValue.ToString().Substring(0, 2)))
                {
                    if (!CommonMethods.getEmployeePayrollType(txtEmployeeId.Text).Equals("D"))//is daily paid
                    {
                        err += "\nOnly daily paid employees can file leaves on Special Holidays";
                    }
                }
                #endregion
            }
            if (err.Equals(string.Empty))
            {
                DateTime dateFiled = Convert.ToDateTime(txtDateFiled.Text.Trim());
                DateTime dateLeave = Convert.ToDateTime(dtpLVDate.Date.ToShortDateString());
                DateTime[] arDate = GetDatesBetween(dateFiled, dateLeave).ToArray();
                DataTable dt;
                int numberOfDays = 0;
                #region Leave Type Parameter
                string paramClassification = ddlType.Text.Substring(0, 2).ToString();
                string query = string.Format(@"Select Pmx_ParameterValue
                                ,Pmx_Classification
                                from T_ParameterMasterExt
                                where Pmx_ParameterID ='LVDAYSFLB4'
                                and Pmx_Classification = '{0}'", paramClassification);
                #endregion
                dt = dal.ExecuteDataSet(query).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (ddlType.Text.Substring(0, 2).Equals(dt.Rows[0]["Pmx_Classification"]) && !ddlType.Text.Equals(string.Empty))
                    {

                        foreach (DateTime date in arDate)
                        {
                            if (numberOfDays < Convert.ToInt32(dt.Rows[0]["Pmx_ParameterValue"]))
                            {
                                bool logLedger = isInLogledger(date.ToShortDateString());
                                if (logLedger == true)
                                {
                                    numberOfDays += getLogLedger(date.ToShortDateString());
                                }
                                else
                                {
                                    numberOfDays += getHoliday(date.ToShortDateString());
                                    int HolidayValue = Convert.ToInt32(getHoliday(date.ToShortDateString()));
                                    if (HolidayValue <= 0)
                                    {
                                        numberOfDays += getRestDay(date);
                                        int RestDayValue = Convert.ToInt32(getRestDay(date));
                                        if (HolidayValue < 0 && RestDayValue == 0)
                                        {
                                            numberOfDays += 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (canFileLV != true)
                        {
                            if (dateLeave <= dateFiled)
                            {
                                err += string.Format("\nMust file leave minimum of {0} days before the actual leave date.", dt.Rows[0]["Pmx_ParameterValue"]);
                            }
                            else if (numberOfDays < Convert.ToInt32(dt.Rows[0]["Pmx_ParameterValue"]))
                            {
                                err += string.Format("\nMust file leave minimum of {0} days before the actual leave date.", dt.Rows[0]["Pmx_ParameterValue"]);
                            }
                        }
                    }
                }
            }
            if (err.Equals(string.Empty))
            {
                if (CommonMethods.isMismatchCostcenterAMS(dal, txtEmployeeId.Text, dtpLVDate.Date.ToString("MM/dd/yyyy")))
                {
                    err += "Cannot proceed with transaction, there is a mismatch in the cost center setup between AMS system.";
                }
            }
        }
        else
        {
            err += CommonMethods.GetErrorMessageForCutOff("LEAVE");
        }
        return err;
    }

    private string CheckIfLeaveCreditsSyncWithLeaveRecords(DALHelper dal, string transactType)
    {
        string err = string.Empty;
        string leaveYear = string.Format("{0}", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
        bool isCreditsSyncWithLeaveRecords = LVBL.IsCreditsSameBasedOnTransactions(
                        transactType
                        , txtEmployeeId.Text
                        , leaveYear
                        , ddlType.SelectedValue.ToString().Substring(0, 2)
                        , dal);
        if (!isCreditsSyncWithLeaveRecords)
        {
            err = "Leave Credits Doesnot sync with Leave Transactions, Credits have been modified outside of Workflow";
        }
        return err;
    }

    private void UpdateCredits(string type, DALHelper dal)
    {
        DataRow dr = PopulateDRforCredits();
        dr["Elt_ControlNo"] = this.txtControlNo.Text.Trim();

        if (type == "NEW")
        {
            #region New Transaction
            dr["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                               , txtLVEndTime.Text.Replace(":", "")
                                                               , rblDayUnit.SelectedValue
                                                               , hfShiftCode.Value
                                                               , ddlType.SelectedValue.Substring(0, 2)
                                                               , true);
            LVBL.LeaveControlNo = this.txtControlNo.Text.Trim();
            LVBL.UpdateCredits("NEW", dr, dal);
            #endregion
        }
        else
        {
            #region Get Data
            decimal dHours = LVBL.calculateLeaveHoursHours(this.hfPrevStart.Value.ToString().Replace(":", "")
                                                            , this.hfPrevEnd.Value.ToString().Replace(":", "")
                                                            , this.hfPrevDayUnit.Value
                                                            , this.hfShiftCode.Value
                                                            , this.hfPrevType.Value.ToString().Substring(0, 2)
                                                            , true);
            DataSet ds = dal.ExecuteDataSet(string.Format(@"
SELECT
	CONVERT(VARCHAR(20), Elt_LeaveDate, 101) [Elt_LeaveDate]
	, Elt_LeaveType
	, Elt_StartTime
	, Elt_EndTime
    , Elt_DayUnit
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveAvailment
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Elt_LeaveType = Ltm_LeaveType
WHERE Elt_ControlNo = '{0}'
            ", this.txtControlNo.Text.Trim()), CommandType.Text);
            #endregion
            if (!CommonMethods.isEmpty(ds))
            {
                if (type == "UPDATE")
                {
                    #region Update
                    dr["Elt_LeaveDate"] = Convert.ToDateTime(this.hfPrevLVDate.Value).ToString("MM/dd/yyyy");
                    dr["Elt_LeaveType"] = this.hfPrevType.Value.ToString().Substring(0, 2);
                    dr["Elt_LeaveHour"] = dHours;
                    dr["Elt_StartTime"] = this.hfPrevStart.Value.ToString().Replace(":", "");
                    dr["Elt_EndTime"] = this.hfPrevEnd.Value.ToString().Replace(":", "");
                    dr["Elt_DayUnit"] = this.hfPrevDayUnit.Value.ToString().Trim();
                    LVBL.UpdateCredits("CANCEL", dr, dal);

                    dr["Elt_LeaveDate"] = dtpLVDate.Date;
                    dr["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
                    dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                                       , txtLVEndTime.Text.Replace(":", "")
                                                                       , rblDayUnit.SelectedValue
                                                                       , hfShiftCode.Value
                                                                       , ddlType.SelectedValue.Substring(0, 2)
                                                                       , true);

                    dr["Elt_StartTime"] = txtLVStartTime.Text.Replace(":", "");
                    dr["Elt_EndTime"] = txtLVEndTime.Text.Replace(":", "");
                    dr["Elt_DayUnit"] = rblDayUnit.SelectedValue;
                    LVBL.UpdateCredits("NEW", dr, dal);
                    #endregion
                }
                else if (type == "CANCEL")
                {
                    #region Cancel
                    dr["Elt_LeaveDate"] = ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString();
                    dr["Elt_LeaveType"] = ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString();
                    dr["Elt_LeaveHour"] = ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString();
                    dr["Elt_StartTime"] = ds.Tables[0].Rows[0]["Elt_StartTime"].ToString();
                    dr["Elt_EndTime"] = ds.Tables[0].Rows[0]["Elt_EndTime"].ToString();
                    dr["Elt_DayUnit"] = ds.Tables[0].Rows[0]["Elt_DayUnit"].ToString();
                    LVBL.UpdateCredits("CANCEL", dr, dal);
                    #endregion
                }
                else if (type == "APPROVE")
                {
                    #region Approve
                    dr["Elt_LeaveDate"] = ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString();
                    dr["Elt_LeaveType"] = ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString();
                    dr["Elt_LeaveHour"] = ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString();
                    dr["Elt_StartTime"] = ds.Tables[0].Rows[0]["Elt_StartTime"].ToString();
                    dr["Elt_EndTime"] = ds.Tables[0].Rows[0]["Elt_EndTime"].ToString();
                    dr["Elt_DayUnit"] = ds.Tables[0].Rows[0]["Elt_DayUnit"].ToString();
                    LVBL.UpdateCredits("APPROVE", dr, dal);
                    #endregion
                }
            }
            else
            {
                throw new Exception("No Records retrieved for control no : " + this.txtControlNo.Text);
            }
        }
        //bool checkCredits = true;
        //decimal leaveYear = CommonMethods.getParamterValue("LEAVEYEAR");
        //int leaveYearDTP = Convert.ToInt32(leaveYear);
        //int leaveYearHF = Convert.ToInt32(leaveYear);
        //leaveYearDTP = LVBL.GetYear(dtpLVDate.Date);
        //if (!hfPrevDateCredits.Value.Equals(string.Empty))
        //{
        //    leaveYearHF = LVBL.GetYear(Convert.ToDateTime(hfPrevDateCredits.Value));
        //}
        //if (checkCredits)
        //{
        //    DataRow dr = PopulateDRforCredits();
        //    if (type.ToUpper().Equals("NEW"))
        //    {
        //        //if (leaveYearDTP.Equals(Convert.ToInt32(leaveYear)) || !methods.GetProcessControlFlag("LEAVE", "LVCRBYPASS"))
        //        //{
        //            dr["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
        //            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
        //                                                               , txtLVEndTime.Text.Replace(":", "")
        //                                                               , rblDayUnit.SelectedValue
        //                                                               , hfShiftCode.Value
        //                                                               , ddlType.SelectedValue.Substring(0, 2)
        //                                                               , true);
        //            LVBL.LeaveControlNo = this.txtControlNo.Text.Trim(); 
        //            LVBL.UpdateCredits("NEW", dr, dal);
        //        //}
        //    }
        //    else if (type.ToUpper().Equals("UPDATE"))
        //    {
        //        //if (Convert.ToDecimal(leaveYearHF) == leaveYear)
        //        //{
        //            dr["Elt_LeaveDate"] = hfPrevLVDate.Value.ToString();
        //            dr["Elt_LeaveType"] = hfPrevType.Value.Trim().Substring(0, 2);
        //            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(hfPrevStart.Value.Replace(":", "")
        //                                                               , hfPrevEnd.Value.Replace(":", "")
        //                                                               , hfPrevDayUnit.Value
        //                                                               , hfShiftCode.Value
        //                                                               , hfPrevType.Value.Substring(0, 2)
        //                                                               , true);
        //            dr["Elt_StartTime"] = hfPrevStart.Value.Replace(":", "");
        //            dr["Elt_EndTime"] = hfPrevEnd.Value.Replace(":", "");
        //            dr["Elt_DayUnit"] = hfPrevDayUnit.Value;
        //            LVBL.UpdateCredits("CANCEL", dr, dal);
        //        //}

        //        //if (Convert.ToDecimal(leaveYearDTP) == leaveYear)
        //        //{
        //            dr["Elt_LeaveDate"] = dtpLVDate.Date;
        //            dr["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
        //            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
        //                                                               , txtLVEndTime.Text.Replace(":", "")
        //                                                               , rblDayUnit.SelectedValue
        //                                                               , hfShiftCode.Value
        //                                                               , ddlType.SelectedValue.Substring(0, 2)
        //                                                               , true);

        //            dr["Elt_StartTime"] = txtLVStartTime.Text.Replace(":", "");
        //            dr["Elt_EndTime"] = txtLVEndTime.Text.Replace(":", "");
        //            dr["Elt_DayUnit"] = rblDayUnit.SelectedValue;

        //            LVBL.LeaveControlNo = this.txtControlNo.Text.Trim(); 
        //            LVBL.UpdateCredits("NEW", dr, dal);
        //        //}
        //    }
        //    else if (type.ToUpper().Equals("CANCEL"))
        //    {
        //        //if (Convert.ToDecimal(leaveYearHF) == leaveYear || !methods.GetProcessControlFlag("LEAVE", "LVCRBYPASS"))
        //        //{
        //            dr["Elt_LeaveType"] = hfPrevType.Value.Trim().Substring(0, 2);
        //            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(hfPrevStart.Value.Replace(":", "")
        //                                                               , hfPrevEnd.Value.Replace(":", "")
        //                                                               , hfPrevDayUnit.Value
        //                                                               , hfShiftCode.Value
        //                                                               , hfPrevType.Value.Substring(0, 2)
        //                                                               , true);
        //            LVBL.UpdateCredits("CANCEL", dr, dal);
        //        //}
        //    }
        //    else if (type.ToUpper().Equals("APPROVE"))
        //    {
        //        //if (Convert.ToDecimal(leaveYearHF) == leaveYear || !methods.GetProcessControlFlag("LEAVE", "LVCRBYPASS"))
        //        //{
        //            dr["Elt_LeaveType"] = hfPrevType.Value.Trim().Substring(0, 2);
        //            dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(hfPrevStart.Value.Replace(":", "")
        //                                                               , hfPrevEnd.Value.Replace(":", "")
        //                                                               , hfPrevDayUnit.Value
        //                                                               , hfShiftCode.Value
        //                                                               , hfPrevType.Value.Substring(0, 2)
        //                                                               , true);
        //            LVBL.LeaveControlNo = this.txtControlNo.Text.Trim(); 
        //            LVBL.UpdateCredits("APPROVE", dr, dal);
        //        //}
        //    }

        hfPrevDateCredits.Value = dtpLVDate.Date.ToString("MM/dd/yyyy");
        hfPrevType.Value = ddlType.SelectedValue.ToString().Substring(0, 2);
        hfPrevStart.Value = txtLVStartTime.Text.Replace(":", "");
        hfPrevEnd.Value = txtLVEndTime.Text.Replace(":", "");
        hfPrevDayUnit.Value = rblDayUnit.SelectedValue;
        hfPrevShiftCode.Value = hfShiftCode.Value;
        //}
    }

    private void capturePrevValues()
    {
        hfPrevDateCredits.Value = dtpLVDate.Date.ToString("MM/dd/yyyy");
        hfPrevType.Value = ddlType.SelectedValue;
        hfPrevStart.Value = txtLVStartTime.Text;
        hfPrevEnd.Value = txtLVEndTime.Text;
        hfPrevDayUnit.Value = rblDayUnit.SelectedValue;
        hfPrevShiftCode.Value = hfShiftCode.Value;
        hfPrevEntry.Value = changeSnapShot();
    }

    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = dtpLVDate.Date.ToString()
                 + ddlType.SelectedValue
                 + ddlCategory.SelectedValue
                 + txtLVStartTime.Text
                 + txtLVEndTime.Text
                 + rblDayUnit.SelectedValue
                 + txtReason.Text
                 + txtFiller1.Text
                 + txtFiller2.Text
                 + txtFiller3.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        clearValues();
        initializeEmployee();
        initializeControls();
        initializeLeaveParameters();
        this.tblClinicCertificates.Visible = false;
        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
        {
            this.divCertificates.InnerHtml = "";
        }
    }

    private void clearValues()
    {
        txtControlNo.Text = string.Empty;
        txtLVStartTime.Text = string.Empty;
        txtLVEndTime.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevLVDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfControlNo.Value = string.Empty;
        hfSaved.Value = "0";
        hfNoticeFlag.Value = "FALSE";
    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {

        //this is for saving leave transactions if fillers are not empty.
        // Filler Code (Cfl_ColName) will be saved in database so that other modules will not be affected with the change.
        //jlfegidero 02/11/16
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        if(txtFiller1.Text!=string.Empty)
        {
            DataSet dsFiller1 = getFillerCode(txtFiller1.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller1.Text + "'", "Elt_Filler01"));
            if (dsFiller1.Tables[0].Rows.Count > 0)
                filler1 = dsFiller1.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        if (txtFiller2.Text != string.Empty)
        {
            DataSet dsFiller2 = getFillerCode(txtFiller2.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller2.Text + "'", "Elt_Filler02"));
            if (dsFiller2.Tables[0].Rows.Count > 0)
                filler2 = dsFiller2.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        if (txtFiller3.Text != string.Empty)
        {
            DataSet dsFiller3 = getFillerCode(txtFiller3.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller3.Text + "'", "Elt_Filler03"));
            if (dsFiller3.Tables[0].Rows.Count > 0)
                filler3 = dsFiller3.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        //end

        DataRow dr = DbRecord.Generate("T_EmployeeLeaveAvailment");
        //Andre: removed condition. ALWAYS retreive current. 20130702
        //if (methods.GetProcessControlFlag("LEAVE", "CUT-OFF"))
        //{
        //    dr["Elt_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        //}
        //else
        //{
            dr["Elt_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        //}
        dr["Elt_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Elt_LeaveDate"] = dtpLVDate.Date.ToString("MM/dd/yyyy");
        dr["Elt_LeaveType"] = ddlType.SelectedValue.ToUpper().Substring(0, 2); 
        if (ddlCategory.SelectedValue.ToUpper().IndexOf("NOT APP") != -1)
        {
            dr["Elt_LeaveCategory"] = "";
        }
        else
        {
            dr["Elt_LeaveCategory"] = ddlCategory.SelectedValue.ToUpper();
        }
        dr["Elt_StartTime"] = txtLVStartTime.Text.Replace(":","");
        dr["Elt_EndTime"] = txtLVEndTime.Text.Replace(":","");
        dr["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                           , txtLVEndTime.Text.Replace(":", "")
                                                           , methods.GetProcessControlFlag("LEAVE", "DAYSEL") ? rblDayUnit.SelectedValue.ToUpper() : string.Empty
                                                           , hfShiftCode.Value
                                                           , ddlType.SelectedValue.Substring(0, 2)
                                                           , false);
        dr["Elt_DayUnit"] = rblDayUnit.SelectedValue.ToUpper();
        dr["Elt_Reason"] = txtReason.Text.ToUpper();
        dr["Elt_LeaveFlag"] = LVBL.getLeaveFlag(dtpLVDate.Date.ToString("MM/dd/yyyy"));
        dr["Elt_InformDate"] = txtDateFiled.Text;
        dr["Elt_CheckedBy"] = Session["userLogged"].ToString();
        dr["Elt_Checked2By"] = Session["userLogged"].ToString();
        dr["Elt_ApprovedBy"] = Session["userLogged"].ToString(); 
        dr["Elt_ControlNo"] = ControlNum;
        dr["Elt_Status"] = Status;
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Elt_LeaveNotice"] = Convert.ToBoolean(hfNoticeFlag.Value);
        dr["Elt_Filler1"] = ddlType.SelectedValue.Substring(0,2).Equals("SL") ? filler1.ToUpper() : string.Empty;
        dr["Elt_Filler2"] = filler2.ToUpper();
        dr["Elt_Filler3"] = filler3.ToUpper();

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

    private DataRow PopulateDRforCredits()
    {
        DataRow dr = DbRecord.Generate("T_EmployeeLeaveAvailment");
        dr["Elt_EmployeeId"] = txtEmployeeId.Text.ToString();
        dr["Elt_LeaveDate"] = dtpLVDate.Date.ToString("MM/dd/yyyy");
        dr["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
        dr["Elt_StartTime"] = txtLVStartTime.Text.Replace(":", "");
        dr["Elt_EndTime"] = txtLVEndTime.Text.Replace(":", "");
        dr["Elt_DayUnit"] = rblDayUnit.SelectedValue.ToUpper();
        dr["Elt_LeaveHour"] = 0;
        dr["Usr_Login"] = Session["userLogged"].ToString();

        return dr;
    }

    private void loadTransactionDetail()
    {
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        DataRow dr = LVBL.getLeaveInfo(Request.QueryString["cn"].Trim());
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
        dtpLVDate.Date = Convert.ToDateTime(dr["Leave Date"].ToString());
        txtDayCode.Text = dr["Day Code"].ToString();
        txtLVShift.Text = "[" + dr["Shift Code"].ToString() + "] "
                        + dr["Shift Time In"].ToString().Insert(2, ":") + "-"
                        + dr["Break Start"].ToString().Insert(2, ":") + "  "
                        + dr["Break End"].ToString().Insert(2, ":") + "-"
                        + dr["Shift Time Out"].ToString().Insert(2, ":");
        initializeLeaveParameters();
        bool flagFound = false;
        for (int i = 0; i < ddlType.Items.Count; i++)
        {
            if(ddlType.Items[i].Value.Length >= 2)
            {
                if (i > 0 && (ddlType.Items[i].Value.Substring(0, 2).ToUpper().Equals(dr["Leave Code"].ToString().ToUpper())))
                {
                    ddlType.SelectedIndex = i;
                    flagFound = true;
                    break;

                }
            }
        }
        //this is applicable for loading from checklist
        if (!flagFound)
        {
            ddlType.Items.Add(new ListItem(dr["Leave Desc"].ToString().ToUpper(), dr["Leave Code"].ToString().ToUpper()));
            ddlType.SelectedIndex = ddlType.Items.Count - 1;
        }
        ///end
        this.ddlType_SelectedIndexChanged(null, null);
        if (dr[CommonMethods.getFillerName("Elt_Filler01")] != "")
         showOptionalFields();
        bool flag = true;
        for (int i = 0; i < ddlCategory.Items.Count; i++)
        {
            if (ddlCategory.Items[i].Text.ToUpper().Equals(dr["Category"].ToString().ToUpper()))
            {
                ddlCategory.SelectedIndex = i;
                flag = false;
                break;
            }
        }
        if (flag)
        {
            ddlCategory.Enabled = false;
        }
        txtLVStartTime.Text = dr["Start"].ToString();
        txtLVEndTime.Text = dr["End"].ToString();
        switch (dr["Day Unit"].ToString())
        {
            case "WH":
                rblDayUnit.Items[0].Selected = true;
                break;
            case "HA":
                rblDayUnit.Items[1].Selected = true;
                break;
            case "QR":
                rblDayUnit.Items[2].Selected = true;
                break;
            case "HP":
                rblDayUnit.Items[3].Selected = true;
                break;
            default:
                break;
        }
        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        if (dr[CommonMethods.getFillerName("Elt_Filler01")].ToString() != string.Empty)
        {
            DataSet dsFiller1 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler01")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler01")].ToString() + "'", "Elt_Filler01"));
            if (dsFiller1.Tables[1].Rows.Count > 0)
                filler1 = dsFiller1.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller1.Visible = true;
        }
        if (dr[CommonMethods.getFillerName("Elt_Filler02")].ToString() != string.Empty)
        {
            DataSet dsFiller2 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler02")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler02")].ToString() + "'", "Elt_Filler02"));
            if (dsFiller2.Tables[1].Rows.Count > 0)
                filler2 = dsFiller2.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller2.Visible = true;
        }
        if (dr[CommonMethods.getFillerName("Elt_Filler03")].ToString() != string.Empty)
        {
            DataSet dsFiller3 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler03")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler03")].ToString() + "'", "Elt_Filler03"));
            if (dsFiller3.Tables[1].Rows.Count > 0)
                filler3 = dsFiller3.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller3.Visible = true;
        }
        txtFiller1.Text = filler1.ToUpper();
        txtFiller2.Text = filler2.ToUpper();
        txtFiller3.Text = filler3.ToUpper();

        hfPrevDateCredits.Value = dr["Leave Date"].ToString();
        hfPrevType.Value = dr["Leave Code"].ToString();
        hfPrevStart.Value = dr["Start"].ToString();
        hfPrevEnd.Value = dr["End"].ToString();
        hfPrevDayUnit.Value = dr["Day Unit"].ToString();
        hfPrevShiftCode.Value = dr["Shift Code"].ToString();
        hfNoticeFlag.Value = dr["Notice"].ToString();
        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
        hfLVHRENTRY.Value = methods.GetProcessControlFlag("LEAVE", "LVHRENTRY").ToString();
        hfLHRSINDAY.Value = CommonMethods.getParamterValue("LHRSINDAY").ToString();
        lblUnit.Text = (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS)) ? "UNIT: IN HOURS" : "UNIT: IN DAYS";
        refreshLeaveLedger();
        initializeShift();
        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "LEAVE", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeLeaveIndividual.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
        LoadCertificates(txtControlNo.Text.Trim());
    }

    private void loadTransactionDetail(string controlNo)
    {
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        DataRow dr = LVBL.getLeaveInfoFromNotice(controlNo);
        txtEmployeeId.Text = dr["ID No"].ToString();
        txtEmployeeName.Text = dr["Lastname"].ToString() + ", " + dr["Firstname"].ToString() + " " + (dr["Middlename"].ToString().Trim() == string.Empty ? "" : dr["Middlename"].ToString().Substring(0, 1) + ".");
        if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
        {
            txtNickname.Text = dr["Nickname"].ToString();
        }
        else
        {
            txtNickname.Text = string.Empty;
        }
        dtpLVDate.Date = Convert.ToDateTime(dr["Leave Date"].ToString());
        txtDayCode.Text = dr["Day Code"].ToString();
        txtLVShift.Text = "[" + dr["Shift Code"].ToString() + "] "
                        + dr["Shift Time In"].ToString().Insert(2, ":") + "-"
                        + dr["Break Start"].ToString().Insert(2, ":") + "  "
                        + dr["Break End"].ToString().Insert(2, ":") + "-"
                        + dr["Shift Time Out"].ToString().Insert(2, ":");
        txtLVStartTime.Text = dr["Start"].ToString();
        txtLVEndTime.Text = dr["End"].ToString();
        switch (dr["Day Unit"].ToString())
        {
            case "WH":
                rblDayUnit.Items[0].Selected = true;
                break;
            case "HA":
                rblDayUnit.Items[1].Selected = true;
                break;
            case "QR":
                rblDayUnit.Items[2].Selected = true;
                break;
            case "HP":
                rblDayUnit.Items[3].Selected = true;
                break;
            default:
                break;
        }
        txtReason.Text = dr["Reason"].ToString();
        if (dr[CommonMethods.getFillerName("Elt_Filler01")].ToString() != string.Empty)
        {
            DataSet dsFiller1 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler01")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler01")].ToString() + "'", "Elt_Filler01"));
            if (dsFiller1.Tables[1].Rows.Count > 0)
                filler1 = dsFiller1.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
            tbrFiller1.Visible = true;
        }
        if (dr[CommonMethods.getFillerName("Elt_Filler02")].ToString() != string.Empty)
        {
            DataSet dsFiller2 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler02")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler02")].ToString() + "'", "Elt_Filler02"));
            if (dsFiller2.Tables[1].Rows.Count > 0)
                filler2 = dsFiller2.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
        }
        if (dr[CommonMethods.getFillerName("Elt_Filler03")].ToString() != string.Empty)
        {
            DataSet dsFiller3 = getFillerCode(dr[CommonMethods.getFillerName("Elt_Filler03")].ToString(), getColumnFillerLookUp("Adt_AccountCode ='" + dr[CommonMethods.getFillerName("Elt_Filler03")].ToString() + "'", "Elt_Filler03"));
            if (dsFiller3.Tables[1].Rows.Count > 0)
                filler3 = dsFiller3.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();
        }
        txtFiller1.Text = filler1;
        txtFiller2.Text = filler2;
        txtFiller3.Text = filler3;


        for (int i = 0; i < ddlType.Items.Count; i++)
        {
            if (ddlType.Items[i].Value.Length >= 2)
            {
                if (ddlType.Items[i].Value.Substring(0, 2).ToUpper().Equals(dr["Leave Code"].ToString().ToUpper()))
                {
                    ddlType.SelectedIndex = i;
                    break;
                }
            }
        }
        this.ddlType_SelectedIndexChanged(null, null);
        showOptionalFields();
        bool flag = true;
        for (int i = 0; i < ddlCategory.Items.Count; i++)
        {
            if (ddlCategory.Items[i].Text.ToUpper().Equals(dr["Category"].ToString().ToUpper()))
            {
                ddlCategory.SelectedIndex = i;
                flag = false;
                break;
            }
        }
        if (flag)
        {
            ddlCategory.Enabled = false;
        }
        hfPrevDateCredits.Value = dr["Leave Date"].ToString();
        hfPrevType.Value = dr["Leave Code"].ToString();
        hfPrevStart.Value = dr["Start"].ToString();
        hfPrevEnd.Value = dr["End"].ToString();
        hfPrevDayUnit.Value = dr["Day Unit"].ToString();
        hfPrevShiftCode.Value = dr["Shift Code"].ToString();
        hfNoticeFlag.Value = "TRUE";
        //initialize controls
        txtControlNo.Text = string.Empty;
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
        hfLVHRENTRY.Value = methods.GetProcessControlFlag("LEAVE", "LVHRENTRY").ToString();
        hfLHRSINDAY.Value = CommonMethods.getParamterValue("LHRSINDAY").ToString();
        lblUnit.Text = (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS)) ? "UNIT: IN HOURS" : "UNIT: IN DAYS";
        initializeShift();
        enableControls();
        hfPrevEntry.Value = changeSnapShot();
        LoadCertificates(controlNo);
    }

    //Pert Added 08/15/2012
    private void LoadCertificates(string ControlNo)
    {
        #region Variables
        bool isSuccess = false;
        string ClinicControlNo = string.Empty;
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@Clm_LeaveControlNo", ControlNo);
        #endregion

        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
        {
            #region Check if Control Mapping Exists
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(@"
SELECT 
	Clm_ClinicControlNo
FROM T_ClinicLeavesMapping
WHERE Clm_LeaveControlNo = @Clm_LeaveControlNo
	AND Clm_Status = 'A'
                ", CommandType.Text, param);
                    if (ds != null
                        && ds.Tables[0].Rows.Count > 0)
                    {
                        ClinicControlNo = ds.Tables[0].Rows[0]["Clm_ClinicControlNo"].ToString().Trim();
                        isSuccess = true;
                    }
                }
                catch
                {
                    isSuccess = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            #endregion

            #region Display Certificate Links
            if (isSuccess && ClinicControlNo.Trim() != string.Empty)
            {
                using (DALHelper dal = GetDALHelperClinic())
                {
                    try
                    {
                        dal.OpenDB();
                        this.divCertificates.InnerHtml = "";
                        this.tblClinicCertificates.Visible = true;
                        DataSet dsLinks = dal.ExecuteDataSet(string.Format(@"
SELECT
	DISTINCT 
		Cpm_FileName
FROM T_CLinicPDFMapping
WHERE Cpm_ControlNo = '{0}'
                        ", ClinicControlNo));
                        string ViewingURL = "/Transactions/Leave/pgePDFViewing.aspx?vt=1&dx='" + ClinicControlNo + "'&df=";
                        Session["ClncControlNo"] = Encrypt.encryptText(ClinicControlNo);
                        if (dsLinks != null
                            && dsLinks.Tables.Count > 0
                            && dsLinks.Tables[0].Rows.Count > 0)
                        {
                            string html = "";
                            string imgSrc = "";
                            string fileLength = "";
                            for (int i = 0; i < dsLinks.Tables[0].Rows.Count; i++)
                            {
                                string fileName = dsLinks.Tables[0].Rows[i]["Cpm_FileName"].ToString().Trim();
                                html += "<div style=\"width:100%; height:auto; float:left;\"><a href=\"" + ViewingURL + fileName + "\" target=\"_blank\">" + fileName + "</a></div>";
                            }
                            if (html != "")
                            {
                                this.divCertificates.InnerHtml = html;
                            }
                        }

                        this.hfClinicRec.Value = this.dtpLVDate.Date.ToString("MM/dd/yyyy") 
                                + "|" + this.ddlType.SelectedValue.ToString()
                                + "|" + this.txtRemarks.Text.Trim()
                                + "|" + this.txtLVStartTime.Text.Trim()
                                + "|" + this.txtLVEndTime.Text.Trim();

                        EnableControls(false);
                    }
                    catch
                    {
                        this.hfClinicRec.Value = "";
                        this.divCertificates.InnerHtml = "";
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            else
            {
                this.hfClinicRec.Value = "";
                this.tblClinicCertificates.Visible = false;
                this.divCertificates.InnerHtml = "";
            }
            #endregion
        }
        else
        {
            this.hfClinicRec.Value = "";
            this.tblClinicCertificates.Visible = false;
            this.divCertificates.InnerHtml = "";
        }
    }

    private DALHelper GetDALHelperClinic()
    {
        string Cnck = Resources.Resource.CLINICSYSCNCK.ToString().Trim();
        System.Data.SqlClient.SqlConnectionStringBuilder builderCurrent
            = new System.Data.SqlClient.SqlConnectionStringBuilder(Encrypt.decryptText(Session["dbConn"].ToString()));
        System.Data.SqlClient.SqlConnectionStringBuilder builderClinic
            = new System.Data.SqlClient.SqlConnectionStringBuilder(Cnck);
        builderClinic.DataSource = builderCurrent.DataSource;
        Cnck = Encrypt.encryptText(builderClinic.ConnectionString);
        DALHelper dal = new DALHelper(Cnck);
        return dal;
    }

    private void EnableControls(bool isEnabled)
    {
        if (this.hfClinicRec.Value != null
            && this.hfClinicRec.Value.ToString().Trim() != string.Empty)
        {
            System.Web.UI.HtmlControls.HtmlImage cal = new System.Web.UI.HtmlControls.HtmlImage();
            cal = (System.Web.UI.HtmlControls.HtmlImage)dtpLVDate.Controls[2];

            cal.Disabled = !isEnabled;
            this.txtReason.ReadOnly = !isEnabled;
            this.txtLVStartTime.ReadOnly = !isEnabled;
            this.txtLVEndTime.ReadOnly = !isEnabled;
            this.rblDayUnit.Enabled = isEnabled;
            this.txtReason.BackColor = System.Drawing.Color.Gainsboro;
            this.txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
            this.txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
        }
    }

    private DataTable getLeaveTypePaidLeave(string leavetype)
    {
        string query = string.Format(@"Select Ltm_LeaveType
            FROM T_LeaveTypeMaster 
            Where Ltm_paidleave='1' and ltm_withcredit='0' and ltm_leavetype='{0}'", leavetype);
        DataTable dtResult;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }
        return dtResult;
    }

    //jlfegidero 02/11/16 - convert filler desc to filler code for saving and retreiving purposes
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
                        WHERE Cfl_ColName LIKE 'Elt_Filler%'
                          AND Cfl_Status = 'A'
                        and Cfl_Lookup = (SELECT TOP 1 Adt_AccountType
												FROM T_AccountDetail
                                                INNER JOIN T_ColumnFiller
												ON Cfl_Lookup = Adt_AccountType
												WHERE {0}
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
    private DataTable CheckCreditsNotNegative(string emmpId, string transaction, string transactionCondition)
    {
        DataTable dtErrors = new DataTable();
        dtErrors.Columns.Add("Employee ID");
        dtErrors.Columns.Add("Employee Name");
        dtErrors.Columns.Add("Error Message");
        dtErrors.Columns.Add("Reason");
        DataRow rowDetails = PopulateDRforCredits();

        #region New Transaction
        rowDetails["Elt_LeaveType"] = ddlType.SelectedValue.ToString().Substring(0, 2);
        rowDetails["Elt_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                           , txtLVEndTime.Text.Replace(":", "")
                                                           , rblDayUnit.SelectedValue
                                                           , hfShiftCode.Value
                                                           , ddlType.SelectedValue.Substring(0, 2)
                                                           , true);
        string leaveYear = string.Format("{0}", LVBL.GetYear(Convert.ToDateTime(rowDetails["Elt_LeaveDate"]), rowDetails["Elt_LeaveType"].ToString()));
        #endregion

        #region Transaction Type Checking
        //string transaction = string.Empty;
        //string transactionCondition = string.Empty;

        transaction = "Elm_Reserved = Elm_Reserved + @LeaveHours";
        transactionCondition = " AND ( Elm_Reserved < 0 OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0 )";
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            transactionCondition = " AND ( Elm_Reserved < 0 OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0 )";
        }

        #endregion

        #region Parameters
        decimal dToDeduct = Convert.ToDecimal(rowDetails["Elt_LeaveHour"]);
        try
        {
            dToDeduct = Convert.ToDecimal(rowDetails["CreditsToDeduct"]);
        }
        catch
        {
            dToDeduct = Convert.ToDecimal(rowDetails["Elt_LeaveHour"]);
        }
        ParameterInfo[] paramDetails = new ParameterInfo[5];
        paramDetails[0] = new ParameterInfo("@EmployeeID", rowDetails["Elt_EmployeeId"]);
        paramDetails[1] = new ParameterInfo("@LeaveType", rowDetails["Elt_LeaveType"]);
        paramDetails[2] = new ParameterInfo("@LeaveHours", dToDeduct);
        paramDetails[3] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
        paramDetails[4] = new ParameterInfo("@leaveyear", leaveYear.Trim());


        #endregion

        #region Check If Leaves are ok for Deduction
        DataSet ds = null;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(@"
                    DECLARE @WithCredit AS BIT
                    DECLARE @PartOf AS CHAR(2)
                    declare @DSP as bit
                                          SET @DSP = (SELECT Pcm_ProcessFlag
                                                        FROM T_ProcessControlMaster
                                                       WHERE Pcm_ProcessId = 'DSPFULLNM')
                    SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
                    SET @PartOf =     (SELECT CASE WHEN Ltm_PartOfLeave = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                                         FROM T_LeaveTypeMaster
                                        WHERE Ltm_LeaveType = @LeaveType)

                    IF (RTRIM(@leaveyear) = '')
                    BEGIN

                        IF (@WithCredit = 1)
                            BEGIN
                                IF (@PartOf <> @LeaveType)
                                    BEGIN
                                         SELECT Emt_EmployeeId [EmployeeId]
                                    , CASE WHEN (@DSP = '1')
                                        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                        ELSE Emt_NickName
                                    END AS [Employee Name]
                                    ,CASE WHEN (Elm_Entitled IS NULL)
                                        THEN    'NONE'
                                        ELSE Convert(varchar(10),Elm_Entitled - Elm_Used - Elm_Reserved) 
                                        END AS [Balance]
	                                 FROM T_EmployeeMaster
                                    LEFT JOIN t_employeeleave
                                    ON Emt_EmployeeId = Elm_EmployeeId
                                         WHERE Emt_EmployeeId = '{0}' AND (elm_leavetype = @PartOf
                                            {1}
                                            OR Elm_Leavetype is null)
                    
                                    END
                                    ELSE
				                    SELECT Emt_EmployeeId [EmployeeId]
                                    , CASE WHEN (@DSP = '1')
                                        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                        ELSE Emt_NickName
                                    END AS [Employee Name]
                                      ,CASE WHEN (Elm_Entitled IS NULL)
                                        THEN    'NONE'
                                        ELSE Convert(varchar(10),Elm_Entitled - Elm_Used - Elm_Reserved) 
                                        END AS [Balance]
	                                 FROM T_EmployeeMaster
                                    LEFT JOIN t_employeeleave
                                    ON Emt_EmployeeId = Elm_EmployeeId
                                    WHERE Emt_EmployeeId = '{0}' AND (elm_leavetype = @LeaveType
                                    {1} OR Elm_Leavetype is null)
                            END
                    END
                    ELSE
                    BEGIN

                        IF (@WithCredit = 1)
                            BEGIN
                                IF (@PartOf <> @LeaveType)
                                    BEGIN
                                         SELECT Emt_EmployeeId [EmployeeId]
                                    , CASE WHEN (@DSP = '1')
                                        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                        ELSE Emt_NickName
                                    END AS [Employee Name]
                                      ,CASE WHEN (Elm_Entitled IS NULL)
                                        THEN    'NONE'
                                        ELSE Convert(varchar(10),Elm_Entitled - Elm_Used - Elm_Reserved) 
                                        END AS [Balance]
	                                 FROM T_EmployeeMaster
                                    LEFT JOIN t_employeeleave
                                    ON Emt_EmployeeId = Elm_EmployeeId
                                        AND elm_leavetype = @PartOf
                                        AND Elm_LeaveYear = @leaveyear
                                             WHERE Emt_EmployeeId = '{0}' AND (1=0
                                        {1} OR Elm_Leavetype is null)
                                    END
                                    ELSE BEGIN
                                    SELECT Emt_EmployeeId [EmployeeId]
                                    , CASE WHEN (@DSP = '1')
                                        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                        ELSE Emt_NickName
                                    END AS [Employee Name]
                                      ,CASE WHEN (Elm_Entitled IS NULL)
                                        THEN    'NONE'
                                        ELSE Convert(varchar(10),Elm_Entitled - Elm_Used - Elm_Reserved) 
                                        END AS [Balance]
	                                 FROM T_EmployeeMaster
                                    LEFT JOIN t_employeeleave
                                    ON Emt_EmployeeId = Elm_EmployeeId
                                        AND elm_leavetype = @LeaveType
				                        AND Elm_LeaveYear = @leaveyear
                                    WHERE Emt_EmployeeId = '{0}' AND (1=0
                                    {1} OR Elm_Leavetype is null)
				                    END
                            END
                    END
                                ", emmpId, transactionCondition), CommandType.Text, paramDetails);
            }
            catch (Exception ex)
            { }
            finally
            { dal.CloseDB(); }
        }
        if (ds != null
            && ds.Tables.Count > 0
            && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0];
        }
        else
            return null;
        #endregion

    }
    private int getHoliday(string processDate)
    {
        string curYear = txtDateFiled.Text.Substring(6, 5).Trim();
        int paramValue = 0;
        string query = string.Format(@"Select Distinct Convert(varchar,Hmt_HolidayDate,101) as [HolidayDate]
                        from T_HolidayMaster
                        left join T_EmployeeMaster
					    ON (Hmt_ApplicCity= Emt_LocationCode OR Hmt_ApplicCity = 'ALL')
                        where Right(Convert(varchar,Hmt_HolidayDate, 101),4) like RTRIM('%{0}')
                        and Hmt_ApplicCity in (Emt_LocationCode, 'ALL')
						and Emt_EmployeeID = '{1}'
                        and Hmt_HolidayDate= Convert(varchar, '{2}', 101)
                        ", curYear, hfPrevEmployeeId.Value, processDate);

        DataSet ds;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            paramValue--;
        }
        return paramValue;
    }
    private int getRestDay(DateTime processDate)
    {
        int paramValue = 0;
        string query = string.Format(@"Select TOP 1 [Erd_RestDay] as [RestDay]
                        from T_EmployeeRestDay
                        where [Erd_EmployeeID] ='{0}'
                        order by Ludatetime desc ", hfPrevEmployeeId.Value);

        DataSet ds;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }
        if (ds.Tables[0].Rows.Count > 0)
        {
            if (processDate.DayOfWeek == System.DayOfWeek.Monday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(0, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Tuesday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(1, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Wednesday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(2, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Thursday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(3, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Friday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(4, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Saturday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(5, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
            else if (processDate.DayOfWeek == System.DayOfWeek.Sunday)
            {
                int day = Convert.ToInt32(ds.Tables[0].Rows[0]["RestDay"].ToString().Substring(6, 1));
                if (day == 0)
                {
                    paramValue++;
                }
            }
        }
        return paramValue;
    }
    private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
    {
        List<DateTime> allDates = new List<DateTime>();
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            allDates.Add(date);
        return allDates;

    }
    private bool isInLogledger(string processDate)
    {
        bool inlogLedger = false;
        string query = string.Format(@"SELECT CONVERT(varchar,Ell_ProcessDate, 101) as [Process Date]
                        FROM T_EmployeeLogLedger
                        WHERE Ell_EmployeeId='{0}'
                        AND CONVERT(varchar,Ell_ProcessDate, 101)= '{1}'", hfPrevEmployeeId.Value, processDate);
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }
        if (ds.Tables[0].Rows.Count > 0)
        {
            inlogLedger = true;
        }
        else
        {
            inlogLedger = false;
        }
        return inlogLedger;
    }
    private int getLogLedger(string processDate)
    {
        int paramValue = 0;
        string query = string.Format(@"Select Convert(varchar,Ell_ProcessDate, 101) as [Process Date]
                                ,Ell_DayCode as [Day Code]
                        from T_EmployeeLogLedger
                        where Ell_EmployeeId='{0}'
                        and Ell_RestDay=0 and Ell_Holiday=0
                        and Convert(varchar,Ell_ProcessDate, 101)= '{1}'", hfPrevEmployeeId.Value, processDate);
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dt = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }
        if (dt.Rows.Count > 0)
        {
            if (processDate == dt.Rows[0]["Process Date"].ToString())
            {
                paramValue++;
            }
        }
        return paramValue;
    }
    #endregion
    //end
}
