using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jobboard.Controllers
{
    [ApiController]
    [Route("api/jobsresumes")]
    public class JobsResumesController : ControllerBase
    {
        public readonly IResumesRepository _resumesRepository;
        public readonly IJobsRepository _jobsRepository;
        public readonly IJobResumesRepository _jobResumesRepository;
        private readonly UserManager<JobBoardUser> _userManager;

        public JobsResumesController(IResumesRepository resumesRepository, UserManager<JobBoardUser> userManager, IJobsRepository jobsRepository, IJobResumesRepository jobResumesRepository)
        {
            _resumesRepository = resumesRepository;
            _userManager = userManager;
            _jobsRepository = jobsRepository;
            _jobResumesRepository = jobResumesRepository;
        }
        [HttpGet("{jobId}")]
        public async Task<IEnumerable<DisplayResumesDto>> GetJobResumesAsync(int jobId)
        {
            var resumes = await _jobResumesRepository.GetSelectedResumes(jobId);
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
                x.Summary,
                x.YearOfBirth,
                x.Position,
                x.UserId,
                x.IsHidden
            ));
        }
        //APPLY
        [HttpPost]
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
    }
}
