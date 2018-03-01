/* Revision no. W2.1.00002
 * 
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   04/17/2013
 *  Update Notes    :   
 *      -  Comment out the checking snapshot in workgroup and costcenter
 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Payroll.DAL;

/// <summary>
/// Summary description for WorkInformationBL
/// </summary>
namespace Payroll.DAL
{
    public class WorkInformationBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public WorkInformationBL()
        {
            //
            // TODO: Add constructor logic here
            //
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

        public DataRow getMovementInfo(string controlNumber)
        {
            DataSet ds = new DataSet();
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mve_ControlNo", controlNumber);
            #endregion

            #region SQL Query
            string sqlQuery = @" SELECT Mve_ControlNo [Control No]
                                  , Mve_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Convert(varchar(10),Mve_EffectivityDate,101) [Effectivity Date]
                                  , Mve_From [From Code]
                                  , CASE Mve_Type
                                        WHEN 'G' THEN ISNULL(A1.Adt_AccountDesc, '') + ' / ' + ISNULL(A2.Adt_AccountDesc,'') 
                                        WHEN 'S' THEN s0.Scm_ShiftDesc
		                                            --+ '  ('
		                                            --+ REPLICATE(' ', 10 - LEN(s0.Scm_ShiftCode))
		                                            --+ LEFT(s0.Scm_ShiftTimeIn,2) + ':' + RIGHT(s0.Scm_ShiftTimeIn,2)
		                                            --+ ' - '
		                                            --+ LEFT(s0.Scm_ShiftTimeOut,2) + ':' + RIGHT(s0.Scm_ShiftTimeOut,2)
		                                            --+ ')'
                                        WHEN 'C' THEN dbo.getCostCenterFullNameV2(Mve_From)
                                        ELSE Mve_From
                                    END [From Desc]
                                  , Mve_To [To Code]
                                  , CASE Mve_Type
			                            WHEN 'G' THEN ISNULL(A3.Adt_AccountDesc, '') + ' / ' + ISNULL(A4.Adt_AccountDesc,'') 
			                            WHEN 'S' THEN s1.Scm_ShiftDesc
                                                    --+ '  ('
                                                    --+ REPLICATE(' ', 10 - LEN(s1.Scm_ShiftCode))
                                                    --+ LEFT(s1.Scm_ShiftTimeIn,2) + ':' + RIGHT(s1.Scm_ShiftTimeIn,2)
                                                    --+ ' - '
                                                    --+ LEFT(s1.Scm_ShiftTimeOut,2) + ':' + RIGHT(s1.Scm_ShiftTimeOut,2)
                                                    --+ ')'
                                        WHEN 'C' THEN dbo.getCostCenterFullNameV2(Mve_To)
                                        ELSE Mve_To
                                    END [To Desc]
                                  , Mve_Reason [Reason]
                                  , AD0.Adt_AccountDesc [Status]
                                  , ISNULL(Mve_BatchNo, '') [Mve_BatchNo]
                                  , Trm_Remarks [Remarks]
                                  , Mve_Status 
                                  , Mve_Type
                                  , Emt_PayrollType
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_Movement
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Mve_EmployeeId
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Mve_ControlNo
                               LEFT JOIN T_AccountDetail AD0
                                 ON AD0.Adt_AccountCode = Mve_Status
                                AND AD0.Adt_AccountType = 'WFSTATUS'
                               LEFT JOIN T_AccountDetail A1
                                 ON A1.Adt_AccountCode = LTRIM(LEFT(Mve_From,3))
                                AND A1.Adt_AccounttYPE = 'WORKTYPE'
                               LEFT JOIN T_AccountDetail A2
                                 ON A2.Adt_AccountCode = LTRIM(RIGHT(Mve_From,3))
                                AND A2.Adt_AccounttYPE = 'WORKGROUP'
                               LEFT JOIN T_AccountDetail A3
                                 ON A3.Adt_AccountCode = LTRIM(LEFT(Mve_To,3))
                                AND A3.Adt_AccounttYPE = 'WORKTYPE'
                               LEFT JOIN T_AccountDetail A4
                                 ON A4.Adt_AccountCode = LTRIM(RIGHT(Mve_To,3))
                                AND A4.Adt_AccounttYPE = 'WORKGROUP'
                               LEFT JOIN T_ShiftCodeMaster s0
			                        ON s0.Scm_ShiftCode = Mve_From
                               LEFT JOIN T_ShiftCodeMaster s1
                                    ON s1.Scm_ShiftCode = Mve_To
		                        WHERE Mve_ControlNo = @Mve_ControlNo";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
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

            return ds.Tables[0].Rows[0];
        }

        public string ComputeMVFlag(string ProcessDate)
        {
            DataSet ds = new DataSet();

            #region SQL Query
            string sqlQuery = string.Format(@" select case when '{0}' < Ppm_StartCycle
			                                                then 'P'
			                                                when '{0}' <= Ppm_EndCycle
			                                                then 'C'
			                                                else 'F'
	                                                    end from t_payperiodmaster
                                                where ppm_cycleindicator = 'C'", ProcessDate);
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }

        #region [Create Record]
        public void CreateMVRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Mve_CurrentPayPeriod", rowDetails["Mve_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Mve_EmployeeId", rowDetails["Mve_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Mve_EffectivityDate", rowDetails["Mve_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Mve_AppliedDate", rowDetails["Mve_AppliedDate"]);
            paramDetails[5] = new ParameterInfo("@Mve_Type", rowDetails["Mve_Type"]);
            paramDetails[6] = new ParameterInfo("@Mve_From", rowDetails["Mve_From"]);
            paramDetails[7] = new ParameterInfo("@Mve_To", rowDetails["Mve_To"]);
            paramDetails[8] = new ParameterInfo("@Mve_Reason", rowDetails["Mve_Reason"]);
            paramDetails[9] = new ParameterInfo("@Mve_EndorsedDateToChecker", rowDetails["Mve_EndorsedDateToChecker"]);
            paramDetails[10] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[11] = new ParameterInfo("@Mve_BatchNo", rowDetails["Mve_BatchNo"]);
            paramDetails[12] = new ParameterInfo("@Mve_Flag", rowDetails["Mve_Flag"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                declare @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                      AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Mve_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                    AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END
IF NOT EXISTS(SELECT Mve_ControlNo FROM T_Movement
WHERE Mve_CurrentPayPeriod = @Mve_CurrentPayPeriod
AND Mve_EmployeeId = @Mve_EmployeeId
AND Mve_EffectivityDate = @Mve_EffectivityDate
AND Mve_Type = @Mve_Type
AND Mve_From = @Mve_From
AND Mve_To = @Mve_To
AND Mve_Reason = @Mve_Reason
AND Mve_Status = @Mve_Status
AND Mve_Flag = @Mve_Flag)
BEGIN
                                INSERT INTO T_Movement
                                (
                                      Mve_ControlNo
                                    , Mve_CurrentPayPeriod
                                    , Mve_EmployeeId
                                    , Mve_EffectivityDate
                                    , Mve_AppliedDate
                                    , Mve_Costcenter
                                    , Mve_CostcenterLine
                                    , Mve_Type
                                    , Mve_From
                                    , Mve_To
                                    , Mve_Reason
                                    , Mve_EndorsedDateToChecker
                                    , Mve_Status
                                    , Mve_BatchNo
                                    , Mve_Flag
                                    , Usr_Login
                                    , Ludatetime
                                )
                                VALUES
                                (
                                      @Mve_ControlNo
                                    , @Mve_CurrentPayPeriod
                                    , @Mve_EmployeeId
                                    , @Mve_EffectivityDate
                                    , getdate()
                                    , @Costcenter
                                    , @LineCode
                                    , @Mve_Type
                                    , @Mve_From
                                    , @Mve_To
                                    , @Mve_Reason
                                    , @Mve_EndorsedDateToChecker
                                    , @Mve_Status
                                    , @Mve_BatchNo
                                    , @Mve_Flag
                                    , @Usr_Login
                                    , getdate()
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void CreateMVShiftRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Mve_CurrentPayPeriod", rowDetails["Mve_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Mve_EmployeeId", rowDetails["Mve_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Mve_EffectivityDate", rowDetails["Mve_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Mve_AppliedDate", rowDetails["Mve_AppliedDate"]);
            paramDetails[5] = new ParameterInfo("@Mve_Type", rowDetails["Mve_Type"]);
            paramDetails[6] = new ParameterInfo("@Mve_From", rowDetails["Mve_From"]);
            paramDetails[7] = new ParameterInfo("@Mve_To", rowDetails["Mve_To"]);
            paramDetails[8] = new ParameterInfo("@Mve_Reason", rowDetails["Mve_Reason"]);
            paramDetails[9] = new ParameterInfo("@Mve_EndorsedDateToChecker", rowDetails["Mve_EndorsedDateToChecker"]);
            paramDetails[10] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[11] = new ParameterInfo("@Mve_BatchNo", rowDetails["Mve_BatchNo"]);
            paramDetails[12] = new ParameterInfo("@Mve_Flag", rowDetails["Mve_Flag"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                declare @Costcenter as varchar(10)
                                declare @fromShift as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                      AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Mve_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                    AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END

                                set @fromShift = (SELECT CASE WHEN (@Mve_From = '')
                                                              THEN Ell_ShiftCode
                                                              ELSE @Mve_From
                                                          END
                                                    FROM T_EmployeeLogLedger
                                                   WHERE Ell_ProcessDate = @Mve_EffectivityDate
                                                     AND Ell_EmployeeId = @Mve_EmployeeId
                                                   UNION
                                                  SELECT CASE WHEN (@Mve_From = '')
                                                              THEN Ell_ShiftCode
                                                              ELSE @Mve_From
                                                          END
                                                    FROM T_EmployeeLogLedgerHist
                                                   WHERE Ell_ProcessDate = @Mve_EffectivityDate
                                                     AND Ell_EmployeeId = @Mve_EmployeeId)
IF NOT EXISTS(SELECT Mve_ControlNo FROM T_Movement
WHERE Mve_CurrentPayPeriod = @Mve_CurrentPayPeriod
AND Mve_EmployeeId = @Mve_EmployeeId
AND Mve_EffectivityDate = @Mve_EffectivityDate
AND Mve_Type = @Mve_Type
AND Mve_To = @Mve_To
AND Mve_Reason = @Mve_Reason
AND Mve_Status = @Mve_Status
AND Mve_Flag = @Mve_Flag)
BEGIN
                                INSERT INTO T_Movement
                                (
                                      Mve_ControlNo
                                    , Mve_CurrentPayPeriod
                                    , Mve_EmployeeId
                                    , Mve_EffectivityDate
                                    , Mve_AppliedDate
                                    , Mve_Costcenter
                                    , Mve_CostcenterLine
                                    , Mve_Type
                                    , Mve_From
                                    , Mve_To
                                    , Mve_Reason
                                    , Mve_EndorsedDateToChecker
                                    , Mve_Status
                                    , Mve_BatchNo
                                    , Mve_Flag
                                    , Usr_Login
                                    , Ludatetime
                                )
                                VALUES
                                (
                                      @Mve_ControlNo
                                    , @Mve_CurrentPayPeriod
                                    , @Mve_EmployeeId
                                    , @Mve_EffectivityDate
                                    , getdate()
                                    , @Costcenter
                                    , @LineCode
                                    , @Mve_Type
                                    , @fromShift
                                    , @Mve_To
                                    , @Mve_Reason
                                    , @Mve_EndorsedDateToChecker
                                    , @Mve_Status
                                    , @Mve_BatchNo
                                    , @Mve_Flag
                                    , @Usr_Login
                                    , getdate()
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void CreateMVGroupRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Mve_CurrentPayPeriod", rowDetails["Mve_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Mve_EmployeeId", rowDetails["Mve_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Mve_EffectivityDate", rowDetails["Mve_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Mve_AppliedDate", rowDetails["Mve_AppliedDate"]);
            paramDetails[5] = new ParameterInfo("@Mve_Type", rowDetails["Mve_Type"]);
            paramDetails[6] = new ParameterInfo("@Mve_From", rowDetails["Mve_From"]);
            paramDetails[7] = new ParameterInfo("@Mve_To", rowDetails["Mve_To"]);
            paramDetails[8] = new ParameterInfo("@Mve_Reason", rowDetails["Mve_Reason"]);
            paramDetails[9] = new ParameterInfo("@Mve_EndorsedDateToChecker", rowDetails["Mve_EndorsedDateToChecker"]);
            paramDetails[10] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[11] = new ParameterInfo("@Mve_BatchNo", rowDetails["Mve_BatchNo"]);
            paramDetails[12] = new ParameterInfo("@Mve_Flag", rowDetails["Mve_Flag"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                declare @Costcenter as varchar(10)
                                declare @fromGroup as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                      AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Mve_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                    AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END

                                set @fromGroup = (SELECT CASE WHEN (@Mve_From = '')
                                                              THEN REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                                                                 + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)
                                                              ELSE @Mve_From
                                                          END
                                                    FROM T_EmployeeLogLedger
                                                   WHERE Ell_ProcessDate = @Mve_EffectivityDate
                                                     AND Ell_EmployeeId = @Mve_EmployeeId
                                                   UNION
                                                  SELECT CASE WHEN (@Mve_From = '')
                                                              THEN REPLICATE(' ', 3 - LEN(Ell_WorkType)) + RTRIM(Ell_WorkType)
                                                                 + REPLICATE(' ', 3 - LEN(Ell_WorkGroup)) + RTRIM(Ell_WorkGroup)
                                                              ELSE @Mve_From
                                                          END
                                                    FROM T_EmployeeLogLedgerHist
                                                   WHERE Ell_ProcessDate = @Mve_EffectivityDate
                                                     AND Ell_EmployeeId = @Mve_EmployeeId)
IF NOT EXISTS(SELECT Mve_ControlNo FROM T_Movement
WHERE Mve_CurrentPayPeriod = @Mve_CurrentPayPeriod
AND Mve_EmployeeId = @Mve_EmployeeId
AND Mve_EffectivityDate = @Mve_EffectivityDate
AND Mve_Type = @Mve_Type
AND Mve_To = @Mve_To
AND Mve_Reason = @Mve_Reason
AND Mve_Status = @Mve_Status
AND Mve_Flag = @Mve_Flag)
BEGIN
                                INSERT INTO T_Movement
                                (
                                      Mve_ControlNo
                                    , Mve_CurrentPayPeriod
                                    , Mve_EmployeeId
                                    , Mve_EffectivityDate
                                    , Mve_AppliedDate
                                    , Mve_Costcenter
                                    , Mve_CostcenterLine
                                    , Mve_Type
                                    , Mve_From
                                    , Mve_To
                                    , Mve_Reason
                                    , Mve_EndorsedDateToChecker
                                    , Mve_Status
                                    , Mve_BatchNo
                                    , Mve_Flag
                                    , Usr_Login
                                    , Ludatetime
                                )
                                VALUES
                                (
                                      @Mve_ControlNo
                                    , @Mve_CurrentPayPeriod
                                    , @Mve_EmployeeId
                                    , @Mve_EffectivityDate
                                    , getdate()
                                    , @Costcenter
                                    , @LineCode
                                    , @Mve_Type
                                    , @fromGroup
                                    , @Mve_To
                                    , @Mve_Reason
                                    , @Mve_EndorsedDateToChecker
                                    , @Mve_Status
                                    , @Mve_BatchNo
                                    , @Mve_Flag
                                    , @Usr_Login
                                    , getdate()
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void CreateMVCostCenterRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Mve_CurrentPayPeriod", rowDetails["Mve_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Mve_EmployeeId", rowDetails["Mve_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Mve_EffectivityDate", rowDetails["Mve_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Mve_AppliedDate", rowDetails["Mve_AppliedDate"]);
            paramDetails[5] = new ParameterInfo("@Mve_Type", rowDetails["Mve_Type"]);
            paramDetails[6] = new ParameterInfo("@Mve_From", rowDetails["Mve_From"]);
            paramDetails[7] = new ParameterInfo("@Mve_To", rowDetails["Mve_To"]);
            paramDetails[8] = new ParameterInfo("@Mve_Reason", rowDetails["Mve_Reason"]);
            paramDetails[9] = new ParameterInfo("@Mve_EndorsedDateToChecker", rowDetails["Mve_EndorsedDateToChecker"]);
            paramDetails[10] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[11] = new ParameterInfo("@Mve_BatchNo", rowDetails["Mve_BatchNo"]);
            paramDetails[12] = new ParameterInfo("@Mve_Flag", rowDetails["Mve_Flag"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                declare @Costcenter as varchar(10)
                                declare @fromCostCenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                      AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Mve_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Mve_EmployeeId
                                                    AND @Mve_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END
                                set @fromCostCenter = (SELECT CASE WHEN (@Mve_From = '')
                                                              THEN Emt_CostcenterCode
                                                              ELSE @Mve_From
                                                          END
                                                    FROM T_EmployeeMaster
                                                   WHERE Emt_EmployeeId = @Mve_EmployeeId)
IF NOT EXISTS(SELECT Mve_ControlNo FROM T_Movement
WHERE Mve_CurrentPayPeriod = @Mve_CurrentPayPeriod
AND Mve_EmployeeId = @Mve_EmployeeId
AND Mve_EffectivityDate = @Mve_EffectivityDate
AND Mve_Type = @Mve_Type
AND Mve_To = @Mve_To
AND Mve_Reason = @Mve_Reason
AND Mve_Status = @Mve_Status
AND Mve_Flag = @Mve_Flag)
BEGIN
                                INSERT INTO T_Movement
                                (
                                      Mve_ControlNo
                                    , Mve_CurrentPayPeriod
                                    , Mve_EmployeeId
                                    , Mve_EffectivityDate
                                    , Mve_AppliedDate
                                    , Mve_Costcenter
                                    , Mve_CostcenterLine
                                    , Mve_Type
                                    , Mve_From
                                    , Mve_To
                                    , Mve_Reason
                                    , Mve_EndorsedDateToChecker
                                    , Mve_Status
                                    , Mve_BatchNo
                                    , Mve_Flag
                                    , Usr_Login
                                    , Ludatetime
                                )
                                VALUES
                                (
                                      @Mve_ControlNo
                                    , @Mve_CurrentPayPeriod
                                    , @Mve_EmployeeId
                                    , @Mve_EffectivityDate
                                    , getdate()
                                    , @Costcenter
                                    , @LineCode
                                    , @Mve_Type
                                    , @fromCostCenter
                                    , @Mve_To
                                    , @Mve_Reason
                                    , @Mve_EndorsedDateToChecker
                                    , @Mve_Status
                                    , @Mve_BatchNo
                                    , @Mve_Flag
                                    , @Usr_Login
                                    , getdate()
                                )
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }
        #endregion

        #region [Update Record]
        public void UpdateMVRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[10];
            paramDetails[0] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Mve_CurrentPayPeriod", rowDetails["Mve_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Mve_EffectivityDate", rowDetails["Mve_EffectivityDate"]);
            paramDetails[3] = new ParameterInfo("@Mve_From", rowDetails["Mve_From"]);
            paramDetails[4] = new ParameterInfo("@Mve_To", rowDetails["Mve_To"]);
            paramDetails[5] = new ParameterInfo("@Mve_Reason", rowDetails["Mve_Reason"]);
            paramDetails[6] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[7] = new ParameterInfo("@Mve_BatchNo", rowDetails["Mve_BatchNo"]);
            paramDetails[8] = new ParameterInfo("@Mve_Flag", rowDetails["Mve_Flag"]);
            paramDetails[9] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                UPDATE T_Movement
                                SET   Mve_CurrentPayPeriod = @Mve_CurrentPayPeriod
                                    , Mve_EffectivityDate = @Mve_EffectivityDate
                                    , Mve_From = @Mve_From
                                    , Mve_To = @Mve_To
                                    , Mve_Reason = @Mve_Reason
                                    , Mve_Status = @Mve_Status
                                    , Mve_Flag = @Mve_Flag
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = getdate()
                                WHERE Mve_ControlNo = @Mve_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateMVRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Mve_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Mve_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Mve_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Mve_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Mve_CheckedBy", " ");
                }
                else if (rowDetails["Mve_Status"].ToString().Equals("5")
                    || rowDetails["Mve_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Mve_CheckedBy = @Mve_CheckedBy ,
                                Mve_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Mve_CheckedBy", rowDetails["Mve_CheckedBy"]);
                }
                else if (rowDetails["Mve_Status"].ToString().Equals("7")
                    || rowDetails["Mve_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Mve_Checked2By = @Mve_Checked2By ,
                                Mve_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Mve_Checked2By", rowDetails["Mve_Checked2By"]);
                }
                else if (rowDetails["Mve_Status"].ToString().Equals("9")
                    || rowDetails["Mve_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Mve_ApprovedBy = @Mve_ApprovedBy ,
                                Mve_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Mve_ApprovedBy", rowDetails["Mve_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Mve_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Mve_Status", rowDetails["Mve_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Mve_ControlNo", rowDetails["Mve_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_Movement
                                    SET {0}
                                        Mve_Status = @Mve_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Mve_ControlNo = @Mve_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateMVRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateMVRecord(string.Empty, rowDetails, dal);
        }
        #endregion

        public string GetInitialDayCode(string strWorkType, string strWorkGroup, DateTime ProcessDate, DALHelper dal)
        {
            string Value = "REG";
            try
            {
                if (!(strWorkType.Equals("REG")))
                {
                    string sqlCalendarGrpQuery = string.Format(@"SELECT Cal_WorkCode_{0:dd} 
                                                                   FROM T_CalendarGroup
                                                                  WHERE Cal_WorkType = '{1}'
                                                                    AND Cal_WorkGroup = '{2}'
                                                                    AND Cal_YearMonth = '{0:yyyyMM}'", ProcessDate, strWorkType, strWorkGroup);

                    DataTable dtResult;
                    dtResult = dal.ExecuteDataSet(sqlCalendarGrpQuery).Tables[0];
                    string strWorkCode = dtResult.Rows[0][0].ToString();

                    //Check if WorkCode is R or rest day
                    if (strWorkCode.Equals("R"))
                    {
                        Value = "REST";
                    }

                    //Check for special WorkCode (e.g. D5, N5)
                    if (strWorkCode.Length > 1)
                    {
                        Value = "REG" + strWorkCode.Substring(1, 1);
                    }
                }
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isRestDay(string EmployeeID, DateTime ProcessDate, DALHelper dal)
        {
            string Value = "REG";
            try
            {
                string sqlQuery = @"SELECT	TOP(1) Erd_RestDay
                                    FROM	T_EmployeeRestDay	
                                    WHERE	Erd_EmployeeID = @EmployeeId
                                    AND		Erd_EffectivityDate <= @ProcessDate
                                    ORDER BY Erd_EffectivityDate DESC";

                ParameterInfo[] paramInfo = new ParameterInfo[2];
                paramInfo[0] = new ParameterInfo("@EmployeeId", EmployeeID);
                paramInfo[1] = new ParameterInfo("@ProcessDate", ProcessDate);
                DataTable dtRestDay = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];

                string Day = ProcessDate.ToString("ddd").ToUpper();
                string Rest = dtRestDay.Rows[0][0].ToString();

                if (Day == "MON" && Rest.Substring(0, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "TUE" && Rest.Substring(1, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "WED" && Rest.Substring(2, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "THU" && Rest.Substring(3, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "FRI" && Rest.Substring(4, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "SAT" && Rest.Substring(5, 1).Equals("1"))
                    Value = "REST";
                else if (Day == "SUN" && Rest.Substring(6, 1).Equals("1"))
                    Value = "REST";
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isSpecialDay(string strWorkType, string strWorkGroup, DateTime ProcessDate, string strPayrollType, DALHelper dal)
        {
            string Value = "REG";
            try
            {
                string sqlSplDayQuery = string.Format(@"SELECT Ard_DayCode 
                                                          FROM T_SpecialDayMaster
                                                          WHERE Ard_WorkType = '{1}'
                                                          AND Ard_WorkGroup = '{2}'
                                                          AND Ard_Date = '{0}'
                                                          AND Ard_PayrollType = '{3}'",
                                                          ProcessDate, strWorkType, strWorkGroup, strPayrollType);

                DataTable dtResult;
                dtResult = dal.ExecuteDataSet(sqlSplDayQuery).Tables[0];
                Value = dtResult.Rows[0][0].ToString();
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isSatOFF(string EmployeeID, DateTime ProcessDate, DALHelper dal)
        {
            string Value = "REG";
            try
            {
                if (ProcessDate.ToString("ddd").ToUpper() == "SAT" || !GetProcessFlag("TIMEKEEP", "LATEOFFSET", dal))
                {
                    string sqlQuery = @"SELECT swh_Sat1Date
                                             , swh_Sat2Date
                                             , swh_Sat3Date
                                             , swh_Sat4Date
                                             , swh_Sat5Date
                                          FROM T_SaturdayWorkStatusHeader
                                         WHERE swh_YearMonth = @YearMonth
                                           AND swh_status = 'A'
                                           AND (swh_Sat1Date = @ProcessDate
                                              OR swh_Sat2Date = @ProcessDate
                                              OR swh_Sat3Date = @ProcessDate
                                              OR swh_Sat4Date = @ProcessDate
                                              OR swh_Sat5Date = @ProcessDate)
                                        -----------------------------------
                                        SELECT swd_Sat1Status
                                             , swd_Sat2Status
                                             , swd_Sat3Status
                                             , swd_Sat4Status
                                             , swd_Sat5Status
                                          FROM T_SaturdayWorkStatusDetail
                                         WHERE swd_YearMonth = @YearMonth
                                           AND swd_EmployeeId = @EmployeeId";

                    ParameterInfo[] paramInfo = new ParameterInfo[3];
                    paramInfo[0] = new ParameterInfo("@YearMonth", ProcessDate.ToString("yyyyMM"));
                    paramInfo[1] = new ParameterInfo("@EmployeeId", EmployeeID);
                    paramInfo[2] = new ParameterInfo("@ProcessDate", ProcessDate);
                    DataSet dsSatOFF = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                    if (dsSatOFF.Tables[0].Rows.Count == 0 || dsSatOFF.Tables[1].Rows.Count == 0)
                        Value = "REG";
                    else
                    {
                        if (dsSatOFF.Tables[0].Rows[0]["swh_Sat1Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat1Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat2Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat2Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat3Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat3Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat4Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat4Status"].Equals(true))
                            Value = "SOFF";
                        else if (dsSatOFF.Tables[0].Rows[0]["swh_Sat5Date"].Equals(ProcessDate) && dsSatOFF.Tables[1].Rows[0]["swd_Sat5Status"].Equals(true))
                            Value = "SOFF";
                    }
                }
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string isHoliday(string Location, DateTime ProcessDate, DALHelper dal)
        {
            string Value = "REG";
            try
            {
                string sqlQuery = @"SELECT	Hmt_HolidayCode 
                                    FROM	T_HolidayMaster 
                                    WHERE	Hmt_HolidayDate = @ProcessDate
                                    AND		(Hmt_ApplicCity = @Applicable
                                    OR		 Hmt_ApplicCity = 'ALL')";

                ParameterInfo[] paramInfo = new ParameterInfo[2];
                paramInfo[0] = new ParameterInfo("@Applicable", Location);
                paramInfo[1] = new ParameterInfo("@ProcessDate", ProcessDate);
                DataTable dtHoliday = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];

                Value = dtHoliday.Rows[0][0].ToString();
            }
            catch
            {
                Value = "REG";
            }
            return Value;
        }

        public string GetLocationCode(string processdate, string IdNum, DALHelper dal)
        {
            string LatestEffectDate = string.Empty;
            string LocationCode = string.Empty;

            if (IdNum != string.Empty)
            {
                LatestEffectDate = GetLatestEffectDate(processdate, IdNum, dal);
                if (LatestEffectDate != string.Empty)
                {
                    LocationCode = GetEffectiveLocationCode(LatestEffectDate, IdNum, dal);
                }
            }

            if (LocationCode.Trim() != string.Empty)
            {
                ////reynard::20090527 To update work location
                //Andre Commented 20100720: Transaction should not update location
                //string sqlLocUpdate = @"update t_EmployeeLogLedger set Ell_LocationCode='{2}' where ell_employeeid='{0}' and ell_processdate='{1}'";
                //dal.ExecuteNonQuery(string.Format(sqlLocUpdate, IdNum, processdate, LocationCode), CommandType.Text);
                ////end
                return LocationCode;
            }
            else
            {
                return "REG";
            }
        }

        private string GetEffectiveLocationCode(string processdate, string Ewl_EmployeeID, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Ewl_LocationCode From T_EmployeeWorkLocation
                                Where Ewl_EmployeeID = @Ewl_EmployeeID
                                And Ewl_EffectivityDate = @Ewl_EffectivityDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@Ewl_EffectivityDate", processdate);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Ewl_LocationCode"].ToString();
            else
                return string.Empty;
        }

        private bool GetProcessFlag(string SystemID, string ProcessID, DALHelper dal)
        {
            string sqlQuery = @"SELECT	Pcm_ProcessFlag 
                                FROM	T_ProcessControlMaster 
                                WHERE	Pcm_SystemID = @Pcm_SystemID
                                AND		Pcm_ProcessID = @Pcm_ProcessID";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Pcm_SystemID", SystemID);
            paramInfo[1] = new ParameterInfo("@Pcm_ProcessID", ProcessID);

            try
            {
                return Convert.ToBoolean(dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0].Rows[0][0]);
            }
            catch
            {
                throw new Exception(string.Format("System ID [{0}] and Process ID [{1}] combination not found in Process Control Master!", SystemID, ProcessID));
            }
        }

        private string GetLatestEffectDate(string processdate, string Ewl_EmployeeID, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Convert(char(10), MAX(Ewl_EffectivityDate), 101) as Ewl_EffectivityDate
                                 From T_EmployeeWorkLocation 
                                 Where Ewl_EffectivityDate <= @processdate
		                            and Ewl_EmployeeID = @Ewl_EmployeeID
		                            --and (Select Ppm_EndCycle From T_PayPeriodMaster
                                    --    Where Ppm_CycleIndicator = 'C'
                                    --    And Ppm_Status = 'A' ) <= @processdate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ewl_EmployeeID", Ewl_EmployeeID);
            paramInfo[1] = new ParameterInfo("@processdate", processdate);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        private string GetLedgerTableFromDate(string Date)
        {
            string LedgerTable = string.Empty;
            //MethodsLibrary.Methods ML = new MethodsLibrary.Methods();
            //string start = ML.FetchQuincena('s');

            //CommonMethods CM = new CommonMethods();
            //string end = CM.FetchQuincena('f', 'e').ToString();

            //string period = CommonMethods.getPayPeriod(Convert.ToDateTime(Date));

            //if (Convert.ToDateTime(Date) <= Convert.ToDateTime(end) || period.Equals(GetNextPayPeriod()))
            //{
            //    if (Convert.ToDateTime(Date) < Convert.ToDateTime(start))
            //    {
            //        LedgerTable = "t_employeelogledgerhist";
            //    }
            //    else
            //    {
            //        LedgerTable = "t_employeelogledger";
            //    }
            //}
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(@"SELECT CONVERT(VARCHAR(20), Ppm_StartCycle, 101) [Ppm_StartCycle] FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'");
                    if (!CommonMethods.isEmpty(ds))
                    {
                        if (Convert.ToDateTime(Date) >= Convert.ToDateTime(ds.Tables[0].Rows[0][0].ToString().Trim()))
                        {
                            LedgerTable = "t_employeelogledger";
                        }
                        else
                        {
                            LedgerTable = "t_employeelogledgerhist";
                        }
                    }
                    else
                    {
                        LedgerTable = "t_employeelogledger";
                    }
                }
                catch
                {
                    LedgerTable = "t_employeelogledger";
                }
                finally 
                {
                    dal.CloseDB();
                }
            }
            return LedgerTable;
        }

        public string GetPayPeriod()
        {
            string payPeriod = "";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    string sqlGetPayPeriod = @" SELECT
                                                    Ppm_PayPeriod
                                                FROM
                                                    T_PayPeriodMaster
                                                WHERE
                                                    Ppm_CycleIndicator = 'C' AND Ppm_Status = 'A' ";
                    dal.OpenDB();
                    payPeriod = Convert.ToString(dal.ExecuteScalar(sqlGetPayPeriod, CommandType.Text));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return payPeriod;
        }

        public string GetNextPayPeriod()
        {
            string payPeriod = "";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    string sqlGetPayPeriod = @" DECLARE @EndDate as Datetime
                              Set @EndDate = (Select Ppm_EndCycle From t_PayPeriodMaster
                              Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')

                              SELECT Ppm_PayPeriod
                              FROM t_PayPeriodMaster
                              Where Ppm_StartCycle = dateadd(dd,1,@EndDate)";
                    dal.OpenDB();
                    payPeriod = Convert.ToString(dal.ExecuteScalar(sqlGetPayPeriod, CommandType.Text));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return payPeriod;
        }
		
		//temp USE CommonMethods.getEmployeePayrollType
        private string getPayrollType(string UserId, DALHelper dal)
        {
            string sql = string.Format(@"  SELECT Emt_PayrollType
                                         FROM T_EmployeeMaster
                                        WHERE Emt_EmployeeId = '{0}'", UserId);
            return dal.ExecuteScalar(sql, CommandType.Text).ToString();

        }


        #region Approval Updates
        public void UpdateApproveMovementChecklist(string controlNo, string userLogged, DALHelper dal)
        {
            DataRow drDetails = getMovementInfo(controlNo);
            switch (drDetails["Mve_Type"].ToString().ToUpper().Trim())
            { 
                case "S"://shift update
                    EmployeeLogLedgerUpdateShift( drDetails["ID No"].ToString().Trim()
                                                , drDetails["Effectivity Date"].ToString().Trim()
                                                , drDetails["To Code"].ToString().Trim()
                                                , userLogged
                                                , dal);
                    break;
                case "G"://group update//robert added control number 04022013
                    CompleteCascadeUpdateWorkgroup(drDetails["ID No"].ToString().Trim()
                                                  , Convert.ToDateTime(drDetails["Effectivity Date"].ToString().Trim())
                                                  , drDetails["To Code"].ToString().Substring(0, 3).Trim()
                                                  , drDetails["To Code"].ToString().Substring(3, 3).Trim()
                                                  , drDetails["Emt_PayrollType"].ToString().Trim()
                                                  , userLogged
                                                  , dal
                                                  ,true);
                    RestoreWorkgroupAfterCompleteCascade(drDetails["ID No"].ToString().Trim()
                                                        , Convert.ToDateTime(drDetails["Effectivity Date"].ToString())
                                                        , userLogged
                                                        , dal);
                    break;
                case "C"://costcenter update
                    EmployeeLogLedgerUpdateCostCenter(drDetails["ID No"].ToString().Trim()
                                                    , drDetails["To Code"].ToString().Trim()
                                                    , drDetails["Effectivity Date"].ToString().Trim()
                                                    , userLogged
                                                    , dal
                                                    , drDetails["Control No"].ToString().Trim());
                    break;
                case "R"://restday update
                    EmployeeLogLedgerUpdateRestday(drDetails["ID No"].ToString().Trim()
                                                    , drDetails["From Code"].ToString().Trim()
                                                    , drDetails["To Code"].ToString().Trim()
                                                    , userLogged
                                                    , dal);
                    break;
                default:
                    break;
            }
        }

        public void EmployeeLogLedgerUpdateShift( string employeeId
                                                , string processDate
                                                , string toShift
                                                , string userlogin
                                                , DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", employeeId);
            paramInfo[1] = new ParameterInfo("@EffectivityDate", processDate);
            paramInfo[2] = new ParameterInfo("@ToShift", toShift);
            paramInfo[3] = new ParameterInfo("@Usr_Login", userlogin);

            string LedgerTable = GetLedgerTableFromDate(processDate);
            string Payperiod = CommonMethods.getPayPeriod(Convert.ToDateTime(processDate));
            #endregion

            bool isFuture = GetIndicator(processDate, employeeId);
            string indicator= string.Empty;
            if (isFuture == true)
                indicator = "F";
            else
                indicator = "C";
            #region SQL Query
            string sqlQuery1 = @" 
DECLARE @PPM_STARTCYCLE AS VARCHAR(20)
	SET @PPM_STARTCYCLE = (
		SELECT TOP 1 
			CONVERT(VARCHAR(20), Ppm_StartCycle, 101) 
		FROM T_PayPeriodMaster 
		WHERE Ppm_CycleIndicator = '@cycleIndicator'
	)

DECLARE @MAXSEQNO AS TINYINT
	SET @MAXSEQNO = (
		SELECT 
			CONVERT(TINYINT, ISNULL(max(Elt_Seqno), '00')) + 1
		FROM T_EmployeeLogTrail
		WHERE Elt_ProcessDate = @EffectivityDate
			AND  Elt_EmployeeId = @Ell_EmployeeId)	
	
	
IF @EffectivityDate >= @PPM_STARTCYCLE
BEGIN
	INSERT INTO T_EmployeeLogTrail
	   (Elt_EmployeeId
		,Elt_ProcessDate
		,Elt_Seqno
		,Elt_ShiftCode
		,Elt_ActualTimeIn_1
		,Elt_ActualTimeOut_1
		,Elt_ActualTimeIn_2
		,Elt_ActualTimeOut_2
		,Usr_Login
		,Ludatetime)
	
	SELECT TOP 1 
		Ell_EmployeeId
		, Ell_ProcessDate
		, REPLICATE('0', 2 - LEN(CONVERT(VARCHAR(2), @MAXSEQNO))) 
			+ CONVERT(VARCHAR(2), @MAXSEQNO)
		, Ell_ShiftCode
		, Ell_ActualTimeIn_1
		, Ell_ActualTimeOut_1
		, Ell_ActualTimeIn_2
		, Ell_ActualTimeOut_2
		, @Usr_Login
		, getdate()
	FROM T_EmployeeLogLedger
	WHERE Ell_EmployeeId = @Ell_EmployeeId
		AND Ell_ProcessDate = @EffectivityDate

    
    UPDATE T_EmployeeLogledger
    SET Ell_ShiftCode = @ToShift
        , Usr_Login = @Usr_Login
        , Ludatetime = GETDATE()
    WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	AND Ell_ProcessDate = @EffectivityDate
		
END
ELSE
BEGIN
	INSERT INTO T_EmployeeLogTrail
	   (Elt_EmployeeId
		,Elt_ProcessDate
		,Elt_Seqno
		,Elt_ShiftCode
		,Elt_ActualTimeIn_1
		,Elt_ActualTimeOut_1
		,Elt_ActualTimeIn_2
		,Elt_ActualTimeOut_2
		,Usr_Login
		,Ludatetime)

	SELECT TOP 1 
		Ell_EmployeeId
		, Ell_ProcessDate
		, REPLICATE('0', 2 - LEN(CONVERT(VARCHAR(2), @MAXSEQNO))) 
			+ CONVERT(VARCHAR(2), @MAXSEQNO)
		, Ell_ShiftCode
		, Ell_ActualTimeIn_1
		, Ell_ActualTimeOut_1
		, Ell_ActualTimeIn_2
		, Ell_ActualTimeOut_2
		, @Usr_Login
		, getdate()
	FROM T_EmployeeLogLedgerHist
	WHERE Ell_EmployeeId = @Ell_EmployeeId
		AND Ell_ProcessDate = @EffectivityDate

    UPDATE T_EmployeeLogledgerHist
    SET Ell_ShiftCode = @ToShift
        , Usr_Login = @Usr_Login
        , Ludatetime = GETDATE()
    WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	AND Ell_ProcessDate = @EffectivityDate		
END

                                 ";
            DataSet check = new DataSet();
            try
            {
                check = dal.ExecuteDataSet(@"Select  TOP 1 Elt_DayCode
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
                sqlQuery1 = @" 
DECLARE @PPM_STARTCYCLE AS VARCHAR(20)
	SET @PPM_STARTCYCLE = (
		SELECT TOP 1 
			CONVERT(VARCHAR(20), Ppm_StartCycle, 101) 
		FROM T_PayPeriodMaster 
		WHERE Ppm_CycleIndicator = '@cycleIndicator'
	)

DECLARE @MAXSEQNO AS TINYINT
	SET @MAXSEQNO = (
		SELECT 
			CONVERT(TINYINT, ISNULL(max(Elt_Seqno), '00')) + 1
		FROM T_EmployeeLogTrail
		WHERE Elt_ProcessDate = @EffectivityDate
			AND  Elt_EmployeeId = @Ell_EmployeeId)	
	
	
IF @EffectivityDate >= @PPM_STARTCYCLE
BEGIN
	INSERT INTO T_EmployeeLogTrail
	   (Elt_EmployeeId
		,Elt_ProcessDate
		,Elt_Seqno
	    , Elt_DayCode
	    , Elt_Restday
	    , Elt_Holiday
		,Elt_ShiftCode
		,Elt_ActualTimeIn_1
		,Elt_ActualTimeOut_1
		,Elt_ActualTimeIn_2
		,Elt_ActualTimeOut_2
		,Usr_Login
		,Ludatetime)
	
	SELECT TOP 1 
		Ell_EmployeeId
		, Ell_ProcessDate
		, REPLICATE('0', 2 - LEN(CONVERT(VARCHAR(2), @MAXSEQNO))) 
			+ CONVERT(VARCHAR(2), @MAXSEQNO)
		, Ell_DayCode
	    , Ell_Restday
	    , Ell_Holiday
		, Ell_ShiftCode
		, Ell_ActualTimeIn_1
		, Ell_ActualTimeOut_1
		, Ell_ActualTimeIn_2
		, Ell_ActualTimeOut_2
		, @Usr_Login
		, getdate()
	FROM T_EmployeeLogLedger
	WHERE Ell_EmployeeId = @Ell_EmployeeId
		AND Ell_ProcessDate = @EffectivityDate

    
    UPDATE T_EmployeeLogledger
    SET Ell_ShiftCode = @ToShift
        , Usr_Login = @Usr_Login
        , Ludatetime = GETDATE()
    WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	AND Ell_ProcessDate = @EffectivityDate
		
END
ELSE
BEGIN
	INSERT INTO T_EmployeeLogTrail
	   (Elt_EmployeeId
		,Elt_ProcessDate
		,Elt_Seqno
	    , Elt_DayCode
	    , Elt_Restday
	    , Elt_Holiday
		,Elt_ShiftCode
		,Elt_ActualTimeIn_1
		,Elt_ActualTimeOut_1
		,Elt_ActualTimeIn_2
		,Elt_ActualTimeOut_2
		,Usr_Login
		,Ludatetime)

	SELECT TOP 1 
		Ell_EmployeeId
		, Ell_ProcessDate
		, REPLICATE('0', 2 - LEN(CONVERT(VARCHAR(2), @MAXSEQNO))) 
			+ CONVERT(VARCHAR(2), @MAXSEQNO)
        , Ell_DayCode
	    , Ell_Restday
	    , Ell_Holiday
		, Ell_ShiftCode
		, Ell_ActualTimeIn_1
		, Ell_ActualTimeOut_1
		, Ell_ActualTimeIn_2
		, Ell_ActualTimeOut_2
		, @Usr_Login
		, getdate()
		
	FROM T_EmployeeLogLedgerHist
	WHERE Ell_EmployeeId = @Ell_EmployeeId
		AND Ell_ProcessDate = @EffectivityDate

    UPDATE T_EmployeeLogledgerHist
    SET Ell_ShiftCode = @ToShift
        , Usr_Login = @Usr_Login
        , Ludatetime = GETDATE()
    WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	AND Ell_ProcessDate = @EffectivityDate		
END";

            }
            #endregion

            sqlQuery1 = sqlQuery1.Replace("@cycleIndicator", indicator);
            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(employeeId
                                        , MethodsLibrary.Methods.getPayPeriod(processDate)
                                        , GetPayPeriod()))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(employeeId
                                                , MethodsLibrary.Methods.getPayPeriod(processDate)
                                                , dal);
                    }

                }

            }
            dal.ExecuteNonQuery(sqlQuery1, CommandType.Text, paramInfo);

        }

        public bool GetIndicator(string processDate, string employeeid)
        {
            bool isFutureIndicator = false;

            string query = string.Format(@"Select Ppm_CycleIndicator from T_PayPeriodMaster
                            left join t_employeelogledger
                            on Ell_PayPeriod = Ppm_PayPeriod
                            where Ell_EmployeeId = '{0}'
                            and Ell_ProcessDate = '{1}'", employeeid, processDate);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0 && dtResult.Rows[0]["Ppm_CycleIndicator"].ToString()=="F")
            {
                isFutureIndicator = true;
            }
            return isFutureIndicator;
        }
        public void UpdateOvertimeTransactionsBasedOnNewShiftCurrent(string PayPeriod, string EmployeeID, string ProcessDate, DALHelper dal)
        {
            
        }

        public void UpdateOvertimeTransactionsBasedOnNewShiftPrevious(string PayPeriod, string EmployeeID, string ProcessDate, DALHelper dal)
        {

        }

        public void CompleteCascadeUpdateWorkgroup(string controlNo, string userLogged, DALHelper dal)
        {
            string sql = string.Format(@"SELECT Mve_EmployeeId
                                              , Mve_EffectivityDate
                                              , LTRIM(LEFT(Mve_To,3)) [WorkType] 
                                              , LTRIM(RIGHT(MVE_TO,3)) [WorkGroup]
                                              , Emt_PayrollType 
                                           FROM T_Movement 
                                           JOIN T_EmployeeMaster ON Mve_EmployeeId = Emt_EmployeeID 
                                          WHERE Mve_ControlNo = '{0}'", controlNo);

            try
            {
                DataSet ds = dal.ExecuteDataSet(sql);

                string EmployeeId = ds.Tables[0].Rows[0]["Mve_EmployeeId"].ToString();
                DateTime dtEffectivityDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Mve_EffectivityDate"].ToString());
                string Worktype = ds.Tables[0].Rows[0]["WorkType"].ToString();
                string Workgroup = ds.Tables[0].Rows[0]["WorkGroup"].ToString();
                string PayrollType = ds.Tables[0].Rows[0]["Emt_PayrollType"].ToString();

                CompleteCascadeUpdateWorkgroup(EmployeeId, dtEffectivityDate, Worktype, Workgroup, PayrollType, userLogged, dal);
            }
            catch (Exception e)
            {
                CommonMethods.ErrorsToTextFile(e, "CompleteCascadeUpdateWorkgroup");
            }
        }
        public void CompleteCascadeUpdateWorkgroup(string EmployeeId
                                                  , DateTime dtEffectivityDate
                                                  , string Worktype
                                                  , string Workgroup
                                                  , string PayrollType
                                                  , string userLogged
                                                  , DALHelper dal)
        {
            CompleteCascadeUpdateWorkgroup(EmployeeId , dtEffectivityDate, Worktype , Workgroup , PayrollType , userLogged , dal, true);
        }

        public void CompleteCascadeUpdateWorkgroup( string EmployeeId
                                                  , DateTime dtEffectivityDate
                                                  , string Worktype
                                                  , string Workgroup
                                                  , string PayrollType
                                                  , string userLogged
                                                  , DALHelper dal
                                                  , bool createTrail)
        {
            #region Perth Added 04/23/2012
            string LogledgerTable = "T_EMPLOYEELOGLEDGER";
            string LogledgerTableHist = "T_EMPLOYEELOGLEDGERHIST";
            DateTime dtStartCycle = Convert.ToDateTime(dal.ExecuteScalar("SELECT CONVERT(VARCHAR(20), Ppm_StartCycle, 101) FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'"));
            DateTime dtEndCycle = Convert.ToDateTime(dal.ExecuteScalar("SELECT CONVERT(VARCHAR(20), Ppm_EndCycle, 101) FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C'"));
            #endregion

            //------------ T_EMployee Group
            DataSet dsTemp = new DataSet();
            DataTable dtFinal = new DataTable();
            DateTime origEffectivityDate = dtEffectivityDate;
            #region Step 1 - Get parameters for checking
            string sqlGetParamters = @" SELECT Convert(varchar(10), Ppm_StartCycle, 101) [Ppm_StartCycle]
                                             , Convert(varchar(10), Ppm_EndCycle, 101) [Ppm_EndCycle]
                                          FROM T_PayPeriodMaster   
                                         WHERE Ppm_CycleIndicator = 'C' 

                                        SELECT Convert(varchar(10), MAX(Emv_EffectivityDate), 101) [Emv_EffectivityDate]
                                          FROM T_EmployeeGroup
                                         WHERE Emv_EmployeeId = '{0}' ";

            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetParamters, EmployeeId), CommandType.Text);
            #endregion

            #region Step 2 - INSERT INTO T_EmployeeGroup
            #region queries
            string sqlInsertUpdateWorkgroupMovement = @" IF EXISTS (SELECT Emv_EffectivityDate 
                                                                       FROM T_EmployeeGroup
                                                                      WHERE Emv_EmployeeId = '{0}'
                                                                        AND Emv_EffectivityDate = '{1}')
                                                       BEGIN 
                                                      UPDATE T_EmployeeGroup
                                                         SET Emv_WorkType = '{2}' 
                                                           , Emv_WorkGroup = '{3}' 
                                                       WHERE Emv_EmployeeId = '{0}'
                                                         AND Emv_EffectivityDate = '{1}'                                 
                                                         END 
                                                        ELSE
                                                       BEGIN  
                                                      INSERT INTO T_EmployeeGroup 
                                                      SELECT TOP 1 Emv_EmployeeId
                                                           , '{1}'
                                                           ,  dateadd(day, -1 , Emv_EffectivityDate)
                                                           , '{2}'
                                                           , '{3}'
                                                           , ''
                                                           , '{4}'
                                                           , GETDATE()
                                                        FROM T_EmployeeGroup 
                                                       WHERE Emv_EmployeeId = '{0}'
                                                         AND Emv_EffectivityDate > '{1}' 

                                                      UPDATE T_EmployeeGroup
		                                                 SET Emv_EndDate  = dateadd(day, -1 , '{1}')
                                                       WHERE Emv_EmployeeId = '{0}'
		                                                 AND Emv_EffectivityDate = (SELECT TOP 1 Emv_EffectivityDate
							                                                          FROM T_EmployeeGroup
							                                                         WHERE Emv_EmployeeId = '{0}'
								                                                       AND Emv_EffectivityDate < '{1}' 
							                                                         ORDER BY Emv_EffectivityDate DESC)
                                                         END ";

            string sqlInsertWorkgroupMovement1 = @"DECLARE @MaxStart as datetime
                                                        SET @MaxStart = (SELECT MAX(Emv_EffectivityDate)		
                                                                           FROM T_EmployeeGroup		
                                                                          WHERE Emv_EmployeeId = '{0}' )

                                                    UPDATE T_EmployeeGroup			
                                                       SET Emv_EndDate = Convert(datetime,'{1}') - 1 			
                                                     WHERE Emv_EmployeeId = '{0}'	
                                                       AND Emv_EffectivityDate = @MaxStart
                                    
                                                    INSERT INTO T_EmployeeGroup	
                                                    SELECT '{0}'	
                                                         , '{1}'	
                                                         , null	
                                                         , '{2}'
                                                         , '{3}'
                                                         , ''
                                                         , '{4}'
                                                         , GETDATE() ";

            string sqlInsertWorkgroupMovement2 = @"INSERT INTO T_EmployeeGroup	
                                                    SELECT '{0}'	
                                                         , '{1}'	
                                                         , null	
                                                         , '{2}'
                                                         , '{3}'
                                                         , ''
                                                         , '{4}'
                                                         , GETDATE() ";

            #endregion
            if (dsTemp.Tables[1].Rows.Count > 0)
            {
                if (dsTemp.Tables[1].Rows[0]["Emv_EffectivityDate"].ToString().Equals(string.Empty))
                {
                    dal.ExecuteNonQuery(string.Format(sqlInsertWorkgroupMovement2, EmployeeId, dtEffectivityDate.ToString("MM/dd/yyyy"), Worktype, Workgroup, session["userLogged"].ToString()), CommandType.Text);
                }
                else if (Convert.ToDateTime(dtEffectivityDate.ToString("MM/dd/yyyy")) <= Convert.ToDateTime(dsTemp.Tables[1].Rows[0]["Emv_EffectivityDate"].ToString()))
                {
                    dal.ExecuteNonQuery(string.Format(sqlInsertUpdateWorkgroupMovement, EmployeeId, dtEffectivityDate.ToString("MM/dd/yyyy"), Worktype, Workgroup, session["userLogged"].ToString()), CommandType.Text);
                }
                else
                { 
                    dal.ExecuteNonQuery(string.Format(sqlInsertWorkgroupMovement1, EmployeeId, dtEffectivityDate.ToString("MM/dd/yyyy"), Worktype, Workgroup, session["userLogged"].ToString()), CommandType.Text);
                }
            }
            else//no Employee Group Movement Trail
            {
                dal.ExecuteNonQuery(string.Format(sqlInsertWorkgroupMovement2, EmployeeId, dtEffectivityDate.ToString("MM/dd/yyyy"), Worktype, Workgroup, session["userLogged"].ToString()), CommandType.Text);
            }
            #endregion
            //End T_EmployeeGroup


            DataRow drCycleRange = dal.ExecuteDataSet(string.Format(@" SELECT '{0}' [Min]
                                                                            , Convert(varchar(10), MAX(Ell_ProcessDate), 101) [Max]
                                                                         FROM T_EmployeeLogLedger", dtEffectivityDate.Date.ToString("MM/dd/yyyy")), CommandType.Text).Tables[0].Rows[0];
            DateTime dtEllStart = Convert.ToDateTime(drCycleRange["Min"]);
            DateTime dtEllEnd = Convert.ToDateTime(drCycleRange["Max"]);

            #region Insert Trail
            string LedgerTable = LogledgerTable; // GetLedgerTableFromDate(dtEffectivityDate.Date.ToString("MM/dd/yyyy"));
            if (dtStartCycle > dtEffectivityDate.Date)
                LedgerTable = LogledgerTableHist;

            if (!LedgerTable.ToString().Equals(string.Empty) && createTrail)
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(EmployeeId
                                        , MethodsLibrary.Methods.getPayPeriod(dtEffectivityDate.Date.ToString("MM/dd/yyyy"))
                                        , GetPayPeriod(), dal))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(EmployeeId
                                                , MethodsLibrary.Methods.getPayPeriod(dtEffectivityDate.Date.ToString("MM/dd/yyyy"))
                                                , dal);
                    }
                }
                
            }
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", EmployeeId);
            paramInfo[4] = new ParameterInfo("@UserLogin", userLogged);
            paramInfo[5] = new ParameterInfo("@Ell_WorkType", Worktype);
            paramInfo[6] = new ParameterInfo("@Ell_WorkGroup", Workgroup);

            string query;
            while (dtEffectivityDate <= dtEllEnd && dtEffectivityDate >= dtEllStart)
            {
                LedgerTable = LogledgerTable; 
                if (dtStartCycle > dtEffectivityDate.Date)
                    LedgerTable = LogledgerTableHist;
                //Set default values
                paramInfo[1] = new ParameterInfo("@Ell_ProcessDate", dtEffectivityDate);
                paramInfo[2] = new ParameterInfo("@Ell_DayCode", "REG");
                paramInfo[3] = new ParameterInfo("@Ell_RestDay", "False");
                paramInfo[7] = new ParameterInfo("@Ell_Holiday", "False");

                //Check for special shifting
                string DayCode = GetInitialDayCode(Worktype, Workgroup, dtEffectivityDate, dal);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    if (DayCode.Equals("REST"))
                    {
                        paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                    }
                }

                //Check if rest day on a saturday
                DayCode = isSatOFF(EmployeeId, dtEffectivityDate, dal);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    paramInfo[7] = new ParameterInfo("@Ell_Holiday", "True");
                }

                //Check if rest day
                DayCode = isRestDay(EmployeeId, dtEffectivityDate, dal);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                }

                //Check for special day
                DayCode = isSpecialDay(Worktype, Workgroup, dtEffectivityDate, PayrollType, dal);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    if (DayCode.Equals("REST"))
                    {
                        paramInfo[3] = new ParameterInfo("@Ell_RestDay", "True");
                    }
                }

                //Check if holiday
                DayCode = isHoliday(this.GetLocationCode(dtEffectivityDate.ToString(), EmployeeId, dal), dtEffectivityDate, dal);
                if (!DayCode.Equals("REG"))
                {
                    paramInfo[2] = new ParameterInfo("@Ell_DayCode", DayCode);
                    paramInfo[7] = new ParameterInfo("@Ell_Holiday", "True");
                }

                //Update Log Ledger table
                query = string.Format(@"UPDATE {0}
                             SET [Ell_DayCode] = @Ell_DayCode
                               , [Ell_RestDay] = @Ell_RestDay
                               , [Ell_Holiday] = @Ell_Holiday
                               , [Ell_WorkType] = @Ell_WorkType
                               , [Ell_WorkGroup] = @Ell_WorkGroup
                               , [Usr_Login] = @UserLogin
                               , [Ludatetime] = GETDATE()
                           WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                             AND [Ell_ProcessDate] = @Ell_ProcessDate", LedgerTable);

                dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

                if (!Worktype.Equals("") && !Worktype.ToString().Equals("REG"))
                {
                    //Update shift code for employee with special shift (e.g. 4-2, 5-1, 512)
                    query = string.Format(@"
DECLARE @SHIFTCODE AS VARCHAR(20)
SET @SHIFTCODE = (SELECT Cal_ShiftCode_{0:dd} 
                    FROM T_CalendarGroup
                    WHERE Cal_WorkType = @Ell_WorkType
                    AND Cal_WorkGroup = @Ell_WorkGroup
                    AND Cal_YearMonth = {0:yyyyMM})
IF @SHIFTCODE IS NOT NULL AND RTRIM(@SHIFTCODE) <> ''
BEGIN
    UPDATE {1}
        SET Ell_ShiftCode = (SELECT Cal_ShiftCode_{0:dd} 
                                FROM T_CalendarGroup
                                WHERE Cal_WorkType = @Ell_WorkType
                                AND Cal_WorkGroup = @Ell_WorkGroup
                                AND Cal_YearMonth = {0:yyyyMM})
        WHERE Ell_EmployeeId = @Ell_EmployeeId
        AND Ell_ProcessDate = @Ell_ProcessDate
END
                                                    ", dtEffectivityDate, LedgerTable);
                }
                else
                {
                    //Update shift code for employee with REG shift
                    query = string.Format(@"--new query for update shift in logledger
                              UPDATE {0}
                                 SET [Ell_ShiftCode] = CASE WHEN Ell_DayCode ='REG'
                                                            THEN Emt_ShiftCode
                                                            ELSE CASE WHEN LEN(RTRIM(Scm_EquivalentShiftCode)) > 0 
                                                                      THEN Scm_EquivalentShiftCode
                                                                      ELSE Emt_Shiftcode
                                                                  END
                                                        END
                                FROM {0}
                               INNER JOIN T_EmployeeMaster 
                                  ON Emt_EmployeeID = Ell_EmployeeID
                               INNER JOIN T_ShiftCodeMaster 
                                  ON Scm_ShiftCode = Emt_ShiftCode
                               WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                 AND [Ell_ProcessDate] = @Ell_ProcessDate
                                                    ", LedgerTable);
                }

                dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

                query = string.Format(@"--Jule Added 20090620 : Special Working Days ShiftCode Recurrence
                          UPDATE {0}
                             SET [Ell_ShiftCode] = [Esm_EquivShiftcode]
                            FROM {0}
                            JOIN [T_SpecialShiftMaster]
                              ON [Ssm_ProcessDate] = [Ell_ProcessDate]
                            JOIN [T_EquivShiftMaster]
                              ON [Esm_Shiftcode] = [Ell_ShiftCode]
                             AND [Esm_ShiftHours] = [Ssm_ShiftHours]
                           WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                             AND [Ell_ProcessDate] = @Ell_ProcessDate
                             AND LEFT(Ell_Locationcode,1) <> 'D'
                                        ", LedgerTable);
                dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

                dtEffectivityDate = dtEffectivityDate.AddDays(1);
            }
            if (origEffectivityDate <= CommonMethods.getQuincenaDate('C', "END"))    
            {
                string sql = @"UPDATE T_EmployeeMaster 
	                              SET Emt_WorkType = '{0}'
	                                , Emt_WorkGroup = '{1}' 
	                            WHERE Emt_EmployeeID = '{2}'";
	
	            dal.ExecuteNonQuery(string.Format(sql, Worktype, Workgroup, EmployeeId), CommandType.Text);
            }
        }

        public void RestoreWorkgroupAfterCompleteCascade(string EmployeeId
                                                  , DateTime dtEffectivityDate
                                                  , string userLogged
                                                  , DALHelper dal)
        {
            string sql = @"SELECT Mve_EffectivityDate
                                , Mve_To
                             FROM T_Movement 
                            WHERE Mve_Status = '9' 
                              AND Mve_EffectivityDate > '{0}'
                              AND Mve_EmployeeID = {1}
                              AND Mve_Type = 'G' 
                            ORDER BY Mve_EffectivityDate";

            DataSet ds = new DataSet();
            try
            {
                ds = dal.ExecuteDataSet(string.Format(sql, dtEffectivityDate.ToString("MM/dd/yyyy"), EmployeeId), CommandType.Text);
            }
            catch (Exception ex) 
            {
                CommonMethods.ErrorsToTextFile(ex, "RestoreWorkgroupAfterCompleteCascade");
            }

            if (!CommonMethods.isEmpty(ds))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CompleteCascadeUpdateWorkgroup(EmployeeId
                                             , Convert.ToDateTime(ds.Tables[0].Rows[i]["Mve_EffectivityDate"])
                                             , ds.Tables[0].Rows[i]["Mve_To"].ToString().Substring(0, 3).Trim()
                                             , ds.Tables[0].Rows[i]["Mve_To"].ToString().Substring(3, 3).Trim()
                                             , getPayrollType(EmployeeId.ToUpper(), dal)
                                             , userLogged
                                             , dal
                                             , false);
                }
            }
        }

        //Not in use anymore
        public void EmployeeLogLedgerUpdateCostCenter(string employeeId
                                                  , string toCC
                                                  , string date
                                                  , string userlogin
                                                  , DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Emt_EmployeeId", employeeId);
            paramInfo[1] = new ParameterInfo("@Emt_CostCenterDate", date);
            paramInfo[2] = new ParameterInfo("@ToCostCenter", toCC);
            paramInfo[3] = new ParameterInfo("@Usr_Login", userlogin);

            #endregion

            #region SQL Query
            string sqlQuery = @"    DECLARE @MaxStart as datetime
                                        SET @MaxStart = (SELECT MAX(Ecm_StartDate)		
                                                           FROM T_EmployeeCostcenterMovement		
                                                          WHERE Ecm_EmployeeID = @Emt_EmployeeId)

                                    UPDATE T_EmployeeCostcenterMovement			
                                       SET Ecm_EndDate = Convert(datetime,@Emt_CostCenterDate) - 1 			
                                     WHERE Ecm_Employeeid = @Emt_EmployeeId		
                                       AND Ecm_Startdate = @MaxStart
                                    
                                    INSERT INTO T_EmployeeCostcenterMovement
										 ( Ecm_EmployeeID
										 , Ecm_StartDate
										 , Ecm_EndDate
										 , Ecm_CostCenterCode
										 , Usr_Login
										 , Ludatetime
									     )	
                                    SELECT @Emt_EmployeeId	
                                         , @Emt_CostCenterDate	
                                         , null	
                                         , @ToCostCenter
                                         , @Usr_Login	
                                         , getdate()
                                    
                                    UPDATE T_EmployeeMaster		
                                       SET Emt_CostCenterCode = @ToCostCenter
                                         , Emt_CostCenterDate = @Emt_CostCenterDate	
                                     WHERE Emt_EmployeeId = @Emt_EmployeeId ";
            #endregion
            dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
        }

        public void EmployeeLogLedgerUpdateCostCenter( string employeeId
                                                     , string toCC
                                                     , string date
                                                     , string userlogin
                                                     , DALHelper dal
                                                     , string controlNo)
        {
            DataSet dsTemp = new DataSet();
            DataTable dtFinal = new DataTable();
            #region Step 1 - Get parameters for checking
            string sqlGetParamters = @" SELECT Convert(varchar(10), Ppm_StartCycle, 101) [Ppm_StartCycle]
                                             , Convert(varchar(10), Ppm_EndCycle, 101) [Ppm_EndCycle]
                                          FROM T_PayPeriodMaster   
                                         WHERE Ppm_CycleIndicator = 'C' 

                                        SELECT Convert(varchar(10), MAX(Ecm_StartDate), 101) [Ecm_StartDate]
                                          FROM T_EmployeeCostcenterMovement
                                         WHERE Ecm_EmployeeId = '{0}' 
                                         HAVING MAX(Ecm_StartDate) is not null";

            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetParamters, employeeId), CommandType.Text);
            #endregion

            #region Step 2 - INSERT INTO T_EmployeeCostcenterMovement
            #region queries
            string sqlInsertUpdateCostcenterMovement = @" IF EXISTS (SELECT Ecm_StartDate 
                                                                       FROM T_EmployeeCostcenterMovement
                                                                      WHERE Ecm_EmployeeId = '{0}'
                                                                        AND Ecm_StartDate = '{1}')
                                                       BEGIN 
                                                      UPDATE T_EmployeeCostcenterMovement
                                                         SET Ecm_CostcenterCode = '{2}' 
                                                       WHERE Ecm_EmployeeId = '{0}'
                                                         AND Ecm_StartDate = '{1}'                                 
                                                         END 
                                                        ELSE
                                                       BEGIN  
                                                      INSERT INTO T_EmployeeCostcenterMovement 
														   ( Ecm_EmployeeID
														   , Ecm_StartDate
														   , Ecm_EndDate
														   , Ecm_CostCenterCode
														   , Usr_Login
														   , Ludatetime
													       )	
                                                      SELECT TOP 1 Ecm_EmployeeId
                                                           , '{1}'
                                                           ,  dateadd(day, -1 , Ecm_StartDate)
                                                           , '{2}'
                                                           , '{3}'
                                                           , GETDATE()
                                                        FROM T_EmployeeCostcenterMovement 
                                                       WHERE Ecm_EmployeeId = '{0}'
                                                         AND Ecm_StartDate > '{1}' 

                                                      UPDATE T_EmployeeCostcenterMovement
		                                                 SET Ecm_EndDate  = dateadd(day, -1 , '{1}')
                                                       WHERE Ecm_EmployeeId = '{0}'
		                                                 AND Ecm_StartDate = (SELECT TOP 1 Ecm_StartDate
							                                                    FROM T_EmployeeCostcenterMovement
							                                                   WHERE Ecm_EmployeeId = '{0}'
								                                                 AND Ecm_StartDate < '{1}' 
							                                                   ORDER BY Ecm_StartDate DESC)
                                                         END ";

            string sqlInsertCostcenterMovement1 = @"DECLARE @MaxStart as datetime
                                                        SET @MaxStart = (SELECT MAX(Ecm_StartDate)		
                                                                           FROM T_EmployeeCostcenterMovement		
                                                                          WHERE Ecm_EmployeeID = '{0}' )

                                                    UPDATE T_EmployeeCostcenterMovement			
                                                       SET Ecm_EndDate = Convert(datetime,'{1}') - 1 			
                                                     WHERE Ecm_Employeeid = '{0}'	
                                                       AND Ecm_Startdate = @MaxStart
                                    
                                                    INSERT INTO T_EmployeeCostcenterMovement
														 ( Ecm_EmployeeID
														 , Ecm_StartDate
														 , Ecm_EndDate
														 , Ecm_CostCenterCode
														 , Usr_Login
														 , Ludatetime
													     )		
                                                    SELECT '{0}'	
                                                         , '{1}'	
                                                         , null	
                                                         , '{2}'
                                                         , '{3}'
                                                         , GETDATE() ";

            string sqlInsertCostcenterMovement2 = @"INSERT INTO T_EmployeeCostcenterMovement
														 ( Ecm_EmployeeID
														 , Ecm_StartDate
														 , Ecm_EndDate
														 , Ecm_CostCenterCode
														 , Usr_Login
														 , Ludatetime
													     )			
                                                    SELECT '{0}'	
                                                         , '{1}'	
                                                         , null	
                                                         , '{2}'
                                                         , '{3}'
                                                         , GETDATE() ";

            #endregion
            if (dsTemp.Tables[1].Rows.Count > 0)
            {
                if (Convert.ToDateTime(date) <= Convert.ToDateTime(dsTemp.Tables[1].Rows[0]["Ecm_StartDate"].ToString()))
                {
                    dal.ExecuteNonQuery(string.Format(sqlInsertUpdateCostcenterMovement, employeeId, date, toCC, session["userLogged"].ToString()), CommandType.Text);
                }
                else
                {
                    dal.ExecuteNonQuery(string.Format(sqlInsertCostcenterMovement1, employeeId, date, toCC, session["userLogged"].ToString()), CommandType.Text);
                }
            }
            else//no Employee Costcetner Movement Trail
            {
                dal.ExecuteNonQuery(string.Format(sqlInsertCostcenterMovement2, employeeId, date, toCC, session["userLogged"].ToString()), CommandType.Text);
            }
            #endregion

            #region Step 3 - Cascade to transactions

            string sqlGetEmployeeMovement = @"  SELECT ISNULL(Ecm_StartDate, '01/01/1900') [Ecm_StartDate]
                                                     , ISNULL(Ecm_EndDate, '12/31/9999') [Ecm_EndDate] 
                                                     , Ecm_CostcenterCode 
                                                  FROM T_EmployeeCostcenterMovement
                                                 WHERE Ecm_EmployeeId = '{0}' 
                                                   AND Ecm_StartDate >= '{1}' 
                                                 ORDER BY 1 ASC";
            //taysa robert comment sa ni hap :)
            string sqlUpdateEmployeeMaster = @"--if((select mve_from from T_Movement
                                                     --where Mve_type='C' 
		                                               -- and Mve_EmployeeId='{2}'
		                                               -- and Mve_ControlNo='{3}')!=
                                                   -- (select Emt_CostCenterCode from T_EmployeeMaster
	                                                  --  where Emt_EmployeeID='{2}'))
                                               -- begin
	                                              --  RAISERROR('An error occured in the Inner procedure.',17,1)
                                               -- end
                                                UPDATE T_EmployeeMaster		
                                                   SET Emt_CostCenterCode = '{0}'
                                                     , Emt_CostCenterDate = '{1}'
                                                 WHERE Emt_EmployeeId = '{2}' ";

            string sqlUpdateTransactionCostcenter = @" UPDATE T_EmployeeOvertime
                                                          SET Eot_Costcenter = '{0}'
                                                        WHERE Eot_OvertimeDate BETWEEN '{1}' AND '{2}' 
                                                          AND Eot_EmployeeId = '{3}'

                                                       UPDATE T_EmployeeOvertimeHist
                                                          SET Eot_Costcenter = '{0}'
                                                        WHERE Eot_OvertimeDate BETWEEN '{1}' AND '{2}' 
                                                          AND Eot_EmployeeId = '{3}' 

                                                       UPDATE T_EmployeeLeaveAvailment
                                                          SET Elt_Costcenter = '{0}'
                                                        WHERE Elt_LeaveDate BETWEEN '{1}' AND '{2}' 
                                                          AND Elt_EmployeeId = '{3}' 

                                                       UPDATE T_EmployeeLeaveAvailmentHist
                                                          SET Elt_Costcenter = '{0}'
                                                        WHERE Elt_LeaveDate BETWEEN '{1}' AND '{2}' 
                                                          AND Elt_EmployeeId = '{3}'  

                                                       UPDATE T_TimeRecMod
                                                          SET Trm_Costcenter = '{0}'
                                                        WHERE Trm_ModDate BETWEEN '{1}' AND '{2}' 
                                                          AND Trm_EmployeeId = '{3}' 

                                                       UPDATE T_AddressMovement
                                                          SET Amt_Costcenter = '{0}'
                                                        WHERE Amt_EffectivityDate BETWEEN '{1}' AND '{2}' 
                                                          AND Amt_EmployeeId = '{3}' 

                                                       UPDATE T_Movement
                                                          SET Mve_Costcenter = '{0}'
                                                        WHERE Mve_EffectivityDate BETWEEN '{1}' AND '{2}' 
                                                          AND Mve_EmployeeId = '{3}' 

                                                       UPDATE T_BeneficiaryUpdate
                                                          SET But_Costcenter = '{0}'
                                                        WHERE But_EffectivityDate BETWEEN '{1}' AND '{2}' 
                                                          AND But_EmployeeId = '{3}'

                                                       UPDATE T_PersonnelInfoMovement
                                                          SET Pit_Costcenter = '{0}'
                                                        WHERE Pit_EffectivityDate BETWEEN '{1}' AND '{2}' 
                                                          AND Pit_EmployeeId = '{3}' ";



            dtFinal = dal.ExecuteDataSet(string.Format(sqlGetEmployeeMovement, employeeId, date), CommandType.Text).Tables[0];
            for (int i = 0; i < dtFinal.Rows.Count; i++)
            {//robert add controlno
                //Step 3.1 - Update Emt_CostcenterCde in T_EMployeeMaster if start date is within quincena
                if ( (Convert.ToDateTime(dtFinal.Rows[i]["Ecm_StartDate"].ToString()) >= Convert.ToDateTime(dsTemp.Tables[0].Rows[0]["Ppm_StartCycle"].ToString()))
                  && (Convert.ToDateTime(dtFinal.Rows[i]["Ecm_StartDate"].ToString()) <= Convert.ToDateTime(dsTemp.Tables[0].Rows[0]["Ppm_EndCycle"].ToString())) )
                {
                    dal.ExecuteNonQuery(string.Format(sqlUpdateEmployeeMaster, dtFinal.Rows[i]["Ecm_CostcenterCode"].ToString()
                                                                             , dtFinal.Rows[i]["Ecm_StartDate"].ToString()
                                                                             , employeeId, controlNo), CommandType.Text);
                }

                //Step 3.2 - Update all transaction between Ecm_StartDate and Ecm_EndDate
                dal.ExecuteNonQuery(string.Format(sqlUpdateTransactionCostcenter, dtFinal.Rows[i]["Ecm_CostcenterCode"].ToString()
                                                                                , dtFinal.Rows[i]["Ecm_StartDate"].ToString()
                                                                                , dtFinal.Rows[i]["Ecm_EndDate"].ToString()
                                                                                , employeeId), CommandType.Text);
            }
            #endregion
        }

        public void EmployeeLogLedgerUpdateRestday(string employeeId
                                                  , string fromDate
                                                  , string toDate
                                                  , string userlogin
                                                  , DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", employeeId);
            paramInfo[1] = new ParameterInfo("@FromDate", fromDate);
            paramInfo[2] = new ParameterInfo("@ToDate", toDate);
            paramInfo[3] = new ParameterInfo("@Usr_Login", userlogin);

            string LedgerTable = GetLedgerTableFromDate(fromDate);
            #endregion

            #region SQL Query
            string sqlQuery = @"
                                 -- Old Resday
                                 UPDATE T_EmployeeLogledger		
                                    SET Ell_DayCode = CASE WHEN Ell_Holiday = 0  then 'REG' 			
		                                                   ELSE (SELECT Hmt_HolidayCode  
                                                                   FROM T_HolidayMaster
			                                                      WHERE Hmt_HolidayDate = Ell_ProcessDate 
			                                                        AND Hmt_ApplicCity = Ell_LocationCode) 
		                                                    END
                                      , Ell_Restday = 0
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()
                                  WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	                                AND Ell_ProcessDate = @FromDate		

                                UPDATE T_EmployeeLogledgerHist			
                                    SET Ell_DayCode = CASE WHEN Ell_Holiday = 0  then 'REG' 			
		                                                   ELSE (SELECT Hmt_HolidayCode  
                                                                   FROM T_HolidayMaster
			                                                      WHERE Hmt_HolidayDate = Ell_ProcessDate 
			                                                        AND Hmt_ApplicCity = Ell_LocationCode) 
		                                                    END
                                      , Ell_Restday = 0
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()
                                  WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	                                AND Ell_ProcessDate = @FromDate		

			
                                 -- New Restday			
                                 UPDATE T_EmployeeLogledger		
                                    SET Ell_DayCode = 'REST'			
	                                  , Ell_Restday = 1	
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()	
                                  WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	                                AND Ell_ProcessDate = @ToDate

			
                                 -- New Restday			
                                 UPDATE T_EmployeeLogledgerHist		
                                    SET Ell_DayCode = 'REST'			
	                                  , Ell_Restday = 1	
                                      , Usr_Login  = @Usr_Login
                                      , Ludatetime = getdate()	
                                  WHERE Ell_EmployeeId =  @Ell_EmployeeId			
	                                AND Ell_ProcessDate = @ToDate";
            #endregion

            if (!LedgerTable.ToString().Equals(string.Empty))
            {
                if (LedgerTable.ToString().ToUpper().Equals("T_EMPLOYEELOGLEDGERHIST"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(employeeId
                                        , MethodsLibrary.Methods.getPayPeriod(fromDate)
                                        , GetPayPeriod()))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(employeeId
                                                , MethodsLibrary.Methods.getPayPeriod(fromDate)
                                                , dal);
                    }
                    else if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(employeeId
                                        , MethodsLibrary.Methods.getPayPeriod(toDate)
                                        , GetPayPeriod()))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(employeeId
                                                , MethodsLibrary.Methods.getPayPeriod(toDate)
                                                , dal);
                    }

                }

                dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
        }
        #endregion
    }
}
