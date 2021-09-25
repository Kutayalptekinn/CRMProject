
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/04/2021 16:44:30
-- Generated from EDMX file: C:\Users\kutay\source\repos\CRM\CRM.Data\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CrmDbTest];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserRoles_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [FK_UserRoles_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRoles_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [FK_UserRoles_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_NewRequests_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewRequests] DROP CONSTRAINT [FK_NewRequests_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Step_CompletedRequests]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Step] DROP CONSTRAINT [FK_Step_CompletedRequests];
GO
IF OBJECT_ID(N'[dbo].[FK_Step_WorkingRequests]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Step] DROP CONSTRAINT [FK_Step_WorkingRequests];
GO
IF OBJECT_ID(N'[dbo].[FK_Step_NotCompletedRequests]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Step] DROP CONSTRAINT [FK_Step_NotCompletedRequests];
GO
IF OBJECT_ID(N'[dbo].[FK_Step_WorkingRequests1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Step] DROP CONSTRAINT [FK_Step_WorkingRequests1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[UserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoles];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[NewRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NewRequests];
GO
IF OBJECT_ID(N'[dbo].[WorkingRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkingRequests];
GO
IF OBJECT_ID(N'[dbo].[CompletedRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompletedRequests];
GO
IF OBJECT_ID(N'[dbo].[NotCompletedRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotCompletedRequests];
GO
IF OBJECT_ID(N'[dbo].[Step]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Step];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(250)  NOT NULL
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

-- Creating table 'UserRoles'
CREATE TABLE [dbo].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [RoleId] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(250)  NOT NULL,
    [Password] nvarchar(250)  NOT NULL
);
GO

-- Creating table 'NewRequests'
CREATE TABLE [dbo].[NewRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Request] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [Priority] tinyint  NULL,
    [RequestorName] nvarchar(50)  NULL,
    [AssignTo] nvarchar(50)  NULL,
    [StartingDate] datetime  NULL,
    [Deadline] datetime  NULL,
    [WorkSteps] nvarchar(50)  NULL,
    [UserId] int  NOT NULL,
    [Checked] bit  NULL
);
GO

-- Creating table 'WorkingRequests'
CREATE TABLE [dbo].[WorkingRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Request] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [Priority] tinyint  NULL,
    [RequestorName] nvarchar(50)  NULL,
    [AssignTo] nvarchar(50)  NULL,
    [StartingDate] datetime  NULL,
    [Deadline] datetime  NULL,
    [WorkSteps] nvarchar(50)  NULL,
    [UserId] int  NOT NULL,
    [Checked] bit  NULL
);
GO

-- Creating table 'CompletedRequests'
CREATE TABLE [dbo].[CompletedRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Request] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [Priority] nvarchar(50)  NULL,
    [RequestorName] nvarchar(50)  NULL,
    [AssignTo] nvarchar(50)  NULL,
    [StartingDate] datetime  NULL,
    [Deadline] datetime  NULL,
    [WorkSteps] nvarchar(50)  NULL,
    [UserId] int  NOT NULL,
    [Checked] bit  NULL
);
GO

-- Creating table 'NotCompletedRequests'
CREATE TABLE [dbo].[NotCompletedRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Request] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [Priority] nvarchar(50)  NULL,
    [RequestorName] nvarchar(50)  NULL,
    [AssignTo] nvarchar(50)  NULL,
    [StartingDate] datetime  NULL,
    [Deadline] datetime  NULL,
    [WorkSteps] nvarchar(50)  NULL,
    [UserId] int  NOT NULL,
    [Checked] bit  NULL
);
GO

-- Creating table 'Step'
CREATE TABLE [dbo].[Step] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NewRequestsId] int  NULL,
    [WorkingRequestId] int  NULL,
    [Name] nvarchar(50)  NULL,
    [StepDetail] nvarchar(50)  NULL,
    [Checkedd] bit  NULL,
    [CompletedRequestsId] int  NULL,
    [NotCompletedRequestsId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [PK_UserRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NewRequests'
ALTER TABLE [dbo].[NewRequests]
ADD CONSTRAINT [PK_NewRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkingRequests'
ALTER TABLE [dbo].[WorkingRequests]
ADD CONSTRAINT [PK_WorkingRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompletedRequests'
ALTER TABLE [dbo].[CompletedRequests]
ADD CONSTRAINT [PK_CompletedRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NotCompletedRequests'
ALTER TABLE [dbo].[NotCompletedRequests]
ADD CONSTRAINT [PK_NotCompletedRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [PK_Step]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [RoleId] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_UserRoles_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRoles_Roles'
CREATE INDEX [IX_FK_UserRoles_Roles]
ON [dbo].[UserRoles]
    ([RoleId]);
GO

-- Creating foreign key on [UserId] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_UserRoles_Users]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRoles_Users'
CREATE INDEX [IX_FK_UserRoles_Users]
ON [dbo].[UserRoles]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'NewRequests'
ALTER TABLE [dbo].[NewRequests]
ADD CONSTRAINT [FK_NewRequests_Users]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NewRequests_Users'
CREATE INDEX [IX_FK_NewRequests_Users]
ON [dbo].[NewRequests]
    ([UserId]);
GO

-- Creating foreign key on [CompletedRequestsId] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [FK_Step_CompletedRequests]
    FOREIGN KEY ([CompletedRequestsId])
    REFERENCES [dbo].[CompletedRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Step_CompletedRequests'
CREATE INDEX [IX_FK_Step_CompletedRequests]
ON [dbo].[Step]
    ([CompletedRequestsId]);
GO

-- Creating foreign key on [NewRequestsId] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [FK_Step_WorkingRequests]
    FOREIGN KEY ([NewRequestsId])
    REFERENCES [dbo].[NewRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Step_WorkingRequests'
CREATE INDEX [IX_FK_Step_WorkingRequests]
ON [dbo].[Step]
    ([NewRequestsId]);
GO

-- Creating foreign key on [NotCompletedRequestsId] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [FK_Step_NotCompletedRequests]
    FOREIGN KEY ([NotCompletedRequestsId])
    REFERENCES [dbo].[NotCompletedRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Step_NotCompletedRequests'
CREATE INDEX [IX_FK_Step_NotCompletedRequests]
ON [dbo].[Step]
    ([NotCompletedRequestsId]);
GO

-- Creating foreign key on [WorkingRequestId] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [FK_Step_WorkingRequests1]
    FOREIGN KEY ([WorkingRequestId])
    REFERENCES [dbo].[WorkingRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Step_WorkingRequests1'
CREATE INDEX [IX_FK_Step_WorkingRequests1]
ON [dbo].[Step]
    ([WorkingRequestId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------