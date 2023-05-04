using jobboard.Auth;

namespace jobboard.Data.Entities
{
    public class Experience
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public int ResumeId { get; set; }
    }
}
