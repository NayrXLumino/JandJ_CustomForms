//// JScript File

function clearControlsT() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnClear').value == 'CLEAR') {
        document.getElementById('ctl00_ContentPlaceHolder1_GMDatePickerDate').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeName').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtNickname').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_rblTypeOfApplication_0').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblTypeOfApplication_1').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblTypeOfApplication_2').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblTypeOfApplication_3').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtTypeRemarks').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtVehicle').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtAttachment').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtRemarks').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = '';


        return false;
    }
    else {
        return true;
    }
}