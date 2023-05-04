using jobboard.Data.Entities;
using jobboard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace jobboard.Data.Repositories
{
    public interface IJobResumesRepository
    {
        Task CreateJobResumesAsync(JobResumes jobResumes);
        Task<IReadOnlyList<Resume>> GetSelectedResumes(int jobId);
        Task<IReadOnlyList<JobResumes>> GetSelectedJobResumes(int jobId);
        Task<JobResumes?> GetJobAsync(int jobId);
        Task UpdateJobResumesAsync(JobResumes jobResumes);
        Task<IReadOnlyList<JobResumes>> GetJobResumesByResume(int resumeId);
        Task DeleteResumeSkillAsync(JobResumes jobResumes);
    }

    public class JobResumesRepository : IJobResumesRepository
    {
        private readonly JobBoardDbContext _db;

        public JobResumesRepository(JobBoardDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<JobResumes>> GetJobResumesByResume(int resumeId)
        {
            var local = await _db.JobResumes
                .Include(e => e.Resume)
                .Include(e => e.Job)
                .Where(o => o.Resume.Id == resumeId).ToListAsync();
            return local;
        }
        public async Task<IReadOnlyList<JobResumes>> GetSelectedJobResumes(int jobId)
        {
            var local = await _db.JobResumes
                .Include(e => e.Resume)
                .Include(e => e.Job)
                .Where(o => o.Job.Id == jobId).ToListAsync();
            return local;
        }
        public async Task<IReadOnlyList<Resume>> GetSelectedResumes(int jobId)
        {
            var local = await _db.JobResumes
                .Include(e => e.Resume)
                .Include(e => e.Job)
                .Where(o => o.Job.Id == jobId).ToListAsync();
            var resumesIds = local.Select(o => o.Resume.Id).ToHashSet();

            return await _db.Resumes.Where(j => resumesIds.Contains(j.Id))
                .Include(j => j.Experience)
                .Include(j => j.Skills)
                .Include(j => j.Education)
                .Include(j => j.User)
                .ToListAsync();
        }
        public async Task<JobResumes?> GetJobAsync(int jobId)
        {
            return await _db.JobResumes.Include(e => e.Resume).Include(e => e.Job).FirstOrDefaultAsync(x => x.Id == jobId);
        }

        public async Task CreateJobResumesAsync(JobResumes jobResumes)
        {
            _db.JobResumes.Add(jobResumes);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateJobResumesAsync(JobResumes jobResumes)
        {
            _db.JobResumes.Update(jobResumes);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteResumeSkillAsync(JobResumes jobResumes)
        {
            _db.JobResumes.Remove(jobResumes);
            await _db.SaveChangesAsync();
        }
    }
}
