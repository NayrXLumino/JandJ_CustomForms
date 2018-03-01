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


public partial class Transactions_Payroll_pgeTaxCodeCivilStatus : System.Web.UI.Page
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
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFTAXCODE"))
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
            LoadComplete += new EventHandler(Transactions_Payroll_pgeTaxCodeCivilStatus_LoadComplete);
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

    void Transactions_Payroll_pgeTaxCodeCivilStatus_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "payrollScripts";
        string jsurl = "_payroll.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtFromCivilCode.Attributes.Add("readOnly", "true");
        txtFromCivilDesc.Attributes.Add("readOnly", "true");
        txtFromTaxCodeCode.Attributes.Add("readOnly", "true");
        txtFromTaxCodeDesc.Attributes.Add("readOnly", "true");
        txtToCivilCode.Attributes.Add("readOnly", "true");
        txtToCivilDesc.Attributes.Add("readOnly", "true");
        txtToTaxCodeCode.Attributes.Add("readOnly", "true");
        txtToTaxCodeDesc.Attributes.Add("readOnly", "true");
        btnTaxCode.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc(" + "'txtToTaxCode','TAXCODE');");
        btnCivilStatus.Attributes.Add("OnClick", "javascript:return lookupAccountCodeDesc(" + "'txtToCivil','CIVILSTAT');");
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
                    if (!methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
                    {
                        using (DALHelper dal = new DALHelper())
                        { 
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "TAXMVMNT", btnX.Text.Trim().ToUpper());
                            EmailNotificationBL EMBL = new EmailNotificationBL();
                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TAXCIVIL;
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
                                            PRBL.UpdateTXRecord( btnX.Text.Trim().ToUpper()
                                                               , drDetails
                                                               , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO CHECKER 2":
                                            PRBL.UpdateTXRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO APPROVER":
                                            PRBL.UpdateTXRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "APPROVE":
                                            PRBL.UpdateTXRecord(drDetails, dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            PRBL.UpdateEmployeeMasterTaxCivil( txtControlNo.Text
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
                                    SystemMenuLogBL.InsertEditLog("WFTAXCODE", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
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
                                //        CommonMethods.sendNotification("TAXMVMNT"
                                //                                      , txtEmployeeId.Text
                                //                                      , txtEmployeeName.Text
                                //                                      , status
                                //                                      , drDetails);
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
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("PAYROLL"));
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
            if (!methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
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
                                PRBL.UpdateTXRecord(PopulateDR("2", txtControlNo.Text), dal);
                                //MenuLOg
                                SystemMenuLogBL.InsertDeleteLog("WFTAXCODE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                                    PRBL.UpdateTXRecord(PopulateDR(status, txtControlNo.Text), dal);
                                    PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    {
                                        EmailNotificationBL EMBL = new EmailNotificationBL();
                                        EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TAXCIVIL;
                                        EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                        EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                        , Session["userLogged"].ToString()
                                                                        , dal);
                                    }
                                    //MenuLOg
                                    SystemMenuLogBL.InsertEditLog("WFTAXCODE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("PAYROLL"));
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
                if (!methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
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
                                                string controlNo = CommonMethods.GetControlNumber("TAXMVMNT");
                                                if (controlNo.Equals(string.Empty))
                                                {
                                                    MessageBox.Show("Transaction control for Tax Code & Civil Status was not created.");
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", controlNo);
                                                    PRBL.CreateTXRecord(dr, dal);
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
                                                PRBL.UpdateTXRecordSave(dr, dal);
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
                                            PRBL.UpdateTXRecord(PopulateDR("1", txtControlNo.Text), dal);
                                            PRBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.TAXCIVIL;
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
                                SystemMenuLogBL.InsertAddLog("WFTAXCODE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
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
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("PAYROLL"));
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "PERSONNEL", "WFTAXCODE");
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
        #region Tax Code
        string sqlTaxCode = @"  SELECT Emt_TaxCode [Code]
                                     , Adt_AccountDesc [Desc]
                                  FROM T_EmployeeMaster
                                  LEFT JOIN T_AccountDetail
                                    ON Adt_AccountCode = Emt_Taxcode
                                   AND Adt_AccountType = 'TAXCODE'
                                 WHERE Emt_EmployeeId = '{0}'";
        DataSet dsTax = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsTax = dal.ExecuteDataSet(string.Format(sqlTaxCode, txtEmployeeId.Text), CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }

        }
        if (!CommonMethods.isEmpty(dsTax))
        {
            txtFromTaxCodeCode.Text = dsTax.Tables[0].Rows[0]["Code"].ToString();
            txtFromTaxCodeDesc.Text = dsTax.Tables[0].Rows[0]["Desc"].ToString();
        }
        else
        {
            txtFromTaxCodeCode.Text = string.Empty;
            txtFromTaxCodeDesc.Text = "     -  INVALID TAX CODE. Code is not in master.  -";
        }
        #endregion
        #region Civil Status
        string sqlCivil = @" SELECT Emt_CivilStatus [Code]
                             , Adt_AccountDesc [Desc]
                          FROM T_EmployeeMaster
                          LEFT JOIN T_AccountDetail
                            ON Adt_AccountCode = Emt_CivilStatus
                           AND Adt_AccountType = 'CIVILSTAT'
                         WHERE Emt_EmployeeId = '{0}'";
        DataSet dsCivil = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsCivil = dal.ExecuteDataSet(string.Format(sqlCivil, txtEmployeeId.Text), CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }

        }
        if (!CommonMethods.isEmpty(dsCivil))
        {
            txtFromCivilCode.Text = dsCivil.Tables[0].Rows[0]["Code"].ToString();
            txtFromCivilDesc.Text = dsCivil.Tables[0].Rows[0]["Desc"].ToString();
        }
        else
        {
            txtFromCivilCode.Text = string.Empty;
            txtFromCivilDesc.Text = "     -  INVALID CIVIL STATUS. Code is not in master.  -";
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

                btnCivilStatus.Enabled = true;
                btnTaxCode.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                //dtpEffectivityDate.Enabled = true;
                cal.Disabled = false;

                btnCivilStatus.Enabled = true;
                btnTaxCode.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                btnCivilStatus.Enabled = false;
                btnTaxCode.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                btnCivilStatus.Enabled = false;
                btnTaxCode.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                //dtpEffectivityDate.Enabled = false;
                cal.Disabled = true;

                btnCivilStatus.Enabled = false;
                btnTaxCode.Enabled = false;
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
        if (txtToCivilCode.Text.Trim().Equals(string.Empty) && txtToTaxCodeCode.Text.Trim().Equals(string.Empty))
        {
            err += "\nNo changes will be done. Check entries.";
        }
        //if(txtFromCivilCode.Text.Equals(txtToCivilCode.Text))
        //{
        //    err += "\nFROM - TO Civil Status is invalid.";
        //}
        //if (txtFromTaxCodeCode.Text.Equals(txtToTaxCodeCode.Text))
        //{
        //    err += "\nFROM - TO Tax Code is invalid.";
        //}
        if (txtReason.Text.Trim().Equals(string.Empty))
        {
            err += "\nReason for update is required.";
        }
        string civilStat = txtToCivilCode.Text.Trim() == string.Empty ? txtFromCivilCode.Text.Trim() : txtToCivilCode.Text.Trim();
        string taxCode = txtToTaxCodeCode.Text.Trim() == string.Empty ? txtFromTaxCodeCode.Text.Trim() : txtToTaxCodeCode.Text.Trim();
        if (civilStat.Trim() == "S")
        {
            if (taxCode.ToString().Trim().StartsWith("M"))
                err += "Tax Code should be in the single category.";
        }
        else if (civilStat.Trim() == "M")
        {
            if (taxCode.ToString().Trim().StartsWith("S"))
                err += "Tax Code should be in the married, zero-rated, or tax-exempted category.";
        }

        if (txtFromTaxCodeCode.Text.Trim() != string.Empty && txtToTaxCodeCode.Text.Trim() != string.Empty
            && txtFromTaxCodeCode.Text.Trim() == txtToTaxCodeCode.Text.Trim())
        {
            err += "No changes in tax code.";
        }
        return err;
    }

    private string checkEntry2(DALHelper dal)//Validation from DB parameters/data
    {
        DataSet ds = new DataSet();
        string err = string.Empty;
        string sqlEnroute = string.Format(@"SELECT Pit_ControlNo [Control No]
                                                 , Adt_AccountDesc [Status]
                                              FROM T_PersonnelInfoMovement
                                             INNER JOIN T_AccountDetail
                                                ON Adt_AccountCode = Pit_Status
                                               AND Adt_AccountType = 'WFSTATUS'
                                             WHERE Pit_EmployeeId = '{0}'
                                               AND Pit_MoveType = 'P1'
                                               AND Pit_Status IN ('1','3','5','7') 
                                               AND Pit_ControlNo <> '{1}'", txtEmployeeId.Text, txtControlNo.Text);
        if (!methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
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
            err += CommonMethods.GetErrorMessageForCutOff("PAYROLL");
        }
        return err;
    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_PersonnelInfoMovement");

        if (methods.GetProcessControlFlag("PAYROLL", "CUT-OFF"))
        {
            dr["Pit_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        }
        else
        {
            dr["Pit_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        }
        dr["Pit_ControlNo"] = ControlNum.ToUpper();
        dr["Pit_EmployeeId"] = txtEmployeeId.Text;
        dr["Pit_EffectivityDate"] = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        dr["Pit_AppliedDate"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Pit_MoveType"] = "P1";
        dr["Pit_From"] = txtFromTaxCodeCode.Text;
        dr["Pit_To"] = txtToTaxCodeCode.Text;
        dr["Pit_Reason"] = txtReason.Text.ToUpper();
        dr["Pit_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Pit_CheckedBy"] = Session["userLogged"].ToString();
        dr["Pit_Checked2By"] = Session["userLogged"].ToString();
        dr["Pit_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Pit_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        //Just used extra columns for the transactions
        dr["Pit_Filler1"] = txtFromCivilCode.Text.ToUpper();
        dr["Pit_Filler2"] = txtToCivilCode.Text.ToUpper();

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
                 + txtToTaxCodeCode.Text
                 + txtToCivilCode.Text
                 + txtReason.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtControlNo.Text = string.Empty;
        txtToCivilCode.Text = string.Empty;
        txtToCivilDesc.Text = string.Empty;
        txtToTaxCodeCode.Text = string.Empty;
        txtToTaxCodeDesc.Text = string.Empty;
        txtFromCivilCode.Text = string.Empty;
        txtFromCivilDesc.Text = string.Empty;
        txtFromTaxCodeCode.Text = string.Empty;
        txtFromTaxCodeDesc.Text = string.Empty;
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
        DataRow dr = PRBL.getTaxCivilInfo(Request.QueryString["cn"].Trim());
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

        txtFromCivilCode.Text = dr["From Civil Code"].ToString();
        txtFromCivilDesc.Text = dr["From Civil Desc"].ToString();
        txtToCivilCode.Text = dr["To Civil Code"].ToString();
        txtToCivilDesc.Text = dr["To Civil Desc"].ToString();

        txtFromTaxCodeCode.Text =  dr["From Tax Code"].ToString();
        txtFromTaxCodeDesc.Text =  dr["From Tax Desc"].ToString();
        txtToTaxCodeCode.Text =  dr["To Tax Code"].ToString();
        txtToTaxCodeDesc.Text =  dr["To Tax Desc"].ToString();

        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.MinDate = CommonMethods.getQuincenaDate('C', "START");
        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "TAXMVMNT", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeTaxCodeCivilStatus.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }
    #endregion
}
