using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MultiTenantSchema.Contexts;

namespace MultiTenantSchema.Support
{
    public class DbSchemaAwareModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            return new
            {
                Type = context.GetType(),
                Schema = context is IDbContextSchema schema ? schema.SchemaName : null
            };
        }
    }
}