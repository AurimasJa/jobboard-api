using jobboard.Auth;
using jobboard.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace jobboard.Data.Models
{
    public class Job : ICompanyOwnedResource
    {
        public int Id { get; set; }
        public string Title { get; set; } // pavadinimas
        public string Description { get; set; } //darbo aprašymas
        public DateTime CreationDate { get; set; } //skelbimo kūrimo data
        public DateTime ValidityDate { get; set; } //skelbimo galiojimo laikas - 30d.
        public virtual List<Requirements> Requirements { get; set; }
        public string Position { get; set; }
        public string PositionLevel { get; set; }//pareigų lygis
        public string CompanyOffers { get; set; }//ką siūlo įmonė
        public string Location { get; set; } //vieta, kur dirbti
        public string City { get; set; }
        public double Salary { get; set; } //alga nuo
        public double SalaryUp { get; set; } //alga nuo
        public bool RemoteWork { get; set; } //remote work?? taip/ne
        public string TotalWorkHours { get; set; } //full work, half, pilnas/visas etatas
        public string Selection { get; set; } //internetu, telefonu, tiesiogiai
        [Required]
        public string CompanyId { get; set; }
        public JobBoardUser Company { get; set; }
        public bool IsHidden { get; set; }
    }
}
