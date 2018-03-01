using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Payroll.DAL;
using System.IO;
using Microsoft.Office.Interop.Word; //New
using Microsoft.Office.Core; //New
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Collections; //New

public partial class Transactions_CustomForms_GatePassReport : System.Web.UI.Page
{
    byte[] _DocFile = null;  
    Document doc = null;
    Application word;
    CommonMethods methods = new CommonMethods();

    protected void Page_Load(object sender, EventArgs e)
    {       
         
    }
    public System.Byte[] FetchDocument()
    {
        DataSet ds = new DataSet();

        string sqlQuery = @"select 
                                dm_DocTemplate
                                from T_DocumentTypeMaster
                                Where dm_DocCode = 'GATEPASS'";
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
            }
            catch (Exception Error)
            {
                int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(Error);
                //CommonProcedures.ShowMessage(messageCode, "");
            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (ds.Tables[0].Rows.Count > 0)
            return (byte[])ds.Tables[0].Rows[0]["dm_DocTemplate"];
        else
            return null;
    }

    private void getTemplate()
    {
       this._DocFile = (byte[])FetchDocument();
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
            string employeeId = "";
            object file = string.Empty;
            string pathValue = string.Empty;
            System.Data.DataTable testDT = getGatePassTags();
            try
            {
                //pathValue = System.Configuration.ConfigurationManager.AppSettings["DocumentTaggingSavePath"].ToString();

                //if (pathValue.Trim().Equals(string.Empty))
                pathValue = @"C:\DocTag\";
            }
            catch
            {
                pathValue = @"C:\DocTag\";
            }

            file = string.Format(@"{2}{0}_{1}.doc", "GATEPASS", employeeId, pathValue.Trim());

            try
            {
                if (!Directory.Exists(pathValue.Trim()))
                    Directory.CreateDirectory(pathValue.Trim());
            }
            catch
            {
                //CommonProcedures.ShowMessage(10068, pathValue.Trim(), this.menuCode);
            }

            try
            {
                if (File.Exists(file.ToString()))
                    File.Delete(file.ToString());

                FileStream fs = new FileStream(file.ToString(), FileMode.Create, FileAccess.Write);
                fs.Write(this._DocFile, 0, this._DocFile.Length);
                fs.Close();

                object filename = file;
                object falseValue = false;
                object trueValue = true;
                object missing = Type.Missing;
                string tag = string.Empty;
                string value = string.Empty;
                if (this.doc != null)
                    this.doc = null;
                if (this.word != null)
                    this.word = null;
                this.word = new Application();
                this.word.Visible = false;

                doc = word.Documents.Open(ref filename, ref missing, ref falseValue, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing,  ref missing,  ref missing,  ref missing,  ref missing);

                doc.Activate();
                //=================================================================
                //for (int i = 0; i < this.dgvDocumentTags.RowCount; i++)
                //{
                //    tag = this.dgvDocumentTags.Rows[i].Cells[0].Value.ToString();
                //    value = this.dgvDocumentTags.Rows[i].Cells[1].Value.ToString();
                //    SearchReplace(tag, value);
                //}

                for (int i = 0; i < testDT.Rows.Count; i++)
                { 
                    
                }
                //=================================================================
                doc.SaveAs(ref file, ref missing,
                  ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing,
                  ref missing, ref missing);
                //doc.Close(ref missing, ref missing, ref missing);
                word.Quit(ref missing, ref missing, ref missing);
                //if (CommonProcedures.ShowMessage(60004, this.menuCode) == CommonEnum.GenieDialogResult.Yes)
                //{
                //    System.Diagnostics.Process.Start(file.ToString());
            }

            catch
            {
                //CommonProcedures.ShowMessage(10069, string.Format("\"{0}\"", file), this.menuCode);
            }
        }

    private void searchWordToReplace()
    { 
        
    }
    private System.Data.DataTable getEmployeeGatePassRecord()
    {
        System.Data.DataTable dtResult = new System.Data.DataTable();
        using (DALHelper dal = new DALHelper())
        {
            string query = @"SELECT 
                ";
            dal.OpenDB();

            dtResult = dal.ExecuteDataSet(query).Tables[0];

            dal.CloseDB();
        }
        return dtResult;
    }

