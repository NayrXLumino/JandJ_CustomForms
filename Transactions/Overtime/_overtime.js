// JScript File
function timeEntry(evt)
{
  var charCode = (evt.which) ? evt.which : event.keyCode
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    if(charCode != 58) 
       return false;
  return true;
}

function hoursEntry(evt)
{
  var charCode = (evt.which) ? evt.which : event.keyCode
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    if(charCode != 46)//20090104 andre uncommented for minutes entry
       return false;
    else
    {
        if(document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value == '')
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '0';
        if(document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value.indexOf('.') > -1)
            return false;
    }
  return true;
}
function AssignValueToControlNo(index) 
{
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var tbr = tbl.rows[parseInt(index, 10) + 1];
    var remIndex = 0;
    //Affected columns after EmployeeId
    if (document.getElementById('ctl00_ContentPlaceHolder1_hfVWNICKNAME').value == "TRUE") {
        remIndex += 1;
    }
    else {
        remIndex += 2;
    }
    if (document.getElementById('ctl00_ContentPlaceHolder1_hfDSPFULLNM').value != "TRUE") {
        remIndex += 3;
    }

    var tbcControlNo = tbr.cells[0];
    document.getElementById('ctl00_ContentPlaceHolder1_txtControlNo').value = tbcControlNo.innerHTML;
    //overtimecancellation 08/19/2013 ROBERT
    document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeId').value = tbcControlNo.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeId2').value = tbr.cells[1].innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_btnCancel').disabled=false;
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

function formatStartTime()
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value.length == 2)
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value + ':';
    }
    computeEndTime();
}
//function formatEndTime() 
//{
//    if (document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value.length == 2) 
//    {
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value + ':';
//    }
//    computeOTHours();
//}
//function computeOTHours() {

//    var OTFraction = document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRACTION').value;
//    if (parseInt(OTFraction) == 1) {
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').readOnly = false;
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').readOnly = true;
//    }
//    else {
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').readOnly = true;
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').readOnly = false;
//    }
//    if (checkIfHasCharactersInField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
//    {
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
//        return;
//    }
//    else 
//    {
//        if (document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value.length == 5
//            && !checkIfCorrectTimeField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
//        {
//            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
//            return;
//        }
//    }

//    if (document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value.length == 5) 
//    {
//            if (!checkIfCorrectTimeField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
//            {
//                document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
//                return;
//            }
//        var _startTime = document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value;
//        var _endTime = document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value;

//        _startTime = _startTime.replace(/:/, '');
//        _endTime = _endTime.replace(/:/, '');

//        var _start = parseFloat(_startTime.substring(0, 2),10) * 60 + parseFloat(_startTime.substring(2, 4),10);
//        var _end = parseFloat(_endTime.substring(0, 2),10) * 60 + parseFloat(_endTime.substring(2, 4),10);

//         var intOTHours = 'X';

//         var shiftTimeIn = document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value;
//         var shiftBreakStart = document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value;
//         var shiftBreakEnd = document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value;
//         var shiftTimeOut = document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value;
//         var shiftPaidBreak = document.getElementById('ctl00_ContentPlaceHolder1_hfShiftPaid').value;
//         var shiftScheduleType = document.getElementById('ctl00_ContentPlaceHolder1_hfShiftType').value;

//         var intTimeIn = parseInt(shiftTimeIn.substring(0, 2), 10) * 60 + parseInt(shiftTimeIn.substring(2, 4), 10);
//         var intBreakStart = parseInt(shiftBreakStart.substring(0, 2), 10) * 60 + parseInt(shiftBreakStart.substring(2, 4), 10);
//         var intBreakEnd = parseInt(shiftBreakEnd.substring(0, 2), 10) * 60 + parseInt(shiftBreakEnd.substring(2, 4), 10);
//         var intTimeOut = parseInt(shiftTimeOut.substring(0, 2), 10) * 60 + parseInt(shiftTimeOut.substring(2, 4), 10);
//         var intPaidBreak = parseInt(shiftPaidBreak, 10);

//         if (_startTime != '' && _endTime != '') 
//         {

//             if (shiftScheduleType == "G") {
//                 if (_end < _start)
//                 {
//                     _end += 1440;
//                 }
//                 if (intBreakStart < intTimeIn) {

//                     intBreakStart += 1440;
//                 }
//                 if (intBreakEnd < intTimeIn) 
//                 {
//                     intBreakEnd += 1440;
//                 }
//                 if (intTimeOut < intTimeIn) 
//                 {
//                     intTimeOut += 1440;
//                 }
//             }

//             intOTHours = (_end - _start);

