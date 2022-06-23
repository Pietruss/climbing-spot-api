using System;

namespace ClimbingAPI.Exceptions
{
    public class UnAuthorizeException: Exception
    {
        public UnAuthorizeException(string message) : base(message) { }
    }
}
