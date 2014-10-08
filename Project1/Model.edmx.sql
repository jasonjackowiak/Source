
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/08/2014 11:12:57
-- Generated from EDMX file: D:\JCode\GitHub\FAAS\Project1\Model.edmx
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

IF OBJECT_ID(N'[Admin].[FK_EntityResidence_Entity]', 'F') IS NOT NULL
    ALTER TABLE [Admin].[EntityResidence] DROP CONSTRAINT [FK_EntityResidence_Entity];
GO
IF OBJECT_ID(N'[Admin].[FK_Interface_Entity]', 'F') IS NOT NULL
    ALTER TABLE [Admin].[Interface] DROP CONSTRAINT [FK_Interface_Entity];
GO
IF OBJECT_ID(N'[Admin].[FK_InterfaceReporting_Entity]', 'F') IS NOT NULL
    ALTER TABLE [Admin].[InterfaceReporting] DROP CONSTRAINT [FK_InterfaceReporting_Entity];
GO
IF OBJECT_ID(N'[Admin].[FK_InterfaceReporting_Interface]', 'F') IS NOT NULL
    ALTER TABLE [Admin].[InterfaceReporting] DROP CONSTRAINT [FK_InterfaceReporting_Interface];
GO
IF OBJECT_ID(N'[Admin].[FK_InternalInterface_Entity]', 'F') IS NOT NULL
    ALTER TABLE [Admin].[InternalInterface] DROP CONSTRAINT [FK_InternalInterface_Entity];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Record_LanguageReference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Record] DROP CONSTRAINT [FK_Record_LanguageReference];
