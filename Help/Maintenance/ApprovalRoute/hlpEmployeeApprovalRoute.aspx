<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpEmployeeApprovalRoute.aspx.cs" Inherits="Help_Maintenance_ApprovalRoute_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
              <td colspan="2">
                  <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/EmpAppRoute.png" />
              </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
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
                    This is where you can set-up approval route for employee per transactions. You can select route by clicking on the lookup button for Route ID.  <br /><br />
                    <i>Assume you will set a route for Overtime transaction:</i>
                    <b>Lookup for Route ID</b>: <i>This pop-up window will show lists of approval route available for an employee.</i> <br /><br />
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/EmpAppRoute-LookupRouteID.png" /> 
                    <ul>
                        <li>You may use the <b>SEARCH box</b> to search for specific information.</li>
                        <li>Select a row from the list. (<i>Please be noted that selected row/information is highlighted with red color background</i>)</li>
                        <li>You may double click a row from the list to select an information directly.</li>
                        <li>You may use the <b>SELECT button to select information.</b></li>
                    </ul>
                    You can also set an email notification from this route per transaction. <i>Click the button under column Notify</i> <br /><br />
                    Email Notification: <br /><br />
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/EmpAppRoute-Notify.png" />
                    <ul>
                        <li>Put a checked mark on the check-box if you want to recieve notification from it.</li>
                        <li>If you dont want to recieve notification, just leave the check-box un-check</li>
                    </ul>
                </td>
            </tr>
            <tr><td colspan="2"><br /></td></tr>
            <tr>
                <td class="number"></td>
                <td class="note">Note: You may follow thesame set-up procedure for other transactions.</td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">3.</td>
                <td>
                    Once you have done updating routes, you may now click the <b>"SAVE"</b> button to update changes you have made.
                    Or, you may click <b>"CANCEL"</b> button if you wish not to continue with this. <br /><br />
                    This message from WEB prompts after saving updates for routes. Click OK to continue. <br /><br />
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/EmpAppRoute-msgSuccessfullyupdatedRoutes.png" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>