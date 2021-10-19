using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.Services;
using kroniiapitest.Intergration.Controller.AccountControllerTest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    [TestFixture]
    public class CreateNewAccountByExcelTest : AccountControllerTestCaseData
    {
        [Test]
        public async Task CreateNewAccountByExcelTestTrue()
        {
            // Arrange
            string path = "not yet dude";
            var stream = File.OpenRead(path);
            IFormFile file = new FormFile(stream, 0, stream.Length, "Book1", "Book1.xls");
            // Act
            var rs = await controller.CreateNewAccountByExcel(file);
            var obResult = rs.Result as ObjectResult;
            // Assert
            Assert.AreEqual(400, obResult.StatusCode);
        }


    }
}