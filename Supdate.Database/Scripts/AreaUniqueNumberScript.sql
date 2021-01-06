-- No constraint added yet, so record check is needed rather than constraint check. Need to modify after making schema changes

ALTER TABLE Area ADD [UniqueNumber] INT NULL
GO

IF OBJECT_ID('dbo.Area') IS NOT NULL
BEGIN
IF ((
      (
    SELECT COUNT(*)
    FROM dbo.Area
    WHERE ISNULL(UniqueNumber, 0) = 0
    ) > 0
      )
    )
BEGIN
  BEGIN TRANSACTION

  BEGIN TRY
    PRINT 'Starting unique number generation for Area';

    WITH ZeroAndNullCTE
    AS (
      SELECT a.CompanyId
        , a.Id
        , a.UniqueNumber
        , NewSort = ROW_NUMBER() OVER (
          PARTITION BY a.CompanyId ORDER BY UniqueNumber ASC
            , a.Id ASC
          )
      FROM dbo.Area a
      INNER JOIN (
        SELECT DISTINCT CompanyId
        FROM dbo.Area
        WHERE UniqueNumber = 0
          OR UniqueNumber IS NULL
        ) dt
        ON dt.CompanyId = a.CompanyId
      )
    UPDATE ZeroAndNullCTE
    SET UniqueNumber = NewSort

    IF EXISTS (
        SELECT 1
        FROM dbo.Area
        WHERE ISNULL(UniqueNumber, 0) = 0
        )
    BEGIN
      RAISERROR (
          'Unique number generation failed'
          , 11
          , 1
          );
    END;

    PRINT 'Completed Unique number generation for Area'

    ALTER TABLE Area ALTER COLUMN [UniqueNumber] INT NOT NULL
    COMMIT TRANSACTION;
  END TRY

  BEGIN CATCH
    IF (XACT_STATE() != 0)
    BEGIN
      ROLLBACK TRANSACTION;
    END

  END CATCH
END
END
