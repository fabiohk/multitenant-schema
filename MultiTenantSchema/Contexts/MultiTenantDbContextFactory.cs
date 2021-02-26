using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using MultiTenantSchema.Support;

namespace MultiTenantSchema.Contexts
{
    public class MultiTenantDbContextFactory : IDesignTimeDbContextFactory<MultiTenantDbContext>
    {
        public MultiTenantDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var schema = Environment.GetEnvironmentVariable("DB_SCHEMA");
            return CreateDbContext(schema, config);
        }

        public static MultiTenantDbContext CreateDbContext(string schema, IConfiguration config)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MultiTenantDbContext>();
            var connectionString = config.GetConnectionString(nameof(MultiTenantDbContext));

            optionsBuilder
                .UseSqlServer(
                    connectionString,
                    builder => builder.MigrationsHistoryTable("__EFMigrationsHistory", schema))
                .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>();
            return new MultiTenantDbContext(optionsBuilder.Options, schema);
        }
    }
}