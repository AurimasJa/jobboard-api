using jobboard.Data.Entities;

namespace jobboard.Data.Models;

public record DisplayCustomResumeDto(int Id, string FullName, DisplayCustomUserInfoDto User);
public record DisplayResumesDto(int id, string FullName, string Email, string PhoneNumber, string Address, string City, List<Education> Educations, List<Skills> Skills, List<Experience> Experiences, string Position, DateTime YearOfBirth, string Summary, string UserId, bool IsHidden);
public record DisplayCreatedResumeDto(int id);
public record CreateCompaniesReviewDto(string CompanyId, int ResumeId); 
public record UpdateJobResumeDto(int reviewed);

