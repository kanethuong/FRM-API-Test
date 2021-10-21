using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.TokenControllerTest
{
    [TestFixture]
    public class RefreshAccessTokenTest
    {
        private readonly IRefreshToken _refreshToken;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private TokenController tokenController;
        public static readonly Mock<IRefreshToken> mockRefreshToken = new Mock<IRefreshToken>();
        public static readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        public static readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        public static readonly Mock<IJwtGenerator> mockJwtGenerator = new Mock<IJwtGenerator>();
        private static readonly AuthenticationController authController = new AuthenticationController(mockAccountService.Object, mockMapper.Object, mockJwtGenerator.Object, mockRefreshToken.Object);


        public static IEnumerable<TestCaseData> RefreshTestCaseTrue
        {
            get
            {
                // True case: with valid cookie and refresh token in server
                yield return new TestCaseData(
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        email = "hostcode0301@gmail.com"
                    },
                    200
                );
                // Fail case: with no cookie and refresh token in server
                yield return new TestCaseData(
                    new
                    {
                        // localCookie = "",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        email = "hostcode0301@gmail.com"
                    },
                    400
                );
                // Fail case: with valid cookie but no refresh token in server
                yield return new TestCaseData(
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        // serverCookie = "",
                        email = "hostcode0301@gmail.com"
                    },
                    400
                );
            }
        }

        [Test]
        [TestCaseSource("RefreshTestCaseTrue")]
        public async Task RefreshAccessTokenTest_True(dynamic cookieOptions, int expectedStatus)
        {

            string localCookie, serverCookie, serverMail;
            try { localCookie = (string)cookieOptions.localCookie; } catch { localCookie = null; }
            try { serverCookie = (string)cookieOptions.serverCookie; } catch { serverCookie = null; }
            try { serverMail = (string)cookieOptions.email; } catch { serverMail = null; }
            // Mock cookie
            var cookie = new StringValues((localCookie == null) ? "" : "X-Refresh-Token" + "=" + localCookie);
            authController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            authController.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);
            // Mock IRefreshToken helper
            mockRefreshToken.Setup(rt => rt.GetEmailByToken(serverCookie)).Returns(serverMail);

            if (serverCookie == null)
            {
                mockRefreshToken.Setup(rt => rt.RemoveTokenByEmail(serverMail)).Throws(new Exception());
            }


            tokenController = new TokenController(_refreshToken, _jwtGenerator, _accountService, _mapper);

            var rs = await tokenController.RefreshAccessToken();
            var obResult = rs.Result as ObjectResult;

            Assert.AreEqual(200, obResult);

        }
    }
}