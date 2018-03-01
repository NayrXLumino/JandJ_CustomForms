using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using System.Data;
using DevExpress.Web.ASPxGridView;
using Payroll.DAL;

/// <summary>
/// Summary description for CheckListBL
/// </summary>
public class CheckListBL
{
    
    //public enum ChecklistTransactionTypes
    //{
    //    [StringValue("OVERTIME")]
    //    OVERTIME = 0,
    //    [StringValue("LEAVE")]
    //    LEAVE = 1,
    //    [StringValue("TIMEMODIFICATION")]
    //    TIMEMODIFICATION = 2,
    //    [StringValue("ADDRESS")]
    //    ADDRESS = 3,
    //    [StringValue("BENEFICIARY")]
    //    BENEFICIARY = 4,
    //    [StringValue("TAXCIVIL")]
    //    TAXCIVIL = 5,
    //    [StringValue("GROUP")]
    //    GROUP = 6,
    //    [StringValue("COSTCENTER")]
    //    COSTCENTER = 7,
    //    [StringValue("SHIFT")]
    //    SHIFT = 8,
    //    [StringValue("RESTDAY")]
    //    RESTDAY = 9,
    //    [StringValue("JOBSPLIT")]
    //    JOBSPLIT = 10,
    //    [StringValue("FLEXTIME")]
    //    FLEXTIME = 11,
    //    [StringValue("STRAIGHTWORK")]
    //    STRAIGHTWORK = 12,
    //    [StringValue("NONE")]
    //    NONE = 13,
    //    [StringValue("WORKINFORMATION")]
    //    WORKINFORMATION = 14
    //}
	public CheckListBL()
	{
		//
		// TODO: Add constructor logic here
		//
         
	}
    public string getOvertimeQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLD query SQL
//        string sql = @"  
//SELECT
//     ISNULL(Convert(varchar(10),[Transaction Date],101),'')+'|'+ ISNULL(Convert(varchar(10),[Start Time]),'')
//		+'|'+ ISNULL(Convert(varchar(10),[End Time]),'')
//		+'|'+ ISNULL(Convert(varchar(10),[Hours]),'')
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[OT type] as [Key]
//    ,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//	,[Start Time]
//	,[End Time]
//	,[Hours]
//	,[Line]
//    ,''[Col 06]
//	,[Section]
//    ,[Type]
//	,COUNT(Eot_ControlNo)[Transaction Count]
//	FROM (
//				SELECT 
//					Eot_ControlNo
//					,Eot_OvertimeDate [Transaction Date]
//					,Eot_StartTime[Start Time]
//					,Eot_EndTime[End Time]
//					,Eot_OvertimeHour[Hours]
//					, Eot_CostCenterLine [Line]
//					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) [Section]
//                    , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
//                    ,LEFT(Eot_Costcenter, 6)[Section Code]
//                    ,Eot_CostCenterLine[Line Code]
//                    ,Pmx_ParameterValue[OT type]
//				FROM T_EmployeeOvertime
//				LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//					ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
//				AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
//				LEFT JOIN T_ParameterMasterExt
//					ON Pmx_ParameterValue = Eot_OvertimeType
//				AND Pmx_ParameterID = 'OTTYPE'
//				INNER JOIN T_ApprovalRouteMaster AS routeMaster
//					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//                @CONDITION
//				UNION
//
//				SELECT 
//					Eot_ControlNo
//					,Eot_OvertimeDate [Transaction Date]
//					,Eot_StartTime[Start Time]
//					,Eot_EndTime[End Time]
//					,Eot_OvertimeHour[Hours]
//					, Eot_CostCenterLine [Line]
//					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) [Section]
//                    , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
//                    ,LEFT(Eot_Costcenter, 6)[Section Code]
//                    ,Eot_CostCenterLine[Line Code]
//                    ,Pmx_ParameterValue[OT type]
//
//				FROM T_EmployeeOvertimeHist
//				LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//					ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
//				AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
//				LEFT JOIN T_ParameterMasterExt
//					ON Pmx_ParameterValue = Eot_OvertimeType
//				AND Pmx_ParameterID = 'OTTYPE'
//				INNER JOIN T_ApprovalRouteMaster AS routeMaster
//					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//                @CONDITION
//				)
//				AS TEMP
//GROUP BY  Convert(varchar(10),[Transaction Date],101)+'|'+Convert(varchar(10),[Start Time])
//		+'|'+Convert(varchar(10),[End Time])
//		+'|'+Convert(varchar(10),[Hours])
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[OT type]
//    ,[Transaction Date]
//	,[Start Time]
//	,[End Time]
//	,[Hours]
//	,[Line]
//	,[Section]
//    ,[Type]
//ORDER BY [Transaction Date] DESC
//	,[Start Time]
//	,[End Time]
//	,[Hours]
//	,[Line]
//	,[Section]
//    ,[Type]
//";
        #endregion

        string sql = @"SELECT  STUFF((SELECT '|' + Eot_ControlNo
           FROM T_EmployeeOvertime intern
           INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
	            ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
            AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
			AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
            INNER JOIN T_ApprovalRouteMaster AS routeMaster
	            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                AND (@CONDITION)
	            LEFT JOIN T_ParameterMasterExt
		            ON Pmx_ParameterValue = Eot_OvertimeType
	            AND Pmx_ParameterID = 'OTTYPE'
           WHERE extern.Eot_OvertimeDate = intern.Eot_OvertimeDate
		    and extern.Eot_OvertimeHour = intern.Eot_OvertimeHour
		    and extern.Eot_StartTime = intern.Eot_StartTime
		    and extern.Eot_EndTime = intern.Eot_EndTime
		    and extern.Eot_CostCenterLine = intern.Eot_CostCenterLine
		    and extern.Eot_Costcenter = intern.Eot_Costcenter
		    and extern.Eot_OVertimeType = intern.Eot_OVertimeType
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Eot_OvertimeDate,101)[Transaction Date]
	, count(Eot_ControlNo)[Transaction Count]
	,Eot_OvertimeHour[Hours]
	,Eot_StartTime[Start Time]
	,Eot_EndTime[End Time]
	,Eot_CostCenterLine[Line]
    , '' [Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(MAX(Eot_Costcenter), 6)) [Section]
	,ISNULL(MAX(Pmx_ParameterDesc), '-no overtime type setup-')[Type]
FROM T_EmployeeOvertime AS extern
INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
	ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
INNER JOIN T_ApprovalRouteMaster AS routeMaster
	ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
    AND (@CONDITION)
	LEFT JOIN T_ParameterMasterExt
		ON Pmx_ParameterValue = Eot_OvertimeType
	AND Pmx_ParameterID = 'OTTYPE'
GROUP BY Eot_OvertimeDate
    ,Eot_OvertimeHour
	,Eot_StartTime
	,Eot_EndTime
	,Eot_CostCenterLine
	,Eot_OVertimeType
	,Eot_Costcenter
