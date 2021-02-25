using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantSchema.Contexts;
using MultiTenantSchema.Models;

namespace MultiTenantSchema.EntityConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly string _schema;

        public UserEntityConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            if (!String.IsNullOrWhiteSpace(_schema))
                builder.ToTable(nameof(MultiTenantDbContext.Users), _schema);

            builder.Property(u => u.Id)
                .HasDefaultValueSql("(newid())");
        }
    }
}