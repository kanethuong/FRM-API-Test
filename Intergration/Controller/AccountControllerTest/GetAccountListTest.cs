using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
<<<<<<< HEAD
using kroniiapi.DTO.AccountDTO;
=======
>>>>>>> a81745f7359e7de7701e286cf67ca745ae8a4b05
using kroniiapi.DTO.PaginationDTO;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    public class GetAccountListTest
    {
        private DataContext _context;
<<<<<<< HEAD
        Admin testAdmin = new Admin()
        {
            AdminId = 1,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "abc@gmail.com",
            RoleId = 2
        };
        Trainer testTrainer = new Trainer()
        {
            TrainerId = 1,
            Username = "duykhang",
            Fullname = "thuongduykhang",
            Email = "khang@gmail.com",
            RoleId = 3
        };
        Trainee testTrainee = new Trainee()
        {
            TraineeId = 1,
            Username = "anhtho",
            Fullname = "tieuanhtho",
            Email = "tho@gmail.com",
            RoleId = 4
        };
        Company testCompany = new Company()
        {
            CompanyId = 1,
            Username = "hailong",
            Fullname = "lehoanghailong",
            Email = "long@gmail.com",
            RoleId = 5
        };
        IEnumerable<AccountResponse> listAcc = new List<AccountResponse>
        {
            new AccountResponse
            {
                AccountId = 1,
                Username = "khanhtoan",
                Fullname = "trankhanhtoan",
                Email = "toan@gmail.com",
                Role = "admin"
            },

            new AccountResponse
            {
                AccountId = 1,
                Username = "duykhang",
                Fullname = "thuongduykhang",
                Email = "khang@gmail.com",
                Role = "Trainer"
            },
            new AccountResponse
            {
                AccountId = 1,
                Username = "anhtho",
                Fullname = "tieuanhtho",
                Email = "tho@gmail.com",
                Role = "Trainee"
            },
            new AccountResponse
            {
                AccountId = 1,
                Username = "hailong",
                Fullname = "lehoanghailong",
                Email = "long@gmail.com",
                Role = "Company"
            }
        };

        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

=======
        Admin testAdmin = new Admin(){
            AdminId =1,
            Username="khanhtoan",
            Fullname="trankhanhtoan",
            Email="abc@gmail.com",
            RoleId=2
        };
        Trainer testTrainer = new Trainer(){
            TrainerId =1,
            Username="duykhang",
            Fullname="thuongduykhang",
            Email="khang@gmail.com",
            RoleId=3
        };
        Trainee testTrainee = new Trainee(){
            TraineeId = 1,
            Username="anhtho",
            Fullname="tieuanhtho",
            Email="tho@gmail.com",
            RoleId=4
        };
        Company testCompany = new Company(){
            CompanyId =1,
            Username="hailong",
            Fullname="lehoanghailong",
            Email="long@gmail.com",
            RoleId=5
        };
        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

>>>>>>> a81745f7359e7de7701e286cf67ca745ae8a4b05
            _context = new DataContext(option);

            _context.Admins.AddRange(testAdmin);
            _context.Trainers.AddRange(testTrainer);
            _context.Trainees.AddRange(testTrainee);
            _context.Companies.AddRange(testCompany);
            _context.SaveChanges();
        }
        public static IEnumerable<TestCaseData> GetAccountListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
<<<<<<< HEAD
                        PageSize = 4,
                        SearchName = ""
=======
                        PageSize = 1,
                        SearchName = "hostcode0301"
>>>>>>> a81745f7359e7de7701e286cf67ca745ae8a4b05
                    },
                    200
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
<<<<<<< HEAD
                        
                        SearchName = "toan@gmail.com"
=======
                        SearchName = "hostcode0301"
>>>>>>> a81745f7359e7de7701e286cf67ca745ae8a4b05
                    },
                    200
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        SearchName = "fpt.com"
                    },
                    200
                );
            }
        }
        [Test]
        public void GetAccountListTestTrue()
        {
            Assert.Pass();
        }
    }
}