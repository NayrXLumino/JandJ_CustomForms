<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_CustomForms_Default" Title="CustomForms" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="400px" Width="225px">
                <table cellpadding="0" cellspacing = "0">
                    <tr>
                        <td>
                            <asp:Button ID="btnApplicationForm" runat="server" Text="APPLICATION FORM" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/ApplicationForm.aspx" 
                            Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnApplicationReport" runat="server" Text="APPLICATION REPORT" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/ApplicationReport.aspx" 
                            Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnGatePass" runat="server" Text="GATE PASS" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/GatePass.aspx" 
                            Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="Button1" runat="server" Text="GATE PASS REPORT" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/GatePassReport.aspx" 
                            Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnShiftingBreak" runat="server" Text="SHIFTING BREAK" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/ShiftingBreak.aspx" 
                            Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="Button2" runat="server" Text="SHIFTING BREAK REPORT" 
                            Height="50px" Width="200px" CssClass="naviButton" 
                            PostBackUrl="~/Transactions/CustomForms/ShiftingBreakReport.aspx" 
                            Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Work Information" Height="400px" ScrollBars="Vertical" Width="658px">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Default Shift"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDefaultShift" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Work Type"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtWorkType" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Work Group"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtWorkGroup" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label4" runat="server" Text="Cost Center Assignment"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCostCenter" runat="server" ReadOnly="true" Width="470px" 
                                TextMode="MultiLine" Height="58px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label5" runat="server" Text="Assignmened Date"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAssignedDate" runat="server" ReadOnly="true" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>