ORDER BY [Transaction Date]DESC
";
        string condition = getConditionBasedOnTransaction("OT", ColNumber, isAllAccess, isCurrentQuincena);
        sql =sql.Replace("@CONDITION", condition.Replace("WHERE", ""));
        //condition = condition.Replace("WHERE (", "WHERE((");
        //sql = sql.Replace("@crossConDITion", condition);
        //sql += " ORDER BY [Transaction Date] DESC,  [Employee Name] ASC ";

        
         return sql;
    }

    public string getOvertimeQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena ,string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0}
				SELECT 
					Eot_ControlNo[Control No]
					,Eot_OvertimeDate [Transaction Date]
					,Eot_StartTime[Start Time]
					,Eot_EndTime[End Time]
					,Eot_OvertimeHour[Hours]
                    ,Eot_Status[Status]
					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 8)) [Line]
					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) [Section]
                    , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type Code]
                    ,''[Section Code]
                    ,''[Line Code]
                    ,Pmx_ParameterValue[OT type]
                    ,case when Eot_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Eot_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Eot_Status!=3)
								then 9
								else 
									case when (Eot_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Eot_Status!=7 )
											OR (Eot_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
				FROM T_EmployeeOvertime
                INNER JOIN #SelectedControlNumbers
                ON ControlNo = Eot_ControlNo COLLATE DATABASE_DEFAULT
				LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
					ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
				AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
            	AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
				LEFT JOIN T_ParameterMasterExt
					ON Pmx_ParameterValue = Eot_OvertimeType
				AND Pmx_ParameterID = 'OTTYPE'
				INNER JOIN T_ApprovalRouteMaster AS routeMaster
					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                @CONDITION
				UNION

				SELECT 
					Eot_ControlNo
					,Eot_OvertimeDate [Transaction Date]
					,Eot_StartTime[Start Time]
					,Eot_EndTime[End Time]
					,Eot_OvertimeHour[Hours]
                    ,Eot_Status[Status]
					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 8)) [Line]
					, dbo.getCostCenterFullNameV2(LEFT(Eot_Costcenter, 6)) [Section]
                    , ISNULL(Pmx_ParameterDesc, '-no overtime type setup-') [Type]
                    ,''[Section Code]
                    ,''[Line Code]
                    ,Pmx_ParameterValue[OT type]
                    ,case when Eot_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Eot_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Eot_Status!=3)
								then 9
								else 
									case when (Eot_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Eot_Status!=7 )
											OR (Eot_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
				FROM T_EmployeeOvertimeHist
                INNER JOIN #SelectedControlNumbers
                ON ControlNo = Eot_ControlNo COLLATE DATABASE_DEFAULT
				LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
					ON empApprovalRoute.Arm_EmployeeId = Eot_EmployeeId
				AND empApprovalRoute.Arm_TransactionId = 'OVERTIME'
            	AND Eot_OvertimeDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
				LEFT JOIN T_ParameterMasterExt
					ON Pmx_ParameterValue = Eot_OvertimeType
				AND Pmx_ParameterID = 'OTTYPE'
				INNER JOIN T_ApprovalRouteMaster AS routeMaster
					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                @CONDITION
", getAdditionalConditionBasedOnTransaction("OT", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("OT", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql=sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getLeaveDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLDSQL
//        string sql = @"  
//SELECT Convert(varchar(10),[Transaction Date],101)+'|'+Convert(varchar(10),[Start Time])
//		+'|'+Convert(varchar(10),[End Time])
//		+'|'+Convert(varchar(10),[Hours])
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code] as [Key]
//		,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//	,[Start Time]
//	,[End Time]
//	,[Hours]
//	,[Line]
//    ,''[Col 06]
//	,[Section]
//    ,[Type]
//	,COUNT(Elt_ControlNo)[Transaction Count] FROM (
//
//SELECT Elt_ControlNo
//		, Elt_LeaveDate[Transaction Date]
//		, Elt_StartTime[Start Time]
//		, Elt_EndTime[End Time]
//		, Elt_LeaveHour[Hours]
//		, Elt_CostCenterLine [Line]
//		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) [Section]
//		,LEFT(Elt_Costcenter, 6)[Section Code]
//        ,Elt_CostCenterLine[Line Code]
//		, Elt_LeaveType[Type Code]
//		,Ltm_LeaveDesc[Type]
//FROM T_EmployeeLeaveAvailment
//	LEFT JOIN T_LeaveTypeMaster
//		ON Ltm_LeaveType = Elt_LeaveType
//	LEFT JOIN T_AccountDetail ADT2
//		ON ADT2.Adt_AccountType = 'LVECATEGRY'
//		AND ADT2.Adt_AccountCode = Elt_LeaveCategory
//	LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
//		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
//	INNER JOIN T_ApprovalRouteMaster AS routeMaster
//					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//					@CONDITION
//UNION
//
//SELECT Elt_ControlNo
//		, Elt_LeaveDate[Transaction Date]
//		, Elt_StartTime[Start Time]
//		, Elt_EndTime[End Time]
//		, Elt_LeaveHour[Hours]
//		, Elt_CostCenterLine [Line]
//		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) [Section]
//		,LEFT(Elt_Costcenter, 6)[Section Code]
//        ,Elt_CostCenterLine[Line Code]
//		, Elt_LeaveType[Type Code]
//		,Ltm_LeaveDesc[Type]
//FROM T_EmployeeLeaveAvailmentHist
//	LEFT JOIN T_LeaveTypeMaster
//		ON Ltm_LeaveType = Elt_LeaveType
//	LEFT JOIN T_AccountDetail ADT2
//		ON ADT2.Adt_AccountType = 'LVECATEGRY'
//		AND ADT2.Adt_AccountCode = Elt_LeaveCategory
//	LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
//		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
//	INNER JOIN T_ApprovalRouteMaster AS routeMaster
//					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//					@CONDITION
//					)AS TEMP
//
//GROUP BY Convert(varchar(10),[Transaction Date],101)+'|'+Convert(varchar(10),[Start Time])
//		+'|'+Convert(varchar(10),[End Time])
//		+'|'+Convert(varchar(10),[Hours])
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//    ,[Transaction Date]
//	,[Start Time]
//	,[End Time]
//	,[Hours]
//	,[Line]
//	,[Section]
//    ,[Type]
//";
        #endregion
        string sql = @"SELECT  STUFF((SELECT '|' + Elt_ControlNo
           FROM T_EmployeeLeaveAvailment intern
		   INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
		AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
	    INNER JOIN T_ApprovalRouteMaster AS routeMaster
					    ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
					    AND (@CONDITION)
               WHERE extern.Elt_LeaveDate = intern.Elt_LeaveDate
		    and extern.Elt_LeaveHour = intern.Elt_LeaveHour
		    and extern.Elt_StartTime = intern.Elt_StartTime
		    and extern.Elt_EndTime = intern.Elt_EndTime
		    and extern.Elt_CostCenterLine = intern.Elt_CostCenterLine
		    and extern.Elt_Costcenter = intern.Elt_Costcenter
		    and extern.Elt_LeaveType = intern.Elt_LeaveType
              FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Elt_LeaveDate,101)[Transaction Date]
	, count(Elt_ControlNo)[Transaction Count]
	,Elt_LeaveHour[Hours]
	,Elt_StartTime[Start Time]
	,Elt_EndTime[End Time]
	,Elt_CostCenterLine[Line]
	,''[Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(MAX(Elt_Costcenter), 6)) [Section]
	,MAX(Ltm_LeaveDesc) [Type]
FROM T_EmployeeLeaveAvailment AS extern
	INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
		AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
	INNER JOIN T_ApprovalRouteMaster AS routeMaster
					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                    AND (@CONDITION)
	LEFT JOIN T_LeaveTypeMaster
		ON Ltm_LeaveType = Elt_LeaveType
             
GROUP BY Elt_LeaveDate
    ,Elt_LeaveHour
	,Elt_StartTime
	,Elt_EndTime
	,Elt_CostCenterLine
	,Elt_LeaveType
	,Elt_Costcenter
ORDER BY [Transaction Date]DESC";
        string condition = getConditionBasedOnTransaction("LV", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));
        //sql += " ORDER BY [Transaction Date] DESC,  [Employee Name] ASC ";


        return sql;
        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("LV", ColNumber, isAllAccess, isCurrentQuincena));

        //sql += getConditionBasedOnTransaction("LV", ColNumber, isAllAccess, isCurrentQuincena);
       
        
    }

    public string getLeaveDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0}
				SELECT Elt_ControlNo[Control No]
		, Elt_LeaveDate[Transaction Date]
		, Elt_StartTime[Start Time]
		, Elt_EndTime[End Time]
		, Elt_LeaveHour[Hours]
		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 8)) [Line]
		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) [Section]
        , Elt_Status[Status]
		,''[Section Code]
        ,''[Line Code]
		, Elt_LeaveType[Type Code]
		,Ltm_LeaveDesc[Type]
        ,case when Elt_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Elt_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Elt_Status!=3)
								then 9
								else 
									case when (Elt_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Elt_Status!=7 )
											OR (Elt_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
FROM T_EmployeeLeaveAvailment
INNER JOIN #SelectedControlNumbers
ON ControlNo = Elt_ControlNo COLLATE DATABASE_DEFAULT
	LEFT JOIN T_LeaveTypeMaster
		ON Ltm_LeaveType = Elt_LeaveType
	LEFT JOIN T_AccountDetail ADT2
		ON ADT2.Adt_AccountType = 'LVECATEGRY'
		AND ADT2.Adt_AccountCode = Elt_LeaveCategory
	LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
		AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
	INNER JOIN T_ApprovalRouteMaster AS routeMaster
					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
					@CONDITION
UNION

SELECT Elt_ControlNo
		, Elt_LeaveDate[Transaction Date]
		, Elt_StartTime[Start Time]
		, Elt_EndTime[End Time]
		, Elt_LeaveHour[Hours]
		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 8)) [Line]
		, dbo.getCostCenterFullNameV2(LEFT(Elt_Costcenter, 6)) [Section]
        , Elt_Status
		,''[Section Code]
        ,''[Line Code]
		, Elt_LeaveType[Type Code]
		,Ltm_LeaveDesc[Type]
        ,case when Elt_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Elt_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Elt_Status!=3)
								then 9
								else 
									case when (Elt_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Elt_Status!=7 )
											OR (Elt_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
FROM T_EmployeeLeaveAvailmentHist
INNER JOIN #SelectedControlNumbers
ON ControlNo = Elt_ControlNo COLLATE DATABASE_DEFAULT
	LEFT JOIN T_LeaveTypeMaster
		ON Ltm_LeaveType = Elt_LeaveType
	LEFT JOIN T_AccountDetail ADT2
		ON ADT2.Adt_AccountType = 'LVECATEGRY'
		AND ADT2.Adt_AccountCode = Elt_LeaveCategory
	LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
		ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
		AND empApprovalRoute.Arm_TransactionId = 'LEAVE'
		AND Elt_LeaveDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
	INNER JOIN T_ApprovalRouteMaster AS routeMaster
					ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
					@CONDITION
", getAdditionalConditionBasedOnTransaction("LV", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("LV", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getTimeModificationDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLDSQL
//        string sql = @"  
//SELECT Convert(varchar(10),[Transaction Date],101)+'|'
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Mod type] as [Key]
//		,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//        ,''[Start Time]
//	    ,''[End Time]
//    	,''[Hours]
//        ,''[Col 06]
//		,[Line]
//		,[Section]
//		,[Type]
//		,COUNT(Trm_ControlNo)[Transaction Count]
//		FROM (
//
//					SELECT Trm_ControlNo
//										,Trm_ModDate [Transaction Date]
//										, Trm_CostCenterLine [Line]
//										, dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 6)) [Section]
//										, ADT2.Adt_AccountDesc [Type]
//										,LEFT(Trm_Costcenter, 6)[Section Code]
//										,Trm_CostCenterLine[Line Code]
//										,Trm_Type[Mod Type] FROM T_TimeRecMod
//					LEFT JOIN T_EmployeeLogLedger E1
//						ON E1.Ell_ProcessDate = Trm_ModDate
//					AND E1.Ell_EmployeeId = Trm_EmployeeId
//					LEFT JOIN T_EmployeeLogLedgerHist E2
//						ON E2.Ell_ProcessDate = Trm_ModDate
//					AND E2.Ell_EmployeeId = Trm_EmployeeId
//					LEFT JOIN T_AccountDetail ADT2
//						ON ADT2.Adt_AccountType = 'TMERECTYPE'
//					AND ADT2.Adt_AccountCode = Trm_Type
//					LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//						ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
//					AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
//					INNER JOIN T_ApprovalRouteMaster AS routeMaster
//						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//
//						@CONDITION
//						)AS TEMP
//GROUP BY  Convert(varchar(10),[Transaction Date],101)+'|'
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Mod type]
//    ,[Transaction Date]
//	,[Line]
//	,[Section]
//    ,[Type]
//   
//ORDER BY [Transaction Date] DESC
//	,[Line]
//	,[Section]
        //    ,[Type] ";


        #endregion
        string sql = @"SELECT  STUFF((SELECT '|' + Trm_ControlNo
           FROM T_TimeRecMod intern
		   INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
					AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						AND (@CONDITION)
           WHERE extern.Trm_ModDate = intern.Trm_ModDate
		and extern.Trm_CostCenterLine = intern.Trm_CostCenterLine
		and extern.Trm_Costcenter = intern.Trm_Costcenter
		and extern.Trm_Type = intern.Trm_Type
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Trm_ModDate,101)[Transaction Date]
	, count(Trm_ControlNo)[Transaction Count]
	,''[Hours]
	,''[Start Time]
	,''[End Time]
	,Trm_CostCenterLine[Line]
	,''[Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(MAX(Trm_Costcenter), 6)) [Section]
	,MAX(ADT2.Adt_AccountDesc) [Type]
