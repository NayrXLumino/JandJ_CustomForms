var _handle = true;
var _selectNumber = 0;  // the number of selected rows within the page
var _isEndorse1 = 0;
var _isEndorse2 = 0;
var _isApprover = 0;
var isTriggered = false;
var isFromSelection = false;
var isFromCheckAll = false;
var isStillInProcess = false;

var isEnabled2 = false;
var isEnabledA = false;
var isApprove = false;

function OnAllCheckedChanged(s, e) {
    isTriggered = true;
    isFromCheckAll = true;
    if (s.GetChecked())
    { agvData.SelectRows(); }
    else
    { agvData.UnselectRows(); }
}

function OnPageCheckedChanged(s, e) {
    _handle = false;
    if (s.GetChecked())
    { agvData.SelectAllRowsOnPage(); }
    else
    { agvData.UnselectAllRowsOnPage(); }
    isTriggered = true;
    isFromCheckAll = true;
}

function OnGridSelectionChanged(s, e) {
    if (!isFromCheckAll) {
        cbAll.SetChecked(s.GetSelectedRowCount() == s.cpVisibleRowCount);
        if (e.isChangedOnServer == false) {
            if (e.isAllRecordsOnPage && e.isSelected)
                _selectNumber = s.GetVisibleRowsOnPage(); // when all rows are selected within the page
            else if (e.isAllRecordsOnPage && !e.isSelected)
                _selectNumber = 0; // when all rows are deselected within the page
            else if (!e.isAllRecordsOnPage && e.isSelected)
                _selectNumber++; // when one row is selected
            else if (!e.isAllRecordsOnPage && !e.isSelected)
                _selectNumber--; // when one row is deselected
//            if (_handle) { // if the selection wasn’t performed by clicking the cbPage
//                cbPage.SetChecked(_selectNumber == s.GetVisibleRowsOnPage()); // let’s change the cbPage state if needed
//                _handle = false;
//            }
            _handle = true;
        }
        else {
            //cbPage.SetChecked(cbAll.GetChecked()); // if the selection was performed on the server, let’s check cbPage
        }
        isTriggered = true;
        //if (isStillInProcess) {
        //    while (isStillInProcess) {
        //        //do nothing just loop until DevEx Process finishes
        //    }
        //    setTimeout(function () { SetButtons(); }, 1500);
        //}
        //else {
        //SetButtonFromRowCheck(e.visibleIndex, e.isSelected);
        var obj = document.getElementById('ContentPlaceHolder1_ASPxSplitter1_agvData_DXDataRow' + e.visibleIndex);
        if (obj != null) {
            var statusName = obj.cells[1].innerHTML;
            if (e.isSelected) {
                AddCount(statusName);
            }
            else {
                MinusCount(statusName);
            }
        }
        //}
    }
    //SetButtons();
}

function SetButtonFromRowCheck(iDx, isAdd) {
    if (isAdd) {
        agvData.GetRowValues(iDx, "Status", AddCount);
    }
    else {
        agvData.GetRowValues(iDx, "Status", MinusCount);
    }
}


function MinusCount(values) {
    try {
        if (isTriggered || isFromSelection) {
            isStillInProcess = true;
            if (values.length > 0) {
                if (values == "ENDORSED TO CHECKER 1") {
                    _isEndorse1 -= 1;
                    if (_isEndorse1 <= 0) {
                        _isEndorse1 = 0;
                        btnEndorseChecker2.SetEnabled(false);
                        isEnabled2 = false;
                    }
                }
                else if (values == "ENDORSED TO CHECKER 2") {
                    _isEndorse2 -= 1;
                    if (_isEndorse2 <= 0) {
                        _isEndorse2 = 0;
                        btnEndorseApprove.SetEnabled(false);
                        isEnabledA = false;
                    }
                }
                else if (values == "ENDORSED TO APPROVER") {
                    _isApprover -= 1;
                    if (_isApprover <= 0) {
                        _isApprover = 0;
                        btnApprove.SetEnabled(false);
                        isApprove = false;
                    }
                }
            }
            isFromSelection = false;
            isTriggered = false;
            isStillInProcess = false;
        }
    }
    catch (e) {
        isStillInProcess = false;
    }
}

