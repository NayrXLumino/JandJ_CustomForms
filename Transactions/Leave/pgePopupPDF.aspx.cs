using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

public partial class Transactions_Leave_pgePopupPDF : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.LoadComplete += new EventHandler(Transactions_Leave_pgePopupPDF_LoadComplete);
    }

    void Transactions_Leave_pgePopupPDF_LoadComplete(object sender, EventArgs e)
    {
        DisplayPDF();
    }

    private void DisplayPDF()
    {
        try
        {
            string transactiontype = Request.QueryString["vt"].ToString().Trim();
            string encrypt = Request.QueryString["dx"].ToString().Trim().Replace("'", "").Replace(" ", "+");
            string controlNo = CommonLibrary.Encrypt.decryptText(encrypt).ToString();
            controlNo = Request.QueryString["dx"].ToString().Trim().Replace("'", "");
            string filename = Request.QueryString["df"].ToString().Trim();
            string path = string.Empty;
            path = Resources.Resource.CLINICSYSUPLDLOC + controlNo + "/" + filename;
            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(path);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", buffer.Length.ToString());
            Response.BinaryWrite(buffer);
            //Response.Write("Encrypt : " + encrypt + "<br />Decrypt : " + CommonLibrary.Encrypt.decryptText(encrypt).ToString() + "<br />Filename : " + filename.Replace("--", "."));
        }
        catch
        {

        }
    }
}