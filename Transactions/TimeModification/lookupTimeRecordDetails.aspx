<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupTimeRecordDetails.aspx.cs" Inherits="Transactions_TimeModification_lookupTimeRecordDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Process Date Details</title>
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
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Label ID="lblID" runat="server" Text="Employee ID: "></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblDate" runat="server" Text="Date: "></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table cellpadding="0" cellspacing="0" style="width:auto; height:auto; margin-top:3px;">
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="Search: "></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="500px" 
                                    ontextchanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="height: 197px" colspan="2">
                    <asp:Panel ID="Panel5" runat="server" GroupingText ="Transaction Trail(Overtime,Leave, Time Mod)">
                        <asp:Panel ID="pnlResult" runat="server" Height="150px" ScrollBars="Both" Width="830px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                            <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="825px" OnRowDataBound="Lookup_RowDataBound" OnRowCreated="dgvResult_RowCreated">
                                <RowStyle BackColor="#F7F7DE" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </asp:Panel>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel4" runat="server" GroupingText ="Log Trail" Width="650px">
                        <asp:Panel ID="Panel1" runat="server" Height="120px" ScrollBars="Both" 
                            Width="640px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                            <asp:GridView ID="dgvLogTrail" runat="server" BackColor="White" 
                                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                ForeColor="Black" GridLines="Vertical" Width="640px" 
                                OnRowDataBound="Lookup_RowDataBound" 
                                OnRowCreated="dgvResult_RowCreated">
                                <RowStyle BackColor="#F7F7DE" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </asp:Panel>
                    </asp:Panel>
                </td>
                <td align="right">
                    <asp:Panel ID="Panel3" runat="server" GroupingText ="Proximity Logs" 
                        Width="155px" >
                        <asp:Panel ID="Panel2" runat="server" Height="120px" ScrollBars="Vertical" 
                            Width="140px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                            <asp:GridView ID="dgvProximity" runat="server" BackColor="White" 
                                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                ForeColor="Black" GridLines="Vertical" Width="120px" 
                                OnRowDataBound="Lookup_RowDataBound" 
                                OnRowCreated="dgvResult_RowCreated">
                                <RowStyle BackColor="#F7F7DE" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </asp:Panel>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
