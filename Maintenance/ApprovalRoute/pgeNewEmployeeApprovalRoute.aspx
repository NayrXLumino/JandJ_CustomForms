<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeNewEmployeeApprovalRoute.aspx.cs" Inherits="Maintenance_ApprovalRoute_pgeNewEmployeeApprovalRoute" Title="Employee Approval Route" EnableEventValidation="false"  %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPopupControl" TagPrefix="dx" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <table id = "table1" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <table style="width: 785px">
                <tr>
                    <td valign="top">
                        <asp:Panel ID="pnlUserInfo" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" 
                                            BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." 
                                            UseSubmitBehavior="false" Width="22px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeName" runat="server" Text="Name"></asp:Label>    
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" 
                                            BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblCostcenter" runat="server" Text="Costcenter" Width="94px"></asp:Label>    
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtNickname" runat="server" Width="290px" 
                                            BackColor="Gainsboro" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                    <td valign="top">
                        <asp:Panel ID="pnlOtherInfo" runat="server" Width="360px">
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
        <td>
        </td>
    </tr>

    <tr>
        <td valign="top">
            &nbsp;</td>
                </tr>
                <tr>
                <td>
                </td>
                </tr>
     <tr style="border: 1px solid #000000">
        <td align = "center">
       
        </td>
        <td>       
        
        </td>
    </tr>
 </table>
    <asp:Panel ID="pnlOT" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
             <td align = "center">
             <asp:Label ID="lblOvertime" runat="server" Text="OVERTIME"  
                        Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" 
                     style= "margin-left: 345px"></asp:Label>
                     <asp:Button ID="btnOvertime" runat="server" Text="Add" Height= "23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small"  
                     style= "margin-left: 350px" />
                
             </td>
    </tr>
     <tr>
        <td>         

             <asp:GridView ID="dgvOvertime" runat="server" BackColor="White" 
                               BorderColor="#CCCCCC" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
             GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
             EnableModelValidation="True" AllowPaging="True" 
                        onpageindexchanging="dgvOvertime_PageIndexChanging" PageSize="5" 
                               onrowdatabound="dgvOvertime_RowDataBound" 
                               onrowdeleting="dgvOvertime_RowDeleting" 
                               onrowediting="dgvOvertime_RowEditing">
                            <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="Silver" Font-Bold="false" ForeColor="Black" 
                                   Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Start Date" HeaderText="Start Date">
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                <ControlStyle Width="70px" />
                                <HeaderStyle Width="70px" />
                                <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                            <asp:CommandField ShowEditButton = true >
                                <ControlStyle Width="20px" />
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                </asp:CommandField>
                            <asp:CommandField ShowDeleteButton = true >
                                <ControlStyle Width="30px" />
                                <HeaderStyle Width="30px" />
                                <ItemStyle Width="30px" />
                                </asp:CommandField>
                            </Columns>  
                        </asp:GridView>
        </td>
    </tr>
