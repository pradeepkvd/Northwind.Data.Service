using Enterprise.Lib.Core.Interface;
using Enterprise.Lib.Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enterprise.Lib.DataAccess
{
    public class RepositoryMongo<T> : IRepository<T> where T : ModelBase
    {
        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal IMongoCollection<T> collection;

        /// <summary>
        /// The database
        /// </summary>
        private IMongoDatabase _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMongo{T}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databasename">The databasename.</param>
        public RepositoryMongo(string connectionString, string databasename)
        {
            var client = new MongoClient(connectionString);
            if (client != null)
                _db = client.GetDatabase(databasename);
        }

        public dynamic Add(T item)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> AddAsync(T item, bool commitChanges = true)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetCollections()
        {
            return _db.ListCollectionNames()?.ToList();
        }

        public async Task<IEnumerable<dynamic>> GetCollectionsAsync()
        {
            return (IEnumerable<dynamic>)await _db.ListCollectionNamesAsync();
        }

        public IEnumerable<dynamic> GetAll(object item)
        {
            var collection = _db.GetCollection<BsonDocument>(item.ToString());
            var filter = new BsonDocument();
            return collection.Find(filter)?.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> GetAllAsync(object item)
        {
            throw new NotImplementedException();
        }

        public T GetById(IIdentifier id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(IIdentifier id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetByPredicate(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public dynamic Remove(T item)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> RemoveAsync(T item, bool commitChanges = true)
        {
            throw new NotImplementedException();
        }

        public dynamic Update(T item)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> UpdateAsync(T item, bool commitChanges = true)
        {
            throw new NotImplementedException();
        }
    }
}