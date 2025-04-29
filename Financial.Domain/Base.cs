using System;
using System.Collections.Generic;
using System.Text;

namespace Financial.Domain
{
    public class Base
    {
        public Guid Id { get; private set; }
        public DateTime CreateDate { get; set; }

    }
}
