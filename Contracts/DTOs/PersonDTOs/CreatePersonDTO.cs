using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.PersonDTOs
{
    public class CreatePersonDTO
    {
 
        public required string FirstName { get; set; }
        public required string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public required DateTime DateOfBirth { get; set; }

        public required bool Gender { get; set; } // Male = 0 , Female = 1
        public required string Phone { get; set; }
        public string? Address { get; set; }

        public bool IsValid() =>
            !string.IsNullOrEmpty(FirstName) &&
            !string.IsNullOrEmpty(SecondName) &&
            DateOfBirth != default &&
            !string.IsNullOrEmpty(Phone);

    }
}
