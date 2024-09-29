using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyAdmin.ApiHost.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Host = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Origin = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Referer = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponseStatusCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponseBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HttpMethod = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Exceptions = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLog", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaMenu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false, comment: "路由名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Path = table.Column<string>(type: "longtext", nullable: true, comment: "路由路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MenuType = table.Column<int>(type: "int", nullable: false),
                    Locale = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMenu", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: true, comment: "角色编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaRole", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaTenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaTenant", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Salt = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Account = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mobile = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaUser", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DescBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoleMenu",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenu", x => new { x.RoleId, x.MenuId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "MaMenu",
                columns: new[] { "Id", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Icon", "IsDeleted", "Label", "LastModificationTime", "LastModifierId", "Locale", "MenuType", "Name", "Order", "ParentId", "Path", "TenantId" },
                values: new object[,]
                {
                    { new Guid("08a917b6-8964-4d98-86e6-851ebc83a4ea"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3612), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "删除角色", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.deleterole", 2, "delete_role", 2, new Guid("52620b9b-ee0a-4153-b7b5-a58299756fe6"), null, null },
                    { new Guid("19bac5dc-0b43-4809-ab3a-53be906de0e4"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3623), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "菜单管理", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.menus", 1, "menus", 4, new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), "/sys/menu/index", null },
                    { new Guid("262e8351-96a0-45c0-a536-d99a278ddd8b"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3608), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "添加角色", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.addrole", 2, "add_role", 2, new Guid("52620b9b-ee0a-4153-b7b5-a58299756fe6"), "/sys/role/index", null },
                    { new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3079), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "系统管理", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.sysmanager", 3, "SysManager", 2, null, null, null },
                    { new Guid("368a7827-0377-48ec-a335-59e49451b2c9"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3619), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "更新租户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.updatetenant", 2, "update_tenant", 3, new Guid("95ca7b1b-55dc-48e9-9317-7d67da1bec78"), null, null },
                    { new Guid("4f15068a-62a7-4e73-8cc3-31501643f699"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3603), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "删除用户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.deleteuser", 2, "delete_user", 1, new Guid("a39e28a9-9da9-48ee-a89d-5071ccf854fc"), null, null },
                    { new Guid("52620b9b-ee0a-4153-b7b5-a58299756fe6"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3605), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "角色管理", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.roles", 1, "roles", 2, new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), "/sys/role/index", null },
                    { new Guid("5a7b38b6-b162-40ac-98e9-821660821a5e"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3594), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "添加用户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.adduser", 2, "add_user", 1, new Guid("a39e28a9-9da9-48ee-a89d-5071ccf854fc"), null, null },
                    { new Guid("95ca7b1b-55dc-48e9-9317-7d67da1bec78"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3614), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "租户管理", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.tenants", 1, "tenants", 3, new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), "/sys/tenant/index", null },
                    { new Guid("9ad04f5c-4fec-40e0-be8d-3f359a058024"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3601), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "更新用户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.updateuser", 2, "update_user", 1, new Guid("a39e28a9-9da9-48ee-a89d-5071ccf854fc"), null, null },
                    { new Guid("9f857d8d-8558-47e3-9bda-119e0e63500c"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3628), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "更新菜单", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.updatemenu", 2, "update_menu", 4, new Guid("19bac5dc-0b43-4809-ab3a-53be906de0e4"), null, null },
                    { new Guid("a39e28a9-9da9-48ee-a89d-5071ccf854fc"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3086), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "用户管理", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.users", 1, "users", 1, new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), "/sys/user/index", null },
                    { new Guid("a454a788-4231-4d96-8e98-8ce426ea4acc"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(1927), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "首页", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.home", 1, "home", 1, null, null, null },
                    { new Guid("ae4f9eed-3e20-45c8-89ea-204c68000a06"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3633), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "系统日志", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.syslog", 1, "sys_log", 5, new Guid("32bb7608-ceff-4e7c-8c67-ff3892625e17"), null, null },
                    { new Guid("b479f220-bbb6-4878-a184-619292efacd3"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3621), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "删除租户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.deletetenant", 2, "delete_tenant", 3, new Guid("95ca7b1b-55dc-48e9-9317-7d67da1bec78"), null, null },
                    { new Guid("bc5c7271-8b77-43f1-9937-d3bebf85d3ed"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3610), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "更新角色", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.updaterole", 2, "update_role", 2, new Guid("52620b9b-ee0a-4153-b7b5-a58299756fe6"), null, null },
                    { new Guid("bf8559c0-b3cd-40a8-8465-8c46778f63bc"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3630), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "删除菜单", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.deletemenu", 2, "delete_menu", 4, new Guid("19bac5dc-0b43-4809-ab3a-53be906de0e4"), null, null },
                    { new Guid("e8b21899-0105-42a5-8b12-b3cd291e81e5"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3626), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "添加菜单", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.addmenu", 2, "add_menu", 4, new Guid("19bac5dc-0b43-4809-ab3a-53be906de0e4"), null, null },
                    { new Guid("ecdf8af6-c987-41ad-8c60-48da796bf278"), new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(3616), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, "添加租户", null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "menu.server.addtenant", 2, "add_tenant", 3, new Guid("95ca7b1b-55dc-48e9-9317-7d67da1bec78"), null, null }
                });

            migrationBuilder.InsertData(
                table: "MaRole",
                columns: new[] { "Id", "Code", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Description", "IsDeleted", "IsEnabled", "LastModificationTime", "LastModifierId", "Name", "TenantId" },
                values: new object[,]
                {
                    { new Guid("530da417-da46-492a-9f40-7840823f697f"), "Admin", new DateTime(2024, 9, 29, 14, 17, 15, 521, DateTimeKind.Local).AddTicks(9292), new Guid("dcf2696e-8364-4559-b6ad-720eba834af7"), new Guid("00000000-0000-0000-0000-000000000000"), null, "admin role", false, true, null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "admin", null },
                    { new Guid("8c9f0746-f181-4f43-81b3-6bc087ad2b56"), "Dev", new DateTime(2024, 9, 29, 14, 17, 15, 522, DateTimeKind.Local).AddTicks(966), new Guid("dcf2696e-8364-4559-b6ad-720eba834af7"), new Guid("00000000-0000-0000-0000-000000000000"), null, "dev role", false, true, null, new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "dev", null }
                });

            migrationBuilder.InsertData(
                table: "MaUser",
                columns: new[] { "Id", "Account", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Email", "IsDeleted", "IsEnabled", "LastModificationTime", "LastModifierId", "Mobile", "Name", "Password", "Salt", "TenantId" },
                values: new object[,]
                {
                    { new Guid("dcf2696e-8364-4559-b6ad-720eba834af7"), "dev", new DateTime(2024, 9, 29, 14, 17, 15, 521, DateTimeKind.Local).AddTicks(8767), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, true, null, new Guid("00000000-0000-0000-0000-000000000000"), null, "dev", "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", "7894561230", null },
                    { new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1"), "admin", new DateTime(2024, 9, 29, 14, 17, 15, 512, DateTimeKind.Local).AddTicks(2957), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, true, null, new Guid("00000000-0000-0000-0000-000000000000"), null, "admin", "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", "7894561230", null }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("8c9f0746-f181-4f43-81b3-6bc087ad2b56"), new Guid("dcf2696e-8364-4559-b6ad-720eba834af7") },
                    { new Guid("530da417-da46-492a-9f40-7840823f697f"), new Guid("dd07176d-c210-41d6-bf0d-31182ec9c1b1") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaLog");

            migrationBuilder.DropTable(
                name: "MaMenu");

            migrationBuilder.DropTable(
                name: "MaRole");

            migrationBuilder.DropTable(
                name: "MaTenant");

            migrationBuilder.DropTable(
                name: "MaUser");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "RoleMenu");

            migrationBuilder.DropTable(
                name: "UserRole");
        }
    }
}
