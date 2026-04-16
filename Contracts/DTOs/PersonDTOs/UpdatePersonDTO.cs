using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.PersonDTOs
{
    public class UpdatePersonDTO
    {
        [Required(ErrorMessage = "Person ID is Required")]
        public int PersonID {  get; set; }
        [Required(ErrorMessage = "First name is Required")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Second name is Required")]
        public required string SecondName { get; set; }
        public  string? ThirdName { get; set; }
        [Required(ErrorMessage = "Date Of Birth is Required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public bool Gender { get; set; } // Male = 0 , Female = 1

        [Required(ErrorMessage = "Phone is Required")]
        public required string Phone { get; set; }
        public string? Address { get; set; }
    }
}
