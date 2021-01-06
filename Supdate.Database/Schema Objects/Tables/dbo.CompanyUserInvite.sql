CREATE TABLE [dbo].[CompanyUserInvite]
(
  [Id]                      INT IDENTITY(1, 1)    NOT NULL,
  [CompanyId]               INT                   NOT NULL,
  [EmailAddress]            NVARCHAR(200)         NOT NULL,
  [CanViewReports]          BIT                   NOT NULL DEFAULT 0,
  [UsedDate]                DATETIME,
  [ResultantUserId]         INT,
  [UniqueId]                UNIQUEIDENTIFIER      NOT NULL DEFAULT NEWID(),
  [CreatedDate]             DATETIME              NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]             DATETIME              NULL,

   CONSTRAINT [PK_CompanyUserInvite]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_CompanyUserInvite_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
