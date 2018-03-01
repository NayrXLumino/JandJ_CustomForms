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


public partial class Transactions_CustomForms_ApplicationForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        //if (RadioButtonList1.SelectedIndex > -1)
        //{
        //    Label1.Text = "You selected: " + RadioButtonList1.SelectedItem.Text;
        //}
    }
    protected void chkLayout_CheckedChanged(object sender, EventArgs e)
    {
        //if (chkLayout.Checked == true)
        //{
        //    RadioButtonList1.RepeatLayout = RepeatLayout.Table;
        //}
        //else
        //{
        //    RadioButtonList1.RepeatLayout = RepeatLayout.Flow;
        //}  
    }
    protected void chkDirection_CheckedChanged(object sender, EventArgs e)
    {
        //if (chkDirection.Checked == true)
        //{
        //    RadioButtonList1.RepeatDirection = RepeatDirection.Horizontal;
        //}
        //else
        //{
        //    RadioButtonList1.RepeatDirection = RepeatDirection.Vertical;
        //} 
    }
}