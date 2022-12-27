using System;
using System.Reflection;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace LINGYUN.Abp.UI.Navigation.VueVbenAdmin
{
    public class AbpUINavigationVueVbenAdminNavigationDefinitionProvider : NavigationDefinitionProvider
    {
        public override void Define(INavigationDefinitionContext context)
        {
            context.Add(GetDashboard());
            context.Add(GetManage());
            context.Add(GetSaas());
            context.Add(GetPlatform());
            // TODO: 网关不再需要动态管理
            // context.Add(GetApiGateway());
            context.Add(GetLocalization());
            context.Add(GetOssManagement());
            context.Add(GetTaskManagement());
            context.Add(GetWebhooksManagement());
            context.Add(GetMessages());
            context.Add(GetTextTemplating());
        }

        private static NavigationDefinition GetDashboard()
        {
            var dashboard = new ApplicationMenu(
                name: "Vben Dashboard",
                displayName: "Dashboard",
                url: "/dashboard",
                component: "",
                description: "Dashboard",
                icon: "ion:grid-outline",
                redirect: "/dashboard/workbench");

            dashboard.AddItem(
                new ApplicationMenu(
                    name: "Analysis",
                    displayName: "Analysis Page",
                    url: "/dashboard/analysis",
                    component: "/dashboard/analysis/index",
                    description: "Analysis Page"));
            dashboard.AddItem(
               new ApplicationMenu(
                   name: "Workbench",
                   displayName: "Workbench",
                   url: "/dashboard/workbench",
                   component: "/dashboard/workbench/index",
                   description: "Workbench"));


            return new NavigationDefinition(dashboard);
        }

        private static NavigationDefinition GetManage()
        {
            var manage = new ApplicationMenu(
                name: "Manage",
                displayName: "Manage",
                url: "/manage",
                component: "",
                description: "Manage",
                icon: "ant-design:control-outlined");

            var identity = manage.AddItem(
                new ApplicationMenu(
                    name: "Identity",
                    displayName: "Identity Management",
                    url: "/manage/identity",
                    component: "",
                    description: "Identity Management"));
            identity.AddItem(
              new ApplicationMenu(
                  name: "User",
                  displayName: "User",
                  url: "/manage/identity/user",
                  component: "/identity/user/index",
                  description: "User"));
            identity.AddItem(
              new ApplicationMenu(
                  name: "Role",
                  displayName: "Role",
                  url: "/manage/identity/role",
                  component: "/identity/role/index",
                  description: "Role"));
            identity.AddItem(
              new ApplicationMenu(
                  name: "Claim",
                  displayName: "ID",
                  url: "/manage/identity/claim-types",
                  component: "/identity/claim-types/index",
                  description: "ID",
                  multiTenancySides: MultiTenancySides.Host));
            identity.AddItem(
              new ApplicationMenu(
                  name: "OrganizationUnits",
                  displayName: "Organization",
                  url: "/manage/identity/organization-units",
                  component: "/identity/organization-units/index",
                  description: "Organization"));
            identity.AddItem(
              new ApplicationMenu(
                  name: "SecurityLogs",
                  displayName: "Security Logs",
                  url: "/manage/identity/security-logs",
                  component: "/identity/security-logs/index",
                  description: "Security Logs")
                // 此路由需要依赖安全日志特性
                .SetProperty("requiredFeatures", "AbpAuditing.Logging.SecurityLog"));

            manage.AddItem(new ApplicationMenu(
                   name: "AuditLogs",
                   displayName: "Audit Logs",
                   url: "/manage/audit-logs",
                   component: "/auditing/index",
                   description: "Audit Logs")
                // 此路由需要依赖审计日志特性
                .SetProperty("requiredFeatures", "AbpAuditing.Logging.AuditLog"));

            manage.AddItem(new ApplicationMenu(
                   name: "Settings",
                   displayName: "Settings",
                   url: "/manage/settings",
                   component: "/sys/settings/index",
                   description: "Settings")
                // 此路由需要依赖设置管理特性
                .SetProperty("requiredFeatures", "SettingManagement.Enable"));

            var removedIdsVersion = false;
            var assembly = typeof(AbpUINavigationVueVbenAdminNavigationDefinitionProvider).Assembly;
            var versionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (versionAttr != null)
            {
                var version = new Version(versionAttr.InformationalVersion);
                var version6 = new Version("6.0.0");
                removedIdsVersion = version6 >= version;
            }

            if (!removedIdsVersion)
            {
                var identityServer = manage.AddItem(
                    new ApplicationMenu(
                        name: "IdentityServer",
                        displayName: "Authentication Server",
                        url: "/manage/identity-server",
                        component: "",
                        description: "Authentication Server",
                        multiTenancySides: MultiTenancySides.Host));
                identityServer.AddItem(
                    new ApplicationMenu(
                        name: "Clients",
                        displayName: "Clients",
                        url: "/manage/identity-server/clients",
                        component: "/identity-server/clients/index",
                        description: "Clients",
                        multiTenancySides: MultiTenancySides.Host));
                identityServer.AddItem(
                    new ApplicationMenu(
                        name: "ApiResources",
                        displayName: "Api Resources",
                        url: "/manage/identity-server/api-resources",
                        component: "/identity-server/api-resources/index",
                        description: "Api Resources",
                        multiTenancySides: MultiTenancySides.Host));
                identityServer.AddItem(
                    new ApplicationMenu(
                        name: "IdentityResources",
                        displayName: "Identity Resources",
                        url: "/manage/identity-server/identity-resources",
                        component: "/identity-server/identity-resources/index",
                        description: "Identity Resources",
                        multiTenancySides: MultiTenancySides.Host));
                identityServer.AddItem(
                    new ApplicationMenu(
                        name: "ApiScopes",
                        displayName: "Api Scope",
                        url: "/manage/identity-server/api-scopes",
                        component: "/identity-server/api-scopes/index",
                        description: "Api Scope",
                        multiTenancySides: MultiTenancySides.Host));
                identityServer.AddItem(
                    new ApplicationMenu(
                        name: "PersistedGrants",
                        displayName: "Persisted Grants",
                        url: "/manage/identity-server/persisted-grants",
                        component: "/identity-server/persisted-grants/index",
                        description: "Persisted Grants",
                        multiTenancySides: MultiTenancySides.Host));
            }
            else
            {
                var openIddict = manage.AddItem(
                    new ApplicationMenu(
                        name: "OpenIddict",
                        displayName: "Authentication Server",
                        url: "/manage/openiddict",
                        component: "LAYOUT",
                        description: "Authentication Server(OpenIddict)",
                        multiTenancySides: MultiTenancySides.Host));
                openIddict.AddItem(
                    new ApplicationMenu(
                        name: "OpenIddictApplications",
                        displayName: "Application Management",
                        url: "/manage/openiddict/applications",
                        component: "/openiddict/applications/index",
                        description: "Application Management",
                        multiTenancySides: MultiTenancySides.Host));
                openIddict.AddItem(
                    new ApplicationMenu(
                        name: "OpenIddictAuthorizations",
                        displayName: "Authorization Management",
                        url: "/manage/openiddict/authorizations",
                        component: "/openiddict/authorizations/index",
                        description: "Authorization Management",
                        multiTenancySides: MultiTenancySides.Host));
                openIddict.AddItem(
                    new ApplicationMenu(
                        name: "OpenIddictScopes",
                        displayName: "Api Scope",
                        url: "/manage/openiddict/scopes",
                        component: "/openiddict/scopes/index",
                        description: "Api Scope",
                        multiTenancySides: MultiTenancySides.Host));
                openIddict.AddItem(
                    new ApplicationMenu(
                        name: "OpenIddictTokens",
                        displayName: "Authorization Token",
                        url: "/manage/openiddict/tokens",
                        component: "/openiddict/tokens/index",
                        description: "Authorization Token",
                        multiTenancySides: MultiTenancySides.Host));
            }

            manage.AddItem(
                new ApplicationMenu(
                    name: "Logs",
                    displayName: "System Log",
                    url: "/sys/logs",
                    component: "/sys/logging/index",
                    description: "System Log"));

            manage.AddItem(
                new ApplicationMenu(
                    name: "ApiDocument",
                    displayName: "Api Document",
                    url: "/openapi",
                    component: "IFrame",
                    description: "Api Document",
                    multiTenancySides: MultiTenancySides.Host)
                // TODO: 注意在部署完毕之后手动修改此菜单iframe地址
                .SetProperty("frameSrc", "http://127.0.0.1:30000/swagger/index.html"));

            manage.AddItem(
                new ApplicationMenu(
                    name: "Caches",
                    displayName: "Cache Management",
                    url: "/manage/cache",
                    component: "/caching-management/cache/index",
                    description: "Cache Management"));

            return new NavigationDefinition(manage);
        }

        private static NavigationDefinition GetSaas()
        {
            var saas = new ApplicationMenu(
                name: "Saas",
                displayName: "Saas",
                url: "/saas",
                component: "",
                description: "Saas",
                icon: "ant-design:cloud-server-outlined",
                multiTenancySides: MultiTenancySides.Host);
            saas.AddItem(
              new ApplicationMenu(
                  name: "Tenants",
                  displayName: "Tenant Management",
                  url: "/saas/tenants",
                  component: "/saas/tenant/index",
                  description: "Tenant Management",
                  multiTenancySides: MultiTenancySides.Host));
            saas.AddItem(
              new ApplicationMenu(
                  name: "Editions",
                  displayName: "Version Management",
                  url: "/saas/editions",
                  component: "/saas/editions/index",
                  description: "Version Management",
                  multiTenancySides: MultiTenancySides.Host));

            return new NavigationDefinition(saas);
        }

        private static NavigationDefinition GetPlatform()
        {
            var platform = new ApplicationMenu(
                name: "Platform",
                displayName: "Platform Management",
                url: "/platform",
                component: "",
                description: "Platform Management",
                icon: "ep:platform");
            platform.AddItem(
              new ApplicationMenu(
                  name: "DataDictionary",
                  displayName: "Data Dictionary",
                  url: "/platform/data-dic",
                  component: "/platform/dataDic/index",
                  description: "Data Dictionary"));
            platform.AddItem(
              new ApplicationMenu(
                  name: "Layout",
                  displayName: "Layout",
                  url: "/platform/layout",
                  component: "/platform/layout/index",
                  description: "Layout"));
            platform.AddItem(
              new ApplicationMenu(
                  name: "Menu",
                  displayName: "Menu",
                  url: "/platform/menu",
                  component: "/platform/menu/index",
                  description: "Menu"));

            return new NavigationDefinition(platform);
        }

        private static NavigationDefinition GetApiGateway()
        {
            var apiGateway = new ApplicationMenu(
                name: "ApiGateway",
                displayName: "Gateway Management",
                url: "/api-gateway",
                component: "",
                description: "Gateway Management",
                icon: "ant-design:gateway-outlined",
                multiTenancySides: MultiTenancySides.Host);
            apiGateway.AddItem(
              new ApplicationMenu(
                  name: "RouteGroup",
                  displayName: "Routing Packet",
                  url: "/api-gateway/group",
                  component: "/api-gateway/group/index",
                  description: "Routing Packet",
                  multiTenancySides: MultiTenancySides.Host));
            apiGateway.AddItem(
              new ApplicationMenu(
                  name: "GlobalConfiguration",
                  displayName: "Public Configuration",
                  url: "/api-gateway/global",
                  component: "/api-gateway/global/index",
                  description: "Public Configuration",
                  multiTenancySides: MultiTenancySides.Host));
            apiGateway.AddItem(
              new ApplicationMenu(
                  name: "Route",
                  displayName: "Routing Management",
                  url: "/api-gateway/route",
                  component: "/api-gateway/route/index",
                  description: "Routing Management",
                  multiTenancySides: MultiTenancySides.Host));
            apiGateway.AddItem(
             new ApplicationMenu(
                 name: "AggregateRoute",
                 displayName: "Aggregate Routing",
                 url: "/api-gateway/aggregate",
                 component: "/api-gateway/aggregate/index",
                 description: "Aggregate Routing",
                 multiTenancySides: MultiTenancySides.Host));

            return new NavigationDefinition(apiGateway);
        }

        private static NavigationDefinition GetLocalization()
        {
            var localization = new ApplicationMenu(
                name: "Localization",
                displayName: "Localization Management",
                url: "/localization",
                component: "",
                description: "Localization Management",
                icon: "ant-design:translation-outlined",
                multiTenancySides: MultiTenancySides.Host);
            localization.AddItem(
              new ApplicationMenu(
                  name: "Languages",
                  displayName: "Language Management",
                  url: "/localization/languages",
                  component: "/localization/languages/index",
                  description: "Language Management",
                  multiTenancySides: MultiTenancySides.Host)
                );
            localization.AddItem(
              new ApplicationMenu(
                  name: "Resources",
                  displayName: "Resource Management",
                  url: "/localization/resources",
                  component: "/localization/resources/index",
                  description: "Resource Management",
                  multiTenancySides: MultiTenancySides.Host)
                );
            localization.AddItem(
              new ApplicationMenu(
                  name: "Texts",
                  displayName: "Document Management",
                  url: "/localization/texts",
                  component: "/localization/texts/index",
                  description: "Document Management",
                  multiTenancySides: MultiTenancySides.Host)
                );

            return new NavigationDefinition(localization);
        }

        private static NavigationDefinition GetOssManagement()
        {
            var oss = new ApplicationMenu(
                name: "OssManagement",
                displayName: "Object Storage",
                url: "/oss",
                component: "",
                description: "Object Storage",
                icon: "ant-design:file-twotone");
            oss.AddItem(
              new ApplicationMenu(
                  name: "Containers",
                  displayName: "Container Management",
                  url: "/oss/containers",
                  component: "/oss-management/containers/index",
                  description: "Container Management"));
            oss.AddItem(
              new ApplicationMenu(
                  name: "Objects",
                  displayName: "File Management",
                  url: "/oss/objects",
                  component: "/oss-management/objects/index",
                  description: "File Management"));

            return new NavigationDefinition(oss);
        }

        private static NavigationDefinition GetTaskManagement()
        {
            var task = new ApplicationMenu(
                name: "TaskManagement",
                displayName: "Task Scheduling Platform",
                url: "/task-management",
                component: "",
                description: "Task Scheduling Platform",
                icon: "bi:list-task");
            task.AddItem(
              new ApplicationMenu(
                  name: "BackgroundJobs",
                  displayName: "Task Management",
                  url: "/task-management/background-jobs",
                  component: "/task-management/background-jobs/index",
                  description: "Task Management"));
            task.AddItem(
              new ApplicationMenu(
                  name: "BackgroundJobInfoDetail",
                  displayName: "Task Details",
                  url: "/task-management/background-jobs/:id",
                  component: "/task-management/background-jobs/components/BackgroundJobInfoDetail",
                  description: "Task Details")
              .SetProperty("hideMenu", "true")
              .SetProperty("hideTab", "true"));

            return new NavigationDefinition(task);
        }

        private static NavigationDefinition GetWebhooksManagement()
        {
            var webhooks = new ApplicationMenu(
                name: "WebHooks",
                displayName: "WebHooks",
                url: "/webhooks",
                component: "",
                description: "WebHooks",
                icon: "ic:outline-webhook",
                multiTenancySides: MultiTenancySides.Host);
            webhooks.AddItem(
              new ApplicationMenu(
                  name: "Subscriptions",
                  displayName: "Manage Subscription",
                  url: "/webhooks/subscriptions",
                  component: "/webhooks/subscriptions/index",
                  description: "Manage Subscription",
                  multiTenancySides: MultiTenancySides.Host));
            webhooks.AddItem(
              new ApplicationMenu(
                  name: "SendAttempts",
                  displayName: "Management Records",
                  url: "/webhooks/send-attempts",
                  component: "/webhooks/send-attempts/index",
                  description: "Management Records",
                  multiTenancySides: MultiTenancySides.Host));

            return new NavigationDefinition(webhooks);
        }

        private static NavigationDefinition GetMessages()
        {
            var messages = new ApplicationMenu(
                name: "Messages",
                displayName: "Message Management",
                url: "/messages",
                component: "",
                description: "Message Management",
                icon: "ant-design:message-outlined");
            messages.AddItem(
              new ApplicationMenu(
                  name: "Notifications",
                  displayName: "Notification Management",
                  url: "/messages/notifications",
                  component: "/messages/notifications/index",
                  description: "Notification Management"));

            return new NavigationDefinition(messages);
        }

        private static NavigationDefinition GetTextTemplating()
        {
            var textTemplating = new ApplicationMenu(
                name: "Templates",
                displayName: "Template Management",
                url: "/text-templating",
                component: "",
                description: "Template Management",
                icon: "eos-icons:templates-outlined",
                multiTenancySides: MultiTenancySides.Host);
            textTemplating.AddItem(
              new ApplicationMenu(
                  name: "TextTemplates",
                  displayName: "Text Template",
                  url: "/text-templating/text-templates",
                  component: "/text-templating/templates/index",
                  description: "Text Template",
                  multiTenancySides: MultiTenancySides.Host));

            return new NavigationDefinition(textTemplating);
        }
    }
}
