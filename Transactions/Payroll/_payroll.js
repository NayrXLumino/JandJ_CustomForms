// JScript File


function lookupPayperiod()
{
    var left=(screen.width/2)-(350/2);
    var top=(screen.height/2)-(310/2);
    popupWin = window.open("lookupPSPayPeriod.aspx?indic=AL&control='txtPayPeriod'", "Sample", "scrollbars=no,resizable=no,width=350,height=310,top=" + top + ", left=" + left);
	return false;

}

function SendValueToParent()
{

	var myVal1 = document.getElementById('valueReturn').value;
	var myVal2 = document.getElementById('valueControl').value;
	window.opener.GetValueFromPayPeriodLookup(myVal1);
	window.close();
	return false;
}

function GetValueFromPayPeriodLookup(val1)
{
    //alert('wewe');
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val1;
    return false;
}

function lookupPSPayPeriod() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupPSPayPeriod.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}
function GetValueFrom_lookupPSPayPeriod(val1, val2) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val1;
    try{
        document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriodDesc').value = val2;
    }
    catch(e){}
}
function lookupPREmployee() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupPRRepEmployee.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}
function lookupPRCostcenter() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupPRRepCostCenter.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}
function GetValueFrom_lookupPREmployee(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}
function GetValueFrom_lookupPRCostcenter(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}