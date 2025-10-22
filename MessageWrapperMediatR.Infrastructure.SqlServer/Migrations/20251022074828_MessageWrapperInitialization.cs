using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessageWrapperInitialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MessageWrapperMeditaR");

            migrationBuilder.CreateTable(
                name: "CollectedMessages",
                schema: "MessageWrapperMeditaR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RawMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromQueue = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ReceptionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsHandled = table.Column<bool>(type: "bit", nullable: false),
                    TimeToLiveInDays = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    AssociateCommand = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectedMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Handlers",
                schema: "MessageWrapperMeditaR",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Queue = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TimeToLiveInDays = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    MessageIsStored = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssociateCommand = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BusType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Handlers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bindings",
                schema: "MessageWrapperMeditaR",
                columns: table => new
                {
                    HandlerId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Exchange = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bindings", x => new { x.Exchange, x.RoutingKey, x.HandlerId });
                    table.ForeignKey(
                        name: "FK_Bindings_Handlers_HandlerId",
                        column: x => x.HandlerId,
                        principalSchema: "MessageWrapperMeditaR",
                        principalTable: "Handlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bindings_HandlerId",
                schema: "MessageWrapperMeditaR",
                table: "Bindings",
                column: "HandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectedMessages_FromQueue",
                schema: "MessageWrapperMeditaR",
                table: "CollectedMessages",
                column: "FromQueue");

            migrationBuilder.CreateIndex(
                name: "IX_CollectedMessages_ReceptionDate",
                schema: "MessageWrapperMeditaR",
                table: "CollectedMessages",
                column: "ReceptionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bindings",
                schema: "MessageWrapperMeditaR");

            migrationBuilder.DropTable(
                name: "CollectedMessages",
                schema: "MessageWrapperMeditaR");

            migrationBuilder.DropTable(
                name: "Handlers",
                schema: "MessageWrapperMeditaR");
        }
    }
}
