using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_WorkInformation_pgeRestdayBatchUpdate : System.Web.UI.Page
{
    private WorkInformationBL WIBL = new WorkInformationBL();
    private CommonMethods methods = new CommonMethods();
    private MenuGrant MGBL = new MenuGrant();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBRESTDAY"))
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
                if (!hfDateFrom.Value.Equals(dtpFromDate.Date.ToShortDateString()))
                {
                    txtSearch_TextChanged(null, null);
                    hfDateFrom.Value = dtpFromDate.Date.ToShortDateString();
                }
                if(!hfDateTo.Value.Equals(dtpToDate.Date.ToShortDateString()))
                {
                    txtSearch_TextChanged(null, null);
                    hfDateTo.Value = dtpToDate.Date.ToShortDateString();
                }
            }
            LoadComplete += new EventHandler(Transactions_WorkInformation_pgeRestdayBatchUpdate_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_WorkInformation_pgeRestdayBatchUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        TextBox txtTemp = (TextBox)dtpFromDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        TextBox txtTemp2 = (TextBox)dtpToDate.Controls[0];
        txtTemp2.Attributes.Add("readOnly", "true");

        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpFromDate.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");

        System.Web.UI.WebControls.Calendar cal2 = new System.Web.UI.WebControls.Calendar();
        cal2 = (System.Web.UI.WebControls.Calendar)dtpToDate.Controls[3];
        cal2.Attributes.Add("OnClick", "javascript:__doPostBack();");

        disableControlsOnView();
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
                if (!methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
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

                                    for (int i = 0; i < lbxInclude.Items.Count; i++)
                                    {
                                        status = CommonMethods.getStatusRoute(lbxInclude.Items[i].Value.ToString(), "MOVEMENT", "ENDORSE TO CHECKER 1", dal);
                                        DataRow dr = PopulateDR(status, CommonMethods.GetControlNumber("MOVEMENT"), lbxInclude.Items[i].Value.ToString(), batchNo);
                                        WIBL.CreateMVRecord(dr, dal);
                                        controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                        counter++;
                                    }
                                    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    {
                                        EmailNotificationBL EMBL = new EmailNotificationBL();
                                        EMBL.TransactionProperty = EmailNotificationBL.TransactionType.RESTDAY;
                                        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                        EMBL.InsertInfoForNotification(controlNumbers
                                                                        , Session["userLogged"].ToString()
                                                                        , dal);
                                    }
                                    //MenuLog
                                    SystemMenuLogBL.InsertEditLog("WFBRESTDAY", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "", false);
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
            if (!methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
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
                                    WIBL.CreateMVRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.RESTDAY;
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
            if (!methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
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
                                    WIBL.CreateMVRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.RESTDAY;
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
        string[] MinMaxDate = new string[2];
        MinMaxDate = CommonMethods.GetMinMaxDateOfFiling();

        try
        {
            dtpFromDate.Date = Convert.ToDateTime(GetFirstRestDate());
            dtpFromDate.MinDate = Convert.ToDateTime(MinMaxDate[0]);
            dtpFromDate.MaxDate = Convert.ToDateTime(MinMaxDate[1]);
            dtpToDate.Date = DateTime.Now;
            dtpToDate.MinDate = Convert.ToDateTime(MinMaxDate[0]);
            dtpToDate.MaxDate = Convert.ToDateTime(MinMaxDate[1]);
            
        }
        catch
        {
            MessageBox.Show("Failed to initialize minimum and maximum date of date pickers. Press OK to continue.");
        }

        hfDateFrom.Value = dtpFromDate.Date.ToShortDateString();
        hfDateTo.Value = dtpToDate.Date.ToShortDateString();
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

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        hfDateFrom.Value = string.Empty;
        hfDateTo.Value = string.Empty; 
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
        initializeControls();
    }

    private void FillChoiceList(bool flag)
    {
        DataTable dt = new DataTable();
        string sql = string.Format(@"  DECLARE @DSP as bit
                                           SET @DSP = (SELECT Pcm_ProcessFlag
                                                      FROM T_ProcessControlMaster
                                                     WHERE Pcm_ProcessId = 'DSPFULLNM')

                                        SELECT DISTINCT Emt_EmployeeId
                                             + '  -  '
                                             +  CASE WHEN (@DSP = '1')
                                                             THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
                                                             ELSE Emt_NickName
                                                         END AS [Employee Name]
                                             , Emt_EmployeeId
                                             , CASE WHEN (@DSP = '1')
                                                    THEN Emt_LastName + Emt_FirstName
                                                    ELSE Emt_NickName
                                                END AS [ForSort]
                                          FROM T_EmployeeMaster
                                         INNER JOIN T_EmployeeLogLedger L1
                                            ON L1.Ell_EmployeeID = Emt_EmployeeID
                                           AND (L1.Ell_DayCode = 'REST' AND L1.Ell_ProcessDate = '{1}') 
                                         INNER JOIN T_EmployeeLogLedger L2
                                            ON L2.Ell_EmployeeID = Emt_EmployeeID
                                           AND (L2.Ell_DayCode LIKE 'REG%' AND L2.Ell_ProcessDate = '{2}')
                                         WHERE 1 = 1
                                           AND Emt_JobStatus <> 'IN'
                                           {0}
                                             --Filter Insertion -- 
                                         ORDER BY [ForSort]", QueryFilter()
                                                            , dtpFromDate.Date.ToString("MM/dd/yyyy")
                                                            , dtpToDate.Date.ToString("MM/dd/yyyy"));

        if (dtpFromDate.Date != null && dtpToDate.Date != null)
        {
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
                lbxChoice.Items.Add(new ListItem(dt.Rows[ctr]["Employee Name"].ToString(), dt.Rows[ctr]["Emt_EmployeeId"].ToString()));
            }
            lblNoOfItemsChoice.Text = lbxChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfItemsInclude.Text = lbxInclude.Items.Count.ToString() + " no. of item(s)";
        }
    }

    private void fillDropDownCostcenter()
    {
        string sql = string.Empty;
        if (CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
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
        ddlCostcenter.Items.Clear();
        ddlCostcenter.Items.Add(new ListItem("ALL", "ALL"));
        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
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

        #region Restday
        if (dtpFromDate.Date.ToString("MM/dd/yyyy").Equals(dtpToDate.Date.ToString("MM/dd/yyyy")))
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

        if (methods.GetProcessControlFlag("TIMEKEEP", "CUT-OFF"))
        {
            dr["Mve_CurrentPayPeriod"] = CommonMethods.getPayPeriod("F");
        }
        else
        {
            dr["Mve_CurrentPayPeriod"] = CommonMethods.getPayPeriod("C");
        }
        dr["Mve_ControlNo"] = ControlNum;
        dr["Mve_EmployeeId"] = empId.ToUpper();
        dr["Mve_EffectivityDate"] = dtpToDate.Date.ToString("MM/dd/yyyy");
        dr["Mve_AppliedDate"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_Type"] = "R";

        dr["Mve_From"] = dtpFromDate.Date.ToString("MM/dd/yyyy");
        dr["Mve_To"] = dtpToDate.Date.ToString("MM/dd/yyyy");
        dr["Mve_Reason"] = txtReason.Text.ToString().ToUpper();
        dr["Mve_EndorsedDateToChecker"] = DateTime.Now.ToString("MM/dd/yyyy");
        dr["Mve_CheckedBy"] = string.Empty;
        dr["Mve_Checked2By"] = string.Empty;
        dr["Mve_ApprovedBy"] = string.Empty;
        dr["Mve_Status"] = Status.ToUpper();
        dr["Mve_BatchNo"] = BatchNum;
        dr["Mve_Flag"] = WIBL.ComputeMVFlag(dtpToDate.Date.ToString("MM/dd/yyyy")).ToUpper();
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
        img = (System.Web.UI.HtmlControls.HtmlImage)dtpFromDate.Controls[2];

        System.Web.UI.HtmlControls.HtmlImage imgTo = new System.Web.UI.HtmlControls.HtmlImage();
        imgTo = (System.Web.UI.HtmlControls.HtmlImage)dtpToDate.Controls[2];
        if (pnlBound.Visible)
        {
            img.Attributes.Remove("disabled");
            imgTo.Attributes.Remove("disabled");
            txtReason.Attributes.Remove("readOnly");
        }
        else
        {
            img.Attributes.Remove("disabled");
            img.Attributes.Add("disabled", "true");
            imgTo.Attributes.Remove("disabled");
            imgTo.Attributes.Add("disabled", "true");
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
            value[0] = ds.Tables[0].Rows[0]["MinDate"].ToString();
            value[1] = ds.Tables[0].Rows[0]["MaxDate"].ToString();
        }

        return value;
    }

    private string GetFirstRestDate()
    {
        string sql = @"DECLARE @dateMax as datetime
                           SET @dateMax = (SELECT Convert(varchar(10), Ppm_EndCycle, 101)
				                          FROM T_PayPeriodMaster
				                         WHERE Ppm_CycleIndicator = 'C')

                        SELECT TOP 1 Convert(varchar(10), Ell_ProcessDate, 101) FROM T_EmployeeLogLedger
                         WHERE Ell_DayCode = 'REST'
                           AND Ell_ProcessDate <= @dateMax";
        string date = DateTime.Now.ToString("MM/dd/yyyy");
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                date = dal.ExecuteScalar(sql, CommandType.Text).ToString();
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }

        return date;
    }
    #endregion
}