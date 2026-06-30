namespace ContractorsDesk.WebPortal.Helpers
{
	public static class PageHelper
	{
		public static string IsActive(string pageName, string menuItemName)
		{
			return pageName == menuItemName ? "active" : "";
		}
		public static string IsActiveTab(string tabName, string param)
		{
			return param == tabName ? "active" : "";
		}
	}
}
