// JScript File
function lookupRPLEmployee()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupPLRepEmployee.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRPLEmployee(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupRPLStatus()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupPLRepStatus.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRPLStatus(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupRPLCostcenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupPLRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRPLCostcenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}


function lookupRPLCostcenterLine() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupPLRepCostCenterLine.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRPLCostcenterLine(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenterLine').value = val;
}

function lookupRPLPayPeriod()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupPLRepPayPeriod.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRPLPayPeriod(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val;
}

function lookupRPLCheckerApprover(col, control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupPLRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRPLCheckerApprover(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+ control).value = val;
}

function lookupRTCTaxCode(control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupRTCTaxCode.aspx?ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTCTaxCode(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+ control).value = val;
}

function lookupBeneficiaryType() 
{
    var type = document.getElementById('ctl00_ContentPlaceHolder1_ddlType').value;
    if (type == 'N') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtLastname').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtFirstname').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtMiddlename').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_cbxHMO').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxInsurance').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxBIR').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxAccident').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_dtpCancelDate').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_dtpDeceasedDate').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_ddlMonth').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlDay').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlYear').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_hfSaved').value = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_txtControlNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfSeqNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtOccupation').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtCompany').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_ddlGender').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCivilStatus').selectedIndex = 0;

    }   
    else 
    {
        var left = (screen.width / 2) - (820 / 2);
        var top = (screen.height / 2) - (410 / 2);
        var employeeid = document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value;
        popupWin = window.open("lookupBeneficiary.aspx?id=" + employeeid, "Sample", "scrollbars=no,resizable=no,width=820,height=410,top=" + top + ",left=" + left);
    }
    return false;
}

function GetValueFrom_lookupBeneficiary( valLastname
                                       , valFirstName
                                       , valMiddlename
                                       , valOccupation
                                       , valCompany
                                       , valGender
                                       , valCivilStatus
                                       , valBirthdate
                                       , valRelationshipCode
                                       , valRelationshipDesc
                                       , valHierarchyCode
                                       , valHierarchyDesc
                                       , valHMO
                                       , valInsurance
                                       , valBIR
                                       , valAccident
                                       , valDeceasedDate
                                       , valCancelledDate
                                       , valSeqNo)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtLastname').value = valLastname.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtFirstname').value = valFirstName.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtMiddlename').value = valMiddlename.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyCode').value = valHierarchyCode.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyDesc').value = valHierarchyDesc.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipCode').value = valRelationshipCode.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipDesc').value = valRelationshipDesc.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_dtpCancelDate').value = valCancelledDate.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_dtpDeceasedDate').value = valDeceasedDate.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtCompany').value = valCompany.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_ddlGender').value = valGender.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_txtOccupation').value = valOccupation.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    document.getElementById('ctl00_ContentPlaceHolder1_ddlCivilStatus').value = valCivilStatus.replace(/amp;/g, '').replace(/&nbsp;/g, '');
    
    if(valHMO.toUpperCase() == 'TRUE')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxHMO').checked = true;
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxHMO').checked = false;
    }
    if (valInsurance.toUpperCase() == 'TRUE')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxInsurance').checked = true;
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxInsurance').checked = false;
    }
    if (valBIR.toUpperCase() == 'TRUE')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxBIR').checked = true;
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxBIR').checked = false;
    }
    if (valAccident.toUpperCase() == 'TRUE')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxAccident').checked = true;
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_cbxAccident').checked = false;
    }

    var _myDate = new Date(valBirthdate);
    var month = _myDate.getMonth();
    var day = _myDate.getDate();
    var len;
    var maxYear;
    var myYear;
    var ddlYear = document.getElementById('ctl00_ContentPlaceHolder1_ddlYear');
    document.getElementById('ctl00_ContentPlaceHolder1_ddlMonth').selectedIndex = parseInt(month, 10) + 1;
    document.getElementById('ctl00_ContentPlaceHolder1_ddlDay').selectedIndex = parseInt(day, 10);
    len = document.getElementById('ctl00_ContentPlaceHolder1_ddlYear').length;
    maxYear = ddlYear.options[len - 1].value;
    myYear = _myDate.getFullYear();
    document.getElementById('ctl00_ContentPlaceHolder1_ddlYear').value = myYear.toString();
    document.getElementById('ctl00_ContentPlaceHolder1_hfSeqNo').value = valSeqNo;
    document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
    document.getElementById('ctl00_ContentPlaceHolder1_hfPrevEntry').value = changeSnapShot();
}

