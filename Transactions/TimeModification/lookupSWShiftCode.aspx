<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupSWShiftCode.aspx.cs" Inherits="Transactions_TimeModification_lookupSWShiftCode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shift Lookup</title>
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
        function SendValueToParent() {
            var vShiftCode = document.getElementById('hfShiftCode').value;
            var vShiftDesc = document.getElementById('hfShiftDesc').value;

            window.opener.GetValueFrom_lookupSWShiftCode( vShiftCode
                                                        , vShiftDesc);
            window.close();
            return false;
        }

        function SendValueToParent2(index) {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index, 10) + 1];
            var vShiftCode = tbr.cells[0];
            var vShiftDesc = tbr.cells[1];
            window.opener.GetValueFrom_lookupSWShiftCode( vShiftCode.innerHTML
                                                        , vShiftDesc.innerHTML);
            window.close();
            return false;
        }

        function AssignValue(index) {
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

            document.getElementById('hfShiftCode').value = tbr.cells[0].innerHTML;
            document.getElementById('hfShiftDesc').value = tbr.cells[1].innerHTML;
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
                                <asp:TextBox ID="txtSearch" runat="server" Width="346px" AutoPostBack="True" 
                                    OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
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
                        var IsPostBack = '<%=IsPostBack.ToString() %>';
                        window.onload = function () {
                            var strCook = document.cookie;
                            if (strCook.indexOf("!~") != 0) {
                                var intS = strCook.indexOf("!~");
                                var intE = strCook.indexOf("~!");
                                var strPos = strCook.substring(intS + 2, intE);
                                if (IsPostBack == 'True') {
                                    document.getElementById("<%=pnlResult.ClientID %>").scrollTop = strPos;
                                }
                                else {
                                    document.cookie = "yPos=!~0~!";
                                }
                            }
                        }
                        function SetDivPosition() {
                            var intY = document.getElementById("<%=pnlResult.ClientID %>").scrollTop;
                            //document.title = intY;
                            document.cookie = "yPos=!~" + intY + "~!";
                        }
                    </script>
                    <asp:Panel ID="pnlResult" runat="server" Height="180px" ScrollBars="Both" Width="720px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                            ForeColor="Black" GridLines="Vertical" Width="700px" 
                            OnRowDataBound="Lookup_RowDataBound" 
                            OnSelectedIndexChanged="dgvResult_SelectedIndexChanged" 
                            OnRowCreated="dgvResult_RowCreated">
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
        
        <asp:HiddenField ID="hfShiftCode" runat="server"/>
        <asp:HiddenField ID="hfShiftDesc" runat="server"/>
    </div>
    </form>
</body>
</html>
