USE [InvoiceService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/18/2024 11:19:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoice_Products]    Script Date: 11/18/2024 11:19:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice_Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdInvoice] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Invoice_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 11/18/2024 11:19:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](12) NOT NULL,
	[IdPromotion] [int] NULL,
	[Price] [int] NOT NULL,
	[IdPaymentMethod] [int] NULL,
	[IdBranch] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[IsSuccess] [bit] NOT NULL,
	[CustomerPhoneNumber] [nvarchar](10) NULL,
	[CustomerName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentMethods]    Script Date: 11/18/2024 11:19:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethods](
	[Id] [int] NOT NULL,
	[Method] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PaymentMethods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241109161300_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241111034317_update1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241115165244_ud2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241116063003_up3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241116063135_up3', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Invoice_Products] ON 

INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (18, 12, 3, 15)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (19, 12, 10200, 4)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (20, 13, 3, 15)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (21, 13, 10200, 4)
SET IDENTITY_INSERT [dbo].[Invoice_Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Invoices] ON 

INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IsSuccess], [CustomerPhoneNumber], [CustomerName]) VALUES (12, N'PK8996604758', NULL, 0, 1, 2, CAST(N'2024-11-18T09:24:07.5195780' AS DateTime2), 0, N'0123456782', N'cus2name')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IsSuccess], [CustomerPhoneNumber], [CustomerName]) VALUES (13, N'PK2720447153', 2, 0, 1, 2, CAST(N'2024-11-18T09:55:58.5263679' AS DateTime2), 0, N'0123456782', N'cus2name')
SET IDENTITY_INSERT [dbo].[Invoices] OFF
GO
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (1, N'Nhận tại cửa hàng')
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (2, N'Ship đến nhà')
GO
ALTER TABLE [dbo].[Invoice_Products]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Products_Invoices_IdInvoice] FOREIGN KEY([IdInvoice])
REFERENCES [dbo].[Invoices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Invoice_Products] CHECK CONSTRAINT [FK_Invoice_Products_Invoices_IdInvoice]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_PaymentMethods_IdPaymentMethod] FOREIGN KEY([IdPaymentMethod])
REFERENCES [dbo].[PaymentMethods] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_PaymentMethods_IdPaymentMethod]
GO
