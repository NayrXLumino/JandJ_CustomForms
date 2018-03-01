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
/// Summary description for StraightWorkBL
/// </summary>

namespace Payroll.DAL
{
    public class StraightWorkBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public StraightWorkBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DataRow getStraightWorkInfo(string controlNo, DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Swt_ControlNo
                                  , Swt_CurrentpayPeriod
                                  , Swt_EmployeeId
                                  , Swt_FromDate
                                  , Swt_ToDate
                                  , Swt_ShiftCode
                                  , Swt_UnpaidBreak
                                  , Swt_Reason
                                  , Swt_EndorsedDateToChecker
                                  , Swt_CheckedBy
                                  , Swt_CheckedDate
                                  , Swt_Checked2By
                                  , Swt_Checked2Date
                                  , Swt_ApprovedBy
                                  , Swt_ApprovedDate
                                  , Swt_Status
                                  , Swt_PayPeriodFlag
                                  , Swt_Costcenter
                                  , Swt_Filler1
                                  , Swt_Filler2
                                  , Swt_Filler3
                                  , Swt_BatchNo
                                  , Usr_Login
                                  , Ludatetime
                               FROM T_EmployeeStraightWork
                              WHERE Swt_ControlNo = @ControlNo";
            return dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0].Rows[0];
        }

        public DataRow getStraightWorkInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Swt_ControlNo [Control No]
                                  , Swt_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Swt_FromDate [From Date]
                                  , Swt_ToDate [To Date]
                                  , Swt_ShiftCode [Shift Code]
                                  , Scm_ShiftDesc [Shift Desc]
                                  , '['+Swt_ShiftCode+'] '
			                              + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
			                              + '-'
			                              + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
			                              + '  ' 
			                              + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
			                              + '-'
			                              + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) 
		                            [Shift]
                                  , Swt_UnpaidBreak [Unpaid Break]
                                  , Swt_Reason [Reason]
                                  , AD2.Adt_AccountDesc [@Swt_Filler1Desc]
                                  , AD3.Adt_AccountDesc [@Swt_Filler2Desc]
                                  , AD4.Adt_AccountDesc [@Swt_Filler3Desc]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
                               FROM T_EmployeeStraightWork
                               LEFT JOIN T_EmployeeMaster 
                                 ON Emt_EmployeeId = Swt_EmployeeId
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Swt_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail AD2
                                 ON AD2.Adt_AccountCode = Swt_Filler1
                                AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler01')
                               LEFT JOIN T_AccountDetail AD3
                                 ON AD3.Adt_AccountCode = Swt_Filler2
                                AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler02')
                               LEFT JOIN T_AccountDetail AD4
                                 ON AD4.Adt_AccountCode = Swt_Filler3
                                AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Swt_Filler03')
                               LEFT JOIN T_PayPeriodMaster 
                                 ON Ppm_PayPeriod = Swt_CurrentPayPeriod
                               LEFT JOIN T_ShiftCodeMaster
                                 ON Scm_ShiftCode = Swt_ShiftCode
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Swt_ControlNo
                              WHERE Swt_ControlNo = @ControlNo";
            sql = sql.Replace("@Swt_Filler1Desc", CommonMethods.getFillerName("Swt_Filler01"));
            sql = sql.Replace("@Swt_Filler2Desc", CommonMethods.getFillerName("Swt_Filler02"));
            sql = sql.Replace("@Swt_Filler3Desc", CommonMethods.getFillerName("Swt_Filler03"));
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

        public string ComputeStraightWorkFlag(string ProcessDate)
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

        //Saving Function
        public void CreateSWRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[16];
            paramDetails[0] = new ParameterInfo("@Swt_CurrentPayPeriod", rowDetails["Swt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Swt_EmployeeId", rowDetails["Swt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Swt_FromDate", rowDetails["Swt_FromDate"]);
            paramDetails[3] = new ParameterInfo("@Swt_ToDate", rowDetails["Swt_ToDate"]);
            paramDetails[4] = new ParameterInfo("@Swt_ShiftCode", rowDetails["Swt_ShiftCode"]);
            paramDetails[5] = new ParameterInfo("@Swt_UnpaidBreak", rowDetails["Swt_UnpaidBreak"]);
            paramDetails[6] = new ParameterInfo("@Swt_Reason", rowDetails["Swt_Reason"]);
            paramDetails[7] = new ParameterInfo("@Swt_Status", rowDetails["Swt_Status"]);
            paramDetails[8] = new ParameterInfo("@Swt_ControlNo", rowDetails["Swt_ControlNo"]);
            paramDetails[9] = new ParameterInfo("@Swt_PayPeriodFlag", rowDetails["Swt_PayPeriodFlag"]);
            paramDetails[10] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[11] = new ParameterInfo("@Swt_Filler1", rowDetails["Swt_Filler1"]);
            paramDetails[12] = new ParameterInfo("@Swt_Filler2", rowDetails["Swt_Filler2"]);
            paramDetails[13] = new ParameterInfo("@Swt_Filler3", rowDetails["Swt_Filler3"]);
            paramDetails[14] = new ParameterInfo("@Swt_BatchNo", rowDetails["Swt_BatchNo"]);
            paramDetails[15] = new ParameterInfo("@Swt_EndorsedDateToChecker", rowDetails["Swt_EndorsedDateToChecker"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                DECLARE @Swt_Costcenter as varchar(10)
                                set @Swt_Costcenter = (SELECT Emt_CostcenterCode FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Swt_EmployeeId)

                               INSERT INTO T_EmployeeStraightWork
                                (     Swt_CurrentPayPeriod
                                    , Swt_EmployeeId
                                    , Swt_FromDate
                                    , Swt_ToDate
                                    , Swt_ShiftCode
                                    , Swt_UnpaidBreak
                                    , Swt_AppliedDate
                                    , Swt_Reason
                                    , Swt_Status
                                    , Swt_ControlNo
                                    , Swt_PayPeriodFlag
                                    , Swt_Costcenter
                                    , Usr_Login
                                    , Ludatetime
                                    , Swt_Filler1
                                    , Swt_Filler2
                                    , Swt_Filler3
                                    , Swt_BatchNo
                                    , Swt_EndorsedDateToChecker
                                )
                                VALUES
                                (
                                      @Swt_CurrentPayPeriod
                                    , @Swt_EmployeeId
                                    , @Swt_FromDate
                                    , @Swt_ToDate
                                    , @Swt_ShiftCode
                                    , @Swt_UnpaidBreak
                                    , GETDATE()
                                    , @Swt_Reason
                                    , @Swt_Status
                                    , @Swt_ControlNo
                                    , @Swt_PayPeriodFlag
                                    , @Swt_Costcenter
                                    , @Usr_Login
                                    , GETDATE()
                                    , @Swt_Filler1
                                    , @Swt_Filler2
                                    , @Swt_Filler3
                                    , @Swt_BatchNo
                                    , @Swt_EndorsedDateToChecker
                                )";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void CreateEndorseSWRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[16];
            paramDetails[0] = new ParameterInfo("@Swt_CurrentPayPeriod", rowDetails["Swt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Swt_EmployeeId", rowDetails["Swt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Swt_FromDate", rowDetails["Swt_FromDate"]);
            paramDetails[3] = new ParameterInfo("@Swt_ToDate", rowDetails["Swt_ToDate"]);
            paramDetails[4] = new ParameterInfo("@Swt_ShiftCode", rowDetails["Swt_ShiftCode"]);
            paramDetails[5] = new ParameterInfo("@Swt_UnpaidBreak", rowDetails["Swt_UnpaidBreak"]);
            paramDetails[6] = new ParameterInfo("@Swt_Reason", rowDetails["Swt_Reason"]);
            paramDetails[7] = new ParameterInfo("@Swt_Status", rowDetails["Swt_Status"]);
            paramDetails[8] = new ParameterInfo("@Swt_ControlNo", rowDetails["Swt_ControlNo"]);
            paramDetails[9] = new ParameterInfo("@Swt_PayPeriodFlag", rowDetails["Swt_PayPeriodFlag"]);
            paramDetails[10] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[11] = new ParameterInfo("@Swt_Filler1", rowDetails["Swt_Filler1"]);
            paramDetails[12] = new ParameterInfo("@Swt_Filler2", rowDetails["Swt_Filler2"]);
            paramDetails[13] = new ParameterInfo("@Swt_Filler3", rowDetails["Swt_Filler3"]);
            paramDetails[14] = new ParameterInfo("@Swt_BatchNo", rowDetails["Swt_BatchNo"]);
            paramDetails[15] = new ParameterInfo("@Swt_EndorsedDateToChecker", rowDetails["Swt_EndorsedDateToChecker"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for filler insertion
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT Emt_CostcenterCode FROM T_EmployeeMaster
				                                   WHERE Emt_EmployeeID = @Swt_EmployeeId)

                                INSERT INTO T_EmployeeStraightWork
                                (
                                      Swt_CurrentPayPeriod
                                    , Swt_EmployeeId
                                    , Swt_FromDate
                                    , Swt_ToDate
                                    , Swt_UnpaidBreak
                                    , Swt_AppliedDate
                                    , Swt_Reason
                                    , Swt_Status
                                    , Swt_ControlNo
                                    , Swt_PayPeriodFlag
                                    , Swt_Costcenter
                                    , Usr_Login
                                    , Ludatetime
                                    , Swt_Filler1
                                    , Swt_Filler2
                                    , Swt_Filler3
                                    , Swt_BatchNo
                                    , Swt_EndorsedDateToChecker
                                )
                                VALUES
                                (
                                      @Swt_CurrentPayPeriod
                                    , @Swt_EmployeeId
                                    , Swt_FromDate
                                    , Swt_ToDate
                                    , Swt_UnpaidBreak
                                    , GETDATE()
                                    , @Swt_Reason
                                    , @Swt_Status
                                    , @Swt_ControlNo
                                    , @Swt_PayPeriodFlag
                                    , @Costcenter
                                    , @Usr_Login
                                    , GETDATE()
                                    , @Swt_Filler1
                                    , @Swt_Filler2
                                    , @Swt_Filler3
                                    , @Swt_BatchNo
                                    , GETDATE()
                                )";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateSWRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[16];
            paramDetails[0] = new ParameterInfo("@Swt_CurrentPayPeriod", rowDetails["Swt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Swt_EmployeeId", rowDetails["Swt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Swt_FromDate", rowDetails["Swt_FromDate"]);
            paramDetails[3] = new ParameterInfo("@Swt_ToDate", rowDetails["Swt_ToDate"]);
            paramDetails[4] = new ParameterInfo("@Swt_ShiftCode", rowDetails["Swt_ShiftCode"]);
            paramDetails[5] = new ParameterInfo("@Swt_UnpaidBreak", rowDetails["Swt_UnpaidBreak"]);
            paramDetails[6] = new ParameterInfo("@Swt_Reason", rowDetails["Swt_Reason"]);
            paramDetails[7] = new ParameterInfo("@Swt_Status", rowDetails["Swt_Status"]);
            paramDetails[8] = new ParameterInfo("@Swt_ControlNo", rowDetails["Swt_ControlNo"]);
            paramDetails[9] = new ParameterInfo("@Swt_PayPeriodFlag", rowDetails["Swt_PayPeriodFlag"]);
            paramDetails[10] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Andre added for filler 20100513
            paramDetails[11] = new ParameterInfo("@Swt_Filler1", rowDetails["Swt_Filler1"]);
            paramDetails[12] = new ParameterInfo("@Swt_Filler2", rowDetails["Swt_Filler2"]);
            paramDetails[13] = new ParameterInfo("@Swt_Filler3", rowDetails["Swt_Filler3"]);
            paramDetails[14] = new ParameterInfo("@Swt_BatchNo", rowDetails["Swt_BatchNo"]);
            paramDetails[15] = new ParameterInfo("@Swt_EndorsedDateToChecker", rowDetails["Swt_EndorsedDateToChecker"]);
            #endregion

            #region SQL Query
            //Andre 20100513 updated for Swt_Filler
            string sqlInsert = @"
                               UPDATE T_EmployeeStraightWork
                                  SET Swt_CurrentPayPeriod = @Swt_CurrentPayPeriod
                                    , Swt_FromDate = @Swt_FromDate
                                    , Swt_ToDate = @Swt_ToDate
                                    , Swt_UnpaidBreak = @Swt_UnpaidBreak
                                    , Swt_Reason = @Swt_Reason
                                    , Swt_Status = @Swt_Status
                                    , Swt_PayPeriodFlag = @Swt_PayPeriodFlag
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = GETDATE()
                                    , Swt_Filler1 = @Swt_Filler1
                                    , Swt_Filler2 = @Swt_Filler2
                                    , Swt_Filler3 = @Swt_Filler3
                                WHERE Swt_ControlNo = @Swt_ControlNo ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateSWRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateSWRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateSWRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Swt_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Swt_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Swt_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Swt_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Swt_CheckedBy", " ");
                }
                else if (rowDetails["Swt_Status"].ToString().Equals("5")
                    || rowDetails["Swt_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Swt_CheckedBy = @Swt_CheckedBy ,
                                Swt_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Swt_CheckedBy", rowDetails["Swt_CheckedBy"]);
                }
                else if (rowDetails["Swt_Status"].ToString().Equals("7")
                    || rowDetails["Swt_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Swt_Checked2By = @Swt_Checked2By ,
                                Swt_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Swt_Checked2By", rowDetails["Swt_Checked2By"]);
                }
                else if (rowDetails["Swt_Status"].ToString().Equals("9")
                    || rowDetails["Swt_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Swt_ApprovedBy = @Swt_ApprovedBy ,
                                Swt_ApprovedDate = GETDATE() ,";
                    paramDetails[0] = new ParameterInfo("@Swt_ApprovedBy", rowDetails["Swt_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Swt_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Swt_Status", rowDetails["Swt_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Swt_ControlNo", rowDetails["Swt_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_EmployeeStraightWork
                                    SET
                                        {0}
                                        Swt_Status = @Swt_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = GETDATE()
                                    WHERE Swt_ControlNo = @Swt_ControlNo", fieldUsed);
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

        public void EmployeeLogLedgerUpdate(string controlNo, string userLogged, DALHelper dal)
        {
            DataRow dr = getStraightWorkInfo(controlNo, dal);
            ParameterInfo[] paramInfo = new ParameterInfo[10];
            paramInfo[0] = new ParameterInfo("@EmployeeID", dr["Swt_EmployeeId"]);
            paramInfo[1] = new ParameterInfo("@Ell_ShiftCode", dr["Swt_ShiftCode"]);
            paramInfo[2] = new ParameterInfo("@FromDate", dr["Swt_FromDate"]);
            paramInfo[3] = new ParameterInfo("@ToDate", dr["Swt_ToDate"]);
            paramInfo[4] = new ParameterInfo("@UnpaidBreak", dr["Swt_UnpaidBreak"], SqlDbType.Int);
            paramInfo[5] = new ParameterInfo("@Usr_Login", userLogged);
            paramInfo[6] = new ParameterInfo("@Eot_CurrentPayPeriod", dr["Swt_CurrentPayPeriod"]);
            paramInfo[7] = new ParameterInfo("@Eot_Costcenter", dr["Swt_Costcenter"]);
            paramInfo[8] = new ParameterInfo("@Eot_OvertimeFlag", dr["Swt_PayPeriodFlag"]);
            paramInfo[9] = new ParameterInfo("@dbPrefix", session["dbPrefix"].ToString());

            string sql = @"
                declare @date datetime
                declare @shiftIn char(4)
                declare @shiftOut char (4)           

                set @date = @FromDate      
                set @shiftIn = (SELECT Scm_ShiftTimeIn FROM T_ShiftCodeMaster WHERE Scm_ShiftCode = @Ell_ShiftCode)    
                set @shiftOut = (SELECT Scm_ShiftTimeOut FROM T_ShiftCodeMaster WHERE Scm_ShiftCode = @Ell_ShiftCode)    

                declare @outPM char(4)
                set @outPM = (SELECT CAST(LEFT(@shiftIn, 2) + 24 AS CHAR(2)) +  RIGHT(@shiftIn, 2))

                declare @days as int
                set @days = (SELECT CAST(@outPM as int) / 2400)

                declare @timeOnly as char(4)
                set @timeOnly = (SELECT '0' + CAST(CAST(@outPM as int) % 2400 as CHAR(4)))

                declare @ComputedTime time
                set @ComputedTime = (SELECT DATEADD(mi, -@UnpaidBreak, CAST(LEFT(@timeOnly, 2) + ':' + RIGHT(@timeOnly, 2) as time)))
                
                declare @OTEndTime char(4)
                set @OTEndTime = (SELECT LEFT(@ComputedTime, 2) + SUBSTRING(CAST(@ComputedTime as char(5)), 4, 2) + (@days * 2400))

                declare @OTHours decimal(5,2)
                set @OTHours = CAST(LEFT(@OTEndTime ,2) as int) - CAST(LEFT(@shiftOut, 2) as int)
                        + SUBSTRING( CAST( CAST( RIGHT(CAST( CAST(@OTEndTime as int) - CAST(@shiftOut as int) as CHAR(4)), 2)  as float) / 60 AS CHAR(4)), 2, 3)
                
                DECLARE @yr2Digit varchar(2)
                DECLARE @ControlNo varchar(12)

                WHILE (@date <= @ToDate) 
                BEGIN
                    IF (@date = @FromDate)
                        UPDATE T_EmployeeLogLedger 
                        SET Ell_ActualTimeOut_2 = @outPM
                            , Usr_Login = @Usr_Login
                            , Ludatetime = GETDATE()
                            WHERE Ell_EmployeeId = @EmployeeID AND Ell_ProcessDate = @date
                    ELSE IF(@date = @ToDate)
                        UPDATE T_EmployeeLogLedger 
                        SET Ell_ActualTimeIn_1 = @shiftIn
                            , Usr_Login = @Usr_Login
                            , Ludatetime = GETDATE()
                            WHERE Ell_EmployeeId = @EmployeeID AND Ell_ProcessDate = @date            
                    ELSE
                        UPDATE T_EmployeeLogLedger 
                        SET 
                            Ell_ActualTimeIn_1 = @shiftIn
                            , Ell_ActualTimeOut_2 = @outPM
                            , Usr_Login = @Usr_Login
                            , Ludatetime = GETDATE()
                            WHERE Ell_EmployeeId = @EmployeeID AND Ell_ProcessDate = @date
                    
                    
                    SET @yr2Digit = (SELECT right(Ccd_CurrentYear, 2) from T_CompanyMaster)

                    SET @ControlNo = (  SELECT Tcm_TransactionPrefix 
                                            + @dbPrefix
	                                        + @yr2Digit 
	                                        + replicate('0', 8 - len(RTrim(Tcm_LastSeries)))
	                                        + RTrim(Tcm_LastSeries)
                                        FROM T_TransactionControlMaster
                                        WITH (UPDLOCK)
                                        WHERE Tcm_TransactionCode = 'OVERTIME')

                    UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = 'OVERTIME'

                    INSERT INTO T_EmployeeOvertime
                       ([Eot_CurrentPayPeriod]
                       ,[Eot_EmployeeId]
                       ,[Eot_OvertimeDate]
                       ,[Eot_Seqno]
                       ,[Eot_AppliedDate]
                       ,[Eot_OvertimeType]
                       ,[Eot_StartTime]
                       ,[Eot_EndTime]
                       ,[Eot_OvertimeHour]
                       ,[Eot_Reason]
                       ,[Eot_JobCode]
                       ,[Eot_ClientJobNo]
                       ,[Eot_EndorsedDateToChecker]
                       ,[Eot_CheckedBy]
                       ,[Eot_CheckedDate]
                       ,[Eot_Checked2By]
                       ,[Eot_Checked2Date]
                       ,[Eot_ApprovedBy]
                       ,[Eot_ApprovedDate]
                       ,[Eot_Status]
                       ,[Eot_ControlNo]
                       ,[Eot_OvertimeFlag]
                       ,[Eot_Costcenter]
                       ,[Eot_Filler1]
                       ,[Eot_Filler2]
                       ,[Eot_Filler3]
                       ,[Eot_BatchNo]
                       ,[Usr_Login]
                       ,[Ludatetime])
                 VALUES
                       (@Eot_CurrentPayPeriod
                       ,@EmployeeID
                       ,@date
                       ,'01'
                       ,GETDATE()
                       ,'P'
                       ,@shiftOut
                       ,@OTEndTime
                       ,@OTHours
                       ,'STRAIGHT WORK'
                       ,''
                       ,''
                       ,NULL
                       ,NULL
                       ,NULL
                       ,NULL
                       ,NULL
                       ,@Usr_Login
                       ,GETDATE()
                       ,'9'
                       ,@ControlNo
                       ,@Eot_OvertimeFlag
                       ,@Eot_Costcenter
                       ,''
                       ,''
                       ,''
                       ,''
                       ,@Usr_Login
                       ,GETDATE()
                        )
                        
                    set @date = @date + 1
                END";

            dal.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
        }
        public string extended(DataRow dr, bool isPrevCheck,DALHelper dal)
        {
            //if found extensions returns the shift
            string shift = "";
            ParameterInfo[] paramInfo = new ParameterInfo[20];
            string controlnum = dr["Swt_ControlNo"].ToString();
            string employeeid =dr["Swt_EmployeeId"].ToString();
            try
            {
                string query="";
                if (isPrevCheck)
                    query = @"SELECT Swt_ShiftCode
	                                FROM T_EmployeeStraightwork
                                WHERE SWT_TODATE = (SELECT DATEADD(DAY,-1,Swt_FromDate)
						                                FROM  T_EmployeeStraightwork
					                                WHERE Swt_ControlNo='{0}')
	                                AND Swt_EmployeeID='{1}'
	                                AND Swt_Status =9";
                else
                    query = @"SELECT Swt_ShiftCode
	                                FROM T_EmployeeStraightwork
                                WHERE Swt_FromDate = (SELECT DATEADD(DAY,1,Swt_ToDate)
						                                FROM  T_EmployeeStraightwork
					                                WHERE Swt_ControlNo='{0}')
	                                AND Swt_EmployeeID='{1}'
	                                AND Swt_Status =9";
                shift = dal.ExecuteScalar(string.Format(query, controlnum, employeeid)).ToString();
            }
            catch 
            { 
            }
            return shift;
        }
        public void EmployeeLogLedgerAndOvertimeUpdate(string controlNo, string userLogged, DALHelper dal)
        {//try
            DataRow dr = getStraightWorkInfo(controlNo, dal);
            DateTime fromDate = Convert.ToDateTime(dr["Swt_FromDate"]);
            DateTime toDate = Convert.ToDateTime(dr["Swt_ToDate"]);
            
            DateTime date = fromDate;

            
            ParameterInfo[] paramInfo = new ParameterInfo[20];
            paramInfo[0] = new ParameterInfo("@FromDate", fromDate);
            paramInfo[1] = new ParameterInfo("@ToDate", toDate);
            paramInfo[2] = new ParameterInfo("@EmployeeID", dr["Swt_EmployeeId"]);
            paramInfo[3] = new ParameterInfo("@ShiftCode", dr["Swt_ShiftCode"]);
            paramInfo[4] = new ParameterInfo("@Usr_Login", userLogged);

            paramInfo[5] = new ParameterInfo("@Eot_CurrentPayPeriod", dr["Swt_CurrentPayPeriod"]);
            paramInfo[6] = new ParameterInfo("@Eot_OvertimeFlag", dr["Swt_PayPeriodFlag"]);
            paramInfo[7] = new ParameterInfo("@Eot_Costcenter", dr["Swt_Costcenter"]);
            paramInfo[8] = new ParameterInfo("@UnpaidBreak", dr["Swt_UnpaidBreak"], SqlDbType.Int);
            paramInfo[9] = new ParameterInfo("@Eot_EndorsedDateToChecker", dr["Swt_EndorsedDateToChecker"]);
            paramInfo[10] = new ParameterInfo("@Eot_CheckedBy", dr["Swt_CheckedBy"]);
            paramInfo[11] = new ParameterInfo("@Eot_CheckedDate", dr["Swt_CheckedDate"]);
            paramInfo[12] = new ParameterInfo("@Eot_Checked2By", dr["Swt_Checked2By"]);
            paramInfo[13] = new ParameterInfo("@Eot_Checked2Date", dr["Swt_Checked2Date"]);
            paramInfo[14] = new ParameterInfo("@Eot_ApprovedBy", dr["Swt_ApprovedBy"]);
            paramInfo[15] = new ParameterInfo("@Eot_ApprovedDate", dr["Swt_ApprovedDate"]);
            paramInfo[16] = new ParameterInfo("@SWControlNo", controlNo);
            paramInfo[17] = new ParameterInfo("@Eot_Reason", controlNo + ": " + dr["Swt_Reason"]);

            //newly added for straightwork continuous ROBERT
            DateTime dateExtended=DateTime.Parse("01/01/0001");;
            DateTime dateExtended2=DateTime.Parse("01/01/0001");;
            string shiftPrev = extended(dr, true, dal);//returns the shift of the previous transaction connected to current
            string shiftFuture = extended(dr, false, dal);//returns the shift of the future transaction connected to current
            //hold the real shift.. can be disregarded
            string shiftholder = dr["Swt_ShiftCode"].ToString();
            if (shiftPrev.Trim() != "")
            {
                dateExtended = date = date.AddDays(-1);
                paramInfo[0] = new ParameterInfo("@FromDate", date);
            }

            if (shiftFuture.Trim() != "")
            {
                dateExtended2 = toDate = toDate.AddDays(1);
                paramInfo[1] = new ParameterInfo("@ToDate", toDate);
            }
            

            while (date <= toDate)
            {
                paramInfo[18] = new ParameterInfo("@date", date);
                paramInfo[19] = new ParameterInfo("@OTControlNo", CommonMethods.GetControlNumber("OVERTIME"));
                
                #region EmployeeLogLedger
                if (date.ToString() == dateExtended.ToString() || date.ToString() == dateExtended2.ToString())
                {
                    if (shiftPrev.Trim() != "")
                    {
                        paramInfo[3] = new ParameterInfo("@ShiftCode", shiftPrev.Trim());
                        shiftPrev = "";
                    }
                    else if (shiftFuture.Trim() != "")
                    {
                        paramInfo[3] = new ParameterInfo("@ShiftCode", shiftFuture.Trim());
                        shiftFuture = "";
                    }
                }
                else
                    paramInfo[3] = new ParameterInfo("@ShiftCode", dr["Swt_ShiftCode"]);
                

                string LedgerTable = GetLedgerTableFromDate(date.ToString());

                dal.ExecuteNonQuery(string.Format(
                    @"
                    declare @shiftIn char(4)
                    declare @shiftOut char(4)           
     
                    set @shiftIn = (SELECT Scm_ShiftTimeIn FROM T_ShiftCodeMaster WHERE Scm_ShiftCode = @ShiftCode)    
                    set @shiftOut = (SELECT Scm_ShiftTimeOut FROM T_ShiftCodeMaster WHERE Scm_ShiftCode = @ShiftCode)    
                    
                    IF(CAST(@shiftIn as int) > CAST(@shiftOut as int))
                        set @shiftOut = CAST(@shiftOut as INT) + 2400
                    
                    declare @outPM char(4)
                    set @outPM = (SELECT CAST(LEFT(@shiftIn, 2) + 24 AS CHAR(2)) +  RIGHT(@shiftIn, 2))

                    declare @days as int
                    set @days = (SELECT CAST(@outPM as int) / 2400)
 
                    declare @timeOnly as char(4)
                    set @timeOnly = RIGHT('000' + CAST( CAST(@outPM as int) % 2400 as varchar(4)), 4)

                    declare @ComputedTime time
                    set @ComputedTime = (SELECT DATEADD(mi, -@UnpaidBreak, CAST(LEFT(@timeOnly, 2) + ':' + RIGHT(@timeOnly, 2) as time)))
                
                    declare @OTEndTime char(4)
                    set @OTEndTime = (SELECT LEFT(@ComputedTime, 2) + SUBSTRING(CAST(@ComputedTime as char(5)), 4, 2) + (@days * 2400))
                    
                    declare @DayCode varchar(4)
                    set @DayCode = (SELECT Ell_DayCode From {0} WHERE Ell_ProcessDate = @date AND Ell_EmployeeID = @EmployeeID)

                    declare @OTHours decimal(5,2)
                    IF(@DayCode = 'REG')
                        set @OTHours = CAST(LEFT(@OTEndTime ,2) as float) - CAST(LEFT(@shiftOut, 2) as float)
                            + SUBSTRING( CAST( CAST( RIGHT(CAST( CAST(@OTEndTime as int) - CAST(@shiftOut as int) as CHAR(4)), 2)  as float) / 60 AS CHAR(4)), 2, 3)
                    ELSE
                        set @OTHours = CAST(LEFT(@OTEndTime ,2) as float) - CAST(LEFT(@shiftIn, 2) as float)
                            + SUBSTRING( CAST( CAST( RIGHT(CAST( CAST(@OTEndTime as int) - CAST(@shiftIn as int) as CHAR(4)), 2)  as float) / 60 AS CHAR(4)), 2, 3)               

                    UPDATE {0}
                    SET 
                          Ell_ActualTimeIn_1 = 
                                CASE 
                                    WHEN @date = @FromDate THEN Ell_ActualTimeIn_1
                                    ELSE @shiftIn
                                END
                        , Ell_ActualTimeOut_1 = 
                                CASE 
                                    WHEN @date <> @FromDate AND @date <> @ToDate THEN '0000'
                                    ELSE Ell_ActualTimeOut_1
                                END
                        , Ell_ActualTimeIn_2 = 
                                CASE 
                                    WHEN @date <> @FromDate AND @date <> @ToDate THEN '0000'
                                    ELSE Ell_ActualTimeIn_2
                                END
                        , Ell_ActualTimeOut_2 = 
                                CASE 
                                    WHEN @date = @ToDate THEN Ell_ActualTimeOut_2
                                    ELSE @outPM
                                END
                        , Ell_EncodedOvertimePostHr = @OTHours
                        , Ell_ShiftCode = @ShiftCode
                        , Usr_Login = @Usr_Login
                        , Ludatetime = GETDATE()
                        WHERE Ell_EmployeeId = @EmployeeID AND Ell_ProcessDate = @date

                    declare @conflictOTControlNo varchar(12)

                    DECLARE conflict_cursor CURSOR FOR
                    SELECT Eot_ControlNo FROM T_EmployeeOvertime 
                        WHERE Eot_EmployeeId = @EmployeeID 
                        AND Eot_OvertimeDate = @date 
                        AND Eot_Status IN ('1','3','5','7','9') 
                        AND (  (Eot_OvertimeDate = @fromDate AND Eot_OvertimeType = 'P')
			                OR (Eot_OvertimeDate > @fromDate AND Eot_OvertimeDate < @ToDate)
			                OR (Eot_OvertimeDate = @ToDate)
			                );
                    
                    OPEN conflict_cursor
					
					FETCH NEXT FROM conflict_cursor INTO @conflictOTControlNo
					
					WHILE @@FETCH_STATUS = 0
                    BEGIN
                        UPDATE T_EmployeeOvertime
                        SET Eot_Status = '2'
                            , Ludatetime = GETDATE()
                            , Usr_Login = @Usr_Login
                        WHERE Eot_ControlNo = @conflictOTControlNo

                        IF EXISTS(SELECT TOP 1 Trm_ControlNo FROM T_TransactionRemarks WHERE Trm_ControlNo = @conflictOTControlNo)
                            UPDATE T_TransactionRemarks
                                    SET Trm_Remarks = 'Another OT is created by Straight Work: ' + @SWControlNo + '. New OT is ' + @OTControlNo
                                       ,Usr_Login = @Usr_Login
                                       ,Ludatetime = GETDATE()
                                WHERE Trm_ControlNo = @conflictOTControlNo
                        ELSE
                            INSERT INTO T_TransactionRemarks
                                       (Trm_ControlNo
                                       ,Trm_Remarks
                                       ,Usr_Login
                                       ,Ludatetime)
                                 VALUES
                                       (@conflictOTControlNo
                                       ,'Another OT is created by Straight Work: ' + @SWControlNo + '. New OT is ' + @OTControlNo
                                       ,@Usr_Login
                                       ,GETDATE()
                                        )

                        FETCH NEXT FROM conflict_cursor INTO @conflictOTControlNo
                    END

                    CLOSE conflict_cursor;
                    DEALLOCATE conflict_cursor

                    INSERT INTO T_EmployeeOvertime
                           ([Eot_CurrentPayPeriod]
                           ,[Eot_EmployeeId]
                           ,[Eot_OvertimeDate]
                           ,[Eot_Seqno]
                           ,[Eot_AppliedDate]
                           ,[Eot_OvertimeType]
                           ,[Eot_StartTime]
                           ,[Eot_EndTime]
                           ,[Eot_OvertimeHour]
                           ,[Eot_Reason]
                           ,[Eot_JobCode]
                           ,[Eot_ClientJobNo]
                           ,[Eot_EndorsedDateToChecker]
                           ,[Eot_CheckedBy]
                           ,[Eot_CheckedDate]
                           ,[Eot_Checked2By]
                           ,[Eot_Checked2Date]
                           ,[Eot_ApprovedBy]
                           ,[Eot_ApprovedDate]
                           ,[Eot_Status]
                           ,[Eot_ControlNo]
                           ,[Eot_OvertimeFlag]
                           ,[Eot_Costcenter]
                           ,[Eot_Filler1]
                           ,[Eot_Filler2]
                           ,[Eot_Filler3]
                           ,[Eot_BatchNo]
                           ,[Usr_Login]
                           ,[Ludatetime])
                   VALUES
                           (@Eot_CurrentPayPeriod
                           ,@EmployeeID
                           ,@date
                           ,'01'
                           ,GETDATE()
                           ,'P'
                           , CASE @DayCode
                                WHEN 'REG' THEN @shiftOut
                                ELSE @shiftIn
                             END
                           , CASE @date 
		                        WHEN @ToDate THEN '2400'
		                        ELSE @OTEndTime
	                         END
                           ,@OTHours
                           ,SUBSTRING(@Eot_Reason, 1, 200)
                           ,''
                           ,''
                           ,NULL
                           ,NULL
                           ,NULL
                           ,NULL
                           ,NULL
                           ,@Usr_Login
                           ,GETDATE()
                           ,'9'
                           ,@OTControlNo
                           ,@Eot_OvertimeFlag
                           ,@Eot_Costcenter
                           ,''
                           ,''
                           ,''
                           ,''
                           ,@Usr_Login
                           ,GETDATE()
                            )

                    INSERT INTO T_TransactionRemarks
                               (Trm_ControlNo
                               ,Trm_Remarks
                               ,Usr_Login
                               ,Ludatetime)
                         VALUES
                               (@OTControlNo
                               ,'STRAIGHT WORK: ' + @SWControlNo
                               ,@Usr_Login
                               ,GETDATE()
                                )", LedgerTable), CommandType.Text, paramInfo);
                #endregion

                if (!LedgerTable.ToString().Equals(string.Empty))
                {
                    if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                    {
                        if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(dr["Swt_EmployeeId"].ToString()
                                                                               , CommonMethods.getPayPeriod(date)
                                                                               , CommonMethods.getPayPeriod("C"), dal))
                        {
                            MethodsLibrary.Methods.InsertLogLedgerTrail(dr["Swt_EmployeeId"].ToString()
                                                    , CommonMethods.getPayPeriod(date)
                                                    , dal);
                        }
                    }
                }

                date = date.AddDays(1);
            }
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
            return LedgerTable;
        }
    }
}