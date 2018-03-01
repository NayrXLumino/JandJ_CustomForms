<%@ Page Title="Restday Individual Update" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeRestdayIndividualUpdate.aspx.cs" Inherits="Transactions_WorkInformation_pgeRestdayIndividualUpdate" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <td valign="top">
                            <asp:Panel ID="pnlUserInfo" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmployeeId" runat="server" Text="ID No." Width="116px"></asp:Label>    
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('TIMEKEEP')" UseSubmitBehavior="false" Width="22px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmployeeName" runat="server" Text="Name"></asp:Label>    
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="100px"></asp:Label>    
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td valign="top">
                            <asp:Panel ID="pnlOtherInfo" runat="server" Width="460px">
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td valign="top">
                <asp:Table ID="tblLeftControls" runat="server">
                    <asp:TableRow ID="tbrControlNo" runat="server">
                        <asp:TableCell ID="TableCell3" runat="server">
                            <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell4" runat="server" ColumnSpan="2">
                            <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro" Width="200px" MaxLength="12"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tbrFromDate" runat="server">
                        <asp:TableCell ID="TableCell111" runat="server">
                            <asp:Label ID="lblFromDate" runat="server" Text="From Date"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell21" runat="server">
                            <cc1:GMDatePicker ID="dtpFromDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                        <CalendarDayStyle Font-Size="9pt" />
                                        <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                        <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                        <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                        <CalendarFont Names="Arial" Size="X-Small" />
                                    <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                    <CalendarFooterStyle Width="150px" />
                                    <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                    <CalendarDayHeaderStyle Width="150px" />
                            </cc1:GMDatePicker>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtFromInfo" runat="server" BackColor="Gainsboro" Width="390px"></asp:TextBox>
                            <asp:HiddenField ID="hfValidFrom" runat="server" Value="1"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="From Date must be a restday" ValidationExpression="REST.*" ControlToValidate="txtFromInfo"></asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpFromDate" Font-Bold="true"></asp:RequiredFieldValidator>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tbrToDate" runat="server">
                        <asp:TableCell ID="TableCell5" runat="server">
                            <asp:Label ID="Label3" runat="server" Text="To Date"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell6" runat="server">
                            <cc1:GMDatePicker ID="dtpToDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                        <CalendarDayStyle Font-Size="9pt" />
                                        <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                        <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                        <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                        <CalendarFont Names="Arial" Size="X-Small" />
                                    <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                    <CalendarFooterStyle Width="150px" />
                                    <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                    <CalendarDayHeaderStyle Width="150px" />
                            </cc1:GMDatePicker>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox ID="txtToInfo" runat="server" BackColor="Gainsboro" Width="390px"></asp:TextBox>
                            <asp:HiddenField ID="hfValidTo" runat="server" Value="1"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="To Date must not be a restday" ValidationExpression="^((?!REST).)*$" ControlToValidate="txtToInfo"></asp:RegularExpressionValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpToDate" Font-Bold="true"></asp:RequiredFieldValidator>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tbrReason" runat="server">
                        <asp:TableCell ID="TableCell61" runat="server" VerticalAlign="Top">
                            <asp:Label ID="Label1" runat="server" Text="Reason for Movement"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell7" runat="server" ColumnSpan="2">
                            <asp:TextBox ID="txtReason" runat="server" Width="510px" TextMode="MultiLine" Height="35px" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tbrRemarks" runat="server">
                        <asp:TableCell ID="TableCell8" runat="server" VerticalAlign="Top">
                            <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell9" runat="server" ColumnSpan="2">
                            <asp:TextBox ID="txtRemarks" runat="server" Width="510px" TextMode="MultiLine" Height="35px" BackColor="Gainsboro" MaxLength="200"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="tbrStatus" runat="server">
                        <asp:TableCell>
                            <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell10" runat="server" ColumnSpan="2">
                            <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="290px" ReadOnly="true" MaxLength="100"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnZ" runat="server" Text="[btnZ]" Width="225px" OnClick="btnZ_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControlsRestDay()" />
                        </td>
                        <td>
                            <asp:Button ID="btnX" runat="server" Text="[btnX]" Width="225px" OnClick="btnX_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
    <asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
    <asp:HiddenField ID="hfPrevEntry" runat="server"/>
    <asp:HiddenField ID="hfDateFrom" runat="server" />
    <asp:HiddenField ID="hfDateTo" runat="server" />
</asp:Content>

