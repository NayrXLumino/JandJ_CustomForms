using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Payroll.DAL;
using CommonLibrary;
using MethodsLibrary;

public partial class pgeLookupJob : System.Web.UI.Page
{
    #region [Class Variables]
    DataSet dsUser = new DataSet();
    DataSet dsShift = new DataSet();
    DataSet dsLogLedger = new DataSet();
    DataSet dsSplit = new DataSet();
    string strShift = string.Empty;
    DataSet dsJobs = new DataSet();
    string DayCode = string.Empty;
    private JobSplitBL JSBL = new JobSplitBL();
    string flagEntry = string.Empty;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadJobSplitEntry();
    }

    #region LoadComplete
    protected void _Default_LoadComplete(object sender, EventArgs e)
    {
    }
    #endregion

    #region Load the Job Split entry from checklist or pending
    protected void LoadJobSplitEntry()
    {
        if (Request.RawUrl.Contains("cn"))
        {
            try
            {
                string controlNum = Request.QueryString["cn"].ToString();
                this.txtControlNo.Text = controlNum;
                if (!controlNum.Equals(string.Empty))
                {
                    DataRow dr = JSBL.GetJobSplitHeaderInfo(controlNum);
                    DataSet dsLoad = JSBL.LoadCurrentJobSplitDetails(controlNum);

                    Session["userId"] = dr["Jsh_EmployeeId"].ToString();
                    if (dsLoad.Tables[0] != null && dsLoad.Tables[0].Rows.Count > 0)
                    {
                        flagEntry = "C";
                        string v1 = string.Empty;
                        string v2 = string.Empty;
                        string v3 = string.Empty;
                        string v4 = string.Empty;
                        string v5 = string.Empty;
                        string v6 = string.Empty;
                        string v7 = string.Empty;

                        //Kelvin added: 20110415 - added v8 and v9 for billable and overtime
                        string v8 = string.Empty;
                        string v9 = string.Empty;

                        int max = dsLoad.Tables[0].Rows.Count;

                        for (int ctr = 0; ctr < max; ctr++)
                        {
                            v1 = v1 + dsLoad.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(0, 2) + ":" + dsLoad.Tables[0].Rows[ctr]["Jsd_StartTime"].ToString().Substring(2, 2) + ",";
                            v2 = v2 + dsLoad.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(0, 2) + ":" + dsLoad.Tables[0].Rows[ctr]["Jsd_EndTime"].ToString().Substring(2, 2) + ",";
                            v3 = v3 + Convert.ToString(Convert.ToDecimal(dsLoad.Tables[0].Rows[ctr]["Jsd_PlanHours"])) + ",";
                            v4 = v4 + dsLoad.Tables[0].Rows[ctr]["Jsd_JobCode"].ToString() + ",";
                            v5 = v5 + dsLoad.Tables[0].Rows[ctr]["Jsd_ClientJobNo"].ToString() + ",";
                            v6 = v6 + dsLoad.Tables[0].Rows[ctr]["Slm_ClientJobName"].ToString() + ",";
                            v7 = v7 + dsLoad.Tables[0].Rows[ctr]["Jsd_SubWorkCode"].ToString() + ",";
                            v8 = v8 + dsLoad.Tables[0].Rows[ctr]["Jsd_Category"].ToString() + ",";
                            v9 = v9 + dsLoad.Tables[0].Rows[ctr]["Jsd_Overtime"].ToString() + ",";
                        }

                        //Kelvin commented below lines of code: 20110415 - Javascript/JobSplitLookup.js Manipulate() will handle this
                        //v1 = v1.Substring(0, v1.Length - 1);
                        //v2 = v2.Substring(0, v2.Length - 1);
                        //v3 = v3.Substring(0, v3.Length - 1);
                        //v4 = v4.Substring(0, v4.Length - 1);
                        //v5 = v5.Substring(0, v5.Length - 1);
                        //v6 = v6.Substring(0, v6.Length - 1);
                        //v7 = v7.Substring(0, v7.Length - 1);
                        //v8 = v8.Substring(0, v8.Length - 1);
                        //v9 = v9.Substring(0, v9.Length - 1);

                        //Assigning the values to the page
                        txtJStart.Text = v1;
                        txtJEnd.Text = v2;
                        txtJHours.Text = v3;
                        txtJJobCode.Text = v4;
                        txtJClientNo.Text = v5;
                        txtJClientName.Text = v6;
                        txtSubWork.Text = v7;
                        hBillable.Value = v8;
                        hOvertime.Value = v9;

                        hiddenFlag.Value = dsLoad.Tables[0].Rows[0]["Jsh_Status"].ToString();
                        hiddenControl.Value = dsLoad.Tables[0].Rows[0]["Jsh_ControlNo"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Invalid control number retrieve. Try again.");
                    }
                }
                else
                {
                    MessageBox.Show("Could not retrieve control number.");
                }
                hiddenDFlag.Value = "0";
            }
            catch (Exception ex)
            {
                //Do Nothing
            }
        }
    }
    #endregion
}
