using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.CategoryDTOs;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryResponse>> AddNewCategoryAsync(CreateCategoryRequest category);

        Task<Result<CategoryResponse>> UpdateCategoryAsync(int ID, UpdateCategoryRequest category);

        Task<Result<CategoryResponse>> GetCategoryByIDAsync(int id);

        Task<Result<List<CategoryResponse>>> GetAllCategoriesAsync();

        Task<Result<bool>> DeleteCategoryByIDAsync(int id);

        Task<Result<bool>> DoesCategoryExistsAsync(int id);
    }
}