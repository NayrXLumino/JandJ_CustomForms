using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using CommonLibrary;
using System.Collections;
using System.Web.Services;

public partial class Maintenance_ApprovalRoute_pgeNewEmployeeApprovalRoute : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    GeneralBL GNBL = new GeneralBL();
    MenuGrant MGBL = new MenuGrant();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFROUTEENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeEmployee();
                hfEmployeeID.Value = txtEmployeeId.Text;
                hfEmployeeName.Value = txtEmployeeName.Text;
                hfCostCenter.Value = txtNickname.Text;
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                //if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                //{
                //    if (!hfPrevEntry.Value.Equals(changeSnapShot()))
                //    {
                //        MessageBox.Show("There were some changes made and was not saved.");
                //    }
                txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                //    hfEncryptId.Value = Encrypt.encryptText(txtEmployeeId.Text);
                //    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                
            }
        }
        LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeNewEmployeeApprovalRoute_LoadComplete);
    }
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }
    void Maintenance_ApprovalRoute_pgeNewEmployeeApprovalRoute_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployeeId.OnClientClick = string.Format("return lookupEmployeeApprovalRoute('GENERAL')", txtEmployeeId.Text);
        btnOvertime.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('OVERTIME', '{0}', 'NEW')", txtEmployeeId.Text);
        btnLeave.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('LEAVE', '{0}', 'NEW')", txtEmployeeId.Text);
        btnAddress.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('ADDRESS', '{0}', 'NEW')", txtEmployeeId.Text);
        btnBeneficiary.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('BNEFICIARY', '{0}', 'NEW')", txtEmployeeId.Text);
        btnManPower.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('MANPOWER', '{0}', 'NEW')", txtEmployeeId.Text);
        btnTaxCivil.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('TAXMVMNT', '{0}', 'NEW')", txtEmployeeId.Text);
        btnTimeRecord.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('TIMEMOD', '{0}', 'NEW')", txtEmployeeId.Text);
        btnTraining.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('TRAINING', '{0}', 'NEW')", txtEmployeeId.Text);
        btnWorkInfo.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('MOVEMENT', '{0}', 'NEW')", txtEmployeeId.Text);
        btnGatePass.OnClientClick = string.Format("return lookEmployeeApprovalRouteTransaction('GATEPASS', '{0}', 'NEW')", txtEmployeeId.Text);
        
    }
    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        txtEmployeeId.Text = hfEmployeeID.Value.ToString();
        txtEmployeeName.Text = hfEmployeeName.Value.ToString();
        txtNickname.Text = hfCostCenter.Value.ToString();
        initializeControls();
        setCostcenterAssignment(txtEmployeeId.Text);
    }
    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFROUTEENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();

        btnAddress.Enabled = userGrant.canEdit();
        btnBeneficiary.Enabled = userGrant.canEdit();
        btnLeave.Enabled = userGrant.canEdit();
        btnJobsplit.Enabled = userGrant.canEdit();
        btnOvertime.Enabled = userGrant.canEdit();
        btnStraightWork.Enabled = userGrant.canEdit();
        btnTaxCivil.Enabled = userGrant.canEdit();
        btnTimeRecord.Enabled = userGrant.canEdit();
        btnWorkInfo.Enabled = userGrant.canEdit();
        btnManPower.Enabled = userGrant.canEdit();
        btnTraining.Enabled = userGrant.canEdit();
        btnGatePass.Enabled = userGrant.canEdit();
        //btnY.Visible = userGrant.canEdit();
        //btnX.Visible = userGrant.canEdit();

        //hfPrevEmployeeId.Value = txtEmployeeId.Text;
        initializeRouteAssignment();
        showRowTransactionUsed();
        //hfPrevEntry.Value = changeSnapShot();
        //SetMinDate();
        
    }
    private void showRowTransactionUsed()
    {
        //based on applicable transaction in company
        pnlAD.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWADDRESS);
        pnlBE.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWBENEFICIARY);
        pnlLV.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE);
        pnlMP.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWMANPOWER);
        pnlOT.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME);
        pnlTC.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTAXCIVIL);
        pnlTR.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION);
        pnlTN.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTRAINING);
        pnlWI.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION);
        pnlJB.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWJOBSPLIT);
        pnlSW.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWSTRAIGHTWORK);
        pnlGP.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWGATEPASS);
    }
    private void initializeEmployee()
    {
        DataSet ds = new DataSet();

        string sql = @"  
                    SELECT Emt_EmployeeId [ID No]
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
                    WHERE Emt_EmployeeId = @EmployeeId

                    UNION

                    SELECT
	                    Umt_Usercode [ID No]
                        , Umt_userfname [Nickname]
                        , Umt_userlname [Lastname]
                        , Umt_userfname [Firstname] 
	                    , '' [Costcenter]
	                    , '' [Department]
                    FROM T_UserMaster
                    WHERE Umt_Usercode = @EmployeeId
                                        ";

        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", Session["_employeeID"] == null ? Session["userLogged"].ToString() : Session["_employeeID"].ToString());
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
            //hfEncryptId.Value = Encrypt.encryptText(txtEmployeeId.Text);
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0]["Costcenter"].ToString();

        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
        }
    }
    private void initializeRouteAssignment()
    {
        DataSet ds = new DataSet();
        
        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text,"OVERTIME");
        dgvOvertime.DataSource = ds.Tables[0];
        dgvOvertime.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text,"LEAVE");            
        dgvLeave.DataSource = ds.Tables[0];
        dgvLeave.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text,"TIMEMOD");            
        dgvTimeRecord.DataSource = ds.Tables[0];
        dgvTimeRecord.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "MOVEMENT");
        dgvWorkInfo.DataSource = ds.Tables[0];
        dgvWorkInfo.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "BNEFICIARY");
        dgvBeneficiary.DataSource = ds.Tables[0];
        dgvBeneficiary.DataBind();
           
        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "TAXMVMNT");
        dgvTaxCivil.DataSource = ds.Tables[0];
        dgvTaxCivil.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "ADDRESS");
        dgvAddress.DataSource = ds.Tables[0];
        dgvAddress.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "MANPOWER");
        dgvManPower.DataSource = ds.Tables[0];
        dgvManPower.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "TRAINING");
        dgvTraining.DataSource = ds.Tables[0];
        dgvTraining.DataBind();

        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "GATEPASS");
        dgvGatePass.DataSource = ds.Tables[0];
        dgvGatePass.DataBind();
        }
    private void setCostcenterAssignment(string employeeID)
    {
        string sql = @" SELECT dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter] 
                          FROM T_EmployeeMaster
                         WHERE Emt_EmployeeID = '{0}' ";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, employeeID), CommandType.Text);
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
            txtNickname.Text = ds.Tables[0].Rows[0]["Costcenter"].ToString();
        }
        else
        {
            txtNickname.Text = string.Empty;
        }
    }
    
    private int DeleteEmployeeRoute(string employeeID
                                        , string transactionID
                                        , string routeID
                                        , string startDate
                                        , DALHelper dal)
    {
        int retVal = 0;
        string query = string.Format(@"DELETE FROM T_EmployeeApprovalRoute
                            WHERE Arm_EmployeeId = '{0}'
                            AND Arm_TransactionID = '{1}'
                            AND Arm_RouteID = '{2}'
                            AND Arm_StartDate = '{3}'", employeeID, transactionID, routeID, startDate);
       
           return retVal = Convert.ToInt32(dal.ExecuteNonQuery(query));
    }
    private void CorrectEmployeeApprovalRouteEffectivity(string employeeID, string userLogIn, string transactionID, DALHelper dal)
    {
        string query = string.Format(@"UPDATE T_EmployeeApprovalRoute
                            SET Arm_StartDate = CORRECTED.NewStartDate
	                            , Arm_EndDate = CORRECTED.NewEndDate
                                , Usr_Login = '{1}'
                                , Ludatetime = GETDATE()
                            FROM T_EmployeeApprovalRoute ORIG
                            INNER JOIN (
	                            SELECT A.Arm_EmployeeID AS EmployeeID
		                            , DATEADD(dd, 0, DATEDIFF(dd, 0, A.Arm_StartDate)) AS NewStartDate
		                            , (
				                            SELECT TOP 1 DATEADD(dd, 0, DATEDIFF(dd, 0, B.Arm_StartDate)) - 1 
				                            FROM T_EmployeeApprovalRoute B
				                            WHERE A.Arm_EmployeeID = B.Arm_EmployeeID
				                            AND B.Arm_StartDate > A.Arm_StartDate
                                            AND B.Arm_EmployeeID =  '{0}'
											AND B.Arm_TransactionID = '{2}'
			                            ) AS NewEndDate
	                            FROM T_EmployeeApprovalRoute A
                            ) CORRECTED
                            ON ORIG.Arm_EmployeeID = CORRECTED.EmployeeID
	                            AND DATEADD(dd, 0, DATEDIFF(dd, 0, ORIG.Arm_StartDate)) = CORRECTED.NewStartDate
                            WHERE (ORIG.Arm_StartDate != CORRECTED.NewStartDate
	                            OR ORIG.Arm_EndDate != CORRECTED.NewEndDate
	                            OR (ORIG.Arm_EndDate IS NULL AND CORRECTED.NewEndDate IS NOT NULL)
	                            OR (ORIG.Arm_EndDate IS NOT NULL AND CORRECTED.NewEndDate IS NULL))
                                AND ORIG.Arm_EmployeeID = '{0}'
                                AND ORIG.Arm_TransactionID = '{2}'", employeeID, userLogIn, transactionID);
       
            dal.ExecuteNonQuery(query);
    }

    #region Row Editing
    protected void dgvOvertime_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "OVERTIME", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=OVERTIME&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }

    protected void dgvJobsplit_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "JOBSPLIT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=JOBSPLIT&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvStraightWork_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "STRAIGHTWK", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=STRAIGHTWK&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }


    protected void dgvLeave_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "LEAVE", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=LEAVE&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvTimeRecord_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "TIMEMOD", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=TIMEMOD&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvWorkInfo_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "MOVEMENT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=MOVEMENT&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvBeneficiary_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "BNEFICIARY", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=BNEFICIARY&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvTaxCivil_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "TAXMVMNT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=TAXMVMNT&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvAddress_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "ADDRESS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=ADDRESS&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvManPower_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "MANPOWER", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=MANPOWER&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=450,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvTraining_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "TRAINING", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=TRAINING&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=250,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    protected void dgvGatePass_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (isBelongToPastPayperiod(txtEmployeeId.Text, "GATEPASS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
        {
            MessageBox.Show(string.Format("Cannot edit record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
        }
        else
        {
            string url = "pgePopEmployeeApprovalRoute.aspx?transaction=GATEPASS&employeeId=" + txtEmployeeId.Text + "&state=EDIT&startDate=" + hfStartDate.Value.ToString().Trim() + "&endDate=" + hfEndDate.Value.ToString() + "&routeID=" + hfRouteID.Value.ToString();
            string s = "window.open('" + url + "', 'popup_window', 'scrollbars=no,resizable=no,width=400,height=340,left=250,top=250');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
    }
    #endregion
    #region Row Data Bound
    protected void dgvOvertime_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvOvertime')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }

    protected void dgvLeave_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none'; ";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvLeave')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvTimeRecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvTimeRecord')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvWorkInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvWorkInfo')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvBeneficiary_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvBeneficiary')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvTaxCivil_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvTaxCivil')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvAddress_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvAddress')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvManPower_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvManPower')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvTraining_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvTraining')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvJobsplit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvJobsplit')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvStraightWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvStraightWork')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    protected void dgvGatePass_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:return ColorSelected('" + e.Row.RowIndex + "', 'dgvGatePass')");
            ((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "if(!confirm('Do you want to delete " + e.Row.Cells[2].Text + "-" + e.Row.Cells[0].Text + "?')){ return false; };";
        }
    }
    #endregion

    #region Page Indexing
    protected void dgvOvertime_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvOvertime.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "OVERTIME").Tables[0];
        dgvOvertime.PageIndex = e.NewPageIndex;
        dgvOvertime.DataBind();
    }
    protected void dgvLeave_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvLeave.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "LEAVE").Tables[0];
        dgvLeave.PageIndex = e.NewPageIndex;
        dgvLeave.DataBind();
    }
    protected void dgvTimeRecord_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvTimeRecord.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "TIMEMOD").Tables[0];
        dgvTimeRecord.PageIndex = e.NewPageIndex;
        dgvTimeRecord.DataBind();
    }
    protected void dgvWorkInfo_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvWorkInfo.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "MOVEMENT").Tables[0];
        dgvWorkInfo.PageIndex = e.NewPageIndex;
        dgvWorkInfo.DataBind();
    }
    protected void dgvBeneficiary_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvBeneficiary.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "BNEFICIARY").Tables[0];
        dgvBeneficiary.PageIndex = e.NewPageIndex;
        dgvBeneficiary.DataBind();
    }
    protected void dgvTaxCivil_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvTaxCivil.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "TAXMVMNT").Tables[0];
        dgvTaxCivil.PageIndex = e.NewPageIndex;
        dgvTaxCivil.DataBind();
    }
    protected void dgvAddress_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvAddress.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "ADDRESS").Tables[0];
        dgvAddress.PageIndex = e.NewPageIndex;
        dgvAddress.DataBind();
    }
    protected void dgvManPower_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvManPower.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "MANPOWER").Tables[0];
        dgvManPower.PageIndex = e.NewPageIndex;
        dgvManPower.DataBind();

    }
    protected void dgvTraining_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvTraining.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "TRAINING").Tables[0];
        dgvTraining.PageIndex = e.NewPageIndex;
        dgvTraining.DataBind();
    }
    protected void dgvJobsplit_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvJobsplit.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "JOBSPLIT").Tables[0];
        dgvJobsplit.PageIndex = e.NewPageIndex;
        dgvJobsplit.DataBind();
    }
    protected void dgvStraightWork_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvStraightWork.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "STRAIGHTWK").Tables[0];
        dgvStraightWork.PageIndex = e.NewPageIndex;
        dgvStraightWork.DataBind();
    }
    protected void dgvGatePass_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvGatePass.DataSource = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text, "GATEPASS").Tables[0];
        dgvGatePass.PageIndex = e.NewPageIndex;
        dgvGatePass.DataBind();
    }
    #endregion

    #region Row Deleting
    protected void dgvOvertime_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
       
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if(isBelongToPastPayperiod(txtEmployeeId.Text,"OVERTIME",hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                     MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "OVERTIME", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "OVERTIME", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    } 
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Overtime record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }

        initializeRouteAssignment();
    }

    protected void dgvLeave_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "LEAVE", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                     MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "LEAVE", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "LEAVE", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }

                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Leave record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }

        initializeRouteAssignment();
    }
    protected void dgvTimeRecord_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "TIMEMOD", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "TIMEMOD", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "TIMEMOD", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Time Modification record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }

        initializeRouteAssignment();
    }
    protected void dgvWorkInfo_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "MOVEMENT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "MOVEMENT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "MOVEMENT", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Movement record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }

        initializeRouteAssignment();
    }
    protected void dgvBeneficiary_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "BNEFICIARY", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {                    
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "BNEFICIARY", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "BNEFICIARY", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Beneficiary record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvTaxCivil_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "TAXMVMNT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "TAXMVMNT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "TAXMVMNT", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Tax Code /Civil Status record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvAddress_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "ADDRESS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "ADDRESS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "ADDRESS", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Address record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvManPower_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "MANPOWER", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "MANPOWER", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "MANPOWER", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Man Power record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvTraining_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "TRAINING", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "TRAINING", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "TRAINING", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Training record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvJobsplit_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "JOBSPLIT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "JOBSPLIT", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "JOBSPLIT", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during deletion of Jobsplit record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }

        initializeRouteAssignment();
    }
    protected void dgvStraightWork_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "STRAIGHTWK", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "STRAIGHTWK", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "STRAIGHTWK", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during the deletion of Straight Work record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }
    protected void dgvGatePass_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dal.BeginTransaction();
            try
            {
                if (isBelongToPastPayperiod(txtEmployeeId.Text, "GATEPASS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString()))
                {
                    MessageBox.Show(string.Format("Cannot delete record from previous quincena. [Start Date]: {0}", hfStartDate.Value));
                }
                else
                {
                    if (DeleteEmployeeRoute(txtEmployeeId.Text, "GATEPASS", hfRouteID.Value.ToString(), hfStartDate.Value.ToString(), dal) > 0)
                    {
                        CorrectEmployeeApprovalRouteEffectivity(txtEmployeeId.Text, Session["userLogged"].ToString(), "GATEPASS", dal);
                        dal.CommitTransaction();
                        MessageBox.Show("Record successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record(s) deleted.");
                    }
                }
            }
            catch
            {
                dal.RollBackTransaction();
                MessageBox.Show("An error occurred during the deletion of Gate Pass record.");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        initializeRouteAssignment();
    }

    private bool isBelongToPastPayperiod(string employeeID, string transactionID, string routeID, string startDate)
    {
        string query = string.Format(@"SELECT Arm_StartDate
                        FROM T_EmployeeApprovalRoute
                        WHERE Arm_EmployeeID = '{0}'
                        AND '{1}' < (SELECT Ppm_StartCycle FROM T_PayperiodMaster
                        WHERE Ppm_CycleIndicator = 'C')
                        AND Arm_RouteID = '{2}'
                        AND Arm_TransactionID = '{3}'", employeeID, startDate, routeID, transactionID);

        DataTable dtResult = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }

        return dtResult.Rows.Count > 0 ? true : false;
    }
    #endregion
   
    protected void dgvOvertime_SelectedIndexChanged(object sender, EventArgs e)
    {
        string rowNum = dgvOvertime.SelectedRow.Cells[0].Text;
    }
}