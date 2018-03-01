<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeBatch.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeBatch" Title="Batch Overtime Entry" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table cellpadding="0" cellspacing="0" style="width: 894px">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" style="width: 893px">
                <tr>
                    <td valign="top">
                        <asp:Table ID="tblEntryLeft" runat="server" Width="371px">
                            <asp:TableRow runat="server">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblOTDate" runat="server" Text="Overtime Date"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <cc1:GMDatePicker ID="dtpOTDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                            <asp:TableRow runat="server">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblGroup" runat="server" Text="Work Group"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="ddlGroup" runat="server" Width="270px" OnSelectedIndexChanged="ddlGroup_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="reqGroup" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="ddlGroup"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblShift" runat="server" Text="Shift"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="ddlShift" runat="server" Width="270px" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="reqShift" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="ddlShift"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller1" Visible="false">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller1" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller1" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller1" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller2" Visible="false">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller2" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller2" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller2" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller3" Visible="false">
                                <asp:TableCell runat="server">
                                    <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller3" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller3" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller3" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                    <td valign="top">
                        <asp:Table ID="tblENtryRight" runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:DropDownList ID="ddlType" runat="server" Width="130px">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlType" Font-Bold="true"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblStart" runat="server" Text="Start Time"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTStartTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqStart" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTStartTime"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblHours" runat="server" Text="Hours"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTHours" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqHours" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTHours"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblEnd" runat="server" Text="End Time"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTEndTime" runat="server" BackColor="Gainsboro" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqEnd" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTEndTime"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="top">
                                    <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtReason" runat="server" Width="350px" TextMode="MultiLine" Height="35px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtReason"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table>
                            <tr>
                                <td style="width: 75px">
                                    <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlCostcenter" runat="server" Width="787px" AutoPostBack="True" OnSelectedIndexChanged="ddlCostcenter_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnlBound" runat="server" Width="890px" GroupingText="Bound">
                <table width="100%">
                    <tr>
                        <td style="width: 41px">
                            <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="349px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                        </td>
                        <td>
                        
                        </td>
                        <td align="center">
                            <asp:Label ID="ibiInclude" runat="server" Text="Included in Transaction"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top">
                            <asp:ListBox ID="lbxChoice" runat="server" Height="150px" Width="400px"></asp:ListBox>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnIncludeAll" runat="server" Text=">>" Width="22px" OnClick="btnIncludeAll_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnIncludeIndi" runat="server" Text=">" Width="22px" OnClick="btnIncludeIndi_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnRemoveIndi" runat="server" Text="<" Width="22px" OnClick="btnRemoveIndi_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnRemaoveAll" runat="server" Text="<<" Width="22px" OnClick="btnRemaoveAll_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center" valign="top">
                            <asp:ListBox ID="lbxInclude" runat="server" Height="150px" Width="400px"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblNoOfItemsChoice" runat="server" Text="[No. of item(s)]"></asp:Label>
                        </td>
                        <td>
                        
                        </td>
                        <td>
                            &nbsp;
                            <asp:Label ID="lblNoOfItemsInclude" runat="server" Text="[No. of item(s)]"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Button ID="btnEndorse" runat="server" Text="ENDORSE TO CHECKER 1" OnClick="btnEndorse_Click" OnClientClick="return confirm('Endorse transaction(s)?');" />
                            <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="106px" OnClick="btnClear_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlReview" runat="server" Width="890px" GroupingText="For Review" Visible="False">
                <table width="100%">
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="lblErrorInfo" runat="server" Text="[Validate]" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlReviewGrid" runat="server" Width="875px" Height="180px" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                                <asp:GridView ID="dgvReview" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="858px">
                                    <RowStyle BackColor="#F7F7DE" />
                                    <FooterStyle BackColor="#CCCC99" />
                                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnDisregard" runat="server" Text="DISREGARD LIST AND CONTINUE" Width="269px" OnClick="btnDisregard_Click" />
                        </td>
                        <td align="center">
                            <asp:Button ID="btnKeep" runat="server" Text="KEEP LIST AND CONTINUE" Width="260px" OnClick="btnKeep_Click" />
                        </td>
                        <td align="right">
                            <asp:Button ID="btnCancel" runat="server" Text="CANCEL TRANSACTIONS" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevOTDate" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfDayCode" runat="server"/>
<asp:HiddenField ID="hfSTRTOTFRAC" runat="server"/>
<asp:HiddenField ID="hfOTFRACTION" runat="server"/>
<asp:HiddenField ID="hfOTSTARTPAD" runat="server"/>
<asp:HiddenField ID="hfddlChange" runat="server"/>
</asp:Content>

