CREATE TABLE [dbo].[MetricDataPoint]
(
  [Id]          INT IDENTITY(1, 1)  NOT NULL,
  [MetricId]    INT                 NOT NULL,
  [Actual]      FLOAT               NULL,
  [Target]      FLOAT               NULL,
  [Date]       DATE                NOT NULL,
  [CreatedDate] DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate] DATETIME            NULL,

  CONSTRAINT [PK_MetricDataPoint]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_MetricDataPoint_Metric]
    FOREIGN KEY ([MetricId])
    REFERENCES [dbo].[Metric]([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [nci_MetricDataPoint_MetricId_Date]
ON [dbo].[MetricDataPoint] ([MetricId], [Date])
INCLUDE ([Actual], [Target])
WITH (ONLINE = ON, FILLFACTOR = 95, PAD_INDEX = ON)
GO
