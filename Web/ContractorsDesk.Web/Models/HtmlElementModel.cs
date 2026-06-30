namespace ContractorsDesk.WebPortal.Models
{
	public class HtmlElementModel
	{
		public string Id { get; set; } = string.Empty;
		public string Classes { get; set; } = string.Empty;
		public bool Required { get; set; } = false;
		public string DataModel { get; set; }
		public string LabelCssClass {  get; set; }

	}
}