</table>
<hr/>
    </asp:Panel>
    
    <asp:Panel ID="pnlLV" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td  align ="center">
                 <asp:Label ID="lblLeave" runat="server" Text="LEAVE" 
                            Width = "135px" Font-Bold="True" Font-Names="Arial" 
                    Font-Size="13px" style= "margin-left: 345px"></asp:Label>
                <asp:Button ID="btnLeave" runat="server" Text="Add" Height= "23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small"  style= "margin-left: 350px" />
                </td>
        </tr>
         <tr>
         <td>
        <asp:GridView ID="dgvLeave" runat="server" BackColor="White" BorderColor="#DEDFDE" 
                 BorderStyle="Solid" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
                 GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
                 EnableModelValidation="True" AllowPaging="True" 
                 onpageindexchanging="dgvLeave_PageIndexChanging" 
                 onrowdatabound="dgvLeave_RowDataBound" 
                 onrowdeleting="dgvLeave_RowDeleting" PageSize="5" 
                 onrowediting="dgvLeave_RowEditing" >
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <ControlStyle Width="70px" />
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true >
                                    <ControlStyle Width="20px" />
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >
                                    <ControlStyle Width="30px" />
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>  
                            </asp:GridView>
          </td>
        </tr>

    </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlTR" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align = "center">
            <asp:Label ID="lblTimeRecord" runat="server" Text="TIME RECORD" style= "margin-left: 345px" 
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px"></asp:Label>
            <asp:Button ID="btnTimeRecord" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
                </td>
        </tr>
         <tr>  
         <td>
   
        <asp:GridView ID="dgvTimeRecord" runat="server" BackColor="White" 
                 BorderColor="#DEDFDE" BorderWidth="1px" CellPadding="4" 
                 ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                 AutoGenerateColumns="False" EnableModelValidation="True" 
                 AllowPaging="True" onpageindexchanging="dgvTimeRecord_PageIndexChanging" 
                 onrowdatabound="dgvTimeRecord_RowDataBound" 
                 onrowdeleting="dgvTimeRecord_RowDeleting" PageSize="5" 
                 onrowediting="dgvTimeRecord_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <ControlStyle Width="70px" />
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlWI" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align= "center">
            <asp:Label ID="lblWorkInfo" runat="server" Text="WORK INFO"  
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>      
            <asp:Button ID="btnWorkInfo" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>  
        </tr>
        <tr>
        <td>

       <asp:GridView ID="dgvWorkInfo" runat="server" BackColor="White" 
                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                AutoGenerateColumns="False" EnableModelValidation="True" 
                AllowPaging="True" onpageindexchanging="dgvWorkInfo_PageIndexChanging" 
                onrowdatabound="dgvWorkInfo_RowDataBound" 
                onrowdeleting="dgvWorkInfo_RowDeleting" PageSize="5" 
                onrowediting="dgvWorkInfo_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
           </td>
        </tr>
    </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlBE" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align="center">
            <asp:Label ID="lblBeneficiary" runat="server" Text="BENEFICIARY"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>      
            <asp:Button ID="btnBeneficiary" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>   
        <td>
  
        <asp:GridView ID="dgvBeneficiary" runat="server" BackColor="White" 
                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                AutoGenerateColumns="False" EnableModelValidation="True" 
                AllowPaging="True" onpageindexchanging="dgvBeneficiary_PageIndexChanging" 
                onrowdatabound="dgvBeneficiary_RowDataBound" 
                onrowdeleting="dgvBeneficiary_RowDeleting" PageSize="5" 
                onrowediting="dgvBeneficiary_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" Wrap="True" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlTC" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align="center">
            <asp:Label ID="lblTaxCivil" runat="server" Text="TAX / CIVIL STATUS"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>       
            <asp:Button ID="btnTaxCivil" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>
        <td>
       <asp:GridView ID="dgvTaxCivil" runat="server" BackColor="White" 
                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                AutoGenerateColumns="False" EnableModelValidation="True" 
                AllowPaging="True" onpageindexchanging="dgvTaxCivil_PageIndexChanging" 
                onrowdatabound="dgvTaxCivil_RowDataBound" 
                onrowdeleting="dgvTaxCivil_RowDeleting"  PageSize="5" 
                onrowediting="dgvTaxCivil_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlAD" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align="center">
            <asp:Label ID="lblAddress" runat="server" Text="ADDRESS"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>        
            <asp:Button ID="btnAddress" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>    
        <td>
       <asp:GridView ID="dgvAddress" runat="server" BackColor="White" BorderColor="#DEDFDE" 
                BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
                GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
                EnableModelValidation="True" AllowPaging="True" 
                onpageindexchanging="dgvAddress_PageIndexChanging" 
                onrowdatabound="dgvAddress_RowDataBound" 
                onrowdeleting="dgvAddress_RowDeleting" PageSize="5" 
                onrowediting="dgvAddress_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" Wrap="True" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlMP" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align ="center">
            <asp:Label ID="lblManPower" runat="server" Text="MAN POWER"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>     
            <asp:Button ID="btnManPower" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Bold="False" Font-Names="Arial" 
                    Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>   
        <td>
       <asp:GridView ID="dgvManPower" runat="server" BackColor="White" 
                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                AutoGenerateColumns="False" EnableModelValidation="True" 
                AllowPaging="True" onpageindexchanging="dgvManPower_PageIndexChanging" 
                onrowdatabound="dgvManPower_RowDataBound" 
                onrowdeleting="dgvManPower_RowDeleting" PageSize="5" 
                onrowediting="dgvManPower_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlTN" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align = "center">
            <asp:Label ID="lblTraining" runat="server" Text="TRAINING"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>   
            <asp:Button ID="btnTraining" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>
        <td>
       <asp:GridView ID="dgvTraining" runat="server" BackColor="White" 
                BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                ForeColor="Black" GridLines="Vertical" Width="881px" Font-Size="11px" 
                AutoGenerateColumns="False" EnableModelValidation="True" 
                AllowPaging="True" onpageindexchanging="dgvTraining_PageIndexChanging" 
                onrowdatabound="dgvTraining_RowDataBound" 
                onrowdeleting="dgvTraining_RowDeleting" PageSize="5" 
                onrowediting="dgvTraining_RowEditing">
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#999999" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="False" ForeColor="Black" 
                                    Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >  
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>
                            </asp:GridView>
                            </td>
        </tr>
        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlJB" runat="server">
        <table style="border: 1px solid #000000">
         <tr>
            <td align = "center">
            <asp:Label ID="lblJobsplit" runat="server" Text="JOBSPLIT"
                    Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px" style= "margin-left: 345px"
                ></asp:Label>   
            <asp:Button ID="btnJobsplit" runat="server" Text="Add" Height="23px" 
                    CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
            </td>
        </tr>
        <tr>
            <td>
                               <asp:GridView ID="dgvJobsplit" runat="server" BackColor="White" BorderColor="#CCCCCC" 
                 BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
                 GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
                 EnableModelValidation="True" AllowPaging="True" 
                            onpageindexchanging="dgvJobsplit_PageIndexChanging" PageSize="5" 
                                   onrowdatabound="dgvJobsplit_RowDataBound" 
                                   onrowdeleting="dgvJobsplit_RowDeleting" 
                                   onrowediting="dgvJobsplit_RowEditing" >
                                <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                                <FooterStyle BackColor="#CCCC99" />
                                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="Silver" Font-Bold="false" ForeColor="Black" 
                                       Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                    <ControlStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                    <ControlStyle Width="70px" />
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                    <ControlStyle Width="200px" />
                                    <HeaderStyle Width="200px" />
                                    <ItemStyle Width="200px" />
                                    </asp:BoundField>
                                <asp:CommandField ShowEditButton = true >
                                    <ControlStyle Width="20px" />
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    </asp:CommandField>
                                <asp:CommandField ShowDeleteButton = true >
                                    <ControlStyle Width="30px" />
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle Width="30px" />
                                    </asp:CommandField>
                                </Columns>  
                            </asp:GridView>
            </td>
        </tr>

        </table>
    <hr/>
    </asp:Panel>
    <asp:Panel ID="pnlSW" runat="server">
    <table style="border: 1px solid #000000">
    <tr>
        <td align = "center">
        <asp:Label ID="lblStraightWork" runat="server" Text="STRAIGHT WORK" style= "margin-left: 345px"
                Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px"
            ></asp:Label>   
        <asp:Button ID="btnStraightWork" runat="server" Text="Add" Height="23px" 
                CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
        </td>
    </tr>
    <tr>
        <td>
                           <asp:GridView ID="dgvStraightWork" runat="server" BackColor="White" BorderColor="#CCCCCC" 
             BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
             GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
             EnableModelValidation="True" AllowPaging="True" 
                        onpageindexchanging="dgvStraightWork_PageIndexChanging" PageSize="5" 
                               onrowdatabound="dgvStraightWork_RowDataBound" 
                               onrowdeleting="dgvStraightWork_RowDeleting" 
                               onrowediting="dgvStraightWork_RowEditing" >
                            <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="Silver" Font-Bold="false" ForeColor="Black" 
                                   Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                <ControlStyle Width="70px" />
                                <HeaderStyle Width="70px" />
                                <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                            <asp:CommandField ShowEditButton = true >
                                <ControlStyle Width="20px" />
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                </asp:CommandField>
                            <asp:CommandField ShowDeleteButton = true >
                                <ControlStyle Width="30px" />
                                <HeaderStyle Width="30px" />
                                <ItemStyle Width="30px" />
                                </asp:CommandField>
                            </Columns>  
                        </asp:GridView>
        </td>
    </tr>
