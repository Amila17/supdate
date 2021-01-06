CREATE TABLE [dbo].[CompanyUserAreaPermission]
(
  [Id]                  INT IDENTITY(1, 1)    NOT NULL,
  [UserId]              INT                   NOT NULL,
  [CompanyId]           INT                   NOT NULL,
  [AreaId]              INT                   NOT NULL

  CONSTRAINT [PK_CompanyUserAreaPermission]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_CompanyUserAreaPermission_AreaId]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]([Id]) ON DELETE CASCADE,

  CONSTRAINT [FK_CompanyUserAreaPermission_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AppUser]([Id]) ON DELETE CASCADE
)
