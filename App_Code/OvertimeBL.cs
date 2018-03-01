using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for OvertimeBL
/// </summary>
namespace Payroll.DAL
{
    public class OvertimeBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public OvertimeBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DataRow getOvertimeInfo(string controlNo, DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Eot_CurrentPayPeriod
                                  , Eot_EmployeeId
                                  , Eot_OvertimeDate
                                  , Eot_Seqno
                                  , Eot_AppliedDate
                                  , Eot_OvertimeType
                                  , Eot_StartTime
                                  , Eot_EndTime
                                  , Eot_OvertimeHour
                                  , Eot_Reason
                                  , Eot_JobCode
                                  , Eot_ClientJobNo
                                  , Eot_EndorsedDateToChecker
                                  , Eot_CheckedBy
                                  , Eot_CheckedDate
                                  , Eot_Checked2By
                                  , Eot_Checked2Date
                                  , Eot_ApprovedBy
                                  , Eot_ApprovedDate
                                  , Eot_Status
                                  , Eot_ControlNo
                                  , Eot_OvertimeFlag
                                  , Eot_Costcenter
                                  , Eot_Filler1
                                  , Eot_Filler2
                                  , Eot_Filler3
                                  , Eot_BatchNo
                                  , Usr_Login
                                  , Ludatetime
                               FROM T_EmployeeOvertime
                              WHERE Eot_ControlNo = @ControlNo
UNION
SELECT Eot_CurrentPayPeriod
                                  , Eot_EmployeeId
                                  , Eot_OvertimeDate
                                  , Eot_Seqno
                                  , Eot_AppliedDate
                                  , Eot_OvertimeType
                                  , Eot_StartTime
                                  , Eot_EndTime
                                  , Eot_OvertimeHour
                                  , Eot_Reason
                                  , Eot_JobCode
                                  , Eot_ClientJobNo
                                  , Eot_EndorsedDateToChecker
                                  , Eot_CheckedBy
                                  , Eot_CheckedDate
                                  , Eot_Checked2By
                                  , Eot_Checked2Date
                                  , Eot_ApprovedBy
                                  , Eot_ApprovedDate
                                  , Eot_Status
                                  , Eot_ControlNo
                                  , Eot_OvertimeFlag
                                  , Eot_Costcenter
                                  , Eot_Filler1
                                  , Eot_Filler2
                                  , Eot_Filler3
                                  , Eot_BatchNo
                                  , Usr_Login
                                  , Ludatetime
                               FROM T_EmployeeOvertimeHist
                              WHERE Eot_ControlNo = @ControlNo";
            return dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0].Rows[0];
        }

        public DataRow getOvertimeInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            param[1] = new ParameterInfo("@DEFAULTSHIFT", Resources.Resource.DEFAULTSHIFT);
            string sql = @"  SELECT Eot_ControlNo [Control No]
                                  , Eot_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Eot_OvertimeDate [OT Date]
                                  , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                                  , LEFT(Eot_StartTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_StartTime,2) [Start]
                                  , LEFT(Eot_EndTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_EndTime,2) [End]
                                  , Eot_OvertimeHour [Hours]
                                  , Eot_Reason [Reason]
                                  , Eot_JobCode [Job Code]
                                  , Eot_ClientJobNo [Client Job No]
                                  , Slm_ClientJobName [Client Job Name]
                                  , AD2.Adt_AccountCode [@Eot_Filler1Desc]
                                  , AD3.Adt_AccountDesc [@Eot_Filler2Desc]
                                  , AD4.Adt_AccountDesc [@Eot_Filler3Desc]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
                                  , CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                         THEN LedgerHist.Ell_DayCode
                                         ELSE Ledger.Ell_DayCode 
                                     END [Day Code]
							      , Scm_ShiftCode [Shift Code]
							      , Scm_ShiftTimeIn [Shift Time In]
							      , Scm_ShiftBreakStart [Break Start]
							      , Scm_ShiftBreakEnd [Break End]
							      , Scm_ShiftTimeOut [Shift Time Out]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_EmployeeOvertime
                               LEFT JOIN T_EmployeeLogLedger Ledger
                                 ON Ledger.Ell_EmployeeId = Eot_EmployeeId
                                AND Ledger.Ell_ProcessDate = Eot_OvertimeDate
                               LEFT JOIN T_EmployeeLogLedgerHist LedgerHist
                                 ON LedgerHist.Ell_EmployeeId = Eot_EmployeeId
                                AND LedgerHist.Ell_ProcessDate = Eot_OvertimeDate
                               LEFT JOIN T_EmployeeMaster 
                                 ON Emt_EmployeeId = Eot_EmployeeId
                               LEFT JOIN T_ShiftCodeMaster
                                 ON Scm_ShiftCode = CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                                         THEN ISNULL(LedgerHist.Ell_ShiftCode, ISNULL(Emt_ShiftCode, @DEFAULTSHIFT))
                                                         ELSE Ledger.Ell_ShiftCode
                                                     END
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
                               LEFT JOIN T_SalesMaster 
                                 ON Eot_JobCode = Slm_DashJobCode 
                                AND Eot_ClientJobNo = Slm_ClientJobNo
                               LEFT JOIN T_PayPeriodMaster 
                                 ON Ppm_PayPeriod = Eot_CurrentPayPeriod
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Eot_ControlNo
                              WHERE Eot_ControlNo = @ControlNo";

            if (Convert.ToBoolean(Resources.Resource.USELOVERTIMEDEFAULTSHIFT))
            {
                #region SQL with USELOVERTIMEDEFAULTSHIFT
                sql = @"     SELECT Eot_ControlNo [Control No]
                                  , Eot_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Eot_OvertimeDate [OT Date]
                                  , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                                  , LEFT(Eot_StartTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_StartTime,2) [Start]
                                  , LEFT(Eot_EndTime,2) 
                                    + ':' 
                                    + RIGHT(Eot_EndTime,2) [End]
                                  , Eot_OvertimeHour [Hours]
                                  , Eot_Reason [Reason]
                                  , Eot_JobCode [Job Code]
                                  , Eot_ClientJobNo [Client Job No]
                                  , Slm_ClientJobName [Client Job Name]
                                  , AD2.Adt_AccountDesc [@Eot_Filler1Desc]
                                  , AD3.Adt_AccountDesc [@Eot_Filler2Desc]
                                  , AD4.Adt_AccountDesc [@Eot_Filler3Desc]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
                                  , CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                         THEN LedgerHist.Ell_DayCode
                                         ELSE Ledger.Ell_DayCode 
                                     END [Day Code]
							      , FNLSHIFT.Scm_ShiftCode [Shift Code]
							      , FNLSHIFT.Scm_ShiftTimeIn [Shift Time In]
							      , FNLSHIFT.Scm_ShiftBreakStart [Break Start]
							      , FNLSHIFT.Scm_ShiftBreakEnd [Break End]
							      , FNLSHIFT.Scm_ShiftTimeOut [Shift Time Out]
                               FROM T_EmployeeOvertime
                               LEFT JOIN T_EmployeeLogLedger Ledger
                                 ON Ledger.Ell_EmployeeId = Eot_EmployeeId
                                AND Ledger.Ell_ProcessDate = Eot_OvertimeDate
                               LEFT JOIN T_EmployeeLogLedgerHist LedgerHist
                                 ON LedgerHist.Ell_EmployeeId = Eot_EmployeeId
                                AND LedgerHist.Ell_ProcessDate = Eot_OvertimeDate
                               LEFT JOIN T_EmployeeMaster 
                                 ON Emt_EmployeeId = Eot_EmployeeId
                               LEFT JOIN T_ShiftCodeMaster EMTSHIFT
                                 ON EMTSHIFT.Scm_ShiftCode = Emt_ShiftCode
                               LEFT JOIN T_ShiftCodeMaster DEFSHIFT
                                 ON DEFSHIFT.Scm_ScheduleType = EMTSHIFT.Scm_ScheduleType
                                AND DEFSHIFT.Scm_DefaultShift = 'True'
                                AND DEFSHIFT.Scm_Status = 'A'
                               LEFT JOIN T_ShiftCodeMaster FNLSHIFT
                                 ON FNLSHIFT.Scm_ShiftCode = CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                                                  THEN ISNULL(LedgerHist.Ell_ShiftCode, ISNULL(DEFSHIFT.Scm_ShiftCode, @DEFAULTSHIFT))
                                                                  ELSE Ledger.Ell_ShiftCode
                                                              END
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
                               LEFT JOIN T_SalesMaster 
                                 ON Eot_JobCode = Slm_DashJobCode 
                                AND Eot_ClientJobNo = Slm_ClientJobNo
                               LEFT JOIN T_PayPeriodMaster 
                                 ON Ppm_PayPeriod = Eot_CurrentPayPeriod
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Eot_ControlNo
                              WHERE Eot_ControlNo = @ControlNo";
                #endregion
            }
            sql = sql.Replace("@Eot_Filler1Desc", CommonMethods.getFillerName("Eot_Filler01"));
            sql = sql.Replace("@Eot_Filler2Desc", CommonMethods.getFillerName("Eot_Filler02"));
            sql = sql.Replace("@Eot_Filler3Desc", CommonMethods.getFillerName("Eot_Filler03"));
            DataSet dsRecord = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRecord = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsRecord.Tables[0].Rows[0];
        }

        private string GetLedgerTableFromDate(string Date)
        {
            string LedgerTable = "T_EmployeeLogLedger";//Default
            DateTime start = CommonMethods.getQuincenaDate('C', "START");
            DateTime end = CommonMethods.getQuincenaDate('F', "END");
            string period = CommonMethods.getPayPeriod(Convert.ToDateTime(Date));

            if (Convert.ToDateTime(Date) <= Convert.ToDateTime(end) || period.Equals(CommonMethods.getPayPeriod("F")))
            {
                if (Convert.ToDateTime(Date) < Convert.ToDateTime(start))
                {
                    LedgerTable = "T_EmployeeLogLedgerHist";
                }
                else
                {
                    LedgerTable = "T_EmployeeLogLedger";
                }
            }
            return LedgerTable;
        }

        public string GetOTSequence(string OvertimeDate, string EmployeeID)
        {
            string OTSequenceNum = string.Empty;

            #region [Parameters]
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Eot_OvertimeDate", OvertimeDate);
            paramInfo[1] = new ParameterInfo("@Eot_EmployeeID", EmployeeID);
            #endregion

            #region [SQL Query]
            string sql = @" SELECT MAX(seqno)
                              FROM ( SELECT REPLICATE('0', 2 - LEN(ISNULL(max(eot_seqno), 0) + 1)) + RTRIM(isnull(max(eot_seqno), 0) + 1) [seqno]
                                       FROM T_EmployeeOvertime
                                      WHERE Eot_OvertimeDate = @Eot_OvertimeDate
                                        AND Eot_EmployeeID = @Eot_EmployeeID
                                      UNION
                                     SELECT REPLICATE('0', 2 - LEN(ISNULL(max(eot_seqno), 0) + 1)) + RTRIM(ISNULL(MAX(Eot_SeqNo), 0) + 1) [seqno]
                                       FROM T_EmployeeOvertimeHist
                                      WHERE Eot_OvertimeDate = @Eot_OvertimeDate
                                        AND Eot_EmployeeID = @Eot_EmployeeID) as maxseqno";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    OTSequenceNum = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo).Tables[0].Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return OTSequenceNum;
        }

        public string ComputeOTFlag(string ProcessDate)
        {
            DataSet ds = new DataSet();
            #region SQL Query
            string sqlQuery = string.Format(@" SELECT CASE WHEN '{0}' < Ppm_StartCycle
			                                               THEN 'P'
			                                               WHEN '{0}' <= Ppm_EndCycle
			                                               THEN 'C'
			                                               ELSE 'F'
	                                                   END 
                                                 FROM T_PayPeriodMaster
                                                WHERE Ppm_CycleIndicator = 'C'", ProcessDate);
            
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                sqlQuery = string.Format(@" SELECT CASE WHEN '{0}' < Ppm_StartCycle
			                                            THEN 'P'
			                                            WHEN '{0}' <= Ppm_EndCycle
			                                            THEN 'H'
			                                            ELSE 'F'
	                                                END 
                                              FROM T_PayPeriodMaster
                                             WHERE Ppm_CycleIndicator = 'C'", ProcessDate);
            }
            #endregion
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }
       
        public DataSet getOvertimeTransactionWithDetail(string idNumber, string fromMMDDYYY, string toMMDDYYYY, string type, string hours)
        {
            DataSet ds = new DataSet();
            #region Query
            string sql = @" SELECT Convert(varchar(10), Ell_ProcessDate, 101) [OT Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , Ell_ShiftCode [Shift Code]
                                 , '[' + Scm_ShiftCode + '] '
                                 + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc] 
                                 , CASE WHEN (@type = 'P')
                                        THEN CASE WHEN (Ell_DayCode = 'REG')
				                                  THEN LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2)
				                                  ELSE LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
			                                  END
                                        ELSE CASE WHEN (@type = 'A') 
                                                  THEN dbo.computeOTTime(Scm_ShiftTimeIn, (Convert(decimal(7,2),@hours)*-1), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
                                                  ELSE LEFT(Scm_ShiftBreakStart,2) +':'+ RIGHT(Scm_ShiftBreakStart, 2)
                                              END
                                    END [Start]
                                 , CASE WHEN (@type = 'P')
		                                THEN CASE WHEN (Ell_DayCode = 'REG')
		 		                                  THEN dbo.computeOTTime(Scm_ShiftTimeOut, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
				                                  ELSE dbo.computeOTTime(Scm_ShiftTimeIn, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
			                                  END
		                                ELSE CASE WHEN (@type = 'A') 
                                                  THEN LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
                                                  ELSE dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0)
                                                  --ELSE LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2)
                                              END
	                                 END [End]
                                 , Convert(decimal(7,2),@hours) [Hours]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                               AND Ell_EmployeeId = @employeeId
                               AND 1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               --AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType 
								--										                                     FROM T_EmployeeOvertime
								--										                                    WHERE Eot_EmployeeId = @employeeId
								--										                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--										                                      AND Eot_Status IN ('1','3','5','7','9')
								--										                                    UNION
								--										                                   SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--										                                     FROM T_EmployeeOvertimeHist
								--										                                    WHERE Eot_EmployeeId = @employeeId
								--										                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--										                                      AND Eot_Status IN ('1','3','5','7','9'))

                             UNION

                            SELECT Convert(varchar(10), Ell_ProcessDate, 101) [OT Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , Ell_ShiftCode [Shift Code]
                                 , '[' + Scm_ShiftCode + '] '
                                 + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc] 
                                 , CASE WHEN (@type = 'P')
                                        THEN CASE WHEN (Ell_DayCode = 'REG')
				                                  THEN LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2)
				                                  ELSE LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
			                                  END
                                        ELSE CASE WHEN (@type = 'A') 
                                                  THEN dbo.computeOTTime(Scm_ShiftTimeIn, (Convert(decimal(7,2),@hours)*-1), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
                                                  ELSE LEFT(Scm_ShiftBreakStart,2) +':'+ RIGHT(Scm_ShiftBreakStart, 2)
                                              END
                                    END [Start]
                                 , CASE WHEN (@type = 'P')
		                                THEN CASE WHEN (Ell_DayCode = 'REG')
		 		                                  THEN dbo.computeOTTime(Scm_ShiftTimeOut, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
				                                  ELSE dbo.computeOTTime(Scm_ShiftTimeIn, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakEnd, Scm_PaidBreak)
			                                  END
		                                ELSE CASE WHEN (@type = 'A') 
                                                  THEN LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2)
                                                  ELSE dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0)
                                                  --ELSE LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2)
                                              END
	                                 END [End]
                                 , Convert(decimal(7,2),@hours) [Hours]
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                               AND Ell_EmployeeId = @employeeId
                               AND 1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               --AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--									                                         FROM T_EmployeeOvertime
								--									                                        WHERE Eot_EmployeeId = @employeeId
								--									                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--									                                          AND Eot_Status IN ('1','3','5','7','9')
								--									                                        UNION
								--									                                       SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
								--									                                         FROM T_EmployeeOvertimeHist
								--									                                        WHERE Eot_EmployeeId = @employeeId
								--									                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
								--									                                          AND Eot_Status IN ('1','3','5','7','9'))";
            #endregion
            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@employeeId", idNumber);
            param[1] = new ParameterInfo("@fromDate", fromMMDDYYY);
            param[2] = new ParameterInfo("@toDate", toMMDDYYYY);
            param[3] = new ParameterInfo("@type", type);
            param[4] = new ParameterInfo("@hours", hours);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        //Saving Function
        public void CreateOTRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[20];
            paramDetails[0] = new ParameterInfo("@Eot_CurrentPayPeriod", rowDetails["Eot_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Eot_EmployeeId", rowDetails["Eot_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Eot_OvertimeDate", rowDetails["Eot_OvertimeDate"]);
            paramDetails[3] = new ParameterInfo("@Eot_Seqno", rowDetails["Eot_Seqno"]);
            paramDetails[4] = new ParameterInfo("@Eot_OvertimeType", rowDetails["Eot_OvertimeType"]);
            paramDetails[5] = new ParameterInfo("@Eot_StartTime", rowDetails["Eot_StartTime"]);
            paramDetails[6] = new ParameterInfo("@Eot_EndTime", rowDetails["Eot_EndTime"]);
            paramDetails[7] = new ParameterInfo("@Eot_OvertimeHour", rowDetails["Eot_OvertimeHour"]);
            paramDetails[8] = new ParameterInfo("@Eot_Reason", rowDetails["Eot_Reason"]);
            paramDetails[9] = new ParameterInfo("@Eot_JobCode", rowDetails["Eot_JobCode"]);
            paramDetails[10] = new ParameterInfo("@Eot_ClientJobNo", rowDetails["Eot_ClientJobNo"]);
            paramDetails[11] = new ParameterInfo("@Eot_Status", rowDetails["Eot_Status"]);
            paramDetails[12] = new ParameterInfo("@Eot_ControlNo", rowDetails["Eot_ControlNo"]);
            paramDetails[13] = new ParameterInfo("@Eot_OvertimeFlag", rowDetails["Eot_OvertimeFlag"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[15] = new ParameterInfo("@Eot_Filler1", rowDetails["Eot_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Eot_Filler2", rowDetails["Eot_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Eot_Filler3", rowDetails["Eot_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Eot_BatchNo", rowDetails["Eot_BatchNo"]);
            paramDetails[19] = new ParameterInfo("@Eot_EndorsedDateToChecker", rowDetails["Eot_EndorsedDateToChecker"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for filler insertion
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Eot_EmployeeId
                                                      AND @Eot_OvertimeDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Eot_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Eot_EmployeeId
                                                    AND @Eot_OvertimeDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END
                                
                                DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                               FROM T_ProcessControlMaster
                                                              WHERE Pcm_SystemID = 'OVERTIME'
                                                                AND Pcm_ProcessID = 'AUTOROUTE'
                                                                AND Pcm_Status = 'A' )
                                DECLARE @RouteCode as char(4)
                                SELECT @RouteCode  = Ert_RouteCode 
                                  FROM T_AllowanceHeader
                                  LEFT JOIN T_EmployeeRoute
                                    ON Ert_LedgerAlwCol = Alh_LedgerAlwCol
                                 WHERE Alh_LedgerAlwCol = (SELECT Pmt_CharValue
                                                             FROM T_ParameterMaster
                                                            WHERE Pmt_ParameterID = 'ARALWCOL' )
                                   AND Alh_ExtensionTable = 'T'
                                   AND Ert_EmployeeID = @Eot_EmployeeId
                                   AND @Eot_OvertimeDate BETWEEN Ert_EffectivityDate AND ISNULL(Ert_EndDate, DATEADD(month, 5, GETDATE()))

                                --select only for testing
                                --SELECT ISNULL(@RouteCode, '')
IF NOT EXISTS(SELECT Eot_ControlNo FROM T_EmployeeOvertime
WHERE Eot_CurrentPayPeriod = @Eot_CurrentPayPeriod
AND Eot_EmployeeId = @Eot_EmployeeId
AND Eot_OvertimeDate = @Eot_OvertimeDate
AND Eot_OvertimeType = @Eot_OvertimeType
AND Eot_StartTime = @Eot_StartTime
AND Eot_EndTime = @Eot_EndTime
AND Eot_OvertimeHour = @Eot_OvertimeHour
AND Eot_Reason = @Eot_Reason
AND Eot_JobCode = @Eot_JobCode
AND Eot_ClientJobNo = @Eot_ClientJobNo
AND Eot_Status = @Eot_Status
AND Eot_OvertimeFlag = @Eot_OvertimeFlag
AND Eot_Filler1 = @Eot_Filler1
AND Eot_Filler2 = @Eot_Filler2
AND Eot_Filler3 = @Eot_Filler3)
BEGIN
                                INSERT INTO T_EmployeeOvertime
                                (
                                      Eot_CurrentPayPeriod
                                    , Eot_EmployeeId
                                    , Eot_OvertimeDate
                                    , Eot_Seqno
                                    , Eot_AppliedDate
                                    , Eot_OvertimeType
                                    , Eot_StartTime
                                    , Eot_EndTime
                                    , Eot_OvertimeHour
                                    , Eot_Reason
                                    , Eot_JobCode
                                    , Eot_ClientJobNo
                                    , Eot_Status
                                    , Eot_ControlNo
                                    , Eot_OvertimeFlag
                                    , Eot_Costcenter
                                    , Eot_CostcenterLine --apsungahid added 20141124
                                    , Usr_Login
                                    , Ludatetime
                                    , Eot_Filler1
                                    , Eot_Filler2
                                    , Eot_Filler3
                                    , Eot_BatchNo
                                    , Eot_EndorsedDateToChecker
                                )
                                VALUES
                                (
                                      @Eot_CurrentPayPeriod
                                    , @Eot_EmployeeId
                                    , @Eot_OvertimeDate
                                    , @Eot_Seqno
                                    , getdate()
                                    , @Eot_OvertimeType
                                    , @Eot_StartTime
                                    , @Eot_EndTime
                                    , @Eot_OvertimeHour
                                    , @Eot_Reason
                                    , @Eot_JobCode
                                    , @Eot_ClientJobNo
                                    , @Eot_Status
                                    , @Eot_ControlNo
                                    , @Eot_OvertimeFlag
                                    , @Costcenter
                                    , @LineCode
                                    , @Usr_Login
                                    , getdate()
                                    , IIF(ISNULL(@AUTOROUTE, 0) = 1 , ISNULL(@RouteCode, ''), @Eot_Filler1)
                                    , @Eot_Filler2
                                    , @Eot_Filler3
                                    , @Eot_BatchNo
                                    , @Eot_EndorsedDateToChecker
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void CreateEndorseOTRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[20];
            paramDetails[0] = new ParameterInfo("@Eot_CurrentPayPeriod", rowDetails["Eot_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Eot_EmployeeId", rowDetails["Eot_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Eot_OvertimeDate", rowDetails["Eot_OvertimeDate"]);
            paramDetails[3] = new ParameterInfo("@Eot_Seqno", rowDetails["Eot_Seqno"]);
            paramDetails[4] = new ParameterInfo("@Eot_OvertimeType", rowDetails["Eot_OvertimeType"]);
            paramDetails[5] = new ParameterInfo("@Eot_StartTime", rowDetails["Eot_StartTime"]);
            paramDetails[6] = new ParameterInfo("@Eot_EndTime", rowDetails["Eot_EndTime"]);
            paramDetails[7] = new ParameterInfo("@Eot_OvertimeHour", rowDetails["Eot_OvertimeHour"]);
            paramDetails[8] = new ParameterInfo("@Eot_Reason", rowDetails["Eot_Reason"]);
            paramDetails[9] = new ParameterInfo("@Eot_JobCode", rowDetails["Eot_JobCode"]);
            paramDetails[10] = new ParameterInfo("@Eot_ClientJobNo", rowDetails["Eot_ClientJobNo"]);
            paramDetails[11] = new ParameterInfo("@Eot_Status", rowDetails["Eot_Status"]);
            paramDetails[12] = new ParameterInfo("@Eot_ControlNo", rowDetails["Eot_ControlNo"]);
            paramDetails[13] = new ParameterInfo("@Eot_OvertimeFlag", rowDetails["Eot_OvertimeFlag"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[15] = new ParameterInfo("@Eot_Filler1", rowDetails["Eot_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Eot_Filler2", rowDetails["Eot_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Eot_Filler3", rowDetails["Eot_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Eot_BatchNo", rowDetails["Eot_BatchNo"]);
            paramDetails[19] = new ParameterInfo("@Eot_EndorsedDateToChecker", rowDetails["Eot_EndorsedDateToChecker"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for filler insertion
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT Emt_CostcenterCode FROM T_EmployeeMaster
				                                   WHERE Emt_EmployeeID = @Eot_EmployeeId)

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE @Eot_OvertimeDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END

                                DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                               FROM T_ProcessControlMaster
                                                              WHERE Pcm_SystemID = 'OVERTIME'
                                                                AND Pcm_ProcessID = 'AUTOROUTE'
                                                                AND Pcm_Status = 'A' )
                                DECLARE @RouteCode as char(4)
                                SELECT @RouteCode  = Ert_RouteCode 
                                  FROM T_AllowanceHeader
                                  LEFT JOIN T_EmployeeRoute
                                    ON Ert_LedgerAlwCol = Alh_LedgerAlwCol
                                 WHERE Alh_LedgerAlwCol = (SELECT Pmt_CharValue
                                                             FROM T_ParameterMaster
                                                            WHERE Pmt_ParameterID = 'ARALWCOL' )
                                   AND Alh_ExtensionTable = 'T'
                                   AND Ert_EmployeeID = @Eot_EmployeeId
                                   AND @Eot_OvertimeDate BETWEEN Ert_EffectivityDate AND ISNULL(Ert_EndDate, DATEADD(month, 5, GETDATE()))

                                --select only for testing
                                --SELECT ISNULL(@RouteCode, '')
IF NOT EXISTS(SELECT Eot_ControlNo FROM T_EmployeeOvertime
WHERE Eot_CurrentPayPeriod = @Eot_CurrentPayPeriod
AND Eot_EmployeeId = @Eot_EmployeeId
AND Eot_OvertimeDate = @Eot_OvertimeDate
AND Eot_OvertimeType = @Eot_OvertimeType
AND Eot_StartTime = @Eot_StartTime
AND Eot_EndTime = @Eot_EndTime
AND Eot_OvertimeHour = @Eot_OvertimeHour
AND Eot_Reason = @Eot_Reason
AND Eot_JobCode = @Eot_JobCode
AND Eot_ClientJobNo = @Eot_ClientJobNo
AND Eot_Status = @Eot_Status
AND Eot_OvertimeFlag = @Eot_OvertimeFlag
AND Eot_Filler1 = @Eot_Filler1
AND Eot_Filler2 = @Eot_Filler2
AND Eot_Filler3 = @Eot_Filler3)
BEGIN
                                INSERT INTO T_EmployeeOvertime
                                (
                                      Eot_CurrentPayPeriod
                                    , Eot_EmployeeId
                                    , Eot_OvertimeDate
                                    , Eot_Seqno
                                    , Eot_AppliedDate
                                    , Eot_OvertimeType
                                    , Eot_StartTime
                                    , Eot_EndTime
                                    , Eot_OvertimeHour
                                    , Eot_Reason
                                    , Eot_JobCode
                                    , Eot_ClientJobNo
                                    , Eot_Status
                                    , Eot_ControlNo
                                    , Eot_OvertimeFlag
                                    , Eot_Costcenter
                                    , Eot_CostcenterLine
                                    , Usr_Login
                                    , Ludatetime
                                    , Eot_Filler1
                                    , Eot_Filler2
                                    , Eot_Filler3
                                    , Eot_BatchNo
                                    , Eot_EndorsedDateToChecker
                                )
                                VALUES
                                (
                                      @Eot_CurrentPayPeriod
                                    , @Eot_EmployeeId
                                    , @Eot_OvertimeDate
                                    , @Eot_Seqno
                                    , getdate()
                                    , @Eot_OvertimeType
                                    , @Eot_StartTime
                                    , @Eot_EndTime
                                    , @Eot_OvertimeHour
                                    , @Eot_Reason
                                    , @Eot_JobCode
                                    , @Eot_ClientJobNo
                                    , @Eot_Status
                                    , @Eot_ControlNo
                                    , @Eot_OvertimeFlag
                                    , @Costcenter
                                    , @LineCode
                                    , @Usr_Login
                                    , getdate()
                                    , IIF(ISNULL(@AUTOROUTE, 0) = 1 , ISNULL(@RouteCode, ''), @Eot_Filler1)
                                    , @Eot_Filler2
                                    , @Eot_Filler3
                                    , @Eot_BatchNo
                                    , GETDATE()
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateOTRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[18];
            paramDetails[0] = new ParameterInfo("@Eot_CurrentPayPeriod", rowDetails["Eot_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Eot_EmployeeId", rowDetails["Eot_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Eot_OvertimeDate", rowDetails["Eot_OvertimeDate"]);
            paramDetails[3] = new ParameterInfo("@Eot_Seqno", rowDetails["Eot_Seqno"]);
            paramDetails[4] = new ParameterInfo("@Eot_OvertimeType", rowDetails["Eot_OvertimeType"]);
            paramDetails[5] = new ParameterInfo("@Eot_StartTime", rowDetails["Eot_StartTime"]);
            paramDetails[6] = new ParameterInfo("@Eot_EndTime", rowDetails["Eot_EndTime"]);
            paramDetails[7] = new ParameterInfo("@Eot_OvertimeHour", rowDetails["Eot_OvertimeHour"]);
            paramDetails[8] = new ParameterInfo("@Eot_Reason", rowDetails["Eot_Reason"]);
            paramDetails[9] = new ParameterInfo("@Eot_JobCode", rowDetails["Eot_JobCode"]);
            paramDetails[10] = new ParameterInfo("@Eot_ClientJobNo", rowDetails["Eot_ClientJobNo"]);
            paramDetails[11] = new ParameterInfo("@Eot_Status", rowDetails["Eot_Status"]);
            paramDetails[12] = new ParameterInfo("@Eot_ControlNo", rowDetails["Eot_ControlNo"]);
            paramDetails[13] = new ParameterInfo("@Eot_OvertimeFlag", rowDetails["Eot_OvertimeFlag"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[15] = new ParameterInfo("@Eot_Filler1", rowDetails["Eot_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Eot_Filler2", rowDetails["Eot_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Eot_Filler3", rowDetails["Eot_Filler3"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for Eot_Filler
            string sqlInsert = @"DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                FROM T_ProcessControlMaster
                                WHERE Pcm_SystemID = 'OVERTIME'
                                AND Pcm_ProcessID = 'AUTOROUTE'
                                AND Pcm_Status = 'A' )

                                DECLARE @RouteCode as char(4)
                                SELECT @RouteCode  = Ert_RouteCode 
                                  FROM T_AllowanceHeader
                                  LEFT JOIN T_EmployeeRoute
                                    ON Ert_LedgerAlwCol = Alh_LedgerAlwCol
                                 WHERE Alh_LedgerAlwCol = (SELECT Pmt_CharValue
                                                             FROM T_ParameterMaster
                                                            WHERE Pmt_ParameterID = 'ARALWCOL' )
                                   AND Alh_ExtensionTable = 'T'
                                   AND Ert_EmployeeID = @Eot_EmployeeId
                                   AND @Eot_OvertimeDate BETWEEN Ert_EffectivityDate AND ISNULL(Ert_EndDate, DATEADD(month, 5, GETDATE()))

                                UPDATE T_EmployeeOvertime
                                SET   Eot_CurrentPayPeriod = @Eot_CurrentPayPeriod
                                    , Eot_OvertimeDate = @Eot_OvertimeDate
                                    , Eot_OvertimeType = @Eot_OvertimeType
                                    , Eot_StartTime = @Eot_StartTime
                                    , Eot_EndTime = @Eot_EndTime
                                    , Eot_OvertimeHour = @Eot_OvertimeHour
                                    , Eot_Reason = @Eot_Reason
                                    , Eot_JobCode = @Eot_JobCode
                                    , Eot_ClientJobNo = @Eot_ClientJobNo
                                    , Eot_Status = @Eot_Status
                                    , Eot_OvertimeFlag = @Eot_OvertimeFlag
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = getdate()
                                    , Eot_Filler1 = IIF(ISNULL(@AUTOROUTE, 0) = 1 , ISNULL(@RouteCode, ''), @Eot_Filler1)
                                    , Eot_Filler2 = @Eot_Filler2
                                    , Eot_Filler3 = @Eot_Filler3
                                WHERE Eot_ControlNo = @Eot_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateOTRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateOTRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateOTRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Eot_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Eot_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Eot_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", " ");
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("5")
                    || rowDetails["Eot_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Eot_CheckedBy = @Eot_CheckedBy ,
                                Eot_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", rowDetails["Eot_CheckedBy"]);
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("7")
                    || rowDetails["Eot_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Eot_Checked2By = @Eot_Checked2By ,
                                Eot_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_Checked2By", rowDetails["Eot_Checked2By"]);
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("9")
                    || rowDetails["Eot_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Eot_ApprovedBy = @Eot_ApprovedBy ,
                                Eot_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_ApprovedBy", rowDetails["Eot_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Eot_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Eot_Status", rowDetails["Eot_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Eot_ControlNo", rowDetails["Eot_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_EmployeeOvertime
                                    SET
                                        {0}
                                        Eot_Status = @Eot_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Eot_ControlNo = @Eot_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }
        public void UpdateOTRecordBatch(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[5];

            if (rowDetails["Eot_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Eot_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Eot_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", " ");
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("5")
                    || rowDetails["Eot_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Eot_CheckedBy = @Eot_CheckedBy ,
                                Eot_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_CheckedBy", rowDetails["Eot_CheckedBy"]);
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("7")
                    || rowDetails["Eot_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Eot_Checked2By = @Eot_Checked2By ,
                                Eot_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_Checked2By", rowDetails["Eot_Checked2By"]);
                }
                else if (rowDetails["Eot_Status"].ToString().Equals("9")
                    || rowDetails["Eot_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Eot_ApprovedBy = @Eot_ApprovedBy ,
                                Eot_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Eot_ApprovedBy", rowDetails["Eot_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Eot_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Eot_Status", rowDetails["Eot_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Eot_BatchNo", rowDetails["Eot_BatchNo"]);
            paramDetails[4] = new ParameterInfo("@Eot_EmployeeID", rowDetails["Eot_EmployeeID"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_EmployeeOvertime
                                    SET
                                        {0}
                                        Eot_Status = @Eot_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Eot_BatchNo = @Eot_BatchNo
                                          AND Eot_EmployeeID=@Eot_EmployeeID", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void InsertUpdateRemarks(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[3];
            paramDetails[0] = new ParameterInfo("@Trm_ControlNo", rowDetails["Trm_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Trm_Remarks", !rowDetails["Trm_Remarks"].ToString().Equals(string.Empty) ? rowDetails["Usr_Login"].ToString() + ":" + rowDetails["Trm_Remarks"].ToString() : string.Empty);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"INSERT INTO T_TransactionRemarks
                                      ( Trm_ControlNo
                                      , Trm_Remarks
                                      , Usr_Login
                                      , Ludatetime
                                      )
                                 VALUES
                                      ( @Trm_ControlNo
                                      , @Trm_Remarks
                                      , @Usr_Login
                                      , getdate()  
                                      ) ";
            string sqlUpdate = @"UPDATE T_TransactionRemarks
                                    SET Trm_Remarks = RIGHT((RTRIM(Trm_Remarks) + '|' + @Trm_Remarks),200)
                                      , Usr_Login = @Usr_Login
                                      , Ludatetime = getdate()
                                  WHERE Trm_ControlNo = @Trm_ControlNo";
            #endregion
            try
            {
                dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
            }
            catch
            {
                dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
            }
            
        }

        public void EmployeeLogLedgerUpdate(string idnumber
                                          , string processDate
                                          , string overtimeConsumed
                                          , string userlogin
                                          , string overtimetype
                                          , DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", idnumber);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", processDate);
            paramInfo[2] = new ParameterInfo("@OvertimeConsumed", overtimeConsumed);
            paramInfo[3] = new ParameterInfo("@Usr_Login", userlogin);

            string FieldUsed = string.Empty;
            if (overtimetype == "A")
                FieldUsed = "Ell_EncodedOvertimeAdvHr";
            else //for post and mid OT
                FieldUsed = "Ell_EncodedOvertimePostHr";

            string LedgerTable = GetLedgerTableFromDate(processDate);
            #endregion

            #region SQL Query
            string sqlQuery = string.Format(@"
                                    declare @totalHours as varchar(10)

                                    SET @totalHours = (SELECT SUM(Eot_OvertimeHour )
                                                         FROM (SELECT Eot_OvertimeHour 
                                                                 FROM T_EmployeeOvertime
                                                                WHERE Eot_EmployeeId = @Ell_EmployeeId
                                                                  AND Eot_OvertimeDate = @Ell_ProcessDate
                                                                  AND Eot_OvertimeType = '" + overtimetype + @"'
                                                                  AND Eot_Status IN ('9', 'A')
                                                                UNION ALL
                                                               SELECT Eot_OvertimeHour 
                                                                 FROM T_EmployeeOvertimeHist
                                                                WHERE Eot_EmployeeId = @Ell_EmployeeId
                                                                  AND Eot_OvertimeDate = @Ell_ProcessDate
                                                                  AND Eot_OvertimeType = '" + overtimetype + @"'
                                                                  AND Eot_Status IN ('9', 'A')
                                                                GROUP BY Eot_OvertimeHour) xx)

                                 UPDATE {0}
                                    SET {1} = @totalHours
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()
                                  WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                            AND Ell_ProcessDate = @Ell_ProcessDate", LedgerTable, FieldUsed);
            #endregion

            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(idnumber
                                        , MethodsLibrary.Methods.getPayPeriod(processDate)
                                        , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(idnumber
                                                , MethodsLibrary.Methods.getPayPeriod(processDate)
                                                , dal);
                    }
                }
                dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
        }

        public void EmployeeLogLedgerUpdate(string controlNo, string userLogged, DALHelper dal)
        {
            #region Parameters
            DataRow dr = getOvertimeInfo(controlNo, dal);
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", dr["Eot_EmployeeId"].ToString(), SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dr["Eot_OvertimeDate"].ToString(), SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@OvertimeConsumed", dr["Eot_OvertimeHour"].ToString());
            paramInfo[3] = new ParameterInfo("@OvertimeType", dr["Eot_OvertimeType"].ToString());
            paramInfo[4] = new ParameterInfo("@Usr_Login", userLogged);

            string FieldUsed = string.Empty;
            if (dr["Eot_OvertimeType"].ToString().Equals("A"))
                FieldUsed = "Ell_EncodedOvertimeAdvHr";
            else // for post and mid OT
                FieldUsed = "Ell_EncodedOvertimePostHr";

            string LedgerTable = GetLedgerTableFromDate(dr["Eot_OvertimeDate"].ToString());

            #endregion

            #region SQL Query
            string sqlQuery = string.Format(@"
                                    declare @totalHours as varchar(10)

                                    SET @totalHours = (SELECT SUM(Eot_OvertimeHour)
                                                         FROM (SELECT Eot_OvertimeHour 
                                                                 FROM T_EmployeeOvertime
                                                                WHERE Eot_EmployeeId = @Ell_EmployeeId
                                                                  AND Eot_OvertimeDate = @Ell_ProcessDate
                                                                  AND Eot_OvertimeType = @OvertimeType
                                                                  AND Eot_Status IN ('9', 'A')
                                                                UNION ALL
                                                               SELECT Eot_OvertimeHour 
                                                                 FROM T_EmployeeOvertimeHist
                                                                WHERE Eot_EmployeeId = @Ell_EmployeeId
                                                                  AND Eot_OvertimeDate = @Ell_ProcessDate
                                                                  AND Eot_OvertimeType = @OvertimeType
                                                                  AND Eot_Status IN ('9', 'A')
                                                                GROUP BY Eot_OvertimeHour) xx)
                                    --For overtime cancellation...column does not accept null value..updated by ROBERT 08/19/20103
                                 IF(@totalHours is NULL)
                                    SET @totalHours = 0;

                                 UPDATE {0}
                                    SET {1} = @totalHours
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()
                                WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                          AND Ell_ProcessDate = @Ell_ProcessDate", LedgerTable, FieldUsed);
            #endregion

            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(dr["Eot_EmployeeId"].ToString()
                                                                           , CommonMethods.getPayPeriod(Convert.ToDateTime(dr["Eot_OvertimeDate"].ToString()))
                                                                           , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(dr["Eot_EmployeeId"].ToString()
                                                , CommonMethods.getPayPeriod(Convert.ToDateTime(dr["Eot_OvertimeDate"].ToString()))
                                                , dal);
                    }
                }
                dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
        }

        public void initializeOTTypes(DropDownList ddlOTType, bool includeAllOption)
        {
            string sql = @" SELECT Pmx_ParameterValue
                                 , Pmx_ParameterDesc
                              FROM T_ParameterMasterExt
                             WHERE Pmx_ParameterID = 'OTTYPE'
                               AND Pmx_Status = 'A'
                             ORDER BY Pmx_ParameterDesc DESC";
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
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            ddlOTType.Items.Clear();
            if (includeAllOption)
            {
                ddlOTType.Items.Add(new ListItem( "ALL"
                                                , "ALL"));
            }
            if (!CommonMethods.isEmpty(ds))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ddlOTType.Items.Add(new ListItem(ds.Tables[0].Rows[i]["Pmx_ParameterDesc"].ToString()
                                                    , ds.Tables[0].Rows[i]["Pmx_ParameterValue"].ToString()));
                }
            }
            else
            {
                MessageBox.Show("No allowable overtime type setup.");
            }
        }

        public DataSet getMinimumAdvanceOvertime(string[] empId, string fromMMDDYYY, string toMMDDYYYY, string type, string hours)
        {
            #region SQL
            string sql = string.Format(@" DECLARE @EMPTABLE AS TABLE
													   (EmpID varchar(15))
													   INSERT INTO @EMPTABLE
													   SELECT Emt_EmployeeID FROM T_EmployeeMaster
													   WHERE Emt_EmployeeID IN ({0})

                            SELECT Ell_EmployeeId, MIN(Scm_ShiftTimeIn + ' ' + Convert(varchar(10),Ell_ProcessDate, 101)) [hhmm MM/dd/yyyy]
                              FROM (
                            SELECT Ell_EmployeeId, Scm_ShiftTimeIn
	                             , Ell_ProcessDate
                              FROM T_EmployeeLogLedger
                             INNER JOIN @EMPTABLE
                                    ON Ell_EmployeeId = EmpID
                                AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType 
																		                                     FROM T_EmployeeOvertime
																		                                    WHERE Eot_EmployeeId = EmpID
																		                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																		                                      AND Eot_Status IN ('1','3','5','7','9')
																		                                    UNION
																		                                   SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																		                                     FROM T_EmployeeOvertimeHist
																		                                    WHERE Eot_EmployeeId = EmpID
																		                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																		                                      AND Eot_Status IN ('1','3','5','7','9'))

                             UNION

                            SELECT Ell_EmployeeId, Scm_ShiftTimeIn
	                             , Ell_ProcessDate
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN @EMPTABLE
                                    ON Ell_EmployeeId = EmpID
                                AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																	                                         FROM T_EmployeeOvertime
																	                                        WHERE Eot_EmployeeId = EmpID
																	                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																	                                          AND Eot_Status IN ('1','3','5','7','9')
																	                                        UNION
																	                                       SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																	                                         FROM T_EmployeeOvertimeHist
																	                                        WHERE Eot_EmployeeId = EmpID
																	                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																	                                          AND Eot_Status IN ('1','3','5','7','9')) ) AS TEMP GROUP BY Ell_EmployeeId", FormatForInQuery(empId));
            #endregion


            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@fromDate", fromMMDDYYY);
            param[1] = new ParameterInfo("@toDate", toMMDDYYYY);
            param[2] = new ParameterInfo("@type", type);
            param[3] = new ParameterInfo("@hours", hours);
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (!CommonMethods.isEmpty(ds))
            {
                return ds;
            }
            else
            {
                return null;
            }
        }

        protected string FormatForInQuery(string[] array)
        {
            string retVal = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    retVal += ",";
                }
                retVal += String.Format("'{0}'", array[i]);
            }
            return retVal;
        }

        public string getMinimumAdvanceOvertime(string idNumber, string fromMMDDYYY, string toMMDDYYYY, string type, string hours)
        {
            #region SQL
            string sql = @" SELECT MIN(Scm_ShiftTimeIn + ' ' + Convert(varchar(10),Ell_ProcessDate, 101)) [hhmm MM/dd/yyyy]
                              FROM (
                            SELECT Scm_ShiftTimeIn
	                             , Ell_ProcessDate
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                                AND Ell_EmployeeId = @employeeId
                                AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType 
																		                                     FROM T_EmployeeOvertime
																		                                    WHERE Eot_EmployeeId = @employeeId
																		                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																		                                      AND Eot_Status IN ('1','3','5','7','9')
																		                                    UNION
																		                                   SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																		                                     FROM T_EmployeeOvertimeHist
																		                                    WHERE Eot_EmployeeId = @employeeId
																		                                      AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																		                                      AND Eot_Status IN ('1','3','5','7','9'))

                             UNION

                            SELECT Scm_ShiftTimeIn
	                             , Ell_ProcessDate
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                                AND Ell_EmployeeId = @employeeId
                                AND Ell_ProcessDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
                              LEFT JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE 
                                1 = CASE WHEN (@type = 'M')
                                            THEN CASE WHEN (LEFT(Scm_ShiftBreakEnd,2) +':'+ RIGHT(Scm_ShiftBreakEnd, 2) < dbo.computeOTTime(Scm_ShiftBreakStart, Convert(decimal(7,2),@hours), Scm_ShiftBreakStart, Scm_ShiftBreakStart, 0))
                                                      THEN 0
                                                      ELSE 1
                                                  END
                                            ELSE 1
                                        END
                               AND Ell_EmployeeId+CONVERT(varchar(10), Ell_ProcessDate, 101)+@type NOT IN (SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																	                                         FROM T_EmployeeOvertime
																	                                        WHERE Eot_EmployeeId = @employeeId
																	                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																	                                          AND Eot_Status IN ('1','3','5','7','9')
																	                                        UNION
																	                                       SELECT Eot_EmployeeId+CONVERT(varchar(10), Eot_OvertimeDate, 101)+Eot_OvertimeType
																	                                         FROM T_EmployeeOvertimeHist
																	                                        WHERE Eot_EmployeeId = @employeeId
																	                                          AND Eot_OvertimeDate BETWEEN Convert(datetime, @fromDate) AND Convert(datetime, @toDate)
																	                                          AND Eot_Status IN ('1','3','5','7','9')) ) AS TEMP";
            #endregion


            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@employeeId", idNumber);
            param[1] = new ParameterInfo("@fromDate", fromMMDDYYY);
            param[2] = new ParameterInfo("@toDate", toMMDDYYYY);
            param[3] = new ParameterInfo("@type", type);
            param[4] = new ParameterInfo("@hours", hours);
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (!CommonMethods.isEmpty(ds))
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public bool CheckHasRoute(DALHelper dal, string idNumber, string MMddyyyy)
        {
            string sql = @" DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                           FROM T_ProcessControlMaster
                                                          WHERE Pcm_SystemID = 'OVERTIME'
                                                            AND Pcm_ProcessID = 'AUTOROUTE'
                                                            AND Pcm_Status = 'A' )

                            DECLARE @RouteCode as char(4) = ''
                            IF (@AUTOROUTE = 1)
                            BEGIN
                             SELECT @RouteCode = Ert_RouteCode 
                               FROM T_AllowanceHeader
                               LEFT JOIN T_EmployeeRoute
                                 ON Ert_LedgerAlwCol = Alh_LedgerAlwCol
                              WHERE Alh_LedgerAlwCol = (SELECT Pmt_CharValue
                                                          FROM T_ParameterMaster
                                                         WHERE Pmt_ParameterID = 'ARALWCOL' )
                                AND Alh_ExtensionTable = 'T'
                                AND Ert_EmployeeID = '{0}'
                                AND '{1}' BETWEEN Ert_EffectivityDate AND ISNULL(Ert_EndDate, DATEADD(month, 5, GETDATE()))
                            END
                            ELSE
                            BEGIN
                                SET @RouteCode = 'N/A'
                            END 
                            SELECT IIF(ISNULL(@RouteCode, '') = '' , 'FALSE', 'TRUE')  ";

            return Convert.ToBoolean(dal.ExecuteScalar(string.Format(sql, idNumber, MMddyyyy), CommandType.Text).ToString());
        }
    }
}
