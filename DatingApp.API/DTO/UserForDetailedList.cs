using System;
using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp.API.DTO
{
    public class UserForDetailedList
    {
        public int Id { get; set; }
        public string Username { get; set; }     
        public string Gender { get; set; } 

        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoURL { get; set; }

        public ICollection<PhotoForDetailedDTO> Photos { get; set; }
    }
}