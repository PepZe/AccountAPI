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

        [SetUp]
        public void SetUp()
        {
            _accountController = new AccountController(new DaoAccount());
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
            var mockDao = new Mock<IDao<Account>>();
            mockDao.Setup(_ => _.Search(account.Id)).Returns(account);

            // Set endpointHandler to null.
            _accountController.PostReset();
            _accountController = new AccountController(mockDao.Object);

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
    }
}
