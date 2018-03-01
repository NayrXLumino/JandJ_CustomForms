using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using CommonLibrary;

public partial class Transactions_Leave_pgePDFViewing : System.Web.UI.Page
{
    string MenuCode = "MSLEAVEPOSTING";
    protected void Page_Load(object sender, EventArgs e)
    {
        DisplayPDFFromDB();
    }

    private void DisplayPDFFromDB()
    {
        try
        {
            string EncrypSession = Session["ClncControlNo"].ToString().Trim();
            string transactiontype = Request.QueryString["vt"].ToString().Trim();
            string encrypt = Request.QueryString["dx"].ToString().Trim().Replace("'", "").Replace(" ", "+");
            string controlNo = CommonLibrary.Encrypt.decryptText(encrypt).ToString();
            controlNo = Request.QueryString["dx"].ToString().Trim().Replace("'", "");
            string filename = Request.QueryString["df"].ToString().Trim();
            string path = string.Empty;
            if (controlNo != CommonLibrary.Encrypt.decryptText(EncrypSession))
            {
                throw new Exception("");
            }

            System.Data.DataSet dsPDF = new System.Data.DataSet();
            System.Data.SqlClient.SqlConnectionStringBuilder builderCurrent
            = new System.Data.SqlClient.SqlConnectionStringBuilder(Encrypt.decryptText(Session["dbConn"].ToString()));
            string strDataSource = builderCurrent.DataSource.ToString();
            using (DALHelper dal = new DALHelper(strDataSource, "BINARYDB"))
            {
                try
                {
                    dal.OpenDB();
                    ParameterInfo[] param = new ParameterInfo[2];
                    param[0] = new ParameterInfo("@Bft_EmployeeId", controlNo);
                    param[1] = new ParameterInfo("@Bft_FileCode", MenuCode + "[" + filename + "]");
                    dsPDF = dal.ExecuteDataSet(@"
SELECT 
	Bft_BinaryValue
FROM BINARYDB..T_BinaryFile
WHERE Bft_EmployeeId = @Bft_EmployeeId
	AND Bft_FileCode = @Bft_FileCode
	AND Bft_Status = 'A'
                    ", System.Data.CommandType.Text, param);
                }
                catch(Exception er)
                {
                    Response.Write("EXCEPTION" + er.Message);
                    dsPDF = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (dsPDF != null
                    && dsPDF.Tables != null
                    && dsPDF.Tables.Count > 0
                    && dsPDF.Tables[0].Rows.Count > 0)
            {
                byte[] buffer = (byte[])dsPDF.Tables[0].Rows[0][0];
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", buffer.Length.ToString());
                Response.BinaryWrite(buffer);
            }
            else
            {
                //Response.Write("No PDF Found");
            }
        }
        catch
        {
            Response.Write("Error");
        }
    }
}