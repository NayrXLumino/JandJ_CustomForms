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

public partial class Transactions_TimeModification_pgeStraightWorkIndividual : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    StraightWorkBL SWBL = new StraightWorkBL();
    MenuGrant MGBL = new MenuGrant();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }//WFTIMERECENTRY                                                 WFSTWORKENTRY
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFSTRAIGHTWORK"))
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
                catch {
                    //CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
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
            LoadComplete += new EventHandler(Transactions_TimeModification_pgeStraightWorkIndividual_LoadComplete);
            if (Session["transaction"].ToString().Equals("CHECKLIST"))
            {
                try
                {
                    loadTransactionDetail();
                    MessageBox.Show("Transaction loaded from checklist");
                }
                catch (Exception ex)
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

    void Transactions_TimeModification_pgeStraightWorkIndividual_LoadComplete(object sender, EventArgs e)
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
        txtShiftCode.Attributes.Add("readOnly", "true");
        txtShiftDesc.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        TextBox txtTemp1 = (TextBox)dtpFromDate.Controls[0];
        txtTemp1.Attributes.Add("readOnly", "true");
        TextBox txtTemp2 = (TextBox)dtpToDate.Controls[0];
        txtTemp2.Attributes.Add("readOnly", "true");
        txtUnpaidBreak.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        btnShift.OnClientClick = "return lookupSWShiftCode()";
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        
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
                      || !CommonMethods.isAffectedByCutoff("TIMEKEEP", dtpFromDate.Date.ToString("MM/dd/yyyy"), dtpToDate.Date.ToString("MM/dd/yyyy")))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "STRAIGHTWK", btnX.Text.Trim().ToUpper());
                            EmailNotificationBL EMBL = new EmailNotificationBL();
                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.STRAIGHTWORK;
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
                                            SWBL.UpdateSWRecord(btnX.Text.Trim().ToUpper()
                                                                , drDetails
                                                                , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO CHECKER 2":
                                            SWBL.UpdateSWRecord(drDetails, dal);
                                            SWBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO APPROVER":
                                            SWBL.UpdateSWRecord(drDetails, dal);
                                            SWBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "APPROVE":
                                            if (!CommonMethods.isAffectedByCutoff("TIMEKEEP", dtpFromDate.Date.ToString("MM/dd/yyyy"), dtpToDate.Date.ToString("MM/dd/yyyy")))
                                            {
                                                SWBL.UpdateSWRecord(drDetails, dal);
                                                SWBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                                SWBL.EmployeeLogLedgerAndOvertimeUpdate(txtControlNo.Text
                                                                            , Session["userLogged"].ToString()
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
                if ( Convert.ToBoolean(Resources.Resource.ALLOWFLOW) 
                  || !CommonMethods.isAffectedByCutoff("TIMEKEEP", dtpFromDate.Date.ToString("MM/dd/yyyy"), dtpToDate.Date.ToString("MM/dd/yyyy")))
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
                                    SWBL.UpdateSWRecord(PopulateDR("2", txtControlNo.Text), dal);
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
                                        SWBL.UpdateSWRecord(PopulateDR(status, txtControlNo.Text), dal);
                                        SWBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.STRAIGHTWORK;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
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
                if ( Convert.ToBoolean(Resources.Resource.ALLOWFLOW) 
                  || !CommonMethods.isAffectedByCutoff("TIMEKEEP", dtpFromDate.Date.ToString("MM/dd/yyyy"), dtpToDate.Date.ToString("MM/dd/yyyy")))
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
                                                string controlNo = CommonMethods.GetControlNumber("STRAIGHTWK");
                                                if (controlNo.Equals(string.Empty))
                                                {
                                                    MessageBox.Show("Transaction control for Straight Work was not created.");
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", controlNo);
                                                    SWBL.CreateSWRecord(dr, dal);
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
                                                SWBL.UpdateSWRecordSave(dr, dal);
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
                                            SWBL.UpdateSWRecord(PopulateDR("1", txtControlNo.Text), dal);
                                            SWBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.STRAIGHTWORK;
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
                                            MessageBox.Show("Enter remarks for of action.");
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFSTRAIGHTWORK");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        //Default Parameter
        //dtpFromDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        dtpFromDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpFromDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        //Default Parameter
        //dtpToDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        dtpToDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpToDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        enableControls();
        enableButtons();
        showOptionalFields();
    }

    private void enableControls()
    {
        switch (txtStatus.Text.ToUpper())
        {
            case "":
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
                txtUnpaidBreak.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                btnShift.Enabled = true;
                btnFiller1.Enabled = true;
                btnFiller2.Enabled = true;
                btnFiller3.Enabled = true;

                txtUnpaidBreak.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
                txtUnpaidBreak.ReadOnly = false;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                btnShift.Enabled = true;
                btnFiller1.Enabled = true;
                btnFiller2.Enabled = true;
                btnFiller3.Enabled = true;

                txtUnpaidBreak.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
                txtUnpaidBreak.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                btnShift.Enabled = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtUnpaidBreak.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
                txtUnpaidBreak.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                btnShift.Enabled = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtUnpaidBreak.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
                txtUnpaidBreak.ReadOnly = true;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                btnShift.Enabled = false;
                btnFiller1.Enabled = false;
                btnFiller2.Enabled = false;
                btnFiller3.Enabled = false;

                txtUnpaidBreak.BackColor = System.Drawing.Color.Gainsboro;
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
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Swt_Filler%'
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
                        case "SWT_FILLER01":
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
                        case "SWT_FILLER02":
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
                        case "SWT_FILLER03":
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

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        #region Start Date and End Date
        if (dtpFromDate.Date.Equals(dtpToDate.Date))
        {
            err += "\nFROM - TO date cannot be the same.";
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
        string err = string.Empty;
        #region SQL
        #region Application Conflicts
        //added '9' for conflicts..Robert
        string sqlConfilcts = @"DECLARE @START varchar(10)
                                DECLARE @END varchar(10)

                                SET @START = @startDate
                                SET @END = @endDate

                                SELECT Swt_ControlNo [Control No] 
                                     , Convert(varchar(10), Swt_FromDate, 101) [From Date]
	                                 , Convert(varchar(10), Swt_ToDate, 101) [To Date]
	                                 , '[' + Swt_ShiftCode + ']'
	                                 + ' ' + LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2)
	                                 + ' - ' + LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2)
	                                 + ' ' + LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2)
	                                 + ' - ' + LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2) [Shift]
	                                 , Swt_UnpaidBreak [Unpaid Break]
	                                 , Adt_AccountDesc [Status] 
                                  FROM T_EmployeeStraightWork
                                  LEFT JOIN T_ShiftCodeMaster
	                                ON Scm_ShiftCode = Swt_ShiftCode
                                  LEFT JOIN T_AccountDetail
                                    ON Adt_AccountCode = Swt_Status
                                   AND Adt_AccountType = 'WFSTATUS'
                                 WHERE ( @START BETWEEN Swt_FromDate AND Swt_ToDate
                                      OR @END BETWEEN Swt_FromDate AND Swt_ToDate 
                                      OR (@START <= Swt_FromDate AND @END >=Swt_ToDate ))
                                   AND Swt_EmployeeId = @EmployeeId
                                   AND Swt_Status IN ('1','3','5','7','9') 
                                   AND Swt_ControlNo <> @CurrentControlNo";
        #endregion
        ParameterInfo[] param = new ParameterInfo[4];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@CurrentControlNo", txtControlNo.Text);
        param[2] = new ParameterInfo("@startDate", dtpFromDate.Date.ToString("MM/dd/yyyy"));
        param[3] = new ParameterInfo("@endDate", dtpToDate.Date.ToString("MM/dd/yyyy"));

        #endregion
        ds = dal.ExecuteDataSet(sqlConfilcts, CommandType.Text, param);
        if (!CommonMethods.isEmpty(ds))
        {
            err += "Transaction conflict:";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                err += @"\n    "
                     + ds.Tables[0].Rows[i]["Control No"].ToString()
                     + "  ("
                     + ds.Tables[0].Rows[i]["From Date"].ToString()
                     + " - "
                     + ds.Tables[0].Rows[i]["To Date"].ToString().Insert(2, ":")
                     + ")  "
                     + ds.Tables[0].Rows[i]["Shift"].ToString().Insert(2, ":");
            }
        }
        return err;
    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_EmployeeStraightWork");

        if (methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
        {
            dr["Swt_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        }
        else
        {
            dr["Swt_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        }

        dr["Swt_ControlNo"] = ControlNum;
        dr["Swt_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Swt_FromDate"] = dtpFromDate.Date.ToString("MM/dd/yyyy");
        dr["Swt_ToDate"] = dtpToDate.Date.ToString("MM/dd/yyyy");
        dr["Swt_ShiftCode"] = txtShiftCode.Text;
        dr["Swt_UnpaidBreak"] = txtUnpaidBreak.Text;
        dr["Swt_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["Swt_CheckedBy"] = Session["userLogged"].ToString();
        dr["Swt_Checked2By"] = Session["userLogged"].ToString();
        dr["Swt_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Swt_Status"] = Status.ToUpper();
        dr["Swt_PayPeriodFlag"] = SWBL.ComputeStraightWorkFlag(dtpFromDate.Date.ToString("MM/dd/yyyy")).ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Swt_Filler1"] = txtFiller1.Text.ToUpper();
        dr["Swt_Filler2"] = txtFiller2.Text.ToUpper();
        dr["Swt_Filler3"] = txtFiller3.Text.ToUpper();

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
        snapShot = dtpFromDate.Date.ToString()
                 + dtpToDate.Date.ToString()
                 + txtShiftCode.Text
                 + txtUnpaidBreak.Text
                 + txtReason.Text
                 + txtFiller1.Text
                 + txtFiller2.Text
                 + txtFiller3.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtControlNo.Text = string.Empty;
        (dtpFromDate.Controls[0] as TextBox).Text = string.Empty;
        (dtpToDate.Controls[0] as TextBox).Text = string.Empty;
        txtShiftCode.Text = string.Empty;
        txtShiftDesc.Text = string.Empty;
        txtUnpaidBreak.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
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
        DataRow dr = SWBL.getStraightWorkInfo(Request.QueryString["cn"].Trim());
        txtEmployeeId.Text = dr["ID No"].ToString();
        txtEmployeeName.Text = dr["Lastname"].ToString() + ", " + dr["Firstname"].ToString() + " " + dr["Firstname"].ToString().Substring(0, 1) + ".";
        if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
        {
            txtNickname.Text = dr["Nickname"].ToString();
        }
        else
        {
            txtNickname.Text = string.Empty;
        }
        txtControlNo.Text = dr["Control No"].ToString();
        dtpFromDate.Date = Convert.ToDateTime(dr["From Date"].ToString());
        dtpToDate.Date = Convert.ToDateTime(dr["To Date"].ToString());
        txtShiftCode.Text = dr["Shift Code"].ToString();
        txtShiftDesc.Text = dr["Shift"].ToString();
        txtUnpaidBreak.Text = dr["Unpaid Break"].ToString();
        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        txtFiller1.Text = dr[CommonMethods.getFillerName("Swt_Filler01")].ToString();
        txtFiller2.Text = dr[CommonMethods.getFillerName("Swt_Filler02")].ToString();
        txtFiller3.Text = dr[CommonMethods.getFillerName("Swt_Filler03")].ToString();

        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;

        //Default Parameter
        //dtpFromDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        dtpFromDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpFromDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        //Default Parameter
        //dtpToDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        dtpToDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpToDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        enableControls();
        enableButtons();
        showOptionalFields();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }
    #endregion
}
