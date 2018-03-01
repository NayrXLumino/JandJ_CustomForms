// JScript File
position = 1;
sORe = null;
//splitPos = 0;
//currPos = 1;

// INPUT Tag index
var TXTJJOBCODEINDEX = 5;
var TXTJCLIENTNOINDEX = 6;
var TXTJCLIENTNAMEINDEX = 7;
var BTNSUBWORKINDEX = 8;
var TXTSUBWORKINDEX = 9;
var RBLBILLABLEINDEX = 10;  // Billabe Radio button Yes
var RBLBILLABLEINDEX2 = 11; // Billabe Radio button No
var CHKOVERTIMEINDEX = 12;
var HCATINDEX= 13;
var HCCTINDEX = 14;
var HSUBINDEX = 15;
var HCFLAGINDEX = 16;
var HBILLABLEINDEX = 17;
var HOVERTIMEINDEX = 18;

function Spooler()
{
    var look = document.getElementById('ctl00_ContentPlaceHolder1_txtJStart').value;
    if(look.match(",") != null)
   {
        Manipulate();
        readonlyControls();
       
   } 
   else {
        readonlyControls();
   }
    
}

function Manipulate() {
  var start = document.getElementById('ctl00_ContentPlaceHolder1_txtJStart').value;
  var end = document.getElementById('ctl00_ContentPlaceHolder1_txtJEnd').value;
  var hours = document.getElementById('ctl00_ContentPlaceHolder1_txtJHours').value;
  var jobcode = document.getElementById('ctl00_ContentPlaceHolder1_txtJJobCode').value;
  var clientno = document.getElementById('ctl00_ContentPlaceHolder1_txtJClientNo').value;
  var clientname = document.getElementById('ctl00_ContentPlaceHolder1_txtJClientName').value;
  var subwork = document.getElementById('ctl00_ContentPlaceHolder1_txtSubWork').value;
  var cct = document.getElementById('ctl00_ContentPlaceHolder1_hCCT').value;
  var cat = document.getElementById('ctl00_ContentPlaceHolder1_hCat').value;
  var hSub = document.getElementById('ctl00_ContentPlaceHolder1_hSub').value;
  var hCFlag = document.getElementById('ctl00_ContentPlaceHolder1_hCFlag').value;
  var billable = document.getElementById('ctl00_ContentPlaceHolder1_hBillable').value;  
  var overtime = document.getElementById('ctl00_ContentPlaceHolder1_hOvertime').value;  

  if (start.charAt(start.length - 1) != ',') {
      start = start + ",";
      end = end + ",";
      hours = hours + ",";
      jobcode = jobcode + ",";
      clientno = clientno + ",";
      clientname = clientname + ",";
      subwork = subwork + ",";
      cct = cct + ",";
      cat = cat + ",";
      hSub = hSub + ",";
      hCFlag = hCFlag + ",";
      billable = billable + ",";
      overtime = overtime + ",";
  }

    //clears the data
   document.getElementById('ctl00_ContentPlaceHolder1_txtJStart').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_txtJEnd').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_txtJHours').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_txtJJobCode').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_txtJClientNo').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_txtJClientName').value = ""; 
   document.getElementById('ctl00_ContentPlaceHolder1_txtSubWork').value = ""; 
   document.getElementById('ctl00_ContentPlaceHolder1_hCCT').value = ""; 
   document.getElementById('ctl00_ContentPlaceHolder1_hCat').value = ""; 
   document.getElementById('ctl00_ContentPlaceHolder1_hSub').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_hCFlag').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_hBillable').value = "";
   document.getElementById('ctl00_ContentPlaceHolder1_hOvertime').value = "";

   var table=document.getElementById('tst');
   var tbody = table.getElementsByTagName('TBODY')[0];
   var counter = 0;
    while(start.length > 1)
    {
      
      table=document.getElementById('tst');
      tbody=table.getElementsByTagName('TBODY')[0];
      //create rows
      var oRow = table.rows[(table.rows.length)-1];  
            
            var newrow=tbody.appendChild(table.rows[1].cloneNode(true));
              
              
            newrow.getElementsByTagName('INPUT')[0].checked=false;
            newrow.getElementsByTagName('INPUT')[1].value=start.substring(0,start.indexOf(","));
            newrow.getElementsByTagName('INPUT')[2].value=end.substring(0,end.indexOf(","));
            newrow.getElementsByTagName('INPUT')[3].value=hours.substring(0,hours.indexOf(","));
            newrow.getElementsByTagName('INPUT')[4].value='...';
            newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value=jobcode.substring(0,jobcode.indexOf(","));
            newrow.getElementsByTagName('INPUT')[6].value=clientno.substring(0,clientno.indexOf(","));
            newrow.getElementsByTagName('INPUT')[7].value = clientname.substring(0, clientname.indexOf(","));
            newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].value='...';
            newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value = subwork.substring(0, subwork.indexOf(","));
            
            newrow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = (overtime.substring(0, overtime.indexOf(",")).toLowerCase() == 'true');
            newrow.getElementsByTagName('INPUT')[HCATINDEX].value=cat.substring(0,cat.indexOf(","));
            newrow.getElementsByTagName('INPUT')[HCCTINDEX].value=cct.substring(0,cct.indexOf(","));
            newrow.getElementsByTagName('INPUT')[HSUBINDEX].value=hSub.substring(0,hSub.indexOf(","));
            //Added for lock control
            newrow.getElementsByTagName('INPUT')[HCFLAGINDEX].value=hCFlag.substring(0,hCFlag.indexOf(","));
            newrow.getElementsByTagName('INPUT')[HBILLABLEINDEX].value = billable.substring(0, billable.indexOf(","));
            newrow.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value = overtime.substring(0, overtime.indexOf(","));

            //changing the name of radio button in the next row
            setNameNewRadio(newrow);
            newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked = (cat.substring(0, cat.indexOf(",")).toLowerCase() == 'b');
            newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = (cat.substring(0, cat.indexOf(",")).toLowerCase() != 'b');

            //trimming the string
      start = start.substring(start.indexOf(",")+1, start.length);
      end = end.substring(end.indexOf(",")+1, end.length);
      hours = hours.substring(hours.indexOf(",")+1, hours.length);
      jobcode = jobcode.substring(jobcode.indexOf(",")+1, jobcode.length);
      clientno = clientno.substring(clientno.indexOf(",")+1, clientno.length);
      clientname = clientname.substring(clientname.indexOf(",")+1, clientname.length);
      subwork = subwork.substring(subwork.indexOf(",")+1, subwork.length);
      cat = cat.substring(cat.indexOf(",")+1, cat.length);
      cct = cct.substring(cct.indexOf(",")+1, cct.length);
      hSub = hSub.substring(hSub.indexOf(",")+1, hSub.length);
      hCFlag = hCFlag.substring(hCFlag.indexOf(",")+1, hCFlag.length);
      billable = billable.substring(billable.indexOf(",") + 1, billable.length);
      overtime = overtime.substring(overtime.indexOf(",") + 1, overtime.length);

      counter++;
   }
   table.deleteRow(1);
   position = table.rows.length - 1;
   //currPos = table.rows.length - 1;
}

function readonlyControls()
{
    var table = document.getElementById('tst');
    var tbody=table.getElementsByTagName('TBODY')[0]; 
    var rowCount = table.rows.length;  
    var stat = document.getElementById('ctl00_ContentPlaceHolder1_hiddenStatus').value;
    var b1 = document.getElementById('ctl00_ContentPlaceHolder1_break_Start').value;
    var b2 = document.getElementById('ctl00_ContentPlaceHolder1_break_End').value;
    var cmpFlag = document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value; 
    var data = 0;
    var accumulated = 0;

   for(var i=1; i<rowCount; i++) 
    {  
        var oRow = table.rows[i];  
            var forNOTSET = "FALSE";
            var newrow=table.rows[i];
            var temp = 0;
            if(newrow.getElementsByTagName('INPUT')[3].value != "")
            {
            //    accumulated += parseInt(newrow.getElementsByTagName('INPUT')[3].value,10);
                var temp2 = newrow.getElementsByTagName('INPUT')[3].value;
                temp = parseFloat(temp2);
            }

            newrow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = newrow.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value.toLowerCase() == 'true';
            
            if(newrow.getElementsByTagName('INPUT')[HSUBINDEX].value == '2')
            {
                newrow.getElementsByTagName('INPUT')[1].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[2].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[3].className = 'isLockedV2';
                newrow.getElementsByTagName('INPUT')[4].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[6].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[7].className = 'isLocked';
                //newrow.getElementsByTagName('INPUT')[8].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].className = 'isLocked';
                

                newrow.getElementsByTagName('INPUT')[0].disabled = true;
                newrow.getElementsByTagName('INPUT')[1].readOnly = true;
                newrow.getElementsByTagName('INPUT')[2].readOnly = true;
                newrow.getElementsByTagName('INPUT')[3].readOnly = true;
                newrow.getElementsByTagName('INPUT')[4].disabled = true;
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].readOnly = true;
                newrow.getElementsByTagName('INPUT')[6].readOnly = true;
                newrow.getElementsByTagName('INPUT')[7].readOnly = true;
                //newrow.getElementsByTagName('INPUT')[8].disabled = true;
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].readOnly = true;

            }
            accumulated += temp;
            
