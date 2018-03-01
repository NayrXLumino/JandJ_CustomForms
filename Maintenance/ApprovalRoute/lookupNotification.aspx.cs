using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;

public partial class Maintenance_ApprovalRoute_lookupNotification : System.Web.UI.Page
{
    GeneralBL GNBL = new GeneralBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            initializeControls();
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
            Page.PreRender += new EventHandler(Page_PreRender);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    GNBL.InserUpdateNotification(PopulateDR(), Session["userLogged"].ToString(), dal);
                    MessageBox.Show("Successfully saved changes");
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
    }
    #endregion

    #region Methods
    private void initializeControls()
    {
        string sql = string.Format(@"  SELECT CASE WHEN Arm_NotifyEndorse    = 1 THEN 'TRUE' ELSE 'FALSE' END [Endorse]
                                            , CASE WHEN Arm_NotifyApprove    = 1 THEN 'TRUE' ELSE 'FALSE' END [Approve]
                                            , CASE WHEN Arm_NotifyReturn     = 1 THEN 'TRUE' ELSE 'FALSE' END [Return]
                                            , CASE WHEN Arm_NotifyDisapprove = 1 THEN 'TRUE' ELSE 'FALSE' END [Disapprove]
                                            , ISNULL(Tcm_TransactionDesc, 'NOTIFICATION') [Description]
                                         FROM T_EmployeeApprovalRoute
                                         LEFT JOIN T_TransactionControlMaster
                                           ON Tcm_TransactionCode = Arm_TransactionID
                                        WHERE Arm_EmployeeId = '{0}'
                                          AND Arm_TransactionID = '{1}' 
                                          AND Convert(varchar,GETDATE(),101) BETWEEN Arm_StartDate AND ISNULL(Arm_EndDate, GETDATE())", Encrypt.decryptText(Request.QueryString["ei"].ToString()), Encrypt.decryptText(Request.QueryString["tp"].ToString()));
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
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

        if (!CommonMethods.isEmpty(ds))
        {
            cbxNotify.Items[0].Selected = Convert.ToBoolean(ds.Tables[0].Rows[0]["Endorse"].ToString());
            cbxNotify.Items[1].Selected = Convert.ToBoolean(ds.Tables[0].Rows[0]["Approve"].ToString());
            cbxNotify.Items[2].Selected = Convert.ToBoolean(ds.Tables[0].Rows[0]["Return"].ToString());
            cbxNotify.Items[3].Selected = Convert.ToBoolean(ds.Tables[0].Rows[0]["Disapprove"].ToString());
            lblNotification.Text = ds.Tables[0].Rows[0]["Description"].ToString();
        }
        else
        {
            cbxNotify.Items[0].Selected = false;
            cbxNotify.Items[1].Selected = false;
            cbxNotify.Items[2].Selected = false;
            cbxNotify.Items[3].Selected = false;
        }
    }

    private DataRow PopulateDR()
    {
        DataRow dr = DbRecord.Generate("T_EmployeeApprovalRoute");
        dr["Arm_EmployeeId"] = Encrypt.decryptText(Request.QueryString["ei"]);
        dr["Arm_TransactionID"] = Encrypt.decryptText(Request.QueryString["tp"]);
        dr["Arm_NotifyEndorse"] = cbxNotify.Items[0].Selected.ToString();
        dr["Arm_NotifyApprove"] = cbxNotify.Items[1].Selected.ToString();
        dr["Arm_NotifyReturn"] = cbxNotify.Items[2].Selected.ToString();
        dr["Arm_NotifyDisapprove"] = cbxNotify.Items[3].Selected.ToString();
        return dr;
    }
    #endregion
}