<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeLeaveNotification.aspx.cs" Inherits="Transactions_Leave_pgeLeaveNotification" Title="Leave Notification" EnableEventValidation="false" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
       .loadingDiv
        {
	        width:100px;
            height:100px;
            position:relative;
            margin:200px auto 0;
            background-image:url('../../Images/loading.gif');
            background-repeat:no-repeat;
            z-index:100;
        }
    </style>
    <script type="text/javascript">
        function showDiv() {
            document.getElementById('divLoadingPanel').style.display = 'block';
            window.setTimeout(partB, 100000)
            //sleep(3000000, foobar_cont);

        }
        function partB() {
            document.getElementById('divLoadingPanel').style.display = 'none';
        }
        function sleep(millis, callback) {
            setTimeout(function ()
            { callback(); }
            , millis);
        }
        function foobar_cont() {
            //console.log("finished.");
        };
        sleep(300000, foobar_cont);
    </script>
    <div id="divLoadingPanel" style="display:none;position:fixed; top:0; left:0; width:100%; height:100%; background-color:#CCEEEE;opacity:0.5;filter:alpha(opacity=50); ">
        <div class="loadingDiv">                    
        </div> 
    </div>
<table id="mainTable" cellpadding="0" cellspacing="0" onmouseover="page_ClientLoad()">
    <tr>
        <td colspan="2">
            <table>
                <tr>
                    <td valign="top" style="width: 424px">
                        <asp:Panel ID="pnlUserInfo" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('LEAVE')" UseSubmitBehavior="false" Width="22px" />
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
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblUnit" runat="server" Text="[UNIT]"></asp:Label>
                                        <asp:Label ID="lblCheckLedger" runat="server" Text=""></asp:Label>
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:GridView ID="dgvLedger" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" Font-Size="10px" ForeColor="Black" GridLines="Vertical" Width="452px">
                                            <RowStyle BackColor="#F7F7DE" />
                                            <FooterStyle BackColor="#CCCC99" />
                                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
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
            <asp:Table ID="tblLeftControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrControlNo" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No" AssociatedControlID="txtControlNo"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtControlNo" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnControlNo" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblOTDate" runat="server" Text="Date of Leave" AssociatedControlID="dtpLVDate"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <cc1:GMDatePicker ID="dtpLVDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>&nbsp;<asp:TextBox ID="txtDayCode" runat="server" BackColor="Gainsboro" Width="70px" ReadOnly="true"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="lblShift" runat="server" Text="Shift for the Day" AssociatedControlID="txtLVShift"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txtLVShift" runat="server" Width="290px" ReadOnly="true" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblDateFiled" runat="server" Text="Date/Time Filed"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <cc1:GMDatePicker ID="dtpInformDateTime" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null" EnableTimePicker="true"> 
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
                <asp:TableRow ID="tbrFiller1" Visible="false">
                    <asp:TableCell>
                        <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1]" AssociatedControlID="txtFiller1"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtFiller1" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller1" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller1" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller2" Visible="false">
                    <asp:TableCell>
                        <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2]" AssociatedControlID="txtFiller2"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtFiller2" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller2" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller2" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller3" Visible="false">
                    <asp:TableCell>
                        <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]" AssociatedControlID="txtFiller3" ></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtFiller3" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller3" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller3" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow9">
                    <asp:TableCell>
                        <asp:Label ID="lblModeOfInformation" runat="server" Text="Inform Mode" AssociatedControlID="txtModeOfInformation"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtModeOfInformation" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnInformMode" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtModeOfInformation" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow11">
                    <asp:TableCell>
                        <asp:Label ID="lblInformant" runat="server" Text="Informant" AssociatedControlID="txtModeOfInformation"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtInformant" runat="server" Width="148px" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtInformant" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow10">
                    <asp:TableCell>
                        <asp:Label ID="lblInformantRelation" runat="server" Text="Relation" AssociatedControlID="txtRelation"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtRelation" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnInformantRelation" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtRelation" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
        <td valign="top">
            <asp:Table ID="tblRightControls" runat="server" Width="440px">
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="Top">
                        <asp:Label ID="lblType" runat="server" Text="Type" Width="100px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ColumnSpan="2">
                        <asp:DropDownList ID="ddlType" runat="server" Width="250px" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlType" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="Top">
                        <asp:Label ID="lblCategory" runat="server" Text="Category"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ColumnSpan="2">
                        <asp:DropDownList ID="ddlCategory" runat="server"  Width="250px">
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:Label ID="lblStart" runat="server" Text="Start Time" AssociatedControlID="txtLVStartTime"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:TextBox ID="txtLVStartTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqOTStartTiime" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtLVStartTime" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                    <asp:TableCell RowSpan="2">
                        <asp:Panel ID="pnlDayUnit" runat="server" Width="200px">
                            <asp:RadioButtonList ID="rblDayUnit" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" Width="200px">
                                <asp:ListItem Value="WH" >Whole Day</asp:ListItem>
                                <asp:ListItem Value="HA" >Half Day AM</asp:ListItem>
                                <asp:ListItem Value="QR" >Quarter Day</asp:ListItem>
                                <asp:ListItem Value="HP" >Half Day PM</asp:ListItem>
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow6" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="lblLVEndTime" runat="server" Text="End Time" AssociatedControlID="txtLVEndTime"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:TextBox ID="txtLVEndTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtLVEndTime" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow7" runat="server" VerticalAlign="top">
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason" AssociatedControlID="txtReason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server" ColumnSpan="2">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                 <asp:TableRow ID="TableRow8" runat="server">
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status" AssociatedControlID="txtStatus"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server" ColumnSpan="2">
                        <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="295px" ReadOnly="true"></asp:TextBox>
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
                    <td>
                        <asp:Button ID="btnZ" runat="server" Text="SAVE" Width="225px" OnClick="btnZ_Click" OnClientClick="showDiv()"/>
                    </td>
                    <td>
                        <asp:Button ID="btnY" runat="server" Text="CANCEL" Width="225px" OnClientClick="return clearControls()" OnClick="btnY_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnX" runat="server" Text="CLEAR" Width="225px" OnClick="btnX_Click"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfControlNo" runat="server"/>
<asp:HiddenField ID="hfPrevLVDate" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfShiftCode" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfLVHRENTRY" runat="server"/>
<asp:HiddenField ID="hfLHRSINDAY" runat="server" Value="8"/>
<asp:HiddenField ID="hfPrevDateCredits" runat="server"/>
<asp:HiddenField ID="hfPrevType" runat="server"/>
<asp:HiddenField ID="hfPrevShiftCode" runat="server"/>
<asp:HiddenField ID="hfPrevStart" runat="server"/>
<asp:HiddenField ID="hfPrevEnd" runat="server"/>
<asp:HiddenField ID="hfPrevDayUnit" runat="server"/>
<asp:HiddenField ID="hfFromNotice" runat="server" Value="FALSE"/>

<asp:HiddenField ID="hfCHIYODA" runat="server" Value="FALSE"/>
</asp:Content>

