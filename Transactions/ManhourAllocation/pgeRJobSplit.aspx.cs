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
using System.Text;
using Payroll.DAL;
using System.Collections.Specialized;

public partial class _Default : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    DataSet dsView;
    ListDictionary Totals;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFJOBSPLTREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            chkJobSplitMod.Attributes.Add("oncLick", "return CheckPopUp();");
            InitializeCheckers();
            if (!Page.IsPostBack)
            {
                DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFJOBSPLTREP");
                if (dr != null)
                {
                    bool canCheck = Convert.ToBoolean(dr["Ugt_CanCheck"]);
                    bool canApprove = Convert.ToBoolean(dr["Ugt_CanApprove"]);
                    
                    txtEmpName.Text = Session["userLogged"].ToString();
                    txtEmpName.Visible = canCheck;
                    Label3.Visible = canCheck;
                    btnEmployee.Visible = canCheck;
                    hiddenEmpFlag.Value = canCheck.ToString();
                    BtnExport.Visible = Convert.ToBoolean(dr["Ugt_CanGenerate"]);
                    btnPrint.Visible = Convert.ToBoolean(dr["Ugt_CanPrint"]);
                }
                //btnClear_Click(null, null);
                InitializeButtons();
                initializeControls();
                ddlType.Items.Clear();
                ddlType.Items.Add(new ListItem("", ""));
                ddlType.Items.Add(new ListItem("NEW", "N"));
                ddlType.Items.Add(new ListItem("CHANGE", "C"));
            }

            LoadComplete += new EventHandler(_Default_LoadComplete);
        }
    }

    protected void _Default_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "manhourscripts";
        string jsurl = "_manhour.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;

        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        btnCostCenter.OnClientClick = string.Format("javascript:return lookupRJSCostcenter()");
        btnEmployee.OnClientClick = string.Format("javascript:return lookupRJSEmployee()");
        btnStatus.OnClientClick = string.Format("javascript:return lookupRJSStatus()");
    }

    #region Button Events
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                Control[] ctrl = new Control[2];
                ctrl[0] = CommonLookUp.HeaderPanelOptionERP(grdView.Rows[0].Cells.Count, "Manhour Allocation Report", initializeHeader(), grdView.Width);
                ctrl[1] = grdView;
                Session["ctrl"] = ctrl;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
            }
            catch
            {
                MessageBox.Show("Some error occurred during initialization of page for printing.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }
    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                Control[] ctrl = new Control[2];
                ctrl[0] = CommonLookUp.HeaderPanelOptionERP(grdView.Rows[0].Cells.Count, "Manhour Allocation Report", initializeHeader());
                ctrl[1] = grdView;
                
                ExportExcelHelper.ExportControl2(ctrl, "Manhour Allocation Report");
            }
            catch
            {
                MessageBox.Show("Some error occurred during exporting to Excel.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (txtEmpName.Visible)
            txtEmpName.Text = "";
        txtStatus.Text = "";
        txtCostCenter.Text = "";

        cbLossTime.Checked = false;
        chkJobSplitMod.Checked = false;
        chkSubWorkCode.Checked = false;

        valueReturn.Value = null;
        ddlType.SelectedValue = "";

        chkAdjustment.Checked = false;
        txtperiodcycle.Text = "";

        txtChecker1.Text = "";
        txtChecker2.Text = "";
        txtApprover.Text = "";

        dtpJobSplitDateFrom.Reset();
        dtpJobSplitDateTo.Reset();
        dtpApprovedDate1From.Reset();
        dtpApprovedDate1To.Reset();
        dtpCheckedDate1From.Reset();
        dtpCheckedDate1To.Reset();
        dtpCheckedDate2From.Reset();
        dtpCheckedDate2To.Reset();
        dtpDateAppliedFrom.Reset();
        dtpDateAppliedTo.Reset();
        dtpDateEndorsedFrom.Reset();
        dtpDateEndorsedTo.Reset();
        grdView.DataSource = null;
        grdView.DataBind();

        Panel3.Attributes["style"] = "display:none;";
        Panel4.Attributes["style"] = "display:none;";
        Panel5.Attributes["style"] = "display:none;";
    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        dsView = GetData(SqlBuilder());
        LoadGridView();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFJOBSPLTREP", Session["userLogged"].ToString(), true, Session["userLogged"].ToString());

    }
    #endregion

    #region Other Methods
    protected DataSet GetData(string sqlQuery)
    {
        DataSet dsTemp = null;
        string sqlFetch = sqlQuery;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                CommonMethods.ErrorsToTextFile(new Exception(), "generate log:" + Session["userLogged"].ToString() + DateTime.Now.ToString());
                dsTemp = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
                CommonMethods.ErrorsToTextFile(new Exception(), "generate log:" + Session["userLogged"].ToString() + DateTime.Now.ToString());
            }
            catch
            {
                dsView = null;
            }
            finally
            {
                dal.CloseDB();
            }
        }
        return dsTemp;
    }

    private string SqlBuilder()
    {
        StringBuilder sql = new StringBuilder();
        StringBuilder sql2 = new StringBuilder();
        StringBuilder filters = new StringBuilder();

        string tableDetail = string.Empty;
        string tableHeader = string.Empty;

        if (this.cbLossTime.Checked)
        {
            tableDetail = "T_JobSplitDetailLossTime";
            tableHeader = "T_JObSplitHeaderLossTime";
        }
        else
        {
            tableDetail = "T_JobSplitDetail";
            tableHeader = "T_JObSplitHeader";
        }

        string pivotCols = string.Empty;
        if (chkSubWorkCode.Checked)
            pivotCols = @"SET @PivotCol  = 'Convert(VarChar(50),ISNULL(Slm_ClientJobName,'''')) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'') + Convert(Char(10),ISNULL(Jsd_ClientJobNo,'''')) + Convert(char(6),''<br />'') + Convert(VarChar(15),Jsd_SubWorkCode)'";
        else
            pivotCols = @"SET @PivotCol  = 'Convert(VarChar(50),ISNULL(Slm_ClientJobName,'''')) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'') + Convert(Char(10),ISNULL(Jsd_ClientJobNo,''''))'";
        string groupBy = string.Empty;
        if (chkSubWorkCode.Checked)
            groupBy = @"GROUP BY Ell_EmployeeId, Q.Emt_FirstName, Q.Emt_MiddleName, Q.Emt_LastName, Jsd_JobCode, Jsd_ClientJobNo, Slm_DashWorkCode, Slm_ClientJobName, Ell_ProcessDate, Ell_DayCode
          , CASE WHEN Ell_ActualTimeIn_1 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeIn_2 = ''0000'' THEN ''''
						ELSE LEFT(Ell_ActualTimeIn_2,2) + '':'' + RIGHT(Ell_ActualTimeIn_2,2)
					END 
                ELSE LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1,2)
            END
          ,CASE WHEN Ell_ActualTimeOut_2 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeOut_1 = ''0000'' THEN ''''
						ELSE LEFT(Ell_ActualTimeOut_1,2) + '':'' + RIGHT(Ell_ActualTimeOut_1,2)
					END 
                ELSE LEFT(Ell_ActualTimeOut_2,2) + '':'' + RIGHT(Ell_ActualTimeOut_2,2)
            END, Jsd_SubWorkCode";
        else
            groupBy = @"GROUP BY Ell_EmployeeId, Q.Emt_FirstName, Q.Emt_MiddleName, Q.Emt_LastName, Jsd_JobCode, Jsd_ClientJobNo, Slm_DashWorkCode, Slm_ClientJobName, Ell_ProcessDate, Ell_DayCode
          , CASE WHEN Ell_ActualTimeIn_1 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeIn_2 = ''0000'' THEN ''''
						 ELSE LEFT(Ell_ActualTimeIn_2,2) + '':'' + RIGHT(Ell_ActualTimeIn_2,2)
					END 
                 ELSE LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1,2)
            END
          ,CASE WHEN Ell_ActualTimeOut_2 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeOut_1 = ''0000'' THEN ''''
						 ELSE LEFT(Ell_ActualTimeOut_1,2) + '':'' + RIGHT(Ell_ActualTimeOut_1,2)
					END 
                 ELSE LEFT(Ell_ActualTimeOut_2,2) + '':'' + RIGHT(Ell_ActualTimeOut_2,2)
            END";

        sql.Append(@"
                    declare @SelectList varchar(max)
                    declare @PivotCol varchar(600)
                    declare @Summaries varchar(600)

                    SET @SelectList = '
                    SELECT2 
                          Ell_EmployeeId
                        , LEFT(Q.Emt_FirstName,1) + LEFT(Q.Emt_MiddleName,1) + Q.Emt_LastName as [Employee]
                        , CONVERT(varchar(20), Ell_ProcessDate, 101) as [ALLOCATION DATE]
                        , UPPER(LEFT(DATENAME(dw, Ell_ProcessDate), 3)) [DoW]
                        , SUM(Jsd_PlanHours) as [Jsd_PlanHours]
                    FROM T_EmployeeLogLedger
                    LEFT JOIN " + tableHeader + @" 
                        ON  Ell_ProcessDate = Jsh_JobSplitDate
                        AND Ell_EmployeeId = Jsh_EmployeeId
                        AND (	
                                (   Jsh_Status = ''9'')
                            OR  (   LEFT(Jsh_ControlNo, 1) = ''S'' 
                                    AND Jsh_Entry = ''N'' 
                                    AND Jsh_Status IN (''1'',''3'',''5'',''7'') 
                                    AND Jsh_ControlNo NOT IN (select SUB.Jsh_ControlNo from T_JobSplitHeader SUB where SUB.Jsh_JobSplitDate = Jsh_JobSplitDate
														and SUB.Jsh_EmployeeId = Jsh_EmployeeId and SUB.Jsh_Status = ''9''))
                            OR  (   Jsh_Entry = ''C'' 
                                    AND Jsh_Status IN (''1'',''3'',''5'',''7'')
                                    AND Jsh_RefControlNo NOT IN (SELECT Jsh_ControlNo FROM " + tableHeader + @" WHERE Jsh_Status = ''9''))
                            OR  (   LEFT(Jsh_ControlNo, 1) = ''J'' 
                                    AND Jsh_Status = ''1'' 
                                    AND Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) NOT IN (SELECT Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) 
                                                                                                FROM " + tableHeader + @" WHERE LEFT(Jsh_ControlNo,1) = ''S'' AND 
                                                                                                                 Jsh_Status IN (''1'',''3'',''5'',''7'',''9''))))    
                    LEFT JOIN " + tableDetail + @"
                        ON Jsh_ControlNo = Jsd_ControlNo
                    LEFT JOIN T_PayPeriodMaster 
                        ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN T_SalesMaster 
                        ON Jsd_JobCode = Slm_DashJobCode
	                    AND Jsd_ClientJobNo = Slm_ClientJobNo");
        if (!cbLossTime.Checked)
        {
            sql.Append(@" 
                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)");
        }
        sql.Append(@" 
                    LEFT JOIN T_AccountDetail 
                        ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                    LEFT JOIN T_LeaveTypeMaster 
                        ON T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    LEFT JOIN T_LeaveTypeMaster T_LeaveTypeMaster2 
                        ON T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 
                        ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 
                        ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr 
                        ON apr.Emt_EmployeeId = Jsh_ApprovedBy
				    LEFT JOIN T_EmployeeMaster Q 
                        ON Q.Emt_EmployeeId = Ell_EmployeeId
                    LEFT JOIN T_CostCenter 
                        ON Cct_CostCenterCode = Q.Emt_CostCenterCode

                     WHERE 1 = 1
                        {0}
                    " + groupBy + @"
                    UNION

                    SELECT2 
                          Ell_EmployeeId
                        , LEFT(Q.Emt_FirstName,1) + LEFT(Q.Emt_MiddleName,1) + Q.Emt_LastName as [Employee]
                        , CONVERT(varchar(20), Ell_ProcessDate, 101) as [ALLOCATION DATE]
                        , UPPER(LEFT(DATENAME(dw, Ell_ProcessDate), 3)) [DoW]
                        , SUM(Jsd_PlanHours) as [Jsd_PlanHours]
                    FROM T_EmployeeLogLedgerHist
                    LEFT JOIN " + tableHeader + @" 
                        ON  Ell_ProcessDate = Jsh_JobSplitDate
                        AND Ell_EmployeeId = Jsh_EmployeeId
                        AND (	
                                (   Jsh_Status = ''9'')
                            OR  (   LEFT(Jsh_ControlNo, 1) = ''S'' 
                                    AND Jsh_Entry = ''N'' 
                                    AND Jsh_Status IN (''1'',''3'',''5'',''7'') 
                                    AND Jsh_ControlNo NOT IN (select SUB.Jsh_ControlNo from T_JobSplitHeader SUB where SUB.Jsh_JobSplitDate = Jsh_JobSplitDate
														and SUB.Jsh_EmployeeId = Jsh_EmployeeId and SUB.Jsh_Status = ''9''))
                            OR  (   Jsh_Entry = ''C'' 
                                    AND Jsh_Status IN (''1'',''3'',''5'',''7'')
                                    AND Jsh_RefControlNo NOT IN (SELECT Jsh_ControlNo FROM " + tableHeader + @" WHERE Jsh_Status = ''9''))
                            OR  (   LEFT(Jsh_ControlNo, 1) = ''J'' 
                                    AND Jsh_Status = ''1'' 
                                    AND Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) NOT IN (SELECT Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) 
                                                                                                FROM " + tableHeader + @" WHERE LEFT(Jsh_ControlNo,1) = ''S'' AND 
                                                                                                                 Jsh_Status IN (''1'',''3'',''5'',''7'',''9''))))    
                    LEFT JOIN " + tableDetail + @"
                        ON Jsh_ControlNo = Jsd_ControlNo
                    LEFT JOIN T_PayPeriodMaster 
                        ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN T_SalesMaster 
                        ON Jsd_JobCode = Slm_DashJobCode
	                    AND Jsd_ClientJobNo = Slm_ClientJobNo");
        if (!cbLossTime.Checked)
        {
            sql.Append(@" 
                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)");
        }
        sql.Append(@" 
                    LEFT JOIN T_AccountDetail 
                        ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                    LEFT JOIN T_LeaveTypeMaster 
                        ON T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    LEFT JOIN T_LeaveTypeMaster T_LeaveTypeMaster2 
                        ON T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 
                        ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 
                        ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr 
                        ON apr.Emt_EmployeeId = Jsh_ApprovedBy
				    LEFT JOIN T_EmployeeMaster Q 
                        ON Q.Emt_EmployeeId = Ell_EmployeeId
                    LEFT JOIN T_CostCenter 
                        ON Cct_CostCenterCode = Q.Emt_CostCenterCode

                    WHERE 1 = 1
                        {0}

                    " + groupBy + @"'

                    " + pivotCols + @"

                    SET @Summaries = 'SUM(Jsd_PlanHours)'

                    EXECUTE dbo.dynamic_pivot2 @SelectList,@PivotCol,@Summaries");

        #region Query 2
        if (chkSubWorkCode.Checked)
            groupBy = @"GROUP BY Jsh_EmployeeId, Q.Emt_FirstName, Q.Emt_MiddleName, Q.Emt_LastName, Ell_ProcessDate, Ell_DayCode, Adt_AccountDesc
          , CASE WHEN Ell_ActualTimeIn_1 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeIn_2 = ''0000'' THEN ''''
						ELSE LEFT(Ell_ActualTimeIn_2,2) + '':'' + RIGHT(Ell_ActualTimeIn_2,2)
					END 
                ELSE LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1,2)
            END
          ,CASE WHEN Ell_ActualTimeOut_2 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeOut_1 = ''0000'' THEN ''''
						ELSE LEFT(Ell_ActualTimeOut_1,2) + '':'' + RIGHT(Ell_ActualTimeOut_1,2)
					END 
                ELSE LEFT(Ell_ActualTimeOut_2,2) + '':'' + RIGHT(Ell_ActualTimeOut_2,2)
            END, Jsd_SubWorkCode";
        else
            groupBy = @"GROUP BY Jsh_EmployeeId, Q.Emt_FirstName, Q.Emt_MiddleName, Q.Emt_LastName, Ell_ProcessDate, Ell_DayCode, Adt_AccountDesc
          , CASE WHEN Ell_ActualTimeIn_1 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeIn_2 = ''0000'' THEN ''''
						 ELSE LEFT(Ell_ActualTimeIn_2,2) + '':'' + RIGHT(Ell_ActualTimeIn_2,2)
					END 
                 ELSE LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1,2)
            END
          ,CASE WHEN Ell_ActualTimeOut_2 = ''0000'' THEN 
					CASE WHEN Ell_ActualTimeOut_1 = ''0000'' THEN ''''
						 ELSE LEFT(Ell_ActualTimeOut_1,2) + '':'' + RIGHT(Ell_ActualTimeOut_1,2)
					END 
                 ELSE LEFT(Ell_ActualTimeOut_2,2) + '':'' + RIGHT(Ell_ActualTimeOut_2,2)
            END";

        string groupBy2 = groupBy.Replace("''", "'");
        if(chkSubWorkCode.Checked)
            groupBy2 = groupBy2.Remove(groupBy2.LastIndexOf(", Jsd_SubWorkCode"));

        sql2.Append(@"
                        SELECT
                          LEFT(Q.Emt_FirstName,1) + LEFT(Q.Emt_MiddleName,1) + Q.Emt_LastName as [Employee]
                        , CONVERT(varchar(20), Ell_ProcessDate, 101) as [ALLOCATION DATE]
                        , ISNULL(Adt_AccountDesc, 'N/A') [Status]
                        , UPPER(LEFT(DATENAME(dw, Ell_ProcessDate), 3)) [DoW]
                        , Ell_DayCode [Day Code]
                        , CASE 
                              WHEN Ell_ActualTimeIn_1 = '0000' THEN 
					              CASE 
                                      WHEN Ell_ActualTimeIn_2 = '0000' THEN ''
						              ELSE LEFT(Ell_ActualTimeIn_2,2) + ':' + RIGHT(Ell_ActualTimeIn_2,2)
					              END 
                              ELSE LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1,2)
                          END [IN]
                        , CASE 
				              WHEN Ell_ActualTimeOut_2 = '0000' THEN 
					              CASE 
						              WHEN Ell_ActualTimeOut_1 = '0000' THEN ''
						              ELSE LEFT(Ell_ActualTimeOut_1,2) + ':' + RIGHT(Ell_ActualTimeOut_1,2)
					              END 
                              ELSE LEFT(Ell_ActualTimeOut_2,2) + ':' + RIGHT(Ell_ActualTimeOut_2,2)
                          END [OUT]");
        if(!cbLossTime.Checked)
        {
            sql2.Append(@"
                        , SUM(CASE WHEN Jsd_Overtime = 0
								THEN Jsd_PlanHours
								ELSE 0
							END) [Reg]
						, SUM(CASE WHEN Jsd_Overtime = 1
								THEN Jsd_PlanHours
								ELSE 0
							END) [OT]
						, SUM(ISNULL(Jsd_PlanHours,0)) as [Total]
                        , SUM(ISNULL(Jsd_ActHours,0)) as [Actual]");
        }
        sql2.Append(@"
                    FROM T_EmployeeLogLedger
                    LEFT JOIN " + tableHeader + @" 
                        ON  Ell_ProcessDate = Jsh_JobSplitDate
                        AND Ell_EmployeeId = Jsh_EmployeeId
                        AND (	
                                (   Jsh_Status = '9')
                            OR  (   LEFT(Jsh_ControlNo, 1) = 'S' 
                                    AND Jsh_Entry = 'N' 
                                    AND Jsh_Status IN ('1','3','5','7') 
                                    AND Jsh_ControlNo NOT IN (select SUB.Jsh_ControlNo from T_JobSplitHeader SUB where SUB.Jsh_JobSplitDate = Jsh_JobSplitDate
														and SUB.Jsh_EmployeeId = Jsh_EmployeeId and SUB.Jsh_Status = '9'))
                            OR  (   Jsh_Entry = 'C' 
                                    AND Jsh_Status IN ('1','3','5','7')
                                    AND Jsh_RefControlNo NOT IN (SELECT Jsh_ControlNo FROM " + tableHeader + @" WHERE Jsh_Status = '9'))
                            OR  (   LEFT(Jsh_ControlNo, 1) = 'J' 
                                    AND Jsh_Status = '1' 
                                    AND Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) NOT IN (SELECT Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) 
                                                                                                FROM " + tableHeader + @" WHERE LEFT(Jsh_ControlNo,1) = 'S' AND
                                                                                                                           Jsh_Status IN ('1','3','5','7','9'))))    
                    LEFT JOIN " + tableDetail + @"
                        ON Jsh_ControlNo = Jsd_ControlNo
                    LEFT JOIN T_PayPeriodMaster 
                        ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN T_SalesMaster 
                        ON Jsd_JobCode = Slm_DashJobCode
	                    AND Jsd_ClientJobNo = Slm_ClientJobNo");
        if(!cbLossTime.Checked)
        {
            sql2.Append(@"
                        AND (Jsd_CostCenter = Slm_CostCenter OR 'ALL' = Slm_CostCenter)");
        }
        sql2.Append(@"
                    LEFT JOIN T_AccountDetail 
                        ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  'WFSTATUS'
                    LEFT JOIN T_LeaveTypeMaster 
                        ON T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    LEFT JOIN T_LeaveTypeMaster T_LeaveTypeMaster2 
                        ON T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 
                        ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 
                        ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr 
                        ON apr.Emt_EmployeeId = Jsh_ApprovedBy
				    LEFT JOIN T_EmployeeMaster Q 
                        ON Q.Emt_EmployeeId = Ell_EmployeeId
                    LEFT JOIN T_CostCenter 
                        ON Cct_CostCenterCode = Q.Emt_CostCenterCode

                     WHERE 1 = 1
                        {0}
                    " + groupBy2 + @"
                    
                    UNION

                    SELECT 
                          LEFT(Q.Emt_FirstName,1) + LEFT(Q.Emt_MiddleName,1) + Q.Emt_LastName as [Employee]
                        , CONVERT(varchar(20), Ell_ProcessDate, 101) as [ALLOCATION DATE]
                        , ISNULL(Adt_AccountDesc, 'N/A') [Status]
                        , UPPER(LEFT(DATENAME(dw, Ell_ProcessDate), 3)) [DoW]
                        , Ell_DayCode [Day Code]
                        , CASE 
                              WHEN Ell_ActualTimeIn_1 = '0000' THEN 
					              CASE 
                                      WHEN Ell_ActualTimeIn_2 = '0000' THEN ''
						              ELSE LEFT(Ell_ActualTimeIn_2,2) + ':' + RIGHT(Ell_ActualTimeIn_2,2)
					              END 
                              ELSE LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1,2)
                          END [IN]
                        , CASE 
				              WHEN Ell_ActualTimeOut_2 = '0000' THEN 
					              CASE 
						              WHEN Ell_ActualTimeOut_1 = '0000' THEN ''
						              ELSE LEFT(Ell_ActualTimeOut_1,2) + ':' + RIGHT(Ell_ActualTimeOut_1,2)
					              END 
                              ELSE LEFT(Ell_ActualTimeOut_2,2) + ':' + RIGHT(Ell_ActualTimeOut_2,2)
                          END [OUT]");
        if(!cbLossTime.Checked)
        {
            sql2.Append(@"
                        , SUM(CASE WHEN Jsd_Overtime = 0
								THEN Jsd_PlanHours
								ELSE 0
							END) [Reg]
						, SUM(CASE WHEN Jsd_Overtime = 1
								THEN Jsd_PlanHours
								ELSE 0
							END) [OT]
						, SUM(ISNULL(Jsd_PlanHours,0)) as [Total]
                        , SUM(ISNULL(Jsd_ActHours,0)) as [Actual]");
        }
        sql2.Append(@"
                    FROM T_EmployeeLogLedgerHist
                    LEFT JOIN " + tableHeader + @" 
                        ON  Ell_ProcessDate = Jsh_JobSplitDate
                        AND Ell_EmployeeId = Jsh_EmployeeId
                        AND (	
                                (   Jsh_Status = '9')
                            OR  (   LEFT(Jsh_ControlNo, 1) = 'S' 
                                    AND Jsh_Entry = 'N' 
                                    AND Jsh_Status IN ('1','3','5','7') 
                                    AND Jsh_ControlNo NOT IN (select SUB.Jsh_ControlNo from T_JobSplitHeader SUB where SUB.Jsh_JobSplitDate = Jsh_JobSplitDate
														and SUB.Jsh_EmployeeId = Jsh_EmployeeId and SUB.Jsh_Status = '9'))
                            OR  (   Jsh_Entry = 'C' 
                                    AND Jsh_Status IN ('1','3','5','7')
                                    AND Jsh_RefControlNo NOT IN (SELECT Jsh_ControlNo FROM " + tableHeader + @" WHERE Jsh_Status = '9'))
                            OR  (   LEFT(Jsh_ControlNo, 1) = 'J' 
                                    AND Jsh_Status = '1' 
                                    AND Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) NOT IN (SELECT Jsh_EmployeeId+CAST(Jsh_JobSplitDate AS VARCHAR(10)) 
                                                                                                FROM " + tableHeader + @" WHERE LEFT(Jsh_ControlNo,1) = 'S' AND 
                                                                                                                           Jsh_Status IN ('1','3','5','7','9'))))    
                    LEFT JOIN " + tableDetail + @"
                        ON Jsh_ControlNo = Jsd_ControlNo
                    LEFT JOIN T_PayPeriodMaster 
                        ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN T_SalesMaster 
                        ON Jsd_JobCode = Slm_DashJobCode
	                    AND Jsd_ClientJobNo = Slm_ClientJobNo");
        if (!cbLossTime.Checked)
        {
            sql2.Append(@"
                        AND (Jsd_CostCenter = Slm_CostCenter OR 'ALL' = Slm_CostCenter)");
        }
        sql2.Append(@"
                    LEFT JOIN T_AccountDetail 
                        ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  'WFSTATUS'
                    LEFT JOIN T_LeaveTypeMaster 
                        ON T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    LEFT JOIN T_LeaveTypeMaster T_LeaveTypeMaster2 
                        ON T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 
                        ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 
                        ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr 
                        ON apr.Emt_EmployeeId = Jsh_ApprovedBy
				    LEFT JOIN T_EmployeeMaster Q 
                        ON Q.Emt_EmployeeId = Ell_EmployeeId
                    LEFT JOIN T_CostCenter 
                        ON Cct_CostCenterCode = Q.Emt_CostCenterCode

                    WHERE 1 = 1
                        {0}
                    " + groupBy2
                      + " ORDER BY Employee, [ALLOCATION DATE] ");
        #endregion

        #region Filters
        #region Status
        if (txtStatus.Text != "")
        {
            if (valueReturn.Value.ToString() != "")
            {
                string acctCode = valueReturn.Value.ToString().Replace("'", "").Replace(" ", "");
                ArrayList arr1 = CommonLookUp.DivideString(acctCode);
                if (arr1.Count > 0)
                {
                    filters.Append("\nAND aaaaaaaa in (");
                    string code = "";
                    for (int j = 0; j < arr1.Count; j++)
                    {
                        code += string.Format("''{0}'',", arr1[j].ToString().Trim());
                    }
                    filters.Append(code.Substring(0, code.Length - 1));
                    filters.Append(")");
                }
            }
        }
        #endregion
        #region Cost Center Access
        if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
        {
            if (hiddenEmpFlag.Value.ToUpper().Equals("TRUE"))
            {
                string temp = hfUserCostCenters.Value;
                temp = temp.Replace("x", "''").Replace("y", ",");
                filters.Append("\nAND Q.Emt_CostCenterCode in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            }
        }

        if (!txtCostCenter.Text.Equals(string.Empty))
        {
            ArrayList arr = CommonLookUp.DivideString(txtCostCenter.Text.Replace("'", "").Replace(" ", ""));
            if (arr.Count > 0)
            {
                string costcenter = string.Empty;
                for (int i = 0; i < arr.Count; i++)
                {
                    costcenter += string.Format("''{0}'',", arr[i].ToString());
                }
                //if (!cbLossTime.Checked)
                //    filters.Append("\nAND Slm_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", costcenter.Substring(0, costcenter.Length - 1)));
                //else
                filters.Append("\nAND Q.Emt_CostCenterCode in (@costCenterAcc)".Replace("@costCenterAcc", costcenter.Substring(0, costcenter.Length - 1)));
            }
        }
        #endregion
        #region Job Split
        if (txtEmpName.Text != string.Empty)
        {
            string text = txtEmpName.Text.Replace("'", "");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string id = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    if (CommonLookUp.isNumeric(arr[i].ToString(), System.Globalization.NumberStyles.Integer))
                        id += string.Format("{0},", arr[i].ToString());
                }
                filters.Append("\nAND (");
                if (id != "")
                    filters.Append(string.Format("Ell_EmployeeId in (''{0}'')", id.Substring(0, id.Length - 1)));
                else
                    filters.Append("\n 1 = 0");
                filters.Append( ")");
            }
        }
        //if (ddlStatus.SelectedValue != string.Empty)
        //    filters.Append(string.Format("\nAND Jsh_Status = ''{0}''", ddlStatus.SelectedValue.ToString()));
        if (txtStatus.Text != "")
        {
            ArrayList arr = CommonLookUp.DivideString(txtStatus.Text.Replace("'", ""));
            if (arr.Count > 0)
            {
                string Desc = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    Desc += string.Format("\nOR Adt_AccountCode like ''{0}%''", arr[i].ToString().Trim());
                }
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(Desc + ")");
            }

            if (valueReturn.Value.ToString() != "")
            {
                string acctCode = valueReturn.Value.ToString().Replace("'", "");
                ArrayList arr1 = CommonLookUp.DivideString(acctCode);
                if (arr.Count > 0)
                {
                    filters.Append("\nAND Adt_AccountCode in (");
                    string code = "";
                    for (int j = 0; j < arr1.Count; j++)
                    {
                        code += string.Format("''{0}'',", arr1[j].ToString().Trim());
                    }
                    filters.Append(code.Substring(0, code.Length - 1));
                    filters.Append(")");
                }
            }
        }

        if (ddlType.SelectedValue != string.Empty)
            filters.Append(string.Format("\nAND Jsh_Entry = ''{0}''", ddlType.SelectedValue.ToString()));
        #endregion
        #region Checker and Approver
        if (txtperiodcycle.Text != "")
            filters.Append(string.Format("\nAND Convert(varchar(20),Ppm_StartCycle,101) + '' - '' + Convert(varchar(20),Ppm_EndCycle,101) like ''{0}''", txtperiodcycle.Text.Replace("''", " ")));
        if (chkJobSplitMod.Checked)
        {
            if (txtChecker1.Text != "")
            {
                ArrayList arr = CommonLookUp.DivideString(txtChecker1.Text.Replace("'", ""));
                if (arr.Count > 0)
                {
                    string id = "", idCode = "", lastName = "", firstName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        if (CommonLookUp.isNumeric(arr[i].ToString(), System.Globalization.NumberStyles.Integer))
                            id += string.Format("{0},", arr[i].ToString());
                        idCode += string.Format("\nOR C1.Emt_Nickname like ''{0}''", arr[i].ToString().Trim());
                        lastName += string.Format("\nOR C1.Emt_LastName like ''{0}%''", arr[i].ToString().Trim());
                        firstName += string.Format("\nOR C1.Emt_FirstName like ''{0}%''", arr[i].ToString().Trim());
                    }
                    filters.Append("\nAND (");
                    if (id != "")
                        filters.Append(string.Format("Jsh_CheckedBy in ({0})", id.Substring(0, id.Length - 1)));
                    else
                        filters.Append("\n 1 = 0");
                    filters.Append(idCode + ")");
                }
            }

            if (txtChecker2.Text != "")
            {
                ArrayList arr = CommonLookUp.DivideString(txtChecker2.Text.Replace("'", ""));
                if (arr.Count > 0)
                {
                    string id = "", idCode = "", lastName = "", firstName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        if (CommonLookUp.isNumeric(arr[i].ToString(), System.Globalization.NumberStyles.Integer))
                            id += string.Format("{0},", arr[i].ToString());
                        idCode += string.Format("\nOR C2.Emt_Nickname like ''{0}%''", arr[i].ToString().Trim());
                        lastName += string.Format("\nOR C2.Emt_LastName like ''{0}%''", arr[i].ToString().Trim());
                        firstName += string.Format("\nOR C2.Emt_FirstName like ''{0}%''", arr[i].ToString().Trim());
                    }
                    filters.Append("\nAND (");
                    if (id != "")
                        filters.Append(string.Format("Jsh_Checked2By in ({0})", id.Substring(0, id.Length - 1)));
                    else
                        filters.Append("\n 1 = 0");
                    filters.Append(idCode);
                    filters.Append(lastName);
                    filters.Append(firstName + ")");
                }
            }

            if (txtApprover.Text != "")
            {
                ArrayList arr = CommonLookUp.DivideString(txtApprover.Text.Replace("'", ""));
                if (arr.Count > 0)
                {
                    string id = "", idCode = "", lastName = "", firstName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        if (CommonLookUp.isNumeric(arr[i].ToString(), System.Globalization.NumberStyles.Integer))
                            id += string.Format("{0},", arr[i].ToString());
                        idCode += string.Format("\nOR apr.Emt_Nickname like ''{0}%''", arr[i].ToString().Trim());
                        lastName += string.Format("\nOR apr.Emt_LastName like ''{0}%''", arr[i].ToString().Trim());
                        firstName += string.Format("\nOR apr.Emt_FirstName like ''{0}%''", arr[i].ToString().Trim());
                    }
                    filters.Append("\nAND (");
                    if (id != "")
                        filters.Append(string.Format("Jsh_ApprovedBy in ({0})", id.Substring(0, id.Length - 1)));
                    else
                        filters.Append("\n 1 = 0");
                    filters.Append(idCode);
                    filters.Append(lastName);
                    filters.Append(firstName + ")");
                }
            }
        }
        #endregion
        #region Date Time Picker 
        //Job Split Date From & To
        if (!dtpJobSplitDateFrom.IsNull || !dtpJobSplitDateTo.IsNull)
        {
            if (!dtpJobSplitDateFrom.IsNull && !dtpJobSplitDateTo.IsNull)
            {
                filters.Append(string.Format(@"AND (Convert(DateTime, Ell_ProcessDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpJobSplitDateFrom.Date)
                                                                 , Convert.ToDateTime(dtpJobSplitDateTo.Date)));
            }
            else if (!dtpJobSplitDateFrom.IsNull && dtpJobSplitDateTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Ell_ProcessDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpJobSplitDateFrom.Date)));
            }
            else if (dtpJobSplitDateFrom.IsNull && !dtpJobSplitDateTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Ell_ProcessDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpJobSplitDateTo.Date)));
            }
        }
        if (chkJobSplitMod.Checked)
        {
            #region Common Date Time Picker
            //Date Applied From & To
            if (!dtpDateAppliedFrom.IsNull || !dtpDateAppliedTo.IsNull)
            {
                if (!dtpDateAppliedFrom.IsNull && !dtpDateAppliedTo.IsNull)
                {
                    if (dtpDateAppliedFrom.Date == dtpDateAppliedTo.Date)
                        filters.Append(string.Format(@"AND Convert(varchar(10), Jsh_AppliedDate, 101) =  Convert(varchar(10),convert(datetime,''{0:d}''),101)", Convert.ToDateTime(dtpDateAppliedFrom.Date)));
                    else
                        filters.Append(string.Format(@"AND Jsh_AppliedDate
                                 BETWEEN Convert(DATETIME,''{0}'') AND Convert(DATETIME,''{1}'') ", Convert.ToDateTime(dtpDateAppliedFrom.Date)
                                                                         , Convert.ToDateTime(dtpDateAppliedTo.Date)));
                }
                else if (!dtpDateAppliedFrom.IsNull && dtpDateAppliedTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_AppliedDate
                                 >= Convert(DATETIME,''{0}'') ", dtpDateAppliedFrom.Date));
                }
                else if (dtpDateAppliedFrom.IsNull && !dtpDateAppliedTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_AppliedDate
                                 <= Convert(DATETIME,''{0}'')", dtpDateAppliedTo.Date));
                }
            }

            //Endorse Date From & To
            if (!dtpDateEndorsedFrom.IsNull || !dtpDateEndorsedTo.IsNull)
            {
                if (!dtpDateEndorsedFrom.IsNull && !dtpDateEndorsedTo.IsNull)
                {
                    if (dtpDateEndorsedFrom.Date == dtpDateEndorsedTo.Date)
                        filters.Append(string.Format(@"AND Convert(varchar(10), Jsh_EndorsedDateToChecker, 101) =   Convert(varchar(10),convert(datetime,''{0:d}''),101)", Convert.ToDateTime(dtpDateEndorsedFrom.Date)));
                    else
                        filters.Append(string.Format(@"AND Jsh_EndorsedDateToChecker
                                 BETWEEN Convert(DATETIME,''{0}'') AND Convert(DATETIME,''{1}'') ", Convert.ToDateTime(dtpDateEndorsedFrom.Date)
                                                                         , Convert.ToDateTime(dtpDateEndorsedTo.Date)));
                }
                else if (!dtpDateEndorsedFrom.IsNull && dtpDateEndorsedTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_EndorsedDateToChecker
                                 >= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpDateEndorsedFrom.Date)));
                }
                else if (dtpDateEndorsedFrom.IsNull && !dtpDateEndorsedTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_EndorsedDateToChecker
                                 <= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpDateEndorsedTo.Date)));
                }
            }

            //Checker 1 Date From & To
            if (!dtpCheckedDate1From.IsNull || !dtpCheckedDate1To.IsNull)
            {
                if (!dtpCheckedDate1From.IsNull && !dtpCheckedDate1To.IsNull)
                {
                    if (dtpCheckedDate1From.Date == dtpCheckedDate1To.Date)
                        filters.Append(string.Format(@"AND Convert(varchar(10), Jsh_CheckedDate, 101) =   Convert(varchar(10),convert(datetime,''{0:d}''),101)", Convert.ToDateTime(dtpCheckedDate1From.Date)));
                    else
                        filters.Append(string.Format(@"AND Jsh_CheckedDate
                                 BETWEEN Convert(DATETIME,''{0}'') AND Convert(DATETIME,''{1}'')", Convert.ToDateTime(dtpCheckedDate1From.Date)
                                                                         , Convert.ToDateTime(dtpCheckedDate1To.Date)));
                }
                else if (!dtpCheckedDate1From.IsNull && dtpCheckedDate1To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_CheckedDate
                                 >= Convert(DATETIME,''{0}'') ", Convert.ToDateTime(dtpCheckedDate1From.Date)));
                }
                else if (dtpCheckedDate1From.IsNull && !dtpCheckedDate1To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_CheckedDate
                                 <= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpCheckedDate1To.Date)));
                }
            }
            //Checked 2 Date From & To
            if (!dtpCheckedDate2From.IsNull || !dtpCheckedDate2To.IsNull)
            {
                if (!dtpCheckedDate2From.IsNull && !dtpCheckedDate2To.IsNull)
                {
                    if (dtpCheckedDate2From.Date == dtpCheckedDate2To.Date)
                        filters.Append(string.Format(@"AND Convert(varchar(10), Jsh_Checked2Date, 101) =   Convert(varchar(10),convert(datetime,''{0:d}''),101)", Convert.ToDateTime(dtpCheckedDate2From.Date)));
                    else
                        filters.Append(string.Format(@"AND Jsh_Checked2Date
                                 BETWEEN Convert(DATETIME,''{0}'') AND Convert(DATETIME,''{1}'')", Convert.ToDateTime(dtpCheckedDate2From.Date)
                                                                         , Convert.ToDateTime(dtpCheckedDate2To.Date)));
                }
                else if (!dtpCheckedDate2From.IsNull && dtpCheckedDate2To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_Checked2Date
                                 >= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpCheckedDate2From.Date)));
                }
                else if (dtpCheckedDate2From.IsNull && !dtpCheckedDate2To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_Checked2Date
                                 <= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpCheckedDate2To.Date)));
                }
            }
            //Approve Date From & To
            if (!dtpApprovedDate1From.IsNull || !dtpApprovedDate1To.IsNull)
            {
                if (!dtpApprovedDate1From.IsNull && !dtpApprovedDate1To.IsNull)
                {
                    if (dtpApprovedDate1From.Date == dtpApprovedDate1To.Date)
                        filters.Append(string.Format(@"AND Convert(varchar(10), Jsh_ApprovedDate, 101) =   Convert(varchar(10),convert(datetime,''{0:d}''),101)", Convert.ToDateTime(dtpApprovedDate1From.Date)));
                    else
                        filters.Append(string.Format(@"AND Jsh_ApprovedDate
                                 BETWEEN Convert(DATETIME,''{0}'') AND Convert(DATETIME,''{1}'')", Convert.ToDateTime(dtpApprovedDate1From.Date)
                                                                         , Convert.ToDateTime(dtpApprovedDate1To.Date)));
                }
                else if (!dtpApprovedDate1From.IsNull && dtpApprovedDate1To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_ApprovedDate
                                 >= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpApprovedDate1From.Date)));
                }
                else if (dtpApprovedDate1From.IsNull && !dtpApprovedDate1To.IsNull)
                {
                    filters.Append(string.Format(@"AND Jsh_ApprovedDate
                                 <= Convert(DATETIME,''{0}'')", Convert.ToDateTime(dtpApprovedDate1To.Date)));
                }
            }
            #endregion
        }
        #endregion
        #endregion

        return string.Format(sql.Append(string.Format(sql2.ToString(), filters.ToString().Replace("''","'"))).ToString(), filters.ToString());
    }

    private void LoadGridView()
    {
        if (dsView != null && dsView.Tables.Count == 2 && dsView.Tables[0].Rows.Count == dsView.Tables[1].Rows.Count)
        {
            // sorts the 1st table of dsView object
            DataView dv = dsView.Tables[0].DefaultView;
            dv.Sort = " Employee ASC, [ALLOCATION DATE] ASC";
            DataTable dt = dv.ToTable();

            // add columns to the final table 
            for (int j = 4; j < dt.Columns.Count; j++)
            {
                dsView.Tables[1].Columns.Add(dt.Columns[j].Caption);
            }

            // add data to the new columns
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 4; j < dt.Columns.Count; j++)
                {
                    if (cbLossTime.Checked)
                        dsView.Tables[1].Rows[i][j + 4] = dt.Rows[i][j];
                    else
                        dsView.Tables[1].Rows[i][j + 7] = dt.Rows[i][j];
                }
            }
            Totals = new ListDictionary();
            DataTable dt2 = dsView.Tables[1].Copy();
            InitDataTable(dt2);
            VSDataTable = dt2;
            grdView.DataSource = VSDataTable;
            grdView.DataBind();
        }
        else
        {
            VSDataTable = null;
            grdView.DataSource = null;
            grdView.DataBind();
        }
        //grdView.Attributes["Width"] = Panel8.Width.ToString();
    }

    private DataTable VSDataTable
    {
        get
        {
            return (DataTable)ViewState["VSDataTable"];
        }
        set
        {
            ViewState["VSDataTable"] = value;
        }
    }
    #endregion

    #region Initializing Methods
    private void InitDataTable(DataTable dt)
    {
        try
        {
            if (!chkSubWorkCode.Checked)
                dt.Columns.Remove("Sub Work Code");
        }
        catch
        {
        }
    }

    private void initializeControls()
    {
        dtpJobSplitDateFrom.Date = CommonMethods.getQuincenaDate('C', "START");
        dtpJobSplitDateFrom.MinDate = CommonMethods.getMinimumDate();
        dtpJobSplitDateFrom.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        dtpJobSplitDateTo.Date = CommonMethods.getQuincenaDate('C', "END");
        dtpJobSplitDateTo.MinDate = CommonMethods.getMinimumDate();
        dtpJobSplitDateTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");
    }

    private void InitializeButtons()
    {
        hfUserCostCenters.Value = "";
        DataTable dt = CommonLookUp.GetUserCostCenterCode(Session["userId"].ToString(), "TIMEKEEP");
        if (dt != null)
        {
            foreach (DataRow dr in dt.Rows)
            {
                hfUserCostCenters.Value += string.Format("x{0}xy", dr[0].ToString());
            }

            btnEmployee.OnClientClick = string.Format("return OpenPopupLookupEmployee('T_JobSplitHeader','Jsh_EmployeeId','T_JobSplitHeader','Jsh_CostCenter','{0}','txtEmpName','Employee Lookup'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
            
            btnChecker1.OnClientClick = string.Format("return lookupRJSCheckerApprover('Jsh_CheckedBy','txtChecker1')");
            btnChecker2.OnClientClick = string.Format("return lookupRJSCheckerApprover('Jsh_Checked2By','txtChecker2')");
            btnApprover.OnClientClick = string.Format("return lookupRJSCheckerApprover('Jsh_ApprovedBy','txtApprover')");
            //btnApprover.OnClientClick = string.Format("return OpenPopupLookupEmployee('T_JobSplitHeader','Jsh_ApprovedBy','T_JobSplitHeader','Jsh_CostCenter','{0}','txtApprover','Approver Lookup'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
            //btnPayPeriod.OnClientClick = "return OpenPopupLookupPeriod('txtperiodcycle'); return true;";
            //status
            btnStatus.OnClientClick = "return OpenPopupLookupAccount2('WFSTATUS','txtStatus','valueReturn','Adt_AccountCode','Adt_AccountDesc','Adt_AccountType','T_AccountDetail','Status Lookup')";
            txtStatus.Attributes.Add("OnChange", "javascript:document.getElementById('ctl00_ContentPlaceHolder1_valueReturn').value = ''; return true;");
            btnCostCenter.OnClientClick = string.Format("return OpenPopupLookupCostCenter('T_JobSplitHeader','Jsh_CostCenter','T_JobSplitHeader','{0}','txtCostCenter','Cost Center Lookup'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
        }
    }

    private string initializeHeader()
    {
        string options = "";
        //if (hfUserCostCenters.Value != "")
        //    options += "Cost Center Access: " + hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1).Replace("x", "'").Replace("y", ",") + "; ";
        if (txtEmpName.Text != "")
            options += "Employee(s): " + txtEmpName.Text + "; ";
        if (txtStatus.Text != "")
            options += "Status: " + txtStatus.Text + "; ";
        if (ddlType.SelectedValue != "")
            options += "Type: " + ddlType.SelectedItem.Text.Trim() + "; ";
        if (txtperiodcycle.Text != "")
            options += "Pay Period: " + txtperiodcycle.Text.Trim() + "; ";

        if (!dtpJobSplitDateTo.IsNull && !dtpJobSplitDateFrom.IsNull)
            if (dtpJobSplitDateTo.Date == dtpJobSplitDateFrom.Date)
                options += "Manhour Date: " + dtpJobSplitDateFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Manhour Date: " + dtpJobSplitDateFrom.Date + " - " + dtpJobSplitDateTo.Date + "; ";
        else if (dtpJobSplitDateTo.IsNull && !dtpJobSplitDateFrom.IsNull)
            options += "Manhour Date: From " + dtpJobSplitDateFrom.Date + "; ";
        else if (!dtpJobSplitDateTo.IsNull && dtpJobSplitDateFrom.IsNull)
            options += "Manhour Date: To " + dtpJobSplitDateFrom.Date + "; ";
        #region Common Option
        //Date Applied
        if (!dtpDateAppliedTo.IsNull && !dtpDateAppliedFrom.IsNull)
            if (dtpDateAppliedTo.Date == dtpDateAppliedFrom.Date)
                options += "Date Applied: " + dtpDateAppliedFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Date Applied: " + dtpDateAppliedTo.Date + " - " + dtpDateAppliedFrom.Date + "; ";
        else if (dtpDateAppliedTo.IsNull && !dtpDateAppliedFrom.IsNull)
            options += "Date Applied: From " + dtpDateAppliedFrom.Date + "; ";
        else if (!dtpDateAppliedTo.IsNull && dtpDateAppliedFrom.IsNull)
            options += "Date Applied: To " + dtpDateAppliedTo.Date + "; ";
        //Endorsed Date
        if (!dtpDateEndorsedTo.IsNull && !dtpDateEndorsedFrom.IsNull)
            if (dtpDateEndorsedTo.Date == dtpDateEndorsedFrom.Date)
                options += "Endorsed Date to Checker 1: " + dtpDateEndorsedFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Endorsed Date to Checker 1: " + dtpDateEndorsedTo.Date + " - " + dtpDateEndorsedFrom.Date + "; ";
        else if (dtpDateEndorsedTo.IsNull && !dtpDateEndorsedFrom.IsNull)
            options += "Endorsed Date to Checker 1: From " + dtpDateEndorsedFrom.Date + "; ";
        else if (!dtpDateEndorsedTo.IsNull && dtpDateEndorsedFrom.IsNull)
            options += "Endorsed Date to Checker 1: To " + dtpDateEndorsedTo.Date + "; ";
        //Checker 1
        if (txtChecker1.Text != "")
            options += "Checker 1: " + txtChecker1.Text + "; ";
        if (!dtpCheckedDate1To.IsNull && !dtpCheckedDate1From.IsNull)
            if (dtpCheckedDate1To.Date == dtpCheckedDate1From.Date)
                options += "Checked Date 1: " + dtpCheckedDate1From.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Checked Date 1: " + dtpCheckedDate1To.Date + " - " + dtpCheckedDate1From.Date + "; ";
        else if (dtpCheckedDate1To.IsNull && !dtpCheckedDate1From.IsNull)
            options += "Checked Date 1: From " + dtpCheckedDate1From.Date + "; ";
        else if (!dtpCheckedDate1To.IsNull && dtpCheckedDate1From.IsNull)
            options += "Checked Date 1: To " + dtpCheckedDate1To.Date + "; ";
        //Checker 2
        if (txtChecker2.Text != "")
            options += "Checker 2: " + txtChecker2.Text + "; ";
        if (!dtpCheckedDate2To.IsNull && !dtpCheckedDate2From.IsNull)
            if (dtpCheckedDate2To.Date == dtpCheckedDate2From.Date)
                options += "Checked Date 2: " + dtpCheckedDate2From.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Checked Date 2: " + dtpCheckedDate2To.Date + " - " + dtpCheckedDate2From.Date + "; ";
        else if (dtpCheckedDate2To.IsNull && !dtpCheckedDate2From.IsNull)
            options += "Checked Date 2: From " + dtpCheckedDate2From.Date + "; ";
        else if (!dtpCheckedDate2To.IsNull && dtpCheckedDate2From.IsNull)
            options += "Checked Date 2: To " + dtpCheckedDate2To.Date + "; ";
        //Approver
        if (txtApprover.Text != "")
            options += "Approver: " + txtApprover.Text + "; ";
        if (!dtpApprovedDate1To.IsNull && !dtpApprovedDate1From.IsNull)
            if (dtpApprovedDate1To.Date == dtpApprovedDate1From.Date)
                options += "Approved Date: " + dtpApprovedDate1From.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Approved Date: " + dtpApprovedDate1To.Date + " - " + dtpApprovedDate1From.Date + "; ";
        else if (dtpApprovedDate1To.IsNull && !dtpApprovedDate1From.IsNull)
            options += "Approved Date: From " + dtpApprovedDate1From.Date + "; ";
        else if (!dtpApprovedDate1To.IsNull && dtpApprovedDate1From.IsNull)
            options += "Approved Date: To " + dtpApprovedDate1To.Date + "; ";
        #endregion
        return options.Trim();
    }

    private void InitializeCheckers()
    {
        if (chkJobSplitMod.Checked)
        {
            Panel3.Attributes["style"] = "display:inline;";
            Panel4.Attributes["style"] = "display:inline;";
            Panel5.Attributes["style"] = "display:inline;";
        }
        else
        {
            Panel3.Attributes["style"] = "display:none;";
            Panel4.Attributes["style"] = "display:none;";
            Panel5.Attributes["style"] = "display:none;";
        }
    }
    #endregion

    #region GridView Events
    protected void grdView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        int j = 7;
        if (!chkSubWorkCode.Checked)
            j = 6;
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = j; i < e.Row.Cells.Count; i++)
            {
                TableCell tc = e.Row.Cells[i];
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.VerticalAlign = VerticalAlign.Top;
                //tc.Text = tc.Text.Insert(25, "mon");
            }
        }
    }
    protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int j = 7;
        if (e.Row.RowType == DataControlRowType.Header)
        {
            //Andre: commented out this lines below to display only the dates not including DoW or Day Code
            
            //for (int i = j; i < e.Row.Cells.Count; i++)
            //{
            //    TableCell tc = e.Row.Cells[i];
            //    tc.VerticalAlign = VerticalAlign.Top;
            //    int temp = int.Parse(tc.Text.Substring(26, 3));
            //    string day = "   ";
            //    switch (temp)
            //    {
            //        case 1: day = "Sun"; break;
            //        case 2: day = "Mon"; break;
            //        case 3: day = "Tue"; break;
            //        case 4: day = "Wed"; break;
            //        case 5: day = "Thu"; break;
            //        case 6: day = "Fri"; break;
            //        case 7: day = "Sat"; break;
            //        default: break;
            //    }
            //    tc.Text = tc.Text.Replace(tc.Text.Substring(26, 3), day);
            //}
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
            for (int i = j; i < e.Row.Cells.Count; i++)
            {
                TableCell tc = e.Row.Cells[i];
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Attributes.Add("class", "Nums");
                try
                {
                    decimal dec = 0;
                    if (tc.Text == "&nbsp;")
                        dec = 0;
                    else
                        dec = decimal.Parse(tc.Text);
                    if (Totals.Contains(i))
                        Totals[i] = (decimal)Totals[i] + dec;
                    else
                        Totals.Add(i, dec);
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }
            }

            if (e.Row.Cells[4].Text.Contains("REG"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORREG);
            }
            else if (e.Row.Cells[4].Text.Contains("HOL"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORHOL);
            }
            else if (e.Row.Cells[4].Text.Contains("REST"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORREST);
            }
            else if (e.Row.Cells[4].Text.Contains("COMP"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORCOMP);
            }
            else if (e.Row.Cells[4].Text.Contains("SPL"))
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLORSPL);
            }
            else
            {
                e.Row.Attributes["style"] = string.Format("background-color:{0};", Resources.Resource.LEDGERCOLOROTHERS);
            }
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'";
            e.Row.Attributes["onclick"] = "gridSelectAllocationEntry('" + e.Row.RowIndex.ToString() + "');";
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            TableRow tr = e.Row;
            tr.Cells[0].Text = "TOTAL";
            for (int i = j; i < tr.Cells.Count; i++)
            {
                TableCell tc = tr.Cells[i];
                tc.Attributes.Add("class", "Nums");
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Text = string.Format("{0:0.00}", decimal.Parse(Totals[i].ToString()));
            }
        }
    }
    #endregion
}
