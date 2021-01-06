CREATE PROCEDURE [dbo].[RecipientGet]
  @companyId INT,
  @recipientGuid UNIQUEIDENTIFIER
AS
BEGIN
  DECLARE @id int
  SELECT
    @id = Id
  FROM Recipient
  WHERE CompanyId = @companyId
  AND UniqueId = @recipientGuid

  SELECT *
  FROM Recipient
  WHERE id = @id
END
