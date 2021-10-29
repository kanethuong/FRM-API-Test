using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Services;
using Moq;

namespace kroniiapitest.Unit.FeedbackControllerTest
{
    public class SendTrainerFeedbackTest
    {
        private readonly Mock<ITrainerService> mockTrainerService = new Mock<ITrainerService>();
        private readonly Mock<IClassService> mockClassService = new Mock<IClassService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();

        private readonly Mock<IFeedbackService> mockFeedbackService = new Mock<IFeedbackService>();
        private readonly Mock<IAdminService> mockAdminService = new Mock<IAdminService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
    }
}