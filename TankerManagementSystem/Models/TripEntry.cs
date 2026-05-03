using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class TripEntry
    {
        [Key]
        public int Id { get; set; }
        public DateTime LoadDate { get; set; }
        public int TankerId { get; set; }
        public Tanker TankerFk { get; set; }
        public decimal AdvanceCash { get; set; }
        public string? FromLocation { get; set; } = null;
        public string ToLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
