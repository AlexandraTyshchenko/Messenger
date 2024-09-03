using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Business.Validators;
using Messenger.Client.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Messenger.Business.Commands;

public class CreateGroupConversationCommand : IRequest<ResultDto<ConversationDto>>
{
    public GroupModelDto Group { get; set; }
    public Guid CreatorUserId { get; set; }
}

public class CreateGroupConversationCommandValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationCommandValidator(IOptions<ImageFormatsSettings> imageServiceSettings)
    {
        RuleFor(x => x.Group)
            .NotNull()
            .WithMessage("Group cannot be null.");

        RuleFor(x => x.Group.Title)
            .NotNull()
            .WithMessage("Group title cannot be null.")
            .NotEmpty()
            .WithMessage("Group title cannot be empty.");

        When(x => x.Group.Image != null, () =>
        {
            RuleFor(x => x.Group.Image).SetValidator(new ImageValidator(imageServiceSettings));
        });
    }
}

public class CreateGroupConversationCommandHandler : IRequestHandler<CreateGroupConversationCommand, 
    ResultDto<ConversationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageClient _imageClient;
    private readonly IAuthHeaderService _authHeaderService;

    public CreateGroupConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, 
        IImageClient imageClient, IAuthHeaderService authHeaderService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _authHeaderService = authHeaderService;
        _imageClient = imageClient;
    }

    public async Task<ResultDto<ConversationDto>> Handle(CreateGroupConversationCommand request,
        CancellationToken cancellationToken)
    {
       
        Conversation conversation = await _unitOfWork.Conversations.CreateGroupConversationAsync(
            request.Group.Title,
            request.CreatorUserId);

        if (request.Group.Image != null)
        {
            var fileExtension = Path.GetExtension(request.Group.Image.FileName).ToLowerInvariant();

            string authToken = _authHeaderService.GetAuthToken();

            ResultDto<ImageResultDto> response = await _imageClient.AddConversationImageAsync(request.Group.Image, authToken, conversation.Id);

            if (!response.Success)
            {
                return ResultDto.FailureResult<ConversationDto>(response.HttpStatusCode, response.ErrorMessage);
            }

            conversation.Group.ImgUrl = response.Payload.RelativePath;
        }

        await _unitOfWork.SaveChangesAsync();

        var mappedConversation = _mapper.Map<ConversationDto>(conversation);

        return ResultDto.SuccessResult(mappedConversation, HttpStatusCode.OK);
    }
}
