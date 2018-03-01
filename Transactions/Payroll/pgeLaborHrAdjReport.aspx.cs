using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
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
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxGridView.Export;
using DevExpress.Web.ASPxGridView.Export.Helper;
using System.IO;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;


public partial class Transactions_Payroll_pgeLaborHrAdjReport : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();
    OvertimeBL OTBL = new OvertimeBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        
         //agvResult.TotalSummary.Add(new DevExpress.Web.ASPxGridView.ASPxSummaryItem() { FieldName = name, SummaryType = DevExpress.Data.SummaryItemType.Sum, DisplayFormat = "Total {1} {0}" });
             //agvResult.TotalSummary.Add()   

        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }//WFOTREP//WFLBRADJREP
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFLBRADJREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
            PreRender += new EventHandler(Transactions_Overtime_pgeOvertimeReport_PreRender);
        }
        LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeReport_LoadComplete);
    }

    #region Events
    void Transactions_Overtime_pgeOvertimeReport_LoadComplete(object sender, EventArgs e)
    {
        //string jsname = "overtimeScripts";
        //string jsurl = "_overtime.js";
        //Type jsType = this.GetType();
        //ClientScriptManager js = Page.ClientScript;
        //if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        //{
        //    js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        //}
        string jsname = "payrollScripts";
        string jsurl = "_payroll.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupPREmployee()");
        btnStatus.OnClientClick = string.Format("return lookupROTStatus()");
        btnCostcenter.OnClientClick = string.Format("return lookupPRCostcenter()");
        btnPayPeriod.OnClientClick = string.Format("return lookupPSPayPeriod()");
        btnBatchNo.OnClientClick = string.Format("return lookupROTBatchNo()");
        btnChecker1.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_CheckedBy','txtChecker1')");
        btnChecker2.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_Checked2By','txtChecker2')");
        btnApprover.OnClientClick = string.Format("return lookupROTCheckerApprover('Eot_ApprovedBy','txtApprover')");
    }

    void Transactions_Overtime_pgeOvertimeReport_PreRender(object sender, EventArgs e)
    {

    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFLBRADJREP", txtEmployee.Text, true, Session["userLogged"].ToString());
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        //CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
        GridViewRow gvr = e.Row;
        ArrayList arr=null;
        if (arr == null)
            arr = new ArrayList();
        if (gvr.RowType == DataControlRowType.Header)
        {
            System.Web.UI.WebControls.TableRow thr = gvr;
            for (int i = 0; i < thr.Cells.Count; i++)
            {
                thr.Cells[i].Wrap = false;
                thr.Cells[i].Text = HttpUtility.HtmlDecode(thr.Cells[i].Text);
                thr.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                thr.Cells[i].Attributes.Add("Width", "auto");
            }

        }
        else if (gvr.RowType == DataControlRowType.DataRow)
        {
            System.Web.UI.WebControls.TableRow tr = gvr;
            for (int i = 0; i < tr.Cells.Count; i++)
            {
                tr.Cells[i].Wrap = false;
                if (!tr.Cells[i].Text.Equals("&nbsp;"))
                    tr.Cells[i].Text = HttpUtility.HtmlDecode(tr.Cells[i].Text);
                tr.Cells[i].Attributes.Add("Width", "100%");
                try
                {
                    //if (arr != null && arr.Contains(i))
                        tr.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
                catch (Exception)
                { }
                finally
                { }

            }
        }
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        bindGrid();
        UpdatePagerLocation();
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string s = SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString());
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nick Name");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    //ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nick Name");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Last Name");
                    ds.Tables[0].Columns.Remove("First Name");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //DASH Specific
                //ds.Tables[0].Columns.Remove("Job Code");
                //ds.Tables[0].Columns.Remove("Client Job Name");
                //ds.Tables[0].Columns.Remove("Client Job No");
                //ds.Tables[0].Columns.Remove("DASH Class Code");
                //ds.Tables[0].Columns.Remove("DASH Work Code");
                ////Depending if Used
                //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                for (int trav = 0; trav < ds.Tables[0].Columns.Count; )
                {
                    bool indicator = false;
                    //string s = ds.Tables[0].Columns[trav].ToString();
                    for (int travRow = 0; travRow < ds.Tables[0].Rows.Count && !indicator; travRow++)
                    {
                        if (ds.Tables[0].Rows[travRow][trav] != "")
                            indicator = true;
                    }
                    if (!indicator)
                        ds.Tables[0].Columns.Remove(ds.Tables[0].Columns[trav].ColumnName);
                    else
                        trav++;
                }
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    agvResult.DataSource = ds;
                    agvResult.DataBind();
                    //ASPxGridView tempAspxGridView = new ASPxGridView();
                    //tempAspxGridView.DataSource = ds;
                    //tempAspxGridView.DataBind();
                    grivRight(this.agvResult);
                    //Control[] ctrl = new Control[2];
                    //ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "ADJUSTMENT REPORT", initializeExcelHeader());
                    //ctrl[1] = tempAspxGridView;
                    //ExportExcelHelper.ExportControl2(ctrl, "Adjustment Report");
                    ExportDevExGridReport(ASPxGridViewExporter1, agvResult, null, "XLS", "", null);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }
    public void ExportDevExGridReport(
                    ASPxGridViewExporter ASPxGridViewExporter1
                    , DevExpress.Web.ASPxGridView.ASPxGridView aspGridView
                    , System.Web.UI.WebControls.SqlDataSource sqlGridDatasource
                    , string format
                    , string sqlDataQuery
                    , DataTable dt)
    {
        if (sqlDataQuery.Trim() != string.Empty)
        {
            sqlGridDatasource.SelectCommand = sqlDataQuery;
        }
        if (dt != null)
        {
            aspGridView.DataSource = dt;
        }
        System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
        ASPxGridViewExporter1.FileName = currentPage.Title + "-" + DateTime.Now.ToString("MM_dd_yyyy");

        if (aspGridView.VisibleRowCount > 0)
        {
            #region Export
            PrintingSystem ps = new PrintingSystem();

            Link header = new Link();
            header.CreateDetailArea += new CreateAreaEventHandler(header_CreateDetailAreaNoLogo);

            GridViewLink link1 = new GridViewLink(ASPxGridViewExporter1);

            CompositeLink compositeLink = new CompositeLink(ps);
            compositeLink.Links.AddRange(new object[] { header, link1 });
            compositeLink.PaperKind = ASPxGridViewExporter1.PaperKind;
            compositeLink.Landscape = ASPxGridViewExporter1.Landscape;
            compositeLink.Margins = new System.Drawing.Printing.Margins(
                (ASPxGridViewExporter1.LeftMargin < 0) ? 0 : ASPxGridViewExporter1.LeftMargin,
                (ASPxGridViewExporter1.RightMargin < 0) ? 0 : ASPxGridViewExporter1.RightMargin,
                (ASPxGridViewExporter1.TopMargin < 0) ? 0 : ASPxGridViewExporter1.TopMargin,
                (ASPxGridViewExporter1.BottomMargin < 0) ? 0 : ASPxGridViewExporter1.BottomMargin);

            compositeLink.CreateDocument();

            using (MemoryStream msExport = new MemoryStream())
            {
                
                    try
                    {
                        compositeLink.PrintingSystem.ExportToXls(msExport);
                        WriteToResponse(ASPxGridViewExporter1.FileName, true, "xls", msExport);
                    }
                    catch
                    {
                        //Try Xlsx
                        compositeLink.PrintingSystem.ExportToXlsx(msExport);
                        WriteToResponse(ASPxGridViewExporter1.FileName, true, "xlsx", msExport);
                    }
            }
            #endregion
        }
    }
    private void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, MemoryStream stream)
    {
        try
        {
            System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

            if (currentPage == null || currentPage.Response == null)
                return;
            string disposition = saveAsFile ? "attachment" : "inline";
            currentPage.Response.Clear();
            currentPage.Response.Buffer = false;
            currentPage.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat));
            currentPage.Response.AppendHeader("Content-Transfer-Encoding", "binary");
            currentPage.Response.AppendHeader("Content-Disposition",
                string.Format("{0}; filename={1}.{2}", disposition, fileName, fileFormat));
            currentPage.Response.BinaryWrite(stream.GetBuffer());
            currentPage.Response.End();
        }
        catch
        {

        }

    }
    private  void header_CreateDetailAreaNoLogo(object sender, CreateAreaEventArgs e)
    {

        string[] companyDetails = GetCompanyNameandAddress();
        

        e.Graph.BorderWidth = 0;

        Rectangle r = new Rectangle(0, 0, 700, 20);
        e.Graph.DrawString(companyDetails[0], r);

        r = new Rectangle(0, 20, 700, 20);
        e.Graph.DrawString(companyDetails[1], r);

        System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
        r = new Rectangle(0, 40, 700, 20);
        e.Graph.DrawString(currentPage.Title, r);

        r = new Rectangle(0, 60, 0, 20);
        e.Graph.DrawEmptyBrick(r);


    }

    private string[] GetCompanyNameandAddress()
    {
        string[] ret = new string[2];
        ret[0] = "";
        ret[1] = "";
         using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = null;
                        #region HRC
                        ds = dal.ExecuteDataSet(@"
SELECT 
	Ccd_CompanyName
	, Ccd_CompanyAddress1
		+ CASE WHEN RTRIM(Ccd_CompanyAddress2) = '' THEN ' ' + RTRIM(Ccd_CompanyAddress2) + ' ' ELSE ' ' END
		+ ISNULL(Adt_AccountDesc, Ccd_CompanyAddress3)
FROM T_CompanyMaster
LEFT JOIN T_AccountDetail
	ON Adt_AccountType = 'ZIPCODE'
	AND Adt_AccountCode = Ccd_CompanyAddress3
                        ");
                        #endregion
                    
                   
                    ret[0] = ds.Tables[0].Rows[0][0].ToString().Trim().ToUpper();
                    ret[1] = ds.Tables[0].Rows[0][1].ToString().Trim().ToUpper();
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
        

        return ret;
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //DASH Specific
                ds.Tables[0].Columns.Remove("Job Code");
                ds.Tables[0].Columns.Remove("Client Job Name");
                ds.Tables[0].Columns.Remove("Client Job No");
                ds.Tables[0].Columns.Remove("DASH Class Code");
                ds.Tables[0].Columns.Remove("DASH Work Code");
                //Depending if Used
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
                ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "OVERTIME REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        btnGenerate.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        //WFLBRADJREP//WFOTREP
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFLBRADJREP");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);

            txtEmployee.Text = Session["userLogged"].ToString();
            tbrEmployee.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
        }
        else
        {
            btnExport.Enabled = false;
            btnPrint.Enabled = false;
        }

    }

    private string SQLBuilder(string replaceString)
    {
        #region Query
        string query = @"SELECT  Emt_EmployeeId[ID],
                            Emt_NickName [Nick Name]
                          , Emt_Lastname [Last Name]
                          , Emt_Firstname [First Name]
      ,'SYSTEM' AS [Type]
      , A.Lha_PayPeriod as [Pay Period]
      , Convert(varchar(10), A.Lha_ProcessDate,101) as [Date]
	  , CASE WHEN Lha_AbsentHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentHr as MONEY),1) END as [Absent Hr]
      , CASE WHEN Lha_AbsentLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentLegalHolidayHr as MONEY),1) END as [Absent HOL Hr]
      , CASE WHEN Lha_AbsentSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentSpecialHolidayHr as MONEY),1) END as [Absent SPL Hr]
      , CASE WHEN Lha_AbsentCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentCompanyHolidayHr as MONEY),1) END as [Absent COMP Hr]
      , CASE WHEN Lha_AbsentPlantShutdownHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentPlantShutdownHr as MONEY),1) END as [Absent PSD Hr]      
      , CASE WHEN Lha_AbsentFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentFillerHolidayHr as MONEY),1) END as [Absent Other HOL Hr]
      , CASE WHEN Lha_PaidLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidLegalHolidayHr as MONEY),1) END as [Unwork HOL Hr]
      , CASE WHEN Lha_PaidSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidSpecialHolidayHr as MONEY),1) END as [Unwork SPL Hr]
      , CASE WHEN Lha_PaidCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidCompanyHolidayHr as MONEY),1) END as [Unwork COMP Hr]
      , CASE WHEN Lha_PaidFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidFillerHolidayHr as MONEY),1) END as [Unwork Other HOL Hr]
      , CASE WHEN Lha_RegularHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularHr as MONEY),1) END as [Regular Hr]
      , CASE WHEN Lha_RegularOTHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularOTHr as MONEY),1) END as [OT Hr]
      , CASE WHEN (Lha_RestdayHr + Lha_RestdayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RestdayHr + Lha_RestdayOTHr as MONEY),1) END as [REST Hr]
	  , CASE WHEN (Lha_LegalHolidayHr + Lha_LegalHolidayOTHr + Lha_RestdayLegalHolidayHr + Lha_RestdayLegalHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LegalHolidayHr + Lha_LegalHolidayOTHr + Lha_RestdayLegalHolidayHr + Lha_RestdayLegalHolidayOTHr as MONEY),1) END as [HOL Hr]
	  , CASE WHEN (Lha_SpecialHolidayHr + Lha_SpecialHolidayOTHr + Lha_RestdaySpecialHolidayHr + Lha_RestdaySpecialHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_SpecialHolidayHr + Lha_SpecialHolidayOTHr + Lha_RestdaySpecialHolidayHr + Lha_RestdaySpecialHolidayOTHr as MONEY),1) END as [SPL Hr]
	  , CASE WHEN (Lha_CompanyHolidayHr + Lha_CompanyHolidayOTHr + Lha_RestdayCompanyHolidayHr + Lha_RestdayCompanyHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_CompanyHolidayHr + Lha_CompanyHolidayOTHr + Lha_RestdayCompanyHolidayHr + Lha_RestdayCompanyHolidayOTHr as MONEY),1) END as [COMP Hr]
	  , CASE WHEN (Lha_PlantShutdownHr + Lha_PlantShutdownOTHr + Lha_RestdayPlantShutdownHr + Lha_RestdayPlantShutdownOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PlantShutdownHr + Lha_PlantShutdownOTHr + Lha_RestdayPlantShutdownHr + Lha_RestdayPlantShutdownOTHr as MONEY),1) END as [PSD Hr]
	  , CASE WHEN ISNULL(Lha_Filler01_Hr + Lha_Filler01_OTHr
				+ Lha_Filler02_Hr + Lha_Filler02_OTHr
				+ Lha_Filler03_Hr + Lha_Filler03_OTHr
				+ Lha_Filler04_Hr + Lha_Filler04_OTHr
				+ Lha_Filler05_Hr + Lha_Filler05_OTHr
				+ Lha_Filler06_Hr + Lha_Filler06_OTHr, 0)=0 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_Filler01_Hr + Lha_Filler01_OTHr
				+ Lha_Filler02_Hr + Lha_Filler02_OTHr
				+ Lha_Filler03_Hr + Lha_Filler03_OTHr
				+ Lha_Filler04_Hr + Lha_Filler04_OTHr
				+ Lha_Filler05_Hr + Lha_Filler05_OTHr
				+ Lha_Filler06_Hr + Lha_Filler06_OTHr as MONEY),1) END as [Other HOL Hr]
	  , CASE WHEN (Lha_RegularNDHr + Lha_RegularOTNDHr
		+ Lha_RestdayNDHr + Lha_RestdayOTNDHr
		+ Lha_LegalHolidayNDHr + Lha_LegalHolidayOTNDHr
		+ Lha_SpecialHolidayNDHr + Lha_SpecialHolidayOTNDHr
		+ Lha_PlantShutdownNDHr + Lha_PlantShutdownOTNDHr
		+ Lha_CompanyHolidayNDHr + Lha_CompanyHolidayOTNDHr
		+ Lha_RestdayLegalHolidayNDHr + Lha_RestdayLegalHolidayOTNDHr
		+ Lha_RestdaySpecialHolidayNDHr + Lha_RestdaySpecialHolidayOTNDHr
		+ Lha_RestdayPlantShutdownNDHr + Lha_RestdayPlantShutdownOTNDHr
		+ Lha_RestdayCompanyHolidayNDHr + Lha_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Lha_Filler01_NDHr + Lha_Filler01_OTNDHr
				+ Lha_Filler02_NDHr + Lha_Filler02_OTNDHr
				+ Lha_Filler03_NDHr + Lha_Filler03_OTNDHr
				+ Lha_Filler04_NDHr + Lha_Filler04_OTNDHr
				+ Lha_Filler05_NDHr + Lha_Filler05_OTNDHr
				+ Lha_Filler06_NDHr + Lha_Filler06_OTNDHr, 0))=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularNDHr + Lha_RegularOTNDHr
		+ Lha_RestdayNDHr + Lha_RestdayOTNDHr
		+ Lha_LegalHolidayNDHr + Lha_LegalHolidayOTNDHr
		+ Lha_SpecialHolidayNDHr + Lha_SpecialHolidayOTNDHr
		+ Lha_PlantShutdownNDHr + Lha_PlantShutdownOTNDHr
		+ Lha_CompanyHolidayNDHr + Lha_CompanyHolidayOTNDHr
		+ Lha_RestdayLegalHolidayNDHr + Lha_RestdayLegalHolidayOTNDHr
		+ Lha_RestdaySpecialHolidayNDHr + Lha_RestdaySpecialHolidayOTNDHr
		+ Lha_RestdayPlantShutdownNDHr + Lha_RestdayPlantShutdownOTNDHr
		+ Lha_RestdayCompanyHolidayNDHr + Lha_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Lha_Filler01_NDHr + Lha_Filler01_OTNDHr
				+ Lha_Filler02_NDHr + Lha_Filler02_OTNDHr
				+ Lha_Filler03_NDHr + Lha_Filler03_OTNDHr
				+ Lha_Filler04_NDHr + Lha_Filler04_OTNDHr
				+ Lha_Filler05_NDHr + Lha_Filler05_OTNDHr
				+ Lha_Filler06_NDHr + Lha_Filler06_OTNDHr,0) as MONEY),1) END as [REST Hr]
      --, CASE WHEN Lha_SalaryRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_SalaryRate as MONEY),1) END as [Salary Rate]
	  , CASE WHEN Lha_HourlyRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_HourlyRate as MONEY),1) END as [Hourly Rate]
      , CASE WHEN (Lha_LaborHrsAdjustmentAmt - Lha_OvertimeAdjustmentAmt)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LaborHrsAdjustmentAmt - Lha_OvertimeAdjustmentAmt as MONEY),1) END as [Regular Adj Amt]
      , CASE WHEN Lha_OvertimeAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_OvertimeAdjustmentAmt as MONEY),1) END as [OT Adj Amt]
	  , CASE WHEN Lha_LaborHrsAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LaborHrsAdjustmentAmt as MONEY),1) END as [Labor Hrs Adj Amt]

  FROM T_LaborHrsAdjustment A
  LEFT JOIN T_LaborHrsAdjustmentExt B
  ON A.Lha_EmployeeId = B.Lha_EmployeeId
	AND A.Lha_AdjustpayPeriod = B.Lha_AdjustpayPeriod
	AND A.Lha_PayPeriod = B.Lha_PayPeriod
	AND A.Lha_ProcessDate = B.Lha_ProcessDate
  LEFT JOIN T_EmployeeMaster
    ON A.Lha_EmployeeId= Emt_employeeID
    @filter1
  --WHERE A.Lha_EmployeeId = @EMPLOYEEID
	--AND A.Lha_AdjustpayPeriod = @PAYPERIOD
