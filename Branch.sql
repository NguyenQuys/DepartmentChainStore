USE [BranchService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/21/2024 1:25:56 AM ******/
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
/****** Object:  Table [dbo].[Branches]    Script Date: 11/21/2024 1:25:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Location] [nvarchar](max) NOT NULL,
	[Account] [nvarchar](10) NOT NULL,
	[Password] [nvarchar](61) NOT NULL,
	[Latitude] [nvarchar](20) NOT NULL,
	[Longtitude] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Branches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportProductHistories]    Script Date: 11/21/2024 1:25:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportProductHistories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdBranch] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
	[IdBatch] [int] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[Consignee] [nvarchar](20) NOT NULL,
	[ImportTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ImportProductHistories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Branches]    Script Date: 11/21/2024 1:25:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Branches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdBranch] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
	[IdBatch] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Product_Branches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241024165550_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241102065327_update1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241102070804_update2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241102074700_update1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241102075317_update3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241109150635_update2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241112174415_ud2', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Branches] ON 

INSERT [dbo].[Branches] ([Id], [Location], [Account], [Password], [Latitude], [Longtitude]) VALUES (2, N'Võ Văn Ngân', N'vvn123', N'$2a$11$C/OZrmAwmC14fIs0hb2Sn.gHO3YBLEjuCe2kZKiERiyGs3qPSYmYG', N'10.851895', N'106.7662451')
INSERT [dbo].[Branches] ([Id], [Location], [Account], [Password], [Latitude], [Longtitude]) VALUES (8, N'Đặng Văn Bi', N'dvb123', N'$2a$11$9aryabz2j8hGwhtkSRo4Ce.GSfJ8hJ6pPRiaENP9Hv5INKZvI7TVu', N'10.770706154510874', N'106.61914493090823')
SET IDENTITY_INSERT [dbo].[Branches] OFF
GO
SET IDENTITY_INSERT [dbo].[ImportProductHistories] ON 

INSERT [dbo].[ImportProductHistories] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity], [Consignee], [ImportTime]) VALUES (1, 8, 3, 1, 100, N'Quys', CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[ImportProductHistories] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity], [Consignee], [ImportTime]) VALUES (2, 2, 3, 1, 500, N'Nam', CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[ImportProductHistories] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity], [Consignee], [ImportTime]) VALUES (3, 2, 3, 1, 500, N'Nam', CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[ImportProductHistories] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity], [Consignee], [ImportTime]) VALUES (4, 2, 3, 1, 500, N'Nam', CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[ImportProductHistories] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity], [Consignee], [ImportTime]) VALUES (5, 2, 3, 1, 500, N'Nam', CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[ImportProductHistories] OFF
GO
SET IDENTITY_INSERT [dbo].[Product_Branches] ON 

INSERT [dbo].[Product_Branches] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity]) VALUES (2, 2, 3, 1, 100)
INSERT [dbo].[Product_Branches] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity]) VALUES (3, 2, 3, 2, 200)
INSERT [dbo].[Product_Branches] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity]) VALUES (4, 2, 10200, 3, 400)
SET IDENTITY_INSERT [dbo].[Product_Branches] OFF
GO
ALTER TABLE [dbo].[Branches] ADD  DEFAULT (N'') FOR [Latitude]
GO
ALTER TABLE [dbo].[Branches] ADD  DEFAULT (N'') FOR [Longtitude]
GO
ALTER TABLE [dbo].[ImportProductHistories]  WITH CHECK ADD  CONSTRAINT [FK_ImportProductHistories_Branches_IdBranch] FOREIGN KEY([IdBranch])
REFERENCES [dbo].[Branches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ImportProductHistories] CHECK CONSTRAINT [FK_ImportProductHistories_Branches_IdBranch]
GO
ALTER TABLE [dbo].[Product_Branches]  WITH CHECK ADD  CONSTRAINT [FK_Product_Branches_Branches_IdBranch] FOREIGN KEY([IdBranch])
REFERENCES [dbo].[Branches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product_Branches] CHECK CONSTRAINT [FK_Product_Branches_Branches_IdBranch]
GO
