using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProcessoSeletivo.Model;
using ProcessoSeletivo.Model.Enum;
using ProcessoSeletivo.Model.Interface;
using ProcessoSeletivo.Model.Operator;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProcessoSeletivo.Controllers
{
    [Route("")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static IDao<Account> _dao;

        public AccountController(IDao<Account> dao)
        {
            if (_dao != null)
            {
                return;
            }
            _dao = dao;
        }

        [Route("Balance")]
        [HttpGet]
        public ActionResult<double> GetBalance(int account_id)
        {
            var account = _dao.Search(account_id);

            if (account != null)
            {
                return account.Balance;
            }

            return NotFound(0);
        }


        [Route("Reset")]
        [HttpPost]
        public IActionResult PostReset()
        {
            return Ok();
        }

        [Route("Event")]
        [HttpPost]
        public IActionResult PostAccountEvent([FromBody] AccountOperator account)
        {
            return EventsHandler(account);
        }

        public IActionResult EventsHandler(AccountOperator account)
        {
            var operation = Enum.Parse(typeof(TypesOperation), account.Type);
            Account destination, origin;
            try
            {
                switch (operation)
                {
                    case TypesOperation.withdraw:
                        origin = _dao.Search(int.Parse(account.Origin));
                        origin.Withdraw(account.Amount);

                        return CreatedAtAction(nameof(PostAccountEvent), new { origin });

                    case TypesOperation.deposit:
                        destination = _dao.Search(int.Parse(account.Destination));
                        if (destination == null)
                        {
                            destination = new Account(int.Parse(account.Destination), 0);
                            _dao.Include(destination);
                        }

                        destination.Deposit(account.Amount);
                        return CreatedAtAction(nameof(PostAccountEvent), new { destination });

                    case TypesOperation.transfer:
                        origin = _dao.Search(int.Parse(account.Origin));
                        destination = _dao.Search(int.Parse(account.Destination));

                        origin.Transfer(destination, account.Amount);
                        return CreatedAtAction(nameof(PostAccountEvent), new { origin, destination });

                    default:
                        return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound(0);
            }
        }
    }
}
