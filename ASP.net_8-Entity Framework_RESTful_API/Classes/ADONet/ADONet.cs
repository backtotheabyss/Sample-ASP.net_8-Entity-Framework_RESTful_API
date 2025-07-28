using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    class ADONet
    {
        /* ADO.net */        
        /* connection - MySQL */
        /* public MySqlConnection connectionMySQL; */        
		
		/* connection - Access 97/2000 */
		/*public OleDbConnection connectionOLEDB;*/
		
		/* connection - SQL Server */
        public SqlConnection connectionSQLServer;

        /* connection - connection string */
        /*public const connectionStringMySQL = "SERVER=34.76.45.236; DATABASE=data_team; UID=diego;PASSWORD=J6ZMJ-XHrRb3c-JKjMWH; default command timeout=480";*/
        /* dev */
        /* public const string connectionStringMySQL = "SERVER=34.76.45.236; DATABASE=zoho_crm; UID=diego;PASSWORD=J6ZMJ-XHrRb3c-JKjMWH; default command timeout = 480"; */

        /* connection - SQL Server */
        //public const string connectionStringSQLServer = "Server=tcp:aspdotnet-7-restful-apidbserver.database.windows.net,1433;Initial Catalog=NORTHWND;Persist Security Info=False;User ID=dsendra;Password=Venom_666;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //public const string connectionStringSQLServer = "SERVER=34.76.45.236; DATABASE=zoho_crm; UID=diego;PASSWORD=J6ZMJ-XHrRb3c-JKjMWH; default command timeout = 480";
        /* dev */
        public const string connectionStringSQLServer = "Server = SCRAPING\\SQLSERVERDEV; User ID =sa; Password=Abducted27641299; Database = NORTHWND; Trusted_Connection = No; Encrypt = False;";

        /* connection - Access 97/2000 */
        /*public const string connectionStringAccess = "SERVER=34.76.45.236; DATABASE=zoho_crm; UID=diego;PASSWORD=J6ZMJ-XHrRb3c-JKjMWH; default command timeout = 480";*/

        public SqlConnection connectionOpen(SqlConnection pSQLServerConnection, int piconnectionType)
        {
            //connection - open
            switch (piconnectionType)
            {
                case 1:
                    {
                        /* connection - MYSQL - open */
                        /*pMySqlConnection = new MySqlConnection(connectionStringMySQL);
                        pMySqlConnection.Open();*/
                        break;
                    }

                case 2:
                    {
                        /* connection - SQL Server - open */
                        pSQLServerConnection = new SqlConnection(connectionStringSQLServer);
                        pSQLServerConnection.Open();
                        break;
                    }

                case 3:
                    {
                        /* connection - Access 97/2000 - open */
                        /*pconnectionOLEDB = new OleDbConnection(connectionStringAccess);
                        pconnectionOLEDB.Open();*/
                        break;
                    }
            }
            
            return (pSQLServerConnection);
        }
    }
}
