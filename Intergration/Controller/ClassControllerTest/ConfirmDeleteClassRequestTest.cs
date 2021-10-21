using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.ClassControllerTest
{
    public class ConfirmDeleteClassRequestTest
    {
        private Mock<IClassService> mockEmailService = new Mock<IClassService>();
        private DataContext dataContext;
        private IMapper mapper;
        private ClassService classService;
        private ClassController classController;
        private ITraineeService traineeService;


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
        private readonly Admin admin = new Admin(){
            AdminId = 1,
            Username = "HoangNN",
            Password = "12345",
            Fullname = "Nguyen Nhat Hoang",
            AvatarURL = null,
            Email = "hoangnnce150226@fpt.edu.vn",
            Phone = "0928140471",
            DOB = new DateTime(2021,1,1),
            Address = null,
            Gender = null,
            Wage = 0,
            IsDeactivated = false,
            DeactivatedAt = null,
            RoleId = 2
        };
        private readonly Trainer trainer = new Trainer() {
            TrainerId = 1,
            Username = "DatLT",
            Password = "12345",
            Fullname = "Le Thanh Dat",
            AvatarURL = null,
            Email = "datltce150226@fpt.edu.vn",
            Phone = "0928140472",
            DOB = new DateTime(2021,1,1),
            Address = null,
            Gender = null,
            Wage = 0,
            IsDeactivated = false,
            DeactivatedAt = null,
            RoleId = 3
        };
        private readonly List<Class> classes = new List<Class>()
        {
            new Class()
            {
                ClassId = 1,
                ClassName = "Phuttt",
                Description = "Tran Thien Phu",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                AdminId = 1,
                TrainerId = 1,
                RoomId = 1,
            },
            new Class()
            {
                ClassId = 2,
                ClassName = "SE1501",
                Description = "Programming",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                AdminId = 1,
                TrainerId = 1,
                RoomId = 1,
            }
        };
        private readonly Room room = new Room
        {
            RoomId = 1,
            RoomName = "se1501"
        };

        private readonly ConfirmDeleteClassInput confirmDeleteClassInput_Success = new ConfirmDeleteClassInput()
        {
            ClassId = 2,
            DeleteClassRequestId = 1,
            IsDeactivate = true
        };

        private readonly ConfirmDeleteClassInput confirmDeleteClassInput_NotFound = new ConfirmDeleteClassInput()
        {
            ClassId = 1,
            DeleteClassRequestId = 3,
            IsDeactivate = true
        };

        private readonly ConfirmDeleteClassInput confirmDeleteClassInput_Conflict = new ConfirmDeleteClassInput()
        {
            ClassId = 1,
            DeleteClassRequestId = 4,
            IsDeactivate = true
        };

        private readonly DeleteClassRequest deleteClassRequest_Success = new DeleteClassRequest(){
            DeleteClassRequestId = 1,
            Reason = "phong hu",
            ClassId = 2,
            IsAccepted = false,
            AdminId = 1
        };
        private readonly DeleteClassRequest deleteClassRequest_NotFound = new DeleteClassRequest(){
            DeleteClassRequestId = 2,
            Reason = "phong hu",
            ClassId = 1,
            IsAccepted = false,
            AdminId = 1
        };

        private readonly DeleteClassRequest deleteClassRequest_Conflict = new DeleteClassRequest(){
            DeleteClassRequestId = 4,
            Reason = "phong hu",
            ClassId = 1,
            IsAccepted = true,
            AdminId = 1
        };

        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Admins.AddRange(admin);
            dataContext.Trainers.AddRange(trainer);
            dataContext.Rooms.AddRange(room);
            dataContext.Classes.AddRange(classes);
            dataContext.SaveChanges();
            classService = new ClassService(dataContext,mapper,traineeService);
            classController = new ClassController(classService, null, null, null, null, null, null, null);

        }

        [OneTimeTearDown]
        public void TearDown(){
            dataContext.DeleteClassRequests.RemoveRange(dataContext.DeleteClassRequests);
            dataContext.Classes.RemoveRange(dataContext.Classes);
            dataContext.Rooms.RemoveRange(dataContext.Rooms);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.SaveChanges();
        }
        [Test]
        public async Task ConfirmDeleteClassReq_Success(){
                dataContext.DeleteClassRequests.Add(deleteClassRequest_Success);
                dataContext.SaveChanges();
                var result = await classController.ConfirmDeleteClassRequest(confirmDeleteClassInput_Success) as ObjectResult;
                var rs = result.Value as ResponseDTO;   
                Assert.True(
                result.StatusCode == 200 && rs.Status == 200, "Wrong status code"
            );
        }

        [Test]
        public async Task ConfirmDeleteClassReq_NotFound(){
                dataContext.DeleteClassRequests.Add(deleteClassRequest_NotFound);
                dataContext.SaveChanges();
                var result = await classController.ConfirmDeleteClassRequest(confirmDeleteClassInput_NotFound) as ObjectResult;
                var rs = result.Value as ResponseDTO;
                Assert.True(
                result.StatusCode == 404 && rs.Status == 404, "Wrong status code"
            );
        }

        [Test]
        public async Task ConfirmDeleteClassReq_Conflict(){
                dataContext.DeleteClassRequests.Add(deleteClassRequest_Conflict);
                dataContext.SaveChanges();
                var result = await classController.ConfirmDeleteClassRequest(confirmDeleteClassInput_Conflict) as ObjectResult;
                var rs = result.Value as ResponseDTO;
                Assert.True(
                result.StatusCode == 409 && rs.Status == 409, "Wrong status code"
            );
        }

    }
}