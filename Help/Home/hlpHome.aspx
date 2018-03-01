<%@ Page MaintainScrollPositionOnPostback="true" Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpHome.aspx.cs" Inherits="Help_Home_Default" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2" align="center">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Home/Images/CheckerOrApproversView.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td>Refreshes or Updates the counter for the checklist.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">2.</td>
                            <td>Waitlist. There are transactions that are still for endorsement to you as checker or approver of that transaction.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>For Checking and Approval. There are transactions you are responsible for checking, approval, or disapproval.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>For Next Level Checking and Approval. These are transactions that are still needs to be endorse to next level or to be approved by approver. (note: this is only applicable if you are the Checker 1 or Checker 2 of the transactions. If you are the approver, the counter will not increment.)</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>List of transactions where you have the right to check, approve, or disapprove. List may vary from user logged in's rights to check.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top"></td>
                            <td class="note">
                                Note: All counters can be clicked and will open a popup page where you can view those items. If counter is equal to zero(0) then no popup page will show.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td colspan="2">EMPLOYEE'S VIEW</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Home/Images/EmployeesView.png" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td class="number" valign="top">1.</td>
                            <td>These are the list of transactions of the user logged in. This is divided into parts, the NEW and PENDING and APPROVED and DISAPPROVED.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top">2.</td>
                            <td>NEW and PENDING. These are transactions that are still for approval categorized per transaction type.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>APPROVED and DISAPPROVED. These are transactions that are approved or disapproved by you checker or approver.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>List of transactions which are applicable to file. This may vary if application is allowed by company policy to be filed on workflow.</td>
                        </tr>
                        <tr>
                            <td class="number" valign="top"></td>
                            <td class="note">
                                Note: All counters can be clicked and will open a popup page where you can view those items. If counter is equal to zero(0) then no popup page will show.
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>EMPLOYEE'S VIEW</b>: <br /><br />
                                 <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Home/Images/EmployeesView.png" />
                                 <ul>
                                    <li>(1.) These are the list of transactions of the user logged in. This is divided into parts, the NEW and PENDING and APPROVED and DISAPPROVED.</li>
                                    <li>(2.) NEW and PENDING. These are transactions that are still for approval categorized per transaction type.</li>
                                    <li>(3.) APPROVED and DISAPPROVED. These are transactions that are approved or disapproved by you checker or approver.</li>
                                    <li>(4.) List of transactions which are applicable to file. This may vary if application is allowed by company policy to be filed on workflow.</li>
                                 </ul>
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">Note that all counters can be clicked and will open a popup page where you can view those items. If counter is equal to zero(0) then no popup page will show.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>LOOKUP FOR NEW AND PENDING</b>: <br /><br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Home/Images/LookUpForNewPending.png" />
                                <ul>
                                    <li>(1.) Allows you to search specific item from the list. Special characters like ampersand(&) and percent(%) can be used. Search in different columns and wild character search respectively.</li>
                                    <li>(2.) Indicates the status of the transaction. Transaction listed is based on transaction type counter selected.</li>
                                    <li>(3.) If status of transaction is NEW, LOAD TRANSACTION button is enabled that way you can retreive the transaction for updating, endorsement or cancellation.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>LOOKUP FOR APPROVED AND DISAPPROVED</b>: <br /><br />
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Home/Images/LookUpForApproveDisapproved.png" />
                                <ul>
                                    <li>(1.) Allows you to search specific item from the list. Special characters like ampersand(&) and percent(%) can be used. Search in different columns and wild character search respectively.</li>
                                    <li>(2.) Indicates the status of the transaction. Transaction listed is based on transaction type counter selected.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                               <b>WAITLIST</b> <br /><br />
                               <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Home/Images/Waitlist.png" />
                               <ul>
                                    <li>(1.) Allows you to search specific item from the list. Special characters like ampersand(&) and percent(%) can be used. Search in different columns and wild character search respectively.</li>
                                    <li>(2.) Indicates the status of the transaction. Transaction listed is based on transaction type counter selected.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>NEXT LEVEL CHECKING/APPROVING</b> <br /><br />
                                <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Home/Images/NextLevelCheckingApproval copy.png" />
                                <ul>
                                    <li>(1.) Allows you to search specific item from the list. Special characters like ampersand(&) and percent(%) can be used. Search in different columns and wild character search respectively.</li>
                                    <li>(2.) Indicates the status of the transaction. Transaction listed is based on transaction type counter selected.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>CHECKLIST</b> <br /><br />
                                <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Home/Images/Checklist.png" />
                                <ul>
                                    <li>(1.) A drop down control that filters the list based on the cost center of the transaction.</li>
                                    <li>(2.) Allows filtering of cost center from per department or per section.</li>
                                    <li>(3.) Allows you to search specific item from the list. Special characters like ampersand(&) and percent(%) can be used. Search in different columns and wild character search respectively.</li>
                                    <li>(4.) Increses or decreases the font size of the list.</li>
                                    <li>(5.) Allows user to switch from approval screen to disapproval screen.</li>
                                    <li>(6.) Check / Uncheck All functionality</li>
                                    <li>(7.) Indicates the status of the transaction. Transaction listed is based on transaction type counter selected.</li>
                                    <li>(8.) Individual checkbox per transaction. Only checked transactions are affected by action.</li>
                                    <li>(9.) If status of transaction is Endorsed to Checker 1 button will be enable otherwise disabled. This will endose transaction to next level of checker or approver.</li>
                                    <li>(10.) If status of transaction is Endorsed to Checker 2 button will be enable otherwise disabled. This will endose transaction to next level of checker or approver. </li>
                                    <li>(11.) If status of transaction is Endorsed to Approver button will be enable otherwise disabled. This will approve the transaction.</li>
                                    <li>(12.) This button will load the selected item(high-lighted in red) to be loaded to the transaction screen for a more detailed view of the transaction.</li>
                                </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td>
                                <b>CHECKLIST (FOR DISAPPROVAL CHANGES)</b> <br /><br />
                                <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Home/Images/Checklist-disApprovalChanges.png" />
                                <ul>
                                    <li>(1.) Remarks for disapproval. Maximum of 200 characters only and required.</li>
                                    <li>(2.) Once you check the items to be disapproved this will enable otherwise if there are no checkd items it will be disabled. This will disapproved the transaction.</li>
                                </ul>
                                <asp:Image ID="Image10" runat="server" ImageUrl="~/Help/Home/Images/Checklist-disApprovalChanges2.png" /> 
                                <ul>
                                    <li>(1.) Prompts user that endorsement or approval of the transaction was not successful. Some errors could have occured.</li>
                                    <li>(2.) Click Tab to show error. This will display the errors detail and shows the control number of that transaction.</li>
                                </ul>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
