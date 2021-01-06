CREATE TABLE [dbo].[Webhook]
(
  [Id]                          INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [CompanyId]                   INT                 NOT NULL,
  [WebhookUrl]                  NVARCHAR(300)       NOT NULL,
  [ServiceName]                 NVARCHAR(300)       NOT NULL,
  [ConfigUrl]                   NVARCHAR(300)       NULL,
  [ConfigInfo1]                 NVARCHAR(150)       NULL,
  [ConfigInfo2]                 NVARCHAR(150)       NULL,
  [EventReportingAreaUpdated]   BIT                 NOT NULL DEFAULT 0,
  [EventReportViewed]           BIT                 NOT NULL DEFAULT 0,
  [EventReportComment]          BIT                 NOT NULL DEFAULT 0,
  [CreatedDate]                 DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]                 DATETIME            NULL,

  CONSTRAINT [PK_Webhook]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Webhook_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
