using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

public partial class Transactions_Personnel_pgeAddressPermanent : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    PersonnelBL PRBL = new PersonnelBL();
    MenuGrant MGBL = new MenuGrant();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFADDPERMA"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
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
            }
            LoadComplete += new EventHandler(Transactions_Personnel_pgeAddressPermanent_LoadComplete);
            if (Session["transaction"].ToString().Equals("CHECKLIST"))
            {
                loadTransactionDetail();
                Session["transaction"] = string.Empty;
                MessageBox.Show("Transaction loaded from checklist");
            }
            else if (Session["transaction"].ToString().Equals("PENDING"))
            {
                //loadTransactionDetail();
                Session["transaction"] = string.Empty;
                //MessageBox.Show("Transaction loaded from pending list");
                //hfSaved.Value = "1";
            }
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Personnel_pgeAddressPermanent_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "personnelScripts";
        string jsurl = "_personnel.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtAddress2Code.Attributes.Add("readOnly", "true");
        txtAddress2Desc.Attributes.Add("readOnly", "true");
        txtAddress3Code.Attributes.Add("readOnly", "true");
        txtAddress3Desc.Attributes.Add("readOnly", "true");
        btnAddress2.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc(" + "'txtAddress2','BARANGAY');");
        btnAddress3.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc(" + "'txtAddress3','ZIPCODE');");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");

        txtAddress1.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        initializeControls();
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (hfPrevEntry.Value.Equals(changeSnapShot()))
                {
                    if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "ADDRESS", btnX.Text.Trim().ToUpper());
                            EmailNotificationBL EMBL = new EmailNotificationBL();
                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.ADDRESS;
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
                                            PRBL.UpdateADRecord(btnX.Text.Trim().ToUpper()
                                                               , drDetails
                                                               , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO CHECKER 2":
                                            PRBL.UpdateADRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO APPROVER":
                                            PRBL.UpdateADRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "APPROVE":
                                            PRBL.UpdateADRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            PRBL.UpdateEmployeeMasterAddress(txtControlNo.Text
                                                                             , Session["userLogged"].ToString()
                                                                             , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.APPROVE;
                                            MessageBox.Show("Successfully approved transaction.");
                                            break;
                                        default:
                                            break;
                                    }
                                    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    {
                                        EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                      , Session["userLogged"].ToString()
                                                                      , dal);
                                    }
                                    //MenuLog
                                    SystemMenuLogBL.InsertEditLog("WFADDPERMA", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
                                    restoreDefaultControls();
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
                        MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
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
                if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
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
                                    PRBL.UpdateADRecord(PopulateDR("2", txtControlNo.Text), dal);
                                    //MenuLOg
                                    SystemMenuLogBL.InsertDeleteLog("WFADDPERMA", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                                        PRBL.UpdateADRecord(PopulateDR(status, txtControlNo.Text), dal);
                                        PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.ADDRESS;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
                                        //MenuLOg
                                        SystemMenuLogBL.InsertEditLog("WFADDPERMA", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                    MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
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
                if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
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
                                        string errMsg2 = checkEntry2(dal);
                                        if (errMsg2.Equals(string.Empty))
                                        {
                                            if (hfSaved.Value.Equals("0"))
                                            {
                                                string controlNo = CommonMethods.GetControlNumber("ADDRESS");
                                                if (controlNo.Equals(string.Empty))
                                                {
                                                    MessageBox.Show("Transaction control for Address Update was not created.");
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", controlNo);
                                                    PRBL.CreateADRecord(dr, dal);
                                                    txtControlNo.Text = controlNo;
                                                    txtStatus.Text = "NEW";
                                                    enableButtons();
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("New transaction saved.");
                                                }
                                            }
                                            else
                                            {
                                                DataRow dr = PopulateDR("1", txtControlNo.Text);
                                                PRBL.UpdateADRecordSave(dr, dal);
                                                hfSaved.Value = "1";
                                                MessageBox.Show("Transaction updated.");
                                            }
                                            hfPrevEntry.Value = changeSnapShot();
                                        }
                                        else
                                        {
                                            MessageBox.Show(errMsg2);
                                        }
                                        break;
                                    case "RETURN TO EMPLOYEE":
                                        if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                        {
                                            PRBL.UpdateADRecord(PopulateDR("1", txtControlNo.Text), dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.ADDRESS;
                                                EMBL.ActionProperty = EmailNotificationBL.Action.RETURN;
                                                EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                              , Session["userLogged"].ToString()
                                                                              , dal);
                                            }
                                           
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
                                //MenuLog

                                SystemMenuLogBL.InsertAddLog("WFADDPERMA", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
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
                    MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "PERSONNEL", "WFADDPERMA");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.Date = DateTime.Now;
        dtpEffectivityDate.MinDate = CommonMethods.getQuincenaDate('C', "START");
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;

        initializeCurrentValues();

        enableControls();
        enableButtons();
    }

    private void initializeCurrentValues()
    {
        #region Address

        string sqlAddress = @"  SELECT Emt_EmployeeProvAddress1 [Address1]
                                     , Address2.Adt_AccountCode [Address2 Code]
                                     , Address2.Adt_AccountDesc [Address2 Desc]
                                     , Address3.Adt_AccountCode [Address3 Code]
                                     , Address3.Adt_AccountDesc [Address3 Desc]
                                     , Emt_EmployeeProvTelephoneNo [Telephone No]
                                  FROM T_EmployeeMaster
                                  LEFT JOIN T_AccountDetail Address2
                                    ON Address2.Adt_AccountCode = Emt_EmployeeProvAddress2
                                   AND Address2.Adt_AccountType = 'BARANGAY'
                                  LEFT JOIN T_AccountDetail Address3
                                    ON Address3.Adt_AccountCode = Emt_EmployeeProvAddress3
                                   AND Address3.Adt_AccountType = 'ZIPCODE'
                                 WHERE Emt_EmployeeId = '{0}'";
        DataSet dsAddress = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                
                dsAddress = dal.ExecuteDataSet(string.Format(sqlAddress, txtEmployeeId.Text), CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }

        }
        if (!CommonMethods.isEmpty(dsAddress))
        {
            txtAddress1.Text = dsAddress.Tables[0].Rows[0]["Address1"].ToString();
            txtAddress2Code.Text = dsAddress.Tables[0].Rows[0]["Address2 Code"].ToString();
            txtAddress2Desc.Text = dsAddress.Tables[0].Rows[0]["Address2 Desc"].ToString();
            txtAddress3Code.Text = dsAddress.Tables[0].Rows[0]["Address3 Code"].ToString();
            txtAddress3Desc.Text = dsAddress.Tables[0].Rows[0]["Address3 Desc"].ToString();
            txtTelephoneNo.Text = dsAddress.Tables[0].Rows[0]["Telephone No"].ToString();
        }
        else
        {
            txtAddress2Code.Text = string.Empty;
            txtAddress2Desc.Text = "     -  INVALID BARANGAY CODE. Code is not in master.  -";
            txtAddress3Code.Text = string.Empty;
            txtAddress3Desc.Text = "     -  INVALID ZIPCODE . Code is not in master.  -";
        }
        #endregion
    }

    private void enableControls()
    {
        System.Web.UI.HtmlControls.HtmlImage cal = new System.Web.UI.HtmlControls.HtmlImage();
        cal = (System.Web.UI.HtmlControls.HtmlImage)dtpEffectivityDate.Controls[2];

        switch (txtStatus.Text.ToUpper())
        {
            case "":
                //dtpEffectivityDate.Enabled = true;
                cal.Disabled = false;

                txtAddress1.ReadOnly = false;
                txtTelephoneNo.ReadOnly = false;

                btnAddress2.Enabled = true;
                btnAddress3.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                //dtpEffectivityDate.Enabled = true;
                cal.Disabled = false;

                txtAddress1.ReadOnly = false;
                txtTelephoneNo.ReadOnly = false;

                btnAddress2.Enabled = true;
                btnAddress3.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                txtAddress1.ReadOnly = true;
                txtTelephoneNo.ReadOnly = true;

                btnAddress2.Enabled = false;
                btnAddress3.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                txtAddress1.ReadOnly = true;
                txtTelephoneNo.ReadOnly = true;

                btnAddress2.Enabled = false;
                btnAddress3.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                txtAddress1.ReadOnly = true;
                txtTelephoneNo.ReadOnly = true;

                btnAddress2.Enabled = false;
                btnAddress3.Enabled = false;
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

        if (txtAddress1.Text.Equals(string.Empty))
        {
            err += "\nNumber/Street is required.";
        }
        if (Regex.IsMatch(txtTelephoneNo.Text, "[^\\d+-]"))
        {
            err += "\nTelephone Number must contain only numbers, + and -";
        }
        if (txtReason.Text.Equals(string.Empty))
        {
            err += "\nReason for update is required.";
        }
        return err;
    }

    private string checkEntry2(DALHelper dal)//Validation from DB parameters/data
    {
        DataSet ds = new DataSet();
        string err = string.Empty;
        string sqlEnroute = string.Format(@"SELECT Amt_ControlNo [Control No]
                                                 , Adt_AccountDesc [Status]
                                              FROM T_AddressMovement
                                             INNER JOIN T_AccountDetail
                                                ON Adt_AccountCode = Amt_Status
                                               AND Adt_AccountType = 'WFSTATUS'
                                             WHERE Amt_EmployeeId = '{0}'
                                               AND Amt_Type = 'A2'
                                               AND Amt_Status IN ('1','3','5','7') 
                                               AND Amt_ControlNo <> '{1}'", txtEmployeeId.Text, txtControlNo.Text);
        if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            ds = dal.ExecuteDataSet(sqlEnroute, CommandType.Text);
            if (!CommonMethods.isEmpty(ds))
            {
                err += "\nThere is already a transaction on route.\nRefer to " + ds.Tables[0].Rows[0]["Control No"].ToString() + " " + ds.Tables[0].Rows[0]["Status"].ToString();
            }

            if (err.Equals(string.Empty))
            {
                if (CommonMethods.isMismatchCostcenterAMS(dal, txtEmployeeId.Text, dtpEffectivityDate.Date.ToString("MM/dd/yyyy")))
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
        DataRow dr = DbRecord.Generate("T_AddressMovement");

        dr["Amt_ControlNo"] = ControlNum.ToUpper();
        dr["Amt_EmployeeId"] = txtEmployeeId.Text;
        dr["Amt_EffectivityDate"] = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        dr["Amt_AppliedDate"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Amt_Type"] = "A2";
        dr["Amt_Address1"] = txtAddress1.Text.ToUpper();
        dr["Amt_Address2"] = txtAddress2Code.Text;
        dr["Amt_Address3"] = txtAddress3Code.Text;
        dr["Amt_TelephoneNo"] = txtTelephoneNo.Text;
        dr["Amt_Reason"] = txtReason.Text.ToUpper();
        dr["Amt_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Amt_CheckedBy"] = Session["userLogged"].ToString();
        dr["Amt_Checked2By"] = Session["userLogged"].ToString();
        dr["Amt_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Amt_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();

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
        snapShot = dtpEffectivityDate.Date.ToString()
                 + txtAddress1.Text
                 + txtAddress2Code.Text
                 + txtAddress3Code.Text
                 + txtTelephoneNo.Text
                 + txtReason.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtControlNo.Text = string.Empty;
        txtAddress1.Text = string.Empty;
        txtAddress2Code.Text = string.Empty;
        txtAddress2Desc.Text = string.Empty;
        txtAddress3Code.Text = string.Empty;
        txtAddress3Desc.Text = string.Empty;
        txtTelephoneNo.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeEmployee();
        initializeControls();
    }

    private void loadTransactionDetail()
    {
        DataRow dr = PRBL.getAddressInfo(Request.QueryString["cn"].Trim());
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
        dtpEffectivityDate.Date = Convert.ToDateTime(dr["Effectivity Date"].ToString());

        txtAddress1.Text = dr["Address1"].ToString();
        txtAddress2Code.Text = dr["Address2 Code"].ToString();
        txtAddress2Desc.Text = dr["Address2 Desc"].ToString();
        txtAddress3Code.Text = dr["Address3 Code"].ToString();
        txtAddress3Desc.Text = dr["Address3 Desc"].ToString();
        txtTelephoneNo.Text = dr["TelephoneNo"].ToString();

        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.MinDate = CommonMethods.getQuincenaDate('C', "START");
        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "ADDRESS", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeAddressPermanent.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }
    #endregion
}