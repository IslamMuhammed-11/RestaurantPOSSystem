using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.CategoryDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.ErrorHandling;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryService(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<Result<CategoryResponse>> AddNewCategoryAsync(CreateCategoryRequest category)
        {
            if (category == null || !category.IsValid())
                return Result<CategoryResponse>.Failure(new Error("Invalid category data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = CategoryMap.ToEntity(category);

            int? id = await _categoryRepo.CreateCategoryAsync(entity);

            if (!id.HasValue)
                return Result<CategoryResponse>.Failure(new Error("Failed to create category.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new CategoryResponse
            {
                CategoryID = id.Value,
                Name = category.Name
            };

            return Result<CategoryResponse>.Success(response);
        }

        public async Task<Result<CategoryResponse>> GetCategoryByIDAsync(int id)
        {
            if (id < 0)
                return Result<CategoryResponse>.Failure(new Error("ID Cannot be negative.", ErrorCodes.enErrorCodes.INVALID_ID));

            var entity = await _categoryRepo.GetCategoryByIDAsync(id);
            if (entity == null)
                return Result<CategoryResponse>.Failure(new Error("Category Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<CategoryResponse>.Success(CategoryMap.ToReadDTO(entity));
        }

        public async Task<Result<List<CategoryResponse>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return Result<List<CategoryResponse>>.Success(CategoryMap.ToReadDTOList(categories));
        }

        public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int ID, UpdateCategoryRequest category)
        {
            if (category == null || ID < 0)
                return Result<CategoryResponse>.Failure(new Error("Invalid category data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _categoryRepo.GetCategoryByIDAsync(ID);
            if (existing == null)
                return Result<CategoryResponse>.Failure(new Error("Category Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));
            bool ok = CategoryMap.ToEntity(category, existing);
            if (!ok)
                return Result<CategoryResponse>.Failure(new Error("Invalid category data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updated = await _categoryRepo.UpdateCategoryAsync(existing);

            if (!updated)
                return Result<CategoryResponse>.Failure(new Error("Failed to update category.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<CategoryResponse>.Success(CategoryMap.ToReadDTO(existing));
        }

        public async Task<Result<bool>> DeleteCategoryByIDAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("ID Cannot be negative.", ErrorCodes.enErrorCodes.INVALID_ID));

            var existing = await _categoryRepo.GetCategoryByIDAsync(id);
            if (existing == null)
                return Result<bool>.Failure(new Error("Category Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));
            bool deleted = await _categoryRepo.DeleteCategoryAsync(id);

            if (!deleted)
                return Result<bool>.Failure(new Error("Failed to delete category.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(deleted);
        }

        public async Task<Result<bool>> DoesCategoryExistsAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("ID Cannot be negative.", ErrorCodes.enErrorCodes.INVALID_ID));

            bool exists = await _categoryRepo.DoesCategoryExistAsync(id);

            return Result<bool>.Success(exists);
        }
    }
}