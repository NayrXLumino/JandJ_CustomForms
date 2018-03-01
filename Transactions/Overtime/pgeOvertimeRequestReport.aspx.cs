using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Payroll.DAL;
using System.Collections;

public partial class Transactions_Overtime_pgeOvertimeRequestReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MenuGrant MGBL = new MenuGrant();
        //System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        CommonMethods methods = new CommonMethods();
        OvertimeBL OTBL = new OvertimeBL();
        //FormatToHTMLTable(GetData());
        //FormatTable(GetData());
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFOTREQREP"))
        {
            Response.Redirect("../../index.aspx?pr=ur");
        }
        if (!Page.IsPostBack)
        {
            dtpFrom.Date = DateTime.Now;
            dtpTo.Date = DateTime.Now;
        }
        Session["dateOvertimeBatchReq"] = dtpFrom.Date.ToString("MM/dd/yyyy");
        LoadComplete += new EventHandler(Transactions_Overtime_pgeOvertimeRequestReport_LoadComplete);
    }
    void Transactions_Overtime_pgeOvertimeRequestReport_LoadComplete(object sender, EventArgs e)
    {
        string jsname = "overtimeScripts";
        string jsurl = "_overtime.js";
        Type jsType = this.GetType();
        ClientScriptManager js = Page.ClientScript;
        if (!js.IsClientScriptIncludeRegistered(jsType, jsname))
        {
            js.RegisterClientScriptInclude(jsType, jsname, ResolveClientUrl(jsurl));
        }
        btnEmployee.OnClientClick = string.Format("return lookupROTEmployee()");

        btnCostcenter.OnClientClick = string.Format("return lookupROTCostcenter()");
        btnCostcenterLine.OnClientClick = string.Format("return lookupROTCostcenterLine()");
        btnBatchNo.OnClientClick = string.Format("return lookupROTBatchNo()");
        TextBox txtTemp = (TextBox)dtpFrom.Controls[0];
        txtTemp.Attributes.Add("readOnly", "true");
        System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
        cal = (System.Web.UI.WebControls.Calendar)dtpFrom.Controls[3];
        cal.Attributes.Add("OnClick", "javascript:__doPostBack();");
        
    }
//    private DataTable GetData()
//    {
//        string sql = @" SELECT TOP 100 RTRIM(Eot_EmployeeId) [Employee|ID]
//	                         , Emt_LastName [Employee|Lastname]
//	                         , Emt_FirstName [Employee|Firstname]
//	                         , Convert(varchar(10), Eot_OvertimeDate, 101) [OT Date]
//	                         , LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime, 2) [Start]
//	                         , LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2) [End]
//	                         , '' [Signature]
//	                         , '' [Shuttle Service|  LP  ]
//	                         , '' [Shuttle Service|  ST  ]
//	                         , '' [Shuttle Service|  CL  ]
//	                         , '' [Shuttle Service|  MD  ]
//	                         , Eot_OvertimeHour [Plan OT Hours]
//	                         , '' [Accum # of OT Hours]
//	                         , '' [Actual|OT Hours]
//	                         , '' [Actual|AGM/MG/Dept. MG Approved]
//	                         , '' [Actual|AGM/GM Approved]
//	                         , Eot_Reason [Reason]
//                          FROM T_EmployeeOvertime
//                          LEFT JOIN T_EmployeeMaster
//                            ON Emt_EmployeeID = Eot_EmployeeId
//                         ORDER BY Eot_OvertimeDate DESC ";
//        DataSet ds = new DataSet();
//        using (DALHelper dal = new DALHelper())
//        {
//            try
//            {
//                dal.OpenDB();
//                ds = dal.ExecuteDataSet(sql, CommandType.Text);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//            finally
//            {
//                dal.CloseDB();
//            }
//        }

