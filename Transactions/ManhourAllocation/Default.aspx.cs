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


public partial class Transactions_Manhour_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {
            showControlsOnRights();
            fillChartData();
        }
    }

    #region Methods
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
                    case "WFJOBSPLTENTRY":
                        btnAllocationEntry.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFJOBSPLTMOD":
                        btnAllocationModification.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFJOBSPLTREP":
                        btnAllocationReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFMANHOURREP":
                        btnManhourReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFJSENROUTE":
                        btnRoutedReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
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

    private void fillChartData()
    {
        DataSet ds = new DataSet();
        string sqlGetLVHours = string.Format(@"
                                    declare @MAXDATE datetime
                                        SET @MAXDATE  = (SELECT MAX(Ell_ProcessDate) FROM T_EMployeeLogLEdger)
                                        SET @MAXDATE  = (CASE WHEN (GETDATE() > @MAXDATE) THEN @MAXDATE ELSE GETDATE() END)
           
                                     SELECT Ell_ProcessDate [Date]
                                          , isnull(sum(Jsd_PlanHours), 0.00) [Hours]
                                       FROM T_EmployeeLogLedger
                                       Left join T_JobSplitHeader on Jsh_EmployeeId = Ell_EmployeeId and Jsh_JobSplitDate = Ell_ProcessDate 
			                                and Jsh_Status = '9'
                                       Left join T_JobSplitDetail on Jsh_ControlNo = Jsd_ControlNo
                                      WHERE Ell_EmployeeId = '{0}' 
                                        AND Ell_ProcessDate BETWEEN DATEADD(day,-15, @MAXDATE) AND @MAXDATE
                                       group by Ell_ProcessDate 
           
                                      UNION 
      
                                      SELECT Ell_ProcessDate [Date]
                                          , isnull(sum(Jsd_PlanHours), 0.00) [Hours]
                                       FROM T_EmployeeLogLedgerHist
                                       Left join T_JobSplitHeader on Jsh_EmployeeId = Ell_EmployeeId and Jsh_JobSplitDate = Ell_ProcessDate 
			                                and Jsh_Status = '9'
                                       Left join T_JobSplitDetail on Jsh_ControlNo = Jsd_ControlNo
                                      WHERE Ell_EmployeeId = '{0}'
                                        AND Ell_ProcessDate BETWEEN DATEADD(day,-15, @MAXDATE) AND @MAXDATE
                                       group by Ell_ProcessDate 
                                      ORDER BY 1 ASC", Session["userLogged"].ToString());

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlGetLVHours, CommandType.Text);
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
            for (int point = 0; point < ds.Tables[0].Rows.Count; point++)
            {
                Chart1.Series["Series1"].Points.AddXY(Convert.ToDateTime(ds.Tables[0].Rows[point]["Date"].ToString())
                                                     , Convert.ToDecimal(ds.Tables[0].Rows[point]["Hours"].ToString()));
            }
            // Set series chart type
            Chart1.Series["Series1"].ChartType = SeriesChartType.Line;

            // Set point labels
            Chart1.Series["Series1"].IsValueShownAsLabel = true;

            // Enable X axis margin
            Chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;
            Chart1.ChartAreas["ChartArea1"].AxisX.TextOrientation = TextOrientation.Auto;
            Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;


            // Enable 3D, and show data point marker lines
            Chart1.Series["Series1"]["ShowMarkerLines"] = "True";

        }
    }
    #endregion
}