//             if (_start <= intBreakStart && _end >= intBreakEnd) {
//                 intOTHours = intOTHours - (intBreakEnd - intBreakStart);
//             }
//             else if (_start <= intBreakStart && (_end > intTimeOut && _end <= intBreakEnd)) 
//             {
//                 intOTHours = intOTHours - (_end - intBreakStart);
//             }
//             else if ((_start >= intBreakStart && _start < intBreakEnd) && _end >= intBreakEnd) 
//             {
//                intOTHours = intOTHours - (intBreakEnd - _start);
//            }

//            if (_start >= intBreakStart && _start <= intBreakEnd) 
//            {
//                intPaidBreak = intPaidBreak - (intBreakEnd - _start);
//            }
//            else if (_end >= intBreakStart && _end <= intBreakEnd) 
//            {
//                intPaidBreak = intPaidBreak - (_end - intBreakStart);
//            }
//            else if (_start > intBreakEnd || _end <= intBreakStart) 
//            {
//                intPaidBreak = 0;
//            }
//                if (intPaidBreak < 0) 
//            {
//                intPaidBreak = 0;
//            }

//            if (intPaidBreak != 0 && parseFloat(_startTime) == parseFloat(shiftBreakEnd)) 
//            {
//                intOTHours = (intOTHours + intPaidBreak) / 60;
//            }
//            else if (intPaidBreak != 0 && parseFloat(_endTime) == parseFloat(shiftBreakStart)) 
//            {
//            
//                intOTHours = (intOTHours) / 60;
//            }
//            else 
//            {
//                intOTHours = (intOTHours + intPaidBreak) / 60;
//            }
//   

//            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "white";
//            if (_end.value == '' || _end == 0) 
//            {
//                document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
//                document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').innerText = '';
//                document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
//            }
//            else 
//            {
//                if (intOTHours != 'X') {
//                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value =parseFloat(intOTHours).toFixed(2);
//                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').innerText = parseFloat(intOTHours).toFixed(2);
//                }
//                else 
//                {
//                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
//                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').innerText = '';
//                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
//                }
//            }
//        }
//        else 
//        {
//            document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
//            document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').innerText = '';
//            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
//        }
//    }

//    else 
//    {
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
//        document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
//    }
//}
function computeEndTime() 
{
    
    if (checkIfHasCharactersInField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
        return;
    }
    else 
    {
        if (document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value.length == 5
            && !checkIfCorrectTimeField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
            {
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
            return;
        }
    }
    if(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value.length == 5) 
    {
        if (!checkIfCorrectTimeField(document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value)) 
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value = '';
            return;
        }
        var _start = document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').value;
        var _hours = document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value;

        if (_hours > 100) 
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '';
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').innerText = '';
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
            return;
        }

	    _start = _start.replace(/:/, '');
	    
	    var finalEndTimeHours = 'X';
	    var fianlEndTimeMins = 'X';
	    
	    var shiftTimeIn = document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value;
        var shiftBreakStart = document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value;
        var shiftBreakEnd = document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value;
        var shiftTimeOut = document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value;
        var shiftPaidBreak = document.getElementById('ctl00_ContentPlaceHolder1_hfShiftPaid').value;
        var shiftScheduleType = document.getElementById('ctl00_ContentPlaceHolder1_hfShiftType').value;
	    
        var intTimeIn = parseInt(shiftTimeIn.substring(0,2),10) * 60 + parseInt(shiftTimeIn.substring(2,4),10);
        var intBreakStart = parseInt(shiftBreakStart.substring(0,2),10) * 60 + parseInt(shiftBreakStart.substring(2,4),10);
        var intBreakEnd = parseInt(shiftBreakEnd.substring(0,2),10) * 60 + parseInt(shiftBreakEnd.substring(2,4),10);
        var intTimeOut = parseInt(shiftTimeOut.substring(0,2),10) * 60 + parseInt(shiftTimeOut.substring(2,4),10);
        var intPaidBreak = parseInt(shiftPaidBreak,10);
        
        
	    if(_start != '' && _hours != '')
	    {
	        if(shiftScheduleType == "G")
	        {
	            if(intBreakStart < intTimeIn)
	            {
	                intBreakStart += 1440;
	            }
	            if(intBreakEnd < intTimeIn)
	            {
	                intBreakEnd += 1440;
	            }
	            if(intTimeOut < intTimeIn)
	            {
	                intTimeOut += 1440;
	            }
	        }

	        var actualTimeIn = (parseInt(_start.substring(0, 2), 10) * 60) + parseInt(_start.substring(2, 4), 10);
	        var actualTimeOut = actualTimeIn + parseInt((parseFloat(_hours) * 60),10);
	        var excessFromPaid = 0;
	        var intFinalTime = actualTimeOut;
		    
	        if(actualTimeIn <= intBreakStart && actualTimeOut >= intBreakEnd)
	        {
	            intFinalTime = intFinalTime + (intBreakEnd - intBreakStart - intPaidBreak);
	        }
	        if (actualTimeOut > intBreakStart && actualTimeOut <= intBreakEnd)
            {
                excessFromPaid = (actualTimeOut - intBreakStart - intPaidBreak);
                if (excessFromPaid > 0)
                {
                    intFinalTime = intBreakEnd + excessFromPaid;
                }
            }
            
            //added for allowing OVERTIME for breaktime. Just comment if not used
            if(actualTimeIn >= intBreakStart && actualTimeIn <= intBreakEnd && actualTimeOut >= intBreakStart && actualTimeOut <= intBreakEnd && intPaidBreak == 0)
            {
                intFinalTime = intFinalTime - (intBreakEnd - intBreakStart);
            }
            //end allow overtimein break
            
            var round = intFinalTime / 1440;
            if (shiftScheduleType == "G" && intFinalTime > 1440 && (parseInt(shiftTimeOut.substring(0, 2), 10) * 60 + parseInt(shiftTimeOut.substring(2, 4), 10) > 1440))
            {
                intFinalTime = (intFinalTime - 1440) + (parseInt(round, 10) * 1440); //parseInt(round, 10)
            }
            //Start format
            finalEndTimeHours = Math.floor(intFinalTime / 60);
            finalEndTimeMins = intFinalTime - (Math.floor(intFinalTime / 60) * 60);
            
            if (finalEndTimeHours < 10)
            {
                finalEndTimeHours = '0' + finalEndTimeHours;
            }
            if (finalEndTimeMins < 10)
            {
                finalEndTimeMins = '0' + finalEndTimeMins;
            }
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "white";
            if(_hours.value == '' || _hours == 0)
            {
               document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '' ;
               document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').innerText = '';
               document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
            }
            else
            {
                if(finalEndTimeHours != 'X' && finalEndTimeMins != 'X')
                {
                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = finalEndTimeHours +':'+ finalEndTimeMins;
                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').innerText = finalEndTimeHours+':'+ finalEndTimeMins;
                }
                else
                {
                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '' ;
                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').innerText = '';
                    document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
                }
            }
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '' ;
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').innerText = '';
            document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
        }
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTStartTime').style.backgroundColor = "red";
    }
   
}

