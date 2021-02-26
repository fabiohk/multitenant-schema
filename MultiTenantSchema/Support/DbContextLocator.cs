using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using MultiTenantSchema.Contexts;

namespace MultiTenantSchema.Support
{
    public class DbContextLocator
    {
        private readonly IConfiguration _config;
        private ConcurrentDictionary<string, MultiTenantDbContext> _dbContextCache;

        public DbContextLocator(IConfiguration config)
        {
            _config = config;
            _dbContextCache = new ConcurrentDictionary<string, MultiTenantDbContext>();
        }

        public MultiTenantDbContext GetInstance(string schema)
        {
            return _dbContextCache.GetOrAdd(schema, newSchema => MultiTenantDbContextFactory.CreateDbContext(newSchema, _config));
        }
    }
}