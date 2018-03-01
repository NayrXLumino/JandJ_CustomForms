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

public partial class Transactions_TimeModification_Default : System.Web.UI.Page
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
                    case "WFTIMERECENTRY":
                        btnTimeModification.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFTIMERECREP":
                        btnTimeModificationReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFWORKREC":
                        btnTimeRecordEntryReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
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
        #region Query
        string sqlGetTRCount = string.Format(@"declare @MAXDATE datetime
                                                   SET @MAXDATE  = (SELECT MAX(Ell_ProcessDate) FROM T_EMployeeLogLEdger)
                                                   SET @MAXDATE  = (CASE WHEN (GETDATE() > @MAXDATE) THEN @MAXDATE ELSE GETDATE() END)
                                                SELECT [Date]
                                                     , [Year Month]
                                                     , SUM([Count]) [Count]
                                                  FROM (
	                                            SELECT Convert(varchar(4),DATEPART(year,Ell_ProcessDate))
		                                             + Convert(varchar(20),DATEPART(month,Ell_ProcessDate)) [Date]
		                                             , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate) [Year Month] 
		                                             , SUM(CASE WHEN Trm_ControlNo IS NULL 
					                                            THEN 0
					                                            ELSE 1
				                                            END ) [Count]
	                                              FROM T_EmployeeLogLEdger
	                                              LEFT JOIN T_TimeRecMod
		                                            ON Trm_ModDate = Ell_ProcessDate
	                                               AND Trm_EmployeeId = Ell_EmployeeId
	                                               AND Trm_Status IN ('1','3','5','7','9','A')
	                                             WHERE Ell_EmployeeId = '{0}'
	                                               AND Ell_ProcessDate BETWEEN DATEADD(month,-15, @MAXDATE) AND @MAXDATE
	                                             GROUP BY Convert(varchar(4),DATEPART(year,Ell_ProcessDate)) 
		                                             + Convert(varchar(20),DATEPART(month,Ell_ProcessDate)) 
		                                             , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate)
	                                             UNION
	                                            SELECT Convert(varchar(4),DATEPART(year,Ell_ProcessDate))
		                                             + Convert(varchar(20),DATEPART(month,Ell_ProcessDate)) [Date]
		                                             , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate) [Year Month] 
		                                             , SUM(CASE WHEN Trm_ControlNo IS NULL 
					                                            THEN 0
					                                            ELSE 1
				                                            END ) [Count]
	                                              FROM T_EmployeeLogLedgerHist
	                                              LEFT JOIN T_TimeRecMod
		                                            ON Trm_ModDate = Ell_ProcessDate
	                                               AND Trm_EmployeeId = Ell_EmployeeId
	                                               AND Trm_Status IN ('1','3','5','7','9','A')
	                                             WHERE Ell_EmployeeId = '{0}'
	                                               AND Ell_ProcessDate BETWEEN DATEADD(month,-15, @MAXDATE) AND @MAXDATE
	                                             GROUP BY Convert(varchar(4),DATEPART(year,Ell_ProcessDate)) 
		                                             + Convert(varchar(20),DATEPART(month,Ell_ProcessDate)) 
		                                             , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate)
		                                             ) AS TEMP
                                                 GROUP BY [Date]
                                                       , [Year Month]
                                                 ORDER BY 1 ASC", Session["userLogged"].ToString());
        #endregion
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlGetTRCount, CommandType.Text);
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
                Chart1.Series["Series1"].Points.AddXY( ds.Tables[0].Rows[point]["Year Month"].ToString()
                                                     , Convert.ToDecimal(ds.Tables[0].Rows[point]["Count"].ToString()));
            }
            // Set series chart type
            Chart1.Series["Series1"].ChartType = SeriesChartType.Column;

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
