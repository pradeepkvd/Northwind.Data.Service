using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Enterprise.Lib.Core.Model;
using Enterprise.Lib.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Lib.DataAccess
{
    /// <summary>
    /// default repository using Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepositoryEF<T> where T : ModelBase
    {
        protected readonly DbContext Context;
        protected DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);

            Save();
        }

        public T Get<TKey>(TKey id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public void Update(T entity)
        {
            Save();
        }

        private void Save()
        {
            Context.SaveChanges();
        }
    }
}