GO
IF OBJECT_ID(N'[dbo].[FK_Record_Snapshot]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Record] DROP CONSTRAINT [FK_Record_Snapshot];
GO
IF OBJECT_ID(N'[dbo].[FK_Snapshot_PhaseReference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Snapshot] DROP CONSTRAINT [FK_Snapshot_PhaseReference];
GO
IF OBJECT_ID(N'[dbo].[FK_Snapshot_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Snapshot] DROP CONSTRAINT [FK_Snapshot_Project];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[Admin].[AuditLog]', 'U') IS NOT NULL
    DROP TABLE [Admin].[AuditLog];
GO
IF OBJECT_ID(N'[Admin].[Bucket]', 'U') IS NOT NULL
    DROP TABLE [Admin].[Bucket];
GO
IF OBJECT_ID(N'[Admin].[BucketConnection]', 'U') IS NOT NULL
    DROP TABLE [Admin].[BucketConnection];
GO
IF OBJECT_ID(N'[Admin].[BucketReporting]', 'U') IS NOT NULL
    DROP TABLE [Admin].[BucketReporting];
GO
IF OBJECT_ID(N'[Admin].[Entity]', 'U') IS NOT NULL
    DROP TABLE [Admin].[Entity];
GO
IF OBJECT_ID(N'[Admin].[EntityOwnershipStrength]', 'U') IS NOT NULL
    DROP TABLE [Admin].[EntityOwnershipStrength];
GO
IF OBJECT_ID(N'[Admin].[EntityRelationship]', 'U') IS NOT NULL
    DROP TABLE [Admin].[EntityRelationship];
GO
IF OBJECT_ID(N'[Admin].[EntityResidence]', 'U') IS NOT NULL
    DROP TABLE [Admin].[EntityResidence];
GO
IF OBJECT_ID(N'[Admin].[Interface]', 'U') IS NOT NULL
    DROP TABLE [Admin].[Interface];
GO
IF OBJECT_ID(N'[Admin].[InterfaceReporting]', 'U') IS NOT NULL
    DROP TABLE [Admin].[InterfaceReporting];
GO
IF OBJECT_ID(N'[Admin].[InternalInterface]', 'U') IS NOT NULL
    DROP TABLE [Admin].[InternalInterface];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[FunctionDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FunctionDefinition];
GO
IF OBJECT_ID(N'[dbo].[LanguageReference]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LanguageReference];
GO
IF OBJECT_ID(N'[dbo].[PackageDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PackageDefinition];
GO
IF OBJECT_ID(N'[dbo].[PhaseReference]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PhaseReference];
GO
IF OBJECT_ID(N'[dbo].[Project]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Project];
GO
IF OBJECT_ID(N'[dbo].[Record]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Record];
GO
IF OBJECT_ID(N'[dbo].[RuleDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RuleDefinition];
GO
IF OBJECT_ID(N'[dbo].[Snapshot]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Snapshot];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TableDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TableDefinition];
GO
IF OBJECT_ID(N'[dbo].[TableForeignConstraint]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TableForeignConstraint];
GO
IF OBJECT_ID(N'[dbo].[TransactionDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionDefinition];
GO
IF OBJECT_ID(N'[dbo].[TriggerDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TriggerDefinition];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'LanguageReferences'
CREATE TABLE [dbo].[LanguageReferences] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Variant] nvarchar(8)  NOT NULL,
    [Version] decimal(2,0)  NULL
);
GO

-- Creating table 'PhaseReferences'
CREATE TABLE [dbo].[PhaseReferences] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Projects'
CREATE TABLE [dbo].[Projects] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CustomerId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] nvarchar(8)  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL
);
GO

-- Creating table 'Records'
CREATE TABLE [dbo].[Records] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SnapshotId] int  NOT NULL,
    [LanguageId] int  NOT NULL,
    [DateTimeStamp] datetime  NOT NULL
);
GO

-- Creating table 'Snapshots'
CREATE TABLE [dbo].[Snapshots] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [PhaseId] int  NOT NULL,
    [Status] nvarchar(8)  NOT NULL,
    [DateTimeStamp] datetime  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'AuditLogs'
CREATE TABLE [dbo].[AuditLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LogTime] datetime  NULL,
    [Line] nvarchar(max)  NULL
);
GO

-- Creating table 'Buckets'
CREATE TABLE [dbo].[Buckets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Unit] nvarchar(50)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Type] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'BucketConnections'
CREATE TABLE [dbo].[BucketConnections] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CallingBucket] nvarchar(50)  NOT NULL,
    [NumberCalledInterfaces] int  NOT NULL,
    [CalledBucket] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'BucketReportings'
CREATE TABLE [dbo].[BucketReportings] (
    [BucketId] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Unit] nvarchar(8)  NOT NULL,
    [Entities] int  NOT NULL,
    [NumberCallingBuckets] int  NOT NULL,
    [CallingBuckets] nvarchar(max)  NOT NULL,
    [NumberCalleduckets] int  NOT NULL,
    [CalledBuckets] nvarchar(max)  NOT NULL,
    [NumberInternalInterfaces] int  NOT NULL,
    [NumberExternalInterfaces] int  NOT NULL
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

-- Creating table 'EntityRelationships'
CREATE TABLE [dbo].[EntityRelationships] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CallingEntityId] int  NOT NULL,
    [CalledEntityId] int  NOT NULL
);
GO

-- Creating table 'EntityResidences'
CREATE TABLE [dbo].[EntityResidences] (
    [EntityId] int  NOT NULL,
    [InternalWeight] int  NOT NULL,
    [ExternalWeight] int  NOT NULL,
    [ExternalSources] int  NOT NULL
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

-- Creating table 'InterfaceReportings'
CREATE TABLE [dbo].[InterfaceReportings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InterfaceId] int  NOT NULL,
    [TargetEntityId] int  NOT NULL,
    [TargetUnit] nvarchar(8)  NOT NULL,
    [Type] nvarchar(10)  NOT NULL,
    [TotalEntitySources] int  NOT NULL,
    [TotalBucketsSources] int  NOT NULL,
    [SourceBuckets] nvarchar(max)  NULL
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

-- Creating table 'FunctionDefinitions'
CREATE TABLE [dbo].[FunctionDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PackageId] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Type] nvarchar(50)  NOT NULL,
    [CodeLine] int  NOT NULL,
    [Body] nvarchar(400)  NOT NULL,
    [Unit] nvarchar(8)  NULL
);
GO

-- Creating table 'PackageDefinitions'
CREATE TABLE [dbo].[PackageDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [LineType] nvarchar(50)  NOT NULL,
    [CodeLine] int  NOT NULL,
    [Body] nvarchar(400)  NOT NULL,
    [PackageType] nvarchar(50)  NOT NULL,
    [Unit] nvarchar(8)  NULL
);
GO

-- Creating table 'RuleDefinitions'
CREATE TABLE [dbo].[RuleDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [CodeLine] int  NOT NULL,
    [Body] nvarchar(400)  NOT NULL,
    [Unit] nvarchar(8)  NOT NULL
);
GO

