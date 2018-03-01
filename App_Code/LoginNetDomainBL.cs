using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Configuration;
using CommonLibrary;


namespace Payroll.DAL
{
    public class LoginNetDomainBL
    {
        string ADMappingDB;
        string ADDomainName;
        string ProfileDB;
        string ADDataSource;
        string ADMappingDBUserID;
        string ADMappingDBPassword;

        public LoginNetDomainBL()
        {
            try
            {
                ADMappingDB = Encrypt.decryptText(ConfigurationManager.AppSettings["ADMappingDB"].ToString());
                ADDomainName = Encrypt.decryptText(ConfigurationManager.AppSettings["ADDomainName"].ToString());
                ProfileDB = Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString());
                ADDataSource = Encrypt.decryptText(ConfigurationManager.AppSettings["ADDataSource"].ToString());
                ADMappingDBUserID = Encrypt.decryptText(ConfigurationManager.AppSettings["ADMappingDBUserID"].ToString());
                ADMappingDBPassword = Encrypt.decryptText(ConfigurationManager.AppSettings["ADMappingDBPassword"].ToString());
                //checkDomainConnection();
            }
            catch
            { 
                
            }
        }

        /// <summary>
        /// check the connection of Active Directory Domain/Server
        /// </summary>
        /// <returns>true or false</returns>
        public static DALHelper GetDALHelperClinic()
        {
            //string Cnck = Resources.ClinicResource.CLINICSYSCNCK;
            //System.Data.SqlClient.SqlConnectionStringBuilder builderCurrent
            //    = new System.Data.SqlClient.SqlConnectionStringBuilder(new DALHelper().GetActiveConnectionString());
            //System.Data.SqlClient.SqlConnectionStringBuilder builderClinic
            //    = new System.Data.SqlClient.SqlConnectionStringBuilder(Cnck);
            //builderClinic.DataSource = builderCurrent.DataSource;
            //DALHelper dal = new DALHelper(builderClinic.ConnectionString);
            //return dal;
            return null;
        }
        public static string GetActiveClinicConnectionString()
        {
            //string Cnck = Resources.ClinicResource.CLINICSYSCNCK;
            //System.Data.SqlClient.SqlConnectionStringBuilder builderCurrent
            //    = new System.Data.SqlClient.SqlConnectionStringBuilder(new DALHelper().GetActiveConnectionString());
            //System.Data.SqlClient.SqlConnectionStringBuilder builderClinic
            //    = new System.Data.SqlClient.SqlConnectionStringBuilder(Cnck);
            //builderClinic.DataSource = builderCurrent.DataSource;
            //DALHelper dal = new DALHelper(builderClinic.ConnectionString);
            //return builderClinic.ConnectionString;
            return null;
        }
        public bool checkDomainConnection(string username)
        {
            #region check if the Domain Server is available
            DirectoryEntry de = new DirectoryEntry();

            de.Path = "LDAP://" + ADDomainName;
            de.AuthenticationType = AuthenticationTypes.Secure;
            de.Username = username;
            //de.Password = username.ToLower(); //this is required
            
            // Set the filter - searching for user name only
            DirectorySearcher search = new DirectorySearcher(de);
            search.Filter = String.Format("(SAMAccountName={0})", username);            
            search.PropertiesToLoad.Add("SAMAccountName");             
            SearchResult results = null;
            try
            {
                results = search.FindOne();
                de = results.GetDirectoryEntry();
                if (results == null)
                {
                    //invalid username: return 400800; -- sample only
                    return false;
                }
            }
            catch(Exception ex)
            {                
                //the server is not operational
                return false;
            }
            return true;
            #endregion
        }

