using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Enums;
using Messenger.Business.EventBus;

namespace Messenger.Business.Commands;

public class StartRealSpamCommand : IRequest<ResultDto>
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; }
    public double Lambda { get; set; }
    public int DurationSeconds { get; set; }
    public Guid SenderId { get; set; }
}

public class StartRealSpamCommandHandler : IRequestHandler<StartRealSpamCommand, ResultDto>
{
    private readonly IEventPublisher _eventPublisher;

    public StartRealSpamCommandHandler(IEventPublisher eventBus)
    {
        _eventPublisher = eventBus;
    }

    public async Task<ResultDto> Handle(StartRealSpamCommand request, CancellationToken cancellationToken)
    {
        var rnd = Random.Shared;

        double currentTime = 0.0;
        double endTime = request.DurationSeconds;

        var start = DateTime.UtcNow;

        var message = new MessageDto
        {
            Text = request.Text,
            IsJoinMessage = false
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
                    Mode = ExecutionMode.Real,
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

        return ResultDto.SuccessResult();
    }
}