using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.CategoryDTOs
{
    public class CreateCategoryDTO
    {
        private int _ID;
        public int CategoryID { get { return _ID; } }
        public required string Name { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
