--select * from Emails
DECLARE @UserId int;
DECLARE @Inbox Table
(
	ProjectId uniqueidentifier null,
	MessageId uniqueidentifier,
	ReplyToMessageId uniqueidentifier,
	[From] nvarchar(150),
	[To] nvarchar(150),
	[Subject] nvarchar(150),
	[Body] nvarchar(max),
	DateCreated datetime,
	IsRead bit
);
DECLARE @SentItems Table
(
	ProjectId uniqueidentifier null,
	MessageId uniqueidentifier,
	ReplyToMessageId uniqueidentifier,
	[From] nvarchar(150),
	[To] nvarchar(150),
	[Subject] nvarchar(150),
	[Body] nvarchar(max),
	DateCreated datetime,
	IsRead bit
);

SET @UserId = 55;

INSERT INTO @Inbox
select 
	sentitems.ProjectId,
	inbox.MessageId,
	inbox.ReplyToMessageId,	
	inbox.[From],
	inbox.[To],
	inbox.[Subject],
	inbox.Body,
	inbox.DateCreated,
	inbox.IsRead
from 
Emails inbox
inner join Emails sentitems
on inbox.ReplyToMessageId = sentitems.MessageId
where sentitems.SenderId = @UserId

INSERT INTO @SentItems
select 
	sentitems.ProjectId,
	sentitems.MessageId,
	null,	
	sentitems.[From],
	sentitems.[To],
	sentitems.[Subject],
	sentitems.Body,
	sentitems.DateCreated,
	1
from Emails sentitems
where sentitems.MessageId in (SELECT ReplyToMessageId from @Inbox)

SELECT * from @Inbox
UNION ALL
SELECT * FROM @SentItems
order by DateCreated desc

--select * from Emails where MessageId = 'CAFpE4jEPQwsi9QmPdEPVb1CJ6rtRkc6JGmYmdX5TQ9jF3jESug'