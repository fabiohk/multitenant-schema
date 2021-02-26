using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TenantController> _logger;
        private readonly DbContextLocator _dbContextLocator;
        public TenantController(ILogger<TenantController> logger, DbContextLocator contextLocator)
        {
            _logger = logger;
            _dbContextLocator = contextLocator;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Tenant tenant)
        {
            _logger.LogInformation("Creating a new tenant...");
            using (var dbContext = _dbContextLocator.GetInstance(tenant.TenantName))
            {
                await dbContext.Database.MigrateAsync();
            }

            return NoContent();
        }
    }
}
