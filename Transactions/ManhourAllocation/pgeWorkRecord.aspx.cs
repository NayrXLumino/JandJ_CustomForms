using System;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using MethodsLibrary;

public partial class _Default : System.Web.UI.Page
{
    #region [Class Variables]
    private MenuGrant MGBL = new MenuGrant();
    private DataSet dsUser = new DataSet();
    private DataSet dsShift = new DataSet();
    private DataSet dsLogLedger = new DataSet();
    private DataSet dsSplit = new DataSet(); 
    private DataSet dsJobs = new DataSet();
    private string strShift = string.Empty;
    private string DayCode = string.Empty;
    private JobSplitBL JSBL = new JobSplitBL(); 
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFJOBSPLTENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            LoadComplete += new EventHandler(_Default_LoadComplete);
            hiddenType.Value = "entry";
            if (!Page.IsPostBack)
            {
                InitializeUser();
                InitializeValues();
                InitializeControls();
                InitializeUnsplittedHours();
                hasJobSplit();
                if (hiddenStatus.Value == "9" && (Session["flagCheck"] == null || Session["flagCheck"].ToString().Equals(string.Empty)))
                {
                    Session["flagCheck"] = "JSSUBMITTEDTODAY";
                }
                else if (JSBL.CheckBillingLock(txtDate.Text))
                {
                    hiddenDFlag.Value = "X";
                    Session["flagCheck"] = "JSLOCKED";
                }
                hfMaxHours.Value = Methods.GetParameterValue("MAXOTHR").ToString();

                //hfMinHours.Value = CommonMethods.getMINOTHR(txtEmployeeId.Text);
                hfMinHours.Value = "0";
            }

