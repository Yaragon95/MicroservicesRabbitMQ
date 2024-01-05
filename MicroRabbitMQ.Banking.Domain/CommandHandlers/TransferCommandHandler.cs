using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbitMQ.Banking.Domain.Commands;
using MicroRabbitMQ.Banking.Domain.Events;

namespace MicroRabbitMQ.Banking.Domain.CommandHandlers
{
    public class TransferCommandHandler(IEventBus eventBus) : IRequestHandler<CreateTransferCommand, bool>
    {
        private readonly IEventBus _eventBus = eventBus;

        public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            // logica para publicar el mensaje dentro del event bus rabbitmq
            _eventBus.Publish(new TransferCreateEvent(request.From, request.To, request.Amount));

            return Task.FromResult(true);
        }
    }
}
