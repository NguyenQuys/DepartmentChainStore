USE [ProductService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/18/2024 11:21:20 AM ******/
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
/****** Object:  Table [dbo].[Batches]    Script Date: 11/18/2024 11:21:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchNumber] [nvarchar](10) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[ExpiryDate] [date] NOT NULL,
	[InitQuantity] [int] NOT NULL,
	[RemainingQuantity] [int] NOT NULL,
	[ImportDate] [datetime2](7) NOT NULL,
	[Receiver] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Carts]    Script Date: 11/18/2024 11:21:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Carts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[IdUser] [int] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[IdBranch] [int] NOT NULL,
 CONSTRAINT [PK_Carts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CategoryProducts]    Script Date: 11/18/2024 11:21:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoryProducts](
	[Id] [tinyint] NOT NULL,
	[Type] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_CategoryProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Images]    Script Date: 11/18/2024 11:21:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Images](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[ImagePath] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/18/2024 11:21:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](30) NOT NULL,
	[Price] [int] NOT NULL,
	[CategoryId] [tinyint] NOT NULL,
	[IsHide] [bit] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[UpdatedTime] [datetime2](7) NOT NULL,
	[MainImage] [nvarchar](100) NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241004180530_init', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241005174701_udpdate1', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241005175329_udpdate2', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241027191744_update3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241028170612_update4', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241029043339_update6', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241029091411_update7', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105072845_update5', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241105083321_update8', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241107084922_update9', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241107090118_update10', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241107143739_update9', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241108174355_update10', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Batches] ON 

INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (1, N'B01', 3, CAST(N'2024-11-05' AS Date), 200, 150, CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (2, N'B02', 3, CAST(N'2024-08-09' AS Date), 140, 70, CAST(N'2024-05-06T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (3, N'AA', 3, CAST(N'2024-11-02' AS Date), 500, 0, CAST(N'2024-05-06T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
SET IDENTITY_INSERT [dbo].[Batches] OFF
GO
SET IDENTITY_INSERT [dbo].[Carts] ON 

INSERT [dbo].[Carts] ([Id], [IdProduct], [IdUser], [Quantity], [IdBranch]) VALUES (1, 3, 18, 15, 2)
INSERT [dbo].[Carts] ([Id], [IdProduct], [IdUser], [Quantity], [IdBranch]) VALUES (3, 10200, 18, 4, 2)
INSERT [dbo].[Carts] ([Id], [IdProduct], [IdUser], [Quantity], [IdBranch]) VALUES (4, 3, 1, 1, 2)
INSERT [dbo].[Carts] ([Id], [IdProduct], [IdUser], [Quantity], [IdBranch]) VALUES (5, 10200, 1, 1, 2)
SET IDENTITY_INSERT [dbo].[Carts] OFF
GO
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (1, N'Nước')
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (2, N'Bánh')
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (3, N'Trái Cây')
GO
SET IDENTITY_INSERT [dbo].[Images] ON 

INSERT [dbo].[Images] ([Id], [IdProduct], [ImagePath]) VALUES (37, 10208, N'/product/images/43b97a8c-aa39-4f76-86dc-0e027da416b2.jpg')
INSERT [dbo].[Images] ([Id], [IdProduct], [ImagePath]) VALUES (38, 10208, N'/product/images/00554b17-f3f2-46fd-bed1-81dd04cb75e0.jpg')
INSERT [dbo].[Images] ([Id], [IdProduct], [ImagePath]) VALUES (39, 10209, N'/product/images/279476351_799868287658883_6513972129707658847_n.jpg')
INSERT [dbo].[Images] ([Id], [IdProduct], [ImagePath]) VALUES (40, 10209, N'/product/images/324042873_892292125299907_176441753814452640_n.jpg')
SET IDENTITY_INSERT [dbo].[Images] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime], [MainImage]) VALUES (3, N'Nho', 50000, 3, 0, 1, CAST(N'2024-11-15T11:07:12.2614393' AS DateTime2), N'/product/images/279455187_594598114865951_7440947279832479676_n.jpg')
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime], [MainImage]) VALUES (10200, N'Bánh', 40000, 2, 0, 1, CAST(N'2024-11-05T15:14:03.0176418' AS DateTime2), N'/product/images/279455187_594598114865951_7440947279832479676_n.jpg')
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime], [MainImage]) VALUES (10208, N'Pepsi', 30000, 1, 0, 1, CAST(N'2024-11-15T11:07:28.4537687' AS DateTime2), N'/product/images/279455187_594598114865951_7440947279832479676_n.jpg')
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime], [MainImage]) VALUES (10209, N'PepsiAA', 20000, 1, 0, 1, CAST(N'2024-11-15T10:10:17.1261221' AS DateTime2), N'/product/images/279455187_594598114865951_7440947279832479676_n.jpg')
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime], [MainImage]) VALUES (10211, N'MMMMM', 1321321, 2, 0, 1, CAST(N'2024-11-14T09:30:02.3834738' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
ALTER TABLE [dbo].[Batches] ADD  DEFAULT (N'') FOR [Receiver]
GO
ALTER TABLE [dbo].[Carts] ADD  DEFAULT ((0)) FOR [IdBranch]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [UpdatedBy]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [UpdatedTime]
GO
ALTER TABLE [dbo].[Batches]  WITH CHECK ADD  CONSTRAINT [FK_Batches_Products_IdProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Batches] CHECK CONSTRAINT [FK_Batches_Products_IdProduct]
GO
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_Carts_Products_IdProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_Carts_Products_IdProduct]
GO
ALTER TABLE [dbo].[Images]  WITH CHECK ADD  CONSTRAINT [FK_Images_Products_IdProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Images] CHECK CONSTRAINT [FK_Images_Products_IdProduct]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_CategoryProducts_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[CategoryProducts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_CategoryProducts_CategoryId]
GO
