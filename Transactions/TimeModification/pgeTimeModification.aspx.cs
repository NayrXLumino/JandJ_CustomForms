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

public partial class Transactions_TimeModification_pgeTimeModification : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    TimekeepBL TKBL = new TimekeepBL();
    MenuGrant MGBL = new MenuGrant();
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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFTIMERECENTRY"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
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
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
            }
            LoadComplete += new EventHandler(Transactions_TimeModification_pgeTimeModification_LoadComplete);
            if (Session["transaction"].ToString().Equals("CHECKLIST"))
            {
                loadTransactionDetail();
                Session["transaction"] = string.Empty;
                MessageBox.Show("Transaction loaded from checklist");
            }
            else if (Session["transaction"].ToString().Equals("PENDING"))
            {
                //try
                //{
                //  loadTransactionDetail();
                //  //MessageBox.Show("Transaction loaded from pending list");
                //  hfSaved.Value = "1";
                //}
                //catch(Exception ex)
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

    void Transactions_TimeModification_pgeTimeModification_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "timemodificationScripts";
        string jsurl = "_timemodification.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtAdjustmentDate.Attributes.Add("readOnly", "true");
        txtShift.Attributes.Add("readOnly", "true");
        btnAdjustmentDate.OnClientClick = "return lookupTKAdjustmentDate()";
        txtType.Attributes.Add("readOnly", "true");
        btnProximityLogs.OnClientClick = "return lookupTKProximityLogs()";

        txtTimeIn1.Attributes.Add("OnKeyDown", "javascript:validateEntry()");
        txtTimeIn1.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtTimeIn1.Attributes.Add("OnKeyUp", "javascript:autoAddColon('txtTimeIn1');");
        txtTimeIn1.Attributes.Add("autocomplete", "off");

        txtTimeOut1.Attributes.Add("OnKeyDown", "javascript:validateEntry()");
        txtTimeOut1.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtTimeOut1.Attributes.Add("OnKeyUp", "javascript:autoAddColon('txtTimeOut1');");
        txtTimeOut1.Attributes.Add("autocomplete", "off");

        txtTimeIn2.Attributes.Add("OnKeyDown", "javascript:validateEntry()");
        txtTimeIn2.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtTimeIn2.Attributes.Add("OnKeyUp", "javascript:autoAddColon('txtTimeIn2');");
        txtTimeIn2.Attributes.Add("autocomplete", "off");

        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
        txtTimeOut2.Attributes.Add("OnKeyDown", "javascript:validateEntry()");
        txtTimeOut2.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtTimeOut2.Attributes.Add("OnKeyUp", "javascript:autoAddColon('txtTimeOut2');");
        txtTimeOut2.Attributes.Add("autocomplete", "off");

        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        txtControlNo.Text = string.Empty;
        txtAdjustmentDate.Text = string.Empty;
        txtType.Text = string.Empty;
        txtTimeIn1.Text = string.Empty;
        txtTimeOut1.Text = string.Empty;
        txtTimeIn2.Text = string.Empty;
        txtTimeOut2.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeControls();
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        bool flagSuccessful = true;
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (hfPrevEntry.Value.Equals(changeSnapShot()))
                {
                    if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                      || !CommonMethods.isAffectedByCutoff("TIMEKEEP", txtAdjustmentDate.Text))
                    {
                        using (DALHelper dal = new DALHelper())
                        { 
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "TIMEMOD", btnX.Text.Trim().ToUpper());
                            EmailNotificationBL EMBL = new EmailNotificationBL();
                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TIMEMOD;
                            if (!status.Equals(string.Empty))
                            {
                                DataRow drDetails = PopulateDR(status, txtControlNo.Text);
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransactionSnapshot();
                                    switch (btnX.Text.ToUpper())
                                    {
                                        case "ENDORSE TO CHECKER 1":
                                            TKBL.UpdateTKRecord(btnX.Text.Trim().ToUpper()
                                                               , drDetails
                                                               , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO CHECKER 2":
                                            TKBL.UpdateTKRecord(drDetails, dal);
                                            TKBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO APPROVER":
                                            TKBL.UpdateTKRecord(drDetails, dal);
                                            TKBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "APPROVE":
                                            if (!CommonMethods.isAffectedByCutoff("TIMEKEEP", txtAdjustmentDate.Text))
                                            {
                                                TKBL.UpdateTKRecord(drDetails, dal);
                                                TKBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                                TKBL.EmployeeLogLedgerUpdate(drDetails
                                                                            , dal);
                                                EMBL.ActionProperty = EmailNotificationBL.Action.APPROVE;
                                                MessageBox.Show("Successfully approved transaction.");
                                            }
                                            else
                                            {
                                                flagSuccessful = false;
                                                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("TIMEKEEP"));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (flagSuccessful)
                                    {
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
                                        //MenuLOg
                                        SystemMenuLogBL.InsertEditLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
                                        restoreDefaultControls();
                                    }
                                    dal.CommitTransactionSnapshot();
                                }
                                catch (Exception ex)
                                {
                                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                                    drDetails = null;
                                    dal.RollBackTransactionSnapshot();
                                }
                                finally
                                {
                                    dal.CloseDB();
                                }
                            }
                            else
                            {
                                MessageBox.Show("No route defined for user.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("TIMEKEEP"));
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
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("TIMEKEEP", txtAdjustmentDate.Text))
                {
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransactionSnapshot();
                            switch (btnY.Text.ToUpper())
                            {
                                case "CLEAR":
                                    restoreDefaultControls();
                                    break;
                                case "CANCEL":
                                    TKBL.UpdateTKRecord(PopulateDR("2", txtControlNo.Text), dal);
                                    TKBL.TagTimeMod(txtAdjustmentDate.Text, txtEmployeeId.Text, "N", dal);
                                    //MenuLOg
                                    SystemMenuLogBL.InsertDeleteLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                    restoreDefaultControls();
                                    MessageBox.Show("Transaction cancelled");
                                    break;
                                case "DISAPPROVE":
                                    if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                    {
                                        string status = string.Empty;
                                        switch (txtStatus.Text.ToUpper())
                                        {
                                            case "ENDORSED TO CHECKER 1":
                                                status = "4";
                                                break;
                                            case "ENDORSED TO CHECKER 2":
                                                status = "6";
                                                break;
                                            case "ENDORSED TO APPROVER":
                                                status = "8";
                                                break;
                                            default:
                                                break;
                                        }
                                        TKBL.UpdateTKRecord(PopulateDR(status, txtControlNo.Text), dal);
                                        TKBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TIMEMOD;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
                                        //MenuLOg
                                        SystemMenuLogBL.InsertEditLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                        restoreDefaultControls();
                                        MessageBox.Show("Successfully disapproved transaction.");
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
                    }
                }   
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("TIMEKEEP"));
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
                  || !CommonMethods.isAffectedByCutoff("TIMEKEEP", txtAdjustmentDate.Text))
                {
                    string errMsg1 = checkEntry1();
                    if (errMsg1.Equals(string.Empty))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                dal.BeginTransactionSnapshot();
                                switch (btnZ.Text.ToUpper())
                                {
                                    case "SAVE":
                                        if (hfSaved.Value.Equals("0"))
                                        {
                                            string strmsg = checkEntry2(dal);
                                            if (strmsg.Trim() == string.Empty)
                                            {
                                                string controlNo = CommonMethods.GetControlNumber("TIMEMOD");
                                                if (controlNo.Equals(string.Empty))
                                                {
                                                    MessageBox.Show("Transaction control for Overtime was not created.");
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", controlNo);
                                                    TKBL.CreateTKRecord(dr, dal);
                                                    TKBL.TagTimeMod(txtAdjustmentDate.Text, txtEmployeeId.Text, "T", dal);
                                                    txtControlNo.Text = controlNo;
                                                    txtStatus.Text = "NEW";
                                                    enableButtons();
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("New transaction saved.");
                                                    //MenuLog
                                                    SystemMenuLogBL.InsertAddLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show(strmsg); 
                                            }
                                        }
                                        else
                                        {
                                            DataRow dr = PopulateDR("1", txtControlNo.Text);
                                            TKBL.UpdateTKRecordSave(dr, dal);
                                            hfSaved.Value = "1";
                                            MessageBox.Show("Transaction updated.");
                                            //MenuLog
                                            SystemMenuLogBL.InsertEditLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                        }
                                        hfPrevEntry.Value = changeSnapShot();
                                        break;
                                    case "RETURN TO EMPLOYEE":
                                        if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                        {
                                            TKBL.UpdateTKRecord(PopulateDR("1", txtControlNo.Text), dal);
                                            TKBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TIMEMOD;
                                                EMBL.ActionProperty = EmailNotificationBL.Action.RETURN;
                                                EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                              , Session["userLogged"].ToString()
                                                                              , dal);
                                            }
                                            //MenuLog
                                            SystemMenuLogBL.InsertEditLog("WFTIMERECENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                                            restoreDefaultControls();
                                            MessageBox.Show("Successfully returned transaction.");
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
                    }
                    else
                    {
                        MessageBox.Show(errMsg1);
                    }
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("TIMEKEEP"));
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFTIMERECENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfTimeModGap.Value = Convert.ToInt32(CommonMethods.getParamterValue("TIMEMODGAP")).ToString();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        enableControls();
        enableButtons();
        tbrProximityLogs.Visible = Convert.ToBoolean(Resources.Resource.VIEWLOGREFERENCE);
        hfBYPASSTIMEVERIFIACTION.Value = Resources.Resource.BYPASSTIMEVERIFICATION;
    }

    private void enableControls()
    {
        switch (txtStatus.Text.ToUpper())
        {
            case "":
                btnAdjustmentDate.Enabled = true;
                txtTimeIn1.ReadOnly = false;
                txtTimeOut1.ReadOnly = false;
                txtTimeIn2.ReadOnly = false;
                txtTimeOut2.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                btnAdjustmentDate.Enabled = true;
                txtTimeIn1.ReadOnly = false;
                txtTimeOut1.ReadOnly = false;
                txtTimeIn2.ReadOnly = false;
                txtTimeOut2.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                btnAdjustmentDate.Enabled = false;
                txtTimeIn1.ReadOnly = true;
                txtTimeOut1.ReadOnly = true;
                txtTimeIn2.ReadOnly = true;
                txtTimeOut2.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":

                btnAdjustmentDate.Enabled = false;
                txtTimeIn1.ReadOnly = true;
                txtTimeOut1.ReadOnly = true;
                txtTimeIn2.ReadOnly = true;
                txtTimeOut2.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":

                btnAdjustmentDate.Enabled = false;
                txtTimeIn1.ReadOnly = true;
                txtTimeOut1.ReadOnly = true;
                txtTimeIn2.ReadOnly = true;
                txtTimeOut2.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

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

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        #region Time In 1
        if (!txtTimeIn1.Text.Equals(string.Empty))
        {
            if (txtTimeIn1.Text.Length < 5)
            {
                err += "\nTime In 1 invalid format.(hh:mm)";
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(txtTimeIn1.Text.Substring(0, 2));
                    DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtTimeIn1.Text.Substring(3, 2));
                    decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtTimeIn1.Text.Substring(0, 2)) + Convert.ToDecimal(txtTimeIn1.Text.Substring(3, 2)) / 60);
                    temp = Math.Round(temp, 2);

                    if (Convert.ToInt32(txtTimeIn1.Text.Substring(3, 2)) > 59)
                    {
                        err += "\nTime In 1 minutes is invalid";
                    }
                    else if (temp > Convert.ToDecimal(71.98))
                    {
                        err += "\nTime In 1 maximum time exceeded.(up to 71:59)";
                    }
                    else if (!txtTimeIn1.Text.Contains(":"))
                    {
                        err += "\nTime In 1 invalid format.(hh:mm)";
                    }
                }
                catch
                {
                    err += "\nTime In 1 invalid format.(hh:mm)";
                }
            }
        }
        #endregion

        #region Time Out 1
        if (!txtTimeOut1.Text.Equals(string.Empty))
        {
            if (txtTimeOut1.Text.Length < 5)
            {
                err += "\nTime Out 1 invalid format.(hh:mm)";
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(txtTimeOut1.Text.Substring(0, 2));
                    DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtTimeOut1.Text.Substring(3, 2));
                    decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtTimeOut1.Text.Substring(0, 2)) + Convert.ToDecimal(txtTimeOut1.Text.Substring(3, 2)) / 60);
                    temp = Math.Round(temp, 2);

                    if (Convert.ToInt32(txtTimeOut1.Text.Substring(3, 2)) > 59)
                    {
                        err += "\nTime Out 1 minutes is invalid";
                    }
                    else if (temp > Convert.ToDecimal(71.98))
                    {
                        err += "\nTime Out 1 maximum time exceeded.(up to 71:59)";
                    }
                    else if (!txtTimeOut1.Text.Contains(":"))
                    {
                        err += "\nTime Out 1 invalid format.(hh:mm)";
                    }
                }
                catch
                {
                    err += "\nTime Out 1 invalid format.(hh:mm)";
                }
            }
        }
        #endregion

        #region Time In 2
        if (!txtTimeIn2.Text.Equals(string.Empty))
        {
            if (txtTimeIn2.Text.Length < 5)
            {
                err += "\nTime In 2 invalid format.(hh:mm)";
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(txtTimeIn2.Text.Substring(0, 2));
                    DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtTimeIn2.Text.Substring(3, 2));
                    decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtTimeIn2.Text.Substring(0, 2)) + Convert.ToDecimal(txtTimeIn2.Text.Substring(3, 2)) / 60);
                    temp = Math.Round(temp, 2);

                    if (Convert.ToInt32(txtTimeIn2.Text.Substring(3, 2)) > 59)
                    {
                        err += "\nTime In 2 minutes is invalid";
                    }
                    else if (temp > Convert.ToDecimal(71.98))
                    {
                        err += "\nTime In 2 maximum time exceeded.(up to 71:59)";
                    }
                    else if (!txtTimeIn2.Text.Contains(":"))
                    {
                        err += "\nTime In 2 invalid format.(hh:mm)";
                    }
                }
                catch
                {
                    err += "\nTime In 2 invalid format.(hh:mm)";
                }
            }
        }
        #endregion

        #region Time Out 2
        if (!txtTimeOut2.Text.Equals(string.Empty))
        {
            if (txtTimeOut2.Text.Length < 5)
            {
                err += "\nTime Out 2 invalid format.(hh:mm)";
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(txtTimeOut2.Text.Substring(0, 2));
                    DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtTimeOut2.Text.Substring(3, 2));
                    decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtTimeOut2.Text.Substring(0, 2)) + Convert.ToDecimal(txtTimeOut2.Text.Substring(3, 2)) / 60);
                    temp = Math.Round(temp, 2);

                    if (Convert.ToInt32(txtTimeOut2.Text.Substring(3, 2)) > 59)
                    {
                        err += "\nTime Out 2 minutes is invalid";
                    }
                    else if (temp > Convert.ToDecimal(71.98))
                    {
                        err += "\nTime Out 2 maximum time exceeded.(up to 71:59)";
                    }
                    else if (!txtTimeOut2.Text.Contains(":"))
                    {
                        err += "\nTime Out 2 invalid format.(hh:mm)";
                    }
                }
                catch
                {
                    err += "\nTime Out 2 invalid format.(hh:mm)";
                }
            }
        }
        #endregion

        if (err.Equals(string.Empty))//...proceed with other trappings
        {
            if (!hfShiftType.Value.ToString().Equals("G"))
            {
                if (!txtTimeOut1.Text.Equals(string.Empty))
                {
                    if (!txtTimeIn1.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeIn1.Text.Replace(":", "")) > Convert.ToInt32(txtTimeOut1.Text.Replace(":", "")))
                    {
                        err += "\nTime In 1 invalid.";
                    }
                }
                if (!txtTimeIn2.Text.Equals(string.Empty) && err.Equals(string.Empty))
                {
                    if (!txtTimeIn1.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeIn1.Text.Replace(":", "")) > Convert.ToInt32(txtTimeIn2.Text.Replace(":", "")))
                    {
                        err += "\nTime In 1 invalid.";
                    }
                    if (!txtTimeOut1.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeOut1.Text.Replace(":", "")) > Convert.ToInt32(txtTimeIn2.Text.Replace(":", "")))
                    {
                        err += "\nTime Out 1 invalid.";
                    }
                }
                if (!txtTimeOut2.Text.Equals(string.Empty) && err.Equals(string.Empty))
                {
                    if (!txtTimeIn1.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeIn1.Text.Replace(":", "")) > Convert.ToInt32(txtTimeOut2.Text.Replace(":", "")))
                    {
                        err += "\nTime In 1 invalid. Time Out must be greater.\n(For OUT for next day add 24:00)";
                    }
                    if (!txtTimeOut1.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeOut1.Text.Replace(":", "")) > Convert.ToInt32(txtTimeOut2.Text.Replace(":", "")))
                    {
                        err += "\nTime Out 1 invalid. Time Out must be greater.\n(For OUT for next day add 24:00)";
                    }
                    if (!txtTimeIn2.Text.Equals(string.Empty) && Convert.ToInt32(txtTimeIn2.Text.Replace(":", "")) > Convert.ToInt32(txtTimeOut2.Text.Replace(":", "")))
                    {
                        err += "\nTime In 2 invalid. Time Out must be greater.\n(For OUT for next day add 24:00)";
                    }
                }
            }

            if (txtReason.Text.Equals(string.Empty))
            {
                err += "\nEnter reason for modification.";
            }
        }

        if (err.Equals(string.Empty))
        {
            if (txtStatus.Text.Equals("NEW") || txtStatus.Text.Equals(""))
            {
                int shiftIn1 = (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(3, 2));
                int shiftOut1 = (Convert.ToInt32(hfO1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO1.Value.Substring(3, 2));
                int shiftIn2 = (Convert.ToInt32(hfI2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI2.Value.Substring(3, 2));
                int shiftOut2 = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(3, 2));

                int addOut1 = 0;
                int addIn2 = 0;
                int addOut2 = 0;

                if (hfShiftType.Value.Equals("G"))
                {
                    if (shiftOut1 < shiftIn1)
                    {
                        shiftOut1 += 1440;
                    }

                    if (shiftIn2 < shiftIn1)
                    {
                        shiftIn2 += 1440;
                    }

                    if (shiftOut2 < shiftIn1)
                    {
                        shiftOut2 += 1440;
                    }

                    #region Time Entry validation for graveyard
                    if (!txtTimeIn1.Text.Equals(string.Empty) && !txtTimeOut1.Text.Equals(string.Empty))
                    {
                        if (((Convert.ToInt32(txtTimeOut1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeOut1.Text.Substring(3, 2)))
                           < ((Convert.ToInt32(txtTimeIn1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn1.Text.Substring(3, 2))))
                        {
                            addOut1 = 1400;
                        }
                    }

                    if (!txtTimeIn1.Text.Equals(string.Empty) && !txtTimeIn2.Text.Equals(string.Empty))
                    {
                        if (((Convert.ToInt32(txtTimeIn2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn2.Text.Substring(3, 2)))
                           < ((Convert.ToInt32(txtTimeIn1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn1.Text.Substring(3, 2))))
                        {
                            addIn2 = 1400;
                        }
                    }

                    if (!txtTimeIn1.Text.Equals(string.Empty) && !txtTimeOut2.Text.Equals(string.Empty))
                    {
                        if (((Convert.ToInt32(txtTimeOut2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeOut2.Text.Substring(3, 2)))
                           < ((Convert.ToInt32(txtTimeIn1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn1.Text.Substring(3, 2))))
                        {
                            addOut2 = 1400;
                        }
                    }
                    else if (!txtTimeIn2.Text.Equals(string.Empty) && !txtTimeOut2.Text.Equals(string.Empty))
                    {
                        if (((Convert.ToInt32(txtTimeOut2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeOut2.Text.Substring(3, 2)))
                           < ((Convert.ToInt32(txtTimeIn2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn2.Text.Substring(3, 2))))
                        {
                            addOut2 = 1400;
                        }
                    }
                    #endregion

                }

                if (!txtTimeIn1.Text.Equals(string.Empty) && ((Convert.ToInt32(txtTimeIn1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn1.Text.Substring(3, 2))) > shiftOut1)
                {
                    err += "\nTime IN 1 is invalid. Log maybe placed at Time In 2.";
                }

                if (!txtTimeOut1.Text.Equals(string.Empty) && ((Convert.ToInt32(txtTimeOut1.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeOut1.Text.Substring(3, 2)) + addOut1) > shiftIn2)
                {
                    err += "\nTime OUT 1 is invalid. Log maybe placed at Time Out 2.";
                }

                if (!txtTimeIn2.Text.Equals(string.Empty) && ((Convert.ToInt32(txtTimeIn2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeIn2.Text.Substring(3, 2)) + addIn2) < shiftOut1)
                {
                    err += "\nTime IN 2 is invalid. Log maybe placed at Time IN 1.";
                }

                if (!txtTimeOut2.Text.Equals(string.Empty) && ((Convert.ToInt32(txtTimeOut2.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtTimeOut2.Text.Substring(3, 2)) + addOut2) < shiftIn2)
                {
                    err += "\nTime OUT 2 is invalid. Log maybe placed at Time Out 1.";
                }

                if ((txtTimeIn1.Text.Equals(txtTimeOut1.Text) && !txtTimeIn1.Text.Equals(string.Empty) && !txtTimeOut1.Text.Equals(string.Empty))
                  || (txtTimeIn1.Text.Equals(txtTimeIn2.Text) && !txtTimeIn1.Text.Equals(string.Empty) && !txtTimeIn2.Text.Equals(string.Empty))
                  || (txtTimeIn1.Text.Equals(txtTimeOut2.Text) && !txtTimeIn1.Text.Equals(string.Empty) && !txtTimeOut2.Text.Equals(string.Empty))

                  || (txtTimeOut1.Text.Equals(txtTimeIn2.Text) && !txtTimeOut1.Text.Equals(string.Empty) && !txtTimeIn2.Text.Equals(string.Empty))
                  || (txtTimeOut1.Text.Equals(txtTimeOut2.Text) && !txtTimeOut1.Text.Equals(string.Empty) && !txtTimeOut2.Text.Equals(string.Empty))

                  || (txtTimeIn2.Text.Equals(txtTimeOut2.Text) && !txtTimeIn2.Text.Equals(string.Empty) && !txtTimeOut2.Text.Equals(string.Empty)))
                {
                    err += "\nInvalid input. No logs should be the same.";
                }
            }
        }

        if (err.Equals(string.Empty))
        {
            if (MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
            {
                err += CommonMethods.GetErrorMessageForCYCCUTOFF();
            }
        }
        return err;
    }

    private string checkEntry2(DALHelper dal)
    {
        DataSet ds = new DataSet();
        string err = string.Empty;

        string sqlCheckDuplicate = @"
SELECT 
	Trm_ControlNo [Control No]
	, TMERECTYPE.Adt_AccountDesc [Modification Type]
	, WFSTATUS.Adt_AccountDesc [Status]	
FROM T_TimeRecMod
LEFT JOIN T_AccountDetail WFSTATUS
	ON WFSTATUS.Adt_AccountType = 'WFSTATUS'
	AND WFSTATUS.Adt_AccountCode = Trm_Status
LEFT JOIN T_AccountDetail TMERECTYPE
	ON TMERECTYPE.Adt_AccountType = 'TMERECTYPE'
	AND TMERECTYPE.Adt_AccountCode = Trm_Type
WHERE Trm_EmployeeId = @EmployeeID
	AND Trm_Status IN ('1','3','5','7')
	AND Trm_ControlNo <> @ControlNo
	AND Trm_ModDate = @ModDate
        ";

        ParameterInfo[] param = new ParameterInfo[3];
        param[0] = new ParameterInfo("@EmployeeID", this.txtEmployeeId.Text.Trim());
        param[1] = new ParameterInfo("@ControlNo", this.txtControlNo.Text.Trim());
        param[2] = new ParameterInfo("@ModDate", txtAdjustmentDate.Text.Trim());

        if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            ds = dal.ExecuteDataSet(sqlCheckDuplicate, CommandType.Text, param);
            if (!CommonMethods.isEmpty(ds))
            {
                err += "Time Modification transaction already exists.\nRefer to : ["
                        + ds.Tables[0].Rows[0]["Control No"].ToString()
                        + "] - " + ds.Tables[0].Rows[0]["Modification Type"].ToString()
                        + " - " + ds.Tables[0].Rows[0]["Status"].ToString();
            }

            if (err.Equals(string.Empty))
            {
                if (CommonMethods.isMismatchCostcenterAMS(dal, txtEmployeeId.Text,Convert.ToDateTime(txtAdjustmentDate.Text).ToString("MM/dd/yyyy")))
                {
                    err += "Cannot proceed with transaction, there is a mismatch in the cost center setup between AMS system.";
                }
            }
        }
        else
        {
            err += CommonMethods.GetErrorMessageForCYCCUTOFF();
        }
        return err;
    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_TimeRecMod");

        if (methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
        {
            dr["Trm_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        }
        else
        {
            dr["Trm_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        }
        dr["Trm_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Trm_ModDate"] = txtAdjustmentDate.Text;
        dr["Trm_Type"] = hfModType.Value.ToString();
        dr["Trm_ActualTimeIn1"] = txtTimeIn1.Text.ToString().Replace(":", "");
        dr["Trm_ActualTimeOut1"] = txtTimeOut1.Text.ToString().Replace(":", "");
        dr["Trm_ActualTimeIn2"] = txtTimeIn2.Text.ToString().Replace(":", "");
        dr["Trm_ActualTimeOut2"] = txtTimeOut2.Text.ToString().Replace(":", "");
        dr["Trm_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["Trm_CheckedBy"] = Session["userLogged"].ToString();
        dr["Trm_Checked2By"] = Session["userLogged"].ToString();
        dr["Trm_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Trm_Status"] = Status.ToUpper();
        dr["Trm_ControlNo"] = ControlNum;
        dr["Trm_Flag"] = TKBL.ComputeTKFlag(txtAdjustmentDate.Text).ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Trm_LogControl"] = hfLogControl.Value.ToString();

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
    
    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = txtAdjustmentDate.Text
                 + txtType.Text
                 + txtTimeIn1.Text
                 + txtTimeOut1.Text
                 + txtTimeIn2.Text
                 + txtTimeOut2.Text
                 + txtReason.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtControlNo.Text = string.Empty;
        txtAdjustmentDate.Text = string.Empty;
        txtShift.Text = string.Empty;
        txtType.Text = string.Empty;
        txtTimeIn1.Text = string.Empty;
        txtTimeOut1.Text = string.Empty;
        txtTimeIn2.Text = string.Empty;
        txtTimeOut2.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfLogControl.Value = "!!!!";
        hfSaved.Value = "0";
        initializeEmployee();
        initializeControls();
    }

    private void loadTransactionDetail()
    {
        DataRow dr = TKBL.getTimeRecordInfo(Request.QueryString["cn"].Trim());
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
        txtAdjustmentDate.Text = dr["Adjustment Date"].ToString();
        txtShift.Text = "[" + dr["Shift Code"].ToString() + "] "
                        + dr["Shift Time In"].ToString().Insert(2, ":") + "-"
                        + dr["Break Start"].ToString().Insert(2, ":") + "  "
                        + dr["Break End"].ToString().Insert(2, ":") + "-"
                        + dr["Shift Time Out"].ToString().Insert(2, ":");

        hfI1.Value = dr["Shift Time In"].ToString().Insert(2, ":");
        hfO1.Value = dr["Break Start"].ToString().Insert(2, ":");
        hfI2.Value = dr["Break End"].ToString().Insert(2, ":");
        hfO2.Value = dr["Shift Time Out"].ToString().Insert(2, ":");
        hfShiftType.Value = dr["Schedule Type"].ToString();

        txtType.Text = dr["Mod Description"].ToString();
        hfModType.Value = dr["Mod Code"].ToString();
        txtTimeIn1.Text = dr["Time In 1"].ToString();
        txtTimeOut1.Text = dr["Time Out 1"].ToString();
        txtTimeIn2.Text = dr["Time In 2"].ToString();
        txtTimeOut2.Text = dr["Time Out 2"].ToString();
        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();
        hfLogControl.Value = dr["Log Control"].ToString();

        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "TIMEMOD", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeTimeModification.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }
    #endregion
}
