using MediatR;
using Messenger.Business;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediatoR;
        public ConversationsController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet("byUser")]
        public async Task<IActionResult> GetConversationsByUserId([FromQuery]int userId)//
        {
            var result = await _mediatoR.Send(new GetConversationByUserId.Query { UserId = userId });
            return Ok(result);
        }
    }
}
