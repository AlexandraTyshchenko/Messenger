using Messenger.Image.Api.Interfaces;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Image.Api.Controllers
{
    [Route("api/Conversations/{conversationId}/[controller]")]
    [ApiController]
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
            ImageResultDto imageDto = await _imageService.SaveImageAsync(image, conversationId);

            return Ok(imageDto);
        }

        [HttpPost("conversationImage")]
        public async Task<IActionResult> AddConversationImage([FromForm] IFormFile image, [FromRoute] Guid conversationId)
        {
            ImageResultDto imageDto = await _imageService.AddConversationImage(image, conversationId);

            return Ok(imageDto);
        }

        [HttpDelete("{imageFileName}")]
        public async Task<IActionResult> DeleteImageMessage([FromRoute] Guid conversationId, [FromRoute] string imageFileName)
        {
            await _imageService.DeleteImageAsync(conversationId, imageFileName);

            return Ok();
        }
    }
}
