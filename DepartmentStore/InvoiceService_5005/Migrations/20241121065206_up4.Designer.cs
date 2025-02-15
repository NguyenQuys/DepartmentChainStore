﻿// <auto-generated />
using System;
using InvoiceService_5005.InvoiceModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InvoiceService_5005.Migrations
{
    [DbContext(typeof(InvoiceDbContext))]
    [Migration("20241121065206_up4")]
    partial class up4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CustomerNote")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CustomerPhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("EmployeeShip")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("IdBranch")
                        .HasColumnType("int");

                    b.Property<int?>("IdPaymentMethod")
                        .HasColumnType("int");

                    b.Property<int?>("IdPromotion")
                        .HasColumnType("int");

                    b.Property<short>("IdStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<int>("Price")
                        .HasMaxLength(9)
                        .HasColumnType("int");

                    b.Property<string>("StoreNote")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("IdPaymentMethod");

                    b.HasIndex("IdStatus");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Invoice_Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdInvoice")
                        .HasColumnType("int");

                    b.Property<int>("IdProduct")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdInvoice");

                    b.ToTable("Invoice_Products");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Status", b =>
                {
                    b.Property<short>("Id")
                        .HasColumnType("smallint");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Invoice", b =>
                {
                    b.HasOne("InvoiceService_5005.InvoiceModels.PaymentMethod", "PaymentMethod")
                        .WithMany("Invoices")
                        .HasForeignKey("IdPaymentMethod")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("InvoiceService_5005.InvoiceModels.Status", "Status")
                        .WithMany("Invoices")
                        .HasForeignKey("IdStatus")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PaymentMethod");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Invoice_Product", b =>
                {
                    b.HasOne("InvoiceService_5005.InvoiceModels.Invoice", "Invoice")
                        .WithMany("Invoice_Products")
                        .HasForeignKey("IdInvoice")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Invoice", b =>
                {
                    b.Navigation("Invoice_Products");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.PaymentMethod", b =>
                {
                    b.Navigation("Invoices");
                });

            modelBuilder.Entity("InvoiceService_5005.InvoiceModels.Status", b =>
                {
                    b.Navigation("Invoices");
                });
#pragma warning restore 612, 618
        }
    }
}