FROM T_TimeRecMod AS extern
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
					AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                        AND (@CONDITION)
					LEFT JOIN T_AccountDetail ADT2
						ON ADT2.Adt_AccountType = 'TMERECTYPE'
					AND ADT2.Adt_AccountCode = Trm_Type

GROUP BY Trm_ModDate
	,Trm_CostCenterLine
	,Trm_Type
	,Trm_Costcenter
ORDER BY [Transaction Date]DESC
";
        string condition = getConditionBasedOnTransaction("TR", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));
        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("TR", ColNumber, isAllAccess, isCurrentQuincena));
        
        return sql;
    }

    public string getTimeModificationDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0}
				SELECT Trm_ControlNo[Control No]
										,Trm_ModDate [Transaction Date]
										, dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 8)) [Line]
										, dbo.getCostCenterFullNameV2(LEFT(Trm_Costcenter, 6)) [Section]
										, ADT2.Adt_AccountDesc [Type]
                                        , Trm_Status[Status]
										,''[Section Code]
										,''[Line Code]
										,Trm_Type[Type Code]
                                        ,case when Trm_Status not in ('3','5','7')
										then 10
										else
											case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
															or Trm_Status=7 
															or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Trm_Status!=3)
												then 9
												else 
													case when (Trm_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
															or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Trm_Status!=7 )
															OR (Trm_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
														then 7
														else 5
														end
												end
										end[Next Status]
                     FROM T_TimeRecMod
                    INNER JOIN #SelectedControlNumbers
                    ON ControlNo = Trm_ControlNo COLLATE DATABASE_DEFAULT
					LEFT JOIN T_EmployeeLogLedger E1
						ON E1.Ell_ProcessDate = Trm_ModDate
					AND E1.Ell_EmployeeId = Trm_EmployeeId
					LEFT JOIN T_EmployeeLogLedgerHist E2
						ON E2.Ell_ProcessDate = Trm_ModDate
					AND E2.Ell_EmployeeId = Trm_EmployeeId
					LEFT JOIN T_AccountDetail ADT2
						ON ADT2.Adt_AccountType = 'TMERECTYPE'
					AND ADT2.Adt_AccountCode = Trm_Type
					LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Trm_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'TIMEMOD'
                    AND Trm_ModDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId

						@CONDITION
", getAdditionalConditionBasedOnTransaction("TR", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("TR", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getBeneficiaryDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
        {
            #region SQL
//            string sql = @"   
//SELECT Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code] as [Key]
//		,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//        ,''[Start Time]
//	    ,''[End Time]
//    	,''[Hours]
//		,[Line]
//        ,''[Col 06]
//		,[Section]
//		,[Type]
//		,COUNT(But_ControlNo)[Transaction Count]
//		FROM (
//							SELECT        
//						But_ControlNo
//						, But_EffectivityDate [Transaction Date]
//						, 'NEW'[Type] --But_Type[Type]
//						, But_CostCenterLine [Line]
//						, dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
//						,LEFT(But_Costcenter, 6)[Section Code]
//						,But_CostCenterLine[Line Code]
//						,But_Type[Type Code]
//					FROM T_BeneficiaryUpdate
//					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
//						ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
//						AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
//					LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
//						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//						@CONDITION
//			) as Temp
//
//GROUP BY   Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//		,Convert(varchar(10),[Transaction Date],101)
//		,[Line]
//		,[Section]
//		,[Type]
//   
//ORDER BY [Transaction Date] DESC
//	,[Line]
//	,[Section]
//    ,[Type]   ";
            #endregion
            string sql = @"SELECT  STUFF((SELECT '|' + But_ControlNo
           FROM T_BeneficiaryUpdate intern
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
						AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						AND (@CONDITION)
           WHERE extern.But_EffectivityDate = intern.But_EffectivityDate
		                                and extern.But_CostCenterLine = intern.But_CostCenterLine
		                                and extern.But_Costcenter = intern.But_Costcenter
		                                and extern.But_Type = intern.But_Type  
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),But_EffectivityDate,101)[Transaction Date]
	, count(But_ControlNo)[Transaction Count]
	,''[Hours]
	,''[Start Time]
	,''[End Time]
	,But_CostCenterLine[Line]
	,''[Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
	,CASE WHEN (But_Type = 'N') 
			                          THEN 'NEW ENTRY'
			                          ELSE 'UPDATE EXISTING' END [Type]
FROM T_BeneficiaryUpdate AS extern
INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
						AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                        AND (@CONDITION)
GROUP BY But_EffectivityDate
	,But_CostCenterLine
	,But_Type
	,But_Costcenter
ORDER BY [Transaction Date]DESC";
            string condition = getConditionBasedOnTransaction("BF", ColNumber, isAllAccess, isCurrentQuincena);
            sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));
            //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("BF", ColNumber, isAllAccess, isCurrentQuincena));
            //sql += getConditionBasedOnTransaction("BF", ColNumber, isAllAccess, isCurrentQuincena);

            //sql += " ORDER BY [Transaction Date] DESC,  [Employee Name] ASC ";
           
            return sql;
        }

    public string getBeneficiaryDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0}
					SELECT        
						But_ControlNo[Control No]
						, But_EffectivityDate [Transaction Date]
						, But_Type[Type Code]
						, dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 8)) [Line]
						, dbo.getCostCenterFullNameV2(LEFT(But_Costcenter, 6)) [Section]
                        , But_Status[Status]
						,''[Section Code]
						,''[Line Code]
						,But_Type[Type Code]
                        ,case when But_Status not in ('3','5','7')
						    then 10
						    else
							    case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											    or But_Status=7 
											    or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and But_Status!=3)
								    then 9
								    else 
									    case when (But_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											    or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and But_Status!=7 )
											    OR (But_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										    then 7
										    else 5
										    end
								    end
						    end[Next Status]
					FROM T_BeneficiaryUpdate
                    INNER JOIN #SelectedControlNumbers
                    ON ControlNo = But_ControlNo COLLATE DATABASE_DEFAULT
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
                        AND But_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						@CONDITION
", getAdditionalConditionBasedOnTransaction("BF", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("BF", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getMovementDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLDSQL
//        string sql = @"  SELECT Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//		+'|'+ [From Code]
//		+'|'+ [To Code] as [Key]
//		,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//        ,[Move From][Start Time]
//	    ,[Move To][End Time]
//    	,''[Hours]
//        ,''[Col 06]
//		,[Line]
//		,[Section]
//		,[Type]
//		,COUNT(Mve_ControlNo)[Transaction Count]
//		FROM (
//							SELECT        
//						Mve_ControlNo
//						, Mve_EffectivityDate [Transaction Date]
//						, ADT2.Adt_AccountDesc [Type]
//						, Mve_CostCenterLine [Line]
//						, dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) [Section]
//						, Mve_From[From Code]
//						, Mve_To[To Code]
//						,LEFT(Mve_Costcenter, 6)[Section Code]
//						,Mve_CostCenterLine[Line Code]
//						,Mve_Type[Type Code]
//						, CASE WHEN (Mve_Type = 'S')
//								THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
//								WHEN (Mve_Type = 'G')
//								THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
//								WHEN (Mve_Type = 'C')
//								THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
//								ELSE Mve_From
//							END [Move From]
//						, CASE WHEN (Mve_Type = 'S')
//								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
//								WHEN (Mve_Type = 'G')
//								THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
//								WHEN (Mve_Type = 'C')
//								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
//								ELSE Mve_To
//							END [Move To]
//                        , CASE WHEN (Mve_Type = 'S')
//								THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
//								WHEN (Mve_Type = 'G')
//								THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
//								WHEN (Mve_Type = 'C')
//								THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
//								ELSE Mve_From
//							END [Move From Code]
//						, CASE WHEN (Mve_Type = 'S')
//								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
//								WHEN (Mve_Type = 'G')
//								THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
//								WHEN (Mve_Type = 'C')
//								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
//								ELSE Mve_To
//							END [Move To Code]
//					FROM T_Movement
//					LEFT JOIN T_AccountDetail ADT2
//						ON ADT2.Adt_AccountType = 'MOVETYPE'
//						AND ADT2.Adt_AccountCode = Mve_Type
//					LEFT JOIN T_ShiftCodeMaster S1
//						ON S1.Scm_ShiftCode = Mve_From
//						AND Mve_Type = 'S'
//					LEFT JOIN T_ShiftCodeMaster S2
//						ON S2.Scm_ShiftCode = Mve_To
//						AND Mve_Type = 'S'
//					LEFT JOIN T_AccountDetail A1
//						ON A1.Adt_AccountCode = LEFT(Mve_From, 3)
//						AND A1.Adt_AccountType = 'WORKTYPE'
//						AND Mve_Type = 'G'
//					LEFT JOIN T_AccountDetail A2
//						ON A2.Adt_AccountCode = LEFT(Mve_To, 3)
//						AND A2.Adt_AccountType = 'WORKTYPE'
//						AND Mve_Type = 'G'
//					LEFT JOIN T_AccountDetail A3
//						ON A3.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
//						AND A3.Adt_AccountType = 'WORKGROUP'
//						AND Mve_Type = 'G'
//					LEFT JOIN T_AccountDetail A4
//						ON A4.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
//						AND A4.Adt_AccountType = 'WORKGROUP'
//						AND Mve_Type = 'G'
//					LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
//						ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
//					AND empApprovalRoute.Arm_TransactionId = 'MOVEMENT'
//					INNER JOIN T_ApprovalRouteMaster AS routeMaster
//						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//						@CONDITION
//			) as Temp
//
//GROUP BY Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//		+'|'+ [From Code]
//		+'|'+ [To Code]
//		,Convert(varchar(10),[Transaction Date],101)
//        ,[Move From]
//	    ,[Move To]
//		,[Line]
//		,[Section]
//		,[Type]
//   
//   
//ORDER BY [Transaction Date] DESC
//	,[Line]
//	,[Section]
//    ,[Type] 
//";
        #endregion
        string sql = @"SELECT  STUFF((SELECT '|' + Mve_ControlNo
           FROM T_Movement intern
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'MOVEMENT'
					AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						AND (@CONDITION)
           WHERE  extern.Mve_EffectivityDate = intern.Mve_EffectivityDate
		and extern.Mve_CostCenterLine = intern.Mve_CostCenterLine
		and extern.Mve_Costcenter = intern.Mve_Costcenter
		and extern.Mve_Type = intern.Mve_Type
		and extern.Mve_From= intern.Mve_From
		and extern.Mve_To = intern.Mve_To 
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Mve_EffectivityDate,101)[Transaction Date]
	, count(Mve_ControlNo)[Transaction Count]
	,''[Hours]
	, CASE WHEN (Mve_Type = 'S')
								THEN S1.Scm_ShiftDesc --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc --LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN dbo.getCostCenterFullNameV2(Mve_From)--Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
								ELSE A3.Adt_AccountDesc
							END [Start Time]
						, CASE WHEN (Mve_Type = 'S')
								THEN S2.Scm_ShiftDesc--Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN A2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc --LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN dbo.getCostCenterFullNameV2(Mve_To)--Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
								ELSE A4.Adt_AccountDesc
							END [End Time]
	,Mve_CostCenterLine[Line]
	,''[Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(MAX(Mve_Costcenter), 6)) [Section]
	,MAX(ADT2.Adt_AccountDesc) [Type]
