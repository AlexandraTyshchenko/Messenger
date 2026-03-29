using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Queues;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.VisualBasic;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Messenger.Business.Commands;

public class StartSpamCommand : IRequest<ResultDto>
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; }
    public double Lambda { get; set; }
    public int DurationSeconds { get; set; }
    public Guid SenderId { get; set; }
}

public class StartSpamCommandHandler : IRequestHandler<StartSpamCommand, ResultDto>
{
    private readonly MessageQueue _queue;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StartSpamCommandHandler(MessageQueue queue, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _queue = queue;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto> Handle(StartSpamCommand request, CancellationToken cancellationToken)
    {
        User sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);

        if (sender == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                "No sender was found.");
        }

        Conversation conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                "No conversation was found.");
        }

        var user = _mapper.Map<UserBasicInfoDto>(sender);

        await _unitOfWork.SaveChangesAsync();

        var message = new Message()
        {
            Conversation = conversation,
            IsJoinMessage = false,
            Sender = sender,
            Text = request.Text,
            SentAt = DateTime.Now,
        };

        var count = await Task.Run(async () =>
        {
            int messageSentCount = 0;

            var rnd = Random.Shared;
            var end = DateTime.UtcNow.AddSeconds(request.DurationSeconds);

            while (DateTime.UtcNow < end)
            {
                var delay = -Math.Log(1 - rnd.NextDouble()) / request.Lambda;

                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                await _queue.EnqueueAsync(new ChatNotification
                {
                    ConversationId = request.ConversationId,
                    Message = new MessageWithSenderDto
                    {
                        ConversationId = request.ConversationId,
                        IsJoinMessage = false,
                        Sender = user,
                        Text = request.Text,
                        SentAt = DateTime.UtcNow,
                    },
                    ArrivalTime = DateTime.UtcNow
                });

                messageSentCount++;
            }

            return messageSentCount;
        }, cancellationToken);

        var tasks = Enumerable.Range(0, count)
            .Select(_ => _unitOfWork.Messages.AddMessageToConversationAsync(message));

        await Task.WhenAll(tasks);
        return ResultDto.SuccessResult();
    }

}