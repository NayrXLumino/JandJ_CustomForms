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
/// Summary description for PayrollBL
/// </summary>
namespace Payroll.DAL
{
    public class PayrollBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public PayrollBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataRow getTaxCivilInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Pit_ControlNo [Control No]
                                  , Pit_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
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
                                    SET @Costcenter = (SELECT Emt_CostcenterCode FROM T_EmployeeMaster
				                                   WHERE Emt_EmployeeID = @Pit_EmployeeId)

                                   INSERT INTO T_PersonnelInfoMovement
	                                    ( Pit_ControlNo
	                                    , Pit_CurrentPayPeriod
	                                    , Pit_EmployeeId
	                                    , Pit_EffectivityDate
	                                    , Pit_AppliedDate
	                                    , Pit_Costcenter
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

        public void UpdateEmployeeMasterTaxCivil(string controlNo, string userLogged, DALHelper dal)
        {
            DataRow dr = getTaxCivilInfo(controlNo);
            if (!dr["To Tax Code"].ToString().Equals(string.Empty) && !dr["To Tax Code"].ToString().Equals(dr["From Tax Code"].ToString()))
            {
                UpdateTaxCode( dr["ID No"].ToString()
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
    }
}
