using jobboard.Data.Entities;
using static jobboard.Data.Enums;

namespace jobboard.Data.Models;

public record DisplayCustomResumeDto(int Id, string FullName, DisplayCustomUserInfoDto User);
public record DisplayResumesDto(int id, string FullName, string Email, string PhoneNumber, string Address, string City, List<Education> Educations, List<Skills> Skills, List<Experience> Experiences, string Position, DateTime YearOfBirth, string Summary, string UserId, bool IsHidden, double? Salary);
public record DisplayCreatedResumeDto(int id);
public record CreateCompaniesReviewDto(string CompanyId, int ResumeId);
public record UpdateResumesVisibility(bool isHidden);
public record UpdateResumeDto(string? FullName, string? Email, string? PhoneNumber, string? Address, string? City, List<EducationDto?>? Educations, List<SkillsDto?>? Skills, List<ExperienceDto?>? Experiences, string? Position, DateTime? YearOfBirth, string? Summary, double? Salary);
public record EducationDto(string? School, Degree? Degree, DateTime? StartDate, DateTime EndDate, bool? IsCurrent);
public record ExperienceDto(string? Company, string? Position, DateTime? StartDate, DateTime EndDate, bool? IsCurrent);
public record SkillsDto(string? Name);

public record UpdateJobResumeDto(int reviewed);

public record CreateResumeCommand(string FullName, string Email, string Address,
    List<Education>? Education, List<Skills>? Skills, List<Experience>? Experience,
    string City, string PhoneNumber, string Summary, string Position, string References, double Salary);
