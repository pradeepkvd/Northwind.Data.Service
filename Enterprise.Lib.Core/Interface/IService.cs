using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enterprise.Lib.Core.Interface
{
    public interface IService
    {

    }
    public interface IService<TModel> : IService, IServiceReadOnly<TModel>
    {
        dynamic Add(TModel entity);
        Task<dynamic> AddAsync(TModel entity, bool commitChanges = true);

        dynamic Update(TModel entity);
        Task<dynamic> UpdateAsync(TModel entity, bool commitChanges = true);

        dynamic Delete(TModel entity);
        Task<dynamic> DeleteAsync(TModel entity, bool commitChanges = true);
    }
}