using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using MethodsLibrary;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using System.IO; //New
public partial class Transactions_CustomForms_GatePass : System.Web.UI.Page{
    //=========================================START
    Object OMissing = System.Reflection.Missing.Value;
    Application Word = new Application();
    Document WordDoc = new Document();
    //=========================================END

    CommonMethods cm = new CommonMethods();
    protected void Page_Load(object sender, EventArgs e){
        GMDatePickerDate.Attributes.Add("ReadOnly","true");
        txtEmployeeId.Attributes.Add("ReadOnly","true");
        txtEmployeeName.Attributes.Add("ReadOnly","true");
        txtNickname.Attributes.Add("ReadOnly","true");

        txtRemarks.Enabled = false;
        btnEndorseToChecker1.Enabled = false;

        LoadComplete += new EventHandler(Transactions_CustomForms_GatePass_LoadComplete);
        initializeontrols();
    }

    protected void btnSaveGatePass_click(object sender, EventArgs e){
        if (Page.IsValid){
            MessageBox.Show("New transaction saved.");

            using (DALHelper dal = new DALHelper()){
                try{
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();

                    string costCenterCode = getCostCenterCode(dal);
                    string currentPayPeriod = getPayPeriod(dal);
                    string controlNumber = CommonMethods.GetControlNumber("GATEPASS" , dal);
                    hfControlNo.Value = controlNumber;
                    dal.ExecuteNonQuery(string.Format(@"INSERT INTO E_EmployeeGatePass(
                            Egp_ControlNo, 
                            Egp_CurrentPayPeriod, 
                            Egp_EmployeeId, 
                            Egp_GatePassDate, 
                            Egp_AppliedDate, 
                            Egp_ApplicationType, 
                            Egp_ApplicationTypeRemarks, 
                            Egp_Vehicle, 
                            Egp_Attachment, 
                            Egp_EndorsedDateToChecker, 
                            Egp_CheckedBy, 
                            Egp_CheckedDate, 
                            Egp_Checked2By, 
                            Egp_Checked2Date, 
                            Egp_ApprovedBy, 
                            Egp_ApprovedDate, 
                            Egp_CostCenter, 
                            Egp_Status, 
                            Usr_Login, 
                            Ludatetime)

                        VALUES('{0}','{1}','{2}','{3}',GETDATE(),'{4}','{5}','{6}','{7}',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'{8}','{9}','{10}',GETDATE())"
                            , controlNumber
                            , currentPayPeriod
                            , txtEmployeeId.Text.ToString().Trim().ToUpper()
                            , GMDatePickerDate.Date.ToString("MM/dd/yyyy")
                            , rblTypeOfApplication.Text.ToString().Trim().ToUpper()
                            , txtTypeRemarks.Text.ToString().Trim().ToUpper()
                            , txtVehicle.Text.ToString().Trim().ToUpper()
                            , txtAttachment.ToString().Trim().ToUpper()
                            , costCenterCode
                            , "1" //Status=NEW
                            , Session["userLogged"].ToString()
                    ));
                    string selectedValue = rblTypeOfApplication.SelectedValue;
                    
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex){
                    dal.RollBackTransactionSnapshot();
                }
                finally{
                    dal.CloseDB();
                }
            }
        }
        txtStatus.Text = "NEW";
        enableButtons();

        //=============START=====================
        string sampleTemplate = @"C:\Demo\Gate Pass.docx";
        string[] arrData = { "[GP_DATE]"
                            ,"[GP_NAME]"
                            ,"[GP_DEPT]"
                            ,"[lbl_OB]"
                            ,"[lbl_UT]"
                            ,"[lbl_OTH]"};

        string[] arrValues = {GMDatePickerDate.Date.ToString()
                             ,txtEmployeeName.Text.ToString().Trim().ToUpper()
                             ,txtNickname.Text.ToString().Trim().ToUpper()
                             ,txtVehicle.Text.ToString().Trim().ToUpper()
                             ,txtAttachment.Text.ToString().Trim().ToUpper()
                             ,txtTypeRemarks.Text.ToString().Trim().ToUpper()};

