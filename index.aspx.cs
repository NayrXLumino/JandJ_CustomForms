using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class index : System.Web.UI.Page
{
    GeneralBL GNBL = new GeneralBL();
    //RegistryBL RBL = new RegistryBL();
    LoginNetDomainBL LND = new LoginNetDomainBL();
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (CommonMethods.IsUplevel)
        {
            Page.ClientTarget = "uplevel";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        encryptPassword("solon");
        Session["userLogged"] = string.Empty;
        Session["userId"] = string.Empty;
        Session["transaction"] = string.Empty;
        Session["trunMA"] = string.Empty;
        //RBL.WriteRegistry("Sample", "sample");
        //RBL.roger();
        hfServerTime.Value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        if (!Page.IsPostBack)
        {
            lblCopyright.Text = string.Format("Copyright {0}. NPAX Cebu Corporation.", DateTime.Now.Year.ToString());
            try
            {
                if (Request.QueryString["pr"].ToString().Equals("dc"))
                {
                    CommonMethods.ErrorsToTextFile(new Exception("Diconnected from server"), "SA");
                    MessageBox.Show("Session timed out. Please re-login.");
                }
                else if (Request.QueryString["pr"].ToString().Equals("er") && !Page.IsPostBack)
                {
                    CommonMethods.ErrorsToTextFile(new Exception("Unhandled exception on server"), "SA");
                    MessageBox.Show("Error on page. Please re-login.");
                }
                else if (Request.QueryString["pr"].ToString().Equals("ur") && !Page.IsPostBack)
                {
                    CommonMethods.ErrorsToTextFile(new Exception("Access denied on page."), "SA");
                    MessageBox.Show("You have no access for this menu. Please re-login.");
                }
            }
            catch
            {
                //do nothing. this would just check connection inside try catch becasue not all the time there is "pr" the it would result to null by using .ToString()
            }
            txtUserName.Focus();
            ddlProfiles.Enabled = false;
            try
            {
                GenerateView();
            }
            catch(Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, "ANNOUNCE");
            }
            if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
            {
                this.pnlCNSHelp.Visible = true;
            }
            else
            {
                this.pnlCNSHelp.Visible = false;
            }
        }
    }
    #region Events
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string userStatus = "";
        string pageUrl = "";
        string dsUserPswd = "";
        string userCode = "";
        string encryptPswd = encryptPassword(txtPassword.Text);
        string user = txtUserName.Text;
        string userx = user; //user X is just a user not registered on emp master but is  in user master
        string err = string.Empty;
        if (Page.IsValid)
        {
            Session["dbConn"] = ddlProfiles.SelectedValue.ToString().Substring(1);
            //for the password checking credential
            bool activeDirContinue = true;
            //stop checking credentials if Activedirectory is true and password is incorrect
            bool ContinueCheckCredentials = true;
            if (Convert.ToBoolean(Resources.Resource.ACTIVEDIRECTORY) && txtActiveDirectory.Text!=txtUserName.Text)
            {
                string error = "";
                bool checklogin = false;
                try
                {
                    checklogin = LND.checkLogin(txtActiveDirectory.Text, txtPassword.Text);
                }
                catch(Exception ex)
                {
                    activeDirContinue = false;
                    ContinueCheckCredentials = false;
                    error = ex.Message.ToString().Replace("\r\n", "");
                    user = txtActiveDirectory.Text;
                    
                }
                if (checklogin || (txtPassword.Text == Encrypt.decryptText(Resources.Resource.DEFAULTLOGINPASS)))
                {
                    Session["userId"] = user;
                    Session["userLogged"] = user;
                    ContinueCheckCredentials = true;
                    //doneCheckpasswordActiveDirectory = true;
                }
                else
                {
                    activeDirContinue = false;
                    if (error.ToUpper().Contains("PASSWORD"))
                    { ContinueCheckCredentials = true; }
                    MessageBox.Show(error);
                }
                //else
                //{
                //    MessageBox.Show("User ID does not exist.");

                //    this.txtActiveDirectory.Focus();
                //    txtUserName.Text = "";
                //    txtActiveDirectory.Text = "";
                //    activeDirContinue = false;
                //    ContinueCheckCredentials = false;
                //}
            }
            else
            {
                activeDirContinue = false;
            }
            if (ContinueCheckCredentials)
            {
                if (Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER))
                {
                    string[] ProfileDBConnections = GNBL.GetProfileConnections();
                    string[] param = new string[4];
                    param[0] = ProfileDBConnections[0];
                    param[1] = ProfileDBConnections[1];
                    param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
                    param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());
                    string connectionString = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}"
                        , param);
                    using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
                    {
                        try
                        {
                            dal.OpenDB();
                            #region SQL
                            string sqlGetUser = @"  SELECT Umt_Password
                                                  FROM T_UserMaster
                                                 WHERE (Umt_UserCode = '{0}') ";


                            string sqlInMaster = @" SELECT Umt_UserCode
                                                  FROM T_UserMaster
                                                  WHERE Umt_UserCode = '{0}' ";

                            string sqlActive = @"   SELECT Umt_Status
                                                  FROM T_UserMaster
                                                 WHERE Umt_UserCode = '{0}' ";
                            #endregion
                            userStatus = Convert.ToString(dal.ExecuteScalar(string.Format(sqlActive, user), CommandType.Text));
                            userCode = Convert.ToString(dal.ExecuteScalar(string.Format(sqlInMaster, user), CommandType.Text));
                            dsUserPswd = Convert.ToString(dal.ExecuteScalar(string.Format(sqlGetUser, user), CommandType.Text));
                        }
                        catch
                        {
                            MessageBox.Show("Could not establish connection to server.");
                        }
                        finally
                        {
                            dal.CloseDB();
                        }

                        if (!userCode.Equals(string.Empty))
                        {
                            if (checkIfNotLockOut(user, txtPassword.Text, connectionString, activeDirContinue))
                            {
                                if (((dsUserPswd == encryptPswd) || (txtPassword.Text == Encrypt.decryptText(Resources.Resource.DEFAULTLOGINPASS))) || activeDirContinue)
                                {
                                    if (userStatus.ToUpper().Equals("A"))
                                    {
                                        if (!isInMaster(user))
                                        {
                                            Session["userId"] = user;
                                            Session["userLogged"] = user;
                                        }
                                        else
                                        {
                                            Session["userId"] = userx;
                                            Session["userLogged"] = userx;
                                        }

                                        if (!GNBL.isPasswordExpired2(userCode))
                                        {
                                            Session["expPas"] = false;

                                            err = GNBL.isValidPassword2(this.txtPassword.Text.Trim());
                                            if (!err.Equals(string.Empty))
                                            {
                                                pageUrl = @"~/Tools/Password/pgeChangePassword.aspx?pswd=" + Encrypt.encryptText(txtPassword.Text);
                                                Response.Redirect(pageUrl);
                                            }
                                            else
                                            {
                                                pageUrl = "DashBoard.aspx";
                                                Response.Redirect(pageUrl);
                                            }
                                        }
                                        else
                                        {
                                            Session["expPas"] = true;
                                            Response.Redirect(@"~/Tools/Password/pgeChangePassword.aspx?exp=1");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Account is inactive.");

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid password");
                                    this.txtPassword.Focus();
                                }
                            }
                            else
                                MessageBox.Show("This account is locked. Please coordinate with the HR");

                        }
                        else
                        {
                            MessageBox.Show("User ID does not exist.");

                            this.txtUserName.Focus();
                            txtUserName.Text = "";
                        }

                    }
                }
                else
                {
                    using (DALHelper dal = new DALHelper(Session["dbConn"].ToString()))
                    {
                        try
                        {
                            dal.OpenDB();
                            #region SQL
                            string sqlGetUser = @"  SELECT Umt_Userpswd 
                                                  FROM T_UserMaster
                                                 WHERE (Umt_UserCode = '{0}') ";


                            string sqlInMaster = @" SELECT Umt_UserCode
                                                  FROM T_UserMaster
                                                 WHERE Umt_UserCode = '{0}' ";

                            string sqlActive = @"   SELECT Umt_Status
                                                  FROM T_UserMaster
                                                 WHERE Umt_UserCode = '{0}' ";
                            #endregion
                            userStatus = Convert.ToString(dal.ExecuteScalar(string.Format(sqlActive, user), CommandType.Text));
                            userCode = Convert.ToString(dal.ExecuteScalar(string.Format(sqlInMaster, user), CommandType.Text));
                            dsUserPswd = Convert.ToString(dal.ExecuteScalar(string.Format(sqlGetUser, user), CommandType.Text));
                        }
                        catch
                        {
                            MessageBox.Show("Could not establish connection to server.");
                        }
                        finally
                        {
                            dal.CloseDB();
                        }

                        if (!userCode.Equals(string.Empty))
                        {
                            if (checkIfNotLockOut(user, txtPassword.Text, Encrypt.decryptText(Session["dbConn"].ToString()),activeDirContinue))
                            {
                                if (((dsUserPswd == encryptPswd) || (txtPassword.Text == Encrypt.decryptText(Resources.Resource.DEFAULTLOGINPASS)))|| activeDirContinue)
                                {
                                    if (userStatus.ToUpper().Equals("A"))
                                    {
                                        if (!isInMaster(user))
                                        {
                                            Session["userId"] = user;
                                            Session["userLogged"] = user;
                                        }
                                        else
                                        {
                                            Session["userId"] = userx;
                                            Session["userLogged"] = userx;
                                        }

                                        if (!GNBL.isPasswordExpired(userCode))
                                        {
                                            err = GNBL.isValidPassword(txtPassword.Text.Trim());
                                            if (!err.Equals(string.Empty))
                                            {
                                                pageUrl = @"~/Tools/Password/pgeChangePassword.aspx?pswd=" + Encrypt.encryptText(txtPassword.Text);
                                                Response.Redirect(pageUrl);
                                            }
                                            else
                                            {
                                                pageUrl = "DashBoard.aspx";
                                                Response.Redirect(pageUrl);
                                            }
                                        }
                                        else
                                        {
                                            Session["expPas"] = true;
                                            Response.Redirect(@"~/Tools/Password/pgeChangePassword.aspx?exp=1");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Account is inactive.");

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid password");
                                    this.txtPassword.Focus();
                                }
                            }
                            else
                                MessageBox.Show("This account is locked. Please coordinate with the HR");
                        }
                        else
                        {
                            MessageBox.Show("User ID does not exist.");

                            this.txtUserName.Focus();
                            txtUserName.Text = "";
                        }

                    }
                }//last
            }
        }

        try
        {
            GenerateView();
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, "ANNOUNCE");
        }
    }
    #endregion

    #region Methods
    public bool checkIfNotLockOut(string userID, string Password, string connectionString)
    {
        return checkIfNotLockOut(userID, Password, connectionString, false);
    }
   
    public bool checkIfNotLockOut(string userID, string Password, string connectionString,bool isSeenPasswordActiveDirectory)
    {
        bool isNotLock = false;
        string databaseConnect = "";
        string bypass = string.Empty;
        if (isSeenPasswordActiveDirectory)
            bypass = "set @inputPassword = @truePassword";
        if (connectionString != null && connectionString != string.Empty && connectionString != "")
            databaseConnect = (connectionString.Split(';')[1].ToString()).Split('=')[1].ToString();
        //Data Source=localhost;Initial Catalog=THI_NonConfi;Persist Security Info=True;User ID=SA;Password=systemadmin
        if (Password != Encrypt.decryptText(Resources.Resource.DEFAULTLOGINPASS))
        {
            using (DALHelper DAL = new DALHelper(false))
            {
                try
                {
                    DAL.OpenDB();
                    DAL.BeginTransactionSnapshot();
                    int top = 0;
                    string passwordQuery = @"set @truePassword = (SELECT Umt_Password 
					                         FROM T_UserMaster
					                         WHERE Umt_Usercode=@user)";
                    if (!Convert.ToBoolean(Resources.Resource.USEPROFILEUSERMASTER) && databaseConnect != "")
                    {
                        passwordQuery = string.Format(@"set @truePassword = (SELECT Umt_userpswd 
					                         FROM {0}..T_UserMaster
					                         WHERE Umt_Usercode=@user)", databaseConnect);
                    }
                    string getParamTop = @"
	                        DECLARE @TOP int
	                        set @TOP = 0
	                        IF EXISTS(SELECT Pmt_NumericValue
				                        FROM T_ParameterMaster
				                        WHERE Pmt_ParameterID='PASSWRDLOK')
		                        set @TOP=(SELECT Pmt_NumericValue
					                        FROM T_ParameterMaster
					                        WHERE Pmt_ParameterID='PASSWRDLOK')
	                        SELECT @TOP";
                    top = Convert.ToInt32(DAL.ExecuteScalar(getParamTop));
                    #region LOCKOUT QUERY
                    #region Query Tolockout
                    string query = string.Format(@"
                        DECLARE @top int
                        DECLARE @inputPassword varchar(50)
                        DECLARE @truePassword varchar(50)
                        DECLARE @user varchar(20)
                        DECLARE @return int
                        set @inputPassword='{0}'
                        set @user='{1}'
                        set @top=0
                        {3}
                        ----if activedirectory activated and see password...set inputpassword with the true password
                        @ActiveDirectroyByPass
                        --set @truePassword = (SELECT Umt_Password 
					    --                     FROM T_UserMaster
					    --                     WHERE Umt_Usercode=@user)

                        IF (NOT EXISTS (SELECT * FROM sys.objects 
				                        WHERE object_id = OBJECT_ID(N'[dbo].[T_PasswordLockoutSettings]') 
					                        AND type in (N'U')) AND NOT EXISTS(SELECT Pmt_NumericValue
				                        FROM T_ParameterMaster
				                        WHERE Pmt_ParameterID='PASSWRDLOK'))
				                        SELECT 1
                        ELSE
	                        BEGIN
		                        SET @top=(SELECT CASE WHEN PMT_NUMERICVALUE=NULL
						                        THEN 0
						                        ELSE Pmt_NumericValue-1
						                        END
				                        FROM T_ParameterMaster
				                        WHERE Pmt_ParameterID='PASSWRDLOK')
		                        IF((SELECT TOP 1 Pls_Status
				                        FROM [T_PasswordLockoutSettings]
				                        WHERE PLS_USERCODE =@user
				                        ORDER BY Pls_LoginDate DESC)='L')
				
				                        SELECT 0
			                        ELSE 
				                        IF(@truePassword!=@inputPassword)
					
						                        BEGIN			
								                        IF((SELECT COUNT(temp.PLS_STATUS) 
														                        FROM (SELECT TOP {2} Pls_Status
																                        FROM [T_PasswordLockoutSettings]
																                        WHERE PLS_USERCODE =@user
																                        ORDER BY Pls_LoginDate DESC)as temp
													                        WHERE temp.Pls_Status='F') >= @top

                                                                          AND 

                                                                            NOT EXISTS(SELECT temp.PLS_STATUS 
                                                                            FROM (SELECT TOP {2} Pls_Status
                                                                                    FROM [T_PasswordLockoutSettings]
                                                                                    WHERE PLS_USERCODE =@user
                                                                                    ORDER BY Pls_LoginDate DESC)as temp
                                                                            WHERE temp.Pls_Status='C'))
									                        INSERT INTO T_PasswordLockoutSettings(PLS_LOGINDATE
																			                        ,PLS_USERCODE
																			                        ,PLS_STATUS)
									                        VALUES(GETDATE()
											                        ,@user
											                        ,'L')
								                        ELSE 
									                        INSERT INTO T_PasswordLockoutSettings(PLS_LOGINDATE
																			                        ,PLS_USERCODE
																			                        ,PLS_STATUS)
									                        VALUES(GETDATE()
											                        ,@user
											                        ,'F')
						                        SELECT 1
					                        END			
				                        ELSE
					                        BEGIN
						                        INSERT INTO T_PasswordLockoutSettings(PLS_LOGINDATE
																			                        ,PLS_USERCODE
																			                        ,PLS_STATUS)
									                        VALUES(GETDATE()
											                        ,@user
											                        ,'S')
						                        SELECT 1	
						
						
					                        END	
					                        --SELECT TOP 5 Pls_Status
					                        --											FROM [T_PasswordLockoutSettings]
					                        --											WHERE PLS_USERCODE ='1061'
					                        --											ORDER BY Pls_LoginDate DESC
	                        END", encryptPassword(Password), userID, getParamLockOutNumber()-1, passwordQuery);
                    #endregion
                    #endregion
                    query = query.Replace("@ActiveDirectroyByPass",bypass);
                    isNotLock = Convert.ToBoolean(DAL.ExecuteScalar(query));
                    DAL.CommitTransactionSnapshot();
                }
                catch { isNotLock = true;}
                finally { DAL.CloseDB(); }
            }
        }
        else isNotLock = true;
        return isNotLock;
    }
    public int getParamLockOutNumber()
    {
        int top = 0;
        using (DALHelper dal = new DALHelper(false))
        {
            try
            {
                dal.OpenDB();
                string getParamTop = @"
	            DECLARE @TOP int
	            set @TOP = 0
	            IF EXISTS(SELECT Pmt_NumericValue
				            FROM T_ParameterMaster
				            WHERE Pmt_ParameterID='PASSWRDLOK')
		            set @TOP=(SELECT Pmt_NumericValue
					            FROM T_ParameterMaster
					            WHERE Pmt_ParameterID='PASSWRDLOK')
	            SELECT @TOP";
                top = Convert.ToInt32(dal.ExecuteScalar(getParamTop));
            }
            catch { }
            finally
            {
                dal.CloseDB();
            }
            return top;
        }
    }
    protected bool isInMaster(string userCode)
    {
        string userID = "";
        string sqlCheck = @"SELECT emt_employeeid
                            FROM T_EmployeeMaster
                            WHERE emt_employeeid = '{0}'";

        using (DALHelper dal = new DALHelper(Session["dbConn"].ToString()))
        {
            try
            {
                dal.OpenDB();
                userID = Convert.ToString(dal.ExecuteScalar(string.Format(sqlCheck, userCode), CommandType.Text));
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

        return (userID.Trim() == "");

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

    private bool fillDDLProfiles()
    {
        bool success = true;
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
                          AND Prf_DataBaseNo IN (SELECT Upt_DatabaseNo FROM T_UserProfile WHERE Upt_UserCode = '{0}' AND Upt_Status = 'A')";
        using (DALHelper dal = new DALHelper(false))
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, txtUserName.Text), CommandType.Text);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, "SA");
                success = false;
            }
            finally
            {
                dal.CloseDB();
            }
        }
        
        string dbId = string.Empty;
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            ddlProfiles.Items.Clear();
            string connetionStringFormat = @"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";
            string temp = string.Empty;
            for (int ctr = 0; ctr < ds.Tables[0].Rows.Count; ctr++)
            {
                //temp = string.Format(connetionStringFormat, Encrypt.decryptText(ds.Tables[0].Rows[ctr]["Prf_Server"].ToString())
                //                                          , Encrypt.decryptText(ds.Tables[0].Rows[ctr]["Prf_DataBase"].ToString())
                //                                          , Encrypt.decryptText(ds.Tables[0].Rows[ctr]["Prf_UserID"].ToString())
                //                                          , Encrypt.decryptText(ds.Tables[0].Rows[ctr]["Prf_Password"].ToString()) );

                temp = string.Format(connetionStringFormat, ds.Tables[0].Rows[ctr]["Prf_Server"].ToString()
                                                          , ds.Tables[0].Rows[ctr]["Prf_DataBase"].ToString()
                                                          , ds.Tables[0].Rows[ctr]["Prf_UserID"].ToString()
                                                          , ds.Tables[0].Rows[ctr]["Prf_Password"].ToString());

                ddlProfiles.Items.Add(new ListItem( ds.Tables[0].Rows[ctr]["Prf_Profile"].ToString()
                                                  , ds.Tables[0].Rows[ctr]["Prf_DatabasePrefix"].ToString() + Encrypt.encryptText(temp)));

                dbId += ds.Tables[0].Rows[ctr]["Prf_DataBaseNo"].ToString()+ ",";       
            }
        }
        else
        {
            success = false;
        }
        hfDB.Value = Encrypt.encryptText(dbId);
        return success;
    }
    protected void txtUserId_TextChanged(object sender, EventArgs e)
    {
        if (Convert.ToBoolean(Resources.Resource.ACTIVEDIRECTORY) && LND.checkDBExists(Encrypt.decryptText(ConfigurationManager.AppSettings["ADMappingDB"].ToString())))
        {
            //connect check password
            //if true ACTDIRONLY
            bool isACTDIRONLY = false;
            bool inProfile = false;
            using (DALHelper dal = new DALHelper(false))
            {
                try
                {
                    dal.OpenDB();
                   

                    isACTDIRONLY  = Convert.ToBoolean(Int32.Parse(dal.ExecuteScalar(@"IF EXISTS(SELECT [Pmt_NumericValue] 
			                                                        FROM [T_ParameterMaster] 
			                                                        WHERE [Pmt_ParameterID] = 'ACTDIRONLY') 
                                                       BEGIN
	                                                        SELECT [Pmt_CharValue] FROM [T_ParameterMaster]
                                                            WHERE [Pmt_ParameterID] = 'ACTDIRONLY'
                                                        END
                                                        ELSE
	                                                        SELECT '0'").ToString()));
//                    inProfile = Convert.ToBoolean(Int32.Parse(dal.ExecuteScalar(string.Format(@"IF EXISTS(SELECT Umt_Usercode
//			                                                        FROM T_UserMaster 
//			                                                        WHERE Umt_Usercode = '{0}'
//                                                                        AND Umt_Status!='C') 
//                                                       BEGIN
//	                                                        SELECT '1'
//                                                        END
//                                                        ELSE
//	                                                        SELECT '0'", txtActiveDirectory.Text.Trim())).ToString()));
                    
                }
                catch
                { }
                finally
                {
                    dal.CloseDB();
                }
            }
           string UID = LND.retrieveHRCUser(txtActiveDirectory.Text);
           bool isMap = LND.isMappID(txtActiveDirectory.Text.Trim(), "HRC");
            //if(isACTDIRONLY && )
           if ((UID == null || UID == string.Empty || UID.Trim() == "")|| !isACTDIRONLY)
           {
               if (isACTDIRONLY && isMap && UID == null)
                   UID = "";
               else if (UID == null || UID == string.Empty || UID.Trim() == "")
                   UID = txtActiveDirectory.Text;
           }
           txtUserName.Text = UID;
        }
        else
        {
            txtUserName.Text = txtActiveDirectory.Text;
        }
        if (!fillDDLProfiles())
        {
            ddlProfiles.Items.Clear();
            ddlProfiles.Enabled = false;
            MessageBox.Show("No profiles retrieved for user.");
            txtUserName.Focus();
        }
        else
        {
            ddlProfiles.Enabled = true;
            Session["dbConn"] = ddlProfiles.SelectedValue.ToString().Substring(1);
            Session["dbPrefix"] = ddlProfiles.SelectedValue.ToString().Substring(0, 1);
            Session["dbProfileName"] = ddlProfiles.SelectedItem.Text;
            Page.Title = Session["dbProfileName"].ToString() + " - Workflow";
            if (ddlProfiles.Items.Count > 1)
            {
                ddlProfiles.Focus();
            }
            else
            {
                txtPassword.Focus();
            }
        }

        try
        {
            GenerateView();
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, "ANNOUNCE");
        }
    }

    protected void ddlProfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["dbConn"] = ddlProfiles.SelectedValue.ToString().Substring(1);
        Session["dbPrefix"] = ddlProfiles.SelectedValue.ToString().Substring(0, 1);
        Session["dbProfileName"] = ddlProfiles.SelectedItem.Text;
        Page.Title = Session["dbProfileName"].ToString() + " - Workflow";
        txtPassword.Focus();

        try
        {
            GenerateView();
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, "ANNOUNCE");
        }
    }
    #endregion

    #region Announcement Methods
    private DataTable GetData()
    {

        DataSet ds = new DataSet();
        string sql = @"
                    declare @param int
                    SET @param = (SELECT ISNULL(Pmt_NumericValue,30)
				                    FROM T_ParameterMaster
		                           WHERE Pmt_ParameterId = 'ANNCRANGE')

                    SELECT Convert(varchar(10),A.Amt_AnnounceDateTime,101) AS [Announce Date]
                         , Convert(varchar(5),A.Amt_AnnounceDateTime,114) AS [Announce Time]
                         , A.Amt_Subject AS [Subject]
                         , A.Amt_Description AS [Information]
                         , Amt_Priority
                         , CASE Amt_Priority
                           WHEN '1' THEN 'HIGH'
                           WHEN '2' THEN 'MID'
                           WHEN '3' THEN 'LOW'
                            END AS [Priority]
                         , A.Amt_Announcer AS [Announced By]
                         , CASE A.Amt_Status 
                           WHEN 'A' THEN 'ACTIVE'
                           WHEN 'C' THEN 'CANCELLED' 
                            END AS [Status]
                         , A.ludatetime AS [Last Updated]
                      FROM T_AnnouncementMaster A
	                 WHERE A.Amt_AnnounceDateTime BETWEEN DATEADD (day , @param * -1 , getdate())  AND getdate()
                       AND Amt_Status = 'A'
                       {0}
                     ORDER BY A.Amt_AnnounceDateTime DESC , A.Amt_Priority ASC ";
        try
        {
            string[] ProfileDBConnections = GNBL.GetProfileConnections();
            using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
            //using(DALHelper dal=new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, getFilter()), CommandType.Text);
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
        }
        catch
        {
            ds = new DataSet();
        }

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0];
        }
        else
        {
            return new DataTable("dummy");
        }
    }

    private string getFilter()
    {
        string filter = string.Empty;
        filter += @" AND Amt_ProfileInclude = 'ALL' ";
        //if (!ddlProfiles.Items.Count.Equals(0))
        //{
        //    string[] separator = new string[1];
        //    separator[0] = ",";
        //    string[] dbID = Encrypt.decryptText(hfDB.Value).ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
        //    filter += " OR ( ";
        //    for (int i = 0; i < dbID.Length; i++)
        //    { 
        //        if(i == 0)
        //        {
        //            filter += string.Format(" Amt_ProfileInclude LIKE '%!{0}!%'", dbID[i]);
        //        }
        //        else
        //        {
        //            filter += string.Format(" OR Amt_ProfileInclude LIKE '%!{0}!%'", dbID[i]);
        //        }
        //    }

        //    filter += @" ) ";
        //}

        return filter;
    }

    private void GenerateView()
    {
        DataTable dt = GetData();
        string holdDate = string.Empty;
        bool isNewDate = true;
        bool firstRow = true;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (!firstRow)
                {
                    if (!holdDate.Equals(dr["Announce Date"].ToString()))
                    {
                        isNewDate = true;
                    }
                }
                if (holdDate.Equals(string.Empty) || isNewDate)
                {
                    holdDate = dr["Announce Date"].ToString();
                    tblAnnounce.Rows.Add(AddHeader(holdDate));
                    isNewDate = false;
                }
                firstRow = false;
                tblAnnounce.Rows.Add(AddBody(dr["Announce Time"].ToString()
                                            , dr["Subject"].ToString()
                                            , dr["Information"].ToString()
                                            , dr["Announced By"].ToString()
                                            , dr["Amt_Priority"].ToString()));
            }
        }
        else
        {
            TableRow tr1 = null;
            TableCell tc1 = null;
            tr1 = new TableRow();
            tc1 = new TableCell();
            tc1.Text = "<hr><h5>No Announcements</h5> ";
            tc1.Width = Unit.Pixel(890);
            tc1.Height = Unit.Percentage(100);
            tc1.ColumnSpan = 2;
            tr1.Cells.Add(tc1);
            tblAnnounce.Rows.Add(tr1);
        }
