using System;
using System.Collections.Generic;
using System.Text;
//using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.DAL
{
    public class SystemMenuLogBL 
    {
        //public override int Add(System.Data.DataRow row)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public override int Update(System.Data.DataRow row)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public override int Delete(string code, string userLogin)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public override System.Data.DataSet FetchAll()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public override System.Data.DataRow Fetch(string code)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        #region View
        public static int InsertViewLog(string menuCode, bool isSuccess, string employeeLogIn,bool unUsed)
        {
            return InsertViewLog(menuCode, isSuccess, employeeLogIn);
        }
        public static int InsertViewLog(string menuCode, bool isSuccess)
        {
            return InsertViewLog(menuCode, isSuccess, LoginInfo.getUser().UserCode);
        }
        public static int InsertViewLog(string menuCode, bool isSuccess, string userCode)
        {
            return InsertLog(menuCode, "V", isSuccess, userCode, "", true);
        } 
        #endregion

        #region Add
        public static int InsertAddLog(string menuCode, bool isSuccess)
        {
            return InsertAddLog(menuCode, isSuccess, "");
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID,string employeeLogIn,string WF)
        {
            return InsertAddLog(menuCode, isSuccess, employeeLogIn, employeeID, true);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID, string employeeLogIn, string WF,bool isPayrolDiff)
        {
            return InsertAddLog(menuCode, isSuccess, employeeLogIn, employeeID, isPayrolDiff);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID)
        {
            return InsertAddLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, true);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertAddLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string userCode, string employeeID)
        {
            return InsertAddLog(menuCode, isSuccess, userCode, employeeID, true);
        }
        public static int InsertAddLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertLog(menuCode, "A", isSuccess, userCode, employeeID, isPayrollDifferential);
        } 
        #endregion

        #region Edit
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID,string employeeLogIn,string WF)
        {
            return InsertEditLog(menuCode, isSuccess, employeeLogIn, employeeID, true);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID, string employeeLogIn, string WF,bool falseIsPayrollDiff)
        {
            return InsertEditLog(menuCode, isSuccess, employeeLogIn, employeeID, falseIsPayrollDiff);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess)
        {
            return InsertEditLog(menuCode, isSuccess, "");
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID)
        {
            return InsertEditLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, true);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertEditLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string userCode, string employeeID)
        {
            return InsertEditLog(menuCode, isSuccess, userCode, employeeID, true);
        }
        public static int InsertEditLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertLog(menuCode, "E", isSuccess, userCode, employeeID, isPayrollDifferential);
        } 
        #endregion

        #region Delete
        public static int InsertDeleteLog(string menuCode, bool isSuccess,string employeeID,string LoginID,string Empty)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginID, employeeID, true);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string employeeID, string LoginID, string Empty,bool IsPayrollDiff)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginID, employeeID, IsPayrollDiff);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess)
        {
            return InsertDeleteLog(menuCode, isSuccess, "");
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string employeeID)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, true);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string employeeID, bool isPayrollDifferential)
        {
            return InsertDeleteLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string userCode, string employeeID)
        {
            return InsertDeleteLog(menuCode, isSuccess, userCode, employeeID, true);
        }
        public static int InsertDeleteLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertLog(menuCode, "D", isSuccess, userCode, employeeID, isPayrollDifferential);
        } 
        #endregion

        #region Generate
        public static int InsertGenerateLog(string menuCode, string employeeID, bool isSuccess, string employeeLogIn)
        {
            return InsertGenerateLog(menuCode, isSuccess, employeeLogIn, employeeID, true);
        }
        public static int InsertGenerateLog(string menuCode, bool isSuccess)
        {
            return InsertGenerateLog(menuCode, "", isSuccess);
        }
        public static int InsertGenerateLog(string menuCode, string employeeID, bool isSuccess)
        {
            return InsertGenerateLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, true);
        }
        public static int InsertGenerateLog(string menuCode, string employeeID, bool isSuccess, bool isPayrollDifferential)
        {
            return InsertGenerateLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential);
        }
        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode)
        {
            return InsertGenerateLog(menuCode, isSuccess, userCode, "");
        }
        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode, string employeeID)
        {
            return InsertGenerateLog(menuCode, isSuccess, userCode, employeeID, true);
        }
        public static int InsertGenerateLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertLog(menuCode, "G", isSuccess, userCode, employeeID, isPayrollDifferential);
        }
        #endregion

        #region Print
        public static int InsertPrintLog(string menuCode, bool isSuccess)
        {
            return InsertPrintLog(menuCode, "", isSuccess);
        }
        public static int InsertPrintLog(string menuCode, string employeeID, bool isSuccess)
        {
            return InsertPrintLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, true);
        }
        public static int InsertPrintLog(string menuCode, string employeeID, bool isSuccess, bool isPayrollDifferential)
        {
            return InsertPrintLog(menuCode, isSuccess, LoginInfo.getUser().UserCode, employeeID, isPayrollDifferential);
        }
        public static int InsertPrintLog(string menuCode, bool isSuccess, string userCode)
        {
            return InsertPrintLog(menuCode, isSuccess, userCode, "");
        }
        public static int InsertPrintLog(string menuCode, bool isSuccess, string userCode, string employeeID)
        {
            return InsertPrintLog(menuCode, isSuccess, userCode, employeeID, true);
        }
        public static int InsertPrintLog(string menuCode, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            return InsertLog(menuCode, "P", isSuccess, userCode, employeeID, isPayrollDifferential);
        } 
        #endregion

        private static int InsertLog(string menuCode, string action, bool isSuccess, string userCode, string employeeID, bool isPayrollDifferential)
        {
            int value = 1;

            string query = string.Format(@"INSERT INTO T_SystemMenuLog
                        (Sml_MenuCode
                        ,Sml_Action
                        ,Sml_IsSuccess
                        ,Usr_Login
                        ,Sml_EmployeeID
                        ,Sml_CurrentPayPeriod
                        ,Ludatetime
                         )

                        VALUES(
                        '{0}'
                        ,'{1}'
                        ,'{2}'
                        ,'{3}'
                        ,'{4}'
                        ,(SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C')
                        ,GETDATE())
                        ", menuCode, action, isSuccess, userCode, employeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();
                try
                {
                    dal.ExecuteDataSet(query, CommandType.Text);
                    if (isSuccess && (action == "A" || action == "E" || action == "D") && isPayrollDifferential && (employeeID != "" && employeeID != "ALL"))
                        InsertEmployeeLogDifferential(employeeID, userCode);
                    dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransaction();
                    value = 0;
                }
                dal.CloseDB();
            }
            return value;
        }

        private static int InsertEmployeeLogDifferential(string employeeID, string userCode)
        {
            int value = 1;

            string query = string.Format(@"INSERT INTO T_EmployeePayrollDifferential
                        (Epd_EmployeeID
                        ,Usr_Login
                        ,Ludatetime
                         )

                        VALUES(
                        '{0}'
                        ,'{1}'
                        ,GETDATE())
                        ", employeeID == "" ? "" : employeeID, userCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();
                try
                {
                    dal.ExecuteDataSet(query, CommandType.Text);
                    dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransaction();
                    value = 0;
                }
                dal.CloseDB();
            }
            return value;
        }

        public static int InsertGroupDifferential(UniqueList<string> employeeID)
        {
            return InsertGroupDifferential(employeeID, LoginInfo.getUser().UserCode);
        }

        public static int InsertGroupDifferential(UniqueList<string> employeeIDList, string userLogin)
        {
            int value = 0;
            string query = "";
            foreach (string employeeID in employeeIDList)
            {
                query += string.Format(@"INSERT INTO T_EmployeePayrollDifferential
                                            (Epd_EmployeeID, Usr_Login, Ludatetime) 
                                            SELECT '{0}', '{1}', GETDATE()
                                         ", employeeID, userLogin);
            }
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransaction();
                try
                {
                    dal.ExecuteDataSet(query, CommandType.Text);
                    dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransaction();
                    value = 0;
                }
                dal.CloseDB();
            }
            return value;
        }

        public class UniqueList<T> : List<T>
        {
            public new void Add(T obj)
            {
                if (!Contains(obj))
                {
                    base.Add(obj);
                }
            }
        }
    }
}
