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
--MERGE SQL statement - Part 2
--Synchronize the target table with
--refreshed data from source table
MERGE TermsAndConditions AS TARGET
USING UpdatedTermsAndConditions AS SOURCE 
ON (TARGET.ProductID = SOURCE.ProductID) 
--When records are matched, update 
--the records if there is any change
WHEN MATCHED AND TARGET.ProductName <> SOURCE.ProductName 
OR TARGET.Rate <> SOURCE.Rate THEN 
UPDATE SET TARGET.ProductName = SOURCE.ProductName, 
TARGET.Rate = SOURCE.Rate 
--When no records are matched, insert
--the incoming records from source
--table to target table
WHEN NOT MATCHED BY TARGET THEN 
INSERT (ProductID, ProductName, Rate) 
VALUES (SOURCE.ProductID, SOURCE.ProductName, SOURCE.Rate)
--When there is a row that exists in target table and
--same record does not exist in source table
--then delete this record from target table
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
--$action specifies a column of type nvarchar(10) 
--in the OUTPUT clause that returns one of three 
--values for each row: 'INSERT', 'UPDATE', or 'DELETE', 
--according to the action that was performed on that row
OUTPUT $action, 
DELETED.ProductID AS TargetProductID, 
DELETED.ProductName AS TargetProductName, 
DELETED.Rate AS TargetRate, 
INSERTED.ProductID AS SourceProductID, 
INSERTED.ProductName AS SourceProductName, 
INSERTED.Rate AS SourceRate; 

SELECT @@ROWCOUNT;
GO
