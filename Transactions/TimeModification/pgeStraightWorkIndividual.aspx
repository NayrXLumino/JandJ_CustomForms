<%@ Page Title="Individual Straight Work Entry" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeStraightWorkIndividual.aspx.cs" Inherits="Transactions_TimeModification_pgeStraightWorkIndividual" MaintainScrollPositionOnPostback="true" %>
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
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
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
                                        <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="94px"></asp:Label>    
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
            <asp:Table ID="tblLeftControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrControlNo" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No" Width="92px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblOTDate" runat="server" Text="From Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <cc1:GMDatePicker ID="dtpFromDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Today"> 
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpFromDate" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow9" runat="server">
                    <asp:TableCell ID="TableCell19" runat="server">
                        <asp:Label ID="Label1" runat="server" Text="To Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell20" runat="server">
                        <cc1:GMDatePicker ID="dtpToDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpToDate" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="lblShift" runat="server" Text="Shift"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtShiftCode" runat="server" BackColor="Gainsboro" Width="90px"></asp:TextBox>            
                                </td>
                                <td>
                                    <asp:Button ID="btnShift" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" CausesValidation="false" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtShiftDesc" runat="server" Width="174px" BackColor="Gainsboro"></asp:TextBox>  
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtShiftCode" Font-Bold="true"></asp:RequiredFieldValidator>          
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblType" runat="server" Text="Unpaid Break"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:TextBox ID="txtUnpaidBreak" runat="server" ToolTip="Unpaid break per day. Input in minutes." Text="0" MaxLength="3" Width="90px" CssClass="textboxNumber" ></asp:TextBox>&nbsp;minute(s)
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtUnpaidBreak" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow7" runat="server">
                    <asp:TableCell ID="TableCell15" VerticalAlign="Top" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
        <td valign="top">
            <asp:Table ID="tblRightControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrFiller1" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller1" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller1" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller2" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller2" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller2" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller3" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller3" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller3" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Width="130px" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtRemarks" Font-Bold="true" Enabled="false"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow8" runat="server">
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="295px" ReadOnly="true"></asp:TextBox>
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
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControlsStraightWork()" /> 
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
</asp:Content>

