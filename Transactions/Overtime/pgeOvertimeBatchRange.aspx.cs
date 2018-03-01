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

public partial class Transactions_Overtime_pgeOvertimeBatchRange : System.Web.UI.Page
{
    private readonly OvertimeBL OTBL = new OvertimeBL();
    private readonly MenuGrant MGBL = new MenuGrant();
    private readonly CommonMethods methods = new CommonMethods();
    string[] noData = null;
    private DataTable dtErrors
    {
        set
        {
            this.ViewState["error"] = value;
        }
        get
        {
            return this.ViewState["error"] == null
                    ? null
                    : (DataTable)this.ViewState["error"];
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string s = txtOTHours.Text;
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBOTENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                if (!hfPrevOTDate.Value.Equals(dtpOTDate.Date.ToShortDateString()))
                {
                    dtpOTDate_Change(dtpOTDate, new EventArgs());
                    hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
                }
                if (!hfPrevToOTDate.Value.Equals(dtpToOTDate.Date.ToShortDateString()))
                {
                    dtpOTDate_Change(dtpOTDate, new EventArgs());
                    hfPrevToOTDate.Value = dtpToOTDate.Date.ToShortDateString();
                }

            }
            LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeIndividual_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Overtime_pgeOvertimeIndividual_LoadComplete(object sender, EventArgs e)
    {
        const string jsname = "overtimeScripts";
        const string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtOTEndTime.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpOTDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpOTDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");

        TextBox txtTemp2 = (TextBox)dtpToOTDate.Controls[0];
        txtTemp2.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal2 = new System.Web.UI.WebControls.Calendar();
        cal2 = (System.Web.UI.WebControls.Calendar)dtpToOTDate.Controls[3];
        cal2.Attributes.Add("OnClick", "javascript:__doPostBack();");
        txtOTStartTime.Attributes.Add("OnKeyUp", "javascript:formatStartTime()");
        txtOTStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtOTStartTime.Attributes.Add("OnChange", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnKeyUp", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnChange", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
        txtReason.Attributes.Add("OnKeyUp", "javascript:isMaxLengthAfterKeyPress('txtReason',199);");

        disableControlsOnView();
    }
    //ROBERT ADDED to set the start time correctly. Depending on the day code 09122013
    public void SetStartTime()
    {
        if (hfDayCode.Value.ToString().Contains("REG"))
        {
            decimal otStartPad = MethodsLibrary.Methods.GetParameterValue("OTSTARTPAD");
            int o2MIN = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
            txtOTStartTime.Text = (Convert.ToInt32(o2MIN + otStartPad) / 60).ToString().PadLeft(2, '0')
                                + ":"
                                + (Convert.ToInt32(o2MIN + otStartPad) % 60).ToString().PadLeft(2, '0');
            //txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
        }
        else
        {

            txtOTStartTime.Text = hfI1.Value.Insert(2, ":");

        }
    }
    protected void dtpOTDate_Change(object sender, EventArgs e)
    {
        ddlGroup.Items.Clear();
        ddlShift.Items.Clear();
        ddlCostcenter.Items.Clear();
        lbxChoice.Items.Clear();

        lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";

        if (dtpOTDate.Date > dtpToOTDate.Date)
        {
            MessageBox.Show("From Date must be less than or equal to To Date.");
        }
        else
        {
            fillddlGroup();
            dgvGenerated.DataSource = new DataTable("dummy");
            dgvGenerated.DataBind();
            lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
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
            }
        }
    }

    protected void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        fillDDLShift();
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
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
    protected void chbAll_Click(object sender, EventArgs e)
    {
        if (chbAll.Checked)
        {
            ddlCostcenter.Enabled = false;
            ddlGroup.Enabled = false;
            ddlShift.Enabled = false;
        }
        else
        {
            ddlCostcenter.Enabled = true;
            ddlGroup.Enabled = true;
            ddlShift.Enabled = true;
        }
        txtSearch_TextChanged(null, null);
    }
    protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
    {
        initializeShift();
        fillddlCostcenter();
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
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
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
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

            dgvGenerated.DataSource = new DataTable("dummy");
            dgvGenerated.DataBind();
            lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
            if (hfEmployeeRemove.Value.Contains(lbxChoice.Items[lbxChoice.SelectedIndex].Value) && btnSave.Text == "UPDATE")
                hfEmployeeRemove.Value = hfEmployeeRemove.Value.Replace(lbxChoice.Items[lbxChoice.SelectedIndex].Value + ",", "");
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
            btnSave.Enabled = true;

        }
    }

