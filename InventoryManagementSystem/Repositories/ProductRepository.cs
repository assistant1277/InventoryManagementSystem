using InventoryManagementSystem.Data;
using InventoryManagementSystem.Interfaces;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repositories
{
    public class ProductRepository:IProductRepository
    {
        private readonly InventoryContext _context;
        public ProductRepository(InventoryContext context)
        {
            _context = context;
        }
        public void Add(Product product)
        {
            _context.Products.Add(product);//add new product to Products dbSet which is table in database
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            //finds existing product by it ProductId
            var existingProduct = _context.Products.Find(product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.Name = string.IsNullOrEmpty(product.Name) ? existingProduct.Name : product.Name;//update name if not empty
                existingProduct.Description = string.IsNullOrEmpty(product.Description) ? existingProduct.Description : product.Description;//update description if not empty
                existingProduct.Price = product.Price > 0 ? product.Price : existingProduct.Price; //update price if it is positive value
                existingProduct.Quantity = product.Quantity >= 0 ? product.Quantity : existingProduct.Quantity;

                _context.Entry(existingProduct).State = EntityState.Modified;//mark entity as modified so EF knows to save changes
                _context.SaveChanges(); //save changes back to database
            }
        }
        public void Delete(Product product,bool deleteInventoryItems)
        {
            if (deleteInventoryItems && product.Inventory != null)
            {
                //remove associated inventory items if deleteInventoryItems is true
                product.Inventory = null; //disassociate inventory from product

                //mark product as modified so EF will track changes
                _context.Entry(product).State = EntityState.Modified;
            }

            _context.Products.Remove(product); //remove product from Products dbSet
            _context.SaveChanges(); //save changes to the database
        }

        //find and return product by it productid including related inventory data
        public Product FindById(int productId)=> _context.Products.Include(p=> p.Inventory).FirstOrDefault(p=> p.ProductId == productId);

        public Product FindByName(string name)=> _context.Products.FirstOrDefault(p=> p.Name == name);

        //retrieve all products from database including related inventory data for each product
        public List<Product> GetAllProducts()=> _context.Products.Include(p=> p.Inventory).ToList();

        //check if any product exists in database with specified name
        public bool AnyProductWithName(string name)=> _context.Products.Any(p=> p.Name == name);

        //find and return inventory by it location name made case insensitive
        public Inventory GetInventoryByLocationName(string locationName)
        {
            return _context.Inventories.FirstOrDefault(inventory=> inventory.Location.ToLower() == locationName.ToLower());
        }
    }
}