function AddCount(values) {
    try {
        if (isTriggered || isFromSelection) {
            isStillInProcess = true;
            if (values.length > 0) {
                if (values == "ENDORSED TO CHECKER 1") {
                    _isEndorse1 += 1;
                    if (_isEndorse1 > 0) {
                        btnEndorseChecker2.SetEnabled(true);
                        isEnabled2 = true;
                    }
                }
                else if (values == "ENDORSED TO CHECKER 2") {
                    _isEndorse2 += 1;
                    if (_isEndorse2 > 0) {
                        btnEndorseApprove.SetEnabled(true);
                        isEnabledA = true;
                    }
                }
                else if (values == "ENDORSED TO APPROVER") {
                    _isApprover += 1;
                    if (_isApprover > 0) {
                        btnApprove.SetEnabled(true);
                        isApprove = true;
                    }
                }
            }
            isFromSelection = false;
            isTriggered = false;
            isStillInProcess = false;
        }
    }
    catch (e) {
        isStillInProcess = false;
    }
}

function OnGridEndCallback(s, e) {
    _selectNumber = s.cpSelectedRowsOnPage; // get the number of selected rows within the page

    if (isFromCheckAll) {
        isFromCheckAll = false;
        _isEndorse1 = s.cpE1;
        _isEndorse2 = s.cpE2;
        _isApprover = s.cpAP;
        SetEnableTransactionButtons2();
    }
//    if (isFromCheckAll)
//        SetButtons();
//    else
//        SetEnableTransactionButtons2();
//    if (document.getElementById('ContentPlaceHolder1_ASPxSplitter1_hfIsFromCheckList').value == 'TRUE') {
//        $get('ContentPlaceHolder1_UpdateGridLoader').style.display = 'block';
//        isTriggered = true;
//        isFromSelection = false;
//        isFromCheckAll = true;
//        SetButtons();
//        document.getElementById('ContentPlaceHolder1_ASPxSplitter1_hfIsFromCheckList').value = 'FALSE';
//    }
}

