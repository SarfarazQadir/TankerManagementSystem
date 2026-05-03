using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class Tanker
    {
        [Key]
        public int Id { get; set; }
        public string TankerNo { get; set; }
        public int OwnerId { get; set; }
        public TankerOwner Owner { get; set; }
        public string Model { get; set; }
        public string Capacity { get; set; }
        public decimal PreviousBalance { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