FROM T_Movement AS extern
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'MOVEMENT'
					AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                        AND (@CONDITION)
LEFT JOIN T_AccountDetail ADT2
						ON ADT2.Adt_AccountType = 'MOVETYPE'
						AND ADT2.Adt_AccountCode = Mve_Type
					LEFT JOIN T_ShiftCodeMaster S1
						ON S1.Scm_ShiftCode = Mve_From
						AND Mve_Type = 'S'
					LEFT JOIN T_ShiftCodeMaster S2
						ON S2.Scm_ShiftCode = Mve_To
						AND Mve_Type = 'S'
					LEFT JOIN T_AccountDetail A1
						ON A1.Adt_AccountCode = LEFT(Mve_From, 3)
						AND A1.Adt_AccountType = 'WORKTYPE'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A2
						ON A2.Adt_AccountCode = LEFT(Mve_To, 3)
						AND A2.Adt_AccountType = 'WORKTYPE'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A3
						ON A3.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
						AND A3.Adt_AccountType = 'WORKGROUP'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A4
						ON A4.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
						AND A4.Adt_AccountType = 'WORKGROUP'
						AND Mve_Type = 'G'

GROUP BY Mve_EffectivityDate
	,Mve_CostCenterLine
	, CASE WHEN (Mve_Type = 'S')
								THEN S1.Scm_ShiftDesc --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc --LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN dbo.getCostCenterFullNameV2(Mve_From)--Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
								ELSE A3.Adt_AccountDesc
							END 
						, CASE WHEN (Mve_Type = 'S')
								THEN S2.Scm_ShiftDesc--Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN A2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc --LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN dbo.getCostCenterFullNameV2(Mve_To)--Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
								ELSE A4.Adt_AccountDesc
							END 
	, Mve_Costcenter
	, Mve_Type
	, Mve_From
	, Mve_To
