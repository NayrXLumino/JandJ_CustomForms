using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Payroll.DAL;
using System.Data;

public partial class Transactions_Payroll_pgePaySlipViewer3 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.UrlReferrer == null)
        {
            DataSet dsPDF = null;

            #region Retrieval of PDF from Database
            try
            {
                DALHelper dal2 = new DALHelper(false);
                string query = string.Format(@"SELECT TOP 1 Prf_Database as DatabaseName
                                                FROM T_PROFILES
                                                WHERE Prf_ProfileType = 'F'");
                DataSet dsquery = dal2.ExecuteDataSet(query);


                #region Get Data
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {

                        dal.OpenDB();
                        ParameterInfo[] param = new ParameterInfo[2];
                        param[0] = new ParameterInfo("@Bft_EmployeeID", Session["employeeID"].ToString());
                        param[1] = new ParameterInfo("@Bft_FileCode", Session["fileCode"].ToString());
                        dsPDF = dal.ExecuteDataSet(string.Format(@"
                        SELECT 
	                        Bft_BinaryValue
                        FROM {0}..T_BinaryFile
                        WHERE Bft_EmployeeID = @Bft_EmployeeID
	                        AND Bft_FileCode = @Bft_FileCode
                            AND Bft_Status='A'", dsquery.Tables[0].Rows[0]["DatabaseName"].ToString()), CommandType.Text, param);
                    }
                    catch (Exception er)
                    {
                        dsPDF = null;
                        CommonMethods.ErrorsToTextFile(er, "Connect to Binary DB ");
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

                #endregion
            }
            catch (Exception er)
            {
                CommonMethods.ErrorsToTextFile(er, "GeneratePDF()");
            }
            #endregion

            #region Display PDF
            try
            {
                if (dsPDF != null
                        && dsPDF.Tables != null
                        && dsPDF.Tables.Count > 0
                        && dsPDF.Tables[0].Rows.Count > 0)
                {
                    byte[] buffer = (byte[])dsPDF.Tables[0].Rows[0][0];
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", buffer.Length.ToString());
                    Response.BinaryWrite(buffer);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    throw new Exception("Payslip is not yet available");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Payslip is not yet available");
            }
            #endregion
        }
    }
}