using jobboard.Auth;
using jobboard.Controllers;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using jobboard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JobBoardTests.Controllers
{
    [TestClass]
    public class JobsControllerTests
    {
        private List<Job> _jobs;
        private List<Requirements> _requirements;
        private JobBoardUser _user;
        private Mock<IUserStore<JobBoardUser>> mockUserStore;
        private Mock<UserManager<JobBoardUser>> mockUserManager;
        private Mock<IAuthorizationService> mockAuthService;
        private Mock<IJobsRepository> mockRepo;

        [TestInitialize]
        public void Initialize()
        {

             mockRepo = new Mock<IJobsRepository>();
             mockAuthService = new Mock<IAuthorizationService>();
            mockUserStore = new Mock<IUserStore<JobBoardUser>>();
             mockUserManager = new Mock<UserManager<JobBoardUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            _user = new JobBoardUser
            {
                Id = "1",
                UserName = "user",
                Email = "test@test.com",
                CompanyName = "test",
                ContactPerson = "test test",
                PhoneNumber = "123456789",
                Site = "Nera",
                City = "Yra",
                Address = "test"
            };
            _requirements = new List<Requirements>
            {
                new Requirements
                {
                    Id = 1,
                    Name = "Updated",
                    JobId = 1
                },
                new Requirements
                {
                    Id = 2,
                    Name = "Updated",
                    JobId = 1
                },
            };
            _jobs = new List<Job>

            {
            new Job
            {
            Id = 1,
                Title = "Updated",
                Description = "Updated",
                CreationDate = DateTime.Now,
                ValidityDate = DateTime.Now.AddDays(30),
                Requirements = new List<Requirements> { new Requirements { Name = "C#", JobId = 1 }, new Requirements { Name = "Updated", JobId = 1 } },
                Position = "Updated",
                PositionLevel = "Updated",
                CompanyOffers = "Updated",
                Location = "Updated",
                City = "Updated",
                Salary = 1000,
                SalaryUp = 2000,
                RemoteWork = false,
                TotalWorkHours = "Updated",
                Selection = "Updated",
                CompanyId = "1",
                Company = _user,
                IsHidden = false
            },
            new Job
            {
            Id = 2,
                Title = "TESTAS",
                Description = "TESTAS",
                CreationDate = DateTime.Now,
                ValidityDate = DateTime.Now.AddDays(30),
                Requirements = new List<Requirements> { new Requirements { Name = "TESTAS", JobId = 1 }, new Requirements { Name = "TESTAS", JobId = 1 } },
                Position = "TESTAS",
                PositionLevel = "TESTAS",
                CompanyOffers = "TESTAS",
                Location = "TESTAS",
                City = "TESTAS",
                Salary = 80000,
                SalaryUp = 300000,
                RemoteWork = false,
                TotalWorkHours = "TESTAS",
                Selection = "TESTAS",
                CompanyId = "1",
                Company = _user,
                IsHidden = false
            }
        };

        }
        [TestMethod]
        public async Task GetJobAsync_ReturnJob_ShouldReturnSpecificJob()
        {
            //Arrange
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();
            mockUserManager.Setup(x => x.FindByIdAsync(_user.Id)).ReturnsAsync(_user);

            mockRepo.Setup(repo => repo.GetRequirementsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _jobs.FirstOrDefault(j => j.Id == id));
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, a.Object, b.Object, c.Object);

            //Act
            var result = await controller.GetJobAsync(1);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Value.Id, _jobs[0].Id);
        }

        [TestMethod]
        public async Task GetJobAsync_ReturnJob_ShouldReturnNotFound()
        {
            //Arrange
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();
            mockUserManager.Setup(x => x.FindByIdAsync(_user.Id)).ReturnsAsync(_user);

            mockRepo.Setup(repo => repo.GetRequirementsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _jobs.FirstOrDefault(j => j.Id == id));
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, a.Object, b.Object, c.Object);

            //Act
            var result = await controller.GetJobAsync(23);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetLatestJobsAsync_ReturnLatestsJobs_ShouldReturnList()
        {
            //Arrange
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();
            mockUserManager.Setup(x => x.FindByIdAsync(_user.Id)).ReturnsAsync(_user);

            mockRepo.Setup(repo => repo.GetLatestJobsAsync())
                    .ReturnsAsync(_jobs);
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, a.Object, b.Object, c.Object);

            //Act
            var results = await controller.GetLatestJobsAsync();
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), _jobs.Count);
        }

        [TestMethod]
        public async Task GetCompanyJobsAsync_ReturnCompanyJobs_ShouldReturnList()
        {
            //Arrange
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();
            mockUserManager.Setup(x => x.FindByIdAsync(_user.Id)).ReturnsAsync(_user);

            mockRepo.Setup(repo => repo.GetCompanyJobsAsync(It.IsAny<string>()))
                    .ReturnsAsync((string id) => _jobs.Where(j => j.CompanyId == id).ToList());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, a.Object, b.Object, c.Object);

            //Act
            var results = await controller.GetCompanyJobsAsync("1");
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), _jobs.Count);
        }
        [TestMethod]
        public async Task GetJobsAsync_ReturnListOfJobs_ShouldReturnListCount()
        {
            //Arrange
            var userStore = Mock.Of<IUserStore<JobBoardUser>>();
            var userManager = new Mock<UserManager<JobBoardUser>>(userStore, null, null, null, null, null, null, null, null);
            var a = new Mock<ICalculations>();
            var b = new Mock<IResumesRepository>();
            var c = new Mock<IAuthorizationService>();
            var testData = new List<Job>();

            mockRepo.Setup(repo => repo.GetJobsAsync())
                    .ReturnsAsync(_jobs);
            var controller = new JobsController(mockRepo.Object, userManager.Object, a.Object, b.Object, c.Object);

            //Act
            var results = await controller.GetJobsAsync();
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), _jobs.Count);
        }

        [TestMethod]
        public async Task CreateJob_WithValidData_ReturnsCreated()
        {
            // Arrange
            var createJobCommand = new CreateJobCommand
            (
               "pavadinimas",
                "miestas",
               "aprasymas",
               new List<Requirements>
        {
            new Requirements { Name = "req 1" },
            new Requirements { Name = "req 2" }
        },
                "pozicijos sritis",
                "pozicijos lvl",
               "offeris",
                "vieta",
                30000,
                true,
                "pilnas etatas",
               "selectionas",
                40000

            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "company_id")
            }));
            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.CreateJobAsync(It.IsAny<Job>()))
                .Callback<Job>(job => job.Id = 4)
                .Returns(Task.CompletedTask);
            mockRepo.Setup(repo => repo.CreateJobRequirementsAsync(It.IsAny<Requirements>()))
                .Returns(Task.CompletedTask);

            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            // Act
            var result = await controller.Create(createJobCommand);

            // Assert


            Assert.IsInstanceOfType(result, typeof(CreatedResult));
        }
        [TestMethod]
        public async Task CreateJob_DataIsNotValid_UserAuthorized()
        {
            // Arrange
            var createJobCommand = new CreateJobCommand
            (
               "ps",
                "miestas",
               "aprasymas",
               new List<Requirements>
        {
            new Requirements { Name = "req 1" },
            new Requirements { Name = "req 2" }
        },
                "pozicijos sritis",
                "pozicijos lvl",
               "offeris",
                "vieta",
                30000,
                false,
                "pilnas etatas",
               "selectionas",
                40000

            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, "company_id")
    }));

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.CreateJobAsync(It.IsAny<Job>()))
                .Callback<Job>(job => job.Id = 4)
                .Returns(Task.CompletedTask);
            mockRepo.Setup(repo => repo.CreateJobRequirementsAsync(It.IsAny<Requirements>()))
                .Returns(Task.CompletedTask);

            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            // Act
            var result = await controller.Create(createJobCommand);

            // Assert


            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateJobValidityAsync_GetExistJob_UserAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobValidityDto = new UpdateJobValidityDto
            (
                DateTime.Parse("2000-03-04"),
                false
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJobValidity(1, updateJobValidityDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));


        }
        [TestMethod]
        public async Task UpdateJobValidityAsync_GetExistJob_UserUnauthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobValidityDto = new UpdateJobValidityDto
            (
                DateTime.Parse("2000-03-04"),
                false
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Failed());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJobValidity(1, updateJobValidityDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));


        }
        [TestMethod]
        public async Task UpdateJobValidityAsync_GetNotExistingJob_UserAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobValidityDto = new UpdateJobValidityDto
            (
                DateTime.Parse("2000-03-04"),
                false
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJobValidity(5, updateJobValidityDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));


        }

        [TestMethod]
        public async Task UpdateJobAsync_GetExistJob_UserAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobDto = new UpdateJobDto
            (
                "Updated Title",
               "Updated Description",
                "Updated Position",
               "Updated Position Level",
                "Updated Company Offers",
                "Updated Location",
                "Updated City",
                100000,
                100,
                true,
                "Updated Total",
                "Updated Selection",
                new List<Requirements>
                    {
                        new Requirements {Id = 1, Name = null, JobId = 1 },
                        new Requirements {Id = 1, Name = "Updatedgv", JobId = 1 },
                        new Requirements {Id = 1, Name = "C#", JobId = 1 }
                    }
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJob(1, updateJobDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            mockRepo.Verify(repo => repo.UpdateJobAsync(It.IsAny<Job>()), Times.Once);
            Assert.AreEqual(job.Title, updateJobDto.Title);


        }
        [TestMethod]
        public async Task UpdateJobAsync_GetNotExistJob_UserAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobDto = new UpdateJobDto
            (
                "Updated Title",
               "Updated Description",
                "Updated Position",
               "Updated Position Level",
                "Updated Company Offers",
                "Updated Location",
                "Updated City",
                3000,
                4000,
                true,
                "Updated Total",
                "Updated Selection",
                new List<Requirements>
                    {
                        new Requirements {Id = 1, Name = "Updated req 1", JobId = 1 },
                        new Requirements {Id = 1, Name = "Updated req 1", JobId = 1 }
                    }
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJob(5, updateJobDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        }
        [TestMethod]
        public async Task UpdateJobAsyncBranches_GetExistJob_UserAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobDto = new UpdateJobDto
            (
                "Updated",
               "Updated",
                "Updated",
               "Updated",
                "Updated",
                "Updated",
                "Updated",
                1200,
                1500,
                true,
                "Updated",
                "Updated",
                new List<Requirements>
                    {
                        new Requirements {Id = 1, Name = "", JobId = 1 }
                    }
            );
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJob(1, updateJobDto);

            // Assert

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateJobAsync_GetExistJob_UserUnAuthorized()
        {
            // Arrange
            var job = _jobs[0];
            var updateJobDto = new UpdateJobDto
            (
                "Updated Title",
               "Updated Description",
                "Updated Position",
               "Updated Position Level",
                "Updated Company Offers",
                "Updated Location",
                "Updated City",
                3000,
                4000,
                true,
                "Updated Total",
                "Updated Selection",
                new List<Requirements>
                    {
                        new Requirements {Id = 1, Name = "Updated req 1", JobId = 1 },
                        new Requirements {Id = 1, Name = "Updated req 1", JobId = 1 }
                    }
            );

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(1)).ReturnsAsync(job);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), job, PolicyNames.CompanyOwner))
                .ReturnsAsync(AuthorizationResult.Failed());
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.UpdateJob(1, updateJobDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));

        }
        [TestMethod]
        public async Task DeleteJobAsync_GetExistJob_UserUnauthorized()
        {
            // Arrange
            var jobId = 1;

            var mockAuthService = new Mock<IAuthorizationService>();
            mockRepo.Setup(repo => repo.GetJobAsync(jobId)).ReturnsAsync(_jobs[0]);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _jobs[0], PolicyNames.CompanyOwner))
                           .ReturnsAsync(AuthorizationResult.Failed);
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.DeleteJobAsync(jobId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task DeleteJobAsync_GetExistJob_UserAuthorized()
        {
            // Arrange
            var jobId = 1;

            var mockAuthService = new Mock<IAuthorizationService>();
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _jobs[0], PolicyNames.CompanyOwner))
                           .ReturnsAsync(AuthorizationResult.Success);

            mockRepo.Setup(repo => repo.GetJobAsync(jobId)).ReturnsAsync(_jobs[0]);
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.DeleteJobAsync(jobId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task DeleteJobAsync_JobDoesNotExist_UserAuthorized()
        {
            // Arrange
            var jobId = 5;

            var mockAuthService = new Mock<IAuthorizationService>();
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _jobs[0], PolicyNames.CompanyOwner))
                           .ReturnsAsync(AuthorizationResult.Success);
            var controller = new JobsController(mockRepo.Object, mockUserManager.Object, null, null, mockAuthService.Object);

            // Act
            var result = await controller.DeleteJobAsync(jobId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}