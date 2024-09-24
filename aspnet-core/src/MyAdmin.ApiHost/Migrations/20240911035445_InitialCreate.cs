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
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "longtext", nullable: true, comment: "路由路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MenuType = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false)
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
                    Code = table.Column<string>(type: "longtext", nullable: false, comment: "角色编码")
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
                    DescBody = table.Column<string>(type: "longtext", nullable: false)
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
                columns: new[] { "Id", "Code", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Icon", "IsDeleted", "LastModificationTime", "LastModifierId", "Level", "MenuType", "Name", "Order", "ParentId", "TenantId", "Url" },
                values: new object[,]
                {
                    { new Guid("15893413-9f8d-4160-89bd-fb18565a95ef"), "RoleManager-Add", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8788), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "角色管理-新增", 2, null, null, null },
                    { new Guid("24f1dcbd-0f97-4735-a235-ad0d8712e37b"), "UserManager-Add", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8719), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "用户管理-新增", 1, null, null, null },
                    { new Guid("26a43341-1169-4af4-bc01-ec06cca006d0"), "TenantManager", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8794), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 1, "租户管理", 3, null, null, "/sys/tenant/index" },
                    { new Guid("69aa45fd-960a-4667-8da8-9fa9580f5494"), "SystemManager", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8532), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 0, 3, "系统管理", 2, null, null, null },
                    { new Guid("82ff097f-c90a-4126-a543-0f6afafd39c1"), "TenantManager-Update", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8798), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "租户管理-更新", 3, null, null, null },
                    { new Guid("8e1cca08-5121-4d1c-b618-3292a2f83a3b"), "UserManager-Delete", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8784), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "用户管理-删除", 1, null, null, null },
                    { new Guid("a07929c4-5aa6-469b-b43a-62e325dce72c"), "Log", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8802), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 0, 1, "系统日志", 3, null, null, null },
                    { new Guid("a706e070-05ef-4ad3-a71a-93cc470ff124"), "RoleManager-Update", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8790), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "角色管理-更新", 2, null, null, null },
                    { new Guid("ace5e6ea-b855-4e48-9623-1d8373f49671"), "UserManager-Update", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8724), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "用户管理-更新", 1, null, null, null },
                    { new Guid("c32e142e-8576-4ab3-bb0e-7f3920da8886"), "UserManager", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8540), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 1, "用户管理", 1, null, null, "/sys/user/index" },
                    { new Guid("cc8d7fc5-f53d-405d-9912-0034e7c8296a"), "RoleManager", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8786), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 1, "角色管理", 2, null, null, "/sys/role/index" },
                    { new Guid("f25f4e3d-112f-4ba6-b3b3-a15de282baa8"), "RoleManager-Delete", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8793), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "角色管理-删除", 2, null, null, null },
                    { new Guid("f2d0fa1a-635a-4e63-a6e5-0371fcdbac47"), "Home", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(7455), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 0, 1, "首页", 1, null, null, null },
                    { new Guid("f9fd2a4d-b549-408d-834f-679addd3886b"), "TenantManager-Delete", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8800), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "租户管理-删除", 3, null, null, null },
                    { new Guid("fcf206e7-4635-4e77-bfae-a4680e151acd"), "TenantManager-Add", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(8796), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), 1, 2, "租户管理-新增", 3, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "MaRole",
                columns: new[] { "Id", "Code", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Description", "IsDeleted", "IsEnabled", "LastModificationTime", "LastModifierId", "Name", "TenantId" },
                values: new object[,]
                {
                    { new Guid("3486d760-d9a1-4a68-82dc-bf827590ad03"), "Dev", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(6569), new Guid("85e67805-3d92-4199-9cc3-347a66855f06"), new Guid("00000000-0000-0000-0000-000000000000"), null, "dev role", false, true, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), "dev", null },
                    { new Guid("7e349556-e2fe-40fc-8d30-40aba7ab5390"), "Admin", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(4724), new Guid("85e67805-3d92-4199-9cc3-347a66855f06"), new Guid("00000000-0000-0000-0000-000000000000"), null, "admin role", false, true, null, new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), "admin", null }
                });

            migrationBuilder.InsertData(
                table: "MaUser",
                columns: new[] { "Id", "Account", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "Email", "IsDeleted", "IsEnabled", "LastModificationTime", "LastModifierId", "Mobile", "Name", "Password", "Salt", "TenantId" },
                values: new object[,]
                {
                    { new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0"), "admin", new DateTime(2024, 9, 11, 11, 54, 45, 111, DateTimeKind.Local).AddTicks(7838), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, true, null, new Guid("00000000-0000-0000-0000-000000000000"), null, "admin", "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", "7894561230", null },
                    { new Guid("85e67805-3d92-4199-9cc3-347a66855f06"), "dev", new DateTime(2024, 9, 11, 11, 54, 45, 123, DateTimeKind.Local).AddTicks(4216), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), null, null, false, true, null, new Guid("00000000-0000-0000-0000-000000000000"), null, "dev", "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", "7894561230", null }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("7e349556-e2fe-40fc-8d30-40aba7ab5390"), new Guid("1d5b1109-e8fe-4ecc-81c7-02d6e23220b0") },
                    { new Guid("3486d760-d9a1-4a68-82dc-bf827590ad03"), new Guid("85e67805-3d92-4199-9cc3-347a66855f06") }
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
