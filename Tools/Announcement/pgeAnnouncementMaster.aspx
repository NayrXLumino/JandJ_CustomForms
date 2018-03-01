<%@ Page Title="Announcement Master" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeAnnouncementMaster.aspx.cs" Inherits="Tools_Announcement_pgeAnnouncementMaster" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
    function AssignValueToControlNo(index) {
        var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
        var tbr = tbl.rows[parseInt(index, 10) + 1];
        var tbcRouteId = tbr.cells[0];

        for (var i = 1; i < tbl.rows.length; i++) {
            if (i % 2 != 0) {
                tbl.rows[i].style.backgroundColor = '#F7F7DE';
            }
            else {
                tbl.rows[i].style.backgroundColor = '#FFFFFF';
            }
        }
        tbr.style.backgroundColor = '#FF2233';
        document.getElementById('ctl00_ContentPlaceHolder1_txtControlNo').value = tbcRouteId.innerHTML;
    }

    function timeEntry(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            if (charCode != 58)
                return false;
        return true;
    }
</script>
<table width="100%">
    <tr>
        <td colspan="5">
            <asp:MultiView ID="mtvTab" runat="server" ActiveViewIndex="0">
                <asp:View ID="mtvList" runat="server">
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="300px" 
                                    ontextchanged="txtSearch_TextChanged" AutoPostBack="True"></asp:TextBox>
                            </td>
                            <td style="width: 100px">
                                <asp:DropDownList ID="ddlStatus" runat="server" 
                                    onselectedindexchanged="txtSearch_TextChanged" AutoPostBack="true">
                                    <asp:ListItem Text="ACTIVE" Value="A" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="CANCELLED" Value="C"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td align="right" style="width: 100px">
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                            </td>
                            <td align="center" style="width: 300px">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)"></asp:Label>
                            </td>
                            <td align="left" style="width: 100px">
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <asp:Panel ID="pnlResult" runat="server" ScrollBars="Auto" BorderStyle="Solid" 
                                    BorderWidth="1px" Width="890px" Height="250px">
                                    <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                                        BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                        EnableModelValidation="True" ForeColor="Black" GridLines="Vertical" 
                                        Width="882px" onrowcreated="dgvResult_RowCreated" 
                                        onrowdatabound="Lookup_RowDataBound">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                        <RowStyle BackColor="#F7F7DE" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <asp:View ID="mtvDetail" runat="server">
                    <table width="100%">
                        <tr>
                            <td style="width: 100px">
                                <asp:Label ID="Label3" runat="server" Text="Announce Date" Width="100px"></asp:Label>
                            </td>
                            <td>
                                <cc1:GMDatePicker ID="dtpAnnonceDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                            <td style="width: 650px">
                                <asp:TextBox ID="txtAnnounceTime" runat="server" Width="70px" MaxLength="5"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 110px">
                                <asp:Label ID="Label4" runat="server" Text="Announced By" AssociatedControlID="txtAnnouncedBy"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtAnnouncedBy" runat="server" MaxLength="50" Width="425px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 110px">
                                <asp:Label ID="Label5" runat="server" Text="Subject" AssociatedControlID="txtSubject"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtSubject" runat="server" MaxLength="100" Width="740px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 110px" valign="top">
                                <asp:Label ID="Label6" runat="server" Text="Information" AssociatedControlID="txtInformation"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtInformation" runat="server" Height="80px" 
                                    TextMode="MultiLine" Width="740px" CssClass="textareaNormal"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 110px">
                                <asp:Label ID="Label7" runat="server" Text="Priority" AssociatedControlID="ddlPriority"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="ddlPriority" runat="server" Height="20px" Width="130px">
                                    <asp:ListItem Text="LOW" Value="3" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="MID" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="HIGH" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 110px">
                                <asp:Label ID="Label8" runat="server" Text="Status" AssociatedControlID="ddlStatus2"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="ddlStatus2" runat="server" Width="130px">
                                    <asp:ListItem Text="ACTIVE" Value="A" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="CANCELLED" Value="C"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Panel ID="pnlProfile" runat="server" GroupingText="PROFILE ACCESS AVAILABILITY" 
                                    Width="860px">
                                    <asp:CheckBoxList ID="cbxProfiles" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" Width="840px">
                                        <asp:ListItem Selected="True" Text="ALL" Value="ALL"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:View>
            </asp:MultiView>    
        </td>
    </tr>
    <tr>
        <td colspan="5">
            <hr />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label1" runat="server" Text="Control No" Width="112px" Font-Bold="true"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtControlNo" runat="server" Width="180px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td>
            <asp:Button ID="btnX" runat="server" Text="NEW" Width="100px" Height="22px" 
                onclick="btnX_Click" />
        </td>
        <td>
            <asp:Button ID="btnY" runat="server" Text="EDIT" Width="100px" Height="22px" 
                onclick="btnY_Click" />
        </td>
        <td width="500px">
            
        </td>
    </tr>
</table>
    
<asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
<asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
<asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
</asp:Content>

