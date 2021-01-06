CREATE TABLE [dbo].[AdminUser]
(
  [Id]                      INT IDENTITY(1,1)     NOT NULL,
  [UserId]                  INT                   NOT NULL,
  [CreatedDate]             DATETIME              NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]             DATETIME              NULL,

  CONSTRAINT [PK_AdminUser]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_AdminUser_AppUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AppUser]([Id])
)
GO
