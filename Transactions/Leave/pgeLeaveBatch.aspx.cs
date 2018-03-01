using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;
using System.Collections.Generic;

public partial class Transactions_Leave_pgeLeaveBatch : System.Web.UI.Page
{
    private readonly OvertimeBL OTBL = new OvertimeBL();
    private readonly LeaveBL LVBL = new LeaveBL();
    private readonly MenuGrant MGBL = new MenuGrant();
    private readonly CommonMethods methods = new CommonMethods();
    private bool isLogledger = true;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBLVENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeControls();
                initializeLeaveParameters();
                ddlType_SelectedIndexChanged(null, null);
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                
                if (!hfPrevLVDate.Value.Equals(dtpLVDate.Date.ToShortDateString()))
                {
                    //initializeControls();
                    initializeLeaveParameters();
                    dtpLVDate_Change(dtpLVDate, new EventArgs());
                    hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveBatch_LoadComplete);
        }
    }


    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
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

    public void illnessShow(DropDownList ddl)
    {
        if (!ddl.SelectedValue.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.USEILLNESSONSL)
                && !Convert.ToBoolean(Resources.Resource.CLINICSYSFLAG))
            {
                if (ddl.SelectedValue.Substring(0, 2).Equals("SL"))
                {
                    showOptionalFields();
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
    void Transactions_Leave_pgeLeaveBatch_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "leaveScripts";
        string jsurl = "_leave.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        ddlType.Attributes.Add("OnChange", "javascript:ddlType_ClientChanged();");
        rblDayUnit.Attributes.Add("OnClick", "javascript:rblDayUnit_ClientChanged();");
        txtLVEndTime.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpLVDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpLVDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");
        txtLVStartTime.Attributes.Add("readOnly", "true");
        //txtOTStartTime.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        txtLVStartTime.Attributes.Add("readOnly", "true");
        txtLVStartTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVStartTime')");
        txtLVStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtLVEndTime.Attributes.Add("OnKeyUp", "javascript:formatTime('txtLVEndTime')");
        txtLVEndTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        //txtLVStartTime.Attributes.Add("OnChange", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnKeyUp", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnChange", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        txtReason.Attributes.Add("OnKeyUp", "javascript:isMaxLengthAfterKeyPress('txtReason',199);");
        hfCHIYODA.Value = Resources.Resource.CHIYODASPECIFIC.ToString().ToUpper();
        disableControlsOnView();
    }
    //ROBERT ADDED to set the start time correctly. Depending on the day code 09122013
    public void SetStartTime()
    {
        if (hfDayCode.Value.ToString().Contains("REG"))
        {
            //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
        }
        else
        {

           // txtOTStartTime.Text = hfI1.Value.Insert(2, ":");

        }
    }
    protected void dtpLVDate_Change(object sender, EventArgs e)
    {
        fillddlGroup();
        if (ddlGroup.Items.Count != 0)
        {
            fillDDLShift();
            if (ddlShift.Items.Count != 0)
            {
                initializeShift();
                fillddlCostcenter();
                if (ddlCostcenter.Items.Count != 0)
                {
                    //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
                    SetStartTime();
                    hfddlChange.Value = "1";
                    showOptionalFields();
                    txtSearch_TextChanged(null, null);
                }
                else
                {
                    MessageBox.Show("No costcenter access retrieved");
                }
            }
            else
            {
                MessageBox.Show("No shift retrieved for this date");
            }
        }
        else
        {
            MessageBox.Show("No Work Group/Work Type retrieved for this date.");
            restoreDefaultControls();
            initializeControls();
        }
    }

    protected void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        fillDDLShift();
        if (ddlShift.Items.Count != 0)
        {
            initializeShift();
            fillddlCostcenter();
            if (ddlCostcenter.Items.Count != 0)
            {
                //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
                SetStartTime();
                hfddlChange.Value = "1";
                txtSearch_TextChanged(null, null);
            }
            else
            {
                MessageBox.Show("No costcenter access retrieved");
            }
        }
        else
        {
            MessageBox.Show("No shift retrieved for this date");
        }
    }

    protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
    {
        initializeShift();
        fillddlCostcenter();
        
        if (ddlCostcenter.Items.Count != 0)
        {
            //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
            SetStartTime();
            hfddlChange.Value = "1";
            txtSearch_TextChanged(null, null);
        }
        else
        {
            MessageBox.Show("No costcenter access retrieved");
        }
    }

    protected void ddlCostcenter_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfddlChange.Value = "1";
        txtSearch_TextChanged(null, null);
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        if (hfddlChange.Value == "1")
        {
            txtSearch.Text = "";
        }
        FillChoiceList();
        for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
        {
            lbxChoice.Items.Remove(lbxInclude.Items[ctr]);
        }

        lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
    }

    protected void btnIncludeIndi_Click(object sender, EventArgs e)
    {
        if (lbxChoice.SelectedIndex > -1)
        {
            lbxInclude.Items.Add(lbxChoice.SelectedItem);
            lbxChoice.Items.Remove(lbxChoice.SelectedItem);
            lbxChoice.SelectedIndex = -1;
            lbxInclude.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
            try
            {
                lbxChoice.SelectedIndex = 0;
                lbxChoice.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnRemoveIndi_Click(object sender, EventArgs e)
    {
        if (lbxInclude.SelectedIndex > -1)
        {
            lbxChoice.Items.Add(lbxInclude.SelectedItem);
            lbxInclude.Items.Remove(lbxInclude.SelectedItem);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
            try
            {
                lbxInclude.SelectedIndex = 0;
                lbxInclude.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnIncludeAll_Click(object sender, EventArgs e)
    {
        while (lbxChoice.Items.Count > 0)
        {
            lbxInclude.Items.Add(lbxChoice.Items[0]);
            lbxChoice.Items.Remove(lbxChoice.Items[0]);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
        }
    }

    protected void btnRemaoveAll_Click(object sender, EventArgs e)
    {
        while (lbxInclude.Items.Count > 0)
        {
            lbxChoice.Items.Add(lbxInclude.Items[0]);
            lbxInclude.Items.Remove(lbxInclude.Items[0]);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
        }
    }

    protected void btnEndorse_Click(object sender, EventArgs e)
    {
        this.Process("E");
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            pnlBound.Visible = true;
            pnlReview.Visible = false;
            //MenuLog
            SystemMenuLogBL.InsertDeleteLog("WFBLVENTRY", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "");
            MessageBox.Show("Cancelled Transaction(s)");
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//This will just disregard all entries

    protected void btnClear_Click(object sender, EventArgs e)
    {
        restoreDefaultControls();
        initializeControls();
    }
    #endregion

    #region Methods
    #region Perth Added 12/27/2012 Consolidate Checking
    private void Process(string TransactionType)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("LEAVE", dtpLVDate.Date.ToString("MM/dd/yyyy")))
                {
                    #region Get Included Employees
                    string strEmployees = "";
                    for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                    {
                        strEmployees += lbxInclude.Items[ctr].Value + ",";
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
                                                                            , strEmployees
                                                                            , dtpLVDate.Date.ToString("MM/dd/yyyy")
                                                                            , ddlType.SelectedValue.ToUpper().Substring(0, 2)
                                                                            , Resources.Resource.UNPAIDLVECODE
                                                                            , txtLVStartTime.Text.ToString().Replace(":", "")
                                                                            , txtLVEndTime.Text.ToString().Replace(":", "")
                                                                            , "-1"
                                                                            , rblDayUnit.SelectedValue
                                                                            , txtReason.Text
                                                                            , ddlCategory.SelectedValue
                                                                            , DateTime.Now.ToString()
                                                                            , ""
                                                                            , Session["userLogged"].ToString()
                                                                            , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                            , "WFBLVENTRY"
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
                                SystemMenuLogBL.InsertAddLog("WFBLVENTRY", true, "", Session["userLogged"].ToString(), "", false);
                            }
                            else
                            {
                                dal.RollBackTransactionSnapshot();
                                SystemMenuLogBL.InsertAddLog("WFBLVENTRY", false, "", Session["userLogged"].ToString(), "", false);
                            }
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
                    /*
                    string errMsg1 = checkEntry1();
                    if (errMsg1.Equals(string.Empty))
                    {
                        #region array to check individual employees
                        string[] employees = null;
                        employees = new string[lbxInclude.Items.Count];
                        for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                        {
                            employees[ctr] = lbxInclude.Items[ctr].Value;
                        }
                        #endregion

                        if (employees.Length > 0)
                        {
                            bool isValid = true;
                            DataTable temp = new DataTable();
                            DataTable dtFinal = new DataTable();

                            #region Validations
                            temp = CheckIfValid();
                            if (temp != null
                                && temp.Rows.Count > 0)
                            {
                                isValid = false;
                                lblErrorInfo.Text = "Consolidated Validation Results";
                                dgvReview.DataSource = temp;
                                dgvReview.DataBind();
                            }
                            #endregion

                            if (!isValid)
                            {
                                pnlBound.Visible = false;
                                pnlReview.Visible = true;
                                if (TransactionType == "E")
                                {
                                    MessageBox.Show("Review the following entries.");
                                }
                            }

                            if (isValid
                                || TransactionType == "D"
                                || TransactionType == "K")
                            {
                                #region Insert Records
                                using (DALHelper dal = new DALHelper())
                                {
                                    try
                                    {
                                        dal.OpenDB();
                                        dal.BeginTransactionSnapshot();

                                        InsertTransactions(dal, temp);

                                        if (TransactionType == "K")
                                        {
                                            RepopulateList();
                                            pnlBound.Visible = false;
                                            pnlReview.Visible = true;
                                        }
                                        else
                                        {
                                            restoreDefaultControls();
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
                                #endregion
                            }
                        }
                        else
                        {
                            MessageBox.Show("No employee(s) selected");
                        }
                    }
                    else
                    {
                        MessageBox.Show(errMsg1);
                    }
                    */
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
    }
    #endregion

    private void initializeControls()
    {
        dtpLVDate.Date = DateTime.Now;
        //Default Parameter
        //dtpOTDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        hfLVHRENTRY.Value = (!methods.GetProcessControlFlag("LEAVE", "DAYSEL")).ToString();
        dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        hfLVHRENTRY.Value = methods.GetProcessControlFlag("LEAVE", "LVHRENTRY").ToString();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevLVDate.Value = dtpLVDate.Date.ToShortDateString();
        //OTBL.initializeOTTypes(ddlType, false);
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        fillddlGroup();
        if (ddlGroup.Items.Count != 0)
        {
            fillDDLShift();
            if (ddlShift.Items.Count != 0)
            {
                initializeShift();
                fillddlCostcenter();
                if (ddlCostcenter.Items.Count != 0)
                {
                    //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
                    SetStartTime();
                    txtSearch_TextChanged(null, null);
                    showOptionalFields();
                }
                else
                {
                    MessageBox.Show("No costcenter access retrieved");
                }
            }
            else
            {
                MessageBox.Show("No shift retrieved for this date");
            }
        }
        else
        {
            MessageBox.Show("No Work Group/Work Type retrieved for this date.");
            restoreDefaultControls();
            //initializeControls();
        }
        hfSTRTOTFRAC.Value = CommonMethods.getParamterValue("STRTOTFRAC").ToString();
        hfOTFRACTION.Value = CommonMethods.getParamterValue("OTFRACTION").ToString();
        //Andre added 20130702
        try
        {
            hfOTSTARTPAD.Value = CommonMethods.getParamterValue("OTSTARTPAD").ToString();
        }
        catch { }

        pnlBound.Visible = true;
        pnlReview.Visible = false;
    }
    
    private void initializeLeaveParameters()
    {
        DataSet ds = new DataSet();
        #region SQL
        string sql = @"
            Select ''
            --Leave type query
                                              SELECT Ltm_LeaveType [Type]
                            , Ltm_LeaveDesc [Description]
                            , Ltm_WithCategory [Category]
                            , REPLICATE(' ', 2 - LEN(Ltm_LeaveType)) + RTRIM(Ltm_LeaveType)
                            + Convert(varchar(1),Ltm_WithCategory)
                            + Convert(varchar(1),Ltm_WithCredit)
                            + REPLICATE(' ', 21- LEN(ISNULL(Ltm_DayUnit,''))) + RTRIM(ISNULL(Ltm_DayUnit,'')) [Code]
                            , Ltm_PaidLeave
                         FROM T_LeaveTypeMaster
                         --LEFT JOIN T_Employeemaster
                         --  ON Emt_EmployeeId = @EmployeeId
                        WHERE Ltm_Status = 'A'
                          AND Ltm_CombinedLeave = 0
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

        
        #endregion
        ParameterInfo[] param = new ParameterInfo[1];
        //param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        param[0] = new ParameterInfo("@Year", LVBL.GetYear(dtpLVDate.Date, ddlType.SelectedValue.ToString() != string.Empty ? ddlType.SelectedValue.ToString().Substring(0, 2) : string.Empty));

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

    private void initializeShift()
    {
        DataSet ds = new DataSet();
        try
        {
            ds = CommonMethods.getShiftInformation(ddlShift.SelectedValue.Substring(0, 10).Trim());
            if (!CommonMethods.isEmpty(ds))
            {
                hfDayCode.Value = ddlShift.SelectedValue.Substring(10, 4).Trim();
                hfShiftType.Value = ds.Tables[0].Rows[0]["Scm_ScheduleType"].ToString();
                hfShiftHours.Value = ds.Tables[0].Rows[0]["Scm_ShiftHours"].ToString();
                hfShiftPaid.Value = ds.Tables[0].Rows[0]["Scm_PaidBreak"].ToString();
                hfI1.Value = ds.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString();
                hfO1.Value = ds.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString();
                hfI2.Value = ds.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString();
                hfO2.Value = ds.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString();
            }
            //this.txtOTHours.Text = string.Empty;
           // this.txtOTEndTime.Text = string.Empty;
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            MessageBox.Show("No shift retrieved for this date.");
        }
    }

    private void restoreDefaultControls()
    {
        txtLVStartTime.Text = string.Empty;
        txtLVEndTime.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;
        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();
        hfPrevLVDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        //initializeControls(); //Commented by Rendell Uy - 3/28/2016 (Will cause infinite loop if there is no cost center access record for LEAVE system
    }

    private void fillddlGroup()
    {
        DataTable dt = new DataTable();
        string sql = @" SELECT DISTINCT ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Desc]
                             , REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup) [Code]
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         LEFT JOIN T_AccountDetail as AD1
                            ON AD1.Adt_AccountType = 'WORKTYPE' 
                           AND AD1.Adt_AccountCode = Ell_WorkType
                         LEFT JOIN T_AccountDetail as AD2
                            ON AD2.Adt_AccountType = 'WORKGROUP'
                           AND AD2.Adt_AccountCode = Ell_WorkGroup
                         WHERE Ell_ProcessDate = @ProcessDate
                           {0}

                         UNION

                        SELECT DISTINCT ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Desc]
                             , REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup) [Code]
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         LEFT JOIN T_AccountDetail as AD1
                            ON AD1.Adt_AccountType = 'WORKTYPE' 
                           AND AD1.Adt_AccountCode = Ell_WorkType
                         LEFT JOIN T_AccountDetail as AD2
                            ON AD2.Adt_AccountType = 'WORKGROUP'
                           AND AD2.Adt_AccountCode = Ell_WorkGroup
                         WHERE Ell_ProcessDate = @ProcessDate
                           {0}

                         ORDER BY [Code]";

        CommonMethods CM = new CommonMethods();
        string end = CM.FetchQuincena('f', 'e').ToString();
        if (dtpLVDate.Date > Convert.ToDateTime(end))
        {
            sql = @" SELECT DISTINCT ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Desc]
                             , REPLICATE(' ', 3 - LEN(Emt_WorkType)) + RTRIM(Emt_WorkType)
                             + REPLICATE(' ', 3 - LEN(Emt_WorkGroup)) + RTRIM(Emt_WorkGroup) [Code]
                          FROM T_EmployeeMaster
                         LEFT JOIN T_AccountDetail as AD1
                            ON AD1.Adt_AccountType = 'WORKTYPE' 
                           AND AD1.Adt_AccountCode = Emt_WorkType
                         LEFT JOIN T_AccountDetail as AD2
                            ON AD2.Adt_AccountType = 'WORKGROUP'
                           AND AD2.Adt_AccountCode = Emt_WorkGroup
                         WHERE 1=1
                           {0}

                         ORDER BY [Code]";
        }

        string filter;
        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
            filter = @"AND Emt_CostcenterCode IN (SELECT Uca_CostcenterCode
                                                            FROM T_UserCostcenterAccess
                                                           WHERE Uca_UserCode = @UserCode
                                                             AND Uca_SytemID = @TransactionType)";
        else
            filter = string.Empty;
        ParameterInfo[] param = new ParameterInfo[3];
        param[0] = new ParameterInfo("@ProcessDate", dtpLVDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[2] = new ParameterInfo("@TransactionType", "LEAVE");

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(string.Format(sql, filter), CommandType.Text, param).Tables[0];
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                MessageBox.Show("No group codes retrieved.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        ddlGroup.Items.Clear();
        for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
        {
            ddlGroup.Items.Add(new ListItem(dt.Rows[ctr]["Desc"].ToString(), dt.Rows[ctr]["Code"].ToString()));
        }
    }

    private void fillDDLShift()
    {
        DataTable dt = new DataTable();
        string sql = @" SELECT DISTINCT REPLICATE(' ' , 10 - LEN(Ell_ShiftCode)) + Ell_ShiftCode
                             + REPLICATE(' ' , 4 - LEN(Ell_DayCode)) + Ell_DayCode [Code]
                             , Scm_ShiftDesc [Desc]
                             , Ell_ShiftCode [ShiftCode]
                             , Ell_DayCode + REPLICATE(' ' , 4 - LEN(Ell_DayCode)) [DayCode]
                             , LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) [TimeIn]
                             , LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) [BreakStart]
                             , LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) [BreakEnd]
                             , LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [TimeOut]
                             , Ell_DayCode
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ' , 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)  = @workTYPGRP
                           AND Ell_Processdate = @ProcessDate

                         UNION

                        SELECT DISTINCT REPLICATE(' ' , 10 - LEN(Ell_ShiftCode)) + Ell_ShiftCode
                             + REPLICATE(' ' , 4 - LEN(Ell_DayCode)) + Ell_DayCode [Code]
                             , Scm_ShiftDesc [Desc]
                             , Ell_ShiftCode [ShiftCode]
                             , Ell_DayCode + REPLICATE(' ' , 4 - LEN(Ell_DayCode)) [DayCode]
                             , LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) [TimeIn]
                             , LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) [BreakStart]
                             , LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) [BreakEnd]
                             , LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [TimeOut]
                             , Ell_DayCode
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType) 
                             + REPLICATE(' ' , 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)  = @workTYPGRP
                           AND Ell_Processdate = @ProcessDate

                         ORDER BY [Code]";
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            if (dtpLVDate.Date > DateTime.Now)
            {
                sql = @" SELECT REPLICATE(' ' , 10 - LEN(DefaultShift.Scm_ShiftCode)) + DefaultShift.Scm_ShiftCode
	                          + REPLICATE(' ' , 4 - LEN('REG')) + 'REG' [Code]
	                          , DefaultShift.Scm_ShiftDesc [Desc]
	                          , DefaultShift.Scm_ShiftCode [ShiftCode]
	                          , 'REG' + REPLICATE(' ' , 4 - LEN('REG')) [DayCode]
	                          , LEFT(DefaultShift.Scm_ShiftTimeIn,2) + ':' + RIGHT(DefaultShift.Scm_ShiftTimeIn,2) [TimeIn]
	                          , LEFT(DefaultShift.Scm_ShiftBreakStart,2) + ':' + RIGHT(DefaultShift.Scm_ShiftBreakStart,2) [BreakStart]
	                          , LEFT(DefaultShift.Scm_ShiftBreakEnd,2) + ':' + RIGHT(DefaultShift.Scm_ShiftBreakEnd,2) [BreakEnd]
	                          , LEFT(DefaultShift.Scm_ShiftTimeOut,2) + ':' + RIGHT(DefaultShift.Scm_ShiftTimeOut,2) [TimeOut]
                              --, Ell_DayCode
	                       FROM T_EmployeeMaster
	                      INNER JOIN T_ShiftCodeMaster ShiftCode
	                         ON ShiftCode.Scm_ShiftCode = Emt_ShiftCode
	                       LEFT JOIN T_ShiftCodeMaster DefaultShift
	                         ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
	                        AND DefaultShift.Scm_DefaultShift = 'True'
	                        AND DefaultShift.Scm_Status = 'A'
	                      WHERE Emt_EmployeeID = '{0}'";
            }
        }
        else
        {
            CommonMethods CM = new CommonMethods();
            string end = CM.FetchQuincena('f', 'e').ToString();
            if (dtpLVDate.Date > Convert.ToDateTime(end))
            {
                sql = @" DECLARE @StartDate AS datetime = @ProcessDate
                        
                        SELECT DISTINCT REPLICATE(' ' , 10 - LEN(Emt_ShiftCode)) + Emt_ShiftCode
                             + REPLICATE(' ' , 4 - LEN([DayCode])) + [DayCode] [Code]
                             , Scm_ShiftDesc [Desc]
                             , Emt_ShiftCode [ShiftCode]
                             , [DayCode] + REPLICATE(' ' , 4 - LEN([DayCode])) [DayCode]
                             , LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) [TimeIn]
                             , LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) [BreakStart]
                             , LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) [BreakEnd]
                             , LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [TimeOut]
                             , [DayCode] Ell_DayCode
                          FROM T_EmployeeMaster
                        INNER JOIN
                        (SELECT Emt_EmployeeID [EMPID], CASE WHEN Emt_WorkType <> 'REG' 
                        THEN
	                        (SELECT 
		                            CASE WHEN Hmt_HolidayDate IS NOT NULL
			                        THEN
				                        Hmt_HolidayCode 
			                        ELSE		
			                            CASE WHEN RTRIM(Cal_WorkCode) = 'R'
				                        THEN 'REST'
				                        ELSE 'REG' +  CASE WHEN LEN(RTRIM(Cal_WorkCode)) > 1
								                        THEN RIGHT(RTRIM(Cal_WorkCode), 1)
								                        ELSE ''
								                        END
				                        END 
			                        END [DayCode]
		
	                        FROM T_CalendarGroupTmp
	                        LEFT JOIN T_HolidayMaster
		                        ON Hmt_HolidayDate = Cal_ProcessDate
		                        AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									                        THEN Hmt_ApplicCity
									                        ELSE Emt_LocationCode--@LOCATIONCODE
									                        END
	
	                        WHERE Cal_WorkType = Emt_WorkType--@WORKTYPE
	                        AND Cal_WorkGroup = Emt_WorkGroup--@WORKGROUP
	                        AND Cal_ProcessDate BETWEEN @StartDate AND @StartDate)
                        ELSE
	                        (SELECT 
		                            CASE WHEN 1 = (
			                        SELECT 
				                        CASE WHEN CAL.DayOfWeek = 1
				                        THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				                        ELSE 
					                        RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				                        END
			                        FROM T_EmployeeRestDay E1
			                        WHERE Erd_EmployeeID = Emt_EmployeeID--@Ell_EmployeeID
				                        AND Erd_EffectivityDate = (
					                        SELECT 
						                        TOP 1 Erd_EffectivityDate 
					                        FROM T_EmployeeRestDay E2
					                        WHERE E2.Erd_EmployeeID = Emt_EmployeeID--@Ell_EmployeeID
						                        AND E2.Erd_EffectivityDate <= CalendarDate
					                        ORDER BY E2.Erd_EffectivityDate DESC
				                        )
			                        ) 
			                        THEN 
				                        CASE WHEN Hmt_HolidayDate IS NOT NULL
					                        THEN 
						                        Hmt_HolidayCode
					                        ELSE
						                        'REST'
				                        END
			                        ELSE
				                        'REG'
			                        END [DayCode]
		
	                        FROM dbo.GetCalendarDates(@StartDate, @StartDate) CAL
	                        LEFT JOIN T_HolidayMaster
		                        ON Hmt_HolidayDate = CAL.CalendarDate
		                        AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									                        THEN Hmt_ApplicCity
									                        ELSE Emt_LocationCode--@LOCATIONCODE
									                        END)
                        END [DayCode]
                        FROM T_EMployeeMaster)A
                        ON EMPID = Emt_EmployeeID
                            AND DayCode IS NOT NULL
                            AND Emt_JobStatus like 'A%'
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Emt_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Emt_WorkType)) + RTRIM(Emt_WorkType)
                             + REPLICATE(' ' , 3 - LEN(Emt_WorkGroup)) + RTRIM(Emt_WorkGroup)  = @workTYPGRP
                         
                        ORDER BY 2
                           ";
                isLogledger = false;
            }
        }
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@ProcessDate", dtpLVDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@workTYPGRP", ddlGroup.SelectedValue);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0];
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                MessageBox.Show("No shift codes retrieved.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        ddlShift.Items.Clear();
        for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
        {
            ddlShift.Items.Add(new ListItem(dt.Rows[ctr]["ShiftCode"]
                                           + "-"
                                           + dt.Rows[ctr]["DayCode"]
                                           + " ["
                                           + dt.Rows[ctr]["TimeIn"]
                                           + "-"
                                           + dt.Rows[ctr]["BreakStart"]
                                           + "   "
                                           + dt.Rows[ctr]["BreakEnd"]
                                           + "-"
                                           + dt.Rows[ctr]["TimeOut"]
                                           + "]"
                                           , dt.Rows[ctr]["Code"].ToString()));
        }
    }

    private void fillddlCostcenter()
    {
        {
            DataTable dt = new DataTable();
            string sql = @" SELECT DISTINCT RTRIM(Emt_CostcenterCode) + '-' + ISNULL(Ecm_LineCode,'') [Code]
	                             , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) +	CASE WHEN Ecm_LineCode IS NULL THEN '' ELSE + ' - LINE ' + Ecm_LineCode END  [Costcenter]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID
							 LEFT JOIN E_EmployeeCostCenterLineMovement
							    ON Ecm_EMployeeID = Emt_EmployeeID
							   AND Ell_ProcessDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))

                             -->INNER JOIN T_UserCostCenterAccess
                             -->   ON Uca_CostcenterCode = Emt_CostcenterCode
                             -->  AND Uca_UserCode = @UserCode
                             -->  AND Uca_SytemID = 'LEAVE'

                             WHERE Ell_ProcessDate = @ProcessDate
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup
                               AND Ell_ShiftCode = @ShiftCode

                             UNION

                            SELECT DISTINCT RTRIM(Emt_CostcenterCode) + '-' + ISNULL(Ecm_LineCode,'') [Code]
	                             , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) +	CASE WHEN Ecm_LineCode IS NULL THEN '' ELSE + ' - LINE ' + Ecm_LineCode END  [Costcenter]
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID
							 LEFT JOIN E_EmployeeCostCenterLineMovement
							    ON Ecm_EMployeeID = Emt_EmployeeID
							   AND Ell_ProcessDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))

                             -->INNER JOIN T_UserCostCenterAccess
                             -->   ON Uca_CostcenterCode = Emt_CostcenterCode
                             -->  AND Uca_UserCode = @UserCode
                             -->  AND Uca_SytemID = 'LEAVE'

                             WHERE Ell_ProcessDate = @ProcessDate
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup
                               AND Ell_ShiftCode = @ShiftCode

                             ORDER BY [Costcenter]";

            CommonMethods CM = new CommonMethods();
            string end = CM.FetchQuincena('f', 'e').ToString();
            if (dtpLVDate.Date > Convert.ToDateTime(end))
            {
                sql = @" SELECT DISTINCT RTRIM(Emt_CostcenterCode) + '-' + ISNULL(Ecm_LineCode,'') [Code]
	                             , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) +	CASE WHEN Ecm_LineCode IS NULL THEN '' ELSE + ' - LINE ' + Ecm_LineCode END  [Costcenter]
                              FROM T_EmployeeMaster
							 LEFT JOIN E_EmployeeCostCenterLineMovement
							    ON Ecm_EMployeeID = Emt_EmployeeID
							   AND @ProcessDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))

                             -->INNER JOIN T_UserCostCenterAccess
                             -->   ON Uca_CostcenterCode = Emt_CostcenterCode
                             -->  AND Uca_UserCode = @UserCode
                             -->  AND Uca_SytemID = 'LEAVE'

                             WHERE Emt_WorkType = @WorkType
                               AND Emt_WorkGroup = @WorkGroup
                               AND Emt_ShiftCode = @ShiftCode

                             ORDER BY [Costcenter]";
            }
            //ParameterInfo[] param = new ParameterInfo[5];
            sql = sql.Replace("@UserCode", string.Format("'{0}'", Session["userLogged"].ToString()));
            sql = sql.Replace("@ProcessDate", string.Format("'{0}'", dtpLVDate.Date.ToString("MM/dd/yyyy")));
            sql = sql.Replace("@WorkType", string.Format("'{0}'", ddlGroup.SelectedValue.Substring(0, 3).Trim()));
            sql = sql.Replace("@WorkGroup", string.Format("'{0}'", ddlGroup.SelectedValue.ToString().Length >= 6 ? ddlGroup.SelectedValue.Substring(3, 3).Trim() : string.Empty));
            sql = sql.Replace("@ShiftCode", string.Format("'{0}'", ddlShift.SelectedValue.Substring(0, 10).Trim()));

            //if costcenter access is not all, inner join with usercostcenter access to filter 
            //by removing commented part of query
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
            {
                sql = sql.Replace("-->", "");
            }
            //end
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                    MessageBox.Show("No costcenter access retrieved.");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            ddlCostcenter.Items.Clear();
            ddlCostcenter.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem("ALL", "ALL"));
            for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
            {
                ddlCostcenter.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem(dt.Rows[ctr]["Costcenter"].ToString(), dt.Rows[ctr]["Code"].ToString()));
            }
            ddlCostcenter.Items[0].Selected = true;
        }
    }

    private void setAllCostcenter()
    {
        string sql = @" SELECT DISTINCT Uca_CostCenterCode [Code]
                             , dbo.getCostCenterFullNameV2(Uca_CostCenterCode)[Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_SytemId = 'LEAVE' 
                           AND Uca_Status = 'A'
                           AND Uca_CostCenterCode NOT IN ('', 'ALL')";
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(string.Format(sql, Session["userLogged"].ToString()), CommandType.Text).Tables[0];
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                MessageBox.Show("No additional costcenter access retrieved.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
        {
            ddlCostcenter.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem(dt.Rows[ctr]["Desc"].ToString(), dt.Rows[ctr]["Code"].ToString()));
        }
    }

    private void FillChoiceList()
    {
        DataTable dt = new DataTable();
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string sql = string.Format(@" 
                        DECLARE @DSP as bit
                        SET @DSP = (SELECT Pcm_ProcessFlag
                                      FROM T_ProcessControlMaster
                                     WHERE Pcm_ProcessId = 'DSPFULLNM')

                        SELECT DISTINCT Emt_EmployeeId 
                             + '   -   '
                             + CASE WHEN (@DSP = '1')
                                    THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                    ELSE Emt_NickName
                                END AS [Employee Name]
                             , Ell_EmployeeId
                             , CASE WHEN (@DSP = '1')
                                    THEN Emt_LastName + Emt_FirstName
                                    ELSE Emt_NickName
                                END AS [ForSort]
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeID = Ell_EmployeeID
                            {3}
                         WHERE Ell_ProcessDate = '{1}'
                            -- Filter Insertion -- 
                           {0}
                           {2}

                         UNION

                        SELECT DISTINCT Emt_EmployeeId 
                             + '   -   '
                             + CASE WHEN (@DSP = '1')
                                    THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                    ELSE Emt_NickName
                                END AS [Employee Name]
                             , Ell_EmployeeId
                             , CASE WHEN (@DSP = '1')
                                    THEN Emt_LastName + Emt_FirstName
                                    ELSE Emt_NickName
                                END AS [ForSort]
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeID = Ell_EmployeeID
                            {3}
                         WHERE Ell_ProcessDate = '{1}'
                            -- Filter Insertion -- 
                           {0}
                           {2}

                         ORDER BY 1 /* [ForSort] */", QueryFilter()
                                                    , dtpLVDate.Date.ToString("MM/dd/yyyy")
                                                    , !ddlShift.SelectedValue.Equals(string.Empty) ? string.Format(@"AND Ell_DayCode = '{0}'", ddlShift.SelectedValue.Substring(10, 4).Trim()) : string.Empty
                                                    , !hasCCLine && (GetValue(ddlCostcenter.Value).Equals("ALL")) ? "" : string.Format(@"---apsungahid added for line code access filter 20141121
                                                                                          
                                                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
								                                                                                                          FROM E_CostCenterLineMaster 
								                                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
					                                                                                                           ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                                                           AND '{0}' BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE())) ", dtpLVDate.Date.ToString("MM/dd/yyyy")));

        CommonMethods CM = new CommonMethods();
        string end = CM.FetchQuincena('f', 'e').ToString();
        if (dtpLVDate.Date > Convert.ToDateTime(end))
        {
            isLogledger = false;
            sql = string.Format(@"
                        DECLARE @DSP as bit
                        SET @DSP = (SELECT Pcm_ProcessFlag
                                      FROM T_ProcessControlMaster
                                     WHERE Pcm_ProcessId = 'DSPFULLNM')
                        DECLARE @StartDate AS datetime = '{1}'

SELECT [Employee Name]
, Emt_EmployeeId [Ell_EmployeeId]
, [ForSort]
FROM
(                          SELECT DISTINCT Emt_EmployeeId 
                             + '   -   '
                             + CASE WHEN (@DSP = '1')
                                    THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                    ELSE Emt_NickName
                                END AS [Employee Name]
                             , Emt_EmployeeId
                             , CASE WHEN (@DSP = '1')
                                    THEN Emt_LastName + Emt_FirstName
                                    ELSE Emt_NickName
                                END AS [ForSort]
, CASE WHEN Emt_WorkType <> 'REG' 
THEN
	(SELECT 
		 CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN
				Hmt_HolidayCode 
			ELSE		
			 CASE WHEN RTRIM(Cal_WorkCode) = 'R'
				THEN 'REST'
				ELSE 'REG' +  CASE WHEN LEN(RTRIM(Cal_WorkCode)) > 1
								THEN RIGHT(RTRIM(Cal_WorkCode), 1)
								ELSE ''
								END
				END 
			END [DayCode]
		
	FROM T_CalendarGroupTmp
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = Cal_ProcessDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE Emt_LocationCode--@LOCATIONCODE
									END
	
	WHERE Cal_WorkType = Emt_WorkType--@WORKTYPE
	AND Cal_WorkGroup = Emt_WorkGroup--@WORKGROUP
	AND Cal_ProcessDate BETWEEN @StartDate AND @StartDate)
ELSE
	(SELECT 
		 CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = Emt_EmployeeID--@Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = Emt_EmployeeID--@Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN 
				CASE WHEN Hmt_HolidayDate IS NOT NULL
					THEN 
						Hmt_HolidayCode
					ELSE
						'REST'
				END
			ELSE
				'REG'
			END [DayCode]
		
	FROM dbo.GetCalendarDates(@StartDate, @StartDate) CAL
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = CAL.CalendarDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE Emt_LocationCode--@LOCATIONCODE
									END)
END [DayCode]
                          FROM T_EmployeeMaster
                          {3}
                         WHERE 1=1
                            AND Emt_JobStatus like 'A%'
                            -- Filter Insertion -- 
                           {0}
)EMP
WHERE 1=1
{2}
                         ORDER BY 1 /* [ForSort] */", QueryFilter()
                                                , dtpLVDate.Date.ToString("MM/dd/yyyy")
                                                , !ddlShift.SelectedValue.Equals(string.Empty) ? string.Format(@"AND DayCode = '{0}'", ddlShift.SelectedValue.Substring(10, 4).Trim()) : string.Empty
                                                , !hasCCLine && (GetValue(ddlCostcenter.Value).Equals("ALL")) ? "" : string.Format(@"---apsungahid added for line code access filter 20141121
                                                                                          
                                                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
								                                                                                                          FROM E_CostCenterLineMaster 
								                                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
					                                                                                                           ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                                                           AND '{0}' BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE())) ", dtpLVDate.Date.ToString("MM/dd/yyyy")));
        }

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception("No employees retrieved");
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                MessageBox.Show("No employees retrieved");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        lbxChoice.Items.Clear();
        if (txtSearch.Text == "" && hfddlChange.Value == "1")
        {
            lbxInclude.Items.Clear();
            hfddlChange.Value = "";
        }
        for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
        {
            lbxChoice.Items.Add(new ListItem(dt.Rows[ctr]["Employee Name"].ToString(), dt.Rows[ctr]["Ell_EmployeeId"].ToString()));
        }
        lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
    }

    protected string QueryFilter()
    {
        string filter = string.Empty;
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        if (!ddlGroup.SelectedValue.Equals(string.Empty))
        {
            if (isLogledger == false)
            {

                filter += string.Format(@"AND ( REPLICATE(' ', 3 - LEN(Emt_WorkType)) + RTRIM(Emt_WorkType)
                                          + REPLICATE(' ', 3 - LEN(Emt_WorkGroup)) + RTRIM(Emt_WorkGroup)) = '{0}' ", ddlGroup.SelectedValue);
            }
            else
            {
                filter += string.Format(@"AND ( REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                                          + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)) = '{0}' ", ddlGroup.SelectedValue);
            }
        }
        if (!ddlShift.SelectedValue.Equals(string.Empty))
        {
            if (isLogledger == false)
            {
                filter += string.Format(@"AND Emt_ShiftCode = '{0}'", ddlShift.SelectedValue.Substring(0, 10).Trim());
            }
            else
            {
                filter += string.Format(@"AND Ell_ShiftCode = '{0}'", ddlShift.SelectedValue.Substring(0, 10).Trim());
            }
        }
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_LastName like '{0}%'
                                           OR Emt_FirstName like '{0}%'
                                           OR Emt_NickName like '{0}%'
                                           OR Emt_EmployeeId like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
        }
        if (GetValue(ddlCostcenter.Value).Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
            {
                filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = 'LEAVE')
                                                                        OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }

            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND ( ISNULL(Ecm_LineCode, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Ecm_LineCode, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE (Ucl_CostCenterCode = Emt_CostCenterCode OR Ucl_CostCenterCode = 'ALL')
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'LEAVE'
																															 AND Ucl_LineCode = ISNULL(Ecm_LineCode, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'LEAVE'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                          OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        else if(ddlCostcenter.SelectedItem != null)
        {
            filter += string.Format(@"AND RTRIM(Emt_CostCenterCode) + '-' + ISNULL(Ecm_LineCode,'') = '{0}'", ddlCostcenter.SelectedItem.Value);



            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND ( ISNULL(Ecm_LineCode, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Ecm_LineCode, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'LEAVE'
																															 AND Ucl_LineCode = ISNULL(Ecm_LineCode, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'LEAVE'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                        OR 'ALL' IN ( SELECT Ucl_LineCode 
										                FROM E_UserCostcenterLineAccess 
													    WHERE Ucl_CostCenterCode = 'ALL'
														AND Ucl_Status = 'A'
														AND Ucl_SystemID = 'LEAVE'
														AND Ucl_LineCode = 'ALL'
														AND Ucl_UserCode = '{0}')
                                          OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }



        return filter;
    }

    private string GetValue(object objValue)
    {
        return (objValue == null) ? string.Empty : objValue.ToString();
    }

    private void showOptionalFields()
    {
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Elt_Filler%'";
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
        }
    }

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
        catch (Exception er)
        {
            throw er;
        }
        return dtStart;
    }

    protected string FormatForInQuery(string[] array)
    {
        string retVal = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
            {
                retVal += ",";
            }
            retVal += String.Format("'{0}'", array[i]);
        }
        return retVal;
    }

    private void disableControlsOnView()
    {
        System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
        img = (System.Web.UI.HtmlControls.HtmlImage)dtpLVDate.Controls[2];
        if (pnlBound.Visible)
        {
            img.Attributes.Remove("disabled");
            ddlGroup.Attributes.Remove("disabled");
            ddlShift.Attributes.Remove("disabled");
            ddlCostcenter.Attributes.Remove("disabled");
            btnFiller1.Attributes.Remove("disabled");
            btnFiller2.Attributes.Remove("disabled");
            btnFiller3.Attributes.Remove("disabled");
            //ddlType.Attributes.Remove("disabled");
            ddlType.Enabled = true;
            txtLVStartTime.Attributes.Remove("readOnly"); ;
            //txtOTHours.Attributes.Remove("readOnly");
            txtReason.Attributes.Remove("readOnly");
        }
        else
        {
            img.Attributes.Remove("disabled");
            img.Attributes.Add("disabled", "true");
            ddlGroup.Attributes.Remove("disabled");
            ddlGroup.Attributes.Add("disabled", "true");
            ddlShift.Attributes.Remove("disabled");
            ddlShift.Attributes.Add("disabled", "true");
            ddlCostcenter.Attributes.Remove("disabled");
            ddlCostcenter.Attributes.Add("disabled", "true");
            btnFiller1.Attributes.Remove("disabled");
            btnFiller1.Attributes.Add("disabled", "true");
            btnFiller2.Attributes.Remove("disabled");
            btnFiller2.Attributes.Add("disabled", "true");
            btnFiller3.Attributes.Remove("disabled");
            btnFiller3.Attributes.Add("disabled", "true");
            //ddlType.Attributes.Remove("disabled");
            //ddlType.Attributes.Add("disabled", "true");
            ddlType.Enabled = false;
            txtLVStartTime.Attributes.Remove("readOnly");
            txtLVStartTime.Attributes.Add("readOnly", "true");
            //txtOTHours.Attributes.Remove("readOnly");
            //txtOTHours.Attributes.Add("readOnly", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
        }
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
        DataTable dt = new DataTable();
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
    private int getHoliday(string processDate, string employeeID)
    {
        string curYear = DateTime.Now.Year.ToString();
        int paramValue = 0;
        string query = string.Format(@"Select Distinct Convert(varchar,Hmt_HolidayDate,101) as [HolidayDate]
                        from T_HolidayMaster
                        left join T_EmployeeMaster
					    ON (Hmt_ApplicCity= Emt_LocationCode OR Hmt_ApplicCity = 'ALL')
                        where Right(Convert(varchar,Hmt_HolidayDate, 101),4) like RTRIM('%{0}')
                        and Hmt_ApplicCity in (Emt_LocationCode, 'ALL')
						and Emt_EmployeeID = '{1}'
                        and Hmt_HolidayDate= Convert(varchar, '{2}', 101)
                        ", curYear, employeeID, processDate);

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

    private int getRestDay(DateTime processDate, string employeeID)
    {
        int paramValue = 0;
        string query = string.Format(@"Select TOP 1 [Erd_RestDay] as [RestDay]
                        from T_EmployeeRestDay
                        where [Erd_EmployeeID] ='{0}'
                        order by Ludatetime desc ", employeeID);

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

    private bool isInLogledger(string processDate, string employeeID)
    {
        bool inlogLedger = false;
        string query = string.Format(@"SELECT CONVERT(varchar,Ell_ProcessDate, 101) as [Process Date]
                        FROM T_EmployeeLogLedger
                        WHERE Ell_EmployeeId='{0}'
                        AND CONVERT(varchar,Ell_ProcessDate, 101)= '{1}'", employeeID, processDate);
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

    private int getLogLedger(string processDate, string employeeID)
    {
        int paramValue = 0;
        string query = string.Format(@"Select Convert(varchar,Ell_ProcessDate, 101) as [Process Date]
                                ,Ell_DayCode as [Day Code]
                        from T_EmployeeLogLedger
                        where Ell_EmployeeId='{0}'
                        and Ell_RestDay=0 and Ell_Holiday=0
                        and Convert(varchar,Ell_ProcessDate, 101)= '{1}'", employeeID, processDate);
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