UNION ALL
SELECT Emt_EmployeeId[ID],
                            Emt_NickName [Nick Name]
                          , Emt_Lastname [Last Name]
                          , Emt_Firstname [First Name]
      ,'SYSTEM' AS [Type]
      , A.Lha_PayPeriod as [Pay Period]
      , Convert(varchar(10), A.Lha_ProcessDate,101) as [Date]
	  , CASE WHEN Lha_AbsentHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentHr as MONEY),1) END as [Absent Hr]
      , CASE WHEN Lha_AbsentLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentLegalHolidayHr as MONEY),1) END as [Absent HOL Hr]
      , CASE WHEN Lha_AbsentSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentSpecialHolidayHr as MONEY),1) END as [Absent SPL Hr]
      , CASE WHEN Lha_AbsentCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentCompanyHolidayHr as MONEY),1) END as [Absent COMP Hr]
      , CASE WHEN Lha_AbsentPlantShutdownHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentPlantShutdownHr as MONEY),1) END as [Absent PSD Hr]      
      , CASE WHEN Lha_AbsentFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_AbsentFillerHolidayHr as MONEY),1) END as [Absent Other HOL Hr]
      , CASE WHEN Lha_PaidLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidLegalHolidayHr as MONEY),1) END as [Unwork HOL Hr]
      , CASE WHEN Lha_PaidSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidSpecialHolidayHr as MONEY),1) END as [Unwork SPL Hr]
      , CASE WHEN Lha_PaidCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidCompanyHolidayHr as MONEY),1) END as [Unwork COMP Hr]
      , CASE WHEN Lha_PaidFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PaidFillerHolidayHr as MONEY),1) END as [Unwork Other HOL Hr]
      , CASE WHEN Lha_RegularHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularHr as MONEY),1) END as [Regular Hr]
      , CASE WHEN Lha_RegularOTHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularOTHr as MONEY),1) END as [OT Hr]
      , CASE WHEN (Lha_RestdayHr + Lha_RestdayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RestdayHr + Lha_RestdayOTHr as MONEY),1) END as [REST Hr]
	  , CASE WHEN (Lha_LegalHolidayHr + Lha_LegalHolidayOTHr + Lha_RestdayLegalHolidayHr + Lha_RestdayLegalHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LegalHolidayHr + Lha_LegalHolidayOTHr + Lha_RestdayLegalHolidayHr + Lha_RestdayLegalHolidayOTHr as MONEY),1) END as [HOL Hr]
	  , CASE WHEN (Lha_SpecialHolidayHr + Lha_SpecialHolidayOTHr + Lha_RestdaySpecialHolidayHr + Lha_RestdaySpecialHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_SpecialHolidayHr + Lha_SpecialHolidayOTHr + Lha_RestdaySpecialHolidayHr + Lha_RestdaySpecialHolidayOTHr as MONEY),1) END as [SPL Hr]
	  , CASE WHEN (Lha_CompanyHolidayHr + Lha_CompanyHolidayOTHr + Lha_RestdayCompanyHolidayHr + Lha_RestdayCompanyHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_CompanyHolidayHr + Lha_CompanyHolidayOTHr + Lha_RestdayCompanyHolidayHr + Lha_RestdayCompanyHolidayOTHr as MONEY),1) END as [COMP Hr]
	  , CASE WHEN (Lha_PlantShutdownHr + Lha_PlantShutdownOTHr + Lha_RestdayPlantShutdownHr + Lha_RestdayPlantShutdownOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_PlantShutdownHr + Lha_PlantShutdownOTHr + Lha_RestdayPlantShutdownHr + Lha_RestdayPlantShutdownOTHr as MONEY),1) END as [PSD Hr]
	  , CASE WHEN ISNULL(Lha_Filler01_Hr + Lha_Filler01_OTHr
				+ Lha_Filler02_Hr + Lha_Filler02_OTHr
				+ Lha_Filler03_Hr + Lha_Filler03_OTHr
				+ Lha_Filler04_Hr + Lha_Filler04_OTHr
				+ Lha_Filler05_Hr + Lha_Filler05_OTHr
				+ Lha_Filler06_Hr + Lha_Filler06_OTHr, 0)=0 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_Filler01_Hr + Lha_Filler01_OTHr
				+ Lha_Filler02_Hr + Lha_Filler02_OTHr
				+ Lha_Filler03_Hr + Lha_Filler03_OTHr
				+ Lha_Filler04_Hr + Lha_Filler04_OTHr
				+ Lha_Filler05_Hr + Lha_Filler05_OTHr
				+ Lha_Filler06_Hr + Lha_Filler06_OTHr as MONEY),1) END as [Other HOL Hr]
	  , CASE WHEN (Lha_RegularNDHr + Lha_RegularOTNDHr
		+ Lha_RestdayNDHr + Lha_RestdayOTNDHr
		+ Lha_LegalHolidayNDHr + Lha_LegalHolidayOTNDHr
		+ Lha_SpecialHolidayNDHr + Lha_SpecialHolidayOTNDHr
		+ Lha_PlantShutdownNDHr + Lha_PlantShutdownOTNDHr
		+ Lha_CompanyHolidayNDHr + Lha_CompanyHolidayOTNDHr
		+ Lha_RestdayLegalHolidayNDHr + Lha_RestdayLegalHolidayOTNDHr
		+ Lha_RestdaySpecialHolidayNDHr + Lha_RestdaySpecialHolidayOTNDHr
		+ Lha_RestdayPlantShutdownNDHr + Lha_RestdayPlantShutdownOTNDHr
		+ Lha_RestdayCompanyHolidayNDHr + Lha_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Lha_Filler01_NDHr + Lha_Filler01_OTNDHr
				+ Lha_Filler02_NDHr + Lha_Filler02_OTNDHr
				+ Lha_Filler03_NDHr + Lha_Filler03_OTNDHr
				+ Lha_Filler04_NDHr + Lha_Filler04_OTNDHr
				+ Lha_Filler05_NDHr + Lha_Filler05_OTNDHr
				+ Lha_Filler06_NDHr + Lha_Filler06_OTNDHr, 0))=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_RegularNDHr + Lha_RegularOTNDHr
		+ Lha_RestdayNDHr + Lha_RestdayOTNDHr
		+ Lha_LegalHolidayNDHr + Lha_LegalHolidayOTNDHr
		+ Lha_SpecialHolidayNDHr + Lha_SpecialHolidayOTNDHr
		+ Lha_PlantShutdownNDHr + Lha_PlantShutdownOTNDHr
		+ Lha_CompanyHolidayNDHr + Lha_CompanyHolidayOTNDHr
		+ Lha_RestdayLegalHolidayNDHr + Lha_RestdayLegalHolidayOTNDHr
		+ Lha_RestdaySpecialHolidayNDHr + Lha_RestdaySpecialHolidayOTNDHr
		+ Lha_RestdayPlantShutdownNDHr + Lha_RestdayPlantShutdownOTNDHr
		+ Lha_RestdayCompanyHolidayNDHr + Lha_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Lha_Filler01_NDHr + Lha_Filler01_OTNDHr
				+ Lha_Filler02_NDHr + Lha_Filler02_OTNDHr
				+ Lha_Filler03_NDHr + Lha_Filler03_OTNDHr
				+ Lha_Filler04_NDHr + Lha_Filler04_OTNDHr
				+ Lha_Filler05_NDHr + Lha_Filler05_OTNDHr
				+ Lha_Filler06_NDHr + Lha_Filler06_OTNDHr,0) as MONEY),1) END as [REST Hr]
      --, CASE WHEN Lha_SalaryRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_SalaryRate as MONEY),1) END as [Salary Rate]
	  , CASE WHEN Lha_HourlyRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_HourlyRate as MONEY),1) END as [Hourly Rate]
      , CASE WHEN (Lha_LaborHrsAdjustmentAmt - Lha_OvertimeAdjustmentAmt)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LaborHrsAdjustmentAmt - Lha_OvertimeAdjustmentAmt as MONEY),1) END as [Regular Adj Amt]
      , CASE WHEN Lha_OvertimeAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_OvertimeAdjustmentAmt as MONEY),1) END as [OT Adj Amt]
	  , CASE WHEN Lha_LaborHrsAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Lha_LaborHrsAdjustmentAmt as MONEY),1) END as [Labor Hrs Adj Amt]
  FROM T_LaborHrsAdjustmentHist A
  LEFT JOIN T_LaborHrsAdjustmentHistExt B
  ON A.Lha_EmployeeId = B.Lha_EmployeeId
	AND A.Lha_AdjustpayPeriod = B.Lha_AdjustpayPeriod
	AND A.Lha_PayPeriod = B.Lha_PayPeriod
	AND A.Lha_ProcessDate = B.Lha_ProcessDate
