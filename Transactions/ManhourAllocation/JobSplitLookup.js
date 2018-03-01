// JScript File
position = 1;
sORe = null;
//splitPos = 0;
//currPos = 1;

function Spooler()
{
    var look = document.getElementById('txtJStart').value;
    if(look.match(",") != null)
   {
        Manipulate();
   } 
   readonlyControls();
    
} 

function Spooler2()
{
    var look = document.getElementById('txtJStart').value;
    if(look.match(",") != null)
   {
        Manipulate();
   }
   readonlyControls();
} 

function Manipulate()
{  
  var start = document.getElementById('txtJStart').value;
  var end = document.getElementById('txtJEnd').value;
  var hours = document.getElementById('txtJHours').value;
  var jobcode = document.getElementById('txtJJobCode').value;
  var clientno = document.getElementById('txtJClientNo').value;
  var clientname = document.getElementById('txtJClientName').value;
  var subwork = document.getElementById('txtSubWork').value;
  var cct = document.getElementById('hCCT').value;
  var cat = document.getElementById('hCat').value;
  var hSub = document.getElementById('hSub').value;

  //Kelvin added: 20110415 - added billable and overtime
  var billable = document.getElementById('hBillable').value;
  var overtime = document.getElementById('hOvertime').value;
  
  start = start +",";
  end = end +",";
  hours = hours +",";
  jobcode = jobcode +",";
  clientno = clientno +",";
  clientname = clientname +",";
  subwork = subwork+",";
  cct = cct +",";
  cat = cat +",";
  hSub = hSub + ",";
  billable = billable + ",";
  overtime = overtime + ",";
  
    //clears the data
   document.getElementById('txtJStart').value = "";
   document.getElementById('txtJEnd').value = "";
   document.getElementById('txtJHours').value = "";
   document.getElementById('txtJJobCode').value = "";
   document.getElementById('txtJClientNo').value = "";
   document.getElementById('txtJClientName').value = ""; 
   document.getElementById('txtSubWork').value = ""; 
   document.getElementById('hCCT').value = ""; 
   document.getElementById('hCat').value = "";
   document.getElementById('hSub').value = "";
   document.getElementById('hBillable').value = "";
   document.getElementById('hOvertime').value = "";

   var table=document.getElementById('tst');
   var tbody=table.getElementsByTagName('TBODY')[0];
    
    while(start.length > 1)
    {
      table=document.getElementById('tst');
      tbody=table.getElementsByTagName('TBODY')[0];
      //create rows
      var oRow = table.rows[(table.rows.length)-1];  
            
            var newrow=tbody.appendChild(table.rows[1].cloneNode(true));
              
            newrow.getElementsByTagName('INPUT')[0].value=start.substring(0,start.indexOf(","));
            newrow.getElementsByTagName('INPUT')[1].value=end.substring(0,end.indexOf(","));
            newrow.getElementsByTagName('INPUT')[2].value=hours.substring(0,hours.indexOf(","));
            newrow.getElementsByTagName('INPUT')[3].value=jobcode.substring(0,jobcode.indexOf(","));
            newrow.getElementsByTagName('INPUT')[4].value=clientno.substring(0,clientno.indexOf(","));
            newrow.getElementsByTagName('INPUT')[5].value = clientname.substring(0, clientname.indexOf(","));
            newrow.getElementsByTagName('INPUT')[6].checked = billable.substring(0, billable.indexOf(",")).toLowerCase() == 'true';
            newrow.getElementsByTagName('INPUT')[7].value = subwork.substring(0, subwork.indexOf(","));
            newrow.getElementsByTagName('INPUT')[8].checked = overtime.substring(0, overtime.indexOf(",")).toLowerCase() == 'true';

      //trimming the string
      start = start.substring(start.indexOf(",")+1, start.length);
      end = end.substring(end.indexOf(",")+1, end.length);
      hours = hours.substring(hours.indexOf(",")+1, hours.length);
      jobcode = jobcode.substring(jobcode.indexOf(",")+1, jobcode.length);
      clientno = clientno.substring(clientno.indexOf(",")+1, clientno.length);
      clientname = clientname.substring(clientname.indexOf(",")+1, clientname.length);
      subwork = subwork.substring(subwork.indexOf(",")+1, subwork.length);      
      billable = billable.substring(billable.indexOf(",")+1, billable.length);
      overtime = overtime.substring(overtime.indexOf(",")+1, overtime.length);
   }
   table.deleteRow(1);
   position = table.rows.length - 1;
}

