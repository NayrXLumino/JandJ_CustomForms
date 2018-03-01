using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using MethodsLibrary;

public partial class _Default : System.Web.UI.Page
{
    #region [Class Variables]
    MenuGrant MGBL = new MenuGrant();
    DataSet dsUser = new DataSet();
    DataSet dsShift = new DataSet();
    DataSet dsLogLedger = new DataSet();
    DataSet dsSplit = new DataSet();
    string strShift = string.Empty;
    DataSet dsJobs = new DataSet();
    string DayCode = string.Empty;
    private JobSplitBL JSBL = new JobSplitBL();
    string flagEntry = string.Empty;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFJOBSPLTMOD"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            LoadComplete += new EventHandler(_Default_LoadComplete);
            hiddenType.Value = "mod";
            hfMaxHours.Value = Methods.GetParameterValue("MAXOTHR").ToString();
            
            if (!Page.IsPostBack)
            {
                if (Session["transaction"].ToString().Equals("CHECKLIST"))
                {
                    Calendar1.Enabled = false;
                    LoadJobSplitEntry();
                    Session["transaction"] = string.Empty;
                    hiddenRoute.Value = "1";
                    if (Request.QueryString["cn"] != null)
                        MessageBox.Show("Transaction loaded from checklist");
                    Session["flagSave"] = "SAVED";
                    Session["flagCheck"] = "SAVED";
                }
                else if (Session["transaction"].ToString().Equals("PENDING"))
                {
                    Calendar1.Enabled = true;
                    LoadJobSplitEntry();
                    Session["transaction"] = string.Empty;
                    MessageBox.Show("Transaction loaded from pending transactions.");
                    Session["flagSave"] = "SAVED";
                    Session["flagCheck"] = "SAVED";
                }
                else
                {
                    Calendar1.Enabled = true;

                    txtEmployeeId.Text = Session["userLogged"].ToString();
                    if (!(Convert.ToBoolean(Session["postBack"])))
                    {
                        txtEmployeeId.Text = Convert.ToString(Session["userId"].ToString());
                    }

                    InitializeUser();
                    InitializeControls();
                    InitializeValues();
                    btnViewPrevious.Visible = false;

                    //Loads the jobsplit for today
                    if ((Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) == DateTime.Now.Date || Calendar1.SelectedDate.ToString("MM/dd/yyyy").Equals("01/01/0001")))
                    {
                        Calendar1.SelectedDate = DateTime.Now;
                        Calendar1_SelectionChanged(null, null);
                    }
                }

