<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpLeaveEntry.aspx.cs" Inherits="Help_Transactions_Leave_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
     <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/IndividualLveEntry.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td>
                                <b>Employee's Information</b>:  This is where the current logged user's basic information appears. With ID No., Name, and Nickname. 
                                A user may file an entry for another employee (depending on the system's access grant for users) by clicking the lookup button for employee. <br /><br />
                                Assume you have clicked the lookup button for Employee, then the system will pop-up another window: <br /><br />
                                <u>Lookup for Employee</u>: <br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Reusable Images/LookupEmployee.png" />
                                <ul>
                                    <li>This pop-up window retrieves lists of all employees from a data source record.</li>
                                    <li>You may filter an employee by its Costcenter.</li>
                                    <li>You may select a row from the list and click the button <b>SELECT</b> to select employee. (<i>Selected row is highlighted with red color background</i>)</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td class="number"></td><td class="note">Note: Filing an entry to another employee is only applicable for users who have an access grant from the system. You may ask your system administrator for further assistance.</td></tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                           <td class="number">2.</td>
                           <td>
                                <b>Control No</b>: or Transaction Control No., is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction. 
                           </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                           <td class="number" valign="top">3.</td>
                           <td>
                                <b>Date of Leave</b>: Default Date is the system's current date. You can select Leave Date by clicking into this icon <asp:Image ID="Image4" runat="server" ImageUrl="" />.<br /><br />
                                <b>Shift for the Day</b>: This shows an employee's shift or woking hour schedule. <br /><br />
                                <b>Date Time Filed</b>: The date of entry being filed. <br /><br />
                                <b>Remarks</b>: Remarks is reserved for future transaction purpose. Your approver/checker mayinput remarks on this entry when checking and approving.
                           </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>UNIT: IN DAYS</b>:
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>TYPE of leave</b>: View lists and select a type of leave: <br />
                                <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/DropDownCtrl-TypeOfLeave.png" />
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">6.</td>
                            <td>
                                <b>Category</b>:
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">7.</td>
                            <td>
                                The <b>Start Time and Endtime</b> indicates the time duration of your Leave. You may choose one of the options provided for you to correspond its Start and End time.
                                <ul>
                                    <li>Whole Day Leave</li>
                                    <li>Quarter Day Leave</li>
                                    <li>Half Day Leave - AM</li>
                                    <li>Half Day Leave - PM</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">8.</td>
                            <td>
                                <b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">9.</td>
                            <td>
                                <b>Status</b>: The status will let the user identify the progress for this transaction. It is either: "NEW", "ENDORSED", "APPROVED".
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">10.</td>
                            <td>
                                <b>SAVE button</b>: When done filling all required informations for this Entry. A user may now click the save button. <br /><br />
                                This message from WEB prompts when an entry was successfully saved, the <u>Status will now become "NEW"</u>. Click OK to continue. <br />
                                <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Reusable Images/msgNewTransactionSaved.png" /> <br /><br />
                                This message from WEB prompts when there is Leave conflicts and cannot be save. Please check your entry and try to save it again.<br />
                                <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/msgLveConflicts.png" /> <br /><br />
                                <b>ENDORSE TO CHECKER 1 button</b>: When entry/transaction was successfully saved. A user may now click this button to Endorse Transaction to Checker/Approver. This message from WEB prompts when entry/transaction was successfully endorsed. Click OK to continue. <br />
                                <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullyEndorsedTransaction.png" />
                            </td>
                        </tr>
                        <tr><td class="number"></td><td class="note">Please be noted that all saved and endorsed transactions will be included to "NEW AND PENDING TRANSACTION" from your "HOME"</td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                Assume you want to view NEW AND PENDING TRANSACTIONS from HOME: <br /><br />
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Reusable Images/clickHomeMenu.png" /> <br /><br />
                                From "HOME" you can see the number(s) of <b>"NEW AND PENDING TRANSACTIONS</b>"  you may click the number to view its details.<br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveHome-NewAndPending.png" /><br /><br />
                                Lookup Details for Leave NEW AND PENDING TRANSACTIONS:<br />
                                <asp:Image ID="Image10" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/IndividualLveEntry-LookupNewPending.png" />
                                <ul>
                                    <li>Here you'll see all Overtime Entry transactions with status <b>"NEW" and "ENDORSED TO CHECKER 1"</b></li>
                                    <li>Please be noted that you can only LOAD TRANSACTION with status <b>"NEW"</b></li>
                                </ul>
                                Assume you have load transaction. This message from WEB prompts. Click OK to continue.<br />
                                <asp:Image ID="Image11" runat="server" ImageUrl="~/Help/Reusable Images/msgLoadingTransactionFromPendingList.png" /> <br /><br />
                                You will be redirected to Individual Overtime Entry with details loaded from New and Pending Transactions as shown below:
                                <asp:Image ID="Image12" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/IndividualLveEntry-LoadedTransactionFromNewPending.png" />
                                <ul>
                                    <li>You can edit this entry and <b>SAVE</b> for update. Or,</li>
                                    <li>You can <b>ENDORSE TO CHECKER 1</b> (Status will change to "ENDORSE TO CHECKER 1). Or,</li>
                                    <li>You can <b>CANCEL</b> this transaction.</li>
                                </ul>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
