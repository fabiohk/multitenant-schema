using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MultiTenantSchema.Contexts;

namespace MultiTenantSchema.Migrations
{
    public partial class Initial : Migration
    {
        private readonly IDbContextSchema _schema;

        public Initial(IDbContextSchema schema)
        {
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(_schema.SchemaName);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                },
                schema: _schema.SchemaName);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: _schema.SchemaName);
        }
    }
}
