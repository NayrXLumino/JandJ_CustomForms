<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupBeneficiary.aspx.cs" Inherits="Transactions_Personnel_lookupBeneficiary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Beneficiary Lookup</title>
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
    <script type="text/javascript">
        function SendValueToParent() 
        {
            var valLastname = document.getElementById('hfLastname').value;
            var valFirstname = document.getElementById('hfFirstname').value;
            var valMiddlename = document.getElementById('hfMiddlename').value;
            var valOccupation = document.getElementById('hfOccupation').value;
            var valCompany = document.getElementById('hfCompany').value;
            var valGender = document.getElementById('hfGender').value;
            var valCivilStatus = document.getElementById('hfCivilStatus').value;
            var valBirthdate = document.getElementById('hfBirthdate').value;
            var valRelationCode = document.getElementById('hfRelationCode').value;
            var valRelationDesc = document.getElementById('hfRelationDesc').value;
            var valHierarchyCode = document.getElementById('hfHierarchyCode').value;
            var valHierarchyDesc = document.getElementById('hfHierarchyDesc').value;
            var valHMO = document.getElementById('hfHMO').value;
            var valInsurance = document.getElementById('hfInsurance').value;
            var valBIR = document.getElementById('hfBIR').value;
            var valAccident = document.getElementById('hfAccident').value;
            var valDeceasedDate = document.getElementById('hfDeceasedDate').value;
            var valCancelledDate = document.getElementById('hfCancelledDate').value;
            var valSeqNo = document.getElementById('hfSeqNo').value;


            window.opener.GetValueFrom_lookupBeneficiary( valLastname
                                                        , valFirstname
                                                        , valMiddlename
                                                        , valOccupation
                                                        , valCompany
                                                        , valGender
                                                        , valCivilStatus
                                                        , valBirthdate
                                                        , valRelationCode
                                                        , valRelationDesc
                                                        , valHierarchyCode
                                                        , valHierarchyDesc
                                                        , valHMO
                                                        , valInsurance
                                                        , valBIR
                                                        , valAccident
                                                        , valDeceasedDate
                                                        , valCancelledDate
                                                        , valSeqNo);
            window.close();
            return false;
        }

        function SendValueToParent2(index) 
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index, 10) + 1];

            var tbcLastname = tbr.cells[0];
            var tbcFirstname = tbr.cells[1];
            var tbcMiddlename = tbr.cells[2];
            var tbcOccupation = tbr.cells[3];
            var tbcCompany = tbr.cells[4];
            var tbcGender = tbr.cells[5];
            var tbcCivilStatus = tbr.cells[6];
            var tbcBirthdate = tbr.cells[7];
            var tbcRelationCode = tbr.cells[8];
            var tbcRelationDesc = tbr.cells[9];
            var tbcHierarchyCode = tbr.cells[10];
            var tbcHierarchyDesc = tbr.cells[11];
            var tbcHMO = tbr.cells[12];
            var tbcInsurance = tbr.cells[13];
            var tbcBIR = tbr.cells[14];
            var tbcAccident = tbr.cells[15];
