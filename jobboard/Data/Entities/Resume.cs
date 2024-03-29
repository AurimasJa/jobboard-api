﻿using jobboard.Auth;
using System.ComponentModel.DataAnnotations;

namespace jobboard.Data.Entities
{
    public class Resume : IUserOwnedResource
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Address { get;set; }
        public string City { get; set; }
        public DateTime YearOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public virtual List<Education?>? Education { get; set; } 
        public virtual List<Experience?>? Experience { get; set; } 
        public string Summary { get; set; }
        public string Position { get; set; }
        public virtual List<Skills?>? Skills { get; set; }
        public string? References { get; set; }
        [Required]
        public string UserId { get; set; }
        public JobBoardUser User { get; set; }
        public bool IsHidden { get; set; }
        public double Salary { get; set; }
    }


}
