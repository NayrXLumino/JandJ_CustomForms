<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeApprovalRouteMaster.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeApprovalRouteMaster" Title="Approval Route Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
                                                <asp:DropDownList ID="ddlStatusList" runat="server" Width="122px" AutoPostBack="True" OnSelectedIndexChanged="ddlStatusList_SelectedIndexChanged">
                                                    <asp:ListItem Value="A">ACTIVE</asp:ListItem>
                                                    <asp:ListItem Value="C">INACTIVE</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
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
                                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="260px" ScrollBars="Both" Width="874px" >
                                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="858px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated" OnRowDataBound="Lookup_RowDataBound">
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
                        </table>
                    </asp:View>
                    <asp:View ID="DETAIL" runat="server">
                        <table cellpadding="0" cellspacing="0" >
                            <tr>
                                <td style="width: 884px" valign="top">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblChecker1" runat="server" Text="Checker 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker1Id" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnChecker1" runat="server" Text="..." Width="22px" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker1Name" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="300px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtChecker1Id" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblChecker2" runat="server" Text="Checker 2"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker2Id" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnChecker2" runat="server" Text="..." Width="22px" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtChecker2Name" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="300px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtChecker2Id" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td >
                                                <asp:Label ID="lblApprover" runat="server" Text="Approver"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtApproverId" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnApprover" runat="server" Text="..." Width="22px" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtApproverName" runat="server" BackColor="Gainsboro" ReadOnly="True" Width="301px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtApproverId" Font-Bold="true" Width="2px"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblCostcenter" runat="server" Text="Cost Center" Width="96px"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:TextBox ID="txtCostCenterCode" runat="server" BackColor="Gainsboro"></asp:TextBox>
                                            </td>
                                            <td valign="top">
                                                <asp:Button ID="btnCostcenter" runat="server" Text="..." Width="22px" />
                                            </td>
                                            <td valign="top" rowspan="2">
                                                <asp:TextBox ID="txtCostCenterDesc" runat="server" BackColor="Gainsboro" TextMode="MultiLine" Width="560px" Height="43px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtCostCenterCode" Font-Bold="true"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            
                                            </td>
                                            <td colspan="2" align="right">
                                                <asp:CheckBox ID="cbxAllCostcenter" runat="server" Text="ALL COSTCENTER" TextAlign="Left" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" Text="Status"></asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <asp:DropDownList ID="ddlStatus" runat="server" Width="159px">
                                                    <asp:ListItem Value="A">ACTIVE</asp:ListItem>
                                                    <asp:ListItem Value="C">INACTIVE</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Button ID="btnInUse" runat="server" Text="In-Use List" Visible="false" UseSubmitBehavior="false" CausesValidation="false" OnClientClick="return lookupInUseRoute()" />
                                                <asp:CheckBox ID="cbxForJobMod" runat="server" Checked="false" Text="FOR MANHOUR MODIFICATION" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <hr />
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
                            <asp:Label ID="lblRouteId" runat="server" Text="Route ID" Width="96px"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtRouteId" runat="server" Width="200px" BackColor="Gainsboro" Font-Bold="True" Font-Size="Medium"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:Button ID="btnX" runat="server" Text="NEW" UseSubmitBehavior="false" Width="107px" OnClick="btnX_Click" Height="27px" />
                        </td>
                        <td valign="top">
                            <asp:Button ID="btnY" runat="server" Text="EDIT" UseSubmitBehavior="false" Width="107px" Height="27px" OnClick="btnY_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
</asp:Content>

