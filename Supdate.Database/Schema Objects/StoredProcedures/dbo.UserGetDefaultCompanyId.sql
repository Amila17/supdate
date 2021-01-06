CREATE PROCEDURE [dbo].[UserGetDefaultCompanyId]
  @userId INT,
  @companyId INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;

  -- Returns either the default company, or - if none exists - the first company found for the user
  SELECT TOP 1
    @companyId = CompanyId
  FROM CompanyUser
  WHERE UserId = @userId
  ORDER BY DefaultCompany DESC, IsOwner DESC, Id DESC
END
