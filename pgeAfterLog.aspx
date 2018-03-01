<%@ Page Language="C#" MasterPageFile="~/pgeMaster.master" AutoEventWireup="true" CodeFile="pgeAfterLog.aspx.cs" Inherits="pgeAfterLog" Title="Workflow" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
    <table style="width:851px">
        <tr>
            <td colspan="2">
            
                <asp:Panel ID="pnlChecklist" runat="server" GroupingText="Checklist" 
                    Width="881px">
                    <asp:Table ID="tblChecklist" runat="server" Width="864px" Font-Size="14px" 
                        CellPadding="2" CellSpacing="0">
                        <asp:TableHeaderRow HorizontalAlign="Center" Font-Bold="true">
                            <asp:TableCell Width="20%" CssClass="borderBR">
                                <div>
                                    <asp:Button ID="btnRefresh" runat="server" Text="REFRESH LIST" Font-Names="Calibri" Width="108px" OnClick="btnRefresh_Click" />
                                </div>
                            </asp:TableCell>
                            <asp:TableCell Width="20%" CssClass="borderLBR">
                                <div>
                                    <asp:Label ID="lblWaitList" runat="server" Text="Wait List"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell Width="20%" CssClass="borderLBR">
                                <div>
                                    <asp:Label ID="lblChecking" runat="server" Text="For Checking and Approval"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell Width="20%" CssClass="borderLB">
                                <div>
                                    <asp:Label ID="lblNextLevel" runat="server" Text="For Next Level Checking and Approval"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableRow ID="OT">
                            <asp:TableCell CssClass="borderTBR">
                                <asp:Label ID="lblOTHeader" runat="server" Text="OVERTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupWaitList('OT')" class="selectHand">
                                    <asp:Label ID="lblWOT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupCheckList('OT')" class="selectHand">
                                    <asp:Label ID="lblCOT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLB">
                                <div onclick="lookupNextLevel('OT')" class="selectHand">
                                    <asp:Label ID="lblNOT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="LV">
                            <asp:TableCell CssClass="borderTBR">
                                <asp:Label ID="lblLeaveHeader" runat="server" Text="LEAVE"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupWaitList('LV')" class="selectHand">
                                    <asp:Label ID="lblWLV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupCheckList('LV')" class="selectHand">
                                    <asp:Label ID="lblCLV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLB">
                                <div onclick="lookupNextLevel('LV')" class="selectHand">
                                    <asp:Label ID="lblNLV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TR">
                            <asp:TableCell CssClass="borderTBR">
                                <asp:Label ID="lblTimeModHeader" runat="server" Text="TIME MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupWaitList('TR')" class="selectHand">
                                    <asp:Label ID="lblWTR" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupCheckList('TR')" class="selectHand">
                                    <asp:Label ID="lblCTR" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLB">
                                <div onclick="lookupNextLevel('TR')" class="selectHand">
                                    <asp:Label ID="lblNTR" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="FT">
                            <asp:TableCell CssClass="borderTBR">
                                <asp:Label ID="lblFlexHeader" runat="server" Text="FLEXTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupWaitList('FT')" class="selectHand">
                                    <asp:Label ID="lblWFT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupCheckList('FT')" class="selectHand">
                                    <asp:Label ID="lblCFT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLB">
                                <div onclick="lookupNextLevel('FT')" class="selectHand">
                                    <asp:Label ID="lblNFT" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="JS">
                            <asp:TableCell CssClass="borderTBR">
                                <asp:Label ID="lblJobSplit" runat="server" Text="MANHOUR MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupWaitList('JS')" class="selectHand">
                                    <asp:Label ID="lblWJS" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLBR">
                                <div onclick="lookupCheckList('JS')" class="selectHand">
                                    <asp:Label ID="lblCJS" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLB">
                                <div onclick="lookupNextLevel('JS')" class="selectHand">
                                    <asp:Label ID="lblNJS" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="MV">
                            <asp:TableCell CssClass="borderTR">
                                <asp:Label ID="lblMovementHeader" runat="server" Text="WORK INFORMATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupWaitList('MV')" class="selectHand">
                                    <asp:Label ID="lblWMV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupCheckList('MV')" class="selectHand">
                                    <asp:Label ID="lblCMV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTL">
                                <div onclick="lookupNextLevel('MV')" class="selectHand">
                                    <asp:Label ID="lblNMV" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="BF">
                            <asp:TableCell CssClass="borderTR">
                                <asp:Label ID="Label2" runat="server" Text="BENEFICIARY"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupWaitList('BF')" class="selectHand">
                                    <asp:Label ID="lblWBF" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupCheckList('BF')" class="selectHand">
                                    <asp:Label ID="lblCBF" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTL">
                                <div onclick="lookupNextLevel('BF')" class="selectHand">
                                    <asp:Label ID="lblNBF" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TX">
                            <asp:TableCell CssClass="borderTR">
                                <asp:Label ID="Label1" runat="server" Text="TAX / CIVIL"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupWaitList('TX')" class="selectHand">
                                    <asp:Label ID="lblWTX" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupCheckList('TX')" class="selectHand">
                                    <asp:Label ID="lblCTX" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTL">
                                <div onclick="lookupNextLevel('TX')" class="selectHand">
                                    <asp:Label ID="lblNTX" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="AD">
                            <asp:TableCell CssClass="borderTR">
                                <asp:Label ID="lblAddress" runat="server" Text="ADDRESS"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupWaitList('AD')" class="selectHand">
                                    <asp:Label ID="lblWAD" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupCheckList('AD')" class="selectHand">
                                    <asp:Label ID="lblCAD" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTL">
                                <div onclick="lookupNextLevel('AD')" class="selectHand">
                                    <asp:Label ID="lblNAD" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="SW">
                            <asp:TableCell CssClass="borderTR">
                                <asp:Label ID="lblStraightWork" runat="server" Text="STRAIGHTWORK"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupWaitList('SW')" class="selectHand">
                                    <asp:Label ID="lblWSW" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTLR">
                                <div onclick="lookupCheckList('SW')" class="selectHand">
                                    <asp:Label ID="lblCSW" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderTL">
                                <div onclick="lookupNextLevel('SW')" class="selectHand">
                                    <asp:Label ID="lblNSW" runat="server" Text="0" ToolTip="Click to open list" Width="70px"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr style="width: 880px"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" style="font-weight:bold; color:#4A3DFF;">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width:40%">
                            <hr />
                        </td>
                        <td style="width:20%">
                            M&nbsp;Y&nbsp;&nbsp;&nbsp;&nbsp;T&nbsp;R&nbsp;A&nbsp;N&nbsp;S&nbsp;A&nbsp;C&nbsp;T&nbsp;I&nbsp;O&nbsp;N&nbsp;S
                        </td>
                        <td style="width:40%">
                            <hr style="width: 350px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlNewPending" runat="server" 
                    GroupingText="<span class='myTransactionLegend'><b>NEW AND PENDING TRANSACTIONS</b></span>" 
                    ForeColor="#4A3DFF" Width="423px">
                    <asp:Table ID="tblNewPending" runat="server" Width="100%" CellSpacing="0">
                        <asp:TableRow ID="NPOT">
                            <asp:TableCell Width="200px" CssClass="borderB">
                                <asp:Label ID="lblNPOT" runat="server" Text="OVERTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('OT')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPOT" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPLV">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPLV" runat="server" Text="LEAVE"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('LV')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPLV" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPTR">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPTR" runat="server" Text="TIME MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('TR')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPTR" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPFT">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPFT" runat="server" Text="FLEXTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('FT')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPFT" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPJS">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPJS" runat="server" Text="MANHOUR MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('JS')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPJS" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPMV">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPMV" runat="server" Text="WORK INFORMATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('MV')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPMV" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPBF">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="Label6" runat="server" Text="BENEFICIARY"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('BF')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPBF" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPTX">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPTX" runat="server" Text="TAX/CIVIL"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('TX')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPTX" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPAD">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPAD" runat="server" Text="ADDRESS"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('AD')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPAD" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="NPSW">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblNPSW" runat="server" Text="STRAIGHT WORK"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupNewPending('SW')" class="transactionSelectHand">
                                    <asp:Label ID="btnNPSW" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </td>
            <td>
                <asp:Panel ID="pnlApprovedDisapproved" runat="server" 
                    GroupingText="<span class='myTransactionLegend'><b>APPROVED AND DISAPPROVED TRANSACTIONS</b></span>" 
                    ForeColor="#4A3DFF" Width="450px">
                    <asp:Table ID="tblApprovedDisapproved" runat="server" Width="437px" 
                        CellSpacing="0">
                        <asp:TableRow ID="ADOT">
                            <asp:TableCell Width="200px" CssClass="borderB">
                                <asp:Label ID="lblADOT" runat="server" Text="OVERTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('OT')" class="transactionSelectHand">
                                    <asp:Label ID="btnADOT" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADLV">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADLV" runat="server" Text="LEAVE"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('LV')" class="transactionSelectHand">
                                    <asp:Label ID="btnADLV" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADTR">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADTR" runat="server" Text="TIME MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('TR')" class="transactionSelectHand">
                                    <asp:Label ID="btnADTR" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADFT">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADFT" runat="server" Text="FLEXTIME"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('FT')" class="transactionSelectHand">
                                    <asp:Label ID="btnADFT" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADJS">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADJS" runat="server" Text="MANHOUR MODIFICATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('JS')" class="transactionSelectHand">
                                    <asp:Label ID="btnADJS" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADMV">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADMV" runat="server" Text="WORK INFORMATION"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('MV')" class="transactionSelectHand">
                                    <asp:Label ID="btnADMV" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADBF">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADBF" runat="server" Text="BENEFICIARY"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('BF')" class="transactionSelectHand">
                                    <asp:Label ID="btnADBF" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADTX">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADTX" runat="server" Text="TAX/CIVIL"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('TX')" class="transactionSelectHand">
                                    <asp:Label ID="btnADTX" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADAD">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADAD" runat="server" Text="ADDRESS"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('AD')" class="transactionSelectHand">
                                    <asp:Label ID="btnADAD" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="ADSW">
                            <asp:TableCell CssClass="borderB">
                                <asp:Label ID="lblADSW" runat="server" Text="STRAIGHT WORK"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass="borderB">
                                <div onclick="lookupApprovedDisapproved('SW')" class="transactionSelectHand">
                                    <asp:Label ID="btnADSW" runat="server" Text="0"></asp:Label>
                                </div>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</div>
</asp:Content>

