/*File revision no. W2.1.00001 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Text;
using System.Collections;

public partial class Transactions_WorkInformation_lookupCostCenter : System.Web.UI.Page
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
            LoadComplete += new EventHandler(Transactions_WorkInformation_lookupShift_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void Transactions_WorkInformation_lookupShift_LoadComplete(object sender, EventArgs e)
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
        hfValueCode.Value = string.Empty;
        hfValueDesc.Value = string.Empty;
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

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
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
                      SELECT Row_Number() OVER (Order by [Cost Center Code]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Cost Center Code]
                          , [Description]
                       FROM TempTable
                        WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        if (Request.QueryString["type"].ToString().Equals("FROM"))
        {
            sql.Append(" SELECT Count(DISTINCT Emt_CostCenterCode) [cnt]");
        }
        if (Request.QueryString["type"].ToString().Equals("TO"))
        {
            sql.Append(" SELECT Count(DISTINCT Cct_CostCenterCode) [cnt]");
        }
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;

        if (Request.QueryString["type"].ToString().Equals("FROM"))
        {
            columns = @"SELECT DISTINCT Emt_CostCenterCode [Cost Center Code]
                                 , dbo.GetCostCenterFullNameV2(Emt_CostCenterCode) [Description]";
        }
        else if (Request.QueryString["type"].ToString().Equals("TO"))
        {
            columns = @"SELECT DISTINCT Cct_CostCenterCode [Cost Center Code]
                                 , dbo.GetCostCenterFullNameV2(Cct_CostCenterCode)[Description]";
        }
        
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;

        if (Request.QueryString["type"].ToString().Equals("FROM"))
        {
            filter = string.Format(@" FROM T_EmployeeMaster
                        JOIN T_UserCostcenterAccess
	                        ON (Emt_CostcenterCode = Uca_CostcenterCode OR 'ALL' = Uca_CostCenterCode) AND Uca_SytemID = 'TIMEKEEP'
		                        AND Uca_Usercode = '{0}'
	                        WHERE LEFT(Emt_JobStatus,1) = 'A'",Session["userLogged"].ToString());

            if (!txtSearch.Text.Equals(string.Empty))
            {
                searchFilter = @" AND ( Emt_CostCenterCode like '{0}%'
                                        OR dbo.GetCostCenterFullNameV2(Emt_CostCenterCode) like '%{0}%')";
            }
        }
        else if (Request.QueryString["type"].ToString().Equals("TO"))
        {
            filter = @" FROM T_CostCenter
                             WHERE Cct_Status = 'A'";

            if (!txtSearch.Text.Equals(string.Empty))
            {
                searchFilter = @" AND ( Cct_CostCenterCode like '{0}%'
                                        OR dbo.GetCostCenterFullNameV2(Cct_CostCenterCode) like '%{0}%')";
            }
        }


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
