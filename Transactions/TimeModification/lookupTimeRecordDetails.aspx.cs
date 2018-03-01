using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using MethodsLibrary;
using CommonLibrary;

public partial class Transactions_TimeModification_lookupTimeRecordDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        else if (!Page.IsPostBack)
        {
            try
            {
                lblID.Text = "<b>Employee ID:</b> <u>" + Encrypt.decryptText(Request.QueryString["id"].ToString().Replace(" ", "+")) + "</u>";
                lblDate.Text = "<b>Date:</b> <u>" + Encrypt.decryptText(Request.QueryString["dt"].ToString().Replace(" ", "+")) + "</u>";
                getProximityLogs();
                getTransactionLogs();
                getLogTrail();
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
        }
    }

    #region Methods
    private void getTransactionLogs()
    {
        string sql = string.Format(@"   DECLARE @DATE varchar(10)
                                        DECLARE @EMPLOYEEID varchar(15)

                                        SET @DATE = '{1}'
                                        SET @EMPLOYEEID = '{0}'
                                        --OVERTIME
                                        SELECT 
											*
										FROM (
        SELECT 
			'OVERTIME'[Transaction]
             , Convert(varchar(10),Ell_ProcessDate,101)[Process Date]
             , CASE Eot_OvertimeType
               WHEN 'P' THEN 'POST'
               WHEN 'A' THEN 'ADVANCE'
               WHEN 'M' THEN 'MID' 
                END +' ['+LEFT(Eot_StartTime, 2)+':'+RIGHT(Eot_StartTime,2)+' - '+LEFT(Eot_EndTime,2)+':'+RIGHT(Eot_EndTime,2)+']  ['+Convert(varchar(8), Eot_OvertimeHour)+']'[Details]
             , Eot_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Eot_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeOvertime.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeOvertime.Ludatetime,114)
					ELSE
						CASE WHEN Eot_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Eot_ApprovedDate,101)
								+' '+Convert(varchar(8),Eot_ApprovedDate,114)
							ELSE 
								CASE WHEN Eot_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Eot_Checked2Date,101)
									+' '+Convert(varchar(8),Eot_Checked2Date,114)
									ELSE
										CASE WHEN Eot_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Eot_CheckedDate,101)
												+' '+Convert(varchar(8),Eot_CheckedDate,114)
											ELSE
												CASE WHEN Eot_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Eot_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Eot_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeOvertime.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeOvertime.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EMployeeLogLedger
         INNER JOIN T_EmployeeOvertime ON Eot_EmployeeId=Ell_EmployeeId AND Eot_OvertimeDate=Ell_ProcessDate AND Eot_Status IN  @WFSTATUSES
         LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Eot_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
        SELECT 
			'OVERTIME'[Transaction]
             , Convert(varchar(10),Ell_ProcessDate,101)[Process Date]
             , CASE Eot_OvertimeType
               WHEN 'P' THEN 'POST'
               WHEN 'A' THEN 'ADVANCE'
               WHEN 'M' THEN 'MID' 
                END +' ['+LEFT(Eot_StartTime, 2)+':'+RIGHT(Eot_StartTime,2)+' - '+LEFT(Eot_EndTime,2)+':'+RIGHT(Eot_EndTime,2)+']  ['+Convert(varchar(8), Eot_OvertimeHour)+']'[Details]
             , Eot_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Eot_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeOvertime.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeOvertime.Ludatetime,114)
					ELSE
						CASE WHEN Eot_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Eot_ApprovedDate,101)
								+' '+Convert(varchar(8),Eot_ApprovedDate,114)
							ELSE 
								CASE WHEN Eot_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Eot_Checked2Date,101)
									+' '+Convert(varchar(8),Eot_Checked2Date,114)
									ELSE
										CASE WHEN Eot_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Eot_CheckedDate,101)
												+' '+Convert(varchar(8),Eot_CheckedDate,114)
											ELSE
												CASE WHEN Eot_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Eot_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Eot_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeOvertime.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeOvertime.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EMployeeLogLedgerHist
         INNER JOIN T_EmployeeOvertime ON Eot_EmployeeId=Ell_EmployeeId AND Eot_OvertimeDate=Ell_ProcessDate AND Eot_Status IN  @WFSTATUSES
         LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Eot_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
        SELECT 
			'OVERTIME'[Transaction]
             , Convert(varchar(10),Ell_ProcessDate,101)[Process Date]
             , CASE Eot_OvertimeType
               WHEN 'P' THEN 'POST'
               WHEN 'A' THEN 'ADVANCE'
               WHEN 'M' THEN 'MID' 
                END +' ['+LEFT(Eot_StartTime, 2)+':'+RIGHT(Eot_StartTime,2)+' - '+LEFT(Eot_EndTime,2)+':'+RIGHT(Eot_EndTime,2)+']  ['+Convert(varchar(8), Eot_OvertimeHour)+']'[Details]
             , Eot_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Eot_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeOvertimeHist.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeOvertimeHist.Ludatetime,114)
					ELSE
						CASE WHEN Eot_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Eot_ApprovedDate,101)
								+' '+Convert(varchar(8),Eot_ApprovedDate,114)
							ELSE 
								CASE WHEN Eot_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Eot_Checked2Date,101)
									+' '+Convert(varchar(8),Eot_Checked2Date,114)
									ELSE
										CASE WHEN Eot_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Eot_CheckedDate,101)
												+' '+Convert(varchar(8),Eot_CheckedDate,114)
											ELSE
												CASE WHEN Eot_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Eot_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Eot_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeOvertimeHist.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeOvertimeHist.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EMployeeLogLedgerHist
         INNER JOIN T_EmployeeOvertimeHist ON Eot_EmployeeId=Ell_EmployeeId AND Eot_OvertimeDate=Ell_ProcessDate AND Eot_Status IN  @WFSTATUSES
         LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Eot_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
         --LEAVE
         SELECT 
			'LEAVE'[Transaction]
             , Convert(varchar(10), Ell_ProcessDate, 101)[Process Date]
             , '['+Elt_LeaveType+'] '+'['+LEFT(Elt_StartTime,2)+':'+RIGHT(Elt_StartTime,2)+' - '+LEFT(Elt_EndTime,2)+':'+RIGHT(Elt_StartTime,2)+']  ['+Convert(varchar(8),Elt_LeaveHour)+']'[Details]
             , Elt_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Elt_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeLeaveAvailment.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeLeaveAvailment.Ludatetime,114)
					ELSE
						CASE WHEN Elt_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Elt_ApprovedDate,101)
								+' '+Convert(varchar(8),Elt_ApprovedDate,114)
							ELSE 
								CASE WHEN Elt_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Elt_Checked2Date,101)
									+' '+Convert(varchar(8),Elt_Checked2Date,114)
									ELSE
										CASE WHEN Elt_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Elt_CheckedDate,101)
												+' '+Convert(varchar(8),Elt_CheckedDate,114)
											ELSE
												CASE WHEN Elt_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Elt_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Elt_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeLeaveAvailment.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeLeaveAvailment.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EMployeeLogLedger 
         INNER JOIN T_EmployeeLeaveAvailment ON Elt_EmployeeId=Ell_EmployeeId AND Elt_LeaveDate=Ell_ProcessDate AND Elt_Status IN  @WFSTATUSES
          LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Elt_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
        SELECT 
			 'LEAVE'[Transaction]
             , Convert(varchar(10), Ell_ProcessDate, 101)[Process Date]
             , '['+Elt_LeaveType+'] '+'['+LEFT(Elt_StartTime,2)+':'+RIGHT(Elt_StartTime,2)+' - '+LEFT(Elt_EndTime,2)+':'+RIGHT(Elt_StartTime,2)+']  ['+Convert(varchar(8),Elt_LeaveHour)+']'[Details]
             , Elt_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Elt_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeLeaveAvailment.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeLeaveAvailment.Ludatetime,114)
					ELSE
						CASE WHEN Elt_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Elt_ApprovedDate,101)
								+' '+Convert(varchar(8),Elt_ApprovedDate,114)
							ELSE 
								CASE WHEN Elt_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Elt_Checked2Date,101)
									+' '+Convert(varchar(8),Elt_Checked2Date,114)
									ELSE
										CASE WHEN Elt_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Elt_CheckedDate,101)
												+' '+Convert(varchar(8),Elt_CheckedDate,114)
											ELSE
												CASE WHEN Elt_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Elt_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Elt_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeLeaveAvailment.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeLeaveAvailment.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EmployeeLogLedgerHist
         INNER JOIN T_EmployeeLeaveAvailment ON Elt_EmployeeId=Ell_EmployeeId AND Elt_LeaveDate=Ell_ProcessDate AND Elt_Status IN  @WFSTATUSES
          LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Elt_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
        SELECT 
			'LEAVE'[Transaction]
             , Convert(varchar(10), Ell_ProcessDate, 101)[Process Date]
             , '['+Elt_LeaveType+'] '+'['+LEFT(Elt_StartTime,2)+':'+RIGHT(Elt_StartTime,2)+' - '+LEFT(Elt_EndTime,2)+':'+RIGHT(Elt_StartTime,2)+']  ['+Convert(varchar(8),Elt_LeaveHour)+']'[Details]
             , Elt_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Elt_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_EmployeeLeaveAvailmentHist.Ludatetime,101)
							+' '+Convert(varchar(8),T_EmployeeLeaveAvailmentHist.Ludatetime,114)
					ELSE
						CASE WHEN Elt_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Elt_ApprovedDate,101)
								+' '+Convert(varchar(8),Elt_ApprovedDate,114)
							ELSE 
								CASE WHEN Elt_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Elt_Checked2Date,101)
									+' '+Convert(varchar(8),Elt_Checked2Date,114)
									ELSE
										CASE WHEN Elt_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Elt_CheckedDate,101)
												+' '+Convert(varchar(8),Elt_CheckedDate,114)
											ELSE
												CASE WHEN Elt_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Elt_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Elt_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_EmployeeLeaveAvailmentHist.Ludatetime,101)
													+' '+Convert(varchar(8),T_EmployeeLeaveAvailmentHist.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EmployeeLogLedgerHist
         INNER JOIN T_EmployeeLeaveAvailmentHist ON Elt_EmployeeId=Ell_EmployeeId AND Elt_LeaveDate=Ell_ProcessDate AND Elt_Status IN  @WFSTATUSES
          LEFT JOIN T_AccountDetail STAT
			ON Adt_AccountType = 'WFSTATUS'
			AND Adt_AccountCode = Elt_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
         --TIME MODIFICATION
        SELECT 
			'TIME MOD'[Transaction]
             , Convert(varchar(10), Ell_ProcessDate, 101)[Process Date]
             , TIMEREC.Adt_AccountDesc 
             + ' - ['
             + CASE WHEN ISNULL(Trm_ActualTimeIn1, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeIn_1 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeIn_1,2)+':'+RIGHT(Ell_ActualTimeIn_1,2)
                          END
                    ELSE LEFT(Trm_ActualTimeIn1,2)+':'+RIGHT(Trm_ActualTimeIn1,2) 
                END
             + ' - '
             + CASE WHEN ISNULL(Trm_ActualTimeOut1, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeOut_1 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeOut_1,2)+':'+RIGHT(Ell_ActualTimeOut_1,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeOut1,2)+':'+RIGHT(Trm_ActualTimeOut1,2)
                END
             + '  '
             + CASE WHEN ISNULL(Trm_ActualTimeIn2, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeIn_2 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeIn_2,2)+':'+RIGHT(Ell_ActualTimeIn_2,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeIn2,2)+':'+RIGHT(Trm_ActualTimeIn2,2)
                END
             + ' - '
             + CASE WHEN ISNULL(Trm_ActualTimeOut2, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeOut_2 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeOut_2,2)+':'+RIGHT(Ell_ActualTimeOut_2,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeOut2,2)+':'+RIGHT(Trm_ActualTimeOut2,2) 
                END
             + ']' [Details]
             , Trm_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Trm_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_TimeRecMod.Ludatetime,101)
							+' '+Convert(varchar(8),T_TimeRecMod.Ludatetime,114)
					ELSE
						CASE WHEN Trm_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Trm_ApprovedDate,101)
								+' '+Convert(varchar(8),Trm_ApprovedDate,114)
							ELSE 
								CASE WHEN Trm_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Trm_Checked2Date,101)
									+' '+Convert(varchar(8),Trm_Checked2Date,114)
									ELSE
										CASE WHEN Trm_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Trm_CheckedDate,101)
												+' '+Convert(varchar(8),Trm_CheckedDate,114)
											ELSE
												CASE WHEN Trm_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Trm_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Trm_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_TimeRecMod.Ludatetime,101)
													+' '+Convert(varchar(8),T_TimeRecMod.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EmployeeLogLedger
         INNER JOIN T_TimeRecMod
            ON Trm_ModDate = Ell_ProcessDate
           AND Trm_EMployeeId = Ell_EmployeeId 
           AND Trm_Status IN  @WFSTATUSES
          LEFT JOIN T_AccountDetail TIMEREC
            ON TIMEREC.Adt_AccountCode = Trm_Type
           AND TIMEREC.Adt_AccountType = 'TMERECTYPE'
          LEFT JOIN T_AccountDetail STAT
			ON STAT.Adt_AccountType = 'WFSTATUS'
			AND STAT.Adt_AccountCode = Trm_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE
         UNION
        SELECT 
			'TIME MOD'[Transaction]
             , Convert(varchar(10), Ell_ProcessDate, 101)[Process Date]
             , TIMEREC.Adt_AccountDesc 
             + ' - ['
             + CASE WHEN ISNULL(Trm_ActualTimeIn1, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeIn_1 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeIn_1,2)+':'+RIGHT(Ell_ActualTimeIn_1,2)
                          END
                    ELSE LEFT(Trm_ActualTimeIn1,2)+':'+RIGHT(Trm_ActualTimeIn1,2) 
                END
             + ' - '
             + CASE WHEN ISNULL(Trm_ActualTimeOut1, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeOut_1 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeOut_1,2)+':'+RIGHT(Ell_ActualTimeOut_1,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeOut1,2)+':'+RIGHT(Trm_ActualTimeOut1,2)
                END
             + '  '
             + CASE WHEN ISNULL(Trm_ActualTimeIn2, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeIn_2 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeIn_2,2)+':'+RIGHT(Ell_ActualTimeIn_2,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeIn2,2)+':'+RIGHT(Trm_ActualTimeIn2,2)
                END
             + ' - '
             + CASE WHEN ISNULL(Trm_ActualTimeOut2, '') = '' 
                    THEN CASE WHEN Ell_ActualTimeOut_2 = '0000' 
                              THEN '    '
                              ELSE LEFT(Ell_ActualTimeOut_2,2)+':'+RIGHT(Ell_ActualTimeOut_2,2) 
                          END
                    ELSE LEFT(Trm_ActualTimeOut2,2)+':'+RIGHT(Trm_ActualTimeOut2,2) 
                END
             + ']' [Details]
             , Trm_Reason[Reason]
             @ADDITIONALCOLUMNS
             , CASE WHEN Trm_Status IN ('0', '2', '4', '6', '8')
						THEN  Convert(varchar(10),T_TimeRecMod.Ludatetime,101)
							+' '+Convert(varchar(8),T_TimeRecMod.Ludatetime,114)
					ELSE
						CASE WHEN Trm_ApprovedDate IS NOT NULL
								THEN Convert(varchar(10),Trm_ApprovedDate,101)
								+' '+Convert(varchar(8),Trm_ApprovedDate,114)
							ELSE 
								CASE WHEN Trm_Checked2Date IS NOT NULL
										THEN Convert(varchar(10),Trm_Checked2Date,101)
									+' '+Convert(varchar(8),Trm_Checked2Date,114)
									ELSE
										CASE WHEN Trm_CheckedDate IS NOT NULL
												THEN Convert(varchar(10),Trm_CheckedDate,101)
												+' '+Convert(varchar(8),Trm_CheckedDate,114)
											ELSE
												CASE WHEN Trm_EndorsedDateToChecker IS NOT NULL
													THEN Convert(varchar(10),Trm_EndorsedDateToChecker,101)
													+' '+Convert(varchar(8),Trm_EndorsedDateToChecker,114)
												ELSE 
													Convert(varchar(10),T_TimeRecMod.Ludatetime,101)
													+' '+Convert(varchar(8),T_TimeRecMod.Ludatetime,114)
												END
										END
								END
						END
				END [Status Date]
          FROM T_EmployeeLogLedgerHist
         INNER JOIN T_TimeRecMod
            ON Trm_ModDate = Ell_ProcessDate
           AND Trm_EmployeeId = Ell_EmployeeId
           AND Trm_Status IN  @WFSTATUSES 
          LEFT JOIN T_AccountDetail TIMEREC
            ON TIMEREC.Adt_AccountCode = Trm_Type
           AND TIMEREC.Adt_AccountType = 'TMERECTYPE'
          LEFT JOIN T_AccountDetail STAT
			ON STAT.Adt_AccountType = 'WFSTATUS'
			AND STAT.Adt_AccountCode = Trm_Status
         WHERE Ell_EmployeeId=@EMPLOYEEID
           AND Ell_ProcessDate=@DATE

                                        ) AS TABLETEMP
                                           WHERE 1 = 1 ", Encrypt.decryptText(Request.QueryString["id"].ToString().Replace(" ", "+"))
                                                          , Encrypt.decryptText(Request.QueryString["dt"].ToString().Replace(" ", "+")));

        // Denzo retrieves all transactions from 0 - 9
        if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
        {
            sql = sql.Replace("@WFSTATUSES", " ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A') ")
                       .Replace("@ADDITIONALCOLUMNS", @" , STAT.Adt_AccountDesc [Status] ");
        }
        else
        {
            sql = sql.Replace("@WFSTATUSES", " ('9','A') ")
                       .Replace("@ADDITIONALCOLUMNS", @" ");
        }

        #region Filters
        if (this.txtSearch.Text.Trim() != string.Empty)
        {
            string searchFilter = @"
                                AND (
									[Status Date] LIKE '{0}%'
									OR [Transaction] LIKE '{0}%'
									OR [Process Date] LIKE '{0}%'
									OR [Details] LIKE '{0}%'
									OR [Reason] LIKE '{0}%'
                                    ) ";
            if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
            {
                searchFilter = @"
                                AND (
									[Approved Date] LIKE '{0}%'
									OR [Transaction] LIKE '{0}%'
									OR [Process Date] LIKE '{0}%'
									OR [Details] LIKE '{0}%'
									OR [Reason] LIKE '{0}%'
									OR [Status] LIKE '{0}%'
                                    ) ";
            }
            string holder = string.Empty;
            string searchKey = txtSearch.Text.Replace("'", "");
            searchKey += "&";
            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                sql += string.Format(searchFilter, holder);
            }
        }
        #endregion

        sql += @" ORDER BY [Transaction] ASC, [Details] ASC ";

        if (!Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
        {
            sql = sql.Replace("[Status Date]", "[Approved Date]");
        }        

        DataSet ds = new DataSet();
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
            dgvResult.DataSource = ds;
            dgvResult.DataBind();
        }
        else
        {
            dgvResult.DataSource = null;
            dgvResult.DataBind();
        }
    }

    private void getProximityLogs()
    {
        string sql = string.Format(@"   SELECT LEFT(Plt_LogTime,2) +':'+ RIGHT(Plt_LogTime,2) [Time]
                                          FROM E_ProximityLogs
                                         WHERE Plt_EmployeeId = '{0}'
                                           AND Convert(varchar(10), Plt_LogDate, 101) = '{1}'
                                         ORDER BY 1 ASC", Encrypt.decryptText(Request.QueryString["id"].ToString().Replace(" ", "+"))
                                                        , Encrypt.decryptText(Request.QueryString["dt"].ToString().Replace(" ", "+")));
        DataSet ds = new DataSet();
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
            dgvProximity.DataSource = ds;
            dgvProximity.DataBind();
        }
    }

    private void getLogTrail()
    {
        string sql = string.Format(@"   SELECT Elt_Seqno [Seq No]
	                                         , '[' + Elt_ShiftCode + '] '
	                                         + LEFT(Scm_ShiftTimeIn, 2)+':'+RIGHT(Scm_ShiftTimeIn, 2)
	                                         + ' - '
	                                         + LEFT(Scm_ShiftBreakStart, 2)+':'+RIGHT(Scm_ShiftBreakStart, 2)
	                                         + ' '
	                                         + LEFT(Scm_ShiftBreakEnd, 2)+':'+RIGHT(Scm_ShiftBreakEnd, 2)
	                                         + ' - '
	                                         + LEFT(Scm_ShiftTimeOut, 2)+':'+RIGHT(Scm_ShiftTimeOut, 2) [Shift]
	                                         , CASE WHEN Elt_ActualTimeIn_1 = '0000'
			                                        THEN '-'
			                                        ELSE LEFT(Elt_ActualTimeIn_1, 2)+':'+RIGHT(Elt_ActualTimeIn_1, 2) 
                                                END [Time IN 1]
			
	                                         , CASE WHEN Elt_ActualTimeOut_1 = '0000'
			                                        THEN '-'
			                                        ELSE LEFT(Elt_ActualTimeOut_1, 2)+':'+RIGHT(Elt_ActualTimeOut_1, 2)
                                                END  [Time Out 1]
	                                         , CASE WHEN Elt_ActualTimeIn_2 = '0000'
			                                        THEN '-'
			                                        ELSE LEFT(Elt_ActualTimeIn_2, 2)+':'+RIGHT(Elt_ActualTimeIn_2, 2)
                                                END  [Time IN 2]
	                                         , CASE WHEN Elt_ActualTimeOut_2 = '0000'
			                                        THEN '-'
			                                        ELSE LEFT(Elt_ActualTimeOut_2, 2)+':'+RIGHT(Elt_ActualTimeOut_2, 2) 
                                                END [Time Out 2]
                                              , CONVERT(varchar(10), T_EmployeeLogTrail.Ludatetime, 101)
                                              + ' '
                                              + CONVERT(varchar(8), T_EmployeeLogTrail.Ludatetime, 114) [Datetime Updated]
                                          FROM T_EmployeeLogTrail
                                          LEFT JOIN T_ShiftCodeMaster
                                            ON Scm_ShiftCode = Elt_ShiftCode
                                         WHERE Elt_EmployeeId = '{0}'
                                           AND Elt_ProcessDate = '{1}' ", Encrypt.decryptText(Request.QueryString["id"].ToString().Replace(" ", "+"))
                                                                        , Encrypt.decryptText(Request.QueryString["dt"].ToString().Replace(" ", "+")));
        DataSet ds = new DataSet();
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
            dgvLogTrail.DataSource = ds;
            dgvLogTrail.DataBind();
        }
    }

    #endregion

    #region Events
    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'; this.style.color='blue';this.style.fontWeight='normal'";
            e.Row.Attributes["onmouseout"] = ";this.style.color='black';this.style.fontWeight='normal';";
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'";
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        getTransactionLogs();
        this.txtSearch.Focus();
    }
    #endregion
}

