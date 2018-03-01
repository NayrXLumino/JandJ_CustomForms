// JScript File

function callOnce()
{
    if(!isPostBack)
    {
        validateEntry();
        isPostBack = true;
    }
}

function timeEntry(evt)
{
  var charCode = (evt.which) ? evt.which : event.keyCode
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    if(charCode != 58) 
       return false;
  return true;
}

function autoAddColon(id)
{
    if(document.getElementById('ctl00_ContentPlaceHolder1_'+id).value.length == 2)
        document.getElementById('ctl00_ContentPlaceHolder1_'+id).value = document.getElementById('ctl00_ContentPlaceHolder1_'+id).value + ':';
        
    validateEntry();
}

function validateEntry()
{
    var flag = true; 
    var in1 = document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').value;
    var out1= document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').value;
    var in2= document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').value;
    var out2= document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').value;
    
    if(in1 == "")
    {
        if(out1 != "")
        {
            flag = false;
            document.getElementById('reqIn1').innerHTML = '*';
        }
        else
        {
            if(in2 == "" && out2 != "")
           {
                flag = false;
                document.getElementById('reqIn1').innerHTML = '*';
           }
           else
           {
                if(in2 == "" && out1 == "" && out2 == "")
                {
                    flag = false;
                    document.getElementById('reqIn1').innerHTML = '*';
                }
                else
                {
                    document.getElementById('reqIn1').innerHTML = ' '; 
                }
           }
        } 
    }
    else if(in1.length != '5')
    { 
        document.getElementById('reqIn1').innerHTML = '*'; 
        flag = false;
    }
    else
    {
        document.getElementById('reqIn1').innerHTML = ' '; 
    } 
    
    
    if(out1 == "")
    {
        if(in1 != "" && in2 != "" && out2 !="")
        {
             flag = false;
            document.getElementById('reqOut1').innerHTML = '*';
        }
        else
        {
            if(in1 != "" && out2 == "")
            {
                flag = false;
                document.getElementById('reqOut1').innerHTML = '*';
            }
            else
            {
                if(in1 == "" && in2== "" && out2 == "")
                {
                   flag = false;
                   document.getElementById('reqOut1').innerHTML = '*';
                }
                else
                {
                    document.getElementById('reqOut1').innerHTML = ' '; 
                }
            }
        }
    }
    else if(out1.length != '5')
    { 
        document.getElementById('reqOut1').innerHTML = '*'; 
        flag = false;
    }
    else
    {
        document.getElementById('reqOut1').innerHTML = ' ';
    }
    
    
    if(in2 == "")
    {
        if(in1 != "" && out1 != "" && out2 != "")
        {
            flag = false;
            document.getElementById('reqIn2').innerHTML = '*';
        }
        else
        {
            if(out2 != "" && in1 == "")
            {
                flag = false;
                document.getElementById('reqIn2').innerHTML = '*';
            }
            else
            {
                if(in1 == "" && out1== "" && out2 == "")
                {
                   flag = false;
                   document.getElementById('reqIn2').innerHTML = '*';
                }
                else
                {
                    document.getElementById('reqIn2').innerHTML = ' '; 
                }
            } 
        }
    }
    else if(in2.length != '5')
    { 
        document.getElementById('reqIn2').innerHTML = '*'; 
        flag = false;
    }
    else
    {
      document.getElementById('reqIn2').innerHTML = ' ';
    }
    
    if(out2 == "")
    {
        if(in1 !="" && out1 == "")
        {
            flag = false;
            document.getElementById('reqOut2').innerHTML = '*';
        }
        else
        {
            if(in2 != "")
            {
                flag = false;
                document.getElementById('reqOut2').innerHTML = '*';
            }
            else
            {
                if(in1 == "" && out1 == "" && in2 == "")
                {
                     flag = false;
                     document.getElementById('reqOut2').innerHTML = '*';
                }
                else
                {
                    document.getElementById('reqOut2').innerHTML = ' ';
                }
            }   
        } 
    }
    else if(out2.length != '5')
    { 
        document.getElementById('reqOut2').innerHTML = '*'; 
        flag = false;
    }
    else
    {
        document.getElementById('reqOut2').innerHTML = ' ';  
    }
    
    var stat = document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value;
    var bit = document.getElementById('ctl00_ContentPlaceHolder1_hfLogControl').value;
    //if(stat != "")
    if(true)
    {
        if(bit.substring(0,1) == '1')
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').className = "txtReadOnly";
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').readOnly = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').className = "textboxNumber";
        }
        if(bit.substring(1,2) == '1')
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').className = "txtReadOnly";
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').readOnly = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').className = "textboxNumber";
        }
        if(bit.substring(2,3) == '1')
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').className = "txtReadOnly";
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').readOnly = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').className = "textboxNumber";
        }
        if(bit.substring(3,4) == '1')
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').className = "txtReadOnly";
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').readOnly = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').className = "textboxNumber";;
        }
    }

    if (stat != '') 
    {
        if (stat != 'NEW') 
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').className = "txtReadOnly";
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').className = "txtReadOnly";
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').className = "txtReadOnly";
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').readOnly = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').className = "txtReadOnly";
        }
    }
    
    var button = document.getElementById('ctl00_ContentPlaceHolder1_btnZ');
    button.disabled = !flag;
}

