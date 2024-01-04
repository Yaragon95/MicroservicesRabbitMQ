using MicroRabbitMQ.Banking.Data.Contexts;
using MicroRabbitMQ.Banking.Domain.Interfaces;
using MicroRabbitMQ.Banking.Domain.Models;

namespace MicroRabbitMQ.Banking.Data.Repository
{
    public class AccountRepository(BankingDbContext context) : IAccountRepository
    {
        private readonly BankingDbContext _context = context;

        public IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts;
        }
    }
}
