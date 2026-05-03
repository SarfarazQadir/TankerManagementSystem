using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