function lookupTKAdjustmentDate()
{
    var left=(screen.width/2)-(860/2);
    var top=(screen.height/2)-(410/2);
    var EI = document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value;
	popupWin = window.open("lookupTKAdjustmentDate.aspx?ei="+EI,"Sample","scrollbars=no,resizable=no,width=860,height=300,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupTKAdjustmentDate(date, shift, in1, out1, in2, out2, sIn1, sOut1, sIn2, sOut2, sType)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtAdjustmentDate').value = date;
    document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value = shift;
    document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').value = (in1 == '&nbsp;') ? '' : in1;
    document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').value = (out1 == '&nbsp;') ? '' :  out1;
    document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').value = (in2 == '&nbsp;') ? '' :  in2;
    document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').value = (out2 == '&nbsp;') ? '' :  out2;
    document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value = sIn1;
    document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value = sOut1;
    document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value = sIn2;
    document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value = sOut2;
    document.getElementById('ctl00_ContentPlaceHolder1_hfShiftType').value = sType;
    //Reset Transaction State
    document.getElementById('ctl00_ContentPlaceHolder1_hfSaved').value = '0';
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = '';
    document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';

    if (sType == 'G') 
    {
        if (sOut1 < sIn1) 
        {
            sOut1 = '' + (parseInt(sOut1.substring(0, 2), 10) + 24) + sOut1.substring(3, 5);
        }
        if (sIn2 < sIn1) 
        {
            sIn2 = '' + (parseInt(sIn2.substring(0, 2), 10) + 24) + sIn2.substring(3, 5);
        }
        if (sOut2 < sIn1) 
        {
            sOut2 = '' + (parseInt(sOut2.substring(0, 2), 10) + 24) + sOut2.substring(3, 5);
        }
    }

    //Logic for LogControl and Modification Type
    //LogControl

    var gap = document.getElementById('ctl00_ContentPlaceHolder1_hfTimeModGap').value;
    var bitRep = '';
    if(in1 == '&nbsp;')
    {
        bitRep += '0';
    }
    else 
    {
        if ( (in1 != '&nbsp;' && out1 != '&nbsp;') 
          && (Math.abs( ((parseInt( in1.substring(0,2), 10) * 60) + parseInt( in1.substring(3,5), 10)) 
                      - ((parseInt(out1.substring(0,2), 10) * 60) + parseInt(out1.substring(3,5), 10))
                      ) <= parseInt(gap, 10)))
        { 
            if( ((parseInt( in1.substring(0,2), 10) * 60) + parseInt( in1.substring(3,5), 10)) 
              > ((parseInt(sOut1.substring(0,2), 10) * 60) + parseInt(sOut1.substring(3,5), 10)) 
              )
              {
                  bitRep += '0';
              }
              else
              {
                bitRep += '1';
              }
        }else if( (in1 != '&nbsp;' && out2 != '&nbsp;') 
               && (Math.abs( ((parseInt( in1.substring(0,2), 10) * 60) + parseInt( in1.substring(3,5), 10)) 
                           - ((parseInt(out2.substring(0,2), 10) * 60) + parseInt(out2.substring(3,5), 10))
                           ) <= parseInt(gap, 10)))
        { 
            if( ((parseInt( in1.substring(0,2), 10) * 60) + parseInt( in1.substring(3,5), 10)) 
                > ((parseInt(sOut1.substring(0,2), 10) * 60) + parseInt(sOut1.substring(3,5), 10)) 
                )
                {
                    bitRep += '0';
                }
                else
                {
                    bitRep += '1';
                }
        }
        else
        {
            bitRep += '1';
        }

        //validate correct placement
        if (((parseInt(in1.substring(0, 2), 10) * 60) + parseInt(in1.substring(3, 5), 10))
             >= ((parseInt(sOut1.substring(0, 2), 10) * 60) + parseInt(sOut1.substring(3, 5), 10))) 
        {
            bitRep = bitRep.substring(0, 0) + '0';
        }
        //end validate correct placement
    }
    if(out1 == '&nbsp;')
    {
        bitRep += '0';
    }
    else 
    {
        if ((in1 != '&nbsp;' && out1 != '&nbsp;')
          && (Math.abs(((parseInt(in1.substring(0, 2), 10) * 60) + parseInt(in1.substring(3, 5), 10))
                      - ((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                      ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(in1.substring(0, 2), 10) * 60) + parseInt(in1.substring(3, 5), 10))
              > ((parseInt(sOut1.substring(0, 2), 10) * 60) + parseInt(sOut1.substring(3, 5), 10))
              ) 
            {
                bitRep += '1';
            }
            else {
                bitRep += '0';
            }
        } else if ((out1 != '&nbsp;' && out2 != '&nbsp;')
               && (Math.abs(((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                           - ((parseInt(out2.substring(0, 2), 10) * 60) + parseInt(out2.substring(3, 5), 10))
                           ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                > ((parseInt(sIn2.substring(0, 2), 10) * 60) + parseInt(sIn2.substring(3, 5), 10))
                ) 
            {
                bitRep += '0';
            }
            else 
            {
                bitRep += '1';
            }
        } else if ((out1 != '&nbsp;' && in2 != '&nbsp;')
               && (Math.abs(((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                           - ((parseInt(in2.substring(0, 2), 10) * 60) + parseInt(in2.substring(3, 5), 10))
                           ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                > ((parseInt(sIn2.substring(0, 2), 10) * 60) + parseInt(sIn2.substring(3, 5), 10))
                ) 
            {
                bitRep += '0';
            }
            else 
            {
                bitRep += '1';
            }
        }
        else 
        {
            bitRep += '1';
        }

        //validate correct placement
        if (    ((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
             >= ((parseInt(sIn2.substring(0, 2), 10) * 60) + parseInt(sIn2.substring(3, 5), 10))) 
        {
            bitRep = bitRep.substring(0, 1) + '0';
        }
        //end validate correct placement
    }
    if(in2 == '&nbsp;')
    {
        bitRep += '0';
    }
    else 
    {
        if ((out1 != '&nbsp;' && in2 != '&nbsp;')
               && (Math.abs(((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                           - ((parseInt(in2.substring(0, 2), 10) * 60) + parseInt(in2.substring(3, 5), 10))
                           ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                > ((parseInt(sIn2.substring(0, 2), 10) * 60) + parseInt(sIn2.substring(3, 5), 10))
                ) 
            {
                bitRep += '1';
            }
            else 
            {
                bitRep += '0';
            }
        }
        else 
        {
            bitRep += '1';
        }

        //validate correct placement
        if (((parseInt(in2.substring(0, 2), 10) * 60) + parseInt(in2.substring(3, 5), 10))
             < ((parseInt(sOut1.substring(0, 2), 10) * 60) + parseInt(sOut1.substring(3, 5), 10))) {
            bitRep = bitRep.substring(0, 2) + '0';
        }
        //end validate correct placement

    }
    if(out2 == '&nbsp;')
    {
        bitRep += '0';
    }
    else
    {
        if ((in1 != '&nbsp;' && out1 != '&nbsp;')
          && (Math.abs(((parseInt(in1.substring(0, 2), 10) * 60) + parseInt(in1.substring(3, 5), 10))
                      - ((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                      ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(in1.substring(0, 2), 10) * 60) + parseInt(in1.substring(3, 5), 10))
              > ((parseInt(sOut1.substring(0, 2), 10) * 60) + parseInt(sOut1.substring(3, 5), 10))
              ) 
            {
                bitRep += '1';
            }
            else 
            {
                bitRep += '0';
            }
        } else if ((out1 != '&nbsp;' && out2 != '&nbsp;')
               && (Math.abs(((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                           - ((parseInt(out2.substring(0, 2), 10) * 60) + parseInt(out2.substring(3, 5), 10))
                           ) <= parseInt(gap, 10))) 
        {
            if (((parseInt(out1.substring(0, 2), 10) * 60) + parseInt(out1.substring(3, 5), 10))
                > ((parseInt(sOut1.substring(0, 2), 10) * 60) + parseInt(sOut1.substring(3, 5), 10))
                ) 
            {
                bitRep += '1';
            }
            else 
            {
                bitRep += '0';
            }
        }
        else 
        {
            bitRep += '1';
        }

        //validate correct placement
        if (((parseInt(out2.substring(0, 2), 10) * 60) + parseInt(out2.substring(3, 5), 10))
             < ((parseInt(sIn2.substring(0, 2), 10) * 60) + parseInt(sIn2.substring(3, 5), 10))) 
        {
            bitRep = bitRep.substring(0, 3) + '0';
        }
        //end validate correct placement
    }

    document.getElementById('ctl00_ContentPlaceHolder1_hfLogControl').value = bitRep;
    
    
    //Modification Type
    //No Time In
    var type = '';
    var typeCode = '';
    if ( (in1+out1+in2 == '&nbsp;&nbsp;&nbsp;' && out2 != '&nbsp;')
      || (in1+in2+out2 == '&nbsp;&nbsp;&nbsp;' && out1 != '&nbsp;')
      || (in1 == '&nbsp;' && out1 != '&nbsp;' && in2 != '&nbsp;' && out2 != '&nbsp;')
      || (in2 == '&nbsp;' && in1 != '&nbsp;' && out1 != '&nbsp;' && out2 != '&nbsp;') )
    {
        type = 'NO IN';
        typeCode = 'IN';
    }
    
    //No Time Out
    if ( (out1+in2+out2 == '&nbsp;&nbsp;&nbsp;' && in1 != '&nbsp;')
      || (in1+out1+out2 == '&nbsp;&nbsp;&nbsp;' && in2 != '&nbsp;')
      || (in1 != '&nbsp;' && out1 != '&nbsp;' && in2 != '&nbsp;' && out2 != '&nbsp;')
      || (in1 != '&nbsp;' && out1 != '&nbsp;' && in2 != '&nbsp;' && out2 == '&nbsp;') )
    {
        type = 'NO OUT';
        typeCode = 'OU';
    }
    
    //No In_1 and In 2
    if ( (in1 + in2 == '&nbsp;&nbsp;' && out1 != '&nbsp;' && out2 != '&nbsp;') )
    {
        type = 'NO IN1 AND NO IN2';
        typeCode = 'I2';
    }
    
    //No Out 1 and Out 2
    if ( (out1 + out2 == '&nbsp;&nbsp;' && in1 != '&nbsp;' && in2 != '&nbsp;') )
    {
        type = 'NO OUT1 AND NO OUT2';
        typeCode = 'O2';
    }
    
    //No In and Out
    if ( (in1 + in2 + out1 + out2 == '&nbsp;&nbsp;&nbsp;&nbsp;') )
    {
        type = 'NO IN AND NO OUT';
        typeCode = 'IO';
    }

    if (typeCode == '') 
    {
        switch (bitRep) 
        {
            case '0000':
                type = 'NO IN AND NO OUT';
                typeCode = 'IO';
                break;
            case '0001':
                type = 'NO IN';
                typeCode = 'IN';
                break;
            case '0010':
                type = 'NO OUT';
                typeCode = 'OU';
                break;
            case '0011':
                type = '';
                typeCode = '';
                break;
            case '0100':
                type = 'NO IN';
                typeCode = 'IN';
                break;
            case '0101':
                type = 'NO IN1 AND NO IN2';
                typeCode = 'I2';
                break;
            case '0110':
                type = 'NO IN AND NO OUT';
                typeCode = 'IO';
                break;
            case '0111':
                type = 'NO IN';
                typeCode = 'IN';
                break;
            case '1000':
                type = 'NO OUT';
                typeCode = 'OU';
                break;
            case '1001':
                type = '';
                typeCode = '';
                break;
            case '1010':
                type = 'NO OUT1 AND NO OUT2';
                typeCode = 'O2';
                break;
            case '1011':
                type = 'NO OUT';
                typeCode = 'OU';
                break;
            case '1100':
                type = '';
                typeCode = '';
                break;
            case '1101':
                type = 'NO IN';
                typeCode = 'IN';
                break;
            case '1110':
                type = 'NO OUT';
                typeCode = 'OU';
                break;
            case '1111':
                type = '';
                typeCode = '';
                break;
            default:
                break;
        }
    }
    document.getElementById('ctl00_ContentPlaceHolder1_txtType').value = type;
    document.getElementById('ctl00_ContentPlaceHolder1_hfModType').value = typeCode;

    if (type == '') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtType').value = 'MODIFICATION';
        document.getElementById('ctl00_ContentPlaceHolder1_hfModType').value = 'MD';
    }

    if (document.getElementById('ctl00_ContentPlaceHolder1_hfBYPASSTIMEVERIFIACTION').value == 'TRUE') 
    {
        document.getElementById('ctl00_ContentPlaceHolder1_hfLogControl').value = '0000';
    }

    validateEntry();
}

function lookupTKProximityLogs()
{
    var left=(screen.width/2)-(300/2);
    var top=(screen.height/2)-(410/2);
    var EI = document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value;
    var DT = document.getElementById('ctl00_ContentPlaceHolder1_txtAdjustmentDate').value;
	popupWin = window.open("lookupTKProximityLogs.aspx?id="+EI+"&dt="+DT,"Sample","scrollbars=no,resizable=no,width=317,height=318,top=" +top+ ",left=" +left);
	return false;
}
///////////////////////////////////dfghjkl'
function lookupRTKEmployee()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepEmployee.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKEmployee(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtEmployee').value = val;
}

function lookupRTKStatus()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepStatus.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKStatus(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = val;
}

function lookupRTKCostcenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKCostcenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}

function lookupRTKCostcenterLine() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupTKRepCostCenterLine.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupRTKCostcenterLine(val) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenterLine').value = val;
}

function lookupRTKPayPeriod()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepPayPeriod.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKPayPeriod(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtPayPeriod').value = val;
}

function lookupRTKType()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepType.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKType(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtType').value = val;
}

function lookupRTKCheckerApprover(col, control)
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupTKRepCheckerApprover.aspx?col=" + col + "&ctrl=" + control,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupRTKCheckerApprover(control,val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_'+ control).value = val;
}

function lookupWRRepCostCenter()
{
    var left=(screen.width/2)-(750/2);
    var top=(screen.height/2)-(410/2);
	popupWin = window.open("lookupWRRepCostCenter.aspx" ,"Sample","scrollbars=no,resizable=no,width=750,height=410,top=" +top+ ",left=" +left);
	return false;
}

function GetValueFrom_lookupWRRepCostCenter(val)
{
    document.getElementById('ctl00_ContentPlaceHolder1_txtCostcenter').value = val;
}

function gridSelectTimeRecEntry(val) 
{
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    for (var i = 1; i < tbl.rows.length; i++) 
    {
        tbl.rows[i].style.textDecoration = 'none';
        tbl.rows[i].style.color = 'black';
    }

    tbl.rows[parseInt(val, 10) + 1].style.textDecoration = 'underline';
    tbl.rows[parseInt(val, 10) + 1].style.color = 'blue';
}

function hoursEntry(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        //if (charCode != 46)//20090104 andre uncommented for minutes entry
            return false;
        //else {
        //    if (document.getElementById('ctl00_ContentPlaceHolder1_txtUnpaidBreak').value == '')
        //        document.getElementById('ctl00_ContentPlaceHolder1_txtUnpaidBreak').value = '0';
        //    if (document.getElementById('ctl00_ContentPlaceHolder1_txtUnpaidBreak').value.indexOf('.') > -1)
        //        return false;
        //}
    return true;
}

function lookupSWShiftCode() {
    var left = (screen.width / 2) - (750 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin = window.open("lookupSWShiftCode.aspx", "Sample", "scrollbars=no,resizable=no,width=750,height=410,top=" + top + ",left=" + left);
    return false;
}

function GetValueFrom_lookupSWShiftCode(val1, val2) {
    document.getElementById('ctl00_ContentPlaceHolder1_txtShiftCode').value = val1;
    document.getElementById('ctl00_ContentPlaceHolder1_txtShiftDesc').value = val2;
}

function clearControls() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtAdjustmentDate').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtShift').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn1').value ='';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTimeIn2').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtTimeOut2').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfI1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfO1').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfI2').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfO2').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_hfShiftType').value = '';
        //Reset Transaction State
        document.getElementById('ctl00_ContentPlaceHolder1_hfSaved').value = '0';
        document.getElementById('ctl00_ContentPlaceHolder1_txtStatus').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';
    }
}

function gridViewTimeRecEntry(date, id) 
{
    var left = (screen.width / 2) - (880 / 2);
    var top = (screen.height / 2) - (400 / 2);
    popupWin = window.open("lookupTimeRecordDetails.aspx?id=" + id + "&dt=" + date, "Sample", "scrollbars=no,resizable=no,width=870,height=400,top=" + top + ",left=" + left);
    return false;
}

function clearControlsStraightWork() {
    if (document.getElementById('ctl00_ContentPlaceHolder1_btnY').value == 'CLEAR') {
        document.getElementById('ctl00_ContentPlaceHolder1_dtpToDate').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtShiftCode').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtShiftDesc').value = '';
        document.getElementById('ctl00_ContentPlaceHolder1_txtUnpaidBreak').value = '0';
        document.getElementById('ctl00_ContentPlaceHolder1_txtReason').value = '';

        try { document.getElementById('ctl00_ContentPlaceHolder1_txtFiller1').value = ''; }
        catch (err) { } //Control is not shown. Controlled by server side
        try { document.getElementById('ctl00_ContentPlaceHolder1_txtFiller2').value = ''; }
        catch (err) { } //Control is not shown. Controlled by server side
        try { document.getElementById('ctl00_ContentPlaceHolder1_txtFiller3').value = ''; }
        catch (err) { } //Control is not shown. Controlled by server side

        return false;
    }
    else {
        return true;
    }
}
function GetValueFrom_lookupROTCheckerApprover(control, val) {
    document.getElementById('ctl00_ContentPlaceHolder1_' + control).value = val;
}