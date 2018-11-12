using System;

namespace Enterprise.Lib.Core.Interface
{
    public class IdGuid : IIdentifier
    {
        public Type Type => typeof(Guid);

        public dynamic Value { get; set; }
    }
}