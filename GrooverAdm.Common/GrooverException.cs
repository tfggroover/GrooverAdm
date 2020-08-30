using System;

namespace GrooverAdm.Common
{
    public class GrooverException : Exception
    {
        public int HttpCode { get; set; }
        public GrooverException() : base() { this.HttpCode = 400; }
        public GrooverException(string message) : base(message) { this.HttpCode = 400; }
        public GrooverException(int httpCode) : base() 
        {
            this.HttpCode = httpCode;
        }
        public GrooverException(string message, int httpCode) : base(message) 
        {
            this.HttpCode = httpCode;
        }
    }
}