function SetButtons() {
    agvData.GetSelectedFieldValues("Status", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    try {
        if (isTriggered && !isFromSelection && isFromCheckAll) {
            _isEndorse1 = 0;
            _isEndorse2 = 0;
            _isApprover = 0;
            for (var i = 0; i < values.length; i++) {
                if (values[i] == "ENDORSED TO CHECKER 1")
                    _isEndorse1 += 1;
                else if (values[i] == "ENDORSED TO CHECKER 2")
                    _isEndorse2 += 1;
                else if (values[i] == "ENDORSED TO APPROVER")
                    _isApprover += 1;
            }
            btnEndorseChecker2.SetEnabled(_isEndorse1 > 0);
            btnEndorseApprove.SetEnabled(_isEndorse2 > 0);
            btnApprove.SetEnabled(_isApprover > 0);
            isEnabled2 = _isEndorse1 > 0;
            isEnabledA = _isEndorse2 > 0;
            isApprove = _isApprover > 0;
            isTriggered = false;
            isFromCheckAll = false;
        }
    } catch (e) {
        try {

        }
        catch (e) {

        }
    }
}

function SetButtonsFromChoicesChange() {
    setTimeout(function () { SetEnableTransactionButtons(); }, 1500);
    return true;
}

function SetEnableTransactionButtons() {
    try {
        btnEndorseChecker2.SetEnabled(_isEndorse1 > 0);
        btnEndorseApprove.SetEnabled(_isEndorse2 > 0);
        btnApprove.SetEnabled(_isApprover > 0);
        isEnabled2 = _isEndorse1 > 0;
        isEnabledA = _isEndorse2 > 0;
        isApprove = _isApprover > 0;
    } catch (e) {

    }
}

function SetEnableTransactionButtons2() {
    try {
        btnEndorseChecker2.SetEnabled(_isEndorse1 > 0);
        btnEndorseApprove.SetEnabled(_isEndorse2 > 0);
        btnApprove.SetEnabled(_isApprover > 0);
    }
    catch (e) {

    }
}

function ColapsePane() {
    var panel = ASPxSplitter1.GetPane(1);
    panel.Expand(ASPxSplitter1.GetPane(0));
    try {
        
        var panel2 = ASPxSplitter1.GetPane(0);
        panel2.Collapse(ASPxSplitter1.GetPane(1));
        //panel.ColapsePane(ASPxSplitter1.GetPane(0));
       // ColapsePane
    }
    catch (ex)
    { }
    _isEndorse1 = 0;
    _isEndorse2 = 0;
    _isApprover = 0;
    return true;
}

function SetRowToLoad(s, e) {
        agvData.GetRowValues(agvData.GetFocusedRowIndex(), 'Key', OnGetRowValues);
}
function OnGetRowValues(data) {
    // use the "data" parameter to obtain the requested values
    document.getElementById('ctl00_ContentPlaceHolder1_hfToLoad').value = data;
    //transactiontype
    var transactionType = document.getElementById('ctl00_ContentPlaceHolder1_hfTransactionType').value;

    var userCode = '';
    if (document.getElementById('ctl00_ContentPlaceHolder1_hfUserCode') != null)
        userCode = document.getElementById('ctl00_ContentPlaceHolder1_hfUserCode').value;
    //apcResultsGrid.Show();
    //agvDetails.PerformCallback();
    var lbl = lblInformation.GetValue();
    if (lbl == "Loaded from Check List") {
        var ctr = 0;

        var count = 1;
        if (count > 0) {
            refresh = true;
            var left = (screen.width / 2) - (990 / 2);
            var top = (screen.height / 2) - (700 / 2);
            var url = "lookupChecklist.aspx?type=" + transactionType + "&usercode=" + userCode + "&condition=" + data;
            popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=1000,height=620,top=" + top + ",left=" + left);
            return false;
        }
        else {
            return true;
        }
    }
    else if (lbl == "Loaded from New and Pending List") {
        agvData.GetRowValues(agvData.GetFocusedRowIndex(), "Status", LoadTransaction);
    }
    else if (lbl == "Loaded from Wait List") {
        var count = 1;
        if (count > 0) {
            refresh = true;
            var left = (screen.width / 2) - (990 / 2);
            var top = (screen.height / 2) - (530 / 2);
            var url = "lookupWaitlist.aspx?type=" + transactionType + "&usercode=" + userCode + "&condition=" + data;
            popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
            return false;
        }
        else {
            return true;
        }

    }
    else if (lbl == "Loaded from Next Level / For Approval List") {
        var count = 1;
        if (count > 0) {
            refresh = true;
            var left = (screen.width / 2) - (990 / 2);
            var top = (screen.height / 2) - (530 / 2);
            var url = "lookupNextLevel.aspx?type=" + transactionType + "&usercode=" + userCode + "&condition=" + data;
            var popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
            popupWin.focus();
            return false;
        }
        else {
            return true;
        }

    }
}
function lookupCheckList(userCode) {
    var transactionType = document.getElementById('ctl00_ContentPlaceHolder1_hfTransactionType').value;
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (700 / 2);
        var url = "lookupChecklist.aspx?type=" + transactionType + "&usercode=" + userCode;
        var popupWin3 = window.open(url, "Sample", "scrollbars=no,resizable=no,width=1000,height=620,top=" + top + ",left=" + left);
        window.focus();
        popupWin3.focus();
       
        return false;
    }
    else {
        return true;
    }
}
function LoadTransaction(s) {
    if (s != null && s == "NEW") {
        document.getElementById('ContentPlaceHolder1_ASPxSplitter1_hfToLoad').value = "1";
        __doPostBack();
    }
}
function lookupApprovedDisapproved(transactionType) {
    var panel = ASPxSplitter1.GetPane(1);
    try{
        panel.ColapsePane(ASPxSplitter1.GetPane(0));
    }
    catch (ex) { }
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (500 / 2);
        var url = "lookupApproved.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=450,top=" + top + ",left=" + left);
        return false;
    }
    else {
        return true;
    }
}
function lookupDisapproved(transactionType) {
    var panel = ASPxSplitter1.GetPane(1);
    try {
        panel.ColapsePane(ASPxSplitter1.GetPane(0));
    }
    catch (ex) { }
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (500 / 2);
        var url = "lookupDisapproved.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=450,top=" + top + ",left=" + left);
        return false;
    }
    else {
        return true;
    }
}
function lookupNewPending(transactionType) {
    var panel = ASPxSplitter1.GetPane(1);
    panel.Collapse(ASPxSplitter1.GetPane(0));
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (530 / 2);
        var url = "lookupNewAndPending.aspx?type=" + transactionType;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
        return false;
    }
    else {
        return true;
    }
}
function Disapprove_Click()
{
    btnDisApproveProceed.SetEnabled(false);
    acpProceed.Show();
}
function Return_Click()
{
    btnReturnProceed.SetEnabled(false);
    apcReturn.Show();
}
function OnAllCheckedChanged(s, e) {
//    //isTriggered = true;
//    //isFromCheckAll = true;
//    if (s.GetChecked())
//    { agvData.SelectRows(); }
//    else
    //    { agvData.UnselectRows(); }
    agvData.PerformCallback('-1;All;' + s.GetChecked());
}
function checkGroup(s, e)
{
    //ContentPlaceHolder1_ASPxSplitter1_hfTriggeredChildNode
    var x = document.getElementById('ctl00_ContentPlaceHolder1_hfTriggeredChildNode').value;
    if (x != 1)
        agvData.PerformCallback('{0};{1};' + s.GetChecked());
    else
        document.getElementById('ctl00_ContentPlaceHolder1_hfTriggeredChildNode').value = 0;
}
function OnGridSelectionChanged(s, e) {
//    //if (!isFromCheckAll) {
//    //document.getElementById('hfTransactionType').value;
//    //document.getElementById('ctl00_ContentPlaceHolder1_hfTriggeredChildNode').value = 1;
//    cbAll.SetChecked(s.GetSelectedRowCount() == s.cpVisibleRowCount && s.cpVisibleRowCount != 0);
//    var countRow = s.cpAP;
//    var uncheckGroup = false;
//    var uncheckParentGroup = false;
//    var uncheckParent2Group = false;
//    var isParentAllShouldBeUnchecked = false;
//    var groupPass = 0;
//    for (var i = countRow; i >= 0; i--) {
//        if (!agvData.IsGroupRow(i)) {
//            if (!agvData.IsRowSelectedOnPage(i)) {
//                uncheckParent2Group = true;
//                uncheckParentGroup = true;
//                isParentAllShouldBeUnchecked = true;
//                groupPass = 0;
//            }
//            

//        }
//        else
//        {
//            groupPass++;
//            if (!uncheckParent2Group && !uncheckParentGroup)
//            {
//                if (!agvData.IsRowSelectedOnPage(i)) {
//                    agvData.SelectRows(i);
//                }
//            }
//            if (uncheckParent2Group) {
//                //if (agvData.IsRowSelectedOnPage(i))
//                {
//                    agvData.UnselectRows(i);
//                }
//                uncheckParent2Group = false;
//            }
//            else if (uncheckParentGroup && groupPass>1) {
//                //if (agvData.IsRowSelectedOnPage(i))
//                {
//                    agvData.UnselectRows(i);
//                }
//                uncheckParent2Group = false;
//                uncheckParentGroup = false;
//                groupPass = 0;
//            }

//        }
//    }

//        //if (e.isChangedOnServer == false) {
//            //if (e.isAllRecordsOnPage && e.isSelected)
//                //_selectNumber = s.GetVisibleRowsOnPage(); // when all rows are selected within the page
//            //else if (e.isAllRecordsOnPage && !e.isSelected)
//                //_selectNumber = 0; // when all rows are deselected within the page
//            //else if (!e.isAllRecordsOnPage && e.isSelected)
//                //_selectNumber++; // when one row is selected
//           // else if (!e.isAllRecordsOnPage && !e.isSelected)
//                //_selectNumber--; // when one row is deselected
//            ////            if (_handle) { // if the selection wasn’t performed by clicking the cbPage
//            ////                cbPage.SetChecked(_selectNumber == s.GetVisibleRowsOnPage()); // let’s change the cbPage state if needed
//            ////                _handle = false;
//            ////            }
//            //_handle = true;
//        //}
//        //else {
//            ////cbPage.SetChecked(cbAll.GetChecked()); // if the selection was performed on the server, let’s check cbPage
//        //}
//        //isTriggered = true;
//        ////if (isStillInProcess) {
//        ////    while (isStillInProcess) {
//        ////        //do nothing just loop until DevEx Process finishes
//        ////    }
//        ////    setTimeout(function () { SetButtons(); }, 1500);
//        ////}
//        ////else {
//        ////SetButtonFromRowCheck(e.visibleIndex, e.isSelected);
//        //var obj = document.getElementById('ContentPlaceHolder1_ASPxSplitter1_agvData_DXDataRow' + e.visibleIndex);
//        //if (obj != null) {
//        //    var statusName = obj.cells[1].innerHTML;
//        //    if (e.isSelected) {
//        //        AddCount(statusName);
//        //    }
//        //    else {
//        //        MinusCount(statusName);
//        //    }
//        //}
//        //}
//    //}
//    //SetButtons();
}

