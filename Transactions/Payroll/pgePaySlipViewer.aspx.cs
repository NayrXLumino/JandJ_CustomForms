using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommonLibrary;
using Payroll.DAL;
using MethodsLibrary;
using System.Net;


public partial class pgePaySlipViewer : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    private string HRCMenucode = "PAYROLLSLIP";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFPSVIEWING"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeEmployee(false);
                initializeControls();
                //Session["update"]
                //Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                //Page.PreRender += new EventHandler(Page_PreRender);
                initializeIfNoPassword();
                //GeneratePDF2();
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
            }
            LoadComplete += new EventHandler(pgePayslipPreview_LoadComplete);
        }
        

    }

    void Page_PreRender(object sender, EventArgs e)
    {
       ViewState["update"] = Session["update"];
    }

    void pgePayslipPreview_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "payrollScripts";
        string jsurl = "_payroll.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnPayPeriod.OnClientClick = "return lookupPSPayPeriod();";
        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        initializeEmployee(true);
        if (!txtEmployeeId.Text.Equals(Session["userLogged"].ToString()))
        {
            btnChangePassword.Enabled = false;
        }
        else
        {
            btnChangePassword.Enabled = true;
        }
    }

    #region Methods

    private void initializeEmployee(bool flag)
    {
        DataSet ds = new DataSet();
        string sql = @"  SELECT Emt_EmployeeId [ID No]
                              , Emt_NickName [Nickname]
                              , Emt_Lastname [Lastname]
                              , Emt_Firstname [Firstname] 
	                          , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	                          , ISNULL(Dcm_Departmentdesc, '') [Department]
                           FROM T_EmployeeMaster
	                       LEFT JOIN T_DepartmentCodeMaster
	                         ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                          THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                          ELSE ''
							                          END
                          WHERE Emt_EmployeeId = @EmployeeId";
        ParameterInfo[] param = new ParameterInfo[1];
        if (!flag)
        {
            param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString());
        }
        else 
        {
            param[0] = new ParameterInfo("@EmployeeId", txtEmployeeId.Text);
        }
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
        lblNickName.Text = Resources.Resource._3RDINFO;
        if (!CommonMethods.isEmpty(ds))
        {
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFPSVIEWING");
        if (Convert.ToBoolean(Resources.Resource.CHIYODASPECIFIC))
        {
            userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFPSVIEWING");
        }
        btnEmployeeId.Enabled = userGrant.canAdd();
        btnReset.Visible = userGrant.canEdit();
        hfPrevEmployeeId.Value = txtEmployeeId.Text;
        txtPassword.Focus();
    }

    private void initializeIfNoPassword()
    {
        DataSet ds = null;
        string sql = string.Format(@"   SELECT *
                                          FROM T_PasswordVerification
                                         WHERE Pvr_UserCode = '{0}'
                                           AND Pvr_Transaction = 'WFPSVIEWING' ", Session["userLogged"].ToString());
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
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

        if (ds == null
            || ds.Tables[0].Rows.Count == 0)
        {
            pnlChargePassword.Visible = true;
            this.newPasswordFlag.Value = "TRUE";
            this.Label2.Visible = false;
            this.txtCurrentPassword.Visible = false;
            //this.txtCurrentPassword.Text = "systemadmin";
            MessageBox.Show("No payslip password setup");
        }
    }

    #region Perth Added for BINARYDB

    private void GeneratePDF2()
    {
        DataSet dsPDF = null;

        #region Retrieval of PDF from Database
        try
        {
            #region Check for PayPeriod if Previous

            DataSet ds = ExecuteDatasetQuery(string.Format(@"
                SELECT 
	                Ppm_CycleIndicator
                FROM T_PayPeriodMaster
                WHERE Ppm_PayPeriod = '{0}'
                ", this.txtPayPeriod.Text.Trim().Replace("'", "")));

            bool isCurrent = true;

            if (ds != null
                && ds.Tables != null
                && ds.Tables.Count > 0
                && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0].ToString().Trim() == "P")
                    isCurrent = false;
            }

            #endregion

            #region Get Data

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ParameterInfo[] param = new ParameterInfo[2];
                    param[0] = new ParameterInfo("@Bft_EmployeeID", "015512");
                    param[1] = new ParameterInfo("@Bft_FileCode", "PAYROLLSLIP_2014032");
                    dsPDF = dal.ExecuteDataSet(@"
                            SELECT 
	                            Bft_BinaryValue
                            FROM BINARYDB..T_BinaryFile
                            WHERE Bft_EmployeeID = @Bft_EmployeeID
	                            AND Bft_FileCode = @Bft_FileCode", CommandType.Text, param);
                }
                catch (Exception er)
                {
                    dsPDF = null;
                    CommonMethods.ErrorsToTextFile(er, "Connect to Binary DB ");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            #endregion
        }
        catch(Exception er)
        {
            CommonMethods.ErrorsToTextFile(er, "GeneratePDF()");
        }
        #endregion

        #region Display PDF
        try
        {
            if (dsPDF != null
                    && dsPDF.Tables != null
                    && dsPDF.Tables.Count > 0
                    && dsPDF.Tables[0].Rows.Count > 0)
            {
                //Response.Flush();
                byte[] buffer = (byte[])dsPDF.Tables[0].Rows[0][0];
                //Response.ClearContent();
                //Response.ClearHeaders();
                //Response.AddHeader("cache-control", "max-age=1");
                Response.ContentType = "application/pdf";
                Response.Clear();
                //Response.AddHeader("content-disposition", "attachment; filename=whatever.pdf");
                Response.AddHeader("content-length", buffer.Length.ToString());
                Response.BufferOutput = true;
                Response.BinaryWrite(buffer);
                
                Response.Flush();
                Response.End();
                MessageBox.Show("Naay solod ang binary");
                //Response.Write('A');
            }
            else
            {
                //throw new Exception("Null");
                MessageBox.Show("Else walay solod");
            }
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message.ToString());
        }
        #endregion
    }

    #endregion
    private void GeneratePDF()
    {

        DataSet dsPDF = null;
        Session["employeeID"] = this.txtEmployeeId.Text.Trim();
        Session["fileCode"] = this.HRCMenucode.Trim() + "_" + this.txtPayPeriod.Text.Trim();

        Response.Write("<script>");
        Response.Write("window.open('../Payroll/pgePaySlipViewer3.aspx','_blank')");

        Response.Write("</script>");

        #region Retrieval of PDF from Database
//        try
//        {
           
//            #region Get Data
//            using (DALHelper dal = new DALHelper())
//            {
//                try
//                {
                    
//                    dal.OpenDB();
//                    ParameterInfo[] param = new ParameterInfo[2];
//                    param[0] = new ParameterInfo("@Bft_EmployeeID", this.txtEmployeeId.Text.Trim());
//                    param[1] = new ParameterInfo("@Bft_FileCode", this.HRCMenucode.Trim() + "_" + this.txtPayPeriod.Text.Trim());
//                    dsPDF = dal.ExecuteDataSet(@"
//                            SELECT 
//	                            Bft_BinaryValue
//                            FROM BINARYDB..T_BinaryFile
//                            WHERE Bft_EmployeeID = @Bft_EmployeeID
//	                            AND Bft_FileCode = @Bft_FileCode
//                                AND Bft_Status='A'", CommandType.Text, param);
//                }
//                catch (Exception er)
//                {
//                    dsPDF = null;
//                    //MessageBox.Show(er.Message.ToString());
//                    CommonMethods.ErrorsToTextFile(er, "Connect to Binary DB ");
//                }
//                finally
//                {
//                    dal.CloseDB();
//                }
//            }

//            #endregion
//        }
//        catch (Exception er)
//        {
//            //MessageBox.Show("Second error");
//            CommonMethods.ErrorsToTextFile(er, "GeneratePDF()");
//        }
        #endregion

        #region Display PDF
        //try
        //{
        //    if (dsPDF != null
        //            && dsPDF.Tables != null
        //            && dsPDF.Tables.Count > 0
        //            && dsPDF.Tables[0].Rows.Count > 0)
        //    {
        //        byte[] buffer = (byte[])dsPDF.Tables[0].Rows[0][0];
        //        //Response.ClearContent();
        //        //Response.ClearHeaders();
        //        //Response.AddHeader("cache-control", "max-age=1");
        //        Response.ContentType = "application/pdf";
        //        //Response.AddHeader("content-disposition", "attachment; filename=whatever.pdf");
        //        Response.AddHeader("content-length", buffer.Length.ToString());
        //        Response.BinaryWrite(buffer);
        //        // Response.Write('A');
        //        Response.Flush();
        //        Response.End();

        //        //MessageBox.Show("Binary found a file");
        //    }
        //    else
        //    {
        //        throw new Exception("Payslip is not yet available");
        //        //MessageBox.Show(dsPDF.Tables[0].Rows.Count.ToString());
        //    }
        //}
        //catch(Exception ex)
        //{
        //    MessageBox.Show("Payslip is not yet available");
        //    //MessageBox.Show(ex.Message.ToString());
        //}
        #endregion

       
    }
    private DataSet ExecuteDatasetQuery(string query)
    {
        DataSet ds = null;

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
            }
            catch(Exception er)
            {
                MessageBox.Show("Error in query : " + er.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return ds;
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        GeneratePDF();
        //MenuLOg

        SystemMenuLogBL.InsertGenerateLog("WFPSVIEWING", txtEmployeeId.Text, true, Session["userLogged"].ToString());
        #region Previous
        //        string url = string.Empty;
//        try
//        {
//            string query = string.Format(@" SELECT Bft_BinaryValue
//                                              FROM T_BinaryFile
//                                             WHERE Bft_EmployeeId = '{0}'
//                                               AND Bft_FileCode = '{1}'
//                                               AND Bft_FileType = 'PDF' ", this.txtEmployeeId.Text.Trim()
//                                                                         , this.HRCMenucode.Trim() + "_" + this.txtPayPeriod.Text.Trim());

//            DataSet ds = ExecuteDatasetQuery(query);

//            if (ds != null
//                && ds.Tables[0].Rows.Count > 0)
//            {
//                byte[] buffer = (byte[])ds.Tables[0].Rows[0][0];
//                Response.ContentType = "application/pdf";
//                Response.AddHeader("content-length", buffer.Length.ToString());
//                Response.BinaryWrite(buffer);
//            }
//            else
//            {
//                throw new Exception("Payslip is not yet available");
//            }
//        }
//        catch
//        {
//            MessageBox.Show("Payslip is not yet available");
        //        }
        #endregion

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string encryptPswd = encryptPassword(txtPassword.Text);
        string Userpassword = string.Empty;
        #region SQL
        string sqlPassword = @" SELECT Pvr_UserPswd
                                  FROM T_PasswordVerification 
                                 WHERE Pvr_UserCode = '{0}' 
                                   AND Pvr_Transaction = 'WFPSVIEWING' ";
        #endregion
        if (Page.IsValid)
        {
            if(txtPassword.Text.Equals(Encrypt.decryptText(Resources.Resource.DEFAULTPAYSLIPPASS)))
            {
                mtvPayslipViewing.ActiveViewIndex = 1;
            }
            else
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        Userpassword = dal.ExecuteScalar(string.Format(sqlPassword, txtEmployeeId.Text), CommandType.Text).ToString();
                    }
                    catch(Exception ex)
                    {
                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                if (encryptPswd.Equals(Userpassword))
                {
                    mtvPayslipViewing.ActiveViewIndex = 1;
                    //Menu Log
                    SystemMenuLogBL.InsertAddLog("WFPSVIEWING", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                }
                else
                {
                    mtvPayslipViewing.ActiveViewIndex = 0;
                    MessageBox.Show("Incorrect Password");
                }
            }
        }
    }

    protected void btnChangePassword_Click(object sender, EventArgs e)
    {
        pnlChargePassword.Visible = true;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pnlChargePassword.Visible = false;
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
    private string encryptPasswordWhole(string password)
    {
        string pass = Encrypt.encryptText(password).ToString();
        return pass;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string encryptPswd = encryptPassword(txtPassword.Text);
        string encryotPswdWhole = encryptPasswordWhole(txtPassword.Text);
        string Userpassword = string.Empty;
        #region SQL
        string sqlPassword = @" SELECT Pvr_UserPswd
                                  FROM T_PasswordVerification 
                                 WHERE Pvr_UserCode = '{0}' 
                                   AND Pvr_Transaction = 'WFPSVIEWING' ";
        string sqlUpdatePassword = @" UPDATE T_PasswordVerification
                                         SET Pvr_UserPswd = '{1}'
                                            ,Pvr_UserPswdWhole='{2}'
                                       WHERE Pvr_UserCode = '{0}' 
                                         AND Pvr_Transaction = 'WFPSVIEWING' ";

        string sqlInsertPassword = @" INSERT INTO T_PasswordVerification
                                      SELECT 'WFPSVIEWING'
                                           , '{0}'
                                           , '{1}'
                                           , '{2}'
                                           , '{0}', GETDATE() ";
        #endregion
        if (this.newPasswordFlag.Value.ToString() == "TRUE")
        {
            if (txtNewPassword.Text.Equals(txConfirmPassword.Text.Trim()))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransaction();
                        dal.ExecuteNonQuery(string.Format(sqlInsertPassword, txtEmployeeId.Text, encryptPassword(txtNewPassword.Text), encryptPasswordWhole(txtNewPassword.Text)), CommandType.Text);

                        dal.CommitTransaction();
                        pnlChargePassword.Visible = false;
                        MessageBox.Show("Successfully inserted new password");
                        this.newPasswordFlag.Value = string.Empty;
                        this.txtCurrentPassword.Visible = true;
                        this.Label2.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                        dal.RollBackTransaction();
                        MessageBox.Show("Saving unsuccessful.");
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            else
            {
                MessageBox.Show("Passwords do not match.");
            }
            //return;
        }
        if (Page.IsValid)
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    Userpassword = dal.ExecuteScalar(string.Format(sqlPassword, txtEmployeeId.Text), CommandType.Text).ToString();
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
            string currentPayslipPassword = encryptPassword(this.txtCurrentPassword.Text.Trim());
            if (currentPayslipPassword.Equals(Userpassword))
            {
                if (txtNewPassword.Text.Equals(txConfirmPassword.Text.Trim()))
                {
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransaction();
                            dal.ExecuteNonQuery(string.Format(sqlUpdatePassword, txtEmployeeId.Text, encryptPassword(txtNewPassword.Text), encryptPasswordWhole(txtNewPassword.Text)), CommandType.Text);
                            //Menu Log
                            SystemMenuLogBL.InsertAddLog("WFPSVIEWING", true, txtEmployeeId.Text.ToString(), Session["userLogged"].ToString(), "");
                            dal.CommitTransaction();
                            pnlChargePassword.Visible = false;
                            MessageBox.Show("Successfully updated password");
                        }
                        catch (Exception ex)
                        {
                            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                            dal.RollBackTransaction();
                            MessageBox.Show("Saving unsuccessful.");
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Passwords do not match.");
                }
            }
            else
            {
                MessageBox.Show("Incorrect Current Password.");
            }
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        mtvPayslipViewing.ActiveViewIndex = 0;
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        string sql = @" UPDATE T_PasswordVerification
                           SET Pvr_UserPswd = '{1}'
                             , Usr_Login = '{2}'
                             , Pvr_UserPswdWhole = '{3}'
                             , Ludatetime = GETDATE()
                         WHERE Pvr_UserCode = '{0}'
                           AND Pvr_Transaction = 'WFPSVIEWING'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dal.BeginTransaction();
                dal.ExecuteNonQuery(string.Format(sql, txtEmployeeId.Text
                                                     , encryptPassword(Resources.Resource.RESETPASSWORD)
                                                     , Session["userLogged"].ToString()
                                                     , encryptPasswordWhole(Resources.Resource.RESETPASSWORD)), CommandType.Text);
                //MenuLog
                SystemMenuLogBL.InsertAddLog("WFPSVIEWING", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");


                dal.CommitTransaction();
                MessageBox.Show("Successfully resetted password: " + Resources.Resource.RESETPASSWORD);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to reset password. Contact system administrator.");
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                dal.RollBackTransaction();
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }

    #endregion
}
