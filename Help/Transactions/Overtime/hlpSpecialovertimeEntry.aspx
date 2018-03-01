<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpSpecialovertimeEntry.aspx.cs" Inherits="Help_Transactions_Overtime_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
     <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Entry" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/SpecialOTEntry.png" />
                </td>
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
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Reusable Images/LookupEmployee.png" />
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
                            <td valign="top">
                                <b>FROM DATE</b>: Determines the start date of Overtime for the employee. <br /><br />
                                TO DATE:  Determines the end date of Overtime for the employee.
                                To select/pick a date, a user may click this icon. <asp:Image ID="IconDatepicker" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp" /> <br />
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                                <b>Type</b>: This will determine a type of overtime. There are two types available for Overtime:
                                <ul>
                                    <li> <b>POST</b>Overtime  after the shift</li>
                                    <li> <b>ADVANCE</b> Overtime type  prior from shift</li>
                                </ul> 
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td><b>Overtime Hours</b>: Is an hour spend by the employee in addition to those regular schedule. Input the Overtime hours.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">5.</td>
                            <td><b>Reason</b>: State a reason for this entry. Please be noted that if you have leave this entry empty or any of the required information above during saving, this transaction cannot be save.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">6.</td>
                            <td>
                                <b>Shuttle Route</b>:
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">7.</td>
                            <td>
                                <b>GENERATE OVERTIME(S) button</b>: Click this to generate overtime(s) and display result.
                                <asp:Image ID="GeneratedOvertimes" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/SpecialOTEntry-GeneratedOT.png" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">
                                Note:You may do some cheking from the  generated list(s) of overtimes bofore Saving and Endorsing transaction.<br /><br />
                                Note :You may Check/Uncheck a check-box before saving and endorsing transaction. Only checked box(s) can be Save and Endorse a transaction and will disregard the unchecked box(s).
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">8.</td>
                            <td>
                                <b>SAVE AND ENDORSE button</b>: A user may click on this button to save and endorse transaction after finalizing overtime from the generated overtime(s) list.<br /><br />
                                
                                This message from WEB prompts when transaction was successfully saved and endorsed. Click OK to continue.<br />
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Reusable Images/msgSuccessfullySaveEndorsedTransaction.png" /> <br /><br />

                                This message from WEB prompts when a user has  filed an overtime(s)  that is conflicting  with a previously filed overtimes(s).  A user may have to check the range of date he/she selected for this entry(s). Click OK to continue.  <br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Transactions/Overtime/Images/OT-msgOTconflicts.png" /> <br /><br />
                            
                                <b>CLEAR Button</b>: A user may use this button to clear out some field(s) that was inputted.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>