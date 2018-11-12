using Enterprise.Lib.Core.Interface;
using Enterprise.Lib.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enterprise.Lib.Service
{
    public class Service<TEntity> : IService<TEntity> where TEntity : ModelBase
    {
        private readonly IRepository<TEntity> _repository;

        /// <summary>
        /// GET All
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return _repository.GetAll();
        }
        public virtual Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        /// <summary>
        /// GET BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById(IIdentifier id)
        {
            return _repository.GetById(id);
        }

        public virtual Task<TEntity> GetByIdAsync(IIdentifier id)
        {
            return _repository.GetByIdAsync(id);
        }

        ////////////////////////////////////
        /// <summary>
        /// GET BY OBJECT
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// 
        public virtual IEnumerable<dynamic> GetAll(object item)
        {
            return _repository.GetAll(item);
        }

        public virtual Task<IEnumerable<dynamic>> GetAllAsync(object item)
        {
            return _repository.GetAllAsync(item);
        }

        ////////////////////////////////////
        /// <summary>
        /// GET BY PREDICATE
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.GetByPredicate(predicate);
        }

        public virtual Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.GetByPredicateAsync(predicate);
        }

        ////////////////////////////////////
        /// <summary>
        /// CRUD
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        public virtual dynamic Add(TEntity entity)
        {
            return _repository.Add(entity);
        }

        public virtual Task<dynamic> AddAsync(TEntity entity, bool commitChanges = true)
        {
            return  _repository.AddAsync(entity);
        }

        public virtual dynamic Delete(TEntity entity)
        {
            return _repository.Remove(entity);
        }

        public virtual Task<dynamic> DeleteAsync(TEntity entity, bool commitChanges = true)
        {
            return _repository.RemoveAsync(entity);
        }

        public virtual dynamic Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        public virtual Task<dynamic> UpdateAsync(TEntity entity, bool commitChanges = true)
        {
            return _repository.UpdateAsync(entity);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServiceLNData() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}