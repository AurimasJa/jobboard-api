﻿using jobboard.Auth;

namespace jobboard.Data.Entities
{
    public class Skills
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ResumeId { get; set; }
    }
}
