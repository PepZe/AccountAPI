using AccountAPI.Controllers;
using AccountAPI.Infrastructure;
using AccountAPI.Model;
using AccountAPI.Model.Interface;
using AccountAPI.Model.Operator;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AccountAPI.Tests
{
    [TestFixture]
    class AccountControllerTest
    {
        private AccountController _accountController;
        private Mock<IDao<Account>> mockDao;
        [SetUp]
        public void SetUp()
        {
            mockDao = new Mock<IDao<Account>>();
            _accountController = new AccountController(mockDao.Object);
        }

        [Test]
        public void GetBalance_ShouldReturnNotFound()
        {
            //Arrange
            var account_id = 0;

            //Act
            var action = _accountController.GetBalance(account_id);

            //Assert    
            var status = action.Result as ObjectResult;
            Assert.That(status.StatusCode, Is.EqualTo(404));
            Assert.That(action.Value, Is.EqualTo(0));
        }

        [Test]
        public void GetBalance_ShouldReturnBalance()
        {
            //Arrange
            var account = new Account(1, 300);
            mockDao.Setup(_ => _.Search(account.Id)).Returns(account);

            //Act
            var act = _accountController.GetBalance(account.Id);

            //Assert
            Assert.That(act.Value, Is.EqualTo(account.Balance));
        }

        [Test]
        public void PostReset_ShouldSetEndpointNull()
        {
            //Act
            var act = _accountController.PostReset() as ActionResult;

            //Assert
            var status = act as StatusCodeResult;
            Assert.That(status.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void PostAccountEvent_ShouldReturnNotFound()
        {
            //Act
            var act = _accountController.PostAccountEvent(It.IsAny<AccountOperator>()) as ActionResult;

            //Assert
            var status = act as ObjectResult;
            Assert.That(status.StatusCode, Is.EqualTo(404));
        }

        [Test]
        [TestCase("1", "2", "transfer", 100)]
        public void PostAccountEvent_ShouldReturnCreated(string origin, string destination, string type, int amount)
        {
            //Arrange
            var account = new Account(1, 300);
            var accountOperator = new AccountOperator()
            {
                Amount = amount,
                Origin = origin,
                Destination = destination,
                Type = type
            };
            mockDao.Setup(_ => _.Search(account.Id)).Returns(account);

            //Act
            var act = _accountController.PostAccountEvent(accountOperator) as ActionResult;

            //Assert
            var status = act as ObjectResult;

            Assert.That(status.StatusCode, Is.EqualTo(201));
        }
    }
}
