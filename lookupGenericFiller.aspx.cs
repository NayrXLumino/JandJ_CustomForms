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

/// <summary>
/// Andre created 20101001:
/// This is located in main folder outside of transactions so that other transaction using T_ColumnFiller can access this without revision
/// Currently using this form is/are:
///     pgeOvertimeIndividual.aspx
/// 
/// Javascript file for this is located in pgeMaster.master
/// </summary>
public partial class pgeLookupGenericFiller : System.Web.UI.Page
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
            LoadComplete += new EventHandler(lookupGenericReportsMultiple_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void lookupGenericReportsMultiple_LoadComplete(object sender, EventArgs e)
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
        hfValue.Value = string.Empty;
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
            hfValue.Value = dgvResult.SelectedRow.Cells[0].Text;
        }
        catch
        {
            hfValue.Value = string.Empty;
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
                      SELECT Row_Number() OVER (Order by [Code]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Code]
                          , [Description]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(Adt_AccountCode) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"SELECT Adt_AccountCode [Code]
                         , Adt_AccountDesc [Description]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" FROM T_AccountDetail
                                 WHERE Adt_AccountType = '{0}'
                                   AND Adt_Status = 'A'", Request.QueryString["lookup"].ToString());

        searchFilter = @"AND  ( ( Adt_AccountCode LIKE '{0}%' )
                             OR ( Adt_AccountDesc LIKE '%{0}%' )
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
    #region OLD Code
    //    protected void Page_Load(object sender, EventArgs e)
    //    {
    //        hfControl.Value = Request.QueryString["ctrl"].ToString();
    //        Panel2.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
    //        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
    //        if (!Page.IsPostBack)
    //        {
    //            PopulateList();
    //        }
    //    }

    //    private void PopulateList()
    //    {
    //        DataSet ds = new DataSet();
    //        string sql = string.Format( @" SELECT Adt_AccountCode [Code]
    //                                            , Adt_AccountDesc [Description]
    //                                         FROM T_AccountDetail
    //                                        WHERE Adt_AccountType = '{0}'
    //                                          AND Adt_Status = 'A' 
    //                                          {1} ", Request.QueryString["lookup"].ToString() , getFilter());
    //        using (DALHelper dal = new DALHelper())
    //        {
    //            try
    //            {
    //                dal.OpenDB();
    //                ds = dal.ExecuteDataSet(sql, CommandType.Text);
    //            }
    //            catch
    //            {
    //                Response.Write("No data was retrieved.");       
    //            }
    //            finally
    //            {
    //                dal.CloseDB();
    //            }
    //        }
    //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
    //        {
    //            dgvList.DataSource = ds.Tables[0];
    //            dgvList.DataBind();
    //        }


    //    }
    //    private string getFilter()
    //    {
    //        string cond = string.Empty;
    //        if (!txtSearch.Text.Equals(string.Empty))
    //        {
    //            cond = string.Format(@" AND ( Adt_AccountCode like '{0}%'
    //                                       OR Adt_AccountDesc like '{0}%')", txtSearch.Text.Trim().Replace("'", ""));
    //        }
    //        return cond;
    //    }
    //    protected void dgvList_RowCreated(object sender, GridViewRowEventArgs e)
    //    {
    //        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    //    }
    //    protected void dgvList_RowDataBound(object sender, GridViewRowEventArgs e)
    //    {
    //        if (e.Row.RowType == DataControlRowType.DataRow)
    //        {
    //            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
    //            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
    //            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.dgvList, "Select$" + e.Row.RowIndex);
    //        }
    //    }
    //    protected void dgvList_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            hfValue.Value = dgvList.SelectedRow.Cells[0].Text;
    //        }
    //        catch
    //        {
    //            hfValue.Value = string.Empty;
    //        }
    //    }
    //    protected void txtSearch_TextChanged(object sender, EventArgs e)
    //    {
    //        PopulateList();
    //    }
    #endregion
}
