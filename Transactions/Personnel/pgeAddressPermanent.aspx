<%@ Page Title="Permanent Address Update" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeAddressPermanent.aspx.cs" Inherits="Transactions_Personnel_pgeAddressPermanent" %>
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
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No." Width="115px"></asp:Label>    
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('OVERTIME')" UseSubmitBehavior="false" Width="22px" />
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
                <asp:TableRow ID="tbrAddress1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label2" runat="server" Text="Number/Street"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell7" runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtAddress1" runat="server" MaxLength="500" Width="512px" TextMode="MultiLine" Height="35px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtAddress1" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrAddress2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="lblAddress2" runat="server" Text="Barangay/Municipality"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:TextBox ID="txtAddress2Code" runat="server" BackColor="Gainsboro" MaxLength="10"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnAddress2" runat="server" Text="..." Width="22px" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtAddress2Desc" runat="server" BackColor="Gainsboro" Width="350px" MaxLength="100"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrAddress3" runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblAddress3" runat="server" Text="City/Province/District"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txtAddress3Code" runat="server" BackColor="Gainsboro" MaxLength="10"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnAddress3" runat="server" Text="..." Width="22px" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtAddress3Desc" runat="server" BackColor="Gainsboro" Width="350px" MaxLength="100"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrTelephoneNo" runat="server">
                    <asp:TableCell ID="TableCell4" runat="server">
                        <asp:Label ID="lblTelephoneNo" runat="server" Text="Telephone Number"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" ColumnSpan="3">
                        <div onkeydown="return fn_validateNumericOnly()">
                        <asp:TextBox ID="txtTelephoneNo" runat="server" MaxLength="10" Width="512px"></asp:TextBox>
                            </div>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrReason" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label1" runat="server" Text="Reason for Update"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtReason" runat="server" Width="512px" TextMode="MultiLine" Height="35px" MaxLength="200"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrRemarks" runat="server">
                    <asp:TableCell ID="TableCell8" runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" ColumnSpan="3">
                        <asp:TextBox ID="txtRemarks" runat="server" Width="512px" TextMode="MultiLine" Height="35px" MaxLength="200" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrStatus" runat="server">
                    <asp:TableCell>
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server" ColumnSpan="3">
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
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControlsPermanent()" />
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

