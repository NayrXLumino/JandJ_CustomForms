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

public partial class Transactions_Personnel_pgeBeneficiaryUpdateReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFTAXCIVILREP"))
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
    void Transactions_Overtime_pgeOvertimeReport_PreRender(object sender, EventArgs e)
    {

    }

    void Transactions_Overtime_pgeOvertimeReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "personnelScripts";
        string jsurl = "_personnel.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupRPLEmployee()");
        btnStatus.OnClientClick = string.Format("return lookupRPLStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupRBFCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupRPLCostcenterLine()");//use same as address report
        btnPayPeriod.OnClientClick = string.Format("return lookupRBFPayPeriod()");
        btnFirstname.OnClientClick = string.Format("return lookupBFRepBeneficairyNames('But_Firstname','txtFirstname')");
        btnLastname.OnClientClick = string.Format("return lookupBFRepBeneficairyNames('But_Lastname','txtLastname')");
        btnChecker1.OnClientClick = string.Format("return lookupRPLCheckerApprover('But_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupRPLCheckerApprover('But_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupRPLCheckerApprover('But_ApprovedBy','txtApprover')");
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFTAXCIVILREP", txtEmployee.Text, true, Session["userLogged"].ToString());
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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "BENEFICIARY UPDATE REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "Beneficiary Update Report");
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
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "BENEFICIARY UPDATE REPORT", initializeExcelHeader());
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
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFTAXCIVILREP");
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
        sql.Append(string.Format(@"                ) AS temp)
                                           SELECT [Control No]
                                                , [Status]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [Effectivity Date]
                                                , [Type]
                                                , [Beneficiary Lastname]
                                                , [Beneficiary Firstname]
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
                                                , [Occupation]
                                                , [Company]
                                                , [Gender]
                                                , [Civil Status]
                                                , [Deceased Date]
                                                , [Cancelled Date]
                                                , [Reason]
                                                , [Last Name]
                                                , [First Name]
                                                , [Checker 1]
                                                , [Checked Date 1]
                                                , [Checker 2]
                                                , [Checked Date 2]
                                                , [Approver]
                                                , [Approved Date]
                                                , [Remarks]
                                                , [Pay Period]
                                                , [Cost Center]
                                                {0}
                                             FROM TempTable
                                            !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                            ", !hasCCLine ? "" : ", [CC Line]"));
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(distinct But_ControlNo) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        if (cbxEnroute.Checked == true)
        {
            enrouteQuery = @"LEFT JOIN T_EmployeeApprovalRoute on Arm_EmployeeId = But_EmployeeId
	                                AND Arm_TransactionID = 'BNEFICIARY'
									AND But_EffectivityDate BETWEEN T_EmployeeApprovalRoute.Arm_StartDate AND ISNULL(T_EmployeeApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
                                    LEFT JOIN T_ApprovalRouteMaster 
                                    on  T_ApprovalRouteMaster.Arm_RouteId = T_EmployeeApprovalRoute.Arm_RouteID";
            sql = sql.Replace("@Checker1", "ISNULL(But_CheckedBy, Arm_Checker1)").Replace("@Checker2", "ISNULL(But_Checked2By, Arm_Checker2)").Replace("@Approver", "ISNULL(But_ApprovedBy, Arm_Approver)").Replace("@Enroute", enrouteQuery);
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
        columns = string.Format(@"    SELECT distinct But_ControlNo [Control No]
                                            , But_EmployeeId [ID No]
                                            , Emt_NickName [ID Code]
                                            , Emt_NickName [Nickname]
                                            , Emt_Lastname [Lastname]
                                            , Emt_Firstname [Firstname]
                                            , But_SeqNo [Seq No]
                                            , Convert(varchar(10), But_EffectivityDate, 101) [Effectivity Date]
                                            , CASE WHEN (But_Type = 'N')
                                                   THEN 'NEW ENTRY'
                                                   ELSE 'UPDATE EXISTING'
                                               END [Type]
                                            , But_Lastname [Beneficiary Lastname]
                                            , But_Firstname [Beneficiary Firstname]
                                            , But_Middlename [Beneficiary Middlename]
                                            , Convert(varchar(10), But_Birthdate, 101) [Birthdate]
                                            , But_Relationship [Relationship Code]
                                            , AD2.Adt_AccountDesc [Relationship Desc]
                                            , But_Hierarchy [Hierarchy Code]
                                            , AD3.Adt_AccountDesc [Hierarchy Desc]
                                            , CASE WHEN (But_HMODependent = 0)
                                                THEN 'NO' 
                                                ELSE 'YES' 
                                                END [HMO Dependent]
                                            , CASE WHEN (But_InsuranceDependent = 0)
                                                THEN 'NO' 
                                                ELSE 'YES' 
                                                END [Insurance Dependent]
                                            , CASE WHEN (But_BIRDependent = 0) 
                                                THEN 'NO' 
                                                ELSE 'YES' 
                                                END [BIR Dependent]
                                            , CASE WHEN (But_AccidentDependent = 0)
                                                THEN 'NO' 
                                                ELSE 'YES' 
                                                END [Accident Dependent]   
                                            , But_Occupation [Occupation]
                                            , But_Company [Company]
                                            , But_Gender [Gender]
                                            , But_CivilStatus [Civil Status]
                                            , Convert(varchar(10), But_DeceasedDate, 101) [Deceased Date]
                                            , Convert(varchar(10), But_CancelDate, 101) [Cancelled Date]
                                            , But_Reason [Reason]
                                            , AD1.Adt_AccountDesc [Status]
                                            , Emt_Lastname [Last Name]
                                            , Emt_Firstname [First Name]
                                            , dbo.GetControlEmployeeNameV2(@Checker1) [Checker 1]
                                            , Convert(varchar(10), But_CheckedDate,101) 
                                              + ' ' 
                                              + RIGHT(Convert(varchar(17), But_CheckedDate,113),5) [Checked Date 1]
                                            , dbo.GetControlEmployeeNameV2(@Checker2) [Checker 2]
                                            , Convert(varchar(10), But_Checked2Date,101) 
                                              + ' ' 
                                              + RIGHT(Convert(varchar(17), But_Checked2Date,113),5) [Checked Date 2]
                                            , dbo.GetControlEmployeeNameV2(@Approver) [Approver]
                                            , Convert(varchar(10), But_ApprovedDate,101) 
                                              + ' ' 
                                              + RIGHT(Convert(varchar(17), But_ApprovedDate,113),5) [Approved Date]
                                            , Trm_Remarks [Remarks]
                                            , But_CurrentPayPeriod [Pay Period]
                                            , But_Costcenter [Cost Center]
                                            {0} ", !hasCCLine ? "" : ", But_CostCenterLine [CC Line]");
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        filter = string.Format( @"     FROM T_BeneficiaryUpdate
                                       LEFT JOIN T_UserMaster C1 
                                         ON C1.Umt_UserCode = But_CheckedBy
                                       LEFT JOIN T_UserMaster C2 
                                         ON C2.Umt_UserCode = But_Checked2By
                                       LEFT JOIN T_UserMaster AP 
                                         ON AP.Umt_UserCode = But_ApprovedBy
                                       INNER JOIN T_EmployeeMaster 
                                         ON Emt_EmployeeId = But_EmployeeId
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
                                       LEFT JOIN T_PayPeriodMaster 
                                         ON Ppm_PayPeriod = But_CurrentPayPeriod
                                        @Enroute
                                      WHERE 1 = 1  "); 
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
            filter += string.Format(@" AND  ( But_Costcenter {0}
                                           OR dbo.getCostCenterFullNameV2(But_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text));
        }
        if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "PERSONNEL"))
            {
                filter += string.Format(@" AND  (  ( But_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'PERSONNEL')
                                                      OR But_EmployeeId = '{0}'))", Session["userLogged"].ToString());


            }
            if (hasCCLine)//flag costcenter line to retain old code
            {
                filter += string.Format(@" AND (But_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'PERSONNEL') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
									            OR But_CostCenter + ISNULL(But_CostcenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'PERSONNEL'))
                                                OR But_EmployeeID = '{0}') ", Session["userLogged"].ToString());
            }
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_Status {0}
                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
        }
        if (!txtFirstname.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_Firstname {0})", sqlINFormat(txtFirstname.Text));
        }
        if (!txtLastname.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_Lastname {0})", sqlINFormat(txtLastname.Text));
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_CheckedBy {0}
                                           OR C1.Umt_UserCode {0}
                                           OR C1.Umt_UserLname {0}
                                           OR C1.Umt_UserFname {0}
                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_Checked2By {0}
                                           OR C2.Umt_UserCode {0}
                                           OR C2.Umt_UserLname {0}
                                           OR C2.Umt_UserFname {0}
                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( But_ApprovedBy {0}
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
            filter += string.Format(@" AND But_EffectivityDate BETWEEN '{0}' AND '{1}'", dtpEffDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpEffDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEffDateFrom.IsNull)
        {
            filter += string.Format(@" AND But_EffectivityDate >= '{0}'", dtpEffDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEffDateTo.IsNull)
        {
            filter += string.Format(@" AND But_EffectivityDate <= '{0}'", dtpEffDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Applied Date
        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND But_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedFrom.IsNull)
        {
            filter += string.Format(@" AND But_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAppliedTo.IsNull)
        {
            filter += string.Format(@" AND But_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
        }

        //Birth Date
        if (!dtpBirthDateFrom.IsNull && !dtpBirthDateTo.IsNull)
        {
            filter += string.Format(@" AND But_BirthDate BETWEEN '{0}' AND '{1}'", dtpBirthDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                 , dtpBirthDateTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpBirthDateFrom.IsNull)
        {
            filter += string.Format(@" AND But_BirthDate >= '{0}'", dtpBirthDateFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpBirthDateTo.IsNull)
        {
            filter += string.Format(@" AND But_BirthDate <= '{0}'", dtpBirthDateTo.Date.ToString("MM/dd/yyyy"));
        }

        //Endorsed Date
        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND But_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedFrom.IsNull)
        {
            filter += string.Format(@" AND But_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpEndorsedTo.IsNull)
        {
            filter += string.Format(@" AND But_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date
        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
        {
            filter += string.Format(@" AND But_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1From.IsNull)
        {
            filter += string.Format(@" AND But_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC1To.IsNull)
        {
            filter += string.Format(@" AND But_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
        }
        //Checked Date 2
        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
        {
            filter += string.Format(@" AND But_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2From.IsNull)
        {
            filter += string.Format(@" AND But_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpC2To.IsNull)
        {
            filter += string.Format(@" AND But_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
        }
        //Approved Date
        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND But_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPFrom.IsNull)
        {
            filter += string.Format(@" AND But_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
        }
        else if (!dtpAPTo.IsNull)
        {
            filter += string.Format(@" AND But_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
        }
        #endregion
        #region Drop down filler
        if (!ddlAccident.SelectedValue.Equals(string.Empty))
        {
            filter += string.Format(@" AND  But_AccidentDependent = {0} ", ddlAccident.SelectedValue);
        }

        if (!ddlBIR.SelectedValue.Equals(string.Empty))
        {
            filter += string.Format(@" AND  But_BIRDependent = {0} ", ddlBIR.SelectedValue);
        }

        if (!ddlHMO.SelectedValue.Equals(string.Empty))
        {
            filter += string.Format(@" AND  But_HMODependent = {0} ", ddlHMO.SelectedValue);
        }

        if (!ddlInsurance.SelectedValue.Equals(string.Empty))
        {
            filter += string.Format(@" AND  But_InsuranceDependent = {0} ", ddlInsurance.SelectedValue);
        }
        #endregion
        if (cbxEnroute.Checked == true)
        {
            filter += @" AND 1 = 1 AND But_Status in ('3','5','7')";
        }

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( But_ControlNo LIKE '{0}%' )
                                          OR ( But_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                          OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 4)) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),But_EffectivityDate,101) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),But_AppliedDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),But_AppliedDate,114),5) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),But_EndorsedDateToChecker,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),But_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                          OR ( CASE WHEN (But_Type = 'N')
                                                    THEN 'NEW ENTRY'
                                                    ELSE 'UPDATE EXISTING'
                                                END LIKE '{0}%' )
                                          OR ( But_Lastname LIKE '{0}%' )
                                          OR ( But_Firstname LIKE '{0}%' )
                                          OR ( But_Middlename LIKE '{0}%' )
                                          OR ( Convert(varchar(10), But_Birthdate, 101) LIKE '{0}%' )
                                          OR ( But_Relationship LIKE '{0}%' )
                                          OR ( AD2.Adt_AccountDesc LIKE '{0}%' )
                                          OR ( But_Hierarchy LIKE '{0}%' )
                                          OR ( AD3.Adt_AccountDesc LIKE '{0}%' )
                                          OR ( But_HMODependent LIKE '{0}%' )
                                          OR ( But_InsuranceDependent LIKE '{0}%' )
                                          OR ( But_BIRDependent LIKE '{0}%' )
                                          OR ( But_AccidentDependent LIKE '{0}%' )
                                          OR ( Convert(varchar(10), But_DeceasedDate, 101) LIKE '{0}%' )
                                          OR ( Convert(varchar(10), But_CancelDate, 101) LIKE '{0}%' )
                                          OR (But_Occupation LIKE '{0}%')
                                          OR (But_Company LIKE '{0}%')
                                          OR (But_Gender LIKE '{0}%')
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
            criteria += lblEffDateFrom.Text + ":" + dtpEffDateFrom.Date.ToString("MM/dd/yyyy") + "-";
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
