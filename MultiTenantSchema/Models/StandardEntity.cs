using System;
using System.ComponentModel.DataAnnotations;

namespace MultiTenantSchema.Models
{
    public abstract class StandardEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}