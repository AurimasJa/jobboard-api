using jobboard.Auth;
using jobboard.Data.Entities;
using System.ComponentModel.DataAnnotations;
namespace jobboard.Data.Models;

public record JobDto(int Id, string City, string Description, string Title, double Salary, double SalaryUp, DateTime CreationDate, string Position, bool RemoteWork, string TotalWorkHours, bool IsHidden);
public record FullJobDto(int Id, CompanyJobDto Company, string Title, string Description, string Position, string PositionLevel,
    string CompanyOffers, List<Requirements> Requirements, string City, double SalaryUp,string Location, double Salary,
    string TotalWorkHours, bool RemoteWork, string Selection, DateTime CreationDate, DateTime ValidityDate, bool IsHidden);
public record DisplayedJobDto(int Id, string Title, string CompanyName, string Address, string Email, DisplayCustomUserInfoDto User);
public record UpdateJobValidityDto(DateTime ValidityDate, bool IsHidden);
public record UpdateJobDto(string? Title, string? Description, string? Position, string? PositionLevel, string? CompanyOffers, string? Location, string? City, double? Salary, double? SalaryUp, bool? RemoteWork, string? TotalWorkHours, string? Selection, List<Requirements> Requirements);

public record GetJobDto(int Id);



public record CreateJobCommand([CustomTitleValidation(ErrorMessage = "Per trumpas pavadinimas")] string Title, string City, string Description, List<Requirements> Requirements, string Position,
    string PositionLevel, string CompanyOffers, string Location,
    double Salary, /*List<Skills> Skills,*/ bool RemoteWork, string TotalWorkHours, string Selection,
    double SalaryUp);

public record CreateJobResumesCommand(int resumeId, int jobId, DateTime CreationDate, int ReviewCount);

public class CustomTitleValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string title = value.ToString();
            if (title.Length < 4)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}

public class ValidatorJob : IValidatableObject
{
    public object ObjectToValidate { get; set; }

    public ValidatorJob(object objectToValidate)
    {
        ObjectToValidate = objectToValidate;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (ObjectToValidate is CreateJobCommand createJobCommand)
        {
            if (createJobCommand.Title.Length < 4)
                results.Add(new ValidationResult("Pavadinimas per trumpas", new[] { nameof(CreateJobCommand.Title) }));
        }


        return results;
    }
}
