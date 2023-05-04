using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jobboard.Controllers
{
    [ApiController]
    [Route("api/companiesresume")]
    public class CompaniesResumeController : ControllerBase
    {
        public readonly IResumesRepository _resumesRepository;
        public readonly IJobsRepository _jobsRepository;
        public readonly ICompaniesResumesRepository _companiesResumesRepository;
        private readonly UserManager<JobBoardUser> _userManager;

        public CompaniesResumeController(IResumesRepository resumesRepository, UserManager<JobBoardUser> userManager, IJobsRepository jobsRepository, ICompaniesResumesRepository companiesResumesRepository)
        {
            _resumesRepository = resumesRepository;
            _userManager = userManager;
            _jobsRepository = jobsRepository;
            _companiesResumesRepository = companiesResumesRepository;
        }

        [HttpGet]
        [Route("companies/{id}")]
        public async Task<IEnumerable<CompaniesResumesDto>> GetCompaniesViews(string id)
        {
            List<CompaniesResume> views = new List<CompaniesResume>();
            var resumes = await _resumesRepository.GetUserResumesAsync(id);
            foreach (var item in resumes)
            {
                var temp = await _companiesResumesRepository.GetCompaniesResumesAsync(item.Id);
                views.AddRange(temp);
            }
            var viewsByDate = views.OrderByDescending(j => j.ReviewDate);

            return viewsByDate.Select(x => new CompaniesResumesDto
            (
                x.Id,
                new DisplayCustomCompanyInfoDto
                (
                    x.Company.Id,
                    x.Company.CompanyName,
                    x.Company.Address,
                    x.Company.Email
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
               x.ReviewDate
            ));
        }
        [HttpPost]
        public async Task<ActionResult<CompaniesResumesDto>> Create(CreateCompaniesReviewDto createCompaniesReviewDto)
        {
            var company = await _userManager.FindByIdAsync(createCompaniesReviewDto.CompanyId);
            var resume = await _resumesRepository.GetResumeAsync(createCompaniesReviewDto.ResumeId);
            if (company == null)
                return NotFound($"Company {createCompaniesReviewDto.CompanyId} does not exist");
            if (resume == null)
                return NotFound($"Company {createCompaniesReviewDto.ResumeId} does not exist");
            var view = new CompaniesResume
            {
                Company = company,
                Resume = resume,
                ReviewDate = DateTime.Today
            };

            await _companiesResumesRepository.CreateViewedByCompanyAsync(view);
            var customCompany = new DisplayCustomCompanyInfoDto
            (
                company.Id,
                company.CompanyName,
                company.Address,
                company.Email
            );
            var customResume = new DisplayCustomResumeDto
            (
            resume.Id,
            resume.FullName,
                new DisplayCustomUserInfoDto
                (
                    resume.User.Id,
                    (resume.User.Name + " " + resume.User.Surname),
                    resume.User.Email
                )
            );
            return Created("", new CompaniesResumesDto(view.Id, customCompany, customResume, view.ReviewDate));

        }
    }
}
