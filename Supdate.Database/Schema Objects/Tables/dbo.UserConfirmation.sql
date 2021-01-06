CREATE TABLE [dbo].[UserConfirmation]
(
  [Id]                  INT IDENTITY(1, 1)  NOT NULL,
  [UserId]              INT                 NOT NULL,
  [Url]                 NVARCHAR(500)       NOT NULL,
  [RemindersSent]       INT                 NOT NULL DEFAULT 0,
  [CreatedDate]         DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]         DATETIME            NULL,

  CONSTRAINT [PK_UserUnconfirmed]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_UserUnconfirmed_AppUser_Id]
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AppUser] ([Id]) ON DELETE CASCADE
)
GO
