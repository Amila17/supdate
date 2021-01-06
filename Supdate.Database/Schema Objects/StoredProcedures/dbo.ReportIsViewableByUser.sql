CREATE PROCEDURE [dbo].[ReportIsViewableByUser]
  @reportGuid UNIQUEIDENTIFIER,
  @userId INT
AS
BEGIN
  -- Get company id for report
  DECLARE @companyId INT

  SELECT @companyId = CompanyId FROM Report WHERE UniqueId = @reportGuid

  IF (@companyId IS NULL)
  BEGIN
    SELECT 0 AS result
    RETURN
  END

  DECLARE @check INT

  -- Is user an admin of this company or able to view reports for it?
  SELECT TOP 1 @check = Id FROM CompanyUser
  WHERE CompanyId = @companyId
    AND UserId = @userId
    AND (IsOwner = 1 OR CanViewReports = 1)

  IF (@check IS NOT NULL)
  BEGIN
    SELECT 1 AS result
    RETURN
  END

  -- is user an app admin?
  SELECT TOP 1 @check = Id FROM AdminUser
  WHERE UserId = @userId

  IF(@check IS NOT NULL)
  BEGIN
    SELECT 1 AS result
    RETURN
  END

 SELECT 0 AS result
END
GO
