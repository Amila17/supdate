CREATE TABLE [dbo].[UtmInfo]
(
  [Id]                  INT IDENTITY(1, 1)  NOT NULL,
  [UserId]              INT                 NOT NULL,
  [Source]              NVARCHAR(200),
  [Medium]              NVARCHAR(200),
  [Term]                NVARCHAR(200),
  [Content]             NVARCHAR(200),
  [Campaign]            NVARCHAR(200),
  [CreatedDate]         DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]         DATETIME            NULL,

  CONSTRAINT [PK_UtmInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_UtmInfo_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AppUser]([Id]) ON DELETE CASCADE
)
GO
