CREATE TABLE [dbo].[ReportEmail]
(
  [Id]                  INT IDENTITY(1, 1)  NOT NULL,
  [RecipientId]         INT                 NOT NULL,
  [CompanyId]           INT                 NOT NULL,
  [ReportId]            INT                 NOT NULL,
  [Status]              SMALLINT            NOT NULL,
  [Views]               INT                 NOT NULL DEFAULT 0,
  [LastViewedDate]      DATETIME            NULL,
  [UniqueId]            UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [ViewKey]             UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [CreatedDate]         DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]         DATETIME            NULL,

  CONSTRAINT [PK_ReportEmail]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_ReportEmail_Recipient]
    FOREIGN KEY ([RecipientId])
    REFERENCES [dbo].[Recipient]([Id]) ON DELETE CASCADE,

  CONSTRAINT [FK_ReportEmail_Report]
    FOREIGN KEY ([ReportId])
    REFERENCES [dbo].[Report]([Id])
    ON DELETE CASCADE,

  CONSTRAINT [FK_ReportEmail_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]),
)
GO
