CREATE TABLE [dbo].[Metric]
(
  [Id]                    INT IDENTITY(1, 1)    NOT NULL,
  [CompanyId]             INT                   NOT NULL,
  [UniqueId]              UNIQUEIDENTIFIER      NOT NULL DEFAULT NEWID(),
  [AreaId]                INT                   NULL,
  [Name]                  NVARCHAR(200)         NOT NULL,
  [DataSourceId]          INT                   NULL,
  [Prefix]                NVARCHAR(50)          NULL,
  [Suffix]                NVARCHAR(50)          NULL,
  [ThousandsSeparator]    BIT                   NOT NULL DEFAULT 1,
  [LowerIsBetter]         BIT                   NOT NULL DEFAULT 0,
  [GraphType]             SMALLINT              NOT NULL DEFAULT 0,
  [DisplayOrder]          SMALLINT              NOT NULL DEFAULT 1,
  [DataPoints]            INT                   NOT NULL DEFAULT 0,
  [LatestValue]           FLOAT                 NULL,
  [CreatedDate]           DATETIME              NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]           DATETIME              NULL,

  CONSTRAINT [PK_Metric]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_Metric_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id]),

  CONSTRAINT [FK_Metric_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO

CREATE NONCLUSTERED INDEX [nci_Metric_CompanyId]
ON [dbo].[Metric] ([CompanyId])
INCLUDE ([DataSourceId])
WITH (ONLINE = ON, FILLFACTOR = 95, PAD_INDEX = ON)
GO
