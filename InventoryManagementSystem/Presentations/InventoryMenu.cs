using InventoryManagementSystem.Controllers;
using InventoryManagementSystem.Data;
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

namespace InventoryManagementSystem.Presentations
{
    public class InventoryMenu
    {
        private readonly ProductController _productController;
        private readonly SupplierController _supplierController;
        private readonly TransactionController _transactionController;
        private readonly InventoryController _inventoryController;

        public InventoryMenu(ProductController productController,SupplierController supplierController,TransactionController transactionController,InventoryController inventoryController)
        {
            _productController = productController;
            _supplierController = supplierController;
            _transactionController = transactionController;
            _inventoryController = inventoryController;
            _inventoryController = inventoryController;
        }

        public static void Show()
        {
            Console.WriteLine("***** Welcome to Inventory Management System App *****");
            
            var context= new InventoryContext();
            var menu =new InventoryMenu(
                new ProductController(new ProductRepository(context)),
                new SupplierController(new SupplierRepository(context)),
                new TransactionController(new TransactionRepository(context),new ProductRepository(context)), 
                new InventoryController(new InventoryRepository(context))
            );

            while (true)
            {
                Console.WriteLine("\nMain menu ->");
                Console.WriteLine("1)Product management");
                Console.WriteLine("2)Supplier management");
                Console.WriteLine("3)Transaction management");
                Console.WriteLine("4)Generate report");
                Console.WriteLine("5)Exit");
                Console.Write("\nEnter your choice -> ");

                var choice=Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        menu.ManageProducts();
                        break;
                    case "2":
                        menu.ManageSuppliers();
                        break;
                    case "3":
                        menu.ManageTransactions();
                        break;
                    case "4":
                        menu.GenerateReport();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice and try again");
                        break;
                }
            }
        }

