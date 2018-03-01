<%@ Page Title="No Man-hours Report" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeNoManHour.aspx.cs" Inherits="Transactions_ManhourAllocation_pgeNoManHour"%>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2.Export" Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dx" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGridView" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGridLookup" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="_collapseDiv.js"></script>
    <script type="text/javascript">
        function CheckPopUp() {
            if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked) {
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = false;

                if (document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_1').checked == false) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked = true;
                }

            }
            else if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked == false && document.getElementById('ctl00_ContentPlaceHolder1_chkDefaultReport').checked == true) {
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = true;
            }

            if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_2').checked) {
                document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = false; //Daily / Weekly / Monthly
                document.getElementById('ctl00_ContentPlaceHolder1_rblCode_0').disabled = false; //ClientCode
                //document.getElementById('ctl00_ContentPlaceHolder1_rblCode_1').disabled = false;  //WorkCode
                if (document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').value == 'M') {
                    document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').disabled = false;
                    //For update
                    //document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').disabled = true; 
                }
                else {
                    document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').disabled = true;
                }
                checkRange('selection');
            }
            else if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_1').checked) {
                document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = false; //Daily / Weekly / Monthly
            }
            else {
                document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = false;
                document.getElementById('ctl00_ContentPlaceHolder1_rblCode_0').disabled = true;
                //document.getElementById('ctl00_ContentPlaceHolder1_rblCode_1').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_ddlBilling').disabled = true;
            }

            return true;
        }
        function CheckWorkCode() {
            if (document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked) {
                document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').disabled = false;
                document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').style.backgroundColor = "#fff";
                document.getElementById('ctl00_ContentPlaceHolder1_btnSubWork').disabled = false;
            }
            else {
                document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').disabled = true;
                document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').style.backgroundColor = "#f3f3f3";
                document.getElementById('ctl00_ContentPlaceHolder1_btnSubWork').disabled = true;
            }
        }

        function checkDefaultReport() {
            if (document.getElementById('ctl00_ContentPlaceHolder1_chkDefaultReport').checked) {
                document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked = false;

                //            document.getElementById('ctl00_ContentPlaceHolder1_rblOption').disabled = false; 

                // if no report option is set, make the employee option selected
                if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rblOption_1').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rblOption_2').checked == false) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked = true;
                }

                if (!document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = true;
                }

                if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_2').checked) {
                    document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = false;

                    //                document.getElementById('ctl00_ContentPlaceHolder1_rblCode_0').disabled = false;
                    //                document.getElementById('ctl00_ContentPlaceHolder1_rblCode_1').disabled = false;
                    radios = document.forms[0].ctl00$ContentPlaceHolder1$rblCode;
                    for (var i = 0; i < radios.length; i++) {
                        radios[i].disabled = false;
                    }
                }
            }
            else {

                document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked = true;

                //            document.getElementById('ctl00_ContentPlaceHolder1_rblOption').disabled = true;

                document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = true;

                //            document.getElementById('ctl00_ContentPlaceHolder1_rblCode_0').disabled = true;
                //            document.getElementById('ctl00_ContentPlaceHolder1_rblCode_1').disabled = true;
                radios = document.forms[0].ctl00$ContentPlaceHolder1$rblCode;
                for (var i = 0; i < radios.length; i++) {
                    radios[i].disabled = true;
                }

                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = false;

                if (document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_1').checked == false) {
                    document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked = true;
                }


            }

            CheckWorkCode();
        }
