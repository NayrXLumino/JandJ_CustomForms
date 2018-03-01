using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;

public partial class Transactions_TimeModification_pgeStraightWorkReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }//WFTIMERECENTRY                                                 WFSTWORKREPORT
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFSTRGHTWORKREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
        }
        LoadComplete += new EventHandler(Transactions_TimeModification_pgeStraightWorkReport_LoadComplete);
    }

    #region Events
    void Transactions_TimeModification_pgeStraightWorkReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "timemodificationScripts";
        string jsurl = "_timemodification.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupRTKEmployee()");
        btnStatus.OnClientClick = string.Format("return lookupRTKStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupRSWCostcenter()");
        btnPayPeriod.OnClientClick = string.Format("return lookupRSWPayPeriod()");
        btnBatchNo.OnClientClick = string.Format("return lookupRSWBatchNo()");
        btnShift.OnClientClick = string.Format("return lookupRSWShift()");
        btnChecker1.OnClientClick = string.Format("return lookupRSWCheckerApprover('Swt_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupRSWCheckerApprover('Swt_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupRSWCheckerApprover('Swt_ApprovedBy','txtApprover')");
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //Depending if Used
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler01"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler02"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler03"));

                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "STRAIGHT WORK REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Straight Work Report");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //Depending if Used
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler01"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler02"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler03"));

                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];

                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "STRAIGHT WORK REPORT", initializeExcelHeader() );
                    ctrl[1] = tempGridView;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        bindGrid();
        UpdatePagerLocation();
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        btnGenerate.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFSTRGHTWORKREP");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);

            txtEmployee.Text = Session["userLogged"].ToString();
            tbrEmployee.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
        }
        else
        {
            btnExport.Enabled = false;
            btnPrint.Enabled = false;
        }
        #region Filler Controls
        DataSet ds = new DataSet();
        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Swt_Filler%'";
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
                        case "SWT_FILLER01":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller1.Visible = true;
                                lblFiller1.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller1.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller1','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller1.Visible = false;
                            }
                            break;
                        case "SWT_FILLER02":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller2.Visible = true;
                                lblFiller2.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller2.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller2','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
                            }
                            else
                            {
                                tbrFiller2.Visible = false;
                            }
                            break;
                        case "SWT_FILLER03":
                            if (!ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().Equals(string.Empty)
                              && !ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString().Equals(string.Empty)
                              && ds.Tables[0].Rows[i]["Cfl_Status"].ToString().ToUpper().Equals("A"))
                            {
                                tbrFiller3.Visible = true;
                                lblFiller3.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString().ToLower());
                                btnFiller3.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller3','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
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
        #endregion
    }

    private string SQLBuilder(string stringReplace)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Lastname], [From Date]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@"                              ) AS temp)
                                           SELECT [Control No]
                                                , [Status]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [From Date]
                                                , [To Date]
                                                , [Shift]
                                                , [Unpaid Break]
                                                , [Cost Center]
                                                , [Applied Date]
                                                , [Endorsed Date]
                                                , [Reason]
                                                , [@Swt_Filler1Desc]
                                                , [@Swt_Filler2Desc]
                                                , [@Swt_Filler3Desc]
                                                , [Batch No]
                                                , [First Name]
                                                , [Last Name]
                                                , [Checker 1]
                                                , [Checked Date 1]
		                                        , [Checker 2]
                                                , [Checked Date 2]
		                                        , [Approver]
                                                , [Approved Date]
                                                , [Remarks]
                                                , [Pay Period]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            ");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(Swt_EmployeeId) [cnt]");
        sql.Append(getFilters());
        //No History table for T_TimeRecMod
        //sql.Append(@"           UNION 
        //                       SELECT COUNT(Trm_EmployeeId)");
        //sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@"        ) as Rows");

        sql = sql.Replace("@Swt_Filler1Desc", CommonMethods.getFillerName("Swt_Filler01"));
        sql = sql.Replace("@Swt_Filler2Desc", CommonMethods.getFillerName("Swt_Filler02"));
        sql = sql.Replace("@Swt_Filler3Desc", CommonMethods.getFillerName("Swt_Filler03"));
        return sql.ToString().Replace("!#!", stringReplace);
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Swt_ControlNo [Control No]
                          , Swt_EmployeeId [ID No]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname]
                          , Convert(varchar(10), Swt_FromDate, 101) [From Date]
                          , Convert(varchar(10), Swt_ToDate, 101) [To Date]
                          , '['+Swt_ShiftCode+'] '
			                + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
			                + '-'
			                + LEFT(S1.Scm_ShiftBreakStart,2) + ':' + RIGHT(S1.Scm_ShiftBreakStart,2)
			                + '  ' 
			                + LEFT(S1.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S1.Scm_ShiftBreakEnd,2)
			                + '-'
			                + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2) 
	                         [Shift]
                          , Swt_UnpaidBreak [Unpaid Break]
                          , dbo.getCostCenterFullNameV2(Swt_CostCenter) [Cost Center]
                          , Convert(varchar(10), Swt_AppliedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Swt_AppliedDate,113),5) [Applied Date]
                          , Convert(varchar(10), Swt_EndorsedDateToChecker,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Swt_EndorsedDateToChecker,113),5) [Endorsed Date]
                          , Swt_Reason [Reason]
                          , AD2.Adt_AccountDesc [@Swt_Filler1Desc]
                          , AD3.Adt_AccountDesc [@Swt_Filler2Desc]
                          , AD4.Adt_AccountDesc [@Swt_Filler3Desc]
                          , AD1.Adt_AccountDesc [Status]
                          , Swt_BatchNo [Batch No]
                          , Emt_FirstName [First Name]
                          , Emt_LastName [Last Name]
                          , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                          , Convert(varchar(10), Swt_CheckedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Swt_CheckedDate,113),5) [Checked Date 1]
	                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                          , Convert(varchar(10), Swt_Checked2Date,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Swt_Checked2Date,113),5) [Checked Date 2]
	                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                          , Convert(varchar(10), Swt_ApprovedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Swt_ApprovedDate,113),5) [Approved Date]
                          , Trm_Remarks [Remarks]
                          , Swt_CurrentPayPeriod [Pay Period]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        filter = @"   FROM T_EmployeeStraightWork
                      LEFT JOIN T_ShiftCodeMaster S1
                        ON S1.Scm_ShiftCode = Swt_ShiftCode
                      LEFT JOIN T_UserMaster C1 
                        ON C1.Umt_UserCode = Swt_CheckedBy
                      LEFT JOIN T_UserMaster C2 
                        ON C2.Umt_UserCode = Swt_Checked2By
                      LEFT JOIN T_UserMaster AP 
                        ON AP.Umt_UserCode = Swt_ApprovedBy
                      LEFT JOIN T_EmployeeMaster 
                        ON Emt_EmployeeId = Swt_EmployeeId
                      LEFT JOIN T_AccountDetail AD1 
                        ON AD1.Adt_AccountCode = Swt_Status 
                       AND AD1.Adt_AccountType =  'WFSTATUS'
                      LEFT JOIN T_AccountDetail AD2
                        ON AD2.Adt_AccountCode = Swt_Filler1
                       AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler01')
                      LEFT JOIN T_AccountDetail AD3
                        ON AD3.Adt_AccountCode = Swt_Filler2
                       AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler02')
                      LEFT JOIN T_AccountDetail AD4
                        ON AD4.Adt_AccountCode = Swt_Filler3
                       AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler03')
                      LEFT JOIN T_PayPeriodMaster 
                        ON Ppm_PayPeriod = Swt_CurrentPayPeriod
                      LEFT JOIN T_TransactionRemarks 
                        ON Trm_ControlNo = Swt_ControlNo
                    WHERE 1 = 1 AND Swt_Status <> '' ";
        #region textBox Filters
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_EmployeeId {0}
                                           OR Emt_Lastname {0}
                                           OR Emt_Firstname {0}
                                           OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Costcenter {0}
                                           OR dbo.getCostCenterFullNameV2(Swt_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                    , Session["userLogged"].ToString());
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" OR  (  ( Swt_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                            FROM T_UserCostCenterAccess
                                                                        WHERE Uca_UserCode = '{0}'
                                                                            AND Uca_SytemId = 'TIMEKEEP')
                                                    OR Swt_Costcenter = '{0}'))", Session["userLogged"].ToString());
            }
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Status {0}
                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
        }
        if (!txtBatchNo.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_BatchNo {0})", sqlINFormat(txtBatchNo.Text));
        }
        if (!txtFiller1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Filler1 {0}
                                           OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtFiller1.Text));
        }
        if (!txtFiller2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Filler2 {0}
                                           OR AD3.Adt_AccountDesc {0})", sqlINFormat(txtFiller2.Text));
        }
        if (!txtFiller3.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Filler3 {0}
                                           OR AD4.Adt_AccountDesc {0})", sqlINFormat(txtFiller3.Text));
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_CheckedBy {0}
                                           OR C1.Umt_UserCode {0}
                                           OR C1.Umt_UserLname {0}
                                           OR C1.Umt_UserFname {0}
                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_Checked2By {0}
                                           OR C2.Umt_UserCode {0}
                                           OR C2.Umt_UserLname {0}
                                           OR C2.Umt_UserFname {0}
                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Swt_ApprovedBy {0}
                                           OR AP.Umt_UserCode {0}
                                           OR AP.Umt_UserLname {0}
                                           OR AP.Umt_UserFname {0}
                                           OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
        }
        #endregion
        #region DateTime Pickers        
        if (!dtpDateFrom.IsNull && !dtpDateTo.IsNull)
        {
            filter += string.Format(@" AND ('{0}' BETWEEN Swt_FromDate AND Swt_ToDate OR '{1}' BETWEEN Swt_FromDate AND Swt_ToDate)"
                , dtpDateFrom.Date.ToString("MM/dd/yyyy"), dtpDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpDateFrom.IsNull)
        {
            filter += string.Format(@" AND '{0}' BETWEEN Swt_FromDate AND Swt_ToDate ", dtpDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpDateTo.IsNull)
        {
            filter += string.Format(@" AND '{0}' BETWEEN Swt_FromDate AND Swt_ToDate ", dtpDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Applied Date
        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Swt_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedFrom.IsNull)
        {
            filter += string.Format(@" AND Swt_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Swt_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Endorsed Date
        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Swt_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedFrom.IsNull)
        {
            filter += string.Format(@" AND Swt_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Swt_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date
        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Swt_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1From.IsNull)
        {
            filter += string.Format(@" AND Swt_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Swt_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date 2
        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Swt_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2From.IsNull)
        {
            filter += string.Format(@" AND Swt_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Swt_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        //Approved Date
        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Swt_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPFrom.IsNull)
        {
            filter += string.Format(@" AND Swt_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Swt_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @" AND ( ( Swt_ControlNo LIKE '{0}%' )
                                        OR ( Swt_EmployeeId LIKE '%{0}%' )
                                        OR ( Emt_NickName LIKE '%{0}%' )
                                        OR ( Emt_Lastname LIKE '%{0}%' )
                                        OR ( Emt_Firstname LIKE '%{0}%' )
                                        OR ( Convert(varchar(10),Swt_FromDate,101) LIKE '%{0}%' )
                                        OR ( dbo.getCostCenterFullNameV2(Swt_CostCenter) LIKE '%{0}%' )
                                        OR ( Convert(varchar(10), Swt_AppliedDate,101) 
                                           + ' ' 
                                           + RIGHT(Convert(varchar(17), Swt_AppliedDate,113),5) LIKE '%{0}%' )
                                        OR ( Swt_Reason LIKE '{0}%' )
                                        OR ( Trm_Remarks LIKE '{0}%' )
                                        OR ( AD1.Adt_AccountDesc LIKE '{0}%' )
                                        OR ( Swt_Filler1 LIKE '{0}%' )
                                        OR ( Swt_Filler2 LIKE '{0}%' )
                                        OR ( Swt_Filler3 LIKE '{0}%' )
                                        OR ( Swt_BatchNo LIKE '{0}%' )
                                         )";

            string holder = string.Empty;
            string searchKey = txtSearch.Text.Replace("'", "");
            searchKey += "&";
            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                filter += string.Format(searchFilter, holder);
            }
        }
        return filter;
    }

    private string sqlINFormat(string text)
    {
        string[] temp = text.Split(',');
        string value = "IN ( ";
        for (int i = 0; i < temp.Length; i++)
        {
            value += "'" + temp[i].Trim() + "'";
            if (i != temp.Length - 1)
                value += ",";
        }
        value += ")";
        return value;
    }

    private void UpdatePagerLocation()
    {
        int pageIndex = Convert.ToInt32(hfPageIndex.Value);
        int numRows = Convert.ToInt32(hfNumRows.Value);
        int rowCount = Convert.ToInt32(hfRowCount.Value);
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRowNo.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                string s = SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString());
                ds = dal.ExecuteDataSet(SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
            #region Remove Columns
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    ds.Tables[0].Columns.Remove("Nickname");
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                }
            }
            else
            {
                ds.Tables[0].Columns.Remove("ID Code");
                ds.Tables[0].Columns.Remove("Nickname");
            }

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                ds.Tables[0].Columns.Remove("Lastname");
                ds.Tables[0].Columns.Remove("Firstname");
                ds.Tables[0].Columns.Remove("MI");
            }
            //Depending if Used
            ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler01"));
            ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler02"));
            ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler03"));

            //Includes
            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                if (!cblInclude.Items[i].Selected)
                    ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
            }
            #endregion
        }
        hfRowCount.Value = "0";
        foreach (DataRow dr in ds.Tables[1].Rows)
            hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    private string initializeExcelHeader()
    {
        string criteria = string.Empty;
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            criteria += lblEmployee.Text + ":" + txtEmployee.Text.Trim() + "-";
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            criteria += lblCostcenter.Text + ":" + txtCostcenter.Text.Trim() + "-";
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            criteria += lblStatus.Text + ":" + txtStatus.Text.Trim() + "-";
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            criteria += lblPayPeriod.Text + ":" + txtPayPeriod.Text.Trim() + "-";
        }
        if (!txtBatchNo.Text.Trim().Equals(string.Empty))
        {
            criteria += lblBatchNo.Text + ":" + txtBatchNo.Text.Trim() + "-";
        }
        if (!txtFiller1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller1.Text + ":" + txtFiller1.Text.Trim() + "-";
        }
        if (!txtFiller2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller2.Text + ":" + txtFiller2.Text.Trim() + "-";
        }
        if (!txtFiller3.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller3.Text + ":" + txtFiller3.Text.Trim() + "-";
        }
        if (!txtShift.Text.Trim().Equals(string.Empty))
        {
            criteria += lblShift.Text + ":" + txtShift.Text.Trim() + "-";
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker1.Text + ":" + txtChecker1.Text.Trim() + "-";
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker2.Text + ":" + txtChecker2.Text.Trim() + "-";
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            criteria += lblApprover.Text + ":" + txtApprover.Text.Trim() + "-";
        }
        if (!dtpDateFrom.IsNull)
        {
            criteria += lblDateFrom.Text + ":" + dtpDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpDateTo.IsNull)
        {
            criteria += lblDateTo.Text + ":" + dtpDateTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedFrom.IsNull)
        {
            criteria += lblAppliedFrom.Text + ":" + dtpAppliedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedTo.IsNull)
        {
            criteria += lblAppliedTo.Text + ":" + dtpAppliedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedFrom.IsNull)
        {
            criteria += lblEndorsedFrom.Text + ":" + dtpEndorsedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedTo.IsNull)
        {
            criteria += lblEndorsedTo.Text + ":" + dtpEndorsedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        return criteria.Trim();
    }
    #endregion

}