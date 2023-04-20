using Microsoft.AspNetCore.Identity;

namespace jobboard.Auth
{
    public class JobBoardUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public string? AboutSection { get; set; }

        //public string Position { get; set; } //Pageidaujamas pareigų lygis, veliau pagalvosiu

        //i rankas, velia galbut prideti field, kad butu pasirinkti i rankas/neatskaicius mokesciu, per savaite/per menesi/per diena/ per valanda ir t.t.
        //public double SalaryFrom { get; set; }
        //public double SalaryTo { get; set; }

        //#################################### COMPANY ###########################################//

        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ContactPerson { get; set; } // NAME + " " + SURNAME
        public string? Site { get; set; }

    }
}
