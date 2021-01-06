CREATE PROCEDURE [dbo].[CompanyDelete]
	@id int,
	@name nvarchar(300)
AS
	DECLARE @deleteId int
	DECLARE @reporttbl TABLE (
	 reportId int,
	 reportGuid uniqueidentifier
	)

	DECLARE @metricTbl TABLE (
	 metricId int
	)
	
	SELECT @deleteId = Id FROM Company WHERE id=@id AND Name = @name

	-- Users, Area Permiessions and Invites
	DELETE FROM CompanyUserAreaPermission WHERE CompanyId = @deleteId
	DELETE FROM CompanyUserInviteAreaPermission WHERE InviteId IN (SELECT Id From CompanyUserInvite WHERE CompanyId = @deleteId)
	DELETE FROM CompanyUserInvite WHERE CompanyId = @deleteId
	DELETE FROM CompanyUser WHERE CompanyId = @deleteId

	-- Reports
	INSERT INTO @reporttbl
	SELECT Id, UniqueId FROM Report WHERE CompanyId = @deleteId

	DELETE FROM ReportEmail WHERE CompanyId = @deleteId
	DELETE FROM ReportGoal WHERE ReportId IN (SELECT reportId FROM @reporttbl)
	DELETE FROM ReportMetric WHERE ReportId IN (SELECT reportId FROM @reporttbl)
	DELETE FROM ReportArea WHERE ReportId IN (SELECT reportId FROM @reporttbl)
	DELETE FROM ReportAttachment WHERE ReportId IN (SELECT reportId FROM @reporttbl)
	DELETE FROM Report WHERE CompanyId = @deleteId

	-- Discussion
	DELETE FROM Comment WHERE DiscussionId IN (SELECT Id FROM Discussion WHERE CompanyId = @deleteId)
	DELETE FROM Discussion WHERE CompanyId = @deleteId

	-- Company Data
	INSERT INTO @metricTbl
	SELECT Id FROM Metric WHERE CompanyId = @deleteId

	DELETE FROM ExternalApiAuth WHERE CompanyId = @deleteId
	DELETE FROM Goal WHERE CompanyId = @deleteId
	DELETE FROM ExternalApiAuth WHERE CompanyId = @deleteId
	DELETE FROM MetricDataPoint WHERE MetricId IN (SELECT metricId FROM @metricTbl)
	DELETE FROM MetricForecast WHERE MetricId IN (SELECT metricId FROM @metricTbl)
	DELETE FROM Metric WHERE CompanyId = @deleteId
	DELETE FROM Area WHERE CompanyId = @deleteId
	DELETE FROM Recipient WHERE CompanyId = @deleteId

	DELETE FROM Subscription WHERE CompanyId = @deleteId
	DELETE FROM Webhook WHERE CompanyId = @deleteId
	DELETE FROM CompanyStats WHERE CompanyId = @deleteId

	-- Company
	DELETE FROM Company WHERE id=@deleteId

	select isnull(@deleteId,0) as deleteid