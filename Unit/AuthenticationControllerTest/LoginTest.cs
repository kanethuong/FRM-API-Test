using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace kroniiapiTest.Unit.AuthenticationControllerTest
{
    public class LoginTest
    {
        public static readonly Mock<IRefreshToken> mockRefreshToken = new Mock<IRefreshToken>();
        public static readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        public static readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        public static readonly Mock<IJwtGenerator> mockJwtGenerator = new Mock<IJwtGenerator>();
        private readonly AuthenticationController controller = new AuthenticationController(mockAccountService.Object, mockMapper.Object, mockJwtGenerator.Object, mockRefreshToken.Object);
        public static IEnumerable<TestCaseData> LoginSuccess
        {
            get
            {
                //True case
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "danhlptce150056@fpt.edu.vn",
                        Password = "admin"
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$v.Hdp7QJKFozspX2LS1kmOzRsdkCdnAO3vYtod32eqhiwrunNPWiu",
                    200

                );
                //True case       
                // yield return new TestCaseData(
                //     new LoginInput
                //     {
                //         Email = "lephamthanhdanh@gmail.com",
                //         Password = "abcxyz123@"
                //     },
                //     new {
                //         localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                //         serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                //         },
                //     "$2a$12$1CTsXUySliIfM3vYf9m0W.ZQJarX11YKcntw5hjNpn.O1x5DcEDCi",
                //     200

                // );                

            }
        }

        [Test]
        [TestCaseSource("LoginSuccess")]
        public async Task Login_Success(LoginInput logInput, dynamic cookieOptions, string passBcrypt, int stacode)
        {
            string localCookie, serverCookie, serverMail;
            try { localCookie = (string)cookieOptions.localCookie; } catch { localCookie = null; }
            try { serverCookie = (string)cookieOptions.serverCookie; } catch { serverCookie = null; }
            try { serverMail = (string)cookieOptions.email; } catch { serverMail = null; }

            var cookie = new StringValues((localCookie == null) ? "" : "X-Refresh-Token" + "=" + localCookie);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);



            //Calling Controller using 2 mock Object

            // Setup Services return using Mock
            mockAccountService.Setup(x => x.GetAccountByEmail(logInput.Email)).ReturnsAsync(Tuple.Create(accTrue, passBcrypt));
            mockJwtGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns("ThinhChoDien");
            mockRefreshToken.Setup(x => x.CreateRefreshToken(accTrue.Email)).Returns("ThinhDeoDepTrai");
            // Mock IRefreshToken helper
            mockRefreshToken.Setup(rt => rt.GetEmailByToken(serverCookie)).Returns(serverMail);
            mockMapper.Setup(m => m.Map<AuthResponse>(accTrue)).Returns(authTrue);
            if (serverCookie == null)
            {
                mockRefreshToken.Setup(rt => rt.RemoveTokenByEmail(serverMail)).Throws(new Exception());
            }
            // Get Controller return result
            var actual = await controller.Login(logInput);
            var okResult = actual.Result as ObjectResult;

            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        AccountResponse accTrue = new AccountResponse
        {
            AccountId = 1,
            Username = "danhlpt",
            Fullname = "LePhamThanhDanh",
            Email = "danhlptce150056@fpt.edu.vn",
            Role = "Admin"
        };

        AuthResponse authTrue = new AuthResponse
        {
            AccountId = 1,
            Username = "danhlpt",
            Fullname = "LePhamThanhDanh",
            Email = "danhlptce150056@fpt.edu.vn",
            AvatarURL = "cocaicc",
            Role = "Admin",
            AccessToken = "ThinhChoDien"
        };

        public static IEnumerable<TestCaseData> LoginFail
        {
            get
            {
                // Fail case: wrong email format
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "danhlptce150056",
                        Password = "admin"
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$v.Hdp7QJKFozspX2LS1kmOzRsdkCdnAO3vYtod32eqhiwrunNPWiu",
                    404

                );
                //Fail case: Null login input
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "",
                        Password = ""
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$1CTsXUySliIfM3vYf9m0W.ZQJarX11YKcntw5hjNpn.O1x5DcEDCi",
                    404

                );
                // Fail case: No cookie            
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "danhlpt@gmail.com",
                        Password = "admin"
                    },
                    new
                    {
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$1CTsXUySliIfM3vYf9m0W.ZQJarX11YKcntw5hjNpn.O1x5DcEDCi",
                    404
                );
                //Fail case: Wrong password bcrypted 
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "danhlpt@gmail.com",
                        Password = "admin"
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "bcryptSai",
                    404

                );
            }
        }
        [Test]
        [TestCaseSource("LoginFail")]
        public async Task Login_Fail(LoginInput logInput, dynamic cookieOptions, string passBcrypt, int stacode)
        {
            string localCookie, serverCookie, serverMail;
            try { localCookie = (string)cookieOptions.localCookie; } catch { localCookie = null; }
            try { serverCookie = (string)cookieOptions.serverCookie; } catch { serverCookie = null; }
            try { serverMail = (string)cookieOptions.email; } catch { serverMail = null; }

            var cookie = new StringValues((localCookie == null) ? "" : "X-Refresh-Token" + "=" + localCookie);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);



            mockAccountService.Setup(x => x.GetAccountByEmail(logInput.Email)).ReturnsAsync(Tuple.Create(accTrue, passBcrypt));
            mockJwtGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns("ThinhChoDien");
            mockRefreshToken.Setup(x => x.CreateRefreshToken(accTrue.Email)).Returns("ThinhDeoDepTrai");
            // Mock IRefreshToken helper
            mockRefreshToken.Setup(rt => rt.GetEmailByToken(serverCookie)).Returns(serverMail);
            mockMapper.Setup(m => m.Map<AuthResponse>(accTrue)).Returns(authTrue);
            if (serverCookie == null)
            {
                mockRefreshToken.Setup(rt => rt.RemoveTokenByEmail(serverMail)).Throws(new Exception());
            }
            // Get Controller return result
            var actual = await controller.Login(logInput);
            var okResult = actual.Result as ObjectResult;

            Assert.AreEqual(stacode, okResult.StatusCode);
        }

    }
}