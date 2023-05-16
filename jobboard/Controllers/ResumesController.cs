using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace jobboard.Controllers;

[ApiController]
[Route("api/resumes")]
public class ResumesController : ControllerBase
{
    public readonly IResumesRepository _resumesRepository;
    public readonly IJobsRepository _jobsRepository;
    public readonly IJobResumesRepository _jobResumesRepository;
    private readonly UserManager<JobBoardUser> _userManager;
    private readonly IAuthorizationService _authorizationService;

    public ResumesController(IResumesRepository resumesRepository, UserManager<JobBoardUser> userManager, IJobsRepository jobsRepository, IAuthorizationService authorizationService, IJobResumesRepository jobResumesRepository)
    {
        _resumesRepository = resumesRepository;
        _userManager = userManager;
        _jobsRepository = jobsRepository;
        _authorizationService = authorizationService;
        _jobResumesRepository = jobResumesRepository;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys)]
    public async Task<IEnumerable<DisplayResumesDto>> GetResumesAsync()
    {
        var resumes = await _resumesRepository.GetResumesAsync();
        return resumes.Select(x => new DisplayResumesDto
        (
            x.Id,
            x.FullName,
            x.Email,
            x.PhoneNumber,
            x.Address,
            x.City,
            x.Education,
            x.Skills,
            x.Experience,
            x.Position,
            x.YearOfBirth,
            x.Summary,
            x.UserId,
            x.IsHidden,
            x.Salary
        ));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserResumesAsync(string userId)
    {
        var resumes = await _resumesRepository.GetUserResumesAsync(userId);
        foreach (var resume in resumes)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, resume, PolicyNames.ResourceOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
        }
        return Ok(resumes.Select(x => new DisplayResumesDto
        (
            x.Id,
            x.FullName,
            x.Email,
            x.PhoneNumber,
            x.Address,
            x.City,
            x.Education,
            x.Skills,
            x.Experience,
            x.Position,
            x.YearOfBirth,
            x.Summary,
            x.UserId,
            x.IsHidden,
            x.Salary
        )));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys + "," + Roles.Darbuotojas)]
    public async Task<ActionResult<DisplayResumesDto>> GetResumeAsync(int id)
    {
        var resume = await _resumesRepository.GetResumeAsync(id);
        if (resume == null)
            return NotFound($"Tokio CV nėra!");

        return new DisplayResumesDto
        (
            resume.Id,
            resume.FullName,
            resume.Email,
            resume.PhoneNumber,
            resume.Address,
            resume.City,
            resume.Education,
            resume.Skills,
            resume.Experience,
            resume.Position,
            resume.YearOfBirth,
            resume.Summary,
            resume.UserId,
            resume.IsHidden,
            resume.Salary
        );
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbuotojas)]
    public async Task<ActionResult<DisplayCreatedResumeDto>> Create(CreateResumeCommand createResumeCommand)
    {
        ///VALIDATION
        ///
        ///
        ///
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("Įvyko klaida!");
        var temp = await _resumesRepository.CreateResumeAsync(createResumeCommand, id, user);

        return Created("", temp);
    }
    [HttpPut("visibility/{id}")]
    public async Task<ActionResult<DisplayResumesDto>> UpdateResumeVisibility(int id, UpdateResumesVisibility updateResumesVisibility)
    {
        var oldResume = await _resumesRepository.GetResumeAsync(id);
        if (oldResume == null)
            return NotFound($"{id} CV neegzistuoja!"); //change
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, oldResume, PolicyNames.ResourceOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        oldResume.IsHidden = updateResumesVisibility.isHidden;

        await _resumesRepository.UpdateResumeAsync(oldResume);

        return Ok( new DisplayResumesDto
        (
            oldResume.Id,
            oldResume.FullName,
            oldResume.Email,
            oldResume.PhoneNumber,
            oldResume.Address,
            oldResume.City,
            oldResume.Education,
            oldResume.Skills,
            oldResume.Experience,
            oldResume.Position,
            oldResume.YearOfBirth,
            oldResume.Summary,
            oldResume.UserId,
            oldResume.IsHidden,
            oldResume.Salary
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(int id, UpdateResumeDto updateResumeDto)
    {
        var resume = await _resumesRepository.GetResumeAsync(id);
        if (resume == null)
            return NotFound($"Įvyko klaida");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, resume, PolicyNames.ResourceOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        resume.FullName = string.IsNullOrEmpty(updateResumeDto.FullName) ? resume.FullName : updateResumeDto.FullName ?? resume.FullName;
        resume.Email = string.IsNullOrEmpty(updateResumeDto.Email) ? resume.Email : updateResumeDto.Email ?? resume.Email;
        resume.PhoneNumber = string.IsNullOrEmpty(updateResumeDto.PhoneNumber) ? resume.PhoneNumber : updateResumeDto.PhoneNumber ?? resume.PhoneNumber;
        resume.Address = string.IsNullOrEmpty(updateResumeDto.Address) ? resume.Address : updateResumeDto.Address ?? resume.Address;
        resume.City = string.IsNullOrEmpty(updateResumeDto.City) ? resume.City : updateResumeDto.City ?? resume.City;
        resume.Summary = string.IsNullOrEmpty(updateResumeDto.Summary) ? resume.Summary : updateResumeDto.Summary ?? resume.Summary;
        resume.Position = string.IsNullOrEmpty(updateResumeDto.Position) ? resume.Position : updateResumeDto.Position ?? resume.Position;
        if (updateResumeDto.Salary > 0)
            resume.Salary = (double)updateResumeDto.Salary;

        for (int i = 0; i < updateResumeDto.Experiences.Count && i < resume.Experience.Count; i++)
        {
            resume.Experience[i].Company = updateResumeDto.Experiences[i]?.Company != resume.Experience[i].Company ? updateResumeDto.Experiences[i].Company : resume.Experience[i].Company;
            resume.Experience[i].IsCurrent = (bool)(updateResumeDto.Experiences[i]?.IsCurrent != resume.Experience[i].IsCurrent ? updateResumeDto.Experiences[i].IsCurrent : resume.Experience[i].IsCurrent);
            resume.Experience[i].EndDate = updateResumeDto.Experiences[i]?.EndDate != resume.Experience[i].EndDate ? updateResumeDto.Experiences[i].EndDate : resume.Experience[i].EndDate;
            resume.Experience[i].StartDate = (DateTime)(updateResumeDto.Experiences[i]?.StartDate != resume.Experience[i].StartDate ? updateResumeDto.Experiences[i].StartDate : resume.Experience[i].StartDate);
            resume.Experience[i].Position = updateResumeDto.Experiences[i]?.Position != resume.Experience[i].Position ? updateResumeDto.Experiences[i].Position : resume.Experience[i].Position;
        }

        for (int i = resume.Experience.Count - 1; i >= updateResumeDto.Experiences.Count; i--)
        {
            resume.Experience.RemoveAt(i);
        }

        resume.Experience.RemoveAll(e => string.IsNullOrEmpty(e.Company));
        updateResumeDto.Experiences.RemoveAll(e => string.IsNullOrEmpty(e.Company));

        for (int i = 0; i < updateResumeDto.Experiences.Count; i++)
        {
            if (i < resume.Experience.Count)
            {
                resume.Experience[i].Company = updateResumeDto.Experiences[i].Company;
                resume.Experience[i].StartDate = (DateTime)updateResumeDto.Experiences[i].StartDate;
                resume.Experience[i].EndDate = updateResumeDto.Experiences[i].EndDate;
                resume.Experience[i].IsCurrent = (bool)updateResumeDto.Experiences[i].IsCurrent;
            }
            else
            {
                resume.Experience.Add(new Experience
                {
                    Company = updateResumeDto.Experiences[i].Company,
                    Position = updateResumeDto.Experiences[i].Position,
                    StartDate = (DateTime)updateResumeDto.Experiences[i].StartDate,
                    EndDate = updateResumeDto.Experiences[i].EndDate,
                    IsCurrent = (bool)updateResumeDto.Experiences[i].IsCurrent,
                    ResumeId = resume.Id
                });
            }
        }

        for (int i = 0; i < updateResumeDto.Skills.Count && i < resume.Skills.Count; i++)
        {
            resume.Skills[i].Name = updateResumeDto.Skills[i]?.Name != resume.Skills[i].Name ? updateResumeDto.Skills[i].Name : resume.Skills[i].Name;
        }

        for (int i = resume.Skills.Count - 1; i >= updateResumeDto.Skills.Count; i--)
        {
            resume.Skills.RemoveAt(i);
        }

        resume.Skills.RemoveAll(e => string.IsNullOrEmpty(e.Name));
        updateResumeDto.Skills.RemoveAll(e => string.IsNullOrEmpty(e.Name));

        for (int i = 0; i < updateResumeDto.Skills.Count; i++)
        {
            if (i < resume.Skills.Count)
                resume.Skills[i].Name = updateResumeDto.Skills[i].Name;
            else
            {
                resume.Skills.Add(new Skills
                {
                    Name = updateResumeDto.Skills[i].Name,
                    ResumeId = resume.Id
                });
            }
        }

        for (int i = 0; i < updateResumeDto.Educations.Count && i < resume.Education.Count; i++)
        {
            resume.Education[i].School = updateResumeDto.Educations[i]?.School != resume.Education[i].School ? updateResumeDto.Educations[i].School : resume.Education[i].School;
            resume.Education[i].Degree = (Data.Enums.Degree)(updateResumeDto.Educations[i]?.Degree != resume.Education[i].Degree ? updateResumeDto.Educations[i].Degree : resume.Education[i].Degree);
            resume.Education[i].EndDate = updateResumeDto.Educations[i]?.EndDate != resume.Education[i].EndDate ? updateResumeDto.Educations[i].EndDate : resume.Education[i].EndDate;
            resume.Education[i].IsCurrent = (bool)(updateResumeDto.Educations[i]?.IsCurrent != resume.Education[i].IsCurrent ? updateResumeDto.Educations[i].IsCurrent : resume.Education[i].IsCurrent);
            resume.Education[i].StartDate = (DateTime)(updateResumeDto.Educations[i]?.StartDate != resume.Education[i].StartDate ? updateResumeDto.Educations[i].StartDate : resume.Education[i].StartDate);
        }

        for (int i = resume.Education.Count - 1; i >= updateResumeDto.Educations.Count; i--)
        {
            resume.Education.RemoveAt(i);
        }

        resume.Education.RemoveAll(e => string.IsNullOrEmpty(e.School));
        updateResumeDto.Educations.RemoveAll(e => string.IsNullOrEmpty(e.School));

        for (int i = 0; i < updateResumeDto.Educations.Count; i++)
        {
            if (i < resume.Education.Count)
            {
                resume.Education[i].School = updateResumeDto.Educations[i].School;
                resume.Education[i].Degree = (Data.Enums.Degree)updateResumeDto.Educations[i].Degree;
                resume.Education[i].EndDate = updateResumeDto.Educations[i].EndDate;
                resume.Education[i].StartDate = (DateTime)updateResumeDto.Educations[i].StartDate;
                resume.Education[i].IsCurrent = (bool)updateResumeDto.Educations[i].IsCurrent;
            }
            else
            {
                resume.Education.Add(new Education
                {
                    School = updateResumeDto.Educations[i].School,
                    Degree = (Data.Enums.Degree)updateResumeDto.Educations[i].Degree,
                    EndDate = updateResumeDto.Educations[i].EndDate,
                    StartDate = (DateTime)updateResumeDto.Educations[i].StartDate,
                    IsCurrent = (bool)updateResumeDto.Educations[i].IsCurrent,
                    ResumeId = resume.Id
                });
            }
        }
        await _resumesRepository.UpdateResumeAsync(resume);

        return Ok("Atnaujinta!");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteJobAsync(int id)
    {
        var resume = await _resumesRepository.GetResumeAsync(id);
        var jobResumes = await _jobResumesRepository.GetJobResumesByResume(id);
        if (resume == null)
            return NotFound($"Resume {id} does not exist!"); //change

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, resume, PolicyNames.ResourceOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        foreach (var item in jobResumes)
        {
            await _jobResumesRepository.DeleteResumeSkillAsync(item);
        }
        await _resumesRepository.DeleteResumeAsync(resume);

        return NoContent();
    }
}
