using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Enterprise.Lib.Core.Interface;

namespace Enterprise.Lib.DataAccess
{
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// default conectinreturn SQLConnection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public virtual dynamic GetDefaultConnection(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error getting Connection", ex);
            }
        }

        /// <summary>
        /// this will return SQLConnection from GetDefaultConnection
        /// Do not use this if GetDefaultConnection is overriden
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static System.Data.IDbConnection GetSQLConnection(string connectionString)
        {
            return new ConnectionFactory().GetDefaultConnection(connectionString) as SqlConnection;
        }
    }
}