<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pgeLeaveBatch.aspx.cs" Inherits="Transactions_Leave_pgeLeaveBatch"  Title="Batch Leave Entry" MasterPageFile="~/pgeMaster.master"%>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

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
    <table cellpadding="0" cellspacing="0" style="width: 894px" onmouseover="page_ClientLoad()">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0" style="width: 893px">
                <tr>
                    <td valign="top">
                        <asp:Table ID="tblEntryLeft" runat="server" Width="371px">
                            <asp:TableRow ID="TableRow1" runat="server">
                                <asp:TableCell ID="TableCell1" runat="server">
                                    <asp:Label ID="lblLVDate" runat="server" Text="Leave Date"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell2" runat="server">
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
                            <asp:TableRow ID="TableRow3" runat="server">
                                <asp:TableCell ID="TableCell5" runat="server">
                                    <asp:Label ID="lblShift" runat="server" Text="Shift"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell6" runat="server">
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
                                            </td>
                                        </tr>
                                    </table>
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
                        <asp:DropDownList ID="ddlCategory" EnableViewState="true" runat="server"  Width="250px" OnSelectedIndexChanged = "ddlCategory_SelectedIndexChanged" AutoPostBack = "true">
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow4" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="lblStart" runat="server" Text="Start Time"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
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
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Label ID="lblLVEndTime" runat="server" Text="End Time"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:TextBox ID="txtLVEndTime" runat="server" CssClass="textboxNumber" MaxLength="5"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtLVEndTime" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow6" runat="server" VerticalAlign="top">
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server" ColumnSpan="2">
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                        
                    </asp:TableCell>
                </asp:TableRow>
                 
            </asp:Table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table>
                            <tr>
                                <td style="width: 75px">
                                    <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter"></asp:Label>
                                </td>
                                <td>
                                    <%--<asp:DropDownList ID="ddlCostcenter" runat="server" Width="787px" AutoPostBack="True" OnSelectedIndexChanged="ddlCostcenter_SelectedIndexChanged">
                                    </asp:DropDownList>--%>
                                    <dx:ASPxComboBox ID="ddlCostcenter" runat="server" Width="787px" 
                                        DropDownStyle="DropDownList" IncrementalFilteringMode="StartsWith"
                                        onvaluechanged="ddlCostcenter_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
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
                <table width="100%">
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
                        <td colspan="4">
                            <asp:Button ID="btnEndorse" runat="server" Text="SAVE AND ENDORSE" OnClick="btnEndorse_Click" OnClientClick="return confirm('Endorse transaction(s)?'); " />
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
                                <asp:GridView ID="dgvReview" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="858px">
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
                        <asp:Button ID="Button1" runat="server" Text="CLEAR" Width="106px" OnClick="btnClear_Click" />                        
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevLVDate" runat="server"/>
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
<asp:HiddenField ID="hfLVHRENTRY" runat="server"/>
    
<asp:HiddenField ID="hfCHIYODA" runat="server"/>
<asp:HiddenField ID="txtStatus" runat="server"/>

</asp:Content>