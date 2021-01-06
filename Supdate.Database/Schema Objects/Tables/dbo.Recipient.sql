CREATE TABLE [dbo].[Recipient]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [Name]          NVARCHAR(200)       NULL,
  [FirstName]     NVARCHAR(200)       NULL,
  [LastName]      NVARCHAR(200)       NULL,
  [Email]         NVARCHAR(200)       NOT NULL,
  [CompanyId]     INT                 NOT NULL,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_Recipient]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Recipient_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]),
)
GO
