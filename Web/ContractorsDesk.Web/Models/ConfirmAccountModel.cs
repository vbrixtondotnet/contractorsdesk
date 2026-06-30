namespace ContractorsDesk.WebPortal.Models
{
	public class ConfirmAccountModel
	{
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? ConfirmPassword { get; set; }
		public string? ConfirmationCode { get; set; }
        public Dictionary<string, string> ModelError { get; set; } = new Dictionary<string, string>();

	}
}
