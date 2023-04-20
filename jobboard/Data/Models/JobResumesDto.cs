namespace jobboard.Data.Models;

public record CreateJobResumesDto(int ResumeId, int JobId, DateTime CreationDate, int ReviewCount);

public record JobResumesAppliedToDto(int Id, DisplayedJobDto Job, DisplayCustomResumeDto Resume, DateTime CreationDate, int Reviewed);
public record JobResumesDto(int Id);