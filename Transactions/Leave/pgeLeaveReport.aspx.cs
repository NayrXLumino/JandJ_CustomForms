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

public partial class Transactions_Leave_pgeLeaveReport : System.Web.UI.Page
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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLVEREP"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    PreRender += new EventHandler(Transactions_Leave_pgeLeaveReport_PreRender);
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveReport_LoadComplete);
        }
    }

    #region Events
    void Transactions_Leave_pgeLeaveReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "leaveScripts";
        string jsurl = "_leave.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupRLVEmployee()");
        btnStatus.OnClientClick = string.Format("return lookupRLVStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupRLVCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRLVCostcenterLine()");
        btnPayPeriod.OnClientClick = string.Format("return lookupRLVPayPeriod()");
        btnLeaveType.OnClientClick = string.Format("return lookupRLVType()");
        btnCategory.OnClientClick = string.Format("return lookupRLVCategory()");
        btnDayUnit.OnClientClick = string.Format("return lookupRLVDayUnit()");
        btnChecker1.OnClientClick = string.Format("return lookupRLVCheckerApprover('Elt_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupRLVCheckerApprover('Elt_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupRLVCheckerApprover('Elt_ApprovedBy','txtApprover')");
    }

    void Transactions_Leave_pgeLeaveReport_PreRender(object sender, EventArgs e)
    {

    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFLVEREP", txtEmployee.Text, true, Session["userLogged"].ToString());
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
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
                RemoveColumns(ds);
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "LEAVE REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Leave Report");
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
                RemoveColumns(ds);
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "LEAVE REPORT", initializeExcelHeader());
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
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFLVEREP");
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
                                btnFiller1.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller1','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
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
                                btnFiller2.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller2','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
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
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string enrouteQuery = string.Empty;
        StringBuilder sql = new StringBuilder();
        if (!cbxLeaveNotice.Checked)
        {
            sql.Append(@"declare @startIndex int;
                            SET @startIndex = (@pageIndex * @numRow) + 1;
                           WITH TempTable AS ( SELECT Row_Number()
                                                 OVER ( ORDER BY [Lastname], [Leave Date] @Sort) [Row]
                                                    , *
                                                 FROM ( ");
            sql.Append(getColumns());
            sql.Append(getFilters());
            sql.Append(" UNION ");
            sql.Append(getColumns());
            sql.Append(getFilters().Replace("T_EmployeeLeaveAvailment", "T_EmployeeLeaveAvailmentHist"));
            sql.Append(string.Format(@"  ) AS temp)
                                               SELECT [Control No]
                                                    , [Status]
                                                    , [ID No]
                                                    , [Nickname]
                                                    , [ID Code]
                                                    , [Lastname]
                                                    , [Firstname]
                                                    , [Leave Date]
                                                    , [Leave Type]
                                                    , [Category]
                                                    , [@Elt_Filler1Desc]
                                                    , [@Elt_Filler2Desc]
                                                    , [@Elt_Filler3Desc]
                                                    , [Day Unit]
                                                    , [Start]
                                                    , [End]
                                                    , [Hours]
                                                    , [Cost Center]
                                                    {0}
                                                    , [Applied Date]
                                                    , [Informed Date]
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
                            FROM ( SELECT COUNT(distinct Elt_ControlNo) [cnt]");
            sql.Append(getFilters());
            sql.Append(@"           UNION 
                                   SELECT COUNT(distinct Elt_ControlNo)");
            sql.Append(getFilters().Replace("T_EmployeeLeaveAvailment", "T_EmployeeLeaveAvailmentHist"));
            sql.Append(@"        ) as Rows");

            sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
            sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
            sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
        }
        else //Leave Notice Report
        {
            sql.Append(@"declare @startIndex int;
                             SET @startIndex = (@pageIndex * @numRow) + 1;
                            WITH TempTable AS ( SELECT Row_Number()
                                                 OVER ( ORDER BY [Lastname], [Leave Date] DESC) [Row]
                                                    , *
                                                 FROM ( ");
            sql.Append(getColumns());
            sql.Append(getFilters()); sql.Append(string.Format(@"  ) AS temp)
                                               SELECT [Control No]
                                                    , [Status]
                                                    , [ID No]
                                                    , [Nickname]
                                                    , [ID Code]
                                                    , [Lastname]
                                                    , [Firstname]
                                                    , [Leave Date]
                                                    , [Leave Type]
                                                    , [Category]
                                                    , [@Eln_Filler1Desc]
                                                    , [@Eln_Filler2Desc]
                                                    , [@Eln_Filler3Desc]
                                                    , [Day Unit]
                                                    , [Start]
                                                    , [End]
                                                    , [Hours]
											        , [Inform Mode]
											        , [Informant]
											        , [Relation]
                                                    , [Cost Center]
                                                    {0}
                                                    , [Applied Date]
                                                    , [Informed Date]
                                                    , [Reason]
                                                    , [First Name]
                                                    , [Last Name]
                                                    , [Remarks]
                                                 FROM TempTable
                                                !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                                ", !hasCCLine ? "" : ", [CC Line]"));
            sql.Append(@" SELECT SUM(cnt)
                            FROM ( SELECT COUNT(Eln_EmployeeId) [cnt]");
            sql.Append(getFilters());
            sql.Append(@"        ) as Rows");

            sql = sql.Replace("@Eln_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
            sql = sql.Replace("@Eln_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
            sql = sql.Replace("@Eln_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
        }
        if (cbxEnroute.Checked == true)
        {
            enrouteQuery = @"LEFT JOIN T_EmployeeApprovalRoute on Arm_EmployeeId = Elt_EmployeeId
	                         AND Arm_TransactionID = 'LEAVE'
                             AND Elt_LeaveDate BETWEEN T_EmployeeApprovalRoute.Arm_StartDate AND ISNULL(T_EmployeeApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                             LEFT JOIN T_ApprovalRouteMaster 
                             on  T_ApprovalRouteMaster.Arm_RouteId = T_EmployeeApprovalRoute.Arm_RouteID";
            sql = sql.Replace("@Checker1", "ISNULL(Elt_CheckedBy, Arm_Checker1)").Replace("@Checker2", "ISNULL(Elt_Checked2By, Arm_Checker2)").Replace("@Approver", "ISNULL(Elt_ApprovedBy, Arm_Approver)").Replace("@Enroute", enrouteQuery);
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
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string columns = string.Empty;
        if (!cbxLeaveNotice.Checked)
        {
            columns = string.Format(@"   SELECT distinct Elt_ControlNo [Control No]
                                              , Elt_EmployeeId [ID No]
                                              , Emt_NickName [ID Code]
                                              , Emt_NickName [Nickname]
                                              , Emt_Lastname [Lastname]
                                              , Emt_Firstname [Firstname]
                                              , Convert(varchar(10), Elt_LeaveDate, 101) [Leave Date]
                                              , Ltm_LeaveDesc [Leave Type]
                                              , ISNULL(ADCTGRY.Adt_AccountDesc, '- not applicable -') [Category]
                                              , LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) [Start]
                                              , LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) [End]
                                              , Elt_LeaveHour [Hours]
                                              , Elt_DayUnit [Day Unit]
                                              , dbo.getCostCenterFullNameV2(Elt_CostCenter) [Cost Center]
                                              {0}
                                              , Convert(varchar(10), Elt_AppliedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Elt_AppliedDate,113),5) [Applied Date]
                                              , CONVERT(varchar(10),Elt_InformDate,101) 
                                                + ' ' 
                                                + LEFT(CONVERT(varchar(20),Elt_InformDate,114),5) [Informed Date]
                                              , Convert(varchar(10), Elt_EndorsedDateToChecker,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Elt_EndorsedDateToChecker,113),5) [Endorsed Date]
                                              , Elt_Reason [Reason]
                                              , AD2.Adt_AccountDesc [@Elt_Filler1Desc]
                                              , AD3.Adt_AccountDesc [@Elt_Filler2Desc]
                                              , AD4.Adt_AccountDesc [@Elt_Filler3Desc]
                                              , AD1.Adt_AccountDesc [Status]
                                              , Emt_FirstName [First Name]
                                              , Emt_LastName [Last Name]
                                              , dbo.GetControlEmployeeNameV2(@Checker1) [Checker 1]
                                              , Convert(varchar(10), Elt_CheckedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Elt_CheckedDate,113),5) [Checked Date 1]
	                                          , dbo.GetControlEmployeeNameV2(@Checker2) [Checker 2]
                                              , Convert(varchar(10), Elt_Checked2Date,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Elt_Checked2Date,113),5) [Checked Date 2]
	                                          , dbo.GetControlEmployeeNameV2(@Approver) [Approver]
                                              , Convert(varchar(10), Elt_ApprovedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Elt_ApprovedDate,113),5) [Approved Date]
                                              , Trm_Remarks [Remarks]
                                              , Elt_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Elt_CostCenterLine [CC Line]");
        }
        else//Leave Notice
        {
            columns = string.Format(@"   SELECT distinct Eln_ControlNo [Control No]
                                              , Eln_EmployeeId [ID No]
                                              , Emt_NickName [ID Code]
                                              , Emt_NickName [Nickname]
                                              , Emt_Lastname [Lastname]
                                              , Emt_Firstname [Firstname]
                                              , Convert(varchar(10), Eln_LeaveDate, 101) [Leave Date]
                                              , Ltm_LeaveDesc [Leave Type]
                                              , ISNULL(ADCTGRY.Adt_AccountDesc, '- not applicable -') [Category]
                                              , LEFT(Eln_StartTime,2) + ':' + RIGHT(Eln_StartTime,2) [Start]
                                              , LEFT(Eln_EndTime,2) + ':' + RIGHT(Eln_EndTime,2) [End]
                                              , Eln_LeaveHour [Hours]
                                              , Eln_DayUnit [Day Unit]
                                              , ISNULL(AD5.Adt_AccountDesc, '') [Inform Mode]
                                              , Eln_Informant [Informant]
                                              , ISNULL(AD6.Adt_AccountDesc, '') [Relation]
                                              , dbo.getCostCenterFullNameV2(Eln_CostCenter) [Cost Center]
                                              {0}
                                              , Convert(varchar(10), Eln_AppliedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Eln_AppliedDate,113),5) [Applied Date]
                                              , CONVERT(varchar(10),Eln_InformDate,101) 
                                                + ' ' 
                                                + LEFT(CONVERT(varchar(20),Eln_InformDate,114),5) [Informed Date]
                                              , Eln_Reason [Reason]
                                              , AD2.Adt_AccountDesc [@Eln_Filler1Desc]
                                              , AD3.Adt_AccountDesc [@Eln_Filler2Desc]
                                              , AD4.Adt_AccountDesc [@Eln_Filler3Desc]
                                              , AD1.Adt_AccountDesc [Status]
                                              , Emt_FirstName [First Name]
                                              , Emt_LastName [Last Name]
                                              , Trm_Remarks [Remarks]", !hasCCLine ? "" : ", Eln_CostCenterLine [CC Line]");
        }
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        string searchFilter = string.Empty;
        if (!cbxLeaveNotice.Checked)
        {
            filter = string.Format(@"    FROM T_EmployeeLeaveAvailment
                                          LEFT JOIN T_UserMaster C1 
                                            ON C1.Umt_UserCode = Elt_CheckedBy
                                          LEFT JOIN T_UserMaster C2 
                                            ON C2.Umt_UserCode = Elt_Checked2By
                                          LEFT JOIN T_UserMaster AP 
                                            ON AP.Umt_UserCode = Elt_ApprovedBy
                                          LEFT JOIN T_EmployeeMaster 
                                            ON Emt_EmployeeId = Elt_EmployeeId
                                          LEFT JOIN T_AccountDetail AD1 
                                            ON AD1.Adt_AccountCode = Elt_Status 
                                           AND AD1.Adt_AccountType =  'WFSTATUS'
                                          LEFT JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Elt_LeaveType
                                          LEFT JOIN T_AccountDetail ADCTGRY
                                            ON ADCTGRY.Adt_AccountType = 'LVECATEGRY'
                                           AND ADCTGRY.Adt_AccountCode = Elt_LeaveCategory
                                          LEFT JOIN T_AccountDetail AD2
                                            ON AD2.Adt_AccountCode = Elt_Filler1
                                           AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler01')
                                          LEFT JOIN T_AccountDetail AD3
                                            ON AD3.Adt_AccountCode = Elt_Filler2
                                           AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler02')
                                          LEFT JOIN T_AccountDetail AD4
                                            ON AD4.Adt_AccountCode = Elt_Filler3
                                           AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler03')
                                          LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Elt_CurrentPayPeriod
                                          LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Elt_ControlNo
                                          @Enroute
                                         WHERE 1 = 1 AND Elt_Status <> '' ");
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
                filter += string.Format(@" AND  ( Elt_Costcenter {0}
                                               OR dbo.getCostCenterFullNameV2(Elt_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text));
            }

            if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
            }
            if (!txtStatus.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Status {0}
                                               OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
            }
            
            //No need else because if employee login user cannot change the txtEmployee filter 
            //so report would always filter only user's own transaction
            //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
            //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
            if (tbrEmployee.Visible)
            {
                if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
                {
                    filter += string.Format(@" AND  (  ( Elt_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'LEAVE')
                                                      OR Elt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


                }
                if (hasCCLine)//flag costcenter line to retain old code
                {
                    filter += string.Format(@" AND (Elt_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'LEAVE') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									            OR Elt_CostCenter + ISNULL(Elt_CostCenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'LEAVE'))
                                                OR Elt_EmployeeID = '{0}') ", Session["userLogged"].ToString());
                }
            }
            if (cbxEnroute.Checked == true)
            {
                filter += @" AND 1 = 1 AND Elt_Status in ('3','5','7')";
            }
            if (!txtStatus.Text.Trim().Equals(string.Empty))
            {
                if (sqlINFormat(txtStatus.Text).IndexOf("'9'") != -1)
                {
                    filter += @" AND Elt_ControlNo NOT IN ( 
                                                SELECT 
					                                Elt_RefControlNo
				                                FROM T_EMPLOYEELEAVEAVAILMENT
				                                WHERE Elt_RefControlNo IS NOT NULL
				                                AND RTRIM(Elt_RefControlNo) <> ''
				
				                                UNION
				
                                                SELECT 
					                                Elt_RefControlNo
				                                FROM T_EMPLOYEELEAVEAVAILMENTHIST
				                                WHERE Elt_RefControlNo IS NOT NULL
				                                AND RTRIM(Elt_RefControlNo) <> '' ) ";
                }
            }

            if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
            }
            if (!txtLeaveType.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_LeaveType {0}
                                               OR Ltm_LeaveDesc {0})", sqlINFormat(txtLeaveType.Text));
            }
            if (!txtCategory.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_LeaveCategory {0}
                                               OR ADCTGRY.Adt_AccountDesc {0})", sqlINFormat(txtCategory.Text));
            }
            if (!txtDayUnit.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_DayUnit {0} )", sqlINFormat(txtDayUnit.Text));
            }
            if (!txtFiller1.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler1 {0}
                                               OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtFiller1.Text));
            }
            if (!txtFiller2.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler2 {0}
                                               OR AD3.Adt_AccountDesc {0})", sqlINFormat(txtFiller2.Text));
            }
            if (!txtFiller3.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler3 {0}
                                               OR AD4.Adt_AccountDesc {0})", sqlINFormat(txtFiller3.Text));
            }
            if (!txtChecker1.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_CheckedBy {0}
                                               OR C1.Umt_UserCode {0}
                                               OR C1.Umt_UserLname {0}
                                               OR C1.Umt_UserFname {0}
                                               OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
            }
            if (!txtChecker2.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Checked2By {0}
                                               OR C2.Umt_UserCode {0}
                                               OR C2.Umt_UserLname {0}
                                               OR C2.Umt_UserFname {0}
                                               OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
            }
            if (!txtApprover.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_ApprovedBy {0}
                                               OR AP.Umt_UserCode {0}
                                               OR AP.Umt_UserLname {0}
                                               OR AP.Umt_UserFname {0}
                                               OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
            }
            #endregion
            #region DateTime Pickers
            //LEAVE Date
            if (!dtpLVDateFrom.IsNull && !dtpLVDateTo.IsNull)
            {
                filter += string.Format(@" AND Elt_LeaveDate BETWEEN '{0}' AND '{1}'", dtpLVDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpLVDateTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpLVDateFrom.IsNull)
            {
                filter += string.Format(@" AND Elt_LeaveDate >= '{0}'", dtpLVDateFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpLVDateTo.IsNull)
            {
                filter += string.Format(@" AND Elt_LeaveDate <= '{0}'", dtpLVDateTo.Date.ToString("MM/dd/yyyy"));
            }

            //Applied Date
            if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Elt_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                       , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedFrom.IsNull)
            {
                filter += string.Format(@" AND Elt_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Elt_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }
            //Endorsed Date
            if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
            {
                filter += string.Format(@" AND Elt_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                                 , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpEndorsedFrom.IsNull)
            {
                filter += string.Format(@" AND Elt_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpEndorsedTo.IsNull)
            {
                filter += string.Format(@" AND Elt_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
            }

            //INFORM Date
            if (!dtpInformDateFrom.IsNull && !dtpInformDateTo.IsNull)
            {
                filter += string.Format(@" AND Elt_InformDate BETWEEN '{0}' AND '{1}'", dtpInformDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpInformDateTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpInformDateFrom.IsNull)
            {
                filter += string.Format(@" AND Elt_InformDate >= '{0}'", dtpInformDateFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpInformDateTo.IsNull)
            {
                filter += string.Format(@" AND Elt_InformDate <= '{0}'", dtpInformDateTo.Date.ToString("MM/dd/yyyy"));
            }
            //Checked Date
            if (!dtpC1From.IsNull && !dtpC1To.IsNull)
            {
                filter += string.Format(@" AND Elt_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                       , dtpC1To.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC1From.IsNull)
            {
                filter += string.Format(@" AND Elt_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC1To.IsNull)
            {
                filter += string.Format(@" AND Elt_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
            }
            //Checked Date 2
            if (!dtpC2From.IsNull && !dtpC2To.IsNull)
            {
                filter += string.Format(@" AND Elt_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpC2To.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC2From.IsNull)
            {
                filter += string.Format(@" AND Elt_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC2To.IsNull)
            {
                filter += string.Format(@" AND Elt_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
            }
            //Approved Date
            if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
            {
                filter += string.Format(@" AND Elt_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpAPTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAPFrom.IsNull)
            {
                filter += string.Format(@" AND Elt_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAPTo.IsNull)
            {
                filter += string.Format(@" AND Elt_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
            }
            #endregion

            if (!txtSearch.Text.Trim().Equals(string.Empty))
            {
                searchFilter = @"AND ( ( Elt_ControlNo LIKE '{0}%' )
                                    OR ( Elt_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Elt_LeaveDate,101) LIKE '%{0}%' )
                                    OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                    OR ( Elt_DayUnit LIKE '{0}%' )
                                    OR ( Elt_LeaveCategory  LIKE '{0}%' )
                                    OR ( ADCTGRY.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) LIKE '{0}%' )
                                    OR ( LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) LIKE '{0}%' )
                                    OR ( Elt_LeaveHour LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Elt_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Elt_AppliedDate,101) 
                                        + ' ' 
                                        + RIGHT(Convert(varchar(17), Elt_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Elt_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + RIGHT(Convert(varchar(17), Elt_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Elt_Reason LIKE '{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD3.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD4.Adt_AccountDesc LIKE '{0}%' )
                                    )";
            }
        }
        else//Leave Notice
        {
            filter = string.Format(@"     FROM T_EmployeeLeaveNotice
                                          LEFT JOIN T_EmployeeMaster 
                                            ON Emt_EmployeeId = Eln_EmployeeId
                                          LEFT JOIN T_AccountDetail AD1 
                                            ON AD1.Adt_AccountCode = Eln_Status 
                                           AND AD1.Adt_AccountType =  'WFSTATUS'
                                          LEFT JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Eln_LeaveType
                                          LEFT JOIN T_AccountDetail ADCTGRY
                                            ON ADCTGRY.Adt_AccountType = 'LVECATEGRY'
                                           AND ADCTGRY.Adt_AccountCode = Eln_LeaveCategory
                                          LEFT JOIN T_AccountDetail AD2
                                            ON AD2.Adt_AccountCode = Elt_Filler1
                                           AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler01')
                                          LEFT JOIN T_AccountDetail AD3
                                            ON AD3.Adt_AccountCode = Elt_Filler2
                                           AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler02')
                                          LEFT JOIN T_AccountDetail AD4
                                            ON AD4.Adt_AccountCode = Elt_Filler3
                                           AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler03')
                                          LEFT JOIN T_AccountDetail AD5
                                            ON AD5.Adt_AccountCode = Eln_InformMode
                                           AND AD5.Adt_AccountType = 'MODENOTICE'
                                          LEFT JOIN T_AccountDetail AD6
                                            ON AD6.Adt_AccountCode = Eln_InformantRelation
                                           AND AD6.Adt_AccountType = 'NOTICEREL'
                                          LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Eln_ControlNo
                                         WHERE 1 = 1 AND Eln_Status <> '' ");    
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
                filter += string.Format(@" AND  ( Eln_Costcenter {0}
                                               OR dbo.getCostCenterFullNameV2(Eln_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                     , Session["userLogged"].ToString());
            }

            if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
            }
            //No need else because if employee login user cannot change the txtEmployee filter 
            //so report would always filter only user's own transaction
            //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
            //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
            if (tbrEmployee.Visible)
            {
                if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "LEAVE"))
                {
                    filter += string.Format(@" AND  (  ( Eln_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'LEAVE')
                                                      OR Eln_EmployeeId = '{0}'))", Session["userLogged"].ToString());


                }
                if (hasCCLine)//flag costcenter line to retain old code
                {
                    filter += string.Format(@" AND (Eln_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'LEAVE') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									            OR Eln_CostCenter + ISNULL(Eln_CostcenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'LEAVE'))
                                                OR Eln_EmployeeID = '{0}') ", Session["userLogged"].ToString());
                }
            }
            if (!txtStatus.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_Status {0}
                                               OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
            }
            if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
            }
            if (!txtLeaveType.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_LeaveType {0}
                                               OR Ltm_LeaveDesc {0})", sqlINFormat(txtLeaveType.Text));
            }
            if (!txtCategory.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_LeaveCategory {0}
                                               OR ADCTGRY.Adt_AccountDesc {0})", sqlINFormat(txtCategory.Text));
            }
            if (!txtDayUnit.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eln_DayUnit {0} )", sqlINFormat(txtDayUnit.Text));
            }
            if (!txtFiller1.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler1 {0}
                                               OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtFiller1.Text));
            }
            if (!txtFiller2.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler2 {0}
                                               OR AD3.Adt_AccountDesc {0})", sqlINFormat(txtFiller2.Text));
            }
            if (!txtFiller3.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Elt_Filler3 {0}
                                               OR AD4.Adt_AccountDesc {0})", sqlINFormat(txtFiller3.Text));
            }
            #endregion
            #region DateTime Pickers
            //LEAVE Date
            if (!dtpLVDateFrom.IsNull && !dtpLVDateTo.IsNull)
            {
                filter += string.Format(@" AND Eln_LeaveDate BETWEEN '{0}' AND '{1}'", dtpLVDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpLVDateTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpLVDateFrom.IsNull)
            {
                filter += string.Format(@" AND Eln_LeaveDate >= '{0}'", dtpLVDateFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpLVDateTo.IsNull)
            {
                filter += string.Format(@" AND Eln_LeaveDate <= '{0}'", dtpLVDateTo.Date.ToString("MM/dd/yyyy"));
            }

            //Applied Date
            if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Eln_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                       , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedFrom.IsNull)
            {
                filter += string.Format(@" AND Eln_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Eln_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }

            //INFORM Date
            if (!dtpInformDateFrom.IsNull && !dtpInformDateTo.IsNull)
            {
                filter += string.Format(@" AND Eln_InformDate BETWEEN '{0}' AND '{1}'", dtpInformDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpInformDateTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpInformDateFrom.IsNull)
            {
                filter += string.Format(@" AND Eln_InformDate >= '{0}'", dtpInformDateFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpInformDateTo.IsNull)
            {
                filter += string.Format(@" AND Eln_InformDate <= '{0}'", dtpInformDateTo.Date.ToString("MM/dd/yyyy"));
            }
            #endregion

            if (!txtSearch.Text.Trim().Equals(string.Empty))
            {
                searchFilter = @"AND ( ( Eln_ControlNo LIKE '{0}%' )
                                    OR ( Eln_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Eln_LeaveDate,101) LIKE '%{0}%' )
                                    OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                    OR ( Eln_DayUnit LIKE '{0}%' )
                                    OR ( Eln_LeaveCategory  LIKE '{0}%' )
                                    OR ( ISNULL(AD5.Adt_AccountCode, '')  LIKE '{0}%' )
                                    OR ( ISNULL(AD6.Adt_AccountCode, '')  LIKE '{0}%' )
                                    OR ( ISNULL(AD5.Adt_AccountDesc, '')  LIKE '{0}%' )
                                    OR ( ISNULL(AD6.Adt_AccountDesc, '')  LIKE '{0}%' )
                                    OR ( Eln_Informant  LIKE '{0}%' )
                                    OR ( ADCTGRY.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( LEFT(Eln_StartTime,2) + ':' + RIGHT(Eln_StartTime,2) LIKE '{0}%' )
                                    OR ( LEFT(Eln_EndTime,2) + ':' + RIGHT(Eln_EndTime,2) LIKE '{0}%' )
                                    OR ( Eln_LeaveHour LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Eln_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Eln_AppliedDate,101) 
                                        + ' ' 
                                        + RIGHT(Convert(varchar(17), Eln_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Eln_Reason LIKE '{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD3.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD4.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD5.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD6.Adt_AccountDesc LIKE '{0}%' )
                                    )";
            }
        }

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            filter += string.Format(searchFilter, holder);
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

    //    private string getFillerName(string fillerName)
    //    {

    //        string sql = string.Format(@"  SELECT CASE WHEN ISNULL(Cfl_TextDisplay, Cfl_ColName) = ''
    //			                                    THEN Cfl_ColName
    //			                                    ELSE Cfl_TextDisplay
    //	                                          END
    //	                                     FROM T_ColumnFiller
    //                                        WHERE Cfl_ColName = '{0}'", fillerName);
    //        string flag = string.Empty;
    //        using (DALHelper dal = new DALHelper())
    //        {
    //            try
    //            {
    //                dal.OpenDB();
    //                flag = dal.ExecuteScalar(sql, CommandType.Text).ToString();
    //            }
    //            catch
    //            {

    //            }
    //            finally
    //            {
    //                dal.CloseDB();
    //            }
    //        }
    //        TextInfo UsaTextInfo = new CultureInfo("en-US", false).TextInfo;
    //        return UsaTextInfo.ToTitleCase(flag.ToLower());
    //    }

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
            RemoveColumns(ds);
            hfRowCount.Value = "0";
            foreach (DataRow dr in ds.Tables[1].Rows)
                hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        }
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    private void RemoveColumns(DataSet ds)
    {
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
            if (CommonMethods.getFillerStatus("Elt_Filler01") == "C")
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler01"));
            if (CommonMethods.getFillerStatus("Elt_Filler02") == "C")
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler02"));
            if (CommonMethods.getFillerStatus("Elt_Filler03") == "C")
            ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler03"));

            //Includes
            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                try
                {
                    if (cbxEnroute.Checked == true &&
                        (cblInclude.Items[i].Value == "Checker 1" || cblInclude.Items[i].Value == "Checker 2" || cblInclude.Items[i].Value == "Approver"))
                        continue;
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                catch
                {
                    //do nothing. this is to trap for leave notice
                }
            }
            #endregion
        }
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
        if (txtLeaveType.Text.Trim().Equals(string.Empty))
        {
            criteria += lblLeaveType.Text + ":" + txtLeaveType.Text.Trim() + "-";
        }
        if (txtCategory.Text.Trim().Equals(string.Empty))
        {
            criteria += lblLeaveCategory.Text + ":" + txtCategory.Text.Trim() + "-";
        }
        if (txtDayUnit.Text.Trim().Equals(string.Empty))
        {
            criteria += lblDayUnit.Text + ":" + txtDayUnit.Text.Trim() + "-";
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
        if (!dtpLVDateFrom.IsNull)
        {
            criteria += lblLVDateFrom.Text + ":" + dtpLVDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpLVDateTo.IsNull)
        {
            criteria += lblLVDateTo.Text + ":" + dtpLVDateTo.Date.ToString("MM/dd/yyyy") + "-";
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
