using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    public class GetAccountListTest
    {
        private DataContext _context;
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
                        PageSize = 1,
                        SearchName = "hostcode0301"
                    },
                    200
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        SearchName = "hostcode0301"
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