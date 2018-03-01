<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Transactions_Personnel_Default" Title="Personnel" MaintainScrollPositionOnPostback="false" EnableEventValidation="False" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlNavigation" runat="server" ScrollBars="Vertical" Height="400px" Width="225px">
                <table cellpadding="0" cellspacing = "0">
                    <tr>
                        <td>
                            <asp:Button ID="btnTaxCivil" runat="server" Text="TAX / CIVIL STATUS UPDATE" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeTaxCodeCivilStatus.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnTaxCivilReport" runat="server" Text="TAX / CIVIL STATUS REPORT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeTaxCodeCivilStatusReport.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnBeneficiary" runat="server" Text="BENEFICIARY UPDATE" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeBeneficiaryUpdate.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnBeneficiaryReport" runat="server" Text="BENEFICIARY REPORT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeBeneficiaryUpdateReport.aspx" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAddressPresent" runat="server" Text="PRESENT ADDRESS" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeAddressPresent.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAddressPermanent" runat="server" Text="PERMANENT ADDRESS" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeAddressPermanent.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAddressEmergency" runat="server" Text="EMERGENCY CONTACT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeAddressEmergency.aspx" Visible="false"  />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAddressReport" runat="server" Text="ADDRESS/CONTACT REPORT" Height="50px" Width="200px" CssClass="naviButton" PostBackUrl="~/Transactions/Personnel/pgeAddressReport.aspx" Visible="false"  />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblNoAccess" runat="server" Text="NO SYSTEM ACCESS GRANTED"></asp:Label>
            </asp:Panel>
        </td>
        <td valign="top">
            <asp:Panel ID="pnlInfo" runat="server" GroupingText="Personal Information" Height="400px" ScrollBars="Vertical" Width="658px">
                <table cellpadding="2" cellspacing="2">
                    <tr>
                        <th colspan="2" align="left" 
                            
                            style="border-style: outset none none none; border-width:medium; border-top-color: #808080;">
                            <asp:Label ID="Label21" runat="server" Text="Tax Code ad Civil Status"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTax" runat="server" Text="Tax Code"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtTaxCode" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Civil Status"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtCivilStatus" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2" align="left" 
                            
                            style="border-style: outset none none none; border-width:medium; border-top-color: #808080;">
                            <asp:Label ID="Label6" runat="server" Text="Present Address"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td valign="top" >
                            <asp:Label ID="Label2" runat="server" Text="Address 1"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresent1" runat="server" TextMode="MultiLine" Width="500px" ReadOnly="true" Height="40px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Address 2"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresent2" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Address 3"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresent3" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Tel. No."></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresentTelNo" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Mobile No."></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresentMobileNo" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Email"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPresentEmail" runat="server" Width="500px" ReadOnly="true" CssClass="textareaNormal"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2" align="left" 
                            style="border-style: outset none none none; border-top-width: medium; border-top-color: #808080;">
                            <asp:Label ID="Label5" runat="server" Text="Permanent Address"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label7" runat="server" Text="Address 1"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPermanent1" runat="server" TextMode="MultiLine" Width="500px" ReadOnly="true" Height="40px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Address 2"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPermanent2" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Address 3"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPermanent3" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Tel. No."></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressPermanentTelNo" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2" align="left" 
                            style="border-style: outset none none none; border-top-width: medium; border-top-color: #808080;">
                            <asp:Label ID="Label14" runat="server" Text="In Case of Emergency"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label19" runat="server" Text="Contanct"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICEContact" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label20" runat="server" Text="Relation"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICERelation" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label15" runat="server" Text="Address 1"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICE1" runat="server" TextMode="MultiLine" Width="500px" ReadOnly="true" Height="40px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Address 2"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICE2" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Address 3"></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICE3" runat="server" Width="500px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Tel. No."></asp:Label>    
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddressICETelNo" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Content>



