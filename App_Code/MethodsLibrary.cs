/* File revision no. W2.1.00002
 * 
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :  05/22/2013
 *  Update Notes    :   
 *      -   change query and add checking to insertlogledgertrail
 *      -> go to "robert05222013"
 * 
 */
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;

/// <summary>
/// Summary description for MethodsLibrary
/// </summary>

namespace MethodsLibrary
{
    public class Methods
    {
        public Methods()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region Gets the next status to be assigned to transactions for button X
        public char GetStatusX(string buttonName, string empID, string transactionType)
        {
            char retStatus = ' ';
            bool flag = true;
            string[] routeMembers = new string[3];
            ////////Legend for Transaction Type//////////////////
            //  01 - Overtime                                  //
            //  02 - Leave                                     //
            //  03 - Job Split                                 //
            //  04 - Time Modification                         //
            //  05 - Flextime                                  //
            //  06 - Movement                                  //
            /////////////////////////////////////////////////////
            try
            {
                routeMembers = GetEmployeeRoute(empID, transactionType);
            }
            catch
            {
                flag = false;
                MessageBox.Show("No routes setup for this employee for this transaction.");
            }

            if (flag)
            {
                string c1 = routeMembers[0];
                string c2 = routeMembers[1];
                string ap = routeMembers[2];
                //Switch for btnX
                switch (buttonName.Trim().ToUpper())
                {
                    case "ENDORSE TO CHECKER 1":
                        if (c1 == c2 && c2 == ap)
                        {
                            retStatus = '7';
                        }
                        else if (c1 == c2)
                        {
                            retStatus = '5';
                        }
                        else
                        {
                            retStatus = '3';
                        }
                        break;
                    case "ENDORSE TO CHECKER 2":
                        if (c2 == ap)
                        {
                            retStatus = '7';
                        }
                        else
                        {
                            retStatus = '5';
                        }
                        break;
                    case "ENDORSE TO APPROVER":
                        retStatus = '7';
                        break;
                    case "APPROVE":
                        retStatus = '9';
                        break;
                    default:
                        break;
                }
            }

            return retStatus;
        }
        #endregion

        #region Gets the next status to be assigned to transactions for button Y
        public char GetStatusY(string status)
        {
            char retStatus = ' ';
            
            //Switch for btnY
            switch (status.Trim().ToUpper())
            { 
                case "ENDORSED TO CHECKER 1":
                    retStatus = '4';
                    break;
                case "ENDORSED TO CHECKER 2":
                    retStatus = '6';
                    break;
                case "ENDORSED TO APPROVER":
                    retStatus = '8';
                    break;
                default:
                    break;
            }
            return retStatus;
        }
        #endregion

        #region Gets the next status to be assigned to transactions for button Z
        public char GetStatusZ(string status)
        {
            return '1';
        }
        #endregion

        #region Gets the value of the ProcessID from the ParameterMaster
        public decimal GetParameter(string processId)
        {
            decimal value = 0;
            string sql = @"  SELECT 
                           ISNUll(Pmt_Numericvalue, 0)
                        FROM 
                            T_ParameterMaster
                        WHERE 
                            Pmt_ParameterId = '{0}' AND Pmt_status ='A'";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = Convert.ToDecimal(dal.ExecuteScalar(string.Format(sql, processId), CommandType.Text));
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
            return value;
        }
        #endregion

