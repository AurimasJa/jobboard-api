using jobboard.Auth;
using jobboard.Controllers;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using jobboard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace JobBoardTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
        [TestMethod]
        public async Task Index_ReturnsViewWithModel()
        {
            //Arrange
            var mockRepo = new Mock<IJobsRepository>();
            var userStore = Mock.Of<IUserStore<JobBoardUser>>();
            var userManager = new Mock<UserManager<JobBoardUser>>(userStore, null, null, null, null, null, null, null, null);
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();

            var job1 = new Job
            {
                Id = 1,
                Title = "Software Developer",
                Description = "Develop software applications using C# and .NET",
                CreationDate = DateTime.Now,
                ValidityDate = DateTime.Now.AddDays(30),
                Requirements = new List<Requirements> { new Requirements { Name = "C#", JobId = 1 }, new Requirements { Name = ".NET", JobId = 1 } },
                Position = "Software Developer",
                PositionLevel = "Mid-Level",
                CompanyOffers = "Competitive salary and benefits package",
                Location = "New York, NY",
                City = "New York",
                Salary = 80000,
                SalaryUp = 120000,
                RemoteWork = false,
                TotalWorkHours = "Full-time",
                Selection = "Online",
                CompanyId = "123",
                Company = new JobBoardUser { Id = "123", Name = "Acme Corp" },
                IsHidden = false
            };
            testData.Add(job1);
            mockRepo.Setup(repo => repo.GetJobAsync(It.IsAny<int>()))
                          .ReturnsAsync(job1);
            mockRepo.Setup(repo => repo.GetJobsAsync())
                          .ReturnsAsync(testData);
            var controller = new JobsController(mockRepo.Object, userManager.Object, a.Object, b.Object, c.Object);

            //Act
            var results = await controller.GetJobsAsync();
            var result = await controller.GetJobAsync(1);
            var okResult = result.Result as OkObjectResult;
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"OkResult: {okResult}");
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(okResult);
            var model = okResult.Value as Job;
            Assert.IsNotNull(model);
            Assert.AreEqual(job1.Id, model.Id);
            Assert.AreEqual(job1.Title, model.Title);
        }
    }
}