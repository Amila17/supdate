CREATE TABLE [dbo].[Report]
(
  [Id]                INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]          UNIQUEIDENTIFIER    NOT NULL UNIQUE DEFAULT NEWID(),
  [CompanyId]         INT                 NOT NULL,
  [Date]              DATE                NOT NULL,
  [Summary]           NVARCHAR(MAX)       NULL,
  [StatusId]          INT                 NOT NULL DEFAULT 1,
  [IsStatusManual]    BIT                 NOT NULL DEFAULT 0,
  [CreatedDate]       DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]       DATETIME            NULL,

  CONSTRAINT [PK_Report]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_Report_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]),
)
GO

CREATE NONCLUSTERED INDEX [nci_Report_CompanyId_Date]
ON [dbo].[Report] ([CompanyId], [Date])
INCLUDE ([IsStatusManual], [StatusId], [UniqueId])
WITH (ONLINE = ON, FILLFACTOR = 95, PAD_INDEX = ON)
GO
