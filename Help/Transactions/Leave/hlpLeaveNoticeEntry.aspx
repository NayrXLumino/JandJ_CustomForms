<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpLeaveNoticeEntry.aspx.cs" Inherits="Help_Transactions_Leave_Default2" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
     <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image0" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry.png" />
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
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Reusable Images/LookupEmployee.png" />
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
                                <b>UNIT: IN DAYS</b>:
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Control No</b>: or Transaction Control No., is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction. <br /><br />
                                <i>For this type of transaction there is a lookup button in control no. This will retrieves all notice(s) you have previously filed for employee(s) UNLESS IF, the employee retrieved the notice from "LEAVE ENTRY".</i> <br /><br />
                                Assume you have previously filed a Leave notice entry and not yet been retrieved by the employee subject for this notice:<br /><br />
                                Lookup Detail: Leave Notice Entry<br />
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-LookupControlNo.png" />
                                <ul>
                                    <li>This pop-up window retrieves lists of leave notices from a data source record.</li>
                                    <li>You may use the Search box to seach specific detail.</li>
                                    <li>You may select a row from the list and click the button <b>SELECT</b> to select a notice. (Selected row is highlighted with red color background)</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Date of Leave</b>: Default Date is the system's current date. You can select Leave Date by clicking into this icon (date picker).<br /><br />
                                <b>Shift for the Day</b>: This shows an employee's shift or woking hour schedule.<br /><br />
                                <b>Date Time Filed</b>: The date of entry being filed.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>Inform Mode</b>: Select inform mode by clicking on its lookup button.<br /><br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-LookupInformMode.png" />
                                <ul>
                                    <li>You may use the search box to search specific detail.</li>
                                    <li>You may double click on the row to select detail directly and close lookup window.</li>
                                    <li>Once you have selected a row, you may click the "Select" button to select detail and close lookup window.</li>
                                </ul>
                                <b>Relation</b>: Select Relation by clicking on its lookup button.<br />
                                <asp:Image ID="Image13" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-LookupRelation.png" /> <br /><br />
                                You may follow procedures from <a href=""><u>Inform Mode</u></a>lookup for selecting detail.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="numner">6.</td>
                            <td>
                                <b>TYPE of leave</b>: View lists and select a type of leave: <br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/DropDownCtrl-TypeOfLeave.png" />
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">7.</td>
                            <td
                                ><b>Category:</b>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">8.</td>
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
                            <td class="number">9.</td>
                            <td>
                                <b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
                            </td>
                        </tr>
                        <tr>
                            <td class="number">10.</td>
                            <td>
                                <b>Status</b>: The status will let the user identify the progress for this transaction. It is either: "NEW", "ENDORSED", "APPROVED".
                            </td>
                        </tr>
                        <tr>
                            <td class="number">11.</td>
                            <td>
                                <b>SAVE Button</b>: A user may click this button when done filing entry.<br /><br />
                                A message from WEB prompts after saving leave notice. An employee subject for this notice can now retrieve transaction. Click OK to continue.<br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-msgNewTransacSavedEmployeeCanRetrieveNow.png" /><br /><br />
                                A user may encounter this messasge from the web. This means that an entry he/she filed is conflicting with an entry that was previously filed. A user may check the Date from this form.<br />
                                <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/msgLveConflicts.png" />
                            </td>
                        </tr>
                        <tr><td class="number"></td><td class="note">Note: Filing leave notice to other employee(s) is only applicable to users whose incharge to file from his/her behalf.</td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                Assume that the employee subject for this notice will retrieve entry from "LEAVE ENTRY" <br /><br />
                                To retreive filed notice: Follow these steps below:<br /><br />
                                <b>Step 1.</b> Go to Transaction > Leave > Leave Entry:<br />
                                <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-GotoMenuLeave.png" /> <br /><br />
                                <b>Step 2.</b> Form "Leave Entry" there must be a button lookup for Control No. Click the button.<br />
                                <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-LookupButtonCtrlNoFromLeaveEntry.png" /><br /><br />
                                <b>Step 3.</b> After cliking the lookup button, another window will pop-up with filed notice(s)<br />
                                <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-LookupControlNo.png" /><br /><br />
                                This message from the WEB prompts after selecting leave notice from lookup. Click OK to continue.<br />
                                <asp:Image ID="Image10" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveNoticeEntry-msgTransactionLoadedFromLeaveNotices.png" /><br /><br />
                                <b>Step 4.</b> After you have selected a notice from lookup window the details will be retrieve to "Leave Entry" form which you can Edit and update or Endorse to checker.<br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">
                                NOTE: A user may now save this retrieved transaction and endorse to checker. <br /><br />
                                NOTE: New saved transaction can be viewed and retreive from "NEW AND PENDING TRANSACTION" 
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>CLEAR Button</b>: A user may use this button to clear out some fields from the entry form.<br /><br />
                                <b>CANCEL Button</b>: A user may click this to cancel transaction before saving it.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>