using jobboard.Auth;
using jobboard.Data.Entities;

namespace jobboard.Data.Models;

public record JobDto(int Id, string City, string Description, string Title, double Salary, double SalaryUp, DateTime CreationDate, string Position, bool RemoteWork, bool IsHidden);
public record FullJobDto(int Id, CompanyJobDto Company, string Title, string Description, string Position, string PositionLevel,
    string CompanyOffers, List<Requirements> Requirements, string City, double SalaryUp,string Location, double Salary,
    string TotalWorkHours, bool RemoteWork, string Selection, DateTime CreationDate, DateTime ValidityDate, bool IsHidden);
public record DisplayedJobDto(int Id, string Title, string CompanyName, string Address, string Email, DisplayCustomUserInfoDto User);
public record UpdateJobValidityDto(DateTime ValidityDate, bool IsHidden);
public record UpdateJobDto(string? Title, string? Description, string? Position, string? PositionLevel, string? CompanyOffers, string? Location, string? City, double? Salary, double? SalaryUp, bool? RemoteWork, string? TotalWorkHours, string? Selection, List<Requirements> Requirements);

