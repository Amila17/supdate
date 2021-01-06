CREATE PROCEDURE [dbo].[CompanyAssociateUser]
  @companyId INT,
  @userId INT,
  @isOwner INT = 1,
  @canViewReports int = 0
AS
BEGIN
  IF(@isOwner = 1) SET @canViewReports = 1

  IF NOT EXISTS (SELECT 1 FROM CompanyUser WHERE CompanyId = @companyId AND UserId = @userId)
  BEGIN
    INSERT INTO CompanyUser
      (CompanyId, UserId, IsOwner, CanViewReports)
    VALUES
      (@companyId, @userId, @isOwner, @canViewReports)
  END

  RETURN @@ROWCOUNT;
END
