using System;
using System.Collections.Generic;
using System.Text;

namespace Financial.Domain
{
    public class Base
    {
        public Guid Id { get; protected set; }
        public DateTime CreateDate { get; protected set; }
        public DateTime AlterDate { get; protected set; }

    }
}
