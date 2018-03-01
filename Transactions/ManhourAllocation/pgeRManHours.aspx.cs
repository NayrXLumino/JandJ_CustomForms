/*
 *  Updated By      :   0951 - Te, Perth
 *  Updated Date    :   04/23/2013
 *  Update Notes    :   
 *      -   Updated Export File Name
 */
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxGridView.Export;
using System.Collections.Generic;
using MethodsLibrary;

public partial class Default2 : System.Web.UI.Page
{
    MenuGrant MGBL = new MenuGrant();
    private DataSet dsView;
    int innerHeight = 0;
    int outerHeight = 0;
    private decimal grandTotal = 0;
    private bool print = false;
    private List<string> categoryList;
    private List<string> categoryListCopy;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            if (!Page.IsCallback)
            {
                Response.Redirect("../../index.aspx?pr=dc");
            }
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFMANHOURREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            // options will be removed
            //for (int i = 0; i < rblOption.Items.Count; i++)   
            //    rblOption.Items[i].Attributes.Add("oncLick", "return CheckPopUp();");

            getWeedStartandEnd();
            if (!Page.IsPostBack)
            {
                DataRow dr = CommonLookUp.GetCheckApproveRights(Session["userLogged"].ToString(), "WFMANHOURREP");
                if (dr != null)
                {
                    bool canCheck = Convert.ToBoolean(dr["Ugt_CanCheck"]);
                    bool canApprove = Convert.ToBoolean(dr["Ugt_CanApprove"]);

                    btnExport.Visible = Convert.ToBoolean(dr["Ugt_CanGenerate"]);
                    btnPrint.Visible = Convert.ToBoolean(dr["Ugt_CanPrint"]);
                }

                //rbEmployeeOptions.SelectedIndex = 0;
                //rblCode.SelectedIndex = 0;
                rblBillable.SelectedValue = "A";
                //ddlDateType.SelectedValue = "D";
                //ddlDateType.Enabled = false;
                this.NumberOfControls = 0;
                ddlBilling.Enabled = false;
                dsView = null;
                initializeControls();
            }

