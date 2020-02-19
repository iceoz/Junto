using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Exceptions
{
    public class UnexpectedUserException : Exception
    {
        public UnexpectedUserException(string message) : base(message)
        {
        }
    }
}
