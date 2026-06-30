CREATE PROCEDURE [dbo].[spCDGetUserInbox]  
 @UserId int  
AS  
BEGIN  
 DECLARE @Inbox Table  
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
  
 INSERT INTO @Inbox  
 select   
  inbox.Id,  
  sentitems.ProjectId,  
  qbc.Name,  
  inbox.MessageId,  
  inbox.ReplyToMessageId,   
  CASE WHEN inbox.SenderId IS NULL THEN cl.Name  
  ELSE CONCAT(u.FirstName, ' ', u.LastName)  
  END as SenderName,  
  inbox.[From],  
  inbox.[To],  
  inbox.[Subject],  
  ISNULL(inbox.Body,''),  
  inbox.DateCreated,  
  inbox.IsRead  
 from   
 Emails inbox  
 inner join Emails sentitems  
 on inbox.ReplyToMessageId = sentitems.MessageId  
 left join QBClasses qbc  
 on qbc.ID = sentitems.ProjectId  
 left join Proposals p  
 on p.QBClassId = qbc.ID  
 left join Clients cl  
 on cl.ID = p.ClientId  
 left join Users u  
 on u.ID = inbox.SenderId  
 where sentitems.SenderId = @UserId  
 and inbox.IsDeleted = 0  
 and inbox.IsArchived = 0  
  
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
 where sentitems.MessageId in (SELECT ReplyToMessageId from @Inbox)  
 and sentitems.IsDeleted = 0  
 and sentitems.IsArchived = 0  
  
 SELECT *   
  FROM (  
   SELECT * FROM @Inbox --WHERE RowNum = 1  
   UNION ALL  
   SELECT * FROM @SentItems  
  ) AS Combined  
  ORDER BY   
   ProjectName,  
   --CASE WHEN IsRead = 1 THEN 1 ELSE 0 END,  -- Prioritize IsRead = 1  
   DateCreated DESC;  
  
END