//            //Added 20090930
//            //endtime recalculation when break is overlapped
//            ////////////////////////////////////////////////
//            var _start = newrow.getElementsByTagName('INPUT')[1].value;
//            var _end = newrow.getElementsByTagName('INPUT')[2].value;
//            _start = _start.replace(/:/,'');
//            _end = _end.replace(/:/,'');
//            
//            if(parseInt(_start,10) <= parseInt(b1,10) && parseInt(_end,10) >= parseInt(b2, 10))
//            {
//                var breakdiff = (parseFloat(b2.substring(0,2)) * 60) - (parseFloat(b1.substring(0,2))* 60) + parseFloat(b2.substring(2,4)) - parseFloat(b1.substring(2,4));
//                //alert(breakdiff);
//                data = (parseFloat(_end.substring(0,2)) * 60) - (parseFloat(_start.substring(0,2))* 60) + parseFloat(_end.substring(2,4)) - parseFloat(_start.substring(2,4));
//                //data = (parseInt(_end,10) - parseInt(_start,10)) / 100;
//                data = (parseFloat(data - breakdiff)) /60.0;
//                newrow.getElementsByTagName('INPUT')[3].value = data.toFixed(2);
//            }
//            //End of recalculation code

            // transferred to setposition() function so that arrow will be real time
//            if (i == position) {
//                newrow.cells[12].innerText = '◄';
//            }
//            else
//                newrow.cells[12].innerText = '';
            
            newrow.getElementsByTagName('INPUT')[3].readOnly = true;
            
            newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].readOnly = true;
            newrow.getElementsByTagName('INPUT')[6].readOnly = true;
            newrow.getElementsByTagName('INPUT')[7].readOnly = true;
//            newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
            newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].readOnly = true;
            newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].disabled = true;
            newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].disabled= true;
            newrow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].disabled = true;  

            if (newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value != '') {
                //newrow.getElementsByTagName('SELECT')[0].disabled = false;   
                if(newrow.getElementsByTagName('INPUT')[HSUBINDEX].value != '' && newrow.getElementsByTagName('INPUT')[HSUBINDEX].value != 'NOTSET') {
            
                    newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value = newrow.getElementsByTagName('INPUT')[HSUBINDEX].value; 
//                    newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
                    forNOTSET = "TRUE";
                }
                else
                {
                    newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = false;   
                    forNOTSET = "FALSE";
                }
            }
            else
            {
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
                forNOTSET = "TRUE";
            }
            
            
            if (document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "1" || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "9" || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "0" || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "")
            {
               newrow.getElementsByTagName('INPUT')[1].style.backgroundColor = "#fff";
//               newrow.getElementsByTagName('INPUT')[2].style.backgroundColor = "#fff";
               newrow.getElementsByTagName('INPUT')[0].disabled = false;
               newrow.getElementsByTagName('INPUT')[1].readOnly = false;
               newrow.getElementsByTagName('INPUT')[2].readOnly = true;
               newrow.getElementsByTagName('INPUT')[3].readOnly = false;
               //for now because the input of work hours then compute for start and end does not work yet

               newrow.getElementsByTagName('INPUT')[4].disabled = false;
               if (newrow.getElementsByTagName('INPUT')[HCATINDEX].value == 'X' || newrow.getElementsByTagName('INPUT')[HCATINDEX].value == 'D' 
                    || newrow.getElementsByTagName('INPUT')[HCATINDEX].value == 'T') {
                   newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = true;
                   newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].disabled = true;
                   newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].disabled = true;
               }
               else 
               {
                   newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].disabled = false;
                   newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].disabled = false;
               }
               if (forNOTSET == "TRUE") { }
               //                  newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
               else
                   newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = false;
            }
            else
            {
               newrow.getElementsByTagName('INPUT')[1].style.backgroundColor = "#f3f3f3";
               newrow.getElementsByTagName('INPUT')[2].style.backgroundColor = "#f3f3f3";
               newrow.getElementsByTagName('INPUT')[3].style.backgroundColor = "#f3f3f3";
               newrow.getElementsByTagName('INPUT')[0].disabled = true;
               newrow.getElementsByTagName('INPUT')[1].readOnly = true;
               newrow.getElementsByTagName('INPUT')[2].readOnly = true;
               newrow.getElementsByTagName('INPUT')[3].readOnly = true;
               newrow.getElementsByTagName('INPUT')[4].disabled = true;
               
               if (forNOTSET == "TRUE")
                  newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
               else
                  newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = false;
            }

           if (stat == '9' && document.getElementById('ctl00_ContentPlaceHolder1_hiddenType').value != 'mod')
           {
               newrow.getElementsByTagName('INPUT')[0].disabled = true;
               newrow.getElementsByTagName('INPUT')[1].readOnly = true;
               newrow.getElementsByTagName('INPUT')[2].readOnly = true;
               newrow.getElementsByTagName('INPUT')[3].readOnly = true;
               newrow.getElementsByTagName('INPUT')[4].disabled = true;
               newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].readOnly = true;
               newrow.getElementsByTagName('INPUT')[6].readOnly = true;
               newrow.getElementsByTagName('INPUT')[7].readOnly = true;
               newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].disabled = true;
               newrow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].disabled = true;
               
               if (forNOTSET == "TRUE")
                  newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
               else
                  newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = false;

               newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].readOnly = true;
               //newrow.getElementsByTagName('SELECT')[0].disabled = true;
               if (document.getElementById('btnRecent') != null)
                    document.getElementById('btnRecent').disabled = true;
           }
           
           if(newrow.getElementsByTagName('INPUT')[HCFLAGINDEX].value == '2')
            {
                newrow.getElementsByTagName('INPUT')[1].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[2].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[3].className = 'isLockedV2';
                newrow.getElementsByTagName('INPUT')[4].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[6].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[7].className = 'isLocked';
                //newrow.getElementsByTagName('INPUT')[8].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].className = 'isLocked';
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].className = 'isLocked';
                
                newrow.getElementsByTagName('INPUT')[0].disabled = true;
                newrow.getElementsByTagName('INPUT')[1].readOnly = true;
                newrow.getElementsByTagName('INPUT')[2].readOnly = true;
                newrow.getElementsByTagName('INPUT')[3].readOnly = true;
                newrow.getElementsByTagName('INPUT')[4].disabled = true;
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].readOnly = true;
                newrow.getElementsByTagName('INPUT')[6].readOnly = true;
                newrow.getElementsByTagName('INPUT')[7].readOnly = true;
                //newrow.getElementsByTagName('INPUT')[8].disabled = true;
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].disabled = true;
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].readOnly = true;
            }
