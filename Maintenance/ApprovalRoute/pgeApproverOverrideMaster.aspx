<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeApproverOverrideMaster.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeApproverOverrideMaster" Title="Approver Override Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="contentBody" onmouseover="Spooler()" enableviewstate="true" onload="Spooler()" onprerender="Spooler()">
    <table>
        <tr>
            <td>
                <asp:MultiView ID="VIEWER" runat="server" ActiveViewIndex="0">
                    <asp:View ID="LIST" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSearch" runat="server" Text="Search" Width="43px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSearch" runat="server" Width="355px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                                            </td>
                                            <td style="width: 143px">
                                                &nbsp;</td>
                                            <td>
                                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                                            </td>
                                            <td style="width: 205px" align="center">
                                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" OnClick="NextPrevButton" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Table ID="Table2" runat="server">
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:Panel ID="pnlResult" runat="server" ScrollBars="Both" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="890px" Height="300px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="2" 
                            ForeColor="Black" GridLines="Vertical" OnRowCreated="dgvResult_RowCreated" 
                            OnRowDataBound="Lookup_RowDataBound" Font-Size="10pt" 
                            Width="100%" 
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
                                        <asp:CheckBox ID="chkBoxAll" runat="server"/>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkBox" runat="server"/>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Wrap="False" Width="15px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="DETAIL" runat="server">
                        <table cellpadding="0" cellspacing="0" >
                            <tr>
                                <td style="width: 884px" valign="top">
                                    <table width="100%">                                
                                        <tr>
                                            <td>
                                                                         
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblEmployeeId" runat="server" Text="User Code"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtEmployeeId" runat="server" BackColor="Gainsboro" 
                                                        Width="260px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnEmployeeId" runat="server" 
                                                        OnClientClick="return lookupEmployee('GENERAL')" Text="..." 
                                                        UseSubmitBehavior="false" Width="22px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblEmployeeName" runat="server" Text="User Name"></asp:Label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtEmployeeName" runat="server" BackColor="Gainsboro" 
                                                        Width="290px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="94px"></asp:Label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtNickname" runat="server" BackColor="Gainsboro" 
                                                        Width="290px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                            </td>
                                        </tr>
                                        <tr><td></td></tr>
                                        <tr>
                                            <td colspan="4">
                                                <input type="button" name="" value="Add Row" onclick="Addk('overrideApprover');" style="height: 20px" accesskey="a" id="btnAddRow" runat="server"/>
                                                <input type="button" name="" value="Delete Row" onclick="deleteRowk('overrideApprover');" style="height: 20px" accesskey="d" id="btnDeleteRow" runat="server"/>
                                                <br /><br />
                                                
                                                <table ID="Table1" width="100%" border="1">
                                                    <tr>
                                                    <th style="width: 50px">Check All</th>
                                                    <th style="width: 200px">Checker/Approver</th>
                                                    <th>Transactions</th>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:CheckBox ID="chkAll" runat="server"/>
                                                        </td>
                                                        <td align="center">
                                                            <asp:Button ID="btnBatchChecker" runat="server" Text="..." Width="22px" />
                                                        </td> 
                                                        <td>
                                                            <table ID="transactionAll" runat="server" width="100%">
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td> 
                                                    </tr>
                                                </table>
                                                <table ID="overrideApprover" width="100%">
                                                    <tr>
                                                        <td style="width: 50px;" align="center">
                                                            <input type="checkbox" name="chkboxChecker" accesskey="1"/>
                                                        </td>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td style="width: 200px" valign="middle">
                                                                        <asp:TextBox ID="txtChecker1Id" runat="server" BackColor="Gainsboro" Width="100px"></asp:TextBox>
                                                                        <asp:Button ID="btnChecker1" runat="server" Text="..." Width="22px" />
                                                                        <asp:TextBox ID="txtChecker1Name" runat="server" BackColor="Gainsboro" 
                                                                            Width="180px"></asp:TextBox>
                                                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                                            ControlToValidate="txtChecker1Id" CssClass="reqIndicator" ErrorMessage="*" 
                                                                            Font-Bold="true"></asp:RequiredFieldValidator>--%>
                                                                        <asp:HiddenField ID="hTransactions" runat="server" />
                                                                    </td>
                                                                    <td ID="test" runat="server" colspan="3">
                                                                        <table ID="transactions" runat="server" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td ID="arrow">
                                                            &nbsp;&nbsp;&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <hr />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        
                    </asp:View>
                    <asp:View ID="EDITDETAIL" runat="server">
                        <table cellpadding="0" cellspacing="0" >
                            <tr>
                                <td style="width: 884px" valign="top">
                                    <table width="100%">                                
                                        
                                        <tr>
                                            <td colspan="4">
                                                <br /><br />
                                                
                                                <table ID="Table3" width="100%" border="1">
                                                    <tr>
                                                    <th style="width: 150px">User Code</th>
                                                    <th style="width: 150px">Checker/Approver</th>
                                                    <th>Transactions</th>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Button ID="Button4" runat="server" Text="..." Width="22px" Enabled="false" />
                                                        </td> 
                                                        <td align="center">
                                                            <asp:Button ID="Button5" runat="server" Text="..." Width="22px" Enabled="false" />
                                                        </td> 
                                                        <td>
                                                            <table ID="transactionAllEdit" runat="server" width="100%">
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td> 
                                                    </tr>
                                                </table>
                                                <table ID="overrideApproverEdit" width="100%">
                                                    <tr>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td style="width: 150px" valign="middle">
                                                                        <asp:TextBox ID="txtUserCodeId" runat="server" BackColor="Gainsboro" Width="75px"></asp:TextBox>
                                                                        <asp:Button ID="btnUserCodeId" runat="server" Text="..." Width="22px" Enabled="false" />
                                                                        <asp:TextBox ID="txtUserCodeName" runat="server" BackColor="Gainsboro" 
                                                                            Width="150px"></asp:TextBox>
                                                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                                            ControlToValidate="txtChecker1Id" CssClass="reqIndicator" ErrorMessage="*" 
                                                                            Font-Bold="true"></asp:RequiredFieldValidator>--%>
                                                                        <asp:HiddenField ID="hTransactionsEdit" runat="server" />
                                                                    </td>
                                                                    <td style="width: 150px" valign="middle">
                                                                        <asp:TextBox ID="txtChecker1EditId" runat="server" BackColor="Gainsboro" Width="75px"></asp:TextBox>
                                                                        <asp:Button ID="btnChecker1Edit" runat="server" Text="..." Width="22px" Enabled="false" />
                                                                        <asp:TextBox ID="txtChecker1EditName" runat="server" BackColor="Gainsboro" 
                                                                            Width="150px"></asp:TextBox>
                                                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                                            ControlToValidate="txtChecker1Id" CssClass="reqIndicator" ErrorMessage="*" 
                                                                            Font-Bold="true"></asp:RequiredFieldValidator>--%>
                                                                    </td>
                                                                    <td ID="Td1" runat="server" colspan="3">
                                                                        <table ID="transactionsEdit" runat="server" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td ID="arrowEdit">
                                                            &nbsp;&nbsp;&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <hr />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td valign="top">
                            <asp:Button ID="btnX" runat="server" Text="NEW" UseSubmitBehavior="false" Width="107px" OnClick="btnX_Click" Height="27px" />
                        </td>
                        <td valign="top">
                            <asp:Button ID="btnY" runat="server" Text="EDIT" UseSubmitBehavior="false" Width="107px" Height="27px" OnClick="btnY_Click" CausesValidation="False" />
                        </td>
                        <td valign="top">
                            <asp:Button ID="btnZ" runat="server" Text="DELETE" UseSubmitBehavior="true" Width="107px" Height="27px" CausesValidation="False" OnClick="btnZ_Click" OnClientClick = "return deleteGridRow();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
    <asp:HiddenField ID="hfState" runat="server" Value="100"/>
</div>
</asp:Content>

