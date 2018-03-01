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
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    static DataSet dsView;
    static int pageIndex = 0;
    static int rowCount = 0;
    static int numRows = 100;
    static string userCostCenters = "";
    static ArrayList colIndex = new ArrayList();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFMANHOURDTL"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                Panel8.Attributes.Add("OnScroll", "javascript:SetDivPosition();");
                DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFOTREP");
                if (dr != null)
                {
                    bool canCheck = Convert.ToBoolean(dr["Ugt_CanCheck"]);
                    bool canApprove = Convert.ToBoolean(dr["Ugt_CanApprove"]);

                    if (!canCheck)
                        txtEmpName.Text = Session["userLogged"].ToString();
                    txtEmpName.Visible = canCheck;
                    Label2.Visible = canCheck;
                    btnEmployee.Visible = canCheck;
                    hiddenEmpFlag.Value = canCheck.ToString();
                    PopulateDdlBilling();
                }

                InitializeButtons();
                pageIndex = 0;
                rowCount = 0;
                UpdatePagerLocation();
            }
        }
    }

    private void PopulateDdlBilling()
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"  SELECT Adt_AccountDesc as [Description]
                                       , Adt_AccountCode as [Value]
                                    FROM T_AccountDetail
                                   WHERE Adt_AccountType = 'BILLCYCLE'
                                     AND Adt_Status = 'A'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
                ddlBilling.Items.Clear();
                ddlBilling.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddlBilling.Items.Add(new ListItem(dr["Description"].ToString(), dr["Value"].ToString()));
        }
    }

    #region Button Events
    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (txtEmpName.Visible)
            txtEmpName.Text = "";
        txtCostCenter.Text = "";

        dtpSplitDateFrom.Reset();
        dtpSplitDateTo.Reset();

        pageIndex = 0;
        rowCount = 0;
        UpdatePagerLocation();
        grdView.DataSource = null;
        grdView.DataBind();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                string title = "";
                DataTable dtGrid;
                dtGrid = GetData(SqlBuilder(true)).Tables[0];
                title = "Man Hours Details Report";
                if (dtGrid.Rows.Count > 0)
                {
                    InitDataTable(dtGrid);
                    GridView grdv = new GridView();
                    grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
                    grdv.DataSource = dtGrid;
                    grdv.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(dtGrid.Columns.Count, grdv.Rows.Count, title, initializeHeader());
                    ctrl[1] = grdv;
                    ExportExcelHelper.ExportControl2(ctrl, title);
                }
                else
                    MessageBox.Show("Filter have been changed! Please regenerate the table.");
            }
            catch
            {
                MessageBox.Show("Some error occurred during exporting to Excel.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (this.grdView.Rows.Count > 0)
        {
            try
            {
                string title = "";
                DataTable dtGrid;
                dtGrid = GetData(SqlBuilder().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString())).Tables[0];
                title = "Leave & Absence Report";
                if (dtGrid.Rows.Count > 0)
                {
                    InitDataTable(dtGrid);
                    GridView grdv = new GridView();
                    grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
                    grdv.DataSource = dtGrid;
                    grdv.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.HeaderPanelOptionERP(dtGrid.Columns.Count, grdv.Rows.Count, title, initializeHeader());
                    ctrl[1] = grdv;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe');</script>");
                }
                else
                    MessageBox.Show("No Records Found!");
            }
            catch
            {
                MessageBox.Show("Some error occurred during initialization of page for printing.\nPlease Try Again!");
            }
        }
        else
            MessageBox.Show("No Records Found!");
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        pageIndex = 0;
        rowCount = 0;
        dsView = GetData(SqlBuilder().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString()));
        LoadGridView();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFMANHOURDTL", Session["userLogged"].ToString(), true, Session["userLogged"].ToString());

    }
    #endregion

    #region Other Methods
    protected DataSet GetData(string sqlQuery)
    {
        DataSet dsTemp = null;
        string sqlFetch = sqlQuery;
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsTemp = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
                dsView = null;
            }
            finally
            {
                dal.CloseDB();
            }
        }
        return dsTemp;
    }

    protected string SqlBuilder()
    {
        return SqlBuilder(false);
    }
    protected string SqlBuilder(bool forReport)
    {
        StringBuilder sql = new StringBuilder();
        StringBuilder sqlFilter = new StringBuilder();

        string startdate = string.Empty;
        string enddate = string.Empty;
        string billingCycle = string.Empty;

        if (!dtpSplitDateFrom.IsNull)
        {
            startdate = dtpSplitDateFrom.Date.ToString("MM/dd/yyyy");
        }
        if (!dtpSplitDateTo.IsNull)
        {
            enddate = dtpSplitDateTo.Date.ToString("MM/dd/yyyy");
        }

        if(ddlBilling.SelectedValue.ToString().Equals(string.Empty))
        {
            billingCycle = "ALL";
        }
        else
        {
            billingCycle = ddlBilling.SelectedValue.ToString();
        }

        sql.Append(string.Format(@"
                    Declare @start as varchar(20)
                    Declare @End as varchar(20)
                    Declare @BillingCycle as varchar(20)

					Declare @unsplitted as table
					(employeeid varchar(15), 
					jobsplitdate datetime,
					acthours decimal(9,2))

                    Set @start = '" + startdate + @"'
                    Set @BillingCycle = '" + billingCycle + @"'
                    Set @end =  case when '" + "" + @"' = '' then getdate() else '" + "" + @"' end

                    insert into @unsplitted
                    select jsh_employeeid, jsh_jobsplitdate, sum(Jsd_PlanHours)
                    from T_JobSplitHeaderLossTime
                    inner join T_JobSplitDetailLossTime on Jsd_Controlno = Jsh_Controlno
                          and jsd_status = '9'
                    where jsh_jobsplitdate between @start and @end
                    and  jsh_status ='9'
                    group by jsh_employeeid, jsh_jobsplitdate

                 declare @startIndex int;
                 set @startIndex = {0}(@pageIndex * @numRow) + 1;
                    ", (forReport)? "0;--": ""));

        sql.Append("\nWITH TempTable AS (Select Row_Number() Over (ORDER BY [Costcenter], [Nickname], [Job Split Date], [Seq No]) [Row],*");
        sql.Append(@"FROM ( SELECT");

        sql.Append(getColumns());
        sql.Append(getFilters());

        sql.Append(string.Format(@") as Temp)
                    SELECT  [Costcenter]
                          , [Employee ID]
                          , [Nickname]
                          , [Job Split Date]
                          , [Shift Code]
                          , [Day Code]
                          , [ActualTimeIn_1]
                          , [ActualTimeOut_1]
                          , [ActualTimeIn_2]
                          , [ActualTimeOut_2]
                          , [Flex]
                          , [Time Mod]
                          , [OT Start Time]
                          , [OT End Time]
                          , [OT Hours]
                          , [Leave Type]
                          , [Leave Hour]
                          , [Control Number]
                          , [Seq No]
                          , [Job Start Time]
                          , [Job End Time]
                          , [Dash Job Code]
                          , [Client Job No]
                          , [Client Job Name]
                          , [Dash Work Code]
                          , [Sub Work Code]
                          , [Plan Hours]
                          , [Actual Hours]
                          , [Unsplitted Hours]
	                  FROM TempTable
                   {0}WHERE Row between @startIndex and @startIndex + @numRow - 1
					ORDER BY [Costcenter]
                          , [Nickname]
                          , [Job Split Date] 
                          , [Seq No]
                    ", (forReport)? "--": "  "));

        sql.Append(@"SELECT COUNT(T_JobSplitHeader.Jsh_Costcenter)");
        sql.Append(getFilters());

        return sql.ToString();
    }
    private string getColumns()
    {
        StringBuilder sqlColumns = new StringBuilder();

        sqlColumns.Append(@"
                          dbo.getCostCenterFullName(T_JobSplitHeader.Jsh_Costcenter) as [Costcenter]
                          , T_JobSplitHeader.Jsh_EmployeeID [Employee ID]
                          , Emt_Nickname as [Nickname]
                          , Convert(Char(10),T_JobSplitHeader.Jsh_Jobsplitdate,101) as [Job Split Date]
                          , Mhd_ShiftCode as [Shift Code]
                          , Mhd_DayCode [Day Code]
                          , Case when Mhd_ActualTimeIn_1 = '0000' then ''
                                Else Left(Mhd_ActualTimeIn_1,2)+':'+ Right(Mhd_ActualTimeIn_1,2) End as [ActualTimeIn_1]
                          , Case when Mhd_ActualTimeOut_1 = '0000' then ''
                                Else Left(Mhd_ActualTimeOut_1,2)+':'+ Right(Mhd_ActualTimeOut_1,2) End as [ActualTimeOut_1]
                          , Case when Mhd_ActualTimeIn_2 = '0000' then ''
                                Else Left(Mhd_ActualTimeIn_2,2)+':'+ Right(Mhd_ActualTimeIn_2,2) End as [ActualTimeIn_2]
                          , Case when Mhd_ActualTimeOut_2 = '0000' then ''
                                Else Left(Mhd_ActualTimeOut_2,2)+':'+ Right(Mhd_ActualTimeOut_2,2) End as [ActualTimeOut_2]
                          , Case when Mhd_Flex = 'T' then 'PENDING' 
                                  when Mhd_Flex = 'A' then 'APPROVED'
                                else '' End as [Flex]
                          , Case when Mhd_TimeMod = 'T' then 'PENDING' 
                                  when Mhd_TimeMod = 'A' then 'APPROVED'
                                else '' End as [Time Mod]
                          , Case when Len(Mhd_OTStartTime) = 0 then ''
                                Else Left(Mhd_OTStartTime,2)+':'+ Right(Mhd_OTStartTime,2) End as [OT Start Time]
                          , Case when Len(Mhd_OTEndTime) = 0 then ''
                                Else Left(Mhd_OTEndTime,2)+':'+ Right(Mhd_OTEndTime,2) End as [OT End Time]
                          , Mhd_OTHours as [OT Hours]
                          , Mhd_LeaveType as [Leave Type]
                          , Mhd_LeaveHour as [Leave Hour]
                          , Mhd_controlno as [Control Number]
                          , Mhd_Seqno as [Seq No]
                          , Left(T_JobSplitDetail.Jsd_StartTime,2) + ':' + Right(T_JobSplitDetail.Jsd_StartTime,2) as [Job Start Time]
                          , Left(T_JobSplitDetail.Jsd_EndTime,2) + ':' + Right(T_JobSplitDetail.Jsd_EndTime,2) as [Job End Time]
                          , T_JobSplitDetail.Jsd_JobCode as [Dash Job Code]
                          , T_JobSplitDetail.Jsd_ClientJobNo as [Client Job No]
                          , Slm_ClientJobName as [Client Job Name]
                          , Adt1.Adt_AccountDesc as [Dash Work Code]
                          , Adt2.Adt_AccountDesc  as [Sub Work Code]
                          , T_JobSplitDetail.Jsd_PlanHours as [Plan Hours]
                          , Mhd_ActualHour as [Actual Hours]
                          , Isnull(acthours, 0) as [Unsplitted Hours]");

        return sqlColumns.ToString();
    }

    private string getFilters()
    {
        StringBuilder sql = new StringBuilder();

        sql.Append(@"
                    From dbo.T_ManHourDetails
                    Inner Join T_JobSplitHeader on T_JobSplitHeader.Jsh_Controlno = Mhd_Controlno
                          and Jsh_Jobsplitdate >= @start
                    Inner Join T_JobSplitDetail on T_JobSplitDetail.Jsd_Controlno = Mhd_ControlNo
                          and T_JobSplitDetail.Jsd_Seqno = Mhd_Seqno
                    Inner Join T_EmployeeMaster on Emt_EmployeeID = T_JobSplitHeader.Jsh_EmployeeID
                    Inner Join T_SalesMaster on Slm_DashJobCode = T_JobSplitDetail.Jsd_JobCode
                          and Slm_ClientJobNo = T_JobSplitDetail.Jsd_ClientJobNo
                    Left Join T_ClientHeader on Clh_ClientCode = Slm_ClientCode
                    Left Join T_Accountdetail Adt1 on Adt1.Adt_AccountCode = Slm_DashWorkCode
                          and Adt1.Adt_accountType = 'WRKCODE'
                    Left Join T_Accountdetail Adt2 on Adt2.Adt_AccountCode = T_JobSplitDetail.Jsd_SubWorkCode
                          and Adt2.Adt_accountType = 'SUBWRKCODE'
                    Left Join @unsplitted on employeeid = Jsh_employeeid
                          and  jobsplitdate = jsh_jobsplitdate
                    WHERE (Clh_BillingCycle = @BillingCycle or @BillingCycle = 'ALL')");

        #region Filters
        #region Cost Center Access
        if (userCostCenters != "" && !userCostCenters.Contains("ALL"))//Andre editted to include self cost center
            if (hiddenEmpFlag.Value.ToString().ToUpper().Equals("TRUE"))
                sql.Append("\nAND Jsh_CostCenter in (@ownCostCenter,@costCenterAcc)".Replace("@costCenterAcc", userCostCenters.Replace("x", "'").Replace("y", ",").Substring(0, userCostCenters.Length - 1)).Replace("@ownCostCenter", getOwnCostCenter()));
        #endregion
        #region Leave
        //Employee
        if (txtEmpName.Text != "")
        {
            ArrayList arr = CommonLookUp.DivideString(txtEmpName.Text);
            if (arr.Count > 0)
            {
                string id = "", idCode = "", lastName = "", firstName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    if (CommonLookUp.isNumeric(arr[i].ToString(), System.Globalization.NumberStyles.Integer))
                        id += string.Format("{0},", arr[i].ToString());
                    idCode += string.Format("\nOR Emt_Nickname like '{0}%'", arr[i].ToString().Trim());
                    lastName += string.Format("\nOR Jsh_EmployeeId like '{0}%'", arr[i].ToString().Trim());
                    firstName += string.Format("\nOR Jsh_EmployeeId like '{0}%'", arr[i].ToString().Trim());
                }
                sql.Append("\nAND (");
                if (id != "")
                    sql.Append(string.Format("Jsh_EmployeeId in ({0})", id.Substring(0, id.Length - 1)));
                else
                    sql.Append("\n 1 = 0");
                sql.Append(idCode + ")");
            }
        }

        if (txtCostCenter.Text != "")
        {
            ArrayList arr = CommonLookUp.DivideString(txtCostCenter.Text);
            if (arr.Count > 0)
            {
                string costCenterCode = "", costCenterName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    costCenterCode += string.Format("\nOR Jsh_CostCenter like '{0}%'", arr[i].ToString().Trim());
                    costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsh_CostCenter) like '{0}%'", arr[i].ToString().Trim());
                }
                sql.Append("\nAND (");
                sql.Append("\n 1 = 0");
                sql.Append(costCenterCode);
                sql.Append(costCenterName + ")");
            }
        }

        #endregion
        #endregion
        return sql.ToString();
    }

    private void LoadGridView()
    {
        if (dsView != null)
        {
            DataTable dt = dsView.Tables[0].Copy();
            InitDataTable(dt);
            foreach (DataRow dr in dsView.Tables[1].Rows)
                rowCount += Convert.ToInt32(dr[0]);
            grdView.DataSource = dt;
            grdView.DataBind();
        }
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            pageIndex--;
        else if (((Button)sender).ID == "btnNext")
            pageIndex++;
        dsView = GetData(SqlBuilder().Replace("@pageIndex", pageIndex.ToString()).Replace("@numRow", numRows.ToString()));
        LoadGridView();
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
    #endregion

    #region Initializing Methods
    private void InitDataTable(DataTable dt)
    {
    }

    private void InitializeButtons()
    {
        userCostCenters = "";
        DataTable dt = CommonLookUp.GetUserCostCenterCode(Session["userId"].ToString(), "TIMEKEEP");
        if (dt != null)
        {
            foreach (DataRow dr in dt.Rows)
            {
                userCostCenters += string.Format("x{0}xy", dr[0].ToString());
            }

            btnEmployee.OnClientClick = string.Format("return OpenPopupLookupEmployee('T_JobSplitHeader','Jsh_EmployeeID','T_JobSplitHeader','Jsh_CostCenter','{0}','txtEmpName','Employee Lookup'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
            btnCostCenter.OnClientClick = string.Format("return OpenPopupLookupCostCenter('T_JobSplitHeader','Jsh_CostCenter','T_JobSplitHeader','{0}','txtCostCenter','Cost Center Lookup'); return true;", userCostCenters.Substring(0, userCostCenters.Length - 1));
        }
        else
            userCostCenters = "xNo Cost Center Accessxy";
    }

    private void InitalizeColumnIndex(DataTable dt)
    {
        colIndex.Clear();
    }

    private string initializeHeader()
    {
        string options = "";
        if (userCostCenters != "")
            options += "Cost Center Access: " + userCostCenters.Substring(0, userCostCenters.Length - 1).Replace("x", "'").Replace("y", ",") + "; ";
        if (txtEmpName.Text != "")
            options += "Employee(s): " + txtEmpName.Text + "; ";
        if (txtCostCenter.Text != "")
            options += "Cost Centers(s): " + txtCostCenter.Text.Trim() + "; ";

        //Inform Date
        if (!dtpSplitDateTo.IsNull && !dtpSplitDateFrom.IsNull)
            if (dtpSplitDateTo.Date == dtpSplitDateFrom.Date)
                options += "Job Split Date: " + dtpSplitDateFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Job Split Date: " + dtpSplitDateTo.Date + " - " + dtpSplitDateFrom.Date + "; ";
        else if (dtpSplitDateTo.IsNull && !dtpSplitDateFrom.IsNull)
            options += "Job Split Date: From " + dtpSplitDateFrom.Date + "; ";
        else if (!dtpSplitDateTo.IsNull && dtpSplitDateFrom.IsNull)
            options += "Job Split Date: To " + dtpSplitDateFrom.Date + "; ";
        return options.Trim();
    }
    #endregion

    #region GridView Events
    protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdView, "Select$" + e.Row.RowIndex);
        }
    }

    protected void grdView_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void grdView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, colIndex);
    }
    #endregion

    protected void rblOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        pageIndex = 0;
        rowCount = 0;
        grdView.DataSource = null;
        grdView.DataBind();
        UpdatePagerLocation();
    }

    private string getOwnCostCenter()
    {
        string sql = string.Format(@"
                    SELECT Emt_CostCenterCode
                      FROM T_EmployeeMaster'
                     WHERE Emt_EmployeeID = '{0}'", Session["userLogged"].ToString());
        string temp = string.Empty;

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                temp = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        return "'"+temp+"'";
    }
}
