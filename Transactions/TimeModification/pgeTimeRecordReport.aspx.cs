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

public partial class Transactions_TimeModification_pgeTimeRecordReport : System.Web.UI.Page
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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFWORKREC"))
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
        btnCostcenter.OnClientClick = string.Format("return lookupWRRepCostCenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRTKCostcenterLine()");
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
       //MenuLog
        //SystemMenuLogBL.InsertGenerateLog("WFWORKREC", txtEmployee.Text, true, Session["userLogged"].ToString());

    }

    protected void dgvResult_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int index = 3;
        if (cblInclude.Items[0].Selected)
            index++;
        if (cblInclude.Items[2].Selected)
            index++;
        if (cblInclude.Items[3].Selected)
            index++;
        if (cblInclude.Items[4].Selected)
            index++;
        if (cblInclude.Items[5].Selected)
            index++;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[index].Text.Contains("REG"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORREG);
            }
            else if (e.Row.Cells[index].Text.Contains("HOL"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORHOL);
            }
            else if (e.Row.Cells[index].Text.Contains("REST"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORREST);
            }
            else if (e.Row.Cells[index].Text.Contains("COMP"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORCOMP);
            }
            else if (e.Row.Cells[index].Text.Contains("SPL"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORSPL);
            }
            else
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLOROTHERS);
            }
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'; this.style.color='blue';this.style.fontWeight='normal'";
            //e.Row.Attributes["onmouseout"] = ";this.style.color='black';this.style.fontWeight='normal';";
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'";
            e.Row.Attributes["onclick"] = "gridSelectTimeRecEntry('" + e.Row.RowIndex.ToString() + "');";
            if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
            {
                e.Row.Attributes["ondblclick"] = "gridViewTimeRecEntry('" + Encrypt.encryptText(e.Row.Cells[index - 2].Text) + "','" + Encrypt.encryptText(e.Row.Cells[index + 18].Text) + "');";
            }
            else
            {
                e.Row.Attributes["ondblclick"] = "gridViewTimeRecEntry('" + Encrypt.encryptText(e.Row.Cells[index - 2].Text) + "','" + Encrypt.encryptText(e.Row.Cells[index + 17].Text) + "');";
            }
        }
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
                    ds.Tables[0].Columns.Remove("Last Name");
                    //ds.Tables[0].Columns.Remove("First Name");
                    ds.Tables[0].Columns.Remove("Middle Name");
                }

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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "TIME RECORD REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "TIME RECORD REPORT");
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
                    ds.Tables[0].Columns.Remove("Last Name");
                    //ds.Tables[0].Columns.Remove("First Name");
                    ds.Tables[0].Columns.Remove("Middle Name");
                }

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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "TIME RECORD REPORT", initializeExcelHeader());
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
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFWORKREC");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);

            txtEmployee.Text = Session["userLogged"].ToString();
            tbrEmployee.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
            tbrCostcenter.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
        }
        else
        {
            btnExport.Enabled = false;
            btnPrint.Enabled = false;
        }
        if (!tbrEmployee.Visible)
        {
            dtpDateFrom.CalendarOffsetY = Unit.Pixel(0);
            dtpDateTo.CalendarOffsetY = Unit.Pixel(0);
        }
        else
        {
            dtpDateFrom.CalendarOffsetY = Unit.Pixel(-120);
            dtpDateTo.CalendarOffsetY = Unit.Pixel(-120);
        }

        dtpDateFrom.Date = CommonMethods.getQuincenaDate('C', "START");
        dtpDateTo.Date = CommonMethods.getQuincenaDate('C', "END");
        dtpDateFrom.MinDate = dtpDateTo.MinDate = CommonMethods.getMinimumDate();
        dtpDateFrom.MaxDate = dtpDateTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");

        if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
        {
            lblAbsentHours.Text = "Tardiness Hour(s)";
        }
        lblComputationDateValue.Text = getLastCompuationDate();
    }
    private string getLastCompuationDate()
    {
        string strLastComputationDate = "NO COMPUTATION YET";
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                string date = dal.ExecuteScalar(@"SELECT TOP 1 Ludatetime
FROM T_SystemMenuLog
WHERE Sml_MenuCode = 'BATCHPAYRLLCALC'
       AND Sml_Action = 'G'
       AND Sml_IsSuccess = 1
       AND Sml_CurrentPayPeriod = '2014041'--(SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C')
ORDER BY Ludatetime DESC
").ToString();
                if (date != null && date.Trim() != "")
                    strLastComputationDate = date;
            }
            catch (Exception e)
            { }
            finally
            { dal.CloseDB();}
        }
        return strLastComputationDate;
    }
    private string SQLBuilder(string replaceString)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Last Name],[First Name], [Pay Period], [Process Date]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeLogLedger", "T_EmployeeLogLedgerHist"));
        sql.Append(@"                              ) AS temp)
                                           SELECT [Pay Period]
                                                , [ID Number]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Last Name]
                                                , [First Name]
                                                , [Middle Name]
                                                , [Cost Center Code]
                                                , [Cost Center Desc]
                                                , [Process Date]
                                                , [DoW]
                                                , [Day Code]
                                                , [Shift for the Day]
                                                , [Time In 1]
                                                , [Time Out 1]
                                                , [Time In 2]
                                                , [Time Out 2]
                                                , [Hours In Office]
                                                , [Work Group]
                                                , [Work Type]
                                                , [Paid Leave]
                                                , [Unpaid Leave]
                                                , [Approved Adv OT]
                                                , [Approved Post OT]
                                                , [Regular Hour(s)] 
                                                , [Regular Night Prem Hour(s)]
                                                , [Overtime Hour(s)]
                                                , [Overtime Night Prem Hour(s)]
                                                , [Location]
                                                , [Employee ID]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            ");
        //Perth Added 03/20/2012
        sql.Append(@" order by  [Last Name],[First Name],[Middle Name], [Process Date]");
        //End

        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(*) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"           UNION 
                               SELECT COUNT(*)");
        sql.Append(getFilters().Replace("T_EmployeeLogLedger", "T_EmployeeLogLedgerHist"));
        sql.Append(@"        ) as Rows");

        //Summary
        sql.Append(@" SELECT SUM([Regular]) [Regular]
	                       , SUM([Absent]) [Absent]
	                       , SUM([Paid Leave]) [Paid Leave]
	                       , SUM([Unpaid Leave]) [Unpaid Leave]
	                       , SUM([Post OT]) [Post OT]
	                       , SUM([Advance OT]) [Advance OT]
                        FROM ( ");
        sql.Append(@" SELECT distinct Ell_EmployeeId [Employee ID]
                           , Ell_PayPeriod [Pay Period]
	                       , SUM(CASE WHEN Ell_Holiday=0 AND Ell_RestDay=0 THEN Ell_RegularHour ELSE 0 END) [Regular]
	                       , SUM(Ell_AbsentHour) [Absent]
	                       , SUM(Ell_EncodedPayLeaveHr) [Paid Leave]
	                       , SUM(Ell_EncodedNoPayLeaveHr) [Unpaid Leave]
	                       , SUM(Ell_EncodedOvertimePostHr) [Post OT]
	                       , SUM(Ell_EncodedOvertimeAdvHr) [Advance OT] ");
        sql.Append(getFilters().Replace("!#!", string.Empty));
        sql.Append(@"  GROUP BY Ell_EmployeeId
                              , Ell_PayPeriod ");
        sql.Append(" UNION ");
        sql.Append(@" SELECT distinct Ell_EmployeeId [Employee ID]
                           , Ell_PayPeriod [Pay Period]
	                       , SUM(Ell_RegularHour) [Regular]
	                       , SUM(Ell_AbsentHour) [Absent]
	                       , SUM(Ell_EncodedPayLeaveHr) [Paid Leave]
	                       , SUM(Ell_EncodedNoPayLeaveHr) [Unpaid Leave]
	                       , SUM(Ell_EncodedOvertimePostHr) [Post OT]
	                       , SUM(Ell_EncodedOvertimeAdvHr) [Advance OT] ");
        sql.Append(getFilters().Replace("T_EmployeeLogLedger", "T_EmployeeLogLedgerHist").Replace("!#!", string.Empty).Replace("@Sort", Resources.Resource.REPORTSORTING));
        sql.Append(@"  GROUP BY Ell_EmployeeId
                              , Ell_PayPeriod ");
        sql.Append(@" ) AS TEMP2 ");
        return sql.ToString().Replace("!#!", replaceString).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT 
                            distinct Ell_EmployeeId [ID Number]
                          , Ell_PayPeriod [Pay Period]
                          , Ell_EmployeeId [Employee ID]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Last Name]
                          , Emt_Firstname [First Name]
                          , Emt_MiddleName [Middle Name]
                          , Emt_CostCenterCode [Cost Center Code]
                          , dbo.getCostcenterFullNameV2(Emt_CostCenterCode) [Cost Center Desc]
                          , Convert(varchar(10), Ell_ProcessDate, 101) [Process Date]
                          , UPPER(LEFT(DATENAME(dw, Ell_ProcessDate), 3)) [DoW]
                          , Ell_DayCode [Day Code]
                          , '[' + Ell_ShiftCode +'] '
                          + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
                          + '-'
                          + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
                          + '  ' 
                          + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
                          + '-'
                          + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [Shift for the Day]
                          , CASE Ell_ActualTimeIn_1
                            WHEN '0000'
                            THEN ''
                            ELSE LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1,2)
                             END [Time In 1]
                          , CASE Ell_ActualTimeOut_1
                            WHEN '0000'
                            THEN ''
                            ELSE LEFT(Ell_ActualTimeOut_1,2) + ':' + RIGHT(Ell_ActualTimeOut_1,2)
                             END [Time Out 1]
                          , CASE Ell_ActualTimeIn_2
                            WHEN '0000'
                            THEN ''
                            ELSE LEFT(Ell_ActualTimeIn_2,2) + ':' + RIGHT(Ell_ActualTimeIn_2,2)
                             END as [Time In 2]
                          , CASE Ell_ActualTimeOut_2
                            WHEN '0000'
                            THEN ''
                            ELSE LEFT(Ell_ActualTimeOut_2,2) + ':' + RIGHT(Ell_ActualTimeOut_2,2)
                             END as [Time Out 2]
                          , CASE WHEN LTRIM(RTRIM(Ell_EncodedPayLeaveType + ' ' + Cast(Ell_EncodedPayLeaveHr as char))) = '0.00'
                                 THEN ''
                                 ELSE Ell_EncodedPayLeaveType + ' ' + Cast(Ell_EncodedPayLeaveHr as char) 
                             END [Paid Leave]
                          , CASE
                            WHEN LTRIM(RTRIM(Ell_EncodedNoPayLeaveType + ' ' + Cast(Ell_EncodedNoPayLeaveHr as char))) = '0.00'
                            THEN ''
                            ELSE Ell_EncodedNoPayLeaveType + ' ' + Cast(Ell_EncodedNoPayLeaveHr as char)
                             END [Unpaid Leave]
                          , CASE Cast(Ell_EncodedOvertimeAdvHr as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(Ell_EncodedOvertimeAdvHr as char)
                             END [Approved Adv OT]
                          , CASE Cast(Ell_EncodedOvertimePostHr as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(Ell_EncodedOvertimePostHr as char) 
                             END [Approved Post OT]
                          , CASE Cast(CASE WHEN Ell_DayCode <> 'REG'
                                           THEN '0.00' 
                                           ELSE Ell_RegularHour
                                       END as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(Ell_RegularHour as char) 
                             END [Regular Hour(s)] 
                          , CASE Cast(Ell_RegularNightPremHour as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(Ell_RegularNightPremHour as char) 
                             END [Regular Night Prem Hour(s)]
                          , CASE Cast(CASE WHEN Ell_DayCode <> 'REG'
                                           THEN Ell_RegularHour+Ell_OvertimeHour
                                           ELSE Ell_OvertimeHour
                                       END as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(CASE WHEN Ell_DayCode <> 'REG'
                                           THEN Ell_RegularHour+Ell_OvertimeHour
                                           ELSE Ell_OvertimeHour
                                       END as char) 
                             END [Overtime Hour(s)]
                          , CASE Cast(Ell_OvertimeNightPremHour as char)
                            WHEN '0.00'
                            THEN ''
                            ELSE Cast(Ell_OvertimeNightPremHour as char) 
                             END [Overtime Night Prem Hour(s)]
                          , ISNULL(AD1.Adt_AccountDesc, Ell_LocationCode + ' - zipcode not set') [Location] 
                          , AD2.Adt_AccountDesc [Work Group]
                          , AD3.Adt_AccountDesc [Work Type] ";

        if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
        {
            //This will get the total hours spent in the office by the employee per date.
            //Scenarios captured for the logs which are paried.
            columns += @" , CONVERT(decimal(8,2),
	                        CASE WHEN (Ell_ActualTimeIn_1 <> '0000' AND Ell_ActualTimeOut_1 <> '0000' AND Ell_ActualTimeIn_2 <> '0000' AND Ell_ActualTimeOut_2 <> '0000')
			                    THEN ( DATEDIFF(MINUTE, dbo.getDatetimeFormatV2( Ell_ActualTimeIn_1, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')
					                                    , dbo.getDatetimeFormatV2( Ell_ActualTimeOut_1, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')) 
			                            + DATEDIFF(MINUTE, dbo.getDatetimeFormatV2( Ell_ActualTimeIn_2, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')
					                                    , dbo.getDatetimeFormatV2( Ell_ActualTimeOut_2, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000'))) / 60.00
			                    ELSE CASE WHEN (Ell_ActualTimeIn_1 <> '0000' AND Ell_ActualTimeOut_2 <> '0000')
					                        THEN DATEDIFF(MINUTE, dbo.getDatetimeFormatV2( Ell_ActualTimeIn_1, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')
					                                            , dbo.getDatetimeFormatV2( Ell_ActualTimeOut_2, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')) / 60.00
					                        ELSE CASE WHEN (Ell_ActualTimeIn_1 <> '0000' AND Ell_ActualTimeOut_1 <> '0000')
								                    THEN DATEDIFF(MINUTE, dbo.getDatetimeFormatV2( Ell_ActualTimeIn_1, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')
												                        , dbo.getDatetimeFormatV2( Ell_ActualTimeOut_1, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')) / 60.00
								                    ELSE CASE WHEN (Ell_ActualTimeIn_2 <> '0000' AND Ell_ActualTimeOut_2 <> '0000')
										                        THEN  DATEDIFF(MINUTE, dbo.getDatetimeFormatV2( Ell_ActualTimeIn_2, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')
												                                    , dbo.getDatetimeFormatV2( Ell_ActualTimeOut_2, Convert(varchar(10), Ell_ProcessDate, 101), 'P', Scm_ScheduleType, '0000')) / 60.00
									                            ELSE '0'
									                        END
							                    END
			                            END
	                        END) [Hours In Office]";
        }
        else
        {
            columns += ", 0 [Hours In Office] ";
        }
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string filter = string.Empty;
        filter =string.Format( @"    FROM T_EmployeeLogLedger
                       LEFT JOIN T_EmployeeMaster
                         ON Emt_EmployeeID = Ell_EmployeeId
                       LEFT JOIN T_ShiftCodeMaster
                         ON Scm_ShiftCode = Ell_ShiftCode
                       LEFT JOIN T_AccountDetail AD1
                         ON AD1.Adt_AccountType ='ZIPCODE'
                        AND AD1.Adt_AccountCode = Ell_LocationCode
                       LEFT JOIN T_AccountDetail AD2
                         ON AD2.Adt_AccountCode = Ell_WorkGroup
                        AND AD2.Adt_AccountType = 'WORKGROUP'
                       LEFT JOIN T_AccountDetail AD3
                         ON AD3.Adt_AccountCode = Ell_WorkType
                        AND AD3.Adt_AccountType = 'WORKTYPE'
                        {0}
                      WHERE LEFT(Emt_JobStatus,1) = 'A' ", !hasCCLine ? "" : @"---apsungahid added for line code access filter 20141211
                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                      FROM E_CostCenterLineMaster 
												                                                         WHERE Clm_Status = 'A' ) AS HASLINE
										                                                       ON Clm_CostCenterCode = Emt_CostCenterCode 

					                                                                        LEFT JOIN E_EmployeeCostCenterLineMovement
					                                                                        ON Ecm_EmployeeID = Emt_EmployeeID
					                                                                        AND Ell_ProcessDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))"); 
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
            filter += string.Format(@" AND  ( Emt_CostCenterCode {0}
                                           OR dbo.getCostCenterFullNameV2(Emt_CostCenterCode) {0}) ", sqlINFormat(txtCostcenter.Text)
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
                filter += string.Format(@" AND  (  ( Emt_CostcenterCode IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'TIMEKEEP')
                                                      OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }
            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND ( ISNULL(Ecm_LineCode, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Ecm_LineCode, ''), ( SELECT Ucl_LineCode 
										                                                                                            FROM E_UserCostcenterLineAccess 
																														           WHERE (Ucl_CostCenterCode = Emt_CostcenterCode OR Ucl_CostCenterCode = 'ALL')
																														             AND Ucl_Status = 'A'
																														             AND Ucl_SystemID = 'TIMEKEEP'
																															         AND Ucl_LineCode = ISNULL(Ecm_LineCode, '')
																														             AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Emt_CostcenterCode
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
                                          OR Emt_EmployeeID = '{0}' )", Session["userLogged"].ToString());
            }
        }
        #endregion
        #region DateTime Pickers
        //Process Date
        if (!dtpDateFrom.IsNull && !dtpDateTo.IsNull)
        {
            filter += string.Format(@" AND Ell_ProcessDate BETWEEN '{0}' AND '{1}'", dtpDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpDateFrom.IsNull)
        {
            filter += string.Format(@" AND Ell_ProcessDate >= '{0}'", dtpDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpDateTo.IsNull)
        {
            filter += string.Format(@" AND Ell_ProcessDate <= '{0}'", dtpDateTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @" AND ( ( Ell_PayPeriod LIKE '{0}%' )
                                        OR ( Ell_EmployeeId LIKE '%{0}%' )
                                        OR ( Emt_NickName LIKE '%{0}%' )
                                        OR ( Emt_Lastname LIKE '%{0}%' )
                                        OR ( Emt_Firstname LIKE '%{0}%' )
                                        OR ( Convert(varchar(10),Ell_ProcessDate,101) LIKE '%{0}%' )
                                        OR ( LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1,2) LIKE '{0}%' )
                                        OR ( LEFT(Ell_ActualTimeOut_1,2) + ':' + RIGHT(Ell_ActualTimeOut_1,2) LIKE '{0}%' )
                                        OR ( LEFT(Ell_ActualTimeIn_2,2) + ':' + RIGHT(Ell_ActualTimeIn_2,2) LIKE '{0}%' )
                                        OR ( LEFT(Ell_ActualTimeOut_2,2) + ':' + RIGHT(Ell_ActualTimeOut_2,2) LIKE '{0}%' )
                                        OR ( dbo.getCostCenterFullNameV2(Emt_CostCenterCode) LIKE '%{0}%' )
                                        OR ( DATENAME(dw, Ell_ProcessDate) LIKE '%{0}%' )
                                        OR ( Ell_DayCode LIKE '{0}%' )
                                        OR ( '[' + Ell_ShiftCode +'] '
                                             + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
                                             + '-'
                                             + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
                                             + '  ' 
                                             + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
                                             + '-'
                                             + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) LIKE '%{0}%' )
                                        OR ( CASE WHEN LTRIM(RTRIM(Ell_EncodedPayLeaveType + ' ' + Cast(Ell_EncodedPayLeaveHr as char))) = '0.00'
                                                  THEN ''
                                                  ELSE Ell_EncodedPayLeaveType + ' ' + Cast(Ell_EncodedPayLeaveHr as char) 
                                              END LIKE '{0}%' )
                                        OR ( CASE WHEN LTRIM(RTRIM(Ell_EncodedNoPayLeaveType + ' ' + Cast(Ell_EncodedNoPayLeaveHr as char))) = '0.00'
                                                  THEN ''
                                                  ELSE Ell_EncodedNoPayLeaveType + ' ' + Cast(Ell_EncodedNoPayLeaveHr as char)
                                              END LIKE '{0}%' )
                                        OR ( CASE Cast(Ell_EncodedOvertimeAdvHr as char)
                                             WHEN '0.00'
                                             THEN ''
                                             ELSE Cast(Ell_EncodedOvertimeAdvHr as char)
                                              END LIKE '{0}%' )
                                        OR ( CASE Cast(Ell_EncodedOvertimePostHr as char)
                                             WHEN '0.00'
                                             THEN ''
                                             ELSE Cast(Ell_EncodedOvertimePostHr as char) 
                                              END LIKE '{0}%' )
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
                string str = SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString());
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
                ds.Tables[0].Columns.Remove("Last Name");
                //ds.Tables[0].Columns.Remove("First Name");
                ds.Tables[0].Columns.Remove("Middle Name");
            }

            if (!Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
            {
                ds.Tables[0].Columns.Remove("Hours In Office");
            }

            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                if (!cblInclude.Items[i].Selected)
                    ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
            }
            #endregion

            lblRegularHoursValue.Text = ds.Tables[2].Rows[0]["Regular"].ToString();
            lblAbsentHoursValue.Text = ds.Tables[2].Rows[0]["Absent"].ToString();
            lblPostOvertimeHoursValue.Text = ds.Tables[2].Rows[0]["Post OT"].ToString();
            lblAdvanceOvertimeHoursValue.Text = ds.Tables[2].Rows[0]["Advance OT"].ToString();
            lblPaidLeaveHoursValue.Text = ds.Tables[2].Rows[0]["Paid Leave"].ToString();
            lblUnpaidLeaveHoursValue.Text = ds.Tables[2].Rows[0]["Unpaid Leave"].ToString();
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
        if (!dtpDateFrom.IsNull)
        {
            criteria += lblDateFrom.Text + ":" + dtpDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpDateTo.IsNull)
        {
            criteria += lblDateTo.Text + ":" + dtpDateTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        return criteria.Trim();
    }
    #endregion

}
