/*File revision no. W2.1.00001 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_WorkInformation_pgeCostCenterIndividualUpdate : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    WorkInformationBL WIBL = new WorkInformationBL();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFCCUPDATE"))  
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
            LoadComplete += new EventHandler(Transactions_WorkInformation_pgeCostCenterIndividualUpdate_LoadComplete);
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

    void Transactions_WorkInformation_pgeCostCenterIndividualUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtFromCostCenterCode.Attributes.Add("readOnly", "true");
        txtFromCostCenterDesc.Attributes.Add("readOnly", "true");
        txtToCostCenterCode.Attributes.Add("readOnly", "true");
        txtToCostCenterDesc.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpEffectivityDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        btnToCostCenter.Attributes.Add("OnClick", "javascript:return lookupCostCenter(" + "'TO', 'txtToCostCenter');");
        txtRemarks.Attributes.Add("OnFocus", "javascript:getElementById('ctl00_ContentPlaceHolder1_txtRemarks').select();");
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
                            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "MOVEMENT", btnX.Text.Trim().ToUpper());
                            EmailNotificationBL EMBL = new EmailNotificationBL();
                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
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
                                            WIBL.UpdateMVRecord(btnX.Text.Trim().ToUpper()
                                                               , drDetails
                                                               , dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO CHECKER 2":
                                            WIBL.UpdateMVRecord(drDetails, dal);
                                            WIBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "ENDORSE TO APPROVER":
                                            WIBL.UpdateMVRecord(drDetails, dal);
                                            WIBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            MessageBox.Show("Successfully endorsed transaction.");
                                            break;
                                        case "APPROVE":
                                            WIBL.UpdateMVRecord(drDetails, dal);
                                            WIBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            WIBL.EmployeeLogLedgerUpdateCostCenter( txtEmployeeId.Text
                                                                                  , txtToCostCenterCode.Text
                                                                                  , dtpEffectivityDate.Date.ToString("MM/dd/yyyy")
                                                                                  , Session["userLogged"].ToString()
                                                                                  , dal
                                                                                  , txtControlNo.Text);
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
                                    SystemMenuLogBL.InsertEditLog("WFCCUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "",false);
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
                                //        CommonMethods.sendNotification("TAXMVMNT"   //temp
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
                                    WIBL.UpdateMVRecord(PopulateDR("2", txtControlNo.Text), dal);
                                    //MenuLog
                                    SystemMenuLogBL.InsertDeleteLog("WFCCUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                                        WIBL.UpdateMVRecord(PopulateDR(status, txtControlNo.Text), dal);
                                        WIBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                            EMBL.InsertInfoForNotification(txtControlNo.Text
                                                                          , Session["userLogged"].ToString()
                                                                          , dal);
                                        }
                                        //MenuLog
                                        SystemMenuLogBL.InsertEditLog("WFCCUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
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
                            
                            restoreDefaultControls();
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
                                                string controlNo = CommonMethods.GetControlNumber("MOVEMENT");
                                                if (controlNo.Equals(string.Empty))
                                                {
                                                    MessageBox.Show("Transaction control for Work Information Movement was not created.");
                                                }
                                                else
                                                {
                                                    DataRow dr = PopulateDR("1", controlNo);
                                                    WIBL.CreateMVRecord(dr, dal);
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
                                                WIBL.UpdateMVRecordSave(dr, dal);
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
                                            WIBL.UpdateMVRecord(PopulateDR("1", txtControlNo.Text), dal);
                                            WIBL.InsertUpdateRemarks(PopultateDRForRemarks(txtControlNo.Text), dal);
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
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
                                SystemMenuLogBL.InsertAddLog("WFCCUPDATE", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "",false);



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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFCCUPDATE"); 
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpEffectivityDate.Date = DateTime.Now;

        string[] MinMaxDate = new string[2];
        MinMaxDate = CommonMethods.GetMinMaxDateOfFiling();
        try
        {
            dtpEffectivityDate.MinDate = Convert.ToDateTime(MinMaxDate[0]);
            dtpEffectivityDate.MaxDate = Convert.ToDateTime(MinMaxDate[1]);
        }
        catch
        {
            MessageBox.Show("Failed to initialize minimum and maximum date of date pickers. Press OK to continue.");
        }

        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        initializeCurrentValues();
        enableControls();
        enableButtons();
    }

    private void initializeCurrentValues()
    {
        #region SQL
        string sql = @"SELECT Emt_CostCenterCode [Code]
                             , dbo.GetCostCenterFullNameV2(Emt_CostCenterCode) [Desc]
                          FROM T_EmployeeMaster
                         WHERE Emt_EmployeeId = '{0}'";
        DataSet dsCostCenter = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsCostCenter = dal.ExecuteDataSet(string.Format(sql, txtEmployeeId.Text), CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }

        }
        if (!CommonMethods.isEmpty(dsCostCenter))
        {
            txtFromCostCenterCode.Text = dsCostCenter.Tables[0].Rows[0]["Code"].ToString();
            txtFromCostCenterDesc.Text = dsCostCenter.Tables[0].Rows[0]["Desc"].ToString();
        }
        else
        {
            txtFromCostCenterCode.Text = string.Empty;
            txtFromCostCenterDesc.Text = "     -  INVALID COSTCENTER CODE. Code is not in master.  -";
        }
        #endregion
    }

    private void enableControls()
    {
        switch (txtStatus.Text.ToUpper())
        {
            case "":
                dtpEffectivityDate.Enabled = true;
                btnToCostCenter.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "NEW":
                dtpEffectivityDate.Enabled = true;
                btnToCostCenter.Enabled = true;
                txtReason.ReadOnly = false;
                txtRemarks.ReadOnly = true;

                txtReason.BackColor = System.Drawing.Color.White;
                txtRemarks.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 1":
                dtpEffectivityDate.Enabled = true;
                btnToCostCenter.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 2":
                dtpEffectivityDate.Enabled = true;
                btnToCostCenter.Enabled = false;
                txtReason.ReadOnly = true;
                txtRemarks.ReadOnly = false;

                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                txtRemarks.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO APPROVER":
                dtpEffectivityDate.Enabled = true;
                btnToCostCenter.Enabled = false;
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
        if (txtToCostCenterCode.Text.Equals(string.Empty))
        {
            err += "\nNo changes will be done. Check entries.";
        }
        if (txtFromCostCenterCode.Text.Equals(txtToCostCenterCode.Text))
        {
            err += "\nFROM - TO Costcenter is invalid.";
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
        string sqlEnroute = string.Format(@"SELECT Mve_ControlNo [Control No]
	                                            , Adt_AccountDesc [Status]
	                                            FROM T_Movement
	                                            JOIN T_AccountDetail ON Adt_AccountCode = Mve_Status
		                                            AND Adt_AccountType = 'WFSTATUS'
	                                            WHERE Mve_EmployeeId = '{0}'
		                                            AND Mve_Type = 'C'
		                                            AND Mve_Status IN ('1','3','5','7')
		                                            AND Mve_ControlNo <> '{1}'
                                                    AND Mve_EffectivityDate = '{2}'"
            , txtEmployeeId.Text, txtControlNo.Text, dtpEffectivityDate.Date.ToString("MM/dd/yyyy"));
        
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
        DataRow dr = DbRecord.Generate("T_Movement");
        //Andre: removed condition. ALWAYS retreive current. 20130702
        //if (methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
        //{
        //    dr["Mve_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        //}
        //else
        //{
            dr["Mve_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        //}
        dr["Mve_ControlNo"] = ControlNum.ToUpper();
        dr["Mve_EmployeeId"] = txtEmployeeId.Text;
        dr["Mve_EffectivityDate"] = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        dr["Mve_AppliedDate"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_Type"] = "C";
        dr["Mve_From"] = txtFromCostCenterCode.Text;
        dr["Mve_To"] = txtToCostCenterCode.Text;
        dr["Mve_Reason"] = txtReason.Text.ToUpper();
        dr["Mve_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_CheckedBy"] = Session["userLogged"].ToString();
        dr["Mve_Checked2By"] = Session["userLogged"].ToString();
        dr["Mve_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Mve_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Mve_Flag"] = WIBL.ComputeMVFlag(dtpEffectivityDate.Date.ToString("MM/dd/yyyy")).ToUpper();

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
                 + txtToCostCenterCode.Text
                 + txtRemarks.Text   
                 + txtReason.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtControlNo.Text = string.Empty;
        txtToCostCenterCode.Text = string.Empty;
        txtToCostCenterDesc.Text = string.Empty;
        txtFromCostCenterCode.Text = string.Empty;
        txtFromCostCenterDesc.Text = string.Empty;
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
        DataRow dr = WIBL.getMovementInfo(Request.QueryString["cn"].Trim());
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

        txtFromCostCenterCode.Text = dr["From Code"].ToString();
        txtFromCostCenterDesc.Text = dr["From Desc"].ToString();
        txtToCostCenterCode.Text = dr["To Code"].ToString();
        txtToCostCenterDesc.Text = dr["To Desc"].ToString();

        txtReason.Text = dr["Reason"].ToString();
        txtRemarks.Text = dr["Remarks"].ToString();
        txtStatus.Text = dr["Status"].ToString();

        //initialize controls
        btnEmployeeId.Enabled = false;
        hfPrevEmployeeId.Value = txtEmployeeId.Text;

        string[] MinMaxDate = new string[2];
        MinMaxDate = CommonMethods.GetMinMaxDateOfFiling();
        try
        {
            dtpEffectivityDate.MinDate = Convert.ToDateTime(MinMaxDate[0]);
            dtpEffectivityDate.MaxDate = Convert.ToDateTime(MinMaxDate[1]);
        }
        catch
        {
            MessageBox.Show("Failed to initialize minimum and maximum date of date pickers. Press OK to continue.");
        }

        enableControls();
        //ifchecker/approver
        if (CommonMethods.isCorrectRoutePage(Session["userLogged"].ToString(), txtEmployeeId.Text, "MOVEMENT", txtStatus.Text))
            enableButtons();
        else
        {
            string pageUrl = @"pgeCostCenterIndividualUpdate.aspx";
            Response.Redirect(pageUrl);
        }
        //else create enablebutton
        //enableButtons();
        txtRemarks.Focus();
        hfPrevEntry.Value = changeSnapShot();
    }

    private string[] GetMinMaxLegder()
    {
        string sql = @" SELECT Convert(varchar(10), MIN(Ell_ProcessDate), 101) [MinDate]
                             , Convert(varchar(10), MAX(Ell_ProcessDate), 101) [MaxDate]
                          FROM T_EMployeeLogLedger";
        DataSet ds = new DataSet();
        string[] value = new string[2] { string.Empty, string.Empty };
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            value[0] = ds.Tables[0].Rows[0]["MinDate"].ToString();
            value[1] = ds.Tables[0].Rows[0]["MaxDate"].ToString();
        }

        return value;
    }
    #endregion
}
