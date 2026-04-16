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
        Task<int?> AddNewProductAsync(CreateProductDTO product);
        Task<ReadProductDTO?> GetProductByIDAsync(int id);
        Task<List<ReadProductDTO>> GetAllProductsAsync();
        Task<bool> UpdateProductAsync(int ID, UpdateProductDTO product);
        Task<bool> DeleteProductByIDAsync(int id);
        Task<bool> DoesProductExistAsync(int id);
        Task<bool> IsProductAvailableAsync(int id);
        Task<List<int>> ValidateProducts(List<int> productIds);
    }
}