//            //Condition to catch the start time and end time valuse must not be equal to the break values
//            var xyStart = newrow.getElementsByTagName('INPUT')[1].value.replace(/:/, '');
//            var xyEnd = newrow.getElementsByTagName('INPUT')[2].value.replace(/:/, '');
//            if(xyStart.length == 4 && xyStart == b1)
//                newrow.getElementsByTagName('INPUT')[1].value = b2.substring(0,2) + ':' +  b2.substring(2,4);
//            if(xyEnd.length == 4 && xyEnd == b2)
//                newrow.getElementsByTagName('INPUT')[2].value = b1.substring(0,2) + ':' +  b1.substring(2,4);
//            //End
            
            
    } 
    //alert(stat); 
    if(stat != '9')
   {
       if ((checkEntry('tst') && document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value != "")
            || document.getElementById('ctl00_ContentPlaceHolder1_txtDate').value.replace(/ /,'') == "")
        { 
            document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = false;
            document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = false;
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
        }
   }
   else
   {
        document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = true;
        document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
   }
   
   /////////////////////////////////////////////////////////////////////////////////////
   /////////////////////Exclusive trappings for jobsplit modification only.....
   if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenType').value == 'mod')
   {
       if(cmpFlag == 'X')
       {
                document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = true;
      }
      else
      {
            if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenFlag').value == '9' || document.getElementById('ctl00_ContentPlaceHolder1_hiddenFlag').value == '1')
            {
                if (checkEntry('tst')) 
                {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = false;
                    document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = false;
                }
                
                if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenFlag').value == 1)
                {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = false;
                }
                else
                {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
                }


            }
            else if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenFlag').value == "")//No date was retrieved
            {
                if (checkEntry('tst'))
                    document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = false;
                
                document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = false;
            }
            else if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenRoute').value == '1')// At this point data retrieved status may only be one of the following: 3,5,or 7
            {
                document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = false;
                document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = false;
                document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = false;
            }
            else
            {//Logged on user is not the checker of this transaction which in on route
                document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = true;
            } 
        }
   }
   
   //MAXOTHR
   //if(parseInt(accumulated,10) > parseInt(document.getElementById('ctl00_ContentPlaceHolder1_hfMaxHours').value))
   //{
   //     document.getElementById('ctl00_ContentPlaceHolder1_btnZ').disabled = true;
   //     document.getElementById('ctl00_ContentPlaceHolder1_btnX').disabled = true;
   //     if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenType').value == 'mod')
   //     {
   //         document.getElementById('ctl00_ContentPlaceHolder1_btnY').disabled = true;
   //     }
   //}
  
  
  
  if(accumulated=="0")
  {
        document.getElementById('ctl00_ContentPlaceHolder1_txtTotalHours').value = "" ;
  }
  else
  {
        document.getElementById('ctl00_ContentPlaceHolder1_txtTotalHours').value = accumulated.toFixed(2) ;
  }
  
  
   //END OF -- if(document.getElementById('ctl00_ContentPlaceHolder1_hiddenType').value == 'mod')
   /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}//end of readonly controls

// also auto start time
function autoEndtime() {

    var OTFRAC = document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRAC').value;

    shift = document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value;

    var breakStartTime = shift.substring(16, 21);
    var breakStartHour = breakStartTime.substring(0, 2);
    var breakStartMin = breakStartTime.substring(3, 5);


    var breakEndTime = shift.substring(23, 28);
    var breakEndHour = breakEndTime.substring(0, 2);
    var breakEndMin = breakEndTime.substring(3, 5);

    //Convert hour in minutes
    breakStartHour = parseInt(breakStartHour, 10) * 60;
    breakStartMin = parseInt(breakStartMin, 10) + parseInt(breakStartHour, 10);

    breakEndHour = parseInt(breakEndHour, 10) * 60;
    breakEndMin = parseInt(breakEndMin, 10) + parseInt(breakEndHour, 10);

    var breakMin = breakEndMin - breakStartMin;

    var table = document.getElementById('tst');
    var tbody = table.getElementsByTagName('TBODY')[0];
    var oRow = table.rows[position];

    oRow.getElementsByTagName('INPUT')[1].value = formatTime(oRow.getElementsByTagName('INPUT')[1].value, OTFRAC);
    var start = oRow.getElementsByTagName('INPUT')[1].value;

    if (start.length == '5') 
    {
        
        var startHourInMin = parseInt(start.substring(0, 2), 10) * 60;
        var startMin = parseInt(start.substring(3, 5), 10);

        var workHoursInMin = parseFloat(oRow.getElementsByTagName('INPUT')[3].value, 10) * 60;

        var startTimeInMin = startHourInMin + startMin;

        var endTimeInMin = startTimeInMin + workHoursInMin;

        if (startTimeInMin >= breakStartMin && startTimeInMin < breakEndMin) 
        {
//            var hour = ((startTimeInMin + breakMin) / 60).toString();
//            if (hour.indexOf(".", 0) > 0)    // convert hour into a whole number
//                hour = hour.substring(0, hour.indexOf(".", 0));
//            var min = startTimeInMin % 60;

//            if (hour < 10)
//                hour = '0' + hour;

//            if (min < 10)
//                min = '0' + min;

            oRow.getElementsByTagName('INPUT')[1].value = breakEndTime;
        }
            

        if (endTimeInMin > breakStartMin && startHourInMin <= breakStartMin)
            endTimeInMin += breakMin;

        //convert minutes back to hour
        var hour = (endTimeInMin / 60).toString();
        if (hour.indexOf(".", 0) > 0)    // convert hour into a whole number
            hour = hour.substring(0, hour.indexOf(".", 0));
        var min = endTimeInMin % 60;


        if (hour < 10)
            hour = '0' + hour;

        if (min < 10)
            min = '0' + min;

        oRow.getElementsByTagName('INPUT')[2].value = hour + ':' + min;
        

        var nRow = table.rows[position + 1];
        if (nRow != undefined) 
        {
            nRow.getElementsByTagName('INPUT')[1].value = hour + ':' + min;

//            var start = nRow.getElementsByTagName('INPUT')[1].value;
//            var end = nRow.getElementsByTagName('INPUT')[2].value;

//            var startHour = parseInt(start.substring(0, 2), 10) * 60;
//            var startMin = parseInt(start.substring(3, 5), 10);
//            start = startHour + startMin;

//            var endHour = parseInt(end.substring(0, 2), 10) * 60;
//            var endMin = parseInt(end.substring(3, 5), 10);
//            end = endHour + endMin;

//            var workHours = (end - start) / 60;

//            nRow.getElementsByTagName('INPUT')[3].value = workHours;

//            if (workHours <= 0) {
                nRow.getElementsByTagName('INPUT')[2].value = '';
                nRow.getElementsByTagName('INPUT')[3].value = '';
//            }
        }
    }
}


function selection()
{
    alert("im in selection");
/*
    var list = document.getElementById('ctl00_ContentPlaceHolder1_listBox');
    
    var table = document.getElementById('tst');
	var row = table.rows[position];
*/		    
    //row.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value= list.options[list.selectedIndex].text;
    //row.getElementsByTagName('INPUT')[6].value=val2;
    //row.getElementsByTagName('INPUT')[7].value=val3;
}

function formatTime(time, OTFRAC) {
    switch (time.length) {
        case 1:
            if (time.charAt(0) == ':')
                time = '';
            else if (time.charAt(0) != '0' && time.charAt(0) != '1' && time.charAt(0) != '2') {
                time = '0' + time;
            }
            break;
        case 2:
            if (time.charAt(1) == ':')
                time = '0' + time;
                // allow entry of 24:00, 25:00, etc.
//            else if (time.charAt(1) != '0' && time.charAt(1) != '1' && time.charAt(1) != '2' && time.charAt(1) != '3') {
//                if (time.charAt(0) == '2')
//                    time = time.substring(0, 1);
//            }
            break;
        case 4:
            if (time.charAt(3) == ':' || time.charAt(3) == '6' || time.charAt(3) == '7' || time.charAt(3) == '8' || time.charAt(3) == '9')
                time = time.substring(0, 3);
            break;
        case 5:
            if (time.charAt(4) == ':')
                time = time.substring(0, 4);
        default:
            break;
    }

    if (time.length == 2) {
        if (OTFRAC == '60.00')
            time = time + ':00';
        else
            time = time + ':';

    }

    return time;
}

