using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entites;
namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepo
    {
        Task<CategoryEntity?> GetCategoryByIDAsync(int id);
        Task<List<CategoryEntity>> GetAllCategoriesAsync();
        Task<int?> CreateCategoryAsync(CategoryEntity category);
        Task<bool> UpdateCategoryAsync(CategoryEntity category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> DoesCategoryExistAsync(int id);
    }
}
