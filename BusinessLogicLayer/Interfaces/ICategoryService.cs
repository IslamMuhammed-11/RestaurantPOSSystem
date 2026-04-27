using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.CategoryDTOs;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICategoryService
    {
        Task<int?> AddNewCategoryAsync(CreateCategoryRequest category);
        Task<bool> UpdateCategoryAsync(int ID, UpdateCategoryRequest category);
        Task<CategoryResponse?> GetCategoryByIDAsync(int id);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
        Task<bool> DeleteCategoryByIDAsync(int id);
        Task<bool> DoesCategoryExistsAsync(int id);
    }
}
