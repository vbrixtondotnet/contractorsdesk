namespace ContractorsDesk.WebPortal.Models
{
	public class ResetPasswordModel
	{
        public Guid? Id { get; set; }
        public string? Token { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? ConfirmPassword { get; set; }
        public Dictionary<string, string> ModelError { get; set; } = new Dictionary<string, string>();

	}
}
