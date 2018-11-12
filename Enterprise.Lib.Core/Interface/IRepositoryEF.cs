using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprise.Lib.Core.Interface
{
    public interface IRepositoryEF<T>
    {
       
            T Get<TKey>(TKey id);
            IQueryable<T> GetAll();
            void Add(T entity);
            void Update(T entity);
        
    }
}
