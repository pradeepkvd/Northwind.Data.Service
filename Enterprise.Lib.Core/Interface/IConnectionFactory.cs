using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Lib.Core.Interface
{
    public interface IConnectionFactory
    {
        dynamic GetDefaultConnection(string connectionString);
    }
}