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


public partial class Transactions_WorkInformation_lookupShift2 : System.Web.UI.Page
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
        try
        {
            hfDate.Value = Request.QueryString["shiftDate"].ToString();
        }
        catch
        {
            hfDate.Value = "";
        }
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
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 1);
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
        sql.Append(" SELECT COUNT(cnt) FROM (");
        sql.Append(" SELECT distinct Scm_ShiftCode [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        if (Request.QueryString["type"].ToString().ToUpper().Equals("TO"))
        {
            columns = @"SELECT DISTINCT Scm_ShiftCode [Code]
                             , Scm_ShiftDesc
                             --+ '  ('
                             --+ LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
                             --+ ' - '
                             --+ LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
                             --+ '   '
                             --+ LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
                             --+ ' - '
                             --+ LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2)
                             --+ ')' 
                          [Description]";
        }
        else//just the same as code above just coding convention for lookups having different types
        {
            columns = @"SELECT DISTINCT Scm_ShiftCode [Code]
                             , Scm_ShiftDesc
                             --+ '  ('
                             --+ LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
                             --+ ' - '
                             --+ LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
                             --+ '   '
                             --+ LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
                             --+ ' - '
                             --+ LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2)
                             --+ ')' 
                          [Description]";
        }

        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;

        if (Request.QueryString["type"].ToString().ToUpper().Equals("TO"))
        {
            filter = @" FROM T_ShiftCodeMaster
                       WHERE Scm_Status = 'A' ";
        }
        else
        {
            filter = @" FROM T_EmployeeMaster
                       INNER JOIN T_ShiftCodeMaster
                          ON Scm_ShiftCode = Emt_ShiftCode
                       WHERE 1 = 1 ";

            if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
            {
                filter += string.Format(@" AND Emt_CostCenterCode IN (SELECT Uca_CostCenterCode
                                                                       FROM T_UserCostCenterAccess
                                                                      WHERE Uca_SytemID = 'TIMEKEEP'
                                                                        AND Uca_UserCode = '{0}'
                                                                        AND Uca_Status = 'A')", Session["userLogged"].ToString());
            }
        }



        try
        {
            string strDate = Request.QueryString["shiftDate"].ToString();
            DateTime dt;
            if (DateTime.TryParse(strDate, out dt))
            {
                if (Request.QueryString["type"].ToString().ToUpper().Equals("TO"))
                {
                    filter = @" FROM T_ShiftCodeMaster
                       WHERE Scm_Status = 'A' ";
                }
                else
                {
                    filter = string.Format(@" FROM T_ShiftCodeMaster
                            INNER JOIN (
	                            SELECT 
		                            Ell_EmployeeID
		                            ,Ell_ShiftCode 
	                            FROM T_EmployeeLogledger
	                            WHERE Ell_ProcessDate = '{0}'
	
	                            UNION
	
	                            SELECT 
		                            Ell_EmployeeID
		                            ,Ell_ShiftCode 
	                            FROM T_EmployeeLogledgerHist
	                            WHERE Ell_ProcessDate = '{0}'
                            ) AS LEDGERTABLE
                            ON LEDGERTABLE.Ell_ShiftCOde = Scm_ShiftCode
                            INNER JOIN T_EmployeeMaster
                            ON LEDGERTABLE.Ell_EmployeeID = Emt_EmployeeID
                            WHERE 1 = 1 
                            ", dt.ToString("MM/dd/yyyy"));

                    if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
                    {
                        filter += string.Format(@" AND Emt_CostCenterCode IN (SELECT Uca_CostCenterCode
                                                                       FROM T_UserCostCenterAccess
                                                                      WHERE Uca_SytemID = 'TIMEKEEP'
                                                                        AND Uca_UserCode = '{0}'
                                                                        AND Uca_Status = 'A')", Session["userLogged"].ToString());
                    }
                }
            }
        }
        catch
        { 
            
        }

        searchFilter = @" AND ( Scm_ShiftCode like '{0}%'
                             OR Scm_ShiftDesc like '{0}%')";


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
        //if (Convert.ToBoolean(Resources.Resource.LEARSPECIFIC))
        //{
        //    filter += @" AND Scm_DefaultShift=1";
        //}
        return filter;
    }
    #endregion
}
