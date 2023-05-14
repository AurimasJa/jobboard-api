using AutoMapper;
using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using jobboard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace jobboard.Controllers;

[ApiController]
[Route("api/job")]
public class JobsController : ControllerBase
{
    public readonly ICalculations _calc;
    public readonly IJobsRepository _jobsRepository;
    public readonly IResumesRepository _resumesRepository;
    public readonly UserManager<JobBoardUser> _userManager;
    public readonly IAuthorizationService _authorizationService;
    public JobsController(IJobsRepository jobsRepository, UserManager<JobBoardUser> userManager, ICalculations calc, IResumesRepository resumesRepository, IAuthorizationService authorizationService)
    {
        _calc = calc;
        _jobsRepository = jobsRepository;
        _userManager = userManager;
        _resumesRepository = resumesRepository;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<IEnumerable<JobDto>> GetJobsAsync()
    {
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetJobsAsync();
        return jobs.Select(x => new JobDto
        (
            x.Id,
            x.City,
            x.Description,
            x.Title,
            x.Salary,
            x.SalaryUp,
            x.CreationDate,
            x.Position,
            x.RemoteWork,
            x.TotalWorkHours,
            x.IsHidden
        ));
    }
    [HttpGet("details")]
    public async Task<IEnumerable<JobDto>> GetSimilarJobsAsync(string position, string city, int id)
    {
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetSimilarJobsAsync(position, city, id);
        return jobs.Select(x => new JobDto
        (
            x.Id,
            x.City,
            x.Description,
            x.Title,
            x.Salary,
            x.SalaryUp,
            x.CreationDate,
            x.Position,
            x.RemoteWork,
            x.TotalWorkHours,
            x.IsHidden
        ));
    }
    [HttpGet("company/{companyId}")]
    public async Task<IEnumerable<JobDto>> GetCompanyJobsAsync(string companyId)
    {

        //TODO: hangfire
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetCompanyJobsAsync(companyId);
        return jobs.Select(x => new JobDto
        (
            x.Id,
            x.City,
            x.Description,
            x.Title,
            x.Salary,
            x.SalaryUp,
            x.CreationDate,
            x.Position,
            x.RemoteWork,
            x.TotalWorkHours,
            x.IsHidden
        ));
    }
    [HttpGet("latest")]
    public async Task<IEnumerable<JobDto>> GetLatestJobsAsync()
    {
        var jobs = await _jobsRepository.GetLatestJobsAsync();

        return jobs.Select(x => new JobDto
        (
            x.Id,
            x.City,
            x.Description,
            x.Title,
            x.Salary,
            x.SalaryUp,
            x.CreationDate,
            x.Position,
            x.RemoteWork,
            x.TotalWorkHours,
            x.IsHidden
        ));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<FullJobDto>> GetJobAsync(int id)
    {
        var job = await _jobsRepository.GetRequirementsAsync(id);
        if(job == null) 
            return NotFound($"Darbas {id} neegzistuoja!");
        var temp = await _userManager.FindByIdAsync(job.CompanyId);
        return new FullJobDto
    (
        job.Id,
        new CompanyJobDto(temp.Id, temp.CompanyName, temp.ContactPerson, temp.PhoneNumber),
        job.Title,
        job.Description.Replace(Environment.NewLine, "<br />"),
        job.Position,
        job.PositionLevel,
        job.CompanyOffers.Replace(Environment.NewLine, "<br />"),
        job.Requirements,
        job.City,
        job.SalaryUp,
        job.Location,
        job.Salary,
        job.TotalWorkHours,
        job.RemoteWork,
        job.Selection,
        job.CreationDate,
        job.ValidityDate,
        job.IsHidden
    );
    }

    [HttpGet("average")]
    public async Task<IEnumerable<AverageSalary>> GetAverageCitySalary()
    {
        var jobs = await _jobsRepository.GetJobsAsync();
        var averages = _calc.GetCityAverageSalaries(jobs);
        return averages.Select(x => new AverageSalary
        {
            CityName = x.CityName,
            AverageCitySalary = x.AverageCitySalary
        });
    }
    [HttpGet("biggest/companies")]
    public async Task<IEnumerable<BiggestCompaniesDto>> GetBiggestCompanies()
    {
        var jobs = await _jobsRepository.GetJobsAsync();
        var companies = await _userManager.GetUsersInRoleAsync(Roles.Darbdavys);
        var biggestCompanies = _calc.GetBiggestCompanies(jobs, companies);
        return biggestCompanies;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys)]
    public async Task<ActionResult<CreateJobCommand>> Create(CreateJobCommand createJobCommand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = new Job
        {
            Title = createJobCommand.Title,
            City = createJobCommand.City,
            Description = createJobCommand.Description.Replace("\n", Environment.NewLine),
            Position = createJobCommand.Position,
            PositionLevel = createJobCommand.PositionLevel,
            CompanyOffers = createJobCommand.CompanyOffers.Replace("\n", Environment.NewLine),
            Location = createJobCommand.Location,
            Salary = createJobCommand.Salary,
            SalaryUp = createJobCommand.SalaryUp,
            TotalWorkHours = createJobCommand.TotalWorkHours,
            RemoteWork = createJobCommand.RemoteWork,
            Selection = createJobCommand.Selection,
            CompanyId = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            CreationDate = DateTime.Today.Date,
            ValidityDate = DateTime.Today.Date.AddDays(30)
        };

        job.IsHidden = job.ValidityDate < job.CreationDate ? true : false;

        await _jobsRepository.CreateJobAsync(job);
        foreach (var requirement in createJobCommand.Requirements)
        {
            requirement.JobId = job.Id;
            await _jobsRepository.CreateJobRequirementsAsync(requirement);
        };

        return Created("", new GetJobCommand(job.Id));
    }


    [HttpPut("validity/{id}")]
    public async Task<ActionResult<DisplayedJobDto>> UpdateJobValidity(int id, UpdateJobValidityDto updateJobValidityDto)
    {
        var oldJob = await _jobsRepository.GetJobAsync(id);
        if (oldJob == null)
            return NotFound($"Job {id} does not exist!"); //change

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, oldJob, PolicyNames.CompanyOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        oldJob.IsHidden = updateJobValidityDto.IsHidden;
        oldJob.ValidityDate = updateJobValidityDto.ValidityDate;

        await _jobsRepository.UpdateJobAsync(oldJob);

        return Ok(new DisplayedJobDto
            (
            oldJob.Id,
            oldJob.Title,
            oldJob.Company.CompanyName,
            oldJob.Location,
            oldJob.Company.Email,
            new DisplayCustomUserInfoDto
                (
                    oldJob.Company.Id, 
                    oldJob.Company.Name + " " + oldJob.Company.Surname,
                    oldJob.Company.Email
                )
            )
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SuccessfulLoginDto>> UpdateJob(int id, UpdateJobDto updateJobDto)
    {
        var job = await _jobsRepository.GetJobAsync(id);
        if (job == null)
            return NotFound($"Įvyko klaida");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, PolicyNames.CompanyOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        job.Title = job.Title == updateJobDto.Title || String.IsNullOrEmpty(updateJobDto.Title) ? job.Title : updateJobDto.Title;
        job.Description = updateJobDto.Description.Replace("\n", Environment.NewLine);
        job.Location = job.Location == updateJobDto.Location || String.IsNullOrEmpty(updateJobDto.Location) ? job.Location : updateJobDto.Location;
        job.CompanyOffers = updateJobDto.CompanyOffers.Replace("\n", Environment.NewLine);
        job.City = job.City == updateJobDto.City || String.IsNullOrEmpty(updateJobDto.City) ? job.City : updateJobDto.City;
        job.Selection = job.Selection == updateJobDto.Selection || String.IsNullOrEmpty(updateJobDto.Selection) ? job.Selection : updateJobDto.Selection;
        job.Position = job.Position == updateJobDto.Position || String.IsNullOrEmpty(updateJobDto.Position) ? job.Position : updateJobDto.Position;
        job.PositionLevel = job.PositionLevel == updateJobDto.PositionLevel || String.IsNullOrEmpty(updateJobDto.PositionLevel) ? job.PositionLevel : updateJobDto.PositionLevel;
        job.RemoteWork = (bool)updateJobDto.RemoteWork;

        if (updateJobDto.SalaryUp > 0 && updateJobDto.SalaryUp > job.Salary)
            job.SalaryUp = (double)updateJobDto.SalaryUp;
        if(updateJobDto.Salary > 0 && updateJobDto.Salary < job.SalaryUp)
            job.Salary = (double)updateJobDto.Salary;

        for (int i = 0; i < updateJobDto.Requirements.Count && i < job.Requirements.Count; i++)
        {
            if (updateJobDto.Requirements[i]?.Name != job.Requirements[i].Name)
                job.Requirements[i].Name = updateJobDto.Requirements[i].Name;
        }

        for (int i = job.Requirements.Count - 1; i >= updateJobDto.Requirements.Count; i--)
        {
            job.Requirements.RemoveAt(i);
        }

        job.Requirements.RemoveAll(e => string.IsNullOrEmpty(e.Name));
        updateJobDto.Requirements.RemoveAll(e => string.IsNullOrEmpty(e.Name));

        for (int i = 0; i < updateJobDto.Requirements.Count; i++)
        {
            if (i < job.Requirements.Count)
                job.Requirements[i].Name = updateJobDto.Requirements[i].Name;
            else
            {
                job.Requirements.Add(new Requirements
                {
                    Name = updateJobDto.Requirements[i].Name,
                    JobId = job.Id
                });
            }
        }

        await _jobsRepository.UpdateJobAsync(job);

        return Ok("Atnaujinta!");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteJobAsync(int id)
    {
        var job = await _jobsRepository.GetJobAsync(id);
        if (job == null)
            return NotFound($"Job {id} does not exist!"); //change

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, PolicyNames.CompanyOwner);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await _jobsRepository.DeleteJobAsync(job);

        return NoContent();
    }
}
