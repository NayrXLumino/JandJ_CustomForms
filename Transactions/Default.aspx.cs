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
using Payroll.DAL;
using MethodsLibrary;


public partial class Transactions_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        showControlsOnRights();
    }

    #region Methods
    private void showControlsOnRights()
    {
        int cntOVERTIME = 0;
        int cntLEAVE = 0;
        int cntTIMEMODIFICATION = 0;
        int cntMOVEMENT = 0;
        int cntJOBSPLT = 0;
        int cntPERSONNEL = 0;
        int cntFLEX = 0;

        string sql = @" SELECT Ugt_sysmenucode [Menu]
                          FROM T_UserGroupDetail
                         INNER JOIN T_UserGrant
                            ON Ugt_Usergroup = Ugd_usergroupcode
                           AND Ugt_SystemID = Ugd_SystemID
                           AND Ugt_sysmenucode LIKE 'WF%'
                           AND Ugt_CanRetrieve = '1'
                           AND Ugt_Status = 'A'
                         WHERE Ugd_usercode = @UserCode";
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                switch (ds.Tables[0].Rows[i]["Menu"].ToString().ToUpper())
                {
                    case "WFADDON": cntMOVEMENT++;
                        break;
                    case "WFADDPERMA": cntPERSONNEL++;
                        break;
                    case "WFADDPRES": cntPERSONNEL++;
                        break;
                    case "WFADEMER": cntPERSONNEL++;
                        break;
                    case "WFBCCUPDATE": cntMOVEMENT++;
                        break;
                    case "WFBCHANGEGROUP": cntMOVEMENT++;
                        break;
                    case "WFBOTENTRY": cntOVERTIME++;
                        break;
                    case "WFBRESTDAY": cntMOVEMENT++;
                        break;
                    case "WFBSHIFTUPDATE": cntMOVEMENT++;
                        break;
                    case "WFCCUPDATE": cntMOVEMENT++;
                        break;
                    case "WFCHANGEGROUP": cntMOVEMENT++;
                        break;
                    case "WFFLXENTRY": cntFLEX++;
                        break;
                    case "WFFLXREP": cntFLEX++;
                        break;
                    case "WFJOBSPLTENTRY": cntJOBSPLT++;
                        break;
                    case "WFJOBSPLTMOD": cntJOBSPLT++;
                        break;
                    case "WFJOBSPLTREP": cntJOBSPLT++;
                        break;
                    case "WFJSENROUTE": cntJOBSPLT++;
                        break;
                    case "WFLVECANCEL": cntLEAVE++;
                        break;
                    case "WFLVEENTRY": cntLEAVE++;
                        break;
                    case "WFLVENOTEENTRY": cntLEAVE++;
                        break;
                    case "WFLVEREP": cntLEAVE++;
                        break;
                    case "WFMANHOURDTL": cntJOBSPLT++;
                        break;
                    case "WFMANHOURREP": cntJOBSPLT++;
                        break;
                    case "WFMVEREP": cntMOVEMENT++;
                        break;
                    case "WFOTENTRY": cntOVERTIME++;
                        break;
                    case "WFOTREP": cntOVERTIME++;
                        break;
                    case "WFRESTDAY": cntMOVEMENT++;
                        break;
                    case "WFSHIFTUPDATE": cntMOVEMENT++;
                        break;
                    case "WFSPLLVEENTRY": cntLEAVE++;
                        break;
                    case "WFSPLOTENTRY": cntOVERTIME++;
                        break;
                    case "WFTAXCIVILREP": cntPERSONNEL++;
                        break;
                    case "WFTAXCODE": cntPERSONNEL++;
                        break;
                    case "WFTIMERECENTRY": cntTIMEMODIFICATION++;
                        break;
                    case "WFTIMERECREP": cntTIMEMODIFICATION++;
                        break;
                    case "WFWORKREC": cntTIMEMODIFICATION++;
                        break;
                    default:
                        break;
                }
            }
            lblNoAccess.Visible = false;
        }
        else
        {
            lblNoAccess.Visible = true;
        }

        btnOvertime.Visible = (cntOVERTIME > 0);
        btnLeave.Visible = (cntLEAVE > 0);
        btnTimeRecord.Visible = (cntTIMEMODIFICATION > 0);
        btnPersonnel.Visible = (cntPERSONNEL > 0);
        btnMovement.Visible = (cntMOVEMENT > 0);
    }
    #endregion
}
