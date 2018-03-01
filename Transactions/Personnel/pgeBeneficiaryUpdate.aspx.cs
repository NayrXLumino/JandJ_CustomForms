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

public partial class Transactions_Personnel_pgeBeneficiaryUpdate : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    PersonnelBL PLBL = new PersonnelBL();
    MenuGrant MGBL = new MenuGrant();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBENEUPDATE"))
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
            LoadComplete += new EventHandler(Transactions_Personnel_pgeBeneficiaryUpdate_LoadComplete);
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

    void Transactions_Personnel_pgeBeneficiaryUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "personnelScripts";
        string jsurl = "_personnel.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        ddlType.Attributes.Add("OnChange", "javascript:return lookupBeneficiaryType();");
        btnHierarchy.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc('txtHierarchy','HIERARCHDP');");
        btnRelationship.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc('txtRelationship','RELATION');");
        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtHierarchyCode.Attributes.Add("readOnly", "true");
        txtHierarchyDesc.Attributes.Add("readOnly", "true");
        txtRelationshipCode.Attributes.Add("readOnly", "true");
        txtRelationshipDesc.Attributes.Add("readOnly", "true");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
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
                    using (DALHelper dal = new DALHelper())
                    {
                        string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "BNEFICIARY", btnX.Text.Trim().ToUpper());
                        EmailNotificationBL EMBL = new EmailNotificationBL();
                        EMBL.TransactionProperty = EmailNotificationBL.TransactionType.BENEFICIARY;
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
                                        PLBL.UpdateBFRecord(btnX.Text.Trim().ToUpper()
                                                            , drDetails
                                                            , dal);
                                        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                        MessageBox.Show("Successfully endorsed transaction.");
                                        break;
                                    case "ENDORSE TO CHECKER 2":
                                        PLBL.UpdateBFRecord(drDetails, dal);
                                        PLBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                        MessageBox.Show("Successfully endorsed transaction.");
                                        break;
                                    case "ENDORSE TO APPROVER":
                                        PLBL.UpdateBFRecord(drDetails, dal);
                                        PLBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                        MessageBox.Show("Successfully endorsed transaction.");
                                        break;
                                    case "APPROVE":
                                        PLBL.UpdateBFRecord(drDetails, dal);
                                        PLBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        PLBL.UpdateEmployeeBeneficiaryMaster( txtControlNo.Text
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
                                SystemMenuLogBL.InsertEditLog("WFBENEUPDATE", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
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
                            //if (drDetails != null && false)//change to ture to enable email no parameter setup yet
                            //{
                            //    try
                            //    {
                            //        CommonMethods.sendNotification("BENEFICIARY"
                            //                                        , txtEmployeeId.Text
                            //                                        , txtEmployeeName.Text
                            //                                        , status
                            //                                        , drDetails);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                            //    }
                            //}
                        }
                        else
                        {
                            MessageBox.Show("No route defined for user.");
                        }
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
                                PLBL.UpdateBFRecord(PopulateDR("2", txtControlNo.Text), dal);
                                //MenuLOg
                                SystemMenuLogBL.InsertEditLog("WFBENEUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                                    PLBL.UpdateBFRecord(PopulateDR(status, txtControlNo.Text), dal);
                                    PLBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    {
                                        EmailNotificationBL EMBL = new EmailNotificationBL();
                                        EMBL.TransactionProperty = EmailNotificationBL.TransactionType.BENEFICIARY;
                                        EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                        EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                      , Session["userLogged"].ToString()
                                                                      , dal);
                                    }
                                    //MenuLOg
                                    SystemMenuLogBL.InsertEditLog("WFBENEUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                                           
                                            string controlNo = CommonMethods.GetControlNumber("BNEFICIARY");
                                            if (controlNo.Equals(string.Empty))
                                            {
                                                MessageBox.Show("Transaction control for Beneficiary Update was not created.");
                                            }
                                            else
                                            {
                                                DataRow dr = PopulateDR("1", controlNo);
                                                if (ddlType.SelectedValue=="N")
                                                {
                                                    PLBL.CreateBFRecord(dr, dal);
                                                    txtControlNo.Text = controlNo;
                                                    txtStatus.Text = "NEW";
                                                    enableButtons();
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("New transaction saved.");
                                                }
                                                else
                                                {
                                                    PLBL.UpdateExistingBFRecord(dr, dal);
                                                    txtControlNo.Text = controlNo;
                                                    txtStatus.Text = "UPDATE";
                                                    enableButtons();
                                                    hfSaved.Value = "1";
                                                    MessageBox.Show("Update transaction saved.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            DataRow dr = PopulateDR("1", txtControlNo.Text);
                                            PLBL.UpdateBFRecordSave(dr, dal);
                                            hfSaved.Value = "1";
                                            MessageBox.Show("Transaction updated.");
                                        }
                                        ddlType.Enabled = PLBL.hasExistingBeneficiary(txtEmployeeId.Text);
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
                                        PLBL.UpdateBFRecord(PopulateDR("1", txtControlNo.Text), dal);
                                        PLBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.BENEFICIARY;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.RETURN;
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
                                        SystemMenuLogBL.InsertAddLog("WFBENEUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "",false);
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "PERSONNEL", "WFBENEUPDATE");
        btnEmployeeId.Enabled = userGrant.canAdd();
        btnEmployeeId.Enabled = true;
        ddlType.Enabled = PLBL.hasExistingBeneficiary(txtEmployeeId.Text);
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.Date = DateTime.Now;
        dtpEffectivityDate.MinDate = CommonMethods.getQuincenaDate('C', "START");
        dtpDeceasedDate.MaxDate = DateTime.Now.AddDays(-1);
        hfSaved.Value = "0";
        txtControlNo.Text = string.Empty;
        dtpEffectivityDate.Date = DateTime.Now;
        txtLastname.Text = string.Empty;
        txtFirstname.Text = string.Empty;
        txtMiddlename.Text = string.Empty;
        txtOccupation.Text = string.Empty;
        txtCompany.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
        txtHierarchyCode.Text = string.Empty;
        txtHierarchyDesc.Text = string.Empty;
        txtRelationshipCode.Text = string.Empty;
        txtRelationshipDesc.Text = string.Empty;
        cbxAccident.Checked = false;
        cbxBIR.Checked = false;
        cbxHMO.Checked = false;
        cbxInsurance.Checked = false;
        hfPrevEntry.Value = string.Empty;
        ddlType.SelectedIndex = 0;
        ddlMonth.SelectedIndex = 0;
        ddlDay.SelectedIndex = 0;
        ddlYear.SelectedIndex = 0;
        ddlCivilStatus.SelectedIndex = 0;
        ddlGender.SelectedIndex = 0;
        dtpDeceasedDate.Reset();
        dtpCancelDate.Reset();
        populateYear();
        enableControls();
        enableButtons();
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

                dtpDeceasedDate.Enabled = true;
                dtpCancelDate.Enabled = true;
                txtLastname.ReadOnly = false;
                txtFirstname.ReadOnly = false;
                txtMiddlename.ReadOnly = false;
                btnHierarchy.Enabled = true;
                btnRelationship.Enabled = true;
                txtReason.ReadOnly = false;
                txtOccupation.ReadOnly = false;
                txtCompany.ReadOnly = false;
                txtRemarks.ReadOnly = true;
                ddlMonth.Enabled = true;
                ddlDay.Enabled = true;
                ddlYear.Enabled = true;
                ddlGender.Enabled = true;
                ddlCivilStatus.Enabled = true;


                txtLastname.BackColor = System.Drawing.Color.White;
                txtFirstname.BackColor = System.Drawing.Color.White;
                txtMiddlename.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                txtOccupation.BackColor = System.Drawing.Color.White;
                txtCompany.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                //dtpEffectivityDate.Enabled = true;
                cal.Disabled = false;

                dtpDeceasedDate.Enabled = true;
                dtpCancelDate.Enabled = true;
                txtLastname.ReadOnly = false;
                txtFirstname.ReadOnly = false;
                txtMiddlename.ReadOnly = false;
                btnHierarchy.Enabled = true;
                btnRelationship.Enabled = true;
                txtReason.ReadOnly = false;
                txtOccupation.ReadOnly = false;
                txtCompany.ReadOnly = false;
                txtRemarks.ReadOnly = true;
                ddlMonth.Enabled = true;
                ddlDay.Enabled = true;
                ddlYear.Enabled = true;
                ddlGender.Enabled = true;
                ddlCivilStatus.Enabled = true;

                txtLastname.BackColor = System.Drawing.Color.White;
                txtFirstname.BackColor = System.Drawing.Color.White;
                txtMiddlename.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                txtOccupation.BackColor = System.Drawing.Color.White;
                txtCompany.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                dtpDeceasedDate.Enabled = false;
                dtpCancelDate.Enabled = false;
                txtLastname.ReadOnly = true;
                txtFirstname.ReadOnly = true;
                txtMiddlename.ReadOnly = true;
                btnHierarchy.Enabled = false;
                btnRelationship.Enabled = false;
                txtReason.ReadOnly = true;
                txtOccupation.ReadOnly = false;
                txtCompany.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                ddlMonth.Enabled = false;
                ddlDay.Enabled = false;
                ddlYear.Enabled = false;
                ddlGender.Enabled = true;
                ddlCivilStatus.Enabled = true;

                txtLastname.BackColor = System.Drawing.Color.Gainsboro;
                txtFirstname.BackColor = System.Drawing.Color.Gainsboro;
                txtMiddlename.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtOccupation.BackColor = System.Drawing.Color.White;
                txtCompany.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                dtpDeceasedDate.Enabled = false;
                dtpCancelDate.Enabled = false;
                txtLastname.ReadOnly = true;
                txtFirstname.ReadOnly = true;
                txtMiddlename.ReadOnly = true;
                btnHierarchy.Enabled = false;
                btnRelationship.Enabled = false;
                txtReason.ReadOnly = true;
                txtOccupation.ReadOnly = false;
                txtCompany.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                ddlMonth.Enabled = false;
                ddlDay.Enabled = false;
                ddlYear.Enabled = false;
                ddlGender.Enabled = true;
                ddlCivilStatus.Enabled = true;

                txtLastname.BackColor = System.Drawing.Color.Gainsboro;
                txtFirstname.BackColor = System.Drawing.Color.Gainsboro;
                txtMiddlename.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtOccupation.BackColor = System.Drawing.Color.White;
                txtCompany.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                dtpDeceasedDate.Enabled = false;
                dtpCancelDate.Enabled = false;
                txtLastname.ReadOnly = true;
                txtFirstname.ReadOnly = true;
                txtMiddlename.ReadOnly = true;
                btnHierarchy.Enabled = false;
                btnRelationship.Enabled = false;
                txtReason.ReadOnly = true;
                txtOccupation.ReadOnly = false;
                txtCompany.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                ddlMonth.Enabled = false;
                ddlDay.Enabled = false;
                ddlYear.Enabled = false;
                ddlGender.Enabled = true;
                ddlCivilStatus.Enabled = true;

                txtLastname.BackColor = System.Drawing.Color.Gainsboro;
                txtFirstname.BackColor = System.Drawing.Color.Gainsboro;
                txtMiddlename.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtOccupation.BackColor = System.Drawing.Color.White;
                txtCompany.BackColor = System.Drawing.Color.White;
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
            case "UPDATE":
                btnZ.Enabled = true;
                btnY.Enabled = true;
                btnX.Enabled = true;

                btnZ.Text = "SAVE";
                btnY.Text = "CANCEL";
                btnX.Text = "ENDORSE TO CHECKER 1";
                break;

            default:
                break;
        }
    }

    private void populateYear()
    {
        ddlYear.Items.Clear();
        ddlYear.Items.Add(new ListItem("", ""));
        for (int i = DateTime.Now.Year - 100; i < DateTime.Now.Year + 1; i++)
        { 
            ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        ddlYear.SelectedIndex = 0;
    }

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        DateTime temp=DateTime.Parse("01/01/0001");
        #region Birthdate
        try
        {
             temp = new DateTime(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue), Convert.ToInt32(ddlDay.SelectedValue));
        }
        catch
        {
            err += "Invalid birthdate.";
        }
        #endregion
        if (dtpDeceasedDate.Date.ToString("MM/dd/yyyy") != "01/01/0001" && err != "Invalid birthdate.")
        {
            if (dtpDeceasedDate.Date < temp)
                err += "Birthdate must be less than Deceased Date.";
        }
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
        if (ddlType.SelectedValue.Equals("U") && hfPrevEntry.Value.Equals(changeSnapShot()))
        {
            err += "\nInvalid transaction. No changes on beneficiary updating.";
        }
        return err;
    }

    private string checkEntry2(DALHelper dal)//Validation from DB parameters/data
    {
        DataSet dsTemp = new DataSet();
        string err = string.Empty;

        #region Hierarchy
        string sqlCheckHierarchy = string.Format(@"  SELECT But_Lastname
                                                          + ', '
                                                          + But_Firstname 
                                                          + ' '
                                                          + But_MiddleName [Name]
                                                       FROM T_BeneficiaryUpdate  
                                                      WHERE But_EmployeeId = '{0}'
                                                        AND But_Status IN ('1','3','5','7') 
                                                        AND But_Hierarchy = '{1}' 
                                                        AND But_ControlNo <> '{2}'
                                                      UNION
                                                     SELECT Ebm_Lastname
                                                          + ', '
                                                          + Ebm_Firstname 
                                                          + ' '
                                                          + Ebm_MiddleName [Name]
                                                       FROM T_EmployeeBeneficiary
                                                      WHERE Ebm_EmployeeId = '{0}'
                                                        AND Ebm_CancelDate IS NULL
                                                        AND Ebm_DeceaseDate IS NULL
                                                        AND Ebm_Hierarchy = '{1}' 
                                                        AND Ebm_BenSeqNo <> '{3}' ", txtEmployeeId.Text, txtHierarchyCode.Text, txtControlNo.Text, hfSeqNo.Value);

        //dsTemp = dal.ExecuteDataSet(sqlCheckHierarchy, CommandType.Text);
        if (!CommonMethods.isEmpty(dsTemp))
        {
            err += "Hierarchy is invalid. Hierarchy used by:";
            for (int i = 0; i < dsTemp.Tables[0].Rows.Count; i++)
            {
                err += "\n" + dsTemp.Tables[0].Rows[i]["Name"].ToString();
            }
        }
        #endregion
        #region Duplicate
        if (err.Equals(string.Empty))
        {
            string sqlDuplicate = string.Format(@" SELECT But_Lastname
                                                        + ', '
                                                        + But_Firstname 
                                                        + ' '
                                                        + But_MiddleName [Name]
                                                     FROM T_BeneficiaryUpdate  
                                                    WHERE But_EmployeeId = '{0}'
                                                      AND But_Status IN ('1','3','5','7') 
                                                      AND But_ControlNo <> '{1}'
                                                      AND But_Lastname
                                                        + But_Firstname 
                                                        + But_MiddleName
                                                        + Convert(varchar(10),But_Birthdate,101) = '{2}'
                                                    UNION
                                                   SELECT Ebm_Lastname
                                                        + ', '
                                                        + Ebm_Firstname 
                                                        + ' '
                                                        + Ebm_MiddleName [Name]
                                                     FROM T_EmployeeBeneficiary
                                                    WHERE Ebm_EmployeeId = '{0}'
                                                      AND Ebm_CancelDate IS NULL
                                                      AND Ebm_DeceaseDate IS NULL
                                                      AND Ebm_BenSeqNo <> '{3}'
                                                      AND Ebm_Lastname
                                                        + Ebm_Firstname 
                                                        + Ebm_MiddleName
                                                        + Convert(varchar(10),Ebm_Birthdate,101) ='{2}'  ", txtEmployeeId.Text.ToUpper()
                                                                                                            , txtControlNo.Text.ToUpper()
                                                                                                            , txtLastname.Text.Trim().ToUpper()
                                                                                                            + txtFirstname.Text.Trim().ToUpper()
                                                                                                            + txtMiddlename.Text.Trim().ToUpper()
                                                                                                            + new DateTime(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue), Convert.ToInt32(ddlDay.SelectedValue)).ToString("MM/dd/yyyy")
                                                                                                            , hfSeqNo.Value.ToUpper());
            dsTemp = dal.ExecuteDataSet(sqlDuplicate, CommandType.Text);
            if (!CommonMethods.isEmpty(dsTemp))
            {
                err += "Beneficiary already exists:";
                for (int i = 0; i < dsTemp.Tables[0].Rows.Count; i++)
                {
                    err += "\n" + dsTemp.Tables[0].Rows[i]["Name"].ToString();
                }
            }
        }


        if (err.Equals(string.Empty))
        {
            if (CommonMethods.isMismatchCostcenterAMS(dal, txtEmployeeId.Text, dtpEffectivityDate.Date.ToString("MM/dd/yyyy")))
            {
                err += "Cannot proceed with transaction, there is a mismatch in the cost center setup between AMS system.";
            }
        }
        #endregion

        if (ddlType.SelectedValue == "U" && hfSeqNo.Value.ToString() == "")
            err += "Please select existing beneficiary from lookup.";

        return err;
    }


    private DataRow PopulateDR(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_BeneficiaryUpdate");

        if (methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
        {
            dr["But_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        }
        else
        {
            dr["But_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        }
        dr["But_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["But_EffectivityDate"] = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        dr["But_Seqno"] = hfSeqNo.Value.ToString();
        dr["But_Type"] = ddlType.SelectedValue.ToString();
        dr["But_Lastname"] = txtLastname.Text.ToUpper();
        dr["But_Firstname"] = txtFirstname.Text.ToUpper();
        dr["But_Middlename"] = txtMiddlename.Text.ToUpper();            
        dr["But_Birthdate"] = Convert.ToDateTime(ddlMonth.SelectedValue + "/" + ddlDay.SelectedValue + "/" + ddlYear.SelectedValue);
        dr["But_Relationship"] = txtRelationshipCode.Text.ToUpper();
        dr["But_Hierarchy"] = txtHierarchyCode.Text.ToUpper();
        dr["But_HMODependent"] = (cbxHMO.Checked) ? 1 : 0; 
        dr["But_InsuranceDependent"] = (cbxInsurance.Checked) ? 1 : 0; 
        dr["But_BIRDependent"] = (cbxBIR.Checked) ? 1 : 0; 
        dr["But_AccidentDependent"] = (cbxAccident.Checked) ? 1 : 0;
        dr["But_DeceasedDate"] = dtpDeceasedDate.Date.ToString("MM/dd/yyyy");
        dr["But_CancelDate"] = dtpCancelDate.Date.ToString("MM/dd/yyyy");
        dr["But_Occupation"] = txtOccupation.Text.ToUpper();
        dr["But_Company"] = txtCompany.Text.ToUpper();
        dr["But_Gender"] = ddlGender.SelectedValue.ToString();
        dr["But_CivilStatus"] = ddlCivilStatus.SelectedValue.ToString(); 
        dr["But_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["But_Remarks"] = txtRemarks.Text.ToUpper();
        dr["But_CheckedBy"] = Session["userLogged"].ToString();
        dr["But_Checked2By"] = Session["userLogged"].ToString();
        dr["But_ApprovedBy"] = Session["userLogged"].ToString();
        dr["But_Status"] = Status.ToUpper();
        dr["But_ControlNo"] = ControlNum;
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
        snapShot = dtpEffectivityDate.Date.ToString("MM/dd/yyyy")
                 + txtLastname.Text.ToUpper()
                 + txtFirstname.Text.ToUpper()
                 + txtMiddlename.Text.ToUpper()
                 + txtOccupation.Text.ToUpper()
                 + txtCompany.Text.ToUpper()
                 + txtHierarchyCode.Text.ToUpper()
                 + txtHierarchyDesc.Text.ToUpper()
                 + txtRelationshipCode.Text.ToUpper()
                 + txtRelationshipDesc.Text.ToUpper()
                 + txtRemarks.Text.ToUpper()
                 + (dtpCancelDate.Date.ToString("MM/dd/yyyy").Equals("01/01/0001") ? string.Empty : dtpCancelDate.Date.ToString("MM/dd/yyyy"))
                 + (dtpDeceasedDate.Date.ToString("MM/dd/yyyy").Equals("01/01/0001") ? string.Empty : dtpDeceasedDate.Date.ToString("MM/dd/yyyy"))
                 + cbxAccident.Checked.ToString().ToUpper()
                 + cbxBIR.Checked.ToString().ToUpper()
                 + cbxHMO.Checked.ToString().ToUpper()
                 + cbxInsurance.Checked.ToString().ToUpper()
                 + ddlMonth.SelectedValue.ToString()
                 + ddlDay.SelectedValue.ToString()
                 + ddlYear.SelectedValue.ToString()
                 + ddlCivilStatus.SelectedValue.ToString()
                 + ddlGender.SelectedValue.ToString();
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        //initializeEmployee();
        initializeControls();
    }

    private void loadTransactionDetail()
    {
        DataRow dr = PLBL.getBeneficiaryInfo(Request.QueryString["cn"].Trim());
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
        txtLastname.Text = dr["Beneficiary Lastname"].ToString();
        txtFirstname.Text = dr["Beneficiary Firstname"].ToString();
        txtMiddlename.Text = dr["Beneficiary Middlename"].ToString();
        txtOccupation.Text = dr["Occupation"].ToString();
        txtCompany.Text = dr["Company"].ToString();
        ddlGender.SelectedValue = dr["Gender"].ToString();
        ddlCivilStatus.SelectedValue = dr["Civil Status"].ToString();
        txtHierarchyCode.Text = dr["Hierarchy Code"].ToString();
        txtHierarchyDesc.Text = dr["Hierarchy Desc"].ToString();
        txtRelationshipCode.Text = dr["Relationship Code"].ToString();
        txtRelationshipDesc.Text = dr["Relationship Desc"].ToString();
        cbxHMO.Checked = dr["HMO"].ToString().ToUpper().Equals("TRUE");
        cbxBIR.Checked = dr["BIR"].ToString().ToUpper().Equals("TRUE");
        cbxInsurance.Checked = dr["Insurance"].ToString().ToUpper().Equals("TRUE");
        cbxAccident.Checked = dr["Accident"].ToString().ToUpper().Equals("TRUE");
        try { dtpDeceasedDate.Date = Convert.ToDateTime(dr["Deceased Date"].ToString()); }
        catch { }
        try { dtpCancelDate.Date = Convert.ToDateTime(dr["Cancelled Date"].ToString()); }
        catch { }
        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        hfSeqNo.Value = dr["Seq No"].ToString();
        txtStatus.Text = dr["Status"].ToString();

       
        ddlMonth.SelectedIndex = Convert.ToDateTime(dr["Birthdate"].ToString()).Month;
        ddlDay.SelectedIndex = Convert.ToDateTime(dr["Birthdate"].ToString()).Day;
        int len = ddlYear.Items.Count;
        int maxYear = Convert.ToInt32(ddlYear.Items[len - 1].Value);
        int myYear = Convert.ToDateTime(dr["Birthdate"].ToString()).Year;
        ddlYear.SelectedValue = Convert.ToDateTime(dr["Birthdate"].ToString()).Year.ToString();
        if (dr["Type"].ToString().Equals("N"))
        {
            ddlType.SelectedIndex = 0;
        }
        else
        {
            ddlType.SelectedIndex = 1;
        }
        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.Date = DateTime.Now;
        dtpEffectivityDate.MinDate = CommonMethods.getQuincenaDate('C', "START");
        hfSaved.Value = "0";
        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "BNEFICIARY", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeBeneficiaryUpdate.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }
    #endregion
}
