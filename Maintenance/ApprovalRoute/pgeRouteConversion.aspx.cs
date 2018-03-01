using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class Maintenance_ApprovalRoute_pgeRouteConversion : System.Web.UI.Page
{

    private MenuGrant MGBL = new MenuGrant();
    private CommonMethods methods = new CommonMethods();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFROUTEREVISION"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                if (!hfPrevFromRoute.Value.Equals(txtFromRoute.Text))
                {
                    //dtpOTDate_Change(dtpOTDate, new EventArgs());
                    hfPrevFromRoute.Value = txtFromRoute.Text;
                }
            }
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeRouteConversion_LoadComplete);
        }
    }

    #region Events

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Maintenance_ApprovalRoute_pgeRouteConversion_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        btnFromRoute.Attributes.Add("OnClick", "javascript:return lookupARRouteAssignmentRC('txtFromRoute');");
        btnToRoute.Attributes.Add("OnClick", "javascript:return lookupARRouteAssignmentRC('txtToRoute');");
        txtFromRoute.Attributes.Add("readOnly", "true");
        txtFromRouteC1.Attributes.Add("readOnly", "true");
        txtFromRouteC2.Attributes.Add("readOnly", "true");
        txtFromRouteAP.Attributes.Add("readOnly", "true");
        txtToRoute.Attributes.Add("readOnly", "true");
        txtToRouteC1.Attributes.Add("readOnly", "true");
        txtToRouteC2.Attributes.Add("readOnly", "true");
        txtToRouteAP.Attributes.Add("readOnly", "true");

        foreach (ListItem checkbox in cbxTransactions.Items)
        {
            checkbox.Attributes.Add("onClick", "javascript:__doPostBack()");
        }
    }

    protected void rblBound_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblBound.SelectedValue == "C")
        {
            mtvBound.ActiveViewIndex = 0;
            FillCostCenterList();
        }
        else if (rblBound.SelectedValue == "E")
        {
            mtvBound.ActiveViewIndex = 1;
            FillEmployeeList();
        }
    }

    protected void txtFromRoute_TextChanged(object sender, EventArgs e)
    {
        lbxCostcenterInclude.Items.Clear();
        lbxEmployeeInclude.Items.Clear();

        if (rblBound.SelectedValue == "C" && mtvBound.ActiveViewIndex == 0)
        {
            FillCostCenterList();
        }
        else if (rblBound.SelectedValue == "E" && mtvBound.ActiveViewIndex == 1)
        {
            FillEmployeeList();
        }
    }
    
    protected void cbxTransactions_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblBound.SelectedValue == "C" && mtvBound.ActiveViewIndex == 0)
        {
            FillCostCenterList();
        }
        else if (rblBound.SelectedValue == "E" && mtvBound.ActiveViewIndex == 1)
        {
            FillEmployeeList();
        }
    }
    
    protected void txtSearchCostcenter_TextChanged(object sender, EventArgs e)
    {
        FillCostCenterList();
    }

    protected void txtSearchEmployee_TextChanged(object sender, EventArgs e)
    {
        FillEmployeeList();
    }

    protected void btnCostcenterSelectIndividual_Click(object sender, EventArgs e)
    {
        if (lbcCostcenterChoice.SelectedIndex > -1)
        {
            lbxCostcenterInclude.Items.Add(lbcCostcenterChoice.SelectedItem);
            lbcCostcenterChoice.Items.Remove(lbcCostcenterChoice.SelectedItem);
            lbcCostcenterChoice.SelectedIndex = -1;
            lbxCostcenterInclude.SelectedIndex = -1;
            
            lblNoOfCostcentersChoice.Text = lbcCostcenterChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfCostcentersInclude.Text = lbxCostcenterInclude.Items.Count.ToString() + " no. of item(s)";
            try
            {
                lbcCostcenterChoice.SelectedIndex = 0;
                lbcCostcenterChoice.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnCostcenterRemoveIndividual_Click(object sender, EventArgs e)
    {
        if (lbxCostcenterInclude.SelectedIndex > -1)
        {
            lbcCostcenterChoice.Items.Add(lbxCostcenterInclude.SelectedItem);
            lbxCostcenterInclude.Items.Remove(lbxCostcenterInclude.SelectedItem);
            lbxCostcenterInclude.SelectedIndex = -1;
            lbcCostcenterChoice.SelectedIndex = -1;

            lblNoOfCostcentersChoice.Text = lbcCostcenterChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfCostcentersInclude.Text = lbxCostcenterInclude.Items.Count.ToString() + " no. of item(s)";
            try
            {
                lbxCostcenterInclude.SelectedIndex = 0;
                lbxCostcenterInclude.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnCostcenterSelectAll_Click(object sender, EventArgs e)
    {
        while (lbcCostcenterChoice.Items.Count > 0)
        {
            lbxCostcenterInclude.Items.Add(lbcCostcenterChoice.Items[0]);
            lbcCostcenterChoice.Items.Remove(lbcCostcenterChoice.Items[0]);
            lbxCostcenterInclude.SelectedIndex = -1;
            lbcCostcenterChoice.SelectedIndex = -1;

            lblNoOfCostcentersChoice.Text = lbcCostcenterChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfCostcentersInclude.Text = lbxCostcenterInclude.Items.Count.ToString() + " no. of item(s)";
        }
    }

    protected void btnCostcenterRemoveAll_Click(object sender, EventArgs e)
    {
        while (lbxCostcenterInclude.Items.Count > 0)
        {
            lbcCostcenterChoice.Items.Add(lbxCostcenterInclude.Items[0]);
            lbxCostcenterInclude.Items.Remove(lbxCostcenterInclude.Items[0]);
            lbxCostcenterInclude.SelectedIndex = -1;
            lbcCostcenterChoice.SelectedIndex = -1;

            lblNoOfCostcentersChoice.Text = lbcCostcenterChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfCostcentersInclude.Text = lbxCostcenterInclude.Items.Count.ToString() + " no. of item(s)";
        }
    }

    protected void btnEmployeeSelectIndividual_Click(object sender, EventArgs e)
    {
        if (lbcEmployeeChoice.SelectedIndex > -1)
        {
            lbxEmployeeInclude.Items.Add(lbcEmployeeChoice.SelectedItem);
            lbcEmployeeChoice.Items.Remove(lbcEmployeeChoice.SelectedItem);
            lbcEmployeeChoice.SelectedIndex = -1;
            lbxEmployeeInclude.SelectedIndex = -1;

            lblNoOfEmployeesChoice.Text = lbcEmployeeChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfEmployeesInclude.Text = lbxEmployeeInclude.Items.Count.ToString() + " no. of item(s)";
            try
            {
                lbcEmployeeChoice.SelectedIndex = 0;
                lbcEmployeeChoice.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnEmployeeRemoveIndividual_Click(object sender, EventArgs e)
    {
        if (lbxEmployeeInclude.SelectedIndex > -1)
        {
            lbcEmployeeChoice.Items.Add(lbxEmployeeInclude.SelectedItem);
            lbxEmployeeInclude.Items.Remove(lbxEmployeeInclude.SelectedItem);
            lbxEmployeeInclude.SelectedIndex = -1;
            lbcEmployeeChoice.SelectedIndex = -1;

            lblNoOfEmployeesChoice.Text = lbcEmployeeChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfEmployeesInclude.Text = lbxEmployeeInclude.Items.Count.ToString() + " no. of item(s)";
            try
            {
                lbxEmployeeInclude.SelectedIndex = 0;
                lbxEmployeeInclude.Focus();
            }
            catch
            {
                //no exeption to do
            }
        }
    }

    protected void btnEmployeeSelectAll_Click(object sender, EventArgs e)
    {
        while (lbcEmployeeChoice.Items.Count > 0)
        {
            lbxEmployeeInclude.Items.Add(lbcEmployeeChoice.Items[0]);
            lbcEmployeeChoice.Items.Remove(lbcEmployeeChoice.Items[0]);
            lbxEmployeeInclude.SelectedIndex = -1;
            lbcEmployeeChoice.SelectedIndex = -1;

            lblNoOfEmployeesChoice.Text = lbcEmployeeChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfEmployeesInclude.Text = lbxEmployeeInclude.Items.Count.ToString() + " no. of item(s)";
        }
    }

    protected void btnEmployeeRemoveAll_Click(object sender, EventArgs e)
    {
        while (lbxEmployeeInclude.Items.Count > 0)
        {
            lbcEmployeeChoice.Items.Add(lbxEmployeeInclude.Items[0]);
            lbxEmployeeInclude.Items.Remove(lbxEmployeeInclude.Items[0]);
            lbxEmployeeInclude.SelectedIndex = -1;
            lbcEmployeeChoice.SelectedIndex = -1;

            lblNoOfEmployeesChoice.Text = lbcEmployeeChoice.Items.Count.ToString() + " no. of item(s)";
            lblNoOfEmployeesInclude.Text = lbxEmployeeInclude.Items.Count.ToString() + " no. of item(s)";
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        GeneralBL GNBL = new GeneralBL();
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                string sql = string.Empty;
                string errMsg = checkEntry1();

                if (errMsg.Equals(string.Empty))
                {
                   
                    string whereTransaction = BuildTextForTrans();

                    int cnt = 0;
                   
                    for (int i = 0; i < cbxTransactions.Items.Count; i++)
                    {
                        if (cbxTransactions.Items[i].Selected == true)
                        {
                            string lastSeqNo = string.Empty;
                            if (rblBound.SelectedValue == "C")
                            {
                                DataTable dt = getEmployeeID(cbxTransactions.Items[i].Value);
                                for(int j = 0; j < dt.Rows.Count; j++)
                                {
//                                    lastSeqNo = GNBL.GetLastSeqNoInTrail(dt.Rows[j]["Arm_EmployeeID"].ToString(), cbxTransactions.Items[i].Value, dal);


//                                    sql += string.Format(@"
//                                INSERT INTO T_EmployeeApprovalRouteTrail
//                                SELECT Arm_EmployeeID
//                                        , Arm_TransactionID
//                                    --  , Arm_RouteId
//	                                    , '{0}'
//                                        , Arm_NotifyEndorse
//                                        , Arm_NotifyApprove
//                                        , Arm_NotifyReturn
//                                        , Arm_NotifyDisapprove
//                                        , '{2}' 
//                                        , Arm_Status
//                                        , '{1}'
//                                        , GETDATE()
//                                    FROM T_EmployeeApprovalRoute
//                                    WHERE Arm_EmployeeID ='{3}' ", txtToRoute.Text, Session["userLogged"].ToString(), lastSeqNo, dt.Rows[j]["Arm_EmployeeID"].ToString());




//                                    if (rblBound.SelectedValue == "C")
//                                    {
//                                        sql += @"   INNER JOIN T_EmployeeMaster
//                                    ON Emt_EmployeeId = Arm_EmployeeId 
//                                    WHERE Emt_CostCenterCode IN " + BuildTextForCC();
//                                    }

                                    if (txtFromRoute.Text != "")
                                        sql += @" AND Arm_RouteID = '" + txtFromRoute.Text + "'";

                                    sql += string.Format(@" AND Arm_RouteId <> '{0}'
                                        -- AND Arm_TransactionId IN {1}
                                        AND Arm_TransactionId = '{1}'"
                                            , txtToRoute.Text, cbxTransactions.Items[i].Value);
                                }
                            }
                            else if (rblBound.SelectedValue == "E")
                            {
                                for (int j = 0; j < lbxEmployeeInclude.Items.Count; j++)
                                {
                                    //lastSeqNo = GNBL.GetLastSeqNoInTrail(lbxEmployeeInclude.Items[j].Value, cbxTransactions.Items[i].Value, dal);
                                    #region Inserting to T_EmployeeApprovalRouteTrail Table
//                                    sql += string.Format(@"
//                                    INSERT INTO T_EmployeeApprovalRouteTrail
//                                    SELECT Arm_EmployeeID
//                                            , Arm_TransactionID
//                                        --  , Arm_RouteId
//	                                        , '{0}'
//                                            , Arm_NotifyEndorse
//                                            , Arm_NotifyApprove
//                                            , Arm_NotifyReturn
//                                            , Arm_NotifyDisapprove
//                                            , '{2}' 
//                                            , Arm_Status
//                                            , '{1}'
//                                            , GETDATE()
//                                        FROM T_EmployeeApprovalRoute", txtToRoute.Text, Session["userLogged"].ToString(), lastSeqNo);




//                                    if (rblBound.SelectedValue == "C")
//                                    {
//                                        sql += @"   INNER JOIN T_EmployeeMaster
//                                                    ON Emt_EmployeeId = Arm_EmployeeId 
//                                                    WHERE Emt_CostCenterCode IN " + BuildTextForCC();
//                                    }
                                    //else 
                                    if (rblBound.SelectedValue == "E")
                                    {
                                        //sql += @" WHERE Arm_EmployeeID IN " + BuildTextForEmp();
                                        sql += string.Format(@" WHERE Arm_EmployeeID ='{0}' ", lbxEmployeeInclude.Items[j].Value);
                                    }

                                    if (txtFromRoute.Text != "")
                                        sql += @" AND Arm_RouteID = '" + txtFromRoute.Text + "'";

                                    sql += string.Format(@" AND Arm_RouteId <> '{0}'
                                                        -- AND Arm_TransactionId IN {1}
                                                            AND Arm_TransactionId = '{1}'"
                                            , txtToRoute.Text, cbxTransactions.Items[i].Value);
                                }
                            }
                        }
                    }
                    #endregion


                    sql += @"

                    UPDATE T_EmployeeApprovalRoute
                       SET Arm_RouteId = '" + txtToRoute.Text.ToUpper() + @"'
                         , Usr_Login = '" + Session["userLogged"].ToString() + @"'
                         , Ludatetime = getdate() 
                         FROM T_EmployeeApprovalRoute";

                    if (rblBound.SelectedValue == "C")
                    {
                        sql += @"   INNER JOIN T_EmployeeMaster
                            ON Emt_EmployeeId = Arm_EmployeeId 
                                AND Emt_JobStatus <> 'IN'
                            WHERE Emt_CostCenterCode IN " + BuildTextForCC();
                    }
                    else if (rblBound.SelectedValue == "E")
                    {
                        sql += @" WHERE Arm_EmployeeID IN " + BuildTextForEmp();
                    }

                    if (txtFromRoute.Text != "")
                        sql += @" AND Arm_RouteID = '" + txtFromRoute.Text + "'";

                    sql += string.Format(@" AND Arm_RouteId <> '{0}'
                              AND Arm_TransactionId IN {1}", txtToRoute.Text, whereTransaction);

                    if (txtFromRoute.Text == "")    // no insertion in the employeeapprovalroute when from route is not blank
                    {
                        foreach (ListItem transaction in cbxTransactions.Items)
                        {
                            if (transaction.Selected)
                            {
                                if (rblBound.SelectedValue == "C")
                                {
                                    sql += string.Format(@"

                                INSERT T_EmployeeApprovalRoute
                                SELECT 
                                    Emt_EmployeeID
                                    , '{0}'
                                    , '{1}'
                                    , NULL
                                    , NULL
                                    , NULL
                                    , NULL
                                    , 'A'
                                    , '{2}'
                                    , GETDATE()  
                                from T_EmployeeMaster where Emt_CostCenterCode IN {3}
	                                and Emt_JobStatus <> 'IN'
	                                and Emt_EmployeeID NOT IN 
	                                (select Arm_EmployeeId from T_EmployeeApprovalRoute EAR
		                                join T_EmployeeMaster EMT
			                                ON Arm_EmployeeId = Emt_EmployeeID 	
		                                where Arm_TransactionID = '{0}' AND Arm_Status = 'A'
		                                and Emt_CostCenterCode IN {3})"
                                        , transaction.Value, txtToRoute.Text, Session["userLogged"].ToString(), BuildTextForCC());

                                }
                                else if (rblBound.SelectedValue == "E")
                                {
                                    sql += string.Format(@"

                                INSERT T_EmployeeApprovalRoute
                                SELECT 
                                    Emt_EmployeeID
                                    , '{0}'
                                    , '{1}'
                                    , NULL
                                    , NULL
                                    , NULL
                                    , NULL
                                    , 'A'
                                    , '{2}'
                                    , GETDATE()  
                                from T_EmployeeMaster where Emt_EmployeeID IN {3}
	                                and Emt_JobStatus <> 'IN'
	                                and Emt_EmployeeID NOT IN 
	                                (select Arm_EmployeeId from T_EmployeeApprovalRoute EAR
		                                join T_EmployeeMaster EMT
			                                ON Arm_EmployeeId = Emt_EmployeeID 	
		                                where Arm_TransactionID = '{0}' AND Arm_Status = 'A'
		                                and Emt_EmployeeID IN {3})"
                                    , transaction.Value, txtToRoute.Text, Session["userLogged"].ToString(), BuildTextForEmp());

                                }

                            }

                        }
                    }

                    MessageBox.Show(GeneralBL.UpdateRoutes(sql));
                    restoreDefaultControls();
                }
                else
                {
                    MessageBox.Show(errMsg);
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    #endregion

    private string checkEntry1()
    {
        string retFlag = string.Empty;
        if (txtFromRoute.Text.ToUpper() == txtToRoute.Text.ToUpper())
        {
            retFlag += "Converting to the same route is invalid.";
        }
        //temp
        //if (!RouteRevisionBL.isValidRoute(txtFromRoute.Text))
        //{
        //    retFlag += "Route ID in FROM entry in invalid \n";
        //}
        //temp
        //if (!RouteRevisionBL.isValidRoute(txtToRoute.Text))
        //{
        //    retFlag += "Route ID in TO entry in invalid \n";
        //}
        if (cbxTransactions.SelectedIndex < 0)
        {
            retFlag += "Select at least one transaction. \n";
        }
        if (rblBound.SelectedValue == "C" && lbxCostcenterInclude.Items.Count < 1)
        {
            retFlag += "Select at least one costcenter. \n";
        }
        else if (rblBound.SelectedValue == "E" && lbxEmployeeInclude.Items.Count < 1)
        {
            retFlag += "Select at least one employee. \n";
        }
        return retFlag;
    }

    protected void FillEmployeeList()
    {
        lbcEmployeeChoice.Items.Clear();

        string sql = string.Empty;
        
        if (txtFromRoute.Text == "")
        {
            sql = @" SELECT Emt_EmployeeId
                             , Emt_EmployeeId + ' - ' + dbo.GetControlEmployeeName(Emt_EmployeeID) 
                             , dbo.GetControlEmployeeName(Emt_EmployeeID) AS [ForSort]
                          FROM T_EmployeeMaster
                         WHERE LEFT(Emt_JobStatus,1) = 'A'";
        }
        else
        {
            sql = @"SELECT DISTINCT Emt_EmployeeId
                             , Emt_EmployeeId + ' - ' + dbo.GetControlEmployeeName(Emt_EmployeeID) 
                             , dbo.GetControlEmployeeName(Emt_EmployeeID) AS [ForSort]
                          FROM T_EmployeeApprovalRoute 
                          JOIN T_EmployeeMaster 
                            ON Arm_EmployeeId = Emt_EmployeeId
                         WHERE LEFT(Emt_JobStatus,1) = 'A' AND Arm_RouteID = '" + txtFromRoute.Text + "'";

            
            sql += " AND Arm_TransactionID IN (";
            foreach (ListItem checkbox in cbxTransactions.Items)
            {
                if (checkbox.Selected)
                {
                    sql += "'" + checkbox.Value + "', ";
                }
            }
            sql = sql.Substring(0, sql.Length - 2) + ") ";
            
        }

        if (lbxEmployeeInclude.Items.Count > 0)
        {
            sql += " AND Emt_EmployeeId NOT IN " + BuildTextForEmp();
        }

        if (!txtSearchEmployee.Text.Trim().Equals(string.Empty))
        {
            sql += string.Format(@" AND( Emt_EmployeeID LIKE '{0}%'
                                      OR Emt_LastName LIKE '{0}%'
                                      OR Emt_FirstName LIKE '{0}%'
                                      --OR Dcm_DepartmentDesc LIKE '%{0}%'
                                      OR Emt_NickName LIKE '{0}%')", txtSearchEmployee.Text);
        }
        sql += "ORDER BY [ForSort]";
        FillList(sql, lbcEmployeeChoice);
        txtSearchEmployee.Focus();

        lblNoOfEmployeesChoice.Text = lbcEmployeeChoice.Items.Count.ToString() + " no. of item(s)";
        lblNoOfEmployeesInclude.Text = lbxEmployeeInclude.Items.Count.ToString() + " no. of item(s)";
    }

    protected void FillCostCenterList()
    {
        lbcCostcenterChoice.Items.Clear();

        string sql = string.Empty;
        if (txtFromRoute.Text == "")
        {
            sql = @" SELECT  Cct_CostCenterCode as [Costcenter Code]
	                            , RTRIM(Cct_CostCenterCode) + ' - ' + dbo.getCostCenterName(Cct_CostCenterCode) [Costcenter Description]
	                    FROM T_CostCenter
                        LEFT JOIN T_DivisionCodeMaster 
		                    ON Cct_DivisionCode = Dcm_DivisionCode
	                    LEFT JOIN T_DepartmentCodeMaster
		                    ON Cct_Departmentcode = Dcm_Departmentcode
                        LEFT JOIN T_SectionCodeMaster 
		                    ON Cct_Sectioncode = Scm_Sectioncode 
                        WHERE Cct_Status = 'A' 
		                    --And Cct_CostCenterCode in (Select distinct(Emt_CostcenterCode) From T_EmployeeMaster Where Emt_JobStatus <> 'IN')
                        ";
        }
        else
        {
            sql = @" SELECT  DISTINCT Emt_CostCenterCode [Costcenter Code]
	                            , RTRIM(Emt_CostCenterCode) + ' - ' + dbo.getCostCenterName(Emt_CostCenterCode) [Costcenter Description]
	                    FROM T_EmployeeApprovalRoute
                        JOIN T_EmployeeMaster 
		                    ON Arm_EmployeeId = Emt_EmployeeID 
                        JOIN T_CostCenter 
                            ON Emt_CostCenterCode = Cct_CostCenterCode
                        LEFT JOIN T_DivisionCodeMaster 
		                    ON Cct_DivisionCode = Dcm_DivisionCode
	                    LEFT JOIN T_DepartmentCodeMaster
		                    ON Cct_Departmentcode = Dcm_Departmentcode
                        LEFT JOIN T_SectionCodeMaster 
		                    ON Cct_Sectioncode = Scm_Sectioncode 
                        WHERE LEFT(Emt_JobStatus,1) = 'A' AND Arm_RouteID = '" + txtFromRoute.Text + "'";

            sql += " AND Arm_TransactionID IN (";
            foreach (ListItem checkbox in cbxTransactions.Items)
            {
                if (checkbox.Selected)
                {
                    sql += "'" + checkbox.Value + "', ";
                }
            }
            sql = sql.Substring(0, sql.Length - 2) + ") ";
        }

        if (lbxCostcenterInclude.Items.Count > 0)
        {
            sql += "AND Cct_CostCenterCode NOT IN " + BuildTextForCC();
        }

        if (!txtSearchCostcenter.Text.Trim().Equals(string.Empty))
        {
            sql += string.Format(@" AND ( Cct_CostCenterCode = '{0}'
                                  OR dbo.getCostCenterName(Cct_CostCenterCode) LIKE '%{0}%') ", txtSearchCostcenter.Text);

//            sql += string.Format(@" AND ( Cct_CostCenterCode = '{0}'
//                                  OR Dcm_DivisionDesc LIKE '%{0}%'
//                                  OR Dcm_DepartmentDesc LIKE '%{0}%'
//                                  OR Scm_Sectiondesc LIKE '%{0}%') ", txtSearchCostcenter.Text);
        }

        sql += "ORDER BY [Costcenter Code]";
        FillList(sql, lbcCostcenterChoice);
        txtSearchCostcenter.Focus();

        lblNoOfCostcentersChoice.Text = lbcCostcenterChoice.Items.Count.ToString() + " no. of item(s)";
        lblNoOfCostcentersInclude.Text = lbxCostcenterInclude.Items.Count.ToString() + " no. of item(s)";
    }

    protected string BuildTextForEmp()
    {
        string temp = @"(";

        for (int i = 0; i < lbxEmployeeInclude.Items.Count; i++)
        {
            if (i != 0 && !temp.Equals(string.Empty))
            {
                temp += ",";
            }
            temp += "'" + lbxEmployeeInclude.Items[i].Value + "'";
        }
        temp += ")";

        return temp.Equals("()") ? string.Empty : temp;
    }

    protected string BuildTextForCC()
    {
        string temp = @"(";

        for (int i = 0; i < lbxCostcenterInclude.Items.Count; i++)
        {
            if (i != 0 && !temp.Equals(string.Empty))
            {
                temp += ",";
            }
            temp += "'" + lbxCostcenterInclude.Items[i].Value + "'";
        }
        temp += ")";

        return temp.Equals("()") ? string.Empty : temp;
    }

    protected string BuildTextForTrans()
    {
        string temp = @"(";

        for (int i = 0; i < cbxTransactions.Items.Count; i++)
        {
            if (i != 0 && cbxTransactions.Items[i].Selected && temp.Length > 1)
            {
                temp += ",";
            }
            if (cbxTransactions.Items[i].Selected)
                temp += "'" + cbxTransactions.Items[i].Value + "'";
        }
        temp += ")";

        return temp.Equals("()") ? string.Empty : temp;
    }

    private void initializeControls()
    {
        showOptionalFields();
    }

    private void restoreDefaultControls()
    {
        txtFromRoute.Text = string.Empty;
        txtFromRouteAP.Text = string.Empty;
        txtFromRouteC1.Text = string.Empty;
        txtFromRouteC2.Text = string.Empty;
        txtToRoute.Text = string.Empty;
        txtToRouteAP.Text = string.Empty;
        txtToRouteC1.Text = string.Empty;
        txtToRouteC2.Text = string.Empty;
        cbxTransactions.ClearSelection();
        rblBound.ClearSelection();
        lbcCostcenterChoice.Items.Clear();
        lbcEmployeeChoice.Items.Clear();
        lbxCostcenterInclude.Items.Clear();
        lbxEmployeeInclude.Items.Clear();
        mtvBound.ActiveViewIndex = -1;
    }

    private void showOptionalFields()
    {
        if (Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME))
        {
            cbxTransactions.Items.Add(new ListItem("Overtime", "OVERTIME"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE))
        {
            cbxTransactions.Items.Add(new ListItem("Leave", "LEAVE"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION))
        {
            cbxTransactions.Items.Add(new ListItem("Time Record", "TIMEMOD"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWFLEXTIME))
        {
            cbxTransactions.Items.Add(new ListItem("Flex Time", "FLEXTIME"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWJOBSPLIT))
        {
            cbxTransactions.Items.Add(new ListItem("Man Hour", "JOBMOD"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION))
        {
            cbxTransactions.Items.Add(new ListItem("Work Info", "MOVEMENT"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWBENEFICIARY))
        {
            cbxTransactions.Items.Add(new ListItem("Beneficiary", "BNEFICIARY"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWTAXCIVIL))
        {
            cbxTransactions.Items.Add(new ListItem("Tax/Civil Status", "TAXMVMNT"));
        }
        if (Convert.ToBoolean(Resources.Resource.AFSHOWADDRESS))
        {
            cbxTransactions.Items.Add(new ListItem("Address", "ADDRESS"));
        }
    }

    private void FillList(string sqlCode_Desc, ListBox lbxObj)
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlCode_Desc, CommandType.Text);
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
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                try
                {
                    lbxObj.Items.Add(new ListItem(Convert.ToString(ds.Tables[0].Rows[i][1]), Convert.ToString(ds.Tables[0].Rows[i][0])));
                }
                catch
                {
                    //Do nothing loop will continue.
                }
            }
        }
    }
    private DataTable getEmployeeID(string transactionId)
    {
        DataTable dt = new DataTable();
        string query = string.Format(@"SELECT DISTINCT Arm_EmployeeID
                        FROM T_EmployeeApprovalRoute
                        INNER JOIN T_EmployeeMaster
                        ON Emt_EmployeeId = Arm_EmployeeId 
                        WHERE Emt_CostCenterCode IN {0}
                        AND Arm_TransactionID = '{1}'" , BuildTextForCC()
                              , transactionId);
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dt = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }
        return dt;
    }
}