CREATE TABLE [dbo].[Discussion]
(
  [Id]              INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [CompanyId]       INT                 NOT NULL,
  [ReportGuid]      UNIQUEIDENTIFIER    NOT NULL,
  [TargetType]      SMALLINT            NOT NULL,
  [Target]          UNIQUEIDENTIFIER    NOT NULL,
  [CreatedDate]     DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]     DATETIME            NULL,

  CONSTRAINT [PK_Discussion]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Discussion_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]),

  CONSTRAINT [FK_Discussion_Report]
    FOREIGN KEY ([ReportGuid])
    REFERENCES [dbo].[Report]([UniqueId]) ON DELETE CASCADE
)
GO
