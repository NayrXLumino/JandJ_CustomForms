using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;

public partial class pgeAnnouncementsEx : System.Web.UI.Page
{
    GeneralBL GNBL = new GeneralBL();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {
            DataSet ds = new DataSet();
            string sql = @"SELECT Prf_DataBaseNo
                             FROM T_Profiles
                            WHERE Prf_Status = 'A'
                              AND Prf_DataBaseNo IN (SELECT Upt_DatabaseNo 
                                                       FROM T_UserProfile 
                                                      WHERE Upt_UserCode = '{0}' 
                                                        AND Upt_Status = 'A')";
            using (DALHelper dal = new DALHelper(false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, Session["userLogged"].ToString()), CommandType.Text);
                }
                catch (Exception ex)
                {
                    CommonMethods.ErrorsToTextFile(ex, "SA");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            string dbId = string.Empty;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string temp = string.Empty;
                for (int ctr = 0; ctr < ds.Tables[0].Rows.Count; ctr++)
                {
                    dbId += ds.Tables[0].Rows[ctr]["Prf_DataBaseNo"].ToString() + ",";
                }
            }
            hfDB.Value = Encrypt.encryptText(dbId);
            GenerateView();
        }
    }


    private DataTable GetData()
    {

        DataSet ds = new DataSet();
        string sql = @"
                    declare @param int
                    SET @param = (SELECT ISNULL(Pmt_NumericValue,30)
				                    FROM T_ParameterMaster
		                           WHERE Pmt_ParameterId = 'ANNCRANGE')

                    SELECT Convert(varchar(10),A.Amt_AnnounceDateTime,101) AS [Announce Date]
                         , Convert(varchar(5),A.Amt_AnnounceDateTime,114) AS [Announce Time]
                         , A.Amt_Subject AS [Subject]
                         , A.Amt_Description AS [Information]
                         , Amt_Priority
                         , CASE Amt_Priority
                           WHEN '1' THEN 'HIGH'
                           WHEN '2' THEN 'MID'
                           WHEN '3' THEN 'LOW'
                            END AS [Priority]
                         , A.Amt_Announcer AS [Announced By]
                         , CASE A.Amt_Status 
                           WHEN 'A' THEN 'ACTIVE'
                           WHEN 'C' THEN 'CANCELLED' 
                            END AS [Status]
                         , A.ludatetime AS [Last Updated]
                      FROM T_AnnouncementMaster A
	                 WHERE A.Amt_AnnounceDateTime BETWEEN DATEADD (day , @param * -1 , getdate())  AND getdate()
                       AND Amt_Status = 'A'
                       {0}
                     ORDER BY A.Amt_AnnounceDateTime DESC , A.Amt_Priority ASC ";
        try
        {
            string[] ProfileDBConnections = GNBL.GetProfileConnections();
            using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(sql, getFilter()), CommandType.Text);
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
        }
        catch
        {
            ds = new DataSet();
        }

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0];
        }
        else
        {
            return new DataTable("dummy");
        }
    }

    private string getFilter()
    {
        string filter = string.Empty;
        filter += @" AND Amt_ProfileInclude = 'ALL' ";
        string[] separator = new string[1];
        separator[0] = ",";
        string[] dbID = Encrypt.decryptText(hfDB.Value).ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
        filter += " OR ( ";
        for (int i = 0; i < dbID.Length; i++)
        {
            if (i == 0)
            {
                filter += string.Format(" Amt_ProfileInclude LIKE '%!{0}!%'", dbID[i]);
            }
            else
            {
                filter += string.Format(" OR Amt_ProfileInclude LIKE '%!{0}!%'", dbID[i]);
            }
        }

        filter += @" ) ";
        return filter;
    }

    private void GenerateView()
    {
        DataTable dt = GetData();
        string holdDate = string.Empty;
        bool isNewDate = true;
        bool firstRow = true;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (!firstRow)
                {
                    if (!holdDate.Equals(dr["Announce Date"].ToString()))
                    {
                        isNewDate = true;
                    }
                }
                if (holdDate.Equals(string.Empty) || isNewDate)
                {
                    holdDate = dr["Announce Date"].ToString();
                    tblAnnounce.Rows.Add(AddHeader(holdDate));
                    isNewDate = false;
                }
                firstRow = false;
                tblAnnounce.Rows.Add(AddBody(dr["Announce Time"].ToString()
                                            , dr["Subject"].ToString()
                                            , dr["Information"].ToString()
                                            , dr["Announced By"].ToString()
                                            , dr["Amt_Priority"].ToString()));
            }
        }
        else
        {
            TableRow tr1 = null;
            TableCell tc1 = null;
            tr1 = new TableRow();
            tc1 = new TableCell();
            tc1.Text = "<hr><h5>No Announcements</h5> ";
            tc1.Width = Unit.Pixel(890);
            tc1.Height = Unit.Percentage(100);
            tc1.ColumnSpan = 2;
            tr1.Cells.Add(tc1);
            tblAnnounce.Rows.Add(tr1);
        }
        //        //Getting Started
        //        TableRow tr = null;
        //        TableCell tc = null;

        //        tr = new TableRow();
        //        tc = new TableCell();
        //        tc.Text = "<hr><hr><h5>Getting started...</h5> ";
        //        tc.ColumnSpan = 2;
        //        tr.Cells.Add(tc);
        //        tblAnnounce.Rows.Add(tr);

        //        tr = new TableRow();
        //        tc = new TableCell();
        //        tc.Text = "STEP 1";
        //        tc.Width = Unit.Pixel(160);
        //        tc.VerticalAlign = VerticalAlign.Top;
        //        tr.Cells.Add(tc);
        //        tc = new TableCell();
        //        tc.Text = @"Login at the left panel of this page.
        //                    The username of your login is your ID number. 
        //                    You are provided with a 5-digit system generated
        //                    password. Please change your
        //                    password during your first login.";
        //        tr.Cells.Add(tc);
        //        tblAnnounce.Rows.Add(tr);

        //        tr = new TableRow();
        //        tc = new TableCell();
        //        tc.Text = "STEP 2";
        //        tc.VerticalAlign = VerticalAlign.Top;
        //        tr.Cells.Add(tc);
        //        tc = new TableCell();
        //        tc.Text = @"After logging in you will be redirected to the page where
        //                    you can navigate the workflow. Some navigation might not 
        //                    be available to you due to some restrictions to the user 
        //                    type.";
        //        tr.Cells.Add(tc);
        //        tblAnnounce.Rows.Add(tr);

        //        tr = new TableRow();
        //        tc = new TableCell();
        //        tc.Text = "STEP 3";
        //        tc.VerticalAlign = VerticalAlign.Top;
        //        tr.Cells.Add(tc);
        //        tc = new TableCell();
        //        tc.Text = @"Make the necessary transactions.";
        //        tr.Cells.Add(tc);
        //        tblAnnounce.Rows.Add(tr);

        //        tr = new TableRow();
        //        tc = new TableCell();
        //        tc.Text = "STEP 4";
        //        tc.VerticalAlign = VerticalAlign.Top;
        //        tr.Cells.Add(tc);
        //        tc = new TableCell();
        //        tc.Text = @"Logout.";
        //        tr.Cells.Add(tc);
        //        tblAnnounce.Rows.Add(tr);



    }

    private TableRow AddHeader(string dateHeader)
    {
        TableRow tr = new TableRow();
        TableCell tc = new TableCell();
        tc.ColumnSpan = 2;
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.Font.Bold = true;
        tc.Text = "<hr /><br />";
        tc.Text += Convert.ToDateTime(dateHeader).ToString("MMM dd yyyy") + " - " + Convert.ToDateTime(dateHeader).DayOfWeek;
        tc.CssClass = "cell0";
        tc.Font.Size = FontUnit.Large;
        tr.Cells.Add(tc);

        return tr;
    }

    private TableRow AddBody(string hhmmTime, string subject, string info, string announcer, string priority)
    {
        TableRow tr = new TableRow();
        TableCell tc = null;

        //Time 1st Column
        string temp = announcer.Trim().Equals(string.Empty) ? "-NO INFO-" : announcer.Trim().ToUpper();
        tc = new TableCell();
        tc.Text = "[" + hhmmTime + "]  " + subject + "<br />" + "by: <u>" + temp + "</u>";
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.VerticalAlign = VerticalAlign.Top;
        tc.Width = Unit.Pixel(200);
        tc.Height = Unit.Percentage(100);
        tc.Wrap = true;
        tc.CssClass = "cell1";
        switch (priority)
        {
            case "1":
                tc.ForeColor = System.Drawing.Color.Red;
                break;
            case "2":
                tc.ForeColor = System.Drawing.Color.DodgerBlue;
                break;
            default:
                break;
        }
        tr.Cells.Add(tc);


        //Information 2nd Column
        tc = new TableCell();
        tc.Text = info.Replace("\r\n", "<br />");
        tc.HorizontalAlign = HorizontalAlign.Left;
        tc.VerticalAlign = VerticalAlign.Top;
        tc.Wrap = true;
        tc.Width = Unit.Pixel(710);
        tc.Height = Unit.Percentage(100);
        tc.CssClass = "cell2";
        switch (priority)
        {
            case "1":
                tc.ForeColor = System.Drawing.Color.Red;
                break;
            case "2":
                tc.ForeColor = System.Drawing.Color.DodgerBlue;
                break;
            default:
                break;
        }
        tr.Cells.Add(tc);

        return tr;
    } 
}