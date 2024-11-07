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
    public class TransactionController
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITransactionRepository _repository;

        public TransactionController(ITransactionRepository transactionRepository, IProductRepository productRepository)
        {
            _transactionRepository = transactionRepository;
            _productRepository = productRepository;
        }

        public void AddStock(int productId, int quantity)
        {
            var product = _productRepository.FindById(productId);

            if (product == null)
            {
                throw new ItemNotFoundException("\nProduct not found");
            }

            product.Quantity += quantity;
            _productRepository.Update(product); 

            var transaction = new Transaction
            {
                ProductId = productId,
                Quantity = quantity,
                Type = "Add",
                Date = DateTime.Now,
                InventoryId = product.InventoryId 
            };
            _transactionRepository.Add(transaction);
        }

        public void RemoveStock(int productId, int quantity)
        {
            var product = _productRepository.FindById(productId);
            if (product == null)
            {
                throw new ItemNotFoundException("\nProduct not found");
            }

            if (product.Quantity < quantity)
            {
                throw new InsufficientStockException("\nInsufficient stock available");
            }
            product.Quantity -= quantity;
            _productRepository.Update(product);  

            var transaction = new Transaction
            {
                ProductId = productId,
                Quantity = quantity,
                Type = "Remove",
                Date = DateTime.Now,
                InventoryId = product.InventoryId 
            };
            _transactionRepository.Add(transaction);
        }

        public List<Transaction> ViewTransactionHistory()
        {
            return _transactionRepository.GetAllTransactions();
        }

        public List<Transaction> GetAllTransactions()
        {
            return _transactionRepository.GetAllTransactions();
        }
    }
}