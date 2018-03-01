/*
 * Updated By       : 1090 - Marx
 * Updated Date     : 04/17/2013
 * Update Notes     : Added condition in UpdateUserPassword if UserMaster not in Profile 
 *                    1. Umt_EffectivityDate is not included in query.
 *                    2. Validity of Columns for user_login, Umt_Userpswd. UserMaster in Profile table is not thesame as UserMaster in NonConfi
 */
using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using CommonLibrary;

/// <summary>
/// Summary description for GeneralBL
/// </summary>
namespace Payroll.DAL
{
    public class GeneralBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public GeneralBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //Approval Route Methods
        public string GetRouteID(string C1, string C2, string AP)
        {
            string sql = @" SELECT Arm_RouteId
                              FROM T_ApprovalRouteMaster
                             WHERE Arm_Checker1 = @Checker1
                               AND Arm_Checker2 = @Checker2
                               AND Arm_Approver = @Approver";
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@Checker1", C1);
            param[1] = new ParameterInfo("@Checker2", C2);
            param[2] = new ParameterInfo("@Approver", AP);
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch
                {

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

        public int countRouteUsage(string routeId)
        {
            string sql = @"SELECT COUNT(Arm_RouteID) 
                             FROM T_EmployeeApprovalRoute
                            INNER JOIN T_TransactionControlMaster
                               ON Tcm_TransactionCode = Arm_TransactionID
                              AND Tcm_Status = 'A'
                            WHERE Arm_Status = 'A'
                              AND Arm_RouteID = @RouteId
                              AND Convert(varchar,GETDATE(),101) BETWEEN Arm_StartDate AND ISNULL(Arm_EndDate, GETDATE())";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@RouteId", routeId);
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (!CommonMethods.isEmpty(ds))
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public DataSet GetEmployeeRouteAssignment(string employeeId)
        {
            string sql = @" SELECT T_EmployeeApprovalRoute.Arm_TransactionID [Transaction Code]
	                             , T_EmployeeApprovalRoute.Arm_RouteID [Route ID]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
                                 , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
                                -- , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_StartDate,101) [Start Date]
                                -- , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_EndDate,101) [End Date]
                              FROM T_EmployeeApprovalRoute
                             INNER JOIN T_ApprovalRouteMaster
                                ON T_ApprovalRouteMaster.Arm_RouteID = T_EmployeeApprovalRoute.Arm_RouteID
                             WHERE Arm_EmployeeId = @EmployeeID
                             AND Convert(varchar,GETDATE(),101) BETWEEN T_EmployeeApprovalRoute.Arm_StartDate AND ISNULL(T_EmployeeApprovalRoute.Arm_EndDate, GETDATE())"; 
            
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@EmployeeID", employeeId);
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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
            return ds;
        }
        public DataSet GetEmployeeRouteAssignment(string employeeId, string transactionType)
        {
            string sql = @" SELECT T_EmployeeApprovalRoute.Arm_TransactionID [Transaction Code]
	                             , T_EmployeeApprovalRoute.Arm_RouteID [Route ID]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
                                 , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
                                 , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
                                 , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_StartDate,101) [Start Date]
                                 , CONVERT(varchar,T_EmployeeApprovalRoute.Arm_EndDate,101) [End Date]
                              FROM T_EmployeeApprovalRoute
                             INNER JOIN T_ApprovalRouteMaster
                                ON T_ApprovalRouteMaster.Arm_RouteID = T_EmployeeApprovalRoute.Arm_RouteID
                             WHERE Arm_EmployeeId = @EmployeeID
                            AND Arm_TransactionID = @TransactionID
                            ORDER BY Arm_StartDate DESC";

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@EmployeeID", employeeId);
            param[1] = new ParameterInfo("@TransactionID", transactionType);
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
            return ds;
        }
        public string GetRouteControlNumber()
        {
            string sql = @" SELECT Tcm_TransactionPrefix 
	                                            + replicate('0', 6 - len(RTrim(Tcm_LastSeries)))
	                                            + RTrim(Tcm_LastSeries)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
                                            WHERE Tcm_TransactionCode = 'APRROUTE'

                                            UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = 'APRROUTE'";
            string controlid = "";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    controlid=dal.ExecuteScalar(sql, CommandType.Text).ToString();
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Control Number not created.");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return controlid;
        }
        public void CreateApprovalRoute(DataRow dr, bool isForJobMod)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[7];
            paramDetails[0] = new ParameterInfo("@Arm_Checker1", dr["Arm_Checker1"]);
            paramDetails[1] = new ParameterInfo("@Arm_Checker2", dr["Arm_Checker2"]);
            paramDetails[2] = new ParameterInfo("@Arm_Approver", dr["Arm_Approver"]);
            paramDetails[3] = new ParameterInfo("@Arm_CostcenterCode", dr["Arm_CostcenterCode"]);
            paramDetails[4] = new ParameterInfo("@Arm_Status", dr["Arm_Status"]);
            paramDetails[5] = new ParameterInfo("@Usr_Login", dr["Usr_Login"]);
            paramDetails[6] = new ParameterInfo("@Arm_RouteId", dr["Arm_RouteId"]);
            #endregion

            #region SQL
            string sql = @"--declare @Arm_RouteId as char(4)
                               --SET @Arm_RouteId = (  SELECT 'R'
                               --                                  + REPLICATE('0', 3 -COALESCE(LEN(RIGHT(MAX(Arm_RouteId),3) + 1),1)) 
                               --                                  + Convert(varchar(3), (COALESCE(RIGHT(MAX(LTRIM(RTRIM(Arm_RouteId))),3),0) + 1))
                               --                         FROM T_ApprovalRouteMaster)

                           

                            INSERT INTO T_ApprovalRouteMaster
                                 ( Arm_RouteId
                                 , Arm_Checker1
                                 , Arm_Checker2
                                 , Arm_Approver
                                 , Arm_CostcenterCode
                                 , Arm_Status
                                 , Usr_Login
                                 , Ludatetime)
                          VALUES ( @Arm_RouteId
                                 , @Arm_Checker1
                                 , @Arm_Checker2
                                 , @Arm_Approver
                                 , @Arm_CostcenterCode
                                 , @Arm_Status
                                 , @Usr_Login
                                 , GETDATE() )
                            ";
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC) || Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC) || Convert.ToBoolean(Resources.Resource.LEARSPECIFIC))
            {
                if (isForJobMod || Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
                {
                    sql = @"declare @RouteID as char(7)
                              SET @RouteID = ( SELECT 'R0'
                                                        + REPLICATE('0', 5 - LEN(RIGHT(MAX(Arm_RouteId),5) + 1)) 
                                                        + Convert(varchar(3),(RIGHT(MAX(Arm_RouteId),5) + 1))
                                                    FROM T_ApprovalRouteMaster
                                                   WHERE Arm_RouteID LIKE 'R0%')

                           INSERT INTO T_ApprovalRouteMaster
                                ( Arm_RouteId
                                , Arm_Checker1
                                , Arm_Checker2
                                , Arm_Approver
                                , Arm_CostcenterCode
                                , Arm_Status
                                , Usr_Login
                                , Ludatetime)
                         VALUES ( @RouteID
                                , @Arm_Checker1
                                , @Arm_Checker2
                                , @Arm_Approver
                                , @Arm_CostcenterCode
                                , @Arm_Status
                                , @Usr_Login
                                , GETDATE() )
                        ";
                }
                else
                {
                    sql = @"declare @RouteID as char(7)
                              SET @RouteID = ( SELECT 'R1'
                                                        + REPLICATE('0', 5 - LEN(RIGHT(MAX(Arm_RouteId),5) + 1)) 
                                                        + Convert(varchar(3),(RIGHT(MAX(Arm_RouteId),5) + 1))
                                                    FROM T_ApprovalRouteMaster
                                                   WHERE Arm_RouteID LIKE 'R1%')

                           INSERT INTO T_ApprovalRouteMaster
                                ( Arm_RouteId
                                , Arm_Checker1
                                , Arm_Checker2
                                , Arm_Approver
                                , Arm_CostcenterCode
                                , Arm_Status
                                , Usr_Login
                                , Ludatetime)
                         VALUES ( @RouteID
                                , @Arm_Checker1
                                , @Arm_Checker2
                                , @Arm_Approver
                                , @Arm_CostcenterCode
                                , @Arm_Status
                                , @Usr_Login
                                , GETDATE() )
                        ";
                }

            }
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.ExecuteNonQuery(sql, CommandType.Text, paramDetails);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, dr["Usr_Login"].ToString());
                    throw;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void UpdateApprovalRoute(DataRow dr)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[7];
            paramDetails[0] = new ParameterInfo("@Arm_RouteId", dr["Arm_RouteId"]);
            paramDetails[1] = new ParameterInfo("@Arm_Checker1", dr["Arm_Checker1"]);
            paramDetails[2] = new ParameterInfo("@Arm_Checker2", dr["Arm_Checker2"]);
            paramDetails[3] = new ParameterInfo("@Arm_Approver", dr["Arm_Approver"]);
            paramDetails[4] = new ParameterInfo("@Arm_CostcenterCode", dr["Arm_CostcenterCode"]);
            paramDetails[5] = new ParameterInfo("@Arm_Status", dr["Arm_Status"]);
            paramDetails[6] = new ParameterInfo("@Usr_Login", dr["Usr_Login"]);
            #endregion

            #region SQL
            string sql = @" UPDATE T_ApprovalRouteMaster
                               SET Arm_RouteId = @Arm_RouteId
                                 , Arm_Checker1 = @Arm_Checker1
                                 , Arm_Checker2 = @Arm_Checker2
                                 , Arm_Approver = @Arm_Approver
                                 , Arm_CostcenterCode = @Arm_CostcenterCode
                                 , Arm_Status = @Arm_Status
                                 , Usr_Login = @Usr_Login
                                 , Ludatetime = GETDATE()
                             WHERE Arm_RouteId = @Arm_RouteId
                            ";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.ExecuteNonQuery(sql, CommandType.Text, paramDetails);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void InsertEmployeeRoute(string employeeId
            , string routeId
            , string transactionId
            , string userLogin
            , string startDate
            , string endDate
            , bool endorse
            , bool returned
            , bool approve
            , bool disapprove
            , DALHelper dal)
        {
            string sql = string.Format(@"INSERT INTO T_EmployeeApprovalRoute
                                             ( Arm_EmployeeId
                                             , Arm_TransactionID
                                             , Arm_RouteID
                                             , Arm_NotifyEndorse
                                             , Arm_NotifyApprove
                                             , Arm_NotifyReturn
                                             , Arm_NotifyDisapprove
                                             , Arm_Status
                                             , Usr_login
                                             , Ludatetime
                                            ,Arm_StartDate
                                            ,Arm_EndDate)
                                       VALUES( '{0}'
                                             , '{1}' 
                                             , '{2}'
                                             , '{6}'
                                             , '{7}'
                                             , '{8}'
                                             , '{9}'
                                             , 'A' 
                                             , '{3}'
                                             , GETDATE()
                                             , '{4}'
                                             , Case when '{5}' = '' THEN NULL else '{5}' end)", employeeId
                                                                                              , transactionId
                                                                                              , routeId
                                                                                              , userLogin
                                                                                              , startDate
                                                                                              , endDate
                                                                                              , endorse
                                                                                              , approve
                                                                                              , returned
                                                                                              , disapprove);
           int retVal = dal.ExecuteNonQuery(sql, CommandType.Text);
           if (retVal > 0)
               InsertEmployeeRouteTrail(employeeId, routeId, transactionId, userLogin, startDate, dal);
        }
        public string GetLastSeqNoInTrail(string employeeID, string transactionID, DALHelper dal)
        {

            int x = 0;
            string LastSeqNo = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[2];

            paramInfo[0] = new ParameterInfo("@Arm_EmployeeId", employeeID);
            paramInfo[1] = new ParameterInfo("@Arm_TransactionId", transactionID);

            string sqlQuery = string.Format(@"Select COUNT(Arm_SeqNo) AS [Seq No]
                                            FROM T_EmployeeApprovalRouteTrail
                                            WHERE Arm_EmployeeID = '{0}'
                                            AND Arm_TransactionID = '{1}'"
                                                            , employeeID
                                                            , transactionID);

                x = Convert.ToInt32(dal.ExecuteScalar(sqlQuery, CommandType.Text, paramInfo));
             

            x++;
           
            if (x < 100)
                LastSeqNo = x.ToString().PadLeft(3, '0');
            else if (x > 999)
                 LastSeqNo = "999";
            else
                LastSeqNo = x.ToString();

            return LastSeqNo;
        }
        public int UpdateEmployeeRoute(string employeeId
                                        , string routeId
                                        , string transactionId
                                        , string userLogin
                                        , string startDate
                                        , string endDate
                                        , bool endorse
                                        , bool returned
                                        , bool approve
                                        , bool disapprove
                                        , DALHelper dal)
        {
            DataTable dtResult = FetchEmployeeApprovalRoute(employeeId, transactionId, startDate, dal);
            bool isModified = false;
            int retVal = 0;

            if (dtResult.Rows.Count > 0)
            {
                if (!startDate.Equals(dtResult.Rows[0]["Arm_StartDate"].ToString()))
                    isModified = true;
                if (!endDate.Equals(dtResult.Rows[0]["Arm_EndDate"].ToString()))
                    isModified = true;
                if (!routeId.Equals(dtResult.Rows[0]["Arm_RouteID"].ToString()))
                    isModified = true;
                if (!endorse.Equals(Convert.ToBoolean(dtResult.Rows[0]["Endorse"].ToString())))
                    isModified = true;
                if (!returned.Equals(Convert.ToBoolean(dtResult.Rows[0]["Return"].ToString())))
                    isModified = true;
                if (!approve.Equals(Convert.ToBoolean(dtResult.Rows[0]["Approve"].ToString())))
                    isModified = true;
                if (!disapprove.Equals(Convert.ToBoolean(dtResult.Rows[0]["Disapprove"].ToString())))
                    isModified = true;
            }
            string sql = string.Format(@"UPDATE T_EmployeeApprovalRoute
                                         SET Arm_RouteID = '{2}'
                                              , Arm_Status = 'A'
                                              , Usr_login = '{3}'
                                              , Ludatetime = GETDATE()
                                              , Arm_EndDate = Case when '{5}' = '' THEN NULL else '{5}' end
                                              , Arm_NotifyEndorse = '{6}'
                                              , Arm_NotifyReturn = '{7}'
                                              , Arm_NotifyApprove = '{8}'
                                              , Arm_NotifyDisapprove = '{9}'
                                          WHERE Arm_TransactionId = '{1}' 
                                            AND Arm_EmployeeId = '{0}'
                                            AND Arm_StartDate = '{4}'
                                            ", employeeId, transactionId, routeId, userLogin, startDate, endDate, endorse, returned, approve, disapprove);

            if (isModified)
            {
                retVal = dal.ExecuteNonQuery(sql, CommandType.Text);
                if(retVal > 0)
                InsertEmployeeRouteTrail(employeeId, routeId, transactionId, userLogin, startDate, dal);
            }
            return retVal;
        }

        private DataTable FetchEmployeeApprovalRoute(string employeeID
                                                            , string transactionID
                                                            , string startDate
                                                            , DALHelper dal)
        {
            DataTable dtResult = new DataTable();

            string query = string.Format(@"SELECT TOP 1 CONVERT(varchar, Arm_StartDate,101) [Arm_StartDate]
                                            , CONVERT(varchar,Arm_EndDate,101) [Arm_EndDate]
                                            , Arm_RouteID
                                            , Arm_NotifyEndorse [Endorse]
                                            , Arm_NotifyReturn [Return]
                                            , Arm_NotifyDisapprove [Disapprove]
                                            , Arm_NotifyApprove [Approve]
                                            FROM T_EmployeeApprovalRoute
                                            WHERE Arm_EmployeeID = '{0}'
                                            AND Arm_TransactionId = '{1}'
                                            AND Convert(varchar, Arm_StartDate, 101) = Convert(varchar,'{2}', 101)", employeeID, transactionID, startDate);
            dtResult = dal.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }   
        public static string UpdateRoutes(string sqlUpdate)
        {
            string msg = string.Empty;
            int ctr = 0;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    ctr = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch
                {
                    msg = "Saving was unsuccessful. \n";
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (ctr == 0 && msg.Equals(string.Empty))
            {
                msg = "No routes were updated. There may be no user using the specified route.";
            }

            return (msg.Equals(string.Empty) ? "Successfully updated routes." : msg);
        }

        public void InsertEmployeeRouteTrail(string employeeId, string routeId, string transactionId, string userLogin, string startDate, DALHelper dal)
        {
            string lastseqNo = GetLastSeqNoInTrail(employeeId, transactionId, dal);
            string query = string.Format(@"INSERT INTO T_EmployeeApprovalRouteTrail
                                              ( Arm_EmployeeID
                                              , Arm_TransactionID
                                              , Arm_RouteID
                                              , Arm_StartDate
                                              , Arm_EndDate
                                              , Arm_NotifyEndorse
                                              , Arm_NotifyApprove
                                              , Arm_NotifyReturn
                                              , Arm_NotifyDisapprove
                                              , Arm_SeqNo 
                                              , Arm_Status
                                              , Usr_Login
                                              , Ludatetime )
                                         SELECT Arm_EmployeeID
                                              , Arm_TransactionID
                                              , '{2}'
                                              , Arm_StartDate
                                              , Arm_EndDate
                                              , Arm_NotifyEndorse
                                              , Arm_NotifyApprove
                                              , Arm_NotifyReturn
                                              , Arm_NotifyDisapprove
                                              , '{4}'
                                              , Arm_Status
                                              , '{3}'
                                              , GETDATE()
                                           FROM T_EmployeeApprovalRoute
                                           WHERE Arm_TransactionId = '{1}' 
                                            AND Arm_EmployeeId = '{0}'
                                            AND Arm_StartDate = '{5}'
                                            ", employeeId, transactionId, routeId, userLogin, lastseqNo, startDate);
            dal.ExecuteNonQuery(query, CommandType.Text);
        }

        public void InserUpdateNotification(DataRow dr, string userLogged, DALHelper dal)
        {
            #region PrameterInfo
            ParameterInfo[] param = new ParameterInfo[7];
            param[0] = new ParameterInfo("@Arm_EmployeeId", dr["Arm_EmployeeId"].ToString());
            param[1] = new ParameterInfo("@Arm_TransactionID", dr["Arm_TransactionID"].ToString());
            param[2] = new ParameterInfo("@Arm_NotifyEndorse", dr["Arm_NotifyEndorse"].ToString());
            param[3] = new ParameterInfo("@Arm_NotifyApprove", dr["Arm_NotifyApprove"].ToString());
            param[4] = new ParameterInfo("@Arm_NotifyReturn", dr["Arm_NotifyReturn"].ToString());
            param[5] = new ParameterInfo("@Arm_NotifyDisapprove", dr["Arm_NotifyDisapprove"].ToString());
            param[6] = new ParameterInfo("@Usr_Login", userLogged);
            #endregion

            string sqlInsertUpdate = @" IF EXISTS (SELECT Arm_EmployeeId
                                                     FROM T_EmployeeApprovalRoute
                                                    WHERE Arm_EmployeeId = @Arm_EmployeeId
                                                      AND Arm_TransactionId = @Arm_TransactionId
                                                      AND Convert(varchar,GETDATE(),101) BETWEEN Arm_StartDate AND ISNULL(Arm_EndDate, GETDATE()))
                                     BEGIN 
                                    UPDATE T_EmployeeApprovalRoute
                                       SET Arm_NotifyEndorse = @Arm_NotifyEndorse
                                         , Arm_NotifyApprove = @Arm_NotifyApprove
                                         , Arm_NotifyReturn = @Arm_NotifyReturn
                                         , Arm_NotifyDisapprove = @Arm_NotifyDisapprove
                                         , Usr_Login = @Usr_Login
                                         , Ludatetime = GETDATE()
                                     WHERE Arm_EmployeeId = @Arm_EmployeeId
                                       AND Arm_TransactionId = @Arm_TransactionId
                                       END

                                      ELSE

                                     BEGIN
                                    INSERT INTO T_EmployeeApprovalRoute
                                             ( Arm_EmployeeId
                                             , Arm_TransactionID
                                             , Arm_RouteID
                                             , Arm_NotifyEndorse
                                             , Arm_NotifyApprove
                                             , Arm_NotifyReturn
                                             , Arm_NotifyDisapprove
                                             , Arm_Status
                                             , Usr_login
                                             , Ludatetime )
                                       VALUES( @Arm_EmployeeId
                                             , @Arm_TransactionId
                                             ,''
                                             , @Arm_NotifyEndorse
                                             , @Arm_NotifyApprove
                                             , @Arm_NotifyReturn
                                             , @Arm_NotifyDisapprove
                                             , 'A'
                                             , @Usr_Login
                                             , GETDATE() )
                                       END
                                            ";
            dal.ExecuteNonQuery(sqlInsertUpdate, CommandType.Text, param);
        }

        //Password Module Methods
        public void UpdateUserPassword(string employeeId, string password, string userLogged, DALHelper dal)
        {
            string sql = string.Empty;
            //each DB
            if (true)//!Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
            {
                sql = @" UPDATE T_UserMaster
                               SET Umt_Userpswd = @Password
                                 , user_login = @UserLogged
                                 , Ludatetime = GETDATE()
                             WHERE Umt_UserCode = @EmployeeId ";
            }
//            //For Password Update in Profiles DB
//            else
//            {
//                sql = @" UPDATE T_UserMaster
//                               SET Umt_Password = @Password
//                                 , Umt_EffectivityDate = CONVERT(char(10), GETDATE(), 101)
//                                 , User_Login = @UserLogged
//                                 , Ludatetime = GETDATE()
//                             WHERE Umt_UserCode = @EmployeeId ";
//            }
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo(@"EmployeeId", employeeId);
            param[1] = new ParameterInfo("@UserLogged", userLogged);
            param[2] = new ParameterInfo("@Password", encryptPassword(password));

            dal.ExecuteNonQuery(sql, CommandType.Text, param);
        }

        //Perth Added For Password Update in Profiles DB
        public void UpdateUserPassword2(string employeeId, string password, string userLogged, DALHelper dal)
        {
            string sql = @" UPDATE T_UserMaster
                               SET Umt_Password = @Password
                                 , Umt_EffectivityDate = CONVERT(char(10), GETDATE(), 101)
                                 , Usr_Login = @UserLogged
                                 , Ludatetime = GETDATE()
                             WHERE Umt_UserCode = @EmployeeId ";
            
            
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo(@"EmployeeId", employeeId);
            param[1] = new ParameterInfo("@UserLogged", userLogged);
            param[2] = new ParameterInfo("@Password", encryptPassword(password));

            dal.ExecuteNonQuery(sql, CommandType.Text, param);
        }
        //End

        public string isValidPassword(string password)
        {
            string errMessage = string.Empty;
            int PASSWRDCMB = 0;
            int PASSWRDEXP = 0;
            int PASSWRDLEN = 0;
            int PASSWRDUSE = 0;
            string hold = string.Empty;
            string sqlParameter = @"  SELECT Pmt_ParameterID
                                           , Pmt_ParameterDesc 
                                           , Convert(int,Pmt_NumericValue) [Pmt_NumericValue]
                                        FROM T_ParameterMaster
                                       WHERE Pmt_ParameterID IN ('PASSWRDCMB','PASSWRDEXP','PASSWRDLEN','PASSWRDUSE')";

            string sqlPasswordSet = @"SELECT Pcs_Code
                                           , Pcs_Description
                                           , Pcs_EnumeratedList
                                           , 'FALSE' [inUseFlag]
                                        FROM T_PasswordCharacterSet
                                       WHERE Pcs_Status = 'A'";
            DataSet dsPasswordSet = new DataSet();
            DataSet dsParameter = new DataSet();
            DataTable dtSet = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsParameter = dal.ExecuteDataSet(sqlParameter, CommandType.Text);
                    dsPasswordSet = dal.ExecuteDataSet(sqlPasswordSet, CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (!CommonMethods.isEmpty(dsPasswordSet) && !CommonMethods.isEmpty(dsParameter))
            {
                dtSet = dsPasswordSet.Tables[0];
                for (int i = 0; i < password.Length; i++)
                {
                    hold = password.Substring(i,1);
                    for (int idx = 0; idx < dtSet.Rows.Count; idx++)
                    { 
                        if(dtSet.Rows[idx]["Pcs_EnumeratedList"].ToString().Contains(hold))
                        {
                            dtSet.Rows[idx]["inUseFlag"] = "TRUE";
                        }
                    }
                }
                for (int i = 0; i < dsParameter.Tables[0].Rows.Count; i++)
                {
                    switch (dsParameter.Tables[0].Rows[i]["Pmt_ParameterID"].ToString())
                    { 
                        case "PASSWRDCMB":
                            PASSWRDCMB = Convert.ToInt32(dsParameter.Tables[0].Rows[i]["Pmt_NumericValue"].ToString());
                            break;
                        case "PASSWRDEXP":
                            PASSWRDEXP = Convert.ToInt32(dsParameter.Tables[0].Rows[i]["Pmt_NumericValue"].ToString());
                            break;
                        case "PASSWRDLEN":
                            PASSWRDLEN = Convert.ToInt32(dsParameter.Tables[0].Rows[i]["Pmt_NumericValue"].ToString());
                            break;
                        case "PASSWRDUSE":
                            PASSWRDUSE = Convert.ToInt32(dsParameter.Tables[0].Rows[i]["Pmt_NumericValue"].ToString());
                            break;
                        default:
                            break;
                    }
                }
                if (PASSWRDLEN != 0 && password.Length < PASSWRDLEN && errMessage.Equals(string.Empty))
                {
                    errMessage += "\nMinimum pasword length:" + PASSWRDLEN.ToString();
                }
                if (PASSWRDCMB != 0 && usedSet(dtSet) < PASSWRDCMB && errMessage.Equals(string.Empty))
                {
                    errMessage += "\nPassword combination must contain at least 1 character out of the " + PASSWRDCMB.ToString() + " following combination." + getAvailableSet(dtSet);
                }
            }
            return errMessage;
        }

        public bool isPasswordExpired(string userCode)
        {
            bool expired = false;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    expired = Convert.ToBoolean(dal.ExecuteScalar(
                    string.Format(@"SELECT 
		                                CASE 
                                            WHEN Pmt_NumericValue = 0 THEN 'FALSE'
			                                WHEN DATEADD(d, Pmt_NumericValue, Umt_effectivityDAte) < GETDATE() THEN 'TRUE'
			                                ELSE 'FALSE'
		                                END [Expired]
		                            FROM T_UserMaster 
		                            JOIN T_ParameterMaster ON Pmt_ParameterID = 'PASSWRDEXP' WHERE Umt_Usercode = '{0}'", userCode)));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return expired;
        }
        public bool isUsedPrev(string EmployeeID, string password)
    {
            bool isused=false;
        string[] ProfilesDBConnection = GetProfileConnections();
        using (DALHelper dal = new DALHelper(ProfilesDBConnection[0], ProfilesDBConnection[1]))
        {
            try
            {
                dal.OpenDB();
                string sqlIsusedPassPrev = string.Format(@"IF(EXISTS(SELECT Pmt_CharValue 
			                                                            FROM T_ParameterMaster
		                                                                WHERE Pmt_ParameterID = 'PASSTRLCHK'))
	                                                            BEGIN
		                                                            IF(SELECT Pmt_CharValue 
			                                                            FROM T_ParameterMaster 
			                                                            WHERE Pmt_ParameterID = 'PASSTRLCHK') = '0'
				                                                            BEGIN
	                                                            DECLARE @TOP int
	                                                            IF(EXISTS(SELECT Pmt_NumericValue
							                                                             FROM T_ParameterMaster 
							                                                             WHERE Pmt_ParameterID = 'PASSWRDCNT'))
							                                                             BEGIN
		                                                            SET @TOP = (SELECT 
						                                                            CASE WHEN (ISNULL(Pmt_NumericValue,0)-2) < 0
							                                                            THEN 0
							                                                            ELSE (ISNULL(Pmt_NumericValue,0)-2) 
						                                                            END
					                                                            FROM T_ParameterMaster WHERE Pmt_ParameterID = 'PASSWRDCNT')
	                                                            END
		                                                            ELSE SET @TOP= 0
				
	                                                            DECLARE @PASSWORD TABLE
	                                                            (Umt_Password varchar(max), Umt_EffectivityDate datetime)

	                                                            INSERT INTO @PASSWORD
	                                                            select TOP (@TOP) Umt_Password, Umt_EffectivityDate from T_PasswordTrail
	                                                            WHERE Umt_Usercode = '{0}'
	                                                            ORDER BY Umt_EffectivityDate desc
                                                                
	                                                            IF(SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = 'PASSWRDCNT') > 0
	                                                            BEGIN
		                                                            INSERT INTO @PASSWORD
		                                                            SELECT Umt_Password, Umt_EffectivityDate FROM T_UserMaster
		                                                            WHERE Umt_Usercode = '{0}'
	                                                            END

	                                                            SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END FROM @PASSWORD
	                                                            WHERE Umt_Password COLLATE Latin1_General_CS_AS = '{1}'
	                                                            END
		                                                            ELSE 
			                                                            BEGIN
			                                                            IF EXISTS(SELECT Umt_Password from T_PasswordTrail
									                                                            WHERE Umt_Usercode ='{0}'
										                                                            AND Umt_Password='{1}')
				                                                            SELECT 1
			                                                            ELSE
				                                                            SELECT 0
			                                                            END
	                                                            END
                                                            ELSE SELECT 0", EmployeeID, encryptPassword(password));

                isused = Convert.ToBoolean(dal.ExecuteScalar(sqlIsusedPassPrev));
            }
            catch { }
        }
        return isused;
    }
        public bool isCorrectCurrentPass(string currentPassword, string employeeId)
        {
            string passwordDB = string.Empty;

            if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
            {
                GeneralBL GNBL = new GeneralBL();
                string[] ProfileDBConnection = GNBL.GetProfileConnections();
                using (DALHelper dal = new DALHelper(ProfileDBConnection[0], ProfileDBConnection[1]))
                {
                    try
                    {
                        dal.OpenDB();
                        passwordDB = dal.ExecuteScalar(string.Format(@" SELECT Umt_Password
                                                                          FROM T_UserMaster
                                                                         WHERE Umt_UserCode = '{0}' ", employeeId), CommandType.Text).ToString();
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
            }
            else
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        passwordDB = dal.ExecuteScalar(string.Format(@" SELECT Umt_UserPswd
                                                                          FROM T_UserMaster
                                                                         WHERE Umt_UserCode = '{0}' ", employeeId), CommandType.Text).ToString();
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
            }

            return passwordDB.Equals(encryptPassword(currentPassword));
        }

        private int usedSet(DataTable dt)
        { 
            int ctr = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dt.Rows[i]["inUseFlag"].ToString()))
                {
                    ctr++;
                }
            }
            return ctr;
        }

        private string getAvailableSet(DataTable dt)
        {
            string retValue = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                retValue += "\n " + dt.Rows[i]["Pcs_Description"].ToString();
            }
            return retValue;
        }

        private string encryptPassword(string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Pass = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] arrB;
            StringBuilder sb = new StringBuilder(String.Empty);

            arrB = md5Pass.ComputeHash(Encoding.ASCII.GetBytes(password));

            foreach (byte b in arrB)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }

            md5Pass.Clear();

            return sb.ToString().Substring(0, 15);
        }

        //Announcement Methods

        public void CreateAnnouncement(DataRow dr)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[9];
            paramDetails[0] = new ParameterInfo("@Amt_ControlNo", CommonMethods.GetControlNumberFromProfile("ANNOUNCE"));
            paramDetails[1] = new ParameterInfo("@Amt_AnnounceDateTime", dr["Amt_AnnounceDateTime"]);
            paramDetails[2] = new ParameterInfo("@Amt_Announcer", dr["Amt_Announcer"]);
            paramDetails[3] = new ParameterInfo("@Amt_Subject", dr["Amt_Subject"]);
            paramDetails[4] = new ParameterInfo("@Amt_Description", dr["Amt_Description"], SqlDbType.VarChar, dr["Amt_Description"].ToString().Length);
            paramDetails[5] = new ParameterInfo("@Amt_Priority", dr["Amt_Priority"]);
            paramDetails[6] = new ParameterInfo("@Amt_Status", dr["Amt_Status"]);
            paramDetails[7] = new ParameterInfo("@User_Login", dr["User_Login"]);
            paramDetails[8] = new ParameterInfo("@Amt_ProfileInclude", dr["Amt_ProfileInclude"]);
            #endregion

            #region SQL
            string sql = @" INSERT INTO T_AnnouncementMaster
                                 ( Amt_ControlNo
                                 , Amt_AnnounceDateTime
                                 , Amt_Announcer
                                 , Amt_Subject
                                 , Amt_Description
                                 , Amt_Priority
                                 , Amt_ProfileInclude
                                 , Amt_Status
                                 , User_Login
                                 , Ludatetime)
                          VALUES ( @Amt_ControlNo
                                 , @Amt_AnnounceDateTime
                                 , @Amt_Announcer
                                 , @Amt_Subject
                                 , @Amt_Description
                                 , @Amt_Priority
                                 , @Amt_ProfileInclude
                                 , @Amt_Status
                                 , @User_Login
                                 , GETDATE())
                            ";
            #endregion

            string[] ProfileDBConnections = GetProfileConnections();
            using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    dal.ExecuteNonQuery(sql, CommandType.Text, paramDetails);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    CommonMethods.ErrorsToTextFile(ex, dr["User_Login"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void UpdateAnnouncement(DataRow dr)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[9];
            paramDetails[0] = new ParameterInfo("@Amt_ControlNo", dr["Amt_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Amt_AnnounceDateTime", dr["Amt_AnnounceDateTime"]);
            paramDetails[2] = new ParameterInfo("@Amt_Announcer", dr["Amt_Announcer"]);
            paramDetails[3] = new ParameterInfo("@Amt_Subject", dr["Amt_Subject"]);
            paramDetails[4] = new ParameterInfo("@Amt_Description", dr["Amt_Description"], SqlDbType.VarChar, dr["Amt_Description"].ToString().Length);
            paramDetails[5] = new ParameterInfo("@Amt_Priority", dr["Amt_Priority"]);
            paramDetails[6] = new ParameterInfo("@Amt_Status", dr["Amt_Status"]);
            paramDetails[7] = new ParameterInfo("@User_Login", dr["User_Login"]);
            paramDetails[8] = new ParameterInfo("@Amt_ProfileInclude", dr["Amt_ProfileInclude"]);
            #endregion

            #region SQL
            string sql = @" UPDATE T_AnnouncementMaster
                               SET Amt_AnnounceDateTime = @Amt_AnnounceDateTime
                                 , Amt_Announcer = @Amt_Announcer
                                 , Amt_Subject = @Amt_Subject
                                 , Amt_Description = @Amt_Description
                                 , Amt_Priority = @Amt_Priority
                                 , Amt_Status = @Amt_Status
                                 , User_Login = @User_Login
                                 , Ludatetime = GETDATE()
                                 , Amt_ProfileInclude = @Amt_ProfileInclude
                             WHERE Amt_ControlNo = @Amt_ControlNo ";
            #endregion

            string[] ProfileDBConnections = GetProfileConnections();
            using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
            //using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    dal.ExecuteNonQuery(sql, CommandType.Text, paramDetails);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    CommonMethods.ErrorsToTextFile(ex, dr["User_Login"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }


        //Perth Added 08/24/2011
        public string[] GetProfileConnections()
        {
            string[] ret = new string[2];
            ret[0] = string.Empty;
            ret[1] = string.Empty;

            ret[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            ret[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString());

            return ret;
        }

        public string isValidPassword2(string password)
        {
            string errmsg = string.Empty;
            int PasswordLen = 0;
            int PasswordComb = 0;
            DataSet dschar = null;
            string defaultPassword = string.Empty;

            string[] ProfilesDBConnection = GetProfileConnections();

            #region Query Parameters for Password

            using (DALHelper dal = new DALHelper(ProfilesDBConnection[0], ProfilesDBConnection[1]))
            {
                try
                {
                    dal.OpenDB();
                    PasswordLen = Convert.ToInt32(dal.ExecuteScalar(
                                @"
                                select Pmt_NumericValue 
                                from T_ParameterMaster
                                where Pmt_ParameterID = 'PASSWRDLEN'
                                ", CommandType.Text));
                    PasswordComb = Convert.ToInt32(dal.ExecuteScalar(
                            @"
                                select Pmt_NumericValue 
                                from T_ParameterMaster
                                where Pmt_ParameterID = 'PASSWRDCMB'
                                ", CommandType.Text));
                    dschar = dal.ExecuteDataSet(
                            @"
                                select 
	                                Pcs_EnumeratedList
                                    ,Pcs_Description
                                from T_PasswordCharacterSet
                                where Pcs_Status = 'A'
                                ");
                    defaultPassword = GetDefaultPassword(dal);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            #endregion

            #region Compare password to Parameters

            #region Test length

            if (password.Trim().Length < PasswordLen)
            {
                errmsg += "\n Password length should be greater / equal to " + PasswordLen.ToString();
            }

            #endregion

            #region Test Password Combinations

            string TempPass = password;

            if (dschar != null)
            {
                int ctr = 0;
                for (int idx = 0; idx < dschar.Tables[0].Rows.Count; idx++)
                {
                    string characters = dschar.Tables[0].Rows[idx]["Pcs_EnumeratedList"].ToString();
                    if (characters.Trim() == string.Empty)
                    {
                        characters = " ";
                    }
                    bool flag = false;
                    for (int i = 0; i < characters.Length; i++)
                    {
                        string ch = characters[i].ToString();
                        if (TempPass.IndexOf(ch) != -1)
                            flag = true;
                    }
                    if (flag)
                        ctr++;
                }
                if (ctr < PasswordComb)
                {
                    string s = "\n Password must have " + PasswordComb.ToString() + " / " + dschar.Tables[0].Rows.Count.ToString() + " of the following combination(s):";
                    for (int idx = 0; idx < dschar.Tables[0].Rows.Count; idx++)
                    {
                        s += "\n  - " + dschar.Tables[0].Rows[idx]["Pcs_Description"].ToString() + " ( \"" + dschar.Tables[0].Rows[idx]["Pcs_EnumeratedList"].ToString() + "\")";
                    }
                    errmsg += s;
                }
            }

            #endregion

            #region Test password is default
            if (password.Equals(defaultPassword))
            {
                errmsg += "\nYou are using a default password. It is recommended to change your password to avoid others from gaining access to your account.";
            }
            #endregion

            #endregion

            return errmsg;
        }

        public bool isPasswordExpired2(string userCode)
        {
            bool expired = false;

            string[] ProfilesDBConnection = GetProfileConnections();

            using (DALHelper dal = new DALHelper(ProfilesDBConnection[0], ProfilesDBConnection[1]))
            {
                try
                {
                    dal.OpenDB();
                    expired = Convert.ToBoolean(dal.ExecuteScalar(
                    string.Format(@"SELECT 
		                                CASE 
                                            WHEN Pmt_NumericValue = 0 THEN 'FALSE'
			                                WHEN DATEADD(d, Pmt_NumericValue, Umt_effectivityDAte) < GETDATE() THEN 'TRUE'
			                                ELSE 'FALSE'
		                                END [Expired]
		                            FROM T_UserMaster 
		                            JOIN T_ParameterMaster ON Pmt_ParameterID = 'PASSWRDEXP' WHERE Umt_Usercode = '{0}'", userCode)));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return expired;
        }

        //Add End

        public string GetDefaultPassword(DALHelper dal)
        {
            string defaultPassword = string.Empty;
            
            string sql = @"     SELECT Pmt_CharValue 
            					  FROM T_ParameterMaster
                                 WHERE Pmt_ParameterID = 'PASSWRDDEF'";

            defaultPassword = Convert.ToString(dal.ExecuteScalar(sql));
            
            return defaultPassword;
        }

        public void InsertPasswordTrail(string employeeId, string userLogged, DALHelper dal)
        {
            string sql = string.Format(@"INSERT T_PasswordTrail
                                         SELECT '{0}', GETDATE(), Umt_UserPswd, '{1}', GETDATE() 
                                            FROM T_UserMaster WHERE Umt_Usercode = '{0}'", employeeId, userLogged);

            dal.ExecuteNonQuery(sql);
        }

        public void InsertPasswordTrail2(string employeeId, string userLogged, DALHelper dal)
        {
            string sql = string.Format(@"INSERT T_PasswordTrail
                                         SELECT '{0}', GETDATE(), Umt_Password, '{1}', GETDATE() 
                                            FROM T_UserMaster WHERE Umt_Usercode = '{0}'", employeeId, userLogged);

            dal.ExecuteNonQuery(sql);
        }
        public void UpdateLatestRoute(string employeeID
            , string transactionID
            , string startDate
            , DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@EmployeeID", employeeID);
            param[1] = new ParameterInfo("@TransactionID", transactionID);
            param[2] = new ParameterInfo("@StartDate", startDate);

            string query = @"UPDATE T_EmployeeApprovalRoute
                                    SET Arm_EndDate = dateadd(dd, -1, @StartDate)
                                    WHERE Arm_EmployeeID = @EmployeeID
                                    AND Arm_TransactionID = @TransactionID
                                    AND Arm_StartDate = (SELECT MAX(Route.Arm_StartDate)
                                    FROM T_EmployeeApprovalRoute Route
                                    WHERE Arm_EmployeeID = @EmployeeID
                                    AND Arm_TransactionID = @TransactionID)";
            dal.ExecuteNonQuery(query, CommandType.Text, param);
        }
    }
}
