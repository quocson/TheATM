using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using TheATM.Service;
using TheATM.Models;
namespace TheATM.Controllers
{
    public class AccountController : ApiController
    {
        //
        // GET: /Account/

        private AccountService service = AccountService.Instance();

        public void Post(string accountName, int initBalance)
        {
            service.AddAccount(new Models.Account
            {
                Balance = initBalance,
                AccountName = accountName
            });
        }

        public void Put(string accountFrom, string accountTo, int amount)
        {
            service.Transfer(accountFrom, accountTo, amount);
        }

        public List<Account> Get()
        {
            return service.ListAll();
        }

    }
}