ORDER BY [Transaction Date]DESC";
        string condition = getConditionBasedOnTransaction("MV", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));

        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("MV", ColNumber, isAllAccess, isCurrentQuincena));
        //sql += getConditionBasedOnTransaction("MV", ColNumber, isAllAccess, isCurrentQuincena);

        
       
        return sql;
    }

    public string getMovementDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0}
					SELECT        
						Mve_ControlNo[Control No]
						, Mve_EffectivityDate [Transaction Date]
						, ADT2.Adt_AccountDesc [Type]
						, dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 8)) [Line]
						, dbo.getCostCenterFullNameV2(LEFT(Mve_Costcenter, 6)) [Section]
                        , Mve_Status[Status]
						, Mve_From[From Code]
						, Mve_To[To Code]
						,''[Section Code]
						,''[Line Code]
						,Mve_Type[Type Code]
						, CASE WHEN (Mve_Type = 'S')
								THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
								ELSE Mve_From
							END [Move From]
						, CASE WHEN (Mve_Type = 'S')
								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
								ELSE Mve_To
							END [Move To]
                        , CASE WHEN (Mve_Type = 'S')
								THEN Mve_From --Andre commented show only code as instructed ->S1.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN LEFT(Mve_From,3) + ' / ' + RIGHT(Mve_From,3) --Andre commented show only code as instructed ->A1.Adt_AccountDesc + '/' + A3.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN Mve_From --Andre commented show only code as instructed ->dbo.getCostCenterFullNameV2(Mve_From)
								ELSE Mve_From
							END [From Code]
						, CASE WHEN (Mve_Type = 'S')
								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDesc
								WHEN (Mve_Type = 'G')
								THEN LEFT(Mve_To,3) + ' / ' + RIGHT(Mve_To,3)--Andre commented show only code as instructed ->S2.Scm_ShiftDescA2.Adt_AccountDesc + ' / ' + A4.Adt_AccountDesc 
								WHEN (Mve_Type = 'C')
								THEN Mve_To --Andre commented show only code as instructed ->S2.Scm_ShiftDescdbo.getCostCenterFullNameV2(Mve_To)
								ELSE Mve_To
							END [To Code]
                        ,case when Mve_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Mve_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Mve_Status!=3)
								then 9
								else 
									case when (Mve_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Mve_Status!=7 )
											OR (Mve_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
					FROM T_Movement
                    INNER JOIN #SelectedControlNumbers
                    ON ControlNo = Mve_ControlNo COLLATE DATABASE_DEFAULT
					LEFT JOIN T_AccountDetail ADT2
						ON ADT2.Adt_AccountType = 'MOVETYPE'
						AND ADT2.Adt_AccountCode = Mve_Type
					LEFT JOIN T_ShiftCodeMaster S1
						ON S1.Scm_ShiftCode = Mve_From
						AND Mve_Type = 'S'
					LEFT JOIN T_ShiftCodeMaster S2
						ON S2.Scm_ShiftCode = Mve_To
						AND Mve_Type = 'S'
					LEFT JOIN T_AccountDetail A1
						ON A1.Adt_AccountCode = LEFT(Mve_From, 3)
						AND A1.Adt_AccountType = 'WORKTYPE'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A2
						ON A2.Adt_AccountCode = LEFT(Mve_To, 3)
						AND A2.Adt_AccountType = 'WORKTYPE'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A3
						ON A3.Adt_AccountCode = LTRIM(RIGHT(Mve_From, 3))
						AND A3.Adt_AccountType = 'WORKGROUP'
						AND Mve_Type = 'G'
					LEFT JOIN T_AccountDetail A4
						ON A4.Adt_AccountCode = LTRIM(RIGHT(Mve_To, 3))
						AND A4.Adt_AccountType = 'WORKGROUP'
						AND Mve_Type = 'G'
					LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
						ON empApprovalRoute.Arm_EmployeeId = Mve_EmployeeId
					AND empApprovalRoute.Arm_TransactionId = 'MOVEMENT'
                    AND Mve_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						@CONDITION
", getAdditionalConditionBasedOnTransaction("MV", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("MV", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getTaxCivilDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLDSQL
//        string sql = @"   
//SELECT Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//		+'|'+ [From Code]
//		+'|'+ [To Code]
//		+'|'+[Tax Code From]
//		+'|'+[Tax Code To] as [Key]
//		,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//        ,[Civil From][Start Time]
//	    ,[Civil To][End Time]
//    	,[Tax From][Hours]
//		,[Tax To][Col 06]
//		,[Line]
//		,[Section]
//		,[Type]
//		,COUNT(Pit_ControlNo)[Transaction Count]
//		FROM (
//							SELECT        
//						Pit_ControlNo
//						, Pit_EffectivityDate [Transaction Date]
//						, Pit_MoveType [Type]
//						, Pit_CostCenterLine [Line]
//						, dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) [Section]
//						,LEFT(Pit_Costcenter, 6)[Section Code]
//						,Pit_CostCenterLine[Line Code]
//						,Pit_MoveType[Type Code]
//                        , Pit_From [From Code]
//						, Pit_To [To Code]
//						,ADFCIVIL.Adt_AccountDesc[Civil From]
//						,ADTCIVIL.Adt_AccountDesc[Civil To]
//						,ADFTAX.Adt_AccountDesc[Tax From]
//						,ADTTAX.Adt_AccountDesc[Tax To]
//						, Pit_Filler1[Tax Code From]
//						, Pit_Filler1[Tax Code To]
//					FROM T_PersonnelInfoMovement
//					LEFT JOIN T_AccountDetail ADFTAX 
//						ON ADFTAX.Adt_AccountCode = Pit_From 
//						AND ADFTAX.Adt_AccountType =  'TAXCODE'
//					LEFT JOIN T_AccountDetail ADTTAX 
//						ON ADTTAX.Adt_AccountCode = Pit_To 
//						AND ADTTAX.Adt_AccountType =  'TAXCODE'
//					LEFT JOIN T_AccountDetail ADFCIVIL 
//						ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
//						AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
//					LEFT JOIN T_AccountDetail ADTCIVIL 
//						ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
//						AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'
//					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
//						ON empApprovalRoute.Arm_EmployeeId = Pit_EmployeeId
//						AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
//					LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
//						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//						@CONDITION
//			) as Temp
//
//GROUP BY Convert(varchar(10),[Transaction Date],101)
//		+'|'+Convert(varchar(10),[Line Code])
//		+'|'+[Section Code]+'|'+[Type Code]
//		+'|'+ [From Code]
//		+'|'+ [To Code]
//		+'|'+[Tax Code From]
//		+'|'+[Tax Code To] 
//		,Convert(varchar(10),[Transaction Date],101)
//        ,[Civil From]
//	    ,[Civil To]
//    	,[Tax From]
//		,[Tax To]
//		,[Line]
//		,[Section]
//		,[Type]
//   
//ORDER BY [Transaction Date] DESC
//	,[Line]
//	,[Section]
//    ,[Type] ";
        #endregion
        string sql = @"SELECT  STUFF((SELECT '|' + Pit_ControlNo
           FROM T_PersonnelInfoMovement intern
		   INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = Pit_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
						AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
    AND(@CONDITION)
           WHERE  extern.Pit_EffectivityDate = intern.Pit_EffectivityDate
		and extern.Pit_CostCenterLine = intern.Pit_CostCenterLine
		and extern.Pit_Costcenter = intern.Pit_Costcenter
		and extern.Pit_MoveType = intern.Pit_MoveType
		and extern.Pit_From= intern.Pit_From
		and extern.Pit_To = intern.Pit_To
		and extern.Pit_Filler1 = intern.Pit_Filler1
		and extern.Pit_Filler2 = intern.Pit_Filler2 
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Pit_EffectivityDate,101)[Transaction Date]
	, count(Pit_ControlNo)[Transaction Count]
	, MAX(ADFTAX.Adt_AccountDesc) [Hours]
	, MAX(ADFCIVIL.Adt_AccountDesc) [Start Time]
	, MAX(ADTCIVIL.Adt_AccountDesc) [End Time]
	,Pit_CostCenterLine[Line]
	, MAX(ADTTAX.Adt_AccountDesc) [Col 06]
	, dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) [Section]
	,Pit_MoveType[Type]
FROM T_PersonnelInfoMovement AS extern
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = Pit_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                        AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                    AND (@CONDITION)
                    LEFT JOIN T_AccountDetail ADFTAX 
						ON ADFTAX.Adt_AccountCode = Pit_From 
						AND ADFTAX.Adt_AccountType =  'TAXCODE'
					LEFT JOIN T_AccountDetail ADTTAX 
						ON ADTTAX.Adt_AccountCode = Pit_To 
						AND ADTTAX.Adt_AccountType =  'TAXCODE'
					LEFT JOIN T_AccountDetail ADFCIVIL 
						ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
						AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
					LEFT JOIN T_AccountDetail ADTCIVIL 
						ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
						AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'

GROUP BY Pit_EffectivityDate
	,Pit_CostCenterLine
	,Pit_Costcenter
	,Pit_MoveType
	,Pit_From
	,Pit_To
	,Pit_Filler1
	,Pit_Filler2 
