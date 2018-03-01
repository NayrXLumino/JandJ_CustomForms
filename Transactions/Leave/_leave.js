// JScript File
// isPostBack variable is found in pgeMaster.master

function timeEntry(evt)
{
  var charCode = (evt.which) ? evt.which : event.keyCode
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    if(charCode != 58) 
       return false;
  return true;
}

function formatTime(control)
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_' + control).value.length == 2)
    {
        document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = document.getElementById('ctl00_ContentPlaceHolder1_' + control).value + ':';
    }
}

function page_ClientLoad()
{
    if(!isPostBack)
    {
        ddlType_ClientChanged();
        rblDayUnit_ClientChanged();
        isPostBack = true;
    }
    
}

function ddlType_ClientChanged() {
    try {
        var ddlType = document.getElementById('ctl00_ContentPlaceHolder1_ddlType');
        var ddlCode = ddlType.options[ddlType.selectedIndex].value;

        //Leave category controls
        //    if(ddlCode.substring(2,3) == '1')
        //    {
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').disabled = false;
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].text = '';
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].value = '';
        //    }
        //    else
        //    {
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').disabled = true;
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].text = '- not applicable -';
        //        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].value = '- not applicable -';
        //    }
        //    document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').selectedIndex = 0;

        //Day Unit controls
        if (document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value.toUpperCase() != ''
      && document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value.toUpperCase() != 'NEW')//transactions are loaded from checklist and editting is disabled
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = true;
        }
        else if (document.getElementById('ctl00_ContentPlaceHolder1_hfLVHRENTRY').value.toUpperCase() == 'FALSE') {
            var dayUnit = ddlCode.substring(4, 31);
            dayUnit = dayUnit.replace(/^\s*/, "").replace(/\s*$/, "");

            document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = true;
            if (dayUnit != '')//if no day unit then start and end is on entry
            {
                var valWH = dayUnit.search(/WH/i);
                var valHA = dayUnit.search(/HA/i);
                var valHP = dayUnit.search(/HP/i);
                var valQR = dayUnit.search(/QR/i);
                if (valWH != -1) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled = false;
                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled = true;
                }
                if (valHA != -1) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled = false;
                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled = true;
                }
                if (valQR != -1) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled = false;
                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled = true;
                }
                if (valHP != -1) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled = false;
                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled = true;
                }
            }
            else {
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled = true;

                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').checked = false;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').checked = false;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').checked = false;
                document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').checked = false;


                document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = false;
                document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = false;
            }

            //Start and End time control
            if (document.getElementById('ctl00_ContentPlaceHolder1_ddlType').value.toUpperCase().substring(0, 2) != 'OB'
          && document.getElementById('ctl00_ContentPlaceHolder1_ddlType').value.toUpperCase().substring(0, 2) != 'UN')//Andre added hard coded for OB and UN entry is always in hours
            {
                //document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = "";
                //document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = "";
            }

            rblDayUnit_ClientChanged(); //only applicable if LVHRENTRY is false

        }
        else// Leave entry is in hours based on LVHRENTRY
        {
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').checked = false;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').checked = false;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').checked = false;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').checked = false;

            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled = true;

            document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = false;
        }
    } catch (e) { 
    
    }
}

