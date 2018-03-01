<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_Default" Title="Maintenance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="300px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnArrovalRoute" runat="server" Text="APPROVAL ROUTE" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/Default.aspx" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Information" Height="300px" ScrollBars="Vertical" Width="658px"  BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>

