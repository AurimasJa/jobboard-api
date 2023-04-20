using AutoMapper;
using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;

namespace jobboard.Data.Helpers
{
    public class JobBoardProfiles : Profile
    {
        public JobBoardProfiles()
        {
            CreateMap<CompaniesResume, CompaniesResumesDto>();
            CreateMap<JobResumes, JobResumesAppliedToDto>();
            CreateMap<Job, DisplayedJobDto>();
        }
    }
}
