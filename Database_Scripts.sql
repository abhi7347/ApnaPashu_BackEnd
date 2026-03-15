-- =======================================================
-- Database: APNAPASHU
-- Script for Dynamic Categories for User Hub
-- =======================================================

USE APNAPASHU;
GO

-- 1. Create Categories Table if it doesn't exist 
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        CategoryId INT PRIMARY KEY IDENTITY(1,1),
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        IconUrl NVARCHAR(MAX),
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_Categories_IsActive ON [dbo].[Categories](IsActive);
    CREATE INDEX IX_Categories_Name ON [dbo].[Categories](CategoryName);

    -- Insert Sample Data
    INSERT INTO [dbo].[Categories] (CategoryName, Description, IconUrl, IsActive)
    VALUES 
    ('Cows & Buffaloes', 'Dairy and farm animals', '🐄', 1),
    ('Dogs', 'All breeds of dogs', '🐕', 1),
    ('Cats', 'All breeds of cats', '🐈', 1),
    ('Birds', 'All types of birds', '🦜', 1),
    ('Horses', 'Equine animals', '🐎', 1),
    ('Goats & Sheep', 'Livestock', '🐐', 1);
END
GO

-- 2. Create Stored Procedure for fetching active categories
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetCategoriesForUser')
BEGIN
    DROP PROCEDURE [dbo].[sp_GetCategoriesForUser]
END
GO

CREATE PROCEDURE [dbo].[sp_GetCategoriesForUser]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CategoryId, 
        CategoryName, 
        Description, 
        IconUrl, 
        CreatedDate, 
        UpdatedDate, 
        IsActive
    FROM 
        [dbo].[Categories]
    WHERE 
        IsActive = 1
    ORDER BY 
        CategoryName ASC;
END
GO