LEFT JOIN T_EmployeeMaster
    ON A.Lha_EmployeeId= Emt_employeeID
@filter1
  --WHERE A.Lha_EmployeeId = @EMPLOYEEID
	--AND A.Lha_AdjustpayPeriod = @PAYPERIOD
UNION ALL
SELECT Emt_EmployeeId[ID],
                            Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname],'MANUAL' AS [Type]
      , A.Ead_CurrentPayPeriod as [Pay Period]
      , NULL as [Date]
      , CASE WHEN Ead_AbsentHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentHr as MONEY),1) END as [Absent Hr]
	  , CASE WHEN Ead_AbsentLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentLegalHolidayHr as MONEY),1) END as [Absent HOL Hr]
      , CASE WHEN Ead_AbsentSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentSpecialHolidayHr as MONEY),1) END as [Absent SPL Hr]
      , CASE WHEN Ead_AbsentCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentCompanyHolidayHr as MONEY),1) END as [Absent COMP Hr]
      , CASE WHEN Ead_AbsentPlantShutdownHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentPlantShutdownHr as MONEY),1) END as [Absent PSD Hr]
      , CASE WHEN Ead_AbsentFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentFillerHolidayHr as MONEY),1) END as [Absent Other HOL Hr]
      , CASE WHEN Ead_PaidLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidLegalHolidayHr as MONEY),1) END as [Unwork HOL Hr]
      , CASE WHEN Ead_PaidSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidSpecialHolidayHr as MONEY),1) END as [Unwork SPL Hr]
      , CASE WHEN Ead_PaidCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidCompanyHolidayHr as MONEY),1) END as [Unwork COMP Hr]
      , CASE WHEN Ead_PaidFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidFillerHolidayHr as MONEY),1) END as [Unwork Other HOL Hr]
      , CASE WHEN Ead_RegularHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularHr as MONEY),1) END as [Regular Hr]
      , CASE WHEN Ead_RegularOTHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr as MONEY),1) END as [OT Hr]
      , CASE WHEN (Ead_RegularOTHr+ Ead_RestdayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr+ Ead_RestdayOTHr as MONEY),1) END as [REST Hr]
	  , CASE WHEN (Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr as MONEY),1) END as [HOL Hr]
	  , CASE WHEN (Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr as MONEY),1) END as [SPL Hr]
	  , CASE WHEN (Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr as MONEY),1) END as [COMP Hr]
	  , CASE WHEN (Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr as MONEY),1) END as [PSD Hr]
	  , CASE WHEN ISNULL(Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr, 0)=0 THEN '' ELSE  CONVERT(VARCHAR(10),Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr) END as [Other HOL Hr]
	  , CASE WHEN (Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0))=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0) as MONEY),1) END as [ND Hr]
      --, '' as [Salary Rate]
      , CASE WHEN A.Ead_HrlyRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(A.Ead_HrlyRate as MONEY),1) END as [Hourly Rate]
      , CASE WHEN (Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt as MONEY),1) END as [Regular Adj Amt]
      , CASE WHEN Ead_OvertimeAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_OvertimeAdjustmentAmt as MONEY),1) END as [OT Adj Amt]
	  , CASE WHEN Ead_LaborHrsAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt as MONEY),1) END as [Labor Hrs Adj Amt]
  FROM T_EmployeeAdjustment A
  LEFT JOIN T_EmployeeAdjustmentExt B
  ON A.Ead_EmployeeId = B.Ead_EmployeeId
	AND A.Ead_CurrentPayPeriod = B.Ead_CurrentPayPeriod
	AND A.Ead_HrlyRate = B.Ead_HrlyRate
