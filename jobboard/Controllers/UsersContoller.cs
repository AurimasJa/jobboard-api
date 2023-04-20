using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace jobboard.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersContoller : ControllerBase
    {
        private readonly UserManager<JobBoardUser> _userManager;
        private readonly IResumesRepository _resumesRepository;
        private readonly IJobsRepository _jobsRepository;
        private readonly IJwtTokenService _jwtTokenService;
        public UsersContoller(UserManager<JobBoardUser> userManager, IResumesRepository resumesRepository, IJobsRepository jobsRepository, IJwtTokenService jwtTokenService)
        {
            _resumesRepository = resumesRepository;
            _jobsRepository = jobsRepository;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RealUserDto>> GetUserData(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Tokio naudotojo nėra"); // keist

            return new RealUserDto
            (
                user.Id,
                user.Name,
                user.Email,
                user.Surname,
                user.DateOfBirth
            );
        }
        [HttpGet]
        [Route("company/{id}")]
        public async Task<CompanyProfileDto> GetCompanyDate(string id)
        {
            var company = await _userManager.FindByIdAsync(id);
            if (company == null)
                return null; // keist

            return new CompanyProfileDto
            (
                company.CompanyName,
                company.AboutSection,
                company.CompanyCode,
                company.PhoneNumber,
                company.Address,
                company.City,
                company.Created,
                company.Site,
                company.ContactPerson,
                company.Email

            );
        }


        [HttpGet]
        [Route("applied/job/{id}")]
        public async Task<IEnumerable<JobResumesAppliedToDto>> GetJobResumesAppliedDto(string id)
        {
            List<JobResumes> applied = new List<JobResumes>();
            var resumes = await _resumesRepository.GetUserResumesAsync(id);
            foreach (var item in resumes)
            {
                var temp = await _jobsRepository.GetJobResumesAsync(item.Id);
                applied.AddRange(temp);
            }
            return applied.Select(x => new JobResumesAppliedToDto
            (
                x.Id,
                new DisplayedJobDto
                (
                    x.Job.Id,
                    x.Job.Title,
                    x.Job.Company.CompanyName,
                    x.Job.Company.Address,
                    x.Job.Company.Email,
                    new DisplayCustomUserInfoDto
                    (
                        x.Job.Company.Id,
                        (x.Job.Company.Name + " " + x.Job.Company.Surname),
                        x.Job.Company.Email
                    )
                ),
                new DisplayCustomResumeDto
                (
                    x.Resume.Id,
                    x.Resume.FullName,
                    new DisplayCustomUserInfoDto
                    (
                        x.Resume.User.Id,
                        (x.Resume.User.Name + " " + x.Resume.User.Surname),
                        x.Resume.User.Email
                    )
               ),
               x.CreationDate,
               x.Reviewed
            ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SuccessfulLoginDto>> UpdateJobValidity(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"Įvyko klaida");

            user.Surname = user.Surname == updateUserDto.Surname || String.IsNullOrEmpty(updateUserDto.Surname) ? user.Surname : updateUserDto.Surname;
            user.Name = user.Name == updateUserDto.Name || String.IsNullOrEmpty(updateUserDto.Name) ? user.Name : updateUserDto.Name;
            user.Email = user.Email == updateUserDto.Email || String.IsNullOrEmpty(updateUserDto.Email) ? user.Email : updateUserDto.Email;
            //if(updateUserDto.DateOfBirth is not null)
            //{
            //    user.DateOfBirth = user.DateOfBirth == updateUserDto.DateOfBirth ? user.DateOfBirth : updateUserDto.DateOfBirth;
            //}

            if(updateUserDto.Password is not null && updateUserDto.NewPassword is not null)
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, updateUserDto.Password);
                if (!isPasswordValid)
                    return BadRequest("Nepavyko atnaujinti slaptažodžio");
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, updateUserDto.NewPassword);
                if (!result.Succeeded)
                    return BadRequest("Nepavyko atnaujinti slaptažodžio");
            }

            var response = await _userManager.UpdateAsync(user);
            if (!response.Succeeded)
                return BadRequest("Nepavyko atnaujinti naudotojo");

            var roles = await _userManager.GetRolesAsync(user);
            var fullName = user.Name;
            var accesToken = _jwtTokenService.CreateAccessToken(fullName, user.Id, roles);

            return Ok(new SuccessfulLoginDto(accesToken));
        }
    }
}
