<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeRange.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeRange" Title="Special Overtime Entry" EnableEventValidation="false" %>
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
<table id="mainTable" cellpadding="0" cellspacing="0">
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
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblOTDate" runat="server" Text="FROM Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <cc1:GMDatePicker ID="dtpOTDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker><asp:RequiredFieldValidator ID="reqFROMDate" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpOTDateFrom" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="Label1" runat="server" Text="TO Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <cc1:GMDatePicker ID="dtpOTDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                <CalendarDayStyle Font-Size="9pt" />
                                <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                <CalendarFont Names="Arial" Size="X-Small" />
                            <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                            <CalendarFooterStyle Width="150px" />
                            <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                            <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker><asp:RequiredFieldValidator ID="reqTODate" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="dtpOTDateTo" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:DropDownList ID="ddlOTType" runat="server" Width="132px">
                        </asp:DropDownList><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlOTType" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:Label ID="lblOTHours" runat="server" Text="Overtime Hours"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:TextBox ID="txtOTHours" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqOTHours" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtOTHours" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow7" runat="server">
                    <asp:TableCell ID="TableCell15" VerticalAlign="Top" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="40px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller2" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller3" Visible="false">
                    <asp:TableCell Width="130px">
                        <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell Width="200px">
                        <asp:TextBox ID="txtFiller3" runat="server" BackColor="Gainsboro"></asp:TextBox>
                        <asp:Button ID="btnFiller3" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller3" Font-Bold="true"></asp:RequiredFieldValidator>
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
            </asp:Table>
        </td>
    </tr>
     <tr>
    <td colspan = "2">
    <asp:Panel ID="pnlBound" runat="server" GroupingText="Bound" Width="875px">
     <table width="100%">
    <tr>
        <td style="padding-left: 2px">
            <asp:Button ID="btnGenerate" runat="server" Text="GENERATE OVERTIME(S)" Width="400px" OnClick="btnGenerate_Click" />
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <script language="javascript" type="text/javascript">
                var IsPostBack= '<%=IsPostBack.ToString() %>';
                window.onload = function()
                {
                    var strCook = document.cookie;
                    if(strCook.indexOf("!~")!=0)
                    {
                        var intS = strCook.indexOf("!~");
                        var intE = strCook.indexOf("~!");
                        var strPos = strCook.substring(intS+2,intE);
                        if (IsPostBack=='True')
                        {
                        document.getElementById("<%=pnlGenerated.ClientID %>").scrollTop = strPos;
                        }
                        else
                        {
                        document.cookie = "yPos=!~0~!";
                        }
                    }
                }
                function SetDivPosition(){
                    var intY = document.getElementById("<%=pnlGenerated.ClientID %>").scrollTop;
                    //document.title = intY;
                    document.cookie = "yPos=!~" + intY + "~!";
                }
        </script>
            <asp:Panel ID="pnlGenerated" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" ScrollBars="Both" Width="855px" Height="165px">
                <asp:GridView ID="dgvGenerated" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="880px" AutoGenerateColumns="False">
                    <RowStyle BackColor="#F7F7DE" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkBoxAll" runat="server" Width="15px" OnCheckedChanged="btnSelectAll_Click" AutoPostBack="true" Checked="true"/>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkBox" runat="server" Width="15px" Checked="true"/>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" Width="15px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="OT Date" HeaderText="OT Date" />
                        <asp:BoundField DataField="DoW" HeaderText="DoW" />
                        <asp:BoundField DataField="Day Code" HeaderText="Day Code" />
                        <asp:BoundField DataField="Shift Code" HeaderText="Shift Code" />
                        <asp:BoundField DataField="Shift Desc" HeaderText="Shift Desc" />
                        <asp:BoundField DataField="Start" HeaderText="Start Time" />
                        <asp:BoundField DataField="End" HeaderText="End Time" />
                        <asp:BoundField DataField="Hours" HeaderText="Hours" />
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" />
                
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btnSaveEndorse" runat="server" Text="SAVE AND ENDORSE" Width="225px" OnClick="btnSaveEndorse_Click" OnClientClick="showDiv()"/>
                    </td>
                    <td>
                        <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="225px" OnClientClick="return clearControlsLeaveRange()" OnClick="btnClear_Click" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    </table>
    </asp:Panel>
    </td>
    </tr>
     <tr>
        <td colspan = "2">
        <asp:Panel ID="pnlReview" runat="server" GroupingText="For Review" Visible="False" Width="875px">
                <table width="100%">
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="lblErrorInfo" runat="server" Text="[Validate]" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlReviewGrid" runat="server" Width="855px" Height="180px" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                                <asp:GridView ID="dgvReview" runat="server" BackColor="White" 
                                    BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                    ForeColor="Black" GridLines="Vertical" Width="858px">
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
                    <tr>
                        <td>
                            <asp:Button ID="btnClear0" runat="server" OnClick="btnClear_Click" Text="CLEAR" 
                                Width="106px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
                        <td align="right">
                            &nbsp;</td>
                    </tr>
                </table>
     </asp:Panel>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfPrevOTDateFrom" runat="server"/>
<asp:HiddenField ID="hfPrevOTDateTo" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfPrevType" runat="server"/>
<asp:HiddenField ID="hfPrevStart" runat="server"/>
<asp:HiddenField ID="hfPrevEnd" runat="server"/>
</asp:Content>

