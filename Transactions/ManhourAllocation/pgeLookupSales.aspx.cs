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
using System.Text;
using Payroll.DAL;

public partial class pgeLookupSales : System.Web.UI.Page
{
    static string type = string.Empty;
    static string retCol = string.Empty;
    static string retcontrol = string.Empty;
    static string userCostCenter = string.Empty;
    static DataSet dsView;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userLogged"].ToString().Equals(string.Empty))
        {
            this.Dispose();
        }
        else
        {
            type = Request.QueryString["type"].ToString();
            retcontrol = Request.QueryString["control"].ToString();
            userCostCenter = Request.QueryString["costCenter"].ToString().Replace("x", "'").Replace("y", ",");
            if (!Page.IsPostBack)
            {
                retCol = InitializeControls();
                CommonLookUp.InitializeStatus(ddlStatus, "SALESSTAT");
                GetData();
                EnableButtons();
                LoadComplete +=new EventHandler(pgeLookupSales_LoadComplete);
            }
        }
        SetFocus(txtDashJobCode);

    }

    #region
    protected string InitializeControls()
    {
        string sqlRet = "";
        switch (type)
        { 
            case "S1":
                Page.Title = "CPH Job No Lookup";
                lblValues.Text = "CPH Job No(s)";
                retCol = "CPH Job No";
                sqlRet = @"Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobNo [Client Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              ,Slm_ClientCode [Client Code]
                              --,Slm_ClientFWBSCode [Client FWBS Code]
                              ,dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              --,Slm_DashWorkCode [Dash Work Code]
                              ,Adt_AccountDesc [Status]";
                break;
            case "S2":
                Page.Title = "Client Job Number Lookup";
                lblValues.Text = "Client Job Number(s)";
                retCol = "Client Job No";
                sqlRet = @"Slm_ClientJobNo [Client Job No]
                              ,Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              ,Slm_ClientCode [Client Code]
                              --,Slm_ClientFWBSCode [Client FWBS Code]
                              ,dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              --,Slm_DashWorkCode [Dash Work Code]
                              ,Adt_AccountDesc [Status]";
                break;
            case "S3":
                Page.Title = "Client Code Lookup";
                lblValues.Text = "Client Code(s)";
                retCol = "Client Code";
                sqlRet = @"Slm_ClientCode [Client Code]
                              ,Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobNo [Client Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              --,Slm_ClientFWBSCode [Client FWBS Code]
                              ,dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              --,Slm_DashWorkCode [Dash Work Code]
                              ,Adt_AccountDesc [Status]";
                break;
            case "S4":
                Page.Title = "Client FWBS Lookup";
                lblValues.Text = "Client FWBS(s)";
                retCol = "Client FWBS Code";
                sqlRet = @"Slm_ClientFWBSCode [Client FWBS Code]
                              ,Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobNo [Client Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              ,Slm_ClientCode [Client Code]
                              ,dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              --,Slm_DashWorkCode [Dash Work Code]
                              ,Adt_AccountDesc [Status]";
                break;
            case "S5":
                Page.Title = "Cost Center Lookup";
                lblValues.Text = "Cost Center(s)";
                retCol = "Cost Center";
                sqlRet = @"dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              ,Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobNo [Client Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              ,Slm_ClientCode [Client Code]
                              ,Slm_ClientFWBSCode [Client FWBS Code]
                              --,Slm_DashWorkCode [Dash Work Code]
                              ,Adt_AccountDesc [Status]";
                break;
            case "S6":
                Page.Title = "DASH Work Code Lookup";
                lblValues.Text = "DASH Work Code(s)";
                retCol = "Dash Work Code";
                sqlRet = @"Slm_DashWorkCode [Dash Work Code]
                              ,Slm_DashJobCode [CPH Job No]
                              ,Slm_ClientJobNo [Client Job No]
                              ,Slm_ClientJobName [Client Job Name]
                              ,Slm_ClientCode [Client Code]
                              --,Slm_ClientFWBSCode [Client FWBS Code]
                              ,dbo.GetCostCenterName(Jsd_Costcenter) [Cost Center]
                              ,Adt_AccountDesc [Status]";
                break;
            default: sqlRet = "1";
                break;
        }
        return sqlRet;
    }
    #endregion

    protected void pgeLookupSales_LoadComplete(object sender, EventArgs e)
    {
        btnRetrieve.Attributes.Add("OnClick", "javascript:return SendValueToParent()");
        lbxValues.Attributes.Add("ondblclick", "javascript:removeItem()");
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        GetData();
        EnableButtons();
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
                grdViewList.DataSource = dsView;
                grdViewList.DataBind();
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
    }
    private string SqlBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"   SELECT DISTINCT " + retCol + @"
                          FROM T_JobSplitHeader 
                             INNER JOIN T_JobSplitDetail ON Jsh_ControlNo = Jsd_ControlNo
							        AND Jsh_Status = '9'
                             INNER JOIN T_SalesMaster ON Jsd_JobCode = Slm_DashJobCode
							        AND Jsd_ClientJobNo = Slm_ClientJobNo
							        --AND Jsd_CostCenter = Slm_Costcenter
                             LEFT JOIN T_ClientHeader ON Clh_ClientCode = Slm_ClientCode
                             LEFT JOIN T_AccountDetail ON Adt_AccountCode = Slm_Status
                                   AND Adt_AccountType = 'SALESSTAT'
                         WHERE 1 = 1");
        #region Cost Center Access
        if (userCostCenter != "" && !userCostCenter.Contains("ALL"))
            sql.Append("\nAND Jsd_CostCenter in (@costCenterAcc)".Replace("@costCenterAcc", userCostCenter));
        #endregion

        #region Textbox filter
        if (txtDashJobCode.Text != "")
        {
            string text = txtDashJobCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string dashJobCode = "";
                for (int i = 0; i < arr.Count; i++)
                    dashJobCode += string.Format("\nOR Slm_DashJobCode like '{0}%'", arr[i].ToString().Trim());
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
                    clientJobNo += string.Format("\nOR Slm_ClientJobNo like '{0}%'", arr[i].ToString().Trim());
                sql.Append("\nAND (");
                sql.Append("\n 1 = 0");
                sql.Append(clientJobNo + ")");
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
                    clientCode += string.Format("\nOR Slm_ClientCode like '{0}%'", arr[i].ToString().Trim());
                    clientName += string.Format("\nOR Clh_ClientName like '{0}%'", arr[i].ToString().Trim());
                    clientShortName += string.Format("\nOR Clh_ClientName like '{0}%'", arr[i].ToString().Trim());
                }
                sql.Append("\nAND (");
                sql.Append("\n 1 = 0");
                sql.Append(clientCode);
                sql.Append(clientName);
                sql.Append(clientShortName + ")");
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
                    FWBSCode += string.Format("\nOR Slm_ClientFWBSCode like '{0}%'", arr[i].ToString().Trim());
                sql.Append("\nAND (");
                sql.Append("\n 1 = 0");
                sql.Append(FWBSCode + ")");
            }
        }
        if (txtDashWorkCode.Text != "")
        {
            string text = txtDashWorkCode.Text.Replace("'", "`");
            ArrayList arr = CommonLookUp.DivideString(text);
            if (arr.Count > 0)
            {
                string dashWorkCode = "";
                for (int i = 0; i < arr.Count; i++)
                    dashWorkCode += string.Format("\nOR Slm_DashWorkCode like '{0}%'", arr[i].ToString().Trim());
                sql.Append("\nAND (");
                sql.Append("\n 1 = 0");
                sql.Append(dashWorkCode + ")");
            }
        }
        #endregion

        if (ddlStatus.SelectedValue != "")
            sql.Append(string.Format("\nAND Slm_Status = '{0}'", ddlStatus.SelectedValue));
        sql.Append("\nOrder by 1");
        return sql.ToString();
    }
    protected void grdViewList_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList());
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (grdViewList.SelectedRow != null && Convert.ToString(grdViewList.SelectedRow.Cells[0]) != "")
        {
            if (grdViewList.SelectedRow.Cells[0].Text != "&nbsp;" && lbxValues.Items.FindByValue(grdViewList.SelectedRow.Cells[0].Text) == null)
                lbxValues.Items.Add(new ListItem(grdViewList.SelectedRow.Cells[0].Text, grdViewList.SelectedRow.Cells[0].Text));
        }
        EnableButtons();
    }
    protected void btnDel_Click(object sender, EventArgs e)
    {
        if (lbxValues.SelectedItem != null)
            lbxValues.Items.Remove(lbxValues.SelectedItem);
        EnableButtons();
    }
    private void EnableButtons()
    {
        if (lbxValues.Items.Count > 0)
        {
            btnRetrieve.Enabled = true;
            btnDel.Enabled = true;
        }
        else
        {
            //btnRetrieve.Enabled = false;
            btnDel.Enabled = false;
        }
        if (grdViewList.Rows.Count > 0)
            btnAdd.Enabled = true;
        else
            btnAdd.Enabled = false;
    }

    protected void grdViewList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdViewList, "Select$" + e.Row.RowIndex);
            e.Row.Attributes["onclick"] = "gridSelect('" + e.Row.RowIndex.ToString() + "');";
            e.Row.Attributes.Add("ondblclick", "javascript:return SendValueToParent2('" + e.Row.RowIndex + "')");
        }
    }
    protected void btnRetrieve_Click(object sender, EventArgs e)
    {
        if (lbxValues.Items.Count > 0)
        {
            string retString = "";
            for (int i = 0; i < lbxValues.Items.Count; i++)
                retString += lbxValues.Items[i].Value.ToString() + ", ";
            valueControl.Value = retcontrol;
            valueReturn.Value = retString.Substring(0, retString.Length - 2);
            btnRetrieve.Attributes.Add("OnMouseOver", "javascript:return SendValueToParent();");
        }
    }
}
