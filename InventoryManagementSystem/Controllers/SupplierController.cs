using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Interfaces;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class SupplierController
    {
        private readonly ISupplierRepository _repository;
        public SupplierController(ISupplierRepository repository)
        {
            _repository = repository;
        }
        public List<Supplier> GetAllSuppliers()=> _repository.GetAllSuppliers();
        public Supplier GetSupplierById(int supplierId)
        {
            var supplier = _repository.FindById(supplierId);
            if (supplier == null)
            {
                throw new ItemNotFoundException("\nSupplier not found");
            }
            return supplier;
        }
        public void AddSupplier(Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.Name) && string.IsNullOrEmpty(supplier.ContactInformation))
            {
                throw new ItemNotFoundException("\nInvalid supplier data and ensure supplier name or contact information is not empty");
            }

            if (_repository.AnySupplierWithContact(supplier.ContactInformation))
            {
                throw new DuplicateEntryException("\nSupplier with same contact information already exists");
            }
            _repository.Add(supplier);
        }
        public void UpdateSupplier(Supplier supplier)
        {
            var existingSupplier = _repository.FindById(supplier.SupplierId);
            if (existingSupplier == null)
            {
                throw new ItemNotFoundException("\nSupplier not found");
            }

            if (string.IsNullOrWhiteSpace(supplier.Name) && string.IsNullOrEmpty(supplier.ContactInformation))
            {
                throw new ItemNotFoundException("\nInvalid supplier data and ensure supplier name or contact information is not empty");
            }

            if (_repository.AnySupplierWithContact(supplier.ContactInformation))
            {
                throw new DuplicateEntryException("\nSupplier with this same contact information already exists\nbut then also updating your given data");
            }
            _repository.Update(supplier);
        }
        public void DeleteSupplier(int supplierId,bool deleteInventoryItems)
        {
            var supplier = _repository.FindById(supplierId);

            if (supplier == null)
            {
                throw new ItemNotFoundException("\nSupplier not found");
            }

            if (supplier.Inventory != null && !deleteInventoryItems)
            {
                throw new InsufficientStockException("\nSupplier are not deleted as per your decision and also it has non zero stock");
            }
            _repository.Delete(supplier, deleteInventoryItems);
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