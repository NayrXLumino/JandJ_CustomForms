using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class Transactions_TimeModification_pgeTimeModificationReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFTIMERECREP"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    PreRender += new EventHandler(Transactions_TimeModification_pgeTimeModificationReport_PreRender);
                }
            }
        }
        LoadComplete += new EventHandler(Transactions_TimeModification_pgeTimeModificationReport_LoadComplete);
    }

    #region Events
    void Transactions_TimeModification_pgeTimeModificationReport_LoadComplete(object sender, EventArgs e)
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
        btnCostcenter.OnClientClick = string.Format("return lookupRTKCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRTKCostcenterLine()");
        btnPayPeriod.OnClientClick = string.Format("return lookupRTKPayPeriod()");
        btnType.OnClientClick = string.Format("return lookupRTKType()");
        btnChecker1.OnClientClick = string.Format("return lookupRTKCheckerApprover('Trm_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupRTKCheckerApprover('Trm_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupRTKCheckerApprover('Trm_ApprovedBy','txtApprover')");
    }

    void Transactions_TimeModification_pgeTimeModificationReport_PreRender(object sender, EventArgs e)
    {
        
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //Menu Log
        SystemMenuLogBL.InsertGenerateLog("WFTIMERECREP", txtEmployee.Text, true, Session["userLogged"].ToString());

    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
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

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "TIME MODIFICATION REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Time Modification Report");
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

                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "TIME MODIFICATION REPORT", initializeExcelHeader());
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

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        btnGenerate.Focus();
        UpdatePagerLocation();
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFTIMERECREP");
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


        if (MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF"))
        {
            tbrCostcenterLine.Visible = true;
        }
        else
        {
            tbrCostcenterLine.Visible = false;
        }
    }

    private string SQLBuilder(string stringReplace)
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string enrouteQuery = string.Empty;
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Lastname], [Adjustment Date] @Sort) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(string.Format(@"                ) AS temp)
                                           SELECT [Control No]
                                                , [Status]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [Adjustment Date]
                                                , [Shift for the Day]
                                                , [Mod Type]
                                                , [Mod Description]
                                                , [Time In 1]
                                                , [Time Out 1]
                                                , [Time In 2]
                                                , [Time Out 2]
                                                , [Cost Center]
                                                {0} 
                                                , [Applied Date]
                                                , [Endorsed Date]
                                                , [Reason]
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
                                            ", !hasCCLine ? "" : ", [CC Line]"));
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(distinct T_TimeRecMod.Trm_ControlNo) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        if (cbxEnroute.Checked == true)
        {
            enrouteQuery = @"LEFT JOIN T_EmployeeApprovalRoute ON Arm_EmployeeId = Trm_EmployeeId
	                            AND Arm_TransactionID = 'TIMEMOD'
								AND Trm_ModDate BETWEEN T_EmployeeApprovalRoute.Arm_StartDate AND ISNULL(T_EmployeeApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                             LEFT JOIN T_ApprovalRouteMaster 
	                            ON  T_ApprovalRouteMaster.Arm_RouteId = T_EmployeeApprovalRoute.Arm_RouteID";
            sql = sql.Replace("@Checker1", "ISNULL(Trm_CheckedBy, Arm_Checker1)").Replace("@Checker2", "ISNULL(Trm_Checked2By, Arm_Checker2)").Replace("@Approver", "ISNULL(Trm_ApprovedBy, Arm_Approver)").Replace("@Enroute", enrouteQuery);
        }
        else
        {
            enrouteQuery = @"";
            sql = sql.Replace("@Checker1", "C1.Umt_UserCode").Replace("@Checker2", "C2.Umt_UserCode").Replace("@Approver", "AP.Umt_UserCode").Replace("@Enroute", enrouteQuery);
        }

        return sql.ToString().Replace("!#!", stringReplace).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }

    private string getColumns()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string columns = string.Empty;
        columns = string.Format(@" SELECT distinct T_TimeRecMod.Trm_ControlNo [Control No]
                          , Trm_EmployeeId [ID No]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname]
                          , Convert(varchar(10), Trm_ModDate, 101) [Adjustment Date]
                          , CASE WHEN ISNULL(E1.Ell_ShiftCode, '') <> ''
		                         THEN '['+E1.Ell_ShiftCode+'] '
			                        + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
			                        + '-'
			                        + LEFT(S1.Scm_ShiftBreakStart,2) + ':' + RIGHT(S1.Scm_ShiftBreakStart,2)
			                        + '  ' 
			                        + LEFT(S1.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S1.Scm_ShiftBreakEnd,2)
			                        + '-'
			                        + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2) 
		                         ELSE '['+E2.Ell_ShiftCode+'] '
			                        + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
			                        + '-'
			                        + LEFT(S2.Scm_ShiftBreakStart,2) + ':' + RIGHT(S2.Scm_ShiftBreakStart,2)
			                        + '  ' 
			                        + LEFT(S2.Scm_ShiftBreakEnd,2) + ':' + RIGHT(S2.Scm_ShiftBreakEnd,2)
			                        + '-'
			                        + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2) 
	                         END [Shift for the Day]
                          , Trm_Type [Mod Type]
                          , AD2.Adt_AccountDesc [Mod Description]
                          , CASE WHEN (Trm_ActualTimeIn1 = '')
                                 THEN ''
                                 ELSE LEFT(Trm_ActualTimeIn1,2) + ':' + RIGHT(Trm_ActualTimeIn1,2)
                             END [Time In 1]
                          , CASE WHEN (Trm_ActualTimeOut1 = '')
                                 THEN ''
                                 ELSE LEFT(Trm_ActualTimeOut1,2) + ':' + RIGHT(Trm_ActualTimeOut1,2)
                             END [Time Out 1]
                          , CASE WHEN (Trm_ActualTimeIn2 = '')
                                 THEN ''
                                 ELSE LEFT(Trm_ActualTimeIn2,2) + ':' + RIGHT(Trm_ActualTimeIn2,2)
                             END [Time In 2]
                          , CASE WHEN (Trm_ActualTimeOut2 = '')
                                 THEN ''
                                 ELSE LEFT(Trm_ActualTimeOut2,2) + ':' + RIGHT(Trm_ActualTimeOut2,2)
                             END [Time Out 2]
                          , dbo.getCostCenterFullNameV2(Trm_CostCenter) [Cost Center]
                          {0}
                          , Convert(varchar(10), Trm_AppliedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Trm_AppliedDate,113),5) [Applied Date]
                          , Convert(varchar(10), Trm_EndorsedDateToChecker,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Trm_EndorsedDateToChecker,113),5) [Endorsed Date]
                          , Trm_Reason [Reason]
                          , AD1.Adt_AccountDesc [Status]
                          , Emt_FirstName [First Name]
                          , Emt_LastName [Last Name]
                          , dbo.GetControlEmployeeNameV2(@Checker1) [Checker 1]
                          , Convert(varchar(10), Trm_CheckedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Trm_CheckedDate,113),5) [Checked Date 1]
	                      , dbo.GetControlEmployeeNameV2(@Checker2) [Checker 2]
                          , Convert(varchar(10), Trm_Checked2Date,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Trm_Checked2Date,113),5) [Checked Date 2]
	                      , dbo.GetControlEmployeeNameV2(@Approver) [Approver]
                          , Convert(varchar(10), Trm_ApprovedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Trm_ApprovedDate,113),5) [Approved Date]
                          , Trm_Remarks [Remarks]
                          , Trm_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Trm_CostCenterLine [CC Line]");
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        filter = string.Format(@"     FROM T_TimeRecMod
                                      LEFT JOIN T_EmployeeLogLedger E1
                                        ON E1.Ell_ProcessDate = Trm_ModDate
                                       AND E1.Ell_EmployeeId = Trm_EmployeeId
                                      LEFT JOIN T_EmployeeLogLedgerHist E2
                                        ON E2.Ell_ProcessDate = Trm_ModDate
                                       AND E2.Ell_EmployeeId = Trm_EmployeeId
                                      LEFT JOIN T_ShiftCodeMaster S1
                                        ON S1.Scm_ShiftCode = E1.Ell_ShiftCode
                                      LEFT JOIN T_ShiftCodeMaster S2
                                        ON S2.Scm_ShiftCode = E2.Ell_ShiftCode
                                      LEFT JOIN T_UserMaster C1 
                                        ON C1.Umt_UserCode = Trm_CheckedBy
                                      LEFT JOIN T_UserMaster C2 
                                        ON C2.Umt_UserCode = Trm_Checked2By
                                      LEFT JOIN T_UserMaster AP 
                                        ON AP.Umt_UserCode = Trm_ApprovedBy
                                      INNER JOIN T_EmployeeMaster 
                                        ON Emt_EmployeeId = Trm_EmployeeId
                                      LEFT JOIN T_AccountDetail AD1 
                                        ON AD1.Adt_AccountCode = Trm_Status 
                                       AND AD1.Adt_AccountType =  'WFSTATUS'
                                      LEFT JOIN T_AccountDetail AD2
                                        ON AD2.Adt_AccountCode = Trm_Type
                                       AND AD2.Adt_AccountType = 'TMERECTYPE'
                                      LEFT JOIN T_PayPeriodMaster 
                                        ON Ppm_PayPeriod = Trm_CurrentPayPeriod
                                      LEFT JOIN T_TransactionRemarks 
                                        ON T_TransactionRemarks.Trm_ControlNo = T_TimeRecMod.Trm_ControlNo
                                       @Enroute
                                    WHERE 1 = 1 AND Trm_Status <> '' ");    
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
            filter += string.Format(@" AND  ( Trm_Costcenter {0}
                                           OR dbo.getCostCenterFullNameV2(Trm_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text));
        }
        if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {

            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" AND  (  ( Trm_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'TIMEKEEP')
                                                      OR Trm_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }
            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND (Trm_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'TIMEKEEP') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									            OR Trm_CostCenter + ISNULL(Trm_CostcenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'TIMEKEEP'))
                                                OR Trm_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_Status {0}
                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
        }
        if (!txtType.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_Type {0}
                                           OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtType.Text));
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_CheckedBy {0}
                                           OR C1.Umt_UserCode {0}
                                           OR C1.Umt_UserLname {0}
                                           OR C1.Umt_UserFname {0}
                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_Checked2By {0}
                                           OR C2.Umt_UserCode {0}
                                           OR C2.Umt_UserLname {0}
                                           OR C2.Umt_UserFname {0}
                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Trm_ApprovedBy {0}
                                           OR AP.Umt_UserCode {0}
                                           OR AP.Umt_UserLname {0}
                                           OR AP.Umt_UserFname {0}
                                           OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
        }
        #endregion
        #region DateTime Pickers
        //Adjustment Date
        if (!dtpTKDateFrom.IsNull && !dtpTKDateTo.IsNull)
        {
            filter += string.Format(@" AND Trm_ModDate BETWEEN '{0}' AND '{1}'", dtpTKDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpTKDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpTKDateFrom.IsNull)
        {
            filter += string.Format(@" AND Trm_ModDate >= '{0}'", dtpTKDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpTKDateTo.IsNull)
        {
            filter += string.Format(@" AND Trm_ModDate <= '{0}'", dtpTKDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Applied Date
        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Trm_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedFrom.IsNull)
        {
            filter += string.Format(@" AND Trm_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Trm_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Endorsed Date
        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Trm_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedFrom.IsNull)
        {
            filter += string.Format(@" AND Trm_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Trm_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date
        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Trm_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1From.IsNull)
        {
            filter += string.Format(@" AND Trm_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Trm_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date 2
        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Trm_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2From.IsNull)
        {
            filter += string.Format(@" AND Trm_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Trm_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        //Approved Date
        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Trm_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPFrom.IsNull)
        {
            filter += string.Format(@" AND Trm_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Trm_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion
        if (cbxEnroute.Checked == true)
        {
            filter += @" AND 1 = 1 AND Trm_Status in ('3','5','7')";
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @" AND ( ( T_TimeRecMod.Trm_ControlNo LIKE '{0}%' )
                                        OR ( Trm_EmployeeId LIKE '%{0}%' )
                                        OR ( Emt_NickName LIKE '%{0}%' )
                                        OR ( Emt_Lastname LIKE '%{0}%' )
                                        OR ( Emt_Firstname LIKE '%{0}%' )
                                        OR ( Convert(varchar(10),Trm_ModDate,101) LIKE '%{0}%' )
                                        OR ( LEFT(Trm_ActualTimeIn1,2) + ':' + RIGHT(Trm_ActualTimeIn1,2) LIKE '{0}%' )
                                        OR ( LEFT(Trm_ActualTimeOut1,2) + ':' + RIGHT(Trm_ActualTimeOut1,2) LIKE '{0}%' )
                                        OR ( LEFT(Trm_ActualTimeIn2,2) + ':' + RIGHT(Trm_ActualTimeIn2,2) LIKE '{0}%' )
                                        OR ( LEFT(Trm_ActualTimeOut2,2) + ':' + RIGHT(Trm_ActualTimeOut2,2) LIKE '{0}%' )
                                        OR ( dbo.getCostCenterFullNameV2(Trm_CostCenter) LIKE '%{0}%' )
                                        OR ( Convert(varchar(10), Trm_AppliedDate,101) 
                                           + ' ' 
                                           + RIGHT(Convert(varchar(17), Trm_AppliedDate,113),5) LIKE '%{0}%' )
                                        OR ( Trm_Reason LIKE '{0}%' )
                                        OR ( Trm_Remarks LIKE '{0}%' )
                                        OR ( AD1.Adt_AccountDesc LIKE '{0}%' )
                                        OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
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

            //Includes
            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                if (cbxEnroute.Checked == true &&
                        (cblInclude.Items[i].Value == "Checker 1" || cblInclude.Items[i].Value == "Checker 2" || cblInclude.Items[i].Value == "Approver"))
                    continue;
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
        if (!txtType.Text.Trim().Equals(string.Empty))
        {
            criteria += lblType.Text + ":" + txtType.Text.Trim() + "-";
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
        if (!dtpTKDateFrom.IsNull)
        {
            criteria += lblTKDateFrom.Text + ":" + dtpTKDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpTKDateTo.IsNull)
        {
            criteria += lblTKDateTo.Text + ":" + dtpTKDateTo.Date.ToString("MM/dd/yyyy") + "-";
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
