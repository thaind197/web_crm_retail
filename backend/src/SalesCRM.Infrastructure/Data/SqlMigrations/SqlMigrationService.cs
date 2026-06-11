using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using System.IO;

namespace SalesCRM.Infrastructure.Data.SqlMigrations;

public class SqlMigrationService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<SqlMigrationService> _logger;

    public SqlMigrationService(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ILogger<SqlMigrationService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("Starting SQL Migrations...");

        // 1. Ensure migration history table exists
        await EnsureHistoryTableExistsAsync();

        // 2. Run Schema migrations (Tạo bảng)
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Data", "SqlScripts", "Schema");
        if (Directory.Exists(schemaPath))
        {
            var schemaFiles = Directory.GetFiles(schemaPath, "*.sql").OrderBy(f => f).ToList();
            foreach (var file in schemaFiles)
            {
                var migrationId = Path.GetFileNameWithoutExtension(file);
                if (!await IsMigrationAppliedAsync(migrationId))
                {
                    _logger.LogInformation("Applying SQL Schema Migration: {MigrationId}", migrationId);
                    var sql = await File.ReadAllTextAsync(file);
                    
                    await ExecuteSqlScriptAsync(sql);
                    await RecordMigrationAppliedAsync(migrationId);
                    _logger.LogInformation("Successfully applied SQL Schema Migration: {MigrationId}", migrationId);
                }
            }
        }
        else
        {
            _logger.LogWarning("Schema scripts directory not found at: {Path}", schemaPath);
        }

        // 3. Run Seed migrations (Seeding data)
        var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "SqlScripts", "Seeds");
        if (Directory.Exists(seedPath))
        {
            var seedFiles = Directory.GetFiles(seedPath, "*.sql").OrderBy(f => f).ToList();
            foreach (var file in seedFiles)
            {
                var migrationId = Path.GetFileNameWithoutExtension(file);
                if (!await IsMigrationAppliedAsync(migrationId))
                {
                    _logger.LogInformation("Applying SQL Seed Migration: {MigrationId}", migrationId);
                    var sql = await File.ReadAllTextAsync(file);

                    // Replace password hash placeholders
                    sql = ReplacePasswordPlaceholders(sql);

                    await ExecuteSqlScriptAsync(sql);
                    await RecordMigrationAppliedAsync(migrationId);
                    _logger.LogInformation("Successfully applied SQL Seed Migration: {MigrationId}", migrationId);
                }
            }
        }
        else
        {
            _logger.LogWarning("Seed scripts directory not found at: {Path}", seedPath);
        }

        _logger.LogInformation("SQL Migrations completed successfully.");
    }

    private async Task EnsureHistoryTableExistsAsync()
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS ""__SqlMigrationsHistory"" (
                ""MigrationId"" character varying(150) NOT NULL,
                ""AppliedOn"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""PK___SqlMigrationsHistory"" PRIMARY KEY (""MigrationId"")
            );";
        
        await _context.Database.ExecuteSqlRawAsync(sql);
    }

    private async Task<bool> IsMigrationAppliedAsync(string migrationId)
    {
        var connection = _context.Database.GetDbConnection();
        var wasOpen = connection.State == System.Data.ConnectionState.Open;
        if (!wasOpen) await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT EXISTS (SELECT 1 FROM ""__SqlMigrationsHistory"" WHERE ""MigrationId"" = @MigrationId)";
            
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@MigrationId";
            parameter.Value = migrationId;
            command.Parameters.Add(parameter);

            var exists = await command.ExecuteScalarAsync();
            return exists is bool b && b;
        }
        finally
        {
            if (!wasOpen) await connection.CloseAsync();
        }
    }

    private async Task RecordMigrationAppliedAsync(string migrationId)
    {
        var sql = @"INSERT INTO ""__SqlMigrationsHistory"" (""MigrationId"") VALUES ({0})";
        await _context.Database.ExecuteSqlRawAsync(sql, migrationId);
    }

    private async Task ExecuteSqlScriptAsync(string sql)
    {
        await _context.Database.ExecuteSqlRawAsync(sql);
    }

    private string ReplacePasswordPlaceholders(string sql)
    {
        var adminUser = new ApplicationUser { UserName = "admin", FullName = "System Administrator" };
        var staffUser = new ApplicationUser { UserName = "staff", FullName = "Nhân viên Chi Nhánh 1" };

        var adminHash = _passwordHasher.HashPassword(adminUser, "Password123!");
        var staffHash = _passwordHasher.HashPassword(staffUser, "Password123!");

        return sql
            .Replace("{{ADMIN_PASSWORD_HASH}}", adminHash)
            .Replace("{{STAFF_PASSWORD_HASH}}", staffHash);
    }
}
