using System;

namespace ClimbingAPI.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message) : base(message)
        { }
    }
}
