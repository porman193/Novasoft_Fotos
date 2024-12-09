﻿// <auto-generated />
using System;
using CasaToro.Novasoft.Fotos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CasaToro.Novasoft.Fotos.Migrations
{
    [DbContext(typeof(NovasoftDbContext))]
    [Migration("20241206151820_mssql_migration_324")]
    partial class mssql_migration_324
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CasaToro.Novasoft.Fotos.Models.BusinessUnit", b =>
                {
                    b.Property<string>("code")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("codigo");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nombre");

                    b.HasKey("code");

                    b.ToTable("gen_clasif3");
                });

            modelBuilder.Entity("CasaToro.Novasoft.Fotos.Models.Employee", b =>
                {
                    b.Property<string>("idEmployee")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("cod_emp");

                    b.Property<string>("codeBusinessUnit")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("cod_cl3");

                    b.Property<DateTime?>("dischargeDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("fec_egr");

                    b.Property<DateTime?>("entryDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("fec_ing");

                    b.Property<string>("nameEmployee")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nom_emp");

                    b.Property<byte[]>("photoEmployee")
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("fto_emp");

                    b.Property<string>("surname1Employee")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ap1_emp");

                    b.Property<string>("surname2Employee")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ap2_emp");

                    b.HasKey("idEmployee");

                    b.ToTable("rhh_emplea");
                });
#pragma warning restore 612, 618
        }
    }
}