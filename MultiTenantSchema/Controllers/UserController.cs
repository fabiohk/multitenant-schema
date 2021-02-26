using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTenantSchema.Models;
using MultiTenantSchema.Support;

namespace MultiTenantSchema.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbContextLocator _dbContextLocator;
        public UserController(ILogger<UserController> logger, DbContextLocator contextLocator)
        {
            _logger = logger;
            _dbContextLocator = contextLocator;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string schema)
        {
            _logger.LogInformation("Retrieving users from {schema}", schema);
            using (var dbContext = _dbContextLocator.GetInstance(schema))
            {
                return Ok(new { Users = await dbContext.Users.ToListAsync() });
            }
        }

        [HttpPost("{schema}")]
        public async Task<User> Post([FromBody] User newUser, [FromRoute] string schema)
        {
            _logger.LogInformation("Trying to create a new user ({username}) for {schema}", newUser.Username, schema);
            using (var dbContext = _dbContextLocator.GetInstance(schema))
            {
                dbContext.Add(newUser);
                await dbContext.SaveChangesAsync();
            }
            return newUser;
        }
    }
}