function formatCompute(evt) {

    var OTFRAC = document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRAC').value;
    var MINOTHR = document.getElementById('ctl00_ContentPlaceHolder1_hfMinHours').value;
    var MINOTMIN = parseInt(MINOTHR * 60, 10);

    var table = document.getElementById('tst');
    var tbody = table.getElementsByTagName('TBODY')[0];
    var oRow = table.rows[position];
    //validate entry
    var start = oRow.getElementsByTagName('INPUT')[1].value;
    var end = oRow.getElementsByTagName('INPUT')[2].value;

    oRow.getElementsByTagName('INPUT')[1].value = formatTime(oRow.getElementsByTagName('INPUT')[1].value, OTFRAC);
    oRow.getElementsByTagName('INPUT')[2].value = formatTime(oRow.getElementsByTagName('INPUT')[2].value, OTFRAC);

    if (start.length == '3' || end.length == '3') {

    }
    //BEFORE COMPUTE HOURS CHECK AGAINST OT FRACTION TO ROUND DOWN 
    /////Added:20091223

    var tempHold = '0';
    var tempXY = '0';
    ////This if for round down function for the start time if the minutes is not divisible by OTFRAC 
    //Xif(start.length == '5')
    //X{
    //X    tempHold = start.substring(3,5);
    //X    if(parseInt(tempHold,10) /% parseInt(OTFRAC) != 0)
    //X    {
    //X        tempXY = parseInt(tempHold,10) % parseInt(OTFRAC, 10);
    //X        tempHold = parseInt(tempHold,10) - parseInt(tempXY,10);
    //X        oRow.getElementsByTagName('INPUT')[1].value = start.substring(0,2) + ':' + tempHold;
    //X    }
    //X} 
    ////This if for round down function for the end time if the minutes is not divisible by OTFRAC 
    //Xif(end.length == '5')
    //X{
    //X    tempHold = end.substring(3,5);
    //X    if(parseInt(tempHold,10) % parseInt(OTFRAC) != 0)
    //X    {
    //X        tempXY = parseInt(tempHold,10) % parseInt(OTFRAC, 10);
    //X        tempHold = parseInt(tempHold,10) - parseInt(tempXY,10);
    //X        if(tempHold < 10)
    //X        {
    //X            tempHold = '0' + tempHold;
    //X        }
    //X        oRow.getElementsByTagName('INPUT')[2].value = end.substring(0,2) + ':' + tempHold;
    //X    }
    //X}

    //////  END  //////////// 

    if (start.length == '5' && end.length == '5') {
        var shour = start.substring(0, 2);
        var smin = start.substring(3, 5);

        var ehour = end.substring(0, 2);
        var emin = end.substring(3, 5);
        /////This is for those start time and end time input is the same. System would add the OTFRAC value to endtime
        //Xif(start == end)
        //X{
        //X    if(parseInt(OTFRAC,10) < 60)
        //X    {
        //X         emin = parseInt(emin,10) + parseInt(OTFRAC,10);
        //X         if(emin < 10)
        //X         {
        //X             emin = '0' + emin;
        //X         }
        //X         oRow.getElementsByTagName('INPUT')[2].value = ehour + ':' + emin;
        //X    }
        //X    else
        //X    {
        //X         ehour = parseInt(ehour,10) + 1;
        //X         if(ehour < 10)
        //X         {
        //X             ehour = '0' + ehour;
        //X         }
        //X         oRow.getElementsByTagName('INPUT')[2].value = ehour + ':' + emin;
        //X    }
        //X}

        //Convert hours in minutes
        shour = parseInt(shour, 10) * 60;

        smin = parseInt(smin, 10) + parseInt(shour, 10);


        ehour = parseInt(ehour, 10) * 60;
        emin = parseInt(emin, 10) + parseInt(ehour, 10);

//        var thours = emin - smin;
        //alert(emin + '-' + smin); 
        //if(thours > 0 )
        //{ 
//        thours = thours / 60;
//        ehour = thours - parseInt(thours, 10);
//        var xyz = parseInt(thours, 10) + ehour;
//        oRow.getElementsByTagName('INPUT')[3].value = xyz.toFixed(2);
        //}
        //else
        //{
        //     oRow.getElementsByTagName('INPUT')[3].value = '0';
        //}


        // for trapping the shift of employee -added by Kelvin 04-01-2011
        shift = document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value;

        var shiftStart = shift.substring(8, 13);
        var shiftEnd = shift.substring(31, 36);

        var shiftStartHour = shiftStart.substring(0, 2);
        var shiftStartMin = shiftStart.substring(3, 5);

        var shiftEndHour = shiftEnd.substring(0, 2);
        var shiftEndMin = shiftEnd.substring(3, 5);

        //Convert hours in minutes
        shiftStartHour = parseInt(shiftStartHour, 10) * 60;
        shiftStartMin = parseInt(shiftStartMin, 10) + parseInt(shiftStartHour, 10);

        shiftEndHour = parseInt(shiftEndHour, 10) * 60;
        shiftEndMin = parseInt(shiftEndMin, 10) + parseInt(shiftEndHour, 10);

        var message = '';

        oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].enabled = true;


        if (document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value != 'REG' && document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value != '') {
            oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = true;
        }
        else {
            if (smin >= shiftEndMin && emin > shiftEndMin && emin - shiftEndMin >= MINOTMIN) //temp MINOTMIN to be fixed
            {
                oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = true;
            }
            else if (smin < shiftStartMin && emin <= shiftStartMin && shiftStartMin - smin >= MINOTMIN) {
                oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = true;
            }
            else if (smin < shiftStartMin && emin > shiftStartMin) {
                if (document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == 'REG' || document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == '') {
                    if (document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '1' || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '9'
                    || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '0' || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '') {
                        var answer = confirm('The start time of the job you entered goes before your shift schedule. Do you want to make the job to be overtime');
                        if (answer) {
                            table.insertRow(position + 1);
                            table.rows[position + 1].appendChild(table.rows[position].cells[0].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[1].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[2].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[3].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[4].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[5].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[6].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[7].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[8].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[9].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[10].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[11].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[12].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[13].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[14].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[15].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[16].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[17].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[18].cloneNode(true));

                            shift = document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value;

                            var breakStart = shift.substring(16, 21);
                            var breakEnd = shift.substring(23, 28);

                            var breakStartHour = breakStart.substring(0, 2);
                            var breakStartMin = breakStart.substring(3, 5);

                            var breakEndHour = breakEnd.substring(0, 2);
                            var breakEndMin = breakEnd.substring(3, 5);

                            //Convert hour in minutes
                            breakStartHour = parseInt(breakStartHour, 10) * 60;
                            breakStartMin = parseInt(breakStartMin, 10) + parseInt(breakStartHour, 10);

                            breakEndHour = parseInt(breakEndHour, 10) * 60;
                            breakEndMin = parseInt(breakEndMin, 10) + parseInt(breakEndHour, 10);

                            var breakMin = breakEndMin - breakStartMin;


                            // for row 1
                            row1 = table.rows[position];
                            row1.getElementsByTagName('INPUT')[0].checked = false;
                            row1.getElementsByTagName('INPUT')[1].value = start;
                            row1.getElementsByTagName('INPUT')[2].value = shiftStart;

                            var workhours;
                            if (smin < breakStartMin && shiftStartMin > breakEndMin)
                                workhours = (shiftStartMin - smin - breakMin) / 60;
                            else
                                workhours = (shiftStartMin - smin) / 60;
                            row1.getElementsByTagName('INPUT')[3].value = workhours.toFixed(2);
                            row1.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = true;
                            row1.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value = 'true';

                            // row 2
                            row2 = table.rows[position + 1];
                            row2.getElementsByTagName('INPUT')[0].checked = false;
                            row2.getElementsByTagName('INPUT')[1].value = shiftStart;
                            row2.getElementsByTagName('INPUT')[2].value = end;

                            //Kelvin modified: 20110415 - added toFixed() to display decimal values of work hours
                            row2.getElementsByTagName('INPUT')[3].value = ((emin - shiftStartMin) / 60).toFixed(2);

                            row2.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
                            row2.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value = 'false';

                            //Kelvin added code below: 20110415 - copy the value of checkbox in row 1 to checkbox in row 2 because CHECKED is not copied by clone node
                            //row2.getElementsByTagName('INPUT')[8].checked = row1.getElementsByTagName('INPUT')[8].checked;

                            // change the name of the new radio button Element
                            setNameNewRadio(row2);

                        }
                        else {
                            oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
                        }
                    }
                }
            }
            else if (smin < shiftEndMin && emin > shiftEndMin && emin - shiftEndMin >= MINOTMIN) {
                if (document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == 'REG' || document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == '') {
                    if (document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '1' || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '9'
                    || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '0' || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == '') {
                        var answer = confirm('The end time of the job you entered goes beyond your shift schedule. Do you want to make the job to be overtime');
                        if (answer) {
                            table.insertRow(position + 1);
                            table.rows[position + 1].appendChild(table.rows[position].cells[0].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[1].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[2].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[3].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[4].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[5].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[6].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[7].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[8].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[9].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[10].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[11].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[12].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[13].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[14].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[15].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[16].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[17].cloneNode(true));
                            table.rows[position + 1].appendChild(table.rows[position].cells[18].cloneNode(true));

                            shift = document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value;

                            var breakStart = shift.substring(16, 21);
                            var breakEnd = shift.substring(23, 28);

                            var breakStartHour = breakStart.substring(0, 2);
                            var breakStartMin = breakStart.substring(3, 5);

                            var breakEndHour = breakEnd.substring(0, 2);
                            var breakEndMin = breakEnd.substring(3, 5);

                            //Convert hour in minutes
                            breakStartHour = parseInt(breakStartHour, 10) * 60;
                            breakStartMin = parseInt(breakStartMin, 10) + parseInt(breakStartHour, 10);

                            breakEndHour = parseInt(breakEndHour, 10) * 60;
                            breakEndMin = parseInt(breakEndMin, 10) + parseInt(breakEndHour, 10);

                            var breakMin = breakEndMin - breakStartMin;


                            // for row 1
                            row1 = table.rows[position];
                            row1.getElementsByTagName('INPUT')[0].checked = false;
                            row1.getElementsByTagName('INPUT')[1].value = start;
                            row1.getElementsByTagName('INPUT')[2].value = shiftEnd;

                            var workhours;
                            if (smin < breakStartMin && shiftEndMin > breakEndMin)
                                workhours = (shiftEndMin - smin - breakMin) / 60;
                            else
                                workhours = (shiftEndMin - smin) / 60;
                            row1.getElementsByTagName('INPUT')[3].value = workhours;
                            row1.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
                            row1.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value = 'false';

                            // row 2
                            row2 = table.rows[position + 1];
                            row2.getElementsByTagName('INPUT')[0].checked = false;
                            row2.getElementsByTagName('INPUT')[1].value = shiftEnd;
                            row2.getElementsByTagName('INPUT')[2].value = end;

                            //Kelvin modified: 20110415 - added toFixed() to display decimal values of work hours
                            row2.getElementsByTagName('INPUT')[3].value = ((emin - shiftEndMin) / 60).toFixed(2);

                            row2.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = true;
                            row2.getElementsByTagName('INPUT')[HOVERTIMEINDEX].value = 'true';

                            //Kelvin added code below: 20110415 - copy the value of checkbox in row 1 to checkbox in row 2 because CHECKED is not copied by clone node
                            //row2.getElementsByTagName('INPUT')[8].checked = row1.getElementsByTagName('INPUT')[8].checked;

                            // change the name of the new radio button Element
                            setNameNewRadio(row2);

                        }
                        else {
                            oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
                        }
                    }
                }
            }
            else {
                if (document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == 'REG' || document.getElementById('ctl00_ContentPlaceHolder1_txtDay').value == '')
                    oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
            }
        }
        oRow.cells[18].childNodes[0].value = oRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked.toString();

    }

}

