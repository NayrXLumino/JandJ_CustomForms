/*  Revision no. W2.1.00001 
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/11/2013
 *  Update Notes    :   
 *      -   Updated Shift details retrieval to retrieve correct future details
 */
/*
 * Updated By       : 1090 - Marx
 * Updated Date     : 04/17/2013
 * Update Notes     : Updated query for sqlDefaultInfo 
 *                    Return empty string when null value for MAX date
 *                    Null occurs when employee not found in LogLedger
 *                    Added condition to pass if MAX value return empty string
 */
/*
 * Updated By       : 1277 - Robert
 * Updated Date     : 10/10/2013
 * Update Notes     : Added a function to get the parameterchar value.Used for refresh. THI
 */
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using CommonLibrary;
using System.IO;

namespace Payroll.DAL
{
    public class CommonMethods
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session; 
        static System.Web.SessionState.HttpSessionState sessionStatic = HttpContext.Current.Session;
        private enum UpLevel { chrome, firefox, safari }
        #region Email Parameters (Company Sepcific)
        private static string fromParameter = "noreply-workflow@n-pax.com";
        private static string subjectParameter = "Workflow Notification";
        private static string urlParameter = "192.168.135.139";
        private static string smtpSevrer = Encrypt.decryptText(ConfigurationManager.AppSettings["SMTPServer"].ToString());
        private static string smtpUsername = Encrypt.decryptText(ConfigurationManager.AppSettings["SMTPUsername"].ToString());
        private static string smtpPassword = Encrypt.decryptText(ConfigurationManager.AppSettings["SMTPPassword"].ToString());
        #endregion
        public CommonMethods()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //20100909 Andre added new methods
        #region IsUplevel Browser property
        public static bool IsUplevel
        {
            get
            {
                bool ret = false;
                string _browser;

                try
                {

                    if (HttpContext.Current == null) return ret;
                    _browser = HttpContext.Current.Request.UserAgent.ToLower();

                    foreach (UpLevel br in Enum.GetValues(typeof(UpLevel)))
                    { if (_browser.Contains(br.ToString())) { ret = true; break; } }

                    return ret;
                }
                catch { return ret; }
            }
        }
        #endregion

        #region Checks the connection if it is still alive
        public static bool isAlive()
        {
            bool isAlive = true;
            System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
            try
            {
                isAlive = !session["dbConn"].ToString().Equals(string.Empty);
            }
            catch
            {
                isAlive = false;
            }
            return isAlive;
        }
        #endregion