        #region Gets the Process Flag from the Process control master
        public bool GetProcessControl(string sysID, string processID)
        {
            bool retFlag = true;

            string sql = @" SELECT 
                                        Pcm_ProcessFlag
                                    FROM   
                                        T_ProcessControlMaster
                                    WHERE
                                        Pcm_SystemID = '{0}'
                                    AND
                                        Pcm_ProcessID = '{1}'
                                ";

            using (DALHelper dal = new DALHelper())
            {
                try 
                {
                    dal.OpenDB();
                    retFlag = Convert.ToBoolean(dal.ExecuteScalar(string.Format(sql, sysID, processID), CommandType.Text));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retFlag;

        }
        #endregion

        #region Gets the Date of the current quincena with parameter indicating the return date if "s" as start date or "e" as end date
        public string FetchQuincena(char i)
        {
            DataSet dsQuincena = new DataSet();
            string sqlGetQuincena = @" SELECT 
                                            Convert(varchar(20) , Ppm_StartCycle, 101),
                                            Convert(varchar(20) , Ppm_EndCycle , 101)
                                            
                                        FROM
                                            T_PayPeriodMaster
                                        WHERE
                                            Ppm_CycleIndicator = 'C' AND Ppm_Status = 'A' ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsQuincena = dal.ExecuteDataSet(sqlGetQuincena, CommandType.Text);
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
            switch (i)
            {
                case 's':
                    return Convert.ToString(dsQuincena.Tables[0].Rows[0][0]);
                case 'e':
                    return Convert.ToString(dsQuincena.Tables[0].Rows[0][1]);
                default:
                    return DateTime.Today.ToShortDateString();
            }

        }
        #endregion

        #region Gets the shift for the day based on the employee id and date parameters
        public DataSet GetShifts(string employeeid, string date)
        {
                DataSet dsShifts = new DataSet();

                string[] array = new string[2];
                array[0] = date;
                array[1] = employeeid;
                dsShifts.Clear();
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    try
                    {
                        string sqlGetShift = @" if(select convert(char(10),Ppm_EndCycle,101) from dbo.T_PayPeriodMaster where Ppm_CycleIndicator='C')>'{0}'
                                             begin
                                             select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,
                                                        --Leave hours in 1st half  
                                                          convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                          ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                        --Leave hours in 2nd half
                                                          convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                             (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                               from T_ShiftCodeMaster where  Scm_ShiftCode=
                                                 (select Ell_ShiftCode from T_EmployeeLogLedger  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'
                                                  union
                                                  select Ell_ShiftCode from T_EmployeeLogLedgerHist  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}')                                                  
                                             end
                                           else
                                             begin 
                                             select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,
                                                        --Leave hours in 1st half  
                                                          convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                          ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                        --Leave hours in 2nd half
                                                          convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                             (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                               from T_ShiftCodeMaster where  Scm_ShiftCode=(select emt_shiftcode from t_employeemaster where emt_employeeid='{1}')
                                             end";
                        
                        dsShifts = dal.ExecuteDataSet(string.Format(sqlGetShift, array), CommandType.Text);
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
            
            return dsShifts;
        }
        #endregion

        #region Returns a bool value evaluting if usercode is in employee master
        public bool InEmployeeMaster(string usercode)
        { 
            Methods m = new Methods();
            int count = 0;
            string sql1 =@"  SELECT Count(emt_employeeid)
                                     FROM t_employeemaster
                                     WHERE emt_employeeid = '{0}'";
            string sql2 =@"  SELECT Count(emt_employeeid)
                                     FROM t_employeemaster
                                     WHERE emt_oldemployeeid = '{0}'";
            while (usercode.Length < 6)
            {
                    usercode = "0" + usercode;
            }
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    if (m.GetProcessControl("PERSONNEL", "VIEWOLDID"))
                    {
                        count = Convert.ToInt32(dal.ExecuteScalar(string.Format(sql1, usercode), CommandType.Text));
                    }
                    else
                    {
                        count = Convert.ToInt32(dal.ExecuteScalar(string.Format(sql2, usercode), CommandType.Text));
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return (count > 0);
            
        }
        #endregion

        #region Populate Drop down list from account details
        public static void PopulateDropDown(DropDownList ddl, string type)
        {
            DataTable dt = new DataTable();
            string sqlGetStatus = @"     SELECT Adt_AccountDesc
                                                            ,Adt_AccountCode
                                                        FROM T_AccountDetail
                                                        WHERE Adt_AccountType = @type
                                                        AND Adt_Status = 'A'";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@type", type);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text, param).Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                    ddl.Items.Clear();
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(dr["Adt_AccountDesc"].ToString(), dr["Adt_AccountCode"].ToString()));
            }
        }
        #endregion

        #region Gets the Can Retrieve Flag of User Logged
        public static bool getAccessRights(string userID, string sysMenuCode)
        {
            DataSet dsRights = new DataSet();

            #region sql query
             string sqlGetRights = @"select isnull(Ugt_CanCheck, 0)
                                    from t_usergrant
                                    left join t_usergroupdetail on ugd_usercode = '{0}'
                                    where   ugt_systemid  = ugd_systemid
                                     and ugt_usergroup = ugd_usergroupcode
                                        and ugt_sysmenucode = '{1}'
                                     and ugt_status    = 'A'
            ";
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

            if (dsRights.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean((dsRights.Tables[0].Rows[0][0].ToString()));
            else
                return false;
        }
        #endregion

        #region Gets the employee route of the given employee id and transaction type
        private string[] GetEmployeeRoute(string empID, string transactionType)
        {
            string[] empRoute = new string[3];

            string transType = string.Empty;

            DataSet dsRoutes = new DataSet();

            string sql = @"SELECT a.arm_checker1, 
                                  a.arm_checker2,
                                  a.arm_approver
                             FROM T_EmployeeApprovalRoute AS e
                             LEFT JOIN T_ApprovalRouteMaster AS a 
                               ON a.arm_routeid = e.arm_routeid
                            WHERE e.arm_employeeid = '{0}' 
                              AND e.arm_transactionid = '{1}'
                                ";
            
            ////////////////////Legend////////////////////////
            //  01 - Overtime                                  //
            //  02 - Leave                                     //
            //  03 - Job Split                                 //
            //  04 - Time Modification                         //
            //  05 - Flextime                                  //
            //  06 - Movement                                  //
            /////////////////////////////////////////////////////
            switch(transactionType.Trim().ToUpper())
            {
                case "OVERTIME":
                    transType = "OVERTIME";
                    break;
                case "LEAVE":
                    transType = "LEAVE";
                    break;
                case "JOBMOD":
                    transType = "JOBMOD";
                    break;
                case "TIMEMOD":
                    transType = "TIMEMOD";
                    break;
                case "FLEXTIME":
                    transType = "FLEXTIME";
                    break;
                case "MOVEMENT":
                    transType = "MOVEMENT";
                    break;
                default:
                    break;
            }

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRoutes = dal.ExecuteDataSet(string.Format(sql, empID, transType), CommandType.Text);
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

            empRoute[0] = Convert.ToString(dsRoutes.Tables[0].Rows[0][0]);
            empRoute[1] = Convert.ToString(dsRoutes.Tables[0].Rows[0][1]);
            empRoute[2] = Convert.ToString(dsRoutes.Tables[0].Rows[0][2]);

            return empRoute;
        }
        #endregion

        #region Method for Control Number Generation
        public static string GetControlNumber(string transactionCode)
        {
            string controlNum = "";

            string sqlControlNoFetch = @"   DECLARE @yr2Digit varchar(2)
                                            SET @yr2Digit = (SELECT right(Ccd_CurrentYear, 2) from T_CompanyMaster)


                                            SELECT Tcm_TransactionPrefix 
	                                            + @yr2Digit 
	                                            + replicate('0', 9 - len(RTrim(Tcm_LastSeries)))
	                                            + RTrim(Tcm_LastSeries)
                                            FROM T_TransactionControlMaster
                                            WITH (UPDLOCK)
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
                    dal.ExecuteNonQuery(string.Format(sqlControlNoUpdate, transactionCode.ToUpper()), CommandType.Text);
                    controlNum = Convert.ToString(dal.ExecuteScalar(string.Format(sqlControlNoFetch, transactionCode.ToUpper()), CommandType.Text));
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return controlNum;
        }
        #endregion

        #region Method for getting Process Control Flag
        public static bool GetProcessControlFlag(string Pcm_SystemID, string Pcm_ProcessID)
        {
            DataSet ds = new DataSet();


            string qString = @"Select Pcm_ProcessFlag, Pcm_Status, Pcm_ProcessDesc
                                From T_ProcessControlMaster
                                Where Pcm_SystemID = @Pcm_SystemID
                                And Pcm_ProcessID = @Pcm_ProcessID
                                And Pcm_Status = 'A'";


            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Pcm_SystemID", Pcm_SystemID);
            paramInfo[1] = new ParameterInfo("@Pcm_ProcessID", Pcm_ProcessID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString().Trim());
            }
            else
                //20090104 dave changed to throw exception, terminate function if the flag does not exist
                throw (new Exception(Pcm_SystemID + " - " + Pcm_ProcessID + " is not yet setup."));
            //return false;  
        }
        #endregion

        #region Method for getting Parameter value
        public static decimal GetParameterValue(string processId)
        {
            decimal value = 0;
            string sqlQuery = @"SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = @Pmt_ParameterID";

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Pmt_ParameterID", processId);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = Convert.ToDecimal(dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0].Rows[0][0]);
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
            return value;
        }
        #endregion

        #region Method for getting PayPeriod
        public static string getPayPeriod(string Date)
        {
            string value = string.Empty;
            string sql = @" SELECT Ppm_PayPeriod
                            FROM T_PayPeriodMaster
                            WHERE @ProcessDate BETWEEN Ppm_StartCycle AND Ppm_EndCycle
                                AND Ppm_CycleIndicator <> 'S'";

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", Date);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    value = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo).Tables[0].Rows[0][0].ToString();
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
            return value;
        }
        #endregion

        #region Method to check if user is registered in the Employee Master table
        #region Check if logged in user is in Employee Master
        public static bool isInMaster(string userCode)
        {
            string userID = "";
            string sqlCheck = @"SELECT emt_employeeid
                            FROM T_EmployeeMaster
                            WHERE emt_employeeid = '{0}'";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    userID = Convert.ToString(dal.ExecuteScalar(string.Format(sqlCheck, userCode), CommandType.Text));
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

            return (userID.Trim() != "");
        }
        #endregion
        #endregion

        #region Gets the Access rights flag based on sysmenu code and user id and specific column
        public static bool getAccessRights(string userID, string sysMenuCode, string Field)
        {
            DataSet dsRights = new DataSet();

            #region sql query
            string sqlGetRights = @"select isnull({2}, 0)
                                    from t_usergrant
                                    left join t_usergroupdetail on ugd_usercode = '{0}'
                                    where   ugt_systemid  = ugd_systemid
                                     and ugt_usergroup = ugd_usergroupcode
                                        and ugt_sysmenucode = '{1}'
                                     and ugt_status    = 'A'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRights = dal.ExecuteDataSet(string.Format(sqlGetRights, userID, sysMenuCode, Field), CommandType.Text);
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

            if (dsRights.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean((dsRights.Tables[0].Rows[0][0].ToString()));
            else
                return false;
        }

        #endregion

        public static void InsertLogLedgerTrail(string Ell_EmployeeId, string Ell_PayPeriod, DALHelper dal)
        {
            int retval = 0;
            /* WHERE Ell_EmployeeId = @Ell_EmployeeId
                                   AND Ell_PayPeriod = @Ell_PayPeriod*/
            Boolean existprocess = false;
            #region SQL Query
            //robert05222013
            string qString1 = @" DECLARE @EmployeeID VARCHAR(15) = @Ell_EmployeeId
                        DECLARE @AffectedPayPeriod VARCHAR(7) = @Ell_PayPeriod
                        DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Ppm_PayPeriod 
									                           From T_PayPeriodMaster 
									                           Where Ppm_CycleIndicator='C' 
										                        and Ppm_Status = 'A')

                        --INSERT INTO LOG LEDGER TRAIL
                        IF NOT EXISTS (
	                        Select TOP 1 * 
	                        From T_EmployeeLogLedgerTrail
	                        Where Ell_EmployeeId = @EmployeeID
		                        And Ell_PayPeriod = @AffectedPayPeriod
		                        And Ell_AdjustpayPeriod = @AdjustPayPeriod
                        )
                        INSERT INTO T_EmployeeLogLedgerTrail
                        (
	                        Ell_EmployeeId
	                        , Ell_ProcessDate
	                        , Ell_AdjustPayPeriod
	                        , Ell_PayPeriod
	                        , Ell_DayCode
	                        , Ell_ShiftCode
	                        , Ell_Holiday
	                        , Ell_RestDay
	                        , Ell_ActualTimeIn_1
	                        , Ell_ActualTimeOut_1
	                        , Ell_ActualTimeIn_2
	                        , Ell_ActualTimeOut_2
	                        , Ell_ConvertedTimeIn_1Min
	                        , Ell_ConvertedTimeOut_1Min
	                        , Ell_ConvertedTimeIn_2Min
	                        , Ell_ConvertedTimeOut_2Min
	                        , Ell_ComputedTimeIn_1Min
	                        , Ell_ComputedTimeOut_1Min
	                        , Ell_ComputedTimeIn_2Min
	                        , Ell_ComputedTimeOut_2Min
	                        , Ell_AdjustShiftMin
	                        , Ell_ShiftTimeIn_1Min
	                        , Ell_ShiftTimeOut_1Min
	                        , Ell_ShiftTimeIn_2Min
	                        , Ell_ShiftTimeOut_2Min
	                        , Ell_ShiftMin
	                        , Ell_ScheduleType
	                        , Ell_EncodedPayLeaveType
	                        , Ell_EncodedPayLeaveHr
	                        , Ell_PayLeaveMin
	                        , Ell_ExcessLeaveMin
	                        , Ell_EncodedNoPayLeaveType
	                        , Ell_EncodedNoPayLeaveHr
	                        , Ell_NoPayLeaveMin
	                        , Ell_EncodedOvertimeAdvHr
	                        , Ell_EncodedOvertimePostHr
	                        , Ell_EncodedOvertimeMin
	                        , Ell_ComputedOvertimeMin
	                        , Ell_OffsetOvertimeMin
	                        , Ell_ComputedLateMin
	                        , Ell_LatePost
	                        , Ell_InitialAbsentMin
	                        , Ell_ComputedAbsentMin
	                        , Ell_ComputedRegularMin
	                        , Ell_ComputedDayWorkMin
	                        , Ell_ComputedRegularNightPremMin
	                        , Ell_ComputedOvertimeNightPremMin
	                        , Ell_PreviousDayWorkMin
	                        , Ell_PreviousDayHolidayReference
	                        , Ell_GraveyardPost
	                        , Ell_GraveyardPostBy
	                        , Ell_GraveyardPostDate
	                        , Ell_AssumedPresent
	                        , Ell_AssumedPresentBy
	                        , Ell_AssumedPresentDate
	                        , Ell_ForceLeave
	                        , Ell_ForceLeaveBy
	                        , Ell_ForceLeaveDate
	                        , Ell_ForOffsetMin
	                        , Ell_ExcessOffset
	                        , Ell_EarnedSatOff
	                        , Ell_SundayHolidayCount
	                        , Ell_WorkingDay
	                        , Ell_MealDay
	                        , Ell_ExpectedHour
	                        , Ell_AbsentHour
	                        , Ell_RegularHour
	                        , Ell_OvertimeHour
	                        , Ell_RegularNightPremHour
	                        , Ell_OvertimeNightPremHour
	                        , Ell_LeaveHour
	                        , Usr_Login
	                        , Ludatetime
	                        , Ell_ForwardedNextDayHour
	                        , Ell_AllowanceAmt01
	                        , Ell_AllowanceAmt02
	                        , Ell_AllowanceAmt03
	                        , Ell_AllowanceAmt04
	                        , Ell_AllowanceAmt05
	                        , Ell_AllowanceAmt06
	                        , Ell_AllowanceAmt07
	                        , Ell_AllowanceAmt08
	                        , Ell_AllowanceAmt09
	                        , Ell_AllowanceAmt10
	                        , Ell_AllowanceAmt11
	                        , Ell_AllowanceAmt12
	                        , Ell_LocationCode
	                        , Ell_Flex
	                        , Ell_TagFlex
	                        , Ell_TagTimeMod
	                        , Ell_WorkType
	                        , Ell_WorkGroup
	                        , Ell_AssumedPostBack
	                        , Ell_InitialOffsetMin
	                        , Ell_AppliedOffsetMin
	                        , Ell_ComputedOffsetMin
	                        , Ell_ComputedLate2Min
	                        , Ell_ComputedUndertimeMin
	                        , Ell_ComputedUndertime2Min
                        )
                        Select Ell_EmployeeId
	                        ,Ell_ProcessDate
	                        ,@AdjustPayPeriod
	                        ,Ell_PayPeriod
	                        ,Ell_DayCode
	                        ,Ell_ShiftCode
	                        ,Ell_Holiday
	                        ,Ell_RestDay
	                        ,Ell_ActualTimeIn_1
	                        ,Ell_ActualTimeOut_1
	                        ,Ell_ActualTimeIn_2
	                        ,Ell_ActualTimeOut_2
	                        ,Ell_ConvertedTimeIn_1Min
	                        ,Ell_ConvertedTimeOut_1Min
	                        ,Ell_ConvertedTimeIn_2Min
	                        ,Ell_ConvertedTimeOut_2Min
	                        ,Ell_ComputedTimeIn_1Min
	                        ,Ell_ComputedTimeOut_1Min
	                        ,Ell_ComputedTimeIn_2Min
	                        ,Ell_ComputedTimeOut_2Min
	                        ,Ell_AdjustShiftMin
	                        ,Ell_ShiftTimeIn_1Min
	                        ,Ell_ShiftTimeOut_1Min
	                        ,Ell_ShiftTimeIn_2Min
	                        ,Ell_ShiftTimeOut_2Min
	                        ,Ell_ShiftMin
	                        ,Ell_ScheduleType
	                        ,Ell_EncodedPayLeaveType
	                        ,Ell_EncodedPayLeaveHr
	                        ,Ell_PayLeaveMin
	                        ,Ell_ExcessLeaveMin
	                        ,Ell_EncodedNoPayLeaveType
	                        ,Ell_EncodedNoPayLeaveHr
	                        ,Ell_NoPayLeaveMin
	                        ,Ell_EncodedOvertimeAdvHr
	                        ,Ell_EncodedOvertimePostHr
	                        ,Ell_EncodedOvertimeMin
	                        ,Ell_ComputedOvertimeMin
	                        ,Ell_OffsetOvertimeMin
	                        ,Ell_ComputedLateMin
	                        ,Ell_LatePost
	                        ,Ell_InitialAbsentMin
	                        ,Ell_ComputedAbsentMin
	                        ,Ell_ComputedRegularMin
	                        ,Ell_ComputedDayWorkMin
	                        ,Ell_ComputedRegularNightPremMin
	                        ,Ell_ComputedOvertimeNightPremMin
	                        ,Ell_PreviousDayWorkMin
	                        ,Ell_PreviousDayHolidayReference
	                        ,Ell_GraveyardPost
	                        ,Ell_GraveyardPostBy
	                        ,Ell_GraveyardPostDate
	                        ,Ell_AssumedPresent
	                        ,Ell_AssumedPresentBy
	                        ,Ell_AssumedPresentDate
	                        ,Ell_ForceLeave
	                        ,Ell_ForceLeaveBy
	                        ,Ell_ForceLeaveDate
	                        ,Ell_ForOffsetMin
	                        ,Ell_ExcessOffset
	                        ,Ell_EarnedSatOff
	                        ,Ell_SundayHolidayCount
	                        ,Ell_WorkingDay
	                        ,Ell_MealDay
	                        ,Ell_ExpectedHour
	                        ,Ell_AbsentHour
	                        ,Ell_RegularHour
	                        ,Ell_OvertimeHour
	                        ,Ell_RegularNightPremHour
	                        ,Ell_OvertimeNightPremHour
	                        ,Ell_LeaveHour
	                        ,Usr_Login
	                        ,Ludatetime
	                        ,Ell_ForwardedNextDayHour
	                        ,Ell_AllowanceAmt01
	                        ,Ell_AllowanceAmt02
	                        ,Ell_AllowanceAmt03
	                        ,Ell_AllowanceAmt04
	                        ,Ell_AllowanceAmt05
	                        ,Ell_AllowanceAmt06
	                        ,Ell_AllowanceAmt07
	                        ,Ell_AllowanceAmt08
	                        ,Ell_AllowanceAmt09
	                        ,Ell_AllowanceAmt10
	                        ,Ell_AllowanceAmt11
	                        ,Ell_AllowanceAmt12       
	                        ,Ell_LocationCode
	                        ,Ell_Flex
	                        ,Ell_TagFlex
	                        ,Ell_TagTimeMod
	                        ,Ell_WorkType
	                        ,Ell_WorkGroup
	                        ,Ell_AssumedPostBack
	                        ,Ell_InitialOffsetMin
	                        ,Ell_AppliedOffsetMin
	                        ,Ell_ComputedOffsetMin
	                        ,Ell_ComputedLate2Min
	                        ,Ell_ComputedUndertimeMin
	                        ,Ell_ComputedUndertime2Min
                        From T_EmployeeLogLedgerHist
                        Where Ell_EmployeeId = @EmployeeID
	                        And Ell_PayPeriod = @AffectedPayPeriod";

            existprocess = Convert.ToBoolean(dal.ExecuteScalar(@"IF exists(select Pcm_ProcessFlag from T_ProcessControlmaster
                                    WHERE Pcm_ProcessID ='PAYTRANTRL')
                                    SELECT Pcm_ProcessFlag from T_ProcessControlmaster
                                    WHERE Pcm_ProcessID ='PAYTRANTRL'
                                    else 
	                                    SELECT 0"));
            if (existprocess)
                qString1 += @"--INSERT INTO PAYROLL TRANSACTION TRAIL
                            IF NOT EXISTS (
	                            Select TOP 1 * 
	                            From T_EmployeePayrollTransactionTrail
	                            Where Ept_EmployeeId = @EmployeeID
		                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
		                            And Ept_AdjustpayPeriod = @AdjustPayPeriod
                            )
                            INSERT INTO T_EmployeePayrollTransactionTrail
                            (
	                               Ept_EmployeeId
                                  ,Ept_AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_AbsentHr
                                  ,Ept_RegularHr
                                  ,Ept_RegularOTHr
                                  ,Ept_RegularNDHr
                                  ,Ept_RegularOTNDHr
                                  ,Ept_RestdayHr
                                  ,Ept_RestdayOTHr
                                  ,Ept_RestdayNDHr
                                  ,Ept_RestdayOTNDHr
                                  ,Ept_LegalHolidayHr
                                  ,Ept_LegalHolidayOTHr
                                  ,Ept_LegalHolidayNDHr
                                  ,Ept_LegalHolidayOTNDHr
                                  ,Ept_SpecialHolidayHr
                                  ,Ept_SpecialHolidayOTHr
                                  ,Ept_SpecialHolidayNDHr
                                  ,Ept_SpecialHolidayOTNDHr
                                  ,Ept_PlantShutdownHr
                                  ,Ept_PlantShutdownOTHr
                                  ,Ept_PlantShutdownNDHr
                                  ,Ept_PlantShutdownOTNDHr
                                  ,Ept_CompanyHolidayHr
                                  ,Ept_CompanyHolidayOTHr
                                  ,Ept_CompanyHolidayNDHr
                                  ,Ept_CompanyHolidayOTNDHr
                                  ,Ept_RestdayLegalHolidayHr
                                  ,Ept_RestdayLegalHolidayOTHr
                                  ,Ept_RestdayLegalHolidayNDHr
                                  ,Ept_RestdayLegalHolidayOTNDHr
                                  ,Ept_RestdaySpecialHolidayHr
                                  ,Ept_RestdaySpecialHolidayOTHr
                                  ,Ept_RestdaySpecialHolidayNDHr
                                  ,Ept_RestdaySpecialHolidayOTNDHr
                                  ,Ept_RestdayCompanyHolidayHr
                                  ,Ept_RestdayCompanyHolidayOTHr
                                  ,Ept_RestdayCompanyHolidayNDHr
                                  ,Ept_RestdayCompanyHolidayOTNDHr
                                  ,Ept_RestdayPlantShutdownHr
                                  ,Ept_RestdayPlantShutdownOTHr
                                  ,Ept_RestdayPlantShutdownNDHr
                                  ,Ept_RestdayPlantShutdownOTNDHr
                                  ,Ept_LaborHrsAdjustmentAmt
                                  ,Ept_TaxAdjustmentAmt
                                  ,Ept_NonTaxAdjustmentAmt
                                  ,Ept_TaxAllowanceAmt
                                  ,Ept_NonTaxAllowanceAmt
                                  ,Ept_RestdayLegalHolidayCount
                                  ,Ept_WorkingDay
                                  ,Ept_PayrollType
                                  , Ept_LateHr
                                  ,Ept_UndertimeHr
                                  ,Ept_WholeDayAbsentHr
                                  ,Ept_UnpaidLeaveHr
                                  ,Ept_AbsentLegalHolidayHr
                                  ,Ept_AbsentSpecialHolidayHr
                                  ,Ept_AbsentCompanyHolidayHr
                                  ,Ept_AbsentPlantShutdownHr
                                  ,Ept_AbsentFillerHolidayHr
                                  ,Ept_PaidLeaveHr
                                  ,Ept_PaidLegalHolidayHr
                                  ,Ept_PaidSpecialHolidayHr
                                  ,Ept_PaidCompanyHolidayHr
                                  ,Ept_PaidFillerHolidayHr
                                  ,Ept_OvertimeAdjustmentAmt
								  ,Ept_UserGenerated
                                  ,Usr_Login
                                  ,Ludatetime
                            )
                            Select Ept_EmployeeId
                                  ,@AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_AbsentHr
                                  ,Ept_RegularHr
                                  ,Ept_RegularOTHr
                                  ,Ept_RegularNDHr
                                  ,Ept_RegularOTNDHr
                                  ,Ept_RestdayHr
                                  ,Ept_RestdayOTHr
                                  ,Ept_RestdayNDHr
                                  ,Ept_RestdayOTNDHr
                                  ,Ept_LegalHolidayHr
                                  ,Ept_LegalHolidayOTHr
                                  ,Ept_LegalHolidayNDHr
                                  ,Ept_LegalHolidayOTNDHr
                                  ,Ept_SpecialHolidayHr
                                  ,Ept_SpecialHolidayOTHr
                                  ,Ept_SpecialHolidayNDHr
                                  ,Ept_SpecialHolidayOTNDHr
                                  ,Ept_PlantShutdownHr
                                  ,Ept_PlantShutdownOTHr
                                  ,Ept_PlantShutdownNDHr
                                  ,Ept_PlantShutdownOTNDHr
                                  ,Ept_CompanyHolidayHr
                                  ,Ept_CompanyHolidayOTHr
                                  ,Ept_CompanyHolidayNDHr
                                  ,Ept_CompanyHolidayOTNDHr
                                  ,Ept_RestdayLegalHolidayHr
                                  ,Ept_RestdayLegalHolidayOTHr
                                  ,Ept_RestdayLegalHolidayNDHr
                                  ,Ept_RestdayLegalHolidayOTNDHr
                                  ,Ept_RestdaySpecialHolidayHr
                                  ,Ept_RestdaySpecialHolidayOTHr
                                  ,Ept_RestdaySpecialHolidayNDHr
                                  ,Ept_RestdaySpecialHolidayOTNDHr
                                  ,Ept_RestdayCompanyHolidayHr
                                  ,Ept_RestdayCompanyHolidayOTHr
                                  ,Ept_RestdayCompanyHolidayNDHr
                                  ,Ept_RestdayCompanyHolidayOTNDHr
                                  ,Ept_RestdayPlantShutdownHr
                                  ,Ept_RestdayPlantShutdownOTHr
                                  ,Ept_RestdayPlantShutdownNDHr
                                  ,Ept_RestdayPlantShutdownOTNDHr
                                  ,Ept_LaborHrsAdjustmentAmt
                                  ,Ept_TaxAdjustmentAmt
                                  ,Ept_NonTaxAdjustmentAmt
                                  ,Ept_TaxAllowanceAmt
                                  ,Ept_NonTaxAllowanceAmt
                                  ,Ept_RestdayLegalHolidayCount
                                  ,Ept_WorkingDay
                                  ,Ept_PayrollType
                                  , Ept_LateHr
                                  ,Ept_UndertimeHr
                                  ,Ept_WholeDayAbsentHr
                                  ,Ept_UnpaidLeaveHr
                                  ,Ept_AbsentLegalHolidayHr
                                  ,Ept_AbsentSpecialHolidayHr
                                  ,Ept_AbsentCompanyHolidayHr
                                  ,Ept_AbsentPlantShutdownHr
                                  ,Ept_AbsentFillerHolidayHr
                                  ,Ept_PaidLeaveHr
                                  ,Ept_PaidLegalHolidayHr
                                  ,Ept_PaidSpecialHolidayHr
                                  ,Ept_PaidCompanyHolidayHr
                                  ,Ept_PaidFillerHolidayHr
                                  ,Ept_OvertimeAdjustmentAmt
								  ,Ept_UserGenerated
                                  ,Usr_Login
                                  ,Ludatetime
                            From T_EmployeePayrollTransactionHist
                            Where Ept_EmployeeId = @EmployeeID
	                            And Ept_CurrentPayPeriod = @AffectedPayPeriod

                            --INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
                            IF NOT EXISTS (
	                            Select TOP 1 * 
	                            From T_EmployeePayrollTransactionTrailDetail
	                            Where Ept_EmployeeId = @EmployeeID
		                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
		                            And Ept_AdjustpayPeriod = @AdjustPayPeriod
                            )
                            INSERT INTO T_EmployeePayrollTransactionTrailDetail
                            (
	                               Ept_EmployeeId
                                  ,Ept_AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_ProcessDate
                                  ,Ept_SalaryRate
                                  ,Ept_HourlyRate
                                  ,Ept_SalaryType
                                  ,Ept_PayrollType
                                  ,Ept_AbsentHr
                                  ,Ept_RegularHr
                                  ,Ept_RegularOTHr
                                  ,Ept_RegularNDHr
                                  ,Ept_RegularOTNDHr
                                  ,Ept_RestdayHr
                                  ,Ept_RestdayOTHr
                                  ,Ept_RestdayNDHr
                                  ,Ept_RestdayOTNDHr
                                  ,Ept_LegalHolidayHr
                                  ,Ept_LegalHolidayOTHr
                                  ,Ept_LegalHolidayNDHr
                                  ,Ept_LegalHolidayOTNDHr
                                  ,Ept_SpecialHolidayHr
                                  ,Ept_SpecialHolidayOTHr
                                  ,Ept_SpecialHolidayNDHr
                                  ,Ept_SpecialHolidayOTNDHr
                                  ,Ept_PlantShutdownHr
                                  ,Ept_PlantShutdownOTHr
                                  ,Ept_PlantShutdownNDHr
                                  ,Ept_PlantShutdownOTNDHr
                                  ,Ept_CompanyHolidayHr
                                  ,Ept_CompanyHolidayOTHr
                                  ,Ept_CompanyHolidayNDHr
                                  ,Ept_CompanyHolidayOTNDHr
                                  ,Ept_RestdayLegalHolidayHr
                                  ,Ept_RestdayLegalHolidayOTHr
                                  ,Ept_RestdayLegalHolidayNDHr
                                  ,Ept_RestdayLegalHolidayOTNDHr
                                  ,Ept_RestdaySpecialHolidayHr
                                  ,Ept_RestdaySpecialHolidayOTHr
                                  ,Ept_RestdaySpecialHolidayNDHr
                                  ,Ept_RestdaySpecialHolidayOTNDHr
                                  ,Ept_RestdayCompanyHolidayHr
                                  ,Ept_RestdayCompanyHolidayOTHr
                                  ,Ept_RestdayCompanyHolidayNDHr
                                  ,Ept_RestdayCompanyHolidayOTNDHr
                                  ,Ept_RestdayPlantShutdownHr
                                  ,Ept_RestdayPlantShutdownOTHr
                                  ,Ept_RestdayPlantShutdownNDHr
                                  ,Ept_RestdayPlantShutdownOTNDHr
                                  ,Ept_RestdayLegalHolidayCount
                                  ,Ept_WorkingDay
                                  ,Ept_LateHr
                                  ,Ept_UndertimeHr
                                  ,Ept_WholeDayAbsentHr
                                  ,Ept_UnpaidLeaveHr
                                  ,Ept_AbsentLegalHolidayHr
                                  ,Ept_AbsentSpecialHolidayHr
                                  ,Ept_AbsentCompanyHolidayHr
                                  ,Ept_AbsentPlantShutdownHr
                                  ,Ept_AbsentFillerHolidayHr
                                  ,Ept_PaidLeaveHr
                                  ,Ept_PaidLegalHolidayHr
                                  ,Ept_PaidSpecialHolidayHr
                                  ,Ept_PaidCompanyHolidayHr
                                  ,Ept_PaidFillerHolidayHr
                                  ,Ept_OvertimeAdjustmentAmt
                                  ,Usr_Login
                                  ,Ludatetime
                            )
                            Select Ept_EmployeeId
                                  ,@AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_ProcessDate
                                  ,Ept_SalaryRate
                                  ,Ept_HourlyRate
                                  ,Ept_SalaryType
                                  ,Ept_PayrollType
                                  ,Ept_AbsentHr
                                  ,Ept_RegularHr
                                  ,Ept_RegularOTHr
                                  ,Ept_RegularNDHr
                                  ,Ept_RegularOTNDHr
                                  ,Ept_RestdayHr
                                  ,Ept_RestdayOTHr
                                  ,Ept_RestdayNDHr
                                  ,Ept_RestdayOTNDHr
                                  ,Ept_LegalHolidayHr
                                  ,Ept_LegalHolidayOTHr
                                  ,Ept_LegalHolidayNDHr
                                  ,Ept_LegalHolidayOTNDHr
                                  ,Ept_SpecialHolidayHr
                                  ,Ept_SpecialHolidayOTHr
                                  ,Ept_SpecialHolidayNDHr
                                  ,Ept_SpecialHolidayOTNDHr
                                  ,Ept_PlantShutdownHr
                                  ,Ept_PlantShutdownOTHr
                                  ,Ept_PlantShutdownNDHr
                                  ,Ept_PlantShutdownOTNDHr
                                  ,Ept_CompanyHolidayHr
                                  ,Ept_CompanyHolidayOTHr
                                  ,Ept_CompanyHolidayNDHr
                                  ,Ept_CompanyHolidayOTNDHr
                                  ,Ept_RestdayLegalHolidayHr
                                  ,Ept_RestdayLegalHolidayOTHr
                                  ,Ept_RestdayLegalHolidayNDHr
                                  ,Ept_RestdayLegalHolidayOTNDHr
                                  ,Ept_RestdaySpecialHolidayHr
                                  ,Ept_RestdaySpecialHolidayOTHr
                                  ,Ept_RestdaySpecialHolidayNDHr
                                  ,Ept_RestdaySpecialHolidayOTNDHr
                                  ,Ept_RestdayCompanyHolidayHr
                                  ,Ept_RestdayCompanyHolidayOTHr
                                  ,Ept_RestdayCompanyHolidayNDHr
                                  ,Ept_RestdayCompanyHolidayOTNDHr
                                  ,Ept_RestdayPlantShutdownHr
                                  ,Ept_RestdayPlantShutdownOTHr
                                  ,Ept_RestdayPlantShutdownNDHr
                                  ,Ept_RestdayPlantShutdownOTNDHr
                                  ,Ept_RestdayLegalHolidayCount
                                  ,Ept_WorkingDay
                                  ,Ept_LateHr
                                  ,Ept_UndertimeHr
                                  ,Ept_WholeDayAbsentHr
                                  ,Ept_UnpaidLeaveHr
                                  ,Ept_AbsentLegalHolidayHr
                                  ,Ept_AbsentSpecialHolidayHr
                                  ,Ept_AbsentCompanyHolidayHr
                                  ,Ept_AbsentPlantShutdownHr
                                  ,Ept_AbsentFillerHolidayHr
                                  ,Ept_PaidLeaveHr
                                  ,Ept_PaidLegalHolidayHr
                                  ,Ept_PaidSpecialHolidayHr
                                  ,Ept_PaidCompanyHolidayHr
                                  ,Ept_PaidFillerHolidayHr
                                  ,Ept_OvertimeAdjustmentAmt
                                  ,Usr_Login
                                  ,Ludatetime
                            From T_EmployeePayrollTransactionHistDetail
                            Where Ept_EmployeeId = @EmployeeID
	                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
	
                            --INSERT INTO PAYROLL TRANSACTION TRAIL EXT
                            IF NOT EXISTS (
	                            Select TOP 1 * 
	                            From T_EmployeePayrollTransactionTrailExt
	                            Where Ept_EmployeeId = @EmployeeID
		                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
		                            And Ept_AdjustpayPeriod = @AdjustPayPeriod
                            )
                            INSERT INTO T_EmployeePayrollTransactionTrailExt
                            (
	                               Ept_EmployeeId
                                  ,Ept_AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_Filler01_Hr
                                  ,Ept_Filler01_OTHr
                                  ,Ept_Filler01_NDHr
                                  ,Ept_Filler01_OTNDHr
                                  ,Ept_Filler02_Hr
                                  ,Ept_Filler02_OTHr
                                  ,Ept_Filler02_NDHr
                                  ,Ept_Filler02_OTNDHr
                                  ,Ept_Filler03_Hr
                                  ,Ept_Filler03_OTHr
                                  ,Ept_Filler03_NDHr
                                  ,Ept_Filler03_OTNDHr
                                  ,Ept_Filler04_Hr
                                  ,Ept_Filler04_OTHr
                                  ,Ept_Filler04_NDHr
                                  ,Ept_Filler04_OTNDHr
                                  ,Ept_Filler05_Hr
                                  ,Ept_Filler05_OTHr
                                  ,Ept_Filler05_NDHr
                                  ,Ept_Filler05_OTNDHr
                                  ,Ept_Filler06_Hr
                                  ,Ept_Filler06_OTHr
                                  ,Ept_Filler06_NDHr
                                  ,Ept_Filler06_OTNDHr
                                  ,Usr_Login
                                  ,Ludatetime
                            )
                            Select Ept_EmployeeId
                                  ,@AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_Filler01_Hr
                                  ,Ept_Filler01_OTHr
                                  ,Ept_Filler01_NDHr
                                  ,Ept_Filler01_OTNDHr
                                  ,Ept_Filler02_Hr
                                  ,Ept_Filler02_OTHr
                                  ,Ept_Filler02_NDHr
                                  ,Ept_Filler02_OTNDHr
                                  ,Ept_Filler03_Hr
                                  ,Ept_Filler03_OTHr
                                  ,Ept_Filler03_NDHr
                                  ,Ept_Filler03_OTNDHr
                                  ,Ept_Filler04_Hr
                                  ,Ept_Filler04_OTHr
                                  ,Ept_Filler04_NDHr
                                  ,Ept_Filler04_OTNDHr
                                  ,Ept_Filler05_Hr
                                  ,Ept_Filler05_OTHr
                                  ,Ept_Filler05_NDHr
                                  ,Ept_Filler05_OTNDHr
                                  ,Ept_Filler06_Hr
                                  ,Ept_Filler06_OTHr
                                  ,Ept_Filler06_NDHr
                                  ,Ept_Filler06_OTNDHr
                                  ,Usr_Login
                                  ,Ludatetime
                            From T_EmployeePayrollTransactionHistExt
                            Where Ept_EmployeeId = @EmployeeID
	                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
	
                            --INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
                            IF NOT EXISTS (
	                            Select TOP 1 * 
	                            From T_EmployeePayrollTransactionTrailExtDetail
	                            Where Ept_EmployeeId = @EmployeeID
		                            And Ept_CurrentPayPeriod = @AffectedPayPeriod
		                            And Ept_AdjustpayPeriod = @AdjustPayPeriod
                            )
                            INSERT INTO T_EmployeePayrollTransactionTrailExtDetail
                            (
	                               Ept_EmployeeId
                                  ,Ept_AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_ProcessDate
                                  ,Ept_SalaryRate
                                  ,Ept_HourlyRate
                                  ,Ept_SalaryType
                                  ,Ept_PayrollType
                                  ,Ept_Filler01_Hr
                                  ,Ept_Filler01_OTHr
                                  ,Ept_Filler01_NDHr
                                  ,Ept_Filler01_OTNDHr
                                  ,Ept_Filler02_Hr
                                  ,Ept_Filler02_OTHr
                                  ,Ept_Filler02_NDHr
                                  ,Ept_Filler02_OTNDHr
                                  ,Ept_Filler03_Hr
                                  ,Ept_Filler03_OTHr
                                  ,Ept_Filler03_NDHr
                                  ,Ept_Filler03_OTNDHr
                                  ,Ept_Filler04_Hr
                                  ,Ept_Filler04_OTHr
                                  ,Ept_Filler04_NDHr
                                  ,Ept_Filler04_OTNDHr
                                  ,Ept_Filler05_Hr
                                  ,Ept_Filler05_OTHr
                                  ,Ept_Filler05_NDHr
                                  ,Ept_Filler05_OTNDHr
                                  ,Ept_Filler06_Hr
                                  ,Ept_Filler06_OTHr
                                  ,Ept_Filler06_NDHr
                                  ,Ept_Filler06_OTNDHr
                                  ,Usr_Login
                                  ,Ludatetime
                            )
                            Select Ept_EmployeeId
                                  ,@AdjustPayPeriod
                                  ,Ept_CurrentPayPeriod
                                  ,Ept_ProcessDate
                                  ,Ept_SalaryRate
                                  ,Ept_HourlyRate
                                  ,Ept_SalaryType
                                  ,Ept_PayrollType
                                  ,Ept_Filler01_Hr
                                  ,Ept_Filler01_OTHr
                                  ,Ept_Filler01_NDHr
                                  ,Ept_Filler01_OTNDHr
                                  ,Ept_Filler02_Hr
                                  ,Ept_Filler02_OTHr
                                  ,Ept_Filler02_NDHr
                                  ,Ept_Filler02_OTNDHr
                                  ,Ept_Filler03_Hr
                                  ,Ept_Filler03_OTHr
                                  ,Ept_Filler03_NDHr
                                  ,Ept_Filler03_OTNDHr
                                  ,Ept_Filler04_Hr
                                  ,Ept_Filler04_OTHr
                                  ,Ept_Filler04_NDHr
                                  ,Ept_Filler04_OTNDHr
                                  ,Ept_Filler05_Hr
                                  ,Ept_Filler05_OTHr
                                  ,Ept_Filler05_NDHr
                                  ,Ept_Filler05_OTNDHr
                                  ,Ept_Filler06_Hr
                                  ,Ept_Filler06_OTHr
                                  ,Ept_Filler06_NDHr
                                  ,Ept_Filler06_OTNDHr
                                  ,Usr_Login
                                  ,Ludatetime
                            From T_EmployeePayrollTransactionHistExtDetail
                            Where Ept_EmployeeId = @EmployeeID
	                            And Ept_CurrentPayPeriod = @AffectedPayPeriod";
            #endregion
           
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", Ell_EmployeeId, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Ell_PayPeriod", Ell_PayPeriod, SqlDbType.VarChar, 7);

            retval = dal.ExecuteNonQuery(qString1, CommandType.Text, paramInfo);
            
        }

        public static bool CheckIfRecordExixstsInTrail(string Ell_EmployeeId, string Ell_PayPeriod, string Ell_AdjustpayPeriod)
        {
            DataSet ds = new DataSet();
            Boolean isexistprocess = false;
            DataSet ds2 = new DataSet();
            //robert05222013
            string sql2 = @"Select Ept_EmployeeId 
	                    From T_EmployeePayrollTransactionTrail
	                    Where Ept_EmployeeId = @Ell_EmployeeId
		                    And Ept_CurrentPayPeriod = @Ell_PayPeriod
		                    And Ept_AdjustpayPeriod = @Ell_AdjustpayPeriod";
            #region SQL Query
            string qString = @"Select  Ell_EmployeeId
                                From T_EmployeeLogLedgerTrail
                                Where Ell_EmployeeId = @Ell_EmployeeId
                                And Ell_PayPeriod = @Ell_PayPeriod
                                And Ell_AdjustpayPeriod = @Ell_AdjustpayPeriod";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", Ell_EmployeeId, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Ell_PayPeriod", Ell_PayPeriod, SqlDbType.VarChar, 7);
            paramInfo[2] = new ParameterInfo("@Ell_AdjustpayPeriod", Ell_AdjustpayPeriod, SqlDbType.VarChar, 7);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                isexistprocess =Convert.ToBoolean(dal.ExecuteScalar(@"IF exists(select Pcm_ProcessFlag from T_ProcessControlmaster
                    WHERE Pcm_ProcessID ='PAYTRANTRL')
                    SELECT Pcm_ProcessFlag from T_ProcessControlmaster
                    WHERE Pcm_ProcessID ='PAYTRANTRL'
                    else 
	                    SELECT 0"));
                ds2 = dal.ExecuteDataSet(sql2, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            
            
            bool bFinal = false;

           
            bFinal = (ds.Tables[0].Rows.Count > 0);

            if (isexistprocess && ds2.Tables[0].Rows.Count == 0)
            {
                bFinal = false;
            }

            return bFinal;
        }

        public static bool CheckIfRecordExixstsInTrail(string Ell_EmployeeId, string Ell_PayPeriod, string Ell_AdjustpayPeriod, DALHelper dal)
        {
            DataSet ds = new DataSet();
            Boolean existprocess = false;
            DataSet ds2 = new DataSet();
            bool retstring = true;
            string sql2 = @"Select TOP 1 * 
	                    From T_EmployeePayrollTransactionTrail
	                    Where Ept_EmployeeId = @Ell_EmployeeId
		                    And Ept_CurrentPayPeriod = @Ell_PayPeriod
		                    And Ept_AdjustpayPeriod = @Ell_AdjustpayPeriod";
            #region SQL Query
            string qString = @"Select TOP 1 Ell_EmployeeID 
                                From T_EmployeeLogLedgerTrail
                                Where Ell_EmployeeId = @Ell_EmployeeId
                                And Ell_PayPeriod = @Ell_PayPeriod
                                And Ell_AdjustpayPeriod = @Ell_AdjustpayPeriod";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Ell_EmployeeId", Ell_EmployeeId);
            paramInfo[1] = new ParameterInfo("@Ell_PayPeriod", Ell_PayPeriod);
            paramInfo[2] = new ParameterInfo("@Ell_AdjustpayPeriod", Ell_AdjustpayPeriod);

            ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            existprocess = Convert.ToBoolean(dal.ExecuteScalar(@"IF exists(select Pcm_ProcessFlag from T_ProcessControlmaster
                    WHERE Pcm_ProcessID ='PAYTRANTRL')
                    SELECT Pcm_ProcessFlag from T_ProcessControlmaster
                    WHERE Pcm_ProcessID ='PAYTRANTRL'
                    else 
	                    SELECT 0"));
            ds2 = dal.ExecuteDataSet(sql2, CommandType.Text, paramInfo);


            bool bFinal = false;

            //if (ds.Tables[0].Rows.Count > 0 && ds2.Tables[0].Rows.Count > 0)
            //    bFinal = true;
            //if (ds.Tables[0].Rows.Count > 0 && (ds2.Tables[0].Rows.Count == 0 && !existprocess))
            //    bFinal = true;
            bFinal = (ds.Tables[0].Rows.Count > 0);

            if (existprocess && ds2.Tables[0].Rows.Count == 0)
            {
             bFinal = false;
            }

            return bFinal;
        }

        #region Return a date if cut-off is set to true and string.Empty if no cut-off
        public static bool isCutOff(string system, string date)
        {
            bool retFlag = true;
            string sql = @"
                        SELECT CASE WHEN Pcm_ProcessFlag = 1 AND Convert(datetime,'{0}') <= Ppm_EndCycle
                        THEN 1 ELSE 0 END
                        FROM T_ProcessControlMaster
                        LEFT JOIN T_PayPeriodMaster ON Ppm_CycleIndicator = 'C' and ppm_status = 'A'
                        WHERE Pcm_SystemID = '{1}'
                        AND	  Pcm_ProcessId = 'CUT-OFF'
                        ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    retFlag = Convert.ToBoolean(dal.ExecuteScalar(string.Format(sql, date, system), CommandType.Text));
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }

            }

            return retFlag;
        }
        #endregion

        #region Get compant information
        public static string GetCompanyInfo(string colName)
        {
            string sql = string.Format(@"
                    SELECT TOP 1 {0} FROM T_CompanyMaster", colName);
            string temp = "No data was retrieved";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    temp = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
                }
                catch
                {
                    temp = "Error in getting information";
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return temp;
        }
        
        public static string GetCompanyInfoERP(string colName)
        {
            string sql = string.Format(@"
                    SELECT TOP 1 {0} FROM {1}..T_SystemControl", colName, ConfigurationManager.AppSettings["ERP_DB"]);
            string temp = "No data was retrieved";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    temp = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
                }
                catch
                {
                    temp = "Error in getting information";
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return temp;
        }
        #endregion

        #region
        public static bool checkTransactionControlExist(string transactionType)
        {
            string sql = @"SELECT Tcm_TransactionCode 
                             FROM T_TransactionControlMaster
                            WHERE Tcm_TransactionCode = '{0}'";
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, transactionType), CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);
        }
        #endregion
    }
}
