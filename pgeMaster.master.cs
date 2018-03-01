/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   03/22/2013
 *  Update Notes    :   
 *      -   Updated TMS Login Shortcut
 */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Resources;
using Payroll.DAL;
using CommonLibrary;

public partial class pgeMaster : System.Web.UI.MasterPage
{
    MenuGrant userGrant = new MenuGrant();
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (CommonMethods.IsUplevel)
        {
            Page.ClientTarget = "uplevel";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Response.Cache.SetAllowResponseInBrowserHistory(true);
        if (!Page.IsPostBack)
        {
            hfServerTime.Value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            lblCopyright.Text = string.Format("Copyright {0}. NPAX Cebu Corporation&nbsp;&nbsp;&nbsp;", DateTime.Now.Year.ToString());
            this.SetMenuAccessRights();
            this.lblUserLogged.Text = getUserLoggedInfo();
            this.lblLastLogin.Text = getLastLogIn();
            Session["transaction"] = string.Empty;
            if (Convert.ToBoolean(Resources.Resource.TMSSYSFLAG))
            {
                this.hlTMS.Visible = true;
                
                this.hlTMS.NavigateUrl = Resources.Resource.TMSURL + "initializeUser.aspx?user=" + HttpUtility.UrlEncode(Encrypt.encryptText(Session["userLogged"].ToString()));
            }
            else
            {
                this.hlTMS.Visible = false;
            }
            if (Convert.ToBoolean(Resources.Resource.TRAININGFLAG))
            {
                //string userid = NXpert.SSO.Library.CommonFunction.ConvertStringToHex(NXpert.SSO.Library.Encrypt.encryptText(Session["userId"].ToString()), System.Text.Encoding.Unicode);

                //string profile = NXpert.SSO.Library.CommonFunction.ConvertStringToHex(NXpert.SSO.Library.Encrypt.encryptText( Encrypt.decryptText(Session["dbConn"].ToString()).ToString()), System.Text.Encoding.Unicode);
                //this.hlTraining.Visible = true;

                //this.hlTraining.NavigateUrl = Resources.Resource.TRAININGURL + "index.aspx?nxi=" + userid + "&nxf=" + profile;
            }
            else
            {
                this.hlTMS.Visible = false;
            }
        }
    }
    private string getLastLogIn()
    {
        string lastlog = "";
        string query = string.Format(@"IF(EXISTS(SELECT TABLE_NAME 
                                                         FROM INFORMATION_SCHEMA.TABLES 
                                                         WHERE  TABLE_NAME = 'T_PasswordLockoutSettings'))
				                                         BEGIN
				                                         SELECT TOP 1 
							                                        UPPER(CAST(DATENAME(MONTH,Pls_LoginDate) AS VARCHAR(12))) 
							                                        + ' ' + CAST(DATEPART(DAY,Pls_LoginDate) AS VARCHAR(2)) 
							                                        + ','+ CAST(YEAR(Pls_LoginDate) AS VARCHAR(4)) 
							                                        +' '+CAST(CAST(Pls_LoginDate as time)AS VARCHAR(8))
                                                                                    FROM(
                                                                                    SELECT TOP 2 [Pls_LoginDate]
                                                                                    FROM [T_PasswordLockoutSettings]
                                                                                    WHERE Pls_UserCode = '{0}'
                                                                                    AND Pls_Status = 'S'
                                                                                    AND (SELECT COUNT(*) FROM [T_PasswordLockoutSettings]
                                                                                    WHERE Pls_UserCode = '{0}'
                                                                                    AND Pls_Status = 'S') > 1
                                                                                    ORDER BY Pls_LoginDate desc) TEMP
                                                                                    ORDER BY Pls_LoginDate asc
				                                        END
                                        ELSE SELECT 'xxx'", Session["userLogged"].ToString());
        using (DALHelper dal = new DALHelper(false))
        {
            try
            {
                dal.OpenDB();
                lastlog = dal.ExecuteScalar(query).ToString();
                dal.CloseDB();
                if (lastlog.Trim() != null && lastlog.Trim() != "" && lastlog.Trim() != "xxx")
                    lastlog = "Last Logged in: " + lastlog;
                else
                    lastlog = "No login record";
            }
            catch
            {
                lastlog = "No login record";
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return lastlog;
    }
    private string getUserLoggedInfo()
    {
        string sql = string.Format(@"SELECT dbo.GetControlEmployeeNameV2('{0}')", Session["userLogged"].ToString());
        string iName = string.Empty;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                iName = dal.ExecuteScalar(sql, CommandType.Text).ToString();
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
        return iName;
    }
    private bool isConnected()
    {
        bool isConnected = true;
        string test = string.Empty;
        try
        {
            test = Session["userLogged"].ToString();
        }
        catch
        {
            isConnected = false;
        }
        isConnected = isConnected && !test.Equals(string.Empty);
        isConnected = isConnected && !checkSqlConnection();

        return isConnected;
    }

    protected bool checkSqlConnection()
    {
        string sql = @"SELECT getdate()";
        string temp = string.Empty;
        using (DALHelper dal = new DALHelper(Session["dbConn"].ToString()))
        {
            try
            {
                dal.OpenDB();
                temp = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {
                temp = string.Empty;
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return temp.Equals(string.Empty);
    }
    #region [Menu Access Rights]
    //Andre revised: old method takes longer time because of mutiple queries in database per menucode.
    //private void SetMenuAccessRights()
    //{
    //    string x = DateTime.Now.Millisecond.ToString();

    //    int itemCount = menuMain.Items.Count;
    //    string sysmenucode = string.Empty;

    //    for (int counter = 1; counter < menuMain.Items.Count - 1; counter++)
    //    {
    //        for (int childitem1 = 0; childitem1 < menuMain.Items[counter].ChildItems.Count; )
    //        {
    //            for (int childitem2 = 0; childitem2 < menuMain.Items[counter].ChildItems[childitem1].ChildItems.Count; )
    //            {
    //                sysmenucode = menuMain.Items[counter].ChildItems[childitem1].ChildItems[childitem2].Value;
    //                if (!userGrant.getAccessRights(Session["userLogged"].ToString(), sysmenucode))
    //                    menuMain.Items[counter].ChildItems[childitem1].ChildItems.RemoveAt(childitem2);
    //                else
    //                    childitem2++;
    //            }
    //            if (menuMain.Items[counter].ChildItems[childitem1].ChildItems.Count == 0)
    //            {
    //                menuMain.Items[counter].ChildItems.RemoveAt(childitem1);
    //            }
    //            else
    //            {
    //                childitem1++;
    //            }
    //        }
    //    }
    //    for (int counter = 0; counter < menuMain.Items[3].ChildItems.Count; )
    //    {
    //        sysmenucode = menuMain.Items[3].ChildItems[counter].Value;
    //        if (!userGrant.getAccessRights(Session["userLogged"].ToString(), sysmenucode))
    //            menuMain.Items[3].ChildItems.RemoveAt(counter);
    //        else
    //            counter++;
    //    }

    //    MessageBox.Show(x + " ---- " + DateTime.Now.Millisecond.ToString());
    //}

    private void SetMenuAccessRights()
    {
        int itemCount = menuMain.Items.Count;
        string sysmenucode = string.Empty;
        string truncateSysMenuCode = string.Empty;
        string sqlGetAccess = @"SELECT Ugt_SysMenuCode
                                  FROM T_UserGrant
                                  LEFT JOIN T_UserGroupDetail 
                                    ON Ugd_UserCode = '{0}'
                                 WHERE Ugt_systemid  = Ugd_SystemId
                                   AND Ugt_usergroup = Ugd_UserGroupCode
                                   AND Ugt_CanRetrieve = 1
                                   AND Ugt_status    = 'A'
                                   AND Ugt_SysMenuCode LIKE 'WF%'";
        if (Session["trunMA"].ToString().Equals(string.Empty))
        {
            DataSet dsRights = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsRights = dal.ExecuteDataSet(string.Format(sqlGetAccess, Session["userLogged"].ToString()), CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (!CommonMethods.isEmpty(dsRights))
            {
                for (int i = 0; i < dsRights.Tables[0].Rows.Count; i++)
                {
                    truncateSysMenuCode += dsRights.Tables[0].Rows[i][0].ToString().ToUpper() + ",";
                }
            }
            Session["trunMA"] = Encrypt.encryptText(truncateSysMenuCode);
        }
        else
        {
            truncateSysMenuCode = Encrypt.decryptText(Session["trunMA"].ToString());
        }

        for (int counter = 1; counter < menuMain.Items.Count - 1; counter++)
        {
            for (int childitem1 = 0; childitem1 < menuMain.Items[counter].ChildItems.Count; )
            {
                for (int childitem2 = 0; childitem2 < menuMain.Items[counter].ChildItems[childitem1].ChildItems.Count; )
                {
                    sysmenucode = menuMain.Items[counter].ChildItems[childitem1].ChildItems[childitem2].Value;
                    if (!truncateSysMenuCode.Contains(sysmenucode))
                        menuMain.Items[counter].ChildItems[childitem1].ChildItems.RemoveAt(childitem2);
                    else
                        childitem2++;
                }
                if (menuMain.Items[counter].ChildItems[childitem1].ChildItems.Count == 0)
                {
                    menuMain.Items[counter].ChildItems.RemoveAt(childitem1);
                }
                else
                {
                    childitem1++;
                }
            }
        }
        for (int counter = 0; counter < menuMain.Items[3].ChildItems.Count; )
        {
            sysmenucode = menuMain.Items[3].ChildItems[counter].Value;
            if (!truncateSysMenuCode.Contains(sysmenucode))
                menuMain.Items[3].ChildItems.RemoveAt(counter);
            else
                counter++;
        }
    }
    #endregion
}