    protected void btnRemoveIndi_Click(object sender, EventArgs e)
    {
        if (lbxInclude.SelectedIndex > -1)
        {
            dgvGenerated.DataSource = new DataTable("dummy");
            dgvGenerated.DataBind();
            lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
            if (btnSave.Text == "UPDATE")
                hfEmployeeRemove.Value += lbxInclude.Items[lbxInclude.SelectedIndex].Value + ",";

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
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
        while (lbxChoice.Items.Count > 0)
        {
            if (btnSave.Text == "UPDATE" && hfEmployeeRemove.Value.Contains(lbxChoice.Items[lbxChoice.SelectedIndex].Value))
                hfEmployeeRemove.Value = hfEmployeeRemove.Value.Replace(lbxChoice.Items[0].Value + ",", "");

            lbxInclude.Items.Add(lbxChoice.Items[0]);
            lbxChoice.Items.Remove(lbxChoice.Items[0]);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
        }
        //btnEndorse.Enabled = false;
        btnSave.Enabled = true;
    }

    protected void btnRemaoveAll_Click(object sender, EventArgs e)
    {
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
        while (lbxInclude.Items.Count > 0)
        {
            if (btnSave.Text == "UPDATE")
                hfEmployeeRemove.Value += lbxInclude.Items[0].Value + ",";

            lbxChoice.Items.Add(lbxInclude.Items[0]);
            lbxInclude.Items.Remove(lbxInclude.Items[0]);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " employee(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " employee(s)";
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (CheckChangesEntry() || hfSaveOrEndorse.Value == "")
        {
            if (dgvGenerated.Rows.Count > 0)
            {
                hfSaveOrEndorse.Value = "Save";
                this.Process("S");
            }
            else
            {
                MessageBox.Show("Generate Overtime before saving.");
            }
        }
        else
        {
            MessageBox.Show("Generate Overtime. You have changes.");
        }

    }
    public bool CheckChangesEntry()
    {
        //return (hf)
        return (Convert.ToDateTime(hfPrevOTDate.Value.ToString()).ToString("MM/dd/yyyy") == dtpOTDate.Date.ToString("MM/dd/yyyy") &&
                Convert.ToDateTime(hfPrevToOTDate.Value.ToString()).ToString("MM/dd/yyyy") == dtpToOTDate.Date.ToString("MM/dd/yyyy") &&
                hfWorkGroup.Value == ddlGroup.SelectedValue.ToString() &&
                hfShift.Value == ddlShift.SelectedValue.ToString() &&
                hfOTtype.Value == ddlType.SelectedValue.ToString() &&
                hfHours.Value == txtOTHours.Text &&
                hfEndTime.Value == txtOTEndTime.Text &&
                hfStartTime.Value == txtOTStartTime.Text);
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        restoreDefaultControls();
        initializeControls();
    }
    #endregion

    #region Methods
    #region Perth Added 12/27/2012 Consolidate Checking

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        CheckAll();
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
    private void Process(string TransactionType)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                  || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
                {
                    #region Get Included Employees and Dates
                    string strEmployees = "";
                    for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                    {
                        strEmployees += lbxInclude.Items[ctr].Value + ",";
                    }
                    string strDates = "";
                    DateTime dtDate = dtpOTDate.Date;
                    while (dtDate <= dtpToOTDate.Date)
                    {
                        strDates += string.Format("{0:MM/dd/yyyy},", dtDate);
                        dtDate = dtDate.AddDays(1);
                    }
                    #endregion

                    #region Get Excluded Employees and Dates
                    string strExcluded = "";
                    for (int i = 0; i < dgvGenerated.Rows.Count; i++)
                    {
                        CheckBox tempCB = (CheckBox)dgvGenerated.Rows[i].Cells[0].FindControl("chkBox");
                        bool isChecked = tempCB.Checked;
                        if (isChecked == false)
                        {
                            strExcluded += dgvGenerated.Rows[i].Cells[1].Text + "|" + dgvGenerated.Rows[i].Cells[3].Text + ",";
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
                            string batchNo = ""; //hfBatch.Value.ToString(); //Not going to reuse batch number to update transactions

                            string filler1 = string.Empty;
                            string filler2 = string.Empty;
                            string filler3 = string.Empty;
                            if (txtFiller1.Text != string.Empty)
                            {
                                DataSet dsFiller1 = getFillerCode(txtFiller1.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller1.Text + "'", "Eot_Filler01"));
                                if (dsFiller1.Tables[0].Rows.Count > 0)
                                    filler1 = dsFiller1.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                            }
                            if (txtFiller2.Text != string.Empty)
                            {
                                DataSet dsFiller2 = getFillerCode(txtFiller2.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller2.Text + "'", "Eot_Filler02"));
                                if (dsFiller2.Tables[0].Rows.Count > 0)
                                    filler2 = dsFiller2.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                            }
                            if (txtFiller3.Text != string.Empty)
                            {
                                DataSet dsFiller3 = getFillerCode(txtFiller3.Text, getColumnFillerLookUp("Adt_AccountDesc ='" + txtFiller3.Text + "'", "Eot_Filler03"));
                                if (dsFiller3.Tables[0].Rows.Count > 0)
                                    filler3 = dsFiller3.Tables[0].Rows[0]["Adt_AccountCode"].ToString();
                            }

                            string strCreateBatchOTQuery = string.Format("EXEC CreateBatchOvertimeRecord '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}' "
                                                                        , batchNo
                                                                        , strEmployees
                                                                        , strDates
                                                                        , ddlType.SelectedValue
                                                                        , txtOTStartTime.Text.ToString().Replace(":", "")
                                                                        , txtOTEndTime.Text.ToString().Replace(":", "")
                                                                        , txtOTHours.Text
                                                                        , txtReason.Text
                                                                        , "" //Next level status depends on approval route
                                                                        , Session["userLogged"].ToString()
                                                                        , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                        , "WFBOTENTRY"
                                                                        , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION)
                                                                        , ""
                                                                        , ""
                                                                        , filler1
                                                                        , filler2
                                                                        , filler3
                                                                        , strExcluded);
                            DataTable dtResult = dal.ExecuteDataSet(strCreateBatchOTQuery).Tables[0];

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

                            //Must have at least one successfully saved OT record
                            if (iCountSuccess > 0)
                            {
                                dal.CommitTransactionSnapshot();
                                SystemMenuLogBL.InsertAddLog("WFBOTENTRY", true, "", Session["userLogged"].ToString(), "", false);
                            }
                            else
                            {
                                dal.RollBackTransactionSnapshot();
                                SystemMenuLogBL.InsertAddLog("WFBOTENTRY", false, "", Session["userLogged"].ToString(), "", false);
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

                    hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
                    hfPrevToOTDate.Value = dtpToOTDate.Date.ToShortDateString();
                    hfWorkGroup.Value = ddlGroup.SelectedValue.ToString();
                    hfShift.Value = ddlShift.SelectedValue.ToString();
                    hfOTtype.Value = ddlType.SelectedValue.ToString();
                    hfHours.Value = txtOTHours.Text;
                    hfEndTime.Value = txtOTEndTime.Text;
                    hfStartTime.Value = txtOTStartTime.Text;
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
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
        dtpOTDate.Date = DateTime.Now;
        dtpToOTDate.Date = DateTime.Now;
        //Default Parameter
        //dtpOTDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
        //Andre added for use MINPASTPRD paramter
        dtpOTDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpOTDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        dtpToOTDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //END
        dtpToOTDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
        hfPrevToOTDate.Value = dtpToOTDate.Date.ToShortDateString();
        OTBL.initializeOTTypes(ddlType, false);
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
        fillddlGroup();
        chbAll.Visible = Boolean.Parse(Resources.Resource.ALLOWFLEX.ToString());
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
        //btnEndorse.Enabled = false;
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
            this.txtOTHours.Text = string.Empty;
            this.txtOTEndTime.Text = string.Empty;
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            MessageBox.Show("No shift retrieved for this date.");
        }
    }

    private void restoreDefaultControls()
    {
        txtOTStartTime.Text = string.Empty;
        txtOTHours.Text = string.Empty;
        txtOTEndTime.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        hfPrevOTDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        hfBatch.Value = string.Empty;
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
        lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
        initializeControls();
    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {//robert added allowflow
        //if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)||!methods.GetProcessControlFlag("OVERTIME", "CUT-OFF"))
        if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy"), dtpToOTDate.Date.ToString("MM/dd/yyyy")))
            {

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
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            {
                                if (hfBatch.Value != null && hfBatch.Value.ToString().Trim() != "")
                                    //delete all batch previous batch entry
                                    DeletePrevBatchEntry(dal);
                                DataSet dsOvertimeDates = getOvertimeTransactionWithDetailBatchRange(employees, dtpOTDate.Date.ToString("MM/dd/yyyy"), dtpToOTDate.Date.ToString("MM/dd/yyyy"), ddlType.SelectedValue, txtOTHours.Text, txtOTStartTime.Text);

                                if (!CommonMethods.isEmpty(dsOvertimeDates))
                                {
                                    dgvGenerated.DataSource = dsOvertimeDates;
                                    dgvGenerated.DataBind();
                                    lblNoOfTransGen.Text = dgvGenerated.Rows.Count + " transaction(s)";
                                    btnSave.Enabled = true;
                                    hfSaveOrEndorse.Value = "";
                                }
                                else
                                {
                                    MessageBox.Show("Unable to create transactions between " + dtpOTDate.Date.ToString("MM/dd/yyyy") + " - " + dtpToOTDate.Date.ToString("MM/dd/yyyy") + "\nThere may be existing transactions.");
                                }
                            }
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
                }
                else
                {
                    MessageBox.Show(errMsg1);
                }
            }
            else
            {
                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
            }
        }
        else
        {
            MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
        }

    }

