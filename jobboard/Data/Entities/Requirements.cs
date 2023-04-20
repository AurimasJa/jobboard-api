using jobboard.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace jobboard.Data.Entities
{
    public class Requirements
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int JobId { get; set; }
    }
}
