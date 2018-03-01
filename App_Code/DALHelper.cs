using System;
using System.Data;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using CommonLibrary;

namespace Payroll.DAL
{
    public class DALHelper : BaseDAL, IDisposable
    {
        private const int BatchSize = 10;
        private SqlConnection dbConn;
        private SqlTransaction trans;
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        #region constructor

        public DALHelper()
        {
            ////if (ConfigurationManager.AppSettings["Confidential"].ToString() == "FALSE")
            ////    this.dbConn = this.getConnection();
            ////else
            this.dbConn = this.getLocalConnection(session["dbConn"].ToString());
            //this.dbConn = "Data Source=192.168.135.80;Initial Catalog=PAYROLLGENIE_EPSON20140728;Persist Security Info=True;User ID=SA;Password=$y$t3m@dM!n";
        }

        //charlie edited 06032008
        public DALHelper(bool flag)
        {
            if (flag)
                this.dbConn = this.getConnectionDTR();
            else
                this.dbConn = this.getConnectionConfi();
        }

        //Andre commented 20100906: resused constructor - public DALHelper(string connectionString)
        //public DALHelper(string DB)
        //{
        //    this.dbConn = this.getConnection();
        //}

        public DALHelper(string datasource, string DBName)
        {
            this.dbConn = this.getLocalConnection(datasource, DBName);
        }

        public DALHelper(string connectionString)
        {
            this.dbConn = this.getLocalConnection(connectionString);
        }

        #endregion//constructor

        #region methods

        #region public

        #region others

        public void OpenDB()
        {
            if (this.dbConn.State == ConnectionState.Closed)
            {
                try
                {
                    this.dbConn.Open();
                }
                catch (Exception e)
                {
                    //CommonProcedures.showMessageError(e.ToString());
                }
            }
        }

        public void CloseDB()
        {
            if (this.dbConn.State == ConnectionState.Open)
                this.dbConn.Close();
        }

        public void BeginTransaction()
        {
            if (this.trans == null)
                this.trans = this.dbConn.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.trans.Commit();
            this.trans = null;
        }

        public void RollBackTransaction()
        {
            this.trans.Rollback();
            this.trans = null;
        }

        // 03/17/2007
        // kris
        // added for transaction level snapshot isolation
        public void BeginTransactionSnapshot()
        {
            //if (this.trans == null)
            //{
            //    this.trans = this.dbConn.BeginTransaction(IsolationLevel.Snapshot);

            //}
            BeginTransaction();
        }

        public void BeginTransactionSnapshot(string transactionName)
        {
            if (this.trans == null)
            {
                //this.trans = this.dbConn.BeginTransaction(IsolationLevel.Snapshot, transactionName);
                this.trans = this.dbConn.BeginTransaction(transactionName);
            }
        }

        public void CommitTransactionSnapshot()
        {
            //this.trans.Commit();
            CommitTransaction();
        }

        public void RollBackTransactionSnapshot()
        {
            //this.trans.Rollback();
            RollBackTransaction();
        }
        // end of 03/17/2007

        public DataTable GetTableStructure(string tableName)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.FillSchema(dt, SchemaType.Source);

            return dt;
        }

        public DataTable GetTableStructure(string strqry, bool istablename)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = this.dbConn.CreateCommand();
            if (!istablename)
                cmd.CommandText = strqry;
            else
                cmd.CommandText = string.Format("SELECT * FROM {0}", strqry);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.FillSchema(dt, SchemaType.Source);

