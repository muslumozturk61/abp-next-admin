using DotNetCore.CAP;
using LINGYUN.Abp.IdentityServer.IdentityResources;
using LINGYUN.Abp.Localization.CultureMap;
using LINGYUN.Abp.Serilog.Enrichers.Application;
using LINGYUN.Abp.Serilog.Enrichers.UniqueId;
using LY.MicroService.IdentityServer.IdentityResources;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Volo.Abp.Account.Localization;
using Volo.Abp.Auditing;
using Volo.Abp.Caching;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.IdentityServer;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace LY.MicroService.IdentityServer;

public partial class IdentityServerModule
{
    protected const string ApplicationName = "Identity-Server-STS";
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    private void PreConfigureFeature()
    {
        OneTimeRunner.Run(() =>
        {
            GlobalFeatureManager.Instance.Modules.Editions().EnableAll();
        });
    }

    private void PreConfigureApp()
    {
        AbpSerilogEnrichersConsts.ApplicationName = ApplicationName;

        PreConfigure<AbpSerilogEnrichersUniqueIdOptions>(options =>
        {
            // It is distinguished by open ports, it should be between 0-31
            options.SnowflakeIdOptions.WorkerId = 1;
            options.SnowflakeIdOptions.WorkerIdBits = 5;
            options.SnowflakeIdOptions.DatacenterId = 1;
        });
    }

    private void PreConfigureCAP(IConfiguration configuration)
    {
        PreConfigure<CapOptions>(options =>
        {
            options
            .UseSqlServer(mySqlOptions =>
            {
                configuration.GetSection("CAP:SqlServer").Bind(mySqlOptions);
            })
            .UseRabbitMQ(rabbitMQOptions =>
            {
                configuration.GetSection("CAP:RabbitMQ").Bind(rabbitMQOptions);
            })
            .UseDashboard();
        });
    }

    private void PreConfigureCertificate(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var cerConfig = configuration.GetSection("Certificates");
        if (environment.IsProduction() &&
            cerConfig.Exists())
        {
            // There is a certificate configuration in the development environment
            // And the certificate file exists, use the custom certificate file to start the Ids server
            var cerPath = Path.Combine(environment.ContentRootPath, cerConfig["CerPath"]);
            if (File.Exists(cerPath))
            {
                PreConfigure<AbpIdentityServerBuilderOptions>(options =>
                {
                    options.AddDeveloperSigningCredential = false;
                });

                var cer = new X509Certificate2(cerPath, cerConfig["Password"]);

                PreConfigure<IIdentityServerBuilder>(builder =>
                {
                    builder.AddSigningCredential(cer);
                });
            }
        }
    }

    private void ConfigureDbContext()
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });
    }

    private void ConfigureDataSeeder()
    {
        Configure<CustomIdentityResourceDataSeederOptions>(options =>
        {
            options.Resources.Add(new CustomIdentityResources.AvatarUrl());
        });
    }

    private void ConfigureJsonSerializer()
    {
        // Uniform time and date format
        Configure<AbpJsonOptions>(options =>
        {
            options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        });
        // Chinese serialization encoding problem
        Configure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        });
    }
    private void ConfigureCaching(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            configuration.GetSection("DistributedCache").Bind(options);
        });

        Configure<RedisCacheOptions>(options =>
        {
            var redisConfig = ConfigurationOptions.Parse(options.Configuration);
            options.ConfigurationOptions = redisConfig;
            options.InstanceName = configuration["Redis:InstanceName"];
        });
    }
    private void ConfigureIdentity(IConfiguration configuration)
    {
        // Add configuration file definition, which is required when creating a new tenant
        Configure<IdentityOptions>(options =>
        {
            var identityConfiguration = configuration.GetSection("Identity");
            if (identityConfiguration.Exists())
            {
                identityConfiguration.Bind(options);
            }
        });
    }
    private void ConfigureVirtualFileSystem()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<IdentityServerModule>("LY.MicroService.IdentityServer");
        });
    }
    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("tr-TR", "tr-TR", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
          options.Resources
                .Get<AccountResource>()
                .AddVirtualJson("/Localization/Resources");
        });

        Configure<AbpLocalizationCultureMapOptions>(options =>
        {
            var zhHansCultureMapInfo = new CultureMapInfo
            {
                TargetCulture = "tr-TR",
                SourceCultures = new string[] { "tr", "tr_TR", "tr-TR" }
            };

            options.CulturesMaps.Add(zhHansCultureMapInfo);
            options.UiCulturesMaps.Add(zhHansCultureMapInfo);
        });
    }
    private void ConfigureAuditing()
    {
        Configure<AbpAuditingOptions>(options =>
        {
            // options.IsEnabledForGetRequests = true;
            options.ApplicationName = ApplicationName;
        });
    }
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.Applications["STS"].RootUrl = configuration["App:StsUrl"];

            options.Applications["MVC"].Urls["EmailVerifyLogin"] = "Account/VerifyCode";
            options.Applications["MVC"].Urls["EmailConfirm"] = "Account/EmailConfirm";
        });
    }
    private void ConfigureSecurity(IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
    {
        services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = configuration["AuthServer:ApiName"];
                });

        if (!isDevelopment)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            services
                .AddDataProtection()
                .SetApplicationName("LINGYUN.Abp.Application")
                .PersistKeysToStackExchangeRedis(redis, "LINGYUN.Abp.Application:DataProtection:Protection-Keys");
        }

        services.AddSameSiteCookiePolicy();
    }
    private void ConfigureMultiTenancy(IConfiguration configuration)
    {
        // multi-tenant
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });

        var tenantResolveCfg = configuration.GetSection("App:Domains");
        if (tenantResolveCfg.Exists())
        {
            Configure<AbpTenantResolveOptions>(options =>
            {
                var domains = tenantResolveCfg.Get<string[]>();
                foreach (var domain in domains)
                {
                    options.AddDomainTenantResolver(domain);
                }
            });
        }
    }
    private void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    // When referencing the LINGYUN.Abp.AspNetCore.Mvc.Wrapper package, it can be replaced with WithAbpWrapExposedHeaders
                    .WithExposedHeaders("_AbpWrapResult", "_AbpDontWrapResult")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