function readonlyControls()
{
    
    var table = document.getElementById('tst');
    var tbody=table.getElementsByTagName('TBODY')[0]; 
    var rowCount = table.rows.length;  
    var stat = document.getElementById('hiddenStatus').value;
    var b1 = document.getElementById('break_Start').value;
    var b2 = document.getElementById('break_End').value;
    var cmpFlag = document.getElementById('hiddenDFlag').value; 
    var data = 0;
  var accumulated = 0;
  
   for(var i=1; i<rowCount; i++) 
    {  
        var oRow = table.rows[i];  
            
            var newrow=table.rows[i];
            var _start = newrow.getElementsByTagName('INPUT')[1].value;
            var _end = newrow.getElementsByTagName('INPUT')[2].value;
            _start = _start.replace(/:/,'');
            _end = _end.replace(/:/,'');
            
            if(parseInt(_start,10) <= parseInt(b1,10) && parseInt(_end,10) >= parseInt(b2, 10))
            {
                data = (parseInt(_end,10) - parseInt(_start,10)) / 100;
                data -= 1;
            }
        
        newrow.getElementsByTagName('INPUT')[0].readOnly = true;
        newrow.getElementsByTagName('INPUT')[1].readOnly = true;
        newrow.getElementsByTagName('INPUT')[2].readOnly = true;
        newrow.getElementsByTagName('INPUT')[3].readOnly = true;
        newrow.getElementsByTagName('INPUT')[4].readOnly = true;
        newrow.getElementsByTagName('INPUT')[5].readOnly = true;
        newrow.getElementsByTagName('INPUT')[6].disabled = true;
        newrow.getElementsByTagName('INPUT')[7].readOnly = true;
        newrow.getElementsByTagName('INPUT')[8].disabled = true;
        
            var temp = 0;          

            if(newrow.getElementsByTagName('INPUT')[2].value != "")
            {
                var temp2 = newrow.getElementsByTagName('INPUT')[2].value;
                temp = parseInt(temp2, 10);
            }
            
            accumulated += temp;
    } 
  
      if(accumulated=="0")
      {
            document.getElementById('txtTotalHours').value = "" ;
      }
      else
      {
            document.getElementById('txtTotalHours').value = accumulated ;
      }
      
}

function autoEndtime()
{
    var table = document.getElementById('tst');  
    var tbody=table.getElementsByTagName('TBODY')[0]; 
    var oRow = table.rows[position];
   
    var start = oRow.getElementsByTagName('INPUT')[1].value; 
    var hour = start.substring(0,2);
    var min = start.substring(3,5);
   
    var noOfHours = oRow.getElementsByTagName('INPUT')[3].value; 
    //Start computation
   hour = parseInt(hour,10) + parseInt(noOfHours, 10);
   
   if(hour < 10)
        hour = '0' + hour;
   
   oRow.getElementsByTagName('INPUT')[2].value = hour + ':' + min;
     
}


function selection()
{
    alert("im in selection");
}

function formatCompute(evt)
{

                var table = document.getElementById('tst');  
                var tbody=table.getElementsByTagName('TBODY')[0]; 
                var oRow = table.rows[position];
                //validate entry
                var start = oRow.getElementsByTagName('INPUT')[1].value; 
                var end = oRow.getElementsByTagName('INPUT')[2].value;  
                
                 if(start.length == '2')
                 {
                        oRow.getElementsByTagName('INPUT')[1].value =  oRow.getElementsByTagName('INPUT')[1].value + ':00';
                 } 
                   
                 if(end.length == '2')
                 {
                        oRow.getElementsByTagName('INPUT')[2].value =  oRow.getElementsByTagName('INPUT')[2].value + ':00';
                 }
                 
                 if(start.length == '3' || end.length == '3')
                 {
                 
                 }
              
                if(start.length == '5' && end.length == '5')
                { 
                    var shour = start.substring(0,2);
                    var smin = start.substring(3,5);
                    
                    var ehour = end.substring(0,2);
                    var emin = end.substring(3,5);
                    
                    //Convert hours in minutes
                   shour = parseInt(shour,10) * 60;
                   
                   smin = parseInt(smin,10) + parseInt(shour,10);

                   
                   ehour = parseInt(ehour,10) * 60;
                   emin = parseInt(emin,10) + parseInt(ehour,10);
                   
                   var thours = emin - smin;
                  //alert(emin + '-' + smin); 
                  if(thours > 0 )
                  { 
                       thours = thours / 60;  
                       oRow.getElementsByTagName('INPUT')[3].value = thours;
                  }
                }
}

function isNumberJS(evt)
{
    flag = true;
    var charCode = (evt.which) ? evt.which : event.keyCode
          if (charCode > 31 && (charCode < 48 || charCode > 57))
               flag =  false;
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
            //alert(parseInt(val.substring(p, p+1),10));
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
        
    return flag;
}

