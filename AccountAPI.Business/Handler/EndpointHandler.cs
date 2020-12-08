using AccountAPI.Model;
using AccountAPI.Model.Enum;
using AccountAPI.Model.Interface;
using AccountAPI.Model.Operator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountAPI.Business.Handler
{
    public class EndpointHandler
    {
        private static IDao<Account> _dao;

        public EndpointHandler(IDao<Account> dao)
        {
            _dao = dao;
        }

        public object EventsHandler(AccountOperator account)
        {
            var operation = Enum.Parse(typeof(TypesOperation), account.Type);
            try
            {
                Account destination, origin;
                switch (operation)
                {
                    case TypesOperation.withdraw:
                        origin = GetAccount(int.Parse(account.Origin));
                        Withdraw(origin, account.Amount);

                        return new { origin = new { id = account.Origin, balance = origin.Balance} };

                    case TypesOperation.deposit:
                        destination = GetAccount(int.Parse(account.Destination));
                        if (destination == null)
                        {
                            destination = CreateAccount(account);
                        }

                        Deposit(destination, account.Amount);

                        return new { destination = new { id = account.Destination, balance = destination.Balance} };

                    case TypesOperation.transfer:
                        origin = GetAccount(int.Parse(account.Origin));
                        destination = GetAccount(int.Parse(account.Destination));
                        if (destination == null)
                        {
                            destination = CreateAccount(account);
                        }

                        Transfer(origin, destination, account.Amount);
                        return new { origin = new { id = account.Origin, balance = origin.Balance },
                            destination = new { id = account.Destination, balance = destination.Balance } };

                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        private void Transfer(Account origin, Account destination, double amount)
        {
            Withdraw(origin, amount);
            Deposit(destination, amount);
        }


        private static Account CreateAccount(AccountOperator accountOperator)
        {
            var account = new Account(int.Parse(accountOperator.Destination), 0);
            _dao.Include(account);

            return account;
        }

        private void Deposit(Account destination, double amount)
        {
            destination.Deposit(amount);
        }


        private void Withdraw(Account origin, double amount)
        {
            origin.Withdraw(amount);
        }

        public Account GetAccount(int id)
        {
            return _dao.Search(id);
        }
    }
}
