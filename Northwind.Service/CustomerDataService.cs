using Enterprise.Lib.Service;
using Northwind.Customer.DAL.Mongo;
using System;

namespace Northwind.Service
{
    public class CustomerDataService : Service<Customer.Core.Customer>
    {
        private readonly CustomerRepository _repository;

        public CustomerDataService(CustomerRepository repository) : base(repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            _repository = (CustomerRepository)repository;
        }
    }
}
