// JScript File
function lookupUserMaster(transactionType)
{   
    try
    {
        isPostBack = false;
    }
    catch(err)
    {
    
    }
    var left=(screen.width/2)-(735/2);
    var top=(screen.height/2)-(400/2);
    var url = "lookupUserMaster.aspx?type="+transactionType;
    popupWin = window.open( url ,"Sample","scrollbars=no,resizable=no,width=735,height=400,top=" +top+ ",left=" +left);
    return false;
}
function GetValueFrom_lookupUserMaster(empId, empName, empNickname)
{
    if(empId != '')
    {
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeId').value = empId;
        document.getElementById('ctl00_ContentPlaceHolder1_txtEmployeeName').value = empName;
        document.getElementById('ctl00_ContentPlaceHolder1_txtNickname').value = empNickname;
        __doPostBack();
    }
    return false;
}