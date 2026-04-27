using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.ProductDTOs;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProductService
    {
        Task<int?> AddNewProductAsync(CreateProductRequest product);
        Task<ProductResponse?> GetProductByIDAsync(int id);
        Task<List<ProductResponse>> GetAllProductsAsync();
        Task<bool> UpdateProductAsync(int ID, UpdateProductRequest product);
        Task<bool> DeleteProductByIDAsync(int id);
        Task<bool> DoesProductExistAsync(int id);
        Task<bool> IsProductAvailableAsync(int id);
        Task<List<int>> ValidateProducts(List<int> productIds);
    }
}
