using MicroRabbitMQ.Banking.Application.Interfaces;
using MicroRabbitMQ.Banking.Application.Models;
using MicroRabbitMQ.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbitMQ.Banking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingController(IAccountServices accountServices) : ControllerBase
    {
        private readonly IAccountServices _accountServices = accountServices;

        [HttpGet]
        public ActionResult<IEnumerable<Account>> Get()
        {
            return Ok(_accountServices.GetAccounts());
        }

        [HttpPost]
        public IActionResult Post([FromBody] AccountTransfer accountTransfer)
        {
            _accountServices.
            return Ok(accountTransfer);
        }
    }
}
