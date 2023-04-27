using jobboard.Auth;
using jobboard.Data.Entities;

namespace jobboard.Data.Models;

public record DisplayedJobDto(int Id, string Title, string CompanyName, string Address, string Email, DisplayCustomUserInfoDto User);
public record UpdateJobValidityDto(DateTime ValidityDate, bool IsHidden);
public record UpdateJobDto(string? Title, string? Description, string? Position, string? PositionLevel, string? CompanyOffers, string? Location, string? City, double? Salary, double? SalaryUp, bool? RemoteWork, string? TotalWorkHours, string? Selection, List<Requirements> Requirements);

