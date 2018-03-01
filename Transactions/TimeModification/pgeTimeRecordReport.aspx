<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeTimeRecordReport.aspx.cs" Inherits="Transactions_TimeModification_pgeTimeRecordReport" Title="Time Record Entry Report" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" src="_collapseDiv.js"></script>
<div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>
    <div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:Table ID="tblFilter1" runat="server" Width="895px" Height="200px">
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
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="3" VerticalAlign="Top">
                        <table>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlTimeModDate" runat="server" GroupingText="Process Date" Width="390px">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblDateFrom" runat="server" Text="From"></asp:Label>
                                                </td>
                                                <td>
                                                    <cc1:GMDatePicker ID="dtpDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-130px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                    <asp:Label ID="lblDateTo" runat="server" Text="To"></asp:Label>
                                                </td>
                                                <td>
                                                    <cc1:GMDatePicker ID="dtpDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-130px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                </td>
                                <%--<td>
                                    <asp:Panel ID="pnlTransactionSummary" runat="server" GroupingText="Payroll Transaction Summary">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Pay Period"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPayrollPeriod" runat="server" MaxLength="7" ></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnPayrollPeriod" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>--%>
                            </tr>
                        </table>
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
                    <asp:ListItem Value="ID Number">ID Number</asp:ListItem>
                    <asp:ListItem Value="Pay Period">Pay Period</asp:ListItem>
                    <asp:ListItem Value="Last Name">Last Name</asp:ListItem>
                    <asp:ListItem Value="Middle Name">Middle Name</asp:ListItem>
                    <asp:ListItem Value="Cost Center Code">Cost Center Code</asp:ListItem>
                    <asp:ListItem Value="Cost Center Desc">Cost Center Desc</asp:ListItem>
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
            <%--<tr>
                <td>
                    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
                        Payroll Transaction Summary
                    </div>
                    <div class="dhtmlgoodies_answer" style="width: 896px; height: 1px; left: 0px; top: 0px;">
                        <div>
                            <table>
                                <tr>
                                    <td>
                                        dsdas
                                    </td>
                                    <td>
                                        erer
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>--%>
            <tr>
                <td>
                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="350px" ScrollBars="Both" Width="898px" >
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                            Width="896px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated" 
                            OnRowDataBound="dgvResult_RowDataBound" EnableModelValidation="True">
                            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                            <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
                            <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                            <RowStyle BackColor="White" ForeColor="#000000" />
                            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
             <tr><td>
                 <table><tr><td align="left" style="width:215px" valign="top">
                                    <asp:Label ID="lblComputation" runat="server" Font-Bold="true" Text="Last Computation For This Quincena : "></asp:Label>
                                </td>
                                <td align="left" style="width:150px;color:blue" >
                                    <%--<asp:ListBox ID="listBox" runat="server" AccessKey="r" CssClass="exclude" 
                                     Height="80px" SelectionMode="Single" Width="490px"></asp:ListBox>--%>
                                    <asp:Label ID="lblComputationDateValue" runat="server" Text="" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                </td></tr></table>
                 </td>

             </tr>
            
            <tr>
                <td>
                    <asp:Panel ID="pnlSumamry" runat="server" GroupingText="Summary" Width="898px">
                        <table width="100%">
                            <tr>
                                <td align="right" style="width:18%">
                                    <asp:Label ID="lblRegularHours" runat="server" Text="Regular Hour(s): "></asp:Label>
                                </td>
                                <td align="left" style="width:18%">
                                    <asp:Label ID="lblRegularHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                                <td align="right" style="width:18%">
                                    <asp:Label ID="lblPostOvertimeHours" runat="server" Text="Post Overtime Hours(s):"></asp:Label>
                                </td>
                                <td align="left" style="width:18%">
                                    <asp:Label ID="lblPostOvertimeHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                                <td align="right" style="width:18%">
                                    <asp:Label ID="lblPaidLeaveHours" runat="server" Text="Paid Leave Hour(s): "></asp:Label>
                                </td>
                                <td align="left" style="width:18%">
                                    <asp:Label ID="lblPaidLeaveHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                                
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblAbsentHours" runat="server" Text="Absent Hour(s): "></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblAbsentHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblAdvanceOvertimeHours" runat="server" Text="Advance Overtime Hours(s): "></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblAdvanceOvertimeHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblUnpaidLeaveHours" runat="server" Text="Unpaid Leave Hour(s): "></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblUnpaidLeaveHoursValue" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                           
                        </table>
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




