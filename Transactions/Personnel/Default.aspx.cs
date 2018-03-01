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

public partial class Transactions_Personnel_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        showControlsOnRights();
        populateDetails();
    }

    private void showControlsOnRights()
    {
        string sql = @" SELECT Ugt_sysmenucode
	                         , Ugt_CanRetrieve
                          FROM T_UserGroupDetail
                         INNER JOIN T_UserGrant
                            ON Ugt_Usergroup = Ugd_usergroupcode
                           AND Ugt_SystemID = Ugd_SystemID
                           AND Ugt_sysmenucode LIKE 'WF%'
                         WHERE Ugd_usercode = @UserCode
                           AND Ugd_SystemID = @SystemID ";
        ParameterInfo[] param = new ParameterInfo[2];
        param[0] = new ParameterInfo("@UserCode", Session["userLogged"].ToString());
        param[1] = new ParameterInfo("@SystemID", "PERSONNEL");
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
                switch (ds.Tables[0].Rows[i]["Ugt_sysmenucode"].ToString())
                {
                    case "WFTAXCODE":
                        btnTaxCivil.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFTAXCIVILREP":
                        btnTaxCivilReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBENEUPDATE":
                        btnBeneficiary.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFBENEREPORT":
                        btnBeneficiaryReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFADDPRES":
                        btnAddressPresent.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFADDPERMA":
                        btnAddressPermanent.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFADDEMER":
                        btnAddressEmergency.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
                        break;
                    case "WFADDREPORT":
                        btnAddressReport.Visible = Convert.ToBoolean(ds.Tables[0].Rows[i]["Ugt_CanRetrieve"].ToString());
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
    }

    private void populateDetails()
    {
        string sql = string.Format(@"   SELECT ADTAX.Adt_AccountDesc [Tax Code]
                                             , ADCIVIL.Adt_AccountDesc [Civil Status]
                                             , Emt_EmployeeAddress1 [Present Address 1]
                                             , ISNULL(ADPRES2.Adt_AccountDesc, '') [Present Address 2]
                                             , ISNULL(ADPRES3.Adt_AccountDesc, '')  [Present Address 3]
                                             , REPLACE(RTRIM(Emt_TelephoneNo), ' ', ' / ') [Present Address Tel No]
                                             , Emt_CellularNo [Present Address Cell No]
                                             , Emt_EmailAddress [Present Address Email]
                                             , Emt_EmployeeProvAddress1 [Permanent Address 1]
                                             , ISNULL(ADPERMA2.Adt_AccountDesc, '') [Permanent Address 2]
                                             , ISNULL(ADPERMA2.Adt_AccountDesc, '') [Permanent Address 3]
                                             , REPLACE(RTRIM(Emt_EmployeeProvTelephoneNo), ' ', ' / ') [Permanent Address Tel No]
                                             , Emt_ContactAddress1 [Emergency Address 1]
                                             , ISNULL(ADICE2.Adt_AccountDesc, '') [Emergency Address 2]
                                             , ISNULL(ADICE2.Adt_AccountDesc, '') [Emergency Address 3]
                                             , Emt_ContactPersonName [Emergency Person Name]
                                             , ISNULL(ADICEREL.Adt_AccountDesc, '') [Emergency Relation]
                                             , REPLACE(RTRIM(Emt_ContactPersonNo), ' ', ' / ') [Emergency Tel No]
                                          FROM T_EmployeeMaster
                                          LEFT JOIN T_AccountDetail ADTAX
                                            ON ADTAX.Adt_AccountCode = Emt_TaxCode
                                           AND ADTAX.Adt_AccountType = 'TAXCODE'
                                          LEFT JOIN T_AccountDetail ADCIVIL
                                            ON ADCIVIL.Adt_AccountCode = Emt_CivilStatus
                                           AND ADCIVIL.Adt_AccountType = 'CIVILSTAT'
                                          LEFT JOIN T_AccountDetail ADPRES2
                                            ON ADPRES2.Adt_AccountCode = Emt_EmployeeAddress2
                                           AND ADPRES2.Adt_AccountType = 'BARANGAY'
                                          LEFT JOIN T_AccountDetail ADPRES3
                                            ON ADPRES3.Adt_AccountCode = Emt_EmployeeAddress3
                                           AND ADPRES3.Adt_AccountType = 'ZIPCODE'
                                          LEFT JOIN T_AccountDetail ADPERMA2
                                            ON ADPERMA2.Adt_AccountCode = Emt_EmployeeProvAddress2
                                           AND ADPERMA2.Adt_AccountType = 'BARANGAY'
                                          LEFT JOIN T_AccountDetail ADPERMA3
                                            ON ADPERMA3.Adt_AccountCode = Emt_EmployeeProvAddress3
                                           AND ADPERMA3.Adt_AccountType = 'ZIPCODE'
                                          LEFT JOIN T_AccountDetail ADICE2
                                            ON ADICE2.Adt_AccountCode = Emt_ContactAddress2
                                           AND ADICE2.Adt_AccountType = 'BARANGAY'
                                          LEFT JOIN T_AccountDetail ADICE3
                                            ON ADICE3.Adt_AccountCode = Emt_ContactAddress3
                                           AND ADICE3.Adt_AccountType = 'ZIPCODE'
                                          LEFT JOIN T_AccountDetail ADICEREL
                                            ON ADICEREL.Adt_AccountCode = Emt_ContactRelation
                                           AND ADICEREL.Adt_AccountType = 'RELATION'
                                         WHERE Emt_EmployeeID = '{0}'", Session["userLogged"].ToString());
        DataSet ds = new DataSet();
        using(DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
            }
            catch
            {

            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (!CommonMethods.isEmpty(ds))
        {
            txtTaxCode.Text = ds.Tables[0].Rows[0]["Tax Code"].ToString();
            txtCivilStatus.Text = ds.Tables[0].Rows[0]["Civil Status"].ToString();

            txtAddressPresent1.Text = ds.Tables[0].Rows[0]["Present Address 1"].ToString();
            txtAddressPresent2.Text = ds.Tables[0].Rows[0]["Present Address 2"].ToString();
            txtAddressPresent3.Text = ds.Tables[0].Rows[0]["Present Address 3"].ToString();
            txtAddressPresentEmail.Text = ds.Tables[0].Rows[0]["Present Address Email"].ToString();
            txtAddressPresentMobileNo.Text = ds.Tables[0].Rows[0]["Present Address Cell No"].ToString();
            txtAddressPresentTelNo.Text = ds.Tables[0].Rows[0]["Present Address Tel No"].ToString();

            txtAddressPermanent1.Text = ds.Tables[0].Rows[0]["Permanent Address 1"].ToString();
            txtAddressPermanent2.Text = ds.Tables[0].Rows[0]["Permanent Address 2"].ToString();
            txtAddressPermanent3.Text = ds.Tables[0].Rows[0]["Permanent Address 3"].ToString();
            txtAddressPermanentTelNo.Text = ds.Tables[0].Rows[0]["Permanent Address Tel No"].ToString();

            txtAddressICE1.Text = ds.Tables[0].Rows[0]["Emergency Address 1"].ToString();
            txtAddressICE2.Text = ds.Tables[0].Rows[0]["Emergency Address 2"].ToString();
            txtAddressICE3.Text = ds.Tables[0].Rows[0]["Emergency Address 3"].ToString();
            txtAddressICEContact.Text = ds.Tables[0].Rows[0]["Emergency Person Name"].ToString();
            txtAddressICERelation.Text = ds.Tables[0].Rows[0]["Emergency Relation"].ToString();
            txtAddressICETelNo.Text = ds.Tables[0].Rows[0]["Emergency Tel No"].ToString();
        }
        
    }
}
