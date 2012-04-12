using System;

namespace Sinks
{
    public class DataSinkException : Exception
    {
        public DataSinkException(string s) : base(s)
        {
        }
    }
}