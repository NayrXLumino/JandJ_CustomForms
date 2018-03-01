<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpBatchOvertimeEntry.aspx.cs" Inherits="Help_Transactions_Overtime_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/BatchOTEntry.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td valign="bottom"><b>Date of Overtime</b>: Default Date is the system's current date. You can select Overtime Date by clicking into this icon (date picker). <%--<asp:Image ID="Image1" runat="server" ImageUrl="" />--%></td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">2.</td>
                            <td><b>Work Group</b>: You can filter employees by selecting specific Work Group here.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td><b>Shift</b>: You can filter specific shift schedule of the employees here.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Shuttle Route</b>:
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>Type</b>: This will determine a type of overtime. There are two types available for Overtime:
                                <ul>
                                    <li><b>POST</b> Overtime  after the shift. </li>
                                    <li><b>ADVANCE</b> Overtime type  prior from shift.</li>
                                </ul>
                                <b>Start Time</b>:  By default, start time is the end time of employee's shift. This is an open entry, it means that a user may input a time as he/she specifies. <br /><br />
                                <b>Hours</b>: Is an hour spend by the employee in addition to those regular schedule. <br /><br />
                                <b>End Time</b>: The "End Time" will automatically calculate its duration of time base from the Start Time plus the amount of Overtime Hours inputted by the user.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">6.</td>
                            <td><b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top">7.</td>
                            <td><b>Costcenter</b>: You can filter a group of employees by selecting its costcenter here.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top">8.</td>
                            <td>
                                This is where you can select an employee to be included in transaction or exclude. You may follow this image below for the procedures: <br />
                                <asp:Image ID="Image21" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/BatchOT-IncludeExcludeDetail.png" /> <br /><br />
                            </td>
                        </tr>
                        <tr><td class="number"></td><td class="note">Note: You may transfer all names from the list by clicking (>>) or (<<)</td></tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">9.</td>
                            <td>
                                <b>ENDORSE TO CHECKER 1</b>:  This button will endorse an entry to checker 1. <br /><br />
                                This message from WEB prompts when trying to endorse transaction. <br />
                                <asp:Image ID="Image22" runat="server" ImageUrl="~/Help/Reusable Images/msgConfirmEndorseTransaction.png" />
                                <ul>
                                    <li>You may clik <b>OK</b> to continue the endorsement. Or,</li>
                                    <li>You may click <b>CANCEL</b> to cancel the endorsement</li>
                                </ul>   
                                Assume you have click OK, this message from the WEB prompts when transaction successfully endorsed:<br />
                                <asp:Image ID="Image23" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullyEndorsedTransaction.png" /> <br /><br />
                                This message from WEB prompts when the system detects problem/error while endorsing transaction:<br />
                                <asp:Image ID="Image24" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/BatchOT-msgReviewEntries.png" />
                                <ul><li>It ask the user to check details for review. Click OK to continue.</li></ul>
                                <u>Below screenshot shows an example that a user may encounter while endorsing transaction</u>: <br /><br />
                                This means that the system could not find a route set-up for the employee(s). You may ask your system administrator to assist you with this problem: <br />
                                <asp:Image ID="Image25" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/BatchOTEntry-NoRouteSetup.png" />
                                <ul>
                                    <li><b>"DISREGARD LIST AND CONTINUE"</b> will save and endorse those transactions that does not have any errors and will just disregard those employees listed for review for certain validation like no route.</li>
                                    <li><b>"KEEP LIST AND CONTINUE"</b> will save and endorse those transactions that does not have any errors and will retain those employees listed for review for certain validation in the Include list. </li>
                                    <li><b>"CANCEL TRANSACTIONS"</b>  will just disregard all entries and will not save and endorse any transaction at all.</li>
                                </ul>                                                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>