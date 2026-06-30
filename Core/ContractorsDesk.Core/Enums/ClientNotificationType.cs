namespace ContractorsDesk.Core.Enums
{
	public enum ClientNotificationType
	{
		[StringValue("DefaultGlobalNotification")]
		DefaultGlobalNotification = 1,
		[StringValue("DefaultUserNotification")]
		DefaultUserNotification = 2,
		[StringValue("GlobalNotification")]
		GlobalNotification = 3,
		[StringValue("UserNotification")]
		UserNotification = 4,
		[StringValue("ReloadActionItemDashboard")]
		ReloadActionItemDashboard = 5,
		[StringValue("OnActionItemCreated")]
		OnActionItemCreated = 6,
		[StringValue("OnNewEmailReceived")]
		OnNewEmailReceived = 7
	}
}
