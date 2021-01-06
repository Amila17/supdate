CREATE TABLE [dbo].[ReportMetric]
(
  [Id]                INT IDENTITY(1, 1)    NOT NULL,
  [MetricId]          INT                   NOT NULL,
  [ReportId]          INT                   NOT NULL,
  [Value]             FLOAT                 NULL,
  [CreatedDate]       DATETIME              NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]       DATETIME              NULL,

  CONSTRAINT [PK_ReportMetric]
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
