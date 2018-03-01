<%@ Page Language="C#" AutoEventWireup="true" CodeFile="hlpDefault.aspx.cs" Inherits="Help_Default" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">    
    <link href="../CSS/HelpMaster.css" rel="stylesheet" type="text/css" />
    <title></title>
    
</head>
<body>
<form id="form1" runat="server">
<table cellpadding="0" cellspacing="0" align="center" style="width:900px;">
    <tr>
        <td colspan="2">
            <div id="Header"></div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div id="Navigate1" style="width:900px; height:20px; background-color:White; background-color:#9BC099; border-bottom:1px solid #000000;">
                <table align="left" cellspacing="0" cellpadding="0">
                    <tr>
                        <td align="right" style="width:900px;">
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
    <tr>
        <td align="left" valign="top">
            <div id="Navigation">
                <asp:TreeView ID="TreeView1" runat="server" ImageSet="Faq" Target="Content">
                                <Nodes>
                                    <asp:TreeNode Text="Login" Value="Login" 
                                        NavigateUrl="~/Help/Login/hlpLogin.aspx" Checked="True">
                                    </asp:TreeNode>
                                    <asp:TreeNode Text="Home" Value="Home" 
                                        NavigateUrl="~/Help/Home/hlpHome.aspx" Checked="True">
                                    </asp:TreeNode>
                                    <asp:TreeNode Text="Transaction" Value="Transaction" Checked="True" 
                                        SelectAction="Expand">
                                        <asp:TreeNode Text="Overtime" Value="Overtime" Expanded="True" Checked="True" 
                                            SelectAction="Expand">
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Overtime/hlpOvertimeEntry.aspx" 
                                                Text="Overtime Entry" Value="OvertimeEntry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Overtime/hlpBatchOvertimeEntry.aspx" 
                                                Text="Batch Overtime Entry" Value="Batch Overtime Entry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Overtime/hlpSpecialovertimeEntry.aspx" 
                                                Text="Special Overtime Entry" Value="New Node">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Overtime/hlpOvertimeReport.aspx" 
                                                Text="Overtime Report" Value="Overtime Report">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Leave" Value="New Node" Expanded="True" Checked="True" 
                                            SelectAction="Expand">
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Leave/hlpLeaveEntry.aspx" 
                                                Text="Leave Entry" Value="Leave Entry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Leave/hlpLeaveNoticeEntry.aspx" 
                                                Text="Leave Notice Entry" Value="Leave Notice Entry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Leave/hlpSpecialLeaveEntry.aspx" 
                                                Text="Special Leave Entry" Value="Scpecial Leave Entry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Leave/hlpLeaveCancellation.aspx" 
                                                Text="Leave Cancellation" Value="Leave Cancellation">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Leave/hlpLeaveReport.aspx" 
                                                Text="Leave Report" Value="Leave Report">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Time Record" Value="Time Record" Expanded="True" 
                                            Checked="True" SelectAction="Expand">
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/TimeRecord/hlpTimeModEntry.aspx" 
                                                Text="Time Modification Entry" Value="Tim Modification Entry">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/TimeRecord/hlpTimeModReport.aspx" 
                                                Text="Time Modification Report" Value="Time Modification Report">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/TimeRecord/hlpTimeRecordEntryReport.aspx" 
                                                Text="Time Record Entry Report" Value="Time Record Entry Report">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Work Information" Value="Work Information" Expanded="True" 
                                            Checked="True" SelectAction="Expand">
                                            <asp:TreeNode Text="Group Update" Value="Group Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpGroupUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Batch Group Update" Value="Batch Group Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpBatchGoupUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Shift Update" Value="Shift Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpShiftUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Batch Shift Update" Value="Batch Shift Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpBatchShiftUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Costcenter Update" Value="Costcenter Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpCostCenterUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Batch Costcenter Update" Value="Batch Costcenter Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpBatchCostcenterUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Restday Update" Value="Restday Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpRestdayUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Batch Restday Update" Value="Batch Restday Update" 
                                                NavigateUrl="~/Help/Transactions/WorkInformation/hlpBatchRestdayUpdate.aspx">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Personnel" Value="Personnel" Expanded="True" Checked="True" 
                                            SelectAction="Expand">
                                            <asp:TreeNode Text="Tax Code/Civil Status Update" 
                                                Value="Tax Code/Civil Status Update" 
                                                NavigateUrl="~/Help/Transactions/Personnel/hlpTaxCodeCivilStatusUpdate.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Tax Code/Civil Status Report" 
                                                Value="Tax Code/Civil Status Report" 
                                                NavigateUrl="~/Help/Transactions/Personnel/hlpTaxCodeCivilStatusReport.aspx">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Personnel/hlpPresentAddress.aspx" 
                                                Text="Present Address" Value="Present Address">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Personnel/hlpPermanentAddress.aspx" 
                                                Text="Permanent Address" Value="Permanent Address">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Personnel/hlpEmergencyContact.aspx" 
                                                Text="Emergency Contact" Value="Emergency Contact">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Transactions/Personnel/hlpAddressContactReport.aspx" 
                                                Text="Address/Contact Report" Value="Address/Contact Report">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                    </asp:TreeNode>
                                    <asp:TreeNode Text="Maintenance" Value="Maintenance" Checked="True" 
                                        SelectAction="Expand">
                                        <asp:TreeNode Text="Approval Route" Value="ApprovalRoute" Expanded="True" 
                                            Checked="True" SelectAction="Expand">
                                            <asp:TreeNode NavigateUrl="~/Help/Maintenance/ApprovalRoute/hlpApprovalRouteMaster.aspx" 
                                                Text="Approval Route Master" Value="ApprovalRouteMaster"></asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Maintenance/ApprovalRoute/hlpEmployeeApprovalRoute.aspx" 
                                                Text="Employee Approval Route" Value="EmployeeApprovalRoute">
                                            </asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Maintenance/ApprovalRoute/hlpApprovalRouteReport.aspx" 
                                                Text="Approval Route Report" Value="Approval Route Report"></asp:TreeNode>
                                            <asp:TreeNode NavigateUrl="~/Help/Maintenance/ApprovalRoute/hlpEmployeeApprovalRouteReport.aspx" 
                                                Text="Employee Approval Route Report" Value="Employee Approval Route Report">
                                            </asp:TreeNode>
                                        </asp:TreeNode>
                                    </asp:TreeNode>
                                    <asp:TreeNode Text="Tools" Value="Tools" Checked="True" Expanded="True" 
                                        SelectAction="Expand">
                                        <asp:TreeNode NavigateUrl="~/Help/Tools/Password/hlpChangePassword.aspx" 
                                            Text="Change Password" Value="Change Password" ></asp:TreeNode>
                                    </asp:TreeNode>
                                </Nodes>
                                <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="DarkBlue" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                <ParentNodeStyle Font-Bold="False" />
                                <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" VerticalPadding="0px" />
                </asp:TreeView>
            </div>
        </td>
        <td valign="top">
            <%--src="<%= Session["currentPage"] %>"--%>
            <iframe title="ContentFrame" 
                    name="Content" 
                    align="top" 
                    height="550px"
                    width="650px" 
                    id="Content" 
                    frameborder="0" 
                    src="Login/hlpLogin.aspx">
            </iframe>
        </td>
    </tr>
</table>
</form>
</body>
</html>