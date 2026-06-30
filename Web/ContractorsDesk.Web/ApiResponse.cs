using ContractorsDesk.WebPortal.Models;

namespace ContractorsDesk.WebPortal
{
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public T Data { get; set; }
		public string ErrorMessage { get; set; }
		public PermissionViewModel Permissions { get; set; }
		public static ApiResponse<T> SuccessResponse() => new ApiResponse<T> { Success = true };
		public static ApiResponse<T> SuccessResponse(T data) => new ApiResponse<T> { Success = true, Data = data };
		public static ApiResponse<T> SuccessResponse(T data, PermissionViewModel permissions) => new ApiResponse<T> { Success = true, Data = data, Permissions = permissions };
		public static ApiResponse<T> ErrorResponse(string errorMessage) => new ApiResponse<T> { Success = false, ErrorMessage = errorMessage };
	}
}
