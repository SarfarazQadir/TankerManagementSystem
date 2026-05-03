using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class PersonalKhata
    {
        [Key]
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public int PersonId { get; set; }
        public string Description { get; set; }
        public decimal AddAmount { get; set; }
        public decimal MinusAmount { get; set; }
        public decimal Balance { get; set; } // If zero show Nill
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
