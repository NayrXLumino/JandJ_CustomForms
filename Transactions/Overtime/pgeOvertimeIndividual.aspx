<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeIndividual.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeIndividual" Title="Individual Overtime Entry" EnableEventValidation="false" %>
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
<table cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2">
            <table>
                <tr>
                    <td valign="top">
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
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('OVERTIME')" UseSubmitBehavior="false" Width="22px" />
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
            <asp:Table ID="tblLeftControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrControlNo" runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblOTDate" runat="server" Text="Date of Overtime"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <cc1:GMDatePicker ID="dtpOTDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                        &nbsp;<asp:TextBox ID="txtDayCode" runat="server" BackColor="Gainsboro" Width="70px" ReadOnly="true"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblShift" runat="server" Text="Shift for the Day"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOTShift" runat="server" Width="295px" ReadOnly="true" BackColor="Gainsboro"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:DropDownList ID="ddlOTType" runat="server" Width="132px">
                        </asp:DropDownList><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlOTType" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblStartTime" runat="server" Text="Start Time"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOTStartTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqOTStartTiime" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtOTStartTime" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblOTHours" runat="server" Text="Overtime Hours"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOTHours" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqOTHours" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtOTHours" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblOTEndTime" runat="server" Text="End Time"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtOTEndTime" runat="server" BackColor="Gainsboro" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtOTEndTime" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell VerticalAlign="Top" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
        <td valign="top">
            <asp:Table ID="tblRightControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrFiller1" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller1" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller1" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller1" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller2" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller2" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller2" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller3" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller3" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller3" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrJobCode" Visible="true">
                    <asp:TableCell ColumnSpan="2" Width="130px">
                        <table cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Label ID="lblJobCode" runat="server" Text="Job Code" Width="130px"></asp:Label>    
                                </td>
                                <td>
                                    <asp:TextBox ID="txtJobCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                    <asp:Button ID="btnJobCode" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblClientJobNo" runat="server" Text="Client Job No" Width="130px"></asp:Label>    
                                </td>
                                <td>
                                    <asp:TextBox ID="txtClientJobNo" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lblClientJobName" runat="server" Text="Client Job Name" Width="130px"></asp:Label>  
                                </td>
                                <td>
                                    <asp:TextBox ID="txtClientJobName" runat="server" TextMode="MultiLine" Width="295px" BackColor="Gainsboro" Wrap="true"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Width="130px" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtRemarks" Font-Bold="true" Enabled="false"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
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
                        <asp:Button ID="btnZ" runat="server" Text="[btnZ]" Width="225px" OnClick="btnZ_Click" OnClientClick="showDiv()" />
                    </td>
                    <td>
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControls()" />
                    </td>
                    <td>
                        <asp:Button ID="btnX" runat="server" Text="[btnX]" Width="225px" OnClick="btnX_Click"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfPrevOTDate" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
     <asp:HiddenField ID="hfShiftCode" runat="server" />
     <%-- <asp:HiddenField ID="hfOTFRACTION" runat="server" />--%>
</asp:Content>

