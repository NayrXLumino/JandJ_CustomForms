/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/23/2013
 *  Update Notes    :   
 *      -   Updated Reports for Correct Data
 */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Text;
using System.Collections;
using DevExpress.Web.ASPxGridView.Export;
using MethodsLibrary;
using System.Data;
using CommonLibrary;

public partial class Transactions_ManhourAllocation_pgeNoManHour : System.Web.UI.Page
{

    //System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
    MenuGrant MGBL = new MenuGrant();
    private DataSet dsView;
    int innerHeight = 0;
    int outerHeight = 0;
    private decimal grandTotal = 0;
    private bool print = false;
    private List<string> categoryList;
    private List<string> categoryListCopy;
    protected void Page_init(object sender, EventArgs e)
    {
       // txtEmpName.Text = Session["userId"].ToString();

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            if (!Page.IsCallback)
            {
                Response.Redirect("../../index.aspx?pr=dc");
            }
        }//WFNOMANHRREP WFNOMANHRREP
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFNOMANHRREP"))
        {
            Response.Redirect("../../index.aspx?pr=uc");
        }
        else
        {
            if (!IsPostBack)
            {
                DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFNOMANHRREP");
                if (dr != null)
                {
                    
                    btnExport.Visible = Convert.ToBoolean(dr["Ugt_CanGenerate"]);
                }
                txtEmpName.Text = Session["userId"].ToString();
                btnClear_Click(null, null);
            }
            System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
            SqlDataSource1.SelectCommand = query.Value;
            LoadComplete += new EventHandler(_Default_LoadComplete);
            btnEmployee.OnClientClick = string.Format("return lookupRJSEmployeeMR('{0}')", false);
            btnCostCenter.OnClientClick = string.Format("return lookupRJSCostcenterMR('{0}')", false);
            DALHelper dh = new DALHelper();
            SqlDataSource1.ConnectionString = Encrypt.decryptText(session["dbConn"].ToString());
        }
    }
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

    }

    private string costcenterFilter()
    {
        StringBuilder filters = new StringBuilder();

        #region Cost Center Access
        if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
        {
            string temp = hfUserCostCenters.Value;
            temp = temp.Replace("x", "'").Replace("y", ",");
            filters.Append("\n AND (");
            filters.Append("\n Emt_Costcentercode in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\n OR LEFT(Emt_Costcentercode, 2) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\n OR LEFT(Emt_Costcentercode, 4) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\n OR LEFT(Emt_Costcentercode, 6) in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
            filters.Append("\n OR LEFT(Emt_Costcentercode, 8) in (@costCenterAcc) )".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
        }
        if (txtCostCenter.Text != "")
        {
            string temp = txtCostCenter.Text.ToString();
            temp = "'" + temp.Replace(",", "','") + "'";
            filters.Append("\nAND ( Emt_Costcentercode in (@costCenterAcc)".Replace("@costCenterAcc", temp));
            filters.Append("\nOR LEFT(Emt_Costcentercode, 2) in (@costCenterAcc)".Replace("@costCenterAcc", temp));
            filters.Append("\nOR LEFT(Emt_Costcentercode, 4) in (@costCenterAcc)".Replace("@costCenterAcc", temp));
            filters.Append("\nOR LEFT(Emt_Costcentercode, 6) in (@costCenterAcc)".Replace("@costCenterAcc", temp));
            filters.Append("\nOR LEFT(Emt_Costcentercode, 8) in (@costCenterAcc) )".Replace("@costCenterAcc", temp));
        }
        return filters.ToString();
    }
    private string leaveFilter()
    {
        string str = string.Empty;
        string strDateFilter = string.Empty;
        if (!dtpOTDateFrom.IsNull || !dtpOTDateTo.IsNull)
        {
            if (!dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
            {
                if (Convert.ToDateTime(dtpOTDateTo.Date.ToString("MM/dd/yyyy")) > Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")))
                {
                    dtpOTDateTo.Date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                }
                strDateFilter = string.Format(@"AND Elt_LeaveDate
                                 BETWEEN '{0}' AND '{1}' ", Convert.ToDateTime(dtpOTDateFrom.Date).ToString("MM/dd/yyyy")
                                                                 , Convert.ToDateTime(dtpOTDateTo.Date).ToString("MM/dd/yyyy"));
               
            }
            else if (!dtpOTDateFrom.IsNull && dtpOTDateTo.IsNull)
            {
                strDateFilter = string.Format(@"AND Elt_LeaveDate
                                 >= '{0}' ", Convert.ToDateTime(dtpOTDateFrom.Date).ToString("MM/dd/yyyy"));
            }
            else if (dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
            {
                if (Convert.ToDateTime(dtpOTDateTo.Date.ToString("MM/dd/yyyy")) > Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")))
                {
                    strDateFilter = string.Format(@"AND Elt_LeaveDate 
                                 <= getdate() ");
                }
                else
                    strDateFilter = string.Format(@"AND Elt_LeaveDate
                                 <= '{0}' ", Convert.ToDateTime(dtpOTDateTo.Date).ToString("MM/dd/yyyy"));
            }
        }
        if (dtpOTDateTo.IsNull)
        {
            strDateFilter = string.Format(@"AND Ell_ProcessDate
                                 <= getdate() ");
        }
        str = string.Format(@"
AND Ell_EmployeeID + CONVERT(VARCHAR(20), Ell_ProcessDate, 101) NOT IN (
SELECT
	Elt_EmployeeId + CONVERT(VARCHAR(20), Elt_LeaveDate, 101)
FROM T_EmployeeLeaveAvailment
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = Elt_LeaveType
WHERE Ltm_LeaveType <> 'OB'
	{0}
	AND Elt_Status IN ( '9', 'A' )
	AND Elt_ControlNo NOT IN (
		SELECT Elt_RefControlNo FROM T_EmployeeLeaveAvailment
		WHERE Elt_RefControlNo IS NOT NULL
		AND RTRIM(Elt_RefControlNo) <> ''
        {0}
		UNION		
		SELECT Elt_RefControlNo FROM T_EmployeeLeaveAvailmentHist
		WHERE Elt_RefControlNo IS NOT NULL
		AND RTRIM(Elt_RefControlNo) <> ''
        {0}
	)
UNION

SELECT
	Elt_EmployeeId + CONVERT(VARCHAR(20), Elt_LeaveDate, 101)
FROM T_EmployeeLeaveAvailmentHist
LEFT JOIN T_LeaveTypeMaster
	ON Ltm_LeaveType = Elt_LeaveType
WHERE Ltm_LeaveType <> 'OB'
	{0}
	AND Elt_Status IN ( '9', 'A' )
	AND Elt_ControlNo NOT IN (
		SELECT Elt_RefControlNo FROM T_EmployeeLeaveAvailment
		WHERE Elt_RefControlNo IS NOT NULL
		AND RTRIM(Elt_RefControlNo) <> ''
        {0}
		UNION		
		SELECT Elt_RefControlNo FROM T_EmployeeLeaveAvailmentHist
		WHERE Elt_RefControlNo IS NOT NULL
		AND RTRIM(Elt_RefControlNo) <> ''
        {0}
	) )
        ", strDateFilter);
        return str;
    }
    private string employeeFilter()
    {
        StringBuilder filters = new StringBuilder();
        if (txtEmpName.Text != string.Empty)
        {
           
            string text = txtEmpName.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string employeeId = "", employeeName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    employeeId += string.Format("\nOR Emt_EmployeeId like '{0}%'", arr[i].ToString().Trim());
                    employeeName += string.Format("\nOR Emt_LastName like '{0}%' OR Emt_FirstName like '{0}%'", arr[i].ToString().Trim());
                }
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(employeeId);
                filters.Append(employeeName + ")");
            }
        }

        #endregion

        return filters.ToString();
    }
    private string costtwo()
    {  StringBuilder filters = new StringBuilder();

      
        if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
        {
            string temp = hfUserCostCenters.Value;
            temp = temp.Replace("x", "'").Replace("y", ",");
            filters.Append("\nAND Uca_CostCenterCode in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
        }
        if (txtCostCenter.Text != "")
        {
            string temp = txtCostCenter.Text.ToString();
            temp = "'" + temp.Replace(",", "','") + "'";
            filters.Append("\nAND Uca_CostCenterCode in (@costCenterAcc)".Replace("@costCenterAcc", temp));
        }
        return filters.ToString();
    }
    private string datefilter()
    {
        StringBuilder filters = new StringBuilder();
        if (!dtpOTDateFrom.IsNull || !dtpOTDateTo.IsNull)
        {
            if (!dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
            {
                if (Convert.ToDateTime(dtpOTDateTo.Date.ToString("MM/dd/yyyy")) > Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")))
                {
                    dtpOTDateTo.Date = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                }
                filters.Append(string.Format(@"AND Ell_ProcessDate
                                 BETWEEN '{0}' AND '{1}' ", Convert.ToDateTime(dtpOTDateFrom.Date).ToString("MM/dd/yyyy")
                                                                 , Convert.ToDateTime(dtpOTDateTo.Date).ToString("MM/dd/yyyy")));
            }
            else if (!dtpOTDateFrom.IsNull && dtpOTDateTo.IsNull)
            {
                filters.Append(string.Format(@"AND Ell_ProcessDate
                                 >= '{0}' ", Convert.ToDateTime(dtpOTDateFrom.Date).ToString("MM/dd/yyyy")));
            }
            else if (dtpOTDateFrom.IsNull && !dtpOTDateTo.IsNull)
            {
                //string s = DateTime.Now.ToString("MM/dd/yyyy");
                //string d = dtpOTDateTo.Date.ToString("MM/dd/yyyy");
                if (Convert.ToDateTime(dtpOTDateTo.Date.ToString("MM/dd/yyyy")) > Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")))
                {
                    filters.Append(string.Format(@"AND Ell_ProcessDate 
                                 <= getdate() "));
                }
                else
                    filters.Append(string.Format(@"AND Ell_ProcessDate
                                 <= '{0}' ", Convert.ToDateTime(dtpOTDateTo.Date).ToString("MM/dd/yyyy")));
            }
        }
        if (dtpOTDateTo.IsNull )
        {
            filters.Append(string.Format(@"AND Ell_ProcessDate
                                 <= getdate() "));
        }

        return filters.ToString();
    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        txtEmpName.Text.ToString();
        #region query
//        SqlDataSource1.SelectCommand = query.Value = string.Format(@"
//    declare @date varchar(15)
//    declare @userid varchar(5)
//    declare @maxdate date
//    set @maxdate=(SELECT max(Ell_ProcessDate)FROM T_EmployeeLogLedger)
//    set @userid='{2}'
//    DECLARE @EMPLOYEETEMP AS TABLE(Employee_Name varchar(max),
//    Emt_EmployeeId varchar(10)
//    ,ForSort varchar(max)
//    ,Ell_ShiftCode varchar(10)
//    ,Emt_LastName varchar(max)
//    ,Emt_FirstName varchar(max)
//    ,Emt_JobStatus varchar(max))
//    DECLARE @DSP as bit
//    SET @DSP = (SELECT Pcm_ProcessFlag
//    FROM T_ProcessControlMaster
//    WHERE Pcm_ProcessId = 'DSPFULLNM')
//
//    declare @flag varchar(3)
//    declare @filter table( costcenter varchar(50))
//    set @flag = (SELECT Uca_CostCenterCode
//            FROM T_UserCostCenterAccess
//            WHERE Uca_Status = 'A'
//            AND Uca_SytemId = 'TIMEKEEP'
//            AND Uca_UserCode = @userid)
//                                               
//    if (@flag='ALL')
//    begin
//	                         
//    insert into @filter
//    SELECT cct_costcentercode
//    FROM T_CostCenter
//    WHERE Cct_status = 'A'
//    --add conditions with costcenters--
//    {0}
//									
//	                         
//    end
//    else
//    begin
//    insert into @filter
//    SELECT Uca_CostCenterCode
//    FROM T_UserCostCenterAccess
//    WHERE Uca_Status = 'A'
//    AND Uca_SytemId = 'TIMEKEEP'
//    AND Uca_UserCode = @userid
//    --add conditions with costcenters--
//    {4}
//    end
//                         
//    insert @EMPLOYEETEMP
//    SELECT DISTINCT Emt_EmployeeId 
//    + '   -   '
//    + CASE WHEN (@DSP = '1')
//    THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
//    ELSE Emt_NickName
//    END AS [Employee Name]
//    , Emt_EmployeeId
//    , CASE WHEN (@DSP = '1')
//    THEN Emt_LastName + Emt_FirstName
//    ELSE Emt_NickName
//    END AS [ForSort]
//    ,Ell_ShiftCode
//    ,Emt_LastName
//    ,Emt_FirstName
//    ,Emt_JobStatus
//    FROM T_EmployeeLogLedger
//    INNER JOIN T_EmployeeMaster
//    ON Emt_EmployeeID = Ell_EmployeeID
//    and Emt_EmployeeID not in ('0000','0585')
//    and Emt_JobStatus!='IN'
//    WHERE Ell_ProcessDate =@maxdate
//    -- Filter Insertion -- 
//    --employee condition--
//    {1}
//    AND Emt_CostCenterCode IN (select * from @filter)
//                           
//    UNION
//                         
//    SELECT DISTINCT Emt_EmployeeId 
//    + '   -   '
//    + CASE WHEN (@DSP = '1')
//    THEN dbo.GetControlEmployeeName(Emt_EmployeeId)
//    ELSE Emt_NickName
//    END AS [Employee Name]
//    , Emt_EmployeeId
//    , CASE WHEN (@DSP = '1')
//    THEN Emt_LastName + Emt_FirstName
//    ELSE Emt_NickName
//    END AS [ForSort]
//    ,Ell_ShiftCode
//    ,Emt_LastName
//    ,Emt_FirstName
//    ,Emt_JobStatus
//    FROM T_EmployeeLogLedgerHist
//    INNER JOIN T_EmployeeMaster
//    ON Emt_EmployeeID = Ell_EmployeeID
//    and Emt_EmployeeID not in ('0000',0585)
//    and Emt_JobStatus!='IN'
//    WHERE Ell_ProcessDate = @maxdate
//    -- Filter Insertion -- 
//    --employee condition--
//    {1}
//    AND Emt_CostCenterCode IN (select * from @filter)
//      
//    SELECT 
//		Emt_EmployeeID
//		,Emt_LastName+','+Emt_FirstName[Emt_LastName]
//		,Emt_FirstName
//		--,Emt_EmailAddress
//		--,DATENAME(M,Ell_ProcessDate) +' '+ DATENAME(DAY,Ell_ProcessDate) +' '+ convert(varchar(4),YEAR(Ell_ProcessDate))[Ell_ProcessDate]
//		,+CONVERT(varchar(10),Ell_ProcessDate,121)[Ell_ProcessDate]--+')'+CONVERT(VARCHAR(20),Ell_ProcessDate,107)
//		,Ell_ProcessDate[orders]
//		,lgldgr.Scm_ShiftDesc[sched]
//		,lgldgr.Scm_ShiftHours[hrs]
//		,case when left(lgldgr.Ell_ActualTimeIn_1,2)+':'+right(lgldgr.Ell_ActualTimeIn_1,2)='00:00'
//				then ' '
//				ELSE  left(lgldgr.Ell_ActualTimeIn_1,2)+':'+right(lgldgr.Ell_ActualTimeIn_1,2)
//				END [ATin]
//		,CASE WHEN left(lgldgr.Ell_ActualTimeOut_1,2)+':'+right(lgldgr.Ell_ActualTimeOut_1,2)='00:00'
//				THEN ' '
//				ELSE left(lgldgr.Ell_ActualTimeIn_1,2)+':'+right(lgldgr.Ell_ActualTimeIn_1,2)
//				END[Ell_ActualTimeOut_1]
//		,CASE WHEN left(lgldgr.Ell_ActualTimeIn_2,2)+':'+right(lgldgr.Ell_ActualTimeIn_2,2)='00:00'
//				THEN ' '
//				ELSE left(lgldgr.Ell_ActualTimeIn_2,2)+':'+right(lgldgr.Ell_ActualTimeIn_2,2)
//				END Ell_ActualTimeIn_2
//		,CASE WHEN left(lgldgr.Ell_ActualTimeOut_2,2)+':'+right(lgldgr.Ell_ActualTimeOut_2,2)='00:00'
//				THEN ' '
//				ELSE left(lgldgr.Ell_ActualTimeOut_2,2)+':'+right(lgldgr.Ell_ActualTimeOut_2,2)
//				END [Ell_ActualTimeOut_2]
//	FROM @EMPLOYEETEMP
//	left join (
//		SELECT Ell_ProcessDate,Ell_EmployeeId,Ell_DayCode,ELL_SHIFTCODE,Scm_ShiftCode+' ('+left(Scm_ShiftTimeIn,2)+':'+right(Scm_ShiftTimeIn,2)+'-'+left(Scm_ShiftBreakStart,2)+':'+right(Scm_ShiftBreakStart,2)+' '+
//		left(Scm_ShiftBreakEnd,2)+':'+right(Scm_ShiftBreakEnd,2)+'-'+
//		left(Scm_ShiftTimeOut,2)+':'+right(Scm_ShiftTimeOut,2)+')'[Scm_ShiftDesc]
//        ,Scm_ShiftHours,Ell_ActualTimeIn_1,Ell_ActualTimeOut_1,Ell_ActualTimeIn_2,Ell_ActualTimeOut_2 from T_EmployeeLogLedger
//		LEFT JOIN T_SHIFTCODEMASTER
//ON Scm_ShiftCode = Ell_ShiftCode
//			WHERE Ell_DayCode='REG' 
//				-- and Ell_ProcessDate BETWEEN (GETDATE()-100) AND GETDATE()
//                {3}
//                                                                                        
//				and ell_employeeid not in ('0000','0585')
//				                                                
//		union ALL
//		                                                
//		SELECT Ell_ProcessDate,Ell_EmployeeId,Ell_DayCode,ELL_SHIFTCODE,Scm_ShiftCode+' ('+left(Scm_ShiftTimeIn,2)+':'+right(Scm_ShiftTimeIn,2)+'-'+left(Scm_ShiftBreakStart,2)+':'+right(Scm_ShiftBreakStart,2)+' '+
//		left(Scm_ShiftBreakEnd,2)+':'+right(Scm_ShiftBreakEnd,2)+'-'+
//		left(Scm_ShiftTimeOut,2)+':'+right(Scm_ShiftTimeOut,2)+')'[Scm_ShiftDesc]
//        ,Scm_ShiftHours,Ell_ActualTimeIn_1,Ell_ActualTimeOut_1,Ell_ActualTimeIn_2,Ell_ActualTimeOut_2 from T_EmployeeLogLedgerHist
//		LEFT JOIN T_SHIFTCODEMASTER
//ON Scm_ShiftCode = Ell_ShiftCode 
//			WHERE Ell_DayCode='REG' 
//				-- and Ell_ProcessDate BETWEEN (GETDATE()-100) AND GETDATE()
//                    {3}
//				and ell_employeeid not in ('0000','0585')
//			)as lgldgr
//		on Emt_EmployeeID= lgldgr.Ell_EmployeeId
//		left join T_JobSplitheader
//		on lgldgr.Ell_EmployeeId = jsh_employeeid
//			and lgldgr.Ell_ProcessDate=Jsh_JobSplitDate
//		where 
//			Emt_JobStatus <> 'IN'
//			and Jsh_ControlNo is null
//            --date and employee filter 
//            {1}
//			AND  Emt_EmployeeID NOT IN ('0000','0585')
//			--and emt_employeeid = '00021'
//            --OR emt_employeeid = '00024'
//			AND Ell_ProcessDate IS NOT NULL
//			--group by CONVERT(VARCHAR(20),Ell_ProcessDate,107)
//			--,Emt_EmployeeID,Emt_LastName
//			--,Emt_FirstName
//			ORDER BY Emt_EmployeeID,orders", costcenterFilter(), employeeFilter(), Session["userId"].ToString(), datefilter(),costtwo()); 
        #endregion
        SqlDataSource1.SelectCommand = query.Value = string.Format(@"
SELECT
	Ell_EmployeeId [Employee ID]
	, Emt_LastName + ', ' + Emt_FirstName [Employee Name]
	, CONVERT(varchar(10),Ell_ProcessDate,121) [Date]
	, Scm_ShiftCode
		+' ('+left(Scm_ShiftTimeIn,2)+':'+right(Scm_ShiftTimeIn,2)+'-'+left(Scm_ShiftBreakStart,2)+':'+right(Scm_ShiftBreakStart,2)+' '+
		left(Scm_ShiftBreakEnd,2)+':'+right(Scm_ShiftBreakEnd,2)+'-'+
		left(Scm_ShiftTimeOut,2)+':'+right(Scm_ShiftTimeOut,2)+')'[Scm_ShiftDesc]
	, Scm_ShiftHours [Hours]
	,case when left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)='00:00'
			then ' '
			ELSE  left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)
			END [IN 1]
	,CASE WHEN left(Ell_ActualTimeOut_1,2)+':'+right(Ell_ActualTimeOut_1,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)
			END [OUT 1]
	,CASE WHEN left(Ell_ActualTimeIn_2,2)+':'+right(Ell_ActualTimeIn_2,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeIn_2,2)+':'+right(Ell_ActualTimeIn_2,2)
			END [IN 2]
	,CASE WHEN left(Ell_ActualTimeOut_2,2)+':'+right(Ell_ActualTimeOut_2,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeOut_2,2)+':'+right(Ell_ActualTimeOut_2,2)
			END [OUT 2]
	, dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Section]
FROM T_EmployeeLogLedger
LEFT JOIN T_EmployeeMaster
	ON Emt_EmployeeID = Ell_EmployeeId
LEFT JOIN T_JobSplitHeader
	ON Jsh_EmployeeId = Ell_EmployeeId
	AND Jsh_JobSplitDate = Ell_ProcessDate
LEFT JOIN T_ShiftCodeMaster
	ON Scm_ShiftCode = Ell_ShiftCode
WHERE 1 = 1
    AND Ell_ProcessDate >= Emt_HireDate
    AND Ell_Daycode LIKE 'REG%'
	AND Jsh_EmployeeId IS NULL
    AND Ell_ProcessDate <= GETDATE()
	AND Emt_CostCenterCode IN (
		SELECT Cct_Costcentercode
		FROM T_UserCostCenterAccess
		INNER JOIN T_CostCenter
			ON Uca_CostCenterCode = 'ALL'
			OR Uca_CostCenterCode = Cct_CostCenterCode
		WHERE Uca_SytemID = 'TIMEKEEP'
			AND Uca_Status = 'A'
			AND Uca_Usercode = '{0}'
	)
    AND Emt_Jobstatus <> 'IN'
    {1}
    {2}
    {3}
    {4}
	
UNION ALL

SELECT
	Ell_EmployeeId [Employee ID]
	, Emt_LastName + ', ' + Emt_FirstName [Employee Name]
	, CONVERT(varchar(10),Ell_ProcessDate,121) [Date]
	, Scm_ShiftCode
		+' ('+left(Scm_ShiftTimeIn,2)+':'+right(Scm_ShiftTimeIn,2)+'-'+left(Scm_ShiftBreakStart,2)+':'+right(Scm_ShiftBreakStart,2)+' '+
		left(Scm_ShiftBreakEnd,2)+':'+right(Scm_ShiftBreakEnd,2)+'-'+
		left(Scm_ShiftTimeOut,2)+':'+right(Scm_ShiftTimeOut,2)+')'[Scm_ShiftDesc]
	, Scm_ShiftHours [Hours]
	,case when left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)='00:00'
			then ' '
			ELSE  left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)
			END [IN 1]
	,CASE WHEN left(Ell_ActualTimeOut_1,2)+':'+right(Ell_ActualTimeOut_1,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeIn_1,2)+':'+right(Ell_ActualTimeIn_1,2)
			END [OUT 1]
	,CASE WHEN left(Ell_ActualTimeIn_2,2)+':'+right(Ell_ActualTimeIn_2,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeIn_2,2)+':'+right(Ell_ActualTimeIn_2,2)
			END [IN 2]
	,CASE WHEN left(Ell_ActualTimeOut_2,2)+':'+right(Ell_ActualTimeOut_2,2)='00:00'
			THEN ' '
			ELSE left(Ell_ActualTimeOut_2,2)+':'+right(Ell_ActualTimeOut_2,2)
			END [OUT 2]
	, dbo.getCostCenterFullNameV2(Emt_CostCenterCode) [Section]
FROM T_EmployeeLogLedgerHist
LEFT JOIN T_EmployeeMaster
	ON Emt_EmployeeID = Ell_EmployeeId
LEFT JOIN T_JobSplitHeader
	ON Jsh_EmployeeId = Ell_EmployeeId
	AND Jsh_JobSplitDate = Ell_ProcessDate
LEFT JOIN T_ShiftCodeMaster
	ON Scm_ShiftCode = Ell_ShiftCode
WHERE 1 = 1
    AND Ell_ProcessDate >= Emt_HireDate
    AND Ell_Daycode LIKE 'REG%'
	AND Jsh_EmployeeId IS NULL
    AND Ell_ProcessDate <= GETDATE()
	AND Emt_CostCenterCode IN (
		SELECT Cct_Costcentercode
		FROM T_UserCostCenterAccess
		INNER JOIN T_CostCenter
			ON Uca_CostCenterCode = 'ALL'
			OR Uca_CostCenterCode = Cct_CostCenterCode
		WHERE Uca_SytemID = 'TIMEKEEP'
			AND Uca_Status = 'A'
			AND Uca_Usercode = '{0}'
	)
    AND Emt_Jobstatus <> 'IN'
    {1}
    {2}
    {3}
    {4}

ORDER BY 2, 3
        ", Session["userId"].ToString(), costcenterFilter(), employeeFilter(), datefilter(), leaveFilter());
        ASPxGridView1.DataBind();
        if (ASPxGridView1.VisibleRowCount > 0)
            pnlResult.Visible = true;
        else
        {
            pnlResult.Visible = false;
            MessageBox.Show("No Data Found");
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        pnlResult.Visible = false; 
        txtEmpName.Text = Session["userId"].ToString();
        dtpOTDateTo.Date=DateTime.Now;
        dtpOTDateFrom.Date = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MinDate = CommonMethods.getMinimumDateOfFiling();
        //dtpLVDate.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        
        //dtpOTDateFrom.InitialText = "Select date";
        //Page_Load(null, null);
        query.Value = "";
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        //btnGenerate_Click(sender, e);

        //ASPxGridView1.DataBind();
        if (ASPxGridView1.VisibleRowCount > 1)
        { 
            GridViewExporterHeaderFooter headerFooter = ASPxGridViewExporter1.PageHeader;


            headerFooter.Left = Methods.GetCompanyInfoERP("Scm_CompName") + "\r\n\r\n"
                                 + Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
                                 + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
                                 + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
                                 + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos");

            //ASPxGridViewExporter1.ReportHeader = ddlReport.Items[Convert.ToInt32(hfSelectedRepIndex.Value)].Text;

            ASPxGridViewExporter1.WriteXlsToResponse(new DevExpress.XtraPrinting.XlsExportOptions(DevExpress.XtraPrinting.TextExportMode.Value));
        }
        else
            MessageBox.Show("No Data Found");
               
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
         //btnGenerate_Click(sender, e);

         if (ASPxGridView1.VisibleRowCount > 1)
         {
             GridViewExporterHeaderFooter headerFooter = ASPxGridViewExporter1.PageHeader;


             headerFooter.Left = Methods.GetCompanyInfoERP("Scm_CompName") + "\r\n\r\n"
                                 + Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
                                 + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
                                 + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
                                 + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos");

             // ASPxGridViewExporter1.ReportHeader = ddlReport.Items[Convert.ToInt32(hfSelectedRepIndex.Value)].Text;
             ASPxGridViewExporter1.Landscape = false;
             ASPxGridViewExporter1.BottomMargin = 0;
             ASPxGridViewExporter1.LeftMargin = 0;
             ASPxGridViewExporter1.RightMargin = 0;
             ASPxGridViewExporter1.PaperKind = System.Drawing.Printing.PaperKind.A4;
             ASPxGridViewExporter1.Styles.Cell.Font.Size = FontUnit.Small;
             ASPxGridViewExporter1.Styles.Header.Font.Size = FontUnit.Small;
             ASPxGridViewExporter1.Styles.Footer.Font.Size = FontUnit.Small;
             ASPxGridViewExporter1.Styles.GroupFooter.Font.Size = FontUnit.Small;
             ASPxGridViewExporter1.Styles.GroupRow.Font.Size = FontUnit.Small;

             headerFooter.VerticalAlignment = DevExpress.XtraPrinting.BrickAlignment.None;
             ASPxGridViewExporter1.WritePdfToResponse();
         }
         else
             MessageBox.Show("No Data Found");
               

    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {

    }
    protected void NextPrevButton(object sender, EventArgs e)
    {

    }
}