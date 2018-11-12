using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Enterprise.Lib.Core.Model;
using Enterprise.Lib.Core.Interface;
using Dapper;
using System.Threading.Tasks;

namespace Enterprise.Lib.DataAccess
{
    /// <summary>
    /// default repository using Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : ModelBase
    {
        private bool _disposed;

        public string TableName { get; set; }

        public string ConnectionString { get; set; }
        public string SchemaName { get; set; }

        public Repository(string connectionString, string tableName = null, string schemaName = null)
        {
            ConnectionString = connectionString;
            TableName = tableName ?? typeof(T).Name; // this will assign table name name of the Model
            SchemaName = schemaName ?? "dbo";
        }

        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        /// <summary>
        /// GET ALL
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T>GetAll()
        {
            IEnumerable<T> items = null;

            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                items = conn.Query<T>($"SELECT * FROM {TableName}");
            }

            return items;

            //using (var conn = ConnectionFactory.GetDbConnection())
            //{
            //    var queryStr = DBScriptFactory.GetScript(typeof(T));
            //    return conn.Query<T>(queryStr);
            //}
        }
        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.Run(() => GetAll());
        }

        /// <summary>
        /// FIND BY OBJECT
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> GetAll(object item)
        {
            IEnumerable<T> items = null;

            var whereQuery = DynamicQuery.GetWhereQuery(item, true);

            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                //items = conn.Query<T>(result.Sql, (object)result.Param);
                items = conn.Query<T>($"SELECT * FROM {TableName} WHERE {whereQuery}");
            }

            return items;
        }
        public virtual Task<IEnumerable<dynamic>> GetAllAsync(object item)
        {
            return Task.Run(() => GetAll(item));
        }
        /// <summary>
        /// FIND BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(IIdentifier id)
        {
            T item = default(T);

            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                object idVal = Convert.ChangeType(id.Value, id.Type);

                item = conn.Query<T>($"SELECT * FROM {TableName} WHERE Id=@Id", new { Id = idVal }).SingleOrDefault();
            }

            return item;
        }
        public virtual Task<T> GetByIdAsync(IIdentifier id)
        {
            return Task.Run(() => GetById(id));
        }

        /// <summary>
        /// FIND BY PREDICATE
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetByPredicate(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> items = null;

            // extract the dynamic sql query and parameters from predicate
            QueryResult result = DynamicQuery.GetDynamicQuery(TableName, predicate);

            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                items = conn.Query<T>(result.Sql, (object)result.Param);
            }

            return items;
        }
        public virtual Task<IEnumerable<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.Run(() => GetByPredicate(predicate));
        }

        

        public virtual dynamic Add(T item)
        {
            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                var parameters = (T)Mapping(item);

                item.Id = (Guid)conn.Insert<Guid>(TableName, parameters);
            }

            return item.Id;
        }
        public virtual Task<dynamic> AddAsync(T item, bool commitChanges = true)
        {
            return Task.Run(() => Add(item));
        }

        public virtual dynamic Update(T item)
        {
            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                var parameters = (object)Mapping(item);
                conn.Update(TableName, parameters);
            }

            return true;
        }
        public virtual Task<dynamic> UpdateAsync(T item, bool commitChanges = true)
        {
            return Task.Run(() => Update(item));
        }

        public virtual dynamic Remove(T item)
        {
            using (var conn = ConnectionFactory.GetSQLConnection(ConnectionString))
            {
                conn.Execute($"DELETE FROM {TableName} WHERE Id=@Id", new { Id = item.Id });
            }

            return true;
        }
        public virtual Task<dynamic> RemoveAsync(T item, bool commitChanges = true)
        {
            return Task.Run(() => Remove(item));
        }

            public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                ConnectionFactory.GetSQLConnection(ConnectionString).Dispose();
            }
            _disposed = true;
        }
    }
}