CREATE PROCEDURE [dbo].[ExternalApiAuthGetByCompanyGuid]
  @companyGuid UNIQUEIDENTIFIER,
  @externalApiId int
AS
  DECLARE @CompanyId int

  SELECT @CompanyId = Id from Company Where UniqueId = @companyGuid

  SELECT * FROM ExternalApiAuth WHERE CompanyId = @CompanyId AND ExternalApiId = @externalApiId

RETURN 0
