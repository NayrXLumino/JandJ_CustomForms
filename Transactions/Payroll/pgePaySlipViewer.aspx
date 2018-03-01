<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgePaySlipViewer.aspx.cs" Inherits="pgePaySlipViewer" MaintainScrollPositionOnPostback="true" Title="Pay Slip Viewer" EnableEventValidation="false"%>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div class="bodyContent">
        <br />
        <asp:MultiView ID="mtvPayslipViewing" runat="server" ActiveViewIndex="0">
            <asp:View ID="mtvVerify" runat="server">
                <table>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlUser" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('GENERAL')" UseSubmitBehavior="false" Width="22px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmployeeName" runat="server" Text="Name"></asp:Label>    
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="140px"></asp:Label>    
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="Gainsboro" TextMode="MultiLine" Height="30px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Enter Payslip Password" Font-Bold="true" Width="140px"></asp:Label> 
                        </td>
                        <td>
                            <asp:TextBox ID="txtPassword" runat="server" Width="290px" TextMode="Password" CssClass="textareaNormal"></asp:TextBox>
                        </td>
                        <td rowspan="5" style="width: 438px;" align="center">
                            <asp:Panel ID="pnlChargePassword" runat="server" GroupingText="Change Password" Width="405px" Visible="false">
                                <table style="text-align:left" width="100%">
                                    <tr>
                                        <td>
                                             <asp:Label ID="Label2" runat="server" Text="Current Password" ></asp:Label> 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" Width="225px" CssClass="textareaNormal">
                                            </asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                                                runat="server" ErrorMessage="*" CssClass="reqIndicator" 
                                                ControlToValidate="txtCurrentPassword" Font-Bold="true" 
                                                ValidationGroup="CHANGEPASS"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                             <asp:Label ID="Label3" runat="server" Text="New Password" ></asp:Label> 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" Width="225px" CssClass="textareaNormal"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtNewPassword" 
                                                Font-Bold="true" ValidationGroup="CHANGEPASS"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                             <asp:Label ID="Label4" runat="server" Text="Confirm Password" ></asp:Label> 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txConfirmPassword" runat="server" TextMode="Password" Width="225px" CssClass="textareaNormal"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                                ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txConfirmPassword" 
                                                Font-Bold="true" ValidationGroup="CHANGEPASS"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSave" runat="server" Text="SAVE" Width="100px" 
                                                onclick="btnSave_Click" ValidationGroup="CHANGEPASS" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnCancel" runat="server" Text="CANCEL" Width="100px" 
                                                onclick="btnCancel_Click"  />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        
                        </td>
                        <td>
                            <asp:Button ID="btnSubmit" runat="server" Text="Verify" Width="300px" 
                                Height="25px" onclick="btnSubmit_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        
                        </td>
                        <td>
                            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" 
                                UseSubmitBehavior="true" Width="300px" onclick="btnChangePassword_Click"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        
                        </td>
                        <td>
                            <asp:Button ID="btnReset" runat="server" Text="Reset Password" 
                                UseSubmitBehavior="true" Width="300px" 
                                onclientclick="javascript:return confirm('Reset password?')" 
                                onclick="btnReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height:30px">
                        
                        </td>
                        <td>
                        
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="mtvGenerate" runat="server">
                <table>
                    <tr>
                        <td style="width: 790px; height: 227px" valign="top">
                             <table style="height: 40px">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblpayPeriod" runat="server" Text="Pay Period" Width="100px"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPayPeriod" runat="server" Width="120px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnPayPeriod" runat="server" Text="..." Width="22px" Font-Size="9pt" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPayPeriodDesc" runat="server" Width="200px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnGenerate" Text="Generate" runat="server" Height="27px" OnClick="btnGenerate_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnBack" Text="BACK" runat="server" onclick="btnBack_Click" 
                                            Width="95px"/>
                                    </td>
                                </tr>
                            </table>       
                        </td>
                    </tr>
                </table>
            </asp:View>
        </asp:MultiView>
    </div>
    <asp:HiddenField ID="hiddenTType" runat="server" Value="AL" />
    <br />
    <asp:HiddenField ID="hfPrevEmployeeId" runat="server" />
    <asp:HiddenField ID="newPasswordFlag" runat="server" Value=""/>
    
</asp:Content>


