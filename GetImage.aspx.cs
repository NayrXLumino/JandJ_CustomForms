using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using Payroll.DAL;
using System.Configuration;

public partial class GetImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Drawing.Image tempImg;
        string qstring = Request.QueryString["dbConn"];
        DataRow dr = GetCompanyLogo(qstring);
        byte[] compimage = (byte[])dr["Scm_CompanyLogo"];
        MemoryStream ms = new MemoryStream(compimage);

        tempImg = System.Drawing.Image.FromStream(ms);
        tempImg.Save(Response.OutputStream, ImageFormat.Jpeg);
        //Response.ContentType = "image/jpg";
        Response.Flush();
        Response.End();

        
    }

    public static DataRow GetCompanyLogo(String queryString)
    {
        queryString = queryString.Replace(' ', '+');
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper(queryString))
        {
            try
            {
                string query = @"SELECT Scm_CompanyLogo FROM {0}..T_SystemControl";

                dt = dal.ExecuteDataSet(string.Format(query, ConfigurationManager.AppSettings["ERP_DB"]), CommandType.Text).Tables[0];
            }
            catch { }
        }
        if (dt.Rows.Count > 0)
            return dt.Rows[0];
        return null;
    }
}