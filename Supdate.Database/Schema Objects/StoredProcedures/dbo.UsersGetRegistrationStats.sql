CREATE PROCEDURE [dbo].[UsersGetRegistrationStats]
(
  @windowInDays int = 1
)
AS
BEGIN
  DECLARE @fromdate_full DATETIME = DATEADD(day, -@windowInDays, GETUTCDATE())
  DECLARE @fromdate_midnight DATETIME = CAST(FLOOR(CAST(@fromdate_full AS FLOAT)) AS DATETIME)

  SELECT
    (SELECT Count(1) FROM AppUser) AS Total
    , (SELECT Count(1) FROM AppUser WHERE EmailConfirmed = 1) AS TotalEmailConfirmed
    , (SELECT Count(1) FROM Report) AS TotalReports
    , (SELECT Count(1) FROM ReportEmail) AS TotalReportsEmailed
    , (SELECT Count(1) FROM AppUser WHERE CreatedDate > @fromdate_midnight) AS TotalInWindow
    , (SELECT Count(1) FROM AppUser WHERE EmailConfirmed = 1 AND CreatedDate >  @fromdate_midnight) AS TotalEmailConfirmedInWindow
    , (SELECT Count(1) FROM Report WHERE CreatedDate > @fromdate_midnight) AS TotalReportsInWindow
    , (SELECT Count(1) FROM ReportEmail WHERE CreatedDate > @fromdate_midnight) AS TotalReportsEmailedInWindow
    , (SELECT Count(1) FROM AppUser WHERE EmailConfirmed = 0) AS UnconfirmedAccounts
    , (SELECT Count(1) FROM AppUser WHERE LastLogin > @fromdate_midnight) AS LoginsInWindow
END
