<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeApprovalRouteMasterReport.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeApprovalRouteMasterReport" Title="Approval Route Master Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" src="_collapseDiv.js"></script>
<div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>
    <div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:Table ID="tblFilter1" runat="server" Width="895px">
                <asp:TableRow ID="tbrRouteId" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" Width="120px">
                        <asp:Label ID="lblRouteId" runat="server" Text="Route ID(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtRouteId" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Button ID="btnRouteId" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrCostcenter" runat="server">
                    <asp:TableCell ID="TableCell4" runat="server" Width="120px">
                        <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:TextBox ID="txtCostcenter" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:Button ID="btnCostcenter" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrChecker1" runat="server">
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:Label ID="lblChecker1" runat="server" Text="Checker 1"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:TextBox ID="txtChecker1" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:Button ID="btnChecker1" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrChecker2" runat="server">
                    <asp:TableCell ID="TableCell19" runat="server">
                        <asp:Label ID="lblChecker2" runat="server" Text="Checker 2"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell20" runat="server">
                        <asp:TextBox ID="txtChecker2" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell21" runat="server">
                        <asp:Button ID="btnChecker2" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrApprover" runat="server">
                    <asp:TableCell ID="TableCell22" runat="server">
                        <asp:Label ID="lblApprover" runat="server" Text="Approver"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell23" runat="server">
                        <asp:TextBox ID="txtApprover" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell24" runat="server">
                        <asp:Button ID="btnApprover" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrStatus" runat="server">
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="150px">
                            <asp:ListItem Value="ALL">ALL</asp:ListItem>
                            <asp:ListItem Value="A">ACTIVE</asp:ListItem>
                            <asp:ListItem Value="C">INACTIVE</asp:ListItem>
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </div>
    <div style="width:900px">
        <table cellpadding="0" cellspacing="0" style="width:900px">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnGenerate" runat="server" Text="GENERATE" Width="100px" OnClick="btnGenerate_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnClear" runat="server" Text="CLEAR" Width="100px" UseSubmitBehavior="false" OnClick="btnClear_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnExport" runat="server" Text="EXPORT" Width="100px" UseSubmitBehavior="false" OnClick="btnExport_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnPrint" runat="server" Text="PRINT" Width="100px" UseSubmitBehavior="false" OnClick="btnPrint_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="398px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td style="width: 72px">
                                
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
                    <asp:Panel ID="pnlResult" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Height="350px" ScrollBars="Both" Width="898px" >
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated">
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
    </div>
    <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
    <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
    <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
</div>
</asp:Content>



