using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MethodsLibrary;
using Payroll.DAL;

/// <summary>
/// Summary description for JobSplitBL
/// </summary>
namespace Payroll.DAL
{
    public class JobSplitBL
    {
        public JobSplitBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void CreateJSHeader(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[8];
            paramDetails[0] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsh_EmployeeId", rowDetails["Jsh_EmployeeId"]);
            paramDetails[2] = new ParameterInfo("@Jsh_JobSplitDate", rowDetails["Jsh_JobSplitDate"]);
            paramDetails[3] = new ParameterInfo("@Jsh_Entry", rowDetails["Jsh_Entry"]);
            paramDetails[4] = new ParameterInfo("@Jsh_Costcenter", rowDetails["Jsh_Costcenter"]);
            paramDetails[5] = new ParameterInfo("@Jsh_RefControlNo", rowDetails["Jsh_RefControlNo"]);
            paramDetails[6] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            paramDetails[7] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            #endregion
            #region SQL Query
            string sqlInsert = @"
                                    DECLARE @Costcenter as varchar(10)
                                    set @Costcenter = (SELECT Emt_CostcenterCode FROM T_EmployeeMaster
				                                       WHERE Emt_EmployeeID = @Jsh_EmployeeId)

                                    INSERT INTO T_JobSplitHeader
                                    (
                                          Jsh_ControlNo
                                        , Jsh_EmployeeId
                                        , Jsh_JobSplitDate
                                        , Jsh_AppliedDate
                                        , Jsh_Entry
                                        , Jsh_Costcenter
                                        , Jsh_RefControlNo
                                        , Jsh_Status
                                        , Usr_Login
                                        , Ludatetime
                                    )
                                    VALUES
                                    (
                                          @Jsh_ControlNo
                                        , @Jsh_EmployeeId
                                        , @Jsh_JobSplitDate
                                        , getdate()
                                        , @Jsh_Entry
                                        , @Costcenter
                                        , @Jsh_RefControlNo
                                        , @Jsh_Status
                                        , @Usr_Login
                                        , getdate()
                                    )";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void InsertDetails(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Jsd_ControlNo", rowDetails["Jsd_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsd_Seqno", rowDetails["Jsd_Seqno"]);
            paramDetails[2] = new ParameterInfo("@Jsd_StartTime", rowDetails["Jsd_StartTime"]);
            paramDetails[3] = new ParameterInfo("@Jsd_EndTime", rowDetails["Jsd_EndTime"]);
            paramDetails[4] = new ParameterInfo("@Jsd_JobCode", rowDetails["Jsd_JobCode"]);
            paramDetails[5] = new ParameterInfo("@Jsd_ClientJobNo", rowDetails["Jsd_ClientJobNo"]);
            paramDetails[6] = new ParameterInfo("@Jsd_PlanHours", rowDetails["Jsd_PlanHours"]);
            paramDetails[7] = new ParameterInfo("@Jsd_ActHours", rowDetails["Jsd_ActHours"]);
            paramDetails[8] = new ParameterInfo("@Jsd_Status", rowDetails["Jsd_Status"]);
            paramDetails[9] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Billable
            paramDetails[11] = new ParameterInfo("@Jsd_Category", rowDetails["Jsd_Category"]);

            paramDetails[10] = new ParameterInfo("@Jsd_SubWorkCode", rowDetails["Jsd_SubWorkCode"]);
            paramDetails[12] = new ParameterInfo("@Jsd_Overtime", rowDetails["Jsd_Overtime"]);
            paramDetails[13] = new ParameterInfo("@Jsd_CostCenter", rowDetails["Jsd_CostCenter"]);
            #endregion
            #region SQL Query
            string sqlInsert = @"
                                    INSERT INTO T_JobSplitDetail
                                    (
                                          Jsd_ControlNo
                                        , Jsd_Seqno
                                        , Jsd_StartTime
                                        , Jsd_EndTime
                                        , Jsd_JobCode
                                        , Jsd_ClientJobNo
                                        , Jsd_CostCenter
                                        , Jsd_PlanHours
                                        , Jsd_ActHours
                                        , Jsd_Status
                                        , Jsd_Category --added by Manny 12212010
                                        , Usr_Login
                                        , Ludatetime
                                        , Jsd_SubWorkCode
                                        , Jsd_Overtime
                                    )
                                    VALUES
                                    (
                                          @Jsd_ControlNo
                                        , @Jsd_Seqno
                                        , @Jsd_StartTime
                                        , @Jsd_EndTime
                                        , @Jsd_JobCode
                                        , @Jsd_ClientJobNo
                                        --, CASE @Jsd_CostCenter
			                            --    WHEN 'ALL' THEN (SELECT Jsh_Costcenter from T_JobSplitHeader where Jsh_ControlNo = @Jsd_ControlNo)
		                                --    ELSE @Jsd_CostCenter
                                        --  END
                                        , @Jsd_CostCenter
                                        , @Jsd_PlanHours
                                        , @Jsd_ActHours
                                        , @Jsd_Status
                                        , @Jsd_Category
                                        , @Usr_Login
                                        , getdate()
                                        , @Jsd_SubWorkCode
                                        , @Jsd_Overtime
                                    )";
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateJSHeader(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[3];
            paramDetails[0] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[2] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            #endregion
            #region SQL Query
            string sqlUpdate = @"
                                    UPDATE T_JobSplitHeader
                                    SET
                                          Jsh_Status = @Jsh_Status
                                        , Usr_Login = @Usr_Login
                                        , Ludatetime = getdate()
                                    WHERE
                                        Jsh_ControlNo = @Jsh_ControlNo
                                    ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);

        }

