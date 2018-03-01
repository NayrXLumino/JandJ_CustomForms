<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgePendingTransaction.aspx.cs" Inherits="Tools_PendingTransactions_pgePendingTransaction" Title="Pending Transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Table ID="Table2" runat="server">
       <asp:TableRow>
            <asp:TableCell Width="100px">
                <asp:Label ID="Label1" runat="server" Text="Transactions"></asp:Label>
            </asp:TableCell>
            <asp:TableCell Width="600px">
                <asp:DropDownList ID="ddlTransactions" runat="server" Width="250px" AutoPostBack="True" OnSelectedIndexChanged="ddlTransactions_SelectedIndexChanged">
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label2" runat="server" Text="Search"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtSearch" runat="server" Width="470px" OnTextChanged="txtSearch_TextChanged" AutoPostBack="True"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
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
                    function SendValueToParent() {
                        var val1 = document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value;
                        //window.opener.GetValueFrom_lookupNewPending(val1);
                        var url = '';
                        switch (val1.substring(0, 1)) {
                            case 'V':
                                url = 'Transactions/Overtime/pgeOvertimeIndividual.aspx?cn=' + val1;
                                break;
                            case 'L':
                                url = 'Transactions/Leave/pgeLeaveIndividual.aspx?cn=' + val1;
                                break;
                            case 'T':
                                url = 'Transactions/TimeModification/pgeTimeModification.aspx?cn=' + val1;
                                break;
                            case 'I':
                                url = 'Transactions/Personnel/pgeTaxCodeCivilStatus.aspx?cn=' + val1;
                                break;
                            case 'E':
                                url = 'Transactions/Personnel/pgeBeneficiaryUpdate.aspx?cn=' + val1;
                                break;
                            case 'P':
                                if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'Present')
                                    url = 'Transactions/Personnel/pgeAddressPresent.aspx?cn=' + val1;
                                else if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'Permanent')
                                    url = 'Transactions/Personnel/pgeAddressPermanent.aspx?cn=' + val1;
                                else if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'Emergency Contact')
                                    url = 'Transactions/Personnel/pgeAddressEmergency.aspx?cn=' + val1;
                                break;
                            case 'M':
                                if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'GROUP')
                                    url = 'Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx?cn=' + val1;
                                else if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'SHIFT')
                                    url = 'Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx?cn=' + val1;
                                else if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'COSTCENTER')
                                    url = 'Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx?cn=' + val1;
                                else if (document.getElementById('ctl00_ContentPlaceHolder1_hfType').value == 'RESTDAY')
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
                        window.location = "../../" + url;
                        //window.opener.setURL(url);
//                        window.close();
                        return false;
                    }

                    function RowSelection(index) {
                        var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
                        var tbr = tbl.rows[parseInt(index, 10) + 1];
                        var tbcControlNo = tbr.cells[1];
                        var tbcStatus = tbr.cells[2];
                        var tbcMoveType = tbr.cells[7]; //Applicable for Address Movement, WorkInfo movement
                        var cell;

                        for (var i = 1; i < tbl.rows.length; i++) {
                            if (i % 2 != 0) {
                                tbl.rows[i].style.backgroundColor = '#F7F7DE';

                            }
                            else {
                                tbl.rows[i].style.backgroundColor = '#FFFFFF';
                            }

                            //        cell = tbl.rows[i].cells[0];
                            //        var tbcCBX = cell.document.getElementById('ctl00_ContentPlaceHolder1_dgvResult_ctl02_chkBox');
                            //        if (tbcCBX.checked) {
                            //            document.getElementById('ctl00_ContentPlaceHolder1_btnEndorse').disabled = false;
                            //        }
                            //        else {
                            //            document.getElementById('ctl00_ContentPlaceHolder1_btnEndorse').disabled = true;
                            //        }
                        }
                        tbr.style.backgroundColor = '#FF2233';
                        document.getElementById('ctl00_ContentPlaceHolder1_hfControlNo').value = tbcControlNo.innerHTML;
                        document.getElementById('ctl00_ContentPlaceHolder1_hfType').value = tbcMoveType.innerHTML;
                        if (tbcStatus.innerHTML == 'NEW') {
                            document.getElementById('ctl00_ContentPlaceHolder1_btnLoad').disabled = false;
                        }
                        else {
                            document.getElementById('ctl00_ContentPlaceHolder1_bntLoad').disabled = true;
                        }
                    }
                </script>
                <asp:Panel ID="pnlResult" runat="server" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="890px" Height="300px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="2" 
                            ForeColor="Black" GridLines="Vertical" OnRowCreated="dgvResult_RowCreated" 
                            OnRowDataBound="PendingTransactions_RowDataBound" Font-Size="10pt" 
                            Width="100%" 
                            style="margin-bottom: 0px">
                            <RowStyle BackColor="#F7F7DE" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkBoxAll" runat="server"/>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkBox" runat="server"/>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Wrap="False" Width="15px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="lblCount" Text="" runat="server"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="btnEndorse" OnClick="btnEndorse_Click" runat="server" Text="ENDORSE" Width="200px" Enabled="True" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnCancel" OnClick="btnCancel_Click" runat="server" Text="CANCEL NEW TRANSACTIONS" Width="200px" Enabled="True" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnLoad" runat="server" Text="LOAD" Width="139px" />
                </asp:TableCell>
            </asp:TableRow>
    </asp:Table>
    <asp:HiddenField ID="hfTransactionType" runat="server"/>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
    <asp:HiddenField ID="hfControlNo" runat="server" Value=""/>
    <asp:HiddenField ID="hfType" runat="server" Value="" />
</asp:Content>