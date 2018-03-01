using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;
using CommonLibrary;
using System.Configuration;

public partial class Transactions_Overtime_pgeOvertimeBatchUploading : System.Web.UI.Page
{
    #region Variables
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();
    private static DataTable dtPastedRecord = new DataTable();
    private static DataTable dtOTType = new DataTable();
    private static string filler1ColName = string.Empty;
    private static string filler2ColName = string.Empty;
    private static string filler3ColName  = string.Empty;
    private static DateTime quincenaDate;
    decimal otHours = 0;
    string otEndTime = string.Empty;
    string otStartTime = string.Empty;
    bool isDataGridEmpty = true;
    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBOTUPLD"))
        {
            Response.Redirect(@"../../index.aspx?pr=ur");
        }
        InitializeControls();
        if (!Page.IsPostBack)
        {
            filler1ColName = string.Empty;
            filler2ColName = string.Empty;
            filler3ColName = string.Empty;
            dtPastedRecord = new DataTable();
            dtOTType = GetOTType();
            quincenaDate = CommonMethods.getQuincenaDate('F', "END");
            DataTable dgvUploadTable = new DataTable();
            DataTable dtCols = getOvertimeColumns();
            DataTable dt = showOptionalFields(dtCols);

            foreach (DataColumn column in dt.Columns)
            {
                dgvUploadTable.Columns.Add(column.ColumnName);
            }
        }
    }
    protected void btnPaste_Click(object sender, EventArgs e)
    {
        System.Windows.Forms.IDataObject idat = null;
        Exception threadEx = null;
        String text = "";
        System.Threading.Thread staThread = new System.Threading.Thread(
            delegate()
            {
                try
                {
                    idat = System.Windows.Forms.Clipboard.GetDataObject();
                    text = idat.GetData(System.Windows.Forms.DataFormats.Text).ToString();
                }

                catch (Exception ex)
                {
                    threadEx = ex;
                }
            });
        staThread.SetApartmentState(System.Threading.ApartmentState.STA);
        staThread.Start();
        staThread.Join();

        System.Globalization.TextInfo txtInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        DataTable dgvUploadTable = new DataTable();
        string s = hfText.Value == null || hfText.Value.ToString().Trim() == string.Empty ? text : hfText.Value;
        string[] lines = s.Split(new char[] { '\n' });
        DataTable dt = getOvertimeColumns();

        if (!s.Trim().Equals(string.Empty))
        {
            tblCols.Attributes.Add("style", "display:none;");
            btnExport.Visible = true;
            isDataGridEmpty = false;
        }
        else
        {
            tblCols.Attributes.Add("style", "display:relative;");
            btnExport.Visible = false;
            isDataGridEmpty = true;
        }
        if (dt == null)
            dt = getOvertimeColumns();

        foreach (DataColumn column in dt.Columns)
        {
            dgvUploadTable.Columns.Add(column.ColumnName);
        }
        foreach (string line in lines)
        {
            if (line.Length > 0)
            {
                DataRow dr = dgvUploadTable.NewRow();
                string[] cells = line.Split(new char[] { '\t' });
                for (int i = 0; i < cells.GetLength(0) && i < 11; i++)
                {
                    if (cells[i] != "")
                    {
                        if (cells[i] != "\r")
                            dr[i] = cells[i];
                        else
                            dr[i] = "";
                    }

                }
                dgvUploadTable.Rows.Add(dr);
            }
        }
        ViewState["pastedRecord"] = dgvUploadTable;
        dgvOTUpload.DataSource = ViewState["pastedRecord"];
        if (filler1ColName == null || filler1ColName.Equals(string.Empty))
            dgvOTUpload.Columns[9].Visible = false;
        else
            dgvOTUpload.Columns[9].HeaderText = txtInfo.ToTitleCase(filler1ColName);

        if (filler2ColName == null || filler2ColName.Equals(string.Empty))
            dgvOTUpload.Columns[10].Visible = false;
        else
            dgvOTUpload.Columns[10].HeaderText = txtInfo.ToTitleCase(filler2ColName);

        if (filler3ColName == null || filler3ColName.Equals(string.Empty))
            dgvOTUpload.Columns[11].Visible = false;
        else
            dgvOTUpload.Columns[11].HeaderText = txtInfo.ToTitleCase(filler3ColName);
        dgvOTUpload.DataBind();

        UpdateRowCount(dgvOTUpload.Rows.Count);
    }

    protected void dgvOTUpload_RowEditing(object sender, GridViewEditEventArgs e)
    {
        btnUploadEndorse.Enabled = false;
        btnPaste.Enabled = false;
        btnClear.Enabled = false;
        btnExport.Visible = true;
        btnExport.Enabled = false;
        btnAdd.Enabled = false;
        dgvOTUpload.EditIndex = e.NewEditIndex;
        dgvOTUpload.DataSource = ViewState["pastedRecord"];
        dgvOTUpload.DataBind();

        foreach (GridViewRow row in dgvOTUpload.Rows)
        {
            if (row.RowIndex != e.NewEditIndex)
            {
                row.Enabled = false;
            }
        }
    }
    protected void dgvOTUpload_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        btnUploadEndorse.Enabled = true;
        btnPaste.Enabled = true;
        btnClear.Enabled = true;
        btnExport.Visible = true;
        btnExport.Enabled = true;
        btnAdd.Enabled = true;
        dgvOTUpload.EditIndex = -1;
        dgvOTUpload.DataSource = ViewState["pastedRecord"];
        dgvOTUpload.DataBind();
    }

    protected void dgvOTUpload_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        btnUploadEndorse.Enabled = true;
        btnPaste.Enabled = true;
        btnClear.Enabled = true;
        btnExport.Visible = true;
        btnExport.Enabled = true;
        btnAdd.Enabled = true;
        GridViewRow row = dgvOTUpload.Rows[e.RowIndex];
        TextBox employeeID = (TextBox)row.FindControl("txtEmployeeID");
        TextBox dateOvertime = (TextBox)row.FindControl("txtDOT");
        TextBox otType = (TextBox)row.FindControl("txtOTType");
        TextBox startTime = (TextBox)row.FindControl("txtStartTime");
        TextBox endTime = (TextBox)row.FindControl("txtEndTime");
        TextBox hours = (TextBox)row.FindControl("txtHours");
        TextBox reason = (TextBox)row.FindControl("txtReason");
        TextBox filler1 = (TextBox)row.FindControl("txtFiller1");
        TextBox filler2 = (TextBox)row.FindControl("txtFiller2");
        TextBox filler3 = (TextBox)row.FindControl("txtFiller3");

        if (dtPastedRecord != null && dtPastedRecord.Rows.Count > 0)
        {
            dtPastedRecord.Rows[e.RowIndex]["Employee ID"] = employeeID.Text;
            dtPastedRecord.Rows[e.RowIndex]["Date of Overtime"] = dateOvertime.Text;
            dtPastedRecord.Rows[e.RowIndex]["Overtime Type"] = otType.Text;
            dtPastedRecord.Rows[e.RowIndex]["Start Time"] = startTime.Text;
            dtPastedRecord.Rows[e.RowIndex]["End Time"] = endTime.Text;
            dtPastedRecord.Rows[e.RowIndex]["Hours"] = hours.Text;
            dtPastedRecord.Rows[e.RowIndex]["Reason"] = reason.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler1"] = filler1.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler2"] = filler2.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler3"] = filler3.Text;

            dtPastedRecord.AcceptChanges();
        }

        dgvOTUpload.EditIndex = -1; 
        ViewState["pastedRecord"] = dtPastedRecord;
        dgvOTUpload.DataSource = dtPastedRecord;
        dgvOTUpload.DataBind();
    }

    protected void dgvOTUpload_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        btnUploadEndorse.Enabled = true;
        btnPaste.Enabled = true;
        btnClear.Enabled = true;
        btnAdd.Enabled = true;
        if (dtPastedRecord != null && dtPastedRecord.Rows.Count > 0)
        {
            btnExport.Visible = true;
            btnExport.Enabled = true;
            dtPastedRecord.Rows[e.RowIndex].Delete();
            dtPastedRecord.AcceptChanges(); 
            ViewState["pastedRecord"] = dtPastedRecord;
            dgvOTUpload.DataSource = dtPastedRecord;
            dgvOTUpload.DataBind();

            if (((System.Data.DataTable)(dgvOTUpload.DataSource)).Rows.Count == 0)
            {
                tblCols.Attributes.Add("style", "display:relative;");
                btnExport.Visible = false;
                hfText.Value = string.Empty;
                isDataGridEmpty = true;
            }
            else
                isDataGridEmpty = false;
        }
        else
        {
            tblCols.Attributes.Add("style", "display:none;");
            btnExport.Visible = false;
            btnExport.Enabled = false;
        }

        UpdateRowCount(dgvOTUpload.Rows.Count);
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        dgvOTUpload.DataSource = null;
        dgvOTUpload.DataBind();
        ViewState["pastedRecord"] = null;
        hfText.Value = string.Empty;
        tblCols.Attributes.Add("style", "display:relative;");
        btnExport.Visible = false;
        isDataGridEmpty = true;
        UpdateRowCount(dgvOTUpload.Rows.Count);
    }
    protected void btnUploadEndorse_Click(object sender, EventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        if (dtPastedRecord != null && dtPastedRecord.Rows.Count > 0)
        {
            tblCols.Attributes.Add("style", "display:none;");
            btnExport.Visible = true;
            isDataGridEmpty = false;
            transact();
        }
        else
        {
            tblCols.Attributes.Add("style", "display:relative;");
            btnExport.Visible = false;
            isDataGridEmpty = true;
            MessageBox.Show("There are no rows to be uploaded.");
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        GridView tempGridView = new GridView();
        tempGridView.DataSource = dtPastedRecord;
        tempGridView.DataBind();
        if (dtPastedRecord != null && dtPastedRecord.Rows.Count > 0)
        {
            Control[] ctrl = new Control[2];
            ctrl[0] = CommonLookUp.GetHeaderPanelOption(dtPastedRecord.Columns.Count, dtPastedRecord.Rows.Count, "BATCH OVERTIME UPLOADING", "");
            ctrl[1] = tempGridView;
            ExportExcelHelper.ExportControl2(ctrl, "Batch Overtime Uploading");
        }
        else
        {
            MessageBox.Show("Error in exporting grid.");
            btnExport.Visible = true;
            btnExport.Enabled = true;
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        System.Globalization.TextInfo txtInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        DataTable dgvUploadTable = new DataTable();
        DataTable dt = getOvertimeColumns();

        tblCols.Attributes.Add("style", "display:none;");
        btnExport.Visible = true;

        if (dtPastedRecord == null || dtPastedRecord.Rows.Count == 0 && isDataGridEmpty)
        {
            foreach (DataColumn column in dt.Columns)
            {
                dgvUploadTable.Columns.Add(column.ColumnName);
            }
            dtPastedRecord = dt;

            dtPastedRecord.AcceptChanges();
            dgvOTUpload.DataSource = dtPastedRecord;
            if (filler1ColName == null || filler1ColName.Equals(string.Empty))
                dgvOTUpload.Columns[9].Visible = false;
            else
                dgvOTUpload.Columns[9].HeaderText = filler1ColName;

            if (filler2ColName == null || filler2ColName.Equals(string.Empty))
                dgvOTUpload.Columns[10].Visible = false;
            else
                dgvOTUpload.Columns[10].HeaderText = filler2ColName;

            if (filler3ColName == null || filler3ColName.Equals(string.Empty))
                dgvOTUpload.Columns[11].Visible = false;
            else
                dgvOTUpload.Columns[11].HeaderText = filler3ColName;

            isDataGridEmpty = true;
        }
        else
        {
            dtPastedRecord.Rows.Add();
            dtPastedRecord.AcceptChanges();
            dgvOTUpload.DataSource = dtPastedRecord;
            isDataGridEmpty = false;
        }
        ViewState["pastedRecord"] = dtPastedRecord;
        dgvOTUpload.DataBind();

        UpdateRowCount(dgvOTUpload.Rows.Count);
    }
    #endregion

    #region Function
    private void InitializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "OVERTIME", "WFBOTUPLD");
        btnUploadEndorse.Enabled = userGrant.canPrint();
        btnExport.Visible = false;
    }

    private DataTable getOvertimeColumns()
    {
        string query = @"SELECT TOP 1
                    '' [Employee ID]
                    , '' [Date of Overtime]
                    , '' [Overtime Type]
                    , '' [Start Time]
                    , '' [End Time]
                    , '' [Hours]
                    , '' [Reason]
                    , '' [Filler1]
                    , '' [Filler2]
                    , '' [Filler3]
                    , '' [Remarks]";

        DataTable dtResult = new DataTable();

        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }
        return dtResult;
    }
    private DataTable InsertDataSuccess(int rowTrav, DataTable dtPastedRecord)
    {
        DataTable dtResult;
        DataRow[] drArrRows;
        string errMsg = string.Empty;        

        ParameterInfo[] param = new ParameterInfo[15];
        param[0] = new ParameterInfo("@ControlNo", string.Empty);
        param[1] = new ParameterInfo("@EmployeeId", dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().Trim());
        param[2] = new ParameterInfo("@RecordDate", Convert.ToDateTime(dtPastedRecord.Rows[rowTrav]["Date of Overtime"].ToString().Trim()));
        param[3] = new ParameterInfo("@OTType", dtPastedRecord.Rows[rowTrav]["Overtime Type"].ToString().Trim());
        param[4] = new ParameterInfo("@StartTime", otStartTime);
        param[5] = new ParameterInfo("@EndTime", otEndTime);
        param[6] = new ParameterInfo("@OTHours", otHours);
        param[7] = new ParameterInfo("@Reason", dtPastedRecord.Rows[rowTrav]["Reason"].ToString().Trim());
        param[8] = new ParameterInfo("@CreatedBy", Session["userLogged"].ToString().Trim());
        param[9] = new ParameterInfo("@Filler1", filler1ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler1"].ToString().Trim() : string.Empty);
        param[10] = new ParameterInfo("@Filler2", filler2ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler2"].ToString().Trim() : string.Empty);
        param[11] = new ParameterInfo("@Filler3", filler3ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler3"].ToString().Trim() : string.Empty);
        param[12] = new ParameterInfo("@DBPrefix", HttpContext.Current.Session["dbPrefix"].ToString().Trim());
        param[13] = new ParameterInfo("@MenuCode", "WFBOTUPLD");
        param[14] = new ParameterInfo("@NotifyEmail", Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
        string query = @"EXEC CreateOvertimeRecord
                            @ControlNo
									, @EmployeeId
									, @RecordDate
									, @OTType
									, @StartTime
									, @EndTime
									, @OTHours
									, @Reason
									, ''
									, @CreatedBy
									, @DBPrefix
									, @MenuCode
									, @NotifyEmail
									, ''
									, ''
									, @Filler1
									, @Filler2
									, @Filler3
									, NULL";
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query, CommandType.Text, param).Tables[0];
            dal.CloseDB();
        }
        drArrRows = dtResult.Select("Result = 1");
        if (drArrRows.Length > 0)
        {
            SystemMenuLogBL.InsertAddLog("WFBOTUPLD", true, dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
            dtPastedRecord.Rows[rowTrav]["Remarks"] = dtResult.Rows[0]["Message"].ToString();
        }
        else
        {
            SystemMenuLogBL.InsertAddLog("WFBOTUPLD", false, dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
            errMsg = string.Empty;
            drArrRows = dtResult.Select("Result <> 1");
            foreach (DataRow drRow in drArrRows)
            {
                errMsg += "[" + drRow[4].ToString() + "] ";
            }
            dtPastedRecord.Rows[rowTrav]["Remarks"] = errMsg;
        }
               
        return dtPastedRecord;
    }
    private void transact()
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        DataTable dtResult = new DataTable();
        for (int rowTrav = 0; rowTrav < dtPastedRecord.Rows.Count; rowTrav++)
        {
            if (dtPastedRecord.Rows[rowTrav]["Remarks"].ToString().Trim() == ""
                || dtPastedRecord.Rows[rowTrav]["Remarks"].ToString().Trim() != "Successful")
            {
                if (checkFormat(rowTrav, dtPastedRecord))
                {
                    try
                    {
                        dtResult = InsertDataSuccess(rowTrav, dtPastedRecord);
                        dgvOTUpload.DataBind();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    dtResult = dtPastedRecord;
                }
            }
        }

        dgvOTUpload.DataSource = dtResult;
        dgvOTUpload.DataBind();
    }
    private bool checkFormat(int rowNumber, DataTable dtPastedRecord)
    {
        bool isValid = false;
        string columnName = string.Empty;
        string strRemarks = string.Empty;
        DataTable dtEmployee = new DataTable(); 
        DataRow[] drArrResult;

        for (int cellNumber = 0; cellNumber < dtPastedRecord.Columns.Count - 1; cellNumber++)
        {
            columnName = dtPastedRecord.Columns[cellNumber].ColumnName.ToString();
            if (columnName.Equals("Employee ID"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Employee ID"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Employee ID must not be blank]";
            }
            if (columnName.Equals("Date of Overtime"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Date of Overtime"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Overtime Date must not be blank]";
                else if (!dtPastedRecord.Rows[rowNumber]["Date of Overtime"].ToString().Trim().Equals(string.Empty))
                {
                    try
                    {
                        DateTime dateOT = Convert.ToDateTime(dtPastedRecord.Rows[rowNumber]["Date of Overtime"].ToString().Trim());

                        if (dateOT > quincenaDate)
                            strRemarks += "[OT Date is greater than the maximum date]";
                    }
                    catch
                    {
                        strRemarks += "[Invalid Overtime Date (mm/dd/yyyy)]";
                    }
                }
            }
            if (columnName.Equals("Overtime Type"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Overtime Type"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Overtime Type must not be blank]";
                else if (!dtPastedRecord.Rows[rowNumber]["Overtime Type"].ToString().Trim().Equals(string.Empty))
                {
                    drArrResult = dtOTType.Select("Pmx_ParameterValue = '" +dtPastedRecord.Rows[rowNumber]["Overtime Type"].ToString().Trim()+ "'");
                    if (drArrResult.Length == 0)
                        strRemarks += "[Overtime Type does not exist]";
                }
            }

            if (columnName.Equals("Start Time") || columnName.Equals("End Time"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length != 4)
                    strRemarks += "[Invalid Format (HHMM)]";
                else if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4)
                    otStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
            }
            if (columnName.Equals("End Time"))
            {
                if (dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length != 4)
                    strRemarks += "[Invalid Format (HHMM)]";
                else if (dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length == 4)
                    otEndTime = dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Trim();
            }

            if (columnName.Equals("Hours"))
            {
                try
                {
                    if (dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                    {
                        decimal hour = Convert.ToDecimal(dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim());

                        if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4
                        && dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length == 4
                        && dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                        {
                            try
                            {
                                otHours = -1;
                                otStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
                                otEndTime = dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Trim();
                            }
                            catch
                            {
                                strRemarks += "[Invalid Hours (Numeric)]";
                            }
                        }
                        else if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4
                            && dtPastedRecord.Rows[rowNumber]["End Time"].ToString() == string.Empty
                            && dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                        {
                            otEndTime = string.Empty;
                            otStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
                            otHours = Convert.ToDecimal(dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim());
                        }
                        else if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() == string.Empty
                            && dtPastedRecord.Rows[rowNumber]["End Time"].ToString() == string.Empty
                            && dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                        {
                            otStartTime = string.Empty;
                            otEndTime = string.Empty;
                            otHours = Convert.ToDecimal(dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim());
                        }
                    }
                    else
                        otHours = -1;
                }
                catch
                {
                    strRemarks += "[Invalid Hours (Numeric)]";
                }
            }
            if (filler1ColName != null && !filler1ColName.Equals(string.Empty))
            {
                if (columnName.Equals("Filler1"))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Filler1"].ToString().Equals(string.Empty))
                        strRemarks += "[" + filler1ColName + " is empty]";
                }
            }
            if (filler2ColName != null && !filler2ColName.Equals(string.Empty))
            {
                if (columnName.Equals("Filler2"))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Filler2"].ToString().Equals(string.Empty))
                        strRemarks += "[" + filler2ColName + " is empty]";
                }
            }
            if (filler3ColName != null && !filler3ColName.Equals(string.Empty))
            {
                if (columnName.Equals("Filler3"))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Filler3"].ToString().Equals(string.Empty))
                        strRemarks += "[" + filler3ColName + " is empty]";
                }
            }
            if (columnName.Equals("Reason"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Reason"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Reason must not be blank]";
                else if (!dtPastedRecord.Rows[rowNumber]["Reason"].ToString().Trim().Equals(string.Empty))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Reason"].ToString().Trim().Length > 200)
                        strRemarks += "[Reason exceeds the maximum length of characters (200 chars)]";
                }
            }
        }

        dtPastedRecord.Rows[rowNumber]["Remarks"] = strRemarks;
        if (strRemarks.Trim() == "")
            isValid = true;
        return isValid;
    }
    private DataTable GetOTType()
    {
        string query = @"SELECT Pmx_ParameterValue
                                FROM T_ParameterMasterExt
                                WHERE Pmx_ParameterID = 'OTTYPE'
                                AND Pmx_Status = 'A'";

        DataTable dtResult = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }

        return dtResult;
    }
    private DataTable showOptionalFields(DataTable dtCols)
    {
        System.Globalization.TextInfo txtInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        DataSet ds = new DataSet();
        string sql = @"DECLARE @AUTOROUTE as bit = (SELECT Pcm_ProcessFlag
                                                      FROM T_ProcessControlMaster
                                                     WHERE Pcm_SystemID = 'OVERTIME'
                                                       AND Pcm_ProcessID = 'AUTOROUTE'
                                                       AND Pcm_Status = 'A' )
                       SELECT IIF(Cfl_ColName = 'Eot_Filler01' AND @AUTOROUTE = 1, '', Cfl_ColName) Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Eot_Filler%'
                          AND Cfl_Status = 'A'
                          AND 1 = IIF(Cfl_ColName <> 'Eot_Filler01', 1, IIF(@AUTOROUTE = 1, 0, 1)) ";
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

        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("EOT_FILLER01"))
                {
                    filler1ColName = txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler1"].ColumnName = filler1ColName;
                    thFiller1.InnerText = filler1ColName;
                    thFiller1.Visible = true;
                    tdFiller1.Visible = true;
                }
                else if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("EOT_FILLER02"))
                {
                    filler2ColName = txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler2"].ColumnName = filler2ColName;
                    thFiller2.InnerText = filler2ColName;
                    thFiller2.Visible = true;
                    tdFiller2.Visible = true;
                }
                else if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("EOT_FILLER03"))
                {
                    filler3ColName = txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler3"].ColumnName = filler3ColName;
                    thFiller3.InnerText = filler3ColName;
                    thFiller3.Visible = true;
                    tdFiller3.Visible = true;
                }
                else
                {
                    dtCols.Columns.Remove(ds.Tables[0].Rows[i]["Cfl_ColName"].ToString());
                    if (filler1ColName == null || filler1ColName.Equals(string.Empty))
                    {
                        thFiller1.Visible = false;
                        tdFiller1.Visible = false;
                    }
                    else
                    {
                        thFiller1.Visible = true;
                        tdFiller1.Visible = true;
                    }

                    if (filler2ColName == null || filler2ColName.Equals(string.Empty))
                    {
                        thFiller2.Visible = false;
                        tdFiller2.Visible = false;
                    }
                    else
                    {
                        thFiller2.Visible = true;
                        tdFiller2.Visible = true;
                    }
                    if (filler3ColName == null || filler3ColName.Equals(string.Empty))
                    {
                        thFiller3.Visible = false;
                        tdFiller3.Visible = false;
                    }
                    else
                    {
                        thFiller3.Visible = true;
                        tdFiller3.Visible = true;
                    }
                }
            }
        }
        else
        {
            dtCols.Columns.Remove("Filler1");
            dtCols.Columns.Remove("Filler2");
            dtCols.Columns.Remove("Filler3"); 
            thFiller1.Visible = false;
            thFiller2.Visible = false;
            thFiller3.Visible = false;
            tdFiller1.Visible = false;
            tdFiller2.Visible = false;
            tdFiller3.Visible = false;
        }
        return dtCols;
    }

    private void UpdateRowCount(int rows)
    {
        lblRowCount.Text = string.Format("{0} Row(s)", rows);
    }
    #endregion
}