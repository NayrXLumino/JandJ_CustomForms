<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_ApprovalRoute_Default" Title="Approval Route" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="300px" Width="225px">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnApprovalRouteMaster" runat="server" Text="Approval Route Master" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/pgeApprovalRouteMaster.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnApprovalRouteMasterReport" runat="server" Text="Approval Route Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/pgeApprovalRouteMasterReport.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnEmployeeApprovalRoute" runat="server" Text="Employee Approval Route" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/pgeNewEmployeeApprovalRoute.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnEmployeeApprovalRouteReport" runat="server" Text="Employee Approval Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/pgeEmployeeApprovalRouteReport.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnApproverOverrideMaster" runat="server" Text="Approver Override Master" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Maintenance/ApprovalRoute/pgeApproverOverrideMaster.aspx" Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Information" Height="300px" ScrollBars="Vertical" Width="658px">
                <div style="height:266px">
                    <h1>SOME INFORMATION</h1>
                </div>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>