LEFT JOIN T_EmployeeMaster
    ON A.Ead_EmployeeId= Emt_employeeID
@filter2
  --WHERE A.Ead_EmployeeId = @EMPLOYEEID
	--AND A.Ead_CurrentPayPeriod = @PAYPERIOD
UNION ALL
SELECT Emt_EmployeeId[ID],
                            Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname],'MANUAL' AS [Type]
      , A.Ead_CurrentPayPeriod as [Pay Period]
      , NULL as [Date]
      , CASE WHEN Ead_AbsentHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentHr as MONEY),1) END as [Absent Hr]
	  , CASE WHEN Ead_AbsentLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentLegalHolidayHr as MONEY),1) END as [Absent HOL Hr]
      , CASE WHEN Ead_AbsentSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentSpecialHolidayHr as MONEY),1) END as [Absent SPL Hr]
      , CASE WHEN Ead_AbsentCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentCompanyHolidayHr as MONEY),1) END as [Absent COMP Hr]
      , CASE WHEN Ead_AbsentPlantShutdownHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentPlantShutdownHr as MONEY),1) END as [Absent PSD Hr]
      , CASE WHEN Ead_AbsentFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentFillerHolidayHr as MONEY),1) END as [Absent Other HOL Hr]
      , CASE WHEN Ead_PaidLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidLegalHolidayHr as MONEY),1) END as [Unwork HOL Hr]
      , CASE WHEN Ead_PaidSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidSpecialHolidayHr as MONEY),1) END as [Unwork SPL Hr]
      , CASE WHEN Ead_PaidCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidCompanyHolidayHr as MONEY),1) END as [Unwork COMP Hr]
      , CASE WHEN Ead_PaidFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidFillerHolidayHr as MONEY),1) END as [Unwork Other HOL Hr]
      , CASE WHEN Ead_RegularHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularHr as MONEY),1) END as [Regular Hr]
      , CASE WHEN Ead_RegularOTHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr as MONEY),1) END as [OT Hr]
      , CASE WHEN (Ead_RegularOTHr+ Ead_RestdayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr+ Ead_RestdayOTHr as MONEY),1) END as [REST Hr]
	  , CASE WHEN (Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr as MONEY),1) END as [HOL Hr]
	  , CASE WHEN (Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr as MONEY),1) END as [SPL Hr]
	  , CASE WHEN (Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr as MONEY),1) END as [COMP Hr]
	  , CASE WHEN (Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr as MONEY),1) END as [PSD Hr]
	  , CASE WHEN ISNULL(Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr, 0)=0 THEN '' ELSE  CONVERT(VARCHAR(10),Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr) END as [Other HOL Hr]
	  , CASE WHEN (Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0))=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0) as MONEY),1) END as [ND Hr]
      --, '' as [Salary Rate]
      , CASE WHEN A.Ead_HrlyRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(A.Ead_HrlyRate as MONEY),1) END as [Hourly Rate]
      , CASE WHEN (Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt as MONEY),1) END as [Regular Adj Amt]
      , CASE WHEN Ead_OvertimeAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_OvertimeAdjustmentAmt as MONEY),1) END as [OT Adj Amt]
	  , CASE WHEN Ead_LaborHrsAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt as MONEY),1) END as [Labor Hrs Adj Amt]
  FROM T_EmployeeAdjustmentHist A
  LEFT JOIN T_EmployeeAdjustmentHistExt B
  ON A.Ead_EmployeeId = B.Ead_EmployeeId
	AND A.Ead_CurrentPayPeriod = B.Ead_CurrentPayPeriod
	AND A.Ead_HrlyRate = B.Ead_HrlyRate
