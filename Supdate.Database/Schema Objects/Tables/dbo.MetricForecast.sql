CREATE TABLE [dbo].[MetricForecast]
(
  [Id]          INT IDENTITY(1, 1)  NOT NULL,
  [MetricId]    INT                 NOT NULL,
  [Value]       FLOAT               NULL,
  [Month]       DATE                NOT NULL,
  [CreatedDate] DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate] DATETIME            NULL,

  CONSTRAINT [PK_MetricForecast]
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
