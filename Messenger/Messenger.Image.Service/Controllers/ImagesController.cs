using Messenger.Image.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Image.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Conversations/{conversationId}/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<IActionResult> SendImageMessage([FromForm] IFormFile image, [FromRoute] Guid conversationId)
    {
        string imageUrl = await _imageService.SaveImageAsync(image, conversationId);

        return Ok(imageUrl);
    }

    [HttpDelete("{imageFileName}")]
    public async Task<IActionResult> DeleteImageMessage([FromRoute] Guid conversationId, [FromRoute] string imageFileName)
    {
        var result = await _imageService.DeleteImageAsync(conversationId, imageFileName);

        if (result)
        {
            return NoContent(); 
        }

        return NotFound(); 
    }

}
