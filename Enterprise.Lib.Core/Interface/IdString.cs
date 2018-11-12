using System;

namespace Enterprise.Lib.Core.Interface
{
    public class IdString : IIdentifier
    {
        public Type Type
        {
            get
            {
                return typeof(String);
            }
        }

        public dynamic Value { get; set; }
    }
}