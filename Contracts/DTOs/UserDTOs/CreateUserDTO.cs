using Contracts.DTOs.PersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        public required CreatePersonDTO createPersonDTO { get; set; }

        private int _ID;
        public int UserID { get { return _ID; } }
        public int PersonID { get; set; }
        public required int RoleID { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required bool IsActive { get; set; } = true;


        public  bool IsValid() => 
            createPersonDTO.IsValid() &&
            RoleID > 0 &&
            !string.IsNullOrEmpty(UserName) &&
            !string.IsNullOrEmpty(Password);

       public void SetUserID(int id) => _ID = id;
    }
}
