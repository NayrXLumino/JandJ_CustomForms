/* Revision no. W2.1.00003 
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   06/11/2013
 *  Update Notes    :   
 *      -   calculate hours for cancellation add query to look on history
 */
/*
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   06/11/2013
 *  Update Notes    :   
 *      -   calculate hours for cancellation add query to look on history
 */
/*
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   05/29/2013
 *  Update Notes    :   
 *      -   calculate hours for cancellation
 */
/*
*  Updated By      :   0951 - Te, Perth
*  Updated Date    :   03/14/2013
*  Update Notes    :   
*      -   Updated Cancellation part to check if Leave Credits are cancellable
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


public partial class Transactions_Leave_pgeLeaveCancellation : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    LeaveBL LVBL = new LeaveBL();

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
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLVECANCEL"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    txtSearch_TextChanged(null, null);
                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                    Page.PreRender += new EventHandler(Page_PreRender);
                }
            }
            LoadComplete += new EventHandler(Transactions_Leave_pgeLeaveIndividual_LoadComplete);
        }
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    #region Events

    void Transactions_Leave_pgeLeaveIndividual_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "leaveScripts";
        string jsurl = "_leave.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtControlNo.Attributes.Add("readOnly", "true");
        txtRemarks.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtRemarks',199);");
        txtReason.Attributes.Add("OnKeyPress", "javascript:return isMaxLength('txtReason',199);");
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

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValueToControlNo('" + e.Row.RowIndex + "')");
        }
    }

    protected void iBtnListDetail_Click(object sender, ImageClickEventArgs e)
    {
        if (((ImageButton)sender).ID == "iBtnList")
        {
            VIEWER.ActiveViewIndex = 0;
            txtControlNo.Text = string.Empty;
            iBtnDetail.Enabled = true;
            iBtnList.Enabled = false;
        }
        else
        {
            if (!txtControlNo.Text.Equals(string.Empty))
            {
                VIEWER.ActiveViewIndex = 1;
                setDetailInformation();
                iBtnDetail.Enabled = false;
                iBtnList.Enabled = true;
            }
            else
            {
                MessageBox.Show("Select item in grid to show details.");
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            string errorMessage = string.Empty;
            if (Page.IsValid)
            {
                if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                {
                    if (!CommonMethods.isAffectedByCutoff("LEAVE", hfLeaveDate.Value.ToUpper()))
                    {
                        errorMessage = checkEntry();
                        if (errorMessage.Equals(string.Empty))
                        {
                            #region Transact to Database
                            using (DALHelper dal = new DALHelper())
                            {
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransactionSnapshot();

                                    DataTable dtResult = dal.ExecuteDataSet(string.Format("EXEC CancelApprovedLeave '{0}', '{1}', '{2}', '{3}', '{4}' "
                                                                                                , txtControlNo.Text
                                                                                                , Session["userLogged"].ToString()
                                                                                                , txtRemarks.Text.Trim().ToUpper()
                                                                                                , HttpContext.Current.Session["dbPrefix"].ToString()
                                                                                                , Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))).Tables[0];
                                    DataRow[] drArrRows = dtResult.Select("Result = 1");
                                    if (drArrRows.Length > 0)
                                    {
                                        //MenuLog
                                        SystemMenuLogBL.InsertAddLog("WFLVECANCEL", true, hfEmployeeId.Value.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                                        MessageBox.Show("Successfully cancelled transaction.");
                                    }
                                    else
                                    {
                                        drArrRows = dtResult.Select("Result <> 1");
                                        if (drArrRows.Length > 0)
                                            MessageBox.Show(drArrRows[0]["Message"].ToString());
                                    }

                                    dal.CommitTransactionSnapshot();

                                    txtSearch_TextChanged(null, null);
                                    txtRemarks.Text = string.Empty;
                                    txtControlNo.Text = string.Empty;
                                    VIEWER.ActiveViewIndex = 0;
                                }
                                catch (Exception ex)
                                {
                                    dal.RollBackTransactionSnapshot();
                                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                                }
                                finally
                                {
                                    dal.CloseDB();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            MessageBox.Show(errorMessage);
                        }
                    }
                    else
                    {
                        MessageBox.Show(CommonMethods.GetErrorMessageForCutOff("LEAVE"));
                    }
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
                }
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
            }
        }
    }
    #endregion

    #region Methods
    public decimal getHourForCancellation()
        {
            decimal final = 0;
            string sqlquery = string.Format(@"
SELECT
	CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Leave Credits to Deduct to Leave Credits Table]
FROM T_EmployeeLeaveAvailment
LEFT JOIN  T_ProcessControlMaster
	ON Pcm_SystemID = 'LEAVE'
	AND Pcm_ProcessID = 'DAYSEL'
LEFT JOIN T_ParameterMasterExt	
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
	WHERE Elt_ControlNo='{0}'
union
SELECT
	CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Leave Credits to Deduct to Leave Credits Table]
FROM T_EmployeeLeaveAvailmenthist
LEFT JOIN  T_ProcessControlMaster
	ON Pcm_SystemID = 'LEAVE'
	AND Pcm_ProcessID = 'DAYSEL'
LEFT JOIN T_ParameterMasterExt	
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
	WHERE Elt_ControlNo='{0}'
	", this.txtControlNo.Text);
            
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        final = Convert.ToDecimal(dal.ExecuteScalar(sqlquery));
                        dal.CloseDB();
                    }
                    catch
                    {
                    }
                 }
           
            return final;
 
        }
    private string checkEntry()
    {
        string errMsg = string.Empty;
        if (txtControlNo.Text.Equals(string.Empty) && errMsg.Equals(string.Empty))
        {
            errMsg = "Error: \n No control number selected.";//double checking
        }
        if (txtRemarks.Text.Equals(string.Empty) && errMsg.Equals(string.Empty))
        {
            errMsg = "Error: \n Enter remarks for cancellation.";//double checking
        }
        if (methods.GetProcessControlFlag("LEAVE", "CUT-OFF") && errMsg.Equals(string.Empty))
        {
            errMsg = "Error: \n Leave is on cut-off.";
        }

        if (errMsg.Equals(string.Empty))
        {
            if (MethodsLibrary.Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
            {
                errMsg += CommonMethods.GetErrorMessageForCYCCUTOFF();
            }
        }
        return errMsg;
    }
    private void bindGrid()
    {
        txtControlNo.Text = txtRemarks.Text = string.Empty;
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                string str = SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString());
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
        if (!CommonMethods.isEmpty(ds))
        {
            #region Remove Columns
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                hfVWNICKNAME.Value = "TRUE";
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    hfDSPIDCODE.Value = "TRUE";
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
            else
            {
                hfDSPFULLNM.Value = "TRUE";
            }
            //Depending if Used
            if (CommonMethods.getFillerStatus("Elt_Filler01") == "C")
                ds.Tables[0].Columns.Remove(getFillerName("Elt_Filler01"));
            if (CommonMethods.getFillerStatus("Elt_Filler02") == "C")
                ds.Tables[0].Columns.Remove(getFillerName("Elt_Filler02"));
            if (CommonMethods.getFillerStatus("Elt_Filler03") == "C")
                ds.Tables[0].Columns.Remove(getFillerName("Elt_Filler03"));

            #endregion
        }
        hfRowCount.Value = "0";
        if (!CommonMethods.isEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[1].Rows)
                hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
            dgvResult.DataSource = ds;
            dgvResult.DataBind();
        }
        else
        {
            dgvResult.DataSource = null;
            dgvResult.DataBind();
        }
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

    private string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                     declare @Current as datetime;
                     declare @MINPASTDATE as datetime;
                         SET @startIndex = (@pageIndex * @numRow) + 1;
                     
                         SET @Current = (SELECT Ppm_StartCycle 
                                           FROM T_PayPeriodMaster 
                                          WHERE Ppm_CycleIndicator = 'C');
                         SET @MINPASTDATE = (SELECT ISNULL(MIN(Dates),@Current)
                                               FROM ( SELECT TOP @MINPASTPRD Ppm_StartCycle [Dates]
	                                                    FROM T_PayPeriodMaster
 	                                                   WHERE Ppm_CycleIndicator = 'P'
	                                                   ORDER BY 1 DESC) AS TEMP);

                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [ID No], [Leave Date]) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("@Table", "T_EmployeeLeaveAvailment"));
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("@Table", "T_EmployeeLeaveAvailmentHist"));
        sql.Append(@"                              ) AS temp)
                                           SELECT [Control No]
                                                , [ID No]
                                                , [ID Code]
                                                , [Nickname]
                                                , [Lastname]
                                                , [Firstname]
                                                , [MI]
                                                , [Leave Date]
                                                , [Leave Code]
                                                , [Leave Desc]
                                                , [Category]
                                                , [Start]
                                                , [End]
                                                , [Hours]
                                                , [Day Unit]
                                                , [Shift Code]
                                                , [Cost Center]
                                                , [Applied Date]
                                                , [Endorsed Date]
                                                , [Reason]
                                                , [@Elt_Filler1Desc]
                                                , [@Elt_Filler2Desc]
                                                , [@Elt_Filler3Desc]
                                                , [Status]
                                                , [Checker 1]
                                                , [Checked Date 1]
		                                        , [Checker 2]
                                                , [Checked Date 2]
		                                        , [Approver]
                                                , [Approved Date]
                                                , [Remarks]
                                                , [Pay Period]
                                             FROM TempTable
                                            WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(distinct Elt_ControlNo) [cnt]");
        sql.Append(getFilters().Replace("@Table", "T_EmployeeLeaveAvailment"));
        sql.Append(@"           UNION 
                               SELECT COUNT(Elt_EmployeeId)");
        sql.Append(getFilters().Replace("@Table", "T_EmployeeLeaveAvailmentHist"));
        sql.Append(@"        ) as Rows");

        sql.Replace("@Elt_Filler1Desc", getFillerName("Elt_Filler01"));
        sql.Replace("@Elt_Filler2Desc", getFillerName("Elt_Filler02"));
        sql.Replace("@Elt_Filler3Desc", getFillerName("Elt_Filler03"));
        sql.Replace("@MINPASTPRD", Convert.ToInt32(CommonMethods.getParamterValue("MINPASTPRD")).ToString());
        sql.Replace("@UserLogged", Session["userLogged"].ToString());
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT distinct Elt_ControlNo [Control No]
                          , Elt_EmployeeId [ID No]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname]
                          , LEFT(Emt_MiddleName,1) [MI]
                          , Convert(varchar(10), Elt_LeaveDate, 101) [Leave Date]
                          , Elt_LeaveType [Leave Code]
                          , Ltm_LeaveDesc [Leave Desc]
                          , ISNULL(AD5.Adt_AccountDesc, '- not applicable -') [Category]
                          , LEFT(Elt_StartTime,2) 
                            + ':' 
                            + RIGHT(Elt_StartTime,2) [Start]
                          , LEFT(Elt_EndTime,2) 
                            + ':' 
                            + RIGHT(Elt_EndTime,2) [End]
                          , Elt_LeaveHour [Hours]
						  , Elt_DayUnit [Day Unit]
                          , ISNULL(EL1.Ell_ShiftCode, ISNULL(EL2.Ell_ShiftCode, Emt_ShiftCode)) [Shift Code]
                          , dbo.getCostCenterFullNameV2(Elt_CostCenter) [Cost Center]
                          , Convert(varchar(10), Elt_AppliedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Elt_AppliedDate,113),5) [Applied Date]
                          , Convert(varchar(10), Elt_EndorsedDateToChecker,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Elt_EndorsedDateToChecker,113),5) [Endorsed Date]
                          , Elt_Reason [Reason]
                          , AD2.Adt_AccountDesc [@Elt_Filler1Desc]
                          , AD3.Adt_AccountDesc [@Elt_Filler2Desc]
                          , AD4.Adt_AccountDesc [@Elt_Filler3Desc]
                          , AD1.Adt_AccountDesc [Status]
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
                          , Trm_Remarks [Remarks]
                          , Elt_CurrentPayPeriod [Pay Period]";
        return columns;
    }

    private string getFilters()
    {
        //apsungahid added 20141121
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string filter = string.Empty;
        filter =string.Format(@"   FROM @Table
                      LEFT JOIN T_LeaveTypeMaster
                        ON Ltm_LeaveType = Elt_LeaveType
                      LEFT JOIN T_EmployeeLogLedger EL1
                        ON EL1.Ell_ProcessDate = Elt_LeaveDate
                       AND EL1.Ell_EmployeeId = Elt_EmployeeId
                      LEFT JOIN T_EmployeeLogLedgerHist EL2
                        ON EL2.Ell_ProcessDate = Elt_LeaveDate
                       AND EL2.Ell_EmployeeId = Elt_EmployeeId
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
                        ON AD5.Adt_AccountCode = Elt_LeaveCategory
                       AND AD5.Adt_AccountType = 'LVECATEGRY'
                      LEFT JOIN T_TransactionRemarks 
                        ON Trm_ControlNo = Elt_ControlNo 
                     INNER JOIN T_UserCostCenterAccess
						ON Uca_SytemID = 'LEAVE'
					   AND (Uca_CostCenterCode = Elt_Costcenter OR Uca_CostCenterCode = 'ALL')
					   AND Uca_Status = 'A'
					   AND Uca_Usercode = '@UserLogged'
                        {0}
                     WHERE 1 = 1 
                       AND Elt_Status IN ('9', 'A')
                       AND Elt_LeaveDate >= @MINPASTDATE 
                       AND Elt_ControlNo NOT IN (   SELECT Elt_RefControlNo
								                      FROM T_EmployeeLeaveAvailment
							                         WHERE 
                                                       --Elt_EmployeeId = ISNULL(EL1.Ell_EmployeeId, EL2.Ell_EmployeeId)
                                                       --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,ISNULL(EL1.Ell_ProcessDate, EL2.Ell_ProcessDate))) AND dateadd(day, 1, Convert(datetime,ISNULL(EL1.Ell_ProcessDate, EL2.Ell_ProcessDate)))
								                       --AND 
                                                        Elt_Status = '0'
                                                        AND Elt_RefControlNo IS NOT NULL
							                         UNION 
							                        SELECT Elt_RefControlNo
								                      FROM T_EmployeeLeaveAvailmentHist
							                         WHERE 
                                                       --Elt_EmployeeId = ISNULL(EL1.Ell_EmployeeId, EL2.Ell_EmployeeId)
                                                       --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,ISNULL(EL1.Ell_ProcessDate, EL2.Ell_ProcessDate))) AND dateadd(day, 1, Convert(datetime,ISNULL(EL1.Ell_ProcessDate, EL2.Ell_ProcessDate)))
								                       --AND 
                                                        Elt_Status = '0'
                                                        AND Elt_RefControlNo IS NOT NULL) 
                                                ", !hasCCLine ? "" : @"---apsungahid added for line code access filter 20141121
                                                                                          LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                                      FROM E_CostCenterLineMaster 
												                                                         WHERE Clm_Status = 'A' ) AS HASLINE
										                                                       ON Clm_CostCenterCode = Elt_Costcenter  ");

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
            filter += string.Format(@" AND ( ISNULL(Elt_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Elt_CostcenterLine, ''), ( SELECT Ucl_LineCode 
										                                                                                            FROM E_UserCostcenterLineAccess 
																														           WHERE (Ucl_CostCenterCode = Elt_CostCenter OR Ucl_CostCenterCode = 'ALL')
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
                                          OR Elt_EmployeeID = '{0}' )", Session["userLogged"].ToString());
        }
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter =string.Format(@"AND (    ( Elt_ControlNo LIKE '{0}%' )
                                          OR ( Elt_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                          OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 4)) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Elt_LeaveDate,101) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),Elt_AppliedDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Elt_AppliedDate,114),5) LIKE '%{0}%' )
                                          OR ( Ltm_LeaveDesc LIKE '{0}%' )
                                          OR ( ISNULL(AD5.Adt_AccountDesc, '- not applicable -') LIKE '%{0}%')
                                          OR ( LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) LIKE '{0}%' )
                                          OR ( LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) LIKE '{0}%' )
                                          OR ( Elt_LeaveHour LIKE '{0}%' )
                                          OR ( Elt_Reason LIKE '{0}%' )
                                          OR ( CONVERT(varchar(10),Elt_EndorsedDateToChecker,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),Elt_EndorsedDateToChecker,114),5) LIKE '{0}%' )
                                          OR ( Elt_Filler1 LIKE '{0}%' )
                                          OR ( Elt_Filler2 LIKE '{0}%' )
                                          OR ( Elt_Filler3 LIKE '{0}%' )
                                    )", txtSearch.Text.ToString());
           
            string holder = string.Empty;
            string searchKey = txtSearch.Text.Replace("'", "");
            searchKey += "&";
            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                filter += string.Format(searchFilter, holder, !hasCCLine ? "" : string.Format(@" OR ( Elt_CostCenterLine LIKE '{0}%' )", holder));
            }
        }
        return filter;
    }

    private string getFillerName(string fillerName)
    {

        string sql = string.Format(@"  SELECT CASE WHEN ISNULL(Cfl_TextDisplay, Cfl_ColName) = ''
			                                    THEN Cfl_ColName
			                                    ELSE Cfl_TextDisplay
	                                          END
	                                     FROM T_ColumnFiller
                                        WHERE Cfl_ColName = '{0}'", fillerName);
        string flag = string.Empty;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                flag = dal.ExecuteScalar(sql, CommandType.Text).ToString();
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
        TextInfo UsaTextInfo = new CultureInfo("en-US", false).TextInfo;
        return UsaTextInfo.ToTitleCase(flag.ToLower());
    }

    private void setDetailInformation()
    {
        DataSet ds = new DataSet();
        #region Query
        string sql = @"  SELECT Elt_ControlNo [Control No]
                              , Elt_EmployeeId [ID No]
                              , Emt_NickName [ID Code]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                              , Convert(varchar(10), Elt_LeaveDate, 101) [Leave Date]
                              , Ltm_LeaveDesc [Type]
                              , ISNULL(AD5.Adt_AccountDesc, '- not applicable -') [Category]
                              , LEFT(Elt_StartTime,2) 
                                + ':' 
                                + RIGHT(Elt_StartTime,2) [Start]
                              , LEFT(Elt_EndTime,2) 
                                + ':' 
                                + RIGHT(Elt_EndTime,2) [End]
                              , Elt_LeaveHour [Hours]
                              , Elt_DayUnit [Day Unit]
                              , dbo.getCostCenterFullNameV2(Elt_CostCenter) [Cost Center]
                              , Convert(varchar(10), Elt_AppliedDate,101) 
                                + ' ' 
                                + RIGHT(Convert(varchar(17), Elt_AppliedDate,113),5) [Applied Date]
                              , Convert(varchar(10), Elt_EndorsedDateToChecker,101) 
                                + ' ' 
                                + RIGHT(Convert(varchar(17), Elt_EndorsedDateToChecker,113),5) [Endorsed Date]
                              , Elt_Reason [Reason]
                              , AD2.Adt_AccountDesc [Illness  Code]
                              , AD3.Adt_AccountDesc [Elt_Filler02]
                              , AD4.Adt_AccountDesc [Elt_Filler03]
                              , AD1.Adt_AccountDesc [Status]
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
                              , Trm_Remarks [Remarks]
                              , Elt_CurrentPayPeriod [Pay Period]   
                          FROM T_EmployeeLeaveAvailment
                          LEFT JOIN T_LeaveTypeMaster
                            ON Ltm_LeaveType = Elt_LeaveType
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
                            ON AD5.Adt_AccountCode = Elt_LeaveCategory
                           AND AD5.Adt_AccountType = 'LVECATEGRY'
                          LEFT JOIN T_TransactionRemarks 
                            ON Trm_ControlNo = Elt_ControlNo
                         WHERE Elt_ControlNo = @Elt_ControlNo 

                         UNION

                        SELECT Elt_ControlNo [Control No]
                             , Elt_EmployeeId [ID No]
                             , Emt_NickName [ID Code]
                             , Emt_NickName [Nickname]
                             , Emt_Lastname [Lastname]
                             , Emt_Firstname [Firstname]
                              , Convert(varchar(10), Elt_LeaveDate, 101) [Leave Date]
                             , Ltm_LeaveDesc [Type]
                             , ISNULL(AD5.Adt_AccountDesc, '- not applicable -') [Category]
                             , LEFT(Elt_StartTime,2) 
                               + ':' 
                               + RIGHT(Elt_StartTime,2) [Start]
                             , LEFT(Elt_EndTime,2) 
                               + ':' 
                               + RIGHT(Elt_EndTime,2) [End]
                             , Elt_LeaveHour [Hours]
                              , Elt_DayUnit [Day Unit]
                             , dbo.getCostCenterFullNameV2(Elt_CostCenter) [Cost Center]
                             , Convert(varchar(10), Elt_AppliedDate,101) 
                               + ' ' 
                               + RIGHT(Convert(varchar(17), Elt_AppliedDate,113),5) [Applied Date]
                             , Convert(varchar(10), Elt_EndorsedDateToChecker,101) 
                               + ' ' 
                               + RIGHT(Convert(varchar(17), Elt_EndorsedDateToChecker,113),5) [Endorsed Date]
                             , Elt_Reason [Reason]
                             , AD2.Adt_AccountDesc [Illness  Code]
                             , AD3.Adt_AccountDesc [Elt_Filler02]
                             , AD4.Adt_AccountDesc [Elt_Filler03]
                             , AD1.Adt_AccountDesc [Status]
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
                             , Trm_Remarks [Remarks]
                             , Elt_CurrentPayPeriod [Pay Period]   
                          FROM T_EmployeeLeaveAvailmentHist
                          LEFT JOIN T_LeaveTypeMaster
                            ON Ltm_LeaveType = Elt_LeaveType
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
                            ON AD5.Adt_AccountCode = Elt_LeaveCategory
                           AND AD5.Adt_AccountType = 'LVECATEGRY'
                          LEFT JOIN T_TransactionRemarks 
                            ON Trm_ControlNo = Elt_ControlNo
                         WHERE Elt_ControlNo = @Elt_ControlNo 
                        ";
        #endregion
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@Elt_ControlNo", txtControlNo.Text);
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString() + ", " + ds.Tables[0].Rows[0]["Firstname"].ToString();
            txtLeaveDate.Text = ds.Tables[0].Rows[0]["Leave Date"].ToString();
            txtLeaveType.Text = ds.Tables[0].Rows[0]["Type"].ToString();
            txtCategory.Text = ds.Tables[0].Rows[0]["Category"].ToString();
            txtStart.Text = ds.Tables[0].Rows[0]["Start"].ToString();
            txtEnd.Text = ds.Tables[0].Rows[0]["End"].ToString();
            txtHours.Text = ds.Tables[0].Rows[0]["Hours"].ToString();
            txtDayUnit.Text = ds.Tables[0].Rows[0]["Day Unit"].ToString();
            txtReason.Text = ds.Tables[0].Rows[0]["Reason"].ToString();

            txtDateTimeInform.Text = ds.Tables[0].Rows[0]["Applied Date"].ToString();
            txtEndorsedDate.Text = ds.Tables[0].Rows[0]["Endorsed Date"].ToString();
            txtChecker1.Text = ds.Tables[0].Rows[0]["Checker 1"].ToString();
            txtCheckDate1.Text = ds.Tables[0].Rows[0]["Checked Date 1"].ToString();
            txtChecker2.Text = ds.Tables[0].Rows[0]["Checker 2"].ToString();
            txtCheckDate2.Text = ds.Tables[0].Rows[0]["Checked Date 2"].ToString();
            txtApprover.Text = ds.Tables[0].Rows[0]["Approver"].ToString();
            txtApprovedDate.Text = ds.Tables[0].Rows[0]["Approved Date"].ToString();
            txtDetailRemarks.Text = ds.Tables[0].Rows[0]["Remarks"].ToString();
        }
    }
    #endregion
}
