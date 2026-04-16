using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.CustomerDTOs
{
    public class CreateCustomerDTO
    {

        private int _ID;
        public int CustomerID { get{ return _ID; } }
        public required string Name { get; set; }

        public string? Phone { get; set; }

        public void SetID(int ID)
            => _ID = ID;
    }
}
