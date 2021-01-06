CREATE TABLE [dbo].[ReportGoal]
(
  [Id]                INT IDENTITY(1, 1)  NOT NULL,
  [GoalId]            INT                 NOT NULL,
  [ReportId]          INT                 NOT NULL,
  [DueDate]           DATETIME            NULL,
  [Status]            SMALLINT            NOT NULL,
  [Summary]           NVARCHAR(MAX)       NULL,
  [CreatedDate]       DATETIME            NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedDate]       DATETIME            NULL,

  CONSTRAINT [PK_ReportGoal]
    PRIMARY KEY CLUSTERED ([Id] ASC),

  CONSTRAINT [FK_ReportGoal_Goal]
    FOREIGN KEY ([GoalId])
    REFERENCES [Goal]([Id]),

  CONSTRAINT [FK_ReportGoal_Report]
    FOREIGN KEY ([ReportId])
    REFERENCES [dbo].[Report]([Id])
    ON DELETE CASCADE
)
GO
