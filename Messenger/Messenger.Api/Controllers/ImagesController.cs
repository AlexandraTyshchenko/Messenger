using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Conversations/{conversationId}/[controller]")]
[ConversationRoleFilter(Role.Participant)]
public class ImagesController : BaseController
{
    private readonly IMediator _mediatoR;

    public ImagesController(IMediator mediator)
    {
        _mediatoR = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SendImageMessage([FromForm] IFormFile image, [FromRoute] Guid conversationId)
    {
        ResultDto<ImageDto> response = await _mediatoR.Send(new SendImageCommand
        {
            Image = image,
            ConversationId = conversationId
        });

        return response.ToHttpResponse();
    }

    [HttpDelete("{imageFileName}")]
    public async Task<IActionResult> DeleteImageMessage([FromForm] IFormFile image, 
        [FromRoute] Guid conversationId, [FromRoute] string imageFileName)
    {
        ResultDto response = await _mediatoR.Send(new DeleteImageCommand
        {
            ImageFileName = imageFileName,
            ConversationId = conversationId
        });

        return response.ToHttpResponse();
    }
}
