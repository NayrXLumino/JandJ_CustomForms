<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/pgeHELP.master" CodeFile="hlpTimeRecordEntryReport.aspx.cs" Inherits="Help_Transactions_TimeRecord_Default2" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeRecReport.png" />
                </td>   
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td>
                                <b>FILTER</b>: Filtering is use to select specific details or informations to be viewed only or to be print-out as specified by the user. <br /><br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeRecReport-Filter.png" /><br /><br />
                                You can filter report by its <b>Employee or Costcenter</b>. <br /><br />
                                <i>Each informations are constantly designed, for you to do the same procedures</i>. You can type an informations directly from the space provided for you, or view its list from the lookup window by cliking the lookup button. <br /><br />

                                 Look up for Employee:<br />
                                <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeModReport-LookupEmployee.png" />
                                <ul>
                                    <li>You may use the <b>SEARCH BOX</b> to search for a specific detail/information.</li>
                                    <li>From the costcenter list, select a row to be included in the selected list by clicking it ones.</li>
                                    <li>From the selected list, exlude costcenter by double clicking on it.</li>
                                    <li>You may use the "SELECT" button to to select all costcenters from the selected list.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">
                               Note: You may double click a row from the list to select an information directly. <br />
                               Note: You may follow the same procedures in selecting informations for other lookups. <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <br />
                                <b>INCLUDE</b>: Is use to select specific Column Header to be inluded in reports.<br />
                                <asp:Image ID="Image11" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeModReportColumnIncluder.png" /><br />
                                <ul>
                                    <li>A user may click on the box to check/uncheck column.</li>
                                    <li>Put a cheked mark on the box to inlude column in reports. If not just leave un-checked.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">2.</td>
                            <td>
                               <b>GENERATE button</b>: A user may click this button to display result for report. <br /><br />
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeRecReport-GeneratedReport.png" /> <br />
                                <i>Based on Day Code (e.g REST, COMP HOLIDAY) color indicators are present for better viewing</i> <br /><br />
                                <b>CLEAR button</b>: A user may click this button to clear out or empty the result. <br /><br />
                                <b>EXPORT button</b>: <br /><br />
                                <b>PRINT button</b>: To print report(s) from the Result List. <br /><br />
                                <b>CANCEL button</b>: Use to cancel Report. <br /><br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
         </table>
    </div>
</asp:Content>
