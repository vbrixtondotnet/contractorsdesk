namespace ContractorsDesk.Core.Enums
{
	public enum Roles
	{
		[StringValue("Super Admin")]
		SuperAdmin = 1,
		[StringValue("Customer Support")]
		CustomerSupport = 2,
		[StringValue("Super IT")]
		SuperIT = 3,
		[StringValue("Company Owner")]
		CompanyOwner = 4,
		[StringValue("Project Manager")]
		ProjectManager = 5,
		[StringValue("Assistant Project Manager")]
		AssistantProjectManager = 6,
		[StringValue("Bookkeeper")]
		Bookkeeper = 7,
		[StringValue("Company IT")]
		CompanyIT = 8,
		[StringValue("Office Manager")]
		OfficeManager = 9,
		[StringValue("Client")]
		Client = 10
	}
}
