using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class AddMessageToConversationCommand : IRequest<ResultDto<MessageWithSenderDto>>
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public MessageDto Message { get; set; }
}

public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageWithSenderDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddMessageToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResultDto<MessageWithSenderDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
    {
        User sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);

        Conversation conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto<MessageWithSenderDto>.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                "No conversation was found.");
        }

        Message message = await _unitOfWork.Messages
             .AddMessageToConversationAsync(request.Message.Text, conversation, sender);

        await _unitOfWork.SaveChangesAsync();

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

        return ResultDto<MessageWithSenderDto>.SuccessResult(mappedMessage, HttpStatusCode.Created);
    }
}

