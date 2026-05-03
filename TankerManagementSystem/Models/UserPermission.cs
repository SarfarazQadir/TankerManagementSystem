using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ModuleId { get; set; }
        public AppModule Module { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
