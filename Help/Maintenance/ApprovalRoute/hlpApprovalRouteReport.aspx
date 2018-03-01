<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpApprovalRouteReport.aspx.cs" Inherits="Help_Maintenance_ApprovalRoute_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="ReportEntryForm" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteReport.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                   <table width="100%">
                    <tr>
                        <td class="number" valign="top">1.</td>
                        <td>
                            <b>FILTER</b>: Filtering is use to select specific details or informations to be viewed only or to be print-out as specified by the user.<br /><br />
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteReport-FILTER.png" /> <br /><br />
                            <b>1.1</b> You can filter Route report by its <b>Route ID, Costcenter, Checker 1, CHecker 2, Approver, Status</b>. 
                            <ul>
                                <li>Each informations are constantly designed, for you to do the same procedures:</li>
                                <li>You can type an informations directly from the space provided for you or view its list from the lookup window by cliking the lookup button. </li>
                            </ul>
                            Assume you have click the <b>lookup button for Route IDs</b>: <br /><br />
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteReport-LookupRouteID.png" />
                            <ul>
                                <li>You may use the <b>SEARCH BOX</b> to search for a specific information.</li>
                                <li>From the Route ID list, select a row to be included in the selected list by clicking it ones.</li>
                                <li>From the selected list, exlude Route ID by double clicking on it.</li>
                                <li>You may use the "SELECT" button to to select Route ID(s) from the selected list.</li>
                            </ul>
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td class="note">
                            Note: You may double click a row from the list to select an information directly. <br />
                            Note: Selected row/information from the list is highlighted with red color background.
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td>
                            <br />
                            You may also filter Route Report by its <b>STATUS</b>: <b>ACTIVE</b> or <b>INACTIVE</b> <br /><br />
                        </td>
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>
                    <tr>
                        <td class="number" valign="top">2.</td>
                        <td>
                            <b>GENERATE button</b>: CLick this button to display report. <br /><br />
                            GENERATED REPORT: <br /><br />
                            <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteReport-GeneratedReport.png" /> <br /><br />
                            <b>CLEAR button</b>: Click this button for to clear out some informations you have entered. <br /><br />
                            <b>EXPORT button</b>: <br /><br />
                            <b>PRINT button</b>: To print report(s) from the result/generated list. <br /><br />
                            <b>CANCEL button</b>: Click this button if you wish not to continue. <br /><br />
                        </td>
                    </tr>
                   </table> 
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

