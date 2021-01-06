CREATE TABLE [dbo].[CompanyUser]
(
  [Id]              INT IDENTITY(1, 1)  NOT NULL,
  [CompanyId]       INT,
  [UserId]          INT,
  [DefaultCompany]  INT                 NULL,
  [IsOwner]         INT                 NOT NULL DEFAULT 1,
  [CanViewReports]  BIT                 NOT NULL DEFAULT 0,
  [CreatedDate]     DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]     DATETIME            NULL,

  CONSTRAINT [PK_CompanyUser]
    PRIMARY KEY CLUSTERED (Id ASC),
  CONSTRAINT [FK_CompanyUser_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id]) ON DELETE CASCADE,
  CONSTRAINT [FK_CompanyUser_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AppUser]([Id]) ON DELETE CASCADE
)
GO
