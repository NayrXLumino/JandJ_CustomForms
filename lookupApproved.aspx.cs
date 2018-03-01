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
using Payroll.DAL;
using CommonLibrary;
using System.Text;

public partial class lookupApproved : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    private bool hasCCLine
    {
        get { return this.ViewState["hasCCLine"] == null ? false : Boolean.Parse(this.ViewState["hasCCLine"].ToString()); }
        set { this.ViewState["hasCCLine"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        if (!Page.IsPostBack)
        {

            hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
            initializeControls();
            hfPageIndex.Value = "0";
            hfRowCount.Value = "0";
            bindGrid();
            UpdatePagerLocation();
            LoadComplete += new EventHandler(lookupNewAndPending_LoadComplete);

            Session["transaction"] = "PENDING";
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void lookupNewAndPending_LoadComplete(object sender, EventArgs e)
    {
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
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
        txtSearch.Focus();
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValue('" + e.Row.RowIndex + "')");
        }
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
    }

    #endregion

    #region Methods
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
                ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
        hfRowCount.Value = "0";
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
        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                //DASH Specific
                ds.Tables[0].Columns.Remove("Job Code");
                ds.Tables[0].Columns.Remove("Client Job Name");
                ds.Tables[0].Columns.Remove("Client Job No");
                ds.Tables[0].Columns.Remove("DASH Class Code");
                ds.Tables[0].Columns.Remove("DASH Work Code");
                //Depending if Used
                if (CommonMethods.getFillerStatus("Eot_Filler01") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                if (CommonMethods.getFillerStatus("Eot_Filler02") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                if (CommonMethods.getFillerStatus("Eot_Filler03") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));
                break;
            case "LV":
                if (CommonMethods.getFillerStatus("Elt_Filler01") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler01"));
                if (CommonMethods.getFillerStatus("Elt_Filler02") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler02"));
                if (CommonMethods.getFillerStatus("Elt_Filler03") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Elt_Filler03"));
                break;
            case "TR":
                break;
            case "FT":
                break;
            case "JS":
                break;
            case "MV":
                break;
            case "TX":
                break;
            case "BF":
                break;
            case "SW":
                //Depending if Used
                if (CommonMethods.getFillerStatus("Swt_Filler01") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler01"));
                if (CommonMethods.getFillerStatus("Swt_Filler02") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler02"));
                if (CommonMethods.getFillerStatus("Swt_Filler03") == "C")
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Swt_Filler03"));
                break;
            default:
                break;
        }
        #endregion
        foreach (DataRow dr in ds.Tables[1].Rows)
            hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    protected string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;
                    
                        WITH TempTable AS (
                      SELECT Row_Number() OVER (Order by [Control No] DESC) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());

        //For transactions with History Table
        if (Request.QueryString["type"].ToString().ToUpper().Equals("OT")
          || Request.QueryString["type"].ToString().ToUpper().Equals("LV"))
        {
            sql.Append(" UNION ");
            sql.Append(getColumns());
            sql.Append(getFilters().Replace("@TableMain", "@TableHist"));
        }
        sql.Append(@")   AS temp)");
        sql.Append(getFinalColumns());
        sql.Append(@"  FROM TempTable
                      WHERE Row between @startIndex AND @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(@CountColumn) [cnt]");
        sql.Append(getFilters());

        //For transactions with History Table
        if (Request.QueryString["type"].ToString().ToUpper().Equals("OT")
          || Request.QueryString["type"].ToString().ToUpper().Equals("LV"))
        {
            sql.Append(" UNION ");
            sql.Append(" SELECT Count(@CountColumn) [cnt]");
            sql.Append(getFilters().Replace("@TableMain", "@TableHist"));
        }
        sql.Append(") AS Rows");

        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                sql = sql.Replace("@TableMain", "T_EmployeeOvertime");
                sql = sql.Replace("@TableHist", "T_EmployeeOvertimeHist");
                sql = sql.Replace("@Eot_Filler1Desc", CommonMethods.getFillerName("Eot_Filler01"));
                sql = sql.Replace("@Eot_Filler2Desc", CommonMethods.getFillerName("Eot_Filler02"));
                sql = sql.Replace("@Eot_Filler3Desc", CommonMethods.getFillerName("Eot_Filler03"));
                sql = sql.Replace("@CountColumn", "Eot_Status");
                break;
            case "LV":
                sql = sql.Replace("@TableMain", "T_EmployeeLeaveAvailment");
                sql = sql.Replace("@TableHist", "T_EmployeeLeaveAvailmentHist");
                sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
                sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
                sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
                sql = sql.Replace("@CountColumn", "Elt_Status");
                break;
            case "TR":
                sql = sql.Replace("@TableMain", "T_TimeRecMod");
                sql = sql.Replace("@CountColumn", "Trm_Status");
                break;
            case "FT":
                break;
            case "JS":
                sql = sql.Replace("@TableMain", "T_JobSplitHeader");
                sql = sql.Replace("@CountColumn", "Jsh_Status");
                break;
            case "MV":
                sql = sql.Replace("@TableMain", "T_Movement");
                sql = sql.Replace("@CountColumn", "Mve_Status");
                break;
            case "TX":
                sql = sql.Replace("@TableMain", "T_PersonnelInfoMovement");
                sql = sql.Replace("@CountColumn", "Pit_Status");
                break;
            case "BF":
                sql = sql.Replace("@TableMain", "T_BeneficiaryUpdate");
                sql = sql.Replace("@CountColumn", "But_Status");
                break;
            case "AD":
                sql = sql.Replace("@TableMain", "T_AddressMovement");
                sql = sql.Replace("@CountColumn", "Amt_Status");
                break;
            case "SW":
                sql = sql.Replace("@TableMain", "T_EmployeeStraightWork");
                sql = sql.Replace("@Swt_Filler1Desc", CommonMethods.getFillerName("Swt_Filler01"));
                sql = sql.Replace("@Swt_Filler2Desc", CommonMethods.getFillerName("Swt_Filler02"));
                sql = sql.Replace("@Swt_Filler3Desc", CommonMethods.getFillerName("Swt_Filler03"));
                sql = sql.Replace("@CountColumn", "Swt_Status");
                break;
            case "GP":
                sql = sql.Replace("@TableMain", "E_EmployeeGatePass");
                sql = sql.Replace("@CountColumn", "Egp_Status");
                break;
            default:
                break;
        }

        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                #region SELECT
                columns =string.Format( @" SELECT Eot_ControlNo [Control No]
                                  , Eot_EmployeeId [ID No]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10),Eot_OvertimeDate,101) [OT Date]
                                  , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                                  , LEFT(Eot_StartTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_StartTime,2) [Start]
                                  , LEFT(Eot_EndTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_EndTime,2) [End]
                                  , Eot_OvertimeHour [Hours]
                                  , dbo.getCostCenterFullNameV2(Eot_CostCenter) [Cost Center]
                                    {0}
                                  , Convert(varchar(10), Eot_AppliedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Eot_AppliedDate,113),5) [Applied Date]
                                  , Convert(varchar(10), Eot_EndorsedDateToChecker,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Eot_EndorsedDateToChecker,113),5) [Endorsed Date]
                                  , Eot_Reason [Reason]
                                  , Eot_JobCode [Job Code]
                                  , Eot_ClientJobNo [Client Job No]
                                  , Slm_ClientJobName [Client Job Name]
                                  , Slm_DashClassCode [DASH Class Code]
                                  , Slm_DashWorkCode [DASH Work Code]
                                  , AD2.Adt_AccountDesc [@Eot_Filler1Desc]
                                  , AD3.Adt_AccountDesc [@Eot_Filler2Desc]
                                  , AD4.Adt_AccountDesc [@Eot_Filler3Desc]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Eot_BatchNo [Batch No]
                                  , Emt_FirstName [First Name]
                                  , Emt_LastName [Last Name]
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Eot_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Eot_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Eot_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Eot_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Eot_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Eot_ApprovedDate,113),5) [Approved Date]
                                  , CASE WHEN Eot_Status = '9' 
                                         THEN '' 
                                         ELSE Trm_Remarks 
                                     END [Remarks]
                                  , Eot_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Eot_CostCenterLine [CC Line]"); 
                #endregion
                break;
            case "LV":
                #region SELECT
                columns =string.Format( @" SELECT Elt_ControlNo [Control No]
                                  , Elt_EmployeeId [ID No]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10), Elt_LeaveDate, 101) [Leave Date]
                                  , Ltm_LeaveDesc [Leave Type]
                                  , ISNULL(ADT2.Adt_AccountDesc, '- not applicable -') [Category]
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
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Elt_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Elt_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Elt_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Elt_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Elt_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Elt_ApprovedDate,113),5) [Approved Date]
                                  , CASE WHEN Elt_Status = '9' 
                                         THEN '' 
                                         ELSE Trm_Remarks 
                                     END [Remarks]
                                  , Elt_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Elt_CostCenterLine [CC Line]");
                #endregion
                break;
            case "TR":
                #region SELECT
                columns = string.Format(@" SELECT T_TimeRecMod.Trm_ControlNo [Control No]
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
                                    {0}
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Trm_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Trm_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Trm_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Trm_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Trm_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Trm_ApprovedDate,113),5) [Approved Date]
                                  , CASE WHEN @TableMain.Trm_Status = '9' 
                                         THEN '' 
                                         ELSE Trm_Remarks 
                                     END [Remarks]
                                  , Trm_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Trm_CostCenterLine [CC Line]");
                #endregion
                break;
            case "FT":
                columns = @"";
                break;
            case "JS":
                columns = @" SELECT Jsh_ControlNo [Control No]
	                              , Jsh_EmployeeId [ID No]
	                              , Emt_NickName [ID Code]
	                              , Emt_NickName [Nickname]
	                              , Emt_Lastname [Lastname]
	                              , Emt_Firstname [Firstname]
	                              , Convert(varchar(10),Jsh_JobSplitDate,101) [Manhour Date]
	                              , CASE Jsh_Entry 
		                            WHEN 'N' THEN 'NEW'
		                            WHEN 'C' THEN 'CHANGE'
                                    ELSE ''
	                                 END [Type]
	                              , dbo.getCostCenterFullNameV2(Jsh_CostCenter) [Cost Center]
	                              , Convert(varchar(10), Jsh_AppliedDate,101) + ' ' 
	                              + RIGHT(Convert(varchar(17), Jsh_AppliedDate,113),5) [Applied Date]
	                              , Convert(varchar(10), Jsh_EndorsedDateToChecker,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Jsh_EndorsedDateToChecker,113),5) [Endorsed Date]
	                              , Adt_AccountDesc [Status]
	                              , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
	                              , Convert(varchar(10), Jsh_CheckedDate,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Jsh_CheckedDate,113),5) [Checked Date 1]
	                                , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
	                              , Convert(varchar(10), Jsh_Checked2Date,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Jsh_Checked2Date,113),5) [Checked Date 2]
	                              , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
	                              , Convert(varchar(10), Jsh_ApprovedDate,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Jsh_ApprovedDate,113),5) [Approved Date]
	                                , CASE WHEN Jsh_Status = '9' 
                                           THEN '' 
                                           ELSE Trm_Remarks 
	                                   END [Remarks]";
                break;
            case "MV":
                #region COLUMNS
                columns =string.Format( @"SELECT Mve_ControlNo [Control No]
                                  , Mve_EmployeeId [ID No]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10), Mve_EffectivityDate, 101) [Effectivity Date]
                                  , ad9.Adt_Accountdesc [Move Type]
                                  , CASE Mve_Type 
                                        WHEN 'G' THEN WTF.Adt_AccountDesc +'/'+ WGF.Adt_AccountDesc 
                                        WHEN 'S' THEN s0.Scm_ShiftDesc
		                                      + '  ('
		                                      --+ REPLICATE(' ', 10 - LEN(s0.Scm_ShiftCode))
		                                      + LEFT(s0.Scm_ShiftTimeIn,2) + ':' + RIGHT(s0.Scm_ShiftTimeIn,2)
		                                      + ' - '
		                                      + LEFT(s0.Scm_ShiftTimeOut,2) + ':' + RIGHT(s0.Scm_ShiftTimeOut,2)
		                                      + ')'
                                        WHEN 'C' THEN dbo.getCostCenterFullNameV2(Mve_From)
                                        ELSE Mve_From
                                    END [From]
                                  , CASE Mve_Type 
                                        WHEN 'G' THEN WTT.Adt_AccountDesc +'/'+ WGT.Adt_AccountDesc 
                                        WHEN 'S' THEN s1.Scm_ShiftDesc
                                            + '  ('
                                            --+ REPLICATE(' ', 10 - LEN(s1.Scm_ShiftCode))
                                            + LEFT(s1.Scm_ShiftTimeIn,2) + ':' + RIGHT(s1.Scm_ShiftTimeIn,2)
                                            + ' - '
                                            + LEFT(s1.Scm_ShiftTimeOut,2) + ':' + RIGHT(s1.Scm_ShiftTimeOut,2)
                                            + ')'
                                        WHEN 'C' THEN dbo.getCostCenterFullNameV2(Mve_To)
                                        ELSE Mve_To
                                    END [To]
                                  , dbo.getCostCenterFullNameV2(Mve_CostCenter) [Cost Center]
                                {0}
                                  , Convert(varchar(10), Mve_AppliedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Mve_AppliedDate,113),5) [Applied Date]
                                  , Convert(varchar(10), Mve_EndorsedDateToChecker,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Mve_EndorsedDateToChecker,113),5) [Endorsed Date]
                                  , Mve_Reason [Reason]
                                  , Emt_FirstName [First Name]
                                  , Emt_LastName [Last Name]
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Mve_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Mve_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Mve_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Mve_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Mve_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Mve_ApprovedDate,113),5) [Approved Date]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Mve_BatchNo [Batch No]
                                  , Trm_Remarks [Remarks]
                                  , Mve_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Mve_CostCenterLine [CC Line]");
                #endregion
                break;
            case "TX":
                #region SELECT
                columns = string.Format(@" SELECT Pit_ControlNo [Control No]
                                  , Pit_EmployeeId [ID No]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10), Pit_EffectivityDate, 101) [Effectivity Date]
                                    {0}
                                  , Pit_MoveType [Type]
                                  , Pit_From [From Tax Code]
                                  , ADFTAX.Adt_AccountDesc [From Tax Desc]
                                  , Pit_To [To Tax Code]
                                  , ADTTAX.Adt_AccountDesc [To Tax Desc]
                                  , Pit_Filler1 [From Civil Code]
                                  , ADFCIVIL.Adt_AccountDesc [From Civil Desc]
                                  , Pit_Filler2 [To Civil Code]
                                  , ADTCIVIL.Adt_AccountDesc [To Civil Desc]
                                  , Pit_Reason [Reason]
                                  , Emt_FirstName [First Name]
                                  , Emt_LastName [Last Name]
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Pit_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Pit_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Pit_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Pit_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Pit_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Pit_ApprovedDate,113),5) [Approved Date]
                                  , AD1.Adt_AccountDesc [Status]
                                  , CASE WHEN Pit_Status = '9' 
                                         THEN '' 
                                         ELSE Trm_Remarks 
                                     END [Remarks]", !hasCCLine ? "" : ", Eot_CostCenterLine [CC Line]");
                #endregion
                break;
            case "BF":
                #region SELECT
                columns =string.Format( @" SELECT But_ControlNo [Control No]
                                  , But_EmployeeId [ID No]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , LEFT(ISNULL(Emt_Middlename,''),1) [MI]
                                  , dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) [Department]
                                  , dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
                                    {0}
                                  , Convert(varchar(10), But_EffectivityDate, 101) [Effectivity Date]
                                  , CONVERT(varchar(10),But_AppliedDate,101) 
                                      + ' ' 
                                      + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5)[Applied Date/Time]
                                  , CASE WHEN (But_Type = 'N') 
			                              THEN 'NEW ENTRY'
			                              ELSE 'UPDATE EXISTING'
		                              END [Type]
                                  , But_Lastname [Beneficiary Lastname]
	                              , But_Firstname [Beneficiary Firtsname]
	                              , But_Middlename [Beneficiary Middlename]
	                              , Convert(varchar(10), But_Birthdate, 101) [Birthdate]
	                              , But_Relationship [Relationship Code]
	                              , AD2.Adt_AccountDesc [Relationship Desc]
	                              , But_Hierarchy [Hierarchy Code]
	                              , AD3.Adt_AccountDesc [Hierarchy Desc]
	                              , But_HMODependent [HMO Dependent]
	                              , But_InsuranceDependent [Insurance Dependent]
	                              , But_BIRDependent [BIR Dependent]
	                              , But_AccidentDependent [Accident Dependent]
	                              , Convert(varchar(10), But_DeceasedDate, 101) [Deceased Date]
	                              , Convert(varchar(10), But_CancelDate, 101) [Cancelled Date]
                                  , But_Reason [Reason]
                                  , Emt_FirstName [First Name]
                                  , Emt_LastName [Last Name]
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), But_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), But_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), But_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), But_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), But_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), But_ApprovedDate,113),5) [Approved Date]
                                  , Trm_Remarks [Remarks]", !hasCCLine ? "" : ", But_CostCenterLine [CC Line]");
                #endregion
                break;
            case "AD":
                #region COLUMNS
                columns =string.Format( @" SELECT Amt_ControlNo [Control No]
                                  , Amt_EmployeeId [ID No]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10), Amt_EffectivityDate, 101) [Effectivity Date]
                                  , dbo.getCostCenterFullNameV2(Amt_CostCenter) [Cost Center]
                                    {0}
                                  , CASE Amt_Type 
                                        WHEN 'A1' THEN 'Present'
                                        WHEN 'A2' THEN 'Permanent'
                                        WHEN 'A3' THEN 'Emergency Contact'
                                        ELSE ''
                                    END [Type]
                                  , Convert(varchar(10), Amt_AppliedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Amt_AppliedDate,113),5) [Applied Date]
                                  , Convert(varchar(10), Amt_EndorsedDateToChecker,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Amt_EndorsedDateToChecker,113),5) [Endorsed Date]
                                  , Amt_Address1 [Number/Street]
                                  , ADADDRESS2.Adt_AccountDesc [Barangay/Municipality]
                                  , ADADDRESS3.Adt_AccountDesc [City/Province/District]
                                  , Amt_TelephoneNo [Telephone No]
                                  , case Amt_Type
		                                when 'A1' then Amt_CellularNo 
		                                else '- not applicable -'
	                                end [Cellular No]
                                  , case Amt_Type
		                                when 'A1' then Amt_EmailAddress 
		                                else '- not applicable -'
	                                end [Email Address]
                                  , case Amt_Type 
					                    when 'A1' then Rte_RouteCode
					                    else '- not applicable -'
				                    end [Route Code]
			                      , case Amt_Type	
					                    when 'A1' then Rte_RouteName
					                    else '- not applicable -'
				                    end [Route Name]
			                      , case Amt_Type	
                                        when 'A1' then CAST(Rte_Amount AS VARCHAR)
                                        else '- not applicable -'
                                    end [Amount]
                                  , Amt_Reason [Reason]
                                  , Emt_FirstName [First Name]
                                  , Emt_LastName [Last Name]
                                  , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                                  , Convert(varchar(10), Amt_CheckedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Amt_CheckedDate,113),5) [Checked Date 1]
			                      , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                                  , Convert(varchar(10), Amt_Checked2Date,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Amt_Checked2Date,113),5) [Checked Date 2]
			                      , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                                  , Convert(varchar(10), Amt_ApprovedDate,101) 
                                    + ' ' 
                                    + RIGHT(Convert(varchar(17), Amt_ApprovedDate,113),5) [Approved Date]
                                  , Trm_Remarks [Remarks]
                                  , CASE Amt_Type 
                                        WHEN 'A3' THEN Amt_ContactPerson
                                        ELSE '- not applicable -'
                                    END [Contact Person]
                                  , CASE Amt_Type 
                                        WHEN 'A3' THEN ADRelation.Adt_AccountDesc
                                        ELSE '- not applicable -'
                                    END [Contact Relation]
