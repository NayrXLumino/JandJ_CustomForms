/*
 *  Updated By      :   1277 - Arriesgado, Robert Jayre
 *  Updated Date    :   04/12/2013
 *  Update Notes    :   
 *      -   Added Query onisCreditsDeductable() to cancel leave on Hist
 * 
 */
/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/14/2013
 *  Update Notes    :   
 *      -   Added Validations on Leave Credits manipulation on Query
 * 
 */
/*
 * Updated By       : 1277 - Robert
 * Updated Date     : 10/10/2013
 * Update Notes     : Get year function THI. 
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
using System.Globalization;

/// <summary>
/// Summary description for LeaveBL
/// </summary>
namespace Payroll.DAL
{
    public class LeaveBL
    {
        CommonMethods methods = new CommonMethods();
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        public string LeaveControlNo = string.Empty;
        public string LeaveBatchNo = string.Empty;
        public LeaveBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public decimal getCredits(string EmployeeId, string LeaveYear, string LeaveType, DALHelper dal)
        {
            DataSet ds = new DataSet();
            decimal value = 0;
            #region Pervious Query
            //            string sql = @" declare @LHRSINDAY as decimal(8,2)
//                            declare @LVHRENTRY as char(1)
//                                --SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
//                                --              FROM T_ParameterMaster 
//                                --             WHERE Pmt_ParameterId = 'LHRSINDAY')
//                                SET @LHRSINDAY = 1 --credits checking is always in hours
//                                SET @LVHRENTRY = (SELECT Convert(char(1),Pcm_ProcessFlag)
//                                              FROM T_ProcessControlMaster 
//                                             WHERE Pcm_SystemID = 'LEAVE'
//                                               AND Pcm_ProcessID = 'LVHRENTRY')
//
//                            SELECT CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '1')
//                                                         THEN ELM1.Elm_Entitled - ELM1.Elm_Used - ELM1.Elm_Reserved 
//                                                         ELSE (ELM1.Elm_Entitled / @LHRSINDAY ) - (ELM1.Elm_Used / @LHRSINDAY) - (ELM1.Elm_Reserved / @LHRSINDAY)
//                                                     END ) [Credit1]
//                                 , CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '1')
//                                                         THEN ISNULL(ELM2.Elm_Entitled,0) - ISNULL(ELM2.Elm_Used,0) - ISNULL(ELM2.Elm_Reserved,0) 
//                                                         ELSE (ISNULL(ELM2.Elm_Entitled,0) / @LHRSINDAY ) - (ISNULL(ELM2.Elm_Used,0) / @LHRSINDAY) - (ISNULL(ELM2.Elm_Reserved,0) / @LHRSINDAY)
//                                                     END )
//                                 , ISNULL(ELM2.Elm_Entitled,0) - ISNULL(ELM2.Elm_Used,0) - ISNULL(ELM2.Elm_Reserved,0) [Credit2]
//                                 , CASE WHEN (ISNULL(Ltm_PartOfLeave,'') = '')
//			                            THEN 'FALSE'
//			                            ELSE 'TRUE'
//	                                END [hasPart]
//                              FROM T_EmployeeLeave ELM1
//                              LEFT JOIN T_LeaveTypeMaster
//	                            ON Ltm_LeaveType = ELM1.Elm_LeaveType
//                              LEFT JOIN T_EmployeeLeave ELM2
//                                ON ELM2.Elm_LeaveType = Ltm_PartOfLeave
//                               AND ELM2.Elm_LeaveYear = @leaveYear
//                               AND ELM2.Elm_EmployeeId = @employeeId
//                             WHERE ELM1.Elm_LeaveYear = @leaveYear
//                               AND ELM1.Elm_EmployeeId = @employeeId
            //                               AND ELM1.Elm_LeaveType = @leaveType";
            #endregion
            #region Query
            //PERTH REVISED 02/12/2013
            string sql = @"
SELECT
	ELM1.Elm_Entitled 
		- (ELM1.Elm_Used 
			+ ELM1.Elm_Reserved) [Credit1]	
	, ISNULL(ELM2.Elm_Entitled, 0.00) 
		- (ISNULL(ELM2.Elm_Used, 0.00) 
			+ ISNULL(ELM2.Elm_Reserved, 0.00)) [Credit2]
	, CASE WHEN isnull(Ltm_PartOfLeave, '') = ''
		THEN 'FALSE'
		ELSE 'TRUE'
		END [hasPart]
FROM T_EmployeeLeave ELM1
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = ELM1.Elm_Leavetype
LEFT JOIN T_EmployeeLeave ELM2
	ON ELM2.Elm_LeaveYear = ELM1.Elm_LeaveYear
	AND ELM2.Elm_EmployeeId = ELM1.Elm_EmployeeId
	AND ELM2.Elm_Leavetype = Ltm_PartOfLeave
WHERE ELM1.Elm_EmployeeId = @employeeId
	AND ELM1.Elm_LeaveYear = @leaveYear
	AND ELM1.Elm_Leavetype = @leaveType
            ";

            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                sql = @"
SELECT
	ELM1.Elm_Entitled 
		- (ELM1.Elm_Used 
			+ ELM1.Elm_Reserved
			+ ELM1.Elm_Reimbursed) [Credit1]	
	, ISNULL(ELM2.Elm_Entitled, 0.00) 
		- (ISNULL(ELM2.Elm_Used, 0.00) 
			+ ISNULL(ELM2.Elm_Reserved, 0.00)
			+ isnull(ELM2.Elm_Reimbursed, 0.00)) [Credit2]
	, CASE WHEN isnull(Ltm_PartOfLeave, '') = ''
		THEN 'FALSE'
		ELSE 'TRUE'
		END [hasPart]
FROM T_EmployeeLeave ELM1
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = ELM1.Elm_Leavetype
LEFT JOIN T_EmployeeLeave ELM2
	ON ELM2.Elm_LeaveYear = ELM1.Elm_LeaveYear
	AND ELM2.Elm_EmployeeId = ELM1.Elm_EmployeeId
	AND ELM2.Elm_Leavetype = Ltm_PartOfLeave
WHERE ELM1.Elm_EmployeeId = @employeeId
	AND ELM1.Elm_LeaveYear = @leaveYear
	AND ELM1.Elm_Leavetype = @leaveType
            ";
            }
            #endregion

            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@leaveYear", LeaveYear);
            param[1] = new ParameterInfo("@employeeId", EmployeeId);
            param[2] = new ParameterInfo("@leaveType", LeaveType.Substring(0,2));
                    
            ds = dal.ExecuteDataSet(sql, CommandType.Text, param);

            if (!CommonMethods.isEmpty(ds))
            {
                if (Convert.ToBoolean(ds.Tables[0].Rows[0]["hasPart"].ToString()))
                {
                    if (Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString()) < Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit2"].ToString()))
                    {
                        value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString());
                    }
                    else
                    {
                        value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit2"].ToString());
                    }
                }
                else
                {
                    value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString());
                }
            }
            else
            {
                if (LeaveType.Substring(3, 1).Equals("1"))
                {
                    value = 0;
                }
                else
                {
                    value = 9999;
                }
            }

            return value;
        }

        public decimal getCredits(string EmployeeId, string LeaveYear, string LeaveType)
        {
            DataSet ds = new DataSet();
            decimal value = 0;
            #region Previous
            //            string sql = @" declare @LHRSINDAY as decimal(8,2)
//                            declare @LVHRENTRY as char(1)
//                                --SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
//                                --             FROM T_ParameterMaster 
//                                --             WHERE Pmt_ParameterId = 'LHRSINDAY')
//                                SET @LHRSINDAY = 1 --credits checking is always in hours
//                                SET @LVHRENTRY = (SELECT Convert(char(1),Pcm_ProcessFlag)
//                                              FROM T_ProcessControlMaster 
//                                             WHERE Pcm_SystemID = 'LEAVE'
//                                               AND Pcm_ProcessID = 'LVHRENTRY')
//
//                            SELECT CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '1')
//                                                         THEN ELM1.Elm_Entitled - ELM1.Elm_Used - ELM1.Elm_Reserved 
//                                                         ELSE (ELM1.Elm_Entitled / @LHRSINDAY ) - (ELM1.Elm_Used / @LHRSINDAY) - (ELM1.Elm_Reserved / @LHRSINDAY)
//                                                     END ) [Credit1]
//                                 , CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '1')
//                                                         THEN ISNULL(ELM2.Elm_Entitled,0) - ISNULL(ELM2.Elm_Used,0) - ISNULL(ELM2.Elm_Reserved,0) 
//                                                         ELSE (ISNULL(ELM2.Elm_Entitled,0) / @LHRSINDAY ) - (ISNULL(ELM2.Elm_Used,0) / @LHRSINDAY) - (ISNULL(ELM2.Elm_Reserved,0) / @LHRSINDAY)
//                                                     END )
//                                 , ISNULL(ELM2.Elm_Entitled,0) - ISNULL(ELM2.Elm_Used,0) - ISNULL(ELM2.Elm_Reserved,0) [Credit2]
//                                 , CASE WHEN (ISNULL(Ltm_PartOfLeave,'') = '')
//			                            THEN 'FALSE'
//			                            ELSE 'TRUE'
//	                                END [hasPart]
//                              FROM T_EmployeeLeave ELM1
//                              LEFT JOIN T_LeaveTypeMaster
//	                            ON Ltm_LeaveType = ELM1.Elm_LeaveType
//                              LEFT JOIN T_EmployeeLeave ELM2
//                                ON ELM2.Elm_LeaveType = Ltm_PartOfLeave
//                               AND ELM2.Elm_LeaveYear = @leaveYear
//                               AND ELM2.Elm_EmployeeId = @employeeId
//                             WHERE ELM1.Elm_LeaveYear = @leaveYear
//                               AND ELM1.Elm_EmployeeId = @employeeId
            //                               AND ELM1.Elm_LeaveType = @leaveType";
            #endregion
            #region Query
            string sql = @"
SELECT
	ELM1.Elm_Entitled 
		- (ELM1.Elm_Used 
			+ ELM1.Elm_Reserved) [Credit1]	
	, ISNULL(ELM2.Elm_Entitled, 0.00) 
		- (ISNULL(ELM2.Elm_Used, 0.00) 
			+ ISNULL(ELM2.Elm_Reserved, 0.00)) [Credit2]
	, CASE WHEN isnull(Ltm_PartOfLeave, '') = ''
		THEN 'FALSE'
		ELSE 'TRUE'
		END [hasPart]
FROM T_EmployeeLeave ELM1
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = ELM1.Elm_Leavetype
LEFT JOIN T_EmployeeLeave ELM2
	ON ELM2.Elm_LeaveYear = ELM1.Elm_LeaveYear
	AND ELM2.Elm_EmployeeId = ELM1.Elm_EmployeeId
	AND ELM2.Elm_Leavetype = Ltm_PartOfLeave
WHERE ELM1.Elm_EmployeeId = @employeeId
	AND ELM1.Elm_LeaveYear = @leaveYear
	AND ELM1.Elm_Leavetype = @leaveType
            ";
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                sql = @"
SELECT
	ELM1.Elm_Entitled 
		- (ELM1.Elm_Used 
			+ ELM1.Elm_Reserved
			+ ELM1.Elm_Reimbursed) [Credit1]	
	, ISNULL(ELM2.Elm_Entitled, 0.00) 
		- (ISNULL(ELM2.Elm_Used, 0.00) 
			+ ISNULL(ELM2.Elm_Reserved, 0.00)
			+ isnull(ELM2.Elm_Reimbursed, 0.00)) [Credit2]
	, CASE WHEN isnull(Ltm_PartOfLeave, '') = ''
		THEN 'FALSE'
		ELSE 'TRUE'
		END [hasPart]
FROM T_EmployeeLeave ELM1
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = ELM1.Elm_Leavetype
LEFT JOIN T_EmployeeLeave ELM2
	ON ELM2.Elm_LeaveYear = ELM1.Elm_LeaveYear
	AND ELM2.Elm_EmployeeId = ELM1.Elm_EmployeeId
	AND ELM2.Elm_Leavetype = Ltm_PartOfLeave
WHERE ELM1.Elm_EmployeeId = @employeeId
	AND ELM1.Elm_LeaveYear = @leaveYear
	AND ELM1.Elm_Leavetype = @leaveType
            ";
            }
            #endregion
            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@leaveYear", LeaveYear);
            param[1] = new ParameterInfo("@employeeId", EmployeeId);
            param[2] = new ParameterInfo("@leaveType", LeaveType.Substring(0, 2));
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
                if (Convert.ToBoolean(ds.Tables[0].Rows[0]["hasPart"].ToString()))
                {
                    if (Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString()) < Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit2"].ToString()))
                    {
                        value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString());
                    }
                    else
                    {
                        value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit2"].ToString());
                    }
                }
                else
                {
                    value = Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"].ToString());
                }
            }
            else
            {
                if (LeaveType.Substring(3, 1).Equals("1"))
                {
                    value = 0;
                }
                else
                {
                    value = 9999;
                }
            }

            return value;
        }

        public string messageCannotSuffice(string EmployeeId, string LeaveYear, string LeaveType, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string msg = string.Empty;
            string sql = @" declare @LHRSINDAY as decimal(8,2)
                            declare @LVHRENTRY as char(1)
                                SET @LHRSINDAY = (SELECT ISNULL(Pmt_NumericValue,8) 
                                                    FROM T_ParameterMaster 
                                                   WHERE Pmt_ParameterId = 'LHRSINDAY')
                                --SET @LVHRENTRY = (SELECT Convert(char(1),Pcm_ProcessFlag)
                                --                    FROM T_ProcessControlMaster 
                                --                   WHERE Pcm_SystemID = 'LEAVE'
                                --                     AND Pcm_ProcessID = 'LVHRENTRY')
                                SET @LVHRENTRY = '{0}'

                            SELECT CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '0')
                                                         THEN ELM1.Elm_Entitled - ELM1.Elm_Used - ELM1.Elm_Reserved 
                                                         ELSE (ELM1.Elm_Entitled / @LHRSINDAY ) - (ELM1.Elm_Used / @LHRSINDAY) - (ELM1.Elm_Reserved / @LHRSINDAY)
                                                     END ) [Credit1]
                                 , CONVERT(decimal(7,2), CASE WHEN (@LVHRENTRY = '0')
                                                         THEN ISNULL(ELM2.Elm_Entitled,0) - ISNULL(ELM2.Elm_Used,0) - ISNULL(ELM2.Elm_Reserved,0) 
                                                         ELSE (ISNULL(ELM2.Elm_Entitled,0) / @LHRSINDAY ) - (ISNULL(ELM2.Elm_Used,0) / @LHRSINDAY) - (ISNULL(ELM2.Elm_Reserved,0) / @LHRSINDAY)
                                                     END )
                                 , ISNULL(ELM2.Elm_Entitled,0) [Credit2]
                                 , ELM1.Elm_LeaveType [LeaveType1]
                                 , ELM2.Elm_LeaveType [LeaveType2]
                                 , CASE WHEN (ISNULL(Ltm_PartOfLeave,'') = '')
			                            THEN 'FALSE'
			                            ELSE 'TRUE'
	                                END [hasPart]
                              FROM T_EmployeeLeave ELM1
                              LEFT JOIN T_LeaveTypeMaster
	                            ON Ltm_LeaveType = ELM1.Elm_LeaveType
                              LEFT JOIN T_EmployeeLeave ELM2
                                ON ELM2.Elm_LeaveType = Ltm_PartOfLeave
                               AND ELM2.Elm_LeaveYear = @leaveYear
                               AND ELM2.Elm_EmployeeId = @employeeId
                             WHERE ELM1.Elm_LeaveYear = @leaveYear
                               AND ELM1.Elm_EmployeeId = @employeeId
                               AND ELM1.Elm_LeaveType = @leaveType";

            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@leaveYear", LeaveYear);
            param[1] = new ParameterInfo("@employeeId", EmployeeId);
            param[2] = new ParameterInfo("@leaveType", LeaveType.Substring(0,2));

            ds = dal.ExecuteDataSet(string.Format(sql, (Convert.ToBoolean(Resources.Resource.LEAVECREDITSINHOURS) ? '1' : '0')), CommandType.Text, param);

            if (!CommonMethods.isEmpty(ds))
            {
                if (Convert.ToBoolean(ds.Tables[0].Rows[0]["hasPart"].ToString()))
                { 
                    if(Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit1"]) < Convert.ToDecimal(ds.Tables[0].Rows[0]["Credit2"]))
                    {
                        msg = "Credits for " + ds.Tables[0].Rows[0]["LeaveType1"] + " cannot suffice.";
                    }
                    else
                    {
                        msg = "Credits for " + ds.Tables[0].Rows[0]["LeaveType2"] + " cannot suffice as " + ds.Tables[0].Rows[0]["LeaveType1"] + " is part of " + ds.Tables[0].Rows[0]["LeaveType2"] + ".";
                    }
                }
                else
                {
                    msg = "Credits for " + ds.Tables[0].Rows[0]["LeaveType1"] + " cannot suffice.";
                }
            }
            else
            {
                //Leave must have been for next year
            }

            return msg;
        }

        public string getLeaveFlag(string LeaveDate)
        {
            string value = string.Empty;
            string sql = @"SELECT CASE WHEN @leaveDate < Ppm_StartCycle
                                       THEN 'P'
                                       WHEN @leaveDate <= Ppm_EndCycle
                                       THEN 'C'
                                       ELSE 'F'
                                   END FROM T_PayPeriodMaster
                            WHERE Ppm_CycleIndicator = 'C'";
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@leaveDate", LeaveDate);

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

        public DataSet CheckIfPaidWithCreditCombined(string LeaveType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveType", LeaveType.Substring(0,2));

            string sqlQuery = @" SELECT Ltm_LeaveType
                                      , Ltm_CombinedLeave
                                      , Ltm_PaidLeave
                                      , Ltm_WithCredit
                                   FROM T_LeaveTypeMaster
                                  WHERE Ltm_LeaveType = @LeaveType";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public void UpdateLogLedger(DataRow rowDetails, DALHelper dal)
        {
            #region [Variables]
            DataSet tmpDS = new DataSet();
            string firstLType = string.Empty;
            string lveCombination = string.Empty;
            decimal firstLTypeHrs = 0;
            decimal totalHrs = 0;
            #endregion

            tmpDS = CheckIfLogLedgerHasEntryOnFiledDate(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString());

            if (tmpDS != null && tmpDS.Tables.Count > 0 && tmpDS.Tables[0].Rows.Count > 0 && tmpDS.Tables[0].Rows[0][0].ToString() != string.Empty)
            {
                if (CheckIfCombinedLeave(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString()))
                {
                    lveCombination = tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString();
                    totalHrs = (Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveHr"]) + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString()));
                }
                else
                {
                    firstLType = tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString();
                    firstLTypeHrs = Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveHr"]);
                    //Get Leave Type Combination
                    tmpDS = GetLeaveCombination(firstLType, rowDetails["Elt_LeaveType"].ToString());
                    if (tmpDS.Tables[0].Rows.Count > 0)
                    {
                        lveCombination = tmpDS.Tables[0].Rows[0]["Ltm_LeaveType"].ToString();
                        totalHrs = firstLTypeHrs + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                    else if (tmpDS.Tables[0].Rows.Count == 0)
                    {
                        lveCombination = rowDetails["Elt_LeaveType"].ToString();
                        totalHrs = Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                }
                PostToLogLedgerWithPay(rowDetails["Elt_LeaveFlag"].ToString(), lveCombination, totalHrs.ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
            else
            {
                PostToLogLedgerWithPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_LeaveType"].ToString(), rowDetails["Elt_LeaveHour"].ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
        }

        public void UpdateLogLedger(string controlNo, string userLogged, DALHelper dal)
        {
            #region [Variables]
            DataSet tmpDS = new DataSet();
            string firstLType = string.Empty;
            string lveCombination = string.Empty;
            decimal firstLTypeHrs = 0;
            decimal totalHrs = 0;
            #endregion
            DataRow rowDetails = getLeaveInfo(controlNo, dal);
            rowDetails["Usr_login"] = userLogged;

            UpdateCredits("APPROVE", rowDetails, dal);

            tmpDS = CheckIfLogLedgerHasEntryOnFiledDate(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString());

            if (tmpDS != null && tmpDS.Tables.Count > 0 && tmpDS.Tables[0].Rows.Count > 0 && tmpDS.Tables[0].Rows[0][0].ToString() != string.Empty)
            {
                if (CheckIfCombinedLeave(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString()))
                {
                    lveCombination = tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString();
                    totalHrs = (Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveHr"]) + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString()));
                }
                else
                {
                    firstLType = tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveType"].ToString();
                    firstLTypeHrs = Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedPayLeaveHr"]);

                    //To sum up the current leave hour from ledger with the same leave type in a day: 
                    if (firstLType == rowDetails["Elt_LeaveType"].ToString())
                    {
                        lveCombination = firstLType;
                        totalHrs = firstLTypeHrs + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                    else
                    {
                        //Get Leave Type Combination
                        tmpDS = GetLeaveCombination(firstLType, rowDetails["Elt_LeaveType"].ToString());
                        if (tmpDS.Tables[0].Rows.Count > 0)
                        {
                            lveCombination = tmpDS.Tables[0].Rows[0]["Ltm_LeaveType"].ToString();
                            totalHrs = firstLTypeHrs + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                        }
                        else if (tmpDS.Tables[0].Rows.Count == 0)
                        {
                            lveCombination = rowDetails["Elt_LeaveType"].ToString();
                            totalHrs = Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                        }
                    }
                    
                }
                PostToLogLedgerWithPay(rowDetails["Elt_LeaveFlag"].ToString(), lveCombination, totalHrs.ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
            else
            {
                PostToLogLedgerWithPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_LeaveType"].ToString(), rowDetails["Elt_LeaveHour"].ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
        }

        public void UpdateLogLedgerWithNoPay(DataRow rowDetails, DALHelper dal)
        {
            #region [Variables]
            DataSet tmpDS = new DataSet();
            string firstLType = string.Empty;
            string lveCombination = string.Empty;
            decimal firstLTypeHrs = 0;
            decimal totalHrs = 0;
            #endregion

            tmpDS = CheckIfLogLedgerHasEntryOnFiledDateNoPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString());

            if (tmpDS != null && tmpDS.Tables.Count > 0 && tmpDS.Tables[0].Rows.Count > 0 && tmpDS.Tables[0].Rows[0][0].ToString() != string.Empty)
            {
                //Check if combined leave
                if (CheckIfCombinedLeave(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString()))
                {
                    lveCombination = tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString();
                    totalHrs = (Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveHr"]) + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString()));
                }
                else
                {
                    firstLType = tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString();
                    firstLTypeHrs = Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveHr"]);
                    //Get leave type combination
                    tmpDS = GetLeaveCombination(firstLType, rowDetails["Elt_LeaveType"].ToString());
                    if (tmpDS.Tables[0].Rows.Count > 0)
                    {
                        lveCombination = tmpDS.Tables[0].Rows[0]["Ltm_LeaveType"].ToString();
                        totalHrs = firstLTypeHrs + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                    else if (tmpDS.Tables[0].Rows.Count == 0)
                    {
                        lveCombination = rowDetails["Elt_LeaveType"].ToString();
                        totalHrs = Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                }
                PostToLogLedgerWithNoPay(rowDetails["Elt_LeaveFlag"].ToString(), lveCombination, totalHrs.ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
            else
            {
                PostToLogLedgerWithNoPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_LeaveType"].ToString(), rowDetails["Elt_LeaveHour"].ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
        }

        public void UpdateLogLedgerWithNoPay(string controlNo, string userLogged, DALHelper dal)
        {
            #region [Variables]
            DataSet tmpDS = new DataSet();
            string firstLType = string.Empty;
            string lveCombination = string.Empty;
            decimal firstLTypeHrs = 0;
            decimal totalHrs = 0;
            #endregion

            DataRow rowDetails = getLeaveInfo(controlNo, dal);
            rowDetails["Usr_login"] = userLogged;
            UpdateCredits("APPROVE", rowDetails, dal);

            tmpDS = CheckIfLogLedgerHasEntryOnFiledDateNoPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString());

            if (tmpDS != null && tmpDS.Tables.Count > 0 && tmpDS.Tables[0].Rows.Count > 0 && tmpDS.Tables[0].Rows[0][0].ToString() != string.Empty)
            {
                //Check if combined leave
                if (CheckIfCombinedLeave(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString()))
                {
                    lveCombination = tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString();
                    totalHrs = (Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveHr"]) + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString()));
                }
                else
                {
                    firstLType = tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveType"].ToString();
                    firstLTypeHrs = Convert.ToDecimal(tmpDS.Tables[0].Rows[0]["Ell_EncodedNoPayLeaveHr"]);
                    //Get leave type combination
                    tmpDS = GetLeaveCombination(firstLType, rowDetails["Elt_LeaveType"].ToString());
                    if (tmpDS.Tables[0].Rows.Count > 0)
                    {
                        lveCombination = tmpDS.Tables[0].Rows[0]["Ltm_LeaveType"].ToString();
                        totalHrs = firstLTypeHrs + Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                    else if (tmpDS.Tables[0].Rows.Count == 0)
                    {
                        lveCombination = rowDetails["Elt_LeaveType"].ToString();
                        totalHrs = Convert.ToDecimal(rowDetails["Elt_LeaveHour"].ToString());
                    }
                }
                PostToLogLedgerWithNoPay(rowDetails["Elt_LeaveFlag"].ToString(), lveCombination, totalHrs.ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
            else
            {
                PostToLogLedgerWithNoPay(rowDetails["Elt_LeaveFlag"].ToString(), rowDetails["Elt_LeaveType"].ToString(), rowDetails["Elt_LeaveHour"].ToString(), rowDetails["Usr_Login"].ToString(), rowDetails["Elt_EmployeeId"].ToString(), rowDetails["Elt_LeaveDate"].ToString(), dal);
            }
        }

        public int PostToLogLedgerWithNoPay(string PayPeriod, string LeaveType, string LeaveHr, string UserLogin, string EmployeeID, string LeaveDate, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType.Substring(0,2));
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveHr", LeaveHr);
            paramInfo[paramIndex++] = new ParameterInfo("@UserLogin", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region SQL Query

            string sqlQuery = @"DECLARE @TableName char(30)

                                IF @PayPeriod = 'C'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                           ELSE IF @PayPeriod = 'P'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedgerHist' END
                           ELSE IF @PayPeriod = 'F'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                              ELSE 
	                         BEGIN Set @TableName = 'T_EmployeeLogLedger' END

                           EXECUTE ('UPDATE '+ @TableName +'
                                        SET Ell_EncodedNoPayLeaveType = '''+ @LeaveType +''' 
                                          , Ell_EncodedNoPayLeaveHr = '''+ @LeaveHr +'''
                                          , Usr_Login = '''+ @UserLogin +'''
                                          , Ludatetime = GetDate()
	                                  WHERE Ell_EmployeeId = '''+ @EmployeeID +''' 
	                                    AND Ell_ProcessDate = '''+ @LeaveDate +'''' )";

            if (Convert.ToBoolean(Resources.Resource.USELEAVEDEFAULTSHIFT))
            {
                sqlQuery = @"DECLARE @TableName char(30)

                                IF @PayPeriod = 'C'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                           ELSE IF @PayPeriod = 'P'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedgerHist' END
                           ELSE IF @PayPeriod = 'F'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                              ELSE 
	                         BEGIN Set @TableName = 'T_EmployeeLogLedger' END

                           EXECUTE (' UPDATE '+ @TableName +'
                                         SET Ell_EncodedNoPayLeaveType = '''+ @LeaveType +''' 
                                           , Ell_EncodedNoPayLeaveHr   = '''+ @LeaveHr +'''
                                           , Ell_ShiftCode = DefaultShift.Scm_ShiftCode
                                           , Usr_Login = '''+ @UserLogin +'''
                                           , Ludatetime = GetDate()
                                        FROM '+ @TableName +'
                                       INNER JOIN T_ShiftCodeMaster ShiftCode
                                          ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                                        LEFT JOIN T_ShiftCodeMaster DefaultShift
                                          ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                                         AND DefaultShift.Scm_DefaultShift = ''True''
                                         AND DefaultShift.Scm_Status = ''A''
                                       WHERE Ell_EmployeeId = '''+ @EmployeeID +''' 
                                         AND Ell_ProcessDate = '''+ @LeaveDate +''' ')";
            }
            #endregion

            try
            {
                if (PayPeriod.ToString().Equals("P"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(EmployeeID
                                        , MethodsLibrary.Methods.getPayPeriod(LeaveDate)
                                        , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(EmployeeID
                                                , MethodsLibrary.Methods.getPayPeriod(LeaveDate)
                                                , dal);
                    }
                }

                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                CommonMethods.ErrorsToTextFile(e, "PostToLogLedgerWithPay");
            }

            return retVal;
        }

        public int PostToLogLedgerWithPay(string PayPeriod, string LeaveType, string LeaveHr, string UserLogin, string EmployeeID, string LeaveDate, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType.Substring(0,2));
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveHr", LeaveHr);
            paramInfo[paramIndex++] = new ParameterInfo("@UserLogin", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region SQL Query
            string sqlQuery = @"DECLARE @TableName char(30)

                                IF @PayPeriod = 'C'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                           ELSE IF @PayPeriod = 'P'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedgerHist' END
                           ELSE IF @PayPeriod = 'F'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                              ELSE 
	                         BEGIN Set @TableName = 'T_EmployeeLogLedger' END

                           EXECUTE ('UPDATE '+ @TableName +'
                                        SET Ell_EncodedPayLeaveType = '''+ @LeaveType +''' 
                                          , Ell_EncodedPayLeaveHr = '''+ @LeaveHr +'''
                                          , Usr_Login = '''+ @UserLogin +'''
                                          , Ludatetime = GetDate()
	                                  WHERE Ell_EmployeeId = '''+ @EmployeeID +''' 
	                                    AND Ell_ProcessDate = '''+ @LeaveDate +'''' )";

            if (Convert.ToBoolean(Resources.Resource.USELEAVEDEFAULTSHIFT))
            { 
                sqlQuery = @"DECLARE @TableName char(30)

                                IF @PayPeriod = 'C'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                           ELSE IF @PayPeriod = 'P'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedgerHist' END
                           ELSE IF @PayPeriod = 'F'
	                         BEGIN SET @TableName = 'T_EmployeeLogLedger' END
                              ELSE 
	                         BEGIN Set @TableName = 'T_EmployeeLogLedger' END

                           EXECUTE (' UPDATE '+ @TableName +'
                                         SET Ell_EncodedPayLeaveType = '''+ @LeaveType +''' 
                                           , Ell_EncodedPayLeaveHr   = '''+ @LeaveHr +'''
                                           , Ell_ShiftCode = DefaultShift.Scm_ShiftCode
                                           , Usr_Login = '''+ @UserLogin +'''
                                           , Ludatetime = GetDate()
                                        FROM '+ @TableName +'
                                       INNER JOIN T_ShiftCodeMaster ShiftCode
                                          ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                                        LEFT JOIN T_ShiftCodeMaster DefaultShift
                                          ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                                         AND DefaultShift.Scm_DefaultShift = ''True''
                                         AND DefaultShift.Scm_Status = ''A''
                                       WHERE Ell_EmployeeId = '''+ @EmployeeID +''' 
                                         AND Ell_ProcessDate = '''+ @LeaveDate +''' ')";
            }
            #endregion

            try
            {
                if (PayPeriod.ToString().Equals("P"))
                {
                    if (!MethodsLibrary.Methods.CheckIfRecordExixstsInTrail(EmployeeID
                                        , MethodsLibrary.Methods.getPayPeriod(LeaveDate)
                                        , CommonMethods.getPayPeriod("C")))
                    {
                        MethodsLibrary.Methods.InsertLogLedgerTrail(EmployeeID
                                                , MethodsLibrary.Methods.getPayPeriod(LeaveDate)
                                                , dal);
                    }
                }

                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                CommonMethods.ErrorsToTextFile(e, "PostToLogLedgerWithPay");
            }


            return retVal;
        }

        public DataSet CheckIfLogLedgerHasEntryOnFiledDateNoPay(string Type, string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            string table = string.Empty;
            if (Type.Equals("C"))
            {
                table = "T_EmployeeLogLedger";
            }
            else if (Type.Equals("P"))
            {
                table = "T_EmployeeLogLedgerHist";
            }
            else//Andre added: 20091203 - so that whenever the leave flag is future it would not give an error
            {
                table = "T_EmployeeLogLedger";
            }

            #region query

            string query = string.Format(
                            @"Select Ell_EncodedNoPayLeaveType, Ell_EncodedNoPayLeaveHr
                              From {0}
                              Where Ell_EmployeeId = @EmployeeID
	                          And Ell_ProcessDate = @LeaveDate", table);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetLeaveCombination(string firstLveType, string secondLveType)
        {
            DataSet ds = new DataSet();

            #region query

            string sqlstr = @"SELECT Ltm_LeaveType
                                    ,Substring(Ltm_LeaveDesc,1,2) as FirstCombination
                                    ,Substring(Ltm_LeaveDesc,6,7) as SecondCombination
                              FROM T_LeaveTypeMaster
                              WHERE Ltm_LeaveDesc Like '%+%'
                              AND 
                                  (
                                     (Substring(Ltm_LeaveDesc,1,2) = '{0}'
                                      And Substring(Ltm_LeaveDesc,6,7) = '{1}')
                              OR
                                      (Substring(Ltm_LeaveDesc,1,2) = '{1}'
                                       And Substring(Ltm_LeaveDesc,6,7) = '{0}')
                                   )";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(string.Format(sqlstr, firstLveType, secondLveType), CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckIfCombinedLeave(string LeaveType)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;
            bool isCombined;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType.Substring(0,2));

            #region query

            string statement = @"Select Ltm_CombinedLeave
                                 From T_LeaveTypeMaster
                                 Where Ltm_LeaveType = @LeaveType";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows[0][0].ToString().Equals("True"))
                isCombined = true;
            else
                isCombined = false;

            return isCombined;
        }

        public DataSet CheckIfLogLedgerHasEntryOnFiledDate(string Type, string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            string table = string.Empty;
            if (Type.Equals("C") || Type.Equals("F"))
            {
                table = "T_EmployeeLogLedger";
            }
            else if (Type.Equals("P"))
            {
                table = "T_EmployeeLogLedgerHist";
            }

            #region query

            string query = string.Format(
                            @"Select Ell_EncodedPayLeaveType, Ell_EncodedPayLeaveHr
                              From {0}
                              Where Ell_EmployeeId = @EmployeeID
	                          And Ell_ProcessDate = @LeaveDate", table);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public static DataRow getLeaveInfo(string controlNo, DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Elt_CurrentPayPeriod 
                                  , Elt_EmployeeId 
                                  , Elt_LeaveDate 
                                  , Elt_LeaveType 
                                  , Elt_AppliedDate 
                                  , Elt_StartTime 
                                  , Elt_EndTime 
                                  , Elt_LeaveHour 
                                  , Elt_DayUnit 
                                  , Elt_Reason 
                                  , Elt_LeaveFlag 
                                  , Elt_InformDate 
                                  , Elt_EndorsedDateToChecker 
                                  , Elt_CheckedBy 
                                  , Elt_CheckedDate 
                                  , Elt_Checked2By 
                                  , Elt_Checked2Date 
                                  , Elt_ApprovedBy 
                                  , Elt_ApprovedDate 
                                  , Elt_ControlNo 
                                  , Elt_Status 
                                  , T_EmployeeLeaveAvailment.Usr_Login [Usr_Login]
                                  , T_EmployeeLeaveAvailment.Ludatetime [Ludatetime]
                                  , Elt_LeaveCategory 
                                  , Elt_Costcenter 
                                  , Elt_Leavecode 
                                  , Elt_LeaveNotice 
                                  , Elt_Filler1 
                                  , Elt_Filler2 
                                  , Elt_Filler3 
                                  , ISNULL(EL1.Ell_ShiftCode, EL2.Ell_ShiftCode) [ShiftCode]
                                  , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
                                        THEN Elt_LeaveHour
                                        ELSE CASE WHEN Elt_LeaveHour < 0
                                                  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
                                                  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
                                              END
                                    END [CreditsToDeduct]
                               FROM T_EmployeeLeaveAvailment 
                               LEFT JOIN T_EmployeeLogLedger EL1
                                 ON EL1.Ell_EmployeeId = Elt_EmployeeId
                                AND EL1.Ell_ProcessDate = Elt_LeaveDate
                               LEFT JOIN T_EmployeeLogLedgerHist EL2
                                 ON EL2.Ell_EmployeeId = Elt_EmployeeId
                                AND EL2.Ell_ProcessDate = Elt_LeaveDate
                              LEFT JOIN T_ParameterMasterExt
                                ON Pmx_ParameterID = 'LVDEDUCTN'
                               AND Pmx_Classification = Elt_DayUnit
                              WHERE Elt_ControlNo = @ControlNo";
            return dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0].Rows[0];
        }

        public DataRow getLeaveInfo(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Elt_ControlNo [Control No]
	                              , Elt_EmployeeId [ID No]
	                              , Emt_NickName [Nickname]
	                              , Emt_Lastname [Lastname]
	                              , Emt_Firstname [Firstname]
	                              , Emt_Middlename [Middlename]
	                              , Elt_LeaveDate [Leave Date]
                                  , Elt_LeaveType [Leave Code]
                                  , Ltm_LeaveDesc [Leave Desc]
                                  , Elt_DayUnit [Day Unit]
	                              , ISNULL(AD5.Adt_AccountDesc, '- not applicable -') [Category]
	                              , LEFT(Elt_StartTime,2) + ':' + RIGHT(Elt_StartTime,2) [Start]
	                              , LEFT(Elt_EndTime,2) + ':' + RIGHT(Elt_EndTime,2) [End]
                                  , Elt_AppliedDate [Applied Date]
	                              , Elt_LeaveHour [Hours]
	                              , Elt_Reason [Reason]
	                              , Elt_Filler1 [@Elt_Filler1Desc]
	                              , Elt_Filler2 [@Elt_Filler2Desc]
	                              , Elt_Filler3 [@Elt_Filler3Desc]
	                              , AD1.Adt_AccountDesc [Status]
	                              , Trm_Remarks [Remarks]
	                              , CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
			                             THEN ISNULL(LedgerHist.Ell_DayCode, 'REG')
			                             ELSE Ledger.Ell_DayCode 
		                             END [Day Code]
	                              , Scm_ShiftCode [Shift Code]
	                              , Scm_ShiftTimeIn [Shift Time In]
	                              , Scm_ShiftBreakStart [Break Start]
	                              , Scm_ShiftBreakEnd [Break End]
	                              , Scm_ShiftTimeOut [Shift Time Out]
	                              , CASE Elt_LeaveNotice 
                                    WHEN 1 THEN 'TRUE'
                                    WHEN 0 THEN 'FALSE'
                                     END [Notice]
	                              , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	                           FROM T_EmployeeLeaveAvailment
                               LEFT JOIN T_LeaveTypeMaster
                                 ON Ltm_LeaveType = Elt_LeaveType
	                           LEFT JOIN T_EmployeeLogLedger Ledger
	                             ON Ledger.Ell_EmployeeId = Elt_EmployeeId
	                            AND Ledger.Ell_ProcessDate = Elt_LeaveDate
	                           LEFT JOIN T_EmployeeLogLedgerHist LedgerHist
	                             ON LedgerHist.Ell_EmployeeId = Elt_EmployeeId
	                            AND LedgerHist.Ell_ProcessDate = Elt_LeaveDate
	                           LEFT JOIN T_EmployeeMaster 
	                             ON Emt_EmployeeId = Elt_EmployeeId
	                           LEFT JOIN T_ShiftCodeMaster
	                             ON Scm_ShiftCode = CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
							                             THEN ISNULL(LedgerHist.Ell_ShiftCode, Emt_ShiftCode)
							                             ELSE Ledger.Ell_ShiftCode
						                             END
	                           LEFT JOIN T_AccountDetail AD1 
	                             ON AD1.Adt_AccountCode = Elt_Status 
	                            AND AD1.Adt_AccountType =  'WFSTATUS'
	                           LEFT JOIN T_AccountDetail AD2
	                             ON AD2.Adt_AccountCode = Elt_Filler1
	                            AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler01')
	                           LEFT JOIN T_AccountDetail AD3
	                             ON AD3.Adt_AccountCode = Elt_Filler2
	                            AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler02')
	                           LEFT JOIN T_AccountDetail AD4
	                             ON AD4.Adt_AccountCode = Elt_Filler3
	                            AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler03')
                               LEFT JOIN T_AccountDetail AD5
                                 ON AD5.Adt_AccountCode = Elt_LeaveCategory
                                AND AD5.Adt_AccountType = 'LVECATEGRY'
                               LEFT JOIN T_PayPeriodMaster 
 	                             ON Ppm_PayPeriod = Elt_CurrentPayPeriod
                               LEFT JOIN T_TransactionRemarks 
	                             ON Trm_ControlNo = Elt_ControlNo
                              WHERE Elt_ControlNo = @ControlNo";
            sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
            sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
            sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
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

        public DataRow getLeaveInfoFromNotice(string controlNo)
        {
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@ControlNo", controlNo);
            string sql = @"  SELECT Eln_ControlNo [Control No]
                                  , Eln_EmployeeId [ID No]
                                  , Emt_NickName [Nickname]
                                  , Emt_Lastname [Lastname]
                                  , Emt_Firstname [Firstname]
                                  , Emt_Middlename [Middlename]
                                  , Eln_LeaveDate [Leave Date]
                                  , Eln_LeaveType [Leave Code]
                                  , Ltm_LeaveDesc [Leave Desc]
                                  , Eln_DayUnit [Day Unit]
                                  , ISNULL(AD5.Adt_AccountDesc, '- not applicable -') [Category]
                                  , LEFT(Eln_StartTime,2) + ':' + RIGHT(Eln_StartTime,2) [Start]
                                  , LEFT(Eln_EndTime,2) + ':' + RIGHT(Eln_EndTime,2) [End]
                                  , Eln_LeaveHour [Hours]
                                  , Eln_Reason [Reason]
                                  , Elt_Filler1 [@Elt_Filler1Desc]
                                  , Elt_Filler2 [@Elt_Filler2Desc]
                                  , Elt_Filler3 [@Elt_Filler3Desc]
                                  , Eln_InformDate [Inform Date]
                                  , Eln_InformMode [Inform Mode]
                                  , Eln_Informant [Informant]
                                  , Eln_InformantRelation [Relation]
                                  , AD1.Adt_AccountDesc [Status]
                                  , CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                         THEN ISNULL(LedgerHist.Ell_DayCode, 'REG')
                                         ELSE Ledger.Ell_DayCode 
                                     END [Day Code]
                                  , Scm_ShiftCode [Shift Code]
                                  , Scm_ShiftTimeIn [Shift Time In]
                                  , Scm_ShiftBreakStart [Break Start]
                                  , Scm_ShiftBreakEnd [Break End]
                                  , Scm_ShiftTimeOut [Shift Time Out]
                               FROM T_EmployeeLeaveNotice
                               LEFT JOIN T_LeaveTypeMaster
                                 ON Ltm_LeaveType = Eln_LeaveType
                               LEFT JOIN T_EmployeeLogLedger Ledger
                                 ON Ledger.Ell_EmployeeId = Eln_EmployeeId
                                AND Ledger.Ell_ProcessDate = Eln_LeaveDate
                               LEFT JOIN T_EmployeeLogLedgerHist LedgerHist
                                 ON LedgerHist.Ell_EmployeeId = Eln_EmployeeId
                                AND LedgerHist.Ell_ProcessDate = Eln_LeaveDate
                               LEFT JOIN T_EmployeeMaster 
                                 ON Emt_EmployeeId = Eln_EmployeeId
                               LEFT JOIN T_ShiftCodeMaster
                                 ON Scm_ShiftCode = CASE WHEN (Ledger.Ell_ProcessDate IS NULL)
                                                         THEN ISNULL(LedgerHist.Ell_ShiftCode, Emt_ShiftCode)
                                                         ELSE Ledger.Ell_ShiftCode
                                                     END
                               LEFT JOIN T_AccountDetail AD1 
                                 ON AD1.Adt_AccountCode = Eln_Status 
                                AND AD1.Adt_AccountType =  'WFSTATUS'
                               LEFT JOIN T_AccountDetail AD2
                                 ON AD2.Adt_AccountCode = Elt_Filler1
                                AND AD2.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler01')
                               LEFT JOIN T_AccountDetail AD3
                                 ON AD3.Adt_AccountCode = Elt_Filler2
                                AND AD3.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler02')
                               LEFT JOIN T_AccountDetail AD4
                                 ON AD4.Adt_AccountCode = Elt_Filler3
                                AND AD4.Adt_AccountType = (SELECT Cfl_Lookup FROM T_ColumnFiller WHERE Cfl_ColName = 'Elt_Filler03')
                               LEFT JOIN T_AccountDetail AD5
                                 ON AD5.Adt_AccountCode = Eln_LeaveCategory
                                AND AD5.Adt_AccountType = 'LVECATEGRY'
                              WHERE Eln_ControlNo = @ControlNo";
            sql = sql.Replace("@Elt_Filler1Desc", CommonMethods.getFillerName("Elt_Filler01"));
            sql = sql.Replace("@Elt_Filler2Desc", CommonMethods.getFillerName("Elt_Filler02"));
            sql = sql.Replace("@Elt_Filler3Desc", CommonMethods.getFillerName("Elt_Filler03"));
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
                    MessageBox.writeErrorLog(ex.Message, this.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsRecord.Tables[0].Rows[0];
        }

        public bool hasLeaveNotice(string employeeId)
        {
            int count = 0;
            string sql = string.Format(@" SELECT COUNT(Eln_ControlNo)
                                            FROM T_EmployeeLeaveNotice
                                           WHERE Eln_EmployeeId = '{0}'
                                             AND Eln_Status IN ('1')", employeeId);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    count = Convert.ToInt32(dal.ExecuteScalar(sql, CommandType.Text).ToString());
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
            return (count > 0);
        }

        public void updateCreditsFromChecklist(string controlNo, string userLogged, string type, DALHelper dal)
        {
            DataRow dr = getLeaveInfo(controlNo);
            bool checkCredits = false;
            if (Convert.ToDateTime(dr["Leave Date"].ToString()).Year.Equals(Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR"))))
            {
                //if (CommonMethods.isWithinCurrentYear(dtpLVDate.Date.ToString("MM/dd/yyy"), dal))
                //{
                checkCredits = true;
                //}
            }
            else if (Convert.ToDecimal(Convert.ToDateTime(dr["Leave Date"].ToString()).Year) > CommonMethods.getParamterValue("LEAVEYEAR"))
            {
                if (!methods.GetProcessControlFlag("LEAVE", "LVCRBYPASS"))
                {
                    checkCredits = true;
                }
            }
            if (checkCredits)
            {
                DataRow drCredits = PopulateDRforCredits();
                drCredits["Elt_ControlNo"] = controlNo;
                drCredits["Elt_EmployeeId"] = dr["ID No"].ToString();
                drCredits["Elt_LeaveType"] = dr["Leave Code"].ToString();
                drCredits["Usr_Login"] = session["userLogged"].ToString();
                //drCredits["Elt_LeaveHour"] = calculateLeaveHoursHours( dr["Start"].ToString().Replace(":", "")
                //                                                     , dr["End"].ToString().Replace(":", "")
                //                                                     , dr["Day Unit"].ToString()
                //                                                     , dr["Shift Code"].ToString()
                //                                                     , dr["Leave Code"].ToString()
                //                                                     , true);
                //instead of recalculating, use the shift hour filed by the employee
                //08-06-2013
                drCredits["Elt_LeaveHour"] = dr["Hours"].ToString().Trim() ;
                drCredits["Elt_LeaveDate"] = dr["Leave Date"].ToString();
                drCredits["Elt_LeaveType"] = dr["Leave Code"].ToString();
                drCredits["Elt_StartTime"] = dr["Start"].ToString().Replace(":", "");
                drCredits["Elt_EndTime"] = dr["End"].ToString().Replace(":", "");
                drCredits["Elt_DayUnit"] = dr["Day Unit"].ToString();
                if (type.ToUpper().Equals("NEW"))
                {
                    UpdateCredits("NEW", drCredits, dal);
                }
                else if (type.ToUpper().Equals("CANCEL"))
                {
                    UpdateCredits("CANCEL", drCredits, dal);
                }
                else if (type.ToUpper().Equals("APPROVE"))
                {
                    UpdateCredits("APPROVE", drCredits, dal);
                }
            }
        }

        //no reference
        //private decimal computeLeaveHours(string startTime, string endTime, string dayUnit, string shiftCode, string leaveType)
        //{
        //    //Logic is not yet finished for computations: LVEDEDUCTN parameter is not yet used from T_ParameterMasterExt
        //    decimal final = 0;//final varialble is handled as IN MINUTES, still need to convert for hours at return statement;
        //    DataSet dsShift = new DataSet();
        //    try
        //    {
        //        decimal start = 0;
        //        decimal end = 0;
        //        decimal paidBreak = 0;
        //        decimal i1 = 0;
        //        decimal o1 = 0;
        //        decimal i2 = 0;
        //        decimal o2 = 0;
        //        decimal hoursPerDay = 8;//default value

        //        start = Convert.ToDecimal(startTime.Substring(0, 2)) * 60 + Convert.ToDecimal(startTime.Substring(2, 2));
        //        end = Convert.ToDecimal(endTime.Substring(0, 2)) * 60 + Convert.ToDecimal(endTime.Substring(2, 2));

        //        dsShift = CommonMethods.getShiftInformation(shiftCode);

        //        i1 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString().Substring(2, 2));
        //        o1 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString().Substring(2, 2));
        //        i2 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString().Substring(2, 2));
        //        o2 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString().Substring(2, 2));

        //        hoursPerDay = Convert.ToDecimal(CommonMethods.getParamterValue("LHRSINDAY"));
        //        paidBreak = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_PaidBreak"].ToString());

        //        while (end < start)
        //        {
        //            end += 1440;
        //        }
        //        //for now hard coded that if leave entry is OFFICIAL BUSINESS, UNDERTIME, or DayUnit is Quarter Day leave hours
        //        //is computed other than if LVHRENTRY is true then computation is in hours for all || rblDayUnit.Items[2].Selected)
        //        if (methods.GetProcessControlFlag("LEAVE", "LVHRENTRY")
        //            || leaveType.Equals("OB")
        //            || leaveType.Equals("UN")
        //            || dayUnit.Equals("QR"))
        //        {
        //            final = end - start;
        //            if (start <= o1 && end >= i2)// ..|.B...B.|..
        //            {
        //                final = final - (i2 - o1);
        //            }
        //            else if (start <= o1 && (end > o2 && end <= i2))// ..|.B.|..B..
        //            {
        //                final = final - (end - o1);
        //            }
        //            else if ((start >= o1 && start < i2) && end >= i2)// ...B.|..B.|..
        //            {
        //                final = final - (i2 - start);
        //            }

        //            final = (final + paidBreak) / 60;
        //        }
        //        else//LVHRENTRY is FALSE: Leave is in days
        //        {
        //            #region OLD/Usual calculation
        //            //decimal mult = 0;
        //            //switch (dayUnit)
        //            //{
        //            //    case "WH":
        //            //        mult = 1;
        //            //        break;
        //            //    case "HA":
        //            //        mult = (hoursPerDay / 2) / hoursPerDay;
        //            //        break;
        //            //    case "HP":
        //            //        mult = (hoursPerDay / 2) / hoursPerDay;
        //            //        break;
        //            //    case "QR":
        //            //        mult = (hoursPerDay / 4) / hoursPerDay;
        //            //        break;
        //            //    default:
        //            //        break;
        //            //}

        //            //final = hoursPerDay * mult;
        //            #endregion
        //            #region CREDITS is divisble by LHRSINDAY
        //            //not yet coded
        //            #endregion
        //            #region SHIFT HOURS not equal to LHRSINDAY
        //            //no yet coded
        //            #endregion
        //            #region Other special case/company specific
        //            //CHIYODA Specific fixed hours on day unit
        //            switch (dayUnit)
        //            {
        //                case "WH":
        //                    final = 9;
        //                    break;
        //                case "HA":
        //                    final = 4;
        //                    break;
        //                case "HP":
        //                    final = 5;
        //                    break;
        //                case "QR":
        //                    final = 2;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
        //    }
        //    return final;
        //}

        public bool isPaidWithNoCreditsLeave(string leaveType)
        {
            bool flag = false;
            string sql = string.Format(@"SELECT CASE WHEN (Ltm_PaidLeave = 1 AND Ltm_WithCredit = 0)	
			                                         THEN 'TRUE'
			                                         ELSE 'FALSE'
	                                             END
                                                FROM T_LeavetypeMaster
                                               WHERE Ltm_LeaveType = '{0}' ", leaveType);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text).ToString());
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
            return flag;
        }

        public decimal calculateLeaveHoursHours(string startTime, string endTime, string dayUnit, string shiftCode, string leaveType, bool forCreditDeduction)
        {
            decimal final = 0;//final varialble is handled as IN MINUTES, still need to convert for hours at return statement;
            DataSet dsShift = new DataSet();
            try
            {
                decimal start = 0;
                decimal end = 0;
                decimal paidBreak = 0;
                decimal i1 = 0;
                decimal o1 = 0;
                decimal i2 = 0;
                decimal o2 = 0;
                start = Convert.ToDecimal(startTime.Substring(0, 2)) * 60 + Convert.ToDecimal(startTime.Substring(2, 2));
                end = Convert.ToDecimal(endTime.Substring(0, 2)) * 60 + Convert.ToDecimal(endTime.Substring(2, 2));

                dsShift = CommonMethods.getShiftInformation(shiftCode);

                i1 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"].ToString().Substring(2, 2));
                o1 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString().Substring(2, 2));
                i2 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString().Substring(2, 2));
                o2 = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString().Substring(0, 2)) * 60 + Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"].ToString().Substring(2, 2));
                paidBreak = Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_PaidBreak"].ToString());

                #region For Graveyard shifts
                while (end < start)
                {
                    end += 1440;
                }
                while (o1 < i1)
                {
                    o1 += 1440;
                }
                while (i2 < i1)
                {
                    i2 += 1440;
                }
                while (o2 < i1)
                {
                    o2 += 1440;
                }
                #endregion
                final = end - start;
                if (start <= o1 && end >= i2)// ..|.B...B.|..
                {
                    final = final - (i2 - o1);
                }
                else if (start <= o1 && (end > o2 && end <= i2))// ..|.B.|..B..
                {
                    final = final - (end - o1);
                }
                else if ((start >= o1 && start < i2) && end >= i2)// ...B.|..B.|..
                {
                    final = final - (i2 - start);
                }


                if (start >= o1 && start <= i2)
                {
                    paidBreak = paidBreak - (i2 - start);
                }
                else if (end >= o1 && end <= i2)
                {
                    paidBreak = paidBreak - (end - o1);
                }
                else if (start > i2 || end <= o1)
                {
                    paidBreak = 0;
                }

                if (paidBreak < 0)
                {
                    paidBreak = 0;
                }
                if (paidBreak != 0 && Convert.ToDecimal(startTime) == Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"].ToString()))
                {
                    final = (final + paidBreak) / 60;
                }
                else if (paidBreak != 0 && Convert.ToDecimal(endTime) == Convert.ToDecimal(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"].ToString()))
                {
                    final = (final) / 60;
                }
                else
                {
                    final = (final + paidBreak) / 60;
                }

                final = Math.Round(final, 2);
                //Denso does not matter with actual hours from Satrt and End Date
                if (Convert.ToBoolean(Resources.Resource.DENSOSPECIFIC))
                {
                    switch (dayUnit)
                    { 
                        case "WH":
                            final = 8;
                            break;
                        case "HP":
                            final = 4;
                            break;
                        case "HA":
                            final = 4;
                            break;
                        case "QR":
                            final = 2;
                            break;

                    }
                }

                if (forCreditDeduction)
                {
                    if ( !methods.GetProcessControlFlag("LEAVE", "LVHRENTRY")
                      && methods.GetProcessControlFlag("LEAVE", "DAYSEL")
                      && !leaveType.Equals("OB")
                      && !leaveType.Equals("UN")
                      && !dayUnit.Equals("QR"))
                    {
                        string sqlParemeterExt = string.Format(@" SELECT Pmx_ParameterValue
                                                                    FROM T_ParameterMasterExt
                                                                   WHERE Pmx_ParameterID = 'LVDEDUCTN'
                                                                     AND Pmx_Classification = '{0}' ", dayUnit);
                        decimal leaveValue = 0;

                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                leaveValue = Convert.ToDecimal(dal.ExecuteScalar(sqlParemeterExt, CommandType.Text));
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

                        if (!leaveValue.Equals(0))
                        {
                            final = leaveValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
                throw ex;
            }

            return final;
        }

        private DataRow PopulateDRforCredits()
        {
            DataRow dr = DbRecord.Generate("T_EmployeeLeaveAvailment");
            return dr;
        }
        //Saving Functions
        public int CreateLVRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[19];
            paramDetails[0] = new ParameterInfo("@Elt_CurrentPayPeriod", rowDetails["Elt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Elt_EmployeeId", rowDetails["Elt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Elt_LeaveDate", rowDetails["Elt_LeaveDate"]);
            paramDetails[3] = new ParameterInfo("@Elt_LeaveType", rowDetails["Elt_LeaveType"]);
            paramDetails[4] = new ParameterInfo("@Elt_StartTime", rowDetails["Elt_StartTime"]);
            paramDetails[5] = new ParameterInfo("@Elt_EndTime", rowDetails["Elt_EndTime"]);
            paramDetails[6] = new ParameterInfo("@Elt_LeaveHour", rowDetails["Elt_LeaveHour"]);
            paramDetails[7] = new ParameterInfo("@Elt_DayUnit", rowDetails["Elt_DayUnit"]);
            paramDetails[8] = new ParameterInfo("@Elt_Reason", rowDetails["Elt_Reason"]);
            paramDetails[9] = new ParameterInfo("@Elt_LeaveFlag", rowDetails["Elt_LeaveFlag"]);
            paramDetails[10] = new ParameterInfo("@Elt_InformDate", rowDetails["Elt_InformDate"]);
            paramDetails[11] = new ParameterInfo("@Elt_ControlNo", rowDetails["Elt_ControlNo"]);
            paramDetails[12] = new ParameterInfo("@Elt_Status", rowDetails["Elt_Status"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[14] = new ParameterInfo("@Elt_LeaveCategory", rowDetails["Elt_LeaveCategory"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Elt_LeaveNotice", rowDetails["Elt_LeaveNotice"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
DECLARE @Costcenter as varchar(10)
set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                        FROM T_EmployeeCostcenterMovement
                    WHERE Ecm_EmployeeID = @Elt_EmployeeId
                        AND @Elt_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                        --AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)

IF(@Costcenter IS NULL)
BEGIN
    SET @Costcenter = (SELECT Emt_CostcenterCode 
                            FROM T_EmployeeMaster
				        WHERE Emt_EmployeeID = @Elt_EmployeeId)
END

--apsungahid added 20141124
DECLARE @LineCode as varchar(15)
SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                    FROM E_EmployeeCostCenterLineMovement
                    WHERE Ecm_EmployeeID = @Elt_EmployeeId
                    AND @Elt_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                    AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)
IF(@LineCode IS NULL)
BEGIN
    SET @LineCode = ''
END

IF NOT EXISTS(SELECT Elt_ControlNo 
                FROM T_EmployeeLeaveAvailment
                WHERE Elt_CurrentPayPeriod = @Elt_CurrentPayPeriod
                AND Elt_EmployeeId = @Elt_EmployeeId
                AND Elt_LeaveDate = @Elt_LeaveDate
                AND Elt_LeaveType = @Elt_LeaveType
                AND Elt_StartTime = @Elt_StartTime
                AND Elt_EndTime = @Elt_EndTime
                AND Elt_LeaveHour = @Elt_LeaveHour
                AND Elt_DayUnit = @Elt_DayUnit
                AND Elt_Reason = @Elt_Reason
                AND Elt_LeaveFlag = @Elt_LeaveFlag
                AND Elt_Status = @Elt_Status
                AND Elt_LeaveCategory = @Elt_LeaveCategory
                AND Elt_Filler1 = @Elt_Filler1
                AND Elt_Filler2 = @Elt_Filler2
                AND Elt_Filler3 = @Elt_Filler3
                AND Elt_LeaveNotice = @Elt_LeaveNotice)
BEGIN
    INSERT INTO T_EmployeeLeaveAvailment
    (
            Elt_CurrentPayPeriod
        , Elt_EmployeeId
        , Elt_LeaveDate
        , Elt_LeaveType
        , Elt_StartTime
        , Elt_EndTime
        , Elt_LeaveHour
        , Elt_DayUnit
        , Elt_Reason
        , Elt_LeaveFlag
        , Elt_AppliedDate
        , Elt_InformDate
        , Elt_ControlNo
        , Elt_Status
        , Usr_Login
        , Ludatetime
        , Elt_LeaveCategory
        , Elt_Costcenter
        , Elt_CostcenterLine
        , Elt_Filler1
        , Elt_Filler2
        , Elt_Filler3
        , Elt_LeaveNotice
    )
    VALUES
    (
            @Elt_CurrentPayPeriod
        , @Elt_EmployeeId
        , @Elt_LeaveDate
        , @Elt_LeaveType
        , @Elt_StartTime
        , @Elt_EndTime
        , @Elt_LeaveHour
        , @Elt_DayUnit
        , @Elt_Reason
        , @Elt_LeaveFlag
        , getdate()
        , @Elt_InformDate
        , @Elt_ControlNo
        , @Elt_Status
        , @Usr_Login
        , getdate()
        , @Elt_LeaveCategory
        , @Costcenter
        , @LineCode
        , CASE WHEN (@Elt_LeaveType = 'SL' OR @Elt_LeaveCategory = 'SL')
                THEN @Elt_Filler1
                ELSE ''
            END
        , @Elt_Filler2
        , @Elt_Filler3
        , @Elt_LeaveNotice
    )

    IF @Elt_LeaveNotice <> 'True'
        EXEC UpdateLeaveCredits @Elt_ControlNo, 0, @Elt_LeaveHour, @Usr_Login, 0;
END";
            #endregion

            return dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }
        public int CreateLVRecord2(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[20];
            paramDetails[0] = new ParameterInfo("@Elt_CurrentPayPeriod", rowDetails["Elt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Elt_EmployeeId", rowDetails["Elt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Elt_LeaveDate", rowDetails["Elt_LeaveDate"]);
            paramDetails[3] = new ParameterInfo("@Elt_LeaveType", rowDetails["Elt_LeaveType"]);
            paramDetails[4] = new ParameterInfo("@Elt_StartTime", rowDetails["Elt_StartTime"]);
            paramDetails[5] = new ParameterInfo("@Elt_EndTime", rowDetails["Elt_EndTime"]);
            paramDetails[6] = new ParameterInfo("@Elt_LeaveHour", rowDetails["Elt_LeaveHour"]);
            paramDetails[7] = new ParameterInfo("@Elt_DayUnit", rowDetails["Elt_DayUnit"]);
            paramDetails[8] = new ParameterInfo("@Elt_Reason", rowDetails["Elt_Reason"]);
            paramDetails[9] = new ParameterInfo("@Elt_LeaveFlag", rowDetails["Elt_LeaveFlag"]);
            paramDetails[10] = new ParameterInfo("@Elt_InformDate", rowDetails["Elt_InformDate"]);
            paramDetails[11] = new ParameterInfo("@Elt_ControlNo", rowDetails["Elt_ControlNo"]);
            paramDetails[12] = new ParameterInfo("@Elt_Status", rowDetails["Elt_Status"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[14] = new ParameterInfo("@Elt_LeaveCategory", rowDetails["Elt_LeaveCategory"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Elt_LeaveNotice", rowDetails["Elt_LeaveNotice"]);
            paramDetails[19] = new ParameterInfo("@Elt_BatchNo", rowDetails["Elt_BatchNo"]);
            #endregion

            #region SQL Query
            string sqlInsert = @"
DECLARE @Costcenter as varchar(10)
set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                        FROM T_EmployeeCostcenterMovement
                    WHERE Ecm_EmployeeID = @Elt_EmployeeId
                        AND @Elt_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                        --AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)

IF(@Costcenter IS NULL)
BEGIN
    SET @Costcenter = (SELECT Emt_CostcenterCode 
                            FROM T_EmployeeMaster
				        WHERE Emt_EmployeeID = @Elt_EmployeeId)
END

--apsungahid added 20141124
DECLARE @LineCode as varchar(15)
SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                    FROM E_EmployeeCostCenterLineMovement
                    WHERE Ecm_EmployeeID = @Elt_EmployeeId
                    AND @Elt_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                    AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)
IF(@LineCode IS NULL)
BEGIN
    SET @LineCode = ''
END

IF NOT EXISTS(SELECT Elt_ControlNo 
                FROM T_EmployeeLeaveAvailment
                WHERE Elt_CurrentPayPeriod = @Elt_CurrentPayPeriod
                    AND Elt_EmployeeId = @Elt_EmployeeId
                    AND Elt_LeaveDate = @Elt_LeaveDate
                    AND Elt_LeaveType = @Elt_LeaveType
                    AND Elt_StartTime = @Elt_StartTime
                    AND Elt_EndTime = @Elt_EndTime
                    AND Elt_LeaveHour = @Elt_LeaveHour
                    AND Elt_DayUnit = @Elt_DayUnit
                    AND Elt_Reason = @Elt_Reason
                    AND Elt_LeaveFlag = @Elt_LeaveFlag
                    AND Elt_Status = @Elt_Status
                    AND Elt_LeaveCategory = @Elt_LeaveCategory
                    AND Elt_Filler1 = @Elt_Filler1
                    AND Elt_Filler2 = @Elt_Filler2
                    AND Elt_Filler3 = @Elt_Filler3
                    AND Elt_LeaveNotice = @Elt_LeaveNotice)
BEGIN
    INSERT INTO T_EmployeeLeaveAvailment
    (
            Elt_CurrentPayPeriod
        , Elt_EmployeeId
        , Elt_LeaveDate
        , Elt_LeaveType
        , Elt_StartTime
        , Elt_EndTime
        , Elt_LeaveHour
        , Elt_DayUnit
        , Elt_Reason
        , Elt_LeaveFlag
        , Elt_AppliedDate
        , Elt_InformDate
        , Elt_ControlNo
        , Elt_Status
        , Usr_Login
        , Ludatetime
        , Elt_LeaveCategory
        , Elt_Costcenter
        , Elt_CostcenterLine
        , Elt_Filler1
        , Elt_Filler2
        , Elt_Filler3
        , Elt_LeaveNotice
        , Elt_BatchNo
    )
    VALUES
    (
            @Elt_CurrentPayPeriod
        , @Elt_EmployeeId
        , @Elt_LeaveDate
        , @Elt_LeaveType
        , @Elt_StartTime
        , @Elt_EndTime
        , @Elt_LeaveHour
        , @Elt_DayUnit
        , @Elt_Reason
        , @Elt_LeaveFlag
        , getdate()
        , @Elt_InformDate
        , @Elt_ControlNo
        , @Elt_Status
        , @Usr_Login
        , getdate()
        , @Elt_LeaveCategory
        , @Costcenter
        , @LineCode
        , CASE WHEN (@Elt_LeaveType = 'SL' OR @Elt_LeaveCategory = 'SL')
                THEN @Elt_Filler1
                ELSE ''
            END
        , @Elt_Filler2
        , @Elt_Filler3
        , @Elt_LeaveNotice
        , @Elt_BatchNo
    )

    EXEC UpdateLeaveCredits @Elt_ControlNo, 0, @Elt_LeaveHour, @Usr_Login, 0;
END";
            #endregion

            return dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
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

        public void UpdateLVRecordSave(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[19];
            paramDetails[0] = new ParameterInfo("@Elt_CurrentPayPeriod", rowDetails["Elt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Elt_EmployeeId", rowDetails["Elt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Elt_LeaveDate", rowDetails["Elt_LeaveDate"]);
            paramDetails[3] = new ParameterInfo("@Elt_LeaveType", rowDetails["Elt_LeaveType"]);
            paramDetails[4] = new ParameterInfo("@Elt_StartTime", rowDetails["Elt_StartTime"]);
            paramDetails[5] = new ParameterInfo("@Elt_EndTime", rowDetails["Elt_EndTime"]);
            paramDetails[6] = new ParameterInfo("@Elt_LeaveHour", rowDetails["Elt_LeaveHour"]);
            paramDetails[7] = new ParameterInfo("@Elt_DayUnit", rowDetails["Elt_DayUnit"]);
            paramDetails[8] = new ParameterInfo("@Elt_Reason", rowDetails["Elt_Reason"]);
            paramDetails[9] = new ParameterInfo("@Elt_LeaveFlag", rowDetails["Elt_LeaveFlag"]);
            paramDetails[10] = new ParameterInfo("@Elt_InformDate", rowDetails["Elt_InformDate"]);
            paramDetails[11] = new ParameterInfo("@Elt_ControlNo", rowDetails["Elt_ControlNo"]);
            paramDetails[12] = new ParameterInfo("@Elt_Status", rowDetails["Elt_Status"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[14] = new ParameterInfo("@Elt_LeaveCategory", rowDetails["Elt_LeaveCategory"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Elt_LeaveNotice", rowDetails["Elt_LeaveNotice"]);
            #endregion

            #region SQL Query
            string sqlUpdate = @"
DECLARE @LeaveHours AS DECIMAL(5,2)
DECLARE @PendingLeaveHours AS DECIMAL(7,4)

SELECT @LeaveHours = Elt_LeaveHour
FROM T_EmployeeLeaveAvailment
WHERE Elt_ControlNo = @Elt_ControlNo

SET @PendingLeaveHours = @LeaveHours * -1
EXEC UpdateLeaveCredits @Elt_ControlNo, 0, @PendingLeaveHours, @Usr_Login, 0;

UPDATE T_EmployeeLeaveAvailment
SET   Elt_CurrentPayPeriod = @Elt_CurrentPayPeriod
    , Elt_EmployeeId = @Elt_EmployeeId
    , Elt_LeaveDate = @Elt_LeaveDate
    , Elt_LeaveType = @Elt_LeaveType
    , Elt_StartTime = @Elt_StartTime
    , Elt_EndTime = @Elt_EndTime
    , Elt_LeaveHour = @Elt_LeaveHour
    , Elt_DayUnit = @Elt_DayUnit
    , Elt_Reason = @Elt_Reason
    , Elt_LeaveFlag = @Elt_LeaveFlag
    , Elt_InformDate = @Elt_InformDate
    , Elt_ControlNo = @Elt_ControlNo
    , Usr_Login = @Usr_Login
    , Ludatetime = getdate()
    , Elt_LeaveCategory = @Elt_LeaveCategory
    , Elt_Filler1 = CASE WHEN (@Elt_LeaveType = 'SL' OR @Elt_LeaveCategory = 'SL')
                    THEN @Elt_Filler1
                    ELSE ''
                    END
    , Elt_Filler2 = @Elt_Filler2
    , Elt_Filler3 = @Elt_Filler3
WHERE Elt_ControlNo = @Elt_ControlNo

EXEC UpdateLeaveCredits @Elt_ControlNo, 0, @Elt_LeaveHour, @Usr_Login, 0;";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }
        public void UpdateLVRecordSaveBatch(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[19];
            paramDetails[0] = new ParameterInfo("@Elt_CurrentPayPeriod", rowDetails["Elt_CurrentPayPeriod"]);
            paramDetails[1] = new ParameterInfo("@Elt_EmployeeId", rowDetails["Elt_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Elt_LeaveDate", rowDetails["Elt_LeaveDate"]);
            paramDetails[3] = new ParameterInfo("@Elt_LeaveType", rowDetails["Elt_LeaveType"]);
            paramDetails[4] = new ParameterInfo("@Elt_StartTime", rowDetails["Elt_StartTime"]);
            paramDetails[5] = new ParameterInfo("@Elt_EndTime", rowDetails["Elt_EndTime"]);
            paramDetails[6] = new ParameterInfo("@Elt_LeaveHour", rowDetails["Elt_LeaveHour"]);
            paramDetails[7] = new ParameterInfo("@Elt_DayUnit", rowDetails["Elt_DayUnit"]);
            paramDetails[8] = new ParameterInfo("@Elt_Reason", rowDetails["Elt_Reason"]);
            paramDetails[9] = new ParameterInfo("@Elt_LeaveFlag", rowDetails["Elt_LeaveFlag"]);
            paramDetails[10] = new ParameterInfo("@Elt_InformDate", rowDetails["Elt_InformDate"]);
            paramDetails[11] = new ParameterInfo("@Elt_BatchNo", rowDetails["Elt_BatchNo"]);
            paramDetails[12] = new ParameterInfo("@Elt_Status", rowDetails["Elt_Status"]);
            paramDetails[13] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[14] = new ParameterInfo("@Elt_LeaveCategory", rowDetails["Elt_LeaveCategory"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[16] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[17] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[18] = new ParameterInfo("@Elt_LeaveNotice", rowDetails["Elt_LeaveNotice"]);
            #endregion

            #region SQL Query
            string sqlUpdate = @"
                                UPDATE T_EmployeeLeaveAvailment
                                SET   Elt_CurrentPayPeriod = @Elt_CurrentPayPeriod
                                    , Elt_EmployeeId = @Elt_EmployeeId
                                    , Elt_LeaveDate = @Elt_LeaveDate
                                    , Elt_LeaveType = @Elt_LeaveType
                                    , Elt_StartTime = @Elt_StartTime
                                    , Elt_EndTime = @Elt_EndTime
                                    , Elt_LeaveHour = @Elt_LeaveHour
                                    , Elt_DayUnit = @Elt_DayUnit
                                    , Elt_Reason = @Elt_Reason
                                    , Elt_LeaveFlag = @Elt_LeaveFlag
                                    , Elt_InformDate = @Elt_InformDate
                                    , Elt_ControlNo = @Elt_ControlNo
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = getdate()
                                    , Elt_LeaveCategory = @Elt_LeaveCategory
                                    , Elt_Filler1 = CASE WHEN (@Elt_LeaveType = 'SL' OR @Elt_LeaveCategory = 'SL')
                                                         THEN @Elt_Filler1
                                                         ELSE ''
                                                     END
                                    , Elt_Filler2 = @Elt_Filler2
                                    , Elt_Filler3 = @Elt_Filler3
                                WHERE Elt_BatchNo = @Elt_BatchNo
                                and Elt_EmployeeId = @Elt_EmployeeId
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }
        public void UpdateLVRecord(DataRow rowDetails, DALHelper dal)
        {
            UpdateLVRecord(string.Empty, rowDetails, dal);
        }

        public void UpdateLVRecord(string buttonName, DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (rowDetails["Elt_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Elt_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Elt_CheckedBy", " ");
            }
            else
            {
                if (!buttonName.Equals(string.Empty) && buttonName.Equals("ENDORSE TO CHECKER 1"))
                {
                    fieldUsed = @"Elt_EndorsedDateToChecker = getdate(),";
                    paramDetails[0] = new ParameterInfo("@Elt_CheckedBy", " ");
                }
                else if (rowDetails["Elt_Status"].ToString().Equals("5")
                    || rowDetails["Elt_Status"].ToString().Equals("4"))
                {
                    fieldUsed = @"  Elt_CheckedBy = @Elt_CheckedBy ,
                                Elt_CheckedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Elt_CheckedBy", rowDetails["Elt_CheckedBy"]);
                }
                else if (rowDetails["Elt_Status"].ToString().Equals("7")
                    || rowDetails["Elt_Status"].ToString().Equals("6"))
                {
                    fieldUsed = @"  Elt_Checked2By = @Elt_Checked2By ,
                                Elt_Checked2Date = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Elt_Checked2By", rowDetails["Elt_Checked2By"]);
                }
                else if (rowDetails["Elt_Status"].ToString().Equals("9")
                    || rowDetails["Elt_Status"].ToString().Equals("8"))
                {
                    fieldUsed = @"  Elt_ApprovedBy = @Elt_ApprovedBy ,
                                Elt_ApprovedDate = getdate() ,";
                    paramDetails[0] = new ParameterInfo("@Elt_ApprovedBy", rowDetails["Elt_ApprovedBy"]);
                }
                else
                {
                    paramDetails[0] = new ParameterInfo("@Elt_ApprovedBy", " ");
                }
            }
            paramDetails[1] = new ParameterInfo("@Elt_Status", rowDetails["Elt_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Elt_ControlNo", rowDetails["Elt_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlUpdate = string.Format(@"
                                    UPDATE T_EmployeeLeaveAvailment
                                    SET
                                        {0}
                                        Elt_Status = @Elt_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Elt_ControlNo = @Elt_ControlNo
                                    AND Elt_Status != '2'", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateCredits(string transactionType, DataRow rowDetails, DALHelper dal)
        {
            decimal dToDeduct = Convert.ToDecimal(rowDetails["Elt_LeaveHour"]);
            try
            {
                dToDeduct = Convert.ToDecimal(rowDetails["CreditsToDeduct"]);
                dToDeduct = Decimal.Round(dToDeduct, 2);
            }
            catch
            {
                dToDeduct = Convert.ToDecimal(rowDetails["Elt_LeaveHour"]);
                dToDeduct = Decimal.Round(dToDeduct, 2);
            }
            UpdateCredits(transactionType
                            , rowDetails["Elt_ControlNo"].ToString()
                            , rowDetails["Elt_EmployeeId"].ToString()
                            , rowDetails["Elt_LeaveType"].ToString()
                            , rowDetails["Elt_LeaveDate"].ToString()
                            , dToDeduct.ToString()
                            , rowDetails["Usr_Login"].ToString()
                            , dal);
        }

        public void UpdateCredits(string transactionType, string controlNo, string empID, string type, string leaveDate, string hour, string user, DALHelper dal)
        {
            string strleaveyear = string.Format("{0}", GetYear(Convert.ToDateTime(leaveDate), type != string.Empty ? type.Substring(0, 2) : string.Empty));
            decimal dUsedLeaveHours = 0;
            decimal dPendingLeaveHours = 0;

            #region Get Leave Hours to Deduct Based on Transaction Type
            if (transactionType.Equals("NEW"))
            {
                dUsedLeaveHours = 0;
                dPendingLeaveHours = Convert.ToDecimal(hour);
            }
            else if (transactionType.Equals("APPROVE"))
            {
                dUsedLeaveHours = Convert.ToDecimal(hour);
                dPendingLeaveHours = Convert.ToDecimal(hour) * -1;
            }
            else if (transactionType.Equals("CANCEL"))
            {
                dUsedLeaveHours = 0;
                dPendingLeaveHours = Convert.ToDecimal(hour) * -1;
            }
            else if (transactionType.Equals("APPROVECANCEL"))
            {
                dUsedLeaveHours = Convert.ToDecimal(hour) * -1;
                dPendingLeaveHours = 0;
            }
            #endregion

            #region Check if Leave Credit Deduction is Allowed or Not
            #region Query
            string strQueryCheck = string.Format(@"
DECLARE @ControlNo VARCHAR(12) = '{0}'
DECLARE @EmployeeID VARCHAR(15) = '{1}'
DECLARE @LeaveType VARCHAR(2) = '{2}'
DECLARE @LeaveDate DATETIME = '{3}'
DECLARE @leaveyear VARCHAR(4) = '{4}'
DECLARE @UsedLeaveHours DECIMAL(7,4) = {5}
DECLARE @PendingLeaveHours DECIMAL(7,4) = {6}
DECLARE @Usr_Login VARCHAR(15) = '{7}'
-----------------------------------------------

DECLARE @WithCredit AS BIT
DECLARE @PartOf AS CHAR(2)

SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
SET @PartOf =     (SELECT CASE WHEN ISNULL(Ltm_PartOfLeave,'') = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                     FROM T_LeaveTypeMaster
                    WHERE Ltm_LeaveType = @LeaveType)

IF (RTRIM(@leaveyear) = '')
BEGIN

    IF (@WithCredit = 1)
        BEGIN
            IF (@PartOf <> @LeaveType)
                BEGIN
                    SELECT Elm_LeaveType
						, Elm_Used = Elm_Used + @UsedLeaveHours
						, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
						, Elm_Balance = Elm_Entitled - (Elm_Used + @UsedLeaveHours) - (Elm_Reserved + @PendingLeaveHours) {8}
					FROM T_EmployeeLeave
                     WHERE Elm_EmployeeId = @EmployeeID AND Elm_LeaveType = @PartOf
                    
                END
				SELECT Elm_LeaveType
						, Elm_Used = Elm_Used + @UsedLeaveHours
						, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
						, Elm_Balance = Elm_Entitled - (Elm_Used + @UsedLeaveHours) - (Elm_Reserved + @PendingLeaveHours) {8}
				FROM T_EmployeeLeave
				WHERE Elm_EmployeeId = @EmployeeID AND Elm_LeaveType = @LeaveType
        END
END
ELSE
BEGIN

    IF (@WithCredit = 1)
        BEGIN
            IF (@PartOf <> @LeaveType)
                BEGIN
                     SELECT Elm_LeaveType
						, Elm_Used = Elm_Used + @UsedLeaveHours
						, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
						, Elm_Balance = Elm_Entitled - (Elm_Used + @UsedLeaveHours) - (Elm_Reserved + @PendingLeaveHours) {8}
					 FROM T_EmployeeLeave
                     WHERE Elm_EmployeeId = @EmployeeID AND Elm_LeaveType = @PartOf
                     AND Elm_LeaveYear = @leaveyear
                END
				SELECT Elm_LeaveType
						, Elm_Used = Elm_Used + @UsedLeaveHours
						, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
						, Elm_Balance = Elm_Entitled - (Elm_Used + @UsedLeaveHours) - (Elm_Reserved + @PendingLeaveHours) {8}
				FROM T_EmployeeLeave
				WHERE Elm_EmployeeId = @EmployeeID AND Elm_LeaveType = @LeaveType
				AND Elm_LeaveYear = @leaveyear
				
        END
END", controlNo
    , empID
    , type
    , leaveDate
    , strleaveyear
    , dUsedLeaveHours
    , dPendingLeaveHours
    , user
    , (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC)) == true? "- Elm_Reimbursed": "");
            #endregion

            DataSet ds = dal.ExecuteDataSet(strQueryCheck);
            string strErrorMsg = "";

            if (ds != null
                && ds.Tables.Count > 0
                && ds.Tables[0].Rows.Count > 0) //For Part-of Leave Type OR Parent Leave Type
            {
                if ((transactionType == "APPROVE" || transactionType == "APPROVECANCEL")
                    && Convert.ToDecimal(ds.Tables[0].Rows[0]["Elm_Used"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} used credits will be negative. ", ds.Tables[0].Rows[0]["Elm_LeaveType"]);
                }
                if ((transactionType == "NEW" || transactionType == "APPROVE" || transactionType == "CANCEL")
                    && Convert.ToDecimal(ds.Tables[0].Rows[0]["Elm_Reserved"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} pending credits will be negative. ", ds.Tables[0].Rows[0]["Elm_LeaveType"]);
                }
                if (Convert.ToDecimal(ds.Tables[0].Rows[0]["Elm_Balance"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} balance will be negative. ", ds.Tables[0].Rows[0]["Elm_LeaveType"]);
                }
            }
            if (ds != null
                && ds.Tables.Count > 1
                && ds.Tables[1].Rows.Count > 0) //For Parent Leave Type (if there is a Part-of Leave Type)
            {
                if ((transactionType == "APPROVE" || transactionType == "APPROVECANCEL")
                    && Convert.ToDecimal(ds.Tables[1].Rows[0]["Elm_Used"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} used credits will be negative. ", ds.Tables[1].Rows[0]["Elm_LeaveType"]);
                }
                if ((transactionType == "NEW" || transactionType == "APPROVE" || transactionType == "CANCEL")
                    && Convert.ToDecimal(ds.Tables[1].Rows[0]["Elm_Reserved"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} pending credits will be negative. ", ds.Tables[1].Rows[0]["Elm_LeaveType"]);
                }
                if (Convert.ToDecimal(ds.Tables[1].Rows[0]["Elm_Balance"].ToString()) < 0)
                {
                    strErrorMsg += string.Format("{0} balance will be negative. ", ds.Tables[0].Rows[1]["Elm_LeaveType"]);
                }
            }

            if (strErrorMsg != "")
                throw new Exception("Invalid Transaction. " + strErrorMsg);
            #endregion

            #region Update Leave Credits
            #region Query
            string strQueryUpdate = string.Format(@"
DECLARE @ControlNo VARCHAR(12) = '{0}'
DECLARE @EmployeeID VARCHAR(15) = '{1}'
DECLARE @LeaveType VARCHAR(2) = '{2}'
DECLARE @LeaveDate DATETIME = '{3}'
DECLARE @leaveyear VARCHAR(4) = '{4}'
DECLARE @UsedLeaveHours DECIMAL(7,4) = {5}
DECLARE @PendingLeaveHours DECIMAL(7,4) = {6}
DECLARE @Usr_Login VARCHAR(15) = '{7}'
DECLARE @Ludatetime DATETIME = GETDATE()
-----------------------------------------------

DECLARE @WithCredit AS BIT
DECLARE @PartOf AS CHAR(2)
DECLARE @LeaveTrailUsedHours AS DECIMAL(7,4)
DECLARE @LeaveTrailPendingHours AS DECIMAL(7,4)
SET @LeaveTrailUsedHours = ISNULL((SELECT TOP 1 Elm_Hours
									FROM T_EmployeeLeaveTrail
									WHERE Remarks = @ControlNo
										AND Elm_Action = 'AU'
										AND Elm_SystemGenerated = 1
									ORDER BY Ludatetime DESC), 0)
SET @LeaveTrailPendingHours = ISNULL((SELECT TOP 1 Elm_Hours
									FROM T_EmployeeLeaveTrail
									WHERE Remarks = @ControlNo
										AND Elm_Action = 'AP'
										AND Elm_SystemGenerated = 1
									ORDER BY Ludatetime DESC), 0)

SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
SET @PartOf =     (SELECT CASE WHEN ISNULL(Ltm_PartOfLeave,'') = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                        FROM T_LeaveTypeMaster
                    WHERE Ltm_LeaveType = @LeaveType)

IF (RTRIM(@leaveyear) = '')
BEGIN
    IF (@WithCredit = 1)
	    BEGIN
		    IF (@PartOf <> @LeaveType)
			    BEGIN
				    UPDATE T_EmployeeLeave
				    SET  Elm_Used = CASE WHEN @LeaveTrailUsedHours <> @UsedLeaveHours
									THEN Elm_Used + @UsedLeaveHours
									ELSE Elm_Used
									END
						, Elm_Reserved = CASE WHEN @LeaveTrailPendingHours <> @PendingLeaveHours
											THEN Elm_Reserved + @PendingLeaveHours
											ELSE Elm_Reserved
											END
					    , Usr_Login = @Usr_Login
					    , Ludatetime = @Ludatetime
				    WHERE elm_employeeid = @EmployeeID AND elm_leavetype = @PartOf

					-----INSERT LEAVE TRAIL-----
					IF (@UsedLeaveHours <> 0)
					BEGIN
						INSERT INTO T_EmployeeLeaveTrail
							(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
							,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
						SELECT '',@EmployeeID,@PartOf,@LeaveDate,@UsedLeaveHours
							,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AU',1,@Usr_Login,@Ludatetime
					END
					IF (@PendingLeaveHours <> 0)
					BEGIN
						INSERT INTO T_EmployeeLeaveTrail
							(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
							,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
						SELECT '',@EmployeeID,@PartOf,@LeaveDate,@PendingLeaveHours
							,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AP',1,@Usr_Login,@Ludatetime
					END
			    END
				---------------------------------------------------------------
				UPDATE T_EmployeeLeave
				SET   Elm_Used = Elm_Used + @UsedLeaveHours
					, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
					, Usr_Login = @Usr_Login
					, Ludatetime = @Ludatetime
				WHERE elm_employeeid = @EmployeeID AND elm_leavetype = @LeaveType

				-----INSERT LEAVE TRAIL-----
				IF (@UsedLeaveHours <> 0)
				BEGIN
					INSERT INTO T_EmployeeLeaveTrail
						(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
						,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
					SELECT '',@EmployeeID,@LeaveType,@LeaveDate,@UsedLeaveHours
						,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AU',1,@Usr_Login,@Ludatetime
				END
				IF (@PendingLeaveHours <> 0)
				BEGIN
					INSERT INTO T_EmployeeLeaveTrail
						(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
						,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
					SELECT '',@EmployeeID,@LeaveType,@LeaveDate,@PendingLeaveHours
						,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AP',1,@Usr_Login,@Ludatetime
				END
	    END
END 
ELSE
BEGIN
    IF (@WithCredit = 1)
	    BEGIN
		    IF (@PartOf <> @LeaveType)
			    BEGIN
				    UPDATE T_EmployeeLeave
				    SET   Elm_Used = Elm_Used + @UsedLeaveHours
						, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
					    , Usr_Login = @Usr_Login
					    , Ludatetime = @Ludatetime
				    WHERE elm_employeeid = @EmployeeID AND elm_leavetype = @PartOf
                        AND Elm_LeaveYear = @leaveyear

					-----INSERT LEAVE TRAIL-----
					IF (@UsedLeaveHours <> 0)
					BEGIN
						INSERT INTO T_EmployeeLeaveTrail
							(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
							,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
						SELECT @leaveyear,@EmployeeID,@PartOf,@LeaveDate,@UsedLeaveHours
							,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AU',1,@Usr_Login,@Ludatetime
					END
					IF (@PendingLeaveHours <> 0)
					BEGIN
						INSERT INTO T_EmployeeLeaveTrail
							(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
							,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
						SELECT @leaveyear,@EmployeeID,@PartOf,@LeaveDate,@PendingLeaveHours
							,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AP',1,@Usr_Login,@Ludatetime
					END
			    END    
				---------------------------------------------------------------
				UPDATE T_EmployeeLeave
				SET   Elm_Used = Elm_Used + @UsedLeaveHours
					, Elm_Reserved = Elm_Reserved + @PendingLeaveHours
					, Usr_Login = @Usr_Login
					, Ludatetime = @Ludatetime
				WHERE elm_employeeid = @EmployeeID AND elm_leavetype = @LeaveType
					AND Elm_LeaveYear = @leaveyear

				-----INSERT LEAVE TRAIL-----
				IF (@UsedLeaveHours <> 0)
				BEGIN
					INSERT INTO T_EmployeeLeaveTrail
						(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
						,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
					SELECT @leaveyear,@EmployeeID,@LeaveType,@LeaveDate,@UsedLeaveHours
						,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AU',1,@Usr_Login,@Ludatetime
				END
				IF (@PendingLeaveHours <> 0)
				BEGIN
					INSERT INTO T_EmployeeLeaveTrail
						(Elm_LeaveYear,Elm_EmployeeId,Elm_Leavetype,Elm_TransactDate,Elm_Hours
						,Remarks,Elm_LeaveMonth,Elm_Action,Elm_SystemGenerated,Usr_Login,Ludatetime)
					SELECT @leaveyear,@EmployeeID,@LeaveType,@LeaveDate,@PendingLeaveHours
						,@ControlNo,CONVERT(VARCHAR(2),@LeaveDate,101),'AP',1,@Usr_Login,@Ludatetime
				END
	    END
END", controlNo
    , empID
    , type
    , leaveDate
    , strleaveyear
    , dUsedLeaveHours
    , dPendingLeaveHours
    , user);
            #endregion

            dal.ExecuteNonQuery(strQueryUpdate); 
            #endregion
        }

        public bool isCreditsDeductable(string transactionType, string ControlNo, DALHelper dal)
        {
            bool ret = true;
            DataSet ds = dal.ExecuteDataSet(string.Format(@"
SELECT
	Elt_EmployeeID
    , CONVERT(VARCHAR(20), Elt_LeaveDate, 101) [Elt_LeaveDate]
	, Elt_LeaveType
	, Elt_StartTime
	, Elt_EndTime
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveAvailment
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Elt_LeaveType = Ltm_LeaveType
WHERE Elt_ControlNo = '{0}'

UNION

SELECT
	Elt_EmployeeID
    , CONVERT(VARCHAR(20), Elt_LeaveDate, 101) [Elt_LeaveDate]
	, Elt_LeaveType
	, Elt_StartTime
	, Elt_EndTime
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Elt_LeaveHour
					ELSE 
						CASE WHEN Elt_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
						END
				END
			ELSE
			Elt_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveAvailmentHist
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Elt_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Elt_LeaveType = Ltm_LeaveType
WHERE Elt_ControlNo = '{0}'
            ", ControlNo), CommandType.Text);
            if (!CommonMethods.isEmpty(ds))
            {
                ret = isCreditsDeductable(
                            transactionType
                            , string.Format("{0}", GetYear(Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()), ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()))
                            , ds.Tables[0].Rows[0]["Elt_EmployeeID"].ToString().Trim()
                            , ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()
                            , ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()
                            , dal);

            }
            else
            {
                throw new Exception("Leave transaction was not found for control no : " + ControlNo);
            }
            return ret;
        }

        public bool isCreditsDeductableFromNotice(string transactionType, string ControlNo, DALHelper dal)
        {
            bool ret = true;
            DataSet ds = dal.ExecuteDataSet(string.Format(@"
SELECT
	Eln_EmployeeID [Elt_EmployeeID]
    , CONVERT(VARCHAR(20), Eln_LeaveDate, 101) [Elt_LeaveDate]
	, Eln_LeaveType [Elt_LeaveType]
	, Eln_StartTime [Elt_StartTime]
	, Eln_EndTime [Elt_EndTime]
    , Eln_DayUnit [Elt_DayUnit]
	, CASE WHEN Pcm_ProcessFlag = 1
			THEN
				CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
					THEN Eln_LeaveHour
					ELSE 
						CASE WHEN Eln_LeaveHour < 0
							THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
							ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
						END
				END
			ELSE
			Eln_LeaveHour	
		END	[Elt_LeaveHour]
FROM T_EmployeeLeaveNotice
LEFT JOIN T_ProcessControlMaster
	ON Pcm_ProcessID = 'DAYSEL'
	AND Pcm_SystemID = 'LEAVE'
LEFT JOIN T_ParameterMasterExt
	ON Pmx_ParameterID = 'LVDEDUCTN'
	AND Pmx_Classification = Eln_DayUnit
LEFT JOIN T_LeaveTypeMaster
	ON Eln_LeaveType = Ltm_LeaveType
WHERE Eln_ControlNo = '{0}'
            ", ControlNo), CommandType.Text);
            if (!CommonMethods.isEmpty(ds))
            {
                ret = isCreditsDeductable(
                            transactionType
                            , string.Format("{0}", GetYear(Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()), ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()))
                            , ds.Tables[0].Rows[0]["Elt_EmployeeID"].ToString().Trim()
                            , ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()
                            , ds.Tables[0].Rows[0]["Elt_LeaveHour"].ToString().Trim()
                            , dal);

            }
            else
            {
                throw new Exception("Leave Notice transaction was not found for control no : " + ControlNo);
            }
            return ret;
        }

        public bool isCreditsDeductable(string transactionType, string leaveYear, string employeeID, string leaveType, string startTime, string endTime, string dayUnit, string shiftCode, DALHelper dal)
        {
            bool ret = true;
            decimal dHours = calculateLeaveHoursHours(startTime, endTime, dayUnit, shiftCode, leaveType, true);
            ParameterInfo[] paramDetails = new ParameterInfo[4];
            paramDetails[0] = new ParameterInfo("@EmployeeID", employeeID);
            paramDetails[1] = new ParameterInfo("@LeaveType", leaveType);
            paramDetails[2] = new ParameterInfo("@LeaveHours", dHours);
            paramDetails[3] = new ParameterInfo("@leaveyear", leaveYear);

            string transaction = string.Empty;
            string transactionCondition = string.Empty;
            if (transactionType.Equals("NEW"))
            {
                transaction = "Elm_Reserved = Elm_Reserved + @LeaveHours";
                transactionCondition = @" AND ( Elm_Reserved + @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - (Elm_Reserved + @LeaveHours)) < 0 )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Reserved + @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - (Elm_Reserved + @LeaveHours)) < 0 )";
                }
            }
            else if (transactionType.Equals("APPROVE"))
            {
                transaction = @"Elm_Reserved = Elm_Reserved - @LeaveHours
                                   , Elm_Used = Elm_Used + @LeaveHours";
                transactionCondition = @" AND ((Elm_Reserved - @LeaveHours < 0 OR Elm_Used + @LeaveHours < 0)  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used + @LeaveHours) - (Elm_Reserved - @LeaveHours)) < 0  )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ((Elm_Reserved - @LeaveHours < 0 OR Elm_Used + Elm_Reimbursed + @LeaveHours < 0)  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used + Elm_Reimbursed + @LeaveHours) - (Elm_Reserved - @LeaveHours)) < 0  )";
                }
            }
            else if (transactionType.Equals("CANCEL"))
            {
                transaction = "Elm_Reserved = Elm_Reserved - @LeaveHours";
                transactionCondition = @" AND ( Elm_Reserved - @LeaveHours < 0  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - (Elm_Reserved - @LeaveHours)) < 0 )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Reserved - @LeaveHours < 0  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - (Elm_Reserved - @LeaveHours)) < 0 )";
                }
            }
            else if (transactionType.Equals("APPROVECANCEL"))
            {
                transaction = "Elm_Used = Elm_Used - @LeaveHours";
                transactionCondition = @" AND ( Elm_Used - @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used - @LeaveHours) - Elm_Reserved) < 0  )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Used - @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used - @LeaveHours) - Elm_Reimbursed - Elm_Reserved) < 0  )";
                }
            }

            DataSet ds = dal.ExecuteDataSet(string.Format(@"
DECLARE @WithCredit AS BIT
DECLARE @PartOf AS CHAR(2)

SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
SET @PartOf =     (SELECT CASE WHEN Ltm_PartOfLeave = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                     FROM T_LeaveTypeMaster
                    WHERE Ltm_LeaveType = @LeaveType)
                   
IF (RTRIM(@Leaveyear) <> '')
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
ELSE 
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
            ", transactionCondition), CommandType.Text, paramDetails);
            if (!CommonMethods.isEmpty(ds))
            {
                ret = false;
            }
            return ret;
        }

        public bool isCreditsDeductable(string transactionType, string leaveYear, string employeeID, string leaveType, string leavehours, DALHelper dal)
        {
            bool ret = true;
            ParameterInfo[] paramDetails = new ParameterInfo[4];
            paramDetails[0] = new ParameterInfo("@EmployeeID", employeeID);
            paramDetails[1] = new ParameterInfo("@LeaveType", leaveType);
            paramDetails[2] = new ParameterInfo("@LeaveHours", leavehours);
            paramDetails[3] = new ParameterInfo("@leaveyear", leaveYear);

            string transaction = string.Empty;
            string transactionCondition = string.Empty;
            if (transactionType.Equals("NEW"))
            {
                transaction = "Elm_Reserved = Elm_Reserved + @LeaveHours";
                transactionCondition = @" AND ( Elm_Reserved + @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - (Elm_Reserved + @LeaveHours)) < 0 )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Reserved + @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - (Elm_Reserved + @LeaveHours)) < 0 )";
                }
            }
            else if (transactionType.Equals("APPROVE"))
            {
                transaction = @" Elm_Reserved=( case when (Elm_Reserved - round(CAST(@LeaveHours AS decimal(7,4)),2))<0
								then 0
								else  Elm_Reserved - round(CAST(@LeaveHours AS decimal(7,4)),2)
								end
								)
                                   , Elm_Used = Elm_Used + @LeaveHours";
                transactionCondition = @" AND (( Elm_Used + @LeaveHours < 0)  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used + @LeaveHours) - (Elm_Reserved - @LeaveHours)) < 0  )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ((Elm_Reserved - @LeaveHours < 0 OR Elm_Used + Elm_Reimbursed + @LeaveHours < 0)  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used + Elm_Reimbursed + @LeaveHours) - (Elm_Reserved - @LeaveHours)) < 0  )";
                }
            }
            else if (transactionType.Equals("CANCEL"))
            {
                transaction = "Elm_Reserved = Elm_Reserved - @LeaveHours";
                transactionCondition = @" AND ( Elm_Reserved - @LeaveHours < 0  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - (Elm_Reserved - @LeaveHours)) < 0 )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Reserved - @LeaveHours < 0  
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - (Elm_Reserved - @LeaveHours)) < 0 )";
                }
            }
            else if (transactionType.Equals("APPROVECANCEL"))
            {
                transaction = "Elm_Used = Elm_Used - @LeaveHours";
                transactionCondition = @" AND ( Elm_Used - @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used - @LeaveHours) - Elm_Reserved) < 0  )";
                if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
                {
                    transactionCondition = @" AND ( Elm_Used - @LeaveHours < 0 
                                                OR (Elm_Entitled - Elm_Used - Elm_Reimbursed - Elm_Reserved) < 0
                                                OR (Elm_Entitled - (Elm_Used - @LeaveHours) - Elm_Reimbursed - Elm_Reserved) < 0  )";
                }
            }
            string s = string.Format(@"
DECLARE @WithCredit AS BIT
DECLARE @PartOf AS CHAR(2)

SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
SET @PartOf =     (SELECT CASE WHEN Ltm_PartOfLeave = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                     FROM T_LeaveTypeMaster
                    WHERE Ltm_LeaveType = @LeaveType)
                   
IF (RTRIM(@Leaveyear) <> '')
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
ELSE 
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
            ", transactionCondition);
            DataSet ds = dal.ExecuteDataSet(string.Format(@"
DECLARE @WithCredit AS BIT
DECLARE @PartOf AS CHAR(2)

SET @WithCredit = (SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType)
SET @PartOf =     (SELECT CASE WHEN Ltm_PartOfLeave = '' THEN Ltm_LeaveType ELSE Ltm_PartOfLeave END
                     FROM T_LeaveTypeMaster
                    WHERE Ltm_LeaveType = @LeaveType)
                   
IF (RTRIM(@Leaveyear) <> '')
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_LeaveYear = @Leaveyear
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
ELSE 
BEGIN
	IF (@WithCredit = 1)
	BEGIN
		IF (@PartOf <> @LeaveType AND RTRIM(@PartOf) <> '')
		BEGIN
			SELECT
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @PartOf
				{0}
			
			UNION
			
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END		
		ELSE 
		BEGIN
			SELECT 
				*
			FROM T_EmployeeLeave
			WHERE Elm_EmployeeId = @EmployeeID
				AND Elm_Leavetype = @LeaveType
				{0}
		END
	END
END
            ", transactionCondition), CommandType.Text, paramDetails);
            if (!CommonMethods.isEmpty(ds))
            {
                ret = false;
            }
            return ret;
        }

        public bool IsCreditsSameBasedOnTransactions(string transtype, string ControlNo, DALHelper dal)
        {
            DataSet ds = dal.ExecuteDataSet(string.Format(@"
SELECT
	Elt_EmployeeId
	, CONVERT(VARCHAR(20), Elt_LeaveDate, 101) [Elt_LeaveDate]
	, Elt_LeaveType
FROM T_EmployeeLeaveAvailment
WHERE Elt_ControlNo = '{0}'
            ", ControlNo));
            bool ret = true;
            if (!CommonMethods.isEmpty(ds))
            {
                ret = IsCreditsSameBasedOnTransactions(
                                    transtype
                                    , ds.Tables[0].Rows[0]["Elt_EmployeeId"].ToString().Trim()
                                    , string.Format("{0}", this.GetYear(Convert.ToDateTime(ds.Tables[0].Rows[0]["Elt_LeaveDate"].ToString().Trim()), ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()))
                                    , ds.Tables[0].Rows[0]["Elt_LeaveType"].ToString().Trim()
                                    , dal);
            }
            else
            {
                throw new Exception(@"Leave Record with Control No : [{0}] not found.");
            }
            return ret;
        }

        public bool IsCreditsSameBasedOnTransactions(string transtype, string empID, string leaveYear, string leavetype , DALHelper dal)
        {
            bool ret = true;
            if (IsCreditsSameBasedOnTransactionsMessage(transtype, empID, leaveYear, leavetype, dal) != string.Empty)
                ret = false;
            return ret;
        }

        public string IsCreditsSameBasedOnTransactionsMessage(string transtype, string empID, string leaveYear, string leavetype, DALHelper dal)
        {
            string ret = string.Empty;
            #region Get Hours Based on Transaction Records
            DataSet dsLeaveTransactions = dal.ExecuteDataSet(string.Format(@"
DECLARE @RefreshDate datetime
DECLARE @LeaveYear int
DECLARE @EmployeeID as varchar(15)
DECLARE @LeaveType AS VARCHAR(20)
DECLARE @ControlNo AS VARCHAR(20)
	SET @EmployeeID = '{0}'
	SET @LeaveYear = '{1}'
	SET @LeaveType = '{2}'
	
DECLARE @ISWITHCREDITS AS BIT
	SET @ISWITHCREDITS = (
		SELECT Ltm_WithCredit FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = @LeaveType
	)

IF @ISWITHCREDITS = 1
BEGIN
	SET @RefreshDate = CONVERT(datetime, '{3}' + '/' + CONVERT(varchar(4), @LeaveYear + 1))

	SELECT 
		ISNULL(SUM([Reserved]), 0) AS [Reserved]
		, ISNULL(SUM([Used]), 0) AS [Used]
	FROM (
	SELECT 
			CASE WHEN Elt_Status IN ('1', '3', '5', '7')
				THEN
					CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
						THEN Elt_LeaveHour
						ELSE CASE WHEN Elt_LeaveHour < 0
								  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
								  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
							  END
					END
				ELSE 0
			END [Reserved]
			, CASE WHEN Elt_Status IN ('9', 'A')
				THEN
					CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
						THEN Elt_LeaveHour
						ELSE CASE WHEN Elt_LeaveHour < 0
								  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
								  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
							  END
					END
				ELSE 0
			END [Used]
	  FROM T_EmployeeLeaveAvailmentHist
	 INNER JOIN T_LeavetypeMaster 
		ON Ltm_LeaveType = Elt_LeaveType
	   AND Ltm_WithCredit =  1
	  LEFT JOIN T_ParameterMasterExt
		ON Pmx_ParameterID = 'LVDEDUCTN'
	   AND Pmx_Classification = Elt_DayUnit
	 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
	   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
				THEN CASE WHEN Elt_LeaveDate < @RefreshDate
						  THEN DATEPART(YEAR, Elt_LeaveDate) - 1
						  ELSE DATEPART(YEAR, Elt_LeaveDate)
					  END
				ELSE DATEPART(YEAR, Elt_LeaveDate)
			END  = @LeaveYear
	   AND Elt_EmployeeId = @EmployeeID
	   AND Elt_Status NOT IN ('2','4','6','8')
	   AND Elt_LeaveType = @LeaveType
	 UNION ALL
	SELECT 
			CASE WHEN Elt_Status IN ('1', '3', '5', '7')
				THEN
					CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
						THEN Elt_LeaveHour
						ELSE CASE WHEN Elt_LeaveHour < 0
								  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
								  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
							  END
					END
				ELSE 0
			END [Reserved]
			, CASE WHEN Elt_Status IN ('9', 'A')
				THEN
					CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
						THEN Elt_LeaveHour
						ELSE CASE WHEN Elt_LeaveHour < 0
								  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
								  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
							  END
					END
				ELSE 0
			END [Used]
	  FROM T_EmployeeLeaveAvailment
	 INNER JOIN T_LeavetypeMaster 
		ON Ltm_LeaveType = Elt_LeaveType
	   AND Ltm_WithCredit =  1
	  LEFT JOIN T_ParameterMasterExt
		ON Pmx_ParameterID = 'LVDEDUCTN'
	   AND Pmx_Classification = Elt_DayUnit
	 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
	   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
				THEN CASE WHEN Elt_LeaveDate < @RefreshDate
						  THEN DATEPART(YEAR, Elt_LeaveDate) - 1
						  ELSE DATEPART(YEAR, Elt_LeaveDate)
					  END
				ELSE DATEPART(YEAR, Elt_LeaveDate)
			END  = @LeaveYear
	   AND Elt_EmployeeId = @EmployeeID
	   AND Elt_Status NOT IN ('2','4','6','8')
	   AND Elt_LeaveType = @LeaveType
	 UNION ALL
	SELECT 
			CASE WHEN Eln_Status = '1'
				THEN 
					CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
						THEN Eln_LeaveHour
						ELSE CASE WHEN Eln_LeaveHour < 0
								  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
								  ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
							  END
					END
				ELSE 0
			END	 [Reserved]
			, 0 [Used]
	  FROM T_EmployeeLeaveNotice
	  LEFT JOIN T_EmployeeLeaveAvailment 
		ON T_EmployeeLeaveAvailment.Elt_EmployeeID = Eln_EmployeeID
	   AND T_EmployeeLeaveAvailment.Elt_LeaveDate = Eln_LeaveDate
	   AND T_EmployeeLeaveAvailment.Elt_LeaveType = Eln_LeaveType
	  LEFT JOIN T_EmployeeLeaveAvailmentHist 
		ON T_EmployeeLeaveAvailmentHist.Elt_EmployeeID = Eln_EmployeeID
	   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveDate = Eln_LeaveDate
	   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveType = Eln_LeaveType
	 INNER JOIN T_LeavetypeMaster 
		ON Ltm_LeaveType = Eln_LeaveType
	   AND Ltm_WithCredit =  1
	  LEFT JOIN T_ParameterMasterExt
		ON Pmx_ParameterID = 'LVDEDUCTN'
	   AND Pmx_Classification = Eln_DayUnit
	 WHERE Eln_status = '1'
	   AND Eln_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
	   AND CASE WHEN DATEPART(YEAR, Eln_LeaveDate) > @LeaveYear
				THEN CASE WHEN Eln_LeaveDate < @RefreshDate
						  THEN DATEPART(YEAR, Eln_LeaveDate) - 1
						  ELSE DATEPART(YEAR, Eln_LeaveDate)
					  END
				ELSE DATEPART(YEAR, Eln_LeaveDate)
			END  = @LeaveYear
	   AND ISNULL(T_EmployeeLeaveAvailment.Elt_Status, T_EmployeeLeaveAvailmentHist.Elt_Status) IS NULL
	   AND Eln_EmployeeId = @EmployeeID
	   AND Eln_LeaveType = @LeaveType
	   ) AS LEAVEHRTABLE
END   
ELSE
BEGIN
	SELECT 9999 AS [Reserved]
		, 9999 AS [Used]
END
            ", empID, leaveYear, leavetype, Resources.Resource.LEAVEREFRESH, LeaveControlNo));
            DataSet ds2 = dal.ExecuteDataSet(string.Format(@"
DECLARE @RefreshDate datetime
DECLARE @LeaveYear int
DECLARE @EmployeeID as varchar(15)
DECLARE @LeaveType AS VARCHAR(20)
	SET @EmployeeID = '{0}'
	SET @LeaveYear = '{1}'
	SET @LeaveType = '{2}'
	

SELECT
	Elm_Entitled - Elm_Used - Elm_Reserved [Balance]
	, Elm_Used [Used]
	, Elm_Reserved [Reserved]
FROM T_EmployeeLeave
WHERE Elm_LeaveYear = @LeaveYear
	AND Elm_EmployeeId = @EmployeeID
	AND Elm_Leavetype = @LeaveType
", empID, leaveYear, leavetype));
            #endregion

            if (dsLeaveTransactions != null
                && dsLeaveTransactions.Tables.Count > 0
                && dsLeaveTransactions.Tables[0].Rows.Count > 0)
            {
                if (ds2 != null
                    && ds2.Tables.Count > 0
                    && ds2.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToDecimal(dsLeaveTransactions.Tables[0].Rows[0]["Reserved"].ToString()) != 9999)
                    {
                        decimal dOrigReserve = Convert.ToDecimal(dsLeaveTransactions.Tables[0].Rows[0]["Reserved"].ToString());
                        decimal dOrigUsed = Convert.ToDecimal(dsLeaveTransactions.Tables[0].Rows[0]["Used"].ToString());
                        decimal dLeaveReserve = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Reserved"].ToString());
                        decimal dLeaveUsed = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Used"].ToString());
                        if (transtype == "NEW")
                        {
                            if (dOrigReserve != dLeaveReserve)
                                ret = string.Format("Leave Credits Not Sync with Leave Records :\n Credits Reserved : {0}\n Leaves Records : {1}", dOrigReserve, dLeaveReserve);
                        }
                        else if (transtype == "APPROVE")
                        {
                            if (dOrigReserve != dLeaveReserve)
                                ret = string.Format("Leave Credits Not Sync with Leave Records :\n Credits Reserved : {0}\n Leaves Records : {1}", dOrigReserve, dLeaveReserve);
                            if (dOrigUsed != dLeaveUsed)
                                ret = string.Format("Leave Credits Not Sync with Leave Records :\n Credits Used : {0}\n Leaves Records : {1}", dOrigReserve, dLeaveReserve);
                        }
                        else if (transtype == "CANCEL")
                        {
                            if (dOrigReserve != dLeaveReserve)
                                ret = string.Format("Leave Credits Not Sync with Leave Records :\n Credits Reserved : {0}\n Leaves Records : {1}", dOrigReserve, dLeaveReserve);
                        }
                        else if (transtype == "APPROVECANCEL")
                        {
                            if (dOrigUsed != dLeaveUsed)
                                ret = string.Format("Leave Credits Not Sync with Leave Records :\n Credits Used : {0}\n Leaves Records : {1}", dOrigReserve, dLeaveReserve);
                        }
                    }
                }
            }
            return ret;
        }

        public int GetYear(DateTime dtTransactiondate, string leaveType)
        {
            int retleaveYear = 0;
            //robert update leave refresh parametermaster or resource 10/10/2013
            //DateTime dtStart = Convert.ToDateTime(Resources.Resource.LEAVEREFRESH
            //                            + "/"
            //                            + retleaveYear.ToString());
            //DateTime dtEnd = Convert.ToDateTime(Resources.Resource.LEAVEREFRESH
            //                            + "/"
            //                            + retleaveYear.ToString()).AddYears(1).AddDays(-1);
            #region Previous Query
            //string lvrefresh = CommonMethods.getParamterCharValue("LVREFDATE");
            //DateTime dtStart = Convert.ToDateTime(lvrefresh == "" ? Resources.Resource.LEAVEREFRESH : lvrefresh
            //                           + "/"
            //                           + retleaveYear.ToString());
            //DateTime dtEnd = Convert.ToDateTime(lvrefresh == "" ? Resources.Resource.LEAVEREFRESH : lvrefresh
            //                            + "/"
            //                            + retleaveYear.ToString()).AddYears(1).AddDays(-1);
            //if (dtStart > dtTransactiondate)
            //{
            //    retleaveYear = dtStart.Year - 1;
            //}
            //else if (dtStart <= dtTransactiondate && dtEnd >= dtTransactiondate)
            //{
            //    retleaveYear = dtStart.Year;
            //}
            //else if (dtTransactiondate > dtEnd)
            //{
            //    retleaveYear = dtStart.Year + 1;
            //} 
            #endregion
            string query = string.Format(@"SELECT Lym_LeaveYear
                                            FROM  T_LeaveYearMaster 
                                            WHERE '{0}' BETWEEN Lym_StartCycle AND Lym_EndCycle", dtTransactiondate.ToShortDateString());
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retleaveYear = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            //decimal leaveYear = CommonMethods.getParamterValue("LEAVEYEAR");
            //if (dtTransactiondate.Year > leaveYear)
            //{
            //    if (dtTransactiondate <
            //        Convert.ToDateTime(Resources.Resource.LEAVEREFRESH
            //                            + "/"
            //                            + Convert.ToInt32(CommonMethods.getParamterValue("LEAVEYEAR") + 1).ToString()))
            //    {
            //        retleaveYear = (dtTransactiondate.Year - 1);
            //    }
            //    else
            //    {
            //        retleaveYear = dtTransactiondate.Year;
            //    }
            //}
            //else
            //{
            //    retleaveYear = dtTransactiondate.Year;
            //}
            return retleaveYear;
        }

        //Leave Notice

        public void CreateLVNoticeRecord(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[19];
            paramDetails[0] = new ParameterInfo("@Eln_EmployeeId", rowDetails["Eln_EmployeeId"]);
            paramDetails[1] = new ParameterInfo("@Eln_LeaveDate", rowDetails["Eln_LeaveDate"]);
            paramDetails[2] = new ParameterInfo("@Eln_LeaveType", rowDetails["Eln_LeaveType"]);
            paramDetails[3] = new ParameterInfo("@Eln_StartTime", rowDetails["Eln_StartTime"]);
            paramDetails[4] = new ParameterInfo("@Eln_EndTime", rowDetails["Eln_EndTime"]);
            paramDetails[5] = new ParameterInfo("@Eln_LeaveHour", rowDetails["Eln_LeaveHour"]);
            paramDetails[6] = new ParameterInfo("@Eln_DayUnit", rowDetails["Eln_DayUnit"]);
            paramDetails[7] = new ParameterInfo("@Eln_Reason", rowDetails["Eln_Reason"]);
            paramDetails[8] = new ParameterInfo("@Eln_InformDate", rowDetails["Eln_InformDate"]);
            paramDetails[9] = new ParameterInfo("@Eln_ControlNo", rowDetails["Eln_ControlNo"]);
            paramDetails[10] = new ParameterInfo("@Eln_Status", rowDetails["Eln_Status"]);
            paramDetails[11] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[12] = new ParameterInfo("@Eln_LeaveCategory", rowDetails["Eln_LeaveCategory"]);
            paramDetails[13] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[14] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[16] = new ParameterInfo("@Eln_InformMode", rowDetails["Eln_InformMode"]);
            paramDetails[17] = new ParameterInfo("@Eln_Informant", rowDetails["Eln_Informant"]);
            paramDetails[18] = new ParameterInfo("@Eln_InformantRelation", rowDetails["Eln_InformantRelation"]);

            #endregion

            #region SQL Query
            string sqlInsert = @"
DECLARE @Costcenter as varchar(10)
set @Costcenter = (SELECT TOP 1 ISNULL(Ecm_CostcenterCode, '')
                        FROM T_EmployeeCostcenterMovement
                    WHERE Ecm_EmployeeID = @Eln_EmployeeId
                        AND @Eln_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                        --AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)

IF(@Costcenter IS NULL)
BEGIN
    SET @Costcenter = (SELECT Emt_CostcenterCode 
                            FROM T_EmployeeMaster
				        WHERE Emt_EmployeeID = @Eln_EmployeeId)
END

--apsungahid added 20141124
DECLARE @LineCode as varchar(15)
SET @LineCode = (SELECT TOP 1 ISNULL(Ecm_LineCode, '')
                    FROM E_EmployeeCostCenterLineMovement
                    WHERE Ecm_EmployeeID = @Eln_EmployeeId
                    AND @Eln_LeaveDate BETWEEN Ecm_StartDate AND ISNULL(Ecm_EndDate, DATEADD(month, 5, GETDATE()))
                    AND Ecm_Status = 'A' 
                    ORDER BY Ludatetime DESC)
IF(@LineCode IS NULL)
BEGIN
    SET @LineCode = ''
END
IF NOT EXISTS(SELECT Eln_ControlNo 
                FROM T_EmployeeLeaveNotice
                WHERE Eln_EmployeeId = @Eln_EmployeeId
                AND Eln_LeaveDate = @Eln_LeaveDate
                AND Eln_LeaveType = @Eln_LeaveType
                AND Eln_StartTime = @Eln_StartTime
                AND Eln_EndTime = @Eln_EndTime
                AND Eln_LeaveHour = @Eln_LeaveHour
                AND Eln_DayUnit = @Eln_DayUnit
                AND Eln_Reason = @Eln_Reason
                AND Eln_Status = @Eln_Status
                AND Eln_LeaveCategory = @Eln_LeaveCategory
                AND Elt_Filler1 = @Elt_Filler1
                AND Elt_Filler2 = @Elt_Filler2
                AND Elt_Filler3 = @Elt_Filler3
                AND Eln_InformMode = @Eln_InformMode
                AND Eln_Informant = @Eln_Informant
                AND Eln_InformantRelation = @Eln_InformantRelation)
BEGIN
    INSERT INTO T_EmployeeLeaveNotice
    (     Eln_EmployeeId
        , Eln_LeaveDate
        , Eln_AppliedDate
        , Eln_LeaveType
        , Eln_StartTime
        , Eln_EndTime
        , Eln_LeaveHour
        , Eln_DayUnit
        , Eln_Reason
        , Eln_InformDate
        , Eln_ControlNo
        , Eln_Status
        , Usr_Login
        , Ludatetime
        , Eln_LeaveCategory
        , Eln_Costcenter
        , Eln_CostcenterLine
        , Elt_Filler1
        , Elt_Filler2
        , Elt_Filler3
        , Eln_InformMode
        , Eln_Informant
        , Eln_InformantRelation
    )
    VALUES
    (
            @Eln_EmployeeId
        , @Eln_LeaveDate
        , GETDATE()
        , @Eln_LeaveType
        , @Eln_StartTime
        , @Eln_EndTime
        , @Eln_LeaveHour
        , @Eln_DayUnit
        , @Eln_Reason
        , @Eln_InformDate
        , @Eln_ControlNo
        , @Eln_Status
        , @Usr_Login
        , getdate()
        , @Eln_LeaveCategory
        , @Costcenter
        , @LineCode
        , @Elt_Filler1
        , @Elt_Filler2
        , @Elt_Filler3
        , @Eln_InformMode
        , @Eln_Informant
        , @Eln_InformantRelation
    )

    EXEC UpdateLeaveCredits @Eln_ControlNo, 0, @Eln_LeaveHour, @Usr_Login, 1;
END";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateLVNoticeRecordSave(DataRow rowDetails, DALHelper dal, bool bIsCancelled)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[20];
            paramDetails[0] = new ParameterInfo("@Eln_EmployeeId", rowDetails["Eln_EmployeeId"]);
            paramDetails[1] = new ParameterInfo("@Eln_LeaveDate", rowDetails["Eln_LeaveDate"]);
            paramDetails[2] = new ParameterInfo("@Eln_LeaveType", rowDetails["Eln_LeaveType"]);
            paramDetails[3] = new ParameterInfo("@Eln_StartTime", rowDetails["Eln_StartTime"]);
            paramDetails[4] = new ParameterInfo("@Eln_EndTime", rowDetails["Eln_EndTime"]);
            paramDetails[5] = new ParameterInfo("@Eln_LeaveHour", rowDetails["Eln_LeaveHour"]);
            paramDetails[6] = new ParameterInfo("@Eln_DayUnit", rowDetails["Eln_DayUnit"]);
            paramDetails[7] = new ParameterInfo("@Eln_Reason", rowDetails["Eln_Reason"]);
            paramDetails[8] = new ParameterInfo("@Eln_InformDate", rowDetails["Eln_InformDate"]);
            paramDetails[9] = new ParameterInfo("@Eln_ControlNo", rowDetails["Eln_ControlNo"]);
            paramDetails[10] = new ParameterInfo("@Eln_Status", rowDetails["Eln_Status"]);
            paramDetails[11] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[12] = new ParameterInfo("@Eln_LeaveCategory", rowDetails["Eln_LeaveCategory"]);
            paramDetails[13] = new ParameterInfo("@Elt_Filler1", rowDetails["Elt_Filler1"]);
            paramDetails[14] = new ParameterInfo("@Elt_Filler2", rowDetails["Elt_Filler2"]);
            paramDetails[15] = new ParameterInfo("@Elt_Filler3", rowDetails["Elt_Filler3"]);
            paramDetails[16] = new ParameterInfo("@Eln_InformMode", rowDetails["Eln_InformMode"]);
            paramDetails[17] = new ParameterInfo("@Eln_Informant", rowDetails["Eln_Informant"]);
            paramDetails[18] = new ParameterInfo("@Eln_InformantRelation", rowDetails["Eln_InformantRelation"]);
            paramDetails[19] = new ParameterInfo("@IsCancelled", bIsCancelled, SqlDbType.Bit);
            #endregion

            #region SQL Query
            string sqlUpdate = @"
DECLARE @LeaveHours AS DECIMAL(5,2)
DECLARE @PendingLeaveHours AS DECIMAL(7,4)

SELECT @LeaveHours = Eln_LeaveHour
FROM T_EmployeeLeaveNotice
WHERE Eln_ControlNo = @Eln_ControlNo

SET @PendingLeaveHours = @LeaveHours * -1
EXEC UpdateLeaveCredits @Eln_ControlNo, 0, @PendingLeaveHours, @Usr_Login, 1;

UPDATE T_EmployeeLeaveNotice
SET   Eln_EmployeeId = @Eln_EmployeeId
    , Eln_LeaveDate = @Eln_LeaveDate
    , Eln_LeaveType = @Eln_LeaveType
    , Eln_StartTime = @Eln_StartTime
    , Eln_EndTime = @Eln_EndTime
    , Eln_LeaveHour = @Eln_LeaveHour
    , Eln_DayUnit = @Eln_DayUnit
    , Eln_Reason = @Eln_Reason
    , Eln_InformDate = @Eln_InformDate
    , Eln_ControlNo = @Eln_ControlNo
    , Usr_Login = @Usr_Login
    , Ludatetime = getdate()
    , Eln_LeaveCategory = @Eln_LeaveCategory
    , Elt_Filler1 = @Elt_Filler1
    , Elt_Filler2 = @Elt_Filler2
    , Elt_Filler3 = @Elt_Filler3
    , Eln_status = @Eln_Status
WHERE Eln_ControlNo = @Eln_ControlNo

IF @IsCancelled = 0
BEGIN
    EXEC UpdateLeaveCredits @Eln_ControlNo, 0, @Eln_LeaveHour, @Usr_Login, 1;
END";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateLVNoticeRecordStatus(string controlNo, string status, string userLogin, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[3];
            paramDetails[0] = new ParameterInfo("@Eln_ControlNo", controlNo);
            paramDetails[1] = new ParameterInfo("@Eln_Status", status);
            paramDetails[2] = new ParameterInfo("@Usr_Login", userLogin);
            #endregion

            #region SQL Query
            string sqlUpdate = @"
                                UPDATE T_EmployeeLeaveNotice
                                   SET Eln_Status = @Eln_Status
                                     , Usr_Login = @Usr_Login
                                     , Ludatetime = GETDATE()
                                WHERE Eln_ControlNo = @Eln_ControlNo
                                ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }
        //Special Leave
        // not used anymore : public DataSet getRegularDaysOfEmployee(string idNumber, string fromMMDDYYY, string toMMDDYYYY)
        public DataSet getRegularDaysOfEmployee(string idNumber, string fromMMDDYYY, string toMMDDYYYY)
        { 
            DataSet ds = new DataSet();
            string sql = string.Format(@"
                        SELECT Ell_ProcessDate
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                           AND 1 = CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
                                        THEN 1
                                        ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR Ell_DayCode = 'REG'))
                                                  THEN 1
                                                  ELSE 0
                                              END
                                    END
                           AND Ell_EncodedPayLeaveHr = 0
                           AND Ell_EncodedNoPayLeaveHr = 0
                           AND Ell_EmployeeID = '{0}'
                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailment
                                                        WHERE Elt_EmployeeID = '{0}'
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                        UNION
                                                       SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailmentHist
                                                        WHERE Elt_EmployeeID = '{0}'
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                        UNION
                                                       SELECT Eln_LeaveDate
                                                         FROM T_EmployeeLeaveNotice
                                                        WHERE Eln_EmployeeID = '{0}'
                                                          AND Eln_Status NOT IN ('2','4','6','8','C'))

                         UNION

                        SELECT Ell_ProcessDate
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                         WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                           AND 1 = CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
                                        THEN 1
                                        ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR Ell_DayCode = 'REG'))
                                                  THEN 1
                                                  ELSE 0
                                              END
                                    END
                           AND Ell_EncodedPayLeaveHr = 0
                           AND Ell_EncodedNoPayLeaveHr = 0
                           AND Ell_EmployeeID = '{0}'
                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailment
                                                        WHERE Elt_EmployeeID = '{0}'
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                        UNION
                                                       SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailmentHist
                                                        WHERE Elt_EmployeeID = '{0}'
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                        UNION
                                                       SELECT Eln_LeaveDate
                                                         FROM T_EmployeeLeaveNotice
                                                        WHERE Eln_EmployeeID = '{0}'
                                                          AND Eln_Status NOT IN ('2','4','6','8','C'))", idNumber, fromMMDDYYY, toMMDDYYYY);
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

        public DataSet getRegularDaysOfEmployeeWithDetail(string idNumber, string fromMMDDYYY, string toMMDDYYYY, string leaveType, decimal credits)
        {
            DataSet ds = new DataSet();
            #region Previous
//            string sql = string.Format(@"
//                    DECLARE @withCredit as bit
//                    SET @withCredit = ( SELECT Ltm_WithCredit
//                                          FROM T_LeaveTypeMaster
//                                         WHERE Ltm_LeaveType = '{3}' )
//
//                    --FOR DATE greater then Ledger MAX
//                    DECLARE @START AS Datetime
//                    DECLARE @END AS Datetime
//                    DECLARE @INCREMENT AS Int
//                    DECLARE @COUNT AS Int
//                    DECLARE @RESTDAY AS char(7)
//                    DECLARE @EMPID AS varchar(15)
//
//                    SET @START = '{1}'
//                    SET @END = '{2}'
//                    SET @EMPID = '{0}'
//
//                    CREATE TABLE {5} (x_date datetime)
//
//                    INSERT INTO {5}
//                    SELECT TOP ( datediff(DAY,@START,@END) + 1 )
//                                [Date] = dateadd(DAY,ROW_NUMBER()
//                          OVER(ORDER BY c1.name),
//                          DATEADD(DD,-1,@START))
//                    FROM   [master].[dbo].[spt_values] c1 
//
//                    SET @RESTDAY = (SELECT TOP 1 Erd_RestDay 
//                                      FROM T_EmployeeRestDay
//                                     WHERE Erd_EmployeeID = @EMPID
//                                     ORDER BY Erd_EffectivityDate DESC)
//                        
//                    SET @COUNT = 0
//                    SET @INCREMENT = 1
//                    WHILE @INCREMENT < 7
//                    BEGIN
//                          IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
//                                DELETE FROM {5}                                      --NOT RESTDAY
//                                WHERE datepart(dw,x_date) = @INCREMENT + 1 
//                          SET @INCREMENT = @INCREMENT + 1
//                    END
//
//                    IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
//                          DELETE FROM {5}
//                          WHERE datepart(dw,x_date) = 1  
//                  
//                    --FOR DATE greater then Ledger MAX
//
//                    SELECT CASE WHEN ROW_NUMBER() OVER (ORDER BY Convert(datetime,[Leave Date])) <= {4}/SUM([Hours])
//                                THEN '{3}'  
//                                ELSE CASE WHEN (@withCredit = '1' )
//                                          THEN '{6}' 
//                                          ELSE '{3}'
//                                      END 
//                            END [Leave Type]
//                         , * FROM (
//
//                        SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
//                             , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
//                             , Ell_DayCode [Day Code]
//                             , Ell_ShiftCode [Shift Code]
//                             , '[' + Scm_ShiftCode + '] '
//                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
//                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
//                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
//                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
//                             , Scm_ShiftHours [Hours]
//                          FROM T_EmployeeLogLedger
//                         INNER JOIN T_EmployeeMaster
//                            ON Emt_EmployeeId = Ell_EmployeeId
//                          LEFT JOIN T_ShiftCodeMaster
//                            ON Scm_ShiftCode = Ell_ShiftCode
//                         WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
//                           AND 1 = CASE WHEN '{3}' = 'OB'
//                                        THEN 1
//                                        ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
//                                                  THEN 1
//                                                  ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR Ell_DayCode = 'CMPY' OR Ell_DayCode = 'REG'))
//                                                            THEN 1
//                                                            ELSE 0
//                                                        END
//                                              END
//                                    END
//                           AND Ell_EncodedPayLeaveHr = 0
//                           AND Ell_EncodedNoPayLeaveHr = 0
//                           AND Ell_EmployeeID = '{0}'
//                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
//                                                         FROM T_EmployeeLeaveAvailment
//                                                        WHERE Elt_EmployeeID = '{0}'
//                                                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
//                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailment
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0'
//							                                                             UNION 
//							                                                            SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailmentHist
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0')
//                                                        UNION
//                                                       SELECT Elt_LeaveDate
//                                                         FROM T_EmployeeLeaveAvailmentHist
//                                                        WHERE Elt_EmployeeID = '{0}'
//                                                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
//                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailment
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0'
//							                                                             UNION 
//							                                                            SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailmentHist
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0')
//                                                        UNION
//                                                       SELECT Eln_LeaveDate
//                                                         FROM T_EmployeeLeaveNotice
//                                                        WHERE Eln_EmployeeID = '{0}'
//                                                          AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Eln_Status NOT IN ('2','4','6','8','C'))
//
//                         UNION
//
//                        SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
//                             , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
//                             , Ell_DayCode [Day Code]
//                             , Ell_ShiftCode [Shift Code]
//                             , '[' + Scm_ShiftCode + '] '
//                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
//                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
//                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
//                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
//                             , Scm_ShiftHours [Hours]
//                          FROM T_EmployeeLogLedgerHist
//                         INNER JOIN T_EmployeeMaster
//                            ON Emt_EmployeeId = Ell_EmployeeId
//                          LEFT JOIN T_ShiftCodeMaster
//                            ON Scm_ShiftCode = Ell_ShiftCode
//                         WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
//                           AND 1 = CASE WHEN '{3}' = 'OB'
//                                        THEN 1
//                                        ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
//                                                  THEN 1
//                                                  ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR  Ell_DayCode = 'CMPY' OR Ell_DayCode = 'REG'))
//                                                            THEN 1
//                                                            ELSE 0
//                                                        END
//                                              END
//                                    END
//                           AND Ell_EncodedPayLeaveHr = 0
//                           AND Ell_EncodedNoPayLeaveHr = 0
//                           AND Ell_EmployeeID = '{0}'
//                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
//                                                         FROM T_EmployeeLeaveAvailment
//                                                        WHERE Elt_EmployeeID = '{0}'
//                                                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
//                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailment
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0'
//							                                                             UNION 
//							                                                            SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailmentHist
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0')
//                                                        UNION
//                                                       SELECT Elt_LeaveDate
//                                                         FROM T_EmployeeLeaveAvailmentHist
//                                                        WHERE Elt_EmployeeID = '{0}'
//                                                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
//                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailment
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0'
//							                                                             UNION 
//							                                                            SELECT Elt_RefControlNo
//								                                                          FROM T_EmployeeLeaveAvailmentHist
//							                                                             WHERE Elt_EmployeeId = '{0}'
//                                                                                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//								                                                           AND Elt_Status = '0')
//                                                        UNION
//                                                       SELECT Eln_LeaveDate
//                                                         FROM T_EmployeeLeaveNotice
//                                                        WHERE Eln_EmployeeID = '{0}'
//                                                          AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
//                                                          AND Eln_Status NOT IN ('2','4','6','8','C')) 
//
//                         UNION
//                    
//                        SELECT Convert(varchar(10), x_date, 101) [Leave Date]
//                             , LEFT(UPPER(datename(DW, x_date)), 3) [DoW]
//                             , 'REG' [Day Code]
//                             , Emt_Shiftcode [Shift Code]
//                             , '[' + Scm_ShiftCode + '] '
//                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
//                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
//                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
//                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
//                             , Scm_ShiftHours [Hours]
//                          FROM {5}
//                         INNER JOIN T_EmployeeMaster
//                            ON Emt_EmployeeId = @EMPID
//                          LEFT JOIN T_ShiftCodeMaster
//                            ON Scm_ShiftCode = Emt_Shiftcode
//                         WHERE x_date NOT IN (SELECT Hmt_HolidayDate FROM T_HolidayMaster)   --NOT HOlIDAY
//                           AND x_date > (SELECT MAX(Ell_ProcessDate)
//				                           FROM T_EmployeeLogLedger
//				                          WHERE Ell_EmployeeID = @EMPID)
//                           AND x_date NOT IN ( SELECT Elt_LeaveDate
//						                         FROM T_EmployeeLeaveAvailment
//						                        WHERE Elt_EmployeeID = @EMPID
//						                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//						                          AND Elt_Status NOT IN ('2','4','6','8','C')
//						                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//														                          FROM T_EmployeeLeaveAvailment
//														                         WHERE Elt_EmployeeId = @EMPID
//														                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//														                           AND Elt_Status = '0'
//														                         UNION 
//														                        SELECT Elt_RefControlNo
//														                          FROM T_EmployeeLeaveAvailmentHist
//														                         WHERE Elt_EmployeeId = @EMPID
//														                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//														                           AND Elt_Status = '0')
//						                        UNION
//					                           SELECT Elt_LeaveDate
//						                         FROM T_EmployeeLeaveAvailmentHist
//						                        WHERE Elt_EmployeeID = @EMPID
//						                          AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//						                          AND Elt_Status NOT IN ('2','4','6','8','C')
//						                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
//														                          FROM T_EmployeeLeaveAvailment
//														                         WHERE Elt_EmployeeId = @EMPID
//														                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//														                           AND Elt_Status = '0'
//														                         UNION 
//														                        SELECT Elt_RefControlNo
//														                          FROM T_EmployeeLeaveAvailmentHist
//														                         WHERE Elt_EmployeeId = @EMPID
//														                           AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//														                           AND Elt_Status = '0')
//                                                                                 UNION
//                                                                                SELECT Eln_LeaveDate
//                                                                                  FROM T_EmployeeLeaveNotice
//                                                                                 WHERE Eln_EmployeeID = '{0}'
//                                                                                   AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
//                                                                                   AND Eln_Status NOT IN ('2','4','6','8','C')) )AS TEMP
//
//                         GROUP BY [Leave Date]
//                                , [DoW]
//                                , [Day Code]
//                                , [Shift Code]
//                                , [Shift Desc]
//                                , [Start Time]
//                                , [End Time]
//                                , [Hours]
//
//
//                          DROP TABLE {5} ", idNumber
//                                          , fromMMDDYYY
//                                          , toMMDDYYYY
//                                          , leaveType.Substring(0, 2)
//                                          , credits
//                                          , "_" + idNumber + fromMMDDYYY.Replace("/", string.Empty)
//                                          , Resources.Resource.UNPAIDLVECODE);
            #endregion
            string sql = string.Format(@"
                    DECLARE @withCredit as bit
                    SET @withCredit = ( SELECT Ltm_WithCredit
                                          FROM T_LeaveTypeMaster
                                         WHERE Ltm_LeaveType = '{3}' )
                    DECLARE @DAYSEL AS BIT 
						SET @DAYSEL = (
										SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster
										WHERE Pcm_SystemID = 'LEAVE'
											AND Pcm_ProcessID = 'DAYSEL'
								)
					DECLARE @WH AS DECIMAL(16, 2)
						SET @WH = CONVERT(DECIMAL(16, 2), isnull((
										SELECT Pmx_ParameterValue FROM T_ParameterMasterExt
										WHERE Pmx_ParameterID = 'LVDEDUCTN'
											AND Pmx_Classification = 'WH' ), 0.00))

                    --FOR DATE greater then Ledger MAX
                    DECLARE @START AS Datetime
                    DECLARE @END AS Datetime
                    DECLARE @INCREMENT AS Int
                    DECLARE @COUNT AS Int
                    DECLARE @RESTDAY AS char(7)
                    DECLARE @EMPID AS varchar(15)
                    
					DECLARE @WORKTYPE AS VARCHAR(20)
					DECLARE @WORKGROUP AS VARCHAR(20)
					DECLARE @SHIFTCODE AS VARCHAR(10)
					DECLARE @SHIFTCODEREST AS VARCHAR(20)
					DECLARE @LOCATIONCODE AS VARCHAR(20)
					DECLARE @LeaveDate AS date

                    SET @START = '{1}'
                    SET @END = '{2}'
                    SET @EMPID = '{0}'

                    SET @WORKTYPE = (SELECT RTRIM(Emt_WorkType)
										FROM T_EmployeeMaster
										WHERE Emt_EmployeeID = @EMPID)

					SET @WORKGROUP = (SELECT RTRIM(Emt_WorkGroup)
					                    FROM T_EmployeeMaster
					                    WHERE Emt_EmployeeID = @EMPID)

					SET @SHIFTCODE = (SELECT Emt_Shiftcode 
					FROM T_EmployeeMaster 
					WHERE Emt_EmployeeID = @EMPID )

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
					WHERE Emt_EmployeeID = @EMPID )

					DECLARE @TEMPTABLELEAVE AS TABLE
					([Leave Date] varchar(10)
					, [DoW] char(3)
					, [Day Code] char(4)
					, [Shift Code] char(10)
					, [Shift Desc] varchar(100)
					, [Start Time] varchar(10)
					, [End Time] varchar(10)
					, [Hours] int)

                    CREATE TABLE {5} (x_date datetime)

                    INSERT INTO {5}
                    SELECT TOP ( datediff(DAY,@START,@END) + 1 )
                                [Date] = dateadd(DAY,ROW_NUMBER()
                          OVER(ORDER BY c1.name),
                          DATEADD(DD,-1,@START))
                    FROM   [master].[dbo].[spt_values] c1 

                    SET @RESTDAY = (SELECT TOP 1 Erd_RestDay 
                                      FROM T_EmployeeRestDay
                                     WHERE Erd_EmployeeID = @EMPID
                                     ORDER BY Erd_EffectivityDate DESC)
                        
                    SET @COUNT = 0
                    SET @INCREMENT = 1
                    WHILE @INCREMENT < 7
                    BEGIN
                          IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                DELETE FROM {5}                                      --NOT RESTDAY
                                WHERE datepart(dw,x_date) = @INCREMENT + 1 
                          SET @INCREMENT = @INCREMENT + 1
                    END

                    IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                          DELETE FROM {5}
                          WHERE datepart(dw,x_date) = 1  
                  
                    --FOR DATE greater then Ledger MAX

                    DECLARE LEAVETABLE CURSOR FOR
					        SELECT *   
                            FROM {5}

                    OPEN LEAVETABLE
					FETCH NEXT FROM LEAVETABLE INTO @LeaveDate
					WHILE @@FETCH_STATUS = 0

                    BEGIN
					IF EXISTS(SELECT Cal_ProcessDate
						FROM T_CalendarGroupTmp
						WHERE cal_worktype = @Worktype
						and cal_workgroup = @Workgroup
						and Cal_ProcessDate = @LeaveDate) 

						BEGIN

						IF @WORKTYPE <> 'REG'
								insert into @TEMPTABLELEAVE	
								
								SELECT CONVERT(VARCHAR(20), Cal_ProcessDate, 101) [ProcessDate]
											, LEFT(UPPER(datename(DW, Cal_ProcessDate)), 3) [DoW]
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
											, Cal_ShiftCode [Shift Code]
											, '[' +   Cal_ShiftCode + '] '
											+ LEFT(SC2.Scm_ShiftTimeIn, 2) +':'+ RIGHT(SC2.Scm_ShiftTimeIn, 2) + ' - '
											+ LEFT(SC2.Scm_ShiftTimeOut, 2) +':'+ RIGHT(SC2.Scm_ShiftTimeOut, 2) [Shift Desc]
											 
											, LEFT(SC2.Scm_ShiftTimeIn, 2) +':'+ RIGHT(SC2.Scm_ShiftTimeIn, 2) [Start Time]
											, LEFT(SC2.Scm_ShiftTimeOut, 2) +':'+ RIGHT(SC2.Scm_ShiftTimeOut, 2) [End Time]
											, SC2.Scm_ShiftHours [Hours]
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
											AND Cal_ProcessDate = @LEAVEDATE

						ELSE
						INSERT INTO @TEMPTABLELEAVE
						SELECT CONVERT(VARCHAR(20), CalendarDate, 101) [ProcessDate]
											 , LEFT(UPPER(datename(DW, CalendarDate)), 3) [DoW]
											 , CASE WHEN 1 = ( SELECT 
																	CASE WHEN CAL.DayOfWeek = 1
																	THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
																	ELSE 
																		RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
																	END
																FROM T_EmployeeRestDay E1
																WHERE Erd_EmployeeID = @EMPID
																AND Erd_EffectivityDate = (
																	SELECT 
																		TOP 1 Erd_EffectivityDate 
																	FROM T_EmployeeRestDay E2
																	WHERE E2.Erd_EmployeeID = @EMPID
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
											 , CASE WHEN 1 = (SELECT 
																	CASE WHEN CAL.DayOfWeek = 1
																	THEN RIGHT(LEFT(Erd_RestDay, 7), 1)
																	ELSE 
																		RIGHT(LEFT(Erd_RestDay, CAL.DayOfWeek - 1), 1)
																	END
																FROM T_EmployeeRestDay E1
																WHERE Erd_EmployeeID = @EMPID
																	AND Erd_EffectivityDate = (
																		SELECT 
																			TOP 1 Erd_EffectivityDate 
																		FROM T_EmployeeRestDay E2
																		WHERE E2.Erd_EmployeeID = @EMPID
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
											 , '[' + SC1.Scm_ShiftCode + '] '
											 + LEFT(SC1.Scm_ShiftTimeIn, 2) +':'+ RIGHT(SC1.Scm_ShiftTimeIn, 2) + ' - '
											+ LEFT(SC1.Scm_ShiftTimeOut, 2) +':'+ RIGHT(SC1.Scm_ShiftTimeOut, 2) [Shift Desc]
											 
											 , LEFT(SC1.Scm_ShiftTimeIn, 2) +':'+ RIGHT(SC1.Scm_ShiftTimeIn, 2) [Start Time]
											 , LEFT(SC1.Scm_ShiftTimeOut, 2) +':'+ RIGHT(SC1.Scm_ShiftTimeOut, 2) [End Time]
											 , SC1.Scm_ShiftHours [Hours]
											FROM dbo.GetCalendarDates(@LeaveDate, @LeaveDate) CAL
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
													WHERE Erd_EmployeeID = @EMPID
														AND Erd_EffectivityDate = (
															SELECT 
																TOP 1 Erd_EffectivityDate 
															FROM T_EmployeeRestDay E2
															WHERE E2.Erd_EmployeeID = @EMPID
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

					ELSE

					BEGIN

                    --SELECT 
                        --CASE WHEN ROW_NUMBER() OVER (ORDER BY Convert(datetime,[Leave Date])) <= {4}/SUM([Hours])
                        --        THEN '{3}'  
                        --        ELSE CASE WHEN (@withCredit = '1' )
                        --                  THEN '{6}' 
                        --                  ELSE '{3}'
                        --              END 
                        --    END [Leave Type]
                        -- , 
                        --* 
                        --INTO #TEMPTABLELEAVES
                        --FROM (
                        INSERT INTO @TEMPTABLELEAVE
                        SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
                             , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                             , Ell_DayCode [Day Code]
                             , Ell_ShiftCode [Shift Code]
                             , '[' + Scm_ShiftCode + '] '
                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
                             , Scm_ShiftHours [Hours]
                          FROM T_EmployeeLogLedger
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                          LEFT JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode                            
							WHERE Ell_ProcessDate = @LeaveDate
                         --WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                           AND 1 = CASE WHEN '{3}' = 'OB'
                                        THEN 1
                                        ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode IN('REG', 'PSD') )
                                                  THEN 1
                                                  ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR Ell_DayCode = 'CMPY' OR Ell_DayCode IN('REG', 'PSD') ))
                                                            THEN 1
                                                            ELSE 0
                                                        END
                                              END
                                    END
                           AND Ell_EncodedPayLeaveHr = 0
                           AND Ell_EncodedNoPayLeaveHr = 0
                           AND Ell_EmployeeID = '{0}'
                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailment
                                                        WHERE Elt_EmployeeID = '{0}'
                                                        AND Elt_LeaveDate = @LeaveDate
                                                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailment
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0'
							                                                             UNION 
							                                                            SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailmentHist
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0')
                                                        UNION
                                                       SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailmentHist
                                                        WHERE Elt_EmployeeID = '{0}'
                                                            AND Elt_LeaveDate = @LeaveDate
                                                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailment
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0'
							                                                             UNION 
							                                                            SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailmentHist
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0')
                                                        UNION
                                                       SELECT Eln_LeaveDate
                                                         FROM T_EmployeeLeaveNotice
                                                        WHERE Eln_EmployeeID = '{0}'
                                                            AND Eln_LeaveDate = @LeaveDate
                                                          --AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Eln_Status NOT IN ('2','4','6','8','C'))

                         UNION

                        SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
                             , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                             , Ell_DayCode [Day Code]
                             , Ell_ShiftCode [Shift Code]
                             , '[' + Scm_ShiftCode + '] '
                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
                             , Scm_ShiftHours [Hours]
                          FROM T_EmployeeLogLedgerHist
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Ell_EmployeeId
                          LEFT JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Ell_ShiftCode
                            WHERE Ell_ProcessDate = @LeaveDate
                         --WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                           AND 1 = CASE WHEN '{3}' = 'OB'
                                        THEN 1
                                        ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode IN('REG', 'PSD') ) 
                                                  THEN 1
                                                  ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR  Ell_DayCode = 'CMPY' OR Ell_DayCode IN('REG', 'PSD') ))
                                                            THEN 1
                                                            ELSE 0
                                                        END
                                              END
                                    END
                           AND Ell_EncodedPayLeaveHr = 0
                           AND Ell_EncodedNoPayLeaveHr = 0
                           AND Ell_EmployeeID = '{0}'
                           AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailment
                                                        WHERE Elt_EmployeeID = '{0}'
                                                        AND Elt_LeaveDate = @LeaveDate
                                                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailment
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0'
							                                                             UNION 
							                                                            SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailmentHist
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0')
                                                        UNION
                                                       SELECT Elt_LeaveDate
                                                         FROM T_EmployeeLeaveAvailmentHist
                                                        WHERE Elt_EmployeeID = '{0}'
                                                            AND Elt_LeaveDate = @LeaveDate
                                                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Elt_Status NOT IN ('2','4','6','8','C')
                                                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailment
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0'
							                                                             UNION 
							                                                            SELECT Elt_RefControlNo
								                                                          FROM T_EmployeeLeaveAvailmentHist
							                                                             WHERE Elt_EmployeeId = '{0}'
                                                                                            AND Elt_LeaveDate = @LeaveDate
                                                                                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                           AND Elt_Status = '0')
                                                        UNION
                                                       SELECT Eln_LeaveDate
                                                         FROM T_EmployeeLeaveNotice
                                                        WHERE Eln_EmployeeID = '{0}'
                                                        AND Eln_LeaveDate = @LeaveDate
                                                          --AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                          AND Eln_Status NOT IN ('2','4','6','8','C')) 

                         UNION
                    
                        SELECT Convert(varchar(10), @LeaveDate, 101) [Leave Date]
                             , LEFT(UPPER(datename(DW, @LeaveDate)), 3) [DoW]
                             , 'REG' [Day Code]
                             , Emt_Shiftcode [Shift Code]
                             , '[' + Scm_ShiftCode + '] '
                             + LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) + ' - '
                             + LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [Shift Desc]
                             , LEFT(Scm_ShiftTimeIn, 2) +':'+ RIGHT(Scm_ShiftTimeIn, 2) [Start Time]
                             , LEFT(Scm_ShiftTimeOut, 2) +':'+ RIGHT(Scm_ShiftTimeOut, 2) [End Time]
                             , Scm_ShiftHours [Hours]
                          FROM {5}
                         INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = @EMPID
                          LEFT JOIN T_ShiftCodeMaster
                            ON Scm_ShiftCode = Emt_Shiftcode
                         WHERE @LeaveDate NOT IN (SELECT Hmt_HolidayDate FROM T_HolidayMaster)   --NOT HOlIDAY
                           AND @LeaveDate > (SELECT MAX(Ell_ProcessDate)
				                           FROM T_EmployeeLogLedger
				                          WHERE Ell_EmployeeID = @EMPID)
                           AND @LeaveDate NOT IN ( SELECT Elt_LeaveDate
						                         FROM T_EmployeeLeaveAvailment
						                        WHERE Elt_EmployeeID = @EMPID
                                                    AND Elt_LeaveDate = @LeaveDate
						                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
						                          AND Elt_Status NOT IN ('2','4','6','8','C')
						                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
														                          FROM T_EmployeeLeaveAvailment
														                         WHERE Elt_EmployeeId = @EMPID
                                                                                     AND Elt_LeaveDate = @LeaveDate
														                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                           AND Elt_Status = '0'
														                         UNION 
														                        SELECT Elt_RefControlNo
														                          FROM T_EmployeeLeaveAvailmentHist
														                         WHERE Elt_EmployeeId = @EMPID
                                                                                     AND Elt_LeaveDate = @LeaveDate
														                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                           AND Elt_Status = '0')
						                        UNION
					                           SELECT Elt_LeaveDate
						                         FROM T_EmployeeLeaveAvailmentHist
						                        WHERE Elt_EmployeeID = @EMPID
                                                    AND Elt_LeaveDate = @LeaveDate
						                          --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
						                          AND Elt_Status NOT IN ('2','4','6','8','C')
						                          AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
														                          FROM T_EmployeeLeaveAvailment
														                         WHERE Elt_EmployeeId = @EMPID
                                                                                     AND Elt_LeaveDate = @LeaveDate
														                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                           AND Elt_Status = '0'
														                         UNION 
														                        SELECT Elt_RefControlNo
														                          FROM T_EmployeeLeaveAvailmentHist
														                         WHERE Elt_EmployeeId = @EMPID
                                                                                     AND Elt_LeaveDate = @LeaveDate
														                           --AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                           AND Elt_Status = '0')
                                                                                 UNION
                                                                                SELECT Eln_LeaveDate
                                                                                  FROM T_EmployeeLeaveNotice
                                                                                 WHERE Eln_EmployeeID = '{0}'
                                                                                     AND Eln_LeaveDate = @LeaveDate
                                                                                   --AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
                                                                                   AND Eln_Status NOT IN ('2','4','6','8','C')) --)AS TEMP

                         --GROUP BY [Leave Date]
                         --       , [DoW]
                         --       , [Day Code]
                         --       , [Shift Code]
                         --       , [Shift Desc]
                         --       , [Start Time]
                         --       , [End Time]
                         --       , [Hours]


                        END
                        FETCH NEXT FROM LEAVETABLE INTO @LeaveDate
					    END
					    CLOSE LEAVETABLE
					    DEALLOCATE LEAVETABLE

                            DECLARE @SUMHOURS AS DECIMAL(16, 2)
							SET @SUMHOURS = ( 
								SELECT sum( CASE WHEN @DAYSEL = 1 THEN @WH ELSE [Hours] END )
								 FROM @TEMPTABLELEAVE )
                        SELECT
                        CASE WHEN ROW_NUMBER() OVER (ORDER BY Convert(datetime,[Leave Date])) <= {4}/( CASE WHEN @DAYSEL = 1 THEN CASE WHEN @WH = 0 THEN [Hours] ELSE @WH END ELSE [Hours] END )
                                THEN '{3}'  
                                ELSE CASE WHEN (@withCredit = '1' )
                                          THEN '{6}' 
                                          ELSE '{3}'
                                      END 
                            END [Leave Type]
                         ,  * FROM @TEMPTABLELEAVE

                          DELETE @TEMPTABLELEAVE
                          DROP TABLE {5} ", idNumber
                                          , fromMMDDYYY
                                          , toMMDDYYYY
                                          , leaveType.Substring(0, 2)
                                          , credits
                                          , "[_" + idNumber + fromMMDDYYY.Replace("/", string.Empty)+"]"
                                          , Resources.Resource.UNPAIDLVECODE);

            if (Convert.ToBoolean(Resources.Resource.USELEAVEDEFAULTSHIFT))
            {
                #region SQL with USELEAVEDEFAULTSHIFT
                sql = string.Format(@"
                        DECLARE @withCredit as bit
                        SET @withCredit = ( SELECT Ltm_WithCredit
                                              FROM T_LeaveTypeMaster
                                             WHERE Ltm_LeaveType = '{3}' )
                        DECLARE @DAYSEL AS BIT 
						    SET @DAYSEL = (
										    SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster
										    WHERE Pcm_SystemID = 'LEAVE'
											    AND Pcm_ProcessID = 'DAYSEL'
								    )
					    DECLARE @WH AS DECIMAL(16, 2)
						    SET @WH = CONVERT(DECIMAL(16, 2), isnull((
										    SELECT Pmx_ParameterValue FROM T_ParameterMasterExt
										    WHERE Pmx_ParameterID = 'LVDEDUCTN'
											    AND Pmx_Classification = 'WH' ), 0.00))

                        --FOR DATE greater then Ledger MAX
                        DECLARE @START AS Datetime
                        DECLARE @END AS Datetime
                        DECLARE @INCREMENT AS Int
                        DECLARE @COUNT AS Int
                        DECLARE @RESTDAY AS char(7)
                        DECLARE @EMPID AS varchar(15)

                        SET @START = '{1}'
                        SET @END = '{2}'
                        SET @EMPID = '{0}'

                        CREATE TABLE {5} (x_date datetime)

                        INSERT INTO {5}
                        SELECT TOP ( datediff(DAY,@START,@END) + 1 )
                                    [Date] = dateadd(DAY,ROW_NUMBER()
                              OVER(ORDER BY c1.name),
                              DATEADD(DD,-1,@START))
                        FROM   [master].[dbo].[spt_values] c1 

                        SET @RESTDAY = (SELECT TOP 1 Erd_RestDay 
                                          FROM T_EmployeeRestDay
                                         WHERE Erd_EmployeeID = @EMPID
                                         ORDER BY Erd_EffectivityDate DESC)
                        
                        SET @COUNT = 0
                        SET @INCREMENT = 1
                        WHILE @INCREMENT < 7
                        BEGIN
                              IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                                    DELETE FROM {5}                                      --NOT RESTDAY
                                    WHERE datepart(dw,x_date) = @INCREMENT + 1 
                              SET @INCREMENT = @INCREMENT + 1
                        END

                        IF SUBSTRING(@RESTDAY, @INCREMENT, 1) = 1
                              DELETE FROM {5}
                              WHERE datepart(dw,x_date) = 1  
                  
                        --FOR DATE greater then Ledger MAX

                        SELECT 
                            --CASE WHEN ROW_NUMBER() OVER (ORDER BY Convert(datetime,[Leave Date])) <= {4}/SUM([Hours])
                            --        THEN '{3}'  
                            --        ELSE CASE WHEN (@withCredit = '1' )
                            --                  THEN '{6}' 
                            --                  ELSE '{3}'
                            --              END 
                            --    END [Leave Type]
                            -- , 
                            * INTO #TEMPTABLELEAVES 
                            FROM (

                            SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , DefaultShift.Scm_ShiftCode [Shift Code]
                                 , '[' + DefaultShift.Scm_ShiftCode + '] '
                                 + LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [Shift Desc]
                                 , LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) [Start Time]
                                 , LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [End Time]
                                 , DefaultShift.Scm_ShiftHours [Hours]
                              FROM T_EmployeeLogLedger
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster ShiftCode
                                ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                              LEFT JOIN T_ShiftCodeMaster DefaultShift
                                ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                               AND DefaultShift.Scm_DefaultShift = 'True'
                               AND DefaultShift.Scm_Status = 'A'
                             WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                               AND 1 = CASE WHEN '{3}' = 'OB'
                                            THEN 1
                                            ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
                                                      THEN 1
                                                      ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR Ell_DayCode = 'CMPY' OR Ell_DayCode = 'REG'))
                                                                THEN 1
                                                                ELSE 0
                                                            END
                                                  END
                                        END
                               AND Ell_EncodedPayLeaveHr = 0
                               AND Ell_EncodedNoPayLeaveHr = 0
                               AND Ell_EmployeeID = '{0}'
                               AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                             FROM T_EmployeeLeaveAvailment
                                                            WHERE Elt_EmployeeID = '{0}'
                                                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Elt_Status NOT IN ('2','4','6','8','C')
                                                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailment
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0'
							                                                                 UNION 
							                                                                SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailmentHist
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0')
                                                            UNION
                                                           SELECT Elt_LeaveDate
                                                             FROM T_EmployeeLeaveAvailmentHist
                                                            WHERE Elt_EmployeeID = '{0}'
                                                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Elt_Status NOT IN ('2','4','6','8','C')
                                                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailment
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0'
							                                                                 UNION 
							                                                                SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailmentHist
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0')
                                                            UNION
                                                           SELECT Eln_LeaveDate
                                                             FROM T_EmployeeLeaveNotice
                                                            WHERE Eln_EmployeeID = '{0}'
                                                              AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Eln_Status NOT IN ('2','4','6','8','C'))

                             UNION

                            SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Leave Date]
                                 , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                                 , Ell_DayCode [Day Code]
                                 , DefaultShift.Scm_ShiftCode [Shift Code]
                                 , '[' + DefaultShift.Scm_ShiftCode + '] '
                                 + LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [Shift Desc]
                                 , LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) [Start Time]
                                 , LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [End Time]
                                 , DefaultShift.Scm_ShiftHours [Hours]
                              FROM T_EmployeeLogLedgerHist
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = Ell_EmployeeId
                              LEFT JOIN T_ShiftCodeMaster ShiftCode
                                ON ShiftCode.Scm_ShiftCode = Ell_ShiftCode
                              LEFT JOIN T_ShiftCodeMaster DefaultShift
                                ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                               AND DefaultShift.Scm_DefaultShift = 'True'
                               AND DefaultShift.Scm_Status = 'A'
                             WHERE Ell_ProcessDate BETWEEN Convert(datetime, '{1}') AND Convert(datetime, '{2}')
                               AND 1 = CASE WHEN '{3}' = 'OB'
                                            THEN 1
                                            ELSE CASE WHEN (Emt_PayrollType <> 'D' AND Ell_DayCode = 'REG') 
                                                      THEN 1
                                                      ELSE CASE WHEN (Emt_PayrollType = 'D' AND (Ell_DayCode = 'SPL' OR  Ell_DayCode = 'CMPY' OR Ell_DayCode = 'REG'))
                                                                THEN 1
                                                                ELSE 0
                                                            END
                                                  END
                                        END
                               AND Ell_EncodedPayLeaveHr = 0
                               AND Ell_EncodedNoPayLeaveHr = 0
                               AND Ell_EmployeeID = '{0}'
                               AND Ell_ProcessDate NOT IN (SELECT Elt_LeaveDate
                                                             FROM T_EmployeeLeaveAvailment
                                                            WHERE Elt_EmployeeID = '{0}'
                                                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Elt_Status NOT IN ('2','4','6','8','C')
                                                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailment
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0'
							                                                                 UNION 
							                                                                SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailmentHist
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0')
                                                            UNION
                                                           SELECT Elt_LeaveDate
                                                             FROM T_EmployeeLeaveAvailmentHist
                                                            WHERE Elt_EmployeeID = '{0}'
                                                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Elt_Status NOT IN ('2','4','6','8','C')
                                                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailment
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0'
							                                                                 UNION 
							                                                                SELECT Elt_RefControlNo
								                                                              FROM T_EmployeeLeaveAvailmentHist
							                                                                 WHERE Elt_EmployeeId = '{0}'
                                                                                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
								                                                               AND Elt_Status = '0')
                                                            UNION
                                                           SELECT Eln_LeaveDate
                                                             FROM T_EmployeeLeaveNotice
                                                            WHERE Eln_EmployeeID = '{0}'
                                                              AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,'{1}')) AND dateadd(day, 1, Convert(datetime,'{2}'))
                                                              AND Eln_Status NOT IN ('2','4','6','8','C')) 

                             UNION
                    
                            SELECT Convert(varchar(10), x_date, 101) [Leave Date]
                                 , LEFT(UPPER(datename(DW, x_date)), 3) [DoW]
                                 , 'REG' [Day Code]
                                 , DefaultShift.Scm_ShiftCode [Shift Code]
                                 , '[' + DefaultShift.Scm_ShiftCode + '] '
                                 + LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) + ' - '
                                 + LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [Shift Desc]
                                 , LEFT(DefaultShift.Scm_ShiftTimeIn, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeIn, 2) [Start Time]
                                 , LEFT(DefaultShift.Scm_ShiftTimeOut, 2) +':'+ RIGHT(DefaultShift.Scm_ShiftTimeOut, 2) [End Time]
                                 , DefaultShift.Scm_ShiftHours [Hours]
                              FROM {5}
                             INNER JOIN T_EmployeeMaster
                                ON Emt_EmployeeId = @EMPID
                              LEFT JOIN T_ShiftCodeMaster ShiftCode
                                ON ShiftCode.Scm_ShiftCode = Emt_ShiftCode
                              LEFT JOIN T_ShiftCodeMaster DefaultShift
                                ON DefaultShift.Scm_ScheduleType = ShiftCode.Scm_ScheduleType
                               AND DefaultShift.Scm_DefaultShift = 'True'
                               AND DefaultShift.Scm_Status = 'A'
                             WHERE x_date NOT IN (SELECT Hmt_HolidayDate FROM T_HolidayMaster)   --NOT HOlIDAY
                               AND x_date > (SELECT MAX(Ell_ProcessDate)
				                               FROM T_EmployeeLogLedger
				                              WHERE Ell_EmployeeID = @EMPID)
                               AND x_date NOT IN ( SELECT Elt_LeaveDate
						                             FROM T_EmployeeLeaveAvailment
						                            WHERE Elt_EmployeeID = @EMPID
						                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
						                              AND Elt_Status NOT IN ('2','4','6','8','C')
						                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
														                              FROM T_EmployeeLeaveAvailment
														                             WHERE Elt_EmployeeId = @EMPID
														                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                               AND Elt_Status = '0'
														                             UNION 
														                            SELECT Elt_RefControlNo
														                              FROM T_EmployeeLeaveAvailmentHist
														                             WHERE Elt_EmployeeId = @EMPID
														                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                               AND Elt_Status = '0')
						                            UNION
					                               SELECT Elt_LeaveDate
						                             FROM T_EmployeeLeaveAvailmentHist
						                            WHERE Elt_EmployeeID = @EMPID
						                              AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
						                              AND Elt_Status NOT IN ('2','4','6','8','C')
						                              AND Elt_RefControlNo NOT IN ( SELECT Elt_RefControlNo
														                              FROM T_EmployeeLeaveAvailment
														                             WHERE Elt_EmployeeId = @EMPID
														                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                               AND Elt_Status = '0'
														                             UNION 
														                            SELECT Elt_RefControlNo
														                              FROM T_EmployeeLeaveAvailmentHist
														                             WHERE Elt_EmployeeId = @EMPID
														                               AND Elt_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
														                               AND Elt_Status = '0')
                                                                                     UNION
                                                                                    SELECT Eln_LeaveDate
                                                                                      FROM T_EmployeeLeaveNotice
                                                                                     WHERE Eln_EmployeeID = '{0}'
                                                                                       AND Eln_LeaveDate BETWEEN dateadd(day, -1, Convert(datetime,@START)) AND dateadd(day, 1, Convert(datetime,@END))
                                                                                       AND Eln_Status NOT IN ('2','4','6','8','C')) )AS TEMP

                             --GROUP BY [Leave Date]
                             --       , [DoW]
                             --       , [Day Code]
                             --       , [Shift Code]
                             --       , [Shift Desc]
                             --       , [Start Time]
                             --       , [End Time]
                             --       , [Hours]

                            DECLARE @SUMHOURS AS DECIMAL(16, 2)
							SET @SUMHOURS = ( 
								SELECT sum( CASE WHEN @DAYSEL = 1 THEN @WH ELSE [Hours] END )
								 FROM #TEMPTABLELEAVES )
                            SELECT 
                            CASE WHEN ROW_NUMBER() OVER (ORDER BY Convert(datetime,[Leave Date])) <= {4}/( CASE WHEN @DAYSEL = 1 THEN CASE WHEN @WH = 0 THEN [Hours] ELSE @WH END ELSE [Hours] END )
                                    THEN '{3}'  
                                    ELSE CASE WHEN (@withCredit = '1' )
                                              THEN '{6}' 
                                              ELSE '{3}'
                                          END 
                                END [Leave Type]
                             ,  * FROM #TEMPTABLELEAVES

                              DROP TABLE #TEMPTABLELEAVES
                              DROP TABLE {5} ", idNumber
                                              , fromMMDDYYY
                                              , toMMDDYYYY
                                              , leaveType.Substring(0, 2)
                                              , credits
                                              , "_" + idNumber + fromMMDDYYY.Replace("/", string.Empty)
                                              , Resources.Resource.UNPAIDLVECODE);
                #endregion
            }

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    CommonMethods.ErrorsToTextFile(ex, HttpContext.Current.Session["userLogged"].ToString());
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

        //Leave Cancellation
        public bool isPastLeaveDate(string leaveDate)
        {
            bool flag = new bool();
            string sql = string.Format(@"
                        SELECT CASE WHEN ('{0}' < Ppm_StartCycle)
	                           THEN 'TRUE'
	                           ELSE 'FALSE' END AS [Flag]
                          FROM T_PayPeriodMaster
                         WHERE Ppm_CycleIndicator = 'C'", leaveDate);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text));
                }
                catch
                {
                    flag = false; ;
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return flag;
        }

        public bool isPaidLeave(string leaveType)
        {
            bool flag = new bool();
            string sql = string.Format(@"
                        SELECT Ltm_PaidLeave
                          FROM T_LeaveTypeMaster
                         WHERE Ltm_LeaveType = '{0}'", leaveType.Substring(0,2));

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    flag = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text));
                }
                catch
                {
                    flag = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return flag;
        }

        public string getLeaveFlagCancellation(string leaveDate)
        {   
            #region SQL Query
            string sqlQuery = string.Format(@" SELECT CASE WHEN '{0}' < Ppm_StartCycle
			                                          THEN 'P'
			                                          WHEN '{0}' <= Ppm_EndCycle
			                                          THEN 'C'
			                                          ELSE 'F' END 
                                                 FROM T_PayPeriodMaster
                                                WHERE Ppm_CycleIndicator = 'C'", leaveDate);
            #endregion
            string temp = string.Empty;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    temp = Convert.ToString(dal.ExecuteScalar(sqlQuery, CommandType.Text));
                }
                catch
                {
                    temp = "";
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return temp;
        }

        private string getEmployeeShift(string MMddyyyy, string employeeID)
        {
            string sql = @" SELECT Ell_ShiftCode [Shift]
                              FROM T_EmployeeLogLedger
                             WHERE Ell_ProcessDate = '{0}' 
                               AND Ell_EmployeeId = '{1}'
 
                             UNION

                            SELECT Ell_ShiftCode
                              FROM T_EmployeeLogLedgerHist
                             WHERE Ell_ProcessDate = '{0}' 
                               AND Ell_EmployeeId = '{1}' 
                            
                            SELECT Emt_ShiftCode [Shift]
                              FROM T_EmployeeMaster
                             WHERE Emt_EmployeeId = '{1}' ";

            string retValue = string.Empty;
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, MMddyyyy, employeeID), CommandType.Text);
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

            if (ds.Tables[0].Rows.Count > 0)
            {
                //From Logledger
                retValue = ds.Tables[0].Rows[0]["Shift"].ToString();
            }
            else
            {
                //From Employee Master
                retValue = ds.Tables[1].Rows[0]["Shift"].ToString();
            }

            return retValue;
        }

        public static void CorrectLeaveLedger()
        {
            return;
            CorrectLeaveLedger(null);
        }
        public static void CorrectLeaveLedger(DALHelper dal)
        {
            return;
            #region Prev SQL
//            string sql = @"     /*STEP 1*/
//                                DECLARE @RefreshDate datetime
//                                DECLARE @LeaveYear int
//                                DECLARE @EmployeeID as varchar(15)
//                                SET @LeaveYear = ( SELECT Pmt_NumericValue 
//                                                     FROM T_ParameterMaster
//                                                    WHERE Pmt_ParameterID = 'LEAVEYEAR')
//                    
//                                SET @RefreshDate = CONVERT(datetime, '{0}' + '/' + CONVERT(varchar(4), @LeaveYear + 1))
//
//                                CREATE TABLE #Leave
//                                (idnumber varchar(15), 
//                                leavedate datetime,
//                                leavetype varchar(2), 
//                                leavehour decimal(5,2), 
//                                status char(1),
//                                fromWhere char(1))
//                                INSERT INTO #Leave
//                                SELECT Elt_EmployeeID
//                                     , Elt_leavedate
//                                     , Elt_LeaveType
//                                     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
//			                                THEN Elt_LeaveHour
//			                                ELSE CASE WHEN Elt_LeaveHour < 0
//					                                  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
//					                                  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
//				                                  END
//	                                    END lvhr
//                                     , Elt_status 
//                                     , 'L'
//                                  FROM T_EmployeeLeaveAvailmentHist
//                                 INNER JOIN T_LeavetypeMaster 
//                                    ON Ltm_LeaveType = Elt_LeaveType
//                                   AND Ltm_WithCredit =  1
//                                  LEFT JOIN T_ParameterMasterExt
//                                    ON Pmx_ParameterID = 'LVDEDUCTN'
//                                   AND Pmx_Classification = Elt_DayUnit
//                                 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
//                                   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
//			                                THEN CASE WHEN Elt_LeaveDate < @RefreshDate
//					                                  THEN DATEPART(YEAR, Elt_LeaveDate) - 1
//					                                  ELSE DATEPART(YEAR, Elt_LeaveDate)
//				                                  END
//		                                    ELSE DATEPART(YEAR, Elt_LeaveDate)
//                                        END  = @LeaveYear
//                                 UNION ALL
//                                SELECT Elt_EmployeeID
//                                     , Elt_leavedate
//                                     , Elt_LeaveType
//                                     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
//			                                THEN Elt_LeaveHour
//			                                ELSE CASE WHEN Elt_LeaveHour < 0
//					                                  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
//					                                  ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
//				                                  END
//	                                    END lvhr
//                                     , Elt_status 
//                                     , 'L'
//                                  FROM T_EmployeeLeaveAvailment
//                                 INNER JOIN T_LeavetypeMaster 
//                                    ON Ltm_LeaveType = Elt_LeaveType
//                                   AND Ltm_WithCredit =  1
//                                  LEFT JOIN T_ParameterMasterExt
//                                    ON Pmx_ParameterID = 'LVDEDUCTN'
//                                   AND Pmx_Classification = Elt_DayUnit
//                                 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
//                                   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
//			                                THEN CASE WHEN Elt_LeaveDate < @RefreshDate
//					                                  THEN DATEPART(YEAR, Elt_LeaveDate) - 1
//					                                  ELSE DATEPART(YEAR, Elt_LeaveDate)
//				                                  END
//		                                    ELSE DATEPART(YEAR, Elt_LeaveDate)
//                                        END  = @LeaveYear
//                                 UNION ALL
//                                SELECT Eln_EmployeeID
//                                     , Eln_LeaveDate    
//                                     , Eln_LeaveType 
//                                     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
//			                                THEN Eln_LeaveHour
//			                                ELSE CASE WHEN Eln_LeaveHour < 0
//					                                  THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
//					                                  ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
//				                                  END
//	                                    END lvhr
//                                     , 'P' 
//                                     , 'N'
//                                  FROM T_EmployeeLeaveNotice
//                                  LEFT JOIN T_EmployeeLeaveAvailment 
//                                    ON T_EmployeeLeaveAvailment.Elt_EmployeeID = Eln_EmployeeID
//                                   AND T_EmployeeLeaveAvailment.Elt_LeaveDate = Eln_LeaveDate
//                                   AND T_EmployeeLeaveAvailment.Elt_LeaveType = Eln_LeaveType
//                                  LEFT JOIN T_EmployeeLeaveAvailmentHist 
//                                    ON T_EmployeeLeaveAvailmentHist.Elt_EmployeeID = Eln_EmployeeID
//                                   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveDate = Eln_LeaveDate
//                                   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveType = Eln_LeaveType
//                                 INNER JOIN T_LeavetypeMaster 
//                                    ON Ltm_LeaveType = Eln_LeaveType
//                                   AND Ltm_WithCredit =  1
//                                  LEFT JOIN T_ParameterMasterExt
//                                    ON Pmx_ParameterID = 'LVDEDUCTN'
//                                   AND Pmx_Classification = Eln_DayUnit
//                                 WHERE Eln_status <> '2'
//                                   AND Eln_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
//                                   AND CASE WHEN DATEPART(YEAR, Eln_LeaveDate) > @LeaveYear
//			                                THEN CASE WHEN Eln_LeaveDate < @RefreshDate
//					                                  THEN DATEPART(YEAR, Eln_LeaveDate) - 1
//					                                  ELSE DATEPART(YEAR, Eln_LeaveDate)
//				                                  END
//		                                    ELSE DATEPART(YEAR, Eln_LeaveDate)
//                                        END  = @LeaveYear
//                                   AND ISNULL(T_EmployeeLeaveAvailment.Elt_Status, T_EmployeeLeaveAvailmentHist.Elt_Status) IS NULL
//
//                                DELETE FROM #Leave
//                                 WHERE status IN ('2','4','6','8')
// 
//                                SELECT * 
//                                  INTO #EmpLeave
//                                  FROM T_EmployeeLeave
//  
//                                UPDATE #EmpLeave
//                                   SET elm_used = 0
//                                     , elm_reserved = 0
//
//                                SELECT idnumber
//                                     , leavetype
//                                     , SUM(leavehour) as Lvhour
//                                     , CASE WHEN status IN ('9','0') 
//			                                THEN 'A' 
//			                                ELSE 'P' 
//		                                END  as status
//                                  INTO #total 
//                                  FROM #Leave
//                                 GROUP BY idnumber
//                                        , leavetype
//                                        , CASE WHEN status IN ('9','0') 
//                                               THEN 'A' 
//                                               ELSE 'P' 
//                                           END  
//
//                                SELECT *
//                                  INTO #Approved 
//                                  FROM #total
//                                 WHERE status='A'
//
//                                SELECT * 
//                                  INTO #Half
//                                  FROM #Approved
//                                 WHERE leavetype = 'EL'
//
//                                UPDATE #EmpLeave
//                                   SET Elm_Used  = Lvhour  
//                                  FROM #EmpLeave
//                                 INNER JOIN #Approved 
//                                    ON #Approved.idnumber = Elm_EmployeeID  COLLATE SQL_Latin1_General_CP1_CI_AS
//                                   AND #Approved.leavetype = Elm_Leavetype COLLATE SQL_Latin1_General_CP1_CI_AS
//
//                                UPDATE #EmpLeave
//                                   SET Elm_Used = Elm_Used + Lvhour 
//                                  FROM #Half 
//                                 INNER JOIN #EmpLeave 
//                                    ON #Half.idnumber   = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS
//                                   AND Elm_Leavetype = 'VL'
//
//                                /*SELECT T_employeeLeave.*
//                                     , #EmpLeave.* 
//                                  FROM T_employeeLeave 
//                                 INNER JOIN #EmpLeave 
//                                    ON T_employeeLeave.Elm_EmployeeID = #EmpLeave.Elm_EmployeeID
//                                   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
//                                 INNER JOIN T_Employeemaster 
//                                    ON Emt_Employeeid = T_employeeLeave.Elm_Employeeid
//                                 WHERE T_employeeLeave.Elm_Used  <> #EmpLeave.Elm_Used
//                                 ORDER BY 1,3 */
//
//                                /*END STEP 1*/
//                                ----------------------------------------------------------------------
//
//
//
//                                /*STEP 2*/
//                                UPDATE T_employeeLeave
//                                   SET T_employeeLeave.Elm_Used = #EmpLeave.Elm_Used
//                                  FROM T_employeeLeave 
//                                 INNER JOIN  #EmpLeave 
//                                    ON T_employeeLeave.Elm_EmployeeID  = #EmpLeave.Elm_EmployeeID
//                                   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
//                                 WHERE T_employeeLeave.Elm_Used  <> #EmpLeave.Elm_Used
//                                /*END STEP 2*/
//                                ----------------------------------------------------------------------
//
//
//
//                                /*STEP 3*/
//                                SELECT *
//                                  INTO #unapproved 
//                                  FROM #total
//                                 WHERE status='P'
//
//                                SELECT * 
//                                  INTO #Halfunapproved
//                                  FROM #unapproved
//                                 WHERE leavetype='EL'
//
//                                UPDATE #EmpLeave
//                                   SET Elm_Reserved  = Elm_Reserved + Lvhour  
//                                  FROM #EmpLeave
//                                 INNER JOIN #unapproved 
//                                    ON #unapproved.idnumber = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS
//                                   AND #unapproved.leavetype = Elm_Leavetype COLLATE SQL_Latin1_General_CP1_CI_AS
//
//
//                                UPDATE #EmpLeave
//                                   SET Elm_Reserved  = Elm_Reserved + Lvhour 
//                                  FROM #EmpLeave 
//                                 INNER JOIN #Halfunapproved 
//                                    ON #Halfunapproved.idnumber = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS   
//                                 WHERE Elm_Leavetype = 'VL'
//
//
//                                /*SELECT T_EmployeeLeave.*
//                                     , #EmpLeave.* 
//                                  FROM T_employeeLeave 
//                                 INNER JOIN  #EmpLeave 
//                                    ON T_employeeLeave.Elm_EmployeeID  = #EmpLeave.Elm_EmployeeID
//                                   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
//                                 INNER JOIN T_Employeemaster 
//                                    ON Emt_Employeeid = T_employeeLeave.Elm_Employeeid
//                                 WHERE T_EmployeeLeave.Elm_Reserved  <> #EmpLeave.Elm_Reserved*/
//
//                                /*END STEP 3*/
//                                ----------------------------------------------------------------------
//
//
//
//                                /*STEP 4*/
//                                UPDATE T_EmployeeLeave
//                                   SET T_EmployeeLeave.Elm_Reserved = #EmpLeave.Elm_Reserved
//                                  FROM T_EmployeeLeave 
//                                 INNER JOIN  #EmpLeave 
//                                    ON T_EmployeeLeave.Elm_EmployeeID = #EmpLeave.Elm_EmployeeID
//                                   AND T_EmployeeLeave.Elm_Leavetype = #EmpLeave.Elm_Leavetype
//                                 WHERE T_EmployeeLeave.Elm_Reserved  <> #EmpLeave.Elm_Reserved
//                                /*END STEP 4*/
//                                ----------------------------------------------------------------------
//
//
//
//                                /*STEP 5*/
//                                DROP TABLE #Leave
//                                DROP TABLE #Approved
//                                DROP TABLE #Half
//                                DROP TABLE #HalfUnApproved
//                                DROP TABLE #unapproved
//                                DROP TABLE #EmpLeave
//                                DROP TABLE #total
//                                /*END STEP 5*/
//                                ";
            #endregion

            #region Revised Query
            string sql = @"
