CREATE PROCEDURE [dbo].[spCDGetUserSentItems]
	@UserId int
AS
BEGIN
	DECLARE @SentItems Table
	(
		Id uniqueidentifier,
		ProjectId uniqueidentifier null,
		ProjectName nvarchar(100) null,
		MessageId uniqueidentifier,
		ReplyToMessageId uniqueidentifier,
		SenderName nvarchar(150),
		[From] nvarchar(150),
		[To] nvarchar(150),
		[Subject] nvarchar(150),
		[Body] nvarchar(max),
		DateCreated datetime,
		IsRead bit
	);

	INSERT INTO @SentItems
	select 
		sentitems.Id,
		sentitems.ProjectId,
		qbc.Name,
		sentitems.MessageId,
		null,	
		CONCAT(u.FirstName,' ', u.LastName),
		sentitems.[From],
		sentitems.[To],
		sentitems.[Subject],
		ISNULL(sentitems.Body,''),
		sentitems.DateCreated,
		1
	from Emails sentitems
	left join Users u
	on u.Id = @UserId
	left join QBClasses qbc
	on sentitems.ProjectId = qbc.ID
	where sentitems.SenderId = @UserId
	and sentitems.IsDeleted = 0
	and sentitems.IsArchived = 0

	SELECT * from @SentItems ORDER BY ProjectName, DateCreated DESC;

END