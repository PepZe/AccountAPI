using AccountAPI.Business.Handler;
using AccountAPI.Controllers;
using AccountAPI.Infrastructure;
using AccountAPI.Model;
using AccountAPI.Model.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
