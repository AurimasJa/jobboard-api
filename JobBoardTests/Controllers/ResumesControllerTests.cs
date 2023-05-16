using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using jobboard.Auth;
using jobboard.Controllers;
using jobboard.Data.Repositories;
using jobboard.Data.Models;
using jobboard.Data.Entities;
using jobboard.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace JobBoardTests.Controllers
{
    [TestClass]
    public class ResumesControllerTests
    {
        private MockRepository mockRepository;

        private Mock<IResumesRepository> mockRepo;
        private Mock<UserManager<JobBoardUser>> mockUserManager;
        private Mock<IJobsRepository> mockJobsRepository;
        private Mock<IAuthorizationService> mockAuthService;
        private Mock<IUserStore<JobBoardUser>> mockUserStore;
        private Mock<IJobResumesRepository> mockJobResumesRepository;

        private List<Resume> _resumes;
        private List<Education> _educations;
        private List<Skills> _skills;
        private List<Experience> _experiences;
        private JobBoardUser _user;
        [TestInitialize]
        public void Initialize()
        {

            mockJobResumesRepository = new Mock<IJobResumesRepository>();
            mockJobsRepository = new Mock<IJobsRepository>();
            mockUserStore = new Mock<IUserStore<JobBoardUser>>();
            mockRepo = new Mock<IResumesRepository>();
            mockAuthService = new Mock<IAuthorizationService>();
            mockUserStore = new Mock<IUserStore<JobBoardUser>>();
            mockUserManager = new Mock<UserManager<JobBoardUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            _user = new JobBoardUser
            {
                Id = "1",
                UserName = "user",
                Email = "test@test.com",
                CompanyName = "test",
                ContactPerson = "test",
                PhoneNumber = "123456789",
                Site = "Nera",
                City = "Yra",
                Address = "Aasd"
            };
            _skills = new List<Skills>
            {
                new Skills
                {
                    Id = 1,
                    Name = "Updated",
                    ResumeId = 1
                },
                new Skills
                {
                    Id = 1,
                    Name = "Updated",
                    ResumeId = 1
                },
            };

            _educations = new List<Education>
            {
                new Education
                {
                    Id = 1,
                    School = "garliavos",
                    Degree = Enums.Degree.PROFFESIONAL,
                    StartDate = DateTime.Parse("1999-03-02"),
                    EndDate = DateTime.Parse("2000-03-02"),
                    IsCurrent = false,
                    ResumeId = 1
                },
                new Education
                {
                    Id = 2,
                    School = "astridos",
                    Degree = Enums.Degree.DOCTOR,
                    StartDate = DateTime.Parse("1999-03-02"),
                    EndDate = DateTime.Parse("2000-03-02"),
                    IsCurrent = false,
                    ResumeId = 1
                },
            };
            _experiences = new List<Experience>
            {
                new Experience
                {
                    Id = 1,
                    Company = "garliavosss",
                    Position = "barmenas",
                    StartDate = DateTime.Parse("1999-03-02"),
                    EndDate = DateTime.Parse("2000-03-02"),
                    IsCurrent = false,
                    ResumeId = 1
                },
                new Experience
                {
                    Id = 2,
                    Company = "testas",
                    Position = "daktaras",
                    StartDate = DateTime.Parse("1999-03-02"),
                    EndDate = DateTime.Parse("2000-03-02"),
                    IsCurrent = false,
                    ResumeId = 1
                },
            };

            _resumes = new List<Resume>
            {
                new Resume
                {
                    Id = 1,
                    Address = "garliava",
                    City = "testas",
                    Experience = _experiences,
                    Education = _educations,
                    Skills = _skills,
                    Email = "kompanija@email.com",
                    FullName = "Petras petraitis",
                    Position = "barmenas",
                    IsHidden = false,
                    PhoneNumber = "+37061833322",
                    References = "",
                    Summary = "Aprasymas ilgas ilgas ilgas ilgas ilgas ilgas ilgas ilgas ilgas ilgas ilgas ilgas",
                    User = _user,
                    UserId = "1",
                    YearOfBirth = DateTime.Parse("1988-09-02")
                },
                new Resume
                {
                    Id = 1,
                    Address = "siauliai",
                    City = "siauliai",
                    Experience = _experiences,
                    Education = _educations,
                    Skills = _skills,
                    Email = "siauliai@email.com",
                    FullName = "siauliai petraitis",
                    Position = "sandelininkas",
                    IsHidden = false,
                    PhoneNumber = "+3702121212",
                    References = "",
                    Summary = "Aprasymas trumpas trumpas trumpas trumpas trumpas trumpas trumpas trumpas",
                    User = _user,
                    UserId = "1",
                    YearOfBirth = DateTime.Parse("1988-09-02")
                },
            };
        }


        [TestMethod]
        public async Task GetCompanyJobsAsync_ReturnLatestsJobs_ShouldReturnList()
        {
            //Arrange
            mockUserManager.Setup(x => x.FindByIdAsync(_user.Id)).ReturnsAsync(_user);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "id")
            }));
            mockRepo.Setup(repo => repo.GetResumesAsync())
                    .ReturnsAsync(_resumes);
            var controller = new ResumesController(mockRepo.Object, mockUserManager.Object, mockJobsRepository.Object, mockAuthService.Object, mockJobResumesRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            //Act
            var results = await controller.GetResumesAsync();
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), _resumes.Count);
        }


        [TestMethod]
        public async Task UpdateResumeAsyncBranches_GetExistResume_UserAuthorized()
        {
            // Arrange
            var resume = _resumes[0];
            List<EducationDto> educationDtos = new List<EducationDto>
            {
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                )
            };
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>
            {
                new ExperienceDto(
                    "updated",
                    "Barmenas",
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new ExperienceDto(
                    "updated",
                    "Barmenas",
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new ExperienceDto(
                    "updated",
                    "Barmenas",
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                )
            };
            List<SkillsDto> skillsDtos = new List<SkillsDto>
            {
                new SkillsDto("updated"),
                new SkillsDto("updated"),
                new SkillsDto("updated"),
                new SkillsDto("updated")
            };
            var updateResumeDto = new UpdateResumeDto
            (
                "Updated",
               "Updated",
                "Updated",
               "Updated",
                "Updated",
                educationDtos,
                skillsDtos,
                experienceDtos,
                "Updated",
                DateTime.Parse("2020-02-20"),
                "Updated",
                1111
            );
            mockRepo.Setup(repo => repo.GetResumeAsync(1)).ReturnsAsync(resume);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), resume, PolicyNames.ResourceOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new ResumesController(mockRepo.Object, mockUserManager.Object, mockJobsRepository.Object, mockAuthService.Object, mockJobResumesRepository.Object);

            // Act
            var result = await controller.UpdateResume(1, updateResumeDto);

            // Assert

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateResume_UserUnauthorized_ReturnForbidden()
        {
            // Arrange
            var resume = _resumes[0];
            List<EducationDto> educationDtos = new List<EducationDto>
            {
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                ),
                new EducationDto(
                    "updated",
                    Enums.Degree.PROFFESIONAL,
                    DateTime.Parse("2010-02-20"),
                    DateTime.Parse("2020-02-20"),
                    false
                )
            };
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();
            List<SkillsDto> skillsDtos = new List<SkillsDto>
            {
                new SkillsDto("updated"),
                new SkillsDto("updated"),
                new SkillsDto("updated"),
                new SkillsDto("updated")
            };
            var updateResumeDto = new UpdateResumeDto
            (
                "",
               "",
                "",
               "",
                "",
                educationDtos,
                skillsDtos,
                experienceDtos,
                "",
                DateTime.Parse("2020-02-20"),
                "",
                1234
            );
            mockRepo.Setup(repo => repo.GetResumeAsync(1)).ReturnsAsync(resume);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), resume, PolicyNames.ResourceOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new ResumesController(mockRepo.Object, mockUserManager.Object, mockJobsRepository.Object, mockAuthService.Object, mockJobResumesRepository.Object);

            // Act
            var result = await controller.UpdateResume(1, updateResumeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }


        [TestMethod]
        public async Task UpdateResume_ResumeDoesNotExist_ReturnNotFound()
        {
            // Arrange
            var resume = _resumes[0];
            List<EducationDto> educationDtos = new List<EducationDto>();
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();
            List<SkillsDto> skillsDtos = new List<SkillsDto>();
            var updateResumeDto = new UpdateResumeDto
            (
                "",
               "",
                "",
               "",
                "",
                educationDtos,
                skillsDtos,
                experienceDtos,
                "",
                DateTime.Parse("2020-02-20"),
                "",
                1234
            );
            mockRepo.Setup(repo => repo.GetResumeAsync(1)).ReturnsAsync(resume);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), resume, PolicyNames.ResourceOwner))
                .ReturnsAsync(AuthorizationResult.Failed());
            var controller = new ResumesController(mockRepo.Object, mockUserManager.Object, mockJobsRepository.Object, mockAuthService.Object, mockJobResumesRepository.Object);

            // Act
            var result = await controller.UpdateResume(51, updateResumeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task UpdateResume_WithNullValues_ReturnOk()
        {
            // Arrange
            var resume = _resumes[0];
            List<EducationDto> educationDtos = new List<EducationDto>();
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();
            List<SkillsDto> skillsDtos = new List<SkillsDto>();
            var updateResumeDto = new UpdateResumeDto
            (
                null,
                null,
                null,
                null,
                null,
                educationDtos,
                skillsDtos,
                experienceDtos,
                null,
                null,
                null,
                -4
            );
            mockRepo.Setup(repo => repo.GetResumeAsync(1)).ReturnsAsync(resume);
            mockAuthService.Setup(service => service.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), resume, PolicyNames.ResourceOwner))
                .ReturnsAsync(AuthorizationResult.Success());
            var controller = new ResumesController(mockRepo.Object, mockUserManager.Object, mockJobsRepository.Object, mockAuthService.Object, mockJobResumesRepository.Object);

            // Act
            var result = await controller.UpdateResume(1, updateResumeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
