using System;

namespace GrooverAdm.Common
{
    public class GrooverException : Exception
    {
        public GrooverException() : base() { }
        public GrooverException(string message) : base(message) { }
    }
}
