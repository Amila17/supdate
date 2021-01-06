CREATE TABLE [dbo].[Company]
(
  [Id]                      INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [Name]                    NVARCHAR(300)       NOT NULL,
  [TwitterHandle]           NVARCHAR(100)       NULL,
  [StartMonth]              DATE                NULL,
  [LogoPath]                NVARCHAR(255)       NULL,
  [EnableCommenting]        BIT                 NOT NULL DEFAULT 1,
  [ReportType]              SMALLINT            NOT NULL DEFAULT 2,  -- 1 = Weekly, 2 = Monthly, 3 = Quarterly
  [ReportTitle]             NVARCHAR(500)       NULL DEFAULT 'Shareholder Update',
  [ReportEmailSubject]      NVARCHAR(1000)      NULL,
  [ReportEmailBody]         NVARCHAR(MAX)       NULL,
  [UseCustomSender]         BIT                 NULL DEFAULT 0,
  [CustomSenderName]        NVARCHAR(200)       NULL,
  [CustomSenderEmail]       NVARCHAR(200)       NULL,
  [BillingName]             NVARCHAR(255)       NULL,
  [BillingAddress1]         NVARCHAR(255)       NULL,
  [BillingAddress2]         NVARCHAR(255)       NULL,
  [BillingCity]             NVARCHAR(255)       NULL,
  [BillingPostcode]         NVARCHAR(255)       NULL,
  [BillingCountry]          NVARCHAR(255)       NULL,
  [CreatedDate]             DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]             DATETIME            NULL,

  CONSTRAINT [PK_Company]
    PRIMARY KEY CLUSTERED (Id ASC)
)
GO
