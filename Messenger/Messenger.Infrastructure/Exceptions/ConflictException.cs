using System.Net;

namespace Messenger.Infrastructure.Exceptions
{
    public class ConflictException : CustomException
    {
        public ConflictException(string message, HttpStatusCode statusCode = HttpStatusCode.Conflict) 
            : base(message, statusCode) { }
    }
}