function changeSnapShot()
{
    return ( document.getElementById('ctl00_ContentPlaceHolder1_dtpEffectivityDate').value
           + document.getElementById('ctl00_ContentPlaceHolder1_txtLastname').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtFirstname').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtMiddlename').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtOccupation').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtCompany').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_ddlGender').value
           + document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyCode').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyDesc').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipCode').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipDesc').value.toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_dtpCancelDate').value
           + document.getElementById('ctl00_ContentPlaceHolder1_dtpDeceasedDate').value
           + document.getElementById('ctl00_ContentPlaceHolder1_cbxAccident').checked.toString().toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_cbxBIR').checked.toString().toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_cbxHMO').checked.toString().toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_cbxInsurance').checked.toString().toUpperCase()
           + document.getElementById('ctl00_ContentPlaceHolder1_ddlMonth').value
           + document.getElementById('ctl00_ContentPlaceHolder1_ddlDay').value
           + document.getElementById('ctl00_ContentPlaceHolder1_ddlYear').value);
}


function lookupRBFStatus() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupBFRepStatus.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRBFStatus(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupRBFCostcenter() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupBFRepCostCenter.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRBFCostcenter(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}

function lookupRBFPayPeriod() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupBFRepPayPeriod.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRBFPayPeriod(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val;
}

function lookupRBFCheckerApprover(col, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupBFRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRBFCheckerApprover(control, val) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
}

function lookupBFRepBeneficairyNames(col, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupBFRepBeneficairyNames.aspx?col=" + col + "&ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupBFRepBeneficairyNames(control, val) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
}


function lookupADRoute() 
{
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupADRoute.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupADRoute(val1, val2, val3) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = val1;
    document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1Desc').value = val2;
    //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = val3;
}
function lookupRADAddress(control, accountType) 
{
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupRADAddress.aspx?ctrl=" + control + "&type=" +accountType, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function clearControlsAddressPres() 
{
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTelephoneNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtMobileNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmailAddress').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        if (document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1') != null) {
            document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = '';
            document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1Desc').value = '';
        }
        if (document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2') != null) {
            document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = '';
        }

        if (document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3') != null)
            document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = '';
    }
}

function clearControlsPermanent() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') {
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTelephoneNo').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtMobileNo').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtEmailAddress').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1Desc').value = '';
       // document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = '';

        if (document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3') != null)
            document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = '';
    }
}

function clearControlsTax() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') {
        document.getElementById('ctl00_ContentPlaceHolder1_txtFromTaxCodeCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtFromTaxCodeDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToTaxCodeCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToTaxCodeDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtFromCivilCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtFromCivilDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToCivilCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToCivilDesc').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1Desc').value = '';
        //document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = '';

        //if (document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3') != null)
        //    document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = '';
    }
}

function clearControlsBen() 
{
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtLastname').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtFirstname').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtMiddlename').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtRelationshipDesc').value = '';

        document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtHierarchyDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_cbxHMO').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxInsurance').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxAccident').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_cbxBIR').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
    }
}

function clearControlsEmergency() 
{
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtContactPerson').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtContactRelationCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtContactRelationDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress2Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Code').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAddress3Desc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTelephoneNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
    }
}

function lookupRADRoute(control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupRADRoute.aspx?ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}
function GetValueFrom_lookupRADRoute(val1, val2) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1).value = val2;
}
function fn_validateNumeric(thi, e) {
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;
    if (((keycode >= 65) && (keycode <= 90))   || keycode==8 || keycode==32) {
        return true;
    }
    else {
        //((keycode >= 97) && (keycode <= 122))
        return false;
    }
}
function fn_validateNumericOnly(thi, e) {
    if (window.event) keycode = window.event.keyCode;
    else if (e) keycode = e.which;
    else return true;
    if (((keycode >= 96) && (keycode <= 105) || (keycode >= 48 && keycode<=57)) || keycode == 8) {
        return true;
    }
    else {
        //((keycode >= 97) && (keycode <= 122))
        return false;
    }
}