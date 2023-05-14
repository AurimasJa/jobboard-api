using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace jobboard.Controllers
{
    [ApiController]
    [Route("api/jobsresumes")]
    public class JobsResumesController : ControllerBase
    {
        private readonly IResumesRepository _resumesRepository;
        private readonly IJobsRepository _jobsRepository;
        private readonly IJobResumesRepository _jobResumesRepository;
        private readonly UserManager<JobBoardUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public JobsResumesController(IResumesRepository resumesRepository, UserManager<JobBoardUser> userManager, IJobsRepository jobsRepository, IJobResumesRepository jobResumesRepository, IAuthorizationService authorizationService)
        {
            _resumesRepository = resumesRepository;
            _userManager = userManager;
            _jobsRepository = jobsRepository;
            _jobResumesRepository = jobResumesRepository;
            _authorizationService = authorizationService;
        }
        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJobResumesAsync(int jobId)
        {
            var job = await _jobsRepository.GetJobAsync(jobId);
            if(job == null)
                return NotFound($"{jobId} neegzistuoja!");

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, PolicyNames.CompanyOwner);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var resumes = await _jobResumesRepository.GetSelectedResumes(jobId);

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
                x.Summary,
                x.YearOfBirth,
                x.Position,
                x.UserId,
                x.IsHidden
            )));
        }
        [HttpGet("count/{jobId}")]
        public async Task<int> GetJobResumesCountAsync(int jobId)
        {
         
            var resumes = await _jobResumesRepository.GetSelectedResumes(jobId);

            return resumes.Count;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys + "," + Roles.Darbuotojas)]
        [Route("specific/{id}")]
        public async Task<IEnumerable<JobResumes>> GetJobResumesAppliedDto(int id)
        {
            var jobResumes = await _jobResumesRepository.GetSelectedJobResumes(id);
            return jobResumes.Select(x => new JobResumes
            {
                Id = x.Id,
                Resume = x.Resume,
                Job = x.Job,
                Reviewed = x.Reviewed,
                CreationDate = x.CreationDate
            });
        }

        //APPLY
        [HttpPost]
        [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys + "," + Roles.Darbuotojas)]
        public async Task<ActionResult<JobResumesDto>> Create(CreateJobResumesDto createJobResumesDto)
        {
            var job = await _jobsRepository.GetJobAsync(createJobResumesDto.JobId);
            var resume = await _resumesRepository.GetResumeAsync(createJobResumesDto.ResumeId);
            var jobresumes = new JobResumes
            {
                Job = job,
                Resume = resume,
                CreationDate = DateTime.Today.Date,
                Reviewed = 0
            };

            await _jobResumesRepository.CreateJobResumesAsync(jobresumes);

            return Created("", new JobResumesDto(job.Id));
        }


        [HttpPut]
        [Authorize(Roles = Roles.Administratorius + "," + Roles.Darbdavys + "," + Roles.Darbuotojas)]
        [Route("{id}")]
        public async Task<ActionResult<JobResumes>> UpdateReviewCount(int id, UpdateJobResumeDto updateJobResumeDto)
        {
            var jobResumes = await _jobResumesRepository.GetJobAsync(id);
            if (jobResumes == null)
                return NotFound("Įvyko klaida.");

            jobResumes.Reviewed += updateJobResumeDto.reviewed;

            await _jobResumesRepository.UpdateJobResumesAsync(jobResumes);

            return Ok("Success!");
        }
    }
}
