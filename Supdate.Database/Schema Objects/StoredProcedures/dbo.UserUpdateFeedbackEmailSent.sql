CREATE PROCEDURE [dbo].[UserUpdateFeedbackEmailSent]
(
  @userId int
)
AS
BEGIN
  INSERT INTO UserEmail
  (UserId, FeedbackEmailSent, UpdatedDate)
  VALUES
  (@userId, 1, GETUTCDATE())
END
