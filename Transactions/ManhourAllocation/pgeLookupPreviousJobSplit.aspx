<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pgeLookupPreviousJobSplit.aspx.cs" Inherits="pgeLookupJob" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Job Lookup</title>
    <style type="text/css">
        .align-right
        {
            text-align :right;
        }
        input
        {
            text-transform: uppercase;
        }
        textarea
        {
            text-transform: uppercase;
        }
        .bodyContent
        {
            width:700px;
            font-size:10px;
            font-family: arial narrow;
            padding: 0 auto;
        }
        input, td
        {
            height:12px;
            font-size:10px;
        }
        select
        {
	        height:20px;
	        font-size:12px;
        }

        span
        {
	        font-size:12px;
	        height:14px;
        }

    </style>
    <link href="CSS/iFrameCSS.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Javascript/inFrame.js"></script>
    <script type="text/javascript" src="JobSplitLookup.js"></script>
    <script type="text/javascript">
    	function SendValueToParent() {
    	
			var myVal1 = document.getElementById('valueJob').value;
			var myVal2 = document.getElementById('valueName').value;
			var myVal3 = document.getElementById('valueClient').value;
			window.opener.GetValueFromChildJob(myVal1, myVal2, myVal3);
			window.close();
			return false;
		}
		
		var popWin = null;
		
		function popupCost()
		{
		    popWin = window.open("pgeLookupCost.aspx","Sample","scrollbars=no,resizable=no,width=685,height=400");
			return false;
		}
		
		
		function scram()
		{
          if (popWin != null && popWin.open) 
          {
            popWin.close();
          }
        }
        window.onfocus=scram;
		
	</script>
	
</head>
<body style="margin: 10px 0px 0px 8px; padding:0px" onmouseover="Spooler()">    
    <form id="form1" runat="server">
        <table style="width: 788px">
            <tr>
                <td colspan="3" style="width: 803px">
                    <table>
                        <tr>
                            <td style="width: 81px; height: 22px">
                                <asp:Label ID="lblControlNo" runat="server" Text="Control No"></asp:Label>
                            </td>
                            <td style="width: 155px; height: 22px">
                                <asp:TextBox ID="txtControlNo" runat="server" BackColor="WhiteSmoke" ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="width: 803px" valign="top">
                    <script language="javascript" src="Javascript/autoTime.js" type="text/javascript"></script>
                    <table id="tst" border="0" onkeypress="return isNumberJS(event)" onkeyup="formatCompute()"
                        style="width: 99%; text-align: center">
                        <tbody>
                            <tr>
                                <th style="width: 18px">
                                </th>
                                <th style="font-weight: normal; width: 51px; text-align: center">
                                    Start Time
                                </th>
                                <th style="font-weight: normal; width: 51px; text-align: center">
                                    End Time
                                </th>
                                <th style="font-weight: normal; width: 52px; text-align: center">
                                    Work Hours
                                </th>
                                <th style="width: 21px">
                                </th>
                                <th style="font-weight: normal; width: 77px; text-align: center">
                                    CPH Job No.
                                </th>
                                <th style="font-weight: normal; width: 70px; text-align: center">
                                    Client Job No.
                                </th>
                                <th align="center" style="font-weight: normal; width: 246px; text-align: center">
                                    Client Job Name&nbsp;</th>
                                <th align="center" style="font-weight: normal; width: 15px; text-align: center">
                                    Billable</th>
                                <th align="center" style="font-weight: normal; width: 69px; text-align: center">
                                    Work Act. Code</th>
                                <th align="center" style="font-weight: normal; widht: 18px;text-align: center">
                                    Overtime</th>
                            </tr>
                            <tr>
                                <td style="width: 18px; height: 41px" valign="top">
                                    &nbsp;</td>
                                <td style="width: 51px; height: 41px; text-align: center" valign="top">
                                    <asp:TextBox ID="txtJStart" runat="server" AccessKey="2" BackColor="#F3F3F3" EnableViewState="true"
                                        MaxLength="5" Width="43px"></asp:TextBox>
                                </td>
                                <td style="width: 51px; height: 41px; text-align: center" valign="top">
                                    <asp:TextBox ID="txtJEnd" runat="server" AccessKey="3" BackColor="#F3F3F3" EnableViewState="true"
                                        MaxLength="5" Width="42px"></asp:TextBox>
                                </td>
                                <td style="width: 52px; height: 41px; text-align: center" valign="top">
                                    <asp:TextBox ID="txtJHours" CssClass="align-right" runat="server" AccessKey="4" BackColor="WhiteSmoke" EnableViewState="true"
                                        MaxLength="3" Width="40px"></asp:TextBox>
                                </td>
                                <td style="width: 21px; height: 41px; text-align: center" valign="top">
                                    &nbsp;</td>
                                <td style="width: 77px; height: 41px" valign="top">
                                    <asp:TextBox ID="txtJJobCode" runat="server" BackColor="#F3F3F3" EnableViewState="true"
                                        Width="67px"></asp:TextBox>
                                </td>
                                <td align="center" style="width: 70px; height: 41px" valign="top">
                                    <asp:TextBox ID="txtJClientNo" runat="server" BackColor="#F3F3F3" EnableViewState="true"
                                        Width="73px"></asp:TextBox>
                                </td>
                                <td style="width: 246px; height: 41px; text-align: center" valign="top">
                                    <asp:TextBox ID="txtJClientName" runat="server" BackColor="#F3F3F3" EnableViewState="true"
                                        Width="243px"></asp:TextBox>
                                </td>
                                <td style="height: 41px; text-align: center" valign="top">
                                    <asp:CheckBox ID="chkBillable" runat="server" BackColor="WhiteSmoke"/></td>
                                <td style="width: 69px; height: 41px" valign="top">
                                    <asp:TextBox ID="txtSubWork" runat="server" BackColor="WhiteSmoke" Width="88px"></asp:TextBox>
                                </td>
                                <td style="height: 41px; text-align: center" valign="top">
                                    <asp:CheckBox ID="chkOvertime" runat="server" BackColor="WhiteSmoke"/></td>
                                <td style="width: 122px; height: 41px">
                                    <asp:HiddenField ID="hCat" runat="server" />
                                </td>
                                <td style="height: 41px">
                                    <asp:HiddenField ID="hCCT" runat="server" />
                                </td>
                                <td style="height: 41px">
                                    <asp:HiddenField ID="hSub" runat="server" />
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
                                <td style="width: 109px" align="right">
                                    <asp:Label ID="lblTotalHours" runat="server" Text="Total Work Hours"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTotalHours" CssClass="align-right" runat="server" BackColor="#FFFFC0" Width="40px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHoursInfo" runat="server" BackColor="Transparent" BorderStyle="None" Width="602px"></asp:TextBox>
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
        <asp:HiddenField ID="hiddenRoute" runat="server" />
        <asp:HiddenField ID="break_Start" runat="server" />
        <asp:HiddenField ID="break_End" runat="server" />
        <asp:HiddenField ID="hasJob_Split" runat="server" />
        <asp:HiddenField ID="hiddenCNumber" runat="server" />
        <asp:HiddenField ID="hiddenShift" runat="server" />
    </form>
</body>
</html>
