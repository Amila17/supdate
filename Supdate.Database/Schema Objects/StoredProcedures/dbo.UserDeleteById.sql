CREATE PROCEDURE [dbo].[UserDeleteById]
  @userId INT,
  @deleteOwner BIT = 0
AS
BEGIN
  SET NOCOUNT ON;

  IF NOT EXISTS (SELECT 1 FROM CompanyUser WHERE UserId = @userId AND IsOwner = 1)
  BEGIN
    DELETE CompanyUser
    WHERE UserId = @userId

    DELETE AppUser
    WHERE Id = @userId
  END

  RETURN @@ROWCOUNT
END
