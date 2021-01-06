CREATE TABLE [dbo].[Area]
(
  [Id]            INT IDENTITY(1, 1)  NOT NULL,
  [Name]          NVARCHAR(200)       NOT NULL,
  [UniqueId]          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
  [CompanyId]     INT                 NOT NULL,
  [DisplayOrder]  INT                 NOT NULL DEFAULT 0,
  [CreatedDate]   DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]   DATETIME            NULL,

  CONSTRAINT [PK_Area]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Area_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