    public void DeletePrevBatchEntry(DALHelper dal)
    {
        string query = string.Format(@"DELETE FROM T_EMPLOYEEOVERTIME
                        WHERE Eot_BatchNo='{0}'", hfBatch.Value.ToString().Trim());
        dal.ExecuteNonQuery(query);
    }
    public DataSet getOvertimeTransactionWithDetailBatchRange(string[] employees, string fromMMDDYYY, string toMMDDYYYY, string type, string hours)
    {
        return getOvertimeTransactionWithDetailBatchRange(employees, fromMMDDYYY, toMMDDYYYY, type, hours, "");
    }
    public DataSet getOvertimeTransactionWithDetailBatchRange(string[] employees, string fromMMDDYYY, string toMMDDYYYY, string type, string hours, string startTime)
    {

        noData = new string[lbxInclude.Items.Count];
        DataSet ds = new DataSet();
        ds.Tables.Add(new DataTable());
        ds.Tables[0].Columns.Add("Employee ID");
        ds.Tables[0].Columns.Add("Employee Name");
        ds.Tables[0].Columns.Add("OT Date");
        ds.Tables[0].Columns.Add("DoW");
        ds.Tables[0].Columns.Add("Day Code");
        ds.Tables[0].Columns.Add("Shift Code");
        ds.Tables[0].Columns.Add("Shift Desc");
        ds.Tables[0].Columns.Add("Start");
        ds.Tables[0].Columns.Add("End");
        ds.Tables[0].Columns.Add("Hours");

        DataSet dsDetails = getOvertimeTransactionWithDetail(employees, fromMMDDYYY, toMMDDYYYY, type, hours);
        if (dsDetails != null && dsDetails.Tables != null && dsDetails.Tables.Count > 0 && dsDetails.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].NewRow();
            //if (ds == null || ds.Tables == null || ds.Tables.Count<=0|| ds.Tables[0].Rows.Count <= 0)
            //{
            // ds.Tables.Add(dsDetails.Tables[0].Copy());
            //}
            //else
            //{

            foreach (DataRow drDetail in dsDetails.Tables[0].Rows)
            {
                dr["Employee ID"] = drDetail["Emp ID"]; ;
                dr["Employee Name"] = drDetail["Emp Name"];
                dr["OT Date"] = drDetail["OT Date"];
                dr["DoW"] = drDetail["DoW"];
                dr["Day Code"] = drDetail["Day Code"];
                dr["Shift Code"] = drDetail["Shift Code"];
                dr["Shift Desc"] = drDetail["Shift Desc"];
                dr["Start"] = startTime != "" ? startTime : drDetail["Start"];
                dr["End"] = txtOTEndTime.Text != "" ? txtOTEndTime.Text : drDetail["End"];
                dr["Hours"] = drDetail["Hours"];
                ds.Tables[0].Rows.Add(dr.ItemArray);
            }

            //}
        }
        return ds;
    }
    public DataSet getOvertimeTransactionWithDetail(string[] empId, string fromMMDDYYY, string toMMDDYYYY, string type, string hours)
    {
        DataSet ds = new DataSet();
        #region Query
        string sql = string.Format(@" 
DECLARE @EMPTABLE AS TABLE
													   (EmpID varchar(15))
													   INSERT INTO @EMPTABLE
													   SELECT Emt_EmployeeID FROM T_EmployeeMaster
													   WHERE Emt_EmployeeID IN ({0})

SELECT Emt_EmployeeID [Emp ID]
, Emt_LastName + ', ' + Emt_FirstName [Emp Name]
                                 , Convert(varchar(10), Ell_ProcessDate, 101) [OT Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , Ell_ShiftCode [Shift Code]
                                 , '[' + Scm_ShiftCode + '] '
                                 + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc] 
                                 , CASE WHEN (@type = 'P')
                                        THEN CASE WHEN (Ell_DayCode = 'REG')
				                                  THEN LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2)
				                                  ELSE LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
			                                  END
                                        ELSE CASE WHEN (@type = 'A') 
                                                  THEN dbo.computeOTTime(Scm_ShiftTimeIn, (Convert(decimal(7,2),@hours)*-1), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
                                                  ELSE LEFT(Scm_ShiftBreakStart,2) +':'+ RIGHT(Scm_ShiftBreakStart, 2)
                                              END
                                    END [Start]
                                 , CASE WHEN (@type = 'P')
		                                THEN CASE WHEN (Ell_DayCode = 'REG')
		 		                                  THEN dbo.computeOTTime(Scm_ShiftTimeOut, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
				                                  ELSE dbo.computeOTTime(Scm_ShiftTimeIn, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
			                                  END
		                                ELSE CASE WHEN (@type = 'A') 
                                                  THEN LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
                                                  ELSE dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0)
                                                  --ELSE LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2)
                                              END
	                                 END [End]
                                 , Convert(decimal(7,2),@hours) [Hours]
                              FROM T_EmployeeLogLedger
                                INNER JOIN @EMPTABLE
                                    ON Ell_EmployeeId = EmpID     
                                    AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)                        
                                INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                              -- AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType 
								--										                                     FROM T_EmployeeOvertime
								--										                                    WHERE Eot_EmployeeId = @employeeId
								--										                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--										                                      AND Eot_Status IN ('1','3','5','7','9')
								--										                                    UNION
								--										                                   SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--										                                     FROM T_EmployeeOvertimeHist
								--										                                    WHERE Eot_EmployeeId = @employeeId
								--										                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--										                                      AND Eot_Status IN ('1','3','5','7','9')
--)

