using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Exceptions
{
    public class ForbidException: Exception
    {
        public ForbidException(string message): base(message)
        {
            
        }
    }
}
