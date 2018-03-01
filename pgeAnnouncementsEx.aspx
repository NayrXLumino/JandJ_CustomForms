<%@ Page Title="" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeAnnouncementsEx.aspx.cs" Inherits="pgeAnnouncementsEx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel ID="Panel1" runat="server" GroupingText="<b>Announcements</b>" Width="900px" Height="400px">
    <asp:Panel ID="pnlAnnouncement" runat="server" Width="890px" Height="380px" ScrollBars="Vertical">
        <asp:Table ID="tblAnnounce" runat="server">
            <asp:TableRow>
                <asp:TableCell ColumnSpan="2">
                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                        <tr>
                            <td style="width:200px;">
                                                                
                            </td>
                            <td>
                                <table cellpadding="1" cellspacing="1">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgHigh" runat="server" Height="18px" ToolTip="High Importance" ImageUrl="~/Images/impt_red.jpg" />
                                        </td>
                                        <td>
                                            HIGH
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="1" cellspacing="1">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgMid" runat="server" Height="18px" ToolTip="Mid Importance" ImageUrl="~/Images/impt_blue.jpg"  />
                                        </td>
                                        <td>
                                            MID
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="1" cellspacing="1">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgLow" runat="server" Height="18px" ToolTip="Normal Importance" ImageUrl="~/Images/impt_black.jpg"/>
                                        </td>
                                        <td>
                                            LOW
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
</asp:Panel>
    <asp:HiddenField ID="hfDB" runat="server" />
</asp:Content>

