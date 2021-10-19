using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace kroniiapitest.Intergration.Controller.AccountControllerTest
{
    public class AccountControllerTestCaseData
    {
        protected DataContext _context;
        protected IAccountService _accountService;
        protected IMapper _mapper;
        protected IAdminService _adminService;
        protected IAdministratorService _administratorService;
        protected ITrainerService _trainerService;
        protected ITraineeService _traineeService;
        protected ICompanyService _comapyService;
        protected IEmailService _emailService;
        protected AccountController controller;
        List<Administrator> administratorsList = new List<Administrator>
        {
            new Administrator
            {
                AdministratorId = 1,
                Username = "oneaboveall",
                Password = "iamironman",
                Fullname = "Tony Dead",
                AvatarURL = "none",
                Email = "iamironman@gmail.com",
                Phone = "none",
                RoleId = 1,
            }
        };
        List<Admin> adminsList = new List<Admin>
        {
            new Admin{
                AdminId = 1,
                Username = "fptunisersity",
                Password = "fptunisersity",
                Fullname = "DaiHocFPTCanTho",
                AvatarURL = "none",
                Email = "fptu.cantho@fe.edu.vn",
                Phone = "02927303636",
                DOB = new DateTime(2000, 1 , 1),
                Address = "600, An Binh, Can Tho",
                Gender = "Male",
                Wage = 10000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            },
            new Admin {
                AdminId = 2,
                Username = "thuytienkhongsaoke",
                Password = "123456",
                Fullname = "TranThuyTien",
                AvatarURL = "none",
                Email = "thuytienkosaoke@gmail.vn",
                Phone = "0786342221",
                DOB = new DateTime(2000, 4 , 7),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Female",
                Wage = 20000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            },
            new Admin {
                AdminId = 3,
                Username = "congvinhkosaoke",
                Password = "654321",
                Fullname = "LeCongVinh",
                AvatarURL = "none",
                Email = "congvinhkhongsaoke@gmail.vn",
                Phone = "029288290",
                DOB = new DateTime(2000, 2 , 1),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Male",
                Wage = 10000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            }
        };

        List<Trainer> trainersList = new List<Trainer>
        {
            new Trainer {
                TrainerId = 1,
                Username = "luonghoanghuong",
                Password = "huongdeptrai",
                Fullname = "Luong Hoang Huong",
                AvatarURL = "none",
                Email = "huongdeptrai@gmail.com",
                Phone = "059228190",
                DOB = new DateTime(1990, 1 , 1),
                Address = "600, An Binh, Can Tho",
                Gender = "Male",
                Wage = 30000,
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 3,
            },
            new Trainer {
                TrainerId = 2,
                Username = "vohongkhanh",
                Password = "khanhdeptrai",
                Fullname = "Vo Hong Khanh",
                AvatarURL = "none",
                Email = "khanhdeptrai@gmail.com",
                Phone = "059548190",
                DOB = new DateTime(1990, 1 , 1),
                Address = "600, An Binh, Can Tho",
                Gender = "Male",
                Wage = 30000,
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 3,
            }
        };
        List<Trainee> traineesList = new List<Trainee>
        {
            new Trainee {
                TraineeId = 1,
                Username = "thinhdeptrai",
                Password = "thinhdeptraivcl",
                Fullname = "Nguyen Phuc Thinh",
                AvatarURL = "none",
                Email = "thinhdeptrai@gmail.com",
                Phone = "0766555444",
                DOB = new DateTime(2001, 1 , 1),
                Address = "Soc Trang",
                Gender = "Male",
                TuitionFee = 10000,
                Wage = 5000,
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 4,
                ClassId = 1,
            },
            new Trainee {
                TraineeId = 2,
                Username = "number2",
                Password = "number2",
                Fullname = "Thang Number 2",
                AvatarURL = "none",
                Email = "number2@gmail.com",
                Phone = "072222222",
                DOB = new DateTime(2001, 1 , 1),
                Address = "Bac Lieu",
                Gender = "Male",
                TuitionFee = 10000,
                Wage = 5000,
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 4,
                ClassId = 1,
            },
            new Trainee {
                TraineeId = 3,
                Username = "thanglopkhac",
                Password = "thanglopkhac",
                Fullname = "Thang Lop Khac",
                AvatarURL = "none",
                Email = "thanglopkhac@gmail.com",
                Phone = "0766555444",
                DOB = new DateTime(2001, 1 , 1),
                Address = "Can Tho",
                Gender = "Male",
                TuitionFee = 10000,
                Wage = 5000,
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 4,
                ClassId = 2,
            }
        };
        List<Company> companiesList = new List<Company>
        {
            new Company
            {
                CompanyId = 1,
                Username = "fptsoft",
                Password = "fptsuck",
                Fullname = "FPT Software",
                AvatarURL = null,
                Email = "fptsoft@gmail.com",
                Phone = "0766555444",
                Address = "Can Tho",
                Gender = "Male",
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 5,
            },
            new Company
            {
                CompanyId = 2,
                Username = "jypentertainment",
                Password = "jypissuck",
                Fullname = "JYP Entertainment",
                AvatarURL = null,
                Email = "jypenter@gmail.com",
                Phone = "0766555444",
                Address = "Korea",
                Gender = "Male",
                CreatedAt = new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                DeactivatedAt = null,
                RoleId = 5,
            }
        };

        [SetUp]
        public void Setup()
        {
            //Create option for Fake DB
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;
            //Add Fake DB to context, service
            _context = new DataContext(option);
            _accountService = new AccountService(_context, _mapper, _adminService,_administratorService,_comapyService, _traineeService, _trainerService, _emailService);
            controller = new AccountController(_accountService, _mapper, _emailService);
            //Add fake data to Context
            _context.Administrators.AddRange(administratorsList);
            _context.Admins.AddRange(adminsList);
            _context.Trainers.AddRange(trainersList);
            _context.Trainees.AddRange(traineesList);
            _context.Companies.AddRange(companiesList);
            _context.SaveChanges();
        }
        [TearDown]
        public void TearDown()
        {
            _context.Administrators.RemoveRange(_context.Administrators);
            _context.Admins.RemoveRange(_context.Admins);
            _context.Trainers.RemoveRange(_context.Trainers);
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.Companies.AddRange(_context.Companies);
            _context.SaveChanges();
        }
    }
}