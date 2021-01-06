CREATE TABLE [dbo].[CompanyUserInviteAreaPermission]
(
  [Id]                   INT IDENTITY(1, 1)    NOT NULL,
  [InviteId]             INT                   NOT NULL,
  [AreaId]               INT                   NOT NULL

  CONSTRAINT [PK_CompanyUserInviteAreaPermission]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_CompanyUserInviteAreaPermission_InviteId]
    FOREIGN KEY ([InviteId])
    REFERENCES [dbo].[CompanyUserInvite]([Id]) ON DELETE CASCADE,

  CONSTRAINT [FK_CompanyUserInviteAreaPermission_AreaId]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id]) ON DELETE CASCADE
)
GO
