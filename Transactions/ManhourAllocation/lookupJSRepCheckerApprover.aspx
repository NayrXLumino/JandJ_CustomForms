<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupJSRepCheckerApprover.aspx.cs" Inherits="Transactions_ManhourAllocation_lookupJSRepCheckerApprover" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
   <title>Checker/Approver Lookup</title>
    <style type="text/css">
        body
        {
            font-family:Arial;
            font-size:12px;
            text-transform:uppercase;
        }
        input
        {
            text-transform:uppercase;
        }
    </style>
    <script type="text/javascript">
    //Not in use gridSelect()
    function gridSelect(val)
    {
        var lbx = document.getElementById("lbxSelected");
        var tbl = document.getElementById('dgvResult');
        var temp = document.createElement('option');
        temp.text = tbl.rows[parseInt(val,10)+1].cells[1].innerHTML;
        temp.value = tbl.rows[parseInt(val,10)+1].cells[0].innerHTML;
        var isExist = 'false';
        for(var i = 0; i < lbx.length && isExist == 'false'; i++)
        {
            if(lbx.options[i].value == temp.value)
            {
                isExist = 'true';
            }
        }
        if(isExist == 'false')
        {
            try
            {
              lbx.add(temp,0); // standards compliant
            }
            catch(err)
            {
              lbx.add(temp); // IE only
            }
        }
        var tbl = document.getElementById('dgvResult');
        var tbr = tbl.rows[parseInt(val,10)+1];
        var tbc = tbr.cells[0];
        
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
            
        document.getElementById('hfValue').value = formatReturnValue();
    }
    
    function removeItem()
    {
        var lbx = document.getElementById('lbxSelected');
        if (lbx.selectedIndex >= 0) {
            lbx.remove(lbx.selectedIndex);
            document.getElementById('hfValue').value = formatReturnValue();
        }
    }
    function formatReturnValue()
    {
        var lbx = document.getElementById('lbxSelected');
        var retValue = '';
        for (var i = 0; i < lbx.length; i++)
        {
            if (i != 0)
            {
                retValue += ",";
            }
            retValue += lbx.options[i].value;
        }
        return retValue;
    }
    
    function SendValueToParent()
    {
        var control = document.getElementById('hfControl').value;
        var val = document.getElementById('hfValue').value;
        window.opener.GetValueFrom_lookupRJSCheckerApprover(control,val);
        window.close();
        return false;
    }
    
    function SendValueToParent2(index)
    {
        var control = document.getElementById('hfControl').value;
        var tbl = document.getElementById('dgvResult');
        var tbr = tbl.rows[parseInt(index,10)+1];
        var tbc = tbr.cells[0];
        window.opener.GetValueFrom_lookupRJSCheckerApprover(control,tbc.innerHTML);
        window.close();
        return false;
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td style="width: 309px">
                                <asp:TextBox ID="txtSearch" runat="server" Width="401px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
                            </td>
                            <td style="width: 141px" align="center">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
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
                    <asp:Panel ID="pnlResult" runat="server" Height="180px" ScrollBars="Both" Width="720px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="705px" OnRowDataBound="Lookup_RowDataBound" OnSelectedIndexChanged="dgvResult_SelectedIndexChanged" OnRowCreated="dgvResult_RowCreated" OnSelectedIndexChanging="dgvResult_SelectedIndexChanging">
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
                    <asp:Panel ID="pnlSelected" runat="server" GroupingText="SELECTED">
                        <asp:ListBox ID="lbxSelected" runat="server" Width="711px" Height="113px"></asp:ListBox>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSelect" runat="server" Text="SELECT" Width="124px" UseSubmitBehavior="False" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
        <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
        <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
        <asp:HiddenField ID="hfValue" runat="server"/>
        <asp:HiddenField ID="hfControl" runat="server"/>
    </div>
    </form>
</body>
</html>