</table>
<hr/>
    </asp:Panel>

    <asp:Panel ID="pnlGP" runat="server">
    <table style="border: 1px solid #000000">
    <tr>
        <td align = "center">
        <asp:Label ID="lblGatePass" runat="server" Text="GATE PASS" style= "margin-left: 345px"
                Width = "135px" Font-Bold="True" Font-Names="Arial" Font-Size="13px"></asp:Label>   
        <asp:Button ID="btnGatePass" runat="server" Text="Add" Height="23px" 
                CssClass="textareaNormal" Font-Names="Arial" Font-Size="Small" style= "margin-left: 350px"/>
        </td>
    </tr>
    <tr>
        <td>
                           <asp:GridView ID="dgvGatePass" runat="server" BackColor="White" BorderColor="#CCCCCC" 
             BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
             GridLines="Vertical" Width="881px" Font-Size="11px" AutoGenerateColumns="False" 
             EnableModelValidation="True" AllowPaging="True" 
                        onpageindexchanging="dgvGatePass_PageIndexChanging" PageSize="5" 
                               onrowdatabound="dgvGatePass_RowDataBound" 
                               onrowdeleting="dgvGatePass_RowDeleting" 
                               onrowediting="dgvGatePass_RowEditing" >
                            <RowStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="Silver" Font-Bold="false" ForeColor="Black" 
                                   Font-Size="12px" BorderStyle="Solid" BorderWidth="1px" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Start Date" HeaderText="Start Date" >
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="End Date" HeaderText="End Date" >
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Route ID" HeaderText="Route ID" >
                                <ControlStyle Width="70px" />
                                <HeaderStyle Width="70px" />
                                <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 1" HeaderText="Checker 1" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Checker 2" HeaderText="Checker 2" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Approver" HeaderText="Approver" >
                                <ControlStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                                </asp:BoundField>
                            <asp:CommandField ShowEditButton = true >
                                <ControlStyle Width="20px" />
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                </asp:CommandField>
                            <asp:CommandField ShowDeleteButton = true >
                                <ControlStyle Width="30px" />
                                <HeaderStyle Width="30px" />
                                <ItemStyle Width="30px" />
                                </asp:CommandField>
                            </Columns>  
                        </asp:GridView>
        </td>
    </tr>
</table>
    </asp:Panel>
    <asp:HiddenField ID="hfRouteID" runat="server" />
    <asp:HiddenField ID="hfStartDate" runat="server" />
    <asp:HiddenField ID="hfC1" runat="server" />
    <asp:HiddenField ID="hfC2" runat="server" />
    <asp:HiddenField ID="hfAP" runat="server" />
    <asp:HiddenField ID="hfEndDate" runat="server" />
    <asp:HiddenField ID="hfEmployeeID" runat="server" />
    <asp:HiddenField ID="hfEmployeeName" runat="server" />
    <asp:HiddenField ID="hfCostCenter" runat="server" />
</asp:Content>

