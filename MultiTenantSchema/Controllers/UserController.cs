using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenantSchema.Contexts;
using MultiTenantSchema.Models;
using MultiTenantSchema.Support;

namespace MultiTenantSchema.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string schema)
        {
            _logger.LogInformation("Retrieving users from {schema}", schema);
            using (var dbContext = GetMultiTenantDbContext(schema))
            {
                return Ok(new { Users = await dbContext.Users.ToListAsync() });
            }
        }

        [HttpPost("{schema}")]
        public async Task<User> Post([FromBody] User newUser, [FromRoute] string schema)
        {
            _logger.LogInformation("Trying to create a new user ({username}) for {schema}", newUser.Username, schema);
            using (var dbContext = GetMultiTenantDbContext(schema))
            {
                dbContext.Add(newUser);
                await dbContext.SaveChangesAsync();
            }
            return newUser;
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
