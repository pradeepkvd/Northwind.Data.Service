using System;

namespace Enterprise.Lib.Core.Interface
{
    public class IdInt : IIdentifier
    {
        public Type Type
        {
            get
            {
                return typeof(int);
            }
        }

        public dynamic Value { get; set; }
    }
}