function checkIfCorrectTimeField(timeVar) {
    var ret = false;
    try {
        var myRegExp = new RegExp("[0-9][0-9]:[0-5][0-9]");
        if (myRegExp.test(timeVar)) {
            ret = true;
        }
        else {
            ret = false;
        }
    }
    catch (e) {
        ret = false;
    }
    return ret;
}

function checkIfHasCharactersInField(timeVar) {
    var ret = false;
    try {
        var myRegExp = new RegExp("^[0-9:]+$");
        if (!myRegExp.test(timeVar)) {
            ret = true;
        }
        else {
            ret = false;
        }
    }
    catch (e) {
        ret = false;
    }
    return ret;
}

function checkIfOnlyNumbersEntry(numberVal) {
    var ret = true;
    try {
        var myRegExp = new RegExp("^[0-9]+$");
        if (!myRegExp.test(numberVal)) {
            ret = false;
        }
        else {
            ret = true;
        }
    }
    catch (e) {
        ret = true;
    }
    return ret;
}

function clearControls()
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTHours').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtOTEndTime').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtJobCode').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtClientJobNo').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtClientJobName').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        
        return false;
    }
    else
    {
        return true;
    }
}

function lookupROTEmployee()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepEmployee.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTEmployee(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupROTStatus()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepStatus.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTStatus(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupROTCostcenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTCostcenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}

function lookupROTCostcenterLine() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupOTRepCostCenterLine.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupROTCostcenterLine(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenterLine').value = val;
}

function lookupROTPayPeriod()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepPayPeriod.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTPayPeriod(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val;
}

function lookupROTBatchNo()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepBatchNo.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTBatchNo(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtBatchNo').value = val;
}

function lookupOTJobCode()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(320/2);
	popupWin = window.open("lookupOTJobCode.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=320,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupOTJobCode(jobCode,jobNo,jobName)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtJobCode').value = jobCode;
    document.getElementById('ctl00_ContentPlaceHolder1_txtClientJobNo').value = jobNo;
    document.getElementById('ctl00_ContentPlaceHolder1_txtClientJobName').value = jobName;
}


function lookupROTCheckerApprover(col, control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupOTRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupROTCheckerApprover(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+ control).value = val;
}
