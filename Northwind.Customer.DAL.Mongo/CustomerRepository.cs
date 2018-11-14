using System;
using Enterprise.Lib.DataAccess;

namespace Northwind.Customer.DAL.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Enterprise.Lib.DataAccess.RepositoryMongo{Northwind.Customer.Core.Customer}" />
    public class CustomerRepository : RepositoryMongo<Core.Customer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="connectionstring">The connectionstring.</param>
        /// <param name="databasename">The databasename.</param>
        public CustomerRepository(string connectionstring =null, string databasename = null) : base (connectionstring, databasename)
        {

        }
    }
}
