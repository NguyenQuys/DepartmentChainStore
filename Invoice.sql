USE [InvoiceService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 1/9/2025 4:02:24 PM ******/
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
/****** Object:  Table [dbo].[Invoice_Products]    Script Date: 1/9/2025 4:02:24 PM ******/
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
/****** Object:  Table [dbo].[Invoices]    Script Date: 1/9/2025 4:02:24 PM ******/
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
	[IdStatus] [smallint] NOT NULL,
	[CustomerPhoneNumber] [nvarchar](10) NULL,
	[CustomerName] [nvarchar](50) NULL,
	[IdEmployeeShip] [int] NULL,
	[CustomerNote] [nvarchar](100) NULL,
	[StoreNote] [nvarchar](100) NULL,
	[Address] [nvarchar](200) NULL,
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentMethods]    Script Date: 1/9/2025 4:02:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethods](
	[Id] [int] NOT NULL,
	[Method] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_PaymentMethods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 1/9/2025 4:02:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [smallint] NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121025729_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121030656_up1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121030729_up2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121030830_up3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121065206_up4', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121160306_up5', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241122170934_up6', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241122171235_up6', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241206160118_up7', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Invoice_Products] ON 

INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (67, 34, 10200, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (68, 34, 10221, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (69, 35, 10200, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (70, 35, 10221, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (71, 36, 10200, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (72, 36, 10221, 3)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (73, 37, 3, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (74, 37, 10200, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (75, 37, 10219, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (76, 37, 10221, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (79, 40, 10221, 3)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (80, 41, 3, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (81, 41, 10221, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (82, 42, 3, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (83, 42, 10221, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (84, 43, 10200, 2)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (85, 43, 10221, 3)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (86, 44, 3, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (87, 44, 10200, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (88, 44, 10219, 1)
INSERT [dbo].[Invoice_Products] ([Id], [IdInvoice], [IdProduct], [Quantity]) VALUES (89, 44, 10221, 1)
SET IDENTITY_INSERT [dbo].[Invoice_Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Invoices] ON 

INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (34, N'PK3560512122', NULL, 110000, 1, 2, CAST(N'2024-12-24T20:22:20.8624759' AS DateTime2), 4, N'0258963147', N'cus2name', NULL, N'nho chuan bi', NULL, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (35, N'PK2462465481', NULL, 110000, 1, 2, CAST(N'2024-12-25T20:22:21.3470221' AS DateTime2), 1, N'0258963147', N'cus2name', NULL, N'nho chuan bi', NULL, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (36, N'SH6478929636', NULL, 123000, 2, 2, CAST(N'2024-12-25T20:23:26.5658281' AS DateTime2), 4, N'0123456782', N'cus2name', 22, NULL, NULL, N'156/11 Võ Văn Ngân, phường Bình Thọ, Thủ Đức')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (37, N'DY3156523653', NULL, 175000, 3, 2, CAST(N'2024-12-25T13:24:14.2672956' AS DateTime2), 4, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (40, N'SH9445841733', NULL, 54000, 2, 2, CAST(N'2024-12-27T21:32:30.6316125' AS DateTime2), 4, N'0123456782', N'cus2name', 22, NULL, NULL, N'156/11 Võ Văn Ngân, phường Bình Thọ, Thủ Đức')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (41, N'PK0013523381', NULL, 65000, 1, 2, CAST(N'2024-12-28T10:52:57.9416622' AS DateTime2), 1, N'0111111111', N'cus2name', NULL, NULL, NULL, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (42, N'PK9932775112', NULL, 65000, 1, 2, CAST(N'2024-12-28T10:52:59.0382782' AS DateTime2), 1, N'0111111111', N'cus2name', NULL, NULL, NULL, NULL)
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (43, N'SH9716744594', 2, 123000, 2, 2, CAST(N'2024-12-28T10:54:33.5687909' AS DateTime2), 1, N'0123456782', N'cus2name', NULL, NULL, NULL, N'156/11 Võ Văn Ngân, phường Bình Thọ, Thủ Đức')
INSERT [dbo].[Invoices] ([Id], [InvoiceNumber], [IdPromotion], [Price], [IdPaymentMethod], [IdBranch], [CreatedDate], [IdStatus], [CustomerPhoneNumber], [CustomerName], [IdEmployeeShip], [CustomerNote], [StoreNote], [Address]) VALUES (44, N'DY2021070115', NULL, 125000, 3, 2, CAST(N'2024-12-28T03:56:52.6190736' AS DateTime2), 4, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Invoices] OFF
GO
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (1, N'Nhận tại cửa hàng')
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (2, N'Ship tận nơi')
INSERT [dbo].[PaymentMethods] ([Id], [Method]) VALUES (3, N'Mua trực tiếp')
GO
INSERT [dbo].[Status] ([Id], [Type]) VALUES (1, N'Đang chờ xử lý')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (2, N'Đang giao hàng')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (3, N'Giao hàng thành công')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (4, N'Đã hoàn tất')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (5, N'Hủy bởi cửa hàng')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (6, N'Hủy bởi khách hàng')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (7, N'Hủy bởi shipper')
INSERT [dbo].[Status] ([Id], [Type]) VALUES (8, N'Chưa nhận được đơn')
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
