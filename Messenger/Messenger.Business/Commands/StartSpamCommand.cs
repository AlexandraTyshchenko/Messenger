using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Enums;
using Messenger.Business.EventBus;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

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
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public StartSpamCommandHandler(IEventPublisher eventBus, IUnitOfWork unitOfWork)
    {
        _eventPublisher = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto> Handle(StartSpamCommand request, CancellationToken cancellationToken)
    {
        var rnd = Random.Shared;

        double currentTime = 0.0;
        double endTime = request.DurationSeconds;

        var start = DateTime.UtcNow;

        var message = new MessageDto
        {
            IsJoinMessage = false,
            Text = request.Text,
        };

        var tasks = new List<Task>();

        while (currentTime < endTime)
        {
            var dt = -Math.Log(1 - rnd.NextDouble()) / request.Lambda;
            currentTime += dt;

            if (currentTime > endTime)
                break;

            var scheduledTime = start.AddSeconds(currentTime);

            var task = Task.Run(async () =>
            {
                var delay = scheduledTime - DateTime.UtcNow;

                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, cancellationToken);

                await _eventPublisher.PublishAsync(new EventMessage
                {
                    Lambda = request.Lambda,
                    Mode = ExecutionMode.Theoretical,
                    Payload = new MessageSentEvent
                    {
                        SenderId = request.SenderId,
                        ConversationId = request.ConversationId,
                        Message = message
                    }
                }, cancellationToken);
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        var sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);
        if (sender == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(
                HttpStatusCode.NotFound,
                "No sender was found.");
        }

        var conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);
        if (conversation == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(
                HttpStatusCode.NotFound,
                "No conversation was found.");
        }

        await SaveMessagesBatch(sender, conversation, request.Text, tasks.Count);
        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult();
    }

    private async Task SaveMessagesBatch(User sender, Conversation conversation, string text, int count)
    {
        var tasks = Enumerable.Range(0, count)
            .Select(_ => _unitOfWork.Messages.AddMessageToConversationAsync(new Message
            {
                Conversation = conversation,
                IsJoinMessage = false,
                Sender = sender,
                Text = text,
                SentAt = DateTime.UtcNow,
            }));

        await Task.WhenAll(tasks);
        await _unitOfWork.SaveChangesAsync();
    }
}