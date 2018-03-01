<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupCostCenter.aspx.cs" Inherits="Transactions_WorkInformation_lookupCostCenter" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Lookup CostCenter</title>
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
            var val1 = document.getElementById('hfControl').value;
            var val2 = document.getElementById('hfValueCode').value;
            var val3 = document.getElementById('hfValueDesc').value;

            window.opener.GetValueFrom_lookupCostCenter(val1, val2, val3);
            var txtBox = window.opener.document.getElementById('ctl00_ContentPlaceHolder1_txtFromCostCenterCode');
            if (txtBox != null)
                txtBox.focus();
            window.close();
            return false;
        }

        function SendValueToParent2(index) {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index, 10) + 1];
            var tbcCode = tbr.cells[0];
            var tbcDesc = tbr.cells[1];
            var val1 = document.getElementById('hfControl').value;
            window.opener.GetValueFrom_lookupCostCenter(val1, tbcCode.innerHTML, tbcDesc.innerHTML);
            var txtBox = window.opener.document.getElementById('ctl00_ContentPlaceHolder1_txtFromCostCenterCode');
            if (txtBox != null)
                txtBox.focus();
            window.close();
            return false;
        }

        function AssignValue(index) {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index, 10) + 1];
            var tbcCode = tbr.cells[0];
            var tbcDesc = tbr.cells[1];

            for (var i = 1; i < tbl.rows.length; i++) {
                if (i % 2 != 0) {
                    tbl.rows[i].style.backgroundColor = '#F7F7DE';
                }
                else {
                    tbl.rows[i].style.backgroundColor = '#FFFFFF';
                }

            }

            tbr.style.backgroundColor = '#FF2233';
            document.getElementById('hfValueCode').value = tbcCode.innerHTML;
            document.getElementById('hfValueDesc').value = tbcDesc.innerHTML;
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
                                <asp:TextBox ID="txtSearch" runat="server" Width="360px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td>
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
                    <asp:Panel ID="pnlResult" runat="server" Height="200px" ScrollBars="Both" Width="720px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="704px" OnRowDataBound="Lookup_RowDataBound" OnRowCreated="dgvResult_RowCreated">
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
        <asp:HiddenField ID="hfValueCode" runat="server"/>
        <asp:HiddenField ID="hfValueDesc" runat="server"/>
        <asp:HiddenField ID="hfControl" runat="server"/>
    </div>
    </form>
</body>
</html>
