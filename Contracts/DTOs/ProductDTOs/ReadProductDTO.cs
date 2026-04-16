using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ProductDTOs
{
    public class ReadProductDTO
    {
        public required int ProductID { get; set; }
        public required int CategoryID { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
    }
}
