using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Payroll.DAL;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Menu
/// </summary>
namespace Payroll.DAL
{
    public class MenuGrant
    {
        private bool CanRetrieve;
        private bool CanAdd;
        private bool CanEdit;
        private bool CanDelete;
        private bool CanGenerate;
        private bool CanCheck;
        private bool CanApprove;
        private bool CanPrintPreview;
        private bool CanPrint;
        private bool CanReprint;

        public MenuGrant()
        {

        }

        public MenuGrant(string userId, string sysId, string sysMenu)
        {
            string userID = userId;
            DataSet dsRights = new DataSet();

            string sqlGetRights = @"    SELECT 
                                            Ugt_CanRetrieve,
                                            Ugt_CanAdd,
                                            Ugt_CanEdit,
                                            Ugt_CanDelete,
                                            Ugt_CanGenerate,
                                            Ugt_CanCheck,
                                            Ugt_CanApprove,
                                            Ugt_CanPrintPreview,
                                            Ugt_CanPrint,
                                            Ugt_CanReprint
                                        FROM T_UserGrant
                                        LEFT JOIN T_Usergroupdetail ON Ugd_usercode = '{0}'
                                         and ugd_systemid = '{1}'
                                        WHERE
                                         Ugt_SystemId    = '{1}'
                                         AND Ugt_SysmenuCode = '{2}'
                                         AND Ugt_usergroup = Ugd_usergroupcode";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRights = dal.ExecuteDataSet(string.Format(sqlGetRights, userID, sysId, sysMenu), CommandType.Text);
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

            if (!CommonMethods.isEmpty(dsRights))
            {
                this.CanRetrieve = Convert.ToBoolean(dsRights.Tables[0].Rows[0][0].ToString());
                this.CanAdd = Convert.ToBoolean(dsRights.Tables[0].Rows[0][1].ToString());
                this.CanEdit = Convert.ToBoolean(dsRights.Tables[0].Rows[0][2].ToString());
                this.CanDelete = Convert.ToBoolean(dsRights.Tables[0].Rows[0][3].ToString());
                this.CanGenerate = Convert.ToBoolean(dsRights.Tables[0].Rows[0][4].ToString());
                this.CanCheck = Convert.ToBoolean(dsRights.Tables[0].Rows[0][5].ToString());
                this.CanApprove = Convert.ToBoolean(dsRights.Tables[0].Rows[0][6].ToString());
                this.CanPrintPreview = Convert.ToBoolean(dsRights.Tables[0].Rows[0][7].ToString());
                this.CanPrint = Convert.ToBoolean(dsRights.Tables[0].Rows[0][8].ToString());
                this.CanReprint = Convert.ToBoolean(dsRights.Tables[0].Rows[0][9].ToString());
            }
            else
            {
                this.CanRetrieve = false;
                this.CanAdd = false;
                this.CanEdit = false;
                this.CanDelete = false;
                this.CanGenerate = false;
                this.CanCheck = false;
                this.CanApprove = false;
                this.CanPrintPreview = false;
                this.CanPrint = false;
                this.CanReprint = false;
            }

        }

        public bool getAccessRights(string userID, string sysMenuCode)
        {
            DataSet dsRights = new DataSet();

            #region sql query
            string sqlGetRights = @"select isnull(Ugt_CanRetrieve, 0)
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
                    dsRights = dal.ExecuteDataSet(string.Format(sqlGetRights, userID, sysMenuCode), CommandType.Text);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, "SA");
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

        public bool canRetrieve()
        {
            return CanRetrieve;
        }

        public bool canAdd()
        {
            return CanAdd;
        }

        public bool canEdit()
        {
            return CanEdit;
        }

        public bool canDelete()
        {
            return CanDelete;
        }

        public bool canGenerate()
        {
            return CanGenerate;
        }

        public bool canCheck()
        {
            return CanCheck;
        }

        public bool canApprove()
        {
            return CanApprove;
        }

        public bool canPrintPreview()
        {
            return CanPrintPreview;
        }

        public bool canPrint()
        {
            return CanPrint;
        }

        public bool canReprint()
        {
            return CanReprint;
        }
    }
}