using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Lib.Core.CO
{
    public class EventArg<T> : EventArgs
    {
        public T Message { get; set; }
        public EventArg(T message)
        {
            Message = message;
        }
    }
}