LEFT JOIN T_EmployeeMaster
    ON A.Ead_EmployeeId= Emt_employeeID
@filter2
  --WHERE A.Ead_EmployeeId = @EMPLOYEEID
	--AND A.Ead_CurrentPayPeriod = @PAYPERIOD
UNION ALL
SELECT Emt_EmployeeId[ID],
                            Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname],'MANUAL' AS [Type]
      , A.Ead_CurrentPayPeriod as [Pay Period]
      , NULL as [Date]
      , CASE WHEN Ead_AbsentHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentHr as MONEY),1) END as [Absent Hr]
	  , CASE WHEN Ead_AbsentLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentLegalHolidayHr as MONEY),1) END as [Absent HOL Hr]
      , CASE WHEN Ead_AbsentSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentSpecialHolidayHr as MONEY),1) END as [Absent SPL Hr]
      , CASE WHEN Ead_AbsentCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentCompanyHolidayHr as MONEY),1) END as [Absent COMP Hr]
      , CASE WHEN Ead_AbsentPlantShutdownHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentPlantShutdownHr as MONEY),1) END as [Absent PSD Hr]
      , CASE WHEN Ead_AbsentFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_AbsentFillerHolidayHr as MONEY),1) END as [Absent Other HOL Hr]
      , CASE WHEN Ead_PaidLegalHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidLegalHolidayHr as MONEY),1) END as [Unwork HOL Hr]
      , CASE WHEN Ead_PaidSpecialHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidSpecialHolidayHr as MONEY),1) END as [Unwork SPL Hr]
      , CASE WHEN Ead_PaidCompanyHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidCompanyHolidayHr as MONEY),1) END as [Unwork COMP Hr]
      , CASE WHEN Ead_PaidFillerHolidayHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PaidFillerHolidayHr as MONEY),1) END as [Unwork Other HOL Hr]
      , CASE WHEN Ead_RegularHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularHr as MONEY),1) END as [Regular Hr]
      , CASE WHEN Ead_RegularOTHr=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr as MONEY),1) END as [OT Hr]
      , CASE WHEN (Ead_RegularOTHr+ Ead_RestdayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularOTHr+ Ead_RestdayOTHr as MONEY),1) END as [REST Hr]
	  , CASE WHEN (Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LegalHolidayHr + Ead_LegalHolidayOTHr + Ead_RestdayLegalHolidayHr + Ead_RestdayLegalHolidayOTHr as MONEY),1) END as [HOL Hr]
	  , CASE WHEN (Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_SpecialHolidayHr + Ead_SpecialHolidayOTHr + Ead_RestdaySpecialHolidayHr + Ead_RestdaySpecialHolidayOTHr as MONEY),1) END as [SPL Hr]
	  , CASE WHEN (Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_CompanyHolidayHr + Ead_CompanyHolidayOTHr + Ead_RestdayCompanyHolidayHr + Ead_RestdayCompanyHolidayOTHr as MONEY),1) END as [COMP Hr]
	  , CASE WHEN (Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_PlantShutdownHr + Ead_PlantShutdownOTHr + Ead_RestdayPlantShutdownHr + Ead_RestdayPlantShutdownOTHr as MONEY),1) END as [PSD Hr]
	  , CASE WHEN ISNULL(Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr, 0)=0 THEN '' ELSE  CONVERT(VARCHAR(10),Ead_Filler01_Hr + Ead_Filler01_OTHr
				+ Ead_Filler02_Hr + Ead_Filler02_OTHr
				+ Ead_Filler03_Hr + Ead_Filler03_OTHr
				+ Ead_Filler04_Hr + Ead_Filler04_OTHr
				+ Ead_Filler05_Hr + Ead_Filler05_OTHr
				+ Ead_Filler06_Hr + Ead_Filler06_OTHr) END as [Other HOL Hr]
	  , CASE WHEN (Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0))=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_RegularNDHr + Ead_RegularOTNDHr
		+ Ead_RestdayNDHr + Ead_RestdayOTNDHr
		+ Ead_LegalHolidayNDHr + Ead_LegalHolidayOTNDHr
		+ Ead_SpecialHolidayNDHr + Ead_SpecialHolidayOTNDHr
		+ Ead_PlantShutdownNDHr + Ead_PlantShutdownOTNDHr
		+ Ead_CompanyHolidayNDHr + Ead_CompanyHolidayOTNDHr
		+ Ead_RestdayLegalHolidayNDHr + Ead_RestdayLegalHolidayOTNDHr
		+ Ead_RestdaySpecialHolidayNDHr + Ead_RestdaySpecialHolidayOTNDHr
		+ Ead_RestdayPlantShutdownNDHr + Ead_RestdayPlantShutdownOTNDHr
		+ Ead_RestdayCompanyHolidayNDHr + Ead_RestdayCompanyHolidayOTNDHr
		+ ISNULL(Ead_Filler01_NDHr + Ead_Filler01_OTNDHr
				+ Ead_Filler02_NDHr + Ead_Filler02_OTNDHr
				+ Ead_Filler03_NDHr + Ead_Filler03_OTNDHr
				+ Ead_Filler04_NDHr + Ead_Filler04_OTNDHr
				+ Ead_Filler05_NDHr + Ead_Filler05_OTNDHr
				+ Ead_Filler06_NDHr + Ead_Filler06_OTNDHr, 0) as MONEY),1) END as [ND Hr]
      --, '' as [Salary Rate]
      , CASE WHEN A.Ead_HrlyRate=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(A.Ead_HrlyRate as MONEY),1) END as [Hourly Rate]
      , CASE WHEN (Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt)=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt - Ead_OvertimeAdjustmentAmt as MONEY),1) END as [Regular Adj Amt]
      , CASE WHEN Ead_OvertimeAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_OvertimeAdjustmentAmt as MONEY),1) END as [OT Adj Amt]
	  , CASE WHEN Ead_LaborHrsAdjustmentAmt=0.00 THEN '' ELSE  CONVERT(VARCHAR,CAST(Ead_LaborHrsAdjustmentAmt as MONEY),1) END as [Labor Hrs Adj Amt]
  FROM T_EmployeeAdjustmentSep A
  LEFT JOIN T_EmployeeAdjustmentSepExt B
  ON A.Ead_EmployeeId = B.Ead_EmployeeId
	AND A.Ead_CurrentPayPeriod = B.Ead_CurrentPayPeriod
	AND A.Ead_HrlyRate = B.Ead_HrlyRate
