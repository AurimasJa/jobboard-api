using jobboard.Auth;
using jobboard.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace jobboard.Data.Repositories
{

    public interface ICompaniesResumesRepository
    {
        Task CreateViewedByCompanyAsync(CompaniesResume companiesResume);
        Task<IReadOnlyList<CompaniesResume>> GetCompaniesResumesAsync(int resumeId);
        Task<IReadOnlyList<JobBoardUser>> GetReviewedResumesAsync(int resumeId);
    }

    public class CompaniesResumesRepository : ICompaniesResumesRepository
    {
        private readonly JobBoardDbContext _db;
        private readonly UserManager<JobBoardUser> _userManager;

        public CompaniesResumesRepository(JobBoardDbContext db, UserManager<JobBoardUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IReadOnlyList<JobBoardUser>> GetReviewedResumesAsync(int resumeId)
        {
            var local = await _db.CompaniesResumes
                .Include(e => e.Resume)
                .Include(e => e.Company)
                .Where(o => o.Resume.Id == resumeId)
            .ToListAsync();
            var companiesId = local.Select(o => o.Company.Id).ToHashSet();

            return await _userManager.Users.Where(o => companiesId.Contains(o.Id)).ToListAsync();
        }
        public async Task<IReadOnlyList<CompaniesResume>> GetCompaniesResumesAsync(int resumeId)
        {
            var local = await _db.CompaniesResumes
                .Include(e => e.Resume)
                .Include(e => e.Company)
                .Where(o => o.Resume.Id == resumeId)
                .ToListAsync();
            return local;
        }
        public async Task CreateViewedByCompanyAsync(CompaniesResume companiesResume)
        {
            _db.CompaniesResumes.Add(companiesResume);
            await _db.SaveChangesAsync();
        }
    }
}
