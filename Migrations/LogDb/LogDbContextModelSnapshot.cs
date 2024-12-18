﻿// <auto-generated />
using System;
using CasaToro.Novasoft.Fotos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CasaToro.Novasoft.Fotos.Migrations.LogDb
{
    [DbContext(typeof(LogDbContext))]
    partial class LogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CasaToro.Novasoft.Fotos.Models.Card", b =>
                {
                    b.Property<string>("idEmployee")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id_emple");

                    b.Property<DateTime?>("deliveryDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("fecha_entrega");

                    b.Property<string>("nameEmployee")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nombre_emple");

                    b.Property<DateTime>("registrationDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("fecha_registro");

                    b.HasKey("idEmployee");

                    b.ToTable("NovasoftFotos_RegistroCarnet");
                });

            modelBuilder.Entity("CasaToro.Novasoft.Fotos.Models.Log", b =>
                {
                    b.Property<int>("idLog")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_log");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idLog"));

                    b.Property<DateTime>("downloadDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("fecha_desc");

                    b.Property<string>("idEmployee")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("id_emple");

                    b.Property<string>("idUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("id_usr");

                    b.Property<string>("nameEmployee")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nombre_emple");

                    b.Property<string>("nameUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nombre_usr");

                    b.HasKey("idLog");

                    b.ToTable("NovasoftFotos_LogDescargas");
                });
#pragma warning restore 612, 618
        }
    }
}
