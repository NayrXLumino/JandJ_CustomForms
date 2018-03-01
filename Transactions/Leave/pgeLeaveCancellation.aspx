<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeLeaveCancellation.aspx.cs" Inherits="Transactions_Leave_pgeLeaveCancellation" Title="Approved Leave Cancellation" EnableEventValidation="false" EnableViewState="true" %>
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
        function showDiv(evt) {
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
    <table>
        <tr>
            <td>
                <asp:MultiView ID="VIEWER" runat="server" ActiveViewIndex="0">
                    <asp:View ID="LIST" runat="server">
                        <table>
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
                                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" OnClick="NextPrevButton" CausesValidation="false" />
                                            </td>
                                            <td style="width: 205px" align="center">
                                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" OnClick="NextPrevButton" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="260px" ScrollBars="Both" Width="874px" >
                                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="858px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated" OnRowDataBound="Lookup_RowDataBound">
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
                        </table>
                    </asp:View>
                    <asp:View ID="DETAIL" runat="server">
                        <table cellpadding="0" cellspacing="0" >
                            <tr>
                                <td style="width: 453px" valign="top">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblEmployeeId" runat="server" Text="Employee ID"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEmployeeId" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblEmployeeName" runat="server" Text="Name"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEmployeeName" runat="server" Width="300px" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDate" runat="server" Text="Date of Leave" Width="92px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLeaveDate" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblLeaveType" runat="server" Text="Type"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLeaveType" runat="server" Width="300px" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCategory" runat="server" Text="Category"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCategory" runat="server" Width="300px" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStart" runat="server" Text="Start Time"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtStart" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblEnd" runat="server" Text="End Time"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEnd" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblHours" runat="server" Text="Hours"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtHours" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDayUnit" runat="server" Text="Day Unit"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDayUnit" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtReason" runat="server" BackColor="Gainsboro" ReadOnly="True" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDateTimeInform" runat="server" Text="Datetime Inform"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDateTimeInform" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblEndorsedDate" runat="server" Text="Datetime Endorsed" Width="117px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEndorsedDate" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblChecker1" runat="server" Text="Checker 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker1" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="300px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCheckDate1" runat="server" Text="Checked Date 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCheckDate1" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblChecker2" runat="server" Text="Checker 2"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker2" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="300px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCheckedDate2" runat="server" Text="Checked Date 2"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCheckDate2" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblApprover" runat="server" Text="Approver"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtApprover" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="300px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblApprovedDate" runat="server" Text="Approved Date"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtApprovedDate" runat="server" BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDetailRemarks" runat="server" Text="Remarks"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDetailRemarks" runat="server" BackColor="Gainsboro" ReadOnly="True" TextMode="MultiLine" Width="300px"></asp:TextBox>
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
                        </table>
                        
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblControlNo" runat="server" Text="Control No." Width="92px"></asp:Label>
                        </td>
                        <td style="width: 433px">
                            <asp:TextBox ID="txtControlNo" runat="server" Width="300px" BackColor="Gainsboro"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtControlNo" Font-Bold="true"></asp:RequiredFieldValidator>
                        </td>
                        <td rowspan="2">
                            <asp:Button ID="btnCancel" runat="server" Text="CANCEL" Height="76px" OnClientClick="javascript:return confirm('Are you sure you want to cancel this transaction?') ? showDiv(event) : false;" OnClick="btnCancel_Click" UseSubmitBehavior="true" />
                        </td>
                        <td rowspan="2" style="width: 195px" align="right">
                            <asp:ImageButton ID="iBtnList" runat="server" AlternateText="LIST" Height="70px" Width="60px" CausesValidation="False" OnClick="iBtnListDetail_Click" Enabled="False" ImageUrl="~/Images/list.png" />
                        </td>
                        <td rowspan="2" style="width: 65px" align="right">
                            <asp:ImageButton ID="iBtnDetail" runat="server" AlternateText="DETAIL" Height="70px" Width="60px" CausesValidation="False" OnClick="iBtnListDetail_Click" ImageUrl="~/Images/detail.png" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                        </td>
                        <td style="width: 433px">
                            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" MaxLength="200" Height="44px" Width="401px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtRemarks" Font-Bold="true"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
    
    <asp:HiddenField ID="hfEmployeeId" runat="server"/>
    <asp:HiddenField ID="hfLeaveDate" runat="server"/>
    <asp:HiddenField ID="hfLeaveType" runat="server"/>
    <asp:HiddenField ID="hfStart" runat="server"/>
    <asp:HiddenField ID="hfEnd" runat="server"/>
    <asp:HiddenField ID="hfLeaveHours" runat="server"/>
    <asp:HiddenField ID="hfDayUnit" runat="server"/>
    <asp:HiddenField ID="hfShiftCode" runat="server"/>
    
    <asp:HiddenField ID="hfVWNICKNAME" runat="server" Value="FALSE"/>
    <asp:HiddenField ID="hfDSPIDCODE" runat="server" Value="FALSE"/>
    <asp:HiddenField ID="hfDSPFULLNM" runat="server" Value="FALSE"/>
    
    
</asp:Content>