            LoadComplete += new EventHandler(_Default_LoadComplete);
        }
    }

    #region Events

    protected void _Default_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "manhourscripts";
        string jsurl = "_manhour.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;

        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        Calendar cal1 = (Calendar)dtpRangeFrom.Controls[3];
        cal1.Attributes.Add("OnClick", "javascript:return checkRange('from')");

        Calendar cal2 = (Calendar)dtpRangeTo.Controls[3];
        cal2.Attributes.Add("OnClick", "javascript:return checkRange('to')");

        //ddlDateType.Attributes.Add("OnChange", "return CheckPopUp();");

        ddlBilling.Attributes.Add("OnChange", "javascript:return checkRange('selection')");

        chkDefaultReport.Attributes.Add("OnClick", "javascript:checkDefaultReport();");
        chkSubWorkCode.Attributes.Add("OnClick", "javascript:CheckWorkCode();");

        ddlReport.Attributes.Add("OnChange", "javascript:hideOptions()");
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        dsView = null;
        Panel1.Controls.Clear();
        txtDashJobCode.Text = "";
        txtClientCode.Text = "";
        txtClientJobNo.Text = "";
        txtFWBSCode.Text = "";
        txtEmpName.Text = "";
        txtCostCenter.Text = "";
        txtWorkCode.Text = "";
        txtSubWorkCode.Text = "";
        rblOption.SelectedValue = "E";
        rblBillable.SelectedValue = "A";
        rbEmployeeOptions.SelectedValue = "S";
        cbUnsplitted.Checked = false;
        chkDefaultReport.Checked = true;
        chkSubWorkCode.Checked = true;
        ddlDateType.SelectedValue = "D";
        ddlBilling.SelectedValue = "";
        dtpRangeFrom.Reset();
        dtpRangeTo.Reset();
        this.NumberOfControls = 0;
        ddlReport.SelectedValue = "";
        btnTwist.Visible = false;
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (isProfileSelected())
        {
            if (ddlReport.SelectedValue == "1" || ddlReport.SelectedValue == "2")
            {
                if (ddlReport.SelectedValue != hfSelectedRepIndex.Value)
                {
                    btnGenerate_Click(sender, e);
                }

                GridViewExporterHeaderFooter headerFooter = ASPxGridViewExporter1.PageHeader;

                headerFooter.Left = Methods.GetCompanyInfoERP("Scm_CompName") + "\r\n\r\n"
                                     + Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
                                     + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
                                     + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
                                     + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos");

                ASPxGridViewExporter1.ReportHeader = ddlReport.Items[Convert.ToInt32(hfSelectedRepIndex.Value)].Text;

                ASPxGridViewExporter1.WriteXlsToResponse(ddlReport.Text.Trim(), new DevExpress.XtraPrinting.XlsExportOptions(DevExpress.XtraPrinting.TextExportMode.Value));
            }
            else
            {
                if (Panel1.Controls.Count > 0)
                {
                    btnGenerate_Click(sender, null);

                    try
                    {
                        Control[] ctrl;
                        if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")
                        {
                            ctrl = new Control[this.NumberOfControls + 2];
                        }
                        else
                        {
                            ctrl = new Control[this.NumberOfControls + 1];
                        }

                        string reportTitle = "";
                        if (chkDefaultReport.Checked)
                        {
                            reportTitle = string.Format("Man Hour Report Per {0}", rblOption.SelectedItem.Text.Trim());
                        }
                        else
                        {
                            string reportDateRange = "";
                            if (!dtpRangeFrom.DateString.Equals(string.Empty))
                            {
                                reportDateRange += " from " + dtpRangeFrom.DateString;
                            }
                            if (!dtpRangeTo.DateString.Equals(string.Empty))
                            {
                                reportDateRange += " to " + dtpRangeTo.DateString;
                            }

                            if (rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D" && ddlDateType.SelectedValue == "M")
                            {
                                reportTitle = "Manhour Monthly Summary" + reportDateRange;
                            }
                            else
                            {
                                reportTitle = "Department Manhour Report" + reportDateRange;
                            }
                        }

                        ctrl[0] = CommonLookUp.HeaderPanelOptionERP(this.VSDataTable.Columns.Count, reportTitle, initializeHeader());

                        int ctr = 1;
                        foreach (Control panelctrl in Panel1.Controls)
                        {
                            if (panelctrl is Label)
                            {
                                ctrl[ctr++] = panelctrl;
                            }
                            else if (panelctrl is GridView)
                            {
                                ctrl[ctr++] = panelctrl;
                            }
                            if (panelctrl.HasControls())
                            {
                                foreach (Control child in panelctrl.Controls)
                                {
                                    if (child is Label)
                                    {
                                        ctrl[ctr++] = child;
                                    }
                                    else if (child is GridView)
                                    {
                                        ctrl[ctr++] = child;
                                    }
                                    if (child.HasControls())
                                    {
                                        foreach (Control child2 in child.Controls)
                                        {
                                            if (child2 is Label)
                                            {
                                                ctrl[ctr++] = child2;
                                            }
                                            else if (child2 is GridView)
                                            {
                                                ctrl[ctr++] = child2;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (chkDefaultReport.Checked)
                        {
                            ExportExcelHelper.ExportControl2(ctrl, string.Format("Manhours Report Per {0}", rblOption.SelectedItem.Text.Trim()));
                        }
                        else
                        {
                            ExportExcelHelper.ExportControl2(ctrl, "Department Manhour Report");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Some error occurred during exporting to Excel.\nPlease Try Again!");
                    }
                }
                else
                {
                    MessageBox.Show("No Records Found!");
                }
            }
        }
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        if (isProfileSelected())
        {
            Button button;
            if (sender.GetType().Name.Equals("Button"))
            {
                button = (Button)sender;
                if (button.ID.Equals("btnTwist"))
                {
                    hfTwist.Value = (!Convert.ToBoolean(hfTwist.Value)).ToString();
                }
            }

            setOptions();

            hfSelectedRepIndex.Value = ddlReport.SelectedIndex.ToString();

            if (ddlReport.SelectedValue == "1" || ddlReport.SelectedValue == "2")
            {
                this.VSDataTable = null;
                ASPxGridView1.Columns.Clear();

                ASPxGridView1.DataBind();

                ASPxGridView1.TotalSummary.Clear();

                if (categoryListCopy != null)
                {
                    foreach (string category in categoryListCopy)
                    {
                        ASPxSummaryItem asiLabel = new ASPxSummaryItem("Work Description", DevExpress.Data.SummaryItemType.Sum);
                        ASPxGridView1.TotalSummary.Add(asiLabel);

                        ASPxSummaryItem asiJob = new ASPxSummaryItem("Job Description", DevExpress.Data.SummaryItemType.Sum);
                        ASPxGridView1.TotalSummary.Add(asiJob);
                        
                        ASPxSummaryItem asiREG = new ASPxSummaryItem("REG", DevExpress.Data.SummaryItemType.Custom);
                        asiREG.DisplayFormat = "0.00";
                        asiREG.Tag = category;
                        ASPxGridView1.TotalSummary.Add(asiREG);

                        ASPxSummaryItem asiOT = new ASPxSummaryItem("OT", DevExpress.Data.SummaryItemType.Custom);
                        asiOT.DisplayFormat = "0.00";
                        asiOT.Tag = category;
                        ASPxGridView1.TotalSummary.Add(asiOT);

                        ASPxSummaryItem asiManhours = new ASPxSummaryItem("Manhours", DevExpress.Data.SummaryItemType.Custom);
                        asiManhours.DisplayFormat = "0.00";
                        asiManhours.Tag = category;
                        ASPxGridView1.TotalSummary.Add(asiManhours);
                    }

                    categoryList = new List<string>(categoryListCopy);
                }

                ASPxSummaryItem asiLabelGT = new ASPxSummaryItem("Work Description", DevExpress.Data.SummaryItemType.Sum);
                ASPxGridView1.TotalSummary.Add(asiLabelGT);

                ASPxSummaryItem asiLabelGTJob = new ASPxSummaryItem("Job Description", DevExpress.Data.SummaryItemType.Sum);
                ASPxGridView1.TotalSummary.Add(asiLabelGTJob);

                ASPxSummaryItem asiREGGT = new ASPxSummaryItem("REG", DevExpress.Data.SummaryItemType.Sum);
                asiREGGT.DisplayFormat = "0.00";
                ASPxGridView1.TotalSummary.Add(asiREGGT);

                ASPxSummaryItem asiOTGT = new ASPxSummaryItem("OT", DevExpress.Data.SummaryItemType.Sum);
                asiOTGT.DisplayFormat = "0.00";
                ASPxGridView1.TotalSummary.Add(asiOTGT);

                ASPxSummaryItem asiManHoursGT = new ASPxSummaryItem("Manhours", DevExpress.Data.SummaryItemType.Sum);
                asiManHoursGT.DisplayFormat = "0.00";
                ASPxGridView1.TotalSummary.Add(asiManHoursGT);

                if (ASPxGridView1.Columns.Count > 0)
                {
                    ASPxGridView1.GroupBy(ASPxGridView1.Columns["Department"]);
                    if (ddlReport.SelectedValue == "1")
                        ASPxGridView1.GroupBy(ASPxGridView1.Columns["Job"]);
                    ASPxGridView1.GroupBy(ASPxGridView1.Columns["Category"]);
                    if (ddlReport.SelectedValue == "1")
                        ASPxGridView1.GroupBy(ASPxGridView1.Columns["Employee"]);
                }

                if (ASPxGridView1.DataSource != null)
                {
                    ASPxGridView1.Visible = true;
                }
                else
                {
                    ASPxGridView1.Visible = false;
                }

                ASPxGridView1.ExpandAll();

                this.NumberOfControls = 12;
            }
            else
            {
                Panel1.Attributes["Height"] = "Inherit";
                GetData();

                if (dsView != null && dsView.Tables.Count > 0)
                {
                    innerHeight = 0;
                    outerHeight = 0;
                    GenerateTables();
                    this.VSDataTable = dsView.Tables[0];
                }
                else
                {
                    Panel1.Controls.Clear();
                    dsView = null;
                }
            }

            if (ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7")
            {
                btnTwist.Visible = true;
            }
            else
            {
                btnTwist.Visible = false;
            }
        }
        //MenuLOg
        SystemMenuLogBL.InsertGenerateLog("WFMANHOURREP", Session["userLogged"].ToString(), true, Session["userLogged"].ToString());
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (isProfileSelected())
        {
            if (ddlReport.SelectedValue == "1" || ddlReport.SelectedValue == "2")
            {
                if (ddlReport.SelectedValue != hfSelectedRepIndex.Value)
                {
                    btnGenerate_Click(sender, e);
                }

                GridViewExporterHeaderFooter headerFooter = ASPxGridViewExporter1.PageHeader;

                headerFooter.Left = Methods.GetCompanyInfoERP("Scm_CompName") + "\r\n\r\n"
                                    + Methods.GetCompanyInfoERP("Scm_CompAddress1") + Environment.NewLine
                                    + Methods.GetCompanyInfoERP("Scm_CompAddress2") + Environment.NewLine
                                    + "TEL NO. " + Methods.GetCompanyInfoERP("Scm_TelephoneNos").Trim()
                                    + " FAX NO. " + Methods.GetCompanyInfoERP("Scm_FaxNos");

                ASPxGridViewExporter1.ReportHeader = ddlReport.Items[Convert.ToInt32(hfSelectedRepIndex.Value)].Text;
                ASPxGridViewExporter1.Landscape = false;
                ASPxGridViewExporter1.BottomMargin = 0;
                ASPxGridViewExporter1.LeftMargin = 0;
                ASPxGridViewExporter1.RightMargin = 0;
                ASPxGridViewExporter1.PaperKind = System.Drawing.Printing.PaperKind.A4;
                ASPxGridViewExporter1.Styles.Cell.Font.Size = FontUnit.Small;
                ASPxGridViewExporter1.Styles.Header.Font.Size = FontUnit.Small;
                ASPxGridViewExporter1.Styles.Footer.Font.Size = FontUnit.Small;
                ASPxGridViewExporter1.Styles.GroupFooter.Font.Size = FontUnit.Small;
                ASPxGridViewExporter1.Styles.GroupRow.Font.Size = FontUnit.Small;

                headerFooter.VerticalAlignment = DevExpress.XtraPrinting.BrickAlignment.None;

                if (ddlReport.SelectedValue == "1")
                {
                    if (rblInclude.SelectedValue != "M" && rblInclude.SelectedValue != "F")
                    {
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 360;
                    }
                    else if (rblInclude.SelectedValue == "M")
                    {
                        ASPxGridView1.Columns["MS Code and Description"].ExportWidth = 240;
                        ASPxGridView1.Columns["MM Code and Description"].ExportWidth = 240;
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 210;
                        ASPxGridViewExporter1.Landscape = true;
                    }
                    else if (rblInclude.SelectedValue == "F")
                    {
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 300;
                        ASPxGridView1.Columns["FBS Code and Description"].ExportWidth = 400;
                        ASPxGridViewExporter1.Landscape = true;
                    }
                }
                else if (ddlReport.SelectedValue == "2")
                {
                    if (rblInclude.SelectedValue != "M" && rblInclude.SelectedValue != "F")
                    {
                        ASPxGridView1.Columns["Work Code"].ExportWidth = 100;
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 500;
                        ASPxGridView1.Columns["Manhours"].ExportWidth = 100;
                    }
                    else if (rblInclude.SelectedValue == "M")
                    {
                        ASPxGridView1.Columns["MS Code and Description"].ExportWidth = 210;
                        ASPxGridView1.Columns["MM Code and Description"].ExportWidth = 210;
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 180;
                    }
                    else if (rblInclude.SelectedValue == "F")
                    {
                        ASPxGridView1.Columns["Work Description"].ExportWidth = 300;
                        ASPxGridView1.Columns["FBS Code and Description"].ExportWidth = 300;
                    }
                }

                ASPxGridViewExporter1.WritePdfToResponse();
            }
            else
            {
                print = true;

                if (this.Panel1.Controls.Count > 1)
                {
                    btnGenerate_Click(sender, e);

                    try
                    {
                        Control[] ctrl;
                        if (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D")
                        {
                            ctrl = new Control[this.NumberOfControls + 2];
                        }
                        else
                        {
                            ctrl = new Control[this.NumberOfControls + 1];
                        }

                        if (chkDefaultReport.Checked)
                        {
                            ctrl[0] = CommonLookUp.HeaderPanelOptionERP(
                                this.VSDataTable.Columns.Count,
                                string.Format("Manhours Report Per {0}",
                                rblOption.SelectedItem.Text.Trim()),
                                initializeHeader(),
                                Panel1.Width);
                        }
                        else
                        {
                            ctrl[0] = CommonLookUp.HeaderPanelOptionERP(
                                this.VSDataTable.Columns.Count,
                                ddlReport.Items[Convert.ToInt32(hfSelectedRepIndex.Value)].Text,
                                initializeHeader(),
                                Panel1.Width);
                        }

                        int ctr = 1;
                        foreach (Control panelctrl in Panel1.Controls)
                        {
                            if (panelctrl is Label)
                            {
                                ctrl[ctr++] = panelctrl;
                            }
                            else if (panelctrl is GridView)
                            {
                                ctrl[ctr++] = panelctrl;
                            }
                            if (panelctrl.HasControls())
                            {
                                foreach (Control child in panelctrl.Controls)
                                {
                                    if (child is Label)
                                    {
                                        ctrl[ctr++] = child;
                                    }
                                    else if (child is GridView)
                                    {
                                        ((GridView)child).UseAccessibleHeader = true;
                                        ((GridView)child).HeaderRow.TableSection = TableRowSection.TableHeader;
                                        ctrl[ctr++] = child;
                                    }
                                    if (child.HasControls())
                                    {
                                        foreach (Control child2 in child.Controls)
                                        {
                                            if (child2 is Label)
                                            {
                                                ctrl[ctr++] = child2;
                                            }
                                            else if (child2 is GridView)
                                            {
                                                ((GridView)child2).UseAccessibleHeader = true;
                                                ((GridView)child2).HeaderRow.TableSection = TableRowSection.TableHeader;
                                                ctrl[ctr++] = child2;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        Session["ctrl"] = ctrl;
                        ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('../../Print.aspx','PrintMe');</script>");
                    }
                    catch
                    {
                        MessageBox.Show("Some error occurred during initialization of page for printing.\nPlease Try Again!");
                    }
                }
                else
                {
                    MessageBox.Show("No Records Found!");
                }
            }
        }
    }

    decimal total;
    protected void ASPxGridView1_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
    {
        if (e.IsTotalSummary)
        {
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                total = 0;
            }
            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                string tag = (e.Item as ASPxSummaryItem).Tag;

                if (e.GetValue("Category").ToString().Equals(tag))
                {
                    if (e.FieldValue != DBNull.Value)
                        total += Convert.ToDecimal(e.FieldValue);
                }
            }
            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                e.TotalValue = total;
            }
        }
    }

    protected void ASPxGridView1_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView1.DataSource = GetTable();
       
        using (DataTable dtTemp = (DataTable)ASPxGridView1.DataSource)
        {
            DataRow[] dr = null;

            if (dtTemp != null)
            {
                dr = dtTemp.Select();

                categoryList = new List<string>();

                for (int i = 0; i < dr.Length; i++)
                {
                    if (!categoryList.Contains(dr[i]["Category"].ToString()))
                    {
                        categoryList.Add(dr[i]["Category"].ToString());
                    }
                }

                categoryListCopy = new List<string>(categoryList);
                
            }
        }
    }

    protected void ASPxGridView1_SummaryDisplayText(object sender, ASPxGridViewSummaryDisplayTextEventArgs e)
    {
        if (e.Item.FieldName.Equals("Work Description") || e.Item.FieldName.Equals("Job Description"))
        {
            if (e.IsGroupSummary)
            {
                int rowLevel = ASPxGridView1.GetRowLevel(e.VisibleIndex);
                e.Text = ASPxGridView1.GetRowValues(e.VisibleIndex, ASPxGridView1.GetGroupedColumns()[rowLevel].ToString()) + " Total";
            }
            else if (e.IsTotalSummary)
            {
                if (categoryList == null && categoryListCopy != null)
                {
                    categoryList = new List<string>(categoryListCopy);
                }

                if (categoryList != null && categoryList.Count > 0)
                {
                    e.Text = "Total " + categoryList[0];
                    categoryList.RemoveAt(0);
                }
                else
                {
                    e.Text = "Grand Total";
                    categoryList = null;
                }
            }
        }
    }

    protected void grdView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        int j = 1;
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = j; i < e.Row.Cells.Count; i++)
            {
                TableCell tc = e.Row.Cells[i];
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.VerticalAlign = VerticalAlign.Top;
                //tc.Text = tc.Text.Insert(25, "mon");
            }
        }
    }

    protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //int j = 2;
        //if (ddlManhourType.SelectedValue == "")
        //    j++;
        //if (chkSubWorkCode.Checked)
        //    j++;

        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    //Andre: commented out this lines below to display only the dates not including DoW or Day Code

        //    //for (int i = j; i < e.Row.Cells.Count; i++)
        //    //{
        //    //    TableCell tc = e.Row.Cells[i];
        //    //    tc.VerticalAlign = VerticalAlign.Top;
        //    //    int temp = int.Parse(tc.Text.Substring(26, 3));
        //    //    string day = "   ";
        //    //    switch (temp)
        //    //    {
        //    //        case 1: day = "Sun"; break;
        //    //        case 2: day = "Mon"; break;
        //    //        case 3: day = "Tue"; break;
        //    //        case 4: day = "Wed"; break;
        //    //        case 5: day = "Thu"; break;
        //    //        case 6: day = "Fri"; break;
        //    //        case 7: day = "Sat"; break;
        //    //        default: break;
        //    //    }
        //    //    tc.Text = tc.Text.Replace(tc.Text.Substring(26, 3), day);
        //    //}
        //}
        //else if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    for (int i = j; i < e.Row.Cells.Count; i++)
        //    {
        //        TableCell tc = e.Row.Cells[i];
        //        tc.HorizontalAlign = HorizontalAlign.Right;
        //        try
        //        {
        //            decimal dec = 0;
        //            if (tc.Text == "&nbsp;")
        //                dec = 0;
        //            else
        //                dec = decimal.Parse(tc.Text);
        //            if (Totals.Contains(i))
        //                Totals[i] = (decimal)Totals[i] + dec;
        //            else
        //                Totals.Add(i, dec);
        //        }
        //        catch (Exception ex)
        //        {
        //            Response.Write(ex.ToString());
        //        }
        //    }
        //}
        //else if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    TableRow tr = e.Row;
        //    tr.Cells[0].Text = "TOTALS";
        //    for (int i = j; i < tr.Cells.Count; i++)
        //    {
        //        TableCell tc = tr.Cells[i];
        //        tc.HorizontalAlign = HorizontalAlign.Right;
        //        tc.Text = string.Format("{0:0.00}", decimal.Parse(Totals[i].ToString()));
        //    }
        //}

        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            TableCell tc = e.Row.Cells[i];

            if (!tc.Text.Equals("&nbsp;"))
                tc.Text = HttpUtility.HtmlDecode(tc.Text);
            if (i == 3)   //4th column
                tc.HorizontalAlign = HorizontalAlign.Right;
        }
    }

    protected void grdViewTotal_RowCreated(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TableCell tc = e.Row.Cells[2];
            tc.HorizontalAlign = HorizontalAlign.Right;
            tc.VerticalAlign = VerticalAlign.Top;
            e.Row.Cells[0].Wrap = false;
            tc.Attributes.Add("class", "Nums");
            if (!print)
            {
                tc.Width = Unit.Pixel(810);
            }
        }
    }

    protected void gridViewCDESR_RowDataBound(object sender, GridViewRowEventArgs e)    // Custom Daily Employee Summary Report
    {
        int j = 1;
        if (Convert.ToBoolean(hfTwist.Value))
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = j; i < e.Row.Cells.Count; i++)
                {
                    TableCell tc = e.Row.Cells[i];
                    try
                    {
                        DateTime dt = Convert.ToDateTime(tc.Text);
                        tc.Text = String.Format("{0:MM/dd/yyyy}", dt);
                    }
                    catch { }
                }
            }
        }
        else
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(e.Row.Cells[0].Text);
                    e.Row.Cells[0].Text = String.Format("{0:MM/dd/yyyy}", dt);
                }
                catch { }
            }
        }
    }
    #endregion

    #region Properties

    protected int NumberOfControls
    {
        get { return (int)ViewState["NumControls"]; }
        set { ViewState["NumControls"] = value; }
    }

    private DataTable VSDataTable
    {
        get
        {
            return (DataTable)ViewState["VSDataTable"];
        }
        set
        {
            ViewState["VSDataTable"] = value;
        }
    }

    #endregion

    #region Methods

    private void GenerateTables()
    {
        if (dsView.Tables.Count > 0)
        {
            this.NumberOfControls = 0;
            Panel1.Controls.Clear();
            ArrayList arr = new ArrayList();
            if (!chkSubWorkCode.Checked && rblOption.SelectedValue != "E")
            {
                try
                {
                    dsView.Tables[0].Columns.Remove("Work Activity Code");
                }
                catch
                {
                }
            }

            #region Default Report
            if (chkDefaultReport.Checked == true)
            {
                #region Per Employee
                if (rblOption.SelectedValue == "E")
                {
                    dsView.Tables[0].DefaultView.Sort = "CostCenterName";

                    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                    {
                        int ctr = 0;
                        if (!arr.Contains(dr["CostCenterName"]))
                        {
                            GenerateDynamicControls(dr, "CostCenterName", ctr, arr);
                        }
                    }
                }
                #endregion
                #region Per Department
                else if (rblOption.SelectedValue == "D")
                {
                    dsView.Tables[0].DefaultView.Sort = "CPH Job No.";
                    using (DataTable dtSource = dsView.Tables[0].Copy())
                    {
                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.Controls.Add(AddGridViewControl(dtSource));
                        div1.Attributes["style"] = "height:" + outerHeight + ";";
                        Panel1.Controls.Add(div1);
                    }
                }
                #endregion
                #region Per Client
                else if (rblOption.SelectedValue == "C")
                {
                    dsView.Tables[0].DefaultView.Sort = "Slm_ClientCode";
                    foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                    {
                        int ctr = 0;
                        if (!arr.Contains(dr["Slm_ClientCode"]))
                        {
                            GenerateDynamicControls(dr, "Slm_ClientCode", ctr, arr);
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Not Default Report
            else if (chkDefaultReport.Checked == false) // Default Report checkbox is not checked
            {
                if (dsView != null && dsView.Tables.Count > 0)
                {
                    using (DataTable temp1 = new DataTable(), dtFinal = new DataTable())
                    {
                        if (ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7" || ddlReport.SelectedValue == "14" || ddlReport.SelectedValue == "15")
                        {
                            using (DataTable dtSource = dsView.Tables[0])
                            {
                                HtmlGenericControl div1 = new HtmlGenericControl("div");
                                div1.Controls.Add(AddGridViewControl(dtSource));

                                if ((ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7") && outerHeight > 600)
                                {
                                    div1.Attributes["style"] = "height:600px;";
                                }
                                else
                                {
                                    div1.Attributes["style"] = "height:" + outerHeight + "px;";
                                }

                                Panel1.Controls.Add(div1);
                                return;
                            }
                        }
                        else if (ddlReport.SelectedValue == "4")
                        {
                            dsView.Tables[0].DefaultView.Sort = "Ecc_Description";
                            foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                            {
                                int ctr = 0;
                                if (!arr.Contains(dr["Ecc_Description"]))
                                {
                                    GenerateDynamicControls(dr, "Ecc_Description", ctr, arr);
                                }
                            }
                        }
                        else if (ddlReport.SelectedValue == "3")
                        {
                            dsView.Tables[0].DefaultView.Sort = "Ecc_Description";
                            foreach (DataRowView dr in dsView.Tables[0].DefaultView)
                            {
                                int ctr = 0;
                                if (!arr.Contains(dr["Ecc_Description"]))
                                {
                                    GenerateDynamicControls(dr, "Ecc_Description", ctr, arr);
                                }
                            }
                            using (GridView grdv = new GridView())
                            {
                                grdv.Attributes["runat"] = "server";
                                grdv.CellPadding = 0;
                                grdv.CellSpacing = 1;
                                grdv.ID = "grdv_GrandTotal";
                                grdv.Attributes["Width"] = Panel1.Width.ToString();
                                grdv.Attributes["border"] = "0";
                                using (DataTable dt = new DataTable())
                                {
                                    dt.Columns.Add(" ");
                                    dt.Columns.Add("  ");
                                    dt.Columns.Add("   ");
                                    dt.Rows.Add(dt.NewRow());
                                    dt.Rows[0][0] = "GRAND TOTAL";
                                    dt.Rows[0][1] = "";
                                    dt.Rows[0][2] = grandTotal.ToString();
                                    grdv.RowCreated += new GridViewRowEventHandler(grdViewTotal_RowCreated);
                                    grdv.DataSource = dt;
                                }
                                grdv.DataBind();
                                Panel1.Controls.Add(grdv);
                            }
                        }
                        else
                        {
                            if (ddlReport.SelectedValue == "16")
                            {
                                dtFinal.Columns.Add("Employee");
                                temp1.Columns.Add("Employee");
                                dtFinal.Columns.Add("Date");
                                temp1.Columns.Add("Date");
                            }

                            if (chkSubWorkCode.Checked)
                            {
                                dtFinal.Columns.Add("Work Code");
                                dtFinal.Columns.Add("Work Description");
                                temp1.Columns.Add("WorkCode");
                                temp1.Columns.Add("WorkDescription");
                            }
                            else
                            {
                                // add columns with empty names
                                dtFinal.Columns.Add("  "); // two spaces
                                temp1.Columns.Add("  ");
                                dtFinal.Columns.Add("   "); // three spaces
                                temp1.Columns.Add("   ");
                            }
                            dtFinal.Columns.Add("Man Hours");
                            temp1.Columns.Add("ManHours");

                            if (ddlReport.SelectedValue == "16")
                            {
                                dtFinal.Columns.Add("REG");
                                dtFinal.Columns.Add("OT");
                                dtFinal.Columns.Add("Total");
                                temp1.Columns.Add("REG");
                                temp1.Columns.Add("OT");
                                temp1.Columns.Add("Total");

                                dtFinal.Columns.Add("CYO Job No");
                                temp1.Columns.Add("CYOJobNo");
                                dtFinal.Columns.Add("CPH Job No");
                                temp1.Columns.Add("CPHJobNo");
                                dtFinal.Columns.Add("Department");
                                temp1.Columns.Add("Department");
                                dtFinal.Columns.Add("Category");
                                temp1.Columns.Add("Category");
                            }

                            if (chkSubWorkCode.Checked)
                            {
                                if (rblInclude.SelectedValue == "M")
                                {
                                    dtFinal.Columns.Add("MM Code");
                                    dtFinal.Columns.Add("MM Description");
                                    dtFinal.Columns.Add("MS Code");
                                    dtFinal.Columns.Add("MS Description");

                                    temp1.Columns.Add("MMCode");
                                    temp1.Columns.Add("MMDescription");
                                    temp1.Columns.Add("MSCode");
                                    temp1.Columns.Add("MSDescription");
                                }
                                else if (rblInclude.SelectedValue == "F")
                                {
                                    dtFinal.Columns.Add("FBS Code");
                                    dtFinal.Columns.Add("FBS Description");

                                    temp1.Columns.Add("FBSCode");
                                    temp1.Columns.Add("FBSDescription");
                                }
                            }
                        }
                        int tblIndex = 0;
                        int x = 0;

                        string currentEmpID;
                        string prevEmpID = string.Empty;
                        string currentBillable;
                        string previousBillable = string.Empty;
                        string currentJobCode;
                        string prevJobCode = string.Empty;

                        decimal[] EmpTotalHours = new decimal[2] { 0, 0 };
                        decimal[] BillableTotalHours = new decimal[2] { 0, 0 };
                        decimal[] JobCodeTotalHours = new decimal[2] { 0, 0 };
                        decimal[] GrandTotal = new decimal[2] { 0, 0 };

                        Dictionary<string, decimal> dictCategoryTotal = new Dictionary<string, decimal>();
                        Dictionary<string, decimal> dictCategoryTotalPerJob = new Dictionary<string, decimal>();

                        #region Custom Department Manhour
                        if (ddlReport.SelectedValue == "16")
                        {
                            for (int i = 0; i < dsView.Tables[0].Rows.Count; i++)
                            {
                                temp1.Rows.Clear();
                                tblIndex = 0;

                                currentJobCode = dsView.Tables[0].Rows[i]["Jsd_JobCode"].ToString();
                                currentBillable = dsView.Tables[0].Rows[i]["Jsd_Category"].ToString();
                                currentEmpID = dsView.Tables[0].Rows[i]["Jsh_EmployeeID"].ToString();

                                if (i > 0)
                                {
                                    if (!currentEmpID.Equals(prevEmpID) || !currentJobCode.Equals(prevJobCode) || currentBillable != previousBillable)
                                    {
                                        temp1.Rows.Add(temp1.NewRow());
                                        temp1.Rows.Add(temp1.NewRow());
                                        tblIndex++;
                                        temp1.Rows[tblIndex][1] = "<b>Sub Total For Employee</b>";
                                        temp1.Rows[tblIndex][2] = "<b>" + prevEmpID + "</b>";
                                        SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                                        EmpTotalHours = new decimal[2] { 0, 0 };
                                        tblIndex++;
                                    }

                                    if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                                    {
                                        temp1.Rows.Add(temp1.NewRow());
                                        temp1.Rows.Add(temp1.NewRow());
                                        tblIndex++;
                                        temp1.Rows[tblIndex][1] = "<b>Total</b>";
                                        temp1.Rows[tblIndex][2] = "<b>" + previousBillable + "</b>";
                                        SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                        if (dictCategoryTotalPerJob.ContainsKey(previousBillable))
                                        {
                                            dictCategoryTotalPerJob[previousBillable] += BillableTotalHours[1];
                                        }
                                        else
                                        {
                                            dictCategoryTotalPerJob.Add(previousBillable, BillableTotalHours[1]);
                                        }
                                        BillableTotalHours = new decimal[2] { 0, 0 };
                                        tblIndex++;
                                    }

                                    if (!currentJobCode.Equals(prevJobCode))
                                    {
                                        temp1.Rows.Add(temp1.NewRow());
                                        temp1.Rows.Add(temp1.NewRow());
                                        tblIndex++;
                                        foreach (String category in dictCategoryTotalPerJob.Keys)
                                        {
                                            temp1.Rows[tblIndex][1] = "<b>Sub Total Job No. " + prevJobCode + " " + category + "</b>";
                                            temp1.Rows[tblIndex][7] = "<b>" + dictCategoryTotalPerJob[category] + "</b>";
                                            temp1.Rows.Add(temp1.NewRow());
                                            tblIndex++;

                                            if (dictCategoryTotal.ContainsKey(category))
                                            {
                                                dictCategoryTotal[category] += dictCategoryTotalPerJob[category];
                                            }
                                            else
                                            {
                                                dictCategoryTotal.Add(category, dictCategoryTotalPerJob[category]);
                                            }
                                        }

                                        temp1.Rows[tblIndex][1] = "<b>Total for Job No. " + prevJobCode + "</b>";
                                        SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                        tblIndex += 2;
                                        temp1.Rows.Add(temp1.NewRow());

                                        JobCodeTotalHours = new decimal[2] { 0, 0 };
                                        dictCategoryTotalPerJob.Clear();
                                    }
                                }

                                if (!currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows[tblIndex][0] = "<b>" + currentJobCode + "</b>";
                                    temp1.Rows[tblIndex][1] = "<b>" + dsView.Tables[0].Rows[i]["Jcm_JobDescription"].ToString() + "</b>";
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex += 2;
                                }

                                if (currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows[tblIndex][0] = "<b>" + currentBillable + "</b>";
                                    tblIndex++;
                                }

                                if (!currentEmpID.Equals(prevEmpID) || currentBillable != previousBillable || !currentJobCode.Equals(prevJobCode))
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows[tblIndex][0] = "<b>" + currentEmpID + "</b>";
                                    temp1.Rows[tblIndex][2] = "<b>" + dsView.Tables[0].Rows[i]["Name"].ToString() + "</b>";
                                    tblIndex++;
                                    prevEmpID = currentEmpID;
                                }

                                temp1.Rows.Add(temp1.NewRow());
                                SetTableSummary(dsView, temp1, i, tblIndex, chkSubWorkCode.Checked);
                                tblIndex++;

                                sumValues(EmpTotalHours, dsView, i);
                                sumValues(BillableTotalHours, dsView, i);
                                sumValues(JobCodeTotalHours, dsView, i);
                                sumValues(GrandTotal, dsView, i);

                                if (!currentJobCode.Equals(prevJobCode))
                                    prevJobCode = currentJobCode;
                                if (currentBillable != previousBillable)
                                    previousBillable = currentBillable;

                                if (i == dsView.Tables[0].Rows.Count - 1)
                                {
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "<b>Sub Total For Employee</b>";
                                    temp1.Rows[tblIndex][2] = "<b>" + currentEmpID + "</b>";
                                    SetTableSummaryTotals(EmpTotalHours, temp1, tblIndex);
                                    tblIndex++;

                                    // for total of billable or non-billable if no more rows after this 
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    temp1.Rows[tblIndex][1] = "<b>Total</b>";
                                    temp1.Rows[tblIndex][2] = "<b>" + currentBillable + "</b>";
                                    SetTableSummaryTotals(BillableTotalHours, temp1, tblIndex);
                                    if (dictCategoryTotalPerJob.ContainsKey(previousBillable))
                                    {
                                        dictCategoryTotalPerJob[previousBillable] += BillableTotalHours[1];
                                    }
                                    else
                                    {
                                        dictCategoryTotalPerJob.Add(previousBillable, BillableTotalHours[1]);
                                    }
                                    tblIndex++;

                                    // for total of job code 
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows.Add(temp1.NewRow());
                                    tblIndex++;
                                    foreach (String category in dictCategoryTotalPerJob.Keys)
                                    {
                                        temp1.Rows[tblIndex][1] = "<b>Sub Total Job No. " + currentJobCode + " " + category + "</b>";
                                        temp1.Rows[tblIndex][7] = "<b>" + dictCategoryTotalPerJob[category] + "</b>";
                                        temp1.Rows.Add(temp1.NewRow());
                                        tblIndex++;

                                        if (dictCategoryTotal.ContainsKey(category))
                                        {
                                            dictCategoryTotal[category] += dictCategoryTotalPerJob[category];
                                        }
                                        else
                                        {
                                            dictCategoryTotal.Add(category, dictCategoryTotalPerJob[category]);
                                        }
                                    }
                                    temp1.Rows[tblIndex][1] = "<b>Total for Job No. " + currentJobCode + "</b>";
                                    SetTableSummaryTotals(JobCodeTotalHours, temp1, tblIndex);
                                    tblIndex++;

                                    // for total per category
                                    temp1.Rows.Add(temp1.NewRow());
                                    foreach (KeyValuePair<string, decimal> kvp in dictCategoryTotal)
                                    {
                                        temp1.Rows.Add(temp1.NewRow());
                                        tblIndex++;
                                        temp1.Rows[tblIndex][1] = "<b>Total</b>";
                                        temp1.Rows[tblIndex][2] = "<b>" + kvp.Key + "</b>";
                                        SetTableSummaryTotals(new decimal[] { 0, kvp.Value }, temp1, tblIndex);
                                    }
                                    tblIndex++;

                                    // for Grand Total
                                    temp1.Rows.Add(temp1.NewRow());
                                    temp1.Rows[tblIndex][1] = "<b>Grand Total:</b>";
                                    SetTableSummaryTotals(GrandTotal, temp1, tblIndex);
                                }

                                x = dtFinal.Rows.Count;
                                for (int z = 0; z < temp1.Rows.Count; z++)
                                {
                                    dtFinal.Rows.Add(dtFinal.NewRow());
                                    dtFinal.Rows[x + z]["Employee"] = temp1.Rows[z]["Employee"];
                                    dtFinal.Rows[x + z]["Date"] = temp1.Rows[z]["Date"];
                                    //dtFinal.Rows[x + z]["Man Hours"] = temp1.Rows[z]["ManHours"];
                                    dtFinal.Rows[x + z]["REG"] = temp1.Rows[z]["REG"];
                                    dtFinal.Rows[x + z]["OT"] = temp1.Rows[z]["OT"];
                                    dtFinal.Rows[x + z]["Total"] = temp1.Rows[z]["Total"];
                                    dtFinal.Rows[x + z]["CYO Job No"] = temp1.Rows[z]["CYOJobNo"];
                                    dtFinal.Rows[x + z]["CPH Job No"] = temp1.Rows[z]["CPHJobNo"];
                                    dtFinal.Rows[x + z]["Department"] = temp1.Rows[z]["Department"];
                                    dtFinal.Rows[x + z]["Category"] = temp1.Rows[z]["Category"];
                                    if (chkSubWorkCode.Checked)
                                    {
                                        dtFinal.Rows[x + z]["Work Code"] = temp1.Rows[z]["WorkCode"];
                                        dtFinal.Rows[x + z]["Work Description"] = temp1.Rows[z]["WorkDescription"];

                                        if (rblInclude.SelectedValue == "M")
                                        {
                                            dtFinal.Rows[x + z]["MM Code"] = temp1.Rows[z]["MMCode"];
                                            dtFinal.Rows[x + z]["MM Description"] = temp1.Rows[z]["MMDescription"];
                                            dtFinal.Rows[x + z]["MS Code"] = temp1.Rows[z]["MSCode"];
                                            dtFinal.Rows[x + z]["MS Description"] = temp1.Rows[z]["MSDescription"];
                                        }
                                        if (rblInclude.SelectedValue == "F")
                                        {
                                            dtFinal.Rows[x + z]["FBS Code"] = temp1.Rows[z]["FBSCode"];
                                            dtFinal.Rows[x + z]["FBS Description"] = temp1.Rows[z]["FBSDescription"];
                                        }
                                    }
                                    else
                                    {
                                        dtFinal.Rows[x + z]["  "] = temp1.Rows[z]["  "];
                                        dtFinal.Rows[x + z]["   "] = temp1.Rows[z]["   "];
                                    }
                                }
                            }
                            dtFinal.Columns.Remove("Man Hours");
                        }
                        #endregion

                        if (ddlReport.SelectedValue != "4" && ddlReport.SelectedValue != "3")
                        {
                            using (DataTable dtSource = dtFinal.Copy())
                            {
                                if (dtSource.Rows.Count > 0)
                                {
                                    HtmlGenericControl div1 = new HtmlGenericControl("div");
                                    div1.Controls.Add(AddGridViewControl(dtSource));
                                    if (ddlReport.SelectedValue == "16")
                                    {
                                        div1.Attributes["style"] = "height:" + 600 + "px;";
                                    }

                                    Panel1.Controls.Add(div1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    VSDataTable = null;
                    grdView.DataSource = null;
                    grdView.DataBind();
                }
            }
            #endregion
        }
        else
        {
            Panel1.Controls.Clear();
        }
    }

    protected void GetData()
    {
        string sqlFetch = SqlBuilder();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dsView = dal.ExecuteDataSet(sqlFetch, CommandType.Text);
            }
            catch (Exception e)
            {
                //Response.Write(e.ToString());
                dsView = null;
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }

    private void getWeedStartandEnd()
    {
        string[] SandE = new string[3];
        string sql = @"
                    SELECT Convert(varchar(2),(ccd_weekcoverage - 1)) + '-' + Convert(varchar(2),(ccd_weekcoverage - 2))
                    from t_companymaster
                    ";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                SandE[2] = Convert.ToString(dal.ExecuteScalar(sql, CommandType.Text));
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        SandE = SandE[2].Split('-');

        hWeekStart.Value = SandE[0];
        hWeekEnd.Value = SandE[1];
    }

    private void InitializeButtons()
    {
        hfUserCostCenters.Value = "";
        using (DataTable dt = CommonLookUp.GetUserCostCenterCode(Session["userId"].ToString(), "TIMEKEEP"))
        {
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    hfUserCostCenters.Value += string.Format("x{0}xy", dr[0]);
                }
                btnDashJobCode.OnClientClick = string.Format("return OpenPopupLookupSales('S1','txtDashJobCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                btnClientJobNo.OnClientClick = string.Format("return OpenPopupLookupSales('S2','txtClientJobNo','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                btnCLientCode.OnClientClick = string.Format("return OpenPopupLookupSales('S3','txtClientCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                btnClientFWBS.OnClientClick = string.Format("return OpenPopupLookupSales('S4','txtFWBSCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                btnDashWorkCode.OnClientClick = string.Format("return OpenPopupLookupSales('S6','txtWorkCode','{0}'); return true;", hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1));
                btnCostCenter.OnClientClick = string.Format("return lookupRJSCostcenterMR('{0}')", cbUnsplitted.Visible);
                btnEmployee.OnClientClick = string.Format("return lookupRJSEmployeeMR('{0}')", cbUnsplitted.Visible);
                btnSubWork.OnClientClick = string.Format("javascript:return lookupRJSSubwork()");
            }
            else
            {
                hfUserCostCenters.Value = "xNo Cost Center Accessxy";
            }
        }
    }

    private void initializeControls()
    {
        dtpRangeFrom.Date = CommonMethods.getQuincenaDate('C', "START");
        dtpRangeFrom.MinDate = CommonMethods.getMinimumDate();
        dtpRangeFrom.MaxDate = CommonMethods.getQuincenaDate('F', "END");
        dtpRangeTo.Date = CommonMethods.getQuincenaDate('C', "END");
        dtpRangeTo.MinDate = CommonMethods.getMinimumDate();
        dtpRangeTo.MaxDate = CommonMethods.getQuincenaDate('F', "END");

        InitializeButtons();
        initializeProfile();
        PopulateDdlBilling();
    }

    private string initializeHeader()
    {
        string options = "";
        if (hfUserCostCenters.Value != "")
            options += "Cost Center Access: " + hfUserCostCenters.Value.Substring(0, hfUserCostCenters.Value.Length - 1).Replace("x", "'").Replace("y", ",") + "; ";
        if (txtDashJobCode.Text != "")
            options += "CPH Job No.(s): " + txtDashJobCode.Text.Trim() + "; ";
        if (txtClientJobNo.Text != "")
            options += "Client Job Code(s): " + txtClientJobNo.Text.Trim() + "; ";
        if (txtClientCode.Text != "")
            options += "Client Code(s): " + txtClientCode.Text.Trim() + "; ";
        if (txtFWBSCode.Text != "")
            options += "FWBS Code(s): " + txtFWBSCode.Text.Trim() + "; ";
        if (txtCostCenter.Text != "")
            options += "Cost Center(s): " + txtCostCenter.Text.Trim() + "; ";
        if (rblBillable.SelectedValue != "A")
            options += "Category: " + rblBillable.SelectedItem.Text.Trim() + "; ";

        if (!dtpRangeTo.IsNull && !dtpRangeFrom.IsNull)
        {
            if (dtpRangeTo.Date == dtpRangeFrom.Date)
                options += "Interval Date: " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + "; ";
            else
                options += "Interval Date: " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + " - " + dtpRangeTo.Date.ToString("MM/dd/yyyy") + "; ";
        }
        else if (dtpRangeTo.IsNull && !dtpRangeFrom.IsNull)
            options += "Interval Date: From " + dtpRangeFrom.Date.ToString("MM/dd/yyyy") + "; ";
        else if (!dtpRangeTo.IsNull && dtpRangeFrom.IsNull)
            options += "Interval Date: To " + dtpRangeTo.Date.ToString("MM/dd/yyyy") + "; ";

        return options.Trim();
    }

    private void initializeProfile()
    {
        using (DataTable dt = CommonMethods.GetProfileAccess(Session["userLogged"].ToString()))
        {
            foreach (DataRow drProfile in dt.Rows)
            {
                ListItem li = new ListItem(drProfile["Prf_Profile"].ToString(), drProfile["Prf_Database"].ToString());
                li.Selected = true;
                cblProfile.Items.Add(li);
            }
        }
    }

    private bool isProfileSelected()
    {
        foreach (ListItem li in cblProfile.Items)
        {
            if (li.Selected)
            {
                return true;
            }
        }

        return false;
    }

    #region para unta sa BL sa manhours
    private void PopulateDdlBilling()
    {
        DataTable dt = new DataTable();
        string sqlGetStatus = @"  SELECT Adt_AccountDesc as [Description]
                                       , Adt_AccountCode as [Value]
                                    FROM T_AccountDetail
                                   WHERE Adt_AccountType = 'BILLCYCLE'
                                     AND Adt_Status = 'A'";

        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlGetStatus, CommandType.Text).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dal.CloseDB();
                ddlBilling.Items.Clear();
                ddlBilling.Items.Add(new ListItem("", ""));
            }
        }
        foreach (DataRow dr in dt.Rows)
        {
            ddlBilling.Items.Add(new ListItem(dr["Description"].ToString(), dr["Value"].ToString()));
        }
    }
    #endregion

    private string SetFilters()
    {
        StringBuilder filters = new StringBuilder();

        #region Cost Center Access
        if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
        {
            string temp = hfUserCostCenters.Value;
            temp = temp.Replace("x", "''").Replace("y", ",");
            filters.Append("\nAND Jsd_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
        }
        #endregion

        #region Textboxes

        if (txtDashJobCode.Text != "")
        {
            string text = txtDashJobCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string dashJobCode = "";
                for (int i = 0; i < arr.Count; i++)
                    dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(dashJobCode + ")");
            }
        }

        if (txtClientJobNo.Text != "")
        {
            string text = txtClientJobNo.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string clientJobNo = "";
                for (int i = 0; i < arr.Count; i++)
                    clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(clientJobNo + ")");
            }
        }
        if (txtClientCode.Text != "")
        {
            string text = txtClientCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);

            if (arr.Count > 0)
            {
                string clientCode = "", clientName = "", clientShortName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
                    clientName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                    clientShortName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                }
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(clientCode);
                filters.Append(clientName);
                filters.Append(clientShortName + ")");
            }
        }

        if (txtEmpName.Text != string.Empty)
        {
            string text = txtEmpName.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string employeeId = "", employeeName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    employeeId += string.Format("\nOR Q.Emt_EmployeeId like ''{0}%''", arr[i].ToString().Trim());
                    employeeName += string.Format("\nOR Q.Emt_LastName like ''{0}%'' OR Q.Emt_FirstName like ''{0}%''", arr[i].ToString().Trim());
                }
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(employeeId);
                filters.Append(employeeName + ")");
            }
        }

        if (txtCostCenter.Text != "")
        {
            string text = txtCostCenter.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string costCenterCode = "", costCenterName = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    costCenterCode += string.Format("\nOR Jsd_Costcenter like ''{0}%''", arr[i].ToString().Trim());
                    costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsd_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
                }
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(costCenterCode);
                filters.Append(costCenterName + ")");
            }
        }

        if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
        {
            string text = txtSubWorkCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string SubWorkCode = "";
                for (int i = 0; i < arr.Count; i++)
                    SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(SubWorkCode + ")");
            }
        }

        if (txtFWBSCode.Text != "")
        {
            string text = txtFWBSCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string FWBSCode = "";
                for (int i = 0; i < arr.Count; i++)
                    FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like ''{0}%''", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(FWBSCode + ")");
            }
        }

        if (txtWorkCode.Text != "")
        {
            string text = txtWorkCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string dashWorkCode = "";
                for (int i = 0; i < arr.Count; i++)
                    dashWorkCode += string.Format("\nOR Slm_DashWorkCode like ''{0}%''", arr[i].ToString().Trim());
                filters.Append("\nAND (");
                filters.Append("\n 1 = 0");
                filters.Append(dashWorkCode + ")");
            }
        }

        #endregion

        if (rblBillable.SelectedValue != "A")
        {
            if (rblBillable.SelectedValue.Equals("X"))
                filters.Append(string.Format("\nAND Jsd_Category IN (''D'',''T'', ''X'')", rblBillable.SelectedValue));
            else
                filters.Append(string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue));
        }

        if (rblOption.SelectedValue == "C")
        {
            if (!ddlBilling.SelectedValue.Equals(string.Empty))
            {
                filters.Append(string.Format("\nAND Clh_BillingCycle = ''{0}''", ddlBilling.SelectedValue));
            }
        }

        #region Date Time Pickers
        if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
        {
            if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                 , Convert.ToDateTime(dtpRangeTo.Date)));
            }
            else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date)));
            }
            else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
            {
                filters.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date)));
            }
        }
        #endregion

        return filters.ToString();
    }

    private void setOptions()
    {
        switch (ddlReport.SelectedValue)
        {
            case "1":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "2":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "C";
                break;
            case "3":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "D";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "4":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "D";
                rbEmployeeOptions.SelectedValue = "S";
                break;
            case "5":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "D";
                break;
            case "6":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "W";
                break;
            case "7":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                ddlDateType.SelectedValue = "M";
                break;
            case "8":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            case "9":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "D";
                break;
            case "10":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "D";
                break;
            case "11":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "W";
                break;
            case "12":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "C";
                ddlDateType.SelectedValue = "M";
                break;
            case "13":
                chkDefaultReport.Checked = true;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "S";
                break;
            case "14":
                chkDefaultReport.Checked = false;
                break;
            case "15":
                chkDefaultReport.Checked = false;
                break;
            case "16":
                chkDefaultReport.Checked = false;
                rblOption.SelectedValue = "E";
                rbEmployeeOptions.SelectedValue = "D";
                break;
            default:
                break;
        }
    }

    private string SetQuery()
    {
        string sqlQuery = "";

        if (rblOption.SelectedValue == "E")
        {
            if (rbEmployeeOptions.SelectedValue == "S")
            {
                sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = '";

                sqlQuery += @"SELECT Ecc_Description [CostCenterName]
		                            ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                            , Jsd_ActHours
                            FROM T_JobSplitdetail
                            INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                            and  Jsh_Status = ''9''
                            INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                    AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                            LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                            INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                            LEFT JOIN {0}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter  
                            WHERE Jsh_Status = ''9''";
            }
            else
            {
                if (chkSubWorkCode.Checked)
                {
                    sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = 'SELECT Ecc_Description [CostCenterName]
		                                ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                , Slm_ClientJobName [Client Job Name]
		                                , Slm_DashJobCode [CPH Job No.]
		                                , Jsd_SubWorkCode [Work Activity Code]
		                                , Jsd_ActHours
                                FROM T_JobSplitdetail
                                INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                                and  Jsh_Status = ''9''
                                INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                                LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                LEFT JOIN {0}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter  
                                WHERE Jsh_Status = ''9''";
                }
                else
                {
                    sqlQuery = @"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(max)

                SET @SelectList = 'SELECT Ecc_Description [CostCenterName]
		                                    ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                , Slm_ClientJobName [Client Job Name]
		                                , Slm_DashJobCode [CPH Job No.]
		                                , Jsd_ActHours
                                FROM T_JobSplitdetail
                                INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                                and  Jsh_Status = ''9''
                                INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                                LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                LEFT JOIN {0}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter
                                WHERE Jsh_Status = ''9''";
                }
            }
            sqlQuery = string.Format(sqlQuery, ConfigurationManager.AppSettings["ERP_DB"]);
        }
        else if (rblOption.SelectedValue == "D")
        {
            sqlQuery = @"   declare @SelectList varchar(max)
                        declare @PivotCol varchar(200)
                        declare @Summaries varchar(200)

                        SET @SelectList = 'SELECT 
	                                Jsd_Jobcode [CPH Job No.]
	                            , Slm_ClientJobNo [Client Job No]
	                            , Slm_ClientJobName [Client Job Name]";

            if (chkSubWorkCode.Checked)
                sqlQuery += @"      , Jsd_SubWorkCode [Work Activity Code]";

            sqlQuery += @"          , Jsd_ActHours
                        FROM T_JobSplitdetail
                        INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
                            and  Jsh_Status = ''9''
                        INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                        WHERE Jsh_Status = ''9''";

        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
        {
            sqlQuery = @"declare @SelectList varchar(max)
                    declare @PivotCol varchar(200)
                    declare @Summaries varchar(100)

                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
	                            {0}
                                , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
                        and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                        and Jsd_ClientJobNo = Slm_ClientJobNo
                            AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                
                                , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                , Slm_ClientJobName [Client Job Name]
            //                                , Slm_ClientJobNo [Client Job No]
            //                                , Jsd_Jobcode [CPH Job No.]";
            //                if (chkSubWorkCode.Checked)
            //                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
        {
            sqlQuery = @"declare @SelectList varchar(max)
                    declare @PivotCol varchar(300)
                    declare @Summaries varchar(max)
                    declare @StartDay tinyint
                    set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                    set Datefirst @StartDay
                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                    {0}
		                        , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                    and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                    and Jsd_ClientJobNo = Slm_ClientJobNo
                            AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                , Slm_ClientJobName [Client Job Name]
            //                                , Slm_ClientJobNo [Client Job No]
            //                                , Jsd_Jobcode [CPH Job No.]";
            //                if (chkSubWorkCode.Checked)
            //                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
        {
            sqlQuery = @"
                    declare @SelectList varchar(max)
                    declare @PivotCol varchar(300)
                    declare @Summaries varchar(max)
                    declare @StartDay tinyint
                    set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                    set Datefirst @StartDay
                    SET @SelectList = 'SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                    {0}
		                    , Jsd_ActHours
                    FROM T_JobSplitdetail
                    INNER JOIN T_JobSplitheader on Jsd_ControlNo = Jsh_ControlNo
	                    and  Jsh_Status = ''9''
                    INNER JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                    and Jsd_ClientJobNo = Slm_ClientJobNo
                            AND (Jsd_CostCenter = Slm_CostCenter OR Slm_Costcenter = ''ALL'')
                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                    LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                    LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                , Slm_ClientJobNo [Client Job No]
                                , Slm_ClientJobName [Client Job Name]
                                , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                , Slm_ClientJobName [Client Job Name]
            //                                , Slm_ClientJobNo [Client Job No]
            //                                , Jsd_Jobcode [CPH Job No.]";
            //                if (chkSubWorkCode.Checked)
            //                    insert += @", Jsd_SubWorkCode [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }

        return sqlQuery;
    }

    private string SetQueryLossTime()
    {
        string sqlQuery = "";
        if (rblOption.SelectedValue == "E")
        {
            if (rbEmployeeOptions.SelectedValue == "S")
            {
                sqlQuery = @"SELECT Ecc_Description [CostCenterName]
		                                     ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                    , Jsd_ActHours
                                    FROM T_JobSplitDetailLossTime
                                    INNER JOIN T_JobSplitHeaderLossTIme on Jsd_ControlNo = Jsh_ControlNo
	                                    and  Jsh_Status = ''9''
                                    LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                    and Jsd_ClientJobNo = Slm_ClientJobNo
                                            --AND Jsd_CostCenter = Slm_CostCenter
                                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                    LEFT JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  
                                    WHERE Jsh_Status = ''9''";
            }
            else if (rbEmployeeOptions.SelectedValue == "D")
            {
                if (chkSubWorkCode.Checked)
                    sqlQuery = @"SELECT Ecc_Description [CostCenterName]
		                                         ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                        , Slm_ClientJobName
		                                        , Jsd_JobCode
		                                        
                                                , Jsd_SubWorkCode
                                                , Jsd_ActHours
                                        FROM T_JobSplitdetaillosstime
                                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                                        and  Jsh_Status = ''9''
                                        LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                                --AND Jsd_CostCenter = Slm_CostCenter   
                                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        LEFT JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  
                                        WHERE Jsh_Status = ''9''";
                else
                    sqlQuery = @"SELECT Ecc_Description [CostCenterName]
		                                     ,Emt_LastName + '', '' +Emt_FirstName [Name]
		                                    , Slm_ClientJobName
		                                    , Jsd_JobCode
		                                  
		                                    , Jsd_ActHours
                                        FROM T_JobSplitdetaillosstime
                                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                                        and  Jsh_Status = ''9''
                                        LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                                --AND Jsd_CostCenter = Slm_CostCenter
                                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                                        LEFT JOIN {0}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter 
                                        WHERE Jsh_Status = ''9''";
            }
        }
        else if (rblOption.SelectedValue == "D")
        {
            sqlQuery = @" SELECT 
	                                  Jsd_Jobcode [CPH Job No.]
	                                , Jsd_ClientJobNo [Client Job No]
	                                , Slm_ClientJobName [Client Job Name]";
            if (chkSubWorkCode.Checked)
                sqlQuery += @",  ''TEMP'' as [Work Activity Code]";

            sqlQuery += @"          , Jsd_ActHours
                            FROM T_JobSplitdetaillosstime
                            INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
                                and  Jsh_Status = ''9''
                            LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                                and Jsd_ClientJobNo = Slm_ClientJobNo
                                    --AND Jsd_CostCenter = Slm_CostCenter
                            LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                            INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                            WHERE Jsh_Status = ''9''";

        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
	                             {0}
                                 , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
                            and  Jsh_Status = ''9''
                        LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
	                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                --AND Jsd_CostCenter = Slm_CostCenter
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                 , Jsd_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                
                                 , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                 , Slm_ClientJobName [Client Job Name]
            //                                 , Slm_ClientJobNo [Client Job No]
            //                                 , Jsd_Jobcode [CPH Job No.]";
            //                if (chkSubWorkCode.Checked)
            //                    insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                        {0}
		                          , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                        and  Jsh_Status = ''9''
                        LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                --AND Jsd_CostCenter = Slm_CostCenter
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                 , Jsd_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                
                                 , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                 , Slm_ClientJobName [Client Job Name]
            //                                 , Slm_ClientJobNo [Client Job No]
            //                                 , Jsd_Jobcode [CPH Job No.]";
            //                if(chkSubWorkCode.Checked)
            //                                 insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
        {
            sqlQuery = @"SELECT RTRIM(Slm_ClientCode) + '' ''+  RTRIM(Clh_ClientName) [Slm_ClientCode]
		                        {0}
		                        , Jsd_ActHours
                        FROM T_JobSplitdetaillosstime
                        INNER JOIN T_JobSplitheaderLossTime on Jsd_ControlNo = Jsh_ControlNo
	                        and  Jsh_Status = ''9''
                        LEFT JOIN T_SalesMaster on Jsd_Jobcode = Slm_DashJobCode
		                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                --AND Jsd_CostCenter = Slm_CostCenter
                        LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                        INNER JOIN T_EmployeeMaster on Jsh_EmployeeId = Emt_EmployeeID
                        LEFT JOIN T_CostCenter on   Cct_CostCenterCode = Jsh_Costcenter
                        LEFT JOIN T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                        LEFT JOIN T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                        LEFT JOIN T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                        LEFT JOIN T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                        LEFT JOIN T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                        WHERE Jsh_Status = ''9''";
            string insert = string.Empty;
            //if (rblCode.SelectedValue == "J")
            //{
            insert = @"
                                 , Jsd_ClientJobNo [Client Job No]
                                 , Slm_ClientJobName [Client Job Name]
                                 
                                 , Jsd_Jobcode [CPH Job No.]";
            if (chkSubWorkCode.Checked)
                insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            //            else
            //            {
            //                insert = @"
            //                                 , Slm_ClientJobName [Client Job Name]
            //                                 , Slm_ClientJobNo [Client Job No]
            //                                 , Jsd_Jobcode [CPH Job No.]";
            //                if(chkSubWorkCode.Checked)
            //                                 insert += @",  ''TEMP'' as [Work Activity Code]";
            //            }
            sqlQuery = string.Format(sqlQuery, insert);
        }
        return string.Format(sqlQuery, ConfigurationManager.AppSettings["ERP_DB"]);
    }

    protected void SetTableSummary(DataSet ds, DataTable temp, int dsIndex, int dtIndex, bool includeSubWork)
    {
        if ((rbEmployeeOptions.SelectedValue == "S" && rblOption.SelectedValue == "D")      // Department and Summary
            || (rblOption.SelectedValue == "D" && rbEmployeeOptions.SelectedValue == "D"))  // Department and Detail
        {
            temp.Rows[dtIndex][0] = ds.Tables[0].Rows[dsIndex]["Jsd_JobCode"].ToString();
            temp.Rows[dtIndex][1] = ds.Tables[0].Rows[dsIndex]["Jcm_JobDescription"].ToString();
        }
        else
        {
            if (includeSubWork)
            {
                temp.Rows[dtIndex]["WorkCode"] = ds.Tables[0].Rows[dsIndex]["Subwork Code"].ToString();
                temp.Rows[dtIndex]["WorkDescription"] = ds.Tables[0].Rows[dsIndex]["Work Description"].ToString();

                if (rblInclude.SelectedValue == "M")
                {
                    temp.Rows[dtIndex]["MMCode"] = ds.Tables[0].Rows[dsIndex]["Swc_MMCode"].ToString();
                    temp.Rows[dtIndex]["MMDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_MMCodeDesc"].ToString();
                    temp.Rows[dtIndex]["MSCode"] = ds.Tables[0].Rows[dsIndex]["Swc_MSCode"].ToString();
                    temp.Rows[dtIndex]["MSDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_MSCodeDesc"].ToString();
                }
                if (rblInclude.SelectedValue == "F")
                {
                    temp.Rows[dtIndex]["FBSCode"] = ds.Tables[0].Rows[dsIndex]["Swc_FBSCode"].ToString();
                    temp.Rows[dtIndex]["FBSDescription"] = ds.Tables[0].Rows[dsIndex]["Swc_FBSCodeDesc"].ToString();
                }
            }
            if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
            {
                temp.Rows[dtIndex]["Employee"] = ds.Tables[0].Rows[dsIndex]["Jsh_EmployeeID"].ToString();
                temp.Rows[dtIndex]["Date"] = ds.Tables[0].Rows[dsIndex]["SPLIT DATE"].ToString();
                temp.Rows[dtIndex]["CYOJobNo"] = ds.Tables[0].Rows[dsIndex]["Jsd_ClientJobNo"].ToString();
                temp.Rows[dtIndex]["CPHJobNo"] = ds.Tables[0].Rows[dsIndex]["Jsd_JobCode"].ToString();
                temp.Rows[dtIndex]["REG"] = ds.Tables[0].Rows[dsIndex]["REG"].ToString();
                temp.Rows[dtIndex]["OT"] = ds.Tables[0].Rows[dsIndex]["OT"].ToString();
                temp.Rows[dtIndex]["Total"] = ds.Tables[0].Rows[dsIndex]["Total"].ToString();
                temp.Rows[dtIndex]["Department"] = ds.Tables[0].Rows[dsIndex]["Dcm_DepartmentDesc"].ToString();

                string category = ds.Tables[0].Rows[dsIndex]["Jsd_Category"].ToString();
                if (category.Contains("(B)"))
                {
                    category = category.Remove(category.LastIndexOf("(B)")).Trim();
                }
                else if (category.Contains("(N)"))
                {
                    category = category.Remove(category.LastIndexOf("(N)")).Trim();
                }
                temp.Rows[dtIndex]["Category"] = category;
            }
        }

        temp.Rows[dtIndex]["ManHours"] = ds.Tables[0].Rows[dsIndex]["Jsd_Acthours"].ToString();


    }

    protected void SetTableSummaryTotals(decimal[] totals, DataTable temp, int dtIndex)
    {
        if (rblOption.SelectedValue == "E" && rbEmployeeOptions.SelectedValue == "D")
            temp.Rows[dtIndex][7] = "<b>" + totals[1].ToString() + "</b>";
        else
            temp.Rows[dtIndex][3] = "<b>" + totals[1].ToString() + "</b>";
    }

    protected void SetTableSummaryTotals(decimal[] totals, DataTable temp, int dtIndex, int columnForTotal)
    {
        temp.Rows[dtIndex][columnForTotal] = "" + totals[1].ToString() + "";
    }

    protected string SqlBuilder()
    {
        StringBuilder sql = new StringBuilder();
        StringBuilder filters = new StringBuilder();
        StringBuilder includeCol = new StringBuilder();

        #region Default Report
        if (chkDefaultReport.Checked == true)
        {
            sql.Append(SetQuery());

            sql.Append(SetFilters());

            if (cbUnsplitted.Checked)
            {
                #region UNION for JobSplitDetaillosstime

                sql.Append(@"
                UNION ALL 
                ");
                sql.Append(SetQueryLossTime());

                #region Cost Center Access
                if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
                {
                    string temp = hfUserCostCenters.Value;
                    temp = temp.Replace("x", "''").Replace("y", ",");
                    sql.Append("\nAND Jsh_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1)));
                }
                #endregion

                #region Textboxes
                if (txtDashJobCode.Text != "")
                {
                    string text = txtDashJobCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string dashJobCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(dashJobCode + ")");
                    }
                }

                if (txtClientJobNo.Text != "")
                {
                    string text = txtClientJobNo.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string clientJobNo = "";
                        for (int i = 0; i < arr.Count; i++)
                            clientJobNo += string.Format("\nOR Slm_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(clientJobNo + ")");
                    }
                }

                //if (txtClientCode.Text != "")
                //{
                //    string text = txtClientCode.Text.Replace("'", "`");
                //    ArrayList arr = CommonLookUp.DivideString(text);

                //    if (arr.Count > 0)
                //    {
                //        string clientCode = "", clientName = "", clientShortName = "";
                //        for (int i = 0; i < arr.Count; i++)
                //        {
                //            clientCode += string.Format("\nOR Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
                //            clientName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                //            clientShortName += string.Format("\nOR Clh_ClientName like ''{0}%''", arr[i].ToString().Trim());
                //        }
                //        sql.Append("\nAND (");
                //        sql.Append("\n 1 = 0");
                //        sql.Append(clientCode);
                //        sql.Append(clientName);
                //        sql.Append(clientShortName + ")");
                //    }
                //}

                if (txtFWBSCode.Text != "")
                {
                    string text = txtFWBSCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string FWBSCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(FWBSCode + ")");
                    }
                }

                if (txtWorkCode.Text != "")
                {
                    string text = txtWorkCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string dashWorkCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            dashWorkCode += string.Format("\nOR Slm_DashWorkCode like ''{0}%''", arr[i].ToString().Trim());
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(dashWorkCode + ")");
                    }
                }

                if (txtEmpName.Text != string.Empty)
                {
                    string text = txtEmpName.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string employeeId = "", employeeName = "";
                        for (int i = 0; i < arr.Count; i++)
                        {
                            employeeId += string.Format("\nOR Jsh_EmployeeId like ''{0}%''", arr[i].ToString().Trim());
                            employeeName += string.Format("\nOR Emt_LastName like ''{0}%'' OR Emt_FirstName like ''{0}%''", arr[i].ToString().Trim());
                        }
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(employeeId);
                        sql.Append(employeeName + ")");
                    }
                }

                if (txtCostCenter.Text != "")
                {
                    string text = txtCostCenter.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string costCenterCode = "", costCenterName = "";
                        for (int i = 0; i < arr.Count; i++)
                        {
                            costCenterCode += string.Format("\nOR Jsh_Costcenter like ''{0}%''", arr[i].ToString().Trim());
                            costCenterName += string.Format("\nOR dbo.GetCostCenterName(Jsh_Costcenter) like ''{0}%''", arr[i].ToString().Trim());
                        }
                        sql.Append("\nAND (");
                        sql.Append("\n 1 = 0");
                        sql.Append(costCenterCode);
                        sql.Append(costCenterName + ")");
                    }
                }


                if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
                {
                    //no SubWorkCode for JobSplitDetailLossTime

                    //string text = txtSubWorkCode.Text.Replace("'", "`");
                    //ArrayList arr = CommonLookUp.DivideString(text);
                    //if (arr.Count > 0)
                    //{
                    //    string SubWorkCode = "";
                    //    for (int i = 0; i < arr.Count; i++)
                    //        SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                    //    sql.Append("\nAND (");
                    //    sql.Append("\n 1 = 0");
                    //    sql.Append(SubWorkCode + ")");
                    //}
                }

                #endregion

                if (rblBillable.SelectedValue != "A")
                {
                    if (rblBillable.SelectedValue.Equals("X"))
                        filters.Append(string.Format("\nAND Jsd_Category IN (''D'',''T'', ''X'')", rblBillable.SelectedValue));
                    else
                        filters.Append(string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue));
                }

                #region Date Time Pickers
                if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
                {
                    if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND (Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                         , Convert.ToDateTime(dtpRangeTo.Date)));
                    }
                    else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date)));
                    }
                    else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        sql.Append(string.Format(@"AND Convert(DateTime, Jsh_JobSplitDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date)));
                    }
                }
                #endregion
                #endregion
            }

            if (rblOption.SelectedValue == "E")
            {
                if (rbEmployeeOptions.SelectedValue == "S")
                {
                    if (chkSubWorkCode.Checked)
                        sql.Append(@"'
                    SET @PivotCol  = 'Convert(VarChar(50),ISNULL(Slm_ClientJobName,'''')) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'') + Convert(VarChar(10),Jsd_SubWorkCode)'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                    else
                        sql.Append(@"'
                    SET @PivotCol  = 'Convert(VarChar(50),ISNULL(Slm_ClientJobName,'''')) + COnvert(char(6),''<br />'') + Convert(Char(10),Jsd_JobCode) + Convert(char(6),''<br />'')'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else if (rbEmployeeOptions.SelectedValue == "D")
                {
                    sql.Append(@"'
                    SET @PivotCol  = 'convert(char(10),  Jsh_JobSplitDate,112)'

                    SET @Summaries = 'sum(Jsd_ActHours)'

                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
            }
            else if (rblOption.SelectedValue == "D")
                sql.Append(@"'
                        SET @PivotCol  = 'convert(varchar(50),dbo.getcostcentername(Jsh_Costcenter))'

                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "D")
            {
                sql.Append(@"'

                        --SET @PivotCol  = 'convert(char(4),DATEPART(yy,  Jsh_JobSplitDate)) + convert(char(4),DATEPART(DY,  Jsh_JobSplitDate)) + Convert(varchar(6),Jsh_JobSplitDate,113)'
                        
                        SET @PivotCol  = 'convert(char(10),  Jsh_JobSplitDate,112)'

                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            }
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "W")
            {
                sql.Append(@"'
                            --SET @PivotCol  = 'convert(char(4),DATEPART(yy,  Jsh_JobSplitDate)) + convert(char(3),DATEPART(WW,  Jsh_JobSplitDate)) + CONVERT(char(6), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 107) + '' - '' + CONVERT(char(6), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 107)'

                            SET @PivotCol  = 'CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101)'

                            SET @Summaries = 'sum(Jsd_ActHours)'

                            EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
            }
            else if (rblOption.SelectedValue == "C" && ddlDateType.SelectedValue == "M")
                sql.Append(@"' 
                            --SET @PivotCol  = 'convert(char(6),Jsh_JobSplitDate,112) + '' '' + CONVERT(char(3),Jsh_JobSplitDate,100) + ''-'' + RIGHT(CONVERT(char(4),DATEPART(YY,  Jsh_JobSplitDate)),2)'

                            SET @PivotCol  = 'convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2)'

                            SET @Summaries = 'sum(Jsd_ActHours)'

                            EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
        }
        #endregion
        #region Not Default Report
        else if (chkDefaultReport.Checked == false)
        {
            filters.Append(SetFilters());

            if (rblInclude.SelectedValue == "M")
            {
                includeCol.Append(@", Swc_MMCode + '' '' + Swc_MMCodeDesc [MM Code and Description]
                                   , Swc_MSCode + '' '' + Swc_MSCodeDesc [MS Code and Description]");
            }
            if (rblInclude.SelectedValue == "F")
            {
                includeCol.Append(@", Swc_FBSCode + '' '' + Swc_FBSCodeDesc [FBS Code and Description]");
            }

            string groupBy = string.Empty;
            if (chkSubWorkCode.Checked)
                groupBy = @"GROUP BY Dcm_DivisionDesc, Dcm_Departmentdesc, Scm_Sectiondesc, Sscm_Sectiondesc, Pcm_ProcessDesc, Jsd_JobCode, Slm_ClientJobName, Jsd_Category, Jcm_JobDescription, Jsd_ClientJobNo, Jsd_SubWorkCode, Swc_Description, Swc_MMCode, Swc_MMCodeDesc, Swc_MSCode, Swc_MSCodeDesc, Swc_FBSCode, Swc_FBSCodeDesc";
            else
                groupBy = @"GROUP BY Dcm_DivisionDesc, Dcm_Departmentdesc, Scm_Sectiondesc, Sscm_Sectiondesc, Pcm_ProcessDesc, Jsd_JobCode, Slm_ClientJobName, Jsd_Category, Jcm_JobDescription, Jsd_ClientJobNo, Jsd_SubWorkCode, Swc_Description";

            #region Employee, Detail
            if (ddlReport.SelectedValue == "1")
            {
                groupBy += ", Q.emt_lastname, Q.emt_FirstName, Jsh_JobSplitDate, Jsh_EmployeeId, Ecc_Description";

                groupBy = groupBy.Replace("Jsd_Category", @"case Jsd_Category
                                                                WHEN ''B'' Then ''BILLABLE (B)''
                                                                WHEN ''N'' Then ''NON-BILLABLE (N)''
                                                                WHEN ''X'' Then ''NON JOBS''
                                                                WHEN ''D'' Then ''NON JOBS''
                                                                WHEN ''T'' Then ''NON JOBS''
                                                                ELSE ''''
                                                            end");

                sql.Append(@"declare @SelectList varchar(max)

                    SET @SelectList = '");

                foreach (ListItem li in cblProfile.Items)
                {
                    if (li.Selected)
                    {
                        sql.Append(string.Format(@"
                        SELECT  
                                Ecc_Description [Department]
                                , Jsd_JobCode + '' '' + ISNULL(Jcm_JobDescription, '''') [Job]
                                , case Jsd_Category
                                    WHEN ''B'' Then ''BILLABLE (B)''
                                    WHEN ''N'' Then ''NON-BILLABLE (N)''
                                    WHEN ''X'' Then ''NON JOBS''
                                    WHEN ''D'' Then ''NON JOBS''
                                    WHEN ''T'' Then ''NON JOBS''
                                    ELSE ''''
                                end [Category]
                                ,Jsh_EmployeeID + '' '' + Q.Emt_LastName + '', '' + Q.Emt_FirstName as [Employee]
                                ,convert(varchar(20),Jsh_JobSplitDate, 101) as [DATE]
                                , Jsd_SubWorkCode as [Work Code]
                                , Swc_Description as [Work Description] 
                                , SUM(case when Jsd_Overtime=0 then Jsd_ActHours end) [REG]
							    , SUM(case when Jsd_Overtime=1 then Jsd_ActHours end) [OT]
                                , SUM(Jsd_ActHours) as [Manhours]
                                , Jsd_ClientJobNo [CYO Job No]
                                {2}
                            from {0}..T_JobSplitDetail
                    inner join {0}..T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                        and Jsh_Status = ''9''
                    LEFT JOIN {0}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                    JOIN {0}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                        and Jsd_ClientJobNo = Slm_ClientJobNo
                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                    LEFT JOIN {0}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    LEFT JOIN {0}..T_AccountDetail ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                    LEFT JOIN {0}..T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN {0}..T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN {0}..T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                    LEFT JOIN {0}..T_CostCenter on   Cct_CostCenterCode = Q.Emt_CostCenterCode
                    LEFT JOIN {0}..T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN {0}..T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN {0}..T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN {0}..T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN {0}..T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    LEFT JOIN {3}..E_SubWorkCodeMaster on LTRIM(Jsd_SubWorkCode) = Swc_AccountCode 
                        and (Jsd_CostCenter = Swc_CostCenterCode
						    OR (Swc_CostCenterCode = ''ALL'' and Jsd_CostCenter NOT IN 
							    (select Swc_CostCenterCode from {3}..E_SubWorkCodeMaster 
								    where LTRIM(Jsd_SubWorkCode) = Swc_AccountCode))
						    )
                    LEFT JOIN {3}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                    LEFT JOIN {3}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter  

                            WHERE 1 = 1
                            {1}
                        " + groupBy + @"

                        --ORDER BY Ecc_Description, Jsd_JobCode, Jsd_Category DESC, Jsh_EmployeeId"
                              , li.Value, filters, includeCol, ConfigurationManager.AppSettings["ERP_DB"]));

                        sql.Append(@"
                            UNION ");
                    }
                }

                sql.Remove(sql.ToString().LastIndexOf("UNION"), "UNION".Length);

                sql.Append(@"'
                        execute(@SelectList)");
            }
            else if (ddlReport.SelectedValue == "16")
            {
                StringBuilder includeCol16 = new StringBuilder();
                if (rblInclude.SelectedValue == "M")
                {
                    includeCol16.Append(@", Swc_MMCode
                                          , Swc_MMCodeDesc
                                          , Swc_MSCode  
                                          , Swc_MSCodeDesc");
                }
                if (rblInclude.SelectedValue == "F")
                {
                    includeCol16.Append(@", Swc_FBSCode 
                                          , Swc_FBSCodeDesc");
                }

                groupBy += ", Q.emt_lastname, Q.emt_FirstName, Jsh_JobSplitDate, Jsh_EmployeeId, Ecc_Description";

                groupBy = groupBy.Replace("Jsd_Category", @"case Jsd_Category
                                                                WHEN ''B'' Then ''BILLABLE (B)''
                                                                WHEN ''N'' Then ''NON-BILLABLE (N)''
                                                                WHEN ''X'' Then ''NON JOBS''
                                                                WHEN ''D'' Then ''NON JOBS''
                                                                WHEN ''T'' Then ''NON JOBS''
                                                                ELSE ''''
                                                            end");

                sql.Append(@"declare @SelectList varchar(max)
                    SET @SelectList = '");

                foreach (ListItem li in cblProfile.Items)
                {
                    if (li.Selected)
                    {
                        sql.Append(string.Format(@"
                        SELECT  
                                Jsd_JobCode
                                ,Jcm_JobDescription
                                , case Jsd_Category
                                    WHEN ''B'' Then ''BILLABLE (B)''
                                    WHEN ''N'' Then ''NON-BILLABLE (N)''
                                    WHEN ''X'' Then ''NON JOBS''
                                    WHEN ''D'' Then ''NON JOBS''
                                    WHEN ''T'' Then ''NON JOBS''
                                    ELSE ''''
                                end [Jsd_Category]
                                ,Q.Emt_LastName + '', '' + Q.Emt_FirstName as [Name]
                                ,Jsh_EmployeeID
                                ,convert(varchar(20),Jsh_JobSplitDate, 101) as [SPLIT DATE]
                                ,SUM(Jsd_ActHours) as [Jsd_ActHours]
                                , Jsd_ClientJobNo
                                , Jsd_SubWorkCode as [Subwork Code]
                                , SUM(case when Jsd_Overtime=0 then Jsd_ActHours end) [REG]
							    , SUM(case when Jsd_Overtime=1 then Jsd_ActHours end) [OT]
                                , SUM(Jsd_ActHours) [Total]
                                , Swc_Description as [Work Description]
                                {1}
                                , Ecc_Description
                                , Dcm_DepartmentDesc
                            from {3}..T_JobSplitDetail
                    inner join {3}..T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                         and Jsh_Status = ''9''
                    --Inner join t_employeelogledger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                        --LEFT JOIN {3}..T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN {3}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                    JOIN {3}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                        and Jsd_ClientJobNo = Slm_ClientJobNo
                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                        LEFT JOIN {3}..T_AccountDetail ON Adt_AccountCode = Jsd_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                        --left join {3}..T_LeaveTypeMaster  on {3}..T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                        --left join {3}..T_LeaveTypeMaster  {3}..T_LeaveTypeMaster2 on {3}..T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                        LEFT JOIN {3}..T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				        LEFT JOIN {3}..T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				        LEFT JOIN {3}..T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                    LEFT JOIN {3}..T_CostCenter on   Cct_CostCenterCode = Jsd_CostCenter
                    LEFT JOIN {3}..T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                    LEFT JOIN {3}..T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                    LEFT JOIN {3}..T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                    LEFT JOIN {3}..T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                    LEFT JOIN {3}..T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                    LEFT JOIN {2}..E_SubWorkCodeMaster on Jsd_SubWorkCode = Swc_AccountCode and Swc_CostCenterCode = Slm_CostCenter
                    LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                    JOIN {2}..E_COSASCostCenter on Jsh_Costcenter = Ecc_CostCenter  
                    LEFT JOIN {3}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode

                            WHERE 1 = 1
                            {0}
                        " + groupBy + @"
                            "
                          , filters, includeCol16, ConfigurationManager.AppSettings["ERP_DB"], li.Value));

                        sql.Append(@"
                                        UNION ");
                    }
                }

                sql.Remove(sql.ToString().LastIndexOf("UNION"), "UNION".Length);

                sql.Append(@"ORDER BY Ecc_Description, Jsd_JobCode, Jsd_Category, Jsh_EmployeeId, [SPLIT DATE]");
                sql.Append(@"'
                        execute(@SelectList)");
                sql = new StringBuilder(string.Format(sql.ToString(), filters.ToString(), includeCol16.ToString(), ConfigurationManager.AppSettings["ERP_DB"]));
            }
            #endregion
            #region Client
            else if (ddlReport.SelectedValue == "2")
            {
                groupBy = @"GROUP BY Department, [Job Code], [Job Description], Category";

                sql.Append(@"declare @SelectList varchar(max)
                    SET @SelectList = 'SELECT Department, [Job Code], [Job Description], Category, SUM(Manhours) as [Manhours] FROM (");

                foreach (ListItem li in cblProfile.Items)
                {
                    if (li.Selected)
                    {
                        sql.Append(string.Format(@"
                    SELECT  Ecc_Description [Department]
                            , Jsd_JobCode [Job Code]
                            , Jcm_JobDescription [Job Description]
                            , case Jsd_Category
                                WHEN ''B'' Then ''Billable Jobs''
                                WHEN ''N'' Then ''Other Non-billable Jobs''
                                WHEN ''X'' Then ''Other Non-billable Jobs''
                                WHEN ''D'' Then ''Business Improvement''
                                WHEN ''T'' Then ''Training''
                                ELSE ''''
                            end [Category]
                            ,Jsd_ActHours as [Manhours]
                            {1}
                        from {3}..T_JobSplitDetail
                inner join {3}..T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                    and Jsh_Status = ''9''
                JOIN {3}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                JOIN {3}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                LEFT JOIN {3}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    LEFT JOIN {3}..T_AccountDetail ON Adt_AccountCode = Jsh_Status 
                    AND Adt_AccountType =  ''WFSTATUS''
                    --left join {3}..T_LeaveTypeMaster  on {3}..T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join {3}..T_LeaveTypeMaster  {3}..T_LeaveTypeMaster2 on {3}..T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN {3}..T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN {3}..T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN {3}..T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                LEFT JOIN {3}..T_CostCenter on   Cct_CostCenterCode = Q.Emt_CostCenterCode
                LEFT JOIN {3}..T_DivisionCodeMaster on Dcm_DivisionCode= Cct_DivisionCode 
                LEFT JOIN {3}..T_DepartmentCodeMaster on Dcm_Departmentcode = Cct_Departmentcode
                LEFT JOIN {3}..T_SectionCodeMaster on  Scm_Sectioncode = Cct_Sectioncode
                LEFT JOIN {3}..T_SubSectionCodeMaster  on Sscm_Sectioncode = Cct_Subsectioncode 
                LEFT JOIN {3}..T_ProcessCodeMaster on Pcm_Processcode = Cct_Processcode
                LEFT JOIN {2}..E_SubWorkCodeMaster on LTRIM(Jsd_SubWorkCode) = Swc_AccountCode 
                    and (Jsd_CostCenter = Swc_CostCenterCode
						OR (Swc_CostCenterCode = ''ALL'' and Jsd_CostCenter NOT IN 
							(select Swc_CostCenterCode from {2}..E_SubWorkCodeMaster 
								where LTRIM(Jsd_SubWorkCode) = Swc_AccountCode))
						)
                LEFT JOIN {2}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                LEFT JOIN {2}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter  

                        WHERE 1 = 1
                        {0} 
                            "

                          , filters, includeCol, ConfigurationManager.AppSettings["ERP_DB"], li.Value));

                        sql.Append(@"
                                        UNION ALL");
                    }
                }

                sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);
                sql.Append(@") as tbl " + groupBy);
                sql.Append(@"'
                    execute(@SelectList)");
            }
            #endregion
            #region Department, Detail
            else if (ddlReport.SelectedValue == "3")
            {
                groupBy = "GROUP BY Jsd_JobCode, Jcm_JobDescription, Ecc_Description, Ecc_COSASCostCenter";

                sql.Append(@"declare @SelectList varchar(max)
                    SET @SelectList = '");

                foreach (ListItem li in cblProfile.Items)
                {
                    if (li.Selected)
                    {
                        sql.Append(string.Format(@"
                    SELECT  
                            Ecc_Description 
                            , Jsd_JobCode [CPH Job No.]
                            , Jcm_JobDescription [Job Description]
                            ,SUM(Jsd_ActHours) as [Total Manhours]
                        from {2}..T_JobSplitDetail
                inner join {2}..T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                    and Jsh_Status = ''9''
                --Inner join t_employeelogledger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                    --LEFT JOIN {2}..T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                LEFT JOIN {2}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                JOIN {2}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                    and Jsd_ClientJobNo = Slm_ClientJobNo
                    AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                LEFT JOIN {2}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    LEFT JOIN {2}..T_AccountDetail ON Adt_AccountCode = Jsh_Status 
                    AND Adt_AccountType =  ''WFSTATUS''
                    --left join {2}..T_LeaveTypeMaster  on {2}..T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join {2}..T_LeaveTypeMaster  {2}..T_LeaveTypeMaster2 on {2}..T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN {2}..T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN {2}..T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN {2}..T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                    LEFT JOIN {1}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                    LEFT JOIN {1}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter

                        WHERE 1 = 1
                        {0}
                    " + groupBy + @"
                    UNION

                    select 
                            Ecc_Description 
                            , Jsd_JobCode [CPH Job No.]
                            , Jcm_JobDescription [Client Job Name]
                            ,SUM(Jsd_ActHours) as [Total Manhours]
                    from {2}..T_JobSplitDetail
                inner join {2}..T_JobSplitHeader on Jsh_ControlNo = Jsd_ControlNo
	                        and Jsh_Status = ''9''
                    --Inner join t_employeelogledgerhist on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
                    --LEFT JOIN {2}..T_PayPeriodMaster ON Ppm_PayPeriod = Ell_PayPeriod
                    LEFT JOIN {2}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                    JOIN {2}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                        and Jsd_ClientJobNo = Slm_ClientJobNo
                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                    LEFT JOIN {2}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                    LEFT JOIN {2}..T_AccountDetail ON Adt_AccountCode = Jsh_Status 
                        AND Adt_AccountType =  ''WFSTATUS''
                    --left join {2}..T_LeaveTypeMaster  on {2}..T_LeaveTypeMaster.Ltm_LeaveType = Ell_EncodedPayLeaveType
                    --left join {2}..T_LeaveTypeMaster  {2}..T_LeaveTypeMaster2 on {2}..T_LeaveTypeMaster2.Ltm_LeaveType = Ell_EncodedNoPayLeaveType
                    LEFT JOIN {2}..T_EmployeeMaster C1 ON C1.Emt_EmployeeId = Jsh_CheckedBy
				    LEFT JOIN {2}..T_EmployeeMaster C2 ON C2.Emt_EmployeeId = Jsh_Checked2By
				    LEFT JOIN {2}..T_EmployeeMaster apr ON apr.Emt_EmployeeId = Jsh_ApprovedBy

                    LEFT JOIN {1}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                    LEFT JOIN {1}..E_COSASCostCenter on Jsd_Costcenter = Ecc_CostCenter

                    WHERE 1 = 1
                        {0}

                    " + groupBy + @"
                        
                    --ORDER BY Ecc_Description ,Jsd_JobCode, Jcm_JobDescription"
                          , filters, ConfigurationManager.AppSettings["ERP_DB"], li.Value));

                        sql.Append(@"
                                        UNION ");
                    }
                }

                sql.Remove(sql.ToString().LastIndexOf("UNION"), "UNION".Length);

                sql.Append(@"'
                    execute(@SelectList)
                    ");
            }
            #endregion
            #region Department, Summary
            else if (ddlReport.SelectedValue == "4")
            {
                groupBy = "GROUP BY convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2), Ecc_Description, Jcm_JobDescription, Jsd_SubWorkCode, Jsd_JobCode";
                sql.Append(@"declare @SelectList varchar(max)
                        declare @PivotCol varchar(300)
                        declare @Summaries varchar(300)

                        SET @SelectList = '");

                foreach (ListItem li in cblProfile.Items)
                {
                    if (li.Selected)
                    {
                        sql.Append(string.Format(@"select Ecc_Description
	                        , Jcm_JobDescription [Job Description]
                            , Jsd_JobCode [CPH Job No.]
	                        , case 
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''a'' then ''Contract Mhr''
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''b'' then ''Design Change Mhr''
		                        when right(rtrim(Jsd_SubWorkCode),1) = ''c'' then ''Blanket Mhr''
		                        else ''Others''
	                            end [ ] 
	                        , Sum(Jsd_ActHours) as [Jsd_ActHours]
	                        from {2}..T_JobSplitHeader
	                        join {2}..T_JobSplitDetail on Jsh_ControlNo = Jsd_ControlNo
	                        JOIN {2}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                and Jsd_ClientJobNo = Slm_ClientJobNo 
                                AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                            LEFT JOIN {2}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                            LEFT JOIN {2}..T_EmployeeMaster Q ON Q.Emt_EmployeeId = Jsh_EmployeeId
                            LEFT join {1}..E_COSASCostCenter on Ecc_CostCenter = Jsd_Costcenter
	                        LEFT JOIN {1}..E_JobCodeMaster on Jsd_JobCode = Jcm_CompJobNo and Jsd_ClientJobNo = Jcm_ClientJobNo
                            --JOIN {2}..T_EmployeeLogLedger on ell_processdate = Jsh_JobSplitDate and ell_employeeid = Jsh_EmployeeId
	                        where Jsh_Status = ''9''
                            {0}
	                        " + groupBy + @""
                                  , filters, ConfigurationManager.AppSettings["ERP_DB"], li.Value));

                        sql.Append(@"
                                            UNION ALL ");
                    }
                }

                sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                sql.Append(@"'
                        SET @PivotCol  = 'convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2)'
	
                        SET @Summaries = 'sum(Jsd_ActHours)'

                        EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries ");
            }
            #endregion
            #region Employee, Summary, Daily
            else if (ddlReport.SelectedValue == "5")
            {
                string jobCodeFilter = "";
                string dateFilter = "";
                string clientJobNoFilter = "";
                string clientCodeFilter = "";
                string employeeFilter = "";
                string costCenterFilter = "";
                string subWorkCodeFilter = "";
                string categoryFilter = "";

                #region Filters
                if (txtDashJobCode.Text != "")
                {
                    string text = txtDashJobCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string dashJobCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            dashJobCode += string.Format("\nOR Jsd_Jobcode like ''{0}%''", arr[i].ToString().Trim());
                        jobCodeFilter += "\nAND (";
                        jobCodeFilter += "\n 1 = 0";
                        jobCodeFilter += dashJobCode + ")";
                    }
                }

                if (txtClientJobNo.Text != "")
                {
                    string text = txtClientJobNo.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string clientJobNo = "";
                        for (int i = 0; i < arr.Count; i++)
                            clientJobNo += string.Format("\nOR Jsd_ClientJobNo like ''{0}%''", arr[i].ToString().Trim());
                        clientJobNoFilter += "\nAND (";
                        clientJobNoFilter += "\n 1 = 0";
                        clientJobNoFilter += clientJobNo + ")";
                    }
                }

                if (txtClientCode.Text != "")
                {
                    string text = txtClientCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);

                    if (arr.Count > 0)
                    {
                        for (int i = 0; i < arr.Count; i++)
                        {
                            clientCodeFilter += string.Format("\nAND Slm_ClientCode like ''{0}%''", arr[i].ToString().Trim());
                        }
                    }
                }

                if (txtEmpName.Text.Trim() != "")
                {
                    employeeFilter += "AND Ell_EmployeeId IN (";
                    string[] employees = txtEmpName.Text.Split(',');
                    for (int i = 0; i < employees.Length; i++)
                        employeeFilter += "''" + employees[i] + "'',";
                    employeeFilter = employeeFilter.Remove(employeeFilter.LastIndexOf(","));
                    employeeFilter += ")";
                }

                if (txtCostCenter.Text != "")
                {
                    string text = txtCostCenter.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string costCenterCode = "", costCenterName = "";
                        for (int i = 0; i < arr.Count; i++)
                        {
                            costCenterCode += string.Format("\nOR Q.Emt_CostCenterCode like ''{0}%''", arr[i].ToString().Trim());
                            costCenterName += string.Format("\nOR dbo.GetCostCenterName(Q.Emt_CostCenterCode) like ''{0}%''", arr[i].ToString().Trim());
                        }
                        costCenterFilter += "\nAND (";
                        costCenterFilter += "\n 1 = 0";
                        costCenterFilter += costCenterCode;
                        costCenterFilter += costCenterName + ")";
                    }
                }

                if (hfUserCostCenters.Value != "" && !hfUserCostCenters.Value.Contains("ALL"))
                {
                    string temp = hfUserCostCenters.Value;
                    temp = temp.Replace("x", "''").Replace("y", ",");
                    costCenterFilter += "\nAND Q.Emt_CostCenterCode in (@costCenterAcc)".Replace("@costCenterAcc", temp.Substring(0, temp.Length - 1));
                }

                if (txtSubWorkCode.Text != "" && chkSubWorkCode.Checked)
                {
                    string text = txtSubWorkCode.Text.Replace("'", "`");
                    ArrayList arr = CommonLookUp.DivideString(text);
                    if (arr.Count > 0)
                    {
                        string SubWorkCode = "";
                        for (int i = 0; i < arr.Count; i++)
                            SubWorkCode += string.Format("\nOR Jsd_SubWorkCode like ''{0}%''", arr[i].ToString().Trim());
                        subWorkCodeFilter += "\nAND (";
                        subWorkCodeFilter += "\n 1 = 0";
                        subWorkCodeFilter += SubWorkCode + ")";
                    }
                }

                if (rblBillable.SelectedValue != "A")
                {
                    if (rblBillable.SelectedValue.Equals("X"))
                        categoryFilter += string.Format("\nAND Jsd_Category IN (''D'',''T'', ''X'')", rblBillable.SelectedValue);
                    else
                        categoryFilter += string.Format("\nAND Jsd_Category = ''{0}''", rblBillable.SelectedValue);
                }

                if (!dtpRangeFrom.IsNull || !dtpRangeTo.IsNull)
                {
                    if (!dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        dateFilter += string.Format(@"AND (Convert(DateTime, Ell_ProcessDate, 1) 
                                 BETWEEN ''{0:d}'' AND ''{1:d}'') ", Convert.ToDateTime(dtpRangeFrom.Date)
                                                                         , Convert.ToDateTime(dtpRangeTo.Date));
                    }
                    else if (!dtpRangeFrom.IsNull && dtpRangeTo.IsNull)
                    {
                        dateFilter += string.Format(@"AND Convert(DateTime, Ell_ProcessDate, 1) 
                                 >= ''{0:d}'' ", Convert.ToDateTime(dtpRangeFrom.Date));
                    }
                    else if (dtpRangeFrom.IsNull && !dtpRangeTo.IsNull)
                    {
                        dateFilter += string.Format(@"AND Convert(DateTime, Ell_ProcessDate, 1) 
                                 <= ''{0:d}'' ", Convert.ToDateTime(dtpRangeTo.Date));
                    }
                }

                #endregion

                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"
                                select ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                    , SUM(Jsd_ActHours) Jsd_ActHours
	                                from {0}..T_EmployeeLogLedger
                                    LEFT JOIN {0}..T_JobSplitHeader 
			                            ON Ell_ProcessDate = Jsh_JobSplitDate
			                            AND Ell_EmployeeId = Jsh_EmployeeId 
			                            AND Jsh_Status = ''9''
                                    LEFT JOIN {0}..T_JobSplitDetail
		                                ON Jsh_ControlNo = Jsd_ControlNo "
                                        + jobCodeFilter
                                        + clientJobNoFilter
                                        + subWorkCodeFilter
                                        + categoryFilter + @"
	                                JOIN {0}..T_EmployeeMaster Q on Ell_EmployeeId = Emt_EmployeeID
                                    LEFT join {0}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1 " + dateFilter + employeeFilter + costCenterFilter + @"
                                    GROUP BY Ell_ProcessDate, ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname

                                    UNION ALL

                                select ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                    , SUM(Jsd_ActHours) Jsd_ActHours
	                                from {0}..T_EmployeeLogLedgerHist
                                    LEFT JOIN {0}..T_JobSplitHeader 
                                        ON Ell_EmployeeId = Jsh_EmployeeId and Ell_ProcessDate = Jsh_JobSplitDate and Jsh_Status = ''9''
                                    LEFT JOIN {0}..T_JobSplitDetail 
		                                ON Jsh_ControlNo = Jsd_ControlNo "
                                        + jobCodeFilter
                                        + clientJobNoFilter
                                        + subWorkCodeFilter
                                        + categoryFilter + @"
	                                JOIN {0}..T_EmployeeMaster Q on Ell_EmployeeId = Emt_EmployeeID
                                    LEFT join {0}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1 " + dateFilter + employeeFilter + costCenterFilter + @"
                                    GROUP BY Ell_ProcessDate, ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname
	                                ", li.Value));

                            sql.Append(@" 
                                        UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'
                                SET @PivotCol = 'Ell_ProcessDate'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)

                                SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"
                                select CONVERT(VARCHAR(10),Ell_ProcessDate,101) [Date]
                                    , Jsd_ActHours 
	                                from {0}..T_EmployeeLogLedger
	                                LEFT JOIN {0}..T_JobSplitHeader 
                                        ON Ell_EmployeeId = Jsh_EmployeeId and Ell_ProcessDate = Jsh_JobSplitDate and Jsh_Status = ''9''
                                    LEFT JOIN CHIYODA_RFSTSV..T_JobSplitDetail 
		                                ON Jsh_ControlNo = Jsd_ControlNo " + jobCodeFilter + clientJobNoFilter + subWorkCodeFilter + categoryFilter + @"
	                                JOIN {0}..T_EmployeeMaster Q on Ell_EmployeeId = Emt_EmployeeID
                                    LEFT join {0}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1 " + dateFilter + employeeFilter + costCenterFilter + @"

                                UNION ALL

                                select Ell_ProcessDate [Date]
                                    , Jsd_ActHours 
	                                from {0}..T_EmployeeLogLedgerHist
	                                LEFT JOIN {0}..T_JobSplitHeader 
                                        ON Ell_EmployeeId = Jsh_EmployeeId and Ell_ProcessDate = Jsh_JobSplitDate and Jsh_Status = ''9''
                                    LEFT JOIN CHIYODA_RFSTSV..T_JobSplitDetail 
		                                ON Jsh_ControlNo = Jsd_ControlNo " + jobCodeFilter + clientJobNoFilter + subWorkCodeFilter + categoryFilter + @"
	                                JOIN {0}..T_EmployeeMaster Q on Ell_EmployeeId = Emt_EmployeeID
                                    LEFT join {0}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
		                                AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1 " + dateFilter + employeeFilter + costCenterFilter + @"
	                                ", li.Value));

                            sql.Append(@" 
                                        UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'
                            SET @PivotCol = 'ISNULL(Adt_AccountDesc, '''') + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                            SET @Summaries = 'sum(Jsd_ActHours)'

                            EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
            }
            #endregion
            #region Employee, Summary, Weekly
            else if (ddlReport.SelectedValue == "6")
            {
                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)
                                declare @StartDay tinyint
                                set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                                set Datefirst @StartDay

                                SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"select ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                    , sum(Jsd_ActHours) [Jsd_ActHours]
                                    from {1}..T_JobSplitDetail 
                                    join {1}..T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                    LEFT JOIN {1}..T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN {1}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                    LEFT JOIN {1}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    LEFT join {1}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                        AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}    
           
		                            GROUP BY CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101), ISNULL(Adt_AccountDesc, ''''), Emt_FirstName, Emt_MiddleName, Emt_LastName"
                                , filters, li.Value));

                            sql.Append(@" 
                                    UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'                
                                SET @PivotCol = 'CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101)'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                DECLARE @PivotCol varchar(300)
                                DECLARE @Summaries varchar(max)
                                declare @StartDay tinyint
                                set @StartDay = (select Ccd_WeekCoverage from T_CompanyMaster)
                                set Datefirst @StartDay

                                SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"select CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101) [Date]
                                    , sum(Jsd_ActHours) [Jsd_ActHours]
                                    from {1}..T_JobSplitDetail 
                                    join {1}..T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                    LEFT JOIN {1}..T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN {1}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                        and Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                    LEFT JOIN {1}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    LEFT join {1}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                        AND Adt_AccountType = ''POSITION''
                                    WHERE 1 = 1
                                        {0}    
           
		                            GROUP BY CONVERT(char(10), Jsh_JobSplitDate - (DATEPART(DW,  Jsh_JobSplitDate) - 1), 101) + '' - '' + CONVERT(char(10), Jsh_JobSplitDate+(7 - (DATEPART(DW,  Jsh_JobSplitDate))), 101), ISNULL(Adt_AccountDesc, ''''), Emt_FirstName, Emt_MiddleName, Emt_LastName
                                    ", filters, li.Value));

                            sql.Append(@" 
                                    UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'
                                SET @PivotCol = 'ISNULL(Adt_AccountDesc, '''') + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                                SET @Summaries = 'sum(Jsd_ActHours)'

                                EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }

            }
            #endregion
            #region Employee, Summary, Monthly
            else if (ddlReport.SelectedValue == "7")
            {
                if (Convert.ToBoolean(hfTwist.Value))
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                    DECLARE @PivotCol varchar(300)
                                    DECLARE @Summaries varchar(max)

                                    SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"select ISNULL(Adt_AccountDesc, '''') + '' - '' +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname [Employee]
                                        , sum(Jsd_ActHours) [Jsd_ActHours]
                                        from {1}..T_JobSplitDetail 
                                        join {1}..T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                        LEFT JOIN {1}..T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                        JOIN {1}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                            AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                        LEFT JOIN {1}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        LEFT join {1}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                            AND Adt_AccountType = ''POSITION''
                                        WHERE 1 = 1
                                            {0}
		                                GROUP BY convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2), ISNULL(Adt_AccountDesc, ''''), Emt_FirstName, Emt_MiddleName, Emt_LastName
                                        ", filters, li.Value));

                            sql.Append(@" 
                                        UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'
                                    SET @PivotCol = 'convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2)'
                                    SET @Summaries = 'sum(Jsd_ActHours)'

                                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }
                else
                {
                    sql.Append(@"DECLARE @SelectList varchar(max)
                                    DECLARE @PivotCol varchar(300)
                                    DECLARE @Summaries varchar(max)

                                    SET @SelectList = '");

                    foreach (ListItem li in cblProfile.Items)
                    {
                        if (li.Selected)
                        {
                            sql.Append(string.Format(@"select convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2) [Month]
                                        , sum(Jsd_ActHours) [Jsd_ActHours]
                                        from {1}..T_JobSplitDetail 
                                        join {1}..T_JobSplitHeader on Jsd_ControlNo = Jsh_ControlNo and Jsh_Status = ''9''
                                        LEFT JOIN {1}..T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                        JOIN {1}..T_SalesMaster on Jsd_JobCode = Slm_DashJobCode 
                                            and Jsd_ClientJobNo = Slm_ClientJobNo
                                            AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                        LEFT JOIN {1}..T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                        LEFT join {1}..T_AccountDetail on Emt_PositionCode = Adt_AccountCode
                                            AND Adt_AccountType = ''POSITION''
                                        WHERE 1 = 1
                                            {0}
		                                GROUP BY convert(char(4), Jsh_JobSplitDate,112) + ''-'' + Right(convert(char(6), Jsh_JobSplitDate,112),2), ISNULL(Adt_AccountDesc, ''''), Emt_FirstName, Emt_MiddleName, Emt_LastName
                                        ", filters, li.Value));


                            sql.Append(@" 
                                    UNION ALL ");
                        }
                    }

                    sql.Remove(sql.ToString().LastIndexOf("UNION ALL"), "UNION ALL".Length);

                    sql.Append(@"'   SET @PivotCol = 'ISNULL(Adt_AccountDesc, '''') + Convert(char(6),''<br />'') +  LEFT(Emt_FirstName,1)+ Left(Emt_MiddleName,1) + '' ''+ Emt_lastname'
                                    SET @Summaries = 'sum(Jsd_ActHours)'

                                    EXECUTE dbo.dynamic_pivot @SelectList,@PivotCol,@Summaries");
                }

            }
            #endregion
            else if (ddlReport.SelectedValue == "14")
            {
                sql.Append(@"DECLARE @SelectList varchar(max)
                             SET @SelectList = '

                                 SELECT Bcn_BillingYearMonth [MONTH]
	                                , Jcm_ClientInfo1 [SECTION CODE]
	                                , Jcm_ClientInfo2 [DEPT]
	                                , Jcm_ClientInfo3 [UNIT/GROUP]
	                                , Jcm_DocumentReference [RFES NO.]
	                                , Jsd_ClientJobNo [JOBCODE]
	                                , SUM(Jsd_ActHours) [TOTAL]
	                                , Jsd_JobCode [CPh JOB CODE]
	                                , Jcm_JobDescription [CPh JOB DESCRIPTION]
	                                , Dcm_Departmentcode [CPh DEPT CODE]
                                    , Dcm_Departmentdesc [CPh DEPT NAME]
	                                , Jcm_Remarks [REMARKS]
	                                FROM T_JobSplitHeader
                                    INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
	                                JOIN T_JobSplitDetail ON Jsh_ControlNo = Jsd_ControlNo
	                                JOIN {2}..E_JobCodeMaster ON Jsd_JobCode = Jcm_CompJobNo AND Jsd_ClientJobNo = Jcm_ClientJobNo
	                                JOIN T_SalesMaster ON Jsd_JobCode = Slm_DashJobCode 
                                        AND Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    JOIN {2}..E_BillingConfiguration ON Jsh_JobSplitDate BETWEEN Bcn_StartCycle AND Bcn_EndCycle
                                    LEFT JOIN T_DepartmentCodeMaster ON Dcm_Departmentcode = SUBSTRING(Jsd_Costcenter,3,2)
                                    WHERE Jsh_Status = ''9'' {0}
	                                GROUP BY Bcn_BillingYearMonth
	                                , Jcm_ClientInfo1
	                                , Jcm_ClientInfo2
	                                , Jcm_ClientInfo3
	                                , Jcm_DocumentReference
	                                , Jsd_ClientJobNo
	                                , Jsd_JobCode
	                                , Jcm_JobDescription
	                                , Dcm_Departmentcode
                                    , Dcm_Departmentdesc
	                                , Jcm_Remarks'

                                execute(@SelectList)");
            }
            else if (ddlReport.SelectedValue == "15")
            {
                sql.Append(@"DECLARE @SelectList varchar(max)
                             SET @SelectList = '
                                 SELECT Bcn_BillingYearMonth [MONTH]
                                    , Jcm_ClientInfo1 [SECTION CODE]
                                    , Jcm_ClientInfo2 [DEPT]
                                    , Jcm_ClientInfo3 [UNIT/GROUP]
                                    , Jcm_DocumentReference [RFES NO.]
                                    , Jsd_ClientJobNo [JOBCODE]
                                    , Swc_FBSCode [FBS]
                                    , Swc_FBSCodeDesc [DESCRIPTION]
                                    , Swc_MMCode [MM]
                                    , Swc_MMCodeDesc [DESCRIPTION]
                                    , Swc_MSCode [MS]
                                    , Swc_MSCodeDesc [DESCRIPTION]
                                    , SUM(Jsd_ActHours) [G TOTAL]
                                    , Jsd_JobCode [CPh JOB CODE]
                                    , Dcm_Departmentcode [CPh DEPT CODE]
                                    , Dcm_Departmentdesc [CPh DEPT NAME]
                                    , Swc_Remarks [REMARKS]
                                    FROM T_JobSplitHeader
                                    INNER JOIN T_EmployeeMaster Q on Jsh_EmployeeId = Emt_EmployeeID
                                    JOIN T_JobSplitDetail ON Jsh_ControlNo = Jsd_ControlNo
                                    JOIN {2}..E_JobCodeMaster ON Jsd_JobCode = Jcm_CompJobNo AND Jsd_ClientJobNo = Jcm_ClientJobNo
                                    JOIN T_SalesMaster ON Jsd_JobCode = Slm_DashJobCode 
                                        AND Jsd_ClientJobNo = Slm_ClientJobNo
                                        AND (Jsd_CostCenter = Slm_CostCenter OR ''ALL'' = Slm_CostCenter)
                                    LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                                    LEFT JOIN {2}..E_SubWorkCodeMaster on LTRIM(Jsd_SubWorkCode) = Swc_AccountCode 
                                        and (Jsd_CostCenter = Swc_CostCenterCode
						                    OR (Swc_CostCenterCode = ''ALL'' and Jsd_CostCenter NOT IN 
							                    (select Swc_CostCenterCode from {2}..E_SubWorkCodeMaster 
								                    where LTRIM(Jsd_SubWorkCode) = Swc_AccountCode))
						                    )
                                    JOIN {2}..E_BillingConfiguration ON Jsh_JobSplitDate BETWEEN Bcn_StartCycle AND Bcn_EndCycle
                                    LEFT JOIN T_DepartmentCodeMaster ON Dcm_Departmentcode = SUBSTRING(Jsd_Costcenter,3,2)
                                    WHERE Jsh_Status = ''9'' {0}
                                    GROUP BY Bcn_BillingYearMonth
                                    , Jcm_ClientInfo1
                                    , Jcm_ClientInfo2
                                    , Jcm_ClientInfo3
                                    , Jcm_DocumentReference
                                    , Jsd_ClientJobNo
                                    , Jsd_JobCode
                                    , Dcm_Departmentcode
                                    , Dcm_Departmentdesc
                                    , Jcm_Remarks
                                    , Swc_FBSCode
                                    , Swc_FBSCodeDesc
                                    , Swc_MMCode
                                    , Swc_MMCodeDesc
                                    , Swc_MSCode
                                    , Swc_MSCodeDesc
                                    , Swc_Remarks'

                                execute(@SelectList)");
            }


            sql = new StringBuilder(string.Format(sql.ToString(), filters, includeCol, ConfigurationManager.AppSettings["ERP_DB"]));
        }
        #endregion
        return sql.ToString();
    }

    protected void sumValues(decimal[] values, DataSet ds, int dsIndex)
    {
        values[0] += 1;
        values[1] += Convert.ToDecimal(ds.Tables[0].Rows[dsIndex]["Jsd_ActHours"]);
    }

    private DataTable GetTable()
    {
        DataTable table = this.VSDataTable;

        if (table == null)
        {
            GetData();
            if (!CommonMethods.isEmpty(dsView))
            {
                table = dsView.Tables[0];
            }

            this.VSDataTable = table;

            //create GridViewDataColumns from DataColumns of DataTable
            if (table != null)
            {
                foreach (DataColumn dc in table.Columns)
                {
                    GridViewDataColumn gvdc = new GridViewDataColumn(dc.ColumnName);
                    ASPxGridView1.Columns.Add(gvdc);
                }
            }
        }

        return table;
    }

    #endregion

    #region Custom Methods
    private void GenerateDynamicControls(DataRowView dr, string col, int ctr, ArrayList arr)
    {
        using (DataTable dtSource = dsView.Tables[0].Copy())
        {
            dtSource.DefaultView.RowFilter = string.Format("[{0}] = '{1}'", col, dr[col]);
            dtSource.Columns.Remove(col);
            //header
            HtmlGenericControl div1 = new HtmlGenericControl("div");
            div1.Attributes["class"] = "dhtmlgoodies_question";
            div1.ID = "Div_" + dr[col] + ctr++;
            div1.Attributes["style"] = "width: 894px;";
            div1.Controls.Add(AddLabelControl(dr[col].ToString()));
            //panel
            Panel pan = new Panel();

            pan.Controls.Add(AddGridViewControl(dtSource));
            pan.Attributes["Style"] = "position: static;";
            //pan.Attributes.Add("height", "Auto");
            pan.Width = Unit.Pixel(894);
            pan.ID = "Panel_" + dr[col] + ctr++;
            pan.ScrollBars = ScrollBars.Auto;
            pan.Height = innerHeight;

            //hide2 thingy
            HtmlGenericControl div2 = new HtmlGenericControl("div");
            div2.Attributes["class"] = "dhtmlgoodies_answer";
            div2.ID = "Div_" + dr[col] + ctr++;
            div2.Attributes["style"] = "width: 894px; height: inherit; left: 0px; top: 0px; position: static;";
            arr.Add(dr[col]);

            div2.Controls.Add(pan);
            Panel1.Controls.Add(div1);
            Panel1.Controls.Add(div2);
        }
    }

    private Label AddLabelControl(string Text)
    {
        Label lbl = new Label();
        lbl.ID = "lblGrid_" + NumberOfControls;
        lbl.Text = Text;
        lbl.Font.Bold = true;
        lbl.Font.Italic = true;
        this.NumberOfControls++;
        return lbl;
    }

    private GridView AddGridViewControl(DataTable dtHours)
    {

        if (chkDefaultReport.Checked == true
            || ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7")
            dtHours.Columns.Add("TOTAL");

        using (GridView grdv = new GridView())
        {
            grdv.Attributes["runat"] = "server";
            grdv.CellPadding = 0;
            grdv.CellSpacing = 1;
            grdv.ID = "grdv_" + NumberOfControls;
            if (ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7")
                grdv.RowCreated += new GridViewRowEventHandler(grdView_RowCreated);
            if (ddlReport.SelectedValue == "5")
                grdv.RowDataBound += gridViewCDESR_RowDataBound;
            if (ddlReport.SelectedValue == "16")
                grdv.RowDataBound += grdView_RowDataBound;
            grdv.DataSource = dtHours;
            grdv.DataBind();
            bool flagX = true;
            bool flagY = true;
            string className = string.Empty;

            for (int i = 0, j = 0; i < grdv.Rows.Count; i++)
            {
                if (flagX)
                {
                    flagX = false;
                    if (flagY)
                    {
                        className = "alternativeRowspanColored";
                        flagY = false;
                    }
                    else
                    {
                        className = "alternativeRowspanWhite";
                        flagY = true;
                    }
                }
                if (i > 0)
                    if (grdv.Rows[j].Cells[0].Text == grdv.Rows[i].Cells[0].Text && ddlReport.SelectedValue != "16")
                    {
                        grdv.Rows[i].Cells[0].CssClass = className;
                        grdv.Rows[i].Cells[0].Text = "";
                    }
                    else
                    {
                        j = i;
                        flagX = true;
                    }
                if (chkDefaultReport.Checked == false && !(ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7"))
                {
                    if (ddlReport.SelectedValue == "3")
                    {
                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[2].Attributes.Add("class", "Nums");
                    }
                    else if (ddlReport.SelectedValue == "2")
                    {
                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                        if (grdv.Rows[i].Cells[0].Text != "&nbsp;" && grdv.Rows[i].Cells[0].Text != "" && grdv.Rows[i].Cells[1].Text != "&nbsp;" && grdv.Rows[i].Cells[1].Text != "")
                        {
                            grdv.Rows[i].Cells[1].Font.Bold = true;
                            grdv.Rows[i].Cells[0].Font.Bold = true;
                            grdv.Rows[i].Cells[1].ColumnSpan = 2;
                            grdv.Rows[i].Cells.RemoveAt(2);
                            grdv.Rows[i].Cells[2].Attributes.Add("class", "text");
                            grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Right;
                        }
                        else
                        {
                            grdv.Rows[i].Cells[3].Attributes.Add("class", "Nums");
                            grdv.Rows[i].Cells[3].HorizontalAlign = HorizontalAlign.Right;
                        }
                        grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Left;
                    }
                    else if (ddlReport.SelectedValue == "4")
                    {
                        for (int column = 3; column < grdv.Rows[i].Cells.Count; column++)
                            grdv.Rows[i].Cells[column].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[1].HorizontalAlign = HorizontalAlign.Left;
                    }
                    else if (ddlReport.SelectedValue == "14")
                    {
                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[5].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[6].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[7].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[9].HorizontalAlign = HorizontalAlign.Left;
                    }
                    else if (ddlReport.SelectedValue == "15")
                    {
                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[5].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[6].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[10].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[11].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[12].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[13].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[14].HorizontalAlign = HorizontalAlign.Left;
                    }
                    else if (ddlReport.SelectedValue == "16")
                    {
                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[1].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[3].HorizontalAlign = HorizontalAlign.Left;
                        grdv.Rows[i].Cells[4].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[5].Attributes.Add("class", "Nums");
                        grdv.Rows[i].Cells[6].Attributes.Add("class", "Nums");
                    }
                }
                else
                    if (chkDefaultReport.Checked == true || ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7")
                    {
                        int colNumbersStart = 0;
                        if (ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7")
                        {
                            grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                            colNumbersStart = 1;
                        }
                        else
                            if (rblOption.SelectedValue == "E")
                            {
                                //Employee
                                if (ddlReport.SelectedValue == "8")
                                {
                                    grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Left;
                                    if (chkSubWorkCode.Checked)
                                        colNumbersStart = 4;
                                    else
                                        colNumbersStart = 3;
                                }
                                else
                                    if (rbEmployeeOptions.SelectedValue == "S")
                                        // Summary
                                        colNumbersStart = 1;
                            }
                            else
                                if (ddlReport.SelectedValue == "9")
                                {
                                    // Department
                                    grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                                    grdv.Rows[i].Cells[1].HorizontalAlign = HorizontalAlign.Left;
                                    if (chkSubWorkCode.Checked)
                                        colNumbersStart = 4;
                                    else
                                        colNumbersStart = 3;
                                }
                                else
                                    if (ddlReport.SelectedValue == "10" || ddlReport.SelectedValue == "11" || ddlReport.SelectedValue == "12")
                                    {
                                        grdv.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                                        grdv.Rows[i].Cells[2].HorizontalAlign = HorizontalAlign.Left;
                                        if (chkSubWorkCode.Checked)
                                            colNumbersStart = 4;
                                        else
                                            colNumbersStart = 3;
                                    }
                        for (int column = colNumbersStart; column < grdv.Rows[i].Cells.Count; column++)
                            grdv.Rows[i].Cells[column].Attributes.Add("class", "Nums");
                    }
            }
            grdv.Attributes["Width"] = Panel1.Width.ToString();
            //before value: auto
            if (chkDefaultReport.Checked == true || ddlReport.SelectedValue == "4" || ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7" || ddlReport.SelectedValue == "3")
            {
                grdv.FooterRow.Visible = true;
                int colNumberStart = 0;
                if (ddlReport.SelectedValue == "4")
                    colNumberStart = 3;
                else
                    if (ddlReport.SelectedValue == "3")
                        colNumberStart = 2;
                    else
                        if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "E")
                        {
                            if (rbEmployeeOptions.SelectedValue == "D")
                            {
                                if (chkSubWorkCode.Checked)
                                    colNumberStart = 4;
                                else
                                    colNumberStart = 3;
                            }
                            else
                                if (rbEmployeeOptions.SelectedValue == "S")
                                    colNumberStart = 1;
                        }
                        else
                            if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "D")
                            {
                                // Department
                                if (chkSubWorkCode.Checked)
                                    colNumberStart = 4;
                                else
                                    colNumberStart = 3;
                            }
                            else
                                if (chkDefaultReport.Checked == true && rblOption.SelectedValue == "C")
                                    // Client
                                    if (chkSubWorkCode.Checked)
                                        colNumberStart = 4;
                                    else
                                        colNumberStart = 3;
                for (int column = colNumberStart; column < grdv.FooterRow.Cells.Count; column++)
                    grdv.FooterRow.Cells[column].Attributes.Add("class", "Nums");
            }
            grdv.FooterStyle.BackColor = ColorTranslator.FromHtml("#CCCC99");
            grdv.RowStyle.BackColor = ColorTranslator.FromHtml("#F7F7DE");
            grdv.HeaderStyle.BackColor = ColorTranslator.FromHtml("#6B696B");
            grdv.HeaderStyle.Font.Bold = true;
            grdv.HeaderStyle.ForeColor = Color.White;
            grdv.AlternatingRowStyle.BackColor = Color.White;
            // Default Report or Summary, Department and Monthly 
            if (chkDefaultReport.Checked == true || ddlReport.SelectedValue == "4" || ddlReport.SelectedValue == "5" || ddlReport.SelectedValue == "6" || ddlReport.SelectedValue == "7" || ddlReport.SelectedValue == "3")
            {
                ListDictionary Totals = new ListDictionary();
                decimal Totalcols = 0;
                foreach (Control ctrl in grdv.Controls[0].Controls)
                    if (ctrl is GridViewRow)
                    {
                        int x = 0;
                        if (!chkSubWorkCode.Checked)
                            x = 1;
                        int ctr = 0;
                        if (rblOption.SelectedValue == "E")
                        {
                            if (rbEmployeeOptions.SelectedValue == "S")
                                ctr = 1;
                            else
                                if (rbEmployeeOptions.SelectedValue == "D")
                                    if (chkSubWorkCode.Checked)
                                        ctr = 4;
                                    else
                                        ctr = 3;
                        }
                        else
                            if (ddlReport.SelectedValue == "4")
                                ctr = 3;
                            else
                                if (chkDefaultReport.Checked == false && ddlReport.SelectedValue == "3")
                                    ctr = 2;
                                else
                                    ctr = 4 - x;
                        CommonLookUp.SetGridViewCellsV2((GridViewRow)ctrl, new ArrayList());
                        if (((GridViewRow)ctrl).RowType == DataControlRowType.Header)
                        {
                            for (int i = 5; i < ((GridViewRow)ctrl).Cells.Count; i++)
                            {
                                TableCell tc = ((GridViewRow)ctrl).Cells[i];
                            }
                        }
                        else
                            if (((GridViewRow)ctrl).RowType == DataControlRowType.DataRow)
                                for (int i = ctr; i < ((GridViewRow)ctrl).Cells.Count; i++)
                                {
                                    if (!(ddlReport.SelectedValue == "4" || ddlReport.SelectedValue == "3"))
                                        if (i == ((GridViewRow)ctrl).Cells.Count - 1)
                                        {
                                            ((GridViewRow)ctrl).Cells[i].Text = Totalcols.ToString();
                                            Totalcols = 0;
                                        }
                                    TableCell tc = ((GridViewRow)ctrl).Cells[i];
                                    tc.HorizontalAlign = HorizontalAlign.Right;
                                    try
                                    {
                                        decimal dec = 0;
                                        if (tc.Text == "&nbsp;")
                                            dec = 0;
                                        else
                                            dec = decimal.Parse(tc.Text);
                                        if (i < ((GridViewRow)ctrl).Cells.Count - 1)
                                            Totalcols += dec;
                                        if (Totals.Contains(i))
                                            Totals[i] = (decimal)Totals[i] + dec;
                                        else
                                            Totals.Add(i, dec);
                                    }
                                    catch (Exception ex)
                                    {
                                        //Response.Write(ex.ToString());
                                    }
                                }
                            else
                                if (((GridViewRow)ctrl).RowType == DataControlRowType.Footer)
                                {
                                    TableRow tr = (GridViewRow)ctrl;
                                    ((GridViewRow)ctrl).Cells[0].Text = "TOTAL ";
                                    tr.Cells[0].Wrap = false;
                                    for (int i = ctr; i < tr.Cells.Count; i++)
                                    {
                                        try
                                        {
                                            TableCell tc = tr.Cells[i];
                                            tc.HorizontalAlign = HorizontalAlign.Right;
                                            tc.Text = string.Format("{0:0.00}", decimal.Parse(Totals[i].ToString()));
                                            grandTotal += (decimal)Totals[i];
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                    }
            }
            else
            {
                foreach (Control ctrl in grdv.Controls[0].Controls)
                {
                    if (ctrl is GridViewRow)
                    {
                        CommonLookUp.SetGridViewCellsV2((GridViewRow)ctrl, new ArrayList());
                    }
                }
            }

            this.NumberOfControls++;
            if (rblOption.SelectedValue == "E" && chkDefaultReport.Checked)
            {
                innerHeight = ((grdv.Rows.Count + 2) * 18) + 45;
            }
            else
            {
                innerHeight = ((grdv.Rows.Count + 2) * 18) + 18;
            }
            outerHeight += innerHeight + 60;
            return (GridView)grdv;
        }
    }
    #endregion
}