using System.Net;

namespace Messenger.Infrastructure.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message, statusCode) { }
    }
}
