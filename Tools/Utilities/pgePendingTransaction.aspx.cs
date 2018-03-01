/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/11/2013
 *  Update Notes    :   
 *      -   Updated Processing on status retrieval
 */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Payroll.DAL;
using CommonLibrary;

public partial class Tools_PendingTransactions_pgePendingTransaction : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    GeneralBL GNBL = new GeneralBL();
    DataTable dtErrors = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFPTRANSACTION"))
                {
                    Response.Redirect(@"../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    hfPageIndex.Value = "0";
                    hfRowCount.Value = "0";
                    Page.PreRender += new EventHandler(Page_PreRender);
                    FillddlTransactions();
                    bindGrid();
                    UpdatePagerLocation();
                }
            }
            LoadComplete += new EventHandler(Maintenance_pgePendingTransactions_LoadComplete);
        }
    }
    private void initializeControls()
    {
        txtSearch.Focus();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }
    void Maintenance_pgePendingTransactions_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "pendingTransactionScripts";
        string jsurl = "_pendingTransaction.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        try
        {
            CheckBox cbxTemp = (CheckBox)dgvResult.HeaderRow.FindControl("chkBoxAll");
            cbxTemp.Attributes.Add("onClick", "javascript:SelectAll();");
        }
        catch
        {

        }
        btnLoad.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
        btnLoad.Attributes.Add("disabled", "true");
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    protected void PendingTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return RowSelection('" + e.Row.RowIndex + "')"); ;
        }
    }
    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    }
    private void FillddlTransactions()
    {
        ddlTransactions.Items.Clear();
        if (Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME))
            ddlTransactions.Items.Add(new ListItem("OVERTIME", "OT"));
        if (Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE))
            ddlTransactions.Items.Add(new ListItem("LEAVE", "LV"));
        if (Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION))
            ddlTransactions.Items.Add(new ListItem("TIME RECORD MODIFICATION", "TR"));
        if (Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION))
            ddlTransactions.Items.Add(new ListItem("WORK INFORMATION MOVEMENT", "MV"));
    }
    private void UpdatePagerLocation()
    {
        //int pageIndex = Convert.ToInt32(hfPageIndex.Value);
        //int numRows = Convert.ToInt32(hfNumRows.Value);
        //int rowCount = Convert.ToInt32(hfRowCount.Value);
        //int currentStartRow = (pageIndex * numRows) + 1;
        //int currentEndRow = (pageIndex * numRows) + numRows;
        //if (currentEndRow > rowCount)
        //    currentEndRow = rowCount;
        //if (rowCount == 0)
        //    currentStartRow = 0;
        //lblRowNo.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        //btnPrev.Enabled = (pageIndex > 0) ? true : false;
        //btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }
    private void bindGrid()
    {
        hfTransactionType.Value = ddlTransactions.Text;
        lblCount.Text = "";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                //ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                ds = dal.ExecuteDataSet(SQLBuilder(), CommandType.Text);
            }
            catch (Exception ex)
            {
                ds = null;
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
        hfRowCount.Value = "0";
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
            switch (hfTransactionType.Value.ToString().ToUpper())
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
                default:
                    break;
            }
            #endregion
            #region Remove Columns
            #endregion
            dgvResult.DataSource = ds;
            lblCount.Text = ds.Tables[0].Rows.Count + " record(s) loaded";
        }
        else
        {
            dgvResult.DataSource = null;
            lblCount.Text = "No records loaded";
        }
        dgvResult.DataBind();
    }
    protected string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(getFinalColumns());
        sql.Append("   FROM (");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append("   ) AS TEMPTABLE WHERE  RTRIM([Control No]) <> '' ");
        //        sql.Append(@"declare @startIndex int;
        //                         SET @startIndex = (@pageIndex * @numRow) + 1;
        //                    
        //                        WITH TempTable AS (
        //                      SELECT Row_Number() OVER (Order by [Control No] DESC) [Row], *
        //                        FROM ( ");
        //        sql.Append(getColumns());
        //        sql.Append(getFilters());
        //        sql.Append(@")   AS temp)");
        //        sql.Append(getFinalColumns());
        //        sql.Append(@"  FROM TempTable ");
        //        sql.Append(" SELECT SUM(cnt) FROM (");
        //        sql.Append(" SELECT Count(@CountColumn) [cnt]");//Just find a common column to count
        //        sql.Append(getFilters());
        //        sql.Append(") AS Rows");

        switch (hfTransactionType.Value.ToString().ToUpper())
        {
            case "OT":
                sql = sql.Replace("@Eot_Filler1Desc", CommonMethods.getFillerName("Eot_Filler01"));
                sql = sql.Replace("@Eot_Filler2Desc", CommonMethods.getFillerName("Eot_Filler02"));
                sql = sql.Replace("@Eot_Filler3Desc", CommonMethods.getFillerName("Eot_Filler03"));
                sql = sql.Replace("@CountColumn", "Eot_Status");
                break;
            case "LV":
                sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
                sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
                sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
                sql = sql.Replace("@CountColumn", "Elt_Status");
                break;
            case "TR":
                sql = sql.Replace("@CountColumn", "Trm_Status");
                break;
            case "FT":
                sql = sql.Replace("@CountColumn", "Jsh_Status");
                break;
            case "JS":
                sql = sql.Replace("@CountColumn", "Jsh_Status");
                break;
            case "MV":
                sql = sql.Replace("@CountColumn", "Mve_Status");
                break;
            case "TX":
                sql = sql.Replace("@CountColumn", "Pit_Status");
                break;
            case "BF":
                sql = sql.Replace("@CountColumn", "But_Status");
                break;
            case "AD":
                sql = sql.Replace("@CountColumn", "Amt_Status");
                break;
            default:
                break;
        }
        return sql.ToString();
    }
    private string getColumns()
    {
        string columns = string.Empty;
        switch (hfTransactionType.Value.ToString().ToUpper())
        {
            case "OT":
                #region COLUMNS
                columns = @" SELECT Eot_ControlNo [Control No]
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
                                  , Trm_Remarks [Remarks]
                                  , Eot_CurrentPayPeriod [Pay Period]";
                #endregion
                break;
            case "LV":
                #region COLUMNS
                columns = @" SELECT Elt_ControlNo [Control No]
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
                                  , Elt_BatchNo [Batch No]
                                  , Trm_Remarks [Remarks]
                                  , Elt_CurrentPayPeriod [Pay Period]";
                #endregion
                break;
            case "TR":
                #region COLUMNS
                columns = @" SELECT T_TimeRecMod.Trm_ControlNo [Control No]
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
                                  , Trm_Remarks [Remarks]
                                  , Trm_CurrentPayPeriod [Pay Period]";
                #endregion
                break;
            case "FT":
                #region COLUMNS
                columns = @"";
                #endregion
                break;
            case "JS":
                #region COLUMNS
                columns = @"";
                #endregion
                break;
            case "MV":
                #region COLUMNS
                columns = @"SELECT Mve_ControlNo [Control No]
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
                                  , Mve_CurrentPayPeriod [Pay Period]";
                #endregion
                break;
            default:
                break;
        }
        return columns;
    }
    private string getFilters()
    {

        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFPTRANSACTION");
        bool canEdit = userGrant.canEdit();
        string filter = string.Empty;
        string searchFilter = string.Empty;
        switch (hfTransactionType.Value.ToString().ToUpper())
        {
            case "OT":
                #region FILTER
                filter = @"   FROM T_EmployeeOvertime
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

                              LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                                ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
                               AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
                             INNER JOIN T_ApprovalRouteMaster AS routeMaster
                                ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId " + (!hasCCLine ? "" : @" LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                                             FROM E_CostCenterLineMaster 
												                                                                                WHERE Clm_Status = 'A' ) AS HASLINE
										                                                                              ON Clm_CostCenterCode = Eot_Costcenter ") + @"    

                             WHERE Emt_JobStatus <> 'IN' and
                                --( (Eot_EmployeeId = '{0}')
                                --  OR (routeMaster.Arm_Checker1 = '{0}')
                                --  OR (routeMaster.Arm_Checker2 = '{0}')
                                --  OR (routeMaster.Arm_Approver = '{0}'))
                               --AND
                                Eot_Status = '1'
                                AND Convert(varchar,GETDATE(),101) BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, GETDATE())" + (canEdit
                                ? @" AND Eot_Costcenter IN ( SELECT IIF( Cct_CostCenterCode = 'ALL', Eot_Costcenter, Cct_CostCenterCode)
                                                               FROM T_UserCostCenterAccess
                                                              INNER JOIN T_CostCenter
	                                                             ON ( Uca_CostCenterCode = 'ALL'
		                                                           OR Uca_CostCenterCode = Cct_CostCenterCode )
                                                              WHERE Uca_SytemID = 'OVERTIME'
	                                                            AND Uca_Usercode = '{0}'
	                                                            AND Uca_Status = 'A') "
                                : " AND Eot_EmployeeID = '{0}' ") + (!hasCCLine ? "" : string.Format(@"  AND ( ISNULL(Eot_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Eot_CostcenterLine, ''), (   SELECT Ucl_LineCode 
										                                                                                                                                                        FROM E_UserCostcenterLineAccess 
																														                                                                       WHERE Ucl_CostCenterCode = Eot_CostCenter
																														                                                                         AND Ucl_Status = 'A'
																														                                                                         AND Ucl_SystemID = 'OVERTIME'
																															                                                                     AND Ucl_LineCode = ISNULL(Eot_CostcenterLine, '')
																														                                                                         AND Ucl_UserCode = '{0}' )) )
										                                                                  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                                                                                         FROM E_UserCostcenterLineAccess 
																								                                                                WHERE Ucl_CostCenterCode = Eot_CostCenter
																								                                                                  AND Ucl_Status = 'A'
																								                                                                  AND Ucl_SystemID = 'OVERTIME'
																								                                                                  AND Ucl_LineCode = 'ALL'
																								                                                                  AND Ucl_UserCode = '{0}' )) ) 
                                                                                                          OR 'ALL' IN ( SELECT Ucl_LineCode 
										                                                                                  FROM E_UserCostcenterLineAccess 
													                                                                     WHERE Ucl_CostCenterCode = 'ALL'
														                                                                   AND Ucl_Status = 'A'
														                                                                   AND Ucl_SystemID = 'OVERTIME'
														                                                                   AND Ucl_LineCode = 'ALL'
														                                                                   AND Ucl_UserCode = '{0}')
                                                                                                          OR Eot_EmployeeID = '{0}' )", Session["userLogged"].ToString()));

                searchFilter = @"AND  ( ( Eot_ControlNo LIKE '{0}%' )
                                    OR ( Eot_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Eot_OvertimeDate,101) LIKE '%{0}%' )
                                    OR ( CASE WHEN (Eot_OvertimeType = 'P')
                                              THEN 'POST'
                                              ELSE 'ADVANCE'
                                          END LIKE '{0}%' )
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
                                    OR ( AD1.Adt_AccountDesc LIKE '{0}%' )
                                     )";
                #endregion
                break;
            case "LV":
                #region FILTER
                filter = @"   FROM T_EmployeeLeaveAvailment
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

                              LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                                ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                               AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
                             INNER JOIN T_ApprovalRouteMaster AS routeMaster
                                ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId " + (!hasCCLine ? "" : @" LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                                             FROM E_CostCenterLineMaster 
												                                                                                WHERE Clm_Status = 'A' ) AS HASLINE
										                                                                              ON Clm_CostCenterCode = Elt_Costcenter ") + @"              

                             WHERE Emt_JobStatus <> 'IN' and
                                --( (Elt_EmployeeId = '{0}')
                                --  OR (routeMaster.Arm_Checker1 = '{0}')
                                --  OR (routeMaster.Arm_Checker2 = '{0}')
                                --  OR (routeMaster.Arm_Approver = '{0}') )
                               --AND 
                                Elt_Status = '1'
                                AND Convert(varchar,GETDATE(),101) BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, GETDATE())" + (canEdit
                                ? @" AND Elt_Costcenter IN ( SELECT IIF( Cct_CostCenterCode = 'ALL', Elt_Costcenter, Cct_CostCenterCode)
                                                               FROM T_UserCostCenterAccess
                                                              INNER JOIN T_CostCenter
	                                                             ON ( Uca_CostCenterCode = 'ALL'
		                                                           OR Uca_CostCenterCode = Cct_CostCenterCode )
                                                              WHERE Uca_SytemID = 'LEAVE'
	                                                            AND Uca_Usercode = '{0}'
	                                                            AND Uca_Status = 'A') "
                                : " AND Elt_EmployeeID = '{0}' ") + (!hasCCLine ? "" : string.Format(@"  AND ( ISNULL(Elt_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Elt_CostcenterLine, ''), (   SELECT Ucl_LineCode 
										                                                                                                                                                        FROM E_UserCostcenterLineAccess 
																														                                                                       WHERE Ucl_CostCenterCode = Elt_CostCenter
																														                                                                         AND Ucl_Status = 'A'
																														                                                                         AND Ucl_SystemID = 'LEAVE'
																															                                                                     AND Ucl_LineCode = ISNULL(Elt_CostcenterLine, '')
																														                                                                         AND Ucl_UserCode = '{0}' )) )
										                                                                  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                                                                                         FROM E_UserCostcenterLineAccess 
																								                                                                WHERE Ucl_CostCenterCode = Elt_CostCenter
																								                                                                  AND Ucl_Status = 'A'
																								                                                                  AND Ucl_SystemID = 'LEAVE'
																								                                                                  AND Ucl_LineCode = 'ALL'
																								                                                                  AND Ucl_UserCode = '{0}' )) ) 
                                                                                                          OR 'ALL' IN ( SELECT Ucl_LineCode 
										                                                                                  FROM E_UserCostcenterLineAccess 
													                                                                     WHERE Ucl_CostCenterCode = 'ALL'
														                                                                   AND Ucl_Status = 'A'
														                                                                   AND Ucl_SystemID = 'LEAVE'
														                                                                   AND Ucl_LineCode = 'ALL'
														                                                                   AND Ucl_UserCode = '{0}')
                                                                                                          OR Elt_EmployeeID = '{0}' )", Session["userLogged"].ToString())); ;

                searchFilter = @"AND  ( ( Elt_ControlNo LIKE '{0}%' )
                                    OR ( Elt_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Elt_LeaveDate,101) LIKE '%{0}%' )
                                    OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                    OR ( Elt_LeaveCategory  LIKE '{0}%' )
                                    OR ( AD1.Adt_AccountDesc LIKE '{0}%' )
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
                                    OR ( Elt_BatchNo LIKE '%{0}%' )
                                     )";
                #endregion
                break;
            case "TR":
                #region FILTER
                filter = @"   FROM T_TimeRecMod
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

                            LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
                                ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
                               AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
                             INNER JOIN T_ApprovalRouteMaster AS routeMaster
                                ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId " + (!hasCCLine ? "" : @" LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                                             FROM E_CostCenterLineMaster 
												                                                                                WHERE Clm_Status = 'A' ) AS HASLINE
										                                                                              ON Clm_CostCenterCode = Trm_Costcenter ") + @"                

                             WHERE Emt_JobStatus <> 'IN' and
                                    --( (Trm_EmployeeId = '{0}')
                                  --OR (routeMaster.Arm_Checker1 = '{0}')
                                  --OR (routeMaster.Arm_Checker2 = '{0}')
                                  --OR (routeMaster.Arm_Approver = '{0}') )
                               --AND 
                                Trm_Status = '1'
                                AND Convert(varchar,GETDATE(),101) BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, GETDATE())" + (canEdit
                                ? @" AND Trm_Costcenter IN ( SELECT IIF( Cct_CostCenterCode = 'ALL', Trm_Costcenter, Cct_CostCenterCode)
                                                               FROM T_UserCostCenterAccess
                                                              INNER JOIN T_CostCenter
	                                                             ON ( Uca_CostCenterCode = 'ALL'
		                                                           OR Uca_CostCenterCode = Cct_CostCenterCode )
                                                              WHERE Uca_SytemID = 'TIMEKEEP'
	                                                            AND Uca_Usercode = '{0}'
	                                                            AND Uca_Status = 'A') "
                                : " AND Trm_EmployeeID = '{0}' ") + (!hasCCLine ? "" : string.Format(@"  AND ( ISNULL(Trm_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Trm_CostcenterLine, ''), (   SELECT Ucl_LineCode 
										                                                                                                                                                        FROM E_UserCostcenterLineAccess 
																														                                                                       WHERE Ucl_CostCenterCode = Trm_CostCenter
																														                                                                         AND Ucl_Status = 'A'
																														                                                                         AND Ucl_SystemID = 'TIMEKEEP'
																															                                                                     AND Ucl_LineCode = ISNULL(Trm_CostcenterLine, '')
																														                                                                         AND Ucl_UserCode = '{0}' )) )
										                                                                  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                                                                                         FROM E_UserCostcenterLineAccess 
																								                                                                WHERE Ucl_CostCenterCode = Trm_CostCenter
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
                                                                                                          OR Trm_EmployeeID = '{0}' )", Session["userLogged"].ToString())); ;

                searchFilter = @" AND( ( T_TimeRecMod.Trm_ControlNo LIKE '{0}%' )
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
                #endregion
                break;
            case "FT":
                break;
            case "JS":
                #region FILTER
                filter = @"";
                searchFilter = @"";
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

                              INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                 ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
                                AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
                               LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                 ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId " + (!hasCCLine ? "" : @" LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                                             FROM E_CostCenterLineMaster 
												                                                                                WHERE Clm_Status = 'A' ) AS HASLINE
										                                                                              ON Clm_CostCenterCode = Mve_Costcenter ") + @"   

                                LEFT join t_accountdetail ad9 on ad9.adt_accounttype = 'MOVETYPE' and ad9.adt_accountcode = mve_type
                                LEFT JOIN T_ShiftCodeMaster s0
			                        ON s0.Scm_ShiftCode = Mve_From
                               LEFT JOIN T_ShiftCodeMaster s1
                                    ON s1.Scm_ShiftCode = Mve_To
                               
                               WHERE Emt_JobStatus <> 'IN' and
                                    --( (Mve_EmployeeId = '{0}')
                                  --OR (routeMaster.Arm_Checker1 = '{0}')
                                  --OR (routeMaster.Arm_Checker2 = '{0}')
                                  --OR (routeMaster.Arm_Approver = '{0}') )
                               --AND 
                                Mve_Status = '1'
                                AND Convert(varchar,GETDATE(),101) BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, GETDATE())" + (canEdit
                                ? @" AND Mve_Costcenter IN ( SELECT IIF( Cct_CostCenterCode = 'ALL', Mve_Costcenter, Cct_CostCenterCode)
                                                               FROM T_UserCostCenterAccess
                                                              INNER JOIN T_CostCenter
	                                                             ON ( Uca_CostCenterCode = 'ALL'
		                                                           OR Uca_CostCenterCode = Cct_CostCenterCode )
                                                              WHERE Uca_SytemID = 'TIMEKEEP'
	                                                            AND Uca_Usercode = '{0}'
	                                                            AND Uca_Status = 'A') "
                                : " AND Mve_EmployeeID = '{0}' ") + (!hasCCLine ? "" : string.Format(@"  AND ( ISNULL(Mve_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Mve_CostcenterLine, ''), (   SELECT Ucl_LineCode 
										                                                                                                                                                        FROM E_UserCostcenterLineAccess 
																														                                                                       WHERE Ucl_CostCenterCode = Mve_CostCenter
																														                                                                         AND Ucl_Status = 'A'
																														                                                                         AND Ucl_SystemID = 'TIMEKEEP'
																															                                                                     AND Ucl_LineCode = ISNULL(Mve_CostcenterLine, '')
																														                                                                         AND Ucl_UserCode = '{0}' )) )
										                                                                  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                                                                                         FROM E_UserCostcenterLineAccess 
																								                                                                WHERE Ucl_CostCenterCode = Mve_CostCenter
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
                                                                                                          OR Mve_EmployeeID = '{0}' )", Session["userLogged"].ToString())); ;

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
                                    OR ( Mve_BatchNo LIKE '%{0}%')
                                    OR ( ad9.Adt_AccountDesc LIKE '%{0}%')
                                    OR ( dbo.getCostCenterFullNameV2(Mve_CostCenter) LIKE '%{0}%'))";
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

                filter += string.Format(searchFilter, holder);
            }
        }
        return string.Format(filter, Session["userLogged"].ToString());
    }
    private string getFinalColumns()
    {
        string columns = string.Empty;
        switch (hfTransactionType.Value.ToString().ToUpper())
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
                                    , [Applied Date]
                                    , [Informed Date]
                                    , [Endorsed Date]
                                    , [Reason]
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
                #region COLUMNS
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
                #endregion
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
            default:
                break;
        }
        return columns;
    }
    protected void ddlTransactions_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindGrid();
        UpdatePagerLocation();
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        bindGrid();
        UpdatePagerLocation();
    }
    protected void btnEndorse_Click(object sender, EventArgs e)
    {
        int countAffected = 0;
        int countCutOff = 0;

        #region Get Transaction Type Details
        string transactionSystemID = "";
        string strRouteTransactionID = "";
        string strWFTransactionType = ddlTransactions.SelectedValue.ToString().ToUpper();
        switch (strWFTransactionType)
        {
            case "OT":
                transactionSystemID = "OVERTIME";
                strRouteTransactionID = "OVERTIME";
                break;
            case "LV":
                transactionSystemID = "LEAVE";
                strRouteTransactionID = "LEAVE";
                break;
            case "TR":
                transactionSystemID = "TIMEKEEP";
                strRouteTransactionID = "TIMEMOD";
                break;
            //case "FT":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "FLEXTIME";
            //    break;
            //case "JS":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "JOBMOD";
            //    break;
            case "MV":
                transactionSystemID = "TIMEKEEP";
                strRouteTransactionID = "MOVEMENT";
                break;
            case "TX":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "TAXMVMNT";
                break;
            case "BF":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "BNEFICIARY";
                break;
            case "AD":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "ADDRESS";
                break;
            //case "SW":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "STRAIGHTWK";
            //    break;
        }
        #endregion

        string strStatus = "";
        string strForEndorseToChecker1 = "";
        string strForEndorseToChecker2 = "";
        string strForEndorseToApprover = "";

        CheckBox CB;
        for (int i = 0; i < dgvResult.Rows.Count; i++)
        {
            CB = (CheckBox)dgvResult.Rows[i].Cells[0].FindControl("chkBox");
            if (CB.Checked && dgvResult.Rows[i].Cells[2].Text.ToUpper().Trim().Equals("NEW"))
            {
                strStatus = CommonMethods.getStatusRoute(dgvResult.Rows[i].Cells[3].Text.ToUpper().Trim(), strRouteTransactionID, "ENDORSE TO CHECKER 1");
                if (strStatus == "3")
                    strForEndorseToChecker1 += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                else if (strStatus == "5")
                    strForEndorseToChecker2 += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
                else if (strStatus == "7")
                    strForEndorseToApprover += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
            }
        }

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                string strEndorseToChecker1Query = "";
                string strEndorseToChecker2Query = "";
                string strEndorseToApproverQuery = "";

                if (strForEndorseToChecker1 != "")
                {
                    strForEndorseToChecker1 = strForEndorseToChecker1.Substring(0, strForEndorseToChecker1.Length - 1);
                    strEndorseToChecker1Query = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' "
                                                                , strForEndorseToChecker1
                                                                , Session["userLogged"].ToString()
                                                                , strWFTransactionType
                                                                , "3"
                                                                , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                }
                if (strForEndorseToChecker2 != "")
                {
                    strForEndorseToChecker2 = strForEndorseToChecker2.Substring(0, strForEndorseToChecker2.Length - 1);
                    strEndorseToChecker2Query = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' "
                                                                , strForEndorseToChecker2
                                                                , Session["userLogged"].ToString()
                                                                , strWFTransactionType
                                                                , "5"
                                                                , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                }
                if (strForEndorseToApprover != "")
                {
                    strForEndorseToApprover = strForEndorseToApprover.Substring(0, strForEndorseToApprover.Length - 1);
                    strEndorseToApproverQuery = string.Format("EXEC EndorseWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}', '{5}' "
                                                                , strForEndorseToApprover
                                                                , Session["userLogged"].ToString()
                                                                , strWFTransactionType
                                                                , "7"
                                                                , Convert.ToBoolean(Resources.Resource.ALLOWFLOW)
                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                }

                DataSet dsResult = dal.ExecuteDataSet(string.Format(@"BEGIN TRY
                                                                        BEGIN TRANSACTION
                                                                        {0}
                                                                        {1}
                                                                        {2}
                                                                        COMMIT TRANSACTION
                                                                        END TRY
                                                                        BEGIN CATCH
                                                                        ROLLBACK TRANSACTION
                                                                        THROW;
                                                                        END CATCH
                                                                        ", strEndorseToChecker1Query, strEndorseToChecker2Query, strEndorseToApproverQuery));
                dtErrors.Rows.Clear();
                for (int i = 0; i < dsResult.Tables.Count; i++)
                {
                    ConsolidateResults(dsResult.Tables[i], ref countCutOff, ref countAffected);
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

        string cutOffMsg = string.Empty;
        if (countCutOff > 0)
        {
            cutOffMsg = cutOffMsg + countCutOff + " row(s) affected on CUT-OFF.\n";
            cutOffMsg = cutOffMsg + CommonMethods.GetErrorMessageForCutOff(transactionSystemID);
        }
        string affectedMsg = string.Empty;
        if (countAffected > 0)
        {
            affectedMsg = countAffected + " row(s) affected.";
            if (countCutOff > 0)
            {
                affectedMsg += "\nTransaction was approved because it belongs to the future quincena. Transaction does not affect the current payroll calculation.";
            }
            affectedMsg += "\n\n";
        }

        if (affectedMsg != string.Empty || cutOffMsg != string.Empty)
        {
            MessageBox.Show(affectedMsg + cutOffMsg);
        }

        bindGrid();
        UpdatePagerLocation();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        int countAffected = 0;
        int countCutOff = 0;

        #region Get Transaction Type Details
        string transactionSystemID = "";
        string strRouteTransactionID = "";
        string strWFTransactionType = ddlTransactions.SelectedValue.ToString().ToUpper();
        switch (strWFTransactionType)
        {
            case "OT":
                transactionSystemID = "OVERTIME";
                strRouteTransactionID = "OVERTIME";
                break;
            case "LV":
                transactionSystemID = "LEAVE";
                strRouteTransactionID = "LEAVE";
                break;
            case "TR":
                transactionSystemID = "TIMEKEEP";
                strRouteTransactionID = "TIMEMOD";
                break;
            //case "FT":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "FLEXTIME";
            //    break;
            //case "JS":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "JOBMOD";
            //    break;
            case "MV":
                transactionSystemID = "TIMEKEEP";
                strRouteTransactionID = "MOVEMENT";
                break;
            case "TX":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "TAXMVMNT";
                break;
            case "BF":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "BNEFICIARY";
                break;
            case "AD":
                transactionSystemID = "PAYROLL";
                strRouteTransactionID = "ADDRESS";
                break;
            //case "SW":
            //    transactionSystemID = "TIMEKEEP";
            //    strRouteTransactionID = "STRAIGHTWK";
            //    break;
        }
        #endregion

        string strForDisapproval = "";
        CheckBox CB;
        for (int i = 0; i < dgvResult.Rows.Count; i++)
        {
            CB = (CheckBox)dgvResult.Rows[i].Cells[0].FindControl("chkBox");
            if (CB.Checked && dgvResult.Rows[i].Cells[2].Text.ToUpper().Trim().Equals("NEW"))
            {
                strForDisapproval += dgvResult.Rows[i].Cells[1].Text.Trim() + ",";
            }
        }

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                string strForDisapprovalQuery = "";

                if (strForDisapproval != "")
                {
                    strForDisapproval = strForDisapproval.Substring(0, strForDisapproval.Length - 1);
                    strForDisapprovalQuery = string.Format("EXEC DisapproveWFTransaction '{0}', '{1}', '{2}', '{3}', '{4}' "
                                                                , strForDisapproval
                                                                , Session["userLogged"].ToString()
                                                                , ""
                                                                , strWFTransactionType
                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
                }

                DataSet dsResult = dal.ExecuteDataSet(string.Format(@"BEGIN TRY
                                                                        BEGIN TRANSACTION
                                                                        {0}
                                                                        COMMIT TRANSACTION
                                                                        END TRY
                                                                        BEGIN CATCH
                                                                        ROLLBACK TRANSACTION
                                                                        THROW;
                                                                        END CATCH
                                                                        ", strForDisapprovalQuery));
                dtErrors.Rows.Clear();
                for (int i = 0; i < dsResult.Tables.Count; i++)
                {
                    ConsolidateResults(dsResult.Tables[i], ref countCutOff, ref countAffected);
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

        string cutOffMsg = string.Empty;
        if (countCutOff > 0)
        {
            cutOffMsg = cutOffMsg + countCutOff + " row(s) affected on CUT-OFF.\n";
            cutOffMsg = cutOffMsg + CommonMethods.GetErrorMessageForCutOff(transactionSystemID);
        }
        string affectedMsg = string.Empty;
        if (countAffected > 0)
        {
            affectedMsg = countAffected + " row(s) affected.";
            if (countCutOff > 0)
            {
                affectedMsg += "\nTransaction was approved because it belongs to the future quincena. Transaction does not affect the current payroll calculation.";
            }
            affectedMsg += "\n\n";
        }

        if (affectedMsg != string.Empty || cutOffMsg != string.Empty)
        {
            MessageBox.Show(affectedMsg + cutOffMsg);
        }

        bindGrid();
        UpdatePagerLocation();
    }

    private void ConsolidateResults(DataTable dtResult, ref int countCutOff, ref int countAffected)
    {
        if (dtResult.Rows.Count > 0)
        {
            DataRow[] drArrRows;
            drArrRows = dtResult.Select("Result = 53000");
            countCutOff += drArrRows.Length;
            drArrRows = dtResult.Select("Result = 1");
            countAffected += drArrRows.Length;
            drArrRows = dtResult.Select("Result <> 1 and Result <> 53000");
            foreach (DataRow row in drArrRows)
            {
                dtErrors.Rows.Add(dtErrors.NewRow());
                dtErrors.Rows[dtErrors.Rows.Count - 1]["Control No"] = row["ControlNo"].ToString();
                dtErrors.Rows[dtErrors.Rows.Count - 1]["Exception"] = row["Message"].ToString();
            }
        }
    }
}