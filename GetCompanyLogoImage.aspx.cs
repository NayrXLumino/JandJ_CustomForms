using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using Payroll.DAL;
using System.Configuration;


public partial class GetCompanyLogoImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Drawing.Image tempImg;
        string qstring = Request.QueryString["dbConn"];
        DataRow dr = GetCompanyLogo(qstring);
        if (dr["Ccd_CompanyLogo"] != DBNull.Value)
        {
            byte[] compimage = (byte[])dr["Ccd_CompanyLogo"];
            MemoryStream ms = new MemoryStream(compimage);

            tempImg = System.Drawing.Image.FromStream(ms);
            tempImg.Save(Response.OutputStream, ImageFormat.Jpeg);
        }
        //Response.ContentType = "image/jpg";
        Response.Flush();
        Response.End();
    }

    public static DataRow GetCompanyLogo(string qString)
    {
        qString = qString.Replace(' ', '+');
        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper(qString))
        {
            try
            {
                string query = @"SELECT Ccd_CompanyLogo 
                                   FROM T_CompanyMaster";

                dt = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
            }
            catch( Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, "GET LOGO");
            }
        }
        if (dt.Rows.Count > 0)
            return dt.Rows[0];
        return null;
    }

}