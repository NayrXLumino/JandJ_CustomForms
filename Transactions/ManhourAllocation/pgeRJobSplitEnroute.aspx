<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeRJobSplitEnroute.aspx.cs" Inherits="Default3" Title="Enroute Manhour Allocation Report" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="_collapseDiv.js"></script> 
    <script type="text/javascript" src="Javascript/inFrame.js">


</script> 
        <div class="bodyContent">
                                        <div class="dhtmlgoodies_question" style="width: 898px;">
                                            Filter
                                        </div>
                                        <div class="dhtmlgoodies_answer" style="width: 898px; height: 1px; left: 0px; top: 0px;">
	                                        <div>
	                                            <table>
	                                                <tr>
	                                                    <td style="height: 73px">
                                                            <asp:Label ID="lblStatus" runat="server" Text="Status" Width="90px"></asp:Label>
	                                                    </td>
	                                                    <td style="height: 73px">
                                                            <asp:DropDownList ID="ddlStatus" runat="server" Width="304px">
                                                                <asp:ListItem>ALL</asp:ListItem>
                                                                <asp:ListItem Value="3">ENDORSED TO CHECKER 1</asp:ListItem>
                                                                <asp:ListItem Value="5">ENDORSED TO CHECKER 2</asp:ListItem>
                                                                <asp:ListItem Value="7">ENDORSED TO APPROVER</asp:ListItem>
                                                            </asp:DropDownList>
	                                                    </td>
	                                                </tr>
	                                                <tr>
	                                                    <td colspan="2" style="height: 125px" valign="bottom">
                                                            <asp:Panel ID="pnlDate" runat="server" Height="80px" Width="390px" GroupingText="Manhour Date">
                                                                <table cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="Label1" runat="server" Text="From" Width="35px"></asp:Label>    
                                                                        </td>
                                                                        <td>
                                                                            <cc1:GMDatePicker ID="dtpDateFrom" ZIndex="100" runat="server" 
                                                                                DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" 
                                                                                ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" 
                                                                                Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" 
                                                                                TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" 
                                                                                MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" 
                                                                                TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" 
                                                                                ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-150px" 
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
                                                                       
                                                                        <td align="center">
                                                                            <asp:Label ID="lbl2" runat="server" Text="To" Width="34px"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <cc1:GMDatePicker ID="dtpDateTo" ZIndex="100" runat="server" 
                                                                                DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" 
                                                                                ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" 
                                                                                Width="120px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" 
                                                                                TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" 
                                                                                MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" 
                                                                                TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" 
                                                                                ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-150px" 
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
                                             </div>
                                        </div>
            
                                                 
         <div>
            <table width="100%">
                <tr>
                    <td style="width: 52px; height: 20px;" align="right">
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate" Height="20px" 
                            Width="100px" OnClick="GenerateButton_Click" /></td>
                    <td style="width: 76px; height: 20px;" align="left">
                        <asp:Button ID="btnClear" runat="server" Text="Clear" Height="20px" 
                            Width="100px" OnClick="ClearButton_Click" />
                    </td>
                    <td style="width: 74px; height: 20px;">
                        <asp:Button ID="btnExport" runat="server" Text="Export" Height="20px" 
                            Width="100px" OnClick="btnExport_Click" />
                    </td>
                    <td style="height: 20px">
                        <asp:Button ID="btnPrint" runat="server" Text="Print" Height="20px" 
                            Width="100px" OnClick="btnPrint_Click" />
                    </td>
                </tr>
            </table>
            <table>
                <tr valign="bottom">
                    <td><asp:Button ID="btnPrev" runat="server" Text="Previous" Height="20px" Width="78px" OnClick="NextPrevButton" /></td>
                        <td> <asp:Label ID="lblRows" runat="server" Text=" - of    Row(s)"></asp:Label> </td>
                    <td><asp:Button ID="btnNext" runat="server" Text="Next" Height="20px" Width="78px" OnClick="NextPrevButton" /></td>
                </tr>
            </table>
         </div>   
                        <asp:Panel ID="Panel8" runat="server" Height="400px" Width="898px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                                    <asp:GridView ID="grdView" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="2" Font-Size="12px" ForeColor="Black" GridLines="Vertical" OnRowCreated="grdView_RowCreated" CellSpacing="2" Width="800px">
                                        <FooterStyle BackColor="#CCCC99" />
                                        <RowStyle BackColor="#F7F7DE" Wrap="False" Height="15px" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" Wrap="False" />
                                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" Wrap="False" />
                                    </asp:GridView>
                        </asp:Panel>
        </div>

</asp:Content>

