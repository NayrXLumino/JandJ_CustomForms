<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Tools_Password_Default" Title="Password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="300px" Width="225px">
                <table>
                    <tr>
                        <td>
                            
                        </td>
                    </tr>
                </table>
                
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Information" Height="300px" ScrollBars="Vertical" Width="658px">
            <h1>SOME INFORMATION</h1>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>
