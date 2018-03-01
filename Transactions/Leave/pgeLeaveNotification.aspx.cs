/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/14/2013
 *  Update Notes    :   
 *      -   Updated Leave Credit Checking for Notice
 *      -   Cancellation for Notice
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

/// <summary>
/// Created by: Andre Antonio Sungahid
/// Important notes:
///     Leave Dropdown Code/Value
///         Leave Code       2 char
///         With Category    1 char
///         With Credit      1 char
///         DayUnit         21 char including commas(,)
///                 Total   25 characters
/// </summary>
public partial class Transactions_Leave_pgeLeaveNotification : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    LeaveBL LVBL = new LeaveBL();
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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLVENOTEENTRY"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    initializeEmployee();
                    initializeControls();
                    initializeLeaveParameters();
                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                    Page.PreRender += new EventHandler(Page_PreRender);
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
                    loadTransactionDetail();
                    hfSaved.Value = "1";
                    hfFromNotice.Value = "FALSE";
                    txtControlNo.Text = hfControlNo.Value;
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveNotification_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Leave_pgeLeaveNotification_LoadComplete(object sender, EventArgs e)
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

        btnControlNo.Attributes.Add("OnClick", "javascript:return lookupLVLeaveNotice();");
        txtLVStartTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVStartTime')");
        txtLVStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtLVEndTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVEndTime')");
        txtLVEndTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        ddlType.Attributes.Add("OnChange", "javascript:ddlType_ClientChanged();");
        rblDayUnit.Attributes.Add("OnClick", "javascript:rblDayUnit_ClientChanged();");
        btnInformantRelation.Attributes.Add("OnClick", "javascript:return lookupGenericFiller('txtRelation','NOTICEREL');");
        btnInformMode.Attributes.Add("OnClick", "javascript:return lookupGenericFiller('txtModeOfInformation','MODENOTICE');");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        initializeShift();
        initializeLeaveParameters();
    }

    protected void dtpLVDate_Change(object sender, EventArgs e)
    {
        initializeShift();
        initializeLeaveParameters();
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
            if (this.ddlType.SelectedValue.ToString().Trim().Substring(2, 1) == "1")
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
        if (!ddlType.SelectedValue.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL)
                && !Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
            {
                if (ddlType.SelectedValue.Substring(0, 2).Equals("SL"))
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
        return;
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            Response.Redirect(Request.RawUrl);
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
                            if (!txtControlNo.Text.Equals(string.Empty))
                            {
                                if (LVBL.isCreditsDeductableFromNotice("CANCEL", this.txtControlNo.Text.Trim(), dal))
                                {
                                    LVBL.UpdateLVNoticeRecordSave(PopulateDR("2", txtControlNo.Text), dal, true);
                                    //UpdateCredits("CANCEL", dal);
                                    //MenuLog
                                    SystemMenuLogBL.InsertDeleteLog("WFLVENOTEENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                    restoreDefaultControls();
                                    MessageBox.Show("Transaction cancelled");
                                }
                                else
                                {
                                    MessageBox.Show("Cannot Cancel Leave, Credits will be negative");
                                }
                            }
                            else
                            {
                                MessageBox.Show("No transaction to cancel");
                            }
                            dal.CommitTransactionSnapshot();
                            refreshLeaveLedger();
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
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                }
                if (isLeaveError.Trim() != string.Empty)
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
                    string errMsg1 = checkEntry1();
                    if (errMsg1.Equals(string.Empty))
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                dal.BeginTransactionSnapshot();
                                string errMsg2 = checkEntry2(dal);
                                if (errMsg2.Equals(string.Empty))
                                {
                                    if (hfSaved.Value.Equals("0"))
                                    {
                                        string controlNo = CommonMethods.GetControlNumber("LVENOTICE");
                                        if (controlNo.Equals(string.Empty))
                                        {
                                            MessageBox.Show("Transaction control for Leave Notice was not created.");
                                        }
                                        else
                                        {
                                            DataRow dr = PopulateDR("1", controlNo);
                                            LVBL.CreateLVNoticeRecord(dr, dal);
                                            txtControlNo.Text = controlNo;
                                            //UpdateCredits("NEW", dal);
                                            txtStatus.Text = "NEW";
                                            hfSaved.Value = "1";
                                            //MenuLog
                                            SystemMenuLogBL.InsertAddLog("WFLVENOTEENTRY", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "",false);
                                            MessageBox.Show("New transaction saved.\nEmployee can now retrieve transaction.");
                                        }
                                    }
                                    else
                                    {
                                        DataRow dr = PopulateDR("1", txtControlNo.Text);
                                        LVBL.UpdateLVNoticeRecordSave(dr, dal, false);
                                        //UpdateCredits("UPDATE", dal);
                                        hfSaved.Value = "1";
                                        MessageBox.Show("Transaction updated.\nEmployee can now retrieve transaction.");
                                    }
                                    capturePrevValues();
                                }
                                else
                                {
                                    MessageBox.Show(errMsg2);
                                }
                                dal.CommitTransactionSnapshot();
                                //Andre added after commiting of changes in DB
                                refreshLeaveLedger();
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
                    }
                    else
                    {
                        MessageBox.Show(errMsg1);
                    }
                    if (isLeaveError.Trim() != string.Empty)
                    {
                        MessageBox.Show(isLeaveError);
                    }
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
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
        param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString(), SqlDbType.VarChar, 15);
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "LEAVE", "WFLVENOTEENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        dtpLVDate.Date = DateTime.Now;
        dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        hfLVHRENTRY.Value = methods.GetProcessControlFlag("LEAVE", "LVHRENTRY").ToString();
        hfLHRSINDAY.Value = CommonMethods.getParamterValue("LHRSINDAY").ToString();
        lblUnit.Text = (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS)) ? "UNIT: IN HOURS" : "UNIT: IN DAYS";
        dtpInformDateTime.Date = DateTime.Now;
        initializeShift();
        enableControls();
        showOptionalFields();

        hfCHIYODA.Value = Resources.Resource.CHIYODASPECIFIC.ToString().ToUpper();
    }

    private void initializeShift()
    {
        DataSet ds = new DataSet();
        try
        {
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
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
        catch(Exception er)
        {
            CommonMethods.ErrorsToTextFile(er, "InitializeShift(LeaveNotice)");
        }
    }

    private void enableControls()
    {
        switch (txtStatus.Text.ToUpper())
        {
            case "":
                dtpLVDate.Enabled = true;
                ddlType.Enabled = true;
                ddlCategory.Enabled = true;
                txtLVStartTime.ReadOnly = false;
                txtLVEndTime.ReadOnly = false;
                rblDayUnit.Enabled = true;
                txtReason.ReadOnly = false;

                txtLVStartTime.BackColor = System.Drawing.Color.White;
                txtLVEndTime.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                break;
            case "NEW":
                dtpLVDate.Enabled = true;
                ddlType.Enabled = true;
                ddlCategory.Enabled = true;
                txtLVStartTime.ReadOnly = false;
                txtLVEndTime.ReadOnly = false;
                rblDayUnit.Enabled = true;
                txtReason.ReadOnly = false;

                txtLVStartTime.BackColor = System.Drawing.Color.White;
                txtLVEndTime.BackColor = System.Drawing.Color.White;
                txtReason.BackColor = System.Drawing.Color.White;
                break;
            case "ENDORSED TO CHECKER 1":
                dtpLVDate.Enabled = false;
                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO CHECKER 2":
                dtpLVDate.Enabled = false;
                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
                break;
            case "ENDORSED TO APPROVER":
                dtpLVDate.Enabled = false;
                ddlType.Enabled = false;
                ddlCategory.Enabled = false;
                txtLVStartTime.ReadOnly = true;
                txtLVEndTime.ReadOnly = true;
                rblDayUnit.Enabled = false;
                txtReason.ReadOnly = true;

                txtLVStartTime.BackColor = System.Drawing.Color.Gainsboro;
                txtLVEndTime.BackColor = System.Drawing.Color.Gainsboro;
                txtReason.BackColor = System.Drawing.Color.Gainsboro;
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
                        case "ELT_FILLER02":
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
            ddlType_SelectedIndexChanged(null, null);
            //..End..End..Added additional code for viewing only on SL.
        }
    }

    private void initializeLeaveParameters()
    {
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
                            ,  Elm_LeaveYear [Year]
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
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty), SqlDbType.VarChar, 4);
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
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    ddlType.Items.Add(new ListItem(ds.Tables[1].Rows[i]["Description"].ToString()
                                                  , ds.Tables[1].Rows[i]["Code"].ToString()));
                }
            }
            if (false)
            {
                //Initialize category dropdown
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
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          AND Elm_LeaveYear = @Year";

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
                         FROM T_EmployeeLeave
                        INNER JOIN T_LeaveTypeMaster
                           ON Ltm_LeaveType = Elm_LeaveType
                        WHERE Elm_EmployeeId = @EmployeeId
                          AND Elm_LeaveYear = @Year ";
        }

        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
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
        #region
        if (ddlType.SelectedValue.Equals(string.Empty))
        {
            err += "\nSelect a leave type.";
        }
        #endregion
        #region Check day code of filing
        if (err.Equals(string.Empty))
        {
            if (!(txtDayCode.Text.Contains("REG") || txtDayCode.Text.Contains("SPL") || txtDayCode.Text.Contains("CMPY")))
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
            }
            if (LVEnd < LVStart)
                LVEnd += 1440;

            if (!hfShiftType.Equals("G") && !ddlType.SelectedValue.Substring(0, 2).Equals("OB"))//for THI OB can be outside of shift...
            {
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
            }
        }
        #endregion
        #region Reason
        if (txtReason.Text.Length > 200)
        {
            err += "\nReason exceeds maximum characters allowed.(" + txtReason.Text.Length.ToString() + "/200)";
        }
        #endregion
        return err;
    }

    private string checkEntry2(DALHelper dal)
    {
        string err = string.Empty;
        DataSet ds = new DataSet();
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
        string sqlCombinedLeave = @"declare @1stLeave as char(2)
                                    declare @2ndLeave as char(2)
                                    declare @isExist as varchar(7)
                                    SET @1stLeave = ( SELECT Elt_LeaveType
				                                        FROM T_EmployeeLeaveAvailment
				                                        WHERE Elt_Status IN ('1','3','5','7','9', 'A')
				                                         AND Elt_EmployeeId = @EmployeeId
				                                         AND Elt_LeaveDate = @leaveDate
				                                         AND Elt_ControlNo <> @ControlNo
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
                                                      SELECT Elt_LeaveType
				                                        FROM T_EmployeeLeaveAvailmentHist
				                                       WHERE Elt_Status IN ('1','3','5','7','9', 'A')
				                                         AND Elt_EmployeeId = @EmployeeId
				                                         AND Elt_LeaveDate = @leaveDate 
				                                         AND Elt_ControlNo <> @ControlNo
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
                                                      SELECT Eln_LeaveType
				                                        FROM T_EmployeeLeaveNotice
				                                       WHERE Eln_Status IN ('1')
				                                         AND Eln_EmployeeId = @EmployeeId
				                                         AND Eln_LeaveDate = @leaveDate
                                                         AND Eln_ControlNo <> @NoticeControlNo )
                                    SET @2ndLeave = @LeaveType

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
	                                        END ";
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
                decimal temp = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
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
                                    if (temp < (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"]) / 2)
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("OB")
                                      && !ddlType.SelectedValue.Substring(0, 2).Equals("UN"))
                                    {
                                        err += "\nMinimum leave hour(s) is " + (Convert.ToDecimal(ds.Tables[0].Rows[i]["Pmt_NumericValue"].ToString()) / 2);
                                    }
                                }
                                else if (this.rblDayUnit.SelectedValue != null)
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
            if (err.Equals(string.Empty) && ddlType.SelectedValue.Substring(3, 1).Equals("1") && !hfFromNotice.Value.Equals("TRUE"))
            {
                #region Checking of leave credits if suffice for leave with credits
                #region Get Records From Previous
                ds = null;
                if (this.txtControlNo.Text.Trim() != string.Empty)
                {
                    ds = dal.ExecuteDataSet(@"
SELECT
	Eln_EmployeeID [Elt_EmployeeID]
    , CONVERT(VARCHAR(20), Eln_LeaveDate, 101) [Elt_LeaveDate]
	, Eln_LeaveType [Elt_LeaveType]
	, Eln_StartTime [Elt_StartTime]
	, Eln_EndTime [Elt_EndTime]
    , Eln_DayUnit [Elt_DayUnit]
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Eln_LeaveHour
					ELSE 
						CASE WHEN Eln_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
						END
				END
			ELSE
			Eln_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveNotice
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Eln_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Eln_LeaveType = Ltm_LeaveType
WHERE Eln_ControlNo = @ControlNo
	AND Ltm_WithCredit = 1
                    ", CommandType.Text, param);
                }
                bool isChanged = false;
                bool isNewState = true;
                #endregion
                if (!CommonMethods.isEmpty(ds))
                {
                    if (Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()).ToString("MM/dd/yyyy") != dtpLVDate.Date.ToString("MM/dd/yyyy")
                        || ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim() != this.ddlType.SelectedValue.ToString().Substring(0, 2)
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
        err = LVBL.IsCreditsSameBasedOnTransactionsMessage(
                        transactType
                        , txtEmployeeId.Text
                        , leaveYear
                        , ddlType.SelectedValue.ToString().Substring(0, 2)
                        , dal);
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
	CONVERT(VARCHAR(20), Eln_LeaveDate, 101) [Elt_LeaveDate]
	, Eln_LeaveType [Elt_LeaveType]
	, Eln_StartTime [Elt_StartTime]
	, Eln_EndTime [Elt_EndTime]
    , Eln_DayUnit [Elt_DayUnit]
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Eln_LeaveHour
					ELSE 
						CASE WHEN Eln_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
						END
				END
			ELSE
			Eln_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveNotice
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Eln_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Eln_LeaveType = Ltm_LeaveType
WHERE Eln_ControlNo = '{0}'
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

        hfPrevDateCredits.Value = dtpLVDate.Date.ToString("MM/dd/yyyy");
        hfPrevType.Value = ddlType.SelectedValue.ToString().Substring(0, 2);
        hfPrevStart.Value = txtLVStartTime.Text.Replace(":", "");
        hfPrevEnd.Value = txtLVEndTime.Text.Replace(":", "");
        hfPrevDayUnit.Value = rblDayUnit.SelectedValue;
        hfPrevShiftCode.Value = hfShiftCode.Value;
        
    }

    private void capturePrevValues()
    {
        hfPrevDateCredits.Value = dtpLVDate.Date.ToString("MM/dd/yyyy");
        hfPrevType.Value = ddlType.SelectedValue;
        hfPrevStart.Value = txtLVStartTime.Text;
        hfPrevShiftCode.Value = hfShiftCode.Value;
        hfPrevEnd.Value = txtLVEndTime.Text;
        hfPrevDayUnit.Value = rblDayUnit.SelectedValue;
        hfPrevEntry.Value = changeSnapShot();
        hfControlNo.Value = txtControlNo.Text;
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
        txtControlNo.Text = string.Empty;
        txtLVStartTime.Text = string.Empty;
        txtLVEndTime.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;
        txtStatus.Text = string.Empty;
        hfPrevLVDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeEmployee();
        initializeControls();
        initializeLeaveParameters();
    }

    private DataRow PopulateDR(string Status, string ControlNum)
    {
        string filler1 = string.Empty;
        string filler2 = string.Empty;
        string filler3 = string.Empty;
        string infoMode = string.Empty;
        string relation = string.Empty;

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
        if (txtModeOfInformation.Text != string.Empty)
        {
            DataSet dsModeofInfo = getFillerCode(txtModeOfInformation.Text, "MODENOTICE");
            if (dsModeofInfo.Tables[0].Rows.Count > 0)
                infoMode = dsModeofInfo.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        if (txtRelation.Text != string.Empty)
        {
            DataSet dsRelation = getFillerCode(txtRelation.Text, "NOTICEREL");
            if (dsRelation.Tables[0].Rows.Count > 0)
                relation = dsRelation.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
        }
        DataRow dr = DbRecord.Generate("T_EmployeeLeaveNotice");

        dr["Eln_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Eln_LeaveDate"] = dtpLVDate.Date.ToString("MM/dd/yyyy");
        dr["Eln_LeaveType"] = ddlType.SelectedValue.ToUpper().Substring(0, 2);
        if (ddlCategory.SelectedValue.ToUpper().IndexOf("NOT APP") != -1)
        {
            dr["Eln_LeaveCategory"] = "";
        }
        else
        {
            dr["Eln_LeaveCategory"] = ddlCategory.SelectedValue.ToUpper();
        }
        dr["Eln_StartTime"] = txtLVStartTime.Text.Replace(":", "");
        dr["Eln_EndTime"] = txtLVEndTime.Text.Replace(":", "");
        dr["Eln_LeaveHour"] = LVBL.calculateLeaveHoursHours(txtLVStartTime.Text.Replace(":", "")
                                                           , txtLVEndTime.Text.Replace(":", "")
                                                           , rblDayUnit.SelectedValue.ToUpper()
                                                           , hfShiftCode.Value
                                                           , ddlType.SelectedValue.Substring(0, 2)
                                                           , false);
        dr["Eln_DayUnit"] = rblDayUnit.SelectedValue.ToUpper();
        dr["Eln_Reason"] = txtReason.Text.ToUpper();
        dr["Eln_InformDate"] = dtpInformDateTime.Date;
        dr["Eln_ControlNo"] = ControlNum;
        dr["Eln_Status"] = Status;
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Elt_Filler1"] = ddlType.SelectedValue.Substring(0, 2).Equals("SL") ? filler1.ToUpper() : string.Empty;
        dr["Elt_Filler2"] = filler2.ToUpper();
        dr["Elt_Filler3"] = filler3.ToUpper();
        dr["Eln_InformMode"] = infoMode.ToUpper();
        dr["Eln_Informant"] = txtInformant.Text.ToUpper();
        dr["Eln_InformantRelation"] = relation.ToUpper();

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
        string infoMode = string.Empty;
        string relation = string.Empty;
        DataRow dr = LVBL.getLeaveInfoFromNotice(txtControlNo.Text);
        txtEmployeeId.Text = dr["ID No"].ToString();
        txtEmployeeName.Text = dr["Lastname"].ToString() + ", " + dr["Firstname"].ToString() + " " + dr["Middlename"].ToString().Substring(0, 1) + ".";
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

        DataSet dsModeofInfo = getFillerCode(dr["Inform Mode"].ToString(), "MODENOTICE");
        if (dsModeofInfo.Tables[1].Rows.Count > 0)
            infoMode = dsModeofInfo.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();

        DataSet dsRelation = getFillerCode(dr["Relation"].ToString(), "NOTICEREL");
        if (dsRelation.Tables[1].Rows.Count > 0)
            relation = dsRelation.Tables[1].Rows[0]["Adt_AccountDesc"].ToString();

        dtpInformDateTime.Date = Convert.ToDateTime(dr["Inform Date"].ToString());
        txtModeOfInformation.Text = infoMode;
        txtInformant.Text = dr["Informant"].ToString();
        txtRelation.Text = relation;
        txtReason.Text = dr["Reason"].ToString();
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
                if (i > 0 && ddlType.Items[i].Value.Substring(0, 2).ToUpper().Equals(dr["Leave Code"].ToString().ToUpper()))
                {
                    ddlType.SelectedIndex = i;
                    break;
                }
            }
        }
        for (int i = 0; i < ddlCategory.Items.Count; i++)
        {
            if (ddlCategory.Items[i].Value.ToUpper().Equals(dr["Category"].ToString().ToUpper()))
            {
                ddlCategory.SelectedIndex = i;
                break;
            }
        }

        hfPrevDateCredits.Value = dr["Leave Date"].ToString();
        hfPrevType.Value = dr["Leave Code"].ToString();
        hfPrevStart.Value = dr["Start"].ToString();
        hfPrevEnd.Value = dr["End"].ToString();
        hfPrevDayUnit.Value = dr["Day Unit"].ToString();

        //initialize controls
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
        showOptionalFields();
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
        //LVBL.LeaveControlNo = this.txtControlNo.Text.Trim();
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
    #endregion
}

