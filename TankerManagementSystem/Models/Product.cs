using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }   // PMG, Diesel, Oil etc
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