DECLARE @RefreshDate datetime
DECLARE @LeaveYear int
DECLARE @EmployeeID as varchar(15)

if RTRIM('{1}') = ''
BEGIN
    SET @LeaveYear = ( SELECT Pmt_NumericValue 
                         FROM T_ParameterMaster
                        WHERE Pmt_ParameterID = 'LEAVEYEAR')
END
ELSE
BEGIN
    SET @LeaveYear = Convert(int, '{1}')
END

SET @RefreshDate = CONVERT(datetime, '{0}' + '/' + CONVERT(varchar(4), @LeaveYear + 1))

CREATE TABLE #Leave
(idnumber varchar(15), 
leavedate datetime,
leavetype varchar(2), 
leavehour decimal(5,2), 
status char(1),
fromWhere char(1))

INSERT INTO #Leave
SELECT Elt_EmployeeID
     , Elt_leavedate
     , Elt_LeaveType
     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
            THEN Elt_LeaveHour
            ELSE CASE WHEN Elt_LeaveHour < 0
                      THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
                      ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
                  END
        END lvhr
     , Elt_status 
     , 'L'
  FROM T_EmployeeLeaveAvailmentHist
 INNER JOIN T_LeavetypeMaster 
    ON Ltm_LeaveType = Elt_LeaveType
   AND Ltm_WithCredit =  1
  LEFT JOIN T_ParameterMasterExt
    ON Pmx_ParameterID = 'LVDEDUCTN'
   AND Pmx_Classification = Elt_DayUnit
 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
            THEN CASE WHEN Elt_LeaveDate < @RefreshDate
                      THEN DATEPART(YEAR, Elt_LeaveDate) - 1
                      ELSE DATEPART(YEAR, Elt_LeaveDate)
                  END
            ELSE DATEPART(YEAR, Elt_LeaveDate)
        END  = @LeaveYear
 UNION ALL