        private void ManageProducts()
        {
            while (true)
            {
                Console.WriteLine("\nProduct management ->");
                Console.WriteLine("1)Add product");
                Console.WriteLine("2)Update product");
                Console.WriteLine("3)Delete product");
                Console.WriteLine("4)View products Details");
                Console.WriteLine("5)View all products");
                Console.WriteLine("6)Go back to main menu");
                Console.Write("\nEnter your choice-> ");

                var choice =Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        UpdateProduct();
                        break;
                    case "3":
                        DeleteProduct();
                        break;
                    case "4":
                        ViewProductDetails();
                        break;
                    case "5":
                        ViewAllProducts();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice and try again");
                        break;
                }
            }
        }

        private void AddProduct()
        {
            try
            {
                Console.Write("Enter product name -> ");
                var name = Console.ReadLine();
                Console.Write("Enter description -> ");
                var description = Console.ReadLine();
                Console.Write("Enter quantity -> ");
                var quantity = int.Parse(Console.ReadLine());
                Console.Write("Enter price -> ");
                var price = decimal.Parse(Console.ReadLine());

                int? inventoryId = null;
                Console.Write("\nWould you like to enter inventory id or location name and\n type 'id' for inventory id or type location for location name -> ");
                var choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "id")
                {
                    Console.Write("Enter inventory id -> ");
                    inventoryId = int.Parse(Console.ReadLine());
                }
                else if (choice == "location")
                {
                    Console.Write("Enter location name -> ");
                    var locationName = Console.ReadLine();
                    var inventory = _productController.GetInventoryByLocationName(locationName);
                    inventoryId = inventory.InventoryId;  
                }
                else
                {
                    Console.WriteLine("\nInvalid choice and enter 'id' or 'location'");
                    return; 
                }

                var product = new Product
                {
                    Name = name,
                    Description = description,
                    Quantity = quantity,
                    Price = price,
                    InventoryId = inventoryId.Value  
                };

                _productController.AddProduct(product);
                Console.WriteLine("\nProduct added successfully");
            }
            catch (DuplicateEntryException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            //DbUpdateException is exception in entity framework that occur when database update operation fails
            //and typically happens in response to issues with saving changes to database such as constraint being violated issues
            //or error in database connection or issue with data integrity
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"\nError -> failed to add product check if inventory id or location name exists details -> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred -> {ex.Message}");
            }
        }
        private void UpdateProduct()
        {
            try
            {
                Console.Write("Enter product id to update -> ");
                var id = int.Parse(Console.ReadLine());

                var product = _productController.GetProductById(id);

                Console.Write("Enter new product name and dont leave blank -> ");
                var name = Console.ReadLine();
                product.Name = name;

                Console.Write("Enter new description and dont leave blank -> ");
                var description = Console.ReadLine();
                product.Description = description;

                _productController.UpdateProduct(product);
                Console.WriteLine("\nProduct updated successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError-> {ex.Message}");
            }
            catch (DuplicateEntryException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }

       
        private void DeleteProduct()
        {
            try
            {
                Console.Write("Enter product id to delete -> ");
                var id = int.Parse(Console.ReadLine());

                var product = _productController.GetProductById(id);
                bool deleteInventoryItems = false;
                if (product.Inventory != null && product.Quantity > 0)
                {
                    Console.Write("\nThis product has associated inventory items with stock, do you want to delete them as well?\n" +
                        "Press 'y' to delete inventory items or 'n' to keep them -> ");
                    var confirmation = Console.ReadLine();

                    deleteInventoryItems = confirmation?.ToLower() == "y";
                }
                _productController.DeleteProduct(id,deleteInventoryItems);
                Console.WriteLine("\nProduct deleted successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (InsufficientStockException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred -> {ex.Message}");
            }
        }

        private void ViewProductDetails()
        {
            try
            {
                Console.Write("Enter product id or name -> ");
                var input = Console.ReadLine();

                Product product = int.TryParse(input,out int id) ? _productController.GetProductById(id)
                    : _productController.GetAllProducts().Find(p=> p.Name.Equals(input,StringComparison.OrdinalIgnoreCase));

                Console.WriteLine(product != null
                    ? $"\nId -> {product.ProductId}, Name -> {product.Name}, Description ->{product.Description} , Quantity -> {product.Quantity}, Price -> {product.Price}"
                    : "product not found");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }
        private void ViewAllProducts()
        {
            try
            {
                var products = _productController.GetAllProducts();
                foreach (var product in products)
                {
                    Console.WriteLine($"\nId -> {product.ProductId}, Name -> {product.Name}, Description ->{product.Description} , Quantity -> {product.Quantity}, Price -> {product.Price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }

        private void ManageSuppliers()
        {
            while (true)
            {
                Console.WriteLine("\nSupplier management ->");
                Console.WriteLine("1)Add supplier");
                Console.WriteLine("2)Update supplier");
                Console.WriteLine("3)Delete supplier");
                Console.WriteLine("4)View supplier's details");
                Console.WriteLine("5)View all suppliers");
                Console.WriteLine("6)Go back to main menu");
                Console.Write("\nEnter your choice -> ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddSupplier();
                        break;
                    case "2":
                        UpdateSupplier();
                        break;
                    case "3":
                        DeleteSupplier();
                        break;
                    case "4":
                        ViewSupplierDetails();
                        break;
                    case "5":
                        ViewAllSuppliers();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice and try again");
                        break;
                }
            }
        }
        private void AddSupplier()
        {
            try
            {
                Console.Write("Enter supplier name -> ");
                var name = Console.ReadLine();
                Console.Write("Enter contact information can give email or phone information -> ");
                var contactInfo = Console.ReadLine();

                int? inventoryId = null;
                Console.Write("\nWould you like to enter inventory id or location name and type 'id' for inventory id\n or type 'location' for location name -> ");

                //?.Trim()->check if input is not null before calling .Trim() which removes any leading or whitespace
                //.ToLower()->convert trimmed string to lowercase for consistent comparison ignoring case differences
                var choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "id")
                {
                    Console.Write("Enter inventory id -> ");
                    inventoryId = int.Parse(Console.ReadLine());
                }
                else if (choice == "location")
                {
                    Console.Write("Enter location name -> ");
                    var locationName = Console.ReadLine();

                    var inventory = _supplierController.GetInventoryByLocationName(locationName);
                    inventoryId = inventory.InventoryId;
                }
                else
                {
                    Console.WriteLine("\nInvalid choice enter 'id' or 'location'");
                    return;
                }

                var supplier = new Supplier
                {
                    Name = name,
                    ContactInformation = contactInfo,
                    InventoryId = inventoryId.Value
                };

                _supplierController.AddSupplier(supplier);

                Console.WriteLine("\nSupplier added successfully");
            }
            catch (DuplicateEntryException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }
        private void UpdateSupplier()
        {
            try
            {
                Console.Write("Enter supplier id to update -> ");
                var id = int.Parse(Console.ReadLine());

                var supplier = _supplierController.GetSupplierById(id);

                Console.Write("Enter new supplier name and dont leave blank -> ");
                var name = Console.ReadLine();

                supplier.Name = name;
                Console.Write("Enter new contact information and dont leave blank -> ");
                var contactInfo = Console.ReadLine();
                supplier.ContactInformation=contactInfo;

                _supplierController.UpdateSupplier(supplier);

                Console.WriteLine("\nSupplier updated successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError-> {ex.Message}");
            }
            catch (DuplicateEntryException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }
        private void DeleteSupplier()
        {
            try
            {
                Console.Write("Enter supplier id to delete -> ");
                var id = int.Parse(Console.ReadLine());

                var supplier = _supplierController.GetSupplierById(id); 
                bool deleteInventoryItems = false;
                if (supplier.Inventory != null)
                {
                    Console.Write("\nThis supplier has associated inventory items do you want to delete them as well\n" +
                        " if yes then press 'y' if no press 'n' -> ");
                    var confirmation = Console.ReadLine();

                    deleteInventoryItems = confirmation?.ToLower() == "y";
                }
                _supplierController.DeleteSupplier(id,deleteInventoryItems);
                Console.WriteLine("\nSupplier deleted successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError-> {ex.Message}");
            }
            catch (InsufficientStockException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }

        private void ViewSupplierDetails()
        {
            try
            {
                Console.Write("Enter supplier id or name -> ");
                var input = Console.ReadLine();

                //int.TryParse(input,out int id)-> tries to convert 'input' to integer
                //and 'out' let 'id' be set within tryparse method if parsing succeeds
                //if parsing succeeds we get supplier by id
                //and _supplierController.GetAllSuppliers().Find()->means if parsing fails retrieve all suppliers and search for name match
                //s => s.Name.Equals(input,StringComparison.OrdinalIgnoreCase)-> means
                //lambda expression to check if supplier name matche input ignoring case differences
                Supplier supplier = int.TryParse(input,out int id) ? _supplierController.GetSupplierById(id)
                    : _supplierController.GetAllSuppliers().Find(s=> s.Name.Equals(input,StringComparison.OrdinalIgnoreCase));

                Console.WriteLine(supplier != null ? $"Id -> {supplier.SupplierId}, Name -> {supplier.Name}, Contact information -> {supplier.ContactInformation}"
                    : "supplier not found");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }

        private void ViewAllSuppliers()
        {
            try
            {
                var suppliers = _supplierController.GetAllSuppliers();
                foreach (var supplier in suppliers)
                {
                    Console.WriteLine($"Id -> {supplier.SupplierId}, Name -> {supplier.Name}, Contact information -> {supplier.ContactInformation}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred -> {ex.Message}");
            }
        }
        private void ManageTransactions()
        {
            while (true)
            {
                Console.WriteLine("\nTransaction management ->");
                Console.WriteLine("1)Add stock");
                Console.WriteLine("2)Remove stock");
                Console.WriteLine("3)View transaction history");
                Console.WriteLine("4)Go back to main menu");
                Console.Write("\nEnter your choice -> ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddStock();
                        break;
                    case "2":
                        RemoveStock();
                        break;
                    case "3":
                        ViewTransactionHistory();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice try again");
                        break;
                }
            }
        }

        private void AddStock()
        {
            try
            {
                Console.Write("Enter product id -> ");
                var productId = int.Parse(Console.ReadLine());
                Console.Write("Enter quantity to add -> ");
                var quantity = int.Parse(Console.ReadLine());

                _transactionController.AddStock(productId,quantity);
                Console.WriteLine("\nStock added successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError-> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred -> {ex.Message}");
            }
        }

        private void RemoveStock()
        {
            try
            {
                Console.Write("Enter product id -> ");
                var productId = int.Parse(Console.ReadLine());
                Console.Write("Enter quantity to remove -> ");
                var quantity = int.Parse(Console.ReadLine());

                _transactionController.RemoveStock(productId,quantity);
                Console.WriteLine("\nStock removed successfully");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (InsufficientStockException ex)
            {
                Console.WriteLine($"\nError -> {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred-> {ex.Message}");
            }
        }

        private void ViewTransactionHistory()
        {
            try
            {
                var transactions = _transactionController.GetAllTransactions();
                foreach (var transaction in transactions)
                {
                    Console.WriteLine($"\nTransaction id -> {transaction.TransactionId}, Product id -> {transaction.ProductId}, Product name -> {transaction.Product.Name}, Quantity -> {transaction.Quantity}, Date -> {transaction.Date}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred -> {ex.Message}");
            }
        }

        private void ViewAllInventory()
        {
            var inventories = _inventoryController.GetAllInventoriesWithDetails(); 
            if (inventories == null || !inventories.Any())
            {
                Console.WriteLine("\nNo inventory records available");
                return;
            }

            Console.WriteLine("Inventory Id | Location | Suppliers | Products");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            foreach (var inventory in inventories)
            {
                Console.WriteLine($"Inventory id -> {inventory.InventoryId} | location -> {inventory.Location}");
                if (inventory.Suppliers != null && inventory.Suppliers.Any())
                {
                    Console.WriteLine("  Suppliers ->");
                    foreach (var supplier in inventory.Suppliers)
                    {
                        Console.WriteLine($"    - {supplier.Name} (Id -> {supplier.SupplierId})");
                    }
                }
                else
                {
                    Console.WriteLine("  Suppliers -> none");
                }

                if (inventory.Products != null && inventory.Products.Any())
                {
                    Console.WriteLine("  Products ->");
                    foreach (var product in inventory.Products)
                    {
                        Console.WriteLine($"    - {product.Name} (Id -> {product.ProductId}) | Quantity -> {product.Quantity}");
                    }
                }
                else
                {
                    Console.WriteLine("  Products -> none");
                }

                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            }
        }
        private void GenerateReport()
        {
            try
            {
                Console.WriteLine("\nInventory report =>");

                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Products ->");
                ViewAllProducts();
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("\nSuppliers ->");
                ViewAllSuppliers();
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("\nTransactions ->");
                ViewTransactionHistory();
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("\nInventory ->");
                ViewAllInventory(); 

                Console.WriteLine("Report generated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error occurred while generating report -> {ex.Message}");
            }
        }
    }
}