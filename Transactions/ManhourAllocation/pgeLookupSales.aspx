<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="pgeLookupSales.aspx.cs" Inherits="pgeLookupSales" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Sales Master Lookup</title>
       <link href="CSS/LookupSales.css" rel="stylesheet" type="text/css" /> 
          <link href="CSS/iFrameSales.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        input
        {
            text-transform: uppercase;
            /* margin-bottom: 20px; */
        }
        textarea
        {
            text-transform: uppercase;
        }
    </style>
    <script type="text/javascript" src="javascript/inFrame.js"></script> 
    <script type="text/javascript">

        function gridSelect(val) {
            var lbx = document.getElementById("lbxValues");
            var tbl = document.getElementById('grdViewList');
            var temp = document.createElement('option');
            temp.text = tbl.rows[parseInt(val, 10) + 1].cells[0].innerHTML;
            temp.value = tbl.rows[parseInt(val, 10) + 1].cells[0].innerHTML;
            var isExist = 'false';
            for (var i = 0; i < lbx.length && isExist == 'false'; i++) {
                if (lbx.options[i].value == temp.value) {
                    isExist = 'true';
                }
            }
            if (isExist == 'false') {
                try {
                    lbx.add(temp, 0); // standards compliant
                }
                catch (err) {
                    lbx.add(temp); // IE only
                }
            }
            var tbl = document.getElementById('grdViewList');
            var tbr = tbl.rows[parseInt(val, 10) + 1];
            var tbc = tbr.cells[0];

            for (var i = 1; i < tbl.rows.length; i++) {
                if (i % 2 != 0) {
                    tbl.rows[i].style.backgroundColor = '#F7F7DE';
                }
                else {
                    tbl.rows[i].style.backgroundColor = '#FFFFFF';
                }
            }
            tbr.style.backgroundColor = '#FF2233';

            document.getElementById('valueReturn').value = formatReturnValue();
        }

        function formatReturnValue() {
            var lbx = document.getElementById('lbxValues');
            var retValue = '';
            for (var i = 0; i < lbx.length; i++) {
                if (i != 0) {
                    retValue += ",";
                }
                retValue += lbx.options[i].value;
            }
            return retValue;
        }

    	function SendValueToParent() 
        {
            queryString = window.location.search.substring(1);
            parameters = queryString.split("&");
            var ft;
            var control;

            for (i=0;i<parameters.length;i++) 
            {
                ft = parameters[i].split("=");
                if (ft[0] == 'control') 
                {
                    control = ft[1];
                    break;
                }
            }

            var val = document.getElementById('valueReturn').value;
			window.opener.GetValueFromLookup(control, val);
			window.close();
			return false;
		}

		function SendValueToParent2(index) 
        {
		    queryString = window.location.search.substring(1);
		    parameters = queryString.split("&");
		    var ft;
		    var control;

		    for (i = 0; i < parameters.length; i++) {
		        ft = parameters[i].split("=");
		        if (ft[0] == 'control') {
		            control = ft[1];
		            break;
		        }
		    }

		    var tbl = document.getElementById('grdViewList');
		    var tbr = tbl.rows[parseInt(index, 10) + 1];
		    var tbc = tbr.cells[0];

		    window.opener.GetValueFromLookup(control, tbc.innerHTML);
		    window.close();
		    return false;
		}

		function removeItem() 
        {
		    var lbx = document.getElementById('lbxValues');
		    if (lbx.selectedIndex >= 0) {
		        lbx.remove(lbx.selectedIndex);
		        document.getElementById('valueReturn').value = formatReturnValue();
		    }
		}
		
		var popWin = null;
		
		function scram()
		{
          if (popWin != null && popWin.open) 
          {
            popWin.close();
          }
        }
        window.onfocus=scram;
		
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="content" style="width: 672px; height: 228px">
    <div id="top1" style="height: 422px; width: 630px;" > <%--onclick="return top1_onclick()" --%>
     <table style="width:108%; height: 102px;">
                        <tr>
                            <td style="width: 107px;">
                                <asp:Label ID="Label1" runat="server" Text="CPH Job No."></asp:Label>
                            </td>
                           <td style="width: 340px;">
                                <asp:TextBox ID="txtDashJobCode" runat="server" Width="190px"></asp:TextBox>
                           </td>
                            <td style="width: 157px;">
                                <asp:Label ID="Label3" runat="server" Text="Client Code"></asp:Label>
                            </td>
                           <td style="width: 243px;">
                                <asp:TextBox ID="txtClientCode" runat="server" Width="190px"></asp:TextBox>
                           </td>  
                        </tr>
                        <tr>
                            <td style="height: 22px; width: 107px;">
                                <asp:Label ID="Label2" runat="server" Text="Client Job No."></asp:Label>
                            </td>
                            <td style="width: 340px; height: 22px">
                                <asp:TextBox ID="txtClientJobNo" runat="server" Width="190px" height="16px"></asp:TextBox>
                            </td>
                            <td style="width: 157px; height: 22px;">
                                <asp:Label ID="Label5" runat="server" Text="Cost Center"></asp:Label>
                            </td>
                           <td style="width: 243px; height: 22px;">
                                <asp:TextBox ID="txtCostCenter" runat="server" Width="190px"></asp:TextBox>
                           </td> 
                        </tr>
                        <%--<tr>
                            <td style="width: 107px; height: 22px;">
                                <asp:Label ID="Label6" runat="server" Text="Dash Work Code" Visible="False"></asp:Label>
                            </td>
                           <td style="width: 340px; height: 22px;">
                        --%>
                                <asp:TextBox ID="txtDashWorkCode" runat="server" Width="190px" height="16px" 
                                    Visible="False"></asp:TextBox>
                        <%--</td> 
                            <td style="width: 157px; height: 22px;">
                                <asp:Label ID="Label4" runat="server" Text="Client FWBS Code" Visible="False"></asp:Label>
                            </td>
                           <td style="width: 243px; height: 22px;">
                        --%>
                                <asp:TextBox ID="txtFWBSCode" runat="server" Width="190px" Visible="False"></asp:TextBox>
                        <%--</td>
                        </tr>
                        --%>
                        <tr>
                            <td style="width: 107px; height: 24px;"><asp:Label ID="Label7" runat="server" Text="Status"></asp:Label>
                            </td>
                            <td style="width: 340px; height: 24px;">
                                <asp:DropDownList ID="ddlStatus" runat="server">
                                </asp:DropDownList>
                            </td>
                             <td style="width: 157px; height: 24px;">
                                 &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="width: 107px; height: 22px;"><asp:Button ID="btnGenerate" runat="server" Text="Generate" Font-Size="10px" Height="20px" OnClick="btnGenerate_Click" Width="85px"></asp:Button></td>
                        </tr>
                        <tr>
                            <td colspan="4"><script language="javascript" type="text/javascript">
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
                                document.getElementById("<%=Panel1.ClientID %>").scrollTop = strPos;
                                }
                                else
                                {
                                document.cookie = "yPos=!~0~!";
                                }
                            }
                        }
                        function SetDivPosition(){
                            var intY = document.getElementById("<%=Panel1.ClientID %>").scrollTop;
                            //document.title = intY;
                            document.cookie = "yPos=!~" + intY + "~!";
                        }
                        </script>
                        <table>
                            <tr>
                                <td style="height: 228px"><asp:Panel ID="Panel1" runat="server" Height="240px" ScrollBars="Auto" Width="451px" BorderStyle="Solid" BorderWidth="1px" onscroll="SetDivPosition()">
                                        <asp:GridView ID="grdViewList" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" OnRowCreated="grdViewList_RowCreated" OnRowDataBound="grdViewList_RowDataBound">
                                            <FooterStyle BackColor="#CCCC99" />
                                            <RowStyle BackColor="#F7F7DE" />
                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                  </asp:Panel></td>
                                  <td style="width: 34px; height: 228px">
                                      <table style=" height:100%;">
                                           <tr valign="bottom">
                                            <td style="width: 27px"> <asp:Button ID="btnAdd" runat="server" Text=">>" 
                                                    Font-Bold="True" Font-Size="12px" Height="24px" Width="24px" 
                                                    OnClick="btnAdd_Click" UseSubmitBehavior="False" Visible="False" /></td>
                                           </tr>
                                           <tr valign="top">
                                            <td style="width: 27px"><asp:Button ID="btnDel" runat="server" Text="<<" 
                                                    Font-Bold="True" Font-Size="12px" Height="24px" Width="24px" 
                                                    OnClick="btnDel_Click" Visible="False" /></td>
                                           </tr>
                                       </table>
                                  </td>
                                  <td style="width: 199px; height: 228px">
                                        <table width="99%" cellpadding="0" cellspacing="0" style="height: 202px">
                                          <tr><td align="center" style="width: 218px"><asp:Label ID="lblValues" runat="server" Text=""></asp:Label> </td>
                                          </tr>
                                            <tr>
                                                <td align="center" style="width: 218px; height: 95px;">
                                                    <asp:ListBox ID="lbxValues" runat="server" Height="198px" Width="195">
                                                    </asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="height: 20px; width: 218px;" align="center" valign="bottom">
                                                    <asp:Button ID="btnRetrieve" runat="server" Text="Load Details" Height="24px" Width="194px" UseSubmitBehavior="false"/>
                                                </td>
                                            </tr>
                                         </table>
                                  </td>
                            </tr>
                        </table>
                            </td>
                        </tr>
                    </table>
                                </div>
    </div>
        <asp:HiddenField ID="valueReturn" runat="server" />
        <asp:HiddenField ID="valueControl" runat="server" />
    </form>
</body>
</html>
