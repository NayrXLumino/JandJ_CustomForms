<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_Manhour_Default" Title="Manhour Transaction" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register assembly="System.Web.DataVisualization" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="400px" Width="225px">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnAllocationEntry" runat="server" Text="Allocation Entry" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/ManhourAllocation/pgeWorkRecord.aspx" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAllocationModification" runat="server" Text="Allocation Modification" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/ManhourAllocation/pgeJobSplitMod.aspx" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAllocationReport" runat="server" Text="Allocation Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/ManhourAllocation/pgeRJobSplit.aspx" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnManhourReport" runat="server" Text="Manhour Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/ManhourAllocation/pgeRManHours.aspx" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRoutedReport" runat="server" Text="Route Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/ManhourAllocation/pgeRJobSplitEnroute.aspx" />
                        </td>
                    </tr>
                </table>
                
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Information" Height="400px" ScrollBars="Vertical" Width="658px">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Chart ID="Chart1" runat="server" Width="630px" Height="220px" AlternateText="Rendered Manhour" ToolTip="Rendered Manhour" >
                                <Series>
                                    <asp:Series Name="Series1" XValueType="Date" YValueType="Double">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                    <area3dstyle LightStyle="Realistic" Inclination="38" PointDepth="200" IsRightAngleAxes="False" WallWidth="0" IsClustered="False" />
									        <position Y="2" Height="94" Width="94" X="2"></position>
									        <axisy LineColor="64, 64, 64, 64" >
										        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
										        <MajorGrid LineColor="64, 64, 64, 64" />
									        </axisy>
									        <axisx LineColor="64, 64, 64, 64" LabelAutoFitStyle="LabelsAngleStep30" >
										        <LabelStyle Font="Calibri, 8.25pt" />
										        <MajorGrid LineColor="64, 64, 64, 64" />
									        </axisx>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>    
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>

