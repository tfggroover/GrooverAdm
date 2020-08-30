using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Common
{
    public class GrooverAuthException: GrooverException
    {
        public GrooverAuthException() : base(401) { }
        public GrooverAuthException(string message) : base(message, 401) { }
    }
}
