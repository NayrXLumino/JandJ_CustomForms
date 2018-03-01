using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using MethodsLibrary;
using System.Text;

public partial class pgeLookupSubWork : System.Web.UI.Page
{
    static int pageIndex = 0;
    static int rowCount = 0;
    static int numRows = 100;

    static string[] values = new string[20];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userLogged"].ToString().Equals(string.Empty))
        {
            this.Dispose();
        }
        else
        {
            LoadComplete += new EventHandler(mypop_LoadComplete);
            //Non-Method Initaiization
            if (!Page.IsPostBack)
            {
                pageIndex = 0;
                rowCount = 0;
                FilterSearch();
                UpdatePagerLocation();
            }
        }
        SetFocus(txtSearch);
    }
    #region LoadComplete
    protected void mypop_LoadComplete(object sender, EventArgs e)
    {
        btnSelect.Attributes.Add("OnClick", "javascript:return SendValueToParent();");
        txtSearch.Attributes.Add("OnFocus", "javascript:document.getElementById('txtSearch').select();");
    }
    #endregion

    #region Filter Search with the use of keywords
    protected void FilterSearch()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            try
            {
                string sqlFetch = sqlBuilder();
                ds = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
                GridView1.AutoGenerateColumns = true;
                GridView1.DataSource = ds;
                GridView1.DataBind();

                //string test = GridView1.
                    
                //GridView1.Columns[0].ControlStyle.Width = 100;
                //GridView1.SelectedRow.Cells[0].Text = 'test';
                //GridView1.columns
                try
                {
                    rowCount = int.Parse(ds.Tables[1].Rows[0][0].ToString());
                }
                catch
                {
                    rowCount = 0;
                }
                
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }
    #endregion

    #region sqlBuilder
    protected string sqlBuilder()
    {
        #region NEW Query
        string ret = string.Empty;
        string add = string.Empty;
        string costCenter = Request.QueryString["cct"].ToString();
        string category = Request.QueryString["cat"].ToString();

        StringBuilder sb = new StringBuilder();
#region comment by JOLAN 04-04-2011
        //        sb.Append(@"declare @startIndex int;
//                    set @startIndex = (@pageIndex * @numRow) + 1;
//                
//                    WITH TempTable AS (Select Row_Number() Over (ORDER BY Swm_Costcenter) [Row]
//                          [Swm_Costcenter]
//                          ,[Swm_Category]
//                          ,[Swm_SubWorkCode] AS [Subwork Code]
//                          ,[Swc_Description] AS [Subwork Description]
//                          ,[Swc_MMCode] AS [MM Code]
//                          ,[Swc_MMCodeDesc] AS [MM Code Description]
//                          ,[Swc_MSCode] AS [MS Code]
//                          ,[Swc_MSCodeDesc] AS [MS Code Description]                          
//                     FROM t_subworkmaster                    
//                LEFT JOIN ERP_CPH..E_SubWorkCodeMaster 
//                       ON Swc_AccountCode = Swm_SubWorkCode AND Swc_CostCenterCode = Swm_CostCenter
//                    WHERE 
//                    Swm_Costcenter = '{0}'
//                    --and
//                    --swm_category ='{1}'
//                    and
//                    Swm_Status ='A'
//                    {2}
//                    
//                        )
//                             SELECT 
//                                 [Subwork Code]
//                                ,[Subwork Description]
//                                ,[MM Code]
//                                ,[MM Code Description]
//                                ,[MS Code]
//                                ,[MS Code Description]
//                       FROM TempTable
//                      WHERE Row BETWEEN @startIndex 
//                        AND @startIndex + @numRow - 1
//                        SELECT Count(swm_costcenter) from t_subworkmaster
//                        where 
//                        swm_costcenter = '{0}'
//                        --and
//                        --swm_category ='{1}'
//                        and
//                        swm_status ='A'
//                        {2}
//                                        ");
#endregion
    #region Work Act. Code Query

        if (cbxGlobal.Checked)
        {
            sb.Append(@"declare @startIndex int;
                    set @startIndex = (@pageIndex * @numRow) + 1;
                    Declare @TempTable as Table
                    (
	                    Row int,
	                    Swc_CostCenterCode varchar(12),
	                    Swc_AccountCode varchar(15),
	                    Swc_Description varchar(200),
                        Swc_FBSCode varchar(75),
                        Swc_FBSCodeDesc varchar(200),
	                    Swc_MMCode varchar(50),
	                    Swc_MMCodeDesc varchar(200),
	                    Swc_MSCode varchar(50),
	                    Swc_MSCodeDesc varchar(200),
                        Swm_Category varchar(1)
                    )
                    insert @TempTable
                    Select Row_Number() Over (ORDER BY Swc_AccountCode) [Row]
                          ,[Swc_CostCenterCode]
                          ,[Swc_AccountCode] 
                          ,Swc_Description 
                          , Swc_FBSCode 
                          , Swc_FBSCodeDesc 
                          , Swc_MMCode
                          , Swc_MMCodeDesc
                          , Swc_MSCode
                          , Swc_MSCodeDesc
                          , Swm_Category
                    from {3}..E_SubWorkCodeMaster 
                    join T_SubWorkMaster on Swc_AccountCode = Swm_SubWorkCode
	                    and Swc_CostCenterCode = Swm_Costcenter
                    where 
                    Swc_CostCenterCode = 'ALL'
                    and
                    swc_status ='A'
                    and Swc_UpperLevel = 0
                    {2}
                          SELECT 
                                 Swc_AccountCode [Work Activity Code]
                                , Swm_Category [Category]
                                , Swc_Description [Work Activity Description]
                                , Swc_FBSCode [FBS Code]
                                , Swc_FBSCodeDesc [FBS Description]
                                , Swc_MMCode [MM Code]
                                , Swc_MMCodeDesc [MM Description]
                                , Swc_MSCode [MS Code]
                                , Swc_MSCodeDesc [MS Description]
                       FROM @TempTable
                      WHERE Row BETWEEN @startIndex 
                        AND @startIndex + @numRow - 1
                        SELECT Count(Swc_AccountCode) from @TempTable
                                                ");
        }
        else
        {
            sb.Append(@"declare @startIndex int;
                        set @startIndex = (@pageIndex * @numRow) + 1;
                        Declare @TempTable as Table
                        (
	                        Row int,
	                        Swc_CostCenterCode varchar(12),
	                        Swc_AccountCode varchar(15),
	                        Swc_Description varchar(200),
                            Swc_FBSCode varchar(75),
                            Swc_FBSCodeDesc varchar(200),
	                        Swc_MMCode varchar(50),
	                        Swc_MMCodeDesc varchar(200),
	                        Swc_MSCode varchar(50),
	                        Swc_MSCodeDesc varchar(200),
                            Swm_Category varchar(1)
                        )
                        insert @TempTable
                        Select Row_Number() Over (ORDER BY Swc_AccountCode) [Row]
                              , [Swc_CostCenterCode]
                              , [Swc_AccountCode] 
                              , Swc_Description 
                              , Swc_FBSCode 
                              , Swc_FBSCodeDesc 
                              , Swc_MMCode
                              , Swc_MMCodeDesc
                              , Swc_MSCode
                              , Swc_MSCodeDesc
                              , Swm_Category
                        from {3}..E_SubWorkCodeMaster 
                        join T_SubWorkMaster on Swc_AccountCode = Swm_SubWorkCode
	                        and Swc_CostCenterCode = Swm_Costcenter
                        where 
                        Swc_CostCenterCode = '{0}'
                        and
                        swc_status ='A'
                        and Swc_UpperLevel = 0
                        {2}
                              SELECT 
                                     Swc_AccountCode [Work Activity Code]
                                    , Swm_Category [Category]
                                    , Swc_Description [Work Activity Description]
                                    , Swc_FBSCode [FBS Code]
                                    , Swc_FBSCodeDesc [FBS Description]
                                    , Swc_MMCode [MM Code]
                                    , Swc_MMCodeDesc [MM Description]
                                    , Swc_MSCode [MS Code]
                                    , Swc_MSCodeDesc [MS Description]
                           FROM @TempTable
                          WHERE Row BETWEEN @startIndex 
                            AND @startIndex + @numRow - 1
                            SELECT Count(Swc_AccountCode) from @TempTable
                                                    ");
        }
#endregion
        ret = sb.ToString().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString());


        if (txtSearch.Text.Trim() != string.Empty)
        {
            string searchKey = txtSearch.Text;
            string addWhere = string.Empty;

            #region additional query
            
            addWhere += @"
                                  AND (
                                             [Swc_AccountCode] LIKE '%{0}%'
                                          OR [Swc_Description] LIKE '%{0}%'
                                          OR [Swc_MMCode] LIKE '%{0}%'
                                          OR [Swc_MMCodeDesc] LIKE '%{0}%'
                                          OR [Swc_MSCode] LIKE '%{0}%'
                                          OR [Swc_MSCodeDesc] LIKE '%{0}%'
                                       )";

            searchKey += "&";

            string holder = string.Empty;

            for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
            {
                holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                add += string.Format(addWhere, holder);
            }

            #endregion
        }

        return string.Format(ret, costCenter, category, add, ConfigurationManager.AppSettings["ERP_DB"]);
        
        #endregion
    }
    #endregion

    #region Popup Selection
    protected void Lookup_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnSelect.Enabled = true;
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);

            e.Row.Attributes["onclick"] = "AssignValue('" + e.Row.RowIndex + "');";
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')");
        }
    }
    #endregion

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            pageIndex--;
        else if (((Button)sender).ID == "btnNext")
            pageIndex++;
        FilterSearch();
        UpdatePagerLocation();
    }

    private void UpdatePagerLocation()
    {
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRows.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }
    /// <summary>
    /// 
    /// </summary>

    
    #region [Events]
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        pageIndex = 0;
        rowCount = 0;
        FilterSearch();
        UpdatePagerLocation();
        txtSearch.Focus();
    }
    #endregion

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        GridViewRow grdRow = e.Row;
        
        CommonLookUp.SetGridViewCells(grdRow, new ArrayList());
        TableRow tr = grdRow;
        tr.Cells[0].Attributes.Add("Width", "20%");
    }
    protected void cbxGlobal_CheckedChanged(object sender, EventArgs e)
    {
        FilterSearch();
        UpdatePagerLocation();
    }
}
