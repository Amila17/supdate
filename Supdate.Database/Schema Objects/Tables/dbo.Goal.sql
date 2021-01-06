CREATE TABLE [dbo].[Goal]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [Title]         NVARCHAR(200)       NOT NULL,
  [Description]   NVARCHAR(400)       NULL,
  [DueDate]       DATETIME            NULL,
  [Status]        SMALLINT            NOT NULL DEFAULT 0,
  [CompanyId]     INT                 NOT NULL,
  [AreaId]        INT                 NULL,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_Goal]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_Goal_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id]),

  CONSTRAINT [FK_Goal_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
