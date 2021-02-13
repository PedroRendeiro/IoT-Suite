using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IoTSuite.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlarmUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterClock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Weekday = table.Column<int>(type: "integer", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    Minute = table.Column<int>(type: "integer", nullable: false),
                    Second = table.Column<int>(type: "integer", nullable: false),
                    Millisecond = table.Column<int>(type: "integer", nullable: false),
                    TimeZone = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterClock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Thing",
                columns: table => new
                {
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Features = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thing", x => x.ThingId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true),
                    Salt = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecast",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TemperatureC = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecast", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RFIDTag",
                columns: table => new
                {
                    UID = table.Column<string>(type: "text", nullable: false),
                    AlarmUserId = table.Column<long>(type: "bigint", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFIDTag", x => x.UID);
                    table.ForeignKey(
                        name: "FK_RFIDTag_AlarmUser_AlarmUserId",
                        column: x => x.AlarmUserId,
                        principalTable: "AlarmUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AveragePower",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    P = table.Column<long>(type: "bigint", nullable: false),
                    ClockId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AveragePower", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AveragePower_PowerMeterClock_ClockId",
                        column: x => x.ClockId,
                        principalTable: "PowerMeterClock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Measure",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Measure_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    User = table.Column<Guid>(type: "uuid", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_Policy_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Velocity = table.Column<float>(type: "real", nullable: false),
                    Course = table.Column<float>(type: "real", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterInstantaneous",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    V_L1 = table.Column<double>(type: "double precision", nullable: false),
                    I_L1 = table.Column<double>(type: "double precision", nullable: false),
                    V_L2 = table.Column<double>(type: "double precision", nullable: true),
                    I_L2 = table.Column<double>(type: "double precision", nullable: true),
                    V_L3 = table.Column<double>(type: "double precision", nullable: true),
                    I_L3 = table.Column<double>(type: "double precision", nullable: true),
                    P_L1_In = table.Column<double>(type: "double precision", nullable: true),
                    P_L1_Out = table.Column<double>(type: "double precision", nullable: true),
                    P_L2_In = table.Column<double>(type: "double precision", nullable: true),
                    P_L2_Out = table.Column<double>(type: "double precision", nullable: true),
                    P_L3_In = table.Column<double>(type: "double precision", nullable: true),
                    P_L3_Out = table.Column<double>(type: "double precision", nullable: true),
                    P_In = table.Column<double>(type: "double precision", nullable: false),
                    P_Out = table.Column<double>(type: "double precision", nullable: true),
                    PF_T = table.Column<double>(type: "double precision", nullable: false),
                    PF_L1 = table.Column<double>(type: "double precision", nullable: true),
                    PF_L2 = table.Column<double>(type: "double precision", nullable: true),
                    PF_L3 = table.Column<double>(type: "double precision", nullable: true),
                    F = table.Column<double>(type: "double precision", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterInstantaneous", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerMeterInstantaneous_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alarm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UID = table.Column<string>(type: "text", nullable: true),
                    RFIDTagUID = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alarm_RFIDTag_RFIDTagUID",
                        column: x => x.RFIDTagUID,
                        principalTable: "RFIDTag",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alarm_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterTariff",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    A_In_1 = table.Column<double>(type: "double precision", nullable: false),
                    A_In_2 = table.Column<double>(type: "double precision", nullable: false),
                    A_In_3 = table.Column<double>(type: "double precision", nullable: false),
                    A_In_4 = table.Column<double>(type: "double precision", nullable: true),
                    A_In_5 = table.Column<double>(type: "double precision", nullable: true),
                    A_In_6 = table.Column<double>(type: "double precision", nullable: true),
                    A_In_T = table.Column<double>(type: "double precision", nullable: false),
                    A_Out_1 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_2 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_3 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_4 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_5 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_6 = table.Column<double>(type: "double precision", nullable: true),
                    A_Out_T = table.Column<double>(type: "double precision", nullable: true),
                    Ri_In_1 = table.Column<double>(type: "double precision", nullable: false),
                    Ri_In_2 = table.Column<double>(type: "double precision", nullable: false),
                    Ri_In_3 = table.Column<double>(type: "double precision", nullable: false),
                    Ri_In_4 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_In_5 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_In_6 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_In_T = table.Column<double>(type: "double precision", nullable: false),
                    Rc_In_1 = table.Column<double>(type: "double precision", nullable: false),
                    Rc_In_2 = table.Column<double>(type: "double precision", nullable: false),
                    Rc_In_3 = table.Column<double>(type: "double precision", nullable: false),
                    Rc_In_4 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_In_5 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_In_6 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_In_T = table.Column<double>(type: "double precision", nullable: false),
                    Ri_Out_1 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_2 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_3 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_4 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_5 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_6 = table.Column<double>(type: "double precision", nullable: true),
                    Ri_Out_T = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_1 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_2 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_3 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_4 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_5 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_6 = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out_T = table.Column<double>(type: "double precision", nullable: true),
                    P_In_MAX_1Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_2Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_3Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_4Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_5Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_6Id = table.Column<long>(type: "bigint", nullable: true),
                    P_In_MAX_TId = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_1Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_2Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_3Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_4Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_5Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_6Id = table.Column<long>(type: "bigint", nullable: true),
                    P_Out_MAX_TId = table.Column<long>(type: "bigint", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_1Id",
                        column: x => x.P_In_MAX_1Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_2Id",
                        column: x => x.P_In_MAX_2Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_3Id",
                        column: x => x.P_In_MAX_3Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_4Id",
                        column: x => x.P_In_MAX_4Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_5Id",
                        column: x => x.P_In_MAX_5Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_6Id",
                        column: x => x.P_In_MAX_6Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_In_MAX_TId",
                        column: x => x.P_In_MAX_TId,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_1Id",
                        column: x => x.P_Out_MAX_1Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_2Id",
                        column: x => x.P_Out_MAX_2Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_3Id",
                        column: x => x.P_Out_MAX_3Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_4Id",
                        column: x => x.P_Out_MAX_4Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_5Id",
                        column: x => x.P_Out_MAX_5Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_6Id",
                        column: x => x.P_Out_MAX_6Id,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_AveragePower_P_Out_MAX_TId",
                        column: x => x.P_Out_MAX_TId,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTariff_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterTotal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    A_In = table.Column<double>(type: "double precision", nullable: false),
                    A_Out = table.Column<double>(type: "double precision", nullable: true),
                    Ri_In = table.Column<double>(type: "double precision", nullable: false),
                    Rc_In = table.Column<double>(type: "double precision", nullable: false),
                    Ri_Out = table.Column<double>(type: "double precision", nullable: true),
                    Rc_Out = table.Column<double>(type: "double precision", nullable: true),
                    A_L1_In = table.Column<double>(type: "double precision", nullable: true),
                    A_L2_In = table.Column<double>(type: "double precision", nullable: true),
                    A_L3_In = table.Column<double>(type: "double precision", nullable: true),
                    A_L1_Out = table.Column<double>(type: "double precision", nullable: true),
                    A_L2_Out = table.Column<double>(type: "double precision", nullable: true),
                    A_L3_Out = table.Column<double>(type: "double precision", nullable: true),
                    QI_QIV_InId = table.Column<long>(type: "bigint", nullable: true),
                    QII_QIII_OutId = table.Column<long>(type: "bigint", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerMeterTotal_AveragePower_QI_QIV_InId",
                        column: x => x.QI_QIV_InId,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTotal_AveragePower_QII_QIII_OutId",
                        column: x => x.QII_QIII_OutId,
                        principalTable: "AveragePower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerMeterTotal_Thing_ThingId",
                        column: x => x.ThingId,
                        principalTable: "Thing",
                        principalColumn: "ThingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_RFIDTagUID",
                table: "Alarm",
                column: "RFIDTagUID");

            migrationBuilder.CreateIndex(
                name: "IX_Alarm_ThingId",
                table: "Alarm",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_AveragePower_ClockId",
                table: "AveragePower",
                column: "ClockId");

            migrationBuilder.CreateIndex(
                name: "IX_Measure_ThingId",
                table: "Measure",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_ThingId",
                table: "Policy",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_ThingId",
                table: "Position",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterInstantaneous_ThingId",
                table: "PowerMeterInstantaneous",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_1Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_2Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_3Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_3Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_4Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_4Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_5Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_5Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_6Id",
                table: "PowerMeterTariff",
                column: "P_In_MAX_6Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_In_MAX_TId",
                table: "PowerMeterTariff",
                column: "P_In_MAX_TId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_1Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_2Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_3Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_3Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_4Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_4Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_5Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_5Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_6Id",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_6Id");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_P_Out_MAX_TId",
                table: "PowerMeterTariff",
                column: "P_Out_MAX_TId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTariff_ThingId",
                table: "PowerMeterTariff",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTotal_QI_QIV_InId",
                table: "PowerMeterTotal",
                column: "QI_QIV_InId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTotal_QII_QIII_OutId",
                table: "PowerMeterTotal",
                column: "QII_QIII_OutId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterTotal_ThingId",
                table: "PowerMeterTotal",
                column: "ThingId");

            migrationBuilder.CreateIndex(
                name: "IX_RFIDTag_AlarmUserId",
                table: "RFIDTag",
                column: "AlarmUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alarm");

            migrationBuilder.DropTable(
                name: "Measure");

            migrationBuilder.DropTable(
                name: "Policy");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "PowerMeterInstantaneous");

            migrationBuilder.DropTable(
                name: "PowerMeterTariff");

            migrationBuilder.DropTable(
                name: "PowerMeterTotal");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "WeatherForecast");

            migrationBuilder.DropTable(
                name: "RFIDTag");

            migrationBuilder.DropTable(
                name: "AveragePower");

            migrationBuilder.DropTable(
                name: "Thing");

            migrationBuilder.DropTable(
                name: "AlarmUser");

            migrationBuilder.DropTable(
                name: "PowerMeterClock");
        }
    }
}
