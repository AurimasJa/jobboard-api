using jobboard.Auth;
using jobboard.Data.Entities;

namespace jobboard.Data.Models;


public record CompaniesResumesDto(int Id, DisplayCustomCompanyInfoDto company, DisplayCustomResumeDto resume, DateTime ReviewDate);