function isNumberJS(evt) 
{
    flag = true;
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        flag = false;
    if (charCode == 46)
        flag = true;
    
    /////////Add-on/////////////////
    var table=document.getElementById('tst');
    var tbody=table.getElementsByTagName('TBODY')[0];
    var oRow = table.rows[position];
    var xRow= null;
    
    
    // && oRow.cells[2].childNodes[0].value.replace(/ /,'') != ""
    if(position > 1 && sORe == 's')//if start time is being editted
    {
         xRow = table.rows[position -1];
         var val1 = xRow.cells[2].childNodes[0].value;
         var val2 = oRow.cells[2].childNodes[0].value;
         var p = oRow.cells[1].childNodes[0].value;
         var hold = p.replace(/:/,'') + String.fromCharCode(evt.keyCode);
         //alert(hold +'-'+val1.substring(0, p.length+1).replace(/:/,'')+'-'+val2.substring(0, p.length+1).replace(/:/,''));
         if(val1 != "" && val1.length =='5' && flag)
         {
            //alert(parseInt(val.substring(p, p+1),10));
            //flag = false; remover 20090930 for update below
            //Added 20090930 for input allowed on endtime of row still blank
            if(val2.replace(/ /,'') != "")
            {
                flag = false;
            ////////////////////////////////////////////////////////////////    
                if(parseInt(hold,10) >= parseInt(val1.substring(0, p.length+1).replace(/:/,''),10) && parseInt(hold,10) <= parseInt(val2.substring(0, p.length+1).replace(/:/,''),10))
                    flag = true;
            }
            else if(val1.replace(/ /,'') != "")
            {
                flag = false;
                if(parseInt(hold,10) >= parseInt(val1.substring(0, p.length+1).replace(/:/,''),10))
                    flag = true;
            }
         }
    }
    ////////////////alert(table.rows.length);
     //alert(position > 1 && position < table.rows.length-1 && sORe == 'e');
     if(position > 1 && position < table.rows.length-1 && sORe == 'e')//if end time is being editted
     {
         xRow = table.rows[position +1];
         var val1 = oRow.cells[1].childNodes[0].value;
         var val2 = xRow.cells[1].childNodes[0].value;
         var p = oRow.cells[2].childNodes[0].value;
         var hold = p.replace(/:/,'') + String.fromCharCode(evt.keyCode);
         //alert(hold +'-'+val1.substring(0, p.length+1).replace(/:/,'')+'-'+val2.substring(0, p.length+1).replace(/:/,''));
         if(val1 != "" && val2.length =='5' && flag)
         {
            //alert(parseInt(val.substring(p, p+1),10));//Andre-test-
            flag = false;
            if(parseInt(hold,10) >= parseInt(val1.substring(0, p.length+1).replace(/:/,''),10) && parseInt(hold,10) <= parseInt(val2.substring(0, p.length+1).replace(/:/,''),10))
                flag = true;
         }
     } 
     
     if(position == table.rows.length-1 && sORe == 'e')//if end time is being editted
     {
         var val1 = oRow.cells[1].childNodes[0].value;
         var p = oRow.cells[2].childNodes[0].value;
         var hold = p.replace(/:/,'') + String.fromCharCode(evt.keyCode);
         //alert(hold +'-'+val1.substring(0, p.length+1).replace(/:/,'')+'-'+val2.substring(0, p.length+1).replace(/:/,''));
         if(val1 != "" && val1.length =='5' && flag)
         {
            //alert(parseInt(val.substring(p, p+1),10));
            flag = false;
            if(parseInt(hold,10) >= parseInt(val1.substring(0, p.length+1).replace(/:/,''),10))
                flag = true;
         }
     }
     var q = null;
     if(sORe == 's')
        q= oRow.cells[1].childNodes[0].value;
     else if (sORe == 'e')
        q= oRow.cells[2].childNodes[0].value;
     var OTFRAC = document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRAC').value;
     if (q != null && evt.keyCode != 48 && q.length > 2 && flag && OTFRAC == '60.00')
         flag = false;

    return flag;
}

function getRecent()
{
    var table1 = document.getElementById('tst');  
    var tbody1= table1.getElementsByTagName('TBODY')[0]; 
    var oRow1 = table1.rows[position];

    var list = document.getElementById('ctl00_ContentPlaceHolder1_listBox');
    lvalue = list.value;
    //alert(lvalue);
    if(list.selectedIndex >= 0)
   {  
        list = list.options[list.selectedIndex].text + '~'; 
        
        var val1 = trim(list.substring(0, list.indexOf('~')));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val2 = trim(list.substring(0, list.indexOf('~')));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val3 = trim(list.substring(0, list.indexOf('~')));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val4 = trim(list.substring(0, list.indexOf('~')));
        
        lvalue = lvalue + '~'; 
        
        var val5 = trim(lvalue.substring(0, lvalue.indexOf('~')));
        lvalue = lvalue.substring(lvalue.indexOf('~')+1, lvalue.length);
        
        var val6 = trim(lvalue.substring(0, lvalue.indexOf('~')));
        lvalue = lvalue.substring(lvalue.indexOf('~')+1, lvalue.length);
        
        var val7 = trim(lvalue.substring(0, lvalue.indexOf('~')));
        
        
        GetValueJS(val1, val2, val3, val4, val5, val6, val7);
       
    }


}

