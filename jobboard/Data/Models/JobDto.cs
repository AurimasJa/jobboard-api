using jobboard.Auth;
using jobboard.Data.Entities;

namespace jobboard.Data.Models;

public record DisplayedJobDto(int Id, string Title, string CompanyName, string Address, string Email, DisplayCustomUserInfoDto User);
public record UpdateJobValidityDto(DateTime ValidityDate, bool IsHidden);

