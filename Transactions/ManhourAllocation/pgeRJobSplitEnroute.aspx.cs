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
using System.Text;

using Payroll.DAL;

public partial class Default3 : System.Web.UI.Page
{
    #region [Class Variables]
    MenuGrant MGBL = new MenuGrant();
    static DataSet dsView = new DataSet();
    static int pageIndex = 0;
    static int rowCount = 0;
    static int numRows = 100;
    static ArrayList colIndex = new ArrayList();
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFJSENROUTE"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                UpdatePagerLocation();
            }
        }
    }
    #region [Events]
    protected void GenerateButton_Click(object sender, EventArgs e)
    {
        pageIndex = 0;
        rowCount = 0;
        dsView = GetData(SqlBuilder().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString()));
        LoadGridView();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFJSENROUTE", Session["userLogged"].ToString(), true, Session["userLogged"].ToString());



    }
    #endregion

    #region [Other Methods]
    protected DataSet GetData(string sqlQuery)
    {
        DataSet dsTemp = null;
        string sqlFetch = sqlQuery;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsTemp = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
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
        return dsTemp;
    }

    private void LoadGridView()
    {
        if (dsView != null)
        {
            DataTable dt = dsView.Tables[0].Copy();
            InitDataTable(dt);
            rowCount = 0;
            foreach (DataRow dr in dsView.Tables[1].Rows)
                rowCount += Convert.ToInt32(dr[0]);
            grdView.DataSource = dt;
            grdView.DataBind();
        }
    }

    private void InitDataTable(DataTable dt)
    {
       
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            pageIndex--;
        else if (((Button)sender).ID == "btnNext")
            pageIndex++;
        dsView = GetData(SqlBuilder().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString()));
        LoadGridView();
        UpdatePagerLocation();
    }

    private void UpdatePagerLocation()
    {
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRows.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }

    protected string SqlBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @processflag as bit
                    set @processflag = (select pcm_processflag from t_processcontrolmaster
                    where pcm_systemid = 'GENERAL' and pcm_processid = 'DSPFULLNM')
                    declare @startIndex int;
                    set @startIndex = (@pageIndex * @numRow) + 1;
                
                    WITH TempTable AS (
                    SELECT Row_Number() OVER (Order by [Control No], [Manhour Date]) [Row], *
                    FROM ( 
                    Select ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@") AS temp)
                            SELECT [Control No]
                                 , [Status]
                                 , [Employee Name]
                                 , [Manhour Date]
                                 , [Applied Date]
                                 , [Flag Entry]	
                                 , [Cost Center]
                                 , [Endorsed Date]
                                 , [Checker 1]
                                 , [Checked Date 1]
                                 , [Checker 2]
                                 , [Checked Date 2]
                                 , [Approver]
                                 , [Approved Date]
                                 , [Reference Control No]
                   FROM TempTable
                  WHERE Row between
                    @startIndex and @startIndex + @numRow - 1");
        sql.Append("\nSelect SUM(cnt) FROM (");
        sql.Append("\nSelect Count(Jsh_EmployeeId) [cnt]");
        sql.Append(getFilters());
        
        sql.Append(") as Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        StringBuilder sqlColumns = new StringBuilder();
        sqlColumns.Append(@"       Jsh_ControlNo AS [Control No]
                                 , Adt_AccountDesc AS [Status] 
                                 , dbo.GetControlEmployeeName(emt1.Emt_EmployeeId) AS [Employee Name]
                                 , Convert(varchar(10), Jsh_JobSplitDate, 101) AS [Manhour Date]
                                 , Jsh_AppliedDate AS [Applied Date]
                                 , CASE WHEN Jsh_Entry = 'C' THEN 'CHANGED'
                                        WHEN Jsh_Entry = 'N' THEN 'NEW'
	                                    WHEN Jsh_Entry = 'N' THEN '' END AS [Flag Entry]	
                                 , dbo.getCostCenterFullNameV2(Jsh_Costcenter) AS [Cost Center]
                                 , Convert(varchar(10),Jsh_EndorsedDateToChecker,101) +' '+ Convert(varchar(5),Jsh_EndorsedDateToChecker,114) AS [Endorsed Date]
                                 , dbo.GetControlEmployeeName(emt2.Emt_EmployeeId) AS [Checker 1]
                                 , Convert(varchar(10),Jsh_CheckedDate,101) +' '+ Convert(varchar(5),Jsh_CheckedDate,114) AS [Checked Date 1]
                                 , dbo.GetControlEmployeeName(emt3.Emt_EmployeeId) AS [Checker 2]
                                 , Convert(varchar(10),Jsh_Checked2Date,101) +' '+ Convert(varchar(5),Jsh_Checked2Date,114) AS [Checked Date 2]
                                 , dbo.GetControlEmployeeName(emt4.Emt_EmployeeId) AS [Approver]
                                 , Convert(varchar(10),Jsh_ApprovedDate,101) +' '+ Convert(varchar(5),Jsh_ApprovedDate,114) AS [Approved Date]
                                 , Jsh_RefControlNo AS [Reference Control No]");
        return sqlColumns.ToString();
    }

    private string getFilters()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@" FROM T_JobSplitHeader
                      LEFT JOIN T_EmployeeMaster emt1
                        ON emt1.Emt_EmployeeId = Jsh_EmployeeId
                      LEFT JOIN T_EmployeeMaster emt2
                        ON emt2.Emt_EmployeeId = Jsh_CheckedBy
                      LEFT JOIN T_EmployeeMaster emt3
                        ON emt3.Emt_EmployeeId = Jsh_Checked2By
                      LEFT JOIN T_EmployeeMaster emt4
                        ON emt4.Emt_EmployeeId = Jsh_ApprovedBy
                      LEFT JOIN T_AccountDetail
                        ON Adt_AccountCode = Jsh_Status
                       AND Adt_AccountType = 'WFSTATUS'");
        #region Filters
        if (ddlStatus.SelectedValue.Trim().Equals("ALL"))
        {
            sql.Append(@" WHERE Jsh_Status IN ('3','5','7') AND LEFT(Jsh_ControlNo,1) = 'S' ");
        }
        else
        {
            sql.Append(string.Format(@" WHERE Jsh_Status = '{0}'  AND LEFT(Jsh_ControlNo,1) = 'S' ", ddlStatus.SelectedValue.ToString()));
        }

        if (!dtpDateFrom.IsNull && !dtpDateTo.IsNull)
        {
            sql.Append(string.Format(@" AND Jsh_JobSplitDate BETWEEN '{0}' AND '{1}'", dtpDateFrom.Date.ToString("MM/dd/yyyy")
                                                                                     , dtpDateTo.Date.ToString("MM/dd/yyyy")));
        }
        else if (!dtpDateFrom.IsNull)
        {
            sql.Append(string.Format(@" AND Jsh_JobSplitDate >= '{0}'", dtpDateFrom.Date.ToString("MM/dd/yyyy")));
        }
        else if (!dtpDateTo.IsNull)
        {
            sql.Append(string.Format(@" AND Jsh_JobSplitDate <= '{0}'", dtpDateTo.Date.ToString("MM/dd/yyyy")));
        }
        return sql.ToString();
    }
        #endregion
    #endregion
    protected void ClearButton_Click(object sender, EventArgs e)
    {
        ClearControls();
        grdView.DataSource = null;
        grdView.DataBind();
    }

    protected void ClearControls()
    {
        ddlStatus.SelectedIndex = -1;
        dtpDateFrom.Reset();
        dtpDateTo.Reset();
        pageIndex = 0;
        rowCount = 0;
        UpdatePagerLocation();
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                DataTable dtGrid = GetData(@"declare @processflag as bit
                                        set @processflag = (select pcm_processflag from t_processcontrolmaster
                                        where pcm_systemid = 'GENERAL' and pcm_processid = 'DSPFULLNM')
                                        Select " + getColumns() + getFilters()).Tables[0];
                if (dtGrid.Rows.Count > 0)
                {
                    InitDataTable(dtGrid);
                    GridView grdv = new GridView();
                    grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
                    grdv.DataSource = dtGrid;
                    grdv.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(dtGrid.Columns.Count, grdv.Rows.Count, "Enroute Manhour Allcation Report", initializeHeader());
                    ctrl[1] = grdv;
                    ExportExcelHelper.ExportControl2(ctrl, "Enroute Manhour Allocation Report");
                }
                else
                    MessageBox.Show("Filter have been changed! Please regenerate the table.");
            }
            catch
            {
                MessageBox.Show("Some error occurred during exporting to Excel.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }
    private string initializeHeader()
    {
        string options = "";

        if (ddlStatus.SelectedValue != "")
            options += "Status: " + ddlStatus.SelectedItem.Text + "; ";

        if (!dtpDateFrom.IsNull && dtpDateTo.IsNull)
            options += "Manhour Date: From " + dtpDateFrom.Date.ToString("MM/dd/yyyy") + "; ";
        else if(!dtpDateFrom.IsNull && !dtpDateTo.IsNull)
            options += "Manhour Date: From " + dtpDateFrom.Date.ToString("MM/dd/yyyy") + " To " + dtpDateTo.Date.ToString("MM/dd/yyyy") + "; ";
        else if(!dtpDateTo.IsNull)  
        {
            options += "  Manhour Date: To " + dtpDateTo.Date.ToString("MM/dd/yyyy") + "; ";
        }

        return options.Trim();
    }

    protected void grdView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, colIndex);
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                DataTable dtGrid = GetData(@"declare @processflag as bit
                                        set @processflag = (select pcm_processflag from t_processcontrolmaster
                                        where pcm_systemid = 'GENERAL' and pcm_processid = 'DSPFULLNM')
                                        Select " + getColumns() + getFilters()).Tables[0];
                if (dtGrid.Rows.Count > 0)
                {
                    InitDataTable(dtGrid);
                    GridView grdv = new GridView();
                    grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
                    grdv.DataSource = dtGrid;
                    grdv.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(dtGrid.Columns.Count, grdv.Rows.Count, "Routed Manhour Modification Report", initializeHeader());
                    ctrl[1] = grdv;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                }
                else
                    MessageBox.Show("Filter have been changed! Please regenerate the table.");
            }
            catch
            {
                MessageBox.Show("Some error occurred during initialization of page for printing.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }
}