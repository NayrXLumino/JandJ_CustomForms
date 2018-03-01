using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Payroll.DAL;
using CommonLibrary;

public partial class Maintenance_ApprovalRoute_pgeEmployeeApprovalRoute : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    GeneralBL GNBL = new GeneralBL();
    MenuGrant MGBL = new MenuGrant();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFROUTEENTRY"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                initializeEmployee();
                initializeControls();
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                Page.PreRender += new EventHandler(Page_PreRender);
            }
            else
            {
                if (!hfPrevEmployeeId.Value.Equals(txtEmployeeId.Text))
                {
                    if (!hfPrevEntry.Value.Equals(changeSnapShot()))
                    {
                        MessageBox.Show("There were some changes made and was not saved.");
                    }
                    txtEmployeeId_TextChanged(txtEmployeeId, new EventArgs());
                    hfEncryptId.Value = Encrypt.encryptText(txtEmployeeId.Text);
                    hfPrevEmployeeId.Value = txtEmployeeId.Text;
                }
            }
            LoadComplete += new EventHandler(Maintenance_ApprovalRoute_pgeEmployeeApprovalRoute_LoadComplete);
        }
    }

    #region Events
    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    void Maintenance_ApprovalRoute_pgeEmployeeApprovalRoute_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "approvalrouteScripts";
        string jsurl = "_approvalroute.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }

        #region Read Only Controls
        txtEmployeeId.Attributes.Add("readOnly", "true");
        txtEmployeeName.Attributes.Add("readOnly", "true");
        txtNickname.Attributes.Add("readOnly", "true");

        txtCostCenterAssignment.Attributes.Add("readOnly", "true");
        txtAddress.Attributes.Add("readOnly", "true");
        txtAddressAP.Attributes.Add("readOnly", "true");
        txtAddressC1.Attributes.Add("readOnly", "true");
        txtAddressC2.Attributes.Add("readOnly", "true");
        txtBeneficiary.Attributes.Add("readOnly", "true");
        txtBeneficiaryAP.Attributes.Add("readOnly", "true");
        txtBeneficiaryC1.Attributes.Add("readOnly", "true");
        txtBeneficiaryC2.Attributes.Add("readOnly", "true");
        txtFlexTime.Attributes.Add("readOnly", "true");
        txtFlexTimeAP.Attributes.Add("readOnly", "true");
        txtFlexTimeC1.Attributes.Add("readOnly", "true");
        txtFlexTimeC2.Attributes.Add("readOnly", "true");
        txtLeave.Attributes.Add("readOnly", "true");
        txtLeaveAP.Attributes.Add("readOnly", "true");
        txtLeaveC1.Attributes.Add("readOnly", "true");
        txtLeaveC2.Attributes.Add("readOnly", "true");
        txtManHour.Attributes.Add("readOnly", "true");
        txtManHourAP.Attributes.Add("readOnly", "true");
        txtManHourC1.Attributes.Add("readOnly", "true");
        txtManHourC2.Attributes.Add("readOnly", "true");
        txtOvertime.Attributes.Add("readOnly", "true");
        txtOvertimeAP.Attributes.Add("readOnly", "true");
        txtOvertimeC1.Attributes.Add("readOnly", "true");
        txtOvertimeC2.Attributes.Add("readOnly", "true");
        txtStraightWork.Attributes.Add("readOnly", "true");
        txtStraightWorkAP.Attributes.Add("readOnly", "true");
        txtStraightWorkC1.Attributes.Add("readOnly", "true");
        txtStraightWorkC2.Attributes.Add("readOnly", "true");
        txtTaxCivil.Attributes.Add("readOnly", "true");
        txtTaxCivilAP.Attributes.Add("readOnly", "true");
        txtTaxCivilC1.Attributes.Add("readOnly", "true");
        txtTaxCivilC2.Attributes.Add("readOnly", "true");
        txtTimeRecord.Attributes.Add("readOnly", "true");
        txtTimeRecordAP.Attributes.Add("readOnly", "true");
        txtTimeRecordC1.Attributes.Add("readOnly", "true");
        txtTimeRecordC2.Attributes.Add("readOnly", "true");
        txtWorkInfo.Attributes.Add("readOnly", "true");
        txtWorkInfoAP.Attributes.Add("readOnly", "true");
        txtWorkInfoC1.Attributes.Add("readOnly", "true");
        txtWorkInfoC2.Attributes.Add("readOnly", "true");
        txtManPower.Attributes.Add("readOnly", "true");
        txtManPowerAP.Attributes.Add("readOnly", "true");
        txtManPowerC1.Attributes.Add("readOnly", "true");
        txtManPowerC2.Attributes.Add("readOnly", "true");
        txtTraining.Attributes.Add("readOnly", "true");
        txtTrainingAP.Attributes.Add("readOnly", "true");
        txtTrainingC1.Attributes.Add("readOnly", "true");
        txtTrainingC2.Attributes.Add("readOnly", "true");
        TextBox txtTemp = (TextBox)dtpOTStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpOTEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpLVStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpLVEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpSWStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpSWEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpFXStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpFXEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpWIStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpWIEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpBenStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpBenEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpAddStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpAddEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTCStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTCEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpMPStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpMPEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTrainStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTrainEndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTRStartDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        txtTemp = (TextBox)dtpTREndDate.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        #endregion

        btnTraining.OnClientClick = string.Format("return lookupARRouteAssignment('txtTraining')");
        btnManPower.OnClientClick = string.Format("return lookupARRouteAssignment('txtManPower')");
        btnAddress.OnClientClick = string.Format("return lookupARRouteAssignment('txtAddress')");
        btnBeneficiary.OnClientClick = string.Format("return lookupARRouteAssignment('txtBeneficiary')");
        btnFlexTime.OnClientClick = string.Format("return lookupARRouteAssignment('txtFlexTime')");
        btnLeave.OnClientClick = string.Format("return lookupARRouteAssignment('txtLeave')");
        btnManHour.OnClientClick = string.Format("return lookupARRouteAssignment('txtManHour')");
        btnOvertime.OnClientClick = string.Format("return lookupARRouteAssignment('txtOvertime')");
        btnStraightWork.OnClientClick = string.Format("return lookupARRouteAssignment('txtStraightWork')");
        btnTaxCivil.OnClientClick = string.Format("return lookupARRouteAssignment('txtTaxCivil')");
        btnTimeRecord.OnClientClick = string.Format("return lookupARRouteAssignment('txtTimeRecord')");
        btnWorkInfo.OnClientClick = string.Format("return lookupARRouteAssignment('txtWorkInfo')");

        btnEmployeeId.OnClientClick = string.Format("return lookupEmployeeApprovalRoute('GENERAL')");

        btnOTNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("OVERTIME"), Encrypt.encryptText(txtEmployeeId.Text));
        btnLVNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("LEAVE"), Encrypt.encryptText(txtEmployeeId.Text));
        btnTRNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("TIMEMOD"), Encrypt.encryptText(txtEmployeeId.Text));
        btnSWNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("STRAIGHTWORK"), Encrypt.encryptText(txtEmployeeId.Text));
        btnFTNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("FLEXTIME"), Encrypt.encryptText(txtEmployeeId.Text));
        btnJSNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("JOBSPLIT"), Encrypt.encryptText(txtEmployeeId.Text));
        btnMVNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("MOVEMENT"), Encrypt.encryptText(txtEmployeeId.Text));
        btnADNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("ADDRESS"), Encrypt.encryptText(txtEmployeeId.Text));
        btnBFNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("BNEFICIARY"), Encrypt.encryptText(txtEmployeeId.Text));
        btnTXNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("TAXMVMNT"), Encrypt.encryptText(txtEmployeeId.Text));
        btnMPNotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("MANPOWER"), Encrypt.encryptText(txtEmployeeId.Text));
        btnTRANotify.OnClientClick = string.Format("return lookupNotification('{0}', '{1}')", Encrypt.encryptText("TRAINING"), Encrypt.encryptText(txtEmployeeId.Text));
    }

    protected void txtEmployeeId_TextChanged(object sender, EventArgs e)
    {
        initializeControls();
        setCostcenterAssignment(txtEmployeeId.Text);
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            string transactionUpdated = string.Empty;
            string startDate = string.Empty;
            string endDate = string.Empty;

            string errMsg = checkEntry();
            if (errMsg.Equals(string.Empty))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        dal.BeginTransactionSnapshot();

                        #region OVERTIME
                        if (!hfOvertime.Value.Equals(txtOvertime.Text)
                            || !hfOTStart.Value.Equals(dtpOTStartDate.DateString)
                            || !hfOTEnd.Value.Equals(dtpOTEndDate.DateString))
                        {
                            startDate = dtpOTStartDate.DateString == string.Empty ? string.Empty : dtpOTStartDate.DateString;
                            endDate = dtpOTEndDate.DateString == string.Empty ? string.Empty : dtpOTEndDate.DateString;
                            if (txtOvertime.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtOvertime.Text, "OVERTIME", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtOvertime.Text, hfOvertime.Value, "OVERTIME", Session["userLogged"].ToString(), dal);
                                }
                                hfOvertime.Value = txtOvertime.Text;
                                hfOTStart.Value = dtpOTStartDate.DateString;
                                hfOTEnd.Value = dtpOTEndDate.DateString;
                                transactionUpdated += "\n  Overtime";
                            }
                        }
                        #endregion
                        #region LEAVE
                        if (!hfLeave.Value.Equals(txtLeave.Text)
                            || !hfLVStart.Value.Equals(dtpLVStartDate.DateString)
                            || !hfLVEnd.Value.Equals(dtpLVEndDate.DateString))
                        {
                            startDate = dtpLVStartDate.DateString == string.Empty ? string.Empty : dtpLVStartDate.DateString;
                            endDate = dtpLVEndDate.DateString == string.Empty ? string.Empty : dtpLVEndDate.DateString;
                            if (txtLeave.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtLeave.Text, "LEAVE", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtLeave.Text, "LEAVE", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtLeave.Text, "LEAVE", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtLeave.Text, hfLeave.Value, "LEAVE", Session["userLogged"].ToString(), dal);
                                }
                                hfLeave.Value = txtLeave.Text;
                                hfLVStart.Value = dtpLVStartDate.DateString;
                                hfLVEnd.Value = dtpLVEndDate.DateString;
                                transactionUpdated += "\n  Leave";
                            }
                        }
                        #endregion
                        #region TIME RECORD
                        if (!hfTimeRecord.Value.Equals(txtTimeRecord.Text)
                            || !hfTRStart.Value.Equals(dtpTRStartDate.DateString)
                            || !hfTREnd.Value.Equals(dtpTREndDate.DateString))
                        {
                            startDate = dtpTRStartDate.DateString == string.Empty ? string.Empty : dtpTRStartDate.DateString;
                            endDate = dtpTREndDate.DateString == string.Empty ? string.Empty : dtpTREndDate.DateString;
                            if (txtTimeRecord.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtTimeRecord.Text, "TIMEMOD", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTimeRecord.Text, "TIMEMOD", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtTimeRecord.Text, "TIMEMOD", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTimeRecord.Text, hfTimeRecord.Value, "TIMEMOD", Session["userLogged"].ToString(), dal);
                                }
                                hfTimeRecord.Value = txtTimeRecord.Text;
                                hfTRStart.Value = dtpTRStartDate.DateString;
                                hfTREnd.Value = dtpTREndDate.DateString;
                                transactionUpdated += "\n  TimeRecord";
                            }
                        }
                        #endregion
                        #region STRAIGHT WORK
                        if (!hfStraightWork.Value.Equals(txtStraightWork.Text)
                            || !hfSWStart.Value.Equals(dtpSWStartDate.DateString)
                            || !hfSWEnd.Value.Equals(dtpSWEndDate.DateString))
                        {
                            startDate = dtpSWStartDate.DateString == string.Empty ? null : dtpSWStartDate.DateString;
                            endDate = dtpSWEndDate.DateString == string.Empty ? null : dtpSWEndDate.DateString;
                            if (txtStraightWork.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtStraightWork.Text, "STRAIGHTWK", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtStraightWork.Text, "STRAIGHTWK", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtStraightWork.Text, "STRAIGHTWK", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtStraightWork.Text, hfStraightWork.Value, "STRAIGHTWK", Session["userLogged"].ToString(), dal);
                                }
                                hfStraightWork.Value = txtStraightWork.Text;
                                hfSWStart.Value = dtpSWStartDate.DateString;
                                hfSWEnd.Value = dtpSWEndDate.DateString;
                                transactionUpdated += "\n  Straight Work";
                            }
                        }
                        #endregion
                        #region MAN HOUR
                        if (!hfManHour.Value.Equals(txtManHour.Text)
                            || !hfMHStart.Value.Equals(dtpMHStartDate.DateString)
                            || !hfMHEnd.Value.Equals(dtpMHEndDate.DateString))
                        {
                            startDate = dtpMHStartDate.DateString == string.Empty ? string.Empty : dtpMHStartDate.DateString;
                            endDate = dtpMHEndDate.DateString == string.Empty ? string.Empty : dtpMHEndDate.DateString;
                            if (txtManHour.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtManHour.Text, "JOBMOD", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtManHour.Text, "JOBMOD", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtManHour.Text, "JOBMOD", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtManHour.Text, hfManHour.Value, "JOBMOD", Session["userLogged"].ToString(), dal);
                                }
                                hfManHour.Value = txtManHour.Text;
                                hfMHStart.Value = dtpMHStartDate.DateString;
                                hfMHEnd.Value = dtpMHEndDate.DateString;
                                transactionUpdated += "\n  ManHour";
                            }
                        }
                        #endregion
                        #region BENEFICIARY
                        if (!hfBeneficiary.Value.Equals(txtBeneficiary.Text)
                            || !hfBenStart.Value.Equals(dtpBenStartDate.DateString)
                            || !hfBenEnd.Value.Equals(dtpBenEndDate.DateString))
                        {
                            startDate = dtpBenStartDate.DateString == string.Empty ? string.Empty : dtpBenStartDate.DateString;
                            endDate = dtpBenEndDate.DateString == string.Empty ? string.Empty : dtpBenEndDate.DateString;
                            if (txtBeneficiary.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtBeneficiary.Text, "BNEFICIARY", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtBeneficiary.Text, "BNEFICIARY", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtBeneficiary.Text, "BNEFICIARY", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtBeneficiary.Text, hfBeneficiary.Value, "BNEFICIARY", Session["userLogged"].ToString(), dal);
                                }
                                hfBeneficiary.Value = txtBeneficiary.Text;
                                hfBenStart.Value = dtpBenStartDate.DateString;
                                hfBenEnd.Value = dtpBenEndDate.DateString;
                                transactionUpdated += "\n  Beneficiary";
                            }
                        }
                        #endregion
                        #region ADDRESS
                        if (!hfAddress.Value.Equals(txtAddress.Text)
                            || !hfAddStart.Value.Equals(dtpAddStartDate.DateString)
                            || !hfAddEnd.Value.Equals(dtpAddEndDate.DateString))
                        {
                            startDate = dtpAddStartDate.DateString == string.Empty ? string.Empty : dtpAddStartDate.DateString;
                            endDate = dtpAddEndDate.DateString == string.Empty ? string.Empty : dtpAddEndDate.DateString;
                            if (txtAddress.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtAddress.Text, "ADDRESS", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtAddress.Text, "ADDRESS", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtAddress.Text, "ADDRESS", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtAddress.Text, hfAddress.Value, "ADDRESS", Session["userLogged"].ToString(), dal);
                                }
                                hfAddress.Value = txtAddress.Text;
                                hfAddStart.Value = dtpAddStartDate.DateString;
                                hfAddEnd.Value = dtpAddEndDate.DateString;
                                transactionUpdated += "\n  Address";
                            }
                        }
                        #endregion
                        #region WORK INFORMATION
                        if (!hfWorkInfo.Value.Equals(txtWorkInfo.Text)
                            || !hfWIStart.Value.Equals(dtpWIStartDate.DateString)
                            || !hfWIEnd.Value.Equals(dtpWIEndDate.DateString))
                        {
                            startDate = dtpWIStartDate.DateString == string.Empty ? string.Empty : dtpWIStartDate.DateString;
                            endDate = dtpWIEndDate.DateString == string.Empty ? string.Empty : dtpWIEndDate.DateString;
                            if (txtWorkInfo.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtWorkInfo.Text, "MOVEMENT", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtWorkInfo.Text, "MOVEMENT", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtWorkInfo.Text, "MOVEMENT", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtWorkInfo.Text, hfWorkInfo.Value, "MOVEMENT", Session["userLogged"].ToString(), dal);
                                }
                                hfWorkInfo.Value = txtWorkInfo.Text;
                                hfWIStart.Value = dtpWIStartDate.DateString;
                                hfWIEnd.Value = dtpWIEndDate.DateString;
                                transactionUpdated += "\n  WorkInfo";
                            }
                        }
                        #endregion
                        #region TAXCODE/CIVIL STATUS
                        if (!hfTaxCivil.Value.Equals(txtTaxCivil.Text)
                            || !hfTCStart.Value.Equals(dtpTCStartDate.DateString)
                            || !hfTCEnd.Value.Equals(dtpTCEndDate.DateString))
                        {
                            startDate = dtpTCStartDate.DateString == string.Empty ? string.Empty : dtpTCStartDate.DateString;
                            endDate = dtpTCEndDate.DateString == string.Empty ? string.Empty : dtpTCEndDate.DateString;
                            if (txtTaxCivil.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtTaxCivil.Text, "TAXMVMNT", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTaxCivil.Text, "TAXMVMNT", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtTaxCivil.Text, "TAXMVMNT", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTaxCivil.Text, hfTaxCivil.Value, "TAXMVMNT", Session["userLogged"].ToString(), dal);
                                }
                                hfTaxCivil.Value = txtTaxCivil.Text;
                                hfTCStart.Value = dtpTCStartDate.DateString;
                                hfTCEnd.Value = dtpTCEndDate.DateString;
                                transactionUpdated += "\n  TaxCivil";
                            }
                        }
                        #endregion
                        #region MANPOWER
                        if (!hfManPower.Value.Equals(txtManPower.Text)
                            || !hfMPStart.Value.Equals(dtpMPStartDate.DateString)
                            || !hfMPEnd.Value.Equals(dtpMPEndDate.DateString))
                        {
                            startDate = dtpMPStartDate.DateString == string.Empty ? string.Empty : dtpMPStartDate.DateString;
                            endDate = dtpMPEndDate.DateString == string.Empty ? string.Empty : dtpMPEndDate.DateString;
                            if (txtManPower.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtManPower.Text, "MANPOWER", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtManPower.Text, "MANPOWER", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtManPower.Text, "MANPOWER", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtManPower.Text, hfManPower.Value, "MANPOWER", Session["userLogged"].ToString(), dal);
                                }
                                hfManPower.Value = txtManPower.Text;
                                hfMPStart.Value = dtpMPStartDate.DateString;
                                hfMPEnd.Value = dtpMPEndDate.DateString;
                                transactionUpdated += "\n  ManPower";
                            }
                        }
                        #endregion
                        #region TRAINING
                        if (!hfTraining.Value.Equals(txtTraining.Text)
                            || !hfTrainStart.Value.Equals(dtpTrainStartDate.DateString)
                            || !hfTrainEnd.Value.Equals(dtpTrainEndDate.DateString))
                        {
                            startDate = dtpTrainStartDate.DateString == string.Empty ? string.Empty : dtpTrainStartDate.DateString;
                            endDate = dtpTrainEndDate.DateString == string.Empty ? string.Empty : dtpTrainEndDate.DateString;
                            if (txtTraining.Text != string.Empty && startDate != string.Empty)
                            {
                                try
                                {
                                    //GNBL.InsertEmployeeRoute(txtEmployeeId.Text, txtTraining.Text, "TRAINING", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTraining.Text, "TRAINING", Session["userLogged"].ToString(), dal);
                                }
                                catch//PK error meaning transation exists
                                {
                                    //GNBL.UpdateEmployeeRoute(txtEmployeeId.Text, txtTraining.Text, "TRAINING", Session["userLogged"].ToString(), startDate, endDate, dal);
                                    //GNBL.InsertEmployeeRouteTrail(txtEmployeeId.Text, txtTraining.Text, hfTraining.Value, "TRAINING", Session["userLogged"].ToString(), dal);
                                }
                                hfTraining.Value = txtTraining.Text;
                                hfTrainStart.Value = dtpTrainStartDate.DateString;
                                hfTrainEnd.Value = dtpTrainEndDate.DateString;
                                transactionUpdated += "\n  Training";
                            }
                        }
                        #endregion
                        if (!transactionUpdated.Equals(string.Empty))
                        {
                            MessageBox.Show("Successfully updated route(s):" + transactionUpdated);
                        }
                        hfPrevEntry.Value = changeSnapShot();
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
            else
            {
                MessageBox.Show(errMsg);
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Save

    protected void btnY_Click(object sender, EventArgs e)
    {
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            Response.Redirect(Request.RawUrl);
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }//Cancel

    #endregion

    #region Methods
    //Perth Added 11/29/2012
    private string LoopQueryThroughProfile(string Query)
    {
        string ret = string.Empty;

        using (DALHelper dal = new DALHelper(false))
        {
            try
            {
                dal.OpenDB();
                DataTable dt = dal.ExecuteDataSet(@"
SELECT 
	Prf_Database
FROM PROFILE..T_PRofiles
WHERE Prf_Status = 'A'
AND Prf_ProfileType IN ('P', 'S', 'T')
").Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0)
                        ret += " UNION ";
                    ret += Query.Replace("@PROFILE", dt.Rows[i]["Prf_Database"].ToString().Trim());
                }
            }
            catch(Exception er)
            {
                CommonMethods.ErrorsToTextFile(er, "LoopQueryThroughProfile()");
                ret = Query;
            }
            finally
            {
                dal.CloseDB();
            }
        }

        return ret;
    }

    private void initializeEmployee()
    {
        DataSet ds = new DataSet();

        //Perth Modified 11/29/2012
        string sql = @"  
SELECT Emt_EmployeeId [ID No]
    , Emt_NickName [Nickname]
    , Emt_Lastname [Lastname]
    , Emt_Firstname [Firstname] 
	, dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter]
	, ISNULL(Dcm_Departmentdesc, '') [Department]
FROM T_EmployeeMaster
LEFT JOIN T_DepartmentCodeMaster
	ON Dcm_Departmentcode = CASE WHEN (LEN(Emt_CostcenterCode) >= 4)
								THEN RIGHT(LEFT(Emt_CostcenterCode, 4), 2)
								ELSE ''
							END
WHERE Emt_EmployeeId = @EmployeeId

UNION

SELECT
	Umt_Usercode [ID No]
    , Umt_userfname [Nickname]
    , Umt_userlname [Lastname]
    , Umt_userfname [Firstname] 
	, '' [Costcenter]
	, '' [Department]
FROM T_UserMaster
WHERE Umt_Usercode = @EmployeeId
                    ";

        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@EmployeeId", Session["userLogged"].ToString());
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
        lblNickName.Text = Resources.Resource._3RDINFO;
        if (!CommonMethods.isEmpty(ds))
        {
            txtEmployeeId.Text = ds.Tables[0].Rows[0]["ID No"].ToString();
            hfEncryptId.Value = Encrypt.encryptText(txtEmployeeId.Text);
            txtEmployeeName.Text = ds.Tables[0].Rows[0]["Lastname"].ToString()
                                 + ", "
                                 + ds.Tables[0].Rows[0]["Firstname"].ToString();
            //txtNickname.Text = ds.Tables[0].Rows[0]["Nickname"].ToString();
            txtNickname.Text = ds.Tables[0].Rows[0][Resources.Resource._3RDINFO].ToString();
            txtCostCenterAssignment.Text = ds.Tables[0].Rows[0]["Costcenter"].ToString();
        }
        else
        {
            txtEmployeeId.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;
            txtNickname.Text = string.Empty;
            txtCostCenterAssignment.Text = string.Empty;
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFROUTEENTRY");
        btnEmployeeId.Enabled = userGrant.canAdd();

        btnAddress.Enabled = userGrant.canEdit();
        btnBeneficiary.Enabled = userGrant.canEdit();
        btnLeave.Enabled = userGrant.canEdit();
        btnManHour.Enabled = userGrant.canEdit();
        btnOvertime.Enabled = userGrant.canEdit();
        btnStraightWork.Enabled = userGrant.canEdit();
        btnTaxCivil.Enabled = userGrant.canEdit();
        btnTimeRecord.Enabled = userGrant.canEdit();
        btnWorkInfo.Enabled = userGrant.canEdit();
        btnManPower.Enabled = userGrant.canEdit();
        btnTraining.Enabled = userGrant.canEdit();
        btnY.Visible = userGrant.canEdit();
        btnX.Visible = userGrant.canEdit();

        hfPrevEmployeeId.Value = txtEmployeeId.Text;      
        initializeRouteAssignment();
        showRowTransactionUsed();
        hfPrevEntry.Value = changeSnapShot();
        //SetMinDate();
    }

    private void setCostcenterAssignment(string employeeID)
    {
        string sql = @" SELECT dbo.getCostCenterFullNameV2(Emt_CostcenterCode) [Costcenter] 
                          FROM T_EmployeeMaster
                         WHERE Emt_EmployeeID = '{0}' ";
        DataSet ds = new DataSet();
        using (DALHelper dal = new DALHelper())
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(string.Format(sql, employeeID), CommandType.Text);
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
            txtCostCenterAssignment.Text = ds.Tables[0].Rows[0]["Costcenter"].ToString();
        }
        else
        {
            txtCostCenterAssignment.Text = string.Empty;
        }
    }

    private void initializeRouteAssignment()
    {
        DataSet ds = new DataSet();
        ds = GNBL.GetEmployeeRouteAssignment(txtEmployeeId.Text);
        #region Clear Route Details
        txtOvertime.Text = string.Empty;
        hfOvertime.Value = string.Empty;
        txtOvertimeC1.Text = string.Empty;
        txtOvertimeC2.Text = string.Empty;
        txtOvertimeAP.Text = string.Empty;
        dtpOTEndDate.Reset();
        dtpOTStartDate.Reset();

        txtLeave.Text = string.Empty;
        hfLeave.Value = string.Empty;
        txtLeaveC1.Text = string.Empty;
        txtLeaveC2.Text = string.Empty;
        txtLeaveAP.Text = string.Empty;
        dtpLVStartDate.Reset();
        dtpLVEndDate.Reset();

        txtTimeRecord.Text = string.Empty;
        hfTimeRecord.Value = string.Empty;
        txtTimeRecordC1.Text = string.Empty;
        txtTimeRecordC2.Text = string.Empty;
        txtTimeRecordAP.Text = string.Empty;
        dtpTRStartDate.Reset();
        dtpTREndDate.Reset();

        txtStraightWork.Text = string.Empty;
        hfStraightWork.Value = string.Empty;
        txtStraightWorkC1.Text = string.Empty;
        txtStraightWorkC2.Text = string.Empty;
        txtStraightWorkAP.Text = string.Empty;
        dtpSWStartDate.Reset();
        dtpSWEndDate.Reset();

        txtManHour.Text = string.Empty;
        hfManHour.Value = string.Empty;
        txtManHourC1.Text = string.Empty;
        txtManHourC2.Text = string.Empty;
        txtManHourAP.Text = string.Empty;
        dtpMHStartDate.Reset();
        dtpMHEndDate.Reset();

        txtFlexTime.Text = string.Empty;
        hfFlexTime.Value = string.Empty;
        txtFlexTimeC1.Text = string.Empty;
        txtFlexTimeC2.Text = string.Empty;
        txtFlexTimeAP.Text = string.Empty;
        dtpFXStartDate.Reset();
        dtpFXEndDate.Reset();

        txtWorkInfo.Text = string.Empty;
        hfWorkInfo.Value = string.Empty;
        txtWorkInfoC1.Text = string.Empty;
        txtWorkInfoC2.Text = string.Empty;
        txtWorkInfoAP.Text = string.Empty;
        dtpWIStartDate.Reset();
        dtpWIEndDate.Reset();

        txtBeneficiary.Text = string.Empty;
        hfBeneficiary.Value = string.Empty;
        txtBeneficiaryC1.Text = string.Empty;
        txtBeneficiaryC2.Text = string.Empty;
        txtBeneficiaryAP.Text = string.Empty;
        dtpBenStartDate.Reset();
        dtpBenEndDate.Reset();

        txtAddress.Text = string.Empty;
        hfAddress.Value = string.Empty;
        txtAddressC1.Text = string.Empty;
        txtAddressC2.Text = string.Empty;
        txtAddressAP.Text = string.Empty;
        dtpAddStartDate.Reset();
        dtpAddEndDate.Reset();

        txtTaxCivil.Text = string.Empty;
        hfTaxCivil.Value = string.Empty;
        txtTaxCivilC1.Text = string.Empty;
        txtTaxCivilC2.Text = string.Empty;
        txtTaxCivilAP.Text = string.Empty;
        dtpTCStartDate.Reset();
        dtpTCEndDate.Reset();

        txtManPower.Text = string.Empty;
        hfManPower.Value = string.Empty;
        txtManPowerC1.Text = string.Empty;
        txtManPowerC2.Text = string.Empty;
        txtManPowerAP.Text = string.Empty;
        dtpMPStartDate.Reset();
        dtpMPEndDate.Reset();

        txtTraining.Text = string.Empty;
        hfTraining.Value = string.Empty;
        txtTrainingC1.Text = string.Empty;
        txtTrainingC2.Text = string.Empty;
        txtTrainingAP.Text = string.Empty;
        dtpTrainStartDate.Reset();
        dtpTrainEndDate.Reset();
        #endregion
        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                switch (ds.Tables[0].Rows[i]["Transaction Code"].ToString().ToUpper())
                {
                    case "OVERTIME":
                        txtOvertime.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfOvertime.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtOvertimeC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtOvertimeC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtOvertimeAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpOTStartDate.DateString =ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfOTStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpOTEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfOTEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "LEAVE":
                        txtLeave.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfLeave.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtLeaveC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtLeaveC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtLeaveAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpLVStartDate.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["Start Date"].ToString());
                        hfLVStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpLVEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfLVEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "TIMEMOD":
                        txtTimeRecord.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfTimeRecord.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtTimeRecordC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtTimeRecordC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtTimeRecordAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpTRStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfTRStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpTREndDate.DateString =ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfTREnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "STRAIGHTWK":
                        txtStraightWork.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfStraightWork.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtStraightWorkC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtStraightWorkC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtStraightWorkAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpSWStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfSWStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpSWEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfSWEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "JOBMOD":
                        txtManHour.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfManHour.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtManHourC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtManHourC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtManHourAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpMHStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfMHStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpMHEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfMHEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "FLEXTIME":
                        txtFlexTime.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfFlexTime.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtFlexTimeC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtFlexTimeC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtFlexTimeAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpFXStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfFXStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpFXEndDate.DateString =ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfFXEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "MOVEMENT":
                        txtWorkInfo.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfWorkInfo.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtWorkInfoC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtWorkInfoC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtWorkInfoAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpWIStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfWIStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpWIEndDate.DateString =ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfWIEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "BNEFICIARY":
                        txtBeneficiary.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfBeneficiary.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtBeneficiaryC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtBeneficiaryC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtBeneficiaryAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpBenStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfBenStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpBenEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfBenEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "ADDRESS":
                        txtAddress.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfAddress.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtAddressC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtAddressC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtAddressAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpAddStartDate.DateString =ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfAddStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpAddEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfAddEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "TAXMVMNT":
                        txtTaxCivil.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfTaxCivil.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtTaxCivilC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtTaxCivilC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtTaxCivilAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpTCStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfTCStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpTCEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfTCEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "MANPOWER":
                        txtManPower.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfManPower.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtManPowerC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtManPowerC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtManPowerAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpMPStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfMPStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpMPEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfMPEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    case "TRAINING":
                        txtTraining.Text = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        hfTraining.Value = ds.Tables[0].Rows[i]["Route ID"].ToString().ToUpper();
                        txtTrainingC1.Text = ds.Tables[0].Rows[i]["Checker 1"].ToString().ToUpper();
                        txtTrainingC2.Text = ds.Tables[0].Rows[i]["Checker 2"].ToString().ToUpper();
                        txtTrainingAP.Text = ds.Tables[0].Rows[i]["Approver"].ToString().ToUpper();
                        dtpTrainStartDate.DateString = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        hfTrainStart.Value = ds.Tables[0].Rows[i]["Start Date"].ToString();
                        dtpTrainEndDate.DateString = ds.Tables[0].Rows[i]["End Date"].ToString();
                        hfTrainEnd.Value = ds.Tables[0].Rows[i]["End Date"].ToString();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void showRowTransactionUsed()
    {
        //based on applicable transaction in company
        tbrAddress.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWADDRESS);
        tbrBeneficiary.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWBENEFICIARY);
        tbrFlexTime.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWFLEXTIME);
        tbrLeave.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWLEAVE);
        tbrManHour.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWJOBSPLIT);
        tbrOvertime.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWOVERTIME);
        tbrStraightWork.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWSTRAIGHTWORK);
        tbrTaxCivil.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTAXCIVIL);
        tbrTimeRecord.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTIMEMODIFICATION);
        tbrWorkInfo.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWWORKINFORMATION);
        tbrManPower.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWMANPOWER);
        tbrTraining.Visible = Convert.ToBoolean(Resources.Resource.AFSHOWTRAINING);

        if (!Convert.ToBoolean(Resources.Resource.EMAILNOTIFICATION))
        {
            btnADNotify.Enabled = false;
            btnBFNotify.Enabled = false;
            btnFTNotify.Enabled = false;
            btnLVNotify.Enabled = false;
            btnJSNotify.Enabled = false;
            btnMVNotify.Enabled = false;
            btnOTNotify.Enabled = false;
            btnSWNotify.Enabled = false;
            btnTRNotify.Enabled = false;
            btnTXNotify.Enabled = false;
            btnMPNotify.Enabled = false;
        }
    }

    private string changeSnapShot()
    {
        string snapShot = string.Empty;
        snapShot = txtAddress.Text
                 + txtBeneficiary.Text
                 + txtFlexTime.Text
                 + txtLeave.Text
                 + txtManHour.Text
                 + txtOvertime.Text
                 + txtStraightWork.Text
                 + txtTaxCivil.Text
                 + txtTimeRecord.Text
                 + txtWorkInfo.Text;
        return snapShot;
    }
    private string checkEntry()
    {
        string err = string.Empty;


        if (dtpAddStartDate.DateString.Equals(string.Empty) && txtAddress.Text != string.Empty)
            err += "Address Start Date is empty\n";
        if (dtpBenStartDate.DateString.Equals(string.Empty) && txtBeneficiary.Text != string.Empty)
            err += "Beneficiary Start Date is empty\n";
        if(dtpFXStartDate.DateString.Equals(string.Empty) && txtFlexTime.Text != string.Empty)
            err += "Flextime Start Date is empty\n";
        if (dtpLVStartDate.DateString.Equals(string.Empty) && txtLeave.Text != string.Empty)
            err += "Leave Start Date is empty\n";
        if (dtpMHStartDate.DateString.Equals(string.Empty) && txtManHour.Text != string.Empty)
            err += "Man Hour Start Date is empty\n";
        if (dtpMPStartDate.DateString.Equals(string.Empty) && txtManPower.Text != string.Empty)
            err += "Man Power Start Date is empty\n";
        if (dtpOTStartDate.DateString.Equals(string.Empty) && txtOvertime.Text != string.Empty)
            err += "Overtime Start Date is empty\n";
        if (dtpSWStartDate.DateString.Equals(string.Empty) && txtStraightWork.Text != string.Empty)
            err += "Straight Work Start Date is empty\n";
        if (dtpTCStartDate.DateString.Equals(string.Empty) && txtTaxCivil.Text != string.Empty)
            err += "Tax/Civil Status Start Date is empty\n";
        if (dtpTrainStartDate.DateString.Equals(string.Empty) && txtTraining.Text != string.Empty)
            err += "Training Start Date is empty\n";
        if (dtpTRStartDate.DateString.Equals(string.Empty) && txtTimeRecord.Text != string.Empty)
            err += "Time Record Start Date is empty\n";
        if (dtpWIStartDate.DateString.Equals(string.Empty) && txtWorkInfo.Text != string.Empty)
            err += "Work Info Start Date is empty\n";

        if (dtpAddStartDate.DateString != string.Empty
           && dtpAddEndDate.DateString != string.Empty)
        {
            if (dtpAddStartDate.Date > dtpAddEndDate.Date)
                err += "Address Start Date is greater than End Date\n";
        }
        if (dtpBenStartDate.DateString != string.Empty
            && dtpBenEndDate.DateString != string.Empty)
        {
            if (dtpBenStartDate.Date > dtpBenEndDate.Date)
                err += "Beneficiary Start Date is greater than End Date\n";
        }
        if (dtpFXStartDate.DateString != string.Empty
            && dtpFXEndDate.DateString != string.Empty)
        {
            if (dtpFXStartDate.Date > dtpBenEndDate.Date)
                err += "Flextime Start Date is greater than End Date\n";
        }
        if (dtpLVStartDate.DateString != string.Empty
            && dtpLVEndDate.DateString != string.Empty)
        {
            if (dtpLVStartDate.Date > dtpLVEndDate.Date)
                err += "Leave Start Date is greater than End Date\n";
        }
        if (dtpMHStartDate.DateString != string.Empty
            && dtpMHEndDate.DateString != string.Empty)
        {
            if (dtpMHStartDate.Date > dtpMHEndDate.Date)
                err += "Man Hour Start Date is greater than End Date\n";
        }
        if (dtpMPStartDate.DateString != string.Empty
            && dtpMPEndDate.DateString != string.Empty)
        {
            if (dtpMPStartDate.Date > dtpMPEndDate.Date)
                err += "Man Power Start Date is greater than End Date\n";
        }
        if (dtpOTStartDate.DateString != string.Empty
            && dtpOTEndDate.DateString != string.Empty)
        {
            if (dtpOTStartDate.Date > dtpOTEndDate.Date)
                err += "OT Start Date is greater than End Date\n";
        }
        if (dtpSWStartDate .DateString!= string.Empty
            && dtpSWEndDate.DateString != string.Empty)
        {
            if (dtpSWStartDate.Date > dtpMHEndDate.Date)
                err += "Straight Work Start Date is greater than End Date\n";
        }
        if (dtpTCStartDate.DateString != string.Empty
            && dtpTCEndDate.DateString != string.Empty)
        {
            if (dtpTCStartDate.Date > dtpTCEndDate.Date)
                err += "Tax/Civil Status Start Date is greater than End Date\n";
        }
        if (dtpTrainStartDate.DateString != string.Empty
            && dtpTrainEndDate.DateString != string.Empty)
        {
            if (dtpTrainStartDate.Date > dtpTrainEndDate.Date)
                err += "Training Start Date is greater than End Date\n";
        }
        if (dtpTRStartDate.DateString != string.Empty
            && dtpTREndDate.DateString != string.Empty)
        {
            if (dtpTRStartDate.Date > dtpTREndDate.Date)
                err += "Time Record Start Date is greater than End Date\n";
        }        
        if (dtpWIStartDate.DateString != string.Empty
            && dtpWIEndDate.DateString != string.Empty)
        {
            if (dtpWIStartDate.Date > dtpWIEndDate.Date)
                err += "Work Info Start Date is greater than End Date\n";
        }

        return err;
    }
   
    private bool hasRecordExist()
    {
        bool hasRecord = true;
        string query = string.Format(@"SELECT *
                                FROM T_EmployeeApprovalRoute
                                WHERE Arm_EmployeeID = '{0}'", txtEmployeeId.Text);

        DataTable dt = new DataTable();
        using (DALHelper dal = new DALHelper())
        {
            dal.OpenDB();
            dt = dal.ExecuteDataSet(query).Tables[0];
            dal.CloseDB();
        }
        if (dt.Rows.Count > 0)
        {
            hasRecord = true;
        }
        else
        {
            hasRecord = false;
        }

        return hasRecord;
    }
    #endregion
}
