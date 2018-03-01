<%@ Page Title="" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeShiftBatchUpdateRange.aspx.cs" Inherits="Transactions_WorkInformation_pgeShiftBatchUpdateRange" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table cellpadding="0" cellspacing="0" style="width: 894px">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" style="width: 893px">
                <tr>
                    <td valign="top">
                        <asp:Table ID="tblEntryLeft" runat="server" Width="450px" >
                            <asp:TableRow ID="tbrEffectivityDate" runat="server">
                                <asp:TableCell ID="TableCell1" runat="server">
                                    <asp:Label ID="lblEffectivityDate" runat="server" Text="Date From" Width="82px"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell2" runat="server">
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
                            <asp:TableRow ID="TableRow1" runat="server">
                                <asp:TableCell ID="TableCell7" runat="server">
                                    <asp:Label ID="Label2" runat="server" Text="Date To" Width="82px"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell8" runat="server">
                                    <cc1:GMDatePicker ID="dtpEffectivityDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                            <asp:TableRow ID="tbrFromShift" runat="server" Visible="true">
                                <asp:TableCell ID="TableCell3" runat="server">
                                    <asp:Label ID="lblFromShift" runat="server" Text="From Shift"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell4" runat="server">
                                    <asp:TextBox ID="txtFromShiftCode" runat="server" Width="40px" BackColor="Gainsboro" OnTextChanged="txtFromShiftCode_TextChanged" AutoPostBack="True" />
                                    <asp:Button ID="btnFromShift" runat="server" Text="..." Width="22px" />
                                    <asp:TextBox ID="txtFromShiftDesc" runat="server" Width="280px" BackColor="Gainsboro">
                                    </asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow ID="tbrToShift" runat="server">
                                <asp:TableCell ID="TableCell5" runat="server">
                                    <asp:Label ID="lblToShift" runat="server" Text="To Shift"></asp:Label>
                                </asp:TableCell><asp:TableCell ID="TableCell6" runat="server">
                                    <asp:TextBox ID="txtToShiftCode" runat="server" Width="40px" BackColor="Gainsboro"/>
                                    <asp:Button ID="btnToShift" runat="server" Text="..." Width="22px" />
                                    <asp:TextBox ID="txtToShiftDesc" runat="server" Width="280px" BackColor="Gainsboro">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtToShiftCode"></asp:RequiredFieldValidator>
                                </asp:TableCell></asp:TableRow></asp:Table></td><td valign="top">
                        <asp:Table ID="tblENtryRight" runat="server">
                            <asp:TableRow ID="tbrReason">
                                <asp:TableCell VerticalAlign="top">
                                    <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                                </asp:TableCell><asp:TableCell>
                                    <asp:TextBox ID="txtReason" runat="server" Width="350px" TextMode="MultiLine" Height="49px" MaxLength="200"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtReason"></asp:RequiredFieldValidator>
                                </asp:TableCell></asp:TableRow></asp:Table></td></tr><tr>
                    <td colspan="2">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Costcenter" Width="87px"></asp:Label></td><td>
      <%--                              <asp:DropDownList 
                                        ID="ddlCostcenter" runat="server" Width="785px" AutoPostBack="True" 
                                        onselectedindexchanged="ddlCostcenter_SelectedIndexChanged"></asp:DropDownList>--%>
                                    <dx:ASPxComboBox ID="ddlCostcenter" runat="server" Width="787px" 
                                        DropDownStyle="DropDownList" IncrementalFilteringMode="StartsWith"
                                        onvaluechanged="ddlCostcenter_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
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
                            <asp:Button ID="btnSave" runat="server" Text="SAVE" Width="106px" CausesValidation="True" UseSubmitBehavior="False" OnClick="btnSave_Click" />
                            <asp:Button ID="btnEndorse" runat="server" Text="ENDORSE TO CHECKER 1" OnClick="btnEndorse_Click" OnClientClick="return confirm('Endorse transaction(s)?');" />
                            <asp:Button ID="btnClear" runat="server" Text="CANCEL" Width="106px" OnClick="btnClear_Click" CausesValidation="False" UseSubmitBehavior="False" />
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
<asp:HiddenField ID="hfPrevEffectivityDateTo" runat="server"/>
<asp:HiddenField ID="hfSaveOrEndorse" runat="server"/>
<asp:HiddenField ID="hfPrevReason" runat="server"/>
<asp:HiddenField ID="hfPrevEffectivityDate" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfPrevShift" runat="server"/>
<asp:HiddenField ID="hfBatch" runat="server"/>
</asp:Content>

