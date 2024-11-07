using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Interfaces;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class ProductController
    {   
        private readonly IProductRepository _repository;
        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        public List<Product> GetAllProducts()=> _repository.GetAllProducts();
        public Product GetProductById(int productId)
        {
            var product = _repository.FindById(productId);
            if (product == null)
            {
                throw new ItemNotFoundException("\nProduct not found");
            }
            return product;
        }

        public void AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0 || product.Quantity < 0)
            {
                throw new ItemNotFoundException("\nInvalid product data and ensure product name is not empty and price is greater than zero and quantity is non negative");
            }

            if (_repository.AnyProductWithName(product.Name))
            {
                throw new DuplicateEntryException("\nProduct with the same name already exists");
            }
            _repository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0 || product.Quantity < 0)
            {
                throw new ItemNotFoundException("\nInvalid product data and ensure product name is not empty and price is greater than zero and quantity is non negative");
            }

            var existingProduct = _repository.FindById(product.ProductId);
            if (existingProduct == null)
            {
                throw new ItemNotFoundException("\nProduct not found for update");
            }
            
            if (_repository.AnyProductWithName(product.Name))
            {
                throw new DuplicateEntryException("\nProduct with this name already exists\nbut then also updating your given data");
            }
            _repository.Update(product);
        }
        public void DeleteProduct(int productId,bool deleteInventoryItems)
        {
            var product = _repository.FindById(productId);

            if (product == null)
            {
                throw new ItemNotFoundException("\nProduct not found for deletion");
            }

            if (product.Quantity > 0 && !deleteInventoryItems)
            {
                throw new InsufficientStockException("\nProduct are not deleted as per your decision and also it has non zero stock");
            }
            _repository.Delete(product, deleteInventoryItems);
        }

        public Inventory GetInventoryByLocationName(string locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                throw new ItemNotFoundException("\nLocation name cannot be empty");
            }

            var inventory = _repository.GetInventoryByLocationName(locationName);
            if (inventory == null)
            {
                throw new ItemNotFoundException("\nInventory not found at specified location");
            }
            return inventory;
        }
    }
}
