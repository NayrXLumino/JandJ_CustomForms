<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_Overtime_Default" Title="Overtime" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="400px" Width="225px">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnIndividualOT" runat="server" Text="Individual Overtime" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/pgeOvertimeIndividual.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnSpecialOT" runat="server" Text="Special Overtime" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/pgeOvertimeRange.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnBatchOT" runat="server" Text="Batch Overtime" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/pgeOvertimeBatch.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnBatchOvertimeUploading" runat="server" Text="Batch Overtime Uploading" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/pgeOvertimeBatchUploading.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnOvertimeReport" runat="server" Text="Overtime Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Overtime/pgeOvertimeReport.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnCarpassReport" runat="server" Text="Carpass Report" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="" Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Rendered Overtime in hour(s)" Height="400px" ScrollBars="Vertical" Width="658px">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Chart ID="Chart1" runat="server" Width="630px" Height="220px" AlternateText="Rendered Overtime" ToolTip="Rendered Overtime" >
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

