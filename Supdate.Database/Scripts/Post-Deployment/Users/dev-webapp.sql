CREATE USER [dev-webapp]
  FOR LOGIN [dev-webapp]
  WITH DEFAULT_SCHEMA = [dbo];
GO

-- Run this on [dev-supdate]
GRANT CONNECT TO [dev-webapp];
GO

EXEC sp_addrolemember N'db_owner', N'dev-webapp'
GO

-- Run this on [dev-exceptional]
GRANT CONNECT TO [dev-webapp];
GO

EXEC sp_addrolemember N'db_owner', N'dev-webapp'
GO
