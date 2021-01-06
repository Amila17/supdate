-- Run this on [prod-supdate]
CREATE USER [prod-webapp]
  FOR LOGIN [prod-webapp]
  WITH DEFAULT_SCHEMA = [dbo];
GO

GRANT CONNECT TO [prod-webapp];
GO

EXEC sp_addrolemember N'db_owner', N'prod-webapp'
GO
