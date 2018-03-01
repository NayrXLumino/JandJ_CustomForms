// JScript File
function lookupCheckList(transactionType)
{
    var ctr = 0;
    switch(transactionType)
    {
        case 'OT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCOT').innerHTML;
            break;
        case 'LV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCLV').innerHTML;
            break;
        case 'TR':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCTR').innerHTML;
            break;
        case 'FT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCFT').innerHTML;
            break;
        case 'JS':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCJS').innerHTML;
            break;
        case 'MV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCMV').innerHTML;
            break;
        case 'TX':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCTX').innerHTML;
            break;
        case 'BF':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCBF').innerHTML;
            break;
        case 'AD':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCAD').innerHTML;
            break;
        case 'SW':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblCSW').innerHTML;
        default:
            break;
    }
    var count = parseInt(ctr, 10);
    if (count > 0) 
    {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (700 / 2);
        var url = "lookupChecklist.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=yes,width=1000,height=620,top=" + top + ",left=" + left);
        return false;
    }
    else
    {
        return true;
    }
}
function lookupWaitList(transactionType)
{
    var ctr = 0;
    switch (transactionType)
    {
        case 'OT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWOT').innerHTML;
            break;
        case 'LV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWLV').innerHTML;
            break;
        case 'TR':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWTR').innerHTML;
            break;
        case 'FT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWFT').innerHTML;
            break;
        case 'JS':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWJS').innerHTML;
            break;
        case 'MV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWMV').innerHTML;
            break;
        case 'TX':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWTX').innerHTML;
            break;
        case 'BF':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWBF').innerHTML;
            break;
        case 'AD':
            ctr =  document.getElementById('ctl00_ContentPlaceHolder1_lblWAD').innerHTML;
            break;
        case 'SW':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblWSW').innerHTML;
            break;
        default:
            break;
    }
    var count = parseInt(ctr, 10);
    if (count > 0) 
    {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (530 / 2);
        var url = "lookupWaitlist.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
        return false;
    }
    else
    {
        return true;
    }
}
function lookupNextLevel(transactionType)
{
    var ctr = 0;
    switch (transactionType) 
    {
        case 'OT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNOT').innerHTML;
            break;
        case 'LV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNLV').innerHTML;
            break;
        case 'TR':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNTR').innerHTML;
            break;
        case 'FT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNFT').innerHTML;
            break;
        case 'JS':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNJS').innerHTML;
            break;
        case 'MV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNMV').innerHTML;
            break;
        case 'TX':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNTX').innerHTML;
            break;
        case 'BF':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNBF').innerHTML;
            break;
        case 'AD':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNAD').innerHTML;
            break;
        case 'SW':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_lblNSW').innerHTML;
            break;
        default:
            break;
    }
    var count = parseInt(ctr, 10);
    if (count > 0)
    {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (530 / 2);
        var url = "lookupNextLevel.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
        return false;
    }
    else
    {
        return true;
    }
}

function lookupNewPending(transactionType)
{
    var ctr = 0;
    switch (transactionType)
    {
        case 'OT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPOT').innerHTML;
            break;
        case 'LV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPLV').innerHTML;
            break;
        case 'TR':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPTR').innerHTML;
            break;
        case 'FT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPFT').innerHTML;
            break;
        case 'JS':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPJS').innerHTML;
            break;
        case 'MV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPMV').innerHTML;
            break;
        case 'TX':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPTX').innerHTML;
            break;
        case 'BF':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPBF').innerHTML;
            break;
        case 'AD':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPAD').innerHTML;
            break;
        case 'SW':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnNPSW').innerHTML;
            break;
        default:
            break;
    }
    var count = parseInt(ctr, 10);
    if (count > 0) 
    {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (530 / 2);
        var url = "lookupNewAndPending.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
        return false;
    }
    else
    {
        return true;
    }
}
function GetValueFrom_lookupNewPending(controlNo)
{
    var URL = '';
    if (controlNo != '')
    {
        switch (controlNo.substring(0, 1))
        {
            case 'V':
                URL = 'Transactions/Overtime/pgeOvertimeIndividual.aspx?cn=' + controlNo;
                break;
            case 'L':
                URL = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn=' + controlNo;
                break;
            case 'T':
                URL = 'Transactions/TimeModification/pgeTimeModification.aspx?cn=' + controlNo;
                break;
            case 'F':
                URL = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn=' + controlNo;
                break;
            case 'J':
                URL = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn=' + controlNo;
                break;
            case 'I':
                URL = 'Transactions/Personnel/pgeTaxCodeCivilStatus.aspx?cn=' + controlNo;
                break;
            case 'E':
                URL = 'Transactions/Personnel/pgeBeneficiaryUpdate.aspx?cn=' + controlNo;
                break;
            case 'A':
                URL = 'Transactions/Personnel/pgeAddress.aspx?cn=' + controlNo;
                break;
            default:
                break;
        }
        window.location.href = URL;
    }
    return false;
}

function lookupApprovedDisapproved(transactionType)
{
    var ctr = 0;
    switch (transactionType)
    {
        case 'OT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADOT').innerHTML;
            break;
        case 'LV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADLV').innerHTML;
            break;
        case 'TR':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADTR').innerHTML;
            break;
        case 'FT':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADFT').innerHTML;
            break;
        case 'JS':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADJS').innerHTML;
            break;
        case 'MV':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADMV').innerHTML;
            break;
        case 'TX':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADTX').innerHTML;
            break;
        case 'BF':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADBF').innerHTML;
            break;
        case 'AD':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADAD').innerHTML;
            break;
        case 'SW':
            ctr = document.getElementById('ctl00_ContentPlaceHolder1_btnADSW').innerHTML;
            break;
        default:
            break;
    }
    var count = parseInt(ctr, 10);
    if (count > 0)
    {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (500 / 2);
        var url = "lookupApprovedAndDisapproved.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=450,top=" + top + ",left=" + left);
        return false;
    }
    else
    {
        return true;
    }
}

function setURL(url) 
{
    
    //window.location.href = url;
    //
    var ver = getInternetExplorerVersion();
    alert('Loading transaction from pending list...');
    if (ver > -1) {
        if (ver >= 8.0 && ver < 10.0)
            //window.location = '~/Transactions/Default.aspx';
            window.location=url;
        else
            window.location.href = url;
            //window.location.href = '~/Transactions/Default.aspx';
    }
    scram();
    //checkVersion();
}
function getInternetExplorerVersion()
    // Returns the version of Internet Explorer or a -1
    // (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}
function checkVersion() {
    var msg = "You're not using Internet Explorer.";
    var ver = getInternetExplorerVersion();

    if (ver > -1) {
        if (ver >= 8.0 && ver < 10.0)
            msg = "You're using a recent copy of Internet Explorer."
        else
            msg = "You should upgrade your copy of Internet Explorer.";
    }
    alert(msg);
}
