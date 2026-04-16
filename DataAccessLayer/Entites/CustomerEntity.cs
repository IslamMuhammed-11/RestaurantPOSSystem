using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class CustomerEntity
    {
        public int CustomerID { get; set; }
        public required string Name { get; set; }
        public string? Phone { get; set; }
    }
}
