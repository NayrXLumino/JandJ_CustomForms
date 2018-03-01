<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupChecklist.aspx.cs" Inherits="pgeLookupChecklist" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Checklist</title>
    <link href="CSS/lookup.css" rel="stylesheet" type="text/css" />
    <link href="CSS/collapseCSS.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    .loadingDiv
    {
	    width:100px;
        height:100px;
        position:relative;
        margin:200px auto 0;
        background-image:url('Images/loading.gif');
        background-repeat:no-repeat;
        z-index:100;
    }
    </style>
    <script type="text/javascript">
        function setReadOnly(id) {
            document.getElementById(id).readOnly = true;
            showDiv();
        }
        function scram2() {
            try {
                if (popupWin2 != null && popupWin2.open) {
                    try {
                        popupWin2.close();
                    }
                    catch (err) {
                        //Do nothing
                    }
                }
            }
            catch (err) {

            }
        }
        function isMaxLength(control, len) 
        {
            var ctrl = document.getElementById(control);
            if (ctrl)
                return (ctrl.value.length <= len)
            else
                return false;
        }
        window.onfocus = scram2;

        function showDiv() {
            document.getElementById('divLoadingPanel').style.display = 'block';
            window.setTimeout(partB, 300000)
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
</head>
<body>
    <div id="divLoadingPanel" style="display:none;position:fixed; top:0; left:0; width:100%; height:100%; background-color:#CCEEEE;opacity:0.5;filter:alpha(opacity=50); ">
        <div class="loadingDiv">                    
        </div> 
    </div>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlDetails" runat="server" GroupingText="Details" Width="975px">
                        <table>
                            <tr>
                                <td valign="top" style="height: 30px;">
                                    <asp:Label ID="lblCostCenter" runat="server" Text="Costcenter" Width="73px"></asp:Label>
                                </td>
                                <td valign="top" style="height: 30px; width: 700px;">
                                    <asp:DropDownList ID="ddlCostCenter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCostCenter_SelectedIndexChanged" Width="670px" OnChange="showDiv()">
                                    </asp:DropDownList>
                                </td>
                                <td valign="top" style="height: 30px;">
                                     <asp:RadioButtonList ID="rblCCOption" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblCCOption_SelectedIndexChanged" Width="185px" OnChange="showDiv()">
                                        <asp:ListItem Value="S" Selected="True">Section&amp;nbsp</asp:ListItem>
                                        <asp:ListItem Value="D">Department</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" style="height: 30px;">
                                    <asp:Label ID="lblSearch" runat="server" Text="Search" Width="60px"></asp:Label>
                                </td>
                                <td valign="top" style="height: 30px; width: 700px;">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtSearch" runat="server" 
                                                    OnTextChanged="txtSearch_TextChanged" AutoPostBack="True" Width="560px" OnChange="showDiv()"></asp:TextBox>
                                            </td>
                                            <td style="width:50px; text-align:right;">
                                                <asp:Label ID="Label1" runat="server" Text="Font:"></asp:Label>
                                            </td>
                                            <td style="width:30px; text-align:right;">
                                                <asp:Button ID="btnGrow" runat="server" Text="+" Width="22px" 
                                                    onclick="btnGrow_Click" />
                                            </td>
                                            <td style="width:30px; text-align:right;">
                                                <asp:Button ID="btnShrink" runat="server" Text="-" Width="22px" 
                                                    onclick="btnShrink_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top" style="height: 30px;">
                                    <asp:RadioButtonList ID="rblOtion" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblOtion_SelectedIndexChanged" Width="175px" OnChange="showDiv()">
                                        <asp:ListItem Selected="True" Value="A">Approve</asp:ListItem>
                                        <asp:ListItem Value="D">Disapprove</asp:ListItem>
                                        <asp:ListItem Value="R">Return</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="right">
                                <table>
                                        <tr>
                                            <td style="width: 88px" align="right">
                                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" OnClientClick="showDiv()" />
                                            </td>
                                            <td style="width: 172px" align="center">
                                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)" Font-Size="11px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" OnClientClick="showDiv()" />
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
                <td colspan="2" style="z-index:2;">
                    <div id="errorPanel" style="visibility:hidden;float:left;position:absolute;height:490px;width: 978px;z-index:2; background-color:#BBBBBB;">
                        <table width="100%">
                            <tr>
                                <td align="right">
                                    <asp:ImageButton ID="ibtnClose" runat="server" AlternateText="Close" OnClientClick="return showHideError()" Height="20px" Width="20px" ImageUrl="~/Images/close.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlError" runat="server" Width="970px" ScrollBars="Both" Height="450px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" >
                                        <asp:GridView ID="dgvError" runat="server" BackColor="White" 
                                            BorderColor="#E7E7FF" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" 
                                            EnableModelValidation="True" GridLines="Both" Width="970px" 
                                            OnRowCreated="dgvError_RowCreated" Font-Size="12pt" >
                                            <%--
                                            onselectedindexchanged="dgvError_SelectedIndexChanged--%>
                                            <AlternatingRowStyle BackColor="#F7F7F7" />
                                            <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                                            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                                            <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                                            <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                                            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="width:100%;">
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
                                    document.getElementById("<%=pnlResult.ClientID %>").scrollTop = strPos;
                                    }
                                    else
                                    {
                                    document.cookie = "yPos=!~0~!";
                                    }
                                }
                            }
                            function SetDivPosition(){
                                var intY = document.getElementById("<%=pnlResult.ClientID %>").scrollTop;
                                //document.title = intY;
                                document.cookie = "yPos=!~" + intY + "~!";
                            }
                    </script>
                    <asp:Panel ID="pnlResult" runat="server" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="975px" Height="400px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="2" 
                            ForeColor="Black" GridLines="Vertical" OnRowCreated="dgvResult_RowCreated" 
                            OnRowDataBound="Lookup_RowDataBound" Font-Size="10pt" 
                            OnSelectedIndexChanged="dgvResult_SelectedIndexChanged" Width="100%" 
                            style="margin-bottom: 0px">
                            <RowStyle BackColor="#F7F7DE" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkBoxAll" runat="server" Width="15px"/>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkBox" runat="server" Width="15px"/>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Wrap="False" Width="15px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td style="padding-left: 10px">

                </td>
                <td align="right" style="padding-right: 5px" >
                    <asp:Label ID="lblTimeStamp" runat="server" Width="293px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:Panel ID="pnlForChecking" runat="server">
                        <table cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="padding:0 3px; height: 20px;">
                                    <asp:Button ID="btnEndorseChecker2" runat="server" Text="ENDORSE TO CHECKER 2" Width="180px" Font-Size="10px" Enabled="False" OnClick="btnEndorseChecker2_Click" OnClientClick="showDiv()" />
                                </td>
                                <td style="padding:0 3px; height: 20px;">
                                    <asp:Button ID="btnEndorseApprover" runat="server" Text="ENDORSE TO APPROVER" Width="180px" Font-Size="10px" Enabled="False" OnClick="btnEndorseApprover_Click" OnClientClick="showDiv()" />
                                </td>
                                <td style="padding:0 3px; height: 20px;">
                                    <asp:Button ID="btnApprove" runat="server" Text="APPROVE" Width="180px" Font-Size="10px" Enabled="False" OnClick="btnApprove_Click" OnClientClick="showDiv()" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlForDisapprove" runat="server" Visible="False">
                        <table>
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtRemarks" runat="server" Font-Size="12px" Height="45px" TextMode="MultiLine" Width="388px" MaxLength="200"></asp:TextBox>
                                </td>
                                <td valign="top" style="padding:0 3px;">
                                    <asp:Button ID="btnDisapprove" runat="server" Text="DISAPPROVE" Font-Size="10px" Enabled="False" OnClick="btnDisapprove_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td valign="top" align="right" style="padding-right: 5px">
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Button ID="btnLoad" runat="server" Text="LOAD" Font-Size="10px" Width="139px" Enabled="False" OnClick="btnLoad_Click" />        
                            </td>
                        </tr>
                        <tr>
                            <td height="45px" align="left">
                                    <asp:Label ID="lblError" runat="server" Text="Show Error" AssociatedControlID="ibtnError" Visible="false"></asp:Label>
                                    <asp:ImageButton ID="ibtnError" runat="server" OnClientClick="return showHideError()" CausesValidation="false" Width="139px" ImageUrl="~/Images/error-bg.png"  Visible="false"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfControlNo" runat="server" />
    <asp:HiddenField ID="hfType" runat="server" />
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="500"/>
    </form>
</body>
</html>
