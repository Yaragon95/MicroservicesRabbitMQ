namespace MicroRabbit.Domain.Core.Commands
{
    /// <summary>
    /// Referencia del mensaje que sera transportado
    /// </summary>
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    }
}
