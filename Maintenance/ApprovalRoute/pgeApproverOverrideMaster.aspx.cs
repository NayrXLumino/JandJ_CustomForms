using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Collections;

public partial class Maintenance_ApprovalRoute_pgeApproverOverrideMaster : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    GeneralBL GNBL = new GeneralBL();

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFRTEMASTER"))
                {
                    Response.Redirect(@"../../index.aspx?pr=ur");
                }
                else
                {
                    InitializeControls();
                    hfPageIndex.Value = "0";
                    hfRowCount.Value = "0";
                    Page.PreRender += new EventHandler(Page_PreRender);
                    BindGrid();
                    UpdatePagerLocation();
                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                }
            }
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeApproverOverrideMaster_LoadComplete);
        }
    }

    void Maintenance_ApprovalRoute_pgeApproverOverrideMaster_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        try
        {
            CheckBox cbxTemp = (CheckBox)dgvResult.HeaderRow.FindControl("chkBoxAll");
            cbxTemp.Attributes.Add("onClick", "javascript:SelectAll();");
        }
        catch
        {

        }
        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        txtChecker1Id.Attributes.Add("readOnly", "true");
        txtChecker1Name.Attributes.Add("readOnly", "true");
        txtUserCodeId.Attributes.Add("readOnly", "true");
        txtUserCodeName.Attributes.Add("readOnly", "true");
        txtChecker1EditId.Attributes.Add("readOnly", "true");
        txtChecker1EditName.Attributes.Add("readOnly", "true");
        btnChecker1.OnClientClick = string.Format("javascript:return lookupApprovalOverrideCheckerApprover()");
        txtChecker1Id.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnChecker1.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtChecker1Name.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        transactions.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnBatchChecker.OnClientClick = string.Format("javascript:return lookupAORepEmployee()");
        chkAll.Attributes.Add("OnClick", "javascript:checkAll(event)");
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
        InitializeTransactions();
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            if (Page.IsValid)
            {
                string err = string.Empty;
                switch (btnX.Text.ToUpper())
                {
                    case "NEW":
                        #region NEW
                        SetState(STATE.NEW);
                        InitializeEmployee();
                        txtChecker1Id.Text = "";
                        txtChecker1Name.Text = "";
                        hTransactions.Value = "";
                        #endregion
                        break;
                    case "CREATE":
                        #region CREATE
                        if (CheckValid(STATE.NEW))
                        {
                            Transact(STATE.NEW);
                            SetState(STATE.CANCEL);
                            BindGrid();
                            UpdatePagerLocation();
                        }
                        #endregion
                        break;
                    case "SAVE":
                        #region SAVE
                        if (CheckValid(STATE.EDIT))
                        {
                            Transact(STATE.EDIT);
                            SetState(STATE.CANCEL);
                            BindGrid();
                            UpdatePagerLocation();
                        }
                        #endregion
                        break;
                    default:
                        break;
                }
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }

    protected void btnY_Click(object sender, EventArgs e)
    {
        switch (btnY.Text.ToUpper())
        {
            case "EDIT":
                #region EDIT
                List<ApproverOverrideMaster> listApproverOverrideMaster = GetCheckedRows();
                if (listApproverOverrideMaster.Count > 0)
                {
                    SetState(STATE.EDIT);
                    txtUserCodeId.Text = "";
                    txtUserCodeName.Text = "";
                    txtChecker1EditId.Text = "";
                    txtChecker1EditName.Text = "";
                    hTransactionsEdit.Value = "";
                    foreach (ApproverOverrideMaster approverOverride in listApproverOverrideMaster)
                    {
                        txtUserCodeId.Text += approverOverride.UserCode + ",";
                        txtUserCodeName.Text += approverOverride.UserName + ",";
                        txtChecker1EditId.Text += approverOverride.ApproverChecker + ",";
                        txtChecker1EditName.Text += approverOverride.ApproverCheckerName + ",";
                        foreach (Transactions tr in approverOverride.Transactions)
                        {
                            hTransactionsEdit.Value += tr.TransactionCode + "|";
                        }
                        hTransactionsEdit.Value += ",";
                    }
                    txtChecker1EditId.Focus();
                }
                else
                {
                    MessageBox.Show("No record selected.");
                }
                #endregion
                break;
            case "CANCEL":
                #region CANCEL
                SetState(STATE.CANCEL);
                txtSearch.Text = "";
                BindGrid();
                UpdatePagerLocation();
                #endregion
                break;
            default:
                break;
        }
    }

    protected void btnZ_Click(object sender, EventArgs e)
    {
        switch (btnZ.Text.ToUpper())
        {
            case "DELETE":
                #region DELETE
                List<ApproverOverrideMaster> listApproverOverrideMaster = GetCheckedRows();
                if (listApproverOverrideMaster.Count > 0)
                {
                    SetState(STATE.CANCEL);
                    Dictionary<string, string> listApproverDelete = new Dictionary<string, string>();
                    using (DALHelper dal = new DALHelper())
                    {
                        try
                        {
                            dal.OpenDB();
                            dal.BeginTransaction();
                            foreach (ApproverOverrideMaster approverOverride in listApproverOverrideMaster)
                            {
                                DeleteApproverOverride(approverOverride, dal);
                                if (!listApproverDelete.ContainsKey(approverOverride.UserCode))
                                {
                                    listApproverDelete.Add(approverOverride.UserCode, approverOverride.SeqNo);
                                }
                            }
                            foreach (string userCode in listApproverDelete.Keys)
                            {
                                InsertApproverOverrideTrail(userCode, listApproverDelete[userCode], Session["userLogged"].ToString(), dal);
                            }
                            dal.CommitTransaction();
                            MessageBox.Show("Successfully Deleted.");
                        }
                        catch (Exception ex)
                        {
                            dal.RollBackTransaction();
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            dal.CloseDB();
                        }
                    }
                    BindGrid();
                    UpdatePagerLocation();
                }
                else
                {
                    MessageBox.Show("No record selected.");
                }
                #endregion
                break;
            default:
                break;
        }
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes.Add("onclick", "javascript:ColorSelected('" + e.Row.RowIndex + "')");
        }
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        BindGrid();
        UpdatePagerLocation();
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        BindGrid();
        UpdatePagerLocation();
        Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
    }

    #endregion

    #region Functions

    private bool CheckValid(STATE state)
    {
        bool isValid = true;
        string message = string.Empty;
        List<ApproverOverrideMaster> listApproverOverride = GetSplitData();
        List<string> approverChecker = new List<string>();
        string userCode = string.Empty;
        string duplicateApprover = string.Empty;
        string blankTransaction = string.Empty;
        bool isBlankApprover = false;
        bool isCheckerApproverTheSame = false;

        foreach (ApproverOverrideMaster objApproverOverride in listApproverOverride)
        {
            userCode = objApproverOverride.UserCode;
            if (!approverChecker.Contains(objApproverOverride.ApproverChecker))
            {
                approverChecker.Add(objApproverOverride.ApproverChecker);
            }
            else
            {
                duplicateApprover += string.Format("{0} - {1}\n", objApproverOverride.ApproverChecker, objApproverOverride.ApproverCheckerName);
            }

            if (!isBlankApprover && objApproverOverride.ApproverChecker == string.Empty)
            {
                message += "Must fill up blank Approver/Checker field.\n";
                isBlankApprover = true;
                isValid = false;
            }
            if (objApproverOverride.ApproverChecker != string.Empty && objApproverOverride.Transactions.Count <= 0)
            {
                blankTransaction += string.Format("{0} - {1}\n", objApproverOverride.ApproverChecker, objApproverOverride.ApproverCheckerName);
            }
            if (!isCheckerApproverTheSame && state == STATE.NEW && objApproverOverride.ApproverChecker == txtEmployeeId.Text.Trim())
            {
                message += "User Code must not be the same with Checker/Approver.\n";
                isCheckerApproverTheSame = true;
                isValid = false;
            }
        }
        if (blankTransaction != string.Empty)
        {
            message += "The following Checker/Approver must check atleast one Transaction:\n" + blankTransaction;
            isValid = false;
        }

        if (state == STATE.NEW)
        {
            if (duplicateApprover != string.Empty)
            {
                message += "The following Checker/Approver must be inputted only once:\n" + duplicateApprover;
                isValid = false;
            }

            string id = string.Empty;
            foreach (string strID in approverChecker)
            {
                id += string.Format("'{0}',", strID);
            }
            if (id != string.Empty)
            {
                id = id.Remove(id.Length - 1);

                DataSet ds = GetApproverOverride(userCode, id);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    id = string.Empty;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        id += string.Format("{0} - {1}\n", ds.Tables[0].Rows[0][0].ToString(), ds.Tables[0].Rows[0][1].ToString());
                    }
                    message += "The following Checker/Approver already exists:\n" + id;
                    isValid = false;
                }
            }

        }

        if (!isValid)
        {
            MessageBox.Show(message);
        }

        return isValid;
    }

    private DataSet GetApproverOverride(string userCode, string id)
    {
        DataSet ds;
        string query = string.Format(@"
SELECT DISTINCT Umt_Usercode
, Umt_UserFName + ' ' + LEFT(Umt_usermi ,1) + ' ' + Umt_UserLName
FROM T_ApprovalRouteOverrideMaster
LEFT JOIN T_UserMaster
ON Aro_UserOverride = Umt_Usercode
WHERE Aro_UserCode = '{0}'
AND Aro_UserOverride IN ({1})", userCode, id);
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            ds = dal.ExecuteDataSet(query);
            dal.CloseDB();
        }

        return ds;
    }

    private List<ApproverOverrideMaster> GetCheckedRows()
    {
        List<ApproverOverrideMaster> listApproverOverrideMaster = new List<ApproverOverrideMaster>();
        for (int i = 0; i < dgvResult.Rows.Count; i++)
        {
            CheckBox CB = (CheckBox)dgvResult.Rows[i].Cells[0].FindControl("chkBox");
            bool isChecked = CB.Checked;
            if (isChecked)
            {
                ApproverOverrideMaster approverOverride = new ApproverOverrideMaster();
                approverOverride.UserCode = dgvResult.Rows[i].Cells[1].Text;
                approverOverride.UserName = dgvResult.Rows[i].Cells[2].Text;
                approverOverride.ApproverChecker = dgvResult.Rows[i].Cells[3].Text;
                approverOverride.ApproverCheckerName = dgvResult.Rows[i].Cells[4].Text;
                approverOverride.SeqNo = GetSeqNo(approverOverride.UserCode);

                string[] transactions = dgvResult.Rows[i].Cells[5].Text.Split(',');
                List<Transactions> listTrans = new List<Transactions>();
                foreach (string trans in transactions)
                {
                    Transactions trn = new Transactions();

                    string[] splitTrans = trans.Replace("[", "").Replace("]", "").Split('-');
                    trn.TransactionCode = splitTrans[0].Trim();
                    trn.TransactionName = splitTrans[1].Trim();

                    listTrans.Add(trn);
                }
                approverOverride.Transactions = listTrans;
                listApproverOverrideMaster.Add(approverOverride);
            }
        }
        return listApproverOverrideMaster;
    }

    private List<ApproverOverrideMaster> GetSplitData()
    {
        List<ApproverOverrideMaster> listApproverOverride = new List<ApproverOverrideMaster>();
        string checkerApprover = (hfState.Value == STATE.NEW.ToString()) ? txtChecker1Id.Text : txtChecker1EditId.Text;
        string checkerApproverName = (hfState.Value == STATE.NEW.ToString()) ? txtChecker1Name.Text : txtChecker1EditName.Text;
        string transactions = (hfState.Value == STATE.NEW.ToString()) ? hTransactions.Value : hTransactionsEdit.Value;

        for (int i = 0; i < checkerApprover.Split(',').Length; i++)
        {
            ApproverOverrideMaster objApproverOverrideMaster = new ApproverOverrideMaster();
            objApproverOverrideMaster.UserCode = (hfState.Value == STATE.NEW.ToString()) ? txtEmployeeId.Text : txtUserCodeId.Text.Split(',')[i].ToString();
            objApproverOverrideMaster.UserName = (hfState.Value == STATE.NEW.ToString()) ? txtEmployeeName.Text : txtUserCodeName.Text.Split(',')[i].ToString();
            objApproverOverrideMaster.SeqNo = GetSeqNo((hfState.Value == STATE.NEW.ToString()) ? txtEmployeeId.Text : txtUserCodeId.Text.Split(',')[i].ToString());
            objApproverOverrideMaster.UserLogin = Session["userLogged"].ToString();

            objApproverOverrideMaster.ApproverChecker = checkerApprover.Split(',')[i].ToString();
            objApproverOverrideMaster.ApproverCheckerName = checkerApproverName.Split(',')[i].ToString();

            if (objApproverOverrideMaster.ApproverChecker != null && objApproverOverrideMaster.ApproverChecker != string.Empty)
            {
                string[] trans = transactions.Split(',');
                List<Transactions> trList = new List<Transactions>();
                foreach (string transaction in trans[i].Split('|'))
                {
                    if (transaction.Trim() != string.Empty)
                    {
                        Transactions tr = new Transactions();
                        tr.TransactionCode = transaction;
                        trList.Add(tr);
                    }
                }
                objApproverOverrideMaster.Transactions = trList;
            }
            listApproverOverride.Add(objApproverOverrideMaster);
        }

        return listApproverOverride;
    }

    private void Transact(STATE state)
    {

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                string message = string.Empty;
                List<ApproverOverrideMaster> listApproverOverride = GetSplitData();
                if (listApproverOverride.Count > 0)
                {
                    Dictionary<string, string> listApproverDelete = new Dictionary<string, string>();

                    foreach (ApproverOverrideMaster objApproverOverrideMaster in listApproverOverride)
                    {
                        if (state == STATE.NEW)
                        {
                            InsertApproverOverride(objApproverOverrideMaster, dal);
                            message = "Successfully Added.";
                        }
                        else if (state == STATE.EDIT)
                        {
                            ModifyApproverOverride(objApproverOverrideMaster, dal);
                            message = "Successfully Modified.";
                        }
                        if (!listApproverDelete.ContainsKey(objApproverOverrideMaster.UserCode))
                        {
                            listApproverDelete.Add(objApproverOverrideMaster.UserCode, objApproverOverrideMaster.SeqNo);
                        }
                    }

                    foreach (string userCode in listApproverDelete.Keys)
                    {
                        InsertApproverOverrideTrail(userCode, listApproverDelete[userCode], Session["userLogged"].ToString(), dal);
                    }
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show("No records to transact.");
                }
                dal.CommitTransactionSnapshot();
            }
            catch (Exception ex)
            {
                dal.RollBackTransactionSnapshot();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }

    private void InitializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFRTEMASTER");
        btnX.Enabled = userGrant.canAdd();
        btnY.Enabled = userGrant.canEdit();
        InitializeEmployee();
    }

    private void InitializeTransactions()
    {
        DataSet ds = new DataSet();

        //Perth Modified 11/29/2012
        #region Query
        string sql = @"  
SELECT * FROM T_AccountDetail
WHERE Adt_AccountType = 'WFTRN'
AND Adt_Status = 'A'";
        #endregion

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
        transactions.Rows.Clear();
        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 1; i <= ds.Tables[0].Rows.Count; )
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow rowAll = new HtmlTableRow();

                HtmlTableRow rowEdit = new HtmlTableRow();
                HtmlTableRow rowAllEdit = new HtmlTableRow();
                for (int j = 1; j <= Convert.ToInt32(Math.Ceiling((decimal)ds.Tables[0].Rows.Count / 2)) && i <= ds.Tables[0].Rows.Count; j++, i++)
                {
                    HtmlTableCell cell = new HtmlTableCell();
                    HtmlTableCell cellAll = new HtmlTableCell();

                    HtmlTableCell cellEdit = new HtmlTableCell();
                    HtmlTableCell cellAllEdit = new HtmlTableCell();

                    InitializeTransactionControl(ds, i, "", cell, cellAll);
                    row.Cells.Add(cell);
                    rowAll.Cells.Add(cellAll);

                    InitializeTransactionControl(ds, i, "Edit", cellEdit, cellAllEdit);
                    rowEdit.Cells.Add(cellEdit);
                    rowAllEdit.Cells.Add(cellAllEdit);
                }
                transactions.Rows.Add(row);
                transactionAll.Rows.Add(rowAll);

                transactionsEdit.Rows.Add(rowEdit);
                transactionAllEdit.Rows.Add(rowAllEdit);
            }
        }
    }

    private void InitializeTransactionControl(DataSet ds, int i, string name, HtmlTableCell cell, HtmlTableCell cellAll)
    {
        CheckBox chk = new CheckBox();
        chk.Attributes.Add("OnClick", "javascript:UpdateHiddenField(event)");
        chk.Attributes.Add("OnFocus", "javascript:setInnerPosition(event);");
        chk.ID = string.Format("{0}{1}", ds.Tables[0].Rows[i - 1]["Adt_AccountCode"].ToString(), name);

        Label lbl = new Label();
        lbl.Text = ds.Tables[0].Rows[i - 1]["Adt_AccountDesc"].ToString();
        lbl.Attributes.Add("OnClick", "javascript:setInnerPosition(event);");

        Label lblAll = new Label();
        lblAll.Text = string.Format("ALL {0}", ds.Tables[0].Rows[i - 1]["Adt_AccountDesc"].ToString());

        CheckBox chkAll = new CheckBox();
        chkAll.ID = string.Format("ALL{1}_{0}", ds.Tables[0].Rows[i - 1]["Adt_AccountCode"].ToString(), name);
        chkAll.Attributes.Add("OnClick", string.Format("javascript:checkAllTransaction(event, '{0}')", ds.Tables[0].Rows[i - 1]["Adt_AccountCode"].ToString()));

        cell.Controls.Add(chk);
        cell.Controls.Add(lbl);

        cellAll.Controls.Add(chkAll);
        cellAll.Controls.Add(lblAll);
    }

    private void InitializeEmployee()
    {
        DataSet ds = new DataSet();

        //Perth Modified 11/29/2012
        #region Query
        string sql = @"  
SELECT Emt_EmployeeId [ID No]
    , Emt_NickName [Nickname]
    , Emt_Lastname [Lastname]
    , Emt_Firstname [Firstname] 
	, dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	, ISNULL(Dcm_Departmentdesc, '') [Department]
FROM T_EmployeeMaster
LEFT JOIN T_DepartmentCodeMaster
	ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								ELSE ''
							END
WHERE Emt_EmployeeId = @EmployeeId

UNION

SELECT
	Umt_Usercode [ID No]
    , Umt_userfname [Nickname]
    , Umt_userlname [Lastname]
    , Umt_userfname [Firstname] 
	, '' [Costcenter]
	, '' [Department]
FROM T_UserMaster
WHERE Umt_Usercode = @EmployeeId
                    ";
        #endregion

        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString());
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
        lblNickName.Text = Resources.Resource._3RDINFO;
        if (!CommonMethods.isEmpty(ds))
        {
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
        }
    }

    private void BindGrid()
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
        if (!CommonMethods.isEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[1].Rows)
                hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
            dgvResult.DataSource = ds;
        }
        else
        {
            dgvResult.DataSource = null;
        }
        dgvResult.DataBind();
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

    private string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;

                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [User Code], [Approver/Checker], [Transaction] DESC) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(GetColumns());
        sql.Append(GetFilters());
        sql.Append(@"                              ) AS temp)
                                           SELECT 
                                              [User Code]
                                            , [User Name]
                                            , [Approver/Checker]
                                            , [Approver/Checker Name]
                                            , [Transaction]
                                             FROM TempTable
                                            WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1");
        sql.Append(@" SELECT COUNT(*)
                      FROM ( ");
        sql.Append(GetColumns());
        sql.Append(GetFilters());
        sql.Append(@"        ) as Rows");
        return sql.ToString();
    }

    private string GetColumns()
    {
        string columns = string.Empty;
        columns = @"
SELECT 
DISTINCT Aro_UserCode [User Code]
, USERCODE.Umt_UserFName + ' ' + LEFT(USERCODE.Umt_usermi ,1) + ' ' + USERCODE.Umt_UserLName [User Name]
, Aro_UserOverride [Approver/Checker]
, APPROVER.Umt_UserFName + ' ' + LEFT(APPROVER.Umt_usermi ,1) + ' ' + APPROVER.Umt_UserLName [Approver/Checker Name]
, SUBSTRING(
 (
            Select ',['+ B.Aro_TransactionID + ' - ' + Adt_AccountCode + ']'
            From T_ApprovalRouteOverrideMaster B
			LEFT JOIN T_AccountDetail 
				ON Adt_AccountType = 'WFTRN'
				AND B.Aro_TransactionID = Adt_AccountCode
            Where A.Aro_Usercode = B.Aro_Usercode
            AND A.Aro_UserOverride = B.Aro_UserOverride
            ORDER BY A.Aro_Usercode
            For XML PATH ('')
), 2, 1000) [Transaction]
";
        return columns;
    }

    private string GetFilters()
    {
        string filter = string.Empty;
        filter = string.Format(@"   FROM T_ApprovalRouteOverrideMaster A
LEFT JOIN T_UserMaster APPROVER
ON APPROVER.Umt_UserCode = Aro_UserOverride
LEFT JOIN T_UserMaster USERCODE
ON USERCODE.Umt_UserCode = Aro_UserCode
LEFT JOIN T_AccountDetail
ON Adt_AccountType = 'WFTRN'
AND Adt_AccountCode = Aro_TransactionID
WHERE 1 = 1 ");

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Aro_UserCode LIKE '{0}%' )
                                          OR ( Aro_UserOverride LIKE '{0}%' )
                                          OR ( USERCODE.Umt_UserFName + ' ' + LEFT(USERCODE.Umt_usermi ,1) + ' ' + USERCODE.Umt_UserLName LIKE '{0}%' )
                                          OR ( APPROVER.Umt_UserFName + ' ' + LEFT(APPROVER.Umt_usermi ,1) + ' ' + APPROVER.Umt_UserLName LIKE '{0}%' )
                                          OR ( USERCODE.Umt_UserFName LIKE '{0}%' )
                                          OR ( USERCODE.Umt_UserLName LIKE '{0}%' )
                                          OR ( LEFT(USERCODE.Umt_usermi ,1) LIKE '{0}%' )
                                          OR ( APPROVER.Umt_UserFName LIKE '{0}%' )
                                          OR ( APPROVER.Umt_UserLName LIKE '{0}%' )
                                          OR ( LEFT(APPROVER.Umt_usermi ,1) LIKE '{0}%' )
                                          OR ( Aro_TransactionID LIKE '{0}%' )
                                          OR ( Adt_AccountDesc LIKE '{0}%' )
                                      )";

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

    private void DeleteApproverOverride(ApproverOverrideMaster dr, DALHelper dal)
    {
        if (dr != null)
        {
            foreach (Transactions tr in dr.Transactions)
            {
                DeleteApproverOverride(dr.UserCode, dr.ApproverChecker, tr.TransactionCode, dal);
            }
        }
    }

    private void DeleteApproverOverride(string userCode, string userOverride, string transaction, DALHelper dal)
    {
        #region SQL
        string sql = string.Format(@"
DELETE FROM T_ApprovalRouteOverrideMaster
WHERE Aro_UserCode = '{0}'
", userCode);
        if (userOverride != string.Empty)
        {
            sql += string.Format("AND Aro_UserOverride = '{0}' ", userOverride);
        }

        if (transaction != string.Empty)
        {
            sql += string.Format("AND Aro_TransactionID = '{0}' ", transaction);
        }
        #endregion

        dal.ExecuteNonQuery(sql, CommandType.Text);
    }

    private void DeleteApproverOverride(string userCode, string userOverride, DALHelper dal)
    {
        DeleteApproverOverride(userCode, userOverride, string.Empty, dal);
    }

    private void DeleteApproverOverride(string userCode, DALHelper dal)
    {
        DeleteApproverOverride(userCode, string.Empty, dal);
    }

    public void InsertApproverOverride(ApproverOverrideMaster dr, DALHelper dal)
    {
        if (dr != null)
        {
            foreach (Transactions tr in dr.Transactions)
            {
                #region SQL
                string sql = string.Format(@"INSERT INTO T_ApprovalRouteOverrideMaster
(Aro_Usercode, Aro_UserOverride, Aro_TransactionID, Usr_Login, Ludatetime)
VALUES
('{0}', '{1}', '{2}', '{3}', GETDATE())", dr.UserCode
                                                , dr.ApproverChecker
                                                , tr.TransactionCode
                                                , dr.UserLogin);
                #endregion

                dal.ExecuteNonQuery(sql, CommandType.Text);
            }
        }
    }

    public void ModifyApproverOverride(ApproverOverrideMaster dr, DALHelper dal)
    {
        if (dr != null)
        {
            DeleteApproverOverride(dr.UserCode, dr.ApproverChecker, dal);
            InsertApproverOverride(dr, dal);
        }
    }

    private void InsertApproverOverrideTrail(string userCode, string seqNo, string userLogin, DALHelper dal)
    {
        #region SQL
        string sql = string.Format(@"
IF EXISTS(SELECT * FROM T_ApprovalRouteOverrideMaster WHERE Aro_UserCode = '{0}')
BEGIN 
    INSERT INTO T_ApprovalRouteOverrideTrail
    (Aro_Usercode, Aro_UserOverride, Aro_TransactionID, Aro_SeqNo, Usr_Login, Ludatetime)
    SELECT Aro_Usercode, Aro_UserOverride, Aro_TransactionID, '{1}', Usr_Login, Ludatetime 
    FROM T_ApprovalRouteOverrideMaster
    WHERE Aro_UserCode = '{0}'
END
ELSE BEGIN
    INSERT INTO T_ApprovalRouteOverrideTrail
    SELECT '{0}', '', '', '{1}', '{2}', GETDATE()
END", userCode, seqNo, userLogin);
        #endregion

        dal.ExecuteNonQuery(sql, CommandType.Text);
    }

    private string GetSeqNo(string userCode)
    {
        string seqNo = string.Empty;
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            seqNo = GetSeqNo(userCode, dal);
            dal.CloseDB();
        }
        return seqNo;
    }

    private string GetSeqNo(string userCode, DALHelper dal)
    {
        string query = string.Format(@"
IF EXISTS(SELECT * FROM T_ApprovalRouteOverrideTrail
			WHERE Aro_Usercode = '{0}')
BEGIN
SELECT TOP 1 RIGHT('000' + CAST(CAST(Aro_SeqNo AS INT) + 1 AS VARCHAR(3)), 3)
FROM T_ApprovalRouteOverrideTrail
WHERE Aro_Usercode = '{0}'
GROUP BY Aro_SeqNo
ORDER BY Aro_SeqNo DESC
END
ELSE BEGIN
	SELECT '001'
END", userCode);
        string seqNo = string.Empty;
        DataSet ds = dal.ExecuteDataSet(query);
        if (!CommonMethods.isEmpty(ds))
        {
            seqNo = ds.Tables[0].Rows[0][0].ToString();
        }
        return seqNo;
    }

    private void SetState(STATE state)
    {
        if (state == STATE.CANCEL)
        {
            VIEWER.ActiveViewIndex = 0;
            btnX.Text = "NEW";
            btnY.Text = "EDIT";
            btnZ.Visible = true;
        }
        else if (state == STATE.NEW || state == STATE.EDIT)
        {
            VIEWER.ActiveViewIndex = state == STATE.NEW ? 1 : 2;
            btnX.Text = state == STATE.NEW ? "CREATE" : "SAVE";
            btnY.Text = "CANCEL";
            btnZ.Visible = false;
        }
        hfState.Value = state.ToString();
    } 
    #endregion
}

public class ApproverOverrideMaster
{
    public string UserCode;
    public string UserName;
    public string ApproverChecker;
    public string ApproverCheckerName;
    public List<Transactions> Transactions;
    public string SeqNo;
    public string UserLogin;
}

public class Transactions
{
    public string TransactionCode;
    public string TransactionName;
}

public enum STATE
{ 
    NEW = 0,
    EDIT = 1,
    DELETE = 2,
    CANCEL = 3
}