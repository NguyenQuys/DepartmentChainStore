USE [BranchService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/2/2024 1:44:40 PM ******/
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
/****** Object:  Table [dbo].[Branches]    Script Date: 11/2/2024 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Location] [nvarchar](max) NOT NULL,
	[Account] [nvarchar](10) NOT NULL,
	[Password] [nvarchar](61) NOT NULL,
 CONSTRAINT [PK_Branches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Branches]    Script Date: 11/2/2024 1:44:40 PM ******/
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
GO
SET IDENTITY_INSERT [dbo].[Branches] ON 

INSERT [dbo].[Branches] ([Id], [Location], [Account], [Password]) VALUES (2, N'Võ Văn Ngân', N'vvn123', N'$2a$11$C/OZrmAwmC14fIs0hb2Sn.gHO3YBLEjuCe2kZKiERiyGs3qPSYmYG')
INSERT [dbo].[Branches] ([Id], [Location], [Account], [Password]) VALUES (8, N'Đặng Văn Bi', N'dvb123', N'$2a$11$9aryabz2j8hGwhtkSRo4Ce.GSfJ8hJ6pPRiaENP9Hv5INKZvI7TVu')
SET IDENTITY_INSERT [dbo].[Branches] OFF
GO
SET IDENTITY_INSERT [dbo].[Product_Branches] ON 

INSERT [dbo].[Product_Branches] ([Id], [IdBranch], [IdProduct], [IdBatch], [Quantity]) VALUES (2, 2, 3, 1, 100)
SET IDENTITY_INSERT [dbo].[Product_Branches] OFF
GO
ALTER TABLE [dbo].[Product_Branches]  WITH CHECK ADD  CONSTRAINT [FK_Product_Branches_Branches_IdBranch] FOREIGN KEY([IdBranch])
REFERENCES [dbo].[Branches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product_Branches] CHECK CONSTRAINT [FK_Product_Branches_Branches_IdBranch]
GO
