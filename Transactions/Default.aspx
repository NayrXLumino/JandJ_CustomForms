<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_Default" Title="Transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="300px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Button ID="btnOvertime" runat="server" Text="OVERTIME" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/Default.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnLeave" runat="server" Text="LEAVE" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Leave/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnTimeRecord" runat="server" Text="TIME RECORD" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/TimeModification/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnPersonnel" runat="server" Text="PERSONNEL" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnFlexTime" runat="server" Text="FLEXTIME" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Flextime/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnJobSplit" runat="server" Text="JOBSPLIT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/JobSplit/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnMovement" runat="server" Text="WORK INFO" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/WorkInformation/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnCustomForms" runat="server" Text="CUSTOM FORMS" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/CustomForms/Default.aspx" Visible="false"  />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Information" Height="300px" ScrollBars="Vertical" Width="658px"  BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="White" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="262px" NextPrevFormat="FullMonth" Width="632px">
                    <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                    <TodayDayStyle BackColor="#CCCCCC" />
                    <OtherMonthDayStyle ForeColor="#999999" />
                    <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" VerticalAlign="Bottom" />
                    <DayHeaderStyle Font-Bold="True" Font-Size="8pt" />
                    <TitleStyle BackColor="White" BorderColor="Black" BorderWidth="4px" Font-Bold="True"
                        Font-Size="12pt" ForeColor="#333399" />
                </asp:Calendar>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>

