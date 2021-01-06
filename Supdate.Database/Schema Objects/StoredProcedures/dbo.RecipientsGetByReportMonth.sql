CREATE PROCEDURE [dbo].[RecipientsGetByReportMonth]
  @companyId INT,
  @reportDate DATETIME
AS
BEGIN
  DECLARE @ReportId INT

  SELECT @ReportId = r.Id FROM Report r WHERE r.CompanyId = @companyId AND r.[Date] = @reportDate

  SELECT
    s.Id, s.CompanyId, s.FirstName, s.LastName, s.Email, rn.Id AS ReportEmailId, rn.Status, rn.CreatedDate AS ReportEmailSentDate
  FROM
    Recipient s
  LEFT JOIN
    (SELECT Id, RecipientId, Status, CreatedDate, ROW_NUMBER() OVER (PARTITION BY RecipientId ORDER BY Id DESC) AS RowNo
      FROM ReportEmail
      WHERE ReportId = @ReportId and CompanyId = @companyId
    ) AS rn ON rn.RecipientId = s.Id and RowNo = 1
  WHERE
    s.CompanyId = @companyId

END
