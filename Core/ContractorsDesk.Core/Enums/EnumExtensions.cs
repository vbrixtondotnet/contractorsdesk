namespace ContractorsDesk.Core.Enums
{
	public static class EnumExtensions
	{
		public static string GetStringValue(this Enum enumValue)
		{
			var type = enumValue.GetType();
			var memberInfo = type.GetMember(enumValue.ToString());
			if (memberInfo != null && memberInfo.Length > 0)
			{
				var attributes = memberInfo[0].GetCustomAttributes(typeof(StringValueAttribute), false);
				if (attributes != null && attributes.Length > 0)
				{
					return ((StringValueAttribute)attributes[0]).Value;
				}
			}
			return enumValue.ToString();
		}

		public static TEnum GetEnumValueFromString<TEnum>(string stringValue) where TEnum : Enum
		{
			var type = typeof(TEnum);
			foreach (var field in type.GetFields())
			{
				var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(field, typeof(StringValueAttribute));
				if (attribute != null && attribute.Value == stringValue)
				{
					return (TEnum)field.GetValue(null);
				}
			}
			throw new ArgumentException($"No enum found with string value '{stringValue}' in {typeof(TEnum).Name}");
		}
	}
}
