using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Text;
using System.Collections;
using System.Configuration;

public partial class Transactions_ManhourAllocation_lookupJSRepSubwork : System.Web.UI.Page
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
            LoadComplete += new EventHandler(Transactions_ManhourAllocation_lookupJSRepSubwork_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void Transactions_ManhourAllocation_lookupJSRepSubwork_LoadComplete(object sender, EventArgs e)
    {
        pnlResult.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
        txtSearch.Attributes.Add("OnFocus", "javascript:getElementById('txtSearch').select();");
        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
        lbxSelected.Attributes.Add("ondblclick", "javascript:removeItem()");
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        populateInclude();
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        bindGrid();
        UpdatePagerLocation();
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        populateInclude();
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        txtSearch.Focus();
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand'; this.style.color='blue';this.style.fontWeight='bold';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='black';;this.style.fontWeight='normal';";
            e.Row.Attributes["onclick"] = "gridSelect('" + e.Row.RowIndex.ToString() + "');";
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')"); ;
        }
    }

    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!lbxSelected.Items.Contains(new ListItem(dgvResult.SelectedRow.Cells[1].Text
                                                    , dgvResult.SelectedRow.Cells[0].Text)))
        {
            lbxSelected.Items.Insert(0, new ListItem(dgvResult.SelectedRow.Cells[1].Text
                                                    , dgvResult.SelectedRow.Cells[0].Text));
            hfValue.Value = formatReturnValue();

        }
    }
    protected void lbxSelected_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbxSelected.Items.RemoveAt(lbxSelected.SelectedIndex);
        hfValue.Value = formatReturnValue();
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }

    protected void dgvResult_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        if (e.NewSelectedIndex == dgvResult.SelectedIndex)
        {
            dgvResult.Attributes.Add("onmouseover", "return SendValueToParent()");
        }
    }
    #endregion

    #region Methods
    //~ query to be changed 
    private void populateInclude()
    {
        if (!hfValue.Value.Trim().Equals(string.Empty))
        {
            lbxSelected.Items.Clear();
            string sql = string.Format(@" SELECT  Swc_AccountCode [Code]
	                                            , Swc_Description [Description]
                                            FROM {0}..E_SubWorkCodeMaster
                                           WHERE Swc_AccountCode {1}", ConfigurationManager.AppSettings["ERP_DB"].ToString()
                                                                        , sqlINFormat(hfValue.Value));
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lbxSelected.Items.Insert(0, new ListItem(ds.Tables[0].Rows[i]["Description"].ToString()
                                                            , ds.Tables[0].Rows[i]["Code"].ToString()));
                }
            }
        }
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

    private string formatReturnValue()
    {
        string value = string.Empty;
        for (int i = 0; i < lbxSelected.Items.Count; i++)
        {
            if (i != 0)
            {
                value += ",";
            }
            value += lbxSelected.Items[i].Value;
        }
        return value;
    }

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

    //~ query to be changed 
    protected string SQLBuilder()
    {
        string queryLossTime = getFilters();
        queryLossTime = queryLossTime.Remove(queryLossTime.IndexOf(" AND Jsd_CostCenter = Slm_Costcenter"),
                                                " AND Jsd_CostCenter = Slm_Costcenter)".Length);
        queryLossTime = queryLossTime.Replace("T_JobSplitHeader", "T_JobSplitHeaderLossTime")
            .Replace("T_JobSplitDetail", "T_JobSplitDetailLossTime");

        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;
                    
                        WITH TempTable AS (
                      SELECT Row_Number() OVER (Order by [Code], [Description]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(queryLossTime);
        sql.Append(@")   AS temp)
                     SELECT [Code]
                          , [Description]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1;
                    ");

        sql.Append(@"WITH TempCountTable AS (
                        SELECT DISTINCT Jsd_SubWorkCode ");
        sql.Append(getFilters());
        sql.Append(" UNION SELECT DISTINCT Jsd_SubWorkCode ");
        sql.Append(queryLossTime);
        sql.Append(") SELECT COUNT(Jsd_SubWorkCode) FROM TempCountTable");

        return sql.ToString();
    }

    //~ query to be changed 
    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT DISTINCT Jsd_SubWorkCode [Code]
	                      , Swc_Description [Description]";
        return columns;
    }

    //~ query to be changed 
    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" 
                    FROM T_JobSplitHeader
                    JOIN T_JobSplitDetail ON Jsh_ControlNo = Jsd_ControlNo AND Jsh_Status = '9'
                    LEFT JOIN T_SalesMaster
                      ON Jsd_JobCode = Slm_DashJobCode
		             AND Jsd_ClientJobNo = Slm_ClientJobNo
                     AND Jsd_CostCenter = Slm_Costcenter
		            left join {0}..E_SubWorkCodeMaster ON Swc_AccountCode = LTRIM(Jsd_SubWorkCode)
                        and (Swc_CostCenterCode = Slm_Costcenter OR Swc_CostCenterCode = 'ALL')
                   WHERE dbo.getCostcenterFullnameV2(Slm_Costcenter) IS NOT NULL", ConfigurationManager.AppSettings["ERP_DB"].ToString());

        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
        {
            filter += string.Format(@" AND Slm_Costcenter IN (SELECT Uca_CostCenterCode
                                                                FROM T_UserCostCenterAccess
                                                               WHERE Uca_SytemID = 'TIMEKEEP'
                                                                 AND Uca_UserCode = '{0}'
                                                                 AND Uca_Status = 'A')"
                , Session["userLogged"].ToString());
        }

        searchFilter = @" AND  ( ( Swc_AccountCode LIKE '{0}%' )
                              OR ( Swc_Description LIKE '%{0}%' )
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