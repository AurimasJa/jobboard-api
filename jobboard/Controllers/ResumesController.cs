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
    private readonly UserManager<JobBoardUser> _userManager;

    public ResumesController(IResumesRepository resumesRepository, UserManager<JobBoardUser> userManager, IJobsRepository jobsRepository)
    {
        _resumesRepository = resumesRepository;
        _userManager = userManager;
        _jobsRepository = jobsRepository;
    }

    [HttpGet]
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
            x.IsHidden
        ));
    }

    [HttpGet("user/{userId}")]
    public async Task<IEnumerable<DisplayResumesDto>> GetUserResumesAsync(string userId)
    {
        var resumes = await _resumesRepository.GetUserResumesAsync(userId);

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
            x.IsHidden
        ));
    }

    [HttpGet("{id}")]
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
            resume.IsHidden
        );
    }

    [HttpPost]
    public async Task<ActionResult<DisplayCreatedResumeDto>> Create(CreateResumeCommand createResumeCommand)
    {
        ///VALIDATION
        ///
        ///
        ///

        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        await _resumesRepository.CreateResumeAsync(createResumeCommand, id);

        return Created("", id);
    }

}
