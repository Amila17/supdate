CREATE TABLE [dbo].[UserTermsAndConditions]
(
  [Id]                    INT IDENTITY(1, 1)  NOT NULL,
  [UserId]                INT                 NOT NULL,
  [TermsAndConditionsId]  INT                 NOT NULL,
  [AcceptedDate]          DATETIME            NOT NULL DEFAULT GETUTCDATE(),

  CONSTRAINT [PK_UserTermsAndConditions]
    PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [FK_UserTermsAndConditions_AppUser_Id]
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AppUser] ([Id]) ON DELETE CASCADE,
  CONSTRAINT [FK_UserTermsAndConditions_TAndC_Id]
    FOREIGN KEY ([TermsAndConditionsId]) REFERENCES [dbo].[TermsAndConditions] ([Id]) ON DELETE CASCADE
)
GO
