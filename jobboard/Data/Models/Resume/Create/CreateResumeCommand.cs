using jobboard.Data.Entities;

public record CreateResumeCommand(string FullName, string Email, string Address,
    List<Education>? Education, List<Skills>? Skills, List<Experience>? Experience,
    string City, string PhoneNumber, string Summary, string Position, string References);
