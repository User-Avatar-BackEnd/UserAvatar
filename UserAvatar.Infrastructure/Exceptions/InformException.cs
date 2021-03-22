using System;
namespace UserAvatar.Infrastructure.Exceptions
{
    public class InformException : Exception
    {
        public InformException(string message) : base(message)
        {
        }
    }
}
