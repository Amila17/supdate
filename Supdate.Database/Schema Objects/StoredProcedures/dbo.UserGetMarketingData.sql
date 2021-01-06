CREATE PROCEDURE [dbo].[UserGetMarketingData]
  @uid INT
AS
  SET NOCOUNT ON
  -- get company id
  -- in case user owns multiple companies, get the first one
  DECLARE @companyID INT
  DECLARE @companyName nvarchar(200)
  SELECT TOP 1
    @companyID = cu.CompanyId,
    @companyName = c.Name
  FROM CompanyUser cu
   INNER JOIN Company c ON cu.CompanyId = c.Id
  WHERE cu.UserId = @uid
  ORDER BY c.Id

  -- variables
  DECLARE @Areas INT
  DECLARE @Metrics INT
  DECLARE @Goals INT
  DECLARE @ReportsStarted INT = 0
  DECLARE @ReportsCompleted INT = 0
  DECLARE @ReportsSent INT = 0
  DECLARE @ReportsTotal INT
  DECLARE @subscriptionStatus INT
  DECLARE @subscriptionExpiryDate DATETIME

  -- fill variables
  SELECT @Areas = COUNT(Id) FROM Area WHERE CompanyId = @companyID
  SELECT @Metrics = COUNT(Id) FROM Metric WHERE CompanyId = @companyID
  SELECT @Goals = COUNT(Id) FROM Goal WHERE CompanyId = @companyID
  SELECT @ReportsStarted = COUNT(Id) FROM Report WHERE CompanyId = @companyID AND StatusId = 1 -- InProgress = 1
  SELECT @ReportsCompleted = COUNT(Id) FROM Report WHERE CompanyId = @companyID AND StatusId = 2 -- Completed = 2
  SELECT @ReportsSent = COUNT(r.Id)
    FROM Report r
    INNER JOIN ReportEmail re ON r.Id = re.ReportId
    WHERE r.CompanyId = @companyID
      AND r.StatusId = 2 -- Completed = 2
      AND re.Status = 2 -- Sent = 2
  SELECT @ReportsTotal = COUNT(Id) FROM Report WHERE CompanyId = @companyID
  SELECT @subscriptionStatus = s.[Status], @subscriptionExpiryDate = s.ExpiryDate FROM Subscription s WHERE s.CompanyId = @companyID

  --return data
  SELECT
    Email ,
    @companyName AS CompanyName,
    @Areas AS Areas,
    @Metrics AS Metrics,
    @Goals AS Goals,
    @ReportsStarted AS ReportsStarted,
    @ReportsCompleted AS ReportsCompleted,
    @ReportsSent AS ReportsSent,
    @ReportsTotal AS ReportsTotal,
    LastLogin,
    LoginCount,
    @subscriptionStatus AS SubscriptionStatus,
    @subscriptionExpiryDate AS SubscriptionExpiryDate
  FROM AppUser WHERE Id = @uid