        public void UpdateModHeader(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[6];
            paramDetails[0] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsh_CheckedBy", rowDetails["Jsh_CheckedBy"]);
            paramDetails[2] = new ParameterInfo("@Jsh_Checked2By", rowDetails["Jsh_Checked2By"]);
            paramDetails[3] = new ParameterInfo("@Jsh_ApprovedBy", rowDetails["Jsh_ApprovedBy"]);
            paramDetails[4] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[5] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            #endregion
            #region SQL Query
            string sqlUpdate = @"
                                    UPDATE T_JobSplitHeader
                                    SET
                                          Jsh_Status = @Jsh_Status
                                        , Usr_Login = @Usr_Login
                                        , Ludatetime = getdate()
                                    WHERE
                                        Jsh_ControlNo = @Jsh_ControlNo
                                    ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);

        }

        public void UpdateModHeaderRoute(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];
            if (rowDetails["Jsh_Status"].ToString().Equals("3"))
            {
                fieldUsed = @"  Jsh_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Jsh_CheckedBy", " ");
            }
            else if (rowDetails["Jsh_Status"].ToString().Equals("5")
                || rowDetails["Jsh_Status"].ToString().Equals("4"))
            {
                fieldUsed = @"  Jsh_CheckedBy = @Jsh_CheckedBy ,
                                            Jsh_CheckedDate = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_CheckedBy", rowDetails["Jsh_CheckedBy"]);
            }
            else if (rowDetails["Jsh_Status"].ToString().Equals("7")
                || rowDetails["Jsh_Status"].ToString().Equals("6"))
            {
                fieldUsed = @"  Jsh_Checked2By = @Jsh_Checked2By ,
                                            Jsh_Checked2Date = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_Checked2By", rowDetails["Jsh_Checked2By"]);
            }
            else if (rowDetails["Jsh_Status"].ToString().Equals("9")
                || rowDetails["Jsh_Status"].ToString().Equals("8"))
            {
                fieldUsed = @"  Jsh_ApprovedBy = @Jsh_ApprovedBy ,
                                            Jsh_ApprovedDate = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_ApprovedBy", rowDetails["Jsh_ApprovedBy"]);
            }
            else
            {
                paramDetails[0] = new ParameterInfo("@Jsh_ApprovedBy", " ");
            }

            paramDetails[1] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlInsert = string.Format(@"
                                    UPDATE T_JobSplitHeader
                                    SET
                                        {0}
                                        Jsh_Status = @Jsh_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Jsh_ControlNo = @Jsh_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }
        
        // added by kelvin
        public void UpdateModHeaderRoute(DataRow rowDetails, DALHelper dal, string buttonName)
        {
            #region Parameters
            string fieldUsed = string.Empty;
            ParameterInfo[] paramDetails = new ParameterInfo[4];

            if (buttonName.Trim().ToUpper() == "ENDORSE TO CHECKER 1")
            {
                fieldUsed = @"  Jsh_EndorsedDateToChecker = getdate(),";
                paramDetails[0] = new ParameterInfo("@Jsh_CheckedBy", " ");
            }
            else if (buttonName.Trim().ToUpper() == "ENDORSE TO CHECKER 2"
                || rowDetails["Jsh_Status"].ToString().Equals("4"))
            {
                fieldUsed = @"  Jsh_CheckedBy = @Jsh_CheckedBy ,
                                Jsh_CheckedDate = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_CheckedBy", rowDetails["Jsh_CheckedBy"]);
            }
            else if (buttonName.Trim().ToUpper() == "ENDORSE TO APPROVER"
                || rowDetails["Jsh_Status"].ToString().Equals("6"))
            {
                fieldUsed = @"  Jsh_Checked2By = @Jsh_Checked2By ,
                                Jsh_Checked2Date = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_Checked2By", rowDetails["Jsh_Checked2By"]);
            }
            else if (buttonName.Trim().ToUpper() == "APPROVE"
                || rowDetails["Jsh_Status"].ToString().Equals("8"))
            {
                fieldUsed = @"  Jsh_ApprovedBy = @Jsh_ApprovedBy ,
                                Jsh_ApprovedDate = getdate() ,";
                paramDetails[0] = new ParameterInfo("@Jsh_ApprovedBy", rowDetails["Jsh_ApprovedBy"]);
            }
            else
            {
                paramDetails[0] = new ParameterInfo("@Jsh_ApprovedBy", " ");
            }

            paramDetails[1] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            #endregion

            #region SQL Query
            string sqlInsert = string.Format(@"
                                    UPDATE T_JobSplitHeader
                                    SET
                                        {0}
                                        Jsh_Status = @Jsh_Status,
                                        Usr_Login = @Usr_Login,
                                        Ludatetime = getdate()
                                    WHERE Jsh_ControlNo = @Jsh_ControlNo", fieldUsed);
            #endregion

            dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramDetails);
        }

        public void UpdateRefHeader(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[4];
            paramDetails[0] = new ParameterInfo("@Jsh_ControlNo", rowDetails["Jsh_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsh_Entry", rowDetails["Jsh_Entry"]);
            paramDetails[2] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            paramDetails[3] = new ParameterInfo("@Jsh_Status", rowDetails["Jsh_Status"]);
            #endregion
            #region SQL Query
            string sqlUpdate = @"
                                    UPDATE T_JobSplitHeader
                                    SET
                                          --Jsh_Entry = @Jsh_Entry  
                                          Jsh_Status = @Jsh_Status
                                        , Usr_Login = @Usr_Login
                                        , Ludatetime = getdate()
                                    WHERE
                                        Jsh_ControlNo = @Jsh_ControlNo
                                    ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateDetails(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[14];
            paramDetails[0] = new ParameterInfo("@Jsd_ControlNo", rowDetails["Jsd_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsd_Seqno", rowDetails["Jsd_Seqno"]);
            paramDetails[2] = new ParameterInfo("@Jsd_StartTime", rowDetails["Jsd_StartTime"]);
            paramDetails[3] = new ParameterInfo("@Jsd_EndTime", rowDetails["Jsd_EndTime"]);
            paramDetails[4] = new ParameterInfo("@Jsd_JobCode", rowDetails["Jsd_JobCode"]);
            paramDetails[5] = new ParameterInfo("@Jsd_ClientJobNo", rowDetails["Jsd_ClientJobNo"]);
            paramDetails[6] = new ParameterInfo("@Jsd_PlanHours", rowDetails["Jsd_PlanHours"]);
            paramDetails[7] = new ParameterInfo("@Jsd_ActHours", rowDetails["Jsd_ActHours"]);
            paramDetails[8] = new ParameterInfo("@Jsd_Status", rowDetails["Jsd_Status"]);
            paramDetails[9] = new ParameterInfo("@Usr_Login", rowDetails["Usr_Login"]);
            //Billable
            paramDetails[11] = new ParameterInfo("@Jsd_Category", rowDetails["Jsd_Category"]);

            paramDetails[10] = new ParameterInfo("@Jsd_SubWorkCode", rowDetails["Jsd_SubWorkCode"]);
            paramDetails[12] = new ParameterInfo("@Jsd_Overtime", rowDetails["Jsd_Overtime"]);
            paramDetails[13] = new ParameterInfo("@Jsd_CostCenter", rowDetails["Jsd_CostCenter"]);
            #endregion
            #region SQL Query
            string sqlUpdate = @"
                                    UPDATE T_JobSplitDetail
                                    SET
                                          Jsd_StartTime = @Jsd_StartTime
                                        , Jsd_EndTime = @Jsd_EndTime
                                        , Jsd_JobCode = @Jsd_JobCode
                                        , Jsd_ClientJobNo = @Jsd_ClientJobNo
                                        --, Jsd_CostCenter = CASE @Jsd_CostCenter
			                            --                        WHEN 'ALL' THEN (SELECT Jsh_Costcenter from T_JobSplitHeader where Jsh_ControlNo = @Jsd_ControlNo)
		                                --                        ELSE @Jsd_CostCenter
                                        --                   END
                                        , Jsd_CostCenter = @Jsd_CostCenter
                                        , Jsd_PlanHours = @Jsd_PlanHours
                                        , Jsd_Category = @Jsd_Category
                                        , Jsd_Status = @Jsd_Status
                                        , Usr_Login = @Usr_Login
                                        , Ludatetime = getdate()

                                        , Jsd_SubWorkCode = @Jsd_SubWorkCode
                                        , Jsd_Overtime = @Jsd_Overtime
                                    WHERE
                                        Jsd_ControlNo = @Jsd_ControlNo
                                    AND
                                        Jsd_Seqno = @Jsd_Seqno
                                        
                                    ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void DeleteDetails(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[3];
            paramDetails[0] = new ParameterInfo("@Jsd_ControlNo", rowDetails["Jsd_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Jsd_Seqno", rowDetails["Jsd_Seqno"]);
            paramDetails[2] = new ParameterInfo("@Jsd_Status", rowDetails["Jsd_Status"]);
            #endregion
            #region SQL Query
            string sqlUpdate = @"
                                    DELETE FROM T_JobSplitDetail
                                    WHERE
                                        Jsd_ControlNo = @Jsd_ControlNo
                                    AND
                                        Jsd_Seqno = @Jsd_Seqno
                                    AND 
                                        Jsd_Status = @Jsd_Status
                                    ";
            #endregion

            dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramDetails);
        }

        public void UpdateDetailsStatus(string status, string controlNo, DALHelper dal)
        {
            string sql = @"
                                UPDATE  t_JobSplitDetail
                                SET
                                    Jsd_Status = '{0}'
                                WHERE
                                    Jsd_ControlNo = '{1}'
                                ";
            dal.ExecuteNonQuery(string.Format(sql, status, controlNo), CommandType.Text);
        }

        public DataSet LoadCurrentJobSplitDetails(string date, string empId, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string sql = @"
                                SELECT Jsh_ControlNo
                                     , Jsh_Status
                                     , Jsd_Seqno
                                     , Jsd_StartTime
                                     , Jsd_EndTime
                                     , Jsd_PlanHours
                                     , Jsd_JobCode
                                     , Jsd_ClientJobNo
                                     , Slm_ClientJobName
                                     , Jsd_SubWorkCode
									 , Slm_Category
									 , Jsd_Costcenter
                                     , Jsd_Category
                                     , Jsd_Overtime
                                  FROM T_JobSplitHeader
                                  LEFT JOIN T_JobSplitdetail
                                    ON Jsd_ControlNo = Jsh_ControlNo
                                  LEFT JOIN T_Salesmaster 
                                    ON Slm_DashJobCode = Jsd_JobCode
                                   AND Slm_ClientJobNo = Jsd_ClientJobNo
                                   AND (Slm_Costcenter = Jsd_Costcenter OR Slm_Costcenter = 'ALL')
                                 WHERE Jsh_EmployeeId = '{0}'
                                   AND Convert(varchar(20), Jsh_JobSplitDate, 101) = '{1}'
                                   AND  ( (Jsh_Status = '1' AND  Jsh_RefControlNo = '')
                                       OR (Jsh_Status = '9'))";

            ds = dal.ExecuteDataSet(string.Format(sql, empId, date), CommandType.Text);

            return ds;
        }
        
        public string GetControlNumber(string date, string empId, DALHelper dal)
        {
            string cn = string.Empty;
            string sql = @"
                                SELECT Jsh_ControlNo
                                  FROM T_JobSplitHeader
                                 WHERE Jsh_EmployeeId = '{0}'
                                   AND Convert(varchar(20), Jsh_JobSplitDate, 101) = '{1}'
                                   AND  ((Jsh_Status = '1' AND  Jsh_RefControlNo = '' AND left(Jsh_ControlNo, 1) = 'S')
                                    OR (Jsh_Status = '1' AND left(Jsh_ControlNo, 1) = 'J'
                                        AND Jsh_JobSplitDate < CURRENT_TIMESTAMP)
                                    OR (Jsh_Status = '9'))"; 
                                       
            cn = Convert.ToString(dal.ExecuteScalar(string.Format(sql, empId, date), CommandType.Text));

            return cn;
        }

        public string GetControlNumber(string controlNo, DALHelper dal)
        {
            string cn = string.Empty;
            string sql = @"
                                SELECT 
                                      Jsh_ControlNo
                                FROM 
                                    T_JobSplitHeader
                                WHERE
                                    Jsh_RefControlNo = '{0}'
                                AND
                                    Jsh_Status NOT IN ('2','4','6','8');
                                ";
            cn = Convert.ToString(dal.ExecuteScalar(string.Format(sql, controlNo), CommandType.Text));

            return cn;
        }

        public string GetControlNumberRouted(string date, string empId ,DALHelper dal)
        {
            string cn = string.Empty;
            string sql = @"
                                SELECT 
                                      Jsh_ControlNo
                                FROM 
                                    T_JobSplitHeader
                                WHERE
                                    convert(varchar(10),Jsh_JobSplitDate,101) = '{0}'
                                AND
                                    Jsh_Employeeid = '{1}'
                                AND
                                    Jsh_Status IN ('3','5','7');
                                ";
            cn = Convert.ToString(dal.ExecuteScalar(string.Format(sql, date, empId), CommandType.Text));

            return cn;
        }

        public string GetRefControlNumber(string controlNo, DALHelper dal)
        {
            string cn = string.Empty;
            string sql = @"
                                SELECT 
                                      Jsh_RefControlNo
                                FROM 
                                    T_JobSplitHeader
                                WHERE
                                    Jsh_ControlNo = '{0}'
                                ";
            cn = Convert.ToString(dal.ExecuteScalar(string.Format(sql, controlNo), CommandType.Text));

            return cn;
        }

        public DataSet LoadCurrentJobSplitDetails(string controlNo, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string sql = @"
                               SELECT Jsh_ControlNo
                                    , Jsh_Status
                                    , Jsd_Seqno
                                    , Jsd_StartTime
                                    , Jsd_EndTime
                                    , Jsd_PlanHours
                                    , Jsd_JobCode
                                    , Jsd_ClientJobNo
                                    , Slm_ClientJobName
                                    , Jsd_SubWorkCode
                                    , Slm_Category
								    , Jsd_Costcenter
								    , Jsd_Category --Added By Manny 12/23/2010
                                    , Jsd_Overtime
                                 FROM T_JobSplitHeader
                                 LEFT JOIN T_JobSplitdetail
                                   ON Jsd_ControlNo = Jsh_ControlNo
                                 LEFT JOIN T_Salesmaster 
                                   ON Slm_DashJobCode = Jsd_JobCode
							       AND Slm_ClientJobNo = Jsd_ClientJobNo
							       AND (Slm_Costcenter = Jsd_CostCenter OR Slm_Costcenter = 'ALL')
                                WHERE Jsh_ControlNo = '{0}' ";
            ds = dal.ExecuteDataSet(string.Format(sql, controlNo), CommandType.Text);

            return ds;
        }

        public DataSet LoadCurrentJobSplitDetails(string controlNo)
        {
            DataSet ds = new DataSet();
            string sql = @"
                               SELECT Jsh_ControlNo
                                    , Jsh_Status
                                    , Jsd_Seqno
                                    , Jsd_StartTime
                                    , Jsd_EndTime
                                    , Jsd_PlanHours
                                    , Jsd_JobCode
                                    , Jsd_ClientJobNo
                                    , Slm_ClientJobName
                                    , Jsd_SubWorkCode
                                    , Slm_Category
								    , Jsd_Costcenter
								    , Jsd_Category --Added By Manny 12/23/2010
                                    , Jsd_Overtime
                                 FROM T_JobSplitHeader
                                 LEFT JOIN T_JobSplitdetail
                                   ON Jsd_ControlNo = Jsh_ControlNo
                                 LEFT JOIN T_Salesmaster
                                   ON Slm_DashJobCode = Jsd_JobCode
							       AND Slm_ClientJobNo = Jsd_ClientJobNo
                                   AND (Slm_Costcenter = Jsd_CostCenter OR Slm_Costcenter = 'ALL')
                                WHERE Jsh_ControlNo = '{0}' ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, controlNo), CommandType.Text);
                }
                catch (Exception ex)
                {
                    //Do nothing
                }
                finally 
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public DataRow GetJobSplitHeaderInfo(string controlNo)
        {
            DataSet ds = new DataSet();

            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Jsh_ControlNo", controlNo);
            #endregion

            #region SQL Query
            string sqlQuery = @"SELECT
                                  Jsh_ControlNo
                                , Jsh_EmployeeId
                                , convert(varchar(20), Jsh_JobSplitDate, 101) AS Jsh_JobSplitDate
                                , Jsh_AppliedDate
                                , Jsh_Entry
                                , Jsh_Costcenter
                                , Jsh_EndorsedDateToChecker
                                , Jsh_CheckedBy
                                , Jsh_CheckedDate
                                , Jsh_Checked2By
                                , Jsh_Checked2Date
                                , Jsh_ApprovedBy
                                , Jsh_ApprovedDate
                                , Jsh_RefControlNo
                                , Jsh_Status
                                , JSH.Usr_Login
                                , JSH.Ludatetime      
                                , Trm_Remarks
                                FROM T_JobSplitHeader JSH
                                LEFT JOIN T_TransactionRemarks 
									ON Jsh_ControlNo = Trm_ControlNo
                                WHERE Jsh_ControlNo = @Jsh_ControlNo";
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds.Tables[0].Rows[0];
        }

        public bool IsEditable(string controlNo)
        {
            bool editable = true;

            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Jsh_ControlNo", controlNo);
            #endregion

            #region SQL Query
            string sqlQuery = @"SELECT 1
                                FROM T_JobSplitHeader 
                                JOIN T_PayPeriodMaster PPM
		                                ON  (Ppm_StartCycle <= CONVERT(DATE,Jsh_JobSplitDate,101) AND Ppm_EndCycle >= CONVERT(DATE,Jsh_JobSplitDate,101)
			                                AND Ppm_StartCycle <= CONVERT(DATE,GETDATE(),101) AND Ppm_EndCycle >= CONVERT(DATE,GETDATE(),101))
		                                OR (DATEDIFF(d,Ppm_EndCycle,GETDATE()) <= 3 AND DATEDIFF(d,Ppm_EndCycle,GETDATE()) > 0 
			                                AND Ppm_StartCycle <= CONVERT(DATE,Jsh_JobSplitDate,101) AND Ppm_EndCycle >= CONVERT(DATE,Jsh_JobSplitDate,101)
			                                )
                                    WHERE Jsh_ControlNo = @Jsh_ControlNo";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    editable = Convert.ToBoolean(dal.ExecuteScalar(sqlQuery, CommandType.Text, paramInfo));
                }
                catch {
                    
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return editable;
        }

        public DataSet GetUnsplittedHours(string EmployeeID)
        {
            DataSet ds = new DataSet();

            #region Parameters
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Jsh_EmployeeID", EmployeeID);
            #endregion

            #region SQL Query
            string sqlQuery = @"SELECT Convert(varchar(20), jsh_jobsplitdate,101), ISNULL(sum(jsd_acthours),0)
                                  FROM T_JobSplitDetailLosstime
                                 INNER JOIN T_JobsplitHeaderLossTIme 
                                    ON Jsh_ControlNo = Jsd_ControlNo
                                   AND Jsh_EmployeeId = @Jsh_EmployeeID
                                   AND Jsh_Status = '9'
                                   AND Jsh_JobSplitDate =(SELECT MAX(Jsh_JobSplitDate) AS JobSplitDate
                                                            FROM T_JobSplitDetailLosstime
                                                           INNER Join T_JobSplitHeaderLossTime
                                                              ON Jsh_ControlNo = Jsd_ControlNo
                                                             AND Jsh_JobsplitDate < getdate()
                                                             AND Jsh_EmployeeId = @Jsh_EmployeeID
                                                             AND Jsh_Status = '9') 
                                 GROUP BY Jsh_JobSplitDate";
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
                    //MessageBox.Show(ex.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;
        }

        public bool CheckIfBothBillingCycleLock(string Date)
        {
            DataSet ds = new DataSet();
            bool isDuplicate;

            string sqlQuery = @"SELECT Bcn_BillingYearMonth
                                FROM T_BillingConfiguration
                                WHERE Bcn_Lock = 0
                                  AND '" + Date + @"' BETWEEN Bcn_StartCycle AND Bcn_EndCycle
                                  AND Bcn_status = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                isDuplicate = true;
            else
                isDuplicate = false;

            return isDuplicate;
        }

        public bool CheckPayPeriodLock(string employeeId, string Date)
        {
            DataSet ds = new DataSet();
            bool isLocked = false;

            string sqlQuery = string.Format(@"
                DECLARE @EXTDAYS as varchar(10)
                SET @EXTDAYS = (SELECT Convert(int,Pmt_NumericValue)
				                  FROM T_ParameterMaster
				                 WHERE Pmt_ParameterID LIKE 'JOBEXTDAYS')
				
                exec('
                DECLARE @SPLITDATE as varchar(10)
                DECLARE @CURRENTDATE as varchar(10)
                DECLARE @MAXDATE as varchar(10)
                DECLARE @EMPLOYEEID as varchar(15)

                SET @SPLITDATE = ''{0}''
                SET @CURRENTDATE = Convert(varchar(10),GETDATE(), 101) 
                SET @EMPLOYEEID = ''{1}''

                SET @MAXDATE = (
                SELECT TOP 1 Convert(varchar(10),Ell_ProcessDate, 101)
                  FROM (SELECT TOP '+ @EXTDAYS +' Ell_ProcessDate
		                  FROM (SELECT * FROM T_EmployeeLogLedger UNION SELECT * FROM T_EmployeeLogLedgerHist) ELL
		                  LEFT JOIN T_PayPeriodMaster
			                ON Convert(datetime, @SPLITDATE) BETWEEN Ppm_StartCycle AND Ppm_EndCycle
		                 WHERE Ell_ProcessDate > Ppm_EndCycle
		                   AND Ell_EmployeeId = @EMPLOYEEID
		                   AND Ell_DayCode LIKE ''REG%'') as TEMP 
                 ORDER BY 1 DESC )
 
                IF EXISTS(
	                SELECT * FROM T_PayPeriodMaster 
	                Where Convert(datetime, @SPLITDATE) BETWEEN Ppm_StartCycle AND Ppm_EndCycle 
		                AND GETDATE() BETWEEN Ppm_StartCycle AND Ppm_EndCycle)
	                SELECT ''FALSE''
                ELSE
                SELECT CASE WHEN (Convert(datetime,@CURRENTDATE) > ISNULL(Convert(datetime,@MAXDATE), Convert(datetime,@CURRENTDATE) - 1))
			                THEN ''TRUE''
			                ELSE ''FALSE''
	                    END [LOCKED]')", Date, employeeId);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();

                    isLocked = Convert.ToBoolean(dal.ExecuteScalar(sqlQuery, CommandType.Text));
                }
                catch { }
                finally
                {
                    dal.CloseDB();
                }
            }

            return isLocked;
        }

        public bool CheckBillingLock(string date)
        {
            bool isLocked = false;

            string query = string.Format("SELECT Bcn_Lock FROM {0}..E_BillingConfiguration WHERE '{1}' BETWEEN Bcn_StartCycle and Bcn_EndCycle ", 
                                            ConfigurationManager.AppSettings["ERP_DB"], date);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    isLocked = Convert.ToBoolean(dal.ExecuteScalar(query, CommandType.Text));
                }
                catch { }
                finally
                {
                    dal.CloseDB();
                }
            }

            return isLocked;
        }

        public String GetStartTime(string timeIn1, string timeIn2, string shift, decimal otFRAC)
        {
            string startTime = string.Empty;
            int iTimeIn = 0;
            int iShiftIn = 0;
            int iBreakStart = 0;
            int iBreakEnd = 0;

            if (!timeIn1.Equals(string.Empty))
                iTimeIn = Convert.ToInt32(timeIn1.Remove(2, 1));
            else if (!timeIn2.Equals(string.Empty))
                iTimeIn = Convert.ToInt32(timeIn2.Remove(2, 1));

            if (!shift.Equals(string.Empty))
                iShiftIn = Convert.ToInt32(shift.Substring(8, 5).Remove(2, 1));

            if (!timeIn1.Equals(string.Empty))
                startTime = iTimeIn > iShiftIn || shift.Equals("") ? timeIn1 : shift.Substring(8, 5);
            else if (!timeIn2.Equals(string.Empty))
            {
                iBreakStart = Convert.ToInt32(shift.Substring(16, 5).Remove(2, 1));
                iBreakEnd = Convert.ToInt32(shift.Substring(23, 5).Remove(2, 1));
                if (iTimeIn >= iBreakStart && iTimeIn <= iBreakEnd)
                {
                    startTime = shift.Substring(23, 5);
                }
                else
                {
                    startTime = iTimeIn > iShiftIn || shift.Equals("") ? timeIn2 : shift.Substring(8, 5);
                }
            }

            if (startTime != "")
            {
                if (Convert.ToInt32(startTime.Substring(3, 2)) % otFRAC != 0)
                {
                    int minsToAdd = (int)otFRAC - Convert.ToInt32(startTime.Substring(3, 2)) % (int)otFRAC;
                    int mins = Convert.ToInt32(startTime.Substring(3, 2)) + minsToAdd;
                    if (mins == 60)
                        startTime = (Convert.ToInt32(startTime.Substring(0, 2)) + 1).ToString().PadLeft(2, '0') + ":00";
                    else
                        startTime = startTime.Substring(0, 3) + mins;
                }
            }

            return startTime;
        }

        public DataSet GetRecentJobByEmployee(string EmployeeID)
        {
            string sqlRecentJob = @"declare @Days AS decimal
                                        SET @Days = (SELECT Pmt_NumericValue 
                                                       FROM T_ParameterMaster
                                                      WHERE Pmt_ParameterId = 'DAYVWJOB')
                                SELECT TOP 1 Jsd_JobCode
                                    , Jsd_ClientJobNo
                                    , Jsd_CostCenter
                                    , Slm_ClientJobName
                                    , LTRIM(RTRIM(Jsd_SubWorkCode))  AS Jsd_SubWorkCode
                                    FROM T_JobSplitDetail
                                    LEFT JOIN T_SalesMaster 
                                        ON Slm_DashJobCode = Jsd_JobCode
                                        AND Slm_ClientJobNo = Jsd_ClientJobNo
                                        AND (Slm_Costcenter = Jsd_CostCenter OR Slm_Costcenter = 'ALL')
                                    INNER JOIN T_JobSplitHeader
                                        ON Jsh_ControlNo = Jsd_ControlNo 
                                        AND Jsh_EmployeeId = '{0}'
                                        AND Jsh_Status = '9'
                                        AND Jsh_JobSplitDate BETWEEN dateadd(dd, -@Days,Getdate()) AND Getdate()
                                    WHERE Jsd_Status IN ('9','F')
                                    ORDER BY Jsh_JobSplitDate DESC, Jsd_Seqno DESC";
            
            DataSet dsRecentJob;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRecentJob = dal.ExecuteDataSet(string.Format(sqlRecentJob, EmployeeID, CommandType.Text));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    dsRecentJob = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsRecentJob;
        }

        public void InsertUpdateRemarks(DataRow rowDetails, DALHelper dal)
        {
            #region Parameters
            ParameterInfo[] paramDetails = new ParameterInfo[3];
            paramDetails[0] = new ParameterInfo("@Trm_ControlNo", rowDetails["Trm_ControlNo"]);
            paramDetails[1] = new ParameterInfo("@Trm_Remarks", !rowDetails["Trm_Remarks"].ToString().Equals(string.Empty) ? rowDetails["Trm_Remarks"].ToString() : string.Empty);
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
    }
}