-- Creating table 'TableDefinitions'
CREATE TABLE [dbo].[TableDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [FieldName] nvarchar(50)  NULL,
    [FieldType] nvarchar(10)  NULL,
    [FieldSyntax] nvarchar(10)  NULL,
    [FieldLength] int  NULL,
    [FieldDecimal] int  NULL,
    [FieldNumber] int  NULL,
    [KeyType] nvarchar(10)  NULL,
    [TableType] nvarchar(10)  NULL,
    [Unit] nvarchar(8)  NULL
);
GO

-- Creating table 'TableForeignConstraints'
CREATE TABLE [dbo].[TableForeignConstraints] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [ConastraintName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'TransactionDefinitions'
CREATE TABLE [dbo].[TransactionDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [BuildRule] nvarchar(50)  NULL,
    [Description] nvarchar(50)  NULL,
    [PFKey] nvarchar(50)  NULL,
    [PFKeyRule] nvarchar(50)  NULL,
    [Unit] nvarchar(8)  NULL
);
GO

-- Creating table 'TriggerDefinitions'
CREATE TABLE [dbo].[TriggerDefinitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TableSourceId] int  NULL,
    [TableName] nvarchar(50)  NULL,
    [RuleSourceId] int  NULL,
    [RuleName] nvarchar(50)  NULL,
    [Access] nvarchar(10)  NULL,
    [Type] nvarchar(50)  NULL,
    [Name] nvarchar(50)  NULL,
    [TriggeringEvent] nvarchar(50)  NULL,
    [BaseObjectType] nvarchar(50)  NULL,
    [WhenClause] nvarchar(max)  NULL,
    [Body] nvarchar(max)  NULL,
    [Unit] nvarchar(8)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LanguageReferences'
ALTER TABLE [dbo].[LanguageReferences]
ADD CONSTRAINT [PK_LanguageReferences]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PhaseReferences'
ALTER TABLE [dbo].[PhaseReferences]
ADD CONSTRAINT [PK_PhaseReferences]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [PK_Projects]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Records'
ALTER TABLE [dbo].[Records]
ADD CONSTRAINT [PK_Records]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Snapshots'
ALTER TABLE [dbo].[Snapshots]
ADD CONSTRAINT [PK_Snapshots]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'AuditLogs'
ALTER TABLE [dbo].[AuditLogs]
ADD CONSTRAINT [PK_AuditLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Buckets'
ALTER TABLE [dbo].[Buckets]
ADD CONSTRAINT [PK_Buckets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BucketConnections'
ALTER TABLE [dbo].[BucketConnections]
ADD CONSTRAINT [PK_BucketConnections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [BucketId] in table 'BucketReportings'
ALTER TABLE [dbo].[BucketReportings]
ADD CONSTRAINT [PK_BucketReportings]
    PRIMARY KEY CLUSTERED ([BucketId] ASC);
GO

-- Creating primary key on [Id] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [PK_Entities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityOwnershipStrengths'
ALTER TABLE [dbo].[EntityOwnershipStrengths]
ADD CONSTRAINT [PK_EntityOwnershipStrengths]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityRelationships'
ALTER TABLE [dbo].[EntityRelationships]
ADD CONSTRAINT [PK_EntityRelationships]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [EntityId] in table 'EntityResidences'
ALTER TABLE [dbo].[EntityResidences]
ADD CONSTRAINT [PK_EntityResidences]
    PRIMARY KEY CLUSTERED ([EntityId] ASC);
GO

-- Creating primary key on [Id] in table 'Interfaces'
ALTER TABLE [dbo].[Interfaces]
ADD CONSTRAINT [PK_Interfaces]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InterfaceReportings'
ALTER TABLE [dbo].[InterfaceReportings]
ADD CONSTRAINT [PK_InterfaceReportings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InternalInterfaces'
ALTER TABLE [dbo].[InternalInterfaces]
ADD CONSTRAINT [PK_InternalInterfaces]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FunctionDefinitions'
ALTER TABLE [dbo].[FunctionDefinitions]
ADD CONSTRAINT [PK_FunctionDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PackageDefinitions'
ALTER TABLE [dbo].[PackageDefinitions]
ADD CONSTRAINT [PK_PackageDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RuleDefinitions'
ALTER TABLE [dbo].[RuleDefinitions]
ADD CONSTRAINT [PK_RuleDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TableDefinitions'
ALTER TABLE [dbo].[TableDefinitions]
ADD CONSTRAINT [PK_TableDefinitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TableForeignConstraints'
ALTER TABLE [dbo].[TableForeignConstraints]
ADD CONSTRAINT [PK_TableForeignConstraints]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TransactionDefinitions'
ALTER TABLE [dbo].[TransactionDefinitions]
ADD CONSTRAINT [PK_TransactionDefinitions]
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

-- Creating foreign key on [CustomerId] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [FK_Project_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_Customer'
CREATE INDEX [IX_FK_Project_Customer]
ON [dbo].[Projects]
    ([CustomerId]);
GO

-- Creating foreign key on [LanguageId] in table 'Records'
ALTER TABLE [dbo].[Records]
ADD CONSTRAINT [FK_Record_LanguageReference]
    FOREIGN KEY ([LanguageId])
    REFERENCES [dbo].[LanguageReferences]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Record_LanguageReference'
CREATE INDEX [IX_FK_Record_LanguageReference]
ON [dbo].[Records]
    ([LanguageId]);
GO

-- Creating foreign key on [PhaseId] in table 'Snapshots'
ALTER TABLE [dbo].[Snapshots]
ADD CONSTRAINT [FK_Snapshot_PhaseReference]
    FOREIGN KEY ([PhaseId])
    REFERENCES [dbo].[PhaseReferences]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Snapshot_PhaseReference'
CREATE INDEX [IX_FK_Snapshot_PhaseReference]
ON [dbo].[Snapshots]
    ([PhaseId]);
GO

-- Creating foreign key on [ProjectId] in table 'Snapshots'
ALTER TABLE [dbo].[Snapshots]
ADD CONSTRAINT [FK_Snapshot_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Snapshot_Project'
CREATE INDEX [IX_FK_Snapshot_Project]
ON [dbo].[Snapshots]
    ([ProjectId]);
GO

-- Creating foreign key on [SnapshotId] in table 'Records'
ALTER TABLE [dbo].[Records]
ADD CONSTRAINT [FK_Record_Snapshot]
    FOREIGN KEY ([SnapshotId])
    REFERENCES [dbo].[Snapshots]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Record_Snapshot'
CREATE INDEX [IX_FK_Record_Snapshot]
ON [dbo].[Records]
    ([SnapshotId]);
GO

-- Creating foreign key on [EntityId] in table 'EntityResidences'
ALTER TABLE [dbo].[EntityResidences]
ADD CONSTRAINT [FK_EntityResidence_Entity]
    FOREIGN KEY ([EntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TargetEntityId] in table 'Interfaces'
ALTER TABLE [dbo].[Interfaces]
ADD CONSTRAINT [FK_Interface_Entity]
    FOREIGN KEY ([TargetEntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Interface_Entity'
CREATE INDEX [IX_FK_Interface_Entity]
ON [dbo].[Interfaces]
    ([TargetEntityId]);
GO

-- Creating foreign key on [TargetEntityId] in table 'InterfaceReportings'
ALTER TABLE [dbo].[InterfaceReportings]
ADD CONSTRAINT [FK_InterfaceReporting_Entity]
    FOREIGN KEY ([TargetEntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InterfaceReporting_Entity'
CREATE INDEX [IX_FK_InterfaceReporting_Entity]
ON [dbo].[InterfaceReportings]
    ([TargetEntityId]);
GO

-- Creating foreign key on [TargetEntityId] in table 'InternalInterfaces'
ALTER TABLE [dbo].[InternalInterfaces]
ADD CONSTRAINT [FK_InternalInterface_Entity]
    FOREIGN KEY ([TargetEntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InternalInterface_Entity'
CREATE INDEX [IX_FK_InternalInterface_Entity]
ON [dbo].[InternalInterfaces]
    ([TargetEntityId]);
GO

-- Creating foreign key on [InterfaceId] in table 'InterfaceReportings'
ALTER TABLE [dbo].[InterfaceReportings]
ADD CONSTRAINT [FK_InterfaceReporting_Interface]
    FOREIGN KEY ([InterfaceId])
    REFERENCES [dbo].[Interfaces]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InterfaceReporting_Interface'
CREATE INDEX [IX_FK_InterfaceReporting_Interface]
ON [dbo].[InterfaceReportings]
    ([InterfaceId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------