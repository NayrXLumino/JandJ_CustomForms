/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/17/2013
 *  Update Notes    :   
 *      -  Store Message in Session
 */
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/// <summary>
/// Summary description for DisplayAlertMessage
/// </summary>
namespace Payroll.DAL
{
    public static class MessageBox
    {
        // Create a string builder to help format the javascript
        //private static StringBuilder sb;

        public static void Show(string strMessage)
        {
            // make sure our string builder is initialized with an empty value
            StringBuilder sb = new StringBuilder();

            // Rewrite our Message to conform with javascript syntax
            strMessage = strMessage.Replace("\n", "\\n");
            strMessage = strMessage.Replace("\"", "'");

            // Create the javascript to be sent to the client
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append(@"alert( """ + strMessage + @""" );");
            sb.Append(@"</script>");

            // Get the page that is requesting this method
            Page pgExecutingPage = HttpContext.Current.Handler as Page;

            // Wire up the unload event of the requesting page so that we can
            // add our javascript to the end of the response
            HttpContext.Current.Session["MessageAlert"] = sb.ToString();
            pgExecutingPage.Unload += new EventHandler(pgExecutingPage_Unload);
        }

        private static void pgExecutingPage_Unload(object sender, EventArgs e)
        {
            try
            {
                // Write the javascript to the end of the current response
                HttpContext.Current.Response.Write(HttpContext.Current.Session["MessageAlert"].ToString());
                HttpContext.Current.Session["MessageAlert"] = string.Empty;
            }
            catch
            { 
            
            }
        }

        public static void writeErrorLog(string message, string file)
        {
            try
            {
                StreamWriter sw = File.AppendText("C:\\wferrorlogs.txt");
                sw.WriteLine(DateTime.Now.ToLongTimeString()
                            + "[" + file + "]"
                            + ": "
                            + message);
                sw.Close();
            }
            catch (Exception ex)
            {
                FileStream fs = File.Open("C:\\wferrorlogs.txt", FileMode.CreateNew, FileAccess.Write);
                StreamWriter swx = new StreamWriter(fs, System.Text.Encoding.UTF8);
                Console.WriteLine(DateTime.Now.ToLongTimeString()
                                 + "[" + file + "]"
                                 + ": "
                                 + ex.Message);
                swx.Close();
            }
        }
    }
}

