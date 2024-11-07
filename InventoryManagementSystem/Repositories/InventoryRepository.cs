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
    internal class InventoryRepository:IInventoryRepository
    {
        private readonly InventoryContext _context;
        public InventoryRepository(InventoryContext context)
        {
            _context = context;
        }

        //_context.Inventories is entity framework dbSet representing "Inventories" table in database and
        //Include(i=> i.Suppliers) is used to load related suppliers data for each inventory and
        //Include(i=> i.Products) is used to load related products data for each inventory
        //ToList() execute query and retrieves all records as list
        public List<Inventory> GetAllInventories()
        {
            //ensures that for each inventory record related suppliers are loaded and
            //ensures that for each inventory record related products are loaded
            //and execute query and retrieve result as list of inventory objects
            return _context.Inventories.Include(i=> i.Suppliers).Include(i=> i.Products).ToList();
        }
    }
}