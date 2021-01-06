CREATE TABLE [dbo].[CompanyStats]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [CompanyId]     INT                 NOT NULL,
  [AreaCount]     INT                 NOT NULL DEFAULT 0,
  [MetricCount]   INT                 NOT NULL DEFAULT 0,
  [GoalCount]     INT                 NOT NULL DEFAULT 0,
  [ReportCount]   INT                 NOT NULL DEFAULT 0,
  [UserCount]     INT                 NOT NULL DEFAULT 0,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_CompanyStats]
    PRIMARY KEY CLUSTERED (Id ASC),
  CONSTRAINT [FK_Company_Stats]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
