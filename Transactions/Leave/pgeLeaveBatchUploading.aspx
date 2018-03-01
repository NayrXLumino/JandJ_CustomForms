<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeLeaveBatchUploading.aspx.cs" Inherits="Transactions_Leave_pgeLeaveBatchUploading" Title="Batch Leave Uploading" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript">
        function GetClipBoardText() {
            if (window.clipboardData) {
                document.getElementById('ctl00_ContentPlaceHolder1_hfText').value = window.clipboardData.getData("Text");
            }
        }
    </script>
<table>
    <tr>
        <td>
        </td>
    </tr>

   
    <tr>
    <td width="700px">
    <asp:Label ID="Label1" runat="server" Text="*Required" ForeColor="Red"></asp:Label>
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
        <asp:Button ID="btnUploadEndorse" runat="server" Text="UPLOAD AND ENDORSE"  Width="170px" 
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
        <th scope = "col" style = "width: 132px;">
            Employee ID*
        </th>
        <th style = "width: 106px;">
         Date of Leave*
        </th>
        <th style = "width: 61px;">
         Leave Type*
        </th> 
        <th style = "width: 82px;">
         Start Time*
        </th>
        <th style = "width: 80px;">
         End Time
        </th>
        <th style = "width: 61px;">
         Hours
        </th>
        <th style = "width: 160px;">
         Reason*
        </th>
        <th style = "width: 61px;"  runat = "server" id = "thDayUnit" visible ="false">
         Day Unit
        </th>
        <th style = "width: 61px;" runat = "server" id = "thFiller1" visible ="false">
         Filler1
        </th>
        <th style = "width: 61px;" runat = "server" id = "thFiller2" visible ="false">
         Filler2
        </th>
        <th style = "width: 61px;" runat = "server" id = "thFiller3" visible ="false">
         Filler3
        </th>
        <th style = "width: 150px;">
         Remarks
        </th>
    </tr> 
    <tr>
        <td style = "width: 132px; height: 10px;">            
        </td>
        <td style = "width: 150px; height: 10px;">        
        </td>
        <td style = "width: 150px; height: 10px;">        
        </td>
        <td style = "width: 150px; height: 10px;">        
        </td>
        <td style = "width: 150px; height: 10px;">         
        </td>
        <td style = "width: 150px; height: 10px;">         
        </td>
        <td style = "width: 150px; height: 10px;" runat = "server" visible = "false" id = "tdDayUnit">        
        </td>
        <td style = "width: 150px; height: 10px;" runat = "server" visible = "false" id = "tdFiller1">        
        </td>        
        <td style = "width: 150px; height: 10px;" runat = "server" visible = "false" id = "tdFiller2">        
        </td>       
        <td style = "width: 150px; height: 10px;" runat = "server" visible = "false" id = "tdFiller3">        
        </td>       
        <td style = "width: 150px; height: 10px;">        
        </td>       
        <td style = "width: 150px; height: 10px;">        
        </td>
    </tr>
</table>
<asp:GridView ID="dgvLeaveUpload" runat="server" AutoGenerateColumns="False" 
        EnableModelValidation="True" 
        onrowcancelingedit="dgvLeaveUpload_RowCancelingEdit" 
        onrowediting="dgvLeaveUpload_RowEditing" 
        onrowupdating="dgvLeaveUpload_RowUpdating" 
        onrowdeleting="dgvLeaveUpload_RowDeleting">
 <Columns>
     <asp:CommandField ShowEditButton="True" ButtonType="Image" 
         CancelImageUrl="~/Images/cancelButton.png" CancelText="" 
         EditImageUrl="~/Images/editButton.png" EditText="" 
         UpdateImageUrl="~/Images/saveButton.png" UpdateText="" >
     <HeaderStyle Wrap="False" />
     <ItemStyle Wrap="False" />
     </asp:CommandField>
     <asp:CommandField ShowDeleteButton="True" ButtonType="Image" 
         DeleteImageUrl="~/Images/deleteButton.png" DeleteText="" />
    <asp:TemplateField HeaderText="Employee ID*">
        <ItemTemplate>
            <asp:Label ID="lblEmployeeId" runat="server" Text='<%# Eval("Employee ID") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtEmployeeID" runat="server" Text='<%# Eval("Employee ID") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Date of Leave*">
        <ItemTemplate>
            <asp:Label ID="lblDOL" runat="server" Text='<%# Eval("Date of Leave") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtDOL" runat="server" Text='<%# Eval("Date of Leave") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Leave Type*">
        <ItemTemplate>
            <asp:Label ID="lblLVType" runat="server" Text='<%# Eval("Leave Type") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtLVType" runat="server" Text='<%# Eval("Leave Type") %>' Width = "60px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField> 
    <asp:TemplateField HeaderText="Start Time*">
        <ItemTemplate>
            <asp:Label ID="lblStartTime" runat="server" Text='<%# Eval("Start Time") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtStartTime" runat="server" Text='<%# Eval("Start Time") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="End Time">
        <ItemTemplate>
            <asp:Label ID="lblEndTime" runat="server" Text='<%# Eval("End Time") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtEndTime" runat="server" Text='<%# Eval("End Time") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Hours">
        <ItemTemplate>
            <asp:Label ID="lblHours" runat="server" Text='<%# Eval("Hours") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtHours" runat="server" Text='<%# Eval("Hours") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>   
    <asp:TemplateField HeaderText="Reason*">
        <ItemTemplate>
            <asp:Label ID="lblReason" runat="server" Text='<%# Eval("Reason") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtReason" runat="server" Text='<%# Eval("Reason") %>' Width = "250px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="250px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Day Unit">
        <ItemTemplate>
            <asp:Label ID="lblDayUnit" runat="server" Text='<%# Eval("Day Unit") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtDayUnit" runat="server" Text='<%# Eval("Day Unit") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler1">
        <ItemTemplate>
            <asp:Label ID="lblFiller1" runat="server" Text='<%# Eval("Filler1") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller1" runat="server" Text='<%# Eval("Filler1") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler2">
        <ItemTemplate>
            <asp:Label ID="lblFiller2" runat="server" Text='<%# Eval("Filler2") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller2" runat="server" Text='<%# Eval("Filler2") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Filler3">
        <ItemTemplate>
            <asp:Label ID="lblFiller3" runat="server" Text='<%# Eval("Filler3") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtFiller3" runat="server" Text='<%# Eval("Filler3") %>' Width = "150px" Height = "10px"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="150px" />
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Remarks">
        <ItemTemplate>
            <asp:Label ID="lblRemarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
        </ItemTemplate>
       <EditItemTemplate>
            <asp:TextBox ID="txtRemarks" runat="server" Text='<%# Eval("Remarks") %>' Width = "250px" Height = "10px" ReadOnly = "true"></asp:TextBox>
        </EditItemTemplate>
        <ItemStyle Width="250px"/>
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
