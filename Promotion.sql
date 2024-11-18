USE [PromotionService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/18/2024 11:22:03 AM ******/
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
/****** Object:  Table [dbo].[Promotions]    Script Date: 11/18/2024 11:22:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[InitDate] [date] NOT NULL,
	[ExpiredDate] [date] NOT NULL,
	[Percentage] [tinyint] NOT NULL,
	[ApplyFor] [int] NULL,
	[MinPrice] [int] NOT NULL,
	[MaxPrice] [int] NOT NULL,
	[InitQuantity] [int] NOT NULL,
	[RemainingQuantity] [int] NOT NULL,
	[TimeUpdate] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IdPromotionType] [tinyint] NOT NULL,
 CONSTRAINT [PK_Promotions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionTypes]    Script Date: 11/18/2024 11:22:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionTypes](
	[Id] [tinyint] NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PromotionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241111030845_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241114160114_ud1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241114180920_ud2', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 

INSERT [dbo].[Promotions] ([Id], [Code], [InitDate], [ExpiredDate], [Percentage], [ApplyFor], [MinPrice], [MaxPrice], [InitQuantity], [RemainingQuantity], [TimeUpdate], [IsActive], [IdPromotionType]) VALUES (2, N'SPND', CAST(N'2024-09-08' AS Date), CAST(N'2024-11-20' AS Date), 20, 2, 0, 20000, 200, 198, CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2), 1, 3)
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
INSERT [dbo].[PromotionTypes] ([Id], [Type]) VALUES (1, N'Sản phẩm nhất định')
INSERT [dbo].[PromotionTypes] ([Id], [Type]) VALUES (2, N'Phân loại nhất định')
INSERT [dbo].[PromotionTypes] ([Id], [Type]) VALUES (3, N'Tất cả')
INSERT [dbo].[PromotionTypes] ([Id], [Type]) VALUES (4, N'Giảm ship')
GO
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD  CONSTRAINT [FK_Promotions_PromotionTypes_IdPromotionType] FOREIGN KEY([IdPromotionType])
REFERENCES [dbo].[PromotionTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Promotions] CHECK CONSTRAINT [FK_Promotions_PromotionTypes_IdPromotionType]
GO