function split(id)
{
    var table=document.getElementById(id);
    var tbody=table.getElementsByTagName('TBODY')[0];
    var rowCount = table.rows.length;
    
    var xFlag = checkEntry('tst');
    if(xFlag)
    {  
        for(var i=1; i<rowCount; i++) 
        {  
            var oRow = table.rows[i];
            var oChkbox = oRow.cells[0].childNodes[0];  
            if(null != oChkbox && true == oChkbox.checked && oRow.getElementsByTagName('INPUT')[HSUBINDEX].value != '2')
            {  
                table.insertRow(i+1);
                table.rows[i+1].appendChild(table.rows[i].cells[0].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[1].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[2].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[3].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[4].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[5].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[6].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[7].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[8].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[9].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[10].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[11].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[12].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[13].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[14].cloneNode(true));
// added two rows
                table.rows[i + 1].appendChild(table.rows[i].cells[15].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[16].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[17].cloneNode(true));
                table.rows[i + 1].appendChild(table.rows[i].cells[18].cloneNode(true));

                table.rows[i].getElementsByTagName('INPUT')[3].value = '';

                var nRow = table.rows[i+1];
                nRow.getElementsByTagName('INPUT')[0].checked=false;
                nRow.getElementsByTagName('INPUT')[1].value='';
                //nRow.getElementsByTagName('INPUT')[2].value='';
                nRow.getElementsByTagName('INPUT')[3].value='';
                nRow.getElementsByTagName('INPUT')[4].value='...';
                nRow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value='';
                nRow.getElementsByTagName('INPUT')[6].value='';
                nRow.getElementsByTagName('INPUT')[7].value='';
                nRow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].value='...';
                nRow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value='';
                nRow.getElementsByTagName('INPUT')[CHKOVERTIMEINDEX].checked = false;
                //nRow.getElementsByTagName('INPUT')[12].value='';
                //nRow.cells[12].innerText='';
                nRow.getElementsByTagName('INPUT')[HSUBINDEX].value = '1';

                var billableYes = table.rows[i].getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked;
                var billableNo = table.rows[i].getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked;


                nRow.getElementsByTagName('INPUT')[HCATINDEX].value = '';
                nRow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked = false;
                nRow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = false;
                
                nRow.getElementsByTagName('INPUT')[1].className = '';
                nRow.getElementsByTagName('INPUT')[2].className = '';
                nRow.getElementsByTagName('INPUT')[3].className = '';
                nRow.getElementsByTagName('INPUT')[4].className = '';
                nRow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].className = '';
                nRow.getElementsByTagName('INPUT')[6].className = '';
                nRow.getElementsByTagName('INPUT')[7].className = '';
                nRow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].className = '';
                nRow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].className = '';

                // change the name of the new radio button
                setNameNewRadio(nRow);
                nRow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked = false;
                nRow.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = false;

                nRow = table.rows[i];
                nRow.getElementsByTagName('INPUT')[2].value = '';

                //splitPos = i;
                //currPos = i+1;
                position = i;
                break;
            }
        } 
   
            for(var i=1; i<rowCount; i++) 
            {  
                var oRow = table.rows[i];  
                var oChkbox = oRow.cells[0].childNodes[0];
                oChkbox.checked = false;
            }
    }
    
    
}

function checkOverlap(i, evt)
{
    var table=document.getElementById('tst');
    var tbody=table.getElementsByTagName('TBODY')[0];
    var oRow = table.rows[position];
    var xRow= null; 
    var flag = true; 
    if(position > 1 && i == 's')
    {
         xRow = table.rows[position -1];
         var val1 = xRow.cells[2].childNodes[0].value;
         var val2 = oRow.cells[2].childNodes[0].value;
         var p = oRow.cells[1].childNodes[0].value;
         var hold = p +''+ String.fromCharCode(evt.keyCode);
         //alert(hold);
         if(val != "" && val.length == '5')
         {
            //alert(parseInt(val.substring(p, p+1),10));
            if(parseInt(hold,10) > parseInt(val1.substring(0, p.length+1).replace(/:/,''),10) &&  parseInt(hold,10) < parseInt(val2.substring(0, p.length+1).replace(/:/,''),10) )
                return false;
         }
         
         val = oRow.cells[2].childNodes[0].value;
         if(val != "")
         {
            if(parseInt(hold,10) > parseInt(val.substring(0, p.length+1).replace(/:/,''),10))
                return false;
         }
         
    }
   return true; 
   
}

    function Addk(id)
    {
        var table=document.getElementById(id);
        var tbody=table.getElementsByTagName('TBODY')[0];
        
        var oRow = table.rows[(table.rows.length)-1];  
        
        if(checkOTFRAC())
        {
            var oflag = checkEntry('tst');
            //if(!(oRow.cells[5].childNodes[0].value == '') && !(oRow.cells[1].childNodes[0].value == ''))
            if (oflag && (document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "1"
                || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "9"
                || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == "0" 
                || document.getElementById('ctl00_ContentPlaceHolder1_hiddenDFlag').value == ""))
            {
                var oChkbox = oRow.cells[2].childNodes[0].value;
                
                var newrow=tbody.appendChild(table.rows[1].cloneNode(true));
                newrow.getElementsByTagName('INPUT')[0].checked=false;
                newrow.getElementsByTagName('INPUT')[1].value=oChkbox;
                newrow.getElementsByTagName('INPUT')[2].value='';
                newrow.getElementsByTagName('INPUT')[3].value='';
                newrow.getElementsByTagName('INPUT')[4].value='...';
                
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value='';
                newrow.getElementsByTagName('INPUT')[6].value='';
                newrow.getElementsByTagName('INPUT')[7].value='';
                //newrow.getElementsByTagName('INPUT')[8].checked=false;
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].value='...';
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value='';
                newrow.getElementsByTagName('INPUT')[HCFLAGINDEX].value='1';
                newrow.getElementsByTagName('INPUT')[HBILLABLEINDEX].value='';  //~

                newrow.getElementsByTagName('INPUT')[1].className = '';
                newrow.getElementsByTagName('INPUT')[2].className = '';
                newrow.getElementsByTagName('INPUT')[3].className = '';
                newrow.getElementsByTagName('INPUT')[4].className = '';
                newrow.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].className = '';
                newrow.getElementsByTagName('INPUT')[6].className = '';
                newrow.getElementsByTagName('INPUT')[7].className = '';
                
                newrow.getElementsByTagName('INPUT')[BTNSUBWORKINDEX].className = '';
                newrow.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].className = '';

                newrow.getElementsByTagName('INPUT')[HCATINDEX].value = '';

                setNameNewRadio(newrow);
                
                position = oRow.rowIndex+1;
                //currPos = oRow.rowIndex+1;
                //splitPos = 0;
            }
        }
        else
        {
            alert('Invalid time entry in form. System only allows minutes for every ' + document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRAC').value);
        }
    }

    function deleteRowk(tableID) 
    {  
        var table = document.getElementById(tableID);  
        var rowCount = table.rows.length;  
        
        var countChecked = 1;
        
        for(var i=1; i<rowCount; i++) 
            {  
                var oRow = table.rows[i];  
                var oChkbox = oRow.cells[0].childNodes[0];  
                if(null != oChkbox && true == oChkbox.checked)
                {  
                    countChecked++;  
                }  
            }
        if(rowCount == countChecked)
        {
            window.location.reload();
        }
        
        if(rowCount>2)
        {
            for(var i=1; i<rowCount; i++) 
            {  
                var row = table.rows[i];  
                var chkbox = row.cells[0].childNodes[0];  
                if(null != chkbox && true == chkbox.checked && row.getElementsByTagName('INPUT')[HSUBINDEX].value != '2')
                {  
                    
                    table.deleteRow(i);
                    position = table.rows.length -1;
                    //currPos = table.length -1;
                    //splitPos = 0;
                    rowCount--;  
                    i--;  
                }  
            }
        }
    } 
   
   function checkEntry(tableID)
   {
        var table = document.getElementById(tableID);  
        var rowCount = table.rows.length;  
        
        var countChecked = 0;
        
            for(var i=1; i<rowCount; i++) 
            {  
                var oRow = table.rows[i];  
                var oStart = oRow.cells[1].childNodes[0];
                var oEnd  = oRow.cells[2].childNodes[0];
                var oHour = oRow.cells[3].childNodes[0];
                var oJob = oRow.cells[5].childNodes[0];
                var oSub = oRow.cells[9].childNodes[0];
                var oBillable = oRow.cells[10].childNodes[0];
                
                if(null == oStart || "" == oStart.value || oStart.value.length != 5)
                {  
                    countChecked++;  
                }  
                else if(null == oEnd || "" == oEnd.value  || oEnd.value.length != 5)
                {
                    countChecked++;
                } 
                else if(null == oHour || "" == oHour.value)
                {
                    countChecked++;
                }
                else if(null == oJob || "" == oJob.value)
                {
                    countChecked++;
                } 
                else if(null == oSub || "" == oSub.value)
                {
                    countChecked++;
                }
                else if (null == oBillable || (!oBillable.rows[0].cells[0].childNodes[0].checked && !oBillable.rows[0].cells[1].childNodes[0].checked)) 
                {
                    countChecked++;
                }

                if(parseInt(countChecked,10) > 0)
                    break;

            }
                        
       return !(countChecked > 0);
        
   } 
   
   function checkOTFRAC()
   {
        var table = document.getElementById('tst'); 
        var rowCount = table.rows.length;  
        var OTFRAC = document.getElementById('ctl00_ContentPlaceHolder1_hfOTFRAC').value;
        var countChecked = 0;
        var FLAG = true;
        for(var i=1; i<rowCount; i++) 
        {  
            var oRow = table.rows[i];  
            var oStart = oRow.cells[1].childNodes[0].value;
            var oEnd  = oRow.cells[2].childNodes[0].value;
            var tempHold = '0';
            var tempXY = '0';
            ////This if for round down function for the start time if the minutes is not divisible by OTFRAC 
            if(oStart.length == '5')
            {
                tempHold = oStart.substring(3,5);
                if(parseInt(tempHold,10) % parseInt(OTFRAC,10) != 0)
                {
                    FLAG = false;
                    //tempXY = parseInt(tempHold,10) % parseInt(OTFRAC, 10);
                    //tempHold = parseInt(tempHold,10) - parseInt(tempXY,10);
                    //if(tempHold < 10)
                    //{
                    //    tempHold = '0' + tempHold;
                    //}
                    //oRow.getElementsByTagName('INPUT')[1].value = oStart.substring(0,2) + ':' + tempHold;
                }
            } 
            ////This if for round down function for the end time if the minutes is not divisible by OTFRAC 
            if(oEnd.length == '5')
            {
                tempHold = oEnd.substring(3,5);
                var checking = parseInt(tempHold,10) / parseInt(OTFRAC,10);
                if(parseInt(tempHold,10) % parseInt(OTFRAC,10) != 0)
                {
                    FLAG = false;
                    //tempXY = parseInt(tempHold,10) % parseInt(OTFRAC, 10);
                    //tempHold = parseInt(tempHold,10) - parseInt(tempXY,10);
                    //if(tempHold < 10)
                    //{
                    //    tempHold = '0' + tempHold;
                    //}
                    //oRow.getElementsByTagName('INPUT')[2].value = oEnd.substring(0,2) + ':' + tempHold;
                }
            }
              
            //////  END  //////////// 
        } 
        return FLAG;
         
   }
   
   function setPosition(evt)
   {
        var elem = evt.srcElement;
        var row = elem.parentNode.parentNode;
        sORe = elem.name;
        if(sORe == 'ctl00$ContentPlaceHolder1$txtJStart')
            sORe = 's';
        else if(sORe == 'ctl00$ContentPlaceHolder1$txtJEnd')
            sORe = 'e';
        else
            sORe = null;
        position = row.rowIndex;

        var table = document.getElementById('tst');
        for (var i = 1; i < table.rows.length; i++) 
        {
            row = table.rows[i];
            if (i == position) 
                row.cells[12].innerText = '◄';
            else
                row.cells[12].innerText = '';
        }
   }
   
   function setValueCheck(evt,obj)
   {
        var elem = evt.srcElement;
        var row = elem.parentNode.parentNode;

        if (elem.id == 'ctl00_ContentPlaceHolder1_chkBillable')
            row.cells[17].childNodes[0].value = obj.checked.toString();
        else if (elem.id == 'ctl00_ContentPlaceHolder1_chkOvertime')
            row.cells[18].childNodes[0].value = obj.checked.toString();
        

        //alert(row.cells[17].childNodes[0].nodeName  + " " + row.cells[17].childNodes[0].value);
        //alert(document.getElementById('ctl00_ContentPlaceHolder1_hBillable').value);
    }

    function setValueRadio(evt, obj) 
    {
        var elem = evt.srcElement;
        var row = elem.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        var row2 = elem.parentNode.parentNode;  // row for radio buttons w/ Yes or No as choices

        var name = elem.name;
        var suffix = elem.id.substring(elem.id.length - 1, elem.id.length);

        if (elem.id == 'ctl00_ContentPlaceHolder1_rblBillable_0') { // Yes
            row.cells[17].childNodes[0].value = "True";
            row.cells[13].childNodes[0].value = 'B';
//            row2.cells[1].childNodes[0].checked = false;
        }
        else if (elem.id == 'ctl00_ContentPlaceHolder1_rblBillable_1') {    // No
            row.cells[17].childNodes[0].value = "False";
            row.cells[13].childNodes[0].value = 'N';
//            row2.cells[0].childNodes[0].checked = false;
        }
    }

    function setValueDropdown(evt, obj) {
        var elem = evt.srcElement;
        var row = elem.parentNode.parentNode;

        if (elem.id == 'ctl00_ContentPlaceHolder1_ddlJobType')
            row.cells[18].childNodes[0].value = obj.value.toString();
    }

