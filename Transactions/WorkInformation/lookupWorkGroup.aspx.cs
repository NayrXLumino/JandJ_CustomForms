using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Text;
using System.Collections;

public partial class Transactions_WorkInformation_lookupWorkGroup : System.Web.UI.Page
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
            LoadComplete += new EventHandler(Transactions_WorkInformation_lookupWorkGroup_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void Transactions_WorkInformation_lookupWorkGroup_LoadComplete(object sender, EventArgs e)
    {
        hfControl.Value = Request.QueryString["ctrl"].ToString();
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        hfValueCode1.Value = string.Empty;
        hfValueCode2.Value = string.Empty;
        hfValueDesc.Value = string.Empty;
        bindGrid();
        UpdatePagerLocation();
        txtSearch.Focus();
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
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 2);
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
                      SELECT Row_Number() OVER (Order by [Work Type]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Work Type]
                          , [Work Group]
                          , [Description]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT COUNT(cnt) FROM (");
        sql.Append(@" SELECT DISTINCT REPLICATE(' ', 3 - LEN(AD1.Adt_AccountCode)) + AD1.Adt_AccountCode 
                                    + REPLICATE(' ', 3 - LEN(AD2.Adt_AccountCode)) + AD2.Adt_AccountCode [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        if (Request.QueryString["type"].ToString().ToUpper().Equals("TO"))
        {
            columns = @"SELECT DISTINCT Cal_WorkType [Work Type]
                             , Cal_WorkGroup [Work Group]
                             , ISNULL(AD1.Adt_AccountDesc, '') + ' / ' + ISNULL(AD2.Adt_AccountDesc,'') [Description]";
        }
        else//just the same as code above just coding convention for lookups having different types
        {
            columns = @"SELECT DISTINCT RTRIM(Ell_WorkType) [Work Type]
                             , RTRIM(Ell_WorkGroup) [Work Group]
                             , AD1.Adt_AccountDesc + ' / ' + AD2.Adt_AccountDesc [Description] ";
        }
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;

        if (Request.QueryString["type"].ToString().ToUpper().Equals("TO"))
        {
            filter = @" FROM T_CalendarGroup
                       INNER JOIN T_AccountDetail as AD1
                          ON AD1.Adt_AccountType = 'WORKTYPE' 
                         AND AD1.Adt_AccountCode = Cal_WorkType
                       INNER JOIN T_AccountDetail as AD2
                          ON AD2.Adt_AccountType = 'WORKGROUP'
                         AND AD2.Adt_AccountCode = Cal_WorkGroup ";
        }
        else
        {
            filter = @" FROM T_EmployeeLogLedger
                       INNER JOIN T_AccountDetail as AD1
                          ON AD1.Adt_AccountType = 'WORKTYPE' 
                         AND AD1.Adt_AccountCode = Ell_WorkType
                       INNER JOIN T_AccountDetail as AD2
                          ON AD2.Adt_AccountType = 'WORKGROUP'
                         AND AD2.Adt_AccountCode = Ell_WorkGroup "; 
        }

        searchFilter = @" AND  ( ( AD1.Adt_AccountCode LIKE '{0}%' )
                             OR ( AD1.Adt_AccountDesc LIKE '%{0}%' )
                             OR ( AD2.Adt_AccountCode LIKE '{0}%' )
                             OR ( AD2.Adt_AccountDesc LIKE '%{0}%' )
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