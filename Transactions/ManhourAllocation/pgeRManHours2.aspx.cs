using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using MethodsLibrary;

public partial class Default2 : System.Web.UI.Page
{
    private static DataSet dsView = null;
    static string lastVal = "", lastDate = "";
    static string userCostCenters = "";
    int innerHeight = 0;
    int outerHeight = 0;
    private decimal grandTotal = 0;
    private bool print = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        MenuGrant MG = new MenuGrant();
        if (!MG.getAccessRights(Session["userLogged"].ToString(), "WFMANHOURREP"))
        {
            Response.Redirect("index.aspx");
        }

        for (int i = 0; i < rblOption.Items.Count; i++)
            rblOption.Items[i].Attributes.Add("oncLick", "return CheckPopUp();");
        
        getWeedStartandEnd();
        if (!Page.IsPostBack)
        {
            DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFMANHOURREP");
            if (dr != null)
            {
                bool canCheck = Convert.ToBoolean(dr["Ugt_CanCheck"]);
                bool canApprove = Convert.ToBoolean(dr["Ugt_CanApprove"]);
                
                btnExport.Visible = Convert.ToBoolean(dr["Ugt_CanGenerate"]);
                btnPrint.Visible = Convert.ToBoolean(dr["Ugt_CanPrint"]);
            }

            //LoadComplete += new EventHandler(_Default_LoadComplete);
            rbEmployeeOptions.SelectedIndex = 0;
            rblCode.SelectedIndex = 0;
            lastVal = rblOption.SelectedValue = "E";
            rblBillable.SelectedValue = "A";
            ddlDateType.SelectedValue = "D";
            ddlDateType.Enabled = false;
            this.NumberOfControls = 0;
            ddlBilling.Enabled = false;
            dsView = null;
            InitializeButtons();
            PopulateDdlBilling();
        }
        else
        {
            if (dsView != null)
            {
                //GenerateTables();
            }
        }
        EnableDisableControls(null, null);
        LoadComplete += new EventHandler(_Default_LoadComplete);
    }

    #region LoadComplete
    protected void _Default_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "manhourscripts";
        string jsurl = "_manhour.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;

        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        Calendar cal1 = (Calendar)dtpRangeFrom.Controls[3];
        cal1.Attributes.Add("OnClick", "javascript:return checkRange('from')");

        Calendar cal2 = (Calendar)dtpRangeTo.Controls[3];
        cal2.Attributes.Add("OnClick", "javascript:return checkRange('to')");

        ddlDateType.Attributes.Add("OnChange", "return CheckPopUp();");

        ddlBilling.Attributes.Add("OnChange", "javascript:return checkRange('selection')");

        chkDefaultReport.Attributes.Add("OnClick", "javascript:checkDefaultReport();");
        chkSubWorkCode.Attributes.Add("OnClick", "javascript:CheckWorkCode();");
    }
    #endregion

    private void InitializeButtons()
    {
        userCostCenters = "";
        DataTable dt = CommonLookUp.GetUserCostCenterCode(Session["userId"].ToString(), "TIMEKEEP");
        if (dt != null)
        {
            foreach (DataRow dr in dt.Rows)
            {
                userCostCenters += string.Format("x{0}xy", dr[0].ToString());
            }
            btnDashJobCode.OnClientClick = string.Format("return OpenPopupLookupSales('S1','txtDashJobCode','{0}'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnClientJobNo.OnClientClick = string.Format("return OpenPopupLookupSales('S2','txtClientJobNo','{0}'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnCLientCode.OnClientClick = string.Format("return OpenPopupLookupSales('S3','txtClientCode','{0}'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnClientFWBS.OnClientClick = string.Format("return OpenPopupLookupSales('S4','txtFWBSCode','{0}'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnDashWorkCode.OnClientClick = string.Format("return OpenPopupLookupSales('S6','txtWorkCode','{0}'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnCostCenter.OnClientClick = string.Format("javascript:return lookupRJSCostcenter()");
            //btnCostCenter.OnClientClick = string.Format("return OpenPopupLookupCostCenter('T_JobSplitHeader','Jsh_CostCenter','T_JobSplitHeader','{0}','txtCostCenter','Cost Center Lookup'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            //btnSubWork.OnClientClick = string.Format("return OpenPopupLookupSubWorkCode('{0}','txtSubWorkCode'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnSubWork.OnClientClick = string.Format("javascript:return lookupRJSSubwork()");
        }
        else
            userCostCenters = "xNo Cost Center Accessxy";
    }

    private DataTable VSDataTable
    {
        get
        {
            return (DataTable)ViewState["VSDataTable"];
        }
        set
        {
            ViewState["VSDataTable"] = value;
        }
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        Button button;
        if (sender.GetType().Name.Equals("Button"))
        {
            button = (Button)sender;
            if (button.ID.Equals("btnTwist"))
                hfTwist.Value = (!Convert.ToBoolean(hfTwist.Value)).ToString();
        }

        setOptions();

        lastVal = rblOption.SelectedValue.ToString();
        lastDate = ddlDateType.SelectedValue.ToString();
        Panel1.Attributes["Height"] = "Inherit";
        GetData();
        if (dsView != null && dsView.Tables.Count > 0)
        {
            innerHeight = 0;
            outerHeight = 0;
            GenerateTables();
            this.VSDataTable = dsView.Tables[0];
        }

        else
        {
            Panel1.Controls.Clear();
            dsView = null;
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        dsView = null;
        Panel1.Controls.Clear();
        txtDashJobCode.Text = "";
        txtClientCode.Text = "";
        txtClientJobNo.Text = "";
        txtFWBSCode.Text = "";
        txtCostCenter.Text = "";
        txtWorkCode.Text = "";
        txtSubWorkCode.Text = "";
        rblOption.SelectedValue = "E";
        rblBillable.SelectedValue = "A";
        rbEmployeeOptions.SelectedValue = "S";
        cbUnsplitted.Checked = false;
        chkDefaultReport.Checked = true;
        chkSubWorkCode.Checked = true;
        ddlDateType.SelectedValue = "D";
        ddlBilling.SelectedValue = "";
        dtpRangeFrom.Reset();
        dtpRangeTo.Reset();
        this.NumberOfControls = 0;
        EnableDisableControls(null, null);
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (Panel1.Controls.Count > 0)
        {
            btnGenerate_Click(null,null);
            try
            {
                Control[] ctrl = new Control[this.NumberOfControls + 1];

                string reportTitle = "";

                if (chkDefaultReport.Checked)
                    reportTitle = string.Format("Man Hour Report Per {0}", rblOption.SelectedItem.Text.Trim());
                else
                {
                    string reportDateRange = "";
                    if(!dtpRangeFrom.DateString.Equals(string.Empty))
                        reportDateRange += " from " + dtpRangeFrom.DateString;
                    if(!dtpRangeTo.DateString.Equals(string.Empty))
                        reportDateRange += " to " + dtpRangeTo.DateString;

                    if (rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D" && ddlDateType.SelectedValue == "M")
                        reportTitle = "Manhour Monthly Summary" + reportDateRange;
                    else
                        reportTitle = "Department Manhour Report" + reportDateRange;
                }

                ctrl[0] = CommonLookUp.HeaderPanelOption(this.VSDataTable.Columns.Count, reportTitle, initializeHeader());

                int ctr = 1;
                foreach (Control panelctrl in Panel1.Controls)
                {
                    if (panelctrl is Label)
                        ctrl[ctr++] = panelctrl;
                    else if (panelctrl is GridView)
                        ctrl[ctr++] = panelctrl;
                    if (panelctrl.HasControls())
                        foreach (Control child in panelctrl.Controls)
                        {
                            if (child is Label)
                                ctrl[ctr++] = child;
                            else if (child is GridView)
                                ctrl[ctr++] = child;
                            if (child.HasControls())
                            {
                                foreach (Control child2 in child.Controls)
                                {
                                    if (child2 is Label)
                                        ctrl[ctr++] = child2;
                                    else if (child2 is GridView)
                                        ctrl[ctr++] = child2;
                                }
                            }
                        }
                }

                //April 12,2011 - added by Kelvin 
                if(chkDefaultReport.Checked)
                    ExportExcelHelper.ExportControl2(ctrl, string.Format("Manhours Report Per {0}", rblOption.SelectedItem.Text.Trim()));
                else
                    ExportExcelHelper.ExportControl2(ctrl, "Department Manhour Report");
            }
            catch
            {
                MessageBox.Show("Some error occurred during exporting to Excel.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        print = true;

        if (this.Panel1.Controls.Count > 1)
        {
            btnGenerate_Click(sender, e);
            try
            {
                Control[] ctrl;
                if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")
                    ctrl = new Control[this.NumberOfControls + 2];
                else
                    ctrl = new Control[this.NumberOfControls + 1];

                if(chkDefaultReport.Checked)
                    ctrl[0] = CommonLookUp.HeaderPanelOption(this.VSDataTable.Columns.Count, string.Format("Manhours Report Per {0}", rblOption.SelectedItem.Text.Trim()), initializeHeader());
                else
                    ctrl[0] = CommonLookUp.HeaderPanelOption(this.VSDataTable.Columns.Count, "Department Manhour Report", initializeHeader());
                
                int ctr = 1;
                foreach (Control panelctrl in Panel1.Controls)
                {
                    if (panelctrl is Label)
                        ctrl[ctr++] = panelctrl;
                    else if (panelctrl is GridView)
                        ctrl[ctr++] = panelctrl;
                    if (panelctrl.HasControls())
                        foreach (Control child in panelctrl.Controls)
                        {
                            if (child is Label)
                            {
                                //((Label)child).Font.Size = 8;
                                ctrl[ctr++] = child;
                            }
                            else if (child is GridView)
                            {
                                ((GridView)child).UseAccessibleHeader = true;
                                ((GridView)child).HeaderRow.TableSection = TableRowSection.TableHeader;
                                ctrl[ctr++] = child;
                            }
                            if (child.HasControls())
                            {
                                foreach (Control child2 in child.Controls)
                                {
                                    if (child2 is Label)
                                        ctrl[ctr++] = child2;
                                    else if (child2 is GridView)
                                    {
                                        ((GridView)child2).UseAccessibleHeader = true;
                                        ((GridView)child2).HeaderRow.TableSection = TableRowSection.TableHeader;
                                        ctrl[ctr++] = child2;
                                    }
                                }
                            }
                        }
                }
                Session["ctrl"] = ctrl;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
            }
            catch
            {
                MessageBox.Show("Some error occurred during initialization of page for printing.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }

    private string initializeHeader()
    {
        string options = "";
        if (userCostCenters != "")
            options += "Cost Center Access: " + userCostCenters.Substring(0, userCostCenters.Length - 1).Replace("x", "'").Replace("y", ",") + "; ";
        if (txtDashJobCode.Text != "")
            options += "CPH Job No.(s): " + txtDashJobCode.Text.Trim() + "; ";
        if (txtClientJobNo.Text != "")
            options += "Client Job Code(s): " + txtClientJobNo.Text.Trim() + "; ";
        if (txtClientCode.Text != "")
            options += "Client Code(s): " + txtClientCode.Text.Trim() + "; ";
        if (txtFWBSCode.Text != "")
            options += "FWBS Code(s): " + txtFWBSCode.Text.Trim() + "; ";
        if (txtCostCenter.Text != "")
            options += "Cost Center(s): " + txtCostCenter.Text.Trim() + "; ";
        if (rblBillable.SelectedValue != "A")
            options += "Category: " + rblBillable.SelectedItem.Text.Trim() + "; ";

        if (!dtpRangeTo.IsNull && !dtpRangeFrom.IsNull)
            if (dtpRangeTo.Date == dtpRangeFrom.Date)
                options += "Interval Date: " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Interval Date: " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + " - " + dtpRangeTo.Date.ToString("MM/dd/yyyy") + "; ";
        else if (dtpRangeTo.IsNull && !dtpRangeFrom.IsNull)
            options += "Interval Date: From " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + "; ";
        else if (!dtpRangeTo.IsNull && dtpRangeFrom.IsNull)
            options += "Interval Date: To " + dtpRangeTo.Date.ToString("MM/dd/yyyy") + "; ";

        return options.Trim();
    }

    protected void GetData()
    {
        string sqlFetch = SqlBuilder();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsView = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
                dsView = null;
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }

    private string SetQuery()
    {
        string sqlQuery = "";
       
        if (rblOption.SelectedValue == "E")
        {
            if (rbEmployeeOptions.SelectedValue == "S")
            {
                sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = 'SELECT Ecc_Description [CostCenterName]
		                                    ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                , Jsd_ActHours
                                FROM T_JobSplitdetail
                                INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                                and  Jsh_Status = ''9''
                                INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  
                                WHERE Jsd_Status = ''9''";
            }
            else
            {
                if (chkSubWorkCode.Checked)
                    sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = 'SELECT Ecc_Description [CostCenterName]
		                                    ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                , Slm_ClientJobName [Client Job Name]
		                                , Slm_DashJobCode [CPH Job No.]
		                                
		                                , Jsd_SubWorkCode [Work Activity Code]
		                                , Jsd_ActHours
                                FROM T_JobSplitdetail
                                INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                                and  Jsh_Status = ''9''
                                INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  
                                WHERE Jsd_Status = ''9''";
                else
                    sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = 'SELECT Ecc_Description [CostCenterName]
		                                    ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                , Slm_ClientJobName [Client Job Name]
		                                , Slm_DashJobCode [CPH Job No.]
		                                
		                                , Jsd_ActHours
                                FROM T_JobSplitdetail
                                INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                                and  Jsh_Status = ''9''
                                INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  --?
                                WHERE Jsd_Status = ''9''";
            }
            sqlQuery = string.Format(sqlQuery, ConfigurationManager.AppSettings["ERP_DB"]);
        }
        else if (rblOption.SelectedValue == "D")
        {
            sqlQuery = @"   declare @SelectList varchar(max)
                        declare @PivotCol varchar(200)
                        declare @Summaries varchar(200)

                        SET @SelectList = 'SELECT 
	                                Jsd_Jobcode [CPH Job No.]
	                            , Slm_ClientJobNo [Client Job No]
	                            , Slm_ClientJobName [Client Job Name]";

            if (chkSubWorkCode.Checked)
                sqlQuery += @"      , Jsd_SubWorkCode [Work Activity Code]";

            sqlQuery += @"          , Jsd_ActHours
                        FROM T_JobSplitdetail
                        INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
                            and  Jsh_Status = ''9''
                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                            and Jsd_ClientJobNo = Slm_ClientJobNo
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        WHERE Jsd_Status = ''9''";

        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
        {
            sqlQuery = @"declare @SelectList varchar(max)
                    declare @PivotCol varchar(200)
                    declare @Summaries varchar(100)

                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
	                            {0}
                                , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
                        and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                        and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                
                                , Jsd_Jobcode [CPH Job No.]";
                if (chkSubWorkCode.Checked)
                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                , Slm_ClientJobName [Client Job Name]
//                                , Slm_ClientJobNo [Client Job No]
//                                , Jsd_Jobcode [CPH Job No.]";
//                if (chkSubWorkCode.Checked)
//                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
                sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
        {
            sqlQuery = @"declare @SelectList varchar(max)
                    declare @PivotCol varchar(300)
                    declare @Summaries varchar(max)
                    declare @StartDay tinyint
                    set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                    set Datefirst @StartDay
                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                    {0}
		                        , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                    and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                
                                , Jsd_Jobcode [CPH Job No.]";
                if (chkSubWorkCode.Checked)
                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                , Slm_ClientJobName [Client Job Name]
//                                , Slm_ClientJobNo [Client Job No]
//                                , Jsd_Jobcode [CPH Job No.]";
//                if (chkSubWorkCode.Checked)
//                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
        {
            sqlQuery = @"
                    declare @SelectList varchar(max)
                    declare @PivotCol varchar(300)
                    declare @Summaries varchar(max)
                    declare @StartDay tinyint
                    set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                    set Datefirst @StartDay
                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                    {0}
		                    , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                    and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                
                                , Jsd_Jobcode [CPH Job No.]";
                if (chkSubWorkCode.Checked)
                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                , Slm_ClientJobName [Client Job Name]
//                                , Slm_ClientJobNo [Client Job No]
//                                , Jsd_Jobcode [CPH Job No.]";
//                if (chkSubWorkCode.Checked)
//                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
//            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
       
            
        
        return sqlQuery;
    }

    private string SetQueryLossTime()
    {
        string sqlQuery = "";
        if (rblOption.SelectedValue == "E")
        {
            if (rbEmployeeOptions.SelectedValue == "S")
            {
                sqlQuery = @"SELECT dbo.getCostCenterFullName(Jsh_Costcenter) [CostCenterName]
		                                     ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                    , Jsd_ActHours
                                    FROM T_JobSplitDetailLossTime
                                    INNER JOIN T_JobSplitHeaderLossTIme on Jsd_ControlNo = Jsh_ControlNo
	                                    and  Jsh_Status = ''9''
                                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                    and Jsd_ClientJobNo = Slm_ClientJobNo
                                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    WHERE Jsd_Status = ''9''";
            }
            else if (rbEmployeeOptions.SelectedValue == "D")
            {
                if (chkSubWorkCode.Checked)
                    sqlQuery = @"SELECT dbo.getCostCenterFullName(Jsh_Costcenter) [CostCenterName]
		                                         ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                        , Slm_ClientJobName
		                                        , Slm_DashJobCode
		                                        
                                                , Jsd_SubWorkCode
                                                , Jsd_ActHours
                                        FROM T_JobSplitdetaillosstime
                                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                                        and  Jsh_Status = ''9''
                                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        WHERE Jsd_Status = ''9''";
                else
                    sqlQuery = @"SELECT dbo.getCostCenterFullName(Jsh_Costcenter) [CostCenterName]
		                                     ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                    , Slm_ClientJobName
		                                    , Slm_DashJobCode
		                                  
		                                    , Jsd_ActHours
                                        FROM T_JobSplitdetaillosstime
                                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                                        and  Jsh_Status = ''9''
                                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        WHERE Jsd_Status = ''9''";
            }
        }
        else if (rblOption.SelectedValue == "D")
        {
            sqlQuery = @" SELECT 
	                                  Jsd_Jobcode [CPH Job No.]
	                                , Slm_ClientJobNo [Client Job No]
	                                , Slm_ClientJobName [Client Job Name]";
            if(chkSubWorkCode.Checked)
                                 sqlQuery += @",  ''TEMP'' as [Work Activity Code]";
                                    
	        sqlQuery += @"          , Jsd_ActHours
                            FROM T_JobSplitdetaillosstime
                            INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
                                and  Jsh_Status = ''9''
                            INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                                and Jsd_ClientJobNo = Slm_ClientJobNo
                            LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                            INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                            WHERE Jsd_Status = ''9''";

        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
	                             {0}
                                 , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
                            and  Jsh_Status = ''9''
                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                            and Jsd_ClientJobNo = Slm_ClientJobNo
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                 , Slm_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                
                                 , Jsd_Jobcode [CPH Job No.]";
                if(chkSubWorkCode.Checked)
                                 insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                 , Slm_ClientJobName [Client Job Name]
//                                 , Slm_ClientJobNo [Client Job No]
//                                 , Jsd_Jobcode [CPH Job No.]";
//                if (chkSubWorkCode.Checked)
//                    insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                        {0}
		                          , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                        and  Jsh_Status = ''9''
                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                        and Jsd_ClientJobNo = Slm_ClientJobNo
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                 , Slm_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                
                                 , Jsd_Jobcode [CPH Job No.]";
                if(chkSubWorkCode.Checked)
                                 insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                 , Slm_ClientJobName [Client Job Name]
//                                 , Slm_ClientJobNo [Client Job No]
//                                 , Jsd_Jobcode [CPH Job No.]";
//                if(chkSubWorkCode.Checked)
//                                 insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                        {0}
		                        , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                        and  Jsh_Status = ''9''
                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                        and Jsd_ClientJobNo = Slm_ClientJobNo
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsd_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
                insert = @"
                                 , Slm_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                 
                                 , Jsd_Jobcode [CPH Job No.]";
                if(chkSubWorkCode.Checked)
                                 insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
//            else
//            {
//                insert = @"
//                                 , Slm_ClientJobName [Client Job Name]
//                                 , Slm_ClientJobNo [Client Job No]
//                                 , Jsd_Jobcode [CPH Job No.]";
//                if(chkSubWorkCode.Checked)
//                                 insert += @",  ''TEMP'' as [Work Activity Code]";
//            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        return sqlQuery;
    }

    protected string SqlBuilder()
    {
        StringBuilder sql = new StringBuilder();
        StringBuilder filters = new StringBuilder();
        StringBuilder includeCol = new StringBuilder();

        #region Default Report
        if (chkDefaultReport.Checked == true)
        {   
            sql.Append(SetQuery());

            #region Cost Center Access
            if (userCostCenters != "" && !userCostCenters.Contains("ALL"))
            {
                string temp = userCostCenters;
                temp = temp.Replace("x", "''").Replace("y", ",");
                sql.Append("\nAND Jsh_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            }
            #endregion

            #region Textboxes
            if (txtDashJobCode.Text != "")
            {
                string text = txtDashJobCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string dashJobCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(dashJobCode + ")");
                }
            }

            if (txtClientJobNo.Text != "")
            {
                string text = txtClientJobNo.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string clientJobNo = "";
                    for (int i = 0; i < arr.Count; i++)
                        clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(clientJobNo + ")");
                }
            }
            if (txtClientCode.Text != "")
            {
                string text = txtClientCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);

                if (arr.Count > 0)
                {
                    string clientCode = "", clientName = "", clientShortName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
                        clientName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                        clientShortName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                    }
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(clientCode);
                    sql.Append(clientName);
                    sql.Append(clientShortName + ")");
                }
            }

            if (txtCostCenter.Text != "")
            {
                string text = txtCostCenter.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string costCenterCode = "", costCenterName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        costCenterCode += string.Format("\nOR Jsh_Costcenter like ''{0}%''", arr[i].ToString().Trim());
                        costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsh_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
                    }
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(costCenterCode);
                    sql.Append(costCenterName + ")");
                }
            }

            if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
            {
                string text = txtSubWorkCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string SubWorkCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(SubWorkCode + ")");
                }
            }
            #endregion
            
            
            #region Textboxes
            if (txtFWBSCode.Text != "")
            {
                string text = txtFWBSCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string FWBSCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like ''{0}%''", arr[i].ToString().Trim());
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(FWBSCode + ")");
                }
            }

            if (txtWorkCode.Text != "")
            {
                string text = txtWorkCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string dashWorkCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        dashWorkCode += string.Format("\nOR Slm_DashWorkCode like ''{0}%''", arr[i].ToString().Trim());
                    sql.Append("\nAND (");
                    sql.Append("\n 1 = 0");
                    sql.Append(dashWorkCode + ")");
                }
            }

            #endregion

            if (rblBillable.SelectedValue != "A")  
            {
                sql.Append(string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue == "B" ? true : false));
            }

            if (rblOption.SelectedValue == "C")
            {
                if (!ddlBilling.SelectedValue.Equals(string.Empty))
                {
                    sql.Append(string.Format("\nAND Clh_BillingCycle = ''{0}''", ddlBilling.SelectedValue.ToString()));
                }
            }

            #region Date Time Pickers
            if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
            {
                if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                {
                    sql.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                     , Convert.ToDateTime(dtpRangeTo.Date)));
                }
                else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
                {
                    sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date)));
                }
                else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                {
                    sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date)));
                }
            }
            #endregion

            if (cbUnsplitted.Checked)
            {
                #region UNION for JobSplitDetaillosstime

                sql.Append(@"
                UNION ALL 
                ");
                sql.Append(SetQueryLossTime());

                #region Cost Center Access
                if (userCostCenters != "" && !userCostCenters.Contains("ALL"))
                {
                    string temp = userCostCenters;
                    temp = temp.Replace("x", "''").Replace("y", ",");
                    sql.Append("\nAND Jsh_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
                }
                #endregion

                #region Textboxes
                if (txtDashJobCode.Text != "")
                {
                    string text = txtDashJobCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string dashJobCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(dashJobCode + ")");
                    }
                }

                if (txtClientJobNo.Text != "")
                {
                    string text = txtClientJobNo.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string clientJobNo = "";
                        for (int i = 0; i < arr.Count; i++)
                            clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(clientJobNo + ")");
                    }
                }

                if (txtClientCode.Text != "")
                {
                    string text = txtClientCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);

                    if (arr.Count > 0)
                    {
                        string clientCode = "", clientName = "", clientShortName = "";
                        for (int i = 0; i < arr.Count; i++)
                        {
                            clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
                            clientName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                            clientShortName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                        }
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(clientCode);
                        sql.Append(clientName);
                        sql.Append(clientShortName + ")");
                    }
                }

                if (txtFWBSCode.Text != "")
                {
                    string text = txtFWBSCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string FWBSCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(FWBSCode + ")");
                    }
                }

                if (txtWorkCode.Text != "")
                {
                    string text = txtWorkCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string dashWorkCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            dashWorkCode += string.Format("\nOR Slm_DashWorkCode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(dashWorkCode + ")");
                    }
                }

                if (txtCostCenter.Text != "")
                {
                    string text = txtCostCenter.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string costCenterCode = "", costCenterName = "";
                        for (int i = 0; i < arr.Count; i++)
                        {
                            costCenterCode += string.Format("\nOR Jsh_Costcenter like ''{0}%''", arr[i].ToString().Trim());
                            costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsh_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
                        }
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(costCenterCode);
                        sql.Append(costCenterName + ")");
                    }
                }


                if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
                {
                    //no SubWorkCode for JobSplitDetailLossTime

                    //string text = txtSubWorkCode.Text.Replace("'", "`");
                    //ArrayList arr = CommonLookUp.DivideString(text);
                    //if (arr.Count > 0)
                    //{
                    //    string SubWorkCode = "";
                    //    for (int i = 0; i < arr.Count; i++)
                    //        SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                    //    sql.Append("\nAND (");
                    //    sql.Append("\n 1 = 0");
                    //    sql.Append(SubWorkCode + ")");
                    //}
                }

                #endregion
                if (rblBillable.SelectedValue != "A")   
                {
                    sql.Append(string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue == "B" ? true : false));
                }
                #region Date Time Pickers
                if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
                {
                    if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                         , Convert.ToDateTime(dtpRangeTo.Date)));
                    }
                    else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date)));
                    }
                    else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date)));
                    }
                }
                #endregion
                #endregion
            }

            if (rblOption.SelectedValue == "E")
            {
                if (rbEmployeeOptions.SelectedValue == "S")
                {
                    if (chkSubWorkCode.Checked)
                        sql.Append(@"'
                    SET @PivotCol  = 'Convert(VarChar(50),Slm_ClientJobName) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'') + Convert(VarChar(10),Jsd_SubWorkCode)'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                    else
                        sql.Append(@"'
                    SET @PivotCol  = 'Convert(VarChar(50),Slm_ClientJobName) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'')'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else if (rbEmployeeOptions.SelectedValue == "D")
                {
                    sql.Append(@"'
                    SET @PivotCol  = 'convert(char(10),  Jsh_JobSplitDate,112)'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
            }
            else if (rblOption.SelectedValue == "D")
                sql.Append(@"'
                        SET @PivotCol  = 'convert(varchar(50),dbo.getcostcentername(Jsh_Costcenter))'

                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
            {
                sql.Append(@"'

                        --SET @PivotCol  = 'convert(char(4),DATEPART(yy,  Jsh_JobSplitDate)) + convert(char(4),DATEPART(DY,  Jsh_JobSplitDate)) + Convert(varchar(6),Jsh_JobSplitDate,113)'
                        
                        SET @PivotCol  = 'convert(char(10),  Jsh_JobSplitDate,112)'

                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            }
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
            {
                sql.Append(@"'
                            --SET @PivotCol  = 'convert(char(4),DATEPART(yy,  Jsh_JobSplitDate)) + convert(char(3),DATEPART(WW,  Jsh_JobSplitDate)) + CONVERT(char(6), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 107) + '' - '' + CONVERT(char(6), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 107)'

                            SET @PivotCol  = 'CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101)'

                            SET @Summaries = 'sum(Jsd_ActHours)'

                            EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            }
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
                sql.Append(@"' 
                            --SET @PivotCol  = 'convert(char(6),Jsh_JobSplitDate,112) + '' '' + CONVERT(char(3),Jsh_JobSplitDate,100) + ''-'' + RIGHT(CONVERT(char(4),DATEPART(YY,  Jsh_JobSplitDate)),2)'

                            SET @PivotCol  = 'convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2)'

                            SET @Summaries = 'sum(Jsd_ActHours)'

                            EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
        }
        #endregion
        #region Not Default Report
        else if (chkDefaultReport.Checked == false)
        {
            #region Filters
            if (userCostCenters != "" && !userCostCenters.Contains("ALL"))
            {
                string temp = userCostCenters;
                temp = temp.Replace("x", "''").Replace("y", ",");
                filters.Append("\nAND Jsh_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            }

            #region Textboxes
            if (txtDashJobCode.Text != "")
            {
                string text = txtDashJobCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string dashJobCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                    filters.Append("\nAND (");
                    filters.Append("\n 1 = 0");
                    filters.Append(dashJobCode + ")");
                }
            }
            if (txtClientJobNo.Text != "")
            {
                string text = txtClientJobNo.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string clientJobNo = "";
                    for (int i = 0; i < arr.Count; i++)
                        clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                    filters.Append("\nAND (");
                    filters.Append("\n 1 = 0");
                    filters.Append(clientJobNo + ")");
                }
            }
            if (txtClientCode.Text != "")
            {
                string text = txtClientCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);

                if (arr.Count > 0)
                {
                    string clientCode = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%'')", arr[i].ToString().Trim());
                    }
                    filters.Append("\nAND (");
                    filters.Append("\n 1 = 0");
                    filters.Append(clientCode);
                }
            }

            if (txtCostCenter.Text != "")
            {
                string text = txtCostCenter.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string costCenterCode = "", costCenterName = "";
                    for (int i = 0; i < arr.Count; i++)
                    {
                        costCenterCode += string.Format("\nOR Jsh_Costcenter like ''{0}%''", arr[i].ToString().Trim());
                        costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsh_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
                    }
                    filters.Append("\nAND (");
                    filters.Append("\n 1 = 0");
                    filters.Append(costCenterCode);
                    filters.Append(costCenterName + ")");
                }
            }
            if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
            {
                string text = txtSubWorkCode.Text.Replace("'", "`");
                ArrayList arr = CommonLookUp.DivideString(text);
                if (arr.Count > 0)
                {
                    string SubWorkCode = "";
                    for (int i = 0; i < arr.Count; i++)
                        SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                    filters.Append("\nAND (");
                    filters.Append("\n 1 = 0");
                    filters.Append(SubWorkCode + ")");
                }
            }
            #endregion

            if (rblBillable.SelectedValue != "A")
            {
                filters.Append(string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue == "B" ? true : false));
            }

            #region Date Time Pickers
            if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
            {
                if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                {
                    filters.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                     , Convert.ToDateTime(dtpRangeTo.Date)));
                }
                else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date)));
                }
                else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                {
                    filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date)));
                }
            }
            #endregion
