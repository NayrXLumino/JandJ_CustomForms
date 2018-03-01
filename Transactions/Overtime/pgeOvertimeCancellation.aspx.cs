using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Payroll.DAL;
using CommonLibrary;

public partial class Transactions_Overtime_pgeOvertimeCancellation : System.Web.UI.Page
{

    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    LeaveBL LVBL = new LeaveBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {

            overtimeDatasoucre.ConnectionString = Encrypt.decryptText(Session["dbConn"].ToString());
            if (!Page.IsPostBack)
            {//WFOTCANCEL WFOTENTRY
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFOTCANCEL"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {

                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                    Page.PreRender += new EventHandler(Page_PreRender);

                    setQuery();
                    bindGrid();
                }
            }
            else
            {
                overtimeDatasoucre.SelectCommand = query.Value.ToString();
            }
            LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeIndividual_LoadComplete);
        }
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    #region Events
    void Transactions_Overtime_pgeOvertimeIndividual_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "overtimeScripts";
        string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            //string errorMessage = string.Empty;
            if (hfEmployeeId.Value.ToString().Trim() != "")//Page.IsValid)
            {
                if (!methods.GetProcessControlFlag("PAYROLL", "CYCCUT-OFF"))
                {
                    // hfReason.Value.ToString() + "\r\nDeleted: " +
                    string reasondeletion = txtRemarks.Text.ToString();


                    ParameterInfo[] paramInfo = new ParameterInfo[4];

                    paramInfo[0] = new ParameterInfo("@Reason", txtRemarks.Text.ToString());
                    paramInfo[1] = new ParameterInfo("@ControlNo", hfEmployeeId.Value.ToString().Trim());
                    paramInfo[2] = new ParameterInfo("@Remarks", txtRemarks.Text.ToString());
                    paramInfo[3] = new ParameterInfo("@login", Session["userLogged"].ToString());
                    #region QUERY
                    string queryUpdate = string.Format(@"
	                    IF exists (select * from T_TransactionRemarks where Trm_ControlNo = @ControlNo)
	BEGIN
		UPDATE T_TransactionRemarks
		SET Trm_Remarks =  @Reason
			,Usr_Login=    @login
			,Ludatetime=   GETDATE()
		WHERE Trm_ControlNo= @ControlNo
	END
else
	BEGIN
		INSERT INTO T_TransactionRemarks
			( Trm_ControlNo
			, Trm_Remarks
			, Usr_Login
			, ludatetime)
		VALUES
			(@ControlNo
			, @Reason
			,@login
			, getdate())
		
	END

UPDATE T_EmployeeOvertime
SET Eot_Status='0'
    , Eot_ApprovedBy = @Login
    , Eot_ApprovedDate = GETDATE()
    ,Ludatetime=getdate()
    , Usr_Login = @Login
WHERE Eot_ControlNo=@ControlNo

UPDATE T_EmployeeOvertimeHist
SET Eot_Status='0'
    , Eot_ApprovedBy = @Login
    , Eot_ApprovedDate = GETDATE()
    ,Ludatetime=getdate()
    , Usr_Login = @Login
WHERE Eot_ControlNo=@ControlNo
	            ");
                    #endregion


                    if (txtRemarks.Text.ToString().Trim() != "")
                    {
                        if (true)//!CommonMethods.isAffectedByCutoff("OVERTIME", hfOvertimeDate.ToString()))
                        {
                            using (DALHelper dh = new DALHelper())
                            {
                                try
                                {
                                    dh.OpenDB();
                                    dh.BeginTransaction();
                                    dh.ExecuteNonQuery(queryUpdate, CommandType.Text, paramInfo);
                                    OvertimeBL obl = new OvertimeBL();
                                    obl.EmployeeLogLedgerUpdate(hfEmployeeId.Value.ToString().Trim(), Session["userLogged"].ToString(), dh);
                                    MessageBox.Show("Approved overtime succesfully cancelled.");
                                    //MenuLog
                                    SystemMenuLogBL.InsertEditLog("WFOTCANCEL", true, hfEmployeeId2.Value.ToString().Trim(), Session["userLogged"].ToString(), "");
                                    dh.CommitTransaction();
                                }
                                catch (Exception ex)
                                {
                                    dh.RollBackTransaction();
                                    MessageBox.Show(ex.ToString());
                                }
                                finally
                                {
                                    dh.CloseDB();
                                    btnCancel.Enabled = false;
                                }
                            }

                        }
                        overtimeDatasoucre.SelectCommand = query.Value.ToString();
                        dgvResult.DataBind();
                        txtControlNo.Text = "";
                        txtRemarks.Text = "";
                        //dgvResult.SelectedIndex = -1;
                        //dgvResult.FocusedRowIndex = -1;
                    }
                    else
                        MessageBox.Show("Please provide a reason for cancellation of transaction.");
                }
                else
                {
                    MessageBox.Show(CommonMethods.GetErrorMessageForCYCCUTOFF());
                }
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
            }
            else
                MessageBox.Show("Select a transaction to be cancelled. ");

        }
    }
    #endregion

    #region Methods

    private void setQuery()
    {
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        overtimeDatasoucre.SelectCommand = query.Value = string.Format(@"DECLARE @USERLOGGED AS VARCHAR(20)
DECLARE @SYSTEMID AS VARCHAR(20)
DECLARE @CANEDIT AS BIT
DECLARE @COSTCENTER as table( cost_center VARCHAR(10))
DECLARE @CONDITIONCOSTCENTER BIT
DECLARE @DATESTART DATETIME
DECLARE @DATEEND DATETIME
	SET @USERLOGGED = '{0}'
	SET @SYSTEMID = 'OVERTIME'
	SET @CANEDIT = 1
	SET @DATESTART=(SELECT ISNULL(MIN(Dates),(SELECT Ppm_StartCycle 
                                                FROM T_PayPeriodMaster 
                                               WHERE Ppm_CycleIndicator = 'C'))
                                                FROM (  SELECT Ppm_StartCycle [Dates]
														FROM (
															SELECT ROW_NUMBER() OVER(ORDER BY Ppm_PayPeriod DESC) AS PeriodCount
																	, Ppm_StartCycle
															FROM T_PayPeriodMaster
															WHERE Ppm_CycleIndicator IN ('P')
														) TEMP2
														WHERE PeriodCount <= (SELECT Pmt_NumericValue FROM T_ParameterMaster WHERE Pmt_ParameterID = 'MINPASTPRD')) AS TEMP)
	SET @DATEEND = GETDATE()
--(select Ppm_EndCycle 
--					from T_PayPeriodMaster
--					where Ppm_CycleIndicator = 'C')
	
	SET @CONDITIONCOSTCENTER = 0
	
	INSERT INTO @COSTCENTER
	SELECT Uca_CostCenterCode 
	  FROM T_UserCostCenterAccess
	 WHERE Uca_Usercode = @USERLOGGED
	   AND Uca_SytemID = @SYSTEMID
						
	
	IF( ( SELECT COUNT(*) 
	        FROM @COSTCENTER)!= 0)		
		   BEGIN
				IF ( ( SELECT COUNT(*) 
				         FROM @COSTCENTER ) = 1 
				 AND ( SELECT TOP 1 *  
				         FROM @COSTCENTER) = 'ALL')
				BEGIN
					SELECT DISTINCT Eot_ControlNo [Control Number]
						 , Emt_EmployeeID [Employee ID]
						 , Emt_FirstName+' '+Emt_LastName[Employee Name]
						 , CONVERT(varchar(10),Eot_OvertimeDate,101) [Overtime Date]
						 , CONVERT(varchar(10),Eot_AppliedDate,101) [Applied Date]
						 , CASE WHEN Eot_OvertimeType = 'P'
							    THEN 'Post'
							    ELSE 'Advance'
							END [Overtime Type]
                         , Eot_BatchNo [Batch No]
						 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
						 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
						 , Eot_OvertimeHour [Hours]
						 , Eot_Reason [Reason]
						 , CONVERT(varchar(10),Eot_ApprovedDate,101) [Date Approved]
					  FROM T_EmployeeOvertime
					  LEFT JOIN T_EmployeeMaster
					    ON Emt_EmployeeID=Eot_EmployeeId
                       {2}
					 WHERE Eot_CostCenter IN (SELECT Cct_CostCenterCode 
											    FROM T_CostCenter
											   WHERE Cct_status = 'A')
					   AND Eot_Status = '9'
					   AND Emt_JobStatus <> 'IN'
					   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
                       {1}
UNION
					SELECT DISTINCT Eot_ControlNo [Control Number]
						 , Emt_EmployeeID [Employee ID]
						 , Emt_FirstName+' '+Emt_LastName[Employee Name]
						 , CONVERT(varchar(10),Eot_OvertimeDate,101) [Overtime Date]
						 , CONVERT(varchar(10),Eot_AppliedDate,101) [Applied Date]
						 , CASE WHEN Eot_OvertimeType = 'P'
							    THEN 'Post'
							    ELSE 'Advance'
							END [Overtime Type]
                         , Eot_BatchNo [Batch No]
						 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
						 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
						 , Eot_OvertimeHour [Hours]
						 , Eot_Reason [Reason]
						 , CONVERT(varchar(10),Eot_ApprovedDate,101) [Date Approved]
					  FROM T_EmployeeOvertimeHist
					  LEFT JOIN T_EmployeeMaster
					    ON Emt_EmployeeID=Eot_EmployeeId
                       {2}
					 WHERE Eot_CostCenter IN (SELECT Cct_CostCenterCode 
											    FROM T_CostCenter
											   WHERE Cct_status = 'A')
					   AND Eot_Status = '9'
					   AND Emt_JobStatus <> 'IN'
					   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
                       {1}
						
				END
				ELSE
				BEGIN
					SELECT Distinct Eot_ControlNo [Control Number]
						 , Emt_EmployeeID [Employee ID]
						 , Emt_FirstName + ' ' + Emt_LastName [Employee Name]
						 , CONVERT(varchar(10), Eot_OvertimeDate, 101) [Overtime Date]
						 , CONVERT(varchar(10), Eot_AppliedDate, 101) [Applied Date]
						 , CASE WHEN Eot_OvertimeType='P'
					 			THEN 'Post'
								ELSE 'Advance'
							END [Overtime Type]
                         , Eot_BatchNo [Batch No]
						 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
						 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
						 , Eot_OvertimeHour [Hours]
						 , Eot_Reason [Reason]
						 , CONVERT(varchar(10), Eot_ApprovedDate, 101) [Date Approved]
					  FROM T_EmployeeOvertime
				      LEFT JOIN T_EmployeeMaster
						ON Emt_EmployeeID=Eot_EmployeeId
                       {2}
					 WHERE Eot_CostCenter IN ( SELECT Uca_CostCenterCode 
											     FROM T_UserCostCenterAccess 
									            WHERE Uca_Usercode = @USERLOGGED
                                                  AND Uca_SytemID = @SYSTEMID
										          AND Uca_Status = 'A')
					   AND Eot_Status = '9'
					   AND Emt_JobStatus <> 'IN'
					   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
                       {1}
UNION
					SELECT Distinct Eot_ControlNo [Control Number]
						 , Emt_EmployeeID [Employee ID]
						 , Emt_FirstName + ' ' + Emt_LastName [Employee Name]
						 , CONVERT(varchar(10), Eot_OvertimeDate, 101) [Overtime Date]
						 , CONVERT(varchar(10), Eot_AppliedDate, 101) [Applied Date]
						 , CASE WHEN Eot_OvertimeType='P'
					 			THEN 'Post'
								ELSE 'Advance'
							END [Overtime Type]
                         , Eot_BatchNo [Batch No]
						 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
						 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
						 , Eot_OvertimeHour [Hours]
						 , Eot_Reason [Reason]
						 , CONVERT(varchar(10), Eot_ApprovedDate, 101) [Date Approved]
					  FROM T_EmployeeOvertimeHist
				      LEFT JOIN T_EmployeeMaster
						ON Emt_EmployeeID=Eot_EmployeeId
                       {2}
					 WHERE Eot_CostCenter IN ( SELECT Uca_CostCenterCode 
											     FROM T_UserCostCenterAccess 
									            WHERE Uca_Usercode = @USERLOGGED
                                                  AND Uca_SytemID = @SYSTEMID
										          AND Uca_Status = 'A')
					   AND Eot_Status = '9'
					   AND Emt_JobStatus <> 'IN'
					   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
                       {1}
				END
		END
	ELSE
		BEGIN
			SELECT Distinct Eot_ControlNo[Control Number]
				 , Emt_EmployeeID[Employee ID]
				 , Emt_FirstName+' '+Emt_LastName[Employee Name]
				 , CONVERT(varchar(10),Eot_OvertimeDate,101)[Overtime Date]
				 , CONVERT(varchar(10),Eot_AppliedDate,101)[Applied Date]
				 , CASE WHEN Eot_OvertimeType = 'P'
			 		    THEN 'Post'
					    ELSE 'Advance'
				    END [Overtime Type]
                 , Eot_BatchNo [Batch No]
				 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
				 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
				 , Eot_OvertimeHour [Hours]
				 , Eot_Reason [Reason]
				 , CONVERT(varchar(10), Eot_ApprovedDate, 101) [Date Approved]
			  FROM T_EmployeeOvertime
			  LEFT JOIN T_EmployeeMaster
				ON Emt_EmployeeID=Eot_EmployeeId
               {2}
			 WHERE Eot_EmployeeId = @USERLOGGED
			   AND Eot_Status =9--iya nga account
			   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
               {1}
UNION
			SELECT Distinct Eot_ControlNo[Control Number]
				 , Emt_EmployeeID[Employee ID]
				 , Emt_FirstName+' '+Emt_LastName[Employee Name]
				 , CONVERT(varchar(10),Eot_OvertimeDate,101)[Overtime Date]
				 , CONVERT(varchar(10),Eot_AppliedDate,101)[Applied Date]
				 , CASE WHEN Eot_OvertimeType = 'P'
			 		    THEN 'Post'
					    ELSE 'Advance'
				    END [Overtime Type]
                 , Eot_BatchNo [Batch No]
				 , LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) [Start Time]
				 , LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) [End Time]
				 , Eot_OvertimeHour [Hours]
				 , Eot_Reason [Reason]
				 , CONVERT(varchar(10), Eot_ApprovedDate, 101) [Date Approved]
			  FROM T_EmployeeOvertimeHist
			  LEFT JOIN T_EmployeeMaster
				ON Emt_EmployeeID=Eot_EmployeeId
               {2}
			 WHERE Eot_EmployeeId = @USERLOGGED
			   AND Eot_Status =9--iya nga account
			   AND Eot_OvertimeDate BETWEEN @DATESTART AND @DATEEND
               {1}

		END ", Session["userLogged"].ToString(), searchfilter(), !hasCCLine ? "" : @"---apsungahid added for line code access filter 20141121                                                                    
                                                                                  LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									                                                             FROM E_CostCenterLineMaster 
									                                                            WHERE Clm_Status = 'A' ) AS HASLINE
						                                                            ON Clm_CostCenterCode = Eot_Costcenter ");
    }
    #endregion
    protected void dgvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (this.dgvResult.FocusedRowIndex >= 0)
        //{

        //    DataRow dr = dgvResult.GetDataRow(this.dgvResult.FocusedRowIndex);
        //    txtControlNo.Text = dr["Control Number"].ToString();
        //    //hfReason.Value = row.Cells[10].Text;
        //    btnCancel.Enabled = true;
        //}
        if (dgvResult.SelectedRow.RowIndex != -1)
        {
            putControlNumber(dgvResult.SelectedRow);
            GridViewRow row = dgvResult.Rows[dgvResult.SelectedRow.RowIndex];
            txtControlNo.Text = row.Cells[1].Text;
            hfReason.Value = row.Cells[10].Text;
            btnCancel.Enabled = true;
        }
    }
    private void putControlNumber(GridViewRow gvr)
    {
        //txtControlNo.Text = "Eot_EmployeeId"; //gvr[""].ToString();

    }
    protected void dgvResult_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }
    protected void dgvResult_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void searchBtn_Click(object sender, EventArgs e)
    {

    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        setQuery();
        bindGrid();
    }
    private string searchfilter()
    {
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");
        string condition = "";
        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            condition = string.Format(@"AND (    ( Eot_ControlNo LIKE '{0}%' )
                                          OR ( Emt_EmployeeID LIKE '{0}%' )
                                          OR ( LTRIM(Emt_FirstName) LIKE '{0}%' )
                                          OR ( LTRIM(Emt_LastName) LIKE '{0}%' )
                                          OR ( LTRIM(Emt_FirstName+' '+Emt_LastName) LIKE '{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_OvertimeDate,101) LIKE '{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_AppliedDate,101) LIKE '{0}%' )
                                          OR ( CASE WHEN Eot_OvertimeType = 'P'
							                        THEN 'Post'
							                        ELSE 'Advance'
							                    END LIKE '{0}%' )
                                          OR ( Eot_BatchNo LIKE '{0}%' )
                                          OR ( LEFT(RTRIM(LTRIM(Eot_StartTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_StartTime)),2) LIKE '{0}%' )
                                          OR ( LEFT(RTRIM(LTRIM(Eot_EndTime)),2) + ':' + RIGHT(RTRIM(LTRIM(Eot_EndTime)),2) LIKE '{0}%' )
                                          OR ( Eot_OvertimeHour LIKE '{0}%' )
                                          OR ( Eot_Reason LIKE '{0}%' )
                                          OR ( CONVERT(varchar(10),Eot_ApprovedDate,101) LIKE '{0}%' )
                                          
                                    )", txtSearch.Text.ToString());

        }

        if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
        {
            condition += string.Format(@" AND  (  ( Eot_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = 'OVERTIME')
                                                                        OR Eot_EmployeeId = '{0}'))", Session["userLogged"].ToString());


        }

        if (hasCCLine)//flag costcenter line to retain old code
        {
            condition += string.Format(@" AND ( ISNULL(Eot_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Eot_CostcenterLine, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE (Ucl_CostCenterCode = Eot_CostCenter OR Ucl_CostCenterCode = 'ALL')
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'OVERTIME'
																															 AND Ucl_LineCode = ISNULL(Eot_CostcenterLine, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Eot_CostCenter
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'OVERTIME'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                          OR 'ALL' IN ( SELECT Ucl_LineCode 
										                  FROM E_UserCostcenterLineAccess 
													     WHERE Ucl_CostCenterCode = 'ALL'
														   AND Ucl_Status = 'A'
														   AND Ucl_SystemID = 'OVERTIME'
														   AND Ucl_LineCode = 'ALL'
														   AND Ucl_UserCode = '{0}')

                                          OR Eot_EmployeeID = '{0}') ", Session["userLogged"].ToString());
        }

        return condition;
    }
    //protected void dgvResult_FocusedRowChanged(object sender, EventArgs e)
    //{
    //    if (this.dgvResult.FocusedRowIndex >= 0)
    //    {

    //        DataRow dr = dgvResult.GetDataRow(this.dgvResult.FocusedRowIndex);
    //        txtControlNo.Text = dr["Control Number"].ToString();
    //        //hfReason.Value = row.Cells[10].Text;
    //        btnCancel.Enabled = true;
    //    }
    //}
    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }
    private void bindGrid()
    {
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                //string str = SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString());
                ds = dal.ExecuteDataSet(overtimeDatasoucre.SelectCommand.ToString(), CommandType.Text);
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
        //foreach (DataRow dr in ds.Tables[0].Rows)
        //    hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        //dgvResult.DataSource = ds;
        //dgvResult.DataBind();
    }
    protected void dgvResult_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValueToControlNo('" + e.Row.RowIndex + "')");
        }
    }
}