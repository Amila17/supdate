CREATE TABLE [dbo].[TermsAndConditions]
(
	[Id]            INT IDENTITY(1, 1)     NOT NULL,
  [Description]   NVARCHAR(400),
  [CreatedDate]   DATETIME               NOT NULL DEFAULT GETUTCDATE(),

  CONSTRAINT [PK_TAndC]
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
