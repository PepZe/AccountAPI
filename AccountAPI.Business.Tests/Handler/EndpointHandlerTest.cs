using AccountAPI.Business.Handler;
using AccountAPI.Model;
using AccountAPI.Model.Interface;
using AccountAPI.Model.Operator;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountAPI.Business.Tests.Handler
{
    class EndpointHandlerTest
    {
        private EndpointHandler _endpointHandler;
        private Mock<IDao<Account>> _dao;

        [SetUp]
        public void SetUp()
        {
            _dao = new Mock<IDao<Account>>();
            _endpointHandler = new EndpointHandler(_dao.Object);
        }

        [Test]
        public void EventsHandlerWithdraw_ShouldFailed()
        {
            //Arrange
            var accountOperator = new AccountOperator()
            {
                Origin = "1",
                Type = "withdraw",
                Amount = 100
            };

            const int initialBalance = 50;
            var account = new Account(1, initialBalance);
            _dao.Setup(_ => _.Search(int.Parse(accountOperator.Origin))).Returns(account);

            //Act
            var obj = _endpointHandler.EventsHandler(accountOperator);

            //Assert
            var origin = obj.GetType().GetProperty("origin").GetValue(obj);
            var balance = (double)origin.GetType().GetProperty("balance").GetValue(origin);

            Assert.IsFalse(account.Balance == (initialBalance - accountOperator.Amount));
        }

        [Test]
        public void EventsHandlerWithdraw_ShouldSubtractAmount()
        {
            //Arrange
            var accountOperator = new AccountOperator()
            {
                Origin = "1",
                Type = "withdraw",
                Amount = 50
            };

            const int initialBalance = 200;
            var account = new Account(1, initialBalance);
            _dao.Setup(_ => _.Search(int.Parse(accountOperator.Origin))).Returns(account);

            //Act
            var obj = _endpointHandler.EventsHandler(accountOperator);

            //Assert
            var origin = obj.GetType().GetProperty("origin").GetValue(obj);
            var balance = (double)origin.GetType().GetProperty("balance").GetValue(origin);

            Assert.IsTrue(account.Balance == (initialBalance - accountOperator.Amount));
        }

        [Test]
        public void EventsHandlerDeposit_ShouldThrowException()
        {
            //Arrange
            var accountOperator = new AccountOperator()
            {
                // Deposit uses destination, not origin
                Origin = "1",
                Type = "deposit",
                Amount = 50
            };

            //Act
            Assert.That(() => _endpointHandler.EventsHandler(accountOperator), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EventsHandlerDeposit_ShouldIncreaseBalance()
        {
            //Arrange
            var accountOperator = new AccountOperator()
            {
                Destination = "1",
                Type = "deposit",
                Amount = 50
            };

            //Act
            var obj = _endpointHandler.EventsHandler(accountOperator);

            //Assert
            var destination = obj.GetType().GetProperty("destination").GetValue(obj);
            var balance = (double)destination.GetType().GetProperty("balance").GetValue(destination);

            Assert.That(accountOperator.Amount, Is.EqualTo(balance));
        }

        [Test]
        public void EventsHandlerTransfer_ShouldThrowException()
        {
            //Arrange
            var accountOperator = new AccountOperator()
            {
                Origin = "1",
                Destination = "2",
                Type = "transfer",
                Amount = 50
            };

            //Act
            Assert.That(() => _endpointHandler.EventsHandler(accountOperator), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EventsHandlerTransfer_ShouldSuccess()
        {
            //Arrange
            const int transferValue = 50;
            const int initialBalance = 100;

            var accountOperator = new AccountOperator()
            {
                Origin = "1",
                Destination = "2",
                Type = "transfer",
                Amount = transferValue
            };

            var account = new Account(1, initialBalance);
            _dao.Setup(_ => _.Search(account.Id)).Returns(account);

            //Act
            var obj = _endpointHandler.EventsHandler(accountOperator);

            //Assert
            var origin = obj.GetType().GetProperty("origin").GetValue(obj);
            var originBalance = (double)origin.GetType().GetProperty("balance").GetValue(origin);
            Assert.IsTrue(originBalance == initialBalance - transferValue);

            var destination = obj.GetType().GetProperty("destination").GetValue(obj);
            var destinationBalance = (double)origin.GetType().GetProperty("balance").GetValue(destination);
            Assert.IsTrue(destinationBalance == transferValue);
        }
    }
}
