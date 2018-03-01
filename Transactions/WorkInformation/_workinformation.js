function clearControlsRestDay() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') {
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        return false;
    }
    else {
        return true;
    }
} 

function clearControlsCostcenter()
{
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtToCostCenterCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToCostCenterDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        return false;
    }
    else {
        return true;
    }
} 

function clearControlsGroup() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtToGroupCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToGroupDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        return false;
    }
    else 
    {
        return true;
    }
} 
function clearControlsShift() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtToShiftCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtToShiftDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        return false;
    }
    else 
    {
        return true;
    }
}

function lookupWorkGroup(type, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupWorkGroup.aspx?ctrl=" + control + "&type=" + type, "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupWorkGroup(val1, val2, val3, val4) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Code').value = padding_left(val2,' ', 3) + padding_left(val3,' ', 3);
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Desc').value = val4;
    if (val1 == 'txtFromGroup') {
        __doPostBack();
    }
}

function padding_left(s, c, n) {
    if (!s || !c || s.length >= n) {
        return s;
    }

    var max = (n - s.length) / c.length;
    for (var i = 0; i < max; i++) {
        s = c + s;
    }

    return s;
}


function lookupShift(type,control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    var dateVal = document.getElementById('ctl00_ContentPlaceHolder1_dtpEffectivityDate').value;
    popupWin = window.open("lookupShift.aspx?ctrl=" + control + "&type=" + type + "&shiftDate=" + dateVal, "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupShift(val1,val2,val3) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Code').value = val2;
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Desc').value = val3;
    if (val1 == 'txtFromShift') 
    {
        __doPostBack();
    }
}

function lookupRMVEmployee() 
{
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepEmployee.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVEmployee(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupRMVBatchNo() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepBatchNo.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVBatchNo(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtBatchNo').value = val;
}

function lookupRMVCostcenter() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepCostCenter.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVCostcenter(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}


function lookupRMVCostcenterLine() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepCostCenterLine.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVCostcenterLine(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenterLine').value = val;
}

function lookupRMVStatus() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepStatus.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVStatus(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupRMVFromTo(column, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    var obj = document.getElementById('ctl00_ContentPlaceHolder1_ddlMoveType');
    var type = obj.options[obj.selectedIndex].value;
    popupWin = window.open("lookupMVRepFromTo.aspx?col=" + column + "&ctrl=" + control + "&type=" + type, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVFromTo(val, control) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
}

function lookupRMVPayPeriod() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepPayPeriod.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVPayPeriod(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayrollPeriod').value = val;
}

function lookupRMVCheckerApprover(col, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupMVRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRMVCheckerApprover(control, val) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
}

function lookupCostCenter(type, control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupCostCenter.aspx?type=" + type + "&ctrl="+control, "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupCostCenter(val1, val2, val3) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Code').value = val2;
    document.getElementById('ctl00_ContentPlaceHolder1_' + val1 + 'Desc').value = val3.replace('&amp;', '&');
}