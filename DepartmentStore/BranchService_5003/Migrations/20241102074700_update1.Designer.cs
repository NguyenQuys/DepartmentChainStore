﻿// <auto-generated />
using System;
using BranchService_5003.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BranchService_5003.Migrations
{
    [DbContext(typeof(BranchDBContext))]
    [Migration("20241102074700_update1")]
    partial class update1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BranchService_5003.Models.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(61)
                        .HasColumnType("nvarchar(61)");

                    b.HasKey("Id");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("BranchService_5003.Models.ImportProductHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Consignee")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("IdBatch")
                        .HasColumnType("int");

                    b.Property<int>("IdBranch")
                        .HasColumnType("int");

                    b.Property<int>("IdProduct")
                        .HasColumnType("int");

                    b.Property<DateTime>("ImportTime")
                        .HasColumnType("datetime2");

                    b.Property<short>("Quantity")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("IdBranch");

                    b.ToTable("ExportProductHistories");
                });

            modelBuilder.Entity("BranchService_5003.Models.Product_Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdBatch")
                        .HasColumnType("int");

                    b.Property<int>("IdBranch")
                        .HasColumnType("int");

                    b.Property<int>("IdProduct")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdBranch");

                    b.ToTable("Product_Branches");
                });

            modelBuilder.Entity("BranchService_5003.Models.ImportProductHistory", b =>
                {
                    b.HasOne("BranchService_5003.Models.Branch", "Branch")
                        .WithMany("ExportProductHistory")
                        .HasForeignKey("IdBranch")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("BranchService_5003.Models.Product_Branch", b =>
                {
                    b.HasOne("BranchService_5003.Models.Branch", "Branch")
                        .WithMany("Product_Branches")
                        .HasForeignKey("IdBranch")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("BranchService_5003.Models.Branch", b =>
                {
                    b.Navigation("ExportProductHistory");

                    b.Navigation("Product_Branches");
                });
#pragma warning restore 612, 618
        }
    }
}
