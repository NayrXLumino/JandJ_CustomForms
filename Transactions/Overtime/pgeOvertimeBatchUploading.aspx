<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeOvertimeBatchUploading.aspx.cs" Inherits="Transactions_Overtime_pgeOvertimeBatchUploading" Title="Batch Overtime Uploading" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
       .loadingDiv
        {
	        width:100px;
            height:100px;
            position:relative;
            margin:200px auto 0;
            background-image:url('../../Images/loading.gif');
            background-repeat:no-repeat;
            z-index:100;
        }
    </style>

    <script type="text/javascript">
     function GetClipBoardText() {
         if (window.clipboardData) {
             document.getElementById('ctl00_ContentPlaceHolder1_hfText').value = window.clipboardData.getData("Text");
         }
     }
     function showDiv() {
         document.getElementById('divLoadingPanel').style.display = 'block';
         window.setTimeout(partB, 100000)
         //sleep(3000000, foobar_cont);

     }
     function partB() {
         document.getElementById('divLoadingPanel').style.display = 'none';
     }
     function sleep(millis, callback) {
         setTimeout(function ()
         { callback(); }
            , millis);
     }
     function foobar_cont() {
         //console.log("finished.");
     };
     sleep(300000, foobar_cont);
    </script>
        <div id="divLoadingPanel" style="display:none;position:fixed; top:0; left:0; width:100%; height:100%; background-color:#CCEEEE;opacity:0.5;filter:alpha(opacity=50); ">
        <div class="loadingDiv">                    
        </div> 
    </div>
<table>
    <tr>
        <td>
        </td>
    </tr>

   
    <tr>
    <td width="700px">
    <asp:Label ID="Label2" runat="server" Text="*Required" ForeColor="Red"></asp:Label>
    </td>
    </tr>
     <tr>
        <td>
        </td>
    </tr>
    
    <tr>
        <td width="500px" align ="right">
                <asp:Button ID="btnAdd" runat="server" Text="Add Row" Width="170px" 
                onclick="btnAdd_Click" />
        </td>
        <td>
            <asp:Button ID="btnPaste" runat="server" Text="Paste Clipboard Data"
                Width="170px" 
                OnClientClick = "javascript:GetClipBoardText();" onclick="btnPaste_Click"/>
        </td>
        <td>
            <asp:Button ID="btnClear" runat="server" Text="Clear All Rows" Width="170px" 
                onclick="btnClear_Click" />
        </td>  
        
        <td align ="right">
        <asp:Button ID="btnUploadEndorse" runat="server" Text="UPLOAD AND ENDORSE"  Width="170px" OnClientClick="showDiv()"
                onclick="btnUploadEndorse_Click" /> 
        </td>
        <td>
        <asp:Button ID="btnExport" runat="server" Text="Export Grid" 
                Width="170px" onclick="btnExport_Click"/>
        </td>
    </tr>
    <tr>
    <td>
    </td>
    </tr>
</table>

<asp:Panel ID="pnlUploadTable" runat="server" Width="900px" ScrollBars="Both" Height="450px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" >
<table rules = "all" border = "1" style = "border-collapse:collapse;" id = "tblCols" runat = "server">
    <tr>
        <th scope = "col" style = "width: 150px;">
            Employee ID*
        </th>
        <th style = "width: 150px;">
         Date of Overtime*
        </th>
        <th style = "width: 150px;">
         Overtime Type*
        </th>
        <th style = "width: 100px;">
         Start Time*
        </th>
        <th style = "width: 100px;">
         End Time
        </th>
        <th style = "width: 100px;">
         Hours
        </th>
        <th style = "width: 250px;">
         Reason*
        </th>
        <th style = "width: 160px;" runat = "server" id = "thFiller1" visible ="false">
         Fille1
        </th>
        <th style = "width: 160px;" runat = "server" id = "thFiller2" visible ="false">
         Filler2
        </th>
        <th style = "width: 160px;" runat = "server" id = "thFiller3" visible ="false">
         Filler3
        </th>
        <th style = "width:250px;">
         Remarks
        </th>
    </tr> 
    <tr>
        <td scope = "col" style = "width: 150px; height: 10px;">   <%--EmployeeID--%>     
        </td>
        <td style = "width: 150px; height: 10px;">      <%--DateOfOT--%>  
        </td>
        <td style = "width: 150px; height: 10px;">      <%--OTType--%>   
        </td>
        <td style = "width: 100px; height: 10px;">      <%--StartTime--%>  
        </td>
        <td style = "width: 100px; height: 10px;">      <%--EndTime--%>  
        </td>
        <td style = "width: 100px; height: 10px;">      <%--Hours--%>   
        </td>
        <td style = "width: 250px; height: 10px;">      <%--Reason--%>  
        </td>
        <td style = "width: 160px; height: 10px;" runat = "server" visible = "false" id = "tdFiller1">         
        </td>
        <td style = "width: 160px; height: 10px;" runat = "server" visible = "false" id = "tdFiller2">        
        </td>
        <td style = "width: 160px; height: 10px;" runat = "server" visible = "false" id = "tdFiller3">         
        </td>        
        <td style = "width: 250px; height: 10px;">      <%--Remarks--%>   
        </td>
    </tr>
