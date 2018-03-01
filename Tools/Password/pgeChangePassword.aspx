<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeChangePassword.aspx.cs" Inherits="Tools_Password_pgeChangePassword" Title="Change/Reset Password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2" style="width: 896px">
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
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('GENERAL')" UseSubmitBehavior="false" Width="22px" />
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
                                        <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="120px"></asp:Label>    
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
        <td colspan="2" style="width: 896px">
            <hr />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:Table ID="tblMain" runat="server">
                <asp:TableRow ID="tbrCurrentPassword" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblCurrentPassword" runat="server" Text="Current Password" Width="120px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server" HorizontalAlign="Left">
                        <asp:TextBox ID="txtCurrentPassword" runat="server" Width="295px" TextMode="Password" CssClass="textareaNormal"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtCurrentPassword" Font-Bold="true" ValidationGroup="ChangePass"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblNewPassword" runat="server" Text="New Password"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server" HorizontalAlign="Left">
                        <asp:TextBox ID="txtNewPassword" runat="server" Width="295px" TextMode="Password" CssClass="textareaNormal"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtNewPassword" Font-Bold="true" ValidationGroup="ChangePass"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                    <asp:TableCell RowSpan="3" VerticalAlign="Top">
                        <asp:Label ID="lblPasswordRequirements" runat="server" Text="" Width="450px"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server" HorizontalAlign="Left">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" Width="295px" TextMode="Password" CssClass="textareaNormal"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqOTStartTiime" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtConfirmPassword" Font-Bold="true" ValidationGroup="ChangePass"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="width: 896px">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="2" style="width: 896px">
            <table>
                <tr>
                    <td style="width: 297px" align="right">
                        <asp:Button ID="btnSave" runat="server" Text="SAVE NEW PASSWORD" Width="175px" ValidationGroup="ChangePass" UseSubmitBehavior="true" OnClick="btnSave_Click"/>
                    </td>
                    <td style="width: 126px" align="right">
                        <asp:Button ID="btnReset" runat="server" Text="RESET" Width="103px" UseSubmitBehavior="true" OnClick="btnReset_Click"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
</asp:Content>



