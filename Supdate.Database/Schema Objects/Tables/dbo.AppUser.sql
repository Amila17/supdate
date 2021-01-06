CREATE TABLE [dbo].[AppUser]
(
  [Id]                      INT IDENTITY(1,1)     NOT NULL,
  [UserName]                NVARCHAR(50)          NOT NULL,
  [Email]                   NVARCHAR(200)         NOT NULL,
  [FirstName]               NVARCHAR(200)         NULL,
  [LastName]                NVARCHAR(200)         NULL,
  [EmailConfirmed]          BIT                   NOT NULL,
  [UnConfirmedEmail]        NVARCHAR(256)         NULL,
  [PasswordHash]            NVARCHAR(500)         NOT NULL,
  [SecurityStamp]           NVARCHAR(50)          NOT NULL,
  [PhoneNumber]             NVARCHAR(20)          NULL,
  [PhoneNumberConfirmed]    BIT                   NULL,
  [TwoFactorEnabled]        BIT                   NULL,
  [LockoutEndDateUtc]       DATETIME              NULL,
  [LockoutEnabled]          BIT                   NOT NULL,
  [AccessFailedCount]       INT                   NOT NULL,
  [LoginCount]              INT                   NOT NULL DEFAULT 0,
  [LastLogin]               DATETIME              NULL,
  [UniqueId]                UNIQUEIDENTIFIER      NOT NULL DEFAULT NEWID(),
  [CreatedDate]             DATETIME              NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]             DATETIME              NULL,

  CONSTRAINT [PK_AppUser]
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
