<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeEmployeeApprovalRoute.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeEmployeeApprovalRoute" Title="Employee Approval Route" EnableEventValidation="false"  %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id = "table1" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2">
            <table id = "table2" >
                <tr>
                    <td valign="top">
                        <asp:Panel ID="pnlUserInfo" runat="server">
                            <table id = "table3" >
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." 
                                            UseSubmitBehavior="false" Width="22px" />
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
            <asp:Table ID="tblMain" runat="server" Width="891px">
                <asp:TableRow ID="tbrCostCenterAssignment" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="lblCostCenterAssignment" runat="server" Text="Cost Center Assignment"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ColumnSpan="12" runat="server">
                        <asp:TextBox ID="txtCostCenterAssignment" runat="server" TextMode="MultiLine" Width="783px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableHeaderRow runat="server" HorizontalAlign="Center">
                    <asp:TableHeaderCell runat="server" Width="100px">
                        <asp:Label ID="lblTransactionType" runat="server" Text="Transaction"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell  runat="server" >
                        <asp:Label ID="Label1" runat="server" Text="Start Date"></asp:Label>
                    </asp:TableHeaderCell>
                     <asp:TableHeaderCell runat="server" >
                        <asp:Label ID="Label2" runat="server" Text="End Date"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell  runat="server" ColumnSpan = "2">
                        <asp:Label ID="lblRouteId" runat="server" Text="Route ID" Width="50px"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">
                        <asp:Label ID="lblChecker1" runat="server" Text="Checker l"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">
                        <asp:Label ID="lblChecker2" runat="server" Text="Checker 2"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">
                        <asp:Label ID="lblApprover" runat="server" Text="Approver"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">
                        <asp:Label ID="lblNotify" runat="server" Text="Notify"></asp:Label>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                
                <asp:TableRow ID="tbrOvertime" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblOvertime" runat="server" Text="Overtime"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpOTStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfOTStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpOTEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfOTEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOvertime" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnOvertime" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfOvertime" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOvertimeC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOvertimeC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOvertimeAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnOTNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrLeave" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblLeave" runat="server" Text="Leave"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpLVStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfLVStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell  runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpLVEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfLVEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtLeave" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnLeave" runat="server" Text="..." Width="22px"/>
                        <asp:HiddenField ID="hfLeave" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtLeaveC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtLeaveC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtLeaveAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnLVNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrTimeRecord" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblTimeRecord" runat="server" Text="Time Record"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell  runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTRStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTRStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell  runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTREndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTREnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTimeRecord" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnTimeRecord" runat="server" Text="..." Width="22px"/>
                        <asp:HiddenField ID="hfTimeRecord" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTimeRecordC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTimeRecordC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTimeRecordAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnTRNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrStraightWork" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblStraightWork" runat="server" Text="Straight Work"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpSWStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfSWStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpSWEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfSWEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtStraightWork" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnStraightWork" runat="server" Text="..." Width="22px"/>
                        <asp:HiddenField ID="hfStraightWork" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtStraightWorkC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtStraightWorkC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtStraightWorkAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnSWNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFlexTime" runat="server" HorizontalAlign="Center">
                    <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblFlexTime" runat="server" Text="Flex Time"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpFXStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfFXStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpFXEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfFXEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtFlexTime" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Button ID="btnFlexTime" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfFlexTime" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <asp:TextBox ID="txtFlexTimeC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:TextBox ID="txtFlexTimeC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txtFlexTimeAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnFTNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrManHour" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblManHour" runat="server" Text="Man Hour"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpMHStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfMHStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpMHEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfMHEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtManHour" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnManHour" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfManHour" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtManHourC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtManHourC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtManHourAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnJSNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrWorkInfo" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblWorkInfo" runat="server" Text="Work Info"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpWIStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfWIStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpWIEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfWIEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtWorkInfo" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnWorkInfo" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfWorkInfo" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtWorkInfoC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtWorkInfoC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtWorkInfoAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnMVNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrBeneficiary" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblBeneficiary" runat="server" Text="Beneficiary"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpBenStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfBenStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpBenEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfBenEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtBeneficiary" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnBeneficiary" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfBeneficiary" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtBeneficiaryC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtBeneficiaryC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtBeneficiaryAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnBFNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrTaxCivil" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblTaxCivil" runat="server" Text="Tax/Civil Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTCStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTCStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTCEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTCEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTaxCivil" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnTaxCivil" runat="server" Text="..." Width="22px"  />
                        <asp:HiddenField ID="hfTaxCivil" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTaxCivilC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTaxCivilC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtTaxCivilAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnTXNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrAddress" runat="server" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblAddress" runat="server" Text="Address"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpAddStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfAddStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpAddEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfAddEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtAddress" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Button ID="btnAddress" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfAddress" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtAddressC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtAddressC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtAddressAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnADNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrManPower" runat="server" HorizontalAlign="Center">
                    <asp:TableCell ID="TableCell7" runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblManPower" runat="server" Text="Man Power"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpMPStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfMPStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpMPEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfMPEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:TextBox ID="txtManPower" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:Button ID="btnManPower" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfManPower" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server">
                        <asp:TextBox ID="txtManPowerC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:TextBox ID="txtManPowerC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:TextBox ID="txtManPowerAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnMPNotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrTraining" runat="server" HorizontalAlign="Center">
                    <asp:TableCell ID="TableCell13" runat="server" HorizontalAlign="Left">
                        <asp:Label ID="lblTraining" runat="server" Text="Training"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTrainStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTrainStart" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" Width = "200px">
                        <cc1:GMDatePicker ID="dtpTrainEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="70" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                        <asp:HiddenField ID="hfTrainEnd" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:TextBox ID="txtTraining" runat="server" BackColor="Gainsboro" Width="50px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Button ID="btnTraining" runat="server" Text="..." Width="22px" />
                        <asp:HiddenField ID="hfTraining" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:TextBox ID="txtTrainingC1" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:TextBox ID="txtTrainingC2" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:TextBox ID="txtTrainingAP" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Button ID="btnTRANotify" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
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
                    <td style="width: 230px" align="right">
                        <asp:Button ID="btnX" runat="server" Text="SAVE" Width="130px" OnClick="btnX_Click"/>
                    </td>
                    <td>
                        <asp:Button ID="btnY" runat="server" Text="CANCEL" Width="130px" OnClick="btnY_Click"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfEncryptId" runat="server"/>
</asp:Content>