        #region Gets the check/approve rights for checking or approval
        public static bool getCheckRights(string userID, string sysMenuCode)
        {
            DataSet dsRights = new DataSet();

            #region sql query
            string sqlGetRights = @"SELECT CASE WHEN (ISNULL(Convert(int,Ugt_CanCheck), 0) + ISNULL(Convert(int,Ugt_CanApprove), 0) > 0)
                                                THEN 'TRUE'
                                                ELSE 'FALSE'
                                            END
                                      FROM T_UserGrant
                                      LEFT JOIN T_UserGroupDetail 
                                        ON Ugd_UserCode = '{0}'
                                     WHERE Ugt_SystemId  = Ugd_SystemId
                                       AND Ugt_UserGroup = Ugd_UserGroupCode
                                       AND Ugt_SysMenuCode = '{1}'
                                       AND Ugt_status    = 'A'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRights = dal.ExecuteDataSet(string.Format(sqlGetRights, userID, sysMenuCode), CommandType.Text);
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

            if (dsRights != null && dsRights.Tables.Count > 0 && dsRights.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean((dsRights.Tables[0].Rows[0][0].ToString()));
            else
                return false;
        }

        #endregion

        #region Gets the Start or End date of the given Quincena( C - current or F - Future)
        public static DateTime getQuincenaDate(char cycleIndic, string startOrEnd)
        {
            DataSet dsQuincena = new DataSet();
            string sql = @"  SELECT TOP 1 Ppm_StartCycle
                                 , Ppm_EndCycle 
                              FROM T_PayPeriodMaster
                             WHERE Ppm_CycleIndicator = '{0}' 
                               AND Ppm_Status = 'A'";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                        dsQuincena = dal.ExecuteDataSet(string.Format(sql, cycleIndic), CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            switch (startOrEnd.ToUpper())
            {
                case "START":
                    return Convert.ToDateTime(dsQuincena.Tables[0].Rows[0]["Ppm_StartCycle"]);
                case "END":
                    return Convert.ToDateTime(dsQuincena.Tables[0].Rows[0]["Ppm_EndCycle"]);
                default:
                    return DateTime.Today;
            }
        }
        #endregion

        #region Gets the minimum date available on the database logledger
        public static DateTime getMinimumDate()
        {
            DataSet ds = new DataSet();
            string sql = @"  SELECT TOP 1 Ppm_StartCycle
                            FROM T_PayPeriodMaster
                            WHERE Ppm_CycleIndicator = 'P'
                            ORDER BY Ppm_StartCycle";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql), CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return Convert.ToDateTime(ds.Tables[0].Rows[0][0]);
        }
        #endregion

        #region Gets the PayPeriod based on the date of the given Quincena( C - current or F - Future)
        public static string getPayPeriod(string cycleIndic)
        {
            string sql = @"SELECT TOP 1 Ppm_PayPeriod
                             FROM T_PayPeriodMaster
                            WHERE Ppm_CycleIndicator = '{0}'";
            string payPeriod = string.Empty;
            using(DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    payPeriod = Convert.ToString(dal.ExecuteScalar(string.Format(sql, cycleIndic), CommandType.Text));
                }
                catch
                {
                
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return payPeriod;
        }

        public static string getPayPeriod(DateTime date)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@date", date);
            string sql = @" SELECT Ppm_PayPeriod
                              FROM T_PayPeriodMaster
                             WHERE @date BETWEEN Ppm_StartCycle AND Ppm_EndCycle
                                AND Ppm_CycleIndicator <> 'S'";
            string payPeriod = string.Empty;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    payPeriod = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text, param));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return payPeriod;
        }
        #endregion

        #region Get new control number for transaction
        public static string GetControlNumber(string transactionCode)
        {
            string controlNum = string.Empty;

            string sqlControlNoFetch = @"   DECLARE @yr2Digit varchar(2)
                                            SET @yr2Digit = (SELECT right(Ccd_CurrentYear, 2) from T_CompanyMaster)
    
                                            

                                            SELECT Tcm_TransactionPrefix 
                                                + '{1}'
	                                            + @yr2Digit 
	                                            + replicate('0', 8 - len(RTrim(Tcm_LastSeries+1)))
	                                            + RTrim(Tcm_LastSeries+1)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
                                            WHERE Tcm_TransactionCode = '{0}'

                                            UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'";

            string sqlControlNoUpdate = @"  UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    //dal.ExecuteNonQuery(string.Format(sqlControlNoUpdate, transactionCode.ToUpper()), CommandType.Text);
                    controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper(), HttpContext.Current.Session["dbPrefix"].ToString()), CommandType.Text));
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    ErrorsToTextFile(ex, "Control Number Retrieval GetControlNumber(): " + HttpContext.Current.Session["userLogged"].ToString());
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (controlNum == null
                || controlNum.Trim() == string.Empty)
            {
                if (transactionCode != "LEAVE" && transactionCode != "OVERTIME")
                    throw new Exception(string.Format("Control Number was not created for {0}.", transactionCode));
            }
            return controlNum;
        }
        public static string GetControlNumber(string transactionCode,DALHelper dal)
        {
            string controlNum = string.Empty;

            string sqlControlNoFetch = @"   DECLARE @yr2Digit varchar(2)
                                            SET @yr2Digit = (SELECT right(Ccd_CurrentYear, 2) from T_CompanyMaster)
    
                                            UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'

                                            SELECT Tcm_TransactionPrefix 
                                                + '{1}'
	                                            + @yr2Digit 
	                                            + replicate('0', 8 - len(RTrim(Tcm_LastSeries+1)))
	                                            + RTrim(Tcm_LastSeries)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
                                            WHERE Tcm_TransactionCode = '{0}'";
            string sqlControlNoUpdate = @"  UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'";
            controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper(), HttpContext.Current.Session["dbPrefix"].ToString()), CommandType.Text));
           
            if (controlNum == null
                || controlNum.Trim() == string.Empty)
            {
                if (transactionCode != "LEAVE" && transactionCode != "OVERTIME")
                    throw new Exception(string.Format("Control Number was not created for {0}.", transactionCode));
            }
            return controlNum;
        }
        public static string GetControlNumberFromProfile(string transactionCode)
        {
            string controlNum = string.Empty;

            string sqlControlNoFetch = @"
                                            UPDATE T_TransactionControlMaster
                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
                                            WHERE Tcm_TransactionCode = '{0}'

                                            SELECT Tcm_TransactionPrefix 
                                                + '{1}'
	                                            + replicate('0', 10 - len(RTrim(Tcm_LastSeries)))
	                                            + RTrim(Tcm_LastSeries)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
                                            WHERE Tcm_TransactionCode = '{0}'";

//            string sqlControlNoUpdate = @"  UPDATE T_TransactionControlMaster
//                                            SET Tcm_LastSeries = Tcm_LastSeries + 1
//                                            WHERE Tcm_TransactionCode = '{0}'";
            string profileServer = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            string profileDB = Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString());
            using (DALHelper dal = new DALHelper(profileServer, profileDB))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    //dal.ExecuteNonQuery(string.Format(sqlControlNoUpdate, transactionCode.ToUpper()), CommandType.Text);
                    controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper(), HttpContext.Current.Session["dbPrefix"].ToString()), CommandType.Text));
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    ErrorsToTextFile(ex, "Control Number Retrieval GetControlNumberFromProfile() : " + HttpContext.Current.Session["userLogged"].ToString());
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (controlNum == null
                || controlNum.Trim() == string.Empty)
            {
                if (transactionCode != "LEAVE" && transactionCode != "OVERTIME")
                    throw new Exception(string.Format("Control Number was not created for {0}.", transactionCode));
            }
            return controlNum;
        }
        #endregion

        #region Checks the object if not empty
        public static bool isEmpty(DataSet ds)
        {
            bool isEmpty = true;
            try
            {
                if( ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isEmpty = false;
                }
            }
            catch
            {
                isEmpty = true;
            }
            return isEmpty;
        }

        #endregion

        #region Checks if costcenter access is ALL per transaction type
        public static bool isAllCostcenterAccess(string userId, string transactionType)
        {
            int isAll = 0;
            string sql = @" SELECT COUNT(Uca_CostCenterCode)
                              FROM T_UserCostCenterAccess
                             WHERE Uca_UserCode = @UserCode
                               AND Uca_SytemId = @transactionType
                               AND Uca_CostCenterCode = 'ALL'
                               AND Uca_Status = 'A'";
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@UserCode", userId);
            param[1] = new ParameterInfo("@TransactionType", transactionType);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    isAll = Convert.ToInt32(dal.ExecuteScalar(sql, CommandType.Text, param).ToString());
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return (isAll > 0);
        }
        #endregion

        #region Get Profile Access of User
        public static DataTable GetProfileAccess(string userCode)
        {
            DataSet ds = null;

            string[] ProfileDBConnections = new GeneralBL().GetProfileConnections();
            using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(@"    SELECT Prf_Database
                                                                ,  Prf_Profile
                                                                FROM T_UserProfile 
	                                                            JOIN T_Profiles 
		                                                           ON Upt_DatabaseNo = Prf_DatabaseNo
                                                                WHERE Upt_UserCode = '{0}'", userCode));
                }
                catch { }
                finally
                {
                    dal.CloseDB();
                }
            }

            return CommonMethods.isEmpty(ds) ? new DataTable() : ds.Tables[0];
        }

        #endregion

        #region Get error message for cut-off
        public static string GetErrorMessageForCutOff(string transactionType)
        {
            string message = string.Empty;
            if (Convert.ToBoolean(Resources.Resource.ALLOWFLOW))
            {
                message = transactionType + " is on Cut-Off. Payroll is currently being processed. Only application for the future quincena(s) are open for transaction as of this time.";
            }
            else
            {
                message = transactionType + " is on Cut-Off. Transactions are being held for processing. Transaction will be available after cut-off is released or after payroll calculation.";
            }

            message += "\n" + GetUserLudatetimeOfProcessControl(transactionType);

            return message;
        }

        private static string GetUserLudatetimeOfProcessControl(string transactionType)
        {
            string sqlGetInfo = @"  SELECT ' Set by '
                                         + ISNULL(Umt_userfname, Emt_FirstName)
                                         + ' ' 
                                         + ISNULL(Umt_userlname, Emt_LastName)
                                         + ' on '
                                         + CONVERT(varchar(10), T_ProcessControlMaster.Ludatetime, 101)
                                         + ' '
                                         + CONVERT(varchar(5), T_ProcessControlMaster.Ludatetime, 114)
                                      FROM T_ProcessControlMaster
                                      LEFT JOIN T_UserMaster
                                        ON Umt_Usercode = T_ProcessControlMaster.Usr_Login
                                      LEFT JOIN T_EmployeeMaster
                                        ON Emt_EmployeeID  = T_ProcessControlMaster.Usr_Login
                                     WHERE Pcm_SystemID = '{0}'
                                       AND Pcm_ProcessID = 'CUT-OFF'";
            string message = "";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    message = dal.ExecuteScalar(string.Format(sqlGetInfo, transactionType), CommandType.Text).ToString();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return message;
        }

        public static string GetErrorMessageForCYCCUTOFF()
        {
            string sqlGetInfo = @"  SELECT 'PAYROLL CYCLE processing is on-going. Processing is set by '
                                         + ISNULL(Umt_userfname, Emt_FirstName)
                                         + ' ' 
                                         + ISNULL(Umt_userlname, Emt_LastName)
                                         + ' on '
                                         + CONVERT(varchar(10), T_ProcessControlMaster.Ludatetime, 101)
                                         + ' '
                                         + CONVERT(varchar(5), T_ProcessControlMaster.Ludatetime, 114)
                                      FROM T_ProcessControlMaster
                                      LEFT JOIN T_UserMaster
                                        ON Umt_Usercode = T_ProcessControlMaster.Usr_Login
                                      LEFT JOIN T_EmployeeMaster
                                        ON Emt_EmployeeID  = T_ProcessControlMaster.Usr_Login
                                     WHERE Pcm_SystemID = 'PAYROLL'
                                       AND Pcm_ProcessID = 'CYCCUT-OFF'";
            string message = "";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    message = dal.ExecuteScalar(sqlGetInfo, CommandType.Text).ToString();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return message;
        }
        #endregion

        #region Gets the shift information of employee on a given date
        public static DataSet getEmployeeShift(string empId, string MMddyyyy)
        {
            #region SQL
            string sql = @" SELECT Ell_ShiftCode [Code]
                                 , Ell_DayCode [DayCode]
                                 , Scm_ShiftDesc [Desc]
                                 , Scm_ScheduleType [Type]
                                 , Scm_ShiftTimeIn [TimeIn]
                                 , Scm_ShiftBreakStart [BreakStart]
                                 , Scm_ShiftBreakEnd [BreakEnd]
                                 , Scm_ShiftTimeOut [TimeOut]
                                 , Scm_ShiftHours [Hours]
                                 , Scm_PaidBreak [PaidBreak]
                                 , Ell_ActualTimeIn_1 [TimeIn1]
                                 , Ell_ActualTimeOut_1 [TimeOut1]
                                 , Ell_ActualTimeIn_2 [TimeIn2]
                                 , Ell_ActualTimeOut_2 [TimeOut2]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate = @Date
                               AND Ell_EmployeeId = @UserId
                               
                             UNION

                            SELECT Ell_ShiftCode 
                                 , Ell_DayCode
                                 , Scm_ShiftDesc
                                 , Scm_ScheduleType
                                 , Scm_ShiftTimeIn
                                 , Scm_ShiftBreakStart
                                 , Scm_ShiftBreakEnd
                                 , Scm_ShiftTimeOut
                                 , Scm_ShiftHours
                                 , Scm_PaidBreak
                                 , Ell_ActualTimeIn_1
                                 , Ell_ActualTimeOut_1
                                 , Ell_ActualTimeIn_2
                                 , Ell_ActualTimeOut_2 
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate = @Date
                               AND Ell_EmployeeId = @UserId";
            #endregion

            #region sqlDefaultInfo
            string sqlDefaultInfo = @" SELECT Emt_ShiftCode [Code]
                                                , 'REG' [DayCode]
                                                , Scm_ShiftDesc [Desc]
                                                , Scm_ScheduleType [Type]
                                                , Scm_ShiftTimeIn [TimeIn]
                                                , Scm_ShiftBreakStart [BreakStart]
                                                , Scm_ShiftBreakEnd [BreakEnd]
                                                , Scm_ShiftTimeOut [TimeOut]
                                                , Scm_ShiftHours [Hours]
                                                , Scm_PaidBreak [PaidBreak]
                                                , '0000' [TimeIn1]
                                                , '0000' [TimeOut1]
                                                , '0000' [TimeIn2]
                                                , '0000' [TimeOut2]
                                             FROM T_EmployeeMaster
                                            INNER JOIN T_ShiftCodeMaster
                                               ON Scm_ShiftCode = Emt_Shiftcode
                                            WHERE Emt_EmployeeId = @UserId

                                            SELECT ISNULL(Convert(varchar(10), MAX(Ell_ProcessDate), 101),'') [MAX]
                                              FROM T_EmployeeLogLedger
                                             WHERE Ell_EMployeeId = @UserId";
            #endregion

            #region sqlGetFutureDate
            string sqlGetFutureDate = @"    DECLARE @START AS Datetime
                                            DECLARE @END AS Datetime
                                            DECLARE @INCREMENT AS Int
                                            DECLARE @COUNT AS Int
                                            DECLARE @RESTDAY AS char(7)
                                            DECLARE @EMPID AS varchar(15)

                                            SET @START = '{0}'
                                            SET @END = '{1}'
                                            SET @EMPID = '{2}'

                                            CREATE TABLE {3} (x_date datetime)

                                            INSERT INTO {3}
                                            SELECT TOP ( datediff(DAY,@START,@END) + 1 )
                                                        [Date] = dateadd(DAY,ROW_NUMBER()
                                                    OVER(ORDER BY c1.name),
                                                    DATEADD(DD,-1,@START))
                                            FROM   [master].[dbo].[spt_values] c1 

                                            SET @RESTDAY = (SELECT Erd_RestDay 
                                                              FROM T_EmployeeRestDay
                                                             WHERE Erd_EmployeeID = @EMPID)
                        
                                            SET @COUNT = 0
                                            SET @INCREMENT = 1
                                            WHILE @INCREMENT < 7
                                            BEGIN
                                                    IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                                        DELETE FROM {3}                                      --NOT RESTDAY
                                                        WHERE datepart(dw,x_date) = @INCREMENT + 1 
                                                    SET @INCREMENT = @INCREMENT + 1
                                            END

                                            IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                                    DELETE FROM {3}
                                                    WHERE datepart(dw,x_date) = 1

                                            SELECT * 
                                            FROM {3}
                                            WHERE x_date NOT IN (SELECT Hmt_HolidayDate FROM T_HolidayMaster)   --NOT HOlIDAY

                                            DROP TABLE {3} ";
            #endregion

            #region GetFutureDates
            string sqlFutureNew = @"
DECLARE @Ell_EmployeeID AS VARCHAR(20)
DECLARE @StartDate AS VARCHAR(20)
DECLARE @EndDate AS VARCHAR(20)
	SET @Ell_EmployeeID = @UserId
	SET @StartDate = @Date
	SET @EndDate = @Date

DECLARE @WORKTYPE AS VARCHAR(20)
DECLARE @WORKGROUP AS VARCHAR(20)
DECLARE @SHIFTCODE AS VARCHAR(20)
DECLARE @SHIFTCODEREST AS VARCHAR(20)
DECLARE @LOCATIONCODE AS VARCHAR(20)

SET @WORKTYPE = (
SELECT 
	RTRIM(Emt_WorkType)
FROM T_EmployeeMaster
WHERE Emt_EmployeeID = @Ell_EmployeeID)

SET @WORKGROUP = (
SELECT 
	RTRIM(Emt_WorkGroup)
FROM T_EmployeeMaster
WHERE Emt_EmployeeID = @Ell_EmployeeID)

SET @SHIFTCODE = (
SELECT 
	Emt_Shiftcode 
FROM T_EmployeeMaster 
WHERE Emt_EmployeeID = @Ell_EmployeeID )

SET @SHIFTCODEREST = (
SELECT
	CASE WHEN Scm_EquivalentShiftCode IS NULL OR RTRIM(Scm_EquivalentShiftCode) = ''
		THEN Scm_ShiftCode
		ELSE Scm_EquivalentShiftCode
		END 
FROM T_ShiftCodeMaster
WHERE Scm_ShiftCode = @SHIFTCODE
)

SET @LOCATIONCODE = (
SELECT 
	Emt_LocationCode 
FROM T_EmployeeMaster 
WHERE Emt_EmployeeID = @Ell_EmployeeID )

IF @WORKTYPE <> 'REG' 
BEGIN
	SELECT 
		@Ell_EmployeeID [EmployeeID]
		,CONVERT(VARCHAR(20), Cal_ProcessDate, 101) [ProcessDate]
		, Cal_ShiftCode [ShiftCode]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN
				Hmt_HolidayCode 
			ELSE		
			 CASE WHEN RTRIM(Cal_WorkCode) = 'R'
				THEN 'REST'
				ELSE 'REG' +  CASE WHEN LEN(RTRIM(Cal_WorkCode)) > 1
								THEN RIGHT(RTRIM(Cal_WorkCode), 1)
								ELSE ''
								END
				END 
			END [DayCode]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeIn
					ELSE SC1.Scm_ShiftTimeIn
				END
			ELSE 
				SC1.Scm_ShiftTimeIn
			END [TimeIn]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeOut
					ELSE SC1.Scm_ShiftTimeOut
				END
			ELSE 
				SC1.Scm_ShiftTimeOut
			END  [TimeOut]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakStart
					ELSE SC1.Scm_ShiftBreakStart
				END
			ELSE 
				SC1.Scm_ShiftBreakStart
			END  [BreakStart]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakEnd
					ELSE SC1.Scm_ShiftBreakEnd
				END
			ELSE 
				SC1.Scm_ShiftBreakEnd
			END  [BreakEnd]
        , coalesce(SHIFTTYPE.Adt_AccountDesc + ' SHIFT',  (
            CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE 
				SC1.Scm_ScheduleType
			END)) [SchedType]
	    , CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftDesc
					ELSE SC1.Scm_ShiftDesc
				END
			ELSE 
				SC1.Scm_ShiftDesc
			END  [ShiftDesc]
	FROM T_CalendarGroupTmp
	LEFT JOIN T_ShiftCodeMaster SC1
		ON Cal_ShiftCode = SC1.Scm_ShiftCode
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = Cal_ProcessDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE @LOCATIONCODE
									END
	LEFT JOIN T_ShiftCodeMaster SC2
		ON SC2.Scm_ShiftCode = @SHIFTCODEREST

    LEFT JOIN T_AccountDetail SHIFTTYPE
        ON SHIFTTYPE.Adt_AccountCode = (CASE WHEN Hmt_HolidayDate IS NOT NULL
			                            THEN 
				                            CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					                            THEN SC2.Scm_ScheduleType
					                            ELSE SC1.Scm_ScheduleType
				                            END
			                            ELSE 
				                            SC1.Scm_ScheduleType
			                            END)
        AND SHIFTTYPE.Adt_AccountType = 'SCHEDTYPE'

	WHERE Cal_WorkType = @WORKTYPE
	AND Cal_WorkGroup = @WORKGROUP
	AND Cal_ProcessDate BETWEEN @StartDate AND @EndDate
END
ELSE
BEGIN
	SELECT 
		@Ell_EmployeeID [EmployeeID]
		, CONVERT(VARCHAR(20), CalendarDate, 101) [ProcessDate]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftCode
					ELSE SC1.Scm_ShiftCode
				END
			ELSE 
				SC1.Scm_ShiftCode
			END [ShiftCode]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN 
				CASE WHEN Hmt_HolidayDate IS NOT NULL
					THEN 
						Hmt_HolidayCode
					ELSE
						'REST'
				END
			ELSE
				'REG'
			END [DayCode]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeIn
					ELSE SC1.Scm_ShiftTimeIn
				END
			ELSE
				SC1.Scm_ShiftTimeIn
			END [TimeIn]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeOut
					ELSE SC1.Scm_ShiftTimeOut
				END
			ELSE
				SC1.Scm_ShiftTimeOut
			END [TimeOut]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakStart
					ELSE SC1.Scm_ShiftBreakStart
				END
			ELSE
				SC1.Scm_ShiftBreakStart
			END [BreakStart]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakEnd
					ELSE SC1.Scm_ShiftBreakEnd
				END
			ELSE
				SC1.Scm_ShiftBreakEnd
			END [BreakEnd]
        , coalesce(SHIFTTYPE.Adt_AccountDesc + ' SHIFT',  (
            CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE
				SC1.Scm_ScheduleType
			END)) [SchedType]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftDesc
					ELSE SC1.Scm_ShiftDesc
				END
			ELSE
				SC1.Scm_ShiftDesc
			END [ShiftDesc]
	FROM dbo.GetCalendarDates(@StartDate, @EndDate) CAL
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = CAL.CalendarDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE @LOCATIONCODE
									END
	LEFT JOIN T_ShiftCodeMaster SC1 
		ON SC1.Scm_ShiftCode = @SHIFTCODE
	LEFT JOIN T_ShiftCodeMaster SC2
		ON SC2.Scm_ShiftCode = @SHIFTCODEREST
    LEFT JOIN T_AccountDetail SHIFTTYPE
        ON SHIFTTYPE.Adt_AccountCode = (
            CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE
				SC1.Scm_ScheduleType
			END)
        AND SHIFTTYPE.Adt_AccountType = 'SCHEDTYPE'
END
            ";
            #endregion

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@UserId", empId, SqlDbType.VarChar, 15);
            param[1] = new ParameterInfo("@Date", MMddyyyy, SqlDbType.DateTime);

            DataSet ds = new DataSet();
            DataSet dsDefault = new DataSet();
            DataSet dsTemp = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                    dsDefault = dal.ExecuteDataSet(sqlDefaultInfo , CommandType.Text, param);
                }
                catch (Exception ex)
                {
                    ErrorsToTextFile(ex, "SA");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (isEmpty(ds) && !empId.Equals(string.Empty))
            {
                if( dsDefault.Tables[1].Rows[0]["MAX"].ToString().Equals("") 
                    && dsDefault.Tables[1].Rows.Count > 0
                    || Convert.ToDateTime(MMddyyyy) > Convert.ToDateTime(dsDefault.Tables[1].Rows[0]["MAX"].ToString())) 
                {
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransactionSnapshot();
                            /****************************************************************************************************************************\
                            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetFutureDate, <start date>
                                                                                      , <end date>
                                                                                      , <employee id>
                                                                                      , <temporary table name start with underscore> ), CommandType.Text);
                            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetFutureDate, MMddyyyy
                                                                                      , MMddyyyy
                                                                                      , empId
                                                                                      , "_" + empId + MMddyyyy.Replace("/", string.Empty)), CommandType.Text); 
                            \****************************************************************************************************************************/
                            dsTemp = dal.ExecuteDataSet(sqlFutureNew, CommandType.Text, param);

                            if (!isEmpty(dsTemp))
                            {
                                string shiftCode = dsTemp.Tables[0].Rows[0]["ShiftCode"].ToString().Trim();
                                string dayCode = dsTemp.Tables[0].Rows[0]["DayCode"].ToString().Trim();
                                dsTemp = dal.ExecuteDataSet(string.Format(@"
SELECT '{0}' [Code]
     , '{1}' [DayCode]
     , Scm_ShiftDesc [Desc]
     , Scm_ScheduleType [Type]
     , Scm_ShiftTimeIn [TimeIn]
     , Scm_ShiftBreakStart [BreakStart]
     , Scm_ShiftBreakEnd [BreakEnd]
     , Scm_ShiftTimeOut [TimeOut]
     , Scm_ShiftHours [Hours]
     , Scm_PaidBreak [PaidBreak]
     , Scm_ShiftTimeIn [TimeIn1]
     , Scm_ShiftBreakStart [TimeOut1]
     , Scm_ShiftBreakEnd [TimeIn2]
     , Scm_ShiftTimeOut [TimeOut2]
  FROM T_ShiftCodeMaster
  WHERE Scm_ShiftCode = '{0}'
                                ", shiftCode, dayCode));
                            }

                            dal.CommitTransactionSnapshot();
                        }
                        catch(Exception ex)
                        {
                            dal.RollBackTransactionSnapshot();
                            ErrorsToTextFile(ex, "SA");
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
                    if (isEmpty(dsTemp))
                    {
                        return dsDefault;
                    }
                    else
                    {
                        return dsTemp;
                    }
                }
                else
                {
                    return ds;
                }

            }
            else
            {
                return ds;
            }
        }
        #endregion

        #region Gets the default shift. CHIYODA specific method
        public static DataSet getDefaultShiftCHIYODA(string EmployeeID, string MMddyyyy)
        {
            string sql = @" SELECT DefaultShift.Scm_ShiftCode [Code]
                                 , Ell_DayCode [DayCode]
                                 , DefaultShift.Scm_ShiftDesc [Desc]
                                 , DefaultShift.Scm_ScheduleType [Type]
                                 , DefaultShift.Scm_ShiftTimeIn [TimeIn]
                                 , DefaultShift.Scm_ShiftBreakStart [BreakStart]
                                 , DefaultShift.Scm_ShiftBreakEnd [BreakEnd]
                                 , DefaultShift.Scm_ShiftTimeOut [TimeOut]
                                 , DefaultShift.Scm_ShiftHours [Hours]
                                 , DefaultShift.Scm_PaidBreak [PaidBreak]
                                 , Ell_ActualTimeIn_1 [TimeIn1]
                                 , Ell_ActualTimeOut_1 [TimeOut1]
                                 , Ell_ActualTimeIn_2 [TimeIn2]
                                 , Ell_ActualTimeOut_2 [TimeOut2]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_ShiftCodeMaster ShiftCode
                                ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                              LEFT JOIN T_ShiftCodeMaster DefaultShift
                                ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                               AND DefaultShift.Scm_DefaultShift = 'True'
                               AND DefaultShift.Scm_Status = 'A'
                             WHERE Ell_ProcessDate = '{0}'
                               AND Ell_EmployeeID = '{1}'
                             UNION
                            SELECT DefaultShift.Scm_ShiftCode [Code]
                                 , Ell_DayCode [DayCode]
                                 , DefaultShift.Scm_ShiftDesc [Desc]
                                 , DefaultShift.Scm_ScheduleType [Type]
                                 , DefaultShift.Scm_ShiftTimeIn [TimeIn]
                                 , DefaultShift.Scm_ShiftBreakStart [BreakStart]
                                 , DefaultShift.Scm_ShiftBreakEnd [BreakEnd]
                                 , DefaultShift.Scm_ShiftTimeOut [TimeOut]
                                 , DefaultShift.Scm_ShiftHours [Hours]
                                 , DefaultShift.Scm_PaidBreak [PaidBreak]
                                 , Ell_ActualTimeIn_1 [TimeIn1]
                                 , Ell_ActualTimeOut_1 [TimeOut1]
                                 , Ell_ActualTimeIn_2 [TimeIn2]
                                 , Ell_ActualTimeOut_2 [TimeOut2]
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_ShiftCodeMaster ShiftCode
                                ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                              LEFT JOIN T_ShiftCodeMaster DefaultShift
                                ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                               AND DefaultShift.Scm_DefaultShift = 'True'
                               AND DefaultShift.Scm_Status = 'A'
                             WHERE Ell_ProcessDate = '{0}'
                               AND Ell_EmployeeID = '{1}' ";


            #region sqlDefaultInfo
            string sqlDefaultInfo = @" SELECT DefaultShift.Scm_ShiftCode [Code]
                                            , 'REG' [DayCode]
                                            , DefaultShift.Scm_ShiftDesc [Desc]
                                            , DefaultShift.Scm_ScheduleType [Type]
                                            , DefaultShift.Scm_ShiftTimeIn [TimeIn]
                                            , DefaultShift.Scm_ShiftBreakStart [BreakStart]
                                            , DefaultShift.Scm_ShiftBreakEnd [BreakEnd]
                                            , DefaultShift.Scm_ShiftTimeOut [TimeOut]
                                            , DefaultShift.Scm_ShiftHours [Hours]
                                            , DefaultShift.Scm_PaidBreak [PaidBreak]
                                            , '0000' [TimeIn1]
                                            , '0000' [TimeOut1]
                                            , '0000' [TimeIn2]
                                            , '0000' [TimeOut2]
                                         FROM T_EmployeeMaster
                                        INNER JOIN T_ShiftCodeMaster ShiftCode
                                           ON ShiftCode.Scm_ShiftCode = Emt_ShiftCode
                                         LEFT JOIN T_ShiftCodeMaster DefaultShift
                                           ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                                          AND DefaultShift.Scm_DefaultShift = 'True'
                                          AND DefaultShift.Scm_Status = 'A'
                                        WHERE Emt_EmployeeId = @UserId

                                       SELECT Convert(varchar(10), MAX(Ell_ProcessDate), 101) [MAX]
                                         FROM T_EmployeeLogLedger
                                        WHERE Ell_EMployeeId = @UserId";
            #endregion

            #region sqlGetFutureDate
            string sqlGetFutureDate = @"    DECLARE @START AS Datetime
                                            DECLARE @END AS Datetime
                                            DECLARE @INCREMENT AS Int
                                            DECLARE @COUNT AS Int
                                            DECLARE @RESTDAY AS char(7)
                                            DECLARE @EMPID AS varchar(15)

                                            SET @START = '{0}'
                                            SET @END = '{1}'
                                            SET @EMPID = '{2}'

                                            CREATE TABLE {3} (x_date datetime)

                                            INSERT INTO {3}
                                            SELECT TOP ( datediff(DAY,@START,@END) + 1 )
                                                        [Date] = dateadd(DAY,ROW_NUMBER()
                                                    OVER(ORDER BY c1.name),
                                                    DATEADD(DD,-1,@START))
                                            FROM   [master].[dbo].[spt_values] c1 

                                            SET @RESTDAY = (SELECT Erd_RestDay 
                                                              FROM T_EmployeeRestDay
                                                             WHERE Erd_EmployeeID = @EMPID)
                        
                                            SET @COUNT = 0
                                            SET @INCREMENT = 1
                                            WHILE @INCREMENT < 7
                                            BEGIN
                                                    IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                                        DELETE FROM {3}                                      --NOT RESTDAY
                                                        WHERE datepart(dw,x_date) = @INCREMENT + 1 
                                                    SET @INCREMENT = @INCREMENT + 1
                                            END

                                            IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                                    DELETE FROM {3}
                                                    WHERE datepart(dw,x_date) = 1

                                            SELECT * 
                                            FROM {3}
                                            WHERE x_date NOT IN (SELECT Hmt_HolidayDate FROM T_HolidayMaster)   --NOT HOlIDAY

                                            DROP TABLE {3} ";
            #endregion

            #region GetFutureDates
            string sqlFutureNew = @"
DECLARE @Ell_EmployeeID AS VARCHAR(20)
DECLARE @StartDate AS VARCHAR(20)
DECLARE @EndDate AS VARCHAR(20)
	SET @Ell_EmployeeID = @UserId
	SET @StartDate = @Date
	SET @EndDate = @Date

DECLARE @WORKTYPE AS VARCHAR(20)
DECLARE @WORKGROUP AS VARCHAR(20)
DECLARE @SHIFTCODE AS VARCHAR(20)
DECLARE @SHIFTCODEREST AS VARCHAR(20)
DECLARE @LOCATIONCODE AS VARCHAR(20)

SET @WORKTYPE = (
SELECT 
	RTRIM(Emt_WorkType)
FROM T_EmployeeMaster
WHERE Emt_EmployeeID = @Ell_EmployeeID)

SET @WORKGROUP = (
SELECT 
	RTRIM(Emt_WorkGroup)
FROM T_EmployeeMaster
WHERE Emt_EmployeeID = @Ell_EmployeeID)

SET @SHIFTCODE = (
SELECT 
	Emt_Shiftcode 
FROM T_EmployeeMaster 
WHERE Emt_EmployeeID = @Ell_EmployeeID )

SET @SHIFTCODEREST = (
SELECT
	CASE WHEN Scm_EquivalentShiftCode IS NULL OR RTRIM(Scm_EquivalentShiftCode) = ''
		THEN Scm_ShiftCode
		ELSE Scm_EquivalentShiftCode
		END 
FROM T_ShiftCodeMaster
WHERE Scm_ShiftCode = @SHIFTCODE
)

SET @LOCATIONCODE = (
SELECT 
	Emt_LocationCode 
FROM T_EmployeeMaster 
WHERE Emt_EmployeeID = @Ell_EmployeeID )

IF @WORKTYPE <> 'REG' 
BEGIN
	SELECT 
		@Ell_EmployeeID [EmployeeID]
		,CONVERT(VARCHAR(20), Cal_ProcessDate, 101) [ProcessDate]
		, Cal_ShiftCode [ShiftCode]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN
				Hmt_HolidayCode 
			ELSE		
			 CASE WHEN RTRIM(Cal_WorkCode) = 'R'
				THEN 'REST'
				ELSE 'REG' +  CASE WHEN LEN(RTRIM(Cal_WorkCode)) > 1
								THEN RIGHT(RTRIM(Cal_WorkCode), 1)
								ELSE ''
								END
				END 
			END [DayCode]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeIn
					ELSE SC1.Scm_ShiftTimeIn
				END
			ELSE 
				SC1.Scm_ShiftTimeIn
			END [TimeIn]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeOut
					ELSE SC1.Scm_ShiftTimeOut
				END
			ELSE 
				SC1.Scm_ShiftTimeOut
			END  [TimeOut]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakStart
					ELSE SC1.Scm_ShiftBreakStart
				END
			ELSE 
				SC1.Scm_ShiftBreakStart
			END  [BreakStart]
		, CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakEnd
					ELSE SC1.Scm_ShiftBreakEnd
				END
			ELSE 
				SC1.Scm_ShiftBreakEnd
			END  [BreakEnd]
        , coalesce(SHIFTTYPE.Adt_AccountDesc + ' SHIFT',  (
            CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE 
				SC1.Scm_ScheduleType
			END)) [SchedType]
	    , CASE WHEN Hmt_HolidayDate IS NOT NULL
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftDesc
					ELSE SC1.Scm_ShiftDesc
				END
			ELSE 
				SC1.Scm_ShiftDesc
			END  [ShiftDesc]
	FROM T_CalendarGroupTmp
	LEFT JOIN T_ShiftCodeMaster SC1
		ON Cal_ShiftCode = SC1.Scm_ShiftCode
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = Cal_ProcessDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE @LOCATIONCODE
									END
	LEFT JOIN T_ShiftCodeMaster SC2
		ON SC2.Scm_ShiftCode = @SHIFTCODEREST

    LEFT JOIN T_AccountDetail SHIFTTYPE
        ON SHIFTTYPE.Adt_AccountCode = (CASE WHEN Hmt_HolidayDate IS NOT NULL
			                            THEN 
				                            CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					                            THEN SC2.Scm_ScheduleType
					                            ELSE SC1.Scm_ScheduleType
				                            END
			                            ELSE 
				                            SC1.Scm_ScheduleType
			                            END)
        AND SHIFTTYPE.Adt_AccountType = 'SCHEDTYPE'

	WHERE Cal_WorkType = @WORKTYPE
	AND Cal_WorkGroup = @WORKGROUP
	AND Cal_ProcessDate BETWEEN @StartDate AND @EndDate
END
ELSE
BEGIN
	SELECT 
		@Ell_EmployeeID [EmployeeID]
		, CONVERT(VARCHAR(20), CalendarDate, 101) [ProcessDate]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN 
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftCode
					ELSE SC1.Scm_ShiftCode
				END
			ELSE 
				SC1.Scm_ShiftCode
			END [ShiftCode]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN 
				CASE WHEN Hmt_HolidayDate IS NOT NULL
					THEN 
						Hmt_HolidayCode
					ELSE
						'REST'
				END
			ELSE
				'REG'
			END [DayCode]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeIn
					ELSE SC1.Scm_ShiftTimeIn
				END
			ELSE
				SC1.Scm_ShiftTimeIn
			END [TimeIn]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftTimeOut
					ELSE SC1.Scm_ShiftTimeOut
				END
			ELSE
				SC1.Scm_ShiftTimeOut
			END [TimeOut]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakStart
					ELSE SC1.Scm_ShiftBreakStart
				END
			ELSE
				SC1.Scm_ShiftBreakStart
			END [BreakStart]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftBreakEnd
					ELSE SC1.Scm_ShiftBreakEnd
				END
			ELSE
				SC1.Scm_ShiftBreakEnd
			END [BreakEnd]
        , coalesce(SHIFTTYPE.Adt_AccountDesc + ' SHIFT',  (
            CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE
				SC1.Scm_ScheduleType
			END)) [SchedType]
		, CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ShiftDesc
					ELSE SC1.Scm_ShiftDesc
				END
			ELSE
				SC1.Scm_ShiftDesc
			END [ShiftDesc]
	FROM dbo.GetCalendarDates(@StartDate, @EndDate) CAL
	LEFT JOIN T_HolidayMaster
		ON Hmt_HolidayDate = CAL.CalendarDate
		AND Hmt_ApplicCity = CASE WHEN Hmt_ApplicCity = 'ALL' 
									THEN Hmt_ApplicCity
									ELSE @LOCATIONCODE
									END
	LEFT JOIN T_ShiftCodeMaster SC1 
		ON SC1.Scm_ShiftCode = @SHIFTCODE
	LEFT JOIN T_ShiftCodeMaster SC2
		ON SC2.Scm_ShiftCode = @SHIFTCODEREST
    LEFT JOIN T_AccountDetail SHIFTTYPE
        ON SHIFTTYPE.Adt_AccountCode = (
            CASE WHEN 1 = (
			SELECT 
				CASE WHEN CAL.DayOfWeek = 1
				THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
				ELSE 
					RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
				END
			FROM T_EmployeeRestDay E1
			WHERE Erd_EmployeeID = @Ell_EmployeeID
				AND Erd_EffectivityDate = (
					SELECT 
						TOP 1 Erd_EffectivityDate 
					FROM T_EmployeeRestDay E2
					WHERE E2.Erd_EmployeeID = @Ell_EmployeeID
						AND E2.Erd_EffectivityDate <= CalendarDate
					ORDER BY E2.Erd_EffectivityDate DESC
				)
			) 
			THEN
				CASE WHEN SC2.Scm_ShiftCode IS NOT NULL
					THEN SC2.Scm_ScheduleType
					ELSE SC1.Scm_ScheduleType
				END
			ELSE
				SC1.Scm_ScheduleType
			END)
        AND SHIFTTYPE.Adt_AccountType = 'SCHEDTYPE'
END
            ";
            #endregion

            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@UserId", EmployeeID);
            param[1] = new ParameterInfo("@Date", MMddyyyy);
            param[2] = new ParameterInfo("@DefaultShift", Resources.Resource.DEFAULTSHIFT);

            DataSet ds = new DataSet();
            DataSet dsDefault = new DataSet();
            DataSet dsTemp = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, MMddyyyy, EmployeeID), CommandType.Text, param);
                    dsDefault = dal.ExecuteDataSet(sqlDefaultInfo, CommandType.Text, param);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (isEmpty(ds) && !EmployeeID.Equals(string.Empty))
            {
                if (dsDefault.Tables[1].Rows.Count > 0
                    && (dsDefault.Tables[1].Rows[0]["MAX"].ToString().Equals("")
                    || Convert.ToDateTime(MMddyyyy) > Convert.ToDateTime(dsDefault.Tables[1].Rows[0]["MAX"].ToString()))) 
                {
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransactionSnapshot();
                            dal.BeginTransactionSnapshot();
                            /****************************************************************************************************************************\
                            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetFutureDate, <start date>
                                                                                      , <end date>
                                                                                      , <employee id>
                                                                                      , <temporary table name start with underscore> ), CommandType.Text);
                            dsTemp = dal.ExecuteDataSet(string.Format(sqlGetFutureDate, MMddyyyy
                                                                                      , MMddyyyy
                                                                                      , EmployeeID
                                                                                      , "_" + EmployeeID + MMddyyyy.Replace("/", string.Empty)), CommandType.Text); 
                            \****************************************************************************************************************************/
                            dsTemp = dal.ExecuteDataSet(sqlFutureNew, CommandType.Text, param);

                            if (!isEmpty(dsTemp))
                            {
                                string shiftCode = dsTemp.Tables[0].Rows[0]["ShiftCode"].ToString().Trim();
                                string dayCode = dsTemp.Tables[0].Rows[0]["DayCode"].ToString().Trim();
                                dsTemp = dal.ExecuteDataSet(string.Format(@"
SELECT DefaultShift.Scm_ShiftCode [Code]
	, '{1}' [DayCode]
	, DefaultShift.Scm_ShiftDesc [Desc]
	, DefaultShift.Scm_ScheduleType [Type]
	, DefaultShift.Scm_ShiftTimeIn [TimeIn]
	, DefaultShift.Scm_ShiftBreakStart [BreakStart]
	, DefaultShift.Scm_ShiftBreakEnd [BreakEnd]
	, DefaultShift.Scm_ShiftTimeOut [TimeOut]
	, DefaultShift.Scm_ShiftHours [Hours]
	, DefaultShift.Scm_PaidBreak [PaidBreak]
	, '0000' [TimeIn1]
	, '0000' [TimeOut1]
	, '0000' [TimeIn2]
	, '0000' [TimeOut2]
  FROM T_ShiftCodeMaster ShiftCode

LEFT JOIN T_ShiftCodeMaster DefaultShift
	ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
	AND DefaultShift.Scm_DefaultShift = 'True'
	AND DefaultShift.Scm_Status = 'A'
WHERE ShiftCode.Scm_ShiftCode = '{0}'
                                ", shiftCode, dayCode));
                            }
                            dal.CommitTransactionSnapshot();
                        }
                        catch (Exception ex)
                        {
                            dal.RollBackTransactionSnapshot();
                            ErrorsToTextFile(ex, "getDefaultChiyodaShift");
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
                    if (isEmpty(dsTemp))
                    {
                        return dsDefault;
                    }
                    else
                    {
                        return dsTemp;
                    }
                }
                else
                {
                    return ds;
                }

            }
            else
            {
                return ds;
            }
        }
        #endregion

        #region Get available shifts for a given day
        public static DataSet getAvailableShiftForTheDay(string MMddyyyy, string workTYPGRP)
        {
            string sql = @" SELECT DISTINCT Ell_ShiftCode [Code]
                                 , Ell_DayCode [DayCode]
                                 , Scm_ShiftDesc [Desc]
                                 , Scm_ScheduleType [Type]
                                 , Scm_ShiftTimeIn [TimeIn]
                                 , Scm_ShiftBreakStart [BreakStart]
                                 , Scm_ShiftBreakEnd [BreakEnd]
                                 , Scm_ShiftTimeOut [TimeOut]
                                 , Scm_ShiftHours [Hours]
                                 , Scm_PaidBreak [PaidBreak]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate = @Date
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup
                               
                             UNION

                            SELECT DISTINCT Ell_ShiftCode 
                                 , Ell_DayCode
                                 , Scm_ShiftDesc
                                 , Scm_ScheduleType
                                 , Scm_ShiftTimeIn
                                 , Scm_ShiftBreakStart
                                 , Scm_ShiftBreakEnd
                                 , Scm_ShiftTimeOut
                                 , Scm_ShiftHours
                                 , Scm_PaidBreak
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_ShiftCodeMaster
                                ON Scm_ShiftCode = Ell_ShiftCode
                             WHERE Ell_ProcessDate = @Date
                               AND Ell_WorkType = @WorkType
                               AND Ell_WorkGroup = @WorkGroup";
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@Date", MMddyyyy);
            param[1] = new ParameterInfo("@WorkType", workTYPGRP.Substring(0,3).Trim());
            param[2] = new ParameterInfo("@WorkGroup", workTYPGRP.Substring(3, 3).Trim());
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

            return ds;
        }
        #endregion

        #region Gets the shift information from the shift code passed as parameter
        public static DataSet getShiftInformation(string shiftCode)
        {
            DataSet ds = new DataSet();
            string sql = @" SELECT Scm_ShiftCode 
                                 , Scm_ShiftDesc
                                 , Scm_ScheduleType
                                 , Scm_ShiftTimeIn
                                 , Scm_ShiftBreakStart
                                 , Scm_ShiftBreakEnd
                                 , Scm_ShiftTimeOut
                                 , Scm_ShiftHours
                                 , Scm_PaidBreak
                              FROM T_ShiftCodeMaster
                             WHERE Scm_ShiftCode = @shiftCode";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@shiftCode", shiftCode);
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

            return ds;
        }
        #endregion

        #region Gets the menu rights of the user
        public static DataRow getUserGrant(string empId, string menuCode)
        {
            string sql = @"  SELECT Ugt_CanRetrieve
                                  , Ugt_CanAdd
                                  , Ugt_CanEdit
                                  , Ugt_CanDelete
                                  , Ugt_CanGenerate
                                  , Ugt_CanCheck
                                  , Ugt_CanApprove
                                  , Ugt_CanPrintPreview
                                  , Ugt_CanPrint
                                  , Ugt_CanReprint
                               FROM T_UserGrant
                               LEFT JOIN T_UserGroupDetail
                                 ON Ugd_Systemid = Ugt_SystemId
                                AND Ugd_UserGroupCode = Ugt_UserGroup
                              WHERE Ugd_UserCode = @UserCode
                              AND Ugt_SysmenuCode = @MenuCode";
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@UserCode", empId);
            param[1] = new ParameterInfo("@MenuCode", menuCode);
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

            return ds.Tables[0].Rows[0];
        }
        #endregion

        #region Gets the status of the transaction based on employee route
        public static bool isCorrectRoutePage(string login,string empId, string transactionType, string status)
        {
            login = login.ToUpper();
            empId = empId.ToUpper();
            DataSet ds = new DataSet();
            string statusType = string.Empty;
            bool isCorrectRoutePage = false;
            if (status.ToUpper() == "NEW")
            {
                //if (empId == login)
                //{
                    isCorrectRoutePage = true;
                //}
            }
            else
            {
                string sql = @"SELECT a.Arm_Checker1 [C1]
                                , a.Arm_Checker2 [C2]
                                , a.Arm_Approver [AP]
                             FROM T_EmployeeApprovalRoute AS e
                             LEFT JOIN T_ApprovalRouteMaster AS a 
                               ON a.Arm_RouteId = e.Arm_RouteId
                            WHERE e.Arm_EmployeeId = @EmployeeId 
                              AND e.Arm_TransactionId = @TransactionType
                              AND (a.Arm_Checker1 LIKE '%{0}%'
                                    OR a.Arm_Checker2 LIKE '%{0}%'
                                    OR a.Arm_Approver LIKE '%{0}%'
                                    )
                             AND Convert(varchar,GETDATE(),101) BETWEEN e.Arm_StartDate AND ISNULL(e.Arm_EndDate, GETDATE())";

                ParameterInfo[] param = new ParameterInfo[2];
                param[0] = new ParameterInfo("@EmployeeId", empId);
                param[1] = new ParameterInfo("@TransactionType", transactionType);
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        ds = dal.ExecuteDataSet(string.Format(sql, login), CommandType.Text, param);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

                if (!isEmpty(ds))
                {
                    string C1 = ds.Tables[0].Rows[0]["C1"].ToString();
                    string C2 = ds.Tables[0].Rows[0]["C2"].ToString();
                    string AP = ds.Tables[0].Rows[0]["AP"].ToString();
                    switch (status.ToUpper())
                    {

                        case "ENDORSED TO CHECKER 1":
                            if (C1 == login)
                            {
                                isCorrectRoutePage = true;
                            }
                            break;
                        case "ENDORSED TO CHECKER 2":
                            if (C2 == login)
                            {
                                isCorrectRoutePage = true;
                            }
                            break;
                        case "ENDORSED TO APPROVER":
                            if (AP == login)
                            {
                                isCorrectRoutePage = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return isCorrectRoutePage;

        }
        public static string getStatusRoute(string empId, string transactionType, string btnName)
        {
            DataSet ds = new DataSet();
            string status = string.Empty;
            string sql = @"SELECT a.Arm_Checker1 [C1]
                                , a.Arm_Checker2 [C2]
                                , a.Arm_Approver [AP]
                             FROM T_EmployeeApprovalRoute AS e
                             LEFT JOIN T_ApprovalRouteMaster AS a 
                               ON a.Arm_RouteId = e.Arm_RouteId
                            WHERE e.Arm_EmployeeId = @EmployeeId 
                              AND e.Arm_TransactionId = @TransactionType
                              AND Convert(varchar,GETDATE(),101) BETWEEN e.Arm_StartDate AND ISNULL(e.Arm_EndDate, GETDATE())";
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@EmployeeId", empId);
            param[1] = new ParameterInfo("@TransactionType", transactionType);
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
            if (!isEmpty(ds))
            {
                string C1 = ds.Tables[0].Rows[0]["C1"].ToString();
                string C2 = ds.Tables[0].Rows[0]["C2"].ToString();
                string AP = ds.Tables[0].Rows[0]["AP"].ToString();
                switch (btnName.ToUpper())
                { 
                    case "ENDORSE TO CHECKER 1":
                        if (C1 == C2 && C2 == AP)
                        {
                            status = "7";
                        }
                        else if (C1 == C2)
                        {
                            status = "5";
                        }
                        else
                        {
                            status = "3";
                        }
                        break;
                    case "ENDORSE TO CHECKER 2":
                        if (C2 == AP)
                        {
                            status = "7";
                        }
                        else
                        {
                            status = "5";
                        }
                        break;
                    case "ENDORSE TO APPROVER":
                        status = "7";
                        break;
                    case "APPROVE":
                        status = "9";
                        break;
                    default:
                        break;
                }
            }
            return status;

        }
        public static string getStatusRoute(string empId, string transactionType, string btnName, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string status = string.Empty;
            string sql = @"SELECT a.Arm_Checker1 [C1]
                                , a.Arm_Checker2 [C2]
                                , a.Arm_Approver [AP]
                             FROM T_EmployeeApprovalRoute AS e
                             LEFT JOIN T_ApprovalRouteMaster AS a 
                               ON a.Arm_RouteId = e.Arm_RouteId
                            WHERE e.Arm_EmployeeId = @EmployeeId 
                              AND e.Arm_TransactionId = @TransactionType
                              AND Convert(varchar,GETDATE(),101) BETWEEN e.Arm_StartDate AND ISNULL(e.Arm_EndDate, GETDATE())";
            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@EmployeeId", empId);
            param[1] = new ParameterInfo("@TransactionType", transactionType);

            ds = dal.ExecuteDataSet(sql, CommandType.Text, param);

            if (!isEmpty(ds))
            {
                string C1 = ds.Tables[0].Rows[0]["C1"].ToString();
                string C2 = ds.Tables[0].Rows[0]["C2"].ToString();
                string AP = ds.Tables[0].Rows[0]["AP"].ToString();
                switch (btnName.ToUpper())
                {
                    case "ENDORSE TO CHECKER 1":
                        if (C1 == C2 && C2 == AP)
                        {
                            status = "7";
                        }
                        else if (C1 == C2)
                        {
                            status = "5";
                        }
                        else
                        {
                            status = "3";
                        }
                        break;
                    case "ENDORSE TO CHECKER 2":
                        if (C2 == AP)
                        {
                            status = "7";
                        }
                        else
                        {
                            status = "5";
                        }
                        break;
                    case "ENDORSE TO APPROVER":
                        status = "7";
                        break;
                    case "APPROVE":
                        status = "9";
                        break;
                    default:
                        status = "3";
                        break;
                }
            }
            else {
                status = "3";
            }
            return status;
        }
        #endregion

        #region Gets the Parameter Value
        public static decimal getParamterValue(string parameterID)
        {
            decimal value = 0;
            string sql = @"SELECT Pmt_NumericValue
                             FROM T_ParameterMaster
                            WHERE Pmt_ParameterID = @ParameterId";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ParameterId", parameterID);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = Convert.ToDecimal(dal.ExecuteScalar(sql, CommandType.Text, param));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return value;

        }
        ////robert update leave refresh parametermaster or resource 10/10/2013
        public static string getParamterCharValue(string parameterID)
        {
            string value = string.Empty;
            string sql = @"SELECT Pmt_CharValue
                             FROM T_ParameterMaster
                            WHERE Pmt_ParameterID = @ParameterId";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ParameterId", parameterID);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = dal.ExecuteScalar(sql, CommandType.Text, param).ToString();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return value;

        }
        #endregion

        #region Gets the Parameter Extension Values
        public static DataSet getParamterExtensionValues(string parameterID)
        {
            DataSet ds = new DataSet();
            string sql = @"SELECT Pmx_Classification
                                , Pmx_ParameterDesc
                                , Pmx_ParameterValue
                             FROM T_ParameterMasterExt
                            WHERE Pmx_ParameterID = @ParameterId
                              AND Pmx_Status = 'A'";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ParameterId", parameterID);

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

            return ds;

        }
        #endregion

        #region Return flag if date is still in current year
        public static bool isWithinCurrentYear(string MMddyyyy, DALHelper dal)
        {
            bool value = false;
            string sql = @"SELECT CASE WHEN datepart(yyyy,getdate()) < datepart(yyyy, convert(datetime,@LeaveDate))
		                               THEN 'TRUE'
		                               ELSE 'FALSE'
	                               END ";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@LeaveDate", MMddyyyy);
            value = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text, param).ToString());

            return value;
        }
        #endregion

        #region Get minimum date of filing
        public static DateTime getMinimumDateOfFiling()
        {
            DateTime returnDate = new DateTime();
            //use parameter master MINPASTPRD
            int defaultValue = 1;
            int parameterValue = 0;
            string sql = @" declare @Current as datetime
                                SET @Current = (SELECT Ppm_StartCycle 
                                                  FROM T_PayPeriodMaster 
                                                 WHERE Ppm_CycleIndicator = 'C')

                             SELECT ISNULL(MIN(Dates),@Current)
                               FROM ( SELECT TOP {0} Ppm_StartCycle [Dates]
	                                    FROM T_PayPeriodMaster
 	                                   WHERE Ppm_CycleIndicator = 'P'
	                                   ORDER BY 1 DESC) AS TEMP";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    try
                    {
                        parameterValue = Convert.ToInt32(getParamterValue("MINPASTPRD"));//if not setup this will proceed to catch statement and assign to default
                        sql = string.Format(sql, parameterValue);
                    }
                    catch
                    {
                        sql = string.Format(sql, defaultValue);
                    }
                    dal.OpenDB();
                    returnDate = Convert.ToDateTime(dal.ExecuteScalar(sql, CommandType.Text));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return returnDate;

        }
        #endregion

        #region Get minimum and maximum dateof filing
        public static string[] GetMinMaxDateOfFiling()
        {

            //use parameter master MINPASTPRD
            int defaultValue = 1;
            int parameterValue = 0;
            string sql = @" declare @Current as datetime
                            declare @MinDate as varchar(10)
                                SET @Current = (SELECT Ppm_StartCycle 
                                                  FROM T_PayPeriodMaster 
                                                 WHERE Ppm_CycleIndicator = 'C')
                                --robert change here.. sort then convert...not convert then sort
                                SET @MinDate = (SELECT ISNULL(MIN(Dates),@Current)
                                                  FROM ( SELECT TOP {0} Convert(varchar(10), Ppm_StartCycle, 101) [Dates]
	                                                       FROM T_PayPeriodMaster
 	                                                      WHERE Ppm_CycleIndicator = 'P'
	                                                      ORDER BY Ppm_StartCycle DESC) AS TEMP)
                               

                             SELECT @MinDate [MinDate]
                                  , Convert(varchar(10), MAX(Ell_ProcessDate), 101) [MaxDate]
                               FROM T_EmployeeLogLedger";
            DataSet ds = new DataSet();
            string[] value = new string[2] { string.Empty, string.Empty };
            using (DALHelper dal = new DALHelper())
            {
                try
                {

                    try
                    {
                        parameterValue = Convert.ToInt32(getParamterValue("MINPASTPRD"));//if not setup this will proceed to catch statement and assign to default
                        sql = string.Format(sql, parameterValue);
                    }
                    catch
                    {
                        sql = string.Format(sql, defaultValue);
                    }
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                value[0] = ds.Tables[0].Rows[0]["MinDate"].ToString();
                value[1] = ds.Tables[0].Rows[0]["MaxDate"].ToString();
            }

            return value;
        }
        #endregion

        #region Send Email Notification to Checker/Approver
        public static void sendNotification( string transactionType
                                           , string empId
                                           , string empName
                                           , string status
                                           , DataRow drDetails)
        {
            DataSet ds = new DataSet();
            ds = getToRecepient(empId, status, transactionType);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0]["Email"].ToString().Equals(string.Empty))
            {
                string messageFrom = fromParameter;
                string messageTo = ds.Tables[0].Rows[0]["Email"].ToString();
                string messageSubject = subjectParameter;
                string messageBody = string.Empty;
                string greetings = getGreeting(); 
                messageBody = greetings + ds.Tables[0].Rows[0]["Name"].ToString() + ",";
                #region [Message Body]
                messageBody += @"

This is a system generated email, please do not respond to this email.
Sending you information regading your employees' transactions for {0}.
This transaction is for your checking / approval.

{1} has filled for {0} request.
Information of transaction:{2}

Logon to workflow at http://{5}?dl={3}.

Current quincena is {4}.

Thank you.

Respectfully yours,
    NPAX HRC Support";
                #endregion
                string transactionName = string.Empty;
                string transactionDetail = string.Empty;
                string currentQuincena = string.Empty;
                switch (transactionType)
                {
                    case "OVERTIME":
                        #region
                        transactionName = "OVERTIME";
                        transactionDetail = @"
    Control Number: " + drDetails["Eot_ControlNo"].ToString()
+ @"
    Overtime Date:  " + Convert.ToDateTime(drDetails["Eot_OvertimeDate"].ToString()).ToString("MM/dd/yyyy")
+ @"
    Type:           " + drDetails["Eot_OvertimeType"].ToString()
+ @"
    Start:          " + drDetails["Eot_StartTime"].ToString().Insert(2, ":")
+ @"
    End:            " + drDetails["Eot_EndTime"].ToString().Insert(2, ":")
+ @"
    Hour(s):        " + drDetails["Eot_OvertimeHour"].ToString()
+ @"
    Reason:         " + drDetails["Eot_Reason"].ToString();

                        messageBody = string.Format(messageBody
                                                   , transactionName
                                                   , empName
                                                   , transactionDetail
                                                   , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                   , drDetails["Eot_CurrentPayPeriod"].ToString()
                                                   , urlParameter);
                        #endregion
                        break;
                    case "LEAVE":
                        #region
                        transactionName = "LEAVE";
                        transactionDetail = @"
    Control Number: " + drDetails["Elt_ControlNo"].ToString()
+ @"
    Leave Date:     " + Convert.ToDateTime(drDetails["Elt_LeaveDate"].ToString()).ToString("MM/dd/yyyy")
+ @"
    Type:           " + drDetails["Elt_LeaveType"].ToString()
+ @"
    Category:       " + drDetails["Elt_LeaveCategory"].ToString()
+ @"
    Start:          " + drDetails["Elt_StartTime"].ToString().Insert(2, ":")
+ @"
    End:            " + drDetails["Elt_EndTime"].ToString().Insert(2, ":")
+ @"
    Reason:         " + drDetails["Elt_Reason"].ToString();

                        messageBody = string.Format(messageBody
                                                   , transactionName
                                                   , empName
                                                   , transactionDetail
                                                   , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                   , drDetails["Elt_CurrentPayPeriod"].ToString()
                                                   , urlParameter);
                        #endregion
                        break;
                    case "TIMEMOD":
                        #region
                        transactionName = "TIME MODIFICATION";
                        transactionDetail = @"
    Control Number: " + drDetails["Trm_ControlNo"].ToString()
+ @"
    Modify Date:    " + Convert.ToDateTime(drDetails["Trm_ModDate"].ToString()).ToString("MM/dd/yyyy")
+ @"
    Type:           " + drDetails["Trm_Type"].ToString();
                        try
                        {
                            transactionDetail += @"
    Actual IN 1:    " + drDetails["Trm_ActualTimeIn1"].ToString().Insert(2, ":");
                        }
                        catch
                        {
                            transactionDetail += @"
    Actual IN 1:    ";
                        }
                        try
                        {
                            transactionDetail += @"
    Actual OUT 1:   " + drDetails["Trm_ActualTimeOut1"].ToString().Insert(2, ":");
                        }
                        catch
                        {
                            transactionDetail += @"
    Actual OUT 1:    ";
                        }
                        try
                        {
                            transactionDetail += @"
    Actual IN 2:    " + drDetails["Trm_ActualTimeIn2"].ToString().Insert(2, ":");
                        }
                        catch
                        {
                            transactionDetail += @"
    Actual IN 2:    ";
                        }
                        try
                        {
                            transactionDetail += @"
    Actual OUT 2:   " + drDetails["Trm_ActualTimeOut2"].ToString().Insert(2, ":");
                        }
                        catch
                        {
                            transactionDetail += @"
    Actual OUT 2:    ";
                        }
                        transactionDetail += @"
    Reason:         " + drDetails["Trm_Reason"].ToString();

                        messageBody = string.Format(messageBody
                                                   , transactionName
                                                   , empName
                                                   , transactionDetail
                                                   , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                   , drDetails["Trm_CurrentPayPeriod"].ToString()
                                                   , urlParameter);
                        #endregion
                        break;
                    default:
                        break;
                }
                //Send email
                SendEmail(messageFrom
                         , messageTo
                         , messageSubject
                         , messageBody);
            }
        }

        public static void sendNotification(string transactionType
                                           , string empId
                                           , string empName
                                           , string status
                                           , string messageDetail)
        { 
            DataSet ds = new DataSet();
            ds = getToRecepient(empId, status, transactionType);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0]["Email"].ToString().Equals(string.Empty))
            {
                string messageFrom = fromParameter;
                string messageTo = ds.Tables[0].Rows[0]["Email"].ToString();
                string messageSubject = subjectParameter;
                string greetings = getGreeting();
                string messageBody = string.Empty;
                messageBody = greetings + ds.Tables[0].Rows[0]["Name"].ToString() + ",";
                #region [Message Body]
                messageBody += @"

This is a system generated email, please do not respond to this email.
Sending you information regading your employees' transactions for {0}.
This transaction is for your checking / approval.

{1} has filled for {0} request.
Information of transaction(s):{2}

Logon to workflow at http://{5}?dl={3}.

Current quincena is {4}.

Thank you.

Respectfully yours,
    NPAX HRC Support";
                #endregion
                string transactionName = string.Empty;
                string transactionDetail = string.Empty;
                string currentQuincena = string.Empty;

                transactionName = "LEAVE";
                transactionDetail = messageDetail;

                messageBody = string.Format(messageBody
                                           , transactionName
                                           , empName
                                           , transactionDetail
                                           , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                           , getPayPeriod("C")
                                           , urlParameter);
                //Send email
                SendEmail(messageFrom
                         , messageTo
                         , messageSubject
                         , messageBody);
            }
        }

        public static string getGreeting()
        {
            string value = "Good Day!";
            if (DateTime.Now.Hour < 12)
            {
                value = "Good Morning ";
            }
            else if (DateTime.Now.Hour < 17)
            {
                value = "Good Afternoon ";
            }
            else
            {
                value = "Good Evening ";
            }
            return value;
        }

        public static DataSet getToRecepient(string userId, string status, string transactionType)
        {
            string sql = @" SELECT Umt_UserCode [UserCode]
                                ,  Umt_UserFname + ' ' + Umt_UserLname[Name]
                                ,  CASE WHEN (ISNULL(Unt_NotifyFlag,'0') = '0')
		                                THEN ''
		                                ELSE CASE WHEN (Umt_EmailAdd = '')
					                              THEN Emt_EmailAddress
					                              ELSE Umt_EmailAdd
				                              END 
		                            END [Email] 
                              FROM T_EmployeeApprovalRoute EA
                             INNER JOIN T_ApprovalRouteMaster AR
                                ON AR.Arm_RouteId = EA.Arm_RouteId
                             INNER JOIN T_UserMaster
                                ON Umt_UserCode = CASE WHEN (@Status = '3')
						                               THEN CASE WHEN  ( Arm_Checker1 = Arm_Checker2 
										                             AND Arm_Checker1 = Arm_Approver)
									                             THEN Arm_Approver
									                             ELSE CASE WHEN (Arm_Checker1 = Arm_Checker2)
											                               THEN Arm_Checker2
									                                       ELSE Arm_Checker1
									                                   END
							                                 END
							                            ELSE CASE WHEN (@Status = '5')
									                              THEN CASE WHEN ( Arm_Checker2 = Arm_Approver)
											                                THEN Arm_Approver
											                                ELSE Arm_Checker2
										                                END
									                              ELSE Arm_Approver
							                                 END
					                               END
                              LEFT JOIN T_EMployeeMaster
                                ON Emt_EmployeeId = Umt_UserCode
                              LEFT JOIN T_UserNotification
                                ON Unt_UserCode = Umt_UserCode
                               AND Unt_TransactionType = @transactionType
                             WHERE EA.Arm_EmployeeId = @UserCode
                               AND EA.Arm_TransactionId = @transactionType
                               AND Convert(varchar,GETDATE(),101) BETWEEN EA.Arm_StartDate AND ISNULL(EA.Arm_EndDate, GETDATE())";
            DataSet ds = new DataSet();
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@UserCode", userId);
            param[1] = new ParameterInfo("@Status", status);
            param[2] = new ParameterInfo("@transactionType", transactionType);

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
            return ds;
        }

        public static void SendEmail(string mailFrom, string mailTo, string mailSubject, string mailMsg)
        {
            try
            {
                MailMessage mail = new MailMessage(mailFrom, mailTo);
                mail.Subject = mailSubject;
                mail.Body = mailMsg;
                //mail.Bcc.Add("apsungahid@n-pax.com");
                //mail.CC.Add("mmiglesia@n-pax.com");
                SmtpClient mailClient = new SmtpClient(smtpSevrer);
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                mailClient.Send(mail);
            }
            catch
            {

            }
        }
        #endregion

        #region Get Filler Column Names
        public static string getFillerName(string fillerName)
        {

            string sql = string.Format(@"
                                SELECT CASE WHEN ISNULL(Cfl_TextDisplay, Cfl_ColName) = ''
			                                    THEN Cfl_ColName
			                                    ELSE Cfl_TextDisplay
	                                          END
	                                     FROM T_ColumnFiller
                                        WHERE Cfl_ColName = '{0}'
                                        --and Cfl_Status='A'", fillerName);
            string flag = string.Empty;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = dal.ExecuteScalar(sql, CommandType.Text).ToString();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            TextInfo UsaTextInfo = new CultureInfo("en-US", false).TextInfo;
            return UsaTextInfo.ToTitleCase(flag.ToLower());
        }

        public static string getFillerStatus(string fillerName)
        {

            string sql = string.Format(@"
                                SELECT Cfl_Status
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
                catch
                {
                    flag = "C";
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return flag != null ? flag.ToString().ToUpper() : string.Empty;
        }
        #endregion

		#region Get minute value of time
        public static int getMinuteValue(string hhmm)
        {
            int retValue = 0;
            int hour = 0;
            int minute = 0;

            hour = Convert.ToInt32(hhmm.Substring(0, 2));
            minute = Convert.ToInt32(hhmm.Substring(2, 2));

            retValue = (hour * 60) + minute;

            return retValue;
        }
        #endregion

        #region Get Employee Payroll Type
        public static string getEmployeePayrollType(string employeeId)
        {
            string retValue = "";
            string sql = string.Format(@"  SELECT Emt_PayrollType
                                             FROM T_EmployeeMaster
                                            WHERE Emt_EmployeeId = '{0}'", employeeId);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    retValue = dal.ExecuteScalar(sql, CommandType.Text).ToString();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retValue;
        }
        #endregion

        #region Email Notification Methods/Functions
        public static void sendGenericNotificationEmail(string empId
                                                       , string empName
                                                       , string status
                                                       , string transactionType
                                                       , DataRow drDetails)
        {
            DataSet ds = new DataSet();
            CommonMethods cm = new CommonMethods();

            if (cm.GetProcessControlFlag("GENERAL", "WFNOTIFY"))
            {
                ds = getToRecepient(empId, status, transactionType);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0]["Email"].ToString().Equals(string.Empty))
                {
                    string messageFrom = "noreply-workflow@n-pax.com";
                    string messageTo = ds.Tables[0].Rows[0]["Email"].ToString();
                    string messageSubject = "Workflow Notification";
                    string messageBody = string.Empty;
                    string greetings = getGreeting();

                    messageBody = greetings + ds.Tables[0].Rows[0]["Name"].ToString() + ",";
                    #region [Message Body]
                    messageBody += @"

This is a system generated email, please do not respond to this email.
Sending you information regading your employees' transactions for {0}.
This transaction is for your checking / approval.

{1} has filled for {0} request.
Information of transaction:{2}

Logon to workflow at {5}?dl={3}.

Current quincena is {4}.

Thank you.

Respectfully yours,
    NPAX HRC Support";
                    #endregion
                    string transactionName = string.Empty;
                    string transactionDetail = string.Empty;
                    string currentQuincena = string.Empty;
                    switch (transactionType)
                    {
                        case "OVERTIME":
                            #region
                            transactionName = "OVERTIME";
                            transactionDetail = @"
    Control Number: " + drDetails["Eot_ControlNo"].ToString()
    + @"
    Overtime Date:  " + Convert.ToDateTime(drDetails["Eot_OvertimeDate"].ToString()).ToString("MM/dd/yyyy")
    + @"
    Type:           " + drDetails["Eot_OvertimeType"].ToString()
    + @"
    Start:          " + drDetails["Eot_StartTime"].ToString().Insert(2, ":")
    + @"
    End:            " + drDetails["Eot_EndTime"].ToString().Insert(2, ":")
    + @"
    Hour(s):        " + drDetails["Eot_OvertimeHour"].ToString()
    + @"
    Reason:         " + drDetails["Eot_Reason"].ToString();

                            messageBody = string.Format(messageBody
                                                       , transactionName
                                                       , empName
                                                       , transactionDetail
                                                       , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                       , drDetails["Eot_CurrentPayPeriod"].ToString()
                                                       , ConfigurationManager.AppSettings["workflowAddress"].ToString());
                            #endregion
                            break;
                        case "LEAVE":
                            #region
                            transactionName = "LEAVE";
                            transactionDetail = @"
    Control Number: " + drDetails["Elt_ControlNo"].ToString()
    + @"
    Leave Date:     " + Convert.ToDateTime(drDetails["Elt_LeaveDate"].ToString()).ToString("MM/dd/yyyy")
    + @"
    Type:           " + drDetails["Elt_LeaveType"].ToString()
    + @"
    Category:       " + drDetails["Elt_LeaveCategory"].ToString()
    + @"
    Start:          " + drDetails["Elt_StartTime"].ToString().Insert(2, ":")
    + @"
    End:            " + drDetails["Elt_EndTime"].ToString().Insert(2, ":")
    + @"
    Reason:         " + drDetails["Elt_Reason"].ToString();

                            messageBody = string.Format(messageBody
                                                       , transactionName
                                                       , empName
                                                       , transactionDetail
                                                       , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                       , drDetails["Elt_CurrentPayPeriod"].ToString()
                                                       , ConfigurationManager.AppSettings["workflowAddress"].ToString());
                            #endregion
                            break;
                        case "TIMEMOD":
                            #region
                            transactionName = "TIME MODIFICATION";
                            transactionDetail = @"
    Control Number: " + drDetails["Trm_ControlNo"].ToString()
    + @"
    Modify Date:    " + Convert.ToDateTime(drDetails["Trm_ModDate"].ToString()).ToString("MM/dd/yyyy")
    + @"
    Type:           " + drDetails["Trm_Type"].ToString();
                            try
                            {
                                transactionDetail += @"
    Actual IN 1:    " + drDetails["Trm_ActualTimeIn1"].ToString().Insert(2, ":");
                            }
                            catch
                            {
                                transactionDetail += @"
    Actual IN 1:    ";
                            }
                            try
                            {
                                transactionDetail += @"
    Actual OUT 1:   " + drDetails["Trm_ActualTimeOut1"].ToString().Insert(2, ":");
                            }
                            catch
                            {
                                transactionDetail += @"
    Actual OUT 1:    ";
                            }
                            try
                            {
                                transactionDetail += @"
    Actual IN 2:    " + drDetails["Trm_ActualTimeIn2"].ToString().Insert(2, ":");
                            }
                            catch
                            {
                                transactionDetail += @"
    Actual IN 2:    ";
                            }
                            try
                            {
                                transactionDetail += @"
    Actual OUT 2:   " + drDetails["Trm_ActualTimeOut2"].ToString().Insert(2, ":");
                            }
                            catch
                            {
                                transactionDetail += @"
    Actual OUT 2:    ";
                            }
                            transactionDetail += @"
    Reason:         " + drDetails["Trm_Reason"].ToString();

                            messageBody = string.Format(messageBody
                                                       , transactionName
                                                       , empName
                                                       , transactionDetail
                                                       , ds.Tables[0].Rows[0]["UserCode"].ToString()
                                                       , drDetails["Trm_CurrentPayPeriod"].ToString()
                                                       , ConfigurationManager.AppSettings["workflowAddress"].ToString());
                            #endregion
                            break;
                        default:
                            break;
                    }
                    //Send email
                    SendEmailNotification(messageFrom
                             , messageTo
                             , messageSubject
                             , messageBody);
                }
            }
        }

        public static void SendEmailNotification(string mailFrom, string mailTo, string mailSubject, string mailMsg)
        {
            try
            {
                string addBcc = string.Empty;
                string addCC = string.Empty;
                string smtpClient = string.Empty;
                string credentialUser = string.Empty;
                string credetialPswd = string.Empty;

                addBcc = ConfigurationManager.AppSettings["notificationBCC"].ToString();
                addCC = ConfigurationManager.AppSettings["notificationCC"].ToString();
                smtpClient = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                credentialUser = ConfigurationManager.AppSettings["SMTPUsername"].ToString();
                credetialPswd = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

                MailMessage mail = new MailMessage(mailFrom, mailTo);
                mail.Subject = mailSubject;
                mail.Body = mailMsg;
                if (!addBcc.Equals(string.Empty))
                    mail.Bcc.Add(addBcc);
                if (!addCC.Equals(string.Empty))
                    mail.CC.Add(addCC);
                if (!smtpClient.Equals(string.Empty))
                {
                    if (!credentialUser.Equals(string.Empty) && !credetialPswd.Equals(string.Empty))
                    {
                        SmtpClient mailClient = new SmtpClient(smtpClient);
                        mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailClient.Credentials = new NetworkCredential(credentialUser, credetialPswd);
                        mailClient.Send(mail);
                    }
                    else
                    {
                        //No valid netwrok credential defined in WEB.CONFIG
                    }
                }
                else
                {
                    //No SMTP Client defined in WEB.CONFIG
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Error Logs
        public static void ErrorsToTextFile(Exception er, string user)
        {
            try
            {
                DateTime dt = DateTime.Now;
                StreamWriter writer;
                string websitePath = ConfigurationManager.AppSettings["ErrorLogFolder"].ToString();
                if (websitePath.Equals(string.Empty))
                {
                    websitePath = "C:\\";
                }
                websitePath = websitePath + "\\" + dt.Year.ToString();
                if (!Directory.Exists(websitePath))
                {
                    Directory.CreateDirectory(websitePath);
                }
                websitePath = websitePath + "\\" + dt.ToString("MMMM") + "_ErrorLog.txt";
                if (!File.Exists(websitePath))
                {
                    writer = File.CreateText(websitePath);
                    writer.WriteLine("Error Text File Created on: " + dt.ToString("MMMM dd, yyyy HH:mm"));
                    writer.WriteLine("Errors:");
                }
                else
                {
                    writer = File.AppendText(websitePath);
                }

                writer.WriteLine("\n");
                writer.WriteLine(dt.ToString("MM/dd/yyyy HH:mm") + " BY: " + user);
                writer.WriteLine("  Error     :" + er.Message);
                writer.WriteLine("  Source    :" + er.StackTrace);
                writer.WriteLine("\n");
                writer.WriteLine("-------------------------------------------------------------------------------");
                writer.Close();
            }
            catch
            { 
            
            }
        }
        #endregion

        #region Get Minimum Overtime Hours
        public static string getMINOTHR(string employeeId)
        {
            string value = string.Empty;
            string sql = @"  declare @defaultValue as decimal(8,2)
                             SET @defaultValue = (SELECT Pmt_NumericValue
                                                    FROM T_ParameterMaster
                                                   WHERE Pmt_ParameterId = 'MINOTHR')
                                
                          SELECT ISNULL(Convert(varchar(10),Pmx_ParameterValue),@defaultValue)
                            FROM T_EmployeeMaster
                            LEFT JOIN T_ParameterMasterExt
                              ON Pmx_Classification = Emt_JobLevel
                             AND Pmx_ParameterId = 'MINOTHR'
                           WHERE Emt_EmployeeID = @EmployeeId";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@EmployeeId", employeeId);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text, param));
                }
                catch (Exception ex)
                {
                    CommonMethods cm = new CommonMethods();
                    CommonMethods.ErrorsToTextFile(ex, cm.session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return value;

        }
        #endregion

        #region Get Process Control Flag
        public bool GetProcessControlFlag(string systemId, string processId)
        {
            bool flag = false;
            string sqlGetFlag = @"  SELECT Pcm_Processflag
                                      FROM T_ProcessControlMaster
                                     WHERE Pcm_SystemId = '{0}' AND Pcm_ProcessId = '{1}' ";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = Convert.ToBoolean(dal.ExecuteScalar(string.Format(sqlGetFlag, systemId, processId), CommandType.Text));
                }
                catch (Exception ex)
                {
                    ErrorsToTextFile(ex, "SA");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return flag;
        }
        #endregion

        #region isAffectedByCutoff
        public static bool isAffectedByCutoff(string SystemID, string MMddyyyy)
        {
            return isAffectedByCutoff(SystemID, MMddyyyy, MMddyyyy);
        }

        public static bool isAffectedByCutoff(string SystemID, string FROMMMddyyyy, string TOMMddyyyy)
        {
            bool flag = false;
            #region SQL
            string sqlCheck = @"    DECLARE @START as varchar(10)
                                    DECLARE @END as varchar(10)
                                    DECLARE @SYSTEMID as varchar(10)

                                    SET @SYSTEMID = '{0}'
                                    SET @START = '{1}'
                                    SET @END = '{2}'

                                    SELECT CASE WHEN (Pcm_ProcessFlag = 1)
			                                    THEN CASE WHEN @START >= Ppm_StartCycle AND @START <= Ppm_EndCycle
					                                      THEN 'TRUE'
					                                      ELSE CASE WHEN @END >= Ppm_StartCycle AND @END <= Ppm_EndCycle
								                                    THEN 'TRUE'
								                                    ELSE CASE WHEN @START <= Ppm_StartCycle AND @END >= Ppm_EndCycle
										                                      THEN 'TRUE'
										                                      ELSE CASE WHEN @START < Ppm_StartCycle OR @END < Ppm_StartCycle
												                                        THEN 'TRUE'
												                                        ELSE 'FALSE'
												                                    END
									                                      END
							                                    END		
				                                      END
			                                    ELSE 'FALSE'
	                                        END
                                      FROM T_PayPeriodMaster
                                     INNER JOIN T_ProcessControlMaster
                                        ON Pcm_SystemID = @SYSTEMID
                                       AND Pcm_ProcessID = 'CUT-OFF'
                                     WHERE Ppm_CycleIndicator = 'C' ";

            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                sqlCheck = @"       DECLARE @START as varchar(10)
                                    DECLARE @END as varchar(10)
                                    DECLARE @SYSTEMID as varchar(10)

                                    SET @SYSTEMID = '{0}'
                                    SET @START = '{1}'
                                    SET @END = '{2}'

                                    SELECT CASE WHEN (Pcm_ProcessFlag = 1)
			                                    THEN CASE WHEN @START >= Ppm_StartCycle AND @START <= Ppm_EndCycle
					                                      THEN 'FALSE'
					                                      ELSE CASE WHEN @END >= Ppm_StartCycle AND @END <= Ppm_EndCycle
								                                    THEN 'FALSE'
								                                    ELSE CASE WHEN @START < Ppm_StartCycle AND @END >= Ppm_EndCycle
										                                      THEN 'TRUE'
										                                      ELSE CASE WHEN @START < Ppm_StartCycle OR @END < Ppm_StartCycle
												                                        THEN 'TRUE'
												                                        ELSE 'FALSE'
												                                    END
									                                      END
							                                    END		
				                                      END
			                                    ELSE 'FALSE'
	                                        END
                                      FROM T_PayPeriodMaster
                                     INNER JOIN T_ProcessControlMaster
                                        ON Pcm_SystemID = @SYSTEMID
                                       AND Pcm_ProcessID = 'CUT-OFF'
                                     WHERE Ppm_CycleIndicator = 'C' ";
            }
            #endregion
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = Convert.ToBoolean(dal.ExecuteScalar(string.Format(sqlCheck, SystemID, FROMMMddyyyy, TOMMddyyyy), CommandType.Text));
                }
                catch(Exception ex)
                {
                    ErrorsToTextFile(ex, "SA");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return flag;    
        }
        #endregion

        public static string roundupTime(string hhmm, int interval)
        {
            string retVal = string.Empty;
            try
            {
                DateTime dt = new DateTime(DateTime.Now.Year
                                          , DateTime.Now.Month
                                          , DateTime.Now.Day
                                          , Convert.ToInt32(hhmm.Substring(0, 2))
                                          , Convert.ToInt32(hhmm.Substring(2, 2))
                                          , 0
                                          , DateTimeKind.Unspecified);

                DateTime final = new DateTime(((dt.Ticks + TimeSpan.FromMinutes(interval).Ticks - 1) / TimeSpan.FromMinutes(interval).Ticks) * TimeSpan.FromMinutes(interval).Ticks);
                retVal = final.TimeOfDay.ToString().Substring(0,5);
            }
            catch (Exception ex)
            {
                retVal = string.Empty;
                ErrorsToTextFile(ex, "roundupTime");
            }

            return retVal;
        }
        //20100909 : Old methods below
        public CommonMethods(string SmtpServer)
        {

        }

        #region Fetch Current Quincena Column [q is current or future] and [i is start or end]
        public DateTime FetchQuincena(char q,char i)
        {
            DataSet dsQuincena = new DataSet();
            string sqlGetCQuincena = @" Select Ppm_StartCycle, Ppm_EndCycle From t_PayPeriodMaster
                                                                      Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A'

                                                     
                                                    ";
            string sqlGetFQuincena = @" DECLARE @EndDate as Datetime

                                                        Set @EndDate = (Select Ppm_EndCycle From t_PayPeriodMaster
                                                                      Where Ppm_CycleIndicator = 'C' and Ppm_Status = 'A')

                                                        Select Ppm_StartCycle, Ppm_EndCycle
                                                        from t_PayPeriodMaster
                                                        Where Ppm_StartCycle = dateadd(dd,1,@EndDate) and Ppm_Status = 'A'
                                                    ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    if (q == 'c')
                    {
                        dsQuincena = dal.ExecuteDataSet(sqlGetCQuincena, CommandType.Text);
                    }
                    else if (q == 'f')
                    {
                        dsQuincena = dal.ExecuteDataSet(sqlGetFQuincena, CommandType.Text);
                    }
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
            try
            {
                switch (i)
                {
                    case 's':
                        return Convert.ToDateTime(dsQuincena.Tables[0].Rows[0][0]);
                    case 'e':
                        return Convert.ToDateTime(dsQuincena.Tables[0].Rows[0][1]);
                    default:
                        return DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                message = "Error: No Quincena was retrieved. Contact Technical Support.";
                MessageBox.Show(message);
                return DateTime.Today;
            }

        }
        #endregion

        #region
        public bool isInTransactionRemarks(string controlNumber)
        {
            string check = "";
            string sql = @" SELECT Trm_ControlNo
                            FROM T_TransactionRemarks
                            WHERE Trm_ControlNo = '{0}'";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    check = Convert.ToString(dal.ExecuteScalar(string.Format(sql, controlNumber), CommandType.Text));
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
            return (check != "");
        }
        #endregion

        #region Insert in TransactionRemarks
        public void insertRemarks(string cn, string remarks, string id)
        {
            string sqlInsertRemarks = @"INSERT INTO T_TransactionRemarks
                                        (
                                            Trm_ControlNo,
                                            Trm_Remarks,
                                            Usr_Login,
                                            Ludatetime
                                        )
                                        VALUES
                                        (
                                            '{0}', '{1}', '{2}', getdate()
                                        )";

            string sqlUpdateRemarks = @"UPDATE T_TransactionRemarks
                                        SET
                                            Trm_Remarks = '{0}',
                                            Usr_Login = '{1}',
                                            Ludatetime = getdate()
                                        
                                        WHERE
                                        Trm_ControlNo = '{2}'";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    if (isInTransactionRemarks(cn))
                    {
                        dal.ExecuteNonQuery(string.Format(sqlUpdateRemarks, remarks, id, cn), CommandType.Text);
                    }
                    else 
                    {
                        dal.ExecuteNonQuery(string.Format(sqlInsertRemarks, cn, remarks, id), CommandType.Text);
                    }

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
        }
        #endregion

        #region For Export
        public static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }
        #endregion

        #region For Overtime Current Pay Period CPH 

        public string GetCurrentPayPeriodCPH(string mmddYYYY)
        {
            string payperiod = string.Empty;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    payperiod = dal.ExecuteScalar(string.Format(@"
                            DECLARE @TRANSACTIONDATE AS VARCHAR(20)
                            SET @TRANSACTIONDATE = '{0}'
                            
                            SELECT 
								CASE WHEN @TRANSACTIONDATE <= PREV.Ppm_EndCycle
									THEN 
										CUR.Ppm_PayPeriod
									ELSE 
										ISNULL(FUTR.Ppm_PayPeriod, CUR.Ppm_PayPeriod)
								END [Ppm_PayPeriod]
							FROM T_PayPeriodMaster CUR
							INNER JOIN T_PayPeriodMaster PREV
								ON PREV.Ppm_EndCycle = dateadd(day, -1, CUR.Ppm_StartCycle)
								AND PREV.Ppm_CycleIndicator <> 'S'							
							LEFT JOIN T_PayPeriodMaster FUTR
								ON FUTR.Ppm_StartCycle = dateadd(day, 1, CUR.Ppm_EndCycle)
								AND FUTR.Ppm_CycleIndicator <> 'S'	
							WHERE CUR.Ppm_CycleIndicator = 'C'
							
                    ", mmddYYYY), CommandType.Text).ToString().Trim();
                }
                catch(Exception ex)
                {
                    payperiod = string.Empty;
                    ErrorsToTextFile(ex, "GetCurrentPayPeriodCPH(" + mmddYYYY + ")");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return payperiod;
        }

        #endregion

        public static bool isMismatchCostcenterAMS(DALHelper dal, string employeeID, string MMddyyyy)
        {
            string sql = @" DECLARE @Date as datetime = '{0}'
                            DECLARE @CHECK as varchar(5)
                            DECLARE @CTRLINEREF as bit  = (  SELECT Pcm_ProcessFlag 
								                               FROM T_ProcessControlMaster
								                              WHERE Pcm_SystemID = 'GENERAL'
									                            AND Pcm_ProcessID = 'CTRLINEREF')
                            SELECT @CHECK = IIF( ISNULL(ECM1.Ecm_CostCenterCode, Emt_CostCenterCode) 
                                              <> IIF(Clm_CostCenterCode IS NOT NULL, ISNULL(ECM2.Ecm_CostCenterCode, ''), ISNULL(ECM1.Ecm_CostCenterCode, Emt_CostCenterCode)), 'TRUE', 'FALSE')
                              FROM T_EmployeeMaster
                              LEFT JOIN T_EmployeeCostCenterMovement [ECM1]
                                ON ECM1.Ecm_EmployeeID = Emt_EmployeeID
                               AND @Date BETWEEN ECM1.Ecm_StartDate AND ISNULL(ECM1.Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                              LEFT JOIN E_EmployeeCostCenterLineMovement [ECM2]
                                ON ECM2.Ecm_EmployeeID = Emt_EmployeeID
                               AND @Date BETWEEN ECM2.Ecm_StartDate AND ISNULL(ECM2.Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                               AND Ecm_Status = 'A'
                              LEFT JOIN ( SELECT DISTINCT Clm_CostCenterCode
                                            FROM E_CostCenterLineMaster
			                               WHERE Clm_Status = 'A' ) AS TEMP
                                ON Clm_CostCenterCode = ECM1.Ecm_CostCenterCode
                             WHERE Emt_EmployeeID = '{1}'

                             IF(@CHECK IS NULL)
                             BEGIN 
	                            SET @CHECK = 'FALSE'
                             END

                            SELECT IIF(ISNULL(@CTRLINEREF, 0) = 1, @CHECK,  'FALSE')  ";

            DateTime parseDate = new DateTime();
            if (!DateTime.TryParse(MMddyyyy, out parseDate))
            {
                throw new Exception("Unable to parse date parameter.");
            }

            return Convert.ToBoolean(dal.ExecuteScalar(string.Format(sql, MMddyyyy, employeeID), CommandType.Text).ToString());
        }

        public static DataSet GetDefaultEmployee(string userLogged, string systemID)
        {
            bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
            DataSet ds = new DataSet();

            #region Default Lookup
//            string sql = @"
//
//                          WITH TempTable AS (
//                        SELECT Row_Number() OVER (Order by [ID No]) [Row], *
//                          FROM ( SELECT Emt_EmployeeId [ID No]
//                                      , Emt_Lastname + ', ' + Emt_Firstname [Name]
//                                      , Emt_NickName [Nickname]
//                                      , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Costcenter]
//                                      , ISNULL(Dcm_Departmentdesc, '') [Department]
//                                   FROM T_EmployeeMaster
//                                   LEFT JOIN T_DepartmentCodeMaster
//	                                 ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
//								                                  THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
//								                                  ELSE ''
//							                                  END
//
//                                     {2}
//                                  WHERE LEFT(Emt_JobStatus,1) = 'A'
//                                  {0}
//                              ) AS temp)
//                            {1}
//                           FROM TempTable
//
//                         SELECT COUNT(Emt_EmployeeId) [Rows]
//                           FROM T_EMployeeMaster
//
//                                     {2}
//                          WHERE LEFT(Emt_JobStatus,1) = 'A'
//                            {0} ";

//            string sqlSelect = @" SELECT [ID No] 
//                                   , [Name]
//                                   , [Nickname]
//                                   , [Costcenter]";
//            switch (Resources.Resource._3RDINFO.ToString().ToUpper())
//            {
//                case "COSTCENTER":
//                    sqlSelect = @" SELECT [ID No] 
//                                    , [Name]
//                                    , [Costcenter]
//                                    , [Nickname]";
//                    break;
//                case "NICKNAME":
//                    sqlSelect = @" SELECT [ID No] 
//                                    , [Name]
//                                    , [Nickname]
//                                    , [Costcenter]";
//                    break;
//                case "DEPARTMENT":
//                    sqlSelect = @" SELECT [ID No] 
//                                    , [Name]
//	                                , [Department]
//                                    , [Costcenter]";
//                    break;
//                default:
//                    break;
//            }

//            sql = string.Format(sql, filterQuery(), sqlSelect, (!hasCCLine ? "" : @"---apsungahid added for line code access filter 20141211
//                                                                                                                             LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
//									                                                                                                      FROM E_CostCenterLineMaster 
//												                                                                                         WHERE Clm_Status = 'A' ) AS HASLINE
//										                                                                                       ON Clm_CostCenterCode = Emt_CostcenterCode
//
//
//					                                                                                                          LEFT JOIN E_EmployeeCostCenterLineMovement
//					                                                                                                            ON Ecm_EmployeeID = Emt_EmployeeID
//					                                                                                                           AND GETDATE() BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))"));
//            string transactionType = systemID;
//            ParameterInfo[] param = new ParameterInfo[2];
//            param[0] = new ParameterInfo("@UserCode", userLogged);
//            param[1] = new ParameterInfo("@TransactionType", transactionType);

//            using (DALHelper dal = new DALHelper())
//            {
//                try
//                {
//                    dal.OpenDB();
//                    ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
//                }
//                catch (Exception ex)
//                {
//                    CommonMethods.ErrorsToTextFile(ex, userLogged);
//                }
//                finally
//                {
//                    dal.CloseDB();
//                }
//            }
            #endregion

            return ds;
        }
    }

}
