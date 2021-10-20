using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.Controller.AccountControllerTest
{
    public class DeactivateAccountTest
    {
        private DataContext _context;
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        public static readonly Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();

        [Test]
        public async Task DeactivateAccount_ReturnActionResult_Return200()
        {
            //Calling Controller using 2 mock Object
            AccountController controller 
                = new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);

            // Setup Services return using Mock
            mockAccountService.Setup(x => x.DeactivateAccount(1,"Trainee")).ReturnsAsync(1);

            // Get Controller return result
            var actual = await controller.DeactivateAccount(1,"Trainee");
            var okResult = actual as ObjectResult;
            
            // Assert result with expected result: this time is 200
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DeactivateAccount_ReturnActionResult_Return404()
        {
            //Calling Controller using 2 mock Object
            AccountController controller 
                = new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);

            // Setup Services return using Mock
            mockAccountService.Setup(x => x.DeactivateAccount(1,"Trainee")).ReturnsAsync(0);

            // Get Controller return result
            var actual = await controller.DeactivateAccount(1,"Trainee");
            var okResult = actual as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(500, okResult.StatusCode);
        }
    }
}