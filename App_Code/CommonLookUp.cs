using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using System.Collections;
using System.Web.SessionState;

/// <summary>
/// Summary description for CommonLookUp
/// </summary>
public class CommonLookUp
{
    System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
    public CommonLookUp()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void InitializeStatus(DropDownList ddl, string type)
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"   SELECT Adt_AccountDesc
                                        ,Adt_AccountCode
                                    FROM T_AccountDetail
                                   WHERE Adt_AccountType = @type
                                     AND Adt_Status = 'A'";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@type", type);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text, param).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddl.Items.Add(new ListItem(dr["Adt_AccountDesc"].ToString(), dr["Adt_AccountCode"].ToString()));
        }
    }

    public static void InitializeInStatus(DropDownList ddl, string type)
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"   SELECT Adt_AccountDesc
                                        ,Adt_AccountCode
                                    FROM T_AccountDetail
                                   WHERE Adt_AccountType in (@type)
                                     AND Adt_Status = 'A'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus.Replace("@type", type), CommandType.Text).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddl.Items.Add(new ListItem(dr["Adt_AccountCode"].ToString(), dr["Adt_AccountCode"].ToString()));
        }
    }

    public static void InitializeActiveStatus(DropDownList ddl)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem("", ""));
        ddl.Items.Add(new ListItem("ACTIVE", "A"));
        ddl.Items.Add(new ListItem("CANCELLED", "C"));
    }

    public static void InitializeLeaveType(DropDownList ddl)
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"  SELECT Ltm_LeaveType
                                        ,Ltm_LeaveDesc
                                    FROM T_LeaveTypeMaster
                                   WHERE Ltm_Status = 'A'";

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
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddl.Items.Add(new ListItem(dr["Ltm_LeaveDesc"].ToString(), dr["Ltm_LeaveType"].ToString()));
        }
    }

    public static bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
    {
        Double result;
        return Double.TryParse(val, NumberStyle,
            System.Globalization.CultureInfo.CurrentCulture, out result);
    }

    public static ArrayList DivideString(string str)
    {
        ArrayList arr = new ArrayList();
        string temp = "";
        for (int i = 0; i < str.Length; i++)
            if (str[i] != ',')
                temp += str[i];
            else if (str[i] == ',')
                if (temp != "")
                {
                    arr.Add(temp);
                    temp = "";
                }
        if (temp != "")
            arr.Add(temp);
        return arr;
    }

    public static DataRow GetCompanyInfo()
    {
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dt = dal.ExecuteDataSet(@"SELECT Ccd_CompanyCode
                                               ,  Ccd_CompanyName
                                               , Ccd_CompanyAddress1
                                               , Ccd_CompanyAddress2
                                               , Ccd_CompanyAddress3
                                               , Ccd_EmailAddress
                                               , Ccd_TelephoneNo
                                               , Ccd_CellNo
                                               , Ccd_FaxNo
                                               , Ccd_CompanyLogo
                                               , Ccd_SSS
                                               , Ccd_TIN
                                               , Ccd_CurrentYear
                                               , Ccd_LastEmployeeID
                                               , Ccd_status
                                               , Ccd_SystemVer
                                               , Ccd_SystemVerSwipe
                                               , Ccd_TaxSchedule
                                               , Ccd_BankCode
                                               , Ccd_BankAddress1
                                               , Ccd_BankAddress2
                                               , Ccd_BankAccountNo
                                               , Ccd_LastMCLNo
                                               , Ccd_BranchCode
                                               , Ccd_ATMBankCode
                                               , Ccd_DefaultShift
                                               , Ccd_DefaultRestday
                                               , Ccd_DefaultSSSCode
                                               , Ccd_DefaultPhilhealthCode
                                               , Ccd_DefaultHDMFCode
                                               , Ccd_WeekCoverage
                                               , Ccd_BankInCharge
                                               , Ccd_BICPosition
                                               , Ccd_BankAddress3
                                               , Ccd_LeaveCredit
                                               , Ccd_LeaveRefund
                                               , Ccd_Bonus
                                               , Ccd_PhilhealthNo
                                               , Ccd_HDMFNo 
                                            FROM T_CompanyMaster", CommandType.Text).Tables[0];
            }
            catch { }
        }
        if (dt.Rows.Count > 0)
            return dt.Rows[0];
        return null;
    }

    #region Gridview
    public static void SetGridViewCells(GridViewRow gvr, ArrayList arr)
    {
        if (arr == null)
            arr = new ArrayList();
        if (gvr.RowType == DataControlRowType.Header)
        {
            TableRow thr = gvr;
            for (int i = 1; i < thr.Cells.Count; i++)
            {
                thr.Cells[i].Wrap = false;
                thr.Cells[i].Text = HttpUtility.HtmlDecode(thr.Cells[i].Text);
                thr.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                thr.Cells[i].Attributes.Add("Width", "auto");
            }

        }
        else if (gvr.RowType == DataControlRowType.DataRow)
        {
            TableRow tr = gvr;
            for (int i = 1; i < tr.Cells.Count; i++)
            {
                tr.Cells[i].Wrap = false;
                tr.Cells[i].Attributes.Add("Width", "100%");
                try
                {
                    if (arr != null && arr.Contains(i))
                        tr.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
                catch (Exception)
                { }
                finally
                { }
            }
        }
    }
    public static void SetGridViewCellsV2(GridViewRow gvr, ArrayList arr)
    {
        if (arr == null)
            arr = new ArrayList();
        if (gvr.RowType == DataControlRowType.Header)
        {
            TableRow thr = gvr;
            for (int i = 0; i < thr.Cells.Count; i++)
            {
                thr.Cells[i].Wrap = false;
                thr.Cells[i].Text = HttpUtility.HtmlDecode(thr.Cells[i].Text);
                thr.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                if(i!=0)
                thr.Cells[i].Attributes.Add("Width", "auto");
                else
                    thr.Cells[i].Attributes.Add("Width", "auto");
            }

        }
        else if (gvr.RowType == DataControlRowType.DataRow)
        {
            TableRow tr = gvr;
            for (int i = 0; i < tr.Cells.Count; i++)
            {
                tr.Cells[i].Wrap = false;
                if (!tr.Cells[i].Text.Equals("&nbsp;"))
                    tr.Cells[i].Text = HttpUtility.HtmlDecode(tr.Cells[i].Text);
                tr.Cells[i].Attributes.Add("Width", "auto");
                try
                {
                    if (arr != null && arr.Contains(i))
                        tr.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
                catch (Exception)
                { }
                finally
                { }

            }
        }
    }
    public static void SetGridViewCells(GridViewRow gvr, ArrayList arrRightAlign, int startIndex)
    {
        if (arrRightAlign == null)
            arrRightAlign = new ArrayList();
        if (gvr.RowType == DataControlRowType.Header)
        {
            TableRow thr = gvr;
            for (int i = startIndex; i < thr.Cells.Count; i++)
            {
                thr.Cells[i].Wrap = false;
                thr.Cells[i].Text = HttpUtility.HtmlDecode(thr.Cells[i].Text);
                thr.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                thr.Cells[i].Attributes.Add("Width", "auto");
            }

        }
        else if (gvr.RowType == DataControlRowType.DataRow)
        {
            TableRow tr = gvr;
            for (int i = startIndex; i < tr.Cells.Count; i++)
            {
                tr.Cells[i].Wrap = false;
                tr.Cells[i].Attributes.Add("Width", "100%");
                try
                {
                    if (arrRightAlign != null && arrRightAlign.Contains(i))
                        tr.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
                catch (Exception)
                { }
                finally
                { }

            }
        }
    }
    #endregion

    #region Column
    public static ArrayList rightColChecker(ArrayList rightCols, DataTable dt)
    {
        if (rightCols == null)
            rightCols = new ArrayList();
        if (dt == null)
            dt = new DataTable();
        ArrayList retArr = new ArrayList();
        rightCols.Add("Applied Date");
        rightCols.Add("Endorsed Date");
        rightCols.Add("Checked Date 1");
        rightCols.Add("Checked Date 2");
        rightCols.Add("Approved Date");
        try
        {
            foreach (object obj in rightCols)
            {
                ////Andre replaced 08162009
                //if (dt.Columns.IndexOf((string)obj) > -1)
                //    retArr.Add(dt.Columns.IndexOf((string)obj));
                if (dt.Columns.IndexOf(obj.ToString()) > -1)
                    retArr.Add(dt.Columns.IndexOf(obj.ToString()));

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        return retArr;
    }
    #endregion

    #region HeaderText
    public static Panel HeaderPanelOption(int noColumns, string Report, string options)
    {
        return HeaderPanelOption(noColumns, Report, options, Unit.Pixel(0));
    }

    public static Panel HeaderPanelOption(int noColumns, string Report, string options, Unit tableWidth)
    {
        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();
        Label lblDeptReport = new Label();
        Label lbl5 = new Label();
        Label lblDateFrom = new Label();
        Label lbl7 = new Label();
        Label lblDateTO = new Label();
        Label lblOption = new Label();
        Panel panel1 = new Panel();
        Table table1 = new Table();

        for (int r = 0; r < 7; r++)
        {
            table1.Rows.Add(new TableRow());
            for (int i = 0; i < noColumns; i++)
                table1.Rows[r].Cells.Add(new TableCell());
        }
        //display First line header
        table1.Rows[0].Cells[0].Controls.Add(lbl1);
        table1.Rows[0].Cells[0].ColumnSpan = noColumns;
        lbl1.Text = MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyName");
        lbl1.Font.Size = 9;
        //display Second line header
        table1.Rows[1].Cells[0].Controls.Add(lbl2);
        table1.Rows[1].Cells[0].ColumnSpan = noColumns;
        lbl2.Text = MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress1") + ", " +
                    MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress2") + ", " +
                    MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress3");
        lbl2.Font.Size = 9;
        //display Third line header
        table1.Rows[2].Cells[0].Controls.Add(lbl3);
        table1.Rows[2].Cells[0].ColumnSpan = noColumns;
        lbl3.Text = "TEL NO. " + MethodsLibrary.Methods.GetCompanyInfo("Ccd_TelephoneNo") + "  FAX NO. " + MethodsLibrary.Methods.GetCompanyInfo("Ccd_FaxNo");
        lbl3.Font.Size = 9;
        //Space
        table1.Rows[3].Cells[0].ColumnSpan = noColumns;
        table1.Rows[3].Cells[0].Attributes["height"] = "10px";
        //display department line header
        table1.Rows[4].Cells[0].Controls.Add(lblDeptReport);
        table1.Rows[4].Cells[0].ColumnSpan = noColumns;
        table1.Rows[4].Cells[0].Attributes["align"] = "center";

        //string department;
        lblDeptReport.Text = Report;
        lblDeptReport.Font.Bold = true;
        table1.CellPadding = 0;
        table1.CellSpacing = 0;
        //Options
        if (options != string.Empty)
            lblOption.Text = "Option(s): " + options.Substring(0, options.Length - 1).Replace(";", " && ");
        else
            lblOption.Text = "Option(s): ALL ";
        lblOption.Font.Size = 9;
        table1.Rows[5].Cells[0].Controls.Add(lblOption);
        table1.Rows[5].Cells[0].ColumnSpan = noColumns;

        table1.Width = tableWidth;

        Panel retPan = new Panel();
        retPan.Attributes["Height"] = "auto";
        retPan.Attributes["width"] = "auto";
        retPan.Controls.Add(table1);

        return retPan;
    }

    public static Panel HeaderPanelOptionERP(int noColumns, string Report, string options, Unit tableWidth)
    {
        HttpSessionState session = HttpContext.Current.Session;
        HttpRequest request = HttpContext.Current.Request;
        string url = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath;

        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();
        Label lblDeptReport = new Label();
        Label lbl5 = new Label();
        Label lblDateFrom = new Label();
        Label lbl7 = new Label();
        Label lblDateTO = new Label();
        Label lblOption = new Label();
        Panel panel1 = new Panel();
        Table table1 = new Table();
        Image imglogo = new Image();

        if (url != null && session != null)
            imglogo.ImageUrl = url + "/GetImage.aspx?dbConn=" + session["dbConn"];

        for (int r = 0; r < 10; r++)
        {
            table1.Rows.Add(new TableRow());
            for (int i = 0; i < noColumns; i++)
                table1.Rows[r].Cells.Add(new TableCell());
        }

        imglogo.Width = Unit.Pixel(300);
        table1.Rows[0].Cells[0].Controls.Add(imglogo);
        table1.Rows[0].Cells[0].RowSpan = 4;
        table1.Rows[0].Cells[0].ColumnSpan = noColumns;

        //display Second line header
        table1.Rows[4].Cells[0].Controls.Add(lbl2);
        table1.Rows[4].Cells[0].ColumnSpan = noColumns;
        lbl2.Text = MethodsLibrary.Methods.GetCompanyInfoERP("Scm_CompAddress1");
        lbl2.Font.Size = 12;

        //display Third line header
        table1.Rows[5].Cells[0].Controls.Add(lbl1);
        table1.Rows[5].Cells[0].ColumnSpan = noColumns;
        lbl1.Text = MethodsLibrary.Methods.GetCompanyInfoERP("Scm_CompAddress2");
        lbl1.Font.Size = 12;

        //display Fourth line header
        table1.Rows[6].Cells[0].Controls.Add(lbl3);
        table1.Rows[6].Cells[0].ColumnSpan = noColumns;
        lbl3.Text = "TEL NO. " + MethodsLibrary.Methods.GetCompanyInfoERP("Scm_TelephoneNos") + "  FAX NO. " + MethodsLibrary.Methods.GetCompanyInfoERP("Scm_FaxNos");
        lbl3.Font.Size = 12;

        //Space
        table1.Rows[7].Cells[0].ColumnSpan = noColumns;
        table1.Rows[7].Cells[0].Attributes["height"] = "10px";
        //display department line header
        table1.Rows[8].Cells[0].Controls.Add(lblDeptReport);
        table1.Rows[8].Cells[0].ColumnSpan = noColumns;
        //table1.Rows[8].Cells[0].Attributes["align"] = "center";
        table1.Rows[8].Cells[0].HorizontalAlign = HorizontalAlign.Left;

        //string department;
        lblDeptReport.Text = Report;
        lblDeptReport.Font.Size = 12;
        lblDeptReport.Font.Bold = true;
        table1.CellPadding = 0;
        table1.CellSpacing = 0;
        //Options
        if (options != string.Empty)
            lblOption.Text = "Option(s): " + options.Substring(0, options.Length - 1).Replace(";", " && ");
        else
            lblOption.Text = "Option(s): ALL ";
        lblOption.Font.Size = 12;
        table1.Rows[9].Cells[0].Controls.Add(lblOption);
        table1.Rows[9].Cells[0].ColumnSpan = noColumns;

        table1.Width = tableWidth;

        Panel retPan = new Panel();
        retPan.Attributes["Height"] = "auto";
        retPan.Attributes["width"] = "auto";
        retPan.Controls.Add(table1);

        return retPan;
    }

	public static Panel HeaderPanelOptionERP(int noColumns, string Report, string options)
    {
        return HeaderPanelOptionERP(noColumns, Report, options, Unit.Pixel(0));
    }
    
    public static Panel HeaderPanelOption(int noColumns, int rowCount, string Report, string options)
    {
        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();
        Label lblDeptReport = new Label();
        Label lbl5 = new Label();
        Label lblDateFrom = new Label();
        Label lbl7 = new Label();
        Label lblDateTO = new Label();
        Label lblOption = new Label();
        Label lblDate = new Label();
        Label lblRowCount = new Label();
        Panel panel1 = new Panel();
        Table table1 = new Table();

        for (int r = 0; r < 8; r++)
        {
            table1.Rows.Add(new TableRow());
            for (int i = 0; i < noColumns; i++)
                table1.Rows[r].Cells.Add(new TableCell());
        }
        //display First line header
        table1.Rows[0].Cells[0].Controls.Add(lbl1);
        table1.Rows[0].Cells[0].ColumnSpan = noColumns;
        lbl1.Text = MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyName");
        lbl1.Font.Size = 9;
        //display Second line header
        table1.Rows[1].Cells[0].Controls.Add(lbl2);
        table1.Rows[1].Cells[0].ColumnSpan = noColumns;
        lbl2.Text = MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress1") + ", " +
                    MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress2") + ", " +
                    MethodsLibrary.Methods.GetCompanyInfo("Ccd_CompanyAddress3");
        lbl2.Font.Size = 9;
        //display Third line header
        table1.Rows[2].Cells[0].Controls.Add(lbl3);
        table1.Rows[2].Cells[0].ColumnSpan = noColumns;
        lbl3.Text = "TEL NO. " + MethodsLibrary.Methods.GetCompanyInfo("Ccd_TelephoneNo") + "  FAX NO. " + MethodsLibrary.Methods.GetCompanyInfo("Ccd_FaxNo");
        lbl3.Font.Size = 9;
        //Space
        table1.Rows[3].Cells[0].ColumnSpan = noColumns;
        table1.Rows[3].Cells[0].Attributes["height"] = "10px";
        //display department line header
        table1.Rows[4].Cells[0].Controls.Add(lblDeptReport);
        table1.Rows[4].Cells[0].ColumnSpan = noColumns;
        table1.Rows[4].Cells[0].Attributes["align"] = "center";

        //string department;
        lblDeptReport.Text = Report;
        lblDeptReport.Font.Bold = true;
        table1.CellPadding = 0;
        table1.CellSpacing = 0;
        //Options
        if (options != string.Empty)
            lblOption.Text = "Option(s): " + options.Substring(0, options.Length - 1).Replace(";", " && ");
        else
            lblOption.Text = "Option(s): ALL ";
        lblOption.Font.Size = 9;
        table1.Rows[5].Cells[0].Controls.Add(lblOption);
        table1.Rows[5].Cells[0].ColumnSpan = noColumns;
        //Row Count
        lblRowCount.Text = "Total Row(s) : " + rowCount.ToString();
        lblRowCount.Font.Size = 9;
        table1.Rows[6].Cells[0].Controls.Add(lblRowCount);
        table1.Rows[6].Cells[0].ColumnSpan = noColumns;
        lblDate.Text = "Date exported: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm");
        lblDate.Font.Size = 9;
        table1.Rows[7].Cells[0].Controls.Add(lblDate);
        Panel retPan = new Panel();
        retPan.Attributes["Height"] = "auto";
        retPan.Attributes["width"] = "auto";
        retPan.Controls.Add(table1);

        return retPan;
    }

    public static Panel GetHeaderPanelOption(int noColumns, int rowCount, string Report, string options)
    {
        HttpSessionState session = HttpContext.Current.Session;
        HttpRequest request = HttpContext.Current.Request;
        string url = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath;
        DataRow drCompanyInfo = null;
        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();
        Label lblDeptReport = new Label();
        Label lbl5 = new Label();
        Label lblDateFrom = new Label();
        Label lbl7 = new Label();
        Label lblDateTO = new Label();
        Label lblOption = new Label();
        Label lblDate = new Label();
        Label lblRowCount = new Label();
        Panel panel1 = new Panel();
        Table table1 = new Table();
        Image imglogo = new Image();

        imglogo.Height = Unit.Pixel(120);
        imglogo.Style.Add("overflow", "hidden");
        drCompanyInfo = GetCompanyInfo();
        if (url != null && session != null)
            imglogo.ImageUrl = url + "/GetCompanyLogoImage.aspx?dbConn=" + session["dbConn"];

        for (int r = 0; r < 15; r++)
        {
            table1.Rows.Add(new TableRow());
            for (int i = 0; i < noColumns; i++)
                table1.Rows[r].Cells.Add(new TableCell());
        }

        imglogo.Width = Unit.Pixel(300);
        table1.Rows[0].Cells[0].Controls.Add(imglogo);
        table1.Rows[0].Cells[0].RowSpan = 4;
        table1.Rows[0].Cells[0].ColumnSpan = noColumns;


        //display Second line header
        table1.Rows[8].Cells[0].Controls.Add(lbl2);
        table1.Rows[8].Cells[0].ColumnSpan = noColumns;
        lbl2.Text = drCompanyInfo["Ccd_CompanyAddress1"].ToString();
        lbl2.Font.Size = 12;

        //display Third line header
        table1.Rows[9].Cells[0].Controls.Add(lbl1);
        table1.Rows[9].Cells[0].ColumnSpan = noColumns;
        lbl1.Text = drCompanyInfo["Ccd_CompanyAddress2"].ToString();
        lbl1.Font.Size = 12;

        //display Fourth line header
        table1.Rows[10].Cells[0].Controls.Add(lbl3);
        table1.Rows[10].Cells[0].ColumnSpan = noColumns;
        lbl3.Text = "TEL NO. " + drCompanyInfo["Ccd_TelephoneNo"].ToString() + "  FAX NO. " + drCompanyInfo["Ccd_FaxNo"].ToString();
        lbl3.Font.Size = 12;

        //Space
        table1.Rows[11].Cells[0].ColumnSpan = noColumns;
        table1.Rows[11].Cells[0].Attributes["height"] = "10px";

        //display department line header
        table1.Rows[12].Cells[0].Controls.Add(lblDeptReport);
        table1.Rows[12].Cells[0].ColumnSpan = noColumns;
        //table1.Rows[8].Cells[0].Attributes["align"] = "center";
        table1.Rows[8].Cells[0].HorizontalAlign = HorizontalAlign.Left;

        //string department;
        lblDeptReport.Text = Report;
        lblDeptReport.Font.Bold = true;
        lblDeptReport.Font.Size = 12;
        table1.CellPadding = 0;
        table1.CellSpacing = 0;

        //Options
        if (options != string.Empty)
            lblOption.Text = " Option(s): " + options.Substring(0, options.Length - 1).Replace(";", " && ");
        else
            lblOption.Text = " Option(s): ALL ";
        lblOption.Font.Size = 12;
        table1.Rows[9].Cells[0].Controls.Add(lblOption);
        table1.Rows[9].Cells[0].ColumnSpan = noColumns;

        //Row Count
        lblRowCount.Text = " Total Row(s) : " + rowCount.ToString();
        lblRowCount.Font.Size = 12;
        table1.Rows[10].Cells[0].Controls.Add(lblRowCount);

        table1.Rows[10].Cells[0].ColumnSpan = noColumns;
        lblDate.Text = " Date Exported: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        lblDate.Font.Size = 12;
        table1.Rows[11].Cells[0].Controls.Add(lblDate);

        Panel retPan = new Panel();
        retPan.Attributes["Height"] = "auto";
        retPan.Attributes["width"] = "auto";
        retPan.Controls.Add(table1);

        return retPan;
    }

    public static Panel HeaderPanelOptionERP(int noColumns, int rowCount, string Report, string options)
    {
        HttpSessionState session = HttpContext.Current.Session;
        HttpRequest request = HttpContext.Current.Request;
        string url = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath;

        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();
        Label lblDeptReport = new Label();
        Label lbl5 = new Label();
        Label lblDateFrom = new Label();
        Label lbl7 = new Label();
        Label lblDateTO = new Label();
        Label lblOption = new Label();
        Label lblDate = new Label();
        Label lblRowCount = new Label();
        Panel panel1 = new Panel();
        Table table1 = new Table();
        Image imglogo = new Image();

        if (url != null && session != null)
            imglogo.ImageUrl = url + "/GetImage.aspx?dbConn=" + session["dbConn"];

        for (int r = 0; r < 12; r++)
        {
            table1.Rows.Add(new TableRow());
            for (int i = 0; i < noColumns; i++)
                table1.Rows[r].Cells.Add(new TableCell());
        }

        table1.Rows[0].Cells[0].Controls.Add(imglogo);
        table1.Rows[0].Cells[0].RowSpan = 4;
        table1.Rows[0].Cells[0].ColumnSpan = noColumns;

        //display Second line header
        table1.Rows[4].Cells[0].Controls.Add(lbl2);
        table1.Rows[4].Cells[0].ColumnSpan = noColumns;
        lbl2.Text = MethodsLibrary.Methods.GetCompanyInfoERP("Scm_CompAddress1");
        lbl2.Font.Size = 12;

        //display Third line header
        table1.Rows[5].Cells[0].Controls.Add(lbl1);
        table1.Rows[5].Cells[0].ColumnSpan = noColumns;
        lbl1.Text = MethodsLibrary.Methods.GetCompanyInfoERP("Scm_CompAddress2");
        lbl1.Font.Size = 12;

        //display Fourth line header
        table1.Rows[6].Cells[0].Controls.Add(lbl3);
        table1.Rows[6].Cells[0].ColumnSpan = noColumns;
        lbl3.Text = "TEL NO. " + MethodsLibrary.Methods.GetCompanyInfoERP("Scm_TelephoneNos") + "  FAX NO. " + MethodsLibrary.Methods.GetCompanyInfoERP("Scm_FaxNos");
        lbl3.Font.Size = 12;

        //Space
        table1.Rows[7].Cells[0].ColumnSpan = noColumns;
        table1.Rows[7].Cells[0].Attributes["height"] = "10px";

        //display department line header
        table1.Rows[8].Cells[0].Controls.Add(lblDeptReport);
        table1.Rows[8].Cells[0].ColumnSpan = noColumns;
        //table1.Rows[8].Cells[0].Attributes["align"] = "center";
        table1.Rows[8].Cells[0].HorizontalAlign = HorizontalAlign.Left;

        //string department;
        lblDeptReport.Text = Report;
        lblDeptReport.Font.Bold = true;
        lblDeptReport.Font.Size = 12;
        table1.CellPadding = 0;
        table1.CellSpacing = 0;

        //Options
        if (options != string.Empty)
            lblOption.Text = "Option(s): " + options.Substring(0, options.Length - 1).Replace(";", " && ");
        else
            lblOption.Text = "Option(s): ALL ";
        lblOption.Font.Size = 12;
        table1.Rows[9].Cells[0].Controls.Add(lblOption);
        table1.Rows[9].Cells[0].ColumnSpan = noColumns;

        //Row Count
        lblRowCount.Text = "Total Row(s) : " + rowCount.ToString();
        lblRowCount.Font.Size = 12;
        table1.Rows[10].Cells[0].Controls.Add(lblRowCount);

        table1.Rows[10].Cells[0].ColumnSpan = noColumns;
        lblDate.Text = "Date exported: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm");
        lblDate.Font.Size = 12;
        table1.Rows[11].Cells[0].Controls.Add(lblDate);

        Panel retPan = new Panel();
        retPan.Attributes["Height"] = "auto";
        retPan.Attributes["width"] = "auto";
        retPan.Controls.Add(table1);

        return retPan;
    }

    #endregion

    #region Lookup Queries
    public static string LookupQuery(string Query, string var1)
    {
        string sqlSelect = "";
        switch (Query)
        {
            case "ClientCode": sqlSelect = @"SELECT Slm_ClientJobNo [Client Job No]
	                                          ,Slm_DashJobCode [Dash Job Code]
                                              ,Slm_ClientJobName [Client Job Name]
                                              ,Slm_ClientCode [Client Code]
                                              ,Slm_WorkScope [Work Scope]
                                          FROM T_SalesMaster
                                        "; break;
            case "Contact": sqlSelect = @"SELECT Cld_Seqno [Seq No]
                                              ,Cld_ContactPerson [Contact Name]
                                              ,Cld_Position [Position]
                                              ,Cld_Department [Department]
                                              ,Cld_AttentionTelNo [Tel No]
                                              ,Cld_AttentionFaxNo [Fax No]
                                              ,Cld_AttentionEmailAdd [Email]
                                          FROM T_ClientDetail
                            WHERE Cld_ClientCode = '@temp'".Replace("@temp", var1); break;


            default: break;
        }
        return sqlSelect;
    }

    #endregion

    public static DataTable GetUserCostCenterCode(string userId, string SystemId)
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"   Select Uca_CostCenterCode
                        From T_UserCostCenterAccess
                        where Uca_SytemID = @SystemId
                        and Uca_Usercode = @userId
                        and Uca_Status = 'A'
                        ";

        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@userId", userId);
        param[1] = new ParameterInfo("@SystemId", SystemId);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text, param).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (dt.Rows.Count > 0)
        {
            return dt;
        }
        return null;
    }

    public static string GetCostCenterOfEmployee(string id)
    {
        string sql = string.Format(@"select Emt_CostCenterCode from T_EmployeeMaster
                            where Emt_EmployeeID = '{0}'", id);

        string temp = "";
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

        return temp;
    }

    public static DataRow GetCheckApproveRights(string userID, string sysMenuCode)
    {
        DataSet dsRights = new DataSet();

        #region sql query
        string sqlGetRights = @"select Ugt_CanCheck
                                      ,Ugt_CanApprove
                                      ,Ugt_CanGenerate
                                      ,Ugt_CanPrint
                                    from t_usergrant
                                    left join t_usergroupdetail on ugd_usercode = '{0}'
                                    where   ugt_systemid  = ugd_systemid
                                     and ugt_usergroup = ugd_usergroupcode
                                        and ugt_sysmenucode = '{1}'
                                     and ugt_status    = 'A'";
        #endregion

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsRights = dal.ExecuteDataSet(string.Format(sqlGetRights, userID, sysMenuCode), CommandType.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (dsRights != null && dsRights.Tables.Count > 0 && dsRights.Tables[0].Rows.Count > 0)
            return dsRights.Tables[0].Rows[0];
        else
            return null;
    }

    public static DataRow GetSignatory(string sysMenuCode)
    {
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                ParameterInfo[] param = new ParameterInfo[1];
                param[0] = new ParameterInfo("@menuCode", sysMenuCode);
                dt = dal.ExecuteDataSet(@"SELECT Dsm_Sign1
	                              ,RTRIM(s1.Emt_FirstName) + ' ' + RTRIM(s1.Emt_LastName) [Sig1_Name]
	                              ,p1.Adt_Accountdesc [Sig1_Position]
	                              ,Dsm_Sign2
	                              ,RTRIM(s2.Emt_FirstName) + ' ' + RTRIM(s2.Emt_LastName) [Sig2_Name]
	                              ,p2.Adt_Accountdesc [Sig2_Position]
	                              ,Dsm_Sign3
                                  ,RTRIM(s3.Emt_FirstName) + ' ' + RTRIM(s3.Emt_LastName) [Sig3_Name]
	                              ,p3.Adt_Accountdesc [Sig3_Position]	
                             FROM T_DocumentSignatoryMaster
                            LEFT JOIN T_EmployeeMaster s1 ON s1.Emt_EmployeeId = Dsm_Sign1
                            LEFT JOIN T_EmployeeMaster s2 ON s2.Emt_EmployeeId = Dsm_Sign2
                            LEFT JOIN T_EmployeeMaster s3 ON s3.Emt_EmployeeId = Dsm_Sign3
                            LEFT JOIN T_AccountDetail p1 ON p1.Adt_AccountCode = s1.Emt_PositionCode
	                              AND p1.Adt_AccountType = 'POSITION'
                            LEFT JOIN T_AccountDetail p2 ON p2.Adt_AccountCode = s2.Emt_PositionCode
	                              AND p1.Adt_AccountType = 'POSITION'
                            LEFT JOIN T_AccountDetail p3 ON p3.Adt_AccountCode = s3.Emt_PositionCode
	                              AND p1.Adt_AccountType = 'POSITION'
                            WHERE Dsm_MenuCode = @menuCode", CommandType.Text, param).Tables[0];
            }
            catch
            {
            }
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            return null;
        }
    }

    public static DataRow GetClientDetails(string clientCode)
    {
        DataTable dt;
        string sqlSelect = @"SELECT Clh_ClientCode
                                  ,Clh_ClientName
                                  ,Clh_ClientShortName
                                  ,Clh_ClientAddress
                                  ,Clh_BillingCycle
                                  ,Clh_LastSeriesNo
                                  ,Clh_Remarks
                                  ,Clh_Status
                              FROM T_ClientHeader
                             WHERE Clh_ClientCode = @clientCode";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@clientCode", clientCode, SqlDbType.VarChar, 10);
        using (DALHelper dal = new DALHelper())
        {
            dt = dal.ExecuteDataSet(sqlSelect, CommandType.Text, param).Tables[0];
        }
        if (dt.Rows.Count == 1)
            return dt.Rows[0];
        else
            throw new Exception("Failed to fetch Client Details.");
        
        
    }

    public static DataRow GetClientContactDetails(string clientCode, string seqNo)
    {
        DataTable dt;
        string sqlSelect = @"SELECT Cld_ClientCode
                                  ,Cld_Seqno
                                  ,Cld_ContactPerson
                                  ,Cld_Position
                                  ,Cld_Department
                                  ,Cld_AttentionTelNo
                                  ,Cld_AttentionFaxNo
                                  ,Cld_AttentionEmailAdd
                                  ,Cld_Status
                                  ,Usr_Login
                                  ,Ludatetime
                              FROM T_ClientDetail
                             WHERE Cld_ClientCode = @clientCode
                               AND Cld_Seqno = @seqNo";
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@clientCode", clientCode, SqlDbType.VarChar, 10);
        param[1] = new ParameterInfo("@seqNo", seqNo, SqlDbType.VarChar, 2);
        using (DALHelper dal = new DALHelper())
        {
            dt = dal.ExecuteDataSet(sqlSelect, CommandType.Text, param).Tables[0];
        }
        if (dt.Rows.Count == 1)
            return dt.Rows[0];
        else
            throw new Exception("Failed to fetch Client Contact Details.");
        //return null;
    }

    #region This is used for gettinf the day of the week used in client side initialization
    public static string GetValueForWeek()
    {
        DataSet ds = new DataSet();

        #region SQL Query
        string sqlQuery = "SELECT Ccd_WeekCoverage FROM T_CompanyMaster";
        #endregion

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return ds.Tables[0].Rows[0][0].ToString();
    }
    #endregion

    public static string GetJobBillingCycle(string dashJobCode, string clientJobNo)
    {
        string sql = string.Format(@"
                SELECT Bcc_BillingCycle 
                  FROM T_SalesMaster
                 INNER JOIN T_CostCenterBillingCycle
                    ON Bcc_CostCenter = Slm_CostCenter
                 WHERE Slm_DashJobCode = '{0}'
                   AND Slm_ClientJobNo = '{1}' ", dashJobCode, clientJobNo);

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

        return temp;
    }

   



}
