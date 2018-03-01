/*File revision no. W2.1.00001 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_WorkInformation_pgeCostCenterBatchUpdate : System.Web.UI.Page
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
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBCCUPDATE"))
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
            //else
            //{
            //    if (!hfPrevEffectivityDate.Value.Equals(dtpEffectivityDate.Date.ToShortDateString()))
            //    {
            //        dtpEffectivityDate_Change(dtpEffectivityDate, new EventArgs());
            //        hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToShortDateString();
            //    }
            //}
            LoadComplete += new EventHandler(Transactions_WorkInformation_pgeCostCenterBatchUpdate_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_WorkInformation_pgeCostCenterBatchUpdate_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtFromCostCenterCode.Attributes.Add("readOnly", "true");
        txtFromCostCenterDesc.Attributes.Add("readOnly", "true");
        txtToCostCenterCode.Attributes.Add("readOnly", "true");
        txtToCostCenterDesc.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpEffectivityDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");

        //System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        //cal = (System.Web.UI.WebControls.Calendar)dtpEffectivityDate.Controls[3];
        //cal.Attributes.Add("OnClick", "javascript:__doPostBack();");

        btnToCostCenter.Attributes.Add("OnClick", "javascript:return lookupCostCenter(" + "'TO', 'txtToCostCenter');");   // temp
        btnFromCostCenter.Attributes.Add("OnClick", "javascript:return lookupCostCenter(" + "'FROM', 'txtFromCostCenter');");

        txtFromCostCenterCode.Attributes.Add("onFocus", "javascript:__doPostBack();");

        disableControlsOnView();
    }

    //protected void dtpEffectivityDate_Change(object sender, EventArgs e)
    //{
    //    txtSearch_TextChanged(null, null);
    //}

    protected void txtFromCostCenterCode_TextChanged(object sender, EventArgs e)
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
                if(!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
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
                                            WIBL.CreateMVCostCenterRecord(dr, dal);
                                            controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                            counter++;
                                        }
                                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                        {
                                            EmailNotificationBL EMBL = new EmailNotificationBL();
                                            EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
                                            EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                            EMBL.InsertInfoForNotification(controlNumbers
                                                                            , Session["userLogged"].ToString()
                                                                            , dal);
                                        }
                                        //MenuLog
                                        SystemMenuLogBL.InsertEditLog("WFBCCUPDATE", true, Session["userLogged"].ToString(), Session["userLogged"].ToString(), "",false);

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
                                    WIBL.CreateMVCostCenterRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
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
                                    WIBL.CreateMVCostCenterRecord(dr, dal);
                                    controlNumbers += dr["Mve_ControlNo"].ToString() + ",";
                                    counter++;
                                }
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.COSTCENTER;
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
        //DataTable dt = GetMinMaxDate();
        MinMaxDate = CommonMethods.GetMinMaxDateOfFiling();
        try
        {
            //dtpEffectivityDate.MinDate = Convert.ToDateTime(dt.Rows[0]["MinDate"].ToString());
            //dtpEffectivityDate.MaxDate = Convert.ToDateTime(dt.Rows[0]["MaxDate"].ToString());
            dtpEffectivityDate.MinDate = Convert.ToDateTime(MinMaxDate[0]);
            dtpEffectivityDate.MaxDate = Convert.ToDateTime(MinMaxDate[1]);
        }
        catch
        {
            MessageBox.Show("Failed to initialize minimum and maximum date of date pickers. Press OK to continue.");
        }

        //txtFromCostCenterCode.Text = GetDefaultCCCode();
        if(!txtFromCostCenterCode.Text.Equals(string.Empty))
            txtFromCostCenterDesc.Text = GetCCDescription(txtFromCostCenterCode.Text);

        //hfPrevEffectivityDate.Value = dtpEffectivityDate.Date.ToShortDateString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;

        txtSearch_TextChanged(null, null);

        pnlBound.Visible = true;
        pnlReview.Visible = false;
    }

    private void restoreDefaultControls()
    {
        txtReason.Text = string.Empty;
        txtToCostCenterCode.Text = string.Empty;
        txtToCostCenterDesc.Text = string.Empty;

        txtFromCostCenterCode.Text = string.Empty;
        txtFromCostCenterDesc.Text = string.Empty;

        lbxChoice.Items.Clear();
        lbxInclude.Items.Clear();

        //hfPrevEffectivityDate.Value = string.Empty;
        hfPrevEntry.Value = string.Empty;
        hfSaved.Value = "0";
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
                             , Emt_EmployeeId
                             , CASE WHEN (@DSP = '1')
                                    THEN Emt_LastName + Emt_FirstName
                                    ELSE Emt_NickName
                                END AS [ForSort]
                          FROM T_EmployeeMaster
                            {1}
                         WHERE LEFT(Emt_JobStatus, 1) = 'A'
                            -- Filter Insertion -- 
                           {0}

                         ORDER BY [ForSort]", QueryFilter(), !hasCCLine  ? "" : string.Format(@"---apsungahid added for line code access filter 20141121
                                                                                          
                                                                                            LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
								                                                                        FROM E_CostCenterLineMaster 
								                                                                        WHERE Clm_Status = 'A' ) AS HASLINE
					                                                                        ON Clm_CostCenterCode = Emt_CostcenterCode

					                                                                        LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                        ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                        AND '{0}' BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE())) ",dtpEffectivityDate.Date.ToString("MM/dd/yyyy")));
        if (!txtFromCostCenterCode.Text.Equals(string.Empty))
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

    protected string QueryFilter()
    {
        string filter = string.Empty;
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        if (!txtFromCostCenterCode.Text.Equals(string.Empty))
        {
            filter += string.Format(@"AND Emt_CostCenterCode = '{0}' ", txtFromCostCenterCode.Text);
        }
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_LastName like '{0}%'
                                           OR Emt_FirstName like '{0}%'
                                           OR Emt_NickName like '{0}%'
                                           OR Emt_EmployeeId like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
        }

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
        

        return filter;
    }

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;

        #region CostCenter
        if (txtFromCostCenterCode.Text.Equals(txtToCostCenterCode.Text))
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
        dr["Mve_Type"] = "C";
        dr["Mve_From"] = txtFromCostCenterCode.Text;
        dr["Mve_To"] = txtToCostCenterCode.Text;
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
            btnFromCostCenter.Attributes.Remove("disabled");
            btnToCostCenter.Attributes.Remove("disabled");
            txtReason.Attributes.Remove("readOnly");
        }
        else
        {
            img.Attributes.Remove("disabled");
            img.Attributes.Add("disabled", "true");
            btnFromCostCenter.Attributes.Remove("disabled");
            btnFromCostCenter.Attributes.Add("disabled", "true");
            btnToCostCenter.Attributes.Remove("disabled");
            btnToCostCenter.Attributes.Add("disabled", "true");
            txtReason.Attributes.Remove("readOnly");
            txtReason.Attributes.Add("readOnly", "true");
        }
    }

    private DataTable GetMinMaxDate()
    {
        string sql = @" SELECT Convert(varchar(10), Ppm_StartCycle, 101) [MinDate]
                             , Convert(varchar(10), Ppm_EndCycle, 101) [MaxDate]
                          FROM T_PayPeriodMaster
                         WHERE Ppm_CycleIndicator = 'C'";
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        return dt;
    }

//    private string GetDefaultCCCode()   // from v1
//    {
//        string sql = @" SELECT TOP 1 ISNULL(Emt_CostCenterCode, '')
//                          FROM T_EmployeeMaster
//                         WHERE LEFT(Emt_JobStatus, 1) = 'A'
//                         ORDER BY 1";
//        string retVal = string.Empty;
//        using (DALHelper dal = new DALHelper())
//        {
//            try
//            {
//                dal.OpenDB();
//                retVal = dal.ExecuteScalar(sql, CommandType.Text).ToString();
//            }
//            catch
//            {

//            }
//            finally
//            {
//                dal.CloseDB();
//            }
//        }
//        return retVal;
//    }

    private string GetCCDescription(string ccCode)
    {
        string sql = @"SELECT dbo.GetCostCenterFullNameV2(Cct_CostCenterCode)
                         FROM T_CostCenter
                        WHERE Cct_CostCenterCode = '" + ccCode + @"'
                          AND Cct_Status = 'A'";
        string retVal = string.Empty;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                retVal = dal.ExecuteScalar(sql, CommandType.Text).ToString();
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        return (retVal.Equals(string.Empty)) ? "     -  INVALID COST CENTER CODE. Code is not in master.  -" : retVal;
    }
    #endregion
}
