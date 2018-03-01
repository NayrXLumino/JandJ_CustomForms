<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeTaxCodeCivilStatus.aspx.cs" Inherits="Transactions_Payroll_pgeTaxCodeCivilStatus" Title="Tax Code/ Civil Status" %>
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
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('PERSONNEL')" UseSubmitBehavior="false" Width="22px" />
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
                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro" Width="200px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrEffectivity" runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblEffectivityDate" runat="server" Text="Effectivity Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" ColumnSpan="3">
                        <cc1:GMDatePicker ID="dtpEffectivityDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                </asp:TableRow>
                <asp:TableRow ID="tbrTaxCodeFrom" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="Label2" runat="server" Text="From Tax Code"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:TextBox ID="txtFromTaxCodeCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        -
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtFromTaxCodeDesc" runat="server" BackColor="Gainsboro" Width="350px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrTaxCodeTo" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="lblTaxCode" runat="server" Text="To Tax Code"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:TextBox ID="txtToTaxCodeCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnTaxCode" runat="server" Text="..." Width="22px" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtToTaxCodeDesc" runat="server" BackColor="Gainsboro" Width="350px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCivilFrom" runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="ldlFromCivil" runat="server" Text="From Civil Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txtFromCivilCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        -
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtFromCivilDesc" runat="server" BackColor="Gainsboro" Width="350px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCivilTo" runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblToCivil" runat="server" Text="To Civil Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtToCivilCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnCivilStatus" runat="server" Text="..." Width="22px" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtToCivilDesc" runat="server" BackColor="Gainsboro" Width="350px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrReason" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label1" runat="server" Text="Reason for Update"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtReason" runat="server" Width="510px" TextMode="MultiLine" Height="35px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrRemarks" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtRemarks" runat="server" Width="510px" TextMode="MultiLine" Height="35px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrStatus" runat="server">
                    <asp:TableCell>
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="300px" ReadOnly="true"></asp:TextBox>
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
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" 
                            OnClick="btnY_Click" OnClientClick="return clearControlsTax()" 
                            CausesValidation="False" />
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