LEFT JOIN T_EmployeeMaster
    ON A.Ead_EmployeeId= Emt_employeeID
@filter2
  --WHERE A.Ead_EmployeeId = @EMPLOYEEID
	--AND A.Ead_CurrentPayPeriod = @PAYPERIOD"; 
        #endregion
        string filter1 = "";
        filter1 = getFilters();
        string filter2 = "";
        filter2 = getFilters().Replace("A.Lha_AdjustpayPeriod", "A.Ead_CurrentPayPeriod");
        filter2 = filter2.Replace("Lha_PayrollPost", "Ead_PayrollPost");
        query = query.Replace("@filter1", filter1);
        query = query.Replace("@filter2", filter2);
        return query;
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Eot_ControlNo [Control No]
                          , Eot_EmployeeId [ID No]
                          , Emt_NickName [ID Code]
                          , Emt_NickName [Nickname]
                          , Emt_Lastname [Lastname]
                          , Emt_Firstname [Firstname]
                          , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]
                          , Pmx_ParameterDesc [Type]
                          , LEFT(Eot_StartTime,2) 
                            + ':' 
                            + RIGHT(Eot_StartTime,2) [Start]
                          , LEFT(Eot_EndTime,2) 
                            + ':' 
                            + RIGHT(Eot_EndTime,2) [End]
                          , Eot_OvertimeHour [Hours]
                          , dbo.getCostCenterFullNameV2(Eot_CostCenter) [Cost Center]
                          , Convert(varchar(10), Eot_AppliedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Eot_AppliedDate,113),5) [Applied Date]
                          , Convert(varchar(10), Eot_EndorsedDateToChecker,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Eot_EndorsedDateToChecker,113),5) [Endorsed Date]
                          , Eot_Reason [Reason]
                          , Eot_JobCode [Job Code]
                          , Eot_ClientJobNo [Client Job No]
                          , Slm_ClientJobName [Client Job Name]
                          , Slm_DashClassCode [DASH Class Code]
                          , Slm_DashWorkCode [DASH Work Code]
                          , AD2.Adt_AccountDesc [@Eot_Filler1Desc]
                          , AD3.Adt_AccountDesc [@Eot_Filler2Desc]
                          , AD4.Adt_AccountDesc [@Eot_Filler3Desc]
                          , AD1.Adt_AccountDesc [Status]
                          , Eot_BatchNo [Batch No]
                          , Emt_FirstName [First Name]
                          , Emt_LastName [Last Name]
                          , dbo.GetControlEmployeeNameV2(C1.Umt_UserCode) [Checker 1]
                          , Convert(varchar(10), Eot_CheckedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Eot_CheckedDate,113),5) [Checked Date 1]
			              , dbo.GetControlEmployeeNameV2(C2.Umt_UserCode) [Checker 2]
                          , Convert(varchar(10), Eot_Checked2Date,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Eot_Checked2Date,113),5) [Checked Date 2]
			              , dbo.GetControlEmployeeNameV2(AP.Umt_UserCode) [Approver]
                          , Convert(varchar(10), Eot_ApprovedDate,101) 
                            + ' ' 
                            + RIGHT(Convert(varchar(17), Eot_ApprovedDate,113),5) [Approved Date]
                          , Trm_Remarks [Remarks]
                          , Eot_CurrentPayPeriod [Pay Period]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        filter = @" 
                     WHERE 1 = 1 AND Lha_PayrollPost = 1 ";
        #region textBox Filters
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_EmployeeId {0}
                                           OR Emt_Lastname {0}
                                           OR Emt_Firstname {0}
                                           OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( Emt_CostCenterCode {0}
                                           OR dbo.getCostCenterFullNameV2(Emt_CostCenterCode) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                     , Session["userLogged"].ToString());
        }
        //No need else because if employee login user cannot change the txtEmployee filter 
        //so report would always filter only user's own transaction
        //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
        //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
        if (tbrEmployee.Visible)
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "PAYROLL"))
            {
                filter += string.Format(@" AND  (  ( Emt_CostCenterCode IN ( SELECT Uca_CostCenterCode
                                                                            FROM T_UserCostCenterAccess
                                                                        WHERE Uca_UserCode = '{0}'
                                                                            AND Uca_SytemId = 'PAYROLL')
                                                    OR Emt_EmployeeId = '{0}'))", Session["userLogged"].ToString());
            }
        }
        
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            filter += string.Format(@" AND  ( A.Lha_AdjustpayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
        }
       #region Unwanted
