<%@ Page Title="CostCenter Batch Update" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeCostCenterBatchUpdate.aspx.cs" Inherits="Transactions_WorkInformation_pgeCostCenterBatchUpdate" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table cellpadding="0" cellspacing="0" style="width: 894px">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" style="width: 893px">
                <tr>
                    <td valign="top">
                        <asp:Table ID="tblEntryLeft" runat="server" Width="800px" >
                            <asp:TableRow ID="tbrEffectivityDate" runat="server">
                                <asp:TableCell ID="TableCell1" runat="server">
                                    <asp:Label ID="lblEffectivityDate" runat="server" Text="Effectivity Date"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell2" runat="server" ColumnSpan="2">
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
                            <asp:TableRow ID="tbrFromCostCenter" runat="server" VerticalAlign="Top">
                                <asp:TableCell ID="TableCell3" runat="server">
                                    <asp:Label ID="lblFromCostCenter" runat="server" Text="From CostCenter"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell4" runat="server">
                                    <asp:TextBox ID="txtFromCostCenterCode" runat="server" Width="80px" BackColor="Gainsboro" OnTextChanged="txtFromCostCenterCode_TextChanged" AutoPostBack="True" />
                                    <asp:Button ID="btnFromCostCenter" runat="server" Text="..." Width="22px" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="txtFromCostCenterDesc" runat="server" Width="500px" BackColor="Gainsboro" TextMode="MultiLine">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqGroup" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtFromCostCenterCode"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tbrToCostCenter" runat="server" VerticalAlign="Top">
                                <asp:TableCell ID="TableCell5" runat="server">
                                    <asp:Label ID="lblToCostCenter" runat="server" Text="To CostCenter"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell6" runat="server">
                                    <asp:TextBox ID="txtToCostCenterCode" runat="server" Width="80px" BackColor="Gainsboro"/>
                                    <asp:Button ID="btnToCostCenter" runat="server" Text="..." Width="22px" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="txtToCostCenterDesc" runat="server" Width="500px" BackColor="Gainsboro" TextMode="MultiLine">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtToCostCenterCode"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="tbrReason">
                                <asp:TableCell VerticalAlign="top">
                                    <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ColumnSpan="2">
                                    <asp:TextBox ID="txtReason" runat="server" Width="400px" TextMode="MultiLine" Height="49px" MaxLength="200"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtReason"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
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
                            <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label></td><td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="349px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox></td><td>
                        
                        </td>
                        <td align="center">
                            <asp:Label ID="ibiInclude" runat="server" Text="Included in Change"></asp:Label></td></tr><tr>
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
                                        <asp:Button ID="btnRemoveAll" runat="server" Text="<<" Width="22px" OnClick="btnRemoveAll_Click" UseSubmitBehavior="False" />
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
                            <asp:Label ID="lblNoOfItemsChoice" runat="server" Text="[No. of item(s)]"></asp:Label></td><td>
                        
                        </td>
                        <td>
                            &nbsp; <asp:Label ID="lblNoOfItemsInclude" runat="server" Text="[No. of item(s)]"></asp:Label></td></tr><tr>
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
                            <asp:Label ID="lblErrorInfo" runat="server" Text="[Validate]" Font-Bold="True" ForeColor="Red"></asp:Label></td></tr><tr>
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
<asp:HiddenField ID="hfPrevEffectivityDate" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
</asp:Content>

