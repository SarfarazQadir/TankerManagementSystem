using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class TripExpense
    {
        [Key]
        public int Id { get; set; }
        public int TripLedgerId { get; set; }
        public TripLedger TripLedger { get; set; }
        public string ExpenseName { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
