CREATE PROCEDURE [dbo].[AdminSearch]
	@query nvarchar(200)
AS
	SELECT * FROM AppUser WHERE Email LIKE '%' + @query + '%'
	SELECT * FROM Company WHERE Name LIKE '%' + @query + '%'