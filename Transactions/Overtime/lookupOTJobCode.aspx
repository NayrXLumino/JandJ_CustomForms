<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupOTJobCode.aspx.cs" Inherits="Transactions_Overtime_lookupOTJobCode" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Job Lookup</title>
    <style type="text/css">
        body
        {
            font-family:Arial;
            font-size:12px;
        }
        input
        {
            text-transform:uppercase;
        }
    </style>
    <script type="text/javascript">
        function SendValueToParent()
        {
            var val1 = document.getElementById('hfJobCode').value;
            var val2 = document.getElementById('hfClientJobNo').value;
            var val3 = document.getElementById('hfClientJobName').value;
            
            window.opener.GetValueFrom_lookupOTJobCode(val1, val2, val3);
            window.close();
            return false;
        }
        
        function SendValueToParent()
        {
        
            var val1 = document.getElementById('hfJobCode').value;
            var val2 = document.getElementById('hfClientJobNo').value;
            var val3 = document.getElementById('hfClientJobName').value;
            window.opener.GetValueFrom_lookupOTJobCode(val1, val2, val3);
            window.close();
            return false;
        }
        
        function SendValueToParent2(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            var tbc1 = tbr.cells[0];
            var tbc2 = tbr.cells[1];
            var tbc3 = tbr.cells[2];
            window.opener.GetValueFrom_lookupGenericFiller(tbc1.innerHTML, tbc2.innerHTML, tbc3.innerHTML,);
            window.close();
            return false;
        }
        
        function  AssignValue(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            var tbc1 = tbr.cells[0];
            var tbc2 = tbr.cells[1];
            var tbc3 = tbr.cells[2];
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
            document.getElementById('hfJobCode').value = tbc1.innerHTML;
            document.getElementById('hfClientJobNo').value = tbc2.innerHTML;
            document.getElementById('hfClientJobName').value = tbc3.innerHTML;
            
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 717px">
                    <table style="width: 710px">
                        <tr>
                            <td>
                                <asp:Label ID="lblCostCenter" runat="server" Text="Costcenter" Font-Size="11px" Width="75px"></asp:Label>
                            </td>
                            <td colspan="4">
                                <asp:DropDownList ID="ddlCostCenter" runat="server" Width="584px" Font-Size="11px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:CheckBox ID="cbxExtension" runat="server" Text="EXT" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search" Font-Size="11px"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="300px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged" Font-Size="11px"></asp:TextBox>
                            </td>
                            <td align="right">
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" Font-Size="11px" />
                            </td>
                            <td style="width: 175px" align="center">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)" Font-Size="11px"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" Font-Size="11px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 717px">
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 197px; width: 717px;">
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
                    <asp:Panel ID="pnlResult" runat="server" Height="180px" ScrollBars="Both" Width="715px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="699px" OnRowDataBound="Lookup_RowDataBound" OnSelectedIndexChanged="dgvResult_SelectedIndexChanged" OnRowCreated="dgvResult_RowCreated">
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
                <td style="height: 35px; width: 717px;" valign="bottom">
                    <asp:Button ID="btnSelect" runat="server" Text="SELECT" Width="124px" UseSubmitBehavior="False" Font-Size="12px" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
        <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
        <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
        <asp:HiddenField ID="hfJobCode" runat="server"/>
        <asp:HiddenField ID="hfClientJobNo" runat="server"/>
        <asp:HiddenField ID="hfClientJobName" runat="server"/>
    </div>
    </form>
</body>
</html>
