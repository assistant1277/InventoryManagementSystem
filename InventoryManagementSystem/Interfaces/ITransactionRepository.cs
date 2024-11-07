using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Interfaces
{
    public interface ITransactionRepository
    {
        void Add(Transaction transaction);
        List<Transaction> GetAllTransactions();
        List<Transaction> GetTransactionsByProductId(int productId);
    }
}