SELECT Elt_EmployeeID
     , Elt_leavedate
     , Elt_LeaveType
     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
            THEN Elt_LeaveHour
            ELSE CASE WHEN Elt_LeaveHour < 0
                      THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Elt_LeaveHour)) 
                      ELSE ISNULL(Pmx_ParameterValue, Elt_LeaveHour)
                  END
        END lvhr
     , Elt_status 
     , 'L'
  FROM T_EmployeeLeaveAvailment
 INNER JOIN T_LeavetypeMaster 
    ON Ltm_LeaveType = Elt_LeaveType
   AND Ltm_WithCredit =  1
  LEFT JOIN T_ParameterMasterExt
    ON Pmx_ParameterID = 'LVDEDUCTN'
   AND Pmx_Classification = Elt_DayUnit
 WHERE Elt_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
   AND CASE WHEN DATEPART(YEAR, Elt_LeaveDate) > @LeaveYear
            THEN CASE WHEN Elt_LeaveDate < @RefreshDate
                      THEN DATEPART(YEAR, Elt_LeaveDate) - 1
                      ELSE DATEPART(YEAR, Elt_LeaveDate)
                  END
            ELSE DATEPART(YEAR, Elt_LeaveDate)
        END  = @LeaveYear
 UNION ALL
SELECT Eln_EmployeeID
     , Eln_LeaveDate    
     , Eln_LeaveType 
     , CASE WHEN ISNULL(Pmx_ParameterValue, 0) = 0
            THEN Eln_LeaveHour
            ELSE CASE WHEN Eln_LeaveHour < 0
                      THEN -1 * ISNULL(Pmx_ParameterValue, (-1 * Eln_LeaveHour)) 
                      ELSE ISNULL(Pmx_ParameterValue, Eln_LeaveHour)
                  END
        END lvhr
     , 'P' 
     , 'N'
  FROM T_EmployeeLeaveNotice
  LEFT JOIN T_EmployeeLeaveAvailment 
    ON T_EmployeeLeaveAvailment.Elt_EmployeeID = Eln_EmployeeID
   AND T_EmployeeLeaveAvailment.Elt_LeaveDate = Eln_LeaveDate
   AND T_EmployeeLeaveAvailment.Elt_LeaveType = Eln_LeaveType
  LEFT JOIN T_EmployeeLeaveAvailmentHist 
    ON T_EmployeeLeaveAvailmentHist.Elt_EmployeeID = Eln_EmployeeID
   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveDate = Eln_LeaveDate
   AND T_EmployeeLeaveAvailmentHist.Elt_LeaveType = Eln_LeaveType
 INNER JOIN T_LeavetypeMaster 
    ON Ltm_LeaveType = Eln_LeaveType
   AND Ltm_WithCredit =  1
  LEFT JOIN T_ParameterMasterExt
    ON Pmx_ParameterID = 'LVDEDUCTN'
   AND Pmx_Classification = Eln_DayUnit
 WHERE Eln_status <> '2'
   AND Eln_LeaveDate > DATEADD(day, -1 , DATEADD(year, - 1, @RefreshDate))
   AND CASE WHEN DATEPART(YEAR, Eln_LeaveDate) > @LeaveYear
            THEN CASE WHEN Eln_LeaveDate < @RefreshDate
                      THEN DATEPART(YEAR, Eln_LeaveDate) - 1
                      ELSE DATEPART(YEAR, Eln_LeaveDate)
                  END
            ELSE DATEPART(YEAR, Eln_LeaveDate)
        END  = @LeaveYear
   AND ISNULL(T_EmployeeLeaveAvailment.Elt_Status, T_EmployeeLeaveAvailmentHist.Elt_Status) IS NULL