", !hasCCLine ? "" : ", Amt_CostCenterLine [CC Line]");
                #endregion
                break;
            case "SW":
                #region SELECT
                columns = @" SELECT Swt_ControlNo [Control No]
                                  , Swt_EmployeeId [ID No]
                                  , Emt_NickName [ID Code]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Convert(varchar(10),Swt_FromDate,101) [From Date]
                                  , Convert(varchar(10),Swt_ToDate,101) [To Date]
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
                                  , CASE WHEN Swt_Status = '9' 
                                         THEN '' 
                                         ELSE Trm_Remarks 
                                     END [Remarks]
                                  , Swt_CurrentPayPeriod [Pay Period]";
                #endregion
                break;
            case "GP":
                columns = @" SELECT Egp_ControlNo [Control No]
	                              , Egp_EmployeeId [ID No]
	                              , Emt_NickName [ID Code]
	                              , Emt_NickName [Nickname]
	                              , Emt_Lastname [Lastname]
	                              , Emt_Firstname [Firstname]
	                              , Convert(varchar(10),Egp_GatePassDate,101) [Gate Pass Date]
	                              , CASE Egp_ApplicationType 
		                                WHEN 'OB' THEN 'OFFICIAL BUSINESS'
		                                WHEN 'UT' THEN 'UNDERTIME'
                                        WHEN 'PL' THEN 'PERSONAL'
                                        WHEN 'OTH' THEN 'OTHERS'
                                        ELSE ''
	                                 END [Type]
	                              , dbo.getCostCenterFullNameV2(Egp_CostCenter) [Cost Center]
	                              , Convert(varchar(10), Egp_AppliedDate,101) + ' ' 
	                              + RIGHT(Convert(varchar(17), Egp_AppliedDate,113),5) [Applied Date]
	                              , Convert(varchar(10), Egp_EndorsedDateToChecker,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Egp_EndorsedDateToChecker,113),5) [Endorsed Date]
	                              , Adt_AccountDesc [Status]
	                              , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
	                              , Convert(varchar(10), Egp_CheckedDate,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Egp_CheckedDate,113),5) [Checked Date 1]
	                                , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
	                              , Convert(varchar(10), Egp_Checked2Date,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Egp_Checked2Date,113),5) [Checked Date 2]
	                              , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
	                              , Convert(varchar(10), Egp_ApprovedDate,101) 
	                                + ' ' 
	                                + RIGHT(Convert(varchar(17), Egp_ApprovedDate,113),5) [Approved Date]
	                                , CASE WHEN Egp_Status = '9' 
                                           THEN '' 
                                           ELSE Trm_Remarks 
	                                   END [Remarks]";
                break;
            default:
                break;
        }
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        string columnName = string.Empty;
        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                #region FILTER
                filter = @"   FROM @TableMain
                              LEFT JOIN T_UserMaster C1 
                                ON C1.Umt_UserCode = Eot_CheckedBy
	                          LEFT JOIN T_UserMaster C2 
                                ON C2.Umt_UserCode = Eot_Checked2By
	                          LEFT JOIN T_UserMaster AP 
                                ON AP.Umt_UserCode = Eot_ApprovedBy
	                          LEFT JOIN T_EmployeeMaster 
                                ON Emt_EmployeeId = Eot_EmployeeId
                              LEFT JOIN T_AccountDetail AD1 
                                ON AD1.Adt_AccountCode = Eot_Status 
                               AND AD1.Adt_AccountType =  'WFSTATUS'
                              LEFT JOIN T_AccountDetail AD2
                                ON AD2.Adt_AccountCode = Eot_Filler1
                               AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler01')
                              LEFT JOIN T_AccountDetail AD3
                                ON AD3.Adt_AccountCode = Eot_Filler2
                               AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler02')
                              LEFT JOIN T_AccountDetail AD4
                                ON AD4.Adt_AccountCode = Eot_Filler3
                               AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Eot_Filler03')
                              LEFT JOIN T_ParameterMasterExt
                                ON Pmx_ParameterValue = Eot_OvertimeType
                               AND Pmx_ParameterID = 'OTTYPE'
                              LEFT JOIN T_SalesMaster ON Eot_JobCode = Slm_DashJobCode AND Eot_ClientJobNo = Slm_ClientJobNo
                              LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Eot_CurrentPayPeriod
                              LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Eot_ControlNo
                             WHERE ( Eot_Status IN ('9') 
                                 AND Eot_EmployeeId = '{0}' ) ";
                searchFilter = @"AND  ( ( Eot_ControlNo LIKE '{0}%' )
                                    OR ( Eot_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Eot_OvertimeDate,101) LIKE '%{0}%' )
                                    OR ( ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') LIKE '{0}%' )
                                    OR ( LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) LIKE '{0}%' )
                                    OR ( LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) LIKE '{0}%' )
                                    OR ( Eot_OvertimeHour LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Eot_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Eot_AppliedDate,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Eot_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Eot_EndorsedDateToChecker,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Eot_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Eot_Reason LIKE '{0}%' )
                                    OR ( Slm_DashWorkCode LIKE '{0}%' )
                                    OR ( Slm_ClientJobName LIKE '{0}%' )
                                    OR ( Eot_JobCode LIKE '{0}%' )
                                    OR ( Eot_ClientJobNo LIKE '{0}%' )
                                    OR ( Slm_DashClassCode LIKE '{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD3.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD4.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( Eot_BatchNo LIKE '{0}%' )
                                    {1} )";
                columnName = " Eot_CostCenterLine ";
                #endregion
                break;
            case "LV":
                #region FILTER
                filter = @"   FROM @TableMain
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
                              LEFT JOIN T_AccountDetail ADT2
                                ON ADT2.Adt_AccountType = 'LVECATEGRY'
                               AND ADT2.Adt_AccountCode = Elt_LeaveCategory
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
                             WHERE ( Elt_Status IN ('9')
                                 AND Elt_EmployeeId = '{0}' )";
                searchFilter = @"AND  ( ( Elt_ControlNo LIKE '{0}%' )
                                    OR ( Elt_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Elt_LeaveDate,101) LIKE '%{0}%' )
                                    OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                    OR ( Elt_LeaveCategory  LIKE '{0}%' )
                                    OR ( ADT2.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( Elt_DayUnit LIKE '{0}%' )
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
                                   {1}  )";
                columnName = " Elt_CostCenterLine ";
                #endregion
                break;
            case "TR":
                #region FILTER
                filter = @"   FROM @TableMain
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
	                          LEFT JOIN T_EmployeeMaster 
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
                             WHERE ( Trm_Status IN ('9') 
                                 AND Trm_EmployeeId = '{0}' ) ";
                searchFilter = @"AND  (( @TableMain.Trm_ControlNo LIKE '{0}%' )
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
                                      {1}  )";
                columnName = " Trm_CostCenterLine ";
                #endregion
                break;
            case "FT":
                break;
            case "JS":
                #region FILTER
                filter = @"		FROM @TableMain
	                            LEFT JOIN T_EmployeeMaster 
                                  ON Emt_EmployeeId = Jsh_EmployeeId
                                LEFT JOIN T_AccountDetail 
                                  ON Adt_AccountCode = Jsh_Status 
                                 AND Adt_AccountType =  'WFSTATUS'
                                LEFT JOIN T_UserMaster C1 
                                  ON C1.Umt_UserCode = Jsh_CheckedBy
                                LEFT JOIN T_UserMaster C2 
                                  ON C2.Umt_UserCode = Jsh_Checked2By
	                            LEFT JOIN T_UserMaster AP 
                                  ON AP.Umt_UserCode = Jsh_ApprovedBy
                                LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Jsh_ControlNo
                                WHERE ( Jsh_Status IN ('9') 
                                  AND Jsh_EmployeeId = '{0}' AND LEFT(Jsh_ControlNo,1) = 'S') ";
                searchFilter = @"AND  ( ( Jsh_ControlNo LIKE '{0}%' )
                                    OR ( Jsh_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Jsh_JobSplitDate,101) LIKE '%{0}%' )
                                    OR ( CASE Jsh_Entry 
                                            WHEN 'N' THEN 'NEW'
                                            WHEN 'C' THEN 'CHANGE'
                                            ELSE ''
                                          END LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Jsh_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Jsh_AppliedDate,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Jsh_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Jsh_EndorsedDateToChecker,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Jsh_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    )"; 
                #endregion
                break;
            case "MV":
                #region FILTER
                filter = @"    FROM T_Movement
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Mve_EmployeeId
                               LEFT JOIN T_UserMaster C1 
                                 ON C1.Umt_UserCode = Mve_CheckedBy
	                           LEFT JOIN T_UserMaster C2 
                                 ON C2.Umt_UserCode = Mve_Checked2By
	                           LEFT JOIN T_UserMaster AP 
                                 ON AP.Umt_UserCode = Mve_ApprovedBy
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Mve_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               left join T_AccountDetail WTF 
		                            ON WTF.Adt_AccountCode = LTRIM(LEFT(Mve_From, 3))
		                            AND WTF.Adt_AccountType = 'WORKTYPE'
                                    AND Mve_Type = 'G'
                                LEFT JOIN T_AccountDetail WTT
		                            ON WTT.Adt_AccountCode = LTRIM(LEFT(Mve_To, 3))
		                            AND WTT.Adt_AccountType = 'WORKTYPE'
		                            AND Mve_Type = 'G'
	                            LEFT JOIN T_AccountDetail WGF
		                            ON WGF.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
		                            AND WGF.Adt_AccountType = 'WORKGROUP'
		                            AND Mve_Type = 'G'
	                            LEFT JOIN T_AccountDetail WGT
		                            ON WGT.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
		                            AND WGT.Adt_AccountType = 'WORKGROUP'
		                            AND Mve_Type = 'G'
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Mve_ControlNo
                              LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                 ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
                                AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
								AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                               LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                 ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                left join t_accountdetail ad9 on ad9.adt_accounttype = 'MOVETYPE' and ad9.adt_accountcode = mve_type
                                LEFT JOIN T_ShiftCodeMaster s0
			                        ON s0.Scm_ShiftCode = Mve_From
                               LEFT JOIN T_ShiftCodeMaster s1
                                    ON s1.Scm_ShiftCode = Mve_To
                                WHERE ( Mve_Status IN ('9') 
                                AND Mve_EmployeeId = '{0}')";
                searchFilter = @" AND( ( Mve_ControlNo LIKE '{0}%' )
                                    OR ( Mve_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%')
                                    OR ( Convert(varchar(10), Mve_EffectivityDate, 101) LIKE '%{0}%')
                                    OR ( Mve_From LIKE '%{0}%')
                                    OR ( WTF.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( WGF.Adt_AccountDesc LIKE '%{0}%') 
                                    OR ( Mve_To LIKE '%{0}%')
                                    OR ( WTT.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( WGT.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Mve_Reason LIKE '%{0}%')
                                    OR ( AD1.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Trm_Remarks LIKE '%{0}%')
                                    {1})";
                columnName = " Mve_CostCenterLine ";
                #endregion
                break;
            case "TX":
                #region FILTER
                filter = @"    FROM @TableMain
                               LEFT JOIN T_UserMaster C1 
                                 ON C1.Umt_UserCode = Pit_CheckedBy
	                           LEFT JOIN T_UserMaster C2 
                                 ON C2.Umt_UserCode = Pit_Checked2By
	                           LEFT JOIN T_UserMaster AP 
                                 ON AP.Umt_UserCode = Pit_ApprovedBy
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Pit_EmployeeId
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Pit_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail ADFTAX 
                                 ON ADFTAX.Adt_AccountCode = Pit_From 
                                AND ADFTAX.Adt_AccountType =  'TAXCODE'
                               LEFT JOIN T_AccountDetail ADTTAX 
                                 ON ADTTAX.Adt_AccountCode = Pit_To 
                                AND ADTTAX.Adt_AccountType =  'TAXCODE'
                               LEFT JOIN T_AccountDetail ADFCIVIL 
                                 ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
                                AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
                               LEFT JOIN T_AccountDetail ADTCIVIL 
                                 ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
                                AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Pit_ControlNo
                             WHERE ( Pit_Status IN ('9') 
                                 AND Pit_EmployeeId = '{0}'
                                 AND Pit_MoveType = 'P1' )";
                searchFilter = @" AND( ( Pit_ControlNo LIKE '{0}%' )
                                    OR ( Pit_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%')
                                    OR ( Convert(varchar(10), Pit_EffectivityDate, 101) LIKE '%{0}%')
                                    OR ( Pit_From LIKE '%{0}%')
                                    OR ( ADFTAX.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Pit_To LIKE '%{0}%') 
                                    OR ( ADTTAX.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Pit_Filler1 LIKE '%{0}%')
                                    OR ( ADFCIVIL.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Pit_Filler2 LIKE '%{0}%')
                                    OR ( ADTCIVIL.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Pit_Reason LIKE '%{0}%')
                                    OR ( AD1.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Trm_Remarks LIKE '%{0}%')
                                    {1})";
                columnName = " Pit_CostCenterLine ";
                #endregion
                break;
            case "BF":
                #region FILTER
                filter = @"    FROM T_BeneficiaryUpdate
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  But_EmployeeId
                               LEFT JOIN T_UserMaster C1 
                                 ON C1.Umt_UserCode = But_CheckedBy
                               LEFT JOIN T_UserMaster C2 
                                 ON C2.Umt_UserCode = But_Checked2By
                               LEFT JOIN T_UserMaster AP 
                                 ON AP.Umt_UserCode = But_ApprovedBy
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = But_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail AD2
                                 ON AD2.Adt_AccountCode = But_Relationship
                                AND AD2.Adt_AccountType = 'RELATION'
                               LEFT JOIN T_AccountDetail AD3
                                 ON AD3.Adt_AccountCode = But_Hierarchy
                                AND AD3.Adt_AccountType = 'HIERARCHDP'
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = But_ControlNo
                              WHERE ( But_Status IN ('9')  
                                AND But_EmployeeId = '{0}' )";
                searchFilter = @"    AND ( ( But_ControlNo LIKE '{0}%' )
                                        OR ( But_EmployeeId LIKE '%{0}%' )
                                        OR ( Emt_NickName LIKE '%{0}%' )
                                        OR ( Emt_Lastname LIKE '%{0}%' )
                                        OR ( Emt_Firstname LIKE '%{0}%')
                                        OR ( Convert(varchar(10), But_EffectivityDate, 101) LIKE '%{0}%')
                                        OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%')
                                        OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) LIKE '{0}%')
                                        OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) LIKE '{0}%')
                                        OR ( CONVERT(varchar(10),But_EffectivityDate,101) LIKE '%{0}%')
                                        OR ( CONVERT(varchar(10),But_AppliedDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5) LIKE '%{0}%')
                                        OR ( But_Lastname LIKE '{0}%' )
                                        OR ( But_Firstname LIKE '%{0}%')
                                        OR ( But_Middlename LIKE '{0}%')
                                        OR ( Convert(varchar(10), But_Birthdate, 101) LIKE '%{0}%')
                                        OR ( But_Relationship LIKE '{0}%')
                                        OR ( AD2.Adt_AccountDesc LIKE '%{0}%')
                                        OR ( But_Hierarchy LIKE '{0}%')
                                        OR ( AD3.Adt_AccountDesc LIKE '%{0}%')
                                        OR ( Convert(varchar(10), But_DeceasedDate, 101) LIKE '%{0}%')
                                        OR ( Convert(varchar(10), But_CancelDate, 101) LIKE '%{0}%')
                                        OR ( AD1.Adt_Accountdesc LIKE '{0}%')
                                        OR ( But_Reason LIKE '{0}%')
                                        OR ( Trm_Remarks LIKE '%{0}%')  {1})";
                columnName = " But_CostCenterLine ";
                #endregion
                break;
            case "AD":
                #region FILTER
                filter = @"    FROM @TableMain
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Amt_EmployeeId
                               LEFT JOIN T_UserMaster C1 
                                 ON C1.Umt_UserCode = Amt_CheckedBy
	                           LEFT JOIN T_UserMaster C2 
                                 ON C2.Umt_UserCode = Amt_Checked2By
	                           LEFT JOIN T_UserMaster AP 
                                 ON AP.Umt_UserCode = Amt_ApprovedBy
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Amt_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail ADADDRESS2
                                 ON ADADDRESS2.Adt_AccountCode = Amt_Address2
                                AND ADADDRESS2.Adt_AccountType = 'BARANGAY'
                               LEFT JOIN T_AccountDetail ADADDRESS3
                                 ON ADADDRESS3.Adt_AccountCode = Amt_Address3
                                AND ADADDRESS3.Adt_AccountType = 'ZIPCODE'
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Amt_ControlNo
                                LEFT JOIN T_AccountDetail ADRelation
                                ON ADRelation.Adt_AccountCode = Amt_ContactRelation
                                AND ADRelation.Adt_AccountType = 'RELATION'
                                LEFT JOIN T_RouteMaster ON Rte_RouteCode = Amt_Filler1 
				                    AND Rte_EffectivityDate = (SELECT MAX(Rte_EffectivityDate) 
									                    FROM T_RouteMaster
									                      WHERE Rte_EffectivityDate <= Amt_EffectivityDate
									                     AND Rte_RouteCode = Amt_Filler1)
                              WHERE ( Amt_Status IN ('9') 
                                AND Amt_EmployeeId = '{0}')";
                searchFilter = @" AND( ( Amt_ControlNo LIKE '{0}%' )
                                    OR ( Amt_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%')
                                    OR ( Convert(varchar(10), Amt_EffectivityDate, 101) LIKE '%{0}%')
                                    OR ( Amt_Address2 LIKE '%{0}%')
                                    OR ( ADADDRESS2.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Amt_Address3 LIKE '%{0}%') 
                                    OR ( ADADDRESS3.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Amt_Reason LIKE '%{0}%')
                                    OR ( AD1.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( Trm_Remarks LIKE '%{0}%') {1})";
                columnName = " Amt_CostCenterLine ";
                #endregion
                break;
            case "SW":
                #region FILTER
                filter = @"   FROM @TableMain
                              LEFT JOIN T_UserMaster C1 
                                ON C1.Umt_UserCode = Swt_CheckedBy
	                          LEFT JOIN T_UserMaster C2 
                                ON C2.Umt_UserCode = Swt_Checked2By
	                          LEFT JOIN T_UserMaster AP 
                                ON AP.Umt_UserCode = Swt_ApprovedBy
	                          LEFT JOIN T_EmployeeMaster 
                                ON Emt_EmployeeId = Swt_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster S1
                                ON S1.Scm_ShiftCode = Swt_ShiftCode
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
                              LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Swt_CurrentPayPeriod
                              LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Swt_ControlNo
                             WHERE ( Swt_Status IN ('9') 
                                 AND Swt_EmployeeId = '{0}' ) ";
                searchFilter = @"AND  ( ( Swt_ControlNo LIKE '{0}%' )
                                    OR ( Swt_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Swt_FromDate,101) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Swt_ToDate,101) LIKE '%{0}%' )
                                    OR ( Swt_UnpaidBreak LIKE '%{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Swt_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Swt_AppliedDate,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Swt_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Swt_EndorsedDateToChecker,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Swt_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Swt_Reason LIKE '{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD3.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( AD4.Adt_AccountDesc LIKE '{0}%' )
                                    OR ( Swt_BatchNo LIKE '{0}%' )
                                     )";
                #endregion
                break;
            case "GP":
                #region FILTER
                filter = @"		FROM @TableMain
	                            LEFT JOIN T_EmployeeMaster 
                                  ON Emt_EmployeeId = Egp_EmployeeId
                                LEFT JOIN T_AccountDetail 
                                  ON Adt_AccountCode = Egp_Status 
                                 AND Adt_AccountType =  'WFSTATUS'
                                LEFT JOIN T_UserMaster C1 
                                  ON C1.Umt_UserCode = Egp_CheckedBy
                                LEFT JOIN T_UserMaster C2 
                                  ON C2.Umt_UserCode = Egp_Checked2By
	                            LEFT JOIN T_UserMaster AP 
                                  ON AP.Umt_UserCode = Egp_ApprovedBy
                                LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Egp_ControlNo
                                WHERE ( Egp_Status IN ('9') 
                                  AND Egp_EmployeeId = '{0}' AND LEFT(Egp_ControlNo,1) = 'G') ";
                searchFilter = @"AND  ( ( Egp_ControlNo LIKE '{0}%' )
                                    OR ( Egp_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Egp_GatePassDate,101) LIKE '%{0}%' )
                                    OR ( CASE Egp_ApplicationType 
		                                    WHEN 'OB' THEN 'OFFICIAL BUSINESS'
		                                    WHEN 'UT' THEN 'UNDERTIME'
                                            WHEN 'PL' THEN 'PERSONAL'
                                            WHEN 'OTH' THEN 'OTHERS'
                                            ELSE ''
                                          END LIKE '{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Egp_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Egp_AppliedDate,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Egp_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Egp_EndorsedDateToChecker,101) 
                                       + ' ' 
                                       + RIGHT(Convert(varchar(17), Egp_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    )";
                #endregion
                break;
            default:
                break;
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string holder = string.Empty;
            string searchKey = txtSearch.Text.Replace("'", "");
            searchKey += "&";
            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                filter += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( " + columnName + " LIKE '{0}%' )", holder));
            }
        }
        return string.Format(filter, Session["userLogged"].ToString());
    }

    private string getFinalColumns()
    {
        string columns = string.Empty;
        switch (Request.QueryString["type"].ToString().ToUpper())
        {
            case "OT":
                #region COLUMNS
                columns = @"   SELECT [Control No]
                                    , [Status]
                                    , [ID No]
                                    , [Nickname]
                                    , [ID Code]
                                    , [Lastname]
                                    , [Firstname]
                                    , [OT Date]
                                    , [Type]
                                    , [Start]
                                    , [End]
                                    , [Hours]
                                    , [Cost Center]
                                    {0}
                                    , [Applied Date]
                                    , [Endorsed Date]
                                    , [Reason]
                                    , [Job Code]
                                    , [Client Job No]
                                    , [Client Job Name]
                                    , [DASH Class Code]
                                    , [DASH Work Code]
                                    , [@Eot_Filler1Desc]
                                    , [@Eot_Filler2Desc]
                                    , [@Eot_Filler3Desc]
                                    , [Batch No]
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]
                                    , [Pay Period]"; 
                #endregion
                break;
            case "LV":
                #region COLUMNS
                columns = @"   SELECT [Control No]
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
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]
                                    , [Pay Period]"; 
                #endregion
                break;
            case "TR":
                #region COLUMNS
                columns = @"   SELECT [Control No]
                                    , [Status]
                                    , [ID No]
                                    , [Nickname]
                                    , [ID Code]
                                    , [Lastname]
                                    , [Firstname]
                                    , [Adjustment Date]
                                    {0}
                                    , [Shift for the Day]
                                    , [Mod Type]
                                    , [Mod Description]
                                    , [Time In 1]
                                    , [Time Out 1]
                                    , [Time In 2]
                                    , [Time Out 2]
                                    , [Cost Center]
                                    , [Applied Date]
                                    , [Endorsed Date]
                                    , [Reason]
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]
                                    , [Pay Period]";
                #endregion
                break;
            case "FT":
                columns = @"";
                break;
            case "JS":
                columns = @"   SELECT [Control No]
                                    , [Status]
                                    , [ID No]
                                    , [Nickname]
                                    , [ID Code]
                                    , [Lastname]
                                    , [Firstname]
                                    , [Manhour Date]
                                    , [Type]
                                    , [Cost Center]
                                    , [Applied Date]
                                    , [Endorsed Date]
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]"; 
                break;
            case "MV":
                #region COLUMNS
                columns = @" SELECT [Control No]
                                  , [Status]
                                  , [ID No]
                                  , [ID Code]
                                  , [Nickname]
                                  , [Lastname]
                                  , [Firstname]
                                  , [Effectivity Date]
                                  , [Move Type]
                                  , [From]
                                  , [To]
                                  , [Reason]
                                  , [Cost Center]
                                   {0}
                                  , [Applied Date]
                                  , [Endorsed Date]
                                  , [Batch No]
                                  , [Checker 1]
                                  , [Checked Date 1]
                                  , [Checker 2]
                                  , [Checked Date 2]
                                  , [Approver]
                                  , [Approved Date]
                                  , [Remarks]
                                  , [Pay Period]";
                #endregion
                break;
            case "TX":
                #region COLUMNS
                columns = @" SELECT [Control No]
                                  , [Status]
                                  , [ID No]
                                  , [ID Code]
                                  , [Nickname]
                                  , [Lastname]
                                  , [Firstname]
                                  , [Effectivity Date]
                                    {0}
                                  , [From Tax Code]
                                  , [From Tax Desc]
                                  , [To Tax Code]
                                  , [To Tax Desc]
                                  , [From Civil Code]
                                  , [From Civil Desc]
                                  , [To Civil Code]
                                  , [To Civil Desc]
                                  , [Reason]
                                  , [Checker 1]
                                  , [Checked Date 1]
                                  , [Checker 2]
                                  , [Checked Date 2]
                                  , [Approver]
                                  , [Approved Date]
                                  , [Remarks]";
                #endregion
                break;
            case "BF":
                #region COLUMNS
                columns = @" SELECT [Control No]
                                  , [Status]
                                  , [ID No]
                                  , [ID Code]
                                  , [Nickname]
                                  , [Lastname]
                                  , [Firstname]
                                  , [Effectivity Date]
                                    {0}
                                  , [Type]
                                  , [Beneficiary Lastname]
	                              , [Beneficiary Firtsname]
	                              , [Beneficiary Middlename]
	                              , [Birthdate]
	                              , [Relationship Code]
	                              , [Relationship Desc]
	                              , [Hierarchy Code]
	                              , [Hierarchy Desc]
	                              , [HMO Dependent]
	                              , [Insurance Dependent]
	                              , [BIR Dependent]
	                              , [Accident Dependent]
	                              , [Deceased Date]
	                              , [Cancelled Date]
                                  , [Reason]
                                  , [Checker 1]
                                  , [Checked Date 1]
                                  , [Checker 2]
                                  , [Checked Date 2]
                                  , [Approver]
                                  , [Approved Date]
                                  , [Remarks]";
                #endregion
                break;
            case "AD":
                #region COLUMNS
                columns = @" SELECT [Control No]
                                  , [Status]
                                  , [ID No]
                                  , [ID Code]
                                  , [Nickname]
                                  , [Lastname]
                                  , [Firstname]
                                  , [Cost Center]
                                    {0}
                                  , [Effectivity Date]
                                  , [Applied Date]
                                  , [Endorsed Date]
                                  , [Type]
                                  , [Number/Street]
                                  , [Barangay/Municipality]
                                  , [City/Province/District]
                                  , [Telephone No]
                                  , [Cellular No]
                                  , [Email Address]
			                      , [Route Code]
			                      , [Route Name] 
			                      , [Amount]
                                  , [Contact Person]
                                  , [Contact Relation]
                                  , [Reason]
                                  , [Checker 1]
                                  , [Checked Date 1]
                                  , [Checker 2]
                                  , [Checked Date 2]
                                  , [Approver]
                                  , [Approved Date]
                                  , [Remarks]";
                #endregion
                break;
            case "SW":
                #region COLUMNS
                columns = @"   SELECT [Control No]
                                    , [Status]
                                    , [ID No]
                                    , [Nickname]
                                    , [ID Code]
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
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]
                                    , [Pay Period]";
                #endregion
                break;
            case "GP":
                columns = @"   SELECT [Control No]
                                    , [Status]
                                    , [ID No]
                                    , [Nickname]
                                    , [ID Code]
                                    , [Lastname]
                                    , [Firstname]
                                    , [Gate Pass Date]
                                    , [Type]
                                    , [Cost Center]
                                    , [Applied Date]
                                    , [Endorsed Date]
                                    , [Checker 1]
                                    , [Checked Date 1]
                                    , [Checker 2]
                                    , [Checked Date 2]
                                    , [Approver]
                                    , [Approved Date]
                                    , [Remarks]";
                break;
            default:
                break;
        }
        columns = string.Format(columns, !hasCCLine ? "" : ", [CC Line]");
        return columns;
    }
    #endregion
}