function rblDayUnit_ClientChanged()//only applicable if LVHRENTRY is false
{
    try {
        var i1 = document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value.substring(0, 2) + ":" + document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value.substring(2, 4);
        var o1 = document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value.substring(0, 2) + ":" + document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value.substring(2, 4);
        var i2 = document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value.substring(0, 2) + ":" + document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value.substring(2, 4);
        var o2 = document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value.substring(0, 2) + ":" + document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value.substring(2, 4);

        if ((document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value.toUpperCase() == ''
      || document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value.toUpperCase() == 'NEW'))//transactions are loaded from checklist and editting is disabled
        {
            if (document.getElementById('ctl00_ContentPlaceHolder1_hfCHIYODA').value.toUpperCase() == 'FALSE') {
                //
                //This is for settings that IS following the shift for the day no hours satisfaction for HA and HP number of hours
                //
                if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').checked)//WH
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = i1;
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = o2;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').checked)//HA
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = i1;
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = o1;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').checked)//QR
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = "";
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = "";
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').checked)//HP
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = i2;
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = o2;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                //
                //..END..END..This IS for settings that is following the shift for the day no hours satisfaction for HA and HP number of hours
                //
            }
            else {
                //
                //This is for settings that IS NOT following the shift for the day no hours satisfaction for HA and HP number of hours
                //CHIYODA offsetting is captured here
                var HA = 4;
                var HP = 5;

                if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').checked)//WH
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = i1;
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = o2;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').checked)//HA
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = i1;
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = computeEndTime(i1, 4);
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').checked)//QR
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = "";
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = "";
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                else if (document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').checked)//HP
                {
                    if (!document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').disabled) {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = computeStartTime(o2, 5);
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = o2;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
                        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
                    }
                }
                //
                //..END..END..This is for settings that IS NOT following the shift for the day no hours satisfaction for HA and HP number of hours
                //
            }
        }
    } catch (e) { 
    
    }
}


function disableControls()
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value != '')
    {
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller1').disabled = true;}
        catch(err){}
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller2').disabled = true;}
        catch(err){}
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller3').disabled = true;}
        catch(err){}
        document.getElementById('ctl00_ContentPlaceHolder1_ddlType').readOnly = true;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').readOnly = true;
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = true;
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = true;
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').readOnly = true;
    }
    else
    {
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller1').disabled = false;}
        catch(err){}
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller2').disabled = false;}
        catch(err){}
        try{document.getElementById('ctl00_ContentPlaceHolder1_btnFiller3').disabled = false;}
        catch(err){}
        document.getElementById('ctl00_ContentPlaceHolder1_ddlType').readOnly = false;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').readOnly = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').readOnly = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').readOnly = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').readOnly = false;
    }
}
function computeEndTime(startTime, hours)
{
    var _start = startTime;
    var _hours = hours;
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
        
        var actualTimeIn = (parseInt(_start.substring(0,2),10) * 60) + parseInt(_start.substring(2,4),10);
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
        if (shiftScheduleType == "G" && intFinalTime > 1440)
        {
            intFinalTime = intFinalTime - 1440 + (round * 1440);
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
        
        if(_hours.value == '' || _hours == 0)
        {
            return "";
        }
        else
        {
            if(finalEndTimeHours != 'X' && finalEndTimeMins != 'X')
            {
                return finalEndTimeHours +':'+ finalEndTimeMins;
            }
            else
            {
                return "";
            }
        }
    }
    else
    {
        return "";
    }
}

function computeStartTime(endTime, hours)
{
    var _end = endTime;
    var _hours = hours;
    _end = _end.replace(/:/, '');
    
    var finalStartTimeHours = 'X';
    var fianlStartTimeMins = 'X';
    
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
    
    
    if(_end != '' && _hours != '')
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
        
        var actualTimeOut = (parseInt(_end.substring(0,2),10) * 60) + parseInt(_end.substring(2,4),10);
        var actualTimeIn = actualTimeOut - parseInt((parseFloat(_hours) * 60),10);
        var excessFromPaid = 0;
        var intFinalTime = actualTimeIn;
	    
        //Start format
        finalStartTimeHours = Math.floor(intFinalTime / 60);
        finalStartTimeMins = intFinalTime - (Math.floor(intFinalTime / 60) * 60);
        
        if (finalStartTimeHours < 10)
        {
            finalStartTimeHours = '0' + finalStartTimeHours;
        }
        if (finalStartTimeMins < 10)
        {
            finalStartTimeMins = '0' + finalStartTimeMins;
        }
        
        if(_hours.value == '' || _hours == 0)
        {
            return "";
        }
        else
        {
            if(finalStartTimeHours != 'X' && finalStartTimeMins != 'X')
            {
                return finalStartTimeHours +':'+ finalStartTimeMins;
            }
            else
            {
                return "";
            }
        }
    }
    else
    {
        return "";
    }
}

function clearControls()
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_ddlType').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').selectedIndex = 0;
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVStartTime').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtLVEndTime').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_0').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_1').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_2').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_rblDayUnit_3').checked = false;
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfFromNotice').value = 'FALSE';
        
        
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        try{document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = '';}
        catch(err){}//Control is not shown. Controlled by server side
        
        //disableControls();
        ddlType_ClientChanged();
        rblDayUnit_ClientChanged();
        
        return false;
    }
    else
    {
        return true;
    }
}

//Special Leave Functions
function page_ClientLoadLeaveRange()
{
    if(!isPostBack)
    {
        ddlType_ClientChangedLeaveRange();
        isPostBack = true;
    }
}

function ddlType_ClientChangedLeaveRange()
{
    var ddlType = document.getElementById('ctl00_ContentPlaceHolder1_ddlType');
    var ddlCode = ddlType.options[ddlType.selectedIndex].value;
    
    if(ddlCode.substring(2,3) == '1')
    {
//        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').disabled = false;
//        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].text = '';
//        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].value = '';
    }
    else
    {
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').disabled = true;
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].text = '- not applicable -';
        document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').options[0].value = '- not applicable -';
    }