DELETE FROM #Leave
 WHERE status IN ('2','4','6','8')

SELECT * 
  INTO #EmpLeave
  FROM T_EmployeeLeave
  WHERE Elm_LeaveYear = @LeaveYear

UPDATE #EmpLeave
   SET elm_used = 0
     , elm_reserved = 0

SELECT idnumber
     , leavetype
     , SUM(leavehour) as Lvhour
     , CASE WHEN status IN ('9','0') 
            THEN 'A' 
            ELSE 'P' 
        END  as status
  INTO #total 
  FROM #Leave
 GROUP BY idnumber
        , leavetype
        , CASE WHEN status IN ('9','0') 
               THEN 'A' 
               ELSE 'P' 
           END  

SELECT *
  INTO #Approved 
  FROM #total
 WHERE status='A'
 
 
 SELECT #Approved.* , Ltm_PartOfLeave as [MotherLeave]
 INTO #Half
 from #Approved
 inner join T_LeaveTypeMaster on Ltm_LeaveType = leavetype
	and Len(Isnull(Ltm_PartOfLeave,'')) > 0

UPDATE #EmpLeave
   SET Elm_Used  = Lvhour  
  FROM #EmpLeave
 INNER JOIN #Approved 
    ON #Approved.idnumber = Elm_EmployeeID  COLLATE SQL_Latin1_General_CP1_CI_AS
   AND #Approved.leavetype = Elm_Leavetype COLLATE SQL_Latin1_General_CP1_CI_AS


