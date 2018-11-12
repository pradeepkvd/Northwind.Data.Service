using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enterprise.Lib.Core.Interface
{
    /// <summary>
    /// Interface for repository pattern
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IRepositoryReadOnly<TModel> : IDisposable
    {

        IEnumerable<TModel> GetAll();
        Task<IEnumerable<TModel>> GetAllAsync();

        IEnumerable<dynamic> GetAll(object item);
        Task<IEnumerable<dynamic>> GetAllAsync(object item);

        TModel GetById(IIdentifier id);
        Task<TModel> GetByIdAsync(IIdentifier id);

        IEnumerable<TModel> GetByPredicate(Expression<Func<TModel, bool>> predicate);
        Task<IEnumerable<TModel>> GetByPredicateAsync(Expression<Func<TModel, bool>> predicate);

    }
}