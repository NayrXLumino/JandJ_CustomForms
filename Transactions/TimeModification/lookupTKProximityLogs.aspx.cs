using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;

public partial class Transactions_TimeModification_lookupTKProximityLogs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            this.Page.Controls.Clear();
            Response.Write("Connection timed-out. Close this window and try again.");
            Response.Write("<script type='text/javascript'>window.close();</script>");
        }
        else if (!Page.IsPostBack)
        {
            getProximityLogs();
        }
    }

    #region Methods
    private void getProximityLogs()
    {
        string sql = string.Format(@"   SELECT Convert(varchar(10),Plt_LogDate,101) [Date]
                                             , LEFT(Plt_LogTime,2) +':'+ RIGHT(Plt_LogTime,2) [Time]
                                          FROM E_ProximityLogs
                                         WHERE Plt_EmployeeId = '{0}'
                                           AND Convert(varchar(10), Plt_LogDate, 101) = '{1}'
                                         UNION
                                        SELECT [Date],[Time]
                                          FROM (SELECT TOP 1 Convert(varchar(10),Plt_LogDate,101) [Date]
	                                                 , LEFT(Plt_LogTime,2) +':'+ RIGHT(Plt_LogTime,2) [Time]
                                                  FROM E_ProximityLogs
                                                 WHERE Plt_EmployeeId = '{0}'
                                                   AND Convert(varchar(10), Plt_LogDate, 101) = DATEADD(dd,-1,Convert(datetime,'{1}') )
                                                 ORDER BY 2 DESC) as temp1
                                         UNION
                                        SELECT [Date],[Time]
                                          FROM (SELECT TOP 1 Convert(varchar(10),Plt_LogDate,101) [Date]
	                                                 , LEFT(Plt_LogTime,2) +':'+ RIGHT(Plt_LogTime,2) [Time]
                                                  FROM E_ProximityLogs
                                                 WHERE Plt_EmployeeId = '{0}'
                                                   AND Convert(varchar(10), Plt_LogDate, 101) = DATEADD(dd,1,Convert(datetime,'{1}') )
                                                 ORDER BY 2 ASC) as temp2
                                         ORDER BY 1 ASC, 2 ASC", Request.QueryString["id"].ToString()
                                                               , Request.QueryString["dt"].ToString());
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
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
            dgvResult.DataSource = ds;
            dgvResult.DataBind();
        }
    }
    #endregion

    #region Events
    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!e.Row.Cells[0].Text.Equals(Request.QueryString["dt"].ToString()))
            {
                e.Row.Attributes.Add("style", "background-color:#BBFFAA");
            }
            else
            {
                e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            }
        }
    }
    #endregion
}

