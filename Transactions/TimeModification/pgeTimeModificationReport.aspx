<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeTimeModificationReport.aspx.cs" Inherits="Transactions_TimeModification_pgeTimeModificationReport" Title="Time Modification Report" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" src="_collapseDiv.js"></script>
<div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>
    <div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:Table ID="tblFilter1" runat="server" Width="895px">
                <asp:TableRow ID="tbrEmployee" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" Width="120px">
                        <asp:Label ID="lblEmployee" runat="server" Text="Employee(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtEmployee" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Button ID="btnEmployee" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCostcenter" runat="server">
                    <asp:TableCell ID="TableCell4" runat="server" Width="120px">
                        <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:TextBox ID="txtCostcenter" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:Button ID="btnCostcenter" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCostcenterLine" runat="server">
                    <asp:TableCell ID="TableCell16" runat="server" Width="120px">
                        <asp:Label ID="Label1" runat="server" Text="Costcenter Line(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:TextBox ID="txtCostcenterLine" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:Button ID="btnCostcenterLine" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrStatus" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:TextBox ID="txtStatus" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:Button ID="btnStatus" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrPayPeriod" runat="server">
                    <asp:TableCell ID="TableCell10" runat="server">
                        <asp:Label ID="lblPayPeriod" runat="server" Text="Payroll Period(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:TextBox ID="txtPayPeriod" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:Button ID="btnPayPeriod" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrType" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="lblType" runat="server" Text="Type(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:TextBox ID="txtType" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Button ID="btnType" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="3">
                        <asp:Table ID="tblFilter2" runat="server" CellPadding="0" CellSpacing="0">
                            <asp:TableRow>
                                <asp:TableCell Width="400px" VerticalAlign="Top">
                                    <asp:Table ID="tblFilter21" runat="server" CellPadding="0" CellSpacing="0">
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlTimeModDate" runat="server" GroupingText="Adjustment Date" Width="390px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblTKDateFrom" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpTKDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblTKDateTo" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpTKDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlDateApplied" runat="server" GroupingText="Applied Date" Width="390px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAppliedFrom" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpAppliedFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAppliedTo" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpAppliedTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="Panel1" runat="server" GroupingText="Endorsed Date" Width="390px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblEndorsedFrom" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpEndorsedFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblEndorsedTo" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpEndorsedTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlOther" runat="server" GroupingText="Other Option">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblEnroute" runat="server" Text="List En-route Transaction(s)"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="cbxEnroute" runat="server" Text=""/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </asp:TableCell>
                                <asp:TableCell Width="450px" VerticalAlign="Top">
                                    <asp:Table ID="tblFilter22" runat="server" CellPadding="0" CellSpacing="0">
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlChec1ker" runat="server" Width="470px" GroupingText="Checker 1">
                                                    <table>
                                                        <tr>
                                                            <td style="width:110px">
                                                                <asp:Label ID="lblChecker1" runat="server" Text="Checker(s) 1"></asp:Label>
                                                            </td>
                                                            <td colspan="4">
                                                                <asp:TextBox ID="txtChecker1" runat="server" Width="320px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnChecker1" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblCheckDate" runat="server" Text="Checked Date"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblC1From" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpC1From" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblC1To" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpC1To" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-177px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlChecker2" runat="server" Width="470px" GroupingText="Checker 2">
                                                    <table>
                                                        <tr>
                                                            <td style="width:110px">
                                                                <asp:Label ID="lblChecker2" runat="server" Text="Checker(s) 2"></asp:Label>
                                                            </td>
                                                            <td colspan="4">
                                                                <asp:TextBox ID="txtChecker2" runat="server" Width="320px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnChecker2" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblC2" runat="server" Text="Checked Date"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblC2From" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpC2From" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblC2To" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpC2To" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-177px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlApprover" runat="server" Width="470px" GroupingText="Approver">
                                                    <table>
                                                        <tr>
                                                            <td style="width:110px">
                                                                <asp:Label ID="lblApprover" runat="server" Text="Approver(s)"></asp:Label>
                                                            </td>
                                                            <td colspan="4">
                                                                <asp:TextBox ID="txtApprover" runat="server" Width="320px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnApprover" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAPDate" runat="server" Text="Checked Date"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAPFrom" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpAPFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAPTo" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpAPTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-177px" CalendarOffsetY="-160px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Include
    </div>
    <div class="dhtmlgoodies_answer" style="width: 896px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:Panel ID="pnlInclude" runat="server" Width="895px" GroupingText="Include" Height="75px">
                <asp:CheckBoxList ID="cblInclude" runat="server" Width="884px" RepeatColumns="5">
                    <asp:ListItem Value="First Name">First Name</asp:ListItem>
                    <asp:ListItem Value="Last Name">Last Name</asp:ListItem>
                    <asp:ListItem Value="Checker 1">Checker 1</asp:ListItem>
                    <asp:ListItem Value="Checked Date 1">Checked Date 1</asp:ListItem>
                    <asp:ListItem Value="Checker 2">Checker 2</asp:ListItem>
                    <asp:ListItem Value="Checked Date 2">Checked Date 2</asp:ListItem>
                    <asp:ListItem Value="Approver">Approver</asp:ListItem>
                    <asp:ListItem Value="Approved Date">Approved Date</asp:ListItem>
                    <asp:ListItem Value="Remarks">Remarks</asp:ListItem>
                    <asp:ListItem Value="Pay Period">Pay Period</asp:ListItem>
                </asp:CheckBoxList>
            </asp:Panel>
        </div>
    </div>
    <div style="width:900px">
        <table cellpadding="0" cellspacing="0" style="width:900px">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnGenerate" runat="server" Text="GENERATE" Width="100px" OnClick="btnGenerate_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="100px" UseSubmitBehavior="false" OnClick="btnClear_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnExport" runat="server" Text="EXPORT" Width="100px" UseSubmitBehavior="false" OnClick="btnExport_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnPrint" runat="server" Text="PRINT" Width="100px" UseSubmitBehavior="false" OnClick="btnPrint_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="398px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td style="width: 72px">
                                
                            </td>
                            <td>
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                            </td>
                            <td style="width: 205px" align="center">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="350px" ScrollBars="Both" Width="898px" >
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="896px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated">
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
        </table>
    </div>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
</div>
</asp:Content>


