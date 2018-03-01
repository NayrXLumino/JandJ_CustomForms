<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" EnableEventValidation="true" CodeFile="pgeJobSplitMod.aspx.cs" Inherits="_Default" Title="Manhour Allocation Modification" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="contentBody" onmouseover="Spooler()" enableviewstate="true" onload="Spooler()" onprerender="Spooler()" onkeydown="disableEnter()">
        <div class="userInfo">
            <table>
                <tr>
                    <td>
                        <div>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblUserId" runat="server" Text="ID No."></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="260px" 
                                            BackColor="#f5f5f5" ontextchanged="txtEmployeeId_TextChanged"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('TIMEKEEP')" UseSubmitBehavior="false" Width="22px"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 83px">
                                        <asp:Label ID="lblUserFname" runat="server" Text="Name" Width="97px"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" BackColor="#f5f5f5" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 83px">
                                        <asp:Label ID="lblUserIdCode" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="#f5f5f5" ></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td>
                        <div>
                            <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="#999999" CellPadding="0" DayNameFormat="Shortest" Font-Names="Calibri" Font-Size="10px" ForeColor="Black" Height="38px" OnSelectionChanged="Calendar1_SelectionChanged" Width="395px">
                                <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                <SelectorStyle BackColor="#CCCCCC" />
                                <WeekendDayStyle BackColor="#FFFFCC" />
                                <TodayDayStyle BackColor="White" ForeColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                                <OtherMonthDayStyle ForeColor="Gray" />
                                <NextPrevStyle VerticalAlign="Bottom" />
                                <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                                <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                            </asp:Calendar>
                        </div>
                    
                    </td>
                </tr>
            </table>
        </div>
        <div class="hr">
            <hr />
        </div>
        <div class="workEntry">
            <table style="width: 880px">
                <tr>
                    <td colspan="3" style="width: 880px">
                        <table style="width: 880px" border="0">
                            <tr>
                                <td style="width: 81px; height: 31px;">
                                    <asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label>
                                </td>
                                <td style="width: 155px; height: 31px;">
                                    <asp:TextBox ID="txtDate" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td style="height: 31px">
                                    <asp:Label ID="lblShift" runat="server" Text="Shift of the Day"></asp:Label>
                                </td>
                                <td style="height: 31px">
                                    <asp:TextBox ID="txtShift" runat="server" Width="200px" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td style="height: 31px">
                                    <asp:Button ID="btnViewPrevious" runat="server" Text="View Previous Allocation" Height="20px" AccessKey="5" />
                                </td>
                           </tr>
                           <tr>
                               <td style="width: 81px">
                                   <asp:Label ID="lblDOW" runat="server" Text="Day of the Week" Width="94px"></asp:Label>
                               </td>
                               <td style="width: 155px">
                                   <asp:TextBox ID="txtDOW" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                               </td>
                                <td colspan="2" rowspan="2">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTimeIn1" runat="server" Text="Time In 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTimeOut1" runat="server" Text="Time Out 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTimeIn2" runat="server" Text="Time In 2"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTimeOut2" runat="server" Text="Time Out 2"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtTimeIn1" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTimeOut1" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTimeIn2" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="60px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTimeOut2" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td style="height: 24px">
                                                <asp:Label ID="lblPLeave" runat="server" Text="Paid Leave" Width="90px"></asp:Label>
                                            </td>
                                            <td style="height: 24px" align="right">
                                                <asp:TextBox ID="txtPLeave" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="180px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                           </tr> 
                           <tr>
                                <td style="width: 81px">
                                    <asp:Label ID="lblDay" runat="server" Text="Day Code"></asp:Label>
                                </td>
                                <td style="width: 155px">
                                    <asp:TextBox ID="txtDay" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblNPLeave" runat="server" Text="Unpaid Leave" Width="90px"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:TextBox ID="txtNPLeave" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Width="180px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                           </tr>
                           <tr>
                                <td valign="top">
                                    <asp:Label ID="Label2" runat="server" Text="Remarks"></asp:Label>
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtRemarks" runat="server" Height="55px" Width="450px" BackColor="WhiteSmoke"></asp:TextBox>
                                </td>
                                <td style="padding: 0px 2px 0px 0px" valign="top">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStatus" runat="server" Text="Status" Width="90px"></asp:Label>
                                            </td>
                                            <td style="width: 100%; padding: 0px 0px 0px 0px" align="right">
                                                <asp:TextBox ID="txtStatus" runat="server" BackColor="WhiteSmoke" 
                                                    ReadOnly="True" Width="180px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                           </tr>
                           <tr>
                                <td colspan="5">
                                    <input type="button" name="" value="Add" onclick="Addk('tst');" style="height: 20px"/>
                                    <input type="button" name="" value="Delete" onclick="deleteRowk('tst');" style="height: 20px"/>
                                    <input type="button" name="" value="Allocate" style="height: 20px" onclick="split('tst')"/>
                                </td>
                           </tr>
                        </table>
                    </td>
                </tr>
               <tr>
                    <td valign="top" style="width:880px" colspan="3">
                      <br />
                      <script type="text/javascript"  language="javascript" src="Javascript/autoTime.js"></script>
                        <table id="tst" border="0" style="width:100%; text-align: center" onkeypress="return isNumberJS(event)" > <%--onkeyup="formatCompute()"--%>
                            <tbody>
                                <tr>
                                    <th style="width: 18px; height: 22px;">
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 51px; height: 22px;">
                                        Start Time
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 51px; height: 22px;">
                                        End Time
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 52px; height: 22px;">
                                        Work Hours
                                    </th> 
                                    <th style="width: 21px; height: 22px;">
                                    </th> 
                                    <th style="text-align: center; font-weight: normal; width: 77px; height: 22px;">
                                        CPH Job No.
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 70px; height: 22px;">
                                        Client Job No.
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 190px; height: 22px;" align="center">
                                        Client Job Name&nbsp;</th>
                                    <th style="text-align: center; font-weight: normal; width: 15px; height: 22px;" align="center">
                                        </th>          
                                    <th style="text-align: center; font-weight: normal; width: 69px; height: 22px;" align="center">
                                        Work Act. Code&nbsp;</th>
                                    <th style="text-align: center; font-weight: normal; width: 201px;" 
                                        align="center" id="hideBillableHeader">
                                        Billable</th>
                                    <th style="text-align: center; font-weight: normal; width: 69px;" align="center">
                                        Overtime
                                    </th>
                                </tr>
                                <tr>
                                    <td style="width: 18px; height: 41px;" valign="top">
                                        <input type="checkbox" name="chkbox" accesskey="1"/>
                                    </td>
                                    <td style="height: 41px; text-align: center; width: 51px;" valign="top">
                                        <asp:TextBox ID="txtJStart" runat="server"  Width="39px" BackColor="White" EnableViewState="true" MaxLength="5" AccessKey="2"></asp:TextBox>  
                                    </td>
                                    <td style="height: 41px; text-align: center; width: 51px;" valign="top">
                                        <asp:TextBox ID="txtJEnd" runat="server"  Width="38px" BackColor="WhiteSmoke" EnableViewState="true" MaxLength="5" AccessKey="3"></asp:TextBox>  
                                    </td>
                                    <td style="height: 41px; text-align:center; width: 52px;" valign="top">
                                        <asp:TextBox ID="txtJHours" CssClass="align-right" runat="server" Width="36px" EnableViewState="true" MaxLength ="5" BackColor="White" AccessKey="4"></asp:TextBox>
                                    </td>
                                    <td style="height: 41px; text-align:center; width: 21px;" valign="top"> 
                                        <%--<asp:Button ID="btnJob" runat="server" Text="..." Height="21px" AccessKey="5" 
                                            BackColor="ActiveBorder" Width="24px" Font-Bold="True"/>--%>
                                        <asp:ImageButton ID="btnJob" runat="server" ImageUrl="~/Images/search.PNG" 
                                            Height="20px" />
                                    </td>
                                    <td valign="top" style="width: 77px; height: 41px;">
                                        <asp:TextBox ID="txtJJobCode" runat="server"  Width="74px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td valign="top" style="width: 70px; height: 41px;" align="center">
                                        <asp:TextBox ID="txtJClientNo" runat="server"  Width="73px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td style="height: 41px; text-align: center; width: 190px;" valign="top">
                                        <asp:TextBox ID="txtJClientName" runat="server"  Width="222px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td style="width: 15px; height: 41px;" valign="top">
                                        <%--<asp:Button ID="btnSubWork" runat="server" Text="..." Height="21px" 
                                            UseSubmitBehavior="false" BackColor="ActiveBorder" Width="24px" 
                                            Font-Bold="True" />--%>
                                         <asp:ImageButton ID="btnSubWork" runat="server" ImageUrl="~/Images/search.PNG" BackColor="ActiveBorder"
                                            Height="20px" /> 
                                    </td>
                                    <td style="width: 69px; height: 41px;" valign="top">
                                        <asp:TextBox ID="txtSubWork" runat="server" Width="87px" BackColor="WhiteSmoke"></asp:TextBox>
                                    </td>
                                    <td style="width:201px; height:41px;" valign="top" id="hideBillable10">
                                        <asp:RadioButtonList ID="rblBillable" runat="server" 
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="Y"/>
                                            <asp:ListItem Text="No" Value="N"/>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td style="text-align: center; height: 41px;" valign="top">
                                        <asp:CheckBox ID="chkOvertime" runat="server"/>
                                    </td>
                                    <td style="height: 41px; text-align: center; width: 18px;" valign="top">                                        
                                    </td>
                                    <td style="height: 41px;">
                                        <asp:HiddenField ID="hCat" runat="server" />
                                    </td>
                                    <td style="height: 41px">
                                        <asp:HiddenField ID="hCCT" runat="server" />
                                    </td>
                                    <td style="height: 41px">
                                        <asp:HiddenField ID="hSub" runat="server" />
                                    </td>
                                    <td style="height: 41px">
                                        <asp:HiddenField ID="hCFlag" runat="server" />
                                    </td>
                                    <td>
                                        <asp:HiddenField ID="hBillable" runat="server" />
                                    </td>
                                    <td>
                                        <asp:HiddenField ID="hOvertime" runat="server" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <table>
                            <tr>
                                <td style="width: 124px" align="right">
                                    <asp:Label ID="lblTotalHours" runat="server" Text="Total Work Hours"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTotalHours" CssClass="align-right" runat="server" BackColor="#FFFFC0" Width="40px"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;</td>
                            </tr>
                        </table>
                      </td>
                </tr> 
                <tr>
                    <td colspan ="3" align="right" style="width: 880px">
                        <asp:Panel ID="pnlRecent" runat="server" Height="90px" Width="622px">
                            <table>
                                <tr>
                                       <td style="width: 111px; height: 73px;" align="right" valign="bottom">
                                           <input id="btnRecent" type="button" value="Load Job" onclick="getRecent()" style="height: 71px; width: 79px;" />
                                       </td> 
                                        <td style="width: 500px; margin-right: 20px; height: 73px;" align="left">
                                            <asp:Panel ID="Panel1" runat="server" GroupingText="Recent Job(s)" Width="502px">
                                                <asp:ListBox ID="listBox" runat="server" Height="64px" Width="490px" SelectionMode="Single" CssClass="exclude">
                                                   
                                                </asp:ListBox>
                                            </asp:Panel>
                                        </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td> 
                </tr> 
               <tr>
                <td colspan ="3" style="height: 4px; width: 880px;">
                    <div class="hr">
                        <hr />
                    </div>
                </td>
               </tr> 
               <tr>
                    <td colspan="3" style="height: 30px; width: 880px;">
                        <table style="width: 99%">
                            <tr>
                                <td style="width: 206px; height: 23px;" align="right">
                                    <asp:Button ID="btnZ" runat="server" Height="22px" Width="225px" OnClick="btnZ_Click" />
                                </td>
                                <td style="width: 42px; height: 23px;" align="right">
                                    <asp:Button ID="btnX" runat="server" Height="22px" Width="225px" OnClick="btnX_Click"/>
                                </td>
                                <td style="width: 537px; height: 23px;">
                                    <asp:Button ID="btnY" runat="server" Height="22px" Width="225px" OnClick="btnY_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
               </tr> 
            </table>
            <asp:HiddenField ID="hiddenType" runat="server" />
            <asp:HiddenField ID="hiddenStatus" runat="server" />
            <asp:HiddenField ID="hiddenControl" runat="server" />
            <asp:HiddenField ID="hiddenFlag" runat="server" />
            <asp:HiddenField ID="hiddenMonth" runat="server" />
            <asp:HiddenField ID="hiddenDFlag" runat="server" Value="1" />
            <asp:HiddenField ID="hiddenRoute" runat="server"/>
            <asp:HiddenField ID="break_Start" runat="server"/>
            <asp:HiddenField ID="break_End" runat="server"/>
            <asp:HiddenField ID="hasJob_Split" runat="server"/>
            <asp:HiddenField ID="hiddenCNumber" runat="server"/>
            <asp:HiddenField ID="hiddenShift" runat="server"/>
            <asp:HiddenField ID="hfPrevCNumber" runat="server"/>
            <asp:HiddenField ID="hfMaxHours" runat="server"/>
            <asp:HiddenField ID="hfMinHours" runat="server" />
            <asp:HiddenField ID="hfFlagEntry" runat="server"/>
            <asp:HiddenField ID="hfOTFRAC" runat="server" />
            <asp:HiddenField ID="hfSave" runat="server" />
            <asp:HiddenField ID="hfEditable" runat="server" />
            <asp:HiddenField ID="hfEmployeeCostCenter" runat="server" />
            </div>
</div>
</asp:Content>

