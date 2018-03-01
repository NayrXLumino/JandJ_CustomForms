<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="ShiftingBreak.aspx.cs" Inherits="Transactions_CustomForms_ShiftingBreak" Title="CustomForms" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--<style type="text/css">
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
    </div>--%>
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
                <td><asp:Label ID="lblDateFiled" runat="server" Text="Date Filed" Width="94px"></asp:Label></td>
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
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblOffsetPeriod" runat="server" Text="Offset Period" Width="94px"></asp:Label></td>
                <td><asp:TextBox ID="txtOffsetPeriod" runat="server" Width="290px"></asp:TextBox></td>
            </tr> 
            <tr>
                <td><asp:Label ID="lblName" runat="server" Text="Name" Width="94px"></asp:Label></td>
                <td><asp:TextBox ID="TextBox4" runat="server" Width="290px"></asp:TextBox></td>
            </tr>
             <tr>
                <td><asp:Label ID="Label5" runat="server" Text="ID No." Width="94px"></asp:Label></td>
                <td><asp:TextBox ID="TextBox5" runat="server" Width="290px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblRemarks" runat="server" Text="Reason"></asp:Label></td>
                <td><asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="288px" MaxLength="200" Height="50px"></asp:TextBox>
                   <%-- <asp:RequiredFieldValidator ID="reqReason" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>--%>
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
</asp:Content>