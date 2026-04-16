using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.PersonDTOs
{
    public class PersonResponseDTO
    {
        public required int PersonID { get; set; }
        public required string FirstName { get; set; }
        public required string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; } // Male = 0 , Female = 1
        public required string Phone { get; set; }
        public string? Address { get; set; }
    }
}
