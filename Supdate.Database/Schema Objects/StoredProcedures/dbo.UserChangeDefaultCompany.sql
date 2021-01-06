CREATE PROCEDURE [dbo].[UserChangeDefaultCompany]
  @CompanyUniqueId UNIQUEIDENTIFIER,
  @UserId INT
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @CompanyId INT

  -- get the numeric id for the company
  SELECT @CompanyId = Id
  FROM Company
  WHERE UniqueId = @CompanyUniqueId

 -- remove default flag for other companies for this user
  UPDATE CompanyUser
  SET DefaultCompany = 0
  WHERE UserId = @UserId

  -- set default flag for this company
  UPDATE CompanyUser
  SET DefaultCompany = 1
  WHERE UserId = @UserId
    AND CompanyId = @CompanyId
END
