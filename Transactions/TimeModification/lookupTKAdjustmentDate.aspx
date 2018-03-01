<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupTKAdjustmentDate.aspx.cs" Inherits="Transactions_TimeModification_lookupTKAdjustmentDate" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Adjustment Date Lookup</title>
    <style type="text/css">
        body
        {
            font-family:Arial;
            font-size:12px;
            text-transform:uppercase;
        }
        input
        {
            text-transform:uppercase;
        }
    </style>
    <script type="text/javascript">
        function SendValueToParent()
        {
            var vDate = document.getElementById('hfDate').value;
            var vShift = document.getElementById('hfShiftDesc').value;
            var vIn1 = document.getElementById('hfIn1').value;
            var vOut1 = document.getElementById('hfOut1').value;
            var vIn2 = document.getElementById('hfIn2').value;
            var vOut2 = document.getElementById('hfOut2').value;
            var vSIn1 = document.getElementById('hfSIn1').value;
            var vSOut1 = document.getElementById('hfSOut1').value;
            var vSIn2 = document.getElementById('hfSIn2').value;
            var vSOut2 = document.getElementById('hfSOut2').value;
            var vSType = document.getElementById('hfSType').value.substring(0,1);;
            
            window.opener.GetValueFrom_lookupTKAdjustmentDate( vDate
                                                             , vShift
                                                             , vIn1
                                                             , vOut1
                                                             , vIn2
                                                             , vOut2
                                                             , vSIn1
                                                             , vSOut1
                                                             , vSIn2
                                                             , vSOut2
                                                             , vSType);
            window.close();
            return false;
        }
        
        function SendValueToParent2(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            var vDate = tbr.cells[0];
            var vShift = tbr.cells[2];
            var vIn1 = tbr.cells[4];
            var vOut1 = tbr.cells[5];
            var vIn2 = tbr.cells[6];
            var vOut2 = tbr.cells[7];
            var vSIn1 = tbr.cells[8];
            var vSOut1 = tbr.cells[9];
            var vSIn2 = tbr.cells[10];
            var vSOut2 = tbr.cells[11];
            var vSType = tbr.cells[12];
            window.opener.GetValueFrom_lookupTKAdjustmentDate( vDate.innerHTML
                                                             , vShift.innerHTML
                                                             , vIn1.innerHTML
                                                             , vOut1.innerHTML
                                                             , vIn2.innerHTML
                                                             , vOut2.innerHTML
                                                             , vSIn1.innerHTML
                                                             , vSOut1.innerHTML
                                                             , vSIn2.innerHTML
                                                             , vSOut2.innerHTML
                                                             , vSType.innerHTML.substring(0,1));
            window.close();
            return false;
        }
        
        function  AssignValue(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            
            
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
            
            document.getElementById('hfDate').value = tbr.cells[0].innerHTML;
            document.getElementById('hfShiftDesc').value = tbr.cells[2].innerHTML;
            document.getElementById('hfIn1').value = tbr.cells[4].innerHTML;
            document.getElementById('hfOut1').value = tbr.cells[5].innerHTML;
            document.getElementById('hfIn2').value = tbr.cells[6].innerHTML;
            document.getElementById('hfOut2').value = tbr.cells[7].innerHTML;
            document.getElementById('hfSIn1').value = tbr.cells[8].innerHTML;
            document.getElementById('hfSOut1').value = tbr.cells[9].innerHTML;
            document.getElementById('hfSIn2').value = tbr.cells[10].innerHTML;
            document.getElementById('hfSOut2').value = tbr.cells[11].innerHTML;
            document.getElementById('hfSType').value = tbr.cells[12].innerHTML.substring(0,1);
            
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
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td style="width: 309px">
                                <asp:TextBox ID="txtSearch" runat="server" Width="446px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td style="width: 70px" align="right">
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
                    <asp:Panel ID="pnlResult" runat="server" Height="200px" ScrollBars="Both" Width="830px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="825px" OnRowDataBound="Lookup_RowDataBound" OnSelectedIndexChanged="dgvResult_SelectedIndexChanged" OnRowCreated="dgvResult_RowCreated">
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
                <td style="height: 35px" valign="bottom">
                    <asp:Button ID="btnSelect" runat="server" Text="SELECT" Width="124px" UseSubmitBehavior="False" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
        <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
        <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
        
        <asp:HiddenField ID="hfDate" runat="server"/>
        <asp:HiddenField ID="hfShiftDesc" runat="server"/>
        <asp:HiddenField ID="hfType" runat="server"/>
        <asp:HiddenField ID="hfIn1" runat="server"/>
        <asp:HiddenField ID="hfOut1" runat="server"/>
        <asp:HiddenField ID="hfIn2" runat="server"/>
        <asp:HiddenField ID="hfOut2" runat="server"/>
        <asp:HiddenField ID="hfLogControl" runat="server"/>
        <asp:HiddenField ID="hfSIn1" runat="server"/>
        <asp:HiddenField ID="hfSOut1" runat="server"/>
        <asp:HiddenField ID="hfSIn2" runat="server"/>
        <asp:HiddenField ID="hfSOut2" runat="server"/>
        <asp:HiddenField ID="hfSType" runat="server"/>
    </div>
    </form>
</body>
</html>
