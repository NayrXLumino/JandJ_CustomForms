﻿<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpSpecialLeaveEntry.aspx.cs" Inherits="Help_Transactions_Leave_Default3" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div id="Top" style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/SpecialLeaveEntry.png" /> 
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
                            <td class="number" valign="top">2.</td>
                            <td>
                                (A.) <b>FROM Date - TO Date</b>: This will cover a number of day(s) depending to the selection of dates.-  <br />
                                (B.) Click this <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Reusable Images/callendar-icon.bmp"  /> icon to view callendar where you can pick a date. <br /><br />
                                (C.) <b>DATE/TIME Filed</b>: Displays the current date and time.
                            
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">3.</td>
                            <td>
                                <b>UNIT:IN DAYS</b>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">Note: This area can only be seen depending on the user whose currently logged.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">4.</td>
                            <td>
                                Select a type of leave by clicking the dropdown button. <br />
                                <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/DropDownCtrl-TypeOfLeave.png"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">Note: Some lists may not available to other users.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">5.</td>
                            <td>Select a category by clicking the dropdown button.</td>
                        </tr>
                        <tr>
                            <td class="number"></td>
                            <td class="note">Note: This dropdown list is only applicable depending to what type of Leave you previously selected. And may not also be applicable to other users.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number">6.</td>
                            <td>State a reaon why you are filing leave.</td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">7.</td>
                            <td>
                                By clicking the <b>"Generate Leave(s)"</b> button, a number of leaves will display inside the box depending of the range of days you provided for "FROM Date" and "TO Date". <br /><br />
                                <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/SpecialLeaveEntry-Generate.png" />
                                    <ul>
                                        <li>You may mark a check-box by clicking on it, checked or unchecked.</li>
                                        <li>Checked mark means to include in transaction. </li>
                                        <li>UnChecked mark means to exclude in transaction.</li>
                                    </ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">9.</td>
                            <td>
                                Click "<b>SAVE AND ENDORSE</b>" button when done filing. This will save and endorse transaction. <br /><br />

                                This message from the WEB prompts if transaction was successfully saved and endorsed. Click OK to continue. <br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/SpecialLeaveEntry-msgSaveEndorsed.png" /> <br /><br />
                                This message from the WEB prompts if there is no filed dates to be retieved. Click OK to continue. <br />
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/SpecialLeaveEntry-msgNoDatesForFilingRetieved.png" /> <br /><br />
                                You may use the <b>"CLEAR"</b> button to clear out some fields.
                            </td>
                        </tr> 
                    </table>               
                </td>
            </tr>

        </table>
    </div>
</asp:Content>




