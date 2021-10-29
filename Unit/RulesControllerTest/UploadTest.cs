using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.Controllers;
using kroniiapi.DTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.RulesControllerTest
{
    public class UploadTest : RulesControllerTest
    {
        public static IEnumerable<TestCaseData> UploadTestCase
        {
            get
            {
                // True case: with valid cookie and refresh token in server
                yield return new TestCaseData(
                    "\\FileForTest\\ERR_Diagram.pdf",
                    new FileDTO
                    {
                        Name = "ERR_Diagram.pdf",
                        ContentType = "application/pdf",
                        Url = "URL"
                    },
                    201
                );
                yield return new TestCaseData(
                    "\\FileForTest\\Avatar.png",
                    new FileDTO
                    {
                        Name = "Avatar.png",
                        ContentType = "image/png",
                        Url = "URL"
                    },
                    400
                );
            }
        }

        [Test]
        [TestCaseSource("UploadTestCase")]
        public async Task UploadRuleTest(string pathTest, FileDTO fileData, int expcetedStatus)
        {
            // Arrange

            // Mock called functions
            mockUploadHelper.Setup(uh => uh.Upload(It.IsAny<Stream>(), fileData.Name, "Rules")).ReturnsAsync(fileData.Url);
            mockCacheProvider.Setup(cp => cp.SetCache<FileDTO>("RulesURL", fileData)).Returns(Task.CompletedTask);

            // Mock file input
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var fileStream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(fileStream, 0, fileStream.Length, "file", fileData.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = fileData.ContentType // Set cái củ lòn ContentType
            };

            // Act 
            var rs = await rulesController.Upload(file);
            var objectRs = rs as ObjectResult;

            // Assert
            Assert.AreEqual(expcetedStatus, objectRs.StatusCode);
        }
    }
}