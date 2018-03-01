<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeBatchRange.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeBatchRange" Title="Batch Overtime Entry" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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

     <style type="text/css">
       .loadingDiv
{
	width:100px;
    height:200px;
    position:relative;
    margin:200px auto 0;
    background-image:url('../Images/loading.gif');
    background-repeat:no-repeat;
    z-index:100;
}
    </style><div id="divLoadingPanel" style="display:none;position:absolute; margin:0 auto;width:100%;height:700px;text-align:left;vertical-align:top;background-color:#CCEEEE;opacity:0.3;filter:alpha(opacity=30); ">
                        <div class="loadingDiv">  
                            </div> 
         </div>                   
                       
<table cellpadding="0" cellspacing="0" style="width: 894px">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" style="width: 893px">
                <tr>
                    <td valign="top">
                        <asp:Table ID="tblEntryLeft" runat="server" Width="371px">
                            <asp:TableRow ID="TableRow1" runat="server">
                                <asp:TableCell ID="TableCell1" runat="server">
                                    <asp:Label ID="lblOTDate" runat="server" Text="From Date"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell2" runat="server">
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
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow4" runat="server">
                                <asp:TableCell ID="TableCell13" runat="server">
                                    <asp:Label ID="Label1" runat="server" Text="To Date"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell14" runat="server">
                                    <cc1:GMDatePicker ID="dtpToOTDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                            <asp:TableRow ID="TableRow2" runat="server">
                                <asp:TableCell ID="TableCell3" runat="server">
                                    <asp:Label ID="lblGroup" runat="server" Text="Work Group"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell4" runat="server">
                                    <asp:DropDownList ID="ddlGroup" runat="server" Width="270px" OnSelectedIndexChanged="ddlGroup_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="reqGroup" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="ddlGroup"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow ID="TableRow3" runat="server" BorderWidth="1px">
                                <asp:TableCell ID="TableCell5" runat="server">
                                    <asp:Label ID="lblShift" runat="server" Text="Shift"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell6" runat="server">
                                    <asp:CheckBox ID="chbAll" runat="server" Text="All Shift"  AutoPostBack="true" OnCheckedChanged="chbAll_Click"/><br></br>
                                    <asp:DropDownList ID="ddlShift" runat="server" Width="270px" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="reqShift" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="ddlShift"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller1" Visible="false">
                                <asp:TableCell ID="TableCell7" runat="server">
                                    <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell8" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller1" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller1" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller1" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller2" Visible="false">
                                <asp:TableCell ID="TableCell9" runat="server">
                                    <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell10" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller2" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller2" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller2" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="tbrFiller3" Visible="false">
                                <asp:TableCell ID="TableCell11" runat="server">
                                    <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3]"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell12" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFiller3" runat="server" Width="173px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnFiller3" runat="server" Text="..." Width="22px" UseSubmitBehavior="false" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFiller3" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                    <td valign="top">
                        <asp:Table ID="tblENtryRight" runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:DropDownList ID="ddlType" runat="server" Width="130px">
                                    </asp:DropDownList><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlType" Font-Bold="true"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblStart" runat="server" Text="Start Time"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTStartTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqStart" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTStartTime"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblHours" runat="server" Text="Hours"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTHours" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqHours" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTHours"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="lblEnd" runat="server" Text="End Time"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtOTEndTime" runat="server" BackColor="Gainsboro" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="reqEnd" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtOTEndTime"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="top">
                                    <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txtReason" runat="server" Width="350px" TextMode="MultiLine" Height="35px"></asp:TextBox><asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" Font-Bold="True" CssClass="reqIndicator" ControlToValidate="txtReason"></asp:RequiredFieldValidator>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="width: 75px">
                                    <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter"></asp:Label>
                                </td>
                                <td>
                                    <%--<asp:DropDownList ID="ddlCostcenter" runat="server" Width="787px" AutoPostBack="True" OnSelectedIndexChanged="ddlCostcenter_SelectedIndexChanged">
                                    </asp:DropDownList>--%>
                                    <dx:aspxcombobox ID="ddlCostcenter" runat="server" Width="787px" 
                                        DropDownStyle="DropDownList" IncrementalFilteringMode="StartsWith"
                                        onvaluechanged="ddlCostcenter_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:aspxcombobox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnlBound" runat="server" Width="890px" GroupingText="Bound">
                <table width="100%" >
                    <tr>
                        <td style="width: 41px">
                            <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="349px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                        </td>
                        <td>
                        
                        </td>
                        <td align="center">
                            <asp:Label ID="ibiInclude" runat="server" Text="Included in Transaction"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top">
                            <asp:ListBox ID="lbxChoice" runat="server" Height="150px" Width="400px"></asp:ListBox>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnIncludeAll" runat="server" Text=">>" Width="22px" OnClick="btnIncludeAll_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnIncludeIndi" runat="server" Text=">" Width="22px" OnClick="btnIncludeIndi_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnRemoveIndi" runat="server" Text="<" Width="22px" OnClick="btnRemoveIndi_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnRemaoveAll" runat="server" Text="<<" Width="22px" OnClick="btnRemaoveAll_Click" UseSubmitBehavior="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center" valign="top">
                            <asp:ListBox ID="lbxInclude" runat="server" Height="150px" Width="400px"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblNoOfItemsChoice" runat="server" Text="[No. of Employee(s)]"></asp:Label>
                        </td>
                        <td>
                        
                        </td>
                        <td>
                            &nbsp;
                            <asp:Label ID="lblNoOfItemsInclude" runat="server" Text="[No. of Employee(s)]"></asp:Label>
                        </td>
                    </tr>
                    <tr>
        <td style="padding-left: 2px" colspan="10">
            <asp:Button ID="btnGenerate" runat="server" Text="GENERATE OVERTIME(S)" Width="400px" OnClick="btnGenerate_Click" />
        </td>
    </tr>
        <tr>
            <td colspan="10" align="center">
                <script language="javascript" type="text/javascript">
                    var IsPostBack = '<%=IsPostBack.ToString() %>';
                    window.onload = function () {
                        var strCook = document.cookie;
                        if (strCook.indexOf("!~") != 0) {
                            var intS = strCook.indexOf("!~");
                            var intE = strCook.indexOf("~!");
                            var strPos = strCook.substring(intS + 2, intE);
                            if (IsPostBack == 'True') {
                                document.getElementById("<%=pnlGenerated.ClientID %>").scrollTop = strPos;
                            }
                            else {
                                document.cookie = "yPos=!~0~!";
                            }
                        }
                    }
                    function SetDivPosition() {
                        var intY = document.getElementById("<%=pnlGenerated.ClientID %>").scrollTop;
                        //document.title = intY;
                        document.cookie = "yPos=!~" + intY + "~!";
                    }
            </script>
                <asp:Panel ID="pnlGenerated" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" ScrollBars="Both" Width="875px" Height="165px">
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
                            <asp:BoundField DataField="Employee ID" HeaderText="Employee ID" />
                            <asp:BoundField DataField="Employee Name" HeaderText="Employee Name" />
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
                    <tr><td colspan="2">
                            <asp:Label ID="lblNoOfTransGen" runat="server" Text="[No. of transaction(s)]"></asp:Label>
                        </td></tr>
                    <tr>
                        <td colspan="4">
                            <asp:Button ID="btnSave" runat="server" Text="SAVE AND ENDORSE" 
                                OnClick="btnSave_Click" UseSubmitBehavior="False" OnClientClick="showDiv()"/>
                            <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="106px" OnClick="btnClear_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlReview" runat="server" Width="890px" GroupingText="For Review" Visible="False">
                <table width="100%">
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="lblErrorInfo" runat="server" Text="[Validate]" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlReviewGrid" runat="server" Width="875px" Height="180px" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
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
                             
    <%--<asp:HiddenField ID="hfFiller1" runat="server"/>
    <asp:HiddenField ID="hfFiller2" runat="server"/>
    <asp:HiddenField ID="hfFiller3" runat="server"/>--%>
    <asp:HiddenField ID="hfShift" runat="server"/>
    <asp:HiddenField ID="hfOTtype" runat="server"/>
    <asp:HiddenField ID="hfStartTime" runat="server"/>
    <asp:HiddenField ID="hfHours" runat="server"/>
    <asp:HiddenField ID="hfEndTime" runat="server"/>
    <asp:HiddenField ID="hfWorkGroup" runat="server"/>
    <asp:HiddenField ID="hfPrevToOTDate" runat="server"/>
    <asp:HiddenField ID="hfPrevOTDate" runat="server"/>
    <asp:HiddenField ID="hfPrevEmployeeAdded" runat="server"/>
    <asp:HiddenField ID="hfEmployeeRemove" runat="server"/>
    <asp:HiddenField ID="hfBatch" runat="server"/>
<asp:HiddenField ID="hfSaveOrEndorse" runat="server"/>
<asp:HiddenField ID="hfShiftType" runat="server"/>
<asp:HiddenField ID="hfI1" runat="server"/>
<asp:HiddenField ID="hfO1" runat="server"/>
<asp:HiddenField ID="hfI2" runat="server"/>
<asp:HiddenField ID="hfO2" runat="server"/>
<asp:HiddenField ID="hfShiftPaid" runat="server"/>
<asp:HiddenField ID="hfShiftHours" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>
<asp:HiddenField ID="hfDayCode" runat="server"/>
<asp:HiddenField ID="hfSTRTOTFRAC" runat="server"/>
<asp:HiddenField ID="hfOTFRACTION" runat="server"/>
<asp:HiddenField ID="hfOTSTARTPAD" runat="server"/>
<asp:HiddenField ID="hfddlChange" runat="server"/>
</asp:Content>