UPDATE #EmpLeave
   SET Elm_Used = Elm_Used + Lvhour 
  FROM #Half 
 INNER JOIN #EmpLeave 
    ON #Half.idnumber   = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS
   AND Elm_Leavetype = [MotherLeave]

/*SELECT T_employeeLeave.*
     , #EmpLeave.* 
  FROM T_employeeLeave 
 INNER JOIN #EmpLeave 
    ON T_employeeLeave.Elm_EmployeeID = #EmpLeave.Elm_EmployeeID
   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
 INNER JOIN T_Employeemaster 
    ON Emt_Employeeid = T_employeeLeave.Elm_Employeeid
 WHERE T_employeeLeave.Elm_Used  <> #EmpLeave.Elm_Used
 ORDER BY 1,3 */

/*END STEP 1*/
----------------------------------------------------------------------



/*STEP 2*/
UPDATE T_employeeLeave
   SET T_employeeLeave.Elm_Used = #EmpLeave.Elm_Used
  FROM T_employeeLeave 
 INNER JOIN  #EmpLeave 
    ON T_employeeLeave.Elm_EmployeeID  = #EmpLeave.Elm_EmployeeID
   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
 WHERE T_employeeLeave.Elm_Used  <> #EmpLeave.Elm_Used
    AND T_employeeLeave.Elm_LeaveYear = @LeaveYear
