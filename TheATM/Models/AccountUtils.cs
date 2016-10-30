using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheATM.Models
{
    public static class AccountUtils
    {        
        public static void Withdraws(this Account account, int amount)
        {
            if (account.Balance >= amount)
            {
                account.Balance -= amount;
            }
            else
            {
                throw new Exception("Insufficient Balance.");
            }

        }

        public static void Deposits(this Account account, int amount)
        {
            account.Balance += amount;
        }
    }
}