using jobboard.Data.Entities;
using jobboard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace jobboard.Data.Repositories
{

    public interface IJobsRepository
    {
        Task CreateJobAsync(Job job);
        Task CreateJobRequirementsAsync(Requirements requirement);
        Task DeleteJobAsync(Job job);
        Task DeleteRequirementAsync(Requirements requirement);
        Task<Job?> GetJobAsync(int jobId); 
        Task<IReadOnlyList<Job>> GetJobsAsync();
        Task<IReadOnlyList<Job>> GetSimilarJobsAsync(string position, string city, int id);
        Task<IReadOnlyList<Job>> GetCompanyJobsAsync(string companyId);
        Task<IReadOnlyList<JobResumes>> GetJobResumesAsync(int resumeId);
        Task UpdateJobAsync(Job job);
        Task<Job> GetRequirementsAsync(int jobId);
        Task<IReadOnlyList<Job?>> GetLatestJobsAsync();
        void CheckAndUpdateValidityDate();
    }

    public class JobsRepository : IJobsRepository
    {
        private readonly JobBoardDbContext _db;

        public JobsRepository(JobBoardDbContext db)
        {
            _db = db;
        }

        public async Task<Job?> GetJobAsync(int jobId)
        {
            return await _db.Jobs.Include(e => e.Company).Include(e => e.Requirements).FirstOrDefaultAsync(x => x.Id == jobId);
        }

        //public async Task<JobResumes> GetJobResumeAsync(int jobId, int resumeId)
        //{
        //    return 
        //}


        public async Task<Job> GetRequirementsAsync(int jobId)
        {
            Job job = await _db.Jobs
                .Include(j => j.Requirements)
                .SingleOrDefaultAsync(j => j.Id == jobId);

            return job;
        }
        public async Task<IReadOnlyList<Job?>> GetLatestJobsAsync()
        {
            var jobs = await _db.Jobs
                      .Include(j => j.Requirements)
                      .Where(j => !j.IsHidden)
                      .OrderByDescending(j => j.CreationDate)
                      .Take(12)
                      .ToListAsync();

            return jobs;
        }

        public async Task<IReadOnlyList<Job>> GetJobsAsync()
        {
            return await _db.Jobs.ToListAsync();
        }
        public async Task<IReadOnlyList<Job>> GetSimilarJobsAsync(string position, string city, int id)
        {
            var similar = await _db.Jobs.Where(x => x.Position == position && x.City == city && x.IsHidden == false && x.Id != id).Take(3).ToListAsync();
            if(similar.Count != 3)
            {
                return await _db.Jobs.Where(x => x.City == city && x.IsHidden == false && x.Id != id).Take(3).ToListAsync();
            }
            return similar;
        }
        
        public async Task<IReadOnlyList<Job>> GetCompanyJobsAsync(string companyId)
        {
            return await _db.Jobs.Where(x => x.CompanyId == companyId).ToListAsync();
        }
        public void CheckAndUpdateValidityDate()
        {
            var currentDate = DateTime.Now;
            var itemsToUpdate = _db.Jobs.Where(x => x.ValidityDate < currentDate && x.IsHidden == false).ToList();

            foreach (var item in itemsToUpdate)
            {
                item.IsHidden = true;
                _db.SaveChanges();
            }
        }

        public async Task CreateJobAsync(Job job)
        {
            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();
        }
        public async Task CreateJobRequirementsAsync(Requirements requirement)
        {
            _db.Requirements.Add(requirement);
            await _db.SaveChangesAsync();
        }
        public async Task<IReadOnlyList<JobResumes>> GetJobResumesAsync(int resumeId)
        {
            var local = await _db.JobResumes
                .Include(e => e.Resume)
                .Include(e => e.Job)
                    .ThenInclude(e => e.Company)
                .Where(o => o.Resume.Id == resumeId).ToListAsync();
            return local;
        }
        //JobResume -> Users resume with Job.

        public async Task UpdateJobAsync(Job job)
        {
            _db.Jobs.Update(job);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteJobAsync(Job job)
        {
            _db.Jobs.Remove(job);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteRequirementAsync(Requirements requirement) 
        {
            _db.Requirements.Remove(requirement);
            await _db.SaveChangesAsync();
        }
    }
}
