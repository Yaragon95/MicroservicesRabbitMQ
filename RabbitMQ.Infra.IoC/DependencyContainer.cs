using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infa.Bus;
using MicroRabbitMQ.Banking.Application.Interfaces;
using MicroRabbitMQ.Banking.Application.Services;
using MicroRabbitMQ.Banking.Data.Contexts;
using MicroRabbitMQ.Banking.Data.Repository;
using MicroRabbitMQ.Banking.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain bus
            services.AddTransient<IEventBus, RabbitMQBus>();
            services.Configure<RabbitMQSettings>(settings => configuration.GetSection("RabbitMQSettings"));

            // Application Services
            services.AddTransient<IAccountServices, AccountServices>();

            // Account Repository
            services.AddTransient<IAccountRepository, AccountRepository>();

            // Context
            services.AddTransient<BankingDbContext>();
        }
    }
}
