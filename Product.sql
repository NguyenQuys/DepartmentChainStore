USE [ProductService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/5/2024 2:23:24 PM ******/
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
/****** Object:  Table [dbo].[Batches]    Script Date: 11/5/2024 2:23:25 PM ******/
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
/****** Object:  Table [dbo].[CategoryProducts]    Script Date: 11/5/2024 2:23:25 PM ******/
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
/****** Object:  Table [dbo].[Images]    Script Date: 11/5/2024 2:23:25 PM ******/
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
/****** Object:  Table [dbo].[Products]    Script Date: 11/5/2024 2:23:25 PM ******/
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
GO
SET IDENTITY_INSERT [dbo].[Batches] ON 

INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (1, N'B01', 3, CAST(N'2024-11-05' AS Date), 200, 150, CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (2, N'B02', 3, CAST(N'2024-08-09' AS Date), 140, 70, CAST(N'2024-05-06T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
INSERT [dbo].[Batches] ([Id], [BatchNumber], [IdProduct], [ExpiryDate], [InitQuantity], [RemainingQuantity], [ImportDate], [Receiver]) VALUES (3, N'AA', 3, CAST(N'2024-11-02' AS Date), 500, 0, CAST(N'2024-05-06T00:00:00.0000000' AS DateTime2), N'Nguyễn Hữu Quý')
SET IDENTITY_INSERT [dbo].[Batches] OFF
GO
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (1, N'Nước')
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (2, N'Bánh')
INSERT [dbo].[CategoryProducts] ([Id], [Type]) VALUES (3, N'Trái Cây')
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (3, N'Nho', 50000, 3, 0, 1, CAST(N'2024-10-25T10:30:18.4019354' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10156, N'AAaaa', 4, 1, 0, 1, CAST(N'2024-10-31T22:57:45.1371805' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10172, N'er', 2, 1, 0, 1, CAST(N'2024-10-25T13:33:48.3595451' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10173, N'gf', 3, 1, 0, 1, CAST(N'2024-10-25T21:04:18.0168482' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10174, N'gf', 3, 1, 0, 1, CAST(N'2024-10-25T13:33:54.3590565' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10175, N'ds', 21, 1, 0, 1, CAST(N'2024-10-25T13:35:24.0163380' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10176, N'erw', 34, 1, 0, 1, CAST(N'2024-10-25T13:35:30.3567398' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10177, N'erw', 34, 1, 0, 1, CAST(N'2024-10-25T13:35:30.3543533' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10178, N'â', 12, 1, 0, 1, CAST(N'2024-10-25T13:37:46.1833717' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10180, N'ads', 12, 1, 0, 1, CAST(N'2024-10-25T13:51:53.0691031' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10182, N'p1', 30000, 1, 0, 1, CAST(N'2024-10-25T13:58:43.2162891' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10183, N'p2', 30000, 1, 0, 1, CAST(N'2024-10-25T13:58:43.2191644' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10184, N'p1', 30000, 1, 0, 1, CAST(N'2024-10-25T14:00:20.3590295' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10185, N'p2', 30000, 1, 0, 1, CAST(N'2024-10-25T14:00:20.3610763' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10186, N'moi them', 12, 1, 0, 1, CAST(N'2024-10-29T00:44:27.1677028' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10187, N'p1', 30000, 1, 0, 1, CAST(N'2024-11-03T13:52:22.4714878' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10188, N'p2', 30000, 1, 0, 1, CAST(N'2024-11-03T13:52:22.4725242' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10189, N'p1', 30000, 1, 0, 1, CAST(N'2024-11-03T13:56:40.2627022' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10190, N'p2', 30000, 1, 0, 1, CAST(N'2024-11-03T13:56:40.2634758' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10191, N'p1', 30000, 1, 0, 1, CAST(N'2024-11-03T13:57:01.9048089' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10192, N'p2', 30000, 1, 0, 1, CAST(N'2024-11-03T13:57:01.9054019' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10193, N'p1', 30000, 1, 0, 1, CAST(N'2024-11-03T14:01:03.9995339' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10194, N'p2', 30000, 1, 0, 1, CAST(N'2024-11-03T14:01:04.0000983' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10197, N'p1', 30000, 1, 0, 1, CAST(N'2024-11-03T15:33:27.6941961' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10198, N'p2', 30000, 1, 0, 1, CAST(N'2024-11-03T15:33:34.4529765' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10199, N'Test', 123123, 1, 0, 1, CAST(N'2024-11-03T17:04:00.8677380' AS DateTime2))
INSERT [dbo].[Products] ([Id], [ProductName], [Price], [CategoryId], [IsHide], [UpdatedBy], [UpdatedTime]) VALUES (10200, N'Test', 123123, 1, 0, 1, CAST(N'2024-11-03T17:08:43.3591342' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
ALTER TABLE [dbo].[Batches] ADD  DEFAULT (N'') FOR [Receiver]
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
