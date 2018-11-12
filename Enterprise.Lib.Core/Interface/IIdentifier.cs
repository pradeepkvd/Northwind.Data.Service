using System;

namespace Enterprise.Lib.Core.Interface
{
    public interface IIdentifier
    {
        Type Type { get; }

        dynamic Value { get; set; }
    }
}