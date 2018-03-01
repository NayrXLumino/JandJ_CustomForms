/*File revision no. W2.1.00002 */
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

public partial class Transactions_Overtime_pgeOvertimeBatch : System.Web.UI.Page
{
    private readonly OvertimeBL OTBL = new OvertimeBL();
    private readonly MenuGrant MGBL = new MenuGrant();
    private readonly CommonMethods methods = new CommonMethods();
    protected void Page_Load(object sender, EventArgs e)
    {
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
        txtOTStartTime.Attributes.Add("OnKeyUp", "javascript:formatStartTime()");
        //txtOTStartTime.Attributes.Add("OnBlur", "javascript:computeEndTime()");
        txtOTStartTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
        txtOTStartTime.Attributes.Add("OnChange", "javascript:computeEndTime()");
        txtOTHours.Attributes.Add("OnKeyUp", "javascript:computeEndTime()");
        //txtOTHours.Attributes.Add("OnBlur", "javascript:computeEndTime()");
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
            txtOTStartTime.Text = hfO2.Value.Insert(2, ":");
        }
        else
        {

            txtOTStartTime.Text = hfI1.Value.Insert(2, ":");

        }
    }
    protected void dtpOTDate_Change(object sender, EventArgs e)
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

        lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
    }

    protected void btnIncludeIndi_Click(object sender, EventArgs e)
    {
        if (lbxChoice.SelectedIndex > -1)
        {
            lbxInclude.Items.Add(lbxChoice.SelectedItem);
            lbxChoice.Items.Remove(lbxChoice.SelectedItem);
            lbxChoice.SelectedIndex = -1;
            lbxInclude.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
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

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
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

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
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

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
        }
    }

    protected void btnEndorse_Click(object sender, EventArgs e)
    {
        #region Previous Logic
        //if (Session["update"].ToString() == ViewState["update"].ToString())
        //{
        //    if (Page.IsValid)
        //    {
        //        if ( Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
        //          || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDate.Date.ToString("MM/dd/yyyy")))
        //        {
        //            string errMsg1 = checkEntry1();
        //            if (errMsg1.Equals(string.Empty))
        //            {
        //                bool isValid = true;
        //                DataTable temp = new DataTable();
        //                DataTable dtFinal = new DataTable();

        //                #region array to check individual employees
        //                string[] employees = null;
        //                employees = new string[lbxInclude.Items.Count];
        //                for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
        //                {
        //                    employees[ctr] = lbxInclude.Items[ctr].Value;
        //                }
        //                #endregion

        //                temp = checkOTConfilcts(employees);
        //                if (temp.Rows.Count > 0 && isValid)
        //                {
        //                    isValid = false;
        //                    lblErrorInfo.Text = "The following employees has conflicting previously filed overtime";
        //                    dgvReview.DataSource = temp;
        //                    dgvReview.DataBind();
        //                }
        //                else//Parameter trappings
        //                {
        //                    temp = checkParameterValid(employees);
        //                    if (temp.Rows.Count > 0 && isValid)
        //                    {
        //                        isValid = false;
        //                        lblErrorInfo.Text = "The following employees has error on parameter settings";
        //                        dgvReview.DataSource = temp;
        //                        dgvReview.DataBind();
        //                    }
        //                }
        //                if (isValid)//Route trappings
        //                {
        //                    temp = checkRoutes(employees);
        //                    if (temp.Rows.Count > 0)
        //                    {
        //                        isValid = false;
        //                        lblErrorInfo.Text = "The following employees has no/incomplete approval route setup.";
        //                        dgvReview.DataSource = temp;
        //                        dgvReview.DataBind();
        //                    }
        //                }

        //                if (!isValid)
        //                {
        //                    pnlBound.Visible = false;
        //                    pnlReview.Visible = true;
        //                    MessageBox.Show("Review the following entries.");
        //                }
        //                else
        //                {
        //                    using (DALHelper dal = new DALHelper())
        //                    {
        //                        try
        //                        {
        //                            dal.OpenDB();
        //                            dal.BeginTransactionSnapshot();
        //                            string batchNo = CommonMethods.GetControlNumber("OTBATCH");
        //                            string status = "3";//default only, still would change in loop...
        //                            int counter = 0;
        //                            string controlNumbers = string.Empty;
        //                            for (int i = 0; i < lbxInclude.Items.Count; i++)
        //                            {
        //                                status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "OVERTIME", "ENDORSE TO CHECKER 1", dal);
        //                                DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("OVERTIME"), lbxInclude.Items[i].Value.ToString(), batchNo);
        //                                OTBL.CreateOTRecord(dr, dal);
        //                                controlNumbers += dr["Eot_ControlNo"].ToString() + ",";
        //                                counter++;
        //                            }
        //                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
        //                            {
        //                                EmailNotificationBL EMBL = new EmailNotificationBL();
        //                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
        //                                EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
        //                                EMBL.InsertInfoForNotification(controlNumbers
        //                                                                , Session["userLogged"].ToString()
        //                                                                , dal);
        //                            }
        //                            restoreDefaultControls();
        //                            MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
        //                            dal.CommitTransactionSnapshot();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
        //                            dal.RollBackTransactionSnapshot();
        //                        }
        //                        finally
        //                        {
        //                            dal.CloseDB();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show(errMsg1);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
        //        }
        //    }
        //    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        //}
        //else
        //{
        //    MessageBox.Show("Page refresh is not allowed.");
        //}
        #endregion
        this.Process("E");
    }

    protected void btnDisregard_Click(object sender, EventArgs e)
    {
        #region Previous Logic
        //if (Session["update"].ToString() == ViewState["update"].ToString())
        //{
        //    if (!methods.GetProcessControlFlag("OVERTIME", "CUT-OFF"))
        //    {
        //        string[] employees = new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
        //        bool flag = true;
        //        DataTable temp = new DataTable();
        //        DataTable dtFinal = new DataTable();
        //        bool isValid = true;
        //        int indx = 0;
        //        for (int i = 0; i < lbxInclude.Items.Count; i++)
        //        {
        //            flag = true;
        //            for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
        //            {
        //                if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
        //                {
        //                    flag = false;
        //                }
        //            }
        //            if (flag)
        //            {
        //                employees[indx++] = lbxInclude.Items[i].Value.ToString();
        //            }
        //        }

        //        if (employees.Length > 0)
        //        {
        //            temp = checkOTConfilcts(employees);
        //            if (temp.Rows.Count > 0 && isValid)
        //            {
        //                isValid = false;
        //                lblErrorInfo.Text = "The following employees has conflicting previously filed overtime";
        //                dgvReview.DataSource = temp;
        //                dgvReview.DataBind();
        //            }
        //            else//Parameter trappings
        //            {
        //                temp = checkParameterValid(employees);
        //                if (temp.Rows.Count > 0 && isValid)
        //                {
        //                    isValid = false;
        //                    lblErrorInfo.Text = "The following employees has error on parameter settings";
        //                    dgvReview.DataSource = temp;
        //                    dgvReview.DataBind();
        //                }
        //            }
        //            if (isValid)//Route trappings
        //            {
        //                temp = checkRoutes(employees);
        //                if (temp.Rows.Count > 0)
        //                {
        //                    isValid = false;
        //                    lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
        //                    dgvReview.DataSource = temp;
        //                    dgvReview.DataBind();
        //                }
        //            }

        //            if (!isValid)
        //            {
        //                pnlBound.Visible = false;
        //                pnlReview.Visible = true;
        //                MessageBox.Show("Review the following entries.");
        //            }
        //            else
        //            {
        //                using (DALHelper dal = new DALHelper())
        //                {
        //                    try
        //                    {
        //                        dal.OpenDB();
        //                        dal.BeginTransactionSnapshot();
        //                        string batchNo = CommonMethods.GetControlNumber("OTBATCH");
        //                        string status = "3";//default only...
        //                        int counter = 0;
        //                        string controlNumbers = string.Empty;
        //                        for (int i = 0; i < employees.Length; i++)
        //                        {
        //                            status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "OVERTIME", "ENDORSE TO CHECKER 1", dal);
        //                            DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("OVERTIME"), employees[i], batchNo);
        //                            OTBL.CreateOTRecord(dr, dal);
        //                            controlNumbers += dr["Eot_ControlNo"].ToString() + ",";
        //                            counter++;
        //                        }
        //                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
        //                        {
        //                            EmailNotificationBL EMBL = new EmailNotificationBL();
        //                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
        //                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
        //                            EMBL.InsertInfoForNotification(controlNumbers
        //                                                            , Session["userLogged"].ToString()
        //                                                            , dal);
        //                        }
        //                        restoreDefaultControls();
        //                        MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
        //                        dal.CommitTransactionSnapshot();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
        //                        dal.RollBackTransactionSnapshot();
        //                    }
        //                    finally
        //                    {
        //                        dal.CloseDB();
        //                    }
        //                }
        //            }
        //            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        //        }
        //        else
        //        {
        //            MessageBox.Show("No selected employees to file. \n Please cancel transaction.");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
        //    }
        //    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        //}
        //else
        //{
        //    MessageBox.Show("Page refresh is not allowed.");
        //}
        #endregion
        this.Process("D");
    }//This will discard error list and just proceed to other transactions

    protected void btnKeep_Click(object sender, EventArgs e)
    {
        #region Previous Logic
        //if (Session["update"].ToString() == ViewState["update"].ToString())
        //{
        //    if (!methods.GetProcessControlFlag("OVERTIME", "CUT-OFF"))
        //    {
        //        string[] employees = new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
        //        bool flag = true;
        //        DataTable temp = new DataTable();
        //        DataTable dtFinal = new DataTable();
        //        bool isValid = true;
        //        int indx = 0;
        //        for (int i = 0; i < lbxInclude.Items.Count; i++)
        //        {
        //            flag = true;
        //            for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
        //            {
        //                if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
        //                {
        //                    flag = false;
        //                }
        //            }
        //            if (flag)
        //            {
        //                employees[indx++] = lbxInclude.Items[i].Value.ToString();
        //            }
        //        }

        //        if (employees.Length > 0)
        //        {
        //            temp = checkOTConfilcts(employees);
        //            if (temp.Rows.Count > 0 && isValid)
        //            {
        //                isValid = false;
        //                lblErrorInfo.Text = "The following employees has conflicting previously filed overtime";
        //                dgvReview.DataSource = temp;
        //                dgvReview.DataBind();
        //            }
        //            else//Parameter trappings
        //            {
        //                temp = checkParameterValid(employees);
        //                if (temp.Rows.Count > 0 && isValid)
        //                {
        //                    isValid = false;
        //                    lblErrorInfo.Text = "The following employees has error on parameter settings";
        //                    dgvReview.DataSource = temp;
        //                    dgvReview.DataBind();
        //                }
        //            }
        //            if (isValid)//Route trappings
        //            {
        //                temp = checkRoutes(employees);
        //                if (temp.Rows.Count > 0)
        //                {
        //                    isValid = false;
        //                    lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
        //                    dgvReview.DataSource = temp;
        //                    dgvReview.DataBind();
        //                }
        //            }

        //            if (!isValid)
        //            {
        //                pnlBound.Visible = false;
        //                pnlReview.Visible = true;
        //                MessageBox.Show("Review the following entries.");
        //            }
        //            else
        //            {
        //                using (DALHelper dal = new DALHelper())
        //                {
        //                    try
        //                    {
        //                        dal.OpenDB();
        //                        dal.BeginTransactionSnapshot();
        //                        string batchNo = CommonMethods.GetControlNumber("OTBATCH");
        //                        string status = "3";//default only...
        //                        int counter = 0;
        //                        string controlNumbers = string.Empty;
        //                        for (int i = 0; i < employees.Length; i++)
        //                        {
        //                            status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "OVERTIME", "ENDORSE TO CHECKER 1", dal);
        //                            DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("OVERTIME"), employees[i], batchNo);
        //                            OTBL.CreateOTRecord(dr, dal);
        //                            controlNumbers += dr["Eot_ControlNo"].ToString() + ",";
        //                            counter++;
        //                        }
        //                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
        //                        {
        //                            EmailNotificationBL EMBL = new EmailNotificationBL();
        //                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
        //                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
        //                            EMBL.InsertInfoForNotification(controlNumbers
        //                                                            , Session["userLogged"].ToString()
        //                                                            , dal);
        //                        }
        //                        RepopulateList();
        //                        pnlBound.Visible = true;
        //                        pnlReview.Visible = false;
        //                        MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
        //                        dal.CommitTransactionSnapshot();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
        //                        dal.RollBackTransactionSnapshot();
        //                    }
        //                    finally
        //                    {
        //                        dal.CloseDB();
        //                    }
        //                }
        //            }
        //            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        //        }
        //        else
        //        {
        //            MessageBox.Show("No selected employees to file. \n Please cancel transaction.");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
        //    }
        //    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        //}
        //else
        //{
        //    MessageBox.Show("Page refresh is not allowed.");
        //}
        #endregion
        this.Process("K");
    }//This will keep error list back to lbxInclude so that user may do some changes(ex. Start Time)

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            pnlBound.Visible = true;
            pnlReview.Visible = false;
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
    private DataTable CheckIfValid()
    {
        string strTrail = string.Empty;
        DataTable temp = new DataTable();
        DataTable dtErrors = new DataTable();
        dtErrors.Columns.Add("Employee ID");
        dtErrors.Columns.Add("Employee Name");
        dtErrors.Columns.Add("Error Message");
        dtErrors.Columns.Add("Reason");

        try
        {

            #region Retrieve Selected Employees
            strTrail = "Retrieving Selected Employees";
            string[] employees = null;
            employees = new string[lbxInclude.Items.Count];
            for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
            {
                employees[ctr] = lbxInclude.Items[ctr].Value;
            }
            #endregion

            #region OT Conflicts
            strTrail = "Checking for Overtime Conflicts";
            temp = checkOTConfilcts(employees);
            if (temp != null
                && temp.Rows.Count > 0)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    DataRow dr = dtErrors.NewRow();
                    dr["Employee ID"] = temp.Rows[i]["EmployeeId"].ToString().Trim();
                    dr["Employee Name"] = temp.Rows[i]["Employee Name"].ToString().Trim();
                    dr["Error Message"] = "Overtime Conflict";
                    dr["Reason"] = temp.Rows[i]["OT Date"].ToString().Trim() + " ["
                                    + temp.Rows[i]["Start Time"].ToString().Trim() + " - "
                                    + temp.Rows[i]["End Time"].ToString().Trim() + "]";
                    dtErrors.Rows.Add(dr);
                }
            }
            #endregion

            #region Parameter Checking
            strTrail = "Parameter Checking";
            temp = checkParameterValid(employees);
            if (temp != null
                && temp.Rows.Count > 0)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    DataRow dr = dtErrors.NewRow();
                    dr["Employee ID"] = temp.Rows[i]["Employee ID"].ToString().Trim();
                    dr["Employee Name"] = temp.Rows[i]["Name"].ToString().Trim();
                    dr["Error Message"] = "Invalid during Parameter Checking";
                    dr["Reason"] = "[Total OT Hours including to file : " + temp.Rows[i]["Total OT Hours"].ToString().Trim() + "] "
                                    + temp.Rows[i]["Error Description"].ToString().Trim();
                    dtErrors.Rows.Add(dr);
                }
            }
            #endregion

            #region Routes Checking
            strTrail = "Employee Routes Checking";
            temp = checkRoutes(employees);
            if (temp.Rows.Count > 0)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    DataRow dr = dtErrors.NewRow();
                    dr["Employee ID"] = temp.Rows[i]["Employee ID"].ToString().Trim();
                    dr["Employee Name"] = temp.Rows[i]["Employee Name"].ToString().Trim();
                    dr["Error Message"] = "No Route setup";
                    dr["Reason"] = "[Checker 1 : " + temp.Rows[i]["Checker 1"].ToString().Trim() + "] "
                                    + "[Checker 2 : " + temp.Rows[i]["Checker 2"].ToString().Trim() + "] "
                                    + "[Approver : " + temp.Rows[i]["Approver"].ToString().Trim() + "] ";
                    dtErrors.Rows.Add(dr);
                }
            }

            strTrail = "Employee Costcenter Checking";
            temp = checkCostCenterMismatch(employees);
            if (temp.Rows.Count > 0)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    DataRow dr = dtErrors.NewRow();
                    dr["Employee ID"] = temp.Rows[i]["Employee ID"].ToString().Trim();
                    dr["Employee Name"] = temp.Rows[i]["Employee Name"].ToString().Trim();
                    dr["Error Message"] = "Mismatch Costcenter with AMS";
                    dr["Reason"] = "[HRC : " + temp.Rows[i]["HRC Costcenter"].ToString().Trim() + "] "
                                    + "[AMS : " + temp.Rows[i]["AMS Costcenter"].ToString().Trim() + "] ";
                    dtErrors.Rows.Add(dr);
                }
            }
            #endregion
        }
        catch(Exception er)
        {
            dtErrors = new DataTable();
            dtErrors.Columns.Add("Message");
            dtErrors.Columns.Add("Error Message");
            
            DataRow dr = dtErrors.NewRow();
            dr["Message"] = "Error in " + strTrail;
            dr["Error Message"] = er.Message;
            dtErrors.Rows.Add(dr);
        }
        if (dtErrors.Rows.Count > 0)
        {
            DataView dv = dtErrors.DefaultView;
            dv.Sort = "[" + dtErrors.Columns[0].Caption.ToString() + "] ASC";
            dtErrors = dv.ToTable();
        }

        return dtErrors;
    }

    private void InsertTransactions(DALHelper dal, DataTable dtErrors)
    {
        string batchNo = CommonMethods.GetControlNumber("OTBATCH");
        string status = "3";
        int counter = 0;
        string controlNumbers = string.Empty;
        string strColumnName = dtErrors.Columns[0].Caption.ToString();
        for (int i = 0; i < lbxInclude.Items.Count; i++)
        {
            bool isContinue = true;
            if (dtErrors.Rows.Count > 0)
            {
                DataRow[] drSelect = dtErrors.Select(string.Format(" [{0}] = '{1}' "
                            , strColumnName
                            , lbxInclude.Items[i].Value.ToString()));
                if (drSelect != null
                    && drSelect.Length > 0)
                {
                    isContinue = false;
                }
            }
            if (isContinue)
            {
                status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "OVERTIME", "ENDORSE TO CHECKER 1", dal);
                DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("OVERTIME"), lbxInclude.Items[i].Value.ToString(), batchNo);
                OTBL.CreateOTRecord(dr, dal);
                controlNumbers += dr["Eot_ControlNo"].ToString() + ",";
                counter++;
            }
        }
        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
        {
            EmailNotificationBL EMBL = new EmailNotificationBL();
            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.OVERTIME;
            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
            EMBL.InsertInfoForNotification(controlNumbers
                                            , Session["userLogged"].ToString()
                                            , dal);
        }
        //MessageBoxManager messageBoxManager = new MessageBoxManager();
        MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
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
            //Default Parameter
            //dtpOTDate.MinDate = CommonMethods.getMinimumDate();//this is for minimum date in logledger including hist
            //Andre added for use MINPASTPRD paramter
            dtpOTDate.MinDate = CommonMethods.getMinimumDateOfFiling();
            //END
        dtpOTDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        hfPrevOTDate.Value = dtpOTDate.Date.ToShortDateString();
        OTBL.initializeOTTypes(ddlType, false);
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
        }
        hfSTRTOTFRAC.Value = CommonMethods.getParamterValue("STRTOTFRAC").ToString();
        hfOTFRACTION.Value = CommonMethods.getParamterValue("OTFRACTION").ToString();
        //Andre added 20130702
		try
		{
        	hfOTSTARTPAD.Value = CommonMethods.getParamterValue("OTSTARTPAD").ToString();
		}
		catch{}

        pnlBound.Visible = true;
        pnlReview.Visible = false;
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

        ///////NOT applicable in batch overtime yet
        //txtJobCode.Text = string.Empty;
        //txtClientJobNo.Text = string.Empty;
        //txtClientJobName.Text = string.Empty;

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        hfPrevOTDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeControls();
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
                         WHERE Ell_ProcessDate = @ProcessDate
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
                         WHERE Ell_ProcessDate = @ProcessDate
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
        ParameterInfo[] param = new ParameterInfo[3];
        param[0] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[2] = new ParameterInfo("@TransactionType", "OVERTIME");

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
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                         WHERE REPLICATE(' ' , 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType) 
                             + REPLICATE(' ' , 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)  = @workTYPGRP
                           AND Ell_Processdate = @ProcessDate

                         ORDER BY [Code]";
		if(Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
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
			if(dtpOTDate.Date > Convert.ToDateTime(end))
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
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
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
            ddlShift.Items.Add(new ListItem( dt.Rows[ctr]["ShiftCode"]
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
            string sql = @" SELECT DISTINCT Emt_CostcenterCode [Code]
	                             , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Costcenter]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID

                             -->INNER JOIN T_UserCostCenterAccess
                             -->   ON Uca_CostcenterCode = Emt_CostcenterCode
                             -->  AND Uca_UserCode = @UserCode
                             -->  AND Uca_SytemID = 'OVERTIME'

                             WHERE Ell_ProcessDate = @ProcessDate
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup
                               AND Ell_ShiftCode = @ShiftCode

                             UNION

                            SELECT DISTINCT Emt_CostcenterCode [Code]
	                             , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Costcenter]
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID

                             -->INNER JOIN T_UserCostCenterAccess
                             -->   ON Uca_CostcenterCode = Emt_CostcenterCode
                             -->  AND Uca_UserCode = @UserCode
                             -->  AND Uca_SytemID = 'OVERTIME'

                             WHERE Ell_ProcessDate = @ProcessDate
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup
                               AND Ell_ShiftCode = @ShiftCode

                             ORDER BY [Costcenter]";
            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
            param[1] = new ParameterInfo("@ProcessDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
            param[2] = new ParameterInfo("@WorkType", ddlGroup.SelectedValue.Substring(0, 3).Trim());
            param[3] = new ParameterInfo("@WorkGroup", ddlGroup.SelectedValue.Substring(3, 3).Trim());
            param[4] = new ParameterInfo("@ShiftCode", ddlShift.SelectedValue.Substring(0, 10).Trim());

            //if costcenter access is not all, inner join with usercostcenter access to filter 
            //by removing commented part of query
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
            {
                sql = sql.Replace("-->", "");
            }
            //end
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
            ddlCostcenter.Items.Add(new ListItem("ALL", "ALL"));
            for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
            {
                ddlCostcenter.Items.Add(new ListItem(dt.Rows[ctr]["Costcenter"].ToString(), dt.Rows[ctr]["Code"].ToString()));
            }
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
            ddlCostcenter.Items.Add(new ListItem(dt.Rows[ctr]["Desc"].ToString(), dt.Rows[ctr]["Code"].ToString()));
        }
    }

    private void FillChoiceList()
    {
        DataTable dt = new DataTable();
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
                         WHERE Ell_ProcessDate = '{1}'
                            -- Filter Insertion -- 
                           {0}

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
                         WHERE Ell_ProcessDate = '{1}'
                            -- Filter Insertion -- 
                           {0}

                         ORDER BY 1 /* [ForSort] */", QueryFilter(), dtpOTDate.Date.ToString("MM/dd/yyyy"));

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
        lblNoOfItemsChoice.Text = lbxChoice.Items.Count + " no. of item(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count + " no. of item(s)";
    }

    protected string QueryFilter()
    {
        string filter = string.Empty;
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
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_LastName like '{0}%'
                                           OR Emt_FirstName like '{0}%'
                                           OR Emt_NickName like '{0}%'
                                           OR Emt_EmployeeId like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
        }
        if (ddlCostcenter.SelectedValue.Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME")) 
            {
                filter += string.Format(@"AND Emt_CostCenterCode IN (SELECT Uca_CostCenterCode
                                                                       FROM T_UserCostCenterAccess
                                                                      WHERE Uca_Status = 'A'
                                                                        AND Uca_SytemId = 'OVERTIME'
                                                                        AND Uca_UserCode = '{0}')", Session["userLogged"]);
            }
        }
        else
        {
            filter += string.Format(@"AND Emt_CostCenterCode = '{0}'", ddlCostcenter.SelectedItem.Value);
        }



        return filter;
    }

    private void showOptionalFields()
    {
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'";
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "OVERTIME", "WFBOTENTRY");
        bool canFileOT = userGrant.canReprint();
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
                if (OTStart < O2 && hfDayCode.Value.ToUpper().Contains("REG"))
                {
                    err += "\nStart Time cannot be within shift in POST Overtime on regular days.";
                }
            }
            else if (ddlType.SelectedValue.Equals("A"))//Overtime Type is Advance
            {
                //for HOGP: just removed trapping for ADVANCE filing for NON-REGULAR days
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
                        err += "\nEnd TIme cannot be greater than shift Time In for ADVANCE Overtime";
                    }
                    else if (OTStart >= I1 && hfDayCode.Value.ToUpper().Contains("REG"))
                    {
                        err += "\nStart Time cannot be greater or equal than shift Time In for ADVANCE Overtime";
                    }
                }
            }
            else//Overtime Type is Mid
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
            err += String.Format("\n Invalid entry in overtime start time. Valid start time is {0}:{1} onwards.", Convert.ToInt32((o2MIN + Convert.ToDecimal(hfOTSTARTPAD.Value)) / 60), Convert.ToInt32((o2MIN + Convert.ToDecimal(hfOTSTARTPAD.Value)) % 60));
        }
        #endregion

        if (err.Equals(string.Empty))
        {
            string timeCutOff = string.Empty;
            DataSet dsParameter = new DataSet();
            DataSet dsShift = CommonMethods.getShiftInformation(ddlShift.SelectedValue.Substring(0, 10).Trim());
            string scheduleType = string.Empty;
            string paramValue = string.Empty;
            decimal timeIn = 0;
            decimal timeOut = 0;
            int timeExt = 0;

            if (dsShift.Tables[0].Rows.Count > 0)
            {
                scheduleType = dsShift.Tables[0].Rows[0]["Scm_ScheduleType"].ToString();
                timeIn = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString());
                timeOut = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString());
            }

            string query = string.Format(@"SELECT Pmx_ParameterValue
                                        FROM T_ParameterMasterExt
                                        WHERE Pmx_ParameterId = 'OTFLB4'
                                        AND Pmx_Classification = '{0}'
                                        
                                        SELECT Pmt_NumericValue
                                        FROM T_ParameterMaster
                                        WHERE Pmt_ParameterId = 'OTFILEPAD'", scheduleType);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsParameter = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            if (dsParameter.Tables[0].Rows.Count > 0)
            {
                paramValue = dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString();
                if (dsParameter.Tables[1].Rows.Count > 0)
                    timeExt = Convert.ToInt32(dsParameter.Tables[1].Rows[0]["Pmt_NumericValue"]);

                if (paramValue.ToUpper().Equals("STARTOFSHIFT"))
                {
                    if (timeExt > 0)
                        timeCutOff = Convert.ToString(Convert.ToDecimal(timeIn.ToString().PadLeft(4, '0').Substring(0, 2)) + timeExt) + ":" + timeIn.ToString().PadLeft(4, '0').Substring(2, 2);
                    else
                        timeCutOff = timeIn.ToString().Substring(0, 2) + ":" + timeIn.ToString().Substring(2, 2);
                }
                else if (paramValue.ToUpper().Equals("ENDOFSHIFT"))
                {
                    if (timeIn < timeOut)
                    {
                         if (timeExt > 0)
                                timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + timeExt).PadLeft(2, '0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                            else
                             timeCutOff = timeOut.ToString().Substring(0, 2) + ":" + timeOut.ToString().Substring(2, 2);
                    }
                    else if (timeOut < timeIn)
                    {
                        if (timeExt > 0)
                            timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + 24 + timeExt).PadLeft(2, '0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                        else
                            timeCutOff = Convert.ToString(Convert.ToDecimal(timeOut.ToString().PadLeft(4, '0').Substring(0, 2)) + 24).PadLeft(2, '0') + ":" + timeOut.ToString().PadLeft(4, '0').Substring(2, 2);
                    }
                }
                else
                {
                    if (paramValue.Length == 4)
                    {
                        timeCutOff = dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString().Substring(0, 2) + ":" + dsParameter.Tables[0].Rows[0]["Pmx_ParameterValue"].ToString().Substring(2, 2);
                    }
                }
                if (timeCutOff != string.Empty)
                {
                    DateTime dateTimeCutOff;
                    if (Convert.ToDouble(timeCutOff.Replace(":", "")) > 2359 && Convert.ToDouble(timeCutOff.Replace(":", "")) < 4759)
                    {
                        dateTimeCutOff = Convert.ToDateTime(string.Format("{0} {1}", dtpOTDate.Date.AddDays(1).ToString("MM/dd/yyyy"), Convert.ToString(Convert.ToDecimal(timeCutOff.Substring(0, 2)) - 24).PadLeft(2, '0') + ":" + timeCutOff.Substring(3, 2).PadLeft(2, '0')));
                        if (DateTime.Now > dateTimeCutOff)
                        {
                            err += string.Format("Overtime Filing Cut-off: {0}", dateTimeCutOff.ToString("MM/dd/yyyy hh:mm tt"));
                        }
                    }
                    else if (Convert.ToDouble(timeCutOff.Replace(":", "")) <= 2359)
                    {
                        dateTimeCutOff = Convert.ToDateTime(string.Format("{0} {1}", dtpOTDate.Date.ToString("MM/dd/yyyy"), timeCutOff));
                        if (DateTime.Now > dateTimeCutOff)
                        {
                            err += string.Format("Overtime Filing Cut-off: {0}", dateTimeCutOff.ToString("MM/dd/yyyy hh:mm tt"));
                        }
                    }
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

    private DataTable checkOTConfilcts(string[] empId)
    {
        DataTable dt = new DataTable();
        DataTable dtToReturn = new DataTable();
        dtToReturn.Columns.Add("EmployeeId");
        dtToReturn.Columns.Add("Employee Name");
        dtToReturn.Columns.Add("OT Date");
        dtToReturn.Columns.Add("Start Time");
        dtToReturn.Columns.Add("End Time");
        #region SQL
        string sql = string.Format(@" declare @start datetime
                                      declare @end datetime
                                      declare @DSP as bit
                                          SET @DSP = (SELECT Pcm_ProcessFlag
                                                        FROM T_ProcessControlMaster
                                                       WHERE Pcm_ProcessId = 'DSPFULLNM')
                                          SET @start = dbo.getDatetimeFormatV2(@startTime,@overtimeDate,@overtimeType,@shiftType,@shiftStart)
                                          SET @end = dbo.getDatetimeFormatV2(@endTime,@overtimeDate,@overtimeType,@shiftType,@shiftStart)

                                SELECT Eot_EmployeeId [EmployeeId]
                                     , CASE WHEN (@DSP = '1')
                                            THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                            ELSE Emt_NickName
                                        END AS [Employee Name]
                                     , CONVERT(varchar(10), Eot_OvertimeDate, 101) AS [OT Date]
                                     , LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) [Start Time]
                                     , LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) [End Time]
                                     , ISNULL(SCM1.Scm_ShiftTimeIn, SCM2.Scm_ShiftTimeIn) [Time In]
                                  FROM T_EmployeeOvertime
                                 INNER JOIN T_EmployeeMaster
                                    ON Emt_EmployeeId = Eot_EmployeeId
                                  LEFT JOIN T_EmployeeLogLedger ELL1
                                    ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL1.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM1
                                    ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode

                                  LEFT JOIN T_EmployeeLogLedgerHist ELL2
                                    ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL2.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM2
                                    ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode

                                 WHERE Eot_EmployeeId IN ({0})
                                   AND Eot_Status IN ('1','3','5','7', '9', 'A')
                                   AND Eot_OvertimeType IN @OTTypes
                                   AND (   ( @start >= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                              , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                              , Eot_OvertimeType 
                                                                              , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                              , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
                                         AND @start < dbo.getDatetimeFormatV2( Eot_EndTime
                                                                             , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                             , Eot_OvertimeType 
                                                                             , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                             , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
                                        OR ( @end > dbo.getDatetimeFormatV2( Eot_StartTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , Eot_OvertimeType 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
                                         AND @end <= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , Eot_OvertimeType 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
	                                    OR (     @start <= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                  , Eot_OvertimeType 
                                                                                  , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                  , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                         AND  @end >= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                                 , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                 , Eot_OvertimeType 
                                                                                 , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                 , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
                                       )
                                 UNION   

                                 SELECT Eot_EmployeeId [EmployeeId]
                                     , CASE WHEN (@DSP = '1')
                                            THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                            ELSE Emt_NickName
                                        END AS [Employee Name]
                                     , CONVERT(varchar(10), Eot_OvertimeDate, 101) AS [OT Date]
                                     , LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) [Start Time]
                                     , LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) [End Time]
                                     , ISNULL(SCM1.Scm_ShiftTimeIn, SCM2.Scm_ShiftTimeIn) [Time In]
                                  FROM T_EmployeeOvertimeHist
                                 INNER JOIN T_EmployeeMaster
                                    ON Emt_EmployeeId = Eot_EmployeeId

                                  LEFT JOIN T_EmployeeLogLedger ELL1
                                    ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL1.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM1
                                    ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode

                                  LEFT JOIN T_EmployeeLogLedgerHist ELL2
                                    ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
                                   AND ELL2.Ell_EmployeeId = Eot_EmployeeID
                                  LEFT JOIN T_ShiftCodeMaster SCM2
                                    ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode

                                 WHERE Eot_EmployeeId IN ({0})
                                   AND Eot_Status IN ('1','3','5','7', '9', 'A')
                                   AND Eot_OvertimeType IN @OTTypes
                                   AND (   ( @start >= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                              , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                              , Eot_OvertimeType 
                                                                              , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                              , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
                                         AND @start < dbo.getDatetimeFormatV2( Eot_EndTime
                                                                             , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                             , Eot_OvertimeType 
                                                                             , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType) 
                                                                             , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
                                        OR ( @end > dbo.getDatetimeFormatV2( Eot_StartTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , Eot_OvertimeType 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
                                         AND @end <= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                            , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                            , Eot_OvertimeType 
                                                                            , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                            , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
	                                    OR (     @start <= dbo.getDatetimeFormatV2( Eot_StartTime
                                                                                  , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                  , Eot_OvertimeType 
                                                                                  , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                  , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                         AND  @end >= dbo.getDatetimeFormatV2( Eot_EndTime
                                                                                 , Convert(varchar(10),Eot_OvertimeDate,101)
                                                                                 , Eot_OvertimeType 
                                                                                 , ISNULL(SCM2.Scm_ScheduleType, SCM1.Scm_ScheduleType)
                                                                                 , ISNULL(SCM2.Scm_ShiftTimeIn, SCM1.Scm_ShiftTimeIn) )
	                                       )
                                       )
                                 ORDER BY [OT Date]", FormatForInQuery(empId));
        #endregion
        ParameterInfo[] param = new ParameterInfo[6];
        param[0] = new ParameterInfo("@overtimeDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        param[1] = new ParameterInfo("@startTime", txtOTStartTime.Text.Replace(":", ""));
        param[2] = new ParameterInfo("@endTime", txtOTEndTime.Text.Replace(":", ""));
        param[3] = new ParameterInfo("@overtimeType", ddlType.SelectedValue);
        param[4] = new ParameterInfo("@shiftType", hfShiftType.Value.Trim());
        param[5] = new ParameterInfo("@shiftStart", hfI1.Value.Trim()); 
        
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                if ( CommonMethods.getMinuteValue(txtOTStartTime.Text.Replace(":", "")) >= CommonMethods.getMinuteValue(hfO1.Value.Replace(":", ""))
                  && CommonMethods.getMinuteValue(txtOTStartTime.Text.Replace(":", "")) <= CommonMethods.getMinuteValue(hfI2.Value.Replace(":", ""))
                  && CommonMethods.getMinuteValue(txtOTEndTime.Text.Replace(":", "")) <= CommonMethods.getMinuteValue(hfO1.Value.Replace(":", ""))
                  && CommonMethods.getMinuteValue(txtOTEndTime.Text.Replace(":", "")) <= CommonMethods.getMinuteValue(hfI2.Value.Replace(":", ""))
                  && ddlType.SelectedValue.Equals("M"))
                {
                    dt = dal.ExecuteDataSet(sql.Replace("@OTTypes", "('M')"), CommandType.Text, param).Tables[0];
                }
                else
                {
                    dt = dal.ExecuteDataSet(sql.Replace("@OTTypes", "('A', 'P')"), CommandType.Text, param).Tables[0];
                }
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                throw ex;
            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (dt != null
            && dt.Rows.Count > 0)
        {
            DateTime dtTransactStart = GetDateValue(dtpOTDate.Date.ToString("MM/dd/yyyy"), txtOTStartTime.Text.Trim(), hfI1.Value);
            DateTime dtTransactEnd = GetDateValue(dtpOTDate.Date.ToString("MM/dd/yyyy"), txtOTEndTime.Text.Trim(), hfI1.Value);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dtTempStart = GetDateValue(
                                                    dt.Rows[i]["OT Date"].ToString()
                                                    , dt.Rows[i]["Start Time"].ToString()
                                                    , dt.Rows[i]["Time In"].ToString());
                DateTime dtTempEnd = GetDateValue(
                                            dt.Rows[i]["OT Date"].ToString()
                                            , dt.Rows[i]["End Time"].ToString()
                                            , dt.Rows[i]["Time In"].ToString());
                if (CheckIfOverlap(dtTransactStart, dtTransactEnd, dtTempStart, dtTempEnd))
                {
                    DataRow dr = dtToReturn.NewRow();
                    dr["EmployeeId"] = dt.Rows[i]["EmployeeId"].ToString();
                    dr["Employee Name"] = dt.Rows[i]["Employee Name"].ToString();
                    dr["OT Date"] = dt.Rows[i]["OT Date"].ToString();
                    dr["Start Time"] = dt.Rows[i]["Start Time"].ToString();
                    dr["End Time"] = dt.Rows[i]["End Time"].ToString();
                    dtToReturn.Rows.Add(dr);
                }
            }
        }
        return dtToReturn;
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

    public bool CheckIfOverlap(DateTime dtToCompareStart, DateTime dtToCompareEnd, DateTime dtTransactionStart, DateTime dtTransactionEnd)
    {
        bool ret = false;
        if (dtTransactionStart > dtTransactionEnd)
        {
            dtTransactionEnd = dtTransactionEnd.AddDays(1);
        }
        if ((dtToCompareStart >= dtTransactionStart && dtToCompareStart <= dtTransactionEnd)
            || (dtToCompareEnd >= dtTransactionStart && dtToCompareEnd <= dtTransactionEnd)
            || (dtToCompareStart <= dtTransactionStart && dtToCompareEnd >= dtTransactionEnd))
        {
            ret = true;
        }
        return ret;
    }


    private DataTable checkParameterValid(string[] empId)
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        #region SQL  Revised by Perth 04/25/2013 retrieve the ot hours using only date
        //        string sqlOvertimeHours = @"declare @tempTable as TABLE
//                                    (
//                                        employeeid varchar(15)
//	                                    ,total decimal(8,2)
//                                    )
//
//                                    INSERT INTO @tempTable
//                                    SELECT Eot_EmployeeId
//                                         , ISNULL( SUM(CASE WHEN (Eot_OvertimeDate < @overtimeDate)
//                                                            THEN (datediff( MINUTE
//                                                                         , dbo.getDatetimeFormatV2( '0000'
//                                                                                                , Convert(varchar(10),@overtimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , '0000' )
//                                                                         , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                   END ))/60.0)
//                                                            ELSE
//                                                            CASE WHEN (Convert(varchar(10),dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                , Eot_OvertimeType 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                                   END 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                   END ),101) > @overtimeDate)
//                                                                 THEN Eot_OvertimeHour - (datediff( MINUTE
//                                                                                                 , dbo.getDatetimeFormatV2( '0000'
//                                                                                                                        , Convert(varchar(10),dateadd(day,1,@overtimeDate),101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , '0000' )
//                                                                                                 , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                        , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                               ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                           END) ) /60.00)
//                                                                 ELSE Eot_OvertimeHour
//                                                             END
//                                                        END), 0) [Total]
//                                      FROM T_EmployeeOvertime
//
//                                      LEFT JOIN T_EmployeeLogLedger ELL1
//                                        ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL1.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM1
//                                        ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode
//                                      LEFT JOIN T_EmployeeLogLedgerHist ELL2
//                                        ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL2.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM2
//                                        ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode
//
//                                     WHERE (Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate
//                                        OR Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate)
//                                       AND Eot_EmployeeId IN ({0})
//                                       AND Eot_Status IN ('1','3','5','7','9','A')
//                                     GROUP BY Eot_EmployeeId
//
//                                     UNION
//
//                                    SELECT Eot_EmployeeId 
//                                         , ISNULL( SUM(CASE WHEN (Eot_OvertimeDate < @overtimeDate)
//                                                            THEN (datediff( MINUTE
//                                                                         , dbo.getDatetimeFormatV2( '0000'
//                                                                                                , Convert(varchar(10),@overtimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , '0000')
//                                                                         , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                , Eot_OvertimeType 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                   END 
//                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                   END ))/60.0)
//                                                            ELSE
//                                                            CASE WHEN (Convert(varchar(10),dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                , Eot_OvertimeType 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ScheduleType
//                                                                                                                       ELSE SCM1.Scm_ScheduleType
//                                                                                                                   END 
//                                                                                                                , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                       THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                       ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                   END ),101) > @overtimeDate)
//                                                                 THEN Eot_OvertimeHour - (datediff( MINUTE
//                                                                                                 , dbo.getDatetimeFormatV2( '0000'
//                                                                                                                        , Convert(varchar(10),dateadd(day,1,@overtimeDate),101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , '0000' )
//                                                                                                 , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                                                                        , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                                                                        , Eot_OvertimeType 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ScheduleType
//                                                                                                                               ELSE SCM1.Scm_ScheduleType
//                                                                                                                           END 
//                                                                                                                        , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                                                               THEN SCM2.Scm_ShiftTimeIn
//                                                                                                                               ELSE SCM1.Scm_ShiftTimeIn
//                                                                                                                           END) ) /60.00)
//                                                                 ELSE Eot_OvertimeHour
//                                                             END
//                                                        END), 0) [Total]
//                                      FROM T_EmployeeOvertimeHist
//
//                                      LEFT JOIN T_EmployeeLogLedger ELL1
//                                        ON ELL1.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL1.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM1
//                                        ON SCM1.Scm_ShiftCode = ELL1.Ell_ShiftCode
//                                      LEFT JOIN T_EmployeeLogLedgerHist ELL2
//                                        ON ELL2.Ell_ProcessDate = Eot_OvertimeDate
//                                       AND ELL2.Ell_EmployeeId = Eot_EmployeeID
//                                      LEFT JOIN T_ShiftCodeMaster SCM2
//                                        ON SCM2.Scm_ShiftCode = ELL2.Ell_ShiftCode
//
//                                     WHERE (Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_StartTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate
//                                        OR Convert( varchar(10)
//                                                  , dbo.getDatetimeFormatV2( Eot_EndTime
//                                                                         , Convert(varchar(10),Eot_OvertimeDate,101)
//                                                                         , Eot_OvertimeType 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ScheduleType
//                                                                                ELSE SCM1.Scm_ScheduleType
//                                                                            END 
//                                                                         , CASE WHEN (Eot_OvertimeFlag = 'P') 
//                                                                                THEN SCM2.Scm_ShiftTimeIn
//                                                                                ELSE SCM1.Scm_ShiftTimeIn
//                                                                            END )
//                                                  , 101) = @overtimeDate)
//                                       AND Eot_EmployeeId IN ({0})
//                                       AND Eot_Status IN ('1','3','5','7','9','A')
//                                     GROUP BY Eot_EmployeeId
//
//                                   declare @DSP as bit
//                                       SET @DSP = (SELECT Pcm_ProcessFlag
//                                                     FROM T_ProcessControlMaster
//                                                    WHERE Pcm_ProcessId = 'DSPFULLNM')
//
//                                    SELECT employeeid [Employee ID]
//                                         , CASE WHEN (@DSP = '1')
//                                                THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
//                                                ELSE Emt_NickName
//                                            END [Employee Name]
//                                         , CASE WHEN 'TRUE' = '{1}' 
//                                                THEN ISNULL(Emt_JobLevel,'')
//                                                ELSE ''
//                                            END [Classification]
//                                         , SUM(Total) [Total OT Hours]
//                                      FROM @tempTable
//                                     INNER JOIN T_EmployeeMaster
//                                        ON Emt_EmployeeId = employeeid
//                                     GROUP BY employeeid
//                                            , CASE WHEN (@DSP = '1')
//                                                   THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
//                                                   ELSE Emt_NickName
//                                               END
//                                            {2}
//                ------ ds.Tables[1]
//                SELECT Pmt_ParameterId
//                     , Pmt_NumericValue
//                     , CASE WHEN (Pmt_Extension = 1)
//                            THEN 'TRUE'
//                            ELSE 'FALSE' 
//                        END [Pmt_Extension]
//                  FROM T_ParameterMaster
//                 WHERE Pmt_ParameterId IN ('MAXOTHR','MAXOTHRRG','MINOTHR','OTFRACTION','STRTOTFRAC')";
        string sqlOvertimeHours = @"
declare @DSP as bit
SET @DSP = (SELECT Pcm_ProcessFlag
         FROM T_ProcessControlMaster
        WHERE Pcm_ProcessId = 'DSPFULLNM')

SELECT
	Eot_EmployeeId [Employee ID]
	, CASE WHEN (@DSP = '1')
        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
        ELSE Emt_NickName
    END [Employee Name]
    , CASE WHEN 'TRUE' = '{1}' 
        THEN ISNULL(Emt_JobLevel,'')
        ELSE ''
    END [Classification]
	, COALESCE(sum(Eot_OvertimeHour), 0.00) [Total OT Hours]
FROM (
SELECT
	Eot_EmployeeId  
	, Eot_OvertimeHour
FROM T_EmployeeOvertime
WHERE Eot_EmployeeId IN ({0})
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
UNION ALL
SELECT 
	Eot_EmployeeId  
	, Eot_OvertimeHour
FROM T_EmployeeOvertimeHist
WHERE Eot_EmployeeId IN ({0})
	AND Eot_OvertimeDate = @overtimeDate
	AND Eot_Status IN ('1', '3', '5', '7', '9', 'A')
	) AS TABLETEMP
LEFT JOIN T_EmployeeMaster
	ON Eot_EmployeeId = Emt_EmployeeID
GROUP BY Eot_EmployeeId 
	, CASE WHEN (@DSP = '1')
        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
        ELSE Emt_NickName
    END
     {2}
                                   
                ------ ds.Tables[1]
                SELECT Pmt_ParameterId
                     , Pmt_NumericValue
                     , CASE WHEN (Pmt_Extension = 1)
                            THEN 'TRUE'
                            ELSE 'FALSE' 
                        END [Pmt_Extension]
                  FROM T_ParameterMaster
                 WHERE Pmt_ParameterId IN ('MAXOTHR','MAXOTHRRG','MINOTHR','OTFRACTION','STRTOTFRAC','OTSTARTPAD')";
        #endregion
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@overtimeDate", dtpOTDate.Date.ToString("MM/dd/yyyy"));
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlOvertimeHours, FormatForInQuery(empId)
                                                                      , Resources.Resource.CHIYODASPECIFIC.ToUpper()
                                                                      , Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC.ToUpper()) ? ", Emt_JobLevel" : string.Empty), CommandType.Text, param);
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

        dt.Columns.Add("Employee ID");
        dt.Columns.Add("Name");
        dt.Columns.Add("Total OT Hours");
        dt.Columns.Add("Error Description");

        decimal MAXOTHR = 0;
        decimal MAXOTHRRG = 0;
        decimal MINOTHR = 0;
        decimal OTFRACTION = 0;//not applicable in this function
        decimal STRTOTFRAC = 0;//not applicable in this function
        bool hasExtensionMINOTHR = false;
        bool hasClassifiaction = false;
        bool hasError = false;

        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
        {
            switch(ds.Tables[1].Rows[i]["Pmt_ParameterId"].ToString().ToUpper())
            {
                case "MAXOTHR":
                    MAXOTHR = Convert.ToDecimal(ds.Tables[1].Rows[i]["Pmt_NumericValue"].ToString());
                    break;
                case "MAXOTHRRG":
                    MAXOTHRRG = Convert.ToDecimal(ds.Tables[1].Rows[i]["Pmt_NumericValue"].ToString());
                    break;
                case "MINOTHR":
                    MINOTHR = Convert.ToDecimal(ds.Tables[1].Rows[i]["Pmt_NumericValue"].ToString());
                    hasExtensionMINOTHR = Convert.ToBoolean(ds.Tables[1].Rows[i]["Pmt_Extension"].ToString());
                    break;
                case "OTFRACTION":
                    OTFRACTION = Convert.ToDecimal(ds.Tables[1].Rows[i]["Pmt_NumericValue"].ToString());
                    break;
                case "STRTOTFRAC":
                    STRTOTFRAC = Convert.ToDecimal(ds.Tables[1].Rows[i]["Pmt_NumericValue"].ToString());
                    break;
                default:
                    break;
            }
        }

        decimal dOTHour = Convert.ToDecimal(this.txtOTHours.Text);
        for (int i = 0; i < ds.Tables[0].Rows.Count && !hasError; i++)
        {
            // 1. MAXOTHR and MAXOTHRRG
            if ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text)) > MAXOTHRRG && hfDayCode.Value.Contains("REG"))
            {
                DataRow dr = dt.NewRow();
                dr["Employee ID"] = ds.Tables[0].Rows[i]["Employee ID"].ToString();
                dr["Name"] = ds.Tables[0].Rows[i]["Employee Name"].ToString();
                dr["Total OT Hours"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text);
                dr["Error Description"] = String.Format("Exceed maximum OT Hours for regular days. {0} hours.", MAXOTHRRG);
                dt.Rows.Add(dr);
                hasError = true;
            }
            else if ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text)) > MAXOTHR)
            {
                DataRow dr = dt.NewRow();
                dr["Employee ID"] = ds.Tables[0].Rows[i]["Employee ID"].ToString();
                dr["Name"] = ds.Tables[0].Rows[i]["Employee Name"].ToString();
                dr["Total OT Hours"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text);
                dr["Error Description"] = String.Format("Exceed maximum OT Hours for non-regular days. {0} hours.", MAXOTHR);
                dt.Rows.Add(dr);
                hasError = true;
            }

            // 2. MINOTHR
            if (!hasError)
            {
                DataSet dsMINOTHR = new DataSet();
                dsMINOTHR = CommonMethods.getParamterExtensionValues("MINOTHR");
                //for chiyoda: MINOTHR will depend on rank and file
                if (hasExtensionMINOTHR)
                {
                    hasClassifiaction = false;
                    if (!CommonMethods.isEmpty(ds))
                    {
                        for (int x = 0; x < dsMINOTHR.Tables[0].Rows.Count; x++)
                        {
                            if (ds.Tables[0].Rows[i]["Classification"].ToString() == dsMINOTHR.Tables[0].Rows[x]["Pmx_Classification"].ToString())
                            {
                                hasClassifiaction = true;
                                if ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text)) < Convert.ToDecimal(dsMINOTHR.Tables[0].Rows[x]["Pmx_ParameterValue"].ToString()))
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["Employee ID"] = ds.Tables[0].Rows[i]["Employee ID"].ToString();
                                    dr["Name"] = ds.Tables[0].Rows[i]["Employee Name"].ToString();
                                    dr["Total OT Hours"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text);
                                    dr["Error Description"] = @"Less than minimum OT Hours for " + dsMINOTHR.Tables[0].Rows[x]["Pmx_ParameterDesc"].ToString() + "."
                                                                                                                     + dsMINOTHR.Tables[0].Rows[x]["Pmx_ParameterValue"].ToString() + " hours.";
                                    dt.Rows.Add(dr);
                                    hasError = true;
                                }
                                break;
                            }
                        }
                        if (!hasClassifiaction)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Employee ID"] = ds.Tables[0].Rows[i]["Employee ID"].ToString();
                            dr["Name"] = ds.Tables[0].Rows[i]["Employee Name"].ToString();
                            dr["Total OT Hours"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text);
                            dr["Error Description"] = @"No employee classification for MINOTHR";
                            dt.Rows.Add(dr);
                            hasError = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No classification defined for MINOTHR");
                    }
                }
                else if ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text)) < MINOTHR)
                {
                    DataRow dr = dt.NewRow();
                    dr["Employee ID"] = ds.Tables[0].Rows[i]["Employee ID"].ToString();
                    dr["Name"] = ds.Tables[0].Rows[i]["Employee Name"].ToString();
                    dr["Total OT Hours"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Total OT Hours"].ToString()) + Convert.ToDecimal(txtOTHours.Text);
                    dr["Error Description"] = String.Format(@"Less than minimum OT Hours. {0} hours.", MINOTHR);
                    dt.Rows.Add(dr);
                    hasError = true;
                }
            }
        }
        return dt;
    }

    private DataTable checkCostCenterMismatch(string[] empID)
    {
        DataSet ds = new DataSet();
        string sql = @"  DECLARE @Date as datetime = '{0}'
                        DECLARE @CHECK as varchar(5)
                        DECLARE @CTRLINEREF as bit  = (  SELECT Pcm_ProcessFlag 
								                           FROM T_ProcessControlMaster
								                          WHERE Pcm_SystemID = 'GENERAL'
									                        AND Pcm_ProcessID = 'CTRLINEREF')
                        DECLARE @DSP as bit
                        SET @DSP = (SELECT Pcm_ProcessFlag
		  	                        FROM T_ProcessControlMaster
			                        WHERE Pcm_ProcessId = 'DSPFULLNM')

                        SELECT Emt_EmployeeId [Employee ID]
	                         , CASE WHEN (@DSP = '1')
			                        THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
			                        ELSE Emt_NickName
		                        END [Employee Name]
                             , ISNULL(ECM1.Ecm_CostCenterCode, '') [HRC Costcenter]
                             , ISNULL(ECM2.Ecm_CostCenterCode, '') [AMS Costcenter]
                          FROM T_EmployeeMaster
                          LEFT JOIN T_EmployeeCostCenterMovement [ECM1]
                            ON ECM1.Ecm_EmployeeID = Emt_EmployeeID
                           AND @Date BETWEEN ECM1.Ecm_StartDate AND ISNULL(ECM1.Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                          LEFT JOIN E_EmployeeCostCenterLineMovement [ECM2]
                            ON ECM2.Ecm_EmployeeID = Emt_EmployeeID
                           AND @Date BETWEEN ECM2.Ecm_StartDate AND ISNULL(ECM2.Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                          LEFT JOIN ( SELECT DISTINCT Clm_CostCenterCode
                                        FROM E_CostCenterLineMaster
			                           WHERE Clm_Status = 'A' ) AS TEMP
                            ON Clm_CostCenterCode = ECM1.Ecm_CostCenterCode
                         WHERE Emt_EmployeeID IN ({1})
                           AND 1 = IIF( ISNULL(ECM1.Ecm_CostCenterCode, Emt_CostCenterCode) 
                                     <> IIF(Clm_CostCenterCode IS NOT NULL, ISNULL(ECM2.Ecm_CostCenterCode, ''), ISNULL(ECM1.Ecm_CostCenterCode, Emt_CostCenterCode)), 1, 0)
                           AND 1 = ISNULL(@CTRLINEREF, 0) ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, dtpOTDate.Date.ToString("MM/dd/yyyy"), FormatForInQuery(empID)), CommandType.Text);
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

        return ds.Tables[0];
    }

    private DataTable checkRoutes(string[] empId)
    {
        DataSet ds = new DataSet();
        #region SQL
        string sql = @"   declare @DSP as bit
	                          SET @DSP = (SELECT Pcm_ProcessFlag
		  		                            FROM T_ProcessControlMaster
				                            WHERE Pcm_ProcessId = 'DSPFULLNM')

                           SELECT Emt_EmployeeId [Employee ID]
	                            , CASE WHEN (@DSP = '1')
			                           THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
			                           ELSE Emt_NickName
		                           END [Employee Name]
                                , ISNULL(a.Arm_Checker1,'-NONE-') [Checker 1]
		                        , ISNULL(a.Arm_Checker2,'-NONE-') [Checker 2]
		                        , ISNULL(a.Arm_Approver,'-NONE-') [Approver]
	                         FROM T_EmployeeMaster
	                         LEFT JOIN T_EmployeeApprovalRoute AS e
	                           ON e.Arm_EmployeeId = Emt_EmployeeId
	                          AND e.Arm_TransactionId = 'OVERTIME'
	                         LEFT JOIN T_ApprovalRouteMaster AS a 
	                           ON a.Arm_RouteId = e.Arm_RouteId
	                        WHERE Emt_EmployeeId IN ({0})
	                          AND ISNULL(a.Arm_Checker1, '') = ''
	                          AND ISNULL(a.Arm_Checker2, '') = ''
	                          AND ISNULL(a.Arm_Approver, '') = ''
                              AND Convert(varchar,GETDATE(),101) BETWEEN e.Arm_StartDate AND ISNULL(e.Arm_EndDate, GETDATE())";
        #endregion
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, FormatForInQuery(empId)), CommandType.Text);
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
            
        return ds.Tables[0];
    }

    private string getMINOTHRExtension(string employeeId)//function not in use
    {
        string value = string.Empty;
        string sql = @"  declare @defaultValue as decimal(8,2)
                             SET @defaultValue = (SELECT Pmt_NumericValue
                                                    FROM T_ParameterMaster
                                                   WHERE Pmt_ParameterId = 'MINOTHR')
                                
                          SELECT ISNULL(Convert(varchar(10),Pmx_ParameterValue),@defaultValue)
                            FROM T_EmployeeMaster
                            LEFT JOIN T_ParameterMasterExt
                              ON Pmx_Classification = Emt_JobLevel
                             AND Pmx_ParameterId = 'MINOTHR'
                           WHERE Emt_EmployeeID = @EmployeeId";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", employeeId);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                value = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text, param));
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

        return value;

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

    private DataRow PopulateDR(string Status, string ControlNum, string empId, string BatchNum)
    {
        DataRow dr = DbRecord.Generate("T_EmployeeOvertime");
        //Andre: removed condition. ALWAYS retreive current. 20130702
        //if (methods.GetProcessControlFlag("OVERTIME", "CUT-OFF"))
        //{
        //    dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        //}
        //else
        //{
            dr["Eot_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        //}
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

        dr["Eot_EmployeeId"] = empId.ToUpper();
        dr["Eot_OvertimeDate"] = dtpOTDate.Date.ToString("MM/dd/yyyy");
        dr["Eot_Seqno"] = OTBL.GetOTSequence(dtpOTDate.Date.ToString("MM/dd/yyyy"), empId.ToUpper());
        dr["Eot_OvertimeType"] = ddlType.SelectedValue.ToUpper();
        dr["Eot_StartTime"] = txtOTStartTime.Text.Replace(":", "");
        dr["Eot_EndTime"] = txtOTEndTime.Text.Replace(":", "");//EndTime(txtOTStartTime.Text, txtOTHours.Text, empId.ToUpper()).ToUpper();
        dr["Eot_OvertimeHour"] = txtOTHours.Text.ToUpper();
        dr["Eot_Reason"] = txtReason.Text.ToUpper();
        dr["Eot_JobCode"] = string.Empty;//NOT yet applicable - txtJobCode.Text.ToString().ToUpper();
        dr["Eot_ClientJobNo"] = string.Empty;//NOT yet applicable - txtClientJobName.Text.ToString().ToUpper();
        dr["Eot_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Eot_CheckedBy"] = string.Empty;
        dr["Eot_Checked2By"] = string.Empty;
        dr["Eot_ApprovedBy"] = string.Empty;
        dr["Eot_Status"] = Status.ToUpper();
        dr["Eot_ControlNo"] = ControlNum;
        dr["Eot_OvertimeFlag"] = OTBL.ComputeOTFlag(dtpOTDate.Date.ToString("MM/dd/yyyy")).ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Eot_Filler1"] = filler1.ToUpper();
        dr["Eot_Filler2"] = filler2.ToUpper();
        dr["Eot_Filler3"] = filler3.ToUpper();
        dr["Eot_BatchNo"] = BatchNum;

        return dr;
    }

    private DataRow PopultateDRForRemarks(string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_TransactionRemarks");

        dr["Trm_ControlNo"] = ControlNum;
        dr["Trm_Remarks"] = string.Empty;
        dr["Usr_Login"] = Session["userLogged"].ToString();
        return dr;
    }

    private string EndTime(string start, string hours, string _empid)
    {
        DataSet dsEnd = new DataSet();
        string finalEndTimeHours = string.Empty;
        string finalEndTimeMins = string.Empty;

        string[] array = new string[2];
        array[0] = dtpOTDate.Date.ToString("MM/dd/yyyy");
        array[1] = _empid;
        dsEnd.Clear();

        #region Get TIMES
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            try
            {
                string sqlGetShift = @" if(select convert(char(10),Ppm_EndCycle,101) from dbo.T_PayPeriodMaster where Ppm_CycleIndicator='C')>'{0}'
                                         begin
                                         select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,Scm_PaidBreak,Scm_ScheduleType,
                                                    --Leave hours in 1st half  
                                                      convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                      ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                    --Leave hours in 2nd half
                                                      convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                         (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                           from T_ShiftCodeMaster where  Scm_ShiftCode=
                                             (select Ell_ShiftCode from T_EmployeeLogLedger  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'
                                              union
                                              select Ell_ShiftCode from T_EmployeeLogLedgerHist  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}')                                                  
                                         end
                                       else
                                         begin 
                                         select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,Scm_PaidBreak,Scm_ScheduleType,
                                                    --Leave hours in 1st half  
                                                      convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                      ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                    --Leave hours in 2nd half
                                                      convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                         (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                           from T_ShiftCodeMaster where  Scm_ShiftCode=(select emt_shiftcode from t_employeemaster where emt_employeeid='{1}')
                                         end";

                string sqlDayCode = @"   select ell_daycode from t_employeelogledger
                                                        where ell_employeeid  = '{1}'
                                                        and ell_processdate ='{0}'
                                                        union
                                                        select ell_daycode from t_employeelogledgerhist
                                                        where ell_employeeid  = '{1}'
                                                        and ell_processdate = '{0}'
                                                    ";

                dsEnd = dal.ExecuteDataSet(string.Format(sqlGetShift, array), CommandType.Text);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        #endregion

            //Start computation of endtime
            try
            {
                string shiftTimeIn = dsEnd.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString();
                string shiftBreakStart = dsEnd.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString();
                string shiftBreakEnd = dsEnd.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString();
                string shiftTimeOut = dsEnd.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString();
                string shiftPaidBreak = dsEnd.Tables[0].Rows[0]["Scm_PaidBreak"].ToString();
                string shiftSchedType = dsEnd.Tables[0].Rows[0]["Scm_ScheduleType"].ToString();

                int intTimeIn = (Convert.ToInt32(shiftTimeIn.Substring(0, 2)) * 60) + Convert.ToInt32(shiftTimeIn.Substring(2, 2));
                int intBreakStart = (Convert.ToInt32(shiftBreakStart.Substring(0, 2)) * 60) + Convert.ToInt32(shiftBreakStart.Substring(2, 2));
                int intBreakEnd = (Convert.ToInt32(shiftBreakEnd.Substring(0, 2)) * 60) + Convert.ToInt32(shiftBreakEnd.Substring(2, 2));
                int intTimeOut = (Convert.ToInt32(shiftTimeOut.Substring(0, 2)) * 60) + Convert.ToInt32(shiftTimeOut.Substring(2, 2));
                int intPaidBreak = Convert.ToInt32(shiftPaidBreak);


                if (shiftSchedType.Equals("G"))
                {
                    #region Add 1440 minutes(24 Hours) for Graveyard shift
                    if (intBreakStart < intTimeIn)
                    {
                        intBreakStart += 1440;
                    }
                    if (intBreakEnd < intTimeIn)
                    {
                        intBreakEnd += 1440;
                    }
                    if (intTimeOut < intTimeIn)
                    {
                        intTimeOut += 1440;
                    }
                    #endregion
                }

                int actualTimeIn = (Convert.ToInt32(start.Replace(":", "").Substring(0, 2)) * 60) + Convert.ToInt32(start.Replace(":", "").Substring(2, 2));
                int actualTimeOut = actualTimeIn + Convert.ToInt32((Convert.ToDecimal(hours) * 60));
                int excessFromPaid = 0;
                int intFinalTime = actualTimeOut;

                if (actualTimeIn <= intBreakStart && actualTimeOut >= intBreakEnd)
                {
                    intFinalTime = intFinalTime + (intBreakEnd - intBreakStart - intPaidBreak);
                }
                if (actualTimeOut > intBreakStart && actualTimeOut <= intBreakEnd)
                {

                    excessFromPaid = (actualTimeOut - intBreakStart - intPaidBreak);
                    if (excessFromPaid > 0)
                    {
                        intFinalTime = intBreakEnd + excessFromPaid;
                    }
                }
                if (shiftSchedType.Equals("G") && intFinalTime > 1440)
                {
                    intFinalTime -= 1440;
                }

                //Start to format for saving. Format sample '0700' not '07:00'
                finalEndTimeHours = Convert.ToString(intFinalTime / 60);
                finalEndTimeMins = Convert.ToString(intFinalTime - ((intFinalTime / 60) * 60));

                if (finalEndTimeHours.Length < 2)
                {
                    finalEndTimeHours = "0" + finalEndTimeHours;
                }
                if (finalEndTimeMins.Length < 2)
                {
                    finalEndTimeMins = "0" + finalEndTimeMins;
                }
                //End formatting
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
        }
        return finalEndTimeHours + finalEndTimeMins;
    }

    private void RepopulateList()
    {
        //lbxInclude.Items.Clear();
        //for (int i = 0; i < dgvReview.Rows.Count; i++)
        //{
        //    lbxInclude.Items.Add(new ListItem(dgvReview.Rows[i].Cells[1].Text, dgvReview.Rows[i].Cells[0].Text));
        //}
        txtSearch_TextChanged(null, null);
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
            //ddlType.Attributes.Remove("disabled");
			ddlType.Enabled = true;
            txtOTStartTime.Attributes.Remove("readOnly");;
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
            //ddlType.Attributes.Remove("disabled");
            //ddlType.Attributes.Add("disabled", "true");
			ddlType.Enabled = false;
            txtOTStartTime.Attributes.Remove("readOnly");
            txtOTStartTime.Attributes.Add("readOnly", "true");
            txtOTHours.Attributes.Remove("readOnly");
            txtOTHours.Attributes.Add("readOnly", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
        }
    }

    private DateTime getMinimumOTDate()
    {
        string sql = @" SELECT TOP 1 Ppm_StartCycle 
                          FROM T_PayPeriodMaster
                         WHERE Ppm_CycleIndicator = 'P'  
                         ORDER BY Ppm_PayPeriod DESC"; //retrieves first date of 1st past quincena
        DateTime returnDate = new DateTime();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                returnDate = Convert.ToDateTime(dal.ExecuteScalar(sql, CommandType.Text));
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
        return returnDate;
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
