<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pgeCostPerProject.aspx.cs" Inherits="Transactions_ManhourAllocation_pgeCostPerProject" MasterPageFile="~/pgeMaster.master" Title="Cost Per Project Report" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v10.2.Export, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPivotGrid.Export" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2.Export" Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dx" %>
<%--<%@ Register Assembly="DevExpress.Web.ASPxGridView.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>--%>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<%--<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>--%>
<%@ Register assembly="DevExpress.Web.ASPxEditors.v10.2" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGridView.v10.2" namespace="DevExpress.Web.ASPxGridView" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="_collapseDiv.js"></script> 
<script type="text/javascript" src="Javascript/inFrame.js"></script> 
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
    <div class="bodyContent" >
        <%--<div class="dhtmlgoodies_question" style="width: 898px;">
                                            Filter
        </div>
        <div class="dhtmlgoodies_answer" style="width: 898px; height: 466px; left: 0px; top: 0px;">--%>
	        <div>
	            <table style="width:895px;" >
	                <tr>
	                    <td colspan="2">
	                        <table style="width: 696px">
	                            <tr>
	                                <td>
                                        <asp:Label ID="Label6" runat="server" Text="Job Code(s)" Width="124px"></asp:Label>
	                                </td>
	                                <td style="width: 430px;">
                                        <asp:TextBox ID="txtDashJobCode" runat="server" Width="330px"></asp:TextBox>
                                         
	                                </td>
                                    <td> <asp:Button ID="btnDashJobCode" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
                                    </td>
	                                <td rowspan="2">
	                                   <asp:Panel ID="Panel6" runat="server" GroupingText="Date Interval" Width="390px">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text="From"></asp:Label>
                                        </td>
                                        <td>
                                            <cc1:GMDatePicker ID="dtpRangeFrom" runat="server" DateFormat="MM/dd/yyyy" 
                                                CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" 
                                                CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" 
                                                NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" 
                                                MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" 
                                                TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" 
                                                ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-150px" 
                                                InitialValueMode="Null"> 
                                                        <CalendarDayStyle Font-Size="9pt" />
			                                            <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
			                                            <CalendarOtherMonthDayStyle BackColor="WhiteSmoke" />
			                                            <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                                        <CalendarFont Names = "Arial" Size="X-Small" />
                                                    <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                                    <CalendarFooterStyle Width="150px" />
                                                    <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                                    <CalendarDayHeaderStyle Width="150px" />
		                                    </cc1:GMDatePicker>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label8" runat="server" Text="To"></asp:Label>
                                        </td>
                                        <td>
                                            <cc1:GMDatePicker ID="dtpRangeTo" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="150px" TextBoxWidth="90" CssClass="datePicker" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="WhiteSmoke" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="-150px" InitialValueMode="Null"> 
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
	                           <%--<tr style="visibility:hidden" >
	                                <td>
                                        <asp:Label ID="Label7" runat="server" Text="Client Job No(s)" Width="124px"></asp:Label>
	                                </td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtClientJobNo" runat="server" Width="710px"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnClientJobNo" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
	                                </td>
	                            </tr>--%>
	                           <%--<tr style="visibility:hidden">
	                                <td>
                                        <asp:Label ID="Label1" runat="server" Text="Client Code(s)" Width="124px"></asp:Label>
	                                </td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtClientCode" runat="server" Width="710px"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnCLientCode" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
	                                </td>
	                            </tr>
	                            <tr style="visibility:hidden">
	                                <td>
                                        <asp:Label ID="Label9" runat="server" Text="Client FWBS Code(s)" Width="124px" 
                                            Visible="False"></asp:Label>
	                                </td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtFWBSCode" runat="server" Width="710px" Visible="False"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnClientFWBS" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" Visible="False" />
	                                </td>
	                            </tr> --%>
                                <tr>
	                                <td>
                                        <asp:Label ID="Label13" runat="server" Text="Employee(s)" Width="124px"></asp:Label>
	                                </td>
	                                <td style="width: 430px">
                                        <asp:TextBox ID="txtEmpName" runat="server" Width="330px"></asp:TextBox>
                                        
	                                </td>
                                    <td>
                                    <asp:Button ID="btnEmployee" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
                                    </td>
	                               
	                            </tr>
	                            <%--<tr style="visibility:hidden">
	                                <td>
                                        <asp:Label ID="Label2" runat="server" Text="Cost Center(s)" Width="124px"></asp:Label>
	                                </td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtCostCenter" runat="server" Width="710px"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnCostCenter" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" />
	                                </td>
	                            </tr>--%>
	                           <%-- <tr style="visibility:hidden">
	                                <td>
                                        <asp:Label ID="Label3" runat="server" Text="DASH Work Code(s)" Width="124px" 
                                            Visible="False"></asp:Label>
	                                </td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtWorkCode" runat="server" Width="710px" Visible="False"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnDashWorkCode" runat="server" Text="..." 
                                            UseSubmitBehavior="False" Width="22px" Visible="False" />
	                                </td>
	                            </tr> --%>
	                           <%-- <tr>
	                                <td>
                                        &nbsp;<asp:CheckBox ID="chkSubWorkCode" runat="server" Height="19px" Text="Work Activity Code(s)"
                                            TextAlign="Left" Width="138px" Checked="True" /></td>
	                                <td style="width: 754px">
                                        <asp:TextBox ID="txtSubWorkCode" runat="server" Width="710px"></asp:TextBox>
	                                </td>
	                                <td>
                                        <asp:Button ID="btnSubWork" runat="server" Text="..." UseSubmitBehavior="False" 
                                            Width="22px" />
	                                </td>
	                            </tr>--%>
	                            
	                                                           
	                            
	                            
	                        </table></td>
	                </tr>
	                
                    
	            </table>
           <%-- </div>--%>
        </div>
        <table>
            <tr>
                <td style="width: 52px" align="right">
                    <asp:Button ID="btnGenerate" runat="server" Text="Generate" Height="20px" 
                        Width="100px" OnClick="btnGenerate_Click" />
                </td>
               <td>
                    <asp:Button ID="btnClear" runat="server" Text="Clear" Height="20px" 
                        Width="100px" OnClick="btnClear_Click"   />
                </td>
               <td>
                    <asp:Button ID="btnExport" runat="server" Text="Export" Height="20px" 
                        Width="100px" OnClick="btnExport_Click"   />
                </td>
               <td style="width: 78px">
                    <asp:Button ID="btnPrint" runat="server" Text="Print" Height="20px" 
                        Width="100px" OnClick="btnPrint_Click"  Visible="false" />
                </td>   
                <td style="width: 78px">
                    <asp:Button ID="btnTwist" runat="server" Text="Twist" Height="20px" 
                        Width="100px" OnClick="btnGenerate_Click" Visible="False"   />
                </td>
            </tr>
            <tr>
                <td>
                  
                </td>
            </tr>
        </table>
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px" Width="898px" Visible="false">
           
            
        </asp:Panel>
         <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="1px" ScrollBars="Horizontal" 
            Width="898px" Visible="false">
                        <dx:ASPxPivotGrid ID="CostPerProjectGrid" runat="server" DataSourceID="SqlDataSource1"
                             
                            Width="880">
                            <Fields>
                                <dx:PivotGridField Area="RowArea" AreaIndex="0" Caption="Job Code" FieldName="Slm_ClientJobName"
                                    ID="fieldCategoryName" />
                               <%-- <dx:PivotGridField Area="RowArea" AreaIndex="1" Caption="Employee" FieldName="Emt_EmployeeID"
                                    ID="fieldProductName" Visible="true" />--%>
                                <dx:PivotGridField Area="RowArea" AreaIndex="1" Caption="Name" FieldName="Name"
                                    ID="PivotGridField3" Visible="true" />
                                <dx:PivotGridField Area="DataArea" AreaIndex="0" Caption="Hours" FieldName="TOTAL"
                                    ID="HRS" CellFormat-FormatType="Numeric" />
                                <dx:PivotGridField Area="ColumnArea" AreaIndex="0"  FieldName="MONTH" ID="MONTHS" Caption="Month and Year" 
                                    />
                            </Fields>
                            <OptionsView ShowFilterHeaders="False" ShowHorizontalScrollBar="True" />
                        </dx:ASPxPivotGrid>
                         <asp:SqlDataSource ID="SqlDataSource1" runat="server" />
                        <br />
                    </asp:Panel>
        <dx:ASPxPivotGridExporter ID="ASPxPivotGridExporter1" runat="server" ASPxPivotGridID="CostPerProjectGrid">
        </dx:ASPxPivotGridExporter>
        <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server" 
            GridViewID="ASPxGridView1">
        </dx:ASPxGridViewExporter>
    <asp:HiddenField ID="hWeekStart" runat="server" />
    <asp:HiddenField ID="hWeekEnd" runat="server" />
    <asp:HiddenField ID="hfTwist" runat="server" Value="true"/>
    <asp:HiddenField ID="hfUserCostCenters" runat="server" />
    <asp:HiddenField ID="hfSelectedRepIndex" runat="server" />
    <asp:HiddenField ID="query" runat="server" Value=" " />
    </div>
    
    <asp:Panel runat="server" Visible="false" Width="897px">
        </td>
                                      
                                           </td>
                                      
                                                    <%--<asp:ListItem Value="W">Work Code</asp:ListItem>--%>
	                                      
	                                  
                                          
                            <%--<table width="150px">
                                <tr>
                                    <td><asp:CheckBox ID="cbIncludeMM" runat="server" Text="MM/MS" /></td>
                                    <td><asp:CheckBox ID="cbIncludeFBS" runat="server" Text="FBS" /></td>
                                </tr>
                            </table>--%>
                                   
                                           
       </asp:Panel>
</asp:Content>

