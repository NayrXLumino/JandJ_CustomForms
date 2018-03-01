<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpOvertimeEntry.aspx.cs" Inherits="Help_Transactions_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/Overtime Entry.png" />
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
                                <b>Lookup for Employee</b>: <br /><br />
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
                            <td class="number" valign="top">2.</td>
                            <td><b>Control No</b>: or <b>Transaction Control No.</b>, is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction. </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Date of Overtime</b>: Default Date is the system's current date. You can select Overtime Date by clicking into this icon <%--<asp:Image ID="Image10" runat="server" ImageUrl="" />--%>. <br /><br />
                                <b>Shift of the Day</b>: This shows an employee's shift or woking hour schedule.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Type</b>: This will determine a type of overtime. There are two types available for Overtime:
                                <ul>
                                    <li><b>POST</b> Overtime  after the shift. </li>
                                    <li><b>ADVANCE</b> Overtime type  prior from shift.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>Start Time</b>:  By default, start time is the end time of employee's shift. This is an open entry, it means that a user may input a time as he/she specifies. <br /><br />
                                <b>Overtime Hours</b>: Is an hour spend by the employee in addition to those regular schedule. <br /><br />
                                <b>End Time</b>: The "End Time" will automatically calculate its duration of time base from the Start Time plus the amount of Overtime Hours inputted by the user. 
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">6.</td>
                            <td>
                                <b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">7.</td>
                            <td>
                                <b>Shuttle Route</b>: <br /><br />
                                <b>Remarks</b>: Remarks is reserved for future transaction purpose. Your approver/checker mayinput remarks on this entry when checking and approving. <br /><br />
                                <b>Status</b>: The status will let the user identify the progress for this transaction. It is either: "NEW", "ENDORSED", "APPROVED".
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">8.</td>
                            <td>
                               <b>SAVE button</b>: When done filling all required informations for this Entry. A user may now click the save button. <br /><br />
                               This message from WEB prompts when an entry was successfully saved, the <b>status will now become "NEW"</b>. Click OK to continue. <br /><br />
                               <asp:Image ID="Image11" runat="server" ImageUrl="~/Help/Reusable Images/msgNewTransactionSaved.png" /> <br /><br />
                               This message from WEB prompts when there is an Overtime conflicts and cannot be save. Please check your entry and try to save it again. <br /><br />
                               <asp:Image ID="Image12" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-msgOTconflicts.png" /> <br /><br />
                               <b>ENDORSE TO CHECKER 1 button</b>: When entry/transaction was successfully saved. A user may now click this button to Endorse Transaction to Checker/Approver. <br /><br />
                               This message from WEB prompts when entry/transaction was successfully endorsed. Click OK to continue. <br /><br />
                               <asp:Image ID="Image20" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullyEndorsedTransaction.png" /> <br /><br />
                            </td>
                        </tr>
                        <tr><td class="number"></td><td class="note">Please be noted that all saved and endorsed transactions will be included to "NEW AND PENDING TRANSACTION" from your "HOME"</td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <br />
                                Assume you want to view NEW AND PENDING TRANSACTIONS from HOME: <br /><br />
                                <asp:Image ID="Image13" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-GotoHOME-Menu.png" /> <br /><br />
                                From "HOME" you can see the number(s) of "NEW AND PENDING TRANSACTIONS"  you may click the number to view its details. <br />
                                <asp:Image ID="Image14" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-NewAndPendingTransac.png" /> <br /><br />
                                Lookup Details for OVERTIME NEW AND PENDING TRANSACTIONS:<br />
                                <asp:Image ID="Image15" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-lookupNewAndPendingTransac.png" /> 
                                <ul>
                                    <li>Here you'll see all Overtime Entry transactions with status "NEW" and "ENDORSED TO CHECKER 1"</li>
                                    <li>Please be noted that you can only LOAD TRANSACTION with status <b>"NEW"</b></li>
                                </ul>
                                Assume you have loaded transaction. This message from WEB prompts. Click OK to continue.
                                <asp:Image ID="Image16" runat="server" ImageUrl="~/Help/Reusable Images/msgLoadingTransactionFromPendingList.png" /> <br /><br />

                                You will be redirected to Individual Overtime Entry with details loaded from New and Pending Transactions. <br />
                                <asp:Image ID="Image17" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-LoadedTransacFromNewPendingList.png" /> 
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