                //hfMinHours.Value = CommonMethods.getMINOTHR(txtEmployeeId.Text);
                hfMinHours.Value = "0";

            }
            #region Catches the value of Session["flagCheck"] to do things on redirect page.
            if (Session["flagCheck"] != null)
            {
                switch (Session["flagCheck"].ToString().ToUpper())
                {
                    case "ENDORSE":
                        MessageBox.Show("Successfully endorsed transaction.");
                        Session["flagSave"] = string.Empty;
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "APPROVE":
                        MessageBox.Show("Successfully approved transaction.");
                        Session["flagSave"] = string.Empty;
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "SAVE":
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "RETURN":
                        MessageBox.Show("Transaction returned to employee");
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "CANCEL":
                        MessageBox.Show("Transaction cancelled.");
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "DISAPPROVE":
                        MessageBox.Show("Transaction disapproved.");
                        Session["flagCheck"] = string.Empty;
                        break;
                    case "CLEAR":
                        InitializeUser();
                        InitializeControls();
                        InitializeValues();

                        Session["flagCheck"] = string.Empty;
                        break;
                    case "LOADJS":
                        LoadJobSplitEntry();
                        //hasJobSplit();
                        //LoadJobSplitEntry(); //ilad2
                        hiddenRoute.Value = "1";
                        if (Request.QueryString["cn"] != null)
                            MessageBox.Show("Transaction loaded from checklist");
                        Session["flagSave"] = "SAVED";
                        Session["flagCheck"] = "SAVED";
                        break;
                    case "LOADJSP":
                        LoadJobSplitEntry();
                        hasJobSplit();
                        LoadJobSplitEntry();//ilad2
                        hiddenRoute.Value = "1";
                        MessageBox.Show("Transaction loaded from pending transactions.");
                        Session["flagSave"] = "SAVED";
                        Session["flagCheck"] = "SAVED";
                        break;
                    default:
                        break;
                }
            }
            #endregion
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

        btnJob.OnClientClick = string.Format("javascript:return lookupJSJobCode()");
        btnJob.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnViewPrevious.Attributes.Add("OnClick", "javascript:return OpenPopupPrevJobSplit(getElementById('ctl00_ContentPlaceHolder1_hfPrevCNumber').value);");
        txtJHours.Attributes.Add("OnKeyUp", "javascript:autoEndtime();");
        txtJStart.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJEnd.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        txtJHours.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        //txtJStart.Attributes.Add("OnKeyUp", "javascript:formatCompute();");
        txtJStart.Attributes.Add("OnKeyUp", "javascript:autoEndtime();");
        txtJEnd.Attributes.Add("OnKeyUp", "javascript:formatCompute();");
        txtJHours.Attributes.Add("OnBlur", "javascript:formatCompute();");

        btnSubWork.Attributes.Add("OnFocus", "javascript:setPosition(event);");
        btnSubWork.Attributes.Add("OnClick", "javascript:return lookupJSSubWork();");
        //chkBillable.Attributes.Add("OnClick", "javascript:setValueCheck(event,this);");
        chkOvertime.Attributes.Add("OnClick", "javascript:setValueCheck(event,this);");
        rblBillable.Attributes.Add("OnClick", "javascript:setValueRadio(event,this);");
    }
    #endregion

    #region Initialize dsUser for the current user
    protected void InitializeUser()
    {
        string userId = txtEmployeeId.Text;
        FillUserDS(userId);

        lblUserIdCode.Text = Resources.Resource._3RDINFO;

        if (!isInMaster(userId))
        {
            txtEmployeeId.Text = dsUser.Tables[0].Rows[0]["emt_employeeid"].ToString();
            txtEmployeeName.Text = dsUser.Tables[0].Rows[0]["emt_lastname"].ToString() + ", " + dsUser.Tables[0].Rows[0]["emt_firstname"].ToString();
            txtNickname.Text = dsUser.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
            hiddenShift.Value = dsUser.Tables[0].Rows[0]["emt_shiftcode"].ToString();
            hfEmployeeCostCenter.Value = dsUser.Tables[0].Rows[0]["Emt_CostCenterCode"].ToString();
            //GetShifts();
        }

        Session["postBack"] = false;

    }
    #endregion

    #region Initialize dsShift for the current user
    protected void GetShifts()
    {
        if (!isInMaster(txtEmployeeId.Text))
        {
            string shiftCode = hiddenShift.Value;
            string[] array = new string[2];
            array[0] = txtDate.Text;
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
                                                            , case when ell_actualtimein_1 = '0000' then '' else left(ell_actualtimein_1, 2) + ':' + right(ell_actualtimein_1, 2) end as ell_actualtimein_1
                                                            , case when ell_actualtimeout_1 = '0000' then '' else left(ell_actualtimeout_1, 2) + ':' + right(ell_actualtimeout_1, 2) end as ell_actualtimeout_1
                                                            , case when ell_actualtimein_2 = '0000' then '' else left(ell_actualtimein_2, 2) + ':' + right(ell_actualtimein_2, 2) end as ell_actualtimein_2
                                                            , case when ell_actualtimeout_2 = '0000' then '' else left(ell_actualtimeout_2, 2) + ':' + right(ell_actualtimeout_2, 2) end as ell_actualtimeout_2

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
                                                            , case when ell_actualtimein_1 = '0000' then '' else left(ell_actualtimein_1, 2) + ':' + right(ell_actualtimein_1, 2) end as ell_actualtimein_1
                                                            , case when ell_actualtimeout_1 = '0000' then '' else left(ell_actualtimeout_1, 2) + ':' + right(ell_actualtimeout_1, 2) end as ell_actualtimeout_1
                                                            , case when ell_actualtimein_2 = '0000' then '' else left(ell_actualtimein_2, 2) + ':' + right(ell_actualtimein_2, 2) end as ell_actualtimein_2
                                                            , case when ell_actualtimeout_2 = '0000' then '' else left(ell_actualtimeout_2, 2) + ':' + right(ell_actualtimeout_2, 2) end as ell_actualtimeout_2

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

                txtDay.Text = DayCode;
                txtShift.Text = strShift;
                if (DayCode == "REST")
                    txtShift.ForeColor = System.Drawing.Color.WhiteSmoke;
                else
                    txtShift.ForeColor = System.Drawing.Color.Black;

                #region Data from logledger
                string sTime = string.Empty;
                string eTime = string.Empty;
                try
                {
                    this.txtTimeIn1.Text = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimein_1"]);
                    this.txtTimeOut1.Text = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimeout_1"]);
                    this.txtTimeIn2.Text = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimein_2"]);
                    this.txtTimeOut2.Text = Convert.ToString(dsLogLedger.Tables[0].Rows[0]["ell_actualtimeout_2"]);

                    if (!dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavetype"].ToString().Equals(string.Empty))
                    {
                        txtPLeave.Text = dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavetype"].ToString() + dsLogLedger.Tables[0].Rows[0]["ell_encodedpayleavehr"].ToString();
                    }
                    else
                    {
                        txtPLeave.Text = string.Empty;
                    }

                    if (!dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavetype"].ToString().Equals(string.Empty))
                    {
                        txtNPLeave.Text = dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavetype"].ToString() + dsLogLedger.Tables[0].Rows[0]["ell_encodednopayleavehr"].ToString();
                    }
                    else
                    {
                        txtNPLeave.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    //do nothing
                }
                #endregion
            }
            catch (Exception ex)
            {
                //MessageBox.Show("No shift retrieved.");
                txtShift.Text = string.Empty;
                txtDay.Text = string.Empty;
                txtTimeIn1.Text = string.Empty;
                txtTimeOut1.Text = string.Empty;
                txtTimeIn2.Text = string.Empty;
                txtTimeOut2.Text = string.Empty;
                txtPLeave.Text = string.Empty;
                txtNPLeave.Text = string.Empty;
            }
            finally
            {
                txtDOW.Text = Convert.ToDateTime(txtDate.Text).DayOfWeek.ToString().ToUpper();
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
                                        FROM
                                            t_employeemaster
                                        LEFT JOIN T_DepartmentCodeMaster
                                          ON Dcm_DepartmentCode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
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

                if (dsUser.Tables[0].Rows.Count == 0)
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

    protected void InitializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "TIMEKEEP", "WFJOBSPLTMOD");
        btnEmployeeId.Enabled = userGrant.canAdd();

        enableControls();
    }

    private void enableControls()
    {
        switch (txtStatus.Text.ToUpper().Trim())
        {
            case "":
                pnlRecent.Visible = true;
                //pnlInfo.Visible = false;
                txtRemarks.ReadOnly = true;
                txtRemarks.BackColor = System.Drawing.Color.WhiteSmoke;

                btnX.Text = "Endorse to Checker 1";
                btnY.Text = "Clear";
                btnZ.Text = "Save";
                break;
            case "NEW":
                pnlRecent.Visible = true;
                //pnlInfo.Visible = false;
                txtRemarks.ReadOnly = true;
                txtRemarks.BackColor = System.Drawing.Color.WhiteSmoke;

                btnX.Text = "Endorse to Checker 1";
                btnY.Text = "Cancel";
                btnZ.Text = "Save";
                break;
            case "ENDORSED TO CHECKER 1":
                pnlRecent.Visible = false;
                //pnlInfo.Visible = true;
                txtRemarks.ReadOnly = false;
                txtRemarks.BackColor = System.Drawing.Color.White;

                btnX.Text = "Endorse to Checker 2";
                btnY.Text = "Disapprove";
                btnZ.Text = "Return to Employee";
                break;
            case "ENDORSED TO CHECKER 2":
                pnlRecent.Visible = false;
                //pnlInfo.Visible = true;
                txtRemarks.ReadOnly = false;
                txtRemarks.BackColor = System.Drawing.Color.White;

                btnX.Text = "Endorse to Approver";
                btnY.Text = "Disapprove";
                btnZ.Text = "Return to Employee";
                break;
            case "ENDORSED TO APPROVER":
                pnlRecent.Visible = false;
                //pnlInfo.Visible = true;
                txtRemarks.ReadOnly = false;
                txtRemarks.BackColor = System.Drawing.Color.White;

                btnX.Text = "Approve";
                btnY.Text = "Disapprove";
                btnZ.Text = "Return to Employee";
                break;
        }
    }

    #region Initialize the values of the controls
    protected void InitializeValues()
    {
            hiddenMonth.Value = DateTime.Now.Month.ToString("MM");
            hfOTFRAC.Value = Convert.ToString(Methods.GetParameterValue("OTFRACTION"));

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
                    dsJobs = dal.ExecuteDataSet(string.Format(sqlRJobs, txtEmployeeId.Text, ConfigurationManager.AppSettings["ERP_DB"]), CommandType.Text);
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

            listBox.Items.Clear();
            if (dsJobs.Tables.Count > 0)//Means that there were data retrieved
            {

                for (int ctr = 0; ctr < dsJobs.Tables[0].Rows.Count; ctr++)
                {
                    val1 = dsJobs.Tables[0].Rows[ctr]["Jsd_jobCode"].ToString().Trim();
                    val2 = dsJobs.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString().Trim();
                    val3 = dsJobs.Tables[0].Rows[ctr]["Slm_ClientJobName"].ToString().Trim();
                    val4 = dsJobs.Tables[0].Rows[ctr]["Jsd_SubWorkCode"].ToString().Trim();
                    val5 = dsJobs.Tables[0].Rows[ctr]["Swc_Description"].ToString().Trim();
                    listValue = getCostCenterAndCategoryV2(val1.Trim(), val2.Trim(), txtEmployeeId.Text);
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
    #endregion

    #region [Events]
    #region EmployeeId textbox is changed
    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        Calendar1.Enabled = true;

        InitializeUser();
        InitializeControls();
        btnViewPrevious.Visible = false;

        InitializeValues();

        //Calendar1.SelectedDate = DateTime.Now;
        Calendar1_SelectionChanged(null, null);

        hfMinHours.Value = "0";
    }
    #endregion

    #region Button Z event
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
        string jobcode2 = txtJJobCode.Text + ",";
        string subwork2 = txtSubWork.Text + ",";
        string cFlag = hCFlag.Value + ",";

        string[] VALUES = new string[4];
        string valid = string.Empty;
        string tValid = string.Empty;
        string lockValid = string.Empty;
        string overlap = string.Empty;
        string holdPrevTime = string.Empty;
        decimal OTFRAC = Convert.ToDecimal(hfOTFRAC.Value);
        try
        {
            while (start.Length > 1 && valid.Equals(string.Empty))
            {
                //Gets the data in order
                VALUES[0] = start.Substring(0, start.IndexOf(",")).Replace(":", "");
                VALUES[1] = end.Substring(0, end.IndexOf(",")).Replace(":", "");

                if (overlap.Equals(string.Empty) && !holdPrevTime.Equals(string.Empty) && checkOverLap(holdPrevTime, VALUES[0]))
                {
                    overlap = "\n Time overlap in entry.";
                }
                holdPrevTime = VALUES[1];
                VALUES[2] = jobcode2.Substring(0, jobcode2.IndexOf(",")).Replace(":", "");
                VALUES[3] = subwork2.Substring(0, subwork2.IndexOf(",")).Replace(":", "");
                if (VALUES[2].ToString().Trim().Equals(string.Empty) || VALUES[3].ToString().Trim().Equals(string.Empty))
                    valid += @"\n Invalid Entry on Job / Subwork";
                if (VALUES[0].ToString().Length != 4 || VALUES[1].ToString().Length != 4)
                    valid += @"\n Invalid End/Start Time Entry. Please Review Transaction.";
                //OTFRACTION CHECK ON HOURS
                if (!valid.Equals(string.Empty) && Convert.ToDecimal(VALUES[0].Substring(2, 2)) % OTFRAC != 0 || Convert.ToDecimal(VALUES[1].Substring(2, 2)) % OTFRAC != 0)
                {
                    tValid = " Invalid time entry in form. System only allows minutes for every " + OTFRAC.ToString();
                }


                start = start.Substring(start.IndexOf(",") + 1);
                end = end.Substring(end.IndexOf(",") + 1);
                jobcode2 = jobcode2.Substring(jobcode2.IndexOf(",") + 1);
                subwork2 = subwork2.Substring(subwork2.IndexOf(",") + 1);
            }

            //Andre added: 20091205 for the cost center bill cycle locking
            string holdChar = string.Empty;

            DataTable tempTable = new DataTable();
            //bool hasLockJobs = false;
            //int insertCol = -1;
            string billCycTmp = string.Empty;
            string tempHeader = string.Empty;
            DataTable dt = new DataTable();
            dt.Columns.Add("cycle");
            dt.Columns.Add("jobcode");
            dt.Columns.Add("clientcode");

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
                    if(!tempHeader.Equals(string.Empty))
                        dt.Rows.Add(new object[] { tempHeader, VALUES[0].Trim(), VALUES[1].Trim() });
                    //valid += @"\n Billing Cycle for " + VALUES[0] + "~" + VALUES[1] + " is locked";
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
            if (txtDay.Text != "REG" && Convert.ToDecimal(txtTotalHours.Text) > Methods.GetParameterValue("MAXOTHR"))
            {
                valid += "\n Invalid number of hours. Total hours must not exceed [" + hfMaxHours.Value + "].";
            }

            if (txtJStart.Text.Trim().Equals(1))
            {
                valid += "\n Invalid entry. Supply neccessary information.";
            }
        }
        catch
        {
            valid += "Saving was unsuccessful.";
        }
        return valid + tValid + lockValid + overlap;
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
    private bool isJobLocked(string splitDate, string jobCode, string clientCode)
    {
        bool flag = true;
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

    protected void btnZ_Click(object sender, EventArgs e)
    {
        if (!Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            string errorMessage = validateEntry();
            if (errorMessage.Equals(string.Empty))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();

                        if (hiddenControl.Value != string.Empty)
                        {
                            dsSplit = JSBL.LoadCurrentJobSplitDetails(hiddenControl.Value);
                            hasJob_Split.Value = dsSplit.Tables[0].Rows.Count.ToString();
                        }

                        switch (btnZ.Text.Trim().ToUpper())
                        {
                            case "SAVE":
                                string start = txtJStart.Text + ",";
                                string end = txtJEnd.Text + ",";
                                string hour = txtJHours.Text + ",";
                                string jobcode = txtJJobCode.Text + ",";
                                string clientno = txtJClientNo.Text + ",";
                                string clientname = txtJClientName.Text + ",";
                                string subworkcode = txtSubWork.Text + ",";
                                string category = hCat.Value + ",";
                                string overtime = hOvertime.Value + ",";
                                string costcenter = hCCT.Value + ",";
                                string[] VALUES = new string[10];

                                //if (Session["flagSave"].ToString().ToUpper() != "SAVED" || hfFlagEntry.Value.Equals("N"))//OLD CODE
                                if (Session["flagSave"].ToString().ToUpper() != "SAVED")//TRUE if no records was retrive
                                {
                                    #region Create New record for header and detail

                                    string controlNo = Methods.GetControlNumber("JOBMOD");
                                    hiddenControl.Value = controlNo;

                                    //Create Header
                                    //DataRow dr = PopulateModHeader("1", controlNo, hfFlagEntry.Value);
                                    string myFlagEntry = string.Empty;
                                    if (hasJob_Split.Value.Equals("0") || hasJob_Split.Value.Equals(string.Empty))
                                        myFlagEntry = "N";
                                    else
                                        myFlagEntry = "C";

                                    DataRow dr = PopulateModHeader("1", controlNo, myFlagEntry);
                                    JSBL.CreateJSHeader(dr, dal);
                                    //Insert Details
                                    int ctr = 1;
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
                                        VALUES[6] = subworkcode.Substring(0, subworkcode.IndexOf(",")).Trim();
                                        VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                        VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                        VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                        //Andre added foe checking of empty parameters
                                        if (isNotValidParameter(VALUES))
                                        {
                                            Session["flagCheck"] = "JSERROR";
                                            break;
                                        }
                                        //End

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
                                        subworkcode = subworkcode.Substring(subworkcode.IndexOf(",") + 1);
                                        category = category.Substring(category.IndexOf(",") + 1);
                                        overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                        costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                                    }
                                    #endregion
                                    Session["flagCheck"] = "NSAVE";
                                    Session["flagSave"] = "SAVED";
                                    MessageBox.Show("New transaction saved."); //hasjob
                                }
                                else// if (hiddenCNumber.Value.Substring(0, 1).Equals("S"))
                                {
                                    #region Updating Job Split for the day
                                    string cn = hiddenControl.Value;
                                    hiddenControl.Value = cn;
                                    int prev = 0;
                                    int curr = 0;
                                    prev = Convert.ToInt32(hasJob_Split.Value.ToString().Equals(string.Empty) ? "0" : hasJob_Split.Value);//prev is set
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
                                        DataRow dr = PopulateDRHeader("1", cn);
                                        JSBL.UpdateJSHeader(dr, dal);
                                        //Insert Details
                                        int ctr = 1;
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
                                            VALUES[6] = subworkcode.Substring(0, subworkcode.IndexOf(",")).Trim();
                                            VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                            VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                            VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                            //Andre added foe checking of empty parameters
                                            if (isNotValidParameter(VALUES))
                                            {
                                                Session["flagCheck"] = "JSERROR";
                                                break;
                                            }
                                            //End

                                            if (ctr < 10)
                                                count = "0" + ctr.ToString();
                                            ctr++;
                                            DataRow drDetail = PopulateDRDetail("1", VALUES, cn, count);
                                            JSBL.UpdateDetails(drDetail, dal); 

                                            //Trimming the data
                                            start = start.Substring(start.IndexOf(",") + 1);
                                            end = end.Substring(end.IndexOf(",") + 1);
                                            hour = hour.Substring(hour.IndexOf(",") + 1);
                                            jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                            clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                            clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                            subworkcode = subworkcode.Substring(subworkcode.IndexOf(",") + 1);
                                            category = category.Substring(category.IndexOf(",") + 1);
                                            overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                            costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                                        }
                                    }
                                    #endregion

                                    #region Scenario 2(if previous count is less than current count):Update all previous and insert new rows
                                    if (prev < curr)
                                    {
                                        //Update Header
                                        DataRow dr = PopulateDRHeader("1", cn);
                                        JSBL.UpdateJSHeader(dr, dal);
                                        //Insert Details
                                        int ctr = 1;
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
                                            VALUES[6] = subworkcode.Substring(0, subworkcode.IndexOf(",")).Trim();
                                            VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                            VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                            VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                            //Andre added foe checking of empty parameters
                                            if (isNotValidParameter(VALUES))
                                            {
                                                Session["flagCheck"] = "JSERROR";
                                                break;
                                            }
                                            //End

                                            count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                            ctr++;
                                            DataRow drDetail = PopulateDRDetail("1", VALUES, cn, count); 
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
                                            subworkcode = subworkcode.Substring(subworkcode.IndexOf(",") + 1);
                                            category = category.Substring(category.IndexOf(",") + 1);
                                            overtime = overtime.Substring(overtime.IndexOf(",") + 1);
                                            costcenter = costcenter.Substring(costcenter.IndexOf(",") + 1);
                                        }
                                    }
                                    #endregion

                                    #region Scenario 3(if previous count is greater than current count):Update previous up to current count and delete the rest
                                    if (prev > curr)
                                    {
                                        //Update Header
                                        DataRow dr = PopulateDRHeader("1", cn);
                                        JSBL.UpdateJSHeader(dr, dal);
                                        //Insert Details
                                        int ctr = 1;
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
                                                VALUES[6] = subworkcode.Substring(0, subworkcode.IndexOf(",")).Trim();
                                                VALUES[7] = category.Substring(0, category.IndexOf(",")).Trim();
                                                VALUES[8] = overtime.Substring(0, overtime.IndexOf(",")).Trim();
                                                VALUES[9] = costcenter.Substring(0, costcenter.IndexOf(",")).Trim();

                                                //Andre added foe checking of empty parameters
                                                if (isNotValidParameter(VALUES))
                                                {
                                                    Session["flagCheck"] = "JSERROR";
                                                    break;
                                                }
                                                //End


                                                count = (ctr < 10) ? "0" + ctr.ToString() : ctr.ToString();

                                                ctr++;
                                                DataRow drDetail = PopulateDRDetail("1", VALUES, cn, count); 

                                                JSBL.UpdateDetails(drDetail, dal);

                                                //Trimming the data
                                                start = start.Substring(start.IndexOf(",") + 1);
                                                end = end.Substring(end.IndexOf(",") + 1);
                                                hour = hour.Substring(hour.IndexOf(",") + 1);
                                                jobcode = jobcode.Substring(jobcode.IndexOf(",") + 1);
                                                clientno = clientno.Substring(clientno.IndexOf(",") + 1);
                                                clientname = clientname.Substring(clientname.IndexOf(",") + 1);
                                                subworkcode = subworkcode.Substring(subworkcode.IndexOf(",") + 1);
                                                category = category.Substring(category.IndexOf(",") + 1);
                                                overtime = overtime.Substring(overtime.IndexOf(",") + 1);
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
                                    Session["flagCheck"] = "USAVE";
                                    MessageBox.Show("Transaction updated.");

                                }
                                txtStatus.Text = "New";
                                hiddenFlag.Value = "1";
                                btnX.Enabled = true;

                                btnY.Text = "Cancel";

                                // manipulate() of jobsplit.js requires these fields have comma in the end of their values
                                txtJStart.Text += ",";
                                txtJEnd.Text += ",";
                                txtJHours.Text += ",";
                                txtJJobCode.Text += ",";
                                txtJClientNo.Text += ",";
                                txtJClientName.Text += ",";
                                txtSubWork.Text += ",";
                                hCat.Value += ",";
                                hOvertime.Value += ",";
                                hCCT.Value += ",";

                                break;
                            case "RETURN TO EMPLOYEE":
                                if (!txtRemarks.Text.Trim().Equals(string.Empty))
                                {
                                    JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute("1", hiddenControl.Value, hfFlagEntry.Value), dal);
                                    JSBL.UpdateDetailsStatus("1", hiddenControl.Value, dal);
                                    JSBL.InsertUpdateRemarks(PopulateDRForRemarks(hiddenControl.Value), dal);
                                    if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                    {
                                        EmailNotificationBL EMBL = new EmailNotificationBL();
                                        EMBL.TransactionProperty = EmailNotificationBL.TransactionType.JOBMOD;
                                        EMBL.ActionProperty = EmailNotificationBL.Action.RETURN;
                                        EMBL.InsertInfoForNotification(hiddenControl.Value
                                                                      , Session["userLogged"].ToString()
                                                                      , dal);
                                    }

                                    Session["flagCheck"] = "RETURN";
                                }
                                else
                                {
                                    MessageBox.Show("Enter remarks for of action.");
                                }
                                break;
                            default:
                                break;
                        }
                        hfSave.Value = "0";
                        //MenuLog
                        SystemMenuLogBL.InsertAddLog("WFJOBSPLTMOD", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "", false);
                       dal.CommitTransactionSnapshot();
                    }
                    catch(Exception ex)
                    {
                         dal.RollBackTransactionSnapshot();
                        Session["flagCheck"] = "JSERROR";
                    }
                    finally
                    {
                        dal.CloseDB();

                        if (Session["flagCheck"].ToString().ToUpper() == "RETURN")
                            Response.Redirect("pgeJobSplitMod.aspx");
                    }
                }
            }
            else
            {
                MessageBox.Show(errorMessage);
                hasJobSplit();

            }
        }
        else
        {
            MessageBox.Show("Cycle Processing is currently being done.\nPlease try again later.");
        }
    }
    #endregion

    #region Button X event
    protected void btnX_Click(object sender, EventArgs e)
    {
        if (!Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
        {
            using (DALHelper dal = new DALHelper())
            {
                
                Methods m = new Methods();
                char statusVal = m.GetStatusX(btnX.Text.ToUpper(), Session["userId"].ToString(), "JOBMOD");
                EmailNotificationBL EMBL = new EmailNotificationBL();
                EMBL.TransactionProperty = EmailNotificationBL.TransactionType.JOBMOD;

                if (statusVal != ' ')
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();

                        switch (btnX.Text.ToUpper())
                        {
                            case "ENDORSE TO CHECKER 1":
                                JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal);
                                JSBL.UpdateDetailsStatus(statusVal.ToString(), hiddenControl.Value, dal);
                                EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                Session["flagCheck"] = "ENDORSE";
                                break;
                            case "ENDORSE TO CHECKER 2":
                                JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal);
                                JSBL.UpdateDetailsStatus(statusVal.ToString(), hiddenControl.Value, dal);
                                EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                Session["flagCheck"] = "ENDORSE";
                                break;
                            case "ENDORSE TO APPROVER":
                                JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal);
                                JSBL.UpdateDetailsStatus(statusVal.ToString(), hiddenControl.Value, dal);
                                EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                                Session["flagCheck"] = "ENDORSE";
                                break;
                            case "APPROVE":
                                JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal);
                                JSBL.UpdateDetailsStatus(statusVal.ToString(), hiddenControl.Value, dal);
                                string cnNum = JSBL.GetRefControlNumber(hiddenControl.Value, dal);
                                JSBL.UpdateRefHeader(PopulateModHeaderRoute("2", cnNum, "C"), dal);
                                JSBL.UpdateDetailsStatus("2", cnNum, dal);
                                EMBL.ActionProperty = EmailNotificationBL.Action.APPROVE;
                                Session["flagCheck"] = "APPROVE";
                                break;
                            default:
                                break;
                        }

                        //switch (statusVal)
                        //{
                        //    case '3':
                        //        JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal,btnX.Text.ToUpper());
                        //        JSBL.UpdateDetailsStatus("3", hiddenControl.Value, dal);
                        //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                        //        Session["flagCheck"] = "ENDORSE";
                        //        break;
                        //    case '5':
                        //        JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal, btnX.Text.ToUpper());
                        //        JSBL.UpdateDetailsStatus("5", hiddenControl.Value, dal);
                        //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                        //        Session["flagCheck"] = "ENDORSE";
                        //        break;
                        //    case '7':
                        //        JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal, btnX.Text.ToUpper());
                        //        JSBL.UpdateDetailsStatus("7", hiddenControl.Value, dal);
                        //        EMBL.ActionProperty = EmailNotificationBL.Action.ENDORSE;
                        //        Session["flagCheck"] = "ENDORSE";
                        //        break;
                        //    case '9':
                        //        JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(statusVal.ToString(), hiddenControl.Value, hfFlagEntry.Value), dal, btnX.Text.ToUpper());
                        //        JSBL.UpdateDetailsStatus("9", hiddenControl.Value, dal);
                        //        string cnNum = JSBL.GetRefControlNumber(hiddenControl.Value, dal);
                        //        JSBL.UpdateRefHeader(PopulateModHeaderRoute("2", cnNum, "C"), dal);
                        //        JSBL.UpdateDetailsStatus("2", cnNum, dal);
                        //        EMBL.ActionProperty = EmailNotificationBL.Action.APPROVE;
                        //        Session["flagCheck"] = "APPROVE";
                        //        break;
                        //    default:
                        //        break;
                        //}

                        if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                        {
                            EMBL.InsertInfoForNotification(hiddenControl.Value
                                                            , Session["userLogged"].ToString()
                                                            , dal);
                        }
                        //MenuLog
                        SystemMenuLogBL.InsertEditLog("WFJOBSPLTMOD", true, txtEmployeeId.Text.ToString().ToUpper(), Session["userLogged"].ToString(), "",false);
                        dal.CommitTransactionSnapshot();
                    }
                    catch (Exception ex)
                    {
                        dal.RollBackTransactionSnapshot();
                    }
                    finally
                    {
                        dal.CloseDB();
                        Session["flagSave"] = string.Empty;
                        Session["userId"] = Session["userLogged"].ToString();
                        Response.Redirect("pgeJobSplitMod.aspx");
                    }
                }
                else
                {
                    // message already in 
                    //MessageBox.Show("No route defined for user.");
                }
            }
        }
        else
        {
            MessageBox.Show("Cycle Processing is currently being done.\nPlease try again later.");
        }
    }
    #endregion

    #region Button Y event
    protected void btnY_Click(object sender, EventArgs e)
    {
        if (!Methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF") || btnY.Text.ToUpper().Equals("CLEAR"))
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                   dal.BeginTransactionSnapshot();
                    switch (btnY.Text.ToUpper())
                    {
                        case "CLEAR":
                            Session["flagCheck"] = "CLEAR";
                            txtJStart.Text = "";
                            txtJEnd.Text = "";
                            txtJHours.Text = "";
                            txtJJobCode.Text = "";
                            txtJClientNo.Text = "";
                            txtJClientName.Text = "";
                            txtSubWork.Text = "";
                            hBillable.Value = "";
                            hOvertime.Value = "";
                            break;
                        case "CANCEL":
                            JSBL.UpdateModHeaderRoute(PopulateModHeader("2", hiddenControl.Value, "N"), dal);
                            JSBL.UpdateDetailsStatus("2", hiddenControl.Value, dal);
                            Session["flagCheck"] = "CANCEL";
                            break;
                        case "DISAPPROVE":
                            string disApproveStatus = string.Empty;
                            switch (txtStatus.Text.ToUpper())
                            {
                                case "ENDORSED TO CHECKER 1":
                                    disApproveStatus = "4";
                                    break;
                                case "ENDORSED TO CHECKER 2":
                                    disApproveStatus = "6";
                                    break;
                                case "ENDORSED TO APPROVER":
                                    disApproveStatus = "8";
                                    break;
                                default:
                                    break;
                            }
                            if (!txtRemarks.Text.Trim().Equals(string.Empty))
                            {
                                JSBL.UpdateModHeaderRoute(PopulateModHeaderRoute(disApproveStatus, hiddenControl.Value, "N"), dal);
                                JSBL.UpdateDetailsStatus(disApproveStatus, hiddenControl.Value, dal);
                                JSBL.InsertUpdateRemarks(PopulateDRForRemarks(hiddenControl.Value), dal);
                                Session["flagCheck"] = "DISAPPROVE";
                                if (Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
                                {
                                    EmailNotificationBL EMBL = new EmailNotificationBL();
                                    EMBL.TransactionProperty = EmailNotificationBL.TransactionType.JOBMOD;
                                    EMBL.ActionProperty = EmailNotificationBL.Action.DISAPPROVE;
                                    EMBL.InsertInfoForNotification(hiddenControl.Value
                                                                  , Session["userLogged"].ToString()
                                                                  , dal);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Enter remarks of disapproval.");
                            }
                            break;
                        default:
                            break;
                    }

                   dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                    if (Session["flagCheck"].ToString().ToUpper() == "CANCEL" || Session["flagCheck"].ToString().ToUpper() == "DISAPPROVE")
                    {
                        Session["userId"] = Session["userLogged"].ToString();
                        Response.Redirect("pgeJobSplitMod.aspx");
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("Cycle Processing is currently being done.\nPlease try again later.");
        }
    }
    #endregion

    #region Calendar event
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        
        txtDate.Text = Calendar1.SelectedDate.ToString("MM/dd/yyyy");
        
        txtJStart.Text = string.Empty;
        txtJEnd.Text = string.Empty;
        txtJHours.Text = string.Empty;
        txtJJobCode.Text = string.Empty;
        txtJClientNo.Text = string.Empty;
        txtJClientName.Text = string.Empty;
        txtSubWork.Text = string.Empty;
        Session["flagSave"] = string.Empty;
        hfSave.Value = "1";
        hiddenControl.Value = hiddenCNumber.Value = string.Empty;
        Methods m = new Methods();

        //Kelvin commented code below: 20110414 - To allow modification of jobsplit on the current date(today)
        //if (Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) == DateTime.Now.Date)
        //{
            
        //    hiddenDFlag.Value = "X";
        //    MessageBox.Show("Modification not allowed for this day.\n Use Manhour Allocation entry form.");

        //}
        //else
        if (Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) > DateTime.Now.Date)
        {
            
            hiddenDFlag.Value = "X";
            MessageBox.Show("Advance Manhour Modification is not allowed");
        }
        //else if (Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) < DateTime.Now.AddDays(-(Convert.ToDouble(m.GetParameter("JOBMODLOCK")))))
        //{
        //    hiddenDFlag.Value = "X";
        //    MessageBox.Show("Job Split date is locked.");
        //}
        else if (JSBL.CheckBillingLock(Calendar1.SelectedDate.ToString("MM/dd/yyyy")))
        {
            hiddenDFlag.Value = "X";
            MessageBox.Show("Manhour Allocation date is locked.");
        }
        else
        {
            txtDate.Text = Calendar1.SelectedDate.ToString("MM/dd/yyyy");
            hiddenDFlag.Value = "0";
            try
            {
                GetShifts();
                hasJobSplit();
            }
            catch 
            {

            }

        }
        
    }
    #endregion
    #endregion

    #region [Saving]
    #region Populate data row for header
    private DataRow PopulateDRHeader(string Status, string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitHeader");
        dr["Jsh_ControlNo"] = ControlNum.ToUpper();
        dr["Jsh_EmployeeId"] = txtEmployeeId.Text.ToString();
        dr["Jsh_JobSplitDate"] = Convert.ToDateTime(txtDate.Text);
        dr["Jsh_Entry"] = "N";
        dr["Jsh_Costcenter"] = string.Empty;
        dr["Jsh_RefControlNo"] = string.Empty;
        dr["Jsh_Status"] = Status;
        dr["Usr_Login"] = Session["userLogged"].ToString().ToUpper();
        return dr;
    }

    private DataRow PopulateModHeader(string Status, string ControlNum, string Entry)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitHeader");
        dr["Jsh_ControlNo"] = ControlNum;
        dr["Jsh_EmployeeId"] = txtEmployeeId.Text.ToString().ToUpper();
        dr["Jsh_JobSplitDate"] = Convert.ToDateTime(txtDate.Text);
        dr["Jsh_Entry"] = Entry;
        dr["Jsh_Costcenter"] = string.Empty;
        dr["Jsh_CheckedBy"] = Session["userLogged"].ToString();
        dr["Jsh_Checked2By"] = Session["userLogged"].ToString();
        dr["Jsh_ApprovedBy"] = Session["userLogged"].ToString();

        //if (dsSplit.Tables.Count > 0)
        if (Convert.ToInt32(hasJob_Split.Value.Equals(string.Empty) ? "0" : hasJob_Split.Value) > 0)
            dr["Jsh_RefControlNo"] = hiddenCNumber.Value.ToUpper();
        else
            dr["Jsh_RefControlNo"] = string.Empty;

        dr["Jsh_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        return dr;
    }

    private DataRow PopulateModHeaderRoute(string Status, string ControlNum, string Entry)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitHeader");
        dr["Jsh_ControlNo"] = ControlNum;
        dr["Jsh_EmployeeId"] = txtEmployeeId.Text.ToString();
        dr["Jsh_JobSplitDate"] = Convert.ToDateTime(txtDate.Text);
        dr["Jsh_Entry"] = Entry;
        dr["Jsh_Costcenter"] = string.Empty;
        dr["Jsh_CheckedBy"] = Session["userLogged"].ToString();
        dr["Jsh_Checked2By"] = Session["userLogged"].ToString();
        dr["Jsh_ApprovedBy"] = Session["userLogged"].ToString();
        dr["Jsh_Status"] = Status.ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        //if (Status == "3")
        //{
        //    //dr["Jsh_EndorsedDateToChecker"] = DateTime.Now;
        //}
        //else if (Status == "5")
        //{
        //    dr["Jsh_CheckedBy"] = Session["userLogged"].ToString();
        //    //dr["Jsh_CheckedDate"] = DateTime.Now;
        //}
        //else if (Status == "7")
        //{
        //    dr["Jsh_Checked2By"] = Session["userLogged"].ToString();
        //    //dr["Jsh_Checked2Date"] = DateTime.Now;
        //}
        //else if (Status == "9")
        //{
        //    dr["Jsh_ApprovedBy"] = Session["userLogged"].ToString();
        //    //dr["Jsh_ApprovedDate"] = DateTime.Now;
        //}
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
        dr["Jsd_ControlNo"] = ControlNum;
        dr["Jsd_Seqno"] = counter;
        dr["Jsd_StartTime"] = values[0].Replace(":", "");
        dr["Jsd_EndTime"] = values[1].Replace(":", "");
        dr["Jsd_JobCode"] = values[3].Trim();
        dr["Jsd_ClientJobNo"] = values[4].Trim();
        dr["Jsd_PlanHours"] = values[2];
        dr["Jsd_ActHours"] = 0;
        dr["Jsd_Status"] = Status.ToUpper().Trim();
        dr["Usr_Login"] = Session["userLogged"].ToString().Trim();
        dr["Jsd_SubWorkCode"] = values[6].ToUpper().Trim();
        dr["Jsd_Category"] = values[7];
        dr["Jsd_Overtime"] = Convert.ToBoolean(values[8]);
        dr["Jsd_CostCenter"] = values[9];
        return dr;
    }

    private DataRow PopulateDRDelete(string ControlNum, string counter)
    {
        DataRow dr = DbRecord.Generate("T_JobSplitDetail");
        dr["Jsd_ControlNo"] = ControlNum.ToUpper();
        dr["Jsd_Seqno"] = counter;
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

    #region Populate data row for remarks
    private DataRow PopulateDRForRemarks(string ControlNum)
    {
        DataRow dr = DbRecord.Generate("T_TransactionRemarks");

        dr["Trm_ControlNo"] = ControlNum;
        dr["Trm_Remarks"] = txtRemarks.Text.ToString().ToUpper();
        dr["Usr_Login"] = Session["userLogged"].ToString();
        return dr;
    }
    #endregion

    #endregion

    #region [Other Methods]
    #region Gets the Job Split Entry for the day if any
    protected void hasJobSplit()
    {
        string x = string.Empty;
        string y = string.Empty;

        txtJStart.Text = string.Empty;
        txtJEnd.Text = string.Empty;
        txtJHours.Text = string.Empty;
        txtJJobCode.Text = string.Empty;
        txtJClientNo.Text = string.Empty;
        txtJClientName.Text = string.Empty;
        txtSubWork.Text = string.Empty;
        //Added by Manny 12/21/2010
        hBillable.Value = string.Empty;
        hOvertime.Value = string.Empty;

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsSplit.Clear();
                //Start logic for getting the jobsplit for the modification

                if (x == "")
                {
                    x = JSBL.GetControlNumber(txtDate.Text, txtEmployeeId.Text, dal);
                }
                if (x.Trim() != string.Empty)
                {
                    y = JSBL.GetControlNumber(x, dal);
                    if (y.Trim() != string.Empty)
                    {
                        while (y.Trim() != "")
                        {
                            x = y;
                            y = JSBL.GetControlNumber(x, dal);
                        }
                    }
                }
                else
                {
                    x = JSBL.GetControlNumberRouted(txtDate.Text, txtEmployeeId.Text, dal);
                }

                DataRow dr = null;
                if (x != string.Empty)
                {
                    dr = JSBL.GetJobSplitHeaderInfo(x);
                    dsSplit = JSBL.LoadCurrentJobSplitDetails(x, dal);
                    hasJob_Split.Value = dsSplit.Tables[0].Rows.Count.ToString();
                }

                if (Convert.ToInt32(hasJob_Split.Value) > 0)
                {
                    hiddenCNumber.Value = dsSplit.Tables[0].Rows[0]["Jsh_ControlNo"].ToString();
                    hiddenDFlag.Value = dsSplit.Tables[0].Rows[0]["Jsh_Status"].ToString();
                }
                else
                {
                    hiddenDFlag.Value = "0";
                }

                if (Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) == DateTime.Now.Date && dr["Jsh_Status"].ToString() != "9" 
                    && dr["Jsh_ControlNo"].ToString().StartsWith("J"))
                {
                    hiddenDFlag.Value = "X";
                }
                
                this.hfPrevCNumber.Value = dr["Jsh_RefControlNo"].ToString();
                txtRemarks.Text = dr["Trm_Remarks"].ToString();
            }
            catch (Exception ex)
            {
                hasJob_Split.Value = "0";

                this.hfPrevCNumber.Value = "";
                hiddenFlag.Value = "";

                //If no jobsplit and date is not today then enty in manhour modification 
                if (Convert.ToDateTime(Calendar1.SelectedDate.ToString("MM/dd/yyyy")) == DateTime.Now.Date || Calendar1.SelectedDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                    hiddenDFlag.Value = "X";
                else
                    hiddenDFlag.Value = "0";

            }
            finally
            {
                dal.CloseDB();
            }
            if (this.hfPrevCNumber.Value.Equals(string.Empty))
            {
                this.btnViewPrevious.Visible = false;
            }
            else
            {
                this.btnViewPrevious.Visible = true;
            }
        }
        try
        {
            if (dsSplit.Tables != null && dsSplit.Tables[0].Rows.Count > 0)
            {
                hfFlagEntry.Value = "C";//Used for saving the Modification Entry. C - changed or N - new
                string v1 = string.Empty;
                string v2 = string.Empty;
                string v3 = string.Empty;
                string v4 = string.Empty;
                string v5 = string.Empty;
                string v6 = string.Empty;
                string v7 = string.Empty;
                string v8 = string.Empty;
                string v9 = string.Empty;
                string v10 = string.Empty;
                string v11 = string.Empty;
                //Added By Manny 12/23/2010
                string v12 = string.Empty;
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
                    v11 = v11 + isLockedPopulate(isJobLocked( txtDate.Text
                                                            , dsSplit.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString()
                                                            , dsSplit.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString())) + ",";
                    v12 = v12 + dsSplit.Tables[0].Rows[ctr]["Jsd_Category"].ToString() + ",";
                    v13 = v13 + dsSplit.Tables[0].Rows[ctr]["Jsd_Overtime"].ToString() + ",";
                }
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

                //Assigning the values to the page
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
                hOvertime.Value = v13;

                hiddenStatus.Value = dsSplit.Tables[0].Rows[0]["Jsh_Status"].ToString();
                hiddenFlag.Value = dsSplit.Tables[0].Rows[0]["Jsh_Status"].ToString();
                hiddenControl.Value = dsSplit.Tables[0].Rows[0]["Jsh_ControlNo"].ToString();

                if (hiddenControl.Value.StartsWith("J"))
                    hiddenFlag.Value = "";

                switch (hiddenFlag.Value)
                {
                    case "1":
                        txtStatus.Text = "NEW";
                        btnX.Enabled = true;
                        btnY.Enabled = true;
                        btnZ.Enabled = true;
                        hfSave.Value = "0";
                        break;
                    case "3":
                        txtStatus.Text = "ENDORSED TO CHECKER 1";
                        btnX.Enabled = false;
                        btnY.Enabled = false;
                        btnZ.Enabled = false;
                        if (Session["flagCheck"] == null || Session["flagCheck"].ToString() != "ENDORSE")
                            MessageBox.Show("There is a Modification transaction on route.");
                        break;
                    case "5":
                        txtStatus.Text = "ENDORSED TO CHECKER 2";
                        btnX.Enabled = false;
                        btnY.Enabled = false;
                        btnZ.Enabled = false;
                        if (Session["flagCheck"] == null || Session["flagCheck"].ToString() != "ENDORSE")
                            MessageBox.Show("There is a Modification transaction on route.");
                        break;
                    case "7":
                        txtStatus.Text = "ENDORSED TO APPROVER";
                        btnX.Enabled = false;
                        btnY.Enabled = false;
                        btnZ.Enabled = false;
                        if (Session["flagCheck"] == null || Session["flagCheck"].ToString() != "ENDORSE")
                            MessageBox.Show("There is a Modification transaction on route.");
                        break;
                    case "9":
                        txtStatus.Text = "APPROVED";
                        break;
                    default:
                        txtStatus.Text = string.Empty;
                        break;
                }

                if (hiddenFlag.Value == "1" && hfSave.Value == "0" && x.StartsWith("S"))
                {
                    Session["flagSave"] = "SAVED";
                }
                else
                {
                    Session["flagSave"] = string.Empty;
                }
            }
            else
            {
                txtStatus.Text = string.Empty;
                hiddenFlag.Value = "";
                hfFlagEntry.Value = "N";
                Session["flagSave"] = string.Empty;
            }
        }
        catch (Exception ex)
        {
            txtStatus.Text = string.Empty;
            hiddenFlag.Value = "";
            hfFlagEntry.Value = "N";
            Session["flagSave"] = string.Empty;

            if (txtJStart.Text.Equals(string.Empty))    // has no jobsplit
            {
                #region Initialize Manhour Allocation Detail
                //Kelvin added: 20110413 - If no jobsplit is retrieved from the database. initialize job fields to the most recent job by the employee
                DataSet dsRecentJob;

                dsRecentJob = JSBL.GetRecentJobByEmployee(txtEmployeeId.Text);
                //dsRecentJob = JSBL.GetRecentJobByEmployee(Session["userId"].ToString());
                if (!CommonMethods.isEmpty(dsRecentJob))
                {
                    txtJJobCode.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_JobCode"].ToString().Trim();
                    txtJClientNo.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_ClientJobNo"].ToString().Trim();
                    txtJClientName.Text = dsRecentJob.Tables[0].Rows[0]["Slm_ClientJobName"].ToString().Trim();
                    txtSubWork.Text = dsRecentJob.Tables[0].Rows[0]["Jsd_SubWorkCode"].ToString().Trim();
                    string categoryAndCostCenter = getCostCenterAndCategoryV2(txtJJobCode.Text, txtJClientNo.Text);
                    string category = categoryAndCostCenter.Substring(0, categoryAndCostCenter.IndexOf("~"));
                    //chkBillable.Checked = category.Equals("B");
                    hCat.Value = category;
                    hBillable.Value = category.Equals("B").ToString();
                    hCCT.Value = dsRecentJob.Tables[0].Rows[0]["Jsd_CostCenter"].ToString().Trim();
                    hSub.Value = getSubWorks(getCostCenterAndCategory(txtJJobCode.Text, txtJClientNo.Text));

                    if (DayCode == "REST")
                        hOvertime.Value = "true";
                    else
                        hOvertime.Value = "false";   
                }

                txtJStart.Text = JSBL.GetStartTime(txtTimeIn1.Text, txtTimeIn2.Text, strShift, Convert.ToDecimal(hfOTFRAC.Value));
                #endregion
            }
        }
    }
    #endregion

    #region Checks the Job Split Header if the is a jobsplit that exist
    //Method not in use yet.
    protected bool checkJobSplit(string date, string empId)
    {
        string x = string.Empty;
        string sql = @"select top 1 jsh_employeeid 
                                from t_jobsplitheader
                                where jsh_employeeid = '{0}'
                                and convert(varchar(10), Jsh_JobSplitDate,101) = '{1}'
                                ";
        using (DALHelper dal = new DALHelper())
        {
            try
            { 
                dal.OpenDB();
                x = Convert.ToString(dal.ExecuteScalar(string.Format(sql, empId, date), CommandType.Text));
            }
            catch (Exception ex)
            {
                //Do nothing
            }
            finally
            { 
                dal.CloseDB();
            }
        }
        return !(x.Trim().Equals(string.Empty));
        
    }
    #endregion

    #region Load the Job Split entry from checklist or pending
    protected void LoadJobSplitEntry()
    {
        if (Request.RawUrl.Contains("cn"))
        {
            try
            {
                string controlNum = Request.QueryString["cn"].ToString();
                hiddenControl.Value = controlNum;
                hiddenCNumber.Value = controlNum;
                if (!controlNum.Equals(string.Empty))
                {
                    DataRow dr = JSBL.GetJobSplitHeaderInfo(controlNum);
                    this.hfPrevCNumber.Value = dr["Jsh_RefControlNo"].ToString();
                    
                    this.btnViewPrevious.Visible = hfPrevCNumber.Value.Equals(string.Empty) ? false : true;
                    
                    DataSet dsLoad = JSBL.LoadCurrentJobSplitDetails(controlNum);
                    if (dsLoad.Tables.Count > 0)
                        if (dsLoad.Tables[0].Rows.Count > 0)
                            hasJob_Split.Value = dsLoad.Tables[0].Rows.Count.ToString();

                    Session["userId"] = dr["Jsh_EmployeeId"].ToString();
                    txtEmployeeId.Text = dr["Jsh_EmployeeId"].ToString();
                    InitializeUser();
                    InitializeValues();
                    string _date = Convert.ToString(dr["Jsh_JobSplitDate"]);
                    txtDate.Text = _date;
                    txtRemarks.Text = dr["Trm_Remarks"].ToString();
                    GetShifts();
                    if (dsLoad.Tables[0] != null && dsLoad.Tables[0].Rows.Count > 0)
                    {
                        hfFlagEntry.Value = "C";//Used for saving the Modification Entry. C - changed or N - new
                        string v1 = string.Empty;
                        string v2 = string.Empty;
                        string v3 = string.Empty;
                        string v4 = string.Empty;
                        string v5 = string.Empty;
                        string v6 = string.Empty;
                        string v7 = string.Empty;
                        string v8 = string.Empty;
                        string v9 = string.Empty;
                        string v10 = string.Empty;
                        string v11 = string.Empty;
                        string v12 = string.Empty;
                        string v13 = string.Empty;

                        string tempJobCode = string.Empty;
                        string tempJobNo = string.Empty;

                        string Category = string.Empty;
                        string Costcenter = string.Empty;


                        int max = dsLoad.Tables[0].Rows.Count;

                        for (int ctr = 0; ctr < max; ctr++)
                        {
                            v1 = v1 + dsLoad.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(0, 2) + ":" + dsLoad.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(2, 2) + ",";
                            v2 = v2 + dsLoad.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(0, 2) + ":" + dsLoad.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(2, 2) + ",";
                            v3 = v3 + Convert.ToString(Convert.ToDouble(dsLoad.Tables[0].Rows[ctr]["Jsd_PlanHours"])) + ",";
                            v4 = v4 + dsLoad.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString() + ",";
                            v5 = v5 + dsLoad.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString() + ",";
                            v6 = v6 + dsLoad.Tables[0].Rows[ctr]["Slm_ClientJobName"].ToString() + ",";
                            v7 = v7 + dsLoad.Tables[0].Rows[ctr]["Jsd_SubWorkCode"].ToString() + ",";

                            //Andre added: update 20091205
                            tempJobCode = dsLoad.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString();
                            tempJobNo = dsLoad.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString();

                            v8 = v8 + getSubWorks(getCostCenterAndCategory(tempJobCode, tempJobNo)) + ",";

                            v9 = v9 + dsLoad.Tables[0].Rows[ctr]["Slm_Category"].ToString() + ",";
                            v10 = v10 + dsLoad.Tables[0].Rows[ctr]["Jsd_Costcenter"].ToString() + ",";
                            v11 = v11 + isLockedPopulate(isJobLocked(txtDate.Text
                                                                    , dsLoad.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString()
                                                                    , dsLoad.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString())) + ",";
                            v12 = v12 + dsLoad.Tables[0].Rows[ctr]["Jsd_Category"].ToString() + ",";
                            v13 = v13 + dsLoad.Tables[0].Rows[ctr]["Jsd_Overtime"].ToString() + ",";
                        }

                        //Assigning the values to the page
                        txtJStart.Text = v1;
                        txtJEnd.Text = v2;
                        txtJHours.Text = v3;
                        txtJJobCode.Text = v4;
                        txtJClientNo.Text = v5;
                        txtJClientName.Text = v6;
                        txtSubWork.Text = v7;

                        //Andre added: 20091205
                        hSub.Value = v7;
                        hCat.Value = v12;
                        hCCT.Value = v10;
                        hCFlag.Value = v11;
                        hBillable.Value = v12;
                        hOvertime.Value = v13;

                        hiddenFlag.Value = dsLoad.Tables[0].Rows[0]["Jsh_Status"].ToString();
                        hiddenControl.Value = dsLoad.Tables[0].Rows[0]["Jsh_ControlNo"].ToString();
                        switch (hiddenFlag.Value)
                        {
                            case "1":
                                txtStatus.Text = "NEW";
                                break;
                            case "3":
                                txtStatus.Text = "ENDORSED TO CHECKER 1";
                                break;
                            case "5":
                                txtStatus.Text = "ENDORSED TO CHECKER 2";
                                break;
                            case "7":
                                txtStatus.Text = "ENDORSED TO APPROVER";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid control number retrieve. Try again.");
                    }
                }
                else
                {
                    MessageBox.Show("Could not retrieve control number.");
                }
                InitializeControls();
                GetShifts();
                //?hiddenDFlag.Value = "0";
                hiddenDFlag.Value = hiddenFlag.Value;
            }
            catch (Exception ex)
            { 
                //Do Nothing
            }
        }
    }
    #endregion

    protected string getCostCenterAndCategoryV2(string jobCode, string jobNo)
    {
        string retVal = string.Empty;
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

        if (ds.Tables.Count > 0)
            if (ds.Tables[0].Rows.Count > 0)
            {
                retVal += ds.Tables[0].Rows[0]["slm_category"].ToString();
                retVal += "~";
                retVal += ds.Tables[0].Rows[0]["slm_costcenter"].ToString();
            }
            else
            {
                retVal = string.Empty;
            }

        return retVal;
    }

    protected string getCostCenterAndCategoryV2(string jobCode, string jobNo, string employeeId)
    {
        string retVal = string.Empty;
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

        if (ds.Tables.Count > 0)
            if (ds.Tables[0].Rows.Count > 0)
            {
                retVal += ds.Tables[0].Rows[0]["slm_category"].ToString();
                retVal += "~";
                if (ds.Tables[0].Rows[0]["slm_costcenter"].ToString().ToUpper().Equals("ALL"))
                    retVal += ds.Tables[1].Rows[0]["Emt_CostCenterCode"].ToString();
                else
                    retVal += ds.Tables[0].Rows[0]["slm_costcenter"].ToString();
            }
            else
            {
                retVal = string.Empty;
            }

        return retVal;
    }

    protected string getSubWorks(string[] values)
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

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, txtEmployeeId.Text, values[1], values[1]), CommandType.Text);
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
                    retVal = string.Empty;
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

        if (ds.Tables.Count > 0)
            if (ds.Tables[0].Rows.Count > 0)
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

    private string isLockedPopulate(bool flag)
    {
        string temp = "0";
        if (flag)
            temp = "2";

        return temp;
    }

    private bool isNotValidParameter(string[] VAL)
    {
        bool retFlag = false;
        int count = VAL.Length;

        for (int i = 0; i < count && retFlag; i++)
        {
            if (VAL[i].Trim().Equals(string.Empty))
                retFlag = true;
        }

        return retFlag;

    }
    #endregion
}
