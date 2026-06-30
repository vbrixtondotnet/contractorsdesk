namespace ContractorsDesk.WebPortal.Models
{
	public class InputGroupModel
	{
		public string Type { get; set; } = "text";
		public string Label { get; set; } = string.Empty;
		public string LabelCssClass { get; set; } = string.Empty;
		public string CustomInputClass { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public string DataModel { get; set; } = string.Empty;
		public string CssClass { get; set; } = string.Empty;
		public bool ReadOnly {  get; set; } = false;
		public bool Required { get; set; } = false;
		public bool IsBookmarkTitle { get; set; } = false;
		public string OnInput { get; set; } = string.Empty;
		public string OnChange { get; set; } = string.Empty;
		public int TextAreaRows { get; set; } = 3;
		public List<DropdownOptionModel> DropdownOptions { get; set; } = new List<DropdownOptionModel>();
		public bool Multiple { get; set; } = false;
		public bool Disabled { get; set; }

		public bool IsSelect2 { get; set; } = false;
		public string DataType {  get; set; } = string.Empty;
		public string IsRequired { get { return this.Required ? "required" : ""; } }
		public string IsDisabled { get { return this.Disabled ? "disabled" : ""; } }
		public string IsReadOnly { get { return this.ReadOnly ? "readonly" : ""; } }
		public string IsMultiple { get { return this.Multiple ? "multiple" : ""; } }

		public string GetId() { return !string.IsNullOrEmpty(this.Id) ? $@"id=""{this.Id}""" : string.Empty; }
		public string GetName() { return !string.IsNullOrEmpty(this.Name) ? $@"name=""{this.Name}""" : string.Empty; }
		public string GetBookmarkTitle() { return this.IsBookmarkTitle ? $@"data-bookmark-input-title=""{this.IsBookmarkTitle}""" : string.Empty; }
		public string GetDataType() { return !string.IsNullOrEmpty(this.DataType) ? $@"data-type=""{this.DataType}""" : string.Empty; }
		public string GetDataModel() { return !string.IsNullOrEmpty(this.DataModel) ? $@"data-model=""{this.DataModel}""" : string.Empty; }
		public string GetEvtInput() { return !string.IsNullOrEmpty(this.OnInput) ? $@"evt-input=""{this.OnInput}""" : string.Empty; }
		public string GetEvtChange() { return !string.IsNullOrEmpty(this.OnChange) ? $@"evt-change=""{this.OnChange}""" : string.Empty; }
	}

	public class DropdownOptionModel
	{
		public string Value { get; set; } = string.Empty;
		public string Text { get; set; } = string.Empty;
		public bool Selected { get; set; } = false;
		public bool Disabled { get; set; } = false;
	}
}
