<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeTimeModification.aspx.cs" Inherits="Transactions_TimeModification_pgeTimeModification" Title="Time Modification" EnableEventValidation="false"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table cellpadding="0" cellspacing="0" onmouseover="callOnce()">
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
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblAdjustmentDate" runat="server" Text="Adjustment Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <asp:TextBox ID="txtAdjustmentDate" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnAdjustmentDate" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtAdjustmentDate" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="lblShift" runat="server" Text="Shift for the Day"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txtShift" runat="server" Width="295px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:TextBox ID="txtType" runat="server" Width="295px" BackColor="Gainsboro"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtType" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow4" runat="server">
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:Label ID="lblTimeIn1" runat="server" Text="Time IN 1"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server">
                        <asp:TextBox ID="txtTimeIn1" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox>&nbsp;<label class="reqField" id="reqIn1"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:Label ID="lblTimeOut1" runat="server" Text="Time OUT 1"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:TextBox ID="txtTimeOut1" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox>&nbsp;<label class="reqField" id="reqOut1"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow6" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="lblTimeIn2" runat="server" Text="Time IN 2"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:TextBox ID="txtTimeIn2" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox>&nbsp;<label class="reqField" id="reqIn2"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow7" runat="server">
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Label ID="lblTimeOut2" runat="server" Text="Time OUT 2"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:TextBox ID="txtTimeOut2" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox>&nbsp;<label class="reqField" id="reqOut2"></label>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
        <td valign="top">
            <asp:Table ID="tblRightControls" runat="server" Width="440px">
                <asp:TableRow ID="TableRow9" runat="server">
                    <asp:TableCell ID="TableCell19" VerticalAlign="Top" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell20" runat="server">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Width="130px" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
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
                        <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="299px" ReadOnly="true"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrProximityLogs" runat="server">
                    <asp:TableCell ID="TableCell21" runat="server">
                        <asp:Label ID="Label1" runat="server" Text="Proximity Logs"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell22" runat="server">
                        <asp:Button ID="btnProximityLogs" runat="server" Text="VIEW LOGS" Width="200px" UseSubmitBehavior="false" CausesValidation="false"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="height: 31px">
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
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControls()" />
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
<asp:HiddenField ID="hfPrevAdjustDate" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfLogControl" runat="server" Value="!!!!" />
<asp:HiddenField ID="hfModType" runat="server" Value="" />
<asp:HiddenField ID="hfTimeModGap" runat="server" Value="0" />
<asp:HiddenField ID="hfBYPASSTIMEVERIFIACTION" runat="server"/>
</asp:Content>



