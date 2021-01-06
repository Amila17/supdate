CREATE PROCEDURE [dbo].[ReportEmailDetailsGetByAddress]
  @companyId INT,
  @reportId INT,
  @emailAddress NVARCHAR(200)
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @RecipientId INT
  DECLARE @ReportEmailId INT

  SELECT @RecipientId = Id
  FROM Recipient
  WHERE CompanyId = @companyId
    AND Email = @emailAddress

  SELECT @ReportEmailId = Id
  FROM ReportEmail
  WHERE RecipientId = @RecipientId
   AND ReportId = @reportId

  EXEC ReportEmailDetailsGet @ReportEmailId
END
GO
