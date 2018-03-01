/* Revision no. W2.1.00001 */
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
/// Summary description for LeaveBL
/// </summary>
namespace Payroll.DAL
{
    public class TimekeepBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public TimekeepBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string ComputeTKFlag(string ProcessDate)
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

        private string GetLedgerTableFromDate(string Date)
        {
            string LedgerTable = string.Empty;
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
            else
            {
                LedgerTable = "T_EmployeeLogLedger";
            }
            return LedgerTable;
        }

        public void TagTimeMod(string MMddyyyy, string employeeId, string tag, DALHelper dal)
        {
            string sql = string.Format(@" UPDATE T_EmployeeLogLedger
                                             SET Ell_TagTimeMod = '{0}'
                                           WHERE Ell_ProcessDate = '{1}'
                                             AND Ell_EmployeeId = '{2}'

                                          UPDATE T_EmployeeLogLedgerHist
                                             SET Ell_TagTimeMod = '{0}'
                                           WHERE Ell_ProcessDate = '{1}'
                                             AND Ell_EmployeeId = '{2}'", tag, MMddyyyy, employeeId);
            dal.ExecuteNonQuery(sql, CommandType.Text);
        }

        public DataRow getTimeRecordInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT T_TimeRecMod.Trm_ControlNo [Control No]
                                  , Trm_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Convert(varchar(10), Trm_ModDate, 101) [Adjustment Date]
                                  , Trm_Type [Mod Code]
                                  , ADT2.Adt_AccountDesc [Mod Description]
                                  , CASE WHEN (Trm_ActualTimeIn1 <> '') 
                                         THEN LEFT(Trm_ActualTimeIn1,2) 
                                            + ':' 
                                            + RIGHT(Trm_ActualTimeIn1,2)
                                         ELSE ''
                                     END [Time In 1]
                                  , CASE WHEN (Trm_ActualTimeOut1 <> '') 
                                         THEN LEFT(Trm_ActualTimeOut1,2) 
                                            + ':' 
                                            + RIGHT(Trm_ActualTimeOut1,2)
                                         ELSE ''
                                     END [Time Out 1]
                                  , CASE WHEN (Trm_ActualTimeIn2 <> '') 
                                         THEN LEFT(Trm_ActualTimeIn2,2) 
                                            + ':' 
                                            + RIGHT(Trm_ActualTimeIn2,2)
                                         ELSE ''
                                     END [Time In 2]
                                  , CASE WHEN (Trm_ActualTimeOut2 <> '') 
                                         THEN LEFT(Trm_ActualTimeOut2,2) 
                                            + ':' 
                                            + RIGHT(Trm_ActualTimeOut2,2)
                                         ELSE ''
                                     END [Time Out 2]
                                  , Trm_Reason [Reason]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_LogControl [Log Control]
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
							      , Scm_ScheduleType [Schedule Type]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_TimeRecMod
                               LEFT JOIN T_EmployeeLogLedger Ledger
                                 ON Ledger.Ell_EmployeeId = Trm_EmployeeId
                                AND Ledger.Ell_ProcessDate = Trm_ModDate
                               LEFT JOIN T_EmployeeLogLedgerHist LedgerHist
                                 ON LedgerHist.Ell_EmployeeId = Trm_EmployeeId
                                AND LedgerHist.Ell_ProcessDate = Trm_ModDate
                               LEFT JOIN T_ShiftCodeMaster
                                 ON Scm_ShiftCode = CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                                         THEN LedgerHist.Ell_ShiftCode
                                                         ELSE Ledger.Ell_ShiftCode
                                                     END
                               LEFT JOIN T_EmployeeMaster 
                                 ON Emt_EmployeeId = Trm_EmployeeId
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Trm_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail ADT2
                                 ON ADT2.Adt_AccountType = 'TMERECTYPE'
                                AND ADT2.Adt_AccountCode = Trm_Type
                               LEFT JOIN T_PayPeriodMaster 
                                 ON Ppm_PayPeriod = Trm_CurrentPayPeriod
                               LEFT JOIN T_TransactionRemarks 
                                 ON T_TransactionRemarks.Trm_ControlNo = T_TimeRecMod.Trm_ControlNo
                              WHERE T_TimeRecMod.Trm_ControlNo = @ControlNo";
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

        public static DataRow getTimeRecordInfo(string controlNo, DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Trm_ControlNo
                                  , Trm_EmployeeId
                                  , Trm_ModDate
                                  , Trm_Type
                                  , Trm_ActualTimeIn1
                                  , Trm_ActualTimeOut1
                                  , Trm_ActualTimeIn2
                                  , Trm_ActualTimeOut2
                                  , Trm_Reason
                                  , Trm_Status
                                  , Trm_LogControl
                                  , Trm_EndorsedDateToChecker
                                  , Trm_CheckedBy
                                  , Trm_CheckedDate
                                  , Trm_Checked2By
                                  , Trm_Checked2Date
                                  , Trm_ApprovedBy
                                  , Trm_ApprovedDate
                                  , Trm_Flag
                               FROM T_TimeRecMod
                              WHERE Trm_ControlNo = @ControlNo";
            return dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0].Rows[0];
        }
        //Saving Function
        public void CreateTKRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Trm_ControlNo", rowDetails["Trm_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Trm_CurrentPayPeriod", rowDetails["Trm_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Trm_EmployeeId", rowDetails["Trm_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Trm_ModDate", rowDetails["Trm_ModDate"]);
            paramDetails[4] = new ParameterInfo("@Trm_Type", rowDetails["Trm_Type"]);
            paramDetails[5] = new ParameterInfo("@Trm_ActualTimeIn1", rowDetails["Trm_ActualTimeIn1"]);
            paramDetails[6] = new ParameterInfo("@Trm_ActualTimeOut1", rowDetails["Trm_ActualTimeOut1"]);
            paramDetails[7] = new ParameterInfo("@Trm_ActualTimeIn2", rowDetails["Trm_ActualTimeIn2"]);
            paramDetails[8] = new ParameterInfo("@Trm_ActualTimeOut2", rowDetails["Trm_ActualTimeOut2"]);
            paramDetails[9] = new ParameterInfo("@Trm_Reason", rowDetails["Trm_Reason"]);
            paramDetails[10] = new ParameterInfo("@Trm_Status", rowDetails["Trm_Status"]);
            paramDetails[11] = new ParameterInfo("@Trm_Flag", rowDetails["Trm_Flag"]);
            paramDetails[12] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[13] = new ParameterInfo("@Trm_LogControl", rowDetails["Trm_LogControl"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for filler insertion
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Trm_EmployeeId
                                                      AND @Trm_ModDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Trm_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Trm_EmployeeId
                                                    AND @Trm_ModDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END
IF NOT EXISTS(SELECT Trm_ControlNo FROM T_TimeRecMod
WHERE Trm_CurrentPayPeriod = @Trm_CurrentPayPeriod
AND Trm_EmployeeId = @Trm_EmployeeId
AND Trm_ModDate = @Trm_ModDate
AND Trm_Type = @Trm_Type
AND Trm_ActualTimeIn1 = @Trm_ActualTimeIn1
AND Trm_ActualTimeOut1 = @Trm_ActualTimeOut1
AND Trm_ActualTimeIn2 = @Trm_ActualTimeIn2
AND Trm_ActualTimeOut2 = @Trm_ActualTimeOut2
AND Trm_Reason = @Trm_Reason
AND Trm_Status = @Trm_Status
AND Trm_Flag = @Trm_Flag
AND Trm_LogControl = @Trm_LogControl)
BEGIN
                               INSERT INTO T_TimeRecMod
                                (     Trm_ControlNo
                                    , Trm_CurrentPayPeriod
                                    , Trm_EmployeeId
                                    , Trm_ModDate
                                    , Trm_AppliedDate
                                    , Trm_Costcenter
                                    , Trm_CostcenterLine
                                    , Trm_Type
                                    , Trm_ActualTimeIn1
                                    , Trm_ActualTimeOut1
                                    , Trm_ActualTimeIn2
                                    , Trm_ActualTimeOut2
                                    , Trm_Reason
                                    , Trm_Status
                                    , Trm_Flag
                                    , Usr_Login
                                    , Ludatetime
                                    , Trm_LogControl
                                )
                                VALUES
                                (     @Trm_ControlNo
                                    , @Trm_CurrentPayPeriod
                                    , @Trm_EmployeeId
                                    , @Trm_ModDate
                                    , getdate()
                                    , @Costcenter
                                    , @LineCode
                                    , @Trm_Type
                                    , @Trm_ActualTimeIn1
                                    , @Trm_ActualTimeOut1
                                    , @Trm_ActualTimeIn2
                                    , @Trm_ActualTimeOut2
                                    , @Trm_Reason
                                    , @Trm_Status
                                    , @Trm_Flag
                                    , @Usr_Login
                                    , getdate()
                                    , @Trm_LogControl
                                ) 
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateTKRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Trm_ControlNo", rowDetails["Trm_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Trm_CurrentPayPeriod", rowDetails["Trm_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Trm_EmployeeId", rowDetails["Trm_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Trm_ModDate", rowDetails["Trm_ModDate"]);
            paramDetails[4] = new ParameterInfo("@Trm_Type", rowDetails["Trm_Type"]);
            paramDetails[5] = new ParameterInfo("@Trm_ActualTimeIn1", rowDetails["Trm_ActualTimeIn1"]);
            paramDetails[6] = new ParameterInfo("@Trm_ActualTimeOut1", rowDetails["Trm_ActualTimeOut1"]);
            paramDetails[7] = new ParameterInfo("@Trm_ActualTimeIn2", rowDetails["Trm_ActualTimeIn2"]);
            paramDetails[8] = new ParameterInfo("@Trm_ActualTimeOut2", rowDetails["Trm_ActualTimeOut2"]);
            paramDetails[9] = new ParameterInfo("@Trm_Reason", rowDetails["Trm_Reason"]);
            paramDetails[10] = new ParameterInfo("@Trm_Status", rowDetails["Trm_Status"]);
            paramDetails[11] = new ParameterInfo("@Trm_Flag", rowDetails["Trm_Flag"]);
            paramDetails[12] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[13] = new ParameterInfo("@Trm_LogControl", rowDetails["Trm_LogControl"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                UPDATE T_TimeRecMod
                                SET   Trm_CurrentPayPeriod = @Trm_CurrentPayPeriod
                                    , Trm_ModDate = @Trm_ModDate
                                    , Trm_Type = @Trm_Type
                                    , Trm_ActualTimeIn1 = @Trm_ActualTimeIn1
                                    , Trm_ActualTimeOut1 = @Trm_ActualTimeOut1
                                    , Trm_ActualTimeIn2 = @Trm_ActualTimeIn2
                                    , Trm_ActualTimeOut2 = @Trm_ActualTimeOut2
                                    , Trm_Reason = @Trm_Reason
                                    , Trm_Status = @Trm_Status
                                    , Trm_Flag = @Trm_Flag
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = getdate()
                                WHERE Trm_ControlNo = @Trm_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateTKRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateTKRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateTKRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Trm_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Trm_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Trm_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Trm_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Trm_CheckedBy", " ");
                }
                else if (rowDetails["Trm_Status"].ToString().Equals("5")
                    || rowDetails["Trm_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Trm_CheckedBy = @Trm_CheckedBy ,
                                Trm_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Trm_CheckedBy", rowDetails["Trm_CheckedBy"]);
                }
                else if (rowDetails["Trm_Status"].ToString().Equals("7")
                    || rowDetails["Trm_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Trm_Checked2By = @Trm_Checked2By ,
                                Trm_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Trm_Checked2By", rowDetails["Trm_Checked2By"]);
                }
                else if (rowDetails["Trm_Status"].ToString().Equals("9")
                    || rowDetails["Trm_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Trm_ApprovedBy = @Trm_ApprovedBy ,
                                Trm_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Trm_ApprovedBy", rowDetails["Trm_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Trm_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Trm_Status", rowDetails["Trm_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Trm_ControlNo", rowDetails["Trm_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_TimeRecMod
                                       SET
                                        {0}
                                        Trm_Status = @Trm_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                     WHERE Trm_ControlNo = @Trm_ControlNo

                                    --UPDATE Ell_TagTimeMd to 'N' on disapprove
                                    IF (@Trm_Status = '2' OR @Trm_Status = '4' OR @Trm_Status = '6' OR @Trm_Status = '8')
                                 BEGIN 
                                UPDATE T_EmployeeLogLedger
                                   SET Ell_TagTimeMod = 'N'
                                  FROM T_EmployeeLogLedger
                                 INNER JOIN T_TimeRecMod
                                    ON Trm_ModDate = Ell_ProcessDate
                                   AND Trm_EmployeeId = Ell_EmployeeId
                                   AND Trm_ControlNo = @Trm_ControlNo

                                UPDATE T_EmployeeLogLedgerHist
                                   SET Ell_TagTimeMod = 'N'
                                  FROM T_EmployeeLogLedgerHist
                                 INNER JOIN T_TimeRecMod
                                    ON Trm_ModDate = Ell_ProcessDate
                                   AND Trm_EmployeeId = Ell_EmployeeId
                                   AND Trm_ControlNo = @Trm_ControlNo

                                   END

                                ", fieldUsed);
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

        public void EmployeeLogLedgerUpdate(DataRow dr, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeID", dr["Trm_EmployeeId"]);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dr["Trm_ModDate"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", dr["Usr_Login"]);
            paramInfo[3] = new ParameterInfo("@TimeIn1", dr["Trm_ActualTimeIn1"]);
            paramInfo[4] = new ParameterInfo("@TimeOut1", dr["Trm_ActualTimeOut1"]);
            paramInfo[5] = new ParameterInfo("@TimeIn2", dr["Trm_ActualTimeIn2"]);
            paramInfo[6] = new ParameterInfo("@TimeOut2", dr["Trm_ActualTimeOut2"]);
            #endregion

            string LedgerTable = GetLedgerTableFromDate(dr["Trm_ModDate"].ToString());

            #region SQL Query
            string sqlInsert = @"
                               UPDATE T_EmployeeLogLedger
                                  SET Ell_ActualTimeIn_1 = CASE WHEN (@TimeIn1 <> '')
							                                    THEN CASE WHEN (@TimeIn1 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_1 = CASE WHEN (@TimeOut1 <> '')
							                                    THEN CASE WHEN (@TimeOut1 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeIn_2 = CASE WHEN (@TimeIn2 <> '')
							                                    THEN CASE WHEN (@TimeIn2 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_2 = CASE WHEN (@TimeOut2 <> '')
							                                    THEN CASE WHEN (@TimeOut2 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Usr_Login  = @Usr_Login
                                    , Ludatetime = GetDate()
                                    , Ell_TagTimeMod = 'A'
                                WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                          AND Ell_ProcessDate = @Ell_ProcessDate

                               UPDATE T_EmployeeLogLedgerHist
                                  SET Ell_ActualTimeIn_1 = CASE WHEN (@TimeIn1 <> '')
							                                    THEN CASE WHEN (@TimeIn1 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_1 = CASE WHEN (@TimeOut1 <> '')
							                                    THEN CASE WHEN (@TimeOut1 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeIn_2 = CASE WHEN (@TimeIn2 <> '')
							                                    THEN CASE WHEN (@TimeIn2 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_2 = CASE WHEN (@TimeOut2 <> '')
							                                    THEN CASE WHEN (@TimeOut2 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Usr_Login  = @Usr_Login
                                    , Ludatetime = GetDate()
                                    , Ell_TagTimeMod = 'A'
                                WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                          AND Ell_ProcessDate = @Ell_ProcessDate ";

            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC.ToUpper()))
            {
                sqlInsert += @"

                            UPDATE T_EmployeeLogLedger
                               SET Ell_ShiftCode = CASE WHEN Ell_RestDay = 1 OR Ell_Holiday = 1 
			                                            THEN Shift.Scm_EquivalentShiftCode
                                                        ELSE CASE WHEN (Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr <> 0) OR (UPPER(DATENAME(DW, Ell_ProcessDate))) = 'WEDNESDAY' THEN DefaultShift.Scm_ShiftCode
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 <= '0800'  THEN 'N001'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0800' AND @TimeIn1 <= '0815' THEN 'N002'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0815' AND @TimeIn1 <= '0830' THEN 'N003'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0830' AND @TimeIn1 <= '0845' THEN 'N004'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0845' AND @TimeIn1 < '1200' THEN 'N005'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 <= '2000'  THEN 'G001'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2000' AND @TimeIn1 <= '2015' THEN 'G002'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2015' AND @TimeIn1 <= '2030' THEN 'G003'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2030' AND @TimeIn1 <= '2045' THEN 'G004'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2045' AND @TimeIn1 <= '2359' THEN 'G005'          
					                                              ELSE DefaultShift.Scm_ShiftCode
				                                              END
                                                    END
                                             FROM {0}
                                            INNER JOIN T_ShiftCodeMaster Shift 
                                               ON Shift.Scm_ShiftCode = Ell_ShiftCode
                                             LEFT JOIN T_ShiftCodeMaster DefaultShift 
                                               ON DefaultShift.Scm_ScheduleType = Shift.Scm_ScheduleType
                                              AND DefaultShift.Scm_DefaultShift = 'True'
                                              AND DefaultShift.Scm_Status = 'A'
                                            WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                                      AND Ell_ProcessDate = @Ell_ProcessDate

                            UPDATE T_EmployeeLogLedgerHist
                               SET Ell_ShiftCode = CASE WHEN Ell_RestDay = 1 OR Ell_Holiday = 1 
			                                            THEN Shift.Scm_EquivalentShiftCode
                                                        ELSE CASE WHEN (Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr <> 0) OR (UPPER(DATENAME(DW, Ell_ProcessDate))) = 'WEDNESDAY' THEN DefaultShift.Scm_ShiftCode
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 <= '0800'  THEN 'N001'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0800' AND @TimeIn1 <= '0815' THEN 'N002'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0815' AND @TimeIn1 <= '0830' THEN 'N003'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0830' AND @TimeIn1 <= '0845' THEN 'N004'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0845' AND @TimeIn1 < '1200' THEN 'N005'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 <= '2000'  THEN 'G001'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2000' AND @TimeIn1 <= '2015' THEN 'G002'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2015' AND @TimeIn1 <= '2030' THEN 'G003'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2030' AND @TimeIn1 <= '2045' THEN 'G004'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2045' AND @TimeIn1 <= '2359' THEN 'G005'          
					                                              ELSE DefaultShift.Scm_ShiftCode
				                                              END
                                                    END
                                             FROM {0}
                                            INNER JOIN T_ShiftCodeMaster Shift 
                                               ON Shift.Scm_ShiftCode = Ell_ShiftCode
                                             LEFT JOIN T_ShiftCodeMaster DefaultShift 
                                               ON DefaultShift.Scm_ScheduleType = Shift.Scm_ScheduleType
                                              AND DefaultShift.Scm_DefaultShift = 'True'
                                              AND DefaultShift.Scm_Status = 'A'
                                            WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                                      AND Ell_ProcessDate = @Ell_ProcessDate";
            }

            string sqlInsertLogTrail = string.Format( @"   
                                            INSERT INTO T_EmployeeLogTrail
                                                 ( Elt_EmployeeId
											     , Elt_ProcessDate
											     , Elt_SeqNo
											     , Elt_ShiftCode
											     , Elt_ActualTimeIn_1
											     , Elt_ActualTimeOut_1
											     , Elt_ActualTimeIn_2
											     , Elt_ActualTimeOut_2
											     , Usr_Login
											     , Ludatetime
											     )
                                            SELECT [TEMPCOL]
	                                             , @Ell_ProcessDate
	                                             , REPLICATE('0', 2 - LEN(MAX(ISNULL(Elt_Seqno, 0)) + 1 )) 
	                                             + Convert(varchar(2), MAX(ISNULL(Elt_Seqno, 0)) + 1 )
	                                             , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
	                                             , @Usr_Login
	                                             , GETDATE()
                                              FROM (SELECT '{0}' [TEMPCOL]) AS TEMP
                                              LEFT JOIN T_EmployeeLogTrail
                                                ON Elt_EmployeeId = [TEMPCOL]
                                               AND Elt_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedger ELL1
                                                ON ELL1.Ell_EmployeeId = [TEMPCOL]
                                               AND ELL1.Ell_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedgerHist Ell2
                                                ON Ell2.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell2.Ell_ProcessDate  = @Ell_ProcessDate
                                             GROUP BY [TEMPCOL]
		                                            , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)", dr["Trm_EmployeeId"].ToString() );
            DataSet check = new DataSet();
            try
            {
                check = dal.ExecuteDataSet(@"Select TOP 1 Elt_DayCode
                                ,Elt_Restday
                                ,Elt_Holiday
                             FROM T_EmployeeLogTrail");
            }
            catch
            {
                check = null;
            }
            if (check != null)
            {
                sqlInsertLogTrail = string.Format(@"INSERT INTO T_EmployeeLogTrail
                                                 ( Elt_EmployeeId
											     , Elt_ProcessDate
											     , Elt_SeqNo
                                                 , Elt_DayCode
                                                 , Elt_Restday
                                                 , Elt_Holiday
											     , Elt_ShiftCode
											     , Elt_ActualTimeIn_1
											     , Elt_ActualTimeOut_1
											     , Elt_ActualTimeIn_2
											     , Elt_ActualTimeOut_2
											     , Usr_Login
											     , Ludatetime

											     )
                                            SELECT [TEMPCOL]
	                                             , @Ell_ProcessDate
	                                             , REPLICATE('0', 2 - LEN(MAX(ISNULL(Elt_Seqno, 0)) + 1 )) 
	                                             + Convert(varchar(2), MAX(ISNULL(Elt_Seqno, 0)) + 1 )
                                                 , ISNULL(Ell1.Ell_DayCode, Ell2.Ell_DayCode)
	                                             , ISNULL(Ell1.Ell_Restday, Ell2.Ell_Restday)
	                                             , ISNULL(Ell1.Ell_Holiday, Ell2.Ell_Holiday)
	                                             , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
	                                             , @Usr_Login
	                                             , GETDATE()
                                                 
                                              FROM (SELECT '{0}' [TEMPCOL]) AS TEMP
                                              LEFT JOIN T_EmployeeLogTrail
                                                ON Elt_EmployeeId = [TEMPCOL]
                                               AND Elt_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedger ELL1
                                                ON ELL1.Ell_EmployeeId = [TEMPCOL]
                                               AND ELL1.Ell_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedgerHist Ell2
                                                ON Ell2.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell2.Ell_ProcessDate  = @Ell_ProcessDate
                                             GROUP BY [TEMPCOL]
		                                            , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
                                                    , ISNULL(Ell1.Ell_DayCode, Ell2.Ell_DayCode)
	                                                , ISNULL(Ell1.Ell_Restday, Ell2.Ell_Restday)
	                                                , ISNULL(Ell1.Ell_Holiday, Ell2.Ell_Holiday)", dr["Trm_EmployeeId"].ToString());
            }

            #endregion

            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(dr["Trm_EmployeeId"].ToString()
                                        , MethodsLibrary.Methods.getPayPeriod(dr["Trm_ModDate"].ToString())
                                        , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(dr["Trm_EmployeeId"].ToString()
                                                , MethodsLibrary.Methods.getPayPeriod(dr["Trm_ModDate"].ToString())
                                                , dal);
                    }
                }
                dal.ExecuteNonQuery(sqlInsertLogTrail, CommandType.Text, paramInfo);
                dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
        }

        public void EmployeeLogLedgerUpdate(string controlNo, string userLogged, DALHelper dal)
        {
            #region Parameters
            DataRow dr = getTimeRecordInfo(controlNo, dal);
            ParameterInfo[] paramInfo = new ParameterInfo[7];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeID", dr["Trm_EmployeeId"]);
            paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dr["Trm_ModDate"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", userLogged);
            paramInfo[3] = new ParameterInfo("@TimeIn1", dr["Trm_ActualTimeIn1"]);
            paramInfo[4] = new ParameterInfo("@TimeOut1", dr["Trm_ActualTimeOut1"]);
            paramInfo[5] = new ParameterInfo("@TimeIn2", dr["Trm_ActualTimeIn2"]);
            paramInfo[6] = new ParameterInfo("@TimeOut2", dr["Trm_ActualTimeOut2"]);
            #endregion
            string LedgerTable = GetLedgerTableFromDate(dr["Trm_ModDate"].ToString());

            #region SQL Query
            string sqlInsert = @"
                               UPDATE T_EmployeeLogLedger
                                  SET Ell_ActualTimeIn_1 = CASE WHEN (@TimeIn1 <> '')
							                                    THEN CASE WHEN (@TimeIn1 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_1 = CASE WHEN (@TimeOut1 <> '')
							                                    THEN CASE WHEN (@TimeOut1 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeIn_2 = CASE WHEN (@TimeIn2 <> '')
							                                    THEN CASE WHEN (@TimeIn2 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_2 = CASE WHEN (@TimeOut2 <> '')
							                                    THEN CASE WHEN (@TimeOut2 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Usr_Login  = @Usr_Login
                                    , Ludatetime = GetDate()
                                    , Ell_TagTimeMod = 'A'
                                WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                          AND Ell_ProcessDate = @Ell_ProcessDate

                                UPDATE T_EmployeeLogLedgerHist
                                  SET Ell_ActualTimeIn_1 = CASE WHEN (@TimeIn1 <> '')
							                                    THEN CASE WHEN (@TimeIn1 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_1 = CASE WHEN (@TimeOut1 <> '')
							                                    THEN CASE WHEN (@TimeOut1 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut1 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeIn_2 = CASE WHEN (@TimeIn2 <> '')
							                                    THEN CASE WHEN (@TimeIn2 = '2400')
								 		                                  THEN '2359'
										                                  ELSE @TimeIn2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Ell_ActualTimeOut_2 = CASE WHEN (@TimeOut2 <> '')
							                                    THEN CASE WHEN (@TimeOut2 = '2400')
								 		                                  THEN '2401'
										                                  ELSE @TimeOut2 
									                                  END
							                                    ELSE '0000' 
						                                    END
                                    , Usr_Login  = @Usr_Login
                                    , Ludatetime = GetDate()
                                    , Ell_TagTimeMod = 'A'
                                WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                          AND Ell_ProcessDate = @Ell_ProcessDate";
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                sqlInsert += @"
                             UPDATE T_EmployeeLogLedger
                               SET Ell_ShiftCode = CASE WHEN Ell_RestDay = 1 OR Ell_Holiday = 1 
			                                            THEN Shift.Scm_EquivalentShiftCode
                                                        ELSE CASE WHEN (Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr <> 0) OR (UPPER(DATENAME(DW, Ell_ProcessDate))) = 'WEDNESDAY' THEN DefaultShift.Scm_ShiftCode
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 <= '0800'  THEN 'N001'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0800' AND @TimeIn1 <= '0815' THEN 'N002'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0815' AND @TimeIn1 <= '0830' THEN 'N003'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0830' AND @TimeIn1 <= '0845' THEN 'N004'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0845' AND @TimeIn1 < '1200' THEN 'N005'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 <= '2000'  THEN 'G001'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2000' AND @TimeIn1 <= '2015' THEN 'G002'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2015' AND @TimeIn1 <= '2030' THEN 'G003'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2030' AND @TimeIn1 <= '2045' THEN 'G004'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2045' AND @TimeIn1 <= '2359' THEN 'G005'          
					                                              ELSE DefaultShift.Scm_ShiftCode
				                                              END
                                                    END
                                             FROM T_EmployeeLogLedger
                                            INNER JOIN T_ShiftCodeMaster Shift 
                                               ON Shift.Scm_ShiftCode = Ell_ShiftCode
                                             LEFT JOIN T_ShiftCodeMaster DefaultShift 
                                               ON DefaultShift.Scm_ScheduleType = Shift.Scm_ScheduleType
                                              AND DefaultShift.Scm_DefaultShift = 'True'
                                              AND DefaultShift.Scm_Status = 'A'
                                            WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                                      AND Ell_ProcessDate = @Ell_ProcessDate 
                            UPDATE T_EmployeeLogLedgerHist
                               SET Ell_ShiftCode = CASE WHEN Ell_RestDay = 1 OR Ell_Holiday = 1 
			                                            THEN Shift.Scm_EquivalentShiftCode
                                                        ELSE CASE WHEN (Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr <> 0) OR (UPPER(DATENAME(DW, Ell_ProcessDate))) = 'WEDNESDAY' THEN DefaultShift.Scm_ShiftCode
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 <= '0800'  THEN 'N001'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0800' AND @TimeIn1 <= '0815' THEN 'N002'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0815' AND @TimeIn1 <= '0830' THEN 'N003'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0830' AND @TimeIn1 <= '0845' THEN 'N004'
                                                                  WHEN Shift.Scm_ScheduleType = 'N' AND @TimeIn1 > '0845' AND @TimeIn1 < '1200' THEN 'N005'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 <= '2000'  THEN 'G001'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2000' AND @TimeIn1 <= '2015' THEN 'G002'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2015' AND @TimeIn1 <= '2030' THEN 'G003'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2030' AND @TimeIn1 <= '2045' THEN 'G004'
                                                                  WHEN Shift.Scm_ScheduleType = 'G' AND @TimeIn1 > '2045' AND @TimeIn1 <= '2359' THEN 'G005'          
					                                              ELSE DefaultShift.Scm_ShiftCode
				                                              END
                                                    END
                                             FROM T_EmployeeLogLedgerHist
                                            INNER JOIN T_ShiftCodeMaster Shift 
                                               ON Shift.Scm_ShiftCode = Ell_ShiftCode
                                             LEFT JOIN T_ShiftCodeMaster DefaultShift 
                                               ON DefaultShift.Scm_ScheduleType = Shift.Scm_ScheduleType
                                              AND DefaultShift.Scm_DefaultShift = 'True'
                                              AND DefaultShift.Scm_Status = 'A'
                                            WHERE Ell_EmployeeId  = @Ell_EmployeeId
		                                      AND Ell_ProcessDate = @Ell_ProcessDate 
                                                ";
            }

            string sqlInsertLogTrail = string.Format(@"   
                                            INSERT INTO T_EmployeeLogTrail
                                                 ( Elt_EmployeeId
											     , Elt_ProcessDate
											     , Elt_SeqNo
											     , Elt_ShiftCode
											     , Elt_ActualTimeIn_1
											     , Elt_ActualTimeOut_1
											     , Elt_ActualTimeIn_2
											     , Elt_ActualTimeOut_2
											     , Usr_Login
											     , Ludatetime
											     )
                                            SELECT [TEMPCOL]
	                                             , @Ell_ProcessDate
	                                             , REPLICATE('0', 2 - LEN(MAX(ISNULL(Elt_Seqno, 0)) + 1 )) 
	                                             + Convert(varchar(2), MAX(ISNULL(Elt_Seqno, 0)) + 1 )
	                                             , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
	                                             , @Usr_Login
	                                             , GETDATE()
                                              FROM (SELECT '{0}' [TEMPCOL]) AS TEMP
                                              LEFT JOIN T_EmployeeLogTrail
                                                ON Elt_EmployeeId = [TEMPCOL]
                                               AND Elt_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedger ELL1
                                                ON ELL1.Ell_EmployeeId = [TEMPCOL]
                                               AND ELL1.Ell_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedgerHist Ell2
                                                ON Ell2.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell2.Ell_ProcessDate  = @Ell_ProcessDate
                                             GROUP BY [TEMPCOL]
		                                            , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
                                                    , ISNULL(Ell1.Ell_DayCode, Ell2.Ell_DayCode)
	                                                , ISNULL(Ell1.Ell_Restday, Ell2.Ell_Restday)
	                                                , ISNULL(Ell1.Ell_Holiday, Ell2.Ell_Holiday)", dr["Trm_EmployeeId"].ToString());
            DataSet check = new DataSet();
            try
            {
                check = dal.ExecuteDataSet(@"Select TOP 1 Elt_DayCode
                                ,Elt_Restday
                                ,Elt_Holiday
                             FROM T_EmployeeLogTrail");
            }
            catch
            {
                check = null;
            }
            if (check != null)
            {
                sqlInsertLogTrail = string.Format(@"INSERT INTO T_EmployeeLogTrail
                                                 ( Elt_EmployeeId
											     , Elt_ProcessDate
											     , Elt_SeqNo
                                                 , Elt_DayCode
                                                 , Elt_Restday
                                                 , Elt_Holiday
											     , Elt_ShiftCode
											     , Elt_ActualTimeIn_1
											     , Elt_ActualTimeOut_1
											     , Elt_ActualTimeIn_2
											     , Elt_ActualTimeOut_2
											     , Usr_Login
											     , Ludatetime

											     )
                                            SELECT [TEMPCOL]
	                                             , @Ell_ProcessDate
	                                             , REPLICATE('0', 2 - LEN(MAX(ISNULL(Elt_Seqno, 0)) + 1 )) 
	                                             + Convert(varchar(2), MAX(ISNULL(Elt_Seqno, 0)) + 1 )
                                                 , ISNULL(Ell1.Ell_DayCode, Ell2.Ell_DayCode)
	                                             , ISNULL(Ell1.Ell_Restday, Ell2.Ell_Restday)
	                                             , ISNULL(Ell1.Ell_Holiday, Ell2.Ell_Holiday)
	                                             , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
	                                             , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
	                                             , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
	                                             , @Usr_Login
	                                             , GETDATE()
                                              FROM (SELECT '{0}' [TEMPCOL]) AS TEMP
                                              LEFT JOIN T_EmployeeLogTrail
                                                ON Elt_EmployeeId = [TEMPCOL]
                                               AND Elt_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedger ELL1
                                                ON ELL1.Ell_EmployeeId = [TEMPCOL]
                                               AND ELL1.Ell_ProcessDate = @Ell_ProcessDate
                                              LEFT JOIN T_EmployeeLogLedgerHist Ell2
                                                ON Ell2.Ell_EmployeeId = [TEMPCOL]
                                               AND Ell2.Ell_ProcessDate  = @Ell_ProcessDate
                                             GROUP BY [TEMPCOL]
		                                            , ISNULL(Ell1.Ell_ShiftCode, Ell2.Ell_ShiftCode)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_1, Ell2.Ell_ActualTimeIn_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_1, Ell2.Ell_ActualTimeOut_1)
		                                            , ISNULL(Ell1.Ell_ActualTimeIn_2, Ell2.Ell_ActualTimeIn_2)
		                                            , ISNULL(Ell1.Ell_ActualTimeOut_2, Ell2.Ell_ActualTimeOut_2)
                                                    , ISNULL(Ell1.Ell_DayCode, Ell2.Ell_DayCode)
	                                                , ISNULL(Ell1.Ell_Restday, Ell2.Ell_Restday)
	                                                , ISNULL(Ell1.Ell_Holiday, Ell2.Ell_Holiday)", dr["Trm_EmployeeId"].ToString());
            }
            #endregion

            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(dr["Trm_EmployeeId"].ToString()
                                        , MethodsLibrary.Methods.getPayPeriod(dr["Trm_ModDate"].ToString())
                                        , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(dr["Trm_EmployeeId"].ToString()
                                                , MethodsLibrary.Methods.getPayPeriod(dr["Trm_ModDate"].ToString())
                                                , dal);
                    }
                }
            }
            dal.ExecuteNonQuery(sqlInsertLogTrail, CommandType.Text, paramInfo);
            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
        }

        public void UpdateTagTimeMod(string ControlNo, string flag, string UserLoggedIn, DALHelper dal)
        {
            string sql = @"  UPDATE T_EmployeeLogLedger
                                SET Ell_TagTimeMod = @Flag
                               FROM T_EmployeeLogLedger
                              INNER JOIN T_TimeRecMod
                                 ON Trm_ModDate = Ell_ProcessDate
                                AND Trm_EmployeeId = Ell_EmployeeId
                                AND Trm_ControlNo = @ControlNo

                             UPDATE T_EmployeeLogLedgerHist
                                SET Ell_TagTimeMod = @Flag
                               FROM T_EmployeeLogLedgerHist
                              INNER JOIN T_TimeRecMod
                                 ON Trm_ModDate = Ell_ProcessDate
                                AND Trm_EmployeeId = Ell_EmployeeId
                                AND Trm_ControlNo = @ControlNo";

            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@ControlNo", ControlNo);
            param[1] = new ParameterInfo("@UserLogin", UserLoggedIn);
            param[2] = new ParameterInfo("@Flag", flag);

            dal.ExecuteNonQuery(sql, CommandType.Text, param);

        }
    }
}
