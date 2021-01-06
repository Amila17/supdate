CREATE TABLE [dbo].[ExternalApiAuth]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [CompanyId]     INT                 NOT NULL,
  [ExternalApiId] INT                 NOT NULL,
  [Token]         NVARCHAR(350)       NULL,
  [Key]           NVARCHAR(150)       NULL,
  [ConfigData]    NVARCHAR(250)       NULL,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_ExternalApiAuth]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_ExternalApiAutha_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
