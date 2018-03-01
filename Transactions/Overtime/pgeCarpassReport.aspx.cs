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

public partial class Transactions_Overtime_pgeCarpassReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();
    OvertimeBL OTBL = new OvertimeBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFCRPASREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
            PreRender += new EventHandler(Transactions_Overtime_pgeOvertimeReport_PreRender);
        }
        LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeReport_LoadComplete);
    }

    #region Events
    void Transactions_Overtime_pgeOvertimeReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "overtimeScripts";
        string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupROTEmployee()");
        btnStatus.OnClientClick = string.Format("return lookupROTStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupROTCostcenter()");
        btnPayPeriod.OnClientClick = string.Format("return lookupROTPayPeriod()");
        btnBatchNo.OnClientClick = string.Format("return lookupROTBatchNo()");
        btnChecker1.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_ApprovedBy','txtApprover')");
    }

    void Transactions_Overtime_pgeOvertimeReport_PreRender(object sender, EventArgs e)
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
        SystemMenuLogBL.InsertGenerateLog("WFCRPASREP", txtEmployee.Text, true, Session["userLogged"].ToString());
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
                    if (rblReportType.SelectedValue.Equals("D"))
                    {
                        ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                    }
                    else
                    {
                        ds = dal.ExecuteDataSet(SQLBuilderSummary("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                    }
                    //ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                if (rblReportType.SelectedValue.Equals("D"))
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
                    //DASH Specific
                    ds.Tables[0].Columns.Remove("Job Code");
                    ds.Tables[0].Columns.Remove("Client Job Name");
                    ds.Tables[0].Columns.Remove("Client Job No");
                    ds.Tables[0].Columns.Remove("DASH Class Code");
                    ds.Tables[0].Columns.Remove("DASH Work Code");
                    //Depending if Used
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

                    //Includes
                    for (int i = 0; i < cblInclude.Items.Count; i++)
                    {
                        if (!cblInclude.Items[i].Selected)
                            ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                    }
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "CARPASS REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Carpass Report");
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
                    if (rblReportType.SelectedValue.Equals("D"))
                    {
                        ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                    }
                    else
                    {
                        ds = dal.ExecuteDataSet(SQLBuilderSummary("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                    }
                    //ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                if (rblReportType.SelectedValue.Equals("D"))
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
                    //DASH Specific
                    ds.Tables[0].Columns.Remove("Job Code");
                    ds.Tables[0].Columns.Remove("Client Job Name");
                    ds.Tables[0].Columns.Remove("Client Job No");
                    ds.Tables[0].Columns.Remove("DASH Class Code");
                    ds.Tables[0].Columns.Remove("DASH Work Code");
                    //Depending if Used
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                    ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

                    //Includes
                    for (int i = 0; i < cblInclude.Items.Count; i++)
                    {
                        if (!cblInclude.Items[i].Selected)
                            ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                    }
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "CARPASS REPORT", initializeExcelHeader());
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
        OTBL.initializeOTTypes(ddlType, true);
        btnGenerate.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFOTREP");
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
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'";
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
                                btnFiller1.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller1','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
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
                                btnFiller2.Attributes.Add("OnClick", "javascript:return lookupGenericFillerMultiple(" + "'txtFiller2','" + ds.Tables[0].Rows[i]["Cfl_Lookup"].ToString() + "');");
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

    private string SQLBuilder(string replaceString)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Lastname], [OT Date] @Sort) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@"                              ) AS temp)
                                           SELECT [Control No]
                                                , [Status]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [@Eot_Filler1Desc]
                                                , [@Eot_Filler2Desc]
                                                , [@Eot_Filler3Desc]
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
                        FROM ( SELECT COUNT(Eot_EmployeeId) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"           UNION 
                               SELECT COUNT(Eot_EmployeeId)");
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@"        ) as Rows");

        sql = sql.Replace("@Eot_Filler1Desc", CommonMethods.getFillerName("Eot_Filler01"));
        sql = sql.Replace("@Eot_Filler2Desc", CommonMethods.getFillerName("Eot_Filler02"));
        sql = sql.Replace("@Eot_Filler3Desc", CommonMethods.getFillerName("Eot_Filler03"));
        return sql.ToString().Replace("!#!", replaceString).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }

    private string SQLBuilderSummary(string replaceString)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY  [OT Date] @Sort, [Route Name], [Route Time]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns2());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns2());
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@"                              ) AS temp)
                                           SELECT [OT Date]
                                                , [Route Code]
                                                , [Route Name]
                                                , [Route Type] 
                                                , CASE WHEN ([Route Time] = 'N/A')
                                                       THEN 'N/A'
                                                       ELSE CASE WHEN(datepart(hour,Convert(datetime,[Route Time])) < 10)
                                                                 THEN '0'
						                                         ELSE ''
                                                             END 
                                                   END
                                                + CASE WHEN ([Route Time] = 'N/A')
                                                       THEN ''
                                                       ELSE Convert(varchar(2),datepart(hour,Convert(datetime,[Route Time])))
                                                   END
                                                + CASE WHEN ([Route Time] = 'N/A')
                                                       THEN ''
                                                       ELSE ':'
                                                   END
                                                + CASE WHEN ([Route Time] = 'N/A')
                                                       THEN ''
                                                       ELSE Convert(varchar(2),datepart(minute,Convert(datetime,[Route Time]))) 
                                                   END [Route Time]
                                                , COUNT([Control No]) [COUNT]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            GROUP BY [Route Name]
                                                   , [Route Code]
                                                   , [Route Type]
                                                   , [Route Time]
                                                   , [OT Date]  
                                            ");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(Eot_ControlNo) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"           UNION 
                               SELECT COUNT(Eot_ControlNo)");
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@"        ) as Rows");

        return sql.ToString().Replace("!#!", replaceString).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }
    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Eot_ControlNo [Control No]
                          , Eot_EmployeeId [ID No]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname]
                          , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]
                          , Pmx_ParameterDesc [Type]
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
        return columns;
    }

    private string getColumns2()
    {
        string columns = string.Empty;
        columns = @" SELECT AD2.Adt_AccountCode [Route Code]
                          , SUBSTRING(LEFT(AD2.Adt_AccountDesc,CHARINDEX('-',AD2.Adt_AccountDesc ) - 1),3, 100) [Route Name]
                          , CASE LEFT(AD2.Adt_AccountDesc,1)
                            WHEN 'X' THEN '-not applicable-'
                            WHEN 'I' THEN 'IN-COMING'
                            WHEN 'O' THEN 'OUT-GOING'
                             END [Route Type]
                          , CASE WHEN LEFT(AD2.Adt_AccountDesc,1) = 'X'
                                 THEN 'N/A'
                                 ELSE CONVERT(varchar(8), LTRIM(RTRIM(SUBSTRING(AD2.Adt_AccountDesc,CHARINDEX('-',AD2.Adt_AccountDesc ) + 1,LEN(AD2.Adt_AccountDesc)))))
                             END [Route Time]
                          , Eot_ControlNo [Control No]
                          , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
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
                     WHERE 1 = 1 AND Eot_Status <> '' 
                       AND Eot_Filler1 <> ''
                       AND Eot_Filler1 IS NOT NULL ";
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
            filter += string.Format(@" AND  ( Eot_Costcenter {0}
                                           OR dbo.getCostCenterFullNameV2(Eot_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                     , Session["userLogged"].ToString());
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
            {
                filter += string.Format(@" AND  (  ( Eot_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                            FROM T_UserCostCenterAccess
                                                                        WHERE Uca_UserCode = '{0}'
                                                                            AND Uca_SytemId = 'OVERTIME')
                                                    OR Eot_EmployeeId = '{0}'))", Session["userLogged"].ToString());
            }
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_Status {0}
                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
        }
        if (!txtBatchNo.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_BatchNo {0})", sqlINFormat(txtBatchNo.Text));
        }
        if (!txtFiller1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_Filler1 {0}
                                           OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtFiller1.Text));
        }
        if (!txtFiller2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_Filler2 {0}
                                           OR AD3.Adt_AccountDesc {0})", sqlINFormat(txtFiller2.Text));
        }
        if (!txtFiller3.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_Filler3 {0}
                                           OR AD4.Adt_AccountDesc {0})", sqlINFormat(txtFiller3.Text));
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_CheckedBy {0}
                                           OR C1.Umt_UserCode {0}
                                           OR C1.Umt_UserLname {0}
                                           OR C1.Umt_UserFname {0}
                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_Checked2By {0}
                                           OR C2.Umt_UserCode {0}
                                           OR C2.Umt_UserLname {0}
                                           OR C2.Umt_UserFname {0}
                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Eot_ApprovedBy {0}
                                           OR AP.Umt_UserCode {0}
                                           OR AP.Umt_UserLname {0}
                                           OR AP.Umt_UserFname {0}
                                           OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
        }
        #endregion
        if (!ddlType.SelectedValue.Equals("ALL"))
        {
            filter += string.Format(@" AND Eot_OvertimeType = '{0}'", ddlType.SelectedValue);
        }
        if (!cbxDefaultOT.Checked)
        {
            filter += @" AND Eot_ControlNo LIKE 'V%'";
        }
        #region DateTime Pickers
        //Overtime Date
        if (!dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
        {
            filter += string.Format(@" AND Eot_OvertimeDate BETWEEN '{0}' AND '{1}'", dtpOTDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpOTDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpOTDateFrom.IsNull)
        {
            filter += string.Format(@" AND Eot_OvertimeDate >= '{0}'", dtpOTDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpOTDateTo.IsNull)
        {
            filter += string.Format(@" AND Eot_OvertimeDate <= '{0}'", dtpOTDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Applied Date
        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Eot_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedFrom.IsNull)
        {
            filter += string.Format(@" AND Eot_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Eot_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Endorsed Date
        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Eot_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedFrom.IsNull)
        {
            filter += string.Format(@" AND Eot_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Eot_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date
        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Eot_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1From.IsNull)
        {
            filter += string.Format(@" AND Eot_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Eot_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date 2
        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Eot_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2From.IsNull)
        {
            filter += string.Format(@" AND Eot_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Eot_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        //Approved Date
        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Eot_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPFrom.IsNull)
        {
            filter += string.Format(@" AND Eot_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Eot_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Eot_ControlNo LIKE '{0}%' )
                                          OR ( Eot_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                          OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 4)) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_OvertimeDate,101) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_AppliedDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Eot_AppliedDate,114),5) LIKE '%{0}%' )
                                          OR ( Pmx_ParameterDesc LIKE '{0}%' )
                                          OR ( LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) LIKE '{0}%' )
                                          OR ( LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) LIKE '{0}%' )
                                          OR ( Eot_OvertimeHour LIKE '{0}%' )
                                          OR ( Eot_Reason LIKE '{0}%' )
                                          OR ( Slm_DashWorkCode LIKE '{0}%' )
                                          OR ( Slm_ClientJobName LIKE '{0}%' )
                                          OR ( Eot_JobCode LIKE '{0}%' )
                                          OR ( Eot_ClientJobNo LIKE '{0}%' )
                                          OR ( Slm_DashClassCode LIKE '{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_EndorsedDateToChecker,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Eot_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                          OR ( Eot_Filler1 LIKE '{0}%' )
                                          OR ( Eot_Filler2 LIKE '{0}%' )
                                          OR ( Eot_Filler3 LIKE '{0}%' )
                                          OR ( Eot_BatchNo LIKE '{0}%' )
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

        if(rblReportType.SelectedValue.Equals("S"))
        {
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
            lblRowNo.Text = "-paging not appicable-";
        }
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                if (rblReportType.SelectedValue.Equals("D"))
                {
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
                }
                else
                {
                    ds = dal.ExecuteDataSet(SQLBuilderSummary("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
        if (!CommonMethods.isEmpty(ds))
        {
            if(rblReportType.SelectedValue.Equals("D"))
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
                //DASH Specific
                ds.Tables[0].Columns.Remove("Job Code");
                ds.Tables[0].Columns.Remove("Client Job Name");
                ds.Tables[0].Columns.Remove("Client Job No");
                ds.Tables[0].Columns.Remove("DASH Class Code");
                ds.Tables[0].Columns.Remove("DASH Work Code");
                //Depending if Used
                //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
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
        if (!dtpOTDateFrom.IsNull)
        {
            criteria += lblOTDateFrom.Text + ":" + dtpOTDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpOTDateTo.IsNull)
        {
            criteria += lblOTDateTo.Text + ":" + dtpOTDateTo.Date.ToString("MM/dd/yyyy") + "-";
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
