using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
public partial class Tools_Announcement_pgeAnnouncementMaster : System.Web.UI.Page
{
    CommonMethods methods = new CommonMethods();
    MenuGrant MGBL = new MenuGrant();
    GeneralBL GNBL = new GeneralBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CommonMethods.isAlive())
        {
            Response.Redirect("../../index.aspx?pr=dc");
        }
        else
        {
            if (!Page.IsPostBack)
            {
                if (!MGBL.getAccessRights(Session["userLogged"].ToString(), "WFANNOUNCE"))
                {
                    Response.Redirect("../../index.aspx?pr=ur");
                }
                else
                {
                    initializeControls();
                    txtSearch_TextChanged(null, null);
                    Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                    Page.PreRender += new EventHandler(Page_PreRender);
                }
            }
            LoadComplete += new EventHandler(Tools_Announcement_pgeAnnouncementMaster_LoadComplete);
        }
    }

    void Tools_Announcement_pgeAnnouncementMaster_LoadComplete(object sender, EventArgs e)
    {
        txtControlNo.Attributes.Add("readOnly", "true");
        txtAnnounceTime.Attributes.Add("OnKeyPress", "javascript:return timeEntry(event);");
    }

    void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["update"] = Session["update"];
    }

    protected void NextPrevButton(object sender, EventArgs e)
    {
        if (((Button)sender).ID == "btnPrev")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) - 1);
        else if (((Button)sender).ID == "btnNext")
            hfPageIndex.Value = Convert.ToString(Convert.ToInt32(hfPageIndex.Value) + 1);
        //bindGrid();
        UpdatePagerLocation();
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        hfPageIndex.Value = "0";
        hfRowCount.Value = "0";
        bindGrid();
        UpdatePagerLocation();
    }

    protected void dgvResult_RowCreated(object sender, GridViewRowEventArgs e)
    {
        CommonLookUp.SetGridViewCells(e.Row, new ArrayList(), 0);
    }

    protected void Lookup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.textDecoration='underline';this.style.cursor='hand';this.style.color='#0000FF'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#000000'";
            e.Row.Attributes.Add("onclick", "javascript:return AssignValueToControlNo('" + e.Row.RowIndex + "')");
        }
    }

    protected void btnX_Click(object sender, EventArgs e)
    {
        string err = string.Empty;
        if (Session["update"].ToString() == ViewState["update"].ToString())
        {
            switch (btnX.Text.ToUpper())
            {
                case "NEW":
                    mtvTab.ActiveViewIndex = 1;
                    btnX.Text = "CREATE";
                    btnY.Text = "CANCEL";
                    txtControlNo.Text = "AUTO-GENERATED";
                    dtpAnnonceDate.Date = DateTime.Now;
                    txtAnnounceTime.Text = string.Empty;
                    txtAnnouncedBy.Text = string.Empty;
                    txtSubject.Text = string.Empty;
                    txtInformation.Text = string.Empty;
                    ddlPriority.SelectedIndex = 0;
                    ddlStatus2.SelectedIndex = 0;
                    for (int i = 0; i < cbxProfiles.Items.Count; i++)
                    {
                        if (!cbxProfiles.Items[i].Value.Equals("ALL"))
                        {
                            cbxProfiles.Items[i].Selected = false;
                        }
                        else
                        {

                            cbxProfiles.Items[i].Selected = true;
                        }
                    }
                    break;
                case "CREATE":
                    err = checkEntry();
                    if (err.Equals(string.Empty))
                    {
                        GNBL.CreateAnnouncement(PopulateDr());
                        MessageBox.Show("Successfully created new announcement.");
                        txtControlNo.Text = string.Empty;
                        btnX.Text = "NEW";
                        btnY.Text = "EDIT";
                        mtvTab.ActiveViewIndex = 0;
                        txtSearch_TextChanged(null, null);
                    }
                    else
                    {
                        MessageBox.Show(err);
                    }
                    break;
                case "SAVE":
                    err = checkEntry();
                    if (err.Equals(string.Empty))
                    {
                        GNBL.UpdateAnnouncement(PopulateDr());
                        MessageBox.Show("Successfully updated announcement.");
                        btnX.Text = "NEW";
                        btnY.Text = "EDIT";
                        mtvTab.ActiveViewIndex = 0;
                        txtControlNo.Text = string.Empty;
                        dtpAnnonceDate.Date = DateTime.Now;
                        txtAnnounceTime.Text = string.Empty;
                        txtAnnouncedBy.Text = string.Empty;
                        txtSubject.Text = string.Empty;
                        txtInformation.Text = string.Empty;
                        ddlPriority.SelectedIndex = 0;
                        ddlStatus2.SelectedIndex = 0;
                        txtSearch_TextChanged(null, null);
                    }
                    else
                    {
                        MessageBox.Show(err);
                    }
                    break;
                default:
                    break;
            }
            Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
        }
        else
        {
            MessageBox.Show("Page refresh is not allowed.");
        }
    }

    protected void btnY_Click(object sender, EventArgs e)
    {
        switch (btnY.Text.ToUpper())
        {
            case "EDIT":
                if (!txtControlNo.Text.Equals(string.Empty))
                {
                    mtvTab.ActiveViewIndex = 1;
                    btnX.Text = "SAVE";
                    btnY.Text = "CANCEL";
                    for (int i = 0; i < cbxProfiles.Items.Count; i++)
                    {
                        cbxProfiles.Items[i].Selected = false;
                    }
                    setDetailInformation();
                }
                else
                {
                    MessageBox.Show("Select route from list.");
                }
                break;
            case "CANCEL":
                mtvTab.ActiveViewIndex = 0;
                txtControlNo.Text = string.Empty;
                btnX.Text = "NEW";
                btnY.Text = "EDIT";
                break;
            default:
                break;
        }
    }

    private void initializeControls()
    {
        MenuGrant userGrant = new MenuGrant(Session["userLogged"].ToString(), "GENERAL", "WFANNOUNCE");
        btnX.Enabled = userGrant.canAdd();
        btnY.Enabled = userGrant.canEdit();
        fillProfileChecklist();
    }

    private void UpdatePagerLocation()
    {
        int pageIndex = Convert.ToInt32(hfPageIndex.Value);
        int numRows = Convert.ToInt32(hfNumRows.Value);
        int rowCount = Convert.ToInt32(hfRowCount.Value);
        int currentStartRow = (pageIndex * numRows) + 1;
        int currentEndRow = (pageIndex * numRows) + numRows;
        if (currentEndRow > rowCount)
            currentEndRow = rowCount;
        if (rowCount == 0)
            currentStartRow = 0;
        lblRowNo.Text = currentStartRow.ToString() + " - " + currentEndRow.ToString() + " of " + rowCount.ToString() + " Row(s)";
        btnPrev.Enabled = (pageIndex > 0) ? true : false;
        btnNext.Enabled = (pageIndex + 1) * numRows < rowCount ? true : false;
    }

    private void bindGrid()
    {
        DataSet ds = new DataSet();
        string[] ProfileDBConnections = GNBL.GetProfileConnections();
        using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(SQLBuilder().Replace("@pageIndex", hfPageIndex.Value.ToString()).Replace("@numRow", hfNumRows.Value.ToString()), CommandType.Text);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }

        hfRowCount.Value = "0";
        foreach (DataRow dr in ds.Tables[1].Rows)
            hfRowCount.Value = Convert.ToString(Convert.ToInt32(hfRowCount.Value) + Convert.ToInt32(dr[0]));
        dgvResult.DataSource = ds;
        dgvResult.DataBind();
    }

    private string SQLBuilder()
    {
        StringBuilder sql = new StringBuilder();
        sql.Append(@"declare @startIndex int;
                         SET @startIndex = (@pageIndex * @numRow) + 1;

                       WITH TempTable AS ( SELECT Row_Number()
                                             OVER ( ORDER BY [Control No] DESC) [Row]
                                                , *
                                             FROM ( ");
        sql.Append(getColumns());
        sql.Append(getFilters());
        sql.Append(@"                              ) AS temp)
                                           SELECT [Control No]
                                                , [Announce Date]
                                                , [Announce Time]
                                                , [Announced By]
                                                , [Subject]
                                                , [Information]
                                                , [Priority]
                                                , [Status]
                                             FROM TempTable
                                            WHERE Row BETWEEN @startIndex AND @startIndex + @numRow - 1");
        sql.Append(@" SELECT SUM(cnt)
                        FROM ( SELECT COUNT(Amt_ControlNo) [cnt]");
        sql.Append(getFilters());
        sql.Append(@"        ) as Rows");
        return sql.ToString();
    }

    private string getColumns()
    {
        string columns = string.Empty;
        columns = @"   SELECT Amt_ControlNo [Control No]
                            , Convert(varchar(10), Amt_AnnounceDateTime, 101) [Announce Date]
                            , LEFT(Convert(varchar(20), Amt_AnnounceDateTime,108),5) [Announce Time]
                            , Amt_Announcer [Announced By]
                            , Amt_Subject [Subject]
                            , Amt_Description [Information]
                            , Amt_Priority [Priority]
                            , CASE Amt_Status
                              WHEN 'A' THEN 'ACTIVE'
                              WHEN 'C' THEN 'INACTIVE'
                               END [Status]";
        return columns;
    }

    private string getFilters()
    {
        string filter = string.Empty;
        filter = string.Format(@"   FROM T_AnnouncementMaster
                                   WHERE Amt_Status = '{0}' ", ddlStatus.SelectedValue);

        if (!txtSearch.Text.Trim().Equals(string.Empty))
        {
            string searchFilter = @"AND (    ( Amt_ControlNo LIKE '{0}%' )
                                          OR ( Convert(varchar(10), Amt_AnnounceDateTime, 101) LIKE '%{0}%' )
                                          OR ( LEFT(Convert(varchar(20), Amt_AnnounceDateTime,108),5) LIKE '{0}%' )
                                          OR ( Amt_Announcer LIKE '%{0}%' )
                                          OR ( Amt_Subject LIKE '%{0}%' )
                                          OR ( Amt_Description LIKE '%{0}%' )
                                          OR ( Amt_Priority LIKE '{0}%' )
                                          OR ( CASE Amt_Status
	                                           WHEN 'A' THEN 'ACTIVE'
	                                           WHEN 'C' THEN 'INACTIVE'
	                                            END LIKE '{0}%' )
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
        return filter;
    }

    private void fillProfileChecklist()
    {
        string sqlGetProfiles = @" SELECT Prf_DatabaseNo [Profile ID]
	                                    , Prf_Profile [Profile Name]
                                     FROM T_Profiles ";

        DataSet ds = new DataSet();
        string[] ProfileDBConnections = GNBL.GetProfileConnections();
        using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlGetProfiles, CommandType.Text);
            }
            catch(Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }

        if (!CommonMethods.isEmpty(ds))
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            { 
                cbxProfiles.Items.Add(new ListItem( ds.Tables[0].Rows[i]["Profile Name"].ToString()
                                                  , ds.Tables[0].Rows[i]["Profile ID"].ToString() ));
            }
        }
    }

    private string checkEntry()
    {
        string errMsg = string.Empty;
        if (txtSubject.Text.Equals(string.Empty))
        {
            errMsg += "\nSubject required.";
        }
        if (txtAnnounceTime.Text.Equals(string.Empty))
        {
            errMsg += "\nTime required.";
        }
        if (txtAnnouncedBy.Text.Equals(string.Empty))
        {
            errMsg += "\nAnnounced By required.";
        }
        if (txtInformation.Text.Equals(string.Empty))
        {
            errMsg += "\nInformation required.";
        }
        try
        {
            DateTime checkDate = new DateTime();
            checkDate = Convert.ToDateTime(dtpAnnonceDate.Date.ToString("MM/dd/yyyy") + " " + ((txtAnnounceTime.Text.Length == 4) ? txtAnnounceTime.Text.Insert(2, ":") : txtAnnounceTime.Text));
        }
        catch
        {
            errMsg += "\nInvalid date time format";
        }

        if (btnX.Text.ToUpper().Equals("SAVE") || btnX.Text.ToUpper().Equals("CREATE"))
        {
            int ctrChecked = 0;
            for (int i = 0; i < cbxProfiles.Items.Count; i++)
            {
                if (cbxProfiles.Items[i].Selected)
                {
                    ctrChecked++;
                }
            }

            if (ctrChecked <= 0)
            {
                errMsg += "At least one(1) profile availablity is checked.";
            }
        }

        return errMsg;
    }

    private DataRow PopulateDr()
    {
        DataRow dr = DbRecord.Generate("T_AnnouncementMaster", true);
        dr["Amt_ControlNo"] = txtControlNo.Text;
        dr["Amt_AnnounceDateTime"] = dtpAnnonceDate.Date.ToString("MM/dd/yyyy") + " " + ((txtAnnounceTime.Text.Length == 4) ? txtAnnounceTime.Text.Insert(2, ":") : txtAnnounceTime.Text);
        dr["Amt_Announcer"] = txtAnnouncedBy.Text.ToUpper();
        dr["Amt_Subject"] = txtSubject.Text.ToUpper();
        dr["Amt_Description"] = txtInformation.Text;
        dr["Amt_Priority"] = ddlPriority.SelectedValue.ToUpper();
        dr["Amt_ProfileInclude"] = getProfileInclude();
        dr["Amt_Status"] = ddlStatus2.SelectedValue.ToUpper();
        dr["User_login"] = Session["userLogged"].ToString();
        return dr;
    }

    private void setDetailInformation()
    {
        DataSet ds = new DataSet();
        #region Query
        string sql = @"SELECT Amt_ControlNo [Control No]
                            , Convert(varchar(10), Amt_AnnounceDateTime, 101) [Announce Date]
                            , LEFT(Convert(varchar(20), Amt_AnnounceDateTime,108),5) [Announce Time]
                            , Amt_Announcer [Announced By]
                            , Amt_Subject [Subject]
                            , Amt_Description [Information]
                            , Amt_Priority [Priority]
                            , Amt_ProfileInclude [Include]
                            , CASE Amt_Status
                              WHEN 'A' THEN 'ACTIVE'
                              WHEN 'C' THEN 'INACTIVE'
                               END [Status]
                        FROM T_AnnouncementMaster
                       WHERE Amt_ControlNo = @ControlNo
                        ";
        #endregion
        ParameterInfo[] param = new ParameterInfo[1];
        param[0] = new ParameterInfo("@ControlNo", txtControlNo.Text);
        string[] ProfileDBConnections = GNBL.GetProfileConnections();
        using (DALHelper dal = new DALHelper(ProfileDBConnections[0], ProfileDBConnections[1]))
        {
            try
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, Session["userLogged"].ToString());
            }
            finally
            {
                dal.CloseDB();
            }
        }
        if (!CommonMethods.isEmpty(ds))
        {
            dtpAnnonceDate.Date = Convert.ToDateTime(ds.Tables[0].Rows[0]["Announce Date"].ToString());
            txtAnnounceTime.Text = ds.Tables[0].Rows[0]["Announce Time"].ToString();
            txtAnnouncedBy.Text = ds.Tables[0].Rows[0]["Announced By"].ToString();
            txtSubject.Text = ds.Tables[0].Rows[0]["Subject"].ToString();
            txtInformation.Text = ds.Tables[0].Rows[0]["Information"].ToString();
            for (int i = 0; i < ddlPriority.Items.Count; i++)
            {
                if (ddlPriority.Items[i].Value.ToUpper().Equals(ds.Tables[0].Rows[0]["Priority"].ToString()))
                {
                    ddlPriority.SelectedIndex = i;
                    break;
                }
            }
            if (ds.Tables[0].Rows[0]["Status"].ToString().Equals("ACTIVE"))
            {
                ddlStatus.SelectedIndex = 0;
            }
            else
            {
                ddlStatus.SelectedIndex = 1;
            }
            for(int i = 0; i < cbxProfiles.Items.Count; i++)
            {
                if(ds.Tables[0].Rows[0]["Include"].ToString().Equals("ALL"))
                {
                    cbxProfiles.Items[0].Selected = true;
                    break;
                }
                else
                {
                    cbxProfiles.Items[i].Selected = ds.Tables[0].Rows[0]["Include"].ToString().Contains("!" + cbxProfiles.Items[i].Value + "!");
                }
            }
        }
    }

    private string getProfileInclude()
    {
        string retVal = string.Empty;
        for (int i = 0; i < cbxProfiles.Items.Count; i++)
        {
            if (cbxProfiles.Items[i].Value.Equals("ALL"))
            {
                if (cbxProfiles.Items[i].Selected)
                {
                    retVal = "ALL";
                    break;
                }
            }
            else
            {
                if (cbxProfiles.Items[i].Selected)
                {
                    retVal += "!" + cbxProfiles.Items[i].Value.ToString() + "!,";
                }
            }
        }

        if (!retVal.Equals("ALL"))//remove last comma
        {
            retVal = (retVal + "!!").Replace(",!!", "");
        }

        return retVal;
    }
}