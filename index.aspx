<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Workflow</title>
    <link href="CSS/index.css" rel="stylesheet" type="text/css" />
    <link rel="Shortcut Icon" href="Images/nxperticon.ico" />
    <style type="text/css">
        .cnsLinks
        {
            text-decoration:none;
            font-weight:bold;
            color:red;	
        }
        
        .cnsLinks:hover
        {
            text-decoration:none;
            color:blue;	
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfServerTime" runat="server" />
    <div class="pageContent">
        <table cellpadding = "0" cellspacing = "0">
            <tr>
                <td>
                    <div class="logo">
                        <script type="text/javascript">

                            // Current Server Time script (SSI or PHP)- By JavaScriptKit.com (http://www.javascriptkit.com)
                            // For this and over 400+ free scripts, visit JavaScript Kit- http://www.javascriptkit.com/
                            // This notice must stay intact for use.

                            //Depending on whether your page supports SSI (.shtml) or PHP (.php), UNCOMMENT the line below your page supports and COMMENT the one it does not:
                            //Default is that SSI method is uncommented, and PHP is commented:

                            //var currenttime = '<!--#config timefmt="%B %d, %Y %H:%M:%S"--><!--#echo var="DATE_LOCAL" -->' //SSI method of getting server date
                            //var currenttime = '<? print date("F d, Y H:i:s", time())?>' //PHP method of getting server date
                            var currenttime = document.getElementById('hfServerTime').value;

                            ///////////Stop editting here/////////////////////////////////

                            var montharray = new Array("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December")
                            var serverdate = new Date(currenttime)

                            function padlength(what) {
                                var output = (what.toString().length == 1) ? "0" + what : what
                                return output
                            }

                            function displaytime() {
                                serverdate.setSeconds(serverdate.getSeconds() + 1)
                                var datestring = montharray[serverdate.getMonth()] + " " + padlength(serverdate.getDate()) + ", " + serverdate.getFullYear()
                                var timestring = padlength(serverdate.getHours()) + ":" + padlength(serverdate.getMinutes()) + ":" + padlength(serverdate.getSeconds())
                                document.getElementById("servertime").innerHTML = datestring + " " + timestring
                            }

                            window.onload = function () {
                                setInterval("displaytime()", 1000)
                            }

                        </script>

                        <p><span id="servertime"></span></p>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color:<%= Resources.Resource.MENUBGCOLOR %>;height:25px">
                    &nbsp; 
                </td>
            </tr>
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0">
                        <tr valign="top">
                            <td valign="top" style="height:180px">
                                <asp:Panel ID="pnlLogin" runat="server" GroupingText="Login" Width="310px">
                                    <table class="login"  >
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblUserName" runat="server" Text="User ID"></asp:Label>
                                            </td>
                                            <td>
                                             <asp:TextBox ID="txtActiveDirectory" runat="server" MaxLength="100" Width="204px" AutoPostBack="True" OnTextChanged="txtUserId_TextChanged" ></asp:TextBox>
                                             <asp:RequiredFieldValidator ID="rfUserName" runat="server" ErrorMessage="Enter User ID" ControlToValidate="txtActiveDirectory"></asp:RequiredFieldValidator>
                                            </td>
                                            <td style="display:none">
                                             <asp:TextBox ID="txtUserName" runat="server" MaxLength="100" Width="204px" AutoPostBack="False" ></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="Label1" runat="server" Text="Profile"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlProfiles" runat="server" Width="210px" AutoPostBack="True" OnSelectedIndexChanged="ddlProfiles_SelectedIndexChanged" TabIndex="1">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Choose Profile" ControlToValidate="ddlProfiles"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblPassword" runat="server" Text="Password" Width="65px"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:TextBox ID="txtPassword" runat="server" Width="204px" TextMode="Password" TabIndex="3"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfPassword" runat="server" ErrorMessage="Enter Password" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <asp:Button ID="btnLogin" runat="server" Text="Login" Width="100px" OnClick="btnLogin_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="pnlCNSHelp" runat="server" Width="300px">
                                    <table cellpadding="1px" cellspacing="1px" style="width:auto; height:auto; margin:0 auto; text-decoration:none; color:red; font-size:12px; text-align:left;">
                                        <tr align="left">
                                            <td colspan="2">
                                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Help/hlpDefault.aspx" CssClass="cnsLinks" Font-Underline="true" Target="_blank">CNS USER MANUAL</asp:HyperLink>&nbsp;
                                                &nbsp;&#38;&nbsp;&nbsp;
                                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="HTTP://CPH45/FAQ.php" CssClass="cnsLinks" Font-Underline="true" Target="_blank">FAQ</asp:HyperLink>
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td colspan="2">
                                                <br />
                                                <label>CNS Support Contact Numbers:</label>    
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td style="width:40px;">
                                                <label>CNS 1</label>
                                            </td>
                                            <td>
                                            
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td>

                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <label>HMR - 4128, 4135</label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <label>Payroll - 4139, 4117</label>
                                                        </td>
                                                    </tr>
                                                </table>                                                
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td >
                                                <label>CNS 2</label>
                                            </td>
                                            <td>
                                            
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td>
                                            
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <label>Manhour - (20)4343</label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <label>Billing - (20)4339</label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td >
                                                <label>CNS 3</label>
                                            </td>
                                            <td>
                                            
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td>
                                            
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <label>Procurement - 4137</label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td colspan="2">
                                                <label>email: cns_support@cph.chiyoda.co.jp</label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <td><br /></td>
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td colspan="2">
                                                <label style="color:Blue;">Click </label><asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="HTTP://CPH45" CssClass="cnsLinks" Target="_blank">HERE</asp:HyperLink>
                                                <label style="color:Blue;"> for CNS HELPDESK Page</label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                            <td style="width:10px; text-align:center" valign="top">
                                <asp:Panel ID="Panel1" runat="server" GroupingText="<b>Announcements</b>" 
                                    Width="583px" Height="430px">
                                    <asp:Panel ID="pnlAnnouncement" runat="server" Width="566px" Height="400px" 
                                        ScrollBars="Vertical">
                                        <asp:Table ID="tblAnnounce" runat="server">
                                            <asp:TableRow>
                                                <asp:TableCell ColumnSpan="2">
                                                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                        <tr>
                                                            <td style="width:200px;">
                                                                
                                                            </td>
                                                            <td>
                                                                <table cellpadding="1" cellspacing="1">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Image ID="imgHigh" runat="server" Height="18px" ToolTip="High Importance" ImageUrl="~/Images/impt_red.jpg" />
                                                                        </td>
                                                                        <td>
                                                                            HIGH
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td>
                                                                <table cellpadding="1" cellspacing="1">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Image ID="imgMid" runat="server" Height="18px" ToolTip="Mid Importance" ImageUrl="~/Images/impt_blue.jpg"  />
                                                                        </td>
                                                                        <td>
                                                                            MID
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td>
                                                                <table cellpadding="1" cellspacing="1">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Image ID="imgLow" runat="server" Height="18px" ToolTip="Normal Importance" ImageUrl="~/Images/impt_black.jpg"/>
                                                                        </td>
                                                                        <td>
                                                                            LOW
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </asp:Panel>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <table cellpadding="0" cellspacing="0" style="text-align:right;" width="100%">
                        <tr>
                            <td align="right">
                                <span style="padding-right:10px;">
                                    <asp:HyperLink ID="hplHelp" runat="server" NavigateUrl="~/Help/hlpDefault.aspx" Target="_blank">Help</asp:HyperLink>
                                </span>
                                <asp:Label ID="lblCopyright" runat="server" Text="Copyright" Font-Size="XX-Small" ForeColor="InactiveCaptionText"></asp:Label>
                                 <span style="padding-right:10px; padding-left:1px">
                                <asp:Label ID="lblVesion" runat="server" Text="Version 4.5011.1268.0"  
                                    Font-Size="XX-Small" ForeColor="InactiveCaptionText"></asp:Label>
                                </span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
       
        <asp:HiddenField ID="hfServerDate" runat="server" />
        <asp:HiddenField ID="hfDB" runat="server" />
    </form>
</body>
</html>