            return dt;
        }

        #endregion//others

        #region batchUpdate

        public void BatchUpdate(DataTable dt, string tableName)
        {
            this.BatchUpdate(dt, tableName, true, BatchSize);
        }

        public void BatchUpdate(DataTable dt, string tableName, bool continueOnError)
        {
            this.BatchUpdate(dt, tableName, continueOnError, BatchSize);
        }

        public void BatchUpdate(DataTable dt, string tableName, bool continueOnError, int batchSize)
        {
            ///*SqlCommand cmd = this.dbConn.CreateCommand();
            //cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            //cmd.CommandType = CommandType.Text;

            //if (this.trans != null)
            //    cmd.Transaction = this.trans;

            //dt.TableName = tableName;*/

            //SqlDataAdapter adapter = new SqlDataAdapter(string.Format("SELECT * FROM {0}", tableName), this.dbConn);
            ////adapter.SelectCommand = cmd;
            ////adapter.UpdateBatchSize = batchSize;
            ////adapter.ContinueUpdateOnError = continueOnError;

            //if (this.trans != null)
            //    adapter.SelectCommand.Transaction = this.trans;

            //SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
            ////cmdBuilder.SetAllValues = false;
            //cmdBuilder.RefreshSchema();

            //adapter.InsertCommand = cmdBuilder.GetInsertCommand();
            //adapter.UpdateCommand = cmdBuilder.GetUpdateCommand();
            //adapter.DeleteCommand = cmdBuilder.GetDeleteCommand();

            //DataSet ds = new DataSet();

            //ds.Tables.Add(dt.Copy());



            //int x = adapter.Update(ds);

            //x = x + 1;

            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = 0;
            cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
            cmd.CommandType = CommandType.Text;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            dt.TableName = tableName;

            SqlDataAdapter adapter = new SqlDataAdapter(string.Format("SELECT * FROM {0}", tableName), this.dbConn);
            adapter.SelectCommand = cmd;
            adapter.UpdateBatchSize = batchSize;
            adapter.ContinueUpdateOnError = continueOnError;

            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
            cmdBuilder.SetAllValues = false;

            adapter.Update(dt);
        }

        #endregion//batchUpdate

        #region executeNonQuery

        public int ExecuteNonQuery(string sqlStatement)
        {
            return this.ExecuteNonQuery(sqlStatement, CommandType.Text, null);
        }

        public int ExecuteNonQuery(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteNonQuery(sqlStatement, cmdType, null);
        }

        public int ExecuteNonQuery(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            int affectedRows = 0;
            bool isOutputParameter = false;


            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = 0;
            cmd.CommandText = sqlStatement;
            cmd.CommandType = cmdType;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (paramCol != null)
            {
                foreach (ParameterInfo prmInfo in paramCol)
                {
                    SqlParameter prm = new SqlParameter();

                    prm.ParameterName = prmInfo.Name;
                    prm.SqlDbType = prmInfo.DataType;
                    prm.Size = prmInfo.Size;
                    prm.Value = prmInfo.Value;
                    prm.Direction = prmInfo.Direction;

                    cmd.Parameters.Add(prm);
                }
            }

            affectedRows = cmd.ExecuteNonQuery();

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                isOutputParameter = (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.Output) || (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.InputOutput) || (((SqlParameter)cmd.Parameters[i]).Direction == ParameterDirection.ReturnValue);

                if (isOutputParameter)
                    paramCol[i].Value = ((SqlParameter)cmd.Parameters[i]).Value;
            }


            return affectedRows;
        }

        #endregion//executeNonQuery

        #region executeReader

        public SqlDataReader ExecuteReader(string sqlStatement)
        {
            return this.ExecuteReader(sqlStatement, CommandType.Text, null, CommandBehavior.Default);
        }

        public SqlDataReader ExecuteReader(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteReader(sqlStatement, cmdType, null, CommandBehavior.Default);
        }

        public SqlDataReader ExecuteReader(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol, CommandBehavior behavior)
        {
            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = 0;
            cmd.CommandText = sqlStatement;
            cmd.CommandType = cmdType;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (paramCol != null)
            {
                foreach (ParameterInfo prmInfo in paramCol)
                {
                    SqlParameter prm = new SqlParameter();

                    prm.ParameterName = prmInfo.Name;
                    prm.SqlDbType = prmInfo.DataType;
                    prm.Size = prmInfo.Size;
                    prm.Value = prmInfo.Value;
                    prm.Direction = prmInfo.Direction;

                    cmd.Parameters.Add(prm);
                }
            }

            return cmd.ExecuteReader(behavior);

        }

        #endregion//executeReader

        #region executeScalar

        public object ExecuteScalar(string sqlStatement)
        {
            return this.ExecuteScalar(sqlStatement, CommandType.Text, null);
        }

        public object ExecuteScalar(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteScalar(sqlStatement, cmdType, null);
        }

        public object ExecuteScalar(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            SqlCommand cmd = this.dbConn.CreateCommand();
            cmd.CommandTimeout = 0;
            cmd.CommandText = sqlStatement;
            cmd.CommandType = cmdType;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (paramCol != null)
            {
                foreach (ParameterInfo prmInfo in paramCol)
                {
                    SqlParameter prm = new SqlParameter();

                    prm.ParameterName = prmInfo.Name;
                    prm.SqlDbType = prmInfo.DataType;
                    prm.Size = prmInfo.Size;
                    prm.Value = prmInfo.Value;
                    prm.Direction = prmInfo.Direction;

                    cmd.Parameters.Add(prm);
                }
            }

            return cmd.ExecuteScalar();
        }

        #endregion//executeScalar

        #region executeDataSet

        public DataSet ExecuteDataSet(string sqlStatement)
        {
            return this.ExecuteDataSet(sqlStatement, CommandType.Text, null);
        }

        public DataSet ExecuteDataSet(string sqlStatement, CommandType cmdType)
        {
            return this.ExecuteDataSet(sqlStatement, cmdType, null);
        }

        public DataSet ExecuteDataSet(string sqlStatement, CommandType cmdType, ParameterInfo[] paramCol)
        {
            DataSet dataSet = new DataSet();

            try
            {
                SqlCommand cmd = this.dbConn.CreateCommand();
                cmd.CommandTimeout = 0;
                cmd.CommandText = sqlStatement;
                cmd.CommandType = cmdType;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                if (paramCol != null)
                {
                    foreach (ParameterInfo prmInfo in paramCol)
                    {
                        SqlParameter prm = new SqlParameter();

                        prm.ParameterName = prmInfo.Name;
                        prm.SqlDbType = prmInfo.DataType;
                        prm.Size = prmInfo.Size;
                        prm.Value = prmInfo.Value;
                        prm.Direction = prmInfo.Direction;

                        cmd.Parameters.Add(prm);
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(dataSet);

                //arthur added 20070121 start
                //code below will trim all text fields 
                foreach (DataTable dt in dataSet.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (dt.Columns[i].DataType == Type.GetType("System.String", true))
                                row[i] = row[i].ToString().TrimEnd();
                        }
                    }
                }
                //end
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return dataSet;
        }

        #endregion//executeDataSet

        #endregion//public

        #region private

        private SqlConnection getConnection()
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameNonConfi"].ToString());
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["PayrollConnectionString"].ConnectionString, param);
            
            return new SqlConnection(connectionString);
        }

        private SqlConnection getConnectionConfi()
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["ProfileDB"].ToString());
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());
            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["ProfileConnectionString"].ConnectionString,param);

            return new SqlConnection(connectionString);
        }

        //charlie edited 06032008
        private SqlConnection getConnectionDTR()
        {
            string[] param = new string[4];
            param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
            param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameDTR"].ToString());
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());

            string connectionString = string.Format(ConfigurationManager.ConnectionStrings["dtrConnectionString"].ConnectionString, param);

            return new SqlConnection(connectionString);
        }

        private SqlConnection getLocalConnection(string datasource, string DBName)
        {
            string[] param = new string[4];
            param[0] = datasource;
            param[1] = DBName;
            param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
            param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());
            string connectionString = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}"
                , param);

            return new SqlConnection(connectionString);
        }

        private SqlConnection getLocalConnection(string connectionString)
        {   
            string conn = Encrypt.decryptText(connectionString);
            return new SqlConnection(conn);
        }

        #endregion//private

        #endregion//methods

        #region IDisposable

        public void Dispose()
        {
            if (this.dbConn != null && this.dbConn.State != ConnectionState.Closed)
                this.dbConn.Close();

            GC.SuppressFinalize(this);
        }

        #endregion//IDisposable

    }//DALHelper
}