//    document.getElementById('ctl00_ContentPlaceHolder1_ddlCategory').selectedIndex = 0;
}


//Leave Cancellation
function AssignValueToControlNo(index)
{
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var tbr = tbl.rows[parseInt(index,10)+1];
    var remIndex = 0;
    //Affected columns after EmployeeId
    if(document.getElementById('ctl00_ContentPlaceHolder1_hfVWNICKNAME').value == "TRUE")
    {
        remIndex += 1;
    }
    else 
    {
        remIndex += 2;
    }
    if(document.getElementById('ctl00_ContentPlaceHolder1_hfDSPFULLNM').value != "TRUE")
    {
        remIndex += 3;
    }
    
    var tbcControlNo = tbr.cells[0];
    var tbcEmployeeId = tbr.cells[1];
    var tbcLeaveDate = tbr.cells[7 - parseInt(remIndex,10)];
    var tbcLeaveType = tbr.cells[8 - parseInt(remIndex,10)];
    var tbcStart = tbr.cells[11 - parseInt(remIndex,10)];
    var tbcEnd = tbr.cells[12 - parseInt(remIndex,10)];
    var tbcHours = tbr.cells[13 - parseInt(remIndex, 10)];
    var tbcDayUnit = tbr.cells[14 - parseInt(remIndex, 10)];
    var tbcShiftCode = tbr.cells[15 - parseInt(remIndex, 10)];
    
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
    document.getElementById('ctl00_ContentPlaceHolder1_txtControlNo').value = tbcControlNo.innerHTML;
    
    document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeId').value = tbcEmployeeId.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfLeaveDate').value = tbcLeaveDate.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfLeaveType').value = tbcLeaveType.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfStart').value = tbcStart.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfEnd').value = tbcEnd.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfLeaveHours').value = tbcHours.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfDayUnit').value = tbcDayUnit.innerHTML;
    document.getElementById('ctl00_ContentPlaceHolder1_hfShiftCode').value = tbcShiftCode.innerHTML;
}

//Leave Report scripts
function lookupRLVEmployee()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepEmployee.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVEmployee(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupRLVStatus()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepStatus.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVStatus(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupRLVCostcenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVCostcenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}

function lookupRLVCostcenterLine() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupLVRepCostCenterLine.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRLVCostcenterLine(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenterLine').value = val;
}

function lookupRLVPayPeriod()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepPayPeriod.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function lookupRLVLeaveYear() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupLVRepLeaveYear.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRLVLeaveYear(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtLeaveYear').value = val;
}


function GetValueFrom_lookupRLVPayPeriod(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val;
}

//added
function lookupRLVType()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepLeaveType.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVType(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtLeaveType').value = val;
}

function lookupRLVCategory()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepCategory.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVCategory(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCategory').value = val;
}

function lookupRLVDayUnit()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepDayUnit.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVDayUnit(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtDayUnit').value = val;
}
//end

function lookupRLVCheckerApprover(col, control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupLVRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRLVCheckerApprover(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+ control).value = val;
}

//Leave Notice
function lookupLVLeaveNotice()
{
    var left=(screen.width/2)-(875/2);
    var top=(screen.height/2)-(310/2);
	popupWin = window.open("lookupLVLeaveNotice.aspx" ,"Sample","scrollbars=no,resizable=no,width=875,height=310,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupLVLeaveNotice(employeeId, employeeName, employeeNickname, controlNo)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value = employeeId;
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeName').value = employeeName;
    document.getElementById('ctl00_ContentPlaceHolder1_txtNickname').value = employeeNickname;
    document.getElementById('ctl00_ContentPlaceHolder1_txtControlNo').value = controlNo;
    document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value = controlNo;
    document.getElementById('ctl00_ContentPlaceHolder1_hfFromNotice').value = 'TRUE';
    __doPostBack();
    return false;
}

function lookupLVLeaveNoticeOnEntry()
{
    var left=(screen.width/2)-(875/2);
    var top=(screen.height/2)-(310/2);
	popupWin = window.open("lookupLVLeaveNoticeOnEntry.aspx" ,"Sample","scrollbars=no,resizable=no,width=875,height=310,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupLVLeaveNoticeOnEntry(employeeId, employeeName, employeeNickname, controlNo)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value = employeeId;
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeName').value = employeeName;
    document.getElementById('ctl00_ContentPlaceHolder1_txtNickname').value = employeeNickname;
    document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value = controlNo;
    document.getElementById('ctl00_ContentPlaceHolder1_hfFromNotice').value = 'TRUE';
    __doPostBack();
    return false;
}