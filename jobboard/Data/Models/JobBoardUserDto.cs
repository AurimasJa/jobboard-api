using jobboard.Auth;
using jobboard.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace jobboard.Data.Models;
public record DisplayCustomCompanyInfoDto(string Id, string CompanyName, string Address, string Email);
public record DisplayCustomUserInfoDto(string Id, string FullName, string Email);


