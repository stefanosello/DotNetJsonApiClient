IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Library')
BEGIN
    CREATE DATABASE [Library];
END;
GO

USE [Library];
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Authors' AND xtype='U')
BEGIN
CREATE TABLE Authors (
    AuthorID int IDENTITY(1,1) NOT NULL,
    FirstName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    LastName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    Email nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CreatedAt datetime DEFAULT getdate() NULL,
    DateOfBirth date NULL,
    Avatar nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT PK__Authors__70DAFC144A0FC6F8 PRIMARY KEY (AuthorID),
    CONSTRAINT UQ__Authors__A9D10534C87ED6E6 UNIQUE (Email)
);
END;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
BEGIN
CREATE TABLE Books (
   BookID int IDENTITY(1,1) NOT NULL,
   AuthorID int NULL,
   Title nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
   ISBN nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
   PublishedDate date NULL,
   Price decimal(18,2) NULL,
   StockQuantity int NULL,
   Summary nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
   Cover nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
   CONSTRAINT PK__Books__3DE0C2271298D4A9 PRIMARY KEY (BookID),
   CONSTRAINT FK__Books__AuthorID__3B75D760 FOREIGN KEY (AuthorID) REFERENCES Library.dbo.Authors(AuthorID)
);
END;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tags' AND xtype='U')
BEGIN
CREATE TABLE Tags (
    TagID INT PRIMARY KEY IDENTITY(1,1),
    Label NVARCHAR(255) NOT NULL,
    ParentTagID INT NULL FOREIGN KEY REFERENCES Tags(TagID)
);
END;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BooksTags' AND xtype='U')
BEGIN
CREATE TABLE BooksTags (
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    TagID INT FOREIGN KEY REFERENCES Tags(TagID),
    PRIMARY KEY (BookID, TagID)
);
END;
GO
