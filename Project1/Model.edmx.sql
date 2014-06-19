
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/02/2014 15:38:36
-- Generated from EDMX file: D:\Code\Asciano TMSAAS\Dev\R1-Dev\Source\Import\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Asciano.PN.TMS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AuditLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditLog];
GO
IF OBJECT_ID(N'[dbo].[Entity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Entity];
GO
IF OBJECT_ID(N'[dbo].[EntityOwnershipStrength]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityOwnershipStrength];
GO
IF OBJECT_ID(N'[dbo].[EntityRelationship]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityRelationship];
GO
IF OBJECT_ID(N'[dbo].[Interface]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Interface];
GO
IF OBJECT_ID(N'[dbo].[InternalInterface]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InternalInterface];
GO
IF OBJECT_ID(N'[dbo].[ProcedureDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcedureDefinition];
GO
IF OBJECT_ID(N'[dbo].[SubSystem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubSystem];
GO
IF OBJECT_ID(N'[dbo].[TableDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TableDefinition];
GO
IF OBJECT_ID(N'[dbo].[TriggerDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TriggerDefinition];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AuditLogs'
CREATE TABLE [dbo].[AuditLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LogTime] datetime  NULL,
    [Line] nvarchar(max)  NULL
);
GO

-- Creating table 'Entities'
CREATE TABLE [dbo].[Entities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SourceId] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Type] nvarchar(10)  NOT NULL,
    [SourceUnit] nvarchar(8)  NULL,
    [NormalisedUnit] nvarchar(8)  NULL
);
GO

-- Creating table 'EntityRelationships'
CREATE TABLE [dbo].[EntityRelationships] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CallingEntityId] int  NOT NULL,
    [CalledEntityId] int  NOT NULL
);
GO

-- Creating table 'Interfaces'
CREATE TABLE [dbo].[Interfaces] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TargetUnit] nvarchar(8)  NOT NULL,
    [EntityRelationshipIds] nvarchar(max)  NOT NULL,
    [TargetEntityId] int  NOT NULL
);
GO

-- Creating table 'InternalInterfaces'
CREATE TABLE [dbo].[InternalInterfaces] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TargetEntityId] int  NOT NULL,
    [TargetUnit] nvarchar(8)  NOT NULL,
    [EntityRelationshipIds] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'EntityOwnershipStrengths'
CREATE TABLE [dbo].[EntityOwnershipStrengths] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EntityId] int  NOT NULL,
    [InternalWeight] int  NULL,
    [ExternalWeight] int  NULL,
    [ExternalSources] int  NULL,
    [RecommendNewUnit] nvarchar(8)  NULL
);
GO

-- Creating table 'ProcedureDefinitions'
CREATE TABLE [dbo].[ProcedureDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PackageName] nvarchar(50)  NOT NULL,
    [Type] nvarchar(50)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [CodeLine] int  NOT NULL,
    [Body] nvarchar(400)  NOT NULL,
    [SubSystem] nvarchar(8)  NOT NULL
);
GO

-- Creating table 'SubSystems'
CREATE TABLE [dbo].[SubSystems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SubSystem1] nvarchar(50)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'TableDefinitions'
CREATE TABLE [dbo].[TableDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'TriggerDefinitions'
CREATE TABLE [dbo].[TriggerDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Type] nvarchar(50)  NOT NULL,
    [TriggeringEvent] nvarchar(50)  NOT NULL,
    [TableName] nvarchar(50)  NOT NULL,
    [WhenClause] nvarchar(400)  NULL,
    [CodeLine] int  NOT NULL,
    [Body] nvarchar(400)  NOT NULL,
    [SubSystem] nvarchar(8)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AuditLogs'
ALTER TABLE [dbo].[AuditLogs]
ADD CONSTRAINT [PK_AuditLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [PK_Entities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityRelationships'
ALTER TABLE [dbo].[EntityRelationships]
ADD CONSTRAINT [PK_EntityRelationships]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Interfaces'
ALTER TABLE [dbo].[Interfaces]
ADD CONSTRAINT [PK_Interfaces]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InternalInterfaces'
ALTER TABLE [dbo].[InternalInterfaces]
ADD CONSTRAINT [PK_InternalInterfaces]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityOwnershipStrengths'
ALTER TABLE [dbo].[EntityOwnershipStrengths]
ADD CONSTRAINT [PK_EntityOwnershipStrengths]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProcedureDefinitions'
ALTER TABLE [dbo].[ProcedureDefinitions]
ADD CONSTRAINT [PK_ProcedureDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SubSystems'
ALTER TABLE [dbo].[SubSystems]
ADD CONSTRAINT [PK_SubSystems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TableDefinitions'
ALTER TABLE [dbo].[TableDefinitions]
ADD CONSTRAINT [PK_TableDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TriggerDefinitions'
ALTER TABLE [dbo].[TriggerDefinitions]
ADD CONSTRAINT [PK_TriggerDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------