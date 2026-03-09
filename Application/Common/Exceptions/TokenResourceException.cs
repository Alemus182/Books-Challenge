using System;

namespace Application.Common.Exceptions
{
    public class TokenResourceException : Exception
    {
        public TokenResourceException(string message) : base(message)
        {
        }
    }

}
