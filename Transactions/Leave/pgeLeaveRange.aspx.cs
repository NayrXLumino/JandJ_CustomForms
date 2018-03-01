/*  Revision no. W2.1.00001
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   03/26/2013
 *  Update Notes    :   
 *      -   adding a query for endorse date on saving leave range
 *      -   separated check clinic in checkEntry2 function
 *      -   add filters in SL so that you could not file the leave which is used in the clinic
 */
/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/14/2013
 *  Update Notes    :   
 *      -   Updated saving part to check if Leave Credits are deductable
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
/// </summary>
public partial class Transactions_Leave_pgeLeaveRange : System.Web.UI.Page
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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFSPLLVEENTRY"))
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
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveIndividual_LoadComplete);
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

        TextBox txtTempFrom = (TextBox)dtpLVDateFrom.Controls[0];
        txtTempFrom.Attributes.Add("readOnly", "true");

        TextBox txtTempTo = (TextBox)dtpLVDateTo.Controls[0];
        txtTempTo.Attributes.Add("readOnly", "true");

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        ddlType.Attributes.Add("OnChange", "javascript:ddlType_ClientChangedLeaveRange();");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        initializeLeaveParameters();
        DataTable dt = new DataTable();
        dgvGenerated.DataSource = dt;
        dgvGenerated.DataBind();
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        CheckAll();
    }

    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Special trapping to enable filler for sickeness. Control Lookup is dependent on T_CoulumnFillerValues.
        //This would just hide or unhide extra control for Filler 1
        if (!ddlType.SelectedValue.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
            {
                string err = "";
                ParameterInfo[] param = new ParameterInfo[1];
                param[0] = new ParameterInfo("@LeaveType", ddlType.SelectedValue.Substring(0, 2).ToString());
                err = checkClinic(param, new DALHelper());
                if (!err.Equals(""))
                {
                    btnGenerate.Enabled = false;
                }
                else
                {
                    btnGenerate.Enabled = true;
                }
            }
            else
            {
                btnGenerate.Enabled = true;
                if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL))
                {
                    if (ddlType.SelectedValue.Substring(0, 2).Equals("SL"))
                    {


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
        }
        else
        {
            tbrFiller1.Visible = false;
        }
        
    }


    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!ddlCategory.SelectedValue.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
            {
                string err = "";
                ParameterInfo[] param = new ParameterInfo[1];
                param[0] = new ParameterInfo("@LeaveType", ddlCategory.SelectedValue.Substring(0, 2).ToString());
                err = checkClinic(param, new DALHelper());
                if (!err.Equals(""))
                {
                    btnGenerate.Enabled = false;
                }
                else
                {
                    btnGenerate.Enabled = true;
                }
            }
            else
            {
                btnGenerate.Enabled = true;
                if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL))
                {
                    if (ddlCategory.SelectedValue.Substring(0, 2).Equals("SL"))
                    {


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
        }
        else
        {
            tbrFiller1.Visible = false;
        }
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        string leaveYear = string.Format("{0}", LVBL.GetYear(dtpLVDateFrom.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
        decimal credits = LVBL.getCredits( txtEmployeeId.Text
                                         , leaveYear
                                         , ddlType.SelectedValue);
        string err = "";
        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG) && !ddlCategory.SelectedValue.Equals(""))//Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
        {
            
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@LeaveType", ddlCategory.SelectedValue.Substring(0, 2).ToString());
            err = checkClinic(param, new DALHelper());
           
        }
        if (err.Equals(""))
        {
            if (LeaveBL.IsUnpaidLeaveInMaster())
            {
                DataSet dsLeaveDates = LVBL.getRegularDaysOfEmployeeWithDetail(txtEmployeeId.Text
                                                                              , dtpLVDateFrom.Date.ToString("MM/dd/yyyy")
                                                                              , dtpLVDateTo.Date.ToString("MM/dd/yyyy")
                                                                              , ddlType.SelectedValue.Substring(0, 2)
                                                                              , credits);
                if (!CommonMethods.isEmpty(dsLeaveDates))
                {
                    dgvGenerated.DataSource = dsLeaveDates;
                    //MenuLog

                    //SystemMenuLogBL.InsertGenerateLog("WFSPLLVEENTRY", txtEmployeeId.Text, true, Session["userLogged"].ToString());
                    dgvGenerated.DataBind();
                    //pnlExcessCategory.Visible = (credits < dsLeaveDates.Tables[0].Rows.Count);
                    btnSaveEndorse.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No dates for filing retrieved between " + dtpLVDateFrom.Date.ToString("MM/dd/yyyy") + " - " + dtpLVDateTo.Date.ToString("MM/dd/yyyy"));
                }
            }
            else
            {
                MessageBox.Show(string.Format("Leave type {0} is not in Leave Type Master. Please contact the admin.", Resources.Resource.UNPAIDLVECODE));
            }
        }
        else
        {
            MessageBox.Show(err.ToString());
        }

    }
    protected void btnSaveEndorse_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            string datesFiled = string.Empty;
            string controlNo = string.Empty;
            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "LEAVE", "ENDORSE TO CHECKER 1");

            if (!status.Equals(string.Empty))
            {
                DataRow dr = null;
                int totalCount = 0;
                string forEmailDetail = string.Empty;
                string errmsg = string.Empty;
                if (Page.IsValid)
                {
                    if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                    {
                        int leaveCutOffCnt = 0;
                        for (int i = 0; i < dgvGenerated.Rows.Count; i++)
                        {
                            CheckBox tempCB = (CheckBox)dgvGenerated.Rows[i].Cells[0].FindControl("chkBox");
                            bool isChecked = tempCB.Checked;
                            if (isChecked)
                                if (CommonMethods.isAffectedByCutoff("LEAVE", dgvGenerated.Rows[i].Cells[1].Text.ToString()))
                                {
                                    leaveCutOffCnt++;
                                }
                        }
                        #region Get Included Dates
                        string strDates = "";
                        DateTime dtDate = dtpLVDateFrom.Date;
                        while (dtDate <= dtpLVDateTo.Date)
                        {
                            strDates += string.Format("{0:MM/dd/yyyy},", dtDate);
                            dtDate = dtDate.AddDays(1);
                        }
                        #endregion

                        if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                          || leaveCutOffCnt == 0)
                        {
                            bool isSuccess = false;
                            #region Count total transactions created
                            for (int i = 0; i < dgvGenerated.Rows.Count; i++)
                            {
                                CheckBox tempCB = (CheckBox)dgvGenerated.Rows[i].Cells[0].FindControl("chkBox");
                                bool isChecked = tempCB.Checked;
                                if (isChecked)
                                {
                                    totalCount++;
                                }
                            }
                            #endregion
                            using (DALHelper dal = new DALHelper())
                            {
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransactionSnapshot();
                                    int iCountSuccess = 0;
                                    string batchNo = "";

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

                                    string strCreateBatchLeaveQuery = string.Format("EXEC CreateBatchLeaveRecord '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}' "
                                                                            , batchNo
                                                                            , txtEmployeeId.Text.ToString()
                                                                            , strDates
                                                                            , ddlType.SelectedValue.ToUpper().Substring(0, 2)
                                                                            , Resources.Resource.UNPAIDLVECODE
                                                                            , "" //txtLVStartTime.Text.ToString().Replace(":", "")
                                                                            , "" //txtLVEndTime.Text.ToString().Replace(":", "")
                                                                            , "-1"
                                                                            , "WH"
                                                                            , txtReason.Text
                                                                            , ddlCategory.SelectedValue
                                                                            , DateTime.Now.ToString()
                                                                            , ""
                                                                            , Session["userLogged"].ToString()
                                                                            , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                            , "WFSPLLVEENTRY"
                                                                            , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION)
                                                                            , ""
                                                                            , filler1
                                                                            , filler2
                                                                            , filler3);
                                    DataTable dtResult = dal.ExecuteDataSet(strCreateBatchLeaveQuery).Tables[0];

                                    #region Displaying of Results
                                    if (dtResult.Rows.Count > 0)
                                    {
                                        batchNo = dtResult.Rows[0]["Batch Control No."].ToString();
                                        DataRow[] drArrRows = dtResult.Select("Result = 1");
                                        iCountSuccess = drArrRows.Length;
                                        DataTable dtFailedTransactions = dtResult.Clone();
                                        drArrRows = dtResult.Select("Result <> 1");
                                        foreach (DataRow drRow in drArrRows)
                                        {
                                            dtFailedTransactions.ImportRow(drRow);
                                        }
                                        int iCountFail = dtFailedTransactions.DefaultView.ToTable(true, "Employee ID", "Record Date").Rows.Count;
                                        lblErrorInfo.Text = string.Format("Consolidated Results (Successful = {0}, Failed = {1})", iCountSuccess, iCountFail);
                                        dgvReview.DataSource = dtResult;
                                        dgvReview.DataBind();
                                        pnlBound.Visible = false;
                                        pnlReview.Visible = true;
                                    }
                                    #endregion

                                    //Must have at least one successfully saved Leave record
                                    if (iCountSuccess > 0)
                                    {
                                        dal.CommitTransactionSnapshot();
                                        SystemMenuLogBL.InsertAddLog("WFSPLLVEENTRY", true, txtEmployeeId.Text, Session["userLogged"].ToString(), "", false);
                                    }
                                    else
                                    {
                                        dal.RollBackTransactionSnapshot();
                                        SystemMenuLogBL.InsertAddLog("WFSPLLVEENTRY", false, txtEmployeeId.Text, Session["userLogged"].ToString(), "", false);
                                    }
                                    refreshLeaveLedger();
                                }
                                catch (Exception ex)
                                {
                                    errmsg = ex.Message;
                                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                                    dal.RollBackTransactionSnapshot();
                                }
                                finally
                                {
                                    dal.CloseDB();
                                }
                            }
                            //if (isSuccess)
                            //{
                            //    restoreDefaultControls();
                                //MessageBox.Show("Successfully saved and endorsed transactions.");
                            //}
                        }
                        else
                        {
                            MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
                    }
                }
                if (errmsg.Trim() != string.Empty)
                {
                    MessageBox.Show(errmsg);
                }
            }
            else
            {
                MessageBox.Show("No route defined for user.");
            }
            
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        restoreDefaultControls();
    }
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "LEAVE", "WFSPLLVEENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;

        dtpLVDateFrom.Date = DateTime.Now;
        dtpLVDateFrom.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDateFrom.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        dtpLVDateTo.Date = DateTime.Now;
        dtpLVDateTo.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDateTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");

        hfPrevLVDateFrom.Value = dtpLVDateFrom.Date.ToShortDateString();
        hfPrevLVDateTo.Value = dtpLVDateTo.Date.ToShortDateString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        hfLVHRENTRY.Value = methods.GetProcessControlFlag("LEAVE", "LVHRENTRY").ToString();
        hfLHRSINDAY.Value = CommonMethods.getParamterValue("LHRSINDAY").ToString();
        lblUnit.Text = (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS)) ? "UNIT: IN HOURS" : "UNIT: IN DAYS";
        txtDateFiled.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm");
        enableButtons();
        showOptionalFields();
        pnlBound.Visible = true;
        pnlReview.Visible = false;
    }

    private void enableButtons()
    {
        btnSaveEndorse.Enabled = (dgvGenerated.Rows.Count > 0);
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
                                if (ddlType.SelectedValue.Equals("SL"))
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
                         -- AND Elm_LeaveYear = @Year

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
                          AND   (( Ltm_PaidLeave = 1
                              AND Ltm_WithCredit = 0)
                           OR   ( Ltm_PaidLeave = 0
                              AND ( Ltm_LeaveType <> CASE WHEN (Emt_Gender = 'F')
                                                          THEN ''
                                                          ELSE 'ML'
                                                      END
                                AND Ltm_WithCredit <> 1)
                              AND Ltm_Status = 'A'))
                        ORDER BY Ltm_PaidLeave DESC, Ltm_WithCategory ASC

            --Leave category query
                       SELECT Adt_AccountCode [Code]
                          , Adt_AccountDesc [Description]
                       FROM T_AccountDetail
                      WHERE Adt_AccountType = 'LVECATEGRY'
                        AND Adt_Status = 'A'";
        }
        #endregion
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDateFrom.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
        //if (dtpLVDateFrom.Date.Year > Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR")))
        //{
        //    if (dtpLVDateFrom.Date < Convert.ToDateTime(Resources.Resource.LEAVEREFRESH + "/" + (Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR") + 1).ToString())))
        //    {
        //        param[1] = new ParameterInfo("@Year", (dtpLVDateFrom.Date.Year - 1).ToString());
        //    }
        //    else
        //    {
        //        param[1] = new ParameterInfo("@Year", dtpLVDateFrom.Date.ToString("yyyy"));
        //    }
        //}
        //else
        //{
        //    param[1] = new ParameterInfo("@Year", dtpLVDateFrom.Date.ToString("yyyy"));
        //}

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
            //Initialize category dropdown
            ddlCategory.Items.Clear();
            ddlCategoryExcess.Items.Clear();
            if (ds.Tables[2].Rows.Count > 0)
            {
                ddlCategory.Items.Add(new ListItem("", ""));
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {
                    ddlCategory.Items.Add(new ListItem(ds.Tables[2].Rows[i]["Description"].ToString()
                                                  , ds.Tables[2].Rows[i]["Code"].ToString()));
                    ddlCategoryExcess.Items.Add(new ListItem(ds.Tables[2].Rows[i]["Description"].ToString()
                                                  , ds.Tables[2].Rows[i]["Code"].ToString()));
                }
            }
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
                         -- AND Elm_LeaveYear = @Year ";
        }

        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[1] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDateFrom.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));
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
        }
        else
        {
            dgvLedger.DataSource = new DataTable("Dummy");
            dgvLedger.DataBind();
            lblCheckLedger.Text = " - No PAID Leave credits";
        }

    }

    private string checkClinic(ParameterInfo[] param,DALHelper dal)
    {
        string err = "";
        if (Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
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
	AND Ltm_LeaveType = @LeaveType
	AND Ltm_LeaveType NOT IN ('UN','{0}')
                ", Resources.Resource.UNPAIDLVECODE), CommandType.Text, param);
            if (!CommonMethods.isEmpty(dsLeave))
            {
                if (dsLeave.Tables[0].Rows[0][0].ToString().Trim() == "WITH CAT")
                {
                    if (ddlCategory.SelectedValue.ToString().Trim() == "SL")
                    {
                        err += "Leave type " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " with SICK LEAVE Category can only be filed by CLINIC";
                    }
                }
                else
                {
                    err += "Leave type " + ddlType.Items[ddlType.SelectedIndex].Text.ToString() + " can only be filed by CLINIC";
                }
            }
            #endregion
        }
        return err;
    }

    private void CheckAll()
    {
        int ctrZ = 0;
        if (dgvGenerated.Rows.Count > 0)
        {
            CheckBox CBA = (CheckBox)dgvGenerated.HeaderRow.Cells[0].FindControl("chkBoxAll");
            if (CBA.Checked)
            {
                for (ctrZ = 0; ctrZ < dgvGenerated.Rows.Count; ctrZ++)
                {
                    CheckBox CB = (CheckBox)dgvGenerated.Rows[ctrZ].Cells[0].FindControl("chkBox");
                    CB.Checked = true;
                }
            }
            else
            {
                for (ctrZ = 0; ctrZ < dgvGenerated.Rows.Count; ctrZ++)
                {
                    CheckBox CB = (CheckBox)dgvGenerated.Rows[ctrZ].Cells[0].FindControl("chkBox");
                    CB.Checked = false;
                }
            }
        }
    }
    
    private void capturePrevValues()
    {
        hfPrevType.Value = ddlType.SelectedValue;
        hfPrevEntry.Value = changeSnapShot();
    }

    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = dtpLVDateFrom.Date.ToString()
                 + dtpLVDateTo.Date.ToString()
                 + ddlType.SelectedValue
                 + ddlCategory.SelectedValue
                 + txtReason.Text
                 + txtFiller1.Text
                 + txtFiller2.Text
                 + txtFiller3.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;
        hfPrevEntry.Value = string.Empty;
        btnSaveEndorse.Enabled = false;
        pnlExcessCategory.Visible = false;
        hfSaved.Value = "0";
        initializeEmployee();
        initializeControls();
        initializeLeaveParameters();
        //hard coded
        if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL))
        {
            if (ddlType.Items.Count > 0
                && ddlType.SelectedValue.ToString().Trim() != string.Empty
                && !ddlType.SelectedValue.Substring(0, 2).Equals("SL"))
            {
                tbrFiller1.Visible = false;
            }
        }
        else
        {
            tbrFiller1.Visible = false;
        }

        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
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
        string query = string.Format(@"SELECT Cfl_Lookup
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
        DataTable dt= new DataTable();
        string result = string.Empty;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
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
        if (dt.Rows.Count > 0)
            result = dt.Rows[0]["Cfl_Lookup"].ToString();
        return result;
    }
    //end
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
}
