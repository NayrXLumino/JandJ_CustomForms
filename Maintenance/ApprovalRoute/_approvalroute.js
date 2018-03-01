// JScript File

function AssignValueToControlNo(index)
{
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var tbr = tbl.rows[parseInt(index,10)+1];
    var tbcRouteId = tbr.cells[0];
    
    for(var i = 1; i < tbl.rows.length; i++)
    {
        if(i % 2 != 0)
        {
            tbl.rows[i].style.backgroundColor = '#F7F7DE';
        }
        else
        {
            tbl.rows[i].style.backgroundColor = '#FFFFFF';
        }   
    }    
    tbr.style.backgroundColor = '#FF2233';
    document.getElementById('ctl00_ContentPlaceHolder1_txtRouteId').value = tbcRouteId.innerHTML;
}

function lookupARRouteUsage(index)
{
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var tbr = tbl.rows[parseInt(index,10)+1];
    var tbcRouteId = tbr.cells[0];
    
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(300/2);
	popupWin = window.open("lookupARRouteUsage.aspx?ri="+tbcRouteId.innerHTML,"Sample","scrollbars=no,resizable=no,width=750,height=300,top=" +top+ ",left=" +left);
	return false;
}

function lookupInUseRoute()
{
    var val = document.getElementById('ctl00_ContentPlaceHolder1_txtRouteId').value;
    
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(300/2);
	popupWin = window.open("lookupARRouteUsage.aspx?ri="+val,"Sample","scrollbars=no,resizable=no,width=750,height=300,top=" +top+ ",left=" +left);
	return false;
}

function lookupARCheckerApprover(control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(300/2);
	popupWin = window.open("lookupARApproverChecker.aspx?ctrl="+control,"Sample","scrollbars=no,resizable=no,width=750,height=300,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARCheckerApprover(control, valId, valName)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'Id').value = valId;
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'Name').value = valName;
}

function lookupARCostCenter(control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(300/2);
	popupWin = window.open("lookupARCostCenter.aspx?ctrl="+control,"Sample","scrollbars=no,resizable=no,width=750,height=300,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARCostCenter(control, valId, valName)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'Code').value = valId;
    document.getElementById('ctl00_ContentPlaceHolder1_' + control + 'Desc').value = valName;
    document.getElementById('ctl00_ContentPlaceHolder1_cbxAllCostcenter').checked = false;
}

function lookupARRouteAssignment(control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(300/2);
	popupWin = window.open("lookupARRouteAssignment.aspx?ctrl="+control,"Sample","scrollbars=no,resizable=no,width=750,height=300,top=" +top+ ",left=" +left);
	return false;
}
function lookupNewARRouteAssignment(control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupNewARRouteAssignment.aspx?ctrl=" + control, "_blank", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupARRouteAssignment(control, valId, valC1, valC2, valAP)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+control).value = valId;
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'C1').value = valC1;
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'C2').value = valC2;
    document.getElementById('ctl00_ContentPlaceHolder1_'+control+'AP').value = valAP;
}

function GetValueFrom_lookupNewARRouteAssignment(control, valId, valC1, valC2, valAP) {
    document.getElementById(control).value = valId;
    document.getElementById(control + 'C1').value = valC1;
    document.getElementById(control + 'C2').value = valC2;
    document.getElementById(control + 'AP').value = valAP;
}

function lookupARRouteAssignmentRC(control) {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupARRouteAssignmentRC.aspx?ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupARRouteAssignmentRC(control, valId, valC1, valC2, valAP) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = valId;
    document.getElementById('ctl00_ContentPlaceHolder1_' + control + 'C1').value = valC1;
    document.getElementById('ctl00_ContentPlaceHolder1_' + control + 'C2').value = valC2;
    document.getElementById('ctl00_ContentPlaceHolder1_' + control + 'AP').value = valAP;

    if (control == 'txtFromRoute')
        __doPostBack();
}

function lookupARRepRouteID()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupARRepRouteID.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARRepRouteID(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtRouteId').value = val;
}


function lookupARRepCostcenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupARRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARRepCostcenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}


