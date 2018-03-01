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
using System.Web.UI.DataVisualization.Charting;
using System.ComponentModel;
using System.Drawing;
using Payroll.DAL;
using MethodsLibrary;


public partial class Transactions_WorkInformation_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        showControlsOnRights();
        populateDetails();
    }

    private void showControlsOnRights()
    {
        string sql = @" SELECT Ugt_sysmenucode
	                         , Ugt_CanRetrieve
                          FROM T_UserGroupDetail
                         INNER JOIN T_UserGrant
                            ON Ugt_Usergroup = Ugd_usergroupcode
                           AND Ugt_SystemID = Ugd_SystemID
                           AND Ugt_sysmenucode LIKE 'WF%'
                         WHERE Ugd_usercode = @UserCode
                           AND Ugd_SystemID = @SystemID ";
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[1] = new ParameterInfo("@SystemID", "TIMEKEEP");
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                switch (ds.Tables[0].Rows[i]["Ugt_sysmenucode"].ToString())
                {
                    case "WFSHIFTUPDATE":
                        btnShiftIndividual.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBSHIFTUPDATE":
                        btnShiftBatch.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFCHANGEGROUP":
                        btnWorkgroupIndividual.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBCHANGEGROUP":
                        btnWorkgroupBatch.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFRESTDAY":
                        btnRestdayIndividual.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBRESTDAY":
                        btnRestdayBatch.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFCCUPDATE":
                        btnCostcenterIndividual.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBCCUPDATE":
                        btnCostcenterBatch.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFMVEREP":
                        btnWorkInfoReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    default:
                        break;

                }
            }
            lblNoAccess.Visible = false;
        }
        else
        {
            lblNoAccess.Visible = true;
        }
    }

    private void populateDetails()
    {
        string sql = string.Format(@"   SELECT '[' 
	                                         + Emt_Shiftcode
	                                         + '] '
	                                         + Scm_ShiftDesc
	                                         + '   '
	                                         + LEFT(Scm_ShiftTimeIn,2) + ':' + RIGHT(Scm_ShiftTimeIn,2)
	                                         + ' '
	                                         + LEFT(Scm_ShiftBreakStart,2) + ':' + RIGHT(Scm_ShiftBreakStart,2)
	                                         + ' - '
	                                         + LEFT(Scm_ShiftBreakEnd,2) + ':' + RIGHT(Scm_ShiftBreakEnd,2)
	                                         + ' '
	                                         + LEFT(Scm_ShiftTimeOut,2) + ':' + RIGHT(Scm_ShiftTimeOut,2) [Shift]
                                             , ISNULl(Ad1.Adt_AccountDesc, '') [Work Type]
                                             , ISNULl(Ad2.Adt_AccountDesc, '') [Work Group]
                                             , dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
                                             , Convert(varchar(10), Emt_CostCenterDate, 101) [Assigned Date]
                                          FROM T_EmployeeMaster
                                          LEFT JOIN T_ShiftCodeMaster
                                            ON Scm_ShiftCode = Emt_Shiftcode
                                          LEFT JOIN T_AccountDetail AD1
                                            ON AD1.Adt_AccountCode = Emt_WorkType
                                           AND Ad1.Adt_AccountType = 'WORKTYPE'
                                          LEFT JOIN T_AccountDetail AD2
                                            ON AD2.Adt_AccountCode = Emt_WorkGroup
                                           AND Ad1.Adt_AccountType = 'WORKGROUP'
                                         WHERE Emt_EmployeeID = '{0}'", Session["userLogged"].ToString());
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (!CommonMethods.isEmpty(ds))
        {
            txtDefaultShift.Text = ds.Tables[0].Rows[0]["Shift"].ToString();
            txtWorkType.Text = ds.Tables[0].Rows[0]["Work Type"].ToString();
            txtWorkGroup.Text = ds.Tables[0].Rows[0]["Work Group"].ToString();
            txtCostCenter.Text = ds.Tables[0].Rows[0]["Costcenter"].ToString();
            txtAssignedDate.Text = ds.Tables[0].Rows[0]["Assigned Date"].ToString();
        }

    }
}
