using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enterprise.Lib.Core.Interface
{
    public interface IServiceReadOnly<TModel> : IService, IDisposable
    {
        IEnumerable<dynamic> GetCollections();
        Task<IEnumerable<dynamic>> GetCollectionsAsync();

        IEnumerable<dynamic> GetAll(object item);
        Task<IEnumerable<dynamic>> GetAllAsync(object item);

        TModel GetById(IIdentifier id);
        Task<TModel> GetByIdAsync(IIdentifier id);
        
        IEnumerable<TModel> GetByPredicate(Expression<Func<TModel, bool>> predicate);
        Task<IEnumerable<TModel>> GetByPredicateAsync(Expression<Func<TModel, bool>> predicate);


    }
}