//        if (!txtStatus.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_Status {0}
//                                           OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
//        }
//		 if (!txtBatchNo.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_BatchNo {0})", sqlINFormat(txtBatchNo.Text));
//        }
//        if (!txtFiller1.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_Filler1 {0}
//                                           OR AD2.Adt_AccountDesc {0})", sqlINFormat(txtFiller1.Text));
//        }
//        if (!txtFiller2.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_Filler2 {0}
//                                           OR AD3.Adt_AccountDesc {0})", sqlINFormat(txtFiller2.Text));
//        }
//        if (!txtFiller3.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_Filler3 {0}
//                                           OR AD4.Adt_AccountDesc {0})", sqlINFormat(txtFiller3.Text));
//        }
//        if (!txtChecker1.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_CheckedBy {0}
//                                           OR C1.Umt_UserCode {0}
//                                           OR C1.Umt_UserLname {0}
//                                           OR C1.Umt_UserFname {0}
//                                           OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
//        }
//        if (!txtChecker2.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_Checked2By {0}
//                                           OR C2.Umt_UserCode {0}
//                                           OR C2.Umt_UserLname {0}
//                                           OR C2.Umt_UserFname {0}
//                                           OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
//        }
//        if (!txtApprover.Text.Trim().Equals(string.Empty))
//        {
//            filter += string.Format(@" AND  ( Eot_ApprovedBy {0}
//                                           OR AP.Umt_UserCode {0}
//                                           OR AP.Umt_UserLname {0}
//                                           OR AP.Umt_UserFname {0}
//                                           OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
//        }
//        #endregion
//        if (!ddlType.SelectedValue.Equals("ALL"))
//        {
//            filter += string.Format(@" AND Eot_OvertimeType = '{0}'", ddlType.SelectedValue);
//        }
//        if (!cbxDefaultOT.Checked)
//        {
//            filter += @" AND Eot_ControlNo LIKE 'V%'";
//        }
//        #region DateTime Pickers
//        //Overtime Date
//        if (!dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_OvertimeDate BETWEEN '{0}' AND '{1}'", dtpOTDateFrom.Date.ToString("MM/dd/yyyy")
//                                                                                    , dtpOTDateTo.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpOTDateFrom.IsNull)
//        {
//            filter += string.Format(@" AND Eot_OvertimeDate >= '{0}'", dtpOTDateFrom.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpOTDateTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_OvertimeDate <= '{0}'", dtpOTDateTo.Date.ToString("MM/dd/yyyy"));
//        }

