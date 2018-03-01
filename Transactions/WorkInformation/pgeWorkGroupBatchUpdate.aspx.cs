/*File revision no. W2.1.00001 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_WorkInformation_pgeWorkGroupBatchUpdate : System.Web.UI.Page
{
    private WorkInformationBL WIBL = new WorkInformationBL();
    private MenuGrant MGBL = new MenuGrant();
    private CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBCHANGEGROUP")) 
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
                if (!hfPrevEffectivityDate.Value.Equals(dtpEffectivityDate.Date.ToShortDateString()))
                {
                    dtpEffectivityDate_Change(dtpEffectivityDate, new EventArgs());
                    hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToShortDateString();
                }
            }
            LoadComplete += new EventHandler(Transactions_WorkInformation_pgeWorkGroupBatchUpdate_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_WorkInformation_pgeWorkGroupBatchUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtFromGroupCode.Attributes.Add("readOnly", "true");
        txtToGroupCode.Attributes.Add("readOnly", "true");
        txtFromGroupDesc.Attributes.Add("readOnly", "true");
        txtToGroupDesc.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpEffectivityDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");

        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpEffectivityDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");

        btnToGroup.Attributes.Add("OnClick", "javascript:return lookupWorkGroup('TO', 'txtToGroup');"); 
        btnFromGroup.Attributes.Add("OnClick", "javascript:return lookupWorkGroup('FROM', 'txtFromGroup');");

        disableControlsOnView();
    }

    protected void dtpEffectivityDate_Change(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);
    }

    protected void txtFromGroupCode_TextChanged(object sender, EventArgs e)
    {
        txtSearch_TextChanged(null, null);   
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        FillChoiceList((sender ==  null));
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

                            if (isValid)//Calendar trappings
                            {
                                temp = checkCalendarSetup(employees);
                                if (temp.Rows.Count > 0)
                                {
                                    isValid = false;
                                    lblErrorInfo.Text = "No calendar setup for " + txtToGroupCode.Text + ". List of affected employee(s).";
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
                                        string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                        string status = "3";//default only, still would change in loop...
                                        int counter = 0;
                                        string controlNumbers = string.Empty;

                                        // format ToGroup before saving, fromGroup has been formatted after it is selected in the Workgroup lookup
                                        formatWorkGroup(ref txtToGroupCode);

                                        for (int i = 0; i < lbxInclude.Items.Count; i++)
                                        {
                                            status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                            DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), lbxInclude.Items[i].Value.ToString(), batchNo);
                                            WIBL.CreateMVGroupRecord(dr, dal);
                                            controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                            counter++;
                                        }
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.GROUP;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            EMBL.InsertInfoForNotification(controlNumbers
                                                                            , Session["userLogged"].ToString()
                                                                            , dal);
                                        }
                                        //Menu Log
                                        SystemMenuLogBL.InsertAddLog("WFBCHANGEGROUP", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "");
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

    protected void btnDisregard_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW) || !methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
            {
                string[] employees = new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
                bool flag = true;
                DataTable temp = new DataTable();
                DataTable dtFinal = new DataTable();
                bool isValid = true;
                int indx = 0;
                for (int i = 0; i < lbxInclude.Items.Count; i++)
                {
                    flag = true;
                    for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
                    {
                        if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        employees[indx++] = lbxInclude.Items[i].Value.ToString();
                    }
                }

                if (employees.Length > 0)
                {
                    if (isValid)//Route trappings
                    {
                        temp = checkRoutes(employees);
                        if (temp.Rows.Count > 0)
                        {
                            isValid = false;
                            lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
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
                                string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                string status = "3";//default only...
                                int counter = 0;
                                string controlNumbers = string.Empty;
                                for (int i = 0; i < employees.Length; i++)
                                {
                                    status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                    DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                    WIBL.CreateMVGroupRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.GROUP;
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
                string[] employees = new string[lbxInclude.Items.Count - dgvReview.Rows.Count];
                bool flag = true;
                DataTable temp = new DataTable();
                DataTable dtFinal = new DataTable();
                bool isValid = true;
                int indx = 0;
                for (int i = 0; i < lbxInclude.Items.Count; i++)
                {
                    flag = true;
                    for (int ctr = 0; ctr < dgvReview.Rows.Count && flag; ctr++)
                    {
                        if (dgvReview.Rows[ctr].Cells[0].Text.Equals(lbxInclude.Items[i].Value.ToString()))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        employees[indx++] = lbxInclude.Items[i].Value.ToString();
                    }
                }

                if (employees.Length > 0)
                {
                    if (isValid)//Route trappings
                    {
                        temp = checkRoutes(employees);
                        if (temp.Rows.Count > 0)
                        {
                            isValid = false;
                            lblErrorInfo.Text = "The following employees has no / incomplete approval route setup.";
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
                                string batchNo = CommonMethods.GetControlNumber("MVBATCH");
                                string status = "3";//default only...
                                int counter = 0;
                                string controlNumbers = string.Empty;
                                for (int i = 0; i < employees.Length; i++)
                                {
                                    status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                    DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), employees[i], batchNo);
                                    WIBL.CreateMVGroupRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.GROUP;
                                    EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                    EMBL.InsertInfoForNotification(controlNumbers
                                                                    , Session["userLogged"].ToString()
                                                                    , dal);
                                }
                                RepopulateList();
                                pnlBound.Visible = true;
                                pnlReview.Visible = false;
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
    private void initializeControls()
    {
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

        hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToShortDateString();
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
        txtToGroupCode.Text = string.Empty;
        txtToGroupDesc.Text = string.Empty;

        txtFromGroupCode.Text = string.Empty;
        txtFromGroupDesc.Text = string.Empty;

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        hfPrevEffectivityDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeControls();
    }

    private void FillChoiceList(bool flag)
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
                         WHERE Ell_ProcessDate >= '{1}'
                            -- Filter Insertion -- 
                           {0}

                         ORDER BY [ForSort]", QueryFilter(), dtpEffectivityDate.Date.ToString("MM/dd/yyyy"));
        
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
        if(CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
        {
            sql = @"SELECT DISTINCT Emt_CostcenterCode [Code]
                         , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Desc]
                      FROM T_EmployeeMaster
                     WHERE LEFT(Emt_JobStatus,1) = 'A'
                     UNION
                     SELECT DISTINCT Emt_CostcenterCode [Code]
                          , dbo.getCostcenterFullnameV2(Emt_CostcenterCode) [Desc]
                       FROM T_EmployeeMasterHist
                      WHERE LEFT(Emt_JobStatus,1) = 'A'";
        }
        else
        {
            sql = string.Format(@"SELECT DISTINCT Uca_CostCenterCode [Code]
                                       , dbo.getCostcenterFullnameV2(Uca_CostCenterCode) [Desc]
                                    FROM T_UserCostcenterAccess
                                   INNER JOIN T_EmployeeMaster
                                      ON Emt_CostcenterCode = Uca_CostcenterCode
                                   WHERE Uca_UserCode = '{0}'
                                     AND Uca_SytemId = 'TIMEKEEP'
                                     AND Uca_Status = 'A'
                                     AND Uca_CostCenterCode <> 'ALL' 
                                   UNION
                                  SELECT DISTINCT Uca_CostCenterCode [Code]
                                       , dbo.getCostcenterFullnameV2(Uca_CostCenterCode) [Desc]
                                    FROM T_UserCostcenterAccess
                                   INNER JOIN T_EmployeeMasterHist
                                      ON Emt_CostcenterCode = Uca_CostcenterCode
                                   WHERE Uca_UserCode = '{0}'
                                     AND Uca_SytemId = 'TIMEKEEP'
                                     AND Uca_Status = 'A'
                                     AND Uca_CostCenterCode <> 'ALL'", Session["userLogged"].ToString());
        }
        DataSet ds = new DataSet();
        using(DALHelper dal = new DALHelper())
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
        ddlCostcenter.Items.Clear();
        ddlCostcenter.Items.Add(new ListItem("ALL", "ALL"));
        if(!CommonMethods.isEmpty(ds))
        {
            for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ddlCostcenter.Items.Add(new ListItem(ds.Tables[0].Rows[i]["Desc"].ToString(), ds.Tables[0].Rows[i]["Code"].ToString()));
            }
        }
        else
        {
            MessageBox.Show("No costcenter access retrieved.");
        }

    }
    protected string QueryFilter()
    {
        string filter = string.Empty;

        // format fromGroup before querying db
        formatWorkGroup(ref txtFromGroupCode);

        if (!txtFromGroupCode.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@"AND ( REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                                          + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)) = '{0}' ", txtFromGroupCode.Text);
        }
		if (ddlCostcenter.SelectedValue.Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" AND Emt_CostCenterCode IN (SELECT Uca_CostCenterCode
                                                                       FROM T_UserCostCenterAccess
                                                                      WHERE Uca_SytemID = 'TIMEKEEP'
                                                                        AND Uca_UserCode = '{0}'
                                                                        AND Uca_Status = 'A')", Session["userLogged"].ToString());
            }
        }
        else
        {
            filter += string.Format(@" AND Emt_CostCenterCode = '{0}'", ddlCostcenter.SelectedValue); 
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

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;

        #region Group
        if (txtFromGroupCode.Text.Equals(txtToGroupCode.Text))
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
	                            , Adt_AccountDesc [Status]
                            FROM T_Movement
                            JOIN T_EmployeeMaster
	                            ON Emt_EmployeeID = Mve_EmployeeId
                            JOIN T_AccountDetail 
	                            ON Adt_AccountCode = Mve_Status
	                            AND Adt_AccountType = 'WFSTATUS'
                            WHERE Mve_EmployeeId IN  ({0})
	                            AND Mve_Type = 'G'
	                            AND Mve_Status IN ('1','3','5','7')
	                            AND Mve_EffectivityDate = '{1}' ";
        #endregion
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, FormatForInQuery(empId), dtpEffectivityDate.Date.ToString("MM/dd/yyyy")), CommandType.Text);
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

    private DataTable checkCalendarSetup(string[] empId)
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        dt.Columns.Add("Employee ID");
        dt.Columns.Add("Employee Name");
        dt.Columns.Add("Process Date");
        #region SQL
        string sqlNoCalendarMasterSetup = @"SELECT ISNULL(Ell1.Ell_EmployeeId, Ell2.Ell_EmployeeId) [Employee ID]
                                                 , Emt_FirstName 
                                                 + ' '
                                                 + Emt_LastName [Employee Name]
                                                 , Convert(varchar(10), ISNULL(Ell1.Ell_ProcessDate, Ell2.Ell_ProcessDate), 101) [Date]
                                              FROM (SELECT '{0}' [TEMPCOL]) AS TEMP
                                              LEFT JOIN T_EmployeeGroup Emv1
                                                ON Emv1.Emv_EmployeeId = [TEMPCOL]
                                               AND Emv1.Emv_EffectivityDate = (SELECT TOP 1 Emv2.Emv_EffectivityDate
								                                                 FROM T_EmployeeGroup Emv2
							                                                    WHERE Emv2.Emv_EmployeeId = '{0}'
									                                              AND Emv2.Emv_EffectivityDate > '{1}'
								                                                ORDER BY Emv2.Emv_EffectivityDate DESC)
                                              LEFT JOIN T_EmployeeLogLedger Ell1
                                                ON Ell1.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell1.Ell_ProcessDate BETWEEN '{1}' AND ISNULL(dateadd(day, -1, Emv1.Emv_EffectivityDate), '12/12/9999')
                                              LEFT JOIN T_EmployeeLogLedgerHist Ell2
                                                ON Ell2.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell2.Ell_ProcessDate BETWEEN '{1}' AND ISNULL(dateadd(day, -1, Emv1.Emv_EffectivityDate), '12/12/9999')
                                              LEFT JOIN T_CalendarGroupTmp
                                                ON Cal_ProcessDate = ISNULL(Ell1.Ell_ProcessDate, Ell2.Ell_ProcessDate)
                                               AND Cal_WorkType = '{2}'
                                               AND Cal_WorkGroup = '{3}'
                                              LEFT JOIN T_EmployeeMaster
                                                ON Emt_EmployeeId = ISNULL(Ell1.Ell_EmployeeId, Ell2.Ell_EmployeeId)
                                             WHERE Cal_ProcessDate IS NULL";
        #endregion
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                for (int i = 0; i < empId.Length; i++)
                {
                    ds = new DataSet();
                    ds = dal.ExecuteDataSet(string.Format(sqlNoCalendarMasterSetup, empId[0]
                                                                                  , dtpEffectivityDate.Date.ToString("MM/dd/yyyy")
                                                                                  , txtToGroupCode.Text.Substring(0, 3).Trim()
                                                                                  , txtToGroupCode.Text.Substring(3, 3).Trim()), CommandType.Text);

                    if (!CommonMethods.isEmpty(ds))
                    {
                        for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                        {
                            dt.Rows.Add(dt.NewRow());
                            dt.Rows[dt.Rows.Count - 1]["Employee ID"] = ds.Tables[0].Rows[x]["Employee ID"].ToString();
                            dt.Rows[dt.Rows.Count - 1]["Employee Name"] = ds.Tables[0].Rows[x]["Employee Name"].ToString();
                            dt.Rows[dt.Rows.Count - 1]["Process Date"] = ds.Tables[0].Rows[x]["Date"].ToString();
                        }
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

        return dt;
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
        dr["Mve_Type"] = "G";

        dr["Mve_From"] = txtFromGroupCode.Text;
        dr["Mve_To"] = txtToGroupCode.Text;
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
            btnFromGroup.Attributes.Remove("disabled");
            btnToGroup.Attributes.Remove("disabled");
            txtReason.Attributes.Remove("readOnly");
        }
        else
        {
            img.Attributes.Remove("disabled");
            img.Attributes.Add("disabled", "true");
            btnFromGroup.Attributes.Remove("disabled");
            btnFromGroup.Attributes.Add("disabled", "true");
            btnToGroup.Attributes.Remove("disabled");
            btnToGroup.Attributes.Add("disabled", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
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
            catch(Exception ex)
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

    private void formatWorkGroup(ref TextBox workGroup)
    {
        if (workGroup.Text.Length != 6)
        {
            string[] toGroup = workGroup.Text.Trim().Split(' ');

            workGroup.Text = string.Empty;

            for (int h = 0; h < toGroup.Length; h++)
            {
                toGroup[h] = toGroup[h].Trim();

                for (int i = toGroup[h].Length; i < 3; i++)
                {
                    toGroup[h] = " " + toGroup[h];
                }
                workGroup.Text += toGroup[h];
            }
        }
    }
    #endregion
}
