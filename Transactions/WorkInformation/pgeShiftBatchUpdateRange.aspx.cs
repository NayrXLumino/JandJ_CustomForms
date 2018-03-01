using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_WorkInformation_pgeShiftBatchUpdateRange : System.Web.UI.Page
{
    private WorkInformationBL WIBL = new WorkInformationBL();
    private MenuGrant MGBL = new MenuGrant();
    private CommonMethods methods = new CommonMethods();
    private bool routeError =false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBSHIFTUPDATE"))
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
                if (!hfPrevEffectivityDate.Value.Equals(dtpEffectivityDate.Date.ToString("MM/dd/yyyy")))
                {
                    dtpEffectivityDate_Change(dtpEffectivityDate, new EventArgs());
                    hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
                }
            }
            LoadComplete += new EventHandler(Transactions_WorkInformation_pgeShiftBatchUpdate_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_WorkInformation_pgeShiftBatchUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtFromShiftCode.Attributes.Add("readOnly", "true");
        txtFromShiftDesc.Attributes.Add("readOnly", "true");
        txtToShiftCode.Attributes.Add("readOnly", "true");
        txtToShiftDesc.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpEffectivityDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");

        TextBox txtTempTo = (TextBox)dtpEffectivityDateTo.Controls[0];
        txtTempTo.Attributes.Add("readOnly", "true");

        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpEffectivityDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");
        btnToShift.Attributes.Add("OnClick", "javascript:return lookupShift('TO','txtToShift');");
        btnFromShift.Attributes.Add("OnClick", "javascript:return lookupShift('FROM','txtFromShift');");

        disableControlsOnView();
    }

    protected void dtpEffectivityDate_Change(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);
    }

    protected void txtFromShiftCode_TextChanged(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {

        FillChoiceList((sender == null));
        for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
        {
            lbxChoice.Items.Remove(lbxInclude.Items[ctr]);
        }

        lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
    }

    protected void ddlCostcenter_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);
    }

    protected void btnIncludeIndi_Click(object sender, EventArgs e)
    {
        if (lbxChoice.SelectedIndex > -1)
        {
            lbxInclude.Items.Add(lbxChoice.SelectedItem);
            lbxChoice.Items.Remove(lbxChoice.SelectedItem);
            lbxChoice.SelectedIndex = -1;
            lbxInclude.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
            hfSaveOrEndorse.Value = "";
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

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
            hfSaveOrEndorse.Value = "";
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

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
            hfSaveOrEndorse.Value = "";
        }
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        while (lbxInclude.Items.Count > 0)
        {
            lbxChoice.Items.Add(lbxInclude.Items[0]);
            lbxInclude.Items.Remove(lbxInclude.Items[0]);
            lbxInclude.SelectedIndex = -1;
            lbxChoice.SelectedIndex = -1;

            lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
            hfSaveOrEndorse.Value = "";
        }
    }
    public void Delete()
    {
        if (hfBatch.Value != null && hfBatch.Value != "")
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    dal.ExecuteNonQuery(string.Format(@"DELETE T_Movement
                                                        WHERE Mve_BatchNo='{0}'",hfBatch.Value.ToString()));
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                { }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //save hidden
        //hfShiftType.Value = ddlType.;

        //     robertjayre
        //param[2] = new ParameterInfo("@endTime", txtLVEndTime.Text.Replace(":", ""));
        //param[3] = new ParameterInfo("@LeaveType", ddlType.SelectedValue.ToUpper().Substring(0, 2));
        //delete previous
        //DeletePrev();
        
        //this.Process("S");

        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                {
                    if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW) || !methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
                    {
                        //delete
                        Delete();
                        string errMsg1 = checkEntry1();
                        if (errMsg1.Equals(string.Empty))
                        {
                            //array to check individual employees
                            string[] employees = null;
                            employees = new string[lbxInclude.Items.Count];
                            for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                            {
                                employees[ctr] = lbxInclude.Items[ctr].Value;
                            }
                            //end

                            //More checking against DB values
                            bool isValid = true;
                            DataTable temp = new DataTable();
                            DataTable dtFinal = new DataTable();

                            if (isValid)//Route trappings
                            {
                                temp = checkRoutes(employees);
                                if (temp.Rows.Count > 0)
                                {
                                    isValid = false;
                                    lblErrorInfo.Text = "The following employees has no/incomplete approval route setup.";
                                    dgvReview.DataSource = temp;
                                    dgvReview.DataBind();
                                }
                            }

                            if (isValid)//for change in shift
                            {
                                temp = checkNoShiftChange(employees, dtpEffectivityDate.Date.ToString("MM/dd/yyyy"), dtpEffectivityDateTo.Date.ToString("MM/dd/yyyy"), txtToShiftCode.Text);
                                if (temp.Rows.Count > 0)
                                {
                                    isValid = false;
                                    lblErrorInfo.Text = "The following employees has no change in shift.";
                                    dgvReview.DataSource = temp;
                                    dgvReview.DataBind();
                                }
                            }

                            if (isValid)//Perth Added 09/09/2012 for duplicates
                            {
                                temp = checkDuplicates(employees);
                                if (temp.Rows.Count > 0)
                                {
                                    isValid = false;
                                    lblErrorInfo.Text = "The following employees has already a transaction on route.";
                                    dgvReview.DataSource = temp;
                                    dgvReview.DataBind();
                                }
                            }

                            //end
                            if (!isValid)
                            {
                                pnlBound.Visible = false;
                                pnlReview.Visible = true;
                                MessageBox.Show("Review the following entries.");
                            }
                            else
                            {
                                using (DALHelper dal = new DALHelper())
                                {
                                    try
                                    {
                                        dal.OpenDB();
                                        dal.BeginTransactionSnapshot();
                                        string batchNo = string.Empty;
                                        //if (hfBatch.Value != null && hfBatch.Value != "")
                                        //{
                                        //     batchNo = hfBatch.Value.ToString();
                                        //}
                                        //else
                                            batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                        hfBatch.Value = batchNo;
                                        string status = "1";//default only, still would change in loop...
                                        int counter = 0;
                                        string controlNumbers = string.Empty;
                                        //CREATION
                                        //if (hfBatch.Value != null && hfBatch.Value != "")
                                        //{
                                            for (int i = 0; i < lbxInclude.Items.Count; i++)
                                            {
                                                //status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                                //DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), lbxInclude.Items[i].Value.ToString(), batchNo);
                                                DateTime dt = dtpEffectivityDate.Date;
                                                while (dt <= dtpEffectivityDateTo.Date)
                                                {
                                                    DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), lbxInclude.Items[i].Value.ToString(), batchNo);
                                                    dr["Mve_EffectivityDate"] = dt.ToString("MM/dd/yyyy");
                                                    WIBL.CreateMVShiftRecord(dr, dal);
                                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                                    counter++;
                                                    dt = dt.AddDays(1);
                                                }
                                            }
                                        // }
                                        //else//UPDATE
                                        //{ 
                                        //    WIBL.UpdateMVRecord(
                                        //}
                                       
                                        //restoreDefaultControls();
                                        MessageBox.Show("Successfully saved " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
                                         //MenuLog
                                        SystemMenuLogBL.InsertAddLog("WFBSHIFTUPDATE", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "",false);
                                        hfPrevReason.Value = txtReason.Text;
                                        hfPrevEffectivityDateTo.Value = dtpEffectivityDateTo.Date.ToString("MM/dd/yyyy");
                                        hfPrevShift.Value = txtToShiftCode.Text;
                                        hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
                                        hfSaveOrEndorse.Value = "Save";
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
                        }
                        else
                        {
                            MessageBox.Show(errMsg1);
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("MOVEMENT"));
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

    }

    protected void btnEndorse_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                {
                    if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW) || !methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
                    {
                        //
                        if (hfSaveOrEndorse.Value == "Save" && hfPrevReason.Value == txtReason.Text && hfPrevEffectivityDate.Value.ToString() == dtpEffectivityDate.Date.ToString("MM/dd/yyyy")
                           && hfPrevEffectivityDateTo.Value.ToString() == dtpEffectivityDateTo.Date.ToString("MM/dd/yyyy") && hfPrevShift.Value == txtToShiftCode.Text)
                        {
                            string errMsg1 = checkEntry1();
                            if (errMsg1.Equals(string.Empty))
                            {
                                //array to check individual employees
                                string[] employees = null;
                                employees = new string[lbxInclude.Items.Count];
                                for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                                {
                                    employees[ctr] = lbxInclude.Items[ctr].Value;
                                }
                                //end

                                //More checking against DB values
                                bool isValid = true;
                                DataTable temp = new DataTable();
                                DataTable dtFinal = new DataTable();

                                if (isValid)//Route trappings
                                {
                                    temp = checkRoutes(employees);
                                    if (temp.Rows.Count > 0)
                                    {
                                        isValid = false;
                                        lblErrorInfo.Text = "The following employees has no/incomplete approval route setup.";
                                        dgvReview.DataSource = temp;
                                        dgvReview.DataBind();
                                    }
                                }

                                //if (isValid)//Perth Added 09/09/2012 for duplicates
                                //{
                                //    temp = checkDuplicates(employees);
                                //    if (temp.Rows.Count > 0)
                                //    {
                                //        isValid = false;
                                //        lblErrorInfo.Text = "The following employees has already a transaction on route.";
                                //        dgvReview.DataSource = temp;
                                //        dgvReview.DataBind();
                                //    }
                                //}

                                //end
                                if (!isValid)
                                {
                                    pnlBound.Visible = false;
                                    pnlReview.Visible = true;
                                    MessageBox.Show("Review the following entries.");
                                }
                                else
                                {
                                    using (DALHelper dal = new DALHelper())
                                    {
                                        try
                                        {
                                            dal.OpenDB();
                                            dal.BeginTransactionSnapshot();

                                            //string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                            string status = "3";//default only, still would change in loop...
                                            //get all data with corresponding batch Number
                                            DataSet dsDetails = dal.ExecuteDataSet(string.Format(@"SELECT Mve_ControlNo
		                                                                                                ,Mve_EmployeeId
		                                                                                                ,Mve_EffectivityDate
                                                                                                FROM T_Movement
                                                                                                WHERE Mve_BatchNo='{0}'
                                                                                                ", hfBatch.Value));
                                            //get all data with corresponding batch Number
                                            int counter = 0;
                                            string controlNumbers = string.Empty;
                                            //loop through the details
                                            if (dsDetails != null && dsDetails.Tables != null && dsDetails.Tables[0].Rows.Count > 0)
                                            {
                                                for (int i = 0; i < dsDetails.Tables[0].Rows.Count; i++)
                                                {
                                                    status = CommonMethods.getStatusRoute(dsDetails.Tables[0].Rows[i]["Mve_EmployeeId"].ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                                    //DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), lbxInclude.Items[i].Value.ToString(), batchNo);
                                                    //update every transaction basing on the status
                                                    dal.ExecuteNonQuery(string.Format(@"UPDATE T_Movement
                                                                                    SET Mve_EndorsedDateToChecker=GETDATE()
	                                                                                    , Mve_Status='{0}'
                                                                                    WHERE Mve_ControlNo='{1}'", status, dsDetails.Tables[0].Rows[i]["Mve_ControlNo"].ToString()));
                                                    counter++;
                                                }
                                            }
                                            if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                            {
                                                EmailNotificationBL EMBL = new EmailNotificationBL();
                                                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.SHIFT;
                                                EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                                EMBL.InsertInfoForNotification(controlNumbers
                                                                                , Session["userLogged"].ToString()
                                                                                , dal);
                                            }
                                            //MenuLog
                                            SystemMenuLogBL.InsertEditLog("WFBSHIFTUPDATE", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "",false);

                                            MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + hfBatch.Value);
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
                            }
                            else
                            {
                                MessageBox.Show(errMsg1);
                            }
                        }
                        else
                            MessageBox.Show("You have changes. Save first.");
                        //else
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("MOVEMENT"));
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
    }

    protected void btnDisregard_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW) || !methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
            {
                string[] employees = null;// new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
                bool flag = true;
                DataTable temp = new DataTable();
                DataTable dtFinal = new DataTable();
                bool isValid = true;
                int indx = 0;
                //for (int i = 0; i < lbxInclude.Items.Count; i++)
                //{
                //    flag = true;
                //    for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
                //    {
                //        if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
                //        {
                //            flag = false;
                //        }
                //    }
                //    if (flag)
                //    {
                //        employees[indx++] = lbxInclude.Items[i].Value.ToString();
                //    }
                //}
                employees = new string[lbxInclude.Items.Count];
                for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                {
                    employees[ctr] = lbxInclude.Items[ctr].Value;
                }
                if (employees.Length > 0)
                {
                    if (isValid)//Route trappings
                    {
                        //temp = checkRoutes(employees);
                    //    if (temp.Rows.Count > 0)
                    //    {
                    //        isValid = false;
                    //        lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
                    //        dgvReview.DataSource = temp;
                    //        dgvReview.DataBind();
                    //    }
                    }
                    //end
                    if (!isValid)
                    {
                        pnlBound.Visible = false;
                        pnlReview.Visible = true;
                        MessageBox.Show("Review the following entries.");
                    }
                    else
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                dal.BeginTransactionSnapshot();
                                string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                string status = "3";//default only...
                                int counter = 0;
                                string controlNumbers = string.Empty;
                                for (int i = 0; i < employees.Length; i++)
                                {
                                    //status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                    //DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                    //WIBL.CreateMVShiftRecord(dr, dal);
                                    //controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    //counter++;
                                    //
                                    DateTime dt = dtpEffectivityDate.Date;

                                    while (dt <= dtpEffectivityDateTo.Date)
                                    {
                                        bool insert = true;
                                        for (int ctr = 0; ctr < dgvReview.Rows.Count && insert; ctr++)
                                        {
                                            if (routeError && dgvReview.Rows[ctr].Cells[0].Text.Equals(employees[i]))
                                                insert = false;
                                            if (insert && dgvReview.Rows[ctr].Cells[0].Text.Equals(employees[i]) && dgvReview.Rows[ctr].Cells[3].Text == dt.ToString("MM/dd/yyyy"))
                                            {
                                                insert = false;
                                            }
                                        }
                                        if (insert)
                                        {
                                            status = CommonMethods.getStatusRoute(employees[i], "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                            DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                            dr["Mve_EffectivityDate"] = dt.ToString("MM/dd/yyyy");
                                            WIBL.CreateMVShiftRecord(dr, dal);
                                            controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                            counter++;
                                        }
                                        dt = dt.AddDays(1);
                                    }

                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.SHIFT;
                                    EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    EMBL.InsertInfoForNotification(controlNumbers
                                                                    , Session["userLogged"].ToString()
                                                                    , dal);
                                }
                                restoreDefaultControls();
                                MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
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
                    MessageBox.Show("No selected employees to file. \n Please cancel transaction.");
                }
            }
            else
            {
                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("MOVEMENT"));
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//This will discard error list and just proceed to other transactions

    protected void btnKeep_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW) || !methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
            {
                string[] employees = null;//new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
                bool flag = true;
                DataTable temp = new DataTable();
                DataTable dtFinal = new DataTable();
                bool isValid = true;
                int indx = 0;
                //for (int i = 0; i < lbxInclude.Items.Count; i++)
                //{
                //    flag = true;
                //    for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
                //    {
                //        if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
                //        {
                //            flag = false;
                //        }
                //    }
                //    if (flag)
                //    {
                //        employees[indx++] = lbxInclude.Items[i].Value.ToString();
                //    }
                //}
                employees = new string[lbxInclude.Items.Count];
                for (int ctr = 0; ctr < lbxInclude.Items.Count; ctr++)
                {
                    employees[ctr] = lbxInclude.Items[ctr].Value;
                }

                if (employees.Length > 0)
                {
                    if (isValid)//Route trappings
                    {
                        //temp = checkRoutes(employees);
                        //if (temp.Rows.Count > 0)
                        //{
                        //    isValid = false;
                        //    lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
                        //    dgvReview.DataSource = temp;
                        //    dgvReview.DataBind();
                        //}
                    }
                    //end
                    if (!isValid)
                    {
                        pnlBound.Visible = false;
                        pnlReview.Visible = true;
                        MessageBox.Show("Review the following entries.");
                    }
                    else
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                dal.BeginTransactionSnapshot();
                                string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                string status = "3";//default only...
                                int counter = 0;
                                string controlNumbers = string.Empty;
                                for (int i = 0; i < employees.Length; i++)
                                {
                                    //status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                    //DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                    //WIBL.CreateMVShiftRecord(dr, dal);
                                    //controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    //counter++;
                                    //loop through date
                                    DateTime dt = dtpEffectivityDate.Date;
                                    
                                    while (dt <= dtpEffectivityDateTo.Date)
                                    {
                                        bool insert = true;
                                        for (int ctr = 0; ctr < dgvReview.Rows.Count && insert; ctr++)
                                        {

                                            if (routeError && dgvReview.Rows[ctr].Cells[0].Text.Equals(employees[i]))
                                                insert = false;
                                            if (insert && dgvReview.Rows[ctr].Cells[0].Text.Equals(employees[i]) && dgvReview.Rows[ctr].Cells[3].Text == dt.ToString("MM/dd/yyyy"))
                                            {
                                                insert = false;
                                            }
                                        }
                                        if (insert)
                                        {
                                            status = CommonMethods.getStatusRoute(employees[i], "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                            DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                            dr["Mve_EffectivityDate"] = dt.ToString("MM/dd/yyyy");
                                            WIBL.CreateMVShiftRecord(dr, dal);
                                            controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                            counter++;
                                        }
                                        dt = dt.AddDays(1);
                                    }
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.SHIFT;
                                    EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    EMBL.InsertInfoForNotification(controlNumbers
                                                                    , Session["userLogged"].ToString()
                                                                    , dal);
                                }
                                //RepopulateList();
                                //pnlBound.Visible = true;
                                //pnlReview.Visible = false;
                                btnCancel.Enabled = false;
                                btnKeep.Enabled = false;
                                btnDisregard.Enabled = false;
                                MessageBox.Show("Successfully endorsed " + counter.ToString() + " transaction(s). \nBatch No: " + batchNo);
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
                    MessageBox.Show("No selected employees to file. \n Please cancel transaction.");
                }
            }
            else
            {
                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("MOVEMENT"));
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//This will keep error list back to lbxInclude so that user may do saom changes(ex. Start Time)

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            pnlBound.Visible = true;
            pnlReview.Visible = false;
            //MenuLog
            SystemMenuLogBL.InsertAddLog("WFBSHIFTUPDATE", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "");
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
        //total cancellation of transaction
        
        if (hfBatch.Value != null && hfBatch.Value != "")
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    dal.ExecuteNonQuery(string.Format(@"UPDATE T_Movement
                                                        SET Mve_Status='2',
                                                        Usr_Login = '{1}',
                                                        Ludatetime = getdate()
                                                        WHERE Mve_BatchNo='{0}'", hfBatch.Value.ToString(), Session["userLogged"].ToString()));
                    dal.CommitTransactionSnapshot();
                    MessageBox.Show("Transaction(s) cancelled.");
                }
                catch (Exception ex)
                { }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
        //MenuLog
        SystemMenuLogBL.InsertDeleteLog("WFBSHIFTUPDATE", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "");
        MessageBox.Show("Cancelled Transaction(s)");
        restoreDefaultControls();
        initializeControls();
    
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        dtpEffectivityDate.Date = DateTime.Now;
        dtpEffectivityDateTo.Date = DateTime.Now;

        try
        {
            dtpEffectivityDate.MinDate = CommonMethods.getMinimumDateOfFiling();
            dtpEffectivityDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
            dtpEffectivityDateTo.MinDate = CommonMethods.getMinimumDateOfFiling();
            dtpEffectivityDateTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        }
        catch
        {
            MessageBox.Show("Failed to initialize minimum and maximum date of date pickers. Press OK to continue.");
        }

        hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;

        fillDropDownCostcenter();
        txtSearch_TextChanged(null, null);

        pnlBound.Visible = true;
        pnlReview.Visible = false;
    }

    private void restoreDefaultControls()
    {
        txtReason.Text = string.Empty;
        txtToShiftCode.Text = string.Empty;
        txtToShiftDesc.Text = string.Empty;
        
        txtFromShiftCode.Text = string.Empty;
        txtFromShiftDesc.Text = string.Empty;

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        hfPrevEffectivityDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";

        hfPrevEffectivityDate.Value = hfPrevEffectivityDateTo.Value = hfPrevReason.Value = hfSaveOrEndorse.Value = hfBatch.Value= string.Empty;
        hfSaveOrEndorse.Value = string.Empty;
        initializeControls();
    }

    private void FillChoiceList(bool flag)
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
                           {2}
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
                           {2}
                         WHERE Ell_ProcessDate = '{1}'
                            -- Filter Insertion -- 
                           {0}

                         ORDER BY [ForSort]", QueryFilter(), dtpEffectivityDate.Date.ToString("MM/dd/yyyy"), !hasCCLine ? "" : string.Format(@"---apsungahid added for line code access filter 20141121
                                                                                          
                                                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
								                                                                                                          FROM E_CostCenterLineMaster 
								                                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
					                                                                                                           ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                                                           AND '{0}' BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE())) ", dtpEffectivityDate.Date.ToString("MM/dd/yyyy")));

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
        if (flag)
        {
            lbxInclude.Items.Clear();
        }
        for (int ctr = 0; ctr < dt.Rows.Count; ctr++)
        {
            lbxChoice.Items.Add(new ListItem(dt.Rows[ctr]["Employee Name"].ToString(), dt.Rows[ctr]["Ell_EmployeeId"].ToString()));
        }
        lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
        lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
    }

    private void fillDropDownCostcenter()
    {
        string sql = string.Empty;
        if (CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
        {
            sql = @"SELECT DISTINCT Emt_CostcenterCode [Code]
                         , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Desc]
                      FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID
                     WHERE LEFT(Emt_JobStatus,1) = 'A'

                             AND Ell_ProcessDate = @ProcessDate


                             UNION

                    SELECT DISTINCT Emt_CostcenterCode [Code]
                         , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Desc]
                      FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeID
                     WHERE LEFT(Emt_JobStatus,1) = 'A'

                             AND Ell_ProcessDate = @ProcessDate

                             ORDER BY [Desc]";
        }
        else
        {
            sql = string.Format(@"SELECT DISTINCT Uca_CostCenterCode [Code]
                                       , dbo.getCostcenterFullnameV2(Uca_CostCenterCode) [Desc]
                                    FROM T_UserCostcenterAccess
                                   INNER JOIN T_EmployeeMaster
                                      ON Emt_CostcenterCode = Uca_CostcenterCode
                                   INNER JOIN T_EmployeeLogLedger
                                      ON Emt_EmployeeID = Ell_EmployeeID
                                   WHERE Uca_UserCode = @UserCode
                                     AND Uca_SytemId = 'TIMEKEEP'
                                     AND Uca_Status = 'A'
                                     AND Uca_CostCenterCode <> 'ALL' 
                                     AND Ell_ProcessDate = @ProcessDate
                                   UNION
                                  SELECT DISTINCT Uca_CostCenterCode [Code]
                                       , dbo.getCostcenterFullnameV2(Uca_CostCenterCode) [Desc]
                                    FROM T_UserCostcenterAccess
                                   INNER JOIN T_EmployeeMaster
                                      ON Emt_CostcenterCode = Uca_CostcenterCode
                                   INNER JOIN T_EmployeeLogLedgerHist
                                      ON Emt_EmployeeID = Ell_EmployeeID
                                   WHERE Uca_UserCode = @UserCode
                                     AND Uca_SytemId = 'TIMEKEEP'
                                     AND Uca_Status = 'A'
                                     AND Uca_CostCenterCode <> 'ALL'
                                     AND Ell_ProcessDate = @ProcessDate

                                    ORDER BY [Desc]");
        }
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[1] = new ParameterInfo("@ProcessDate", dtpEffectivityDate.Date.ToString("MM/dd/yyyy"));
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        ddlCostcenter.Items.Clear();
        ddlCostcenter.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem("ALL", "ALL"));
        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ddlCostcenter.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem(ds.Tables[0].Rows[i]["Desc"].ToString(), ds.Tables[0].Rows[i]["Code"].ToString()));
            }
            ddlCostcenter.Items[0].Selected = true;
        }
        else
        {
            MessageBox.Show("No costcenter access retrieved.");
        }

    }
    protected string QueryFilter()
    {
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string filter = string.Empty;

        if (!txtFromShiftCode.Text.Equals(string.Empty))
        {
            filter += string.Format(@"AND Ell_ShiftCode = '{0}' ", txtFromShiftCode.Text);
        }

        if (GetValue(ddlCostcenter.Value).Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = 'TIMEKEEP')
                                                                        OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }

            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND ( ISNULL(Ecm_LineCode, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Ecm_LineCode, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE (Ucl_CostCenterCode = Emt_CostCenterCode OR Ucl_CostCenterCode = 'ALL')
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'TIMEKEEP'
																															 AND Ucl_LineCode = ISNULL(Ecm_LineCode, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'TIMEKEEP'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                          OR 'ALL' IN ( SELECT Ucl_LineCode 
										                  FROM E_UserCostcenterLineAccess 
													     WHERE Ucl_CostCenterCode = 'ALL'
														   AND Ucl_Status = 'A'
														   AND Ucl_SystemID = 'TIMEKEEP'
														   AND Ucl_LineCode = 'ALL'
														   AND Ucl_UserCode = '{0}')
                                          OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        else if (ddlCostcenter.SelectedItem != null)
        {
            filter += string.Format(@"AND Emt_CostCenterCode = '{0}'", ddlCostcenter.SelectedItem.Value);

            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND ( ISNULL(Ecm_LineCode, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Ecm_LineCode, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'TIMEKEEP'
																															 AND Ucl_LineCode = ISNULL(Ecm_LineCode, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Emt_CostCenterCode
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'TIMEKEEP'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                          OR Emt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_LastName like '{0}%'
                                           OR Emt_FirstName like '{0}%'
                                           OR Emt_NickName like '{0}%'
                                           OR Emt_EmployeeId like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
        }
        return filter;
    }

    private string GetValue(object objValue)
    {
        return (objValue == null) ? string.Empty : objValue.ToString();
    }

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        if(dtpEffectivityDate.Date>dtpEffectivityDateTo.Date)
            err += "\n FROM process date cannot be greater than TO process date.";
        #region Shift
        if (txtFromShiftCode.Text.Equals(txtToShiftCode.Text))
        {
            err += "\n FROM and TO cannot be the same.";
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

        return err;
    }

    private DataTable checkRoutes(string[] empId)
    {
        routeError = false;
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
	                          AND e.Arm_TransactionId = 'MOVEMENT'
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
                if (ds.Tables[0].Rows.Count > 0)
                    routeError = true;
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

    //Perth Added 09/09/2012 for duplicate entries
    private DataTable checkDuplicates(string[] empId)
    {
        DataSet ds = new DataSet();
        #region SQL
        string sql = @"   SELECT
	                            Mve_EmployeeId [Employee ID] 
	                            , Emt_LastName + ', ' + Emt_FirstName [Employee Name]
	                            , Mve_ControlNo [Control No]
                                , convert(varchar(10),Mve_EffectivityDate,101) [Date]
	                            , Adt_AccountDesc [Status]
                            FROM T_Movement
                            JOIN T_EmployeeMaster
	                            ON Emt_EmployeeID = Mve_EmployeeId
                            JOIN T_AccountDetail 
	                            ON Adt_AccountCode = Mve_Status
	                            AND Adt_AccountType = 'WFSTATUS'
                            WHERE Mve_EmployeeId IN  ({0})
	                            AND Mve_Type = 'S'
	                            AND Mve_Status IN ('1','3','5','7')
	                            AND Mve_EffectivityDate between '{1}' and '{2}' ";
        //if (hfBatch.Value != null && hfBatch.Value != "")
        //{
        //    sql += " AND Mve_BatchNo!='{3}'";
        //}
        #endregion

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                //if (hfBatch.Value != null && hfBatch.Value != "")
                //{
                //    ds = dal.ExecuteDataSet(string.Format(sql, FormatForInQuery(empId), dtpEffectivityDate.Date.ToString("MM/dd/yyyy"), dtpEffectivityDateTo.Date.ToString("MM/dd/yyyy"),hfBatch.Value.ToString()), CommandType.Text);
                //}
                //else
                //{
                    ds = dal.ExecuteDataSet(string.Format(sql, FormatForInQuery(empId), dtpEffectivityDate.Date.ToString("MM/dd/yyyy"), dtpEffectivityDateTo.Date.ToString("MM/dd/yyyy")), CommandType.Text);
                //}
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

    private DataTable checkNoShiftChange(string[] employees, string fromDate, string toDate, string shiftCode)
    {
        DataSet ds = new DataSet();
        #region SQL
        string sql = @" SELECT Ell_EmployeeID [Employee ID]
                            , Emt_LastName + ', ' + Emt_FirstName [Employee Name]
                            , Ell_ProcessDate [Date]
                            , Ell_ShiftCode [Shift Code]
                            , Scm_ShiftDesc [Shift Description]
                        FROM T_EmployeeLogLedger
                        INNER JOIN T_EmployeeMaster
	                        ON Emt_EmployeeID = Ell_EmployeeID
                        LEFT JOIN T_ShiftCodeMaster
	                        ON Ell_ShiftCode = Scm_ShiftCode
                        WHERE Ell_EmployeeID IN ({0}) 
                        AND Ell_ProcessDate BETWEEN '{1}' AND '{2}'
                        AND Ell_ShiftCode = '{3}'

                        UNION

                        SELECT Ell_EmployeeID [Employee ID]
                            , Emt_LastName + ', ' + Emt_FirstName [Employee Name]
                            , Ell_ProcessDate [Date]
                            , Ell_ShiftCode [Shift Code]
                            , Scm_ShiftDesc [Shift Description]
                        FROM T_EmployeeLogLedgerHist
                        INNER JOIN T_EmployeeMaster
	                        ON Emt_EmployeeID = Ell_EmployeeID
                        LEFT JOIN T_ShiftCodeMaster
	                        ON Ell_ShiftCode = Scm_ShiftCode
                        WHERE Ell_EmployeeID IN ({0}) 
                        AND Ell_ProcessDate BETWEEN '{1}' AND '{2}'
                        AND Ell_ShiftCode = '{3}'
                         ";
        #endregion

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, FormatForInQuery(employees), fromDate, toDate, shiftCode), CommandType.Text);
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

    protected string FormatForInQuery(string[] array)
    {
        string retVal = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
            {
                retVal += ",";
            }
            retVal += "'" + array[i] + "'";
        }
        return retVal;
    }

    private DataRow PopulateDR(string Status, string ControlNum, string empId, string BatchNum)
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
        dr["Mve_ControlNo"] = ControlNum;
        dr["Mve_EmployeeId"] = empId.ToUpper();
        dr["Mve_EffectivityDate"] = dtpEffectivityDate.Date.ToString("MM/dd/yyyy");
        dr["Mve_AppliedDate"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_Type"] = "S";

        dr["Mve_From"] = txtFromShiftCode.Text;
        dr["Mve_To"] = txtToShiftCode.Text;
        dr["Mve_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["Mve_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_CheckedBy"] = string.Empty;
        dr["Mve_Checked2By"] = string.Empty;
        dr["Mve_ApprovedBy"] = string.Empty;
        dr["Mve_Status"] = Status.ToUpper();
        dr["Mve_BatchNo"] = BatchNum;
        dr["Mve_Flag"] = WIBL.ComputeMVFlag(dtpEffectivityDate.Date.ToString("MM/dd/yyyy")).ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();


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

    private void RepopulateList()
    {
        lbxInclude.Items.Clear();
        for (int i = 0; i < dgvReview.Rows.Count; i++)
        {
            lbxInclude.Items.Add(new ListItem(dgvReview.Rows[i].Cells[1].Text, dgvReview.Rows[i].Cells[0].Text));
        }
        txtSearch_TextChanged(null, null);
    }

    private void disableControlsOnView()
    {
        System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
        img = (System.Web.UI.HtmlControls.HtmlImage)dtpEffectivityDate.Controls[2];
        if (pnlBound.Visible)
        {
            img.Attributes.Remove("disabled");
            btnFromShift.Attributes.Remove("disabled");
            btnToShift.Attributes.Remove("disabled");
            txtReason.Attributes.Remove("readOnly");
        }
        else
        {
            img.Attributes.Remove("disabled");
            img.Attributes.Add("disabled", "true");
            btnFromShift.Attributes.Remove("disabled");
            btnFromShift.Attributes.Add("disabled", "true");
            btnToShift.Attributes.Remove("disabled");
            btnToShift.Attributes.Add("disabled", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
        }

        System.Web.UI.HtmlControls.HtmlImage img2 = new System.Web.UI.HtmlControls.HtmlImage();
        img2 = (System.Web.UI.HtmlControls.HtmlImage)dtpEffectivityDateTo.Controls[2];
        if (pnlBound.Visible)
        {
            img2.Attributes.Remove("disabled");
        }
        else
        {
            img2.Attributes.Remove("disabled");
            img2.Attributes.Add("disabled", "true"); ;
        }
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
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
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
