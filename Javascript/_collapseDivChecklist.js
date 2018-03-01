// JScript File
function showHideError() 
{
    if (document.getElementById('errorPanel').style.visibility != 'visible') 
    {
        document.getElementById('errorPanel').style.visibility = 'visible';
        document.getElementById('dgvError').style.visibility = 'visible'; 
        document.getElementById('lblError').style.zIndex = '1';
    }
    else 
    {
        document.getElementById('errorPanel').style.visibility = 'hidden';
        document.getElementById('dgvError').style.visibility = 'hidden'; 
        document.getElementById('lblError').style.zIndex = '2';
    }
    return false;
}

function lookupTKProximityLogs2(EI, DT) 
{
    var left = (screen.width / 2) - (300 / 2);
    var top = (screen.height / 2) - (410 / 2);
    popupWin2 = window.open("Transactions/TimeModification/lookupTKProximityLogs.aspx?id=" + EI + "&dt=" + DT, "Popup", "scrollbars=no,resizable=no,width=317,height=318,top=" + top + ",left=" + left);
    return false;
}

function ChecklistSelection(index) 
{
    var tbl = document.getElementById('dgvResult');
    var tbr = tbl.rows[parseInt(index, 10) + 1];


    for (var i = 1; i < tbl.rows.length; i++) {
        if (i % 2 != 0) {
            tbl.rows[i].style.backgroundColor = '#F7F7DE';
        }
        else {
            tbl.rows[i].style.backgroundColor = '#FFFFFF';
        }

    }

    tbr.style.backgroundColor = '#FF2233';

    document.getElementById('hfControlNo').value = tbr.cells[1].innerHTML;
    document.getElementById('hfType').value = tbr.cells[8].innerHTML;
    document.getElementById('btnLoad').disabled = false;
    
    CheckBoxChecking();
}

function CheckBoxChecking() 
{
    var tbl = document.getElementById('dgvResult');
    var tbr = null;

    var tbcCBX = null;
    var tbcCN = null;
    var tbcSTATUS = null;

    var cbxHolder = null;
    var E1 = 0;
    var E2 = 0;
    var AP = 0;

    for (var i = 1; i < tbl.rows.length; i++)
    {
        tbr = tbl.rows[i];
        tbcCBX = tbr.cells[0];
        tbcCN = tbr.cells[1];
        tbcSTATUS = tbr.cells[3];

        cbxHolder = tbcCBX.getElementsByTagName('INPUT')[0];
        if (cbxHolder.checked) 
        {
            switch (tbcSTATUS.innerHTML) 
            {
                case 'ENDORSED TO CHECKER 1':
                    E1++;
                    break;
                case 'ENDORSED TO CHECKER 2':
                    E2++;
                    break;
                case 'ENDORSED TO APPROVER':
                    AP++;
                    break;
                default:
                    break;
            }
        }
    }

    try {
        document.getElementById('btnEndorseChecker2').disabled = !(E1 > 0);
        document.getElementById('btnEndorseApprover').disabled = !(E2 > 0);
        document.getElementById('btnApprove').disabled = !(AP > 0);
    }
    catch (err) { 
        //DISAPPROVE MODE
    }

    if ((E1 + E2 + AP) > 0) 
    {
        try {
            document.getElementById('btnDisapprove').disabled = false;
        }
        catch (err) {
            //CHECK or APPROVE MODE
        }
    }
}

function SelectAll() 
{
    var tbl = document.getElementById('dgvResult');
    var cbxTemp = null;
    var cbxAALL = tbl.rows[0].cells[0].getElementsByTagName('INPUT')[0];
    for (var i = 1; i < tbl.rows.length; i++) 
    {
        cbxTemp = tbl.rows[i].cells[0].getElementsByTagName('INPUT')[0];
        cbxTemp.checked = cbxAALL.checked;
    }

    CheckBoxChecking();
}

function loadTransaction() 
{
    var controlNo = document.getElementById('hfControlNo').value;
    var type = document.getElementById('hfType').value;
    switch (controlNo.substring(0, 1)) 
    {
        case "V": //OVERTIME
            url = "Transactions/Overtime/pgeOvertimeIndividual.aspx";
            break;
        case "L": //LEAVE
            url = "Transactions/Leave/pgeLeaveIndividual.aspx";
            break;
        case "T": //TIME MODIFICATION
            url = "Transactions/TimeModification/pgeTimeModification.aspx";
            break;
        case "I": //TAX CODE / CIVIL STATUS
            url = "Transactions/Personnel/pgeTaxCodeCivilStatus.aspx";
            break;
        case "E": //BENEFICIARY
            url = "Transactions/Personnel/pgeBeneficiaryUpdate.aspx";
            break;
        case "P": //ADDRESS MOVEMENT
                if (type.toUpperCase() == 'PRESENT'){
                    url = "Transactions/Personnel/pgeAddressPresent.aspx";
                }
                else if (type.toUpperCase() == 'PERMANENT') {
                    url = "Transactions/Personnel/pgeAddressPermanent.aspx";
                }
                else if (type.toUpperCase() == 'EMERGENCY CONTACT') {
                    url = "Transactions/Personnel/pgeAddressEmergency.aspx";
                }
            break;
        case "M": // MOVEMENT
                if (type == 'GROUP') {
                    url = "Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx";
                }
                else if (type == 'SHIFT'){
                    url = "Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx";
                }
                else if (type == 'COSTCENTER'){
                    url = "Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx";
                }
                else if (type == 'RESTDAY'){
                    url = "Transactions/WorkInformation/pgeRestdayIndividualUpdate.aspx";
                }
            break;
        case "S":
            url = "Transactions/ManhourAllocation/pgeJobSplitMod.aspx";
            break;
        case "W":
            url = "Transactions/TimeModification/pgeStraightWorkIndividual.aspx";
            break;
        case "G": // GATEPASS
            url = "Transactions/CustomForms/GatePass.aspx";
            break;
        default:
            break;
    }

    window.opener.location.href = url + '?cn=' + controlNo;
    return true;
}