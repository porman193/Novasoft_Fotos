using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaToro.Novasoft.Fotos.Migrations
{
    /// <inheritdoc />
    public partial class mssql_migration_717 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gen_clasif3",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gen_clasif3", x => x.codigo);
                });

            migrationBuilder.CreateTable(
                name: "rhh_emplea",
                columns: table => new
                {
                    cod_emp = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ap1_emp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ap2_emp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nom_emp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fec_ing = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fec_egr = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fto_emp = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    cod_cl3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rhh_emplea", x => x.cod_emp);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gen_clasif3");

            migrationBuilder.DropTable(
                name: "rhh_emplea");
        }
    }
}
