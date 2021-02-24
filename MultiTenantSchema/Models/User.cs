using System.ComponentModel.DataAnnotations;

namespace MultiTenantSchema.Models
{
    public class User : StandardEntity
    {
        [Required]
        public string Username { get; set; }
        public string FullName { get; set; }
    }
}