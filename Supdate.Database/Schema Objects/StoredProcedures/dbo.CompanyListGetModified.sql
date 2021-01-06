CREATE PROCEDURE [dbo].[CompanyListGetModified]
AS
BEGIN
  DECLARE @updationDate DATETIME

  SELECT @updationDate = MAX(UpdatedDate) FROM CompanyStats

  SELECT DISTINCT CompanyId FROM Area WHERE UpdatedDate > @updationDate
  UNION
  SELECT DISTINCT CompanyId FROM Goal WHERE UpdatedDate > @updationDate
  UNION
  SELECT DISTINCT CompanyId FROM Metric WHERE UpdatedDate > @updationDate
  UNION
  SELECT DISTINCT CompanyId FROM Report WHERE UpdatedDate > @updationDate
  UNION
  SELECT DISTINCT CompanyId FROM CompanyUser WHERE UpdatedDate > @updationDate
  UNION
  SELECT DISTINCT CompanyId FROM Subscription WHERE UpdatedDate > @updationDate
  UNION
  SELECT Id FROM Company WHERE Id NOT IN (SELECT CompanyId FROM CompanyStats)
END
