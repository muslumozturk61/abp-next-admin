using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LY.MicroService.WebhooksManagement.Migrations
{
    public partial class init_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpWebhooksEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebhookName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhooksEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpWebhooksSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebhookUri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Webhooks = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Headers = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhooksSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpWebhooksSendAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebhookEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebhookSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                    RequestHeaders = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    SendExactSameData = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhooksSendAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpWebhooksSendAttempts_AbpWebhooksEvents_WebhookEventId",
                        column: x => x.WebhookEventId,
                        principalTable: "AbpWebhooksEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpWebhooksSendAttempts_WebhookEventId",
                table: "AbpWebhooksSendAttempts",
                column: "WebhookEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpWebhooksSendAttempts");

            migrationBuilder.DropTable(
                name: "AbpWebhooksSubscriptions");

            migrationBuilder.DropTable(
                name: "AbpWebhooksEvents");
        }
    }
}
