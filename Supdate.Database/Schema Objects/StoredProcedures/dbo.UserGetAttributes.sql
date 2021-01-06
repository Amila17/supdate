CREATE PROCEDURE [dbo].[UserGetAttributes]
  @userId INT,
  @companyId INT = 0
AS
BEGIN
  SET NOCOUNT ON;

  IF (@companyId = 0)
  BEGIN
    EXEC UserGetDefaultCompanyId @userId, @companyId OUTPUT
  END

  DECLARE @isCompanyAdmin INT
  DECLARE @latestTermsId INT
  SELECT @isCompanyAdmin = IsOwner FROM CompanyUser WHERE UserId = @userId AND CompanyId = @companyId
  SELECT @latestTermsId = max(Id) FROM TermsAndConditions

  SELECT TOP 1 u.Id
    , u.Email
    , u.UnConfirmedEmail
    , u.FirstName
    , u.LastName
    , c.Id AS CompanyId
    , c.Name AS Company
    , c.LogoPath as LogoPath
    , CASE (cu.IsOwner)
        WHEN 1 THEN 1
        ELSE cu.CanViewReports END AS CanViewReports
    , cu.IsOwner AS IsCompanyAdmin
    , s.[Status] AS SubscriptionStatus
    , s.ExpiryDate AS SubscriptionExpiryDate
    , case
        WHEN (s.StripeCustomerId) IS NULL THEN 0
        ELSE 1 END AS HasValidSubscription
    , (SELECT 1 FROM AdminUser WHERE UserId = @userId) AS IsAdmin
    , isnull(utc.TermsAndConditionsId, 0) as AcceptedLatestTerms
  FROM AppUser u
  INNER JOIN CompanyUser cu ON cu.UserId = u.Id
  INNER JOIN Company c ON c.Id = cu.CompanyId
  LEFT JOIN Subscription s ON s.CompanyId = c.Id
  LEFT JOIN UserTermsAndConditions utc On utc.UserId = u.Id AND utc.TermsAndConditionsId = @latestTermsId
  WHERE u.Id = @userId
    AND c.Id = @companyId
    AND
    (
      (@isCompanyAdmin = 1)
      OR -- Team Members Can only access companies with a valid subscription
      (s.ExpiryDate >= GETUTCDATE())
    )

  -- Get the list of companies that this user is associated with.
  EXEC CompanyGetListForUser @userId

  --get list of accessible areas
  IF (@isCompanyAdmin = 1)
  BEGIN
    SELECT Id as AreaId FROM Area WHERE CompanyId = @companyId
  END
  ELSE
  BEGIN
    SELECT AreaId
    FROM CompanyUserAreaPermission
    WHERE UserId = @userId AND CompanyId = @companyId
  END
END
