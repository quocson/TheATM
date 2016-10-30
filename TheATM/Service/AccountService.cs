using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using TheATM.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace TheATM.Service
{
    public class AccountService
    {

        private const int TIMEOUT = 10000;

        private static AccountService service = new AccountService();
        private AccEntities dbContext;

        private ConcurrentDictionary<int, object> locks = new ConcurrentDictionary<int, object>();
        private AccountService()
        {
            this.dbContext = new AccEntities();

        }
        public static AccountService Instance()
        {
            return service;
        }

        public List<Account> ListAll()
        {
            return this.dbContext.Accounts.ToList();
        }

        public void AddAccount(Account account)
        {
            var accountFrom = this.dbContext.Accounts.FirstOrDefault(a => a.AccountName.Equals(account.AccountName));
            if (accountFrom != null)
            {
                throw new Exception("Account Name is already registered.");
            }
            this.dbContext.Accounts.Add(account);
            this.dbContext.SaveChanges();
        }

        public void Transfer(string accountNameFrom, string accountNameTo, int amount)
        {
            var accountFrom = this.dbContext.Accounts.FirstOrDefault(a => a.AccountName.Equals(accountNameFrom));
            var accountTo = this.dbContext.Accounts.FirstOrDefault(a => a.AccountName.Equals(accountNameTo));
            if (accountFrom == null)
            {
                throw new Exception("Source Account Not Found.");
            }
            if (accountTo== null)
            {
                throw new Exception("Destination Account Not Found.");
            }

            object lock1 = GetLockById(accountFrom.Id);
            object lock2 = GetLockById(accountTo.Id);
            if (accountFrom.Id > accountTo.Id)
            {
                var temp = lock2;
                lock2 = lock1;
                lock1 = temp;
            }
            try
            {
                Monitor.TryEnter(lock1, TIMEOUT);
                Thread.Sleep(TIMEOUT / 2);
                Monitor.TryEnter(lock2, TIMEOUT);
                accountFrom.Withdraws(amount);
                accountTo.Deposits(amount);
                this.dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Monitor.Exit(lock2);
                Monitor.Exit(lock1);
            }
        }

        

        private object GetLockById(int id)
        {
            if(!locks.ContainsKey(id))
            {      
                locks.TryAdd(id, new object());
            }
            return locks[id];
        }
    }
}