        for(int i =0; i < arrData.Length; i++)
        {
            WordFile_FindToReplace(sampleTemplate, arrData[i].ToString(), arrValues[i].ToString(), i);
        }
    }
    private static void WordFile_FindToReplace(string template, string searchText, string replaceText, int index)
    {
        object filename = template;
        object tempFileName;
        object saveAsFileName = @"C:\Demo\Gate Pass_DOCFile.docx";
        object findtext = searchText;
        object replaceTextWith = replaceText; 
        object start = 0;        
        object end = 10;
        object missing = Type.Missing; 
        Application app = new Application();
        if (index == 0)
        {
            tempFileName = filename;
        }
        else
            tempFileName = saveAsFileName;

        Document doc = app.Documents.Open(ref tempFileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        Range range = app.ActiveDocument.Content;
        //var range = doc.Range(ref start, ref end);

        object FindText = Type.Missing;
        range.Find.Execute(ref findtext, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceTextWith, ref missing, ref missing, ref missing, ref missing, ref missing);//FindText: searchText, Replace: WdReplace.wdReplaceAll, ReplaceWith: replaceText

        //var shapes = doc.Shapes;
        //foreach (Microsoft.Office.Interop.Word.Shape shape in shapes)
        //{
        //    var initialText = shape.TextFrame.TextRange.Text;
        //    var resultingText = initialText.Replace(searchText, replaceText);
        //    shape.TextFrame.TextRange.Text = resultingText;
        //}

        //doc.Save();
        //doc.Activate();
        //app.Selection.Font.Size = 28;
        //app.Selection.Font.Bold = 1;
        //app.Selection.TypeText("This is the text");
        //app.Selection.TypeParagraph();
        //app.Selection.Font.Size = 14;
        //app.Selection.Font.Bold = 0;
        //app.Selection.TypeText("This is thfdsfasfse text");
        //app.Selection.TypeParagraph();
        doc.SaveAs(ref saveAsFileName
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing
                 , ref missing);//@"C:\Demo\DummyTemplate_DOC.docx", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault);
        //doc.Close(ref missing,ref missing,ref missing);
        app.Quit(ref missing, ref missing, ref missing);
        Marshal.ReleaseComObject(app);
    }
    protected void rblTypeOfApplication_SelectedIndexChanged(object sender, EventArgs e){
        
    }
    protected void btnClear_Click(object sender, EventArgs e){
        //Response.Redirect(Request.RawUrl);
        if (Page.IsValid){
            using (DALHelper dal = new DALHelper()){
            try{
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
           
                    switch (btnClear.Text.ToString().Trim().ToUpper()){
                        case "CLEAR":
                            clearValues();
                            break;

                        case "CANCEL":
                            cancelTrans(dal);
                            clearValues();
                            enableButtons();
                            break;

                        case "DISAPPROVE":
                            break;

                        default:
                            break;
                    }
                dal.CommitTransactionSnapshot();
                }
                catch (Exception ex){
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                    dal.RollBackTransactionSnapshot();
                }
                finally{
                    dal.CloseDB();
                }
            }
         }
    }
    protected void btnEndorseToChecker1_Click(object sender, EventArgs e){
        string status = getStatusRoute();

        if (status == "")
        {
            MessageBox.Show("No route defined for user.");
        }
        else {
            //=======================================================================
            if (Page.IsValid)
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();

                        //string costCenterCode = getCostCenterCode(dal);
                        //string currentPayPeriod = getPayPeriod(dal);
                        //string controlNumber = CommonMethods.GetControlNumber("GATEPASS", dal);
                        //hfControlNo.Value = controlNumber;
                        string query = string.Format(@"DECLARE @EndorseStatus CHAR(1) = '{0}'  --- TRANSACTION STATUS
                                                            DECLARE @RecordStatus CHAR(1) 
                                                            DECLARE @ControlNo AS VARCHAR(12) = '{1}' --- TRANSACTION CONTROL NUMBER
                                                            DECLARE @EmailNotificationType AS VARCHAR(25) = 'GATEPASS'
                                                            DECLARE @ENDORSETOCHECKER1_STATUS AS CHAR(1) = '3'
                                                            DECLARE @ENDORSETOCHECKER2_STATUS AS CHAR(1) = '5'
                                                            DECLARE @ENDORSETOAPPROVER_STATUS AS CHAR(1) = '7'
                                                            DECLARE @CreateEmailNotification BIT = '{2}' ---  Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION)
                                                            DECLARE @CheckedBy VARCHAR(15) = '{3}' ---- USER LOGGED IN

                                                            IF OBJECT_ID('E_EmployeeGatePass') IS NOT NULL
                                                              BEGIN
                                                               BEGIN
                                                                IF NOT EXISTS (SELECT Egp_ControlNo 
                                                                    FROM E_EmployeeGatePass
                                                                    WHERE Egp_ControlNo = @ControlNo)
                                                                 THROW 51000,'Transaction does not exist',1;

                                                                SELECT @RecordStatus = Egp_Status
                                                                FROM E_EmployeeGatePass
                                                                WHERE Egp_ControlNo = @ControlNo;

                                                                IF @RecordStatus = @EndorseStatus
                                                                    THROW 52000,'Transaction already endorsed',1;

                                                                IF @RecordStatus IN ('4','6','8')
                                                                    THROW 56000,'Transaction already disapproved',1;

                                                                IF @RecordStatus IN ('0','2')
                                                                    THROW 56000,'Transaction already cancelled',1;

                                                                IF @RecordStatus IN ('9')
                                                                    THROW 56000,'Transaction already approved',1;

                                                                IF @RecordStatus IN ('7')
                                                                    THROW 56000,'Transaction cannot be endorsed',1;
   
    
                                                                BEGIN
                                                                 IF @EndorseStatus = @ENDORSETOCHECKER2_STATUS
                                                                 BEGIN
                                                                  IF @RecordStatus IN ('1','3')
                                                                  BEGIN
                                                                   UPDATE E_EmployeeGatePass
                                                                    SET Egp_EndorsedDateToChecker = CASE WHEN @RecordStatus = '1' THEN GETDATE() ELSE Egp_EndorsedDateToChecker END
                                                                    , Egp_CheckedBy = CASE WHEN @RecordStatus = '1' THEN NULL ELSE @CheckedBy END
                                                                    , Egp_CheckedDate = CASE WHEN @RecordStatus = '1' THEN NULL ELSE GETDATE() END
                                                                    , Egp_Status = @ENDORSETOCHECKER2_STATUS
                                                                   WHERE Egp_ControlNo = @ControlNo;
                                                                  END
                                                                  ELSE
                                                                   THROW 56000,'Transaction cannot be endorsed to Checker 2',1;
                                                                 END
                                                                 ELSE IF @EndorseStatus = @ENDORSETOAPPROVER_STATUS
                                                                 BEGIN
                                                                  IF @RecordStatus IN ('1','3','5')
                                                                  BEGIN
                                                                   UPDATE E_EmployeeGatePass
                                                                    SET Egp_EndorsedDateToChecker = CASE WHEN @RecordStatus = '1' THEN GETDATE() ELSE Egp_EndorsedDateToChecker END
                                                                    , Egp_CheckedBy = CASE WHEN @RecordStatus = '3' THEN @CheckedBy ELSE Egp_CheckedBy END
                                                                    , Egp_CheckedDate = CASE WHEN @RecordStatus = '3' THEN GETDATE() ELSE Egp_CheckedDate END
                                                                    , Egp_Checked2By = CASE WHEN @RecordStatus = '5' THEN @CheckedBy ELSE NULL END
                                                                    , Egp_Checked2Date = CASE WHEN @RecordStatus = '5' THEN GETDATE() ELSE NULL END
                                                                    , Egp_Status = @ENDORSETOAPPROVER_STATUS
                                                                   WHERE Egp_ControlNo = @ControlNo;
                                                                  END
                                                                  ELSE
                                                                   THROW 56000,'Transaction cannot be endorsed to Approver',1;
                                                                 END
                                                                 ELSE IF @EndorseStatus = @ENDORSETOCHECKER1_STATUS
                                                                 BEGIN
                                                                  IF @RecordStatus IN ('1')
                                                                  BEGIN
                                                                   UPDATE E_EmployeeGatePass
                                                                    SET Egp_EndorsedDateToChecker = GETDATE()
                                                                    , Egp_Status = @ENDORSETOCHECKER1_STATUS
                                                                   WHERE Egp_ControlNo = @ControlNo;
                                                                  END
                                                                  ELSE
                                                                   THROW 56000,'Transaction cannot be endorsed to Checker 1',1;
                                                                 END
                                                                END
    
                                                               END
                                                              END

                                                              IF @CreateEmailNotification = 1 AND @EmailNotificationType <> ''
                                                              BEGIN
                                                               UPDATE T_EmailNotification
                                                                        SET Ent_Status = 'X'
                                                                        WHERE Ent_ControlNo = @ControlNo
                                                                            AND Ent_Status = 'A'

                                                               INSERT INTO T_EmailNotification
                                                                (Ent_ControlNo
                                                                ,Ent_SeqNo
                                                                ,Ent_TransactionType
                                                                ,Ent_Action
                                                                ,Ent_Status
                                                                ,Usr_Login
                                                                ,Ludatetime)
                                                               VALUES
                                                                (@ControlNo
                                                                ,ISNULL((SELECT RIGHT('000' + CONVERT(VARCHAR, MAX(CONVERT(INT, Ent_SeqNo)) + 1), 3) FROM T_EmailNotification WHERE Ent_ControlNo = @ControlNo), '001')
                                                                ,@EmailNotificationType
                                                                ,'ENDORSE'
                                                                ,'A'
                                                                ,@CheckedBy
                                                                ,GETDATE())
                                                              END"
                        , status, hfControlNo.Value, Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION), Session["userLogged"].ToString());
                        dal.ExecuteNonQuery(query);

                        //string selectedValue = rblTypeOfApplication.SelectedValue;

                        dal.CommitTransactionSnapshot();

                        MessageBox.Show("Successfully endorsed transaction.");
                    }
                    catch (Exception ex)
                    {
                        dal.RollBackTransactionSnapshot();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }
            //=======================================================================
            clearValues();
            enableButtons();
        }
        
    }
    protected void txtEmployeeName_TextChanged(object sender, EventArgs e){

    }
    private string getCostCenterCode(DALHelper dal){
        string query = string.Format(@"SELECT Emt_CostCenterCode, dbo.getCostCenterFullNameV2(Emt_CostCenterCode)
                                      FROM t_employeemaster
                                      WHERE Emt_EmployeeID = '{0}'", txtEmployeeId.Text.ToString().Trim());
        System.Data.DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
        return dtResult.Rows.Count > 0 ? dtResult.Rows[0]["Emt_CostCenterCode"].ToString() : "";
    }
    private string getPayPeriod(DALHelper dal){
        string query = @"SELECT Ppm_PayPeriod, Ppm_StartCycle, Ppm_EndCycle
                        FROM T_PayPeriodMaster
                        WHERE Ppm_CycleIndicator = 'C'";
        System.Data.DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
        return dtResult.Rows.Count > 0 ? dtResult.Rows[0]["Ppm_PayPeriod"].ToString() : "";
    }
    //===================START=====================================================================================
            //private static void WordFile_FindToReplace(string template, string searchText, string replaceText)
            //{
            //    var app = new Application();
            //    var doc = app.Documents.Open(template);
            //    var range = doc.Range();
            //    var shapes = doc.Shapes;

            //    range.Find.Execute(FindText: "searchText", Replace: WdReplace.wdReplaceAll, ReplaceWith: replaceText);

            //    foreach (Microsoft.Office.Interop.Word.Shape shape in shapes)
            //    {
            //        var initialText = shape.TextFrame.TextRange.Text;
            //        var resultingText = initialText.Replace(searchText, replaceText);
            //        shape.TextFrame.TextRange.Text = resultingText;

            //    }

            //    doc.Save();
            //    doc.SaveAs(@"C:\Demo\testTemplateOuput.docx");
            //    doc.Close();

            //    Marshal.ReleaseComObject(app);
            //}
    //================END========================================================================================
    private void enableButtons(){
        switch (txtStatus.Text.ToString().Trim().ToUpper()){
            case "":
                btnSaveGatePass.Enabled = true;
                btnClear.Enabled = true;
                btnEndorseToChecker1.Enabled = false;

                btnSaveGatePass.Text = "SAVE";
                btnClear.Text = "CLEAR";
                btnEndorseToChecker1.Text = "ENDORSE TO CHECKER 1";
                break;
            case "NEW":
                btnSaveGatePass.Enabled = true;
                btnClear.Enabled = true;
                btnEndorseToChecker1.Enabled = true;

                btnSaveGatePass.Text = "SAVE";
                btnClear.Text = "CANCEL";
                btnEndorseToChecker1.Text = "ENDORSE TO CHECKER 1";
                break;
            case "ENDORSED TO CHECKER 1":
                btnSaveGatePass.Enabled = true;
                btnClear.Enabled = true;
                btnEndorseToChecker1.Enabled = true;

                btnSaveGatePass.Text = "RETURN TO EMPLOYEE";
                btnClear.Text = "DISAPPROVE";
                btnEndorseToChecker1.Text = "ENDORSE TO CHECKER 2";
                break;
            case "ENDORSED TO CHECKER 2":
                btnSaveGatePass.Enabled = true;
                btnClear.Enabled = true;
                btnEndorseToChecker1.Enabled = true;

                btnSaveGatePass.Text = "RETURN TO EMPLOYEE";
                btnClear.Text = "DISAPPROVE";
                btnEndorseToChecker1.Text = "ENDORSE TO APPROVER";
                break;
            case "ENDORSED TO APPROVER":
                btnSaveGatePass.Enabled = true;
                btnClear.Enabled = true;
                btnEndorseToChecker1.Enabled = true;

                btnSaveGatePass.Text = "RETURN TO EMPLOYEE";
                btnClear.Text = "DISAPPROVE";
                btnEndorseToChecker1.Text = "APPROVE";
                break;
            default:
                break;
        }
    }
    private void initializeontrols(){
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "PERSONNEL", "WFGATEPASS");
        btnEmployeeId.Enabled = userGrant.canAdd();
        enableButtons();
    }
    private void clearValues(){
        GMDatePickerDate.Reset();
        txtEmployeeId.Text = string.Empty;
        txtEmployeeName.Text = string.Empty;
        txtNickname.Text = string.Empty;
        rblTypeOfApplication.ClearSelection();
        txtTypeRemarks.Text = string.Empty;
        txtVehicle.Text = string.Empty;
        txtAttachment.Text = string.Empty;
        txtRemarks.Text = string.Empty;
        txtStatus.Text = string.Empty;
    }
    private void cancelTrans(DALHelper dal){
        string query = string.Format(@"UPDATE E_EmployeeGatePass
                                        SET Egp_Status = '2'
						                , Usr_Login = '{0}'
						                , Ludatetime = GETDATE()
                                        WHERE Egp_ControlNo = '{1}'"
                              , Session["userLogged"].ToString(), hfControlNo.Value);
        MessageBox.Show("Transaction cancelled");
        dal.ExecuteNonQuery(query);
    }
    private void endorse(string status)
    {
        string query = string.Format(@"UPDATE E_EmployeeGatePass
                                        SET Egp_Status = '{2}'
						                , Usr_Login = '{0}'
						                , Ludatetime = GETDATE()
                                        WHERE Egp_ControlNo = '{1}'"
                              , Session["userLogged"].ToString(), hfControlNo.Value, status);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(query);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
    }
    public string getStatusRoute()
    {
        System.Data.DataTable dtResult = new System.Data.DataTable();
        string status = string.Empty;
        string sql = string.Format(@"SELECT a.Arm_Checker1 [C1]
                                , a.Arm_Checker2 [C2]
                                , a.Arm_Approver [AP]
                             FROM T_EmployeeApprovalRoute AS e
                             LEFT JOIN T_ApprovalRouteMaster AS a 
                               ON a.Arm_RouteId = e.Arm_RouteId
                            WHERE e.Arm_EmployeeId = '{0}'
                              AND e.Arm_TransactionId = 'GATEPASS'
                              AND Convert(varchar,GETDATE(),101) BETWEEN e.Arm_StartDate AND ISNULL(e.Arm_EndDate, GETDATE())",txtEmployeeId.Text);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(sql).Tables[0];
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (dtResult.Rows.Count > 0)
        {
            string C1 = dtResult.Rows[0]["C1"].ToString();
            string C2 = dtResult.Rows[0]["C2"].ToString();
            string AP = dtResult.Rows[0]["AP"].ToString();

            if (C1 == C2 && C2 == AP)
            {
                status = "7"; //Approver
            }
            else if (C1 == C2)
            {
                status = "5"; //Checker 2
            }
            else
            {
                status = "3"; //Checker 1
            }
        }
        return status;
    }

    void Transactions_CustomForms_GatePass_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "gatepassscript";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
    }

    protected void btnExportPDF_Click(object sender, EventArgs e)
    {
        convertWordToPDF();
    }

    public void convertWordToPDF()
    {
        object missing = Type.Missing;
        object saveAsFileName = @"C:\Demo\DummyTemplate_Output.docx";
        int start = 1;
        int end = 1;
        Application app = new Application();
        Document doc = app.Documents.Open(ref saveAsFileName
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing
                                        , ref missing);
        doc.ExportAsFixedFormat(@"C:\Demo\DummyTemplate_DOC.pdf",WdExportFormat.wdExportFormatPDF,false,WdExportOptimizeFor.wdExportOptimizeForPrint,WdExportRange.wdExportAllDocument,start,end,WdExportItem.wdExportDocumentContent,false,false,WdExportCreateBookmarks.wdExportCreateHeadingBookmarks,false,false,false,ref missing);
        //Converter.Convert("C:\Temp\DocumentA.doc", @"C:\Temp\DocumentA.pdf");

        doc.Close(ref missing, ref missing, ref missing);
        MessageBox.Show("Successfully EXPORTED PDF!");
        app.Quit(ref missing, ref missing, ref missing);
        Marshal.ReleaseComObject(WordDoc);
        Marshal.ReleaseComObject(app);
        File.Delete(@"C:\Demo\DummyTemplate_Output.docx");
    }
}
