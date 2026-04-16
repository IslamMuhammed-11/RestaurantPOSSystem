using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.CategoryDTOs
{
    public class ReadCategoryDTO
    {
        public required int CategoryID { get; set; }
        public required string Name { get; set; }
    }
}
