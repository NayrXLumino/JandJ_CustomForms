using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Web.SessionState;
using Payroll.DAL;
using System.Drawing;

public class PrintHelper
{
    public PrintHelper()
    {
    }

    public static void PrintWebControl(Control[] ctrl)
    {
        PrintWebControl2(ctrl, string.Empty);
    }

    public static void PrintWebControl(Control[] ctrl, string Script)
    {
        
        StringWriter stringWrite = new StringWriter();
        HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

        Page pg = new Page();
        pg.EnableEventValidation = false;
        if (Script != string.Empty)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "PrintJavaScript", Script);
        }
        HtmlForm frm = new HtmlForm();
        pg.Controls.Add(frm);
        frm.Attributes.Add("runat", "server");
        
        foreach (Control crt in ctrl)
        {
            if (crt is GridView)
            {
                CommonMethods.PrepareControlForExport((GridView)crt);
                ((GridView)crt).HeaderRow.ForeColor = Color.Black;
                ((GridView)crt).BorderColor = Color.Black;
                ((GridView)crt).RowStyle.BorderColor = Color.Black;
                ((GridView)crt).RowStyle.BorderStyle = BorderStyle.None;
                ((GridView)crt).RowStyle.BorderWidth = 0;
            }
            //if (crt is WebControl)
            //{
            //    Unit w = new Unit(100, UnitType.Percentage); 
            //    ((WebControl)crt).Width = w;
            //}
            frm.Controls.Add(crt);
        }
        pg.DesignerInitialize();
        pg.RenderControl(htmlWrite);
        string strHTML = stringWrite.ToString();
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Write(strHTML);
        HttpContext.Current.Response.Write("<script>window.print();</script>");
        HttpContext.Current.Response.End();
    }

    public static void PrintWebControl2(Control[] ctrl, string Script)
    {

        StringWriter stringWrite = new StringWriter();
        HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
        Script = @"<style type='text/css'>
                          .TableHeader TR 
                            {
	                            page-break-inside: avoid;
                            }
                            THEAD
                            {
                                display: table-header-group;
                                page-break-inside: avoid;
                            }
@media print {
.header, .hide { visibility: hidden }
}
                          </style>";
        Page pg = new Page();
        pg.EnableEventValidation = false;
        if (Script != string.Empty)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "PrintJavaScript", Script);
        }
        HtmlForm frm = new HtmlForm();
        pg.Controls.Add(frm);
        frm.Attributes.Add("runat", "server");
        

        foreach (Control crt in ctrl)
        {
            int name = 0;
            if (crt is GridView)
            {
                GridView grdv = (GridView)crt;
                grdv.ID = "grdv" + name++;
                CommonMethods.PrepareControlForExport(grdv);
                grdv.Attributes.Clear();
                //grdv.Attributes.Add("width", "900px");
                grdv.CellSpacing = 0;
                grdv.CellPadding = 0;
                grdv.BorderStyle = BorderStyle.Solid;
                grdv.BorderWidth = 0;
                grdv.FooterRow.Visible = true;
                grdv.BorderColor = Color.Black;
                TableRow rt = grdv.HeaderRow;
                rt.Attributes.Clear();
                foreach (TableCell tc in rt.Cells)
                {
                    tc.Attributes.Clear();
                    tc.Font.Size = 9;
                    tc.ForeColor = Color.Black;
                    tc.Wrap = true;
                    tc.BorderStyle = BorderStyle.Solid;
                    tc.BorderWidth = 1;
                    tc.Height = 2;
                }
                for (int i = 0; i < grdv.Rows.Count; i++)
                {
                    TableRow tr = grdv.Rows[i];
                    tr.Attributes.Clear();
                    foreach (TableCell tc in tr.Cells)
                    {
                        tc.Attributes.Clear();
                        tc.Font.Size = 8;
                        tc.Wrap = true;
                        tc.BorderStyle = BorderStyle.Solid;
                        tc.BorderWidth = 1;
                    }
                }
                TableRow rf = grdv.FooterRow;
                foreach (TableCell tc in rf.Cells)
                {
                    tc.Attributes.Clear();
                    tc.Font.Size = 9;
                    tc.Wrap = true;
                    tc.BorderStyle = BorderStyle.None;
                    tc.BorderWidth = 0;
                    tc.Height = 2;
                }
                grdv.UseAccessibleHeader = true;
                grdv.HeaderRow.TableSection = TableRowSection.TableHeader;
                grdv.FooterRow.TableSection = TableRowSection.TableFooter;
                grdv.Attributes.Add("class", "TableHeader");
            }
            frm.Controls.Add(crt);
        }
        pg.DesignerInitialize();
        //htmlWrite.AddAttribute("class", "TableHeader");
        pg.RenderControl(htmlWrite);
        string strHTML = stringWrite.ToString();
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Write(strHTML);
        HttpContext.Current.Response.Write("<script>window.print();</script>");
        HttpContext.Current.Response.End();
    }
}