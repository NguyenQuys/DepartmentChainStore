USE [InvoiceService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/21/2024 1:26:29 AM ******/
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
/****** Object:  Table [dbo].[Invoice_Products]    Script Date: 11/21/2024 1:26:29 AM ******/
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
/****** Object:  Table [dbo].[Invoices]    Script Date: 11/21/2024 1:26:29 AM ******/
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
	[CustomerPhoneNumber] [nvarchar](10) NULL,
	[CustomerName] [nvarchar](50) NULL,
	[IdStatus] [smallint] NOT NULL,
	[Note] [nvarchar](max) NULL,
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentMethods]    Script Date: 11/21/2024 1:26:29 AM ******/
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
/****** Object:  Table [dbo].[Status]    Script Date: 11/21/2024 1:26:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [smallint] NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](120) NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241109161300_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241111034317_update1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241115165244_ud2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241116063003_up3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241116063135_up3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241118072232_up4', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241118073140_up5', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241118073854_up6', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241118074000_up7', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Invoice_Products] ON 

INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (18, 12, 3, 15)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (19, 12, 10200, 4)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (20, 13, 3, 15)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (21, 13, 10200, 4)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (22, 14, 10200, 4)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (23, 15, 10200, 4)
SET IDENTITY_INSERT [dbo].[Invoice_Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Invoices] ON 

INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [CustomerPhoneNumber], [CustomerName], [IdStatus], [Note]) VALUES (12, N'PK8996604758', NULL, 890000, 1, 2, CAST(N'2024-11-18T09:24:07.5195780' AS DateTime2), N'0123456782', N'cus2name', 2, N'')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [CustomerPhoneNumber], [CustomerName], [IdStatus], [Note]) VALUES (13, N'PK2720447153', 2, 0, 1, 2, CAST(N'2024-11-18T09:55:58.5263679' AS DateTime2), N'0123456782', N'cus2name', 5, N'')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [CustomerPhoneNumber], [CustomerName], [IdStatus], [Note]) VALUES (14, N'PK3594045620', 2, 0, 1, 2, CAST(N'2024-11-19T11:06:54.1594047' AS DateTime2), N'0123456782', N'cus2name', 1, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [CustomerPhoneNumber], [CustomerName], [IdStatus], [Note]) VALUES (15, N'PK3136082940', NULL, 160000, 1, 2, CAST(N'2024-11-19T11:22:33.2402843' AS DateTime2), N'0123456782', N'cus2name', 1, NULL)
SET IDENTITY_INSERT [dbo].[Invoices] OFF
GO
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (1, N'Nhận tại cửa hàng')
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (2, N'Ship đến nhà')
GO
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (1, N'Đang chờ xử lý', N'Đơn hàng đã được đặt nhưng chưa được xử lý hoặc chưa xác nhận')
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (2, N'Đã đóng gói', N'Đơn hàng đã được đóng gói và sẵn sàng để giao')
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (3, N'Đang giao hàng', N'Đơn hàng đã được giao thành công đến khách hàng')
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (4, N'Đã giao hàng', N' Đơn hàng đã được giao thành công đến khách hàng')
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (5, N'Đã hủy bởi cửa hàng', N'Đơn hàng đã bị hủy bởi khách hàng hoặc bởi hệ thống')
INSERT [dbo].[Status] ([Id], [Type], [Description]) VALUES (6, N'Đã bị hủy bởi khách', N'Đơn hàng bị hủy do khách hàng ko muốn mua nữa')
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (CONVERT([smallint],(0))) FOR [IdStatus]
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
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Status_IdStatus] FOREIGN KEY([IdStatus])
REFERENCES [dbo].[Status] ([Id])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Status_IdStatus]
GO
