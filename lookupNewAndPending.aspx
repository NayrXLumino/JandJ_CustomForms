<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupNewAndPending.aspx.cs" Inherits="lookupNewAndPending" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>New and Pending Transactions</title>
    <link href="CSS/lookup.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function SendValueToParent()
        {
            var val1 = document.getElementById('hfControlNo').value;
            //window.opener.GetValueFrom_lookupNewPending(val1);
            var url = '';
            switch(val1.substring(0,1))
            {
                case 'V': 
                    url = 'Transactions/Overtime/pgeOvertimeIndividual.aspx?cn='+ val1;
                    break;
                case 'L': 
                    url = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn='+ val1;
                    break;
                case 'T': 
                    url = 'Transactions/TimeModification/pgeTimeModification.aspx?cn='+ val1;
                    break;
                case 'I': 
                    url = 'Transactions/Personnel/pgeTaxCodeCivilStatus.aspx?cn='+ val1;
                    break;
                case 'E':
                    url = 'Transactions/Personnel/pgeBeneficiaryUpdate.aspx?cn=' + val1;
                    break;
                case 'P':
                    if (document.getElementById('hfType').value == 'Present')
                        url = 'Transactions/Personnel/pgeAddressPresent.aspx?cn=' + val1;
                    else if (document.getElementById('hfType').value == 'Permanent')
                        url = 'Transactions/Personnel/pgeAddressPermanent.aspx?cn=' + val1;
                    else if (document.getElementById('hfType').value == 'Emergency Contact')
                        url = 'Transactions/Personnel/pgeAddressEmergency.aspx?cn=' + val1;
                    break;
                case 'M':
                    if (document.getElementById('hfType').value == 'GROUP')
                        url = 'Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx?cn=' + val1;
                    else if (document.getElementById('hfType').value == 'SHIFT')
                        url = 'Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx?cn=' + val1;
                    else if (document.getElementById('hfType').value == 'COSTCENTER')
                        url = 'Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx?cn=' + val1;
                    else if (document.getElementById('hfType').value == 'RESTDAY')
                        url = 'Transactions/WorkInformation/pgeRestdayIndividualUpdate.aspx?cn=' + val1;
                    break;
// Jobsplit whose control no starts with J are not currently loadable in this lookup
//                case 'J':
//                    url = 'Transactions/ManhourAllocation/pgeWorkRecord.aspx?cn=' + val1;
//                    break;
                case 'S':
                    url = 'Transactions/ManhourAllocation/pgeJobSplitMod.aspx?cn=' + val1;
                    break;
                case 'W':
                    url = 'Transactions/TimeModification/pgeStraightWorkIndividual.aspx?cn=' + val1;
                    break;
                default:
                    break;
            }
            window.opener.location.href = url;
            //window.opener.setURL(url);
            window.close();
            return false;
        }
        
        function SendValueToParent2(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            var tbc = tbr.cells[0];
            var tbcStatus = tbr.cells[1];
            var tbcMoveType = tbr.cells[6];
            if(tbcStatus.innerHTML == 'NEW')
            {
                //window.opener.GetValueFrom_lookupNewPending(tbc.innerHTML);
                var url = '';
                switch(tbc.innerHTML.substring(0,1))
                {
                    case 'V': 
                        url = 'Transactions/Overtime/pgeOvertimeIndividual.aspx?cn='+tbc.innerHTML;
                        break;
                    case 'L': 
                        url = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn='+tbc.innerHTML;
                        break;
                    case 'T': 
                        url = 'Transactions/TimeModification/pgeTimeModification.aspx?cn='+tbc.innerHTML;
                        break;
                    case 'I': 
                        url = 'Transactions/Personnel/pgeTaxCodeCivilStatus.aspx?cn='+tbc.innerHTML;
                        break;
                    case 'E':
                        url = 'Transactions/Personnel/pgeBeneficiaryUpdate.aspx?cn=' + tbc.innerHTML;
                        break;
                    case 'P':
                        if (tbcMoveType.innerHTML == 'Present')
                            url = 'Transactions/Personnel/pgeAddressPresent.aspx?cn=' + tbc.innerHTML;
                        else if (tbcMoveType.innerHTML == 'Permanent')
                            url = 'Transactions/Personnel/pgeAddressPermanent.aspx?cn=' + tbc.innerHTML;
                        else if (tbcMoveType.innerHTML == 'Emergency Contact')
                            url = 'Transactions/Personnel/pgeAddressEmergency.aspx?cn=' + tbc.innerHTML;
                        break;
                    case 'M':
                        if (tbcMoveType.innerHTML == 'GROUP')
                            url = 'Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx?cn=' + tbc.innerHTML;
                        else if (tbcMoveType.innerHTML == 'SHIFT')
                            url = 'Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx?cn=' + tbc.innerHTML;
                        else if (tbcMoveType.innerHTML == 'COSTCENTER')
                            url = 'Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx?cn=' + tbc.innerHTML;
                        else if (tbcMoveType.innerHTML == 'RESTDAY')
                            url = 'Transactions/WorkInformation/pgeRestdayIndividualUpdate.aspx?cn=' + tbc.innerHTML;
                        break;
// Jobsplit whose control no starts with J are not currently loadable in this lookup 
//                    case 'J': 
//                    url = 'Transactions/ManhourAllocation/pgeWorkRecord.aspx?cn=' + tbc.innerHTML; 
//                    break; 
                    case 'S':
                        url = 'Transactions/ManhourAllocation/pgeJobSplitMod.aspx?cn=' + tbc.innerHTML;  
                    default:
                        break;
                }
                //window.opener.location.href = url;
                window.opener.setURL(url);
                //window.close();
            }
            return false;
        }
        
        function  AssignValue(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            var tbcControlNo = tbr.cells[0];
            var tbcStatus = tbr.cells[1];
            var tbcMoveType = tbr.cells[6];//Applicable for Address Movement, WorkInfo movement
            
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
            document.getElementById('hfControlNo').value = tbcControlNo.innerHTML;
            document.getElementById('hfType').value = tbcMoveType.innerHTML;
            if(tbcStatus.innerHTML == 'NEW')
            {
                document.getElementById('btnRetrieve').disabled = false;
            }
            else
            {
                document.getElementById('btnRetrieve').disabled = true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search" Width="54px"></asp:Label>
                            </td>
                            <td style="width: 309px">
                                <asp:TextBox ID="txtSearch" runat="server" Width="556px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td style="width: 88px" align="right">
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
                            </td>
                            <td style="width: 172px" align="center">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)" Font-Size="11px"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 197px">
                    <script language="javascript" type="text/javascript">
                            var IsPostBack= '<%=IsPostBack.ToString() %>';
                            window.onload = function()
                            {
                                var strCook = document.cookie;
                                if(strCook.indexOf("!~")!=0)
                                {
                                    var intS = strCook.indexOf("!~");
                                    var intE = strCook.indexOf("~!");
                                    var strPos = strCook.substring(intS+2,intE);
                                    if (IsPostBack=='True')
                                    {
                                    document.getElementById("<%=pnlResult.ClientID %>").scrollTop = strPos;
                                    }
                                    else
                                    {
                                    document.cookie = "yPos=!~0~!";
                                    }
                                }
                            }
                            function SetDivPosition(){
                                var intY = document.getElementById("<%=pnlResult.ClientID %>").scrollTop;
                                //document.title = intY;
                                document.cookie = "yPos=!~" + intY + "~!";
                            }
                    </script>
                    <asp:Panel ID="pnlResult" runat="server" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="975px" Height="350px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="100%" OnRowDataBound="Lookup_RowDataBound" OnRowCreated="dgvResult_RowCreated">
                            <RowStyle BackColor="#F7F7DE" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td style="height: 50px">
                    <asp:Button ID="btnRetrieve" runat="server" Text="LOAD TRANSACTION"/>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
        <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
        <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
        <asp:HiddenField ID="hfControlNo" runat="server" Value=""/>
        <asp:HiddenField ID="hfType" runat="server" Value="" />
    </div>
    </form>
</body>
</html>
