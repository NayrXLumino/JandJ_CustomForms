<%@ Page Title="Approval Route Conversion" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeRouteConversion.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeRouteConversion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table width="100%">
    <tr>
        <th>
        
        </th>
        <th>
            
        </th>
        <th>
            <asp:Label ID="Label2" runat="server" Text="Route Code"></asp:Label>
        </th>
        <th>
        
        </th>
        <th align="center">
            <asp:Label ID="Label3" runat="server" Text="Checker 1"></asp:Label>
        </th>
        <th align="center">
            <asp:Label ID="Label4" runat="server" Text="Checker 2"></asp:Label>
        </th>
        <th align="center">
            <asp:Label ID="Label5" runat="server" Text="Approver"></asp:Label>
        </th>
    </tr>
    <tr>
        <td>
            <%--<asp:RequiredFieldValidator ID="RequiredValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFromRouteCode" Font-Bold="true"></asp:RequiredFieldValidator>--%>
        </td>
        <td>
            <asp:Label ID="Label1" runat="server" Text="FROM"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtFromRoute" runat="server" BackColor="Gainsboro" 
                ontextchanged="txtFromRoute_TextChanged"></asp:TextBox>
        </td>
        <td>
            <asp:Button ID="btnFromRoute" runat="server" Text="..." 
                CausesValidation="False" Width="22px"/>
        </td>
        <td align="center">
            <asp:TextBox ID="txtFromRouteC1" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td align="center">
            <asp:TextBox ID="txtFromRouteC2" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td align="center">
            <asp:TextBox ID="txtFromRouteAP" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtToRoute" Font-Bold="true"></asp:RequiredFieldValidator>
        </td>
        <td>
            <asp:Label ID="Label6" runat="server" Text="TO"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtToRoute" runat="server" BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td>
            <asp:Button ID="btnToRoute" runat="server" Text="..." 
                CausesValidation="False" Width="22px"/>
        </td>
        <td align="center">
            <asp:TextBox ID="txtToRouteC1" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td align="center">
            <asp:TextBox ID="txtToRouteC2" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
        <td align="center">
            <asp:TextBox ID="txtToRouteAP" runat="server" Width="200px" 
                BackColor="Gainsboro"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
        
        </td>
        <td colspan="6">
            <asp:Panel ID="pnlTransactions" runat="server" GroupingText="Transactions">
                <asp:CheckBoxList ID="cbxTransactions" runat="server" RepeatColumns="3" 
                    Width="800px" RepeatDirection="Horizontal" 
                    onselectedindexchanged="cbxTransactions_SelectedIndexChanged">
                </asp:CheckBoxList>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td colspan="7">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="7">
            <table>
                <tr>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="rblBound" Font-Bold="true"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblBound" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                            Width="257px" onselectedindexchanged="rblBound_SelectedIndexChanged">
                            <asp:ListItem Text="Costcenter" Value="C" ></asp:ListItem>
                            <asp:ListItem Text="Employee" Value="E" ></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:MultiView ID="mtvBound" runat="server">
                            <asp:View ID="Costcenter" runat="server" >
                                <table style="width: 882px">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtSearchCostcenter" runat="server" Width="394px" AutoPostBack="true"
                                                ontextchanged="txtSearchCostcenter_TextChanged"></asp:TextBox>
                                        </td>
                                        <td>
                                        
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="Label7" runat="server" Text="Included in Transaction"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:ListBox ID="lbcCostcenterChoice" runat="server" Width="400px" 
                                                Height="150px"></asp:ListBox>
                                        </td>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnCostcenterSelectAll" runat="server" Text=">>" Width="22px" 
                                                            onclick="btnCostcenterSelectAll_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnCostcenterSelectIndividual" runat="server" Text=">" 
                                                            Width="22px" onclick="btnCostcenterSelectIndividual_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnCostcenterRemoveIndividual" runat="server" Text="<" 
                                                            Width="22px" onclick="btnCostcenterRemoveIndividual_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnCostcenterRemoveAll" runat="server" Text="<<" Width="22px" 
                                                            onclick="btnCostcenterRemoveAll_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="center" valign="top">
                                            <asp:ListBox ID="lbxCostcenterInclude" runat="server" Width="400px" 
                                                Height="150px"></asp:ListBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNoOfCostcentersChoice" runat="server" Text="[No. of item(s)]"></asp:Label>
                                        </td>
                                        <td>
                        
                                        </td>
                                        <td>
                                            &nbsp;
                                            <asp:Label ID="lblNoOfCostcentersInclude" runat="server" Text="[No. of item(s)]"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="Employee" runat="server" >
                                <table style="width: 882px">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtSearchEmployee" runat="server" Width="394px" AutoPostBack="true"
                                                ontextchanged="txtSearchEmployee_TextChanged"></asp:TextBox>
                                        </td>
                                        <td>
                                        
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="Label8" runat="server" Text="Included in Transaction"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:ListBox ID="lbcEmployeeChoice" runat="server" Width="400px" 
                                                Height="150px"></asp:ListBox>
                                        </td>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnEmployeeSelectAll" runat="server" Text=">>" Width="22px" 
                                                            onclick="btnEmployeeSelectAll_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnEmployeeSelectIndividual" runat="server" Text=">" 
                                                            Width="22px" onclick="btnEmployeeSelectIndividual_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnEmployeeRemoveIndividual" runat="server" Text="<" 
                                                            Width="22px" onclick="btnEmployeeRemoveIndividual_Click" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnEmployeeRemoveAll" runat="server" Text="<<" Width="22px" 
                                                            onclick="btnEmployeeRemoveAll_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="center" valign="top">
                                            <asp:ListBox ID="lbxEmployeeInclude" runat="server" Width="400px" 
                                                Height="150px"></asp:ListBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNoOfEmployeesChoice" runat="server" Text="[No. of item(s)]"></asp:Label>
                                        </td>
                                        <td>
                        
                                        </td>
                                        <td>
                                            &nbsp;
                                            <asp:Label ID="lblNoOfEmployeesInclude" runat="server" Text="[No. of item(s)]"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
        
        </td>
        <td colspan="2">
            <asp:Button ID="btnSubmit" runat="server" Text="Submit Changes" 
                onclick="btnSubmit_Click" />
        </td>
        <td>
        
        </td>
        <td>
            <asp:Button ID="btnClear" runat="server" Text="Clear" Width="177px" 
                onclick="btnClear_Click" CausesValidation="false" />
        </td>
    </tr>
</table>

<asp:HiddenField ID="hfPrevFromRoute" runat="server" />

</asp:Content>

