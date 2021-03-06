CREATE DATABASE [Report_Acunetix]
GO

USE [Report_Acunetix]
GO


CREATE TABLE [dbo].[Website](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	StartURL [nvarchar](250) NULL,
	Host [nvarchar](250) NULL,
	Responsive [nvarchar](250) NULL,
	ServerOS [nvarchar](250) NULL,
	ServerInformation [nvarchar](250) NULL,
	ServerTechnologies [nvarchar](250) NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_Website] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].WebsiteItem(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	WebsiteID int NOT NULL,
	LastScanID int NOT NULL,
	FullPath [nvarchar](2000) NULL,
	ErrorCount int NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_WebsiteItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[WebsiteScan](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	WebsiteID int NOT NULL,
	StartTime [datetime] NOT NULL,
	ScanTime [nvarchar](250) NULL,
	ThreatLevel [nvarchar](250) NULL,
	TotalSecond int NULL,
	TotalItemScan int NULL,
	TotalAlertFound int NULL,
	HighAlert int NULL,
	MediumAlert int NULL,
	LowAlert int NULL,
	InforAlert int NULL,
	ScanProfile [nvarchar](250) NULL,
	ScanStatus [nvarchar](250) NULL,
	RootReportUrl [nvarchar](255) NULL,
	NewReportUrl [nvarchar](255) NULL,
	ReportHTML TEXT NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_WebiteScan] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[AffectedItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	WebsiteID int NOT NULL,
	WebiteScanID int NOT NULL,
	AlertGroupID int NOT NULL,
	ScanPath [nvarchar](2000) NULL,
	Detail [nvarchar](MAX) NULL,
	SourceCode [nvarchar](MAX) NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_AffectedItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[AlertGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	AlertName [nvarchar](250) NOT NULL,
	AlertNameTrans [nvarchar](250) NULL,
	Severity [nvarchar](50) NULL,
	AlertDescription [nvarchar](MAX) NULL,
	AlertDescriptionTrans [nvarchar](MAX) NULL,
	Recommendations [nvarchar](2000) NULL,
	RecommendationsTrans [nvarchar](2000) NULL,
	AlertVariants [nvarchar](250) NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_AlertGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].ScannedItem(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	WebsiteID int NOT NULL,
	WebiteScanID int NOT NULL,
	WebsiteItemID int NOT NULL,
	FullPath [nvarchar](2000) NULL,
	DateCreated [datetime] DEFAULT GETDATE(),
	Status [bit] DEFAULT 1,
 CONSTRAINT [PK_ScannedItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].TuDienVietHoa (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	NgonNgu nvarchar(50) NULL,
	CumTuKhoa [nvarchar](MAX) NULL,
	CumTuThayDoi [nvarchar](MAX) NULL,
	NguoiTaoID int NULL,
	NguoiCapNhatID int NULL,
	[NgayTao] [datetime] DEFAULT GETDATE(),
	[TrangThai] [bit] DEFAULT 1,
 CONSTRAINT [PK_TuDienVietHoa] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ConvertDelimitedListIntoTable] (
     @list NVARCHAR(MAX) ,@delimiter CHAR(1) )
RETURNS @table TABLE ( 
     item VARCHAR(255) NOT NULL )
AS 
    BEGIN
        DECLARE @pos INT ,@nextpos INT ,@valuelen INT

        SELECT  @pos = 0 ,@nextpos = 1

        WHILE @nextpos > 0 
            BEGIN
                SELECT  @nextpos = CHARINDEX(@delimiter,@list,@pos + 1)
                SELECT  @valuelen = CASE WHEN @nextpos > 0 THEN @nextpos
                                         ELSE LEN(@list) + 1
                                    END - @pos - 1
                INSERT  @table ( item )
                VALUES  ( CONVERT(INT,SUBSTRING(@list,@pos + 1,@valuelen)) )
                SELECT  @pos = @nextpos

            END

        DELETE  FROM @table
        WHERE   item = ''

        RETURN 
    END
GO

/****** Object:  Table [dbo].[HeThongThamSo]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HeThongThamSo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MaThamSo] [varchar](50) NULL,
	[TenThamSo] [nvarchar](250) NULL,
	[MoTa] [nvarchar](500) NULL,
	[NguoiCapNhatID] [int] NULL,
	[NgayCapNhat] [datetime] NULL,
	[TrangThai] [bit] NOT NULL,
 CONSTRAINT [PK_HeThongThamSo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NguoiDung]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NguoiDung](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HoVaTen] [nvarchar](250) NULL,
	[NgaySinh] [datetime] NULL,
	[GioiTinh] [bit] NULL,
	[NhomQuyenID] [int] NULL,
	[NhomQuyenList] [nvarchar](500) NULL,
	[ChucVuID] [int] NULL,
	[DonViID] [int] NULL,
	[TenDangNhap] [varchar](100) NULL,
	[MatKhau] [varchar](500) NULL,
	[AnhDaiDien] [varchar](500) NULL,
	[Email] [varchar](250) NULL,
	[SoDienThoai] [varchar](250) NULL,
	[DiaChi] [nvarchar](500) NULL,
	[Fax] [nvarchar](100) NULL,
	[NguoiTao] [varchar](50) NULL,
	[NgayTao] [datetime] NULL,
	[NguoiTaoID] [int] NULL,
	[NguoiCapNhatID] [int] NULL,
	[NguoiCapNhat] [varchar](50) NULL,
	[NgayCapNhat] [datetime] NULL,
	[ThuTu] [int] NULL,
	[TrangThai] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_NguoiDung] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NhatKyHeThong]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NhatKyHeThong](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NguoiDungId] [int] NULL,
	[MoTa] [nvarchar](2000) NULL,
	[NgayTao] [datetime] NULL,
	[IpClient] [varchar](50) NULL,
 CONSTRAINT [PK_HeThongLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[NhomNguoiDung]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NhomNguoiDung](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TenNhom] [nvarchar](250) NULL,
	[ListNguoiDungThuocNhomID] [varchar](max) NULL,
	[PhamViSuDung] [bit] NOT NULL,
	[NguoiTao] [varchar](50) NULL,
	[NgayTao] [datetime] NULL,
	[NguoiTaoID] [int] NULL,
	[NguoiCapNhatID] [int] NULL,
	[NguoiCapNhat] [varchar](50) NULL,
	[NgayCapNhat] [datetime] NULL,
	[TrangThai] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_NhomNguoiDung_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NhomQuyen]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[NhomQuyen](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[KyHieu] [nvarchar](50) NULL,
	[MaNhom] [varchar](50) NULL,
	[TenNhom] [nvarchar](250) NULL,
	[GhiChu] [nvarchar](500) NULL,
	[NguoiTao] [varchar](50) NULL,
	[NgayTao] [datetime] NULL,
	[NguoiTaoID] [int] NULL,
	[NguoiCapNhatID] [int] NULL,
	[NguoiCapNhat] [varchar](50) NULL,
	[NgayCapNhat] [datetime] NULL,
	[ThuTu] [int] NULL,
	[TrangThai] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_NhomNguoiDung] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Quyen]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Quyen](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[KhoaChaID] [int] NULL,
	[MaQuyen] [nvarchar](50) NULL,
	[TenQuyen] [nvarchar](250) NULL,
	[NhanQuyen] [bit] NOT NULL,
	[TenHienThi] [nvarchar](250) NULL,
	[Controller] [nvarchar](50) NULL,
	[IconPath] [nvarchar](255) NULL,
	[GhiChu] [nvarchar](250) NULL,
	[Action] [nvarchar](50) NULL,
	[FontAwesome] [nvarchar](50) NULL,
	[IsMenu] [bit] NOT NULL,
	[PhanHe] [tinyint] NULL,
	[NguoiTao] [varchar](50) NULL,
	[NgayTao] [datetime] NULL,
	[NguoiTaoID] [int] NULL,
	[NguoiCapNhatID] [int] NULL,
	[NguoiCapNhat] [varchar](50) NULL,
	[NgayCapNhat] [datetime] NULL,
	[ThuTu] [int] NULL,
	[TrangThai] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ChucNang] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ThongBao]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ThongBao](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TieuDe] [nvarchar](500) NULL,
	[NoiDung] [nvarchar](max) NULL,
	[LstNguoiNhanID] [varchar](max) NULL,
	[LstNhomNguoiNhanID] [varchar](max) NULL,
	[FileDinhKem] [varchar](max) NULL,
	[NguoiTaoID] [int] NULL,
	[NgayTao] [datetime] NULL,
	[ChucNangID] [int] NULL,
	[Link] [nvarchar](500) NULL,
	[IsDaGui] [bit] NOT NULL,
	[LstNguoiNhanDaXemID] [varchar](max) NULL,
	[LstNguoiNhanDaNhanMoiNhatID] [varchar](max) NULL,
	[isXoaTam] [bit] NOT NULL,
 CONSTRAINT [PK_ThongBao] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ThongBaoAttach]    Script Date: 03/05/2021 23:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ThongBaoAttach](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ThongBaoID] [int] NULL,
	[LinkFile] [varchar](max) NULL,
	[NameFile] [nvarchar](max) NULL,
	[ReplaceName] [nvarchar](max) NULL,
	[Size] [varchar](500) NULL,
	[Code] [varchar](500) NULL,
	[NgayTao] [datetime] NULL,
	[TrangThai] [bit] NOT NULL,
 CONSTRAINT [PK_ThongBaoAttach] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



/****** Object:  Table [dbo].[NhomQuyenQuyen]    Script Date: 16/10/2021 15:22:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[NhomQuyenQuyen](
	[QuyenID] [int] NOT NULL,
	[NhomQuyenID] [int] NOT NULL,
	[QuyenChiTiet] [varchar](100) NULL,
 CONSTRAINT [PK_NhomNguoiDungQuyen] PRIMARY KEY CLUSTERED 
(
	[QuyenID] ASC,
	[NhomQuyenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[NhomQuyenQuyen]  WITH CHECK ADD  CONSTRAINT [FK_NhomQuyenQuyen_NhomQuyen] FOREIGN KEY([NhomQuyenID])
REFERENCES [dbo].[NhomQuyen] ([ID])
GO

ALTER TABLE [dbo].[NhomQuyenQuyen] CHECK CONSTRAINT [FK_NhomQuyenQuyen_NhomQuyen]
GO

ALTER TABLE [dbo].[NhomQuyenQuyen]  WITH CHECK ADD  CONSTRAINT [FK_NhomQuyenQuyen_Quyen] FOREIGN KEY([QuyenID])
REFERENCES [dbo].[Quyen] ([ID])
GO

ALTER TABLE [dbo].[NhomQuyenQuyen] CHECK CONSTRAINT [FK_NhomQuyenQuyen_Quyen]
GO





GO
SET ANSI_PADDING OFF
GO

SET IDENTITY_INSERT [dbo].[NhomQuyen] ON 

GO
INSERT [dbo].[NhomQuyen] ([ID], [KyHieu], [MaNhom], [TenNhom], [GhiChu], [NguoiTao], [NgayTao], [NguoiTaoID], [NguoiCapNhatID], [NguoiCapNhat], [NgayCapNhat], [ThuTu], [TrangThai], [IsDeleted]) VALUES (1, NULL, NULL, N'Quản trị hệ thống', NULL, N'admin', CAST(0x0000AD0B018AC84D AS DateTime), 5, 5, N'admin', CAST(0x0000AD3200189A85 AS DateTime), 3, 1, 0)
GO
INSERT [dbo].[NhomQuyen] ([ID], [KyHieu], [MaNhom], [TenNhom], [GhiChu], [NguoiTao], [NgayTao], [NguoiTaoID], [NguoiCapNhatID], [NguoiCapNhat], [NgayCapNhat], [ThuTu], [TrangThai], [IsDeleted]) VALUES (2, NULL, NULL, N'Báo cáo', NULL, N'admin', CAST(0x0000AD0B018AD2EC AS DateTime), 5, 5, N'admin', CAST(0x0000AD22001AA18A AS DateTime), 1, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[NhomQuyen] OFF
GO 

SET IDENTITY_INSERT [dbo].[NguoiDung] ON 

GO
INSERT [dbo].[NguoiDung] ([ID], [HoVaTen], [NgaySinh], [GioiTinh], [NhomQuyenID], [NhomQuyenList], [ChucVuID], [DonViID], [TenDangNhap], [MatKhau], [AnhDaiDien], [Email], [SoDienThoai], [DiaChi], [Fax], [NguoiTao], [NgayTao], [NguoiTaoID], [NguoiCapNhatID], [NguoiCapNhat], [NgayCapNhat], [ThuTu], [TrangThai], [IsDeleted]) VALUES (5, N'Quản trị hệ thống', CAST(0x0000AD0A00000000 AS DateTime), 0, 1, N',1,', 0, 0, N'admin', N'4297F44B13955235245B2497399D7A93', N'/2021/06/02/02_06_21_13_50_35.png', NULL, N'0338971669', NULL, NULL, NULL, NULL, NULL, 5, N'admin', GETDATE(), NULL, 1, 0)
GO

SET IDENTITY_INSERT [dbo].[NguoiDung] OFF
GO
SET IDENTITY_INSERT [dbo].[NhomNguoiDung] ON 

GO
INSERT [dbo].[NhomNguoiDung] ([ID], [TenNhom], [ListNguoiDungThuocNhomID], [PhamViSuDung], [NguoiTao], [NgayTao], [NguoiTaoID], [NguoiCapNhatID], [NguoiCapNhat], [NgayCapNhat], [TrangThai], [IsDeleted]) VALUES (1, N'Quản trị hệ thống', N',5,', 0, N'admin', CAST(0x0000AD0B018AA64A AS DateTime), 5, NULL, NULL, NULL, 1, 0)
GO
INSERT [dbo].[NhomNguoiDung] ([ID], [TenNhom], [ListNguoiDungThuocNhomID], [PhamViSuDung], [NguoiTao], [NgayTao], [NguoiTaoID], [NguoiCapNhatID], [NguoiCapNhat], [NgayCapNhat], [TrangThai], [IsDeleted]) VALUES (2, N'Báo cáo', N',5,', 0, N'admin', CAST(0x0000AD0E000172E9 AS DateTime), 5, NULL, NULL, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[NhomNguoiDung] OFF
GO


CREATE TRIGGER trg_UpdateWebsiteItem
   ON  dbo.ScannedItem
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	UPDATE WebsiteItem SET ErrorCount = ErrorCount + 1 WHERE FullPath = (SELECT FullPath FROM inserted) AND WebsiteID = (SELECT WebsiteID FROM inserted) AND (SELECT Status FROM inserted) = 0;
	UPDATE WebsiteItem SET Status = (SELECT Status FROM inserted) WHERE FullPath = (SELECT FullPath FROM inserted) AND WebsiteID = (SELECT WebsiteID FROM inserted);
END
GO


