CREATE PROCEDURE [dbo].[UsersGetRegistered]
(
  @desiredPageNumber INT = 1,
  @numberOfRows INT = 10,
  @sortOption INT = 0 -- 0 = id, 1 = logins, 2 = lastLogin
)
AS
BEGIN
  DECLARE @registeredUser AS TABLE
  (
    id INT
    , UniqueId uniqueidentifier
    , email NVARCHAR(50)
    , emailConfirmed BIT
    , lockoutEnabled BIT
    , createdDate DATETIME
    , lastLogin DATETIME
    , logincount int
    , companycount int
    , companyId int
    , companyName NVARCHAR(300)
    , companyUniqueId uniqueidentifier
    , isCompanyAdmin BIT
  )

  INSERT INTO @registeredUser
    (id, UniqueId, email, emailConfirmed, lockoutEnabled, createdDate, lastLogin, logincount, isCompanyAdmin)
  SELECT Id, UniqueId, Email, EmailConfirmed, LockoutEnabled, CreatedDate, LastLogin, LoginCount, 0
  FROM AppUser WHERE EmailConfirmed = 1
  ORDER By
  CASE @sortOption
    WHEN 0
      then Id
    WHEN 1
      then LoginCount
    WHEN 2
      then LastLogin
  END desc
  OFFSET (@numberOfRows * (@desiredPageNumber - 1)) ROWS
  FETCH NEXT @NumberOfRows ROWS ONLY;

  -- Get Company Details for Company Owner
  UPDATE @registeredUser
  SET companyId = (SELECT TOP 1 CompanyId
                    FROM CompanyUser
                    WHERE IsOwner = 1
                      AND UserId = [@registeredUser].id
                    ORDER BY Id ASC)

  UPDATE @registeredUser
  SET isCompanyAdmin = 1
  WHERE companyId IS NOT NULL

  -- Get Company Details for non Owners
  UPDATE @registeredUser
  SET companyId = (SELECT TOP 1 CompanyId
                    FROM CompanyUser
                    WHERE UserId = [@registeredUser].id
                    ORDER BY Id ASC)
  WHERE companyId IS NULL

  -- Get Company Count
  UPDATE @registeredUser
  SET companycount = (SELECT count(CompanyId)
                      FROM CompanyUser
                      WHERE UserId = [@registeredUser].id)

  UPDATE u
    SET companyName = c.Name
      ,companyUniqueId = c.UniqueId
  FROM @registeredUser u
  INNER JOIN Company c ON c.Id = u.companyId

  SELECT * FROM @registeredUser
END
