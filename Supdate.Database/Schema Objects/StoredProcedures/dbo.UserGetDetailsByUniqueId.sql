CREATE PROCEDURE [dbo].[UserGetDetailsByUniqueId]
(
  @uniqueId UNIQUEIDENTIFIER
)
AS
BEGIN
  DECLARE @registeredUser AS TABLE
  (
    id INT
    , uniqueId uniqueidentifier
    , email NVARCHAR(50)
    , emailConfirmed BIT
    , lockoutEnabled BIT
    , createdDate DATETIME
    , lastLogin DATETIME
    , loginCount INT
    , companyCount INT
    , companyId INT
    , companyName NVARCHAR(300)
    , companyUniqueId UNIQUEIDENTIFIER
  )

  INSERT INTO @registeredUser
    (id, uniqueId, email, emailConfirmed, lockoutEnabled, createdDate, lastLogin, loginCount)
  SELECT Id, UniqueId, Email, EmailConfirmed, LockoutEnabled, CreatedDate, LastLogin, LoginCount
  FROM AppUser
  WHERE UniqueId = @uniqueId

  DECLARE @userId INT, @companyId INT, @companyName NVARCHAR(300), @companyUniqueId uniqueidentifier

  SELECT @userId = id FROM @registeredUser

  --get primary company details
  SELECT
     @companyId = c.Id
    ,@companyName = c.Name
    ,@companyUniqueId = c.UniqueId
  FROM Company c
  WHERE c.Id =
  (SELECT TOP 1 CompanyId FROM CompanyUser WHERE IsOwner = 1 AND UserId = @userId  ORDER BY Id)

  --------------------------------------------------
  -- Insert company details and company count
  --------------------------------------------------
  UPDATE @registeredUser
  SET companyCount = (SELECT count(Id) FROM CompanyUser WHERE UserId = @userId),
    companyId = @companyId,
    companyName = @companyName,
    companyUniqueId = @companyUniqueId

  --------------------------------------------------
  -- return results
  --------------------------------------------------
  SELECT * FROM @registeredUser
END
