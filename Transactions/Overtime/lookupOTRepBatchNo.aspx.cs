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

public partial class Transactions_Overtime_lookupOTRepBatch : System.Web.UI.Page
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
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
        //    e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
        //    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.dgvResult, "Select$" + e.Row.RowIndex);
        //}
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
        if (!lbxSelected.Items.Contains(new ListItem("["
                                                    + dgvResult.SelectedRow.Cells[0].Text
                                                    + "] "
                                                    + dgvResult.SelectedRow.Cells[1].Text
                                                    + " - "
                                                    + dgvResult.SelectedRow.Cells[2].Text
                                                    , dgvResult.SelectedRow.Cells[0].Text)))
        {
            lbxSelected.Items.Insert(0, new ListItem("["
                                                    + dgvResult.SelectedRow.Cells[0].Text
                                                    + "] "
                                                    + dgvResult.SelectedRow.Cells[1].Text
                                                    + " - "
                                                    + dgvResult.SelectedRow.Cells[2].Text
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
        //CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
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

    private void populateInclude()
    {
        if (!hfValue.Value.Trim().Equals(string.Empty))
        {
            lbxSelected.Items.Clear();
            string sql = string.Format(@" SELECT DISTINCT Eot_BatchNo [Batch Number]
                                               , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]
                                               , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                                               , LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) [Start]
                                               , LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) [End]
                                            FROM T_EmployeeOvertime
                                            LEFT JOIN T_ParameterMasterExt
                                              ON Pmx_ParameterValue = Eot_OvertimeType
                                             AND Pmx_ParameterID = 'OTTYPE'
                                           WHERE Eot_BatchNo {0}", sqlINFormat(hfValue.Value));
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
                    lbxSelected.Items.Insert(0, new ListItem( "["
                                                            + ds.Tables[0].Rows[i]["Batch Number"].ToString()
                                                            + "] "
                                                            + ds.Tables[0].Rows[i]["OT Date"].ToString()
                                                            + " - "
                                                            + ds.Tables[0].Rows[i]["Type"].ToString()
                                                            , ds.Tables[0].Rows[i]["Batch Number"].ToString()));
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
                      SELECT Row_Number() OVER (Order by [Batch Number]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@")   AS temp)
                     SELECT [Batch Number]
                          , [OT Date]
                          , [Type]
                          , [Start]
                          , [End]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT COUNT(*) FROM (");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT DISTINCT Eot_BatchNo [Batch Number]
                          , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]
                          , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                          , LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) [Start]
                          , LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) [End] ";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" FROM T_EmployeeOvertime
                    INNER JOIN (SELECT Cct_CostCenterCode, Clm_LineCode
			                        FROM T_CostCenter
			                        LEFT JOIN E_CostCenterLineMaster  
                                    ON Clm_CostCenterCode = Cct_CostCenterCode
			                        AND Clm_Status = 'A'
			                        WHERE Cct_status = 'A' ) CctLine 
                        ON Cct_CostCenterCode = Eot_CostCenter
                        AND ISNULL(Clm_LineCode,'') = ISNULL(Eot_CostCenterLine,'') 
                    INNER JOIN T_UserCostcenterAccess 
                        ON CASE WHEN Uca_CostCenterCode = 'ALL' 
                                THEN Cct_CostCenterCode 
		                        ELSE Uca_CostCenterCode 
	                        END = Eot_CostCenter
                        AND Uca_SytemID = 'OVERTIME'
                        AND Uca_UserCode = '{0}'
                        AND Uca_Status = 'A'
                    LEFT JOIN T_ParameterMasterExt
                      ON Pmx_ParameterValue = Eot_OvertimeType
                     AND Pmx_ParameterID = 'OTTYPE'
                   WHERE ISNULL(Eot_BatchNo,'') <> '' ", Session["userLogged"].ToString());

//        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
//        {
//            filter += string.Format(@" AND Eot_CostCenter IN (SELECT Uca_CostCenterCode
//                                                                FROM T_UserCostCenterAccess
//                                                               WHERE Uca_SytemID = 'OVERTIME'
//                                                                 AND Uca_UserCode = '{0}'
//                                                                 AND Uca_Status = 'A')", Session["userLogged"].ToString());
//        }

        searchFilter = @" AND  ( ( Eot_BatchNo LIKE '{0}%' )
                              OR ( Convert(varchar(10),Eot_OvertimeDate,101) LIKE '{0}%')
                              OR ( ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') LIKE '{0}%')
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