//        return ds.Tables[0];
//    }

    private void FormatTable(DataTable dtDetail)
    {
        int rowMax = 0;
        int splitTemp = 0;
        int colSpan = 1;
        bool addColumn = true;


        if (dtDetail != null && dtDetail.Rows.Count > 0)
        {
            //find max rows
            foreach (DataColumn item in dtDetail.Columns)
            {
                splitTemp = item.ColumnName.Split(new char[] { '|' }, StringSplitOptions.None).Length;
                rowMax = (splitTemp > rowMax ? splitTemp : rowMax);
            }
            dtDetail.Columns.Remove("ctrl");
            hfCostcenter.Value = dtDetail.Rows[0]["CostCenter"].ToString();
            dtDetail.Columns.Remove("CostCenter");
            //start creating header rows(row by row)System.Web.UI.WebControls.TableRow tr = new System.Web.UI.WebControls.TableRow();
            for (int i = 0; i < rowMax; i++)
            {
                System.Web.UI.WebControls.TableRow tr = new System.Web.UI.WebControls.TableRow();
                for (int iCol = 0; iCol < dtDetail.Columns.Count; iCol++)
                {
                    addColumn = true;
                    DataColumn itemCol = dtDetail.Columns[iCol];

                    System.Web.UI.WebControls.TableCell td = new System.Web.UI.WebControls.TableCell();

                    #region SET ROW SPAN
                    splitTemp = itemCol.ColumnName.Split(new char[] { '|' }, StringSplitOptions.None).Length;
                    if ((i + 1) == splitTemp)
                    {
                        td.RowSpan = rowMax - i;
                    }
                    else
                    {
                        td.RowSpan = 1;
                    }
                    #endregion

                    #region SET COL SPAN
                    colSpan = 1;
                    try
                    {
                        if (iCol > 0
                         && dtDetail.Columns[iCol - 1].ColumnName.Split(new char[] { '|' }, StringSplitOptions.None)[i] == itemCol.ColumnName.Split(new char[] { '|' }, StringSplitOptions.None)[i])
                        {
                            addColumn = false;
                        }
                        else
                        {
                            //compute colspan
                            for (int x = iCol; x < dtDetail.Columns.Count; x++)
                            {
                                try
                                {
                                    if (x + 1 < dtDetail.Columns.Count
                                     && dtDetail.Columns[x].ColumnName.Split(new char[] { '|' }, StringSplitOptions.None)[i] == dtDetail.Columns[x + 1].ColumnName.Split(new char[] { '|' }, StringSplitOptions.None)[i])
                                    {
                                        colSpan++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    { 
                        //no implementation
                    }

                    td.ColumnSpan = colSpan;
                    //td.HorizontalAlign = HorizontalAlign.Left; 
                    #endregion

                    if ((i + 1) <= splitTemp && addColumn)
                    {
                        td.Text = itemCol.ColumnName.Split(new char[] { '|' }, StringSplitOptions.None)[i].Replace(" ", "&nbsp;");
                        td.BorderStyle = BorderStyle.Solid;
                        td.BorderWidth = Unit.Pixel(1);
                        td.HorizontalAlign = HorizontalAlign.Center;
                        td.Font.Bold = true;
                        td.BackColor = System.Drawing.Color.LightGray;
                        td.Attributes.Add("Width", "auto");
                        tr.Cells.Add(td);
                    }
                }
                
                tblResult.Rows.Add(tr);
            }


            foreach (DataRow iRow in dtDetail.Rows)
            {
                System.Web.UI.WebControls.TableRow tr = new System.Web.UI.WebControls.TableRow();
                foreach (object item in iRow.ItemArray)
                {
                    System.Web.UI.WebControls.TableCell td = new System.Web.UI.WebControls.TableCell();
                    td.Text = item.ToString();

                    td.BorderStyle = BorderStyle.Solid;
                    td.BorderWidth = Unit.Pixel(1);
                    td.HorizontalAlign = HorizontalAlign.Left;
                    td.Attributes.Add("Width", "auto");
                    td.Attributes.Add("Cell-padding", "2");
                    tr.Cells.Add(td);
                }

                tblResult.Rows.Add(tr);
            }

        }

    }
    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCellsV2(e.Row, new ArrayList());
        
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        btnGenerate_Click(null, null);
    }
    private void createRow(GridViewRow gvr, ArrayList arr)
    {
        if (arr == null)
            arr = new ArrayList();
        if (gvr.RowType == DataControlRowType.DataRow)
        {
            TableRow tr = gvr;
            for (int i = 0; i < tr.Cells.Count; i++)
            {
                tr.Cells[i].Wrap = false;
                if (!tr.Cells[i].Text.Equals("&nbsp;"))
                    tr.Cells[i].Text = HttpUtility.HtmlDecode(tr.Cells[i].Text);
                tr.Cells[i].Attributes.Add("Width", "auto");
                try
                {
                    if (arr != null && arr.Contains(i))
                        tr.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
                catch (Exception)
                { }
                finally
                { }

            }
        }
    }
    private string SQLBuilder()
    {
        //apsungahid added 20141201
        bool hasCCLine = MethodsLibrary.Methods.GetProcessControlFlag("GENERAL", "CTRLINEREF");

        string sql = string.Empty;
        string filter = string.Empty;

        if (cbxReportOptions.SelectedItem.Value == "OTR")
        {
            sql = @"   SELECT DISTINCT Eot_ControlNo [ctrl]
							, RTRIM(Eot_EmployeeId) [Employee|ID]
	                        , Emt_LastName [Employee|Lastname]
	                        , Emt_FirstName [Employee|Firstname]
	                        --, Convert(varchar(10), Eot_OvertimeDate, 101) [ OT Date ]
	                        , LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime, 2) [Time| Start ]
	                        , LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2) [Time| End ]
	                        , '' [Signature]
	                        , IIF(Eot_Filler1 = 'LP', 1, 0) [Shuttle Service|  LP  ]
	                        , IIF(Eot_Filler1 = 'ST', 1, 0) [Shuttle Service|  ST  ]
	                        , IIF(Eot_Filler1 = 'CL', 1, 0) [Shuttle Service|  CL  ]
	                        , Eot_OvertimeHour [Plan<br>OT Hours]
	                        , '' [Accum # of<br>OT Hours]
	                        , '' [Actual|<br>OT Hours]
	                        , '' [Actual|AGM/MG/Dept. MG<br>Approved]
	                        , '' [Actual|AGM/GM<br>Approved]
	                        , Eot_Reason [                Reason                ]
                            , dbo.getCostcenterFullNameV2(Eot_Costcenter)[CostCenter]
                         FROM T_EmployeeOvertime
                         LEFT JOIN T_EmployeeMaster
                           ON Emt_EmployeeID = Eot_EmployeeId
                         LEFT JOIN (SELECT DISTINCT Clm_CostCenterCode
									  FROM E_CostCenterLineMaster 
									 WHERE Clm_Status = 'A' ) AS HASLINE
						   ON Clm_CostCenterCode = Eot_Costcenter
                        WHERE 1 = 1 AND Eot_Status in ('1','3','5','7','9')
                          {0}
                    
                        Select Ccd_CompanyName From t_companymaster
                        Where Ccd_status='A'
                       -- ORDER BY Convert(varchar(10), Eot_OvertimeDate, 101) DESC";


            if (!txtEmployee.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Emt_EmployeeId {0}
                                           OR Emt_Lastname {0}
                                           OR Emt_Firstname {0}
                                           OR Emt_Nickname {0})", sqlINFormat(txtEmployee.Text));
            }

            if (!string.IsNullOrEmpty(txtCostcenter.Text))
            {
                filter += string.Format(@" AND  ( Eot_Costcenter {0}
                                            OR dbo.getCostCenterFullNameV2(Eot_Costcenter) {0}) ", sqlINFormat(txtCostcenter.Text)
                                                                                                , Session["userLogged"].ToString());
            }

            if (!txtCostcenterLine.Text.Trim().Equals(string.Empty))
            {
                filter += string.Format(@" AND  ( Eot_CostcenterLine {0} ) ", sqlINFormat(txtCostcenterLine.Text));
            }

            if (tbrEmployee.Visible)
            {
                if (!CommonMethods.isAllCostcenterAccess(Session["userLogged"].ToString(), "OVERTIME"))
                {
                    filter += string.Format(@" AND  (  ( Eot_Costcenter IN ( SELECT Uca_CostCenterCode
                                                                                                FROM T_UserCostCenterAccess
                                                                                            WHERE Uca_UserCode = '{0}'
                                                                                                AND Uca_SytemId = 'OVERTIME')
                                                                        OR Eot_EmployeeId = '{0}'))", Session["userLogged"].ToString());


                }

                if (hasCCLine)//flag costcenter line to retain old code
                {
                    filter += string.Format(@" AND ( ISNULL(Eot_CostcenterLine, '') IN (IIF(Clm_CostCenterCode IS NULL, ISNULL(Eot_CostcenterLine, ''), (SELECT Ucl_LineCode 
										                                                                                    FROM E_UserCostcenterLineAccess 
																														   WHERE (Ucl_CostCenterCode = Eot_CostCenter OR Ucl_CostCenterCode = 'ALL')
																														     AND Ucl_Status = 'A'
																														     AND Ucl_SystemID = 'OVERTIME'
																															 AND Ucl_LineCode = ISNULL(Eot_CostcenterLine, '')
																														     AND Ucl_UserCode = '{0}' )) )
										  OR 'ALL' IN (IIF(Clm_CostCenterCode IS NULL, 'ALL', (SELECT Ucl_LineCode 
										                                                         FROM E_UserCostcenterLineAccess 
																								WHERE Ucl_CostCenterCode = Eot_CostCenter
																								  AND Ucl_Status = 'A'
																								  AND Ucl_SystemID = 'OVERTIME'
																								  AND Ucl_LineCode = 'ALL'
																								  AND Ucl_UserCode = '{0}' )) ) 
                                          OR 'ALL' IN ( SELECT Ucl_LineCode 
										                  FROM E_UserCostcenterLineAccess 
													     WHERE Ucl_CostCenterCode = 'ALL'
														   AND Ucl_Status = 'A'
														   AND Ucl_SystemID = 'OVERTIME'
														   AND Ucl_LineCode = 'ALL'
														   AND Ucl_UserCode = '{0}')
                                          OR Eot_EmployeeID = '{0}') ", Session["userLogged"].ToString());
                }
            }

            if (!string.IsNullOrEmpty(txtBatchNo.Text))
            {
                filter += string.Format(@" AND Eot_BatchNo {0} ", sqlINFormat(txtBatchNo.Text));
            }

            if (dtpFrom.Date != null)
            {
                filter += string.Format(@" AND Eot_OvertimeDate = '{0}'", dtpFrom.Date.ToString("MM/dd/yyyy"));
            }
            if (!txtSearch.Text.Trim().Equals(string.Empty))
            {
                string searchFilter = @"AND (    
                                            ( Eot_EmployeeId LIKE '{0}%' )
                                          OR ( Emt_Lastname LIKE '{0}%' )
                                          OR ( Emt_Firstname LIKE '{0}%' )
                                           OR ( CONVERT(varchar(10),Eot_OvertimeDate,101) LIKE '%{0}%' )
                                          OR ( LEFT(Eot_StartTime,2) + ':' + RIGHT(Eot_StartTime,2) LIKE '{0}%' )
                                          OR ( LEFT(Eot_EndTime,2) + ':' + RIGHT(Eot_EndTime,2) LIKE '{0}%' )
                                          OR ( Eot_OvertimeHour LIKE '{0}%' )
                                          OR ( Eot_Reason LIKE '{0}%' )
                                          OR ( Eot_BatchNo LIKE '{0}%' )
                                    )";

                string holder = string.Empty;
                string searchKey = txtSearch.Text.Replace("'", "");
                searchKey += "&";
                for (int count = searchKey.IndexOf("&"); count > 0; count = searchKey.IndexOf("&"))
                {
                    holder = searchKey.Substring(0, searchKey.IndexOf("&")).Trim();
                    searchKey = searchKey.Substring(searchKey.IndexOf("&") + 1);

                    filter += string.Format(searchFilter, holder);
                }

            }
        }

        return string.Format(sql, filter);
    }

    private string sqlINFormat(string text)
    {
        string[] temp = text.Split(',');
        string value = "IN ( ";
        for (int i = 0; i < temp.Length; i++)
        {
            value += "'" + temp[i].Trim() + "'";
            if (i != temp.Length - 1)
                value += ",";
        }
        value += ")";
        return value;
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        if (dtpFrom != null && dtpFrom.Date.ToString().Trim() != "")
        {
            if ((txtCostcenter.Text.ToString() != "" || txtEmployee.Text.ToString() != ""))
            {
                DataSet ds = new DataSet();
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        ds = dal.ExecuteDataSet(SQLBuilder(), CommandType.Text);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

                tblResult.Rows.Clear();
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                        hfCompanyName.Value = ds.Tables[1].Rows[0][0].ToString();
                    FormatTable(ds.Tables[0]);
                }
            }
            else
            { MessageBox.Show("Either Employee ID or Cost Center should be filled"); }
        }
        else
        {
            MessageBox.Show("Date should be filled");
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //ClientScript.RegisterStartupScript(this.GetType(), "PrintOperation", "PrintPanel()", true);
    }
}