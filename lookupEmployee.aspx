<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupEmployee.aspx.cs" Inherits="lookupEmployee" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Employee Lookup</title>
    <style type="text/css">
        body
        {
            font-family:Arial;
            font-size:12px;
        }
    </style>
    <script type="text/javascript">
        function SendValueToParent() {
			var val1 = document.getElementById('hfEmployeeId').value;
            var val2 = document.getElementById('hfEmployeeName').value;
            var val3 = document.getElementById('hfEmployeeNickname').value;

//            alert('val1' + val1);
//            alert('val2' + val2);
//            alert('val3' + val3);
            
            window.opener.GetValueFrom_lookupEmployee(val1,val2,val3);
            window.close();
			return false;
		}
		
		function SendValueToParent2(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            
            window.opener.GetValueFrom_lookupEmployee(tbr.cells[0].innerHTML, tbr.cells[1].innerHTML, tbr.cells[2].innerHTML);
            window.close();
            return false;
        }
        
		function  AssignValue(index)
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index,10)+1];
            
            for(var i = 1; i < tbl.rows.length; i++)
            {
                if(i % 2 != 0)
                {
                    tbl.rows[i].style.backgroundColor = '#F7F7DE';
                }
                else
                {
                    tbl.rows[i].style.backgroundColor = '#FFFFFF';
                }
                
            }
            
            tbr.style.backgroundColor = '#FF2233';
            
            document.getElementById('hfEmployeeId').value = tbr.cells[0].innerHTML;
            document.getElementById('hfEmployeeName').value = tbr.cells[1].innerHTML;
            document.getElementById('hfEmployeeNickname').value = tbr.cells[2].innerHTML;
        }
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellspacing="0">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblCostCenter" runat="server" Text="Costcenter" Width="77px"></asp:Label>
                            </td>
                            <td style="width: 614px">
                                <asp:DropDownList ID="ddlCostCenter" runat="server" Width="620px" AutoPostBack="True" OnSelectedIndexChanged="ddlCostCenter_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td style="width: 614px">
                                <asp:TextBox ID="txtSearch" runat="server" Width="613px" OnTextChanged="txtSearch_TextChanged" AutoPostBack="True"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" Font-Size="11px" Width="115px" OnClick="PageButton" />
                            </td>
                            <td style="width: 200px" align="center">
                                <asp:Label ID="lblPage" runat="server" Text="0 of 0"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" Font-Size="11px" Width="115px" OnClick="PageButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
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
                    <asp:Panel ID="pnlResult" runat="server" BorderWidth="1px" BorderStyle="solid" BorderColor="black" ScrollBars="Auto" Height="230px" Width="705px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="702px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated" OnRowDataBound="dgvResult_RowDataBound" OnSelectedIndexChanged="dgvResult_SelectedIndexChanged">
                            <RowStyle BackColor="#F7F7DE" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSelect" runat="server" Text="SELECT" UseSubmitBehavior="false" Width="200px" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfEmployeeId" runat="server" />
        <asp:HiddenField ID="hfEmployeeName" runat="server" />
        <asp:HiddenField ID="hfEmployeeNickname" runat="server" />
    </div>
    </form>
</body>
</html>
