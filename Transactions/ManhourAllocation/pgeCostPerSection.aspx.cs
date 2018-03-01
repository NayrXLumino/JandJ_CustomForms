/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/23/2013
 *  Update Notes    :   
 *      -   Updated Reports for Correct Data
 */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxGridView.Export;
using System.Collections.Generic;
using MethodsLibrary;
using CommonLibrary;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System.IO;


public partial class Transactions_ManhourAllocation_pgeCostPerSection : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    private DataSet dsView;
    int innerHeight = 0;
    int outerHeight = 0;
    private decimal grandTotal = 0;
    private bool print = false;
    private List<string> categoryList;
    private List<string> categoryListCopy;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            if (!Page.IsCallback)
            {
                Response.Redirect("../../index.aspx?pr=dc");
            }
        }//WFCSTSECREP WFMANHOURREP
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFCSTSECREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            DALHelper dh = new DALHelper();
            SqlDataSource1.ConnectionString = Encrypt.decryptText(Session["dbConn"].ToString());
            // options will be removed
            //for (int i = 0; i < rblOption.Items.Count; i++)   
            //    rblOption.Items[i].Attributes.Add("oncLick", "return CheckPopUp();");
            SqlDataSource1.SelectCommand = query.Value;
            //getWeedStartandEnd();
            if (!Page.IsPostBack)
            {
                DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFCSTSECREP");
                if (dr != null)
                {
                    bool canCheck = Convert.ToBoolean(dr["Ugt_CanCheck"]);
                    bool canApprove = Convert.ToBoolean(dr["Ugt_CanApprove"]);

                    btnExport.Visible = Convert.ToBoolean(dr["Ugt_CanGenerate"]);
                    btnPrint.Visible = false;//Convert.ToBoolean(dr["Ugt_CanPrint"]);
                }

                //rbEmployeeOptions.SelectedIndex = 0;
                //rblCode.SelectedIndex = 0;
                //rblBillable.SelectedValue = "A";
                //ddlDateType.SelectedValue = "D";
                //ddlDateType.Enabled = false;
                this.NumberOfControls = 0;
                //ddlBilling.Enabled = false;
                dsView = null;
                btnClear_Click(null, null);
                initializeControls();
            }

            LoadComplete += new EventHandler(_Default_LoadComplete);
        }
    }

    

    #region Events

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
        btnCostCenter.OnClientClick = string.Format("return lookupRJSCostcenterMR('{0}')", false);
        //btnEmployee.OnClientClick = string.Format("return lookupRJSEmployeeMR('{0}')", false);
        //Calendar cal1 = (Calendar)dtpRangeFrom.Controls[3];
        //cal1.Attributes.Add("OnClick", "javascript:return checkRange('from')");

        //Calendar cal2 = (Calendar)dtpRangeTo.Controls[3];
        //cal2.Attributes.Add("OnClick", "javascript:return checkRange('to')");

        //ddlDateType.Attributes.Add("OnChange", "return CheckPopUp();");

        //ddlBilling.Attributes.Add("OnChange", "javascript:return checkRange('selection')");

        //chkDefaultReport.Attributes.Add("OnClick", "javascript:checkDefaultReport();");
        //chkSubWorkCode.Attributes.Add("OnClick", "javascript:CheckWorkCode();");

        //ddlReport.Attributes.Add("OnChange", "javascript:hideOptions()");
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        dsView = null;
        Panel1.Controls.Clear();
        txtDashJobCode.Text = "";
        //txtClientCode.Text = "";
        //txtClientJobNo.Text = "";
        //txtFWBSCode.Text = "";
        //txtEmpName.Text = "";
        //txtCostCenter.Text = "";
        //txtWorkCode.Text = "";
        //txtSubWorkCode.Text = "";
        //rblOption.SelectedValue = "E";
        //rblBillable.SelectedValue = "A";
        //rbEmployeeOptions.SelectedValue = "S";
        //cbUnsplitted.Checked = false;
        //chkDefaultReport.Checked = true;
        //chkSubWorkCode.Checked = true;
        //ddlDateType.SelectedValue = "D";
        //ddlBilling.SelectedValue = "";
        dtpRangeFrom.Date = CommonMethods.getMinimumDateOfFiling();
        dtpRangeTo.Date = DateTime.Now;
        
        this.NumberOfControls = 0;
        //ddlReport.SelectedValue = "";
        btnTwist.Visible = false;
        pnlResult.Visible = false;
        query.Value = "";
    }

    protected void btnExport_Click(object sender, EventArgs e)
    { //btnGenerate_Click(sender, e);
        if (CostPerProjectGrid.RowCount > 1)
        {
          
        
        string fileName = "Man_Hour_Cost_Per_Section";
               // ASPxPivotGridExporter1.ExportXlsToResponse(fileName, false);
                Link header = new Link();
                header.CreateDetailArea += new CreateAreaEventHandler(header_CreateDetailArea);

                Link space = new Link();
                space.CreateDetailArea += new CreateAreaEventHandler(space_CreateDetailArea);

                PrintingSystem ps = new PrintingSystem();

                PrintableComponentLink linkDetailReport = new PrintableComponentLink();
                linkDetailReport.Component = ASPxPivotGridExporter1;
                linkDetailReport.PrintingSystem = ps;

                CompositeLink compositeLink = new CompositeLink();
                compositeLink.Links.AddRange(new object[] { header, space, linkDetailReport });
                compositeLink.PrintingSystem = ps;
                compositeLink.CreateDocument();

                using (MemoryStream stream = new MemoryStream())
                {
                    compositeLink.PrintingSystem.ExportToXls(stream);
                    Response.Clear();
                    Response.Buffer = false;
                    Response.AppendHeader("Content-Type", "application/xls");
                    Response.AppendHeader("Content-Transfer-Encoding", "binary");
                    Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));//Page.Title.Replace(' ', '_') + "-" + DateTime.Now));
                    Response.BinaryWrite(stream.GetBuffer());
                    Response.End();
                }
                ps.Dispose();
        }
        else
        {
            CostPerProjectGrid.Visible = false;
            Panel1.Visible = false;
            pnlResult.Visible = false;
            //Table1.Visible = true;
            MessageBox.Show("No Data Found");
        }
        
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        SqlDataSource1.SelectCommand = query.Value = string.Format(@"SELECT Jsd_CostCenter
		                                    ,dbo.getCostCenterFullNameV2(Jsd_CostCenter)[COSTCENTERNAME]
		                                    ,Jsd_JobCode
		                                    ,Slm_ClientJobName
		                                    ,sum(Jsd_ActHours)[TOTAL]
		

                                    FROM T_EmployeeMaster
	                                    left join T_JobSplitHeader
											on Jsh_EmployeeId=Emt_EmployeeID
											--and  Emt_JobStatus <> 'IN'
	                                    left join T_JobSplitDetail
											on Jsh_ControlNo=Jsd_ControlNo
	                                    left join T_SalesMaster
											on Jsd_JobCode= Slm_DashJobCode
											and Slm_Status='A'
										
	                                    --where Jsd_JobCode='AIKW-001'
                                        where 1=1  AND Jsh_Status = '9' {0}
	                                    --and (CONVERT(CHAR(4), Jsh_JobSplitDate, 100) + CONVERT(CHAR(4), Jsh_JobSplitDate, 120)) ='Dec 2012'
	                                    --where Emt_EmployeeID='1246'
                                    GROUP BY Jsd_CostCenter,Jsd_JobCode,Slm_ClientJobName
	

                                    HAVING Jsd_JobCode is not null AND sum(Jsd_ActHours)!=0.00

                                    ORDER BY Jsd_CostCenter,Jsd_JobCode,Slm_ClientJobName", SetFilters()); 
        
        //ASPxGridView1.Visible = false;
        CostPerProjectGrid.DataBind();
        if (checkData())//CostPerProjectGrid.RowCount > 0 && CostPerProjectGrid.ColumnCount!=1)
        {
            CostPerProjectGrid.Visible = true;
            Panel1.Visible = false;
            pnlResult.Visible = true;
        }
        else
        {
            CostPerProjectGrid.Visible = false;
            Panel1.Visible = false;
            pnlResult.Visible = false;
            //Table1.Visible = true;
            MessageBox.Show("No Data Found");
        }
    }

    private Boolean checkData()
    {
        Boolean ok = true;
        using (DALHelper dh = new DALHelper())
        {
            DataSet ds = new DataSet();
            try
            {
                dh.OpenDB();

                ds = dh.ExecuteDataSet(query.Value);
                //ds = dh.ExecuteNonQuery(query.Value);
                dh.CloseDB();
            }
            catch
            {
            }
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 0)
            {
                ok = false;
            }
        }
        return ok;
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //btnGenerate_Click(sender, e);
        if (CostPerProjectGrid.RowCount > 1)
        {
        
        string fileName = "Man_Hour_Cost_Per_Section";

        //ASPxPivotGridExporter1.ExportPdfToResponse(fileName, false);
        //ASPxPivotGridExporter1.ExportToPdf(fileName, false);
        Link header = new Link();
        header.CreateDetailArea += new CreateAreaEventHandler(header_CreateDetailArea);

        Link space = new Link();
        space.CreateDetailArea += new CreateAreaEventHandler(space_CreateDetailArea);

        PrintingSystem ps = new PrintingSystem();

        PrintableComponentLink linkDetailReport = new PrintableComponentLink();
        linkDetailReport.Component = ASPxPivotGridExporter1;
        linkDetailReport.PrintingSystem = ps;

        CompositeLink compositeLink = new CompositeLink();
        compositeLink.Links.AddRange(new object[] { header, space, linkDetailReport });
        compositeLink.PrintingSystem = ps;
        compositeLink.CreateDocument();
        using (MemoryStream stream = new MemoryStream())
        {
            compositeLink.PrintingSystem.ExportToPdf(stream);
            Response.Clear();
            Response.Buffer = false;
            Response.AppendHeader("Content-Type", "application/pdf");
            Response.AppendHeader("Content-Transfer-Encoding", "binary");
            Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.pdf", Page.Title.Replace(' ', '_') + "-" + DateTime.Now));
            Response.BinaryWrite(stream.GetBuffer());
            Response.End();
        }
        ps.Dispose();
        
        }
        else
        {
            CostPerProjectGrid.Visible = false;
            Panel1.Visible = false;
            pnlResult.Visible = false;
            //Table1.Visible = true;
            MessageBox.Show("No Data Found");
        }

        
    }

    decimal total;
    

    #endregion

    #region Properties

    protected int NumberOfControls
    {
        get { return (int)ViewState["NumControls"]; }
        set { ViewState["NumControls"] = value; }
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

    #endregion

    #region Methods

    

    private void InitializeButtons()
    {
        hfUserCostCenters.Value = "";
        using (DataTable dt = CommonLookUp.GetUserCostCenterCode(Session["userId"].ToString(), "TIMEKEEP"))
        {
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    hfUserCostCenters.Value += string.Format("x{0}xy", dr[0]);
                }
                btnDashJobCode.OnClientClick = string.Format("return OpenPopupLookupSales('S1','txtDashJobCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                //btnClientJobNo.OnClientClick = string.Format("return OpenPopupLookupSales('S2','txtClientJobNo','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                //btnCLientCode.OnClientClick = string.Format("return OpenPopupLookupSales('S3','txtClientCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                //btnClientFWBS.OnClientClick = string.Format("return OpenPopupLookupSales('S4','txtFWBSCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                //btnDashWorkCode.OnClientClick = string.Format("return OpenPopupLookupSales('S6','txtWorkCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                //btnCostCenter.OnClientClick = string.Format("return lookupRJSCostcenterMR('{0}')", cbUnsplitted.Visible);
                //btnEmployee.OnClientClick = string.Format("return lookupRJSEmployeeMR('{0}')", cbUnsplitted.Visible);
                //btnSubWork.OnClientClick = string.Format("javascript:return lookupRJSSubwork()");
                //btnCostCenter.OnClientClick = string.Format("return lookupRJSCostcenterMR('{0}')", cbUnsplitted.Visible);
            }
            else
            {
                hfUserCostCenters.Value = "xNo Cost Center Accessxy";
            }
        }
    }

    private void initializeControls()
    {
        //dtpRangeFrom.Date = CommonMethods.getQuincenaDate('C', "START");
        //dtpRangeFrom.MinDate = CommonMethods.getMinimumDate();
        //dtpRangeFrom.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        //dtpRangeTo.Date = CommonMethods.getQuincenaDate('C', "END");
        //dtpRangeTo.MinDate = CommonMethods.getMinimumDate();
        //dtpRangeTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");

        InitializeButtons();
        initializeProfile();
        //PopulateDdlBilling();
    }

    private string initializeHeader()
    {
        string options = "";
        
        return options.Trim();
    }

    private void initializeProfile()
    {
        //using (DataTable dt = CommonMethods.GetProfileAccess(Session["userLogged"].ToString()))
        //{
        //    foreach (DataRow drProfile in dt.Rows)
        //    {
        //        ListItem li = new ListItem(drProfile["Prf_Profile"].ToString(), drProfile["Prf_Database"].ToString());
        //        li.Selected = true;
        //        cblProfile.Items.Add(li);
        //    }
        //}
    }

    private bool isProfileSelected()
    {
    
        return false;
    }

    #region para unta sa BL sa manhours
    
    #endregion

    private string SetFilters()
    {
        StringBuilder filters = new StringBuilder();

        #region Cost Center Access
        if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
        {
            string temp = hfUserCostCenters.Value;
            temp = temp.Replace("x", "'").Replace("y", ",");
            filters.Append("\nAND Jsd_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
        }
        if (txtCostCenter.Text != "")
        {
            string temp = txtCostCenter.Text.ToString();
            temp = "'" + temp.Replace(",", "','")+"'";
            filters.Append("\nAND ( Jsd_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\nOR LEFT(Jsd_CostCenter, 2) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\nOR LEFT(Jsd_CostCenter, 4) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\nOR LEFT(Jsd_CostCenter, 6) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\nOR LEFT(Jsd_CostCenter, 8) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append(" ) ");
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
                    dashJobCode += string.Format("\nOR Jsd_Jobcode like '{0}%'", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(dashJobCode + ")");
            }
        }

        //if (txtClientJobNo.Text != "")
        //{
        //    string text = txtClientJobNo.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string clientJobNo = "";
        //        for (int i = 0; i < arr.Count; i++)
        //            clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(clientJobNo + ")");
        //    }
        //}
        //if (txtClientCode.Text != "")
        //{
        //    string text = txtClientCode.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);

        //    if (arr.Count > 0)
        //    {
        //        string clientCode = "", clientName = "", clientShortName = "";
        //        for (int i = 0; i < arr.Count; i++)
        //        {
        //            clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
        //            clientName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
        //            clientShortName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
        //        }
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(clientCode);
        //        filters.Append(clientName);
        //        filters.Append(clientShortName + ")");
        //    }
        //}

        //if (txtEmpName.Text != string.Empty)
        //{
        //    string text = txtEmpName.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string employeeId = "", employeeName = "";
        //        for (int i = 0; i < arr.Count; i++)
        //        {
        //            employeeId += string.Format("\nOR Emt_EmployeeId like '{0}%'", arr[i].ToString().Trim());
        //            employeeName += string.Format("\nOR Emt_LastName like '{0}%' OR Emt_FirstName like '{0}%'", arr[i].ToString().Trim());
        //        }
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(employeeId);
        //        filters.Append(employeeName + ")");
        //    }
        //}

        //if (txtCostCenter.Text != "")
        //{
        //    string text = txtCostCenter.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string costCenterCode = "", costCenterName = "";
        //        for (int i = 0; i < arr.Count; i++)
        //        {
        //            costCenterCode += string.Format("\nOR Jsd_Costcenter like ''{0}%''", arr[i].ToString().Trim());
        //            costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsd_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
        //        }
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(costCenterCode);
        //        filters.Append(costCenterName + ")");
        //    }
        //}

        //if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
        //{
        //    string text = txtSubWorkCode.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string SubWorkCode = "";
        //        for (int i = 0; i < arr.Count; i++)
        //            SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(SubWorkCode + ")");
        //    }
        //}

        //if (txtFWBSCode.Text != "")
        //{
        //    string text = txtFWBSCode.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string FWBSCode = "";
        //        for (int i = 0; i < arr.Count; i++)
        //            FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like ''{0}%''", arr[i].ToString().Trim());
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(FWBSCode + ")");
        //    }
        //}

        //if (txtWorkCode.Text != "")
        //{
        //    string text = txtWorkCode.Text.Replace("'", "`");
        //    ArrayList arr = CommonLookUp.DivideString(text);
        //    if (arr.Count > 0)
        //    {
        //        string dashWorkCode = "";
        //        for (int i = 0; i < arr.Count; i++)
        //            dashWorkCode += string.Format("\nOR Slm_DashWorkCode like ''{0}%''", arr[i].ToString().Trim());
        //        filters.Append("\nAND (");
        //        filters.Append("\n 1 = 0");
        //        filters.Append(dashWorkCode + ")");
        //    }
        //}

        #endregion

        

        #region Date Time Pickers
        if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
        {
            if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN '{0:d}' AND '{1:d}') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                 , Convert.ToDateTime(dtpRangeTo.Date)));
            }
            else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= '{0:d}' ", Convert.ToDateTime(dtpRangeFrom.Date)));
            }
            else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= '{0:d}' ", Convert.ToDateTime(dtpRangeTo.Date)));
            }
        }
        #endregion

        return filters.ToString();
    }


    #endregion

    #region Custom Methods
    

    

    
    void space_CreateDetailArea(object sender, CreateAreaEventArgs e)
    {
        e.Graph.DrawEmptyBrick(new RectangleF(0, 0, 0, 20));
    }
    void header_CreateDetailArea(object sender, CreateAreaEventArgs e)
    {
        //string reportType = PatternSelection.SelectedItem.Text.ToString() + " " + lookupGrid_Metrics.Text.ToString() + "(" + lookupGrid_UnitOfMeasure.SelectedItem.Text.ToString() + ")" + " " + "Report";
        string reportDate = "Date: " + DateTime.Now;
        e.Graph.BorderWidth = 0;
        //Methods.GetCompanyInfoERP("Scm_CompName") + "\r\n\r\n"
        //                             + Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
        //                             + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
        //                             + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
        //                             + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos");
        Rectangle r = new Rectangle(0, 0, 700, 20);
        e.Graph.DrawString(Methods.GetCompanyInfoERP("Scm_CompName").ToString(), r);

        r = new Rectangle(0, 20, 700, 60);
        e.Graph.DrawString(Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
                                     + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
                                     + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
                                     + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos"), r);

        //r = new Rectangle(0, 40, 700, 20);
        //e.Graph.DrawString(this.Page.Title, r);

        //r = new Rectangle(0, 60, 700, 20);
        //e.Graph.DrawString(reportDate, r);

        //r = new Rectangle(0, 80, 0, 20);
        //e.Graph.DrawEmptyBrick(r);
    }
    #endregion
}
