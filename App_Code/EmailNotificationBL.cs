using System;
using System.Data;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using CommonLibrary;

namespace Payroll.DAL
{

    /// <summary>
    /// Summary description for EmailNotificationBL
    /// </summary>
    public class EmailNotificationBL
    {
        System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
        #region Enum
        public enum TransactionType
        {
            [StringValue("OVERTIME")]
            OVERTIME = 0,
            [StringValue("LEAVE")]
            LEAVE = 1,
            [StringValue("TIMEMOD")]
            TIMEMOD = 2,
            [StringValue("ADDRESS")]
            ADDRESS = 3,
            [StringValue("BENEFICIARY")]
            BENEFICIARY = 4,
            [StringValue("TAXCIVIL")]
            TAXCIVIL = 5,
            [StringValue("GROUP")]
            GROUP = 6,
            [StringValue("COSTCENTER")]
            COSTCENTER = 7,
            [StringValue("SHIFT")]
            SHIFT = 8,
            [StringValue("RESTDAY")]
            RESTDAY = 9,
            [StringValue("JOBMOD")]
            JOBMOD = 10,
            [StringValue("FLEXTIME")]
            FLEXTIME = 11,
            [StringValue("STRAIGHTWORK")]
            STRAIGHTWORK = 12
        }

        public enum Action
        {
            [StringValue("ENDORSE")]
            ENDORSE = 0,
            [StringValue("APPROVE")]
            APPROVE = 1,
            [StringValue("RETURN")]
            RETURN = 2,
            [StringValue("DISAPPROVE")]
            DISAPPROVE = 3
        }
        #endregion

        private TransactionType _transactionProperty;

        private Action _actionProperty;

        public TransactionType TransactionProperty 
        {   
            get 
            { 
                return _transactionProperty; 
            } 
            set 
            { 
                _transactionProperty = value ;
            } 
        }

        public Action ActionProperty
        {
            get
            {
                return _actionProperty;
            }
            set
            {
                _actionProperty = value;
            }
        }

        public EmailNotificationBL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public EmailNotificationBL( string controlNo
                                  , EmailNotificationBL.TransactionType type
                                  , EmailNotificationBL.Action action
                                  , string userLogged
                                  , DALHelper dal)
        {
            this.InsertInfoForNotification(controlNo, type, action, userLogged, dal);
        }

        public void InsertInfoForNotification(string controlNo
                                             , string userLogged
                                             , DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@Ent_Controlno", string.Empty, SqlDbType.VarChar, 12);
            param[1] = new ParameterInfo("@Ent_TransactionType", _transactionProperty);
            param[2] = new ParameterInfo("@Ent_Action", _actionProperty);
            param[3] = new ParameterInfo("@Usr_login", userLogged);

            string sqlInsert = @" UPDATE T_EmailNotification
                                     SET Ent_Status = 'X' --no need to notify there is a new movement of the transaction
                                   WHERE Ent_ControlNo = @Ent_ControlNo
                                     AND Ent_Status = 'A'

					                INSERT INTO T_EmailNotification
						                (Ent_ControlNo
						                ,Ent_SeqNo
						                ,Ent_TransactionType
						                ,Ent_Action
						                ,Ent_Status
						                ,Usr_Login
						                ,Ludatetime)
					                VALUES
						                (@Ent_ControlNo
						                ,ISNULL((SELECT RIGHT('000' + CONVERT(VARCHAR, MAX(CONVERT(INT, Ent_SeqNo)) + 1), 3) FROM T_EmailNotification WHERE Ent_ControlNo = @Ent_ControlNo), '001')
						                ,@Ent_TransactionType
						                ,@Ent_Action
						                ,'A'
						                ,@Usr_Login
						                ,GETDATE())";

            string[] ctrlNumbers = controlNo.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < ctrlNumbers.Length; i++)
                {
                    param[0] = new ParameterInfo("@Ent_Controlno", ctrlNumbers[i], SqlDbType.VarChar, 12);
                    dal.ExecuteNonQuery(sqlInsert, CommandType.Text, param);
                }
            }
            catch (Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
            }
        }

        public void InsertInfoForNotification( string controlNo
                                             , EmailNotificationBL.TransactionType type
                                             , EmailNotificationBL.Action action
                                             , string userLogged
                                             , DALHelper dal)
        {
            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@Ent_Controlno", string.Empty);
            param[1] = new ParameterInfo("@Ent_TransactionType", type.ToString());
            param[2] = new ParameterInfo("@Ent_Action", action.ToString());
            param[3] = new ParameterInfo("@Usr_login", userLogged);

            string sqlInsert = @" INSERT INTO T_EmailNotification
                                  SELECT [Control No]
	                                   , REPLICATE('0', 3 - LEN(Convert(int,MAX(ISNULL(Ent_SeqNo, 0)) + 1))) 
	                                   + Convert(varchar(3),Convert(int,MAX(ISNULL(Ent_SeqNo, 0)) + 1))
	                                   , [Transaction Type]
	                                   , [Action]
	                                   , [Status]
	                                   , [User Login]
	                                   , GETDATE() [Ludatetime]
	                                FROM ( SELECT @Ent_ControlNo [Control No]
                                                , @Ent_TransactionType [Transaction Type]
                                                , @Ent_Action [Action]
                                                , 'A' [Status]
                                                , @Usr_Login [User Login]) AS TEMP
                                    LEFT JOIN T_EmailNotification
	                                  ON Ent_ControlNo = @Ent_ControlNo
                                   GROUP BY [Control No]
		                                  , [Transaction Type]
		                                  , [Action]
		                                  , [Status]
		                                  , [User Login]  ";
            
            string[] ctrlNumbers = controlNo.Split(new char[1]{','}, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < ctrlNumbers.Length; i++)
                {
                    param[0] = new ParameterInfo("@Ent_Controlno", ctrlNumbers[i]);
                    dal.ExecuteNonQuery(sqlInsert, CommandType.Text, param);
                }
            }
            catch(Exception ex)
            {
                CommonMethods.ErrorsToTextFile(ex, session["userLogged"].ToString());
            }
        }

        
    }
}