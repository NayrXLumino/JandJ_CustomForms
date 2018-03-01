<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" EnableEventValidation ="false" CodeFile="pgeRManHoursDetails.aspx.cs" Inherits="_Default" Title="Manhour Detail Report" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="_collapseDiv.js"></script> 
    <script type="text/javascript" src="Javascript/inFrame.js"></script> 
    <div class="contentBody">
        <div class="dhtmlgoodies_question" style="width: 898px;">
            Filter
        </div>
        <div class="dhtmlgoodies_answer" style="width: 898px; height:200px;">
            <div>
                <table style="height:180px">
                    <tr>
                        <td colspan="3" style="width: 885px;">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 20px;width: 120px">
                                         <asp:Label ID="Label2" runat="server" Text="Employee(s)" Width="100px"></asp:Label>
                                    </td>
                                   <td style="height: 20px">
                                         <asp:TextBox ID="txtEmpName" runat="server" Width="730px" ></asp:TextBox>
                                   </td>
                                   <td style="height: 20px;padding-left: 5px">
                                        <asp:Button ID="btnEmployee" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                                   </td> 
                                </tr>
                            </table>
                        </td>
                    </tr> 
                   <tr>
                        <td colspan="3" style="width: 885px;">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 20px;width: 120px">
                                        <asp:Label ID="Label6" runat="server" Text="Cost Center(s)" Width="100px"></asp:Label>
                                    </td>
                                    <td style="height: 20px">
                                        <asp:TextBox ID="txtCostCenter" runat="server" Width="730px"></asp:TextBox>
                                    </td>
                                    <td style="height: 20px;padding-left: 5px">
                                        <asp:Button ID="btnCostCenter" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                                    </td>
                                </tr>
                            </table>
                       </td>
                     </tr> 
                     <tr>
                        <td>
                            <table  cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="width: 116px; height: 37px">
                                        <asp:Label id="Label10" runat="server" Width="100px" Text="Billing Cycle"></asp:Label>
                                    </td>
                                    <td style="height: 37px">
                                        &nbsp;<asp:DropDownList ID="ddlBilling" runat="server" Width="550px">
                                        </asp:DropDownList></td>
                                </tr>
                            </table>
                        </td>
                     </tr>
                    <tr>
                        <td valign="top" style="width: 345px">
                            <asp:Panel ID="Panel10" runat="server" GroupingText="Manhour Date" Width="430px">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label11" runat="server" Text="From"></asp:Label>
                                        </td>
                                        <td style="width: 150px">
                                            <cc1:GMDatePicker ID="dtpSplitDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-105px" InitialValueMode="Null">
                                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                <CalendarFont Names="Arial" Size="X-Small" />
                                                <CalendarFooterStyle Width="150px" />
                                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
                                                <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                                <CalendarDayHeaderStyle Width="150px" />
                                                <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                <CalendarDayStyle Font-Size="9pt" />
                                            </cc1:GMDatePicker>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label15" runat="server" Text="To"></asp:Label>
                                        </td>
                                        <td>
                                            <cc1:GMDatePicker ID="dtpSplitDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="-140px" CalendarOffsetY="-105px" InitialValueMode="Null">
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
            </div>
        </div>
         
         <div>
            <table width="100%">
                <tr>
                    <td style="width: 38px" align="right">
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate" Height="20px" Width="75px" OnClick="btnGenerate_Click" />
                    </td>
                    <td style="width: 70px" align="left">
                        <asp:Button ID="btnClear" runat="server" Text="Clear" Height="20px" Width="75px" OnClick="btnClear_Click" />
                    </td>
                    <td style="width: 76px">
                        <asp:Button ID="btnExport" runat="server" Text="Export" Height="20px" Width="75px" OnClick="btnExport_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnPrint" runat="server" Text="Print" Height="20px" Width="75px" OnClick="btnPrint_Click" />
                    </td>
                </tr>
            </table>
            <table>
                <tr valign="bottom">
                    <td>
                        <asp:Button ID="btnPrev" runat="server" Text="Previous" Height="20px" Width="78px" OnClick="NextPrevButton" />
                    </td>
                    <td>
                        <asp:Label ID="lblRows" runat="server" Text=" - of    Row(s)"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btnNext" runat="server" Text="Next" Height="20px" Width="78px" OnClick="NextPrevButton" />
                    </td>
                </tr>
            </table>
             <asp:Panel ID="Panel1" runat="server" BorderStyle="Solid" BorderWidth="1px" Height="350px"
                 ScrollBars="Auto" Width="100%">
                    <asp:GridView ID="grdView" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" 
                                    BorderWidth="1px" CellPadding="2" Font-Size="12px" ForeColor="Black" GridLines="Vertical" 
                                    OnRowDataBound="grdView_RowDataBound" OnSelectedIndexChanged="grdView_SelectedIndexChanged"
                                    OnRowCreated="grdView_RowCreated" CellSpacing="2">
                                    <FooterStyle BackColor="#CCCC99" />
                                    <RowStyle BackColor="#F7F7DE" Wrap="False" />
                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" Wrap="False" />
                                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="White" Wrap="False" />
                    </asp:GridView>
             </asp:Panel>
         </div>
        <div class="reasonArea">
            <asp:HiddenField ID="valueReturn" runat="server" />
            <asp:HiddenField ID="valueReturn2" runat="server" />
            <asp:HiddenField ID="valueReturn3" runat="server" />
            <asp:HiddenField ID="valueReturn4" runat="server" />
            <asp:HiddenField ID="valueReturn5" runat="server" />
            <asp:HiddenField ID="hiddenEmpFlag" runat="server" />
            <asp:HiddenField ID="hiddenTType" runat="server" Value="LA" />
        </div>
    </div>
    <asp:Panel ID="Panel8" runat="server" Visible="false" Height="400px" Width="100%" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px"></asp:Panel>
</asp:Content>