/*END STEP 2*/
----------------------------------------------------------------------


/*STEP 3*/
SELECT *
  INTO #unapproved 
  FROM #total
 WHERE status='P'

SELECT #unapproved.* , Ltm_PartOfLeave as [MotherLeave]
  INTO #Halfunapproved
  FROM #unapproved
 inner join T_LeaveTypeMaster on Ltm_LeaveType = leavetype
	and Len(Isnull(Ltm_PartOfLeave,'')) > 0


UPDATE #EmpLeave
   SET Elm_Reserved  = Elm_Reserved + Lvhour  
  FROM #EmpLeave
 INNER JOIN #unapproved 
    ON #unapproved.idnumber = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS
   AND #unapproved.leavetype = Elm_Leavetype COLLATE SQL_Latin1_General_CP1_CI_AS


UPDATE #EmpLeave
   SET Elm_Reserved  = Elm_Reserved + Lvhour 
  FROM #EmpLeave 
 INNER JOIN #Halfunapproved 
    ON #Halfunapproved.idnumber = Elm_EmployeeID COLLATE SQL_Latin1_General_CP1_CI_AS   
 WHERE Elm_Leavetype = [MotherLeave]


/*SELECT T_EmployeeLeave.*
     , #EmpLeave.* 
  FROM T_employeeLeave 
 INNER JOIN  #EmpLeave 
    ON T_employeeLeave.Elm_EmployeeID  = #EmpLeave.Elm_EmployeeID
   AND T_employeeLeave.Elm_Leavetype= #EmpLeave.Elm_Leavetype
 INNER JOIN T_Employeemaster 
    ON Emt_Employeeid = T_employeeLeave.Elm_Employeeid
 WHERE T_EmployeeLeave.Elm_Reserved  <> #EmpLeave.Elm_Reserved*/

