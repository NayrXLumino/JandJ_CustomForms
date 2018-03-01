<%@ Page Language="C#" MasterPageFile="~/pgeHELP.master" AutoEventWireup="true" CodeFile="hlpLogin.aspx.cs" Inherits="Help_Login_Default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:630px">
        <div style="width:630px">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td  align="center">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Help/Login/Images/Login.png" />
                </td>
            </tr>
            <tr><td><hr /></td></tr>
            <tr>
                <td>
                   <table style="width:100%">
                    <tr>
                        <td class="number" valign="top">1.</td>
                        <td>Input User ID. By default this would be your company ID unless specified by company to use different ID.</td>
                    </tr>
                    <tr>
                        <td valign="top">2.</td>
                        <td>Options will be filled after inputting the User ID. This will show the profile you can access, you just have to select from the dropdown list the profile you would like to access.</td>
                    </tr>
                    <tr>
                        <td valign="top">3.</td>
                        <td>Input your password. You should have been given a default password and system would advise you to change this on your first login. If you password does not comply to the password complexity it will always redirect you first to the change password page rather than your home page.</td>
                    </tr>
                    <tr>
                        <td valign="top">4.</td>
                        <td>After filling up the neccesary fields in items 1, 2, and 3. Click button to login.</td>
                    </tr>
                    <tr>
                        <td valign="top" style="border-bottom:solid 1px">5.</td>
                        <td>Announcements portion shows you the information given out by HR or whoever is in charge of making anouncements.</td>
                    </tr>
                    <tr><td colspan="2"><hr /></td></tr>
                    <tr>
                        <td colspan="2">
                            <table width="100%">
                                <tr>
                                    <td valign="top">
                                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Help/Login/Images/LoginNoProfilesRetrieved.png" />
                                    </td>
                                    <td valign="middle">This system message prompt when: User ID has no setup on what profile he or she can access. You can approach person in charge in your company who can grant you access.</td>
                                </tr>
                                <tr>
                                    <td valign="top" align="right">
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/Help/Login/Images/LogininvalidPassword.png" />
                                    </td>
                                    <td valign="middle">This system message prompt when: Password inputted is incorrect.</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                   </table> 
                </td>
            </tr>
        </table>
    </div>
    </div>
</asp:Content>