</script>
<div>
    <%--<div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>--%>
  <%--<div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">--%>
        <div>
            <asp:Table ID="tblFilter1" runat="server" Width="895px" >
                <asp:TableRow ID="tbrEmployee" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" Width="150px">
                        <asp:Label ID="lblEmployee" runat="server" Text="Employee(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <%--<dx:ASPxGridLookup ID="wrflwEmployeeID" runat="server" Width="425px" ClientInstanceName="wrflwEmployeeID"
                                                AutoGenerateColumns="False" DataSourceID="sqlPatientID" TextFormatString="{1}"
                                                KeyFieldName="Emt_EmployeeId" AutoPostBack="false" SelectionMode="Multiple">
                                                <GridViewProperties EnableCallBacks="true" Settings-ShowFilterRow="True">
                                                    <Settings ShowFilterRow="True"></Settings>
                                                </GridViewProperties>
                                                <Columns>
                                                    <dx:GridViewCommandColumn ShowSelectCheckbox="true" />
                                                    <dx:GridViewDataTextColumn FieldName="Emt_EmployeeId"  Caption="Employee ID" ReadOnly="True" ShowInCustomizationForm="True">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn FieldName="ForSort" caption="Employee Name" CellStyle-Wrap="False" ShowInCustomizationForm="True">
                                                        <CellStyle Wrap="False">
                                                        </CellStyle>
                                                    </dx:GridViewDataTextColumn>
                                                   
                                                </Columns>
                        </dx:ASPxGridLookup>
                                            <asp:SqlDataSource ID="sqlPatientID" runat="server">
                                            </asp:SqlDataSource>--%>
                                            <asp:TextBox ID="txtEmpName" runat="server" Width="300px"></asp:TextBox>
                                            
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server" Width="120px">
                         <asp:Button ID="btnEmployee" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                   <asp:TableCell ID="TableCell100" runat="server" Width="120px" RowSpan="2">
                          <asp:TableRow>
                    <asp:TableCell ColumnSpan="3">
                        <asp:Table ID="tblFilter3" runat="server" CellPadding="0" CellSpacing="0">
                            <asp:TableRow>
                                <asp:TableCell Width="400px" VerticalAlign="Top">
                                    <asp:Table ID="tblFilter31" runat="server" CellPadding="0" CellSpacing="0">
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <asp:Panel ID="pnlOvertimeDate3" runat="server" GroupingText="Date Range" Width="390px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblOTDateFrom3" runat="server" Text="From"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpOTDateFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-130px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                            <td>
                                                                <asp:Label ID="lblOTDateTo" runat="server" Text="To"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <cc1:GMDatePicker ID="dtpOTDateTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-130px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        
                                    </asp:Table>
                                </asp:TableCell>
                               
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCostcenter" runat="server">
                    <asp:TableCell ID="TableCell4" runat="server" Width="150px">
                        <asp:Label ID="lblCostcenter" runat="server" Text="Costcnter(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">
                       <%-- <dx:ASPxGridLookup ID="wrflwCostCenterID" runat="server" Width="400px" ClientInstanceName="wrflwEmployeeID"
                                                AutoGenerateColumns="False" DataSourceID="sqlCostCenterID" TextFormatString="{1}"
                                                KeyFieldName="costcenter" AutoPostBack="false" SelectionMode="Multiple">
                                                <GridViewProperties EnableCallBacks="true" Settings-ShowFilterRow="True">
                                                    <Settings ShowFilterRow="True"></Settings>
                                                </GridViewProperties>
                                                <Columns>
                                                    <dx:GridViewCommandColumn ShowSelectCheckbox="true" />
                                                    <dx:GridViewDataTextColumn FieldName="costcenter"  Caption="CostCenter ID" ReadOnly="True" ShowInCustomizationForm="True">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn FieldName="Name" caption="Cost Center Name" CellStyle-Wrap="False" ShowInCustomizationForm="True">
                                                        <CellStyle Wrap="False">
                                                        </CellStyle>
                                                    </dx:GridViewDataTextColumn>
                                                   
                                                </Columns>
                                            </dx:ASPxGridLookup>
                                            <asp:SqlDataSource ID="sqlCostCenterID" runat="server">
                                            </asp:SqlDataSource>--%>
                                            <asp:TextBox ID="txtCostCenter" runat="server" Width="300px"></asp:TextBox>
                                            
                    </asp:TableCell>
                     <asp:TableCell ID="TableCell6" runat="server" Width="120px">
                         <asp:Button ID="btnCostCenter" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller1" runat="server" Visible="false">
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:Label ID="lblFiller1" runat="server" Text="[Filler 1](s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:TextBox ID="txtFiller1" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:Button ID="btnFiller1" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller2" runat="server" Visible="false">
                    <asp:TableCell ID="TableCell19" runat="server">
                        <asp:Label ID="lblFiller2" runat="server" Text="[Filler 2](s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell20" runat="server">
                        <asp:TextBox ID="txtFiller2" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell21" runat="server">
                        <asp:Button ID="btnFiller2" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrFiller3" runat="server" Visible="false">
                    <asp:TableCell ID="TableCell22" runat="server">
                        <asp:Label ID="lblFiller3" runat="server" Text="[Filler 3](s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell23" runat="server">
                        <asp:TextBox ID="txtFiller3" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell24" runat="server">
                        <asp:Button ID="btnFiller3" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
               
            </asp:Table>
        </div>
    <%--</div>--%>
    
    <div style="width:900px">
        <table cellpadding="0" cellspacing="0" style="width:900px">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnGenerate" runat="server" Text="GENERATE" Width="100px"
                                    OnClick="btnGenerate_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="100px" UseSubmitBehavior="false" OnClick="btnClear_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnExport" runat="server" Text="EXPORT" Width="100px" UseSubmitBehavior="false" OnClick="btnExport_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnPrint" runat="server" Text="PRINT" Width="100px" UseSubmitBehavior="false" OnClick="btnPrint_Click" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            
            <tr>
                <td>
                <hr>
                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Visible="false" ScrollBars="Horizontal" Width="898px" >
                         <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
                                    DataSourceID="SqlDataSource1" Width="889px">
                                    <Columns>
                                       
                                        <dx:GridViewDataTextColumn FieldName="Employee ID" VisibleIndex="1" Caption="Employee ID" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Employee Name" VisibleIndex="2" Caption="Employee Name" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Date" VisibleIndex="3" Caption="Date" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Scm_ShiftDesc" VisibleIndex="4" CellStyle-Wrap="False" Caption="Shift">
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Hours" VisibleIndex="6" Caption="Hours" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="IN 1" VisibleIndex="7" Caption="IN 1" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="OUT 1" VisibleIndex="8" Caption="OUT 1" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="IN 2" VisibleIndex="9" Caption="IN 2" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="OUT 2" VisibleIndex="10" Caption="OUT 2" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                         <dx:GridViewDataTextColumn FieldName="Section" VisibleIndex="11" Caption="Section" CellStyle-Wrap="False" >
                                            <CellStyle Wrap="False">
                                            </CellStyle>
                                        </dx:GridViewDataTextColumn>
                                        
                                    </Columns>
                                    <SettingsPager PageSize="30">
                                    </SettingsPager>
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True" 
                                        ShowHeaderFilterButton="True" />
                                </dx:ASPxGridView>
                         <asp:SqlDataSource ID="SqlDataSource1" runat="server" />
                         <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server" 
                            GridViewID="ASPxGridView1">
                        </dx:ASPxGridViewExporter>
                        <asp:HiddenField ID="query" runat="server" Value=" " />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
    <asp:HiddenField ID="hfUserCostCenters" runat="server" />
</div>
</asp:Content>