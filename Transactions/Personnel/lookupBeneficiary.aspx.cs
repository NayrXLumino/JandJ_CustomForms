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

public partial class Transactions_Personnel_lookupBeneficiary : System.Web.UI.Page
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
        hfAccident.Value = string.Empty;
        hfBIR.Value = string.Empty;
        hfBirthdate.Value = string.Empty;
        hfCancelledDate.Value = string.Empty;
        hfDeceasedDate.Value = string.Empty;
        hfFirstname.Value = string.Empty;
        hfHierarchyCode.Value = string.Empty;
        hfHierarchyDesc.Value = string.Empty;
        hfHMO.Value = string.Empty;
        hfInsurance.Value = string.Empty;
        hfLastname.Value = string.Empty;
        hfMiddlename.Value = string.Empty;
        hfRelationCode.Value = string.Empty;
        hfRelationDesc.Value = string.Empty;
        hfOccupation.Value = string.Empty;
        hfCompany.Value = string.Empty;
        hfGender.Value = string.Empty;
        hfCivilStatus.Value = string.Empty;
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
                      SELECT Row_Number() OVER (Order by [Seq No]) [Row], *
                        FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@")  AS temp)
                     SELECT [Last Name]
                          , [First Name]
                          , [Middle Name] 
                          , [Occupation]
                          , [Company]        
                          , [Gender]
                          , [Civil Status]                               
                          , [Birthdate]
                          , [Relation Code] 
                          , [Relation Desc]
                          , [Hierarchy Code]
                          , [Hierarchy Desc]
                          , [HMO Dependent]
                          , [Insurance Dependent]
                          , [BIR Dependent]
                          , [Accident Dependent]                          
                          , [Deceased Date] 
                          , [Cancel Date]
                          , [Seq No]
                       FROM TempTable
                      WHERE Row between
                        @startIndex and @startIndex + @numRow - 1");
        sql.Append(" SELECT SUM(cnt) FROM (");
        sql.Append(" SELECT Count(Ebm_LastName) [cnt]");
        sql.Append(getFilters());
        sql.Append(") AS Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @" SELECT Ebm_BenSeqNo [Seq No]
                          , Ebm_LastName [Last Name]
                          , Ebm_FirstName [First Name]
                          , Ebm_MiddleName [Middle Name]
                          , Ebm_Occupation [Occupation] 
                          , Ebm_Company [Company]       
                          , Ebm_Gender [Gender]
                          , Ebm_CivilStatus[Civil Status]     
                          , Convert(varchar(10), Ebm_Birthdate, 101) [Birthdate]
                          , Ebm_Relation [Relation Code] 
                          , AD1.Adt_AccountDesc [Relation Desc]
                          , Ebm_Hierarchy [Hierarchy Code]
                          , AD2.Adt_AccountDesc [Hierarchy Desc]
                          , CASE WHEN Ebm_BIRReport = 0 THEN 'FALSE' ELSE 'TRUE' END [BIR Dependent]
                          , CASE WHEN Ebm_HMODep = 0 THEN 'FALSE' ELSE 'TRUE' END [HMO Dependent]
                          , CASE WHEN Ebm_InsuranceDep = 0 THEN 'FALSE' ELSE 'TRUE' END [Insurance Dependent]
                          , CASE WHEN Ebm_AccidentInsurance = 0 THEN 'FALSE' ELSE 'TRUE' END [Accident Dependent]                         
                          , Convert(varchar(10), Ebm_DeceaseDate, 101) [Deceased Date] 
                          , Convert(varchar(10), Ebm_CancelDate, 101) [Cancel Date]
                          ";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        string searchFilter = string.Empty;
        filter = string.Format(@" FROM T_EmployeeBeneficiary 
                                   LEFT JOIN T_AccountDetail AD1
                                     ON AD1.Adt_AccountCode = Ebm_Relation
                                    AND AD1.Adt_AccountType = 'RELATION'
                                   LEFT JOIN T_AccountDetail AD2
                                     ON AD2.Adt_AccountCode = Ebm_Hierarchy
                                    AND AD2.Adt_AccountType = 'HIERARCHDP'                                
                                  WHERE Ebm_EmployeeId = '{0}' 
                                    AND Ebm_EmployeeId+Ebm_BenSeqNo NOT IN (SELECT But_EmployeeId+But_SeqNo
                                                                            FROM T_BeneficiaryUpdate
                                                                           WHERE But_Status IN ('1','3','5','7')
																			 AND But_Type = 'U'
                                                                             AND But_EmployeeId = '{0}')  ", Request.QueryString["id"].ToString());

        searchFilter = @" AND ( ( Ebm_LastName LIKE '{0}%' )
                             OR ( Ebm_FirstName LIKE '{0}%' )
                             OR ( Ebm_MiddleName LIKE '{0}%' )
                             OR ( Convert(varchar(10), Ebm_Birthdate, 101) LIKE '%{0}%' )
                             OR ( Ebm_Relation LIKE '{0}%' )
                             OR ( AD1.Adt_AccountDesc LIKE '%{0}%' )
                             OR ( Ebm_Hierarchy LIKE '{0}%' )
                             OR ( AD2.Adt_AccountDesc LIKE '%{0}%' )
                             OR ( Convert(varchar(10), Ebm_DeceaseDate, 101) LIKE '%{0}%' )
                             OR ( Convert(varchar(10), Ebm_CancelDate, 101) LIKE '%{0}%' ) 
                             OR (Ebm_Gender LIKE '{0}%')
                             OR (Ebm_CivilStatus LIKE '{0}%')
                             OR (Ebm_Occupation LIKE '{0}%')
                             OR (Ebm_Company LIKE '{0}%') )";


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
