using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

/// <summary>
/// Summary description for PersonnelBL
/// </summary>
namespace Payroll.DAL
{
    public class PersonnelBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public PersonnelBL()
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

        #region Tax Code and Civil Status Updating
        public DataRow getTaxCivilInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Pit_ControlNo [Control No]
                                  , Pit_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Convert(varchar(10), Pit_EffectivityDate, 101) [Effectivity Date]
                                  , Pit_MoveType [Type]
                                  , Pit_From [From Tax Code]
                                  , ADFTAX.Adt_AccountDesc [From Tax Desc]
                                  , Pit_To [To Tax Code]
                                  , ADTTAX.Adt_AccountDesc [To Tax Desc]
                                  , Pit_Filler1 [From Civil Code]
                                  , ADFCIVIL.Adt_AccountDesc [From Civil Desc]
                                  , Pit_Filler2 [To Civil Code]
                                  , ADTCIVIL.Adt_AccountDesc [To Civil Desc]
                                  , Pit_Reason [Reason]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_PersonnelInfoMovement
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Pit_EmployeeId
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Pit_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail ADFTAX 
                                 ON ADFTAX.Adt_AccountCode = Pit_From 
                                AND ADFTAX.Adt_AccountType =  'TAXCODE'
                               LEFT JOIN T_AccountDetail ADTTAX 
                                 ON ADTTAX.Adt_AccountCode = Pit_To 
                                AND ADTTAX.Adt_AccountType =  'TAXCODE'
                               LEFT JOIN T_AccountDetail ADFCIVIL 
                                 ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
                                AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
                               LEFT JOIN T_AccountDetail ADTCIVIL 
                                 ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
                                AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Pit_ControlNo
                              WHERE Pit_ControlNo = @ControlNo";
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

