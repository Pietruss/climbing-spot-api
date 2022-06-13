using System;

namespace ClimbingAPI.Exceptions
{
    public class UnauthorizeException: Exception
    {
        public UnauthorizeException(string message) : base(message) { }
    }
}
