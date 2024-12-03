﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductService_5000.Models;

#nullable disable

namespace ProductService_5000.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20241005175329_udpdate2")]
    partial class udpdate2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductService_5000.Models.CategoryProduct", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("CategoryProducts");
                });

            modelBuilder.Entity("ProductService_5000.Models.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdProduct")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("IdProduct");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ProductService_5000.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte>("CategoryId")
                        .HasColumnType("tinyint");

                    b.Property<bool>("IsHide")
                        .HasColumnType("bit");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ProductService_5000.Models.Image", b =>
                {
                    b.HasOne("ProductService_5000.Models.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("IdProduct")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductService_5000.Models.Product", b =>
                {
                    b.HasOne("ProductService_5000.Models.CategoryProduct", "CategoryProduct")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategoryProduct");
                });

            modelBuilder.Entity("ProductService_5000.Models.CategoryProduct", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ProductService_5000.Models.Product", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
