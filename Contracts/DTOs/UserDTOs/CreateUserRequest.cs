using Contracts.DTOs.PersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class CreateUserRequest
    {
        public CreatePersonRequest PersonData { get; set; }

        private int _ID;

        public int UserID
        { get { return _ID; } }

        public int PersonID { get; set; }
        public int RoleID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsValid() =>
            PersonData.IsValid() &&
            RoleID > 0 &&
            !string.IsNullOrEmpty(UserName) &&
            !string.IsNullOrEmpty(Password);

        public void SetUserID(int id) => _ID = id;
    }
}