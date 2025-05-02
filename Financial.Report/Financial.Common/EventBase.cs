using System;
using System.Collections.Generic;
using System.Text;

namespace Financial.Common
{
    public class EventBase<T>
    {
        public T Entity { get; set; }
        public string Idempotency { get; set; }
        public DateTime Date { get; set; }
    }
}
