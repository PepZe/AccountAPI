using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AccountAPI.Business.Handler;
using AccountAPI.Model;
using AccountAPI.Model.Interface;
using AccountAPI.Model.Operator;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static EndpointHandler endpointHandler;
        public AccountController(IDao<Account> dao)
        {
            if (endpointHandler != null)
            {
                return;
            }
            endpointHandler = new EndpointHandler(dao);
        }

        [Route("Balance")]
        [HttpGet]
        public ActionResult<double> GetBalance(int account_id)
        {
            var account = endpointHandler.GetAccount(account_id);

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
            endpointHandler = null;
            return Ok();
        }

        [Route("Event")]
        [HttpPost]
        public IActionResult PostAccountEvent([FromBody] AccountOperator account)
        {
            try
            {
                return Created(nameof(PostAccountEvent), endpointHandler.EventsHandler(account));
            }
            catch (Exception)
            {
                return NotFound(0);
            }
        }
    }
}
