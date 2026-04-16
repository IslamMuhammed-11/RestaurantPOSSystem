using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Entites;
namespace DataAccessLayer.Interfaces
{
    public interface IProductRepo
    {
        Task<ProductEntity?> GetProductByIDAsync(int id);
        Task<List<ProductEntity>> GetAllProductsAsync();
        Task<int?> CreateProductAsync(ProductEntity product);
        Task<bool> UpdateProductAsync(ProductEntity product);
        Task<bool> DeleteProductAsync(int id);
        Task <bool> DoesProductExistAsync(int id);
        Task<bool> IsProductAvailableAsync(int id);
        Task<List<int>> ValidateProducts(DataTable products);
    }
}