            showMessages();
        }
    }

    #region [Initialize Methods]
    #region LoadComplete
    protected void _Default_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "manhourscripts";
        string jsurl = "_manhour.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");
        btnJob.OnClientClick = "javascript:return lookupJSJobCode()";
        txtJHours.Attributes.Add("OnKeyUp", "javascript:autoEndtime();");
        txtJStart.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJEnd.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJHours.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        //txtJStart.Attributes.Add("OnKeyUp", "javascript:formatCompute();");
        txtJStart.Attributes.Add("OnKeyUp", "javascript:autoEndtime();");
        txtJEnd.Attributes.Add("OnKeyUp", "javascript:formatCompute();");
        btnJob.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnSubWork.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJJobCode.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJClientNo.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJClientName.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtSubWork.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnSubWork.Attributes.Add("OnClick", "javascript:return lookupJSSubWork();");
        txtJHours.Attributes.Add("OnBlur", "javascript:formatCompute();");
        //txtJHours.Attributes.Add("OnKeyPress", "javascript:return isNumberKeyEx(event);");
        //listBox.Attributes.Add("ondblClick", "javascript:selection();");
        chkOvertime.Attributes.Add("OnClick", "javascript:setValueCheck(event,this);");
        rblBillable.Attributes.Add("OnClick", "javascript:setValueRadio(event,this);");
    }
    #endregion

    #region Initialize dsUser for the current user
    protected void InitializeUser()
    {
        string userId = Session["userLogged"].ToString();
        
        FillUserDS(userId);

        lblUserIdCode.Text = Resources.Resource._3RDINFO;

        if (!isInMaster(userId))
        {
            txtEmployeeId.Text = dsUser.Tables[0].Rows[0]["emt_employeeid"].ToString();
            txtEmployeeName.Text = dsUser.Tables[0].Rows[0]["emt_lastname"].ToString() + ", " + dsUser.Tables[0].Rows[0]["emt_firstname"].ToString();
            txtNickname.Text = dsUser.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
            hiddenShift.Value = dsUser.Tables[0].Rows[0]["emt_shiftcode"].ToString();
            hfEmployeeCostCenter.Value = dsUser.Tables[0].Rows[0]["Emt_CostCenterCode"].ToString();
            GetShifts();
        }

    }
    #endregion

    #region Initialize dsShift for the current user
    protected void GetShifts()
    {
        if (!isInMaster(txtEmployeeId.Text))
        {
            string shiftCode = hiddenShift.Value;
            string[] array = new string[2];
            array[0] = DateTime.Now.Date.ToString("MM/dd/yyyy");
            array[1] = txtEmployeeId.Text;
            dsShift.Clear();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                try
                {
                    string sqlGetShift = @" if exists((	select Ell_ShiftCode from T_EmployeeLogLedger  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'
				                                            union
			                                            select Ell_ShiftCode from T_EmployeeLogLedgerHist  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'))
                                             begin
                                             select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,
                                                        --Leave hours in 1st half  
                                                          convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                          ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                        --Leave hours in 2nd half
                                                          convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                             (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                               from T_ShiftCodeMaster where  Scm_ShiftCode=
                                                 (select Ell_ShiftCode from T_EmployeeLogLedger  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}'
                                                  union
                                                  select Ell_ShiftCode from T_EmployeeLogLedgerHist  where convert(char(10),Ell_ProcessDate,101)='{0}' and Ell_EmployeeId='{1}')                                                  
                                             end
                                           else
                                             begin 
                                             select Scm_ShiftCode,Scm_ShiftTimeIn,Scm_ShiftBreakStart,Scm_ShiftBreakEnd,Scm_ShiftTimeOut,Scm_ShiftHours,
                                                        --Leave hours in 1st half  
                                                          convert(decimal(18,2),((((case when Scm_ScheduleType='G' then convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2))+ 24 else convert(decimal(18,2),substring(Scm_ShiftBreakStart,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakStart,3,2)))-
                                                          ((convert(decimal(18,2),substring(Scm_ShiftTimeIn,1,2)))*60 + convert(decimal(18,2),substring(Scm_ShiftTimeIn,3,2))))/60) as firsthalf,
                                                        --Leave hours in 2nd half
                                                          convert(decimal(18,2),(((((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftTimeOut,1,2)) end) *60) + convert(decimal(18,2),substring(Scm_ShiftTimeOut,3,2)))-
                                                             (((case when Scm_ScheduleType='G' then (convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2))+ 24) else convert(decimal(18,2),substring(Scm_ShiftBreakEnd,1,2)) end)*60) + convert(decimal(18,2),substring(Scm_ShiftBreakEnd,3,2)))) + convert(int,scm_paidbreak))/60) as secondhalf
                                               from T_ShiftCodeMaster where  Scm_ShiftCode=(select emt_shiftcode from t_employeemaster where emt_employeeid='{1}')
                                             end";

                    string sqlDayCode = @"   select 
                                                              ell_daycode
                                                            , ell_encodedpayleavetype
                                                            , ell_encodedpayleavehr
                                                            , ell_encodednopayleavetype
                                                            , ell_encodednopayleavehr
                                                            , ell_actualtimein_1
                                                            , ell_actualtimeout_1
                                                            , ell_actualtimein_2
                                                            , ell_actualtimeout_2

                                                            from t_employeelogledger
                                                            
                                                            where ell_employeeid  = '{1}'
                                                            and ell_processdate ='{0}'

                                                            union

                                                            select 
                                                              ell_daycode
                                                            , ell_encodedpayleavetype
                                                            , ell_encodedpayleavehr
                                                            , ell_encodednopayleavetype
                                                            , ell_encodednopayleavehr 
                                                            , ell_actualtimein_1
                                                            , ell_actualtimeout_1
                                                            , ell_actualtimein_2
                                                            , ell_actualtimeout_2

                                                            from t_employeelogledgerhist

                                                            where ell_employeeid  = '{1}'
                                                            and ell_processdate = '{0}'
                                                        ";
                    dsLogLedger.Clear();
                    dsShift.Clear();
                    dsShift = dal.ExecuteDataSet(string.Format(sqlGetShift, array), CommandType.Text);
                    dsLogLedger = dal.ExecuteDataSet(string.Format(sqlDayCode, array), CommandType.Text);
                    DayCode = dsLogLedger.Tables[0].Rows[0]["ell_DayCode"].ToString();
                    
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message+": Cycle opening might not be done yet. \n Please contact the HR or IT Department.");
                }
                finally
                {
                    dal.CloseDB();
                }

            }
            
            try
            {
                string startTime = Convert.ToString(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeIn"]);
                string endTime = Convert.ToString(dsShift.Tables[0].Rows[0]["Scm_ShiftTimeOut"]);
                string breakStart = Convert.ToString(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakStart"]);
                string breakEnd = Convert.ToString(dsShift.Tables[0].Rows[0]["Scm_ShiftBreakEnd"]);

                break_Start.Value = breakStart;
                break_End.Value = breakEnd;
                //Do not change any format here especially trimmming spaces...
                strShift = string.Empty;
                strShift += " [";
                strShift += Convert.ToString(dsShift.Tables[0].Rows[0]["Scm_ShiftCode"]);
                strShift += "] ";
                strShift += startTime.Substring(0, 2) + ":" + startTime.Substring(2, 2);
                strShift += " - ";
                strShift += breakStart.Substring(0, 2) + ":" + breakStart.Substring(2, 2);
                strShift += "  ";
                strShift += breakEnd.Substring(0, 2) + ":" + breakEnd.Substring(2, 2);
                strShift += " - ";
                strShift += endTime.Substring(0, 2) + ":" + endTime.Substring(2, 2);
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show("No shift retrieved.");
            }
        }
    }
    #endregion

    #region Filling of the DataSet dsUser to be used in User Info
    protected void FillUserDS(string userId)
    {
        dsUser.Clear();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            try
            {
                string sqlGetUser = @"  SELECT
                                            Emt_EmployeeID,
                                            Emt_LastName,
                                            Emt_FirstName,
                                            Emt_MiddleName,
                                            Emt_Shiftcode,
                                            Emt_Picture,
                                            Emt_NickName [Nickname],
                                            --Emt_OldEmployeeID,
                                            Emt_CostCenterCode,
                                            dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter],
                                            ISNULL(Dcm_Departmentdesc, '') [Department]
                                        FROM T_EmployeeMaster
	                                    LEFT JOIN T_DepartmentCodeMaster
	                                      ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								                                       THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								                                       ELSE ''
							                                       END
                                        WHERE
                                            Emt_EmployeeID = '{0}' ";

                string sqlGetUserX = @" SELECT
	                                            Umt_Usercode,
	                                            Umt_userlname,
	                                            Umt_userfname,
	                                            Umt_usermi
                                            FROM 
	                                            T_Usermaster
                                            WHERE
	                                            Umt_UserCode = '{0}'";

                dsUser = dal.ExecuteDataSet(string.Format(sqlGetUser, userId), CommandType.Text);

                if (CommonMethods.isEmpty(dsUser))
                {
                    dsUser = dal.ExecuteDataSet(string.Format(sqlGetUserX, userId), CommandType.Text);
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error in getting user:" + ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }
    #endregion

    #region Check if logged in user is in Employee Master
    protected bool isInMaster(string userCode)
    {
        string userID = "";
        string sqlCheck = @"SELECT emt_employeeid
                            FROM T_EmployeeMaster
                            WHERE emt_employeeid = '{0}'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                userID = Convert.ToString(dal.ExecuteScalar(string.Format(sqlCheck, userCode), CommandType.Text));
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return (userID.Trim() == "");

    }
    #endregion

    #region Initialize Controls
    protected void InitializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFJOBSPLTENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();
    }
    #endregion

    #region Initialize the values of the controls
    protected void InitializeValues()
    {
        string startTime1 = string.Empty;
        string endTime1 = string.Empty;
        string startTime2 = string.Empty;
        string endTime2 = string.Empty;
        try
        {
            txtDate.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
            txtDay.Text = DayCode;
            txtShift.Text = string.Empty;
            txtShift.Text = strShift;
            if (DayCode.Equals("REST"))
                txtShift.ForeColor = System.Drawing.Color.WhiteSmoke;
            txtDOW.Text = Convert.ToDateTime(txtDate.Text).DayOfWeek.ToString().ToUpper();
            //Andre added: 20091223 - This is for the time entry on the form
            hfOTFRAC.Value = Convert.ToString(Methods.GetParameterValue("OTFRACTION"));
            try
            {
                startTime1 = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimein_1"]);
                endTime1 = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimeout_1"]);
                startTime2 = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimein_2"]);
                endTime2 = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimeout_2"]);
            }
            catch
            { 
                //No logs were retrieved
            }

            if (startTime1 != "" && startTime1 != "0000")
            {
                txtTimeIn.Text = startTime1.Substring(0, 2) + ":" + startTime1.Substring(2, 2);
            }
            if (endTime1 != "" && endTime1 != "0000")
            {
                txtTimeOut.Text = endTime1.Substring(0, 2) + ":" + endTime1.Substring(2, 2);
            }
            if (startTime2 != "" && startTime2 != "0000")
            {
                txtTimeIn2.Text = startTime2.Substring(0, 2) + ":" + startTime2.Substring(2, 2);
            }
            if (endTime2 != "" && endTime2 != "0000")
            {
                txtTimeOut2.Text = endTime2.Substring(0, 2) + ":" + endTime2.Substring(2, 2);
            }

            string s = dsShift.Tables[0].Rows[0]["scm_shifttimein"].ToString();

            txtJStart.Text = JSBL.GetStartTime(txtTimeIn.Text, txtTimeIn2.Text, strShift, Convert.ToDecimal(hfOTFRAC.Value));

            if (!dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavetype"].ToString().Equals(string.Empty))
            {
                txtPLeave.Text = dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavetype"].ToString() + dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavehr"].ToString();
            }

            if (!dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavetype"].ToString().Equals(string.Empty))
            {
                txtNPLeave.Text = dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavetype"].ToString() + dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavehr"].ToString();
            }
            
        }
        catch(Exception ex)
        {
            //btnZ.Visible = false;
//temporary commenting below line to make the messagebox appear informing the user if his transaction has been saved, etc.
            //Session["flagCheck"] = "NOLOGS";
        }

        #region Recent Jobs
        string sqlRJobs = @"
                                        declare @Days AS decimal
                                        SET @Days = (SELECT Pmt_NumericValue 
                                                       FROM T_ParameterMaster
                                                      WHERE Pmt_ParameterId = 'DAYVWJOB')

                                        SELECT DISTINCT Jsd_JobCode
                                             , Jsd_ClientJobNo
                                             , Slm_ClientJobName
                                             , LTRIM(RTRIM(Jsd_SubWorkCode))  AS Jsd_SubWorkCode
                                             , Swc_Description
                                          FROM T_JobSplitDetail
                                          LEFT JOIN T_SalesMaster 
                                            ON Slm_DashJobCode = Jsd_JobCode
                                           AND Slm_ClientJobNo = Jsd_ClientJobNo
                                           AND (Slm_Costcenter = Jsd_CostCenter OR Slm_Costcenter = 'ALL')
                                         INNER JOIN T_JobSplitHeader
                                            ON Jsh_ControlNo = Jsd_ControlNo 
                                           AND Jsh_EmployeeId = '{0}'
                                           AND Jsh_Status IN ('9','F')
                                           AND Jsh_JobSplitDate BETWEEN dateadd(dd, -@Days,Getdate()) AND Getdate()
                                          LEFT JOIN {1}..E_SubWorkCodeMaster
	                                        ON LTRIM(Jsd_SubWorkCode) = Swc_AccountCode
		                                        AND 
			                                        (		Jsd_CostCenter = Swc_CostCenterCode
				                                        OR	Swc_CostCenterCode = 'ALL')
                                         WHERE Jsd_Status IN ('9','F')";
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsJobs = dal.ExecuteDataSet(string.Format(sqlRJobs, Session["userLogged"].ToString(), ConfigurationManager.AppSettings["ERP_DB"]), CommandType.Text);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
            }
        }
        string val1 = string.Empty;
        string val2 = string.Empty;
        string val3 = string.Empty;
        string val4 = string.Empty;
        string val5 = string.Empty;
        string listValue = string.Empty;
        string listSubWork = string.Empty;

        if (!CommonMethods.isEmpty(dsJobs))
        {
            for (int ctr = 0; ctr < dsJobs.Tables[0].Rows.Count; ctr++)
            {
                val1 = dsJobs.Tables[0].Rows[ctr]["Jsd_jobCode"].ToString().Trim();
                val2 = dsJobs.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString().Trim();
                val3 = dsJobs.Tables[0].Rows[ctr]["Slm_ClientJobName"].ToString().Trim();
                val4 = dsJobs.Tables[0].Rows[ctr]["Jsd_SubWorkCode"].ToString().Trim();
                val5 = dsJobs.Tables[0].Rows[ctr]["Swc_Description"].ToString().Trim();
                listValue = getCostCenterAndCategoryV2(val1.Trim(),val2.Trim(), txtEmployeeId.Text);
                listSubWork = getSubWorks(getCostCenterAndCategory(val1, val2));

                listValue += "~" + listSubWork;
                while (val1.Length < 15)
                {
                    val1 = val1 + " ";
                }
                val1 = val1 + "~ ";

                while (val2.Length < 15)
                {
                    val2 = val2 + " ";
                }
                val2 = val2 + "~ ";

                while (val3.Length < 10)
                {
                    val3 = val3 + " ";
                }
                val3 = val3 + "~ ";

                val4 = val4 + " ~ ";

                listBox.Items.Add(new ListItem(val1 + val2 + val3 + val4 + val5, listValue));
            }
        }
        #endregion
    }
    #endregion

    #region [Initialize Unsplitted Hours]
    private void InitializeUnsplittedHours()
    {
        this.pnlUnsplittedHours.Visible = true;
        DataSet ds = JSBL.GetUnsplittedHours(Session["userLogged"].ToString().Trim());

        if (!CommonMethods.isEmpty(ds))
        {
            if (Convert.ToDecimal(ds.Tables[0].Rows[0][1].ToString()) == 0)
            {
                this.pnlUnsplittedHours.Visible = false;
            }
            else
            {
                this.pnlUnsplittedHours.Visible = true;
                //string contentUnsplittedHours = "Job Split Date";
                //this.lblUnsplittedHoursDate.Text = contentUnsplittedHours;
                //this.lblUnsplittedHoursDateValue.Text = ":  " + ds.Tables[0].Rows[0][0].ToString();
                //contentUnsplittedHours = "Unsplitted Hours";
                //this.lblUnsplittedHours.Text = contentUnsplittedHours;
                //this.lblUnsplittedHoursValue.Text = ":  " + ds.Tables[0].Rows[0][1].ToString();
                string contentUnsplittedHours = "You have ";
                this.lblUnsplittedHours.Text = contentUnsplittedHours;
                this.lblUnsplittedHoursValue.Text = ds.Tables[0].Rows[0][1].ToString();
                this.lblUnsplittedHoursDate.Text = " unallocated hours on ";
                this.lblUnsplittedHoursDateValue.Text = ds.Tables[0].Rows[0][0].ToString();
            }
        }
        else
        {
            this.pnlUnsplittedHours.Visible = false;
        }
    }
    #endregion
    #endregion

    #region [Events]
    #region Button Y events
    protected void btnZ_Click(object sender, EventArgs e)
    {
        string errorMessage = validateEntry();
        if (errorMessage.Equals(string.Empty))
        {
            string start = txtJStart.Text + ",";
            string end = txtJEnd.Text + ",";
            string hour = txtJHours.Text + ",";
            string jobcode = txtJJobCode.Text + ",";
            string clientno = txtJClientNo.Text + ",";
            string clientname = txtJClientName.Text + ",";
            string subworkCode = txtSubWork.Text + ",";
            string category = hCat.Value + ",";
            string overtime = hOvertime.Value + ",";
            string costcenter = hCCT.Value + ",";

            string[] VALUES = new string[10];
            
            using (DALHelper dal = new DALHelper())
            {
                try
                {

                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();

                    int ctr = 0;
                    string controlNo = string.Empty;

                    if (Convert.ToInt32(hasJob_Split.Value) < 1)
                    {
                        dsSplit = JSBL.LoadCurrentJobSplitDetails(txtDate.Text, txtEmployeeId.Text, dal);
                        hasJob_Split.Value = dsSplit.Tables[0].Rows.Count.ToString();
                        if (Convert.ToInt32(hasJob_Split.Value) > 0)
                        {
                            errorMessage = "SYSGENERROR";
                            Session["flagCheck"] = "SYSGENERROR";
                        }
                        else
                        {
                            #region Create New record for header and detail

                            controlNo = Methods.GetControlNumber("JOBSPLIT");

                            //Create Header
                            DataRow dr = PopulateDRHeader("1", controlNo);
                            JSBL.CreateJSHeader(dr, dal);
                            //Insert Details
                            ctr = 1;
                            string count = string.Empty;

                            while (start.Length > 1)
                            {
                                //Gets the data in order
                                VALUES[0] = start.Substring(0, start.IndexOf(",")).Trim();
                                VALUES[1] = end.Substring(0, end.IndexOf(",")).Trim();
                                VALUES[2] = hour.Substring(0, hour.IndexOf(",")).Trim();
                                VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(",")).Trim();
                                VALUES[4] = clientno.Substring(0, clientno.IndexOf(",")).Trim();
                                VALUES[5] = clientname.Substring(0, clientname.IndexOf(",")).Trim();
                                VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(",")).Trim();
                                VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                ctr++;
                                DataRow drDetail = PopulateDRDetail("1", VALUES, controlNo, count);
                                JSBL.InsertDetails(drDetail, dal);

                                //Trimming the data
                                start = start.Substring(start.IndexOf(",") + 1);
                                end = end.Substring(end.IndexOf(",") + 1);
                                hour = hour.Substring(hour.IndexOf(",") + 1);
                                jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                                category = category.Substring(category.IndexOf(",") + 1);
                                overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region Updating Job Split for the day
                        controlNo = hiddenCNumber.Value;
                        int prev = 0;
                        int curr = 0;
                        prev = Convert.ToInt32(hasJob_Split.Value);//prev is set
                        string temp = start;
                        while (temp.Length > 1)
                        {
                            temp = temp.Substring(start.IndexOf(",") + 1);
                            curr++;
                        }//curr is set

                        #region Scenario 1(if previous count is equal to current count): Update all previous records with current
                        if (prev == curr)
                        {
                            //Update Header
                            DataRow dr = PopulateDRHeader("1", controlNo);
                            JSBL.UpdateJSHeader(dr, dal);
                            //Insert Details
                            ctr = 1;
                            string count = string.Empty;

                            while (start.Length > 1)
                            {
                                //Gets the data in order
                                VALUES[0] = start.Substring(0, start.IndexOf(",")).Trim();
                                VALUES[1] = end.Substring(0, end.IndexOf(",")).Trim();
                                VALUES[2] = hour.Substring(0, hour.IndexOf(",")).Trim();
                                VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(",")).Trim();
                                VALUES[4] = clientno.Substring(0, clientno.IndexOf(",")).Trim();
                                VALUES[5] = clientname.Substring(0, clientname.IndexOf(",")).Trim();
                                VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(",")).Trim();
                                VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                if (ctr < 10)
                                    count = "0" + ctr.ToString();
                                ctr++;
                                DataRow drDetail = PopulateDRDetail("1", VALUES, controlNo, count); 
                                JSBL.UpdateDetails(drDetail, dal);

                                //Trimming the data
                                start = start.Substring(start.IndexOf(",") + 1);
                                end = end.Substring(end.IndexOf(",") + 1);
                                hour = hour.Substring(hour.IndexOf(",") + 1);
                                jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                                category = category.Substring(category.IndexOf(",") + 1);
                                overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                            }
                        }
                        #endregion

                        #region Scenario 2(if previous count is less than current count):Update all previous and insert new rows
                        else if (prev < curr)
                        {
                            //Update Header
                            DataRow dr = PopulateDRHeader("1", controlNo);
                            JSBL.UpdateJSHeader(dr, dal);
                            //Insert Details
                            ctr = 1;
                            string count = string.Empty;

                            while (start.Length > 1)
                            {
                                //Gets the data in order
                                VALUES[0] = start.Substring(0, start.IndexOf(",")).Trim();
                                VALUES[1] = end.Substring(0, end.IndexOf(",")).Trim();
                                VALUES[2] = hour.Substring(0, hour.IndexOf(",")).Trim();
                                VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(",")).Trim();
                                VALUES[4] = clientno.Substring(0, clientno.IndexOf(",")).Trim();
                                VALUES[5] = clientname.Substring(0, clientname.IndexOf(",")).Trim();
                                VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(",")).Trim();
                                VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                ctr++;
                                DataRow drDetail = PopulateDRDetail("1", VALUES, controlNo, count); 
                                if (ctr < prev + 2)
                                {
                                    JSBL.UpdateDetails(drDetail, dal);
                                }
                                else
                                {
                                    JSBL.InsertDetails(drDetail, dal);
                                }

                                //Trimming the data
                                start = start.Substring(start.IndexOf(",") + 1);
                                end = end.Substring(end.IndexOf(",") + 1);
                                hour = hour.Substring(hour.IndexOf(",") + 1);
                                jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                                category = category.Substring(category.IndexOf(",") + 1);
                                overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                            }
                        }
                        #endregion

                        #region Scenario 3(if previous count id greater than current count):Update previous up to current count and delete the rest
                        else if (prev > curr)
                        {
                            //Update Header
                            DataRow dr = PopulateDRHeader("1", controlNo);
                            JSBL.UpdateJSHeader(dr, dal);
                            //Insert Details
                            ctr = 1;
                            string count = string.Empty;
                            int loop = 0;
                            while (loop < prev)
                            {
                                if (ctr < curr + 1)
                                {
                                    //Gets the data in order
                                    VALUES[0] = start.Substring(0, start.IndexOf(",")).Trim();
                                    VALUES[1] = end.Substring(0, end.IndexOf(",")).Trim();
                                    VALUES[2] = hour.Substring(0, hour.IndexOf(",")).Trim();
                                    VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(",")).Trim();
                                    VALUES[4] = clientno.Substring(0, clientno.IndexOf(",")).Trim();
                                    VALUES[5] = clientname.Substring(0, clientname.IndexOf(",")).Trim();
                                    VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(",")).Trim();
                                    VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                    VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                    VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                    count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                    ctr++;
                                    DataRow drDetail = PopulateDRDetail("1", VALUES, controlNo, count); 

                                    JSBL.UpdateDetails(drDetail, dal);

                                    //Trimming the data
                                    start = start.Substring(start.IndexOf(",") + 1);
                                    end = end.Substring(end.IndexOf(",") + 1);
                                    hour = hour.Substring(hour.IndexOf(",") + 1);
                                    jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                    clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                    clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                    subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                                    category = category.Substring(category.IndexOf(",") + 1);
                                    overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                    costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                                }
                                else
                                {
                                    count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();
                                    ctr++;
                                    DataRow drDetails = PopulateDRDelete(controlNo, count);
                                    JSBL.DeleteDetails(drDetails, dal);
                                }

                                loop++;
                            }
                        }
                        #endregion
                        #endregion//End:Updating Job Split for the day
                    }
                    if (errorMessage.Equals(string.Empty))
                    {
                        Session["flagCheck"] = "JSSAVED";
                    }

                    //MenuLog
                    SystemMenuLogBL.InsertAddLog("WFJOBSPLTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "",false);
                    dal.CommitTransactionSnapshot();

                    hasJob_Split.Value = (ctr - 1).ToString();
                    hiddenCNumber.Value = controlNo;
                }
                catch
                {
                    dal.RollBackTransactionSnapshot();
                    Session["flagCheck"] = "JSERROR";
                }
                finally
                {
                    dal.CloseDB();

                    if (!new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFJOBSPLTENTRY").canAdd())
                    {
                        Response.Redirect("pgeWorkRecord.aspx");
                    }
                    else
                    {
                        showMessages();
                    }
                }
            }
        }
        else
        {
            MessageBox.Show(errorMessage);
        }
    }
    private bool checkOverLap(string prev, string curr)
    {
        int iPrev = Convert.ToInt32(prev);
        int iCurr = Convert.ToInt32(curr);

        return (iPrev > iCurr);
    }
    private string validateEntry()
    {
        string start = txtJStart.Text + ",";
        string end = txtJEnd.Text + ",";
        string jobcode = txtJJobCode.Text + ",";
        string clientno = txtJClientNo.Text + ",";
        string cFlag = hCFlag.Value + ",";
        decimal OTFRAC = Convert.ToDecimal(hfOTFRAC.Value);
        string[] VALUES = new string[2];
        string valid = string.Empty;
        string tValid = string.Empty;
        string lockValid = string.Empty;

        string overlap = string.Empty;
        string holdPrevTime = string.Empty;

        while (start.Length > 1 && valid.Equals(string.Empty))
        {
            //Gets the data in order
            VALUES[0] = start.Substring(0, start.IndexOf(",")).Replace(":","");
            VALUES[1] = end.Substring(0, end.IndexOf(",")).Replace(":", "");

            if (overlap.Equals(string.Empty) && !holdPrevTime.Equals(string.Empty) && checkOverLap(holdPrevTime, VALUES[0]))
            {
                overlap = "\n Time overlap in entry.";
            }
            holdPrevTime = VALUES[1];

            if (VALUES[0].ToString().Length != 4 || VALUES[1].ToString().Length != 4)
                valid += @"Invalid End/Start Time Entry. Please Review Transaction.";
            //OTFRACTION CHECK ON HOURS
            if (!valid.Equals(string.Empty) && Convert.ToDecimal(VALUES[0].Substring(2, 2)) % OTFRAC != 0 || Convert.ToDecimal(VALUES[1].Substring(2, 2)) % OTFRAC != 0)
            {
                tValid = " Invalid time entry in form. System only allows minutes for every " + OTFRAC.ToString();
            }

            start = start.Substring(start.IndexOf(",") + 1);
            end = end.Substring(end.IndexOf(",") + 1);
        }

        if (txtDay.Text != "REG" && Convert.ToDecimal(txtTotalHours.Text) > Methods.GetParameterValue("MAXOTHR"))
        {
            valid += "\n Invalid number of hours. Total hours must not exceed [" + hfMaxHours.Value + "].";
        }

        //Andre added: 20091205 for the cost center bill cycle locking
        string holdChar = string.Empty;
        string tempHeader = string.Empty;
        DataTable dt = new DataTable();
        dt.Columns.Add("cycle"); 
        dt.Columns.Add("jobcode"); 
        dt.Columns.Add("clientcode");
        //dt.Columns.Add("clientname");
        //string cName = txtJClientName.Text;
            
        while (jobcode.Length > 1 && valid.Equals(string.Empty))
        {
            //Gets the data in order
            VALUES[0] = jobcode.Substring(0, jobcode.IndexOf(","));
            VALUES[1] = clientno.Substring(0, clientno.IndexOf(","));
            
            holdChar = cFlag.Substring(0, cFlag.IndexOf(","));
            //if ((holdChar.Equals("1") || holdChar.Equals("2")) && isJobLocked(txtDate.Text, VALUES[0], VALUES[1]))
            
            if (holdChar.Equals("1") && isJobLocked(txtDate.Text, VALUES[0], VALUES[1]))
            {
                tempHeader = GetBillCycle(VALUES[0], VALUES[1]);
                if (!tempHeader.Equals(string.Empty))
                    dt.Rows.Add(new object[] { tempHeader, VALUES[0].Trim(), VALUES[1].Trim() });

                //valid += @"\n Billing Cycle for the month of " + "December (Nov 16 - Dec 15) has \nbeen locked since 12/05/2009 13:20. \n The following jobs cannot be altered: \n" + VALUES[0] + "~" + VALUES[1];
            }
            jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
            clientno = clientno.Substring(clientno.IndexOf(",") + 1);
            cFlag = cFlag.Substring(cFlag.IndexOf(",") + 1);
        }
        if (dt.Rows.Count > 0)
        {
            DataRow[] dr = dt.Select("1=1", "cycle ASC");
            bool fx = true;
            for (int ctr = 0; ctr < dr.Length; ctr++)
            {
                //Add header or not
                if (!fx && Convert.ToString(dr[ctr - 1]["cycle"]) != Convert.ToString(dr[ctr]["cycle"]))
                {
                    fx = true;
                }
                if (fx)
                {
                    lockValid += "\n Billing Cycle for " + GetBillCycleDesc(Convert.ToString(dr[ctr]["cycle"])) + " has been locked since " + GetLockInfo(txtDate.Text, Convert.ToString(dr[ctr]["cycle"])) + ".\n Affected jobs:(Job Code ~ Client Code)";
                    fx = false;
                }

                lockValid += " \n       " + Convert.ToString(dr[ctr]["jobcode"]) + " ~ " + Convert.ToString(dr[ctr]["clientcode"]); 
                
            }
        }

        return valid + tValid + lockValid + overlap ;
    }
    private string GetLockInfo(string date, string cycle)
    {
        string sql = string.Format(@"
                        SELECT Convert(varchar(10), ludatetime,101) + ' ' 
                             + LEFT(Convert(varchar(10), ludatetime, 114), 5)
                             + ' by ' 
                             + ISNULL(dbo.GetControlEmployeeName(Usr_Login), 'ADMIN')
                          FROM T_BillingConfiguration
                         WHERE Bcn_BillingYearMonth = Convert(varchar(4),datepart(year,'{0}')) + REPLICATE('0' , 2 - LEN(Convert(varchar(2),datepart(month,'{0}')))) + Convert(varchar(2),datepart(month,'{0}'))
                           AND Bcn_BillingCycle = '{1}'", date, cycle);
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                sql = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {
                sql = string.Empty;
            }
            finally
            {
                dal.CloseDB();
            }
        }
        return sql;
    }

    private string GetBillCycleDesc(string billCycleCode)
    {
        string sql = string.Format(@"
                       SELECT Adt_AccountDesc FROM T_AccountDetail
                        WHERE Adt_AccountType = 'BILLCYCLE'
                          AND Adt_AccountCode = '{0}'   
                    ", billCycleCode);

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                sql = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {
                sql = string.Empty;
            }
            finally
            {
                dal.CloseDB();
            }
        }
        return sql;
    }

    private string GetBillCycle(string jobCode, string clientCode)
    {
        string retValue = string.Empty;
        string sql = string.Format(@"
                    SELECT Bcc_BillingCycle
                      FROM T_SalesMaster
                     INNER JOIN T_CostcenterBillingCycle 
                        ON Bcc_Costcenter = Slm_Costcenter
                     WHERE Slm_DashJobCode = '{0}'
                       AND Slm_ClientJobNo = '{1}'
                    ", jobCode.Trim(), clientCode.Trim());
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                retValue = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        return retValue;
    }
    private bool isJobLocked(string splitDate,string jobCode, string clientCode)
    {
        bool flag = new bool();
        string sql = string.Format(@"
                   SELECT Bcn_Lock 
                     FROM T_BillingConfiguration
                    WHERE '{0}' BETWEEN Bcn_StartCycle AND Bcn_EndCycle
                      AND Bcn_BillingCycle = (SELECT Bcc_BillingCycle
                                                FROM T_SalesMaster
                                               INNER JOIN T_CostcenterBillingCycle 
                                                  ON Bcc_Costcenter = Slm_Costcenter
                                               WHERE Slm_DashJobCode = '{1}'
                                                 AND Slm_ClientJobNo = '{2}')", splitDate.Trim()
                                                                              , jobCode.Trim()
                                                                              , clientCode.Trim());


        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                flag = Convert.ToBoolean(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {
                flag = false;
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return flag;
    }
    #endregion

    #region Button X events
    protected void btnX_Click(object sender, EventArgs e)
    {
        string start = txtJStart.Text + ",";
        string end = txtJEnd.Text + ",";
        string hour = txtJHours.Text + ",";
        string jobcode = txtJJobCode.Text + ",";
        string clientno = txtJClientNo.Text + ",";
        string clientname = txtJClientName.Text + ",";
        string subworkCode = txtSubWork.Text + ",";
        //string billable = hBillable.Value + ",";
        string category = hCat.Value + ",";
        string jobType = hOvertime.Value + ",";
        string costcenter = hCCT.Value + ",";
        string errorMessage = string.Empty;
        
        string[] VALUES = new string[10];
        
        using (DALHelper dal = new DALHelper())
        {
            try
            {

                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                int ctr = 0;
                string controlNo = string.Empty;

                if (Convert.ToInt32(hasJob_Split.Value) < 1)//TRUE if no records was retrive
                {
                    dsSplit = JSBL.LoadCurrentJobSplitDetails(txtDate.Text, txtEmployeeId.Text, dal);
                    hasJob_Split.Value = dsSplit.Tables[0].Rows.Count.ToString();
                    if (Convert.ToInt32(hasJob_Split.Value) > 0)
                    {
                        errorMessage = "SYSGENERROR";
                        Session["flagCheck"] = "SYSGENERROR";
                    }
                    else
                    {
                        #region Create New record for header and detail

                        controlNo = Methods.GetControlNumber("JOBSPLIT");

                        //Create Header
                        DataRow dr = PopulateDRHeader("9", controlNo);
                        JSBL.CreateJSHeader(dr, dal);
                        //Insert Details
                        ctr = 1;
                        string count = string.Empty;

                        while (start.Length > 1)
                        {
                            //Gets the data in order
                            VALUES[0] = start.Substring(0, start.IndexOf(","));
                            VALUES[1] = end.Substring(0, end.IndexOf(","));
                            VALUES[2] = hour.Substring(0, hour.IndexOf(","));
                            VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(","));
                            VALUES[4] = clientno.Substring(0, clientno.IndexOf(","));
                            VALUES[5] = clientname.Substring(0, clientname.IndexOf(","));
                            VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(","));
                            VALUES[7] = category.Substring(0, category.IndexOf(","));
                            VALUES[8] = jobType.Substring(0, jobType.IndexOf(","));
                            VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(","));

                            count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                            ctr++;
                            DataRow drDetail = PopulateDRDetail("9", VALUES, controlNo, count);
                            JSBL.InsertDetails(drDetail, dal);

                            //Trimming the data
                            start = start.Substring(start.IndexOf(",") + 1);
                            end = end.Substring(end.IndexOf(",") + 1);
                            hour = hour.Substring(hour.IndexOf(",") + 1);
                            jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                            clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                            clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                            subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                            category = category.Substring(category.IndexOf(",") + 1);
                            jobType = jobType.Substring(jobType.IndexOf(",") + 1);
                            costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                        }
                        #endregion
                    }
                }
                else
                {
                    #region Updating Job Split for the day
                    string cn = hiddenCNumber.Value;
                    int prev = 0;
                    int curr = 0;
                    prev = Convert.ToInt32(hasJob_Split.Value);//prev is set
                    string temp = start;
                    while (temp.Length > 1)
                    {
                        temp = temp.Substring(start.IndexOf(",") + 1);
                        curr++;
                    }//curr is set

                    #region Scenario 1(if previous count is equal to current count): Update all previous records with current
                    if (prev == curr)
                    {
                        //Update Header
                        DataRow dr = PopulateDRHeader("9", cn);
                        JSBL.UpdateJSHeader(dr, dal);
                        //Insert Details
                        ctr = 1;
                        string count = string.Empty;

                        while (start.Length > 1)
                        {
                            //Gets the data in order
                            VALUES[0] = start.Substring(0, start.IndexOf(","));
                            VALUES[1] = end.Substring(0, end.IndexOf(","));
                            VALUES[2] = hour.Substring(0, hour.IndexOf(","));
                            VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(","));
                            VALUES[4] = clientno.Substring(0, clientno.IndexOf(","));
                            VALUES[5] = clientname.Substring(0, clientname.IndexOf(","));
                            VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(","));
                            VALUES[7] = category.Substring(0, category.IndexOf(","));
                            VALUES[8] = jobType.Substring(0, jobType.IndexOf(","));
                            VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(","));

                            if (ctr < 10)
                                count = "0" + ctr.ToString();
                            ctr++;
                            DataRow drDetail = PopulateDRDetail("9", VALUES, cn, count);
                            JSBL.UpdateDetails(drDetail, dal);

                            //Trimming the data
                            start = start.Substring(start.IndexOf(",") + 1);
                            end = end.Substring(end.IndexOf(",") + 1);
                            hour = hour.Substring(hour.IndexOf(",") + 1);
                            jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                            clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                            clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                            subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                            category = category.Substring(category.IndexOf(",") + 1);
                            jobType = jobType.Substring(jobType.IndexOf(",") + 1);
                            costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                        }
                    }
                    #endregion

                    #region Scenario 2(if previous count is less than current count):Update all previous and insert new rows
                    else if (prev < curr)
                    {
                        //Update Header
                        DataRow dr = PopulateDRHeader("9", cn);
                        JSBL.UpdateJSHeader(dr, dal);
                        //Insert Details
                        ctr = 1;
                        string count = string.Empty;

                        while (start.Length > 1)
                        {
                            //Gets the data in order
                            VALUES[0] = start.Substring(0, start.IndexOf(","));
                            VALUES[1] = end.Substring(0, end.IndexOf(","));
                            VALUES[2] = hour.Substring(0, hour.IndexOf(","));
                            VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(","));
                            VALUES[4] = clientno.Substring(0, clientno.IndexOf(","));
                            VALUES[5] = clientname.Substring(0, clientname.IndexOf(","));
                            VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(","));
                            VALUES[7] = category.Substring(0, category.IndexOf(","));
                            VALUES[8] = jobType.Substring(0, jobType.IndexOf(","));
                            VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(","));

                            count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                            ctr++;
                            DataRow drDetail = PopulateDRDetail("9", VALUES, cn, count);
                            if (ctr < prev + 2)
                            {
                                JSBL.UpdateDetails(drDetail, dal);
                            }
                            else
                            {
                                JSBL.InsertDetails(drDetail, dal);
                            }

                            //Trimming the data
                            start = start.Substring(start.IndexOf(",") + 1);
                            end = end.Substring(end.IndexOf(",") + 1);
                            hour = hour.Substring(hour.IndexOf(",") + 1);
                            jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                            clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                            clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                            subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                            category = category.Substring(category.IndexOf(",") + 1);
                            jobType = jobType.Substring(jobType.IndexOf(",") + 1);
                            costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);

                        }
                    }
                    #endregion

                    #region Scenario 3(if previous count id greater than current count):Update previous up to current count and delete the rest
                    else if (prev > curr)
                    {
                        //Update Header
                        DataRow dr = PopulateDRHeader("9", cn);
                        JSBL.UpdateJSHeader(dr, dal);
                        //Insert Details
                        ctr = 1;
                        string count = string.Empty;
                        int loop = 0;
                        while (loop < prev)
                        {
                            if (ctr < curr + 1)
                            {
                                //Gets the data in order
                                VALUES[0] = start.Substring(0, start.IndexOf(","));
                                VALUES[1] = end.Substring(0, end.IndexOf(","));
                                VALUES[2] = hour.Substring(0, hour.IndexOf(","));
                                VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(","));
                                VALUES[4] = clientno.Substring(0, clientno.IndexOf(","));
                                VALUES[5] = clientname.Substring(0, clientname.IndexOf(","));
                                VALUES[6] = subworkCode.Substring(0, subworkCode.IndexOf(","));
                                VALUES[7] = category.Substring(0, category.IndexOf(","));
                                VALUES[8] = jobType.Substring(0, jobType.IndexOf(","));
                                VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(","));

                                count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                ctr++;
                                DataRow drDetail = PopulateDRDetail("9", VALUES, cn, count);

                                JSBL.UpdateDetails(drDetail, dal);

                                //Trimming the data
                                start = start.Substring(start.IndexOf(",") + 1);
                                end = end.Substring(end.IndexOf(",") + 1);
                                hour = hour.Substring(hour.IndexOf(",") + 1);
                                jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                subworkCode = subworkCode.Substring(subworkCode.IndexOf(",") + 1);
                                category = category.Substring(category.IndexOf(",") + 1);
                                jobType = jobType.Substring(jobType.IndexOf(",") + 1);
                                costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                            }
                            else
                            {
                                count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();
                                ctr++;
                                DataRow drDetails = PopulateDRDelete(cn, count);
                                JSBL.DeleteDetails(drDetails, dal);
                            }

                            loop++;
                        }
                    }
                    #endregion
                    #endregion//End:Updating Job Split for the day
                }
