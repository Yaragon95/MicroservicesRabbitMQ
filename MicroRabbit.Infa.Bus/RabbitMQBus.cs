using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroRabbit.Infa.Bus
{
    public sealed class RabbitMQBus(IMediator mediator, IOptions<RabbitMQSettings> rabbitSettings) : IEventBus
    {
        private readonly IMediator _mediator = mediator;
        private readonly RabbitMQSettings _rabbitmqSettings = rabbitSettings.Value;
        private readonly Dictionary<string, List<Type>> _handler = [];
        private readonly List<Type> _eventTypes = [];

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitmqSettings.Username,
                Password = _rabbitmqSettings.Password,
                HostName = _rabbitmqSettings.Hostname,
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var eventName = @event.GetType().Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var message = JsonConvert.SerializeObject(@event);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: eventName, basicProperties: null, body: body);
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T))) _eventTypes.Add(typeof(T));

            if (!_handler.ContainsKey(eventName)) _handler.Add(eventName, []);

            if (_handler[eventName].Any(s => s.GetType() == handlerType))
                throw new ArgumentException($"El handler exception {handlerType.Name} ya fue registrado anteriormente por '{eventName}'", nameof(handlerType));

            _handler[eventName].Add(handlerType);

            StartBasicConsume<T>();
        }

        #region Methods private
        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitmqSettings.Username,
                Password = _rabbitmqSettings.Password,
                HostName = _rabbitmqSettings.Hostname,
                DispatchConsumersAsync = true
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += Consumer_Received;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProccessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task ProccessEvent(string eventName, string message)
        {
            if (_handler.ContainsKey(eventName))
            {
                var subscriptions = _handler[eventName];
                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);
                    if (handler is null) continue;

                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);

                    var @event = JsonConvert.DeserializeObject(message, eventType);

                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { @event })!;
                }
            }
        }
        #endregion
    }
}
