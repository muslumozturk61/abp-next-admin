﻿using LINGYUN.Abp.Data.DbMigrator;
using LINGYUN.Abp.MultiTenancy;
using LINGYUN.Abp.WebhooksManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace LY.MicroService.WebhooksManagement.EventBus.Handlers;

public class TenantSynchronizer :
        IDistributedEventHandler<CreateEventData>,
        ITransientDependency
{
    protected IDataSeeder DataSeeder { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IDbSchemaMigrator DbSchemaMigrator { get; }
    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected ILogger<TenantSynchronizer> Logger { get; }

    public TenantSynchronizer(
        IDataSeeder dataSeeder,
        ICurrentTenant currentTenant,
        IDbSchemaMigrator dbSchemaMigrator,
        IUnitOfWorkManager unitOfWorkManager,
        ILogger<TenantSynchronizer> logger)
    {
        DataSeeder = dataSeeder;
        CurrentTenant = currentTenant;
        DbSchemaMigrator = dbSchemaMigrator;
        UnitOfWorkManager = unitOfWorkManager;

        Logger = logger;
    }

    /// <summary>
    /// Seed data needs to be preset after tenant creation
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public async virtual Task HandleEventAsync(CreateEventData eventData)
    {
        using (var unitOfWork = UnitOfWorkManager.Begin())
        {
            using (CurrentTenant.Change(eventData.Id, eventData.Name))
            {
                Logger.LogInformation("Migrating the new tenant database with WebhooksManagement...");
                // Migrate tenant data
                await DbSchemaMigrator.MigrateAsync<WebhooksManagementDbContext>(
                    (connectionString, builder) =>
                    {
                        builder.UseSqlServer(connectionString);

                        return new WebhooksManagementDbContext(builder.Options);
                    });
                Logger.LogInformation("Migrated the new tenant database with WebhooksManagement...");

                await DataSeeder.SeedAsync(new DataSeedContext(eventData.Id));

                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