//        //Getting Started
//        TableRow tr = null;
//        TableCell tc = null;

//        tr = new TableRow();
//        tc = new TableCell();
//        tc.Text = "<hr><hr><h5>Getting started...</h5> ";
//        tc.ColumnSpan = 2;
//        tr.Cells.Add(tc);
//        tblAnnounce.Rows.Add(tr);

//        tr = new TableRow();
//        tc = new TableCell();
//        tc.Text = "STEP 1";
//        tc.Width = Unit.Pixel(160);
//        tc.VerticalAlign = VerticalAlign.Top;
//        tr.Cells.Add(tc);
//        tc = new TableCell();
//        tc.Text = @"Login at the left panel of this page.
//                    The username of your login is your ID number. 
//                    You are provided with a 5-digit system generated
//                    password. Please change your
//                    password during your first login.";
//        tr.Cells.Add(tc);
//        tblAnnounce.Rows.Add(tr);

//        tr = new TableRow();
//        tc = new TableCell();
//        tc.Text = "STEP 2";
//        tc.VerticalAlign = VerticalAlign.Top;
//        tr.Cells.Add(tc);
//        tc = new TableCell();
//        tc.Text = @"After logging in you will be redirected to the page where
//                    you can navigate the workflow. Some navigation might not 
//                    be available to you due to some restrictions to the user 
//                    type.";
//        tr.Cells.Add(tc);
//        tblAnnounce.Rows.Add(tr);

