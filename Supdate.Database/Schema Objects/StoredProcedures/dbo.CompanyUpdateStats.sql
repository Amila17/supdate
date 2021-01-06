CREATE PROCEDURE [dbo].[CompanyUpdateStats]
  @companyId int
AS
BEGIN
  SET NOCOUNT ON
  -- variables
  DECLARE @Areas int
  DECLARE @Metrics int
  DECLARE @Goals int
  DECLARE @ReportsTotal int
  DECLARE @Users int

  -- fill variables
  SELECT @Areas = COUNT(Id) FROM Area WHERE CompanyId = @companyId
  SELECT @Metrics = COUNT(Id) FROM Metric WHERE CompanyId = @companyId
  SELECT @Goals = COUNT(Id) FROM Goal WHERE CompanyId = @companyId
  SELECT @ReportsTotal = COUNT(Id) FROM Report WHERE CompanyId = @companyId
  SELECT @Users = COUNT(Id) FROM CompanyUser WHERE CompanyId = @companyId

  -- update company stats
  IF EXISTS (SELECT 1 FROM CompanyStats WHERE CompanyId = @companyId)
    UPDATE CompanyStats
    SET AreaCount = @Areas,
      MetricCount = @Metrics,
      GoalCount = @Goals,
      ReportCount = @ReportsTotal,
      UserCount = @Users,
      UpdatedDate = GETUTCDATE()
    WHERE CompanyId = @companyId
  ELSE
    INSERT INTO CompanyStats
      (CompanyId, AreaCount, MetricCount, GoalCount, ReportCount, UserCount, UpdatedDate)
    VALUES
      (@companyId, @Areas, @Metrics, @Goals, @ReportsTotal, @Users, GETUTCDATE())

END
