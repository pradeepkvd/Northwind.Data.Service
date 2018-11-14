using Enterprise.Lib.Core.Interface;
using Northwind.Customer.DAL.Mongo;
using System;
using Northwind.Customer.Core;

namespace Northwind.Service
{
    public class ServiceFactory<T>
    {
        public static IService<T> GetService()
        {
            var type = typeof(T);

            if (type == typeof(Customer.Core.Customer))
                return new CustomerDataService(new CustomerRepository()) as IService<T>;
            else
                throw new Exception($"Service factory is Unable to find service of type {typeof(T).Name}.");
        }
    }
}
