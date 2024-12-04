﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaToro.Novasoft.Fotos.Migrations.LogDb
{
    /// <inheritdoc />
    public partial class mssql_migration_744 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NovasoftFotos_LogDescargas",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_usr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_emple = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_emple = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecha_desc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovasoftFotos_LogDescargas", x => x.id_log);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NovasoftFotos_LogDescargas");
        }
    }
}