﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataProcessorService.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20231128133027_CreateDatabase")]
    partial class CreateDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("ModuleCategory", b =>
                {
                    b.Property<string>("ModuleCategoryID")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ModuleCategoryID");

                    b.ToTable("ModuleCategories");
                });
#pragma warning restore 612, 618
        }
    }
}