using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using MethodsLibrary;

public partial class Maintenance_ApprovalRoute_lookupARRouteAssignment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        if (!Page.IsPostBack)
        {
            initializeControls();
            hfPageIndex.Value = "0";
            hfRowCount.Value = "0";
            bindGrid();
            UpdatePagerLocation();
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_lookupARApproverChecker_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void Maintenance_ApprovalRoute_lookupARApproverChecker_LoadComplete(object sender, EventArgs e)
    {
        hfControl.Value = Request.QueryString["ctrl"].ToString();
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
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
        hfValueId.Value = string.Empty;
        hfValueName.Value = string.Empty;
        bindGrid();
        UpdatePagerLocation();
        txtSearch.Focus();
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValue('" + e.Row.RowIndex + "')");
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')");
        }
    }

    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hfValueId.Value = dgvResult.SelectedRow.Cells[0].Text;
        }
        catch
        {
            hfValueId.Value = string.Empty;
        }
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
    }

    #endregion

    #region Methods
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
                ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
        hfRowCount.Value = "0";
        foreach (DataRow dr in ds.Tables[1].Rows)
            hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    protected string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;
                    
                       WITH TempTable AS (
                     SELECT Row_Number() OVER (Order by [Route ID]) [Row], *
                       FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Route ID]
                          , [Checker 1]
                          , [Checker 2]
                          , [Approver]
                          , [Cost Center Assignment]
                       FROM TempTable
                      WHERE Row between @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT COUNT(Arm_RouteId) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Arm_RouteId [Route ID] 
                                       , dbo.GetControlEmployeeNameV2(Arm_Checker1) [Checker 1]
                                       , dbo.GetControlEmployeeNameV2(Arm_Checker2) [Checker 2]
                                       , dbo.GetControlEmployeeNameV2(Arm_Approver) [Approver]
                                       , dbo.getCostCenterFullNameV2(Arm_CostcenterCode) [Cost Center Assignment]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" FROM T_ApprovalRouteMaster
                                  WHERE Arm_Status = 'A'");

        if (Request.QueryString["ctrl"] == "txtFromRoute")
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "GENERAL"))
            {
                filter += string.Format(@" AND ( Arm_CostcenterCode IN (SELECT Uca_CostCenterCode
                                                                    FROM T_UserCostCenterAccess
                                                                   WHERE Uca_SytemID = 'GENERAL'
                                                                     AND Uca_UserCode = '{0}'
                                                                     AND Uca_Status = 'A')
                                              OR Arm_CostCenterCode = 'ALL' )", Session["userLogged"].ToString());
            }

            filter += @" AND Arm_RouteID IN (SELECT DISTINCT Arm_RouteID
                                               FROM T_EmployeeApprovalRoute
                                              WHERE Arm_Status = 'A' )";
        }

        searchFilter = @" AND ( ( Arm_RouteId LIKE '{0}%' )
                             OR ( dbo.GetControlEmployeeNameV2(Arm_Checker1) LIKE '%{0}%' )
                             OR ( dbo.GetControlEmployeeNameV2(Arm_Checker2) LIKE '%{0}%' )
                             OR ( dbo.GetControlEmployeeNameV2(Arm_Approver) LIKE '%{0}%' )
                             )";


        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
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
    #endregion
}
