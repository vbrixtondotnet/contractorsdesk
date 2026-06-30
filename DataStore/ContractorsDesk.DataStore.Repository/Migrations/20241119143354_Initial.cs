using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContractorsDesk.DataStore.Account.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RequireLogOn = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PermissionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_RoleCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RoleCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBookmarks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateUpdated", "IsDeleted", "Name", "UpdatedBy" },
                values: new object[] { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "CH Anderson Construction", null });

            migrationBuilder.InsertData(
                table: "PermissionCategory",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateUpdated", "IsDeleted", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "System Permissions", null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Company Permissions", null }
                });

            migrationBuilder.InsertData(
                table: "RoleCategory",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateUpdated", "IsDeleted", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "System", null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Company", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "CategoryId", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, 1, null, "Super Admin", "Super Admin" },
                    { 2, 1, null, "Customer Support", "Customer Support" },
                    { 3, 1, null, "Super IT", "Super IT" },
                    { 4, 2, null, "Company Owner", "Company Owner" },
                    { 5, 2, null, "Project Manager", "Project Manager" },
                    { 6, 2, null, "Assistant Project Manager", "Assistant Project Manager" },
                    { 7, 2, null, "Bookkeeper", "Bookkeeper" },
                    { 8, 2, null, "Company IT", "Company IT" },
                    { 9, 2, null, "Office Manager", "Office Manager" },
                    { 10, 2, null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CategoryId", "CreatedBy", "DateCreated", "DateUpdated", "Description", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage All Jobs", false, null },
                    { 2, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Own Jobs", false, null },
                    { 3, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage All Estimates", false, null },
                    { 4, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Own Estimates", false, null },
                    { 5, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage All Action Items", false, null },
                    { 6, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Own Action Items", false, null },
                    { 7, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company Users", false, null },
                    { 8, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company Roles", false, null },
                    { 9, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Schedules", false, null },
                    { 10, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Finance", false, null },
                    { 11, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Client Websites", false, null },
                    { 12, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company Email", false, null },
                    { 13, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company SEO", false, null },
                    { 14, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Client Onboarding", false, null },
                    { 15, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Access to Third-party Services", false, null },
                    { 16, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage System Users", false, null },
                    { 17, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage System Roles", false, null },
                    { 18, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Account Owners", false, null },
                    { 19, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage System Permissions", false, null },
                    { 20, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Client Websites", false, null },
                    { 21, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Client Onboarding", false, null },
                    { 22, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Access to Third-party Services", false, null },
                    { 23, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company Email", false, null },
                    { 24, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Company SEO", false, null },
                    { 25, 1, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Manage Clients", false, null },
                    { 26, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Assign Jobs", false, null },
                    { 27, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Assign Estimates", false, null },
                    { 28, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Assign Action Items", false, null },
                    { 29, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Access Client Jobs", false, null },
					{ 30, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Manage Data Mapping", false, null },
					{ 30, 2, null, new DateTime(2024, 10, 5, 20, 30, 0, 0, DateTimeKind.Unspecified), null, "Can Edit Accepted Proposals", false, null }
				});

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, null, null, 1, 4 },
                    { 2, null, null, 3, 4 },
                    { 3, null, null, 5, 4 },
                    { 4, null, null, 7, 4 },
                    { 5, null, null, 8, 4 },
                    { 6, null, null, 9, 4 },
                    { 7, null, null, 10, 4 },
                    { 8, null, null, 26, 4 },
                    { 9, null, null, 27, 4 },
                    { 10, null, null, 28, 4 },
                    { 11, null, null, 1, 5 },
                    { 12, null, null, 3, 5 },
                    { 13, null, null, 5, 5 },
                    { 14, null, null, 9, 5 },
                    { 15, null, null, 10, 5 },
                    { 16, null, null, 26, 5 },
                    { 17, null, null, 27, 5 },
                    { 18, null, null, 28, 5 },
                    { 19, null, null, 2, 6 },
                    { 20, null, null, 4, 6 },
                    { 21, null, null, 6, 6 },
                    { 22, null, null, 9, 6 },
                    { 23, null, null, 10, 7 },
                    { 24, null, null, 14, 7 },
                    { 25, null, null, 15, 7 },
                    { 26, null, null, 11, 8 },
                    { 27, null, null, 12, 8 },
                    { 28, null, null, 1, 9 },
                    { 29, null, null, 3, 9 },
                    { 30, null, null, 5, 9 },
                    { 31, null, null, 7, 9 },
                    { 32, null, null, 9, 9 },
                    { 33, null, null, 26, 9 },
                    { 34, null, null, 27, 9 },
                    { 35, null, null, 28, 9 },
                    { 36, null, null, 29, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_PermissionId",
                table: "AspNetRoleClaims",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CategoryId",
                table: "AspNetRoles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CategoryId",
                table: "Permissions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_UserId",
                table: "UserBookmarks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "UserBookmarks");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PermissionCategory");

            migrationBuilder.DropTable(
                name: "RoleCategory");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
