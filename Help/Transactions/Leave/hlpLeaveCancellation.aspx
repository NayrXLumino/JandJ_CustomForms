<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpLeaveCancellation.aspx.cs" Inherits="Help_Transactions_Leave_Default4" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveCancellation.png" />
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">1.</td>
                <td>
                    <b>Control No</b>: or Transaction Control No., is an auto generated  number for every transaction made by user within the system. It is unique in all kind of transaction.
                    <i>For this transaction, you will need to select a control no from the above list of filed leave entry(s).</i>
                    <ul>
                        <li>You may use the search box provided for you to search for specific detail.</li>
                        <li>Select a row from the list. (Please be noted that selected row is highlighted with red color background)</li>
                    </ul>
                    <b>Remarks</b>: A suer must put a remarks for this cancellation. (Please be noted that if you leave remarks empty, you won't be able to proceed with this cancellation.
                </td>
            </tr>
            <tr><td colspan="2"><hr /></td></tr>
            <tr>
                <td class="number" valign="top">2.</td>
                <td>
                    <b>CANCEL button</b>: Click this button to canel filed leave entry.<br /><br />
                    This message from a web prompts after clicking "CANCEL" button.<br /><br />
                    <asp:Image ID="img1" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveCancellation-msgConfirmTransaction.png" />
                    <ul>
                        <li>You may click <b>OK</b> button to proceed with the cancellation.</li>
                        <li>You may <b>Cancel</b> this transaction and proceed it later.</li>
                    </ul>
                    This message from a WEB prompts when you choose <b>OK</b> from a confimation message. This means that Leave was cancelled successfully. Click OK to continue.<br /><br />
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveCancellation-msgSuccessfullyCancelled.png" />
                </td>
            </tr>
            <tr>
                <td class="number" valign="top">3.</td>
                <td>
                    <b>LIST button</b>: This will allow the user to see all filed leave entry(s). As shown from the <a href=""><u>image above</u></a>. <br /><br />
                    <b>DETAILS button</b>: This will allow the user to view Leave detail in  form-view:<br />
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Transactions/Leave/Images/LveCancellationDetailView.png" /> <br />
                </td>
            </tr>
            <tr>
                <td class="number"></td>
                <td class="note">Note: You can Cancel Leave in both view; LIST or DETAILS</td>
            </tr>
        </table>
    </div>
</asp:Content>