ORDER BY [Transaction Date]DESC
";

        string condition = getConditionBasedOnTransaction("TX", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));
        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("TX", ColNumber, isAllAccess, isCurrentQuincena));
       
        
        return sql;
    }

    public string getTaxCivilDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" {0} 
						SELECT        
						Pit_ControlNo[Control No]
						, Pit_EffectivityDate [Transaction Date]
						, Pit_MoveType [Type]
						, dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 8)) [Line]
						, dbo.getCostCenterFullNameV2(LEFT(Pit_Costcenter, 6)) [Section]
						,''[Section Code]
						,''[Line Code]
						,Pit_MoveType[Type Code]
                        , Pit_From [From Code]
						, Pit_To [To Code]
                         ,Pit_Status[Status]
						,ADFCIVIL.Adt_AccountDesc[Civil From]
						,ADTCIVIL.Adt_AccountDesc[Civil To]
						,ADFTAX.Adt_AccountDesc[Tax From]
						,ADTTAX.Adt_AccountDesc[Tax To]
						, Pit_Filler1[Tax Code From]
						, Pit_Filler1[Tax Code To]
                        ,case when Pit_Status not in ('3','5','7')
						then 10
						else
							case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or Pit_Status=7 
											or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Pit_Status!=3)
								then 9
								else 
									case when (Pit_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Pit_Status!=7 )
											OR (Pit_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										then 7
										else 5
										end
								end
						end[Next Status]
					FROM T_PersonnelInfoMovement
                    INNER JOIN #SelectedControlNumbers
                    ON ControlNo = Pit_ControlNo COLLATE DATABASE_DEFAULT
					LEFT JOIN T_AccountDetail ADFTAX 
						ON ADFTAX.Adt_AccountCode = Pit_From 
						AND ADFTAX.Adt_AccountType =  'TAXCODE'
					LEFT JOIN T_AccountDetail ADTTAX 
						ON ADTTAX.Adt_AccountCode = Pit_To 
						AND ADTTAX.Adt_AccountType =  'TAXCODE'
					LEFT JOIN T_AccountDetail ADFCIVIL 
						ON ADFCIVIL.Adt_AccountCode = Pit_Filler1 
						AND ADFCIVIL.Adt_AccountType =  'CIVILSTAT'
					LEFT JOIN T_AccountDetail ADTCIVIL 
						ON ADTCIVIL.Adt_AccountCode = Pit_Filler2 
						AND ADTCIVIL.Adt_AccountType =  'CIVILSTAT'
					INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						ON empApprovalRoute.Arm_EmployeeId = Pit_EmployeeId
						AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                        AND Pit_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
						ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
						@CONDITION
", getAdditionalConditionBasedOnTransaction("TX", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("TX", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }

    public string getAddressDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        #region OLDSQL
//        string sql = @" SELECT Convert(varchar(10),[Transaction Date],101)
//					+'|'+Convert(varchar(10),[Line Code])
//					+'|'+[Section Code]+'|'+[Type Code]
//					 as [Key]
//		            ,Convert(varchar(10),[Transaction Date],101)[Transaction Date]
//                    ,''[Start Time]
//	                ,''[End Time]
//    	            ,''[Hours]
//		            ,''[Col 06]
//		            ,[Line]
//		            ,[Section]
//		            ,[Type]
//		            ,COUNT(Amt_ControlNo)[Transaction Count]
//		            FROM (
//							            SELECT        
//						            Amt_ControlNo
//						            , Amt_EffectivityDate [Transaction Date]
//						            , Amt_Type [Type Code]
//						            , Amt_CostCenterLine [Line]
//						            , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) [Section]
//						            ,LEFT(Amt_Costcenter, 6)[Section Code]
//						            ,Amt_CostCenterLine[Line Code]
//                                    , CASE Amt_Type 
//							            WHEN 'A1' THEN 'Present'
//							            WHEN 'A2' THEN 'Permanent'
//							            WHEN 'A3' THEN 'Emergency Contact'
//							            ELSE ''
//						            END [Type]
//					            FROM T_AddressMovement
//					            INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
//						            ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
//						            AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
//					            LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
//						            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
//					            LEFT JOIN T_AccountDetail ADRelation
//						            ON ADRelation.Adt_AccountCode = Amt_ContactRelation
//						            AND ADRelation.Adt_AccountType = 'RELATION'
//					            LEFT JOIN T_RouteMaster ON Rte_RouteCode = Amt_Filler1 
//							            AND Rte_EffectivityDate = (SELECT MAX(Rte_EffectivityDate) 
//								            FROM T_RouteMaster
//									            WHERE Rte_EffectivityDate <= Amt_EffectivityDate
//									            AND Rte_RouteCode = Amt_Filler1) 
//											            @CONDITION
//			            ) as Temp
//
//            GROUP BY Convert(varchar(10),[Transaction Date],101)
//					+'|'+Convert(varchar(10),[Line Code])
//					+'|'+[Section Code]+'|'+[Type Code]
//					
//		            ,Convert(varchar(10),[Transaction Date],101)
//                   
//		            ,[Line]
//		            ,[Section]
//		            ,[Type]
//   
//            ORDER BY [Transaction Date] DESC
//	            ,[Line]
//	            ,[Section]
//                ,[Type] 
//";
        #endregion
        string sql = @"SELECT  STUFF((SELECT '|' + Amt_ControlNo
           FROM T_AddressMovement AS intern
INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						            ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
						            AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
									AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
    AND (@CONDITION)
        WHERE  extern.Amt_EffectivityDate = intern.Amt_EffectivityDate
		and extern.Amt_CostCenterLine = intern.Amt_CostCenterLine
		and extern.Amt_Costcenter = intern.Amt_Costcenter
		and extern.Amt_Type = intern.Amt_Type
          FOR XML PATH('')), 1, 1, '') AS [Key]
	,Convert(varchar(10),Amt_EffectivityDate,101)[Transaction Date]
	, count(Amt_ControlNo)[Transaction Count]
	,''[Start Time]
	,''[End Time]
    ,''[Hours]
	,''[Col 06]
	,Amt_CostCenterLine[Line]
	, dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) [Section]
	, CASE Amt_Type 
							            WHEN 'A1' THEN 'Present'
							            WHEN 'A2' THEN 'Permanent'
							            WHEN 'A3' THEN 'Emergency Contact'
							            ELSE ''
						            END [Type]
FROM T_AddressMovement AS extern
INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						            ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
						            AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
									AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                    AND (@CONDITION)
GROUP BY Amt_EffectivityDate
	,Amt_CostCenterLine
	, Amt_Costcenter
	, Amt_Type
