IF db_id('IPSCM') IS NOT NULL
BEGIN
    SELECT 'database does exist'
END
ELSE
BEGIN
    SELECT 'database does not exist'
	CREATE DATABASE IPSCM;	
END
GO
USE [IPSCM] 
--Create Users if users not exists
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [Users]
(
	[UserId] INT NOT NULL PRIMARY KEY, 
	[PlateNumber] NVARCHAR(30) NOT NULL, 
	[Money] MONEY NOT NULL, 
	[PhoneNumber] CHAR(20) NOT NULL
)	
END
GO
USE [IPSCM] 
--Create Ticket if ticket not exists
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tickets]') AND type in (N'U'))
BEGIN
CREATE TABLE [Tickets]
(
	[TicketId] INT NOT NULL PRIMARY KEY, 
	[Type] VARCHAR(30) NOT NULL, 
	[Money] MONEY NOT NULL, 
	[Value] INT NOT NULL,
	[UserId] INT NOT NULL, 
	[StorageName] NVARCHAR(50) NOT NULL
)
END
GO
USE [IPSCM] 
--Create Ticket if parkrecord not exists
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParkRecord]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ParkRecord]
(
	[RecordId] INT NOT NULL PRIMARY KEY, 
    [PlateNumber] NVARCHAR(30) NOT NULL, 
    [InTime] DATETIME NULL, 
    [OutTime] DATETIME NULL
)
END