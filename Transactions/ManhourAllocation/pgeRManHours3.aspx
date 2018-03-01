<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeRManHours3.aspx.cs" Inherits="Default2" Title="Man Hours Report 2" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" src="_collapseDiv.js"></script> 
<script type="text/javascript" src="Javascript/inFrame.js"></script> 
<script type="text/javascript">
    function CheckPopUp()
    {
        if(document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked)
        {
            document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = false;

            if (document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_1').checked == false) 
            {
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked = true;
            }

        }
        else if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked == false && document.getElementById('ctl00_ContentPlaceHolder1_chkDefaultReport').checked == true)
        {
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
    function CheckWorkCode()
    {
        if(document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked)
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').disabled = false;
            document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').style.backgroundColor = "#fff";
            document.getElementById('ctl00_ContentPlaceHolder1_btnSubWork').disabled = false;
        }
        else
        {
            document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').disabled = true;
            document.getElementById('ctl00_ContentPlaceHolder1_txtSubWorkCode').style.backgroundColor = "#f3f3f3";
            document.getElementById('ctl00_ContentPlaceHolder1_btnSubWork').disabled = true;
        }
    }

    function checkDefaultReport() 
    {
        if (document.getElementById('ctl00_ContentPlaceHolder1_chkDefaultReport').checked) 
        {
            document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked = false;

//            document.getElementById('ctl00_ContentPlaceHolder1_rblOption').disabled = false; 
            
            // commented for additional manhour report: Manhour Report Summary
//            var radios = document.forms[0].ctl00$ContentPlaceHolder1$rblOption;
//            for (var i = 0; i < radios.length; i++) 
//            {
//                radios[i].disabled = false;
//            }
           
            // if no report option is set, make the employee option selected
            if (document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rblOption_1').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rblOption_2').checked == false) 
            {
                document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked = true;
            }

            if (!document.getElementById('ctl00_ContentPlaceHolder1_rblOption_0').checked) 
            {
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
        else 
        {
            
            document.getElementById('ctl00_ContentPlaceHolder1_chkSubWorkCode').checked = true;

//            document.getElementById('ctl00_ContentPlaceHolder1_rblOption').disabled = true;

            // commented for additional manhour report: Manhour Report Summary
//            var radios = document.forms[0].ctl00$ContentPlaceHolder1$rblOption;
//            for (var i = 0; i < radios.length; i++) 
//            {
//                radios[i].disabled = true;
//            }

            document.getElementById('ctl00_ContentPlaceHolder1_ddlDateType').disabled = true;

//            document.getElementById('ctl00_ContentPlaceHolder1_rblCode_0').disabled = true;
//            document.getElementById('ctl00_ContentPlaceHolder1_rblCode_1').disabled = true;
            radios = document.forms[0].ctl00$ContentPlaceHolder1$rblCode;
            for (var i = 0; i < radios.length; i++) 
            {
                radios[i].disabled = true;
            }

            document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions').disabled = false;

            if (document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked == false &&
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_1').checked == false) 
            {
                document.getElementById('ctl00_ContentPlaceHolder1_rbEmployeeOptions_0').checked = true;
            }

            
        }

        CheckWorkCode();
    }
</script>
    <div class="bodyContent" >
        <div class="dhtmlgoodies_question" style="width: 898px;">
                                            Filter
                                        </div>
                                        <div class="dhtmlgoodies_answer" style="width: 898px; height: 466px; left: 0px; top: 0px;">
	                                        <div>
	                                            <table style="width:895px;">
	                                                <tr>
	                                                    <td colspan="2">
	                                                        <table style="width: 696px">
	                                                            <tr>
	                                                                <td>
                                                                        <asp:Label ID="Label6" runat="server" Text="CPh Job Code(s)" Width="124px"></asp:Label>
	                                                                </td>
	                                                                <td style="width: 754px;">
                                                                        <asp:TextBox ID="txtDashJobCode" runat="server" Width="710px"></asp:TextBox>
	                                                                </td>
	                                                                <td>
	                                                                    <asp:Button ID="btnDashJobCode" runat="server" Text="..." 
                                                                            UseSubmitBehavior="False" Width="22px" />
	                                                                </td>
	                                                            </tr>
	                                                           <tr>
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
	                                                            </tr> 
	                                                            <tr>
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
	                                                            </tr> 
	                                                           <tr>
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
	                                                            </tr>
	                                                           <tr style="visibility:hidden">
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
	                                                            </tr> 
	                                                           <tr>
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
	                                                            </tr>
	                                                           <tr>
	                                                                <td valign="middle" style="height: 20px">
                                                                        <asp:Label ID="Label10" runat="server" Text="Category" Width="124px"></asp:Label>
	                                                                </td>
	                                                                <td style="width: 754px; height: 20px;">
	                                                                <table style="width: 100%">
	                                                                    <tr>
	                                                                        <td style="height: 35px">
        	                                                                <asp:RadioButtonList ID="rblBillable" runat="server" RepeatDirection="Horizontal" Width="288px" Height="19px">
                                                                                <asp:ListItem Value="A">All&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</asp:ListItem>
                                                                                <asp:ListItem Value="B">Billable&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</asp:ListItem>
                                                                                <asp:ListItem Value="N">NON-BILLABLE</asp:ListItem>
                                                                            </asp:RadioButtonList>
	                                                                        </td>
	                                                                        <td style="text-align: right; padding-right: 75px;">
	                                                                        <asp:CheckBox ID="cbUnsplitted" runat="server" Height="19px" Text="Include Unsplitted "
                                                                            TextAlign="Left" Width="128px" />
                                                                            </td>
	                                                                    </tr>
	                                                                </table>
	                                                                </td>
	                                                           </tr>
	                                                           
	                                                           <tr>
	                                                                <td valign="middle" style="height: 74px">
                                                                        <asp:Label ID="Label4" runat="server" Text="Report Options" Width="124px"></asp:Label>
	                                                                </td>
	                                                                <td style="width: 754px; height: 74px;">
	                                                                <table>
                                                                         <tr>
                                                                         <td style="width: 273px; height: 46px;" valign="middle">
                                                                           <asp:RadioButtonList ID="rblOption" runat="server" RepeatDirection="Horizontal" Width="245px">
                                                                            <asp:ListItem Value="E">Employee</asp:ListItem>
                                                                            <asp:ListItem Value="D">Department</asp:ListItem>
                                                                               <asp:ListItem Value="C">Client</asp:ListItem>
                                                                        </asp:RadioButtonList></td>
                                                                        <td valign="middle" style="width: 40px; height: 46px;">
                                                                            &nbsp;<asp:Label ID="Label11" runat="server" Text="Date Type :" Width="80px"></asp:Label></td>
                                                                        <td valign="middle" style="width: 103px; height: 46px;">
                                                                            <asp:DropDownList ID="ddlDateType" runat="server" Width="85px">
                                                                                <asp:ListItem Selected="True" Value="D">Daily</asp:ListItem>
                                                                                <asp:ListItem Value="W">Weekly</asp:ListItem>
                                                                                <asp:ListItem Value="M">Monthly</asp:ListItem>
                                                                            
                                                                         </asp:DropDownList>
	                                                                        </td>
	                                                                       <td style="width: 93px; height: 46px;">
                                                                               <asp:RadioButtonList ID="rblCode" runat="server" Width="99px">
                                                                                   <asp:ListItem Value="J">Client Job No.</asp:ListItem>
                                                                               </asp:RadioButtonList>
                                                                                   <%--<asp:ListItem Value="W">Work Code</asp:ListItem>--%>
	                                                                       </td>
	                                                                        
	                                                                    </tr>
	                                                                    </table>
	                                                                </td>
	                                                           </tr>
	                                                           <tr>
	                                                                <td>
	                                                                    <asp:Label ID="lblReportType" runat="server" Text="Report Type" ></asp:Label>
	                                                                </td>
	                                                                <td colspan="4">
	                                                                    <table>
	                                                                        <tr>
	                                                                            <td>
	                                                                                <asp:RadioButtonList ID="rbEmployeeOptions" runat="server" RepeatDirection="Horizontal" Width="160px">
                                                                                        <asp:ListItem Value="S">Summary</asp:ListItem>
                                                                                        <asp:ListItem Value="D">Details</asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
	                                                           </tr>
	                                                           <tr>
	                                                            <td>
	                                                                <asp:Label ID="Label12" runat="server" Text="Billing Cycle" Width="124px"></asp:Label>
	                                                            </td>
	                                                            <td colspan="2">
	                                                                <asp:DropDownList ID="ddlBilling" runat="server" Width="550px">
	                                                                </asp:DropDownList>
	                                                            </td>
	                                                           </tr>
	                                                        </table></td>
	                                                </tr>
	                                               <tr>
	                                                    <td style="width: 300px;" valign="top">
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
	                                                    <td style="width: 335px;" valign="top">
	                                                    
	                                                        <br />
	                                                        <asp:CheckBox ID="chkDefaultReport" runat="server" Text="Default Report" 
                                                                Width="100px" Checked="True" />
	                                                    
	                                                                 
	                                                    </td>
	                                               </tr>
                                                   <tr>
                                                        <td colspan="2">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblReport" runat="server" Text="Report" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlReport" runat="server">
                                                                            <asp:ListItem Text="" Value="" />
                                                                            <asp:ListItem Text="Department Manhour Report Detailed" Value="1" />
                                                                            <asp:ListItem Text="Department Manhour Report Summary" Value="2" />
                                                                            <asp:ListItem Text="Manhour Report Summary" Value="3" />
                                                                            <asp:ListItem Text="Custom Department Summary Report" Value="4" />
                                                                            <asp:ListItem Text="Custom Daily Employee Summary Report" Value="5" />
                                                                            <asp:ListItem Text="Custom Weekly Employee Summary Report" Value="6" />
                                                                            <asp:ListItem Text="Custom Monthly Employee Summary Report" Value="7" />
                                                                            <asp:ListItem Text="Default Employee Detailed Report" Value="8" />
                                                                            <asp:ListItem Text="Default Department Report" Value="9" />
                                                                            <asp:ListItem Text="Default Daily Report" Value="10" />
                                                                            <asp:ListItem Text="Default Weekly Report" Value="11" />
                                                                            <asp:ListItem Text="Default Monthly Report" Value="12" />
                                                                            <asp:ListItem Text="Default Employee Summary Report" Value="13" />
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                   </tr>
	                                            </table>
                                            </div>
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
                        Width="100px" OnClick="btnPrint_Click"   />
                </td>   
                <td style="width: 78px">
                    <asp:Button ID="btnTwist" runat="server" Text="Twist" Height="20px" 
                        Width="100px" OnClick="btnGenerate_Click"   />
                </td>
            </tr>
        </table>
        <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px" Width="898px">
             <asp:GridView ID="grdView" runat="server" BackColor="White" 
                    BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="2" 
                    Font-Size="12px" ForeColor="Black" GridLines="Vertical" CellSpacing="2" 
                    ShowFooter="True">
                <FooterStyle BackColor="#CCCC99" />
                <RowStyle BackColor="#F7F7DE" Wrap="False" />
                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" Wrap="False" />
                <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" Wrap="False" />
            </asp:GridView>
            <%--<dx:ASPxPivotGrid ID="pivotGrid" runat="server" Width="100%" DataSourceID="SqlDataSource1">
                <Fields>
                    <%--<dx:PivotGridField Area="--%
                </Fields>
            </dx:ASPxPivotGrid>--%>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" />
        </asp:Panel>
    <asp:HiddenField ID="hWeekStart" runat="server" />
    <asp:HiddenField ID="hWeekEnd" runat="server" />
    <asp:HiddenField ID="hfTwist" runat="server" Value="true"/>
    </div>
    
</asp:Content>

