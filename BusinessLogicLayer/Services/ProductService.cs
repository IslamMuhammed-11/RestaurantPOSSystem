using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.ProductDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly ICategoryService _categoryService;

        public ProductService(IProductRepo productRepo, ICategoryService categoryService)
        {
            _productRepo = productRepo;
            _categoryService = categoryService;
        }

        public async Task<int?> AddNewProductAsync(CreateProductRequest product)
        {
            if (product == null || !product.IsValid())
                throw new BusinessException("Invalid product data.", 70000, ActionResultEnum.ActionResult.InvalidData);

            if (!await _categoryService.DoesCategoryExistsAsync(product.CategoryID))
                throw new BusinessException("Category not found.", 60001, ActionResultEnum.ActionResult.NotFound);

            var entity = ProductMap.ToEntity(product);

            int? id = await _productRepo.CreateProductAsync(entity);
            if (!id.HasValue)
                throw new BusinessException("Failed to create product.", 70002, ActionResultEnum.ActionResult.DBError);

            return id;
        }

        public async Task<ProductResponse?> GetProductByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid product ID.", 70000, ActionResultEnum.ActionResult.InvalidData);

            var entity = await _productRepo.GetProductByIDAsync(id);
            if (entity == null)
                throw new BusinessException("Product not found.", 70001, ActionResultEnum.ActionResult.NotFound);

            return ProductMap.ToReadDTO(entity);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            return ProductMap.ToReadDTOList(products);
        }

        public async Task<bool> UpdateProductAsync(int ID, UpdateProductRequest product)
        {
            if (product == null || ID < 0)
                throw new BusinessException("Invalid product data.", 70000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _productRepo.GetProductByIDAsync(ID);
            if (existing == null)
                throw new BusinessException("Product not found.", 70001, ActionResultEnum.ActionResult.NotFound);

            if (product.CategoryID.HasValue)
            {
                if (!await _categoryService.DoesCategoryExistsAsync(product.CategoryID.Value))
                    throw new BusinessException("Category not found.", 60001, ActionResultEnum.ActionResult.NotFound);
            }

            bool ok = ProductMap.ToEntity(product, existing);
            if (!ok)
                throw new BusinessException("Invalid product data.", 70000, ActionResultEnum.ActionResult.InvalidData);

            bool updated = await _productRepo.UpdateProductAsync(existing);
            if (!updated)
                throw new BusinessException("Failed to update product.", 70002, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DeleteProductByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid product ID.", 70000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _productRepo.GetProductByIDAsync(id);
            if (existing == null)
                throw new BusinessException("Product not found.", 70001, ActionResultEnum.ActionResult.NotFound);

            bool deleted = await _productRepo.DeleteProductAsync(id);
            if (!deleted)
                throw new BusinessException("Failed to delete product.", 70002, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DoesProductExistAsync(int id)
        {
            try
            {
                var existing = await _productRepo.DoesProductExistAsync(id);
                return existing;
            }
            catch (BusinessException)
            {
                return false;
            }
        }

        public async Task<bool> IsProductAvailableAsync(int id)
        {
            try
            {
                var available = await _productRepo.IsProductAvailableAsync(id);
                return available;
            }
            catch (BusinessException)
            {
                return false;
            }
        }

        public async Task<List<int>> ValidateProducts(List<int> productIds)
        {
            var table = new DataTable();

            table.Columns.Add("ProductID", typeof(int));

            foreach (var id in productIds)
            {
                table.Rows.Add(id);
            }

            try
            {
                var invalidProductIDs = await _productRepo.ValidateProducts(table);
                return invalidProductIDs;
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message, 70002, ActionResultEnum.ActionResult.DBError);
            }
        }
    }
}