CREATE PROCEDURE [dbo].[UsersGetNew]
(
  @registeredBeforeHours int = 72
)
AS
BEGIN
  SELECT u.Id, u.Email, u.UniqueId, u.CreatedDate, u.UpdatedDate
  FROM AppUser u
  LEFT OUTER JOIN UserEmail ue on u.Id = ue.UserId
  WHERE u.EmailConfirmed = 1
    AND u.CreatedDate < DATEADD(hour, -@registeredBeforeHours, GETUTCDATE())
    AND ISNULL(ue.FeedbackEmailSent, 0) = 0
END
