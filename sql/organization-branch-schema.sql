-- ============================================================
-- AdminMicroservices — Organization & Branch Schema
-- Run against: AdminAuth database
-- NOTE: If using EF Core migrations (recommended), run
--       `dotnet ef database update` instead of this script.
-- ============================================================

USE AdminAuth;
GO

-- 1. Organizations (SaaS Tenant)
CREATE TABLE Organizations (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    Name        NVARCHAR(200)    NOT NULL,
    Code        NVARCHAR(50)     NOT NULL,
    Email       NVARCHAR(150)    NULL,
    Phone       NVARCHAR(20)     NULL,
    Address     NVARCHAR(500)    NULL,
    LogoUrl     NVARCHAR(500)    NULL,
    IsActive    BIT              NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2        NULL,
    CONSTRAINT UQ_Organizations_Code UNIQUE (Code)
);
GO

-- 2. Branches (per Organization)
CREATE TABLE Branches (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    OrganizationId UNIQUEIDENTIFIER NOT NULL,
    Name           NVARCHAR(200)    NOT NULL,
    Code           NVARCHAR(50)     NOT NULL,
    Email          NVARCHAR(150)    NULL,
    Phone          NVARCHAR(20)     NULL,
    Address        NVARCHAR(500)    NULL,
    IsActive       BIT              NOT NULL DEFAULT 1,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt      DATETIME2        NULL,
    CONSTRAINT FK_Branches_Organizations FOREIGN KEY (OrganizationId)
        REFERENCES Organizations(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Branches_OrgCode UNIQUE (OrganizationId, Code)
);
GO

-- 3. Add Organization + Branch to Users
ALTER TABLE Users
    ADD OrganizationId UNIQUEIDENTIFIER NULL,
        BranchId       UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE Users
    ADD CONSTRAINT FK_Users_Organizations FOREIGN KEY (OrganizationId)
        REFERENCES Organizations(Id);
GO

ALTER TABLE Users
    ADD CONSTRAINT FK_Users_Branches FOREIGN KEY (BranchId)
        REFERENCES Branches(Id);
GO

-- ============================================================
-- Sample Data
-- ============================================================

DECLARE @OrgId    UNIQUEIDENTIFIER = NEWID();
DECLARE @BranchId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Organizations (Id, Name, Code, Email, Phone, Address, IsActive, CreatedAt)
VALUES (@OrgId, N'FarmsSoft Ltd', N'FARMSSOFT', N'info@farmssoft.com',
        N'+8801700000000', N'Dhaka, Bangladesh', 1, GETUTCDATE());

INSERT INTO Branches (Id, OrganizationId, Name, Code, Email, Phone, Address, IsActive, CreatedAt)
VALUES (NEWID(), @OrgId, N'Head Office', N'HO', N'ho@farmssoft.com',
        N'+8801700000001', N'Dhaka, Bangladesh', 1, GETUTCDATE()),
       (NEWID(), @OrgId, N'Chittagong Branch', N'CTG', N'ctg@farmssoft.com',
        N'+8801800000002', N'Chittagong, Bangladesh', 1, GETUTCDATE());

-- Assign the existing superadmin user to the organization/branch
-- (Update the GUIDs to match your actual data)
-- UPDATE Users SET OrganizationId = @OrgId WHERE Email = 'superadmin@admin.com';

PRINT 'Schema and sample data applied successfully.';
GO
