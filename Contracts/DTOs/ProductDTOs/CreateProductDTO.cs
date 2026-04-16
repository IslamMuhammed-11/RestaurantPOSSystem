using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ProductDTOs
{
    public class CreateProductDTO
    {
        private int _ID;
        public int ProductID { get { return _ID; } }

        public required int CategoryID { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name) && Price >= 0;
    }
}
