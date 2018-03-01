<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="GatePass.aspx.cs" Inherits="Transactions_CustomForms_GatePass" Title="CustomForms" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
        
        .labelClass
        {
            font-weight: bold;
            text-align: center;
            border-style:double;
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
    <%--==========================================================================================================================================--%>
    <table cellpadding="0" cellspacing="0">
    <tr>
    <td>
        <table>
        <tr>
        <td valign="top">
            <asp:Panel ID="pnlUserInfo" runat="server">
            <table>
            <tr>
                <td><asp:Label ID="lblDate" runat="server" Text="Date" Width="94px"></asp:Label></td>
                <td>
                    <cc1:GMDatePicker ID="GMDatePickerDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" CalendarWidth="150px" Width="120px" TextBoxWidth="270" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                    <asp:RequiredFieldValidator id="RequiredFieldValidatorGMDatePickerDate" ControlToValidate="GMDatePickerDate" Display="Dynamic" ErrorMessage="*Date FOUND Empty!" runat="server"/>
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmployeeId" runat="server" Text="ID No." Width="94px"></asp:Label>    </td>
                <td>
                    <asp:TextBox ID="txtEmployeeId" runat="server" Width="267.5px" BackColor="Gainsboro"></asp:TextBox>
                    <asp:Button ID="btnEmployeeId" runat="server" OnClientClick="return lookupEmployee('PERSONNEL')" Text="..." UseSubmitBehavior="false" Width="22px" />
                    <asp:RequiredFieldValidator id="RequiredFieldValidatorbtnEmployeeId" ControlToValidate="txtEmployeeId" Display="Dynamic" ErrorMessage="*ID Number FOUND Empty!" runat="server"/>
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmployeeName" runat="server" Text="Name"></asp:Label></td>
                <td colspan="2">
                    <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblNickName" runat="server" Text="Costcenter" Width="94px"></asp:Label>    </td>
                <td colspan="2">
                    <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                </td>       
            </tr>
            <tr>
                <td colspan="2"><hr /></td>
            </tr>
                <tr>
                    <td><asp:Label ID="Label1" runat="server" Text="Type of Application" Width="94px" CssClass="labelClass"></asp:Label></td>
                </tr>
            <tr>
                <td><%--Empty Fields--%></td>
                <td>   
                    <asp:RadioButtonList id="rblTypeOfApplication" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" TextAlign="Right">
                    <asp:ListItem Text="Official Business" Value="OB"/>
                    <asp:ListItem Text="Undertime" Value="UT"/>
                    <asp:ListItem Text="Personal" Value="PL"/>
                    <asp:ListItem Text="Others" Value="OTH"/>
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidatorTypeOfApplication" Display="Dynamic" ControlToValidate="rblTypeOfApplication"  ErrorMessage="*Please Select Type of Application!"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblTypeRemarks" runat="server" Text="Remarks" Width="94px"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtTypeRemarks" runat="server" Width="290px"></asp:TextBox>
                    <asp:RequiredFieldValidator id="RequiredFieldValidatorTypeRemarks" ControlToValidate="txtTypeRemarks" Display="Dynamic" ErrorMessage="*Please provide remark for Type of Application." runat="server"/> 
                    <asp:RegularExpressionValidator id="RegularExpressionValidatorTypeRemarks" ControlToValidate="txtTypeRemarks" Display="Dynamic" ErrorMessage="*Your maximum limit is only 100 characters only." ValidationExpression="^[\s\S]{1,100}$" runat="server" ></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2"><hr /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblOthers" runat="server" Text="Others" Width="90px" CssClass="labelClass"></asp:Label></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblVehicle" runat="server" Text="Vehicle" Width="94px"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtVehicle" runat="server" Width="290px" MaxLength="100"></asp:TextBox>
                    <asp:RegularExpressionValidator id="RegularExpressionValidatortxtVehicle" ControlToValidate="txtVehicle" Display="Dynamic" ErrorMessage="*Your maximum limit is only 100 characters only." ValidationExpression="^[\s\S]{1,100}$" runat="server" ></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblAttachment" runat="server" Text="Attachment"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtAttachment" runat="server" Width="290px"></asp:TextBox>
                    <asp:RegularExpressionValidator id="RegularExpressionValidatortxtAttachment" ControlToValidate="txtAttachment" Display="Dynamic" ErrorMessage="*Your maximum limit is only 100 characters only." ValidationExpression="^[\s\S]{1,100}$" runat="server" ></asp:RegularExpressionValidator>
                </td>
            </tr>
           <tr>
                <td><asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label></td>
                <td><asp:TextBox ID="txtRemarks" runat="server" BackColor="Gainsboro" TextMode="MultiLine" Width="288px" MaxLength="200" Height="50px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label></td>
                <td><asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="290px" ReadOnly="true" Visible="True"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2"><hr /></td>
            </tr>
            <tr>
                <td colspan="2">
                <table>
                    <tr>
                        <td><asp:Button ID="btnSaveGatePass" runat="server" Text="SAVE" UseSubmitBehavior="false" Width="225px" OnClick="btnSaveGatePass_click" OnClientClick="showDiv()"/></td>
                        <td><asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="225px" OnClick="btnClear_Click" OnClientClick="return clearControlsT()"/></td>
                        <td><asp:Button ID="btnEndorseToChecker1" runat="server" Text="ENDORSE TO CHECKER 1" UseSubmitBehavior="false" Width="225px" Onclick="btnEndorseToChecker1_Click" /></td>  
                    </tr>
                    <tr>
                        <td><asp:Button ID="btnExportPDF" runat="server" Text="EXPORT PDF" UseSubmitBehavior="false" Width="225px" Onclick="btnExportPDF_Click" /></td>  
                    </tr>
                </table>
                </td>
            </tr>
            </table>
            </asp:Panel>
        </td>
        </tr>
        </table>
    </td>
    </tr>
    </table>
    <asp:HiddenField ID="hfControlNo" runat="server"/>
</asp:Content>
