<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" EnableEventValidation="true" CodeFile="pgeWorkRecord.aspx.cs" Inherits="_Default" Title="Manhour Allocation Entry" MaintainScrollPositionOnPostback="true" EnableViewState="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="contentBody" onmouseover="Spooler()" enableviewstate="true" onload="Spooler()" onprerender="Spooler()" onkeydown="disableEnter()">
        <div class="userInfo">
            <div class="userInfoLeft">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td style="width: 94px">
                                        <asp:Label ID="lblUserId" runat="server" Text="ID No."></asp:Label>
                                    </td>
                                    <td style="width: 270px" colspan="2">
                                        <asp:TextBox ID="txtEmployeeId" runat="server" Width="290px" BackColor="#f5f5f5"></asp:TextBox>
                                    </td>
                                    <%--<td style="width: 70px">--%>
                                        <asp:Button ID="btnEmployeeId" runat="server" Text="..." OnClientClick="return lookupEmployee('TIMEKEEP')" UseSubmitBehavior="false" Width="22px" Visible="false"/>
                                    <%--</td>--%>
                                </tr>
                                <tr>
                                    <td style="width: 94px">
                                        <asp:Label ID="lblUserFname" runat="server" Text="Name"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtEmployeeName" runat="server" Width="290px" BackColor="#f5f5f5" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 94px">
                                        <asp:Label ID="lblUserIdCode" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtNickname" runat="server" Width="290px" BackColor="#f5f5f5" ></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="vertical-align:middle; width: 403px;">
                            <asp:Panel style="POSITION: relative; left: 0px; top: 0px;" id="pnlUnsplittedHours" runat="server" Width="400px" Visible="false" GroupingText="Reminder">
                                <table style="height:40px; margin:auto;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblUnsplittedHours" runat="server" Font-Bold="True" Font-Size="10pt" style="color:#1a5891"></asp:Label>
                                            <asp:Label ID="lblUnsplittedHoursValue" runat="server" Font-Bold="True" Font-Size="12pt" style="color:#1a5891"></asp:Label>
                                            <asp:Label ID="lblUnsplittedHoursDate" runat="server" Font-Bold="True" Font-Size="10pt" style="color:#1a5891"></asp:Label>
                                            <asp:Label ID="lblUnsplittedHoursDateValue" runat="server" Font-Bold="True" Font-Size="12pt" style="color:#1a5891"></asp:Label>
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="10pt" style="color:#1a5891" Text="."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    
                </table>
            </div>

        </div>
        <div class="hr">
            <hr />
        </div>
        <div class="workEntry">
            <table width="880px" style="height: 385px">
                <tr>
                    <td colspan="3">
                        <table width="880px" border="0">
                            <tr>
                                <td style="width: 95px; height: 22px;">
                                    <asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label>
                                </td>
                                <td style="width: 155px; height: 22px;">
                                    <asp:TextBox ID="txtDate" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td style="height: 22px; width:100px;">
                                    <asp:Label ID="lblShift" runat="server" Text="Shift of the Day"></asp:Label>
                                </td>
                                <td style="height: 22px">
                                    <asp:TextBox ID="txtShift" runat="server" Width="200px" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td style="height: 22px">
                                    <table width="100%" border="0px">
                                        <tr>
                                            <td style="width: 47px; height: 24px">
                                                <asp:Label ID="lblStatus" runat="server" Text="Status" Visible="false"></asp:Label>
                                            </td>
                                            <td align="right" style="height: 24px">
                                                <asp:TextBox ID="txtStatus" runat="server" BackColor="WhiteSmoke" ReadOnly="True" Visible="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                           </tr>
                           <tr>
                               <td style="width: 95px">
                                     <asp:Label ID="lblDOW" runat="server" Text="Day of the Week"></asp:Label>
                               </td>
                               <td style="width: 155px">
                                   <asp:TextBox ID="txtDOW" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                               </td>
                                <td style="width: 280px; margin:0px; padding:0; vertical-align:top;" colspan="2" rowspan="2">
                                    <table style="width: 280px; margin:0px; padding:0;" border="0px">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTimeIn" runat="server" Text="Time In 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text="Time Out 1"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" Text="Time In 2"></asp:Label>
                                            </td>
                                            <td style="width: 68px">
                                                <asp:Label ID="Label3" runat="server" Text="Time Out 2"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox Width="60px" ID="txtTimeIn" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox Width="60px" ID="txtTimeOut" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox Width="60px" ID="txtTimeIn2" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                            </td>
                                            <td style="width: 68px">
                                                <asp:TextBox Width="60px" ID="txtTimeOut2" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <table width="100%" border="0px">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPLeave" runat="server" Text="Paid Leave"></asp:Label></td>
                                            <td align="right">
                                            <asp:TextBox ID="txtPLeave" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                </td>                                
                           </tr> 
                           <tr>
                                <td style="width: 95px;">
                                    <asp:Label ID="lblDay" runat="server" Text="Day Code"></asp:Label>
                                </td>
                                <td style="width: 155px;">
                                    <asp:TextBox ID="txtDay" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                </td>
                                <td>
                                    <table width="100%" border="0px">   
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblNPLeave" runat="server" Text="Unpaid Leave"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:TextBox ID="txtNPLeave" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td> 
                           </tr>
                        </table>
                    </td>
                </tr>
               <tr>
                    <td valign="top" style="width:99%; height: 95px;" colspan="3">
                            &nbsp;&nbsp;
                        <input type="button" name="" value="Add" onclick="Addk('tst');" style="height: 20px" accesskey="a" id="Button1"/>
                        <input type="button" name="" value="Delete" onclick="deleteRowk('tst');" style="height: 20px" accesskey="d"/>
                        <input type="button" name="" value="ALLOCATE" style="height: 20px" onclick="split('tst')" accesskey="t"/>
                      <br />
                      <script type="text/javascript"  language="javascript" src="Javascript/autoTime.js"></script>
                        <table id="tst" border="0" style="width:880px; text-align: center" onkeypress="return isNumberJS(event)" > <%-- onkeyup="formatCompute()" --%>
                            <tbody>
                                <tr>
                                    <th style="width: 18px;">
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 51px;">
                                        Start Time
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 51px;">
                                        End Time
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 52px;">
                                        Work Hours
                                    </th> 
                                    <th style="width: 21px">
                                    </th> 
                                    <th style="text-align: center; font-weight: normal; width: 77px;">
                                        CPH Job No.
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 70px;">
                                        Client Job No.
                                    </th>
                                    <th style="text-align: center; font-weight: normal; width: 246px;" align="center">
                                        Client Job Name&nbsp;</th>
                                    <th style="text-align: center; font-weight: normal; width: 50px;" 
                                        align="center">
                                        </th>                
                                    <th style="text-align: center; font-weight: normal; width: 168px;" 
                                        align="center">
                                        Work Act. Code</th>
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
                                    <td style="text-align: center; width: 51px; height: 41px;" valign="top">
                                        <asp:TextBox ID="txtJStart" runat="server"  Width="39px" BackColor="White" EnableViewState="true" MaxLength="5" AccessKey="2"></asp:TextBox>  
                                    </td>
                                    <td style="text-align: center; width: 51px; height: 41px;" valign="top">
                                        <asp:TextBox ID="txtJEnd" runat="server"  Width="38px" BackColor="WhiteSmoke" EnableViewState="true" MaxLength="5" AccessKey="3"></asp:TextBox>  
                                    </td>
                                    <td style="text-align:center; width: 52px; height: 41px;" valign="top">
                                        <asp:TextBox ID="txtJHours" CssClass="align-right" runat="server" Width="36px" 
                                            EnableViewState="true" MaxLength ="5" BackColor="White" AccessKey="4"></asp:TextBox>
                                    </td>
                                    <td style="text-align:center; width: 21px; height: 41px;" valign="top"> 
                                       <%-- <asp:Button ID="btnJob" runat="server" Text="..." Height="21px"  AccessKey="5" 
                                            BackColor="ActiveBorder" Font-Bold="True" Width="24px"/>--%>
                                        <asp:ImageButton ID="btnJob" runat="server" ImageUrl="~/Images/search.PNG" 
                                            Height="20px" />
                                    </td>
                                    <td valign="top" style="width: 77px; height: 41px;">
                                        <asp:TextBox ID="txtJJobCode" runat="server"  Width="74px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td valign="top" style="width: 70px; height: 41px;" align="center">
                                        <asp:TextBox ID="txtJClientNo" runat="server"  Width="73px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td style="text-align: center; width: 246px; height: 41px;" valign="top">
                                        <asp:TextBox ID="txtJClientName" runat="server"  Width="222px" BackColor="#F3F3F3" EnableViewState="true"></asp:TextBox>  
                                    </td>
                                    <td style="width: 168px; height: 41px;" valign="top">
                                        <%--<asp:Button ID="btnSubWork" runat="server" Text="..." Height="21px" 
                                            UseSubmitBehavior="False" BackColor="ActiveBorder" Font-Bold="True" 
                                            Width="24px" />--%>
                                        <asp:ImageButton ID="btnSubWork" runat="server" ImageUrl="~/Images/search.PNG" 
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
                                    <td style="height: 41px">
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
                                <td style="width: 127px" align="right">
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
                   <td>
                   <table><tr>
                    <td style="padding: 0px; width: 200px; margin: 0px; " align="center" 
                           valign="bottom">
                        <asp:Button ID="btnZ" runat="server" Text="Save" Height="51px" Width="81px" OnClick="btnZ_Click" AccessKey="s" /><br />
                        <asp:Button ID="btnX" runat="server" Text="Submit" Height="26px" Width="81px" 
                            OnClick="btnX_Click" AccessKey="b" />
                    </td> 
                   <td style="width: 107px" align="right" valign="bottom">
                       <input id="btnRecent" type="button" value="Load Job" onclick="getRecent()" style="height: 72px; width: 80px;" accesskey="l" />
                   </td> 
                    <td style="width: 500px; height:90px" align="left" >
                        <asp:Panel ID="Panel1" runat="server" GroupingText="Recent Job(s)" Height="82px" Width="502px" >
                            <asp:ListBox ID="listBox" runat="server" AccessKey="r" CssClass="exclude" 
                                Height="80px" SelectionMode="Single" Width="490px"></asp:ListBox>
                        </asp:Panel>
                    </td>
                   
                   </tr></table>
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
            <asp:HiddenField ID="hfMaxHours" runat="server" />
            <asp:HiddenField ID="hfMinHours" runat="server" />
            <asp:HiddenField ID="hfOTFRAC" runat="server" />
            <asp:HiddenField ID="hfEditable" runat="server" Value="1" />
            <asp:HiddenField ID="hfEmployeeCostCenter" runat="server" />
    </div>
</div>
</asp:Content>

