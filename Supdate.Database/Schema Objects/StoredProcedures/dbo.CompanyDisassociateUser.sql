CREATE PROCEDURE [dbo].[CompanyDisassociateUser]
  @companyId INT,
  @userId INT
AS
BEGIN
  SET NOCOUNT ON;

  -- Make sure there is one other user associated to this company.
  IF (SELECT Count(1) FROM CompanyUser WHERE CompanyId = @companyId AND UserId <> @userId) > 0
  BEGIN
    DELETE CompanyUser
    WHERE CompanyId = @companyId AND UserId = @userId

    RETURN @@ROWCOUNT;
  END

  RETURN 0;
END