                             UNION

                            SELECT 
Emt_EmployeeID [Emp ID]
, Emt_LastName + ', ' + Emt_FirstName [Emp Name]
                                 , Convert(varchar(10), Ell_ProcessDate, 101) [OT Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , Ell_ShiftCode [Shift Code]
                                 , '[' + Scm_ShiftCode + '] '
                                 + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc] 
                                 , CASE WHEN (@type = 'P')
                                        THEN CASE WHEN (Ell_DayCode = 'REG')
				                                  THEN LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2)
				                                  ELSE LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
			                                  END
                                        ELSE CASE WHEN (@type = 'A') 
                                                  THEN dbo.computeOTTime(Scm_ShiftTimeIn, (Convert(decimal(7,2),@hours)*-1), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
                                                  ELSE LEFT(Scm_ShiftBreakStart,2) +':'+ RIGHT(Scm_ShiftBreakStart, 2)
                                              END
                                    END [Start]
                                 , CASE WHEN (@type = 'P')
		                                THEN CASE WHEN (Ell_DayCode = 'REG')
		 		                                  THEN dbo.computeOTTime(Scm_ShiftTimeOut, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
				                                  ELSE dbo.computeOTTime(Scm_ShiftTimeIn, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
			                                  END
		                                ELSE CASE WHEN (@type = 'A') 
                                                  THEN LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
                                                  ELSE dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0)
                                                  --ELSE LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2)
                                              END
	                                 END [End]
                                 , Convert(decimal(7,2),@hours) [Hours]
                              FROM T_EmployeeLogLedgerHist
                            INNER JOIN @EMPTABLE
                                ON Ell_EmployeeId = EmpID     
                                AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate) 
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                            Order by [Emp Name], [OT Date]
                              -- AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--									                                         FROM T_EmployeeOvertime
								--									                                        WHERE Eot_EmployeeId = @employeeId
								--									                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--									                                          AND Eot_Status IN ('1','3','5','7','9')
								--									                                        UNION
								--									                                       SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--									                                         FROM T_EmployeeOvertimeHist
								--									                                        WHERE Eot_EmployeeId = @employeeId
								--									                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--									                                          AND Eot_Status IN ('1','3','5','7','9'))", FormatForInQuery(empId));
        #endregion
        ParameterInfo[] param = new ParameterInfo[4];
        param[0] = new ParameterInfo("@fromDate", fromMMDDYYY);
        param[1] = new ParameterInfo("@toDate", toMMDDYYYY);
        param[2] = new ParameterInfo("@type", type);
        param[3] = new ParameterInfo("@hours", hours);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
            }
            catch (Exception ex)
            {
                //CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    private void fillddlGroup()
    {
        DataTable dt = new DataTable();
        string sql = @" SELECT DISTINCT ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Desc]
                             , REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup) [Code]
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_AccountDetail as AD1
                            ON AD1.Adt_AccountType = 'WORKTYPE' 
                           AND AD1.Adt_AccountCode = Ell_WorkType
                         INNER JOIN T_AccountDetail as AD2
                            ON AD2.Adt_AccountType = 'WORKGROUP'
                           AND AD2.Adt_AccountCode = Ell_WorkGroup
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         WHERE Ell_ProcessDate between @ProcessDate and @ProcessToDate
                           {0}

                         UNION

                        SELECT DISTINCT ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Desc]
                             , REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup) [Code]
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_AccountDetail as AD1
                            ON AD1.Adt_AccountType = 'WORKTYPE' 
                           AND AD1.Adt_AccountCode = Ell_WorkType
                         INNER JOIN T_AccountDetail as AD2
                            ON AD2.Adt_AccountType = 'WORKGROUP'
                           AND AD2.Adt_AccountCode = Ell_WorkGroup
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         WHERE Ell_ProcessDate between @ProcessDate and @ProcessToDate
                           {0}

                         ORDER BY [Code]";

        string filter;
        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
            filter = @"AND Emt_CostcenterCode IN (SELECT Uca_CostcenterCode
                                                            FROM T_UserCostcenterAccess
                                                           WHERE Uca_UserCode = @UserCode
                                                             AND Uca_SytemID = @TransactionType)";
        else
            filter = string.Empty;
        ParameterInfo[] param = new ParameterInfo[4];
        param[0] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[2] = new ParameterInfo("@TransactionType", "OVERTIME");
        param[3] = new ParameterInfo("@ProcessToDate", dtpToOTDate.Date.ToString("MM/dd/yyyy"));

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
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                             + REPLICATE(' ' , 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)  = @workTYPGRP
                           AND Ell_ProcessDate between @ProcessDate and @ProcessToDate

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
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType) 
                             + REPLICATE(' ' , 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)  = @workTYPGRP
                           AND Ell_ProcessDate between @ProcessDate and @ProcessToDate

                         ORDER BY [Code]";
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            if (dtpOTDate.Date > DateTime.Now)
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
            if (dtpOTDate.Date > Convert.ToDateTime(end) || dtpToOTDate.Date > Convert.ToDateTime(end))
            {
                sql = @" SELECT REPLICATE(' ' , 10 - LEN(Scm_ShiftCode)) + Scm_ShiftCode
	                          + REPLICATE(' ' , 4 - LEN('REG')) + 'REG' [Code]
	                          , Scm_ShiftDesc [Desc]
	                          , Scm_ShiftCode [ShiftCode]
	                          , 'REG' + REPLICATE(' ' , 4 - LEN('REG')) [DayCode]
	                          , LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2) [TimeIn]
	                          , LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2) [BreakStart]
	                          , LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2) [BreakEnd]
	                          , LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [TimeOut]
	                       FROM T_EmployeeMaster
						   LEFT JOIN T_ShiftCodeMaster
 							 ON Scm_ShiftCode = Emt_ShiftCode
	                      WHERE Emt_EmployeeId = '{0}'";
            }
        }
        ParameterInfo[] param = new ParameterInfo[3];
        param[0] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@workTYPGRP", ddlGroup.SelectedValue);
        param[2] = new ParameterInfo("@ProcessToDate", dtpToOTDate.Date.ToString("MM/dd/yyyy"));

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
            string sql = @" SELECT DISTINCT RTRIM(Emt_CostcenterCode) + '-' + ISNULL(Ecm_LineCode,'')  [Code]
	                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) +	CASE WHEN Ecm_LineCode IS NULL THEN '' ELSE + ' - LINE ' + Ecm_LineCode END  [Costcenter]
                            FROM (
	                            SELECT Ell_EmployeeId 
	                            FROM T_EmployeeLogLedger
	                            WHERE Ell_ProcessDate = @ProcessDate
		                            AND Ell_WorkType = @WorkType
		                            AND Ell_WorkGroup = @WorkGroup
		                            AND Ell_ShiftCode = @ShiftCode
	                            UNION
	                            SELECT Ell_EmployeeId 
	                            FROM T_EmployeeLogLedgerHist
	                            WHERE Ell_ProcessDate = @ProcessDate
		                            AND Ell_WorkType = @WorkType
		                            AND Ell_WorkGroup = @WorkGroup
		                            AND Ell_ShiftCode = @ShiftCode
                            ) LOGLEDGER
                            INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeID
                            LEFT JOIN (SELECT * FROM dbo.GetAllLatestCostCenterLines(@ProcessDate, DATEADD(month, 5, GETDATE()))) EmployeeCostCenterLineMovement	
                            ON Ecm_EmployeeID = Ell_EmployeeID
                            WHERE LEFT(Emt_JobStatus,1) = 'A'
	                            AND (Emt_CostCenterCode IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess(@UserCode, 'OVERTIME') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
		                            OR Emt_CostCenterCode + ISNULL(Ecm_LineCode,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess(@UserCode, 'OVERTIME')))
                            ORDER BY [Costcenter] ";
            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString(), SqlDbType.VarChar, 15);
            param[1] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"), SqlDbType.DateTime);
            param[2] = new ParameterInfo("@WorkType", ddlGroup.SelectedValue.Substring(0, 3).Trim(), SqlDbType.Char, 3);
            param[3] = new ParameterInfo("@WorkGroup", ddlGroup.SelectedValue.Substring(3, 3).Trim(), SqlDbType.Char, 3);
            param[4] = new ParameterInfo("@ShiftCode", ddlShift.SelectedValue.Substring(0, 10).Trim(), SqlDbType.VarChar, 10);

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
            //if (checkCostCenter("OVERTIME"))
            //{
            //    setAllCostcenter();
            //}
        }
    }

    private void setAllCostcenter()
    {
        string sql = @" SELECT DISTINCT Uca_CostCenterCode [Code]
                             , dbo.getCostCenterFullNameV2(Uca_CostCenterCode)[Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_SytemId = 'OVERTIME' 
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
                                    FROM (
	                                    SELECT Ell_EmployeeId
	                                    FROM T_EmployeeLogLedger
	                                    WHERE Ell_ProcessDate = '{1}'
		                                    {0}
	                                    UNION
	                                    SELECT Ell_EmployeeId 
	                                    FROM T_EmployeeLogLedgerHist
	                                    WHERE Ell_ProcessDate = '{1}'
		                                    {0}
                                    ) LOGLEDGER
                                    INNER JOIN T_EmployeeMaster
                                    ON Emt_EmployeeId = Ell_EmployeeID
                                    LEFT JOIN (SELECT * FROM dbo.GetAllLatestCostCenterLines('{1}', DATEADD(month, 5, GETDATE()))) EmployeeCostCenterLineMovement	
                                    ON Ecm_EmployeeID = Ell_EmployeeID
                                    WHERE LEFT(Emt_JobStatus,1) = 'A'
	                                    {2}
	                                    AND (Emt_CostCenterCode IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{3}', 'OVERTIME') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
		                                    OR Emt_CostCenterCode + ISNULL(Ecm_LineCode,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{3}', 'OVERTIME')))
                                    ORDER BY 3", QueryDateFilter(), dtpOTDate.Date.ToString("MM/dd/yyyy"), QueryCostCenterFilter(), Session["userLogged"].ToString());

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

    protected string QueryDateFilter()
    {
        string filter = string.Empty;
        if (!chbAll.Checked)
        {
            if (!ddlGroup.SelectedValue.Equals(string.Empty))
            {
                filter += string.Format(@"AND ( REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                                          + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)) = '{0}' ", ddlGroup.SelectedValue);
            }
            if (!ddlShift.SelectedValue.Equals(string.Empty))
            {
                filter += string.Format(@"AND Ell_ShiftCode = '{0}'", ddlShift.SelectedValue.Substring(0, 10).Trim());
                filter += string.Format(@"AND Ell_DayCode = '{0}'", ddlShift.SelectedValue.Substring(10, 4).Trim());
            }
        }

        return filter;
    }

    protected string QueryCostCenterFilter()
    {
        string filter = string.Empty;
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_LastName like '{0}%'
                                           OR Emt_FirstName like '{0}%'
                                           OR Emt_NickName like '{0}%'
                                           OR Emt_EmployeeId like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
        }
        if (!chbAll.Checked)
        {
            if (ddlCostcenter.SelectedItem != null && GetValue(ddlCostcenter.Value).Equals("ALL") == false)
            {
                filter += string.Format(@"AND RTRIM(Emt_CostCenterCode) + '-' + ISNULL(Ecm_LineCode,'') = '{0}'", ddlCostcenter.SelectedItem.Value);
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
        string sql = @"DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                      FROM T_ProcessControlMaster
                                                     WHERE Pcm_SystemID = 'OVERTIME'
                                                       AND Pcm_ProcessID = 'AUTOROUTE'
                                                       AND Pcm_Status = 'A' )
                       SELECT IIF(Cfl_ColName = 'Eot_Filler01' AND @AUTOROUTE = 1, '', Cfl_ColName) Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
                          AND Cfl_Status = 'A'
                          AND 1 = IIF(Cfl_ColName <> 'Eot_Filler01', 1, IIF(@AUTOROUTE = 1, 0, 1)) ";
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
                        case "EOT_FILLER01":
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
                        case "EOT_FILLER02":
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
                        case "EOT_FILLER03":
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
        #region Start Time
        if (txtOTStartTime.Text.Length < 5)
        {
            err += "\nStart Time invalid format.(hh:mm)";
        }
        else
        {
            try
            {
                int x = Convert.ToInt32(txtOTStartTime.Text.Substring(0, 2));
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtOTStartTime.Text.Substring(3, 2));
                decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) / 60);
                temp = Math.Round(temp, 2);

                if (Convert.ToInt32(txtOTStartTime.Text.Substring(3, 2)) > 59)
                {
                    err += "\nStart Time minutes is invalid";
                }
                else if (temp > Convert.ToDecimal(71.98))
                {
                    err += "\nStart Time maximum time exceeded.(up to 71:59)";
                }
                else if (!txtOTStartTime.Text.Contains(":"))
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
        if (txtOTEndTime.Text.Length < 5)
        {
            err += "\nEnd Time invalid format.(hh:mm)";
        }
        else
        {
            try
            {
                int x = Convert.ToInt32(txtOTEndTime.Text.Substring(0, 2));
                DateTime date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:") + txtOTEndTime.Text.Substring(3, 2));
                decimal temp = Convert.ToDecimal(Convert.ToDecimal(txtOTEndTime.Text.Substring(0, 2)) + Convert.ToDecimal(txtOTEndTime.Text.Substring(3, 2)) / 60);
                temp = Math.Round(temp, 2);

                if (Convert.ToInt32(txtOTEndTime.Text.Substring(3, 2)) > 59)
                {
                    err += "\nEnd Time minutes is invalid";
                }
                else if (temp > Convert.ToDecimal(72.00))
                {
                    err += "\nEnd Time maximum time exceeded.(up to 72:00)";
                }
                else if (!txtOTEndTime.Text.Contains(":"))
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
        #region Overtime Hours
        if (Convert.ToDecimal(txtOTHours.Text) > Convert.ToDecimal(72.00))
        {
            err += "\nOvertime hours excceed maximum hours.(72.00)";
        }
        #endregion
        #region Start and End time validation after checking all format is correct.
        if (err.Equals(string.Empty))
        {
            int I1 = (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(2, 2));
            int O1 = (Convert.ToInt32(hfO1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO1.Value.Substring(2, 2));
            int I2 = (Convert.ToInt32(hfI2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI2.Value.Substring(2, 2));
            int O2 = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
            int PaidBreak = Convert.ToInt32(hfShiftPaid.Value);//Already in minutes format
            int OTStart = (Convert.ToInt32(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtOTStartTime.Text.Substring(3, 2));
            int OTEnd = (Convert.ToInt32(txtOTEndTime.Text.Substring(0, 2)) * 60) + Convert.ToInt32(txtOTEndTime.Text.Substring(3, 2));
            if (hfShiftType.Value.Equals("G"))
            {
                if (O1 < I1)
                    O1 += 1440;
                if (I2 < I1)
                    I2 += 1440;
                if (O2 < I1)
                    O2 += 1440;
                if (ddlType.SelectedValue.Equals("P"))
                {

                    //if (OTStart < I1 && !(OTEnd > 1440))
                    if (OTStart < I1)
                        OTStart += 1440;
                    if (OTEnd < I1)
                        OTEnd += 1440;
                    else
                        if (OTStart > OTEnd)
                            OTStart -= 1440;
                }
            }
            if (ddlType.SelectedValue.Equals("P"))//Validate Post Overtime
            {
                if (!chbAll.Checked)
                    if (OTStart < O2 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        err += "\nStart Time cannot be within shift in POST Overtime on regular days.";
                    }
            }
            else if (ddlType.SelectedValue.Equals("A"))//Overtime Type is Advance
            {
                //for HOGP: just removed trapping for ADVANCE filing for NON-REGULAR days
                if (!chbAll.Checked)
                    if (OTEnd > I1 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        err += "\nEnd Time cannot be greater than shift Time In for ADVANCE Overtime";
                    }
                    else if (OTStart >= I1 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        err += "\nStart Time cannot be greater or equal than shift Time In for ADVANCE Overtime";
                    }

                //Andre commented: 20101013 not applicable for HOGP
                //Andre enabled: 20110218
                if (err.Equals(string.Empty))
                {
                    if (!hfDayCode.Value.Contains("REG"))
                    {
                        err += "\nCannot file ADVANCE Overtime on non-regular days.";
                    }
                    else if (OTEnd > I1 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        if (!chbAll.Checked)
                            err += "\nEnd TIme cannot be greater than shift Time In for ADVANCE Overtime";
                    }
                    else if (OTStart >= I1 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        if (!chbAll.Checked)
                            err += "\nStart Time cannot be greater or equal than shift Time In for ADVANCE Overtime";
                    }
                }
            }
            else//Overtime Type is Mid
            {
                if (!chbAll.Checked)
                {
                    if (OTStart < O1)
                    {
                        err += "\nStart Time cannot be lesser than shift break start for MID Overtime";
                    }
                    if (OTStart >= I2)
                    {
                        err += "\nStart Time cannot be greater or equal than shift break end for MID Overtime";
                    }
                    if (OTEnd <= O1)
                    {
                        err += "\nEnd Time cannot be lesser or equal than shift break start for MID Overtime";
                    }
                    if (OTEnd > I2)
                    {
                        err += "\nEnd Time cannot be greater than shift break end for MID Overtime";
                    }
                }
            }

            if (OTStart > OTEnd && err.Equals(string.Empty))
            {
                err += "Start time cannot be greater then endtime.";
            }
        }
        #endregion
        #region Includes
        if (err.Equals(string.Empty) && lbxInclude.Items.Count <= 0)
        {
            err += "\nNo employees selected for transaction.";
        }
        #endregion
        #region Reason
        if (err.Equals(string.Empty) && txtReason.Text.Length > 200)
        {
            err += "\nReason exceeds maximum characters allowed.(" + txtReason.Text.Length.ToString() + "/200)";
        }
        #endregion
        #region OTSTRTFRAC and OTFRACTION
        if (err.Equals(string.Empty))
        {
            try
            {
                if (Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) % Convert.ToDecimal(hfSTRTOTFRAC.Value) != 0)
                    err += String.Format("\n Invalid entry in minutes. Must be divisible by {0}.", hfSTRTOTFRAC.Value);
                else if (Convert.ToDecimal(txtOTEndTime.Text.Substring(3, 2)) % Convert.ToDecimal(hfSTRTOTFRAC.Value) != 0)
                    err += String.Format("\n Invalid entry in minutes. Must be divisible by {0}.", hfSTRTOTFRAC.Value);
            }
            catch
            {
                err += "\n STRTOTFRAC value was not set up. Contact system administrator.";
            }

            try
            {
                if (Convert.ToDecimal(hfOTFRACTION.Value) != 1
                    && (Convert.ToDecimal(txtOTHours.Text) * 60) % Convert.ToDecimal(hfOTFRACTION.Value) != 0)
                {
                    err += String.Format("\n Invalid entry in overtime fraction. Must be divisible by {0}.", hfOTFRACTION.Value);
                }
            }
            catch
            {
                err += "\n OTFRACTION value was not set up. Contact system administrator.";
            }
        }

        #endregion

        #region OTSTARTPAD

        //Andre added 20130702. Determin if start time of transacton is valid. this is only applicable to POST OT REGULAR DAYS

        int o2MIN = (Convert.ToInt32(hfO2.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfO2.Value.Substring(2, 2));
        if (hfShiftType.Value.Equals("G")
            && o2MIN < (Convert.ToInt32(hfI1.Value.Substring(0, 2)) * 60) + Convert.ToInt32(hfI1.Value.Substring(2, 2))
            && (Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2)) > 1440)
        {
            o2MIN += 1440;
        }//ROBERT change the substring (2,2) to (3,2)
        if (hfDayCode.Value.StartsWith("REG")
            && ddlType.SelectedValue.Equals("P")
            && (Convert.ToDecimal(txtOTStartTime.Text.Substring(0, 2)) * 60) + Convert.ToDecimal(txtOTStartTime.Text.Substring(3, 2))
            < o2MIN + Convert.ToDecimal(hfOTSTARTPAD.Value))
        {
            if (!chbAll.Checked)
                err += String.Format("\n Invalid entry in overtime start time. Valid start time is {0}:{1} onwards.", Convert.ToInt32((o2MIN + Convert.ToDecimal(hfOTSTARTPAD.Value)) / 60), Convert.ToInt32((o2MIN + Convert.ToDecimal(hfOTSTARTPAD.Value)) % 60));
        }
        #endregion

        if (err.Equals(string.Empty))
        {
            if (MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
            {
                err += CommonMethods.GetErrorMessageForCYCCUTOFF();
            }
        }
        return err;
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
    protected string FormatForInQuery2(string[] array)
    {
        string retVal = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            retVal += String.Format("{0}", array[i]) + ",";
        }
        return retVal;
    }
    private void disableControlsOnView()
    {
        System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
        img = (System.Web.UI.HtmlControls.HtmlImage)dtpOTDate.Controls[2];
        if (pnlBound.Visible)
        {
            img.Attributes.Remove("disabled");
            ddlGroup.Attributes.Remove("disabled");
            ddlShift.Attributes.Remove("disabled");
            ddlCostcenter.Attributes.Remove("disabled");
            btnFiller1.Attributes.Remove("disabled");
            btnFiller2.Attributes.Remove("disabled");
            btnFiller3.Attributes.Remove("disabled");
            ddlType.Enabled = true;
            txtOTStartTime.Attributes.Remove("readOnly"); ;
            txtOTHours.Attributes.Remove("readOnly");
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
            ddlType.Enabled = false;
            txtOTStartTime.Attributes.Remove("readOnly");
            txtOTStartTime.Attributes.Add("readOnly", "true");
            txtOTHours.Attributes.Remove("readOnly");
            txtOTHours.Attributes.Add("readOnly", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
        }
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
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
                          AND Cfl_Status = 'A'
                        and Cfl_Lookup = (select TOP 1 Adt_AccountType
												from T_AccountDetail
                                                inner join T_ColumnFiller
												on Cfl_Lookup = Adt_AccountType
												where {0}
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
    #endregion
}
