/* File revision no. W2.1.00001
 * Robert Jayre Arriesgado
 * added allow flow on generate overtime
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

public partial class Transactions_Overtime_pgeOvertimeRange : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    OvertimeBL OTBL = new OvertimeBL();
    MenuGrant MGBL = new MenuGrant();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFSPLOTENTRY"))
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
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
            }
            LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeRange_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Transactions_Overtime_pgeOvertimeRange_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "overtimeScripts";
        string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtFiller1.Attributes.Add("readOnly", "true");
        txtFiller2.Attributes.Add("readOnly", "true");
        txtFiller3.Attributes.Add("readOnly", "true");
        txtJobCode.Attributes.Add("readOnly", "true");
        txtClientJobNo.Attributes.Add("readOnly", "true");
        txtClientJobName.Attributes.Add("readOnly", "true");
        txtOTHours.Attributes.Add("OnKeyPress", "javascript:return hoursEntry(event);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {

    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        CheckAll();
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {//robert added allowflow
        //if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)||!methods.GetProcessControlFlag("OVERTIME", "CUT-OFF"))
        if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDateFrom.Date.ToString("MM/dd/yyyy"), dtpOTDateTo.Date.ToString("MM/dd/yyyy")))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();

                        DataSet dsOvertimeDates = OTBL.getOvertimeTransactionWithDetail(txtEmployeeId.Text, dtpOTDateFrom.Date.ToString("MM/dd/yyyy"), dtpOTDateTo.Date.ToString("MM/dd/yyyy"), ddlOTType.SelectedValue, txtOTHours.Text);

                        if (!CommonMethods.isEmpty(dsOvertimeDates))
                        {
                            dgvGenerated.DataSource = dsOvertimeDates;
                            dgvGenerated.DataBind();
                            //MenuLog
                            //SystemMenuLogBL.InsertGenerateLog("WFSPLOTENTRY", txtEmployeeId.Text, true, Session["userLogged"].ToString());
                            btnSaveEndorse.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Unable to create transactions between " + dtpOTDateFrom.Date.ToString("MM/dd/yyyy") + " - " + dtpOTDateTo.Date.ToString("MM/dd/yyyy") + "\nThere may be existing transactions.");
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
                MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("OVERTIME"));
            }
        }
        else
        {
            MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
        }

    }

    protected void btnSaveEndorse_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            string datesFiled = string.Empty;
            string controlNo = string.Empty;
            string status = CommonMethods.getStatusRoute(txtEmployeeId.Text, "OVERTIME", "ENDORSE TO CHECKER 1");

            if (!status.Equals(string.Empty))
            {
                DataRow dr = null;
                if (Page.IsValid)
                {
                    if (!MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                    {
                        if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                          || !CommonMethods.isAffectedByCutoff("OVERTIME", dtpOTDateFrom.Date.ToString("MM/dd/yyyy"), dtpOTDateTo.Date.ToString("MM/dd/yyyy")))
                        {
                            #region Get Included Dates
                            string strDates = "";
                            DateTime dtDate = dtpOTDateFrom.Date;
                            while (dtDate <= dtpOTDateTo.Date)
                            {
                                strDates += string.Format("{0:MM/dd/yyyy},", dtDate);
                                dtDate = dtDate.AddDays(1);
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

                                        //Get Filler Codes
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

                                        string strCreateBatchOTQuery = string.Format("EXEC CreateBatchOvertimeRecord '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}' "
                                                                                    , batchNo
                                                                                    , txtEmployeeId.Text.ToString()
                                                                                    , strDates
                                                                                    , ddlOTType.SelectedValue
                                                                                    , ""
                                                                                    , ""
                                                                                    , txtOTHours.Text
                                                                                    , txtReason.Text
                                                                                    , "" //Next level status depends on approval route
                                                                                    , Session["userLogged"].ToString()
                                                                                    , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                                    , "WFSPLOTENTRY"
                                                                                    , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION)
                                                                                    , txtJobCode.Text.ToString().ToUpper()
                                                                                    , txtClientJobName.Text.ToString().ToUpper()
                                                                                    , filler1
                                                                                    , filler2
                                                                                    , filler3);
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
                                            SystemMenuLogBL.InsertAddLog("WFSPLOTENTRY", true, txtEmployeeId.Text, Session["userLogged"].ToString(), "", false);
                                        }
                                        else
                                        {
                                            dal.RollBackTransactionSnapshot();
                                            SystemMenuLogBL.InsertAddLog("WFSPLOTENTRY", false, txtEmployeeId.Text, Session["userLogged"].ToString(), "", false);
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

                                //restoreDefaultControls();
                                //MessageBox.Show("Successfully saved and endorsed transactions.");
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
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "OVERTIME", "WFSPLOTENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;

        dtpOTDateFrom.Date = DateTime.Now;
        dtpOTDateFrom.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpOTDateFrom.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        dtpOTDateTo.Date = DateTime.Now;
        dtpOTDateTo.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpOTDateTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");

        hfPrevOTDateFrom.Value = dtpOTDateFrom.Date.ToShortDateString();
        hfPrevOTDateTo.Value = dtpOTDateTo.Date.ToShortDateString();
        hfSaved.Value = "0";
        hfPrevEntry.Value = string.Empty;
        OTBL.initializeOTTypes(ddlOTType, false);
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
        try
        {
            tbrJobCode.Visible = methods.GetProcessControlFlag("OVERTIME", "WFOTJOB");
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            tbrJobCode.Visible = false;
        }
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
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
        hfPrevEntry.Value = changeSnapShot();
    }

    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = dtpOTDateFrom.Date.ToString()
                 + dtpOTDateTo.Date.ToString()
                 + ddlOTType.SelectedValue
                 + txtOTHours.Text
                 + txtReason.Text
                 + txtFiller1.Text
                 + txtFiller2.Text
                 + txtFiller3.Text;
        return snapShot;
    }

    private void restoreDefaultControls()
    {
        txtOTHours.Text = string.Empty;
        txtFiller1.Text = string.Empty;
        txtFiller2.Text = string.Empty;
        txtFiller3.Text = string.Empty;
        txtReason.Text = string.Empty;
        ddlOTType.SelectedIndex = 0;
        hfPrevEntry.Value = string.Empty;
        btnSaveEndorse.Enabled = false;
        hfSaved.Value = "0";
        initializeEmployee();
        initializeControls();
        dgvGenerated.DataSource = new DataTable("dummy");
        dgvGenerated.DataBind();
    }
    #endregion
}
