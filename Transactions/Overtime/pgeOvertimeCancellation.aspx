<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pgeOvertimeCancellation.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeCancellation"  MasterPageFile="~/pgeMaster.master" EnableEventValidation="false" EnableViewState="true" %>
<%@ Register assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGridView" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
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
        function showDiv(event) {
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
                            <tr >
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSearch" runat="server" Width="398px" AutoPostBack="True" 
                                                    ontextchanged="txtSearch_TextChanged"></asp:TextBox>
                                            </td>
                                            <td style="width: 72px">
                                                
                                            </td>
                                            <td style="visibility:hidden">
                                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false"  />
                                            </td >
                                            <td style="width: 205px " align="center" style="visibility:hidden">
                                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)" Visible="false"></asp:Label>
                                            </td>
                                            <td style="visibility:hidden">
                                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false"  />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="260px" ScrollBars="Both" Width="874px" >
                                        
                                         <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                                             BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                             ForeColor="Black" GridLines="Vertical" Width="858px" Font-Size="11px" 
                                             OnRowCreated="dgvResult_RowCreated" 
                                             onrowdatabound="dgvResult_RowDataBound" AllowPaging="True" 
                                             DataSourceID="overtimeDatasoucre" EnableModelValidation="True" 
                                             EnableSortingAndPagingCallbacks="True" PageSize="100" >
                                            <RowStyle BackColor="#F7F7DE" />
                                            <FooterStyle BackColor="#CCCC99" />
                                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Justify" />
                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView><%--<asp:GridView ID="dgvResult" runat="server" BackColor="White" DataSourceID=""
                                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                                            ForeColor="Black" GridLines="Vertical" Width="896px" Font-Size="11px" AllowPaging="True" 
                                            AllowSorting="True" AutoGenerateSelectButton="True" 
                                            onrowcreated="dgvResult_RowCreated">
                                            <RowStyle BackColor="#F7F7DE" />
                                            <FooterStyle BackColor="#CCCC99" />
                                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black"  VerticalAlign="Bottom" />
                                            <PagerSettings  Mode="NextPreviousFirstLast" FirstPageText="First" LastPageText="Last" NextPageText="Next" PreviousPageText="Prev" Position="TopAndBottom"/>
                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>--%>

                                       <%-- <dx:ASPxGridView ID="dgvResult" runat="server" AutoGenerateColumns="False" 
                                    DataSourceID="overtimeDatasoucre" 
                                   
                             Width="889px" KeyFieldName="Control Number" 
                                            onfocusedrowchanged="dgvResult_FocusedRowChanged">
                             
                                    <Columns>
                                       
                                        <dx:GridViewDataTextColumn FieldName="Control Number" VisibleIndex="1" Caption="Control Number" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Employee ID" VisibleIndex="2" Caption="Employee ID" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Employee Name" VisibleIndex="3" Caption="Employee Name" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Overtime Date" VisibleIndex="4" CellStyle-Wrap="False" Caption="Overtime Date">
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Applied Date" VisibleIndex="6" Caption="Applied Date" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Overtime Type" VisibleIndex="7" Caption="Overtime Type" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Start Time" VisibleIndex="8" Caption="Start Time" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="End Time" VisibleIndex="9" Caption="End Time" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Hours" VisibleIndex="10" Caption="Hours" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Reason" VisibleIndex="11" Caption="Reason" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Date Approved" VisibleIndex="12" Caption="Date Approved" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        
                                    </Columns>
                                    <SettingsPager PageSize="10">
                                    </SettingsPager>
                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" 
                                                ColumnResizeMode="Control" />
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True" 
                                        ShowHeaderFilterButton="True" />
                                </dx:ASPxGridView>--%>
                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" />
                                    </asp:Panel>
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
                            <asp:TextBox ID="txtControlNo" runat="server" Width="300px" 
                                BackColor="Gainsboro" ForeColor="Black" ReadOnly="True"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtControlNo" Font-Bold="true"></asp:RequiredFieldValidator>
                        </td>
                        <td rowspan="2">
                            <asp:Button ID="btnCancel" runat="server" Text="CANCEL" Height="76px" OnClientClick="javascript:return confirm('Are you sure you want to cancel this transaction?') ? showDiv(event) : false;" OnClick="btnCancel_Click" UseSubmitBehavior="true" />
                        </td>
                        <td rowspan="2" style="width: 195px" align="right">
                            <%--<asp:ImageButton ID="iBtnList" runat="server" AlternateText="LIST" Height="70px" Width="60px" CausesValidation="False"  Enabled="False" ImageUrl="~/Images/list.png" />--%>
                        </td>
                        <td rowspan="2" style="width: 65px" align="right">
                            <%--<asp:ImageButton ID="iBtnDetail" runat="server" AlternateText="DETAIL" Height="70px" Width="60px" CausesValidation="False"  ImageUrl="~/Images/detail.png" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="lblRemarks" runat="server" Text="Reason"></asp:Label>
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
    <asp:HiddenField ID="query" runat="server" />
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
    
    <asp:HiddenField ID="hfEmployeeId2" runat="server"/>
    <asp:HiddenField ID="hfEmployeeId" runat="server"/>
    <asp:HiddenField ID="hfOvertimeDate" runat="server"/>
    <asp:HiddenField ID="hfReason" runat="server"/>
    <asp:HiddenField ID="hfStart" runat="server"/>
    <asp:HiddenField ID="hfEnd" runat="server"/>
    <asp:SqlDataSource ID="overtimeDatasoucre" runat="server"></asp:SqlDataSource>
    <asp:HiddenField ID="hfLeaveHours" runat="server"/>
    <asp:HiddenField ID="hfDayUnit" runat="server"/>
    <asp:HiddenField ID="hfShiftCode" runat="server"/>
    
    <asp:HiddenField ID="hfVWNICKNAME" runat="server" Value="FALSE"/>
    <asp:HiddenField ID="hfDSPIDCODE" runat="server" Value="FALSE"/>
    <asp:HiddenField ID="hfDSPFULLNM" runat="server" Value="FALSE"/>
    
    
</asp:Content>

