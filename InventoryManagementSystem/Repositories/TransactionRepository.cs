using InventoryManagementSystem.Data;
using InventoryManagementSystem.Interfaces;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repositories
{
    public class TransactionRepository :ITransactionRepository
    { 
        private readonly InventoryContext _context;
        public TransactionRepository(InventoryContext context)
        {
            _context = context;
        }

        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);//add new transaction object to Transactions dbSet
            _context.SaveChanges();//commit changes to database which insert new transaction record into database
        }
        public List<Transaction> GetAllTransactions()=> _context.Transactions.Include(t=> t.Product).ToList();

        //includes related Product data for each transaction
        //and filters transactions to only those associated with specified productId
        //and converts result into list of transactions
        public List<Transaction> GetTransactionsByProductId(int productId)=> _context.Transactions.Include(t=> t.Product).Where(t=> t.ProductId == productId).ToList();
    }
}