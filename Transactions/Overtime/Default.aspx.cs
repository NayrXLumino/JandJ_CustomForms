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

public partial class Transactions_Overtime_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        showControlsOnRights();
        fillChartData();
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
        param[1] = new ParameterInfo("@SystemID", "OVERTIME");
        DataSet ds = new DataSet();
        using(DALHelper dal = new DALHelper())
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
            for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                switch (ds.Tables[0].Rows[i]["Ugt_sysmenucode"].ToString())
                { 
                    case "WFOTENTRY":
                        btnIndividualOT.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBOTENTRY":
                        btnBatchOT.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFSPLOTENTRY":
                        btnSpecialOT.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFOTREP":
                        btnOvertimeReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFCRPASREP":
                        btnCarpassReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBOTUPLD":
                        btnBatchOvertimeUploading.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
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
        string sqlGetOTHours = string.Format(@"
                                declare @MAXDATE datetime
                                    SET @MAXDATE  = (SELECT MAX(Ell_ProcessDate) FROM T_EMployeeLogLEdger)
                                    SET @MAXDATE  = (CASE WHEN (GETDATE() > @MAXDATE) THEN @MAXDATE ELSE GETDATE() END)
                                 SELECT Ell_ProcessDate [Date]
                                      , Ell_EncodedOvertimeAdvHr+Ell_EncodedOvertimePostHr[Hours]
                                   FROM T_EmployeeLogLedger
                                  WHERE Ell_EmployeeId = '{0}'
                                    AND Ell_ProcessDate BETWEEN DATEADD(day,-15, @MAXDATE) AND @MAXDATE
                                  UNION 
                                 SELECT Convert(varchar(10), Ell_ProcessDate, 101) [Date]
                                      , Ell_EncodedOvertimeAdvHr+Ell_EncodedOvertimePostHr[Hours]
                                   FROM T_EmployeeLogLedgerHist
                                  WHERE Ell_EmployeeId = '{0}'
                                    AND Ell_ProcessDate BETWEEN DATEADD(day,-15, @MAXDATE) AND @MAXDATE
                                  ORDER BY 1 ASC", Session["userLogged"].ToString());

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlGetOTHours, CommandType.Text);
            }
            catch(Exception ex)
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
                Chart1.Series["Series1"].Points.AddXY( Convert.ToDateTime(ds.Tables[0].Rows[point]["Date"].ToString())
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
