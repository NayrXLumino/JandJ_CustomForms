<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pgePopEmployeeApprovalRoute.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgePopEmployeeApprovalRoute" EnableEventValidation="false" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
 <title>Employee Approval Route</title>
    <style type="text/css">
        #table2
        {
            width: 387px;
        }
        .style1
        {
            width: 207px;
        }
    </style>

 </head>
<body onunload = "refreshPage()">
<form id = "form1" runat = "server">
<table id = "table1" runat = "server" cellpadding="5">
     <tr>
        <td valign="top">
        <asp:Table ID="tblMain" runat="server" Width="300px" Font-Names="Calibri" 
                Font-Size="11pt" CellPadding="3">
        
                <asp:TableRow ID="tbrOvertime" runat="server">
                 <asp:TableCell ID="TableHeaderCell1" runat="server" Width="30px">
                        <asp:Label ID="lblTransactionType" runat="server" Text="Transaction" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell1" runat="server" style = "padding-left: 10px">
                        <asp:Label ID="lblOvertime" runat="server" Text="Overtime"></asp:Label>
                    </asp:TableCell>                    
                </asp:TableRow>

                <asp:TableRow>
                 <asp:TableCell ID="TableHeaderCell2"  runat="server"  Width="30px">
                        <asp:Label ID="Label1" runat="server" Text="Start Date" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" Width = "280px" style = "padding-left: 10px">                
                        <cc1:GMDatePicker ID="dtpOTStartDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="100" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="70px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate = "dtpOTStartDate"></asp:RequiredFieldValidator>
                        <asp:HiddenField ID="hfOTStart" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>                
                <asp:TableCell ID="TableHeaderCell3" runat="server"  Width="30px">
                        <asp:Label ID="Label2" runat="server" Text="End Date" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell3" runat="server" Width = "280px" style = "padding-left: 10px">
                        <cc1:GMDatePicker ID="dtpOTEndDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="50px" TextBoxWidth="100" NoneButtonText="X" TodayButtonStyle-Width="10px" TodayButtonStyle-Height="10px" MonthYearDropDownStyle-Width="70px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                </asp:TableRow>

                <asp:TableRow>
                
                 <asp:TableCell ID="TableHeaderCell4"  runat="server"  Width="30px">
                        <asp:Label ID="lblRouteId" runat="server" Text="Route ID" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell5" runat="server" style = "padding-left: 10px">
                <asp:TextBox ID="txtOvertime" runat="server" BackColor="Gainsboro" Width="250px"></asp:TextBox>
                        <asp:Button ID="btnOvertime" runat="server" Text="..." Width="22px" />   
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate = "txtOvertime"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                <asp:TableCell ID="TableHeaderCell5" runat="server"  Width="30px">
                        <asp:Label ID="lblChecker1" runat="server" Text="Checker 1" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" style = "padding-left: 10px">
                        <asp:TextBox ID="txtOvertimeC1" runat="server" Width="280px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                <asp:TableCell ID="TableHeaderCell6" runat="server"  Width="30px">
                        <asp:Label ID="lblChecker2" runat="server" Text="Checker 2" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell7" runat="server" style = "padding-left: 10px">
                        <asp:TextBox ID="txtOvertimeC2" runat="server" Width="280px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>    
                </asp:TableRow>

                <asp:TableRow>
                 <asp:TableCell ID="TableHeaderCell7" runat="server"  Width="30px">
                        <asp:Label ID="lblApprover" runat="server" Text="Approver" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" style = "padding-left: 10px">
                        <asp:TextBox ID="txtOvertimeAP" runat="server" Width="280px" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                 <asp:TableCell ID="TableHeaderCell8" runat="server"  Width="30px">
                        <asp:Label ID="lblNotify" runat="server" Text="Notify" Font-Bold="True"></asp:Label>
                    </asp:TableCell>
                <asp:TableCell style = "padding-left: 10px">
                       <asp:CheckBox ID="chkEndorse" runat="server" Text  = "ENDORSE">
                    </asp:CheckBox>                                        
                       <asp:CheckBox ID="chkReturn" runat="server" Text = "RETURN" style = "margin-left: 30px">
                    </asp:CheckBox>
                     </asp:TableCell>
                </asp:TableRow>
                 <asp:TableRow>
                    <asp:TableCell style = "padding-left: 10px">                    
                      
                    </asp:TableCell>
                    <asp:TableCell style = "padding-left: 10px"> 
                     <asp:CheckBox ID="chkApprove" runat="server" Text = "APPROVE">
                    </asp:CheckBox>                    
                       <asp:CheckBox ID="chkDisapprove" runat="server" Text = "DISAPPROVE" style = "margin-left: 30px">
                    </asp:CheckBox>
                    </asp:TableCell>
                     </asp:TableRow>
            </asp:Table>
        </td>
    </tr>

    <tr>
        <td>
            <table id = "table2" runat = "server">
                <tr>
                    <td align="center" class="style1">
                        <asp:Button ID="btnX" runat="server" Text="SAVE" Width="130px" 
                            Font-Names="Calibri" Font-Size="11pt" OnClick="btnX_Click"/>
                    </td>
                    <td>
                        <asp:Button ID="btnY" runat="server" Text="CLEAR" Width="130px" 
                            Font-Names="Calibri" Font-Size="11pt" OnClientClick="return clearControls()" OnClick="btnY_Click"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfState" runat="server">
            </asp:HiddenField>
<asp:HiddenField ID="hfEmployeeID" runat="server">
            </asp:HiddenField>
</form>
</body>
</html>