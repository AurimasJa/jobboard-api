using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace jobboard.Data.Repositories
{
    public interface IResumesRepository
    {
        Task<IReadOnlyList<Resume>> GetResumesAsync();
        Task<IReadOnlyList<Resume>> GetUserResumesAsync(string userId);
        Task<Resume?> GetResumeAsync(int resumeId);
        Task<int> CreateResumeAsync(CreateResumeCommand createResumeDto, string userId, JobBoardUser user);
        Task DeleteResumeAsync(Resume resume);
        Task UpdateResumeAsync(Resume resume);
        Task DeleteResumeExperienceAsync(Experience experience);
        Task DeleteResumeEducationAsync(Education education);
        Task DeleteResumeSkillAsync(Skills skill);
    }

    public class ResumesRepository : IResumesRepository
    {
        private readonly UserManager<JobBoardUser> _userManager;
        private readonly JobBoardDbContext _db;

        public ResumesRepository(JobBoardDbContext db, UserManager<JobBoardUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        //get resumes by job id?
        public async Task<Resume?> GetResumeAsync(int resumeId)
        {
            //return await _db.Resumes.FirstOrDefaultAsync(x => x.Id == resumeId);
            Resume resume = await _db.Resumes
                .Include(j => j.Experience)
                .Include(j => j.Skills)
                .Include(j => j.Education)
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.Id == resumeId);

            return resume;
        }

        public async Task<IReadOnlyList<Resume>> GetResumesAsync()
        {
            return await _db.Resumes
                .Include(j => j.Experience)
                .Include(j => j.Skills)
                .Include(j => j.Education)
                .Include(j => j.User)
                .ToListAsync();
        }
        public async Task<IReadOnlyList<Resume>> GetUserResumesAsync(string userId)
        {
            return await _db.Resumes.Where(x => x.UserId == userId)
                .Include(j => j.Experience)
                .Include(j => j.Skills)
                .Include(j => j.Education)
                .Include(j => j.User)
                .ToListAsync();
        }



        //##################
        public async Task<int> CreateResumeAsync(CreateResumeCommand createResumeDto, string userId, JobBoardUser user)
        {
            var resume = new Resume
            {
                FullName = createResumeDto.FullName,
                Email = createResumeDto.Email,
                Address = createResumeDto.Address,
                City = createResumeDto.City,
                PhoneNumber = createResumeDto.PhoneNumber,
                Position = createResumeDto.Position,
                Summary = createResumeDto.Summary,
                References = createResumeDto.References,
                IsHidden = false,
                Salary = createResumeDto.Salary,
                YearOfBirth = user.DateOfBirth,
                UserId = userId
            };

            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();
            foreach (var expDto in createResumeDto.Experience)
            {
                var experience = new Experience
                {
                    ResumeId = resume.Id,
                    Company = expDto.Company,
                    StartDate = expDto.StartDate,
                    EndDate = expDto.EndDate,
                    IsCurrent = expDto.IsCurrent,
                    Position = expDto.Position,

                };

                _db.Experiences.Add(experience);
            }

            foreach (var eduDto in createResumeDto.Education)
            {
                var education = new Education
                {
                    School = eduDto.School,
                    Degree = eduDto.Degree,
                    IsCurrent = eduDto.IsCurrent,
                    ResumeId = resume.Id,
                    StartDate = eduDto.StartDate,
                    EndDate = eduDto.EndDate,
                };

                _db.Education.Add(education);
            }

            foreach (var skillDto in createResumeDto.Skills)
            {
                var skill = new Skills
                {
                    Name = skillDto.Name,
                    ResumeId = resume.Id
                };

                _db.Skills.Add(skill);
            }

            await _db.SaveChangesAsync();
            return resume.Id;
        }

        public async Task UpdateResumeAsync(Resume resume)
        {
            _db.Resumes.Update(resume);
            await _db.SaveChangesAsync();
        }


        public async Task DeleteResumeSkillAsync(Skills skill)
        {
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteResumeEducationAsync(Education education)
        {
            _db.Education.Remove(education);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteResumeExperienceAsync(Experience experience)
        {
            _db.Experiences.Remove(experience);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteResumeAsync(Resume resume)
        {
            _db.Resumes.Remove(resume);
            await _db.SaveChangesAsync();
        }
    }
}
