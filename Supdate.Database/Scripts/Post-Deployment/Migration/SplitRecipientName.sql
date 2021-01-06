/*
Post-Deployment Script Template
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.
 Use SQLCMD syntax to include a file in the post-deployment script.
 Example:      :r .\myfile.sql
 Use SQLCMD syntax to reference a variable in the post-deployment script.
 Example:      :setvar TableName MyTable
               SELECT * FROM [$(TableName)]
--------------------------------------------------------------------------------------
*/

UPDATE Recipient SET
  FirstName = SUBSTRING(name, 1, CHARINDEX(' ', name) - 1) ,
  LastName = SUBSTRING(name, CHARINDEX(' ', name) + 1, 100)
  WHERE  CHARINDEX(' ', name) > 0 AND FirstName is null