// below's previous condition was wrong
                if (errorMessage == string.Empty)
                {
                    Session["flagCheck"] = "JSSUBMIT";
                }
                //MenuLog
                SystemMenuLogBL.InsertAddLog("WFJOBSPLTENTRY", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "");
                dal.CommitTransactionSnapshot();

                hasJob_Split.Value = (ctr - 1).ToString();
                hiddenCNumber.Value = controlNo;
                hiddenStatus.Value = "9";
            }
            catch
            {
                Session["flagCheck"] = "JSERROR";
                dal.RollBackTransactionSnapshot();
            }
            finally
            {
                dal.CloseDB();

                if (!new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFJOBSPLTENTRY").canAdd())
                {
                    Response.Redirect("pgeWorkRecord.aspx");
                }
                else
                {
                    showMessages();
                }
            }
        }
    }
    #endregion
    #endregion

    #region [Saving]
    #region Populate data row
    private DataRow PopulateDRHeader(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitHeader");
        dr["Jsh_ControlNo"] = ControlNum.ToUpper();
        dr["Jsh_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Jsh_JobSplitDate"] = Convert.ToDateTime(txtDate.Text);
        dr["Jsh_Entry"] = " ";
        dr["Jsh_Costcenter"] = string.Empty;
        dr["Jsh_RefControlNo"] =  string.Empty;
        dr["Jsh_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        return dr;
    }
    #endregion

    #region Populate data row for details
    private DataRow PopulateDRDetail(string Status, string[] values, string ControlNum, string counter)
    {
        //For guide of values[] only
        //VALUES[0] = start.Substring(0, start.IndexOf(","));
        //VALUES[1] = end.Substring(0, end.IndexOf(","));
        //VALUES[2] = hour.Substring(0, hour.IndexOf(","));
        //VALUES[3] = jobcode.Substring(0, jobcode.IndexOf(","));
        //VALUES[4] = clientno.Substring(0, clientno.IndexOf(","));
        //VALUES[5] = clientname.Substring(0, clientname.IndexOf(","));

        DataRow dr = DbRecord.Generate("T_JobSplitDetail");
        dr["Jsd_ControlNo"] = ControlNum.ToUpper();
        dr["Jsd_Seqno"] = counter;
        dr["Jsd_StartTime"] = values[0].Replace(":", "").ToUpper();
        dr["Jsd_EndTime"] = values[1].Replace(":", "").ToUpper();
        dr["Jsd_JobCode"] = values[3].Trim().ToUpper();
        dr["Jsd_ClientJobNo"] = values[4].Trim().ToUpper();
        dr["Jsd_PlanHours"] = values[2].ToUpper();
        dr["Jsd_ActHours"] = 0;
        dr["Jsd_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        dr["Jsd_Category"] = values[7];
        dr["Jsd_Overtime"] = Convert.ToBoolean(values[8]);
        dr["Jsd_CostCenter"] = values[9];

        dr["Jsd_SubWorkCode"] = values[6];

        return dr;
    }

    private DataRow PopulateDRDelete(string ControlNum, string counter)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitDetail");
        dr["Jsd_ControlNo"] = ControlNum.ToUpper();
        dr["Jsd_Seqno"] = counter.ToUpper();
        dr["Jsd_StartTime"] = string.Empty;
        dr["Jsd_EndTime"] = string.Empty;
        dr["Jsd_JobCode"] = string.Empty;
        dr["Jsd_ClientJobNo"] = string.Empty;
        dr["Jsd_PlanHours"] = 0.0;
        dr["Jsd_ActHours"] = 0;
        dr["Jsd_Status"] = "1";
        dr["Usr_Login"] = Session["userLogged"].ToString();

        return dr;
    }
    #endregion
    #endregion

    #region [Other Methods]
    #region Gets the Job Split Entry for the day if any
    protected void hasJobSplit()
    {
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsSplit.Clear();
                dsSplit = JSBL.LoadCurrentJobSplitDetails(txtDate.Text, Session["userLogged"].ToString(), dal);
                hasJob_Split.Value = dsSplit.Tables[0].Rows.Count.ToString();
                if (Convert.ToInt32(hasJob_Split.Value) > 0)
                    hiddenCNumber.Value = dsSplit.Tables[0].Rows[0]["Jsh_ControlNo"].ToString();
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
        try
        {
            if (!CommonMethods.isEmpty(dsSplit))
            {
                string v1 = string.Empty;
                string v2 = string.Empty;
                string v3 = string.Empty;
                string v4 = string.Empty;
                string v5 = string.Empty;
                string v6 = string.Empty;
                string v7 = string.Empty;
                string v8 = string.Empty;   // v8 currently not used
                string v9 = string.Empty;
                string v10 = string.Empty;
                string v11 = string.Empty;
                //Billable
                string v12 = string.Empty;
                // job type
                string v13 = string.Empty;

                string tempJobCode = string.Empty;
                string tempJobNo = string.Empty;

                string Category = string.Empty;
                string Costcenter = string.Empty;

                int max = dsSplit.Tables[0].Rows.Count;

                for (int ctr = 0; ctr < max; ctr++)
                {
                    v1 = v1 + dsSplit.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(0, 2) + ":" + dsSplit.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(2, 2) + ",";
                    v2 = v2 + dsSplit.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(0, 2) + ":" + dsSplit.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(2, 2) + ",";
                    v3 = v3 + Convert.ToString(dsSplit.Tables[0].Rows[ctr]["Jsd_PlanHours"]) + ",";
                    v4 = v4 + dsSplit.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString() + ",";
                    v5 = v5 + dsSplit.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString() + ",";
                    v6 = v6 + dsSplit.Tables[0].Rows[ctr]["Slm_ClientJobName"].ToString() + ",";
                    v7 = v7 + dsSplit.Tables[0].Rows[ctr]["Jsd_SubWorkCode"].ToString() + ",";

                    tempJobCode = dsSplit.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString();
                    tempJobNo = dsSplit.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString();

                    v8 = v8 + getSubWorks(getCostCenterAndCategory(tempJobCode, tempJobNo)) + ",";

                    v9 = v9 + dsSplit.Tables[0].Rows[ctr]["Slm_Category"].ToString() + ",";
                    v10 = v10 + dsSplit.Tables[0].Rows[ctr]["Jsd_Costcenter"].ToString() + ",";
                    v11 = v11 + isLockedPopulate(isJobLocked(txtDate.Text
                                                            , dsSplit.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString()
                                                            , dsSplit.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString())) + ",";
                    v12 += dsSplit.Tables[0].Rows[ctr]["Jsd_Category"].ToString() + ",";
                    v13 += dsSplit.Tables[0].Rows[ctr]["Jsd_Overtime"].ToString() + ",";

                }
                // gpangkuha ang mga comma sa ulahi e.g. "True,True," -> "True,True"
                // manipulate function will do this
                //v1 = v1.Substring(0, v1.Length - 1);  
                //v2 = v2.Substring(0, v2.Length - 1);
                //v3 = v3.Substring(0, v3.Length - 1);
                //v4 = v4.Substring(0, v4.Length - 1);
                //v5 = v5.Substring(0, v5.Length - 1);
                //v6 = v6.Substring(0, v6.Length - 1);
                //v7 = v7.Substring(0, v7.Length - 1);
                //v8 = v8.Substring(0, v8.Length - 1);
                //v9 = v9.Substring(0, v9.Length - 1);
                //v10 = v10.Substring(0, v10.Length - 1);
                //v11 = v11.Substring(0, v11.Length - 1);
                //v12 = v12.Substring(0, v12.Length - 1);
                //v13 = v13.Substring(0, v13.Length - 1);

                //Assigning the values to the page. naa pa ni comma dre 
                txtJStart.Text = v1;
                txtJEnd.Text = v2;
                txtJHours.Text = v3;
                txtJJobCode.Text = v4;
                txtJClientNo.Text = v5;
                txtJClientName.Text = v6;
                txtSubWork.Text = v7;
                hSub.Value = v7;
                hCat.Value = v12;
                hCCT.Value = v10;
                hCFlag.Value = v11;
                hBillable.Value = v12;
                hiddenStatus.Value = dsSplit.Tables[0].Rows[0]["Jsh_Status"].ToString();
                hOvertime.Value = v13;
                
            }
            else //Kelvin added: 20110413 - If no jobsplit is retrieved from the database. initialize job fields to the most recent job by the employee
            {
                #region Initialize Job Fields
                DataSet dsRecentJob;

                dsRecentJob = JSBL.GetRecentJobByEmployee(Session["userLogged"].ToString());
                if (!CommonMethods.isEmpty(dsRecentJob))
                {
                    txtJJobCode.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_JobCode"].ToString().Trim();
                    txtJClientNo.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_ClientJobNo"].ToString().Trim();
                    txtJClientName.Text = dsRecentJob.Tables[0].Rows[0]["Slm_ClientJobName"].ToString().Trim();
                    txtSubWork.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_SubWorkCode"].ToString().Trim();
                    string categoryAndCostCenter = getCostCenterAndCategoryV2(txtJJobCode.Text, txtJClientNo.Text);
                    string category = categoryAndCostCenter.Substring(0, categoryAndCostCenter.IndexOf("~"));
                    
                    hCat.Value = category;
                    hBillable.Value = category.Equals("B").ToString();
                    hCCT.Value = dsRecentJob.Tables[0].Rows[0]["Jsd_CostCenter"].ToString().Trim();
                    hSub.Value = getSubWorks(getCostCenterAndCategory(txtJJobCode.Text, txtJClientNo.Text));
                    
                    if(DayCode == "REST")
                        hOvertime.Value = "true";
                    else
                        hOvertime.Value = "false";   
                }
                #endregion
            }
        }
        catch
        {
            //MessageBox.Show(ex.Message);
        }

    }
    #endregion

    protected string getSubWorks(string [] values)
    {
        string retVal = string.Empty;
        DataSet ds = new DataSet();

        string sql = @"
                        Select distinct(Swm_SubWorkCode) from t_usercostcenteraccess
                        inner join t_subworkmaster on swm_costcenter = uca_costcentercode
                        where uca_sytemid = 'TIMEKEEP'
                        and uca_usercode = '{0}'
                        and swm_category = '{1}'
                        and swm_status = 'A'
                    ";

        using(DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, txtEmployeeId.Text,values[1]), CommandType.Text);
            }
            catch
            {
                //Do nothing
            }
            finally
            {
                dal.CloseDB();            
            }
        }
        if (ds.Tables.Count > 0)
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows.Count == 1)
                    retVal = ds.Tables[0].Rows[0]["swm_subworkcode"].ToString();
                else
                    retVal = "NOTSET";
            }
            else
            {
                retVal = "NOTSET";
            }

        return retVal;
    }
    protected string[] getCostCenterAndCategory(string jobCode, string jobNo)
    {
        string[] retVal = new string[2];
        DataSet ds = new DataSet();

        string sql = @"
                    SELECT 
	                    slm_costcenter,
                        slm_category
                    FROM 
	                    t_salesmaster
                    WHERE
	                    slm_dashjobcode = '{0}'
                    AND
	                    slm_clientjobno = '{1}'
                    ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, jobCode, jobNo), CommandType.Text);
            }
            catch
            {
                //Do nothing
            }
            finally
            {
                dal.CloseDB();
            }
        }

        if(ds.Tables.Count > 0)
            if(ds.Tables[0].Rows.Count > 0)
            {
                retVal[0] = ds.Tables[0].Rows[0]["slm_costcenter"].ToString();
                retVal[1] = ds.Tables[0].Rows[0]["slm_category"].ToString();
            }
            else
            {
                retVal[0] = string.Empty;
                retVal[1] = string.Empty;
            }

        return retVal;
    }
    protected string getCostCenterAndCategoryV2(string jobCode, string jobNo)
    {
        DataSet ds = new DataSet();

        string sql = @"
                    SELECT 
	                    slm_costcenter,
                        slm_category
                    FROM 
	                    t_salesmaster
                    WHERE
	                    slm_dashjobcode = '{0}'
                    AND
	                    slm_clientjobno = '{1}'
                    ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, jobCode, jobNo), CommandType.Text);
            }
            catch
            {
                //Do nothing
            }
            finally
            {
                dal.CloseDB();
            }
        }

        string retVal = string.Empty;
        if (!CommonMethods.isEmpty(ds))
        {
            retVal += ds.Tables[0].Rows[0]["slm_category"].ToString();
            retVal += "~";
            retVal += ds.Tables[0].Rows[0]["slm_costcenter"].ToString();
        }

        return retVal;
    }

    protected string getCostCenterAndCategoryV2(string jobCode, string jobNo, string employeeId)
    {
        DataSet ds = new DataSet();

        string sql = @"
                    SELECT 
	                    slm_costcenter,
                        slm_category
                    FROM 
	                    t_salesmaster
                    WHERE
	                    slm_dashjobcode = '{0}'
                    AND
	                    slm_clientjobno = '{1}'

                    SELECT Emt_CostCenterCode FROM T_EmployeeMaster where Emt_EmployeeId = '{2}'
                    ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, jobCode, jobNo, employeeId), CommandType.Text);
            }
            catch
            {
                //Do nothing
            }
            finally
            {
                dal.CloseDB();
            }
        }

        string retVal = string.Empty;
        if (!CommonMethods.isEmpty(ds))
        {
            retVal += ds.Tables[0].Rows[0]["slm_category"].ToString();
            retVal += "~";
            if (ds.Tables[0].Rows[0]["slm_costcenter"].ToString().ToUpper().Equals("ALL"))
                retVal += ds.Tables[1].Rows[0]["Emt_CostCenterCode"].ToString();
            else
                retVal += ds.Tables[0].Rows[0]["slm_costcenter"].ToString();
        }

        return retVal;
    }

    private string isLockedPopulate(bool flag)
    { 
        string temp = "0";
        if (flag)
            temp = "2";

        return temp;
    }

    private void showMessages()
    {
        if (Session["flagCheck"] != null)
        {
            switch (Session["flagCheck"].ToString().ToUpper())
            {
                case "JSSAVED":
                    MessageBox.Show("Manhour Allocation entry saved.");
                    Session["flagCheck"] = string.Empty;
                    break;
                case "JSERROR":
                    MessageBox.Show("Saving Manhour Allocation entry unsuccessful.");
                    Session["flagCheck"] = string.Empty;
                    break;
                case "JSSUBMIT":
                    MessageBox.Show("Manhour Allocation entry successfully submitted.");
                    Session["flagCheck"] = string.Empty;
                    break;
                case "JSSUBMITTEDTODAY":
                    MessageBox.Show("You already submitted a manhour allocation for this day.");
                    Session["flagCheck"] = string.Empty;
                    break;
                case "JSLOCKED":
                    MessageBox.Show("Manhour Allocation date is locked.");
                    Session["flagCheck"] = string.Empty;
                    break;
                case "SYSGENERROR":
                    MessageBox.Show("Error in creating new record, Manhour Allocation record for the day already exists.");
                    Session["flagCheck"] = string.Empty;
                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}