    private System.Data.DataTable getGatePassTags()
    {
        System.Data.DataTable dtResult = new System.Data.DataTable();
        using (DALHelper dal = new DALHelper())
        {
            string query = @"SELECT 
                dttm_TagCode,
                dtm_ResultScript,
                dtm_LookUpScript,
                dtm_TagDescription,
                dtm_ResultArgumentIdx
                FROM T_DocumentTypeTagMaster
                LEFT OUTER JOIN T_DocumentTagMaster
                ON dttm_TagCode = dtm_TagCode
                WHERE dttm_DocCode = 'GATEPASS'
                AND dttm_Status = 'A'";
            dal.OpenDB();

            dtResult = dal.ExecuteDataSet(query).Tables[0];

            dal.CloseDB();
        }
        return dtResult;
    }

    private void SearchReplace(object find, object replace)
    {

        object missing = Type.Missing;

        Range range = word.ActiveDocument.Content;
        object replaceAll = WdReplace.wdReplaceAll;
        object myFind = range.Find;

        Console.WriteLine(myFind.GetType().InvokeMember("Text", BindingFlags.GetProperty, null, myFind, null));
        object findText = find;
        object replaceText = replace;

        // Find "alow" and replace with "allow"
        //try
        //{
        object[] Parameters;
        Parameters = new object[15];
        Parameters[0] = findText;
        Parameters[1] = missing;
        Parameters[2] = missing;
        Parameters[3] = missing;
        Parameters[4] = missing;
        Parameters[5] = missing;
        Parameters[6] = missing;
        Parameters[7] = missing;
        Parameters[8] = missing;
        Parameters[9] = replaceText;
        Parameters[10] = replaceAll;
        Parameters[11] = missing;
        Parameters[12] = missing;
        Parameters[13] = missing;
        Parameters[14] = missing;

        myFind.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, myFind, Parameters);
        //}
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
        //MenuLog
        SystemMenuLogBL.InsertGenerateLog("WFLVEREP", txtEmployee.Text, true, Session["userLogged"].ToString());
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(SQLBuilder(string.Empty).Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
            RemoveColumns(ds);
            hfRowCount.Value = "0";
            foreach (DataRow dr in ds.Tables[1].Rows)
                hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        }
        dgvResult.DataSource = ds;
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
    private string SQLBuilder(string stringReplace)
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string enrouteQuery = string.Empty;
        StringBuilder sql = new StringBuilder();
        
