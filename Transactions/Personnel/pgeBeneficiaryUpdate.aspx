<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeBeneficiaryUpdate.aspx.cs" Inherits="Transactions_Personnel_pgeBeneficiaryUpdate" Title="Beneficiary Update" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2">
            <table>
                <tr>
                    <td valign="top">
                        <asp:Panel ID="pnlUserInfo" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeId" runat="server" Text="ID No."></asp:Label>    
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" BackColor="Gainsboro"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('PERSONNEL')" UseSubmitBehavior="False" 
                                            Width="22px" />
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
                                    <td>
                                        <asp:Label ID="lblNickName" runat="server" Text="Nickname" Width="94px"></asp:Label>    
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="Gainsboro"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                    <td valign="top">
                        <asp:Panel ID="pnlOtherInfo" runat="server" Width="460px">
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:Table ID="tblLeftControls" runat="server" Width="440px">
                <asp:TableRow ID="tbrControlNo" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txtControlNo" runat="server" ReadOnly="true" BackColor="Gainsboro" Width="200px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblOTDate" runat="server" Text="Effectivity Date" Width="90px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <cc1:GMDatePicker ID="dtpEffectivityDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:DropDownList ID="ddlType" runat="server" Width="205px">
                            <asp:ListItem Value="N">New Entry</asp:ListItem>
                            <asp:ListItem Value="U">Update Existing</asp:ListItem>
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label1" runat="server" Text="Last Name"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <div onkeydown="return fn_validateNumeric()">
                        <asp:TextBox ID="txtLastname" runat="server" Width="300px" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtLastname" Font-Bold="true"></asp:RequiredFieldValidator>
                    </div>
                     </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label2" runat="server" Text="First Name"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <div onkeydown="return fn_validateNumeric()">
                        <asp:TextBox ID="txtFirstname" runat="server" Width="300px" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtFirstname" Font-Bold="true"></asp:RequiredFieldValidator>
                    </div>
                            </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label3" runat="server" Text="Middle Name"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <div onkeydown="return fn_validateNumeric()">
                        <asp:TextBox ID="txtMiddlename" runat="server" Width="300px" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtMiddlename" Font-Bold="true"></asp:RequiredFieldValidator>
                    </div>
                            </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell>                     
                        <asp:Label ID="Label9" runat="server" Text="Occupation"></asp:Label>
                    </asp:TableCell>  
                    <asp:TableCell>               
                        <div>                       
                        <asp:TextBox ID="txtOccupation" runat="server" Width="300px" MaxLength="500"></asp:TextBox>                        
                        </div>
                    </asp:TableCell>                               
                </asp:TableRow>

                 <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label13" runat="server" Text="Company"></asp:Label>
                    </asp:TableCell>  
                     <asp:TableCell> 
                        <div>           
                        <asp:TextBox ID="txtCompany" runat="server" Width="300px" Font-Strikeout="False" Font-Underline="False" MaxLength="500"></asp:TextBox>                        
                        </div>
                    </asp:TableCell>                       
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell>
                     <asp:Label ID="Label7" runat="server" Text="Gender"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlGender" runat="server"> 
                                         <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="F">Female</asp:ListItem>
                                        <asp:ListItem Value="M">Male</asp:ListItem>                                        
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlGender" Font-Bold="true"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell>
                     <asp:Label ID="Label8" runat="server" Text="Civil Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlCivilStatus" runat="server"> 
                                        <asp:ListItem Value=""></asp:ListItem> 
                                        <asp:ListItem Value="A">Annulled</asp:ListItem>
                                        <asp:ListItem Value="L">Separated</asp:ListItem>
                                        <asp:ListItem Value="M">Married</asp:ListItem>
                                        <asp:ListItem Value="R">Re-Married</asp:ListItem>
                                        <asp:ListItem Value="S">Single</asp:ListItem>
                                        <asp:ListItem Value="W">Widower/Widowed</asp:ListItem>                                        
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="ddlCivilStatus" Font-Bold="true"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label4" runat="server" Text="Birthdate"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlMonth" runat="server">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="01">January</asp:ListItem>
                                        <asp:ListItem Value="02">February</asp:ListItem>
                                        <asp:ListItem Value="03">March</asp:ListItem>
                                        <asp:ListItem Value="04">April</asp:ListItem>
                                        <asp:ListItem Value="05">May</asp:ListItem>
                                        <asp:ListItem Value="06">June</asp:ListItem>
                                        <asp:ListItem Value="07">July</asp:ListItem>
                                        <asp:ListItem Value="08">August</asp:ListItem>
                                        <asp:ListItem Value="09">September</asp:ListItem>
                                        <asp:ListItem Value="10">October</asp:ListItem>
                                        <asp:ListItem Value="11">November</asp:ListItem>
                                        <asp:ListItem Value="12">December</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlDay" runat="server">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="01">01</asp:ListItem>
                                        <asp:ListItem Value="02">02</asp:ListItem>
                                        <asp:ListItem Value="03">03</asp:ListItem>
                                        <asp:ListItem Value="04">04</asp:ListItem>
                                        <asp:ListItem Value="05">05</asp:ListItem>
                                        <asp:ListItem Value="06">06</asp:ListItem>
                                        <asp:ListItem Value="07">07</asp:ListItem>
                                        <asp:ListItem Value="08">08</asp:ListItem>
                                        <asp:ListItem Value="09">09</asp:ListItem>
                                        <asp:ListItem Value="10">10</asp:ListItem>
                                        <asp:ListItem Value="11">11</asp:ListItem>
                                        <asp:ListItem Value="12">12</asp:ListItem>
                                        <asp:ListItem Value="13">13</asp:ListItem>
                                        <asp:ListItem Value="14">14</asp:ListItem>
                                        <asp:ListItem Value="15">15</asp:ListItem>
                                        <asp:ListItem Value="16">16</asp:ListItem>
                                        <asp:ListItem Value="17">17</asp:ListItem>
                                        <asp:ListItem Value="18">18</asp:ListItem>
                                        <asp:ListItem Value="19">19</asp:ListItem>
                                        <asp:ListItem Value="20">20</asp:ListItem>
                                        <asp:ListItem Value="21">21</asp:ListItem>
                                        <asp:ListItem Value="22">22</asp:ListItem>
                                        <asp:ListItem Value="23">23</asp:ListItem>
                                        <asp:ListItem Value="24">24</asp:ListItem>
                                        <asp:ListItem Value="25">25</asp:ListItem>
                                        <asp:ListItem Value="26">26</asp:ListItem>
                                        <asp:ListItem Value="27">27</asp:ListItem>
                                        <asp:ListItem Value="28">28</asp:ListItem>
                                        <asp:ListItem Value="29">29</asp:ListItem>
                                        <asp:ListItem Value="30">30</asp:ListItem>
                                        <asp:ListItem Value="31">31</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlYear" runat="server">
                                    </asp:DropDownList>                                              
                                </td>                                
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label5" runat="server" Text="Relationship"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtRelationshipCode" runat="server" Width="48px" BackColor="Gainsboro"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtRelationshipDesc" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnRelationship" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtRelationshipCode" Font-Bold="true" ></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label6" runat="server" Text="Hierarchy"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtHierarchyCode" runat="server" Width="48px" BackColor="Gainsboro"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHierarchyDesc" runat="server" Width="150px" BackColor="Gainsboro"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnHierarchy" runat="server" Text="..." UseSubmitBehavior="false" Width="22px"/>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtHierarchyCode" Font-Bold="true"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
        <td valign="top">
            <asp:Table ID="tblRightControls" runat="server" Width="440px">
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <table>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbxHMO" runat="server" Text="HMO Dependent"/>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbxInsurance" runat="server" Text="Insurance Dependent"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbxAccident" runat="server" Text="Overseas Accident Dependent"/>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbxBIR" runat="server" Text="BIR Dependent"/>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label11" runat="server" Text="Deceased Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <cc1:GMDatePicker ID="dtpDeceasedDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="1900-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label12" runat="server" Text="Cancel Date"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <cc1:GMDatePicker ID="dtpCancelDate" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="1900-01-01" InitialValueMode="Null"> 
                                    <CalendarDayStyle Font-Size="9pt" />
                                    <CalendarTodayDayStyle BorderWidth="1px" BorderColor="DarkRed" Font-Bold="True" Width="20px" />
                                    <CalendarOtherMonthDayStyle BackColor="Gainsboro" />
                                    <CalendarTitleStyle BackColor="#E0E0E0" Font-Names="Arial" Font-Size="9pt" />
                                    <CalendarFont Names="Arial" Size="X-Small" />
                                <TodayButtonStyle Font-Size="X-Small" Width="20px" Height="20px" />
                                <CalendarFooterStyle Width="150px" />
                                <MonthYearDropDownStyle Font-Size="X-Small" Width="50px" />
                                <CalendarDayHeaderStyle Width="150px" />
                        </cc1:GMDatePicker>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="Top">
                        <asp:Label ID="Label10" runat="server" Text="Reason"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" CssClass="reqIndicator" ControlToValidate="txtReason" Font-Bold="true"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Width="130px" VerticalAlign="Top">
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks" Width="130px"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="295px" MaxLength="200" Height="50px"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow8" runat="server">
                    <asp:TableCell ID="TableCell17" runat="server">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server">
                        <asp:TextBox ID="txtStatus" runat="server" BackColor="Gainsboro" Width="295px" ReadOnly="true"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btnZ" runat="server" Text="[btnZ]" Width="225px" OnClick="btnZ_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnY" runat="server" Text="[btnY]" Width="225px" OnClick="btnY_Click" OnClientClick="return clearControlsBen()" />
                    </td>
                    <td>
                        <asp:Button ID="btnX" runat="server" Text="[btnX]" Width="225px" OnClick="btnX_Click" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hfPrevEmployeeId" runat="server"/>
<asp:HiddenField ID="hfSeqNo" runat="server"/>
<asp:HiddenField ID="hfSaved" runat="server" Value="0"/>
<asp:HiddenField ID="hfPrevEntry" runat="server"/>

</asp:Content>

