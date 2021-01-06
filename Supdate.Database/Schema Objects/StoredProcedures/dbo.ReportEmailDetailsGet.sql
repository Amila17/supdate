CREATE PROCEDURE [dbo].[ReportEmailDetailsGet]
  @reportEmailId int
AS
BEGIN
  --SET NOCOUNT ON added to prevent extra result sets from
  --interfering with SELECT statements.
  SET NOCOUNT ON;

  SELECT re.*
    , c.Name AS CompanyName
    , c.UniqueId as CompanyGuid
    , u.Email AS CompanyEmail
    , c.ReportTitle AS ReportTitle
    , r.[Date] AS ReportDate
    , r.UniqueId as ReportGuid
    , r.Summary AS ReportSummary
    , r.UniqueId AS ReportUniqueId
    , sh.Name AS RecipientName
    , sh.Email AS RecipientEmail
  FROM ReportEmail re
    INNER JOIN Company c ON c.Id = re.CompanyId
    INNER JOIN CompanyUser cu ON cu.CompanyId = c.Id
    INNER JOIN AppUser u ON u.Id = cu.UserId
    INNER JOIN Report r ON r.Id = re.ReportId
    INNER JOIN Recipient sh ON sh.Id = re.RecipientId
  WHERE re.Id = @reportEmailId

END
GO
