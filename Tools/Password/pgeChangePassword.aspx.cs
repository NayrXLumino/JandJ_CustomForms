using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;

public partial class Tools_Password_pgeChangePassword : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    GeneralBL GNBL = new GeneralBL();
    MenuGrant MGBL = new MenuGrant();
    string defaultPassword = Resources.Resource.RESETPASSWORD;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFCHANGEPASS"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["pswd"]))
                {
                    if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
                    {
                        MessageBox.Show("Password does not meet requirements. Update password to avoid this prompt.\n" 
                            + GNBL.isValidPassword2(Encrypt.decryptText(Request.QueryString["pswd"])));
                    }
                    else
                    {
                        MessageBox.Show("Password does not meet requirements. Update password to avoid this prompt.\n" 
                            + GNBL.isValidPassword(Encrypt.decryptText(Request.QueryString["pswd"])));
                    }
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["exp"]))
                {
                    MessageBox.Show("Password is already expired. Please update your password to be able to use the system.");
                }
             
                initializeEmployee();
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
            }
            LoadComplete += new EventHandler(Tools_Password_pgeChangePassword_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Tools_Password_pgeChangePassword_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "passwordScripts";
        string jsurl = "_password.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        btnReset.OnClientClick = "return confirm('Continue reset password?');";
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        txtCurrentPassword.Text = string.Empty;
        txtNewPassword.Text = string.Empty;
        txtConfirmPassword.Text = string.Empty;
        initializeControls();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                string errMsg1 = checkEntry1();
                if (errMsg1.Equals(string.Empty))
                {
                    string errMsg2 = checkEntry2();
                    if (errMsg2.Equals(string.Empty))
                    {
                        if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
                        {
                            string[] ProfileDBConnection = GNBL.GetProfileConnections();
                            using (DALHelper dal = new DALHelper(ProfileDBConnection[0], ProfileDBConnection[1]))
                            {
                                try
                                {
                                    dal.OpenDB();
                                    dal.BeginTransaction();
                                    GNBL.InsertPasswordTrail2(txtEmployeeId.Text
                                                           , Session["userLogged"].ToString(), dal);
                                    GNBL.UpdateUserPassword2(txtEmployeeId.Text
                                                           , txtNewPassword.Text
                                                           , Session["userLogged"].ToString(), dal);
                                    dal.CommitTransaction();
                                    MessageBox.Show("Successfully updated " + txtEmployeeName.Text + "'s password");
                                    Session["expPas"] = false;
                                }
                                catch (Exception ex)
                                { 
                                    dal.RollBackTransaction();
                                    CommonMethods.ErrorsToTextFile(ex, "SAVE_PASSWORD1");
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
                                    dal.BeginTransaction();
                                    GNBL.UpdateUserPassword(txtEmployeeId.Text
                                                           , txtNewPassword.Text
                                                           , Session["userLogged"].ToString(), dal);
                                    //GNBL.InsertPasswordTrail(txtEmployeeId.Text
                                    //                       , Session["userLogged"].ToString(), dal);
                                    dal.CommitTransaction();
                                    MessageBox.Show("Successfully updated " + txtEmployeeName.Text + "'s password");
                                    Session["expPas"] = false;
                                }
                                catch (Exception ex)
                                {
                                    dal.RollBackTransaction();
                                    CommonMethods.ErrorsToTextFile(ex, "SAVE_PASSWORD2");
                                }
                                finally
                                {
                                    dal.CloseDB();
                                }
                            }
                        }

                        #region Synch password to all database
                        DataSet ds = new DataSet();
                        string sql = @"SELECT Prf_DataBaseNo
                                            , Prf_Profile
                                            , Prf_Server
                                            , Prf_DataBase
                                            , Prf_UserID
                                            , Prf_Password
                                            , Prf_DatabasePrefix
                                         FROM T_Profiles
                                        WHERE Prf_Status = 'A'
                                          AND Prf_DataBaseNo IN (SELECT Upt_DatabaseNo 
                                                                   FROM T_UserProfile 
                                                                  WHERE Upt_UserCode = '{0}' 
                                                                    AND Upt_Status = 'A')";
                        using (DALHelper dal = new DALHelper(false))
                        {
                            try
                            {
                                dal.OpenDB();
                                ds = dal.ExecuteDataSet(string.Format(sql, txtEmployeeId.Text), CommandType.Text);
                            }
                            catch (Exception ex)
                            {
                                CommonMethods.ErrorsToTextFile(ex, "SAVE_PASSWORD1");
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                        }

                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            string connetionStringFormat = @"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";
                            string temp = string.Empty;
                            for (int ctr = 0; ctr < ds.Tables[0].Rows.Count; ctr++)
                            {
                                using (DALHelper dal = new DALHelper(Encrypt.encryptText(string.Format(connetionStringFormat, ds.Tables[0].Rows[ctr]["Prf_Server"].ToString()
                                                                                                                            , ds.Tables[0].Rows[ctr]["Prf_DataBase"].ToString()
                                                                                                                            , ds.Tables[0].Rows[ctr]["Prf_UserID"].ToString()
                                                                                                                            , ds.Tables[0].Rows[ctr]["Prf_Password"].ToString()))))
                                {
                                    try
                                    {
                                        dal.OpenDB();
                                        dal.BeginTransactionSnapshot();
                                        GNBL.UpdateUserPassword(txtEmployeeId.Text
                                                               , txtNewPassword.Text
                                                               , Session["userLogged"].ToString(), dal);
                                        dal.CommitTransactionSnapshot();
                                    }
                                    catch (Exception ex)
                                    {
                                        dal.RollBackTransactionSnapshot();
                                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                                    }
                                    finally
                                    {
                                        dal.CloseDB();
                                    }          
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show(errMsg2);
                    }
                }
                else
                {
                    MessageBox.Show(errMsg1);
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
            {
                string[] ProfileDBConnection = GNBL.GetProfileConnections();
                using (DALHelper dal = new DALHelper(ProfileDBConnection[0], ProfileDBConnection[1]))
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransaction();
                        GNBL.InsertPasswordTrail2(txtEmployeeId.Text
                                               , Session["userLogged"].ToString(), dal);
                        defaultPassword = GNBL.GetDefaultPassword(dal);
                        defaultPassword = defaultPassword.Equals(string.Empty) ? Resources.Resource.RESETPASSWORD : defaultPassword;
                        GNBL.UpdateUserPassword2(txtEmployeeId.Text
                                               , defaultPassword
                                               , Session["userLogged"].ToString(), dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Successfully resetted " + txtEmployeeName.Text + "'s password");
                    }
                    catch (Exception ex)
                    {
                        dal.RollBackTransaction();
                        CommonMethods.ErrorsToTextFile(ex, "RESET_PASSWORD2");
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
                        dal.BeginTransaction();

                        //GNBL.InsertPasswordTrail(txtEmployeeId.Text
                        //                       , Session["userLogged"].ToString(), dal);

                        defaultPassword = GNBL.GetDefaultPassword(dal);
                        defaultPassword = defaultPassword.Equals(string.Empty) ? Resources.Resource.RESETPASSWORD : defaultPassword;
                        GNBL.UpdateUserPassword(txtEmployeeId.Text
                                               , defaultPassword
                                               , Session["userLogged"].ToString(), dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Successfully resetted " + txtEmployeeName.Text + "'s password");
                    }
                    catch (Exception ex)
                    {
                        dal.RollBackTransaction();
                        CommonMethods.ErrorsToTextFile(ex, "RESET_PASSWORD1");
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }

            DataSet ds = new DataSet();
            string sql = @"SELECT Prf_DataBaseNo
                                , Prf_Profile
                                , Prf_Server
                                , Prf_DataBase
                                , Prf_UserID
                                , Prf_Password
                                , Prf_DatabasePrefix
                             FROM T_Profiles
                            WHERE Prf_Status = 'A'
                              AND Prf_DataBaseNo IN (SELECT Upt_DatabaseNo 
                                                       FROM T_UserProfile 
                                                      WHERE Upt_UserCode = '{0}' 
                                                        AND Upt_Status = 'A')";
            using (DALHelper dal = new DALHelper(false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, txtEmployeeId.Text), CommandType.Text);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, "SYNCH_RESET");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string connetionStringFormat = @"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";
                string temp = string.Empty;
                for (int ctr = 0; ctr < ds.Tables[0].Rows.Count; ctr++)
                {
                    using (DALHelper dal = new DALHelper(Encrypt.encryptText(string.Format(connetionStringFormat, ds.Tables[0].Rows[ctr]["Prf_Server"].ToString()
                                                                                                                , ds.Tables[0].Rows[ctr]["Prf_DataBase"].ToString()
                                                                                                                , ds.Tables[0].Rows[ctr]["Prf_UserID"].ToString()
                                                                                                                , ds.Tables[0].Rows[ctr]["Prf_Password"].ToString()))))
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransactionSnapshot();
                            
                            GNBL.UpdateUserPassword(txtEmployeeId.Text
                                                   , defaultPassword
                                                   , Session["userLogged"].ToString(), dal);
                            dal.CommitTransactionSnapshot();
                        }
                        catch (Exception ex)
                        {
                            dal.RollBackTransactionSnapshot();
                            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
    }
    #endregion

    #region Methods
    private void initializeEmployee()
    {
        DataSet ds = new DataSet();
        string sql = @"  SELECT Emt_EmployeeId [ID No]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname]
                           FROM T_EmployeeMaster
                          WHERE Emt_EmployeeId = @EmployeeId";

        string sqlUserMaster = @"SELECT Umt_UserCode [ID No]
                                      , Umt_UserNickname [Nickname]
                                      , Umt_UserLname [Lastname]
                                      , Umt_UserFname [Firstname]
                                   FROM T_UserMaster
                                  WHERE Umt_UserCode = @EmployeeId";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString());
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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

        if (!CommonMethods.isEmpty(ds))
        {
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0]["Nickname"].ToString();
        }
        else
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlUserMaster, CommandType.Text, param);
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
            if (!CommonMethods.isEmpty(ds))
            {
                txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
                txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                     + ", "
                                     + ds.Tables[0].Rows[0]["Firstname"].ToString();
                txtNickname.Text = ds.Tables[0].Rows[0]["Nickname"].ToString();
            }
            else
            {
                txtEmployeeId.Text = string.Empty;
                txtEmployeeName.Text = string.Empty;
                txtNickname.Text = string.Empty;
            }
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFCHANGEPASS");
        btnEmployeeId.Enabled = userGrant.canAdd();
        btnReset.Visible = userGrant.canEdit();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        hfPrevEntry.Value = string.Empty;
        btnSave.Focus();
    }

    private string checkEntry1()//Do not query anything yet
    {
        string err = string.Empty;
        #region Current Password
        if (txtCurrentPassword.Text.Equals(string.Empty))
        {
            err += "\nEnter current password";
        }
        if (txtCurrentPassword.Text.Equals(txtNewPassword.Text))
        {
            err += "\nNew password cannot be the same as current password";
        }
        #endregion
        #region New Password
        if (txtNewPassword.Text.Equals(string.Empty))
        {
            err += "\nEnter new password";
        }
        #endregion
        #region Current Password
        if (txtConfirmPassword.Text.Equals(string.Empty))
        {
            err += "\nEnter confirm password";
        }
        #endregion
        #region
        if (!txtNewPassword.Text.Equals(txtConfirmPassword.Text))
        {
            err += "\nNew and Confirm Password does not match";
        }
        #endregion
        return err;
    }

    private string checkEntry2()//Validation from DB parameters/data
    {
        string err = string.Empty;
        if (!GNBL.isCorrectCurrentPass(txtCurrentPassword.Text, txtEmployeeId.Text))
        {
            err = " Current password is invalid.";
        }
        if (err.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
            {
                err = GNBL.isValidPassword2(txtNewPassword.Text);
            }
            else
            {
                err = GNBL.isValidPassword(txtNewPassword.Text);
            }
        }
        #region Added Robert 11142013
        if (err.Equals(string.Empty))
        {
            if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
            {
                if (GNBL.isUsedPrev(txtEmployeeId.Text, txtNewPassword.Text))
                {
                    err = "Cannot reuse password.";
                }
            }
        }
        if (err.Equals(string.Empty))
        {
            err = txtEmployeeId.Text.Trim() == txtNewPassword.Text.Trim() ? "User ID could not be used as a password" : "";
        } 
        #endregion

        return err;
    }
    #endregion
}
