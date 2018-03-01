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


public partial class Transactions_Leave_Default : System.Web.UI.Page
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
        param[1] = new ParameterInfo("@SystemID", "LEAVE");
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
                    case "WFLVEENTRY":
                        btnIndividualLV.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFLVENOTEENTRY":
                        btnLeaveNotice.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFSPLLVEENTRY":
                        btnSpecialLV.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFLVEREP":
                        btnLeaveReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFLVECANCEL":
                        btnLeaveCancellation.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBLEAVEUPLD":
                        btnBatchLeaveUploading.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
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

                                    SELECT [Date]
                                         , [Year Month]
                                         , SUM([Hours]) [Hours]
                                     FROM ( SELECT Convert(varchar(4),DATEPART(year,Ell_ProcessDate))
                                                 + Convert(varchar(20),FORMAT(DATEPART(month,Ell_ProcessDate), '00')) [Date]
                                                 , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate) [Year Month]
                                                 , Convert(decimal(8,2),SUM(Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr) / 9) [Hours]
                                            FROM T_EmployeeLogLEdger
                                            WHERE Ell_EmployeeId = '{0}'
                                              AND Ell_ProcessDate BETWEEN DATEADD(month,-15, @MAXDATE) AND @MAXDATE
                                            GROUP BY Convert(varchar(4),DATEPART(year,Ell_ProcessDate)) 
                                                 + Convert(varchar(20),FORMAT(DATEPART(month,Ell_ProcessDate), '00'))
                                                 , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate)
                                            UNION
                                            SELECT Convert(varchar(4),DATEPART(year,Ell_ProcessDate)) 
                                                 + Convert(varchar(20),FORMAT(DATEPART(month,Ell_ProcessDate), '00')) [Date]
                                                 , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate) [Year Month]
                                                 , Convert(decimal(8,2),SUM(Ell_EncodedNoPayLeaveHr + Ell_EncodedPayLeaveHr) / 9)  [Hours]
                                              FROM T_EmployeeLogLEdgerHist
                                             WHERE Ell_EmployeeId = '{0}'
                                               AND Ell_ProcessDate BETWEEN DATEADD(month,-15, @MAXDATE) AND @MAXDATE
                                            GROUP BY Convert(varchar(4),DATEPART(year,Ell_ProcessDate)) 
                                                 +  Convert(varchar(20),FORMAT(DATEPART(month,Ell_ProcessDate), '00')) 
                                                 , DATENAME(year, Ell_ProcessDate) + ' ' + DATENAME(month, Ell_ProcessDate)
                                        ) AS TEMP
                                    GROUP BY [Date]
                                           , [Year Month]
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
                Chart1.Series["Series1"].Points.AddXY( ds.Tables[0].Rows[point]["Year Month"].ToString()
                                                     , Convert.ToDecimal(ds.Tables[0].Rows[point]["Hours"].ToString()));
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
