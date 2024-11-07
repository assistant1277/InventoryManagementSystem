using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public string Type { get; set; } //these is for "add" or "remove"
        public int Quantity { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}