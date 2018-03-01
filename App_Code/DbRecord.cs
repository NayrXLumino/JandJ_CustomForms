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
using System.Web.SessionState;
using CommonLibrary;


/// <summary>
/// Summary description for DbRecord
/// </summary>
namespace Payroll.DAL
{
	public class DbRecord
    {
        GeneralBL GNBL = new GeneralBL();
        public static DataRow Generate(string tableName)
        {
            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.NewRow();
        }

        public static DataTable GenerateTable(string tableName)
        {

            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(tableName);

                dbUtil.CloseDB();
            }

            return dt.Clone();
        }

        public static DataTable GenerateCustomTable(string strqry)
        {

            DataTable dt;

            using (DALHelper dbUtil = new DALHelper())
            {
                dbUtil.OpenDB();

                dt = dbUtil.GetTableStructure(strqry, false);

                dbUtil.CloseDB();
            }

            return dt;
        }

        public static DataRow Generate(string tableName, bool isFromProfileDB)
        {
            DataTable dt;
            
            
            if (isFromProfileDB)
            {
                string profileServer = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
                string profileDB = Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString());
                using (DALHelper dal = new DALHelper(profileServer, profileDB))
                {
                    dal.OpenDB();

                    dt = dal.GetTableStructure(tableName);

                    dal.CloseDB();
                }
            }
            else
            {
                using (DALHelper dbUtil = new DALHelper())
                {
                    dbUtil.OpenDB();

                    dt = dbUtil.GetTableStructure(tableName);

                    dbUtil.CloseDB();
                }
            }
            return dt.NewRow();
        }
	}
}
