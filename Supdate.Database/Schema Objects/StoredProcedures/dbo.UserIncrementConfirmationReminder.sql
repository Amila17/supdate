CREATE PROCEDURE [dbo].[UserIncrementConfirmationReminder]
(
  @userId int
)
AS
BEGIN
  UPDATE UserConfirmation
  SET RemindersSent = RemindersSent + 1
    , UpdatedDate = GETUTCDATE()
  WHERE UserId = @userId
END
