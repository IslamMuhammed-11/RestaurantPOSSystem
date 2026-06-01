using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.ProductDTOs;
using Contracts.Enums;
using Contracts.ErrorHandling;
using DataAccessLayer.Interfaces;
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

        public async Task<Result<ProductResponse>> AddNewProductAsync(CreateProductRequest product)
        {
            if (product == null || !product.IsValid())
                return Result<ProductResponse>.Failure(new Error("Invalid product data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var doesExists = await _categoryService.DoesCategoryExistsAsync(product.CategoryID);

            if (!doesExists.IsSuccess || !doesExists.Value)
                return Result<ProductResponse>.Failure(new Error("Category not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            var entity = ProductMap.ToEntity(product);

            int? id = await _productRepo.CreateProductAsync(entity);

            if (!id.HasValue)
                return Result<ProductResponse>.Failure(new Error("Failed to create product.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new ProductResponse
            {
                ProductID = id.Value,
                Name = product.Name,
                IsAvailable = product.IsAvailable,
                Price = product.Price,
                CategoryID = product.CategoryID
            };

            return Result<ProductResponse>.Success(response);
        }

        public async Task<Result<ProductResponse>> GetProductByIDAsync(int id)
        {
            if (id < 0)
                return Result<ProductResponse>.Failure(new Error("Invalid product ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = await _productRepo.GetProductByIDAsync(id);

            if (entity == null)
                return Result<ProductResponse>.Failure(new Error("Product not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<ProductResponse>.Success(ProductMap.ToReadDTO(entity));
        }

        public async Task<Result<List<ProductResponse>>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            return Result<List<ProductResponse>>.Success(ProductMap.ToReadDTOList(products));
        }

        public async Task<Result<ProductResponse>> UpdateProductAsync(int ID, UpdateProductRequest product)
        {
            if (product == null || ID < 0)
                return Result<ProductResponse>.Failure(new Error("Invalid product data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _productRepo.GetProductByIDAsync(ID);
            if (existing == null)
                return Result<ProductResponse>.Failure(new Error("Product not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!product.CategoryID.HasValue)
                return Result<ProductResponse>.Failure(new Error("Category ID is required.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var doesExists = await _categoryService.DoesCategoryExistsAsync(product.CategoryID.Value);

            if (!doesExists.IsSuccess || !doesExists.Value)
                return Result<ProductResponse>.Failure(new Error("Category not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool ok = ProductMap.ToEntity(product, existing);
            if (!ok)
                return Result<ProductResponse>.Failure(new Error("Invalid product data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updated = await _productRepo.UpdateProductAsync(existing);

            if (!updated)
                return Result<ProductResponse>.Failure(new Error("Failed to update product.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new ProductResponse
            {
                ProductID = existing.ProductID,
                Name = existing.Name,
                IsAvailable = existing.IsAvailable,
                Price = existing.Price,
                CategoryID = existing.CategoryID
            };

            return Result<ProductResponse>.Success(response);
        }

        public async Task<Result<bool>> DeleteProductByIDAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("Invalid product ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _productRepo.GetProductByIDAsync(id);
            if (existing == null)
                return Result<bool>.Failure(new Error("Product not found.", ErrorCodes.enErrorCodes.NOT_FOUND));
            bool deleted = await _productRepo.DeleteProductAsync(id);

            return Result<bool>.Success(deleted);
        }

        public async Task<Result<bool>> DoesProductExistAsync(int id)
        {
            var existing = await _productRepo.DoesProductExistAsync(id);
            return Result<bool>.Success(existing);
        }

        public async Task<Result<bool>> IsProductAvailableAsync(int id)
        {
            var available = await _productRepo.IsProductAvailableAsync(id);
            return Result<bool>.Success(available);
        }

        public async Task<List<int>> ValidateProducts(List<int> productIds)
        {
            var table = new DataTable();

            table.Columns.Add("ProductID", typeof(int));

            foreach (var id in productIds)
            {
                table.Rows.Add(id);
            }

            var invalidProductIDs = await _productRepo.ValidateProducts(table);

            return invalidProductIDs;
        }
    }
}