//        tr = new TableRow();
//        tc = new TableCell();
//        tc.Text = "STEP 3";
//        tc.VerticalAlign = VerticalAlign.Top;
//        tr.Cells.Add(tc);
//        tc = new TableCell();
//        tc.Text = @"Make the necessary transactions.";
//        tr.Cells.Add(tc);
//        tblAnnounce.Rows.Add(tr);

//        tr = new TableRow();
//        tc = new TableCell();
//        tc.Text = "STEP 4";
//        tc.VerticalAlign = VerticalAlign.Top;
//        tr.Cells.Add(tc);
//        tc = new TableCell();
//        tc.Text = @"Logout.";
//        tr.Cells.Add(tc);
//        tblAnnounce.Rows.Add(tr);



    }

    private TableRow AddHeader(string dateHeader)
    {
        TableRow tr = new TableRow();
        TableCell tc = new TableCell();
        tc.ColumnSpan = 2;
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.Font.Bold = true;
        tc.Text = "<hr /><br />";
        tc.Text += Convert.ToDateTime(dateHeader).ToString("MMM dd yyyy") + " - " + Convert.ToDateTime(dateHeader).DayOfWeek;
        tc.CssClass = "cell0";
        tc.Font.Size = FontUnit.Large;
        tr.Cells.Add(tc);

        return tr;
    }

    private TableRow AddBody(string hhmmTime, string subject, string info, string announcer, string priority)
    {
        TableRow tr = new TableRow();
        TableCell tc = null;

        //Time 1st Column
        string temp = announcer.Trim().Equals(string.Empty) ? "-NO INFO-" : announcer.Trim().ToUpper();
        tc = new TableCell();
        tc.Text = "[" + hhmmTime + "]  " + subject + "<br />" + "by: <u>" + temp + "</u>";
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.VerticalAlign = VerticalAlign.Top;
        tc.Width = Unit.Pixel(200);
        tc.Height = Unit.Percentage(100);
        tc.Wrap = true;
        tc.CssClass = "cell1";
        switch (priority)
        {
            case "1":
                tc.ForeColor = System.Drawing.Color.Red;
                break;
            case "2":
                tc.ForeColor = System.Drawing.Color.DodgerBlue;
                break;
            default:
                break;
        }
        tr.Cells.Add(tc);


        //Information 2nd Column
        tc = new TableCell();
        tc.Text = info.Replace("\r\n", "<br />");
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.VerticalAlign = VerticalAlign.Top;
        tc.Wrap = true;
        tc.Width = Unit.Pixel(400);
        tc.Height = Unit.Percentage(100);
        tc.CssClass = "cell2";
        switch (priority)
        {
            case "1":
                tc.ForeColor = System.Drawing.Color.Red;
                break;
            case "2":
                tc.ForeColor = System.Drawing.Color.DodgerBlue;
                break;
            default:
                break;
        }
        tr.Cells.Add(tc);

        return tr;
    } 
    #endregion
}
