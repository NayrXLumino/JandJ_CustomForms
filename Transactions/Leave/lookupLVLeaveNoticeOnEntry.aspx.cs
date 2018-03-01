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

public partial class Transactions_Leave_lookupLVLeaveNoticeOnEntry : System.Web.UI.Page
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
        hfControlNo.Value = string.Empty;
        hfEmployeeId.Value = string.Empty;
        hfEmployeeName.Value = string.Empty;
        hfEmployeeNickName.Value = string.Empty;
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
                      SELECT Row_Number() OVER (Order BY [ID Number]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")   AS temp)
                     SELECT [Control No]
                          , [ID Number]
                          , [Employee Name]
                          , [Nickname]
                          , [Leave Date]
                          , [Leave Type]
                          , [Category]
                          , [Start]
                          , [End]
                          , [Hours]
                          , [Day Unit]
                          , [Cost Center]
                          , [Informed Date]
                          , [Reason]
                          , [Inform Mode]
                          , [Informant]
                          , [Relation]
                          , [Status]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(Eln_LeaveDate) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Eln_ControlNo [Control No]
                          , Eln_EmployeeId [ID Number]
                          , dbo.GetControlEmployeeNameV2(Eln_EmployeeId) [Employee Name]
                          , Emt_NickName [Nickname]
                          , Convert(varchar(10), Eln_LeaveDate, 101) [Leave Date]
                          , Ltm_LeaveDesc [Leave Type]
                          , ISNULL(ADCTGRY.Adt_AccountDesc, '- not applicable -') [Category]
                          , LEFT(Eln_StartTime,2) + ':' + RIGHT(Eln_StartTime,2) [Start]
                          , LEFT(Eln_EndTime,2) + ':' + RIGHT(Eln_EndTime,2) [End]
                          , Eln_LeaveHour [Hours]
                          , Eln_DayUnit [Day Unit]
                          , dbo.getCostCenterFullNameV2(Eln_CostCenter) [Cost Center]
                          , CONVERT(varchar(10),Eln_InformDate,101) 
                            + ' ' 
                            + LEFT(CONVERT(varchar(20),Eln_InformDate,114),5) [Informed Date]
                          , Eln_Reason [Reason]
                          , Eln_InformMode [Inform Mode]
                          , Eln_Informant [Informant]
                          , Eln_InformantRelation [Relation]
                          , AD1.Adt_AccountDesc [Status]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" FROM T_EmployeeLeaveNotice
                                  LEFT JOIN T_EmployeeMaster 
                                    ON Emt_EmployeeId = Eln_EmployeeId
                                  LEFT JOIN T_AccountDetail AD1 
                                    ON AD1.Adt_AccountCode = Eln_Status 
                                   AND AD1.Adt_AccountType =  'WFSTATUS'
                                  LEFT JOIN T_LeaveTypeMaster
                                    ON Ltm_LeaveType = Eln_LeaveType
                                  LEFT JOIN T_AccountDetail ADCTGRY
                                    ON ADCTGRY.Adt_AccountType = 'LVECATEGRY'
                                   AND ADCTGRY.Adt_AccountCode = Eln_LeaveCategory
                                 WHERE Eln_Status = '1'
                                   AND Eln_EmployeeId = '{0}'", Session["userLogged"].ToString());

        searchFilter = @" AND ( ( Eln_ControlNo LIKE '{0}%' )
                             OR ( Eln_EmployeeId LIKE '{0}%' )
                             OR ( dbo.GetControlEmployeeNameV2(Eln_EmployeeId) LIKE '%{0}%' )
                             OR ( Ltm_LeaveDesc LIKE '{0}%' )
                             OR ( Convert(varchar(10), Eln_LeaveDate, 101) LIKE '{0}%' )
                             OR ( ISNULL(ADCTGRY.Adt_AccountDesc, '- not applicable -') LIKE '{0}%' )
                             OR ( LEFT(Eln_StartTime,2) + ':' + RIGHT(Eln_StartTime,2) LIKE '{0}%' )
                             OR ( LEFT(Eln_EndTime,2) + ':' + RIGHT(Eln_EndTime,2) LIKE '{0}%' )
                             OR ( dbo.getCostCenterFullNameV2(Eln_CostCenter) LIKE '{0}%' )
                             OR ( CONVERT(varchar(10),Eln_InformDate,101) 
                                + ' ' 
                                + LEFT(CONVERT(varchar(20),Eln_InformDate,114),5) LIKE '{0}%' )
                             OR ( Eln_Reason LIKE '{0}%' )
                             OR ( Eln_InformMode LIKE '{0}%' )
                             OR ( Eln_Informant LIKE '{0}%' )
                             OR ( Eln_InformantRelation LIKE '{0}%' )
                             OR ( AD1.Adt_AccountDesc LIKE '{0}%' )
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
