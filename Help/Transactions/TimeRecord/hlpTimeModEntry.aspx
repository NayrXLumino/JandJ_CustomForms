<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="~/Help/Transactions/TimeRecord/hlpTimeModEntry.aspx.cs" Inherits="Help_Transactions_TimeRecord_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeModEntry.png" />
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
                            <td class="number" valign="top">2.</td>
                            <td>
                                <b>Control No</b>:or Transaction Control No., is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction. 
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Adjustment Date</b>: Click the lookup button to view logs and select one that needs for adjustment. <br /><br />
                                Employee Logs Lookup:<br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/TimeRecord/Images/TimeModEntry-LookupEmployeeLogs.png" />
                                <ul>
                                    <li>This pop-up window will let the user view his/her logs.</li>
                                    <li>You may user the serach box provided for you to search detail quickly.</li>
                                    <li>You select a row from the list  (<i>Selected row is highlighted with red color background</i>) then click the "SELECT" button to select detail.</li>
                                </ul>
                                <b>Shift for the day</b>:  This would be an employee's Shift for the day or his/her working hour schedule. <br /><br />
                                <b>Type</b>:  This would determine an employee's log either <b>NO IN</b>, <b>NO OUT</b>, or <b>NO IN AND NO OUT</b>.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                <b>Time In/Time Out</b>: 
                            </td>
                        </tr>  
                        <tr><td colspan="2"><hr /></td></tr>    
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td>
                                <b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.
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
                                <b>Status</b>: The status will let the user identify the progress for this transaction. It is either: <b>"NEW"</b>, <b>"ENDORSED"</b>, <b>"APPROVED"</b>.
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">8.</td>
                            <td>
                                <b>SAVE Button</b>: You may now click this button to save your entry. <br /><br />
                                This message will show if transaction was successfully saved. Click ok to continue. <br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Reusable Images/msgNewTransactionSaved.png" /> <br /><br />
                                <b>ENDORSE TO CHECKER 1</b>: After saving an entry status will now become "NEW" which means that you may now endorse a transaction. <br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullyEndorsedTransaction.png" />
                            </td>
                        </tr>         
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>