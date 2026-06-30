namespace ContractorsDesk.Core.Enums
{
	public class StringValueAttribute : Attribute
	{
		public string Value { get; }

		public StringValueAttribute(string value)
		{
			Value = value;
		}
	}
}
