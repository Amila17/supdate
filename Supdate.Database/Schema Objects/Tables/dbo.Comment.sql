CREATE TABLE [dbo].[Comment]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [UniqueId]          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [DiscussionId]      INT                 NOT NULL,
  [Text]              NVARCHAR(MAX)       NOT NULL,
  [AuthorEmail]       NVARCHAR(250)       NOT NULL,
  [AuthorName]        NVARCHAR(250)       NULL,
  [CreatedDate]       DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]       DATETIME            NULL,

  CONSTRAINT [PK_Comment]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Comment_Discussion]
    FOREIGN KEY ([DiscussionId])
    REFERENCES [dbo].[Discussion]([Id]) ON DELETE CASCADE
)
GO
