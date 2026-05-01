using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.CategoryDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<int?> AddNewCategoryAsync(CreateCategoryRequest category)
        {
            if (category == null || !category.IsValid())
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);

            var entity = CategoryMap.ToEntity(category);

            int? id = await _categoryRepo.CreateCategoryAsync(entity);
            return id;
        }

        public async Task<CategoryResponse?> GetCategoryByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);

            var entity = await _categoryRepo.GetCategoryByIDAsync(id);
            if (entity == null)
                throw new BusinessException("Category Not Found", 60001, ActionResultEnum.ActionResult.NotFound);

            return CategoryMap.ToReadDTO(entity);
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return CategoryMap.ToReadDTOList(categories);
        }

        public async Task<bool> UpdateCategoryAsync(int ID, UpdateCategoryRequest category)
        {
            if (category == null || ID < 0)
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _categoryRepo.GetCategoryByIDAsync(ID);
            if (existing == null)
                throw new BusinessException("Category Not Found", 60001, ActionResultEnum.ActionResult.NotFound);

            bool ok = CategoryMap.ToEntity(category, existing);
            if (!ok)
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);

            bool updated = await _categoryRepo.UpdateCategoryAsync(existing);
            if (!updated)
                throw new BusinessException("Failed to update category.", 60002, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DeleteCategoryByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _categoryRepo.GetCategoryByIDAsync(id);
            if (existing == null)
                throw new BusinessException("Category Not Found", 60001, ActionResultEnum.ActionResult.NotFound);

            bool deleted = await _categoryRepo.DeleteCategoryAsync(id);
            if (!deleted)
                throw new BusinessException("Failed to delete category.", 60002, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DoesCategoryExistsAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid category data.", 60000, ActionResultEnum.ActionResult.InvalidData);
            return await _categoryRepo.DoesCategoryExistAsync(id);
        }
    }
}
