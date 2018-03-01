<%@ Page Title="" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_WorkInformation_Default" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="400px" Width="225px">
                <table cellpadding="0" cellspacing = "0">
                    <tr>
                        <td>
                            <asp:Button ID="btnShiftIndividual" runat="server" Text="SHIFT UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeShiftIndividualUpdate.aspx" 
                                Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnShiftBatch" runat="server" Text="BATCH SHIFT UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeShiftBatchUpdate.aspx" 
                                Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnWorkgroupIndividual" runat="server" Text="WORKGROUP UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeWorkGroupIndividualUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnWorkgroupBatch" runat="server" Text="BATCH WORKGROUP UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeWorkGroupBatchUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnCostcenterIndividual" runat="server" Text="COSTCENTER UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeCostCenterIndividualUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnCostcenterBatch" runat="server" Text="BATCH COSTCENTER UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeCostCenterBatchUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRestdayIndividual" runat="server" Text="RESTDAY UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeRestdayIndividualUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRestdayBatch" runat="server" Text="BATCH RESTDAY UPDATE" 
                                Height="50px" Width="200px" CssClass="naviButton" 
                                PostBackUrl="~/Transactions/WorkInformation/pgeRestdayBatchUpdate.aspx" 
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnWorkInfoReport" runat="server" Text="WORK INFO REPORT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/WorkInformation/pgeWorkInformationReport.aspx" Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Work Information" Height="400px" ScrollBars="Vertical" Width="658px">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Default Shift"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDefaultShift" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Work Type"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtWorkType" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Work Group"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtWorkGroup" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label4" runat="server" Text="Cost Center Assignment"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCostCenter" runat="server" ReadOnly="true" Width="470px" 
                                TextMode="MultiLine" Height="58px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label5" runat="server" Text="Assignmened Date"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAssignedDate" runat="server" ReadOnly="true" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>





