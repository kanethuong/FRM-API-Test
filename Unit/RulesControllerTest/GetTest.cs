using System.Collections.Generic;
using System.Threading.Tasks;
using kroniiapi.DTO;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;
using Moq;
using System;
using System.IO;

namespace kroniiapiTest.Unit.RulesControllerTest
{
    public class GetTest : RulesControllerTest
    {
        public static IEnumerable<TestCaseData> GetTestCaseData
        {
            get
            {
                // True case: with valid cookie and refresh token in server
                yield return new TestCaseData(
                    null,
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("GetTestCaseData")]
        public async Task GetRuleTest(FileDTO fileData, int expcetedStatus)
        {
            // Arrange
            mockCacheProvider.Setup(cp => cp.GetFromCache<FileDTO>("RulesURL")).Returns(Task.FromResult(fileData));

            // Act
            var rs = await rulesController.Get();
            var objectRs = rs as ObjectResult;

            // Assert
            Assert.AreEqual(expcetedStatus, objectRs.StatusCode);
        }
    }
}