<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpBatchShiftUpdate.aspx.cs" Inherits="Help_Transactions_WorkInformation_Default7" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/BatchShiftUpdate.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td valign="top">
                                <b>Process Date</b>: Is the date on which this entry will be effective. You can select its date by clicking into thi icon <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp" />.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">2.</td>
                            <td>
                                <b>FROM Shift</b>: This is an employee's current work group. <br /><br />

                                Lookup for Shift:<br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/BatchShiftUpdate-LookupShift.png" />
                                <ul>
                                    <li>This pop-up window retrieves lists for SHIFT from a data source record.</li>
                                    <li>You may select record by typing a detail/information from the <b>SEARCH BOX</b>.</li>
                                    <li>You may double click a single row from the list to select a shift directly.</li>
                                    <li>You may select a row from the list and click the button <b>SELECT</b> to select employee. (<i>Selected row is highlighted with red color background</i>)</li>
                                </ul>
                                <b>TO Group</b>:  Select a new work group where an employees will be moved or transferred. <i>You may follow same procedures in</i>  <a href=""><u>Lookup for Shift.</u></a>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Reason for Movement</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Costcenter</b>: You may filter an employee list by its Costcenter. Click the dropsown control to view and select costcenter.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                               This is where you can select an employee to be included in transaction or exclude. <i>Please follow this image below for the procedures</i>: <br /><br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/BatchGroupUpdate-IncludeExlude.png" /> 
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">Note: You may transfer all names from the list by clicking (>>) or (<<).</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">6.</td>
                            <td>
                                <b>"ENDORSE TO CHECKER 1" Button</b>: You may click this button if you wish to endorse this transaction to checker. <br /><br />
                               This message from the WEB prompts after clicking endorse button.<br />
                               <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Reusable Images/msgConfirmEndorseTransaction.png" /> <br /><br />
                               You may click <b>OK</b> if you wish to Endorse this Transaction or click <b>CANCEL</b> if not. <br /><br />
                               <i>Assume you have click <b>OK</b></i>.<br /><br />
                                This message from the WEB prompts when transaction was successfully endorsed. Click OK to continue. <br />
                                <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/BatchGroupUpdate-msgSuccessfullyEndorsedTransaction.png" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
