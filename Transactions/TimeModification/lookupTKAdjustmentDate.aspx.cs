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

public partial class Transactions_TimeModification_lookupTKAdjustmentDate : System.Web.UI.Page
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
        hfDate.Value = string.Empty;
        hfShiftDesc.Value = string.Empty;
        hfType.Value = string.Empty;
        hfIn1.Value = string.Empty;
        hfOut1.Value = string.Empty;
        hfIn2.Value = string.Empty;
        hfOut2.Value = string.Empty;
        hfLogControl.Value = string.Empty;
        hfSIn1.Value = string.Empty;
        hfSOut1.Value = string.Empty;
        hfSIn2.Value = string.Empty;
        hfSOut2.Value = string.Empty;
        hfSType.Value = string.Empty;
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

        }
        catch (Exception ex)
        {
            CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
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
                ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()).Replace("@MINPRD", Convert.ToInt32(CommonMethods.getParamterValue("MINPASTPRD")).ToString()), CommandType.Text);
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
                     declare @pastPeriod as varchar(7);
                     declare @Current as varchar(7);
                     declare @TIMEMODGAP as int;
						 SET @TIMEMODGAP = (SELECT Pmt_NumericValue 
											  FROM T_ParameterMaster
											 WHERE Pmt_ParameterID = 'TIMEMODGAP');
                         SET @Current = (SELECT Ppm_payPeriod
                                           FROM T_PayPeriodMaster 
                                          WHERE Ppm_CycleIndicator = 'C');

                         SET @startIndex = (@pageIndex * @numRow) + 1;
                         SET @pastPeriod = (SELECT ISNULL(MIN(PayPeriod),@Current)
                                             FROM ( SELECT TOP @MINPRD Ppm_PayPeriod [PayPeriod]
	                                                  FROM T_PayPeriodMaster
 	                                                 WHERE Ppm_CycleIndicator = 'P'
	                                                 ORDER BY 1 DESC) AS TEMP);
                        WITH TempTable AS (
                      SELECT Row_Number() OVER (Order by Convert(datetime,[Date]) DESC) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(" UNION ");
        sql.Append(getColumns());
        sql.Append(getFilters().Replace("T_EmployeeLogledger", "T_EmployeeLogLedgerHist"));
        sql.Append(@")   AS temp)
                     SELECT [Date]
                          , [DoW]
                          , [Shift]
                          , [DayCode]
                          , [Actual Time In 1]
                          , [Actual Time Out 1]
                          , [Actual Time In 2]
                          , [Actual Time Out 2]
                          , [Shift Time In]
                          , [Shift Break Start]
                          , [Shift Break End]
                          , [Shift Time Out]
                          , [Shift Type]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(Ell_ProcessDate) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"   SELECT CONVERT(varchar(20), Ell_ProcessDate, 101) AS[Date]
                            , LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) [DoW]
                            , '['+Scm_ShiftCode+'] '
                            + LEFT(Scm_ShiftTimeIn,2)+':'+RIGHT(Scm_ShiftTimeIn,2)
                            + '-'
                            + LEFT(Scm_ShiftBreakStart,2)+':'+RIGHT(Scm_ShiftBreakStart,2)
                            + '  '
                            + LEFT(Scm_ShiftBreakEnd,2)+':'+RIGHT(Scm_ShiftBreakEnd,2)
                            + '-'
                            + LEFT(Scm_ShiftTimeOut,2)+':'+RIGHT(Scm_ShiftTimeOut,2) AS[Shift]
                            , Ell_DayCode AS [DayCode]
                            , CASE WHEN (Ell_ActualTimeIn_1 ='0000'OR Ell_ActualTimeIn_1 = '') 
	                               THEN '' 
	                               ELSE LEFT(Ell_ActualTimeIn_1,2)+':'+RIGHT(Ell_ActualTimeIn_1,2) 
                               END AS [Actual Time In 1]
                            , CASE WHEN(Ell_ActualTimeOut_1 ='0000'OR Ell_ActualTimeOut_1 = '') 
	                               THEN '' 
	                               ELSE LEFT(Ell_ActualTimeOut_1,2)+':'+RIGHT(Ell_ActualTimeOut_1,2) 
                               END AS [Actual Time Out 1]
                            , CASE WHEN(Ell_ActualTimeIn_2 ='0000'OR Ell_ActualTimeIn_2 = '') 
	                               THEN '' 
	                               ELSE LEFT(Ell_ActualTimeIn_2,2)+':'+RIGHT(Ell_ActualTimeIn_2,2)
                               END AS [Actual Time In 2]
                            , CASE WHEN(Ell_ActualTimeOut_2 ='0000'OR Ell_ActualTimeOut_2 = '') 
	                               THEN '' 
	                               ELSE LEFT(Ell_ActualTimeOut_2,2)+':'+RIGHT(Ell_ActualTimeOut_2,2)
                               END AS [Actual Time Out 2]
                            , LEFT(Scm_ShiftTimeIn,2)+':'+RIGHT(Scm_ShiftTimeIn,2) [Shift Time In]
                            , LEFT(Scm_ShiftBreakStart,2)+':'+RIGHT(Scm_ShiftBreakStart,2) [Shift Break Start]
                            , LEFT(Scm_ShiftBreakEnd,2)+':'+RIGHT(Scm_ShiftBreakEnd,2) [Shift Break End]
                            , LEFT(Scm_ShiftTimeOut,2)+':'+RIGHT(Scm_ShiftTimeOut,2) [Shift Time Out]
                            , CASE (Scm_ScheduleType) 
                              WHEN 'D' THEN 'DAY SHIFT'
                              WHEN 'N' THEN 'NIGHT SHIFT'
                              WHEN 'G' THEN 'GRAVEYARD SHIFT'
                              WHEN 'S' THEN 'SWING SHIFT'
                               END [Shift Type]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@"    FROM T_EmployeeLogledger
                                     LEFT JOIN T_ShiftCodeMaster 
                                       ON Scm_ShiftCode = Ell_ShiftCode
                                    WHERE (Ell_TagTimeMod = 'N')--only for CHIYODA
                                      AND Ell_Employeeid = '{0}'
                                      AND Ell_ProcessDate < getdate()
                                      AND Ell_PayPeriod >= @pastPeriod ", Request.QueryString["ei"].ToString());
        if (!Convert.ToBoolean(Resources.Resource.BYPASSTIMEVERIFICATION))
        {
            filter += @"              AND ((
                                             (
                                                 (Ell_ActualTimeIn_1+Ell_ActualTimeOut_1+Ell_ActualTimeIn_2='000000000000' and Ell_ActualTimeOut_2 <> '0000') 
                                              OR (Ell_ActualTimeOut_1+Ell_ActualTimeIn_2+Ell_ActualTimeOut_2='000000000000' and Ell_ActualTimeIn_1 <> '0000') 
                                              OR (Ell_ActualTimeIn_1+Ell_ActualTimeIn_2+Ell_ActualTimeOut_2 = '000000000000' and Ell_ActualTimeOut_1 <> '0000') 
                                              OR (Ell_ActualTimeIn_1 = '0000' and Ell_ActualTimeOut_1 <> '0000' and Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOut_2 <> '0000') 
                                              OR (Ell_ActualTimeIn_2='0000' and Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOut_1 <> '0000' and Ell_ActualTimeOut_2 <> '0000') 
                                              OR (Ell_ActualTimeOut_1='0000' and Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOut_2 <> '0000') 
                                              OR (Ell_ActualTimeIn_1+Ell_ActualTimeOut_1+Ell_ActualTimeOut_2 ='000000000000' and Ell_ActualTimeIn_2 <> '0000') 
                                              OR (Ell_ActualTimeOut_2 ='0000' and Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOut_1 <> '0000' and Ell_ActualTimeIn_2 <> '0000') 
                                              OR (Ell_ActualTimeIn_1+Ell_ActualTimeIn_2 ='00000000' and Ell_ActualTimeOut_1 <> '0000' and Ell_ActualTimeOut_2 <> '0000') 
                                              OR (Ell_ActualTimeOut_1+Ell_ActualTimeOut_2='00000000' and Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeIn_2 <> '0000')
                                              )
                                            ) 
                                           OR (Ell_ActualTimeIn_1+Ell_ActualTimeIn_2+Ell_ActualTimeOut_1+Ell_ActualTimeOut_2 ='0000000000000000' AND Ell_Daycode <> 'REG' AND Ell_EncodedOvertimeAdvHr + Ell_EncodedOvertimePostHr <> 0) 
                                           OR (Ell_ActualTimeIn_1+Ell_ActualTimeIn_2+Ell_ActualTimeOut_1+Ell_ActualTimeOut_2 ='0000000000000000' AND Ell_Daycode = 'REG' AND Ell_EncodedPayLeaveHr + Ell_EncodedNoPayLeaveHr = 0)

                                           OR (Ell_ActualTimeIn_1+Ell_ActualTimeOut_1 <> '00000000' 
											AND(ABS(((Convert(int,Substring(Ell_ActualTimeIn_1,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeIn_1,3,2)))
				                                 -((Convert(int,Substring(Ell_ActualTimeOut_1,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeOut_1,3,2)))) <= @TIMEMODGAP))
                                           OR (Ell_ActualTimeOut_1+Ell_ActualTimeOut_2 <> '00000000' 
											AND(ABS(((Convert(int,Substring(Ell_ActualTimeOut_1,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeOut_1,3,2)))
				                                 -((Convert(int,Substring(Ell_ActualTimeOut_2,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeOut_2,3,2)))) <= @TIMEMODGAP))
                                           OR (Ell_ActualTimeOut_1+Ell_ActualTimeIn_2 <> '00000000' 
											AND(ABS(((Convert(int,Substring(Ell_ActualTimeOut_1,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeOut_1,3,2)))
				                                 -((Convert(int,Substring(Ell_ActualTimeIn_2,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeIn_2,3,2)))) <= @TIMEMODGAP))
                                           OR (Ell_ActualTimeIn_1+Ell_ActualTimeOut_2 <> '00000000' 
											AND(ABS(((Convert(int,Substring(Ell_ActualTimeIn_1,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeIn_1,3,2)))
				                                 -((Convert(int,Substring(Ell_ActualTimeOut_2,1,2)) * 60 ) + Convert(int,Substring(Ell_ActualTimeOut_2,3,2)))) <= @TIMEMODGAP))
                                          )";
            if (Convert.ToBoolean(Resources.Resource.NOINOUTINTIMMOD))
            {
                filter += @" AND Ell_ActualTimeIn_1+Ell_ActualTimeIn_2+Ell_ActualTimeOut_1+Ell_ActualTimeOut_2 <> '0000000000000000' ";
            }
        }

        searchFilter = @" AND ( ( CONVERT(varchar(20), Ell_ProcessDate, 101) LIKE '%{0}%' )
                             OR ( LEFT(UPPER(datename(DW, Ell_ProcessDate)), 3) LIKE '{0}%' )
                             OR ( LEFT(Scm_ShiftTimeIn,2)+':'+RIGHT(Scm_ShiftTimeIn,2)
                                + '-'
                                + LEFT(Scm_ShiftBreakStart,2)+':'+RIGHT(Scm_ShiftBreakStart,2)
                                + '  '
                                + LEFT(Scm_ShiftBreakEnd,2)+':'+RIGHT(Scm_ShiftBreakEnd,2)
                                + '-'
                                + LEFT(Scm_ShiftTimeOut,2)+':'+RIGHT(Scm_ShiftTimeOut,2) LIKE '%{0}%' )
                             OR ( Ell_DayCode LIKE '{0}%' )
                             OR ( CASE WHEN (Ell_ActualTimeIn_1 ='0000'OR Ell_ActualTimeIn_1 = '') 
	                                   THEN '' 
	                                   ELSE LEFT(Ell_ActualTimeIn_1,2)+':'+RIGHT(Ell_ActualTimeIn_1,2) 
                                   END LIKE '{0}%' )
                             OR ( CASE WHEN(Ell_ActualTimeOut_1 ='0000'OR Ell_ActualTimeOut_1 = '') 
	                                   THEN '' 
	                                   ELSE LEFT(Ell_ActualTimeOut_1,2)+':'+RIGHT(Ell_ActualTimeOut_1,2) 
                                   END LIKE '{0}%' )
                             OR ( CASE WHEN(Ell_ActualTimeIn_2 ='0000'OR Ell_ActualTimeIn_2 = '') 
	                                   THEN '' 
	                                   ELSE LEFT(Ell_ActualTimeIn_2,2)+':'+RIGHT(Ell_ActualTimeIn_2,2)
                                   END LIKE '{0}%' )
                             OR ( CASE WHEN(Ell_ActualTimeOut_2 ='0000'OR Ell_ActualTimeOut_2 = '') 
	                                   THEN '' 
	                                   ELSE LEFT(Ell_ActualTimeOut_2,2)+':'+RIGHT(Ell_ActualTimeOut_2,2)
                                   END LIKE '{0}%' )
                             OR ( LEFT(Scm_ShiftTimeIn,2)+':'+RIGHT(Scm_ShiftTimeIn,2) LIKE '{0}%' )
                             OR ( LEFT(Scm_ShiftBreakStart,2)+':'+RIGHT(Scm_ShiftBreakStart,2) LIKE '{0}%' )
                             OR ( LEFT(Scm_ShiftBreakEnd,2)+':'+RIGHT(Scm_ShiftBreakEnd,2) LIKE '{0}%' )
                             OR ( LEFT(Scm_ShiftTimeOut,2)+':'+RIGHT(Scm_ShiftTimeOut,2) LIKE '{0}%' )
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
