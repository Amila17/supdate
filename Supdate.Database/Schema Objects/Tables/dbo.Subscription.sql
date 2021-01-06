CREATE TABLE [dbo].[Subscription]
(
  [Id]                      INT IDENTITY(1, 1)  NOT NULL,
  [CompanyId]               INT                 NOT NULL,
  [Status]                  SMALLINT            NOT NULL,
  [ExpiryDate]              DATETIME            NOT NULL,
  [StripeCustomerId]        NVARCHAR(50)        NULL,
  [StripeSubscriptionId]    NVARCHAR(50)        NULL,
  [CreatedDate]             DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]             DATETIME            NULL,

  CONSTRAINT [PK_Subscription]
    PRIMARY KEY CLUSTERED (Id ASC),

  CONSTRAINT [FK_Subscription_Company]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Company]([Id])
)
GO
