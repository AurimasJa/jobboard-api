
using jobboard.Auth;

public record GetJobCommand(int Id);

public record GetJobsCommand(int Id, string Title, string City, string Position, string Description, double SalaryFrom, double SalaryTo, DateTime CreationDate);