using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    [TestFixture]
    public class CreateNewAccountByExcelTest 
    {
        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private DataContext dataContext;
        private IMapper mapper;
        private AccountService accountService;
        private AccountController accountController;
        private readonly List<Role> roleList = new List<Role>() {
            new Role() {
                RoleId = 1,
                RoleName = "Administrator"
            },
            new Role() {
                RoleId = 2,
                RoleName = "Admin"
            },
            new Role() {
                RoleId = 3,
                RoleName = "Trainer"
            },
            new Role() {
                RoleId = 4,
                RoleName = "Trainee"
            },
            new Role() {
                RoleId = 5,
                RoleName = "Company"
            }
        };
        

        [OneTimeSetUp]
        public void setupFirst() {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config => {
                config.AddProfile(new AccountProfile());
            });
            mapper = config.CreateMapper();

            mockEmailService.Setup(email => email.SendEmailAsync(It.IsAny<EmailContent>())).Returns(Task.CompletedTask);
            
            accountService = new AccountService(
                dataContext, 
                mapper, 
                new AdminService(dataContext),
                new AdministratorService(dataContext),
                new CompanyService(dataContext),
                new TraineeService(dataContext),
                new TrainerService(dataContext),
                mockEmailService.Object
            );
            accountController = new AccountController(accountService, mapper, mockEmailService.Object);
        }

        [TearDown]
        public void tearDown() {
            dataContext.Administrators.RemoveRange(dataContext.Administrators);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.Companies.RemoveRange(dataContext.Companies);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.Roles.RemoveRange(dataContext.Roles);
            dataContext.SaveChanges();
        }
        
        [Test]
        public async Task CreateNewAccountByExcelTestTrue()
        {
            // Arrange
            string path = "D:\\CreateAccTestTrue.xlsx";
            var stream = File.OpenRead(path);
            IFormFile file = new FormFile(stream, 0, stream.Length, "CreateAccTestTrue", "CreateAccTestTrue.xls");
            // Act
            var rs = await accountController.CreateNewAccountByExcel(file);
            var obResult = rs.Result as ObjectResult;
            // Assert
            Assert.AreEqual(201, obResult.StatusCode);
        }


    }
}