//    function setNameNewRadio(row) {  
//        var rbNameSuffix = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name.substring(37, row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name.length)
//        var numSuffix;
//        if (rbNameSuffix != '') {
//            numSuffix = parseInt(rbNameSuffix) + 1;
//        }
//        else {
//            numSuffix = 1;
//        }

//        var newName = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;

//        var elements;
//        var flag = true;
//        while (flag) {
//            elements = document.getElementsByName(newName);
//            if (elements.length > 0) {  // element name exist if length is > 0
//                numSuffix++;
//                newName = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;
//            }
//            else {
//                flag = false;
//            }
//        }
//        row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;
//        row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].name = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].name + numSuffix;
//    }

    function setNameNewRadio(row) {

        var rbNameSuffix = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name.substring(37, row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name.length)
        var numSuffix;
        if (rbNameSuffix != '') {
            numSuffix = parseInt(rbNameSuffix) + 1;
        }
        else {
            numSuffix = 1;
        }

        var tempName = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;

        var elements;
        var flag = true;
        while (flag) {
            elements = document.getElementsByName(tempName);
            if (elements.length > 0) {  // element name exist if length is > 0
                numSuffix++;
                tempName = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;
            }
            else {
                flag = false;
            }
        }

        newName = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].name + numSuffix;
        radio = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX];
        element = document.createElement('<input id="' + radio.id + '" type="radio" name="' + newName + '" value="' + radio.value + '">');
        parentNode = radio.parentNode;
        parentNode.insertBefore(element, radio);
        parentNode.removeChild(radio);

        radio = row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2];
        element = document.createElement('<input id="' + radio.id + '" type="radio" name="' + newName + '" value="' + radio.value + '">');
        parentNode = radio.parentNode;
        parentNode.insertBefore(element, radio);
        parentNode.removeChild(radio);
    }

    function lookupJSJobCode() 
    {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (320 / 2);
        popupWin = window.open("lookupJSJobCode.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=320,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupJSJobCode(jobCode, jobNo, jobName, category, costcenter)
    {
        var table = document.getElementById('tst');
        var row = table.rows[position];

        if (costcenter == 'ALL')
            costcenter = document.getElementById('ctl00_ContentPlaceHolder1_hfEmployeeCostCenter').value;
        
        row.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value = jobCode.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        row.getElementsByTagName('INPUT')[TXTJCLIENTNOINDEX].value = jobNo.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        row.getElementsByTagName('INPUT')[TXTJCLIENTNAMEINDEX].value = jobName.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        row.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value = '';
        row.getElementsByTagName('INPUT')[HCCTINDEX].value = costcenter.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        row.getElementsByTagName('INPUT')[HSUBINDEX].value = '';
        category = category.replace(/amp;/g, '').replace(/&nbsp;/g, '');
        row.getElementsByTagName('INPUT')[HCATINDEX].value = category;

        if (category == 'B') {
            row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked = true;
            row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = false;
        }
        else {
            row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX].checked = false;
            row.getElementsByTagName('INPUT')[RBLBILLABLEINDEX2].checked = true;
        }
        //        row.getElementsByTagName('INPUT')[HBILLABLEINDEX].value = ('B' == category).toString();

    }

    function lookupJSSubWork() {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (320 / 2);

        var table = document.getElementById('tst');
        var row = table.rows[position];

        var param1 = row.getElementsByTagName('INPUT')[HCATINDEX].value; //Category
        var param2 = row.getElementsByTagName('INPUT')[HCCTINDEX].value; //CostCenter
        var url = "pgeLookupSubWork.aspx?cct=" + "ALL" + "&cat=" + param1;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=750,height=320,top=" + top + ", left=" + left);

        return false;
    }

    function GetValueFromChildSubWork(val1, category) 
    {
        var table = document.getElementById('tst');
        var row = table.rows[position];

        row.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value = val1;
        row.getElementsByTagName('INPUT')[HSUBINDEX].value = val1;

        //row.getElementsByTagName('INPUT')[CHKBILLABLEINDEX].checked = ('B' == category);
//        row.getElementsByTagName('INPUT')[HCATINDEX].value = category;
//        row.getElementsByTagName('INPUT')[HBILLABLEINDEX].value = ('B' == category).toString();   // user has to choose wether billable or not

        //row.getElementsByTagName('INPUT')[13].value = '1';    
    }

    function lookupRJSCostcenter() {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepCostCenter.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupRJSCostcenter(val) 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
    }

    function lookupRJSCostcenterMR(isAllocatedVisible) {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepCostCenterMR.aspx?losstime=" + isAllocatedVisible, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupRJSCostcenterMR(val) {
        document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
    }

    function lookupRJSEmployee() {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepEmployee.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }
    
    function GetValueFrom_lookupRJSEmployee(val) {
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmpName').value = val;
    }

    function lookupRJSEmployeeMR(isAllocatedVisible) {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepEmployeeMR.aspx?losstime="+isAllocatedVisible, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupRJSEmployeeMR(val) {
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmpName').value = val;
    }

    function lookupRJSStatus() {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepStatus.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }
             
    function lookupRJSCheckerApprover(col, control) {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control, "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupRJSCheckerApprover(control, val) {
        document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
    }
    
    function OpenPopupLookupSales(val1, val2, val3) {
        var left = (screen.width / 2) - (690 / 2);
        var top = (screen.height / 2) - (390 / 2);
        var url = "pgeLookupSales.aspx?type=" + val1 + "&control=" + val2 + "&costCenter=" + val3;
        popupWin = window.open(url, "Sample", "scrollbars=no,resizable=no,width=690,height=390,top=" + top + ", left=" + left);
        return false;
    }

    function GetValueFromLookup(control, val) 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_' +control).value = val;
    }

    function lookupRJSSubwork() {
        var left = (screen.width / 2) - (750 / 2);
        var top = (screen.height / 2) - (410 / 2);
        popupWin = window.open("lookupJSRepSubwork.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
        return false;
    }

    function GetValueFrom_lookupRJSSubwork(val) 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').value = val;
    }

    function GetValueJS(val1, val2, val3, val4, val5, val6, val7) {
        var table = document.getElementById('tst');
        var row = table.rows[position];
        if (row.getElementsByTagName('INPUT')[HSUBINDEX].value != '2') {
            if (val1 != '&nbsp;')
                row.getElementsByTagName('INPUT')[TXTJJOBCODEINDEX].value = val1;
            if (val2 != '&nbsp;')
                row.getElementsByTagName('INPUT')[6].value = val2;
            if (val3 != '&nbsp;')
                row.getElementsByTagName('INPUT')[7].value = val3;
            if (val4 != '&nbsp;')
                row.getElementsByTagName('INPUT')[TXTSUBWORKINDEX].value = val4; //Subwork
            if (val5 != '&nbsp;') {
                row.getElementsByTagName('INPUT')[HCATINDEX].value = val5; //Category
                //row.getElementsByTagName('INPUT')[8].checked = ('B' == val5);
            }
            if (val6 != '&nbsp;')
                row.getElementsByTagName('INPUT')[HCCTINDEX].value = val6; //CostCenter
            if (val7 != '&nbsp;')
                row.getElementsByTagName('INPUT')[HSUBINDEX].value = val7; //Subwork if only one

            row.getElementsByTagName('INPUT')[HBILLABLEINDEX].value = ('B' == val5).toString(); //Billable
        }

    }

    function OpenPopupPrevJobSplit(val) {
        var left = (screen.width / 2) - (820 / 2);
        var top = (screen.height / 2) - (250 / 2);
        var url = "pgeLookupPreviousJobSplit.aspx?cn=" + val;
        popupWin = window.open(url, "Sample", "scrollbars=yes,resizable=no,width=820,height=250,top=" + top + ",left=" + left);
        return false;
    }

    function checkRange(type) {
        var alertFlag = false;

        //alert(document.getElementByID('ctl00_ContentPlaceHolder1_hWeekStart').value);
        var sOfWeek = document.getElementById('ctl00_ContentPlaceHolder1_hWeekStart').value;
        var eOfWeek = document.getElementById('ctl00_ContentPlaceHolder1_hWeekEnd').value;
        var repOptions = document.getElementById('ctl00_ContentPlaceHolder1_rblOption_2').checked;
        var dateType = document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').value; //D or W or M
        var day1 = new Date(document.getElementById('ctl00_ContentPlaceHolder1_dtpRangeFrom').value);
        var day2 = new Date(document.getElementById('ctl00_ContentPlaceHolder1_dtpRangeTo').value);

        var stringDay = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');

        //type is either 'from' or 'to'


        if (day1 == 'NaN' && day2 == 'NaN') {
            //No specific date selected
        }
        else {
            if (dateType == 'W') {
                if (day1.getDay() != sOfWeek || day2.getDay() != eOfWeek) {
                    //document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = true;
                    alertFlag = true;

                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                    alertFlag = false;
                }
                if (day1.getDay() == sOfWeek && day2 == 'NaN') {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                    alertFlag = false;
                }
                if (day2.getDay() == eOfWeek && day1 == 'NaN') {
                    document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                    alertFlag = false;
                }
                if (alertFlag) {
                    if (type == 'from')
                        alert("Warning : Date range entry are incorrect based on the filter options. \n Company start of the week is " + stringDay[sOfWeek] + ". \n Press OK to continue.");
                    else if (type == 'to')
                        alert("Warning : Date range entry are incorrect based on the filter options. \n Company end of the week is " + stringDay[eOfWeek] + ". \n Press OK to continue.");
                    else
                        alert("Warning : Date range entry are incorrect based on the filter options. \n Company start of the week is " + stringDay[sOfWeek] + ". \n Company end of the week is " + stringDay[eOfWeek] + ". \n Press OK to continue.");
                }
            }
            if (dateType == 'M') {
                if (document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').value == '01' || document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').value.replace(/ /, '') == '') {

                    if (day1.getDate() != 1 || day2.getDate() != daysInMonth(day2.getMonth(), day2.getFullYear())) {
                        //document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = true;
                        alertFlag = true;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }

                    if (day1.getDate() == 1 && day2 == 'NaN') {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }
                    if (day2.getDate() == daysInMonth(day2.getMonth(), day2.getFullYear()) && day1 == 'NaN') {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }
                    if (alertFlag)
                        alert("Warning : Date range entry are incorrect based on the filter options. \n Please be guided with the start date of the selected billing cycle. \n Press OK to continue.");
                }
                if (document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').value == '16') {
                    if (day1.getDate() != 16 || day2.getDate() != 15) {
                        //document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = true;
                        alertFlag = true;
                    }
                    else {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }

                    if (day1.getDate() == 16 && day2 == 'NaN') {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }
                    if (day2.getDate() == 15 && day1 == 'NaN') {
                        document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
                        alertFlag = false;
                    }
                    if (alertFlag)
                        alert("Warning : Date range entry are incorrect based on the filter options. \n Please be guided with the start date of the selected billing cycle. \n Press OK to continue.");
                }
                else {

                }

            }
        }
        if (day1 != 'NaN' && day2 != 'NaN') {
            var r1 = new Date(day1);
            var r2 = new Date(day2);
            if (r1 > r2) {
                document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = true;
                alert("Date range filter is incorrect. Format is from - to, check entries.");
            }
            else
                document.getElementById('ctl00_ContentPlaceHolder1_btnGenerate').disabled = false;
        }
        return true;
    }

    function GetValueFrom_lookupRJSStatus(val) 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
    }

    function hideOptions() 
    {
        var report = document.getElementById('ctl00_ContentPlaceHolder1_ddlReport').value;

        // None, MM/MS, FBS radio button 
        if(report == '1' || report == '2')
        {
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_0').disabled = false;
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_1').disabled = false;
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_2').disabled = false;
        }
        else {
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_0').checked = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_0').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_1').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_rblInclude_2').disabled = true;
        }

        // Include Unallocated checkbox
        if (document.getElementById('ctl00_ContentPlaceHolder1_cbUnsplitted') != null) {
            if (report == '1' || report == '2' || report == '3' || report == '4' || report == '5' || report == '6' || report == '7'
            || report == '14' || report == '15') {
                document.getElementById('ctl00_ContentPlaceHolder1_cbUnsplitted').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_cbUnsplitted').checked = false;
            }
            else {
                document.getElementById('ctl00_ContentPlaceHolder1_cbUnsplitted').disabled = false;
            }
        }

    }

    function gridSelectAllocationEntry(val) {
        var tbl = document.getElementById('ctl00_ContentPlaceHolder1_grdView');
        for (var i = 1; i < tbl.rows.length; i++) {
            tbl.rows[i].style.textDecoration = 'none';
            tbl.rows[i].style.color = 'black';
        }

        tbl.rows[parseInt(val, 10) + 1].style.textDecoration = 'underline';
        tbl.rows[parseInt(val, 10) + 1].style.color = 'blue';
    }

    function disableEnter() {
        if (window.event.keyCode == 13) {
            event.returnValue = false;
            event.cancel = true;
        }
    }

