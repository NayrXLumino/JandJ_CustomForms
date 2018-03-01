using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class pgeAfterLog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("index.aspx?pr=dc");
        }
        if (!Page.IsPostBack)
        {
            this.showChecklist();
            this.populateChecklistCounters();
            this.populateWaitlistCounters();
            this.populateNextLevelCounters();
            this.populateTransactionCounters();
            LoadComplete += new EventHandler(pgeAfterLog_LoadComplete);
        }
        Session["transaction"] = string.Empty;
    }

    void pgeAfterLog_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "afterLogScripts";
        string jsurl = "Javascript/_afterLog.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
    }

    #region Populate and Show checklist/waitlist/next level counters
    private void populateChecklistCounters()
    {
        #region SQL Counters
        string sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Eot_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Eot_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Eot_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Elt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Elt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Elt_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Trm_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Trm_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Trm_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Flx_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Flx_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Flx_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 LEFT JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (  ( Jsh_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                      OR ( Jsh_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                      OR ( Jsh_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                      )
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Mve_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Mve_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Mve_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Pit_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Pit_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Pit_Status = '7' AND routeMaster.Arm_Approver = '{0}')

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( But_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( But_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( But_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Amt_Status = '3' AND routeMaster.Arm_Checker1 = '{0}') 
                                   OR ( Amt_Status = '5' AND routeMaster.Arm_Checker2 = '{0}') 
                                   OR ( Amt_Status = '7' AND routeMaster.Arm_Approver = '{0}')
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString()), CommandType.Text);
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
            try { lblCOT.Text = ds.Tables[0].Rows[0][0].ToString();}
            catch { lblCOT.Text = "0"; }

            try { lblCLV.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { lblCLV.Text = "0"; }

            try { lblCTR.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { lblCTR.Text = "0"; }

            try { lblCFT.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { lblCFT.Text = "0"; }

            try { lblCJS.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { lblCJS.Text = "0"; }

            try { lblCMV.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { lblCMV.Text = "0"; }

            try { lblCTX.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { lblCTX.Text = "0"; }

            try { lblCBF.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { lblCBF.Text = "0"; }

            try { lblCAD.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { lblCAD.Text = "0"; }
        }
        else
        {
            MessageBox.Show("Could not retrieve data.");
        }
    }
    private void populateWaitlistCounters()
    {
        #region SQL Counters
        string sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Eot_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Eot_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Eot_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Elt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Elt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Elt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Trm_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Trm_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Trm_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Flx_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Flx_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Flx_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 LEFT JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ((Jsh_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR  (Jsh_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR  (Jsh_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' ))
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Mve_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Mve_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Mve_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Pit_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Pit_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Pit_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (But_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (But_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (But_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (Amt_Status IN ('1') AND routemaster.Arm_Checker1 = '{0}' ) 
                                   OR (Amt_Status IN ('1', '3') AND routemaster.Arm_Checker2 = '{0}' )
                                   OR (Amt_Status IN ('1', '3', '5') AND routemaster.Arm_Approver =  '{0}' )
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString()), CommandType.Text);
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
            try { lblWOT.Text = ds.Tables[0].Rows[0][0].ToString(); }
            catch { lblWOT.Text = "0"; }

            try { lblWLV.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { lblWLV.Text = "0"; }

            try { lblWTR.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { lblWTR.Text = "0"; }

            try { lblWFT.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { lblWFT.Text = "0"; }

            try { lblWJS.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { lblWJS.Text = "0"; }

            try { lblWMV.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { lblWMV.Text = "0"; }

            try { lblWTX.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { lblWTX.Text = "0"; }

            try { lblWBF.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { lblWBF.Text = "0"; }

            try { lblWAD.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { lblWAD.Text = "0"; }
        }
        else
        {
            MessageBox.Show("Could not retrieve data.");
        }
    }
    private void populateNextLevelCounters()
    {
        #region SQL Counters
        string sqlCounters = @"--Table 1: Overtime
                               SELECT ISNULL(COUNT(Eot_Status),0)
                                 FROM T_EmployeeOvertime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Eot_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Eot_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'OVERTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Eot_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Eot_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 2: LEAVE
                               SELECT ISNULL(COUNT(Elt_Status),0)
                                 FROM T_EmployeeLeaveAvailment
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Elt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId = Elt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'LEAVE'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Elt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2) 
                                   OR ( Elt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Trm_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Trm_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TIMEMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Trm_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Trm_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Flx_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Flx_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'FLEXTIME'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Flx_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Flx_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                 LEFT JOIN T_EmployeeMaster
                                   ON Emt_EmployeeId = Jsh_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Jsh_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'JOBMOD'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE (( Jsh_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR  ( Jsh_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver ))
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Mve_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Mve_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'MOVEMENT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Mve_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Mve_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Pit_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Pit_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'TAXMVMNT'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Pit_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Pit_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )

                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = But_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =But_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'BNEFICIARY'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( But_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( But_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )
                                   
                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                 LEFT JOIN T_EmployeeMaster 
                                   ON Emt_EmployeeId = Amt_EmployeeId
                                INNER JOIN T_EmployeeApprovalRoute AS empApprovalRoute 
                                   ON empApprovalRoute.Arm_EmployeeId =Amt_EmployeeId
                                  AND empApprovalRoute.Arm_TransactionID = 'ADDRESS'
                                 LEFT JOIN T_ApprovalRouteMaster AS routeMaster 
                                   ON routeMaster.Arm_RouteId = empApprovalRoute.Arm_RouteId
                                WHERE ( Amt_Status IN ('5','7')  AND routemaster.Arm_Checker1 = '{0}' AND routemaster.Arm_Checker1 <> routemaster.Arm_Checker2 ) 
                                   OR ( Amt_Status IN ('7') AND routemaster.Arm_Checker2 =  '{0}' AND routemaster.Arm_Checker2 <> routemaster.Arm_Approver )
                                ";
        #endregion
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString()), CommandType.Text);
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
        if (ds.Tables.Count > 0)
        {
            try { lblNOT.Text = ds.Tables[0].Rows[0][0].ToString(); }
            catch { lblNOT.Text = "0"; }

            try { lblNLV.Text = ds.Tables[1].Rows[0][0].ToString(); }
            catch { lblNLV.Text = "0"; }

            try { lblNTR.Text = ds.Tables[2].Rows[0][0].ToString(); }
            catch { lblNTR.Text = "0"; }

            try { lblNFT.Text = ds.Tables[3].Rows[0][0].ToString(); }
            catch { lblNFT.Text = "0"; }

            try { lblNJS.Text = ds.Tables[4].Rows[0][0].ToString(); }
            catch { lblNJS.Text = "0"; }

            try { lblNMV.Text = ds.Tables[5].Rows[0][0].ToString(); }
            catch { lblNMV.Text = "0"; }

            try { lblNTX.Text = ds.Tables[6].Rows[0][0].ToString(); }
            catch { lblNTX.Text = "0"; }

            try { lblNBF.Text = ds.Tables[7].Rows[0][0].ToString(); }
            catch { lblNBF.Text = "0"; }

            try { lblNAD.Text = ds.Tables[8].Rows[0][0].ToString(); }
            catch { lblNAD.Text = "0"; }
        }
        else
        {
            MessageBox.Show("Could not retrieve data.");
        }
    }
    private void showChecklist()
    {
        OT.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFOTENTRY");
        LV.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFLVEENTRY");
        TR.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFTIMERECENTRY");
        FT.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFFLXENTRY");
        JS.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFJOBSPLTMOD");
        MV.Visible = (  CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFSHIFTUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFGROUPUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFCCUPDATE")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFRESTDAY"));
        BF.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFBENEUPDATE");
        TX.Visible = CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFTAXCODE");
        AD.Visible = (  CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDPRES")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDPERMA")
                     || CommonMethods.getCheckRights(Session["userLogged"].ToString(), "WFADDEMER"));
        pnlChecklist.Visible = (OT.Visible || LV.Visible || TR.Visible || FT.Visible || JS.Visible || MV.Visible || TX.Visible || BF.Visible || AD.Visible);
    }         
    #endregion

    #region Populate and Show transaction counters
    private void populateTransactionCounters()
    {
        #region SQL Counters
        string sqlCounters = @"--Table 1: Overtime
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertime
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1})
                                         UNION
                                        SELECT ISNULL(COUNT(Eot_Status),0) [Count]
                                          FROM T_EmployeeOvertimeHist
                                         WHERE Eot_EmployeeId = '{0}'
                                           AND Eot_Status IN ({1}) ) AS TEMP

                               --Table 2: LEAVE
                               SELECT SUM([Count])
                                 FROM ( SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailment
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}'
                                         UNION 
                                        SELECT ISNULL(COUNT(Elt_Status),0) [Count]
                                          FROM T_EmployeeLeaveAvailmentHist
                                         WHERE Elt_Status IN ({1})
                                           AND Elt_EmployeeId = '{0}' ) AS TEMP

                               --Table 3: TIME MODIFICATION
                               SELECT ISNULL(COUNT(Trm_Status),0)
                                 FROM T_TimeRecMod
                                WHERE Trm_EmployeeId = '{0}'
                                  AND Trm_Status IN ({1})

                              --Table 4: FLEXTIME
                               SELECT ISNULL(COUNT(Flx_Status),0)
                                 FROM T_FlexTime
                                WHERE Flx_EmployeeId = '{0}'
                                  AND Flx_Status IN ({1})

                               --Table 5: JOBSPLIT
                               DECLARE @prefix AS varchar(1)
                               SET @prefix = (SELECT Tcm_TransactionPrefix 
                                                FROM T_TransactionControlMaster
                                               WHERE Tcm_TransactionCode = 'JOBMOD')
                               SELECT ISNULL(COUNT(Jsh_Status),0)
                                 FROM T_JobSplitHeader
                                WHERE Jsh_EmployeeId = '{0}'
                                  AND Jsh_Status IN ({1})
                                  AND LEFT(Jsh_ControlNo,1) = @prefix

                               --Table 6: MOVEMENT
                               SELECT ISNULL(COUNT(Mve_Status),0)
                                 FROM T_Movement
                                WHERE Mve_EmployeeId = '{0}'
                                  AND Mve_Status IN ({1})
 
                               --Table 7: TAX/CIVIL
                               SELECT ISNULL(COUNT(Pit_Status),0)
                                 FROM T_PersonnelInfoMovement
                                WHERE Pit_EmployeeId = '{0}'
                                  AND Pit_MoveType = 'P1'
                                  AND Pit_Status IN ({1}) 
 
                               --Table 8: BENEFICIARY
                               SELECT ISNULL(COUNT(But_Status),0)
                                 FROM T_BeneficiaryUpdate
                                WHERE But_EmployeeId = '{0}'
                                  AND But_Status IN ({1}) 

                               --Table 9: ADDRESS
                               SELECT ISNULL(COUNT(Amt_Status),0)
                                 FROM T_AddressMovement
                                WHERE Amt_EmployeeId = '{0}'
                                  AND Amt_Status IN ({1}) 
                                ";
        #endregion
        DataSet dsNP = new DataSet();
        DataSet dsAD = new DataSet();
        string statusNP = "'1','3','5','7','N'";
        string statusAD = "'2','4','6','8','9','A','C'";
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsNP = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString(), statusNP), CommandType.Text);
                dsAD = dal.ExecuteDataSet(string.Format(sqlCounters, Session["userLogged"].ToString(), statusAD), CommandType.Text);
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
        //New and Pending
        try { btnNPOT.Text = dsNP.Tables[0].Rows[0][0].ToString(); }
        catch { btnNPOT.Text = "0"; }

        try { btnNPLV.Text = dsNP.Tables[1].Rows[0][0].ToString(); }
        catch { btnNPLV.Text = "0"; }

        try { btnNPTR.Text = dsNP.Tables[2].Rows[0][0].ToString(); }
        catch { btnNPTR.Text = "0"; }

        try { btnNPFT.Text = dsNP.Tables[3].Rows[0][0].ToString(); }
        catch { btnNPFT.Text = "0"; }

        try { btnNPJS.Text = dsNP.Tables[4].Rows[0][0].ToString(); }
        catch { btnNPJS.Text = "0"; }

        try { btnNPMV.Text = dsNP.Tables[5].Rows[0][0].ToString(); }
        catch { btnNPMV.Text = "0"; }

        try { btnNPTX.Text = dsNP.Tables[6].Rows[0][0].ToString(); }
        catch { btnNPTX.Text = "0"; }

        try { btnNPBF.Text = dsNP.Tables[7].Rows[0][0].ToString(); }
        catch { btnNPBF.Text = "0"; }

        try { btnNPAD.Text = dsNP.Tables[8].Rows[0][0].ToString(); }
        catch { btnNPAD.Text = "0"; }

        //Approved and Disapproved
        try { btnADOT.Text = dsAD.Tables[0].Rows[0][0].ToString(); }
        catch { btnADOT.Text = "0"; }

        try { btnADLV.Text = dsAD.Tables[1].Rows[0][0].ToString(); }
        catch { btnADLV.Text = "0"; }

        try { btnADTR.Text = dsAD.Tables[2].Rows[0][0].ToString(); }
        catch { btnADTR.Text = "0"; }

        try { btnADFT.Text = dsAD.Tables[3].Rows[0][0].ToString(); }
        catch { btnADFT.Text = "0"; }

        try { btnADJS.Text = dsAD.Tables[4].Rows[0][0].ToString(); }
        catch { btnADJS.Text = "0"; }

        try { btnADMV.Text = dsAD.Tables[5].Rows[0][0].ToString(); }
        catch { btnADMV.Text = "0"; }

        try { btnADTX.Text = dsAD.Tables[6].Rows[0][0].ToString(); }
        catch { btnADTX.Text = "0"; }

        try { btnADBF.Text = dsAD.Tables[7].Rows[0][0].ToString(); }
        catch { btnADBF.Text = "0"; }

        try { btnADAD.Text = dsAD.Tables[8].Rows[0][0].ToString(); }
        catch { btnADAD.Text = "0"; }

        ////201100722 Andre revised: based on resources
        ADOT.Visible = NPOT.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME);
        ADLV.Visible = NPLV.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE);
        ADTR.Visible = NPTR.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION);
        ADFT.Visible = NPFT.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWFLEXTIME);
        ADJS.Visible = NPJS.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWJOBSPLIT);
        ADMV.Visible = NPMV.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION);
        ADBF.Visible = NPBF.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWBENEFICIARY);
        ADTX.Visible = NPTX.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTAXCIVIL);
        ADAD.Visible = NPAD.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWADDRESS);
        ////END
    }
    #endregion

    #region Events
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(this.Page.Request.Url.ToString());
    }
    #endregion
}
