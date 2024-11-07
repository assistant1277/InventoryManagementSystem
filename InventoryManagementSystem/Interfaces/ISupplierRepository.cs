using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Interfaces
{
    public interface ISupplierRepository
    {
        void Add(Supplier supplier);
        void Update(Supplier supplier);
        Supplier FindById(int supplierId);
        List<Supplier> GetAllSuppliers();
        bool AnySupplierWithContact(string contactInformation);
        Inventory GetInventoryByLocationName(string locationName);
        void Delete(Supplier supplier,bool deleteInventoryItems);
    }
}
