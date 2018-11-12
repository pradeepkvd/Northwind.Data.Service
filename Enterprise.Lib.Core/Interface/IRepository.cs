using System.Threading.Tasks;

namespace Enterprise.Lib.Core.Interface
{
    /// <summary>
    /// Interface for repository pattern
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IRepository<TModel> : IRepositoryReadOnly<TModel>
    {
        dynamic Add(TModel item);
        Task<dynamic> AddAsync(TModel item, bool commitChanges = true);

        dynamic Remove(TModel item);
        Task<dynamic> RemoveAsync(TModel item, bool commitChanges = true);

        dynamic Update(TModel item);
        Task<dynamic> UpdateAsync(TModel item, bool commitChanges = true);
    }
}