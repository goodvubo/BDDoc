
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/24/2017 23:02:31
-- Generated from EDMX file: C:\Users\VUBO\Documents\BDdocFromRez\BDdoc\BDdoc\Models\BDDoctors.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Doctors];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Doctor_Hospital]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Doctor] DROP CONSTRAINT [FK_Doctor_Hospital];
GO
IF OBJECT_ID(N'[dbo].[FK_Feedback_Doctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Feedback] DROP CONSTRAINT [FK_Feedback_Doctor];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Doctor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Doctor];
GO
IF OBJECT_ID(N'[dbo].[Feedback]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Feedback];
GO
IF OBJECT_ID(N'[dbo].[Hospital]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Hospital];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Doctors'
CREATE TABLE [dbo].[Doctors] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AppTime] nvarchar(max)  NULL,
    [Fee] int  NOT NULL,
    [Location] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NULL,
    [Speciality] nvarchar(max)  NULL,
    [hospital_ID] int  NULL
);
GO

-- Creating table 'Feedbacks'
CREATE TABLE [dbo].[Feedbacks] (
    [feedback_ID] int IDENTITY(1,1) NOT NULL,
    [ID] int  NULL,
    [comment] nvarchar(50)  NULL,
    [Popularity] int  NULL,
    [Reason] nvarchar(50)  NULL,
    [patient_Name] nvarchar(50)  NULL,
    [patient_Mobile] int  NULL,
    [patient_email] nvarchar(50)  NULL
);
GO

-- Creating table 'Hospitals'
CREATE TABLE [dbo].[Hospitals] (
    [hospital_ID] int IDENTITY(1,1) NOT NULL,
    [hospital_Name] nvarchar(50)  NULL,
    [location] nvarchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Doctors'
ALTER TABLE [dbo].[Doctors]
ADD CONSTRAINT [PK_Doctors]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [feedback_ID] in table 'Feedbacks'
ALTER TABLE [dbo].[Feedbacks]
ADD CONSTRAINT [PK_Feedbacks]
    PRIMARY KEY CLUSTERED ([feedback_ID] ASC);
GO

-- Creating primary key on [hospital_ID] in table 'Hospitals'
ALTER TABLE [dbo].[Hospitals]
ADD CONSTRAINT [PK_Hospitals]
    PRIMARY KEY CLUSTERED ([hospital_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [hospital_ID] in table 'Doctors'
ALTER TABLE [dbo].[Doctors]
ADD CONSTRAINT [FK_Doctor_Hospital]
    FOREIGN KEY ([hospital_ID])
    REFERENCES [dbo].[Hospitals]
        ([hospital_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Doctor_Hospital'
CREATE INDEX [IX_FK_Doctor_Hospital]
ON [dbo].[Doctors]
    ([hospital_ID]);
GO

-- Creating foreign key on [ID] in table 'Feedbacks'
ALTER TABLE [dbo].[Feedbacks]
ADD CONSTRAINT [FK_Feedback_Doctor]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[Doctors]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Feedback_Doctor'
CREATE INDEX [IX_FK_Feedback_Doctor]
ON [dbo].[Feedbacks]
    ([ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------