using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using MultiTenantSchema.Contexts;
using MultiTenantSchema.Models;
using MultiTenantSchema.Support;

namespace MultiTenantSchema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<User>> Get([FromQuery] string schema = "people")
        {
            using (var dbContext = GetMultiTenantDbContext(schema))
            {
                return await dbContext.Users.ToListAsync();
            }
        }

        [HttpPost("{schema}")]
        public async Task<User> Post([FromBody] string username, [FromRoute] string schema)
        {
            var user = new User() { Username = username };
            using (var dbContext = GetMultiTenantDbContext(schema))
            {
                dbContext.Add(user);
                await dbContext.SaveChangesAsync();
            }
            return user;
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
