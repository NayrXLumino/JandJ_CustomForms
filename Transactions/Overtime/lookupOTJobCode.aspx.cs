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
using CommonLibrary;
using MethodsLibrary;

/// <summary>
/// Lookup is not yet finished.
/// Lacking for this is the indication for jobs if common or specific. Refer to old code for logic(DASH Job Lookup)
/// </summary>
public partial class Transactions_Overtime_lookupOTJobCode : System.Web.UI.Page
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
            fillCostCenterAccessDropDown();
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
            e.Row.Attributes.Add("onclick", "javascript:return AssignValue('" + e.Row.RowIndex + "')"); ;
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')");
        }
    }

    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hfJobCode.Value = dgvResult.SelectedRow.Cells[0].Text;
            hfClientJobNo.Value = dgvResult.SelectedRow.Cells[1].Text;
            hfClientJobName.Value = dgvResult.SelectedRow.Cells[2].Text;
        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            hfJobCode.Value = string.Empty;
            hfClientJobNo.Value = string.Empty;
            hfClientJobName.Value = string.Empty;
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
                      SELECT Row_Number() OVER (Order by [Job Code]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Job Code]
                          , [Client Job No]
                          , [Client Job Name]
                          , [Work Code]
                          , [FWBS Code]
                          , [Cost Center]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(Slm_DashJobCode) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"SELECT Slm_DashJobCode AS [Job Code]
                         , Slm_ClientJobNo AS [Client Job No]
                         , Slm_ClientJobName AS [Client Job Name]
                         , Slm_DashWorkCode AS [Work Code]
                         , Slm_ClientFWBSCode AS [FWBS Code]
                         , Slm_Costcenter AS [Cost Center]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = @"FROM T_SalesMaster 
                  WHERE Slm_Status IN ('A','R')";

        

        if (ddlCostCenter.SelectedValue.Equals("ALL"))
        {
            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME") && !cbxExtension.Checked)
            {
                filter += string.Format(@"  AND Slm_Costcenter IN (SELECT Uca_CostcenterCode
                                                                     FROM T_UserCostcenterAccess
                                                                    WHERE Uca_UserCode = '{0}'
                                                                      AND Uca_SytemID = 'OVERTIME')", Session["userLogged"].ToString());
            }
        }
        else
        {
            filter += string.Format(@"AND Slm_Costcenter = '{0}'", ddlCostCenter.SelectedValue.ToString());
        }


        searchFilter = @" AND ( ( Slm_DashJobCode LIKE '{0}%' )
                             OR ( Slm_ClientJobNo LIKE '%{0}%' )
                             OR ( Slm_ClientJobName LIKE '%{0}%' )
                             OR ( Slm_DashWorkCode LIKE '%{0}%' )
                             OR ( Slm_ClientFWBSCode LIKE '%{0}%' )
                             OR ( Slm_Costcenter LIKE '%{0}%' )
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

    private void fillCostCenterAccessDropDown()
    {
        string sql = @" SELECT Uca_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Uca_CostCenterCode) [Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_UserCode = @UserCode
                           AND Uca_SytemId = 'OVERTIME'
                           AND Uca_CostCenterCode <> 'ALL' 

                        SELECT DISTINCT Uca_CostCenterCode [Code]
                             , dbo.getCostcenterFullNameV2(Uca_CostCenterCode) [Desc]
                          FROM T_UserCostCenterAccess
                         WHERE Uca_SytemId = 'OVERTIME'
                           AND Uca_CostCenterCode <> 'ALL' ";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());

        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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
        ddlCostCenter.Items.Clear();
        ddlCostCenter.Items.Add(new ListItem("ALL", "ALL"));
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0 && !cbxExtension.Checked)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ddlCostCenter.Items.Add(new ListItem( ds.Tables[0].Rows[i]["Desc"].ToString()
                                                        , ds.Tables[0].Rows[i]["Code"].ToString()));
                }
            }
            else
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    ddlCostCenter.Items.Add(new ListItem( ds.Tables[1].Rows[i]["Desc"].ToString()
                                                        , ds.Tables[1].Rows[i]["Code"].ToString()));
                }
            }
        }

    }
    #endregion
}