/*END STEP 3*/
----------------------------------------------------------------------



/*STEP 4*/
UPDATE T_EmployeeLeave
   SET T_EmployeeLeave.Elm_Reserved = #EmpLeave.Elm_Reserved
  FROM T_EmployeeLeave 
 INNER JOIN  #EmpLeave 
    ON T_EmployeeLeave.Elm_EmployeeID = #EmpLeave.Elm_EmployeeID
   AND T_EmployeeLeave.Elm_Leavetype = #EmpLeave.Elm_Leavetype
 WHERE T_EmployeeLeave.Elm_Reserved  <> #EmpLeave.Elm_Reserved
    AND T_employeeLeave.Elm_LeaveYear = @LeaveYear
/*END STEP 4*/
----------------------------------------------------------------------



/*STEP 5*/
DROP TABLE #Leave
DROP TABLE #Approved
DROP TABLE #Half
DROP TABLE #HalfUnApproved
DROP TABLE #unapproved
DROP TABLE #EmpLeave
DROP TABLE #total
/*END STEP 5*/
            ";
            #endregion

            #region Get All Distinct Leave Years 
            string sqlLeaveYears = @"
SELECT
	distinct 
	Elm_LeaveYear
FROM T_EmployeeLeave
WHERE Elm_LeaveYear > (
	SELECT 
		Pmt_NumericValue 
    FROM T_ParameterMaster
    WHERE Pmt_ParameterID = 'LEAVEYEAR'
)
            ";
            #endregion

            if (dal != null)
            {
                dal.ExecuteNonQuery(string.Format(sql, Resources.Resource.LEAVEREFRESH, string.Empty), CommandType.Text);
                DataSet dsLeaveYears = dal.ExecuteDataSet(sqlLeaveYears, CommandType.Text);
                for (int i = 0; !CommonMethods.isEmpty(dsLeaveYears) && i < dsLeaveYears.Tables[0].Rows.Count; i++)
                {
                    dal.ExecuteNonQuery(string.Format(sql, Resources.Resource.LEAVEREFRESH, dsLeaveYears.Tables[0].Rows[i][0].ToString().Trim()), CommandType.Text);
                }
            }
            else
            {
                using (DALHelper _dal = new DALHelper())
                {
                    try
                    {
                        _dal.OpenDB();
                        _dal.BeginTransactionSnapshot();
                        _dal.ExecuteNonQuery(string.Format(sql, Resources.Resource.LEAVEREFRESH, string.Empty), CommandType.Text);

                        DataSet dsLeaveYears = _dal.ExecuteDataSet(sqlLeaveYears, CommandType.Text);
                        for (int i = 0; !CommonMethods.isEmpty(dsLeaveYears) && i < dsLeaveYears.Tables[0].Rows.Count; i++)
                        {
                            _dal.ExecuteNonQuery(string.Format(sql, Resources.Resource.LEAVEREFRESH, dsLeaveYears.Tables[0].Rows[i][0].ToString().Trim()), CommandType.Text);
                        }

                        _dal.CommitTransactionSnapshot();
                    }
                    catch(Exception ex)
                    {
                        _dal.RollBackTransactionSnapshot();
                        CommonMethods.ErrorsToTextFile(ex, "CorrectLeaveLedger");
                    }
                    finally
                    {
                        _dal.CloseDB();
                    }
                }
            }
        }

        public static bool IsUnpaidLeaveInMaster()
        {
            bool exist = false;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    exist = Convert.ToBoolean( dal.ExecuteScalar(string.Format("SELECT 'True' FROM T_LeaveTypeMaster WHERE Ltm_LeaveType = '{0}'", Resources.Resource.UNPAIDLVECODE)));
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, "CorrectLeaveLedger");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return exist;
        }
    }
}