        /// <summary>
        /// checks the inputted username and password  in ADMappingDB
        /// </summary>
        /// <param name="username">Active Directory Username</param>
        /// <param name="password">Active Directory Password</param>
        /// <returns></returns>
        public string returnID(string username)
        {
            string Usercode = string.Empty;
            string sql = string.Format(@"SELECT  Adu_UserCode
                                 FROM T_ADUserMap WHERE Adu_System='HRC'  
                                    and  Adu_AccountName = '{0}'", username);
           
            //using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB, ADMappingDBUserID, ADMappingDBPassword))
            using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB))
            {
                try
                {
                    dal.OpenDB();
                    Usercode = dal.ExecuteScalar(sql, CommandType.Text).ToString();
                    dal.CloseDB();
                }
                catch  {  }
            }
            return Usercode;
        }
        public bool checkLogin(string username, string password)
        {
            DataTable dtResult = new DataTable();

            #region  check if inputted user is in ADMappingDB
            string sql = string.Format(@"SELECT Adu_AccountName , Adu_UserCode
                                 FROM T_ADUserMap WHERE Adu_System='HRC'  
                                    and  Adu_AccountName = '{0}'", username);
            // using (DALHelper dal = new DALHelper("WIN7DT-PC\MSSQL2012", "ActiveDirectoryMapping", "sa", "systemadmin"))
            //using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB, ADMappingDBUserID, ADMappingDBPassword))
            using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(sql,CommandType.Text).Tables[0];
                dal.CloseDB();
            }
            #endregion
          
            if (dtResult.Rows.Count > 0)
            {
                #region Active Directory Authentication
                 
                DirectoryEntry de = new DirectoryEntry();                
                //de.Path = "LDAP://servername/CN=users,DC=treyresearch,DC=net";
                de.Path = "LDAP://" + ADDomainName;
                de.AuthenticationType = AuthenticationTypes.Secure;
                de.Username = username;
                de.Password = password;

                // Set the filter - searching for user name only
                DirectorySearcher search = new DirectorySearcher(de);
                search.Filter = String.Format("(SAMAccountName={0})", username); 
                //string.Format("(objectClass={0})", '*');
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("SAMAccountName");
                search.PropertiesToLoad.Add("memberOf");
                SearchResult results = null;
                try
                {
                    
                    results = search.FindOne();
                    de = results.GetDirectoryEntry();
                    if (results == null)
                    {
                        //invalid username: return 400800; <-- sample only
                        return false;
                    }
                }
                catch (Exception ex)
                { 
                    throw new Exception(ex.Message);
                }

                // Validate the user password
                DirectoryEntry userEntry = results.GetDirectoryEntry();
                userEntry.Password = password;
                userEntry.Username = ADDomainName + "\\" + username;
                userEntry.AuthenticationType = AuthenticationTypes.Secure;

                try
                {
                    // Binding the underlying com object will cause
                    // this to validate
                    Object temp = userEntry.NativeObject;
                }
                catch (Exception e)
                {
                    return false; //invalid password
                }
                return true;
                #endregion
            }
            else
            {
                return false; 
                //error: username not found or not mapped with utility tool
                //error: return 40060;
            }
        }
        
        /// <summary>
        /// retrieves the list of HRC Databases (T_UserProfile)
        /// </summary>
        /// <param name="username">Username used in HRC</param>
        /// <returns>DataSet</returns>
        public DataSet retrieveHRCDatabase(string username)
        {
            try
            {
                DataSet dsResult = new DataSet();

                string sql = string.Format(@"SELECT  Prf_DatabaseNo
                                    ,Prf_Profile
                                 FROM {0}..T_ADUserMap 
                                 INNER JOIN {1}..T_UserProfile 
                                 ON  Upt_UserCode =Adu_UserCode                               
                                 INNER JOIN {1}..T_Profiles 
                                 ON Prf_DatabaseNo = Upt_DatabaseNo
                                 WHERE Adu_System='HRC'  
                                    AND  Adu_AccountName = '{2}' 
                                    AND Prf_Status = 'A' ", ADMappingDB, ProfileDB, username);
                //  using(DALHelper dal = new DALHelper())
               //using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB, ADMappingDBUserID, ADMappingDBPassword))
                using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB))
                {
                    dal.OpenDB();
                    dsResult = dal.ExecuteDataSet(sql, CommandType.Text);
                    dal.CloseDB();
                }
                if (dsResult.Tables.Count > 0)
                {
                    return dsResult;
                }
                else
                {
                    return null;
                }
            }
            catch
            { 
                //if there are errors in configuration then return null
                return null;
            }
        }

        /// <summary>
        /// retrieves the HRC User equivalent of Active Directory User Account
        /// </summary>
        /// <param name="ADUsername">Active Directory Username</param>
        /// <returns>string</returns>
        public bool isMappID(string ID,string system)
        {
            bool isMapp = false;
            
            if (checkDBExists(ADMappingDB))
            { 
            using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB))
            {
                 try
                {
                            dal.OpenDB();
                            isMapp = Convert.ToBoolean(Int32.Parse(dal.ExecuteScalar(string.Format(@"IF EXISTS(SELECT TOP 1 Adu_UserCode FROM T_ADUserMap
                                                                                    WHERE Adu_UserCode='{0}'
                                                                                    AND Adu_System='{1}') 
                                                                                    BEGIN
                                                                                    SELECT '1'
                                                                                    END
                                                                                    ELSE
                                                                                    SELECT '0'", ID.Trim(), system)).ToString()));
                       
                }
                        catch { }
                        finally { dal.CloseDB(); }
                }      
            }
            return isMapp;
        }
        public string retrieveHRCUser(string ADUsername)
        {
            try
            {
                DataSet dsResult = new DataSet();

                if (checkDBExists(ADMappingDB))
                {
                    string sql = string.Format(@"SELECT  Adu_UserCode
                                 FROM {0}..T_ADUserMap 
                                 INNER JOIN {1}..T_UserProfile 
                                 ON  Upt_UserCode =Adu_UserCode                               
                                 INNER JOIN {1}..T_Profiles 
                                 ON Prf_DatabaseNo = Upt_DatabaseNo
                                 WHERE Adu_System='HRC'  
                                    AND  Adu_AccountName = '{2}' 
                                    AND Prf_Status = 'A' ", ADMappingDB, ProfileDB, ADUsername);
                    //using(DALHelper dal = new DALHelper())
                    //using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB, ADMappingDBUserID, ADMappingDBPassword))
                    using (DALHelper dal = new DALHelper(ADDataSource, ADMappingDB))
                    {
                        dal.OpenDB();
                        dsResult = dal.ExecuteDataSet(sql, CommandType.Text);
                        dal.CloseDB();
                    }
                    if (dsResult.Tables.Count > 0)
                    {
                        return dsResult.Tables[0].Rows[0]["Adu_UserCode"].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                { //db doesn't exist
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// check the db if it exists in master db of sql server
        /// </summary>
        /// <param name="DBName">database name</param>
        /// <returns>true/false</returns>
        public bool checkDBExists(string DBName)
        { 
            DataSet dsResult = null;
            string sql = string.Format(@"SELECT name FROM master.dbo.sysdatabases 
                                                        WHERE ( name = '{0}')",DBName);
            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                dsResult = dal.ExecuteDataSet(sql, CommandType.Text);
                dal.CloseDB();
            }
            if (dsResult.Tables[0].Rows.Count > 0) // || dsResult.Tables[0].Rows[0]["name"].ToString() != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