function lookupWaitList(userCode) {
    var transactionType = document.getElementById('ctl00_ContentPlaceHolder1_hfTransactionType').value;
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (700 / 2);
        var url = "lookupWaitlist.aspx?type=" + transactionType + "&usercode=" + userCode;
        var popupWin4 = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
        popupWin4.focus();
        return false;
    }
    else {
        return true;
    }
}
//function lookupWaitList(transactionType) {
//    var ctr = 0;
//    var panel = ASPxSplitter1.GetPane(1);
//    panel.Collapse(ASPxSplitter1.GetPane(0));
//    var count = 1;
//    if (count > 0) {
//        refresh = true;
//        var left = (screen.width / 2) - (990 / 2);
//        var top = (screen.height / 2) - (530 / 2);
//        var url = "lookupWaitlist.aspx?type=" + transactionType;
//        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
//        return false;
//    }
//    else {
//        return true;
//    }
//}
function lookupNextLevel(userCode) {
    var transactionType = document.getElementById('ctl00_ContentPlaceHolder1_hfTransactionType').value;
    var count = 1;
    if (count > 0) {
        refresh = true;
        var left = (screen.width / 2) - (990 / 2);
        var top = (screen.height / 2) - (700 / 2);
        var url = "lookupNextLevel.aspx?type=" + transactionType + "&usercode=" + userCode; ;
        var popupWin5 = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=600,top=" + top + ",left=" + left);
        popupWin5.focus();
        return false;
    }
    else {
        return true;
    }
}
//function lookupNextLevel(transactionType) {
//    var ctr = 0;
//    var panel = ASPxSplitter1.GetPane(1);
//    panel.Collapse(ASPxSplitter1.GetPane(0));
//    var count = 1;
//    if (count > 0) {
//        refresh = true;
//        var left = (screen.width / 2) - (990 / 2);
//        var top = (screen.height / 2) - (530 / 2);
//        var url = "lookupNextLevel.aspx?type=" + transactionType;
//        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=995,height=500,top=" + top + ",left=" + left);
//        return false;
//    }
//    else {
//        return true;
//    }
//}
function treeList_CustomDataCallback(s, e) {
    //document.getElementById('ContentPlaceHolder1_ASPxSplitter1_treeListCountCell').innerHTML = e.result;
}
function treeList_SelectionChanged(s, e) {
    window.setTimeout(function () { s.PerformCustomDataCallback(''); }, 0)
}
function checkValueDisApprove()
{
    var value = document.getElementById("ctl00_ContentPlaceHolder1_ASPxSplitter1_acpProceed_txtDisAprvRemarks").value;
    if (value.replace(/^\s+|\s+$/g, '') != "")
        btnDisApproveProceed.SetEnabled(true);
    else
        btnDisApproveProceed.SetEnabled(false);
}
function checkValueReturn()
{
    var value = document.getElementById("ctl00_ContentPlaceHolder1_ASPxSplitter1_apcReturn_txtRemarksReturn").value;
    if (value.replace(/^\s+|\s+$/g, '') != "")
        btnReturnProceed.SetEnabled(true);
    else
        btnReturnProceed.SetEnabled(false);
}

function lookupApproverOverride(userCode) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupApproverOverride.aspx?ctrl=" + userCode, "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupApproverOverride(control, valId, valName) {
    if (valId != '') {
        document.getElementById('ctl00_ContentPlaceHolder1_ASPxSplitter1_txtApproverOverride').value = valId.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        document.getElementById('ctl00_ContentPlaceHolder1_ASPxSplitter1_txtApproverOverrideDesc').value = valName.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        __doPostBack();
    }
    return false;
}
function showDiv() {
            document.getElementById('divLoadingPanel').style.display = 'block';
            window.setTimeout(partB, 300000)
            //sleep(3000000, foobar_cont);

        }
function partB() {
    document.getElementById('divLoadingPanel').style.display = 'none';
}
function setReadOnly(id) {
    document.getElementById(id).readOnly = true;
    showDiv();
}