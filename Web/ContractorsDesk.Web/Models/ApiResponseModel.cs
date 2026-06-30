namespace ContractorsDesk.WebPortal.Models
{
	public class ApiResponseModel
	{
		public bool Success { get; set; }
		public string Message { get; private set; }
		public object? Data { get; private set; }
		public ApiResponseModel(string message, object? data = null)
		{
			this.Message = message;
			this.Data = data;
		}
	}
}
