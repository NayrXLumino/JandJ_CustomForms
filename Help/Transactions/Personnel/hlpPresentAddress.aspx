<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpPresentAddress.aspx.cs" Inherits="Help_Transactions_Personnel_Default4" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
         <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/PresentAddress.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td>
            </tr>
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
                                <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Reusable Images/LookupEmployee.png" />
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
                            <td>
                                <b>Control No</b>: or Transaction Control No., is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction. 
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Effectivity Date</b>:  Is the date on which this entry will be effective. You can select its date by clicking into this icon <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp" />
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Number of Street</b>: You may input in the information in the space provided for you.    <br /><br />
                                <b>Barangay/Municipality</b>: <i>Click the lookup button to select a detail/information.</i>   <br /><br />
                                Lookup for Barangay/Municipality: <br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Personnel/Images/PresentAddress-LookupBrgyMunicipality.png" /> 
                                <ul>
                                    <li>This pop-up window retrieves lists of Informations from a data source record.</li>
                                    <li>You may select record by typing a detail/information from the <b>SEARCH BOX</b>.</li>
                                    <li>You may double click a single row from the list to select the information.</li>
                                    <li>You may select a row from the list and click the button <b>SELECT</b> to select the information. (<i>Selected row is highlighted with red color background</i>)</li>
                                </ul>
                                <b>City/Province/District</b>: Click the lookup button to select a detail/information. <i>You may follow procedure from</i>  <a href=""><u>Lookup for Barangay/Municipality</u></a>  <br /><br />
                                <b>Telephone Number</b>: You may input in the information in the space provided for you.    <br /><br />
                                <b>Mobile Number</b>: You may input in the information in the space provided for you.   <br /><br />
                                <b>Email Address</b>: You may input in information in the space provided for you.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>Reason for Update</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">6.</td>
                            <td>
                                <b>Remarks</b>: Remarks is reserved for future transaction purpose. Your approver/checker mayinput remarks on this entry when checking and approving.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">7.</td>
                            <td>
                                <b>Status</b>: The status will let the user identify the progress for this transaction. It is either: "NEW", "ENDORSED", "APPROVED".
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">8.</td>
                            <td>
                                <b>SAVE button</b>: When done filling all required informations for this Entry. A user may now click the save button. <br /><br />
                                This message from WEB prompts when an entry was successfully saved, the <b>Status will now become "NEW"</b>. Click OK to continue. <br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Reusable Images/msgNewTransactionSaved.png" /> <br /><br />
                                A user will notice some changes from the form after saving entry:<br />
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/IndiShiftUpdate-CanNowEndorse.png" /><br />
                                As you can see, the "<b>ENDORSE TO CHECKER 1" button will enable</b>, it means that you may now endorse transaction. <br /><br />
                                This message from WEB prompts when there is already a transaction on route conflicting with this entry. You may check your entry again and try saving it back.<br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Transactions/WorkInformation/Images/IndiGroupUpdate-msgConflicting.png" /> <br /><br />
                                <b>ENDORSE TO CHECKER 1 button</b>: When entry/transaction was successfully saved. A user may now click this button to Endorse Transaction to Checker/Approver. This message from WEB prompts when entry/transaction was successfully endorsed. Click OK to continue. <br />
                                <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullyEndorsedTransaction.png" />
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
