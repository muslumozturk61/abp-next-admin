﻿using DotNetCore.CAP;
using LINGYUN.Abp.AspNetCore.HttpOverrides;
using LINGYUN.Abp.AspNetCore.Mvc.Wrapper;
using LINGYUN.Abp.AuditLogging.Elasticsearch;
using LINGYUN.Abp.Authorization.OrganizationUnits;
using LINGYUN.Abp.Data.DbMigrator;
using LINGYUN.Abp.EventBus.CAP;
using LINGYUN.Abp.ExceptionHandling.Emailing;
using LINGYUN.Abp.Http.Client.Wrapper;
using LINGYUN.Abp.Localization.CultureMap;
using LINGYUN.Abp.LocalizationManagement;
using LINGYUN.Abp.LocalizationManagement.EntityFrameworkCore;
using LINGYUN.Abp.Saas.EntityFrameworkCore;
using LINGYUN.Abp.Serilog.Enrichers.Application;
using LINGYUN.Abp.Serilog.Enrichers.UniqueId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace LY.MicroService.LocalizationManagement
{
    [DependsOn(
        typeof(AbpSerilogEnrichersApplicationModule),
        typeof(AbpSerilogEnrichersUniqueIdModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAuditLoggingElasticsearchModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(AbpLocalizationManagementApplicationModule),
        typeof(AbpLocalizationManagementHttpApiModule),
        typeof(AbpLocalizationManagementEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule),
        typeof(AbpSaasEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpDataDbMigratorModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpAuthorizationOrganizationUnitsModule),
        typeof(AbpEmailingExceptionHandlingModule),
        typeof(AbpCAPEventBusModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAspNetCoreHttpOverridesModule),
        typeof(AbpLocalizationCultureMapModule),
        typeof(AbpHttpClientWrapperModule),
        typeof(AbpAspNetCoreMvcWrapperModule),
        typeof(AbpAutofacModule)
        )]
    public partial class LocalizationManagementHttpApiHostModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigureApp();
            PreConfigureFeature();
            PreConfigureCAP(configuration);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureDbContext();
            ConfigureLocalization();
            ConfigureJsonSerializer();
            ConfigreExceptionHandling();
            ConfigureVirtualFileSystem();
            ConfigureCaching(configuration);
            ConfigureAuditing(configuration);
            ConfigureSwagger(context.Services);
            ConfigureMultiTenancy(configuration);
            ConfigureCors(context.Services, configuration);
            ConfigureSeedWorker(context.Services, hostingEnvironment.IsDevelopment());
            ConfigureSecurity(context.Services, configuration, hostingEnvironment.IsDevelopment());
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            // http调用链
            app.UseCorrelationId();
            // 虚拟文件系统
            app.UseStaticFiles();
            // 路由
            app.UseRouting();
            // 跨域
            app.UseCors(DefaultCorsPolicyName);
            // 认证
            app.UseAuthentication();
            // jwt
            app.UseJwtTokenMiddleware();
            // 本地化
            app.UseMapRequestLocalization();
            // 授权
            app.UseAuthorization();
            // Swagger
            app.UseSwagger();
            // Swagger可视化界面
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support Localization Management API");
            });
            // 审计日志
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            // 路由
            app.UseConfiguredEndpoints();
        }
    }
}
