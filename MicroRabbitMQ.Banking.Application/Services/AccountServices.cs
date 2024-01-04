using MicroRabbitMQ.Banking.Application.Interfaces;
using MicroRabbitMQ.Banking.Domain.Interfaces;
using MicroRabbitMQ.Banking.Domain.Models;

namespace MicroRabbitMQ.Banking.Application.Services
{
    public class AccountServices(IAccountRepository accountRepository) : IAccountServices
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        public IEnumerable<Account> GetAccounts()
        {
            return _accountRepository.GetAccounts();
        }
    }
}
