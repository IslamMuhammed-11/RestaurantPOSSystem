using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ProductDTOs
{
    public class UpdateProductDTO
    {
        public int? CategoryID { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
