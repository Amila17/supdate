CREATE PROCEDURE [dbo].[UsersGetUnconfirmed]
(
  @registeredBeforeHours int = 24
)
AS
BEGIN

  IF (NOT EXISTS (SELECT 1 FROM UserConfirmation) OR @registeredBeforeHours = 0)
  BEGIN
    SELECT u.Id as UserId, u.Email, u.UniqueId, uc.Url, uc.RemindersSent, u.CreatedDate, uc.UpdatedDate
    FROM AppUser u
      LEFT JOIN UserConfirmation uc ON uc.UserId = u.Id
    WHERE u.EmailConfirmed = 0 AND u.UnConfirmedEmail is null
    ORDER BY u.Id DESC
  END
  ELSE
  BEGIN
    SELECT uc.Id, u.Id AS UserId, u.Email, u.UniqueId, uc.Url, u.CreatedDate, uc.UpdatedDate
    FROM AppUser u
    INNER JOIN UserConfirmation uc ON uc.UserId = u.Id
    WHERE u.EmailConfirmed = 0
      AND u.UnConfirmedEmail is null
      AND uc.RemindersSent = 0
      AND u.CreatedDate < DATEADD(hour, -@registeredBeforeHours, GETUTCDATE())
    ORDER BY u.Id DESC
  END
END
