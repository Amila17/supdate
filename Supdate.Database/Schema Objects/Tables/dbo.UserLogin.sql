CREATE TABLE [dbo].[UserLogin]
(
  [Id]              INT IDENTITY(1, 1)  NOT NULL,
  [UserId]          INT                 NOT NULL,
  [LoginProvider]   NVARCHAR(500)       NOT NULL,
  [ProviderKey]     NVARCHAR(500)       NOT NULL,
  [CreatedDate]     DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]     DATETIME            NULL,

  CONSTRAINT [PK_UserLogin]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [UK_UserLogin]
    UNIQUE ([UserId], [ProviderKey], [LoginProvider] ASC),
  CONSTRAINT [FK_dbo.UserLogin_dbo.AppUser_Id]
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AppUser] ([Id]) ON DELETE CASCADE
)
GO
