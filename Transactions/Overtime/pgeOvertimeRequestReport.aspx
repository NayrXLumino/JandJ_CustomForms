<%@ Page Title="" Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeRequestReport.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeRequestReport" %>

<%@ Register Assembly="DevExpress.Web.ASPxEditors.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="GMDatePicker" Namespace="GrayMatterSoft" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxSplitter" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" src="_collapseDiv.js"></script>
    
<script type="text/javascript">
    function PrintPanel(action) {
        var panel = document.getElementById("<%=pnlResult.ClientID %>");
        var stringDate = document.getElementById("ctl00_ContentPlaceHolder1_dtpFrom").value;
        var d = new Date(stringDate);
        var monthNames = ["January", "February", "March", "April", "May", "June",
                        "July", "August", "September", "October", "November", "December"];
        var date = d.getDate() + "-" +  monthNames[d.getMonth()] + "-" + d.getFullYear();
        var printWindow = window.open('', '', 'resizable=1,scrollbars=yes,height=600,width=900');
       
        printWindow.document.write('<html><head><title>Overtime Request Form</title>');
        //<scr' + 'ipt type="text/javascript" src="excellentexport.js"/>
        /*printWindow.document.write('        <style>');
        printWindow.document.write('table {');
        printWindow.document.write('            border-collapse: collapse;');
        printWindow.document.write('        }');

        printWindow.document.write('        table, td, th {');
        printWindow.document.write('            border: 1px solid black;');
        printWindow.document.write('        }');
        printWindow.document.write('        </style>');*/
        printWindow.document.write(' </head><body >');
        //if (action == 'export') {
        //    //export
        //    printWindow.document.write("<a download='OvertimeRequestForm.xls' href='' onclick='return ExcellentExport.excel(this, ''tableToExcel'', ''Sheet Name Here'');'>Export to Excel</a>");
        //    //ExcellentExport.excel(this, 'tableToExcel', 'Overtime Request Form');

        //}
        printWindow.document.write('<table id="tableToExcel" > ')
        printWindow.document.write('<tr>')
        printWindow.document.write('      <td>')
        printWindow.document.write('          <table style="width:100%">')
        printWindow.document.write('             <tr>')
        printWindow.document.write('                  <td style="font-size:small"><b>')
        printWindow.document.write(document.getElementById('ctl00_ContentPlaceHolder1_hfCompanyName').value);
        printWindow.document.write('                      <br>CORPORATE MANAGEMENT DIVISION<br>HUMAN RESOURCE SECTION')
        printWindow.document.write('                 </b> </td>')
        printWindow.document.write('                  <td></td>')
        printWindow.document.write('                  <td></td>')
        printWindow.document.write('              </tr>')
        printWindow.document.write('              <tr>')
        printWindow.document.write('                  <td></td>')
        printWindow.document.write('                  <td style="text-align:center"><b style="font-size:medium">OVERTIME REQUEST FORM<b></td>')
        printWindow.document.write('                  <td></td>')
        printWindow.document.write('              </tr>')
        printWindow.document.write('              <tr>')
        printWindow.document.write('                  <td>')
        printWindow.document.write('                      <table>')
        printWindow.document.write('                          <tr>')
        printWindow.document.write('                              <td>Section:</td><td><u>')
        printWindow.document.write(document.getElementById('ctl00_ContentPlaceHolder1_hfCostcenter').value);
        printWindow.document.write('                              </u></td><td><label ID="lblSection" ></td>');
        printWindow.document.write('                          </tr>')
        printWindow.document.write('                           <tr>')
        printWindow.document.write('                              <td>Date:</td><td><u>')
        printWindow.document.write(date);
        printWindow.document.write('                          </u></td></tr>')
        printWindow.document.write('                          </table>')
        printWindow.document.write('                  </td>')
        printWindow.document.write('                  <td></td>')
        printWindow.document.write('                  <td style="text-align:right">')
        printWindow.document.write('                      <table cellpadding="2"><tr><td style="text-align:left"> Approvals:</td></tr>')
        printWindow.document.write('                          <tr>')
        printWindow.document.write('                              <td>')
        printWindow.document.write('                                  <table cellpadding="2" style="border-collapse: collapse;border: 1px solid black;" >')
        printWindow.document.write('                                      <tr style="border-collapse: collapse;border: 1px solid black;">')
        printWindow.document.write('                                          <td style="border-collapse: collapse;border: 1px solid black;">')
        printWindow.document.write('                                              AMG/MG')
        printWindow.document.write('                                          </td >')
        printWindow.document.write('                                          <td style="border-collapse: collapse;border: 1px solid black;">')
        printWindow.document.write('                                              Dept. MG')
        printWindow.document.write('                                          </td>')
        printWindow.document.write('                                            <td style="border-collapse: collapse;border: 1px solid black;">')
        printWindow.document.write('                                              GM/SGM')
        printWindow.document.write('                                          </td>')
        printWindow.document.write('                                      </tr>')
        printWindow.document.write('                                       <tr style="height:50px">')
        printWindow.document.write('                                          <td style="border-collapse: collapse;border: 1px solid black;">&nbsp;</td>')
        printWindow.document.write('                                           <td style="border-collapse: collapse;border: 1px solid black;">&nbsp;</td>')
        printWindow.document.write('                                           <td style="border-collapse: collapse;border: 1px solid black;">&nbsp;</td>')
        printWindow.document.write('                                      </tr>')
        printWindow.document.write('                                   </table>')

        printWindow.document.write('                              </td>')
        printWindow.document.write('                          </tr>')
        printWindow.document.write('                      </table>')


        printWindow.document.write('                  </td>')
        printWindow.document.write('              </tr>')

        printWindow.document.write('          </table>')
                  
        printWindow.document.write('      </td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
              
        printWindow.document.write('      <td colspan="3"> ')
        printWindow.document.write(panel.innerHTML);
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
        printWindow.document.write('     <td>')
        printWindow.document.write('         <i style="font-size:x-small;color:red">*All employees who will render overtime need to fill-up this form.</i>')
        printWindow.document.write('         <table>')
        printWindow.document.write('             <tr>')
        printWindow.document.write('                 <td>')
                              
        printWindow.document.write('                 </td>')
        printWindow.document.write('             </tr>')
        printWindow.document.write('             <tr>')
        printWindow.document.write('                 <td>')
        printWindow.document.write('                     &nbsp;<br>')
        printWindow.document.write('                 </td>')
        printWindow.document.write('             </tr>')
        printWindow.document.write('             <tr>')
        printWindow.document.write('                 <td>')
        printWindow.document.write('                   <b> <u><i> Process Flow:</u></b><br>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <u><b style="font-size:x-small">Day 1 - Plan</b></u>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section AMG/MG" style="text-align: center;" readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write('<td>&#x2192;</td>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section GM/SGM" style="text-align: center;" readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>&#x2192;</td>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section TMK Incharge" style="text-align: center;" readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>&#x2192;</td>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="GA Incharge" style="text-align: center;"  readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <u><b style="font-size:x-small">Day 2 - Actual</b></u>')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>')
        printWindow.document.write('   &nbsp;')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>')
        printWindow.document.write('     &nbsp;')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>')
        printWindow.document.write('     &nbsp;')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td colspan="2">')
        printWindow.document.write('   <div style="font-size:x-small"> (Consolidate the # of OT Employee)</div>')
        printWindow.document.write('</td>')
        printWindow.document.write('<td >')
        printWindow.document.write('    <div style="font-size:x-small">(Shuttle and Canteen Concern)</div>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('<tr>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section AMG/MG" align="center" style="text-align: center;"readonly >')
        printWindow.document.write('</td>')
        printWindow.document.write('<td>&#x2192;</td>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section GM/SGM" style="text-align: center;" readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write(' <td>&#x2192;</td>')
        printWindow.document.write('<td>')
        printWindow.document.write('    <input type="text" BorderColor="Black" value="Section TMK Incharge" style="text-align: center;"  readonly>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('</table>')
        printWindow.document.write('</td>')
        printWindow.document.write('</tr>')
        printWindow.document.write('</table>')
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        if (action == 'print') {
            setTimeout(function () {
                printWindow.print();
            }, 500);
        }
        else { //printWindow.document.write(' ExcellentExport.excel(this, "tableToExcel", "Overtime Request Form")');
            printWindow.document.write("<a download='OvertimeRequestForm.xls' href='' onclick='return ExcellentExport.excel(this, ''tableToExcel'', ''Sheet Name Here'');'>Export to Excel</a>");
        }
        
        return false;
        }
</script>
<div>
    <div class="dhtmlgoodies_question" style="width: 897px; height: 22px;">
        Filter
    </div>
    
    <div class="dhtmlgoodies_answer" style="width: 895px; height: 1px; left: 0px; top: 0px;">
        <div>
            <asp:HiddenField ID="hfCostcenter" runat="server" />
            <asp:HiddenField ID="hfCompanyName" runat="server" />
        <asp:Table ID="tblFilters" runat="server" Width="895px">
            <asp:TableRow Visible="false">
                <asp:TableCell Width="120px">
                    <asp:Label ID="Label2" runat="server" Text="Report Options"></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <dx:ASPxComboBox ID="cbxReportOptions" runat="server" Width="210px" SelectedIndex="0">
                        <Items>
                            <dx:ListEditItem Text="Overtime Request Form" Value = "OTR" />
                        </Items>
                                        
                    </dx:ASPxComboBox>
                                
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Width="120px">
                    <asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell Width="130px"> 
                    <table cellpadding ="0" cellspacing="0">
                        <tr>
                            <td>
                                <cc1:GMDatePicker ID="dtpFrom" ClientInstanceName="dtpFrom" runat="server" DateFormat="MM/dd/yyyy" CalendarFont-Names="Arial" ShowNoneButton= "false" InitialText="Select a Date" CalendarWidth="150px" Width="120px" TextBoxWidth="90" NoneButtonText="X" TodayButtonStyle-Width="20px" TodayButtonStyle-Height="20px" MonthYearDropDownStyle-Width="40px" MonthYearDropDownStyle-Font-Size="X-Small" TodayButtonText="O" BackColor="Gainsboro" CalendarTheme="Blue" ShowTodayButton="False" CalendarOffsetX="0px" CalendarOffsetY="0px" MinDate="2000-01-01" InitialValueMode="Null"> 
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
                                <%--<dx:ASPxDateEdit ID="dtpFrom" ClientInstanceName="dtpFrom" runat="server" Width="100px">
                                </dx:ASPxDateEdit>--%>
                            </td>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="-" Visible="false"></asp:Label>
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="dtpTo" runat="server" Width="100px" Enabled="false" Visible="false">
                                </dx:ASPxDateEdit>
                            </td>
                        </tr>
                    </table>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Visible="false">
                <asp:TableCell Width="120px">
                    <asp:Label ID="Label1" runat="server" Text="Cost Center"></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell ColumnSpan="2">
                    <dx:ASPxRadioButtonList ID="rblCCOption" runat="server" RepeatDirection="Horizontal" Width="200px" >
                        <Items>        
                            <dx:ListEditItem Text="Department" Value="DEP" Selected="true" />
                                            
                            <dx:ListEditItem Text="Section" Value="SEC"/>
                                            
                            <dx:ListEditItem Text="Group" Value="GRP"/>
                                            
                        </Items>    
                    </dx:ASPxRadioButtonList>
                                
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Width="120px">
                    <asp:Label ID="Label3" runat="server" Text="Batch No."></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtBatchNo" runat="server"  Width="730px"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnBatchNo" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Width="120px">
                    <asp:Label ID="Label5" runat="server" Text="Cost Center"></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtCostcenter" runat="server"  Width="730px"></asp:TextBox>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnCostcenter" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="Label8" runat="server" Text="Costcenter Line(s)"></asp:Label>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtCostcenterLine" runat="server" Width="730px"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnCostcenterLine" runat="server" Text="..." UseSubmitBehavior="false" Width="22px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tbrEmployee" >
                <asp:TableCell Width="120px">
                    <asp:Label ID="Label6" runat="server" Text="Employee"></asp:Label>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtEmployee" runat="server"  Width="730px"></asp:TextBox>
                                
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnEmployee" runat="server" Text="..." UseSubmitBehavior="False" Width="22px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Height="150px">
                
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        </div>
    </div>
    <div style="width:900px">
        <table cellpadding="0" cellspacing="0" style="width:900px">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnGenerate" runat="server" Text="Generate" Width="100px" 
                                    onclick="btnGenerate_Click" />
                            </td>
                            <td style="display:none">
                                <asp:Button ID="btnExport" runat="server" Text="Export" Width="100px" 
                                    OnClientClick = "return PrintPanel('export');"/>
                            </td>
                            <td>
                                <asp:Button ID="btnPrint" runat="server" Text="Print" Width="100px" 
                                    OnClientClick = "return PrintPanel('print');" />
                            </td>
                        </tr>
                    </table>    
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label4" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" Width="300px" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlResult" runat="server" ScrollBars="Both" Width="896px" Height="400px" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px">
                        <asp:Table ID="tblResult" runat="server" CellPadding="2" CellSpacing="0" BorderColor="Black" Width="896px">
                                
                        </asp:Table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
</div>

</asp:Content>