        public void CreateTXRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[15];
            paramDetails[0] = new ParameterInfo("@Pit_ControlNo", rowDetails["Pit_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Pit_CurrentPayPeriod", rowDetails["Pit_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Pit_EmployeeId", rowDetails["Pit_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Pit_EffectivityDate", rowDetails["Pit_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Pit_MoveType", rowDetails["Pit_MoveType"]);
            paramDetails[5] = new ParameterInfo("@Pit_From", rowDetails["Pit_From"]);
            paramDetails[6] = new ParameterInfo("@Pit_To", rowDetails["Pit_To"]);
            paramDetails[7] = new ParameterInfo("@Pit_Reason", rowDetails["Pit_Reason"]);
            paramDetails[8] = new ParameterInfo("@Pit_Filler1", rowDetails["Pit_Filler1"]);
            paramDetails[9] = new ParameterInfo("@Pit_Filler2", rowDetails["Pit_Filler2"]);
            paramDetails[10] = new ParameterInfo("@Pit_Filler3", rowDetails["Pit_Filler3"]);
            paramDetails[11] = new ParameterInfo("@Pit_EndorsedDateToChecker", rowDetails["Pit_EndorsedDateToChecker"]);
            paramDetails[12] = new ParameterInfo("@Pit_Status", rowDetails["Pit_Status"]);
            paramDetails[13] = new ParameterInfo("@Pit_BatchNo", rowDetails["Pit_BatchNo"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Pit_EmployeeId
                                                      AND @Pit_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Pit_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Pit_EmployeeId
                                                    AND @Pit_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END

                                   INSERT INTO T_PersonnelInfoMovement
	                                    ( Pit_ControlNo
	                                    , Pit_CurrentPayPeriod
	                                    , Pit_EmployeeId
	                                    , Pit_EffectivityDate
	                                    , Pit_AppliedDate
	                                    , Pit_Costcenter
	                                    , Pit_CostcenterLine
	                                    , Pit_MoveType
	                                    , Pit_From
	                                    , Pit_To
	                                    , Pit_Reason
	                                    , Pit_Filler1
	                                    , Pit_Filler2
	                                    , Pit_Filler3
	                                    , Pit_Status
	                                    , Pit_BatchNo
	                                    , Usr_Login
	                                    , Ludatetime)
                                  VALUES( @Pit_ControlNo
	                                    , @Pit_CurrentPayPeriod
	                                    , @Pit_EmployeeId
	                                    , @Pit_EffectivityDate
	                                    , GETDATE()
	                                    , @Costcenter
                                        , @LineCode
	                                    , @Pit_MoveType
	                                    , @Pit_From
	                                    , @Pit_To
	                                    , @Pit_Reason
	                                    , @Pit_Filler1
	                                    , @Pit_Filler2
	                                    , @Pit_Filler3
	                                    , @Pit_Status
	                                    , @Pit_BatchNo
	                                    , @Usr_Login
	                                    , GETDATE())";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateTXRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[15];
            paramDetails[0] = new ParameterInfo("@Pit_ControlNo", rowDetails["Pit_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Pit_CurrentPayPeriod", rowDetails["Pit_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@Pit_EmployeeId", rowDetails["Pit_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@Pit_EffectivityDate", rowDetails["Pit_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@Pit_MoveType", rowDetails["Pit_MoveType"]);
            paramDetails[5] = new ParameterInfo("@Pit_From", rowDetails["Pit_From"]);
            paramDetails[6] = new ParameterInfo("@Pit_To", rowDetails["Pit_To"]);
            paramDetails[7] = new ParameterInfo("@Pit_Reason", rowDetails["Pit_Reason"]);
            paramDetails[8] = new ParameterInfo("@Pit_Filler1", rowDetails["Pit_Filler1"]);
            paramDetails[9] = new ParameterInfo("@Pit_Filler2", rowDetails["Pit_Filler2"]);
            paramDetails[10] = new ParameterInfo("@Pit_Filler3", rowDetails["Pit_Filler3"]);
            paramDetails[11] = new ParameterInfo("@Pit_EndorsedDateToChecker", rowDetails["Pit_EndorsedDateToChecker"]);
            paramDetails[12] = new ParameterInfo("@Pit_Status", rowDetails["Pit_Status"]);
            paramDetails[13] = new ParameterInfo("@Pit_BatchNo", rowDetails["Pit_BatchNo"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                UPDATE T_PersonnelInfoMovement
                                   SET Pit_EmployeeId = @Pit_EmployeeId
                                     , Pit_EffectivityDate = @Pit_EffectivityDate
                                     , Pit_From = @Pit_From
                                     , Pit_To = @Pit_To
                                     , Pit_Reason = @Pit_Reason
                                     , Pit_Filler1 = @Pit_Filler1
                                     , Pit_Filler2 = @Pit_Filler2
                                     , Pit_Filler3 = @Pit_Filler3
                                     , Pit_Status = @Pit_Status
                                     , Pit_BatchNo = @Pit_BatchNo
                                     , Usr_Login = @Usr_Login
                                     , Ludatetime = GETDATE()
                                 WHERE Pit_ControlNo = @Pit_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateTXRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateTXRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateTXRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Pit_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Pit_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Pit_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Pit_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Pit_CheckedBy", " ");
                }
                else if (rowDetails["Pit_Status"].ToString().Equals("5")
                    || rowDetails["Pit_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Pit_CheckedBy = @Pit_CheckedBy ,
                                Pit_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Pit_CheckedBy", rowDetails["Pit_CheckedBy"]);
                }
                else if (rowDetails["Pit_Status"].ToString().Equals("7")
                    || rowDetails["Pit_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Pit_Checked2By = @Pit_Checked2By ,
                                Pit_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Pit_Checked2By", rowDetails["Pit_Checked2By"]);
                }
                else if (rowDetails["Pit_Status"].ToString().Equals("9")
                    || rowDetails["Pit_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Pit_ApprovedBy = @Pit_ApprovedBy ,
                                Pit_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Pit_ApprovedBy", rowDetails["Pit_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Pit_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Pit_Status", rowDetails["Pit_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Pit_ControlNo", rowDetails["Pit_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_PersonnelInfoMovement
                                    SET {0}
                                        Pit_Status = @Pit_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Pit_ControlNo = @Pit_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateEmployeeMasterTaxCivil(string controlNo, string userLogged, DALHelper dal)
        {
            DataRow dr = getTaxCivilInfo(controlNo);
            if (!dr["To Tax Code"].ToString().Equals(string.Empty) && !dr["To Tax Code"].ToString().Equals(dr["From Tax Code"].ToString()))
            {
                UpdateTaxCode(dr["ID No"].ToString()
                             , dr["From Tax Code"].ToString()
                             , dr["To Tax Code"].ToString()
                             , userLogged
                             , dal);
            }
            if (!dr["To Civil Code"].ToString().Equals(string.Empty) && !dr["To Civil Code"].ToString().Equals(dr["From Civil Code"].ToString()))
            {
                UpdateCivilStatus(dr["ID No"].ToString()
                             , dr["From Civil Code"].ToString()
                             , dr["To Civil Code"].ToString()
                             , userLogged
                             , dal);
            }
        }

        private void UpdateTaxCode(string employeeId, string fromTaxCode, string toTaxCode, string userLogged, DALHelper dal)
        {
            #region SQL
            string sql = @"declare @SeqNo char(2)
                               SET @SeqNo = (SELECT (REPLICATE('0', 2 - LEN(ISNULL(Convert(int, MAX(Att_SeqNo)), 0 )))
                                                  + Convert(varchar(2),ISNULL(Convert(int, MAX(Att_SeqNo)), 0 )+ 1) )
                                               FROM T_AuditTrail
                                              WHERE Att_ColId = @Att_ColId
                                                AND Att_EmployeeId = @Att_EmployeeId) 
                            INSERT INTO T_AuditTrail
                                 ( Att_ColId
                                 , Att_EmployeeId
                                 , Att_SeqNo
                                 , Att_PreviousValue
                                 , Att_NewValue
                                 , Usr_Login
                                 , Ludatetime ) 
                            VALUES 
                                 ( @Att_ColId
                                 , @Att_EmployeeId
                                 , @SeqNo
                                 , @Att_PreviousValue
                                 , @Att_NewValue
                                 , @Usr_Login
                                 , getdate() )

                        UPDATE T_EmployeeMaster
                           SET Emt_TaxCode = @Att_NewValue
                         WHERE Emt_EmployeeId = @Att_EmployeeId  ";
            #endregion
            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@Att_ColId", "TaxCode");
            param[1] = new ParameterInfo("@Att_EmployeeId", employeeId);
            param[2] = new ParameterInfo("@Att_PreviousValue", fromTaxCode);
            param[3] = new ParameterInfo("@Att_NewValue", toTaxCode);
            param[4] = new ParameterInfo("@Usr_Login", userLogged);

            dal.ExecuteNonQuery(sql, CommandType.Text, param);
        }

        private void UpdateCivilStatus(string employeeId, string fromCivilStatus, string toCivilStatus, string userLogged, DALHelper dal)
        {
            #region SQL
            string sql = @"declare @SeqNo char(2)
                               SET @SeqNo = (SELECT (REPLICATE('0', 2 - LEN(ISNULL(Convert(int, MAX(Att_SeqNo)), 0 )))
                                                  + Convert(varchar(2),ISNULL(Convert(int, MAX(Att_SeqNo)), 0 )+ 1) )
                                               FROM T_AuditTrail
                                              WHERE Att_ColId = @Att_ColId
                                                AND Att_EmployeeId = @Att_EmployeeId) 
                            INSERT INTO T_AuditTrail
                                 ( Att_ColId
                                 , Att_EmployeeId
                                 , Att_SeqNo
                                 , Att_PreviousValue
                                 , Att_NewValue
                                 , Usr_Login
                                 , Ludatetime ) 
                            VALUES 
                                 ( @Att_ColId
                                 , @Att_EmployeeId
                                 , @SeqNo
                                 , @Att_PreviousValue
                                 , @Att_NewValue
                                 , @Usr_Login
                                 , getdate() )

                        UPDATE T_EmployeeMaster
                           SET Emt_CivilStatus = @Att_NewValue
                         WHERE Emt_EmployeeId = @Att_EmployeeId  ";
            #endregion
            ParameterInfo[] param = new ParameterInfo[5];
            param[0] = new ParameterInfo("@Att_ColId", "CivilStatus");
            param[1] = new ParameterInfo("@Att_EmployeeId", employeeId);
            param[2] = new ParameterInfo("@Att_PreviousValue", fromCivilStatus);
            param[3] = new ParameterInfo("@Att_NewValue", toCivilStatus);
            param[4] = new ParameterInfo("@Usr_Login", userLogged);

            dal.ExecuteNonQuery(sql, CommandType.Text, param);
        } 
        #endregion

        #region Beneficiary Updating
        public DataRow getBeneficiaryInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT But_ControlNo [Control No]
                                  , But_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_MiddleName [Middlename]
                                  , But_SeqNo [Seq No]
                                  , Convert(varchar(10), But_EffectivityDate, 101) [Effectivity Date]
                                  , But_Type [Type]
                                  , But_Lastname [Beneficiary Lastname]
                                  , But_Firstname [Beneficiary Firstname]
                                  , But_Middlename [Beneficiary Middlename]
                                  , Convert(varchar(10), But_Birthdate, 101) [Birthdate]
                                  , But_Relationship [Relationship Code]
                                  , AD2.Adt_AccountDesc [Relationship Desc]
                                  , But_Hierarchy [Hierarchy Code]
                                  , AD3.Adt_AccountDesc [Hierarchy Desc]
                                  , But_HMODependent [HMO]
                                  , But_InsuranceDependent [Insurance]
                                  , But_BIRDependent [BIR]
                                  , But_AccidentDependent [Accident]
                                  , Convert(varchar(10), But_DeceasedDate, 101) [Deceased Date]
                                  , Convert(varchar(10), But_CancelDate, 101) [Cancelled Date]
                                  , But_Gender[Gender]
								  , But_Occupation[Occupation]
								  , But_Company[Company]
								  , But_CivilStatus[Civil Status]
                                  , But_Reason [Reason]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_BeneficiaryUpdate
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeId = But_EmployeeId
                               LEFT JOIN T_AccountDetail AD1
                                 ON AD1.Adt_AccountCode = But_Status
                                AND AD1.Adt_AccountType = 'WFSTATUS'
                               LEFT JOIN T_AccountDetail AD2
                                 ON AD2.Adt_AccountCode = But_Relationship
                                AND AD2.Adt_AccountType = 'RELATION'
                               LEFT JOIN T_AccountDetail AD3
                                 ON AD3.Adt_AccountCode = But_Hierarchy
                                AND AD3.Adt_AccountType = 'HIERARCHDP'
                               LEFT JOIN T_TransactionRemarks
                                 ON Trm_ControlNo = But_ControlNo
                              WHERE But_ControlNo = @ControlNo";
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

        public void CreateBFRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[27];
            paramDetails[0] = new ParameterInfo("@But_ControlNo", rowDetails["But_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@But_CurrentPayPeriod", rowDetails["But_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@But_EmployeeId", rowDetails["But_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@But_EffectivityDate", rowDetails["But_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@But_Type", rowDetails["But_Type"]);
            paramDetails[5] = new ParameterInfo("@But_Lastname", rowDetails["But_Lastname"]);
            paramDetails[6] = new ParameterInfo("@But_Firstname", rowDetails["But_Firstname"]);
            paramDetails[7] = new ParameterInfo("@But_Middlename", rowDetails["But_Middlename"]);
            paramDetails[8] = new ParameterInfo("@But_Birthdate", rowDetails["But_Birthdate"]);
            paramDetails[9] = new ParameterInfo("@But_Relationship", rowDetails["But_Relationship"]);
            paramDetails[10] = new ParameterInfo("@But_Hierarchy", rowDetails["But_Hierarchy"]);
            paramDetails[11] = new ParameterInfo("@But_HMODependent", rowDetails["But_HMODependent"]);
            paramDetails[12] = new ParameterInfo("@But_InsuranceDependent", rowDetails["But_InsuranceDependent"]);
            paramDetails[13] = new ParameterInfo("@But_BIRDependent", rowDetails["But_BIRDependent"]);
            paramDetails[14] = new ParameterInfo("@But_AccidentDependent", rowDetails["But_AccidentDependent"]);
            paramDetails[15] = new ParameterInfo("@But_DeceasedDate", Convert.ToDateTime(rowDetails["But_DeceasedDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_DeceasedDate"]);
            paramDetails[16] = new ParameterInfo("@But_CancelDate", Convert.ToDateTime(rowDetails["But_CancelDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_CancelDate"]);
            paramDetails[17] = new ParameterInfo("@But_Reason", rowDetails["But_Reason"]);
            paramDetails[18] = new ParameterInfo("@But_EndorsedDateToChecker", rowDetails["But_EndorsedDateToChecker"]);
            paramDetails[19] = new ParameterInfo("@But_Status", rowDetails["But_Status"]);
            paramDetails[20] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[21] = new ParameterInfo("@But_SeqNo", rowDetails["But_SeqNo"]);
            paramDetails[22] = new ParameterInfo("@But_Gender", rowDetails["But_Gender"]);
            paramDetails[23] = new ParameterInfo("@But_Occupation", rowDetails["But_Occupation"]);
            paramDetails[24] = new ParameterInfo("@But_Company", rowDetails["But_Company"]);
            paramDetails[25] = new ParameterInfo("@But_CivilStatus", rowDetails["But_CivilStatus"]);
            paramDetails[26] = new ParameterInfo("@But_Remarks", rowDetails["But_Remarks"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"  
                                Declare @seqNo varchar(3)
							    set  @seqNo= (select iif(ISNULL(Max(Convert(int,But_seqno))+1,'0')<>'',ISNULL(Max(Convert(int,But_seqno))+1,'0'),1) from T_BeneficiaryUpdate
                                                    WHERE But_EmployeeId=@But_EmployeeId)
                                set @seqNo =(select REPLICATE(0,3-Len(@seqNo))+@seqNo)     
                                 DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @But_EmployeeId
                                                      AND @But_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @But_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @But_EmployeeId
                                                    AND @But_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END                

                                   INSERT INTO T_BeneficiaryUpdate
	                                    ( But_ControlNo
                                        , But_SeqNo
	                                    , But_CurrentPayPeriod
	                                    , But_EmployeeId
	                                    , But_EffectivityDate
	                                    , But_AppliedDate
	                                    , But_Costcenter
	                                    , But_CostcenterLine
	                                    , But_Type
	                                    , But_Lastname
                                        , But_Firstname
                                        , But_Middlename
                                        , But_Birthdate
                                        , But_Relationship
                                        , But_Hierarchy
                                        , But_HMODependent
                                        , But_InsuranceDependent
                                        , But_BIRDependent
                                        , But_AccidentDependent
                                        , But_DeceasedDate
                                        , But_CancelDate
                                        , But_Gender
                                        , But_CivilStatus
                                        , But_Company
                                        , But_Occupation
                                        , But_Reason
                                        , But_Remarks
	                                    , But_Status
	                                    , Usr_Login
	                                    , Ludatetime)
                                  VALUES( @But_ControlNo
                                        , @seqNo
	                                    , @But_CurrentPayPeriod
	                                    , @But_EmployeeId
	                                    , @But_EffectivityDate
	                                    , GETDATE()
	                                    , @Costcenter
                                        , @LineCode
	                                    , @But_Type
	                                    , @But_Lastname
                                        , @But_Firstname
                                        , @But_Middlename
                                        , @But_Birthdate
                                        , @But_Relationship
                                        , @But_Hierarchy
                                        , @But_HMODependent
                                        , @But_InsuranceDependent
                                        , @But_BIRDependent
                                        , @But_AccidentDependent
                                        , CASE WHEN CONVERT(varchar(10),@But_DeceasedDate) = '01/01/0001'
                                               THEN NULL
                                               ELSE @But_DeceasedDate
                                           END
                                        , CASE WHEN CONVERT(varchar(10),@But_CancelDate) = '01/01/0001'
                                               THEN NULL
                                               ELSE @But_CancelDate
                                           END
                                        , @But_Gender
                                        , @But_CivilStatus
                                        , @But_Company
                                        , @But_Occupation
                                        , @But_Reason
                                        , @But_Remarks
	                                    , @But_Status
	                                    , @Usr_Login
	                                    , GETDATE()) ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }


       public void UpdateExistingBFRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[27];
            paramDetails[0] = new ParameterInfo("@But_ControlNo", rowDetails["But_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@But_CurrentPayPeriod", rowDetails["But_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@But_EmployeeId", rowDetails["But_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@But_EffectivityDate", rowDetails["But_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@But_Type", rowDetails["But_Type"]);
            paramDetails[5] = new ParameterInfo("@But_Lastname", rowDetails["But_Lastname"]);
            paramDetails[6] = new ParameterInfo("@But_Firstname", rowDetails["But_Firstname"]);
            paramDetails[7] = new ParameterInfo("@But_Middlename", rowDetails["But_Middlename"]);
            paramDetails[8] = new ParameterInfo("@But_Birthdate", rowDetails["But_Birthdate"]);
            paramDetails[9] = new ParameterInfo("@But_Relationship", rowDetails["But_Relationship"]);
            paramDetails[10] = new ParameterInfo("@But_Hierarchy", rowDetails["But_Hierarchy"]);
            paramDetails[11] = new ParameterInfo("@But_HMODependent", rowDetails["But_HMODependent"]);
            paramDetails[12] = new ParameterInfo("@But_InsuranceDependent", rowDetails["But_InsuranceDependent"]);
            paramDetails[13] = new ParameterInfo("@But_BIRDependent", rowDetails["But_BIRDependent"]);
            paramDetails[14] = new ParameterInfo("@But_AccidentDependent", rowDetails["But_AccidentDependent"]);
            paramDetails[15] = new ParameterInfo("@But_DeceasedDate", Convert.ToDateTime(rowDetails["But_DeceasedDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_DeceasedDate"]);
            paramDetails[16] = new ParameterInfo("@But_CancelDate", Convert.ToDateTime(rowDetails["But_CancelDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_CancelDate"]);
            paramDetails[17] = new ParameterInfo("@But_Reason", rowDetails["But_Reason"]);
            paramDetails[18] = new ParameterInfo("@But_EndorsedDateToChecker", rowDetails["But_EndorsedDateToChecker"]);
            paramDetails[19] = new ParameterInfo("@But_Status", rowDetails["But_Status"]);
            paramDetails[20] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[21] = new ParameterInfo("@But_SeqNo", rowDetails["But_SeqNo"]);
            paramDetails[22] = new ParameterInfo("@But_Gender", rowDetails["But_Gender"]);
            paramDetails[23] = new ParameterInfo("@But_Occupation", rowDetails["But_Occupation"]);
            paramDetails[24] = new ParameterInfo("@But_Company", rowDetails["But_Company"]);
            paramDetails[25] = new ParameterInfo("@But_CivilStatus", rowDetails["But_CivilStatus"]);
            paramDetails[26] = new ParameterInfo("@But_Remarks", rowDetails["But_Remarks"]);
            #endregion

            #region SQL Query
            string sql = @"
                                 DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @But_EmployeeId
                                                      AND @But_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @But_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @But_EmployeeId
                                                    AND @But_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END
                          
                                   INSERT INTO T_BeneficiaryUpdate
	                                    ( But_ControlNo
                                        , But_SeqNo
	                                    , But_CurrentPayPeriod
	                                    , But_EmployeeId
	                                    , But_EffectivityDate
	                                    , But_AppliedDate
	                                    , But_Costcenter
	                                    , But_CostcenterLine
	                                    , But_Type
	                                    , But_Lastname
                                        , But_Firstname
                                        , But_Middlename
                                        , But_Birthdate
                                        , But_Relationship
                                        , But_Hierarchy
                                        , But_HMODependent
                                        , But_InsuranceDependent
                                        , But_BIRDependent
                                        , But_AccidentDependent
                                        , But_DeceasedDate
                                        , But_CancelDate
                                        , But_Gender
                                        , But_CivilStatus
                                        , But_Company
                                        , But_Occupation
                                        , But_Reason
                                        , But_Remarks
	                                    , But_Status
	                                    , Usr_Login
	                                    , Ludatetime)
                                  VALUES( @But_ControlNo
                                        , @But_SeqNo
	                                    , @But_CurrentPayPeriod
	                                    , @But_EmployeeId
	                                    , @But_EffectivityDate
	                                    , GETDATE()
	                                    , @Costcenter
                                        , @LineCode
	                                    , @But_Type
	                                    , @But_Lastname
                                        , @But_Firstname
                                        , @But_Middlename
                                        , @But_Birthdate
                                        , @But_Relationship
                                        , @But_Hierarchy
                                        , @But_HMODependent
                                        , @But_InsuranceDependent
                                        , @But_BIRDependent
                                        , @But_AccidentDependent
                                        , CASE WHEN CONVERT(varchar(10),@But_DeceasedDate) = '01/01/0001'
                                               THEN NULL
                                               ELSE @But_DeceasedDate
                                           END
                                        , CASE WHEN CONVERT(varchar(10),@But_CancelDate) = '01/01/0001'
                                               THEN NULL
                                               ELSE @But_CancelDate
                                           END
                                        , @But_Gender
                                        , @But_CivilStatus
                                        , @But_Company
                                        , @But_Occupation
                                        , @But_Reason
                                        , @But_Remarks
	                                    , @But_Status
	                                    , @Usr_Login
	                                    , GETDATE())
                                  ";
            #endregion

            dal.ExecuteNonQuery(sql, CommandType.Text, paramDetails);
        }
                            
        public void UpdateBFRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[27];
            paramDetails[0] = new ParameterInfo("@But_ControlNo", rowDetails["But_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@But_CurrentPayPeriod", rowDetails["But_CurrentPayPeriod"]);
            paramDetails[2] = new ParameterInfo("@But_EmployeeId", rowDetails["But_EmployeeId"]);
            paramDetails[3] = new ParameterInfo("@But_EffectivityDate", rowDetails["But_EffectivityDate"]);
            paramDetails[4] = new ParameterInfo("@But_Type", rowDetails["But_Type"]);
            paramDetails[5] = new ParameterInfo("@But_Lastname", rowDetails["But_Lastname"]);
            paramDetails[6] = new ParameterInfo("@But_Firstname", rowDetails["But_Firstname"]);
            paramDetails[7] = new ParameterInfo("@But_Middlename", rowDetails["But_Middlename"]);
            paramDetails[8] = new ParameterInfo("@But_Birthdate", rowDetails["But_Birthdate"]);
            paramDetails[9] = new ParameterInfo("@But_Relationship", rowDetails["But_Relationship"]);
            paramDetails[10] = new ParameterInfo("@But_Hierarchy", rowDetails["But_Hierarchy"]);
            paramDetails[11] = new ParameterInfo("@But_HMODependent", rowDetails["But_HMODependent"]);
            paramDetails[12] = new ParameterInfo("@But_InsuranceDependent", rowDetails["But_InsuranceDependent"]);
            paramDetails[13] = new ParameterInfo("@But_BIRDependent", rowDetails["But_BIRDependent"]);
            paramDetails[14] = new ParameterInfo("@But_AccidentDependent", rowDetails["But_AccidentDependent"]);
            paramDetails[15] = new ParameterInfo("@But_DeceasedDate", Convert.ToDateTime(rowDetails["But_DeceasedDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_DeceasedDate"]);
            paramDetails[16] = new ParameterInfo("@But_CancelDate", Convert.ToDateTime(rowDetails["But_CancelDate"]).Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")) ? DBNull.Value : rowDetails["But_CancelDate"]);
            paramDetails[17] = new ParameterInfo("@But_Reason", rowDetails["But_Reason"]);
            paramDetails[18] = new ParameterInfo("@But_EndorsedDateToChecker", rowDetails["But_EndorsedDateToChecker"]);
            paramDetails[19] = new ParameterInfo("@But_Status", rowDetails["But_Status"]);
            paramDetails[20] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[21] = new ParameterInfo("@But_SeqNo", rowDetails["But_SeqNo"]);
            paramDetails[22] = new ParameterInfo("@But_Gender", rowDetails["But_Gender"]);
            paramDetails[23] = new ParameterInfo("@But_Occupation", rowDetails["But_Occupation"]);
            paramDetails[24] = new ParameterInfo("@But_Company", rowDetails["But_Company"]);
            paramDetails[25] = new ParameterInfo("@But_CivilStatus", rowDetails["But_CivilStatus"]);
            paramDetails[26] = new ParameterInfo("@But_Remarks", rowDetails["But_Remarks"]);
            #endregion

            #region SQL Query
            string sqlUpdate = @"  UPDATE T_BeneficiaryUpdate
	                                  SET But_EffectivityDate = @But_EffectivityDate
	                                    , But_Lastname = @But_Lastname
                                        , But_Firstname = @But_Firstname
                                        , But_Middlename = @But_Middlename
                                        , But_Birthdate = @But_Birthdate
                                        , But_Relationship = @But_Relationship
                                        , But_Hierarchy = @But_Hierarchy
                                        , But_HMODependent = @But_HMODependent
                                        , But_InsuranceDependent = @But_InsuranceDependent
                                        , But_BIRDependent = @But_BIRDependent
                                        , But_AccidentDependent = @But_AccidentDependent
                                        , But_DeceasedDate = CASE WHEN CONVERT(varchar(10),@But_DeceasedDate) = '01/01/0001'
                                                                  THEN NULL
                                                                  ELSE @But_DeceasedDate
                                                              END
                                        , But_CancelDate = CASE WHEN CONVERT(varchar(10),@But_CancelDate) = '01/01/0001'
                                                                THEN NULL
                                                                ELSE @But_CancelDate
                                                            END
                                        , But_Gender= @But_Gender
                                        , But_CivilStatus=@But_CivilStatus
                                        , But_Company=@But_Company
                                        , But_Occupation=@But_Occupation
                                        , But_Reason=@But_Reason
                                        , But_Remarks=@But_Remarks
	                                    , But_Status = @But_Status
	                                    , Usr_Login = @Usr_Login
	                                    , Ludatetime = GETDATE()
                                    WHERE But_ControlNo = @But_ControlNo ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateBFRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateBFRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateBFRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["But_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  But_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@But_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"But_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@But_CheckedBy", " ");
                }
                else if (rowDetails["But_Status"].ToString().Equals("5")
                    || rowDetails["But_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  But_CheckedBy = @But_CheckedBy ,
                                But_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@But_CheckedBy", rowDetails["But_CheckedBy"]);
                }
                else if (rowDetails["But_Status"].ToString().Equals("7")
                    || rowDetails["But_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  But_Checked2By = @But_Checked2By ,
                                But_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@But_Checked2By", rowDetails["But_Checked2By"]);
                }
                else if (rowDetails["But_Status"].ToString().Equals("9")
                    || rowDetails["But_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  But_ApprovedBy = @But_ApprovedBy ,
                                But_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@But_ApprovedBy", rowDetails["But_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@But_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@But_Status", rowDetails["But_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@But_ControlNo", rowDetails["But_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_BeneficiaryUpdate
                                    SET {0}
                                        But_Status = @But_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                  WHERE But_ControlNo = @But_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateEmployeeBeneficiaryMaster(string controlNo, string userLogged, DALHelper dal)
        {
            DataRow dr = getBeneficiaryInfo(controlNo);
            ParameterInfo[] param = new ParameterInfo[20];
            param[0] = new ParameterInfo("@EmployeeID", dr["ID No"].ToString());
            param[1] = new ParameterInfo("@Ebm_Lastname", dr["Beneficiary Lastname"].ToString());
            param[2] = new ParameterInfo("@Ebm_Firstname", dr["Beneficiary Firstname"].ToString());
            param[3] = new ParameterInfo("@Ebm_Middlename", dr["Beneficiary Middlename"].ToString());
            param[4] = new ParameterInfo("@Ebm_Birthdate", dr["Birthdate"].ToString());
            param[5] = new ParameterInfo("@Ebm_Relation", dr["Relationship Code"].ToString());
            param[6] = new ParameterInfo("@Ebm_Hierarchy", dr["Hierarchy Code"].ToString());
            param[7] = new ParameterInfo("@Ebm_BIRReport", dr["BIR"].ToString());
            param[8] = new ParameterInfo("@Ebm_HMODependent", dr["HMO"].ToString());
            param[9] = new ParameterInfo("@Ebm_InsuranceDependent", dr["Insurance"].ToString());
            param[10] = new ParameterInfo("@Ebm_AccidentDependent", dr["Accident"].ToString());
            if(dr["Deceased Date"] == null || dr["Deceased Date"].ToString() == string.Empty)
                param[11] = new ParameterInfo("@Ebm_DeceaseDate", DBNull.Value);
            else param[11] = new ParameterInfo("@Ebm_DeceaseDate", dr["Deceased Date"].ToString());
            if (dr["Cancelled Date"] == null || dr["Cancelled Date"].ToString() == string.Empty)
                param[12] = new ParameterInfo("@Ebm_CancelDate", DBNull.Value);
            else param[12] = new ParameterInfo("@Ebm_CancelDate", dr["Cancelled Date"].ToString());
            param[13] = new ParameterInfo("@Usr_Login", userLogged);
            param[14] = new ParameterInfo("@Ebm_BenSeqNo", dr["Seq No"].ToString());
            param[15] = new ParameterInfo("@Ebm_Remarks", dr["Reason"].ToString());
            param[16] = new ParameterInfo("@Ebm_CivilStatus", dr["Civil Status"].ToString());
            param[17] =  new ParameterInfo("@Ebm_Company", dr["Company"].ToString());
            param[18] =new ParameterInfo("@Ebm_Occupation", dr["Occupation"].ToString());
            param[19] = new ParameterInfo("@Ebm_Gender", dr["Gender"].ToString());
         
       
           
            //This is full query with AccidentInsurance and Remarks
            string sql = @" 
                            --Declare @Gender varchar(1)
					        --set @Gender = (select case when @Ebm_Relation in ('BRL','BRO','COE','COU','FND','FRL',
													--'FTR','GRF','GUA','HUS','NEP','REL','SNL','SON','UNC')
													--THEN 'M'
													--ELSE 'F'
													--END)
                        IF EXISTS (SELECT Ebm_EmployeeId
                                         FROM T_EMployeeBeneficiary
                                        WHERE Ebm_EmployeeId = @EmployeeId
                                          AND Ebm_BenSeqNo = @Ebm_BenSeqNo)
                   
                         BEGIN
                                UPDATE T_EmployeeBeneficiary
                                   SET Ebm_Lastname = @Ebm_Lastname
                                     , Ebm_Firstname = @Ebm_Firstname
                                     , Ebm_MiddleName = @Ebm_Middlename
                                     , Ebm_Birthdate = @Ebm_Birthdate
                                     , Ebm_Relation = @Ebm_Relation
                                     , Ebm_Hierarchy = @Ebm_Hierarchy
                                     , Ebm_BIRReport = @Ebm_BIRReport
                                     , Ebm_HMODep = @Ebm_HMODependent
                                     , Ebm_InsuranceDep = @Ebm_InsuranceDependent
                                     , Ebm_AccidentInsurance = @Ebm_AccidentDependent
                                     , Ebm_CivilStatus=  @Ebm_CivilStatus
                                     , Ebm_Occupation=@Ebm_Occupation
                                     , Ebm_Company=@Ebm_Company
                                     , Ebm_Remarks = @Ebm_Remarks
                                     , Ebm_DeceaseDate = @Ebm_DeceaseDate
                                     , Ebm_CancelDate = @Ebm_CancelDate
                                     , Usr_Login = @Usr_Login
                                     , Ebm_Gender=@Ebm_Gender                                    
                                     , Ludatetime = GETDATE()
                                 WHERE Ebm_EmployeeId = @EmployeeId
                                   AND Ebm_BenSeqNo = @Ebm_BenSeqNo
                           END
                          ELSE
                         BEGIN
                                INSERT INTO T_EmployeeBeneficiary
                                     ( Ebm_EmployeeID
                                     , Ebm_BenSeqNo
                                     , Ebm_LastName
                                     , Ebm_FirstName
                                     , Ebm_MiddleName
                                     , Ebm_Birthdate
                                     , Ebm_Relation
                                     , Ebm_BIRReport
                                     , Ebm_DeceaseDate
                                     , Ebm_CancelDate
                                     , Ebm_Hierarchy
                                     , Ebm_HMODep
                                     , Ebm_InsuranceDep
                                     , Ebm_AccidentInsurance
                                     , Ebm_Remarks
                                     , Ebm_Gender
                                     , Ebm_CivilStatus
                                     , Ebm_Age
                                     , Ebm_Occupation
                                     , Ebm_Company
                                    , Ebm_Status
                                     , Usr_Login
                                     , Ludatetime )
                                SELECT @EmployeeId
                                     , ISNULL(REPLICATE('0', 3 - LEN(MAX(Ebm_BenSeqNo)+1)) 
                                     + Convert(varchar(3),MAX(Ebm_BenSeqNo)+1), '001')
                                     , @Ebm_Lastname
                                     , @Ebm_Firstname
                                     , @Ebm_Middlename
                                     , @Ebm_Birthdate
                                     , @Ebm_Relation
                                     , @Ebm_BIRReport
                                     , @Ebm_DeceaseDate
                                     , @Ebm_CancelDate
                                     , @Ebm_Hierarchy
                                     , @Ebm_HMODependent
                                     , @Ebm_InsuranceDependent
                                     , @Ebm_AccidentDependent
                                     , @Ebm_Remarks
                                     , @Ebm_Gender
                                     , @Ebm_CivilStatus
                                     , DATEDIFF(YEAR,@ebm_Birthdate,GETDATE())
                                     , @Ebm_Occupation
                                     , @Ebm_Company
                                     , 'A'                                    
                                     , @Usr_Login
                                     , GETDATE()
                                  FROM T_EmployeeBeneficiary
                                 WHERE Ebm_EmployeeId =  @EmployeeId
                           END ";

            //TEMPORARY solution to merge code. Some database have Ebm_AccidentInsurance and Ebm_Remarks Column.
            #region TEMPORARY Solution
            string sqlTemp = @"IF EXISTS (SELECT Ebm_EmployeeId
                                            FROM T_EMployeeBeneficiary
                                           WHERE Ebm_EmployeeId = @EmployeeId
                                             AND Ebm_BenSeqNo = @Ebm_BenSeqNo)
                         BEGIN
                                UPDATE T_EmployeeBeneficiary
                                   SET Ebm_Lastname = @Ebm_Lastname
                                     , Ebm_Firstname = @Ebm_Firstname
                                     , Ebm_MiddleName = @Ebm_Middlename
                                     , Ebm_Birthdate = @Ebm_Birthdate
                                     , Ebm_Relation = @Ebm_Relation
                                     , Ebm_Hierarchy = @Ebm_Hierarchy
                                     , Ebm_BIRReport = @Ebm_BIRReport
                                     , Ebm_HMODep = @Ebm_HMODependent
                                     , Ebm_InsuranceDep = @Ebm_InsuranceDependent
                                     , Ebm_DeceaseDate = @Ebm_DeceaseDate
                                     , Ebm_CancelDate = @Ebm_CancelDate
                                     , Usr_Login = @Usr_Login
                                     , Ludatetime = GETDATE()
                                 WHERE Ebm_EmployeeId = @EmployeeId
                                   AND Ebm_BenSeqNo = @Ebm_BenSeqNo
                           END
                          ELSE
                         BEGIN
                                INSERT INTO T_EmployeeBeneficiary
                                     ( Ebm_EmployeeID
                                     , Ebm_BenSeqNo
                                     , Ebm_LastName
                                     , Ebm_FirstName
                                     , Ebm_MiddleName
                                     , Ebm_Birthdate
                                     , Ebm_Relation
                                     , Ebm_BIRReport
                                     , Ebm_DeceaseDate
                                     , Ebm_CancelDate
                                     , Ebm_Hierarchy
                                     , Ebm_HMODep
                                     , Ebm_InsuranceDep
                                     , Ebm_CivilStatus
                                     , Ebm_Age
                                     , Usr_Login
                                     , Ludatetime )
                                SELECT @EmployeeId
                                     , ISNULL(REPLICATE('0', 3 - LEN(MAX(Ebm_BenSeqNo)+1)) 
                                     + Convert(varchar(3),MAX(Ebm_BenSeqNo)+1), '001')
                                     , @Ebm_Lastname
                                     , @Ebm_Firstname
                                     , @Ebm_Middlename
                                     , @Ebm_Birthdate
                                     , @Ebm_Relation
                                     , @Ebm_BIRReport
                                     , @Ebm_DeceaseDate
                                     , @Ebm_CancelDate
                                     , @Ebm_Hierarchy
                                     , @Ebm_HMODependent
                                     , @Ebm_InsuranceDependent
                                     , ''
                                     , ''
                                     , @Usr_Login
                                     , GETDATE()
                                  FROM T_EmployeeBeneficiary
                                 WHERE Ebm_EmployeeId =  @EmployeeId
                           END ";
            #endregion
            try
            {
                dal.ExecuteNonQuery(sql, CommandType.Text, param);
            }
            catch(SqlException ex)
            { 
                if(ex.Message.Contains("does not match table definition"))
                {
                    dal.ExecuteNonQuery(sqlTemp, CommandType.Text, param);
                }
                else
                {
                    CommonMethods.ErrorsToTextFile(new Exception(ex.InnerException.ToString()), "UpdateEmployeeBeneficiaryMaster"); 
                }
            }
        }

        public bool hasExistingBeneficiary(string employeeId)
        {
            DataSet ds = new DataSet();
            string sql = string.Format(@" SELECT * 
                                            FROM T_EmployeeBeneficiary 
                                           WHERE Ebm_EmployeeId = '{0}' 
                                             AND Ebm_EmployeeId+Ebm_BenSeqNo NOT IN (SELECT But_EmployeeId  COLLATE SQL_Latin1_General_CP1_CI_AS + But_SeqNo COLLATE SQL_Latin1_General_CP1_CI_AS
                                                                                       FROM T_BeneficiaryUpdate
                                                                                      WHERE But_Status IN ('1','3','5','7','9')
                                                                                        AND But_Type = 'U'
                                                                                        AND But_EmployeeId = '{0}')" , employeeId);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch(Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return !CommonMethods.isEmpty(ds);
        }
        #endregion

        #region Address Updating
        public DataRow getAddressInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Amt_ControlNo [Control No]
                                  , Amt_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Convert(varchar(10), Amt_EffectivityDate, 101) [Effectivity Date]
                                  , Amt_Type [Type]
                                  , Amt_Address1 [Address1]
                                  , Amt_Address2 [Address2 Code]
                                  , ADAddress2.Adt_AccountDesc [Address2 Desc]
                                  , Amt_Address3 [Address3 Code]
                                  , ADAddress3.Adt_AccountDesc [Address3 Desc]
                                  , Amt_TelephoneNo [TelephoneNo]
                                  , Amt_CellularNo [CellularNo]
                                  , Amt_EmailAddress [EmailAddress]
                                  , Amt_Filler1 [Transportation]
                                  , Rte_RouteName [RouteName]
                                  , Amt_Filler2 [Amount]
                                  , Amt_Filler3
                                  , Amt_ContactPerson [ContactPerson]
                                  , Amt_ContactRelation [ContactRelation]
                                  , ADRelation.Adt_AccountDesc [ContactRelationDesc] 
                                  , Amt_Reason [Reason]
                                  , AD1.Adt_AccountDesc [Status]
                                  , Trm_Remarks [Remarks]
                                  , Amt.Usr_Login [Usr_Login]
                                  , Rte_LedgerAlwCol [LedgerAlwCol]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                               FROM T_AddressMovement Amt
                               LEFT JOIN T_EmployeeMaster
                                 ON Emt_EmployeeID =  Amt_EmployeeId
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Amt_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail ADAddress2 
                                 ON ADAddress2.Adt_AccountCode = Amt_Address2 
                                AND ADAddress2.Adt_AccountType =  'BARANGAY'
                               LEFT JOIN T_AccountDetail ADAddress3 
                                 ON ADAddress3.Adt_AccountCode = Amt_Address3 
                                AND ADAddress3.Adt_AccountType =  'ZIPCODE'
                               LEFT JOIN T_RouteMaster 
                                 ON Amt_Filler1 = Rte_RouteCode
                               LEFT JOIN T_TransactionRemarks 
                                 ON Trm_ControlNo = Amt_ControlNo
                               LEFT JOIN T_AccountDetail ADRelation
                                 ON ADRelation.Adt_AccountCode = Amt_ContactRelation
                                AND ADRelation.Adt_AccountType = 'RELATION'
                              WHERE Amt_ControlNo = @ControlNo";
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

        public void CreateADRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[19];
            paramDetails[0] = new ParameterInfo("@Amt_ControlNo", rowDetails["Amt_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Amt_EmployeeId", rowDetails["Amt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Amt_EffectivityDate", rowDetails["Amt_EffectivityDate"]);
            paramDetails[3] = new ParameterInfo("@Amt_Type", rowDetails["Amt_Type"]);
            paramDetails[4] = new ParameterInfo("@Amt_Address1", rowDetails["Amt_Address1"]);
            paramDetails[5] = new ParameterInfo("@Amt_Address2", rowDetails["Amt_Address2"]);
            paramDetails[6] = new ParameterInfo("@Amt_Address3", rowDetails["Amt_Address3"]);
            paramDetails[7] = new ParameterInfo("@Amt_Reason", rowDetails["Amt_Reason"]);
            paramDetails[8] = new ParameterInfo("@Amt_Filler1", rowDetails["Amt_Filler1"]);
            paramDetails[9] = new ParameterInfo("@Amt_Filler2", rowDetails["Amt_Filler2"]);
            paramDetails[10] = new ParameterInfo("@Amt_Filler3", rowDetails["Amt_Filler3"]);
            paramDetails[11] = new ParameterInfo("@Amt_EndorsedDateToChecker", rowDetails["Amt_EndorsedDateToChecker"]);
            paramDetails[12] = new ParameterInfo("@Amt_Status", rowDetails["Amt_Status"]);
            paramDetails[13] = new ParameterInfo("@Amt_TelephoneNo", rowDetails["Amt_TelephoneNo"]);
            paramDetails[14] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[15] = new ParameterInfo("@Amt_CellularNo", rowDetails["Amt_CellularNo"]);
            paramDetails[16] = new ParameterInfo("@Amt_EmailAddress", rowDetails["Amt_EmailAddress"]);
            paramDetails[17] = new ParameterInfo("@Amt_ContactPerson", rowDetails["Amt_ContactPerson"]);
            paramDetails[18] = new ParameterInfo("@Amt_ContactRelation", rowDetails["Amt_ContactRelation"]);

            #endregion

            #region SQL Query
            string sqlInsert = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                                                     FROM T_EmployeeCostcenterMovement
                                                    WHERE Ecm_EmployeeID = @Amt_EmployeeId
                                                      AND @Amt_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                      --AND Ecm_Status = 'A' 
                                                    ORDER BY Ludatetime DESC)

                                IF(@Costcenter IS NULL)
                                BEGIN
                                    SET @Costcenter = (SELECT Emt_CostcenterCode 
                                                         FROM T_EmployeeMaster
				                                        WHERE Emt_EmployeeID = @Amt_EmployeeId)
                                END

                                --apsungahid added 20141124
                                DECLARE @LineCode as varchar(15)
                                SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                                                   FROM E_EmployeeCostCenterLineMovement
                                                  WHERE Ecm_EmployeeID = @Amt_EmployeeId
                                                    AND @Amt_EffectivityDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                                                    AND Ecm_Status = 'A' 
                                                  ORDER BY Ludatetime DESC)
                                IF(@LineCode IS NULL)
                                BEGIN
                                    SET @LineCode = ''
                                END

                                   INSERT INTO T_AddressMovement
	                                    ( Amt_ControlNo
	                                    , Amt_EmployeeId
	                                    , Amt_EffectivityDate
	                                    , Amt_AppliedDate
	                                    , Amt_Costcenter
	                                    , Amt_CostcenterLine
	                                    , Amt_Type
	                                    , Amt_Address1
	                                    , Amt_Address2
	                                    , Amt_Address3
	                                    , Amt_Reason
	                                    , Amt_Filler1
	                                    , Amt_Filler2
	                                    , Amt_Filler3
	                                    , Amt_Status
	                                    , Amt_TelephoneNo
	                                    , Amt_CellularNo
	                                    , Amt_EmailAddress
	                                    , Amt_ContactPerson
	                                    , Amt_ContactRelation
	                                    , Usr_Login
	                                    , Ludatetime)
                                  VALUES( @Amt_ControlNo
	                                    , @Amt_EmployeeId
	                                    , @Amt_EffectivityDate
	                                    , GETDATE()
	                                    , @Costcenter
	                                    , @LineCode
	                                    , @Amt_Type
	                                    , @Amt_Address1
	                                    , @Amt_Address2
	                                    , @Amt_Address3
	                                    , @Amt_Reason
	                                    , @Amt_Filler1
	                                    , @Amt_Filler2
	                                    , @Amt_Filler3
	                                    , @Amt_Status
	                                    , @Amt_TelephoneNo
	                                    , @Amt_CellularNo
	                                    , @Amt_EmailAddress
	                                    , @Amt_ContactPerson
	                                    , @Amt_ContactRelation
	                                    , @Usr_Login
	                                    , GETDATE())";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateADRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[18];
            paramDetails[0] = new ParameterInfo("@Amt_ControlNo", rowDetails["Amt_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Amt_EmployeeId", rowDetails["Amt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Amt_EffectivityDate", rowDetails["Amt_EffectivityDate"]);
            paramDetails[3] = new ParameterInfo("@Amt_Type", rowDetails["Amt_Type"]);
            paramDetails[4] = new ParameterInfo("@Amt_Address1", rowDetails["Amt_Address1"]);
            paramDetails[5] = new ParameterInfo("@Amt_Address2", rowDetails["Amt_Address2"]);
            paramDetails[6] = new ParameterInfo("@Amt_Address3", rowDetails["Amt_Address3"]);
            paramDetails[7] = new ParameterInfo("@Amt_TelephoneNo", rowDetails["Amt_TelephoneNo"]);
            paramDetails[8] = new ParameterInfo("@Amt_CellularNo", rowDetails["Amt_CellularNo"]);
            paramDetails[9] = new ParameterInfo("@Amt_EmailAddress", rowDetails["Amt_EmailAddress"]);
            paramDetails[10] = new ParameterInfo("@Amt_Filler1", rowDetails["Amt_Filler1"]);
            paramDetails[11] = new ParameterInfo("@Amt_Filler2", rowDetails["Amt_Filler2"]);
            paramDetails[12] = new ParameterInfo("@Amt_Filler3", rowDetails["Amt_Filler3"]);
            paramDetails[13] = new ParameterInfo("@Amt_ContactPerson", rowDetails["Amt_ContactPerson"]);
            paramDetails[14] = new ParameterInfo("@Amt_ContactRelation", rowDetails["Amt_ContactRelation"]);
            paramDetails[15] = new ParameterInfo("@Amt_Reason", rowDetails["Amt_Reason"]);
            paramDetails[16] = new ParameterInfo("@Amt_Status", rowDetails["Amt_Status"]);
            paramDetails[17] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            
            #endregion

            #region SQL Query
            string sqlInsert = @"
                                UPDATE T_AddressMovement
                                   SET Amt_EmployeeId = @Amt_EmployeeId
                                     , Amt_EffectivityDate = @Amt_EffectivityDate
                                     , Amt_Address1 = @Amt_Address1
                                     , Amt_Address2 = @Amt_Address2
                                     , Amt_Address3 = @Amt_Address3
                                     , Amt_TelephoneNo = @Amt_TelephoneNo
                                     , Amt_CellularNo = @Amt_CellularNo
                                     , Amt_EmailAddress = @Amt_EmailAddress
                                     , Amt_Filler1 = @Amt_Filler1
                                     , Amt_Filler2 = @Amt_Filler2
                                     , Amt_Filler3 = @Amt_Filler3
                                     , Amt_ContactPerson = @Amt_ContactPerson
                                     , Amt_ContactRelation = @Amt_ContactRelation
                                     , Amt_Status = @Amt_Status
                                     , Amt_Reason = @Amt_Reason                                     
                                     , Usr_Login = @Usr_Login
                                     , Ludatetime = GETDATE()
                                 WHERE Amt_ControlNo = @Amt_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateADRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateADRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateADRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Amt_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Amt_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Amt_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Amt_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Amt_CheckedBy", " ");
                }
                else if (rowDetails["Amt_Status"].ToString().Equals("5")
                    || rowDetails["Amt_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Amt_CheckedBy = @Amt_CheckedBy ,
                                Amt_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Amt_CheckedBy", rowDetails["Amt_CheckedBy"]);
                }
                else if (rowDetails["Amt_Status"].ToString().Equals("7")
                    || rowDetails["Amt_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Amt_Checked2By = @Amt_Checked2By ,
                                Amt_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Amt_Checked2By", rowDetails["Amt_Checked2By"]);
                }
                else if (rowDetails["Amt_Status"].ToString().Equals("9")
                    || rowDetails["Amt_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Amt_ApprovedBy = @Amt_ApprovedBy ,
                                Amt_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Amt_ApprovedBy", rowDetails["Amt_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Amt_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Amt_Status", rowDetails["Amt_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Amt_ControlNo", rowDetails["Amt_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_AddressMovement
                                    SET {0}
                                        Amt_Status = @Amt_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Amt_ControlNo = @Amt_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateEmployeeMasterAddress(string controlNo, string userLogged, DALHelper dal)  //temp wa pa nahuman
        {
            DataRow dr = getAddressInfo(controlNo);

            #region SQL
            #region Present
            string sqlUpdatePresent = @"DECLARE @Effectivity as datetime
                                            SET @Effectivity = (SELECT TOP 1 Ert_EffectivityDate
                                                                  FROM T_EmployeeRoute
                                                                 WHERE Ert_EmployeeId = @Amt_EmployeeId
                                                                   AND Convert(varchar(10),Ert_EffectivityDate,101) = @Amt_EffectivityDate)
											IF('TRUE' = '{0}')
											BEGIN
                                            IF EXISTS (SELECT Ert_EmployeeID
                                                         FROM T_EmployeeRoute
                                                        WHERE Ert_EmployeeID = @Amt_EmployeeId
                                                          AND Ert_LedgerAlwCol = @Ert_LedgerAlwCol
                                                          AND Ert_EffectivityDate = @Amt_EffectivityDate)
                                         BEGIN
                                            UPDATE T_EmployeeRoute
                                               SET Ert_RouteCode = @Amt_Filler1
                                                 , Usr_Login = @Usr_Login
                                                 , Ludatetime = GETDATE()
                                             WHERE Ert_EmployeeID = @Amt_EmployeeId
                                               AND Ert_LedgerAlwCol = @Ert_LedgerAlwCol
                                               AND Ert_EffectivityDate = @Amt_EffectivityDate
                                           END
                                          ELSE
                                         BEGIN
                                            INSERT INTO T_EmployeeRoute
                                                 ( [Ert_LedgerAlwCol]
                                                 , [Ert_EmployeeID]
                                                 , [Ert_EffectivityDate]
                                                 , [Ert_RouteCode]
                                                 , [Usr_Login]
                                                 , [Ludatetime] )
                                            VALUES
                                                 ( @Ert_LedgerAlwCol
                                                 , @Amt_EmployeeId
                                                 , @Amt_EffectivityDate
                                                 , @Amt_Filler1
                                                 , @Usr_Login
                                                 , GETDATE() )
                                           END
										   END

                                         UPDATE T_EmployeeRoute
                                            SET Ert_EndDate = dateadd(day, -1 , @Amt_EffectivityDate)
                                              , Usr_Login = @Usr_Login
                                              , Ludatetime = getdate()
                                          WHERE Ert_EmployeeId = @Amt_EmployeeId 
                                            AND Ert_EffectivityDate = (SELECT TOP 1 Ert_EffectivityDate
	                                                                     FROM T_EmployeeRoute
	                                                                    WHERE Ert_EmployeeId = @Amt_EmployeeId
	                                                                      AND Ert_EffectivityDate < @Amt_EffectivityDate 
                                                                        ORDER BY Ert_EffectivityDate DESC )


                                          UPDATE T_EmployeeMaster
                                             SET Emt_EmployeeAddress1 = @Amt_Address1
                                               , Emt_EmployeeAddress2 = @Amt_Address2
                                               , Emt_EmployeeAddress3 = @Amt_Address3
                                               , Emt_TelephoneNo = @Amt_TelephoneNo
                                               , Emt_CellularNo = @Amt_CellularNo
                                               , Emt_EmailAddress = @Amt_EmailAddress
                                               , Usr_Login = @Usr_Login
                                               , Ludatetime = getdate()
                                            WHERE Emt_EmployeeId = @Amt_EmployeeId";
            #endregion
            #region Permanent
            string sqlUpdatePermanent = @"UPDATE T_EmployeeMaster
                                             SET Emt_EmployeeProvAddress1 = @Amt_Address1
                                               , Emt_EmployeeProvAddress2 = @Amt_Address2
                                               , Emt_EmployeeProvAddress3 = @Amt_Address3
                                               , Emt_EmployeeProvTelephoneNo = @Amt_TelephoneNo
                                               , Usr_Login = @Usr_Login
                                               , Ludatetime = getdate()
                                            WHERE Emt_EmployeeId = @Amt_EmployeeId";
            #endregion
            #region In case of emergency
            string sqlUpdateEmergency = @"UPDATE T_EmployeeMaster
                                                     SET Emt_ContactPersonName = @Amt_ContactPerson
                                                       , Emt_ContactRelation = @Amt_ContactRelation
                                                       , Emt_ContactAddress1 = @Amt_Address1
                                                       , Emt_ContactAddress2 = @Amt_Address2
                                                       , Emt_ContactAddress3 = @Amt_Address3
                                                       , Emt_ContactPersonNo = @Amt_TelephoneNo
                                                       , Usr_Login = @Usr_Login
                                                       , Ludatetime = getdate()
                                            WHERE Emt_EmployeeId = @Amt_EmployeeId";
            #endregion
            #endregion

            ParameterInfo[] param = new ParameterInfo[16];
            param[0] = new ParameterInfo("@Amt_EmployeeId", dr["ID No"].ToString());
            param[1] = new ParameterInfo("@Amt_EffectivityDate", dr["Effectivity Date"].ToString());
            param[2] = new ParameterInfo("@Amt_Type", dr["Type"].ToString());
            param[3] = new ParameterInfo("@Amt_Address1", dr["Address1"].ToString());
            param[4] = new ParameterInfo("@Amt_Address2", dr["Address2 Code"].ToString());
            param[5] = new ParameterInfo("@Amt_Address3", dr["Address3 Code"].ToString());
            param[6] = new ParameterInfo("@Amt_TelephoneNo", dr["TelephoneNo"].ToString());
            param[7] = new ParameterInfo("@Amt_CellularNo", dr["CellularNo"].ToString());
            param[8] = new ParameterInfo("@Amt_EmailAddress", dr["EmailAddress"].ToString());
            param[9] = new ParameterInfo("@Amt_Filler1", dr["Transportation"].ToString());
            param[10] = new ParameterInfo("@Amt_Filler2", dr["Amount"].ToString());
            param[11] = new ParameterInfo("@Amt_Filler3", dr["Amt_Filler3"].ToString());
            param[12] = new ParameterInfo("@Amt_ContactPerson", dr["ContactPerson"].ToString());
            param[13] = new ParameterInfo("@Amt_ContactRelation", dr["ContactRelation"].ToString());
            param[14] = new ParameterInfo("@Usr_Login", userLogged);
            param[15] = new ParameterInfo("@Ert_LedgerAlwCol", dr["LedgerAlwCol"]);
            

            switch (dr["Type"].ToString().ToUpper())
            {
                case "A1"://Present Address
                    dal.ExecuteNonQuery(string.Format(sqlUpdatePresent, Resources.Resource.CHIYODASPECIFIC.ToUpper()), CommandType.Text, param);
                    break;
                case "A2"://Permanent Address
                    dal.ExecuteNonQuery(sqlUpdatePermanent, CommandType.Text, param);
                    break;
                case "A3"://In Case of Emergency
                    dal.ExecuteNonQuery(sqlUpdateEmergency, CommandType.Text, param);
                    break;
                default:
                    break;
            }

        }

        #endregion
    }
}
