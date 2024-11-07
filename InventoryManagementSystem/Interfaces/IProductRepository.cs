using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Interfaces
{
    public interface IProductRepository
    {
        void Add(Product product);
        void Update(Product product);
        Product FindById(int productId);
        Product FindByName(string name);
        List<Product> GetAllProducts();
        bool AnyProductWithName(string name);
        void Delete(Product product,bool deleteInventoryItems);
        Inventory GetInventoryByLocationName(string locationName);
    }
}
