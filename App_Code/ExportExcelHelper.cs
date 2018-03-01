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
using Payroll.DAL;
using System.Drawing;

/// <summary>
/// Summary description for ExportExcelHelper
/// </summary>
public class ExportExcelHelper
{
    public ExportExcelHelper()
    { }

    public ExportExcelHelper(Control[] ctrl, string Filename)
    {
        ExportControl(ctrl, Filename);
    }

    public static void ExportControl(Control[] ctrl, string Filename)
    {
        string attachment = string.Format("attachment; filename={0}.xls", Filename);
        Page pg = new Page();
        pg.EnableEventValidation = false;

        StringWriter sw = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        HtmlForm frm = new HtmlForm();
        pg.Controls.Add(frm);
        frm.Attributes["runat"] = "server";
        foreach (Control ctrl1 in ctrl)
        {
            if (ctrl1 is Label)
            {
                ((Label)ctrl1).ForeColor = Color.Red;
                frm.Controls.Add(ctrl1);
            }
            else if (ctrl1 is GridView)
            {
                frm.Controls.Add(ctrl1);
            }
            else if (ctrl1 is Panel)
            {
                frm.Controls.Add(ctrl1);
            }
        }
        frm.RenderControl(htw);

        HttpContext.Current.Response.ClearContent();
        HttpContext.Current.Response.AddHeader("content-disposition", attachment);
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        HttpContext.Current.Response.Write(sw.ToString());
        HttpContext.Current.Response.End();
    }

    public static void ExportControl2(Control[] ctrl, string Filename)
    {
        string attachment = string.Format("attachment; filename={0}.xls", Filename);
        Page pg = new Page();
        pg.EnableEventValidation = false;

        StringWriter sw = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        HtmlForm frm = new HtmlForm();
        pg.Controls.Add(frm);
        frm.Attributes["runat"] = "server";
        foreach (Control ctrl1 in ctrl)
        {
            if (ctrl1 is Label)
            {
                ((Label)ctrl1).ForeColor = Color.Black;
                frm.Controls.Add(ctrl1);
            }
            else if (ctrl1 is GridView)
            {
                GridView grdView = (GridView)ctrl1;
                grdView.BorderStyle = BorderStyle.None;
                grdView.BorderWidth = 0;
                grdView.HeaderRow.Style.Add("background-color", "WHITE");
                grdView.HeaderRow.ForeColor = Color.Black;
                grdView.FooterRow.Style.Add("background-color", "LIGHTYELLOW");
                for (int i = 0; i < grdView.HeaderRow.Cells.Count; i++)
                {
                    grdView.HeaderRow.Cells[i].Font.Size = 8;
                    //grdView.HeaderRow.Cells[i].Style.Add("background-color", "#006666");
                    grdView.HeaderRow.Cells[i].Text = HttpUtility.HtmlDecode(grdView.HeaderRow.Cells[i].Text);
                }
                for (int i = 0; i < grdView.Rows.Count; i++)
                {
                    GridViewRow row = grdView.Rows[i];
                    row.BackColor = System.Drawing.Color.White;
                    foreach (TableCell tc in row.Cells)
                    {
                        tc.Font.Size = 8;
                        tc.Wrap = false;
                        tc.Attributes.Add("width", "100%");
                    }
                }
                for (int i = 0; i < grdView.FooterRow.Cells.Count; i++)
                {
                    grdView.FooterRow.Cells[i].Font.Size = 8;
                }
                frm.Controls.Add(grdView);
            }
            else if (ctrl1 is Panel)
            {
                frm.Controls.Add(ctrl1);
            }
        }
        frm.RenderControl(htw);

        //style to format numbers with decimal values
        string style = @"<style>.text{mso-number-format:\@;text-align:center;};
                                .Nums{mso-number-format:_(* #,##0.00_);};
                                .unwrap{wrap:false}</style>";

        HttpContext.Current.Response.ClearContent();
        HttpContext.Current.Response.Write(style);
        HttpContext.Current.Response.AddHeader("content-disposition", attachment);
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        HttpContext.Current.Response.Write(sw.ToString());
        HttpContext.Current.Response.End();
    }

    public static void ExportGridView(GridView GridView1)
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Buffer = true;
        HttpContext.Current.Response.AddHeader("content-disposition",
        "attachment;filename=GridViewExport.xls");
        HttpContext.Current.Response.Charset = "";
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);

        GridView1.DataBind();
        //Change the Header Row back to white color
        GridView1.HeaderRow.Style.Add("background-color", "#FFFFFF");
        //Apply style to Individual Cells
        for (int i = 0; i < GridView1.HeaderRow.Cells.Count; i++)
        {
            GridView1.HeaderRow.Cells[i].Style.Add("background-color", "green");
        }
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            GridViewRow row = GridView1.Rows[i];
            //Change Color back to white
            row.BackColor = System.Drawing.Color.White;
            //Apply text style to each Row
            //row.Attributes.Add("class", "text");
        }
        GridView1.RenderControl(hw);
        //style to format numbers to string
        //string style = @"<style> .text { mso-number-format:\@; } </style>";
        //HttpContext.Current.Response.Write(style);
        HttpContext.Current.Response.Output.Write(sw.ToString());
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.End();
    }
}