ORDER BY [Transaction Date]DESC";

        string condition = getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));

        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena));
       // sql += getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena);

        //sql += " ORDER BY [Transaction Date] DESC,  [Employee Name] ASC ";
        return sql;
    }

    public string getAddressDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@" 
						SELECT        
						            Amt_ControlNo[Control No]
						            , Amt_EffectivityDate [Transaction Date]
						            , Amt_Type [Type Code]
						            , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 8)) [Line]
						            , dbo.getCostCenterFullNameV2(LEFT(Amt_Costcenter, 6)) [Section]
						            ,''[Section Code]
						            ,''[Line Code]  
                                    ,Amt_Status[Status]
                                    , CASE Amt_Type 
							            WHEN 'A1' THEN 'Present'
							            WHEN 'A2' THEN 'Permanent'
							            WHEN 'A3' THEN 'Emergency Contact'
							            ELSE ''
						            END [Type]
                                   ,case when Amt_Status not in ('3','5','7')
						            then 10
						            else
							            case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											            or Amt_Status=7 
											            or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Amt_Status!=3)
								            then 9
								            else 
									            case when (Amt_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											            or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Amt_Status!=7 )
											            OR (Amt_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										            then 7
										            else 5
										            end
								            end
						            end[Next Status]
					            FROM T_AddressMovement
                                INNER JOIN #SelectedControlNumbers
                                ON ControlNo = Amt_ControlNo COLLATE DATABASE_DEFAULT
					            INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						            ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
						            AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
									AND Amt_EffectivityDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					            LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
						            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
					            LEFT JOIN T_AccountDetail ADRelation
						            ON ADRelation.Adt_AccountCode = Amt_ContactRelation
						            AND ADRelation.Adt_AccountType = 'RELATION'
					            LEFT JOIN T_RouteMaster ON Rte_RouteCode = Amt_Filler1 
							            AND Rte_EffectivityDate = (SELECT MAX(Rte_EffectivityDate) 
								            FROM T_RouteMaster
									            WHERE Rte_EffectivityDate <= Amt_EffectivityDate
									            AND Rte_RouteCode = Amt_Filler1) 
											            @CONDITION
", getAdditionalConditionBasedOnTransaction("AD", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }
    public string getGatePassDataQuery(string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        string sql = @"SELECT  STUFF((SELECT '|' + Egp_ControlNo
                            FROM E_EmployeeGatePass AS intern
                            INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						            ON empApprovalRoute.Arm_EmployeeId = Egp_EmployeeId
						            AND empApprovalRoute.Arm_TransactionID = 'GATEPASS'
									AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					            INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                AND (@CONDITION)
                                    WHERE  extern.Egp_GatePassDate = intern.Egp_GatePassDate
		                            and extern.Egp_Costcenter = intern.Egp_Costcenter
		                            and extern.Egp_ApplicationType = intern.Egp_ApplicationType
                                      FOR XML PATH('')), 1, 1, '') AS [Key]
	                            ,Convert(varchar(10),Egp_GatePassDate,101)[Transaction Date]
	                            , count(Egp_ControlNo)[Transaction Count]
	                            ,''[Start Time]
	                            ,''[End Time]
                                ,''[Hours]
	                            ,''[Col 06]
	                            ,''[Line]
	                            , dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 6)) [Section]
	                            , CASE Egp_ApplicationType 
		                                WHEN 'OB' THEN 'OFFICIAL BUSINESS'
		                                WHEN 'UT' THEN 'UNDERTIME'
                                        WHEN 'PL' THEN 'PERSONAL'
                                        WHEN 'OTH' THEN 'OTHERS'
							                                        ELSE ''
						                                        END [Type]
                            FROM E_EmployeeGatePass AS extern
                            INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
						                                        ON empApprovalRoute.Arm_EmployeeId = Egp_EmployeeId
						                                        AND empApprovalRoute.Arm_TransactionID = 'GATEPASS'
									                            AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
					                                        INNER JOIN T_ApprovalRouteMaster AS routeMaster 
						                                        ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                                                AND (@CONDITION)
                            GROUP BY Egp_GatePassDate
	                            , Egp_Costcenter
	                            , Egp_ApplicationType
                            ORDER BY [Transaction Date]DESC";

        string condition = getConditionBasedOnTransaction("GP", ColNumber, isAllAccess, isCurrentQuincena);
        sql = sql.Replace("@CONDITION", condition.Replace("WHERE", ""));

        //sql = sql.Replace("@CONDITION", getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena));
        // sql += getConditionBasedOnTransaction("AD", ColNumber, isAllAccess, isCurrentQuincena);

        //sql += " ORDER BY [Transaction Date] DESC,  [Employee Name] ASC ";
        return sql;
    }
    public string getGatePassDataQueryDetails(string ColNumber, bool isAllAccess, bool isCurrentQuincena, string additionalCondition)
    {
        #region SQL
        string sql = string.Format(@"  {0}
                                SELECT Egp_ControlNo[Control No]
		                            , Egp_GatePassDate[Transaction Date]
		                            , dbo.getCostCenterFullNameV2(LEFT(Egp_Costcenter, 6)) [Section]
                                    , Egp_Status[Status]
		                            , CASE Egp_ApplicationType 
		                                WHEN 'OB' THEN 'OFFICIAL BUSINESS'
		                                WHEN 'UT' THEN 'UNDERTIME'
                                        WHEN 'PL' THEN 'PERSONAL'
                                        WHEN 'OTH' THEN 'OTHERS'
                                        ELSE ''
	                                END [Type]
                                    ,case when Egp_Status not in ('3','5','7')
						                            then 10
						                            else
							                            case when(routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											                            or Egp_Status=7 
											                            or (routeMaster.Arm_Checker2=routeMaster.Arm_Approver and Egp_Status!=3)
								                            then 9
								                            else 
									                            case when (Egp_Status=3 And routeMaster.Arm_Checker2=routeMaster.Arm_Approver)
											                            or (routeMaster.Arm_Checker1=routeMaster.Arm_Checker2 and Egp_Status!=7 )
											                            OR (Egp_Status=5 AND  routeMaster.Arm_Checker2!=routeMaster.Arm_Approver)
										                            then 7
										                            else 5
										                            end
								                            end
						                            end[Next Status]
                            FROM E_EmployeeGatePass
                            INNER JOIN #SelectedControlNumbers
                            ON ControlNo = Egp_ControlNo COLLATE DATABASE_DEFAULT
	                            LEFT JOIN T_EmployeeApprovalRoute AS empApprovalRoute
		                            ON empApprovalRoute.Arm_EmployeeId = Egp_EmployeeId
		                            AND empApprovalRoute.Arm_TransactionId = 'GATEPASS'
		                            AND Egp_GatePassDate BETWEEN empApprovalRoute.Arm_StartDate AND ISNULL(empApprovalRoute.Arm_EndDate, DATEADD(YEAR, 2, GETDATE()))
	                            INNER JOIN T_ApprovalRouteMaster AS routeMaster
					                            ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
					                            @CONDITION
", getAdditionalConditionBasedOnTransaction("GP", additionalCondition));
        #endregion
        string condition = string.Empty;
        condition += getConditionBasedOnTransaction("GP", ColNumber, isAllAccess, isCurrentQuincena) + ") ";
        condition = condition.Replace("WHERE (", "WHERE((");

        sql = sql.Replace("@CONDITION", condition);

        return sql;
    }
    public string getConditionBasedOnTransaction(string TransactionType, string ColNumber, bool isAllAccess, bool isCurrentQuincena)
    {
        string sqlCounters = string.Empty;
        string dateColumn = string.Empty;
        switch (TransactionType)
        {
            case "OT":
                #region Overtime
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Eot_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Eot_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Eot_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Eot_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Eot_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Eot_Status = '7' AND routeMaster.Arm_Approver = '{0}')";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Eot_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Eot_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Eot_Status IN ('1','3','5','7','N')  
        AND (Eot_EmployeeId = '{0}' 
                OR Eot_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'OVERTIME'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Eot_Status IN ('1','3','5','7','N')  AND Eot_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Eot_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Eot_EmployeeId = '{0}' 
                OR Eot_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'OVERTIME'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Eot_Status IN ('0','2','4','6','8','9','C','A','F')  AND Eot_EmployeeId = '{0}' ) ";
                }
                dateColumn = "Eot_OvertimeDate";
                #endregion
                break;
            case "LV":
                #region Leave
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Elt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Elt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Elt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Elt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Elt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Elt_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Elt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2) 
        OR ( Elt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Elt_Status IN ('1','3','5','7','N')
        AND (Elt_EmployeeId = '{0}' 
                OR Elt_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'LEAVE'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Elt_Status IN ('1','3','5','7','N')  AND Elt_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Elt_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Elt_EmployeeId = '{0}' 
                OR Elt_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'LEAVE'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Elt_Status IN ('0','2','4','6','8','9','C','A','F')  AND Elt_EmployeeId = '{0}' ) ";
                }
                dateColumn = "Elt_LeaveDate";
                #endregion
                break;
            case "TR":
                #region Time Mod
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Trm_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Trm_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Trm_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Trm_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Trm_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Trm_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Trm_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Trm_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Trm_Status IN ('1','3','5','7','N')
        AND (Trm_Status = '{0}' 
                OR Trm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Trm_Status IN ('1','3','5','7','N')  AND Trm_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Trm_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Trm_Status = '{0}' 
                OR Trm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Trm_Status IN ('0','2','4','6','8','9','C','A','F')  AND Trm_EmployeeId = '{0}' ) ";
                }
                dateColumn = "Trm_ModDate";
                #endregion
                break;
            case "FT":
                #region Flex
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Flx_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Flx_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Flx_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Flx_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Flx_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Flx_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Flx_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Flx_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Flx_Status IN ('1','3','5','7','N')
        AND (Flx_Status = '{0}' 
                OR Flx_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate,GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Flx_Status IN ('1','3','5','7','N')  AND Flx_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Flx_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Flx_Status = '{0}' 
                OR Flx_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Flx_Status IN ('0','2','4','6','8','9','C','A','F')  AND Flx_EmployeeId = '{0}' ) ";
                }
                dateColumn = "Flx_FlexDate";
                #endregion
                break;
            case "JS":
                #region Jobsplit
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE ((Jsh_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR  (Jsh_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR  (Jsh_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' ))
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD') ";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE (  ( Jsh_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
            OR ( Jsh_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
            OR ( Jsh_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            )
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD') ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE (( Jsh_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR  ( Jsh_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver ))
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD') ";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Jsh_Status IN ('1','3','5','7','N') 
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD')  
        AND (Jsh_Status = '{0}' 
                OR Jsh_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Jsh_Status IN ('1','3','5','7','N')  AND Jsh_EmployeeId = '{0}' )
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD')  ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Jsh_Status IN ('0','2','4','6','8','9','C','A','F') 
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD')  
        AND (Jsh_Status = '{0}' 
                OR Jsh_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                    {
                        sqlCounters += @" 
    WHERE ( Jsh_Status IN ('0','2','4','6','8','9','C','A','F')  AND Jsh_EmployeeId = '{0}' ) 
        AND LEFT(Jsh_ControlNo,1) = (SELECT Tcm_TransactionPrefix 
                                        FROM T_TransactionControlMaster
                                        WHERE Tcm_TransactionCode = 'JOBMOD') ";
                        if (isCurrentQuincena)
                            sqlCounters += ")";
                    }
                }
                dateColumn = "Jsh_JobSplitDate";
                #endregion
                break;
            case "MV":
                #region Movement
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Mve_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Mve_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Mve_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Mve_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Mve_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Mve_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Mve_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Mve_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Mve_Status IN ('1','3','5','7','N')
        AND (Mve_Status = '{0}' 
                OR Mve_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Mve_Status IN ('1','3','5','7','N')  AND Mve_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Mve_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Mve_Status = '{0}' 
                OR Mve_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Mve_Status IN ('0','2','4','6','8','9','C','A','F')  AND Mve_EmployeeId = '{0}' )  ";
                }
                dateColumn = "Mve_EffectivityDate";
                #endregion
                break;
            case "TX":
                #region Tax Civil
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Pit_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Pit_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Pit_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Pit_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Pit_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Pit_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Pit_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Pit_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Pit_Status IN ('1','3','5','7','N')
        AND (Pit_Status = '{0}' 
                OR Pit_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Pit_Status IN ('1','3','5','7','N')  AND Pit_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Pit_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Pit_Status = '{0}' 
                OR Pit_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Pit_Status IN ('0','2','4','6','8','9','C','A','F')  AND Pit_EmployeeId = '{0}' )  ";
                }
                dateColumn = "Pit_EffectivityDate";
                #endregion
                break;
            case "BF":
                #region Beneficiary
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (But_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (But_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (But_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( But_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( But_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( But_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( But_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( But_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( But_Status IN ('1','3','5','7','N')
        AND (But_Status = '{0}' 
                OR But_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( But_Status IN ('1','3','5','7','N')  AND But_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( But_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (But_Status = '{0}' 
                OR But_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( But_Status IN ('0','2','4','6','8','9','C','A','F')  AND But_EmployeeId = '{0}' )  ";
                }
                dateColumn = "But_EffectivityDate";
                #endregion
                break;
            case "AD":
                #region Address
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Amt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Amt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Amt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Amt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Amt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Amt_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Amt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Amt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Amt_Status IN ('1','3','5','7','N') 
        AND (Amt_Status = '{0}' 
                OR Arm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Amt_Status IN ('1','3','5','7','N')  AND Amt_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Amt_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Amt_Status = '{0}' 
                OR Arm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'PAYROLL'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Amt_Status IN ('0','2','4','6','8','9','C','A','F')  AND Amt_EmployeeId = '{0}' )  ";
                }
                dateColumn = "Amt_EffectivityDate";
                #endregion
                break;
            case "SW":
                #region Address
                if (ColNumber == "1")
                {
                    sqlCounters += @" 
    WHERE (Swt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
        OR (Swt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
        OR (Swt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @" 
    WHERE ( Swt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
        OR ( Swt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
        OR ( Swt_Status = '7' AND routeMaster.Arm_Approver = '{0}')
            ";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @" 
    WHERE ( Swt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
        OR ( Swt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Swt_Status IN ('1','3','5','7','N') 
        AND (Swt_Status = '{0}' 
                OR Arm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Swt_Status IN ('1','3','5','7','N')  AND Swt_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
    WHERE ( Swt_Status IN ('0','2','4','6','8','9','C','A','F')
        AND (Swt_Status = '{0}' 
                OR Arm_EmployeeId IN (
            SELECT 
	            DISTINCT APPROUTE.Arm_EmployeeID
            FROM T_EmployeeApprovalRoute APPROUTE
            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
            WHERE APPROUTE.Arm_TransactionID = 'TIMEKEEP'
	            AND (Arm_Checker1 = '{0}'
			            OR Arm_Checker2 = '{0}'
			            OR Arm_Approver = '{0}')
	            AND APPROUTE.Arm_Status = 'A'
            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
        )) ) ";
                    else
                        sqlCounters += @" 
    WHERE ( Swt_Status IN ('0','2','4','6','8','9','C','A','F')  AND Swt_EmployeeId = '{0}' )  ";
                }
                dateColumn = "Swt_FromDate";
                #endregion
                break;
            case "GP":
                #region Address
                if (ColNumber == "1")
                {
                    sqlCounters += @" WHERE (Egp_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                        OR (Egp_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                        OR (Egp_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )";
                }
                else if (ColNumber == "2")
                {
                    sqlCounters += @"WHERE ( Egp_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                        OR ( Egp_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                        OR ( Egp_Status = '7' AND routeMaster.Arm_Approver = '{0}')";
                }
                else if (ColNumber == "3")
                {
                    sqlCounters += @"WHERE ( Egp_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                            OR ( Egp_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )";
                }
                else if (ColNumber == "4")
                {
                    if (isAllAccess)
                        sqlCounters += @"WHERE ( Egp_Status IN ('1','3','5','7','N') 
                                            AND (Egp_Status = '{0}' 
                                                    OR Arm_EmployeeId IN (
                                                SELECT 
	                                                DISTINCT APPROUTE.Arm_EmployeeID
                                                FROM T_EmployeeApprovalRoute APPROUTE
                                                INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	                                                ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	                                                AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
                                                WHERE APPROUTE.Arm_TransactionID = 'PERSONNEL'
	                                                AND (Arm_Checker1 = '{0}'
			                                                OR Arm_Checker2 = '{0}'
			                                                OR Arm_Approver = '{0}')
	                                                AND APPROUTE.Arm_Status = 'A'
                                                AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
                                            )) ) ";
                    else
                        sqlCounters += @" 
                                        WHERE ( Egp_Status IN ('1','3','5','7','N')  AND Egp_EmployeeId = '{0}' ) ";
                }
                else if (ColNumber == "5")
                {
                    if (isAllAccess)
                        sqlCounters += @"
                                    WHERE ( Egp_Status IN ('0','2','4','6','8','9','C','A','F')
                                        AND (Egp_Status = '{0}' 
                                                OR Arm_EmployeeId IN (
                                            SELECT 
	                                            DISTINCT APPROUTE.Arm_EmployeeID
                                            FROM T_EmployeeApprovalRoute APPROUTE
                                            INNER JOIN T_ApprovalRouteMaster ROUTEMASTER
	                                            ON APPROUTE.Arm_RouteID = ROUTEMASTER.Arm_RouteId
	                                            AND APPROUTE.Arm_Status = ROUTEMASTER.Arm_Status
                                            WHERE APPROUTE.Arm_TransactionID = 'PERSONNEL'
	                                            AND (Arm_Checker1 = '{0}'
			                                            OR Arm_Checker2 = '{0}'
			                                            OR Arm_Approver = '{0}')
	                                            AND APPROUTE.Arm_Status = 'A'
                                            AND Convert(varchar,GETDATE(),101) BETWEEN APPROUTE.Arm_StartDate AND ISNULL(APPROUTE.Arm_EndDate, GETDATE())
                                        )) ) ";
                    else
                        sqlCounters += @" 
                                WHERE ( Egp_Status IN ('0','2','4','6','8','9','C','A','F')  AND Egp_EmployeeId = '{0}' )  ";
                }
                dateColumn = "Egp_GatePassDate";
                #endregion
                break;
            default:
                break;
        }
        if (sqlCounters != string.Empty)
        {
            if (isCurrentQuincena)
            {
                 DataSet ds=null;
                using(DALHelper dal = new DALHelper())
                {
                    try
                    {
                       ds = dal.ExecuteDataSet(" SELECT CONVERT(VARCHAR(20), Ppm_StartCycle, 101), CONVERT(VARCHAR(20), Ppm_EndCycle, 101) FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C' ");

                    }
                    catch
                    {
                        ds = null;
                    }
                } if (!CommonMethods.isEmpty(ds))
                {
                    sqlCounters = sqlCounters.Replace("WHERE", " WHERE ( ") + " ) AND (" + dateColumn + " <= '"
                                                + ds.Tables[0].Rows[0][1].ToString().Trim() + "' ) ";


                }
            }
            else
            {
                //apill hist
            }
        }
        return sqlCounters;
    }
    
    public string getAdditionalConditionBasedOnTransaction(string TransactionType,string Conditions)
    {
        string strMainQuery = @"CREATE TABLE #SelectedControlNumbers (
                                    ControlNo VARCHAR(12) NOT NULL
                                ) ";

        if (TransactionType == "OT" || TransactionType == "LV" || TransactionType == "TR" || TransactionType == "MV" || TransactionType == "TX" || TransactionType == "BF" || TransactionType == "AD" || TransactionType == "GP")
        {
            string strInsertIntoTemplate = @" 
                                         INSERT INTO #SelectedControlNumbers ";
            string strSelectStatement = "";
            int iSelectCtr = 0;
            string[] extractCondition = Conditions.Split('|');
            foreach (string ControlNumber in extractCondition)
            {
                if (ControlNumber != null)
                {
                    strSelectStatement += "SELECT '" + ControlNumber + "' UNION ALL ";
                    iSelectCtr++;
                }
                if (iSelectCtr == 100)
                {
                    strSelectStatement = strSelectStatement.Substring(0, strSelectStatement.Length - 10);
                    strMainQuery += strInsertIntoTemplate + strSelectStatement;
                    iSelectCtr = 0;
                    strSelectStatement = "";
                }
            }
            if (strSelectStatement != "")
            {
                strSelectStatement = strSelectStatement.Substring(0, strSelectStatement.Length - 10);
                strMainQuery += strInsertIntoTemplate + strSelectStatement;
            }
        }

        return strMainQuery;
    }

    public string getQueryBasedOnTransactionType(string TransactionType, string ddlCostcenter, string checkLevel, bool isAllAccess, string userID)
    {
        string query = string.Empty;
        switch (TransactionType)
        {
            case "OT":
                query = getOvertimeQuery(checkLevel, isAllAccess, false);
                break;
            case "LV":
                query = getLeaveDataQuery(checkLevel, isAllAccess, false);
                break;
            case "TR":
                query = getTimeModificationDataQuery(checkLevel, isAllAccess, false);
                break;
            //case "FT":
            //    query = getFlexTimeDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            //case "JS":
            //    query = getJobModificationDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            case "MV":
                query = getMovementDataQuery(checkLevel, isAllAccess, false);
                break;
            case "TX":
                query = getTaxCivilDataQuery(checkLevel, isAllAccess, false);
                break;
            case "BF":
                query = getBeneficiaryDataQuery(checkLevel, isAllAccess, false);
                break;
            case "AD":
                query = getAddressDataQuery(checkLevel, isAllAccess, false);
                break;

            case "GP":
                query = getGatePassDataQuery(checkLevel, isAllAccess, false);
                break;
            //case "SW":
            //    query = getStraightWorkDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            default:
                break;
        }
        query = query.Replace("@UserLogged", "'" + userID + "'")
                    .Replace("{0}", userID)
                    .Replace("@filterCostCenter", "'" + ddlCostcenter.ToString() + "'");
        return query;
    }
    public string getControlNoQueryBasedOnTransactionType(string TransactionType, string ddlCostcenter, string checkLevel, bool isAllAccess, string userID, string additionalCondition)
    {
        string query = string.Empty;
        switch (TransactionType)
        {
            case "OT":
                query = getOvertimeQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "LV":
                query = getLeaveDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "TR":
                query = getTimeModificationDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            //case "FT":
            //    query = getFlexTimeDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            //case "JS":
            //    query = getJobModificationDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            case "MV":
                query = getMovementDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "TX":
                query = getTaxCivilDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "BF":
                query = getBeneficiaryDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "AD":
                query = getAddressDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            case "GP":
                query = getGatePassDataQueryDetails(checkLevel, isAllAccess, false, additionalCondition);
                break;
            //case "SW":
            //    query = getStraightWorkDataQuery(checkLevel, isAllAccess, isCurrentQuincena);
            //    break;
            default:
                break;
        }
        query = query.Replace("@UserLogged", "'" + userID + "'")
                    .Replace("{0}", userID)
                    .Replace("@filterCostCenter", "'" + ddlCostcenter.ToString() + "'");
        return query;
    }
}
//public class StringValueAttribute : Attribute
//{
//    public StringValueAttribute(string value);
//    public StringValueAttribute(string value, string display);

//    public string Display { get; }
//    public string Value { get; }
//}