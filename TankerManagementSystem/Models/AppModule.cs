using System.ComponentModel.DataAnnotations;

namespace TankerManagementSystem.Models
{
    public class AppModule
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