</table>
<asp:GridView ID="dgvOTUpload" runat="server" AutoGenerateColumns="False" 
        EnableModelValidation="True" 
        onrowcancelingedit="dgvOTUpload_RowCancelingEdit" 
        onrowediting="dgvOTUpload_RowEditing" 
        onrowupdating="dgvOTUpload_RowUpdating" onrowdeleting="dgvOTUpload_RowDeleting">
 <Columns>
     <asp:CommandField ShowEditButton="True" 
         EditImageUrl="~/Images/editButton.png" EditText="" ButtonType="Image" 
         CancelImageUrl="~/Images/cancelButton.png" CancelText=""
         UpdateImageUrl="~/Images/saveButton.png" UpdateText="">
     <HeaderStyle Wrap="False" />
     <ItemStyle Wrap="False" />
     </asp:CommandField>
     <asp:CommandField ShowDeleteButton="True" ButtonType="Image" 
         DeleteImageUrl="~/Images/deleteButton.png">
     <HeaderStyle Wrap="False" />
     <ItemStyle Wrap="False" />
     </asp:CommandField>
    <asp:TemplateField HeaderText="Employee ID*">
        <ItemTemplate>
            <asp:Label ID="lblEmployeeId" runat="server" Text='<%# Eval("Employee ID") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtEmployeeID" runat="server" Text='<%# Eval("Employee ID") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Date of Overtime*">
        <ItemTemplate>
            <asp:Label ID="lblDOT" runat="server" Text='<%# Eval("Date of Overtime") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtDOT" runat="server" Text='<%# Eval("Date of Overtime") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Overtime Type*">
        <ItemTemplate>
            <asp:Label ID="lblOTType" runat="server" Text='<%# Eval("Overtime Type") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtOTType" runat="server" Text='<%# Eval("Overtime Type") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Start Time*">
        <ItemTemplate>
            <asp:Label ID="lblStartTime" runat="server" Text='<%# Eval("Start Time") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtStartTime" runat="server" Text='<%# Eval("Start Time") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="End Time">
        <ItemTemplate>
            <asp:Label ID="lblEndTime" runat="server" Text='<%# Eval("End Time") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtEndTime" runat="server" Text='<%# Eval("End Time") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Hours">
        <ItemTemplate>
            <asp:Label ID="lblHours" runat="server" Text='<%# Eval("Hours") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtHours" runat="server" Text='<%# Eval("Hours") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Reason*">
        <ItemTemplate>
            <asp:Label ID="lblReason" runat="server" Text='<%# Eval("Reason") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtReason" runat="server" Text='<%# Eval("Reason") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler1">
        <ItemTemplate>
            <asp:Label ID="lblFiller1" runat="server" Text='<%# Eval("Filler1") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller1" runat="server" Text='<%# Eval("Filler1") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler2">
        <ItemTemplate>
            <asp:Label ID="lblFiller2" runat="server" Text='<%# Eval("Filler2") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller2" runat="server" Text='<%# Eval("Filler2") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler3">
        <ItemTemplate>
            <asp:Label ID="lblFiller3" runat="server" Text='<%# Eval("Filler3") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller3" runat="server" Text='<%# Eval("Filler3") %>' Width = "200px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Remarks">
        <ItemTemplate>
            <asp:Label ID="lblRemarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
        </ItemTemplate>
       <EditItemTemplate>
            <asp:TextBox ID="txtRemarks" runat="server" Text='<%# Eval("Remarks") %>' Width = "200px" Height = "10px" ReadOnly = "true"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="200px"/>
    </asp:TemplateField>
    </Columns>
   
    </asp:GridView>
</asp:Panel>

<table>
<tr>
<td>
</td>
</tr>
<tr>
<td>
 <asp:Label ID="lblRowCount" runat="server" Text="0 row(s)" ForeColor="Black" 
        Font-Bold="True"></asp:Label>
</td>
</tr>
</table>
<asp:HiddenField ID="hfText" runat="server"/>
<asp:HiddenField ID="hfColumns" runat="server" />
</asp:Content>