function getRecent()
{
    var table1 = document.getElementById('tst');  
    var tbody1= table1.getElementsByTagName('TBODY')[0]; 
    var oRow1 = table1.rows[position];

    var list = document.getElementById('listBox');
    lvalue = list.value;
    //alert(lvalue);
    if(list.selectedIndex >= 0)
   {  
        list = list.options[list.selectedIndex].text + '~'; 
        
        var val1 = list.substring(0, list.indexOf('~'));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val2 = list.substring(0, list.indexOf('~'));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val3 = list.substring(0, list.indexOf('~'));
        list = list.substring(list.indexOf('~')+1, list.length);
        var val4 = list.substring(0, list.indexOf('~'));
        
        lvalue = lvalue + '~'; 
        
        var val5 = lvalue.substring(0, lvalue.indexOf('~'));
        lvalue = lvalue.substring(lvalue.indexOf('~')+1, lvalue.length);
        
        var val6 = lvalue.substring(0, lvalue.indexOf('~'));
        lvalue = lvalue.substring(lvalue.indexOf('~')+1, lvalue.length);
        
        var val7 = lvalue.substring(0, lvalue.indexOf('~'));
        
        
        GetValueJS(val1, val2, val3, val4, val5, val6, val7);
       
		    /*
		    row.getElementsByTagName('INPUT')[5].value=val1;
		    row.getElementsByTagName('INPUT')[6].value=val2;
		    row.getElementsByTagName('INPUT')[7].value=val3;
		    row.getElementsByTagName('INPUT')[9].value=val4;//Subwork
		    
		    row.getElementsByTagName('INPUT')[10].value=val5;//Category
		    row.getElementsByTagName('INPUT')[11].value=val6;//CostCenter
		    row.getElementsByTagName('INPUT')[12].value=val7;//Subwork if only one
		    */
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
            if(null != oChkbox && true == oChkbox.checked)
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
                table.rows[i+1].appendChild(table.rows[i].cells[9].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[10].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[11].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[12].cloneNode(true));
                table.rows[i+1].appendChild(table.rows[i].cells[13].cloneNode(true));
                
                var nRow = table.rows[i+1];
                nRow.getElementsByTagName('INPUT')[0].checked=false;
                nRow.getElementsByTagName('INPUT')[1].value='';
                //nRow.getElementsByTagName('INPUT')[2].value='';
                nRow.getElementsByTagName('INPUT')[3].value='';
                nRow.getElementsByTagName('INPUT')[4].value='...';
                nRow.getElementsByTagName('INPUT')[5].value='';
                nRow.getElementsByTagName('INPUT')[6].value='';
                nRow.getElementsByTagName('INPUT')[7].value='';
                nRow.getElementsByTagName('INPUT')[8].value='...';
                nRow.getElementsByTagName('INPUT')[9].value='';
                //nRow.getElementsByTagName('INPUT')[10].value='';
                //nRow.getElementsByTagName('INPUT')[11].value='';
                //nRow.getElementsByTagName('INPUT')[12].value='';
                
                //nRow.cells[12].innerText='';
                
                
                
                
                nRow = table.rows[i];
                nRow.getElementsByTagName('INPUT')[2].value='';
                
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


//Code moved from autotime.js to JobSplit.js
    function Addk(id)
    {
        var table=document.getElementById(id);
        var tbody=table.getElementsByTagName('TBODY')[0];
        
        var oRow = table.rows[(table.rows.length)-1];  
        
        var oflag = checkEntry('tst');
        
        
        //if(!(oRow.cells[5].childNodes[0].value == '') && !(oRow.cells[1].childNodes[0].value == ''))
        if(oflag)
        {
            var oChkbox = oRow.cells[2].childNodes[0].value;
            
            var newrow=tbody.appendChild(table.rows[1].cloneNode(true));
            newrow.getElementsByTagName('INPUT')[0].checked=false;
            newrow.getElementsByTagName('INPUT')[1].value=oChkbox;
            newrow.getElementsByTagName('INPUT')[2].value='';
            newrow.getElementsByTagName('INPUT')[3].value='';
            newrow.getElementsByTagName('INPUT')[4].value='...';
            
            newrow.getElementsByTagName('INPUT')[5].value='';
            newrow.getElementsByTagName('INPUT')[6].value='';
            newrow.getElementsByTagName('INPUT')[7].value='';
            newrow.getElementsByTagName('INPUT')[8].value='...';
            newrow.getElementsByTagName('INPUT')[9].value='';
                        
            
            
            position = oRow.rowIndex+1;
            //currPos = oRow.rowIndex+1;
            //splitPos = 0;
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
                if(null != chkbox && true == chkbox.checked)
                {  
                    
                    table.deleteRow(i);
                    position = table.length -1;
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
               
               
                if(null == oStart || "" == oStart.value)
                {  
                    countChecked++;  
                }  
                else if(null == oEnd || "" == oEnd.value)
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
               
               if(parseInt(countChecked,10) > 0)
                    break;  
                
            }
            //alert(countChecked);
            //alert(parseInt(countChecked, 10) > 0);
       return !(countChecked > 0);
        
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
   }
   
   
   