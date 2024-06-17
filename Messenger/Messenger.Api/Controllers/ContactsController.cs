using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ContactsController : ControllerBase
    {
        private readonly IMediator _mediatoR;

        public ContactsController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _mediatoR.Send(new GetContactsQuery { OwnerId = new Guid(userIdClaim.Value) });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromQuery]Guid contactId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var result = await _mediatoR.Send(new AddContactCommand
            {
                OwnerId = new Guid(userIdClaim.Value),
                ContactId = contactId
            });

            return Ok(result);
        }

        [HttpDelete("{userContactId:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid userContactId)
        {
            var result = await _mediatoR.Send(new DeleteContactCommand
            {
                UserContactId = userContactId
            });

            return Ok(result);
        }
    }
}
