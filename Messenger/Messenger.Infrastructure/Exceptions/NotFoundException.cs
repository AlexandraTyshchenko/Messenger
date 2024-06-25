using System.Net;

namespace Messenger.Infrastructure.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound) 
            : base(message, statusCode) { }
    }
}
