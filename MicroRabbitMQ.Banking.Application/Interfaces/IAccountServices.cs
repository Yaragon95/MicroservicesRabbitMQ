using MicroRabbitMQ.Banking.Domain.Models;

namespace MicroRabbitMQ.Banking.Application.Interfaces
{
    public interface IAccountServices
    {
        IEnumerable<Account> GetAccounts();
    }
}
