CREATE PROCEDURE [dbo].[UserProfileUpdate]
  @Id INT,
  @firstName NVARCHAR(200),
  @lastName NVARCHAR(200)
AS
BEGIN
  UPDATE AppUser
  SET FirstName = @firstName, LastName = @lastName
  WHERE Id = @Id
END
GO
