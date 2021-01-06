CREATE TABLE [dbo].[UserClaim]
(
  [Id]              INT IDENTITY(1, 1)  NOT NULL,
  [UserId]          INT                 NOT NULL,
  [ClaimType]       NVARCHAR(20)        NOT NULL,
  [ClaimValue]      NVARCHAR(20)        NOT NULL,
  [CreatedDate]     DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]     DATETIME            NULL, 

  CONSTRAINT [PK_UserClaim]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_UserClaim_User]
    FOREIGN KEY ([UserId])
    REFERENCES [AppUser]([Id])
)
GO
