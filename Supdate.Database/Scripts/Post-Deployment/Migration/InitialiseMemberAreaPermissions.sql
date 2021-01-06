--SELECT * FROM AppUser u
--INNER JOIN CompanyUser cu ON u.Id = cu.UserId
--WHERE cu.isOwner = 0

---- Make entries for the above list of users in the CompanyUserAreaPermission (for every area in the Company)


--SELECT * FROM CompanyUserInvite i
--INNER JOIN CompanyUser cu ON i.CompanyId = cu.CompanyId
--WHERE i.ResultantUserId IS NULL

---- Make entries for the above list of invites in the CompanyUserInviteAreaPermission (for every area in the Company)


DECLARE @companyId INT
DECLARE @userId INT
DECLARE @inviteId INT

-- Initialise permissions for existing team members
DECLARE companyUserCursor CURSOR FOR
SELECT CompanyId, UserId FROM CompanyUser cu
WHERE cu.isOwner = 0

OPEN companyUserCursor
FETCH NEXT FROM companyUserCursor INTO @CompanyId, @userId

-- Start the loop
WHILE (@@FETCH_STATUS = 0)
BEGIN

  -- Insert the user permissions for team members who have already accepted the invite.
  IF NOT EXISTS (SELECT 1 FROM CompanyUserAreaPermission WHERE UserId = @userId AND CompanyId = @companyId)
  BEGIN
    INSERT INTO CompanyUserAreaPermission
    SELECT @userId, @companyId, Id
    FROM Area
    WHERE CompanyId = @companyId
  END

  FETCH NEXT FROM companyUserCursor INTO @companyId, @userId
END
CLOSE companyUserCursor
DEALLOCATE companyUserCursor



-- Initialise permissions for team member invites that have not been invited so far
DECLARE companyInviteCursor CURSOR FOR
SELECT Id, CompanyId FROM CompanyUserInvite i
WHERE i.ResultantUserId IS NULL

OPEN companyInviteCursor
FETCH NEXT FROM companyInviteCursor INTO @inviteId, @CompanyId

-- Start the loop
WHILE (@@FETCH_STATUS = 0)
BEGIN

  -- Insert the user permissions for pending team member invitations.
  IF NOT EXISTS (SELECT 1 FROM CompanyUserInviteAreaPermission WHERE InviteId = @inviteId)
  BEGIN
    INSERT INTO CompanyUserInviteAreaPermission
    SELECT @inviteId, Id
    FROM Area
    WHERE CompanyId = @companyId
  END

  FETCH NEXT FROM companyInviteCursor INTO @inviteId, @CompanyId
END
CLOSE companyInviteCursor
DEALLOCATE companyInviteCursor
