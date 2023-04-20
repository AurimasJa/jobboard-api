using jobboard.Auth;
using jobboard.Data.Models;

namespace jobboard.Data.Entities
{
    public class JobResumes
    {
        public int Id { get; set; }
        public Resume Resume { get; set; }
        public Job Job { get; set; }
        public DateTime CreationDate { get; set; }
        public int Reviewed { get; set; }
    }
}
