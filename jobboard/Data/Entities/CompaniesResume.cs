using jobboard.Auth;

namespace jobboard.Data.Entities
{
    public class CompaniesResume
    {
        public int Id { get; set; }
        public JobBoardUser Company { get; set; }
        public Resume Resume { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
