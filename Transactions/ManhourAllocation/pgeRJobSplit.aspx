<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeRJobSplit.aspx.cs" EnableEventValidation="false" MaintainScrollPositionOnPostback ="true" Title="Manhour Allocation Report" Inherits="_Default" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <script type="text/javascript" src="_collapseDiv.js"></script>

    <script type="text/javascript" src="Javascript/inFrame.js"></script>
<script type="text/javascript">
   function CheckPopUp()
    {
        if(document.getElementById('ctl00_ContentPlaceHolder1_chkJobSplitMod').checked)
        {
            document.getElementById('ctl00_ContentPlaceHolder1_Panel3').style.display = '';
            document.getElementById('ctl00_ContentPlaceHolder1_Panel4').style.display = '';
            document.getElementById('ctl00_ContentPlaceHolder1_Panel5').style.display = '';
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_Panel3').style.display = 'none';
            document.getElementById('ctl00_ContentPlaceHolder1_Panel4').style.display = 'none';
            document.getElementById('ctl00_ContentPlaceHolder1_Panel5').style.display = 'none';
        }
        return true;
    }

</script>
    <div class="bodyContent">
     <div class="dhtmlgoodies_question" style="width: 898px; height: 20px;">
            Filter
     </div>
    <div class="dhtmlgoodies_answer" style="width: 898px; height: 400px;">
            <div>
                <table width="898px" >
                    <tr>
                        <td colspan="3" style="width: 890px">
                            <table cellpadding="0" cellspacing="0">
                                 <tr>
                                    <td colspan="3">
                                        <table>
                                            <tr>
                                                <td style="height: 20px;width: 120px">
                                                     <asp:Label ID="Label3" runat="server" Text="Employee(s)" ></asp:Label>
                                                </td>
                                                <td style="height: 20px">
                                                    <asp:TextBox ID="txtEmpName" runat="server" Width="730px"></asp:TextBox>
                                                </td>
                                                <td style="height: 20px">
                                                   <asp:Button ID="btnEmployee" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                                                </td>
                                            </tr>
                                        </table>
                                   </td>
                                 </tr> 
                                <tr>
                                    <td colspan="3" style="height: 39px">
                                        <table>
                                            <tr>
                                                <td style="height: 20px;width:120px">
                                                    <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter" ></asp:Label>
                                                </td>
                                                <td style="height: 20px">
                                                    <asp:TextBox ID="txtCostCenter" runat="server" Width="730px"></asp:TextBox>
                                                </td>
                                                <td style="height: 20px">
                                                    <asp:Button ID="btnCostCenter" runat="server" Text="..." UseSubmitBehavior="False" Width="22px"/>
                                                </td>
                                            </tr>
                                        </table>
                                   </td>
                                 </tr>
                                <tr>
                                    <td colspan="3" style="height: 39px">
                                        <table>
                                            <tr>
                                                <td style="height: 20px;width:120px">
                                                    <asp:Label ID="Label11" runat="server" Text="Status" Width="81px"></asp:Label>
                                                </td>
                                                <td style="height: 20px">
                                                    <asp:TextBox ID="txtStatus" runat="server" Width="730px"></asp:TextBox>
                                                </td>
                                                <td style="height: 20px">
                                                    <asp:Button ID="btnStatus" runat="server" Text="..." UseSubmitBehavior="False" Width="22px"/>
                                                </td>
                                            </tr>
                                        </table>
                                   </td>
                                 </tr>
                                 <tr>
                                    <td colspan="3">
                                        <br />
                                        <asp:CheckBox ID="cbLossTime" runat="server"  
                                            Text="Unallocated Manhours " TextAlign="Left" Width="190px" />
                                        <br />
                                    </td>
                                 </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="height: 275px">
                            <table>
                                 <tr>
                                    <td style="width: 120px">
                                        <asp:Label ID="Label4" runat="server" Text="Type" Width="61px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlType" runat="server" Width="185px" >
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>

                                <tr>
                                    <td valign="top" colspan="3" style="width: 99px;">
                                        <asp:Panel ID="Panel9" runat="server" GroupingText="Payroll Period" Width="335px" Visible="False">
                                            <table>
                                            
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Label ID="Label26" runat="server" Text="Period" Width="37px" Height="20px"></asp:Label>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:TextBox ID="txtperiodcycle" runat="server" Font-Size="10px" Height="13px" Width="147px"></asp:TextBox>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Button ID="btnPayPeriod" runat="server" Text="..." Height="18px" Width="17px" />
                                                    </td>
                                                    <td valign="top">
                                                        <asp:CheckBox ID="chkAdjustment" Text="Adjustment" runat="server" Width="89px" />
                                                    </td>
                                                    </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" colspan="3" style="height: 31px">
                                        <asp:Panel ID="Panel6" runat="server" GroupingText="Date Applied" Width="390px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label5" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 140px">
                                                        <cc1:GMDatePicker ID="dtpDateAppliedFrom" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-125px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label8" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <cc1:GMDatePicker ID="dtpDateAppliedTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial"
                                                            ShowNoneButton="false" InitialText="Select a Date" CalendarWidth="150px" Width="150px"
                                                            TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px"
                                                            TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small"
                                                            TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False"
                                                            CalendarOffsetX="0px" CalendarOffsetY="-125px" InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" colspan="3" style="height: 31px">
                                        <asp:Panel ID="Panel1" runat="server" GroupingText="Manhour Date" Width="390px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label9" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 140px">
                                                        <cc1:GMDatePicker ID="dtpJobSplitDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial"
                                                            ShowNoneButton="false" InitialText="Select a Date" CalendarWidth="150px" Width="150px"
                                                            TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px"
                                                            TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small"
                                                            TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False"
                                                            CalendarOffsetX="0px" CalendarOffsetY="-125px" InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label10" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <cc1:GMDatePicker ID="dtpJobSplitDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial"
                                                            ShowNoneButton="false" InitialText="Select a Date" CalendarWidth="150px" Width="150px"
                                                            TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px"
                                                            TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small"
                                                            TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False"
                                                            CalendarOffsetX="0px" CalendarOffsetY="-125px" InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" colspan="3" style="height: 31px">
                                        <asp:Panel ID="Panel7" runat="server" GroupingText="Date Endorsed To Checker 1" Width="390px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label19" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 140px">
                                                        <cc1:GMDatePicker ID="dtpDateEndorsedFrom" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-160px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label20" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <cc1:GMDatePicker ID="dtpDateEndorsedTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial"
                                                            ShowNoneButton="false" InitialText="Select a Date" CalendarWidth="150px" Width="150px"
                                                            TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px"
                                                            TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small"
                                                            TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False"
                                                            CalendarOffsetX="0px" CalendarOffsetY="-160px" InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" style="width: 488px; height: 275px;" >
                            <table>
                            <tr><td><asp:CheckBox ID="chkJobSplitMod" runat="server" 
                                    Text="Manhour Modification" TextAlign="Left" Width="142px" />
                                <asp:CheckBox ID="chkSubWorkCode" runat="server" Text="Work Activity Code" 
                                    TextAlign="Left" Width="161px" /></td></tr>
                            <tr>
                                <td style="width: 472px">
                                        <asp:Panel ID="Panel3" runat="server" GroupingText="Checker 1" Width="470px">
                                            <table style="width: 408px">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblChecker1" runat="server" Text="Checker(s) 1"></asp:Label>
                                                    </td>
                                                    <td style="width: 112px" colspan="4">
                                                        <asp:TextBox ID="txtChecker1" runat="server" Width="320px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnChecker1" runat="server" Text="..." Font-Size="10px" UseSubmitBehavior="False" Width="22px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblCheckedDate1" runat="server" Text="Checked Date:" Width="95px"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblCheckedDate1From" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpCheckedDate1From" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-60px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label1" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpCheckedDate1To" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-160px" CalendarOffsetY="-60px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                 </td>
                            </tr>
                             <tr>
                                <td style="width: 472px">
                                        <asp:Panel ID="Panel4" runat="server" GroupingText="Checker 2" Width="470px">
                                            <table style="width: 408px">
                                                <tr>
                                                    <td style="height: 26px;">
                                                        <asp:Label ID="lblChecker2" runat="server" Text="Checker(s) 2"></asp:Label>
                                                    </td>
                                                    <td style="width: 112px; height: 26px;" colspan="4">
                                                        <asp:TextBox ID="txtChecker2" runat="server" Width="320px" ></asp:TextBox>
                                                    </td>
                                                    <td style="height: 26px">
                                                        <asp:Button ID="btnChecker2" runat="server" Text="..." Font-Size="10px" UseSubmitBehavior="False" Width="22px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label12" runat="server" Text="Checked Date:" Width="95px"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label13" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpCheckedDate2From" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-110px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label14" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpCheckedDate2To" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-160px" CalendarOffsetY="-110px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                 </td>
                            </tr>
                              <tr>
                                <td style="width: 472px">
                                        <asp:Panel ID="Panel5" runat="server" GroupingText="Approver" Width="470px">
                                            <table style="width: 408px">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblApprover" runat="server" Text="Approver(s)"></asp:Label>
                                                    </td>
                                                    <td style="width: 112px" colspan="4">
                                                        <asp:TextBox ID="txtApprover" runat="server" Width="320px" ></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnApprover" runat="server" Text="..." Font-Size="10px" UseSubmitBehavior="False" Width="22px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label16" runat="server" Text="Approved Date:" Width="95px"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label17" runat="server" Text="From"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpApprovedDate1From" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" 
                                                            CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-159px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                            <CalendarFont Names="Arial" Size="X-Small" />
                                                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                            <CalendarFooterStyle Width="150px" />
                                                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                            <CalendarDayHeaderStyle Width="150px" />
                                                        </cc1:GMDatePicker>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label18" runat="server" Text="To"></asp:Label>
                                                    </td>
                                                    <td style="width: 136px">
                                                        <cc1:GMDatePicker ID="dtpApprovedDate1To" ZIndex="100" runat="server" DateFormat="MM/dd/yyyy"
                                                            CalendarFont-Names="Arial" ShowNoneButton="false" InitialText="Select a Date"
                                                            CalendarWidth="150px" Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X"
                                                            TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px"
                                                            MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke"
                                                            CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-160px" CalendarOffsetY="-159px"
                                                            InitialValueMode="Null">
                                                            <CalendarDayStyle Font-Size="9pt" />
                                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
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
                                  </td>
                            </tr>
                              </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
     <div>
            <table width="100%">
                <tr>
                    <td style="width: 92px; height: 9px;" align="right">
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate" Height="20px" Width="100px"
                            OnClick="btnGenerate_Click" /></td>
                    <td style="width: 28px; height: 9px;" align="left">
                        <asp:Button ID="btnClear" runat="server" Text="Clear" Height="20px" Width="100px"
                            OnClick="btnClear_Click" />
                    </td>
                    <td style="height: 9px;" align="left">
                        <asp:Button ID="BtnExport" runat="server" Text="Export" Height="20px" Width="100px"
                            OnClick="btnExportExcel_Click" />
                    </td>
                    <td style="width: 627px; height: 9px;" align="left">
                        <asp:Button ID="btnPrint" runat="server" Text="Print" Height="20px" Width="100px"
                            OnClick="btnPrint_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="Panel8" runat="server" Height="416px" Width="897px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
            <asp:GridView ID="grdView" runat="server" BackColor="White" 
                BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                Font-Size="12px" OnRowCreated="grdView_RowCreated" 
                OnRowDataBound="grdView_RowDataBound" ShowFooter="True" 
                EnableModelValidation="True">
                <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                <RowStyle BackColor="White" Wrap="False" ForeColor="#000000" />
                <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" 
                    Wrap="False" />
                <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
                <AlternatingRowStyle Wrap="False" />
            </asp:GridView>
        </asp:Panel>
        <asp:HiddenField id="valueReturn" runat="server"></asp:HiddenField>
        <asp:HiddenField id="hiddenEmpFlag" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="hfUserCostCenters" runat="server" />
     
    </div>

</asp:Content>

