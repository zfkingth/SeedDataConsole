USE [master]
GO
/****** Object:  Database [XLDDSMTest2]    Script Date: 2018/8/4 20:47:38 ******/
CREATE DATABASE [XLDDSMTest2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'XLDDSMTest2', FILENAME = N'E:\XLDDSMTest2.mdf' , SIZE = 74129408KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'XLDDSMTest2_log', FILENAME = N'E:\XLDDSMTest2_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [XLDDSMTest2] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [XLDDSMTest2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [XLDDSMTest2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET ARITHABORT OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [XLDDSMTest2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [XLDDSMTest2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET  DISABLE_BROKER 
GO
ALTER DATABASE [XLDDSMTest2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [XLDDSMTest2] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [XLDDSMTest2] SET  MULTI_USER 
GO
ALTER DATABASE [XLDDSMTest2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [XLDDSMTest2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [XLDDSMTest2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [XLDDSMTest2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [XLDDSMTest2] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'XLDDSMTest2', N'ON'
GO
ALTER DATABASE [XLDDSMTest2] SET QUERY_STORE = OFF
GO
USE [XLDDSMTest2]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [XLDDSMTest2]
GO
/****** Object:  Table [dbo].[ProjectInfo]    Script Date: 2018/8/4 20:47:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nchar](10) NOT NULL,
 CONSTRAINT [PK_ProjectInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SensorDataOrigin]    Script Date: 2018/8/4 20:47:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorDataOrigin](
	[SensorID] [uniqueidentifier] NOT NULL,
	[MeaTime] [datetime] NOT NULL,
	[MeaValue1] [float] NULL,
	[MeaValue2] [float] NULL,
	[MeaValue3] [float] NULL,
	[ResValue1] [float] NULL,
	[ResValue2] [float] NULL,
	[ResValue3] [float] NULL,
	[Status] [tinyint] NOT NULL,
	[Origin] [tinyint] NOT NULL,
	[Remark] [varchar](255) NULL,
 CONSTRAINT [PK_SensorDataOrigin] PRIMARY KEY CLUSTERED 
(
	[SensorID] ASC,
	[MeaTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SensorInfo]    Script Date: 2018/8/4 20:47:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[ProjectID] [uniqueidentifier] NOT NULL,
	[SensorCode] [nchar](50) NULL,
	[SensorTypeID] [uniqueidentifier] NULL,
	[ProjectSiteID] [uniqueidentifier] NULL,
	[LayLocation] [varchar](255) NULL,
	[LocationX] [varchar](255) NULL,
	[LocationY] [varchar](255) NULL,
	[LocationZ] [varchar](255) NULL,
 CONSTRAINT [PK_SensorInfo] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ProjectID_Name_ClusteredIndex]    Script Date: 2018/8/4 20:47:38 ******/
CREATE UNIQUE CLUSTERED INDEX [ProjectID_Name_ClusteredIndex] ON [dbo].[SensorInfo]
(
	[ProjectID] ASC,
	[SensorCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SensorDataOrigin] ADD  CONSTRAINT [DF_SensorDataOrigin_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[SensorDataOrigin] ADD  CONSTRAINT [DF_SensorDataOrigin_Origin]  DEFAULT ((0)) FOR [Origin]
GO
ALTER TABLE [dbo].[SensorDataOrigin]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorDataOrigin_SensorInfo] FOREIGN KEY([SensorID])
REFERENCES [dbo].[SensorInfo] ([ID])
GO
ALTER TABLE [dbo].[SensorDataOrigin] CHECK CONSTRAINT [FK_SensorDataOrigin_SensorInfo]
GO
ALTER TABLE [dbo].[SensorInfo]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorInfo_ProjectInfo] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[ProjectInfo] ([ID])
GO
ALTER TABLE [dbo].[SensorInfo] CHECK CONSTRAINT [FK_SensorInfo_ProjectInfo]
GO
USE [master]
GO
ALTER DATABASE [XLDDSMTest2] SET  READ_WRITE 
GO
