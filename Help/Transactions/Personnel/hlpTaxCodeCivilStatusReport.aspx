<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpTaxCodeCivilStatusReport.aspx.cs" Inherits="Help_Transactions_Personnel_Default7" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image11" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/TaxCivilReport.png" />
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
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/TaxCivilReport-Filter.png" /> <br /><br />
                            <b>1.1</b> You can filter TAX CODE/CIVIL STATUS report by <b>Employee, Costcenters, Status, Tax Code From, and Tax Code To.</b>. <br /><br />
                            Each informations are constantly designed, for you to do the same procedures. You can type an informations directly from the space provided for you or view its list from the lookup window by cliking the lookup button. <br /><br />
                            <i>Assume you have click the Lookup button for employee.</i> <br /><br />
                            Lookup for EMPLOYEE: <br />
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/TaxCivilReport-LookupEmployee.png" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td class="note">
                            Note: You may double click a row from the list to select an information directly. <br />
                            Note: You may follow the same procedures in selecting informations to other lookups.
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td>
                            <br />
                            <b>1.2</b> This is where you can filter TAX CODE/CIVIL STATUS by its Effectivity date, Applied, and Endorsed.
                            <i>To select Date, click into this icon</i><asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp" /> <br /><br />
                            <b>1.3</b> This is where you can filter  TAX CODE/CIVIL STATUS by Checkers and its Date Checked. You can type an informations directly from the space provided for you or view its list from the lookup window by cliking the lookup button. <br /><br />
                            <i>Assume you have click the Lookup button for checkers.</i> <br /><br />
                            Lookup for Checkers:<br />
                            <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/TaxCivilReport-LookupEmployee.png" />
                            <ul>
                                <li>You may use the <b>SEARCH BOX</b> to search for a specific detail/information.</li>
                                <li>From the Checker's list, select a row to be included in the selected list by clicking it ones.</li>
                                <li>From the selected list, exlude checkers by double clicking on it.</li>
                                <li>You may use the "SELECT" button to select all checkers from the selected list.</li>
                            </ul>
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td class="note">
                            Note: You may double click a row from the list to select an information directly.<br />
                            Note: You may follow the same procedures in selecting informations to other lookups.
                        </td>
                    </tr>
                    <tr>
                        <td class="number"></td>
                        <td>
                            <b>Cheked Date</b>:  You can filter checkers by when its date checked. Click this icon to select date. <asp:Image ID="img2" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp" /> <br /><br />
                            <b>INCLUDE</b>: Is use to select specific Column Header to be inluded in reports as shown below: <br /><br />
                            <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/TaxCivilReport-Include.png" />
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
                            <b>GENERATE button</b>: You may click this button to display result for report. <br /><br />
                            <%--<asp:Image ID="Image5" runat="server" ImageUrl="" /> <br /><br />--%>
                            <b>CLEAR button</b>: Click this button to clear out some informations you have entered.<br /><br />
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
