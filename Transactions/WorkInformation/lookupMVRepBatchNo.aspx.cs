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

public partial class Transactions_WorkInformation_lookupMVRepBatchNo : System.Web.UI.Page
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
            LoadComplete += new EventHandler(Transactions_WorkInformation_lookupMVRepBatchNo_LoadComplete);
        }
    }

    #region Events
    private void initializeControls()
    {
        txtSearch.Focus();
        UpdatePagerLocation();
        hfNumRows.Value = Resources.Resource.LOOKUPPAGEITEMS;
    }

    void Transactions_WorkInformation_lookupMVRepBatchNo_LoadComplete(object sender, EventArgs e)
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
            string sql = string.Format(@" SELECT DISTINCT Mve_BatchNo [Batch Number]
                                               , Convert(varchar(10), Mve_EffectivityDate, 101) [Effectivity Date]
                                               , MOVETYPE.Adt_AccountDesc [Type]
                                               , CASE WHEN (Mve_Type = 'S')
				                                      THEN '[ ' 
				                                         + S1.Scm_ShiftCode 
					                                     + ' ] ' 
					                                     + ' '
					                                     + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
					                                     + ' - '
					                                     + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2)
				                                      WHEN (Mve_Type = 'G')
				                                      THEN WTF.Adt_AccountDesc + '/' + WGF.Adt_AccountDesc 
				                                      WHEN (Mve_Type = 'C')
				                                      THEN dbo.getCostCenterFullNameV2(Mve_From)
				                                      ELSE Mve_From
				                                  END [From]
                                               , CASE WHEN (Mve_Type = 'S')
					                                  THEN '[ ' 
						                                 + S2.Scm_ShiftCode 
						                                 + ' ] ' 
						                                 + ' '
						                                 + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
						                                 + ' - '
						                                 + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2)
						                              WHEN (Mve_Type = 'G')
						                              THEN WTT.Adt_AccountDesc + ' / ' + WGT.Adt_AccountDesc 
						                              WHEN (Mve_Type = 'C')
						                              THEN dbo.getCostCenterFullNameV2(Mve_To)
						                              ELSE Mve_To
						                          END [To]
                                            FROM T_Movement
					                        LEFT JOIN T_AccountDetail MOVETYPE
					                          ON MOVETYPE.Adt_AccountCode = Mve_Type
					                         AND MOVETYPE.Adt_AccountType = 'MOVETYPE' 
					                        LEFT JOIN T_ShiftCodeMaster S1
					                          ON S1.Scm_ShiftCode = Mve_From
					                         AND Mve_Type = 'S'
					                        LEFT JOIN T_ShiftCodeMaster S2
					                          ON S2.Scm_ShiftCode = Mve_To
					                         AND Mve_Type = 'S'
					                        LEFT JOIN T_AccountDetail WTF
					                          ON WTF.Adt_AccountCode = LTRIM(LEFT(Mve_From, 3))
					                         AND WTF.Adt_AccountType = 'WORKTYPE'
					                         AND Mve_Type = 'G'
					                        LEFT JOIN T_AccountDetail WGF
					                          ON WGF.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
					                         AND WGF.Adt_AccountType = 'WORKGROUP'
					                         AND Mve_Type = 'G'
					                        LEFT JOIN T_AccountDetail WTT
					                          ON WTT.Adt_AccountCode = LTRIM(LEFT(Mve_To, 3))
					                         AND WTT.Adt_AccountType = 'WORKTYPE'
					                         AND Mve_Type = 'G'
					                        LEFT JOIN T_AccountDetail WGT
					                          ON WGT.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
					                         AND WGT.Adt_AccountType = 'WORKGROUP'
					                         AND Mve_Type = 'G'
                                            WHERE Mve_BatchNo {0}", sqlINFormat(hfValue.Value));
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
                    lbxSelected.Items.Insert(0, new ListItem("["
                                                            + ds.Tables[0].Rows[i]["Batch Number"].ToString()
                                                            + "] "
                                                            + ds.Tables[0].Rows[i]["From"].ToString()
                                                            + " - "
                                                            + ds.Tables[0].Rows[i]["To"].ToString()
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
        //sql.Append(" UNION ");
        //sql.Append(getColumns());
        //sql.Append(getFilters().Replace("T_EmployeeOvertime", "T_EmployeeOvertimeHist"));
        sql.Append(@")   AS temp)
                     SELECT [Batch Number]
                          , [Effectivity Date]
                          , [Type]
                          , [From]
                          , [To]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT COUNT(DISTINCT Mve_BatchNo) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT DISTINCT Mve_BatchNo [Batch Number]
                          , Convert(varchar(10), Mve_EffectivityDate, 101) [Effectivity Date]
                          , MT.Adt_AccountDesc [Type]
                          , CASE WHEN (Mve_Type = 'S')
                                    THEN '[ ' 
                                    + S1.Scm_ShiftCode 
                                    + ' ] ' 
                                    + ' '
                                    + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
                                    + ' - '
                                    + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2)
                                WHEN (Mve_Type = 'G')
                                    THEN WTF.Adt_AccountDesc + '/' + WGF.Adt_AccountDesc 
                                WHEN (Mve_Type = 'C')
                                    THEN dbo.getCostCenterFullNameV2(Mve_From)
                                ELSE Mve_From
                            END [From]
                          , CASE WHEN (Mve_Type = 'S')
                                THEN '[ ' 
                                + S2.Scm_ShiftCode 
                                + ' ] ' 
                                + ' '
                                + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
                                + ' - '
                                + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2)
                                WHEN (Mve_Type = 'G')
                                THEN WTT.Adt_AccountDesc + ' / ' + WGT.Adt_AccountDesc 
                                WHEN (Mve_Type = 'C')
                                THEN dbo.getCostCenterFullNameV2(Mve_To)
                                ELSE Mve_To
                            END [To] ";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = @" FROM T_Movement
                        LEFT JOIN T_AccountDetail MT 
                        ON Mve_Type = MT.Adt_AccountCode
                        AND MT.Adt_AccountType = 'MOVETYPE'
                        LEFT JOIN T_ShiftCodeMaster S1
                        ON S1.Scm_ShiftCode = Mve_From
                        AND Mve_Type = 'S'
                       LEFT JOIN T_ShiftCodeMaster S2
                        ON S2.Scm_ShiftCode = Mve_To
                        AND Mve_Type = 'S'
                       LEFT JOIN T_AccountDetail WTF
                        ON WTF.Adt_AccountCode = LTRIM(LEFT(Mve_From, 3))
                        AND WTF.Adt_AccountType = 'WORKTYPE'
                        AND Mve_Type = 'G'
                       LEFT JOIN T_AccountDetail WGF
                        ON WGF.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
                        AND WGF.Adt_AccountType = 'WORKGROUP'
                        AND Mve_Type = 'G'
                       LEFT JOIN T_AccountDetail WTT
                        ON WTT.Adt_AccountCode = LTRIM(LEFT(Mve_To, 3))
                        AND WTT.Adt_AccountType = 'WORKTYPE'
                        AND Mve_Type = 'G'
                       LEFT JOIN T_AccountDetail WGT
                        ON WGT.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
                        AND WGT.Adt_AccountType = 'WORKGROUP'
                        AND Mve_Type = 'G'
                   WHERE ISNULL(Mve_BatchNo,'') <> ''";

        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "TIMEKEEP"))
        {
            filter += string.Format(@" AND Mve_CostCenter IN (SELECT Uca_CostCenterCode
                                                                FROM T_UserCostCenterAccess
                                                               WHERE Uca_SytemID = 'TIMEKEEP'
                                                                 AND Uca_UserCode = '{0}'
                                                                 AND Uca_Status = 'A')", Session["userLogged"].ToString());
        }

        searchFilter = @" AND  ( ( Mve_BatchNo LIKE '{0}%' )
                              OR ( Convert(varchar(10),Mve_EffectivityDate,101) LIKE '{0}%')
                              OR ( CASE WHEN (Mve_Type = 'S')
                                            THEN '[ ' 
                                            + S1.Scm_ShiftCode 
                                            + ' ] ' 
                                            + ' '
                                            + LEFT(S1.Scm_ShiftTimeIn,2) + ':' + RIGHT(S1.Scm_ShiftTimeIn,2)
                                            + ' - '
                                            + LEFT(S1.Scm_ShiftTimeOut,2) + ':' + RIGHT(S1.Scm_ShiftTimeOut,2)
                                        WHEN (Mve_Type = 'G')
                                            THEN WTF.Adt_AccountDesc + '/' + WGF.Adt_AccountDesc 
                                        WHEN (Mve_Type = 'C')
                                            THEN dbo.getCostCenterFullNameV2(Mve_From)
                                        ELSE Mve_From
                                    END LIKE '{0}%')
                              OR ( CASE WHEN (Mve_Type = 'S')
                                        THEN '[ ' 
                                        + S2.Scm_ShiftCode 
                                        + ' ] ' 
                                        + ' '
                                        + LEFT(S2.Scm_ShiftTimeIn,2) + ':' + RIGHT(S2.Scm_ShiftTimeIn,2)
                                        + ' - '
                                        + LEFT(S2.Scm_ShiftTimeOut,2) + ':' + RIGHT(S2.Scm_ShiftTimeOut,2)
                                        WHEN (Mve_Type = 'G')
                                        THEN WTT.Adt_AccountDesc + ' / ' + WGT.Adt_AccountDesc 
                                        WHEN (Mve_Type = 'C')
                                        THEN dbo.getCostCenterFullNameV2(Mve_To)
                                        ELSE Mve_To
                                    END LIKE '{0}%')
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