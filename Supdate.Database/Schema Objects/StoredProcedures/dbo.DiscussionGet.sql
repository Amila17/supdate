CREATE PROCEDURE [dbo].[DiscussionGet]
  @reportGuid UNIQUEIDENTIFIER,
  @targetType SMALLINT,
  @targetGuid UNIQUEIDENTIFIER
AS
BEGIN
  DECLARE @discussionId INT
  DECLARE @companyId INT
  DECLARE @title NVARCHAR(200)

  SELECT
    @discussionId = Id,
    @companyId = CompanyId
  FROM Discussion
  WHERE ReportGuid = @reportGuid
    AND TargetType = @targetType
    AND [Target] = @targetGuid

  -- Summary
  IF (@targetType = 0)
  BEGIN
    SET @title = 'Summary'
  END

  -- Area
  IF (@targetType = 1)
  BEGIN
    SELECT @title = Name FROM Area WHERE UniqueId = @targetGuid AND CompanyId = @companyId
  END

  -- Metric
  IF (@targetType = 2)
  BEGIN
    SELECT @title = Name FROM Metric WHERE UniqueId = @targetGuid AND CompanyId = @companyId
  END

  -- Goal
  IF (@targetType = 3)
  BEGIN
    SELECT @title = Title FROM Goal WHERE UniqueId = @targetGuid AND CompanyId = @companyId
  END

  SELECT @title as Title, r.Id as ReportId, r.Date as ReportDate, d.*
  FROM Discussion d
  INNER JOIN Report r on d.ReportGuid = r.UniqueId
  WHERE d.Id = @discussionId

  SELECT * FROM Comment WHERE DiscussionId = @discussionId
END
GO
