CREATE TABLE [dbo].[UserEmail]
(
  [Id]                  INT IDENTITY(1, 1)  NOT NULL,
  [UserId]              INT                 NOT NULL,
  [FeedbackEmailSent]   BIT                 NOT NULL DEFAULT 0,
  [CreatedDate]         DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]         DATETIME            NULL,

  CONSTRAINT [PK_UserEmail]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_UserEmail_AppUser_Id]
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AppUser] ([Id]) ON DELETE CASCADE

)
GO
