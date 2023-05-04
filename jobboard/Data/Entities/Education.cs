using jobboard.Auth;
using static jobboard.Data.Enums;

namespace jobboard.Data.Entities
{
    public class Education
    {
        public int Id { get; set; }
        public string School { get; set; }
        public Degree Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public int ResumeId { get; set; }
    }
}