//            var tbcHMO = tbr.cells[8].childNodes[0].childNodes[0];
//            var tbcInsurance = tbr.cells[9].childNodes[0].childNodes[0];
//            var tbcBIR = tbr.cells[10].childNodes[0].childNodes[0];
//            var tbcAccident = tbr.cells[11].childNodes[0].childNodes[0];
            var tbcDeceasedDate = tbr.cells[16];
            var tbcCancelledDate = tbr.cells[17];
            var tbcSeqNo = tbr.cells[18];


            window.opener.GetValueFrom_lookupBeneficiary( tbcLastname.innerHTML
                                                        , tbcFirstname.innerHTML
                                                        , tbcMiddlename.innerHTML
                                                        , tbcOccupation.innerHTML
                                                        , tbcCompany.innerHTML
                                                        , tbcGender.innerHTML
                                                        , tbcCivilStatus.innerHTML
                                                        , tbcBirthdate.innerHTML
                                                        , tbcRelationCode.innerHTML
                                                        , tbcRelationDesc.innerHTML
                                                        , tbcHierarchyCode.innerHTML
                                                        , tbcHierarchyDesc.innerHTML
                                                        , tbcHMO.innerHTML
                                                        , tbcInsurance.innerHTML
                                                        , tbcBIR.innerHTML
                                                        , tbcAccident.innerHTML
//                                                      , tbcHMO.checked.toString()
//                                                      , tbcInsurance.checked.toString()
//                                                      , tbcBIR.checked.toString()
//                                                      , tbcAccident.checked.toString()
                                                        , tbcDeceasedDate.innerHTML
                                                        , tbcCancelledDate.innerHTML
                                                        , tbcSeqNo.innerHTML);
            window.close();
            return false;
        }

        function AssignValue(index) 
        {
            var tbl = document.getElementById('dgvResult');
            var tbr = tbl.rows[parseInt(index, 10) + 1];

            var tbcLastname = tbr.cells[0];
            var tbcFirstname = tbr.cells[1];
            var tbcMiddlename = tbr.cells[2];
            var tbcOccupation = tbr.cells[3];
            var tbcCompany = tbr.cells[4];
            var tbcGender = tbr.cells[5];
            var tbcCivilStatus = tbr.cells[6];
            var tbcBirthdate = tbr.cells[7];
            var tbcRelationCode = tbr.cells[8];
            var tbcRelationDesc = tbr.cells[9];
            var tbcHierarchyCode = tbr.cells[10];
            var tbcHierarchyDesc = tbr.cells[11];
            var tbcHMO = tbr.cells[12];
            var tbcInsurance = tbr.cells[13];
            var tbcBIR = tbr.cells[14];
            var tbcAccident = tbr.cells[15];
//            var tbcHMO = tbr.cells[8].childNodes[0].childNodes[0];
//            var tbcInsurance = tbr.cells[9].childNodes[0].childNodes[0];
//            var tbcBIR = tbr.cells[10].childNodes[0].childNodes[0];
//            var tbcAccident = tbr.cells[11].childNodes[0].childNodes[0];
            var tbcDeceasedDate = tbr.cells[16];
            var tbcCancelledDate = tbr.cells[17];
            var tbcSeqNo = tbr.cells[18];


            for (var i = 1; i < tbl.rows.length; i++) 
            {
                if (i % 2 != 0) 
                {
                    tbl.rows[i].style.backgroundColor = '#F7F7DE';
                }
                else 
                {
                    tbl.rows[i].style.backgroundColor = '#FFFFFF';
                }

            }

            tbr.style.backgroundColor = '#FF2233';

            document.getElementById('hfLastname').value = tbcLastname.innerHTML;
            document.getElementById('hfFirstname').value = tbcFirstname.innerHTML;
            document.getElementById('hfMiddlename').value = tbcMiddlename.innerHTML;
            document.getElementById('hfOccupation').value = tbcOccupation.innerHTML;
            document.getElementById('hfCompany').value = tbcCompany.innerHTML;
            document.getElementById('hfGender').value = tbcGender.innerHTML;
            document.getElementById('hfCivilStatus').value = tbcCivilStatus.innerHTML;
            document.getElementById('hfBirthdate').value = tbcBirthdate.innerHTML;
            document.getElementById('hfRelationCode').value = tbcRelationCode.innerHTML;
            document.getElementById('hfRelationDesc').value = tbcRelationDesc.innerHTML;
            document.getElementById('hfHierarchyCode').value = tbcHierarchyCode.innerHTML;
            document.getElementById('hfHierarchyDesc').value = tbcHierarchyDesc.innerHTML;
            document.getElementById('hfHMO').value = tbcHMO.innerHTML;
            document.getElementById('hfInsurance').value = tbcInsurance.innerHTML;
            document.getElementById('hfBIR').value = tbcBIR.innerHTML;
            document.getElementById('hfAccident').value = tbcAccident.innerHTML;
//            document.getElementById('hfHMO').value = tbcHMO.checked.toString();
//            document.getElementById('hfInsurance').value = tbcInsurance.checked.toString();
//            document.getElementById('hfBIR').value = tbcBIR.checked.toString();
//            document.getElementById('hfAccident').value = tbcAccident.checked.toString();
            document.getElementById('hfDeceasedDate').value = tbcDeceasedDate.innerHTML;
            document.getElementById('hfCancelledDate').value = tbcCancelledDate.innerHTML;
            document.getElementById('hfSeqNo').value = tbcSeqNo.innerHTML;

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearch" runat="server" Text="Search"></asp:Label>
                            </td>
                            <td style="width: 309px">
                                <asp:TextBox ID="txtSearch" runat="server" Width="360px" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnPrev" runat="server" Text="PREV" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
                            </td>
                            <td style="width: 172px" align="center">
                                <asp:Label ID="lblRowNo" runat="server" Text="0 of 0 Row(s)" Font-Size="11px"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnNext" runat="server" Text="NEXT" UseSubmitBehavior="false" Width="54px" OnClick="NextPrevButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 197px">
                    <script language="javascript" type="text/javascript">
                        var IsPostBack = '<%=IsPostBack.ToString() %>';
                        window.onload = function () {
                            var strCook = document.cookie;
                            if (strCook.indexOf("!~") != 0) {
                                var intS = strCook.indexOf("!~");
                                var intE = strCook.indexOf("~!");
                                var strPos = strCook.substring(intS + 2, intE);
                                if (IsPostBack == 'True') {
                                    document.getElementById("<%=pnlResult.ClientID %>").scrollTop = strPos;
                                }
                                else {
                                    document.cookie = "yPos=!~0~!";
                                }
                            }
                        }
                        function SetDivPosition() {
                            var intY = document.getElementById("<%=pnlResult.ClientID %>").scrollTop;
                            //document.title = intY;
                            document.cookie = "yPos=!~" + intY + "~!";
                        }
                    </script>
                    <asp:Panel ID="pnlResult" runat="server" Height="300px" ScrollBars="Both" 
                        Width="800px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <asp:GridView ID="dgvResult" runat="server" BackColor="White" 
                            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                            ForeColor="Black" GridLines="Vertical" Width="800px" 
                            OnRowDataBound="Lookup_RowDataBound" OnRowCreated="dgvResult_RowCreated" 
                            EnableModelValidation="True">
                            <RowStyle BackColor="#F7F7DE" />
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td style="height: 35px" valign="bottom">
                    <asp:Button ID="btnSelect" runat="server" Text="SELECT" Width="124px" UseSubmitBehavior="False" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfPageIndex" runat="server" Value="0"/>
        <asp:HiddenField ID="hfRowCount" runat="server" Value="0"/>
        <asp:HiddenField ID="hfNumRows" runat="server" Value="100"/>
        <asp:HiddenField ID="hfLastname" runat="server"/>
        <asp:HiddenField ID="hfFirstname" runat="server"/>
        <asp:HiddenField ID="hfMiddlename" runat="server"/>
        <asp:HiddenField ID="hfOccupation" runat="server" />
        <asp:HiddenField ID="hfCompany" runat="server" />
        <asp:HiddenField ID="hfGender" runat="server" />       
        <asp:HiddenField ID="hfCivilStatus" runat="server" />
        <asp:HiddenField ID="hfBirthdate" runat="server"/>
        <asp:HiddenField ID="hfRelationCode" runat="server"/>
        <asp:HiddenField ID="hfRelationDesc" runat="server"/>
        <asp:HiddenField ID="hfHierarchyCode" runat="server"/>
        <asp:HiddenField ID="hfHierarchyDesc" runat="server"/>
        <asp:HiddenField ID="hfHMO" runat="server"/>
        <asp:HiddenField ID="hfInsurance" runat="server"/>
        <asp:HiddenField ID="hfBIR" runat="server"/>
        <asp:HiddenField ID="hfAccident" runat="server"/>
        <asp:HiddenField ID="hfDeceasedDate" runat="server"/>
        <asp:HiddenField ID="hfCancelledDate" runat="server"/>
        <asp:HiddenField ID="hfSeqNo" runat="server"/>        
        
    </div>
   
    </form>
</body>
</html>
