<%@ Page Title="Leave Credits Report" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeLeaveCreditsReport.aspx.cs" Inherits="Transactions_Leave_pgeLeaveCreditsReport" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" src="_collapseDiv.js"></script>
<div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>
    <div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:Table ID="tblFilter1" runat="server" Width="895px">
                <asp:TableRow ID="tbrEmployee" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" Width="120px">
                        <asp:Label ID="lblEmployee" runat="server" Text="Employee(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtEmployee" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Button ID="btnEmployee" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
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
                <asp:TableRow ID="tbrCostcenterLine" runat="server">
                    <asp:TableCell ID="TableCell31" runat="server" Width="120px">
                        <asp:Label ID="Label3" runat="server" Text="Costcenter Line(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell32" runat="server">
                        <asp:TextBox ID="txtCostcenterLine" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell33" runat="server">
                        <asp:Button ID="btnCostcenterLine" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrPayPeriod" runat="server">
                    <asp:TableCell ID="TableCell10" runat="server">
                        <asp:Label ID="lblPayPeriod" runat="server" Text="Leave Year(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:TextBox ID="txtLeaveYear" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:Button ID="btnLeaveYear" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="tbrLeaveType" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="lblLeaveType" runat="server" Text="Leave Type(s)"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:TextBox ID="txtLeaveType" runat="server" Width="730px"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Button ID="btnLeaveType" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
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
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" Width="896px" Font-Size="11px" OnRowCreated="dgvResult_RowCreated">
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

