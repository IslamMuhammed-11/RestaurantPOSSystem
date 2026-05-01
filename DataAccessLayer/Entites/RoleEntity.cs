using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class RoleEntity
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public short Premission { get; set; }

        public bool isValid() =>
            !string.IsNullOrEmpty(RoleName);
    }
}