            sql.Append(@"declare @startIndex int;
                            SET @startIndex = (@pageIndex * @numRow) + 1;
                           WITH TempTable AS ( SELECT Row_Number()
                                                 OVER ( ORDER BY [Lastname], [Gate Pass Date] @Sort) [Row]
                                                    , *
                                                 FROM ( ");
            sql.Append(getColumns());
            sql.Append(getFilters());
            sql.Append(string.Format(@"  ) AS temp)
                                               SELECT [Control No]
                                                    , [Status]
                                                    , [ID No]
                                                    , [Nickname]
                                                    , [ID Code]
                                                    , [Lastname]
                                                    , [Firstname]
                                                    , [Gate Pass Date]
                                                    , [Application Type]
                                                    , [Cost Center]
                                                   
                                                    , [Applied Date]
                                                    , [Endorsed Date]
                                                    , [Application Type Remarks]
                                                    , [First Name]
                                                    , [Last Name]
                                                    , [Checker 1]
                                                    , [Checked Date 1]
                                                    , [Checker 2]
                                                    , [Checked Date 2]
                                                    , [Approver]
                                                    , [Approved Date]
                                                    , [Remarks]
                                                    , [Pay Period]
                                                 FROM TempTable
                                                !#!WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1
                                                ", !hasCCLine ? "" : ", [CC Line]"));
            sql.Append(@" SELECT SUM(cnt)
                            FROM ( SELECT COUNT(distinct Egp_ControlNo) [cnt]");
            sql.Append(getFilters());
           
            sql.Append(@"        ) as Rows");
            sql = sql.Replace("@Checker1", "C1.Umt_UserCode").Replace("@Checker2", "C2.Umt_UserCode").Replace("@Approver", "AP.Umt_UserCode");
        return sql.ToString().Replace("!#!", stringReplace).Replace("@Sort", Resources.Resource.REPORTSORTING);
    }
    private string getColumns()
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string columns = string.Empty;
       
            columns = string.Format(@"   SELECT distinct Egp_ControlNo [Control No]
                                              , Egp_EmployeeId [ID No]
                                              , Emt_NickName [ID Code]
                                              , Emt_NickName [Nickname]
                                              , Emt_Lastname [Lastname]
                                              , Emt_Firstname [Firstname]
                                              , Convert(varchar(10), Egp_GatePassDate, 101) [Gate Pass Date]
                                              , Egp_ApplicationType [Application Type]
                                              , dbo.getCostCenterFullNameV2(Egp_CostCenter) [Cost Center]
                                              
                                              , Convert(varchar(10), Egp_AppliedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Egp_AppliedDate,113),5) [Applied Date]
                                              , Convert(varchar(10), Egp_EndorsedDateToChecker,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Egp_EndorsedDateToChecker,113),5) [Endorsed Date]
                                              , Egp_ApplicationTypeRemarks [Application Type Remarks]
                                              , AD1.Adt_AccountDesc [Status]
                                              , Emt_FirstName [First Name]
                                              , Emt_LastName [Last Name]
                                              , dbo.GetControlEmployeeNameV2(@Checker1) [Checker 1]
                                              , Convert(varchar(10), Egp_CheckedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Egp_CheckedDate,113),5) [Checked Date 1]
	                                          , dbo.GetControlEmployeeNameV2(@Checker2) [Checker 2]
                                              , Convert(varchar(10), Egp_Checked2Date,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Egp_Checked2Date,113),5) [Checked Date 2]
	                                          , dbo.GetControlEmployeeNameV2(@Approver) [Approver]
                                              , Convert(varchar(10), Egp_ApprovedDate,101) 
                                                + ' ' 
                                                + RIGHT(Convert(varchar(17), Egp_ApprovedDate,113),5) [Approved Date]
                                              , Trm_Remarks [Remarks]
                                              , Egp_CurrentPayPeriod [Pay Period]", !hasCCLine ? "" : ", Egp_CostCenterLine [CC Line]");
       
        return columns;
    }
    private void RemoveColumns(DataSet ds)
    {
        if (!CommonMethods.isEmpty(ds))
        {
            #region Remove Columns
            if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
            {
                if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                {
                    ds.Tables[0].Columns.Remove("Nickname");
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                }
            }
            else
            {
                ds.Tables[0].Columns.Remove("ID Code");
                ds.Tables[0].Columns.Remove("Nickname");
            }

            if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
            {
                ds.Tables[0].Columns.Remove("Lastname");
                ds.Tables[0].Columns.Remove("Firstname");
                ds.Tables[0].Columns.Remove("MI");
            }

            //Includes
            for (int i = 0; i < cblInclude.Items.Count; i++)
            {
                try
                {
                    
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                catch
                {
                    //do nothing. this is to trap for leave notice
                }
            }
            #endregion
        }
    }
    private string getFilters()
    {
        //apsungahid added 20141124
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string filter = string.Empty;
        string searchFilter = string.Empty;
      
            filter = string.Format(@"    FROM E_EmployeeGatePass
                                          LEFT JOIN T_UserMaster C1 
                                            ON C1.Umt_UserCode = Egp_CheckedBy
                                          LEFT JOIN T_UserMaster C2 
                                            ON C2.Umt_UserCode = Egp_Checked2By
                                          LEFT JOIN T_UserMaster AP 
                                            ON AP.Umt_UserCode = Egp_ApprovedBy
                                          LEFT JOIN T_EmployeeMaster 
                                            ON Emt_EmployeeId = Egp_EmployeeId
                                          LEFT JOIN T_AccountDetail AD1 
                                            ON AD1.Adt_AccountCode = Egp_Status 
                                           AND AD1.Adt_AccountType =  'WFSTATUS'
                                          LEFT JOIN T_PayPeriodMaster ON Ppm_PayPeriod = Egp_CurrentPayPeriod
                                          LEFT JOIN T_TransactionRemarks ON Trm_ControlNo = Egp_ControlNo
                                         WHERE 1 = 1 AND Egp_Status <> '' ");
            #region textBox Filters
            if (!txtEmployee.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Emt_EmployeeId {0}
                                               OR Emt_Lastname {0}
                                               OR Emt_Firstname {0}
                                               OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
            }
            if (!txtCostcenter.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_Costcenter {0}
                                               OR dbo.getCostCenterFullNameV2(Egp_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text));
            }

            if (!txtStatus.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_Status {0}
                                               OR AD1.Adt_AccountDesc {0})", sqlINFormat(txtStatus.Text));
            }

            //No need else because if employee login user cannot change the txtEmployee filter 
            //so report would always filter only user's own transaction
            //tbrEmployee.Visible -> indicates that user is not EMPLOYEE or has the right to check 
            //assuming EMPLOYEE GROUP has no right to check at all.( Ugt_CanCheck )
            if (tbrEmployee.Visible)
            {
                if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "PERSONNEL"))
                {
                    filter += string.Format(@" AND  (  ( Egp_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                               FROM T_UserCostCenterAccess
                                                                              WHERE Uca_UserCode = '{0}'
                                                                                AND Uca_SytemId = 'PERSONNEL')
                                                      OR Egp_EmployeeId = '{0}'))", Session["userLogged"].ToString());


                }
//                if (hasCCLine)//flag costcenter line to retain old code
//                {
//                    filter += string.Format(@" AND (Egp_CostCenter IN (SELECT Cct_CostCenterCode FROM dbo.GetCostCenterLineAccess('{0}', 'PERSONNEL') WHERE Clm_LineCode IS NULL OR Clm_LineCode = 'ALL')
//									            OR Egp_CostCenter + ISNULL(Egp_CostCenterLine,'') IN (SELECT Cct_CostCenterCode + ISNULL(Clm_LineCode,'') FROM dbo.GetCostCenterLineAccess('{0}', 'PERSONNEL'))
//                                                OR Egp_EmployeeID = '{0}') ", Session["userLogged"].ToString());
//                }
            }

            if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_CurrentPayPeriod {0})", sqlINFormat(txtPayPeriod.Text));
            }
            if (!txtChecker1.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_CheckedBy {0}
                                               OR C1.Umt_UserCode {0}
                                               OR C1.Umt_UserLname {0}
                                               OR C1.Umt_UserFname {0}
                                               OR C1.Umt_UserNickname {0})", sqlINFormat(txtChecker1.Text));
            }
            if (!txtChecker2.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_Checked2By {0}
                                               OR C2.Umt_UserCode {0}
                                               OR C2.Umt_UserLname {0}
                                               OR C2.Umt_UserFname {0}
                                               OR C2.Umt_UserNickname {0})", sqlINFormat(txtChecker2.Text));
            }
            if (!txtApprover.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Egp_ApprovedBy {0}
                                               OR AP.Umt_UserCode {0}
                                               OR AP.Umt_UserLname {0}
                                               OR AP.Umt_UserFname {0}
                                               OR AP.Umt_UserNickname {0})", sqlINFormat(txtApprover.Text));
            }
            #endregion
            #region DateTime Pickers
            //LEAVE Date
            if (!dtpGatePassFrom.IsNull && !dtpGatePassTo.IsNull)
            {
                filter += string.Format(@" AND Egp_GatePassDate BETWEEN '{0}' AND '{1}'", dtpGatePassFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpGatePassTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpGatePassFrom.IsNull)
            {
                filter += string.Format(@" AND Egp_GatePassDate >= '{0}'", dtpGatePassFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpGatePassTo.IsNull)
            {
                filter += string.Format(@" AND Egp_GatePassDate <= '{0}'", dtpGatePassTo.Date.ToString("MM/dd/yyyy"));
            }

            //Applied Date
            if (!dtpAppliedFrom.IsNull && !dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Egp_AppliedDate BETWEEN '{0}' AND '{1}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy")
                                                                                       , dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedFrom.IsNull)
            {
                filter += string.Format(@" AND Egp_AppliedDate >= '{0}'", dtpAppliedFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAppliedTo.IsNull)
            {
                filter += string.Format(@" AND Egp_AppliedDate <= '{0}'", dtpAppliedTo.Date.ToString("MM/dd/yyyy"));
            }
            //Endorsed Date
            if (!dtpEndorsedFrom.IsNull && !dtpEndorsedTo.IsNull)
            {
                filter += string.Format(@" AND Egp_EndorsedDateToChecker BETWEEN '{0}' AND '{1}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy")
                                                                                                 , dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpEndorsedFrom.IsNull)
            {
                filter += string.Format(@" AND Egp_EndorsedDateToChecker >= '{0}'", dtpEndorsedFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpEndorsedTo.IsNull)
            {
                filter += string.Format(@" AND Egp_EndorsedDateToChecker <= '{0}'", dtpEndorsedTo.Date.ToString("MM/dd/yyyy"));
            }

          
            //Checked Date
            if (!dtpC1From.IsNull && !dtpC1To.IsNull)
            {
                filter += string.Format(@" AND Egp_CheckedDate BETWEEN '{0}' AND '{1}'", dtpC1From.Date.ToString("MM/dd/yyyy")
                                                                                       , dtpC1To.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC1From.IsNull)
            {
                filter += string.Format(@" AND Egp_CheckedDate >= '{0}'", dtpC1From.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC1To.IsNull)
            {
                filter += string.Format(@" AND Egp_CheckedDate <= '{0}'", dtpC1To.Date.ToString("MM/dd/yyyy"));
            }
            //Checked Date 2
            if (!dtpC2From.IsNull && !dtpC2To.IsNull)
            {
                filter += string.Format(@" AND Egp_Checked2Date BETWEEN '{0}' AND '{1}'", dtpC2From.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpC2To.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC2From.IsNull)
            {
                filter += string.Format(@" AND Egp_Checked2Date >= '{0}'", dtpC2From.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpC2To.IsNull)
            {
                filter += string.Format(@" AND Egp_Checked2Date <= '{0}'", dtpC2To.Date.ToString("MM/dd/yyyy"));
            }
            //Approved Date
            if (!dtpAPFrom.IsNull && !dtpAPTo.IsNull)
            {
                filter += string.Format(@" AND Egp_ApprovedDate BETWEEN '{0}' AND '{1}'", dtpAPFrom.Date.ToString("MM/dd/yyyy")
                                                                                        , dtpAPTo.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAPFrom.IsNull)
            {
                filter += string.Format(@" AND Egp_ApprovedDate >= '{0}'", dtpAPFrom.Date.ToString("MM/dd/yyyy"));
            }
            else if (!dtpAPTo.IsNull)
            {
                filter += string.Format(@" AND Egp_ApprovedDate <= '{0}'", dtpAPTo.Date.ToString("MM/dd/yyyy"));
            }
            #endregion

            if (!txtSearch.Text.Trim().Equals(string.Empty))
            {
                searchFilter = @"AND ( ( Egp_ControlNo LIKE '{0}%' )
                                    OR ( Egp_EmployeeId LIKE '%{0}%' )
                                    OR ( Emt_NickName LIKE '%{0}%' )
                                    OR ( Emt_Lastname LIKE '%{0}%' )
                                    OR ( Emt_Firstname LIKE '%{0}%' )
                                    OR ( Convert(varchar(10),Egp_GatePassDate,101) LIKE '%{0}%' )
                                    OR ( dbo.getCostCenterFullNameV2(Egp_CostCenter) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Egp_AppliedDate,101) 
                                        + ' ' 
                                        + RIGHT(Convert(varchar(17), Egp_AppliedDate,113),5) LIKE '%{0}%' )
                                    OR ( Convert(varchar(10), Egp_EndorsedDateToChecker,101) 
                                        + ' ' 
                                        + RIGHT(Convert(varchar(17), Egp_EndorsedDateToChecker,113),5) LIKE '%{0}%' )
                                    OR ( Egp_ApplicationTypeRemarks LIKE '{0}%' )
                                    OR ( Trm_Remarks LIKE '{0}%' )
                                    )";
            }
       
        

        string holder = string.Empty;
        string searchKey = txtSearch.Text.Replace("'", "");
        searchKey += "&";
        for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
        {
            holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
            searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

            filter += string.Format(searchFilter, holder);
        }

        return filter;
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
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "GATEPASS REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    ExportExcelHelper.ExportControl2(ctrl, "GatePass Report");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }

    private string initializeExcelHeader()
    {
        string criteria = string.Empty;
        if (!txtEmployee.Text.Trim().Equals(string.Empty))
        {
            criteria += lblEmployee.Text + ":" + txtEmployee.Text.Trim() + "-";
        }
        if (!txtCostcenter.Text.Trim().Equals(string.Empty))
        {
            criteria += lblCostcenter.Text + ":" + txtCostcenter.Text.Trim() + "-";
        }
        if (!txtStatus.Text.Trim().Equals(string.Empty))
        {
            criteria += lblStatus.Text + ":" + txtStatus.Text.Trim() + "-";
        }
        if (!txtPayPeriod.Text.Trim().Equals(string.Empty))
        {
            criteria += lblPayPeriod.Text + ":" + txtPayPeriod.Text.Trim() + "-";
        }
        if (!txtChecker1.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker1.Text + ":" + txtChecker1.Text.Trim() + "-";
        }
        if (!txtChecker2.Text.Trim().Equals(string.Empty))
        {
            criteria += lblChecker2.Text + ":" + txtChecker2.Text.Trim() + "-";
        }
        if (!txtApprover.Text.Trim().Equals(string.Empty))
        {
            criteria += lblApprover.Text + ":" + txtApprover.Text.Trim() + "-";
        }
        if (!dtpGatePassFrom.IsNull)
        {
            criteria += lblGatePassDate.Text + ":" + dtpGatePassFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpGatePassTo.IsNull)
        {
            criteria += lblGatePassDateTo.Text + ":" + dtpGatePassTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedFrom.IsNull)
        {
            criteria += lblAppliedFrom.Text + ":" + dtpAppliedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpAppliedTo.IsNull)
        {
            criteria += lblAppliedTo.Text + ":" + dtpAppliedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedFrom.IsNull)
        {
            criteria += lblEndorsedFrom.Text + ":" + dtpEndorsedFrom.Date.ToString("MM/dd/yyyy") + "-";
        }
        if (!dtpEndorsedTo.IsNull)
        {
            criteria += lblEndorsedTo.Text + ":" + dtpEndorsedTo.Date.ToString("MM/dd/yyyy") + "-";
        }
        return criteria.Trim();
    }
    protected void btnPrint_Click1(object sender, EventArgs e)
    {
        if (dgvResult.Rows.Count > 0)
        {
            DataSet ds = new DataSet();
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(SQLBuilder("--").Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
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
                #region Remove Columns
                if (methods.GetProcessControlFlag("PERSONNEL", "VWNICKNAME"))
                {
                    if (methods.GetProcessControlFlag("PERSONNEL", "DSPIDCODE"))
                    {
                        ds.Tables[0].Columns.Remove("Nickname");
                    }
                    else
                    {
                        ds.Tables[0].Columns.Remove("ID Code");
                    }
                }
                else
                {
                    ds.Tables[0].Columns.Remove("ID Code");
                    ds.Tables[0].Columns.Remove("Nickname");
                }

                if (!methods.GetProcessControlFlag("GENERAL", "DSPFULLNM"))
                {
                    ds.Tables[0].Columns.Remove("Lastname");
                    ds.Tables[0].Columns.Remove("Firstname");
                    ds.Tables[0].Columns.Remove("MI");
                }
                //Includes
                for (int i = 0; i < cblInclude.Items.Count; i++)
                {
                    if (!cblInclude.Items[i].Selected)
                        ds.Tables[0].Columns.Remove(cblInclude.Items[i].Value);
                }
                #endregion
                try
                {
                    GridView tempGridView = new GridView();
                    tempGridView.RowCreated += new GridViewRowEventHandler(dgvResult_RowCreated);
                    tempGridView.DataSource = ds;
                    tempGridView.DataBind();
                    Control[] ctrl = new Control[2];
                    ctrl[0] = CommonLookUp.GetHeaderPanelOption(ds.Tables[0].Columns.Count, ds.Tables[0].Rows.Count, "GATEPASS REPORT", initializeExcelHeader());
                    ctrl[1] = tempGridView;
                    Session["ctrl"] = ctrl;
                    ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No records found.");
            }
        }
        else
        {
            MessageBox.Show("No records found.");
        }
    }

    private void initializeControls()
    {
        hfNumRows.Value = Resources.Resource.REPORTSPAGEITEMS;
        DataRow dr = CommonMethods.getUserGrant(Session["userLogged"].ToString(), "WFGATEPASSREP");
        if (dr != null)
        {
            btnExport.Enabled = Convert.ToBoolean(dr["Ugt_CanGenerate"]);

            btnPrint.Enabled = Convert.ToBoolean(dr["Ugt_CanPrint"]);

            txtEmployee.Text = Session["userLogged"].ToString();
            tbrEmployee.Visible = (Convert.ToBoolean(dr["Ugt_CanCheck"]) || Convert.ToBoolean(dr["Ugt_CanApprove"]));
        }
        else
        {
            btnExport.Enabled = false;
            btnPrint.Enabled = false;
        }
    }
}