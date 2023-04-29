using AutoMapper;
using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using jobboard.Helpers;
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
    public JobsController(IJobsRepository jobsRepository, UserManager<JobBoardUser> userManager, ICalculations calc, IResumesRepository resumesRepository)
    {
        _calc = calc;
        _jobsRepository = jobsRepository;
        _userManager = userManager;
        _resumesRepository = resumesRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Job>> GetJobsAsync()
    {

        //TODO: hangfire
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetJobsAsync();
        return jobs.Select(x => new Job
        {
            Id = x.Id,
            City = x.City,
            Description = x.Description,
            Title = x.Title,
            Salary = x.Salary,
            SalaryUp = x.SalaryUp,
            //Location = x.Location,
            CreationDate = x.CreationDate,
            Position = x.Position,
            //Skills = x.Skills,
            //CompanyOffers = x.CompanyOffers,
            //Company = x.Company,
            //TotalWorkHours = x.TotalWorkHours,
            ////Requirements = x.Requirements
            //Selection = x.Selection,
            //PositionLevel = x.PositionLevel,
            RemoteWork = x.RemoteWork,
            IsHidden = x.IsHidden
        });
    }
    [HttpGet("details")]
    public async Task<IEnumerable<Job>> GetSimilarJobsAsync(string position, string city, int id)
    {

        //TODO: hangfire
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetSimilarJobsAsync(position, city, id);
        return jobs.Select(x => new Job
        {
            Id = x.Id,
            City = x.City,
            Description = x.Description,
            Title = x.Title,
            Salary = x.Salary,
            SalaryUp = x.SalaryUp,
            //Location = x.Location,
            CreationDate = x.CreationDate,
            Position = x.Position,
            //Skills = x.Skills,
            //CompanyOffers = x.CompanyOffers,
            //Company = x.Company,
            //TotalWorkHours = x.TotalWorkHours,
            ////Requirements = x.Requirements
            //Selection = x.Selection,
            //PositionLevel = x.PositionLevel,
            RemoteWork = x.RemoteWork,
            IsHidden = x.IsHidden
        });
    }
    [HttpGet("company/{companyId}")]
    public async Task<IEnumerable<Job>> GetCompanyJobsAsync(string companyId)
    {

        //TODO: hangfire
        _jobsRepository.CheckAndUpdateValidityDate();

        var jobs = await _jobsRepository.GetCompanyJobsAsync(companyId);
        return jobs.Select(x => new Job
        {
            Id = x.Id,
            City = x.City,
            Description = x.Description,
            Title = x.Title,
            Salary = x.Salary,
            SalaryUp = x.SalaryUp,
            //Location = x.Location,
            CreationDate = x.CreationDate,
            Position = x.Position,
            //Skills = x.Skills,
            //CompanyOffers = x.CompanyOffers,
            //Company = x.Company,
            //TotalWorkHours = x.TotalWorkHours,
            ////Requirements = x.Requirements
            //Selection = x.Selection,
            //PositionLevel = x.PositionLevel,
            RemoteWork = x.RemoteWork,
            IsHidden = x.IsHidden
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Job>> GetJobAsync(int id)
    {
        var job = await _jobsRepository.GetRequirementsAsync(id);
        if(job == null) 
            return null;
        var x = _userManager.FindByIdAsync(job.CompanyId);
        return new Job
        {
            Id = job.Id,
            Company = x.Result,
            Title = job.Title,
            Description = job.Description.Replace(Environment.NewLine, "<br />"),
            Position = job.Position,
            PositionLevel = job.PositionLevel,
            CompanyOffers = job.CompanyOffers.Replace(Environment.NewLine, "<br />"),
            Requirements = job.Requirements,
            City = job.City,
            SalaryUp = job.SalaryUp,
            Location = job.Location,
            Salary = job.Salary,
            TotalWorkHours = job.TotalWorkHours,
            RemoteWork = job.RemoteWork,
            Selection = job.Selection,
            CreationDate = job.CreationDate,
            ValidityDate = job.ValidityDate,
            IsHidden = job.IsHidden
        };
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
    public async Task<IEnumerable<CompanyDto>> GetBiggestCompanies()
    {
        var jobs = await _jobsRepository.GetJobsAsync();
        var companies = await _userManager.GetUsersInRoleAsync(Roles.Darbdavys);
        var biggestCompanies = _calc.GetBiggestCompanies(jobs, companies);
        return biggestCompanies;
    }

    [HttpPost]
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

        return Created("", new GetJobCommand(job.Id, job.Company));
    }


    [HttpPut("validity/{id}")]
    public async Task<ActionResult<DisplayedJobDto>> UpdateJobValidity(int id, UpdateJobValidityDto updateJobValidityDto)
    {
        var oldJob = await _jobsRepository.GetJobAsync(id);
        if (oldJob == null)
            return NotFound($"Job {id} does not exist!"); //change

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
        await _jobsRepository.DeleteJobAsync(job);

        return NoContent();
    }
}
