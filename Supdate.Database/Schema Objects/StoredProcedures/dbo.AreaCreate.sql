CREATE PROCEDURE [dbo].[AreaCreate]
  @companyId int,
  @name NVARCHAR(MAX)
AS
BEGIN
  INSERT INTO Area (CompanyId, Name, DisplayOrder, UpdatedDate)
  VALUES (@companyId, @name, 999, GETUTCDATE())

  SELECT SCOPE_IDENTITY()
END
