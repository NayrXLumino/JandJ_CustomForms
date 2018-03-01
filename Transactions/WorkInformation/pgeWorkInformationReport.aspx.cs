using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Text;
using System.Collections;

public partial class Transactions_WorkInformation_pgeWorkInformationReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFMVEREP"))  
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
            PreRender += new EventHandler(Transactions_WorkInformation_pgeWorkInformationReport_PreRender);
        }
        LoadComplete += new EventHandler(Transactions_WorkInformation_pgeWorkInformationReport_LoadComplete);
    }

    #region Events
    void Transactions_WorkInformation_pgeWorkInformationReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "workinformationScripts";
        string jsurl = "_workinformation.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupRMVEmployee()");
        btnStatus.OnClientClick = string.Format("return lookupRMVStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupRMVCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRMVCostcenterLine()");
        btnChecker1.OnClientClick = string.Format("return lookupRMVCheckerApprover('Mve_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupRMVCheckerApprover('Mve_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupRMVCheckerApprover('Mve_ApprovedBy','txtApprover')");
        
        // temp
        btnFrom.OnClientClick = string.Format("return lookupRMVFromTo('Mve_From', 'txtFrom')");
        btnTo.OnClientClick = string.Format("return lookupRMVFromTo('Mve_To', 'txtTo')");
        btnBatchNo.OnClientClick = string.Format("return lookupRMVBatchNo()");
        btnPayrollPeriod.OnClientClick = string.Format("return lookupRMVPayPeriod()");
        
    }

    void Transactions_WorkInformation_pgeWorkInformationReport_PreRender(object sender, EventArgs e)
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
        SystemMenuLogBL.InsertGenerateLog("WFMVEREP", txtEmployee.Text, true, Session["userLogged"].ToString());
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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "MOVEMENT REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Movement Report");
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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "MOVEMENT REPORT", initializeExcelHeader());
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
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFMVEREP"); 
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

    private string SQLBuilder(string replaceString)
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string enrouteQuery = string.Empty;
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                        SET @startIndex = (@pageIndex * @numRow) + 1;
                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Lastname], [Effectivity Date] @Sort) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(string.Format(@"        ) AS temp)
                                           SELECT [Control No]
                                                , [Status]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [Effectivity Date]
                                                , [Type]
                                                , [From]
                                                , [To]
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
                                                , [Batch No]
                                                , [Remarks]
                                                , [Pay Period]
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                                ", !hasCCLine ? "" : ", [CC Line]"));
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT( distinct Mve_ControlNo ) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        if (cbxEnroute.Checked == true)
        {
            enrouteQuery = @"LEFT JOIN T_EmployeeApprovalRoute on Arm_EmployeeId = Mve_EmployeeId
	                            AND Arm_TransactionID = 'MOVEMENT'
								AND Mve_EffectivityDate BETWEEN T_EmployeeApprovalRoute.Arm_StartDate AND ISNULL(T_EmployeeApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                             LEFT JOIN T_ApprovalRouteMaster 
	                            on  T_ApprovalRouteMaster.Arm_RouteId = T_EmployeeApprovalRoute.Arm_RouteID";
            sql = sql.Replace("@Checker1", "ISNULL(Mve_CheckedBy, Arm_Checker1)").Replace("@Checker2", "ISNULL(Mve_Checked2By, Arm_Checker2)").Replace("@Approver", "ISNULL(Mve_ApprovedBy, Arm_Approver)").Replace("@Enroute", enrouteQuery);
        }
        else
        {
            enrouteQuery = @"";
            sql = sql.Replace("@Checker1", "C1.Umt_UserCode").Replace("@Checker2", "C2.Umt_UserCode").Replace("@Approver", "AP.Umt_UserCode").Replace("@Enroute", enrouteQuery);
        }
        return sql.ToString().Replace("!#!", replaceString).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }

    private string getColumns()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string columns = string.Empty;
        columns = string.Format(@"   SELECT distinct Mve_ControlNo [Control No]
                                          , Mve_EmployeeId [ID No]
                                          , Emt_NickName [ID Code]
                                          , Emt_NickName [Nickname]
                                          , Emt_Lastname [Lastname]
                                          , Emt_Firstname [Firstname]
                                          , Convert(varchar(10), Mve_EffectivityDate, 101) [Effectivity Date]
                                          , MOVETYPE.Adt_AccountDesc [Type]
                                          , CASE WHEN (Mve_Type = 'S')
                                                    THEN '[ ' 
                                                    + S1.Scm_ShiftCode 
                                                    + ' ] ' 
                                                    + ' '
                                                    + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
                                                    + ' - '
                                                    + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2)
                                                WHEN (Mve_Type = 'G')
                                                    THEN WTF.Adt_AccountDesc + '/' + WGF.Adt_AccountDesc 
                                                WHEN (Mve_Type = 'C')
                                                    THEN dbo.getCostCenterFullNameV2(Mve_From)
                                                ELSE Mve_From
                                            END [From]
                                          , CASE WHEN (Mve_Type = 'S')
                                                THEN '[ ' 
                                                + S2.Scm_ShiftCode 
                                                + ' ] ' 
                                                + ' '
                                                + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
                                                + ' - '
                                                + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2)
                                                WHEN (Mve_Type = 'G')
                                                THEN WTT.Adt_AccountDesc + ' / ' + WGT.Adt_AccountDesc 
                                                WHEN (Mve_Type = 'C')
                                                THEN dbo.getCostCenterFullNameV2(Mve_To)
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
                                          , Mve_CurrentPayPeriod [Pay Period]
                                          , Mve_Reason [Reason]
                                          , AD1.Adt_AccountDesc [Status]
                                          , Emt_FirstName [First Name]
                                          , Emt_LastName [Last Name]
                                          , dbo.GetControlEmployeeNameV2(@Checker1) [Checker 1]
                                          , Convert(varchar(10), Mve_CheckedDate,101) 
                                            + ' ' 
                                            + RIGHT(Convert(varchar(17), Mve_CheckedDate,113),5) [Checked Date 1]
                                          , dbo.GetControlEmployeeNameV2(@Checker2) [Checker 2]
                                          , Convert(varchar(10), Mve_Checked2Date,101) 
                                            + ' ' 
                                            + RIGHT(Convert(varchar(17), Mve_Checked2Date,113),5) [Checked Date 2]
                                          , dbo.GetControlEmployeeNameV2(@Approver) [Approver]
                                          , Convert(varchar(10), Mve_ApprovedDate,101) 
                                            + ' ' 
                                            + RIGHT(Convert(varchar(17), Mve_ApprovedDate,113),5) [Approved Date]
                                          , Mve_BatchNo [Batch No]
                                          , Trm_Remarks [Remarks] ", !hasCCLine ? "" : ", Mve_CostCenterLine [CC Line]");
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        filter = string.Format(@"      FROM T_Movement
                                       LEFT JOIN T_UserMaster C1 
                                         ON C1.Umt_UserCode = Mve_CheckedBy
                                       LEFT JOIN T_UserMaster C2 
                                         ON C2.Umt_UserCode = Mve_Checked2By
                                       LEFT JOIN T_UserMaster AP 
                                         ON AP.Umt_UserCode = Mve_ApprovedBy
                                       INNER JOIN T_EmployeeMaster 
                                         ON Emt_EmployeeId = Mve_EmployeeId
                                       LEFT JOIN T_AccountDetail AD1 
                                         ON AD1.Adt_AccountCode = Mve_Status 
                                        AND AD1.Adt_AccountType =  'WFSTATUS'
                                       LEFT JOIN T_AccountDetail MOVETYPE
                                         ON MOVETYPE.Adt_AccountCode = Mve_Type
                                        AND MOVETYPE.Adt_AccountType = 'MOVETYPE' 
                                       LEFT JOIN T_ShiftCodeMaster S1
                                        ON S1.Scm_ShiftCode = Mve_From
                                        AND Mve_Type = 'S'
                                       LEFT JOIN T_ShiftCodeMaster S2
                                        ON S2.Scm_ShiftCode = Mve_To
                                        AND Mve_Type = 'S'
                                       LEFT JOIN T_AccountDetail WTF
                                        ON WTF.Adt_AccountCode = LTRIM(LEFT(Mve_From, 3))
                                        AND WTF.Adt_AccountType = 'WORKTYPE'
                                        AND Mve_Type = 'G'
                                       LEFT JOIN T_AccountDetail WGF
                                        ON WGF.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
                                        AND WGF.Adt_AccountType = 'WORKGROUP'
                                        AND Mve_Type = 'G'
                                       LEFT JOIN T_AccountDetail WTT
                                        ON WTT.Adt_AccountCode = LTRIM(LEFT(Mve_To, 3))
                                        AND WTT.Adt_AccountType = 'WORKTYPE'
                                        AND Mve_Type = 'G'
                                       LEFT JOIN T_AccountDetail WGT
                                        ON WGT.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
                                        AND WGT.Adt_AccountType = 'WORKGROUP'
                                        AND Mve_Type = 'G'
                                       LEFT JOIN T_TransactionRemarks 
                                         ON Trm_ControlNo = Mve_ControlNo
                                       LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Mve_CurrentPayPeriod
                                       @Enroute
                                      WHERE 1 = 1 
                                        AND Mve_Status <> '' ");  
        #region textBox Filters
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_EmployeeId {0}
                                           OR Emt_Lastname {0}
                                           OR Emt_Firstname {0}
                                           OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
        }
        if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_Costcenter {0}
                                           OR dbo.getCostCenterFullNameV2(Mve_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text));
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" AND  (  ( Mve_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                            FROM T_UserCostCenterAccess
                                                                            WHERE Uca_UserCode = '{0}'
                                                                            AND Uca_SytemId = 'TIMEKEEP')
                                                    OR Mve_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }
            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND (Mve_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'TIMEKEEP') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									            OR Mve_CostCenter + ISNULL(Mve_CostcenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'TIMEKEEP'))
                                                OR Mve_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_Status {0}
                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
        }
        if (!txtFrom.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_From {0})", sqlINFormatGroup(txtFrom.Text));
        }
        if (!txtTo.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_To {0})", sqlINFormatGroup(txtTo.Text));  
        }
        if (!txtBatchNo.Text.Trim().Equals(string.Empty))   
        {
            filter += string.Format(@" AND  ( Mve_BatchNo {0})", sqlINFormat(txtBatchNo.Text));   
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_CheckedBy {0}
                                           OR C1.Umt_UserCode {0}
                                           OR C1.Umt_UserLname {0}
                                           OR C1.Umt_UserFname {0}
                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_Checked2By {0}
                                           OR C2.Umt_UserCode {0}
                                           OR C2.Umt_UserLname {0}
                                           OR C2.Umt_UserFname {0}
                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Mve_ApprovedBy {0}
                                           OR AP.Umt_UserCode {0}
                                           OR AP.Umt_UserLname {0}
                                           OR AP.Umt_UserFname {0}
                                           OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
        }
        #endregion
        #region DateTime Pickers
        //Overtime Date
        if (!dtpEffDateFrom.IsNull && !dtpEffDateTo.IsNull)
        {
            filter += string.Format(@" AND Mve_EffectivityDate BETWEEN '{0}' AND '{1}'", dtpEffDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpEffDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEffDateFrom.IsNull)
        {
            filter += string.Format(@" AND Mve_EffectivityDate >= '{0}'", dtpEffDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEffDateTo.IsNull)
        {
            filter += string.Format(@" AND Mve_EffectivityDate <= '{0}'", dtpEffDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Applied Date
        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Mve_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedFrom.IsNull)
        {
            filter += string.Format(@" AND Mve_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND Mve_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Endorsed Date
        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Mve_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedFrom.IsNull)
        {
            filter += string.Format(@" AND Mve_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND Mve_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date
        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Mve_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1From.IsNull)
        {
            filter += string.Format(@" AND Mve_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1To.IsNull)
        {
            filter += string.Format(@" AND Mve_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date 2
        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Mve_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2From.IsNull)
        {
            filter += string.Format(@" AND Mve_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2To.IsNull)
        {
            filter += string.Format(@" AND Mve_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        //Approved Date
        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Mve_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPFrom.IsNull)
        {
            filter += string.Format(@" AND Mve_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND Mve_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion

        if (cbxEnroute.Checked == true)
        {
            filter += @" AND 1 = 1 AND Mve_Status in ('3','5','7')";
        }
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Mve_ControlNo LIKE '{0}%' )
                                          OR ( Mve_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                          OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 4)) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Mve_EffectivityDate,101) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Mve_AppliedDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Mve_AppliedDate,114),5) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Mve_EndorsedDateToChecker,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Mve_EndorsedDateToChecker,114),5) LIKE '{0}%' ) 
                                          OR ( Mve_From LIKE '{0}%' )
                                          OR ( Mve_To LIKE '{0}%' )
                                          OR ( WTF.Adt_AccountDesc LIKE '%{0}%' )
                                          OR ( WGF.Adt_AccountDesc LIKE '%{0}%' )
                                          OR ( WTT.Adt_AccountDesc LIKE '%{0}%' )
                                          OR ( WGT.Adt_AccountDesc LIKE '%{0}%' )
                                          OR ( Mve_BatchNo LIKE '{0}%' )
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

        if (ddlMoveType.SelectedValue != "ALL")
        {
            filter += string.Format(@" AND  ( Mve_Type = '{0}' )", ddlMoveType.SelectedValue);
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

    private string sqlINFormatGroup(string text)    // same with sqlINFormat but formats group codes 
    {
        string[] temp = text.Split(',');
        string value = "IN ( ";
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].Contains(" "))
                temp[i] = formatWorkGroup(temp[i]);
            value += "'" + temp[i] + "'";
            if (i != temp.Length - 1)
                value += ",";
        }
        value += ")";
        return value;
    }

    private string formatWorkGroup(string groupcode) 
    {
        string[] group = groupcode.Trim().Split(' ');

        groupcode = string.Empty;

        for (int h = 0; h < group.Length; h++)
        {
            for (int i = group[h].Length; i < 3; i++)
            {
                group[h] = " " + group[h];
            }
            groupcode += group[h];
        }
        return groupcode;
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
        if (!dtpEffDateFrom.IsNull)
        {
            criteria += lblOTDateFrom.Text + ":" + dtpEffDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEffDateTo.IsNull)
        {
            criteria += lblOTDateTo.Text + ":" + dtpEffDateTo.Date.ToString("MM/dd/yyyy") + "-";
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