<%@ Page Language="C#" CodeFile="pgeLookupSubWork.aspx.cs" Inherits="pgeLookupSubWork" EnableEventValidation="false" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Work Activity Master Lookup</title>
    
    <style type="text/css">
        input
        {
            text-transform: uppercase;
        }
        textarea
        {
            text-transform: uppercase;
        }
    body
    {
        font:12px Calibri;
    }
    </style>
    
    <script type="text/javascript">
        function AssignValue(index) 
        {
            var tbl = document.getElementById('GridView1');
		    var tbr = tbl.rows[parseInt(index, 10) + 1];
		    var tbc1 = tbr.cells[0];
		    var tbc2 = tbr.cells[1];

		    for (var i = 1; i < tbl.rows.length; i++) 
            {
                if (i % 2 != 0) 
                {
		            tbl.rows[i].style.backgroundColor = '#F7F7DE';
		        }
		        else {
		            tbl.rows[i].style.backgroundColor = '#FFFFFF';
		        }

		    }

		    tbr.style.backgroundColor = '#FF2233';

		    document.getElementById('hiddenSubWork').value = tbc1.innerHTML;
		    document.getElementById('hfCategory').value = tbc2.innerHTML;
		}

		function SendValueToParent() {
		    var myVal1 = document.getElementById('hiddenSubWork').value;
		    var myVal2 = document.getElementById('hfCategory').value;
		    window.opener.GetValueFromChildSubWork(myVal1, myVal2);
		    window.close();

		    return false;
		}

		function SendValueToParent2(index) {
		    var tbl = document.getElementById('GridView1');
		    var tbr = tbl.rows[parseInt(index, 10) + 1];

		    window.opener.GetValueFromChildSubWork(tbr.cells[0].innerHTML, tbr.cells[1].innerHTML);
		    window.close();
		    return false;
		}
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td style="width: 100px">
                    <asp:Label ID="Label1" runat="server" Text="Search" Font-Size="13px"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtSearch" runat="server" Width="457px" 
                        OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                </td>
                <td style="width: 100px" align="center">
                    <asp:Button ID="btnSelect" runat="server" Text="Select" />
                </td>
                <td style="width: 100px">
                    <asp:CheckBox ID="cbxGlobal" runat="server" Text="GLOBAL/COMMON" 
                        oncheckedchanged="cbxGlobal_CheckedChanged" AutoPostBack="true" />  
                </td>
            </tr>
            <tr>
                <td colspan="4" style="height: 5px">
                    <hr />
                </td>
            </tr>
            <tr>
                <td colspan = "4">
                    <table>
                        <tr valign="bottom">
                            <td>
                                <asp:Button ID="btnPrev" runat="server" Text="Previous" Height="20px" Width="80px" OnClick="NextPrevButton" Font-Size="12px" />
                            </td>
                            <td style="width: 150px" align="center">
                                <asp:Label ID="lblRows" runat="server" Text=" - of    Row(s)" Font-Names="Calibri" Font-Size="12px"></asp:Label>
                            </td>
                            <td style="width: 83px">
                                <asp:Button ID="btnNext" runat="server" Text="Next" Height="20px" Width="80px" OnClick="NextPrevButton" Font-Size="12px" />
                            </td>
                        </tr>
                    </table>  
                </td>
            </tr>
            <tr>
                <td colspan="4" align="center" style="height: 199px">
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
                                document.getElementById("<%=pnlGrid.ClientID %>").scrollTop = strPos;
                                }
                                else
                                {
                                document.cookie = "yPos=!~0~!";
                                }
                            }
                        }
                        function SetDivPosition(){
                            var intY = document.getElementById("<%=pnlGrid.ClientID %>").scrollTop;
                            //document.title = intY;
                            document.cookie = "yPos=!~" + intY + "~!";
                        }
                        </script> 
                    
                    <asp:Panel ID="pnlGrid" runat="server" Height="210px" Width="720px" 
                        ScrollBars="Vertical" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                    <asp:GridView ID="GridView1" runat="server" BackColor="White" BorderColor="#DEDFDE" 
                            BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
                            GridLines="Vertical" Width="720px" 
                            OnRowDataBound="Lookup_RowDataBound" OnRowCreated="GridView1_RowCreated" 
                            HorizontalAlign="Left">
                        <FooterStyle BackColor="#CCCC99" />
                        <RowStyle HorizontalAlign="Left" BackColor="#F7F7DE" />
                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                    </asp:Panel>
                
                    
                </td>
            </tr>
        </table>
    </div>
        <asp:HiddenField ID="hiddenSubWork" runat="server" />
        <asp:HiddenField ID="hfCategory" runat="server" />
    </form>
</body>
</html>
