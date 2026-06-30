CREATE TABLE [dbo].[UserResetPasswordRequest]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
    [Email] NVARCHAR(250) NOT NULL,
    [DateSent] DATETIME NOT NULL,
    [SentStatus] NVARCHAR(100) NULL,
    [IsUsed] BIT NULL,
    [ResetLink] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_UserResetPasswordRequest] PRIMARY KEY CLUSTERED ([Id] ASC)
)
