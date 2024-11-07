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
    public class SupplierRepository:ISupplierRepository
    {
        private readonly InventoryContext _context;
        public SupplierRepository(InventoryContext context)
        {
            _context = context;
        }

        //add new supplier to database
        public void Add(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);//add new supplier to Suppliers dbSet which corresponds to Suppliers table in database
            _context.SaveChanges(); //save changes to database which means new Supplier is actually inserted
        }
        
        public void Update(Supplier supplier)
        {
            var existingSupplier = _context.Suppliers.Find(supplier.SupplierId);

            //if Supplier exist in database proceed with updating it
            if (existingSupplier != null)
            {
                //update each field if new value is provided otherwise keep the old value
                existingSupplier.Name = string.IsNullOrEmpty(supplier.Name) ? existingSupplier.Name : supplier.Name;
                existingSupplier.ContactInformation = string.IsNullOrEmpty(supplier.ContactInformation) ? existingSupplier.ContactInformation : supplier.ContactInformation;

                //mark existing Supplier as modified so entity framework know that it has been updated
                _context.Entry(existingSupplier).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void Delete(Supplier supplier,bool deleteInventoryItems)
        {
            if (deleteInventoryItems && supplier.Inventory != null)
            {
                //remove associated inventory items if deleteinventoryItems is true
                supplier.Inventory=null;

                //mark Supplier as modified so entity framework will track changes
                _context.Entry(supplier).State = EntityState.Modified; //ensure change to inventory are tracked
            }
            _context.Suppliers.Remove(supplier);//remove Supplier from Suppliers dbSet
            _context.SaveChanges(); //save changes to database which deletes Supplier
        }

        //find and return Supplier by its supplierid including related inventory data
        public Supplier FindById(int supplierId)=> _context.Suppliers.Include(s=> s.Inventory).FirstOrDefault(s=> s.SupplierId == supplierId);

        //retrieve all Suppliers from database including their related inventory data
        public List<Supplier> GetAllSuppliers()=> _context.Suppliers.Include(s=> s.Inventory).ToList();

        //check if there is any Supplier with specified contact information
        public bool AnySupplierWithContact(string contactInformation)=> _context.Suppliers.Any(s=> s.ContactInformation == contactInformation);

        public Inventory GetInventoryByLocationName(string locationName)
        {
            return _context.Inventories.FirstOrDefault(inventory=> inventory.Location.ToLower() == locationName.ToLower());
        }
    }
}