
using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using System.ComponentModel.DataAnnotations;

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