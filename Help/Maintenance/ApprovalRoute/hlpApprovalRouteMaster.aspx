<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpApprovalRouteMaster.aspx.cs" Inherits="Help_Maintenance_ApprovalRoute_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
              <td colspan="2">
                  <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster.png" />
              </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">1.</td>
                <td>
                    <b>SEARCH</b>: You may use searching to to view specific informations from the list. <br /><br />
                    or ou may select a status type either <b>ACTIVE</b> or <b>INACTIVE</b> As shown below: <br /><br />  
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-ActiveInactive.png" /><br /><br />
                    Default is "ACTIVE"
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">2.</td>
                <td>
                    <b id="creatingNew">NEW Button</b>: Clik this button to create new Approval Route. <br /><br />
                    Required informations for Creating new Approval Route: <br /><br />
                    <b>Creating a new Approval Route</b>: <br />
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-CreatingNew.png" /> <br /><br />
                    2.1 <b id="lookupChecker1">Checker 1</b>: Click the lookup button to view and select information. <br /><br />
                    <b>lookup for Checker 1:</b> <br />
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-LookupChecker1.png" />
                    <ul>
                        <li>You may use the <b>SEARCH box</b> to search for specific information.</li>
                        <li>Select a row from the list. (<i>Please be noted that selected row/information is highlighted with red color background</i>)</li>
                        <li>You may double click a row from the list to select an information directly.</li>
                        <li>You may use the <b>SELECT button to select information.</b></li>
                    </ul>
                    To select information for <b>Checker 2, Approver, and Costcenter</b> you may follow thesame procedure from <a href="#lookupChecker1"><u>Lookup for Checker 1</u></a> <br /><br />
                    2.2 <b>STATUS</b>: Use the dropdown control to select a status either <b>ACTIVE</b> or <b>INACTIVE</b> <br /><br />
                    2.3 <b>CREATE Button</b>: Click this button once you have already fill out all required informations to create a new Approval Route. <br /><br />
                    This message from WEB prompts when you have successfully created a new Approval Route Master. Click OK to continue: <br />
                    <asp:Image ID="Image5" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-msgRoutSuccessfullyCreated.png" /> <br /><br />
                    This message from WEB prompts when Approval Route was not created successfully. You may check your entry and try saving it again later. Click OK to continue: <br />
                    <asp:Image ID="Image6" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-msgRoutAlreadyExist.png" /> <br /><br />
                    <b>CANCEL button</b>: Click this button if you wish not to continue with this transaction.
                </td>
            </tr>
            <tr><td colspan="2"><br /></td></tr>
            <tr>
                <td class="number"></td>
                <td class="note">Note: <b>Route Id</b> is auto-generated once you have created this entry successfully.</td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">3.</td>
                <td>
                    <b>EDIT</b>: Click this button if you wish to edit Approval Route from the list. <br /><br />
                    To edit Approval Route Master, select an information from the list as shown below: <br /><br />
                    <asp:Image ID="Image7" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-SelectToEdit.png" /> <br /><br />
                    
                    Selected information will then display for you to edit. <br /><br />
                    <asp:Image ID="Image8" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-SelectedInformationLoaded.png" /> <br /><br />
                    Use the lookup buttons to select informations. You may follow procedures from <a href="#creatingNew"><u>creating a new Approval Route</u></a>
                    <ul>
                        <li>Click the <b>SAVE button</b> if you wish to save the changes you have made.</li>
                        <li>Click the <b>CANCEL button</b> if you wish not to continue in this transaction.</li>
                    </ul>
                    <i>Assume you have not select an information before clicking the <b>EDIT</b> button.</i> This message from WEB prompts if there is no selected information to be updated/edit:<br /><br />
                    <asp:Image ID="Image9" runat="server" ImageUrl="~/Help/Maintenance/ApprovalRoute/Images/AppRouteMaster-msgSelectRouteFromList.png" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>