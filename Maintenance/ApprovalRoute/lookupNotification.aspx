<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupNotification.aspx.cs" Inherits="Maintenance_ApprovalRoute_lookupNotification" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notification Settings</title>
    <style type="text/css">
        body
        {
            font-family:Arial;
            font-size:12px;
            text-transform:uppercase;
        }
        input
        {
            text-transform:uppercase;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <th>
                    <asp:Label ID="lblNotification" runat="server" Text="Notification"></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td> 
                    <asp:CheckBoxList ID="cbxNotify" runat="server" RepeatColumns="2" Width="350px" CellPadding="10" >
                        <asp:ListItem Value="ENDORSE"></asp:ListItem>
                        <asp:ListItem Value="APPROVE"></asp:ListItem>
                        <asp:ListItem Value="RETURN"></asp:ListItem>
                        <asp:ListItem Value="DISAPPROVE"></asp:ListItem>
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr>
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 35px" valign="bottom">
                    <asp:Button ID="btnSave" runat="server" Text="SAVE" Width="350px" 
                        UseSubmitBehavior="False" onclick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