function lookupARRepCheckerApprover(control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupARRepCheckerApprover.aspx?ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARRepCheckerApprover(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txt'+ control).value = val;
}

function lookupARRepTransaction()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupARRepTransaction.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARRepTransaction(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtTransaction').value = val;
}

function lookupARRepEmployee()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupARRepEmployee.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupARRepEmployee(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupNotification(type, id) {
    var left = (screen.width / 2) - (375 / 2);
    var top = (screen.height / 2) - (200 / 2);
    popupWin = window.open("lookupNotification.aspx?ei=" + encodeURIComponent(id) + "&tp=" + encodeURIComponent(type), "_blank", "scrollbars=no,resizable=no,width=375,height=200,top=" + top + ",left=" + left);
    return false;
}

function allCostcenter()
{
    if (document.getElementById('ctl00_ContentPlaceHolder1_cbxAllCostcenter').checked) {
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostCenterCode').value = 'ALL';
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostCenterDesc').value = 'AVAILABLE TO ALL COSTCENTERS';
    }
    else {
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostCenterCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostCenterDesc').value = '';
    }
}

//Perth Added 11/29/2012
function lookupEmployeeApprovalRoute(transactionType) {
    try {
        isPostBack = false;
    }
    catch (err) {

    }
    var left = (screen.width / 2) - (735 / 2);
    var top = (screen.height / 2) - (400 / 2);
    var url = "lookupApprovalRouteEmployee.aspx?type=" + transactionType + "&uni=1";
    popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=735,height=400,top=" + top + ",left=" + left);
    return false;
}

function Addk(id) {
    Add(id, '', '');
}

function Add(id, approver, approverName) {
    AddEdit(id, approver, approverName, '', '');
}

function AddEdit(id, approver, approverName, userCode, userName) {
    var state = document.getElementById('ctl00_ContentPlaceHolder1_hfState').value;
    var table = document.getElementById(id);
    var tbody = table.getElementsByTagName('TBODY')[0];
    var oRow = table.rows[(table.rows.length) - 1];

    var nameChecker = 'ctl00_ContentPlaceHolder1_txtChecker1Id';
    var nameCheckerName = 'ctl00_ContentPlaceHolder1_txtChecker1Name';
    var nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactions';
    var arrow = 'arrow';

    var newrow = tbody.appendChild(table.rows[0].cloneNode(true));
    if (state == 'EDIT') {
        nameChecker = 'ctl00_ContentPlaceHolder1_txtChecker1EditId';
        nameCheckerName = 'ctl00_ContentPlaceHolder1_txtChecker1EditName';
        nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactionsEdit';
        arrow = 'arrowEdit';

        newrow.getElementsByTagName('INPUT')['ctl00_ContentPlaceHolder1_txtUserCodeId'].value = userCode;
        newrow.getElementsByTagName('INPUT')['ctl00_ContentPlaceHolder1_txtUserCodeName'].value = userName;
    }
    else {
        newrow.getElementsByTagName('INPUT')[0].checked = false;
    }
    newrow.getElementsByTagName('INPUT')[nameChecker].value = approver;
    newrow.getElementsByTagName('INPUT')[nameCheckerName].value = approverName;
    newrow.getElementsByTagName('INPUT')[nameTransactions].value = '';
    newrow.children[arrow].textContent = '    ';

    var newrow = tbody.appendChild(table.rows[1].cloneNode(true));

    position = oRow.rowIndex + 1;
}

function deleteRowk(tableID) 
{
    var table = document.getElementById(tableID);
    var rowCount = table.rows.length;

    var countChecked = 0;

    for (var i = 0; i < rowCount; i++) {
        var oRow = table.rows[i];
        var oChkbox = oRow.cells[0].children[0];
        if (null != oChkbox && true == oChkbox.checked) {
            countChecked++;
        }
    }
    if (rowCount/2 == countChecked) {
        Addk(tableID);
    }

    if (table.rows.length > 2) {
        for (var i = 0; i < rowCount; i++) {
            if (i >= 0) {
                var row = table.rows[i];
                var chkbox = row.cells[0].children[0];
                if (null != chkbox && true == chkbox.checked) {

                    table.deleteRow(i);
                    table.deleteRow(i);
                    position = table.rows.length - 2;
                    rowCount -= 2;
                    i -= 2;
                } 
            }
        }
    }
}

function setPosition(evt) 
{
    var elem = evt.srcElement;
    var row = elem.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    setPos(row);
}

function setPos(row) {
    position = row.rowIndex;
    var state = document.getElementById('ctl00_ContentPlaceHolder1_hfState').value;
    var tableName = 'overrideApprover';
    var arrow = 'arrow';
    if (state == 'EDIT') {
        tableName = 'overrideApproverEdit';
        arrow = 'arrowEdit';
    }
    var table = document.getElementById(tableName);
    for (var i = 0; i < table.rows.length; i += 2) {
        row = table.rows[i];
        if (i == position)
            row.cells[arrow].innerText = '◄';
        else
            row.cells[arrow].innerText = '    ';
    }
}

function setInnerPosition(evt) {
    var elem = evt.srcElement;
    var row = elem.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    setPos(row);
}

function setCheckBoxPosition(evt) {
    var elem = evt.srcElement;
    var row = elem.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    setPos(row);
}

function lookupApprovalOverrideCheckerApprover() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (300 / 2);
    popupWin = window.open("lookupAOApproverChecker.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=300,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupAOCheckerApprover(control, valId, valName) {
    var table = document.getElementById('overrideApprover');
    var row = table.rows[position];

    row.cells[1].getElementsByTagName('INPUT')[0].value = valId;
    row.cells[1].getElementsByTagName('INPUT')[2].value = valName;
}

function UpdateHiddenField(evt) {
    setCheckBoxPosition(evt);
    var checked = evt.srcElement.checked;
    var name = evt.srcElement.id.replace("ctl00_ContentPlaceHolder1_", "");
    var state = document.getElementById('ctl00_ContentPlaceHolder1_hfState').value;
    var tableName = 'overrideApprover';
    var transactionName = 'ctl00_ContentPlaceHolder1_hTransactions';
    if (state == 'EDIT') {
        tableName = 'overrideApproverEdit';
        transactionName = 'ctl00_ContentPlaceHolder1_hTransactionsEdit';
        name = name.substring(0, name.length-4);
    }
    var table = document.getElementById(tableName);
    var row = table.rows[position];
    var hTransactions = row.getElementsByTagName('INPUT')[transactionName].value;
    var splitTransactions = hTransactions.split("|");

    if (findArray(splitTransactions, name)) {
        if (checked == false) {
            row.getElementsByTagName('INPUT')[transactionName].value = hTransactions.replace(name + "|", "").replace(name, "");
        }
    }
    else if (checked == true) {
        row.getElementsByTagName('INPUT')[transactionName].value += name + "|";
    }
}

function findArray(arrayList, name) {
    var found = false;
    for (var i = 0; i < arrayList.length && !found; i++) {
        if (arrayList[i] == name) {
            found = true;
        }
    }
    return found;
}

function lookupAORepEmployee() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupAORepEmployee.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupAORepEmployee(val, names) {
    var splitID = val.split(",");
    var splitNames = names.split(",");
    for (var i = 0; i < splitID.length; i++) {
        Add('overrideApprover', splitID[i], (i < splitNames.length) ? splitNames[i] : '');
    }
}

function checkAll(evt) {
    var checked = evt.srcElement.checked;
    var table = document.getElementById('overrideApprover');
    for (var i = 0; i < table.rows.length; i += 2) {
        row = table.rows[i];
        row.getElementsByTagName('INPUT')[0].checked = checked;
    }
}

function checkAllTransaction(evt, transaction) {
    var checked = evt.srcElement.checked;
    var name = 'ctl00$ContentPlaceHolder1$' + transaction;
    var state = document.getElementById('ctl00_ContentPlaceHolder1_hfState').value;
    var nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactions';

    var table;
    if (state == 'EDIT') {
        table = document.getElementById('overrideApproverEdit');
        nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactionsEdit';
        name += 'Edit';
    }
    else {
        table = document.getElementById('overrideApprover');
    }

    for (var i = 0; i < table.rows.length; i += 2) {
        row = table.rows[i];
        row.getElementsByTagName('INPUT')[name].checked = checked;

        var hTransactions = row.getElementsByTagName('INPUT')[nameTransactions].value;
        if (checked == false) {
            row.getElementsByTagName('INPUT')[nameTransactions].value = hTransactions.replace(transaction + "|", "").replace(transaction, "");
        }
        else if (checked == true) {
            if (findArray(row.getElementsByTagName('INPUT')[nameTransactions].value.split('|'), transaction) == false) {
                row.getElementsByTagName('INPUT')[nameTransactions].value += transaction + "|";
            }
        }
    }
}

function ColorSelected(index) {
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var tbr = tbl.rows[parseInt(index, 10) + 1];
    var tbcRouteId = tbr.cells[0];

    for (var i = 1; i < tbl.rows.length; i++) {
        if (i % 2 != 0) {
            tbl.rows[i].style.backgroundColor = '#F7F7DE';
        }
        else {
            tbl.rows[i].style.backgroundColor = '#FFFFFF';
        }
    }
    tbr.style.backgroundColor = '#FF2233';
}

function SelectAll() {
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var cbxTemp = null;
    var cbxAALL = tbl.rows[0].cells[0].getElementsByTagName('INPUT')[0];
    for (var i = 1; i < tbl.rows.length; i++) {
        cbxTemp = tbl.rows[i].cells[0].getElementsByTagName('INPUT')[0];
        cbxTemp.checked = cbxAALL.checked;
    }
}

function Spooler() {
    var look;
    var state = document.getElementById('ctl00_ContentPlaceHolder1_hfState').value;

    if (state == 'EDIT') {
        look = document.getElementById('ctl00_ContentPlaceHolder1_txtChecker1EditId');
    }
    else {
        look = document.getElementById('ctl00_ContentPlaceHolder1_txtChecker1Id');
    }
    if (look != null && look.value.match(",") != null) {
        Manipulate(state);
    }
}

function Manipulate(state) {
    var nameChecker = 'ctl00_ContentPlaceHolder1_txtChecker1Id';
    var nameCheckerName = 'ctl00_ContentPlaceHolder1_txtChecker1Name';
    var nameUserCode = 'ctl00_ContentPlaceHolder1_txtUserCodeId';
    var nameUserName = 'ctl00_ContentPlaceHolder1_txtUserCodeName';
    var nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactions';
    var nameTable = 'overrideApprover';

    var userCode = '';
    var userName = '';

    if (state == 'EDIT') {
        nameChecker = 'ctl00_ContentPlaceHolder1_txtChecker1EditId';
        nameCheckerName = 'ctl00_ContentPlaceHolder1_txtChecker1EditName';
        nameTransactions = 'ctl00_ContentPlaceHolder1_hTransactionsEdit';
        nameTable = 'overrideApproverEdit';

        var userCode = document.getElementById(nameUserCode).value;
        var userName = document.getElementById(nameUserName).value;
    }

    var checker = document.getElementById(nameChecker).value;
    var checkerName = document.getElementById(nameCheckerName).value;
    var transactions = document.getElementById(nameTransactions).value;
    
    if (checker.charAt(checker.length - 1) != ',') {
        checker = checker + ",";
        checkerName = checkerName + ",";
        transactions = transactions + ",";
    }

    if (state == 'EDIT') {
        if (checker.charAt(checker.length - 1) != ',') {
            userCode = userCode + ",";
            userName = userName + ",";
        }
        document.getElementById(nameUserCode).value = "";
        document.getElementById(nameUserName).value = "";
    }

    document.getElementById(nameChecker).value = "";
    document.getElementById(nameCheckerName).value = "";
    document.getElementById(nameTransactions).value = "";
    var i = 0;
    while (checker.length > 1) {
        var table = document.getElementById(nameTable);
        var row;
        if (i == 0) {
            row = table.rows[0];
            row.getElementsByTagName('INPUT')[nameChecker].value = checker.substring(0, checker.indexOf(","));
            row.getElementsByTagName('INPUT')[nameCheckerName].value = checkerName.substring(0, checkerName.indexOf(","));
            
            if (state == 'EDIT') {
                row.getElementsByTagName('INPUT')[nameUserCode].value = userCode.substring(0, userCode.indexOf(","));
                row.getElementsByTagName('INPUT')[nameUserName].value = userName.substring(0, userName.indexOf(","));
            }
        }
        else {
            var splitUserCode = '';
            var splitUserName = '';
            if (state == 'EDIT') {
                splitUserCode = userCode.substring(0, userCode.indexOf(","));
                splitUserName = userName.substring(0, userName.indexOf(","));
            }
            AddEdit(nameTable, checker.substring(0, checker.indexOf(",")), checkerName.substring(0, checkerName.indexOf(",")), splitUserCode, splitUserName);
            row = table.rows[position];
        }

        var trns = transactions.substring(0, transactions.indexOf(","));

        var splittrns = trns.split("|");

        row.getElementsByTagName('INPUT')[nameTransactions].value = trns;
        row.getElementsByTagName('INPUT')[0].checked = false;
        
        for (var i = 0; i < splittrns.length; i++) {
            var transaction = splittrns[i];
            if (transaction != "") {
                var name = 'ctl00$ContentPlaceHolder1$' + transaction;
                if (state == 'EDIT') {
                    name += 'Edit';
                }
                if (row.getElementsByTagName('INPUT')[name] != null) {
                    row.getElementsByTagName('INPUT')[name].checked = true;
                }
            }
        }
        checker = checker.substring(checker.indexOf(",") + 1, checker.length);
        checkerName = checkerName.substring(checkerName.indexOf(",") + 1, checkerName.length);
        transactions = transactions.substring(transactions.indexOf(",") + 1, transactions.length);
        if (state == 'EDIT') {
            userCode = userCode.substring(userCode.indexOf(",") + 1, userCode.length);
            userName = userName.substring(userName.indexOf(",") + 1, userName.length);
        }
        i++;
    }
}

function deleteGridRow() {
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var found = false;
    if (tbl != null) {
        for (var i = 1; i < tbl.rows.length && !found; i++) {
            cbxTemp = tbl.rows[i].cells[0].getElementsByTagName('INPUT')[0];
            if (cbxTemp.checked) {
                found = true;
            }
        } 
    }
    if (found) {
        found = confirm('Are you sure you want to delete?');
    }
    else {
        alert('No record selected.');
    }
    return found;
}
function lookEmployeeApprovalRouteTransaction(transaction
                                            , employeeID
                                            , state) 
{
    var left = (screen.width / 2) - (470 / 2);
    var top = (screen.height / 2) - (370 / 2);
    var url = "pgePopEmployeeApprovalRoute.aspx?transaction=" + transaction + "&employeeId=" + encodeURIComponent(employeeID) + "&state=" + state;
    popupWin4 = window.open(url, "Sample", "scrollbars=yes,resizable=no,width=470,height=370,top=" + top + ",left=" + left);
    popupWin4.focus();
    return false;
}

function ConfirmOnDelete(item)
{
 if (confirm("Are you sure to delete: " + item + "?")==true)
            return true;
          else
            return false;
}

function ColorSelected(index, gridName) {
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_' + gridName);
    var tbr = tbl.rows[parseInt(index, 10) + 1];
    var tbcRouteId = tbr.cells[0];

    for (var i = 1; i < tbl.rows.length; i++) {
        if (i % 2 != 0) {
            tbl.rows[i].style.backgroundColor = 'white';
        }
        else {
            tbl.rows[i].style.backgroundColor = 'white';
        }
    }
    tbr.style.backgroundColor = '#FF2233';
    document.getElementById('ctl00_ContentPlaceHolder1_hfRouteID').value = tbr.cells[2].innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfStartDate').value = tbr.cells[0].innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfEndDate').value = tbr.cells[1].innerHTML;
}

function clearControls() {
    if (document.getElementById('btnY').value == 'CLEAR') {
        document.getElementById('dtpOTStartDate').value = '';
        document.getElementById('dtpOTEndDate').value = '';
        document.getElementById('txtOvertime').value = '';
        document.getElementById('txtOvertimeC1').value = '';
        document.getElementById('txtOvertimeC2').value = '';
        document.getElementById('txtOvertimeAP').value = '';
        document.getElementById('hfState').value = '';
        document.getElementById('hfEmployeeID').value = '';
        document.getElementById('chkEndorse').checked = false;
        document.getElementById('chkReturn').checked = false;
        document.getElementById('chkApprove').checked = false;
        document.getElementById('chkDisapprove').checked = false;

        return false;
    }
    else {
        return true;
    }
}
function refreshPage() {
    window.opener.location.href = window.opener.location.href;
    window.close();
}
function GetValueFrom_lookupApprovalRouteEmployee(empId, empName, empNickname) {
    if (empId != '') {
        document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeID').value = empId.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeName').value = empName.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        document.getElementById('ctl00_ContentPlaceHolder1_hfCostCenter').value = empNickname.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        __doPostBack();
    }
    return false;
}
