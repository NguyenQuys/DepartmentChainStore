USE [UserService]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 1/9/2025 4:04:13 PM ******/
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
/****** Object:  Table [dbo].[Role]    Script Date: 1/9/2025 4:04:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [tinyint] NOT NULL,
	[RoleName] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserOtherInfo]    Script Date: 1/9/2025 4:04:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOtherInfo](
	[UserId] [int] NOT NULL,
	[FullName] [nvarchar](60) NOT NULL,
	[Email] [nvarchar](30) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[Gender] [tinyint] NOT NULL,
	[RoleId] [tinyint] NOT NULL,
	[IdBranch] [nvarchar](4) NULL,
	[BeginDate] [date] NULL,
	[Salary] [int] NULL,
	[NumberOfIncorrectEntries] [tinyint] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[LoginTime] [datetime2](7) NULL,
	[LogoutTime] [datetime2](7) NULL,
	[UpdateBy] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_UserOtherInfo] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1/9/2025 4:04:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumber] [nvarchar](10) NOT NULL,
	[Password] [nvarchar](61) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241031190913_init', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241031192656_up1', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241031195050_up2', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241121063519_up3', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241206165414_up4', N'8.0.10')
GO
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (2, N'Manager')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (3, N'Employee')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (4, N'Customer')
GO
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (1, N'admin', N'quy@gmail.com', CAST(N'2024-09-06' AS Date), 1, 1, NULL, CAST(N'2024-09-06' AS Date), 200000, 8, CAST(N'2024-09-06T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-01T17:13:58.3487946' AS DateTime2), CAST(N'2025-01-01T17:14:00.3992467' AS DateTime2), 1, 1, N'abc')
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (14, N'managerVVN', N'user1@gmail.com', CAST(N'2024-10-30' AS Date), 0, 2, N'2', CAST(N'2024-11-15' AS Date), 12222, 0, CAST(N'2024-11-01T13:52:39.6217564' AS DateTime2), CAST(N'2024-12-25T20:26:27.2329975' AS DateTime2), CAST(N'2024-12-25T20:52:58.9717028' AS DateTime2), 0, 1, N'abc')
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (16, N'managerDVB', N'dvb@gmail.com', CAST(N'2024-11-14' AS Date), 0, 2, N'8', CAST(N'2024-11-13' AS Date), 50000, 0, CAST(N'2024-11-01T14:47:12.6877147' AS DateTime2), CAST(N'2024-11-02T20:20:10.0965242' AS DateTime2), CAST(N'2024-11-02T20:20:21.2028248' AS DateTime2), 0, 1, N'abc')
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (17, N'cus1name', N'cus1@gmail.com', CAST(N'2024-09-08' AS Date), 0, 4, NULL, NULL, NULL, 0, CAST(N'2024-12-27T16:48:41.3140834' AS DateTime2), NULL, NULL, 1, 0, N'abc')
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (18, N'cus2name', N'tranthihieu321@gmail.com', CAST(N'2024-09-08' AS Date), 0, 4, NULL, NULL, NULL, 1, CAST(N'2024-12-27T16:50:01.6351342' AS DateTime2), CAST(N'2025-01-01T17:14:06.6786755' AS DateTime2), NULL, 1, 1, N'156/11 Võ Văn Ngân, phường Bình Thọ, Thủ Đức')
INSERT [dbo].[UserOtherInfo] ([UserId], [FullName], [Email], [DateOfBirth], [Gender], [RoleId], [IdBranch], [BeginDate], [Salary], [NumberOfIncorrectEntries], [UpdatedAt], [LoginTime], [LogoutTime], [UpdateBy], [IsActive], [Address]) VALUES (22, N'em1vvn', N'em1vvn@gmail.com', CAST(N'2024-09-08' AS Date), 0, 3, N'2', CAST(N'2024-09-08' AS Date), 20000, 0, CAST(N'2024-09-08T00:00:00.0000000' AS DateTime2), CAST(N'2024-12-27T21:39:31.0874910' AS DateTime2), CAST(N'2024-12-27T21:39:42.4067614' AS DateTime2), 0, 1, N'')
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (1, N'admin', N'$2a$11$C/OZrmAwmC14fIs0hb2Sn.gHO3YBLEjuCe2kZKiERiyGs3qPSYmYG')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (14, N'VVNmanager', N'$2a$11$03nDNfet5CfR7TeVCOUB2OeKMcUzUUTgOveFq1RNpt8tvPqkGRt8e')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (15, N'1111111111', N'$2a$11$6v.U3R/.z1axs1BE5UAiFeJwskxR6c5CNYMgY.3MtP7sKivHAWzYa')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (16, N'DVBmanager', N'$2a$11$4H95DW8FIGWIMUjsBlX3Q.yB8ZN0IHQKTvMAkGvszFg0lu.G807I6')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (17, N'0123456781', N'$2a$11$4H95DW8FIGWIMUjsBlX3Q.yB8ZN0IHQKTvMAkGvszFg0lu.G807I6')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (18, N'0123456782', N'$2a$11$4H95DW8FIGWIMUjsBlX3Q.yB8ZN0IHQKTvMAkGvszFg0lu.G807I6')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (22, N'em1vvn', N'$2a$11$4H95DW8FIGWIMUjsBlX3Q.yB8ZN0IHQKTvMAkGvszFg0lu.G807I6')
INSERT [dbo].[Users] ([UserId], [PhoneNumber], [Password]) VALUES (73, N'0147852365', N'$2a$11$3Ce/aDl1nwh4Lpphw72xtO9JaNVnBuXlIh9or0WODyHWptzZ5o6cO')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[UserOtherInfo] ADD  DEFAULT (N'') FOR [Address]
GO
ALTER TABLE [dbo].[UserOtherInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserOtherInfo_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserOtherInfo] CHECK CONSTRAINT [FK_UserOtherInfo_Role_RoleId]
GO
ALTER TABLE [dbo].[UserOtherInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserOtherInfo_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserOtherInfo] CHECK CONSTRAINT [FK_UserOtherInfo_Users_UserId]
GO
