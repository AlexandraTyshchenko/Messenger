using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Options;
using Messenger.Client.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Commands;

public class DeleteImageCommand : IRequest<ResultDto>
{
    public string ImageFileName { get; set; }
    public Guid ConversationId { get; set; }
}


public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, ResultDto>
{
    private readonly IImageClient _imageClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteImageCommandHandler(IImageClient imageClient, IOptions<ImageFormatsSettings> imageServiceSettings, IHttpContextAccessor httpContextAccessor)
    {
        _imageClient = imageClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResultDto> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        return await _imageClient.DeleteImageAsync(request.ImageFileName, request.ConversationId);
    }
}