#endregion
            // columns to be included
            if(chkSubWorkCode.Checked)
            {
                includeCol.Append(@"
                                   , Swc_Description as [Work Description]
                                   , Swc_FBSCode
                                   , Swc_FBSCodeDesc
                                   , Swc_MMCode
                                   , Swc_MMCodeDesc
                                   , Swc_MSCode
                                   , Swc_MSCodeDesc
");
            }

            string groupBy = string.Empty;
            if (chkSubWorkCode.Checked)
                groupBy = @"GROUP BY Dcm_DivisionDesc, Dcm_Departmentdesc, Scm_Sectiondesc, Sscm_Sectiondesc, Pcm_ProcessDesc, Jsd_JobCode, Slm_ClientJobName, Jsd_Category, Jcm_JobDescription, Jsd_ClientJobNo, Jsd_SubWorkCode, Swc_Description, Swc_MMCode, Swc_MMCodeDesc, Swc_MSCode, Swc_MSCodeDesc, Swc_FBSCode, Swc_FBSCodeDesc";
            else
                groupBy = @"GROUP BY Dcm_DivisionDesc, Dcm_Departmentdesc, Scm_Sectiondesc, Sscm_Sectiondesc, Pcm_ProcessDesc, Jsd_JobCode, Slm_ClientJobName, Jsd_Category, Jcm_JobDescription, Jsd_ClientJobNo, Jsd_SubWorkCode";

            #region Employee, Summary, Daily
            if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S" && ddlDateType.SelectedValue == "D")
            {
                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = 'select Adt_AccountDesc + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                    , Jsd_ActHours 
	                                from T_JobSplitDetail 
	                                join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
	                                LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                    LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}
	                                '

                                SET @PivotCol = 'CONVERT(VARCHAR(10),Jsh_JobSplitDate,101)'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = 'select CONVERT(VARCHAR(10),Jsh_JobSplitDate,101) [Date]
                                    , Jsd_ActHours 
	                                from T_JobSplitDetail 
	                                join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
	                                LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                    LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}
	                                '

                                SET @PivotCol = 'Adt_AccountDesc + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
            }
            #endregion
            #region Employee, Summary, Weekly
            else if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S" && ddlDateType.SelectedValue == "W") 
            {
                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = 'select Adt_AccountDesc + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                    , sum(Jsd_ActHours) [Jsd_ActHours]
                                    from T_JobSplitDetail 
                                    join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                    LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                    LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                        AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}    
           
		                            GROUP BY CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101), Adt_AccountDesc, Emt_FirstName, Emt_MiddleName, Emt_LastName
                                    '
                                                 
                                SET @PivotCol = 'CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101)'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = 'select CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101) [Date]
                                    , sum(Jsd_ActHours) [Jsd_ActHours]
                                    from T_JobSplitDetail 
                                    join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                    LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                    LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                        AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}    
           
		                            GROUP BY CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101), Adt_AccountDesc, Emt_FirstName, Emt_MiddleName, Emt_LastName
                                    '

                                SET @PivotCol = 'Adt_AccountDesc + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                
            }
            #endregion
            #region Employee, Summary, Monthly
            else if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S" && ddlDateType.SelectedValue == "M")
            {
                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                    DECLARE @PivotCol varchar(300)
                                    DECLARE @Summaries varchar(max)

                                    SET @SelectList = 'select Adt_AccountDesc + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                        , sum(Jsd_ActHours) [Jsd_ActHours]
                                        from T_JobSplitDetail 
                                        join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                        LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                        LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                            AND Adt_AccountType = ''POSITION''
                                        WHERE 1 = 1
                                            {0}
		                                GROUP BY UPPER(DATENAME(MM,  Jsh_JobSplitDate)), Adt_AccountDesc, Emt_FirstName, Emt_MiddleName, Emt_LastName
                                        '

                                    SET @PivotCol = 'UPPER(DATENAME(MM,  Jsh_JobSplitDate))'
                                    SET @Summaries = 'sum(Jsd_ActHours)'

                                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries
    
                                    ");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                    DECLARE @PivotCol varchar(300)
                                    DECLARE @Summaries varchar(max)

                                    SET @SelectList = 'select UPPER(DATENAME(MM,  Jsh_JobSplitDate)) [Month]
                                        , sum(Jsd_ActHours) [Jsd_ActHours]
                                        from T_JobSplitDetail 
                                        join T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                        LEFT JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                        LEFT join T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                            AND Adt_AccountType = ''POSITION''
                                        WHERE 1 = 1
                                            {0}
		                                GROUP BY UPPER(DATENAME(MM,  Jsh_JobSplitDate)), Adt_AccountDesc, Emt_FirstName, Emt_MiddleName, Emt_LastName
                                        '

                                    SET @PivotCol = 'Adt_AccountDesc + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                                    SET @Summaries = 'sum(Jsd_ActHours)'

                                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries
    
                                    ");
                }
                
            }
            #endregion
            #region Employee, Detail
            else if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
            {
                groupBy += ", Q.emt_lastname, Q.emt_FirstName, Jsh_JobSplitDate, Jsh_EmployeeId, Ecc_Description";

                sql.Append(@"declare @SelectList varchar(max)

                    SET @SelectList = '
                    SELECT  
                            Jsd_JobCode
                            ,Jcm_JobDescription
                            ,Jsd_Category
                            ,Q.Emt_LastName + '', '' + Q.Emt_FirstName as [Name]
                            ,Jsh_EmployeeID
                            ,convert(varchar(20),Jsh_JobSplitDate, 101) as [SPLIT DATE]
                            ,SUM(Jsd_ActHours) as [Jsd_ActHours]
                            , Jsd_ClientJobNo
                            , Jsd_SubWorkCode as [Subwork Code]
                            , SUM(case when Jsd_Overtime=0 then Jsd_ActHours end) [REG]
							, SUM(case when Jsd_Overtime=1 then Jsd_ActHours end) [OT]
                            {1}
                            , Ecc_Description
                        from T_JobSplitDetail
                inner join T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                    and Jsh_Status = Jsd_Status and Jsh_Status = ''9''
                --Inner join t_employeelogledger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                    --LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                LEFT JOIN T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_AccountDetail ON Adt_AccountCode = Jsd_Status 
                    AND Adt_AccountType =  ''WFSTATUS''
                    --left join T_LeaveTypeMaster  on T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join T_LeaveTypeMaster  T_LeaveTypeMaster2 on T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Q.Emt_CostCenterCode
                LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                LEFT JOIN {2}..E_SubWorkCodeMaster on Jsd_SubWorkCode = Swc_AccountCode and Swc_CostCenterCode = Slm_CostCenter
                LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                JOIN {2}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  

                        WHERE 1 = 1
                        {0}
                    " + groupBy + @"

                    ORDER BY Ecc_Description, Jsd_JobCode, Jsd_Category DESC, Jsh_EmployeeId'

                    execute(@SelectList)
                    ");

                sql = new StringBuilder(string.Format(sql.ToString(), filters.ToString(), includeCol.ToString(), ConfigurationManager.AppSettings["ERP_DB"]));
            }
            #endregion
            #region Department, Summary
            else if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "S")
            {
                groupBy = "GROUP BY DATENAME(MM, Jsh_JobSplitDate), Ecc_Description, Jcm_JobDescription, Jsd_SubWorkCode, Jsd_JobCode";
                sql.Append(@"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(300)

                        SET @SelectList = 'select Ecc_Description
	                        , Jcm_JobDescription [Job Description]
                            , Jsd_JobCode [CPH Job No.]
	                        , case 
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''a'' then ''Contract Mhr''
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''b'' then ''Design Change Mhr''
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''c'' then ''Blanket Mhr''
		                        else ''Others''
	                            end [ ] 
	                        , Sum(Jsd_ActHours) as [Jsd_ActHours]
	                        from {2}..E_COSASCostCenter 
	                        RIGHT join T_JobSplitHeader on Ecc_CostCenter = Jsh_Costcenter
	                        join T_JobSplitDetail on Jsh_ControlNo = Jsd_ControlNo
	                        LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                            --JOIN T_EmployeeLogLedger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                            JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode and Jsd_ClientJobNo = Slm_ClientJobNo
	                        where Jsh_Status = ''9''
                            {0}
	                        " + groupBy + @"'

                        SET @PivotCol  = 'DATENAME(MM, Jsh_JobSplitDate)'
	
                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries ");

            }
            #endregion
            #region Department, Detail
            else if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")
            {
                groupBy = "GROUP BY Jsd_JobCode, Jcm_JobDescription, Ecc_Description, Ecc_COSASCostCenter";

                sql.Append(@"declare @SelectList varchar(max)
                    SET @SelectList = '
                    SELECT  
                            Ecc_Description 
                            , Jsd_JobCode [CPH Job No.]
                            , Jcm_JobDescription [Job Description]
                            ,SUM(Jsd_ActHours) as [Total Manhours]
                        from T_JobSplitDetail
                inner join T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                    and Jsh_Status = Jsd_Status and Jsh_Status = ''9''
                --Inner join t_employeelogledger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                    --LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                LEFT JOIN T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_AccountDetail ON Adt_AccountCode = Jsd_Status 
                    AND Adt_AccountType =  ''WFSTATUS''
                    --left join T_LeaveTypeMaster  on T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join T_LeaveTypeMaster  T_LeaveTypeMaster2 on T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                LEFT JOIN {2}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter

                        WHERE 1 = 1
                        {0}
                    " + groupBy + @"
                    UNION

                    select 
                            Ecc_Description 
                            , Jsd_JobCode [CPH Job No.]
                            , Jcm_JobDescription [Client Job Name]
                            ,SUM(Jsd_ActHours) as [Total Manhours]
                    from T_JobSplitDetail
                inner join T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                        and Jsh_Status = Jsd_Status and Jsh_Status = ''9''
                    --Inner join t_employeelogledgerhist on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                    --LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                    JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                        and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_AccountDetail ON Adt_AccountCode = Jsd_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                    --left join T_LeaveTypeMaster  on T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join T_LeaveTypeMaster  T_LeaveTypeMaster2 on T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                    LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                    LEFT JOIN {2}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter

                    WHERE 1 = 1
                        {0}

                    " + groupBy + @"
                        
                    ORDER BY Ecc_Description ,Jsd_JobCode, Jcm_JobDescription'

                    execute(@SelectList)
                    ");
            }
            #endregion
            #region Client
            else if (rblOption.SelectedValue == "C")
            {
                sql.Append(@"declare @SelectList varchar(max)
                    SET @SelectList = '
                    SELECT  
                            Jsd_JobCode
                            ,Jcm_JobDescription
                            ,Jsd_Category
                            ,SUM(Jsd_ActHours) as [Jsd_ActHours]
                            , Jsd_SubWorkCode as [Subwork Code]
                            {1}
                        from T_JobSplitDetail
                inner join T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                    and Jsh_Status = Jsd_Status and Jsh_Status = ''9''
                JOIN T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                JOIN T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    LEFT JOIN T_AccountDetail ON Adt_AccountCode = Jsd_Status 
                    AND Adt_AccountType =  ''WFSTATUS''
                    --left join T_LeaveTypeMaster  on T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join T_LeaveTypeMaster  T_LeaveTypeMaster2 on T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Q.Emt_CostCenterCode
                LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                LEFT JOIN {2}..E_SubWorkCodeMaster on Jsd_SubWorkCode = Swc_AccountCode and Swc_CostCenterCode = Slm_CostCenter
                LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo

                        WHERE 1 = 1
                        {0}
                    " + groupBy + @"
                        
                    ORDER BY Jsd_JobCode, Jsd_Category DESC'

                    execute(@SelectList)
                    ");
            }
#endregion
            sql = new StringBuilder(string.Format(sql.ToString(), filters.ToString(), includeCol.ToString(), ConfigurationManager.AppSettings["ERP_DB"]));
        }
        #endregion
        return sql.ToString();
    }
    
    private void GenerateTables()
    {
        if (dsView.Tables.Count > 0)
        {
            this.NumberOfControls = 0;
            Panel1.Controls.Clear();
            ArrayList arr = new ArrayList();
            if (!chkSubWorkCode.Checked && rblOption.SelectedValue != "E")
                try
                {
                    dsView.Tables[0].Columns.Remove("Work Activity Code");
                }
                catch
                {
                }
            #region Default Report
            if (chkDefaultReport.Checked == true)
            {
                #region Per Employee
                if (lastVal == "E")
                {
                    dsView.Tables[0].DefaultView.Sort = "CostCenterName";

                    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                    {
                        //TotalDs.Tables[0].Rows.Add(TotalDs.Tables[0].Rows
                    }

                    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                    {
                        int ctr = 0;
                        if (!arr.Contains(dr["CostCenterName"]))
                        {
                            GenerateDynamicControls(dr, "CostCenterName", ctr, arr);
                        }
                    }
                }
                #endregion
                #region Per Department
                else if (lastVal == "D")
                {
                    dsView.Tables[0].DefaultView.Sort = "CPH Job No.";
                    DataTable dtSource = dsView.Tables[0].Copy();
                    HtmlGenericControl div1 = new HtmlGenericControl("div");
                    div1.Controls.Add(AddGridViewControl(dtSource));
                    div1.Attributes["style"] = "height:" + outerHeight.ToString() + ";";
                    Panel1.Controls.Add(div1);
                }
                #endregion
                #region Per Client
                else if (lastVal == "C")
                {
                    dsView.Tables[0].DefaultView.Sort = "Slm_ClientCode";
                    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                    {
                        int ctr = 0;
                        if (!arr.Contains(dr["Slm_ClientCode"]))
                        {
                            GenerateDynamicControls(dr, "Slm_ClientCode", ctr, arr);
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Not Default Report
            else if (chkDefaultReport.Checked == false) // Default Report checkbox is not checked
            {
                if (dsView != null && dsView.Tables.Count > 0)
                {
                    DataTable temp1 = new DataTable();
                    DataTable dtFinal = new DataTable();


                    if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S") //&& ddlDateType.SelectedValue == "D"
                    {
                        DataTable dtSource = dsView.Tables[0];
                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.Controls.Add(AddGridViewControl(dtSource));
                        div1.Attributes["style"] = "height:" + outerHeight.ToString() + ";";
                        Panel1.Controls.Add(div1);

                        return;
                    }
                    else if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "S")
                    {
                        dsView.Tables[0].DefaultView.Sort = "Ecc_Description";
                        foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                        {
                            int ctr = 0;
                            if (!arr.Contains(dr["Ecc_Description"]))
                            {
                                GenerateDynamicControls(dr, "Ecc_Description", ctr, arr);
                            }
                        }
                    }
                    else if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")
                    {
                        dsView.Tables[0].DefaultView.Sort = "Ecc_Description";
                        foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                        {
                            int ctr = 0;
                            if (!arr.Contains(dr["Ecc_Description"]))
                            {
                                GenerateDynamicControls(dr, "Ecc_Description", ctr, arr);
                            }
                        }

                        GridView grdv = new GridView();
                        grdv.Attributes["runat"] = "server";

                        grdv.CellPadding = 0;
                        grdv.CellSpacing = 1;
                        grdv.ID = "grdv_GrandTotal";
                        grdv.Attributes["Width"] = "auto";
                        grdv.Attributes["border"] = "0";

                        DataTable dt = new DataTable();
                        dt.Columns.Add(" ");
                        dt.Columns.Add("  ");
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[0][0] = "GRAND TOTAL";
                        dt.Rows[0][1] = grandTotal.ToString();
                        grdv.RowCreated += new GridViewRowEventHandler(grdViewTotal_RowCreated);
                        grdv.DataSource = dt;
                        grdv.DataBind();

                        Panel1.Controls.Add(grdv);
                        
                    }
                    else 
                    {
                        //if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
                        //{
                        //    dsView.Tables[0].DefaultView.Sort = "Ecc_Description";
                        //    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                        //    {
                        //        int ctr = 0;
                        //        if (!arr.Contains(dr["Ecc_Description"]))
                        //        {
                        //            GenerateDynamicControls(dr, "Ecc_Description", ctr, arr);
                        //        }
                        //    }
                        //}

                        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
                        {
                            dtFinal.Columns.Add("Date");
                            temp1.Columns.Add("Date");
                        }
                        else 
                        if (rblOption.SelectedValue == "C")  
                        {
                            dtFinal.Columns.Add(" ");
                            temp1.Columns.Add(" ");
                        }

                        if (chkSubWorkCode.Checked)
                        {
                            dtFinal.Columns.Add("Work Code");
                            dtFinal.Columns.Add("Work Description");

                            temp1.Columns.Add("WorkCode");
                            temp1.Columns.Add("WorkDescription");
                        }
                        else // add columns with empty names
                        {
                            dtFinal.Columns.Add("  "); // two spaces
                            temp1.Columns.Add("  ");
                            dtFinal.Columns.Add("   "); // three spaces
                            temp1.Columns.Add("   ");
                        }

                        dtFinal.Columns.Add("Man Hours");
                        temp1.Columns.Add("ManHours");

                        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
                        {
                            dtFinal.Columns.Add("REG");
                            dtFinal.Columns.Add("OT");
                            temp1.Columns.Add("REG");
                            temp1.Columns.Add("OT");

                            dtFinal.Columns.Add("CYO Job No");
                            temp1.Columns.Add("CYOJobNo");
                        }
                        

                        if (chkSubWorkCode.Checked)
                        {
                            dtFinal.Columns.Add("FBS Code");
                            dtFinal.Columns.Add("FBS Description");
                            dtFinal.Columns.Add("MM Code");
                            dtFinal.Columns.Add("MM Description");
                            dtFinal.Columns.Add("MS Code");
                            dtFinal.Columns.Add("MS Description");

                            temp1.Columns.Add("FBSCode");
                            temp1.Columns.Add("FBSDescription");
                            temp1.Columns.Add("MMCode");
                            temp1.Columns.Add("MMDescription");
                            temp1.Columns.Add("MSCode");
                            temp1.Columns.Add("MSDescription");
                        }

                        //if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
                        //{
                        //    dtFinal.Columns.Add("Ecc_Description");
                        //    temp1.Columns.Add("Ecc_Description");
                        //}
                    }

                    int tblIndex = 0;
                    int x = 0;

                    string currentEmpID;
                    string prevEmpID = string.Empty;
                    Boolean currentBillable;
                    Boolean previousBillable = false;
                    string currentJobCode;
                    string prevJobCode = string.Empty;

                    decimal[] EmpTotalHours = new decimal[2] { 0, 0 };
                    decimal[] BillableTotalHours = new decimal[2] { 0, 0 };
                    decimal[] JobCodeTotalHours = new decimal[2] { 0, 0 };
                    decimal[] GrandTotal = new decimal[2] { 0, 0 };

                    #region Employee, Detail
                    if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D") 
                    {
                        for (int i = 0; i < dsView.Tables[0].Rows.Count; i++)
                        {
                            temp1.Rows.Clear();
                            tblIndex = 0;

                            currentJobCode = dsView.Tables[0].Rows[i]["Jsd_JobCode"].ToString();
                            currentBillable = Convert.ToBoolean(dsView.Tables[0].Rows[i]["Jsd_Category"]);
                            currentEmpID = dsView.Tables[0].Rows[i]["Jsh_EmployeeID"].ToString();

                            if (i > 0)
                            {
                                if (!currentEmpID.Equals(prevEmpID) || !currentJobCode.Equals(prevJobCode) || currentBillable != previousBillable)
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "Sub Total For Employee";
                                    temp1.Rows[tblIndex][2] = "" + prevEmpID + "";
                                    SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                                    EmpTotalHours = new decimal[2] { 0, 0 };
                                    tblIndex++;
                                }

                                if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "Total";
                                    temp1.Rows[tblIndex][2] = previousBillable ? "(B)" : "(N)";
                                    SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                    BillableTotalHours = new decimal[2] { 0, 0 };
                                    tblIndex++;
                                }

                                if (!currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "Total for Job No.";
                                    temp1.Rows[tblIndex][2] = "" + prevJobCode + "";
                                    SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                    JobCodeTotalHours = new decimal[2] { 0, 0 };
                                    tblIndex += 2;
                                    temp1.Rows.Add(temp1.NewRow());
                                }
                            }

                            if (!currentJobCode.Equals(prevJobCode))
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows[tblIndex][0] = "" + currentJobCode + "";
                                temp1.Rows[tblIndex][1] = "" + dsView.Tables[0].Rows[i]["Jcm_JobDescription"].ToString() + "";
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex += 2;
                            }

                            if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows[tblIndex][0] = currentBillable ? "BILLABLE (B)" : "NON-BILLABLE (N)";
                                tblIndex++;
                            }

                            if (!currentEmpID.Equals(prevEmpID) || currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows[tblIndex][0] = "" + currentEmpID + "";
                                temp1.Rows[tblIndex][1] = "" + dsView.Tables[0].Rows[i]["Name"].ToString() + "";
                                tblIndex++;
                                prevEmpID = currentEmpID;
                            }

                            temp1.Rows.Add(temp1.NewRow());
                            SetTableSummary(dsView, temp1, i, tblIndex, chkSubWorkCode.Checked);
                            tblIndex++;

                            sumValues(EmpTotalHours, dsView, i);
                            sumValues(BillableTotalHours, dsView, i);
                            sumValues(JobCodeTotalHours, dsView, i);
                            sumValues(GrandTotal, dsView, i);

                            if (!currentJobCode.Equals(prevJobCode))
                                prevJobCode = currentJobCode;
                            if (currentBillable != previousBillable)
                                previousBillable = currentBillable;

                            if (i == dsView.Tables[0].Rows.Count - 1)
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Sub Total For Employee";
                                temp1.Rows[tblIndex][2] = "" + currentEmpID + "";
                                SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                                tblIndex++;

                                // for total of billable or non-billable if no more rows after this 
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Total";
                                temp1.Rows[tblIndex][2] = currentBillable ? "(B)" : "(N)";
                                SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                tblIndex++;

                                // for total of job code 
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Total for Job No.";
                                temp1.Rows[tblIndex][2] = "" + currentJobCode + "";
                                SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                tblIndex++;

                                // for Grand Total
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Grand Total:";
                                SetTableSummaryTotals(GrandTotal, temp1, tblIndex);
                            }

                            x = dtFinal.Rows.Count;
                            for (int z = 0; z < temp1.Rows.Count; z++)
                            {
                                dtFinal.Rows.Add(dtFinal.NewRow());
                                dtFinal.Rows[x + z]["Date"] = temp1.Rows[z]["Date"];
                                //dtFinal.Rows[x + z]["Man Hours"] = temp1.Rows[z]["ManHours"];
                                dtFinal.Rows[x + z]["REG"] = temp1.Rows[z]["REG"];
                                dtFinal.Rows[x + z]["OT"] = temp1.Rows[z]["OT"];
                                dtFinal.Rows[x + z]["CYO Job No"] = temp1.Rows[z]["CYOJobNo"];
                                if (chkSubWorkCode.Checked)
                                {
                                    dtFinal.Rows[x + z]["Work Code"] = temp1.Rows[z]["WorkCode"];
                                    dtFinal.Rows[x + z]["Work Description"] = temp1.Rows[z]["WorkDescription"];
                                    dtFinal.Rows[x + z]["FBS Code"] = temp1.Rows[z]["FBSCode"];
                                    dtFinal.Rows[x + z]["FBS Description"] = temp1.Rows[z]["FBSDescription"];
                                    dtFinal.Rows[x + z]["MM Code"] = temp1.Rows[z]["MMCode"];
                                    dtFinal.Rows[x + z]["MM Description"] = temp1.Rows[z]["MMDescription"];
                                    dtFinal.Rows[x + z]["MS Code"] = temp1.Rows[z]["MSCode"];
                                    dtFinal.Rows[x + z]["MS Description"] = temp1.Rows[z]["MSDescription"];
                                }
                                else
                                {
                                    dtFinal.Rows[x + z]["  "] = temp1.Rows[z]["  "];
                                    dtFinal.Rows[x + z]["   "] = temp1.Rows[z]["   "];
                                }
                            }
                        }
                        dtFinal.Columns.Remove("Man Hours");
                    }
                    #endregion
                    #region Client
                    else if (rblOption.SelectedValue == "C") 
                    {
                        for (int i = 0; i < dsView.Tables[0].Rows.Count; i++)
                        {
                            temp1.Rows.Clear();
                            tblIndex = 0;

                            currentJobCode = dsView.Tables[0].Rows[i]["Jsd_JobCode"].ToString();
                            currentBillable = Convert.ToBoolean(dsView.Tables[0].Rows[i]["Jsd_Category"]);

                            if (i > 0)
                            {

                                if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "Total";
                                    temp1.Rows[tblIndex][2] = previousBillable ? "(B)" : "(N)";
                                    SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                    BillableTotalHours = new decimal[2] { 0, 0 };
                                    tblIndex++;
                                }

                                if (!currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "Total for Job No.";
                                    temp1.Rows[tblIndex][2] = "" + prevJobCode + "";
                                    SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                    JobCodeTotalHours = new decimal[2] { 0, 0 };
                                    tblIndex += 2;
                                    temp1.Rows.Add(temp1.NewRow());

                                }
                            }

                            if (!currentJobCode.Equals(prevJobCode))
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows[tblIndex][0] = "" + currentJobCode + "";
                                temp1.Rows[tblIndex][1] = "" + dsView.Tables[0].Rows[i]["Jcm_JobDescription"].ToString() + "";
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex += 2;
                            }

                            if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                            {
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows[tblIndex][0] = currentBillable ? "BILLABLE (B)" : "NON-BILLABLE (N)";
                                tblIndex++;
                                previousBillable = currentBillable;
                            }

                            temp1.Rows.Add(temp1.NewRow());
                            SetTableSummary(dsView, temp1, i, tblIndex, chkSubWorkCode.Checked);
                            tblIndex++;

                            sumValues(BillableTotalHours, dsView, i);
                            sumValues(JobCodeTotalHours, dsView, i);
                            sumValues(GrandTotal, dsView, i);

                            if (!currentJobCode.Equals(prevJobCode))
                                prevJobCode = currentJobCode;

                            if (i == dsView.Tables[0].Rows.Count - 1)
                            {
                                // for total of billable or non-billable if no more rows after this 
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Total";
                                temp1.Rows[tblIndex][2] = currentBillable ? "(B)" : "(N)";
                                SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                tblIndex++;

                                // for total of job code 
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Total for Job No.";
                                temp1.Rows[tblIndex][2] = "" + currentJobCode + "";
                                SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                tblIndex++;

                                // for Grand Total
                                temp1.Rows.Add(temp1.NewRow());
                                temp1.Rows.Add(temp1.NewRow());
                                tblIndex++;
                                temp1.Rows[tblIndex][1] = "Grand Total:";
                                SetTableSummaryTotals(GrandTotal, temp1, tblIndex);
                            }

                            x = dtFinal.Rows.Count;
                            for (int z = 0; z < temp1.Rows.Count; z++)
                            {
                                dtFinal.Rows.Add(dtFinal.NewRow());
                                dtFinal.Rows[x + z][" "] = temp1.Rows[z][" "];
                                dtFinal.Rows[x + z]["Man Hours"] = temp1.Rows[z]["ManHours"];
                                if (chkSubWorkCode.Checked)
                                {
                                    dtFinal.Rows[x + z]["Work Code"] = temp1.Rows[z]["WorkCode"];
                                    dtFinal.Rows[x + z]["Work Description"] = temp1.Rows[z]["WorkDescription"];
                                    dtFinal.Rows[x + z]["FBS Code"] = temp1.Rows[z]["FBSCode"];
                                    dtFinal.Rows[x + z]["FBS Description"] = temp1.Rows[z]["FBSDescription"];
                                    dtFinal.Rows[x + z]["MM Code"] = temp1.Rows[z]["MMCode"];
                                    dtFinal.Rows[x + z]["MM Description"] = temp1.Rows[z]["MMDescription"];
                                    dtFinal.Rows[x + z]["MS Code"] = temp1.Rows[z]["MSCode"];
                                    dtFinal.Rows[x + z]["MS Description"] = temp1.Rows[z]["MSDescription"];
                                }
                                else
                                {
                                    dtFinal.Rows[x + z]["  "] = temp1.Rows[z]["  "];
                                    dtFinal.Rows[x + z]["   "] = temp1.Rows[z]["   "];
                                }
                            }
                        }

                    }
                    #endregion

                    if ((!(rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "S"))
                        && (!(rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")))
                    {
                        DataTable dtSource = dtFinal.Copy();
                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.Controls.Add(AddGridViewControl(dtSource));
                        div1.Attributes["style"] = "height:" + outerHeight.ToString() + ";";
                        Panel1.Controls.Add(div1);
                    }
                }
                else
                {
                    VSDataTable = null;
                    grdView.DataSource = null;
                    grdView.DataBind();
                }
            }
            #endregion
        }
        else
        {
            Panel1.Controls.Clear();
        }
    }
    protected void SetTableSummaryTotals(decimal[] totals, DataTable temp, int dtIndex)
    {
        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
            temp.Rows[dtIndex][4] = "" + totals[1].ToString() + "";
        else
            temp.Rows[dtIndex][3] = "" + totals[1].ToString() + "";
        //temp.Rows[dtIndex]["Workload"] = (totals[3] / totals[2]).ToString("0.00%");
        //temp.Rows[dtIndex]["EmployeeCount"] = (totals[1]).ToString("0");
        //temp.Rows[dtIndex]["Core"] = (totals[2]).ToString("0.00");
    }

    protected void SetTableSummaryTotals(decimal[] totals, DataTable temp, int dtIndex, int columnForTotal)
    {
        temp.Rows[dtIndex][columnForTotal] = "" + totals[1].ToString() + "";
    }

    protected void SetTableSummary(DataSet ds, DataTable temp, int dsIndex, int dtIndex, bool includeSubWork)
    {
        if ((rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D")      // Department and Summary
            || (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D"))  // Department and Detail
        {
            temp.Rows[dtIndex][0] = ds.Tables[0].Rows[dsIndex]["Jsd_JobCode"].ToString();
            temp.Rows[dtIndex][1] = ds.Tables[0].Rows[dsIndex]["Jcm_JobDescription"].ToString();
        }
        else
        {
            if (includeSubWork)
            {
                temp.Rows[dtIndex]["WorkCode"] = ds.Tables[0].Rows[dsIndex]["Subwork Code"].ToString();
                temp.Rows[dtIndex]["WorkDescription"] = ds.Tables[0].Rows[dsIndex]["Work Description"].ToString();
                temp.Rows[dtIndex]["FBSCode"] = ds.Tables[0].Rows[dsIndex]["Swc_FBSCode"].ToString();
                temp.Rows[dtIndex]["FBSDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_FBSCodeDesc"].ToString();
                temp.Rows[dtIndex]["MMCode"] = ds.Tables[0].Rows[dsIndex]["Swc_MMCode"].ToString();
                temp.Rows[dtIndex]["MMDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_MMCodeDesc"].ToString();
                temp.Rows[dtIndex]["MSCode"] = ds.Tables[0].Rows[dsIndex]["Swc_MSCode"].ToString();
                temp.Rows[dtIndex]["MSDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_MSCodeDesc"].ToString();
            }
            if(rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
            {
                temp.Rows[dtIndex]["Date"] = ds.Tables[0].Rows[dsIndex]["SPLIT DATE"].ToString();
                temp.Rows[dtIndex]["CYOJobNo"] = ds.Tables[0].Rows[dsIndex]["Jsd_ClientJobNo"].ToString();
                temp.Rows[dtIndex]["REG"] = ds.Tables[0].Rows[dsIndex]["REG"].ToString();
                temp.Rows[dtIndex]["OT"] = ds.Tables[0].Rows[dsIndex]["OT"].ToString();
            }
        }
        
        temp.Rows[dtIndex]["ManHours"] = ds.Tables[0].Rows[dsIndex]["Jsd_Acthours"].ToString();
        

    }

    protected void sumValues(decimal[] values, DataSet ds, int dsIndex)
    {
        values[0] += 1;
        values[1] += Convert.ToDecimal(ds.Tables[0].Rows[dsIndex]["Jsd_ActHours"]);
    }

    #region Custom Methods
    private void GenerateDynamicControls(DataRowView dr, string col, int ctr, ArrayList arr)
    {
        DataTable dtSource = dsView.Tables[0].Copy();
        dtSource.DefaultView.RowFilter = string.Format("[{0}] = '{1}'", col, dr[col].ToString());
        dtSource.Columns.Remove(col);
        //header
        HtmlGenericControl div1 = new HtmlGenericControl("div");
        div1.Attributes["class"] = "dhtmlgoodies_question";
        div1.ID = "Div_" + dr[col].ToString() + ctr++;
        div1.Attributes["style"] = "width: 894px;";
        div1.Controls.Add(AddLabelControl(dr[col].ToString()));
        //panel
        Panel pan = new Panel();

        //formatTable(dtSource);

        pan.Controls.Add(AddGridViewControl(dtSource));
        pan.Attributes["Style"] = "position: static;";
        //pan.Attributes.Add("height", "Auto");
        pan.Width = Unit.Pixel(894);
        pan.ID = "Panel_" + dr[col].ToString() + ctr++;
        pan.ScrollBars = ScrollBars.Auto;
        pan.Height = innerHeight;
        
        //hide2 thingy
        HtmlGenericControl div2 = new HtmlGenericControl("div");
        div2.Attributes["class"] = "dhtmlgoodies_answer";
        div2.ID = "Div_" + dr[col].ToString() + ctr++;
        div2.Attributes["style"] = "width: 894px; height: inherit; left: 0px; top: 0px; position: static;";
        arr.Add(dr[col]);

        div2.Controls.Add(pan);
        Panel1.Controls.Add(div1);
        Panel1.Controls.Add(div2);
    }

    private void formatTable(DataTable dt)  // for customized report layout
    {
        DataTable temp1 = new DataTable();
        DataTable dtFinal = new DataTable();

        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
        {
            dtFinal.Columns.Add("Date");
            temp1.Columns.Add("Date");
        }

        if (chkSubWorkCode.Checked)
        {
            dtFinal.Columns.Add("Work Code");
            dtFinal.Columns.Add("Work Description");

            temp1.Columns.Add("WorkCode");
            temp1.Columns.Add("WorkDescription");
        }
        else // add columns with empty names
        {
            dtFinal.Columns.Add("  "); // two spaces
            temp1.Columns.Add("  ");
            dtFinal.Columns.Add("   "); // three spaces
            temp1.Columns.Add("   ");
        }

        dtFinal.Columns.Add("Man Hours");
        temp1.Columns.Add("ManHours");

        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
        {
            dtFinal.Columns.Add("REG");
            dtFinal.Columns.Add("OT");
            temp1.Columns.Add("REG");
            temp1.Columns.Add("OT");

            dtFinal.Columns.Add("CYO Job No");
            temp1.Columns.Add("CYOJobNo");
        }


        if (chkSubWorkCode.Checked)
        {
            dtFinal.Columns.Add("FBS Code");
            dtFinal.Columns.Add("FBS Description");
            dtFinal.Columns.Add("MM Code");
            dtFinal.Columns.Add("MM Description");
            dtFinal.Columns.Add("MS Code");
            dtFinal.Columns.Add("MS Description");

            temp1.Columns.Add("FBSCode");
            temp1.Columns.Add("FBSDescription");
            temp1.Columns.Add("MMCode");
            temp1.Columns.Add("MMDescription");
            temp1.Columns.Add("MSCode");
            temp1.Columns.Add("MSDescription");
        }

        int tblIndex = 0;
        int x = 0;

        string currentEmpID;
        string prevEmpID = string.Empty;
        Boolean currentBillable;
        Boolean previousBillable = false;
        string currentJobCode;
        string prevJobCode = string.Empty;

        decimal[] EmpTotalHours = new decimal[2] { 0, 0 };
        decimal[] BillableTotalHours = new decimal[2] { 0, 0 };
        decimal[] JobCodeTotalHours = new decimal[2] { 0, 0 };
        decimal[] GrandTotal = new decimal[2] { 0, 0 };

        #region Employee, Detail
        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
        {
            for (int i = 0; i < dsView.Tables[0].Rows.Count; i++)
            {
                temp1.Rows.Clear();
                tblIndex = 0;

                currentJobCode = dsView.Tables[0].Rows[i]["Jsd_JobCode"].ToString();
                currentBillable = Convert.ToBoolean(dsView.Tables[0].Rows[i]["Jsd_Category"]);
                currentEmpID = dsView.Tables[0].Rows[i]["Jsh_EmployeeID"].ToString();

                if (i > 0)
                {
                    if (!currentEmpID.Equals(prevEmpID) || !currentJobCode.Equals(prevJobCode) || currentBillable != previousBillable)
                    {
                        temp1.Rows.Add(temp1.NewRow());
                        temp1.Rows.Add(temp1.NewRow());
                        tblIndex++;
                        temp1.Rows[tblIndex][1] = "Sub Total For Employee";
                        temp1.Rows[tblIndex][2] = "" + prevEmpID + "";
                        SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                        EmpTotalHours = new decimal[2] { 0, 0 };
                        tblIndex++;
                    }

                    if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                    {
                        temp1.Rows.Add(temp1.NewRow());
                        temp1.Rows.Add(temp1.NewRow());
                        tblIndex++;
                        temp1.Rows[tblIndex][1] = "Total";
                        temp1.Rows[tblIndex][2] = previousBillable ? "(B)" : "(N)";
                        SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                        BillableTotalHours = new decimal[2] { 0, 0 };
                        tblIndex++;
                    }

                    if (!currentJobCode.Equals(prevJobCode))
                    {
                        temp1.Rows.Add(temp1.NewRow());
                        temp1.Rows.Add(temp1.NewRow());
                        tblIndex++;
                        temp1.Rows[tblIndex][1] = "Total for Job No.";
                        temp1.Rows[tblIndex][2] = "" + prevJobCode + "";
                        SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                        JobCodeTotalHours = new decimal[2] { 0, 0 };
                        tblIndex += 2;
                        temp1.Rows.Add(temp1.NewRow());
                    }
                }

                if (!currentJobCode.Equals(prevJobCode))
                {
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows[tblIndex][0] = "" + currentJobCode + "";
                    temp1.Rows[tblIndex][1] = "" + dsView.Tables[0].Rows[i]["Jcm_JobDescription"].ToString() + "";
                    temp1.Rows.Add(temp1.NewRow());
                    tblIndex += 2;
                }

                if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                {
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows[tblIndex][0] = currentBillable ? "BILLABLE (B)" : "NON-BILLABLE (N)";
                    tblIndex++;
                }

                if (!currentEmpID.Equals(prevEmpID) || currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                {
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows[tblIndex][0] = "" + currentEmpID + "";
                    temp1.Rows[tblIndex][1] = "" + dsView.Tables[0].Rows[i]["Name"].ToString() + "";
                    tblIndex++;
                    prevEmpID = currentEmpID;
                }

                temp1.Rows.Add(temp1.NewRow());
                SetTableSummary(dsView, temp1, i, tblIndex, chkSubWorkCode.Checked);
                tblIndex++;

                sumValues(EmpTotalHours, dsView, i);
                sumValues(BillableTotalHours, dsView, i);
                sumValues(JobCodeTotalHours, dsView, i);
                sumValues(GrandTotal, dsView, i);

                if (!currentJobCode.Equals(prevJobCode))
                    prevJobCode = currentJobCode;
                if (currentBillable != previousBillable)
                    previousBillable = currentBillable;

                if (i == dsView.Tables[0].Rows.Count - 1)
                {
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows.Add(temp1.NewRow());
                    tblIndex++;
                    temp1.Rows[tblIndex][1] = "Sub Total For Employee";
                    temp1.Rows[tblIndex][2] = "" + currentEmpID + "";
                    SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                    tblIndex++;

                    // for total of billable or non-billable if no more rows after this 
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows.Add(temp1.NewRow());
                    tblIndex++;
                    temp1.Rows[tblIndex][1] = "Total";
                    temp1.Rows[tblIndex][2] = currentBillable ? "(B)" : "(N)";
                    SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                    tblIndex++;

                    // for total of job code 
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows.Add(temp1.NewRow());
                    tblIndex++;
                    temp1.Rows[tblIndex][1] = "Total for Job No.";
                    temp1.Rows[tblIndex][2] = "" + currentJobCode + "";
                    SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                    tblIndex++;

                    // for Grand Total
                    temp1.Rows.Add(temp1.NewRow());
                    temp1.Rows.Add(temp1.NewRow());
                    tblIndex++;
                    temp1.Rows[tblIndex][1] = "Grand Total:";
                    SetTableSummaryTotals(GrandTotal, temp1, tblIndex);
                }

                x = dtFinal.Rows.Count;
                for (int z = 0; z < temp1.Rows.Count; z++)
                {
                    dtFinal.Rows.Add(dtFinal.NewRow());
                    dtFinal.Rows[x + z]["Date"] = temp1.Rows[z]["Date"];
                    //dtFinal.Rows[x + z]["Man Hours"] = temp1.Rows[z]["ManHours"];
                    dtFinal.Rows[x + z]["REG"] = temp1.Rows[z]["REG"];
                    dtFinal.Rows[x + z]["OT"] = temp1.Rows[z]["OT"];
                    dtFinal.Rows[x + z]["CYO Job No"] = temp1.Rows[z]["CYOJobNo"];
                    if (chkSubWorkCode.Checked)
                    {
                        dtFinal.Rows[x + z]["Work Code"] = temp1.Rows[z]["WorkCode"];
                        dtFinal.Rows[x + z]["Work Description"] = temp1.Rows[z]["WorkDescription"];
                        dtFinal.Rows[x + z]["FBS Code"] = temp1.Rows[z]["FBSCode"];
                        dtFinal.Rows[x + z]["FBS Description"] = temp1.Rows[z]["FBSDescription"];
                        dtFinal.Rows[x + z]["MM Code"] = temp1.Rows[z]["MMCode"];
                        dtFinal.Rows[x + z]["MM Description"] = temp1.Rows[z]["MMDescription"];
                        dtFinal.Rows[x + z]["MS Code"] = temp1.Rows[z]["MSCode"];
                        dtFinal.Rows[x + z]["MS Description"] = temp1.Rows[z]["MSDescription"];
                    }
                    else
                    {
                        dtFinal.Rows[x + z]["  "] = temp1.Rows[z]["  "];
                        dtFinal.Rows[x + z]["   "] = temp1.Rows[z]["   "];
                    }
                }
            }
            dtFinal.Columns.Remove("Man Hours");
        }
        #endregion

        DataTable dtSource = dtFinal.Copy();
        HtmlGenericControl div1 = new HtmlGenericControl("div");
        div1.Controls.Add(AddGridViewControl(dtSource));
        div1.Attributes["style"] = "height:" + outerHeight.ToString() + ";";
        Panel1.Controls.Add(div1);
    }

    private Label AddLabelControl(string Text)
    {
        Label lbl = new Label();
        lbl.ID = "lblGrid_" + NumberOfControls.ToString();
        lbl.Text = Text;
        lbl.Font.Bold = true;
        lbl.Font.Italic = true;
        this.NumberOfControls++;
        return lbl;
    }
    private GridView AddGridViewControl(DataTable dtHours)
    {
        
        if (chkDefaultReport.Checked == true
            || (chkDefaultReport.Checked == false && rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S"))
            dtHours.Columns.Add("TOTAL");

        GridView grdv = new GridView();
        grdv.Attributes["runat"] = "server";

        grdv.CellPadding = 0;
        grdv.CellSpacing = 1;
        grdv.ID = "grdv_" + NumberOfControls.ToString();
        if (chkDefaultReport.Checked == false && rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S")
            grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
        
        grdv.DataSource = dtHours;
        grdv.DataBind();
        bool flagX = true;
        bool flagY = true;
        string className= string.Empty;

        int countNonBlank = 0;

        for (int i = 0, j =0; i < grdv.Rows.Count; i++ )
        {
            if (flagX)
            {
                flagX = false;
                if (flagY)
                {
                    className = "alternativeRowspanColored";
                    flagY = false;
                }
                else
                {
                    className = "alternativeRowspanWhite";
                    flagY = true;
                }
            }

            if (i > 0)
            {
                if (grdv.Rows[j].Cells[0].Text == grdv.Rows[i].Cells[0].Text)
                {
                    grdv.Rows[i].Cells[0].CssClass = className;
                    grdv.Rows[i].Cells[0].Text = "";
                }
                else
                {
                    j = i;
                    flagX = true;
                }
            }
            
            if (chkDefaultReport.Checked == false && !(rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S")) 
            {
                if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")   //?
                {
                    
                    grdv.Rows[i].Cells[2].Attributes.Add("class", "Nums");
                }
                else if (rblOption.SelectedValue == "C")
                {
                    grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;

                    if (grdv.Rows[i].Cells[0].Text != "&nbsp;" && grdv.Rows[i].Cells[0].Text != ""
                        && grdv.Rows[i].Cells[1].Text != "&nbsp;" && grdv.Rows[i].Cells[1].Text != "")
                    {
                        grdv.Rows[i].Cells[1].Font.Bold = true;
                        grdv.Rows[i].Cells[0].Font.Bold = true;
                        grdv.Rows[i].Cells[1].ColumnSpan = 2;
                        grdv.Rows[i].Cells.RemoveAt(2);

                        grdv.Rows[i].Cells[2].Attributes.Add("class", "text");
                        grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    }
                    else
                    {
                        grdv.Rows[i].Cells[3].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    }

                    grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Left;

                }
                else if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "S")
                {
                    for (int column = 3; column < grdv.Rows[i].Cells.Count; column++)
                    {
                        grdv.Rows[i].Cells[column].Attributes.Add("class", "Nums");
                    }

                    grdv.Rows[i].Cells[1].Attributes.Add("class", "text");
                }
                else if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D") 
                {
                    grdv.Rows[i].Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    grdv.Rows[i].Cells[3].Attributes.Add("class", "Nums");
                }
            }
            else if (chkDefaultReport.Checked == true
                || (chkDefaultReport.Checked == false && rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S"))
            {
                int colNumbersStart = 0;

                if (chkDefaultReport.Checked == false && rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S")
                    colNumbersStart = 1;
                else if (rblOption.SelectedValue == "E")    //Employee
                {
                    if (rbEmployeeOptions.SelectedValue == "D") // Details
                    {
                        grdv.Rows[i].Cells[2].Attributes.Add("class", "text");

                        if (chkSubWorkCode.Checked)
                            colNumbersStart = 4;
                        else
                            colNumbersStart = 3;
                    }
                    else if (rbEmployeeOptions.SelectedValue == "S") // Summary
                        colNumbersStart = 1;
                   
                }
                else if (rblOption.SelectedValue == "D")    // Department
                {
                    grdv.Rows[i].Cells[0].Attributes.Add("class", "text");
                    grdv.Rows[i].Cells[1].Attributes.Add("class", "text");

                    if (chkSubWorkCode.Checked)
                        colNumbersStart = 4;
                    else
                        colNumbersStart = 3;
                }
                else if (rblOption.SelectedValue == "C") // Client
                {
                    grdv.Rows[i].Cells[0].Attributes.Add("class", "text");
                    grdv.Rows[i].Cells[2].Attributes.Add("class", "text");

                    if (chkSubWorkCode.Checked)
                        colNumbersStart = 4;
                    else
                        colNumbersStart = 3;
                }

                for (int column = colNumbersStart; column < grdv.Rows[i].Cells.Count; column++)
                {
                    grdv.Rows[i].Cells[column].Attributes.Add("class", "Nums");
                }
            }
        }

        grdv.Attributes["Width"] = "auto";  //before value: auto

        
        if (chkDefaultReport.Checked == true
            || (chkDefaultReport.Checked == false && ((rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D")
                || (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S"))
                || (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")))
        { 
            grdv.FooterRow.Visible = true;

            int colNumberStart = 0;
            if (chkDefaultReport.Checked == false && (rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D"))
                colNumberStart = 3;
            else if (chkDefaultReport.Checked == false && (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D"))
                colNumberStart = 2;
            else if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "E")
            {
                if (rbEmployeeOptions.SelectedValue == "D")
                {
                    if (chkSubWorkCode.Checked)
                        colNumberStart = 4;
                    else
                        colNumberStart = 3;
                }
                else if (rbEmployeeOptions.SelectedValue == "S")
                    colNumberStart = 1;
            }
            else if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "D") // Department
            {
                if (chkSubWorkCode.Checked)
                    colNumberStart = 4;
                else
                    colNumberStart = 3;
            }
            else if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "C") // Client
            {
                if (chkSubWorkCode.Checked)
                    colNumberStart = 4;
                else
                    colNumberStart = 3;
            }

            for (int column = colNumberStart; column < grdv.FooterRow.Cells.Count; column++)
            {
                grdv.FooterRow.Cells[column].Attributes.Add("class", "Nums");
            }

        }
        grdv.FooterStyle.BackColor = ColorTranslator.FromHtml("#CCCC99");
        grdv.RowStyle.BackColor = ColorTranslator.FromHtml("#F7F7DE");
        grdv.HeaderStyle.BackColor = ColorTranslator.FromHtml("#6B696B");
        grdv.HeaderStyle.Font.Bold = true;
        grdv.HeaderStyle.ForeColor = Color.White;
        grdv.AlternatingRowStyle.BackColor = Color.White;

        #region [Total per column]
        // Default Report or Summary, Department and Monthly 
        if (chkDefaultReport.Checked == true 
            || (chkDefaultReport.Checked == false && ((rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D")
                || (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "S"))
                || (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")))
        {
            ListDictionary Totals = new ListDictionary();
            decimal Totalcols = 0;
            foreach (Control ctrl in grdv.Controls[0].Controls)
            {
                if (ctrl is GridViewRow)
                {
                    int x = 0;
                    if (!chkSubWorkCode.Checked)
                        x = 1;
                    int ctr = 0;
                    if (lastVal == "E")
                    {
                        if (rbEmployeeOptions.SelectedValue == "S")
                        {
                            ctr = 1;
                        }
                        else if (rbEmployeeOptions.SelectedValue == "D")
                        {
                            if (chkSubWorkCode.Checked)
                            {
                                ctr = 4;
                            }
                            else
                            {
                                ctr = 3;
                            }
                        }
                    }
                    else if (chkDefaultReport.Checked == false && rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D")
                    {
                        ctr = 3;
                    }
                    else if (chkDefaultReport.Checked == false && rbEmployeeOptions.SelectedValue == "D" && rblOption.SelectedValue == "D")
                    {
                        ctr = 2;
                    }
                    else
                        ctr = 4 - x;
                    CommonLookUp.SetGridViewCells((GridViewRow)ctrl, new ArrayList());

                    if (((GridViewRow)ctrl).RowType == DataControlRowType.Header)
                    {
                        for (int i = 5; i < ((GridViewRow)ctrl).Cells.Count; i++)
                        {
                            TableCell tc = ((GridViewRow)ctrl).Cells[i];
                        }
                    }
                    else if (((GridViewRow)ctrl).RowType == DataControlRowType.DataRow)
                    {
                        for (int i = ctr; i < ((GridViewRow)ctrl).Cells.Count; i++)
                        {

                            if (!(chkDefaultReport.Checked == false && ((rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D") 
                                || (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D"))))
                            {
                                if (i == ((GridViewRow)ctrl).Cells.Count - 1)
                                {
                                    ((GridViewRow)ctrl).Cells[i].Text = Totalcols.ToString();
                                    Totalcols = 0;
                                }
                            }

                            TableCell tc = ((GridViewRow)ctrl).Cells[i];
                            tc.HorizontalAlign = HorizontalAlign.Right;
                            try
                            {
                                decimal dec = 0;
                                if (tc.Text == "&nbsp;")
                                    dec = 0;
                                else
                                    dec = decimal.Parse(tc.Text);
                                if (i < ((GridViewRow)ctrl).Cells.Count - 1)
                                {
                                    Totalcols += dec;
                                }
                                if (Totals.Contains(i))
                                {
                                    Totals[i] = (decimal)Totals[i] + dec;
                                }
                                else
                                {
                                    Totals.Add(i, dec);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Response.Write(ex.ToString());
                            }
                        }

                    }
                    else if (((GridViewRow)ctrl).RowType == DataControlRowType.Footer)
                    {
                        TableRow tr = (GridViewRow)ctrl;
                        ((GridViewRow)ctrl).Cells[0].Text = "TOTAL "; //Row(s): " + grdv.Rows.Count.ToString();
                        tr.Cells[0].Wrap = false;
                        for (int i = ctr; i < tr.Cells.Count; i++)
                        {
                            try
                            {
                                TableCell tc = tr.Cells[i];
                                tc.HorizontalAlign = HorizontalAlign.Right;
                                tc.Text = string.Format("{0:0.00}", decimal.Parse(Totals[i].ToString()));
                                grandTotal += (decimal)Totals[i];
                            }
                            catch (Exception ex)
                            {
                                //
                            }
                        }
                    }
                }
            }
        #endregion
        }
        this.NumberOfControls++;
        if (rblOption.SelectedValue == "E")
            innerHeight = ((grdv.Rows.Count + 2) * 18) + 45;
        else
            innerHeight = ((grdv.Rows.Count + 2) * 18) + 18;
        outerHeight += innerHeight + 60;
        return (GridView)grdv;
    }
    #endregion

    protected int NumberOfControls
    {
        get { return (int)ViewState["NumControls"]; }
        set { ViewState["NumControls"] = value; }
    }
    protected void EnableDisableControls(object sender, EventArgs e)
    {
        if (rblOption.SelectedValue == "C")
        {
            ddlDateType.Enabled = true;
        }
        else
        {
            ddlDateType.Enabled = false;
        }
    }

    protected void grdView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        int j = 1;
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = j; i < e.Row.Cells.Count; i++)
            {
                TableCell tc = e.Row.Cells[i];
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.VerticalAlign = VerticalAlign.Top;
                //tc.Text = tc.Text.Insert(25, "mon");
            }
        }
    }

    protected void grdViewTotal_RowCreated(object sender, GridViewRowEventArgs e)
    {
       
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TableCell tc = e.Row.Cells[1];
            tc.HorizontalAlign = HorizontalAlign.Right;
            tc.VerticalAlign = VerticalAlign.Top;
            if(!print)
                tc.Width = Unit.Pixel(810);   
        }
    }

    private void setOptions()
    {
        switch (ddlReport.SelectedValue)
        {
            case "1":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "2":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "C";
                break;
            case "3":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "D";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "4":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "D";
                rbEmployeeOptions.SelectedValue = "S";
                break;
            case "5":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "D";
                break;
            case "6":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "W";
                break;
            case "7":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "M";
                break;
            case "8":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "9":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "D";
                break;
            case "10":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "D";
                break;
            case "11":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "W";
                break;
            case "12":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "M";
                break;
            case "13":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                break;
            default:
                break;
        }
    }

    #region para unta sa BL sa manhours
    private void PopulateDdlBilling()
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"  SELECT Adt_AccountDesc as [Description]
                                       , Adt_AccountCode as [Value]
                                    FROM T_AccountDetail
                                   WHERE Adt_AccountType = 'BILLCYCLE'
                                     AND Adt_Status = 'A'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
                ddlBilling.Items.Clear();
                ddlBilling.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddlBilling.Items.Add(new ListItem(dr["Description"].ToString(), dr["Value"].ToString()));
        }
    }


    private void getWeedStartandEnd()
    {
        string[] SandE = new string[3];
        string sql = @"
                    SELECT Convert(varchar(2),(ccd_weekcoverage - 1)) + '-' + Convert(varchar(2),(ccd_weekcoverage - 2))
                    from t_companymaster
                    ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                SandE[2] = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        SandE = SandE[2].Split('-');

        hWeekStart.Value = SandE[0].ToString();
        hWeekEnd.Value = SandE[1].ToString();
    }

    protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //int j = 2;
        //if (ddlManhourType.SelectedValue == "")
        //    j++;
        //if (chkSubWorkCode.Checked)
        //    j++;

        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    //Andre: commented out this lines below to display only the dates not including DoW or Day Code

        //    //for (int i = j; i < e.Row.Cells.Count; i++)
        //    //{
        //    //    TableCell tc = e.Row.Cells[i];
        //    //    tc.VerticalAlign = VerticalAlign.Top;
        //    //    int temp = int.Parse(tc.Text.Substring(26, 3));
        //    //    string day = "   ";
        //    //    switch (temp)
        //    //    {
        //    //        case 1: day = "Sun"; break;
        //    //        case 2: day = "Mon"; break;
        //    //        case 3: day = "Tue"; break;
        //    //        case 4: day = "Wed"; break;
        //    //        case 5: day = "Thu"; break;
        //    //        case 6: day = "Fri"; break;
        //    //        case 7: day = "Sat"; break;
        //    //        default: break;
        //    //    }
        //    //    tc.Text = tc.Text.Replace(tc.Text.Substring(26, 3), day);
        //    //}
        //}
        //else if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    for (int i = j; i < e.Row.Cells.Count; i++)
        //    {
        //        TableCell tc = e.Row.Cells[i];
        //        tc.HorizontalAlign = HorizontalAlign.Right;
        //        try
        //        {
        //            decimal dec = 0;
        //            if (tc.Text == "&nbsp;")
        //                dec = 0;
        //            else
        //                dec = decimal.Parse(tc.Text);
        //            if (Totals.Contains(i))
        //                Totals[i] = (decimal)Totals[i] + dec;
        //            else
        //                Totals.Add(i, dec);
        //        }
        //        catch (Exception ex)
        //        {
        //            Response.Write(ex.ToString());
        //        }
        //    }
        //}
        //else if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    TableRow tr = e.Row;
        //    tr.Cells[0].Text = "TOTALS";
        //    for (int i = j; i < tr.Cells.Count; i++)
        //    {
        //        TableCell tc = tr.Cells[i];
        //        tc.HorizontalAlign = HorizontalAlign.Right;
        //        tc.Text = string.Format("{0:0.00}", decimal.Parse(Totals[i].ToString()));
        //    }
        //}

        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            TableCell tc = e.Row.Cells[i];

            if (!tc.Text.Equals("&nbsp;"))
                tc.Text = HttpUtility.HtmlDecode(tc.Text);
            if (i == 3)   //4th column
                tc.HorizontalAlign = HorizontalAlign.Right;
        }
    }
    
    #endregion
}
