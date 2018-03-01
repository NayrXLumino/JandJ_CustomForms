using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Payroll.DAL;
using System.Configuration;
using CommonLibrary;

public partial class Transactions_Leave_pgeLeaveBatchUploading : System.Web.UI.Page
{
    #region Variables
    MenuGrant MGBL = new MenuGrant();
    CommonMethods methods = new CommonMethods();
    private static DataTable dtDayUnit = new DataTable();
    private static DataTable dtLeaveType = new DataTable();
    private bool bDAYSEL = false;
    private static string filler1ColName = string.Empty;
    private static string filler2ColName = string.Empty;
    private static string filler3ColName = string.Empty;
    decimal leaveHours = 0;
    string leaveEndTime = string.Empty;
    string leaveStartTime = string.Empty;
    bool isDataGridEmpty = true;
    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect(@"../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFBLEAVEUPLD"))
        {
            Response.Redirect(@"../../index.aspx?pr=ur");
        }

        InitializeControls();

        if (!Page.IsPostBack)
        {
            filler1ColName = string.Empty;
            filler2ColName = string.Empty;
            filler3ColName = string.Empty;
            dtDayUnit = GetDayUnit();
            dtLeaveType = GetLeaveType();
            bDAYSEL = methods.GetProcessControlFlag("LEAVE", "DAYSEL");
            DataTable dgvUploadTable = new DataTable();
            DataTable dtCols = getLeaveColumns();
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
            delegate ()
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
        DataTable dt = getLeaveColumns();

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

        foreach (DataColumn column in dt.Columns)
        {
            dgvUploadTable.Columns.Add(column.ColumnName);
        }

        int ctr = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != "")
            {
                ctr++;
            }
        }

        if (ctr != 0)
        {
            foreach (string line in lines)
            {
                if (line != "")
                {
                    if (line.Length > 0)
                    {
                        DataRow dr = dgvUploadTable.NewRow();
                        string[] cells = line.Split(new char[] { '\t' });
                        for (int i = 0; i < cells.GetLength(0) && i < 12; i++)
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
            }
        }
        ViewState["pastedRecord"] = dgvUploadTable;
        dgvLeaveUpload.DataSource = ViewState["pastedRecord"];

        if (filler1ColName == null || filler1ColName.Equals(string.Empty))
            dgvLeaveUpload.Columns[10].Visible = false;
        else
            dgvLeaveUpload.Columns[10].HeaderText = txtInfo.ToTitleCase(filler1ColName);

        if (filler2ColName == null || filler2ColName.Equals(string.Empty))
            dgvLeaveUpload.Columns[11].Visible = false;
        else
            dgvLeaveUpload.Columns[11].HeaderText = txtInfo.ToTitleCase(filler2ColName) + "*"; ;

        if (filler3ColName == null || filler3ColName.Equals(string.Empty))
            dgvLeaveUpload.Columns[12].Visible = false;
        else
            dgvLeaveUpload.Columns[12].HeaderText = txtInfo.ToTitleCase(filler3ColName) + "*"; ;

        if (methods.GetProcessControlFlag("LEAVE", "DAYSEL"))
            dgvLeaveUpload.Columns[9].HeaderText = "Day Unit";
        else
            dgvLeaveUpload.Columns[9].Visible = false;
        dgvLeaveUpload.DataBind();
        UpdateRowCount(dgvLeaveUpload.Rows.Count);
    }

    protected void dgvLeaveUpload_RowEditing(object sender, GridViewEditEventArgs e)
    {
        btnUploadEndorse.Enabled = false;
        btnPaste.Enabled = false;
        btnClear.Enabled = false;
        btnExport.Visible = true;
        btnExport.Enabled = false;
        btnAdd.Enabled = false;
        dgvLeaveUpload.EditIndex = e.NewEditIndex;
        dgvLeaveUpload.DataSource = ViewState["pastedRecord"];
        dgvLeaveUpload.DataBind();

        foreach (GridViewRow row in dgvLeaveUpload.Rows)
        {
            if (row.RowIndex != e.NewEditIndex)
            {
                row.Enabled = false;
            }
        }

    }
    protected void dgvLeaveUpload_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        btnUploadEndorse.Enabled = true;
        btnPaste.Enabled = true;
        btnClear.Enabled = true;
        btnExport.Visible = true;
        btnExport.Enabled = true;
        btnAdd.Enabled = true;
        dgvLeaveUpload.EditIndex = -1;
        dgvLeaveUpload.DataSource = ViewState["pastedRecord"];
        dgvLeaveUpload.DataBind();
    }

    protected void dgvLeaveUpload_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataTable dtPastedRecord = (DataTable)ViewState["pastedRecord"];
        btnUploadEndorse.Enabled = true;
        btnPaste.Enabled = true;
        btnClear.Enabled = true;
        btnExport.Visible = true;
        btnExport.Enabled = true;
        btnAdd.Enabled = true;
        GridViewRow row = dgvLeaveUpload.Rows[e.RowIndex];
        TextBox employeeID = (TextBox)row.FindControl("txtEmployeeID");
        TextBox dateLeave = (TextBox)row.FindControl("txtDOL");
        TextBox otType = (TextBox)row.FindControl("txtLVType");
        TextBox startTime = (TextBox)row.FindControl("txtStartTime");
        TextBox endTime = (TextBox)row.FindControl("txtEndTime");
        TextBox hours = (TextBox)row.FindControl("txtHours");
        TextBox reason = (TextBox)row.FindControl("txtReason");
        TextBox dayUnit = (TextBox)row.FindControl("txtDayUnit");
        TextBox filler1 = (TextBox)row.FindControl("txtFiller1");
        TextBox Filler2 = (TextBox)row.FindControl("txtFiller2");
        TextBox Filler3 = (TextBox)row.FindControl("txtFiller3");

        if (dtPastedRecord != null && dtPastedRecord.Rows.Count > 0)
        {
            dtPastedRecord.Rows[e.RowIndex]["Employee ID"] = employeeID.Text;
            dtPastedRecord.Rows[e.RowIndex]["Date of Leave"] = dateLeave.Text;
            dtPastedRecord.Rows[e.RowIndex]["Leave Type"] = otType.Text;
            dtPastedRecord.Rows[e.RowIndex]["Start Time"] = startTime.Text;
            dtPastedRecord.Rows[e.RowIndex]["End Time"] = endTime.Text;
            dtPastedRecord.Rows[e.RowIndex]["Hours"] = hours.Text;
            dtPastedRecord.Rows[e.RowIndex]["Reason"] = reason.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler1"] = filler1.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler2"] = Filler2.Text;
            dtPastedRecord.Rows[e.RowIndex]["Filler3"] = Filler3.Text;
            dtPastedRecord.Rows[e.RowIndex]["Day Unit"] = dayUnit.Text;

            dtPastedRecord.AcceptChanges();
        }
        dgvLeaveUpload.EditIndex = -1;
        ViewState["pastedRecord"] = dtPastedRecord;
        dgvLeaveUpload.DataSource = dtPastedRecord;
        dgvLeaveUpload.DataBind();
    }

    protected void dgvLeaveUpload_RowDeleting(object sender, GridViewDeleteEventArgs e)
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
            dgvLeaveUpload.DataSource = dtPastedRecord;
            dgvLeaveUpload.DataBind();

            if (((System.Data.DataTable)(dgvLeaveUpload.DataSource)).Rows.Count == 0)
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
        UpdateRowCount(dgvLeaveUpload.Rows.Count);
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        dgvLeaveUpload.DataSource = null;
        dgvLeaveUpload.DataBind();
        ViewState["pastedRecord"] = null;
        tblCols.Attributes.Add("style", "display:relative;");
        btnExport.Visible = false;
        isDataGridEmpty = true;
        UpdateRowCount(dgvLeaveUpload.Rows.Count);
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
            ctrl[0] = CommonLookUp.GetHeaderPanelOption(dtPastedRecord.Columns.Count, dtPastedRecord.Rows.Count, "BATCH LEAVE UPLOADING", "");
            ctrl[1] = tempGridView;
            ExportExcelHelper.ExportControl2(ctrl, "Batch Leave Uploading");
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
        DataTable dt = getLeaveColumns();

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
            dgvLeaveUpload.DataSource = dtPastedRecord;
            if (filler1ColName == null || filler1ColName.Equals(string.Empty))
                dgvLeaveUpload.Columns[10].Visible = false;
            else
                dgvLeaveUpload.Columns[10].HeaderText = txtInfo.ToTitleCase(filler1ColName);

            if (filler2ColName == null || filler2ColName.Equals(string.Empty))
                dgvLeaveUpload.Columns[11].Visible = false;
            else
                dgvLeaveUpload.Columns[11].HeaderText = txtInfo.ToTitleCase(filler2ColName) + "*"; ;

            if (filler3ColName == null || filler3ColName.Equals(string.Empty))
                dgvLeaveUpload.Columns[12].Visible = false;
            else
                dgvLeaveUpload.Columns[12].HeaderText = txtInfo.ToTitleCase(filler3ColName) + "*"; ;

            if (methods.GetProcessControlFlag("LEAVE", "DAYSEL"))
                dgvLeaveUpload.Columns[9].HeaderText = "Day Unit";
            else
                dgvLeaveUpload.Columns[9].Visible = false;

            isDataGridEmpty = true;
        }
        else
        {
            dtPastedRecord.Rows.Add();
            dtPastedRecord.AcceptChanges();           
            dgvLeaveUpload.DataSource = dtPastedRecord;
            isDataGridEmpty = false;
        }
        ViewState["pastedRecord"] = dtPastedRecord;
        dgvLeaveUpload.DataBind();
        UpdateRowCount(dgvLeaveUpload.Rows.Count);
    }
    #endregion

    #region Function
    private void InitializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "LEAVE", "WFBLEAVEUPLD");
        btnUploadEndorse.Enabled = userGrant.canPrint();
        btnExport.Visible = false;       
    }

    private DataTable getLeaveColumns()
    {
        string query = @"SELECT TOP 1
                    '' [Employee ID]
                    , '' [Date of Leave]
                    , '' [Leave Type]
                    , '' [Start Time]
                    , '' [End Time]
                    , '' [Hours]
                    , '' [Reason]
                    , '' [Day Unit]
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

        ParameterInfo[] param = new ParameterInfo[17];
        param[0] = new ParameterInfo("@ControlNo", string.Empty);
        param[1] = new ParameterInfo("@EmployeeId", dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().Trim());
        param[2] = new ParameterInfo("@RecordDate", Convert.ToDateTime(dtPastedRecord.Rows[rowTrav]["Date of Leave"].ToString().Trim()));
        param[3] = new ParameterInfo("@LeaveType", dtPastedRecord.Rows[rowTrav]["Leave Type"].ToString().Trim());
        param[4] = new ParameterInfo("@StartTime", leaveStartTime);
        param[5] = new ParameterInfo("@EndTime", leaveEndTime);
        param[6] = new ParameterInfo("@LeaveHours", leaveHours);
        param[7] = new ParameterInfo("@Reason", dtPastedRecord.Rows[rowTrav]["Reason"].ToString().Trim());
        param[8] = new ParameterInfo("@CreatedBy", Session["userLogged"].ToString().Trim());
        param[9] = new ParameterInfo("@Filler1", filler1ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler1"].ToString().Trim() : string.Empty);
        param[10] = new ParameterInfo("@Filler2", filler2ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler2"].ToString().Trim() : string.Empty);
        param[11] = new ParameterInfo("@Filler3", filler3ColName != string.Empty ? dtPastedRecord.Rows[rowTrav]["Filler3"].ToString().Trim() : string.Empty);
        param[12] = new ParameterInfo("@DBPrefix", HttpContext.Current.Session["dbPrefix"].ToString().Trim());
        param[13] = new ParameterInfo("@MenuCode", "WFBLEAVEUPLD");
        param[14] = new ParameterInfo("@DayUnit", dtPastedRecord.Rows[rowTrav]["Day Unit"].ToString().Trim());
        param[15] = new ParameterInfo("@InformDate", DateTime.Now);
        param[16] = new ParameterInfo("@NotifyEmail", Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION));
        string query = @"EXEC CreateLeaveRecord @ControlNo
									, @EmployeeId
									, @RecordDate
									, @LeaveType
									, @StartTime
									, @EndTime
									, @LeaveHours
									, @DayUnit
									, @Reason
									, ''
									, @InformDate
									, ''
									, @CreatedBy
									, @DBPrefix
									, @MenuCode
									, @NotifyEmail
									, NULL
									, @Filler1
									, @Filler2
									, @Filler3
									, NULL
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
            SystemMenuLogBL.InsertAddLog("WFBLEAVEUPLD", true, dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
            dtPastedRecord.Rows[rowTrav]["Remarks"] = dtResult.Rows[0]["Message"].ToString();
        }
        else
        {
            SystemMenuLogBL.InsertAddLog("WFBLEAVEUPLD", false, dtPastedRecord.Rows[rowTrav]["Employee ID"].ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
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
                bDAYSEL = methods.GetProcessControlFlag("LEAVE", "DAYSEL");
                if (checkFormat(rowTrav, dtPastedRecord))
                {
                    try
                    {
                        dtResult = InsertDataSuccess(rowTrav, dtPastedRecord);
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
        dgvLeaveUpload.DataSource = dtResult;
        dgvLeaveUpload.DataBind();
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
            if (columnName.Equals("Date of Leave"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Date of Leave"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Leave Date must not be blank]";
                else if (!dtPastedRecord.Rows[rowNumber]["Date of Leave"].ToString().Trim().Equals(string.Empty))
                {
                    try
                    {
                        DateTime dateLV = Convert.ToDateTime(dtPastedRecord.Rows[rowNumber]["Date of Leave"].ToString().Trim());
                    }
                    catch
                    {
                        strRemarks += "[Invalid Leave Date (mm/dd/yyyy)]";
                    }
                }
            }
            if (columnName.Equals("Leave Type"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Leave Type"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Leave Type must not be blank]";
                else if (!dtPastedRecord.Rows[rowNumber]["Leave Type"].ToString().Trim().Equals(string.Empty))
                {
                    drArrResult = dtLeaveType.Select("Ltm_LeaveType = '" + dtPastedRecord.Rows[rowNumber]["Leave Type"].ToString().Trim() + "'");
                    if (drArrResult.Length == 0)
                        strRemarks += "[Leave Type does not exist]";
                }
            }
            if (columnName.Equals("Start Time"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim().Equals(string.Empty))
                    strRemarks += "[Start Time must not be blank]";
                if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length != 4)
                    strRemarks += "[Invalid Format (HHMM)]";
                else if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4)
                    leaveStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
            }
            if (columnName.Equals("End Time"))
            {
                if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim() != string.Empty
                    && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Trim() == string.Empty
                    && dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim() == string.Empty)
                    strRemarks += "[End Time or Hours must not be blank]";

                if (dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length != 4)
                    strRemarks += "[Invalid Format (HHMM)]";
                else if (dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length == 4)
                    leaveEndTime = dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Trim();
            }

            if (columnName.Equals("Hours"))
            {
                try
                {
                    if (dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                    {
                        leaveHours = Convert.ToDecimal(dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim());

                        if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4
                        && dtPastedRecord.Rows[rowNumber]["End Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Length == 4
                        && dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                        {
                            leaveHours = -1;
                            leaveStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
                            leaveEndTime = dtPastedRecord.Rows[rowNumber]["End Time"].ToString().Trim();                            
                        }
                        else if (dtPastedRecord.Rows[rowNumber]["Start Time"].ToString() != string.Empty && dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Length == 4
                            && dtPastedRecord.Rows[rowNumber]["End Time"].ToString() == string.Empty
                            && dtPastedRecord.Rows[rowNumber]["Hours"].ToString() != string.Empty)
                        {
                            leaveEndTime = string.Empty;
                            leaveStartTime = dtPastedRecord.Rows[rowNumber]["Start Time"].ToString().Trim();
                            leaveHours = Convert.ToDecimal(dtPastedRecord.Rows[rowNumber]["Hours"].ToString().Trim());
                        }
                    }
                    else
                        leaveHours = -1;
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
                    if (dtPastedRecord.Rows[rowNumber]["Leave Type"].ToString().Trim().ToUpper().Equals("SL") && dtPastedRecord.Rows[rowNumber]["Filler1"].ToString().Trim().Equals(string.Empty))
                        strRemarks += "[" + filler1ColName + " must not be blank]";
                }
            }
            if (filler2ColName != null && !filler2ColName.Equals(string.Empty))
            {
                if (columnName.Equals("Filler2"))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Filler2"].ToString().Equals(string.Empty))
                        strRemarks += "[" + filler2ColName + " must not be blank]";
                }
            }
            if (filler3ColName != null && !filler3ColName.Equals(string.Empty))
            {
                if (columnName.Equals("Filler3"))
                {
                    if (dtPastedRecord.Rows[rowNumber]["Filler3"].ToString().Equals(string.Empty))
                        strRemarks += "[" + filler3ColName + " must not be blank]";
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
            if (columnName.Equals("Day Unit"))
            {
                if (bDAYSEL == true)
                {
                    if (!dtPastedRecord.Rows[rowNumber]["Day Unit"].ToString().Trim().Equals(string.Empty))
                    {
                        drArrResult = dtDayUnit.Select("Pmx_Classification = '" + dtPastedRecord.Rows[rowNumber]["Day Unit"].ToString() + "'");
                        if (drArrResult.Length == 0)
                            strRemarks += "[Day Unit does not exist]";
                    }
                }
            }
        }

        dtPastedRecord.Rows[rowNumber]["Remarks"] = strRemarks;
        if (strRemarks.Trim() == "")
            isValid = true;
        return isValid;
    }
    private DataTable GetLeaveType()
    {
        string query = @"SELECT Ltm_LeaveType FROM T_LeaveTypeMaster
                            WHERE Ltm_Status = 'A'";

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
        bool isDayUnitEnable = methods.GetProcessControlFlag("LEAVE", "DAYSEL");

        string sql = @"SELECT Cfl_ColName
                            , Cfl_TextDisplay
                            , Cfl_Lookup
                            , Cfl_Status
                         FROM T_ColumnFiller
                        WHERE Cfl_ColName LIKE 'Elt_Filler%'
                          AND Cfl_Status = 'A'";
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
                if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("ELT_FILLER01"))
                {
                    filler1ColName =  txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler1"].ColumnName = filler1ColName;
                    thFiller1.InnerText =filler1ColName;
                    thFiller1.Visible = true;
                    tdFiller1.Visible = true;
                }
                else if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("ELT_FILLER02"))
                {
                    filler2ColName = txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler2"].ColumnName = filler2ColName;                    
                    thFiller2.InnerText = filler2ColName + "*";
                    thFiller2.Visible = true;
                    tdFiller2.Visible = true;
                }
                else if (ds.Tables[0].Rows[i]["Cfl_ColName"].ToString().ToUpper().Equals("ELT_FILLER03"))
                {
                    filler3ColName = txtInfo.ToTitleCase(ds.Tables[0].Rows[i]["Cfl_TextDisplay"].ToString());
                    dtCols.Columns["Filler3"].ColumnName = filler3ColName;                    
                    thFiller3.InnerText = filler3ColName + "*";
                    thFiller3.Visible = true;
                    tdFiller3.Visible = true;
                }
                else
                {
                    dtCols.Columns.Remove(ds.Tables[0].Rows[i]["Cfl_ColName"].ToString());
                    if (filler1ColName.Equals(string.Empty))
                    {
                        thFiller1.Visible = false;
                        tdFiller1.Visible = false;
                    }
                    else
                    {
                        thFiller1.Visible = true;
                        tdFiller1.Visible = true;
                    }

                    if (filler2ColName.Equals(string.Empty))
                    {
                        thFiller2.Visible = false;
                        tdFiller2.Visible = false;
                    }
                    else
                    {
                        thFiller2.Visible = true;
                        tdFiller2.Visible = true;
                    }
                    if (filler3ColName.Equals(string.Empty))
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

        if (!isDayUnitEnable)
        {
            dtCols.Columns.Remove("Day Unit");
            thDayUnit.Visible = false;
            tdDayUnit.Visible = false;
        }
        else
        {
            thDayUnit.Visible = true;
            tdDayUnit.Visible = true;
        }
        return dtCols;
    }
    private DataTable GetDayUnit()
    {
        string query = @"SELECT Pmx_Classification
                                FROM T_ParameterMasterExt
                                WHERE Pmx_ParameterID = 'LVDEDUCTN'
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
    private void UpdateRowCount(int rows)
    {
        lblRowCount.Text = string.Format("{0} Row(s)", rows);
    }
    #endregion    
}

