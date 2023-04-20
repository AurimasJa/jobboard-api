using System.ComponentModel.DataAnnotations;

namespace jobboard.Auth;

public record RegisterUserDto([Required(ErrorMessage = "Vardas yra privalomas.")] string Name, [Required(ErrorMessage = "Pavardė yra privaloma.")] string Surname, [Required(ErrorMessage = "El. paštas yra privalomas")] string Email,[Required(ErrorMessage = "Slaptažodis yra privalomas.")] string Password, [Required(ErrorMessage = "Data yra privaloma")] DateTime DateOfBirth,  string? AboutSection);
public record LoginDto([Required(ErrorMessage = "El. paštas yra privalomas")] string Email, [Required(ErrorMessage = "Slaptažodis negali būti tuščias")] string Password);
public record CompanyDto(string Id, string CompanyName, string Address, string Email);
public record CompanyProfileDto(string CompanyName, string AboutSection, string CompanyCode, string PhoneNumber, string Address, string City, DateTime Created, string Site, string ContactPerson, string Email);

public record UserDto(string Id, string UserName, string Email);
public record RealUserDto(string Id, string Name, string Email, string Surname, DateTime DateOfBirth);
public record UpdateUserDto(string? Name, string? Email, string? Surname, DateTime? DateOfBirth, string? Password, string? NewPassword);
public record SuccessfulLoginDto(string AccessToken);
public record RegisterCompanyDto([Required(ErrorMessage = "Vardas yra privalomas.")] string Name, [Required(ErrorMessage = "Pavardė yra privaloma.")] string Surname, string PhoneNumber, [Required(ErrorMessage = "El. paštas yra privalomas")] string Email, [Required(ErrorMessage = "Slaptažodis yra privalomas")] string Password, [Required(ErrorMessage = "Šis laukas yra privalomas")] string AboutSection, [Required(ErrorMessage = "Adresas yra privalomas")] string Address, [Required(ErrorMessage = "Miestas yra privalomas")] string City, [Required(ErrorMessage = "Kompanijos kodas yra privalomas")] string CompanyCode, [Required(ErrorMessage = "Kompanijos pavadinimas yra privalomas")] string CompanyName, string? Site);


public class Validator : IValidatableObject
{
    public object ObjectToValidate { get; set; }

    public Validator(object objectToValidate)
    {
        ObjectToValidate = objectToValidate;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (ObjectToValidate is RegisterCompanyDto registerCompanyDto)
        {
            if (!registerCompanyDto.Email.Contains('@'))
                results.Add(new ValidationResult("El. pašto adresas neteisingas. (pavizdys@pastas.lt)", new[] { nameof(RegisterCompanyDto.Email) }));
        }
        else if (ObjectToValidate is RegisterUserDto registerUserDto)
        {
            if (!registerUserDto.Email.Contains('@'))
                results.Add(new ValidationResult("El. pašto adresas neteisingas. (pavizdys@pastas.lt)", new[] { nameof(RegisterUserDto.Email) }));
        }
        else if (ObjectToValidate is LoginDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Email))
                results.Add(new ValidationResult("El. pašto adresas neteisingas. (pavizdys@pastas.lt)", new[] { nameof(LoginDto.Email) }));
        }

        return results;
    }
}
