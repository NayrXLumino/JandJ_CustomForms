<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpChangePassword.aspx.cs" Inherits="Help_Tools_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Tools/Password/Images/ChangePassword.png" />
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
                                Fill-out all informations required to change a password. <br /><br />
                                <b>Current Password</b>
                                <ul><li>input here your current password. </li></ul>
                                <b>New Password</b>
                                <ul><li>input here a new password. </li></ul>
                                <b>Confirm Password</b>
                                <ul><li>confirm you new password. It should be matched on the New password you have entered.</li></ul>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr /></td></tr>
                        <tr>
                            <td class="number" valign="top">3.</td>
                            <td>
                               Once you have done filing all the required informations, click now the <b>"SAVE"</b> button to update changes you have made. <br /><br />
                               This message for WEB propmts if update have been made successfully. Click OK to continue. <br /><br />
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Tools/Password/Images/ChangePass-msgSuccessfullyUpdated.png" /> <br /><br />
                                This messasge from WEB prompts if you have entered an invalid current password. Click OK to continue. <br /><br />
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Tools/Password/Images/ChangePass-msgInvalidCurrentPass.png" /> <br /><br /> 
                                This message from WEB prompts if New and Confirm password did not match. Click OK to continue. <br /><br />
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Tools/Password/Images/ChangePass-msgConfirmPassDidNotMatch.png" />
                            </td>
                        </tr>         
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>