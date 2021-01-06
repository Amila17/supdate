CREATE TABLE [dbo].[ReportAttachment]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [CompanyId]     INT                 NOT NULL,
  [UniqueId]          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [ReportId]      INT                 NOT NULL,
  [AreaId]        INT                 NULL,
  [MimeType]      NVARCHAR(150)       NOT NULL,
  [FileName]      NVARCHAR(300)       NOT NULL ,
  [FilePath]      NVARCHAR(500)       NOT NULL,
  [Description]   NVARCHAR(MAX),
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_ReportAttachment]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_ReportAttachment_Report]
    FOREIGN KEY ([ReportId])
    REFERENCES [dbo].[Report]([Id])
    ON DELETE CASCADE,

   CONSTRAINT [FK_ReportAttachment_AreaId]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id])
    ON DELETE CASCADE,

  CONSTRAINT [FK_ReportAttachment_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]),
)
GO
