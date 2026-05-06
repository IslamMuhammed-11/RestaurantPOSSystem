using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.ProductDTOs;
using Contracts.Result;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> AddNewProductAsync(CreateProductRequest product);

        Task<Result<ProductResponse>> GetProductByIDAsync(int id);

        Task<Result<List<ProductResponse>>> GetAllProductsAsync();

        Task<Result<ProductResponse>> UpdateProductAsync(int ID, UpdateProductRequest product);

        Task<Result<bool>> DeleteProductByIDAsync(int id);

        Task<Result<bool>> DoesProductExistAsync(int id);

        Task<Result<bool>> IsProductAvailableAsync(int id);

        Task<List<int>> ValidateProducts(List<int> productIds);
    }
}