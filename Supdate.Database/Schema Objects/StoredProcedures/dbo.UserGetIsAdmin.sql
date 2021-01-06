CREATE PROCEDURE [dbo].[UserGetIsAdmin]
(
  @userId int
)
AS
BEGIN
  SELECT 1 FROM AdminUser
  WHERE UserId = @userId
END
