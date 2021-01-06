CREATE TABLE [dbo].[ReportArea]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [AreaId]        INT                 NOT NULL,
  [ReportId]      INT                 NOT NULL,
  [Summary]       NVARCHAR(MAX)       NOT NULL,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_ReportArea]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_ReportArea_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id]),
  CONSTRAINT [FK_ReportArea_Report]
    FOREIGN KEY ([ReportId])
    REFERENCES [dbo].[Report]([Id])
    ON DELETE CASCADE
)
GO