//        //Applied Date
//        if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
//                                                                                   , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpAppliedFrom.IsNull)
//        {
//            filter += string.Format(@" AND Eot_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpAppliedTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
//        }
//        //Endorsed Date
//        if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
//                                                                                             , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpEndorsedFrom.IsNull)
//        {
//            filter += string.Format(@" AND Eot_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpEndorsedTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
//        }
//        //Checked Date
//        if (!dtpC1From.IsNull && !dtpC1To.IsNull)
//        {
//            filter += string.Format(@" AND Eot_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
//                                                                                   , dtpC1To.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpC1From.IsNull)
//        {
//            filter += string.Format(@" AND Eot_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpC1To.IsNull)
//        {
//            filter += string.Format(@" AND Eot_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
//        }
//        //Checked Date 2
//        if (!dtpC2From.IsNull && !dtpC2To.IsNull)
//        {
//            filter += string.Format(@" AND Eot_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
//                                                                                    , dtpC2To.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpC2From.IsNull)
//        {
//            filter += string.Format(@" AND Eot_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpC2To.IsNull)
//        {
//            filter += string.Format(@" AND Eot_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
//        }
//        //Approved Date
//        if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
//                                                                                    , dtpAPTo.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpAPFrom.IsNull)
//        {
//            filter += string.Format(@" AND Eot_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
//        }
//        else if (!dtpAPTo.IsNull)
//        {
//            filter += string.Format(@" AND Eot_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
//        } 
	#endregion

        #endregion
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Emt_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_NickName LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                          OR ( LEFT(ISNULL(Emt_Middlename,''),1) LIKE '{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Emt_CostCenterCode, 4)) LIKE '%{0}%' )
                                          OR ( dbo.getCostCenterFullNameV2(LEFT(Emt_CostCenterCode, 6)) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),A.Lha_ProcessDate,101) LIKE '%{0}%' )
                                          OR ( CONVERT(varchar(10),A.Lha_ProcessDate,101) 
                                            + ' ' 
                                            + LEFT(CONVERT(varchar(20),A.Lha_ProcessDate,114),5) LIKE '%{0}%' )
                                         OR (A.Lha_PayPeriod LIKE '%{0}%')
                                         OR (A.Lha_AdjustpayPeriod LIKE '%{0}%' )
                                          
                                    )";

            string holder = string.Empty;
            string searchKey = txtSearch.Text.Replace("'", "");
            searchKey += "&";
            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                filter += string.Format(searchFilter, holder);
            }
        }
        return filter;
    }

    private string sqlINFormat(string text)
    {
        string[] temp = text.Split(',');
        string value = "IN ( ";
        for (int i = 0; i < temp.Length; i++)
        {
            value += "'" + temp[i].Trim() + "'";
            if (i != temp.Length - 1)
                value += ",";
        }
        value += ")";
        return value;
    }

    //    private string getFillerName(string fillerName)
    //    {

    //        string sql = string.Format(@"  SELECT CASE WHEN ISNULL(Cfl_TextDisplay, Cfl_ColName) = ''
    //			                                    THEN Cfl_ColName
    //			                                    ELSE Cfl_TextDisplay
    //	                                          END
    //	                                     FROM T_ColumnFiller
    //                                        WHERE Cfl_ColName = '{0}'", fillerName);
    //        string flag = string.Empty;
    //        using (DALHelper dal = new DALHelper())
    //        {
    //            try
    //            {
    //                dal.OpenDB();
    //                flag = dal.ExecuteScalar(sql, CommandType.Text).ToString();
    //            }
    //            catch
    //            {

    //            }
    //            finally
    //            {
    //                dal.CloseDB();
    //            }
    //        }
    //        TextInfo UsaTextInfo = new CultureInfo("en-US", false).TextInfo;
    //        return UsaTextInfo.ToTitleCase(flag.ToLower());
    //    }

    private void UpdatePagerLocation()
    {
        int pageIndex = Convert.ToInt32(hfPageIndex.Value);
        int numRows = Convert.ToInt32(hfNumRows.Value);
        int rowCount = Convert.ToInt32(hfRowCount.Value);
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRowNo.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
            #region Remove Columns
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    ds.Tables[0].Columns.Remove("Nickname");
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                }
            }
            else
            {
                //ds.Tables[0].Columns.Remove("ID Code");
                ds.Tables[0].Columns.Remove("Nick Name");
            }

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                ds.Tables[0].Columns.Remove("Last Name");
                ds.Tables[0].Columns.Remove("First Name");
                ds.Tables[0].Columns.Remove("MI");
            }
            //DASH Specific
            //ds.Tables[0].Columns.Remove("Job Code");
            //ds.Tables[0].Columns.Remove("Client Job Name");
            //ds.Tables[0].Columns.Remove("Client Job No");
            //ds.Tables[0].Columns.Remove("DASH Class Code");
            //ds.Tables[0].Columns.Remove("DASH Work Code");
            ////Depending if Used
            //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler01"));
            //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler02"));
            //ds.Tables[0].Columns.Remove(CommonMethods.getFillerName("Eot_Filler03"));

            //Includes
            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                if (!cblInclude.Items[i].Selected)
                    ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
            }
            #endregion

            for (int trav = 0; trav < ds.Tables[0].Columns.Count; )
            {
                bool indicator =false;
                //string s = ds.Tables[0].Columns[trav].ToString();
                for (int travRow = 0; travRow < ds.Tables[0].Rows.Count && !indicator; travRow++)
                {
                    if (ds.Tables[0].Rows[travRow][trav] != "")
                        indicator = true;
                }
                if (!indicator)
                    ds.Tables[0].Columns.Remove(ds.Tables[0].Columns[trav].ColumnName);
                else
                    trav++;
            }
            hfRowCount.Value = "0";
            //foreach (DataRow dr in ds.Tables[1].Rows)
            //    hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
            dgvResult.DataSource = ds;
            //agvResult.Columns.Clear();
            //agvResult.
            //agvResult.DataSource = null;
            //agvResult.DataBind();
            ReCreateColumns(ds.Tables[0]);
            agvResult.DataSource = ds;
            agvResult.DataBind();
            //agvResult.DataBind();
            dgvResult.DataBind();
            grivRight(this.agvResult);
        }
        
    }
    private void ReCreateColumns(DataTable table)
    {
        agvResult.Columns.Clear();

        //DataTable table = GetCurrentTable();
        foreach (DataColumn dataColumn in table.Columns)
        {
            GridViewDataTextColumn column = new GridViewDataTextColumn();
            column.FieldName = dataColumn.ColumnName;
            // set additional column properties
            column.Caption = dataColumn.ColumnName;
            agvResult.Columns.Add(column);
        }
    }
    private void grivRight(ASPxGridView agvResult)
    {
        agvResult.TotalSummary.Clear();
        //agvResult.HtmlEncode(
         for (int i = 0; i < agvResult.Columns.Count; i++)
         {
             string name = ((DevExpress.Web.ASPxGridView.GridViewDataColumn)agvResult.Columns[i]).FieldName;
             if (name != "Last Name" && name != "ID" && name != "First Name" && name != "Type" && name != "Pay Period")
             {
                 agvResult.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                 agvResult.Settings.ShowFooter = true;
                 //if(name=="Labor Hrs Adj Amt")
                     //agvResult.Columns[i].
                 ///agvResult.HtmlEncode(
                 //agvResult.TotalSummary.Clear();
                //ASPxGridView_Details.Settings.ShowFooter = true;
                //ASPxGridView_Details.TotalSummary.Clear();
                //if(dataDetail.SelectCommand.Contains("Hours"))
                 //DevExpress.Web.ASPxGridView.ASPxSummaryItem = new DevExpress.Web.ASPxGridView.ASPxSummaryItem();
                 //DevExpress.Web.ASPxGridView.ASPxSummaryItem = new 
                 if (name != "Date" && name != "Hourly Rate")
                 {
                     ASPxSummaryItem asi = new ASPxSummaryItem();
                     asi.FieldName = name;
                     asi.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                     asi.DisplayFormat = "Total {1} {0:N}";
                     
                     agvResult.TotalSummary.Add(asi);
                 }
                 //agvResult.TotalSummary.Add(new DevExpress.Web.ASPxGridView.ASPxSummaryItem() { FieldName = name, SummaryType = DevExpress.Data.SummaryItemType.Sum, DisplayFormat = "Total {1} {0}" });
                //if (dataDetail.SelectCommand.Contains("Regular Hours"))
                 //   ASPxGridView_Details.TotalSummary.Add(new DevExpress.Web.ASPxGridView.ASPxSummaryItem() { FieldName = "Regular Hours", SummaryType = DevExpress.Data.SummaryItemType.Sum, DisplayFormat = "Total {1} {0}" });
                 //agvResult.DataBind();
                //btnExport.Visible = true;
            
             }
         }
    }
    private string initializeExcelHeader()
    {
        string criteria = string.Empty;
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            criteria += lblEmployee.Text + ":" + txtEmployee.Text.Trim() + "-";
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            criteria += lblCostcenter.Text + ":" + txtCostcenter.Text.Trim() + "-";
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            criteria += lblStatus.Text + ":" + txtStatus.Text.Trim() + "-";
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            criteria += lblPayPeriod.Text + ":" + txtPayPeriod.Text.Trim() + "-";
        }
        if (!txtBatchNo.Text.Trim().Equals(string.Empty))
        {
            criteria += lblBatchNo.Text + ":" + txtBatchNo.Text.Trim() + "-";
        }
        if (!txtFiller1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller1.Text + ":" + txtFiller1.Text.Trim() + "-";
        }
        if (!txtFiller2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller2.Text + ":" + txtFiller2.Text.Trim() + "-";
        }
        if (!txtFiller3.Text.Trim().Equals(string.Empty))
        {
            criteria += lblFiller3.Text + ":" + txtFiller3.Text.Trim() + "-";
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker1.Text + ":" + txtChecker1.Text.Trim() + "-";
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker2.Text + ":" + txtChecker2.Text.Trim() + "-";
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            criteria += lblApprover.Text + ":" + txtApprover.Text.Trim() + "-";
        }
        if (!dtpOTDateFrom.IsNull)
        {
            criteria += lblOTDateFrom.Text + ":" + dtpOTDateFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpOTDateTo.IsNull)
        {
            criteria += lblOTDateTo.Text + ":" + dtpOTDateTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedFrom.IsNull)
        {
            criteria += lblAppliedFrom.Text + ":" + dtpAppliedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedTo.IsNull)
        {
            criteria += lblAppliedTo.Text + ":" + dtpAppliedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedFrom.IsNull)
        {
            criteria += lblEndorsedFrom.Text + ":" + dtpEndorsedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedTo.IsNull)
        {
            criteria += lblEndorsedTo.Text + ":" + dtpEndorsedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        return criteria.Trim();
    }
    decimal sumFooterValue = 0;
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
         string sponsorBonus = "0";//((Label)e.Row.FindControl("OT Hr")).Text;
         //string pairingBonus = ((Label)e.Row.FindControl("Label3")).Text;
         //string staticBonus = ((Label)e.Row.FindControl("Label4")).Text;
         //string leftBonus = ((Label)e.Row.FindControl("Label5")).Text;
         //string rightBonus = ((Label)e.Row.FindControl("Label6")).Text;
         decimal totalvalue = Convert.ToDecimal(sponsorBonus);// +Convert.ToDecimal(pairingBonus) + Convert.ToDecimal(staticBonus) + Convert.ToDecimal(leftBonus) + Convert.ToDecimal(rightBonus);
         e.Row.Cells[6].Text = totalvalue.ToString();
         sumFooterValue += totalvalue;
        }

    if (e.Row.RowType == DataControlRowType.Footer)
        {
           Label lbl = (Label)e.Row.FindControl("lblTotal");
           lbl.Text = sumFooterValue.ToString();
        }

   }
    #endregion

}