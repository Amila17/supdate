CREATE PROCEDURE [dbo].[TermsAndConditionsAcceptLatest]
	@userId int
AS
BEGIN
	DECLARE @latestTermsId INT

  -- Get the id of the latest terms and conditions
  SELECT @latestTermsId = MAX(Id)
  FROM TermsAndConditions

  IF NOT EXISTS (SELECT 1
                  FROM UserTermsAndConditions
                  WHERE UserId = @userId
                    AND TermsAndConditionsId = @latestTermsId)
  BEGIN
    INSERT INTO UserTermsAndConditions
    (UserId, TermsAndConditionsId)
    VALUES
    (@userId, @latestTermsId)
  END

END
GO
