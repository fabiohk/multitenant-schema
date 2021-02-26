using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenantSchema.Contexts;
using MultiTenantSchema.Support;

namespace MultiTenantSchema.Controllers
{
    public class Tenant
    {
        [Required]
        public string TenantName { get; set; }
    }

    [Route("api/v1/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantController> _logger;
        public TenantController(IConfiguration configuration, ILogger<TenantController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Tenant tenant)
        {
            _logger.LogInformation("Creating a new tenant...");
            using (var dbContext = GetMultiTenantDbContext(tenant.TenantName))
            {
                await dbContext.Database.MigrateAsync();
            }

            return NoContent();
        }

        private MultiTenantDbContext GetMultiTenantDbContext(string schema)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MultiTenantDbContext>();
            var connectionString = _configuration.GetConnectionString(nameof(MultiTenantDbContext));

            optionsBuilder
                .UseSqlServer(
                    connectionString,
                    builder => builder.MigrationsHistoryTable("__EFMigrationsHistory", schema))
                .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
                .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>();
            return new MultiTenantDbContext(optionsBuilder.Options, schema);
        }
    }
}
